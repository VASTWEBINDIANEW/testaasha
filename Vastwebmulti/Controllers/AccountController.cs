//<%@ WebHandler Language = "C#" Class="CaptchaHandler" %>
using System.Drawing;
using System.Drawing.Imaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Data.Entity;
using System.IO;    
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Vastwebmulti.Areas.ADMIN.Controllers;
using Vastwebmulti.Models;

namespace Vastwebmulti.Controllers
{


    /// <summary>
    /// Account controller - user registration, login, logout aur password reset manage karta hai
    /// </summary>
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        VastwebmultiEntities DB = new VastwebmultiEntities();

        public AccountController()
        {
            var sertype1 = DB.tblFooterServices.ToList();
            if (sertype1.Count > 0)
            {
                var recharge = sertype1.FirstOrDefault(p => p.ServiceType == "Recharge & Bill");
                ViewBag.recharge = recharge;
                if (recharge != null)
                {
                    ViewBag.rechar1 = recharge.ServiceName1;
                    ViewBag.rechar2 = recharge.ServiceName2;
                    ViewBag.rechar3 = recharge.ServiceName3;
                    ViewBag.rechar4 = recharge.ServiceName4;
                    ViewBag.rechar5 = recharge.ServiceName5;
                    ViewBag.rechar6 = recharge.ServiceName6;
                    ViewBag.rechar7 = recharge.ServiceName7;
                    ViewBag.rechar8 = recharge.ServiceName8;
                }

                var travelfo = sertype1.FirstOrDefault(p => p.ServiceType == "Travel & Hotel");
                ViewBag.travelfo = travelfo;
                if (travelfo != null)
                {
                    ViewBag.tra1 = travelfo.ServiceName1;
                    ViewBag.tra2 = travelfo.ServiceName2;
                    ViewBag.tra3 = travelfo.ServiceName3;
                    ViewBag.tra4 = travelfo.ServiceName4;
                    ViewBag.tra5 = travelfo.ServiceName5;
                    ViewBag.tra6 = travelfo.ServiceName6;
                    ViewBag.tra7 = travelfo.ServiceName7;
                    ViewBag.tra8 = travelfo.ServiceName8;
                }
                var comm = sertype1.FirstOrDefault(p => p.ServiceType == "E-Commerce");
                ViewBag.comm = comm;
                if (comm != null)
                {
                    ViewBag.comm1 = comm.ServiceName1;
                    ViewBag.comm2 = comm.ServiceName2;
                    ViewBag.comm3 = comm.ServiceName3;
                    ViewBag.comm4 = comm.ServiceName4;
                    ViewBag.comm5 = comm.ServiceName5;
                    ViewBag.comm6 = comm.ServiceName6;
                    ViewBag.comm7 = comm.ServiceName7;
                    ViewBag.comm8 = comm.ServiceName8;
                }
                var gift = sertype1.FirstOrDefault(p => p.ServiceType == "Gift Cards");
                ViewBag.gift = gift;
                if (gift != null)
                {
                    ViewBag.gift1 = gift.ServiceName1;
                    ViewBag.gift2 = gift.ServiceName2;
                    ViewBag.gift3 = gift.ServiceName3;
                    ViewBag.gift4 = gift.ServiceName4;
                    ViewBag.gift5 = gift.ServiceName5;
                    ViewBag.gift6 = gift.ServiceName6;
                    ViewBag.gift7 = gift.ServiceName7;
                    ViewBag.gift8 = gift.ServiceName8;
                }
                var fina = sertype1.FirstOrDefault(p => p.ServiceType == "Financial Services");
                ViewBag.fina = fina;
                if (fina != null)
                {
                    ViewBag.fina1 = fina.ServiceName1;
                    ViewBag.fina2 = fina.ServiceName2;
                    ViewBag.fina3 = fina.ServiceName3;
                    ViewBag.fina4 = fina.ServiceName4;
                    ViewBag.fina5 = fina.ServiceName5;
                    ViewBag.fina6 = fina.ServiceName6;
                    ViewBag.fina7 = fina.ServiceName7;
                    ViewBag.fina8 = fina.ServiceName8;
                }
            }
            //adminfooterdata
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        //[AllowAnonymous]
        /// <summary>
        /// Login page dikhata hai aur already logged-in user ko uske role ke dashboard par redirect karta hai
        /// </summary>
        public ActionResult Login(string returnUrl)
        {


            var x = DB.Admin_details.SingleOrDefault().RenivalDate >= DateTime.Now;
            var userid = User.Identity.GetUserId();
            try
            {
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    if (User.IsInRole("Admin"))
                    {
                        return RedirectToAction("Dashboard", "Home", new { area = "ADMIN" });
                    }
                    else if (User.IsInRole("master"))
                    {
                        var stschk = db.Superstokist_details.Where(aa => aa.SSId == userid).SingleOrDefault().Status;
                        if (stschk == "Y")
                        {
                            return RedirectToAction("Dashboard", "Home", new { area = "master" });
                        }
                        //else
                        //{
                        //    TempData.Remove("data");
                        //    AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                        //    TempData["userblocked"] = "YOUR MASTER ID IS BLOCKED. PLEASE CONTACT TO ADMIN";
                        //    return RedirectToAction("Login", "Home", null);
                        //}
                    }
                    else if (User.IsInRole("Dealer"))
                    {
                        var stschk = db.Dealer_Details.Where(aa => aa.DealerId == userid).SingleOrDefault().Status;
                        if (stschk == "Y")
                        {
                            return RedirectToAction("Dashboard", "Home", new { area = "Dealer" });
                        }
                        //else
                        //{
                        //    TempData.Remove("data");
                        //    AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                        //    TempData["userblocked"] = "Your Account is Currently Blocked with Distributor, Contact to Administrator.";
                        //    return RedirectToAction("Login", "Home", null);
                        //}

                    }
                    else if (User.IsInRole("Retailer"))
                    {
                        var RetailerDetails = db.Retailer_Details.Where(aa => aa.RetailerId == userid).SingleOrDefault();
                        var DlmStsChk = db.Dealer_Details.Where(aa => aa.DealerId == RetailerDetails.DealerId).SingleOrDefault().Status;

                        if (DlmStsChk == "Y")
                        {
                            var stschk = RetailerDetails.Status;
                            if (stschk == "Y")
                            {
                                return RedirectToAction("Dashboard", "Home", new { area = "Retailer" });
                            }
                            //else
                            //{
                            //    TempData.Remove("data");
                            //    AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                            //    TempData["userblocked"] = "Your account is currently blocked, contact Administrator";
                            //    return RedirectToAction("Login", "Home", null);
                            //}
                        }
                        //else
                        //{
                        //    TempData.Remove("data");
                        //    AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                        //    TempData["userblocked"] = "Your Account is Currently Blocked with Distributor, Contact to Administrator.";
                        //    return RedirectToAction("Login", "Home", null);
                        //}

                    }
                    else if (User.IsInRole("API"))
                    {
                        var stschk = db.api_user_details.Where(aa => aa.apiid == userid).SingleOrDefault().status;
                        if (stschk == "Y")
                        {
                            return RedirectToAction("Dashboard", "Home", new { area = "API" });
                        }
                        else
                        {
                            TempData.Remove("data");
                            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                            TempData["userblocked"] = "YOUR API ID IS BLOCKED. PLEASE CONTACT TO ADMIN";
                            return RedirectToAction("Login", "Home", null);
                        }
                    }
                    else if (User.IsInRole("RCH"))
                    {
                        return RedirectToAction("Index", "Home", new { area = "RCH" });
                    }
                    else if (User.IsInRole("Vendor"))
                    {
                        var stschk = db.Vendor_details.Where(aa => aa.userid == userid).SingleOrDefault().status;
                        if (stschk == true)
                        {
                            return RedirectToAction("Index", "Home", new { area = "VENDOR" });
                        }
                        else
                        {
                            TempData.Remove("data");
                            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                            TempData["userblocked"] = "YOUR VENDOR ID IS BLOCKED. PLEASE CONTACT TO ADMIN";
                            return RedirectToAction("Login", "Home", null);
                        }

                    }
                    else if (User.IsInRole("FeeCollector"))
                    {
                        var stschk = db.FeeCollector_details.Where(aa => aa.FCId == userid).SingleOrDefault().Status;
                        if (stschk == "Y")
                        {
                            return RedirectToAction("Index", "Home", new { area = "FeeCollector" });
                        }
                        else
                        {
                            TempData.Remove("data");
                            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                            TempData["userblocked"] = "YOUR FEECOLLECTOR ID IS BLOCKED. PLEASE CONTACT TO ADMIN";
                            return RedirectToAction("Login", "Home", null);
                        }
                    }
                    else if (User.IsInRole("Employee"))
                    {
                        return RedirectToAction("Dashboard", "Home", new { area = "Employee" });
                    }
                }
            }
            catch (Exception ex)
            {

            }

            ViewBag.messagechkk = TempData["newmessage"];
            TempData.Remove("newmessage");
            if (x)
            {
                // show all state 
                ViewBag.Sate = DB.State_Desc.Select(a => new SelectListItem { Text = a.State_name, Value = a.State_id.ToString() }).ToList();
                ViewData["email"] = TempData["emailconfrim"];
                ViewData["mobilecon"] = TempData["mobileno"];
                ViewData["wrongpass"] = TempData["msg"];
                ViewData["confrim"] = TempData["Confrim"];
                ViewData["success"] = TempData["success"];
                ViewData["slaberror"] = TempData["slaberror"];
                ViewData["existuser"] = TempData["errorretailer"];
                ViewData["userblock"] = TempData["userblocked"];
            }
            else
            {
                // show all state 
                ViewBag.Sate = DB.State_Desc.Select(a => new SelectListItem { Text = a.State_name, Value = a.State_id.ToString() }).ToList();
                ViewData["msg"] = "websiteblock";
            }



            var lg = DB.tblWhiteLabelLoginBackImages.Where(a => a.Role == "ADMIN" && a.StatusCheck == "Y").ToList();
            if (lg.Count > 0)
            {
                ViewBag.showimg = lg.FirstOrDefault().otherimage;
            }
            else
            {
                //ViewBag.showimg = lg.FirstOrDefault().otherimage;
            }
            var loginSlider = DB.LoginSilders.Where(a => a.Status == "Y").ToList();
            if (loginSlider.Count > 0)
            {
                ViewBag.Loginsilder = loginSlider;
            }
            else
            {
                var LogoPath = DB.tblLoginContents.ToList();
                if (LogoPath.Any())
                {
                    ViewBag.Logo = DB.tblLoginContents.SingleOrDefault().Image;
                }
                else
                {
                    ViewBag.Logo = null;
                }
            }
            return View();

        }

        // POST: /Account/Login
        /// <summary>
        /// User ka login form submit hone par credentials verify karke dashboard par redirect karta hai
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(RegisterViewModel model, string returnUrl, string latloc, string longloc, dynamic hdaddress)
        {
            var userid = User.Identity.GetUserId();
            var onlyadmin = DB.Admin_details.Where(xss => xss.email == model.Email | xss.mobile == model.Email).FirstOrDefault();
            bool mobilelogin = true;
            bool emaillogin = true;
            bool isallowlogin = false;
            if (onlyadmin != null)
            {
                try
                {
                    var checkinfo = DB.Login_setting_email_phone.Where(aa => aa.userid == onlyadmin.userid).SingleOrDefault();
                    mobilelogin = Convert.ToBoolean(checkinfo.mobile);
                    emaillogin = Convert.ToBoolean(checkinfo.email);
                }
                catch { }
            }
            var emailid = DB.Users.Where(p => p.PhoneNumber == model.Email).ToList();
            if (emailid.Count > 0)
            {
                if (mobilelogin == true)
                {
                    isallowlogin = true;
                }
                model.Email = emailid.Single().Email;
            }
            else
            {
                if (emaillogin == true)
                {
                    isallowlogin = true;
                }
            }
            if (isallowlogin == true)
            {
                string getinform = "LAT";
                string postalcode = null;
                string city = ""; string address = "";
                try
                {
                    dynamic desres = JsonConvert.DeserializeObject(hdaddress[0]);
                    try
                    {
                        city = desres.town;
                        if (string.IsNullOrEmpty(city))
                        {
                            city = desres.village;
                        }
                        else if (string.IsNullOrEmpty(city))
                        {
                            city = desres.city;
                        }
                        else if (string.IsNullOrEmpty(city))
                        {
                            city = desres.suburb;
                        }
                        else if (string.IsNullOrEmpty(city))
                        {
                            city = city = desres.state_district;
                        }


                    }
                    catch
                    {
                        city = null;
                    }
                    address = desres.county + " " + desres.state + " " + desres.road + " " + desres.type + " " + desres.state_district + " " + " " + desres.city_district + " " + desres.municipality + " " + desres.village + " " + desres.city + " " + desres.suburb + " " + desres.town + " " + desres.postalcode;

                }
                catch
                {
                    city = null;
                }
                try
                {
                    dynamic desres = JsonConvert.DeserializeObject(hdaddress[0]);
                    postalcode = desres.postcode;
                    if (string.IsNullOrEmpty(postalcode))
                    {
                        postalcode = null;
                    }
                }
                catch { postalcode = null; }


                var locationinformation = DB.latlongstores.Where(aa => aa.latitude == latloc && aa.longitude == longloc).FirstOrDefault();
                try
                {
                    dynamic desres = JsonConvert.DeserializeObject(hdaddress[0]);
                    if (!string.IsNullOrEmpty(latloc) && !string.IsNullOrEmpty(longloc))
                    {

                        if (locationinformation == null)
                        {
                            latlongstore latlongstbl = new latlongstore();
                            latlongstbl.latitude = latloc;
                            latlongstbl.longitude = longloc;
                            latlongstbl.states = desres.state;
                            latlongstbl.city = city;
                            latlongstbl.locations = desres.ToString();
                            DB.latlongstores.Add(latlongstbl);
                            DB.SaveChanges();

                        }
                    }
                }
                catch { }
                string externalip = "";
                try
                {
                    externalip = Request.UserHostAddress;
                    if (externalip == "::1")
                    {
                        externalip = new WebClient().DownloadString("http://ipv4.icanhazip.com/");
                        externalip = externalip.Replace("\n", "");
                    }
                }
                catch { }
                if (onlyadmin != null)
                {
                    var ipexistornot = DB.ipadreesvalidates.Where(a => a.ipstatus == "Y").ToList();
                    if (ipexistornot.Any())
                    {

                        var checkipaddressexist = ipexistornot.Where(aaa => aaa.ipaddress.Contains(externalip)).FirstOrDefault();
                        if (checkipaddressexist == null)
                        {
                            TempData["newmessage"] = "you are Not Authorsie To Login Current I.P.";
                            return RedirectToAction("Login", "Home", new { id = "singin" });
                        }
                    }
                }
                TempData["dashboardpage1111ravi"] = null;

                var usr1 = UserManager.FindByEmail(model.Email);
                if (usr1 != null)
                {
                    string rolename = UserManager.GetRoles(usr1.Id).FirstOrDefault();
                    if (rolename == "Whitelabel" || rolename == "Whitelabeldealer" || rolename == "Whitelabelretailer" || rolename == "Whitelabelmaster")
                    {
                        TempData["newmessage"] = "Login Id Or Password Wrong";
                        return RedirectToAction("Login", "Home");
                    }
                    var employeeisactive = DB.tbl_Admin_Employee.Where(aa => aa.EmployeeID == usr1.Id).SingleOrDefault();
                    if (employeeisactive != null)
                    {
                        if (employeeisactive.Status == false)
                        {
                            TempData["newmessage"] = "Your are not authorise to Login by Website, Please  Contact to Administrator";
                            // TempData["newmessage"] = "Your account is currently blocked, contact Administrator";
                            return RedirectToAction("Login", "Home");
                        }
                    }

                    var retaileractive = DB.Retailer_Details.Where(aa => aa.RetailerId == usr1.Id).SingleOrDefault();
                    if (retaileractive != null)
                    {
                        if (retaileractive.Status == "N")
                        {
                            TempData["newmessage"] = "Your are not authorise to Login by Website, Please  Contact to Administrator";
                            // TempData["newmessage"] = "Your account is currently blocked, contact Administrator";
                            return RedirectToAction("Login", "Home");
                        }
                        if (retaileractive.ISDeleteuser == true)
                        {
                            TempData["newmessage"] = "User Not Found";
                            // TempData["newmessage"] = "Your account is currently blocked, contact Administrator";
                            return RedirectToAction("Login", "Home");
                        }
                    }

                    var masteractive = DB.Superstokist_details.Where(aa => aa.SSId == usr1.Id).SingleOrDefault();
                    if (masteractive != null)
                    {
                        if (masteractive.Status == "N")
                        {
                            TempData["newmessage"] = "Your are not authorise to Login by Website, Please  Contact to Administrator";
                            // TempData["newmessage"] = "YOUR MASTER ID IS BLOCKED. PLEASE CONTACT TO ADMIN";
                            return RedirectToAction("Login", "Home");
                        }
                    }
                    var dealerstatus = DB.Dealer_Details.Where(aa => aa.DealerId == usr1.Id).SingleOrDefault();
                    if (dealerstatus != null)
                    {
                        if (dealerstatus.Status == "N")
                        {
                            TempData["newmessage"] = "Your are not authorise to Login by Website, Please  Contact to Administrator";
                            //  TempData["newmessage"] = "Your Account is Currently Blocked with Distributor, Contact to Administrator.";
                            return RedirectToAction("Login", "Home");
                        }
                    }



                    var retailermanagelocation = DB.Manage_rem_Location_by_Admin.Where(sss => sss.userid == usr1.Id).ToList();
                    if (retailermanagelocation.Any())
                    {


                        if (string.IsNullOrEmpty(latloc) && string.IsNullOrEmpty(longloc))
                        {
                            try
                            {
                                getinform = "IP";
                                var client = new RestClient("http://ip-api.com/json/" + externalip + "?fields=city");
                                var request = new RestRequest(Method.GET);
                                var task = Task.Run(() =>
                                {
                                    return client.Execute(request).Content;
                                });
                                bool isCompletedSuccessfully = task.Wait(TimeSpan.FromMilliseconds(10000));
                                if (isCompletedSuccessfully)
                                {
                                    var responsessss = task.Result;
                                    dynamic presonse111 = JsonConvert.DeserializeObject(responsessss);
                                    city = presonse111.city;
                                }
                            }
                            catch { }
                        }

                        var findexectlocation = DB.Manage_rem_Location_by_Admin.Where(sss => sss.userid == usr1.Id && sss.nameofcity.Trim().ToUpper() == city.Trim().ToUpper()).FirstOrDefault();
                        if (findexectlocation == null)
                        {
                            TempData["newmessage"] = "Login not allowed at this location,Please Contact to Administrator.";
                            return RedirectToAction("Login", "Home");
                        }

                    }
                }

                //if (usr1 != null)
                //  {
                //      string rolename = UserManager.GetRoles(usr1.Id).FirstOrDefault();
                //      if (rolename == "master")
                //      {
                //          var chksts = DB.Superstokist_details.Where(sss => sss.SSId == usr1.Id).SingleOrDefault();
                //          if (chksts.Status == "N")
                //          {
                //              TempData["newmessage"] = "Your Account Is Not Active.";
                //              return RedirectToAction("Login", "Home");
                //          }
                //      }
                //  }

                //if (usr1 != null)
                //{
                //    string rolename = UserManager.GetRoles(usr1.Id).FirstOrDefault();
                //    if (rolename == "Employee")
                //    {
                //        TempData["newmessage"] = "Login Id Or Password Wrong";
                //        return RedirectToAction("Login", "Home");
                //    }
                //}

                //var emailid = DB.Users.Where(p => p.PhoneNumber == model.Email).ToList();
                //if (emailid.Count > 0)
                //{
                //    model.Email = emailid.Single().Email;
                //}
                var x = DB.Admin_details.SingleOrDefault().RenivalDate >= DateTime.Now;



                if (x)
                {
                    var usr = UserManager.FindByEmail(model.Email);

                    if (usr == null)
                    {
                        TempData["newmessage"] = "User Not Found";
                        return RedirectToAction("Login", "Home");
                    }
                    bool? chksecurity = true;
                    try
                    {
                        var chk = DB.security_settings.ToList();
                        if (chk.Any())
                        {
                            chksecurity = DB.security_settings.SingleOrDefault().highsecurity;
                        }
                        else
                        {
                            security_settings defaultInsert = new security_settings();
                            defaultInsert.highsecurity = false;
                            DB.security_settings.Add(defaultInsert);
                            DB.SaveChanges();
                        }

                    }
                    catch { }
                    if (chksecurity == true)
                    {
                        if (usr != null && !UserManager.IsEmailConfirmed(usr.Id))
                        {
                            TempData["Confrim"] = "Confrimed";
                            return RedirectToAction("Login", "Home");
                        }
                    }


                    if (await UserManager.IsLockedOutAsync(usr.Id))
                    {

                        return View("LockoutAdmin");
                        // ModelState.AddModelError("", string.Format("Your account has been locked out for 5 minutes due to multiple failed login attempts."));
                    }




                    var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: true);
                    string roles1 = "";

                    switch (result)
                    {

                        case SignInStatus.Success:
                            try
                            {
                                var user = await UserManager.FindAsync(model.Email, model.Password);
                                var roles = await UserManager.GetRolesAsync(user.Id);
                                roles1 = roles[0];

                                var Ipaddress = Request.UserHostAddress;

                                try
                                {
                                    var ipchk = "";
                                    if (Ipaddress != "::1")
                                    {
                                        ipchk = Ipaddress;
                                    }
                                    var client = new RestClient("http://ip-api.com/json/" + ipchk + "");
                                    client.Timeout = -1;
                                    var request = new RestRequest(Method.GET);
                                    var task = Task.Run(() =>
                                    {
                                        return client.Execute(request).Content;
                                    });
                                    bool isCompletedSuccessfully = task.Wait(TimeSpan.FromMilliseconds(10000));
                                    if (isCompletedSuccessfully == true)
                                    {
                                        var resp = task.Result;
                                        dynamic respo = JsonConvert.DeserializeObject(resp);
                                        string status = respo.status;
                                        if (status == "success")
                                        {
                                            if (string.IsNullOrEmpty(latloc) && string.IsNullOrEmpty(longloc))
                                            {
                                                getinform = "IP";
                                                string country = respo.country;
                                                string regionName = respo.regionName;
                                                city = respo.city;
                                                string zip = respo.zip;
                                                latloc = respo.lat;
                                                longloc = respo.lon;
                                                address = regionName + " , " + country + " , " + zip;
                                            }
                                        }
                                    }
                                }
                                catch { }




                                using (VastwebmultiEntities db = new VastwebmultiEntities())
                                {
                                    if (roles1.ToUpper().Contains("WHITELABEL"))
                                    {
                                        var whitelabelid = "";
                                        if (roles1 == "Whitelabel")
                                        {
                                            whitelabelid = db.WhiteLabel_userList.Where(w => w.EmailId == model.Email).SingleOrDefault()?.WhiteLabelID;
                                        }
                                        else if (roles1 == "Whitelabelmaster")
                                        {
                                            whitelabelid = db.Whitelabel_Superstokist_details.Where(w => w.Email == model.Email).SingleOrDefault()?.Whitelabelid;
                                        }
                                        else if (roles1 == "Whitelabeldealer")
                                        {
                                            whitelabelid = db.whitelabel_Dealer_Details.Where(w => w.Email == model.Email).SingleOrDefault()?.Whitelabelid;
                                        }
                                        else if (roles1 == "Whitelabelretailer")
                                        {
                                            whitelabelid = db.Whitelabel_Retailer_Details.Where(w => w.Email == model.Email).SingleOrDefault()?.Whitelabelid;
                                        }
                                        Whitelabel_Login_info WLogininfo = new Whitelabel_Login_info();
                                        WLogininfo.CurrentLoginTime = DateTime.Now;
                                        WLogininfo.IP_Address = Ipaddress;
                                        WLogininfo.WhitelabelId = whitelabelid;
                                        WLogininfo.City = city;
                                        //WLogininfo. = postalcode;
                                        WLogininfo.Location = address;
                                        WLogininfo.LastLoginTime = db.Whitelabel_Login_info.Where(a => a.userid == model.Email).OrderByDescending(a => a.Idno).FirstOrDefault() != null ?
                                        db.Whitelabel_Login_info.Where(a => a.userid == model.Email).OrderByDescending(a => a.Idno).FirstOrDefault().CurrentLoginTime : DateTime.Now;
                                        WLogininfo.LoginFrom = "Web";
                                        WLogininfo.userid = model.Email;
                                        WLogininfo.ModelNo = getinform;
                                        WLogininfo.Latitude = latloc;
                                        WLogininfo.Logitude = longloc;

                                        WLogininfo.browser = Request.Browser.Browser + " " + Request.Browser.Version;
                                        db.Whitelabel_Login_info.Add(WLogininfo);

                                        db.SaveChanges();
                                    }

                                    Login_info Logininfo = new Login_info();
                                    Logininfo.CurrentLoginTime = DateTime.Now;
                                    Logininfo.IP_Address = Ipaddress;

                                    Logininfo.City = city;
                                    Logininfo.PostalCode = postalcode;
                                    Logininfo.Location = address;
                                    Logininfo.LastLoginTime = db.Login_info.Where(a => a.UserId == model.Email).OrderByDescending(a => a.Idno).FirstOrDefault() != null ?
                                    db.Login_info.Where(a => a.UserId == model.Email).OrderByDescending(a => a.Idno).FirstOrDefault().CurrentLoginTime : DateTime.Now;
                                    Logininfo.LoginFrom = "Web";
                                    Logininfo.UserId = model.Email;
                                    Logininfo.ModelNo = getinform;
                                    Logininfo.Latitude = latloc;
                                    Logininfo.Logitude = longloc;

                                    Logininfo.browser = Request.Browser.Browser + " " + Request.Browser.Version;
                                    db.Login_info.Add(Logininfo);
                                    var app = db.Users.FirstOrDefault(ff => ff.UserId == usr.Id);
                                    if (app.LockoutEndDateUtc != null)
                                    {
                                        User obj = (from dd in db.Users
                                                    where dd.UserId == usr.Id
                                                    select dd).FirstOrDefault();
                                        obj.LockoutEndDateUtc = null;


                                    }
                                    db.SaveChanges();
                                }
                            }
                            catch
                            {

                            }

                            if (roles1 == "Admin")
                            {

                                return RedirectToAction("Dashboard", "Home", new { area = "ADMIN" });
                            }
                            else if (roles1 == "master")
                            {

                                return RedirectToAction("Dashboard", "Home", new { area = "master" });
                            }
                            else if (roles1 == "Dealer")
                            {

                                return RedirectToAction("Dashboard", "Home", new { area = "Dealer" });
                            }
                            else if (roles1 == "Retailer")
                            {
                                var emailmobile = DB.Users.Where(aa => aa.UserId == usr1.Id).SingleOrDefault();
                                var isExist = DB.Email_Mobile_Verify.Any();
                                if (!isExist)
                                {
                                    var entry = new Email_Mobile_Verify();
                                    entry.emailmobile_verified = "ALL";
                                    DB.Email_Mobile_Verify.Add(entry);
                                    DB.SaveChanges();
                                }
                                var emailmobilests = DB.Email_Mobile_Verify.SingleOrDefault().emailmobile_verified;
                                if (emailmobilests.ToUpper().Contains("OFF"))
                                {
                                    return RedirectToAction("Dashboard", "Home", new { area = "Retailer" });
                                }
                                else
                                {
                                    if (emailmobile.EmailConfirmed == false || emailmobile.PhoneNumberConfirmed == false)
                                    {

                                        return RedirectToAction("VeryFY_Profiles_users", "Home", new { area = "Retailer" });
                                    }
                                    return RedirectToAction("Dashboard", "Home", new { area = "Retailer" });
                                }
                            }
                            else if (User.IsInRole("API"))
                            {

                                return RedirectToAction("Dashboard", "Home", new { area = "API" });
                            }
                            else if (roles1 == "RCH")
                            {
                                return RedirectToAction("Index", "Home", new { area = "RCH" });
                            }
                            else if (roles1 == "Sub")
                            {
                                return RedirectToAction("Index", "Home", new { area = "Sub" });
                            }

                            else if (roles1 == "Vendor")
                            {
                                return RedirectToAction("Index", "Home", new { area = "VENDOR" });
                            }
                            else if (roles1 == "FeeCollector")
                            {
                                return RedirectToAction("Index", "Home", new { area = "FeeCollector" });
                            }
                            //**************************************************
                            else if (roles1 == "Whitelabel")
                            {

                                return RedirectToAction("Dashboard", "Home", new { area = "WHITELABEL" });
                            }
                            else if (roles1 == "Whitelabelmaster")
                            {
                                return RedirectToAction("Dashboard", "Home", new { area = "WMASTER" });
                            }
                            else if (roles1 == "Whitelabeldealer")
                            {
                                return RedirectToAction("Dashboard", "Home", new { area = "WDealer" });
                            }
                            else if (roles1 == "Whitelabelretailer")
                            {
                                return RedirectToAction("Dashboard", "Home", new { area = "WRetailer" });
                            }
                            else if (roles1 == "Employee")
                            {

                                return RedirectToAction("Dashboard", "Home", new { area = "Employee" });
                            }
                            //**************************************************
                            else
                            {
                                return RedirectToLocal(returnUrl);
                            }
                        case SignInStatus.LockedOut:

                            return View("LockoutAdmin");
                        case SignInStatus.RequiresVerification:

                            TempData["loginuseremailidtem"] = model.Email;
                            TempData["longitudetemp"] = longloc;
                            TempData["latitudestemp"] = latloc;


                            return RedirectToAction("VerifyCodeAdmin", new { provider = "Email Code", ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                        case SignInStatus.Failure:
                            #region SaveFailedLoginInfo
                            try
                            {

                                var Ipaddress = Request.UserHostAddress;
                                if (string.IsNullOrEmpty(latloc) && string.IsNullOrEmpty(longloc))
                                {

                                    try
                                    {
                                        var ipchk = "";
                                        if (Ipaddress != "::1")
                                        {
                                            ipchk = Ipaddress;
                                        }
                                        var client = new RestClient("http://ip-api.com/json/" + ipchk + "");
                                        client.Timeout = -1;
                                        var request = new RestRequest(Method.GET);
                                        var task = Task.Run(() =>
                                        {
                                            return client.Execute(request).Content;
                                        });
                                        bool isCompletedSuccessfully = task.Wait(TimeSpan.FromMilliseconds(10000));
                                        if (isCompletedSuccessfully == true)
                                        {
                                            var resp = task.Result;
                                            dynamic respo = JsonConvert.DeserializeObject(resp);
                                            string status = respo.status;
                                            if (status == "success")
                                            {
                                                string country = respo.country;
                                                string regionName = respo.regionName;
                                                city = respo.city;
                                                string zip = respo.zip;
                                                latloc = respo.lat;
                                                longloc = respo.lon;
                                                address = regionName + " , " + country + " , " + zip;
                                            }
                                        }
                                    }
                                    catch { }
                                }

                                using (VastwebmultiEntities DB = new VastwebmultiEntities())
                                {

                                    if (roles1.ToUpper().Contains("WHITELABEL"))
                                    {
                                        var whitelabelid = "";
                                        if (roles1 == "Whitelabel")
                                        {
                                            whitelabelid = DB.WhiteLabel_userList.Where(w => w.EmailId == model.Email).SingleOrDefault()?.WhiteLabelID;
                                        }
                                        else if (roles1 == "Whitelabelmaster")
                                        {
                                            whitelabelid = DB.Whitelabel_Superstokist_details.Where(w => w.Email == model.Email).SingleOrDefault()?.Whitelabelid;
                                        }
                                        else if (roles1 == "Whitelabeldealer")
                                        {
                                            whitelabelid = DB.whitelabel_Dealer_Details.Where(w => w.Email == model.Email).SingleOrDefault()?.Whitelabelid;
                                        }
                                        else if (roles1 == "Whitelabelretailer")
                                        {
                                            whitelabelid = DB.Whitelabel_Retailer_Details.Where(w => w.Email == model.Email).SingleOrDefault()?.Whitelabelid;
                                        }
                                        Whitelabel_Failed_Login_info WLogininfo = new Whitelabel_Failed_Login_info();
                                        WLogininfo.CurrentLoginTime = DateTime.Now;
                                        WLogininfo.IP_Address = Ipaddress;
                                        WLogininfo.WhitelabelId = whitelabelid;
                                        WLogininfo.City = city;
                                        //WLogininfo. = postalcode;
                                        WLogininfo.Location = address;
                                        WLogininfo.LastLoginTime = DB.Whitelabel_Failed_Login_info.Where(a => a.userid == model.Email).OrderByDescending(a => a.Idno).FirstOrDefault() != null ?
                                        DB.Whitelabel_Failed_Login_info.Where(a => a.userid == model.Email).OrderByDescending(a => a.Idno).FirstOrDefault().CurrentLoginTime : DateTime.Now;
                                        WLogininfo.LoginFrom = "Web";
                                        WLogininfo.userid = model.Email;
                                        WLogininfo.ModelNo = getinform;
                                        WLogininfo.Latitude = latloc;
                                        WLogininfo.Logitude = longloc;

                                        WLogininfo.browser = Request.Browser.Browser + " " + Request.Browser.Version;
                                        DB.Whitelabel_Failed_Login_info.Add(WLogininfo);

                                        DB.SaveChanges();
                                    }


                                    Failed_Login_info loginInfo = new Failed_Login_info();
                                    // loginInfo = db1.Login_info.Where(a=> a.UserId == UserId).Single();
                                    loginInfo.LoginTime = DateTime.Now;

                                    loginInfo.IP_Address = Ipaddress;
                                    loginInfo.LoginFrom = "Web";
                                    loginInfo.EmailId = model.Email;
                                    loginInfo.pwd = model.Password;
                                    loginInfo.browser = Request.Browser.Browser + " " + Request.Browser.Version;
                                    loginInfo.Macaddress = "";

                                    loginInfo.latitude = latloc;
                                    loginInfo.Logitude = longloc;
                                    loginInfo.Location = address;
                                    loginInfo.city = city;
                                    DB.Failed_Login_info.Add(loginInfo);
                                    DB.SaveChanges();
                                    CommUtilEmail emailsend = new CommUtilEmail();
                                    var ToCC = DB.Admin_details.FirstOrDefault().email;
                                    var admininfoemail = DB.Admin_details.SingleOrDefault();
                                    var txtMsgBody = "Dear user your id is trying to login on the location " + address + " Thank you for business with us";
                                    txtMsgBody = txtMsgBody + " Regards " + admininfoemail.Companyname;

                                    emailsend.EmailLimitChk(model.Email, "", "Invalid Login Attempt", txtMsgBody, "No CallBackUrl");

                                    var SMS_wrongpassword = DB.SMSSendAlls.Where(aa => aa.ServiceName == "wrongpasswordadmin").SingleOrDefault();
                                    if (admininfoemail.email.ToUpper() == model.Email.ToUpper())
                                    {
                                        smssend.sms_init(SMS_wrongpassword.Status, SMS_wrongpassword.Whatsapp_Status, "WRONGPASSWORD", admininfoemail.mobile, model.Email, address);
                                    }


                                }
                            }

                            catch
                            {
                            }
                            TempData["newmessage"] = "Login Id Or Password Wrong";
                            return RedirectToAction("Login", "Home");
                        #endregion
                        default:
                            TempData["newmessage"] = "Failure";
                            //ModelState.AddModelError("", "Invalid login attempt.");
                            return RedirectToAction("Login", "Home");
                    }
                }
                else
                {
                    ViewData["newmessage"] = "websiteblock";
                }
            }
            else
            {
                TempData["newmessage"] = "Not Allow To Login";
            }
            return RedirectToAction("Login", "Home");
        }



        //[HttpGet]
        //[AllowAnonymous]
        //public JObject EKYC_Varification_Status(string userid)
        //{
        //    bool aadhar = true;
        //    bool pan = true;
        //    try
        //    {
        //        VastwebmultiEntities db = new VastwebmultiEntities();

        //        var retailer_exist = db.Retailer_Details.Any(s => s.RetailerId == userid);
        //        if (!retailer_exist)
        //        {
        //            throw new Exception("Retailer Not Found");
        //        }

        //        var verify_data = db.AAdharPanEKYC_Verify.Single();

        //        var retailer_verification_data = db.Retailer_Details.Where(s => s.RetailerId == userid).Single();

        //        if (retailer_verification_data.CretedBy == "Signup")
        //        {
        //            aadhar = (bool)retailer_verification_data.aadhar_verification;
        //            pan = (bool)retailer_verification_data.pan_verification;
        //        }
        //        else
        //        {
        //            if(verify_data.type == "all")
        //            {
        //                aadhar = (bool)retailer_verification_data.aadhar_verification;
        //                pan = (bool)retailer_verification_data.pan_verification;
        //            }
        //        }

        //        var obj = new
        //        {
        //            status = true,
        //            aadhar,
        //            pan
        //        };

        //        return JObject.Parse(obj.ToString());

        //    }
        //    catch (Exception ex)
        //    {
        //        var obj = new
        //        {
        //            status = false,
        //            message = ex.Message,
        //            aadhar = false,
        //            pan = false
        //        };

        //        return JObject.Parse(obj.ToString());
        //    }
        //}

        /// <summary>
        /// Naye login form se user authenticate karke appropriate dashboard par redirect karta hai
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> NewLogin(RegisterViewModel model, string returnUrl, string latloc, string longloc)
        {
            var userid = User.Identity.GetUserId();
            var emailid = DB.Users.Where(p => p.PhoneNumber == model.USERCHECK).ToList();
            model.Email = model.USERCHECK;
            if (emailid.Count > 0)
            {
                model.Email = emailid.Single().Email;
            }
            var usr1 = UserManager.FindByEmail(model.Email);
            if (usr1 != null)
            {
                string rolename = UserManager.GetRoles(usr1.Id).FirstOrDefault();
                if (rolename == "Whitelabel" || rolename == "Whitelabeldealer" || rolename == "Whitelabelretailer" || rolename == "Whitelabelmaster")
                {
                    var x = DB.Admin_details.SingleOrDefault().RenivalDate >= DateTime.Now;
                    if (x)
                    {
                        var usr = UserManager.FindByEmail(model.Email);
                        string currenturl = HttpContext.Request.Url.Authority;
                        currenturl = currenturl.Replace("www.", "").Replace("https://", "").Replace("http://", "");
                        currenturl = currenturl.ToUpper();
                        var whitelabelid = "";
                        if (currenturl.Contains("LOCALHOST"))
                        {
                            whitelabelid = DB.WhiteLabel_userList.Where(aa => aa.localhost.Replace("www.", "").Replace("https://", "").Replace("http://", "").ToLower() == currenturl.ToLower()).SingleOrDefault().WhiteLabelID;
                        }
                        else
                        {
                            whitelabelid = DB.WhiteLabel_userList.Where(aa => aa.websitename.Replace("www.", "").Replace("https://", "").Replace("http://", "").ToLower() == currenturl.ToLower()).SingleOrDefault().WhiteLabelID;
                        }
                        var check = false;
                        var commonid = "";
                        var common = DB.Users.Where(a => a.Email == model.Email).ToList();
                        if (common.Count > 0)
                        {
                            commonid = common.SingleOrDefault().UserId;
                        }
                        else
                        {
                            TempData["msg"] = "Failure";
                            return RedirectToAction("Index", "Home");
                        }
                        var role = (from rol in DB.Roles join user in DB.UserRoles on rol.RoleId equals user.RoleId where user.UserId == commonid select rol.Name).SingleOrDefault().ToString();
                        if (role == "Whitelabel")
                        {
                            if (whitelabelid == commonid)
                            {
                                check = true;
                            }
                        }
                        else if (role == "Whitelabelmaster")
                        {
                            var istrueid = DB.Whitelabel_Superstokist_details.Where(a => a.SSId == commonid).ToList();
                            if (istrueid.Count > 0)
                            {
                                if (whitelabelid == istrueid.SingleOrDefault().Whitelabelid)
                                {
                                    check = true;
                                }
                            }
                        }
                        else if (role == "Whitelabeldealer")
                        {
                            var istrueid = DB.whitelabel_Dealer_Details.Where(a => a.DealerId == commonid).ToList();
                            if (istrueid.Count > 0)
                            {
                                if (whitelabelid == istrueid.SingleOrDefault().Whitelabelid)
                                {
                                    check = true;
                                }
                            }
                        }
                        else if (role == "Whitelabelretailer")
                        {
                            var whitedlmid = DB.Whitelabel_Retailer_Details.Where(a => a.RetailerId == commonid).ToList();
                            if (whitedlmid.Count > 0)
                            {
                                var whitelabeldlmid = DB.Whitelabel_Retailer_Details.Where(a => a.RetailerId == commonid).SingleOrDefault().DealerId;
                                var whiteid = DB.whitelabel_Dealer_Details.Where(a => a.DealerId == whitelabeldlmid).ToList();
                                if (whiteid.Count > 0)
                                {
                                    if (whitelabelid == whiteid.SingleOrDefault().Whitelabelid)
                                    {
                                        check = true;
                                    }
                                }
                            }
                        }
                        if (check == true)
                        {
                            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
                            switch (result)
                            {
                                case SignInStatus.Success:
                                    TempData["Userid"] = usr.Id;
                                    TempData["LoginId"] = model.Email;
                                    TempData["Password"] = model.Password;

                                    try
                                    {
                                        var client = new RestClient(Vastwebmulti.Areas.WHITELABEL.Models.WapiBaseUrl.GetBaseUrl() + "/token");
                                        var request = new RestRequest(Method.POST);
                                        request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                                        request.AddParameter("UserName", model.Email);
                                        request.AddParameter("Password", model.Password);
                                        request.AddParameter("grant_type", "password");
                                        IRestResponse response = client.Execute(request);

                                        dynamic res = JsonConvert.DeserializeObject(response.Content);

                                        using (VastwebmultiEntities db = new VastwebmultiEntities())
                                        {
                                            var uid = db.Users.Where(s => s.Email == model.Email).SingleOrDefault().UserId;
                                            var token_table = db.TokenGenWApis.Where(s => s.UserId == uid).SingleOrDefault();
                                            if (token_table != null)
                                            {
                                                token_table.Token = res.access_token.ToString();
                                                var date = DateTime.Now;
                                                date = date.AddHours(6);
                                                token_table.TknExpTime = date;
                                                db.SaveChanges();
                                            }
                                            else
                                            {
                                                TokenGenWApi tkn_tbl = new TokenGenWApi();
                                                tkn_tbl.WhitelabelId = whitelabelid;
                                                tkn_tbl.Token = res.access_token.ToString();
                                                var date = DateTime.Now;
                                                date = date.AddHours(6);
                                                tkn_tbl.TknExpTime = date;
                                                tkn_tbl.UserId = uid;
                                                tkn_tbl.Role = role;
                                                db.TokenGenWApis.Add(tkn_tbl);
                                                db.SaveChanges();
                                            }

                                        }
                                    }
                                    catch { }


                                    try
                                    {
                                        using (VastwebmultiEntities db = new VastwebmultiEntities())
                                        {
                                            Whitelabel_Login_info WLogininfo = new Whitelabel_Login_info();
                                            WLogininfo.CurrentLoginTime = DateTime.Now;
                                            WLogininfo.IP_Address = "";
                                            WLogininfo.Role = role;
                                            WLogininfo.WhitelabelId = whitelabelid;
                                            WLogininfo.City = "";
                                            //WLogininfo. = postalcode;
                                            WLogininfo.Location = "";
                                            WLogininfo.LastLoginTime = db.Whitelabel_Login_info.Where(a => a.userid == usr.Id).OrderByDescending(a => a.Idno).FirstOrDefault() != null ?
                                            db.Whitelabel_Login_info.Where(a => a.userid == usr.Id).OrderByDescending(a => a.Idno).FirstOrDefault().CurrentLoginTime : DateTime.Now;
                                            WLogininfo.LoginFrom = "Web";
                                            WLogininfo.userid = usr.Id;
                                            WLogininfo.ModelNo = "";
                                            WLogininfo.Latitude = latloc;
                                            WLogininfo.Logitude = longloc;

                                            WLogininfo.browser = Request.Browser.Browser + " " + Request.Browser.Version;
                                            db.Whitelabel_Login_info.Add(WLogininfo);

                                            db.SaveChanges();


                                            var Ipaddress = Request.UserHostAddress;
                                            Login_info Logininfo = new Login_info();
                                            Logininfo.CurrentLoginTime = DateTime.Now;
                                            Logininfo.IP_Address = Ipaddress;
                                            Logininfo.LastLoginTime = db.Login_info.Where(a => a.UserId == model.Email).OrderByDescending(a => a.Idno).FirstOrDefault() != null ?
                                            db.Login_info.Where(a => a.UserId == model.Email).OrderByDescending(a => a.Idno).FirstOrDefault().CurrentLoginTime : DateTime.Now;
                                            Logininfo.LoginFrom = "Web";
                                            Logininfo.UserId = usr.Id;
                                            Logininfo.Latitude = latloc;
                                            Logininfo.Logitude = longloc;
                                            Logininfo.UserId = model.Email;
                                            Logininfo.browser = Request.Browser.Browser + " " + Request.Browser.Version;
                                            db.Login_info.Add(Logininfo);
                                            db.SaveChanges();
                                        }
                                    }
                                    catch (Exception ex)
                                    { }
                                    if (User.IsInRole("Whitelabel"))
                                    {
                                        return RedirectToAction("Dashboard", "Home", new { area = "WHITELABEL" });
                                    }
                                    else if (User.IsInRole("Whitelabelmaster"))
                                    {
                                        return RedirectToAction("Dashboard", "Home", new { area = "WMASTER" });
                                    }
                                    else if (User.IsInRole("Whitelabeldealer"))
                                    {
                                        return RedirectToAction("Dashboard", "Home", new { area = "WDealer" });
                                    }
                                    if (User.IsInRole("Whitelabelretailer"))
                                    {
                                        return RedirectToAction("Dashboard", "Home", new { area = "WRetailer" });
                                    }
                                    else
                                    {
                                        return RedirectToLocal(returnUrl);
                                    }
                                case SignInStatus.LockedOut:
                                    return View("Lockout");
                                case SignInStatus.RequiresVerification:
                                    TempData["Userid"] = usr.Id;
                                    TempData["LoginId"] = model.Email;
                                    TempData["Password"] = model.Password;
                                    return RedirectToAction("VerifyCode", new { provider = "Email Code", ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                                case SignInStatus.Failure:
                                    #region SaveFailedLoginInfo
                                    try
                                    {
                                        using (VastwebmultiEntities DB = new VastwebmultiEntities())
                                        {
                                            if (role.ToUpper().Contains("WHITELABEL"))
                                            {
                                                Whitelabel_Failed_Login_info WLogininfo = new Whitelabel_Failed_Login_info();
                                                WLogininfo.CurrentLoginTime = DateTime.Now;
                                                WLogininfo.IP_Address = "";
                                                WLogininfo.Role = role;
                                                WLogininfo.WhitelabelId = whitelabelid;
                                                WLogininfo.City = "";
                                                //WLogininfo. = postalcode;
                                                WLogininfo.Location = "";
                                                WLogininfo.LastLoginTime = DB.Whitelabel_Failed_Login_info.Where(a => a.userid == usr.Id).OrderByDescending(a => a.Idno).FirstOrDefault() != null ?
                                                DB.Whitelabel_Failed_Login_info.Where(a => a.userid == usr.Id).OrderByDescending(a => a.Idno).FirstOrDefault().CurrentLoginTime : DateTime.Now;
                                                WLogininfo.LoginFrom = "Web";
                                                WLogininfo.userid = usr.Id;
                                                WLogininfo.ModelNo = "";
                                                WLogininfo.Latitude = latloc;
                                                WLogininfo.Logitude = longloc;

                                                WLogininfo.browser = Request.Browser.Browser + " " + Request.Browser.Version;
                                                DB.Whitelabel_Failed_Login_info.Add(WLogininfo);

                                                DB.SaveChanges();
                                            }

                                            var Ipaddress = Request.UserHostAddress;
                                            Failed_Login_info loginInfo = new Failed_Login_info();
                                            loginInfo.LoginTime = DateTime.Now;
                                            loginInfo.IP_Address = Ipaddress;
                                            loginInfo.LoginFrom = "Web";
                                            loginInfo.EmailId = model.Email;
                                            loginInfo.pwd = model.Password;
                                            loginInfo.browser = Request.Browser.Browser + " " + Request.Browser.Version;
                                            loginInfo.Macaddress = "";
                                            DB.Failed_Login_info.Add(loginInfo);
                                            DB.SaveChanges();
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                    }
                                    TempData["msg"] = "Failure";
                                    //return RedirectToAction("Index", "Home");
                                    return RedirectToAction("NewLogin", "Home");

                                #endregion
                                default:
                                    TempData["msg"] = "Failure";
                                    //return RedirectToAction("Index", "Home");
                                    return RedirectToAction("NewLogin", "Home");
                            }
                        }
                        else
                        {
                            TempData["msg"] = "Failure";
                            //return RedirectToAction("Index", "Home");
                            return RedirectToAction("NewLogin", "Home");
                        }
                    }
                    else
                    {
                        TempData["msg"] = "websiteblock";
                    }

                }
                else
                {
                    TempData["msg"] = "Failure";
                }
            }
            else
            {
                TempData["msg"] = "User Not Found";
            }

            //return RedirectToAction("Index", "Home");
            return RedirectToAction("NewLogin", "Home");
        }

        /// <summary>
        /// Naya WhiteLabel user register karta hai aur uski details database mein save karta hai
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> insert_WhiteLabel(RegisterViewModel rem)
        {
            using (var transaction = DB.Database.BeginTransaction())
            {
                try
                {
                    using (VastwebmultiEntities db = new VastwebmultiEntities())
                    {
                        var chkmobile = db.Users.Where(a => a.PhoneNumber == rem.Mobile).Any();
                        if (chkmobile == true)
                        {
                            TempData["error"] = "This Mobile Number Already Exists";
                            return RedirectToAction("NewLogin", "Home");
                        }
                        var userid = User.Identity.GetUserId();
                        var check = db.Whitelabel_Retailer_Details.Where(es => es.Mobile == rem.Mobile).Any();
                        if (check == false)
                        {
                            var user = new ApplicationUser { UserName = rem.Email, Email = rem.Email, PhoneNumber = rem.Mobile };
                            //Generate Random Password
                            bool includeLowercase = false;
                            bool includeUppercase = false;
                            bool includeNumeric = true;
                            bool includeSpecial = false;
                            bool includeSpaces = false;
                            int lengthOfPassword = 8;

                            string pass = PasswordGenerator.GeneratePassword(includeLowercase, includeUppercase, includeNumeric, includeSpecial, includeSpaces, lengthOfPassword);
                            while (!PasswordGenerator.PasswordIsValid(includeLowercase, includeUppercase, includeNumeric, includeSpecial, includeSpaces, pass))
                            {
                                pass = PasswordGenerator.GeneratePassword(includeLowercase, includeUppercase, includeNumeric, includeSpecial, includeSpaces, lengthOfPassword);
                            }
                            var Password = pass;
                            int pin = new Random().Next(1000, 10000);
                            var enpin = Encrypt(pin.ToString());
                            var result = await UserManager.CreateAsync(user, Password);
                            if (result.Succeeded)
                            {
                                System.Data.Entity.Core.Objects.ObjectParameter output = new
                                 System.Data.Entity.Core.Objects.ObjectParameter("Output", typeof(string));
                                var ch = db.whitelabel_Insert_Retailer(rem.Distributorid, user.Id, rem.Name, Convert.ToInt32(rem.state), Convert.ToInt32(rem.distict), rem.Mobile, "", 0, rem.Email, "", "", "", "", 0, "", enpin, "", output).SingleOrDefault().msg;
                                transaction.Commit();
                                // Send an email with this link
                                string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                                var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                                callbackUrl = callbackUrl.Replace("/WHITELABEL", "");
                                string body = new CommonUtil().PopulateBody("", "Confirm your account", "", "" + callbackUrl + "", rem.Email, Password, pin.ToString(), "");
                                new CommonUtil().Insertsendmail(rem.Email, "Confirm your account", body, callbackUrl);

                                ResendConfirmMail resend = new ResendConfirmMail();
                                resend.CallBackUrl = callbackUrl;
                                resend.Email = rem.Email;
                                resend.Password = Password;
                                resend.Pin = pin.ToString();
                                db.ResendConfirmMails.Add(resend);
                                db.SaveChanges();

                                if (ch.ToString() == "Register SuccessFully.")
                                {
                                    TempData["aa"] = "'Account Created Successfully' Welcome to our family. Dear Business Partner, Please verify your email first";
                                    return RedirectToAction("NewLogin", "Home");
                                }
                                else
                                {
                                    transaction.Rollback();
                                    TempData["error"] = ch;
                                    return RedirectToAction("NewLogin", "Home");
                                }

                            }
                            else
                            {
                                TempData["error"] = "Your Email id is Already Exist";
                                return RedirectToAction("NewLogin", "Home");
                            }
                        }
                        else
                        {
                            TempData["error"] = "This Mobile Number Already Exists";
                            return RedirectToAction("NewLogin", "Home");
                        }
                    }
                }

                catch
                {
                    transaction.Rollback();
                    TempData["errorretailer"] = "Now User Not Created. Please Create After Some Time";
                    return RedirectToAction("NewLogin", "Home");
                }
            }
        }

        // ✅ GET: Signup
        
        
        //Insert Retailer
        /// <summary>
        /// Naye retailer ka registration karta hai, captcha verify karke user details save karta hai
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Insert_retailer(RegisterViewModel rem, string CaptchaInput)
        {

            var appDbContext = HttpContext.GetOwinContext().Get<ApplicationDbContext>();
            using (var transaction = appDbContext.Database.BeginTransaction())
            {
                try
                {
                    rem.Mobile = DB.MobileOtps.Where(s => s.idno == rem.Mobile_Id).Single().mobileno;
                    var email_verify = DB.Email_Mobile_Verify.Single().emailmobile_verified;
                    if (email_verify != "OFF")
                    {
                        rem.Email = DB.MobileOtps.Where(s => s.idno == rem.Email_Id).Single().mobileno;
                    }

                    var distributorid = "";
                    var referelcode = "";

                    // ---------------- Referral Code Check ----------------
                    if (!string.IsNullOrEmpty(rem.referralcode))
                    {
                        var chkreffralcode = DB.Retailer_Details
                            .FirstOrDefault(x => x.SelfReferalCode == rem.referralcode);

                        if (chkreffralcode == null)
                        {
                            TempData["defaultstatus"] = "Invalid Referral Code.";
                            return View("Login", rem);
                        }

                        // ---------------- Referral Code Limit Check from token_limit ----------------
                        var refferalsetting = DB.token_limit.SingleOrDefault();

                        if (refferalsetting == null)
                        {
                            TempData["defaultstatus"] = "Referral limit setting not found. Please contact admin.";
                            return View("Login", rem);
                        }

                        var chknoofrefral = DB.Retailer_Details
                            .Count(x => x.referralcode == rem.referralcode);

                        if (chknoofrefral >= refferalsetting.limit)
                        {
                            TempData["defaultstatus"] = "Referral Code is already used.";
                            return View("Login", rem);
                        }

                        // अगर यहां तक पहुंचा है तो मतलब referral code valid है
                        referelcode = rem.referralcode;
                    }
                    else
                    {
                        // Referral Code blank allow करो
                        referelcode = ""; // DB में खाली save होगा
                    }
                    // ---------------------------------------------------------------

                    var status = DB.Dealer_Details.Where(pp => pp.DefaultStatus == "Y").ToList();
                    if (status.Count == 0)
                    {
                        TempData["defaultstatus"] = "You are not authorized to create. Please contact customer care.";
                        return RedirectToAction("Login", "Home");
                    }
                    else
                    {
                        distributorid = status.SingleOrDefault().DealerId;
                    }

                    var chkmobile = DB.Users.Where(a => a.PhoneNumber == rem.Mobile).Any();
                    if (chkmobile == true)
                    {
                        TempData["mobileno"] = "This Mobile Number already exists.";
                        return RedirectToAction("Login", "Home");
                    }
                    if (DB.Dealer_Details.Any(a => a.Mobile == rem.Mobile))
                    {
                        TempData["mobileno"] = "This Mobile Number already exists.";
                        return RedirectToAction("Login", "Home");
                    }
                    else if (DB.Dealer_Details.Any(a => a.Email == rem.Email))
                    {
                        TempData["emailconfirm"] = "This Email Id already exists.";
                        return RedirectToAction("Login", "Home");
                    }
                    else
                    {
                        var check = DB.Retailer_Details.Where(es => es.Mobile == rem.Mobile).Any();

                        if (check == false)
                        {
                            var user = new ApplicationUser { UserName = rem.Email, Email = rem.Email, PhoneNumber = rem.Mobile };

                            var enpin = Encrypt(rem.Pin.ToString());
                            var result = await UserManager.CreateAsync(user, rem.Password);
                            if (result.Succeeded)
                            {
                                System.Data.Entity.Core.Objects.ObjectParameter output =
                                    new System.Data.Entity.Core.Objects.ObjectParameter("Output", typeof(string));

                                string Firmname = !string.IsNullOrWhiteSpace(rem.Companyname) ? rem.Companyname : rem.Name;
                                var frmname = DB.Retailer_Details.Any(x => x.Frm_Name.ToUpper() == Firmname.ToUpper());

                                if (frmname)
                                {
                                    var frmnamecontain = DB.Retailer_Details
                                        .Where(x => x.Frm_Name.ToUpper() == Firmname.ToUpper())
                                        .FirstOrDefault().Frm_Name;

                                    var fname = Firmname.First();
                                    var lchar = Firmname.Last();
                                    int i = 0;
                                    while (Firmname.ToUpper() == frmnamecontain.ToUpper())
                                    {
                                        Firmname = Firmname + "_" + fname + lchar + "+" + i;
                                        i++;
                                    }
                                }

                                var ch = DB.Insert_Retailer(
                                    distributorid, user.Id, rem.Name, Convert.ToInt32(rem.state),
                                    Convert.ToInt32(rem.distict), rem.Mobile, "", 0, rem.Email, "", "",
                                    Firmname, "", "", 0, "", enpin.ToString(), referelcode, "Signup", output
                                ).SingleOrDefault().msg;

                                if (ch == "Register SuccessFully.")
                                {
                                    if (transaction.UnderlyingTransaction.Connection != null)
                                    {
                                        transaction.Commit();
                                    }
                                    var users = DB.Users.Where(a => a.Email == rem.Email && a.PhoneNumber == rem.Mobile).SingleOrDefault();
                                    if (email_verify != "OFF")
                                    {
                                        users.EmailConfirmed = true;
                                    }
                                    users.PhoneNumberConfirmed = true;
                                    DB.SaveChanges();
                                }
                                else
                                {
                                    if (transaction.UnderlyingTransaction.Connection != null)
                                    {
                                        transaction.Rollback();
                                    }
                                    else
                                    {
                                        transaction.UnderlyingTransaction.Connection.Open();
                                        transaction.Rollback();
                                    }
                                    TempData["mobileno"] = ch;
                                    return RedirectToAction("Login", "Home");
                                }

                                // Send an email with this link
                                string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                                var callbackUrl = Url.Action("ConfirmEmailAdmin", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                                callbackUrl = callbackUrl.Replace("/ADMIN", "");

                                string body = new CommonUtil().PopulateBody(rem.Email, "Confirm your account", "", "" + callbackUrl + "", rem.Email, rem.Password, rem.Pin.ToString(), rem.referralcode);
                                string Welcomebody = new CommonUtil().PopulateBodyWelcome(rem.Email, "Confirm your account", "", "SignUp" + callbackUrl + "", rem.Email, rem.Password, rem.Pin.ToString(), rem.referralcode);

                                new CommonUtil().Insertsendmail(rem.Email, "Confirm your account", body, callbackUrl);
                                new CommonUtil().InsertsendmailWelcome(rem.Email, "Confirm your account", Welcomebody, callbackUrl);

                                var adminemail = DB.Admin_details.SingleOrDefault().email;
                                new CommonUtil().Rsendmailadmin(adminemail, "Confirm your account", body, callbackUrl);

                                ResendConfirmMail resend = new ResendConfirmMail();
                                resend.CallBackUrl = callbackUrl;
                                resend.Email = rem.Email;
                                resend.Password = rem.Password;
                                resend.Pin = rem.Pin.ToString();
                                DB.ResendConfirmMails.Add(resend);
                                DB.SaveChanges();

                                if (ch.ToString() == "Register SuccessFully.")
                                {
                                    var resultssss = await SignInManager.PasswordSignInAsync(rem.Email, rem.Password, isPersistent: false, shouldLockout: false);
                                    TempData["success"] = "Welcome to our family. Dear Business Partner, your account has been successfully created. Please verify your email.";
                                    return RedirectToAction("Dashboard", "Home", new { area = "Retailer" });
                                }
                                else
                                {
                                    transaction.Rollback();
                                    TempData["mobileno"] = ch;
                                    return RedirectToAction("Login", "Home");
                                }
                            }
                            else
                            {
                                var ss = "";
                                foreach (var error in result.Errors)
                                {
                                    ss = error;
                                }
                                TempData["emailconfrim"] = ss;
                                return RedirectToAction("Login", "Home");
                            }
                        }
                        else
                        {
                            TempData["mobileno"] = "This Mobile Number already exists.";
                            return RedirectToAction("Login", "Home");
                        }
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    TempData["errorretailer"] = "User not created. Please try again later.";
                    return RedirectToAction("Login", "Home");
                }
            }
        }


        
        /*public async Task<ActionResult> Insert_retailer(RegisterViewModel rem)
        {
            var appDbContext = HttpContext.GetOwinContext().Get<ApplicationDbContext>();
            using (var transaction = appDbContext.Database.BeginTransaction())
            {
                try
                {
                    rem.Mobile = DB.MobileOtps.Where(s => s.idno == rem.Mobile_Id).Single().mobileno;
                    var email_verify = DB.Email_Mobile_Verify.Single().emailmobile_verified;
                    if (email_verify != "OFF")
                    {
                        rem.Email = DB.MobileOtps.Where(s => s.idno == rem.Email_Id).Single().mobileno;
                    }
                    //var Password = "";
                    //   var ConfirmPassword = "";
                    // var slab = "";
                    var distributorid = "";
                    var referelcode = "";
                    var chkreffralcode = DB.Retailer_Details.Where(x => x.SelfReferalCode == rem.referralcode && x.CretedBy != "Signup").SingleOrDefault();
                    var refferalsetting = DB.token_limit.SingleOrDefault();
                    if (refferalsetting == null)
                    {
                        if (chkreffralcode != null)
                        {
                            var chknoofrefral = DB.Retailer_Details.Where(x => x.referralcode == rem.referralcode).Count();
                            if (chknoofrefral > 3)
                            {
                                TempData["defaultstatus"] = "Referal Code is Already Used";
                                return RedirectToAction("Login", "Home");
                            }
                            referelcode = rem.referralcode;


                        }
                        else if (chkreffralcode == null)
                        {
                            TempData["defaultstatus"] = "Please Enter Valid Referal Code";
                            return RedirectToAction("Login", "Home");
                        }
                    }
                    else
                    {
                        var referelstatus = refferalsetting.status;
                        if (referelstatus == "Y")
                        {
                            var chknoofrefral = DB.Retailer_Details.Where(x => x.referralcode == rem.referralcode).Count();
                            if (chkreffralcode == null)
                            {
                                TempData["defaultstatus"] = "Referal Code Does Not Exist";
                                return RedirectToAction("Login", "Home");
                            }
                            else if (chknoofrefral >= refferalsetting.limit)
                            {
                                TempData["defaultstatus"] = "Referal Code is Already Used";
                                return RedirectToAction("Login", "Home");
                            }
                            referelcode = rem.referralcode;
                        }
                    }

                    var status = DB.Dealer_Details.Where(pp => pp.DefaultStatus == "Y").ToList();
                    if (status.Count == 0)
                    {
                        TempData["defaultstatus"] = "You Don't Authoritrized to Create. Please Contact Custmor Care";
                        return RedirectToAction("Login", "Home");
                    }
                    else
                    {
                        distributorid = status.SingleOrDefault().DealerId;
                    }

                    var chkmobile = DB.Users.Where(a => a.PhoneNumber == rem.Mobile).Any();
                    if (chkmobile == true)
                    {
                        TempData["mobileno"] = "This Mobile Number Already Exists";
                        return RedirectToAction("Login", "Home");
                    }
                    if (DB.Dealer_Details.Any(a => a.Mobile == rem.Mobile))
                    {
                        TempData["mobileno"] = "This Mobile Number Already Exists";
                        return RedirectToAction("Login", "Home");
                    }
                    else if (DB.Dealer_Details.Any(a => a.Email == rem.Email))
                    {
                        TempData["emailconfirm"] = "This Email Id Already Exists";
                        return RedirectToAction("Login", "Home");
                    }

                    else
                    {

                        var check = DB.Retailer_Details.Where(es => es.Mobile == rem.Mobile).Any();

                        if (check == false)
                        {
                            var user = new ApplicationUser { UserName = rem.Email, Email = rem.Email, PhoneNumber = rem.Mobile };

                            var enpin = Encrypt(rem.Pin.ToString());
                            var result = await UserManager.CreateAsync(user, rem.Password);
                            if (result.Succeeded)
                            {
                                System.Data.Entity.Core.Objects.ObjectParameter output = new
                                 System.Data.Entity.Core.Objects.ObjectParameter("Output", typeof(string));
                                string Firmname = "";
                                if (!string.IsNullOrWhiteSpace(rem.Companyname))
                                {
                                    Firmname = rem.Companyname;
                                }
                                else
                                {
                                    Firmname = rem.Name;
                                }
                                var frmname = DB.Retailer_Details.Where(x => x.Frm_Name.ToUpper() == Firmname.ToUpper()).Any();

                                if (frmname == true)
                                {
                                    var frmnamecontain = DB.Retailer_Details.Where(x => x.Frm_Name.ToUpper() == Firmname.ToUpper()).FirstOrDefault().Frm_Name;
                                    var fname = Firmname.First();
                                    var lchar = Firmname.Last();
                                    int i = 0;
                                    while (Firmname.ToUpper() == frmnamecontain.ToUpper())
                                    {
                                        Firmname = Firmname + "_" + fname + lchar + "+" + i;
                                        i++;
                                    }

                                    // Firmname = Firmname + "_" + fname + lchar;
                                }

                                var ch = DB.Insert_Retailer(distributorid, user.Id, rem.Name, Convert.ToInt32(rem.state), Convert.ToInt32(rem.distict), rem.Mobile, "", 0, rem.Email, "", "", Firmname, "", "", 0, "", enpin.ToString(), referelcode, "Signup", output).SingleOrDefault().msg;

                                if (ch == "Register SuccessFully.")
                                {

                                    if (transaction.UnderlyingTransaction.Connection != null)
                                    {
                                        transaction.Commit();
                                    }
                                    var users = DB.Users.Where(a => a.Email == rem.Email && a.PhoneNumber == rem.Mobile).SingleOrDefault();
                                    if (email_verify != "OFF")
                                    {
                                        users.EmailConfirmed = true;
                                    }

                                    users.PhoneNumberConfirmed = true;
                                    DB.SaveChanges();
                                }
                                else
                                {
                                    if (transaction.UnderlyingTransaction.Connection != null)
                                    {
                                        transaction.Rollback();
                                    }
                                    else
                                    {
                                        transaction.UnderlyingTransaction.Connection.Open();
                                        transaction.Rollback();
                                    }
                                    TempData["mobileno"] = ch;
                                    return RedirectToAction("Login", "Home");
                                }
                                // Send an email with this link
                                string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                                var callbackUrl = Url.Action("ConfirmEmailAdmin", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                                callbackUrl = callbackUrl.Replace("/ADMIN", "");
                                string body = new CommonUtil().PopulateBody(rem.Email, "Confirm your account", "", "" + callbackUrl + "", rem.Email, rem.Password, rem.Pin.ToString(), rem.referralcode);
                                string Welcomebody = new CommonUtil().PopulateBodyWelcome(rem.Email, "Confirm your account", "", "SignUp" + callbackUrl + "", rem.Email, rem.Password, rem.Pin.ToString(), rem.referralcode);
                                new CommonUtil().Insertsendmail(rem.Email, "Confirm your account", body, callbackUrl);
                                new CommonUtil().InsertsendmailWelcome(rem.Email, "Confirm your account", Welcomebody, callbackUrl);
                                var adminemail = DB.Admin_details.SingleOrDefault().email;
                                new CommonUtil().Rsendmailadmin(adminemail, "Confirm your account", body, callbackUrl);
                                ResendConfirmMail resend = new ResendConfirmMail();
                                resend.CallBackUrl = callbackUrl;
                                resend.Email = rem.Email;
                                resend.Password = rem.Password;
                                resend.Pin = rem.Pin.ToString();
                                DB.ResendConfirmMails.Add(resend);
                                DB.SaveChanges();

                                if (ch.ToString() == "Register SuccessFully.")
                                {

                                    var resultssss = await SignInManager.PasswordSignInAsync(rem.Email, rem.Password, isPersistent: false, shouldLockout: false);
                                    TempData["success"] = "Welcome to our family. Dear Business Partner Your account has been successfully created, first verify your email";

                                    return RedirectToAction("Dashboard", "Home", new { area = "Retailer" });
                                    //  await SignInManager.SignInAsync(user, true, false);
                                    //  TempData["success"] = "Welcome to our family. Dear Business Partner Your account has been successfully created, first verify your email";
                                    //   return RedirectToAction("VeryFY_Profiles_users", "Home");
                                    //   return RedirectToAction("Login", "Home");
                                    //   return RedirectToAction("Dashboard", "Home", new { area = "Retailer" });
                                }
                                else
                                {
                                    transaction.Rollback();
                                    TempData["mobileno"] = ch;
                                    return RedirectToAction("Login", "Home");
                                }

                            }

                            else
                            {
                                var ss = "";
                                foreach (var error in result.Errors)
                                {
                                    ss = error;
                                }
                                TempData["emailconfrim"] = ss;
                                return RedirectToAction("Login", "Home");
                            }

                        }

                        else
                        {
                            TempData["mobileno"] = "This Mobile Number Already Exists";
                            return RedirectToAction("Login", "Home");
                        }
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    TempData["errorretailer"] = "User Not Created. Please Create After Some Time";
                    return RedirectToAction("Login", "Home");
                }
            }
        }

*/




        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                TempData["errorretailer"] = "Password must be, at least one uppercase letter, one lowercase letter one number and one special character:";
            }

        }

        static readonly string PasswordHash = "P@@Sw0rd";
        static readonly string SaltKey = "S@LT&KEY";
        static readonly string VIKey = "@1B2c3D4e5F6g7H8";
        public static string Encrypt(string plainText)
        {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.Zeros };
            var encryptor = symmetricKey.CreateEncryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));

            byte[] cipherTextBytes;

            using (var memoryStream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                    cryptoStream.FlushFinalBlock();
                    cipherTextBytes = memoryStream.ToArray();
                    cryptoStream.Close();
                }
                memoryStream.Close();
            }
            return Convert.ToBase64String(cipherTextBytes);
        }
        //
        /// <summary>
        /// Two-factor verification code ka page dikhata hai jahan user OTP enter karta hai
        /// </summary>
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            TempData["provider"] = provider;
            ViewBag.returnUrl = returnUrl;

            ViewBag.rememberMe = true;

            var chk = await SignInManager.HasBeenVerifiedAsync();
            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync("Email Code"))
            {
                return View("Error");
            }
            if (!await SignInManager.SendTwoFactorCodeAsync("Phone Code"))
            {
                return View("Error");
            }
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            TempData["resend"] = "Send Verification Code Successfully.Please Check Your Email";
            return View();
        }
        //
        // POST: /Account/VerifyCode
        [HttpPost]
        /// <summary>
        /// User dwara submit kiya gaya verification code validate karke login complete karta hai
        /// </summary>
        [AllowAnonymous]

        public async Task<ActionResult> VerifyCode(string ReturnUrl, string Provider, string Code)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync("Email Code", Code, isPersistent: true, rememberBrowser: true);
            if (result == SignInStatus.Failure)
            {
                result = await SignInManager.TwoFactorSignInAsync("Phone Code", Code, isPersistent: true, rememberBrowser: true);
            }
            switch (result)
            {
                case SignInStatus.Success:
                    TempData.Keep("Userid");
                    TempData.Keep("Password");
                    return RedirectToLocal(ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    TempData["invalidcode"] = "Verification Code Is Incorrect, Please Re-Enter Verification Code";
                    return View();
            }
        }


        /// <summary>
        /// Admin ke liye two-factor verification code page dikhata hai
        /// </summary>
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCodeAdmin(string provider, string returnUrl, string type, bool rememberMe)
        {
            TempData["provider"] = provider;
            ViewBag.returnUrl = returnUrl;
            rememberMe = true;
            ViewBag.rememberMe = rememberMe;
            var chk = await SignInManager.HasBeenVerifiedAsync();
            // Generate the token and send it on mail
            if (!await SignInManager.SendTwoFactorCodeAsync("Email Code"))
            {
                return View("Error");
            }
            // Generate the token and send it on Mobile
            if (!await SignInManager.SendTwoFactorCodeAsync("Phone Code"))
            {
                return View("Error");
            }
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            if (type == "Resend")
            {
                var message1 = "OTP Resended Successfully On Registered Email / Mobile";
                TempData["invalidcode"] = message1;
            }
            else
            {
                var message1 = "OTP has been successfully sent to registered Email and Mobile number";
                var message2 = "Please do not share the OTP with anyone, Doing so may cause harm";
                TempData["resend"] = message1;
                TempData["resend2"] = message2;
            }

            return View();
        }
        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]

        public async Task<ActionResult> VerifyCodeAdmin(string ReturnUrl, string Provider, string Code)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync("Email Code", Code, isPersistent: true, rememberBrowser: true);
            if (result == SignInStatus.Failure)
            {
                result = await SignInManager.TwoFactorSignInAsync("Phone Code", Code, isPersistent: true, rememberBrowser: true);
            }
            switch (result)
            {
                case SignInStatus.Success:

                    try
                    {
                        var Email = Convert.ToString(TempData["loginuseremailidtem"]);
                        var latloc = Convert.ToString(TempData["longitudetemp"]);
                        var longloc = Convert.ToString(TempData["latitudestemp"]);

                        string lat = ""; string longt = "";
                        string city = ""; string address = "";
                        var Ipaddress = Request.UserHostAddress;
                        var usr = UserManager.FindByEmail(Email);
                        try
                        {
                            var ipchk = "";
                            if (Ipaddress != "::1")
                            {
                                ipchk = Ipaddress;
                            }
                            var client = new RestClient("http://ip-api.com/json/" + ipchk + "");
                            client.Timeout = -1;
                            var request = new RestRequest(Method.GET);
                            var task = Task.Run(() =>
                            {
                                return client.Execute(request).Content;
                            });
                            bool isCompletedSuccessfully = task.Wait(TimeSpan.FromMilliseconds(10000));
                            if (isCompletedSuccessfully == true)
                            {
                                var resp = task.Result;
                                dynamic respo = JsonConvert.DeserializeObject(resp);
                                string status = respo.status;
                                if (status == "success")
                                {
                                    string country = respo.country;
                                    string regionName = respo.regionName;
                                    city = respo.city;
                                    string zip = respo.zip;
                                    lat = respo.lat;
                                    longt = respo.lon;
                                    address = regionName + " , " + country + " , " + zip;
                                }
                            }
                        }
                        catch { }

                        using (VastwebmultiEntities db = new VastwebmultiEntities())
                        {

                            Login_info Logininfo = new Login_info();
                            Logininfo.CurrentLoginTime = DateTime.Now;
                            Logininfo.IP_Address = Ipaddress;
                            Logininfo.Latitude = lat;
                            Logininfo.Logitude = longt;
                            Logininfo.City = city;
                            Logininfo.Location = address;
                            Logininfo.LastLoginTime = db.Login_info.Where(a => a.UserId == Email).OrderByDescending(a => a.Idno).FirstOrDefault() != null ?
                            db.Login_info.Where(a => a.UserId == Email).OrderByDescending(a => a.Idno).FirstOrDefault().CurrentLoginTime : DateTime.Now;
                            Logininfo.LoginFrom = "Web";
                            Logininfo.UserId = Email;
                            Logininfo.Latitude = latloc;
                            Logininfo.Logitude = longloc;
                            Logininfo.browser = Request.Browser.Browser + " " + Request.Browser.Version;
                            db.Login_info.Add(Logininfo);

                            var app = db.Users.FirstOrDefault(ff => ff.UserId == usr.Id);
                            if (app.LockoutEndDateUtc != null)
                            {
                                User obj = (from dd in db.Users
                                            where dd.UserId == usr.Id
                                            select dd).FirstOrDefault();
                                obj.LockoutEndDateUtc = null;


                            }
                            db.SaveChanges();
                        }
                    }
                    catch
                    {

                    }
                    return RedirectToAction("Login");
                case SignInStatus.LockedOut:
                    return View("LockoutAdmin");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    TempData["invalidcode"] = "⚠️ OTP is Invalid or Expired, Please Enter Correct One Time Password";
                    return View();
            }
        }


        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View();
            }
            var emailConfirmationCode = await UserManager.GenerateEmailConfirmationTokenAsync(userId);
            var result = await UserManager.ConfirmEmailAsync(userId, emailConfirmationCode);
            if (result.Succeeded)
            {
                var emailid = DB.Users.Where(a => a.UserId == userId).SingleOrDefault().Email;
                var count = DB.ResendConfirmMails.Where(m => m.Email == emailid).ToList();
                if (count.Count > 0)
                {
                    var v = DB.ResendConfirmMails.Single(m => m.Email == emailid);
                    DB.ResendConfirmMails.Remove(v);
                    DB.SaveChanges();
                }
                else
                {
                    return View(result.Succeeded ? "ConfirmEmail" : "Error");
                }
            }
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmailAdmin(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error_Admin");
            }
            var emailConfirmationCode = await UserManager.GenerateEmailConfirmationTokenAsync(userId);
            var result = await UserManager.ConfirmEmailAsync(userId, emailConfirmationCode);
            if (result.Succeeded)
            {
                try
                {
                    var emailid = DB.Users.Where(a => a.UserId == userId).SingleOrDefault().Email;
                    var count = DB.ResendConfirmMails.Where(m => m.Email == emailid).ToList();
                    if (count.Count > 0)
                    {
                        var v = DB.ResendConfirmMails.Single(m => m.Email == emailid);
                        DB.ResendConfirmMails.Remove(v);
                        DB.SaveChanges();
                    }
                    else
                    {
                        return View(result.Succeeded ? "ConfirmEmailAdmin" : "Error_Admin");
                    }
                }
                catch { }
            }
            return View(result.Succeeded ? "ConfirmEmailAdmin" : "Error_Admin");
        }
        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            VastwebmultiEntities db = new VastwebmultiEntities();
            string currenturl = HttpContext.Request.Url.Authority;
            currenturl = currenturl.Replace("www.", "").Replace("https://", "").Replace("http://", "");
            currenturl = currenturl.ToUpper();
            var whitelabelid = "";
            if (currenturl.Contains("LOCALHOST"))
            {
                whitelabelid = db.WhiteLabel_userList.Where(aa => aa.localhost.Replace("www.", "").Replace("https://", "").Replace("http://", "").ToLower() == currenturl.ToLower()).SingleOrDefault().WhiteLabelID;
            }
            else
            {
                whitelabelid = db.WhiteLabel_userList.Where(aa => aa.websitename.Replace("www.", "").Replace("https://", "").Replace("http://", "").ToLower() == currenturl.ToLower()).SingleOrDefault().WhiteLabelID;
            }



            if (ModelState.IsValid)
            {
                var findemail = DB.Users.Where(x => x.PhoneNumber == model.Email).ToList();
                if (findemail.Any())
                {
                    model.Email = findemail.SingleOrDefault().Email;
                }
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null)
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    TempData["newmessage"] = "Email Id Not Exist With Us.";
                    return RedirectToAction("NewLogin", "Home");
                }

                bool? highchk = true;
                try
                {
                    highchk = DB.security_settings.SingleOrDefault().highsecurity;
                }
                catch { }
                if (highchk == true)
                {
                    var userid = db.Users.Where(a => a.Email == model.Email).SingleOrDefault().UserId;
                    var chk22 = db.checklogouts.Where(a => a.userid == userid).SingleOrDefault();
                    if (chk22 == null)
                    {
                        checklogout ch = new checklogout();
                        ch.userid = userid;
                        ch.lastupdatedate = DateTime.UtcNow;
                        db.checklogouts.Add(ch);
                        db.SaveChanges();

                    }
                    else
                    {
                        chk22.lastupdatedate = DateTime.UtcNow;
                        db.SaveChanges();
                    }

                    var chk = db.passwordchanges.Where(aa => aa.userid.ToUpper() == model.Email.ToUpper()).ToList();
                    foreach (var item in chk)
                    {
                        item.change = true;
                        db.SaveChanges();
                    }
                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    //Send an email with this link
                    string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                    var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    //await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking " + callbackUrl + "");
                    string body = this.ForgotBody(user.Email, "Reset Password", "", "" + callbackUrl + "");
                    this.SendForgotpasswordEmail_whitelabel(whitelabelid, user.Email, "Confirm your account", body);
                    var adminemail = DB.WhiteLabel_userList.Where(s => s.WhiteLabelID == whitelabelid).SingleOrDefault().EmailId;
                    new CommonUtil().Rsendmailadmin(adminemail, "Confirm your account", body, callbackUrl);
                    var userroleid = DB.UserRoles.Where(aa => aa.UserId == user.Id).SingleOrDefault().RoleId;
                    var userrole = DB.Roles.Where(aa => aa.RoleId == userroleid).SingleOrDefault().Name;
                    // change security code
                    await UserManager.UpdateSecurityStampAsync(User.Identity.GetUserId());


                    forgottable fot = new forgottable();
                    fot.adminid = whitelabelid;
                    fot.Email = user.Email;
                    fot.Userid = user.Id;
                    fot.Roles = userrole;
                    fot.date = DateTime.Now;
                    fot.forgotlink = callbackUrl;
                    DB.forgottables.Add(fot);
                    DB.SaveChanges();
                    return RedirectToAction("ForgotPasswordConfirmation", "Account");
                }
                else
                {
                    bool includeLowercase = false;
                    bool includeUppercase = false;
                    bool includeNumeric = true;
                    bool includeSpecial = false;
                    bool includeSpaces = false;
                    int lengthOfPassword = 8;
                    string pass = PasswordGenerator.GeneratePassword(includeLowercase, includeUppercase, includeNumeric, includeSpecial, includeSpaces, lengthOfPassword);
                    while (!PasswordGenerator.PasswordIsValid(includeLowercase, includeUppercase, includeNumeric, includeSpecial, includeSpaces, pass))
                    {
                        pass = PasswordGenerator.GeneratePassword(includeLowercase, includeUppercase, includeNumeric, includeSpecial, includeSpaces, lengthOfPassword);
                    }
                    var Password = pass;
                    var result = await UserManager.RemovePasswordAsync(user.Id);
                    result = await UserManager.AddPasswordAsync(user.Id, pass);
                    // change security code
                    await UserManager.UpdateSecurityStampAsync(User.Identity.GetUserId());


                    //var smsapichk = db.whitelabel_apisms.Where(aa => aa.sts == "Y" && aa.userfor == whitelabelid).SingleOrDefault();
                    var websiteurl = db.WhiteLabel_userList.Where(aa => aa.WhiteLabelID == whitelabelid).SingleOrDefault().websitename;
                    Password = System.Web.HttpUtility.UrlEncode(Password);
                    var txtMsgBody = "Your New Password is " + Password + " Thank you for business with us";
                    //if (smsapichk != null)
                    //{
                    //    string urlss = "";
                    //    string msgssss = "";
                    //    string tempid = "";
                    //    try
                    //    {
                    //        var chk = db.passwordchanges.Where(aa => aa.userid.ToUpper() == model.Email.ToUpper()).ToList();
                    //        foreach (var item in chk)
                    //        {
                    //            item.change = true;
                    //            db.SaveChanges();
                    //        }
                    //        var smsapionsts = DB.whitelabel_apisms.Where(x => x.sts == "Y" && x.userfor == whitelabelid).SingleOrDefault();
                    //        var smsstypes = DB.Whitelabel_Sending_SMS_Templates.Where(x => x.SMS_TYPE == "NEWPASSWORDIS" && x.SMSAPIID == smsapionsts.id && x.whitelabelId == whitelabelid).SingleOrDefault();
                    //        if (smsstypes != null)
                    //        {
                    //            msgssss = string.Format(smsstypes.Templates, Password);
                    //            tempid = smsstypes.Templateid;
                    //            urlss = smsapionsts.smsapi;

                    //            // smssend.sendsmsallnew(user.PhoneNumber, msgssss, urlss, tempid);

                    //            var apinamechange = urlss.Replace("tttt", user.PhoneNumber).Replace("mmmm", msgssss).Replace("iiii", tempid);
                    //            HttpWebRequest WebRequestObject = (HttpWebRequest)HttpWebRequest.Create(apinamechange);
                    //            WebRequestObject.Timeout = (System.Int32)TimeSpan.FromSeconds(135).TotalMilliseconds;

                    //            WebResponse Response = WebRequestObject.GetResponse();
                    //            Stream WebStream = Response.GetResponseStream();
                    //            StreamReader Reader = new StreamReader(WebStream);
                    //            var s = Reader.ReadToEnd();
                    //            whitelabel_sms_api_entry sms = new whitelabel_sms_api_entry();
                    //            sms.apiname = apinamechange;
                    //            sms.msg = msgssss;
                    //            sms.messagefor = user.Id;
                    //            sms.m_date = System.DateTime.Now;
                    //            sms.response = s;
                    //            sms.userid = whitelabelid;
                    //            db.whitelabel_sms_api_entry.Add(sms);
                    //            db.SaveChanges();
                    //        }
                    //    }
                    //    catch (Exception) { }

                    //}

                    var userid = db.Users.Where(a => a.Email == model.Email).SingleOrDefault().UserId;
                    var chk22 = db.checklogouts.Where(a => a.userid == userid).SingleOrDefault();
                    if (chk22 == null)
                    {
                        checklogout ch = new checklogout();
                        ch.userid = userid;
                        ch.lastupdatedate = DateTime.UtcNow;
                        db.checklogouts.Add(ch);
                        db.SaveChanges();

                    }
                    else
                    {
                        chk22.lastupdatedate = DateTime.UtcNow;
                        db.SaveChanges();
                    }

                    var chk = db.passwordchanges.Where(aa => aa.userid.ToUpper() == model.Email.ToUpper()).ToList();
                    foreach (var item in chk)
                    {
                        item.change = true;
                        db.SaveChanges();
                    }

                    smssend.sms_init_whitelabel(whitelabelid, "Y", "Y", "NEWPASSWORDIS", user.PhoneNumber, Password);

                    try
                    {
                        txtMsgBody = "Your New Password is " + pass + " Thank you for business with us ";
                        this.SendForgotpasswordEmail_whitelabel(whitelabelid, model.Email, "Forgot Password", txtMsgBody);
                    }
                    catch { }

                    TempData["newmessage"] = "New Password Send Successfully, Check Your Mail Or Phone";
                    return RedirectToAction("NewLogin", "Home");
                }
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }


        [AllowAnonymous]
        public ActionResult ForgotPasswordAdmin()
        {
            return View();
        }

        ALLSMSSend smssend = new ALLSMSSend();

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPasswordAdmin(ForgotPasswordViewModel model , string oTP,string oTP1, string oTP2, string oTP3)

        {
            string OTp = oTP + oTP1 + oTP2 + oTP3;
            if (oTP == null && oTP1 == null && oTP2 == null && oTP3 == null)
            {
                OTp = null;
            }
            var chekset = DB.Email_show_passcode.Where(s => s.forgetotp == true).SingleOrDefault();
          
            var setopt = DB.Users.Where(s => s.Email == model.Email).SingleOrDefault();
            if (setopt == null)
            {
                setopt = DB.Users.Where(s => s.PhoneNumber == model.Email).SingleOrDefault();
            }
            
            if (setopt.forgetpin == OTp || OTp == null)
            {


                if (ModelState.IsValid)
                {
                    var emailids = DB.Users.Where(x => x.PhoneNumber == model.Email).ToList();

                    if (emailids.Any())
                    {
                        model.Email = emailids.SingleOrDefault().Email;
                    }

                    var user = await UserManager.FindByNameAsync(model.Email);
                    if (user == null)
                    {
                        // Don't reveal that the user does not exist or is not confirmed
                        TempData["newmessage"] = "Email Id Not Exist With Us.";
                        return RedirectToAction("Login", "Home");
                    }
                    var deleteretailerfind = DB.Retailer_Details.Where(x => x.RetailerId == user.Id).SingleOrDefault();

                    if (deleteretailerfind != null)
                    {
                        if (deleteretailerfind.ISDeleteuser == true)
                        {
                            TempData["newmessage"] = "Email Id Deleted";
                            return RedirectToAction("Login", "Home");
                        }
                    }

                    var userroladminid = DB.UserRoles.Where(aa => aa.UserId == user.Id).SingleOrDefault().RoleId;
                    var userrolename = DB.Roles.Where(aa => aa.RoleId == userroladminid).SingleOrDefault().Name;

                    //if (userrolename.ToUpper() == "ADMIN")
                    //{
                    //    TempData["newmessage"] = "Wrong Mobile No.";
                    //    return RedirectToAction("Login", "Home");
                    //}

                    bool? highchk = true;
                    try
                    {
                        highchk = DB.security_settings.SingleOrDefault().highsecurity;
                    }
                    catch { }
                    if (highchk == true)
                    {
                        var userid = DB.Users.Where(a => a.Email == model.Email).SingleOrDefault().UserId;
                        var chk22 = DB.checklogouts.Where(a => a.userid == userid).SingleOrDefault();
                        if (chk22 == null)
                        {
                            checklogout ch = new checklogout();
                            ch.userid = userid;
                            ch.lastupdatedate = DateTime.UtcNow;
                            DB.checklogouts.Add(ch);
                            DB.SaveChanges();

                        }
                        else
                        {
                            chk22.lastupdatedate = DateTime.UtcNow;
                            DB.SaveChanges();
                        }

                        // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                        //Send an email with this link
                        string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                        var callbackUrl = Url.Action("ResetPasswordAdmin", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                        //await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking " + callbackUrl + "");
                        string body = this.ForgotBody(user.Email, "Reset Password", "", "" + callbackUrl + "");
                        this.SendForgotpasswordEmail(user.Email, "Reset Password", body, callbackUrl);
                        var adminemail = DB.Admin_details.SingleOrDefault();
                        new CommonUtil().Rsendmailadmin(adminemail.email, "Confirm your account", body, callbackUrl);
                        //Insert Forgot Password Entry
                        var userroleid = DB.UserRoles.Where(aa => aa.UserId == user.Id).SingleOrDefault().RoleId;
                        var userrole = DB.Roles.Where(aa => aa.RoleId == userroleid).SingleOrDefault().Name;
                        forgottable fot = new forgottable();
                        fot.adminid = adminemail.userid;
                        fot.Email = user.Email;
                        fot.Userid = user.Id;
                        fot.Roles = userrole;
                        fot.date = DateTime.Now;
                        fot.forgotlink = callbackUrl;
                        DB.forgottables.Add(fot);
                        DB.SaveChanges();
                        return RedirectToAction("ForgotPasswordConfirmationAdmin", "Account");
                    }
                    else
                    {
                        var SMS_ForgetPassword = DB.SMSSendAlls.Where(x => x.ServiceName == "ForgetPasswordMails").SingleOrDefault();
                        var Email_ForgetPassworde = DB.EmailSendAlls.Where(x => x.ServiceName == "ForgetPasswordMails1").SingleOrDefault().Status;
                        bool includeLowercase = false;
                        bool includeUppercase = false;
                        bool includeNumeric = true;
                        bool includeSpecial = false;
                        bool includeSpaces = false;
                        int lengthOfPassword = 8;
                        string pass = PasswordGenerator.GeneratePassword(includeLowercase, includeUppercase, includeNumeric, includeSpecial, includeSpaces, lengthOfPassword);
                        while (!PasswordGenerator.PasswordIsValid(includeLowercase, includeUppercase, includeNumeric, includeSpecial, includeSpaces, pass))
                        {
                            pass = PasswordGenerator.GeneratePassword(includeLowercase, includeUppercase, includeNumeric, includeSpecial, includeSpaces, lengthOfPassword);
                        }
                        var Password = pass;
                        var result = await UserManager.RemovePasswordAsync(user.Id);
                        result = await UserManager.AddPasswordAsync(user.Id, pass);
                        var websiteurl = DB.Admin_details.SingleOrDefault().WebsiteUrl;
                        Password = System.Web.HttpUtility.UrlEncode(Password);
                        var txtMsgBody = "Your New Password is " + Password + " Thank you for business with us";
                        var adminfirm = DB.Admin_details.SingleOrDefault().Companyname;
                        txtMsgBody = txtMsgBody + " Regards " + adminfirm;
                        if (Email_ForgetPassworde == "Y")
                        {
                            this.SendForgotpasswordEmail(user.Email, "Reset Password", txtMsgBody, "");
                        }
                        //if (SMS_ForgetPassword == "Y")
                        //{
                        //    if (smsapichk != null)
                        //    {
                        //        string urlss = "";
                        //        string msgssss = "";
                        //        string tempid = "";
                        //        try
                        //        {
                        //            var smsapionsts = DB.apisms.Where(x => x.sts == "Y").SingleOrDefault();
                        //            var smsstypes = DB.Sending_SMS_Templates.Where(x => x.SMS_TYPE == "NEWPASSWORDIS" && x.SMSAPIID == smsapionsts.id).SingleOrDefault();
                        //            if (smsstypes != null)
                        //            {

                        //                msgssss = string.Format(smsstypes.Templates, Password);
                        //                tempid = smsstypes.Templateid;
                        //                urlss = smsapionsts.smsapi;

                        //                // smssend.sendsmsallnew(user.PhoneNumber, msgssss, urlss, tempid);

                        //                var apinamechange = urlss.Replace("tttt", user.PhoneNumber).Replace("mmmm", msgssss).Replace("iiii", tempid);
                        //                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                        //                HttpWebRequest WebRequestObject = (HttpWebRequest)HttpWebRequest.Create(apinamechange);
                        //                WebRequestObject.Timeout = (System.Int32)TimeSpan.FromSeconds(135).TotalMilliseconds;

                        //                WebResponse Response = WebRequestObject.GetResponse();
                        //                Stream WebStream = Response.GetResponseStream();
                        //                StreamReader Reader = new StreamReader(WebStream);
                        //                var s = Reader.ReadToEnd();
                        //                sms_api_entry sms = new sms_api_entry();
                        //                sms.apiname = apinamechange;
                        //                sms.msg = msgssss;
                        //                sms.messagefor = user.Id;
                        //                sms.m_date = System.DateTime.Now;
                        //                sms.response = s;
                        //                DB.sms_api_entry.Add(sms);
                        //                DB.SaveChanges();
                        //            }
                        //        }
                        //        catch (Exception) { }

                        //    }
                        //}
                        var userid = DB.Users.Where(a => a.Email == model.Email).SingleOrDefault().UserId;
                        var chk22 = DB.checklogouts.Where(a => a.userid == userid).SingleOrDefault();
                        if (chk22 == null)
                        {
                            checklogout ch = new checklogout();
                            ch.userid = userid;
                            ch.lastupdatedate = DateTime.UtcNow;
                            DB.checklogouts.Add(ch);
                            DB.SaveChanges();

                        }
                        else
                        {
                            chk22.lastupdatedate = DateTime.UtcNow;
                            DB.SaveChanges();
                        }

                        smssend.sms_init(SMS_ForgetPassword.Status, SMS_ForgetPassword.Whatsapp_Status, "NEWPASSWORDIS", user.PhoneNumber, Password);

                        TempData["newmessage"] = "New Password Send Successfully, Check Your Mail Or Phone";

                        if (setopt.forgetpin == OTp) {

                            var opts1 = DB.Users.Where(a => a.Email == model.Email).SingleOrDefault();
                            opts1.forgetpin = null;
                            DB.SaveChanges();

                        }



                        return RedirectToAction("Login", "Home");
                    }

                }
            }
            else
            {
                if(OTp == "")
                {
                    TempData["newmessage"] = "Please Enter Otp";
                    return RedirectToAction("Login", "Home");
                }
                else { 
                TempData["newmessage"] = "OTP is Wrong please try again";
                return RedirectToAction("Login", "Home");
            }}
            // If we got this far, something failed, redisplay form
            return View(model);
        }

     

        [HttpPost]
        [AllowAnonymous]
        //[Route("Change_email_mobile")]
        public async Task<ActionResult> Change_email_mobile(string role, string oldemail, string newmail, string oldmobile, string newmobile, int confirmOTP)
        {
            var CHK_OTP = DB.deleteuserotps.Any(a => a.otp == confirmOTP);
            if (CHK_OTP == true)
            {
                var userid = User.Identity.GetUserId();
                if (string.IsNullOrEmpty(oldemail) && string.IsNullOrEmpty(newmail))
                {
                    /////mobile////////
                    var user1 = DB.Users.Where(aa => aa.PhoneNumber == oldmobile).SingleOrDefault();
                    if (user1 == null)
                    {
                        var msg = "Phone Number Allready Exits With Us";
                        return Json(new { msg = msg, error = "error" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var IsPhoneAlreadyRegistered = DB.Users.Any(item => item.PhoneNumber == newmobile);

                        if (IsPhoneAlreadyRegistered)
                        {
                            var msg = "Phone Number Allready Exits With Us";
                            return Json(new { msg = msg, error = "error" }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            using (DbContextTransaction dbTran = DB.Database.BeginTransaction())
                            {
                                var user = await UserManager.FindByEmailAsync(user1.Email);
                                try
                                {
                                    var rolechk = await UserManager.GetRolesAsync(user.Id);
                                    if (rolechk[0].ToString() == "Admin" || rolechk[0].ToString() == "master" || rolechk[0].ToString() == "Dealer" || rolechk[0].ToString() == "Retailer" || rolechk[0].ToString() == "API" || rolechk[0].ToString() == "Whitelabel")
                                    {
                                        var aCHKK = "";
                                        if (role == "Admin")
                                        {
                                            var chk = DB.Admin_details.Where(aa => aa.mobile == oldmobile).SingleOrDefault();
                                            if (chk == null)
                                            {
                                                aCHKK = "NOT OK";
                                            }
                                            else
                                            {
                                                chk.mobile = newmobile;
                                                aCHKK = "OK";
                                            }
                                        }
                                        else if (role == "master")
                                        {
                                            var chk = DB.Superstokist_details.Where(aa => aa.Mobile == oldmobile).SingleOrDefault();
                                            if (chk == null)
                                            {
                                                aCHKK = "NOT OK";
                                            }
                                            else
                                            {
                                                chk.Mobile = newmobile;
                                                aCHKK = "OK";
                                            }
                                        }
                                        else if (role == "Dealer")
                                        {
                                            var chk = DB.Dealer_Details.Where(aa => aa.Mobile == oldmobile).SingleOrDefault();
                                            if (chk == null)
                                            {
                                                aCHKK = "NOT OK";
                                            }
                                            else
                                            {
                                                chk.Mobile = newmobile;
                                                aCHKK = "OK";
                                            }
                                        }
                                        else if (role == "Retailer")
                                        {
                                            var chk = DB.Retailer_Details.Where(aa => aa.Mobile == oldmobile).SingleOrDefault();
                                            if (chk == null)
                                            {
                                                aCHKK = "NOT OK";
                                            }
                                            else
                                            {
                                                chk.Mobile = newmobile;
                                                aCHKK = "OK";
                                            }
                                        }
                                        else if (role == "API")
                                        {
                                            var chk = DB.api_user_details.Where(aa => aa.mobile == oldmobile).SingleOrDefault();
                                            if (chk == null)
                                            {
                                                aCHKK = "NOT OK";
                                            }
                                            else
                                            {
                                                chk.mobile = newmobile;
                                                aCHKK = "OK";
                                            }
                                        }
                                        else if (role == "Whitelabel")
                                        {
                                            var chk = DB.WhiteLabel_userList.Where(aa => aa.Mobile == oldmobile).SingleOrDefault();
                                            if (chk == null)
                                            {
                                                aCHKK = "NOT OK";
                                            }
                                            else
                                            {
                                                chk.Mobile = newmobile;
                                                aCHKK = "OK";
                                            }
                                        }
                                        if (aCHKK == "OK")
                                        {
                                            user.PhoneNumber = newmobile;
                                            var result = await UserManager.UpdateAsync(user);
                                            if (result.Succeeded)
                                            {
                                                var chk22 = DB.checklogouts.Where(a => a.userid == userid).SingleOrDefault();
                                                if (chk22 == null)
                                                {
                                                    checklogout chlogout = new checklogout();
                                                    chlogout.userid = userid;
                                                    chlogout.lastupdatedate = DateTime.UtcNow;
                                                    DB.checklogouts.Add(chlogout);
                                                    DB.SaveChanges();

                                                }
                                                else
                                                {
                                                    chk22.lastupdatedate = DateTime.UtcNow;
                                                    DB.SaveChanges();
                                                }

                                                var messageold = ""; var messagenew = "";
                                                messageold = "Dear Customer Your PhoneNumber is Change Successfully , Your New PhoneNumber is " + newmobile;
                                                messagenew = "Dear Customer Your PhoneNumber is Replace with " + oldmobile;
                                                //var txtMsgBody = "Dear Customer Your PhoneNumber " + oldmobile + " is Replace With " + newmobile + " This PhoneNumber";

                                                if (role == "Whitelabel")
                                                {
                                                    SendForgotpasswordEmail_whitelabel(user.Id, newmobile, "PhoneNumber Change", messagenew);
                                                    SendForgotpasswordEmail_whitelabel(user.Id, oldmobile, "PhoneNumber Change", messageold);
                                                }
                                                else
                                                {

                                                    SendForgotpasswordEmailadmin(user.Email, "PhoneNumber Change", messagenew);
                                                    SendForgotpasswordEmailadmin(user.Email, "PhoneNumber Change", messageold);
                                                    //var smsapichk = DB.apisms.Where(aa => aa.sts == "Y").SingleOrDefault();
                                                    //if (smsapichk != null)
                                                    //{
                                                    //    string urlss = "";
                                                    //    string msgssss = "";
                                                    //    string tempid = "";
                                                    //    try
                                                    //    {
                                                    //        var smsapionsts = DB.apisms.Where(x => x.sts == "Y").SingleOrDefault();
                                                    //        var smsstypes = DB.Sending_SMS_Templates.Where(x => x.SMS_TYPE == "CHANGE_MOBILE_NUMBER" && x.SMSAPIID == smsapionsts.id).SingleOrDefault();
                                                    //        if (smsstypes != null)
                                                    //        {

                                                    //            msgssss = string.Format(smsstypes.Templates, oldmobile, newmobile);
                                                    //            tempid = smsstypes.Templateid;
                                                    //            urlss = smsapionsts.smsapi;

                                                    //            // smssend.sendsmsallnew(user.PhoneNumber, msgssss, urlss, tempid);

                                                    //            var apinamechange = urlss.Replace("tttt", user.PhoneNumber).Replace("mmmm", msgssss).Replace("iiii", tempid);
                                                    //            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                                                    //            HttpWebRequest WebRequestObject = (HttpWebRequest)HttpWebRequest.Create(apinamechange);
                                                    //            WebRequestObject.Timeout = (System.Int32)TimeSpan.FromSeconds(135).TotalMilliseconds;

                                                    //            WebResponse Response = WebRequestObject.GetResponse();
                                                    //            Stream WebStream = Response.GetResponseStream();
                                                    //            StreamReader Reader = new StreamReader(WebStream);
                                                    //            var s = Reader.ReadToEnd();
                                                    //            sms_api_entry sms = new sms_api_entry();
                                                    //            sms.apiname = apinamechange;
                                                    //            sms.msg = msgssss;
                                                    //            sms.messagefor = user.Id;
                                                    //            sms.m_date = System.DateTime.Now;
                                                    //            sms.response = s;
                                                    //            DB.sms_api_entry.Add(sms);
                                                    //            DB.SaveChanges();
                                                    //        }
                                                    //    }
                                                    //    catch (Exception) { }
                                                    //}

                                                    smssend.sms_init("Y", "Y", "CHANGE_MOBILE_NUMBER", user.PhoneNumber, oldmobile, newmobile);

                                                }

                                                Change_mobile_email ch = new Change_mobile_email();
                                                ch.newmobile = newmobile;
                                                ch.oldmobile = oldmobile;
                                                ch.role = role;
                                                ch.updatedate = DateTime.Now;
                                                ch.userid = user.Id;
                                                DB.Change_mobile_email.Add(ch);
                                                DB.SaveChanges();
                                                dbTran.Commit();
                                                var msg = "Mobile Change Succesfully";
                                                return Json(new { msg = msg, error = "success" }, JsonRequestBehavior.AllowGet);
                                                //return Ok();
                                            }
                                        }
                                        else
                                        {
                                            var msg = "User Not Exist With Us.";
                                            return Json(new { msg = msg, error = "error" }, JsonRequestBehavior.AllowGet);
                                        }
                                    }
                                    else if (rolechk[0].ToString() == "Whitelabelmaster" || rolechk[0].ToString() == "Whitelabeldealer" || rolechk[0].ToString() == "Whitelabelretailer")
                                    {
                                        var wCHKK = "";
                                        if (role == "Whitelabelmaster")
                                        {
                                            var chk = DB.Whitelabel_Superstokist_details.Where(aa => aa.Mobile == oldmobile && aa.Whitelabelid == userid).SingleOrDefault();
                                            if (chk == null)
                                            {
                                                wCHKK = "NOT OK";
                                            }
                                            else
                                            {
                                                chk.Mobile = newmobile;
                                                wCHKK = "OK";
                                            }
                                        }
                                        else if (role == "Whitelabeldealer")
                                        {
                                            var chk = DB.whitelabel_Dealer_Details.Where(aa => aa.Mobile == oldmobile && aa.Whitelabelid == userid).SingleOrDefault();
                                            if (chk == null)
                                            {
                                                wCHKK = "NOT OK";
                                            }
                                            else
                                            {
                                                chk.Mobile = newmobile;
                                                wCHKK = "OK";
                                            }
                                        }
                                        else if (role == "Whitelabelretailer")
                                        {
                                            var chk = DB.Whitelabel_Retailer_Details.Where(aa => aa.Mobile == oldmobile && aa.Whitelabelid == userid).SingleOrDefault();
                                            if (chk == null)
                                            {
                                                wCHKK = "NOT OK";
                                            }
                                            else
                                            {
                                                chk.Mobile = newmobile;
                                                wCHKK = "OK";
                                            }
                                        }
                                        if (wCHKK == "OK")
                                        {
                                            user.PhoneNumber = newmobile;
                                            var result = await UserManager.UpdateAsync(user);
                                            if (result.Succeeded)
                                            {
                                                var chk22 = DB.checklogouts.Where(a => a.userid == userid).SingleOrDefault();
                                                if (chk22 == null)
                                                {
                                                    checklogout chlogout = new checklogout();
                                                    chlogout.userid = userid;
                                                    chlogout.lastupdatedate = DateTime.UtcNow;
                                                    DB.checklogouts.Add(chlogout);
                                                    DB.SaveChanges();

                                                }
                                                else
                                                {
                                                    chk22.lastupdatedate = DateTime.UtcNow;
                                                    DB.SaveChanges();
                                                }

                                                var messageold = ""; var messagenew = "";
                                                messageold = "Dear Customer Your PhoneNumber is Change Successfully , Your New PhoneNumber is " + newmobile;
                                                messagenew = "Dear Customer Your PhoneNumber is Replace with " + oldmobile;
                                                var txtMsgBody = "Dear Customer Your PhoneNumber " + oldmobile + " is Replace With " + newmobile + " This PhoneNumber";

                                                try
                                                {
                                                    SendForgotpasswordEmail_whitelabel(userid, newmobile, "PhoneNumber Change", messagenew);
                                                    SendForgotpasswordEmail_whitelabel(userid, oldmobile, "PhoneNumber Change", messageold);
                                                }
                                                catch { }

                                                var smsapichk = DB.whitelabel_apisms.Where(aa => aa.sts == "Y" && aa.userfor == userid).SingleOrDefault();
                                                if (smsapichk != null)
                                                {



                                                    string urlss = "";
                                                    string msgssss = "";
                                                    string tempid = "";
                                                    try
                                                    {




                                                        var smsapionsts = DB.whitelabel_apisms.Where(x => x.sts == "Y" && x.userfor == userid).SingleOrDefault();
                                                        var smsstypes = DB.Whitelabel_Sending_SMS_Templates.Where(x => x.SMS_TYPE == "CHANGE_MOBILE_NUMBER" && x.SMSAPIID == smsapionsts.id && x.whitelabelId == userid).SingleOrDefault();
                                                        if (smsstypes != null)
                                                        {
                                                            msgssss = string.Format(smsstypes.Templates, oldmobile, newmobile);
                                                            tempid = smsstypes.Templateid;
                                                            urlss = smsapionsts.smsapi;

                                                            // smssend.sendsmsallnew(user.PhoneNumber, msgssss, urlss, tempid);

                                                            var apinamechange = urlss.Replace("tttt", user.PhoneNumber).Replace("mmmm", msgssss).Replace("iiii", tempid);
                                                            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                                                            HttpWebRequest WebRequestObject = (HttpWebRequest)HttpWebRequest.Create(apinamechange);
                                                            WebRequestObject.Timeout = (System.Int32)TimeSpan.FromSeconds(10).TotalMilliseconds;

                                                            WebResponse Response = WebRequestObject.GetResponse();
                                                            Stream WebStream = Response.GetResponseStream();
                                                            StreamReader Reader = new StreamReader(WebStream);
                                                            var s = Reader.ReadToEnd();
                                                            whitelabel_sms_api_entry sms = new whitelabel_sms_api_entry();
                                                            sms.apiname = apinamechange;
                                                            sms.msg = msgssss;
                                                            sms.messagefor = user.Id;
                                                            sms.userid = userid;
                                                            sms.m_date = System.DateTime.Now;
                                                            sms.response = s;
                                                            DB.whitelabel_sms_api_entry.Add(sms);
                                                            DB.SaveChanges();
                                                        }
                                                    }
                                                    catch (Exception) { }

                                                }


                                                Whitelabel_Change_mobile_email ch = new Whitelabel_Change_mobile_email();
                                                ch.newmobile = newmobile;
                                                ch.oldmobile = oldmobile;
                                                ch.role = role;
                                                ch.updatedate = DateTime.Now;
                                                ch.userid = user.Id;
                                                DB.Whitelabel_Change_mobile_email.Add(ch);
                                                DB.SaveChanges();
                                                dbTran.Commit();
                                                var msg = "Mobile Change Succesfully";
                                                return Json(new { msg = msg, error = "success" }, JsonRequestBehavior.AllowGet);
                                                //return Ok();
                                            }
                                        }
                                        else
                                        {
                                            var msg = "User Not Exist With Us.";
                                            return Json(new { msg = msg, error = "error" }, JsonRequestBehavior.AllowGet);
                                        }
                                    }
                                    else
                                    {
                                        var msg = "Phone Number Allready Exits With Us";
                                        return Json(new { msg = msg, error = "error" }, JsonRequestBehavior.AllowGet);
                                    }

                                }
                                catch
                                {
                                    dbTran.Rollback();
                                    throw;
                                }
                            }
                        }
                    }
                }
                else
                {
                    /////email change///////////
                    var user = await UserManager.FindByEmailAsync(oldemail);
                    if (user == null)
                    {
                        var msg = "Email Not Exits With Us";
                        return Json(new { msg = msg, error = "error" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        using (DbContextTransaction dbTran = DB.Database.BeginTransaction())
                        {
                            var messageold = ""; var messagenew = "";
                            try
                            {
                                var rolechk = await UserManager.GetRolesAsync(user.Id);
                                if (rolechk[0].ToString() == "Admin" || rolechk[0].ToString() == "master" || rolechk[0].ToString() == "Dealer" || rolechk[0].ToString() == "Retailer" || rolechk[0].ToString() == "API" || rolechk[0].ToString() == "Whitelabel")
                                {
                                    var aCHKK = "";
                                    if (role == "Admin")
                                    {
                                        var chk = DB.Admin_details.Where(aa => aa.email == oldemail).SingleOrDefault();
                                        if (chk == null)
                                        {
                                            aCHKK = "NOT OK";
                                        }
                                        else
                                        {
                                            chk.email = newmail;
                                            aCHKK = "OK";
                                        }
                                    }
                                    else if (role == "master")
                                    {
                                        var chk = DB.Superstokist_details.Where(aa => aa.Email == oldemail).SingleOrDefault();
                                        if (chk == null)
                                        {
                                            aCHKK = "NOT OK";
                                        }
                                        else
                                        {
                                            chk.Email = newmail;
                                            aCHKK = "OK";
                                        }
                                    }
                                    else if (role == "Dealer")
                                    {
                                        var chk = DB.Dealer_Details.Where(aa => aa.Email == oldemail).SingleOrDefault();
                                        if (chk == null)
                                        {
                                            aCHKK = "NOT OK";
                                        }
                                        else
                                        {
                                            chk.Email = newmail;
                                            aCHKK = "OK";
                                        }
                                    }
                                    else if (role == "Retailer")
                                    {
                                        var chk = DB.Retailer_Details.Where(aa => aa.Email == oldemail).SingleOrDefault();
                                        if (chk == null)
                                        {
                                            aCHKK = "NOT OK";
                                        }
                                        else
                                        {
                                            chk.Email = newmail;
                                            aCHKK = "OK";
                                        }
                                    }
                                    else if (role == "API")
                                    {
                                        var chk = DB.api_user_details.Where(aa => aa.emailid == oldemail).SingleOrDefault();
                                        if (chk == null)
                                        {
                                            aCHKK = "NOT OK";
                                        }
                                        else
                                        {
                                            chk.emailid = newmail;
                                            aCHKK = "OK";
                                        }
                                    }
                                    else if (role == "Whitelabel")
                                    {
                                        var chk = DB.WhiteLabel_userList.Where(aa => aa.EmailId == oldemail).SingleOrDefault();
                                        if (chk == null)
                                        {
                                            aCHKK = "NOT OK";
                                        }
                                        else
                                        {
                                            chk.EmailId = newmail;
                                            aCHKK = "OK";
                                        }
                                    }
                                    if (aCHKK == "OK")
                                    {
                                        user.Email = newmail;
                                        user.UserName = newmail;
                                        user.EmailConfirmed = true;
                                        var result = await UserManager.UpdateAsync(user);
                                        if (result.Succeeded)
                                        {
                                            var chk22 = DB.checklogouts.Where(a => a.userid == userid).SingleOrDefault();
                                            if (chk22 == null)
                                            {
                                                checklogout chlogout = new checklogout();
                                                chlogout.userid = userid;
                                                chlogout.lastupdatedate = DateTime.UtcNow;
                                                DB.checklogouts.Add(chlogout);
                                                DB.SaveChanges();

                                            }
                                            else
                                            {
                                                chk22.lastupdatedate = DateTime.UtcNow;
                                                DB.SaveChanges();
                                            }

                                            messageold = "Dear Customer Your Email is Change Successfully , Your New Email is " + newmail;
                                            messagenew = "Dear Customer Your Email is Replace with " + oldemail;
                                            var txtMsgBody = "Dear Customer Your Email " + oldemail + " is Replace With " + newmail + " This Email";

                                            if (role == "Vendor")
                                            {
                                                var chk = DB.Vendor_details.Where(aa => aa.emailid == oldemail).SingleOrDefault();
                                                chk.emailid = newmail;
                                            }

                                            else if (role == "RCH")
                                            {
                                                var chk = DB.RCH_Details.Where(aa => aa.Email == oldemail).SingleOrDefault();
                                                chk.Email = newmail;
                                            }

                                            else if (role == "FeeCollector")
                                            {
                                                var chk = DB.FeeCollector_details.Where(aa => aa.Email == oldemail).SingleOrDefault();
                                                chk.Email = newmail;
                                            }

                                            if (role == "Whitelabel")
                                            {
                                                SendForgotpasswordEmail_whitelabel(user.Id, newmail, "Mail Change", messagenew);
                                                SendForgotpasswordEmail_whitelabel(user.Id, oldemail, "Mail Change", messageold);
                                            }
                                            else
                                            {
                                                SendForgotpasswordEmailadmin(newmail, "Mail Change", messagenew);
                                                SendForgotpasswordEmailadmin(oldemail, "Mail Change", messageold);
                                                //var smsapichk = DB.apisms.Where(aa => aa.sts == "Y").SingleOrDefault();
                                                //if (smsapichk != null)
                                                //{

                                                //    string urlss = "";
                                                //    string msgssss = "";
                                                //    string tempid = "";
                                                //    try
                                                //    {
                                                //        var smsapionsts = DB.apisms.Where(x => x.sts == "Y").SingleOrDefault();
                                                //        var smsstypes = DB.Sending_SMS_Templates.Where(x => x.SMS_TYPE == "CHANGE_EMAILID" && x.SMSAPIID == smsapionsts.id).SingleOrDefault();
                                                //        if (smsstypes != null)
                                                //        {

                                                //            msgssss = string.Format(smsstypes.Templates, oldemail, newmail);
                                                //            tempid = smsstypes.Templateid;
                                                //            urlss = smsapionsts.smsapi;

                                                //            // smssend.sendsmsallnew(user.PhoneNumber, msgssss, urlss, tempid);

                                                //            var apinamechange = urlss.Replace("tttt", user.PhoneNumber).Replace("mmmm", msgssss).Replace("iiii", tempid);
                                                //            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                                                //            HttpWebRequest WebRequestObject = (HttpWebRequest)HttpWebRequest.Create(apinamechange);
                                                //            WebRequestObject.Timeout = (System.Int32)TimeSpan.FromSeconds(135).TotalMilliseconds;

                                                //            WebResponse Response = WebRequestObject.GetResponse();
                                                //            Stream WebStream = Response.GetResponseStream();
                                                //            StreamReader Reader = new StreamReader(WebStream);
                                                //            var s = Reader.ReadToEnd();
                                                //            sms_api_entry sms = new sms_api_entry();
                                                //            sms.apiname = apinamechange;
                                                //            sms.msg = msgssss;
                                                //            sms.messagefor = user.Id;
                                                //            sms.m_date = System.DateTime.Now;
                                                //            sms.response = s;
                                                //            DB.sms_api_entry.Add(sms);
                                                //            DB.SaveChanges();
                                                //        }
                                                //    }
                                                //    catch (Exception) { }
                                                //}

                                                smssend.sms_init("Y", "Y", "CHANGE_EMAILID", user.PhoneNumber, oldemail, newmail);

                                            }
                                            Change_mobile_email ch = new Change_mobile_email();
                                            ch.newemail = newmail;
                                            ch.oldemail = oldemail;
                                            ch.role = role;
                                            ch.updatedate = DateTime.Now;
                                            ch.userid = user.Id;
                                            DB.Change_mobile_email.Add(ch);
                                            DB.SaveChanges();
                                            dbTran.Commit();

                                            //return Ok();
                                            var msg = "Email Change Succesfully";
                                            return Json(new { msg = msg, error = "success" }, JsonRequestBehavior.AllowGet);
                                        }
                                        else
                                        {
                                            var msg = ((string[])result.Errors)[1];
                                            return Json(new { msg = msg, error = "error" }, JsonRequestBehavior.AllowGet);
                                        }
                                    }
                                    else
                                    {
                                        var msg = "User Not Exist With Us.";
                                        return Json(new { msg = msg, error = "error" }, JsonRequestBehavior.AllowGet);
                                    }
                                }
                                else if (rolechk[0].ToString() == "Whitelabelmaster" || rolechk[0].ToString() == "Whitelabeldealer" || rolechk[0].ToString() == "Whitelabelretailer")
                                {
                                    var wCHKK = "";
                                    if (role == "Whitelabelmaster")
                                    {
                                        var chk = DB.Whitelabel_Superstokist_details.Where(aa => aa.Email == oldemail && aa.Whitelabelid == userid).SingleOrDefault();
                                        if (chk == null)
                                        {
                                            wCHKK = "NOT OK";
                                        }
                                        else
                                        {
                                            chk.Email = newmail;
                                            wCHKK = "OK";
                                        }
                                    }
                                    else if (role == "Whitelabeldealer")
                                    {
                                        var chk = DB.whitelabel_Dealer_Details.Where(aa => aa.Email == oldemail && aa.Whitelabelid == userid).SingleOrDefault();
                                        if (chk == null)
                                        {
                                            wCHKK = "NOT OK";
                                        }
                                        else
                                        {
                                            chk.Email = newmail;
                                            wCHKK = "OK";
                                        }
                                    }
                                    else if (role == "Whitelabelretailer")
                                    {
                                        var chk = DB.Whitelabel_Retailer_Details.Where(aa => aa.Email == oldemail && aa.Whitelabelid == userid).SingleOrDefault();
                                        if (chk == null)
                                        {
                                            wCHKK = "NOT OK";
                                        }
                                        else
                                        {
                                            chk.Email = newmail;
                                            wCHKK = "OK";
                                        }
                                    }
                                    if (wCHKK == "OK")
                                    {
                                        user.Email = newmail;
                                        user.UserName = newmail;
                                        user.EmailConfirmed = true;
                                        var result = await UserManager.UpdateAsync(user);
                                        if (result.Succeeded)
                                        {
                                            var chk22 = DB.checklogouts.Where(a => a.userid == userid).SingleOrDefault();
                                            if (chk22 == null)
                                            {
                                                checklogout chlogout = new checklogout();
                                                chlogout.userid = userid;
                                                chlogout.lastupdatedate = DateTime.UtcNow;
                                                DB.checklogouts.Add(chlogout);
                                                DB.SaveChanges();

                                            }
                                            else
                                            {
                                                chk22.lastupdatedate = DateTime.UtcNow;
                                                DB.SaveChanges();
                                            }

                                            messageold = "Dear Customer Your Email is Change Successfully , Your New Email is " + newmail;
                                            messagenew = "Dear Customer Your Email is Replace with " + oldemail;
                                            var txtMsgBody = "Dear Customer Your Email " + oldemail + " is Replace With " + newmail + " This Email";
                                            try
                                            {
                                                SendForgotpasswordEmail_whitelabel(userid, newmail, "Mail Change", messagenew);
                                                SendForgotpasswordEmail_whitelabel(userid, oldemail, "Mail Change", messageold);
                                            }
                                            catch { }

                                            var smsapichk = DB.whitelabel_apisms.Where(aa => aa.sts == "Y" && aa.userfor == userid).SingleOrDefault();
                                            if (smsapichk != null)
                                            {

                                                string urlss = "";
                                                string msgssss = "";
                                                string tempid = "";
                                                try
                                                {
                                                    var smsapionsts = DB.apisms.Where(x => x.sts == "Y").SingleOrDefault();
                                                    var smsstypes = DB.Sending_SMS_Templates.Where(x => x.SMS_TYPE == "CHANGE_EMAILID" && x.SMSAPIID == smsapionsts.id).SingleOrDefault();
                                                    if (smsstypes != null)
                                                    {

                                                        msgssss = string.Format(smsstypes.Templates, oldemail, newmail);
                                                        tempid = smsstypes.Templateid;
                                                        urlss = smsapionsts.smsapi;

                                                        // smssend.sendsmsallnew(user.PhoneNumber, msgssss, urlss, tempid);

                                                        var apinamechange = urlss.Replace("tttt", user.PhoneNumber).Replace("mmmm", msgssss).Replace("iiii", tempid);
                                                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                                                        HttpWebRequest WebRequestObject = (HttpWebRequest)HttpWebRequest.Create(apinamechange);
                                                        WebRequestObject.Timeout = (System.Int32)TimeSpan.FromSeconds(10).TotalMilliseconds;

                                                        WebResponse Response = WebRequestObject.GetResponse();
                                                        Stream WebStream = Response.GetResponseStream();
                                                        StreamReader Reader = new StreamReader(WebStream);
                                                        string webcontent = Reader.ReadToEnd();
                                                        whitelabel_sms_api_entry sms = new whitelabel_sms_api_entry();
                                                        sms.apiname = apinamechange;
                                                        sms.msg = msgssss;
                                                        sms.messagefor = userid;
                                                        sms.userid = userid;
                                                        sms.m_date = System.DateTime.Now;
                                                        sms.response = webcontent;
                                                        DB.whitelabel_sms_api_entry.Add(sms);
                                                        DB.SaveChanges();
                                                    }
                                                }
                                                catch (Exception) { }
                                            }

                                            Whitelabel_Change_mobile_email ch = new Whitelabel_Change_mobile_email();
                                            ch.newemail = newmail;
                                            ch.oldemail = oldemail;
                                            ch.role = role;
                                            ch.updatedate = DateTime.Now;
                                            ch.userid = user.Id;
                                            DB.Whitelabel_Change_mobile_email.Add(ch);
                                            DB.SaveChanges();
                                            dbTran.Commit();

                                            //return Ok();
                                            var msg = "Email Change Succesfully";
                                            return Json(new { msg = msg, error = "success" }, JsonRequestBehavior.AllowGet);
                                        }
                                        else
                                        {
                                            var msg = ((string[])result.Errors)[1];
                                            return Json(new { msg = msg, error = "error" }, JsonRequestBehavior.AllowGet);
                                        }
                                    }
                                    else
                                    {
                                        var msg = "User Not Exist With Us.";
                                        return Json(new { msg = msg, error = "error" }, JsonRequestBehavior.AllowGet);
                                    }
                                }
                                else
                                {
                                    var msg = "Phone Number Allready Exits With Us";
                                    return Json(new { msg = msg, error = "error" }, JsonRequestBehavior.AllowGet);
                                }
                            }
                            catch
                            {
                                dbTran.Rollback();
                                throw;
                            }
                        }
                    }
                }
            }
            else
            {
                var msg = "OTP is incorrect or Expire";
                return Json(new { msg = msg, error = "error" }, JsonRequestBehavior.AllowGet);
            }
            return RedirectToAction("WebLogin", "Home", new { Areas = "Admin" });
        }
        private void SendForgotpasswordEmailadmin(string recepientEmail, string subject, string body)
        {
            var ToCC = DB.Admin_details.FirstOrDefault().email;
            CommUtilEmail emailsend = new CommUtilEmail();
            emailsend.EmailLimitChk(recepientEmail, ToCC, subject, body, "No CallBackUrl");
        }
        private string ForgotBody(string userName, string title, string url, string description)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(Server.MapPath("~/Email-Tamplete/forgotpassword.html")))
            {
                body = reader.ReadToEnd();
            }
            VastwebmultiEntities db = new VastwebmultiEntities();
            //var logocon = db.tblOutsideLogoes.Where(p => p.Role == "WHITELABEL").ToList();
            var Admin_details = db.Admin_details.SingleOrDefault();
            var logoPath = db.tblHeaderLogoes.ToList();
            var logo = logoPath.Count == 0 ? "" : logoPath.Where(p => p.Role == "ADMIN").SingleOrDefault().LogoImage;


            int state = Convert.ToInt32(Admin_details.State);
            int district = Convert.ToInt32(Admin_details.District);
            var City = db.State_Desc.Where(a => a.State_id == state).SingleOrDefault().State_name;
            var State = db.District_Desc.Where(a => a.Dist_id == district && a.State_id == state).SingleOrDefault().Dist_Desc;

            var userid = db.Users.Where(a => a.Email == userName).SingleOrDefault();
            var userroleid = DB.UserRoles.Where(aa => aa.UserId == userid.UserId).SingleOrDefault().RoleId;
            var userrole = DB.Roles.Where(aa => aa.RoleId == userroleid).SingleOrDefault().Name;

            var User_Name = ""; var User_City = ""; var User_State = "";
            if (userrole == "Admin")
            {
                User_Name = Admin_details.Name;
                User_City = City;
                User_State = State;
            }
            else if (userrole == "master")
            {
                var Master_details = db.Superstokist_details.Where(a => a.SSId == userid.UserId).SingleOrDefault();
                int User_state = Convert.ToInt32(Master_details.State);
                int User_district = Convert.ToInt32(Master_details.District);

                User_Name = Master_details.SuperstokistName;
                User_City = db.State_Desc.Where(a => a.State_id == User_state).SingleOrDefault().State_name; ;
                User_State = db.District_Desc.Where(a => a.Dist_id == User_district && a.State_id == User_state).SingleOrDefault().Dist_Desc;
            }
            else if (userrole == "Dealer")
            {
                var Dealer_details = db.Dealer_Details.Where(a => a.DealerId == userid.UserId).SingleOrDefault();
                int User_state = Convert.ToInt32(Dealer_details.State);
                int User_district = Convert.ToInt32(Dealer_details.District);

                User_Name = Dealer_details.DealerName;
                User_City = db.State_Desc.Where(a => a.State_id == User_state).SingleOrDefault().State_name; ;
                User_State = db.District_Desc.Where(a => a.Dist_id == User_district && a.State_id == User_state).SingleOrDefault().Dist_Desc;
            }
            else if (userrole == "Retailer")
            {
                var Retailer_details = db.Retailer_Details.Where(a => a.RetailerId == userid.UserId).SingleOrDefault();
                int User_state = Convert.ToInt32(Retailer_details.State);
                int User_district = Convert.ToInt32(Retailer_details.District);

                User_Name = Retailer_details.RetailerName;
                User_City = db.State_Desc.Where(a => a.State_id == User_state).SingleOrDefault().State_name; ;
                User_State = db.District_Desc.Where(a => a.Dist_id == User_district && a.State_id == User_state).SingleOrDefault().Dist_Desc;
            }
            else if (userrole == "API")
            {
                var Api_details = db.api_user_details.Where(a => a.apiid == userid.UserId).SingleOrDefault();
                int User_state = Convert.ToInt32(Api_details.state);
                int User_district = Convert.ToInt32(Api_details.district);

                User_Name = Api_details.username;
                User_City = db.State_Desc.Where(a => a.State_id == User_state).SingleOrDefault().State_name; ;
                User_State = db.District_Desc.Where(a => a.Dist_id == User_district && a.State_id == User_state).SingleOrDefault().Dist_Desc;
            }


            body = body.Replace("{websiteurl}", Admin_details.WebsiteUrl);
            body = body.Replace("{adminmailid}", Admin_details.email1);
            body = body.Replace("{useremail}", userName);
            body = body.Replace("{mobile}", Admin_details.mobile1);
            body = body.Replace("{companyname}", Admin_details.Companyname);
            body = body.Replace("{city}", City);
            body = body.Replace("{State}", State);
            body = body.Replace("{UserName}", User_Name);
            body = body.Replace("{User_City}", User_City);
            body = body.Replace("{User_State}", User_State);

            String strPathAndQuery = System.Web.HttpContext.Current.Request.Url.PathAndQuery;
            String strUrl = System.Web.HttpContext.Current.Request.Url.AbsoluteUri.Replace(strPathAndQuery, "/");

            var baseurl = strUrl + logo;
            body = body.Replace("{logo}", baseurl);

            body = body.Replace("{ResetImage}", strUrl);
            body = body.Replace("{message}", description);

            return body;
        }
        private void SendForgotpasswordEmail(string recepientEmail, string subject, string body, string callbackUrl)
        {
            var ToCC = DB.Admin_details.FirstOrDefault().email;
            CommUtilEmail emailsend = new CommUtilEmail();
            emailsend.EmailLimitChk(recepientEmail, ToCC, subject, body, callbackUrl);
        }
        private void SendForgotpasswordEmail_whitelabel(string whitelabelid, string recepientEmail, string subject, string body)
        {
            var ToCC = DB.Admin_details.FirstOrDefault().email;
            CommUtilEmail emailsend = new CommUtilEmail();
            emailsend.WhiteLabelEmailLimitChk(recepientEmail, ToCC, subject, body, "", whitelabelid);
        }
        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmationAdmin()
        {
            return View();
        }
        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }



            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                //Remove 
                DB.delete_frogotuser(model.Email);
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        [AllowAnonymous]
        public ActionResult ResetPasswordAdmin(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPasswordAdmin(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmationAdmin", "Account");
            }
            //var resettoken1 = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                //Remove 
                DB.delete_frogotuser(model.Email);
                return RedirectToAction("ResetPasswordConfirmationAdmin", "Account");

            }
            AddErrors(result);


            return View();
        }


        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmationAdmin()
        {
            return View();
        }


        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> LogOff()
        {
            var userid = User.Identity.GetUserId();
            TempData.Remove("data");
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            //await UserManager.UpdateSecurityStampAsync(userid);
            return RedirectToAction("Index", "Home", null);
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }




        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
                if (DB != null)
                {
                    DB.Dispose();
                    DB = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }



        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion

    }
}