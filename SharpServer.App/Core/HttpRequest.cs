using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SharpServer.App.Core
{
    public class HttpRequest
    {
        public string Method { get; set; }
        public string Path { get; set; }
        public string Version { get; set; }
        public Dictionary<string, string> Headers { get; set; } = new();
        public Dictionary<string, string> Query { get; set; } = new();
        public string RawRequest { get; set; }


        public static HttpRequest Parse(string rawRequest)
        {
            var request = new HttpRequest();
            request.RawRequest = rawRequest;
            var lines = rawRequest.Split("\r\n", StringSplitOptions.None);
            if (lines.Length == 0)
                throw new Exception("Invalid HTTP request: No lines found");


            string[] requestLineParts = lines[0].Split(' ');
            if (requestLineParts.Length != 3)
                throw new Exception("Invalid request line format");


            request.Method = requestLineParts[0];
            string fullPath = requestLineParts[1];
            request.Version = requestLineParts[2];



            var uri = new Uri("http://localhost" + fullPath); 
            request.Path = uri.AbsolutePath;


            var query = HttpUtility.ParseQueryString(uri.Query);
            foreach (string key in query)
                request.Query[key] = query[key];


            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i];
                if (string.IsNullOrWhiteSpace(line)) break; 

                var separatorIndex = line.IndexOf(':');
                if (separatorIndex > 0)
                {
                    string headerName = line.Substring(0, separatorIndex).Trim();
                    string headerValue = line.Substring(separatorIndex + 1).Trim();
                    request.Headers[headerName] = headerValue;
                }
            }

            return request;

        }

    }
}
