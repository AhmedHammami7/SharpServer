using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
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
        public string BodyText { get; set; } = "";
        public Dictionary<string, string> Form { get; set; } = new();
        public JsonElement? Json { get; set; }


        public static HttpRequest Parse(string rawRequest)
        {
            var request = new HttpRequest();
            request.RawRequest = rawRequest;

            string[] sections = rawRequest.Split(new[] { "\r\n\r\n" }, 2, StringSplitOptions.None);
            string headerSection = sections[0];
            request.BodyText = sections.Length > 1 ? sections[1] : "";

            var lines = headerSection.Split("\r\n", StringSplitOptions.None);
            if (lines.Length == 0)
                throw new Exception("Invalid HTTP request");


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
                int idx = line.IndexOf(':');
                if (idx > 0)
                {
                    string key = line.Substring(0, idx).Trim();
                    string value = line.Substring(idx + 1).Trim();
                    request.Headers[key] = value;
                }
            }

            if (!string.IsNullOrWhiteSpace(request.BodyText) && request.Headers.TryGetValue("Content-Type", out var contentType))
            {
                if (contentType.StartsWith("application/x-www-form-urlencoded"))
                {
                    var form = HttpUtility.ParseQueryString(request.BodyText);
                    foreach (string key in form)
                        request.Form[key] = form[key];
                }
                else if (contentType.StartsWith("application/json"))
                {
                    try
                    {
                        request.Json = JsonSerializer.Deserialize<JsonElement>(request.BodyText);
                    }
                    catch
                    {
                        // Ignore bad JSON
                    }
                }
            }


            return request;

        }

    }
}
