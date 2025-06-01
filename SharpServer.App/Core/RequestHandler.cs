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

            HttpRequest request = HttpRequest.Parse(requestText);

            Console.WriteLine($"[Parsed Request] {request.Method} {request.Path} {request.Version}");

            foreach (var header in request.Headers)
                Console.WriteLine($"{header.Key}: {header.Value}");



            string responseBody = $"Method: {request.Method}\nPath: {request.Path}\n";


            if (request.Query.Count > 0)
            {
                responseBody += "Query Parameters:\n";
                foreach (var kvp in request.Query)
                    responseBody += $"- {kvp.Key} = {kvp.Value}\n";
            }
            string responseText =
                "HTTP/1.1 200 OK\r\n" +
                "Content-Type: text/plain\r\n" +
                $"Content-Length: {Encoding.UTF8.GetByteCount(responseBody)}\r\n" +
                "Connection: close\r\n" +
                "\r\n" +
                responseBody;

            byte[] responseBytes = Encoding.UTF8.GetBytes(responseText);
            stream.Write(responseBytes, 0, responseBytes.Length);

            client.Close();
        }
    }
}