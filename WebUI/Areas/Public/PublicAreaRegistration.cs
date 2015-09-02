using System.Web.Mvc;

namespace WebUI.Areas.Public
{
    public class PublicAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Public";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
            "Public_home",
                "",
             new { action = "Index", controller = "Home", area = "Public", id = UrlParameter.Optional }
            );

            context.MapRoute(
                 name: "Public_default",
                 url: "Public/{controller}/{action}/{id}",
                 defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

        }
    }
}