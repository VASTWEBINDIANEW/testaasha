using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Vastwebmulti.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace Vastwebmulti.Filters
{
    public class MenuAccessFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string actionName = filterContext.ActionDescriptor.ActionName;
            string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            bool isAccessAllowed = false;
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                var userid = filterContext.HttpContext.User.Identity.GetUserId();

                if (userid != null)
                {
                    if (actionName == "AEPS")
                    {
                        var PriceList = db.PaidServicesChargeLists.FirstOrDefault(a => a.ServiceName == "AEPS");
                        if (PriceList != null)
                        {
                            if (PriceList.IsFree)
                            {
                                isAccessAllowed = true;
                            }
                            else
                            {
                                var entry = db.PaidServicesPaymentHistories.OrderByDescending(a=>a.Idno).FirstOrDefault(a => a.UserId == userid && a.ServiceName == "AEPS");
                                if (entry != null && entry.ExpiryDate > DateTime.Now)
                                {
                                    isAccessAllowed = true;
                                }
                            }
                        }

                    }
                    if (actionName == "Money_transfer1")
                    {
                        var PriceList = db.PaidServicesChargeLists.FirstOrDefault(a => a.ServiceName == "DMT");
                        if (PriceList != null)
                        {
                            if (PriceList.IsFree)
                            {
                                isAccessAllowed = true;
                            }
                            else
                            {
                                var entry = db.PaidServicesPaymentHistories.OrderByDescending(a => a.Idno).FirstOrDefault(a => a.UserId == userid && a.ServiceName == "DMT");
                                if (entry != null && entry.ExpiryDate > DateTime.Now)
                                {
                                    isAccessAllowed = true;
                                }
                            }
                        }

                    }
                    if (actionName == "Travel")
                    {
                        var PriceList = db.PaidServicesChargeLists.FirstOrDefault(a => a.ServiceName == "FLIGHT");
                        if (PriceList != null)
                        {
                            if (PriceList.IsFree)
                            {
                                isAccessAllowed = true;
                            }
                            else
                            {
                                var entry = db.PaidServicesPaymentHistories.OrderByDescending(a => a.Idno).FirstOrDefault(a => a.UserId == userid && a.ServiceName == "FLIGHT");
                                if (entry != null && entry.ExpiryDate > DateTime.Now)
                                {
                                    isAccessAllowed = true;
                                }
                            }
                        }

                    }
                    if (actionName == "Index" && controllerName == "PANCARD")
                    {
                        var PriceList = db.PaidServicesChargeLists.FirstOrDefault(a => a.ServiceName == "PANCARD");
                        if (PriceList != null)
                        {
                            if (PriceList.IsFree)
                            {
                                isAccessAllowed = true;
                            }
                            else
                            {
                                var entry = db.PaidServicesPaymentHistories.OrderByDescending(a => a.Idno).FirstOrDefault(a => a.UserId == userid && a.ServiceName == "PANCARD");
                                if (entry != null && entry.ExpiryDate > DateTime.Now)
                                {
                                    isAccessAllowed = true;
                                    //filterContext.Result = new RedirectToRouteResult(
                                    // new RouteValueDictionary
                                    // {
                                    //    { "controller", controllerName },
                                    //    { "action", "Index" }
                                    // });
                                }
                            }
                        }

                    }
                    if (actionName == "Index" && controllerName == "ECommerce")
                    {
                        var PriceList = db.PaidServicesChargeLists.FirstOrDefault(a => a.ServiceName == "ECOMM");
                        if (PriceList != null)
                        {
                            if (PriceList.IsFree)
                            {
                                isAccessAllowed = true;
                            }
                            else
                            {
                                var entry = db.PaidServicesPaymentHistories.OrderByDescending(a => a.Idno).FirstOrDefault(a => a.UserId == userid && a.ServiceName == "ECOMM");
                                if (entry != null && entry.ExpiryDate > DateTime.Now)
                                {
                                    isAccessAllowed = true;
                                    //filterContext.Result = new RedirectToRouteResult(
                                    // new RouteValueDictionary
                                    // {
                                    //    { "controller", controllerName },
                                    //    { "action", "Index" }
                                    // });
                                }
                            }
                        }

                    }
                    if (actionName == "MPOS")
                    {
                        var PriceList = db.PaidServicesChargeLists.FirstOrDefault(a => a.ServiceName == "MPOSH");
                        if (PriceList != null)
                        {
                            if (PriceList.IsFree)
                            {
                                isAccessAllowed = true;
                            }
                            else
                            {
                                var entry = db.PaidServicesPaymentHistories.OrderByDescending(a => a.Idno).FirstOrDefault(a => a.UserId == userid && a.ServiceName == "MPOSH");
                                if (entry != null && entry.ExpiryDate > DateTime.Now)
                                {
                                    isAccessAllowed = true;
                                }
                            }
                        }

                    }
                    if (actionName == "SchoolFees")
                    {
                        var PriceList = db.PaidServicesChargeLists.FirstOrDefault(a => a.ServiceName == "SchoolFee");
                        if (PriceList != null)
                        {
                            if (PriceList.IsFree)
                            {
                                isAccessAllowed = true;
                            }
                            else
                            {
                                var entry = db.PaidServicesPaymentHistories.OrderByDescending(a => a.Idno).FirstOrDefault(a => a.UserId == userid && a.ServiceName == "SchoolFee");
                                if (entry != null && entry.ExpiryDate > DateTime.Now)
                                {
                                    isAccessAllowed = true;
                                }
                            }
                        }

                    }

                    if (actionName == "Gift_Card")
                    {
                        var PriceList = db.PaidServicesChargeLists.FirstOrDefault(a => a.ServiceName == "GIFTCARD");
                        if (PriceList != null)
                        {
                            if (PriceList.IsFree)
                            {
                                isAccessAllowed = true;
                            }
                            else
                            {
                                var entry = db.PaidServicesPaymentHistories.OrderByDescending(a => a.Idno).FirstOrDefault(a => a.UserId == userid && a.ServiceName == "GIFTCARD");
                                if (entry != null && entry.ExpiryDate > DateTime.Now)
                                {
                                    isAccessAllowed = true;
                                }
                            }
                        }

                    }
                }
            }
            if (!isAccessAllowed)
            {
                filterContext.Result = new RedirectToRouteResult(
                              new RouteValueDictionary
                              {
                                { "controller", "Home" },
                                { "action", "PaidServicePayment" }
                              });
            }

        }
    }
}