using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.Employee.Model
{
    public class PermissioncheckingAttribute: ActionFilterAttribute, IActionFilter
    {
        public string userid { get; set; }
        public string permision { get; set; }
        public string servicename { get; set; }
      
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
          

            //your validation here..
            //for example:
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {

                 string userName = null;
                if (filterContext.HttpContext.User.Identity.IsAuthenticated)
                {
                    userName = filterContext.HttpContext.User.Identity.Name;

                    //ERROR***
                    userid = filterContext.HttpContext.User.Identity.GetUserId();

                }
                else
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Account" }, { "action", "LogOff" } });
                    
                }
              
                var req = filterContext.HttpContext.Request;
                bool userpermision= false;
                if (permision == "Read")
                {
                     userpermision = db.tbl_permission_Services.Where(x => x.Employee_ID == userid && x.Servicename == servicename && x.isRead == true).Any();

                }
                else if (permision == "Write")
                {
                     userpermision = db.tbl_permission_Services.Where(x => x.Employee_ID == userid && x.Servicename == servicename && x.iswrite == true).Any();

                }

                if (userpermision)
                {
                  
                    //  filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Home" }, { "action", "Operator_report_new" }, { "returnUri", filterContext.HttpContext.Request.RawUrl } });
                }
                else
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Home" }, { "action", "Dashboard" } });
                }
                base.OnActionExecuting(filterContext);



            }
            //    if (_currentUser.Role < RoleHasAccess)
            //{
            //    //user has not access to this action, redirect him to home page. 

            //  //  filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Home" }, { "action", "Index" }, { "returnUri", filterContext.HttpContext.Request.RawUrl } });
            //}
            //else
            //{
            //    // user has access to this action
            //}
        }
    }
}