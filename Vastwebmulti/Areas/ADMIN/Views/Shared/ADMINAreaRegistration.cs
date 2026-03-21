using System.Web.Mvc;

namespace Vastwebmulti.Areas.ADMIN
{
    public class ADMINAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "ADMIN";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "ADMIN_default",
                "ADMIN/{controller}/{action}/{id}",
                new { action = "Dashboard", id = UrlParameter.Optional }
            );
        }
    }
}