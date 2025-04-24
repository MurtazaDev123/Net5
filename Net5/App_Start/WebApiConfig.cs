using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace SmartEcommerce
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(name: "DefaultApi", routeTemplate: "api/{controller}/{action}/{id}", defaults: new { id = RouteParameter.Optional } );
            config.Formatters.JsonFormatter.SerializerSettings =
                 new Newtonsoft.Json.JsonSerializerSettings { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore };
        }
    }
}
