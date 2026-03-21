using System.Web.Mvc;

namespace Vastwebmulti.Areas.RETAILER
{
    public class RETAILERAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "RETAILER";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "RETAILER_default",
                "RETAILER/{controller}/{action}/{id}",
                new { action = "Dashboard", id = UrlParameter.Optional }
            );
        }
    }
}