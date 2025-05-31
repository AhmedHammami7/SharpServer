using System.Net.Sockets;
using System.Text;

namespace SharpServer.App.Core
{
    public class RequestHandler
    {
        public void ProcessRequest(TcpClient client)
        {
            using NetworkStream stream = client.GetStream();

            byte[] buffer = new byte[4096];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            string requestText = Encoding.UTF8.GetString(buffer, 0, bytesRead);

            Console.WriteLine("[Request Received]");
            Console.WriteLine(requestText);

            string responseText =
                "HTTP/1.1 200 OK\r\n" +
                "Content-Type: text/plain\r\n" +
                "Content-Length: 13\r\n" +
                "Connection: close\r\n" +
                "\r\n" +
                "Hello, World!";

            byte[] responseBytes = Encoding.UTF8.GetBytes(responseText);
            stream.Write(responseBytes, 0, responseBytes.Length);

            client.Close();
        }
    }
}