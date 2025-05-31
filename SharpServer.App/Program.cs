using SharpServer.App.Core;

class Program
{
    static void Main(string[] args)
    {
        var server = new HttpServer(port: 8080);
        server.Start();
    }
}