using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SharpServer.App.Core
{
    public class Router
    {
        public delegate HttpResponse RouteHandler(HttpRequest request, Dictionary<string, string> routeParams);

        private readonly List<RouteDefinition> _routes = new();

        private class RouteDefinition
        {
            public string Method { get; set; }
            public Regex Pattern { get; set; }
            public RouteHandler Handler { get; set; }
            public List<string> ParameterNames { get; set; }
        }

        public void Register(string method, string pathPattern, RouteHandler handler)
        {
            var paramNames = new List<string>();
            string regexPattern = "^" + Regex.Replace(pathPattern, @"\{(\w+)\}", match =>
            {
                paramNames.Add(match.Groups[1].Value);
                return @"([^\/]+)";
            }) + "$";

            _routes.Add(new RouteDefinition
            {
                Method = method.ToUpper(),
                Pattern = new Regex(regexPattern),
                Handler = handler,
                ParameterNames = paramNames
            });
        }

        public HttpResponse Route(HttpRequest request)
        {
            foreach (var route in _routes)
            {
                if (route.Method != request.Method.ToUpper()) continue;

                var match = route.Pattern.Match(request.Path);
                if (match.Success)
                {
                    var routeParams = new Dictionary<string, string>();
                    for (int i = 0; i < route.ParameterNames.Count; i++)
                    {
                        routeParams[route.ParameterNames[i]] = match.Groups[i + 1].Value;
                    }

                    return route.Handler(request, routeParams);
                }
            }

            return HttpResponse.NotFound($"No route matched {request.Method} {request.Path}");
        }
    }
}
