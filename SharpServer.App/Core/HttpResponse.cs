using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpServer.App.Core
{
    public class HttpResponse
    {
        public int StatusCode { get; set; } = 200;
        public string StatusText { get; set; } = "OK";
        public string ContentType { get; set; } = "text/plain";
        public string Body { get; set; } = "";

        public Dictionary<string, string> Headers { get; set; } = new();


        public string Build()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"HTTP/1.1 {StatusCode} {StatusText}");
            stringBuilder.AppendLine($"Content-Type: {ContentType}");
            stringBuilder.AppendLine($"Content-Length: {Encoding.UTF8.GetByteCount(Body)}");

            foreach (var header in Headers)
                stringBuilder.AppendLine($"{header.Key}: {header.Value}");

            stringBuilder.AppendLine(); // empty line before body
            stringBuilder.Append(Body);

            return stringBuilder.ToString();
        }

        public static HttpResponse NotFound(string message = "404 Not Found")
        {
            return new HttpResponse
            {
                StatusCode = 404,
                StatusText = "Not Found",
                Body = message
            };
        }
    }
}
