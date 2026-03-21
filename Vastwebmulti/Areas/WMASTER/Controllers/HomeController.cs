using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using Rotativa;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using Vastwebmulti.Areas.ADMIN.ViewModel;
using Vastwebmulti.Areas.WHITELABEL.Models;
using Vastwebmulti.Areas.WMASTER.Models;
using Vastwebmulti.Areas.WMASTER.ViewModel;
using Vastwebmulti.Hubs;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.WMASTER.Controllers
{
    [Authorize(Roles = "Whitelabelmaster")]
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
        // GET: WMASTER/Home
        private VastwebmultiEntities _db;
        public HomeController()
        {
            _db = new VastwebmultiEntities();
        }
        AppNotification notify = new AppNotification();
        ALLSMSSend smssend = new ALLSMSSend();
        private string GetToken()
        {
            var userid = User.Identity.GetUserId();
            var tokenCHK = _db.TokenGenWApis.Where(a => a.UserId == userid).SingleOrDefault();

            if (tokenCHK == null)
            {
                TempData.Remove("data");
                AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return null;
            }
            else
            {
                return tokenCHK.Token;
            }
        }
        #region SignalR
        public ActionResult TestSignlR()
        {
            var userid = User.Identity.Name;
            SendPushNotification(userid, "https://www.google.com", "Testing", "SignalR");
            return View();
        }
        public void SendPushNotification(string ReceiverMailID, string RedirectUrl, string Message, string Title)
        {
            VastwebmultiEntities db = new VastwebmultiEntities();
            NotificationHub objNotifHub = new NotificationHub();
            Notification objNotif = new Notification();
            objNotif.SentTo = ReceiverMailID ?? "";
            objNotif.Date = DateTime.Now;
            objNotif.IsRead = false;
            objNotif.IsDeleted = false;
            objNotif.DetailsURL = RedirectUrl ?? "";
            objNotif.Details = Message ?? "";
            objNotif.Title = Title ?? "";

            _db.Configuration.ProxyCreationEnabled = false;
            _db.Notifications.Add(objNotif);
            _db.SaveChanges();

            objNotifHub.SendNotification(objNotif.SentTo);
        }
        #endregion
        //Dashboard 
        public ActionResult Dashboard()
        {
            var userid = User.Identity.GetUserId();
            var vv = _db.Whitelabel_Superstokist_details.SingleOrDefault(a => a.SSId == userid);
            ViewBag.email = vv.Email;
            ViewBag.image = vv.Photo;
            //show News for WMASTER
            var whitelabelid = vv.Whitelabelid;
            ViewBag.news = (from pp in _db.Message_top where (pp.users == "WMaster" || pp.users == "All") where pp.status == "Y" && pp.UserId == whitelabelid select pp).ToList();

            ViewBag.showholiday = 0;

            DateTime todaysDate = DateTime.Now.Date;
            int month = todaysDate.Month;
            int year = todaysDate.Year;

            int Nxtmonth = todaysDate.AddMonths(1).Month;
            int Nxtyear = todaysDate.AddMonths(1).Year;

            Whitelabel_TargetSetviewmodel vmodel = new Whitelabel_TargetSetviewmodel();
            var allOn = _db.Whitelabel_superstockistsettarget.Where(a => a.WhitelabelId == whitelabelid && a.Status == true).ToList();
            vmodel.mdTargetCategory = allOn.Where(a => a.Date.Value.Month == month && a.Date.Value.Year == year).ToList();
            vmodel.mdTargetCategoryNxt = allOn.Where(a => a.Date.Value.Month == Nxtmonth && a.Date.Value.Year == Nxtyear).ToList();
            vmodel.productItems = _db.Whitelabel_PruductGift.Where(a => a.Whitelabelid == whitelabelid).ToList();
            vmodel.CategoryImages = Directory.EnumerateFiles(Server.MapPath("~/CategoryImages")).Select(fn => Path.GetFileNameWithoutExtension(fn));

            return View(vmodel);
        }

        //check WMASTER balance
        public ActionResult Chkbalance()
        {
            var userid = User.Identity.GetUserId();
            //get WMASTER Remain Balance

            //Get Dealer Crdit Balance
            var dealercreditbal = _db.spWhitelabel_total_wdealer_outstanding(userid).FirstOrDefault().totaloutstanding;
            //get WMASTER Credit Balance
            var mastercreditbal = _db.spWhitelabel_total_wmaster_outstanding(userid, "wadmin").FirstOrDefault().totaloutstanding;
            return Json(new
            {
                dealercreditbal = dealercreditbal,
                mastercreditbal = mastercreditbal
            });
        }

        public ActionResult Show_Dealer_outstandingreport()
        {
            var userid = User.Identity.GetUserId();
            var show = _db.spWhitelabel_Dealer_credit_report(userid).ToList();
            return View(show);
        }

        public ActionResult Totalbaltransfer()
        {
            var userid = User.Identity.GetUserId();
            var whitelabelid = _db.Whitelabel_Superstokist_details.Where(a => a.SSId == userid).SingleOrDefault().Whitelabelid;
            //Total WMASTER to Dealer Balance
            var mastertodealer = _db.spWhitelabel_total_master_to_dealer_balance_by_wmaster(whitelabelid, userid).FirstOrDefault().totalbal;
            //Admin to  WMASTER Balance 
            var admintomaster = _db.spWhitelabel_total_bal_master_balance(whitelabelid, userid).FirstOrDefault().totalbal;
            return Json(new
            {
                mastertodealer = mastertodealer,
                admintomaster = admintomaster

            });
        }

        //Total Active and Inactive USER
        //#region show active and inactive users
        //public ActionResult Show_All_ActiveandInactive_user()
        //{
        //    var userid = User.Identity.GetUserId();
        //  //Retailers
        //    var stackedchart = _db.show_all_active_inactive_rem_list_Master(userid).ToList();
        //    int actRtl = stackedchart.Where(a => a.type == "ACTIVE").Count();
        //    var Reatileractive = actRtl;
        //    var inRtl = stackedchart.Where(a => a.type == "INACTIVE").Count();
        //    var Retailerinactive = inRtl;
        //    int total = actRtl + inRtl;
        //    double act_rtl_percent = ((double)actRtl / (double)total) * 100;
        //    double Inact_rtl_percent = ((double)inRtl / (double)total) * 100;
        //    var RetaileractivePer = act_rtl_percent;//Math.Ceiling(act_rtl_percent);
        //    var RetailerTotal = total;
        //    //Distributers

        //    var stackedchartd = _db.show_all_active_inactive_dlm_list_Master(userid).ToList();
        //    int actDealer = stackedchartd.Where(a => a.type == "ACTIVE").Count();
        //    var Dealeractive = actDealer;
        //    var inDealer = stackedchartd.Where(a => a.type == "INACTIVE").Count();
        //    var Dealerinactive = inDealer;
        //    int dealertotal = actDealer + inDealer;
        //    double act_dlm_percent = ((double)actDealer / (double)total) * 100;
        //    double Inact_dlm_percent = ((double)inDealer / (double)total) * 100;
        //    var DealeractivePer = act_dlm_percent;//Math.Ceiling(act_rtl_percent);
        //    var DealerTotal = dealertotal;

        //    return Json(new
        //    {
        //         Dealeractive = Dealeractive,
        //        Dealerinactive = Dealerinactive,
        //        DealerTotal = DealerTotal,
        //        DealeractivePer = DealeractivePer,
        //        Reatileractive = Reatileractive,
        //        Retailerinactive = Retailerinactive,
        //        RetailerTotal = RetailerTotal,
        //        RetaileractivePer = RetaileractivePer

        //    });
        //}
        //#endregion

        //show today and yesterday business
        #region show today and yesterday business
        public ActionResult Show_All_Recharge(string type)
        {
            var userid = User.Identity.GetUserId();
            var whitelabelid = _db.Whitelabel_Superstokist_details.Where(a => a.SSId == userid).SingleOrDefault().Whitelabelid;

            // Donught Chart
            var result = _db.spWhitelabel_todayrecharge_wmaster(whitelabelid, userid, type).ToList();
            //prepaid
            var prepaid = result.Where(a => a.operator_type == "PrePaid").SingleOrDefault() != null ? result.Where(a => a.operator_type == "PrePaid").SingleOrDefault().total : 0;
            //postpaid
            var postpaid = result.Where(a => a.operator_type == "PostPaid").SingleOrDefault() != null ? result.Where(a => a.operator_type == "PostPaid").SingleOrDefault().total : 0;
            //dth
            var dth = result.Where(a => a.operator_type == "DTH").SingleOrDefault() != null ? result.Where(a => a.operator_type == "DTH").SingleOrDefault().total : 0;
            //landline
            var landline = result.Where(a => a.operator_type == "Landline").SingleOrDefault() != null ? result.Where(a => a.operator_type == "Landline").SingleOrDefault().total : 0;
            //electricity
            var Electricity = result.Where(a => a.operator_type == "Electricity").SingleOrDefault() != null ? result.Where(a => a.operator_type == "Electricity").SingleOrDefault().total : 0;
            //gas
            var Gas = result.Where(a => a.operator_type == "Gas").SingleOrDefault() != null ? result.Where(a => a.operator_type == "Gas").SingleOrDefault().total : 0;
            //insurance
            var Insurance = result.Where(a => a.operator_type == "Insurance").SingleOrDefault() != null ? result.Where(a => a.operator_type == "Insurance").SingleOrDefault().total : 0;
            // DTH-Booking
            var dthbooking = result.Where(a => a.operator_type == "DTH-Booking").SingleOrDefault() != null ? result.Where(a => a.operator_type == "DTH-Booking").SingleOrDefault().total : 0;
            // DTH-Booking
            var water = result.Where(a => a.operator_type == "Water").SingleOrDefault() != null ? result.Where(a => a.operator_type == "Water").SingleOrDefault().total : 0;
            var rechargeandbill = (prepaid + postpaid + dth + landline + Electricity + Gas + Insurance + dthbooking + water);

            var moneytransfer = result.Where(a => a.operator_type == "MoneyTransfer").SingleOrDefault() != null ? result.Where(a => a.operator_type == "MoneyTransfer").SingleOrDefault().total : 0;

            var Aeps = result.Where(a => a.operator_type == "Aeps").SingleOrDefault() != null ? result.Where(a => a.operator_type == "Aeps").SingleOrDefault().total : 0;

            var Pancard = result.Where(a => a.operator_type == "Pancard").SingleOrDefault() != null ? result.Where(a => a.operator_type == "Pancard").SingleOrDefault().total : 0;

            return Json(new { Status = type, Recharge = rechargeandbill, Moneytransfer = moneytransfer, Aeps = Aeps, Pancard = Pancard });
        }
        #endregion

        #region Notification
        public ActionResult Notification()
        {
            var userid = User.Identity.GetUserId();

            ViewData["success"] = TempData["success"];
            TempData.Remove("success");
            ViewBag.alldealer = new SelectList(_db.whitelabel_Dealer_Details.Where(a => a.masterid == userid), "Dealerid", "DealerName", null).ToList();
            return View();
        }
        [HttpPost]
        public ActionResult Send_Notification(string dealerid, string txtMsgBody)
        {
            var userid = User.Identity.GetUserId();

            if (dealerid == "" || dealerid == null)
            {
                var usermobiles = _db.whitelabel_Dealer_Details.Where(a => a.masterid == userid).ToList();
                foreach (var item in usermobiles)
                {
                    SendPushNotification(item.Email, Url.Action("Dashboard", "Home"), txtMsgBody, "Notification.");

                    notify.sendmessage(item.Email, txtMsgBody);
                }

            }
            else
            {
                var email = _db.whitelabel_Dealer_Details.Where(a => a.DealerId == dealerid && a.masterid == userid).FirstOrDefault().Email;
                SendPushNotification(email, Url.Action("Dashboard", "Home"), txtMsgBody, "Notification.");

                notify.sendmessage(email, txtMsgBody);
            }
            TempData["success"] = "Notification send successfully.";
            return RedirectToAction("Notification", "Home");
        }
        #endregion End Notification
        #region  WMASTER Income
        public ActionResult Master_income()
        {
            //var userid = User.Identity.GetUserId();

            var Token = string.Empty;
            Token = GetToken();

            if (Token == null)
            {
                return RedirectToAction("Index", "Home", null);
            }
            else
            {
                string txt_to_date1 = DateTime.Now.ToString();
                string frm_date = Convert.ToDateTime(txt_to_date1).AddDays(-1).ToShortDateString();

                var client = new RestClient(WapiBaseUrl.GetBaseUrl() + "/api/master/ShowActualIncome?from=" + frm_date);
                var request = new RestRequest(Method.GET);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", "bearer " + Token + "");
                IRestResponse response = client.Execute(request);

                var statusCode = response.StatusCode.ToString();
                if (statusCode == "Unauthorized")
                {
                    TempData.Remove("data");
                    AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                    return RedirectToAction("Index", "Home", null);
                }

                var responsechk = response.Content.ToString();
                JObject results = JObject.Parse(responsechk);

                List<RootObject> EmpInfo = new List<RootObject>();

                return View(response.StatusCode.ToString() == "OK" ? JsonConvert.DeserializeObject<List<RootObject>>(results["RESULT"].ToString()) : EmpInfo);
            }

        }
        [HttpPost]
        public ActionResult Master_income(string txt_frm_date)
        {
            var Token = string.Empty;
            Token = GetToken();
            if (Token == null)
            {
                return RedirectToAction("Index", "Home", null);
            }
            else
            {
                //var userid = User.Identity.GetUserId();
                DateTime frm = Convert.ToDateTime(txt_frm_date);
                DateTime to = Convert.ToDateTime(txt_frm_date);
                ViewBag.chk = "post";

                txt_frm_date = frm.ToString("dd-MM-yyyy");
                var txt_to_date = to.ToString("dd-MM-yyyy");
                string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy",
                            "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
                DateTime dt = DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
                DateTime dt1 = DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
                string frm_date = Convert.ToDateTime(dt).ToShortDateString();
                string to_date = Convert.ToDateTime(dt1).AddDays(1).ToString();
                //var ch = _db.show_all_user_income(Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), "WMASTER", userid).ToList();

                var client = new RestClient(WapiBaseUrl.GetBaseUrl() + "/api/master/ShowActualIncome?from=" + frm_date);
                var request = new RestRequest(Method.GET);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", "bearer " + Token + "");
                IRestResponse response = client.Execute(request);

                var statusCode = response.StatusCode.ToString();
                if (statusCode == "Unauthorized")
                {
                    TempData.Remove("data");
                    AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                    return RedirectToAction("Index", "Home", null);
                }

                var responsechk = response.Content.ToString();
                JObject results = JObject.Parse(responsechk);

                List<RootObject> EmpInfo = new List<RootObject>();

                return View(response.StatusCode.ToString() == "OK" ? JsonConvert.DeserializeObject<List<RootObject>>(results["RESULT"].ToString()) : EmpInfo);
            }
        }

        public ActionResult Actual_Master_income()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            var client = new RestClient("https://fingpayap.tapits.in/fpaepsservice/api/bankdata/bank/details");
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);
            var items = JsonConvert.DeserializeObject<RETAILER.Models.BankIiNoModel>(response.Content);
            ViewBag.BankList = items.Data.Select(a => new SelectListItem { Text = a.BankName, Value = a.BankName }).ToList();
            var userid = User.Identity.GetUserId();
            var ch = _db.Whitelabel_bank_info.Where(a => a.userid == userid).ToList();
            ViewBag.temp = TempData["add"];
            return View(ch);
        }
        [HttpPost]
        public ActionResult Actual_Master_income(string txt_frm_date)
        {
            var userid = User.Identity.GetUserId();
            DateTime frm = Convert.ToDateTime(txt_frm_date);
            DateTime to = Convert.ToDateTime(txt_frm_date);
            ViewBag.chk = "post";

            txt_frm_date = frm.ToString("dd-MM-yyyy");
            var txt_to_date = to.ToString("dd-MM-yyyy");
            string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy",
                            "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
            DateTime dt = DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            DateTime dt1 = DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            string frm_date = Convert.ToDateTime(dt).ToShortDateString();
            string to_date = Convert.ToDateTime(dt1).AddDays(1).ToString();
            var ch = _db.spWhitelabel_all_user_income(Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), "Master", userid).ToList();
            return View(ch);
        }
        #endregion
        #region BankInfo
        [HttpPost]
        public ActionResult Delete_bankinfo(int idno)
        {
            if (idno != 0)
            {
                var result = _db.Whitelabel_bank_info.Find(idno);
                _db.Whitelabel_bank_info.Remove(result);
                _db.SaveChanges();

                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("Failed", JsonRequestBehavior.AllowGet);

            }

        }

        [HttpPost]
        public ActionResult Insert_bankinfo(string txtbanknm, string txtbranchnm, string txtifsc, string txtacno, string txtacctype, string txtname, string txtaddress)
        {
            var userid = User.Identity.GetUserId();
            var ch = _db.Whitelabel_bank_info.Where(a => a.acno == txtacno).ToList();

            var count1 = ch.Count;

            if (count1 == 0)
            {
                Whitelabel_bank_info objCourse = new Whitelabel_bank_info();
                objCourse.banknm = txtbanknm;
                objCourse.branch_nm = txtbranchnm;
                objCourse.ifsccode = txtifsc;
                objCourse.acno = txtacno;
                objCourse.actype = txtacctype;
                objCourse.holdername = txtname;
                objCourse.address = txtaddress;
                objCourse.userid = userid;

                _db.Whitelabel_bank_info.Add(objCourse);
                _db.SaveChanges();
                TempData["add"] = "Success";
            }
            else
            {
                TempData["add"] = "Failed";
            }
            return RedirectToAction("Actual_Master_income");
        }
        #endregion
        public ActionResult Index()
        {
            try
            {
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    ViewData["msg"] = TempData["msgrem"];
                    ViewData["ResendMail"] = TempData["ResendMAil"];
                    string userid = User.Identity.GetUserId();
                    var whitelabelid = _db.Whitelabel_Superstokist_details.SingleOrDefault(a => a.SSId == userid).Whitelabelid;

                    ViewBag.state = new SelectList(_db.Select_State_Details(), "State_Id", "State_Name").ToList(); ViewBag.state1 = new SelectList(_db.Select_State_Details(), "State_Id", "State_Name");
                    var Details = _db.SpWhitelabel_Select_Dealer_total(whitelabelid, userid).ToList();
                    WMASTER.Models.WDealerModel viewModel = new WMASTER.Models.WDealerModel();
                    viewModel.SpWhitelabel_Select_Dealer_total_Result = Details;

                    ViewBag.state1 = new SelectList(_db.Select_State_Details(), "State_Id", "State_Name");
                    ViewData["ResendMail"] = TempData["ResendMAil"];

                    return View(viewModel);
                }
            }
            catch (Exception)
            {
                throw;
            }

        }

        #region delete Dealer and send otp 
        //public JsonResult DeleteDealerSendOTP()
        //{
        //    try
        //    {
        //        var userid = User.Identity.GetUserId();

        //        int pin = new Random().Next(1000, 10000);
        //        deleteuserotp motp = new deleteuserotp();
        //        motp.otp = pin;
        //        motp.userid = userid;
        //        motp.SentDate = DateTime.Now;
        //        _db.deleteuserotps.Add(motp);
        //        _db.SaveChanges();
        //        new ADMIN.Models.DeleteUserSendOtp().sendotpBymastermail(pin, userid);
        //        return Json("Success", JsonRequestBehavior.AllowGet);
        //    }
        //    catch
        //    {
        //        return Json("Failed", JsonRequestBehavior.AllowGet);
        //    }

        //}
        //public JsonResult Deletedealer(string DealerId, int OTP)
        //{
        //    try
        //    {
        //        var chk = _db.deleteuserotps.Any(a => a.otp == OTP);
        //        if (chk == true)
        //        {
        //            if (DealerId != null && DealerId != "")
        //            {
        //                _db.delete_Dealer(DealerId);
        //                _db.deleteOTP();
        //                return Json("Success", JsonRequestBehavior.AllowGet);

        //            }
        //            else
        //            {
        //                return Json("Failed", JsonRequestBehavior.AllowGet);
        //            }
        //        }
        //        else
        //        {
        //            return Json("OTPWrong", JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //    catch
        //    {
        //        return Json("Failed", JsonRequestBehavior.AllowGet);
        //    }
        //}
        #endregion
        #region Complaint Request 
        //public ActionResult Complaint()
        //{
        //    var userid = User.Identity.GetUserId();
        //    var ch = _db.proc_Complaint_request(userid, "").ToList();
        //    return View(ch);
        //}

        //[HttpPost]
        //public ActionResult Complaint_insert(string message)
        //{
        //    var statusAdmin = _db.PushNotificationStatus.Where(a => a.UserRole == "Admin").SingleOrDefault().Status;
        //    var Emailid = _db.Admin_details.Single().email;
        //    string userid = User.Identity.GetUserId();
        //    var retaileremaillid = _db.Users.Where(p => p.UserId == userid).Single().Email;
        //    Guid randomId = Guid.NewGuid();
        //    string uniqueId = randomId.ToString().Substring(0, 18).ToUpper();
        //    DateTime date = System.DateTime.Now;
        //    complaint_request objCourse = new complaint_request();
        //    objCourse.subject = "Chatting";
        //    objCourse.complant = message;
        //    objCourse.complaintid = uniqueId;
        //    objCourse.userid = userid;
        //    objCourse.sts = "Open";
        //    objCourse.rdate = date;
        //    _db.complaint_request.Add(objCourse);
        //    _db.SaveChanges();
        //    if (statusAdmin == "Y")
        //    {
        //        SendPushNotification(Emailid, Url.Action("Money_Transfer_Report", "Home"), "User " + retaileremaillid + " is Send the Complaint For You .And Compalint is that " + message + "", "Complaint Insert..");
        //    }
        //    return RedirectToAction("Complaint");
        //}
        //public ActionResult RenderHeader()
        //{
        //    return View();
        //}
        //public ActionResult TopBAR()
        //{
        //    var currentuser = User.Identity.GetUserId();
        //    using (VastwebmultiEntities db = new VastwebmultiEntities())
        //    {
        //        TempData["VendorName"] = _db.Vendor_details.FirstOrDefault(a => a.userid == currentuser)?.Name;
        //        TempData["VendorEmail"] = _db.Users.SingleOrDefault(a => a.UserId == currentuser)?.Email;
        //        TempData["Balance"] = _db.Remain_superstokist_balance.SingleOrDefault(a => a.SuperStokistID == currentuser)?.Remainamount;
        //    }
        //    return View();
        //}
        //public ActionResult RenderSection()
        //{
        //    using (VastwebmultiEntities db = new VastwebmultiEntities())
        //    {
        //        var currentuser = User.Identity.GetUserId();
        //        TempData["VendorEmail"] = _db.Users.SingleOrDefault(a => a.UserId == currentuser)?.Email;
        //    }
        //    return View();
        //}
        #endregion

        #region TokenPurcharse
        //[HttpGet]
        //public ActionResult addTokenMaster()
        //{
        //    var ddlMaster = User.Identity.GetUserId();
        //    return View();
        //}

        //[ChildActionOnly]
        //public ActionResult _Tokenreport()
        //{
        //    var ddlMaster = User.Identity.GetUserId();
        //    var ch = _db.Master_token_select(ddlMaster).ToList();

        //    return View(ch);
        //}

        //public ActionResult addTokenMaster1(int tokenCount)
        //{
        //    var ddlMaster = User.Identity.GetUserId();
        //    Token_PaidService_VM1 Tkn_PaidSer = new Token_PaidService_VM1();
        //    try
        //    {
        //        DealerCreationTokensAssignHistory entry = new DealerCreationTokensAssignHistory();
        //        DealerCreationToken token = _db.DealerCreationTokens.SingleOrDefault(a => a.Masterid == ddlMaster);
        //        var TokenAssignEntriess = _db.DealerCreationTokensAssignHistories.Join(_db.Superstokist_details, tkn => tkn.MasterId, dlm => dlm.SSId, (tkn, dlm) => new DealerCreationTokenVM1
        //        {
        //            Masterid = dlm.SSId,
        //            Email = dlm.FarmName,
        //            Idno = tkn.Idno,
        //            Tokens = tkn.Tokens,
        //            CteatedOn = tkn.CreatedOn,
        //            pre = tkn.RemainTokenPre,
        //            post = tkn.RemainTokenPost,
        //            MasterPre = tkn.MasterPre,
        //            MasterPost = tkn.MasterPost,
        //            AdminPre = tkn.AdminPre,
        //            AdminPost = tkn.AdminPost,
        //            PerTokenValue = tkn.PerTokenValue,
        //            TotalDebit = tkn.TotalDebit
        //        }).OrderByDescending(aa => aa.CteatedOn);
        //        Tkn_PaidSer.DealerCreationTokenVM = TokenAssignEntriess.ToList();
        //        if (token == null)
        //        {
        //            token = new DealerCreationToken();
        //            token.Masterid = ddlMaster;
        //            token.Tokens = tokenCount;
        //            _db.DealerCreationTokens.Add(token);
        //            entry.CreatedOn = DateTime.Now;
        //            entry.MasterId = ddlMaster;
        //            entry.Tokens = tokenCount;
        //            entry.RemainTokenPre = 0;
        //            entry.RemainTokenPost = tokenCount;
        //            //_db.RetailerCreationTokensAssignHistories.Add(entry);
        //        }
        //        else
        //        {
        //            entry.CreatedOn = DateTime.Now;
        //            entry.MasterId = ddlMaster;
        //            entry.Tokens = tokenCount;
        //            entry.RemainTokenPre = token.Tokens;
        //            entry.RemainTokenPost = token.Tokens + tokenCount;
        //            //_db.RetailerCreationTokensAssignHistories.Add(entry);
        //            token.Tokens = token.Tokens + tokenCount;
        //        }
        //        var count = _db.TokenValueByAdmins.ToList();
        //        if (count.Any())
        //        {
        //            var Mastervalue = _db.TokenValueByAdmins.SingleOrDefault().MasterValue;
        //            decimal TotalTokenValue = Convert.ToDecimal(Mastervalue) * Convert.ToDecimal(tokenCount);
        //            var Masterinfo = _db.Remain_superstokist_balance.Where(a => a.SuperStokistID == ddlMaster).SingleOrDefault();
        //            var MasterPre = Masterinfo.Remainamount;
        //            var AdminPre = _db.Remain_Admin_balance.SingleOrDefault().RemainAmount;
        //            if (MasterPre >= TotalTokenValue)
        //            {
        //                var MasterPost = MasterPre - TotalTokenValue;
        //                var AdminPost = AdminPre + TotalTokenValue;

        //                entry.MasterPre = MasterPre;
        //                entry.MasterPost = MasterPost;
        //                entry.AdminPre = AdminPre;
        //                entry.AdminPost = AdminPost;
        //                entry.TotalDebit = TotalTokenValue;
        //                entry.TotalTokenValue = TotalTokenValue;
        //                entry.PerTokenValue = Convert.ToDecimal(Mastervalue);
        //                entry.role = "Self";
        //                _db.DealerCreationTokensAssignHistories.Add(entry);

        //                //Update Dealer Remain Balance
        //                Masterinfo.Remainamount = MasterPost;

        //                //Update Admin Remain Balance
        //                var id = _db.Remain_Admin_balance.SingleOrDefault();
        //                id.RemainAmount = AdminPost;

        //                LedgerReport ledger = new LedgerReport();
        //                ledger.UserId = ddlMaster;
        //                ledger.Role = "WMASTER";
        //                ledger.Particulars = "Token Purchase";
        //                ledger.UserRemainAmount = MasterPost;
        //                ledger.Date = Convert.ToDateTime(DateTime.Now.ToString());
        //                ledger.Amount = TotalTokenValue;
        //                ledger.Credit = 0;
        //                ledger.Debit = TotalTokenValue;
        //                _db.LedgerReports.Add(ledger);

        //                ledger.UserId = "Admin";
        //                ledger.Role = "Admin";
        //                ledger.Particulars = "Token Purchase";
        //                ledger.UserRemainAmount = AdminPost;
        //                ledger.Date = Convert.ToDateTime(DateTime.Now.ToString());
        //                ledger.Amount = TotalTokenValue;
        //                ledger.Credit = TotalTokenValue;
        //                ledger.Debit = 0;
        //                _db.LedgerReports.Add(ledger);

        //                TempData["success"] = "Credited Successfully.";
        //            }
        //            else
        //            {
        //                return Json("Insufficient balance.", JsonRequestBehavior.AllowGet);
        //            }
        //        }
        //        else
        //        {
        //            TempData["error"] = "Set Token Value.";
        //            return Json(Tkn_PaidSer.DealerCreationTokenVM, JsonRequestBehavior.AllowGet);
        //        }
        //        _db.SaveChanges();
        //        TokenAssignEntriess = _db.DealerCreationTokensAssignHistories.Join(_db.Superstokist_details, tkn => tkn.MasterId, dlm => dlm.SSId, (tkn, dlm) => new DealerCreationTokenVM1
        //        {
        //            Masterid = dlm.SSId,
        //            Email = dlm.FarmName,
        //            Idno = tkn.Idno,
        //            Tokens = tkn.Tokens,
        //            CteatedOn = tkn.CreatedOn,
        //            pre = tkn.RemainTokenPre,
        //            post = tkn.RemainTokenPost,
        //            MasterPre = tkn.MasterPre,
        //            MasterPost = tkn.MasterPost,
        //            AdminPre = tkn.AdminPre,
        //            AdminPost = tkn.AdminPost,
        //            PerTokenValue = tkn.PerTokenValue,
        //            TotalDebit = tkn.TotalDebit
        //        }).OrderByDescending(aa => aa.CteatedOn);
        //        Tkn_PaidSer.DealerCreationTokenVM = TokenAssignEntriess.ToList();
        //        return Json(Tkn_PaidSer.DealerCreationTokenVM, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["error"] = ex.Message;
        //        return Json(Tkn_PaidSer.DealerCreationTokenVM, JsonRequestBehavior.AllowGet);
        //    }
        //}
        #endregion
        #region TDSReport
        //public ActionResult TDSReport()
        //{
        //    var userid = User.Identity.GetUserId();
        //    ADMIN.Models.TDSReportModel model = new ADMIN.Models.TDSReportModel();
        //    DateTime date = DateTime.Now;
        //    var txt_frm_date = new DateTime(date.Year, date.Month, 1);
        //    string to_date = DateTime.Now.ToString();

        //    string CurrentMonthName = date.ToString("MMMM");
        //    string OldMonth1 = date.AddMonths(-1).ToString("MMMM");
        //    string OldMonth2 = date.AddMonths(-2).ToString("MMMM");
        //    ViewBag.CurrentMonthName = CurrentMonthName;
        //    ViewBag.OldMonth1 = OldMonth1;
        //    ViewBag.OldMonth2 = OldMonth2;
        //    ViewBag.Crmonth = CurrentMonthName;
        //    model.TDSMaster = _db.TDS_Report_master(Convert.ToDateTime(txt_frm_date), Convert.ToDateTime(to_date), userid).ToList();
        //    return View(model);
        //}
        //[HttpPost]
        //public ActionResult TDSReport(string submit)
        //{
        //    var useridid = User.Identity.GetUserId();
        //    ADMIN.Models.TDSReportModel model = new ADMIN.Models.TDSReportModel();
        //    DateTime txt_frm_date = DateTime.Now;
        //    DateTime to_date = DateTime.Now;
        //    DateTime date = DateTime.Now;
        //    string CurrentMonthName = date.ToString("MMMM");
        //    string OldMonth1 = date.AddMonths(-1).ToString("MMMM");
        //    string OldMonth2 = date.AddMonths(-2).ToString("MMMM");
        //    if (CurrentMonthName == submit)
        //    {
        //        txt_frm_date = new DateTime(date.Year, date.Month, 1);
        //        to_date = DateTime.Now;

        //    }
        //    else if (OldMonth1 == submit)
        //    {
        //        txt_frm_date = new DateTime(date.AddMonths(-1).Year, date.AddMonths(-1).Month, 1);
        //        to_date = new DateTime(date.Year, date.Month, 1);
        //    }
        //    else
        //    {
        //        txt_frm_date = new DateTime(date.AddMonths(-2).Year, date.AddMonths(-2).Month, 1);
        //        to_date = new DateTime(date.AddMonths(-1).Year, date.AddMonths(-1).Month, 1);
        //    }
        //    model.TDSMaster = _db.TDS_Report_master(Convert.ToDateTime(txt_frm_date), Convert.ToDateTime(to_date), useridid).ToList();

        //    ViewBag.CurrentMonthName = CurrentMonthName;
        //    ViewBag.OldMonth1 = OldMonth1;
        //    ViewBag.OldMonth2 = OldMonth2;
        //    ViewBag.Crmonth = submit;

        //    return View(model);
        //}
        #endregion
        #region GSTReport
        //public ActionResult GSTReport()
        //{
        //    var userid = User.Identity.GetUserId();
        //    ADMIN.Models.GSTReportModel model = new ADMIN.Models.GSTReportModel();
        //    DateTime date = DateTime.Now;
        //    var txt_frm_date = new DateTime(date.Year, date.Month, 1);
        //    string to_date = DateTime.Now.ToString();

        //    string CurrentMonthName = date.ToString("MMMM");
        //    string OldMonth1 = date.AddMonths(-1).ToString("MMMM");
        //    string OldMonth2 = date.AddMonths(-2).ToString("MMMM");
        //    ViewBag.CurrentMonthName = CurrentMonthName;
        //    ViewBag.OldMonth1 = OldMonth1;
        //    ViewBag.OldMonth2 = OldMonth2;
        //    ViewBag.Crmonth = CurrentMonthName;
        //    model.GSTMaster = _db.GST_Report_master(Convert.ToDateTime(txt_frm_date), Convert.ToDateTime(to_date), userid).ToList();
        //    return View(model);
        //}
        //[HttpPost]
        //public ActionResult GSTReport(string submit)
        //{
        //    ADMIN.Models.GSTReportModel model = new ADMIN.Models.GSTReportModel();
        //    var userid = User.Identity.GetUserId();
        //    DateTime txt_frm_date = DateTime.Now;
        //    DateTime to_date = DateTime.Now;
        //    DateTime date = DateTime.Now;
        //    string CurrentMonthName = date.ToString("MMMM");
        //    string OldMonth1 = date.AddMonths(-1).ToString("MMMM");
        //    string OldMonth2 = date.AddMonths(-2).ToString("MMMM");
        //    if (CurrentMonthName == submit)
        //    {
        //        txt_frm_date = new DateTime(date.Year, date.Month, 1);
        //        to_date = DateTime.Now;

        //    }
        //    else if (OldMonth1 == submit)
        //    {
        //        txt_frm_date = new DateTime(date.AddMonths(-1).Year, date.AddMonths(-1).Month, 1);
        //        to_date = new DateTime(date.Year, date.Month, 1);
        //    }
        //    else
        //    {
        //        txt_frm_date = new DateTime(date.AddMonths(-2).Year, date.AddMonths(-2).Month, 1);
        //        to_date = new DateTime(date.AddMonths(-1).Year, date.AddMonths(-1).Month, 1);
        //    }
        //    model.GSTMaster = _db.GST_Report_master(Convert.ToDateTime(txt_frm_date), Convert.ToDateTime(to_date), userid).ToList();
        //    ViewBag.CurrentMonthName = CurrentMonthName;
        //    ViewBag.OldMonth1 = OldMonth1;
        //    ViewBag.OldMonth2 = OldMonth2;
        //    ViewBag.Crmonth = submit;

        //    return View(model);
        //}

        #endregion

        #region WMASTER Gst Invocing Report
        //public ActionResult Gst_Invocing_Master_report()
        //{
        //    var userid = User.Identity.GetUserId();
        //    DateTime txt_frm_date = DateTime.Now;
        //    DateTime to_date = DateTime.Now;
        //    DateTime date = DateTime.Now;
        //    string OldMonth = date.AddMonths(-1).ToString("MMMM");
        //    ViewBag.OldMonth = OldMonth;
        //    txt_frm_date = new DateTime(date.AddMonths(-1).Year, date.AddMonths(-1).Month, 1);
        //    to_date = new DateTime(date.Year, date.Month, 1);
        //    var show = _db.GST_Monthly_master(Convert.ToDateTime(txt_frm_date), Convert.ToDateTime(to_date)).Where(a => a.apiid == userid).ToList();
        //    return View(show);
        //}
        //public ActionResult GST_Invocing_Report_Pdf()
        //{
        //    var userid = User.Identity.GetUserId();
        //    var today = DateTime.Today;
        //    var month = new DateTime(today.Year, today.Month, 1);
        //    var last = month.AddDays(-1);
        //    ViewBag.last = last.ToShortDateString();

        //    DateTime txt_frm_date = DateTime.Now;
        //    DateTime to_date = DateTime.Now;
        //    DateTime date = DateTime.Now;
        //    txt_frm_date = new DateTime(date.AddMonths(-1).Year, date.AddMonths(-1).Month, 1);
        //    to_date = new DateTime(date.Year, date.Month, 1);

        //    var entries = _db.GST_Monthly_master(Convert.ToDateTime(txt_frm_date), Convert.ToDateTime(to_date)).Where(a => a.apiid == userid).ToList();
        //    ViewBag.name = entries.SingleOrDefault().SuperstokistName;
        //    ViewBag.address = entries.SingleOrDefault().Address;
        //    ViewBag.customergst = entries.SingleOrDefault().GST;
        //    ViewBag.dmtcomm = entries.SingleOrDefault().dmtcomm;
        //    ViewBag.dmttotal = entries.SingleOrDefault().dmtcomm + entries.SingleOrDefault().dmtgst;
        //    ViewBag.rechargecomm = entries.SingleOrDefault().rchcomm;
        //    ViewBag.rechargetotal = entries.SingleOrDefault().rchcomm + entries.SingleOrDefault().rchgst;
        //    ViewBag.Mposcomm = entries.SingleOrDefault().mposcomm;
        //    ViewBag.Mpostotal = entries.SingleOrDefault().mposcomm + entries.SingleOrDefault().mposgst;
        //    ViewBag.Aepscomm = entries.SingleOrDefault().aepscomm;
        //    ViewBag.Aepstotal = entries.SingleOrDefault().aepscomm + entries.SingleOrDefault().aepsgst;
        //    ViewBag.Pancomm = entries.SingleOrDefault().pancomm;
        //    ViewBag.Pantotal = entries.SingleOrDefault().pancomm + entries.SingleOrDefault().pangst;
        //    ViewBag.Flightcomm = entries.SingleOrDefault().flightcomm;
        //    ViewBag.Flighttotal = entries.SingleOrDefault().flightcomm + entries.SingleOrDefault().flightgst;
        //    var final = entries.SingleOrDefault().dmtcomm + entries.SingleOrDefault().rchcomm +
        //        entries.SingleOrDefault().mposcomm + entries.SingleOrDefault().aepscomm +
        //        entries.SingleOrDefault().pancomm + entries.SingleOrDefault().flightcomm;
        //    ViewBag.totaltaxvalue = final;

        //    var valdmt = entries.SingleOrDefault().dmtgst;
        //    var valrecharge = entries.SingleOrDefault().rchgst;
        //    var valmpos = entries.SingleOrDefault().mposgst;
        //    var valaeps = entries.SingleOrDefault().aepsgst;
        //    var valpan = entries.SingleOrDefault().pangst;
        //    var valFlight = entries.SingleOrDefault().flightgst;
        //    var finalgst = valdmt + valrecharge + valmpos + valaeps + valpan +
        //        valFlight;

        //    ViewBag.finaltotal = finalgst + final;
        //    ViewBag.totalgsttotal = finalgst;
        //    double fin = Convert.ToDouble(finalgst + final);
        //    string s = words(fin, true).Substring(0, 3);
        //    if (s == "and")
        //    {
        //        s = words(fin, true).Remove(0, 4);
        //    }
        //    else
        //    {
        //        s = words(fin, true);
        //    }
        //    ViewBag.finalword = s;
        //    if (entries.SingleOrDefault().State_name != "Rajasthan")
        //    {
        //        ViewBag.type = "N";
        //        ViewBag.igst = 18;
        //        ViewBag.dmtigst = valdmt;
        //        ViewBag.rechargeigst = valrecharge;
        //        ViewBag.mposigst = valmpos;
        //        ViewBag.Aepsigst = valaeps;
        //        ViewBag.Panigst = valpan;
        //        ViewBag.Flightigst = valFlight;
        //        ViewBag.totaligst = finalgst;
        //    }
        //    else
        //    {
        //        ViewBag.type = "Y";
        //        ViewBag.cgst = 9;
        //        ViewBag.sgst = 9;


        //        ViewBag.dmtcgst = valdmt / 2;
        //        ViewBag.dmtsgst = valdmt / 2;
        //        ViewBag.rechargecgst = valrecharge / 2;
        //        ViewBag.rechargesgst = valrecharge / 2;
        //        ViewBag.mposcgst = valmpos / 2;
        //        ViewBag.mpossgst = valmpos / 2;
        //        ViewBag.Aepscgst = valaeps / 2;
        //        ViewBag.Aepssgst = valaeps / 2;
        //        ViewBag.Pancgst = valpan / 2;
        //        ViewBag.Pansgst = valpan / 2;
        //        ViewBag.Flightcgst = valFlight / 2;
        //        ViewBag.Flightsgst = valFlight / 2;
        //        ViewBag.totalcgst = finalgst / 2;
        //        ViewBag.totalsgst = finalgst / 2;
        //    }

        //    //ViewBag.particular = "Commission For " + from.ToString("MMMM") + " Month";
        //    ViewBag.netamount = entries.SingleOrDefault().dmtcomm;
        //    ViewBag.firmname = entries.SingleOrDefault().FarmName;
        //    ViewBag.total = entries.SingleOrDefault().dmtcomm + entries.SingleOrDefault().dmtgst;
        //    var admininfo = _db.Admin_details.FirstOrDefault();
        //    ViewBag.cmpyname = admininfo.Companyname;
        //    ViewBag.adminaddress = admininfo.Address;
        //    ViewBag.adminpan = admininfo.pencardno;
        //    ViewBag.admingst = admininfo.Gstno;
        //    ViewBag.pancard = entries.SingleOrDefault().PanCard;
        //    var number = entries.SingleOrDefault().dmtcomm.ToString();
        //    number = Convert.ToDouble(number).ToString();

        //    return new ViewAsPdf("GST_Invocing_Report_Pdf", entries);

        //}
        //public string words(double numbers, Boolean paisaconversion = false)
        //{
        //    var pointindex = numbers.ToString().IndexOf(".");
        //    int number = Convert.ToInt32(Math.Floor(numbers));
        //    decimal paisaamt = 0;
        //    var jj = numbers.ToString().Split('.');
        //    if (pointindex > 0)
        //        paisaamt = Convert.ToDecimal(jj[1]);



        //    if (number == 0) return "Zero";
        //    if (number == -2147483648) return "Minus Two Hundred and Fourteen Crore Seventy Four Lakh Eighty Three Thousand Six Hundred and Forty Eight";
        //    int[] num = new int[4];
        //    int first = 0;
        //    int u, h, t;
        //    System.Text.StringBuilder sb = new System.Text.StringBuilder();
        //    if (number < 0)
        //    {
        //        sb.Append("Minus ");
        //        number = -number;
        //    }
        //    string[] words0 = { "", "One ", "Two ", "Three ", "Four ", "Five ", "Six ", "Seven ", "Eight ", "Nine " };
        //    string[] words1 = { "Ten ", "Eleven ", "Twelve ", "Thirteen ", "Fourteen ", "Fifteen ", "Sixteen ", "Seventeen ", "Eighteen ", "Nineteen " };
        //    string[] words2 = { "Twenty ", "Thirty ", "Forty ", "Fifty ", "Sixty ", "Seventy ", "Eighty ", "Ninety " };
        //    string[] words3 = { "Thousand ", "Lakh ", "Crore " };
        //    num[0] = number % 1000; // units
        //    num[1] = number / 1000;
        //    num[2] = number / 100000;
        //    num[1] = num[1] - 100 * num[2]; // thousands
        //    num[3] = number / 10000000; // crores
        //    num[2] = num[2] - 100 * num[3]; // lakhs
        //    for (int i = 3; i > 0; i--)
        //    {
        //        if (num[i] != 0)
        //        {
        //            first = i;
        //            break;
        //        }
        //    }
        //    for (int i = first; i >= 0; i--)
        //    {
        //        if (num[i] == 0) continue;
        //        u = num[i] % 10; // ones
        //        t = num[i] / 10;
        //        h = num[i] / 100; // hundreds
        //        t = t - 10 * h; // tens
        //        if (h > 0) sb.Append(words0[h] + "Hundred ");
        //        if (u > 0 || t > 0)
        //        {
        //            if (h > 0 || i == 0) sb.Append("and ");
        //            if (t == 0)
        //                sb.Append(words0[u]);
        //            else if (t == 1)
        //                sb.Append(words1[u]);
        //            else
        //                sb.Append(words2[t - 2] + words0[u]);
        //        }
        //        if (i != 0) sb.Append(words3[i - 1]);
        //    }

        //    if (paisaamt == 0 && paisaconversion == false)
        //    {
        //        sb.Append("ruppes only");
        //    }
        //    else if (paisaamt > 0)
        //    {
        //        var paisatext = words(Convert.ToDouble(paisaamt), true);
        //        sb.AppendFormat("rupees {0} paise only", paisatext);
        //    }
        //    return sb.ToString().TrimEnd();
        //}
        #endregion
        #region Profile
        [HttpGet]
        public new ActionResult Profile()
        {
            string userid = User.Identity.GetUserId();
            var userDetails = _db.Users.SingleOrDefault(a => a.UserId == userid);
            var MD = _db.Whitelabel_Superstokist_details.FirstOrDefault(m => m.SSId == userid);
            ViewBag.MD_Details = MD;
            var gt = _db.State_Desc.SingleOrDefault(a => a.State_id == MD.State)?.State_name;
            ViewBag.ddlstate = gt;
            var cities = _db.District_Desc.SingleOrDefault(c => c.Dist_id == MD.District && c.State_id == MD.State)?.Dist_Desc;
            ViewBag.district = cities;

            ViewBag.Sate = _db.State_Desc.Select(a => new SelectListItem { Text = a.State_name, Value = a.State_id.ToString() }).ToList();
            ViewBag.ALLDistrict = _db.District_Desc.Where(a => a.State_id == MD.State).Select(a => new SelectListItem { Text = a.Dist_Desc, Value = a.Dist_id.ToString() }).ToList();

            ViewData["msg"] = TempData["success"];
            TempData.Remove("success");
            return View(userDetails);
        }
        public JsonResult ShowMasterprofile(string SSID)
        {
            var ch = _db.Whitelabel_Superstokist_details.Where(m => m.SSId == SSID).ToList();
            return Json(ch.ToArray(), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult UpdateMasterProfile(string txtid1, string txtfrimname, string txtcity, string txtaddress, int txtzipcode, int State, int District)
        {
            try
            {
                var ad = _db.Whitelabel_Superstokist_details.Where(a => a.SSId == txtid1).SingleOrDefault();
                ad.city = string.IsNullOrWhiteSpace(txtcity) ? ad.city : txtcity;
                ad.FarmName = string.IsNullOrWhiteSpace(txtfrimname) ? ad.FarmName : txtfrimname;
                ad.Address = string.IsNullOrWhiteSpace(txtaddress) ? ad.Address : txtaddress;
                ad.Pincode = txtzipcode;
                ad.State = State;
                ad.District = District;
                _db.SaveChanges();
            }
            catch
            {

            }
            TempData["success"] = "Update Successfully.";
            return RedirectToAction("Profile");
        }
        public JsonResult Showmobileandpancardprofile(string SSID)
        {
            var ch = _db.Whitelabel_Superstokist_details.Where(a => a.SSId == SSID).ToList();
            return Json(ch.ToArray(), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult UpdatePanccardandmobile(string txtid2, string txtname, string txtaadhaarcard, string txtpancard, string txtgst, string ddlPosition, string ddlBusinessType)
        {
            try
            {
                var ad = _db.Whitelabel_Superstokist_details.Where(a => a.SSId == txtid2).SingleOrDefault();
                ad.SuperstokistName = string.IsNullOrWhiteSpace(txtname) ? ad.SuperstokistName : txtname;
                ad.adharcard = string.IsNullOrWhiteSpace(txtaadhaarcard) ? ad.adharcard : txtaadhaarcard;
                ad.pancard = string.IsNullOrWhiteSpace(txtpancard) ? ad.pancard : txtpancard;
                ad.gst = string.IsNullOrWhiteSpace(txtgst) ? ad.gst : txtgst;
                ad.Position = string.IsNullOrWhiteSpace(ddlPosition) ? ad.Position : ddlPosition;
                ad.BusinessType = string.IsNullOrWhiteSpace(ddlBusinessType) ? ad.BusinessType : ddlBusinessType;
                _db.SaveChanges();
            }
            catch
            {

            }
            TempData["success"] = "Update Successfully.";
            return RedirectToAction("Profile");
        }

        //Upload Aadhaar Crad Doc
        [HttpPost]
        public ActionResult UploadAadharcarddoc(string txtaadharid)
        {
            try
            {
                WebImage photo = null;
                WebImage backphoto = null;
                var newFileName = "";
                var imagePath = "";
                var imagePath2 = "";

                if (Request.HttpMethod == "POST")
                {
                    photo = WebImage.GetImageFromRequest("file");
                    backphoto = WebImage.GetImageFromRequest("file1");
                    if (photo == null)
                    {
                        TempData["Warning"] = "Please Upload Front Aadhar Card.";
                    }
                    if (backphoto == null)
                    {
                        TempData["Warning"] = "Please Upload Back Aadhar Card.";
                    }

                    if (photo != null && backphoto != null)
                    {
                        newFileName = Guid.NewGuid().ToString() + "_" +
                       Path.GetFileName(photo.FileName);
                        imagePath = @"\Retailer_image\" + newFileName;

                        photo.Save(@"~\" + imagePath);

                        newFileName = Guid.NewGuid().ToString() + "_" +
                     Path.GetFileName(backphoto.FileName);
                        imagePath2 = @"\Retailer_image\" + newFileName;

                        backphoto.Save(@"~\" + imagePath2);
                    }
                    else
                    {
                        TempData["Warning"] = "Please Upload Front And Back Aadhar Card.";
                        return RedirectToAction("Profile");
                    }
                }
                var ad = _db.Whitelabel_Superstokist_details.Where(a => a.SSId == txtaadharid).SingleOrDefault();
                ad.aadharcardPath = string.IsNullOrWhiteSpace(imagePath) ? ad.aadharcardPath : imagePath;
                ad.BackSideAadharcardphoto = string.IsNullOrWhiteSpace(imagePath2) ? ad.BackSideAadharcardphoto : imagePath2;
                _db.SaveChanges();
                TempData["success"] = "Aadhaar Doucument Upload Successfully.";
                return RedirectToAction("Profile");
            }
            catch (Exception ex)
            {
                TempData["Warning"] = ex.Message;
            }
            return RedirectToAction("Profile");
        }

        //Upload Pancard Crad Doc
        [HttpPost]
        public ActionResult UploadPancardcarddoc(string txtpancardid)
        {
            try
            {
                WebImage photo = null;
                var newFileName = "";
                var imagePath = "";

                if (Request.HttpMethod == "POST")
                {
                    photo = WebImage.GetImageFromRequest();
                    if (photo != null)
                    {
                        newFileName = Guid.NewGuid().ToString() + "_" +
                       Path.GetFileName(photo.FileName);
                        imagePath = @"\Retailer_image\" + newFileName;

                        photo.Save(@"~\" + imagePath);
                    }
                }
                var ad = _db.Whitelabel_Superstokist_details.Single(a => a.SSId == txtpancardid);
                ad.pancardPath = string.IsNullOrWhiteSpace(imagePath) ? ad.pancardPath : imagePath;
                _db.SaveChanges();
                TempData["success"] = "Pancard Doucument Upload Successfully.";
                return RedirectToAction("Profile");
            }
            catch
            {

            }
            return View();
        }

        //Upload Service Aggrement Card Doc
        [HttpPost]
        public ActionResult UploadServiceAggrementdoc(string txtserviceid)
        {
            try
            {
                WebImage photo = null;
                var newFileName = "";
                var imagePath = "";

                if (Request.HttpMethod == "POST")
                {
                    photo = WebImage.GetImageFromRequest();
                    if (photo != null)
                    {
                        newFileName = Guid.NewGuid().ToString() + "_" +
                       Path.GetFileName(photo.FileName);
                        imagePath = @"\Retailer_image\" + newFileName;

                        photo.Save(@"~\" + imagePath);
                    }
                }
                var ad = _db.Whitelabel_Superstokist_details.Single(a => a.SSId == txtserviceid);
                ad.serviceagreementpath = string.IsNullOrWhiteSpace(imagePath) ? ad.serviceagreementpath : imagePath;
                _db.SaveChanges();
                TempData["success"] = "Service Agreement Doucument Upload Successfully.";
                return RedirectToAction("Profile");
            }
            catch
            {

            }
            return View();
        }

        //Upload Registraction Certificate Card Doc
        [HttpPost]
        public ActionResult UploadRegistractionCertificatedoc(string txtRegistractionid)
        {
            try
            {
                WebImage photo = null;
                var newFileName = "";
                var imagePath = "";

                if (Request.HttpMethod == "POST")
                {
                    photo = WebImage.GetImageFromRequest();
                    if (photo != null)
                    {
                        newFileName = Guid.NewGuid().ToString() + "_" +
                       Path.GetFileName(photo.FileName);
                        imagePath = @"\Retailer_image\" + newFileName;

                        photo.Save(@"~\" + imagePath);
                    }
                }
                var ad = _db.Whitelabel_Superstokist_details.Single(a => a.SSId == txtRegistractionid);
                ad.Registractioncertificatepath = string.IsNullOrWhiteSpace(imagePath) ? ad.Registractioncertificatepath : imagePath;
                _db.SaveChanges();
                TempData["success"] = " Registraction Certificate Document Upload Successfully.";
                return RedirectToAction("Profile");
            }
            catch
            {

            }
            return View();
        }

        //Upload Registraction Certificate Card Doc
        [HttpPost]
        public ActionResult UploadAddressProofdoc(string txtAddressproofid)
        {
            try
            {
                WebImage photo = null;
                var newFileName = "";
                var imagePath = "";

                if (Request.HttpMethod == "POST")
                {
                    photo = WebImage.GetImageFromRequest();
                    if (photo != null)
                    {
                        newFileName = Guid.NewGuid().ToString() + "_" +
                       Path.GetFileName(photo.FileName);
                        imagePath = @"\Retailer_image\" + newFileName;

                        photo.Save(@"~\" + imagePath);
                    }
                }
                var ad = _db.Whitelabel_Superstokist_details.Single(a => a.SSId == txtAddressproofid);
                ad.AddressProofpath = string.IsNullOrWhiteSpace(imagePath) ? ad.AddressProofpath : imagePath;
                _db.SaveChanges();
                TempData["success"] = "Address Proof Document Upload Successfully.";
                return RedirectToAction("Profile");
            }
            catch
            {

            }
            return View();
        }
        //Upload Profile Image
        [HttpPost]
        public ActionResult UploadProfileimage(string txtprofileid)
        {
            try
            {
                WebImage photo = null;
                var newFileName = "";
                var imagePath = "";

                if (Request.HttpMethod == "POST")
                {
                    photo = WebImage.GetImageFromRequest();
                    if (photo != null)
                    {
                        newFileName = Guid.NewGuid().ToString() + "_" +
                       Path.GetFileName(photo.FileName);
                        imagePath = @"\Retailer_image\" + newFileName;

                        photo.Save(@"~\" + imagePath);
                    }
                }
                var ad = _db.Whitelabel_Superstokist_details.Single(a => a.SSId == txtprofileid);
                ad.Photo = string.IsNullOrWhiteSpace(imagePath) ? ad.Photo : imagePath;
                _db.SaveChanges();
                TempData["success"] = "Profile Image Upload Successfully.";
                return RedirectToAction("Profile");
            }
            catch
            {

            }
            return View();
        }
        //delete Profile Doc 
        public JsonResult DelereprofileDoc(string SSID, string Docname)
        {
            if (SSID != null && Docname == "Aadhaar")
            {
                var ad = _db.Whitelabel_Superstokist_details.Single(a => a.SSId == SSID);
                ad.aadharcardPath = null;
                _db.SaveChanges();
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            if (SSID != null && Docname == "Pancard")
            {
                var ad = _db.Whitelabel_Superstokist_details.Single(a => a.SSId == SSID);
                ad.pancardPath = null;
                _db.SaveChanges();
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            if (SSID != null && Docname == "ServiceAgrrement")
            {
                var ad = _db.Whitelabel_Superstokist_details.Single(a => a.SSId == SSID);
                ad.serviceagreementpath = null;
                _db.SaveChanges();
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            if (SSID != null && Docname == "RegistractionCertificate")
            {
                var ad = _db.Whitelabel_Superstokist_details.Single(a => a.SSId == SSID);
                ad.Registractioncertificatepath = null;
                _db.SaveChanges();
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            if (SSID != null && Docname == "AddressProof")
            {
                var ad = _db.Whitelabel_Superstokist_details.Single(a => a.SSId == SSID);
                ad.AddressProofpath = null;
                _db.SaveChanges();
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("Failed", JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult FillDistict(int State)
        {
            var cities = _db.District_Desc.Where(c => c.State_id == State);
            return Json(cities, JsonRequestBehavior.AllowGet);
        }
        #endregion

        //public ActionResult TargetHistory()
        //{
        //    return View();
        //}
        #region BankInfo
        public JsonResult ShowBankinfo(string SSID)
        {
            var ch = _db.Whitelabel_Superstokist_details.Where(a => a.SSId == SSID).ToList();
            return Json(ch.ToArray(), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult UpdateBankinfromation(string txtid3, string txtaccholder, string txtbankaccountno, string txtifsc, string txtbankname, string txtbranchaddress)
        {
            try
            {
                var ad = _db.Whitelabel_Superstokist_details.Single(a => a.SSId == txtid3);
                ad.accountholder = string.IsNullOrWhiteSpace(txtaccholder) ? "" : txtaccholder;
                ad.Bankaccountno = string.IsNullOrWhiteSpace(txtbankaccountno) ? "" : txtbankaccountno;
                ad.Ifsccode = string.IsNullOrWhiteSpace(txtifsc) ? "" : txtifsc;
                ad.bankname = string.IsNullOrWhiteSpace(txtbankname) ? "" : txtbankname;
                ad.bankAddress = string.IsNullOrWhiteSpace(txtbranchaddress) ? "" : txtbranchaddress;
                _db.SaveChanges();
            }
            catch
            {

            }
            TempData["success"] = "Update Successfully.";
            return RedirectToAction("Profile");
        }
        #endregion
        #region Manage Password 
        [HttpGet]
        public ActionResult ChangePassword()
        {

            return View();
        }
        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }

                TempData["Message"] = "Your Password has been Changed Successfully..";
                return RedirectToAction("ChangePassword");
            }
            AddErrors(result);
            return View(model);
        }

        #endregion
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
        [HttpPost]
        public ActionResult InsertDealer(WMASTER.Models.WDealerModel model)
        {
            try
            {
                var Token = string.Empty;
                Token = GetToken();

                var newlycreatedUserId = string.Empty;

                var client = new RestClient(WapiBaseUrl.GetBaseUrl() + "/api/master/createDealer");
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Authorization", "bearer " + Token + "");
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", "{\r\n  \"Dealerid\": \"" + newlycreatedUserId + "\",\r\n  \"Name\": \"" + model.Name + "\",\r\n  \"State\": \"" + model.State + "\",\r\n  \"Firm\": \"" + model.Firm + "\",\r\n  \"District\": \"" + model.District + "\",\r\n  \"Mobile\": \"" + model.Mobile + "\",\r\n  \"Email\": \"" + model.Email + "\",\r\n  \"Pan\": \"" + model.Pan + "\",\r\n  \"Adhaar\": \"" + model.Adhaar + "\",\r\n  \"Gst\": \"" + model.Gst + "\",\r\n  \"Address\": \"" + model.Address + "\",\r\n  \"Pincode\": \"" + model.Pincode + "\"\r\n}", ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);

                var statusCode = response.StatusCode.ToString();
                if (statusCode == "Unauthorized")
                {
                    TempData.Remove("data");
                    AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                    return RedirectToAction("Index", "Home", null);
                }

                var responsechk = response.Content.ToString();

                dynamic stuff = Newtonsoft.Json.JsonConvert.DeserializeObject(responsechk);
                string ch = stuff.Message;
                TempData["msgrem"] = ch;
                return RedirectToAction("Index");
            }
            catch
            {
                TempData["Error"] = "User Not Created. Please Create After Some Time.";
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public ActionResult Edit_Distibutor_user(WMASTER.Models.WDealerModel dlm)
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                //Dealer_Details objCourse = (from p in _db.Dealer_Details
                //                            where p.DealerId == dlm.Dealerid
                //                            select p).SingleOrDefault();
                //objCourse.Address = dlm.Address1;
                //objCourse.adharcard = dlm.Adhaar1;
                //objCourse.DealerName = dlm.Name1;
                //objCourse.District = Convert.ToInt32(dlm.District1);
                //objCourse.FarmName = dlm.Firm1;
                //objCourse.gst = dlm.Gst1;
                //objCourse.pancard = dlm.Pan1;
                //objCourse.Pincode = Convert.ToInt32(dlm.Pincode1);
                ////objCourse.slab_name = dlm.Slab1;
                //objCourse.State = Convert.ToInt32(dlm.State1);
                //_db.SaveChanges();
                return RedirectToAction("Index");
            }
        }
        //POST : Delaer Search
        [HttpPost]
        public ActionResult DealerSearch(string userid)
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                var ch = _db.Dealer_Details.Where(aa => aa.DealerId == userid).ToList();
                return Json(ch.ToArray(), JsonRequestBehavior.AllowGet);
            }
        }

        //fill District 
        public JsonResult DistrictList(int Id)
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                var district = from s in _db.District_Desc
                               where s.State_id == Id
                               select s;
                ViewBag.DistrictList = new SelectList(district, "State_id", "");
                return Json(new SelectList(district.ToArray(), "Dist_id", "Dist_Desc"), JsonRequestBehavior.AllowGet);
            }
        }
        //Start Slab Setting 
        #region SlabSetting
        //GET : Show Slab Name 
        public ActionResult generateSlab()
        {
            ADMIN.Models.ResultSetViewModel viewModel = new ADMIN.Models.ResultSetViewModel();
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                var userid = User.Identity.GetUserId();
                viewModel.ResultSet = _db.Slab_name.Where(aa => aa.createdby == userid).Select(aaa => new ADMIN.Models.Slab_name_model
                {
                    cdate = aaa.cdate,
                    createdby = aaa.createdby,
                    idno = aaa.idno,
                    SlabFor = aaa.SlabFor,
                    SlabName = aaa.SlabName
                }).ToList();
                List<SelectListItem> items = new List<SelectListItem>();
                items.Add(new SelectListItem
                {
                    Text = "Distributor",
                    Value = "Distributor"
                });
                ViewData["msg"] = TempData["msg"];
                ViewBag.slabfor = items;
                return View(viewModel);
            }
        }
        // Post : Add New Slab
        [HttpPost]
        public ActionResult AddSlabname(ADMIN.Models.ResultSetViewModel result)
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                var userid = User.Identity.GetUserId();
                try
                {
                    System.Data.Entity.Core.Objects.ObjectParameter output = new
                      System.Data.Entity.Core.Objects.ObjectParameter("Output", typeof(string));
                    var msg = _db.insert_slab_list("Distributor", result.AccountVM.Slab_Name, userid, output).Single().slab;
                    TempData["msg"] = msg;
                    return RedirectToAction("generateSlab");
                }
                catch
                {
                    return RedirectToAction("generateSlab");
                }
            }
        }

        public ActionResult Delete_slabName(int id, string slabfor, string slabname)
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                if (slabfor == "Distributor")
                {
                    var userid = User.Identity.GetUserId();
                    var disresult = _db.Dealer_Details.Where(p => p.slab_name == slabname).ToList();
                    if (disresult.Count > 0)
                    {
                        TempData["api"] = "This slab is Already Assign To Distributor User..";
                    }
                    else
                    {
                        var msg = _db.Delete_slab(slabfor, slabname, userid);
                        TempData["successmsg"] = "Slab Deteted Successfully..";
                    }
                }
                return RedirectToAction("generateSlab");
            }
        }

        #endregion
        //End Slab Setting

        #region Account

        public ActionResult SendFund()
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                string masterid = User.Identity.GetUserId();
                var msg = TempData["result"];
                ViewData["output"] = msg;
                string txt_frm_date = DateTime.Now.ToString();
                string txt_to_date = DateTime.Now.ToString();
                string frm_date = Convert.ToDateTime(txt_frm_date).Date.ToString("yyyy-MM-dd");
                string to_date = Convert.ToDateTime(txt_to_date).AddDays(1).ToString("yyyy-MM-dd");


                var stands = (from dlm in _db.Dealer_Details where dlm.SSId == masterid select dlm).ToList();
                IEnumerable<SelectListItem> selectList = from s in stands
                                                         select new SelectListItem
                                                         {
                                                             Value = s.DealerId,
                                                             Text = s.Email + "--" + s.DealerName.ToString()
                                                         };
                ViewBag.DealerId = new SelectList(selectList, "Value", "Text");
                ViewBag.DealerId1 = new SelectList(selectList, "Value", "Text");
                var ch = _db.select_admin_to_Dealer(masterid, "ALL", Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
                return View(ch);
            }
        }
        [HttpPost]
        public ActionResult SendFund(string txt_frm_date, string txt_to_date, string DealerId1)
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                ViewBag.chk = "post";
                if (DealerId1 == "")
                {
                    DealerId1 = "ALL";
                }
                var userid = User.Identity.GetUserId();
                var stands = (from dlm in _db.Dealer_Details where dlm.SSId == userid select dlm).ToList();
                IEnumerable<SelectListItem> selectList = from s in stands
                                                         select new SelectListItem
                                                         {
                                                             Value = s.DealerId,
                                                             Text = s.Email + "--" + s.DealerName.ToString()
                                                         };
                ViewBag.DealerId = new SelectList(selectList, "Value", "Text");


                ViewBag.DealerId1 = new SelectList(selectList, "Value", "Text");

                DateTime frm = Convert.ToDateTime(txt_frm_date);
                DateTime to = Convert.ToDateTime(txt_to_date);
                txt_frm_date = frm.ToString("dd-MM-yyyy");
                txt_to_date = to.ToString("dd-MM-yyyy");
                string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy",
                    "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
                DateTime dt = !string.IsNullOrWhiteSpace(txt_frm_date) ? DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
                DateTime dt1 = !string.IsNullOrWhiteSpace(txt_to_date) ? DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
                DateTime frm_date = Convert.ToDateTime(dt).Date;
                DateTime to_date = Convert.ToDateTime(dt1).Date.AddDays(1);
                //
                var ch = _db.select_admin_to_Dealer(userid, DealerId1, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();

                return View(ch);
            }
        }

        [HttpPost]
        public ActionResult master_to_Dealer_bal(string DealerId, string balance, string ddl_fund_type, string comment)
        {
            try
            {
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    string masterid = User.Identity.GetUserId();
                    var email = _db.Users.Where(p => p.UserId == masterid).Single().Email;
                    var useremail = _db.Users.Where(p => p.UserId == DealerId).Single().Email;
                    var wid = _db.whitelabel_Dealer_Details.Where(p => p.DealerId == DealerId).Single().Whitelabelid;
                    var diff1 = (_db.Whitelabel_admin_to_dealer.Where(aa => aa.dealerid == DealerId && aa.balance_from == masterid).OrderByDescending(aa => aa.idno).Select(c => c.cr).FirstOrDefault());
                    diff1 = diff1 ?? 0;
                    var ch = "";
                    var tp = "";
                    decimal diff = Convert.ToDecimal(diff1);
                    System.Data.Entity.Core.Objects.ObjectParameter output = new System.Data.Entity.Core.Objects.ObjectParameter("output", typeof(String));
                    decimal amount = Convert.ToDecimal(balance);
                    if (amount > 0)
                    {
                        if (ddl_fund_type == "Cash" || ddl_fund_type == "Credit")
                        {
                            ch = _db.spWhitelabel_Insert_SuperStokist_To_Dealer(wid, masterid, DealerId, amount, 0, ddl_fund_type, comment, "", "", "", "", "", output).Single().msg;
                        }
                        var AdminDetails = _db.WhiteLabel_userList.Single(s => s.WhiteLabelID == wid);
                        if (ch == "Balance Transfer SuccessFully.")
                        {
                            var diff2 = (_db.Whitelabel_admin_to_dealer.Where(aa => aa.dealerid == DealerId && aa.balance_from == masterid).OrderByDescending(aa => aa.idno).Select(c => c.cr).FirstOrDefault());
                            diff2 = diff2 ?? 0;
                            var remaindealer = _db.Whitelabel_Remain_dealer_balance.Where(p => p.DealerID == DealerId).Single().Remainamount;
                            var statusMaster = _db.PushNotificationStatus.Where(a => a.UserRole == "WMASTER").SingleOrDefault().Status;
                            var statusDealer = _db.PushNotificationStatus.Where(a => a.UserRole == "Distributor").SingleOrDefault().Status;
                            var statusSendSmsMasterToDlmFundTransfer = _db.SMSSendAlls.Where(a => a.ServiceName == "MDToDLMdlmFundTrans").SingleOrDefault().Status;
                            var statusSendMailMasterToDlmFundTransfer = _db.EmailSendAlls.Where(a => a.ServiceName == "MDToDLMdlmFundTrans1").SingleOrDefault().Status;

                            var DealerDetails = _db.whitelabel_Dealer_Details.Where(p => p.DealerId == DealerId).Single();
                            var statusSendSmsMasterToDlmFundTransferMaster = _db.SMSSendAlls.Where(a => a.ServiceName == "MDToDLMMDFundTrans").SingleOrDefault().Status;
                            var statusSendMailMasterToDlmFundTransferMaster = _db.EmailSendAlls.Where(a => a.ServiceName == "MDToDLMMDFundTrans1").SingleOrDefault().Status;

                            var MasterDetails = _db.Whitelabel_Superstokist_details.Where(p => p.SSId == masterid).Single();
                            if (ddl_fund_type == "Credit")
                            {
                                smssend.sms_init_whitelabel(wid, statusSendSmsMasterToDlmFundTransfer, "N", "SEND_SMS_CREDIT_RECEIVEDBY", DealerDetails.Mobile, email, amount, remaindealer, diff2);

                                smssend.sms_init_whitelabel(wid, statusSendSmsMasterToDlmFundTransferMaster, "N", "SEND_SMS_CREDIT_TRANSFERREDTO", MasterDetails.Mobile, useremail, amount, diff2);

                                if (statusSendMailMasterToDlmFundTransfer == "Y")
                                {
                                    smssend.SendEmailAll(DealerDetails.Email, "Credit Received by " + email + " Rs." + amount + ".New Balance is " + remaindealer + ".Your O/s Credit is " + diff2 + "", "Fund Transfer", AdminDetails.EmailId);
                                }
                                if (statusSendSmsMasterToDlmFundTransferMaster == "Y")
                                {
                                    smssend.SendEmailAll(MasterDetails.Email, "Credit Transferred To " + useremail + " Rs. " + amount + ".Total Credit(O/s) Balance is " + diff1 + "", "Fund by WMASTER To CC", AdminDetails.EmailId);
                                }

                                notify.sendmessage(useremail, "Credit Received Rs." + amount + ".New Balance is " + remaindealer + ".Your O/s Credit is " + diff2 + "");
                            }
                            else
                            {
                                var mastername = _db.Whitelabel_Superstokist_details.Where(p => p.SSId == masterid).Single().SuperstokistName;
                                var dealername = _db.whitelabel_Dealer_Details.Where(p => p.DealerId == DealerId).Single().DealerName;

                                smssend.sms_init_whitelabel(wid, statusSendSmsMasterToDlmFundTransfer, "N", "FUNDTRANSFERMESSAGE_CASH", DealerDetails.Mobile, amount, mastername, remaindealer, diff2);

                                smssend.sms_init_whitelabel(wid, statusSendSmsMasterToDlmFundTransferMaster, "N", "SENDSMSMASTERTODLMFUNDTRANSFERMSGCASH", MasterDetails.Mobile, amount, dealername, diff2);

                                if (statusSendMailMasterToDlmFundTransfer == "Y")
                                {
                                    smssend.SendEmailAll(DealerDetails.Email, "Cash Paid Rs." + amount + " to " + mastername + ". New Balance is " + remaindealer + ". Your O/s Credit is " + diff2 + "", "Fund Transfer", AdminDetails.EmailId);
                                }
                                if (statusSendMailMasterToDlmFundTransferMaster == "Y")
                                {
                                    smssend.SendEmailAll(MasterDetails.Email, "Cash Recived Rs." + amount + " From " + dealername + ",his O/s Credit is " + diff2 + "", "Fund by WMASTER To CC", AdminDetails.EmailId);
                                }
                                //if (statusMaster == "Y")
                                //{
                                //    SendPushNotification(email, Url.Action("SendFund", "Home"), "Cash Recived Rs." + amount + " From " + dealername + ",his O/s Credit is " + diff2 + "", "Fund Transfer ..");
                                //}
                                //if (statusDealer == "Y")
                                //{
                                //    SendPushNotification(useremail, Websitename + "/DEALER/Home/ReceiveFund", "Cash Paid Rs." + amount + " to " + mastername + ". New Balance is " + remaindealer + ". Your O/s Credit is " + diff2 + "", "Fund Transfer ..");
                                //}
                                notify.sendmessage(useremail, "Cash Paid Rs." + amount + " to " + mastername + ". New Balance is " + remaindealer + ". Your O/s Credit is " + diff2 + "");
                            }
                            TempData["result"] = ch;
                            tp = "success";
                            TempData["sts"] = tp;
                        }
                    }
                    else
                    {
                        TempData["result"] = "Amount should be not zero";
                        tp = "error";
                    }
                    return RedirectToAction("SendFund");
                }
            }
            catch
            {
                return RedirectToAction("SendFund");
            }
        }



        public ActionResult FUNDTRANSFER()
        {
            FundTransferViewModel vmodel = new FundTransferViewModel();
            string masterid = User.Identity.GetUserId();
            var msg = TempData["result"];
            ViewData["output"] = msg;
            string txt_frm_date = DateTime.Now.ToString();
            string txt_to_date = DateTime.Now.ToString();
            string frm_date = Convert.ToDateTime(txt_frm_date).Date.ToString("yyyy-MM-dd");
            string to_date = Convert.ToDateTime(txt_to_date).AddDays(1).ToString("yyyy-MM-dd");

            var masterDetails = _db.Whitelabel_Superstokist_details.Where(a => a.SSId == masterid).SingleOrDefault();

            var stands = (from dlm in _db.whitelabel_Dealer_Details where dlm.masterid == masterid select dlm).ToList();
            IEnumerable<SelectListItem> selectList = from s in stands
                                                     select new SelectListItem
                                                     {
                                                         Value = s.DealerId,
                                                         Text = s.FarmName + "--" + s.Email.ToString()
                                                     };
            ViewBag.DealerId = new SelectList(selectList, "Value", "Text");

            ViewBag.DealerId1 = new SelectList(selectList, "Value", "Text");
            vmodel.mastertodlmlist = _db.spWhitelabel_select_admin_to_Dealer(masterDetails.Whitelabelid, masterid, "ALL", Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            List<SelectListItem> items = new List<SelectListItem>();
            foreach (var item in stands)
            {
                items.Add(new SelectListItem { Text = item.FarmName + " / " + item.Mobile, Value = item.DealerId.ToString() });
            }
            vmodel.ddldealers = items;
            var bindbank = _db.Whitelabel_bank_info.Where(x => x.userid == masterid).ToList();
            List<SelectListItem> bankitem = new List<SelectListItem>();
            foreach (var bank in bindbank)
            {
                bankitem.Add(new SelectListItem { Text = bank.banknm + " / " + bank.holdername, Value = bank.acno });
            }
            vmodel.ddlFillAllBank = bankitem;
            var bindwallet = _db.Whitelabel_Wallet_info.Where(x => x.userid == masterid).ToList();
            List<SelectListItem> walletitem = new List<SelectListItem>();
            foreach (var wallet in bindwallet)
            {
                walletitem.Add(new SelectListItem { Text = wallet.walletname + " / " + wallet.walletholdername, Value = wallet.walletno });
            }
            vmodel.ddlFillAllwallet = walletitem;
            return View(vmodel);
        }
        public PartialViewResult MDTODealer(string tabtype = "MDTODLM", string txt_frm_date = "", string txt_to_date = "", string usernm = "")
        {


            if (string.IsNullOrEmpty(usernm))
            {
                usernm = "ALL";
            }
            DateTime fromdate;
            DateTime Todate;

            if (string.IsNullOrEmpty(txt_frm_date) && string.IsNullOrEmpty(txt_to_date))
            {
                fromdate = DateTime.Now.AddDays(-1);
                Todate = DateTime.Now.AddDays(1);
            }
            else
            {

                string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy",
                            "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };

                DateTime dt = !string.IsNullOrWhiteSpace(txt_frm_date) ? DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
                DateTime dt1 = !string.IsNullOrWhiteSpace(txt_to_date) ? DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
                fromdate = Convert.ToDateTime(dt).Date;
                Todate = Convert.ToDateTime(dt1).Date.AddDays(1);


            }


            FundTransferViewModel vmodel = new FundTransferViewModel();
            string masterid = User.Identity.GetUserId();
            var whitelabelid = _db.Whitelabel_Superstokist_details.Where(a => a.SSId == masterid).SingleOrDefault().Whitelabelid;

            switch (tabtype)
            {
                case "Dealer":
                    vmodel.funrequesttoadmin = _db.spWhitelabel_select_master_pur_order(whitelabelid, masterid, "ALL", fromdate, Todate).ToList();
                    vmodel.AdminCompanyname = _db.WhiteLabel_userList.Where(a => a.WhiteLabelID == whitelabelid).FirstOrDefault().FrmName;
                    break;

                case "Retailer":
                    vmodel.FundRequestRecived = _db.spWhitelabel_select_dlm_pur_order(whitelabelid, usernm, masterid, fromdate, Todate).ToList();
                    break;
                case "Recivefunddetails":
                    vmodel.FundRecievedDetails = _db.spWhitelabel_Select_balance_Super_stokist(whitelabelid, masterid, fromdate, Todate).ToList();
                    break;
                default:
                    vmodel.mastertodlmlist = _db.spWhitelabel_select_admin_to_Dealer(whitelabelid, masterid, usernm, fromdate, Todate).ToList();
                    break;

            }

            return PartialView("_FundTransferMasterToDlmPartial", vmodel);

        }
        [HttpPost]
        public ActionResult FundTransfermaster_to_Dealer_bal(string txtcode, string hdSuperstokistID, string hdMDDLM, string hdPaymentMode,
      string hdPaymentAmount, string hdMDDepositeSlipNo, string hdMDTransferType, string hdMDcollection, string hdMDComments,
      string hdMDBank, string hdsupraccno, string hdMDaccountno, string hdMDutrno, string hdMDwallet,
      string hdMDwalletno, string hdMDtransationno, string hdMDsettelment, string hdMDCreditDetail, string hdMDsubject)
        {
            try
            {
                var userid = User.Identity.GetUserId();
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    string superstockid = hdSuperstokistID;

                    string masterid = User.Identity.GetUserId();
                    userid = db.Whitelabel_Superstokist_details.Single(s => s.SSId == masterid).Whitelabelid;
                    // var dealeremail = db.Users.Where(p => p.UserId == DealerId).Single().Email;
                    string balance = hdPaymentAmount;
                    string type = hdPaymentMode;
                    string comment = hdMDComments;
                    var DealerId = hdMDDLM;
                    //string transferid = null;
                    //try
                    //{
                    //    transferid = TempData["transferMDtoDlm"].ToString();
                    //}
                    //catch (Exception ex)
                    //{
                    //    return Json("Please Try After SomeTime", JsonRequestBehavior.AllowGet);
                    //}
                    var counts = db.spWhitelabel_FundTransfercount(userid, masterid, hdMDDLM, type, Convert.ToDecimal(balance), "Admintodealer").SingleOrDefault().msg;
                    int msgcount = Convert.ToInt32(counts);
                    if (msgcount == 0)
                    {
                        msgcount = 60001;
                    }
                    int max_limit = 60000;
                    if (msgcount > max_limit)
                    {
                        // string utrno = hdMDcollection == null ? hdMDutrno : hdMDcollection;
                        string collectionby = hdMDcollection == null ? hdMDtransationno : hdMDcollection;
                        collectionby = collectionby == null ? hdMDutrno : collectionby;
                        collectionby = collectionby == null ? hdMDtransationno : collectionby;
                        collectionby = collectionby == null ? hdMDsettelment : collectionby;
                        collectionby = collectionby == null ? hdMDCreditDetail : collectionby;
                        collectionby = collectionby == null ? hdMDsubject : collectionby;
                        collectionby = collectionby == null ? hdMDDepositeSlipNo : collectionby;
                        string bankname = hdMDBank == null ? hdMDwallet : hdMDBank;
                        string adminacco = hdMDaccountno == null ? hdMDwalletno : hdMDaccountno;
                        //  adminacco = adminacco == null ? hdMDDepositeSlipNo : adminacco;
                        string DepositeSlipNo = hdMDDepositeSlipNo;
                        if (hdMDTransferType != null && hdPaymentMode == "Online Transfer")
                        {
                            type = hdPaymentMode + "/" + hdMDTransferType;
                        }


                        var email = db.Users.Where(p => p.UserId == masterid).Single().Email;
                        var useremail = db.Users.Where(p => p.UserId == DealerId).Single().Email;
                        var diff1 = (db.Whitelabel_admin_to_dealer.Where(aa => aa.dealerid == DealerId && aa.balance_from == masterid).OrderByDescending(aa => aa.idno).Select(c => c.cr).FirstOrDefault());
                        diff1 = diff1 ?? 0;
                        var ch = "";
                        var tp = "";
                        decimal diff = Convert.ToDecimal(diff1);
                        System.Data.Entity.Core.Objects.ObjectParameter output = new System.Data.Entity.Core.Objects.ObjectParameter("output", typeof(String));
                        decimal amount = Convert.ToDecimal(balance);
                        if (amount > 0)
                        {
                            // if (ddl_fund_type == "Cash" || ddl_fund_type == "Credit")
                            // {
                            ch = db.spWhitelabel_Insert_SuperStokist_To_Dealer(userid, masterid, DealerId, amount, 0, hdPaymentMode, comment, "Master", hdMDcollection, hdMDBank, hdMDaccountno, "Direct", output).Single().msg;
                            // }
                            var Admindetails = db.WhiteLabel_userList.Single(s => s.WhiteLabelID == userid);
                            if (ch == "Balance Transfer SuccessFully.")
                            {
                                var diff2 = (db.Whitelabel_admin_to_dealer.Where(aa => aa.dealerid == DealerId && aa.balance_from == masterid).OrderByDescending(aa => aa.idno).Select(c => c.cr).FirstOrDefault());
                                diff2 = diff2 ?? 0;
                                var remaindealer = db.Whitelabel_Remain_dealer_balance.Where(p => p.DealerID == DealerId).Single().Remainamount;
                                var statusMaster = db.PushNotificationStatus.Where(a => a.UserRole == "Master").SingleOrDefault().Status;
                                var statusDealer = db.PushNotificationStatus.Where(a => a.UserRole == "Distributor").SingleOrDefault().Status;
                                var statusSendSmsMasterToDlmFundTransfer = db.Whitelabel_SMSSendAll.Where(a => a.ServiceName == "MDToDLMdlmFundTrans" && a.Whitelabelid == userid).SingleOrDefault();
                                var statusSendMailMasterToDlmFundTransfer = db.Whitelabel_EmailSendAll.Where(a => a.ServiceName == "MDToDLMdlmFundTrans1" && a.Whitelabelid == userid).SingleOrDefault().Status;

                                var DealerDetails = db.whitelabel_Dealer_Details.Where(p => p.DealerId == DealerId).Single();
                                var statusSendSmsMasterToDlmFundTransferMaster = db.Whitelabel_SMSSendAll.Where(a => a.ServiceName == "MDToDLMMDFundTrans" && a.Whitelabelid == userid).SingleOrDefault();
                                var statusSendMailMasterToDlmFundTransferMaster = db.Whitelabel_EmailSendAll.Where(a => a.ServiceName == "MDToDLMMDFundTrans1" && a.Whitelabelid == userid).SingleOrDefault().Status;

                                var MasterDetails = db.Whitelabel_Superstokist_details.Where(p => p.SSId == masterid).Single();
                                if (hdMDTransferType == "Credit")
                                {
                                    //if (statusSendSmsMasterToDlmFundTransfer == "Y")
                                    //{
                                    //    try
                                    //    {
                                    //        string msgssss = "";
                                    //        string tempid = "";
                                    //        string urlss = "";

                                    //        var smsapionsts = db.apisms.Where(x => x.sts == "Y").SingleOrDefault();
                                    //        var smsstypes = db.Sending_SMS_Templates.Where(x => x.SMS_TYPE == "SEND_SMS_CREDIT_RECEIVEDBY" && x.SMSAPIID == smsapionsts.id).SingleOrDefault();
                                    //        if (smsstypes != null)
                                    //        {

                                    //            msgssss = string.Format(smsstypes.Templates, email, amount, remaindealer, diff2);
                                    //            tempid = smsstypes.Templateid;
                                    //            urlss = smsapionsts.smsapi;

                                    //            smssend.sendsmsallnew(DealerDetails.Mobile, msgssss, urlss, tempid);
                                    //        }
                                    //    }
                                    //    catch { }
                                    //    // smssend.sendsmsall(DealerDetails.Mobile, "Credit Received by " + email + " Rs." + amount + ".New Balance is " + remaindealer + ".Your O/s Credit is " + diff2 + "", "Fund Transfer");
                                    //}

                                    smssend.sms_init_whitelabel(userid, statusSendSmsMasterToDlmFundTransfer.Status, "N", "SEND_SMS_CREDIT_RECEIVEDBY", DealerDetails.Mobile, email, amount, remaindealer, diff2);

                                    //if (statusSendSmsMasterToDlmFundTransferMaster == "Y")
                                    //{

                                    //    try
                                    //    {
                                    //        string msgssss = "";
                                    //        string tempid = "";
                                    //        string urlss = "";

                                    //        var smsapionsts = db.apisms.Where(x => x.sts == "Y").SingleOrDefault();
                                    //        var smsstypes = db.Sending_SMS_Templates.Where(x => x.SMS_TYPE == "SEND_SMS_CREDIT_TRANSFERREDTO" && x.SMSAPIID == smsapionsts.id).SingleOrDefault();
                                    //        if (smsstypes != null)
                                    //        {

                                    //            msgssss = string.Format(smsstypes.Templates, useremail, amount, diff2);
                                    //            tempid = smsstypes.Templateid;
                                    //            urlss = smsapionsts.smsapi;

                                    //            smssend.sendsmsallnew(MasterDetails.Mobile, msgssss, urlss, tempid);
                                    //        }
                                    //    }
                                    //    catch { }
                                    //    //  smssend.sendsmsall(MasterDetails.Mobile, "Credit Transferred To " + useremail + " Rs. " + amount + ".Total Credit(O/s) Balance is " + diff1 + "", "Fund Transfer");
                                    //}

                                    smssend.sms_init_whitelabel(userid, statusSendSmsMasterToDlmFundTransferMaster.Status, "N", "SEND_SMS_CREDIT_TRANSFERREDTO", MasterDetails.Mobile, useremail, amount, diff2);

                                    if (statusSendMailMasterToDlmFundTransfer == "Y")
                                    {
                                        smssend.SendWhitelabelEmailAll(DealerDetails.Email, "Credit Received by " + email + " Rs." + amount + ".New Balance is " + remaindealer + ".Your O/s Credit is " + diff2 + "", "Fund Transfer", Admindetails.EmailId, userid);
                                    }
                                    if (statusSendMailMasterToDlmFundTransferMaster == "Y")
                                    {
                                        smssend.SendWhitelabelEmailAll(MasterDetails.Email, "Credit Transferred To " + useremail + " Rs. " + amount + ".Total Credit(O/s) Balance is " + diff1 + "", "Fund by Master To CC", Admindetails.EmailId, userid);
                                    }
                                    //if (statusMaster == "Y")
                                    //{
                                    //    SendPushNotification(email, Url.Action("SendFund", "Home"), "Credit Transferred Rs. " + amount + ".Total Credit(O/s) Balance is " + diff2 + "", "Fund Transfer ..");
                                    //}
                                    //if (statusDealer == "Y")
                                    //{
                                    //    SendPushNotification(useremail, Websitename + "/DEALER/Home/ReceiveFund", "Credit Received Rs." + amount + ".New Balance is " + remaindealer + ".Your O/s Credit is " + diff2 + "", "Fund Transfer ..");
                                    //}
                                    notify.sendmessage(useremail, "Credit Received Rs." + amount + ".New Balance is " + remaindealer + ".Your O/s Credit is " + diff2 + "");
                                }
                                else
                                {
                                    var mastername = db.Whitelabel_Superstokist_details.Where(p => p.SSId == masterid).Single().SuperstokistName;
                                    var dealername = db.whitelabel_Dealer_Details.Where(p => p.DealerId == DealerId).Single().DealerName;
                                    //if (statusSendSmsMasterToDlmFundTransfer == "Y")
                                    //{

                                    //    try
                                    //    {
                                    //        string msgssss = "";
                                    //        string tempid = "";
                                    //        string urlss = "";

                                    //        var smsapionsts = db.apisms.Where(x => x.sts == "Y").SingleOrDefault();
                                    //        var smsstypes = db.Sending_SMS_Templates.Where(x => x.SMS_TYPE == "FUNDTRANSFERMESSAGE_CASH" && x.SMSAPIID == smsapionsts.id).SingleOrDefault();
                                    //        if (smsstypes != null)
                                    //        {

                                    //            msgssss = string.Format(smsstypes.Templates, amount, mastername, remaindealer, diff2);
                                    //            tempid = smsstypes.Templateid;
                                    //            urlss = smsapionsts.smsapi;

                                    //            smssend.sendsmsallnew(MasterDetails.Mobile, msgssss, urlss, tempid);
                                    //        }
                                    //    }
                                    //    catch { }
                                    //    //  smssend.sendsmsall(DealerDetails.Mobile, "Cash Paid Rs." + amount + " to " + mastername + ". New Balance is " + remaindealer + ". Your O/s Credit is " + diff2 + "", "Fund Transfer");
                                    //}

                                    smssend.sms_init_whitelabel(userid, statusSendSmsMasterToDlmFundTransfer.Status, "N", "FUNDTRANSFERMESSAGE_CASH", MasterDetails.Mobile, amount, mastername, remaindealer, diff2);

                                    //if (statusSendSmsMasterToDlmFundTransferMaster == "Y")
                                    //{

                                    //    try
                                    //    {
                                    //        string msgssss = "";
                                    //        string tempid = "";
                                    //        string urlss = "";

                                    //        var smsapionsts = db.apisms.Where(x => x.sts == "Y").SingleOrDefault();
                                    //        var smsstypes = db.Sending_SMS_Templates.Where(x => x.SMS_TYPE == "SENDSMSMASTERTODLMFUNDTRANSFERMSGCASH" && x.SMSAPIID == smsapionsts.id).SingleOrDefault();
                                    //        if (smsstypes != null)
                                    //        {

                                    //            msgssss = string.Format(smsstypes.Templates, amount, dealername, remaindealer, diff2);
                                    //            tempid = smsstypes.Templateid;
                                    //            urlss = smsapionsts.smsapi;

                                    //            smssend.sendsmsallnew(MasterDetails.Mobile, msgssss, urlss, tempid);
                                    //        }
                                    //    }
                                    //    catch { }
                                    //    // smssend.sendsmsall(MasterDetails.Mobile, "Cash Recived Rs." + amount + " From " + dealername + ",his O/s Credit is " + diff2 + "", "Fund Transfer");
                                    //}

                                    smssend.sms_init_whitelabel(userid, statusSendSmsMasterToDlmFundTransferMaster.Status, "N", "SENDSMSMASTERTODLMFUNDTRANSFERMSGCASH", MasterDetails.Mobile, amount, dealername, remaindealer, diff2);

                                    if (statusSendMailMasterToDlmFundTransfer == "Y")
                                    {
                                        smssend.SendWhitelabelEmailAll(DealerDetails.Email, "Cash Paid Rs." + amount + " to " + mastername + ". New Balance is " + remaindealer + ". Your O/s Credit is " + diff2 + "", "Fund Transfer", Admindetails.EmailId, userid);
                                    }
                                    if (statusSendMailMasterToDlmFundTransferMaster == "Y")
                                    {
                                        smssend.SendWhitelabelEmailAll(MasterDetails.Email, "Cash Recived Rs." + amount + " From " + dealername + ",his O/s Credit is " + diff2 + "", "Fund by Master To CC", Admindetails.EmailId, userid);
                                    }
                                    //if (statusMaster == "Y")
                                    //{
                                    //    SendPushNotification(email, Url.Action("SendFund", "Home"), "Cash Recived Rs." + amount + " From " + dealername + ",his O/s Credit is " + diff2 + "", "Fund Transfer ..");
                                    //}
                                    //if (statusDealer == "Y")
                                    //{
                                    //    SendPushNotification(useremail, Websitename + "/DEALER/Home/ReceiveFund", "Cash Paid Rs." + amount + " to " + mastername + ". New Balance is " + remaindealer + ". Your O/s Credit is " + diff2 + "", "Fund Transfer ..");
                                    //}
                                    notify.sendmessage(useremail, "Cash Paid Rs." + amount + " to " + mastername + ". New Balance is " + remaindealer + ". Your O/s Credit is " + diff2 + "");
                                }
                                TempData["result"] = ch;
                                tp = "success";
                                TempData["sts"] = tp;
                            }
                        }
                        else
                        {
                            tp = "error";
                            ch = "Amount should be not zero";
                        }
                        return Json(ch, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json("Please Try After 1 min ", JsonRequestBehavior.AllowGet);
                    }

                }
            }
            catch (Exception ex)
            {
                VastwebmultiEntities db = new VastwebmultiEntities();
                test1 t1 = new test1();
                t1.name = String.Format("M2D fund Error Message: {0}, Stack: {1}", ex.Message, ex.StackTrace);
                db.test1.Add(t1);
                db.SaveChanges();
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
                // return RedirectToAction("SendFund");
            }
        }

        public ActionResult D_Creditchk(string dealerid)
        {
            string masterid = User.Identity.GetUserId();
            var ch = (_db.Whitelabel_to_Dealer.Where(aa => aa.dealerid == dealerid && aa.balance_from == masterid).OrderByDescending(aa => aa.idno).Select(c => c.cr).FirstOrDefault());
            ch = ch ?? 0;
            return Json(ch, JsonRequestBehavior.AllowGet);
        }
        public ActionResult updatepurchage_dlmqwwwwwwww(int? hdidno, string hdtype, string txtcommentwrite)
        {

            //   if (hdtype == "APP")
            //   {
            //       hdtype = "Approved";
            //   }
            //   else
            //   {
            //       hdtype = "rejected";
            //   }

            //   string masterid = User.Identity.GetUserId();
            //   var whitelabelid = _db.Whitelabel_Superstokist_details.Where(a => a.SSId == masterid).SingleOrDefault().Whitelabelid;
            //   var sts = _db.Whitelabel_dlm_purchage.Where(a => a.id == hdidno.Value && a.frm == whitelabelid).Single().sts.ToUpper();
            //   if (sts == "PENDING")
            //   {
            //       System.Data.Entity.Core.Objects.ObjectParameter output = new
            //System.Data.Entity.Core.Objects.ObjectParameter("Output", typeof(string));
            //       var ch = _db.update_dlm_purchage(Convert.ToInt32(hdidno), hdtype, 0, txtcommentwrite, output).Single().msg;
            //       if (ch == "Balance Transfer SuccessFully.")
            //       {
            //           TempData["successorder"] = ch;
            //       }
            //       else
            //       {
            //           TempData["failedorder"] = ch;
            //       }

            //   }

            //   //return RedirectToAction("purcharge_request");

            //   //  var url= Url.Action("MDTODealer", "Home", new RouteValueDictionary(new { tabtype = "Retailer" }), HttpContext.Request.Url.Scheme, HttpContext.Request.Url.Authority);
            return MDTODealer("Retailer", "", "", "");

        }

        public ActionResult PurchaseOrderSend(string txtcode, string hdSuperstokistID, string hdMDDLM, string hdPaymentMode,
            string hdPaymentAmount, string hdMDDepositeSlipNo, string hdMDTransferType, string hdMDcollection, string hdMDComments,
            string hdMDBank, string hdsupraccno, string hdMDaccountno, string hdMDutrno, string hdMDwallet,
            string hdMDwalletno, string hdMDtransationno, string hdMDsettelment, string hdMDCreditDetail, string hdMDsubject)
        {
            string balance = hdPaymentAmount;
            string type = hdPaymentMode;
            string comment = hdMDComments;
            var DealerId = hdMDDLM;
            // string utrno = hdMDcollection == null ? hdMDutrno : hdMDcollection;
            string collectionby = hdMDcollection == null ? hdMDtransationno : hdMDcollection;
            collectionby = collectionby == null ? hdMDutrno : collectionby;
            collectionby = collectionby == null ? hdMDtransationno : collectionby;
            collectionby = collectionby == null ? hdMDsettelment : collectionby;
            collectionby = collectionby == null ? hdMDCreditDetail : collectionby;
            collectionby = collectionby == null ? hdMDsubject : collectionby;
            collectionby = collectionby == null ? hdMDDepositeSlipNo : collectionby;
            string bankname = hdMDBank == null ? hdMDwallet : hdMDBank;
            string adminacco = hdMDaccountno == null ? hdMDwalletno : hdMDaccountno;
            //  adminacco = adminacco == null ? hdMDDepositeSlipNo : adminacco;
            string DepositeSlipNo = hdMDDepositeSlipNo;
            if (hdMDTransferType != null)
            {
                hdPaymentMode = hdPaymentMode + "/" + hdMDTransferType;
            }

            //using (VastwebmultiEntities db = new VastwebmultiEntities())
            //{
            //    try
            //    {

            //        WebImage photo = null;
            //        var newFileName = "";
            //        var imagePath = "";

            //        //if (Request.HttpMethod == "POST")
            //        //{
            //        //    photo = WebImage.GetImageFromRequest();
            //        //    if (photo != null)
            //        //    {
            //        //        newFileName = Guid.NewGuid().ToString() + "_" +
            //        //                      Path.GetFileName(photo.FileName);
            //        //        imagePath = @"PurchaseOrderImg\" + newFileName;

            //        //        photo.Save(@"~\" + imagePath);
            //        //    }
            //        //}



            //        string userid = User.Identity.GetUserId();

            //        if (_db.master_purchase.Count(aa => aa.masterid.ToLower() == userid.ToLower() && aa.sts == "Pending") < 1)
            //        {
            //            var diff1 = (_db.admin_to_super_balance.Where(aa => aa.SuperStokistID == userid).OrderByDescending(aa => aa.ID).Select(c => c.cr).FirstOrDefault());
            //            diff1 = diff1 ?? 0;
            //            decimal diff =


            //                Convert.ToDecimal(diff1);
            //            var amount = Convert.ToDecimal(balance);
            //            decimal disCharge = 0;
            //            if (amount > 0)
            //            {
            //                creditchargemaster charges = null;
            //                // if (hdPaymentMode == "Credit")
            //                // {
            //                charges = _db.creditchargemasters.Where(aa => aa.userid == userid).FirstOrDefault(aa => aa.type == hdPaymentMode);
            //                // }
            //                if (charges != null)
            //                {
            //                    disCharge = (amount * charges.charge.Value) / 100;
            //                }









            //                _db.insert_masterpurchageorder(userid, hdPaymentMode, collectionby, bankname, "", hdMDComments, Convert.ToDecimal(balance), "", "", "", adminacco, "", "", "", "", disCharge, amount - disCharge);

            //                return Json("SuccessFully", JsonRequestBehavior.AllowGet);
            //            }
            //            else
            //            {
            //                TempData["error"] = "Amount should be Grater then 100";
            //                return Json("Amount should be not zero", JsonRequestBehavior.AllowGet);
            //                //  return RedirectToAction("PurchaseOrder");

            //            }

            //        }
            //        else
            //        {
            //            TempData["error"] = "Your purcharge Order Allready Pending.";
            //            return Json("Your purcharge Order Allready Pending.", JsonRequestBehavior.AllowGet);

            //        }
            //        //return RedirectToAction("PurchaseOrder");
            //    }
            //    catch (Exception ex)
            //    {
            //        TempData["error"] = ex;
            //        return Json(ex, JsonRequestBehavior.AllowGet);
            //        // return RedirectToAction("PurchaseOrder");
            //    }
            //}

            return Json("", JsonRequestBehavior.AllowGet);
        }

        public ActionResult MyOLDCreditChk()
        {
            string masterid = User.Identity.GetUserId();
            var diff1 = (_db.admin_to_super_balance.Where(aa => aa.SuperStokistID == masterid).OrderByDescending(aa => aa.ID).Select(c => c.cr).FirstOrDefault());
            diff1 = diff1 ?? 0;
            var bindbank = _db.bank_info.Where(x => x.userid == "Admin").ToList();
            List<SelectListItem> bankitem = new List<SelectListItem>();
            foreach (var bank in bindbank)
            {
                bankitem.Add(new SelectListItem { Text = bank.banknm + " / " + bank.holdername, Value = bank.acno });
            }
            var bindwallet = _db.tblwallet_info.Where(x => x.userid == "Admin").ToList();
            List<SelectListItem> walletitem = new List<SelectListItem>();
            foreach (var wallet in bindwallet)
            {
                walletitem.Add(new SelectListItem { Text = wallet.walletname + " / " + wallet.walletholdername, Value = wallet.walletno });
            }

            return Json(new { diff = diff1, listbank = bankitem, walletinfo = walletitem }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReceiveFund()
        {
            try
            {
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    string masterid = User.Identity.GetUserId();
                    string txt_frm_date = DateTime.Now.ToString();
                    string txt_to_date = DateTime.Now.ToString();
                    string frm_date = Convert.ToDateTime(txt_frm_date).Date.ToString("yyyy-MM-dd");
                    string to_date = Convert.ToDateTime(txt_to_date).AddDays(1).ToString("yyyy-MM-dd");
                    var ch = _db.Select_balance_Super_stokist(masterid, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
                    return View(ch);
                }
            }
            catch
            {

                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public ActionResult ReceiveFund(string txt_frm_date, string txt_to_date)
        {
            try
            {
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    string masterid = User.Identity.GetUserId();
                    ViewBag.chk = "post";
                    DateTime frm = Convert.ToDateTime(txt_frm_date);
                    DateTime to = Convert.ToDateTime(txt_to_date);
                    txt_frm_date = frm.ToString("dd-MM-yyyy");
                    txt_to_date = to.ToString("dd-MM-yyyy");
                    string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy",
                        "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
                    DateTime dt = !string.IsNullOrWhiteSpace(txt_frm_date) ? DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
                    DateTime dt1 = !string.IsNullOrWhiteSpace(txt_to_date) ? DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
                    DateTime frm_date = Convert.ToDateTime(dt).Date;
                    DateTime to_date = Convert.ToDateTime(dt1).Date.AddDays(1);
                    //
                    var masterlist = (from ma in _db.Master_details select ma);
                    ViewBag.masterid1 = new SelectList(masterlist, "msterid", "msterid");
                    ViewBag.masterid = new SelectList(masterlist, "msterid", "msterid");
                    var ch = _db.Select_balance_Super_stokist(masterid, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
                    return View(ch);
                }
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }

        public ActionResult GotoInvoicePDF(string Value, string DistOldBal, string DistNewBal, string Date)
        {
            string userid = User.Identity.GetUserId();
            return new Rotativa.ActionAsPdf("InvoicePDF", new { masterid = userid, Value = Value, DistOldBal = DistOldBal, DistNewBal = DistNewBal, Date = Date });
        }
        public ActionResult InvoicePDF(string masterid, string Value, string DistOldBal, string DistNewBal, string Date)
        {
            //string userid = User.Identity.GetUserId();
            var userdetaild = _db.Superstokist_details.Where(a => a.SSId == masterid).SingleOrDefault();
            var PDF_Content = new DealerInvoiceModel()
            {
                //From = From,
                Value = Value,
                DistOldBal = DistOldBal,
                DistNewBal = DistNewBal,
                Date = Date
            };
            TempData["retailername"] = userdetaild.SuperstokistName.ToUpper();
            TempData["retailermobile"] = userdetaild.Mobile.ToUpper();
            TempData["firmname"] = userdetaild.FarmName.ToUpper();
            TempData["retailerdate"] = PDF_Content.Date;
            TempData["date"] = DateTime.Now;
            return View(PDF_Content);
        }
        [HttpPost]
        public ActionResult ReceiveFund_GST(int idno)
        {
            try
            {
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {

                    //TODO

                }
                return new ViewAsPdf("GST_PDF");
            }
            catch
            {
                return RedirectToAction("Index");
            }


        }
        public ActionResult PurchaseOrder()
        {
            try
            {
                IEnumerable<select_master_pur_order_Result> model = null;

                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    ViewData["error"] = TempData["error"];
                    ViewData["success"] = TempData["success"];
                    TempData.Remove("error");
                    TempData.Remove("success");
                    string userid = User.Identity.GetUserId();
                    string txt_frm_date = DateTime.Now.ToString();
                    string txt_to_date = DateTime.Now.ToString();
                    string frm_date = Convert.ToDateTime(txt_frm_date).ToShortDateString();
                    string to_date = Convert.ToDateTime(txt_to_date).AddDays(1).ToString();

                    //account no
                    var accunt = (from acc in _db.bank_info select new { acno = acc.acno, acnooo = acc.acno + "||" + acc.holdername });
                    ViewBag.account = new SelectList(accunt, "acno", "acnooo", null);
                    var nm = (from bn in _db.bank_details select bn);
                    ViewBag.banknm = new SelectList(nm, "banknm", "banknm", null);
                    CultureInfo ci = new CultureInfo("en-US");
                    var month = DateTime.Now.ToString("MMMM", ci);
                    ViewData["dayofmonth"] = month + " Month Holiday List.";
                    List<SelectListItem> li = new List<SelectListItem>();
                    var diff1 = (_db.admin_to_super_balance.Where(aa => aa.SuperStokistID == userid).OrderByDescending(aa => aa.ID).Select(c => c.cr).FirstOrDefault());
                    diff1 = diff1 ?? 0;
                    ViewData["oldcredit"] = diff1;
                    var entries = _db.PurchaseOrderCashDepositCharges.ToList();
                    ViewBag.ATMMinCharge = entries.SingleOrDefault(a => a.PurchaseOrderType == "ATM Machine Deposit").MinCharge;
                    ViewBag.ATMChargePercant = entries.SingleOrDefault(a => a.PurchaseOrderType == "ATM Machine Deposit").ChargePercant;

                    ViewBag.CashMinCharge = entries.SingleOrDefault(a => a.PurchaseOrderType == "Branch Cash Deposit").MinCharge;
                    ViewBag.CashChargePercant = entries.SingleOrDefault(a => a.PurchaseOrderType == "Branch Cash Deposit").ChargePercant;
                    model = _db.select_master_pur_order(userid, "ALL", Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
                    return View(model);
                }
            }
            catch
            {
                return RedirectToAction("Index");

            }
        }
        [HttpPost]
        public ActionResult PurchaseOrder(string txt_frm_date, string txt_to_date)
        {
            try
            {
                IEnumerable<select_master_pur_order_Result> model = null;

                ViewData["show"] = TempData["show"];
                ViewBag.chk = "post";
                var userid = User.Identity.GetUserId();
                DateTime frm = Convert.ToDateTime(txt_frm_date);
                DateTime to = Convert.ToDateTime(txt_to_date);
                txt_frm_date = frm.ToString("dd-MM-yyyy");
                txt_to_date = to.ToString("dd-MM-yyyy");
                string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy",
                            "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
                DateTime dt = !string.IsNullOrWhiteSpace(txt_frm_date) ? DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
                DateTime dt1 = !string.IsNullOrWhiteSpace(txt_to_date) ? DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
                DateTime frm_date = Convert.ToDateTime(dt).Date;
                DateTime to_date = Convert.ToDateTime(dt1).Date.AddDays(1);

                //account no
                var accunt = (from acc in _db.bank_info
                              select new { acno = acc.acno, acnooo = acc.acno + "||" + acc.holdername });
                ViewBag.account = new SelectList(accunt, "acno", "acnooo", null);


                var diff1 = (_db.admin_to_super_balance.Where(aa => aa.SuperStokistID == userid)
                    .OrderByDescending(aa => aa.ID).Select(c => c.cr).FirstOrDefault());
                diff1 = diff1 ?? 0;
                ViewData["oldcredit"] = diff1;
                model = _db.select_master_pur_order(userid, "ALL", Convert.ToDateTime(frm_date),
                    Convert.ToDateTime(to_date));
                var entries = _db.PurchaseOrderCashDepositCharges.ToList();
                ViewBag.ATMMinCharge = entries.SingleOrDefault(a => a.PurchaseOrderType == "ATM Machine Deposit").MinCharge;
                ViewBag.ATMChargePercant = entries.SingleOrDefault(a => a.PurchaseOrderType == "ATM Machine Deposit").ChargePercant;

                ViewBag.CashMinCharge = entries.SingleOrDefault(a => a.PurchaseOrderType == "Branch Cash Deposit").MinCharge;
                ViewBag.CashChargePercant = entries.SingleOrDefault(a => a.PurchaseOrderType == "Branch Cash Deposit").ChargePercant;
                return View(model);

            }
            catch
            {
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public ActionResult purchageorder(string Paymode, string type, string utrno, string Comment, decimal balance, string accountno, string pancard, string branch, decimal? dipositCharge, string partyAcc, string AccHolderName)
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                try
                {

                    WebImage photo = null;
                    var newFileName = "";
                    var imagePath = "";

                    if (Request.HttpMethod == "POST")
                    {
                        photo = WebImage.GetImageFromRequest();
                        if (photo != null)
                        {
                            newFileName = Guid.NewGuid().ToString() + "_" +
                                          Path.GetFileName(photo.FileName);
                            imagePath = @"PurchaseOrderImg\" + newFileName;

                            photo.Save(@"~\" + imagePath);
                        }
                    }



                    string userid = User.Identity.GetUserId();

                    if (_db.master_purchase.Count(aa => aa.masterid.ToLower() == userid.ToLower() && aa.sts == "Pending") < 1)
                    {
                        var diff1 = (_db.admin_to_super_balance.Where(aa => aa.SuperStokistID == userid).OrderByDescending(aa => aa.ID).Select(c => c.cr).FirstOrDefault());
                        diff1 = diff1 ?? 0;
                        decimal diff = Convert.ToDecimal(diff1);
                        var amount = Convert.ToDecimal(balance);
                        if (amount > 0)
                        {

                            if (type == "Credit")
                            {
                                Paymode = "Credit";
                                _db.insert_masterpurchageorder(userid, Paymode, utrno, "", "", Comment, Convert.ToDecimal(balance), "", type, accountno, "", @"/PurchaseOrderImg/" + newFileName, "", "", "", 0, amount);
                                TempData["success"] = "Credit Pay Successfully";
                            }

                            if (Paymode == "Third Party Transfer" || Paymode.Contains("Online Transfer") || Paymode.Contains("Deposit"))
                            {
                                decimal disCharge = 0;
                                if (Paymode == "ATM Machine Deposit")
                                {
                                    var chargeEntry = _db.PurchaseOrderCashDepositCharges.FirstOrDefault(a => a.PurchaseOrderType == "ATM Machine Deposit");
                                    disCharge = (amount * Convert.ToDecimal(chargeEntry.ChargePercant)) / 100;
                                    // alert(disCharge);
                                    if (disCharge <= Convert.ToDecimal(chargeEntry.MinCharge))
                                    {
                                        disCharge = Convert.ToDecimal(chargeEntry.MinCharge);
                                    }
                                }
                                else if (Paymode == "Branch Cash Deposit")
                                {
                                    var chargeEntry = _db.PurchaseOrderCashDepositCharges.FirstOrDefault(a => a.PurchaseOrderType == "Branch Cash Deposit");
                                    disCharge = (amount * Convert.ToDecimal(chargeEntry.ChargePercant)) / 100;
                                    // alert(disCharge);
                                    if (disCharge <= Convert.ToDecimal(chargeEntry.MinCharge))
                                    {
                                        disCharge = Convert.ToDecimal(chargeEntry.MinCharge);
                                    }
                                }
                                _db.insert_masterpurchageorder(userid, Paymode, utrno, "", "", Comment, Convert.ToDecimal(balance), "", type, accountno, "", @"/PurchaseOrderImg/" + newFileName, pancard, branch, AccHolderName, disCharge, amount - disCharge);
                                TempData["success"] = "Purchase Order Successfully";
                            }

                        }
                        else
                        {
                            TempData["error"] = "Amount should be not zero";
                            return RedirectToAction("PurchaseOrder");
                        }

                    }
                    else
                    {
                        TempData["error"] = "Your purcharge Order Allready Pending.";
                    }
                    return RedirectToAction("PurchaseOrder");
                }
                catch (Exception ex)
                {
                    TempData["error"] = ex;
                    return RedirectToAction("PurchaseOrder");
                }
            }
        }
        public ActionResult purcharge_request()
        {
            ViewData["successorder"] = TempData["successorder"];
            ViewData["failedorder"] = TempData["failedorder"];
            string userid = User.Identity.GetUserId();
            var Email = _db.Users.FirstOrDefault(a => a.UserId == userid).Email;
            string txt_frm_date = DateTime.Now.Date.ToShortDateString();
            string txt_to_date = DateTime.Now.ToString();
            string frm_date = Convert.ToDateTime(txt_frm_date).ToShortDateString();
            string to_date = Convert.ToDateTime(txt_to_date).AddDays(1).ToString();
            var ch = _db.select_dlm_pur_order("ALL", Email, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            return View(ch);
        }
        [HttpPost]
        public ActionResult purcharge_request(string txt_frm_date, string txt_to_date)
        {
            string userid = User.Identity.GetUserId();
            var Email = _db.Users.FirstOrDefault(a => a.UserId == userid).Email;
            ViewBag.chk = "post";
            DateTime frm = Convert.ToDateTime(txt_frm_date);
            DateTime to = Convert.ToDateTime(txt_to_date);
            txt_frm_date = frm.ToString("dd-MM-yyyy");
            txt_to_date = to.ToString("dd-MM-yyyy");
            string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy",
                        "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
            DateTime dt = !string.IsNullOrWhiteSpace(txt_frm_date) ? DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
            DateTime dt1 = !string.IsNullOrWhiteSpace(txt_to_date) ? DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
            DateTime frm_date = Convert.ToDateTime(dt).Date;
            DateTime to_date = Convert.ToDateTime(dt1).Date.AddDays(1);
            var ch = _db.select_dlm_pur_order("ALL", Email, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            return View(ch);
        }
        public ActionResult updatepurchage_dlm(int id, string type, string txtcomment)
        {
            if (type == "APP")
            {
                type = "Approved";
            }
            else
            {
                type = "rejected";
            }
            var sts = _db.dlm_purchage.Where(a => a.id == id).Single().sts.ToUpper();
            if (sts == "PENDING")
            {
                System.Data.Entity.Core.Objects.ObjectParameter output = new
         System.Data.Entity.Core.Objects.ObjectParameter("Output", typeof(string));
                var ch = _db.update_dlm_purchage(Convert.ToInt32(id), type, 0, txtcomment, output).Single().msg;
                if (ch == "Balance Transfer SuccessFully.")
                {
                    TempData["successorder"] = ch;
                }
                else
                {
                    TempData["failedorder"] = ch;
                }

            }
            return RedirectToAction("purcharge_request");
        }

        [HttpGet]
        public ActionResult show_credit_Report()
        {
            var userid = User.Identity.GetUserId();
            var show = _db.Show_master_credit_report_by_admin(userid, "").ToList();
            return View(show);
        }

        #endregion

        #region Operator_INdex

        public ActionResult OperatorIndex()
        {
            var userid = User.Identity.GetUserId();
            Operater_Commission sb = new Operater_Commission();
            sb.Prepaid = (from cust in _db.prepaid_master_comm
                          join ord in _db.Operator_Code
                          on cust.optcode equals ord.Operator_id.ToString()
                          where (cust.userid == userid)
                          select new Prepaid_Comm
                          {
                              OperatorCode = ord.new_opt_code,
                              Commission = cust.comm,
                              BlockTime = ord.blocktime,
                              Status = ord.status,
                              OperatorType = ord.Operator_type,
                              OperatorName = ord.operator_Name
                          }).ToList();
            sb.Electricity_comm = (from cust in _db.utility_master_comm
                                   join ord in _db.Operator_Code
                                   on cust.optcode equals ord.Operator_id.ToString()
                                   where (cust.userid == userid)
                                   select new Electricity
                                   {
                                       OperatorCode = ord.new_opt_code,
                                       Commission = cust.comm,
                                       BlockTime = ord.blocktime,
                                       Status = ord.status,
                                       OperatorType = ord.Operator_type,
                                       OperatorName = ord.operator_Name,
                                       Gst = cust.gst
                                   }).ToList();
            sb.Money2 = (from cust in _db.paytm_imps_master_comm
                         where (cust.userid == userid)
                         select new money2_comm
                         {
                             Verifycomm = cust.verify_comm,
                             comm_1000 = cust.comm_1000,
                             comm_2000 = cust.comm_2000,
                             comm_3000 = cust.comm_3000,
                             comm_4000 = cust.comm_4000,
                             comm_5000 = cust.comm_5000,
                             comm_6000 = cust.comm_6000,
                             comm_7000 = cust.comm_7000,
                             comm_8000 = cust.comm_8000,
                             comm_9000 = cust.comm_9000,
                             comm_10000 = cust.comm_10000,

                             comm_11000 = cust.comm_11000,
                             comm_12000 = cust.comm_12000,
                             comm_13000 = cust.comm_13000,
                             comm_14000 = cust.comm_14000,
                             comm_15000 = cust.comm_15000,
                             comm_16000 = cust.comm_16000,
                             comm_17000 = cust.comm_17000,
                             comm_18000 = cust.comm_18000,
                             comm_19000 = cust.comm_19000,
                             comm_20000 = cust.comm_20000,

                             comm_31000 = cust.comm_31000,
                             comm_32000 = cust.comm_32000,
                             comm_33000 = cust.comm_33000,
                             comm_34000 = cust.comm_34000,
                             comm_35000 = cust.comm_35000,
                             comm_36000 = cust.comm_36000,
                             comm_37000 = cust.comm_37000,
                             comm_38000 = cust.comm_38000,
                             comm_39000 = cust.comm_39000,
                             comm_40000 = cust.comm_40000,

                             comm_41000 = cust.comm_41000,
                             comm_42000 = cust.comm_42000,
                             comm_43000 = cust.comm_43000,
                             comm_44000 = cust.comm_44000,
                             comm_45000 = cust.comm_45000,
                             comm_46000 = cust.comm_46000,
                             comm_47000 = cust.comm_47000,
                             comm_48000 = cust.comm_48000,
                             comm_49000 = cust.comm_49000,
                             comm_50000 = cust.comm_50000

                         }).ToList();

            sb.Pencard_comm = (from cust in _db.pancard_master_Common_comm
                               where (cust.userid == userid)
                               select new Pencard
                               {
                                   TokenValueDigital = cust.tokenvalueDigital,
                                   TokenValuePhysical = cust.tokenvaluePhysical,
                                   Gst = cust.gst,
                                   CommissionDigital = cust.commDigital,
                                   CommissionPhysical = cust.commPhysical
                               }).ToList();
            //sb.AEPS = (from cust in _db.Aeps_comm_new
            //           where cust.userid == userid && cust.role == "WMASTER"
            //           select new AEPS_Comm
            //           {
            //               comm = cust.comm,
            //               Maxrs = cust.maxrs,
            //               MinValue = cust.minvalue,
            //               MiniStatement = cust.M_statement
            //           }).ToList();

            sb.MPOS = (from cust in _db.Mpos_comm_details
                       where cust.Userid == userid && cust.userRole == "WMASTER"
                       select new MPOS_Comm
                       {
                           CashWithdraw = cust.CashWithdraw,
                           Salesdebitupto2000 = cust.salesDebitUpto2000,
                           Salesdebitabove2000 = cust.salesDebitAbove2000,
                           SalescreditNormal = cust.saleCredit,
                           Salescreditgrocery = cust.saleCreditGrocery,
                           SalescreditEduandInsu = cust.saleCreditEduAndIns,
                           gst = cust.gst,
                           credit_type = cust.credit_type

                       }).ToList();
            sb.INDONEPAL = (from cust in _db.Slab_IndoNepal
                            where cust.UserId == userid && cust.Role == "WMASTER"
                            select new INDONEPAL_Comm
                            {
                                min = cust.min,
                                max = cust.max,
                                charge = cust.charge,
                                margin = cust.margin

                            }).ToList();
            sb.FLIGHT = (from cust in _db.Slab_Flight
                         where cust.UserId == userid && cust.Role == "WMASTER"
                         select new FLIGHT_Comm
                         {
                             IsDomestic = cust.IsDomestic,
                             marginPercentage = cust.marginPercentage,
                             RetailerMarkupCharge = cust.RetailerMarkupCharge,
                             gst = cust.gst,
                             tds = cust.tds
                         }).ToList();

            sb.HOTEL = (from cust in _db.Slab_Hotel
                        where cust.UserId == userid && cust.Role == "WMASTER"
                        select new HOTEL_Comm
                        {
                            IsDomestic = cust.IsDomestic == true ? false : true,
                            marginPercentage = cust.marginPercentage,
                            RetailerMarkupCharge = cust.RetailerMarkupCharge,
                            gst = cust.gst,
                            tds = cust.tds
                        }).ToList();

            sb.BUS = (from cust in _db.Slab_Bus
                      where cust.UserId == userid && cust.Role == "WMASTER"
                      select new BUS_Comm
                      {
                          marginPercentage = cust.marginPercentage,
                          RetailerMarkupCharge = cust.retailerMarkup,
                          gst = cust.gst,
                          tds = cust.tds
                      }).ToList();

            sb.Giftcard = (from cust in _db.giftcard_master_comm
                           join ord in _db.Operator_Code
                           on cust.opt_code equals ord.Operator_id.ToString()
                           where (cust.masterid == userid)
                           select new Giftcard_Comm
                           {
                               OperatorCode = ord.new_opt_code,
                               Commission = cust.comm,
                               Status = ord.status,
                               OperatorType = ord.Operator_type,
                               OperatorName = ord.operator_Name
                           }).ToList();


            sb.Broadband = (from cust in _db.Broandband_master_comm
                            join ord in _db.Operator_Code
                            on cust.optcode equals ord.Operator_id.ToString()
                            where (cust.userid == userid)
                            select new Broadband_Comm
                            {
                                OperatorCode = ord.new_opt_code,
                                Commission = cust.comm,
                                BlockTime = ord.blocktime,
                                Status = ord.status,
                                OperatorType = ord.Operator_type,
                                OperatorName = ord.operator_Name
                            }).ToList();
            sb.Loan = (from cust in _db.Loan_master_comm
                       join ord in _db.Operator_Code
                       on cust.optcode equals ord.Operator_id.ToString()
                       where (cust.userid == userid)
                       select new Loan_Comm
                       {
                           OperatorCode = ord.new_opt_code,
                           Commission = cust.comm,
                           BlockTime = ord.blocktime,
                           Status = ord.status,
                           OperatorType = ord.Operator_type,
                           OperatorName = ord.operator_Name
                       }).ToList();

            sb.Water = (from cust in _db.Water_master_comm
                        join ord in _db.Operator_Code
                        on cust.optcode equals ord.Operator_id.ToString()
                        where (cust.userid == userid)
                        select new Water_Comm
                        {
                            OperatorCode = ord.new_opt_code,
                            Commission = cust.comm,
                            BlockTime = ord.blocktime,
                            Status = ord.status,
                            OperatorType = ord.Operator_type,
                            OperatorName = ord.operator_Name
                        }).ToList();
            sb.Insurance = (from cust in _db.Insurance_master_comm
                            join ord in _db.Operator_Code
                            on cust.optcode equals ord.Operator_id.ToString()
                            where (cust.userid == userid)
                            select new Insurance_Comm
                            {
                                OperatorCode = ord.new_opt_code,
                                Commission = cust.comm,
                                BlockTime = ord.blocktime,
                                Status = ord.status,
                                OperatorType = ord.Operator_type,
                                OperatorName = ord.operator_Name,
                                Gst = cust.gst
                            }).ToList();

            return View(sb);
        }

        [HttpPost]
        public ActionResult OperatorIndex(string ddlcomm)
        {
            var userid = User.Identity.GetUserId();
            Operater_Commission sb = new Operater_Commission();
            sb.Prepaid = (from cust in _db.prepaid_master_comm
                          join ord in _db.Operator_Code
                          on cust.optcode equals ord.Operator_id.ToString()
                          where (cust.userid == userid)
                          select new Prepaid_Comm
                          {
                              OperatorCode = ord.new_opt_code,
                              Commission = cust.comm,
                              BlockTime = ord.blocktime,
                              Status = ord.status,
                              OperatorType = ord.Operator_type,
                              OperatorName = ord.operator_Name
                          }).ToList();
            sb.Electricity_comm = (from cust in _db.utility_master_comm
                                   join ord in _db.Operator_Code
                                   on cust.optcode equals ord.Operator_id.ToString()
                                   where (cust.userid == userid)
                                   select new Electricity
                                   {
                                       OperatorCode = ord.new_opt_code,
                                       Commission = cust.comm,
                                       BlockTime = ord.blocktime,
                                       Status = ord.status,
                                       OperatorType = ord.Operator_type,
                                       OperatorName = ord.operator_Name,
                                       Gst = cust.gst
                                   }).ToList();
            sb.Money = (from cust in _db.imps_master_comm
                        where (cust.userid == userid)
                        select new money_comm
                        {
                            Verifycomm = cust.verify_comm,
                            comm_1000 = cust.comm_1000,
                            comm_2000 = cust.comm_2000,
                            comm_3000 = cust.comm_3000,
                            comm_4000 = cust.comm_4000,
                            comm_5000 = cust.comm_5000,
                            gst = cust.gst
                        }).ToList();
            sb.Pencard_comm = (from cust in _db.pancard_master_Common_comm
                               where (cust.userid == userid)
                               select new Pencard
                               {
                                   TokenValueDigital = cust.tokenvalueDigital,
                                   TokenValuePhysical = cust.tokenvaluePhysical,
                                   Gst = cust.gst,
                                   CommissionDigital = cust.commDigital,
                                   CommissionPhysical = cust.commPhysical

                               }).ToList();
            //sb.AEPS = (from cust in _db.Aeps_comm_new
            //           where cust.userid == userid && cust.role == "WMASTER"
            //           select new AEPS_Comm
            //           {
            //               comm = cust.comm,
            //               Maxrs = cust.maxrs,
            //               MinValue = cust.minvalue,
            //               MiniStatement = cust.M_statement
            //           }).ToList();

            sb.MPOS = (from cust in _db.Mpos_comm_details
                       where cust.Userid == userid && cust.userRole == "WMASTER"
                       select new MPOS_Comm
                       {
                           CashWithdraw = cust.CashWithdraw,
                           Salesdebitupto2000 = cust.salesDebitUpto2000,
                           Salesdebitabove2000 = cust.salesDebitAbove2000,
                           SalescreditNormal = cust.saleCredit,
                           Salescreditgrocery = cust.saleCreditGrocery,
                           SalescreditEduandInsu = cust.saleCreditEduAndIns,
                           gst = cust.gst,
                           credit_type = cust.credit_type

                       }).ToList();
            sb.INDONEPAL = (from cust in _db.Slab_IndoNepal
                            where cust.UserId == userid && cust.Role == "WMASTER"
                            select new INDONEPAL_Comm
                            {
                                min = cust.min,
                                max = cust.max,
                                charge = cust.charge,
                                margin = cust.margin

                            }).ToList();
            sb.FLIGHT = (from cust in _db.Slab_Flight
                         where cust.UserId == userid && cust.Role == "WMASTER"
                         select new FLIGHT_Comm
                         {
                             IsDomestic = cust.IsDomestic,
                             marginPercentage = cust.marginPercentage,
                             RetailerMarkupCharge = cust.RetailerMarkupCharge,
                             gst = cust.gst,
                             tds = cust.tds
                         }).ToList();

            sb.HOTEL = (from cust in _db.Slab_Hotel
                        where cust.UserId == userid && cust.Role == "WMASTER"
                        select new HOTEL_Comm
                        {
                            IsDomestic = cust.IsDomestic == true ? false : true,
                            marginPercentage = cust.marginPercentage,
                            RetailerMarkupCharge = cust.RetailerMarkupCharge,
                            gst = cust.gst,
                            tds = cust.tds
                        }).ToList();

            sb.BUS = (from cust in _db.Slab_Bus
                      where cust.UserId == userid && cust.Role == "WMASTER"
                      select new BUS_Comm
                      {
                          marginPercentage = cust.marginPercentage,
                          RetailerMarkupCharge = cust.retailerMarkup,
                          gst = cust.gst,
                          tds = cust.tds
                      }).ToList();


            sb.Giftcard = (from cust in _db.giftcard_master_comm
                           join ord in _db.Operator_Code
                           on cust.opt_code equals ord.Operator_id.ToString()
                           where (cust.masterid == userid)
                           select new Giftcard_Comm
                           {
                               OperatorCode = ord.new_opt_code,
                               Commission = cust.comm,
                               Status = ord.status,
                               OperatorType = ord.Operator_type,
                               OperatorName = ord.operator_Name
                           }).ToList();


            sb.Broadband = (from cust in _db.Broandband_master_comm
                            join ord in _db.Operator_Code
                            on cust.optcode equals ord.Operator_id.ToString()
                            where (cust.userid == userid)
                            select new Broadband_Comm
                            {
                                OperatorCode = ord.new_opt_code,
                                Commission = cust.comm,
                                BlockTime = ord.blocktime,
                                Status = ord.status,
                                OperatorType = ord.Operator_type,
                                OperatorName = ord.operator_Name
                            }).ToList();
            sb.Loan = (from cust in _db.Loan_master_comm
                       join ord in _db.Operator_Code
                       on cust.optcode equals ord.Operator_id.ToString()
                       where (cust.userid == userid)
                       select new Loan_Comm
                       {
                           OperatorCode = ord.new_opt_code,
                           Commission = cust.comm,
                           BlockTime = ord.blocktime,
                           Status = ord.status,
                           OperatorType = ord.Operator_type,
                           OperatorName = ord.operator_Name
                       }).ToList();

            sb.Water = (from cust in _db.Water_master_comm
                        join ord in _db.Operator_Code
                        on cust.optcode equals ord.Operator_id.ToString()
                        where (cust.userid == userid)
                        select new Water_Comm
                        {
                            OperatorCode = ord.new_opt_code,
                            Commission = cust.comm,
                            BlockTime = ord.blocktime,
                            Status = ord.status,
                            OperatorType = ord.Operator_type,
                            OperatorName = ord.operator_Name
                        }).ToList();

            sb.Insurance = (from cust in _db.Insurance_master_comm
                            join ord in _db.Operator_Code
                            on cust.optcode equals ord.Operator_id.ToString()
                            where (cust.userid == userid)
                            select new Insurance_Comm
                            {
                                OperatorCode = ord.new_opt_code,
                                Commission = cust.comm,
                                BlockTime = ord.blocktime,
                                Status = ord.status,
                                OperatorType = ord.Operator_type,
                                OperatorName = ord.operator_Name,
                                Gst = cust.gst
                            }).ToList();

            ViewData["type"] = ddlcomm;
            return View(sb);
        }
        #endregion

        #region Reports

        //END DisputeReport
        // RechargeReport 

        public JsonResult Getopt(string ID)
        {
            var Operator1 = _db.Operator_Code.Distinct().Where(a => a.Operator_type == ID).ToList();
            IEnumerable<SelectListItem> selectopt = from p in Operator1
                                                    select new SelectListItem
                                                    {
                                                        Value = p.new_opt_code,
                                                        Text = p.operator_Name
                                                    };
            ViewBag.Operator = new SelectList(selectopt, "Value", "Text");
            return Json(new SelectList(selectopt.ToArray(), "Value", "Text"), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult RechargeReport()
        {
            string userid = User.Identity.GetUserId();
            ViewBag.alldealer = new SelectList(_db.spWhitelabel_select_Dealer_for_master_ddl(userid), "Dealerid", "DealerName", null).ToList();
            ViewBag.allretailer = new SelectList(_db.spWhitelabel_select_retailer_for_master_ddl(userid), "RetailerId", "RetailerName", null).ToList();

            //ViewBag.Operator_Type = new SelectList(_db.select_Operator_code(), "Operator_type", "Operator_type").ToList();

            var operator_value = _db.Operator_Code.Distinct().ToList();
            ViewBag.Operator = new SelectList("", "");

            return View();
        }
        [HttpPost]
        public ActionResult RechargeReport(string Operator_Type, string txt_frm_date, string txt_to_date, string ddl_status, string Operator, string txtmob, string alldealer, string allretailer)
        {

            string loginuserid = User.Identity.GetUserId();

            ViewBag.chk = "post";


            ViewBag.alldealer = new SelectList(_db.spWhitelabel_select_Dealer_for_master_ddl(loginuserid), "Dealerid", "DealerName", null).ToList();
            ViewBag.allretailer = new SelectList(_db.spWhitelabel_select_retailer_for_master_ddl(loginuserid), "RetailerId", "RetailerName", null).ToList();

            var Operator1 = _db.Operator_Code.Distinct().Where(a => a.Operator_type == Operator_Type).ToList();
            IEnumerable<SelectListItem> selectopt = from p in Operator1
                                                    select new SelectListItem
                                                    {
                                                        Value = p.new_opt_code,
                                                        Text = p.operator_Name
                                                    };
            ViewBag.Operator = new SelectList(selectopt, "Value", "Text");



            return View();
        }

        [ChildActionOnly]
        public ActionResult _RechargeReport(string btntype, string ddlusers, string txt_frm_date, string txt_to_date, string ddl_status, string Operator, string txtmob, string alldealer, string allretailer)
        {

            string loginuserid = User.Identity.GetUserId();
            string whitelabelid = _db.Whitelabel_Superstokist_details.Where(a => a.SSId == loginuserid).SingleOrDefault().Whitelabelid;
            var userid = "";
            var mobile = "";
            var optname = "";
            if ((txt_frm_date == null || txt_frm_date == "") && (txt_to_date == null || txt_to_date == ""))
            {
                txt_frm_date = DateTime.Now.ToString();
                txt_to_date = DateTime.Now.ToString();

            }
            if (ddlusers == null)
            {

                ddlusers = "Master";

            }

            if (ddlusers == "Master")
            {
                userid = loginuserid;
            }
            if (ddlusers == "Dealer")
            {
                if (alldealer == "" || alldealer.Contains("Distubutor") || alldealer == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = alldealer;
                }
            }
            if (ddlusers == "Retailer")
            {
                if (allretailer == "" || allretailer.Contains("All Retailer") || allretailer == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = allretailer;
                }
            }

            if (Operator == null || Operator == "" || Operator.Contains("ALL OPERATORS"))
            {
                optname = "ALL";
            }
            else
            {
                optname = Operator;
            }
            if (txtmob == "" || txtmob == null)
            {
                mobile = "ALL";
            }
            else
            {
                mobile = txtmob;
            }
            if (ddl_status == "" || ddl_status == null || ddl_status.Contains("ALL STATUS") || Operator == null)
            {
                //optname = "ALL";
                ddl_status = "ALL";
            }

            DateTime frm1 = Convert.ToDateTime(txt_frm_date);
            DateTime to1 = Convert.ToDateTime(txt_to_date);
            txt_frm_date = frm1.ToString("dd-MM-yyyy");
            txt_to_date = to1.ToString("dd-MM-yyyy");
            string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy",
                            "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
            DateTime dt = !string.IsNullOrWhiteSpace(txt_frm_date) ? DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
            DateTime dt1 = !string.IsNullOrWhiteSpace(txt_to_date) ? DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
            DateTime frm_date = Convert.ToDateTime(dt).Date;
            DateTime to_date = Convert.ToDateTime(dt1).Date.AddDays(1);

            var ch = _db.spWhitelabel_Recharge_operator_report_newPaging(whitelabelid, 1, 35, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), ddlusers, userid, ddl_status, optname, mobile, "ALL").ToList();

            return View(ch);
        }
        [HttpPost]
        public ActionResult InfiniteScroll(int pageindex, string ddlusers, string txt_frm_date, string txt_to_date, string ddl_status, string Operator, string txtmob, string alldealer, string allretailer)
        {
            string loginuserid = User.Identity.GetUserId();

            var userid = "";
            var mobile = "";
            var optname = "";
            if (ddlusers == null)
            {

                ddlusers = "WMASTER";

            }

            if (ddlusers == "WMASTER")
            {
                userid = loginuserid;
            }
            if (ddlusers == "Dealer")
            {
                if (alldealer == "" || alldealer.Contains("Distubutor") || alldealer == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = alldealer;
                }
            }
            if (ddlusers == "Retailer")
            {
                if (allretailer == "" || allretailer.Contains("All Retailer") || allretailer == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = allretailer;
                }
            }

            if (Operator == null || Operator == "" || Operator.Contains("ALL OPERATORS"))
            {
                optname = "ALL";
            }
            else
            {
                optname = Operator;
            }
            if (txtmob == "" || txtmob == null)
            {
                mobile = "ALL";
            }
            else
            {
                mobile = txtmob;
            }
            if (ddl_status == "" || ddl_status == null || ddl_status.Contains("ALL STATUS") || Operator == null)
            {
                //optname = "ALL";
                ddl_status = "ALL";
            }

            DateTime frm1 = Convert.ToDateTime(txt_frm_date);
            DateTime to1 = Convert.ToDateTime(txt_to_date);
            txt_frm_date = frm1.ToString("dd-MM-yyyy");
            txt_to_date = to1.ToString("dd-MM-yyyy");
            string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy",
                            "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
            DateTime dt = !string.IsNullOrWhiteSpace(txt_frm_date) ? DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
            DateTime dt1 = !string.IsNullOrWhiteSpace(txt_to_date) ? DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
            DateTime frm_date = Convert.ToDateTime(dt).Date;
            DateTime to_date = Convert.ToDateTime(dt1).Date.AddDays(1);

            int pagesize = 35;
            var tbrow = _db.proc_Recharge_operator_report_newPaging(pageindex, 36, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), ddlusers, userid, ddl_status, optname, mobile, "ALL").ToList();

            JsonModel jsonmodel = new JsonModel();
            jsonmodel.NoMoredata = tbrow.Count < pagesize;
            jsonmodel.HTMLString = renderPartialViewtostring("_RechargeReport", tbrow);
            return Json(jsonmodel);
        }
        public ActionResult TotalRecharge(string ddlusers, string txt_frm_date, string txt_to_date, string ddl_status, string Operator, string txtmob, string alldealer, string allretailer)
        {

            string loginuserid = User.Identity.GetUserId();
            ViewBag.chk = "post";
            var userid = "";
            var mobile = "";
            var optname = "";
            if ((txt_frm_date == null || txt_frm_date == "") && (txt_to_date == null || txt_to_date == ""))
            {
                txt_frm_date = DateTime.Now.ToString();
                txt_to_date = DateTime.Now.ToString();

            }
            if (ddlusers == null)
            {

                ddlusers = "WMASTER";

            }

            if (ddlusers == "WMASTER")
            {
                userid = loginuserid;
            }
            if (ddlusers == "Dealer")
            {
                if (alldealer == "" || alldealer.Contains("Distubutor") || alldealer == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = alldealer;
                }
            }
            if (ddlusers == "Retailer")
            {
                if (allretailer == "" || allretailer.Contains("All Retailer") || allretailer == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = allretailer;
                }
            }

            if (Operator == null || Operator == "" || Operator.Contains("ALL OPERATORS"))
            {
                optname = "ALL";
            }
            else
            {
                optname = Operator;
            }
            if (txtmob == "" || txtmob == null)
            {
                mobile = "ALL";
            }
            else
            {
                mobile = txtmob;
            }
            if (ddl_status == "" || ddl_status == null || ddl_status.Contains("ALL STATUS") || Operator == null)
            {
                //optname = "ALL";
                ddl_status = "ALL";
            }

            DateTime frm1 = Convert.ToDateTime(txt_frm_date);
            DateTime to1 = Convert.ToDateTime(txt_to_date);
            txt_frm_date = frm1.ToString("dd-MM-yyyy");
            txt_to_date = to1.ToString("dd-MM-yyyy");
            string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy",
                            "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
            DateTime dt = !string.IsNullOrWhiteSpace(txt_frm_date) ? DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
            DateTime dt1 = !string.IsNullOrWhiteSpace(txt_to_date) ? DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
            DateTime frm_date = Convert.ToDateTime(dt).Date;
            DateTime to_date = Convert.ToDateTime(dt1).Date.AddDays(1);



            var ch = _db.proc_Recharge_operator_report_newPaging(1, 1000000, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), ddlusers, userid, ddl_status, optname, mobile, "ALL").ToList();

            var successtotal1 = ch.Where(a => a.rstatus.ToUpper() == "SUCCESS").Sum(a => a.amount);
            var Failedtotal1 = ch.Where(s => s.rstatus.ToUpper().Contains("FAILED")).Sum(s => Convert.ToInt32(s.amount));
            var PendingTotal1 = ch.Where(s => s.rstatus.ToUpper().Contains("PENDING")).Sum(s => Convert.ToInt32(s.amount));
            var retailerincome = ch.Where(a => a.rstatus.ToUpper() == "SUCCESS").Sum(a => a.income);
            var dlmincome = ch.Where(a => a.rstatus.ToUpper() == "SUCCESS").Sum(a => a.dealerincome);
            var data = new
            {
                success = successtotal1,
                failed = Failedtotal1,
                pending = PendingTotal1,
                remincome = retailerincome,
                dlmincom = dlmincome

            };

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public virtual ActionResult ExcelRechargereport(string ddlusers, string txt_frm_date, string txt_to_date, string ddl_status, string Operator, string txtmob, string alldealer, string allretailer)
        {
            string loginuserid = User.Identity.GetUserId();

            var userid = "";
            var mobile = "";
            var optname = "";
            if ((txt_frm_date == null || txt_frm_date == "") && (txt_to_date == null || txt_to_date == ""))
            {
                txt_frm_date = DateTime.Now.ToString();
                txt_to_date = DateTime.Now.ToString();

            }
            if (ddlusers == null)
            {

                ddlusers = "WMASTER";

            }

            if (ddlusers == "WMASTER")
            {
                userid = loginuserid;
            }
            if (ddlusers == "Dealer")
            {
                if (alldealer == "" || alldealer.Contains("Distubutor") || alldealer == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = alldealer;
                }
            }
            if (ddlusers == "Retailer")
            {
                if (allretailer == "" || allretailer.Contains("All Retailer") || allretailer == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = allretailer;
                }
            }

            if (Operator == null || Operator == "" || Operator.Contains("ALL OPERATORS"))
            {
                optname = "ALL";
            }
            else
            {
                optname = Operator;
            }
            if (txtmob == "" || txtmob == null)
            {
                mobile = "ALL";
            }
            else
            {
                mobile = txtmob;
            }
            if (ddl_status == "" || ddl_status == null || ddl_status.Contains("ALL STATUS") || Operator == null)
            {
                //optname = "ALL";
                ddl_status = "ALL";
            }

            DateTime frm = Convert.ToDateTime(txt_frm_date);
            DateTime to = Convert.ToDateTime(txt_to_date);
            txt_frm_date = frm.ToString("dd-MM-yyyy");
            txt_to_date = to.ToString("dd-MM-yyyy");
            string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy",
                            "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
            DateTime dt = !string.IsNullOrWhiteSpace(txt_frm_date) ? DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
            DateTime dt1 = !string.IsNullOrWhiteSpace(txt_to_date) ? DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
            DateTime frm_date = Convert.ToDateTime(dt).Date;
            DateTime to_date = Convert.ToDateTime(dt1).Date.AddDays(1);

            DataTable dt2 = new DataTable();
            dt2.Columns.Add("Status", typeof(string));
            dt2.Columns.Add("Request Id", typeof(string));
            dt2.Columns.Add("Consumer No", typeof(string));
            dt2.Columns.Add("Opt Type", typeof(string));
            dt2.Columns.Add("Operator", typeof(string));
            dt2.Columns.Add("Amount", typeof(string));
            dt2.Columns.Add("MY Income", typeof(string));
            dt2.Columns.Add("My Post", typeof(string));
            dt2.Columns.Add("OperatorID", typeof(string));
            dt2.Columns.Add("Date", typeof(string));
            dt2.Columns.Add("Distributor Income", typeof(string));
            dt2.Columns.Add("Distributor Remain", typeof(string));
            dt2.Columns.Add("Retailer Income", typeof(string));
            dt2.Columns.Add("Retailer Remain", typeof(string));



            var respo = _db.proc_Recharge_operator_report_newPaging(1, 100000, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), ddlusers, userid, ddl_status, optname, mobile, "ALL").ToList();
            foreach (var item in respo)
            {

                dt2.Rows.Add(item.rstatus + "=" + item.frm_name, item.idno, item.mobile, item.operator_type, item.Operator_name, item.amount, item.masterincome, item.masterremain, item.opt_id,
                 item.Rch_time, item.dealerincome, item.dealerremain, item.income, item.remain);
            }

            var grid = new GridView();
            grid.DataSource = dt2;
            grid.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=Report.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            return View();
        }
        public ActionResult PDFRechargeReport(string btntype, string ddlusers, string txt_frm_date, string txt_to_date, string ddl_status, string Operator, string txtmob, string alldealer, string allretailer)
        {

            string loginuserid = User.Identity.GetUserId();

            var userid = "";
            var mobile = "";
            var optname = "";
            if ((txt_frm_date == null || txt_frm_date == "") && (txt_to_date == null || txt_to_date == ""))
            {
                txt_frm_date = DateTime.Now.ToString();
                txt_to_date = DateTime.Now.ToString();

            }
            if (ddlusers == null)
            {

                ddlusers = "WMASTER";

            }

            if (ddlusers == "WMASTER")
            {
                userid = loginuserid;
            }
            if (ddlusers == "Dealer")
            {
                if (alldealer == "" || alldealer.Contains("Distubutor") || alldealer == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = alldealer;
                }
            }
            if (ddlusers == "Retailer")
            {
                if (allretailer == "" || allretailer.Contains("All Retailer") || allretailer == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = allretailer;
                }
            }

            if (Operator == null || Operator == "" || Operator.Contains("ALL OPERATORS"))
            {
                optname = "ALL";
            }
            else
            {
                optname = Operator;
            }
            if (txtmob == "" || txtmob == null)
            {
                mobile = "ALL";
            }
            else
            {
                mobile = txtmob;
            }
            if (ddl_status == "" || ddl_status == null || ddl_status.Contains("ALL STATUS") || Operator == null)
            {
                //optname = "ALL";
                ddl_status = "ALL";
            }

            DateTime frm1 = Convert.ToDateTime(txt_frm_date);
            DateTime to1 = Convert.ToDateTime(txt_to_date);
            txt_frm_date = frm1.ToString("dd-MM-yyyy");
            txt_to_date = to1.ToString("dd-MM-yyyy");
            string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy",
                            "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
            DateTime dt = !string.IsNullOrWhiteSpace(txt_frm_date) ? DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
            DateTime dt1 = !string.IsNullOrWhiteSpace(txt_to_date) ? DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
            DateTime frm_date = Convert.ToDateTime(dt).Date;
            DateTime to_date = Convert.ToDateTime(dt1).Date.AddDays(1);



            var respo = _db.proc_Recharge_operator_report_newPaging(1, 100000, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), ddlusers, userid, ddl_status, optname, mobile, "ALL").ToList();

            return new ViewAsPdf(respo);
        }

        [HttpPost]
        public ActionResult Recharge_report_View(int Idno)
        {
            var details = _db.Recharge_report_View_Details(Idno);
            return Json(details, JsonRequestBehavior.AllowGet);
        }

        #region Giftcards
        public ActionResult Giftcard()
        {
            string userid = User.Identity.GetUserId();
            //show all Dealer 
            ViewBag.DealerId = new SelectList(_db.select_master_All_Dealer_for_ddl(userid), "DealerId", "DealerName", null).ToList();
            ViewBag.allretailer = new SelectList(_db.select_retailer_for_master_ddl(userid), "RetailerId", "RetailerName", null).ToList();
            var operator_value = _db.Operator_Code.Where(a => a.Operator_type == "DigitalVoucher").Distinct().ToList();
            ViewBag.Operator = new SelectList(operator_value, "new_opt_code", "operator_Name");
            string txt_frm_date = DateTime.Now.ToString();
            string txt_to_date = DateTime.Now.ToString();
            string frm_date = Convert.ToDateTime(txt_frm_date).Date.ToString("yyyy-MM-dd");
            string to_date = Convert.ToDateTime(txt_to_date).AddDays(1).ToString("yyyy-MM-dd");
            var Liverechargecount = _db.Gift_card_details_report(Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), userid, "", "WMASTER", "", "ALL", 50).ToList();
            var totalsuccesscount = Liverechargecount.Where(a => a.status.ToUpper() == "SUCCESS").Sum(a => a.amount);
            var totalfailed = Liverechargecount.Where(a => a.status.ToUpper() == "FAILED").Sum(a => a.amount);
            var totalpending = Liverechargecount.Where(a => a.status.ToUpper() == "PENDING").Sum(a => a.amount);
            ViewData["Totals"] = totalsuccesscount;
            ViewData["Totalf"] = totalfailed;
            ViewData["Totalp"] = totalpending;
            return View(Liverechargecount);
        }

        [HttpPost]
        public ActionResult Giftcard(string txt_frm_date, string txt_to_date, string ddl_status, string Operator, string ddl_top, string allretailer, string alldealerid, string ddlusers)
        {
            var userid = "";
            var optname = "";
            ViewBag.chk = "post";
            var masterid = User.Identity.GetUserId();
            ViewBag.DealerId = new SelectList(_db.select_master_All_Dealer_for_ddl(masterid), "DealerId", "DealerName", null).ToList();
            ViewBag.allretailer = new SelectList(_db.select_retailer_for_master_ddl(masterid), "RetailerId", "RetailerName", null).ToList();
            if (ddlusers == "WMASTER")
            {
                userid = masterid;
            }
            if (ddlusers == "Dealer")
            {
                if (alldealerid == "" || alldealerid.Contains("All Dealer") || alldealerid == null)
                {
                    userid = "";
                }
                else
                {
                    userid = alldealerid;
                }
            }
            if (ddlusers == "Retailer")
            {
                if (allretailer == "" || allretailer.Contains("All Retailer") || allretailer == null)
                {
                    userid = "";
                }
                else
                {
                    userid = allretailer;
                }
            }

            if (ddl_top == "All")
            {
                ddl_top = "1000000";
            }
            if (Operator == null || Operator == "" || Operator.Contains("Select Operator"))
            {
                optname = "";
            }
            else
            {
                optname = Operator;
            }

            if (ddl_status == "Status")
            {
                ddl_status = "ALL";
            }
            int ddltop = Convert.ToInt32(ddl_top);

            //show all retailer 

            var operator_value = _db.Operator_Code.Where(a => a.Operator_type == "DigitalVoucher").Distinct().ToList();
            ViewBag.Operator = new SelectList(operator_value, "new_opt_code", "operator_Name");
            DateTime frm = Convert.ToDateTime(txt_frm_date);
            DateTime to = Convert.ToDateTime(txt_to_date);
            txt_frm_date = frm.ToString("dd-MM-yyyy");
            txt_to_date = to.ToString("dd-MM-yyyy");
            string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy",
                            "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
            DateTime dt = !string.IsNullOrWhiteSpace(txt_frm_date) ? DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
            DateTime dt1 = !string.IsNullOrWhiteSpace(txt_to_date) ? DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
            DateTime frm_date = Convert.ToDateTime(dt).Date;
            DateTime to_date = Convert.ToDateTime(dt1).Date.AddDays(1);
            var Liverechargecount = _db.Gift_card_details_report(Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), userid, optname, ddlusers, "", ddl_status, ddltop).ToList();
            var totalsuccesscount = Liverechargecount.Where(a => a.status.ToUpper() == "SUCCESS").Sum(a => a.amount);
            var totalfailed = Liverechargecount.Where(a => a.status.ToUpper() == "FAILED").Sum(a => a.amount);
            var totalpending = Liverechargecount.Where(a => a.status.ToUpper() == "PENDING").Sum(a => a.amount);
            ViewData["Totals"] = totalsuccesscount;
            ViewData["Totalf"] = totalfailed;
            ViewData["Totalp"] = totalpending;
            return View(Liverechargecount);

        }
        #endregion
        #region
        public ActionResult Ecommerce_Report()
        {
            var userid = User.Identity.GetUserId();
            //show dealer
            ViewBag.alldealer = new SelectList(_db.select_Dealer_for_master_ddl(userid), "Dealerid", "DealerName", null).ToList();
            //show retailer
            ViewBag.allretailer = new SelectList(_db.select_retailer_for_master_ddl(userid), "RetailerId", "RetailerName", null).ToList();
            string txt_frm_date = DateTime.Now.ToString();
            string txt_to_date = DateTime.Now.ToString();
            var frm_date = Convert.ToDateTime(txt_frm_date).Date;
            var to_date = Convert.ToDateTime(txt_to_date).AddDays(1);
            var category = _db.Catagories.Distinct().ToList();
            ViewBag.category = new SelectList(category, "CatID", "CatName");
            var vv = _db.show_ecomm_report(userid, "WMASTER", "ALL", "", 50, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();

            var totalsuccess = vv.Where(a => a.OrderStatus == 2).Sum(a => a.Amount);
            var totalpending = vv.Where(a => a.OrderStatus == 1).Sum(a => a.Amount);
            var totalreject = vv.Where(a => a.OrderStatus == 3).Sum(a => a.Amount);
            ViewData["Totals"] = totalsuccess;
            ViewData["Totalp"] = totalpending;
            ViewData["Totalf"] = totalreject;
            return View(vv);
        }
        [HttpPost]
        public ActionResult Ecommerce_Report(string category, string txt_frm_date, string txt_to_date, string ddlusers, string alldealer, string allretailer, int ddl_top, string ddl_status)
        {
            var loginid = User.Identity.GetUserId();

            var userid = "";
            ViewBag.chk = "post";
            //show dealer
            ViewBag.alldealer = new SelectList(_db.select_Dealer_for_master_ddl(loginid), "Dealerid", "DealerName", null).ToList();
            //show retailer
            ViewBag.allretailer = new SelectList(_db.select_retailer_for_master_ddl(loginid), "RetailerId", "RetailerName", null).ToList();

            var category1 = _db.Catagories.Distinct().ToList();
            ViewBag.category = new SelectList(category1, "CatID", "CatName");

            DateTime frm = Convert.ToDateTime(txt_frm_date);
            DateTime to = Convert.ToDateTime(txt_to_date);


            txt_frm_date = frm.ToString("dd-MM-yyyy");
            txt_to_date = to.ToString("dd-MM-yyyy");

            string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy",
                            "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
            DateTime dt = DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            DateTime dt1 = DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            string frm_date = Convert.ToDateTime(dt).ToShortDateString();
            string to_date = Convert.ToDateTime(dt1).AddDays(1).ToString();


            if (ddlusers == "Dealer")
            {
                if (alldealer == "" || alldealer.Contains("Distubutor") || alldealer == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = alldealer;
                }
            }
            if (ddlusers == "Retailer")
            {
                if (allretailer == "" || allretailer.Contains("All Retailer") || allretailer == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = allretailer;
                }
            }

            if (ddlusers == "WMASTER")
            {
                userid = loginid;
            }

            var vv = _db.show_ecomm_report(userid, ddlusers, ddl_status, category, ddl_top, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();

            var totalsuccess = vv.Where(a => a.OrderStatus == 2).Sum(a => a.Amount);
            var totalpending = vv.Where(a => a.OrderStatus == 1).Sum(a => a.Amount);
            var totalreject = vv.Where(a => a.OrderStatus == 3).Sum(a => a.Amount);
            ViewData["Totals"] = totalsuccess;
            ViewData["Totalp"] = totalpending;
            ViewData["Totalf"] = totalreject;
            return View(vv);
        }

        #endregion
        // Retailer Ledger Report
        [HttpGet]
        public ActionResult MasterLedger()
        {
            var userid = User.Identity.GetUserId();
            string txt_to_date1 = DateTime.Now.ToString();
            string frm_date = Convert.ToDateTime(txt_to_date1).ToShortDateString();
            string to_date = Convert.ToDateTime(txt_to_date1).AddDays(1).ToString();
            var ledger = _db.Retailer_Cr_Dr_Report("WMASTER", userid, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            return View(ledger);
        }

        [HttpPost]
        public ActionResult MasterLedger(string txt_frm_date)
        {
            var userid = User.Identity.GetUserId();
            ViewBag.chk = "post";
            string txtfrm = DateTime.Now.Date.ToString();
            string frm_date = Convert.ToDateTime(txt_frm_date).Date.ToString();
            string to_date = Convert.ToDateTime(txt_frm_date).AddDays(1).ToString();
            var ledger = _db.Retailer_Cr_Dr_Report("WMASTER", userid, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            return View(ledger);

        }

        //Day Book Report
        [HttpGet]
        public ActionResult Master_Daybook_Report()
        {
            var userid = User.Identity.GetUserId();
            Daybookmasterreport rep = new Daybookmasterreport();
            string txt_frm_date = DateTime.Now.ToString();
            string txt_to_date = DateTime.Now.ToString();
            string frm_date = Convert.ToDateTime(txt_frm_date).Date.ToString();
            string to_date = Convert.ToDateTime(txt_to_date).AddDays(1).ToString();
            rep.DaybookLive = _db.daybook_master_report(userid, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            return View(rep);
        }
        [HttpPost]
        public ActionResult Master_Daybook_Report(string txt_frm_date)
        {
            var userid = User.Identity.GetUserId();
            ViewBag.chk = "post";
            Daybookmasterreport rep = new Daybookmasterreport();
            string txtfrm = DateTime.Now.Date.ToString();
            string frm_date = Convert.ToDateTime(txt_frm_date).Date.ToString();
            string to_date = Convert.ToDateTime(txt_frm_date).AddDays(1).ToString();
            if (txtfrm == frm_date)
            {
                rep.DaybookLive = _db.daybook_master_report(userid, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            }
            else
            {
                rep.DaybookOld = _db.daybook_master_old_report(userid, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            }
            return View(rep);

        }

        //Help and Support
        public ActionResult Help()
        {
            var admininfo = _db.Admin_details.FirstOrDefault();
            ViewBag.admin = admininfo;
            return View();
        }
        public class JsonModel
        {
            public string HTMLString { get; set; }
            public bool NoMoredata { get; set; }
        }
        protected string renderPartialViewtostring(string Viewname, object model)
        {
            if (string.IsNullOrEmpty(Viewname))

                Viewname = ControllerContext.RouteData.GetRequiredString("action");
            ViewData.Model = model;
            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewresult = ViewEngines.Engines.FindPartialView(ControllerContext, Viewname);
                ViewContext viewcontext = new ViewContext(ControllerContext, viewresult.View, ViewData, TempData, sw);
                viewresult.View.Render(viewcontext, sw);
                return sw.GetStringBuilder().ToString();
            }
        }
        // Money_Transfer_Report 
        [HttpGet]
        public ActionResult Money_Transfer_Report()
        {
            var userid = User.Identity.GetUserId();
            //show dealer
            ViewBag.alldealer = new SelectList(_db.select_Dealer_for_master_ddl(userid), "Dealerid", "DealerName", null).ToList();
            //show retailer
            ViewBag.allretailer = new SelectList(_db.select_retailer_for_master_ddl(userid), "RetailerId", "RetailerName", null).ToList();
            //api users 
            ViewBag.apiid = new SelectList(_db.select_apiusers_for_ddl("Admin"), "apiid", "username", null).ToList();
            //apiname
            var apiname = _db.money_api_status.ToList();
            IEnumerable<SelectListItem> selectapiname = from p in apiname
                                                        select new SelectListItem
                                                        {
                                                            Value = p.api_name,
                                                            Text = p.api_name
                                                        };
            ViewBag.apiname = new SelectList(selectapiname, "Value", "Text");

            return View();
        }
        [HttpPost]
        public ActionResult Money_Transfer_Report(string txt_frm_date, string txt_to_date, string ddlusers, string alldealer,
        string allretailer, string allapiuser, string ddl_top, string ddl_status, string btntype, string ddl_Type)
        {
            RETAILER.Models.Money_transfer_report money = new RETAILER.Models.Money_transfer_report();
            var loginid = User.Identity.GetUserId();

            //  var APIname = "";
            ViewBag.chk = "post";

            //show dealer
            ViewBag.alldealer = new SelectList(_db.select_Dealer_for_master_ddl(loginid), "Dealerid", "DealerName", null).ToList();
            //show retailer
            ViewBag.allretailer = new SelectList(_db.select_retailer_for_master_ddl(loginid), "RetailerId", "RetailerName", null).ToList();
            //api users 
            ViewBag.apiid = new SelectList(_db.select_apiusers_for_ddl("Admin"), "apiid", "username", null).ToList();
            //apiname
            var apiname = _db.money_api_status.ToList();
            IEnumerable<SelectListItem> selectapiname = from p in apiname
                                                        select new SelectListItem
                                                        {
                                                            Value = p.api_name,
                                                            Text = p.api_name
                                                        };
            ViewBag.apiname = new SelectList(selectapiname, "Value", "Text");

            //if (api_name == "Select Api Name" || api_name.Contains("Select Api Name") || api_name == "")
            //{
            //    APIname = "ALL";
            //}
            //else
            //{
            //    APIname = api_name.ToUpper();
            //}

            return View();
        }
        public ActionResult _Money_TransferReport(string txt_frm_date, string txt_to_date, string ddlusers, string alldealer, string allretailer, string allapiuser, string ddl_status, string ddl_Type)
        {

            var loginid = User.Identity.GetUserId();
            var userid = "";

            ViewBag.chk = "post";
            if (txt_frm_date == null && txt_to_date == null && ddl_status == null && ddlusers == null && ddl_Type == null)
            {
                txt_frm_date = DateTime.Now.ToString();
                txt_to_date = DateTime.Now.ToString();
                ddlusers = "Admin"; ddl_status = "ALL";
                ddl_Type = "ALL";

            }
            DateTime frm = Convert.ToDateTime(txt_frm_date);
            DateTime to = Convert.ToDateTime(txt_to_date);
            txt_frm_date = frm.ToString("dd-MM-yyyy");
            txt_to_date = to.ToString("dd-MM-yyyy");
            string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy",
                            "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
            DateTime dt = !string.IsNullOrWhiteSpace(txt_frm_date) ? DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
            DateTime dt1 = !string.IsNullOrWhiteSpace(txt_to_date) ? DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
            DateTime frm_date = Convert.ToDateTime(dt).Date;
            DateTime to_date = Convert.ToDateTime(dt1).Date.AddDays(1);

            if (ddlusers == "WMASTER")
            {
                userid = loginid;
            }

            if (ddlusers == "Dealer")
            {
                if (alldealer == "" || alldealer.Contains("Distubutor") || alldealer == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = alldealer;
                }
            }
            if (ddlusers == "Retailer")
            {
                if (allretailer == "" || allretailer.Contains("All Retailer") || allretailer == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = allretailer;
                }
            }


            if (ddl_status == "Status")
            {
                ddl_status = "ALL";
            }


            var ch = _db.money_transfer_report_paging(ddlusers, userid, ddl_status, "VASTWEB", Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), ddl_Type, 1, 20).ToList();


            return View(ch);
        }
        public ActionResult InfiniteScroll1(int pageindex, string txt_frm_date, string txt_to_date, string ddlusers, string alldealer, string allretailer, string ddl_status, string allapiuser, string ddl_Type)
        {
            string userid1 = User.Identity.GetUserId();

            var userid = "";
            if ((txt_frm_date == null || txt_frm_date == "") && (txt_to_date == null || txt_to_date == ""))
            {
                txt_frm_date = DateTime.Now.ToString();
                txt_to_date = DateTime.Now.ToString();

            }

            DateTime frm = Convert.ToDateTime(txt_frm_date);
            DateTime to = Convert.ToDateTime(txt_to_date);
            txt_frm_date = frm.ToString("dd-MM-yyyy");
            txt_to_date = to.ToString("dd-MM-yyyy");
            ViewBag.apiid = new SelectList(_db.select_apiusers_for_ddl("Admin"), "apiid", "username", null).ToList();
            string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy",
                            "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
            DateTime dt = DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            DateTime dt1 = DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            string frm_date = Convert.ToDateTime(dt).ToShortDateString();
            string to_date = Convert.ToDateTime(dt1).AddDays(1).ToString();


            if (ddlusers == "WMASTER")
            {
                userid = userid1;
            }
            if (ddlusers == "Dealer")
            {
                if (alldealer == "" || alldealer.Contains("alldealer") || alldealer == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = alldealer;
                }
            }
            if (ddlusers == "Retailer")
            {
                if (allretailer == "" || allretailer.Contains("allretailer") || allretailer == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = allretailer;
                }
            }

            if (ddlusers == "APIID")
            {
                if (allapiuser == "" || allapiuser.Contains("allapiuser") || allapiuser == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = allapiuser;
                }
            }



            if (ddlusers == "" || ddlusers == null)
            {
                ddlusers = "Admin";
            }

            if (ddl_status == "" || ddl_status == null)
            {
                ddl_status = "ALL";
            }





            System.Threading.Thread.Sleep(1000);
            int pagesize = 20;
            var tbrow = _db.money_transfer_report_paging(ddlusers, userid, ddl_status, "VASTWEB", Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), ddl_Type, pageindex, pagesize).ToList();

            JsonModel jsonmodel = new JsonModel();
            jsonmodel.NoMoredata = tbrow.Count < pagesize;
            jsonmodel.HTMLString = renderPartialViewtostring("_Money_TransferReport", tbrow);
            return Json(jsonmodel);
        }
        public ActionResult ExcelMoneyreport(string txt_frm_date, string txt_to_date, string ddlusers, string alldealer, string allretailer, string allapiuser, string ddl_status, string ddl_Type)
        {
            string loginuserid = User.Identity.GetUserId();

            var userid = "";

            if ((txt_frm_date == null || txt_frm_date == "") && (txt_to_date == null || txt_to_date == ""))
            {
                txt_frm_date = DateTime.Now.ToString();
                txt_to_date = DateTime.Now.ToString();

            }
            if (ddlusers == null)
            {

                ddlusers = "WMASTER";

            }

            if (ddlusers == "WMASTER")
            {
                userid = loginuserid;
            }
            if (ddlusers == "Dealer")
            {
                if (alldealer == "" || alldealer.Contains("Distubutor") || alldealer == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = alldealer;
                }
            }
            if (ddlusers == "Retailer")
            {
                if (allretailer == "" || allretailer.Contains("All Retailer") || allretailer == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = allretailer;
                }
            }

            if (ddl_status == "" || ddl_status == null || ddl_status.Contains("ALL STATUS"))
            {
                //optname = "ALL";
                ddl_status = "ALL";
            }
            if (ddl_Type == null)
            {
                ddl_Type = "ALL";

            }
            DateTime frm = Convert.ToDateTime(txt_frm_date);
            DateTime to = Convert.ToDateTime(txt_to_date);
            txt_frm_date = frm.ToString("dd-MM-yyyy");
            txt_to_date = to.ToString("dd-MM-yyyy");
            string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy",
                            "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
            DateTime dt = !string.IsNullOrWhiteSpace(txt_frm_date) ? DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
            DateTime dt1 = !string.IsNullOrWhiteSpace(txt_to_date) ? DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
            DateTime frm_date = Convert.ToDateTime(dt).Date;
            DateTime to_date = Convert.ToDateTime(dt1).Date.AddDays(1);

            DataTable dt2 = new DataTable();
            dt2.Columns.Add("Status", typeof(string));
            dt2.Columns.Add("Type", typeof(string));
            dt2.Columns.Add("Beneficiary Account", typeof(string));
            dt2.Columns.Add("Sender No", typeof(string));
            dt2.Columns.Add("Net T/F", typeof(string));
            dt2.Columns.Add("Transfer Total", typeof(string));
            dt2.Columns.Add("MY Income", typeof(string));
            dt2.Columns.Add("MY Pre", typeof(string));
            dt2.Columns.Add("Bank UTR", typeof(string));
            dt2.Columns.Add("Request Date", typeof(string));
            dt2.Columns.Add("Request ID", typeof(string));
            dt2.Columns.Add("Bank Name", typeof(string));
            dt2.Columns.Add("IFSC Code", typeof(string));
            dt2.Columns.Add("Response Time", typeof(string));
            dt2.Columns.Add("Retailer Pre", typeof(string));
            dt2.Columns.Add("Retailer Income", typeof(string));
            dt2.Columns.Add("Retailer Post", typeof(string));
            dt2.Columns.Add("Distributor Pre", typeof(string));
            dt2.Columns.Add("Distributor Income", typeof(string));
            dt2.Columns.Add("Distributor Post", typeof(string));

            var respo = _db.money_transfer_report_paging(ddlusers, userid, ddl_status, "VASTWEB", Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), ddl_Type, 1, 10000).ToList();
            foreach (var item in respo)
            {
                decimal amount = 0;
                decimal customerfee = 0;
                decimal total = 0;
                decimal debit = 0;
                decimal remcomm = 0;
                if (item.Trans_type.Contains("IMPS_VERIFY"))
                {
                    amount = Convert.ToDecimal(item.amount);
                    customerfee = Convert.ToDecimal(item.rem_comm);
                    customerfee = customerfee - amount;
                    total = amount + customerfee;
                    debit = Convert.ToDecimal(item.rem_comm);
                }
                else
                {
                    amount = Convert.ToDecimal(item.amount);
                    customerfee = Convert.ToDecimal(item.customer_fee);
                    total = amount + customerfee;
                    remcomm = Convert.ToDecimal(item.rem_comm);
                    debit = amount + remcomm;

                }

                dt2.Rows.Add(item.status + "=" + item.frm_name, item.Trans_type, item.accountno, item.senderno, item.totalamount, debit, item.md_income, item.md_remain,
                 item.bank_trans_id, item.trans_time, item.trans_id, item.bank_nm, item.ifsccode, item.response_time, item.user_remain_pre, item.income, item.user_remain
                , item.dlm_remain_pre, item.dlm_income, item.dlm_remain);
            }

            var grid = new GridView();
            grid.DataSource = dt2;
            grid.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=Report.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            return View();
        }
        public ActionResult PDFMoneyReport(string txt_frm_date, string txt_to_date, string ddlusers, string alldealer, string allretailer, string allapiuser, string ddl_status, string ddl_Type)
        {

            var loginid = User.Identity.GetUserId();
            var userid = "";

            if (txt_frm_date == null && txt_to_date == null && ddl_status == null && ddlusers == null && ddl_Type == null)
            {
                txt_frm_date = DateTime.Now.ToString();
                txt_to_date = DateTime.Now.ToString();
                ddlusers = "Admin"; ddl_status = "ALL";
                ddl_Type = "ALL";

            }
            DateTime frm = Convert.ToDateTime(txt_frm_date);
            DateTime to = Convert.ToDateTime(txt_to_date);

            txt_frm_date = frm.ToString("dd-MM-yyyy");
            txt_to_date = to.ToString("dd-MM-yyyy");
            string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy",
                            "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
            DateTime dt = !string.IsNullOrWhiteSpace(txt_frm_date) ? DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
            DateTime dt1 = !string.IsNullOrWhiteSpace(txt_to_date) ? DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
            DateTime frm_date = Convert.ToDateTime(dt).Date;
            DateTime to_date = Convert.ToDateTime(dt1).Date.AddDays(1);

            if (ddlusers == "WMASTER")
            {
                userid = loginid;
            }

            if (ddlusers == "Dealer")
            {
                if (alldealer == "" || alldealer.Contains("Distubutor") || alldealer == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = alldealer;
                }
            }
            if (ddlusers == "Retailer")
            {
                if (allretailer == "" || allretailer.Contains("All Retailer") || allretailer == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = allretailer;
                }
            }


            if (ddl_status == "Status")
            {
                ddl_status = "ALL";
            }

            if (ddl_Type == null)
            {
                ddl_Type = "ALL";
            }

            var respo = _db.money_transfer_report_paging(ddlusers, userid, ddl_status, "VASTWEB", Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), ddl_Type, 1, 100000).ToList();

            return new ViewAsPdf(respo);
        }
        public ActionResult Money_Tranfer_Details_View(int Idno)
        {
            var details = _db.Money_Tranfer_Details_View(Idno);
            return Json(details, JsonRequestBehavior.AllowGet);
        }
        public ActionResult MoneyTransfer_Total(string txt_frm_date, string txt_to_date, string ddlusers, string alldealer, string allretailer, string allapiuser, string ddl_status, string ddl_Type)
        {

            var loginid = User.Identity.GetUserId();
            var userid = "";
            if (txt_frm_date == null && txt_to_date == null && ddl_status == null && ddlusers == null && ddl_Type == null)
            {
                txt_frm_date = DateTime.Now.ToString();
                txt_to_date = DateTime.Now.ToString();
                ddlusers = "Admin"; ddl_status = "ALL";
                ddl_Type = "ALL";

            }
            DateTime frm = Convert.ToDateTime(txt_frm_date);
            DateTime to = Convert.ToDateTime(txt_to_date);

            txt_frm_date = frm.ToString("dd-MM-yyyy");
            txt_to_date = to.ToString("dd-MM-yyyy");
            string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy",
                            "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
            DateTime dt = !string.IsNullOrWhiteSpace(txt_frm_date) ? DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
            DateTime dt1 = !string.IsNullOrWhiteSpace(txt_to_date) ? DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
            DateTime frm_date = Convert.ToDateTime(dt).Date;
            DateTime to_date = Convert.ToDateTime(dt1).Date.AddDays(1);

            if (ddlusers == "WMASTER")
            {
                userid = loginid;
            }

            if (ddlusers == "Dealer")
            {
                if (alldealer == "" || alldealer.Contains("Distubutor") || alldealer == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = alldealer;
                }
            }
            if (ddlusers == "Retailer")
            {
                if (allretailer == "" || allretailer.Contains("All Retailer") || allretailer == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = allretailer;
                }
            }


            if (ddl_status == "Status")
            {
                ddl_status = "ALL";
            }
            if (ddl_Type == null)
            {
                ddl_Type = "ALL";

            }

            var ch = _db.money_transfer_report_paging(ddlusers, userid, ddl_status, "VASTWEB", Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), ddl_Type, 1, 100000).ToList();
            var successtotal1 = ch.Where(a => a.status.ToUpper() == "SUCCESS").Sum(a => a.totalamount);
            var Failedtotal1 = ch.Where(s => s.status.ToUpper().Contains("FAILED")).Sum(s => Convert.ToInt32(s.totalamount));
            var PendingTotal1 = ch.Where(s => s.status.ToUpper().Contains("PENDING")).Sum(s => Convert.ToInt32(s.totalamount));

            var data = new
            {
                success = successtotal1,
                failed = Failedtotal1,
                pending = PendingTotal1,


            };

            return Json(data, JsonRequestBehavior.AllowGet);
        }




        //#FlightReport
        [HttpGet]
        public ActionResult Flight_Report()
        {
            var userid = User.Identity.GetUserId();
            //show dealer
            ViewBag.alldealer = new SelectList(_db.select_Dealer_for_master_ddl(userid), "Dealerid", "DealerName", null).ToList();
            //show retailer
            ViewBag.allretailer = new SelectList(_db.select_retailer_for_master_ddl(userid), "RetailerId", "RetailerName", null).ToList();

            return View();
        }
        [HttpPost]
        public ActionResult Flight_Report(string txt_frm_date, string txt_to_date, int? ddl_top, string ddl_status, string PNR, string ddlusers, string alldealer, string allretailer)
        {
            var loginid = User.Identity.GetUserId();

            ViewBag.chk = "post";

            //show dealer
            ViewBag.alldealer = new SelectList(_db.select_Dealer_for_master_ddl(loginid), "Dealerid", "DealerName", null).ToList();
            //show retailer
            ViewBag.allretailer = new SelectList(_db.select_retailer_for_master_ddl(loginid), "RetailerId", "RetailerName", null).ToList();



            return View();
        }
        public ActionResult _TicketReport(string txt_frm_date, string txt_to_date, string ddl_status, string PNR, string ddlusers, string alldealer, string allretailer)
        {
            if ((txt_frm_date == null || txt_frm_date == "") && (txt_to_date == null || txt_to_date == ""))
            {
                txt_frm_date = DateTime.Now.ToString();
                txt_to_date = DateTime.Now.ToString();
                ddlusers = "WMASTER";

            }
            string loginuserid = User.Identity.GetUserId();
            ViewBag.chk = "post";
            string retailerid = null;
            string DealerId = null;
            string MasterId = null;

            DateTime frm = Convert.ToDateTime(txt_frm_date);
            DateTime to = Convert.ToDateTime(txt_to_date);
            txt_frm_date = frm.ToString("dd-MM-yyyy");
            txt_to_date = to.ToString("dd-MM-yyyy");
            string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
            DateTime dt = DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            DateTime dt1 = DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            DateTime frm_date = dt.Date;
            DateTime to_date = dt1.AddDays(1);

            PNR = string.IsNullOrWhiteSpace(PNR) ? null : PNR;
            //ddl_status = string.IsNullOrWhiteSpace(ddl_status) ? null : ddl_status;

            if (ddlusers == "Dealer")
            {
                if (alldealer == "" || alldealer.Contains("All Distubutor") || alldealer == null)
                {
                    DealerId = null;
                }
                else
                {
                    DealerId = alldealer;
                }
            }
            if (ddlusers == "Retailer")
            {
                if (allretailer == "" || allretailer.Contains("All Retailer") || allretailer == null)
                {
                    retailerid = null;
                }
                else
                {
                    retailerid = allretailer;
                }
            }
            //if (ddlusers == "Whitelabel")
            //{
            //    if (allwhitelabel == "" || allwhitelabel.Contains("All Retailer") || allwhitelabel == null)
            //    {
            //        userid = "ALL";
            //    }
            //    else
            //    {
            //        userid = allwhitelabel;
            //    }
            //}
            if (ddlusers == "WMASTER")
            {
                MasterId = loginuserid;
                retailerid = null;
                DealerId = null;

            }
            var proc_Response = _db.proc_FlightReport(1, 20, ddl_status, retailerid, DealerId, MasterId, null, null, PNR, null, null, frm_date, to_date).ToList();

            return View(proc_Response);


        }
        public ActionResult InfiniteScroll_Ticket(int pageindex, string txt_frm_date, string txt_to_date, string ddl_status, string PNR, string ddlusers, string alldealer, string allretailer)
        {
            if ((txt_frm_date == null || txt_frm_date == "") && (txt_to_date == null || txt_to_date == ""))
            {
                txt_frm_date = DateTime.Now.ToString();
                txt_to_date = DateTime.Now.ToString();
                ddlusers = "WMASTER";

            }
            string loginuserid = User.Identity.GetUserId();
            ViewBag.chk = "post";
            string retailerid = null;
            string DealerId = null;
            string MasterId = null;

            DateTime frm = Convert.ToDateTime(txt_frm_date);
            DateTime to = Convert.ToDateTime(txt_to_date);
            txt_frm_date = frm.ToString("dd-MM-yyyy");
            txt_to_date = to.ToString("dd-MM-yyyy");
            string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
            DateTime dt = DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            DateTime dt1 = DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            DateTime frm_date = dt.Date;
            DateTime to_date = dt1.AddDays(1);

            PNR = string.IsNullOrWhiteSpace(PNR) ? null : PNR;
            //ddl_status = string.IsNullOrWhiteSpace(ddl_status) ? null : ddl_status;

            if (ddlusers == "Dealer")
            {
                if (alldealer == "" || alldealer.Contains("All Distubutor") || alldealer == null)
                {
                    DealerId = null;
                }
                else
                {
                    DealerId = alldealer;
                }
            }
            if (ddlusers == "Retailer")
            {
                if (allretailer == "" || allretailer.Contains("All Retailer") || allretailer == null)
                {
                    retailerid = null;
                }
                else
                {
                    retailerid = allretailer;
                }
            }
            //if (ddlusers == "Whitelabel")
            //{
            //    if (allwhitelabel == "" || allwhitelabel.Contains("All Retailer") || allwhitelabel == null)
            //    {
            //        userid = "ALL";
            //    }
            //    else
            //    {
            //        userid = allwhitelabel;
            //    }
            //}
            if (ddlusers == "WMASTER")
            {
                MasterId = loginuserid;
                retailerid = null;
                DealerId = null;

            }
            int pagesize = 20;
            var tbrow = _db.proc_FlightReport(pageindex, pagesize, ddl_status, retailerid, DealerId, MasterId, null, null, PNR, null, null, frm_date, to_date).ToList();
            JsonModel jsonmodel = new JsonModel();
            jsonmodel.NoMoredata = tbrow.Count < pagesize;
            jsonmodel.HTMLString = renderPartialViewtostring("_TicketReport", tbrow);
            return Json(jsonmodel);
        }
        public virtual ActionResult ExcelFilghtReport(string txt_frm_date, string txt_to_date, string ddl_status, string PNR, string ddlusers, string alldealer, string allretailer)
        {
            string loginuserid = User.Identity.GetUserId();
            ViewBag.chk = "post";
            string retailerid = null;
            string DealerId = null;
            string MasterId = null;

            DateTime frm = Convert.ToDateTime(txt_frm_date);
            DateTime to = Convert.ToDateTime(txt_to_date);
            txt_frm_date = frm.ToString("dd-MM-yyyy");
            txt_to_date = to.ToString("dd-MM-yyyy");
            string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
            DateTime dt = DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            DateTime dt1 = DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            DateTime frm_date = dt.Date;
            DateTime to_date = dt1.AddDays(1);

            PNR = string.IsNullOrWhiteSpace(PNR) ? null : PNR;
            //ddl_status = string.IsNullOrWhiteSpace(ddl_status) ? null : ddl_status;

            if (ddlusers == "Dealer")
            {
                if (alldealer == "" || alldealer.Contains("All Distubutor") || alldealer == null)
                {
                    DealerId = null;
                }
                else
                {
                    DealerId = alldealer;
                }
            }
            if (ddlusers == "Retailer")
            {
                if (allretailer == "" || allretailer.Contains("All Retailer") || allretailer == null)
                {
                    retailerid = null;
                }
                else
                {
                    retailerid = allretailer;
                }
            }
            //if (ddlusers == "Whitelabel")
            //{
            //    if (allwhitelabel == "" || allwhitelabel.Contains("All Retailer") || allwhitelabel == null)
            //    {
            //        userid = "ALL";
            //    }
            //    else
            //    {
            //        userid = allwhitelabel;
            //    }
            //}
            if (ddlusers == "WMASTER")
            {
                MasterId = loginuserid;
                retailerid = null;
                DealerId = null;

            }

            DataTable dt2 = new DataTable();
            dt2.Columns.Add("Status", typeof(string));
            dt2.Columns.Add("PassangerName", typeof(string));
            dt2.Columns.Add("Flight", typeof(string));
            dt2.Columns.Add("Booking Id", typeof(string));
            dt2.Columns.Add("PNR ", typeof(string));
            dt2.Columns.Add("Fare Amount", typeof(string));
            dt2.Columns.Add("User Income", typeof(string));

            dt2.Columns.Add("MY Pre", typeof(string));
            dt2.Columns.Add("MY Income", typeof(string));
            dt2.Columns.Add("MY Post", typeof(string));
            dt2.Columns.Add("Distributor Income", typeof(string));

            dt2.Columns.Add("Booking Date", typeof(string));

            var respo = _db.proc_FlightReport(1, 100000, ddl_status, retailerid, DealerId, MasterId, null, null, PNR, null, null, frm_date, to_date).ToList();
            foreach (var item in respo)
            {
                var sts = item.TicketStatus;

                if (item.TicketStatus.Contains("Refund"))
                {
                    sts = "Refund";
                }
                dt2.Rows.Add(sts + "" + item.Frm_Name, item.LeadPaxFirstName, item.AirlineName,
                    item.BookingId, item.PNR,
                item.OfferedFare, item.RemInc, item.MdPre, item.MdInc, item.MdPost, item.DlmInc,
               item.TicketDate);
            }

            var grid = new GridView();
            grid.DataSource = dt2;
            grid.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=Report.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            return View();
        }
        public ActionResult TicketReport_Total(string txt_frm_date, string txt_to_date, string ddl_status, string PNR, string ddlusers, string alldealer, string allretailer)
        {

            string loginuserid = User.Identity.GetUserId();

            string retailerid = null;
            string DealerId = null;
            string MasterId = null;

            DateTime frm = Convert.ToDateTime(txt_frm_date);
            DateTime to = Convert.ToDateTime(txt_to_date);
            txt_frm_date = frm.ToString("dd-MM-yyyy");
            txt_to_date = to.ToString("dd-MM-yyyy");
            string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
            DateTime dt = DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            DateTime dt1 = DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            DateTime frm_date = dt.Date;
            DateTime to_date = dt1.AddDays(1);

            PNR = string.IsNullOrWhiteSpace(PNR) ? null : PNR;
            //ddl_status = string.IsNullOrWhiteSpace(ddl_status) ? null : ddl_status;

            if (ddlusers == "Dealer")
            {
                if (alldealer == "" || alldealer.Contains("All Distubutor") || alldealer == null)
                {
                    DealerId = null;
                }
                else
                {
                    DealerId = alldealer;
                }
            }
            if (ddlusers == "Retailer")
            {
                if (allretailer == "" || allretailer.Contains("All Retailer") || allretailer == null)
                {
                    retailerid = null;
                }
                else
                {
                    retailerid = allretailer;
                }
            }
            //if (ddlusers == "Whitelabel")
            //{
            //    if (allwhitelabel == "" || allwhitelabel.Contains("All Retailer") || allwhitelabel == null)
            //    {
            //        userid = "ALL";
            //    }
            //    else
            //    {
            //        userid = allwhitelabel;
            //    }
            //}
            if (ddlusers == "WMASTER")
            {
                MasterId = loginuserid;
                retailerid = null;
                DealerId = null;

            }
            var proc_Response = _db.proc_FlightReport(1, 20, ddl_status, retailerid, DealerId, MasterId, null, null, PNR, null, null, frm_date, to_date).ToList();
            var successtotal1 = proc_Response.Where(s => s.TicketStatus.ToUpper().Contains("SUCCESS")).Sum(s => Convert.ToInt32(s.FareAmount));
            var Failedtotal1 = proc_Response.Where(s => s.TicketStatus.ToUpper().Contains("FAILED")).Sum(s => Convert.ToInt32(s.FareAmount));
            var PendingTotal1 = proc_Response.Where(s => s.TicketStatus.ToUpper().Contains("Proccessed")).Sum(s => Convert.ToInt32(s.FareAmount));
            var data = new
            {
                success = successtotal1,
                failed = Failedtotal1,
                pending = PendingTotal1,

            };

            return Json(data, JsonRequestBehavior.AllowGet);

        }
        public ActionResult PDF_FilghtReport(string txt_frm_date, string txt_to_date, string ddl_status, string PNR, string ddlusers, string alldealer, string allretailer)
        {

            string loginuserid = User.Identity.GetUserId();

            string retailerid = null;
            string DealerId = null;
            string MasterId = null;

            DateTime frm = Convert.ToDateTime(txt_frm_date);
            DateTime to = Convert.ToDateTime(txt_to_date);
            txt_frm_date = frm.ToString("dd-MM-yyyy");
            txt_to_date = to.ToString("dd-MM-yyyy");
            string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
            DateTime dt = DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            DateTime dt1 = DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            DateTime frm_date = dt.Date;
            DateTime to_date = dt1.AddDays(1);

            PNR = string.IsNullOrWhiteSpace(PNR) ? null : PNR;
            //ddl_status = string.IsNullOrWhiteSpace(ddl_status) ? null : ddl_status;

            if (ddlusers == "Dealer")
            {
                if (alldealer == "" || alldealer.Contains("All Distubutor") || alldealer == null)
                {
                    DealerId = null;
                }
                else
                {
                    DealerId = alldealer;
                }
            }
            if (ddlusers == "Retailer")
            {
                if (allretailer == "" || allretailer.Contains("All Retailer") || allretailer == null)
                {
                    retailerid = null;
                }
                else
                {
                    retailerid = allretailer;
                }
            }
            //if (ddlusers == "Whitelabel")
            //{
            //    if (allwhitelabel == "" || allwhitelabel.Contains("All Retailer") || allwhitelabel == null)
            //    {
            //        userid = "ALL";
            //    }
            //    else
            //    {
            //        userid = allwhitelabel;
            //    }
            //}
            if (ddlusers == "WMASTER")
            {
                MasterId = loginuserid;
                retailerid = null;
                DealerId = null;

            }
            var respo = _db.proc_FlightReport(1, 20, ddl_status, retailerid, DealerId, MasterId, null, null, PNR, null, null, frm_date, to_date).ToList();
            return new ViewAsPdf(respo);



        }

        [HttpGet]
        public ActionResult CancellationReport()
        {
            var userid = User.Identity.GetUserId();
            //show dealer
            ViewBag.alldealer = new SelectList(_db.select_Dealer_for_master_ddl(userid), "Dealerid", "DealerName", null).ToList();
            //show retailer
            ViewBag.allretailer = new SelectList(_db.select_retailer_for_master_ddl(userid), "RetailerId", "RetailerName", null).ToList();
            string txt_frm_date = DateTime.Now.ToString();
            string txt_to_date = DateTime.Now.ToString();
            var frm_date = Convert.ToDateTime(txt_frm_date).Date;
            var to_date = Convert.ToDateTime(txt_to_date).AddDays(1);
            var procRespo = _db.proc_FlightCancellationReport(1, 20, "ALL", null, null, userid, null, null, null, frm_date, to_date).ToList();
            return View(procRespo);
        }
        public ActionResult CancellationReport(string txt_frm_date, string txt_to_date, int? ddl_top, string ddl_status, string PNR, string ddlusers, string alldealer, string allretailer)
        {
            var loginid = User.Identity.GetUserId();
            string retailerid = null;
            string DealerId = null;
            string userid = null;
            //show dealer
            ViewBag.alldealer = new SelectList(_db.select_Dealer_for_master_ddl(loginid), "Dealerid", "DealerName", null).ToList();
            //show retailer
            ViewBag.allretailer = new SelectList(_db.select_retailer_for_master_ddl(loginid), "RetailerId", "RetailerName", null).ToList();

            DateTime frm = Convert.ToDateTime(txt_frm_date);
            DateTime to = Convert.ToDateTime(txt_to_date);
            txt_frm_date = frm.ToString("dd-MM-yyyy");
            txt_to_date = to.ToString("dd-MM-yyyy");
            string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
            DateTime dt = DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            DateTime dt1 = DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            DateTime frm_date = dt.Date;
            DateTime to_date = dt1.AddDays(1);
            if (ddl_top == null)
            {
                ddl_top = 10000;
            }
            PNR = string.IsNullOrWhiteSpace(PNR) ? null : PNR;
            ddl_status = string.IsNullOrWhiteSpace(ddl_status) ? "ALL" : ddl_status;
            if (ddlusers == "WMASTER")
            {
                userid = loginid;
            }
            if (ddlusers == "Dealer")
            {
                if (alldealer == "" || alldealer.Contains("Distubutor") || alldealer == null)
                {
                    DealerId = null;
                }
                else
                {
                    DealerId = alldealer;
                }
            }
            if (ddlusers == "Retailer")
            {
                if (allretailer == "" || allretailer.Contains("All Retailer") || allretailer == null)
                {
                    retailerid = null;
                }
                else
                {
                    retailerid = allretailer;
                }
            }

            var procRespo = _db.proc_FlightCancellationReport(1, 20, ddl_status, retailerid, DealerId, userid, null, null, PNR, frm_date, to_date).ToList();
            return View(procRespo);

        }

        //#BusRport
        [HttpGet]
        public ActionResult Bus_Report()
        {
            var userid = User.Identity.GetUserId();
            //show dealer
            ViewBag.alldealer = new SelectList(_db.select_Dealer_for_master_ddl(userid), "Dealerid", "DealerName", null).ToList();
            //show retailer
            ViewBag.allretailer = new SelectList(_db.select_retailer_for_master_ddl(userid), "RetailerId", "RetailerName", null).ToList();

            return View();
        }
        [HttpPost]
        public ActionResult Bus_Report(string ddl_status, int? ddl_top, string txt_frm_date, string txt_to_date, string ddlusers, string alldealer, string allretailer, string TicketNo)
        {
            ViewBag.chk = "post";
            var loginid = User.Identity.GetUserId();
            //show dealer
            ViewBag.alldealer = new SelectList(_db.select_Dealer_for_master_ddl(loginid), "Dealerid", "DealerName", null).ToList();
            //show retailer
            ViewBag.allretailer = new SelectList(_db.select_retailer_for_master_ddl(loginid), "RetailerId", "RetailerName", null).ToList();

            return View();
        }
        public ActionResult _Bus_Report(string ddl_status, string txt_frm_date, string txt_to_date, string ddlusers, string alldealer, string allretailer, string TicketNo)
        {
            var loginid = User.Identity.GetUserId();

            if ((txt_frm_date == null || txt_frm_date == "") && (txt_to_date == null || txt_to_date == ""))
            {
                txt_frm_date = DateTime.Now.ToString();
                txt_to_date = DateTime.Now.ToString();
                ddlusers = "WMASTER";

            }
            string retailerid = null;
            string DealerId = null;
            string userid = null;
            //show dealer
            DateTime frm = Convert.ToDateTime(txt_frm_date);
            DateTime to = Convert.ToDateTime(txt_to_date);
            txt_frm_date = frm.ToString("dd-MM-yyyy");
            txt_to_date = to.ToString("dd-MM-yyyy");
            string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
            DateTime dt = DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            DateTime dt1 = DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            DateTime frm_date = dt.Date;
            DateTime to_date = dt1.AddDays(1);

            TicketNo = string.IsNullOrWhiteSpace(TicketNo) ? null : TicketNo;
            ddl_status = string.IsNullOrWhiteSpace(ddl_status) ? null : ddl_status;
            if (ddlusers == "WMASTER")
            {
                userid = loginid;
            }
            if (ddlusers == "Dealer")
            {
                if (alldealer == "" || alldealer.Contains("Distubutor") || alldealer == null)
                {
                    DealerId = null;
                }
                else
                {
                    DealerId = alldealer;
                }
            }
            if (ddlusers == "Retailer")
            {
                if (allretailer == "" || allretailer.Contains("All Retailer") || allretailer == null)
                {
                    retailerid = null;
                }
                else
                {
                    retailerid = allretailer;
                }
            }
            var proc_Response = _db.proc_BusReport(1, 20, ddl_status, retailerid, DealerId, userid, null, TicketNo, null, null, null, frm_date, to_date).ToList();
            return View(proc_Response);
        }
        public ActionResult InfiniteScrollBus(int pageindex, string ddl_status, string txt_frm_date, string txt_to_date, string ddlusers, string alldealer, string allretailer, string TicketNo)
        {
            var loginid = User.Identity.GetUserId();

            string retailerid = null;
            string DealerId = null;
            string userid = null;
            //show dealer
            DateTime frm = Convert.ToDateTime(txt_frm_date);
            DateTime to = Convert.ToDateTime(txt_to_date);
            txt_frm_date = frm.ToString("dd-MM-yyyy");
            txt_to_date = to.ToString("dd-MM-yyyy");
            string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
            DateTime dt = DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            DateTime dt1 = DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            DateTime frm_date = dt.Date;
            DateTime to_date = dt1.AddDays(1);

            TicketNo = string.IsNullOrWhiteSpace(TicketNo) ? null : TicketNo;
            ddl_status = string.IsNullOrWhiteSpace(ddl_status) ? null : ddl_status;
            if (ddlusers == "WMASTER")
            {
                userid = loginid;
            }
            if (ddlusers == "Dealer")
            {
                if (alldealer == "" || alldealer.Contains("Distubutor") || alldealer == null)
                {
                    DealerId = null;
                }
                else
                {
                    DealerId = alldealer;
                }
            }
            if (ddlusers == "Retailer")
            {
                if (allretailer == "" || allretailer.Contains("All Retailer") || allretailer == null)
                {
                    retailerid = null;
                }
                else
                {
                    retailerid = allretailer;
                }
            }
            int pagesize = 20;
            var tbrow = _db.proc_BusReport(pageindex, pagesize, ddl_status, retailerid, DealerId, userid, null, TicketNo, null, null, null, frm_date, to_date).ToList();
            JsonModel jsonmodel = new JsonModel();
            jsonmodel.NoMoredata = tbrow.Count < pagesize;
            jsonmodel.HTMLString = renderPartialViewtostring("_Bus_Report", tbrow);
            return Json(jsonmodel);
        }
        public ActionResult Bus_ReportTotal(string ddl_status, string txt_frm_date, string txt_to_date, string ddlusers, string alldealer, string allretailer, string TicketNo)
        {
            var loginid = User.Identity.GetUserId();

            if ((txt_frm_date == null || txt_frm_date == "") && (txt_to_date == null || txt_to_date == ""))
            {
                txt_frm_date = DateTime.Now.ToString();
                txt_to_date = DateTime.Now.ToString();
                ddlusers = "WMASTER";

            }
            string retailerid = null;
            string DealerId = null;
            string userid = null;
            //show dealer
            DateTime frm = Convert.ToDateTime(txt_frm_date);
            DateTime to = Convert.ToDateTime(txt_to_date);
            txt_frm_date = frm.ToString("dd-MM-yyyy");
            txt_to_date = to.ToString("dd-MM-yyyy");
            string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
            DateTime dt = DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            DateTime dt1 = DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            DateTime frm_date = dt.Date;
            DateTime to_date = dt1.AddDays(1);

            TicketNo = string.IsNullOrWhiteSpace(TicketNo) ? null : TicketNo;
            ddl_status = string.IsNullOrWhiteSpace(ddl_status) ? null : ddl_status;
            if (ddlusers == "WMASTER")
            {
                userid = loginid;
            }
            if (ddlusers == "Dealer")
            {
                if (alldealer == "" || alldealer.Contains("Distubutor") || alldealer == null)
                {
                    DealerId = null;
                }
                else
                {
                    DealerId = alldealer;
                }
            }
            if (ddlusers == "Retailer")
            {
                if (allretailer == "" || allretailer.Contains("All Retailer") || allretailer == null)
                {
                    retailerid = null;
                }
                else
                {
                    retailerid = allretailer;
                }
            }
            var proc_Response = _db.proc_BusReport(1, 1000000, ddl_status, retailerid, DealerId, userid, null, TicketNo, null, null, null, frm_date, to_date).ToList();
            var successtotal1 = proc_Response.Where(s => s.TicketStatus.ToUpper().Contains("SUCCESS")).Sum(s => Convert.ToInt32(s.FareAmount));
            var Failedtotal1 = proc_Response.Where(s => s.TicketStatus.ToUpper().Contains("FAILED")).Sum(s => Convert.ToInt32(s.FareAmount));
            var PendingTotal1 = proc_Response.Where(s => s.TicketStatus.ToUpper().Contains("Proccessed")).Sum(s => Convert.ToInt32(s.FareAmount));
            var data = new
            {
                success = successtotal1,
                failed = Failedtotal1,
                pending = PendingTotal1,

            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public virtual ActionResult ExcelBusReport(string ddl_status, string txt_frm_date, string txt_to_date, string ddlusers, string alldealer, string allretailer, string TicketNo)
        {
            var loginid = User.Identity.GetUserId();


            string retailerid = null;
            string DealerId = null;
            string userid = null;
            //show dealer
            DateTime frm = Convert.ToDateTime(txt_frm_date);
            DateTime to = Convert.ToDateTime(txt_to_date);
            txt_frm_date = frm.ToString("dd-MM-yyyy");
            txt_to_date = to.ToString("dd-MM-yyyy");
            string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
            DateTime dt = DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            DateTime dt1 = DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            DateTime frm_date = dt.Date;
            DateTime to_date = dt1.AddDays(1);

            TicketNo = string.IsNullOrWhiteSpace(TicketNo) ? null : TicketNo;
            ddl_status = string.IsNullOrWhiteSpace(ddl_status) ? null : ddl_status;
            if (ddlusers == "WMASTER")
            {
                userid = loginid;
            }
            if (ddlusers == "Dealer")
            {
                if (alldealer == "" || alldealer.Contains("Distubutor") || alldealer == null)
                {
                    DealerId = null;
                }
                else
                {
                    DealerId = alldealer;
                }
            }
            if (ddlusers == "Retailer")
            {
                if (allretailer == "" || allretailer.Contains("All Retailer") || allretailer == null)
                {
                    retailerid = null;
                }
                else
                {
                    retailerid = allretailer;
                }
            }

            DataTable dt2 = new DataTable();
            dt2.Columns.Add("Status", typeof(string));
            dt2.Columns.Add("PassangerName", typeof(string));
            dt2.Columns.Add("Booking No", typeof(string));
            dt2.Columns.Add("PNR", typeof(string));

            dt2.Columns.Add("Total Pass.", typeof(string));

            dt2.Columns.Add("Fare", typeof(string));
            dt2.Columns.Add("User Income", typeof(string));
            dt2.Columns.Add("MY Income", typeof(string));
            dt2.Columns.Add("MY Post", typeof(string));
            dt2.Columns.Add("Distributor Income", typeof(string));
            dt2.Columns.Add("Journey Date", typeof(string));
            dt2.Columns.Add("Booking Date", typeof(string));

            var respo = _db.proc_BusReport(1, 100000, ddl_status, retailerid, DealerId, userid, null, TicketNo, null, null, null, frm_date, to_date).ToList();
            foreach (var item in respo)
            {

                dt2.Rows.Add(item.TicketStatus + "" + item.Frm_Name, item.PassengerName, item.TicketNo,
                    item.PNR,
             item.totalseat, item.FareAmount, item.RemInc, item.MdInc, item.MdPost,
               item.DlmInc, item.dateOfJourney, item.TicketDate);
            }

            var grid = new GridView();
            grid.DataSource = dt2;
            grid.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=Report.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            return View();
        }
        public ActionResult PDFBus_Report(string ddl_status, string txt_frm_date, string txt_to_date, string ddlusers, string alldealer, string allretailer, string TicketNo)
        {
            var loginid = User.Identity.GetUserId();

            if ((txt_frm_date == null || txt_frm_date == "") && (txt_to_date == null || txt_to_date == ""))
            {
                txt_frm_date = DateTime.Now.ToString();
                txt_to_date = DateTime.Now.ToString();
                ddlusers = "WMASTER";

            }
            string retailerid = null;
            string DealerId = null;
            string userid = null;
            //show dealer
            DateTime frm = Convert.ToDateTime(txt_frm_date);
            DateTime to = Convert.ToDateTime(txt_to_date);
            txt_frm_date = frm.ToString("dd-MM-yyyy");
            txt_to_date = to.ToString("dd-MM-yyyy");
            string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
            DateTime dt = DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            DateTime dt1 = DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            DateTime frm_date = dt.Date;
            DateTime to_date = dt1.AddDays(1);

            TicketNo = string.IsNullOrWhiteSpace(TicketNo) ? null : TicketNo;
            ddl_status = string.IsNullOrWhiteSpace(ddl_status) ? null : ddl_status;
            if (ddlusers == "WMASTER")
            {
                userid = loginid;
            }
            if (ddlusers == "Dealer")
            {
                if (alldealer == "" || alldealer.Contains("Distubutor") || alldealer == null)
                {
                    DealerId = null;
                }
                else
                {
                    DealerId = alldealer;
                }
            }
            if (ddlusers == "Retailer")
            {
                if (allretailer == "" || allretailer.Contains("All Retailer") || allretailer == null)
                {
                    retailerid = null;
                }
                else
                {
                    retailerid = allretailer;
                }
            }
            var respo = _db.proc_BusReport(1, 100000, ddl_status, retailerid, DealerId, userid, null, TicketNo, null, null, null, frm_date, to_date).ToList();
            return new ViewAsPdf(respo);
        }

        //#HotalReport

        [HttpGet]
        public ActionResult HotelReport()
        {
            var userid = User.Identity.GetUserId();
            //show dealer
            ViewBag.alldealer = new SelectList(_db.select_Dealer_for_master_ddl(userid), "Dealerid", "DealerName", null).ToList();
            //show retailer
            ViewBag.allretailer = new SelectList(_db.select_retailer_for_master_ddl(userid), "RetailerId", "RetailerName", null).ToList();
            return View();
        }
        public ActionResult HotelReport(string ddl_status, int? ddl_top, string txt_frm_date, string txt_to_date, string ddlusers, string alldealer, string allretailer, string TXNID)
        {

            var loginid = User.Identity.GetUserId();
            ViewBag.chk = "post";
            //show dealer
            ViewBag.alldealer = new SelectList(_db.select_Dealer_for_master_ddl(loginid), "Dealerid", "DealerName", null).ToList();
            //show retailer
            ViewBag.allretailer = new SelectList(_db.select_retailer_for_master_ddl(loginid), "RetailerId", "RetailerName", null).ToList();

            return View();
        }
        public ActionResult _HotelReport(string ddl_status, string txt_frm_date, string txt_to_date, string ddlusers, string alldealer, string allretailer, string TXNID)
        {
            if ((txt_frm_date == null || txt_frm_date == "") && (txt_to_date == null || txt_to_date == ""))
            {
                txt_frm_date = DateTime.Now.ToString();
                txt_to_date = DateTime.Now.ToString();
                ddlusers = "WMASTER";

            }
            var loginid = User.Identity.GetUserId();
            string retailerid = null;
            string DealerId = null;
            string userid = null;


            DateTime frm = Convert.ToDateTime(txt_frm_date);
            DateTime to = Convert.ToDateTime(txt_to_date);
            txt_frm_date = frm.ToString("dd-MM-yyyy");
            txt_to_date = to.ToString("dd-MM-yyyy");
            string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
            DateTime dt = DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            DateTime dt1 = DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            DateTime frm_date = dt.Date;
            DateTime to_date = dt1.AddDays(1);

            TXNID = string.IsNullOrWhiteSpace(TXNID) ? null : TXNID;
            ddl_status = string.IsNullOrWhiteSpace(ddl_status) ? null : ddl_status;
            if (ddlusers == "WMASTER")
            {
                userid = loginid;
            }
            if (ddlusers == "Dealer")
            {
                if (alldealer == "" || alldealer.Contains("Distubutor") || alldealer == null)
                {
                    DealerId = null;
                }
                else
                {
                    DealerId = alldealer;
                }
            }
            if (ddlusers == "Retailer")
            {
                if (allretailer == "" || allretailer.Contains("All Retailer") || allretailer == null)
                {
                    retailerid = null;
                }
                else
                {
                    retailerid = allretailer;
                }
            }

            var proc_Response = _db.proc_HotelReport(1, 20, ddl_status, retailerid, DealerId, userid, null, null, TXNID, null, null, frm_date, to_date).ToList();

            return View(proc_Response);
        }
        public ActionResult InfiniteScrollHotelReport(int pageindex, string ddl_status, string txt_frm_date, string txt_to_date, string ddlusers, string alldealer, string allretailer, string TXNID)
        {
            if ((txt_frm_date == null || txt_frm_date == "") && (txt_to_date == null || txt_to_date == ""))
            {
                txt_frm_date = DateTime.Now.ToString();
                txt_to_date = DateTime.Now.ToString();
                ddlusers = "WMASTER";

            }
            var loginid = User.Identity.GetUserId();
            string retailerid = null;
            string DealerId = null;
            string userid = null;


            DateTime frm = Convert.ToDateTime(txt_frm_date);
            DateTime to = Convert.ToDateTime(txt_to_date);
            txt_frm_date = frm.ToString("dd-MM-yyyy");
            txt_to_date = to.ToString("dd-MM-yyyy");
            string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
            DateTime dt = DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            DateTime dt1 = DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            DateTime frm_date = dt.Date;
            DateTime to_date = dt1.AddDays(1);

            TXNID = string.IsNullOrWhiteSpace(TXNID) ? null : TXNID;
            ddl_status = string.IsNullOrWhiteSpace(ddl_status) ? null : ddl_status;
            if (ddlusers == "WMASTER")
            {
                userid = loginid;
            }
            if (ddlusers == "Dealer")
            {
                if (alldealer == "" || alldealer.Contains("Distubutor") || alldealer == null)
                {
                    DealerId = null;
                }
                else
                {
                    DealerId = alldealer;
                }
            }
            if (ddlusers == "Retailer")
            {
                if (allretailer == "" || allretailer.Contains("All Retailer") || allretailer == null)
                {
                    retailerid = null;
                }
                else
                {
                    retailerid = allretailer;
                }
            }
            int pagesize = 20;
            var tbrow = _db.proc_HotelReport(pageindex, pagesize, ddl_status, retailerid, DealerId, userid, null, null, TXNID, null, null, frm_date, to_date).ToList();
            JsonModel jsonmodel = new JsonModel();
            jsonmodel.NoMoredata = tbrow.Count < pagesize;
            jsonmodel.HTMLString = renderPartialViewtostring("_HotelReport", tbrow);
            return Json(jsonmodel);
        }
        public ActionResult Total_HotelReport(string ddl_status, string txt_frm_date, string txt_to_date, string ddlusers, string alldealer, string allretailer, string TXNID)
        {
            if ((txt_frm_date == null || txt_frm_date == "") && (txt_to_date == null || txt_to_date == ""))
            {
                txt_frm_date = DateTime.Now.ToString();
                txt_to_date = DateTime.Now.ToString();
                ddlusers = "WMASTER";

            }
            var loginid = User.Identity.GetUserId();
            string retailerid = null;
            string DealerId = null;
            string userid = null;


            DateTime frm = Convert.ToDateTime(txt_frm_date);
            DateTime to = Convert.ToDateTime(txt_to_date);
            txt_frm_date = frm.ToString("dd-MM-yyyy");
            txt_to_date = to.ToString("dd-MM-yyyy");
            string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
            DateTime dt = DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            DateTime dt1 = DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            DateTime frm_date = dt.Date;
            DateTime to_date = dt1.AddDays(1);

            TXNID = string.IsNullOrWhiteSpace(TXNID) ? null : TXNID;
            ddl_status = string.IsNullOrWhiteSpace(ddl_status) ? null : ddl_status;
            if (ddlusers == "WMASTER")
            {
                userid = loginid;
            }
            if (ddlusers == "Dealer")
            {
                if (alldealer == "" || alldealer.Contains("Distubutor") || alldealer == null)
                {
                    DealerId = null;
                }
                else
                {
                    DealerId = alldealer;
                }
            }
            if (ddlusers == "Retailer")
            {
                if (allretailer == "" || allretailer.Contains("All Retailer") || allretailer == null)
                {
                    retailerid = null;
                }
                else
                {
                    retailerid = allretailer;
                }
            }

            var proc_Response = _db.proc_HotelReport(1, 20, ddl_status, retailerid, DealerId, userid, null, null, TXNID, null, null, frm_date, to_date).ToList();
            var successtotal1 = proc_Response.Where(s => s.TicketStatus.ToUpper().Contains("SUCCESS")).Sum(s => Convert.ToInt32(s.totalOfferedFare));
            var Failedtotal1 = proc_Response.Where(s => s.TicketStatus.ToUpper().Contains("FAILED")).Sum(s => Convert.ToInt32(s.totalOfferedFare));
            var PendingTotal1 = proc_Response.Where(s => s.TicketStatus.ToUpper().Contains("PROCCESSED")).Sum(s => Convert.ToInt32(s.totalOfferedFare));
            var data = new
            {
                success = successtotal1,
                failed = Failedtotal1,
                pending = PendingTotal1,

            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public virtual ActionResult ExcelHotalReport(string ddl_status, string txt_frm_date, string txt_to_date, string ddlusers, string alldealer, string allretailer, string TXNID)
        {

            var loginid = User.Identity.GetUserId();
            string retailerid = null;
            string DealerId = null;
            string userid = null;


            DateTime frm = Convert.ToDateTime(txt_frm_date);
            DateTime to = Convert.ToDateTime(txt_to_date);
            txt_frm_date = frm.ToString("dd-MM-yyyy");
            txt_to_date = to.ToString("dd-MM-yyyy");
            string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
            DateTime dt = DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            DateTime dt1 = DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            DateTime frm_date = dt.Date;
            DateTime to_date = dt1.AddDays(1);

            TXNID = string.IsNullOrWhiteSpace(TXNID) ? null : TXNID;
            ddl_status = string.IsNullOrWhiteSpace(ddl_status) ? null : ddl_status;
            if (ddlusers == "WMASTER")
            {
                userid = loginid;
            }
            if (ddlusers == "Dealer")
            {
                if (alldealer == "" || alldealer.Contains("Distubutor") || alldealer == null)
                {
                    DealerId = null;
                }
                else
                {
                    DealerId = alldealer;
                }
            }
            if (ddlusers == "Retailer")
            {
                if (allretailer == "" || allretailer.Contains("All Retailer") || allretailer == null)
                {
                    retailerid = null;
                }
                else
                {
                    retailerid = allretailer;
                }
            }

            DataTable dt2 = new DataTable();
            dt2.Columns.Add("Status", typeof(string));
            dt2.Columns.Add("Hotel", typeof(string));

            dt2.Columns.Add("Rooms", typeof(string));

            dt2.Columns.Add("Price", typeof(string));

            dt2.Columns.Add("Booking Id", typeof(string));
            dt2.Columns.Add("User Income", typeof(string));
            dt2.Columns.Add("MY Pr", typeof(string));
            dt2.Columns.Add("MY Income", typeof(string));
            dt2.Columns.Add("MY Post", typeof(string));
            dt2.Columns.Add("Distributort Income", typeof(string));
            dt2.Columns.Add("Booking Date", typeof(string));

            var respo = _db.proc_HotelReport(1, 100000, ddl_status, retailerid, DealerId, userid, null, null, TXNID, null, null, frm_date, to_date).ToList();
            foreach (var item in respo)
            {

                dt2.Rows.Add(item.TicketStatus + "" + item.Frm_Name, item.HotelName, item.NoOfRooms,
                    item.totalOfferedFare,
             item.BookingId, item.reminc, item.masterpre, item.masterinc, item.masterpost,
               item.dlminc, item.ticketdate);
            }

            var grid = new GridView();
            grid.DataSource = dt2;
            grid.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=Report.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            return View();
        }
        public ActionResult PDFHotelReport(string ddl_status, string txt_frm_date, string txt_to_date, string ddlusers, string alldealer, string allretailer, string TXNID)
        {

            var loginid = User.Identity.GetUserId();
            string retailerid = null;
            string DealerId = null;
            string userid = null;


            DateTime frm = Convert.ToDateTime(txt_frm_date);
            DateTime to = Convert.ToDateTime(txt_to_date);
            txt_frm_date = frm.ToString("dd-MM-yyyy");
            txt_to_date = to.ToString("dd-MM-yyyy");
            string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
            DateTime dt = DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            DateTime dt1 = DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            DateTime frm_date = dt.Date;
            DateTime to_date = dt1.AddDays(1);

            TXNID = string.IsNullOrWhiteSpace(TXNID) ? null : TXNID;
            ddl_status = string.IsNullOrWhiteSpace(ddl_status) ? null : ddl_status;
            if (ddlusers == "WMASTER")
            {
                userid = loginid;
            }
            if (ddlusers == "Dealer")
            {
                if (alldealer == "" || alldealer.Contains("Distubutor") || alldealer == null)
                {
                    DealerId = null;
                }
                else
                {
                    DealerId = alldealer;
                }
            }
            if (ddlusers == "Retailer")
            {
                if (allretailer == "" || allretailer.Contains("All Retailer") || allretailer == null)
                {
                    retailerid = null;
                }
                else
                {
                    retailerid = allretailer;
                }
            }

            var respo = _db.proc_HotelReport(1, 20, ddl_status, retailerid, DealerId, userid, null, null, TXNID, null, null, frm_date, to_date).ToList();

            return new ViewAsPdf(respo);
        }

        [HttpGet]
        public ActionResult CancellationQueue()
        {
            var userid = User.Identity.GetUserId();
            //show dealer
            ViewBag.alldealer = new SelectList(_db.select_Dealer_for_master_ddl(userid), "Dealerid", "DealerName", null).ToList();
            //show retailer
            ViewBag.allretailer = new SelectList(_db.select_retailer_for_master_ddl(userid), "RetailerId", "RetailerName", null).ToList();
            string txt_frm_date = DateTime.Now.ToString();
            string txt_to_date = DateTime.Now.ToString();
            var frm_date = Convert.ToDateTime(txt_frm_date).Date;
            var to_date = Convert.ToDateTime(txt_to_date).AddDays(1);
            var proc_Response = _db.proc_HotelCancellationReport(50, null, null, null, userid, null, null, null, null, null, frm_date, to_date).ToList();
            //ViewData["Totalofferfare"] = proc_Response.Where(s => s.TicketStatus.ToUpper().Contains("Success")).Sum(s => Convert.ToInt32(s.totalOfferedFare));
            //ViewData["totalf"] = proc_Response.Where(s => s.TicketStatus.ToUpper().Contains("FAILED")).Sum(s => Convert.ToInt32(s.totalOfferedFare));
            //ViewData["totalp"] = proc_Response.Where(s => s.TicketStatus.ToUpper().Contains("Proccessed")).Sum(s => Convert.ToInt32(s.totalOfferedFare));
            return View(proc_Response);
        }
        [HttpPost]
        public ActionResult CancellationQueue(string ddl_status, int? ddl_top, string txt_frm_date, string txt_to_date, string ddlusers, string alldealer, string allretailer, string BookingId)
        {

            var loginid = User.Identity.GetUserId();
            string retailerid = null;
            string DealerId = null;
            string userid = null;
            //show dealer
            ViewBag.alldealer = new SelectList(_db.select_Dealer_for_master_ddl(loginid), "Dealerid", "DealerName", null).ToList();
            //show retailer
            ViewBag.allretailer = new SelectList(_db.select_retailer_for_master_ddl(loginid), "RetailerId", "RetailerName", null).ToList();

            DateTime frm = Convert.ToDateTime(txt_frm_date);
            DateTime to = Convert.ToDateTime(txt_to_date);
            txt_frm_date = frm.ToString("dd-MM-yyyy");
            txt_to_date = to.ToString("dd-MM-yyyy");
            string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
            DateTime dt = DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            DateTime dt1 = DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            DateTime frm_date = dt.Date;
            DateTime to_date = dt1.AddDays(1);
            if (ddl_top == null)
            {
                ddl_top = 10000;
            }
            BookingId = string.IsNullOrWhiteSpace(BookingId) ? null : BookingId;
            ddl_status = string.IsNullOrWhiteSpace(ddl_status) ? null : ddl_status;
            if (ddlusers == "WMASTER")
            {
                userid = loginid;
            }
            if (ddlusers == "Dealer")
            {
                if (alldealer == "" || alldealer.Contains("Distubutor") || alldealer == null)
                {
                    DealerId = null;
                }
                else
                {
                    DealerId = alldealer;
                }
            }
            if (ddlusers == "Retailer")
            {
                if (allretailer == "" || allretailer.Contains("All Retailer") || allretailer == null)
                {
                    retailerid = null;
                }
                else
                {
                    retailerid = allretailer;
                }
            }

            var proc_Response = _db.proc_HotelCancellationReport(ddl_top, ddl_status, retailerid, DealerId, userid, null, null, BookingId, null, null, frm_date, to_date).ToList();
            //ViewData["Totalofferfare"] = proc_Response.Where(s => s.TicketStatus.ToUpper().Contains("Success")).Sum(s => Convert.ToInt32(s.totalOfferedFare));
            //ViewData["totalf"] = proc_Response.Where(s => s.TicketStatus.ToUpper().Contains("FAILED")).Sum(s => Convert.ToInt32(s.totalOfferedFare));
            //ViewData["totalp"] = proc_Response.Where(s => s.TicketStatus.ToUpper().Contains("Proccessed")).Sum(s => Convert.ToInt32(s.totalOfferedFare));
            return View(proc_Response);
        }
        [HttpGet]
        public ActionResult Aeps_report()
        {
            var userid = User.Identity.GetUserId();
            //show dealer
            ViewBag.alldealer = new SelectList(_db.select_Dealer_for_master_ddl(userid), "Dealerid", "DealerName", null).ToList();
            //show retailer
            ViewBag.allretailer = new SelectList(_db.select_retailer_for_master_ddl(userid), "RetailerId", "RetailerName", null).ToList();

            return View();
        }

        [HttpPost]
        public ActionResult Aeps_report(string txt_frm_date, string txt_to_date, string ddlusers, string alldealer, string allretailer, string ddl_top, string ddl_status)
        {
            var loginid = User.Identity.GetUserId();

            //var APIname = "";
            ViewBag.chk = "post";
            //show dealer
            ViewBag.alldealer = new SelectList(_db.select_Dealer_for_master_ddl(loginid), "Dealerid", "DealerName", null).ToList();
            //show retailer
            ViewBag.allretailer = new SelectList(_db.select_retailer_for_master_ddl(loginid), "RetailerId", "RetailerName", null).ToList();




            return View();
        }
        public ActionResult _Aepsreport(string txt_frm_date, string txt_to_date, string ddlusers, string alldealer, string allretailer, string ddl_status)
        {
            var loginid = User.Identity.GetUserId();

            var userid = "";

            if (txt_frm_date == null && txt_to_date == null && ddl_status == null && ddlusers == null)
            {
                txt_frm_date = DateTime.Now.ToString();
                txt_to_date = DateTime.Now.ToString();
                ddlusers = "WMASTER"; ddl_status = "ALL";


            }


            DateTime frm = Convert.ToDateTime(txt_frm_date);
            DateTime to = Convert.ToDateTime(txt_to_date);
            txt_frm_date = frm.ToString("dd-MM-yyyy");
            txt_to_date = to.ToString("dd-MM-yyyy");
            string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy",
                            "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
            DateTime dt = DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            DateTime dt1 = DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            string frm_date = Convert.ToDateTime(dt).ToShortDateString();
            string to_date = Convert.ToDateTime(dt1).AddDays(1).ToString();


            if (ddlusers == "Dealer")
            {
                if (alldealer == "" || alldealer.Contains("Distubutor") || alldealer == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = alldealer;
                }
            }
            if (ddlusers == "Retailer")
            {
                if (allretailer == "" || allretailer.Contains("All Retailer") || allretailer == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = allretailer;
                }
            }

            if (ddlusers == "WMASTER")
            {
                userid = loginid;
            }

            var ch = _db.Aeps_Report_New(ddlusers, userid, ddl_status, 0, "", "", "", "ALL", Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), 1, 20).ToList();

            return View(ch);
        }
        public ActionResult InfiniteScrollAeps(int pageindex, string txt_frm_date, string txt_to_date, string ddlusers, string alldealer, string allretailer, string ddl_status)
        {
            var loginid = User.Identity.GetUserId();

            var userid = "";

            if (txt_frm_date == null && txt_to_date == null && ddl_status == null && ddlusers == null)
            {
                txt_frm_date = DateTime.Now.ToString();
                txt_to_date = DateTime.Now.ToString();
                ddlusers = "WMASTER"; ddl_status = "ALL";


            }


            DateTime frm = Convert.ToDateTime(txt_frm_date);
            DateTime to = Convert.ToDateTime(txt_to_date);


            txt_frm_date = frm.ToString("dd-MM-yyyy");
            txt_to_date = to.ToString("dd-MM-yyyy");

            string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy",
                            "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
            DateTime dt = DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            DateTime dt1 = DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            string frm_date = Convert.ToDateTime(dt).ToShortDateString();
            string to_date = Convert.ToDateTime(dt1).AddDays(1).ToString();





            if (ddlusers == "Dealer")
            {
                if (alldealer == "" || alldealer.Contains("Distubutor") || alldealer == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = alldealer;
                }
            }
            if (ddlusers == "Retailer")
            {
                if (allretailer == "" || allretailer.Contains("All Retailer") || allretailer == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = allretailer;
                }
            }

            if (ddlusers == "WMASTER")
            {
                userid = loginid;
            }

            int pagesize = 20;
            var tbrow = _db.Aeps_Report_New(ddlusers, userid, ddl_status, 0, "", "", "", "ALL", Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), pageindex, pagesize).ToList();


            JsonModel jsonmodel = new JsonModel();
            jsonmodel.NoMoredata = tbrow.Count < pagesize;
            jsonmodel.HTMLString = renderPartialViewtostring("_Aepsreport", tbrow);
            return Json(jsonmodel);
        }
        public ActionResult TotalAepsreport(string txt_frm_date, string txt_to_date, string ddlusers, string alldealer, string allretailer, string ddl_status)
        {
            var loginid = User.Identity.GetUserId();

            var userid = "";

            if (txt_frm_date == null && txt_to_date == null && ddl_status == null && ddlusers == null)
            {
                txt_frm_date = DateTime.Now.ToString();
                txt_to_date = DateTime.Now.ToString();
                ddlusers = "WMASTER"; ddl_status = "ALL";


            }


            DateTime frm = Convert.ToDateTime(txt_frm_date);
            DateTime to = Convert.ToDateTime(txt_to_date);


            txt_frm_date = frm.ToString("dd-MM-yyyy");
            txt_to_date = to.ToString("dd-MM-yyyy");

            string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy",
                            "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
            DateTime dt = DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            DateTime dt1 = DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            string frm_date = Convert.ToDateTime(dt).ToShortDateString();
            string to_date = Convert.ToDateTime(dt1).AddDays(1).ToString();





            if (ddlusers == "Dealer")
            {
                if (alldealer == "" || alldealer.Contains("Distubutor") || alldealer == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = alldealer;
                }
            }
            if (ddlusers == "Retailer")
            {
                if (allretailer == "" || allretailer.Contains("All Retailer") || allretailer == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = allretailer;
                }
            }

            if (ddlusers == "WMASTER")
            {
                userid = loginid;
            }

            var ch = _db.Aeps_Report_New(ddlusers, userid, ddl_status, 0, "", "", "", "ALL", Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), 1, 1000000).ToList();

            var successtotal1 = ch.Where(a => a.Status.ToUpper() == "SUCCESS").Sum(a => a.Amount);
            var Failedtotal1 = ch.Where(s => s.Status.ToUpper().Contains("FAILED")).Sum(s => Convert.ToInt32(s.Amount));
            var PendingTotal1 = ch.Where(s => s.Status.ToUpper().Contains("PENDING")).Sum(s => Convert.ToInt32(s.Amount));

            var data = new
            {
                success = successtotal1,
                failed = Failedtotal1,
                pending = PendingTotal1,


            };

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ExcelAepsreport(string txt_frm_date, string txt_to_date, string ddlusers, string alldealer, string allretailer, string ddl_status)
        {
            var loginid = User.Identity.GetUserId();

            var userid = "";

            if (txt_frm_date == null && txt_to_date == null && ddl_status == null && ddlusers == null)
            {
                txt_frm_date = DateTime.Now.ToString();
                txt_to_date = DateTime.Now.ToString();
                ddlusers = "WMASTER"; ddl_status = "ALL";


            }


            DateTime frm = Convert.ToDateTime(txt_frm_date);
            DateTime to = Convert.ToDateTime(txt_to_date);


            txt_frm_date = frm.ToString("dd-MM-yyyy");
            txt_to_date = to.ToString("dd-MM-yyyy");

            string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy",
                            "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
            DateTime dt = DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            DateTime dt1 = DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            string frm_date = Convert.ToDateTime(dt).ToShortDateString();
            string to_date = Convert.ToDateTime(dt1).AddDays(1).ToString();





            if (ddlusers == "Dealer")
            {
                if (alldealer == "" || alldealer.Contains("Distubutor") || alldealer == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = alldealer;
                }
            }
            if (ddlusers == "Retailer")
            {
                if (allretailer == "" || allretailer.Contains("All Retailer") || allretailer == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = allretailer;
                }
            }

            if (ddlusers == "WMASTER")
            {
                userid = loginid;
            }


            DataTable dt2 = new DataTable();
            dt2.Columns.Add("Status", typeof(string));
            dt2.Columns.Add("Merchant ID", typeof(string));
            dt2.Columns.Add("Aadhaar Number", typeof(string));
            dt2.Columns.Add("Customer Bank Name", typeof(string));
            dt2.Columns.Add("T-Type", typeof(string));
            dt2.Columns.Add("Transfer Amount", typeof(string));
            dt2.Columns.Add("User Credit", typeof(string));
            dt2.Columns.Add("MY Post", typeof(string));
            dt2.Columns.Add("Bank UTR", typeof(string));
            dt2.Columns.Add("Req Date", typeof(string));
            dt2.Columns.Add("Retailer Pre", typeof(string));
            dt2.Columns.Add("Retailer Income", typeof(string));
            dt2.Columns.Add("Retailer Post", typeof(string));
            dt2.Columns.Add("WMASTER Pre", typeof(string));
            dt2.Columns.Add("WMASTER Income", typeof(string));
            dt2.Columns.Add("WMASTER Post", typeof(string));
            dt2.Columns.Add("Distributor Pre", typeof(string));
            dt2.Columns.Add("Distributor Income", typeof(string));
            dt2.Columns.Add("Distributor Post", typeof(string));
            dt2.Columns.Add("Remark", typeof(string));

            var respo = _db.Aeps_Report_New(ddlusers, userid, ddl_status, 0, "", "", "", "ALL", Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), 1, 1000000).ToList();
            foreach (var item in respo)
            {

                decimal amount = 0;
                decimal remincome = 0;
                decimal usercredit = 0;
                if (item.Role == "Retailer")
                {
                    amount = Convert.ToDecimal(item.Amount);
                    remincome = Convert.ToDecimal(item.RemIncome);
                    usercredit = (amount + remincome);
                }

                dt2.Rows.Add(item.Status + "=" + item.Frm_Name, item.MerchantTxnId, item.AccountHolderAadhaar, item.BankName, item.Type, item.Amount, usercredit, item.MdPost,
                 item.BankRRN, item.Txn_Date, item.RemPre, item.RemIncome, item.RemPost, item.MdPre, item.MdIncome, item.MdPost, item.DlmPre
                , item.DlmIncome, item.DlmPost, item.Remark);
            }

            var grid = new GridView();
            grid.DataSource = dt2;
            grid.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=Report.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            return View();
        }
        public ActionResult PDFAepsReport(string txt_frm_date, string txt_to_date, string ddlusers, string alldealer, string allretailer, string ddl_status)
        {

            var loginid = User.Identity.GetUserId();

            var userid = "";

            if (txt_frm_date == null && txt_to_date == null && ddl_status == null && ddlusers == null)
            {
                txt_frm_date = DateTime.Now.ToString();
                txt_to_date = DateTime.Now.ToString();
                ddlusers = "WMASTER"; ddl_status = "ALL";

            }


            DateTime frm = Convert.ToDateTime(txt_frm_date);
            DateTime to = Convert.ToDateTime(txt_to_date);


            txt_frm_date = frm.ToString("dd-MM-yyyy");
            txt_to_date = to.ToString("dd-MM-yyyy");

            string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy",
                            "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
            DateTime dt = DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            DateTime dt1 = DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            string frm_date = Convert.ToDateTime(dt).ToShortDateString();
            string to_date = Convert.ToDateTime(dt1).AddDays(1).ToString();





            if (ddlusers == "Dealer")
            {
                if (alldealer == "" || alldealer.Contains("Distubutor") || alldealer == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = alldealer;
                }
            }
            if (ddlusers == "Retailer")
            {
                if (allretailer == "" || allretailer.Contains("All Retailer") || allretailer == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = allretailer;
                }
            }

            if (ddlusers == "WMASTER")
            {
                userid = loginid;
            }

            var respo = _db.Aeps_Report_New(ddlusers, userid, ddl_status, 0, "", "", "", "ALL", Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), 1, 100000).ToList();

            return new ViewAsPdf(respo);
        }

        [HttpPost]
        public ActionResult AepsReport_View(int Idno)
        {
            var detail = _db.Aeps_Report_New_View(Idno);
            return Json(detail, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Pancard_Report()
        {
            var loginid = User.Identity.GetUserId();
            //show dealer
            ViewBag.alldealer = new SelectList(_db.select_Dealer_for_master_ddl(loginid), "Dealerid", "DealerName", null).ToList();
            //show retailer
            ViewBag.allretailer = new SelectList(_db.select_retailer_for_master_ddl(loginid), "RetailerId", "RetailerName", null).ToList();

            return View();
        }

        [HttpPost]
        public ActionResult Pancard_Report(string ddl_status, string ddl_top, string txt_frm_date, string txt_to_date, string allretailer, string alldealer)
        {
            var loginid = User.Identity.GetUserId();

            ViewBag.chk = "post";

            //show dealer
            ViewBag.alldealer = new SelectList(_db.select_Dealer_for_master_ddl(loginid), "Dealerid", "DealerName", null).ToList();
            //show retailer
            ViewBag.allretailer = new SelectList(_db.select_retailer_for_master_ddl(loginid), "RetailerId", "RetailerName", null).ToList();

            return View();
        }

        public ActionResult _Pancard_Report(string ddlusers, string ddl_status, string txt_frm_date, string txt_to_date, string allretailer, string alldealer)
        {

            var loginid = User.Identity.GetUserId();
            if (txt_frm_date == null && txt_to_date == null && ddlusers == null)
            {

                txt_frm_date = DateTime.Now.ToString();
                txt_to_date = DateTime.Now.ToString();
                ddlusers = "WMASTER";
            }

            var userid = User.Identity.GetUserId();

            if (ddlusers == "Dealer")
            {
                if (alldealer == "" || alldealer.Contains("Distubutor") || alldealer == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = alldealer;
                }
            }
            if (ddlusers == "Retailer")
            {
                if (allretailer == "" || allretailer.Contains("All Retailer") || allretailer == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = allretailer;
                }
            }


            if (ddlusers == "WMASTER")
            {
                userid = loginid;
            }



            if (ddl_status == null)
            {
                ddl_status = "ALL";

            }
            ViewBag.chk = "post";

            DateTime frm = Convert.ToDateTime(txt_frm_date);
            DateTime to = Convert.ToDateTime(txt_to_date);
            txt_frm_date = frm.ToString("dd-MM-yyyy");
            txt_to_date = to.ToString("dd-MM-yyyy");
            string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy",
                            "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
            DateTime dt = !string.IsNullOrWhiteSpace(txt_frm_date) ? DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
            DateTime dt1 = !string.IsNullOrWhiteSpace(txt_to_date) ? DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
            DateTime frm_date = Convert.ToDateTime(dt).Date;
            DateTime to_date = Convert.ToDateTime(dt1).Date.AddDays(1);


            int pagesize = 20;
            var ch = _db.PAN_CARD_IPAY_Token_report_paging(1, pagesize, userid, ddlusers, ddl_status, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();

            return View(ch);
        }

        public ActionResult InfiniteScrollpan(int pageindex, string ddlusers, string ddl_status, string txt_frm_date, string txt_to_date, string allretailer, string alldealer)
        {

            var loginid = User.Identity.GetUserId();
            var userid = "";
            if (ddlusers == "Dealer")
            {
                if (alldealer == "" || alldealer.Contains("Distubutor") || alldealer == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = alldealer;
                }
            }
            if (ddlusers == "Retailer")
            {
                if (allretailer == "" || allretailer.Contains("All Retailer") || allretailer == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = allretailer;
                }
            }


            if (ddlusers == "WMASTER")
            {
                userid = loginid;
            }



            if (ddl_status == null)
            {
                ddl_status = "ALL";

            }
            DateTime frm = Convert.ToDateTime(txt_frm_date);
            DateTime to = Convert.ToDateTime(txt_to_date);
            txt_frm_date = frm.ToString("dd-MM-yyyy");
            txt_to_date = to.ToString("dd-MM-yyyy");
            string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy",
                            "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
            DateTime dt = !string.IsNullOrWhiteSpace(txt_frm_date) ? DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
            DateTime dt1 = !string.IsNullOrWhiteSpace(txt_to_date) ? DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
            DateTime frm_date = Convert.ToDateTime(dt).Date;
            DateTime to_date = Convert.ToDateTime(dt1).Date.AddDays(1);

            int pagesize = 20;
            var tbrow = _db.PAN_CARD_IPAY_Token_report_paging(pageindex, pagesize, userid, ddlusers, ddl_status, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();

            JsonModel jsonmodel = new JsonModel();
            jsonmodel.NoMoredata = tbrow.Count < pagesize;
            jsonmodel.HTMLString = renderPartialViewtostring("_Pancard_Report", tbrow);
            return Json(jsonmodel);
        }
        public ActionResult PancardReport_Total(string ddlusers, string alldealer, string allretailer, string ddl_status, string txt_frm_date, string txt_to_date)
        {
            var loginid = User.Identity.GetUserId();
            var userid = "";
            if (txt_frm_date == null && txt_to_date == null && ddlusers == null)
            {

                txt_frm_date = DateTime.Now.ToString();
                txt_to_date = DateTime.Now.ToString();
                ddlusers = "Admin";
            }



            if (ddlusers == "Dealer")
            {
                if (alldealer == "" || alldealer.Contains("Distubutor") || alldealer == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = alldealer;
                }
            }
            if (ddlusers == "Retailer")
            {
                if (allretailer == "" || allretailer.Contains("All Retailer") || allretailer == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = allretailer;
                }
            }


            if (ddlusers == "WMASTER")
            {
                userid = loginid;
            }



            if (ddl_status == null)
            {
                ddl_status = "ALL";

            }


            DateTime frm = Convert.ToDateTime(txt_frm_date);
            DateTime to = Convert.ToDateTime(txt_to_date);
            txt_frm_date = frm.ToString("dd-MM-yyyy");
            txt_to_date = to.ToString("dd-MM-yyyy");
            string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy",
                            "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
            DateTime dt = !string.IsNullOrWhiteSpace(txt_frm_date) ? DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
            DateTime dt1 = !string.IsNullOrWhiteSpace(txt_to_date) ? DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
            DateTime frm_date = Convert.ToDateTime(dt).Date;
            DateTime to_date = Convert.ToDateTime(dt1).Date.AddDays(1);


            var ch = _db.PAN_CARD_IPAY_Token_report_paging(1, 100000, userid, ddlusers, ddl_status, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();

            var successtotal1 = ch.Where(s => s.Particulars.ToUpper().Contains("SUCCESS")).Sum(s => Convert.ToInt32(s.ProcessingFees));
            var Failedtotal1 = ch.Where(s => s.Particulars.ToUpper().Contains("FAILED")).Sum(s => Convert.ToInt32(s.ProcessingFees));
            var PendingTotal1 = ch.Where(s => s.Particulars.ToUpper().Contains("PENDING")).Sum(s => Convert.ToInt32(s.ProcessingFees));
            var data = new
            {
                success = successtotal1,
                failed = Failedtotal1,
                pending = PendingTotal1,

            };

            return Json(data, JsonRequestBehavior.AllowGet);




        }
        public virtual ActionResult ExcelRechargereportPan(string ddlusers, string alldealer, string allretailer, string ddl_status, string txt_frm_date, string txt_to_date)
        {
            var loginid = User.Identity.GetUserId();
            var userid = "";
            if (ddlusers == null || ddlusers == "")
            {

                ddlusers = "Admin";

            }



            if (ddlusers == "Dealer")
            {
                if (alldealer == "" || alldealer.Contains("Distubutor") || alldealer == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = alldealer;
                }
            }
            if (ddlusers == "Retailer")
            {
                if (allretailer == "" || allretailer.Contains("All Retailer") || allretailer == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = allretailer;
                }
            }


            if (ddlusers == "WMASTER")
            {
                userid = loginid;
            }



            if (ddl_status == null)
            {
                ddl_status = "ALL";

            }
            ViewBag.chk = "post";

            DateTime frm = Convert.ToDateTime(txt_frm_date);
            DateTime to = Convert.ToDateTime(txt_to_date);
            txt_frm_date = frm.ToString("dd-MM-yyyy");
            txt_to_date = to.ToString("dd-MM-yyyy");
            string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy",
                            "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
            DateTime dt = !string.IsNullOrWhiteSpace(txt_frm_date) ? DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
            DateTime dt1 = !string.IsNullOrWhiteSpace(txt_to_date) ? DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
            DateTime frm_date = Convert.ToDateTime(dt).Date;
            DateTime to_date = Convert.ToDateTime(dt1).Date.AddDays(1);


            DataTable dt2 = new DataTable();


            dt2.Columns.Add("User Information", typeof(string));
            dt2.Columns.Add("Physical", typeof(string));
            dt2.Columns.Add("Digital", typeof(string));
            dt2.Columns.Add("Processing Fees", typeof(string));
            dt2.Columns.Add("UTI TXN ID", typeof(string));
            dt2.Columns.Add("User Income", typeof(string));
            dt2.Columns.Add("User Post", typeof(string));
            dt2.Columns.Add("User Pre", typeof(string));
            dt2.Columns.Add("MY Pre", typeof(string));
            dt2.Columns.Add("MY Income", typeof(string));
            dt2.Columns.Add("MY Post", typeof(string));
            dt2.Columns.Add("Distributor Income", typeof(string));

            dt2.Columns.Add("Date", typeof(string));

            var respo = _db.PAN_CARD_IPAY_Token_report_paging(1, 10000000, userid, ddlusers, ddl_status, frm_date, to_date).ToList();
            foreach (var item in respo)
            {
                var sts = item.Particulars;

                if (item.Particulars.Contains("Refund"))
                {
                    sts = "Refund";
                }
                dt2.Rows.Add(sts + "" + item.UserType + "=" + item.Frm_Name, item.PhysicalCount, item.DigitalCount,
                    item.ProcessingFees, item.UTI_TXN_ID,
                item.RetailerIncome, item.retailer_remain_post, item.retailer_remain_pre, item.retailer_master_pre, item.MDIncome, item.retailer_master_post, item.DealerIncome,
               item.Date);
            }

            var grid = new GridView();
            grid.DataSource = dt2;
            grid.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=PANCARD_Report.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            return View();
        }
        public ActionResult PDFreport_PAN(string ddlusers, string alldealer, string allretailer, string ddl_status, string txt_frm_date, string txt_to_date)
        {
            var loginid = User.Identity.GetUserId();
            var userid = "";
            if (ddlusers == null || ddlusers == "")
            {

                ddlusers = "Admin";

            }



            if (ddlusers == "Dealer")
            {
                if (alldealer == "" || alldealer.Contains("Distubutor") || alldealer == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = alldealer;
                }
            }
            if (ddlusers == "Retailer")
            {
                if (allretailer == "" || allretailer.Contains("All Retailer") || allretailer == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = allretailer;
                }
            }


            if (ddlusers == "WMASTER")
            {
                userid = loginid;
            }



            if (ddl_status == null)
            {
                ddl_status = "ALL";

            }

            DateTime frm = Convert.ToDateTime(txt_frm_date);
            DateTime to = Convert.ToDateTime(txt_to_date);
            txt_frm_date = frm.ToString("dd-MM-yyyy");
            txt_to_date = to.ToString("dd-MM-yyyy");
            string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy",
                            "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
            DateTime dt = !string.IsNullOrWhiteSpace(txt_frm_date) ? DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
            DateTime dt1 = !string.IsNullOrWhiteSpace(txt_to_date) ? DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
            DateTime frm_date = Convert.ToDateTime(dt).Date;
            DateTime to_date = Convert.ToDateTime(dt1).Date.AddDays(1);
            var respo = _db.PAN_CARD_IPAY_Token_report_paging(1, 10000000, userid, ddlusers, ddl_status, frm_date, to_date).ToList();
            return new ViewAsPdf(respo);
        }


        public ActionResult MasterImpsIncmoe(string Type, string txt_frm_date, string txt_to_date)
        {
            DateTime frm = Convert.ToDateTime(txt_frm_date);
            DateTime to = Convert.ToDateTime(txt_to_date);

            txt_frm_date = frm.ToString("dd-MM-yyyy");
            txt_to_date = to.ToString("dd-MM-yyyy");

            string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy",
                            "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
            DateTime dt = DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            DateTime dt1 = DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            string frm_date = Convert.ToDateTime(dt).ToShortDateString();
            string to_date = Convert.ToDateTime(dt1).AddDays(1).ToString();

            var chk = _db.DMT_admin_income(Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            var Adminincome = chk.SingleOrDefault().total;
            var Tds = chk.SingleOrDefault().TDS;
            var Gst = chk.SingleOrDefault().GST;
            return Json(new { Type = Type, Adminincome = Adminincome, Tds = Tds, Gst = Gst });
        }

        public ActionResult CountImpasandVerify(string Type, string ddlval, string txt_frm_date, string txt_to_date)
        {
            var userid = User.Identity.GetUserId();

            if (ddlval == "All")
            {
                ddlval = "1000000";
            }


            DateTime frm = Convert.ToDateTime(txt_frm_date);
            DateTime to = Convert.ToDateTime(txt_to_date);

            txt_frm_date = frm.ToString("dd-MM-yyyy");
            txt_to_date = to.ToString("dd-MM-yyyy");

            string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy",
                            "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
            DateTime dt = DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            DateTime dt1 = DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            string frm_date = Convert.ToDateTime(dt).ToShortDateString();
            string to_date = Convert.ToDateTime(dt1).AddDays(1).ToString();
            var ch = _db.money_transfer_report("WMASTER", userid, Convert.ToInt32(ddlval), "ALL", "ALL", Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), "").ToList();
            var Impscount = ch.Where(s => s.status.ToUpper().Contains("SUCCESS") && s.Trans_type.ToUpper() == "IMPS").Count();
            var VerifyCount = ch.Where(s => s.status.ToUpper().Contains("SUCCESS") && s.Trans_type.ToUpper() == "IMPS_VERIFY").Count();
            return Json(new { Type = Type, Impscount = Impscount, VerifyCount = VerifyCount });
        }


        [HttpGet]
        public ActionResult m_Possreport()
        {
            var userid = User.Identity.GetUserId();
            //show dealer
            ViewBag.alldealer = new SelectList(_db.select_Dealer_for_master_ddl(userid), "Dealerid", "DealerName", null).ToList();
            //show retailer
            ViewBag.allretailer = new SelectList(_db.select_retailer_for_master_ddl(userid), "RetailerId", "RetailerName", null).ToList();

            return View();


        }
        [HttpPost]
        public ActionResult m_Possreport(string txt_frm_date, string txt_to_date, string ddlusers, string alldealer, string allretailer, string ddl_status)
        {

            ViewBag.chk = "post";
            var userid = User.Identity.GetUserId();
            //show dealer
            ViewBag.alldealer = new SelectList(_db.select_Dealer_for_master_ddl(userid), "Dealerid", "DealerName", null).ToList();
            //show retailer
            ViewBag.allretailer = new SelectList(_db.select_retailer_for_master_ddl(userid), "RetailerId", "RetailerName", null).ToList();


            return View();
        }
        public ActionResult _m_Possreport(string txt_frm_date, string txt_to_date, string ddlusers, string alldealer, string allretailer, string ddl_top, string ddl_status)
        {

            var loginid = User.Identity.GetUserId();

            var userid = "";

            if (txt_frm_date == null && txt_to_date == null && ddl_status == null && ddlusers == null)
            {
                txt_frm_date = DateTime.Now.ToString();
                txt_to_date = DateTime.Now.ToString();
                ddlusers = "WMASTER"; ddl_status = "ALL";

            }


            DateTime frm = Convert.ToDateTime(txt_frm_date);
            DateTime to = Convert.ToDateTime(txt_to_date);
            txt_frm_date = frm.ToString("dd-MM-yyyy");
            txt_to_date = to.ToString("dd-MM-yyyy");
            string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy",
                            "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
            DateTime dt = DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            DateTime dt1 = DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            string frm_date = Convert.ToDateTime(dt).ToShortDateString();
            string to_date = Convert.ToDateTime(dt1).AddDays(1).ToString();


            if (ddlusers == "Dealer")
            {
                if (alldealer == "" || alldealer.Contains("Distubutor") || alldealer == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = alldealer;
                }
            }
            if (ddlusers == "Retailer")
            {
                if (allretailer == "" || allretailer.Contains("All Retailer") || allretailer == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = allretailer;
                }
            }

            if (ddlusers == "WMASTER")
            {
                userid = loginid;
            }


            int pagesize = 20;

            var ch = _db.Mpos_Report_New_paging(1, pagesize, ddlusers, userid, ddl_status, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            return View(ch);

        }
        [HttpPost]
        public ActionResult InfiniteScroll_mpos(int pageindex, string txt_frm_date, string txt_to_date, string ddlusers, string alldealer, string allretailer, string ddl_top, string ddl_status)
        {


            var loginid = User.Identity.GetUserId();

            var userid = "";

            if (txt_frm_date == null && txt_to_date == null && ddl_status == null && ddlusers == null)
            {
                txt_frm_date = DateTime.Now.ToString();
                txt_to_date = DateTime.Now.ToString();
                ddlusers = "WMASTER"; ddl_status = "ALL";

            }


            DateTime frm = Convert.ToDateTime(txt_frm_date);
            DateTime to = Convert.ToDateTime(txt_to_date);
            txt_frm_date = frm.ToString("dd-MM-yyyy");
            txt_to_date = to.ToString("dd-MM-yyyy");
            string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy",
                            "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
            DateTime dt = DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            DateTime dt1 = DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            string frm_date = Convert.ToDateTime(dt).ToShortDateString();
            string to_date = Convert.ToDateTime(dt1).AddDays(1).ToString();


            if (ddlusers == "Dealer")
            {
                if (alldealer == "" || alldealer.Contains("Distubutor") || alldealer == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = alldealer;
                }
            }
            if (ddlusers == "Retailer")
            {
                if (allretailer == "" || allretailer.Contains("All Retailer") || allretailer == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = allretailer;
                }
            }

            if (ddlusers == "WMASTER")
            {
                userid = loginid;
            }

            int pagesize = 20;

            var tbrow = _db.Mpos_Report_New_paging(pageindex, pagesize, ddlusers, userid, ddl_status, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            JsonModel jsonmodel = new JsonModel();
            jsonmodel.NoMoredata = tbrow.Count < pagesize;
            jsonmodel.HTMLString = renderPartialViewtostring("_m_Possreport", tbrow);
            return Json(jsonmodel);
        }
        public ActionResult mpos_Total(string txt_frm_date, string txt_to_date, string ddlusers, string alldealer, string allretailer, string ddl_status)
        {

            if (txt_frm_date == null && txt_to_date == null && ddlusers == null || ddlusers == "")
            {

                txt_frm_date = DateTime.Now.ToString();
                txt_to_date = DateTime.Now.ToString();
                ddlusers = "Admin";

            }
            var loginid = User.Identity.GetUserId();
            var userid = "";
            DateTime frm = Convert.ToDateTime(txt_frm_date);
            DateTime to = Convert.ToDateTime(txt_to_date);
            txt_frm_date = frm.ToString("dd-MM-yyyy");
            txt_to_date = to.ToString("dd-MM-yyyy");
            string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy",
                            "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
            DateTime dt = !string.IsNullOrWhiteSpace(txt_frm_date) ? DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
            DateTime dt1 = !string.IsNullOrWhiteSpace(txt_to_date) ? DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
            DateTime frm_date = Convert.ToDateTime(dt).Date;
            DateTime to_date = Convert.ToDateTime(dt1).Date.AddDays(1);
            // show WMASTER id 


            if (ddl_status == null || ddl_status == "Status")
            {
                ddl_status = "ALL";
            }

            if (ddlusers == "Dealer")
            {
                if (alldealer == "" || alldealer.Contains("Distubutor") || alldealer == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = alldealer;
                }
            }
            if (ddlusers == "Retailer")
            {
                if (allretailer == "" || allretailer.Contains("All Retailer") || allretailer == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = allretailer;
                }
            }

            if (ddlusers == "WMASTER")
            {
                userid = loginid;
            }


            var ch = _db.Mpos_Report_New_paging(1, 100000, ddlusers, userid, ddl_status, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            var successtotal1 = ch.Where(s => s.status == "00").Sum(s => Convert.ToInt32(s.amount));
            var Failedtotal1 = ch.Where(s => s.status == "22").Sum(s => Convert.ToInt32(s.amount));
            var data = new
            {
                success = successtotal1,
                failed = Failedtotal1,

            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public virtual ActionResult ExcelRechargereport_mpos(string txt_frm_date, string txt_to_date, string ddlusers, string alldealer, string allretailer, string ddl_status)
        {
            var userid = "";
            var loginid = User.Identity.GetUserId();
            if (ddlusers == null || ddlusers == "")
            {

                ddlusers = "WMASTER";

            }

            if (ddlusers == "Dealer")
            {
                if (alldealer == "" || alldealer.Contains("Distubutor") || alldealer == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = alldealer;
                }
            }
            if (ddlusers == "Retailer")
            {
                if (allretailer == "" || allretailer.Contains("All Retailer") || allretailer == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = allretailer;
                }
            }

            if (ddlusers == "WMASTER")
            {
                userid = loginid;
            }


            if (ddl_status == null)
            {
                ddl_status = "ALL";

            }
            ViewBag.chk = "post";

            DateTime frm = Convert.ToDateTime(txt_frm_date);
            DateTime to = Convert.ToDateTime(txt_to_date);
            txt_frm_date = frm.ToString("dd-MM-yyyy");
            txt_to_date = to.ToString("dd-MM-yyyy");
            string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy",
                            "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
            DateTime dt = !string.IsNullOrWhiteSpace(txt_frm_date) ? DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
            DateTime dt1 = !string.IsNullOrWhiteSpace(txt_to_date) ? DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
            DateTime frm_date = Convert.ToDateTime(dt).Date;
            DateTime to_date = Convert.ToDateTime(dt1).Date.AddDays(1);

            DataTable dt2 = new DataTable();
            dt2.Columns.Add("Status", typeof(string));
            dt2.Columns.Add("MPoss Id", typeof(string));
            dt2.Columns.Add("BankRRN No", typeof(string));
            dt2.Columns.Add("TxnId", typeof(string));
            dt2.Columns.Add("TransType", typeof(string));
            dt2.Columns.Add("Amount", typeof(string));
            dt2.Columns.Add("User Comm", typeof(string));
            dt2.Columns.Add("MY Comm", typeof(string));
            dt2.Columns.Add("Distributor Comm", typeof(string));
            dt2.Columns.Add("Debit Amount", typeof(string));
            dt2.Columns.Add("Merchant ID", typeof(string));
            dt2.Columns.Add("Bank MerchantID", typeof(string));
            dt2.Columns.Add("Bank TerminalID", typeof(string));



            var respo = _db.Mpos_Report_New_paging(1, 100000, ddlusers, userid, ddl_status, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            foreach (var item in respo)
            {
                var sts = item.status;
                if (item.status == "00")
                {
                    sts = "Success";
                }
                else
                {

                    sts = "Failed";
                }
                dt2.Rows.Add(sts + "=" + item.Frm_Name, item.mPOSid, item.BankRRN, item.TxnId, item.transType, item.amount, item.Rem_comm, item.mdincome, item.dlmincome, item.totalamount,
                item.merchatID, item.bankmerchantID, item.bankterminalID);
            }

            var grid = new GridView();
            grid.DataSource = dt2;
            grid.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=Report.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            return View();
        }
        public ActionResult PDFMposReport(string txt_frm_date, string txt_to_date, string ddlusers, string alldealer, string allretailer, string ddl_status)
        {
            if (txt_frm_date == "")
            {
                return RedirectToAction("_m_Possreport", "Home");

            }
            if (txt_frm_date == null && txt_to_date == null && ddl_status == null)
            {
                txt_frm_date = DateTime.Now.ToString();
                txt_to_date = DateTime.Now.ToString();

            }
            var loginid = User.Identity.GetUserId();
            DateTime frm = Convert.ToDateTime(txt_frm_date);
            DateTime to = Convert.ToDateTime(txt_to_date);
            txt_frm_date = frm.ToString("dd-MM-yyyy");
            txt_to_date = to.ToString("dd-MM-yyyy");
            ViewBag.apiid = new SelectList(_db.select_apiusers_for_ddl("Admin"), "apiid", "username", null).ToList();
            string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy",
                            "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
            DateTime dt = DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            DateTime dt1 = DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            string frm_date = Convert.ToDateTime(dt).ToShortDateString();
            string to_date = Convert.ToDateTime(dt1).AddDays(1).ToString();


            var userid = "";

            if (ddlusers == "Dealer")
            {
                if (alldealer == "" || alldealer.Contains("alldealer") || alldealer == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = alldealer;
                }
            }
            if (ddlusers == "Retailer")
            {
                if (allretailer == "" || allretailer.Contains("allretailer") || allretailer == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = allretailer;
                }
            }


            if (ddlusers == "" || ddlusers == null)
            {
                ddlusers = "WMASTER";
            }

            if (ddl_status == "" || ddl_status == null)
            {
                ddl_status = "ALL";
            }
            if (ddlusers == "WMASTER")
            {
                userid = loginid;
            }


            System.Threading.Thread.Sleep(1000);

            var respo = _db.Mpos_Report_New_paging(1, 100000, ddlusers, userid, ddl_status, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            return new ViewAsPdf(respo);
        }



        #endregion
        //Login Deatils

        //Web Login Details
        public ActionResult WebLogin()
        {
            var userid = User.Identity.GetUserName();
            DateTime txt_frm_date = DateTime.Now.Date;
            DateTime txt_to_date = DateTime.Now;
            var Login_details = (_db.Login_info.Where(aa => aa.UserId == userid && aa.CurrentLoginTime >= txt_frm_date && aa.CurrentLoginTime <= txt_to_date).Take(100).OrderByDescending(aa => aa.Idno)).ToList();
            return View(Login_details);
        }
        [HttpPost]
        public ActionResult WebLogin(string txt_frm_date, string txt_to_date)
        {
            ViewBag.chk = "post";
            var userid = User.Identity.GetUserName();

            DateTime frm = Convert.ToDateTime(txt_frm_date);
            DateTime to = Convert.ToDateTime(txt_to_date);
            txt_frm_date = frm.ToString("dd-MM-yyyy");
            txt_to_date = to.ToString("dd-MM-yyyy");
            string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy",
                            "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
            DateTime dt = !string.IsNullOrWhiteSpace(txt_frm_date) ? DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
            DateTime dt1 = !string.IsNullOrWhiteSpace(txt_to_date) ? DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
            DateTime frm_date = Convert.ToDateTime(dt).Date;
            DateTime to_date = Convert.ToDateTime(dt1).Date.AddDays(1);

            var Login_details = (_db.Login_info.Where(aa => aa.UserId == userid && aa.CurrentLoginTime >= frm_date && aa.CurrentLoginTime <= to_date).OrderByDescending(aa => aa.Idno)).ToList();
            return View(Login_details);

        }

        //Login Failed
        [HttpGet]
        public ActionResult WebLoginFailed()
        {
            var userid = User.Identity.GetUserName();
            DateTime txt_frm_date = DateTime.Now.Date;
            DateTime txt_to_date = DateTime.Now;
            var Faild_Login_details = (_db.Failed_Login_info.Where(aa => aa.EmailId == userid && aa.LoginTime >= txt_frm_date && aa.LoginTime <= txt_to_date).Take(100).OrderByDescending(aa => aa.Idno)).ToList();
            return View(Faild_Login_details);
        }
        [HttpPost]
        public ActionResult WebLoginFailed(string txt_frm_date, string txt_to_date)
        {
            ViewBag.chk = "post";
            var userid = User.Identity.GetUserName();

            DateTime frm = Convert.ToDateTime(txt_frm_date);
            DateTime to = Convert.ToDateTime(txt_to_date);
            txt_frm_date = frm.ToString("dd-MM-yyyy");
            txt_to_date = to.ToString("dd-MM-yyyy");
            string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy",
                            "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
            DateTime dt = !string.IsNullOrWhiteSpace(txt_frm_date) ? DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
            DateTime dt1 = !string.IsNullOrWhiteSpace(txt_to_date) ? DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
            DateTime frm_date = Convert.ToDateTime(dt).Date;
            DateTime to_date = Convert.ToDateTime(dt1).Date.AddDays(1);

            var Faild_Login_details = (_db.Failed_Login_info.Where(aa => aa.EmailId == userid && aa.LoginTime >= frm_date && aa.LoginTime <= to_date).OrderByDescending(aa => aa.Idno)).ToList();

            return View(Faild_Login_details);

        }
        public ActionResult Bank_info()
        {
            var ch = (from jj in _db.bank_info select jj).ToList();
            return View(ch);
        }

        //Start GST Report 
        public ActionResult GST_Report()
        {
            string userid = User.Identity.GetUserId();
            DateTime fromdate = DateTime.Now.Date;
            DateTime todate = DateTime.Now.Date;
            System.Data.Entity.Core.Objects.ObjectParameter openingBal = new
             System.Data.Entity.Core.Objects.ObjectParameter("OpeningBal", typeof(string));
            System.Data.Entity.Core.Objects.ObjectParameter ClosingBal = new
            System.Data.Entity.Core.Objects.ObjectParameter("ClosingBal", typeof(string));
            System.Data.Entity.Core.Objects.ObjectParameter PurchaseAmt = new
            System.Data.Entity.Core.Objects.ObjectParameter("PurchaseAmt", typeof(string));
            System.Data.Entity.Core.Objects.ObjectParameter IsIGST = new
            System.Data.Entity.Core.Objects.ObjectParameter("IsIGST", typeof(string));
            System.Data.Entity.Core.Objects.ObjectParameter SellAmt = new
        System.Data.Entity.Core.Objects.ObjectParameter("SellAmt", typeof(string));
            var gst = _db.GST_Report_MD(fromdate, todate, userid, openingBal, ClosingBal, PurchaseAmt, SellAmt, IsIGST).ToList();

            ViewBag.OpnBal = !string.IsNullOrWhiteSpace(openingBal.Value.ToString()) ? openingBal.Value.ToString() : "0";
            ViewBag.CloseBal = !string.IsNullOrWhiteSpace(ClosingBal.Value.ToString()) ? ClosingBal.Value.ToString() : "0";
            ViewBag.Purchase = !string.IsNullOrWhiteSpace(PurchaseAmt.Value.ToString()) ? PurchaseAmt.Value.ToString() : "0";
            ViewBag.Sell = !string.IsNullOrWhiteSpace(SellAmt.Value.ToString()) ? SellAmt.Value.ToString() : "0";
            ViewBag.IsIGST = !string.IsNullOrWhiteSpace(IsIGST.Value.ToString()) ? IsIGST.Value.ToString() : "0";
            ViewBag.IsFormSubmmited = _db.GST_Declaration.Where(a => a.UserId.ToLower() == userid.ToLower()).SingleOrDefault();

            ViewBag.Months = Enumerable
             .Range(0, 2)
             .Select(i => DateTime.Now.AddMonths(i - 2))
             .Select(date => date.ToString("MMMM/yyyy")).Select(x => new SelectListItem()
             {
                 Text = x.ToString(),
                 Value = x.ToString()

             });
            return View(gst);
        }
        [HttpPost]
        public ActionResult GST_Report(string txt_frm_date, string txt_to_date, HttpPostedFileBase file)
        {
            string userid = User.Identity.GetUserId();

            ViewBag.fromdate = txt_frm_date;
            ViewBag.todate = txt_to_date;
            DateTime frm = Convert.ToDateTime(txt_frm_date);
            DateTime to = Convert.ToDateTime(txt_to_date);
            txt_frm_date = frm.ToString("dd-MM-yyyy");
            txt_to_date = to.ToString("dd-MM-yyyy");
            string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy",
                            "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
            DateTime dt = !string.IsNullOrWhiteSpace(txt_frm_date) ? DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
            DateTime dt1 = !string.IsNullOrWhiteSpace(txt_to_date) ? DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
            DateTime frm_date = Convert.ToDateTime(dt).Date;
            DateTime to_date = Convert.ToDateTime(dt1).Date.AddDays(1);

            System.Data.Entity.Core.Objects.ObjectParameter openingBal = new
             System.Data.Entity.Core.Objects.ObjectParameter("OpeningBal", typeof(string));
            System.Data.Entity.Core.Objects.ObjectParameter ClosingBal = new
            System.Data.Entity.Core.Objects.ObjectParameter("ClosingBal", typeof(string));
            System.Data.Entity.Core.Objects.ObjectParameter PurchaseAmt = new
            System.Data.Entity.Core.Objects.ObjectParameter("PurchaseAmt", typeof(string));
            System.Data.Entity.Core.Objects.ObjectParameter IsIGST = new
            System.Data.Entity.Core.Objects.ObjectParameter("IsIGST", typeof(string));
            System.Data.Entity.Core.Objects.ObjectParameter SellAmt = new
        System.Data.Entity.Core.Objects.ObjectParameter("SellAmt", typeof(string));
            var gst = _db.GST_Report_MD(frm_date, to_date, userid, openingBal, ClosingBal, PurchaseAmt, SellAmt, IsIGST).ToList();
            ViewBag.OpnBal = !string.IsNullOrWhiteSpace(openingBal.Value.ToString()) ? openingBal.Value.ToString() : "0";
            ViewBag.CloseBal = !string.IsNullOrWhiteSpace(ClosingBal.Value.ToString()) ? ClosingBal.Value.ToString() : "0";
            ViewBag.Purchase = !string.IsNullOrWhiteSpace(PurchaseAmt.Value.ToString()) ? PurchaseAmt.Value.ToString() : "0";
            ViewBag.Sell = !string.IsNullOrWhiteSpace(SellAmt.Value.ToString()) ? SellAmt.Value.ToString() : "0";

            ViewBag.IsIGST = !string.IsNullOrWhiteSpace(IsIGST.Value.ToString()) ? IsIGST.Value.ToString() : "0";
            ViewBag.Months = Enumerable
            .Range(0, 2)
            .Select(i => DateTime.Now.AddMonths(i - 2))
            .Select(date => date.ToString("MMMM/yyyy")).Select(x => new SelectListItem()
            {
                Text = x.ToString(),
                Value = x.ToString()

            });
            if (file != null)
            {
                try
                {

                    if (file.ContentLength > 0)
                    {
                        string _FileName = Guid.NewGuid().ToString() + '_' + Path.GetFileName(file.FileName);
                        string _path = Path.Combine(Server.MapPath("~/GST_Declaration"), _FileName);
                        file.SaveAs(_path);
                        GST_Declaration obj = new GST_Declaration();
                        obj.UserId = userid;
                        obj.FilePath = _FileName;
                        obj.Status = "Pending";
                        obj.Date = DateTime.Now;
                        _db.GST_Declaration.Add(obj);
                        _db.SaveChanges();
                    }
                    ViewBag.Message = "Success";
                    return View("GST_Report", gst);
                }
                catch
                {
                    ViewBag.Message = "Faild";
                    return View("GST_Report", gst);
                }
            }

            ViewBag.IsFormSubmmited = _db.GST_Declaration.Where(a => a.UserId.ToLower() == userid.ToLower()).SingleOrDefault();

            return View(gst);
        }
        //Download GST declaration form
        [HttpGet]
        public FileResult Download_Declaration_form()
        {
            string[] filesInDirectory = Directory.GetFiles(Server.MapPath("~/GST_Declaration"), "*.docx");

            return File(filesInDirectory[0], "application/force-download", Path.GetFileName(filesInDirectory[0]));
        }
        //GST Invocing 
        [HttpPost]
        public ActionResult GST_Invoicing(DateTime month)
        {

            string UserId = User.Identity.GetUserId();
            string from = month.ToString("yyyy-MM-dd");
            string to = month.AddMonths(1).ToString("yyyy-MM-dd");
            //string x = DateTime.Now.ToString("yyyy-MM-dd");
            return new Rotativa.ActionAsPdf("GST_INVOICE_PDF", new { userid = UserId, txt_frm_date = from, txt_to_date = to, month = month });
        }
        public ActionResult GST_INVOICE_PDF(string userid, DateTime txt_frm_date, DateTime txt_to_date, DateTime month)
        {
            string CurrentMonth = String.Format("{0:MMMM-yyyy}", month);
            string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy",
             "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
            ViewData["month"] = txt_to_date.AddDays(-1).ToString("dd/MM/yy");
            string MonthName = txt_frm_date.ToString("MMMM");
            string Year = txt_frm_date.ToString("yyyy");
            System.Data.Entity.Core.Objects.ObjectParameter InvoiceNo = new
       System.Data.Entity.Core.Objects.ObjectParameter("InvoiceNo", typeof(string));
            var invc = _db.generate_GST_Invoice(userid, MonthName, Year, InvoiceNo).SingleOrDefault().Response.ToString();
            DateTime frm_date = txt_frm_date.Date;
            DateTime to_date = txt_to_date.Date;
            Vastwebmulti.Areas.WRetailer.Models.GST_PDF_MODEL model = new Vastwebmulti.Areas.WRetailer.Models.GST_PDF_MODEL();
            var role = _db.showrole(userid).SingleOrDefault();
            System.Data.Entity.Core.Objects.ObjectParameter openingBal = new
             System.Data.Entity.Core.Objects.ObjectParameter("OpeningBal", typeof(string));
            System.Data.Entity.Core.Objects.ObjectParameter ClosingBal = new
            System.Data.Entity.Core.Objects.ObjectParameter("ClosingBal", typeof(string));
            System.Data.Entity.Core.Objects.ObjectParameter PurchaseAmt = new
            System.Data.Entity.Core.Objects.ObjectParameter("PurchaseAmt", typeof(string));
            System.Data.Entity.Core.Objects.ObjectParameter IsIGST = new
            System.Data.Entity.Core.Objects.ObjectParameter("IsIGST", typeof(string));
            System.Data.Entity.Core.Objects.ObjectParameter SellAmt = new
        System.Data.Entity.Core.Objects.ObjectParameter("SellAmt", typeof(string));
            if (role.Name.Contains("WMASTER"))
            {
                model.MdGst = _db.GST_Report_MD(frm_date, to_date, userid, openingBal, ClosingBal, PurchaseAmt, SellAmt, IsIGST).
                         Where(a => a.operator_Name != "Dish TV" && !a.Operator_type.Contains("PostPaid") && a.Operator_type != "Landline" && a.Operator_type != "Electricity" && a.Operator_type != "Gas" && a.Operator_type != "Insurance" && a.Operator_type != "Money" && a.Operator_type != "DTH-Booking").ToList();
                //Convert function call
                var converword = new Vastwebmulti.Areas.WRetailer.Models.Convertword().changeToWords(model.DealerGst.Sum(a => a.NetAmount).ToString());
                ViewData["total"] = converword;
                var UserDetails = _db.Superstokist_details.Where(a => a.SSId.ToLower() == userid.ToLower()).SingleOrDefault();
                ViewBag.UserDetails = UserDetails;
            }

            ViewBag.OpnBal = !string.IsNullOrWhiteSpace(openingBal.Value.ToString()) ? openingBal.Value.ToString() : "0";
            ViewBag.CloseBal = !string.IsNullOrWhiteSpace(ClosingBal.Value.ToString()) ? ClosingBal.Value.ToString() : "0";
            ViewBag.Purchase = !string.IsNullOrWhiteSpace(PurchaseAmt.Value.ToString()) ? PurchaseAmt.Value.ToString() : "0";
            ViewBag.IsIGST = !string.IsNullOrWhiteSpace(IsIGST.Value.ToString()) ? IsIGST.Value.ToString() : "0";
            var AdminDetails = _db.Admin_details.SingleOrDefault();
            ViewBag.AdminDetails = AdminDetails;
            ViewBag.Role = role;


            model.INVOICENO = invc;
            //ViewBag.State = _db.State_Desc.Where(y => y.State_id == AdminDetails.State).Single().State_name;
            ViewBag.city = _db.District_Desc.Where(f => f.Dist_id == AdminDetails.District && f.State_id == AdminDetails.State).Single().Dist_Desc;

            return View(model);
        }

        //GST PDF Invoicing 
        public ActionResult GenerateGST_PDF(DateTime frm_date, DateTime to_date)
        {
            return new Rotativa.ActionAsPdf("GST_PDF", new { txt_frm_date = frm_date, txt_to_date = to_date });
        }
        public ActionResult GST_PDF(string txt_frm_date, string txt_to_date)
        {

            string userid = User.Identity.GetUserId();

            DateTime frm = Convert.ToDateTime(txt_frm_date);
            DateTime to = Convert.ToDateTime(txt_to_date);

            txt_frm_date = frm.ToString("dd-MM-yyyy");
            txt_to_date = to.ToString("dd-MM-yyyy");
            string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy",
                            "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
            DateTime dt = !string.IsNullOrWhiteSpace(txt_frm_date) ? DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
            DateTime dt1 = !string.IsNullOrWhiteSpace(txt_to_date) ? DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
            DateTime frm_date = Convert.ToDateTime(dt).Date;
            DateTime to_date = Convert.ToDateTime(dt1).Date.AddDays(1);
            System.Data.Entity.Core.Objects.ObjectParameter openingBal = new
                  System.Data.Entity.Core.Objects.ObjectParameter("OpeningBal", typeof(string));
            System.Data.Entity.Core.Objects.ObjectParameter ClosingBal = new
            System.Data.Entity.Core.Objects.ObjectParameter("ClosingBal", typeof(string));
            System.Data.Entity.Core.Objects.ObjectParameter PurchaseAmt = new
            System.Data.Entity.Core.Objects.ObjectParameter("PurchaseAmt", typeof(string));
            System.Data.Entity.Core.Objects.ObjectParameter IsIGST = new
            System.Data.Entity.Core.Objects.ObjectParameter("IsIGST", typeof(string));
            System.Data.Entity.Core.Objects.ObjectParameter SellAmt = new
        System.Data.Entity.Core.Objects.ObjectParameter("SellAmt", typeof(string));
            var gst = _db.GST_Report_MD(frm_date, to_date, userid, openingBal, ClosingBal, PurchaseAmt, SellAmt, IsIGST).ToList();
            // var gst = _db.GST_Report_Retailer(fromdate, todate, userid, openingBal, ClosingBal, PurchaseAmt, IsIGST).ToList();

            ViewBag.OpnBal = !string.IsNullOrWhiteSpace(openingBal.Value.ToString()) ? openingBal.Value.ToString() : "0";
            ViewBag.CloseBal = !string.IsNullOrWhiteSpace(ClosingBal.Value.ToString()) ? ClosingBal.Value.ToString() : "0";
            ViewBag.Purchase = !string.IsNullOrWhiteSpace(PurchaseAmt.Value.ToString()) ? PurchaseAmt.Value.ToString() : "0";
            ViewBag.IsIGST = !string.IsNullOrWhiteSpace(IsIGST.Value.ToString()) ? IsIGST.Value.ToString() : "0";
            var UserDetails = _db.Superstokist_details.Where(a => a.SSId.ToLower() == userid.ToLower()).SingleOrDefault();
            ViewBag.UserDetails = UserDetails;
            var AdminDetails = _db.Admin_details.SingleOrDefault();
            ViewBag.AdminDetails = AdminDetails;
            return View(gst);
        }
        #region DTH BooKing
        public ActionResult DthBookingReport()
        {
            var userid = User.Identity.GetUserId();
            string txt_frm_date = DateTime.Now.ToString();
            string txt_to_date = DateTime.Now.ToString();
            string frm_date = Convert.ToDateTime(txt_frm_date).Date.ToString("yyyy-MM-dd");
            string to_date = Convert.ToDateTime(txt_to_date).AddDays(1).ToString("yyyy-MM-dd");
            var ch = _db.DTHConnection_Report_New("WMASTER", userid, "ALL", 50, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            ViewData["totals"] = ch.Where(s => s.Status == "Success").Sum(s => Convert.ToInt32(s.Amount));
            ViewData["Totalf"] = ch.Where(s => s.Status == "Failed").Sum(s => Convert.ToInt32(s.Amount));
            return View(ch);
        }
        [HttpPost]
        public ActionResult DthBookingReport(string ddl_top, string ddl_status, string txt_frm_date, string txt_to_date)
        {
            var userid = User.Identity.GetUserId();
            DateTime frm = Convert.ToDateTime(txt_frm_date);
            DateTime to = Convert.ToDateTime(txt_to_date);
            txt_frm_date = frm.ToString("dd-MM-yyyy");
            txt_to_date = to.ToString("dd-MM-yyyy");
            string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy",
                            "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
            DateTime dt = !string.IsNullOrWhiteSpace(txt_frm_date) ? DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
            DateTime dt1 = !string.IsNullOrWhiteSpace(txt_to_date) ? DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
            DateTime frm_date = Convert.ToDateTime(dt).Date;
            DateTime to_date = Convert.ToDateTime(dt1).Date.AddDays(1);

            if (ddl_top == "All")
            {
                ddl_top = "1000000";
            }
            if (ddl_status == "Status")
            {
                ddl_status = "ALL";
            }
            int ddltop = Convert.ToInt32(ddl_top);
            var ch = _db.DTHConnection_Report_New("WMASTER", userid, ddl_status, ddltop, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            ViewData["totals"] = ch.Where(s => s.Status == "Success").Sum(s => Convert.ToInt32(s.Amount));
            ViewData["Totalf"] = ch.Where(s => s.Status == "Failed").Sum(s => Convert.ToInt32(s.Amount));
            return View(ch);
        }

        #endregion


        public ActionResult All_Operator_Info()
        {
            var userid = User.Identity.GetUserId();
            Operater_Commission sb = new Operater_Commission();
            sb.Prepaid = (from cust in _db.Whitelabel_prepaid_master_comm
                          join ord in _db.Operator_Code
                          on cust.optcode equals ord.Operator_id.ToString()
                          where (cust.userid == userid)
                          select new Prepaid_Comm
                          {
                              OperatorCode = ord.new_opt_code,
                              Commission = cust.comm,
                              BlockTime = ord.blocktime,
                              Status = ord.status,
                              OperatorType = ord.Operator_type,
                              OperatorName = ord.operator_Name
                          }).ToList();
            sb.Electricity_comm = (from cust in _db.Whitelabel_utility_master_comm
                                   join ord in _db.Operator_Code
                                   on cust.optcode equals ord.Operator_id.ToString()
                                   where (cust.userid == userid)
                                   select new Electricity
                                   {
                                       OperatorCode = ord.new_opt_code,
                                       Commission = cust.comm,
                                       BlockTime = ord.blocktime,
                                       Status = ord.status,
                                       OperatorType = ord.Operator_type,
                                       OperatorName = ord.operator_Name,
                                       Gst = cust.gst
                                   }).ToList();
            sb.Money = (from cust in _db.Whitelabel_paytm_imps_master_comm
                        where (cust.userid == userid)
                        select new money_comm
                        {
                            Verifycomm = cust.verify_comm,
                            comm_1000 = cust.comm_1000,
                            comm_2000 = cust.comm_2000,
                            comm_3000 = cust.comm_3000,
                            comm_4000 = cust.comm_4000,
                            comm_5000 = cust.comm_5000,
                            gst = cust.gst
                        }).ToList();
            sb.Pencard_comm = (from cust in _db.Whitelabelpancard_master_Common_comm
                               where (cust.userid == userid)
                               select new Pencard
                               {
                                   TokenValueDigital = cust.tokenvalueDigital,
                                   TokenValuePhysical = cust.tokenvaluePhysical,
                                   Gst = cust.gst,
                                   CommissionDigital = cust.commDigital,
                                   CommissionPhysical = cust.commPhysical
                               }).ToList();
            sb.AEPS = (from cust in _db.Whitelable_Aeps_comm_userwise
                       where cust.Userid == userid
                       select new AEPS_Comm
                       {
                           aadharpay = cust.aadharpay,
                           ministatement = cust.ministatement,
                           per_500_999 = cust.per_500_999,
                           rs_500_999 = cust.rs_500_999,
                           maxrs_500_999 = cust.maxrs_500_999,
                           Type_500_999 = cust.Type_500_999,
                           per_1000_1499 = cust.per_1000_1499,
                           rs_1000_1499 = cust.rs_1000_1499,
                           maxrs_1000_1499 = cust.maxrs_1000_1499,
                           Type_1000_1499 = cust.Type_1000_1499,
                           per_1500_1999 = cust.per_1500_1999,
                           rs_1500_1999 = cust.rs_1500_1999,
                           maxrs_1500_1999 = cust.maxrs_1500_1999,
                           Type_1500_1999 = cust.Type_1500_1999,
                           per_2000_2499 = cust.per_2000_2499,
                           rs_2000_2499 = cust.rs_2000_2499,
                           maxrs_2000_2499 = cust.maxrs_2000_2499,
                           Type_2000_2499 = cust.Type_2000_2499,
                           per_2500_2999 = cust.per_2500_2999,
                           rs_2500_2999 = cust.rs_2500_2999,
                           maxrs_2500_2999 = cust.maxrs_2500_2999,
                           Type_2500_2999 = cust.Type_2500_2999,
                           per_3000_3499 = cust.per_3000_3499,
                           rs_3000_3499 = cust.rs_3000_3499,
                           maxrs_3000_3499 = cust.maxrs_3000_3499,
                           Type_3000_3499 = cust.Type_3000_3499,
                           per_3500_5000 = cust.per_3500_5000,
                           rs_3500_5000 = cust.rs_3500_5000,
                           maxrs_3500_5000 = cust.maxrs_3500_5000,
                           Type_3500_5000 = cust.Type_3500_5000,
                           per_5001_10000 = cust.per_5001_10000,
                           rs_5001_10000 = cust.rs_5001_10000,
                           maxrs_5001_10000 = cust.maxrs_5001_10000,
                           Type_5001_10000 = cust.Type_5001_10000
                       }).ToList();
            sb.Water = (from cust in _db.Water_whitelabel_comm
                        join ord in _db.Operator_Code
                        on cust.optcode equals ord.Operator_id.ToString()
                        where (cust.userid == userid)
                        select new Water_Comm
                        {
                            OperatorCode = ord.new_opt_code,
                            Commission = cust.comm,
                            BlockTime = ord.blocktime,
                            Status = ord.status,
                            OperatorType = ord.Operator_type,
                            OperatorName = ord.operator_Name
                        }).ToList();

            sb.Insurance = (from cust in _db.Insurance_whitelabel_comm
                            join ord in _db.Operator_Code
                            on cust.optcode equals ord.Operator_id.ToString()
                            where (cust.userid == userid)
                            select new Insurance_Comm
                            {
                                OperatorCode = ord.new_opt_code,
                                Commission = cust.comm,
                                BlockTime = ord.blocktime,
                                Status = ord.status,
                                OperatorType = ord.Operator_type,
                                OperatorName = ord.operator_Name,
                                Gst = cust.gst
                            }).ToList();

            sb.Broadband = (from cust in _db.Broandband_whitelabel_comm
                            join ord in _db.Operator_Code
                            on cust.optcode equals ord.Operator_id.ToString()
                            where (cust.userid == userid)
                            select new Broadband_Comm
                            {
                                OperatorCode = ord.new_opt_code,
                                Commission = cust.comm,
                                BlockTime = ord.blocktime,
                                Status = ord.status,
                                OperatorType = ord.Operator_type,
                                OperatorName = ord.operator_Name
                            }).ToList();
            return View(sb);
        }

        [HttpPost]
        public ActionResult All_Operator_Info(string ddlcomm)
        {
            var userid = User.Identity.GetUserId();
            Operater_Commission sb = new Operater_Commission();
            sb.Prepaid = (from cust in _db.Whitelabel_prepaid_master_comm
                          join ord in _db.Operator_Code
                          on cust.optcode equals ord.Operator_id.ToString()
                          where (cust.userid == userid)
                          select new Prepaid_Comm
                          {
                              OperatorCode = ord.new_opt_code,
                              Commission = cust.comm,
                              BlockTime = ord.blocktime,
                              Status = ord.status,
                              OperatorType = ord.Operator_type,
                              OperatorName = ord.operator_Name
                          }).ToList();
            sb.Electricity_comm = (from cust in _db.Whitelabel_utility_master_comm
                                   join ord in _db.Operator_Code
                                   on cust.optcode equals ord.Operator_id.ToString()
                                   where (cust.userid == userid)
                                   select new Electricity
                                   {
                                       OperatorCode = ord.new_opt_code,
                                       Commission = cust.comm,
                                       BlockTime = ord.blocktime,
                                       Status = ord.status,
                                       OperatorType = ord.Operator_type,
                                       OperatorName = ord.operator_Name,
                                       Gst = cust.gst
                                   }).ToList();
            sb.Money = (from cust in _db.Whitelabel_paytm_imps_master_comm
                        where (cust.userid == userid)
                        select new money_comm
                        {
                            Verifycomm = cust.verify_comm,
                            comm_1000 = cust.comm_1000,
                            comm_2000 = cust.comm_2000,
                            comm_3000 = cust.comm_3000,
                            comm_4000 = cust.comm_4000,
                            comm_5000 = cust.comm_5000,
                            gst = cust.gst
                        }).ToList();
            sb.Pencard_comm = (from cust in _db.Whitelabelpancard_master_Common_comm
                               where (cust.userid == userid)
                               select new Pencard
                               {
                                   TokenValueDigital = cust.tokenvalueDigital,
                                   TokenValuePhysical = cust.tokenvaluePhysical,
                                   Gst = cust.gst,
                                   CommissionDigital = cust.commDigital,
                                   CommissionPhysical = cust.commPhysical
                               }).ToList();
            sb.AEPS = (from cust in _db.Whitelable_Aeps_comm_userwise
                       where cust.Userid == userid
                       select new AEPS_Comm
                       {
                           aadharpay = cust.aadharpay,
                           ministatement = cust.ministatement,
                           per_500_999 = cust.per_500_999,
                           rs_500_999 = cust.rs_500_999,
                           maxrs_500_999 = cust.maxrs_500_999,
                           Type_500_999 = cust.Type_500_999,
                           per_1000_1499 = cust.per_1000_1499,
                           rs_1000_1499 = cust.rs_1000_1499,
                           maxrs_1000_1499 = cust.maxrs_1000_1499,
                           Type_1000_1499 = cust.Type_1000_1499,
                           per_1500_1999 = cust.per_1500_1999,
                           rs_1500_1999 = cust.rs_1500_1999,
                           maxrs_1500_1999 = cust.maxrs_1500_1999,
                           Type_1500_1999 = cust.Type_1500_1999,
                           per_2000_2499 = cust.per_2000_2499,
                           rs_2000_2499 = cust.rs_2000_2499,
                           maxrs_2000_2499 = cust.maxrs_2000_2499,
                           Type_2000_2499 = cust.Type_2000_2499,
                           per_2500_2999 = cust.per_2500_2999,
                           rs_2500_2999 = cust.rs_2500_2999,
                           maxrs_2500_2999 = cust.maxrs_2500_2999,
                           Type_2500_2999 = cust.Type_2500_2999,
                           per_3000_3499 = cust.per_3000_3499,
                           rs_3000_3499 = cust.rs_3000_3499,
                           maxrs_3000_3499 = cust.maxrs_3000_3499,
                           Type_3000_3499 = cust.Type_3000_3499,
                           per_3500_5000 = cust.per_3500_5000,
                           rs_3500_5000 = cust.rs_3500_5000,
                           maxrs_3500_5000 = cust.maxrs_3500_5000,
                           Type_3500_5000 = cust.Type_3500_5000,
                           per_5001_10000 = cust.per_5001_10000,
                           rs_5001_10000 = cust.rs_5001_10000,
                           maxrs_5001_10000 = cust.maxrs_5001_10000,
                           Type_5001_10000 = cust.Type_5001_10000
                       }).ToList();
            sb.Water = (from cust in _db.Water_whitelabel_comm
                        join ord in _db.Operator_Code
                        on cust.optcode equals ord.Operator_id.ToString()
                        where (cust.userid == userid)
                        select new Water_Comm
                        {
                            OperatorCode = ord.new_opt_code,
                            Commission = cust.comm,
                            BlockTime = ord.blocktime,
                            Status = ord.status,
                            OperatorType = ord.Operator_type,
                            OperatorName = ord.operator_Name
                        }).ToList();

            sb.Insurance = (from cust in _db.Insurance_whitelabel_comm
                            join ord in _db.Operator_Code
                            on cust.optcode equals ord.Operator_id.ToString()
                            where (cust.userid == userid)
                            select new Insurance_Comm
                            {
                                OperatorCode = ord.new_opt_code,
                                Commission = cust.comm,
                                BlockTime = ord.blocktime,
                                Status = ord.status,
                                OperatorType = ord.Operator_type,
                                OperatorName = ord.operator_Name,
                                Gst = cust.gst
                            }).ToList();

            sb.Broadband = (from cust in _db.Broandband_whitelabel_comm
                            join ord in _db.Operator_Code
                            on cust.optcode equals ord.Operator_id.ToString()
                            where (cust.userid == userid)
                            select new Broadband_Comm
                            {
                                OperatorCode = ord.new_opt_code,
                                Commission = cust.comm,
                                BlockTime = ord.blocktime,
                                Status = ord.status,
                                OperatorType = ord.Operator_type,
                                OperatorName = ord.operator_Name
                            }).ToList();
            ViewData["type"] = ddlcomm;
            return View(sb);
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
        public static string Decrypt(string encryptedText)
        {
            byte[] cipherTextBytes = Convert.FromBase64String(encryptedText);
            byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.None };

            var decryptor = symmetricKey.CreateDecryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));
            var memoryStream = new MemoryStream(cipherTextBytes);
            var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];

            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount).TrimEnd("\0".ToCharArray());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Helper
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
        #endregion
    }
}