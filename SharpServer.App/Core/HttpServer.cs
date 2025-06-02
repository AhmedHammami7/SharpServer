using SharpServe.Core;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading;

namespace SharpServer.App.Core
{
    public class HttpServer
    {
        private readonly int _port;
        private readonly TcpListener _listener;
        private readonly Router _router = new();

        public HttpServer(int port)
        {
            _port = port;
            _listener = new TcpListener(IPAddress.Any, _port);
            RegisterRoutes();

        }
        private void RegisterRoutes()
        {
            _router.Register("GET", "/", (req, _) =>
                new HttpResponse { Body = "Welcome to SharpServer!" });

            _router.Register("GET", "/hello", (req, _) =>
                new HttpResponse { Body = $"Hello {req.Query.GetValueOrDefault("name", "World")}!" });

            _router.Register("GET", "/users/{id}", (req, routeParams) =>
                new HttpResponse { Body = $"User ID: {routeParams["id"]}" });

            _router.Register("POST", "/submit", (req, _) =>
            {
                if (req.Form.TryGetValue("username", out var username))
                    return new HttpResponse { Body = $"Form received: Hello, {username}" };

                return new HttpResponse { Body = "No username found in form data" };
            });

            _router.Register("POST", "/json", (req, _) =>
            {
                if (req.Json is JsonElement json && json.TryGetProperty("name", out var name))
                    return new HttpResponse { Body = $"Hello from JSON, {name.GetString()}" };

                return new HttpResponse { Body = "Invalid JSON" };
            });

            _router.Register("PUT", "/users/{id}", (req, routeParams) =>
            {
                return new HttpResponse { Body = $"Updated user {routeParams["id"]} with data: {req.BodyText}" };
            });

            _router.Register("DELETE", "/users/{id}", (req, routeParams) =>
            {
                return new HttpResponse { Body = $"User {routeParams["id"]} deleted successfully." };
            });
            // Static file catch-all (MUST be last)
            _router.Register("GET", "/static/{*path}", (req, routeParams) =>
            {
                string filePath = req.Path.Replace("/static", "");
                return FileHandler.ServeStatic(filePath);
            });
        }
        public void Start()
        {
            _listener.Start();
            Console.WriteLine($"[Server] Listening on port {_port}...");

            while (true)
            {
                TcpClient client = _listener.AcceptTcpClient();
                Thread clientThread = new Thread(() =>
                {
                    var handler = new RequestHandler(_router);
                    handler.ProcessRequest(client);
                });
                clientThread.Start();
            }

        }
    }
}
