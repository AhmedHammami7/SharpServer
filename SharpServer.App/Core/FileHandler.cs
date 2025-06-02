using SharpServer.App.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SharpServe.Core
{
    public class FileHandler
    {
        private static readonly Dictionary<string, string> MimeTypes = new()
        {
            [".html"] = "text/html",
            [".css"] = "text/css",
            [".js"] = "application/javascript",
            [".json"] = "application/json",
            [".png"] = "image/png",
            [".jpg"] = "image/jpeg",
            [".jpeg"] = "image/jpeg",
            [".gif"] = "image/gif",
            [".ico"] = "image/x-icon",
            [".svg"] = "image/svg+xml",
            [".txt"] = "text/plain"
        };

        public static HttpResponse ServeStatic(string path)
        {
            string wwwroot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            string filePath = Path.Combine(wwwroot, path.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));

            if (!File.Exists(filePath))
                return HttpResponse.NotFound("Static file not found");

            string ext = Path.GetExtension(filePath).ToLowerInvariant();
            string contentType = MimeTypes.ContainsKey(ext) ? MimeTypes[ext] : "application/octet-stream";

            byte[] contentBytes = File.ReadAllBytes(filePath);
            string content = Encoding.UTF8.GetString(contentBytes);

            var response = new HttpResponse
            {
                ContentType = contentType,
                Body = content
            };

            response.Headers["Cache-Control"] = "public, max-age=3600";
            response.Headers["ETag"] = GenerateETag(contentBytes);

            return response;
        }

        private static string GenerateETag(byte[] content)
        {
            int hash = BitConverter.ToInt32(System.Security.Cryptography.MD5.Create().ComputeHash(content), 0);
            return $"\"{hash}\"";
        }
    }
}
