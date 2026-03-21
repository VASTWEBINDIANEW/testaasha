using Microsoft.AspNet.Identity;
using Quartz;
using Quartz.Impl;
using System;
using System.IO;
using System.Security.Principal;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Vastwebmulti.Models;
using Vastwebmulti.Models.Scheduling;

namespace Vastwebmulti
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(config => config.MapHttpAttributeRoutes());
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            SchedulerService.StartAsync().GetAwaiter().GetResult();
            //Removing xml formatting
            GlobalConfiguration.Configuration.Formatters.Remove(GlobalConfiguration.Configuration.Formatters.XmlFormatter);
  
            //The Schedular Class


        }
        protected void Application_BeginRequest()
        {
            // Only disable caching for dynamic pages (HTML responses)
            // Allow browser to cache static files (CSS, JS, images) for faster repeat loads
            string path = Request.AppRelativeCurrentExecutionFilePath ?? "";
            bool isStaticFile = path.EndsWith(".css") || path.EndsWith(".js") ||
                                path.EndsWith(".png") || path.EndsWith(".jpg") ||
                                path.EndsWith(".jpeg") || path.EndsWith(".gif") ||
                                path.EndsWith(".svg") || path.EndsWith(".ico") ||
                                path.EndsWith(".woff") || path.EndsWith(".woff2") ||
                                path.EndsWith(".ttf") || path.EndsWith(".eot");
            if (!isStaticFile)
            {
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
                Response.Cache.SetNoStore();
            }
        }
    }
}
