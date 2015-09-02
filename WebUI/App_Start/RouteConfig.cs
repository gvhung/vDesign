using System.Web.Mvc;
using System.Web.Routing;
using WebUI.Models;

namespace WebUI
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Dashboard",
                url: "Dashboard",
                defaults: new { controller = "Home", action = "Index", area = "" },
                namespaces: new string[] { "WebUI.Controllers" }
            );

            routes.MapRoute(
                name: "Standart",
                url: "Entities/{mnemonic}",
                defaults: new { controller = "Standart", action = "Index", mnemonic = UrlParameter.Optional }
            );


            routes.MapRoute(
                name: "ViewModel",
                url: "EntityType/{mnemonic}-{typeDialog}-{id}",
                defaults: new { controller = "Standart", action = "GetViewModel", mnemonic = UrlParameter.Optional },
                namespaces: new string[] { "WebUI.Controllers" }
            );

            routes.MapRoute(
                name: "Files",
                url: "files/{folder}/{fileName}.xml",
                defaults: new { controller = "Files", action = "GetXml" },
                namespaces: new string[] { "WebUI.Controllers" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "WebUI.Controllers" }
            );

            


        }
    }
}