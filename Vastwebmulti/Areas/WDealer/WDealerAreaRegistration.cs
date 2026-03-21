using System.Web.Mvc;

namespace Vastwebmulti.Areas.WDealer
{
    public class WDealerAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "WDealer";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "WDealer_default",
                "WDealer/{controller}/{action}/{id}",
                new { action = "Dashboard", id = UrlParameter.Optional }
            );
        }
    }
}