using System.Web.Mvc;

namespace Vastwebmulti.Areas.VENDOR
{
    public class VENDORAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "VENDOR";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "VENDOR_default",
                "VENDOR/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}