using System;
using System.Net;
using System.Net.Sockets;
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
