using System.Web.Mvc;

namespace Vastwebmulti.Areas.WHITELABEL
{
    public class WHITELABELAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "WHITELABEL";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "WHITELABEL_default",
                "WHITELABEL/{controller}/{action}/{id}",
                new { action = "Dashboard", id = UrlParameter.Optional }
            );
        }
    }
}