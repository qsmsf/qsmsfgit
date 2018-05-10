using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Cors;

namespace FineUIMvc.QuickStart
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // 允许Web API跨域访问
            var cors = new EnableCorsAttribute(
                    origins: "*",
                    headers: "*",
                    methods: "*"
                );
            config.EnableCors(cors);

            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );            
        }
    }
}
