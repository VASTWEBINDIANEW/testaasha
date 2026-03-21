using System.Web.Mvc;

namespace Vastwebmulti.Areas.FeeCollector
{
    public class FeeCollectorAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "FeeCollector";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "FeeCollector_default",
                "FeeCollector/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}