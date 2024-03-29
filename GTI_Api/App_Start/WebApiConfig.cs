﻿using Newtonsoft.Json.Serialization;
using System.Web.Http;
using System.Web.Http.Cors;


namespace GTI_Api {
    public static class WebApiConfig {
        public static void Register(HttpConfiguration config) {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
               name: "GetImovelDistrito",
               routeTemplate: "Imovel/GetImovelInscricao/{distrito}",
               defaults: new { controller = "Imovel", action = "GetImovelInscricao" }
           );
            config.Routes.MapHttpRoute(
               name: "GetImovelId",
               routeTemplate: "Imovel/GetImovelId/{id}",
               defaults: new { controller = "Imovel", action = "GetImovelId" }
           );



            config.Formatters.JsonFormatter.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            //var jsonpFormatter = new JsonpMediaTypeFormatter(config.Formatters.JsonFormatter);
            //config.Formatters.Insert(0, jsonpFormatter);

            EnableCorsAttribute cors = new EnableCorsAttribute("*", "*", "*");
            config.EnableCors(cors);


        }
    }
}
