using System.Web.Mvc;

namespace Vastwebmulti.Areas.DEALER
{
    public class DEALERAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "DEALER";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "DEALER_default",
                "DEALER/{controller}/{action}/{id}",
                new { action = "Dashboard", id = UrlParameter.Optional }
            );
        }
    }
}