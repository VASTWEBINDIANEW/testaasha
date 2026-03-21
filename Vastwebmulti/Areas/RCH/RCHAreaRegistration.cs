using System.Web.Mvc;

namespace Vastwebmulti.Areas.RCH
{
    public class RCHAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "RCH";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "RCH_default",
                "RCH/{controller}/{action}/{id}",
                new { action = "Deshboard", id = UrlParameter.Optional }
            );
        }
    }
}