using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;
using System.Windows.Interop;
using Vastwebmulti.Areas.ADMIN.Controllers;
using Vastwebmulti.Areas.ADMIN.Models;
using Vastwebmulti.Areas.WHITELABEL.Models;
using Vastwebmulti.Models;

namespace Vastwebmulti.Controllers
{
    /// <summary>
    /// Application ka main controller - home page, login, registration aur sabhi public pages handle karta hai
    /// </summary>
    public class HomeController : Controller
    {

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

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

        ApplicationDbContext context = new ApplicationDbContext();
        VastwebmultiEntities DB = new VastwebmultiEntities();
        ALLSMSSend smssend = new ALLSMSSend();
        /// <summary>
        /// Admin registration ka form dikhata hai, agar admin already register hai to home page par redirect karta hai
        /// </summary>
        [AllowAnonymous]
        public ActionResult Registration()
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                var checkOne = db.Admin_details.ToList();
                if (checkOne.Any())
                {
                    return RedirectToAction("Index1", "Home");
                }
                else
                {
                    ViewData["state"] = DB.State_Desc.Select(a => new SelectListItem { Text = a.State_name, Value = a.State_id.ToString() }).ToList();
                    ViewBag.District = DB.District_Desc.Select(a => new SelectListItem { Text = a.Dist_Desc, Value = a.Dist_id.ToString() }).ToList();

                    string currenturl = HttpContext.Request.Url.Authority;
                    currenturl = currenturl.Replace("www.", "").Replace("https://", "").Replace("http://", "");
                    ViewBag.DomainName = currenturl;
                    return View();
                }
            }
        }
        /// <summary>
        /// User ka purana email change karke naya email set karta hai
        /// </summary>
        public async Task<ActionResult> changeemail(string oldemail, string newmail)
        {
            var user = await UserManager.FindByEmailAsync(oldemail);
            user.Email = newmail;
            user.UserName = newmail;
            user.EmailConfirmed = true;
            var result = await UserManager.UpdateAsync(user);
            return View();
        }
        /// <summary>
        /// Admin registration form submit hone par naya admin account create karta hai
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Registration(RegisterViewModel model)
        {
            ViewData["state"] = DB.State_Desc.Select(a => new SelectListItem { Text = a.State_name, Value = a.State_id.ToString() }).ToList();
            ViewBag.District = DB.District_Desc.Select(a => new SelectListItem { Text = a.Dist_Desc, Value = a.Dist_id.ToString() }).ToList();
            try
            {
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    var checkOne = db.Admin_details.ToList();
                    if (checkOne.Any())
                    {
                        TempData["aa"] = "Admin Can only One Time Register !!!";
                        return RedirectToAction("Index1", "Home");

                    }
                    else
                    {
                        var check = db.Admin_details.Where(es => es.mobile == model.Mobile).Any();
                        if (check == false)
                        {
                            var user = new ApplicationUser { UserName = model.Email, Email = model.Email, PhoneNumber = model.Mobile };
                            //Generate Random Password

                            var Password = model.Password;
                            var result = await UserManager.CreateAsync(user, Password);
                            if (result.Succeeded)
                            {
                                System.Data.Entity.Core.Objects.ObjectParameter output = new
                                System.Data.Entity.Core.Objects.ObjectParameter("Output", typeof(string));

                                string DomainName = HttpContext.Request.Url.Authority;
                                DomainName = DomainName.Replace("www.", "").Replace("https://", "").Replace("http://", "");

                                var ch = db.Insert_Admin(user.Id, model.Name, Convert.ToInt32(model.state), Convert.ToInt32(model.distict), model.Mobile, model.Email, model.SupportMail, model.Pin, model.Address, model.customercareno, DomainName, output).SingleOrDefault().msg;
                                Remain_Admin_balance balance = new Remain_Admin_balance();
                                balance.RemainAmount = Convert.ToDecimal(model.Value);
                                balance.VirtualAmount = Convert.ToDecimal(model.Value);
                                balance.admin = user.Id;
                                balance.date = DateTime.Now;
                                db.Remain_Admin_balance.Add(balance);
                                db.SaveChanges();
                                string Firmname = "";
                                if (!string.IsNullOrWhiteSpace(model.Companyname))
                                {
                                    Firmname = model.Companyname;
                                }
                                else
                                {
                                    Firmname = model.Name;
                                }
                                //update company name 
                                Admin_details ad = (from pp in db.Admin_details
                                                    where pp.userid == user.Id
                                                    select pp).FirstOrDefault();
                                ad.Companyname = Firmname;

                                var _user = db.Users.FirstOrDefault();
                                if (_user != null)
                                {
                                    _user.EmailConfirmed = true;
                                    _user.PhoneNumberConfirmed = true;
                                }
                                db.SaveChanges();

                                // Send an email with this link
                                string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                                var callbackUrl = Url.Action("ConfirmEmailAdmin", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                                callbackUrl = callbackUrl.Replace("/WHITELABEL", "");
                                string body = new CommonUtil().PopulateBodyDealer("", "Confirm your account", "", "" + callbackUrl + "", model.Email, Password);
                                new CommonUtil().Insertsendmail(model.Email, "Confirm your account", body, callbackUrl);
                                ResendConfirmMail resend = new ResendConfirmMail();
                                resend.CallBackUrl = callbackUrl;
                                resend.Email = model.Email;
                                resend.Password = Password;
                                resend.Pin = "";
                                DB.ResendConfirmMails.Add(resend);
                                DB.SaveChanges();

                                TempData["aa"] = ch + "Congratulation ! Your Admin Registration Successful Complate. Please Check Your Mail And Confirm Your Account..";

                                return RedirectToAction("Index1", "Home");
                            }
                            else
                            {
                                var v = result.Errors.ToArray();
                                var ff = v[0].ToString();
                                TempData["aaa"] = ff;
                                return RedirectToAction("Registration");
                            }
                        }
                        else
                        {
                            TempData["aa"] = "This Mobile Number Already Exists.";
                            return RedirectToAction("Registration");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Registration");
            }
        }
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                TempData["RegisterMessage"] = "Password must be, at least one uppercase letter, one lowercase letter one number and one special character:";
            }
        }

        /// <summary>
        /// Home page dikhata hai aur logged-in user ko uske role ke hisaab se dashboard par redirect karta hai
        /// </summary>
        public ActionResult Index1()
        {


            var userid = User.Identity.GetUserId();
            var x = DB.Admin_details.SingleOrDefault().RenivalDate >= DateTime.Now;
            if (x)
            {
                try
                {
                    var currentwebsitess = DB.Admin_details.SingleOrDefault().WebsiteUrl;
                    var pthss = Path.Combine(Server.MapPath("~/AdminApk/" + currentwebsitess + "/Vastwebmulti.txt"));

                    try
                    {

                        if (!System.IO.File.Exists(pthss))
                        {
                            // Create a file to write to.
                            ViewBag.urllink = "No App Url";
                        }
                        else
                        {
                            var fs = new FileStream(pthss, FileMode.Open, FileAccess.Read);
                            var sr = new StreamReader(fs, Encoding.UTF8);

                            string contentaa = sr.ReadToEnd();
                            ViewBag.urllink = contentaa;
                        }


                        string[] filesInDirectory = Directory.GetFiles(Server.MapPath("~/AdminApk/" + currentwebsitess), "*.apk");
                        if (filesInDirectory.Length == 0)
                        {
                            ViewBag.urllink = null;
                        }
                        else
                        {

                            ViewBag.urllink = Url.Action("DownloadAPK", "", new { }, protocol: Request.Url.Scheme);
                        }
                    }
                    catch
                    {
                        ViewBag.urllink = null;
                    }



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
                            else
                            {
                                TempData.Remove("data");
                                AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                                TempData["userblocked"] = "YOUR MASTER ID IS BLOCKED. PLEASE CONTACT TO ADMIN";
                                return RedirectToAction("Login", "Home", null);
                            }
                        }
                        else if (User.IsInRole("Dealer"))
                        {
                            var stschk = db.Dealer_Details.Where(aa => aa.DealerId == userid).SingleOrDefault().Status;
                            if (stschk == "Y")
                            {
                                return RedirectToAction("Dashboard", "Home", new { area = "Dealer" });
                            }
                            else
                            {
                                TempData.Remove("data");
                                AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                                TempData["userblocked"] = "Your Account is Currently Blocked with Distributor, Contact to Administrator.";
                                return RedirectToAction("Login", "Home", null);
                            }

                        }
                        else if (User.IsInRole("Retailer"))
                        {
                            var RetailerDetails = db.Retailer_Details.Where(aa => aa.RetailerId == userid).SingleOrDefault();

                            var stschk = RetailerDetails.Status;
                            if (stschk == "Y")
                            {
                                return RedirectToAction("Dashboard", "Home", new { area = "Retailer" });
                            }
                            else
                            {
                                TempData.Remove("data");
                                AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                                TempData["userblocked"] = "Your account is currently blocked, contact Administrator";
                                return RedirectToAction("Login", "Home", null);
                            }

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

                // Single query instead of ToList() + FirstOrDefault()
                var showRec = DB.tblSilderIsStatus.FirstOrDefault();
                ViewBag.silder1 = showRec != null ? showRec.Silder1Status : "";

                ViewBag.showsilder1 = DB.tblSilders.Where(a => a.SilderType == "Silder1").ToList();
                ViewBag.showsilder2 = DB.tblSilders.Where(a => a.SilderType == "Silder2").ToList();

                // Load all ADMIN travel records once, then split in-memory
                var allTravels = DB.tblTravels.Where(p => p.Role == "ADMIN").ToList();
                ViewBag.bus = allTravels;
                var bus = allTravels.FirstOrDefault(p => p.TravelType == "Bus");
                ViewBag.buscontent = bus != null ? bus.Content : "";
                ViewBag.image = bus != null ? bus.Image : "";
                var hotel = allTravels.FirstOrDefault(p => p.TravelType == "Hotel");
                ViewBag.hotelcontent = hotel != null ? hotel.Content : "";
                ViewBag.hotelimg = hotel != null ? hotel.Image : "";
                var flight = allTravels.FirstOrDefault(p => p.TravelType == "Flight");
                ViewBag.flightcontent = flight != null ? flight.Content : "";
                ViewBag.flightimg = flight != null ? flight.Image : "";

                var travelcolorRec = DB.tblTravelColors.FirstOrDefault(p => p.Role == "ADMIN");
                ViewBag.travelcolor = travelcolorRec != null ? travelcolorRec.TravelColor : "#9c99b7";

                var about = DB.tblAboutUsContents.FirstOrDefault(p => p.Role == "ADMIN");
                ViewBag.aboutimage = about != null ? about.AboutImage : "";
                ViewBag.abouthead = about != null ? about.AboutHeading : "";
                ViewBag.aboutcon = about != null ? about.AboutContent : "";
                var dthcolorRec = DB.tblDTHColors.FirstOrDefault(p => p.Role == "ADMIN");
                ViewBag.colordth = dthcolorRec != null ? dthcolorRec.DthBackColor : "Red";

                ViewBag.dthplan = DB.tblDTHs.Where(p => p.Role == "ADMIN").ToList();
                ViewBag.team = DB.tblteams.ToList();
                var tcolorRec = DB.tblTeamBackColors.FirstOrDefault();
                ViewBag.tcolor = tcolorRec != null ? tcolorRec.TeamColor : "Blue";

                var contentRec = DB.tblWhyChooseContents.FirstOrDefault();
                ViewBag.content = contentRec != null ? contentRec.AllContent : "";
                ViewBag.allcontent = DB.tblWhyChooseUsAllContents.ToList();
                var whychoosecolorRec = DB.tblWhyChooseBackColors.FirstOrDefault();
                ViewBag.whychoosecolor = whychoosecolorRec != null ? whychoosecolorRec.WhyBackColor : "Green";

                ViewBag.newupdate = DB.tblNewUpdates.Where(p => p.Role == "ADMIN").ToList();
                var backnewcolorRec = DB.tblNewUpdateColors.FirstOrDefault(p => p.Role == "ADMIN");
                ViewBag.newcolor = backnewcolorRec != null ? backnewcolorRec.NewContentBackColor : "Red";

                var colorRec = DB.changecolors.FirstOrDefault(p => p.Role == "ADMIN");
                ViewBag.headcolor = (colorRec != null && !string.IsNullOrEmpty(colorRec.adminandouterheadercolor))
                                    ? colorRec.adminandouterheadercolor : "Gray";

                var contectRec = DB.tblContectBackColors.FirstOrDefault(p => p.Role == "ADMIN");
                ViewBag.contect = contectRec != null ? contectRec.ContectColor : "";
                var addressRec = DB.tblContects.FirstOrDefault();
                ViewBag.Headadd = addressRec != null ? addressRec.HeadOfficeAddress : "";
                ViewBag.branch = addressRec != null ? addressRec.BranceOfficeAddress : "";
                ViewBag.compnay = addressRec != null ? addressRec.CompnayName : "";
                ViewBag.landline = addressRec != null ? addressRec.LandlineNo : "";
                ViewBag.phone = addressRec != null ? addressRec.phone : "";
                ViewBag.emil = addressRec != null ? addressRec.Email : "";

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

                var footercolor = DB.tblfooterbackcolors.Where(p => p.Role == "ADMIN").ToList();
                if (footercolor.Count > 0)
                {
                    ViewBag.footercolor = footercolor.SingleOrDefault().FooterBackColor;
                }
                else
                {
                    ViewBag.footercolor = "#da097d";
                }

                //********************** Start Admin Social Media ****************************
                var Afacelink = DB.tblFooterlinks.Where(p => p.FooterType == "Facebook" && p.Role == "ADMIN").ToList();
                ViewBag.Afac = Afacelink.Count == 0 ? "" : Afacelink.SingleOrDefault().FooterLink;
                var Atwitter = DB.tblFooterlinks.Where(p => p.FooterType == "Twitter" && p.Role == "ADMIN").ToList();
                ViewBag.Atwi = Atwitter.Count == 0 ? "" : Atwitter.SingleOrDefault().FooterLink;
                var AInstagram = DB.tblFooterlinks.Where(p => p.FooterType == "Instagram" && p.Role == "ADMIN").ToList();
                ViewBag.AInstagra = AInstagram.Count == 0 ? "" : AInstagram.SingleOrDefault().FooterLink;
                var Alinked = DB.tblFooterlinks.Where(p => p.FooterType == "LinkedIn" && p.Role == "ADMIN").ToList();
                ViewBag.Alinked = Alinked.Count == 0 ? "" : Alinked.SingleOrDefault().FooterLink;
                var APinterest = DB.tblFooterlinks.Where(p => p.FooterType == "Pinterest" && p.Role == "ADMIN").ToList();
                ViewBag.APinteres = APinterest.Count == 0 ? "" : APinterest.SingleOrDefault().FooterLink;
                var AYoutube = DB.tblFooterlinks.Where(p => p.FooterType == "Youtube" && p.Role == "ADMIN").ToList();
                ViewBag.AYoutub = AYoutube.Count == 0 ? "" : AYoutube.SingleOrDefault().FooterLink;
                var ABlogger = DB.tblFooterlinks.Where(p => p.FooterType == "Blogger" && p.Role == "ADMIN").ToList();
                ViewBag.ABlogge = ABlogger.Count == 0 ? "" : ABlogger.SingleOrDefault().FooterLink;
                var ATumblr = DB.tblFooterlinks.Where(p => p.FooterType == "Tumblr" && p.Role == "ADMIN").ToList();
                ViewBag.ATumbl = ATumblr.Count == 0 ? "" : ATumblr.SingleOrDefault().FooterLink;
                //********************** End Admin Social Media ****************************


                var chkk = DB.WhiteLabel_Badges.Where(p => p.UserRole == "ADMIN").ToList();
                if (chkk.Any())
                {
                    ViewBag.checkBadge = DB.WhiteLabel_Badges.Where(p => p.UserRole == "ADMIN").ToList();

                    //********************** Start Admin Badges Media **************************
                    var AppfuturaA = DB.WhiteLabel_Badges.Where(p => p.BadgeType == "Appfutura" && p.UserRole == "ADMIN").ToList();
                    ViewBag.AppfuturA = AppfuturaA.Count == 0 ? "" : AppfuturaA.SingleOrDefault().Url_Link;
                    var ApplancherA = DB.WhiteLabel_Badges.Where(p => p.BadgeType == "Applancher" && p.UserRole == "ADMIN").ToList();
                    ViewBag.ApplancheA = ApplancherA.Count == 0 ? "" : ApplancherA.SingleOrDefault().Url_Link;
                    var BadushA = DB.WhiteLabel_Badges.Where(p => p.BadgeType == "Badush" && p.UserRole == "ADMIN").ToList();
                    ViewBag.BadusA = BadushA.Count == 0 ? "" : BadushA.SingleOrDefault().Url_Link;
                    var ExtractA = DB.WhiteLabel_Badges.Where(p => p.BadgeType == "Extract" && p.UserRole == "ADMIN").ToList();
                    ViewBag.ExtracA = ExtractA.Count == 0 ? "" : ExtractA.SingleOrDefault().Url_Link;
                    var GoodfirmsA = DB.WhiteLabel_Badges.Where(p => p.BadgeType == "Good-firms" && p.UserRole == "ADMIN").ToList();
                    ViewBag.GoodfirmA = GoodfirmsA.Count == 0 ? "" : GoodfirmsA.SingleOrDefault().Url_Link;
                    var MobileappA = DB.WhiteLabel_Badges.Where(p => p.BadgeType == "Mobile-app" && p.UserRole == "ADMIN").ToList();
                    ViewBag.MobileapA = MobileappA.Count == 0 ? "" : MobileappA.SingleOrDefault().Url_Link;
                    var TrustpilotA = DB.WhiteLabel_Badges.Where(p => p.BadgeType == "Trustpilot" && p.UserRole == "ADMIN").ToList();
                    ViewBag.TrustpiloA = TrustpilotA.Count == 0 ? "" : TrustpilotA.SingleOrDefault().Url_Link;
                    var FreelancerA = DB.WhiteLabel_Badges.Where(p => p.BadgeType == "Freelancer" && p.UserRole == "ADMIN").ToList();
                    ViewBag.FreelancerA = FreelancerA.Count == 0 ? "" : FreelancerA.SingleOrDefault().Url_Link;
                    //********************** End Admin Badges Media **************************
                }
                else
                {
                    ViewBag.checkBadge = null;
                }

            }
            else
            {
                var show = DB.tblSilderIsStatus.ToList();
                if (show.Count > 0)
                {
                    if (show.FirstOrDefault().Silder1Status == "Y")
                    {
                        ViewBag.silder1 = show.FirstOrDefault().Silder1Status;
                    }
                    else
                    {
                        ViewBag.silder1 = show.FirstOrDefault().Silder1Status;
                    }
                }
                ViewBag.showsilder1 = DB.tblSilders.Where(a => a.SilderType == "Silder1").ToList();
                ViewBag.showsilder2 = DB.tblSilders.Where(a => a.SilderType == "Silder2").ToList();
                var bus = DB.tblTravels.Where(p => p.Role == "ADMIN" && p.TravelType == "Bus").ToList();
                ViewBag.bus = DB.tblTravels.Where(p => p.Role == "ADMIN").ToList();
                ViewBag.buscontent = bus.Count == 0 ? "" : bus.Where(p => p.TravelType == "Bus").FirstOrDefault().Content;
                ViewBag.image = bus.Count == 0 ? "" : bus.Where(p => p.TravelType == "Bus").FirstOrDefault().Image;
                var hotel = DB.tblTravels.Where(p => p.Role == "ADMIN" && p.TravelType == "Hotel").ToList();
                ViewBag.hotelcontent = hotel.Count == 0 ? "" : hotel.Where(p => p.TravelType == "Hotel").FirstOrDefault().Content;
                ViewBag.hotelimg = hotel.Count == 0 ? "" : hotel.Where(p => p.TravelType == "Hotel").FirstOrDefault().Image;
                var flight = DB.tblTravels.Where(p => p.Role == "ADMIN" && p.TravelType == "Flight").ToList();
                ViewBag.flightcontent = flight.Count == 0 ? "" : flight.Where(p => p.TravelType == "Flight").FirstOrDefault().Content;
                ViewBag.flightimg = flight.Count == 0 ? "" : flight.Where(p => p.TravelType == "Flight").FirstOrDefault().Image;
                var travelcolor = DB.tblTravelColors.Where(p => p.Role == "ADMIN").ToList();
                if (travelcolor.Count > 0)
                {
                    ViewBag.travelcolor = travelcolor.SingleOrDefault().TravelColor;
                }
                else
                {
                    ViewBag.travelcolor = "#9c99b7";
                }
                var about2 = DB.tblAboutUsContents.FirstOrDefault(p => p.Role == "ADMIN");
                ViewBag.aboutimage = about2 != null ? about2.AboutImage : "";
                ViewBag.abouthead = about2 != null ? about2.AboutHeading : "";
                ViewBag.aboutcon = about2 != null ? about2.AboutContent : "";

                var dthcolorRec2 = DB.tblDTHColors.FirstOrDefault(p => p.Role == "ADMIN");
                ViewBag.colordth = dthcolorRec2 != null ? dthcolorRec2.DthBackColor : "Red";

                ViewBag.dthplan = DB.tblDTHs.Where(p => p.Role == "ADMIN").ToList();
                ViewBag.team = DB.tblteams.ToList();
                var tcolorRec2 = DB.tblTeamBackColors.FirstOrDefault();
                ViewBag.tcolor = tcolorRec2 != null ? tcolorRec2.TeamColor : "Blue";

                var contentRec2 = DB.tblWhyChooseContents.FirstOrDefault();
                ViewBag.content = contentRec2 != null ? contentRec2.AllContent : "";
                ViewBag.allcontent = DB.tblWhyChooseUsAllContents.ToList();
                var whychoosecolorRec2 = DB.tblWhyChooseBackColors.FirstOrDefault();
                ViewBag.whychoosecolor = whychoosecolorRec2 != null ? whychoosecolorRec2.WhyBackColor : "Green";

                ViewBag.newupdate = DB.tblNewUpdates.Where(p => p.Role == "ADMIN").ToList();
                var backnewcolorRec2 = DB.tblNewUpdateColors.FirstOrDefault(p => p.Role == "ADMIN");
                ViewBag.newcolor = backnewcolorRec2 != null ? backnewcolorRec2.NewContentBackColor : "Red";

                var colorRec2 = DB.changecolors.FirstOrDefault(p => p.Role == "ADMIN");
                ViewBag.headcolor = (colorRec2 != null && !string.IsNullOrEmpty(colorRec2.adminandouterheadercolor))
                                    ? colorRec2.adminandouterheadercolor : "Gray";

                var contectRec2 = DB.tblContectBackColors.FirstOrDefault(p => p.Role == "ADMIN");
                ViewBag.contect = contectRec2 != null ? contectRec2.ContectColor : "";
                var addressRec2 = DB.tblContects.FirstOrDefault();
                ViewBag.Headadd = addressRec2 != null ? addressRec2.HeadOfficeAddress : "";
                ViewBag.branch = addressRec2 != null ? addressRec2.BranceOfficeAddress : "";
                ViewBag.compnay = addressRec2 != null ? addressRec2.CompnayName : "";
                ViewBag.landline = addressRec2 != null ? addressRec2.LandlineNo : "";
                ViewBag.phone = addressRec2 != null ? addressRec2.phone : "";
                ViewBag.emil = addressRec2 != null ? addressRec2.Email : "";

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

                var footercolor = DB.tblfooterbackcolors.Where(p => p.Role == "ADMIN").ToList();
                if (footercolor.Count > 0)
                {
                    ViewBag.footercolor = footercolor.SingleOrDefault().FooterBackColor;
                }
                else
                {
                    ViewBag.footercolor = "#da097d";
                }

                //********************** Start Admin Social Media ****************************
                var Afacelink = DB.tblFooterlinks.Where(p => p.FooterType == "Facebook" && p.Role == "ADMIN").ToList();
                ViewBag.Afac = Afacelink.Count == 0 ? "" : Afacelink.SingleOrDefault().FooterLink;
                var Atwitter = DB.tblFooterlinks.Where(p => p.FooterType == "Twitter" && p.Role == "ADMIN").ToList();
                ViewBag.Atwi = Atwitter.Count == 0 ? "" : Atwitter.SingleOrDefault().FooterLink;
                var AInstagram = DB.tblFooterlinks.Where(p => p.FooterType == "Instagram" && p.Role == "ADMIN").ToList();
                ViewBag.AInstagra = AInstagram.Count == 0 ? "" : AInstagram.SingleOrDefault().FooterLink;
                var Alinked = DB.tblFooterlinks.Where(p => p.FooterType == "LinkedIn" && p.Role == "ADMIN").ToList();
                ViewBag.Alinked = Alinked.Count == 0 ? "" : Alinked.SingleOrDefault().FooterLink;
                var APinterest = DB.tblFooterlinks.Where(p => p.FooterType == "Pinterest" && p.Role == "ADMIN").ToList();
                ViewBag.APinteres = APinterest.Count == 0 ? "" : APinterest.SingleOrDefault().FooterLink;
                var AYoutube = DB.tblFooterlinks.Where(p => p.FooterType == "Youtube" && p.Role == "ADMIN").ToList();
                ViewBag.AYoutub = AYoutube.Count == 0 ? "" : AYoutube.SingleOrDefault().FooterLink;
                var ABlogger = DB.tblFooterlinks.Where(p => p.FooterType == "Blogger" && p.Role == "ADMIN").ToList();
                ViewBag.ABlogge = Alinked.Count == 0 ? "" : Alinked.SingleOrDefault().FooterLink;
                var ATumblr = DB.tblFooterlinks.Where(p => p.FooterType == "Tumblr" && p.Role == "ADMIN").ToList();
                ViewBag.ATumbl = ATumblr.Count == 0 ? "" : ATumblr.SingleOrDefault().FooterLink;
                //********************** End Admin Social Media ****************************

                //********************** Start Admin Badges Media **************************
                var AppfuturaA = DB.WhiteLabel_Badges.Where(p => p.BadgeType == "Appfutura" && p.UserRole == "ADMIN").ToList();
                ViewBag.AppfuturA = AppfuturaA.Count == 0 ? "" : AppfuturaA.SingleOrDefault().Url_Link;
                var ApplancherA = DB.WhiteLabel_Badges.Where(p => p.BadgeType == "Applancher" && p.UserRole == "ADMIN").ToList();
                ViewBag.ApplancheA = ApplancherA.Count == 0 ? "" : ApplancherA.SingleOrDefault().Url_Link;
                var BadushA = DB.WhiteLabel_Badges.Where(p => p.BadgeType == "Badush" && p.UserRole == "ADMIN").ToList();
                ViewBag.BadusA = BadushA.Count == 0 ? "" : BadushA.SingleOrDefault().Url_Link;
                var ExtractA = DB.WhiteLabel_Badges.Where(p => p.BadgeType == "Extract" && p.UserRole == "ADMIN").ToList();
                ViewBag.ExtracA = ExtractA.Count == 0 ? "" : ExtractA.SingleOrDefault().Url_Link;
                var GoodfirmsA = DB.WhiteLabel_Badges.Where(p => p.BadgeType == "Good-firms" && p.UserRole == "ADMIN").ToList();
                ViewBag.GoodfirmA = GoodfirmsA.Count == 0 ? "" : GoodfirmsA.SingleOrDefault().Url_Link;
                var MobileappA = DB.WhiteLabel_Badges.Where(p => p.BadgeType == "Mobile-app" && p.UserRole == "ADMIN").ToList();
                ViewBag.MobileapA = MobileappA.Count == 0 ? "" : MobileappA.SingleOrDefault().Url_Link;
                var TrustpilotA = DB.WhiteLabel_Badges.Where(p => p.BadgeType == "Trustpilot" && p.UserRole == "ADMIN").ToList();
                ViewBag.TrustpiloA = TrustpilotA.Count == 0 ? "" : TrustpilotA.SingleOrDefault().Url_Link;
                var FreelancerA = DB.WhiteLabel_Badges.Where(p => p.BadgeType == "Freelancer" && p.UserRole == "ADMIN").ToList();
                ViewBag.FreelancerA = FreelancerA.Count == 0 ? "" : FreelancerA.SingleOrDefault().Url_Link;
                //********************** End Admin Badges Media **************************
                ViewData["msg"] = "websiteblock";
            }


            return View();
        }

        /// <summary>
        /// User delete karne ka page dikhata hai
        /// </summary>
        public ActionResult DeleteUser()
        {
            return View();
        }

        private string generatenum()
        {
            //Generate Random no.of 4 Digit between range 0 to 9999
            Random _rdm = new Random();
            var otp = _rdm.Next(0, 9999).ToString("0000");
            return otp;
        }
        public ActionResult DeleteRetailer(string mobiles)
        {
            try
            {
                var retailer = DB.Retailer_Details.FirstOrDefault(x => x.Mobile == mobiles) ?? DB.Retailer_Details.FirstOrDefault(x => x.Email == mobiles);
                if (retailer == null)
                {
                    return Json("This Email and Mobile Number is not registered with us!", JsonRequestBehavior.AllowGet);
                }
                var activeRetailer = DB.Retailer_Details.SingleOrDefault(aa => aa.RetailerId == retailer.RetailerId && aa.ISDeleteuser == false);
                if (activeRetailer == null)
                {
                    return Json("User is already deleted or inactive!", JsonRequestBehavior.AllowGet);
                }
                string userId = retailer.RetailerId;
                var smsStatus = DB.SMSSendAlls.SingleOrDefault(a => a.ServiceName == "DeleteUserOTP")?.Status;
                var emailStatus = DB.EmailSendAlls.SingleOrDefault(a => a.ServiceName == "DeleteUserOTP1")?.Status;
                var otp = generatenum();
                if (smsStatus == "Y" || emailStatus == "Y")
                {
                    var mobileOtp = new MobileOtp
                    {
                        Date = DateTime.Now,
                        mobileno = activeRetailer.Mobile,
                        Otp = otp,
                        Type = "RetailerDelete",
                        Userid = userId
                    };
                    DB.MobileOtps.Add(mobileOtp);
                    DB.SaveChanges();
                }
                var txtMsgBody = $"Retailer Delete OTP: {otp}";
                var adminDetails = DB.Admin_details.SingleOrDefault();
                if (adminDetails == null)
                {
                    return Json("Admin details not found!", JsonRequestBehavior.AllowGet);
                }
                smssend.sms_init(smsStatus, smsStatus, "RetailerDelete", adminDetails.mobile, otp);
                if (emailStatus == "Y")
                {
                    smssend.SendEmailAll(adminDetails.email, txtMsgBody, "User Delete", adminDetails.Companyname);
                }
                return Json("success", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json($"Error: {ex.Message}", JsonRequestBehavior.AllowGet);
            }
        }

        public async Task<ActionResult> ConfirmDeleteRetailerOTP(string mobile, string Deleteotp)
        {
            RetailerDetalsModel viewmodel = new RetailerDetalsModel();
            try
            {
                var retailer = DB.Retailer_Details.FirstOrDefault(x => x.Mobile == mobile) ?? DB.Retailer_Details.FirstOrDefault(x => x.Email == mobile);
                if (retailer == null)
                {
                    return Json("This Email and Mobile Number is not registered with us!", JsonRequestBehavior.AllowGet);
                }
                var chk = await DB.MobileOtps.Where(aa => aa.Otp == Deleteotp).Take(1).OrderByDescending(aa => aa.Date).SingleOrDefaultAsync();
                if (chk != null)
                {
                    var ekycstschk = await DB.ekycChecks.Where(x => x.userid == retailer.RetailerId).SingleOrDefaultAsync();
                    string msg = "OK";
                    if (ekycstschk != null)
                    {
                        if (ekycstschk.isvalid == true)
                        {
                            msg = "NOTOK";
                        }
                    }
                    if(msg == "OK")
                    {
                        System.Data.Entity.Core.Objects.ObjectParameter output = new System.Data.Entity.Core.Objects.ObjectParameter("Output", typeof(string));

                        var msgres = DB.Retailer_delete_Only_change_Status(retailer.RetailerId, output).SingleOrDefault().msg;

                        if (msgres.ToUpper() == "SUCCESS")
                        {
                            UserManager.UpdateSecurityStamp(retailer.RetailerId);

                            var retailersss = await DB.Retailer_Details.Where(x => x.RetailerId == retailer.RetailerId).SingleOrDefaultAsync();

                            string path = Server.MapPath("~/" + retailersss.videokycpath);
                            if (System.IO.File.Exists(path))
                            {
                                System.IO.File.Delete(path);
                                retailersss.videokycpath = null;
                                DB.SaveChanges();
                            }
                        }
                        viewmodel.select_retailer_details_paging = DB.Select_Retailer_Details_all_paging(1, 500, "ADMIN").ToList();
                        viewmodel.Show_Service_namelist = DB.Show_Service_name.ToList();
                        viewmodel.Service_BlockUserwiseclslist = from tbl in DB.Show_Service_name
                                                                 join tbl1 in DB.Service_BlockUserwise
                                                                 on tbl.idno equals tbl1.Show_Service_name_id
                                                                 select new Service_BlockUserwisecls
                                                                 {
                                                                     idno = tbl.idno,
                                                                     servicename = tbl.servicename,
                                                                     serviceidno = tbl1.Show_Service_name_id,
                                                                     remid = tbl1.remid,
                                                                     servicestatus = tbl1.status,
                                                                     basedupdate_id = tbl1.idno
                                                                 };
                        return Json("success", JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json("This User Kyc is Exists. You can not delete this user!", JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json("OTP Miss Match!", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }


        }

        public ActionResult DownloadLink()
        {
            var callbackUrl = Url.Action("DownloadAPK", "", new { }, protocol: Request.Url.Scheme);
            return Redirect("Index1");
        }

        public ActionResult pagecontetn(string type)
        {
            var cont = DB.tblFooterPageContents.Where(p => p.FooterPagetype == type && p.Role == "ADMIN").SingleOrDefault();
            var value = cont == null ? "" : cont.FooterPageContent;
            return Json(value, JsonRequestBehavior.AllowGet);
        }

        public ActionResult pagefootercontent(string type)
        {
            string currenturl = HttpContext.Request.Url.Authority;
            currenturl = currenturl.Replace("www.", "").Replace("https://", "").Replace("http://", "");
            currenturl = currenturl.ToUpper();
            var whitelabelid = "";
            if (currenturl.Contains("LOCALHOST"))
            {
                whitelabelid = DB.WhiteLabel_userList.Where(aa => aa.localhost.Replace("www.", "").Replace("https://", "").Replace("http://", "").Replace("/", "").ToLower() == currenturl.ToLower()).SingleOrDefault().WhiteLabelID;
            }
            else
            {
                whitelabelid = DB.WhiteLabel_userList.Where(aa => aa.websitename.Replace("www.", "").Replace("https://", "").Replace("http://", "").Replace("/", "").ToLower() == currenturl.ToLower()).SingleOrDefault().WhiteLabelID;
            }
            //var whitelabelid = DB.WhiteLabel_userList.Where(aa => aa.websitename.Replace("www.", "").Replace("https://", "").Replace("http://", "").ToLower() == currenturl.ToLower()).SingleOrDefault().WhiteLabelID;

            var cont = DB.tblFooterPageContents.Where(p => p.FooterPagetype == type && p.UserId == whitelabelid).SingleOrDefault();
            var value = cont == null ? "" : cont.FooterPageContent;
            return Json(value, JsonRequestBehavior.AllowGet);
        }
        public ActionResult First()
        {
            string currenturl = HttpContext.Request.Url.Authority;
            currenturl = currenturl.Replace("www.", "").Replace("https://", "").Replace("http://", "");
            ViewBag.DomainName = currenturl;
            return View();
        }

        public ActionResult Index(string returnUrl)
        {

            var xx = DB.Admin_details == null ? DB.Admin_details.SingleOrDefault().RenivalDate : System.DateTime.Now;
            var x = xx >= DateTime.Now;
            if (x)
            {

                var URL = "";
                string currenturl = HttpContext.Request.Url.Authority;
                currenturl = currenturl.Replace("www.", "").Replace("https://", "").Replace("http://", "");
                currenturl = currenturl.ToUpper();
                if (currenturl.Contains("LOCALHOST"))
                {
                    var URL1 = DB.Admin_details.ToList();
                    if (URL1.Count() == 0)
                    {
                        return RedirectToAction("First", "Home");
                    }
                    else
                    {
                        URL = URL1.SingleOrDefault().localhost;
                    }
                }
                else
                {
                    var URL1 = DB.Admin_details.ToList();
                    if (URL1.Count() == 0)
                    {
                        return RedirectToAction("First", "Home");
                    }
                    else
                    {
                        URL = URL1.SingleOrDefault().WebsiteUrl;
                    }
                }

                if (URL.ToLower() != currenturl.ToLower())
                {
                    using (VastwebmultiEntities db = new VastwebmultiEntities())
                    {
                        var whitelabelid = "";
                        if (currenturl.Contains("LOCALHOST"))
                        {
                            whitelabelid = db.WhiteLabel_userList.Where(a => a.localhost.Replace("www.", "").Replace("https://", "").Replace("http://", "").ToLower() == currenturl.ToLower()).SingleOrDefault().WhiteLabelID;
                        }
                        else
                        {
                            whitelabelid = db.WhiteLabel_userList.Where(a => a.websitename.Replace("www.", "").Replace("https://", "").Replace("http://", "").ToLower() == currenturl.ToLower()).SingleOrDefault().WhiteLabelID;
                        }

                        if (User.IsInRole("Whitelabel"))
                        {
                            return RedirectToAction("Dashboard", "Home", new { area = "WHITELABEL" });
                        }
                        else if (User.IsInRole("Whitelabelmaster"))
                        {
                            try
                            {
                                ApiTokenGen TknGen = new ApiTokenGen();
                                TknGen.TokenGenerateForWApi((string)TempData["Userid"], (string)TempData["LoginId"], (string)TempData["Password"], "Whitelabelmaster", whitelabelid);
                            }
                            catch { }
                            TempData.Remove("Userid"); TempData.Remove("LoginId"); TempData.Remove("Password");

                            return RedirectToAction("Dashboard", "Home", new { area = "WMASTER" });
                        }
                        else if (User.IsInRole("Whitelabeldealer"))
                        {
                            try
                            {
                                ApiTokenGen TknGen = new ApiTokenGen();
                                TknGen.TokenGenerateForWApi((string)TempData["Userid"], (string)TempData["LoginId"], (string)TempData["Password"], "Whitelabeldealer", whitelabelid);
                            }
                            catch { }
                            TempData.Remove("Userid"); TempData.Remove("LoginId"); TempData.Remove("Password");

                            return RedirectToAction("Dashboard", "Home", new { area = "WDealer" });
                        }
                        else if (User.IsInRole("Whitelabelretailer"))
                        {
                            try
                            {
                                ApiTokenGen TknGen = new ApiTokenGen();
                                TknGen.TokenGenerateForWApi((string)TempData["Userid"], (string)TempData["LoginId"], (string)TempData["Password"], "Whitelabelretailer", whitelabelid);
                            }
                            catch { }
                            TempData.Remove("Userid"); TempData.Remove("LoginId"); TempData.Remove("Password");

                            return RedirectToAction("Dashboard", "Home", new { area = "WRetailer" });
                        }
                        var silder = db.tblWhitlabelSilders.ToList();
                        if (silder.Count > 0)
                        {
                            ViewBag.indexsilder = silder.Where(p => p.UserId == whitelabelid).ToList();
                        }
                        else
                        {
                            ViewBag.indexsilder = "";
                        }

                        // ViewBag.silderback = db.tblSilderBackColors.SingleOrDefault().SilderColor == null ? "" : DB.tblSilderBackColors.SingleOrDefault().SilderColor;
                        var st = db.tblMenus.Where(p => p.UserId == whitelabelid).ToList();
                        ViewBag.homests = st.Count == 0 ? "" : st.Where(aa => aa.MenuName == "HOME").SingleOrDefault().Status;
                        ViewBag.aboutsts = st.Count == 0 ? "" : st.Where(aa => aa.MenuName == "ABOUT").SingleOrDefault().Status;
                        ViewBag.servicests = st.Count == 0 ? "" : st.Where(aa => aa.MenuName == "SERVICES").SingleOrDefault().Status;
                        ViewBag.cardsts = st.Count == 0 ? "" : st.Where(aa => aa.MenuName == "CARDS").SingleOrDefault().Status;
                        ViewBag.travelsts = st.Count == 0 ? "" : st.Where(aa => aa.MenuName == "TRAVEL").SingleOrDefault().Status;
                        ViewBag.dthsts = st.Count == 0 ? "" : st.Where(aa => aa.MenuName == "DTHCONNECTION").SingleOrDefault().Status;
                        ViewBag.contactsts = st.Count == 0 ? "" : st.Where(aa => aa.MenuName == "CONTACTUS").SingleOrDefault().Status;
                        ViewBag.newupdatasts = st.Count == 0 ? "" : st.Where(aa => aa.MenuName == "NEWUPDATE").SingleOrDefault().Status;
                        //Services Content
                        ViewBag.service = DB.tblServices.Where(p => p.UserId == whitelabelid).ToList();
                        var colorservice = DB.tblServiceOuterColors.Where(p => p.UserId == whitelabelid).ToList();
                        if (colorservice.Count > 0)
                        {
                            ViewBag.servicecolor = colorservice.SingleOrDefault().ServiceBackGroundcolor;
                        }
                        else
                        {
                            ViewBag.servicecolor = "Green";
                        }

                        //Services Content
                        //About Us Content
                        //var aboutimage = DB.tblAboutUsContents.Where(p => p.UserId == whitelabelid).ToList();

                        List<Vastwebmulti.Areas.WHITELABEL.Models.JsonFile.AboutUs> aboutimage = new List<Vastwebmulti.Areas.WHITELABEL.Models.JsonFile.AboutUs>();
                        JSONReadWrite readWrite = new JSONReadWrite();
                        aboutimage = JsonConvert.DeserializeObject<List<Vastwebmulti.Areas.WHITELABEL.Models.JsonFile.AboutUs>>(readWrite.Read("AboutUs.json", "~/Areas/WHITELABEL/Models/JsonFile/"));
                        aboutimage = aboutimage.Where(p => p.UserId == whitelabelid).ToList();

                        //ViewBag.aboutimage = aboutimage.Count == 0 ? "" : aboutimage.SingleOrDefault().AboutImage;
                        ViewBag.aboutheading = aboutimage.Count == 0 ? "" : aboutimage.SingleOrDefault().AboutHeading;
                        ViewBag.aboutcontent = aboutimage.Count == 0 ? "" : aboutimage.SingleOrDefault().AboutContent;

                        //*About Us Content

                        //*Card Content
                        ViewBag.card = DB.tblCards.Where(p => p.UserId == whitelabelid).ToList();
                        var colorcard = DB.tblCardBackColors.Where(p => p.UserId == whitelabelid).ToList();
                        ViewBag.cardcolor = colorcard.Count == 0 ? "" : colorcard.SingleOrDefault().CardBackColor;
                        //*Card Content

                        //*Travels Content
                        var alltravelbus = DB.tblTravels.Where(p => p.UserId == whitelabelid && p.TravelType == "Bus").ToList();
                        ViewBag.busimage = alltravelbus.Count == 0 ? "" : alltravelbus.SingleOrDefault().Image;
                        ViewBag.buscontent = alltravelbus.Count == 0 ? "" : alltravelbus.SingleOrDefault().Content;
                        var alltravelflight = DB.tblTravels.Where(p => p.UserId == whitelabelid && p.TravelType == "Flight").ToList();
                        ViewBag.flightimage = alltravelflight.Count == 0 ? "" : alltravelflight.SingleOrDefault().Image;
                        ViewBag.flightcontent = alltravelflight.Count == 0 ? "" : alltravelflight.SingleOrDefault().Content;
                        var alltravelhotel = DB.tblTravels.Where(p => p.UserId == whitelabelid && p.TravelType == "Hotel").ToList();
                        ViewBag.hotelimage = alltravelhotel.Count == 0 ? "" : alltravelhotel.SingleOrDefault().Image;
                        ViewBag.hotelcontent = alltravelhotel.Count == 0 ? "" : alltravelhotel.SingleOrDefault().Content;
                        var colortravel = DB.tblTravelColors.Where(p => p.UserId == whitelabelid).ToList();
                        ViewBag.travelbackcolor = colortravel.Count == 0 ? "" : colortravel.SingleOrDefault().TravelColor;
                        //*Travels Content
                        //Dth Connection
                        ViewBag.dth = DB.tblDTHs.Where(p => p.UserId == whitelabelid).ToList();
                        var colordth = DB.tblDTHColors.Where(p => p.UserId == whitelabelid).ToList();
                        ViewBag.dthcolor = colordth.Count == 0 ? "" : colordth.SingleOrDefault().DthBackColor;
                        //Dth Connection
                        //New Update Content

                        List<Vastwebmulti.Areas.WHITELABEL.Models.JsonFile.NewsUpdate> NEWS = new List<Vastwebmulti.Areas.WHITELABEL.Models.JsonFile.NewsUpdate>();

                        NEWS = JsonConvert.DeserializeObject<List<Vastwebmulti.Areas.WHITELABEL.Models.JsonFile.NewsUpdate>>(readWrite.Read("NewsUpdate.json", "~/Areas/WHITELABEL/Models/JsonFile/"));
                        ViewBag.newupdate = NEWS.Where(p => p.UserId == whitelabelid).ToList();
                        //ViewBag.newupdate = DB.tblWhiteLabelNewUpdates.Where(p => p.UserId == whitelabelid).ToList();

                        //New Update Content
                        //Contact Data
                        //var companyname = DB.tblWhiteContactDatas.Where(p => p.UserId == whitelabelid).ToList();
                        List<Vastwebmulti.Areas.WHITELABEL.Models.JsonFile.WhiteContactData> ContactData = new List<Vastwebmulti.Areas.WHITELABEL.Models.JsonFile.WhiteContactData>();
                        ContactData = JsonConvert.DeserializeObject<List<Vastwebmulti.Areas.WHITELABEL.Models.JsonFile.WhiteContactData>>(readWrite.Read("WhiteContactData.json", "~/Areas/WHITELABEL/Models/JsonFile/"));
                        var companyname = ContactData.Where(p => p.UserId == whitelabelid).ToList();
                        ViewBag.companyname = companyname.Count == 0 ? "" : companyname.SingleOrDefault().CompnayName;
                        ViewBag.phone = companyname.Count == 0 ? "" : companyname.SingleOrDefault().phone;
                        ViewBag.landline = companyname.Count == 0 ? "" : companyname.SingleOrDefault().LandlineNo;
                        ViewBag.address = companyname.Count == 0 ? "" : companyname.SingleOrDefault().Address;
                        ViewBag.email = companyname.Count == 0 ? "" : companyname.SingleOrDefault().Email;
                        //********************** Start Whitelabel Social Media ****************************
                        var facelink = DB.tblFooterlinks.Where(p => p.FooterType == "Facebook" && p.UserId == whitelabelid).ToList();
                        ViewBag.fac = facelink.Count == 0 ? "" : facelink.SingleOrDefault().FooterLink;
                        var twitter = DB.tblFooterlinks.Where(p => p.FooterType == "Twitter" && p.UserId == whitelabelid).ToList();
                        ViewBag.twi = twitter.Count == 0 ? "" : twitter.SingleOrDefault().FooterLink;
                        var Instagram = DB.tblFooterlinks.Where(p => p.FooterType == "Instagram" && p.UserId == whitelabelid).ToList();
                        ViewBag.Instagra = Instagram.Count == 0 ? "" : Instagram.SingleOrDefault().FooterLink;
                        var linked = DB.tblFooterlinks.Where(p => p.FooterType == "LinkedIn" && p.UserId == whitelabelid).ToList();
                        ViewBag.linked = linked.Count == 0 ? "" : linked.SingleOrDefault().FooterLink;
                        var skype = DB.tblFooterlinks.Where(p => p.FooterType == "Skype" && p.UserId == whitelabelid).ToList();
                        ViewBag.skyp = skype.Count == 0 ? "" : skype.SingleOrDefault().FooterLink;
                        var Pinterest = DB.tblFooterlinks.Where(p => p.FooterType == "Pinterest" && p.UserId == whitelabelid).ToList();
                        ViewBag.Pinteres = Pinterest.Count == 0 ? "" : Pinterest.SingleOrDefault().FooterLink;
                        var Youtube = DB.tblFooterlinks.Where(p => p.FooterType == "Youtube" && p.UserId == whitelabelid).ToList();
                        ViewBag.Youtub = Youtube.Count == 0 ? "" : Youtube.SingleOrDefault().FooterLink;
                        var Blogger = DB.tblFooterlinks.Where(p => p.FooterType == "Blogger" && p.UserId == whitelabelid).ToList();
                        ViewBag.Blogge = linked.Count == 0 ? "" : linked.SingleOrDefault().FooterLink;
                        var Tumblr = DB.tblFooterlinks.Where(p => p.FooterType == "Tumblr" && p.UserId == whitelabelid).ToList();
                        ViewBag.Tumbl = Tumblr.Count == 0 ? "" : Tumblr.SingleOrDefault().FooterLink;
                        //********************** End Whitelabel Social Media ****************************

                        //********************** Start Whitelabel Badges Media **************************
                        var Appfutura = DB.WhiteLabel_Badges.Where(p => p.BadgeType == "Appfutura" && p.Userid == whitelabelid).ToList();
                        ViewBag.Appfutur = Appfutura.Count == 0 ? "" : Appfutura.SingleOrDefault().Url_Link;
                        var Applancher = DB.WhiteLabel_Badges.Where(p => p.BadgeType == "Applancher" && p.Userid == whitelabelid).ToList();
                        ViewBag.Applanche = Applancher.Count == 0 ? "" : Applancher.SingleOrDefault().Url_Link;
                        var Badush = DB.WhiteLabel_Badges.Where(p => p.BadgeType == "Badush" && p.Userid == whitelabelid).ToList();
                        ViewBag.Badus = Badush.Count == 0 ? "" : Badush.SingleOrDefault().Url_Link;
                        var Extract = DB.WhiteLabel_Badges.Where(p => p.BadgeType == "Extract" && p.Userid == whitelabelid).ToList();
                        ViewBag.Extrac = Extract.Count == 0 ? "" : Extract.SingleOrDefault().Url_Link;
                        var Goodfirms = DB.WhiteLabel_Badges.Where(p => p.BadgeType == "Good-firms" && p.Userid == whitelabelid).ToList();
                        ViewBag.Goodfirm = Goodfirms.Count == 0 ? "" : Goodfirms.SingleOrDefault().Url_Link;
                        var Mobileapp = DB.WhiteLabel_Badges.Where(p => p.BadgeType == "Mobile-app" && p.Userid == whitelabelid).ToList();
                        ViewBag.Mobileap = Mobileapp.Count == 0 ? "" : Mobileapp.SingleOrDefault().Url_Link;
                        var Trustpilot = DB.WhiteLabel_Badges.Where(p => p.BadgeType == "Trustpilot" && p.Userid == whitelabelid).ToList();
                        ViewBag.Trustpilo = Trustpilot.Count == 0 ? "" : Trustpilot.SingleOrDefault().Url_Link;
                        //********************** End Whitelabel Badges Media **************************
                        //Contact Data
                        //Footer Data
                        var footerdata = DB.tblWhiteLabel_FooterServices.Where(p => p.ServiceType == "Recharge & Bill" && p.UserId == whitelabelid).ToList();
                        ViewBag.rech1 = footerdata.Count == 0 ? "" : footerdata.SingleOrDefault().ServiceName1;
                        ViewBag.rech2 = footerdata.Count == 0 ? "" : footerdata.SingleOrDefault().ServiceName2;
                        ViewBag.rech3 = footerdata.Count == 0 ? "" : footerdata.SingleOrDefault().ServiceName3;
                        ViewBag.rech4 = footerdata.Count == 0 ? "" : footerdata.SingleOrDefault().ServiceName4;
                        ViewBag.rech5 = footerdata.Count == 0 ? "" : footerdata.SingleOrDefault().ServiceName5;
                        var traveldata = DB.tblWhiteLabel_FooterServices.Where(p => p.ServiceType == "Travel & Hotel" && p.UserId == whitelabelid).ToList();
                        ViewBag.tra1 = traveldata.Count == 0 ? "" : traveldata.SingleOrDefault().ServiceName1;
                        ViewBag.tra2 = traveldata.Count == 0 ? "" : traveldata.SingleOrDefault().ServiceName2;
                        ViewBag.tra3 = traveldata.Count == 0 ? "" : traveldata.SingleOrDefault().ServiceName3;
                        ViewBag.tra4 = traveldata.Count == 0 ? "" : traveldata.SingleOrDefault().ServiceName4;
                        ViewBag.tra5 = traveldata.Count == 0 ? "" : traveldata.SingleOrDefault().ServiceName5;
                        var Ecommercedata = DB.tblWhiteLabel_FooterServices.Where(p => p.ServiceType == "E-Commerce" && p.UserId == whitelabelid).ToList();
                        ViewBag.Ecommerce1 = Ecommercedata.Count == 0 ? "" : Ecommercedata.SingleOrDefault().ServiceName1;
                        ViewBag.Ecommerce2 = Ecommercedata.Count == 0 ? "" : Ecommercedata.SingleOrDefault().ServiceName2;
                        ViewBag.Ecommerce3 = Ecommercedata.Count == 0 ? "" : Ecommercedata.SingleOrDefault().ServiceName3;
                        ViewBag.Ecommerce4 = Ecommercedata.Count == 0 ? "" : Ecommercedata.SingleOrDefault().ServiceName4;
                        ViewBag.Ecommerce5 = Ecommercedata.Count == 0 ? "" : Ecommercedata.SingleOrDefault().ServiceName5;
                        var Giftdata = DB.tblWhiteLabel_FooterServices.Where(p => p.ServiceType == "Gift Cards" && p.UserId == whitelabelid).ToList();
                        ViewBag.Gift1 = Giftdata.Count == 0 ? "" : Giftdata.SingleOrDefault().ServiceName1;
                        ViewBag.Gift2 = Giftdata.Count == 0 ? "" : Giftdata.SingleOrDefault().ServiceName2;
                        ViewBag.Gift3 = Giftdata.Count == 0 ? "" : Giftdata.SingleOrDefault().ServiceName3;
                        ViewBag.Gift4 = Giftdata.Count == 0 ? "" : Giftdata.SingleOrDefault().ServiceName4;
                        ViewBag.Gift5 = Giftdata.Count == 0 ? "" : Giftdata.SingleOrDefault().ServiceName5;

                        var footercolor = DB.tblfooterbackcolors.Where(p => p.UserId == whitelabelid).ToList();
                        ViewBag.footercolor = footercolor.Count == 0 ? "" : footercolor.SingleOrDefault().FooterBackColor;
                        //Footer Data
                        return View();
                    }
                }
                else
                {
                    return RedirectToAction("Index1");
                }
            }
            else
            {
                var URL = "";
                string currenturl = HttpContext.Request.Url.Authority;
                currenturl = currenturl.Replace("www.", "").Replace("https://", "").Replace("http://", "");
                currenturl = currenturl.ToUpper();
                if (currenturl.Contains("LOCALHOST"))
                {
                    var URL1 = DB.Admin_details.ToList();
                    if (URL1.Count() == 0)
                    {
                        return RedirectToAction("First", "Home");
                    }
                    else
                    {
                        URL = URL1.SingleOrDefault().localhost;
                    }
                }
                else
                {
                    var URL1 = DB.Admin_details.ToList();
                    if (URL1.Count() == 0)
                    {
                        return RedirectToAction("First", "Home");
                    }
                    else
                    {
                        URL = URL1.SingleOrDefault().WebsiteUrl;
                    }
                }
                if (URL.ToLower() != currenturl.ToLower())
                {
                    //try
                    //{

                    using (VastwebmultiEntities db = new VastwebmultiEntities())
                    {
                        var whitelabelid = "";
                        if (currenturl.Contains("LOCALHOST"))
                        {
                            whitelabelid = db.WhiteLabel_userList.Where(aa => aa.localhost.Replace("www.", "").Replace("https://", "").Replace("http://", "").ToLower() == currenturl.ToLower()).SingleOrDefault().WhiteLabelID;
                        }
                        else
                        {
                            whitelabelid = db.WhiteLabel_userList.Where(aa => aa.websitename.Replace("www.", "").Replace("https://", "").Replace("http://", "").ToLower() == currenturl.ToLower()).SingleOrDefault().WhiteLabelID;
                        }

                        //  if (User.IsInRole("Admin"))
                        //{
                        //    return RedirectToAction("Dashboard", "Home", new { area = "ADMIN" });
                        //}
                        if (User.IsInRole("Whitelabel"))
                        {
                            return RedirectToAction("Dashboard", "Home", new { area = "WHITELABEL" });
                        }
                        else if (User.IsInRole("Whitelabelmaster"))
                        {
                            try
                            {
                                ApiTokenGen TknGen = new ApiTokenGen();
                                TknGen.TokenGenerateForWApi((string)TempData["Userid"], (string)TempData["LoginId"], (string)TempData["Password"], "Whitelabelmaster", whitelabelid);
                            }
                            catch { }
                            TempData.Remove("Userid"); TempData.Remove("LoginId"); TempData.Remove("Password");

                            return RedirectToAction("Dashboard", "Home", new { area = "WMASTER" });
                        }
                        else if (User.IsInRole("Whitelabeldealer"))
                        {
                            try
                            {
                                ApiTokenGen TknGen = new ApiTokenGen();
                                TknGen.TokenGenerateForWApi((string)TempData["Userid"], (string)TempData["LoginId"], (string)TempData["Password"], "Whitelabeldealer", whitelabelid);
                            }
                            catch { }
                            TempData.Remove("Userid"); TempData.Remove("LoginId"); TempData.Remove("Password");

                            return RedirectToAction("Dashboard", "Home", new { area = "WDealer" });
                        }
                        else if (User.IsInRole("Whitelabelretailer"))
                        {
                            try
                            {
                                ApiTokenGen TknGen = new ApiTokenGen();
                                TknGen.TokenGenerateForWApi((string)TempData["Userid"], (string)TempData["LoginId"], (string)TempData["Password"], "Whitelabelretailer", whitelabelid);
                            }
                            catch { }
                            TempData.Remove("Userid"); TempData.Remove("LoginId"); TempData.Remove("Password");

                            return RedirectToAction("Dashboard", "Home", new { area = "WRetailer" });
                        }
                        //else if (User.IsInRole("API"))
                        //{
                        //    return RedirectToAction("Index", "Home", new { area = "API" });
                        //}
                        //else if (User.IsInRole("RCH"))
                        //{
                        //    return RedirectToAction("Index", "Home", new { area = "RCH" });
                        //}
                        //else if (User.IsInRole("Sub"))
                        //{
                        //    return RedirectToAction("Index", "Home", new { area = "Sub" });
                        //}

                        //}
                        //catch (Exception ex)
                        //{

                        //}
                        var silder = db.tblWhitlabelSilders.ToList();
                        if (silder.Count > 0)
                        {
                            ViewBag.indexsilder = silder.Where(p => p.UserId == whitelabelid).ToList();
                        }
                        else
                        {
                            ViewBag.indexsilder = "";
                        }

                        // ViewBag.silderback = db.tblSilderBackColors.SingleOrDefault().SilderColor == null ? "" : DB.tblSilderBackColors.SingleOrDefault().SilderColor;
                        var st = db.tblMenus.Where(p => p.UserId == whitelabelid).ToList();
                        ViewBag.homests = st.Count == 0 ? "" : st.Where(aa => aa.MenuName == "HOME").SingleOrDefault().Status;
                        ViewBag.aboutsts = st.Count == 0 ? "" : st.Where(aa => aa.MenuName == "ABOUT").SingleOrDefault().Status;
                        ViewBag.servicests = st.Count == 0 ? "" : st.Where(aa => aa.MenuName == "SERVICES").SingleOrDefault().Status;
                        ViewBag.cardsts = st.Count == 0 ? "" : st.Where(aa => aa.MenuName == "CARDS").SingleOrDefault().Status;
                        ViewBag.travelsts = st.Count == 0 ? "" : st.Where(aa => aa.MenuName == "TRAVEL").SingleOrDefault().Status;
                        ViewBag.dthsts = st.Count == 0 ? "" : st.Where(aa => aa.MenuName == "DTHCONNECTION").SingleOrDefault().Status;
                        ViewBag.contactsts = st.Count == 0 ? "" : st.Where(aa => aa.MenuName == "CONTACTUS").SingleOrDefault().Status;
                        ViewBag.newupdatasts = st.Count == 0 ? "" : st.Where(aa => aa.MenuName == "NEWUPDATE").SingleOrDefault().Status;
                        //Services Content
                        ViewBag.service = DB.tblServices.Where(p => p.UserId == whitelabelid).ToList();
                        var colorservice = DB.tblServiceOuterColors.Where(p => p.UserId == whitelabelid).ToList();
                        if (colorservice.Count > 0)
                        {
                            ViewBag.servicecolor = colorservice.SingleOrDefault().ServiceBackGroundcolor;
                        }
                        else
                        {
                            ViewBag.servicecolor = "Green";
                        }

                        //Services Content
                        //About Us Content
                        //var aboutimage = DB.tblAboutUsContents.Where(p => p.UserId == whitelabelid).ToList();
                        List<Vastwebmulti.Areas.WHITELABEL.Models.JsonFile.AboutUs> aboutimage = new List<Vastwebmulti.Areas.WHITELABEL.Models.JsonFile.AboutUs>();
                        JSONReadWrite readWrite = new JSONReadWrite();
                        aboutimage = JsonConvert.DeserializeObject<List<Vastwebmulti.Areas.WHITELABEL.Models.JsonFile.AboutUs>>(readWrite.Read("AboutUs.json", "~/Areas/WHITELABEL/Models/JsonFile/"));
                        aboutimage = aboutimage.Where(p => p.UserId == whitelabelid).ToList();

                        //ViewBag.aboutimage = aboutimage.Count == 0 ? "" : aboutimage.SingleOrDefault().AboutImage;
                        ViewBag.aboutheading = aboutimage.Count == 0 ? "" : aboutimage.SingleOrDefault().AboutHeading;
                        ViewBag.aboutcontent = aboutimage.Count == 0 ? "" : aboutimage.SingleOrDefault().AboutContent;

                        //*About Us Content

                        //*Card Content
                        ViewBag.card = DB.tblCards.Where(p => p.UserId == whitelabelid).ToList();
                        var colorcard = DB.tblCardBackColors.Where(p => p.UserId == whitelabelid).ToList();
                        ViewBag.cardcolor = colorcard.Count == 0 ? "" : colorcard.SingleOrDefault().CardBackColor;
                        //*Card Content

                        //*Travels Content
                        var alltravelbus = DB.tblTravels.Where(p => p.UserId == whitelabelid && p.TravelType == "Bus").ToList();
                        ViewBag.busimage = alltravelbus.Count == 0 ? "" : alltravelbus.SingleOrDefault().Image;
                        ViewBag.buscontent = alltravelbus.Count == 0 ? "" : alltravelbus.SingleOrDefault().Content;
                        var alltravelflight = DB.tblTravels.Where(p => p.UserId == whitelabelid && p.TravelType == "Flight").ToList();
                        ViewBag.flightimage = alltravelflight.Count == 0 ? "" : alltravelflight.SingleOrDefault().Image;
                        ViewBag.flightcontent = alltravelflight.Count == 0 ? "" : alltravelflight.SingleOrDefault().Content;
                        var alltravelhotel = DB.tblTravels.Where(p => p.UserId == whitelabelid && p.TravelType == "Hotel").ToList();
                        ViewBag.hotelimage = alltravelhotel.Count == 0 ? "" : alltravelhotel.SingleOrDefault().Image;
                        ViewBag.hotelcontent = alltravelhotel.Count == 0 ? "" : alltravelhotel.SingleOrDefault().Content;
                        var colortravel = DB.tblTravelColors.Where(p => p.UserId == whitelabelid).ToList();
                        ViewBag.travelbackcolor = colortravel.Count == 0 ? "" : colortravel.SingleOrDefault().TravelColor;
                        //*Travels Content
                        //Dth Connection
                        ViewBag.dth = DB.tblDTHs.Where(p => p.UserId == whitelabelid).ToList();
                        var colordth = DB.tblDTHColors.Where(p => p.UserId == whitelabelid).ToList();
                        ViewBag.dthcolor = colordth.Count == 0 ? "" : colordth.SingleOrDefault().DthBackColor;
                        //Dth Connection
                        //New Update Content
                        List<Vastwebmulti.Areas.WHITELABEL.Models.JsonFile.NewsUpdate> NEWS = new List<Vastwebmulti.Areas.WHITELABEL.Models.JsonFile.NewsUpdate>();

                        NEWS = JsonConvert.DeserializeObject<List<Vastwebmulti.Areas.WHITELABEL.Models.JsonFile.NewsUpdate>>(readWrite.Read("NewsUpdate.json", "~/Areas/WHITELABEL/Models/JsonFile/"));
                        ViewBag.newupdate = NEWS.Where(p => p.UserId == whitelabelid).ToList();
                        //New Update Content
                        //Contact Data
                        //var companyname = DB.tblWhiteContactDatas.Where(p => p.UserId == whitelabelid).ToList();
                        List<Vastwebmulti.Areas.WHITELABEL.Models.JsonFile.WhiteContactData> ContactData = new List<Vastwebmulti.Areas.WHITELABEL.Models.JsonFile.WhiteContactData>();
                        ContactData = JsonConvert.DeserializeObject<List<Vastwebmulti.Areas.WHITELABEL.Models.JsonFile.WhiteContactData>>(readWrite.Read("WhiteContactData.json", "~/Areas/WHITELABEL/Models/JsonFile/"));
                        var companyname = ContactData.Where(p => p.UserId == whitelabelid).ToList();
                        ViewBag.companyname = companyname.Count == 0 ? "" : companyname.SingleOrDefault().CompnayName;
                        ViewBag.phone = companyname.Count == 0 ? "" : companyname.SingleOrDefault().phone;
                        ViewBag.landline = companyname.Count == 0 ? "" : companyname.SingleOrDefault().LandlineNo;
                        ViewBag.address = companyname.Count == 0 ? "" : companyname.SingleOrDefault().Address;
                        ViewBag.email = companyname.Count == 0 ? "" : companyname.SingleOrDefault().Email;

                        //********************** Start Social Media ****************************
                        var facelink = DB.tblFooterlinks.Where(p => p.FooterType == "Facebook" && p.UserId == whitelabelid).ToList();
                        ViewBag.fac = facelink.Count == 0 ? "" : facelink.SingleOrDefault().FooterLink;
                        var twitter = DB.tblFooterlinks.Where(p => p.FooterType == "Twitter" && p.UserId == whitelabelid).ToList();
                        ViewBag.twi = twitter.Count == 0 ? "" : twitter.SingleOrDefault().FooterLink;
                        var Instagram = DB.tblFooterlinks.Where(p => p.FooterType == "Instagram" && p.UserId == whitelabelid).ToList();
                        ViewBag.Instagra = Instagram.Count == 0 ? "" : Instagram.SingleOrDefault().FooterLink;
                        var linked = DB.tblFooterlinks.Where(p => p.FooterType == "LinkedIn" && p.UserId == whitelabelid).ToList();
                        ViewBag.linked = linked.Count == 0 ? "" : linked.SingleOrDefault().FooterLink;
                        var skype = DB.tblFooterlinks.Where(p => p.FooterType == "Skype" && p.UserId == whitelabelid).ToList();
                        ViewBag.skyp = skype.Count == 0 ? "" : skype.SingleOrDefault().FooterLink;
                        var Pinterest = DB.tblFooterlinks.Where(p => p.FooterType == "Pinterest" && p.UserId == whitelabelid).ToList();
                        ViewBag.Pinteres = Pinterest.Count == 0 ? "" : Pinterest.SingleOrDefault().FooterLink;
                        var Youtube = DB.tblFooterlinks.Where(p => p.FooterType == "Youtube" && p.UserId == whitelabelid).ToList();
                        ViewBag.Youtub = Youtube.Count == 0 ? "" : Youtube.SingleOrDefault().FooterLink;
                        var Blogger = DB.tblFooterlinks.Where(p => p.FooterType == "Blogger" && p.UserId == whitelabelid).ToList();
                        ViewBag.Blogge = linked.Count == 0 ? "" : linked.SingleOrDefault().FooterLink;
                        var Tumblr = DB.tblFooterlinks.Where(p => p.FooterType == "Tumblr" && p.UserId == whitelabelid).ToList();
                        ViewBag.Tumbl = Tumblr.Count == 0 ? "" : Tumblr.SingleOrDefault().FooterLink;
                        //********************** End Social Media ****************************

                        //********************** Start Badges Media **************************
                        var Appfutura = DB.WhiteLabel_Badges.Where(p => p.BadgeType == "Appfutura" && p.Userid == whitelabelid).ToList();
                        ViewBag.Appfutur = Appfutura.Count == 0 ? "" : Appfutura.SingleOrDefault().Url_Link;
                        var Applancher = DB.WhiteLabel_Badges.Where(p => p.BadgeType == "Applancher" && p.Userid == whitelabelid).ToList();
                        ViewBag.Applanche = Applancher.Count == 0 ? "" : Applancher.SingleOrDefault().Url_Link;
                        var Badush = DB.WhiteLabel_Badges.Where(p => p.BadgeType == "Badush" && p.Userid == whitelabelid).ToList();
                        ViewBag.Badus = Badush.Count == 0 ? "" : Badush.SingleOrDefault().Url_Link;
                        var Extract = DB.WhiteLabel_Badges.Where(p => p.BadgeType == "Extract" && p.Userid == whitelabelid).ToList();
                        ViewBag.Extrac = Extract.Count == 0 ? "" : Extract.SingleOrDefault().Url_Link;
                        var Goodfirms = DB.WhiteLabel_Badges.Where(p => p.BadgeType == "Good-firms" && p.Userid == whitelabelid).ToList();
                        ViewBag.Goodfirm = Goodfirms.Count == 0 ? "" : Goodfirms.SingleOrDefault().Url_Link;
                        var Mobileapp = DB.WhiteLabel_Badges.Where(p => p.BadgeType == "Mobile-app" && p.Userid == whitelabelid).ToList();
                        ViewBag.Mobileap = Mobileapp.Count == 0 ? "" : Mobileapp.SingleOrDefault().Url_Link;
                        var Trustpilot = DB.WhiteLabel_Badges.Where(p => p.BadgeType == "Trustpilot" && p.Userid == whitelabelid).ToList();
                        ViewBag.Trustpilo = Trustpilot.Count == 0 ? "" : Trustpilot.SingleOrDefault().Url_Link;
                        //********************** End Badges Media **************************

                        //Contact Data
                        //Footer Data
                        var footerdata = DB.tblWhiteLabel_FooterServices.Where(p => p.ServiceType == "Recharge & Bill" && p.UserId == whitelabelid).ToList();
                        ViewBag.rech1 = footerdata.Count == 0 ? "" : footerdata.SingleOrDefault().ServiceName1;
                        ViewBag.rech2 = footerdata.Count == 0 ? "" : footerdata.SingleOrDefault().ServiceName2;
                        ViewBag.rech3 = footerdata.Count == 0 ? "" : footerdata.SingleOrDefault().ServiceName3;
                        ViewBag.rech4 = footerdata.Count == 0 ? "" : footerdata.SingleOrDefault().ServiceName4;
                        ViewBag.rech5 = footerdata.Count == 0 ? "" : footerdata.SingleOrDefault().ServiceName5;
                        var traveldata = DB.tblWhiteLabel_FooterServices.Where(p => p.ServiceType == "Travel & Hotel" && p.UserId == whitelabelid).ToList();
                        ViewBag.tra1 = traveldata.Count == 0 ? "" : traveldata.SingleOrDefault().ServiceName1;
                        ViewBag.tra2 = traveldata.Count == 0 ? "" : traveldata.SingleOrDefault().ServiceName2;
                        ViewBag.tra3 = traveldata.Count == 0 ? "" : traveldata.SingleOrDefault().ServiceName3;
                        ViewBag.tra4 = traveldata.Count == 0 ? "" : traveldata.SingleOrDefault().ServiceName4;
                        ViewBag.tra5 = traveldata.Count == 0 ? "" : traveldata.SingleOrDefault().ServiceName5;
                        var Ecommercedata = DB.tblWhiteLabel_FooterServices.Where(p => p.ServiceType == "E-Commerce" && p.UserId == whitelabelid).ToList();
                        ViewBag.Ecommerce1 = Ecommercedata.Count == 0 ? "" : Ecommercedata.SingleOrDefault().ServiceName1;
                        ViewBag.Ecommerce2 = Ecommercedata.Count == 0 ? "" : Ecommercedata.SingleOrDefault().ServiceName2;
                        ViewBag.Ecommerce3 = Ecommercedata.Count == 0 ? "" : Ecommercedata.SingleOrDefault().ServiceName3;
                        ViewBag.Ecommerce4 = Ecommercedata.Count == 0 ? "" : Ecommercedata.SingleOrDefault().ServiceName4;
                        ViewBag.Ecommerce5 = Ecommercedata.Count == 0 ? "" : Ecommercedata.SingleOrDefault().ServiceName5;
                        var Giftdata = DB.tblWhiteLabel_FooterServices.Where(p => p.ServiceType == "Gift Cards" && p.UserId == whitelabelid).ToList();
                        ViewBag.Gift1 = Giftdata.Count == 0 ? "" : Giftdata.SingleOrDefault().ServiceName1;
                        ViewBag.Gift2 = Giftdata.Count == 0 ? "" : Giftdata.SingleOrDefault().ServiceName2;
                        ViewBag.Gift3 = Giftdata.Count == 0 ? "" : Giftdata.SingleOrDefault().ServiceName3;
                        ViewBag.Gift4 = Giftdata.Count == 0 ? "" : Giftdata.SingleOrDefault().ServiceName4;
                        ViewBag.Gift5 = Giftdata.Count == 0 ? "" : Giftdata.SingleOrDefault().ServiceName5;

                        var footercolor = DB.tblfooterbackcolors.Where(p => p.UserId == whitelabelid).ToList();
                        ViewBag.footercolor = footercolor.Count == 0 ? "" : footercolor.SingleOrDefault().FooterBackColor;
                        //Footer Data
                        ViewData["msg"] = "websiteblock";
                        return View();
                    }
                }
                else
                {
                    ViewData["msg"] = "websiteblock";
                    return RedirectToAction("Index1");
                }
            }
        }

        private string GetComputer_InternetIP()
        {
            string ipaddress;
            ipaddress = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (ipaddress == "" || ipaddress == null)
                ipaddress = Request.ServerVariables["REMOTE_ADDR"];
            return ipaddress;
        }
        #region Application Upload
        public ActionResult AppUpload()
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                try
                {
                    string currentip1 = GetComputer_InternetIP();

                    //currentip1 = "103.182.218.156";

                    currentip1 = currentip1.Replace(".", "");
                    long test = Convert.ToInt64(currentip1);

                    if ((test >= 103182218130) && (103182218256 >= test))
                    {
                        ViewBag.error = TempData["error"];
                        ViewBag.success = TempData["success"];
                        TempData.Remove("error");
                        TempData.Remove("success");
                        //admin apk
                        string adminpath = "";
                        var currentwebsitess = db.Admin_details.FirstOrDefault().WebsiteUrl;
                        var pth = Path.Combine(Server.MapPath("~/AdminApk/" + currentwebsitess));
                        ViewBag.admincounter = Directory.GetFiles(Server.MapPath("~/AdminApk/"), "*.apk").Count();
                        if (ViewBag.admincounter > 0)
                        {
                            adminpath = "Upload Successfully.";
                        }
                        else
                        {
                            adminpath = "File Not Found";
                        }
                        ViewBag.adminshowurl = adminpath;

                        //check if directory available
                        if (!System.IO.Directory.Exists(pth))
                        {
                            System.IO.Directory.CreateDirectory(pth);
                        }
                        else
                        {
                            DirectoryInfo d = new DirectoryInfo(pth);//Assuming Test is your Folder
                            FileInfo[] Files = d.GetFiles("*.apk"); //Getting apk files
                            string str = "";
                            foreach (FileInfo file in Files)
                            {
                                str = file.Name;
                            }
                            var kkk = str;

                            if (!string.IsNullOrEmpty(kkk))
                            {
                                ViewBag.apkfilename = kkk;
                            }
                            else
                            {
                                ViewBag.apkfilename = "File Not Found";
                            }
                        }
                        var pthss = Path.Combine(Server.MapPath("~/AdminApk/" + currentwebsitess + "/Vastwebmulti.txt"));
                        ViewBag.Apk_Version = "";
                        var link = db.uploaded_apklink.SingleOrDefault();
                        if (link != null)
                        {
                            if (!string.IsNullOrWhiteSpace(link.Apk_Version))
                            {
                                ViewBag.Apk_Version = link.Apk_Version;
                            }
                        }

                        var getlist = DB.googleconsoles.ToList();
                        ViewBag.getlist = getlist;
                }
                    else
                {
                    return RedirectToAction("Index");
                }
            }
                catch (Exception e)
                {
                    return RedirectToAction("Index");
                }
            }
            return View();
        }

        [HttpPost]
        public ActionResult Uploadadminapk(HttpPostedFileBase Adminapk, string txtuploadedlink1)
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                try
                {
                    if (txtuploadedlink1 != null && txtuploadedlink1.Trim() != "")
                    {
                        var upload = db.uploaded_apklink.ToList();
                        if (upload.Count() == 0)
                        {
                            uploaded_apklink ul = new uploaded_apklink();
                            ul.Apk_Version = txtuploadedlink1;
                            db.uploaded_apklink.Add(ul);
                        }
                        else
                        {
                            var up = upload.SingleOrDefault();
                            up.Apk_Version = txtuploadedlink1;
                        }
                        db.SaveChanges();
                    }
                    ViewBag.Message = "";
                    var currentwebsitess = db.Admin_details.FirstOrDefault().WebsiteUrl;
                    Directory.EnumerateFiles(Server.MapPath("~/AdminApk/" + currentwebsitess + "/"), "*.apk").ToList().ForEach(x => System.IO.File.Delete(x));
                    var pthh = Path.Combine(Server.MapPath("~/AdminApk/" + currentwebsitess + "/"));

                    string extension = System.IO.Path.GetExtension(Request.Files["Adminapk"].FileName).ToLower();
                    string[] dirs = Directory.GetFiles(Server.MapPath("~/AdminApk/"), "*.apk");
                    if (dirs.Length > 0)
                    {
                        TempData["error"] = "Delete Old File to Upload New Apk.";
                    }
                    else
                    {
                        if (Adminapk != null)
                        {
                            if (extension == ".apk" || extension == ".APK")
                            {
                                string fileName = string.Empty;
                                string destinationPath = string.Empty;
                                fileName = Path.GetFileName(Adminapk.FileName);
                                destinationPath = Path.Combine(Server.MapPath("~/AdminApk/" + currentwebsitess + "/"), fileName);
                                Adminapk.SaveAs(destinationPath);
                                TempData["success"] = "File Uploaded Sucessfully.";
                            }
                            else
                            {
                                TempData["error"] = "Please Select Correct Apk File.";
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    return RedirectToAction("AppUpload", "Home");
                }
            }
            return RedirectToAction("AppUpload", "Home");
        }

        [HttpPost]
        public ActionResult AddGoogleconsole(string txtMobile = null, string txtEmail = null)
        {
            try
            {
                var users = DB.Users.Where(a => a.PhoneNumber == txtMobile).SingleOrDefault();
                var chk = DB.googleconsoles.Where(a => a.Mobile == txtMobile).SingleOrDefault();
                if (users != null && chk == null)
                {
                    string currentip1 = GetComputer_InternetIP();
                    googleconsole googlecs = new googleconsole();

                    googlecs.Mobile = txtMobile;
                    googlecs.Email = txtEmail;
                    googlecs.ipaddress = currentip1;
                    googlecs.message = "Developed by Vast Web India Pvt. Ltd.";
                    DB.googleconsoles.Add(googlecs);
                    DB.SaveChanges();
                    TempData["success"] = "Success";
                    return RedirectToAction("AppUpload", "Home");
                }
                else
                {
                    TempData["error"] = "User not found or wrong detail";
                    return RedirectToAction("AppUpload", "Home");
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = "Something went wrong";
                return RedirectToAction("AppUpload", "Home");
            }
        }

        #endregion

        public ActionResult termsconditions()
        {
            return View();
        }
        public ActionResult privacy()
        {
            return View();
        }

        public ActionResult Help()
        {
            return View();
        }

        public void Passcode(string ids)
        {

            if (ids != null)
            {
                var details = DB.Users.Where(s => s.Email == ids).SingleOrDefault();

                if (details == null)
                {
                    details = DB.Users.Where(s => s.PhoneNumber == ids).SingleOrDefault();

                }



                if (details != null)
                {


                    using (VastwebmultiEntities db = new VastwebmultiEntities())
                    {
                        Random nm = new Random();
                        var pin = nm.Next(1000, 10000);
                        var apiurls = "";

                        var smsapi2 = db.apisms.Where(x => x.sts == "Y").ToList();
                        var smsapionsts2 = smsapi2.Where(s => s.api_type == "whatsapp").SingleOrDefault();
                        if (smsapionsts2 != null)
                        {
                            var setopt = db.Users.Where(s => s.Email == ids).SingleOrDefault();
                            if (setopt == null)
                            {
                                setopt = db.Users.Where(s => s.PhoneNumber == ids).SingleOrDefault();
                            }
                            setopt.forgetpin = Convert.ToString(pin);
                            db.SaveChanges();


                            apiurls = smsapionsts2.smsapi;
                            string text = "Your Forget Password otp is " + pin;
                            text = string.Format(text, "1230");

                            var forgetotp = db.Email_show_passcode.SingleOrDefault();

                            if (forgetotp?.forgetotp == true)
                            {
                                smssend.sms_init("Y", "Y", "ForgetPassOtp", details.PhoneNumber, text);
                            }





                            var apinamechange = apiurls.Replace("tttt", details.PhoneNumber).Replace("mmmm", text);

                            var client = new RestClient(apinamechange);
                            var request = new RestRequest(Method.GET);

                            VastBazaartoken Responsetoken = new VastBazaartoken();
                            var whatsts = db.Email_show_passcode.SingleOrDefault();
                            if (apinamechange.ToUpper().Contains("API.VASTBAZAAR.COM/API/WEB/WHATSAPPMSG") && whatsts.whatsappapists == true)
                            {
                                var token = Responsetoken.gettoken();
                                request.AddHeader("authorization", "bearer " + token);
                                request.AddHeader("content-type", "application/json");
                            }
                            var task = Task.Run(() =>
                            {
                                return client.Execute(request).Content;
                            });
                            bool isCompletedSuccessfully = task.Wait(TimeSpan.FromSeconds(10000));
                            var resp = "";
                            if (isCompletedSuccessfully == true)
                            {
                                resp = task.Result;
                            }

                            sms_api_entry sms = new sms_api_entry();
                            sms.apiname = apinamechange;
                            sms.msg = text;
                            sms.m_date = System.DateTime.Now;
                            sms.response = resp;
                            sms.messagefor = details.UserId;
                            db.sms_api_entry.Add(sms);
                            db.SaveChanges();

                        }
                        var AdminDetails = db.Admin_details.SingleOrDefault();


                        smssend.SendEmailAll(details.Email, "Your Forget Password otp is " + pin, "Forget Password OTP", AdminDetails.email);





                    }
                }
            }
        }

        #region Aeps Demo
        public ActionResult AepsVideo()
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                try
                {
                    var currentwebsitess = db.Admin_details.FirstOrDefault().WebsiteUrl;
                    var relativepath = "~/AepsDemo/" + currentwebsitess + "_video.mp4";
                    var pthh = Server.MapPath(relativepath);
                    if (System.IO.File.Exists(pthh))
                    {
                        ViewBag.adminshowurl = currentwebsitess + "_video.mp4";
                    }
                    else
                    {
                        ViewBag.adminshowurl = null;
                    }
                }
                catch (Exception e)
                {
                    return View();
                }

            }
            return View();
        }
        [HttpPost]
        public ActionResult UploadAepsVideo(HttpPostedFileBase AepsDemo)
        {
            try
            {
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    var currentwebsitess = db.Admin_details.FirstOrDefault().WebsiteUrl;
                    var pthh = Path.Combine(Server.MapPath("~/AepsDemo"));
                    string extension = System.IO.Path.GetExtension(Request.Files["AepsDemo"].FileName).ToLower();
                    if (extension == ".mp4" || extension == ".MP4")
                    {
                        if (!System.IO.Directory.Exists(pthh))
                        {
                            System.IO.Directory.CreateDirectory(pthh);
                        }
                        pthh = Server.MapPath("~/AepsDemo/" + currentwebsitess + "_video.mp4");
                        if (System.IO.File.Exists(pthh))
                        {
                            return RedirectToAction("AepsVideo", "Home");
                        }
                        else
                        {
                            if (AepsDemo != null)
                            {
                                AepsDemo.SaveAs(pthh);
                            }
                        }
                    }

                }
            }
            catch (Exception e)
            {
                return RedirectToAction("AepsVideo", "Home");
            }
            return RedirectToAction("AepsVideo", "Home");
        }
        #endregion


        public ActionResult Login(string id)
        {
            var userid = User.Identity.GetUserId();
            ViewBag.signin_signup = id;

            try
            {
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    var chk1 = db.Email_show_passcode.Select(a => a.forgetotp).FirstOrDefault();

                    Session["chk1"] = chk1 == true ? "1" : "";

                    if (User.Identity.IsAuthenticated)
                    {
                        if (User.IsInRole("Admin"))
                            return RedirectToAction("Dashboard", "Home", new { area = "ADMIN" });

                        
                        if (User.IsInRole("master"))
                        {
                            var data = db.Superstokist_details.FirstOrDefault(a => a.SSId == userid);

                            if (data != null && data.Status == "Y")
                                return RedirectToAction("Dashboard", "Home", new { area = "master" });

                            TempData.Remove("data");
                            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                            TempData["userblocked"] = "YOUR MASTER ID IS BLOCKED. PLEASE CONTACT TO ADMIN";
                            return RedirectToAction("Login", "Home");
                        }

                        
                        if (User.IsInRole("Dealer"))
                        {
                            var data = db.Dealer_Details.FirstOrDefault(a => a.DealerId == userid);

                            if (data != null && data.Status == "Y")
                                return RedirectToAction("Dashboard", "Home", new { area = "Dealer" });

                            TempData.Remove("data");
                            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                            TempData["userblocked"] = "Your Account is Currently Blocked with Distributor, Contact to Administrator.";
                            return RedirectToAction("Login", "Home");
                        }

                        
                        if (User.IsInRole("Retailer"))
                        {
                            var data = db.Retailer_Details.FirstOrDefault(a => a.RetailerId == userid);

                            if (data != null && data.Status == "Y")
                                return RedirectToAction("Dashboard", "Home", new { area = "Retailer" });

                            TempData.Remove("data");
                            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                            TempData["userblocked"] = "Your Account is Currently Blocked, Contact to Administrator.";
                            return RedirectToAction("Login", "Home");
                        }

                        
                        if (User.IsInRole("API"))
                        {
                            var data = db.api_user_details.FirstOrDefault(a => a.apiid == userid);

                            if (data != null && data.status == "Y")
                                return RedirectToAction("Dashboard", "Home", new { area = "API" });

                            TempData.Remove("data");
                            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                            TempData["userblocked"] = "YOUR API ID IS BLOCKED. PLEASE CONTACT TO ADMIN";
                            return RedirectToAction("Login", "Home");
                        }

                     
                        if (User.IsInRole("RCH"))
                            return RedirectToAction("Index", "Home", new { area = "RCH" });

                        
                        if (User.IsInRole("Vendor"))
                        {
                            var data = db.Vendor_details.FirstOrDefault(a => a.userid == userid);

                            if (data != null && data.status == true)
                                return RedirectToAction("Index", "Home", new { area = "VENDOR" });

                            TempData.Remove("data");
                            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                            TempData["userblocked"] = "YOUR VENDOR ID IS BLOCKED. PLEASE CONTACT TO ADMIN";
                            return RedirectToAction("Login", "Home");
                        }

                       
                        if (User.IsInRole("FeeCollector"))
                        {
                            var data = db.FeeCollector_details.FirstOrDefault(a => a.FCId == userid);

                            if (data != null && data.Status == "Y")
                                return RedirectToAction("Index", "Home", new { area = "FeeCollector" });

                            TempData.Remove("data");
                            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                            TempData["userblocked"] = "YOUR FEECOLLECTOR ID IS BLOCKED. PLEASE CONTACT TO ADMIN";
                            return RedirectToAction("Login", "Home");
                        }

                    
                        if (User.IsInRole("Employee"))
                            return RedirectToAction("Dashboard", "Home", new { area = "Employee" });
                    }
                }
            }
            catch (Exception ex)
            {
                
            }

            var admin = DB.Admin_details.FirstOrDefault();
            var x = admin != null && admin.RenivalDate >= DateTime.Now;

            ViewBag.messagechkk = TempData["newmessage"];
            TempData.Remove("newmessage");

            ViewBag.Sate = DB.State_Desc.Select(a => new SelectListItem
            {
                Text = a.State_name,
                Value = a.State_id.ToString()
            }).ToList();

            if (x)
            {
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
                ViewData["msg"] = "websiteblock";
            }

            var lg = DB.tblWhiteLabelLoginBackImages
                       .FirstOrDefault(a => a.Role == "ADMIN" && a.StatusCheck == "Y");

            if (lg != null)
                ViewBag.showimg = lg.otherimage;

            var loginSlider = DB.LoginSilders.Where(a => a.Status == "Y").ToList();

            if (loginSlider.Count > 0)
            {
                ViewBag.Loginsilder = loginSlider;
            }
            else
            {
                string currenturl = HttpContext.Request.Url.Authority;
                currenturl = currenturl.Replace("www.", "").Replace("https://", "").Replace("http://", "");

                var adminData = HttpContext.Request.IsLocal
                    ? DB.Admin_details.FirstOrDefault(aa => aa.localhost.ToUpper() == currenturl.ToUpper())
                    : DB.Admin_details.FirstOrDefault(aa => aa.WebsiteUrl.ToUpper() == currenturl.ToUpper());

                string AdminId = adminData?.userid;

                var logo = DB.tblLoginContents.FirstOrDefault(a => a.UserId == AdminId);
                ViewBag.Logo = logo?.Image;
            }

            string captcha = GenerateCaptchaCode(6);
            Session["CaptchaCode"] = captcha;
            ViewBag.CaptchaCode = captcha;

            return View(new RegisterViewModel());
        }

        [HttpGet]
        public ActionResult RefreshCaptcha()
        {
            string captcha = GenerateAndSetCaptcha();
            return Json(new { Captcha = captcha }, JsonRequestBehavior.AllowGet);
        }



        // ✅ Helper: Generate Captcha & Save in Session


        [HttpGet]
        private string GenerateAndSetCaptcha()
        {
            string captcha = GenerateCaptchaCode(6);
            Session["CaptchaCode"] = captcha;
            ViewBag.CaptchaCode = captcha;
            return captcha;
        }


        private string GenerateCaptchaCode(int length)
        {
            const string chars = "!@#$%^&*()?abcdefghijklmnopqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            Random random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }


        [HttpPost]
        public JsonResult VerifyCaptcha(string inputCaptcha)
        {
            string sessionCaptcha = Session["CaptchaCode"]?.ToString();
            if (string.IsNullOrEmpty(sessionCaptcha))
                return Json(new { success = false, message = "Captcha expired. Please refresh." });

            if (sessionCaptcha.Equals(inputCaptcha, StringComparison.OrdinalIgnoreCase))
                return Json(new { success = true, message = "Captcha verified successfully." });

            return Json(new { success = false, message = "Invalid captcha. Please try again." });
        }



        public ActionResult fetchlatlong(string lat, string lon)
        {
            var sts = false; var resp = "";
            var keyinfo = DB.Location_key.SingleOrDefault();
            var key = "60baa3a9b6154ef3a3ab635c8bd06767";
            if (keyinfo != null)
            {
                key = keyinfo.APIKEY;
            }
            var check = DB.latlongstores.Where(aa => aa.latitude == lat && aa.longitude == lon).SingleOrDefault();
            if (check != null)
            {
                sts = true;
                resp = check.locations;
            }
            var respp = new
            {
                sts = sts,
                resp = resp,
                key = key
            };


            return Json(respp, JsonRequestBehavior.AllowGet);
        }
        public ActionResult IsMobileEmailExist(string entry, string type)
        {
            try
            {
                VastwebmultiEntities db = new VastwebmultiEntities();
                var isExist = false;
                if (type == "mobile")
                {
                    isExist = db.Users.Any(s => s.PhoneNumber == entry);
                }
                else
                {
                    isExist = db.Users.Any(s => s.Email == entry);
                }
                if (!isExist)
                {
                    int lengthOfPassword = 8;
                    string otp = PasswordGenerator.GeneratePassword(false, false, true, false, false, 4);
                    string pass = PasswordGenerator.GeneratePassword(true, true, true, false, false, lengthOfPassword);

                    while (!PasswordGenerator.PasswordIsValid(true, true, true, false, false, pass))
                    {
                        pass = PasswordGenerator.GeneratePassword(true, true, true, false, false, lengthOfPassword);
                    }

                    var otp_table = new MobileOtp();
                    otp_table.Date = DateTime.Now;
                    otp_table.mobileno = entry;
                    otp_table.Userid = type + "_" + pass;
                    otp_table.Otp = otp;
                    otp_table.Type = type;
                    db.MobileOtps.Add(otp_table);
                    db.SaveChanges();

                    if (type == "mobile")
                    {
                        smssend.sms_init("Y", "Y", "VERYFYPROFILESUSERSOTP", entry, otp);
                    }
                    else
                    {
                        CommUtilEmail emailsend = new CommUtilEmail();
                        var ToCC = db.Admin_details.FirstOrDefault().email;
                        emailsend.EmailLimitChk(entry, ToCC, " Email id Veryfication", " OTP for  Veryfy Email ID " + otp, "No CallBackUrl");
                    }

                    var res = new
                    {
                        status = true,
                        entry,
                        client_id = type + "_" + pass,
                        isExist = isExist
                    };

                    return Json(res, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var res = new
                    {
                        status = false,
                        entry,
                        message = type == "mobile" ? "Mobile Number Already Exist" : "Email Already Exist"
                    };

                    return Json(res, JsonRequestBehavior.AllowGet);
                }
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var eve in ex.EntityValidationErrors)
                {
                    Verification_Log(String.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State));
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Verification_Log(String.Format("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage));
                    }
                }

                var res = new
                {
                    status = false,
                    entry,
                    message = ex.Message
                };

                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Verification_Log("Msg: " + ex.Message);
                Verification_Log("Stack: " + ex.StackTrace);
                Verification_Log("inner: " + ex.InnerException?.Message);
                Verification_Log("inner Stack: " + ex.InnerException?.StackTrace);

                var res = new
                {
                    status = false,
                    entry,
                    message = ex.Message
                };

                return Json(res, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult OtpVerify(string type, string clientId, string otp)
        {
            VastwebmultiEntities db = new VastwebmultiEntities();
            var entry = db.MobileOtps.Where(s => s.Userid == clientId).FirstOrDefault();
            if (entry == null)
            {
                var obj = new
                {
                    status = false,
                    message = "Invalid Otp"
                };
                return Json(obj, JsonRequestBehavior.AllowGet);
            }
            else
            {

                if (entry.Otp == otp && entry.Type == type)
                {
                    var obj = new
                    {
                        status = true,
                        message = "Otp Matched",
                        id = entry.idno,
                        type
                    };

                    return Json(obj, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var obj = new
                    {
                        status = false,
                        message = "Invalid Otp"
                    };

                    return Json(obj, JsonRequestBehavior.AllowGet);
                }
            }
        }

        public JObject fetchEmail_Mobile_sts_Data()
        {
            VastwebmultiEntities db = new VastwebmultiEntities();
            var verify_data = db.Email_Mobile_Verify.Single();
            JObject obj = new JObject();

            obj.Add("type", verify_data.emailmobile_verified.ToUpper());

            return obj;
        }

        public static void Verification_Log(string strMessage)
        {
            try
            {
                StreamWriter log;
                FileStream fileStream = null;
                DirectoryInfo logDirInfo = null;
                FileInfo logFileInfo;
                string logFilePath = "C:\\Logs\\";
                logFilePath = logFilePath + "Verification_Log -" + System.DateTime.Today.ToString("MM-dd-yyyy") + "." + "txt";
                logFileInfo = new FileInfo(logFilePath);
                logDirInfo = new DirectoryInfo(logFileInfo.DirectoryName);
                if (!logDirInfo.Exists) logDirInfo.Create();
                if (!logFileInfo.Exists)
                {
                    fileStream = logFileInfo.Create();
                }
                else
                {
                    fileStream = new FileStream(logFilePath, FileMode.Append);
                }
                log = new StreamWriter(fileStream);
                log.WriteLine(strMessage);
                log.Close();
            }
            catch (Exception ex)
            {
            }
        }



        //fill district according to state
        public JsonResult FillDistict(int State)
        {
            var data = new SelectList(DB.show_district(State), "Dist_id", "District_name", null).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        //fill dealer list according to nearest district
        public JsonResult FillDealerList(int District)
        {
            var show = new SelectList(DB.select_Outer_Admin_Dealer_for_ddl(District), "Dealerid", "DealerName", null).ToList();
            return Json(show, JsonRequestBehavior.AllowGet);
        }
        public JsonResult FillRegiDistict(int Id)
        {
            var cities = DB.District_Desc.Where(c => c.State_id == Id);
            return Json(cities, JsonRequestBehavior.AllowGet);
        }
        public ActionResult NewLogin()
        {

            var x = DB.Admin_details.SingleOrDefault().RenivalDate >= DateTime.Now;
            ViewBag.msgchhhk = TempData["newmessage"];
            TempData.Remove("newmessage");
            if (x)
            {
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
                else if (User.IsInRole("Whitelabelretailer"))
                {
                    return RedirectToAction("Dashboard", "Home", new { area = "WRetailer" });
                }
                var whitelabelid = "";
                VastwebmultiEntities db = new VastwebmultiEntities();
                string currenturl = HttpContext.Request.Url.Authority;
                currenturl = currenturl.Replace("www.", "").Replace("https://", "").Replace("http://", "");
                currenturl = currenturl.ToUpper();
                if (currenturl.Contains("LOCALHOST"))
                {
                    whitelabelid = db.WhiteLabel_userList.Where(aa => aa.localhost.Replace("www.", "").Replace("https://", "").Replace("http://", "").ToLower() == currenturl.ToLower()).SingleOrDefault().WhiteLabelID;
                }
                else
                {
                    whitelabelid = db.WhiteLabel_userList.Where(aa => aa.websitename.Replace("www.", "").Replace("https://", "").Replace("http://", "").ToLower() == currenturl.ToLower()).SingleOrDefault().WhiteLabelID;
                }
                //var whitelabelid = db.WhiteLabel_userList.Where(aa => aa.websitename.Replace("www.", "").Replace("https://", "").Replace("http://", "").ToLower() == currenturl.ToLower()).SingleOrDefault().WhiteLabelID;
                ViewBag.Sate = DB.State_Desc.Select(a => new SelectListItem { Text = a.State_name, Value = a.State_id.ToString() }).ToList();
                var compy = "";
                var company = db.tblWhiteContactDatas.Where(p => p.UserId == whitelabelid).ToList();
                if (company.Count > 0)
                {
                    compy = company.SingleOrDefault().CompnayName;
                    ViewBag.company = compy;
                }
                else
                {
                    compy = "";
                    ViewBag.company = compy;
                }
                var SingleImage = db.WhiteLabelLoginBackSingleImages.Where(p => p.UserID == whitelabelid && p.StatusCheck == "Y").ToList();
                var RepeatImages = db.WhiteLabelLoginBackRepeatImages.Where(p => p.UserID == whitelabelid && p.StatusCheck == "Y").ToList();
                if (SingleImage.Count > 0)
                {
                    ViewBag.fullpath = ".." + SingleImage.SingleOrDefault().otherimage.Replace("\\", "/");
                }
                else if (RepeatImages.Count > 0)
                {
                    ViewBag.fullpath = ".." + RepeatImages.SingleOrDefault().otherimage.Replace("\\", "/");
                }
                else
                {
                    ViewBag.color = "linear-gradient(to right , var(--main-bg-lcolor), var(--main-bg-rcolor)) !important";
                }
                var logoimage = db.tblHeaderLogoes.Where(p => p.UserId == whitelabelid).ToList();
                if (logoimage.Count > 0)
                {
                    ViewBag.logo = logoimage.SingleOrDefault().LogoImage;
                }
                else
                {
                    ViewBag.logo = "Login_Images/login22.png";
                }
                ViewData["success"] = TempData["aa"];
                ViewData["aaaa"] = TempData["msg"];
                ViewData["bbbb"] = TempData["Confrim"];
                ViewData["error"] = TempData["error"];
                ViewData["slaberror"] = TempData["slaberror"];
                ViewData["existuser"] = TempData["errorretailer"];
                return View();
            }
            else
            {
                var whitelabelid = "";
                VastwebmultiEntities db = new VastwebmultiEntities();
                string currenturl = HttpContext.Request.Url.Authority;
                currenturl = currenturl.Replace("www.", "").Replace("https://", "").Replace("http://", "");
                currenturl = currenturl.ToUpper();
                if (currenturl.Contains("LOCALHOST"))
                {
                    whitelabelid = db.WhiteLabel_userList.Where(aa => aa.localhost.Replace("www.", "").Replace("https://", "").Replace("http://", "").ToLower() == currenturl.ToLower()).SingleOrDefault().WhiteLabelID;
                }
                else
                {
                    whitelabelid = db.WhiteLabel_userList.Where(aa => aa.websitename.Replace("www.", "").Replace("https://", "").Replace("http://", "").ToLower() == currenturl.ToLower()).SingleOrDefault().WhiteLabelID;
                }
                //var whitelabelid = db.WhiteLabel_userList.Where(aa => aa.websitename.Replace("www.", "").Replace("https://", "").Replace("http://", "").ToLower() == currenturl.ToLower()).SingleOrDefault().WhiteLabelID;
                ViewBag.Sate = DB.State_Desc.Select(a => new SelectListItem { Text = a.State_name, Value = a.State_id.ToString() }).ToList();
                var compy = "";
                var company = db.tblWhiteContactDatas.Where(p => p.UserId == whitelabelid).ToList();
                if (company.Count > 0)
                {
                    compy = company.SingleOrDefault().CompnayName;
                    ViewBag.company = compy;
                }
                else
                {
                    compy = "";
                    ViewBag.company = compy;
                }
                var image1 = "";
                var image = db.tblWhiteLabelLoginBackImages.Where(p => p.UserID == whitelabelid).ToList();
                if (image.Count > 0)
                {
                    image1 = image.FirstOrDefault().ImageUrl;
                }
                else
                {
                    image1 = "img38.png";
                }
                ViewBag.fullpath = "../FooterImages/" + image1;
                var logoimage = db.tblHeaderLogoes.Where(p => p.UserId == whitelabelid).ToList();
                ViewBag.logo = logoimage.Count == 0 ? "" : logoimage.SingleOrDefault().LogoImage;
                ViewData["success"] = TempData["aa"];
                ViewData["aaaa"] = TempData["msg"];
                ViewData["bbbb"] = TempData["Confrim"];
                ViewData["error"] = TempData["error"];
                ViewData["slaberror"] = TempData["slaberror"];
                ViewData["existuser"] = TempData["errorretailer"];
                ViewData["msg"] = "websiteblock";
            }
            return View();
        }

        //fill whitelabel district according to state
        public JsonResult whitelabelFillDistict(int State)
        {
            var data = new SelectList(DB.show_whitelable_district(State), "Dist_id", "District_name", null).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        //fill whitelabel dealer list according to nearest district
        public JsonResult whitelabelFillDealerList(int District)
        {
            var show = new SelectList(DB.select_whitelabel_Dealer_for_ddl(District), "Dealerid", "DealerName", null).ToList();
            return Json(show, JsonRequestBehavior.AllowGet);
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult cal()
        {
            ViewBag.Message = "Your application cal.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        [HttpPost]
        public ActionResult AllContect_Data(MasterDistributerModel model)
        {
            tblFrontContect front = new tblFrontContect();
            front.Email = model.Email;
            front.Message = model.Address;
            front.MobileNo = model.Mb;
            front.Name = model.SuperstokistName;
            front.Subject = model.SuperstokistName;
            DB.tblFrontContects.Add(front);
            DB.SaveChanges();
            TempData["front"] = "Contect Us Data Uploaded Successfully";
            return RedirectToAction("Index1");
        }
        [HttpPost]
        public ActionResult AllWhiteContect_Data(MasterDistributerModel model)
        {
            var whitelabelid = "";
            string currenturl = HttpContext.Request.Url.Authority;
            currenturl = currenturl.Replace("www.", "").Replace("https://", "").Replace("http://", "");
            currenturl = currenturl.ToUpper();
            if (currenturl.Contains("LOCALHOST"))
            {
                whitelabelid = DB.WhiteLabel_userList.Where(aa => aa.localhost.Replace("www.", "").Replace("https://", "").Replace("http://", "").ToLower() == currenturl.ToLower()).SingleOrDefault().WhiteLabelID;
            }
            else
            {
                whitelabelid = DB.WhiteLabel_userList.Where(aa => aa.websitename.Replace("www.", "").Replace("https://", "").Replace("http://", "").ToLower() == currenturl.ToLower()).SingleOrDefault().WhiteLabelID;
            }
            //var whitelabelid = DB.WhiteLabel_userList.Where(aa => aa.websitename.Replace("www.", "").Replace("https://", "").Replace("http://", "").ToLower() == currenturl.ToLower()).SingleOrDefault().WhiteLabelID;
            tblWhiteFront_ContactData Contect = new tblWhiteFront_ContactData();
            Contect.Email = model.Email;
            Contect.UserId = whitelabelid;
            Contect.Message = model.Address;
            Contect.MobileNo = model.Mb;
            Contect.Name = model.SuperstokistName;
            Contect.Subject = model.SuperstokistName;
            DB.tblWhiteFront_ContactData.Add(Contect);
            DB.SaveChanges();
            TempData["front"] = "Contact Us Data Uploaded Successfully";
            return RedirectToAction("Index");
        }

        [Route("Home/totalbusiness")]
        public string totalbusiness()
        {
            var total1 = DB.totalbusiness().SingleOrDefault().total.ToString();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var dict = new Dictionary<string, string>
              {
              {"total",total1}
             };
            Response.Clear();
            Response.ContentType = "application/json";
            return serializer.Serialize(dict);
        }
        [Route("Home/TokenReplace")]
        public string tokenReplace()
        {
            var chk = DB.vastbazzartokens.SingleOrDefault();
            DB.vastbazzartokens.Remove(chk);
            DB.SaveChanges();
            return "";
        }
        [Route("Home/RemoveAepsMerchant")]
        public string RemoveAepsMerchant(string merchantid)
        {
            try
            {
                var merchatinfo = DB.Retailer_Details.Where(aa => aa.AepsMerchandId.ToUpper() == merchantid.ToUpper()).SingleOrDefault();
                if (merchatinfo != null)
                {
                    var ekycchkss = DB.ekycChecks.Where(x => x.mobilenumber == merchatinfo.Mobile && x.isvalid == false).FirstOrDefault();
                    if (ekycchkss != null)
                    {
                        try
                        {
                            DB.ekycChecks.Remove(ekycchkss);
                            DB.SaveChanges();
                        }
                        catch { }
                        //    merchatinfo.AepsMerchandId = "";
                        //  merchatinfo.AepsMPIN = "";
                        DB.SaveChanges();
                    }

                }
                else
                {
                    var whitelabel_merchatinfo = DB.Whitelabel_Retailer_Details.Where(aa => aa.AepsMerchandId.ToUpper() == merchantid.ToUpper()).SingleOrDefault();
                    if (whitelabel_merchatinfo != null)
                    {
                        var whitelabel_chkekyc = DB.whitelabel_ekycCheck.Where(x => x.mobilenumber == whitelabel_merchatinfo.Mobile && x.isvalid == false).FirstOrDefault();
                        if (whitelabel_chkekyc != null)
                        {
                            try
                            {
                                DB.whitelabel_ekycCheck.Remove(whitelabel_chkekyc);
                                DB.SaveChanges();
                            }
                            catch { }
                            whitelabel_merchatinfo.AepsMerchandId = "";
                            whitelabel_merchatinfo.AepsMPIN = "";
                            DB.SaveChanges();
                        }

                    }
                }


                JavaScriptSerializer serializer = new JavaScriptSerializer();
                var dict = new Dictionary<string, string>
                {
                   {"resp","DONE"}
                };
                Response.Clear();
                Response.ContentType = "application/json";
                return serializer.Serialize(dict);
            }
            catch
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                var dict = new Dictionary<string, string>
                {
                  {"resp","ERROR"}
                };
                Response.Clear();
                Response.ContentType = "application/json";
                return serializer.Serialize(dict);
            }
        }
        [Route("Home/RemoveAepsMerchantALL")]
        public string RemoveAepsMerchantALL(string merchantid)
        {
            try
            {
                var merchatinfo = DB.Retailer_Details.Where(aa => aa.AepsMerchandId.ToUpper() == merchantid.ToUpper()).SingleOrDefault();
                if (merchatinfo != null)
                {
                    var ekycchkss = DB.ekycChecks.Where(x => x.mobilenumber == merchatinfo.Mobile).FirstOrDefault();
                    if (ekycchkss != null)
                    {
                        try
                        {
                            DB.ekycChecks.Remove(ekycchkss);
                            DB.SaveChanges();
                        }
                        catch { }
                        // merchatinfo.AepsMerchandId = "";
                        // merchatinfo.AepsMPIN = "";
                        DB.SaveChanges();
                    }

                }
                else
                {
                    var whitelabel_merchatinfo = DB.Whitelabel_Retailer_Details.Where(aa => aa.AepsMerchandId.ToUpper() == merchantid.ToUpper()).SingleOrDefault();
                    if (whitelabel_merchatinfo != null)
                    {
                        var whitelabel_chkekyc = DB.whitelabel_ekycCheck.Where(x => x.mobilenumber == whitelabel_merchatinfo.Mobile && x.isvalid == false).FirstOrDefault();
                        if (whitelabel_chkekyc != null)
                        {
                            try
                            {
                                DB.whitelabel_ekycCheck.Remove(whitelabel_chkekyc);
                                DB.SaveChanges();
                            }
                            catch { }
                            whitelabel_merchatinfo.AepsMerchandId = "";
                            whitelabel_merchatinfo.AepsMPIN = "";
                            DB.SaveChanges();
                        }

                    }
                }


                JavaScriptSerializer serializer = new JavaScriptSerializer();
                var dict = new Dictionary<string, string>
                {
                   {"resp","DONE"}
                };
                Response.Clear();
                Response.ContentType = "application/json";
                return serializer.Serialize(dict);
            }
            catch
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                var dict = new Dictionary<string, string>
                {
                  {"resp","ERROR"}
                };
                Response.Clear();
                Response.ContentType = "application/json";
                return serializer.Serialize(dict);
            }
        }


        [Route("Home/ResetAepsMerchant")]
        public string ResetAepsMerchant(string oldmerchantid, string newmerchantid)
        {
            try
            {
                var merchatinfo = DB.Retailer_Details.Where(aa => aa.AepsMerchandId.ToUpper() == oldmerchantid.ToUpper()).SingleOrDefault();
                merchatinfo.AepsMerchandId = newmerchantid;
                DB.SaveChanges();
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                var dict = new Dictionary<string, string>
              {
              {"resp","DONE"}
             };
                Response.Clear();
                Response.ContentType = "application/json";
                return serializer.Serialize(dict);
            }
            catch
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                var dict = new Dictionary<string, string>
              {
              {"resp","ERROR"}
             };
                Response.Clear();
                Response.ContentType = "application/json";
                return serializer.Serialize(dict);
            }
        }
        public ActionResult about_us()
        {
            return View();
        }
        public ActionResult recharge_api()
        {
            return View();
        }
        public ActionResult travel_api()
        {
            return View();
        }
        public ActionResult domestic_transfer()
        {
            return View();
        }
        public ActionResult bbps()
        {
            return View();
        }
        public ActionResult payout_api()
        {
            return View();
        }
        public ActionResult pancard_api()
        {
            return View();
        }
        public ActionResult aeps()
        {
            return View();
        }
        public ActionResult m_pos()
        {
            return View();
        }
        public ActionResult matm()
        {
            return View();
        }
        public ActionResult indo_nepal()
        {
            return View();
        }
        public ActionResult prepaid_card()
        {
            return View();
        }
        public ActionResult pan_card_UTI()
        {
            return View();
        }
        public ActionResult software_development()
        {
            return View();
        }
        public ActionResult mlm_software()
        {
            return View();
        }
        public ActionResult recharge_application()
        {
            return View();
        }
        public ActionResult digital_maketing()
        {
            return View();
        }
        public ActionResult blockchain()
        {
            return View();
        }
        public ActionResult vas()
        {
            return View();
        }
        public ActionResult bulk_sms()
        {
            return View();
        }
        public ActionResult api_services()
        {
            return View();
        }
        public ActionResult allblog()
        {
            return View();
        }
        public ActionResult RefundCancellation()
        {
            return View();
        }
        public ActionResult blog_detail(int id)
        {
            ViewBag.Showboxid = id;
            return View();
        }




        public ActionResult DownloadAPK()
        {
            try
            {
                var currentwebsitess = DB.Admin_details.FirstOrDefault().WebsiteUrl;
                string[] filesInDirectory = Directory.GetFiles(Server.MapPath("~/AdminApk/" + currentwebsitess), "*.apk");
                if (filesInDirectory.Count() == 0)
                {
                    return RedirectToAction("Index1");
                }
                return File(filesInDirectory[0], "application/force-download", Path.GetFileName(filesInDirectory[0]));
            }
            catch (Exception)
            {
                return RedirectToAction("Index1");
            }
        }
        public ActionResult WhitelabelDownloadAPK()
        {
            string[] filesInDirectory = Directory.GetFiles(Server.MapPath("~/WhitelabelApk"), "*.apk");
            if (filesInDirectory.Count() == 0)
            {
                return RedirectToAction("Index");
            }
            return File(filesInDirectory[0], "application/force-download", Path.GetFileName(filesInDirectory[0]));
        }
        [HttpPost]
        public ActionResult Officeaddress()
        {
            string address = ""; string StateName = ""; string DistirictName = ""; string Firmname = "";
            try
            {
                var client = new RestClient("http://api.vastbazaar.com/api/Web/userinfo");
                var request = new RestRequest(Method.POST);
                var token = getAuthToken();
                request.AddHeader("Authorization", "Bearer " + token);
                IRestResponse response = client.Execute(request);
                var resp = response.Content;
                dynamic respchk = JsonConvert.DeserializeObject(resp);
                address = respchk.Content.ADDINFO[0].Address;
                StateName = respchk.Content.ADDINFO[0].StateName;
                DistirictName = respchk.Content.ADDINFO[0].DistirictName;
                Firmname = respchk.Content.ADDINFO[0].FirmName;
            }
            catch
            { }
            var resp1 = new
            {
                address = address,
                StateName = StateName,
                DistirictName = DistirictName,
                Firmname = Firmname

            };
            return Json(resp1, JsonRequestBehavior.AllowGet);
        }
        public IRestResponse tokencheck()
        {
            var apidetails = DB.Money_API_URLS.Where(aa => aa.API_Name == "VASTWEB").SingleOrDefault();
            var token = apidetails == null ? "" : apidetails.Token;
            var apiid = apidetails == null ? "" : apidetails.API_ID;
            var apiidpwd = apidetails == null ? "" : apidetails.Api_pwd;
            var client = new RestClient("http://api.vastbazaar.com/token");
            var request = new RestRequest(Method.POST);
            request.AddHeader("iptoken", token);
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("application/x-www-form-urlencoded", "UserName=" + apiid + "&Password=" + apiidpwd + "&grant_type=password", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            return response;
        }
        public ActionResult Clientinfo()
        {
            var reminfo = DB.reminfo().ToList();
            var list = JsonConvert.SerializeObject(reminfo,
    Formatting.None,
    new JsonSerializerSettings()
    {
        ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
    });
            return Content(list, "application/json");
        }
        public ActionResult ChangeSts(string delimg, string aadharsts, string pansts, string selfiests, string email, string vaststs)
        {
            var remdetails = DB.Retailer_Details.Where(aa => aa.Email.ToUpper() == email.ToUpper()).SingleOrDefault();
            if (remdetails.vastbazaarsts == "N" || string.IsNullOrEmpty(remdetails.vastbazaarsts))
            {
                if (delimg == "AADHAR")
                {
                    remdetails.aadharcardPath = null;
                    remdetails.BackSideAadharcardphoto = null;
                    remdetails.AadhaarStatus = "N";
                }
                else if (delimg == "PAN")
                {
                    remdetails.PSAStatus = "N";
                    remdetails.pancardPath = null;
                }
                else if (delimg == "SELF")
                {
                    remdetails.ShopwithSalfie = null;
                    remdetails.ShopwithSalfieStatus = "N";
                }
                else
                {
                    if (remdetails.AadhaarStatus == "Y")
                    {
                        remdetails.AadhaarStatus = aadharsts;
                    }
                    if (remdetails.PSAStatus == "Y")
                    {
                        remdetails.PSAStatus = pansts;
                    }
                    if (remdetails.ShopwithSalfieStatus == "Y")
                    {
                        remdetails.ShopwithSalfieStatus = selfiests;
                    }
                    if (remdetails.AadhaarStatus == "Y" && remdetails.PSAStatus == "Y" && remdetails.ShopwithSalfieStatus == "Y")
                    {
                        if (remdetails.vastbazaarsts == "N" || string.IsNullOrEmpty(remdetails.vastbazaarsts))
                        {
                            remdetails.vastbazaarsts = vaststs;
                        }

                    }
                }
                DB.SaveChanges();
            }
            return Content("DONE", "application/json");
        }
        public string getAuthToken()
        {
            try
            {
                var tokn = DB.vastbazzartokens.SingleOrDefault();
                if (tokn == null)
                {
                    var response = tokencheck();
                    var responsechk = response.Content.ToString();
                    var responsecode = response.StatusCode.ToString();
                    if (responsecode == "OK")
                    {
                        dynamic json = JsonConvert.DeserializeObject(responsechk);
                        var token = json.access_token.ToString();
                        var expire = json[".expires"].ToString();
                        DateTime exp = Convert.ToDateTime(expire);
                        vastbazzartoken vast = new vastbazzartoken();
                        vast.apitoken = token;
                        vast.exptime = exp;
                        DB.vastbazzartokens.Add(vast);
                        DB.SaveChanges();
                        return tokn.apitoken;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    DateTime curntdate = DateTime.Now.Date;
                    DateTime expdate = Convert.ToDateTime(tokn.exptime).Date;
                    if (expdate > curntdate)
                    {
                        return tokn.apitoken;
                    }
                    else
                    {
                        var response = tokencheck();
                        var responsechk = response.Content.ToString();
                        var responsecode = response.StatusCode.ToString();
                        if (responsecode == "OK")
                        {

                            dynamic json = JsonConvert.DeserializeObject(responsechk);
                            var token = json.access_token.ToString();
                            var expire = json[".expires"].ToString();
                            DateTime exp = Convert.ToDateTime(expire);

                            tokn.apitoken = token;
                            tokn.exptime = exp;
                            DB.SaveChanges();
                            return token;
                        }
                        else
                        {
                            return null;
                        }
                    }

                }
            }
            catch (Exception Ex)
            {
                return null;
            }
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
        public void chkk()
        {
            System.Net.ServicePointManager.SecurityProtocol =
    SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            var url = "https://apigwuat.kotak.com:8443/cms_generic_service?apikey=l7xx9e2c880d0b4549049ff62c5bf595c310";

            var httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.Method = "POST";

            httpRequest.Accept = "application/xml";
            httpRequest.ContentType = "application/xml";

            var data = @"<soap:Envelope xmlns:soap=""http://www.w3.org/2003/05/soap-envelope"">
    <soap:Body>
        <ns3:Payment xmlns:ns3=""http://www.kotak.com/schemas/CMS_Generic/Payment_Request.xsd"">
            <ns3:RequestHeader>
                 <ns3:MessageId>11111111111</ns3:MessageId>
                <ns3:MsgSource>ABCCOMPANY</ns3:MsgSource>
                <ns3:ClientCode>TEMPTEST1</ns3:ClientCode>
                <ns3:BatchRefNmbr>11111111111</ns3:BatchRefNmbr>
            </ns3:RequestHeader>
            <ns3:InstrumentList>
                <ns3:instrument>
                    <ns3:InstRefNo>11111111111</ns3:InstRefNo>
                    <ns3:MyProdCode>CMSPAY</ns3:MyProdCode>
                    <ns3:PayMode>NEFT</ns3:PayMode>
                    <ns3:TxnAmnt>129.70</ns3:TxnAmnt>
                    <ns3:AccountNo>09582650000173</ns3:AccountNo>
                    <ns3:DrDesc>Hello Paras</ns3:DrDesc>
                    <ns3:PaymentDt>2022-02-01</ns3:PaymentDt>
                    <ns3:RecBrCd>UTIB0000207</ns3:RecBrCd>
                    <ns3:BeneAcctNo>12345678909</ns3:BeneAcctNo>
                    <ns3:BeneName>Paras Mbk</ns3:BeneName>
                    <ns3:InstDt>2022-02-01</ns3:InstDt>
                    <ns3:PaymentDtl1>gj j ghkj</ns3:PaymentDtl1>
                    <ns3:EnrichmentSet>
                        <ns3:Enrichment>TEST CLIENT~SAVING~TEST~09582650000173~FAMILY_MAINTENANCE</ns3:Enrichment>
                    </ns3:EnrichmentSet>
                </ns3:instrument>
            </ns3:InstrumentList>
        </ns3:Payment>
    </soap:Body>
</soap:Envelope>";

            using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
            {
                streamWriter.Write(data);
            }

            var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
            }


        }
    }
}