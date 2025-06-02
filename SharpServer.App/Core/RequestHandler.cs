using System.Net.Sockets;
using System.Text;

namespace SharpServer.App.Core
{
    public class RequestHandler
    {
        private readonly Router _router;

        public RequestHandler(Router router)
        {
            _router = router;
        }

        public void ProcessRequest(TcpClient client)
        {
            using NetworkStream stream = client.GetStream();

            byte[] buffer = new byte[4096];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            string requestText = Encoding.UTF8.GetString(buffer, 0, bytesRead);

            HttpRequest request = HttpRequest.Parse(requestText);
            HttpResponse response = _router.Route(request);



            byte[] responseBytes = Encoding.UTF8.GetBytes(response.Build());
            stream.Write(responseBytes, 0, responseBytes.Length);

            client.Close();
        }
    }
}