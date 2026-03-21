using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace Vastwebmulti.Models
{
    /// <summary>
    /// Action filter enforcing passcode/security PIN verification for protected admin actions
    /// </summary>
    public class CutomAttributforpasscodeset : ActionFilterAttribute, IActionFilter
    {
        public string userid { get; set; }
        public string permision { get; set; }
        public string servicename { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string actionName = filterContext.ActionDescriptor.ActionName;
            string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            CommUtilEmail emailsend = new CommUtilEmail();
            ALLSMSSend smssend = new ALLSMSSend();
            if (actionName.ToUpper() != "TIMEALERT" && actionName.ToUpper() != "WEBSITEPAYMENTAUTO" && actionName.ToUpper() != "PASSCODEVERYFY" && actionName.ToUpper() != "CHECKPASSCODEPASSWORD" && actionName.ToUpper() != "RESENDPASSCODEPASSWORD" && actionName.ToUpper() != "TIMEALERT" && actionName.ToUpper() != "OPERATOR_REPORT_NEW")
            {
                var userid = filterContext.HttpContext.User.Identity.GetUserId();
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    var chk = db.passcodesettings.Where(x => x.userid == userid).SingleOrDefault();

                    if (chk != null)
                    {

                        var chkpasscode = chk.passcodetype;
                        if (chkpasscode != "OFF")
                        {

                            if (chk.expiretime > DateTime.Now)
                            {
                                Cookiesforattributes cokk = new Cookiesforattributes();
                                var passexptime = cokk.getcookies(userid);
                                if (passexptime == null)
                                {
                                    filterContext.Result = new RedirectToRouteResult(
                                      new RouteValueDictionary {
                                           { "controller", "Home" },
                                           { "action", "PassCodeVeryFY" }});
                                }
                                else
                                {
                                    DateTime exp = Convert.ToDateTime(passexptime);
                                    if (exp >= chk.expiretime)
                                    {
                                        return;
                                    }
                                    else
                                    {
                                        filterContext.Result = new RedirectToRouteResult(
                                     new RouteValueDictionary {
                                           { "controller", "Home" },
                                           { "action", "PassCodeVeryFY" }});
                                    }

                                }
                            }
                            else
                            {
                                var useremail = db.Users.Where(x => x.UserId == userid).SingleOrDefault();
                                var active = chk.passcodetype;
                                var expdate = "";
                                var passtypes = "";
                                if (active == "PERDAY")
                                {
                                    expdate = DateTime.Now.AddDays(1).Date.ToString();
                                    passtypes = "PERDAY";
                                }
                                else if (active == "WEAKS")
                                {
                                    expdate = DateTime.Now.AddDays(7).Date.ToString();
                                    passtypes = "WEAKS";
                                }
                                else if (active == "MONTHS")
                                {
                                    expdate = DateTime.Now.AddMonths(1).Date.ToString();
                                    passtypes = "MONTHS";
                                }
                                Random ram = new Random();
                                var pass = ram.Next(111111, 999999);
                                var Password = pass;
                                DateTime? expiredate = Convert.ToDateTime(expdate);
                                var passcodeuser = db.passcodesettings.Where(x => x.userid == userid).SingleOrDefault();

                                passcodeuser.passcodetype = passtypes;
                                passcodeuser.passcode = Password.ToString();
                                passcodeuser.expiretime = expiredate;
                                db.SaveChanges();

                                var SMS_passcodesmsonline = db.SMSSendAlls.Where(x => x.ServiceName == "PasscodeOnline").SingleOrDefault();
                                var Email_passcodesmsonline = db.EmailSendAlls.Where(x => x.ServiceName == "PasscodeOnline1").SingleOrDefault().Status;

                                smssend.sms_init(SMS_passcodesmsonline.Status, SMS_passcodesmsonline.Whatsapp_Status, "PASSCODEOTP", useremail.PhoneNumber, Password, expiredate);

                                if (Email_passcodesmsonline == "Y")
                                {
                                    var ToCC = db.Admin_details.FirstOrDefault().email;
                                    emailsend.EmailLimitChk(useremail.Email, ToCC, "PASSCODE", "Your  Passcode " + Password + " Your Passcode valid for " + expiredate, "No CallBackUrl");
                                }
                                filterContext.Result = new RedirectToRouteResult(
                              new RouteValueDictionary {
                               { "controller", "Home" },
                               { "action", "PassCodeVeryFY" } });
                            }
                        }

                    }
                }
            }


        }

        //public override void OnActionExecuting(ActionExecutingContext filterContext)
        //{

        //    //your validation here..
        //    //for example:
        //    using (VastwebmultiEntities db = new VastwebmultiEntities())
        //    {
        //        userid = filterContext.HttpContext.User.Identity.GetUserId();
        //        string checkuserdashboard = "";
        //        if (((filterContext.ActionDescriptor.ActionName == "timealert" || filterContext.ActionDescriptor.ActionName == "websitepaymentAuto") && filterContext.ActionDescriptor.ControllerDescriptor.ControllerName == "Home"))
        //        {
        //            return;
        //        }
        //        //else
        //        //{
        //        //    if ((filterContext.ActionDescriptor.ActionName != "PassCodeVeryFY" && filterContext.ActionDescriptor.ControllerDescriptor.ControllerName == "Home" && string.IsNullOrEmpty(checkuserdashboard)))
        //        //    {
        //        //        checkuserdashboard = Convert.ToString(filterContext.Controller.TempData["dashboardpage1111ravi"]);
        //        //    }

        //        //}
        //        //if ((filterContext.ActionDescriptor.ActionName == "PassCodeVeryFY" && filterContext.ActionDescriptor.ControllerDescriptor.ControllerName == "Home" && !string.IsNullOrEmpty(checkuserdashboard)))
        //        //{
        //        //    filterContext.Result = new RedirectToRouteResult(
        //        //                 new RouteValueDictionary {
        //        //         { "controller", "Home" },
        //        //               { "action", "Dashboard" } });
        //        //}

        //        if ((filterContext.ActionDescriptor.ActionName == "PassCodeVeryFY" && filterContext.ActionDescriptor.ControllerDescriptor.ControllerName == "Home"))
        //        {
        //            // filterContext.Controller.TempData["dashboardpage1111ravi"] = null;
        //            return;
        //        }

        //        if ((filterContext.ActionDescriptor.ActionName == "CHECKPASSCODEPASSWORD" && filterContext.ActionDescriptor.ControllerDescriptor.ControllerName == "Home"))
        //        {
        //            // filterContext.Controller.TempData["dashboardpage1111ravi"] = null;
        //            return;
        //        }
        //        if ((filterContext.ActionDescriptor.ActionName == "ResendPASSCODEPASSWORD" && filterContext.ActionDescriptor.ControllerDescriptor.ControllerName == "Home"))
        //        {
        //            filterContext.Controller.TempData["dashboardpage1111ravi"] = null;
        //            return;
        //        }



        //        var chkuserofff = db.passcodesettings.Where(x => x.userid == userid && x.passcodetype != "OFF").SingleOrDefault();
        //        if (chkuserofff != null)
        //        {
        //            var foo = Convert.ToString(filterContext.Controller.TempData["userfirsttime"]);
        //            if (!string.IsNullOrEmpty(foo))
        //            {
        //                filterContext.Result = new RedirectToRouteResult(
        //                           new RouteValueDictionary {
        //                 { "controller", "Home" },
        //                       { "action", "PassCodeVeryFY" } });
        //            }
        //        }
        //        var checkuser = Convert.ToString(filterContext.Controller.TempData["checkuseractive"]);
        //        if (!string.IsNullOrEmpty(checkuser))
        //        {


        //            return;
        //        }
        //        var passcodepage = Convert.ToString(filterContext.Controller.TempData["handlebackbutton1"]);
        //        if (!string.IsNullOrEmpty(passcodepage))
        //        {
        //            var CHECKPASSCODEPAGE = Convert.ToString(filterContext.Controller.TempData["handlebackbutton2"]);
        //            if (string.IsNullOrEmpty(CHECKPASSCODEPAGE))
        //            {
        //                filterContext.Result = new RedirectToRouteResult(
        //                               new RouteValueDictionary {
        //                 { "controller", "Home" },
        //                       { "action", "PassCodeVeryFY" } });
        //            }
        //        }


        //        string userName = null;



        //        // bool userpermision = false;
        //        Controller controller = filterContext.Controller as Controller;
        //        var user = db.Users.Where(x => x.UserId == userid).SingleOrDefault().Email;
        //        var ToCC = db.Admin_details.FirstOrDefault().email;
        //        CommUtilEmail emailsend = new CommUtilEmail();

        //        var pardayresult = db.passcodesettings.Where(x => x.userid == userid && x.passcodetype == "PERDAY").SingleOrDefault();
        //        {
        //            if (pardayresult != null)
        //            {
        //                if (pardayresult.expiretime < DateTime.Now)
        //                {

        //                    //bool includeLowercase = false;
        //                    //bool includeUppercase = false;
        //                    //bool includeNumeric = true;
        //                    //bool includeSpecial = false;
        //                    //bool includeSpaces = false;
        //                    //int lengthOfPassword = 6;
        //                    //string pass = PasswordGenerator.GeneratePassword(includeLowercase, includeUppercase, includeNumeric, includeSpecial, includeSpaces, lengthOfPassword);




        //                    //while (!PasswordGenerator.PasswordIsValid(includeLowercase, includeUppercase, includeNumeric, includeSpecial, includeSpaces, pass))
        //                    //{
        //                    //    pass = PasswordGenerator.GeneratePassword(includeLowercase, includeUppercase, includeNumeric, includeSpecial, includeSpaces, lengthOfPassword);
        //                    //}


        //                    //var passcodeuser = db.passcodesettings.Where(x => x.userid == userid).SingleOrDefault();


        //                    //var Password = pass;
        //                    //DateTime? expiredate = DateTime.Now.AddDays(1);
        //                    //passcodeuser.passcodetype = "PERDAY";
        //                    //passcodeuser.passcode = Password;
        //                    //passcodeuser.expiretime = expiredate;
        //                    //db.SaveChanges();
        //                    ////try
        //                    ////{
        //                    ////    new EmailCommUtils().Email_Send(useremail, "PASSCODE", "Your  Passcode" + Password + "Your Passcode valid For " + expiredate, "");
        //                    ////}
        //                    ////catch { }

        //                    //emailsend.EmailLimitChk(user, ToCC, "PASSCODE", "Your Per Day Passcode  " + Password + "Your Passcode valid For "+ expiredate, "No CallBackUrl");

        //                    filterContext.Result = new RedirectToRouteResult(
        //                   new RouteValueDictionary {
        //                 { "controller", "Home" },
        //                       { "action", "PassCodeVeryFY" } });



        //                }

        //            }
        //        }
        //        var weaklyresults = db.passcodesettings.Where(x => x.userid == userid && x.passcodetype == "WEAKS").SingleOrDefault();
        //        {
        //            if (weaklyresults != null)
        //            {
        //                if (weaklyresults.expiretime < DateTime.Now)
        //                {

        //                    //bool includeLowercase = false;
        //                    //bool includeUppercase = false;
        //                    //bool includeNumeric = true;
        //                    //bool includeSpecial = false;
        //                    //bool includeSpaces = false;
        //                    //int lengthOfPassword = 6;
        //                    //string pass = PasswordGenerator.GeneratePassword(includeLowercase, includeUppercase, includeNumeric, includeSpecial, includeSpaces, lengthOfPassword);




        //                    //while (!PasswordGenerator.PasswordIsValid(includeLowercase, includeUppercase, includeNumeric, includeSpecial, includeSpaces, pass))
        //                    //{
        //                    //    pass = PasswordGenerator.GeneratePassword(includeLowercase, includeUppercase, includeNumeric, includeSpecial, includeSpaces, lengthOfPassword);
        //                    //}


        //                    //var passcodeuser = db.passcodesettings.Where(x => x.userid == userid).SingleOrDefault();


        //                    //var Password = pass;
        //                    //DateTime? expiredate = DateTime.Now.AddDays(7);
        //                    //passcodeuser.passcodetype = "WEAKS";
        //                    //passcodeuser.passcode = Password;
        //                    //passcodeuser.expiretime = expiredate;
        //                    //db.SaveChanges();
        //                    ////try
        //                    ////{
        //                    ////    new EmailCommUtils().Email_Send(useremail, "PASSCODE", "Your  Passcode" + Password + "Your Passcode valid For " + expiredate, "");
        //                    ////}
        //                    ////catch { }

        //                    //emailsend.EmailLimitChk(user, ToCC, "PASSCODE", "Your Per Day Passcode  " + Password + "Your Passcode valid For " + expiredate, "No CallBackUrl");

        //                    // emailsend.EmailLimitChk(user, ToCC, "PASSCODE", "Your Weakely Passcode  " + pardayresult.passcode + "Your Weakely Passcode valid For " + weaklyresults.expiretime, "No CallBackUrl");
        //                    filterContext.Result = new RedirectToRouteResult(
        //                   new RouteValueDictionary {
        //                 { "controller", "Home" },
        //                       { "action", "PassCodeVeryFY" } });

        //                }

        //            }
        //        }
        //        var monthlyresults = db.passcodesettings.Where(x => x.userid == userid && x.passcodetype == "MONTHS").SingleOrDefault();
        //        {
        //            if (monthlyresults != null)
        //            {
        //                if (monthlyresults.expiretime < DateTime.Now)
        //                {

        //                    //bool includeLowercase = false;
        //                    //bool includeUppercase = false;
        //                    //bool includeNumeric = true;
        //                    //bool includeSpecial = false;
        //                    //bool includeSpaces = false;
        //                    //int lengthOfPassword = 6;
        //                    //string pass = PasswordGenerator.GeneratePassword(includeLowercase, includeUppercase, includeNumeric, includeSpecial, includeSpaces, lengthOfPassword);




        //                    //while (!PasswordGenerator.PasswordIsValid(includeLowercase, includeUppercase, includeNumeric, includeSpecial, includeSpaces, pass))
        //                    //{
        //                    //    pass = PasswordGenerator.GeneratePassword(includeLowercase, includeUppercase, includeNumeric, includeSpecial, includeSpaces, lengthOfPassword);
        //                    //}


        //                    //var passcodeuser = db.passcodesettings.Where(x => x.userid == userid).SingleOrDefault();


        //                    //var Password = pass;
        //                    //DateTime? expiredate = DateTime.Now.AddMonths(1);
        //                    //passcodeuser.passcodetype = "MONTHS";
        //                    //passcodeuser.passcode = Password;
        //                    //passcodeuser.expiretime = expiredate;
        //                    //db.SaveChanges();
        //                    ////try
        //                    ////{
        //                    ////    new EmailCommUtils().Email_Send(useremail, "PASSCODE", "Your  Passcode" + Password + "Your Passcode valid For " + expiredate, "");
        //                    ////}
        //                    ////catch { }

        //                    //emailsend.EmailLimitChk(user, ToCC, "PASSCODE", "Your Per Day Passcode  " + Password + "Your Passcode valid For " + expiredate, "No CallBackUrl");


        //                    //  emailsend.EmailLimitChk(user, ToCC, "PASSCODE", "Your Monthly Passcode  " + pardayresult.passcode + "Your Monthly Passcode valid For " + monthlyresults.expiretime, "No CallBackUrl");
        //                    filterContext.Result = new RedirectToRouteResult(
        //                    new RouteValueDictionary {
        //                 { "controller", "Home" },
        //                       { "action", "PassCodeVeryFY" } });
        //                }

        //            }
        //        }
        //        base.OnActionExecuting(filterContext);



        //    }

        //}


        //public override void OnActionExecuted(ActionExecutedContext filterContext)
        //{

        //    //your validation here..
        //    //for example:
        //    if ((filterContext.ActionDescriptor.ActionName == "Dashboard" && filterContext.ActionDescriptor.ControllerDescriptor.ControllerName == "Home"))
        //    {
        //        filterContext.Controller.TempData["dashboardpage1111ravi"] = "dashboardpage";
        //        return;
        //    }
        //    if (((filterContext.ActionDescriptor.ActionName == "timealert" || filterContext.ActionDescriptor.ActionName == "websitepaymentAuto") && filterContext.ActionDescriptor.ControllerDescriptor.ControllerName == "Home"))
        //    {
        //        return;
        //    }
        //    //if ((filterContext.ActionDescriptor.ActionName == "PassCodeVeryFY" && filterContext.ActionDescriptor.ControllerDescriptor.ControllerName == "Home"))
        //    //{
        //    //    filterContext.Controller.TempData["dashboardpage1111ravi"] = "dashboardpage";
        //    //    return;
        //    //}
        //    base.OnActionExecuted(filterContext);
        //}

    }





}