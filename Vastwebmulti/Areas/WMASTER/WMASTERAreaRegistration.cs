using System.Web.Mvc;

namespace Vastwebmulti.Areas.WMASTER
{
    public class WMASTERAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "WMASTER";
            }
        }
        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "WMASTER_default",
                "WMASTER/{controller}/{action}/{id}",
                new { action = "Dashboard", id = UrlParameter.Optional }
            );
        }

    }
}