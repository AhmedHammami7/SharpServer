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

        public HttpServer(int port)
        {
            _port = port;
            _listener = new TcpListener(IPAddress.Any, _port);
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
                    var handler = new RequestHandler();
                    handler.ProcessRequest(client);
                });
                clientThread.Start();
            }

        }
    }
}
