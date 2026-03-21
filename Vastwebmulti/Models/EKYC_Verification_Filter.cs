using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace Vastwebmulti.Models
{
    public class EKYC_Verification_Filter : ActionFilterAttribute, IActionFilter
    {
        public string userid { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string actionName = filterContext.ActionDescriptor.ActionName;
            //string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            var userid = filterContext.HttpContext.User.Identity.GetUserId();

            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                dynamic res = EKYC_Varification_Status(userid);

                var aadhar = (bool)res.aadhar;
                var pan = (bool)res.pan;
                var v_type = (string)res.v_type;

                if (actionName == "PanVerification" || actionName == "AadharVerificationOtpSent" || actionName == "AadharVerificationOtpVerify")
                {
                    return;
                }

                bool isTrue = aadhar && pan;

                if (v_type != "all")
                {
                    if (v_type == "aadhar")
                    {
                        isTrue = aadhar;
                    }
                    else if (v_type == "pan")
                    {
                        isTrue = pan;
                    }
                }

                if (isTrue)
                {
                    if (actionName == "Ekyc_Verification")
                    {
                        filterContext.Result = new RedirectToRouteResult(
                                      new RouteValueDictionary {
                                           { "controller", "Home" },
                                           { "action", "/" }});
                    }
                    else
                    {
                        return;
                    }

                }
                else
                {
                    if (actionName == "Ekyc_Verification")
                    {
                        return;
                    }
                    else
                    {
                        filterContext.Result = new RedirectToRouteResult(
                                      new RouteValueDictionary {
                                           { "controller", "Home" },
                                           { "action", "Ekyc_Verification" }});
                    }
                }

            }
        }

        public object EKYC_Varification_Status(string userid)
        {
            bool aadhar = true;
            bool pan = true;
            var v_type = "";
            try
            {
                VastwebmultiEntities db = new VastwebmultiEntities();

                var retailer_exist = db.Retailer_Details.Any(s => s.RetailerId == userid);
                if (!retailer_exist)
                {
                    throw new Exception("Retailer Not Found");
                }

                var verify_data = db.AAdharPanEKYC_Verify.Single();
                v_type = verify_data.verify_type;

                var retailer_verification_data = db.Retailer_Details.Where(s => s.RetailerId == userid).Single();
                if (verify_data.type == "off")
                {
                }
                else if (retailer_verification_data.CretedBy == "Signup")
                {
                    aadhar = (bool)retailer_verification_data.aadhar_verification;
                    pan = (bool)retailer_verification_data.pan_verification;
                }
                else
                {
                    if (verify_data.type == "all")
                    {
                        aadhar = (bool)retailer_verification_data.aadhar_verification;
                        pan = (bool)retailer_verification_data.pan_verification;
                    }
                }

                var obj = new
                {
                    status = true,
                    aadhar,
                    pan,
                    v_type
                };

                return obj;

            }
            catch (Exception ex)
            {
                var obj = new
                {
                    status = false,
                    message = ex.Message,
                    aadhar = false,
                    pan = false,
                    v_type = false
                };

                return obj;
            }
        }
    }
}