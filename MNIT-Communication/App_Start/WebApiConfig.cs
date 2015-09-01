using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using MNIT_Communication.Areas.api;
using Newtonsoft.Json;

namespace MNIT_Communication.App_Start
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Formatters.JsonFormatter.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local;

            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();
            
            config.Routes.MapHttpRoute(
                name: "ConfirmApi",
                routeTemplate: "api/{controller}/{action}/{id}/{secret}"
            );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            
        }
    }
}