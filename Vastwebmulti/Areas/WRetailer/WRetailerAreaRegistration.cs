using System.Web.Mvc;

namespace Vastwebmulti.Areas.WRetailer
{
    public class WRetailerAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "WRetailer";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "WRetailer_default",
                "WRetailer/{controller}/{action}/{id}",
                new { action = "Dashboard", id = UrlParameter.Optional }
            );
        }
    }
}