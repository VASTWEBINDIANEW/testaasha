using A2ZMultiService;
using CyberPlatOpenSSL;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using org.vwipl;
using RestSharp;
using Rotativa;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using Vastwebmulti.Areas.ADMIN.Models;
using Vastwebmulti.Areas.FeeCollector.Models;
using Vastwebmulti.Areas.RETAILER.Models;
using Vastwebmulti.Areas.WRetailer.Models;
using Vastwebmulti.Areas.WRetailer.ViewModels;
using Vastwebmulti.Models;
using ZXing;

namespace Vastwebmulti.Areas.WRetailer.Controllers
{

    [Authorize(Roles = "Whitelabelretailer")]
    /// <summary>
    /// WRetailer Area - Manages WhiteLabel Retailer operations - recharge, bill payment, money transfer and reports
    /// </summary>
    public class HomeController : Controller
    {
        string VastbazaarBaseUrl = "http://api.vastbazaar.com/";
        OpenSSL ssl = new OpenSSL();
        StringBuilder str = new StringBuilder();
        ALLSMSSend smssend = new ALLSMSSend();
        public static string SessionNo = DateTime.Parse(DateTime.Now.ToString()).ToString("dddMMMddyyyyHHmmss");
        private string cert = "";
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
        VastwebmultiEntities db = new VastwebmultiEntities();
        VastBazaartoken Responsetoken = new VastBazaartoken();
        /// <summary>
        /// [GET] Displays the Retailer dashboard with today's recharge summary and account balance
        /// </summary>
        public ActionResult Dashboard()
        {
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

            //var type = "ALL";
            //DateTime? from = null;
            //DateTime? to = null;
            ViewBag.show = "Today";

            var userid = User.Identity.GetUserId();
            var vv = db.Whitelabel_Retailer_Details.Where(a => a.RetailerId == userid).SingleOrDefault();
            ViewBag.email = vv.Email;
            ViewBag.name = vv.RetailerName;
            ViewBag.frmname = vv.Frm_Name;
            ViewBag.image = vv.photo;

            //Retailer News
            var news = (from pp in db.Message_top where (pp.users == "Retailer" || pp.users == "All") where pp.status == "Y" && pp.UserId == whitelabelid select pp).ToList();
            if (news.Any())
            {
                ViewBag.news = news.FirstOrDefault().message;
                ViewBag.newimg = news.FirstOrDefault().image;
            }
            else
            {
                ViewBag.news = null;
                ViewBag.newimg = null;
            }
            //Upcomming Holiday
            ViewBag.showholiday = db.whitelabel_show_upcomming_holiday(userid, "WRetailer").ToList();


            return View();
        }

        #region Show all today and yesterday recharge
        public ActionResult Show_Today_All_Recharge(string yesterday)
        {
            var type = "";
            DateTime? from = null;
            DateTime? to = null;

            if (yesterday == "Today")
            {
                type = "ALL";
                ViewBag.show = "Today";
            }
            else
            {
                type = "";
                from = DateTime.Now.Date.AddDays(-1);
                to = DateTime.Now.Date;
                ViewBag.show = "Yesterday";
            }
            var userid = User.Identity.GetUserId();
            // Donught Chart
            var result = db.whitelable_show_Retailer_Today_Recharges(userid, type, from, to).ToList();
            //prepaid
            var prepaid = result.Where(a => a.operator_type == "PrePaid").SingleOrDefault() != null ? result.Where(a => a.operator_type == "PrePaid").SingleOrDefault().total : 0;
            // cashback
            var prepaidchaskback = result.Where(a => a.operator_type == "PrePaid").SingleOrDefault() != null ? result.Where(a => a.operator_type == "PrePaid").SingleOrDefault().cashback : 0;
            //postpaid
            var postpaid = result.Where(a => a.operator_type == "PostPaid").SingleOrDefault() != null ? result.Where(a => a.operator_type == "PostPaid").SingleOrDefault().total : 0;
            //cashback
            var postpaidchasback = result.Where(a => a.operator_type == "PostPaid").SingleOrDefault() != null ? result.Where(a => a.operator_type == "PostPaid").SingleOrDefault().cashback : 0;
            //dth
            var dth = result.Where(a => a.operator_type == "DTH").SingleOrDefault() != null ? result.Where(a => a.operator_type == "DTH").SingleOrDefault().total : 0;
            //cashback
            var dthcashback = result.Where(a => a.operator_type == "DTH").SingleOrDefault() != null ? result.Where(a => a.operator_type == "DTH").SingleOrDefault().cashback : 0;
            //landline
            var landline = result.Where(a => a.operator_type == "Landline").SingleOrDefault() != null ? result.Where(a => a.operator_type == "Landline").SingleOrDefault().total : 0;
            //cashback
            var landlinecashback = result.Where(a => a.operator_type == "Landline").SingleOrDefault() != null ? result.Where(a => a.operator_type == "Landline").SingleOrDefault().cashback : 0;
            //electricity
            var Electricity = result.Where(a => a.operator_type == "Electricity").SingleOrDefault() != null ? result.Where(a => a.operator_type == "Electricity").SingleOrDefault().total : 0;
            //cashback
            var Electricitycashback = result.Where(a => a.operator_type == "Electricity").SingleOrDefault() != null ? result.Where(a => a.operator_type == "Electricity").SingleOrDefault().cashback : 0;
            //gas
            var Gas = result.Where(a => a.operator_type == "Gas").SingleOrDefault() != null ? result.Where(a => a.operator_type == "Gas").SingleOrDefault().total : 0;
            //cashback
            var Gascashback = result.Where(a => a.operator_type == "Gas").SingleOrDefault() != null ? result.Where(a => a.operator_type == "Gas").SingleOrDefault().cashback : 0;
            //insurance
            var Insurance = result.Where(a => a.operator_type == "Insurance").SingleOrDefault() != null ? result.Where(a => a.operator_type == "Insurance").SingleOrDefault().total : 0;
            //cashback
            var Insurancecashback = result.Where(a => a.operator_type == "Insurance").SingleOrDefault() != null ? result.Where(a => a.operator_type == "Insurance").SingleOrDefault().cashback : 0;
            // DTH-Booking
            var dthbooking = result.Where(a => a.operator_type == "DTH-Booking").SingleOrDefault() != null ? result.Where(a => a.operator_type == "DTH-Booking").SingleOrDefault().total : 0;
            //cashback
            var dthbookingcashback = result.Where(a => a.operator_type == "DTH-Booking").SingleOrDefault() != null ? result.Where(a => a.operator_type == "DTH-Booking").SingleOrDefault().cashback : 0;
            // DTH-Booking
            var water = result.Where(a => a.operator_type == "Water").SingleOrDefault() != null ? result.Where(a => a.operator_type == "Water").SingleOrDefault().total : 0;
            //cashback
            var watercashback = result.Where(a => a.operator_type == "Water").SingleOrDefault() != null ? result.Where(a => a.operator_type == "Water").SingleOrDefault().cashback : 0;
            // total recharge and bill 
            var rechargeandbill = (prepaid + postpaid + dth + landline + Electricity + Gas + Insurance + dthbooking + water);
            //total cashback recharge operator 
            var rechargeearn = (prepaidchaskback + postpaidchasback + dthcashback + landlinecashback + Electricitycashback + Gascashback + Insurancecashback + dthbookingcashback + watercashback);

            var moneytransfer = result.Where(a => a.operator_type == "MoneyTransfer").SingleOrDefault() != null ? result.Where(a => a.operator_type == "MoneyTransfer").SingleOrDefault().total : 0;

            var dmtearn = result.Where(a => a.operator_type == "MoneyTransfer").SingleOrDefault() != null ? result.Where(a => a.operator_type == "MoneyTransfer").SingleOrDefault().cashback : 0;

            var aepsearn = result.Where(a => a.operator_type == "Aeps").SingleOrDefault() != null ? result.Where(a => a.operator_type == "Aeps").SingleOrDefault().cashback : 0;

            var aepsamount = result.Where(a => a.operator_type == "Aeps").SingleOrDefault() != null ? result.Where(a => a.operator_type == "Aeps").SingleOrDefault().total : 0;

            var pancardearn = result.Where(a => a.operator_type == "Pancard").SingleOrDefault() != null ? result.Where(a => a.operator_type == "Pancard").SingleOrDefault().cashback : 0;

            var pancardamount = result.Where(a => a.operator_type == "Pancard").SingleOrDefault() != null ? result.Where(a => a.operator_type == "Pancard").SingleOrDefault().total : 0;

            return Json(new { Status = yesterday, Recharge = rechargeandbill, Moneytransfer = moneytransfer, RechargeEran = rechargeearn, DmtEarn = dmtearn, Aeps = aepsamount, AepsEarn = aepsearn, Pancard = pancardamount, PancardEarn = pancardearn });
        }
        #endregion

        // GET: RETAILER/Home
        /// <summary>
        /// [GET] Displays the main Retailer index page with balance overview
        /// </summary>
        public ActionResult Index(string radiovalue)
        {
            var userid = User.Identity.GetUserId();
            ViewData["preandpostradio"] = "PrePaid";
            var mm = TempData["result"];
            ViewData["output"] = mm;
            ViewBag.optname = new SelectList(db.all_prepaid_code().GroupBy(test => test.optname).Select(grp => grp.First()), "optname", "optname");
            var operator_value_1 = (from opt in db.Operator_Code where (opt.Operator_type == "DTH") select opt);
            ViewBag.Operator1 = new SelectList(operator_value_1, "his_opt_id", "operator_Name", null);
            var operator_value = (from opt in db.opt_all_in_one select opt.optname).ToList().Distinct();
            ViewBag.Operator = new SelectList(operator_value);
            string txt_frm_date = DateTime.Now.ToString();
            string txt_to_date = DateTime.Now.ToString();
            string frm_date = Convert.ToDateTime(txt_frm_date).Date.ToString("yyyy-MM-dd");
            string to_date = Convert.ToDateTime(txt_to_date).AddDays(1).ToString("yyyy-MM-dd");
            Vastwebmulti.Areas.WRetailer.Models.remmulti viewModel = new Vastwebmulti.Areas.WRetailer.Models.remmulti();
            ViewBag.Newsletter = (from gg in db.Message_top where (gg.users == "WRetailer" || gg.users == "All") where gg.status == "Y" select gg).AsNoTracking().ToList();
            viewModel.allopt = db.Apps_opt_all().ToList();
            viewModel.optcode = (from aa in db.Operator_Code where aa.new_opt_code == "ADB" select aa).AsNoTracking().ToList();
            ViewBag.state = new SelectList(db.Select_State_Details(), "State_Id", "State_Name");

            // Load all operator codes once with AsNoTracking (read-only), then filter in memory
            var allvalue = db.Operator_Code.AsNoTracking().ToList();
            // operator code prepaid mobile
            var pre = allvalue.Where(a => a.Operator_type == "PrePaid").ToList();
            ViewBag.perpaid = new SelectList(pre, "new_opt_code", "operator_Name");

            // operator code postpaid mobile
            var post = allvalue.Where(a => a.Operator_type == "PostPaid").ToList();
            ViewBag.postpaid = new SelectList(post, "new_opt_code", "operator_Name");

            //Operator Code Electricity
            var hh = allvalue.Where(a => a.Operator_type == "Electricity").ToList();
            ViewBag.opeartor = new SelectList(hh, "new_opt_code", "operator_Name");
            //Operator Code DTH
            var dth = allvalue.Where(a => a.Operator_type == "DTH").ToList();
            ViewBag.dth = new SelectList(dth, "new_opt_code", "operator_Name");
            //Operator Code Landline
            var landline = allvalue.Where(a => a.Operator_type == "Landline").ToList();
            ViewBag.landline = new SelectList(landline, "new_opt_code", "operator_Name");

            //Operator Code Gas
            var gas = allvalue.Where(a => a.Operator_type == "Gas").ToList();
            ViewBag.gas = new SelectList(gas, "new_opt_code", "operator_Name");

            //Operator Code LPG Gas
            var LPGgas = allvalue.Where(a => a.Operator_type.ToUpper() == "LPG GAS").ToList();
            ViewBag.lpggas = new SelectList(LPGgas, "new_opt_code", "operator_Name");


            //Operator Code Fastag
            var Fastag = allvalue.Where(a => a.Operator_type.ToUpper() == "FASTAG").ToList();
            ViewBag.Fastag = new SelectList(Fastag, "new_opt_code", "operator_Name");

            //Operator Code Insurance
            var insurance = allvalue.Where(a => a.Operator_type == "Insurance").ToList();
            ViewBag.insurance = new SelectList(insurance, "new_opt_code", "operator_Name");

            // Operator Code Water
            var water = allvalue.Where(a => a.Operator_type == "Water").ToList();
            ViewBag.water = new SelectList(water, "new_opt_code", "operator_Name");

            // Operator Code Broadband
            var Broadband = allvalue.Where(a => a.Operator_type == "Broadband").ToList();
            ViewBag.Broadband = new SelectList(Broadband, "new_opt_code", "operator_Name");

            // Operator Code Loan
            var Loan = allvalue.Where(a => a.Operator_type == "Loan").ToList();
            ViewBag.Loandata = new SelectList(Loan, "new_opt_code", "operator_Name");

            // Operator Code DTH Booking
            var Dthbooking = allvalue.Where(a => a.Operator_type == "DTH-Booking").ToList();
            ViewBag.Dthbooking = new SelectList(Broadband, "new_opt_code", "operator_Name");

            Models.remmulti viewModell = new Vastwebmulti.Areas.WRetailer.Models.remmulti();
            //show comm and opertorwise image
            var ch = db.spWhitelabel_show_all_comm_operatorwise_retailer(userid, "Prepaid").ToList();
            var postfixName = Path.Combine(Server.MapPath("~/RetailerRechargeimg/"));
            var virtualpath = postfixName.Replace(postfixName, "/");
            var jj = (from p in ch
                      select new operatorlist { img = virtualpath + p.operator_Name + ".png", comm = p.Commission, operator_Name = p.operator_Name, blocktime = p.blocktime, opertor_type = p.Operator_type }).ToList();
            viewModell.optlist = jj;


            viewModell.recent_rechargereport = db.recent_recharge_report(userid, "Prepaid", "postpaid").ToList();

            return View(viewModell);

        }
        public ActionResult showrecentrecharge(string type)
        {
            var userid = User.Identity.GetUserId();
            Models.remmulti viewModell = new Models.remmulti();
            if (type == "Prepaid")
            {
                viewModell.recent_rechargereport = db.recent_recharge_report(userid, "Prepaid", "postpaid").ToList();
            }
            else
            {
                viewModell.recent_rechargereport = db.recent_recharge_report(userid, type, "").ToList();
            }
            return PartialView("_rechargerecentreport", viewModell);
        }
        #region Offer Plan
        //chehk best offer plan Mobile and DTH or Customer Info
        public ActionResult RechargeBestofferplan(string optname, string mobileno)
        {

            string url = "";
            var token = "";
            var emailid = "";
            var count = db.Bestoffers.Count();
            if (count > 0)
            {
                token = db.Bestoffers.Single().token;
                emailid = db.Bestoffers.Single().Emailid;
                if (optname == "Airtel")
                {
                    url = "https://www.vastwebindia.com/Offer/Roffer_check?userid=" + emailid + "&token=" + token + "&optnm=AIRTEL&number=" + mobileno;
                }
                if (optname == "Idea")
                {
                    url = "https://www.vastwebindia.com/Offer/Roffer_check?userid=" + emailid + "&token=" + token + "&optnm=IDEA&number=" + mobileno;
                }
                if (optname == "Vodafone")
                {
                    url = "https://www.vastwebindia.com/Offer/Roffer_check?userid=" + emailid + "&token=" + token + "&optnm=VODAFONE&number=" + mobileno;
                }
                if (optname == "Airtel Digital TV")
                {
                    url = "https://www.vastwebindia.com/Offer/DTHCustomerinfo?userid=" + emailid + "&token=" + token + "&optnm=AirtelDTH&number=" + mobileno;
                }
                if (optname == "Dish TV")
                {
                    url = "https://www.vastwebindia.com/Offer/DTHCustomerinfo?userid=" + emailid + "&token=" + token + "&optnm=DISHTV&number=" + mobileno;

                }
                if (optname == "Videocon D2H")
                {
                    url = "https://www.vastwebindia.com/Offer/DTHCustomerinfo?userid=" + emailid + "&token=" + token + "&optnm=VIDEOD2H&number=" + mobileno;
                }
                if (optname == "Tata Sky")
                {
                    url = "https://www.vastwebindia.com/Offer/DTHCustomerinfo?userid=" + emailid + "&token=" + token + "&optnm=TATASKY&number=" + mobileno;
                }
                if (url != "")
                {
                    HttpWebRequest WebRequestObject = (HttpWebRequest)HttpWebRequest.Create(url);
                    WebResponse webResponse = WebRequestObject.GetResponse();
                    Stream webStream = webResponse.GetResponseStream();
                    StreamReader responseReader = new StreamReader(webStream);
                    string response = responseReader.ReadToEnd();

                    var jss = new JavaScriptSerializer();
                    var dict = jss.Deserialize<dynamic>(response);
                    return Json(dict, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string response = "";
                    var jss = new JavaScriptSerializer();
                    var dict = jss.Deserialize<dynamic>(response);
                    return Json(dict, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {

                string response = "{'Response':'" + " Can not authorized use this service" + "','status':'FAILED'}";
                var jss = new JavaScriptSerializer();
                var dict = jss.Deserialize<dynamic>(response);
                return Json(dict, JsonRequestBehavior.AllowGet);
            }

        }

        //fill circle code and operator using api   1
        public ActionResult Checkcircle(string mobileno)
        {
            string url = "";
            var emailid = "";
            var count = db.Bestoffers.Count();
            if (count > 0)
            {
                emailid = db.Bestoffers.Single().Emailid;
                var token = db.Bestoffers.Single().token;
                if (mobileno != "" && mobileno != null)
                {
                    url = "https://www.vastwebindia.com/Offer/GetInfo?userid=" + emailid + "&token=" + token + "&number=" + mobileno;
                }
                HttpWebRequest WebRequestObject = (HttpWebRequest)HttpWebRequest.Create(url);
                WebResponse webResponse = WebRequestObject.GetResponse();
                Stream webStream = webResponse.GetResponseStream();
                StreamReader responseReader = new StreamReader(webStream);
                string response = responseReader.ReadToEnd();

                var jss = new JavaScriptSerializer();
                var dict = jss.Deserialize<dynamic>(response);
                return Json(dict, JsonRequestBehavior.AllowGet);
            }
            else
            {
                string response = "{'Response':'" + " Can not authorized use this service" + "','status':'FAILED'}";
                var jss = new JavaScriptSerializer();
                var dict = jss.Deserialize<dynamic>(response);
                return Json(dict, JsonRequestBehavior.AllowGet);
            }


        }


        //chehk View Plan offer Prepaid 2
        public ActionResult RechargeViewPlanoffer(string optname, string circlename)
        {
            string url = "";
            var token = "";
            var emailid = "";
            var count = db.Bestoffers.Count();
            if (count > 0)
            {
                token = db.Bestoffers.Single().token;
                emailid = db.Bestoffers.Single().Emailid;
                if (optname.Contains("BSNL Topup"))
                {
                    optname = "BSNL";
                }
                if (optname.Contains("JIO"))
                {
                    optname = "Jio";
                }

                url = "https://www.vastwebindia.com/Offer/Plan_check?userid=" + emailid + "&token=" + token + "&optnm=" + optname + "&circlenm=" + circlename + "&type=MOBILE";

                HttpWebRequest WebRequestObject = (HttpWebRequest)HttpWebRequest.Create(url);
                WebResponse webResponse = WebRequestObject.GetResponse();
                Stream webStream = webResponse.GetResponseStream();
                StreamReader responseReader = new StreamReader(webStream);
                string response = responseReader.ReadToEnd();

                var jss = new JavaScriptSerializer();
                var dict = jss.Deserialize<dynamic>(response);
                return Json(dict, JsonRequestBehavior.AllowGet);

            }
            else
            {

                string response = "{'Response':'" + " Can not authorized use this service" + "','status':'FAILED'}";
                var jss = new JavaScriptSerializer();
                var dict = jss.Deserialize<dynamic>(response);
                return Json(dict, JsonRequestBehavior.AllowGet);
            }

        }

        //chehk DTH View Plan offer
        public ActionResult RechargeViewPlanofferdth(string optname)
        {
            string url = "";
            var token = "";
            var emailid = "";
            var count = db.Bestoffers.Count();
            if (count > 0)
            {
                token = db.Bestoffers.Single().token;
                emailid = db.Bestoffers.Single().Emailid;

                url = "https://www.vastwebindia.com/Offer/Plan_check?userid=" + emailid + "&token=" + token + "&optnm=" + optname + "&circlenm=" + "" + "&type=DTH";

                HttpWebRequest WebRequestObject = (HttpWebRequest)HttpWebRequest.Create(url);
                WebResponse webResponse = WebRequestObject.GetResponse();
                Stream webStream = webResponse.GetResponseStream();
                StreamReader responseReader = new StreamReader(webStream);
                string response = responseReader.ReadToEnd();

                var jss = new JavaScriptSerializer();
                var dict = jss.Deserialize<dynamic>(response);
                return Json(dict, JsonRequestBehavior.AllowGet);

            }
            else
            {

                string response = "{'Response':'" + " Can not authorized use this service" + "','status':'FAILED'}";
                var jss = new JavaScriptSerializer();
                var dict = jss.Deserialize<dynamic>(response);
                return Json(dict, JsonRequestBehavior.AllowGet);
            }

        }

        //check View Plan dth details offer 
        public ActionResult RechargeViewPlanofferDetailsdth(string optname, string planname)
        {
            string url = "";
            var token = "";
            var emailid = "";
            var count = db.Bestoffers.Count();
            if (count > 0)
            {
                token = db.Bestoffers.Single().token;
                emailid = db.Bestoffers.Single().Emailid;
                url = " http://live.vastwebindia.com/LiveRecharge/Plan_Details?operatorName=" + optname + "&circle=" + "" + "&userid=" + emailid + "&name=" + planname + "&type=dth";

                HttpWebRequest WebRequestObject = (HttpWebRequest)HttpWebRequest.Create(url);
                WebResponse webResponse = WebRequestObject.GetResponse();
                Stream webStream = webResponse.GetResponseStream();
                StreamReader responseReader = new StreamReader(webStream);
                string response = responseReader.ReadToEnd();

                var jss = new JavaScriptSerializer();
                var dict = jss.Deserialize<dynamic>(response);
                return Json(dict, JsonRequestBehavior.AllowGet);
            }
            else
            {
                string response = "{'Response':'" + " Can not authorized use this service" + "','status':'FAILED'}";
                var jss = new JavaScriptSerializer();
                var dict = jss.Deserialize<dynamic>(response);
                return Json(dict, JsonRequestBehavior.AllowGet);
            }
        }
        //chehk View Plan Pripaid offer 3
        public ActionResult RechargeViewPlanofferDetails(string optname, string circlename, string planname)
        {

            string url = "";
            var token = "";
            var emailid = "";
            var count = db.Bestoffers.Count();
            if (count > 0)
            {
                token = db.Bestoffers.Single().token;
                emailid = db.Bestoffers.Single().Emailid;
                if (optname.Contains("BSNL Topup"))
                {
                    optname = "BSNL";
                }
                if (optname.Contains("JIO"))
                {
                    optname = "Jio";
                }
                //url = " http://live.vastwebindia.com/LiveRecharge/Plan_Details?operatorName=" + optname + "&circle=" + circlename + "&userid=" + emailid + "&name=" + planname + "&type=mobile";
                url = "https://www.vastwebindia.com/Offer/Plan_Details?userid=" + emailid + "&token=" + token + "&optnm=" + optname + "&circlenm=" + circlename + "&type=MOBILE&name=" + planname;
                HttpWebRequest WebRequestObject = (HttpWebRequest)HttpWebRequest.Create(url);
                WebResponse webResponse = WebRequestObject.GetResponse();
                Stream webStream = webResponse.GetResponseStream();
                StreamReader responseReader = new StreamReader(webStream);
                string response = responseReader.ReadToEnd();

                var jss = new JavaScriptSerializer();
                var dict = jss.Deserialize<dynamic>(response);
                return Json(dict, JsonRequestBehavior.AllowGet);

            }
            else
            {

                string response = "{'Response':'" + " Can not authorized use this service" + "','status':'FAILED'}";
                var jss = new JavaScriptSerializer();
                var dict = jss.Deserialize<dynamic>(response);
                return Json(dict, JsonRequestBehavior.AllowGet);
            }

        }

        //check view loan EMI details
        public ActionResult ViewLoanEmiBill(string OperatorName, string Loanaccountno, string mobileno)
        {
            string url = "";
            var token = "";
            var emailid = "";
            var count = db.Bestoffers.Count();
            if (count > 0)
            {
                token = db.Bestoffers.Single().token;
                emailid = db.Bestoffers.Single().Emailid;

                url = "https://www.vastwebindia.com/Offer/ViewLoan?userid=" + emailid + "&token=" + token + "&Loanname=" + OperatorName + "&accountno=" + Loanaccountno + "&Mobileno=" + mobileno + "";

                HttpWebRequest WebRequestObject = (HttpWebRequest)HttpWebRequest.Create(url);
                WebResponse webResponse = WebRequestObject.GetResponse();
                Stream webStream = webResponse.GetResponseStream();
                StreamReader responseReader = new StreamReader(webStream);
                string response = responseReader.ReadToEnd();

                var jss = new JavaScriptSerializer();
                var dict = jss.Deserialize<dynamic>(response);
                return Json(dict, JsonRequestBehavior.AllowGet);


            }
            else
            {

                string response = "{'Response':'" + " Can not authorized use this service" + "','status':'FAILED'}";
                var jss = new JavaScriptSerializer();
                var dict = jss.Deserialize<dynamic>(response);
                return Json(dict, JsonRequestBehavior.AllowGet);
            }

        }
        #endregion Offer Plan

        #region showcommissonoperatorwise
        [HttpGet]
        public ActionResult Show_commsion(string type)
        {
            var userid = User.Identity.GetUserId();
            Vastwebmulti.Areas.WRetailer.Models.remmulti viewModel = new Vastwebmulti.Areas.WRetailer.Models.remmulti();
            //show comm and opertorwise image
            var ch = db.spWhitelabel_show_all_comm_operatorwise_retailer(userid, type).ToList();
            var postfixName = Path.Combine(Server.MapPath("~/RetailerRechargeimg/"));
            var virtualpath = postfixName.Replace(postfixName, "/");
            var jj = (from p in ch
                      select new operatorlist { img = virtualpath + p.operator_Name + ".png", comm = p.Commission, operator_Name = p.operator_Name, blocktime = p.blocktime, opertor_type = p.Operator_type }).ToList();
            viewModel.optlist = jj;
            return PartialView("_operator_comm", viewModel);


        }
        #endregion
        // start Retailer Recharge 

        [HttpPost]
        /// <summary>
        /// [POST] Processes a prepaid recharge or utility bill payment transaction
        /// </summary>
        public ActionResult Recharge(string OperatorName, string OptCode, string mobileno, string Amount, string optional1, string optional2, string optional3, string optional4, string BillRefId)
        {
            if (OperatorName == "Bharti Axa Life Insurance")
            {
                optional1 = DateTime.ParseExact(optional1, "dd-MM-yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
            }
            Amount = Amount.Replace(".00", "");
            Amount = Amount.Replace(".0", "");
            decimal aaaaa = Convert.ToDecimal(Amount);
            var Blockstatus = db.AmountBlockunblocks.Where(a => a.OperatorName == OperatorName && a.Amount == aaaaa && a.status == "Y").ToList();
            if (Blockstatus.Any())
            {
                var responsemsg1 = "  {'Message': 'This Amount is Blocked by Admin.','Response':'ERROR'}";
                var jss = new JavaScriptSerializer();
                var dict = jss.Deserialize<dynamic>(responsemsg1.ToString());
                return Json(dict, JsonRequestBehavior.AllowGet);
            }
            else
            {

                var type = (db.Operator_Code.Where(aa => aa.new_opt_code == OptCode).Single().Operator_type);
                try
                {
                    string userid = User.Identity.GetUserId();
                    var dealerid = db.Whitelabel_Retailer_Details.Where(a => a.RetailerId == userid).Single().DealerId;
                    var whitelabelid = db.whitelabel_Dealer_Details.Where(a => a.DealerId == dealerid).Single().Whitelabelid;
                    // get mac address
                    var macaddress = GetMACAddress();
                    //Ip current Ip Address
                    var Ipaddress = GetComputer_InternetIP();
                    var responsemsg = "";
                    if (!string.IsNullOrEmpty(mobileno) && !string.IsNullOrEmpty(Amount))
                    {
                        var statusretailerrechargesuccess = db.Whitelabel_SMSSendAll.Where(a => a.ServiceName == "Rechsucconline" && a.Whitelabelid == whitelabelid).SingleOrDefault().Status;
                        var statusretailerrechargefailed = db.Whitelabel_SMSSendAll.Where(a => a.ServiceName == "RechFailedOnline" && a.Whitelabelid == whitelabelid).SingleOrDefault().Status;
                        var statusretailerrechargeProccess = db.Whitelabel_SMSSendAll.Where(a => a.ServiceName == "RecProconline" && a.Whitelabelid == whitelabelid).SingleOrDefault().Status;
                        var RetailerMobile = db.Whitelabel_Retailer_Details.Where(a => a.RetailerId == userid).SingleOrDefault().Mobile;
                        System.Data.Entity.Core.Objects.ObjectParameter output = new System.Data.Entity.Core.Objects.ObjectParameter("Output", typeof(string));
                        int amount = Convert.ToInt32(Amount);
                        // call Recharge procedure 
                        if (amount > 0)
                        {
                            try
                            {
                                //ENCRYPTIONMODELS cnctype = new ENCRYPTIONMODELS();
                                //var value1 = "12345678901234567890123456789012";
                                //var value2 = "0987654321098765";

                                //value1 = ENCRYPTIONMODELS.Base64Encode("12345678901234567890123456789012");
                                //value2 = ENCRYPTIONMODELS.Base64Encode("0987654321098765");

                                //var keyidsssenckey = ENCRYPTIONMODELS.Base64Decode(value1);
                                //var encIVIDS = ENCRYPTIONMODELS.Base64Decode(value2);

                                //userid = cnctype.convertdatatoALL(userid, keyidsssenckey, encIVIDS);
                                //mobileno = cnctype.convertdatatoALL(mobileno, keyidsssenckey, encIVIDS);
                                //OptCode = cnctype.convertdatatoALL(OptCode, keyidsssenckey, encIVIDS);
                                //Ipaddress = cnctype.convertdatatoALL(Ipaddress, keyidsssenckey, encIVIDS);


                                var Token = string.Empty;
                                Token = GetToken();
                                var client = new RestClient(Vastwebmulti.Areas.WHITELABEL.Models.WapiBaseUrl.GetBaseUrl() + "/Recharge/api/data/hkhk2?rd=" + userid + "&n=" + mobileno + "&ok=" + OptCode + "&amn=" + amount + "&pc=" + optional1 + "&bu=" + optional2 + "&acno=" + optional3 + "&lt=" + "" + "&ip=" + Ipaddress + "&mc=" + macaddress + "&em=" + "" + "&AppType=ONLINE" + "&value1=&value2=");
                                var request = new RestRequest(Method.POST);
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
                                dynamic stuff = Newtonsoft.Json.JsonConvert.DeserializeObject(responsechk);

                                responsemsg = "{'Message':'" + stuff.Message + "','Response':'" + stuff.Response + "'}";
                                var jss = new JavaScriptSerializer();
                                var dict = jss.Deserialize<dynamic>(responsemsg);
                                return Json(dict, JsonRequestBehavior.AllowGet);
                            }
                            catch (Exception ex)
                            {
                                WriteLog("ex " + ex.Message);
                                responsemsg = "{'Message':'" + ex.Message + "','Response':'ERROR'}";
                                var jss = new JavaScriptSerializer();
                                var dict = jss.Deserialize<dynamic>(responsemsg);
                                return Json(dict, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            responsemsg = " {'Message':'Amount must be greater than 0.','Response':'ERROR'}";
                            var jss = new JavaScriptSerializer();
                            var dict = jss.Deserialize<dynamic>(responsemsg);
                            return Json(dict, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        responsemsg = "  {'Message':'Mobile Number or Amount Should Not Be Blank.','Response':'ERROR'}";
                        var jss = new JavaScriptSerializer();
                        var dict = jss.Deserialize<dynamic>(responsemsg);
                        return Json(dict, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    var responsemsg1 = "  {'Message':'" + ex.Message + "','Response':'ERROR'}";
                    var jss = new JavaScriptSerializer();
                    var dict = jss.Deserialize<dynamic>(responsemsg1.ToString());
                    return Json(dict, JsonRequestBehavior.AllowGet);
                }

            }

        }

        [HttpPost]
        public ActionResult viewbill(string OperatorName, string OptCode, string mobileno, string Amount, string optional1, string optional2, string optional3, string optional4)
        {
            try
            {
                if (OperatorName == "Bharti Axa Life Insurance")
                {
                    optional1 = DateTime.ParseExact(optional1, "dd-MM-yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                }

                var Token = string.Empty;
                Token = GetToken();
                var client = new RestClient(Vastwebmulti.Areas.WHITELABEL.Models.WapiBaseUrl.GetBaseUrl() + "/Recharge/api/data/viewbill?billnumber=" + mobileno + "&Operator=" + OptCode + "&billunit=" + optional1 + "&ProcessingCycle=" + optional2 + "&acno=" + optional3 + "&lt=" + optional4 + "&ViewBill=Y");
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
                dynamic stuff = Newtonsoft.Json.JsonConvert.DeserializeObject(responsechk);

                var data = new
                {
                    Response = "SUCCESS",
                    Price = stuff.ADDINFO.BillInfo.billAmount,
                    billduedate = stuff.ADDINFO.BillInfo.billDueDate,
                    DisplayValues = JsonConvert.SerializeObject(stuff.ADDINFO.BillInfo.displayValues)
                };

                var responsemsg = JsonConvert.SerializeObject(data);
                var jss = new JavaScriptSerializer();
                var dict = jss.Deserialize<dynamic>(responsemsg);
                return Json(dict, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                test1 t1 = new test1();
                t1.name = String.Format("View Bill Exception Ex: {0}, Stack: {1}", ex.Message, ex.StackTrace);
                db.test1.Add(t1);
                db.SaveChanges();

                var data = new
                {
                    Response = "ERROR",
                    Message = ex.Message,
                    billduedate = "NA",
                    DisplayValues = "{'Customer Name':'NA'}"
                };

                var responsemsg = JsonConvert.SerializeObject(data);
                var jss = new JavaScriptSerializer();
                var dict = jss.Deserialize<dynamic>(responsemsg);
                return Json(dict, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// [GET] Displays mPOS transaction report
        /// </summary>
        public ActionResult m_Possreport()
        {
            var userid = User.Identity.GetUserId();
            string txt_frm_date = DateTime.Now.ToString();
            string txt_to_date = DateTime.Now.ToString();
            string frm_date = Convert.ToDateTime(txt_frm_date).Date.ToString("yyyy-MM-dd");
            string to_date = Convert.ToDateTime(txt_to_date).AddDays(1).ToString("yyyy-MM-dd");
            // show master id 

            var ch = db.Whitelabel_Mpos_Report_New("Retailer", userid, "ALL", 50, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            ViewData["totals"] = ch.Where(s => s.status == "00").Sum(s => Convert.ToInt32(s.amount));
            ViewData["Totalf"] = ch.Where(s => s.status != "00").Sum(s => Convert.ToInt32(s.amount));
            return View(ch);
        }
        [HttpPost]
        /// <summary>
        /// [POST] Filters mPOS report by date range
        /// </summary>
        public ActionResult m_Possreport(string txt_frm_date, string txt_to_date)
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

            var ch = db.Whitelabel_Mpos_Report_New("Retailer", userid, "ALL", 10000, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            ViewData["totals"] = ch.Where(s => s.status == "00").Sum(s => Convert.ToInt32(s.amount));
            ViewData["Totalf"] = ch.Where(s => s.status != "00").Sum(s => Convert.ToInt32(s.amount));
            return View(ch);
        }

        //get mac addresss 
        public string GetMACAddress()
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            String sMacAddress = string.Empty;
            foreach (NetworkInterface adapter in nics)
            {
                if (sMacAddress == String.Empty)// only return MAC Address from first card
                {
                    IPInterfaceProperties properties = adapter.GetIPProperties();
                    sMacAddress = adapter.GetPhysicalAddress().ToString();
                }
            }
            return sMacAddress;
        }
        //get your current Ip Address
        private string GetComputer_InternetIP()
        {
            string ipaddress;
            ipaddress = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (ipaddress == "" || ipaddress == null)
                ipaddress = Request.ServerVariables["REMOTE_ADDR"];
            return ipaddress;
        }
        //Fund Trnasfer Retailer To Retailer
        [HttpGet]
        public ActionResult Retailer_to_retailer()
        {
            var msg = TempData["msg"];
            ViewData["output"] = msg;
            if (msg != null)
            {
                if (msg.ToString().ToUpper().Contains("SUCCESS"))
                {
                    ViewData["Output1"] = "0";
                }
                else
                {
                    ViewData["Output1"] = "1";
                }
            }
            string userid = User.Identity.GetUserId();
            var emailid = db.Users.Where(a => a.UserId == userid).SingleOrDefault().Email;
            string txt_frm_date = DateTime.Now.ToString();
            string txt_to_date = DateTime.Now.ToString();
            string frm_date = Convert.ToDateTime(txt_frm_date).Date.ToString("yyyy-MM-dd");
            string to_date = Convert.ToDateTime(txt_to_date).AddDays(1).ToString("yyyy-MM-dd");
            var ch = db.show_rem_to_rem_bal(emailid, "ALL", Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            ViewBag.retaileremailid = db.Whitelabel_Retailer_Details.Select(r => new SelectListItem { Value = r.RetailerId, Text = r.Email }).ToList();
            var retaileremail = db.select_rem_rem_to_fundtransfer_dll(emailid).ToList();
            ViewBag.retailerid = new SelectList(retaileremail, "RetailerID", "RetailerName", null);
            return View(ch);
        }
        [HttpPost]
        public ActionResult Retailer_to_retailer(string RetailerID, string txt_frm_date, string txt_to_date)
        {
            if (RetailerID == "")
            {
                RetailerID = "ALL";
            }
            ViewData["output"] = TempData["msg"];
            string userid = User.Identity.GetUserId();
            var dealerid = db.Whitelabel_Retailer_Details.Where(a => a.RetailerId == userid).Single().DealerId;
            var emailid = db.Users.Where(a => a.UserId == userid).SingleOrDefault().Email;
            var rem = db.select_whitelabel_retailer_for_ddl("Dealer", dealerid).Where(re => re.RetailerId != userid).ToList();
            ViewBag.RetailerId = new SelectList(rem, "RetailerId", "RetailerName", null);
            ViewBag.RetailerId1 = new SelectList(rem, "RetailerId", "RetailerName", null);
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
            var ch = db.show_rem_to_rem_bal(emailid, RetailerID, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            var retaileremail = db.select_rem_rem_to_fundtransfer_dll(emailid).ToList();
            ViewBag.retailerid = new SelectList(retaileremail, "RetailerID", "RetailerName", null);
            return View(ch);
        }
        public ActionResult rem_rem_fund_transfer(string txtbal, string RetailerId)
        {
            string userid = User.Identity.GetUserId();
            string reatilerto = db.Whitelabel_Retailer_Details.Where(a => a.Email == RetailerId).SingleOrDefault().RetailerId;
            System.Data.Entity.Core.Objects.ObjectParameter output = new
            System.Data.Entity.Core.Objects.ObjectParameter("output", typeof(string));
            var ch = db.whitelabel_rem_to_rem_bal_transfer(userid, reatilerto, "", Convert.ToDecimal(txtbal), "", output).SingleOrDefault().msg.ToString();
            TempData["msg"] = ch;
            return RedirectToAction("Retailer_to_retailer");
        }
        //End
        //Fund Recive Whitelabel
        [HttpGet]
        /// <summary>
        /// [GET] Displays fund credit received report for today
        /// </summary>
        public ActionResult FundRecive()
        {
            string userid = User.Identity.GetUserId();
            string txt_frm_date = DateTime.Now.ToString();
            string txt_to_date = DateTime.Now.ToString();
            string frm_date = Convert.ToDateTime(txt_frm_date).Date.ToString("yyyy-MM-dd");
            string to_date = Convert.ToDateTime(txt_to_date).AddDays(1).ToString("yyyy-MM-dd");
            var ch = db.select_whitelabel_report_dealer_to_retailer_balance(userid, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            return View(ch);

        }
        [HttpPost]
        /// <summary>
        /// [POST] Filters fund received report by date range
        /// </summary>
        public ActionResult FundRecive(string txt_frm_date, string txt_to_date)
        {
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
            var ch = db.select_whitelabel_report_dealer_to_retailer_balance(userid, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            return View(ch);
        }

        //Fund Recive Dealer
        [HttpGet]
        /// <summary>
        /// [GET] Displays fund received from Dealer report
        /// </summary>
        public ActionResult Funddealer()
        {
            string userid = User.Identity.GetUserId();
            string txt_frm_date = DateTime.Now.ToString();
            string txt_to_date = DateTime.Now.ToString();
            string frm_date = Convert.ToDateTime(txt_frm_date).Date.ToString("yyyy-MM-dd");
            string to_date = Convert.ToDateTime(txt_to_date).AddDays(1).ToString("yyyy-MM-dd");
            var ch = db.show_fundrecive_retailer_wdealer(userid, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            return View(ch);

        }
        [HttpPost]
        public ActionResult Funddealer(string txt_frm_date, string txt_to_date)
        {
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
            var ch = db.show_fundrecive_retailer_wdealer(userid, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            return View(ch);
        }

        /// <summary>
        /// [GET] Displays referral benefit and commission earned from referrals
        /// </summary>
        public ActionResult ReferralBenefit()
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                var userid = User.Identity.GetUserId();
                DateTime frm_date = DateTime.Now.Date;
                DateTime to_date = DateTime.Now.AddDays(1).Date;
                ReferralcodeListRem fund = new ReferralcodeListRem();
                fund.Referalcode_benifitHistoryrem = (from cust in db.Referalcode_benifit_History
                                                      join ord in db.Whitelabel_Retailer_Details
                                                  on cust.Retailerid equals ord.RetailerId
                                                      where (cust.Retailerid == userid && cust.refdate >= frm_date && cust.refdate <= to_date)
                                                      select new ReferralcodeListRem
                                                      {
                                                          whitelabelid = cust.Whitelabelid,
                                                          Retailerid = ord.RetailerId,
                                                          Retailermail = ord.Email,
                                                          Retailername = ord.RetailerName,
                                                          Retailerfirmname = ord.Frm_Name,
                                                          Refamount = cust.Refamount,
                                                          whitelabelremainpre = cust.whiteremainpre,
                                                          whitelabelremainpost = cust.whiteremainpost,
                                                          retailerremainpre = cust.Rempre,
                                                          retailerremainpost = cust.rempost,
                                                          refdate = cust.refdate
                                                      }).OrderByDescending(aa => aa.refdate).ToList();
                var msg = TempData["result"];
                ViewData["output"] = msg;
                return View(fund);
            }
        }
        [HttpPost]
        public ActionResult ReferralBenefit(DateTime txt_frm_date, DateTime txt_to_date)
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                ViewBag.chk = "post";
                var userid = User.Identity.GetUserId();
                DateTime to_date = txt_to_date.AddDays(1).Date;
                ReferralcodeListRem fund = new ReferralcodeListRem();
                fund.Referalcode_benifitHistoryrem = (from cust in db.Referalcode_benifit_History
                                                      join ord in db.Whitelabel_Retailer_Details
                                                  on cust.Retailerid equals ord.RetailerId
                                                      where (cust.Retailerid == userid && cust.refdate >= txt_frm_date && cust.refdate <= to_date)
                                                      select new ReferralcodeListRem
                                                      {
                                                          whitelabelid = cust.Whitelabelid,
                                                          Retailerid = ord.RetailerId,
                                                          Retailermail = ord.Email,
                                                          Retailername = ord.RetailerName,
                                                          Retailerfirmname = ord.Frm_Name,
                                                          Refamount = cust.Refamount,
                                                          whitelabelremainpre = cust.whiteremainpre,
                                                          whitelabelremainpost = cust.whiteremainpost,
                                                          retailerremainpre = cust.Rempre,
                                                          retailerremainpost = cust.rempost,
                                                          refdate = cust.refdate
                                                      }).OrderByDescending(aa => aa.refdate).ToList();
                var msg = TempData["result"];
                ViewData["output"] = msg;
                return View(fund);
            }
        }
        //Invoice Pdf File


        //Invoice Pdf File
        public ActionResult GotoInvoicePDF(string Id, string ReciveFrom, string OldRemain, string Value, string Commission, string FinalValue, string NewRemain, string Date)
        {

            return new Rotativa.ActionAsPdf("InvoicePDF", new { Id = Id, ReciveFrom = ReciveFrom, OldRemain = OldRemain, Value = Value, Commission = Commission, FinalValue = FinalValue, NewRemain = NewRemain, Date = Date });
        }
        public ActionResult InvoicePDF(string Id, string ReciveFrom, string OldRemain, string Value, string Commission, string FinalValue, string NewRemain, string Date)
        {
            var PDF_Content = new RetailerInvoiceModel()
            {
                Commission = Commission,
                Date = Date,
                FinalValue = FinalValue,
                NewRemain = NewRemain,
                OldRemain = OldRemain,
                ReciveFrom = ReciveFrom,
                Value = Value
            };
            string userid = User.Identity.GetUserId();
            var details = db.Whitelabel_Retailer_Details.Where(aa => aa.RetailerId.ToLower() == userid.ToLower()).SingleOrDefault();
            TempData["retailername"] = details.RetailerName.ToUpper();
            TempData["frimname"] = details.Frm_Name.ToUpper();
            TempData["retailermobile"] = details.Mobile;
            TempData["retailerdate"] = Date;
            TempData["invoiceno"] = Id;
            TempData["date"] = DateTime.Now;

            return View(PDF_Content);
        }
        //end

        //Show Credit Report
        /// <summary>
        /// [GET] Displays credit ledger summary report
        /// </summary>
        public ActionResult show_credit_Report()
        {
            string userid = User.Identity.GetUserId();
            var show = db.select_retailer_credit_report_by_Wadmin(userid).ToList();
            return View(show);
        }

        /// <summary>
        /// [GET] Displays dealer-level credit ledger summary
        /// </summary>
        public ActionResult show_credit_Report_Dealer()
        {
            string userid = User.Identity.GetUserId();
            var show = db.select_Wretailer_credit_report_by_Wdealer(userid).ToList();
            return View(show);
        }
        //end
        //Bank Details
        //Bank Details
        [HttpGet]
        /// <summary>
        /// [GET] Displays bank account information management page
        /// </summary>
        public ActionResult Bank_info()
        {
            var userid = User.Identity.GetUserId();
            var dlmid = db.Whitelabel_Retailer_Details.Where(a => a.RetailerId == userid).Single().DealerId;
            var whitelabelid = db.whitelabel_Dealer_Details.Where(a => a.DealerId == dlmid).Single().Whitelabelid;
            var ch = (from jj in db.Whitelabel_bank_info where jj.userid == whitelabelid select jj).ToList();
            return View(ch);
        }

        /// <summary>
        /// [GET] Displays operator commission/slab information for this Retailer
        /// </summary>
        public ActionResult Operator_info()
        {
            var userid = User.Identity.GetUserId();
            Operater_Commission sb = new Operater_Commission();
            sb.Prepaid = (from cust in db.prepaid_whitelabel_retailer_comm
                          join ord in db.Operator_Code
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
            sb.Electricity_comm = (from cust in db.utility_whitelabel_retailer_comm
                                   join ord in db.Operator_Code
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
            sb.Money = (from cust in db.imps_whitelabel_retailer_comm
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
            sb.Pencard_comm = (from cust in db.Pancard_whitelabel_retailer_comm
                               where (cust.userid == userid)
                               select new Pencard
                               {
                                   TokenValueDigital = cust.tokenvalueDigital,
                                   TokenValuePhysical = cust.tokenvaluePhysical,
                                   Gst = cust.gst,
                                   CommissionDigital = cust.commDigital,
                                   CommissionPhysical = cust.commPhysical
                               }).ToList();
            sb.AEPS = (from cust in db.Aeps_comm_userwise
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
            sb.Water = (from cust in db.Water_whitelabel_retailer_comm
                        join ord in db.Operator_Code
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

            sb.Insurance = (from cust in db.Insurance_whitelabel_retailer_comm
                            join ord in db.Operator_Code
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

            sb.Broadband = (from cust in db.Broandband_whitelabel_retailer_comm
                            join ord in db.Operator_Code
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
        public ActionResult Operator_info(string ddlcomm)
        {
            var userid = User.Identity.GetUserId();
            Operater_Commission sb = new Operater_Commission();
            sb.Prepaid = (from cust in db.prepaid_whitelabel_retailer_comm
                          join ord in db.Operator_Code
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
            sb.Electricity_comm = (from cust in db.utility_whitelabel_retailer_comm
                                   join ord in db.Operator_Code
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
            sb.Money = (from cust in db.imps_whitelabel_retailer_comm
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
            sb.Pencard_comm = (from cust in db.Pancard_whitelabel_retailer_comm
                               where (cust.userid == userid)
                               select new Pencard
                               {
                                   TokenValueDigital = cust.tokenvalueDigital,
                                   TokenValuePhysical = cust.tokenvaluePhysical,
                                   Gst = cust.gst,
                                   CommissionDigital = cust.commDigital,
                                   CommissionPhysical = cust.commPhysical
                               }).ToList();
            sb.AEPS = (from cust in db.Aeps_comm_userwise
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
            sb.Water = (from cust in db.Water_whitelabel_retailer_comm
                        join ord in db.Operator_Code
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

            sb.Insurance = (from cust in db.Insurance_whitelabel_retailer_comm
                            join ord in db.Operator_Code
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

            sb.Broadband = (from cust in db.Broandband_whitelabel_retailer_comm
                            join ord in db.Operator_Code
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
        //Web Login Details
        /// <summary>
        /// [GET] Displays web login activity log for this Retailer
        /// </summary>
        public ActionResult WebLogin()
        {
            var userid = User.Identity.GetUserName();
            DateTime txt_frm_date = DateTime.Now.Date;
            DateTime txt_to_date = DateTime.Now;
            var Login_details = (db.Login_info.Where(aa => aa.UserId == userid && aa.CurrentLoginTime >= txt_frm_date && aa.CurrentLoginTime <= txt_to_date && aa.LoginFrom == "Web").Take(100).OrderByDescending(aa => aa.Idno)).ToList();
            return View(Login_details);
        }
        [HttpPost]
        /// <summary>
        /// [POST] Processes web login credentials
        /// </summary>
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

            var Login_details = (db.Login_info.Where(aa => aa.UserId == userid && aa.CurrentLoginTime >= frm_date && aa.CurrentLoginTime <= to_date && aa.LoginFrom == "Web").OrderByDescending(aa => aa.Idno)).ToList();
            return View(Login_details);

        }
        //Failed Web Login History

        /// <summary>
        /// [GET] Displays failed web login attempts
        /// </summary>
        public ActionResult WebFailedLogin()
        {
            var userid = User.Identity.GetUserName();
            DateTime txt_frm_date = DateTime.Now.Date;
            DateTime txt_to_date = DateTime.Now;
            var Faild_Login_details = (db.Failed_Login_info.Where(aa => aa.EmailId == userid && aa.LoginTime >= txt_frm_date && aa.LoginTime <= txt_to_date && aa.LoginFrom == "Web").Take(100).OrderByDescending(aa => aa.Idno)).ToList();
            return View(Faild_Login_details);
        }
        [HttpPost]
        /// <summary>
        /// [POST] Processes failed login attempt data
        /// </summary>
        public ActionResult WebFailedLogin(string txt_frm_date, string txt_to_date)
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

            var Faild_Login_details = (db.Failed_Login_info.Where(aa => aa.EmailId == userid && aa.LoginTime >= frm_date && aa.LoginTime <= to_date && aa.LoginFrom == "Web").OrderByDescending(aa => aa.Idno)).ToList();

            return View(Faild_Login_details);

        }

        //Login Failed
        [HttpGet]
        /// <summary>
        /// [GET] Displays failed web login attempts
        /// </summary>
        public ActionResult WebLoginFailed()
        {
            var userid = User.Identity.GetUserId();
            var Email = db.Users.Where(a => a.UserId == userid).Single().Email;
            var currentdate = DateTime.Now.Date;
            var Faild_Login_details = db.Failed_Login_info.Where(a => a.EmailId == Email && a.LoginTime > currentdate).OrderByDescending(a => a.Idno).ToList();
            return View(Faild_Login_details);
        }
        [HttpPost]
        /// <summary>
        /// [POST] Processes failed login attempt data
        /// </summary>
        public ActionResult WebLoginFailed(string txt_frm_date, string txt_to_date)
        {
            ViewBag.fromdate = txt_frm_date;
            ViewBag.todate = txt_to_date;
            var userid = User.Identity.GetUserId();
            var Email = db.Users.Where(a => a.UserId == userid).Single().Email;

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

            var Faild_Login_details = (db.Failed_Login_info.Where(aa => aa.LoginTime >= frm && aa.LoginTime <= to &&
           aa.EmailId == Email && aa.LoginFrom == "Web").OrderByDescending(aa => aa.Idno)).ToList();
            return View(Faild_Login_details);

        }
        //DisputeReport 
        [HttpGet]
        /// <summary>
        /// [GET] Displays Retailer's dispute/complaint report
        /// </summary>
        public ActionResult DisputeReport()
        {

            var userid = User.Identity.GetUserId();
            string txt_frm_date = DateTime.Now.ToString();
            string txt_to_date = DateTime.Now.ToString();
            DateTime frm_date = Convert.ToDateTime(txt_frm_date).Date;
            DateTime to_date = Convert.ToDateTime(txt_to_date).AddDays(1);
            var disputelist = db.show_whitelabel_dispute_list(userid, frm_date, to_date).ToList();
            return View(disputelist);
        }
        [HttpPost]
        /// <summary>
        /// [POST] Processes and displays transaction dispute report
        /// </summary>
        public ActionResult DisputeReport(string txt_frm_date, string txt_to_date)
        {
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
            var disputelist = db.show_whitelabel_dispute_list(userid, frm_date, to_date).ToList();
            return View(disputelist);
        }


        public ActionResult Help()
        {
            var userid = User.Identity.GetUserId();
            var dealerid = db.Whitelabel_Retailer_Details.Where(a => a.RetailerId == userid).FirstOrDefault().DealerId;
            var whitelabelid = db.whitelabel_Dealer_Details.Where(a => a.DealerId == dealerid).FirstOrDefault().Whitelabelid;
            var admininfo = db.WhiteLabel_userList.Where(a => a.WhiteLabelID == whitelabelid).FirstOrDefault();
            ViewBag.admin = admininfo;
            return View();
        }
        //END DisputeReport
        #region RechargeReport
        [HttpGet]
        /// <summary>
        /// [GET] Displays Retailer's recharge transaction report for today
        /// </summary>
        public ActionResult RechargeReport()
        {
            var operator_value = db.Operator_Code.Distinct().ToList();
            ViewBag.Operator = new SelectList(operator_value, "new_opt_code", "operator_Name");
            return View();
        }

        [HttpPost]
        /// <summary>
        /// [POST] Filters Retailer recharge report by date, status, operator and mobile
        /// </summary>
        public ActionResult RechargeReport(string txt_frm_date, string txt_to_date, string ddl_status, string Operator, string txtmob)
        {
            string userid = User.Identity.GetUserId();
            var operator_value = db.Operator_Code.Distinct().ToList();
            ViewBag.Operator = new SelectList(operator_value, "new_opt_code", "operator_Name");
            ViewBag.chk = "post";
            return View();
        }
        public class JsonModel
        {
            public string HTMLString { get; set; }
            public bool NoMoredata { get; set; }
        }
        [ChildActionOnly]
        /// <summary>
        /// [GET] Renders recharge report partial view
        /// </summary>
        public ActionResult _Rechargereport(string txt_frm_date, string txt_to_date, string ddl_status, string Operator, string txtmob)
        {
            string userid = User.Identity.GetUserId();
            if (txt_frm_date == null && txt_to_date == null && ddl_status == null && Operator == null && txtmob == null)
            {
                txt_frm_date = DateTime.Now.ToString();
                txt_to_date = DateTime.Now.ToString();
                ddl_status = ""; Operator = ""; txtmob = "";
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
            int pagesize = 50;
            var rowdata = db.Sp_Recharge_info_LazyLoad(1, pagesize, "Whitelabelretailer", userid, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), Operator, txtmob, ddl_status).ToList();
            return View(rowdata);
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
        [HttpPost]
        public ActionResult InfiniteScroll(int pageindex, DateTime txt_frm_date, DateTime txt_to_date, string ddl_status, string Operator, string txtmob)
        {
            string userid = User.Identity.GetUserId();
            System.Threading.Thread.Sleep(1000);
            int pagesize = 50;
            var tbrow = db.Sp_Recharge_info_LazyLoad(pageindex, pagesize, "Whitelabelretailer", userid, txt_frm_date, txt_to_date, Operator, txtmob, ddl_status).ToList();
            JsonModel jsonmodel = new JsonModel();
            jsonmodel.NoMoredata = tbrow.Count < pagesize;
            if (tbrow.Any())
            {
                jsonmodel.HTMLString = renderPartialViewtostring("_Rechargereport", tbrow);
            }
            else
            {
                jsonmodel.HTMLString = "";
            }

            return Json(jsonmodel);
        }

        public virtual ActionResult ExcelRechargereport(DateTime txt_frm_date, DateTime txt_to_date, string ddl_status, string Operator, string txtmob)
        {
            txt_to_date = txt_to_date.AddDays(1);
            DataTable dt = new DataTable();
            dt.Columns.Add("Status", typeof(string));
            dt.Columns.Add("Recharge No", typeof(string));
            dt.Columns.Add("Operator", typeof(string));
            dt.Columns.Add("Opt Type", typeof(string));
            dt.Columns.Add("Rch Amt", typeof(string));
            dt.Columns.Add("Income", typeof(string));
            dt.Columns.Add("Remain", typeof(string));
            dt.Columns.Add("Type", typeof(string));
            dt.Columns.Add("Operator ID", typeof(string));
            dt.Columns.Add("Rch Date", typeof(string));
            dt.Columns.Add("Resp Date", typeof(string));

            string userid = User.Identity.GetUserId();
            var respo = db.Sp_Recharge_info_LazyLoad(1, 10000000, "Whitelabelretailer", userid, txt_frm_date, txt_to_date, Operator, txtmob, ddl_status).ToList();

            foreach (var item in respo)
            {
                var sts = item.Rstaus;
                if (item.Rstaus.ToUpper().Contains("REQ"))
                {
                    sts = "Pending";
                }
                dt.Rows.Add(sts, item.Mobile, item.operator_Name, item.Operator_type, item.amount,
                    item.income, item.Remain,
                item.rch_type, item.OPt_id, item.Rch_time, item.Resp_time);
            }

            var grid = new GridView();
            grid.DataSource = dt;
            grid.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=RCH_Report.xls");
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
        /// <summary>
        /// [GET] Generates PDF export of the recharge report
        /// </summary>
        public ActionResult PDFRechargereport(DateTime txt_frm_date, DateTime txt_to_date, string ddl_status, string Operator, string txtmob)
        {
            string userid = User.Identity.GetUserId();
            var respo = db.Sp_Recharge_info_LazyLoad(1, 10000000, "Whitelabelretailer", userid, txt_frm_date, txt_to_date, Operator, txtmob, ddl_status).ToList();
            return new ViewAsPdf(respo);
        }

        public ActionResult FindTotal(DateTime txt_frm_date, DateTime txt_to_date, string ddl_status, string Operator, string txtmob)
        {
            txt_to_date = txt_to_date.AddDays(1);
            DateTime now = DateTime.Now.Date;
            var user = User.Identity.GetUserId();
            if (ddl_status.ToUpper() == "PENDING")
            {
                ddl_status = "REQ";
            }
            if (txt_frm_date == now)
            {
                var chk = db.Recharge_info.Where(aa => aa.Rch_from == user && aa.Rch_time >= txt_frm_date && aa.Rch_time <= txt_to_date && aa.Rstaus.Contains(ddl_status) && (Operator == "" ? aa.optcode.Contains(Operator) : aa.optcode == Operator) && aa.Mobile.Contains(txtmob)).ToList();
                var successtotal = chk.Where(aa => aa.Rstaus.ToUpper() == "SUCCESS").Sum(aa => aa.amount);
                var Failedtotal = chk.Where(aa => aa.Rstaus.ToUpper() == "FAILED").Sum(aa => aa.amount);
                var pendingtotal = chk.Where(aa => aa.Rstaus.ToUpper().Contains("REQ")).Sum(aa => aa.amount);
                var data = new
                {
                    success = successtotal,
                    failed = Failedtotal,
                    pending = pendingtotal
                };
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var chk = db.Recharge_info_old.Where(aa => aa.Rch_from == user && aa.Rch_time >= txt_frm_date && aa.Rch_time <= txt_to_date && aa.Rstaus.Contains(ddl_status) && (Operator == "" ? aa.optcode.Contains(Operator) : aa.optcode == Operator) && aa.Mobile.Contains(txtmob)).ToList();
                var chk_old = db.Recharge_info_old.Where(aa => aa.Rch_from == user && aa.Rch_time >= txt_frm_date && aa.Rch_time <= txt_to_date && aa.Rstaus.Contains(ddl_status) && (Operator == "" ? aa.optcode.Contains(Operator) : aa.optcode == Operator) && aa.Mobile.Contains(txtmob)).ToList();
                var all = chk.Union(chk_old);
                var successtotal = all.Where(aa => aa.Rstaus.ToUpper() == "SUCCESS").Sum(aa => aa.amount);
                var Failedtotal = all.Where(aa => aa.Rstaus.ToUpper() == "FAILED").Sum(aa => aa.amount);
                var pendingtotal = all.Where(aa => aa.Rstaus.ToUpper().Contains("REQ")).Sum(aa => aa.amount);
                var data = new
                {
                    success = successtotal,
                    failed = Failedtotal,
                    pending = pendingtotal
                };
                return Json(data, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult dispute(string id, string txtregion)
        {
            try
            {
                System.Data.Entity.Core.Objects.ObjectParameter output = new
                System.Data.Entity.Core.Objects.ObjectParameter("output", typeof(string));

                var ch = db.distute_insert(id, txtregion, output).SingleOrDefault().msg.ToString();
                return Json(ch, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GotoInvoicePDF1(string Id, string RechargeTo, string OptName, string amt, string OptID, string Date)
        {

            return new Rotativa.ActionAsPdf("InvoicePDF1", new { Id = Id, RechargeTo = RechargeTo, OptName = OptName, amt = amt, OptID = OptID, Date = Date });
        }
        public ActionResult InvoicePDF1(string Id, string RechargeTo, string OptName, string amt, string OptID, string Date)
        {
            string userid = User.Identity.GetUserId();
            var user = db.Whitelabel_Retailer_Details.Where(a => a.RetailerId == userid).FirstOrDefault();
            var PDF_Content = new RetailerInvoiceModel1()
            {

                RetailerName = user.RetailerName,
                RetailerAdd = user.Address,
                RetailerMobile = user.Mobile,
                pDate = DateTime.Now.ToString(),
                Amount = amt,
                Consumer = RechargeTo.Substring(5),
                Operator = OptName,
                optId = OptID,
                tDate = Date,
                Invoiceno = Id
            };

            return View(PDF_Content);
        }

        #endregion RechargeReport

        #region Money_Transfer_Report
        [HttpGet]
        /// <summary>
        /// [GET] Displays money transfer transaction report
        /// </summary>
        public ActionResult Money_Transfer_Report()
        {
            return View();
        }
        [HttpPost]
        /// <summary>
        /// [POST] Filters money transfer report by date
        /// </summary>
        public ActionResult Money_Transfer_Report(string txt_frm_date, string txt_to_date, string ddl_top, string ddl_status, string submit, string ddl_Type)
        {

            ViewBag.chk = "post";
            return View();

        }

        [ChildActionOnly]
        /// <summary>
        /// [GET] Renders money transfer report partial view with filters
        /// </summary>
        public ActionResult _Money_Transfer_Report(string txt_frm_date, string txt_to_date, string ddl_status, string ddl_Type)
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                if (txt_frm_date == null && txt_to_date == null && ddl_status == null)
                {
                    txt_frm_date = DateTime.Now.ToString();
                    txt_to_date = DateTime.Now.ToString();
                    ddl_status = "ALL";
                }
                if (ddl_status == "Status")
                {
                    ddl_status = "ALL";
                }
                if (ddl_Type == null || ddl_Type == "")
                {
                    ddl_Type = "ALL";
                }
                DateTime frm1 = Convert.ToDateTime(txt_frm_date);
                DateTime to1 = Convert.ToDateTime(txt_to_date);

                txt_frm_date = frm1.ToString("dd-MM-yyyy");
                txt_to_date = to1.ToString("dd-MM-yyyy");
                string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
                DateTime dt = DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
                DateTime dt1 = DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
                DateTime frm_date = dt.Date;
                DateTime to_date = dt1.AddDays(1);
                var userid = User.Identity.GetUserId();

                ddl_status = string.IsNullOrWhiteSpace(ddl_status) ? null : ddl_status;
                var proc_Response = db.money_transfer_report_paging("Whitelabelretailer", userid, ddl_status, "ALL", Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), ddl_Type, 1, 50).ToList();
                return View(proc_Response);
            }
        }
        [HttpPost]
        public ActionResult InfiniteScroll_money(int pageindex, string txt_frm_date, string txt_to_date, string ddl_status, string ddl_Type)
        {
            string userid = User.Identity.GetUserId();
            System.Threading.Thread.Sleep(1000);
            int pagesize = 50;
            if (ddl_status == "Status")
            {
                ddl_status = "ALL";
            }
            if (ddl_Type == null || ddl_Type == "")
            {
                ddl_Type = "ALL";
            }
            var tbrow = db.money_transfer_report_paging("Whitelabelretailer", userid, ddl_status, "ALL", Convert.ToDateTime(txt_frm_date), Convert.ToDateTime(txt_to_date), ddl_Type, pageindex, 50).ToList();

            JsonModel jsonmodel = new JsonModel();
            jsonmodel.NoMoredata = tbrow.Count < pagesize;
            if (tbrow.Any())
            {
                jsonmodel.HTMLString = renderPartialViewtostring("_Money_Transfer_Report", tbrow);
            }
            else
            {
                jsonmodel.HTMLString = "";
            }

            return Json(jsonmodel);
        }

        /// <summary>
        /// [GET] Displays new-format Micro ATM transaction report
        /// </summary>
        public ActionResult MIcroAtmReportnew()
        {
            string userid = User.Identity.GetUserId();
            DateTime from = DateTime.Now.Date;
            DateTime to = from.AddDays(1);
            Recent_report recent = new Recent_report();
            recent.Recent_report_imps = null;
            recent.Recent_report_Aeps = null;
            recent.Recent_mPosInfo = null;
            recent.Recent_PAN_CARD_IPAY = null;

            recent.Recent_microatm = db.MicroAtm_Trans_info.Where(x => x.retailerid == userid).OrderByDescending(x => x.transtime).Take(10);

            return PartialView("_RecentReport", recent);
        }

        /// <summary>
        /// [GET] Displays Micro ATM transaction report
        /// </summary>
        public ActionResult MIcroAtmReport()
        {
            string userid = User.Identity.GetUserId();
            DateTime from = DateTime.Now.Date;
            DateTime to = from.AddDays(1);

            var chk = db.microatm_report("Whitelabelretailer", userid, from, to, "");
            return View(chk);
        }

        [HttpPost]
        /// <summary>
        /// [POST] Filters Micro ATM report by date
        /// </summary>
        public ActionResult MIcroAtmReport(string ddl_status, DateTime txt_frm_date, DateTime txt_to_date)
        {
            ViewBag.chk = "post";
            string userid = User.Identity.GetUserId();
            DateTime to = txt_to_date.AddDays(1);
            var chk = db.microatm_report("Whitelabelretailer", userid, txt_frm_date, to, ddl_status);
            return View(chk);
        }

        [ChildActionOnly]
        /// <summary>
        /// [GET] Renders Micro ATM / mPOS report partial view
        /// </summary>
        public ActionResult _m_Possreport(string txt_frm_date, string txt_to_date, string ddl_status)
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                if (txt_frm_date == null && txt_to_date == null && ddl_status == null)
                {
                    txt_frm_date = DateTime.Now.ToString();
                    txt_to_date = DateTime.Now.ToString();
                    ddl_status = "ALL";
                }
                if (ddl_status == "Status")
                {
                    ddl_status = "ALL";
                }

                DateTime frm1 = Convert.ToDateTime(txt_frm_date);
                DateTime to1 = Convert.ToDateTime(txt_to_date);

                txt_frm_date = frm1.ToString("dd-MM-yyyy");
                txt_to_date = to1.ToString("dd-MM-yyyy");
                string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
                DateTime dt = DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
                DateTime dt1 = DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
                DateTime frm_date = dt.Date;
                DateTime to_date = dt1.AddDays(1);
                var userid = User.Identity.GetUserId();

                ddl_status = string.IsNullOrWhiteSpace(ddl_status) ? null : ddl_status;
                var proc_Response = db.Mpos_Report_New_paging(1, 50, "Whitelabelretailer", userid, "ALL", frm_date, to_date).ToList();
                return View(proc_Response);
            }
        }
        [HttpPost]
        public ActionResult InfiniteScroll_mposs(int pageindex, string txt_frm_date, string txt_to_date, string ddl_status)
        {
            string userid = User.Identity.GetUserId();
            System.Threading.Thread.Sleep(1000);
            int pagesize = 50;
            if (ddl_status == "Status")
            {
                ddl_status = "ALL";
            }
            var tbrow = db.Mpos_Report_New_paging(pageindex, 50, "Whitelabelretailer", userid, "ALL", Convert.ToDateTime(txt_frm_date), Convert.ToDateTime(txt_to_date)).ToList();

            JsonModel jsonmodel = new JsonModel();
            jsonmodel.NoMoredata = tbrow.Count < pagesize;
            if (tbrow.Any())
            {
                jsonmodel.HTMLString = renderPartialViewtostring("_m_Possreport", tbrow);
            }
            else
            {
                jsonmodel.HTMLString = "";
            }

            return Json(jsonmodel);
        }

        public ActionResult GeneratePDF(string Idno)
        {
            return new ViewAsPdf("IMPSPDF", new { Idno = Idno });
        }
        public ActionResult IMPSPDF(string Idno)
        {
            Vastwebmulti.Areas.ADMIN.Models.Convertword cc = new Vastwebmulti.Areas.ADMIN.Models.Convertword();
            var user = User.Identity.GetUserId();
            var retailerid = "";
            var ch = db.pdf_creator(Idno).ToList();
            retailerid = ch.First().rch_from;
            var gg = (from hh in db.Whitelabel_Retailer_Details where hh.RetailerId == retailerid select hh).ToList();
            ViewData["retailername"] = gg.Single().RetailerName;
            ViewData["firmname"] = gg.Single().Frm_Name;
            ViewData["retailermobile"] = gg.Single().Mobile;
            ViewData["retaileremail"] = gg.Single().Email;
            ViewData["retaileraddress"] = gg.Single().Address;
            ViewData["retailerpancard"] = gg.Single().PanCard;
            ViewData["surcharge"] = ch.Single().charge ?? 0;
            ViewData["netsurchargefee"] = ch.Sum(s => s.customer_fee_net_amount);
            ViewData["customerfeegst"] = ch.Sum(s => s.customer_fee_gst);
            ViewData["Total"] = ch.Sum(s => s.amount);
            var netsurchargefee = ch.Sum(s => s.customer_fee_net_amount);
            var tranamount = ch.Sum(s => s.amount);
            var customer_fee_gst = ch.Sum(s => s.customer_fee_gst);
            //Convence fees
            var convence = db.Convence_Fees.Where(a => a.RetailerId == user && a.Role == "DMT").ToList();
            decimal? totalconvencefees = 0;
            if (convence.Any())
            {
                ViewData["convencefees"] = convence.Single().Amount;
                totalconvencefees = convence.Single().Amount;
            }
            else
            {
                ViewData["convencefees"] = "0.00";
                totalconvencefees = 0;
            }
            var totalamount = netsurchargefee + tranamount + customer_fee_gst + totalconvencefees;
            ViewData["Totalamount"] = totalamount;
            //CONVERT to world in total amount
            var converword = cc.ConvertToWords(totalamount.ToString());
            ViewData["convertoword"] = converword;
            var ddtt = ch.First().trans_time;
            ViewData["retailerdate"] = ddtt;
            ViewData["invoiceno"] = Idno.ToUpper();
            ViewData["date"] = System.DateTime.Now;

            return new ViewAsPdf("IMPSPDF", ch);


        }

        public ActionResult RechargePDF(string Idno)
        {
            Vastwebmulti.Areas.ADMIN.Models.Convertword cc = new Vastwebmulti.Areas.ADMIN.Models.Convertword();
            var user = User.Identity.GetUserId();
            var retailerid = "";
            var ch = db.pdf_creator_recharge(Idno).ToList();
            retailerid = ch.First().Rch_from;
            var operatorcode = ch.First().optcode;
            var operatortype = db.Operator_Code.Where(a => a.new_opt_code == operatorcode).Single().operator_Name;
            ViewData["operatortype"] = operatortype;
            var gg = (from hh in db.Whitelabel_Retailer_Details where hh.RetailerId == retailerid select hh).ToList();
            ViewData["retailername"] = gg.Single().RetailerName;
            ViewData["firmname"] = gg.Single().Frm_Name;
            ViewData["retailermobile"] = gg.Single().Mobile;
            ViewData["retaileremail"] = gg.Single().Email;
            ViewData["retaileraddress"] = gg.Single().Address;
            ViewData["retailerpancard"] = gg.Single().PanCard;
            //ViewData["surcharge"] = ch.Single().charge;
            ViewData["netsurchargefee"] = ch.Sum(s => s.Retailer_Surcharge);
            ViewData["customerfeegst"] = ch.Sum(s => s.user_gst);
            ViewData["Total"] = ch.Sum(s => s.amount);
            var netsurchargefee = ch.Sum(s => s.Retailer_Surcharge);
            var tranamount = ch.Sum(s => s.amount);
            var customer_fee_gst = ch.Sum(s => s.user_gst);
            var netsurchargefees = (netsurchargefee - customer_fee_gst);
            ViewData["netsurchargefees"] = netsurchargefees;
            //Convence fees
            var convence = db.Convence_Fees.Where(a => a.RetailerId == user && a.Role == "Utility").ToList();
            decimal? totalconvencefees = 0;
            if (convence.Any())
            {
                ViewData["convencefees"] = convence.Single().Amount;
                totalconvencefees = convence.Single().Amount;
            }
            else
            {
                ViewData["convencefees"] = "0.00";
                totalconvencefees = 0;
            }
            var totalamount = netsurchargefees + tranamount + customer_fee_gst + totalconvencefees;
            ViewData["Totalamount"] = totalamount;
            //CONVERT to world in total amount
            var converword = cc.ConvertToWords(totalamount.ToString());
            ViewData["convertoword"] = converword;
            var ddtt = ch.First().Rch_time;
            ViewData["retailerdate"] = ddtt;
            ViewData["invoiceno"] = Idno.ToUpper();
            ViewData["date"] = System.DateTime.Now;
            return new ViewAsPdf("RechargePDF", ch);
        }
        #endregion

        [HttpGet]
        /// <summary>
        /// [GET] Displays Retailer ledger/transaction report for today
        /// </summary>
        public ActionResult RetailerLedger()
        {
            var userid = User.Identity.GetUserId();
            string txt_to_date1 = DateTime.Now.ToString();
            string frm_date = Convert.ToDateTime(txt_to_date1).ToShortDateString();
            string to_date = Convert.ToDateTime(txt_to_date1).AddDays(1).ToString();
            var ledger = db.whitelabel_Retailer_Cr_Dr_Report("Retailer", userid, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            return View(ledger);
        }
        [HttpPost]
        /// <summary>
        /// [POST] Filters Retailer ledger report by date range
        /// </summary>
        public ActionResult RetailerLedger(string txt_frm_date, string txt_to_date)
        {
            var userid = User.Identity.GetUserId();
            ViewBag.chk = "post";
            DateTime frm = Convert.ToDateTime(txt_frm_date);
            DateTime to = Convert.ToDateTime(txt_to_date);
            txt_frm_date = frm.ToString("dd-MM-yyyy");
            txt_to_date = to.ToString("dd-MM-yyyy");
            string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
            DateTime dt = !string.IsNullOrWhiteSpace(txt_frm_date) ? DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
            DateTime dt1 = !string.IsNullOrWhiteSpace(txt_to_date) ? DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
            DateTime frm_date = Convert.ToDateTime(dt).Date;
            DateTime to_date = Convert.ToDateTime(dt1).Date.AddDays(1);
            var ledger = db.whitelabel_Retailer_Cr_Dr_Report("Retailer", userid, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            return View(ledger);

        }
        //Gst Form Declartion
        /// <summary>
        /// [GET] Displays GST transaction report for the Retailer
        /// </summary>
        public ActionResult GST_Report()
        {
            DateTime fromdate = DateTime.Now.Date;
            DateTime todate = DateTime.Now.Date;
            string userid = User.Identity.GetUserId();
            System.Data.Entity.Core.Objects.ObjectParameter openingBal = new
            System.Data.Entity.Core.Objects.ObjectParameter("OpeningBal", typeof(string));
            System.Data.Entity.Core.Objects.ObjectParameter ClosingBal = new
            System.Data.Entity.Core.Objects.ObjectParameter("ClosingBal", typeof(string));
            System.Data.Entity.Core.Objects.ObjectParameter PurchaseAmt = new
            System.Data.Entity.Core.Objects.ObjectParameter("PurchaseAmt", typeof(string));
            System.Data.Entity.Core.Objects.ObjectParameter IsIGST = new
            System.Data.Entity.Core.Objects.ObjectParameter("IsIGST", typeof(string));
            var gst = db.GST_Report_Retailer(fromdate, todate, userid, openingBal, ClosingBal, PurchaseAmt, IsIGST).ToList();
            // var gst = db.GST_Report_Retailer(fromdate, todate, userid, openingBal, ClosingBal, PurchaseAmt, IsIGST).ToList();

            ViewBag.OpnBal = !string.IsNullOrWhiteSpace(openingBal.Value.ToString()) ? openingBal.Value.ToString() : "0";
            ViewBag.CloseBal = !string.IsNullOrWhiteSpace(ClosingBal.Value.ToString()) ? ClosingBal.Value.ToString() : "0";
            ViewBag.Purchase = !string.IsNullOrWhiteSpace(PurchaseAmt.Value.ToString()) ? PurchaseAmt.Value.ToString() : "0";
            ViewBag.IsIGST = !string.IsNullOrWhiteSpace(IsIGST.Value.ToString()) ? IsIGST.Value.ToString() : "0";
            ViewBag.IsFormSubmmited = db.GST_Declaration.Where(a => a.UserId.ToLower() == userid.ToLower()).SingleOrDefault();
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
        /// <summary>
        /// [POST] Filters GST report by date range
        /// </summary>
        public ActionResult GST_Report(string txt_frm_date, string txt_to_date, HttpPostedFileBase file)
        {
            string userid = User.Identity.GetUserId();
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


            System.Data.Entity.Core.Objects.ObjectParameter openingBal = new
            System.Data.Entity.Core.Objects.ObjectParameter("OpeningBal", typeof(string));
            System.Data.Entity.Core.Objects.ObjectParameter ClosingBal = new
            System.Data.Entity.Core.Objects.ObjectParameter("ClosingBal", typeof(string));
            System.Data.Entity.Core.Objects.ObjectParameter PurchaseAmt = new
            System.Data.Entity.Core.Objects.ObjectParameter("PurchaseAmt", typeof(string));
            System.Data.Entity.Core.Objects.ObjectParameter IsIGST = new
            System.Data.Entity.Core.Objects.ObjectParameter("IsIGST", typeof(string));

            var gst = db.GST_Report_Retailer(frm_date, to_date, userid, openingBal, ClosingBal, PurchaseAmt, IsIGST).ToList();
            // var gst = db.GST_Report_Retailer(fromdate, todate, userid, openingBal, ClosingBal, PurchaseAmt, IsIGST).ToList();

            ViewBag.OpnBal = !string.IsNullOrWhiteSpace(openingBal.Value.ToString()) ? openingBal.Value.ToString() : "0";
            ViewBag.CloseBal = !string.IsNullOrWhiteSpace(ClosingBal.Value.ToString()) ? ClosingBal.Value.ToString() : "0";
            ViewBag.Purchase = !string.IsNullOrWhiteSpace(PurchaseAmt.Value.ToString()) ? PurchaseAmt.Value.ToString() : "0";
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
                        var email = db.Whitelabel_Retailer_Details.Where(a => a.RetailerId == userid).SingleOrDefault().Email;
                        string _FileName = Guid.NewGuid().ToString() + '_' + Path.GetFileName(file.FileName);
                        string _path = Path.Combine(Server.MapPath("~/GST_Declaration"), _FileName);
                        file.SaveAs(_path);
                        GST_Declaration obj = new GST_Declaration();
                        obj.UserId = userid;
                        obj.FilePath = _FileName;
                        obj.Status = "Pending";
                        obj.Emailid = email;
                        obj.Date = DateTime.Now;
                        db.GST_Declaration.Add(obj);
                        db.SaveChanges();
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

            ViewBag.IsFormSubmmited = db.GST_Declaration.Where(a => a.UserId.ToLower() == userid.ToLower()).SingleOrDefault();
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

            var invc = db.generate_GST_Invoice(userid, MonthName, Year, InvoiceNo).SingleOrDefault().Response.ToString();
            DateTime frm_date = txt_frm_date.Date;
            DateTime to_date = txt_to_date.Date;
            Vastwebmulti.Areas.WRetailer.Models.GST_PDF_MODEL model = new Vastwebmulti.Areas.WRetailer.Models.GST_PDF_MODEL();
            var role = db.showrole(userid).SingleOrDefault();
            System.Data.Entity.Core.Objects.ObjectParameter openingBal = new
            System.Data.Entity.Core.Objects.ObjectParameter("OpeningBal", typeof(string));
            System.Data.Entity.Core.Objects.ObjectParameter ClosingBal = new
            System.Data.Entity.Core.Objects.ObjectParameter("ClosingBal", typeof(string));
            System.Data.Entity.Core.Objects.ObjectParameter PurchaseAmt = new
            System.Data.Entity.Core.Objects.ObjectParameter("PurchaseAmt", typeof(string));
            System.Data.Entity.Core.Objects.ObjectParameter IsIGST = new
            System.Data.Entity.Core.Objects.ObjectParameter("IsIGST", typeof(string));
            if (role.Name.Contains("Whitelabelretailer"))
            {
                model.RetailerGst = db.GST_Report_Whitelabel_Retailer(frm_date, to_date, userid, openingBal, ClosingBal, PurchaseAmt, IsIGST).
                          Where(a => a.operator_Name != "Dish TV" && !a.Operator_type.Contains("PostPaid") && a.Operator_type != "Landline" && a.Operator_type != "Electricity" && a.Operator_type != "Gas" && a.Operator_type != "Insurance" && a.Operator_type != "Money" && a.Operator_type != "DTH-Booking").ToList();
                //Convert function call
                var converword = new Vastwebmulti.Areas.WRetailer.Models.Convertword().changeToWords(model.RetailerGst.Sum(a => a.NetAmount).ToString());
                ViewData["total"] = converword;
                //ViewData["total"] = changeToWords(model.RetailerGst.Sum(a => a.NetAmount).ToString());
                var UserDetails = db.Whitelabel_Retailer_Details.Where(a => a.RetailerId.ToLower() == userid.ToLower()).SingleOrDefault();
                ViewBag.UserDetails = UserDetails;
            }

            ViewBag.OpnBal = !string.IsNullOrWhiteSpace(openingBal.Value.ToString()) ? openingBal.Value.ToString() : "0";
            ViewBag.CloseBal = !string.IsNullOrWhiteSpace(ClosingBal.Value.ToString()) ? ClosingBal.Value.ToString() : "0";
            ViewBag.Purchase = !string.IsNullOrWhiteSpace(PurchaseAmt.Value.ToString()) ? PurchaseAmt.Value.ToString() : "0";
            ViewBag.IsIGST = !string.IsNullOrWhiteSpace(IsIGST.Value.ToString()) ? IsIGST.Value.ToString() : "0";
            var dealerid = db.Whitelabel_Retailer_Details.Where(a => a.RetailerId == userid).SingleOrDefault().DealerId;
            var whitelabelid = db.whitelabel_Dealer_Details.Where(a => a.DealerId == dealerid).SingleOrDefault().Whitelabelid;
            var AdminDetails = db.WhiteLabel_userList.Where(a => a.WhiteLabelID == whitelabelid).SingleOrDefault();
            ViewBag.AdminDetails = AdminDetails;
            ViewBag.Role = role;


            model.INVOICENO = invc;
            //ViewBag.State = db.State_Desc.Where(y => y.State_id == AdminDetails.State).Single().State_name;
            ViewBag.city = db.District_Desc.Where(f => f.Dist_id == AdminDetails.district && f.State_id == AdminDetails.state).Single().Dist_Desc;

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
            var gst = db.GST_Report_Retailer(frm_date, to_date, userid, openingBal, ClosingBal, PurchaseAmt, IsIGST).ToList();
            // var gst = db.GST_Report_Retailer(fromdate, todate, userid, openingBal, ClosingBal, PurchaseAmt, IsIGST).ToList();

            ViewBag.OpnBal = !string.IsNullOrWhiteSpace(openingBal.Value.ToString()) ? openingBal.Value.ToString() : "0";
            ViewBag.CloseBal = !string.IsNullOrWhiteSpace(ClosingBal.Value.ToString()) ? ClosingBal.Value.ToString() : "0";
            ViewBag.Purchase = !string.IsNullOrWhiteSpace(PurchaseAmt.Value.ToString()) ? PurchaseAmt.Value.ToString() : "0";
            ViewBag.IsIGST = !string.IsNullOrWhiteSpace(IsIGST.Value.ToString()) ? IsIGST.Value.ToString() : "0";
            var UserDetails = db.Whitelabel_Retailer_Details.Where(a => a.RetailerId.ToLower() == userid.ToLower()).SingleOrDefault();
            ViewBag.UserDetails = UserDetails;
            var AdminDetails = db.Admin_details.SingleOrDefault();
            ViewBag.AdminDetails = AdminDetails;
            return View(gst);
        }
        //End 

        #region AEPS
        //public ActionResult AEPS()
        //{
        //    var userid = User.Identity.GetUserId();
        //    var sts = db.Whitelabel_Retailer_Details.Where(a => a.RetailerId == userid && a.AepsMposstatus == "Y" && a.AepsMposstatus != null).Any();
        //    if (sts == true)
        //    {
        //        var retailerDetails = db.Whitelabel_Retailer_Details.Where(a => a.RetailerId == userid).FirstOrDefault();
        //        sts = db.whitelabel_Dealer_Details.Where(a => a.DealerId == retailerDetails.DealerId && a.AepsMposstatus == "Y" && a.AepsMposstatus != null).Any();
        //        if (sts == true)
        //        {
        //            if (retailerDetails != null)
        //            {
        //                string str = string.Empty;
        //                bool IsUpdateRequired = false;
        //                if (retailerDetails.AadharCard == null || retailerDetails.AadharCard.Length < 12)
        //                {
        //                    str = str + "Aadhar,";
        //                    IsUpdateRequired = true;
        //                }
        //                if (retailerDetails.PanCard == null || retailerDetails.PanCard.Length < 10)
        //                {
        //                    str = str + "Pancard,";
        //                    IsUpdateRequired = true;
        //                }
        //                if (retailerDetails.Frm_Name == null || string.IsNullOrWhiteSpace(retailerDetails.Frm_Name))
        //                {
        //                    str = str + "Firm name,";
        //                    IsUpdateRequired = true;
        //                }
        //                if (retailerDetails.Address == null || string.IsNullOrWhiteSpace(retailerDetails.Address))
        //                {
        //                    str = str + "Address,";
        //                    IsUpdateRequired = true;
        //                }

        //                if (retailerDetails.Pincode == 0 || retailerDetails.Pincode.ToString().Length < 6)
        //                {
        //                    str = str + "Pincode,";
        //                    IsUpdateRequired = true;
        //                }

        //                if (IsUpdateRequired)
        //                {
        //                    if (str.EndsWith(","))
        //                    str = str.Substring(0, str.Length - 1);
        //                    str = "Firstly Complete Your Full KYC";

        //                    var results1 = "{'status':'NOTOK','msg':'" + str + "'}";
        //                    var json11 = JsonConvert.DeserializeObject(results1);
        //                    var json111 = JsonConvert.SerializeObject(json11);
        //                    return Json(json111, JsonRequestBehavior.AllowGet);

        //                }
        //                // Fing Pay
        //                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
        //                var client = new RestClient("https://fingpayap.tapits.in/fpaepsservice/api/bankdata/bank/details");
        //                var request = new RestRequest(Method.GET);
        //                IRestResponse response = client.Execute(request);
        //                var items = JsonConvert.DeserializeObject<BankIiNoModel>(response.Content);
        //                var blist = items.Data.Select(a => new SelectListItem { Text = a.BankName, Value = a.IInNo }).ToList();

        //                var results = "{'status':'OK','aepsid':'" + retailerDetails.Email + "','banklist':'" + response.Content + "'}";
        //                var json1 = JsonConvert.DeserializeObject(results);
        //                var json = JsonConvert.SerializeObject(json1);
        //                return Json(json, JsonRequestBehavior.AllowGet);
        //            }
        //            else
        //            {
        //                var results = "{'status':'ERR','msg':''}";
        //                var json1 = JsonConvert.DeserializeObject(results);
        //                var json = JsonConvert.SerializeObject(json1);
        //                return Json(json, JsonRequestBehavior.AllowGet);
        //            }
        //        }
        //        else
        //        {
        //            var results = "{'status':'NOTOK','msg':'Your Aeps Status Inactive Please Contact Distributor OR Customer Care'}";
        //            var json1 = JsonConvert.DeserializeObject(results);
        //            var json = JsonConvert.SerializeObject(json1);
        //            return Json(json, JsonRequestBehavior.AllowGet);

        //        }
        //    }
        //    else
        //    {
        //        var results = "{'status':'NOTOK','msg':'Your Aeps Status Inactive Please Contact Admin'}";
        //        var json1 = JsonConvert.DeserializeObject(results);
        //        var json = JsonConvert.SerializeObject(json1);
        //        return Json(json, JsonRequestBehavior.AllowGet);
        //        // return View();
        //    }
        //}

        /// <summary>
        /// [GET] Displays AEPS (Aadhaar Enabled Payment System) transaction interface
        /// </summary>
        public ActionResult AEPS()
        {
            var check = "OK"; var errormsg = "";
            var userid = User.Identity.GetUserId();
            var sts = db.Whitelabel_Retailer_Details.Where(a => a.RetailerId == userid && a.AepsMposstatus == "Y" && a.AepsMposstatus != null).Any();
            if (sts == true)
            {
                var retailerDetails = db.Whitelabel_Retailer_Details.Where(a => a.RetailerId == userid).FirstOrDefault();
                sts = db.whitelabel_Dealer_Details.Where(a => a.DealerId == retailerDetails.DealerId && a.AepsMposstatus == "Y" && a.AepsMposstatus != null).Any();
                if (sts == true)
                {



                    if (retailerDetails != null)
                    {
                        string str = string.Empty;
                        bool IsUpdateRequired = false;
                        if (retailerDetails.AadharCard == null || retailerDetails.AadharCard.Length < 12)
                        {
                            str = str + "Aadhar,";
                            IsUpdateRequired = true;
                        }
                        if (retailerDetails.PanCard == null || retailerDetails.PanCard.Length < 10)
                        {
                            str = str + "Pancard,";
                            IsUpdateRequired = true;
                        }
                        if (retailerDetails.Frm_Name == null || string.IsNullOrWhiteSpace(retailerDetails.Frm_Name))
                        {
                            str = str + "Firm name,";
                            IsUpdateRequired = true;
                        }
                        if (retailerDetails.Address == null || string.IsNullOrWhiteSpace(retailerDetails.Address))
                        {
                            str = str + "Address,";
                            IsUpdateRequired = true;
                        }

                        //if (retailerDetails.AepsMerchandId == null || string.IsNullOrWhiteSpace(retailerDetails.AepsMerchandId))
                        //{
                        //    str = str + "AepsMerchant Id,";
                        //    IsUpdateRequired = true;
                        ////}
                        //if (retailerDetails.AepsMPIN == null || string.IsNullOrWhiteSpace(retailerDetails.AepsMPIN))
                        //{
                        //    str = str + "AepsMerchant Pin,";
                        //    IsUpdateRequired = true;
                        //}
                        //if (retailerDetails.Position == null || string.IsNullOrWhiteSpace(retailerDetails.Position))
                        //{
                        //    str = str + "Position,";
                        //    IsUpdateRequired = true;
                        //}
                        //if (retailerDetails.BusinessType == null || string.IsNullOrWhiteSpace(retailerDetails.BusinessType))
                        //{
                        //    str = str + "Business Type,";
                        //    IsUpdateRequired = true;
                        //}
                        //if (retailerDetails.Bankaccountno == null || string.IsNullOrWhiteSpace(retailerDetails.Bankaccountno))
                        //{
                        //    str = str + "Bank accountno,";
                        //    IsUpdateRequired = true;
                        //}
                        //if (retailerDetails.Ifsccode == null || string.IsNullOrWhiteSpace(retailerDetails.Ifsccode))
                        //{
                        //    str = str + "IFSC Code,";
                        //    IsUpdateRequired = true;
                        ////}
                        //if (retailerDetails.city == null || string.IsNullOrWhiteSpace(retailerDetails.city))
                        //{
                        //    str = str + "City,";
                        //    IsUpdateRequired = true;
                        //}
                        //if (retailerDetails.accountholder == null || string.IsNullOrWhiteSpace(retailerDetails.accountholder))
                        //{
                        //    str = str + "Accountholder,";
                        //    IsUpdateRequired = true;
                        //}
                        //if (retailerDetails.bankname == null || string.IsNullOrWhiteSpace(retailerDetails.bankname))
                        //{
                        //    str = str + "Bank Name,";
                        //    IsUpdateRequired = true;
                        //}
                        //if (retailerDetails.pancardPath == null || string.IsNullOrWhiteSpace(retailerDetails.pancardPath))
                        //{
                        //    str = str + "Pancard Document,";
                        //    IsUpdateRequired = true;
                        //}
                        //if (retailerDetails.aadharcardPath == null || string.IsNullOrWhiteSpace(retailerDetails.aadharcardPath))
                        //{
                        //    str = str + "Aadhaar Card Document,";
                        //    IsUpdateRequired = true;
                        //}
                        //if (retailerDetails.frimregistrationPath == null || string.IsNullOrWhiteSpace(retailerDetails.frimregistrationPath))
                        //{
                        //    str = str + "Registration Document,";
                        //    IsUpdateRequired = true;
                        //}
                        //if (retailerDetails.Photo == null || string.IsNullOrWhiteSpace(retailerDetails.Photo))
                        //{
                        //    str = str + "Passport Image Document,";
                        //    IsUpdateRequired = true;
                        //}

                        if (retailerDetails.Pincode == 0 || retailerDetails.Pincode.ToString().Length < 6)
                        {
                            str = str + "Pincode,";
                            IsUpdateRequired = true;
                        }

                        //if (retailerDetails.dateofbirth == null)
                        //{
                        //    str = str + "Date of birth";
                        //    IsUpdateRequired = true;
                        //}
                        //if (retailerDetails.AepsMerchandId == null)
                        //{
                        //    str = str + "AEPS Merchand Id";
                        //    IsUpdateRequired = true;
                        //}
                        //if (retailerDetails.AepsMPIN == null)
                        //{
                        //    str = str + "AEPS MPIN";
                        //    IsUpdateRequired = true;
                        //}

                        if (IsUpdateRequired)
                        {
                            if (str.EndsWith(","))
                                str = str.Substring(0, str.Length - 1);
                            //str = str + " are required to become AEPS .";
                            str = "Firstly Complete Your Full KYC";

                            var results1 = "{'status':'NOTOK','msg':'" + str + "'}";
                            var json11 = JsonConvert.DeserializeObject(results1);
                            var json111 = JsonConvert.SerializeObject(json11);
                            return Json(json111, JsonRequestBehavior.AllowGet);

                        }
                        var token = string.Empty;
                        token = getAuthToken();

                        if (string.IsNullOrWhiteSpace(token))
                        {
                            var viewresponse = new { status = false };
                            return Json(viewresponse, JsonRequestBehavior.AllowGet);
                        }

                        var client1 = new RestClient("http://api.vastbazaar.com//api/SBAAEPS/Aepsmove");
                        client1.Timeout = -1;
                        var request1 = new RestRequest(Method.POST);
                        request1.AddHeader("Type", "Balance");
                        request1.AddHeader("Authorization", "Bearer " + token);
                        IRestResponse response1 = client1.Execute(request1);
                        dynamic respchk = JsonConvert.DeserializeObject(response1.Content);
                        var stsvalue = respchk.Content.ADDINFO;
                        //   stsvalue = "SBM";
                        // Fing Pay
                        if (stsvalue == "FingPay")
                        {
                            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                            var client = new RestClient("https://fingpayap.tapits.in/fpaepsservice/api/bankdata/bank/details");
                            var request = new RestRequest(Method.GET);
                            IRestResponse response = client.Execute(request);
                            var items = JsonConvert.DeserializeObject<BankIiNoModel>(response.Content);
                            var blist = items.Data.Select(a => new SelectListItem { Text = a.BankName, Value = a.IInNo }).ToList();
                            var results = "{'status':'OK','aepsid':'" + retailerDetails.Email + "','banklist':'" + response.Content + "','From':'FINGPAY'}";
                            var json1 = JsonConvert.DeserializeObject(results);
                            var json = JsonConvert.SerializeObject(json1);
                            return Json(json, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            var email = db.Whitelabel_Retailer_Details.Where(aa => aa.RetailerId == userid).SingleOrDefault().Email;
                            // SBA Bank List

                            var client = new RestClient("http://api.vastbazaar.com/api/AEPS/BankDetails");
                            client.Timeout = -1;
                            var request = new RestRequest(Method.POST);
                            request.AddHeader("imei", "1234567890123456");
                            request.AddHeader("UserEmail", email);
                            request.AddHeader("Authorization", "Bearer " + token);
                            IRestResponse response = client.Execute(request);
                            var results = "{'status':'OK','aepsid':'" + retailerDetails.Email + "','banklist':'" + response.Content + "','From':'SBA'}";
                            var json1 = JsonConvert.DeserializeObject(results);
                            var json = JsonConvert.SerializeObject(json1);
                            return Json(json, JsonRequestBehavior.AllowGet);

                        }

                    }
                    // Instant Pay
                    //var CheckOutletStatus = db.RetailerOutlets.Where(a => a.RetailerId == userid).FirstOrDefault();
                    //if (CheckOutletStatus != null)
                    //{
                    //    if (CheckOutletStatus.outlet_status)
                    //    {
                    //        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                    //        var client = new RestClient("https://fingpayap.tapits.in/fpaepsservice/api/bankdata/bank/details");
                    //        var request = new RestRequest(Method.GET);
                    //        IRestResponse response = client.Execute(request);
                    //        var items = JsonConvert.DeserializeObject<BankIiNoModel>(response.Content);
                    //        var blist = items.Data.Select(a => new SelectListItem { Text = a.BankName, Value = a.IInNo }).ToList();
                    //        //  ViewBag.BankList = items.Data.Select(a => new SelectListItem { Text = a.BankName, Value = a.IInNo }).ToList();
                    //        var results = "{'status':'OK','aepsid':'" + retailerDetails.Email + "','banklist':'" + response.Content + "'}";
                    //        var json1 = JsonConvert.DeserializeObject(results);
                    //        var json = JsonConvert.SerializeObject(json1);
                    //        return Json(json, JsonRequestBehavior.AllowGet);
                    //    }
                    //    else
                    //    {
                    //        var results = "{'status':'ERR','msg':'You AEPS InActive'}";
                    //        var json1 = JsonConvert.DeserializeObject(results);
                    //        var json = JsonConvert.SerializeObject(json1);
                    //        return Json(json, JsonRequestBehavior.AllowGet);
                    //    }

                    //}
                    else
                    {
                        var results = "{'status':'ERR','msg':''}";
                        var json1 = JsonConvert.DeserializeObject(results);
                        var json = JsonConvert.SerializeObject(json1);
                        return Json(json, JsonRequestBehavior.AllowGet);
                    }
                    /**********************************/
                }
                else
                {
                    var results = "{'status':'" + check + "','msg':'" + errormsg + "'}";
                    var json1 = JsonConvert.DeserializeObject(results);
                    var json = JsonConvert.SerializeObject(json1);
                    return Json(json, JsonRequestBehavior.AllowGet);

                }

            }
            else
            {
                var results = "{'status':'NOTOK','msg':'Your Aeps Status Inactive Please Contact Distributor OR Customer Care'}";
                var json1 = JsonConvert.DeserializeObject(results);
                var json = JsonConvert.SerializeObject(json1);
                return Json(json, JsonRequestBehavior.AllowGet);

            }


        }

        [HttpPost]
        public ActionResult AEPS(string mobile, string uid, string bank, long iin, string cap, string type, string tabvalue, int? amount, string remark, string DeviceSrNo, decimal servicefee, string usernm, string devicenm, string userotp, string pidata)
        {
            try
            {
                var userid = User.Identity.GetUserId();

                string chkamounts = "OK";
                var counts = db.AEPS_Count(uid).SingleOrDefault().msg;
                int msgcount = Convert.ToInt32(counts);
                if (msgcount == 0)
                {
                    msgcount = 11;
                }
                int max_limit = 10;
                if (msgcount > max_limit)
                {
                    if (amount <= 10000)
                    {
                        if (amount >= 100 || type == "BE" || type == "SAP")
                        {
                            if (type == "M")
                            {
                                if (amount >= 5000)
                                {
                                    chkamounts = "NOTOK";
                                    var otpchkretailer = db.MobileOtps.Where(aa => aa.Userid == userid && aa.Type == "AaDHARPayConfirmation" && aa.mobileno == mobile).OrderByDescending(aa => aa.Date).FirstOrDefault().Otp;
                                    if (otpchkretailer == userotp)
                                    {
                                        chkamounts = "OK";
                                    }

                                }
                            }
                            if (chkamounts == "OK")
                            {
                                if (!string.IsNullOrWhiteSpace(mobile) && Regex.IsMatch(mobile, @"[6|7|8|9][0-9]{9}") && !string.IsNullOrWhiteSpace(uid) && Regex.IsMatch(uid, @"[0-9]{12}") && iin != 0 && !string.IsNullOrWhiteSpace(cap) && !string.IsNullOrWhiteSpace(type))
                                {
                                    var chkkkk = JsonConvert.DeserializeObject<CaptureResponse>(cap);
                                    if (chkkkk.Piddata != null)
                                    {

                                        var logo = "";
                                        var retailer = db.Whitelabel_Retailer_Details.SingleOrDefault(a => a.RetailerId == userid);
                                        var logoimg = db.tblHeaderLogoes.Where(aa => aa.Role == "ADMIN").SingleOrDefault();
                                        if (logoimg != null)
                                        {
                                            logo = logoimg.LogoImage;
                                        }
                                        var reminfo = new
                                        {
                                            logo = logo,
                                            firmname = retailer.Frm_Name,
                                            taxenable = retailer.gststatus
                                        };
                                        var ouletid = retailer.AepsMerchandId;
                                        var aepspin = retailer.AepsMPIN;
                                        string lattitude = string.Empty;
                                        string longitude = string.Empty;
                                        //insertGeoLocation(retailer.RetailerId, out lattitude, out longitude);

                                        if (retailer.Whitelabel_UserLocation == null)
                                        {
                                            insertGeoLocation(retailer.RetailerId, out lattitude, out longitude);

                                        }
                                        else
                                        {
                                            lattitude = retailer.Whitelabel_UserLocation.Lattitude;
                                            longitude = retailer.Whitelabel_UserLocation.Longitute;
                                        }
                                        if (string.IsNullOrWhiteSpace(lattitude) || string.IsNullOrWhiteSpace(longitude))
                                        {
                                            var viewresponse = new { Status = "Failed", Message = "We are unable to find you location.", userinfo = reminfo };
                                            return Json(viewresponse, JsonRequestBehavior.AllowGet);
                                        }
                                        var distname = db.District_Desc.Where(aa => aa.State_id == retailer.State && aa.Dist_id == retailer.District).SingleOrDefault().Dist_Desc;

                                        var token = string.Empty;
                                        token = getAuthToken();
                                        if (string.IsNullOrWhiteSpace(token))
                                        {
                                            var viewresponse = new { Status = "Failed", Message = "Failed at provider server.", userinfo = reminfo };
                                            return Json(viewresponse, JsonRequestBehavior.AllowGet);
                                        }
                                        var clientnew = new RestClient("http://api.vastbazaar.com//api/SBAAEPS/Aepsmove");
                                        clientnew.Timeout = -1;
                                        var requestnew = new RestRequest(Method.POST);
                                        requestnew.AddHeader("Type", "Balance");
                                        requestnew.AddHeader("Authorization", "Bearer " + token);
                                        IRestResponse responsenew = clientnew.Execute(requestnew);
                                        dynamic respchknew = JsonConvert.DeserializeObject(responsenew.Content);
                                        var stschk = respchknew.Content.ADDINFO;
                                        // stschk = "SBM";
                                        if (stschk == "FingPay")
                                        {
                                            if (ouletid == null || ouletid == "")
                                            {
                                                var reque = new
                                                {
                                                    merchantName = retailer.RetailerName,
                                                    stateid = retailer.State,
                                                    latitude = lattitude,
                                                    longitude = longitude,
                                                    merchantPhoneNumber = retailer.Mobile,
                                                    merchantPinCode = retailer.Pincode,
                                                    merchantCityName = distname,
                                                    merchantAddress = retailer.Address,
                                                    userPan = retailer.PanCard,
                                                    retilerid = retailer.Email,
                                                    OTP = ""
                                                };
                                                var resquestchk = JsonConvert.SerializeObject(reque);
                                                var client = new RestClient("http://api.vastbazaar.com/api/AEPS/RegisterAEPS");
                                                client.Timeout = -1;
                                                var request = new RestRequest(Method.POST);
                                                request.AddHeader("Authorization", "Bearer " + token);
                                                request.AddHeader("Content-Type", "application/json");

                                                request.AddParameter("application/json", resquestchk, ParameterType.RequestBody);
                                                IRestResponse response = client.Execute(request);
                                                dynamic resp = JsonConvert.DeserializeObject(response.Content);
                                                var stscode = resp.Content.ADDINFO.statuscode.ToString();
                                                var message = resp.Content.ADDINFO.status.ToString();
                                                if (stscode == "TXN")
                                                {
                                                    ouletid = resp.Content.ADDINFO.data.outlet_id.ToString();
                                                    var pin = resp.Content.ADDINFO.data.pin.ToString();
                                                    retailer.AepsMerchandId = ouletid;
                                                    retailer.AepsMPIN = pin;
                                                    db.SaveChanges();
                                                    retailer = db.Whitelabel_Retailer_Details.SingleOrDefault(a => a.RetailerId == userid);
                                                }
                                                else
                                                {
                                                    var viewresponse = new { Status = "Failed", Message = message, userinfo = reminfo };
                                                    return Json(viewresponse, JsonRequestBehavior.AllowGet);
                                                }
                                            }
                                            if (ouletid != null)
                                            {
                                                var client1 = new RestClient("http://api.vastbazaar.com/api/AEPS/supermerchant?merchant=" + retailer.AepsMerchandId + "");
                                                var request1 = new RestRequest(Method.POST);
                                                request1.AddHeader("Authorization", "Bearer " + token);
                                                IRestResponse response1 = client1.Execute(request1);
                                                var respsuper = response1.Content;
                                                dynamic chkresp = JsonConvert.DeserializeObject(respsuper);
                                                var stscode = chkresp.Content.ADDINFO.stscode.ToString();
                                                if (stscode == "0")
                                                {
                                                    var msg = chkresp.Content.ADDINFO.message.ToString();
                                                    var viewresponse = new { Status = "Failed", Message = msg, userinfo = reminfo };
                                                    return Json(viewresponse, JsonRequestBehavior.AllowGet);
                                                }
                                                var superid = chkresp.Content.ADDINFO.data.superid.ToString();
                                                var superusername = chkresp.Content.ADDINFO.data.superusername.ToString();

                                                if (string.IsNullOrWhiteSpace(lattitude) || string.IsNullOrWhiteSpace(longitude))
                                                {
                                                    var msg = chkresp.Content.ADDINFO.message.ToString();
                                                    var viewresponse = new { Status = "Failed", Message = "We are anable to find you location.", userinfo = reminfo };
                                                    return Json(viewresponse, JsonRequestBehavior.AllowGet);
                                                }
                                                Main obj = new Main();
                                                AepsModel reqObject = new AepsModel();

                                                reqObject.captureResponse = JsonConvert.DeserializeObject<CaptureResponse>(cap);
                                                reqObject.cardnumberORUID = new CardnumberOruid { adhaarNumber = uid, nationalBankIdentificationNumber = iin };
                                                //var address = "75 Ninth Avenue 2nd and 4th Floors New York, NY 10011";
                                                //var locationService = new GoogleLocationService();
                                                //var point = locationService.GetLatLongFromAddress(retailer.Address);
                                                reqObject.latitude = Convert.ToDouble(lattitude);//27.617470;// point.Latitude;
                                                reqObject.longitude = Convert.ToDouble(longitude);//75.144400;

                                                // string ProjectName = Path.GetDirectoryName(Path.GetDirectoryName(Server.MapPath(@"~/HomeControllers.cs")));
                                                // ProjectName = ProjectName.Substring(ProjectName.LastIndexOf("\\") + 1);
                                                var admin_details = db.Admin_details.SingleOrDefault();
                                                var infoadmin = admin_details.WebsiteUrl.Replace(".", "");
                                                var agentid = infoadmin + "" + DateTime.Now.ToString("dd-MM-yyy hh-mm-ss").Replace("-", "").Replace(" ", "");
                                                reqObject.merchantTransactionId = agentid;
                                                reqObject.merchantTranId = infoadmin + DateTime.Now.ToString("dd-MM-yyy hh-mm-ss").Replace("-", "").Replace(" ", "");
                                                reqObject.mobileNumber = mobile;
                                                reqObject.requestRemarks = "";
                                                reqObject.superMerchantId = superid;
                                                reqObject.merchantUserName = retailer.AepsMerchandId;//"vastwebm";// "9214711111";//"excelonestopd";// "sai";
                                                reqObject.merchantPin = CreateMD5(retailer.AepsMPIN);//"b027291b0af8cde6ae6e30bf6056204b";// "81dc9bdb52d04dc20036dbd8313ed055";//"81DC9BDB52D04DC20036DBD8313ED055";
                                                                                                     //      var rootDir = Path.GetDirectoryName(Path.GetDirectoryName(Server.MapPath(@"~/HomeControllers.cs")));
                                                                                                     //      rootDir = rootDir.Substring(rootDir.LastIndexOf("\\") + 1);
                                                                                                     //   reqObject.subMerchantId = rootDir; //"skmoney";//"excelonestopm"//TODO

                                                reqObject.timestamp = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);//"dd/MM/yyyy HH:mm:ss"
                                                reqObject.transactionType = type;
                                                if (type == "CW" || type == "M") //DYNAMICALY ADD AMOUNT FIELD IF CASH WITHDRA
                                                {
                                                    if (amount != null && amount > 0)
                                                    {
                                                        reqObject.transactionAmount = (int)amount;
                                                    }
                                                    else
                                                    {
                                                        var msg = chkresp.Content.ADDINFO.message.ToString();
                                                        var viewresponse = new { Status = "Failed", Message = "Invalid Amount.", userinfo = reminfo };
                                                        return Json(viewresponse, JsonRequestBehavior.AllowGet);
                                                    }
                                                }
                                                else if (type == "SAP")
                                                {
                                                    // reqObject.transactionAmount = 10;
                                                    reqObject.transactionType = "MS";
                                                    System.Data.Entity.Core.Objects.ObjectParameter output = new System.Data.Entity.Core.Objects.ObjectParameter("output", typeof(string));
                                                    string fee = servicefee.ToString();
                                                    usernm = usernm + "__" + devicenm + "__" + DeviceSrNo;
                                                    var msg = db.Whitelabel_insert_Aeps_mini_statement(userid, uid, bank, agentid, "Web", mobile, fee, usernm, output).SingleOrDefault().msg;

                                                    if (msg != "DONE")
                                                    {
                                                        var viewresponse = new { Status = "Failed", Message = msg, userinfo = reminfo };
                                                        return Json(viewresponse, JsonRequestBehavior.AllowGet);
                                                    }
                                                }
                                                dynamic RequestJson = JsonConvert.SerializeObject(reqObject);

                                                byte[] hash = Main.generateSha256Hash(Encoding.ASCII.GetBytes(RequestJson));
                                                byte[] skey = Main.generateSessionKey();
                                                string encryptUsingSessionKey = Main.encryptUsingSessionKey(skey, Encoding.ASCII.GetBytes(RequestJson));
                                                string encryptUsingPublicKey = Main.encryptUsingPublicKey(skey);
                                                if (string.IsNullOrWhiteSpace(encryptUsingSessionKey) || string.IsNullOrWhiteSpace(encryptUsingPublicKey))
                                                {
                                                    var viewresponse = new { Status = "Failed", Message = "Failed to initiate request.", userinfo = reminfo };
                                                    return Json(viewresponse, JsonRequestBehavior.AllowGet);
                                                }
                                                var client = new RestClient();
                                                var request = new RestRequest(Method.POST);
                                                if (type == "CW")
                                                {
                                                    //tbl_Holiday
                                                    client = new RestClient(VastbazaarBaseUrl + "api/AEPS/cashWithdrawPost");
                                                    string req = RequestJson.ToString();
                                                    System.Data.Entity.Core.Objects.ObjectParameter status = new System.Data.Entity.Core.Objects.ObjectParameter("Status", typeof(bool));
                                                    System.Data.Entity.Core.Objects.ObjectParameter output = new System.Data.Entity.Core.Objects.ObjectParameter("Output", typeof(string));
                                                    usernm = usernm + "__" + devicenm + "__" + DeviceSrNo;
                                                    var dbRespo = db.Whitelabel_proc_AEPS_PreProcess(userid, agentid, uid, mobile, bank, remark ?? "TODO", amount, "web", req, "", status, "", servicefee, usernm, "Cash Widthdraw", output).SingleOrDefault();
                                                    if (dbRespo.Status == false)
                                                    {
                                                        var viewresponse = new { Status = "Failed", Message = dbRespo.Output, userinfo = reminfo };
                                                        return Json(viewresponse, JsonRequestBehavior.AllowGet);
                                                    }
                                                }
                                                else if (type == "SAP")
                                                {
                                                    client = new RestClient(VastbazaarBaseUrl + "api/AEPS/MiniStateMent");
                                                }
                                                else if (type == "BE")
                                                {
                                                    client = new RestClient(VastbazaarBaseUrl + "api/AEPS/balanceEnquiry");
                                                }
                                                else if (type == "CD")
                                                {
                                                    client = new RestClient(VastbazaarBaseUrl + "api/AEPS/CashDeposite");
                                                    request.AddHeader("superid", superid);
                                                }
                                                else if (type == "M")
                                                {
                                                    client = new RestClient(VastbazaarBaseUrl + "api/AEPS/cashWithdrawAadharPay");
                                                    string req = RequestJson.ToString();
                                                    System.Data.Entity.Core.Objects.ObjectParameter status = new System.Data.Entity.Core.Objects.ObjectParameter("Status", typeof(bool));
                                                    System.Data.Entity.Core.Objects.ObjectParameter output = new System.Data.Entity.Core.Objects.ObjectParameter("Output", typeof(string));
                                                    usernm = usernm + "__" + devicenm + "__" + DeviceSrNo;
                                                    var dbRespo = db.Whitelabel_proc_AEPS_PreProcess(userid, agentid, uid, mobile, bank, remark ?? "TODO", amount, "web", req, "", status, "", servicefee, usernm, "Aadhar Pay", output).SingleOrDefault();
                                                    if (dbRespo.Status == false)
                                                    {
                                                        var viewresponse = new { Status = "Failed", Message = dbRespo.Output, userinfo = reminfo };
                                                        return Json(viewresponse, JsonRequestBehavior.AllowGet);
                                                    }
                                                }

                                                var ammt = amount.ToString();

                                                //request.RequestFormat = DataFormat.Json;
                                                request.AddHeader("content-type", "text/plain");
                                                request.AddHeader("cache-control", "no-cache");
                                                request.AddHeader("authorization", "bearer " + token);//OAUTH token
                                                request.AddHeader("eskey", encryptUsingPublicKey);
                                                request.AddHeader("deviceIMEI", string.IsNullOrWhiteSpace(DeviceSrNo) ? "352801082418919" : DeviceSrNo); //can pass Unique device Id
                                                request.AddHeader("hash", Convert.ToBase64String(hash));
                                                request.AddHeader("trnTimestamp", reqObject.timestamp);
                                                request.AddHeader("agentid", agentid);
                                                request.AddHeader("amount", ammt);
                                                request.AddHeader("MerchantId", ouletid);
                                                request.AddHeader("aadhar", uid);
                                                request.AddHeader("devicesrno", DeviceSrNo);
                                                request.AddHeader("devicenm", devicenm);
                                                request.AddParameter("text/plain",
                                                    encryptUsingSessionKey, ParameterType.RequestBody);
                                                IRestResponse response = client.Execute(request);


                                                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                                                {
                                                    TryLogin();
                                                }
                                                var respoJson = JsonConvert.DeserializeObject<AepsResponseJson>(response.Content);
                                                //dynamic respoJson = JsonConvert.DeserializeObject("{\"Version\":\"1.0\",\"StatusCode\":200,\"Content\":{\"ResponseCode\":0,\"ADDINFO\":{\"status\":true,\"message\":\"Request Completed\",\"data\":{\"terminalId\":\"FA026069\",\"requestTransactionTime\":\"26\/09\/2018 10:26:58\",\"transactionAmount\":0.0,\"transactionStatus\":\"successful\",\"balanceAmount\":4408.83,\"bankRRN\":\"826910115647\",\"transactionType\":\"BE\",\"fpTransactionId\":\"826910115647\"},\"statusCode\":10000}}}");
                                                //if (response.StatusCode != HttpStatusCode.OK || string.IsNullOrWhiteSpace(response.Content))
                                                //{
                                                //    if (type == "CW")
                                                //    {
                                                //        System.Data.Entity.Core.Objects.ObjectParameter procStatus = new System.Data.Entity.Core.Objects.ObjectParameter("ProcStatus", typeof(string));
                                                //        System.Data.Entity.Core.Objects.ObjectParameter procMessage = new System.Data.Entity.Core.Objects.ObjectParameter("ProcMessage", typeof(string));
                                                //        db.whitelabel_proc_AEPS_PostProcess(userid,reqObject.merchantTransactionId, "NA", null, "Failed", response.Content ?? "", "web","Other than status code OK",procStatus,procMessage);
                                                //    }
                                                //    var aa = new { Status =
                                                //
                                                //    "Failed", Message = "Failed from bank." };
                                                //    TempData["Respo"] = JsonConvert.SerializeObject(aa);
                                                //    return RedirectToAction("AEPS");
                                                //}
                                                //else
                                                //{
                                                try
                                                {
                                                    var bal = ""; string rrn = "";
                                                    if (respoJson.Content.ADDINFO.StatusCode == 10000)
                                                    {
                                                        var respchk = (dynamic)null;
                                                        var date = DateTime.Now.ToString();
                                                        if (type == "BE")
                                                        {
                                                            rrn = Convert.ToString(respoJson.Content.ADDINFO.Data.BankRrn);
                                                            bal = Convert.ToString(respoJson.Content.ADDINFO.Data.BalanceAmount);
                                                            respchk = new
                                                            {
                                                                RequestTransactionTime = date,
                                                                BankRrn = rrn,
                                                                TransactionAmount = "0",
                                                                TransactionStatus = "Success",
                                                                BalanceAmount = bal,
                                                                Bank = bank
                                                            };
                                                            try
                                                            {
                                                                decimal rembal1 = Convert.ToDecimal(bal);
                                                                usernm = usernm + "__" + devicenm + "__" + DeviceSrNo;
                                                                db.Whitelabel_Aeps_check_balance(userid, agentid, uid, bank, rrn, remark, "Web", mobile, "", usernm, rembal1);
                                                            }
                                                            catch { }
                                                            var dd = new { Status = "Success", Message = respchk, userinfo = reminfo };
                                                            //TempData["Respo"] = JsonConvert.SerializeObject(dd);
                                                            //return RedirectToAction("AEPS");
                                                            return Json(dd, JsonRequestBehavior.AllowGet);
                                                        }
                                                        else if (type == "CW")
                                                        {
                                                            string terminalid = Convert.ToString(respoJson.Content.ADDINFO.Data.TerminalId);
                                                            rrn = Convert.ToString(respoJson.Content.ADDINFO.Data.BankRrn);
                                                            bal = Convert.ToString(respoJson.Content.ADDINFO.Data.BalanceAmount);
                                                            respchk = new
                                                            {
                                                                BankRrn = rrn,
                                                                BalanceAmount = bal,
                                                                RequestTransactionTime = date,
                                                                TransactionAmount = amount,
                                                                TransactionStatus = "Success",
                                                                Bank = bank
                                                            };
                                                            System.Data.Entity.Core.Objects.ObjectParameter procStatus = new System.Data.Entity.Core.Objects.ObjectParameter("ProcStatus", typeof(string));
                                                            System.Data.Entity.Core.Objects.ObjectParameter procMessage = new System.Data.Entity.Core.Objects.ObjectParameter("ProcMessage", typeof(string));
                                                            decimal remain = 0;
                                                            try
                                                            {
                                                                remain = Convert.ToDecimal(bal);
                                                            }
                                                            catch { }
                                                            db.proc_whitelabel_AEPS_PostProcess(userid, reqObject.merchantTransactionId, rrn, terminalid, "Success", response.Content, "web", "", remain, procStatus, procMessage);
                                                            var dd = new { Status = "Success", Message = respchk, userinfo = reminfo };
                                                            return Json(dd, JsonRequestBehavior.AllowGet);
                                                        }
                                                        else if (type == "M")
                                                        {
                                                            string terminalid = Convert.ToString(respoJson.Content.ADDINFO.Data.TerminalId);
                                                            rrn = Convert.ToString(respoJson.Content.ADDINFO.Data.BankRrn);
                                                            bal = Convert.ToString(respoJson.Content.ADDINFO.Data.BalanceAmount);
                                                            respchk = new
                                                            {
                                                                BankRrn = rrn,
                                                                BalanceAmount = bal,
                                                                RequestTransactionTime = date,
                                                                TransactionAmount = amount,
                                                                TransactionStatus = "Success",
                                                                Bank = bank
                                                            };
                                                            System.Data.Entity.Core.Objects.ObjectParameter procStatus = new System.Data.Entity.Core.Objects.ObjectParameter("ProcStatus", typeof(string));
                                                            System.Data.Entity.Core.Objects.ObjectParameter procMessage = new System.Data.Entity.Core.Objects.ObjectParameter("ProcMessage", typeof(string));
                                                            decimal remain = 0;
                                                            try
                                                            {
                                                                remain = Convert.ToDecimal(bal);
                                                            }
                                                            catch { }
                                                            db.spWhitelabel_AEPS_Aadharpay_PostProcess(userid, reqObject.merchantTransactionId, rrn, terminalid, "Success", response.Content, "web", "", remain, procStatus, procMessage);
                                                            var dd = new { Status = "Success", Message = respchk, userinfo = reminfo };
                                                            return Json(dd, JsonRequestBehavior.AllowGet);
                                                        }
                                                        else if (type == "SAP")
                                                        {
                                                            rrn = Convert.ToString(respoJson.Content.ADDINFO.Data.BankRrn);
                                                            bal = Convert.ToString(respoJson.Content.ADDINFO.Data.BalanceAmount);
                                                            respchk = new
                                                            {
                                                                RequestTransactionTime = date,
                                                                BankRrn = rrn,
                                                                TransactionAmount = "0",
                                                                TransactionStatus = "Success",
                                                                BalanceAmount = bal,
                                                                Bank = bank
                                                            };
                                                            try
                                                            {
                                                                decimal rembal1 = 0;
                                                                try
                                                                {
                                                                    rembal1 = Convert.ToDecimal(bal);
                                                                }
                                                                catch { }
                                                                db.update_Aeps_miniStatement(agentid, "M_Success", rrn, rembal1);
                                                            }
                                                            catch { }

                                                            dynamic respchk1 = JsonConvert.DeserializeObject(response.Content);
                                                            var serdata = JsonConvert.SerializeObject(respchk1.Content.ADDINFO.data);
                                                            var dd = new { Status = "Success", Message = serdata, userinfo = reminfo };
                                                            //TempData["Respo"] = JsonConvert.SerializeObject(dd);
                                                            //return RedirectToAction("AEPS");
                                                            return Json(dd, JsonRequestBehavior.AllowGet);
                                                        }
                                                        else
                                                        {
                                                            var dd = new { Status = "Failed", Message = "Please Try Again.", userinfo = reminfo };
                                                            return Json(dd, JsonRequestBehavior.AllowGet);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        decimal remain = 0;
                                                        try
                                                        {
                                                            remain = Convert.ToDecimal(bal);
                                                        }
                                                        catch { }
                                                        if (type == "CW")
                                                        {
                                                            var msg = respoJson.Content.ADDINFO.Message.ToString();
                                                            System.Data.Entity.Core.Objects.ObjectParameter procStatus = new System.Data.Entity.Core.Objects.ObjectParameter("ProcStatus", typeof(string));
                                                            System.Data.Entity.Core.Objects.ObjectParameter procMessage = new System.Data.Entity.Core.Objects.ObjectParameter("ProcMessage", typeof(string));
                                                            db.proc_whitelabel_AEPS_PostProcess(userid, reqObject.merchantTransactionId, msg, null, "Failed", response.Content, "web", msg, remain, procStatus, procMessage);
                                                            //  db.whitelabel_proc_AEPS_PostProcess(userid, reqObject.merchantTransactionId, msg, null, "Failed", response.Content, "web", msg, remain, procStatus, procMessage);
                                                        }
                                                        else if (type == "M")
                                                        {
                                                            var msg = respoJson.Content.ADDINFO.Message.ToString();
                                                            System.Data.Entity.Core.Objects.ObjectParameter procStatus = new System.Data.Entity.Core.Objects.ObjectParameter("ProcStatus", typeof(string));
                                                            System.Data.Entity.Core.Objects.ObjectParameter procMessage = new System.Data.Entity.Core.Objects.ObjectParameter("ProcMessage", typeof(string));
                                                            db.spWhitelabel_AEPS_Aadharpay_PostProcess(userid, reqObject.merchantTransactionId, msg, null, "Failed", response.Content, "web", msg, remain, procStatus, procMessage);
                                                            //   db.proc_AEPS_Aadharpay_PostProcess(userid, reqObject.merchantTransactionId, msg, null, "Failed", response.Content, "web", msg, remain, procStatus, procMessage);

                                                        }
                                                        else if (type == "SAP")
                                                        {
                                                            var msg = respoJson.Content.ADDINFO.Message.ToString();
                                                            try
                                                            {
                                                                decimal rembal1 = Convert.ToDecimal(0);
                                                                db.update_Aeps_miniStatement(agentid, "M_Failed", msg, rembal1);
                                                            }
                                                            catch { }
                                                            var dd = new { Status = "Failed", Message = msg, userinfo = reminfo };
                                                            //TempData["Respo"] = JsonConvert.SerializeObject(dd);
                                                            //return RedirectToAction("AEPS
                                                            return Json(dd, JsonRequestBehavior.AllowGet);
                                                        }
                                                        var viewresponse = new { Status = "Failed", Message = respoJson.Content.ADDINFO.Message, userinfo = reminfo };
                                                        return Json(viewresponse, JsonRequestBehavior.AllowGet);
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    var viewresponse = new { Status = "Failed", Message = "Something went wrong. Kindly check transaction status from AEPS report.", userinfo = reminfo };
                                                    return Json(viewresponse, JsonRequestBehavior.AllowGet);
                                                }
                                                //}
                                            }
                                            else
                                            {
                                                var viewresponse = new { Status = "Failed", Message = "Complete Your Profile", userinfo = reminfo };
                                                return Json(viewresponse, JsonRequestBehavior.AllowGet);
                                            }
                                        }
                                        else
                                        {
                                            if (string.IsNullOrWhiteSpace(lattitude) || string.IsNullOrWhiteSpace(longitude))
                                            {
                                                var viewresponse1 = new { Status = "Failed", Message = "We are anable to find you location.", userinfo = reminfo };
                                                return Json(viewresponse1, JsonRequestBehavior.AllowGet);
                                            }
                                            pidata = Uri.UnescapeDataString(pidata);
                                            var cappp = JsonConvert.DeserializeObject<CaptureResponse>(cap);
                                            token = getAuthToken();
                                            if (string.IsNullOrWhiteSpace(token))
                                            {
                                                var viewresponse1 = new { Status = "Failed", Message = "Token related issue . Please Try After Time", userinfo = reminfo };
                                                return Json(viewresponse1, JsonRequestBehavior.AllowGet);
                                            }
                                            var today = DateTime.Now;
                                            string year = today.Year.ToString();
                                            year = year.Replace("202", "");
                                            string month = today.Month.ToString();
                                            var myDate = new DateTime(today.Year, today.Month, today.Day);
                                            var julian = myDate.DayOfYear;
                                            string julian1 = julian.ToString().Substring(0, 3);
                                            string hours = today.Hour.ToString();
                                            Random rnd = new Random();
                                            int random = rnd.Next(111111, 999999);
                                            var refid = year + julian1 + hours + random;
                                            if (type == "BE")
                                            {
                                                var client = new RestClient("http://api.vastbazaar.com/api/SBAAEPS/Transaction");
                                                client.Timeout = -1;
                                                var request = new RestRequest(Method.POST);
                                                request.AddHeader("imei", "6465464654");
                                                request.AddHeader("txType", "BE");
                                                request.AddHeader("amount", "10");
                                                request.AddHeader("aadharNo", "0" + uid);
                                                request.AddHeader("bankCode", iin.ToString());
                                                request.AddHeader("location", lattitude + "|" + longitude);
                                                request.AddHeader("Uniqueid", refid);
                                                request.AddHeader("data", cap);
                                                request.AddHeader("pidData", pidata);
                                                request.AddHeader("devicenm", devicenm);
                                                request.AddHeader("devicesrno", DeviceSrNo);
                                                request.AddHeader("Authorization", "Bearer " + token);
                                                IRestResponse response = client.Execute(request);
                                                dynamic respchk = JsonConvert.DeserializeObject(response.Content);
                                                string sts = respchk.Content.ADDINFO.status.ToString();
                                                string msg = respchk.Content.ADDINFO.msg.ToString();
                                                if (sts.ToUpper() == "TRUE")
                                                {
                                                    string respcode = respchk.Content.ADDINFO.respcode.ToString();
                                                    if (respcode == "00")
                                                    {
                                                        string bankRefNo = respchk.Content.ADDINFO.aepsTransactionSbm.bankRefNo;
                                                        string bal = respchk.Content.ADDINFO.balance;
                                                        string[] ballist = bal.Split('#');
                                                        string remain = ballist[1].ToString();
                                                        var respchk1 = (dynamic)null;
                                                        var date = DateTime.Now.ToString();
                                                        respchk1 = new
                                                        {
                                                            RequestTransactionTime = date,
                                                            BankRrn = bankRefNo,
                                                            TransactionAmount = "0",
                                                            TransactionStatus = "Success",
                                                            BalanceAmount = remain,
                                                            Bank = bank
                                                        };
                                                        try
                                                        {
                                                            decimal rembal1 = Convert.ToDecimal(remain);
                                                            usernm = usernm + "__" + devicenm + "__" + DeviceSrNo;
                                                            db.Whitelabel_Aeps_check_balance(userid, refid, uid, bank, bankRefNo, remark, "Web", mobile, "", usernm, rembal1);
                                                        }
                                                        catch { }
                                                        var dd = new { Status = "Success", Message = new { BankRrn = bankRefNo, BalanceAmount = remain }, userinfo = reminfo };
                                                        //TempData["Respo"] = JsonConvert.SerializeObject(dd);
                                                        //return RedirectToAction("AEPS");
                                                        return Json(dd, JsonRequestBehavior.AllowGet);
                                                    }
                                                    else
                                                    {
                                                        var viewresponse = new { Status = "Failed", Message = msg, userinfo = reminfo };
                                                        return Json(viewresponse, JsonRequestBehavior.AllowGet);
                                                    }
                                                }
                                                else
                                                {
                                                    var viewresponse = new { Status = "Failed", Message = msg, userinfo = reminfo };
                                                    return Json(viewresponse, JsonRequestBehavior.AllowGet);
                                                }

                                            }
                                            else if (type == "CW")
                                            {
                                                string amt = amount.ToString();
                                                var client = new RestClient("http://api.vastbazaar.com/api/SBAAEPS/Transaction");
                                                client.Timeout = -1;
                                                var request = new RestRequest(Method.POST);
                                                request.AddHeader("imei", "6465464654");
                                                request.AddHeader("txType", "CW");
                                                request.AddHeader("amount", amt);
                                                request.AddHeader("aadharNo", "0" + uid);
                                                request.AddHeader("bankCode", iin.ToString());
                                                request.AddHeader("location", lattitude + "|" + longitude);
                                                request.AddHeader("Uniqueid", refid);
                                                request.AddHeader("data", cap);
                                                request.AddHeader("pidData", pidata);
                                                request.AddHeader("devicenm", devicenm);
                                                request.AddHeader("devicesrno", DeviceSrNo);
                                                request.AddHeader("Authorization", "Bearer " + token);
                                                var RequestJson = JsonConvert.SerializeObject(request);
                                                string req = RequestJson.ToString();
                                                System.Data.Entity.Core.Objects.ObjectParameter status = new System.Data.Entity.Core.Objects.ObjectParameter("Status", typeof(bool));
                                                System.Data.Entity.Core.Objects.ObjectParameter output = new System.Data.Entity.Core.Objects.ObjectParameter("Output", typeof(string));
                                                usernm = usernm + "__" + devicenm + "__" + DeviceSrNo;
                                                var dbRespo = db.Whitelabel_proc_AEPS_PreProcess(userid, refid, uid, mobile, bank, remark ?? "TODO", amount, "web", req, "", status, "", servicefee, usernm, "Cash Widthdraw", output).SingleOrDefault();
                                                //var dbRespo = db.proc_AEPS_PreProcess(userid, refid, uid, mobile, bank, remark ?? "TODO", amount, "web", req, "", status, "", servicefee, usernm, "Cash Widthdraw", output).SingleOrDefault();
                                                if (dbRespo.Status == false)
                                                {
                                                    var viewresponse = new { Status = "Failed", Message = dbRespo.Output, userinfo = reminfo };
                                                    return Json(viewresponse, JsonRequestBehavior.AllowGet);
                                                }
                                                else if (dbRespo.Status == true)
                                                {
                                                    IRestResponse response = client.Execute(request);
                                                    dynamic respchk = JsonConvert.DeserializeObject(response.Content);
                                                    string sts = respchk.Content.ADDINFO.status.ToString();
                                                    string msg = respchk.Content.ADDINFO.msg.ToString();
                                                    if (sts.ToUpper() == "TRUE")
                                                    {
                                                        string respcode = respchk.Content.ADDINFO.respcode.ToString();
                                                        if (respcode == "00")
                                                        {
                                                            string bankRefNo = respchk.Content.ADDINFO.aepsTransactionSbm.bankRefNo;
                                                            string bal = respchk.Content.ADDINFO.balance;
                                                            string[] ballist = bal.Split('#');
                                                            string remain = ballist[1].ToString();
                                                            var respchk1 = (dynamic)null;
                                                            var date = DateTime.Now.ToString();
                                                            respchk1 = new
                                                            {
                                                                RequestTransactionTime = date,
                                                                BankRrn = bankRefNo,
                                                                TransactionAmount = "0",
                                                                TransactionStatus = "Success",
                                                                BalanceAmount = remain,
                                                                Bank = bank
                                                            };

                                                            decimal rembal1 = Convert.ToDecimal(remain);
                                                            usernm = usernm + "__" + devicenm + "__" + DeviceSrNo;
                                                            System.Data.Entity.Core.Objects.ObjectParameter procStatus = new System.Data.Entity.Core.Objects.ObjectParameter("ProcStatus", typeof(string));
                                                            System.Data.Entity.Core.Objects.ObjectParameter procMessage = new System.Data.Entity.Core.Objects.ObjectParameter("ProcMessage", typeof(string));
                                                            db.proc_whitelabel_AEPS_PostProcess(userid, refid, bankRefNo, "", "Success", response.Content, "web", "", rembal1, procStatus, procMessage);
                                                            var dd1 = new { Status = "Success", Message = new { BankRrn = bankRefNo, BalanceAmount = remain }, userinfo = reminfo };
                                                            return Json(dd1, JsonRequestBehavior.AllowGet);

                                                        }
                                                        else
                                                        {
                                                            System.Data.Entity.Core.Objects.ObjectParameter procStatus = new System.Data.Entity.Core.Objects.ObjectParameter("ProcStatus", typeof(string));
                                                            System.Data.Entity.Core.Objects.ObjectParameter procMessage = new System.Data.Entity.Core.Objects.ObjectParameter("ProcMessage", typeof(string));
                                                            db.proc_whitelabel_AEPS_PostProcess(userid, refid, msg, "", "Failed", response.Content, "web", "", 0, procStatus, procMessage);
                                                            //  db.whitelabel_proc_AEPS_PostProcess(userid, refid, msg, "", "Failed", response.Content, "web", "", 0, procStatus, procMessage);

                                                            var viewresponse = new { Status = "Failed", Message = msg, userinfo = reminfo };
                                                            return Json(viewresponse, JsonRequestBehavior.AllowGet);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        System.Data.Entity.Core.Objects.ObjectParameter procStatus = new System.Data.Entity.Core.Objects.ObjectParameter("ProcStatus", typeof(string));
                                                        System.Data.Entity.Core.Objects.ObjectParameter procMessage = new System.Data.Entity.Core.Objects.ObjectParameter("ProcMessage", typeof(string));
                                                        db.proc_whitelabel_AEPS_PostProcess(userid, refid, msg, "", "Failed", response.Content, "web", "", 0, procStatus, procMessage);

                                                        var viewresponse = new { Status = "Failed", Message = msg, userinfo = reminfo };
                                                        return Json(viewresponse, JsonRequestBehavior.AllowGet);
                                                    }
                                                }
                                                else
                                                {
                                                    var viewresponse = new { Status = "Pending", Message = "transtion Pending", userinfo = reminfo };
                                                    return Json(viewresponse, JsonRequestBehavior.AllowGet);
                                                }
                                            }
                                            else
                                            {
                                                var viewresponse = new { Status = "Failed", Message = "Please Try After Some Time.", userinfo = "" };
                                                return Json(viewresponse, JsonRequestBehavior.AllowGet);
                                            }

                                        }
                                    }
                                    else
                                    {
                                        var viewresponse = new { Status = "Failed", Message = "Please Scan Again.", userinfo = "" };
                                        return Json(viewresponse, JsonRequestBehavior.AllowGet);

                                    }
                                }
                                else
                                {
                                    var viewresponse = new { Status = "Failed", Message = "Either mobile or aadhaar is invalid.", userinfo = "" };
                                    return Json(viewresponse, JsonRequestBehavior.AllowGet);
                                }

                            }
                            else
                            {
                                var viewresponse = new { Status = "Failed", Message = "Wrong OTP", userinfo = "" };
                                return Json(viewresponse, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            var viewresponse = new { Status = "Failed", Message = "minimum Amount Should be 100Rs", userinfo = "" };
                            return Json(viewresponse, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        var viewresponse = new { Status = "Failed", Message = "Amount Should be less than 10000", userinfo = "" };
                        return Json(viewresponse, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    var viewresponse = new { Status = "Failed", Message = "पहले लेनदेन (सफल या असफल) के बाद 10 सेकेंड में फिर से उसी आधार कार्ड का उपयोग न करें।", userinfo = "" };
                    return Json(viewresponse, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                var viewresponse = new { Status = "Failed", Message = "Invalid request", userinfo = "" };
                return Json(viewresponse, JsonRequestBehavior.AllowGet);
            }
        }


        //[HttpPost]
        //public ActionResult AEPS(string mobile, string uid, string bank, long iin, string cap, string type, string tabvalue, int? amount, string remark, string DeviceSrNo, decimal servicefee, string usernm)
        //{
        //    try
        //    {
        //        var userid = User.Identity.GetUserId();
        //        var retailer = db.Whitelabel_Retailer_Details.SingleOrDefault(a => a.RetailerId == userid);
        //        var logo = "";
        //        var logoimg = db.tblHeaderLogoes.Where(aa => aa.Role == "ADMIN").SingleOrDefault();
        //        if (logoimg != null)
        //        {
        //            logo = logoimg.LogoImage;
        //        }
        //        var reminfo = new
        //        {
        //            logo = logo,
        //            firmname = retailer.Frm_Name,
        //            taxenable = retailer.gststatus
        //        };

        //        var captureRes = JsonConvert.DeserializeObject<CaptureResponse>(cap);
        //        var timestamp = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
        //        var deviceIMEI= string.IsNullOrWhiteSpace(DeviceSrNo) ? "352801082418919" : DeviceSrNo; //can pass Unique device Id

        //        var Token = string.Empty;
        //        Token = GetToken();

        //        var responsechk = "";
        //        var client = new RestClient();
        //        if (type == "BE")
        //        {
        //            client = new RestClient(Vastwebmulti.Areas.WHITELABEL.Models.WapiBaseUrl.GetBaseUrl() + "/api/app/AEPS/balanceEnquiry?AppType=web&IMEIfromWEB=" + deviceIMEI + "");
        //        }
        //        else if (type == "SAP")
        //        {
        //            client = new RestClient(Vastwebmulti.Areas.WHITELABEL.Models.WapiBaseUrl.GetBaseUrl() + "/api/app/AEPS/MiniStatement?AppType=web&IMEIfromWEB=" + deviceIMEI + "");                    
        //        }
        //        else if (type == "M")
        //        {
        //            client = new RestClient(Vastwebmulti.Areas.WHITELABEL.Models.WapiBaseUrl.GetBaseUrl() + "/api/app/AEPS/Aadharpay?AppType=web&IMEIfromWEB=" + deviceIMEI + "");
        //        }
        //        else if (type == "CW")
        //        {
        //            client = new RestClient(Vastwebmulti.Areas.WHITELABEL.Models.WapiBaseUrl.GetBaseUrl() + "/api/app/AEPS/cashWithdrawal?AppType=web&IMEIfromWEB=" + deviceIMEI + "");
        //        }

        //        var request = new RestRequest(Method.POST);
        //        request.AddParameter("application/json", "{\r\n  \"merchantTransactionId\": \"sample string 1\",\r\n  \"merchantTranId\": \"sample string 2\"," +
        //        "\r\n  \"captureResponse\": " + cap + ",\r\n  \"cardnumberORUID\": {\r\n    \"adhaarNumber\": \"" + uid + "\",\r\n    \"indicatorforUID\": 0," +
        //        "\r\n    \"nationalBankIdentificationNumber\": " + iin + "\r\n  },\r\n  \"languageCode\": \"en\",\r\n  \"latitude\": 3.1,\r\n  \"longitude\": 4.1," +
        //        "\r\n  \"mobileNumber\": \"" + mobile + "\",\r\n  \"paymentType\": \"B\",\r\n  \"requestRemarks\": \"" + remark + "\",\r\n  \"timestamp\": \"" + timestamp + "\"," +
        //        "\r\n  \"transactionAmount\": " + (int)amount + ",\r\n  \"transactionType\": \"" + type + "\",\r\n  \"merchantUserName\": \"sample string 10\"," +
        //        "\r\n  \"merchantPin\": \"sample string 11\",\r\n  \"subMerchantId\": \"sample string 12\",\r\n  \"superMerchantId\": \"sample string 13\"," +
        //        "\r\n  \"name\": \"" + usernm + "\",\r\n  \"Charge\": " + servicefee + ",\r\n  \"customer_remain\": " + 0 + "\r\n}", ParameterType.RequestBody);
        //        request.AddHeader("Content-Type", "application/json");
        //        request.AddHeader("Authorization", "bearer " + Token + "");
        //        IRestResponse response = client.Execute(request);
        //        responsechk = response.Content.ToString();

        //        var statusCode = response.StatusCode.ToString();
        //        if (statusCode == "Unauthorized")
        //        {
        //            TempData.Remove("data");
        //            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
        //            return RedirectToAction("Index", "Home", null);
        //        }
        //        else if (responsechk != "")
        //        {
        //            WriteLog("responsechk != " + responsechk);
        //            dynamic stuff = Newtonsoft.Json.JsonConvert.DeserializeObject(responsechk);
        //            if (stuff.RESULT.ToString() == "0")
        //            {
        //                string rrn = stuff.ADDINFO.BankRrn.ToString();
        //                var bal = stuff.ADDINFO.BalanceAmount.ToString();

        //                var respchk = (dynamic)null;
        //                respchk = new
        //                {
        //                    RequestTransactionTime = DateTime.Now.ToString(),
        //                    BankRrn = rrn,
        //                    TransactionAmount = "0",
        //                    TransactionStatus = "Success",
        //                    BalanceAmount = bal,
        //                    Bank = bank
        //                };

        //                var viewresponse = new { Status = "Success", Message = respchk, userinfo = reminfo };
        //                return Json(viewresponse, JsonRequestBehavior.AllowGet);
        //            }
        //            else
        //            {
        //                var viewresponse = new { Status = "Failed", Message = stuff.ADDINFO.ToString(), userinfo = reminfo };
        //                return Json(viewresponse, JsonRequestBehavior.AllowGet);
        //            }
        //        }
        //        else
        //        {
        //            var viewresponse = new { Status = "Failed", Message = "Invalid request", userinfo = "" };
        //            return Json(viewresponse, JsonRequestBehavior.AllowGet);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        var viewresponse = new { Status = "Failed", Message = "Invalid request", userinfo = "" };
        //        return Json(viewresponse, JsonRequestBehavior.AllowGet);
        //    }
        //}



        public ActionResult AEPSNAMEFIND(string mobile)
        {
            var name = "";
            var nm = db.AEPS_TXN_Details.Where(aa => aa.Mobile == mobile && (aa.usernm != "" && aa.usernm != null)).OrderByDescending(aa => aa.Idno).FirstOrDefault();
            if (nm != null)
            {
                name = nm.usernm;
            }
            return Json(name, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// [GET] Displays AEPS retailer outlet registration page
        /// </summary>
        public ActionResult RetailerOutletRegister()
        {
            var userid = User.Identity.GetUserId();
            var rem = db.Whitelabel_Retailer_Details.Where(aa => aa.RetailerId == userid).SingleOrDefault();
            var token = string.Empty;
            token = getAuthToken();
            if (string.IsNullOrWhiteSpace(token))
            {
                var viewresponse = new { Status = "ERR", Message = "Failed at provider server." };
                return Json(viewresponse, JsonRequestBehavior.AllowGet);
            }
            var client = new RestClient("http://api.vastbazaar.com/api/AEPS/Outletregister?mobille=" + rem.Mobile + "");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", "Bearer " + token);
            IRestResponse response = client.Execute(request);
            dynamic resp = JsonConvert.DeserializeObject(response.Content);
            var stscode = resp.Content.ADDINFO.statuscode.ToString();
            var message = resp.Content.ADDINFO.status.ToString();

            try
            {
                string MobileNo = rem.Mobile;
                var last4 = MobileNo.Substring(MobileNo.Length - 4, 4);
                message = "OTP Send Successfully On Your Mobile : xxxxxx" + last4;//resp.Content.ADDINFO.status.ToString();
            }
            catch { }

            var viewresponse1 = new { Status = stscode, Message = message };
            return Json(viewresponse1, JsonRequestBehavior.AllowGet);

        }
        public ActionResult EnterOtp(string otp)
        {
            var userid = User.Identity.GetUserId();
            var rem = db.Whitelabel_Retailer_Details.Where(aa => aa.RetailerId == userid).SingleOrDefault();
            var token = string.Empty;
            token = getAuthToken();
            if (string.IsNullOrWhiteSpace(token))
            {
                var viewresponse = new { Status = "ERR", Message = "Failed at provider server." };
                return Json(viewresponse, JsonRequestBehavior.AllowGet);
            }
            var client = new RestClient("http://api.vastbazaar.com/api/AEPS/Outletregister_verify");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", "Bearer " + token + "");
            request.AddHeader("Content-Type", "application/json");
            var req = new
            {
                mobile = rem.Mobile.ToString(),
                email = rem.Email.ToString(),
                company = rem.Frm_Name.ToString(),
                name = rem.RetailerName.ToString(),
                pan = rem.PanCard.ToString(),
                pincode = rem.Pincode.ToString(),
                address = rem.Address.ToString(),
                otp = otp
            };
            var requ = JsonConvert.SerializeObject(req);
            request.AddParameter("application/json", requ, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            dynamic resp = JsonConvert.DeserializeObject(response.Content);
            var stscode = resp.Content.ADDINFO.statuscode.ToString();
            var msg = resp.Content.ADDINFO.status.ToString();
            if (stscode == "TXN")
            {
                var outlet_id = resp.Content.ADDINFO.data.outlet_id.ToString();
                var outlet_status = resp.Content.ADDINFO.data.outlet_status.ToString();
                var kyc_status = resp.Content.ADDINFO.data.kyc_status.ToString();

                RetailerOutlet ret = new RetailerOutlet();
                ret.RetailerId = userid;
                ret.outlet_id = outlet_id;
                ret.store_type = "";
                ret.IsKycUploaded = false;
                ret.kyc_status = false;
                if (outlet_status == "1")
                {
                    ret.outlet_status = true;
                }
                else
                {
                    ret.outlet_status = false;
                }
                ret.CreatedOn = DateTime.Now;
                ret.UpdatedOn = DateTime.Now;
                ret.IsPanConfirmed = false;
                db.RetailerOutlets.Add(ret);
                db.SaveChanges();
            }
            var viewresponse1 = new { Status = stscode, Message = msg };
            return Json(viewresponse1, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult AEPSCHK1(string mobile, string uid, string bank, long iin, string cap, string type, int? amount, string remark, string DeviceSrNo, decimal servicefee)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(mobile) && Regex.IsMatch(mobile, @"[6|7|8|9][0-9]{9}") && !string.IsNullOrWhiteSpace(uid) && Regex.IsMatch(uid, @"[0-9]{12}") && iin != 0 && !string.IsNullOrWhiteSpace(cap) && !string.IsNullOrWhiteSpace(type))
                {
                    var userid = User.Identity.GetUserId();
                    var logo = "";
                    var retailer = db.Whitelabel_Retailer_Details.SingleOrDefault(a => a.RetailerId == userid);
                    var logoimg = db.tblHeaderLogoes.Where(aa => aa.Role == "ADMIN").SingleOrDefault();
                    if (logoimg != null)
                    {
                        logo = logoimg.LogoImage;
                    }
                    var reminfo = new
                    {
                        logo = logo,
                        firmname = retailer.Frm_Name,
                        taxenable = retailer.gststatus
                    };
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    //    var reminfo = js.Serialize(reminfo1);
                    remark = retailer.Frm_Name;
                    string lattitude = string.Empty;
                    string longitude = string.Empty;
                    if (retailer.Whitelabel_UserLocation == null)
                    {
                        insertGeoLocation(retailer.RetailerId, out lattitude, out longitude);
                    }
                    else
                    {
                        lattitude = retailer.Whitelabel_UserLocation.Lattitude;
                        longitude = retailer.Whitelabel_UserLocation.Longitute;
                    }
                    if (string.IsNullOrWhiteSpace(lattitude) || string.IsNullOrWhiteSpace(longitude))
                    {
                        var viewresponse = new { Status = "Failed", Message = "We are unable to find you location.", userinfo = reminfo };
                        //TempData["Respo"] = JsonConvert.SerializeObject(viewresponse);
                        //return RedirectToAction("AEPS");
                        return Json(viewresponse, JsonRequestBehavior.AllowGet);

                    }
                    var token = string.Empty;
                    token = getAuthToken();
                    if (string.IsNullOrWhiteSpace(token))
                    {
                        var viewresponse = new { Status = "Failed", Message = "Failed at provider server.", userinfo = reminfo };
                        //TempData["Respo"] = JsonConvert.SerializeObject(viewresponse);
                        //return RedirectToAction("AEPS");
                        return Json(viewresponse, JsonRequestBehavior.AllowGet);
                    }
                    var outlaet = db.RetailerOutlets.Where(aa => aa.RetailerId == userid).SingleOrDefault();
                    if (outlaet == null)
                    {
                        var viewresponse = new { Status = "Failed", Message = "Firstlly Create Aeps ID.", userinfo = reminfo };
                        //TempData["Respo"] = JsonConvert.SerializeObject(viewresponse);
                        //return RedirectToAction("AEPS");
                        return Json(viewresponse, JsonRequestBehavior.AllowGet);
                    }
                    var outletsts = outlaet.outlet_status;
                    if (outletsts == false)
                    {
                        var viewresponse = new { Status = "Failed", Message = "Your Aeps Id Validation Pending.", userinfo = reminfo };
                        //TempData["Respo"] = JsonConvert.SerializeObject(viewresponse);
                        //return RedirectToAction("AEPS");
                        return Json(viewresponse, JsonRequestBehavior.AllowGet);
                    }
                    var whitelabelid = db.Whitelabel_Retailer_Details.Where(r => r.RetailerId == userid).Single().Whitelabelid;
                    var admin = db.WhiteLabel_userList.Where(w => w.WhiteLabelID == whitelabelid).SingleOrDefault().websitename;
                    admin = admin.Replace(".", "");
                    Random random = new Random();
                    var ran = random.Next(11111, 99999);

                    var agentid = admin + DateTime.Now.ToString("dd-MM-yyy hh-mm-ss").Replace("-", "").Replace(" ", "") + ran;
                    var outletid = outlaet.outlet_id.ToString();
                    //var resp1 = Base64Encode(cap);
                    //var resp2 = Encrypt1(resp1);
                    var req = new
                    {
                        agentid = agentid,
                        Bankiin = iin,
                        mobile = mobile,
                        outlateid = outletid,
                        aepsdata = cap,
                        serialno = DeviceSrNo,
                        amount = amount,
                        aadhar = uid,
                        lattitude = lattitude,
                        longitude = longitude,
                        type = type
                    };
                    dynamic requ = JsonConvert.SerializeObject(req);
                    if (type == "BE")
                    {
                        amount = 10;

                        var client = new RestClient("http://api.vastbazaar.com/api/AEPS/instant");
                        var request = new RestRequest(Method.POST);
                        request.AddHeader("content-type", "application/json");
                        request.AddHeader("authorization", "bearer " + token);
                        request.AddParameter("application/json", requ, ParameterType.RequestBody);
                        IRestResponse response = client.Execute(request);
                        var resp = response.Content;
                        dynamic chkk = JsonConvert.DeserializeObject(resp);
                        var stscode = chkk.Content.ADDINFO.statuscode.ToString();
                        var message = chkk.Content.ADDINFO.status.ToString();
                        var rembal = ""; var rrn = ""; var sts = "Failed";
                        var respchk = (dynamic)null;
                        //    var respchk = new { }; 
                        if (stscode == "TXN" || stscode == "TUP")
                        {
                            var bal = chkk.Content.ADDINFO.data.balance.ToString();
                            rrn = chkk.Content.ADDINFO.data.opr_id.ToString();
                            var wallet_txn_id = chkk.Content.ADDINFO.data.wallet_txn_id.ToString();
                            sts = "Success";
                            var date = DateTime.Now.ToString();
                            respchk = new
                            {
                                RequestTransactionTime = date,
                                BankRrn = rrn,
                                TransactionAmount = "0",
                                TransactionStatus = "Success",
                                BalanceAmount = bal,
                                Bank = bank
                            };
                            try
                            {
                                decimal rembal1 = Convert.ToDecimal(bal);
                                db.Whitelabel_Aeps_check_balance(userid, agentid, uid, bank, rrn, remark, "Web", mobile, "", "", rembal1);
                            }
                            catch { }
                            var dd = new { Status = "Success", Message = respchk, userinfo = reminfo };
                            //TempData["Respo"] = JsonConvert.SerializeObject(dd);
                            //return RedirectToAction("AEPS");
                            return Json(dd, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            try
                            {
                                decimal rembal1 = Convert.ToDecimal(0);
                                db.Whitelabel_Aeps_check_balance(userid, agentid, uid, bank, rrn, remark, "Web", mobile, "", "", rembal1);
                            }
                            catch { }
                            var dd = new { Status = "Failed", Message = message, userinfo = reminfo };
                            //TempData["Respo"] = JsonConvert.SerializeObject(dd);
                            //return RedirectToAction("AEPS
                            return Json(dd, JsonRequestBehavior.AllowGet);
                        }


                    }
                    else if (type == "SAP")
                    {
                        System.Data.Entity.Core.Objects.ObjectParameter output = new System.Data.Entity.Core.Objects.ObjectParameter("output", typeof(string));
                        string fee = servicefee.ToString();
                        var msg = db.Whitelabel_insert_Aeps_mini_statement(userid, uid, bank, agentid, "Web", mobile, fee, "", output).SingleOrDefault().msg;
                        if (msg == "DONE")
                        {
                            amount = 10;

                            var client = new RestClient("http://api.vastbazaar.com/api/AEPS/instant");
                            var request = new RestRequest(Method.POST);
                            request.AddHeader("content-type", "application/json");
                            request.AddHeader("authorization", "bearer " + token);
                            request.AddParameter("application/json", requ, ParameterType.RequestBody);
                            IRestResponse response = client.Execute(request);
                            var resp = response.Content;
                            dynamic chkk = JsonConvert.DeserializeObject(resp);
                            var stscode = chkk.Content.ADDINFO.statuscode.ToString();
                            var message = chkk.Content.ADDINFO.status.ToString();
                            var rembal = ""; var rrn = ""; var sts = "Failed";
                            var respchk = (dynamic)null;
                            //    var respchk = new { }; 
                            if (stscode == "TXN" || stscode == "TUP")
                            {
                                var bal = chkk.Content.ADDINFO.data.balance.ToString();
                                rrn = chkk.Content.ADDINFO.data.opr_id.ToString();
                                var wallet_txn_id = chkk.Content.ADDINFO.data.wallet_txn_id.ToString();
                                sts = "Success";
                                var date = DateTime.Now.ToString();
                                respchk = new
                                {
                                    RequestTransactionTime = date,
                                    BankRrn = rrn,
                                    TransactionAmount = "0",
                                    TransactionStatus = "Success",
                                    BalanceAmount = bal,
                                    Bank = bank
                                };
                                try
                                {
                                    decimal rembal1 = 0;
                                    try
                                    {
                                        rembal1 = Convert.ToDecimal(bal);
                                    }
                                    catch { }
                                    db.update_Aeps_miniStatement(agentid, "M_Success", rrn, rembal1);
                                }
                                catch { }
                                var serdata = JsonConvert.SerializeObject(chkk.Content.ADDINFO.data);
                                var dd = new { Status = "Success", Message = serdata, userinfo = reminfo };
                                //TempData["Respo"] = JsonConvert.SerializeObject(dd);
                                //return RedirectToAction("AEPS");
                                return Json(dd, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                try
                                {
                                    decimal rembal1 = Convert.ToDecimal(0);
                                    db.update_Aeps_miniStatement(agentid, "M_Failed", rrn, rembal1);
                                }
                                catch { }
                                var dd = new { Status = "Failed", Message = message, userinfo = reminfo };
                                //TempData["Respo"] = JsonConvert.SerializeObject(dd);
                                //return RedirectToAction("AEPS
                                return Json(dd, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            var viewresponse = new { Status = "Failed", Message = msg, userinfo = reminfo };
                            return Json(viewresponse, JsonRequestBehavior.AllowGet);
                        }

                    }
                    else if (type == "CW") //DYNAMICALY ADD AMOUNT FIELD IF CASH WITHDRA
                    {
                        if (amount > 0)
                        {
                            System.Data.Entity.Core.Objects.ObjectParameter status = new System.Data.Entity.Core.Objects.ObjectParameter("Status", typeof(bool));
                            System.Data.Entity.Core.Objects.ObjectParameter output = new System.Data.Entity.Core.Objects.ObjectParameter("Output", typeof(string));
                            var dbRespo = db.Whitelabel_proc_AEPS_PreProcess(userid, agentid, uid, mobile, bank, remark ?? "TODO", amount, "web", req.ToString(), "", status, "", servicefee, "", "Cash Widthdraw", output).SingleOrDefault();

                            if (dbRespo.Status == false)
                            {
                                var viewresponse = new { Status = "Failed", Message = dbRespo.Output, userinfo = reminfo };
                                //TempData["Respo"] = JsonConvert.SerializeObject(viewresponse);
                                //return RedirectToAction("AEPS");
                                return Json(viewresponse, JsonRequestBehavior.AllowGet);

                            }

                            var clientpre = new RestClient("http://api.vastbazaar.com/api/AEPS/instantPre");
                            var requestpre = new RestRequest(Method.POST);
                            requestpre.AddHeader("content-type", "application/json");
                            requestpre.AddHeader("authorization", "bearer " + token);
                            requestpre.AddParameter("application/json", requ, ParameterType.RequestBody);
                            IRestResponse responsepre = clientpre.Execute(requestpre);
                            var resppre = responsepre.Content;
                            dynamic chkkpre = JsonConvert.DeserializeObject(resppre);
                            var stscodepre = chkkpre.Content.ADDINFO.Status.ToString();

                            if (stscodepre == "DONE")
                            {
                                var client = new RestClient("http://api.vastbazaar.com/api/AEPS/instantPost");
                                var request = new RestRequest(Method.POST);
                                request.AddHeader("content-type", "application/json");
                                request.AddHeader("authorization", "bearer " + token);
                                request.AddParameter("application/json", requ, ParameterType.RequestBody);
                                IRestResponse response = client.Execute(request);
                                var resp = response.Content;
                                dynamic chkk = JsonConvert.DeserializeObject(resp);
                                var stscode = chkk.Content.ADDINFO.statuscode.ToString();
                                var message = chkk.Content.ADDINFO.status.ToString();
                                var bal = ""; var rrn = ""; var sts = "Failed";
                                var respchk = (dynamic)null;
                                var date = DateTime.Now.ToString();
                                //    var respchk = new { }; 
                                if (stscode == "TXN" || stscode == "TUP")
                                {
                                    bal = chkk.Content.ADDINFO.data.balance.ToString();
                                    rrn = chkk.Content.ADDINFO.data.opr_id.ToString();
                                    var wallet_txn_id = chkk.Content.ADDINFO.data.wallet_txn_id.ToString();
                                    sts = "Success";
                                    respchk = new
                                    {
                                        BankRrn = rrn,
                                        BalanceAmount = bal,
                                        RequestTransactionTime = date,
                                        TransactionAmount = amount,
                                        TransactionStatus = "Success",
                                        Bank = bank
                                    };
                                    if (type == "CW")
                                    {
                                        if (stscode == "TXN")
                                        {
                                            System.Data.Entity.Core.Objects.ObjectParameter procStatus = new System.Data.Entity.Core.Objects.ObjectParameter("ProcStatus", typeof(string));
                                            System.Data.Entity.Core.Objects.ObjectParameter procMessage = new System.Data.Entity.Core.Objects.ObjectParameter("ProcMessage", typeof(string));
                                            decimal remain = 0;
                                            try
                                            {
                                                remain = Convert.ToDecimal(bal);
                                            }
                                            catch { }
                                            db.whitelabel_proc_AEPS_PostProcess(userid, agentid, rrn, wallet_txn_id, "Success", response.Content, "web", "", remain, procStatus, procMessage);

                                        }
                                    }
                                    var dd = new { Status = "Success", Message = respchk, userinfo = reminfo };
                                    //TempData["Respo"] = JsonConvert.SerializeObject(dd);
                                    //return RedirectToAction("AEPS");
                                    return Json(dd, JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    if (type == "CW")
                                    {
                                        System.Data.Entity.Core.Objects.ObjectParameter procStatus = new System.Data.Entity.Core.Objects.ObjectParameter("ProcStatus", typeof(string));
                                        System.Data.Entity.Core.Objects.ObjectParameter procMessage = new System.Data.Entity.Core.Objects.ObjectParameter("ProcMessage", typeof(string));
                                        decimal remain = 0;
                                        try
                                        {
                                            remain = Convert.ToDecimal(bal);
                                        }
                                        catch { }
                                        db.whitelabel_proc_AEPS_PostProcess(userid, agentid, "NA", null, "Failed", response.Content, "web", "Failed from supplier", remain, procStatus, procMessage);
                                    }
                                    var dd = new { Status = "Failed", Message = message, userinfo = reminfo };
                                    //TempData["Respo"] = JsonConvert.SerializeObject(dd);
                                    //return RedirectToAction("AEPS");
                                    return Json(dd, JsonRequestBehavior.AllowGet);
                                }
                            }
                            else
                            {
                                var aa = new { Status = "Failed", Message = "Please Try After Some Time", userinfo = reminfo };
                                //TempData["Respo"] = JsonConvert.SerializeObject(aa);
                                //return RedirectToAction("AEPS");
                                return Json(aa, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            var aa = new { Status = "Failed", Message = "Invalid Amount.", userinfo = reminfo };
                            //TempData["Respo"] = JsonConvert.SerializeObject(aa);
                            //return RedirectToAction("AEPS");
                            return Json(aa, JsonRequestBehavior.AllowGet);

                        }
                    }
                    else
                    {
                        var dd = new { Status = "Failed", Message = "Please Try Again.", userinfo = reminfo };
                        //TempData["Respo"] = JsonConvert.SerializeObject(dd);
                        //return RedirectToAction("AEPS");
                        return Json(dd, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    var dd = new { Status = "Failed", Message = "Either mobile or aadhaar is invalid." };
                    //TempData["Respo"] = JsonConvert.SerializeObject(dd);
                    //return RedirectToAction("AEPS");
                    return Json(dd, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                var dd = new { Status = "Failed", Message = "Invalid request" };
                //TempData["Respo"] = JsonConvert.SerializeObject(dd);
                //return RedirectToAction("AEPS");
                return Json(dd, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        /// <summary>
        /// [GET] Displays AEPS transaction history and report
        /// </summary>
        public ActionResult AepsReport()
        {
            string userid = User.Identity.GetUserId();
            string txt_frm_date = DateTime.Now.ToString();
            string txt_to_date = DateTime.Now.ToString();

            var wid = db.Whitelabel_Retailer_Details.Where(s => s.RetailerId == userid).Single().Whitelabelid;

            //Retailer 
            ViewBag.allretailer = new SelectList(db.Whitelabel_select_retailer_for_ddl("Admin", wid), "RetailerId", "RetailerName", null);

            if (txt_frm_date == null && txt_to_date == null)
            {
                txt_frm_date = DateTime.Now.ToString();
                txt_to_date = DateTime.Now.ToString();
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
            int pagesize = 50;
            var rowdata = db.spWhitelabel_Aeps_Report_New(wid, "Whitelabelretailer", userid, "ALL", null, "", "", "", "ALL", Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), 1, pagesize).ToList();
            //var rowdata = db.proc_whitelabel_AepsReport("Retailer", userid, "ALL", null, "", "", "", "ALL", Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), 1, pagesize).ToList();

            return View(rowdata);
        }

        [HttpPost]
        /// <summary>
        /// [POST] Filters AEPS report by date and user
        /// </summary>
        public ActionResult AepsReport(int? amount, string txt_frm_date, string txt_to_date, string allretailer, string ddl_top, string ddl_status, string BankId, string aadhar)
        {
            ViewBag.chk = "post";
            string userid = User.Identity.GetUserId();
            if (txt_frm_date == null && txt_to_date == null && ddl_status == null)
            {
                txt_frm_date = DateTime.Now.ToString();
                txt_to_date = DateTime.Now.ToString();
                ddl_status = "";
            }
            var wid = db.Whitelabel_Retailer_Details.Where(s => s.RetailerId == userid).Single().Whitelabelid;

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
            ViewBag.checkdata = null;
            var checkdata = db.AEPS_TXN_Details.Where(a => a.UserId == userid).FirstOrDefault();
            if (checkdata != null)
            {
                ViewBag.checkdata = "Data";
            }
            int pagesize = 100;
            if (ddl_status == "")
            {
                ddl_status = "ALL";
            }
            if (BankId == null)
            {
                BankId = "";
            }
            if (aadhar == null)
            {
                aadhar = "";
            }
            var rowdata = db.spWhitelabel_Aeps_Report_New(wid, "Whitelabelretailer", userid, ddl_status, amount, "", BankId, aadhar, "ALL", Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), 1, pagesize).ToList();

            return View(rowdata);
        }

        //[ChildActionOnly]
        //public ActionResult _AepsReport(string txt_frm_date, string txt_to_date, string ddl_status, int? amount, string BankId, string aadhar)
        //{
        //    string userid = User.Identity.GetUserId();
        //    if (txt_frm_date == null && txt_to_date == null && ddl_status == null)
        //    {
        //        txt_frm_date = DateTime.Now.ToString();
        //        txt_to_date = DateTime.Now.ToString();
        //        ddl_status = "";
        //    }
        //    var wid = db.Whitelabel_Retailer_Details.Where(s => s.RetailerId == userid).Single().Whitelabelid;

        //    DateTime frm1 = Convert.ToDateTime(txt_frm_date);
        //    DateTime to1 = Convert.ToDateTime(txt_to_date);

        //    txt_frm_date = frm1.ToString("dd-MM-yyyy");
        //    txt_to_date = to1.ToString("dd-MM-yyyy");
        //    string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy",
        //                    "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
        //    DateTime dt = !string.IsNullOrWhiteSpace(txt_frm_date) ? DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
        //    DateTime dt1 = !string.IsNullOrWhiteSpace(txt_to_date) ? DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
        //    DateTime frm_date = Convert.ToDateTime(dt).Date;
        //    DateTime to_date = Convert.ToDateTime(dt1).Date.AddDays(1);
        //    ViewBag.checkdata = null;
        //    var checkdata = db.AEPS_TXN_Details.Where(a => a.UserId == userid).FirstOrDefault();
        //    if (checkdata != null)
        //    {
        //        ViewBag.checkdata = "Data";
        //    }
        //    int pagesize = 100;
        //    if (ddl_status == "")
        //    {
        //        ddl_status = "ALL";
        //    }
        //    if (BankId == null)
        //    {
        //        BankId = "";
        //    }
        //    if (aadhar == null)
        //    {
        //        aadhar = "";
        //    }
        //    var rowdata = db.spWhitelabel_Aeps_Report_New(wid, "Retailer", userid, ddl_status, amount, "", BankId, aadhar, "ALL", Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), 1, pagesize).ToList();

        //    return View(rowdata);
        //}

        /// <summary>
        /// [GET] Displays the new-format AEPS transaction listing
        /// </summary>
        public ActionResult AEPSreportnew()
        {
            var userid = User.Identity.GetUserId();
            Recent_report recent = new Recent_report();
            recent.WRecent_report_imps = null;
            recent.WRecent_report_Aeps = db.AEPS_TXN_Details.Where(aa => aa.UserId == userid).OrderByDescending(aa => aa.Txn_Date).Take(10).ToList(); ;
            recent.WRecent_PAN_CARD_IPAY = null;
            return PartialView("_RecentReport", recent);
        }
        [HttpGet]
        [ValidateInput(false)]
        public ActionResult Print_aeps_ministatement_Pdf(string txtbankstate, string txtfrmstate, string txtbankaadhar, string txtfrmdate, string txtfeeservice, string statementtbody)
        {
            var logo = "";

            var logochk1 = db.tblHeaderLogoes.Where(aa => aa.Role == "ADMIN").SingleOrDefault();
            if (logochk1 != null)
            {
                logo = logochk1.LogoImage;
            }
            statementtbody = Regex.Replace(statementtbody, @"\\n", "");
            var logochk = logo.Replace("\\Outside_logo\\", "");
            ViewData["txtbankstate"] = txtbankstate;
            ViewData["txtfrmstate"] = txtfrmstate;
            ViewData["txtbankaadhar"] = txtbankaadhar;
            ViewData["txtfrmdate"] = txtfrmdate;
            ViewData["txtfeeservice"] = txtfeeservice;
            ViewData["statementtbody"] = statementtbody;
            ViewData["logochk"] = logochk;
            return new ViewAsPdf("Print_aeps_ministatement_Pdf");
            // return View();
        }


        public ActionResult Print_aeps_balance_Pdf(string rrnno, string firmname, string aadhar, string mobile, string remain)
        {
            var logo = "";

            var logochk1 = db.tblHeaderLogoes.Where(aa => aa.Role == "ADMIN").SingleOrDefault();
            if (logochk1 != null)
            {
                logo = logochk1.LogoImage;
            }
            var logochk = logo.Replace("\\Outside_logo\\", "");
            ViewData["rrnno"] = rrnno;
            ViewData["firmname"] = firmname;
            ViewData["aadhar"] = aadhar;
            ViewData["mobile"] = mobile;
            ViewData["remain"] = remain;
            ViewData["logochk"] = logochk;
            return new ViewAsPdf("Print_aeps_balance_Pdf");
            // return View();
        }

        public ActionResult Print_aeps_transfer_Pdf(string rrnno, string firmname, string aadhar, string mobile, string remain, string amount, string servicefee, string tax, string paidamount)
        {
            var logo = "";

            var logochk1 = db.tblHeaderLogoes.Where(aa => aa.Role == "ADMIN").SingleOrDefault();
            if (logochk1 != null)
            {
                logo = logochk1.LogoImage;
            }
            var logochk = logo.Replace("\\Outside_logo\\", "");
            ViewData["rrnno"] = rrnno;
            ViewData["firmname"] = firmname;
            ViewData["aadhar"] = aadhar;
            ViewData["mobile"] = mobile;
            ViewData["remain"] = remain;
            ViewData["logochk"] = logochk;
            ViewData["servicefee"] = servicefee;
            ViewData["tax"] = tax;
            ViewData["paidamount"] = paidamount;
            ViewData["amount"] = amount;
            return new ViewAsPdf("Print_aeps_transfer_Pdf");
            // return View();
        }
        [HttpGet]
        /// <summary>
        /// [GET] Displays AEPS device registration form
        /// </summary>
        public ActionResult regAeps()
        {
            var Retailerid = User.Identity.GetUserId();
            var show = db.Whitelabel_Retailer_Details.Find(Retailerid);
            AEPSRegModel model = new AEPSRegModel();
            model.AadharCard = show.AadharCard;
            model.Address = show.Address;
            model.canceledPath = "";
            model.Email = show.Email;
            model.kycPath = "";
            model.Mobile = show.Mobile;
            model.PanCard = show.PanCard;
            model.Pwd = "";
            model.RetailerId = show.RetailerId;
            model.RetailerName = show.RetailerName;
            model.State = show.State;
            ViewBag.Sate = db.State_Desc.Select(a => new SelectListItem { Text = a.State_name, Value = a.State_id.ToString() }).ToList();
            ViewBag.District = db.District_Desc.Where(a => a.State_id == show.State).Select(a => new SelectListItem { Text = a.Dist_Desc, Value = a.Dist_id.ToString() }).ToList();
            return View(model);
        }
        [HttpPost]
        public ActionResult regAeps(AEPSRegModel model)
        {
            if (ModelState.IsValid)
            {
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    var Retailerid = User.Identity.GetUserId();
                    var show = db.Retailer_Details.Find(Retailerid);
                    var file1 = Request.Files[0];
                    var imagePath1 = "";
                    if (file1 != null && file1.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(file1.FileName);
                        fileName = Guid.NewGuid().ToString() + "_" + fileName;
                        imagePath1 = Server.MapPath("~/AEPS/files/") + fileName;
                        file1.SaveAs(imagePath1);
                    }
                    var file2 = Request.Files[0];
                    var imagePath2 = "";
                    if (file2 != null && file2.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(file2.FileName);
                        fileName = Guid.NewGuid().ToString() + "_" + fileName + Path.GetExtension(fileName);
                        imagePath2 = Server.MapPath("~/AEPS/files/") + fileName;
                        file2.SaveAs(imagePath2);
                    }
                    var file3 = Request.Files[0];
                    var imagePath3 = "";
                    if (file3 != null && file3.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(file3.FileName);
                        fileName = Guid.NewGuid().ToString() + "_" + fileName + Path.GetExtension(fileName);
                        imagePath3 = Server.MapPath("~/AEPS/files/") + fileName;
                        file3.SaveAs(imagePath3);
                    }
                    var file4 = Request.Files[0];
                    var imagePath4 = "";
                    if (file4 != null && file4.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(file4.FileName);
                        fileName = Guid.NewGuid().ToString() + "_" + fileName + Path.GetExtension(fileName);
                        imagePath4 = Server.MapPath("~/AEPS/files/") + fileName;
                        file4.SaveAs(imagePath4);
                    }
                    AEPS_Merchants newentry = new AEPS_Merchants();
                    newentry.KYCPATH = imagePath2;
                    newentry.LogoPath = imagePath1;
                    newentry.Pwd = model.Pwd;
                    newentry.ShopPanPath = imagePath4;
                    newentry.status = "Pending";
                    newentry.UpdatedOn = DateTime.Now;
                    newentry.UserId = Retailerid;
                    newentry.UserType = model.UserType;
                    newentry.createdOn = DateTime.Now;
                    newentry.createdOn = DateTime.Now;
                    newentry.cancelledCheckPath = imagePath3;
                    db.AEPS_Merchants.Add(newentry);
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Index");
        }
        #endregion

        #region AEPS_old
        //public ActionResult AEPS()
        //{
        //    var userid = User.Identity.GetUserId();
        //    var sts = db.Whitelabel_Retailer_Details.Where(a => a.RetailerId == userid && a.AepsMposstatus == "Y" && a.AepsMposstatus != null).Any();
        //    if (sts == true)
        //    {
        //        var retailerDetails = db.Whitelabel_Retailer_Details.Where(a => a.RetailerId == userid).FirstOrDefault();
        //        sts = db.whitelabel_Dealer_Details.Where(a => a.DealerId == retailerDetails.DealerId).Any();
        //        if (sts == true)
        //        {
        //            var check = "OK"; var errormsg = "";
        //            if (check == "OK")
        //            {
        //                if (retailerDetails != null)
        //                {
        //                    string str = string.Empty;
        //                    bool IsUpdateRequired = false;
        //                    if (retailerDetails.AadharCard == null || retailerDetails.AadharCard.Length < 12)
        //                    {
        //                        str = str + "Aadhar,";
        //                        IsUpdateRequired = true;
        //                    }
        //                    if (retailerDetails.PanCard == null || retailerDetails.PanCard.Length < 10)
        //                    {
        //                        str = str + "Pancard,";
        //                        IsUpdateRequired = true;
        //                    }
        //                    if (retailerDetails.Frm_Name == null || string.IsNullOrWhiteSpace(retailerDetails.Frm_Name))
        //                    {
        //                        str = str + "Firm name,";
        //                        IsUpdateRequired = true;
        //                    }
        //                    if (retailerDetails.Address == null || string.IsNullOrWhiteSpace(retailerDetails.Address))
        //                    {
        //                        str = str + "Address,";
        //                        IsUpdateRequired = true;
        //                    }

        //                    //if (retailerDetails.AepsMerchandId == null || string.IsNullOrWhiteSpace(retailerDetails.AepsMerchandId))
        //                    //{
        //                    //    str = str + "AepsMerchant Id,";
        //                    //    IsUpdateRequired = true;
        //                    ////}
        //                    //if (retailerDetails.AepsMPIN == null || string.IsNullOrWhiteSpace(retailerDetails.AepsMPIN))
        //                    //{
        //                    //    str = str + "AepsMerchant Pin,";
        //                    //    IsUpdateRequired = true;
        //                    //}
        //                    //if (retailerDetails.Position == null || string.IsNullOrWhiteSpace(retailerDetails.Position))
        //                    //{
        //                    //    str = str + "Position,";
        //                    //    IsUpdateRequired = true;
        //                    //}
        //                    //if (retailerDetails.BusinessType == null || string.IsNullOrWhiteSpace(retailerDetails.BusinessType))
        //                    //{
        //                    //    str = str + "Business Type,";
        //                    //    IsUpdateRequired = true;
        //                    //}
        //                    //if (retailerDetails.Bankaccountno == null || string.IsNullOrWhiteSpace(retailerDetails.Bankaccountno))
        //                    //{
        //                    //    str = str + "Bank accountno,";
        //                    //    IsUpdateRequired = true;
        //                    //}
        //                    //if (retailerDetails.Ifsccode == null || string.IsNullOrWhiteSpace(retailerDetails.Ifsccode))
        //                    //{
        //                    //    str = str + "IFSC Code,";
        //                    //    IsUpdateRequired = true;
        //                    ////}
        //                    //if (retailerDetails.city == null || string.IsNullOrWhiteSpace(retailerDetails.city))
        //                    //{
        //                    //    str = str + "City,";
        //                    //    IsUpdateRequired = true;
        //                    //}
        //                    //if (retailerDetails.accountholder == null || string.IsNullOrWhiteSpace(retailerDetails.accountholder))
        //                    //{
        //                    //    str = str + "Accountholder,";
        //                    //    IsUpdateRequired = true;
        //                    //}
        //                    //if (retailerDetails.bankname == null || string.IsNullOrWhiteSpace(retailerDetails.bankname))
        //                    //{
        //                    //    str = str + "Bank Name,";
        //                    //    IsUpdateRequired = true;
        //                    //}
        //                    //if (retailerDetails.pancardPath == null || string.IsNullOrWhiteSpace(retailerDetails.pancardPath))
        //                    //{
        //                    //    str = str + "Pancard Document,";
        //                    //    IsUpdateRequired = true;
        //                    //}
        //                    //if (retailerDetails.aadharcardPath == null || string.IsNullOrWhiteSpace(retailerDetails.aadharcardPath))
        //                    //{
        //                    //    str = str + "Aadhaar Card Document,";
        //                    //    IsUpdateRequired = true;
        //                    //}
        //                    //if (retailerDetails.frimregistrationPath == null || string.IsNullOrWhiteSpace(retailerDetails.frimregistrationPath))
        //                    //{
        //                    //    str = str + "Registration Document,";
        //                    //    IsUpdateRequired = true;
        //                    //}
        //                    //if (retailerDetails.Photo == null || string.IsNullOrWhiteSpace(retailerDetails.Photo))
        //                    //{
        //                    //    str = str + "Passport Image Document,";
        //                    //    IsUpdateRequired = true;
        //                    //}

        //                    if (retailerDetails.Pincode == 0 || retailerDetails.Pincode.ToString().Length < 6)
        //                    {
        //                        str = str + "Pincode,";
        //                        IsUpdateRequired = true;
        //                    }
        //                    //if (retailerDetails.dateofbirth == null)
        //                    //{
        //                    //    str = str + "Date of birth";
        //                    //    IsUpdateRequired = true;
        //                    //}
        //                    //if (retailerDetails.AepsMerchandId == null)
        //                    //{
        //                    //    str = str + "AEPS Merchand Id";
        //                    //    IsUpdateRequired = true;
        //                    //}
        //                    //if (retailerDetails.AepsMPIN == null)
        //                    //{
        //                    //    str = str + "AEPS MPIN";
        //                    //    IsUpdateRequired = true;
        //                    //}

        //                    if (IsUpdateRequired)
        //                    {
        //                        if (str.EndsWith(","))
        //                            str = str.Substring(0, str.Length - 1);
        //                        //str = str + " are required to become AEPS .";
        //                        str = "Firstly Complete Your Full KYC";

        //                        var results1 = "{'status':'NOTOK','msg':'" + str + "'}";
        //                        var json11 = JsonConvert.DeserializeObject(results1);
        //                        var json111 = JsonConvert.SerializeObject(json11);
        //                        return Json(json111, JsonRequestBehavior.AllowGet);

        //                    }
        //                    // Fing Pay
        //                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
        //                    var client = new RestClient("https://fingpayap.tapits.in/fpaepsservice/api/bankdata/bank/details");
        //                    var request = new RestRequest(Method.GET);
        //                    IRestResponse response = client.Execute(request);
        //                    var items = JsonConvert.DeserializeObject<BankIiNoModel>(response.Content);
        //                    var blist = items.Data.Select(a => new SelectListItem { Text = a.BankName, Value = a.IInNo }).ToList();
        //                    //  ViewBag.BankList = items.Data.Select(a => new SelectListItem { Text = a.BankName, Value = a.IInNo }).ToList();
        //                    var results = "{'status':'OK','aepsid':'" + retailerDetails.Email + "','banklist':'" + response.Content + "'}";
        //                    var json1 = JsonConvert.DeserializeObject(results);
        //                    var json = JsonConvert.SerializeObject(json1);
        //                    return Json(json, JsonRequestBehavior.AllowGet);
        //                }
        //                // Instant Pay
        //                //var CheckOutletStatus = db.RetailerOutlets.Where(a => a.RetailerId == userid).FirstOrDefault();
        //                //if (CheckOutletStatus != null)
        //                //{
        //                //    if (CheckOutletStatus.outlet_status)
        //                //    {
        //                          ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
        //                //        var client = new RestClient("https://fingpayap.tapits.in/fpaepsservice/api/bankdata/bank/details");
        //                //        var request = new RestRequest(Method.GET);
        //                //        IRestResponse response = client.Execute(request);
        //                //        var items = JsonConvert.DeserializeObject<BankIiNoModel>(response.Content);
        //                //        var blist = items.Data.Select(a => new SelectListItem { Text = a.BankName, Value = a.IInNo }).ToList();
        //                //        //  ViewBag.BankList = items.Data.Select(a => new SelectListItem { Text = a.BankName, Value = a.IInNo }).ToList();
        //                //        var results = "{'status':'OK','aepsid':'" + retailerDetails.Email + "','banklist':'" + response.Content + "'}";
        //                //        var json1 = JsonConvert.DeserializeObject(results);
        //                //        var json = JsonConvert.SerializeObject(json1);
        //                //        return Json(json, JsonRequestBehavior.AllowGet);
        //                //    }
        //                //    else
        //                //    {
        //                //        var results = "{'status':'ERR','msg':'You AEPS InActive'}";
        //                //        var json1 = JsonConvert.DeserializeObject(results);
        //                //        var json = JsonConvert.SerializeObject(json1);
        //                //        return Json(json, JsonRequestBehavior.AllowGet);
        //                //    }

        //                //}
        //                else
        //                {
        //                    var results = "{'status':'ERR','msg':''}";
        //                    var json1 = JsonConvert.DeserializeObject(results);
        //                    var json = JsonConvert.SerializeObject(json1);
        //                    return Json(json, JsonRequestBehavior.AllowGet);
        //                }
        //                /**********************************/
        //            }
        //            else
        //            {
        //                var results = "{'status':'NOTOK','msg':'" + errormsg + "'}";
        //                var json1 = JsonConvert.DeserializeObject(results);
        //                var json = JsonConvert.SerializeObject(json1);
        //                return Json(json, JsonRequestBehavior.AllowGet);

        //            }
        //        }
        //        else
        //        {
        //            var results = "{'status':'NOTOK','msg':'Your Aeps Status Inactive Please Contact Distributor'}";
        //            var json1 = JsonConvert.DeserializeObject(results);
        //            var json = JsonConvert.SerializeObject(json1);
        //            return Json(json, JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //    else
        //    {
        //        var results = "{'status':'NOTOK','msg':'Your Aeps Status Inactive Please Contact Admin'}";
        //        var json1 = JsonConvert.DeserializeObject(results);
        //        var json = JsonConvert.SerializeObject(json1);
        //        return View();
        //    }
        //}
        //[HttpPost]
        //public ActionResult AEPS(string mobile, string uid, string bank, long iin, string cap, string type, string tabvalue, int? amount, string remark, string DeviceSrNo, decimal servicefee)
        //{
        //    try
        //    {

        //        if (!string.IsNullOrWhiteSpace(mobile) && Regex.IsMatch(mobile, @"[6|7|8|9][0-9]{9}") && !string.IsNullOrWhiteSpace(uid) && Regex.IsMatch(uid, @"[0-9]{12}") && iin != 0 && !string.IsNullOrWhiteSpace(cap) && !string.IsNullOrWhiteSpace(type))
        //        {
        //            var chkkkk = JsonConvert.DeserializeObject<CaptureResponse>(cap);
        //            if (chkkkk.Piddata != null)
        //            {
        //                var userid = User.Identity.GetUserId();
        //                var dealerid = db.Whitelabel_Retailer_Details.Where(aa => aa.RetailerId == userid).SingleOrDefault().DealerId;
        //                var whitelabelid = db.whitelabel_Dealer_Details.Where(aa => aa.DealerId == dealerid).SingleOrDefault().Whitelabelid;
        //                var logo = "";
        //                var retailer = db.Whitelabel_Retailer_Details.SingleOrDefault(a => a.RetailerId == userid);
        //                var logoimg = db.tblHeaderLogoes.Where(aa => aa.UserId == whitelabelid).SingleOrDefault();
        //                if (logoimg != null)
        //                {
        //                    logo = logoimg.LogoImage;
        //                }
        //                var reminfo = new
        //                {
        //                    logo = logo,
        //                    firmname = retailer.Frm_Name
        //                };
        //                var ouletid = retailer.AepsMerchandId;
        //                var aepspin = retailer.AepsMPIN;
        //                string lattitude = string.Empty;
        //                string longitude = string.Empty;
        //                if (retailer.Whitelabel_UserLocation == null)
        //                {
        //                    insertGeoLocation(retailer.RetailerId, out lattitude, out longitude);
        //                }
        //                else
        //                {
        //                    lattitude = retailer.Whitelabel_UserLocation.Lattitude;
        //                    longitude = retailer.Whitelabel_UserLocation.Longitute;
        //                }
        //                if (string.IsNullOrWhiteSpace(lattitude) || string.IsNullOrWhiteSpace(longitude))
        //                {
        //                    var viewresponse = new { Status = "Failed", Message = "We are unable to find you location.", userinfo = reminfo };
        //                    return Json(viewresponse, JsonRequestBehavior.AllowGet);
        //                }
        //                var distname = db.District_Desc.Where(aa => aa.State_id == retailer.State && aa.Dist_id == retailer.District).SingleOrDefault().Dist_Desc;

        //                var token = string.Empty;
        //                token = getAuthToken();
        //                if (string.IsNullOrWhiteSpace(token))
        //                {
        //                    var viewresponse = new { Status = "Failed", Message = "Failed at provider server.", userinfo = reminfo };
        //                    return Json(viewresponse, JsonRequestBehavior.AllowGet);
        //                }
        //                if (ouletid == null || ouletid == "")
        //                {
        //                    var reque = new
        //                    {
        //                        merchantName = retailer.RetailerName,
        //                        stateid = retailer.State,
        //                        latitude = lattitude,
        //                        longitude = longitude,
        //                        merchantPhoneNumber = retailer.Mobile,
        //                        merchantPinCode = retailer.Pincode,
        //                        merchantCityName = distname,
        //                        merchantAddress = retailer.Address,
        //                        userPan = retailer.PanCard,
        //                        retilerid = retailer.Email,
        //                        OTP = ""
        //                    };
        //                    var resquestchk = JsonConvert.SerializeObject(reque);
        //                    var client = new RestClient("http://api.vastbazaar.com/api/AEPS/RegisterAEPS");
        //                    client.Timeout = -1;
        //                    var request = new RestRequest(Method.POST);
        //                    request.AddHeader("Authorization", "Bearer " + token);
        //                    request.AddHeader("Content-Type", "application/json");

        //                    request.AddParameter("application/json", resquestchk, ParameterType.RequestBody);
        //                    IRestResponse response = client.Execute(request);
        //                    dynamic resp = JsonConvert.DeserializeObject(response.Content);
        //                    var stscode = resp.Content.ADDINFO.statuscode.ToString();
        //                    var message = resp.Content.ADDINFO.status.ToString();
        //                    if (stscode == "TXN")
        //                    {
        //                        ouletid = resp.Content.ADDINFO.data.outlet_id.ToString();
        //                        var pin = resp.Content.ADDINFO.data.pin.ToString();
        //                        retailer.AepsMerchandId = ouletid;
        //                        retailer.AepsMPIN = pin;
        //                        db.SaveChanges();
        //                        retailer = db.Whitelabel_Retailer_Details.SingleOrDefault(a => a.RetailerId == userid);
        //                    }
        //                    else
        //                    {
        //                        var viewresponse = new { Status = "Failed", Message = message, userinfo = reminfo };
        //                        return Json(viewresponse, JsonRequestBehavior.AllowGet);
        //                    }
        //                }
        //                if (ouletid != null)
        //                {
        //                    var client1 = new RestClient("http://api.vastbazaar.com/api/AEPS/supermerchant?merchant=" + retailer.AepsMerchandId + "");
        //                    var request1 = new RestRequest(Method.POST);
        //                    request1.AddHeader("Authorization", "Bearer " + token);
        //                    IRestResponse response1 = client1.Execute(request1);
        //                    var respsuper = response1.Content;
        //                    dynamic chkresp = JsonConvert.DeserializeObject(respsuper);
        //                    var stscode = chkresp.Content.ADDINFO.stscode.ToString();
        //                    if (stscode == "0")
        //                    {
        //                        var msg = chkresp.Content.ADDINFO.message.ToString();
        //                        var viewresponse = new { Status = "Failed", Message = msg, userinfo = reminfo };
        //                        return Json(viewresponse, JsonRequestBehavior.AllowGet);
        //                    }
        //                    var superid = chkresp.Content.ADDINFO.data.superid.ToString();
        //                    var superusername = chkresp.Content.ADDINFO.data.superusername.ToString();

        //                    if (string.IsNullOrWhiteSpace(lattitude) || string.IsNullOrWhiteSpace(longitude))
        //                    {
        //                        var msg = chkresp.Content.ADDINFO.message.ToString();
        //                        var viewresponse = new { Status = "Failed", Message = "We are anable to find you location.", userinfo = reminfo };
        //                        return Json(viewresponse, JsonRequestBehavior.AllowGet);
        //                    }
        //                    Main obj = new Main();
        //                    AepsModel reqObject = new AepsModel();

        //                    reqObject.captureResponse = JsonConvert.DeserializeObject<CaptureResponse>(cap);
        //                    reqObject.cardnumberORUID = new CardnumberOruid { adhaarNumber = uid, nationalBankIdentificationNumber = iin };
        //                    //var address = "75 Ninth Avenue 2nd and 4th Floors New York, NY 10011";
        //                    //var locationService = new GoogleLocationService();
        //                    //var point = locationService.GetLatLongFromAddress(retailer.Address);
        //                    reqObject.latitude = Convert.ToDouble(lattitude);//27.617470;// point.Latitude;
        //                    reqObject.longitude = Convert.ToDouble(longitude);//75.144400;

        //                    // string ProjectName = Path.GetDirectoryName(Path.GetDirectoryName(Server.MapPath(@"~/HomeControllers.cs")));
        //                    // ProjectName = ProjectName.Substring(ProjectName.LastIndexOf("\\") + 1);
        //                    var admin_details = db.Admin_details.SingleOrDefault();
        //                    var infoadmin = admin_details.WebsiteUrl.Replace(".", "");
        //                    var agentid = infoadmin + "" + DateTime.Now.ToString("dd-MM-yyy hh-mm-ss").Replace("-", "").Replace(" ", "");
        //                    reqObject.merchantTransactionId = agentid;
        //                    reqObject.merchantTranId = infoadmin + DateTime.Now.ToString("dd-MM-yyy hh-mm-ss").Replace("-", "").Replace(" ", "");
        //                    reqObject.mobileNumber = mobile;
        //                    reqObject.requestRemarks = "";
        //                    reqObject.superMerchantId = superid;
        //                    reqObject.merchantUserName = retailer.AepsMerchandId;//"vastwebm";// "9214711111";//"excelonestopd";// "sai";
        //                    reqObject.merchantPin = CreateMD5(retailer.AepsMPIN);//"b027291b0af8cde6ae6e30bf6056204b";// "81dc9bdb52d04dc20036dbd8313ed055";//"81DC9BDB52D04DC20036DBD8313ED055";
        //                                                                         //      var rootDir = Path.GetDirectoryName(Path.GetDirectoryName(Server.MapPath(@"~/HomeControllers.cs")));
        //                                                                         //      rootDir = rootDir.Substring(rootDir.LastIndexOf("\\") + 1);
        //                                                                         //   reqObject.subMerchantId = rootDir; //"skmoney";//"excelonestopm"//TODO

        //                    reqObject.timestamp = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);//"dd/MM/yyyy HH:mm:ss"
        //                    reqObject.transactionType = type;
        //                    if (type == "CW") //DYNAMICALY ADD AMOUNT FIELD IF CASH WITHDRA
        //                    {
        //                        if (amount != null && amount > 0)
        //                        {
        //                            reqObject.transactionAmount = (int)amount;
        //                        }
        //                        else
        //                        {
        //                            var msg = chkresp.Content.ADDINFO.message.ToString();
        //                            var viewresponse = new { Status = "Failed", Message = "Invalid Amount.", userinfo = reminfo };
        //                            return Json(viewresponse, JsonRequestBehavior.AllowGet);
        //                        }
        //                    }
        //                    else if (type == "SAP")
        //                    {
        //                        // reqObject.transactionAmount = 10;
        //                        reqObject.transactionType = "MS";
        //                        System.Data.Entity.Core.Objects.ObjectParameter output = new System.Data.Entity.Core.Objects.ObjectParameter("output", typeof(string));
        //                        string fee = servicefee.ToString();
        //                        var msg = db.Whitelabel_insert_Aeps_mini_statement(userid, uid, bank, agentid, "Web", mobile, fee, output).SingleOrDefault().msg;
        //                        if (msg != "DONE")
        //                        {
        //                            var viewresponse = new { Status = "Failed", Message = msg, userinfo = reminfo };
        //                            return Json(viewresponse, JsonRequestBehavior.AllowGet);
        //                        }
        //                    }
        //                    dynamic RequestJson = JsonConvert.SerializeObject(reqObject);
        //                    byte[] hash = Main.generateSha256Hash(Encoding.ASCII.GetBytes(RequestJson));
        //                    byte[] skey = Main.generateSessionKey();
        //                    string encryptUsingSessionKey = Main.encryptUsingSessionKey(skey, Encoding.ASCII.GetBytes(RequestJson));
        //                    string encryptUsingPublicKey = Main.encryptUsingPublicKey(skey);
        //                    if (string.IsNullOrWhiteSpace(encryptUsingSessionKey) || string.IsNullOrWhiteSpace(encryptUsingPublicKey))
        //                    {
        //                        var viewresponse = new { Status = "Failed", Message = "Failed to initiate request.", userinfo = reminfo };
        //                        return Json(viewresponse, JsonRequestBehavior.AllowGet);
        //                    }
        //                    var client = new RestClient();
        //                    if (type == "CW")
        //                    {
        //                        //tbl_Holiday
        //                        client = new RestClient(VastbazaarBaseUrl + "api/AEPS/cashWithdrawPost");
        //                        string req = RequestJson.ToString();
        //                        System.Data.Entity.Core.Objects.ObjectParameter status = new System.Data.Entity.Core.Objects.ObjectParameter("Status", typeof(bool));
        //                        System.Data.Entity.Core.Objects.ObjectParameter output = new System.Data.Entity.Core.Objects.ObjectParameter("Output", typeof(string));
        //                        var dbRespo = db.Whitelabel_proc_AEPS_PreProcess(userid, agentid, uid, mobile, bank, remark ?? "TODO", amount, "web", req, "", status, "", servicefee, output).SingleOrDefault();
        //                        if (dbRespo.Status == false)
        //                        {
        //                            var viewresponse = new { Status = "Failed", Message = dbRespo.Output, userinfo = reminfo };
        //                            return Json(viewresponse, JsonRequestBehavior.AllowGet);
        //                        }
        //                    }
        //                    else if (type == "SAP")
        //                    {
        //                        client = new RestClient(VastbazaarBaseUrl + "api/AEPS/MiniStateMent");
        //                    }
        //                    else if (type == "BE")
        //                    {
        //                        client = new RestClient(VastbazaarBaseUrl + "api/AEPS/balanceEnquiry");
        //                    }
        //                    var ammt = amount.ToString();
        //                    var request = new RestRequest(Method.POST);
        //                    //request.RequestFormat = DataFormat.Json;
        //                    request.AddHeader("content-type", "text/plain");
        //                    request.AddHeader("cache-control", "no-cache");
        //                    request.AddHeader("authorization", "bearer " + token);//OAUTH token
        //                    request.AddHeader("eskey", encryptUsingPublicKey);
        //                    request.AddHeader("deviceIMEI", string.IsNullOrWhiteSpace(DeviceSrNo) ? "352801082418919" : DeviceSrNo); //can pass Unique device Id
        //                    request.AddHeader("hash", Convert.ToBase64String(hash));
        //                    request.AddHeader("trnTimestamp", reqObject.timestamp);
        //                    request.AddHeader("agentid", agentid);
        //                    request.AddHeader("amount", ammt);
        //                    request.AddHeader("MerchantId", ouletid);
        //                    request.AddParameter("text/plain",
        //                        encryptUsingSessionKey, ParameterType.RequestBody);
        //                    IRestResponse response = client.Execute(request);
        //                    if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
        //                    {
        //                        TryLogin();
        //                    }
        //                    var respoJson = JsonConvert.DeserializeObject<AepsResponseJson>(response.Content);
        //                    //dynamic respoJson = JsonConvert.DeserializeObject("{\"Version\":\"1.0\",\"StatusCode\":200,\"Content\":{\"ResponseCode\":0,\"ADDINFO\":{\"status\":true,\"message\":\"Request Completed\",\"data\":{\"terminalId\":\"FA026069\",\"requestTransactionTime\":\"26\/09\/2018 10:26:58\",\"transactionAmount\":0.0,\"transactionStatus\":\"successful\",\"balanceAmount\":4408.83,\"bankRRN\":\"826910115647\",\"transactionType\":\"BE\",\"fpTransactionId\":\"826910115647\"},\"statusCode\":10000}}}");
        //                    //if (response.StatusCode != HttpStatusCode.OK || string.IsNullOrWhiteSpace(response.Content))
        //                    //{
        //                    //    if (type == "CW")
        //                    //    {
        //                    //        System.Data.Entity.Core.Objects.ObjectParameter procStatus = new System.Data.Entity.Core.Objects.ObjectParameter("ProcStatus", typeof(string));
        //                    //        System.Data.Entity.Core.Objects.ObjectParameter procMessage = new System.Data.Entity.Core.Objects.ObjectParameter("ProcMessage", typeof(string));
        //                    //        db.whitelabel_proc_AEPS_PostProcess(userid,reqObject.merchantTransactionId, "NA", null, "Failed", response.Content ?? "", "web","Other than status code OK",procStatus,procMessage);
        //                    //    }
        //                    //    var aa = new { Status = "Failed", Message = "Failed from bank." };
        //                    //    TempData["Respo"] = JsonConvert.SerializeObject(aa);
        //                    //    return RedirectToAction("AEPS");
        //                    //}
        //                    //else
        //                    //{
        //                    try
        //                    {
        //                        var bal = ""; string rrn = "";
        //                        if (respoJson.Content.ADDINFO.StatusCode == 10000)
        //                        {
        //                            var respchk = (dynamic)null;
        //                            var date = DateTime.Now.ToString();
        //                            if (type == "BE")
        //                            {
        //                                rrn = Convert.ToString(respoJson.Content.ADDINFO.Data.BankRrn);
        //                                bal = Convert.ToString(respoJson.Content.ADDINFO.Data.BalanceAmount);
        //                                respchk = new
        //                                {
        //                                    RequestTransactionTime = date,
        //                                    BankRrn = rrn,
        //                                    TransactionAmount = "0",
        //                                    TransactionStatus = "Success",
        //                                    BalanceAmount = bal,
        //                                    Bank = bank
        //                                };
        //                                try
        //                                {
        //                                    decimal rembal1 = Convert.ToDecimal(bal);
        //                                    db.Whitelabel_Aeps_check_balance(userid, agentid, uid, bank, rrn, remark, "Web", mobile, "", rembal1);
        //                                }
        //                                catch { }
        //                                var dd = new { Status = "Success", Message = respchk, userinfo = reminfo };
        //                                //TempData["Respo"] = JsonConvert.SerializeObject(dd);
        //                                //return RedirectToAction("AEPS");
        //                                return Json(dd, JsonRequestBehavior.AllowGet);
        //                            }
        //                            else if (type == "CW")
        //                            {
        //                                string terminalid = Convert.ToString(respoJson.Content.ADDINFO.Data.TerminalId);
        //                                rrn = Convert.ToString(respoJson.Content.ADDINFO.Data.BankRrn);
        //                                bal = Convert.ToString(respoJson.Content.ADDINFO.Data.BalanceAmount);
        //                                respchk = new
        //                                {
        //                                    BankRrn = rrn,
        //                                    BalanceAmount = bal,
        //                                    RequestTransactionTime = date,
        //                                    TransactionAmount = amount,
        //                                    TransactionStatus = "Success",
        //                                    Bank = bank
        //                                };
        //                                System.Data.Entity.Core.Objects.ObjectParameter procStatus = new System.Data.Entity.Core.Objects.ObjectParameter("ProcStatus", typeof(string));
        //                                System.Data.Entity.Core.Objects.ObjectParameter procMessage = new System.Data.Entity.Core.Objects.ObjectParameter("ProcMessage", typeof(string));
        //                                decimal remain = 0;
        //                                try
        //                                {
        //                                    remain = Convert.ToDecimal(bal);
        //                                }
        //                                catch { }
        //                                db.proc_whitelabel_AEPS_PostProcess(userid, reqObject.merchantTransactionId, rrn, terminalid, "Success", response.Content, "web", "", remain, procStatus, procMessage);
        //                                var dd = new { Status = "Success", Message = respchk, userinfo = reminfo };
        //                                return Json(dd, JsonRequestBehavior.AllowGet);
        //                            }
        //                            else if (type == "SAP")
        //                            {
        //                                rrn = Convert.ToString(respoJson.Content.ADDINFO.Data.BankRrn);
        //                                bal = Convert.ToString(respoJson.Content.ADDINFO.Data.BalanceAmount);
        //                                respchk = new
        //                                {
        //                                    RequestTransactionTime = date,
        //                                    BankRrn = rrn,
        //                                    TransactionAmount = "0",
        //                                    TransactionStatus = "Success",
        //                                    BalanceAmount = bal,
        //                                    Bank = bank
        //                                };
        //                                try
        //                                {
        //                                    decimal rembal1 = 0;
        //                                    try
        //                                    {
        //                                        rembal1 = Convert.ToDecimal(bal);
        //                                    }
        //                                    catch { }
        //                                    db.update_Aeps_miniStatement(agentid, "M_Success", rrn, rembal1);
        //                                }
        //                                catch { }

        //                                dynamic respchk1 = JsonConvert.DeserializeObject(response.Content);
        //                                var serdata = JsonConvert.SerializeObject(respchk1.Content.ADDINFO.data);
        //                                var dd = new { Status = "Success", Message = serdata, userinfo = reminfo };
        //                                //TempData["Respo"] = JsonConvert.SerializeObject(dd);
        //                                //return RedirectToAction("AEPS");
        //                                return Json(dd, JsonRequestBehavior.AllowGet);
        //                            }
        //                            else
        //                            {
        //                                var dd = new { Status = "Failed", Message = "Please Try Again.", userinfo = reminfo };
        //                                return Json(dd, JsonRequestBehavior.AllowGet);
        //                            }
        //                        }
        //                        else
        //                        {
        //                            decimal remain = 0;
        //                            try
        //                            {
        //                                remain = Convert.ToDecimal(bal);
        //                            }
        //                            catch { }
        //                            if (type == "CW")
        //                            {
        //                                var msg = respoJson.Content.ADDINFO.Message.ToString();
        //                                System.Data.Entity.Core.Objects.ObjectParameter procStatus = new System.Data.Entity.Core.Objects.ObjectParameter("ProcStatus", typeof(string));
        //                                System.Data.Entity.Core.Objects.ObjectParameter procMessage = new System.Data.Entity.Core.Objects.ObjectParameter("ProcMessage", typeof(string));
        //                                db.proc_whitelabel_AEPS_PostProcess(userid, reqObject.merchantTransactionId, msg, null, "Failed", response.Content, "web", msg, remain, procStatus, procMessage);
        //                            }
        //                            else if (type == "SAP")
        //                            {
        //                                var msg = respoJson.Content.ADDINFO.Message.ToString();
        //                                try
        //                                {
        //                                    decimal rembal1 = Convert.ToDecimal(0);
        //                                    db.update_Aeps_miniStatement(agentid, "M_Failed", msg, rembal1);
        //                                }
        //                                catch { }
        //                                var dd = new { Status = "Failed", Message = msg, userinfo = reminfo };
        //                                //TempData["Respo"] = JsonConvert.SerializeObject(dd);
        //                                //return RedirectToAction("AEPS
        //                                return Json(dd, JsonRequestBehavior.AllowGet);
        //                            }
        //                            var viewresponse = new { Status = "Failed", Message = respoJson.Content.ADDINFO.Message, userinfo = reminfo };
        //                            return Json(viewresponse, JsonRequestBehavior.AllowGet);
        //                        }
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        var viewresponse = new { Status = "Failed", Message = "Something went wrong. Kindly check transaction status from AEPS report.", userinfo = reminfo };
        //                        return Json(viewresponse, JsonRequestBehavior.AllowGet);
        //                    }
        //                    //}
        //                }
        //                else
        //                {
        //                    var viewresponse = new { Status = "Failed", Message = "Complete Your Profile", userinfo = reminfo };
        //                    return Json(viewresponse, JsonRequestBehavior.AllowGet);
        //                }
        //            }
        //            else
        //            {
        //                var viewresponse = new { Status = "Failed", Message = "Please Scan Again.", userinfo = "" };
        //                return Json(viewresponse, JsonRequestBehavior.AllowGet);

        //            }
        //        }
        //        else
        //        {
        //            var viewresponse = new { Status = "Failed", Message = "Either mobile or aadhaar is invalid.", userinfo = "" };
        //            return Json(viewresponse, JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        var viewresponse = new { Status = "Failed", Message = "Invalid request", userinfo = "" };
        //        return Json(viewresponse, JsonRequestBehavior.AllowGet);
        //    }
        //}

        #endregion

        #region MPOS
        public ActionResult MPOS(string tabvalue)
        {
            var userid = User.Identity.GetUserId();
            var ch = db.Whitelabel_Retailer_Details.Where(a => a.RetailerId == userid).FirstOrDefault();
            ViewBag.psncardsts = ch.PSAStatus;
            ViewBag.aepsandmpos = ch.AepsMposstatus;
            ViewBag.tab = tabvalue;
            return View();
        }
        public ActionResult MPOSservice()
        {

            var status = ""; var message = "";
            var userid = User.Identity.GetUserId();
            var remdetails = db.Whitelabel_Retailer_Details.Where(aa => aa.RetailerId == userid).SingleOrDefault();
            if (remdetails.AepsMposstatus == "Y")
            {
                var check = "OK";
                if (check == "OK")
                {
                    if (remdetails.PartnerID != "" && remdetails.PartnerID != null)
                    {
                        status = "SUCCESS";
                        message = remdetails.PartnerID;

                    }
                    else
                    {
                        status = "UNREGISTER";
                        message = "Mpos Device Id is Missing";
                    }

                }

            }
            else
            {
                status = "FAILED";
                message = "Your Mpos Status is Inactive Conatct Admin.";
                //sts off
            }
            var results = "{'status':'" + status + "','msg':'" + message + "'}";
            var json1 = JsonConvert.DeserializeObject(results);
            var json = JsonConvert.SerializeObject(json1);
            return Json(json, JsonRequestBehavior.AllowGet);
        }
        #endregion



        #region Gift Card
        public ActionResult Gift_Card(string tabvalue)
        {
            ViewBag.tab = tabvalue;
            ViewBag.Apparel = db.GiftCards.Where(p => p.Category == "Apparel").ToList();
            ViewBag.Accessories = db.GiftCards.Where(p => p.Category == "Accessories").ToList();
            ViewBag.HealthBeauty = db.GiftCards.Where(p => p.Category == "HealthBeauty").ToList();
            ViewBag.Restaurants = db.GiftCards.Where(p => p.Category == "Restaurants").ToList();
            ViewBag.Travel = db.GiftCards.Where(p => p.Category == "Travel").ToList();
            ViewBag.Electronics = db.GiftCards.Where(p => p.Category == "Electronics").ToList();
            return View();
        }
        #endregion


        #region MONEYTRANSFER DMT 2
        [HttpGet]
        /// <summary>
        /// [GET] Displays DMT (Domestic Money Transfer) transaction page
        /// </summary>
        public ActionResult Money_transfer()
        {
            string userid = User.Identity.GetUserId();

            var ChkKYC = db.Whitelabel_Retailer_Details.Where(aa => aa.RetailerId == userid).SingleOrDefault();
            if (ChkKYC.PSAStatus == "Y" && ChkKYC.AadhaarStatus == "Y")
            {
                var isvalid = true;
                var checkekyc = db.whitelabel_ekycCheck.Where(aa => aa.userid == userid).SingleOrDefault();
                if (checkekyc == null)
                {
                    isvalid = false;
                    ViewBag.req = "REQUIREDOTP";
                }
                else
                {
                    var sts = checkekyc.isvalid;
                    if (sts == false)
                    {
                        isvalid = false;
                        ViewBag.req = "REQUIREDSCAN";
                    }
                }

                ViewBag.chkcharge = "DONE";

                Recent_report recent = new Recent_report();
                recent.Recent_report_imps = db.recent_imps_report(userid).ToList();
                recent.Recent_report_Aeps = null;
                recent.Recent_PAN_CARD_IPAY = null;
                recent.Recent_mPosInfo = null;
                return View(recent);
            }
            else
            {
                ViewBag.ChkKYC = "Firstly Complete Your Full KYC !";
                return RedirectToAction("Profile");
            }

        }

        public ActionResult GenrateOtp()
        {
            var userid = User.Identity.GetUserId();
            var token = string.Empty;
            token = getAuthToken();
            var whitelabel_retailer = db.Whitelabel_Retailer_Details.SingleOrDefault(a => a.RetailerId == userid);
            var city = db.District_Desc.Where(aa => aa.State_id == whitelabel_retailer.State && aa.Dist_id == whitelabel_retailer.District).SingleOrDefault().Dist_Desc;
            string lattitude = string.Empty;
            string longitude = string.Empty;
            var infocheck = db.Whitelabel_UserLocation.Where(aa => aa.RetailerId == userid).SingleOrDefault();
            if (infocheck == null)
            {
                insertGeoLocation(whitelabel_retailer.RetailerId, out lattitude, out longitude);
            }
            else
            {
                lattitude = infocheck.Lattitude;
                longitude = infocheck.Longitute;
            }
            if (string.IsNullOrEmpty(whitelabel_retailer.AepsMerchandId))
            {
                var reque = new
                {
                    merchantName = whitelabel_retailer.RetailerName,
                    stateid = whitelabel_retailer.State,
                    latitude = lattitude,
                    longitude = longitude,
                    merchantPhoneNumber = whitelabel_retailer.Mobile,
                    merchantPinCode = whitelabel_retailer.Pincode,
                    merchantCityName = city,
                    merchantAddress = whitelabel_retailer.Address,
                    userPan = whitelabel_retailer.PanCard,
                    retilerid = whitelabel_retailer.Email,
                    OTP = ""
                };
                var resquestchk = JsonConvert.SerializeObject(reque);
                var client2 = new RestClient("http://api.vastbazaar.com/api/AEPS/RegisterAEPS");
                client2.Timeout = -1;
                var request2 = new RestRequest(Method.POST);
                request2.AddHeader("Authorization", "Bearer " + token);
                request2.AddHeader("Content-Type", "application/json");

                request2.AddParameter("application/json", resquestchk, ParameterType.RequestBody);
                IRestResponse response2 = client2.Execute(request2);
                dynamic resp = JsonConvert.DeserializeObject(response2.Content);
                var stscode2 = resp.Content.ADDINFO.statuscode.ToString();
                var message = resp.Content.ADDINFO.status.ToString();
                if (stscode2 == "TXN")
                {
                    var ouletid = resp.Content.ADDINFO.data.outlet_id.ToString();
                    var pin = resp.Content.ADDINFO.data.pin.ToString();
                    whitelabel_retailer.AepsMerchandId = ouletid;
                    whitelabel_retailer.AepsMPIN = pin;
                    db.SaveChanges();
                    whitelabel_retailer = db.Whitelabel_Retailer_Details.SingleOrDefault(a => a.RetailerId == userid);
                }
                else
                {
                    var viewresponse = new { Status = "Failed", Message = message };
                    return Json(viewresponse, JsonRequestBehavior.AllowGet);
                }
            }
            var client1 = new RestClient("http://api.vastbazaar.com/api/AEPS/supermerchant?merchant=" + whitelabel_retailer.AepsMerchandId + "");
            var request1 = new RestRequest(Method.POST);
            request1.AddHeader("Authorization", "Bearer " + token);
            IRestResponse response1 = client1.Execute(request1);
            var respsuper = response1.Content;
            dynamic chkresp = JsonConvert.DeserializeObject(respsuper);
            var stscode = chkresp.Content.ADDINFO.stscode.ToString();
            if (stscode == "0")
            {
                var msg = chkresp.Content.ADDINFO.message.ToString();
                var viewresponse = new { Status = "Failed", Message = msg };
                return Json(viewresponse, JsonRequestBehavior.AllowGet);
            }
            var superid = chkresp.Content.ADDINFO.data.superid.ToString();
            var superusername = chkresp.Content.ADDINFO.data.superusername.ToString();
            AepsModel reqObject = new AepsModel();
            var req = new
            {
                superMerchantId = superid,
                merchantLoginId = whitelabel_retailer.AepsMerchandId,
                transactionType = "EKY",
                mobileNumber = whitelabel_retailer.Mobile,
                aadharNumber = whitelabel_retailer.AadharCard,
                panNumber = whitelabel_retailer.PanCard,
                matmSerialNumber = "",
                latitude = lattitude,
                longitude = longitude
            };
            dynamic RequestJson = JsonConvert.SerializeObject(req);
            byte[] hash = Main.generateSha256Hash(Encoding.ASCII.GetBytes(RequestJson));
            byte[] skey = Main.generateSessionKey();
            string encryptUsingSessionKey = Main.encryptUsingSessionKey(skey, Encoding.ASCII.GetBytes(RequestJson));
            string encryptUsingPublicKey = Main.encryptUsingPublicKey(skey);
            if (string.IsNullOrWhiteSpace(encryptUsingSessionKey) || string.IsNullOrWhiteSpace(encryptUsingPublicKey))
            {
                var viewresponse = new { Status = "Failed", Message = "Failed to initiate request." };
                return Json(viewresponse, JsonRequestBehavior.AllowGet);
            }
            var client = new RestClient();
            client = new RestClient(VastbazaarBaseUrl + "api/AEPS/ICICSendOtp");
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "text/plain");
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("authorization", "bearer " + token);//OAUTH token
            request.AddHeader("hash", Convert.ToBase64String(hash));
            request.AddHeader("deviceIMEI", "352801082418919"); //can pass Unique device Id
            request.AddHeader("eskey", encryptUsingPublicKey);
            request.AddHeader("trnTimestamp", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture));
            request.AddHeader("Requestdata", RequestJson);
            request.AddParameter("text/plain",
                encryptUsingSessionKey, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            dynamic resp1 = JsonConvert.DeserializeObject(response.Content);
            var msg1 = ""; var primarykey = 0; var encodetxnid = "";
            var status = "Failed";

            var sts = resp1.Content.ADDINFO.status;
            msg1 = resp1.Content.ADDINFO.message;
            if (sts == true)
            {
                var statusCode = resp1.Content.ADDINFO.statusCode;

                if (statusCode == 10000)
                {
                    status = "Success";
                    primarykey = resp1.Content.ADDINFO.data.primaryKeyId;
                    encodetxnid = resp1.Content.ADDINFO.data.encodeFPTxnId;
                }
            }

            var viewresponse1 = new { Status = status, Message = msg1, primarykey = primarykey, encodetxnid = encodetxnid };
            return Json(viewresponse1, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult VerifyotpEkyc(string otp, string primaryhide, string encodehide)
        {
            var userid = User.Identity.GetUserId();
            var token = string.Empty;
            token = getAuthToken();
            var whitelabel_Retailer = db.Whitelabel_Retailer_Details.SingleOrDefault(a => a.RetailerId == userid);
            var city = db.District_Desc.Where(aa => aa.State_id == whitelabel_Retailer.State && aa.Dist_id == whitelabel_Retailer.District).SingleOrDefault().Dist_Desc;

            string lattitude = string.Empty;
            string longitude = string.Empty;
            var infocheck = db.Whitelabel_UserLocation.Where(aa => aa.RetailerId == userid).SingleOrDefault();
            if (infocheck == null)
            {
                insertGeoLocation(whitelabel_Retailer.RetailerId, out lattitude, out longitude);
            }
            else
            {
                lattitude = infocheck.Lattitude;
                longitude = infocheck.Longitute;
            }
            if (whitelabel_Retailer.AepsMerchandId == "")
            {
                var reque = new
                {
                    merchantName = whitelabel_Retailer.RetailerName,
                    stateid = whitelabel_Retailer.State,
                    latitude = lattitude,
                    longitude = longitude,
                    merchantPhoneNumber = whitelabel_Retailer.Mobile,
                    merchantPinCode = whitelabel_Retailer.Pincode,
                    merchantCityName = city,
                    merchantAddress = whitelabel_Retailer.Address,
                    userPan = whitelabel_Retailer.PanCard,
                    retilerid = whitelabel_Retailer.Email,
                    OTP = ""
                };
                var resquestchk = JsonConvert.SerializeObject(reque);
                var client2 = new RestClient("http://api.vastbazaar.com/api/AEPS/RegisterAEPS");
                client2.Timeout = -1;
                var request2 = new RestRequest(Method.POST);
                request2.AddHeader("Authorization", "Bearer " + token);
                request2.AddHeader("Content-Type", "application/json");

                request2.AddParameter("application/json", resquestchk, ParameterType.RequestBody);
                IRestResponse response2 = client2.Execute(request2);
                dynamic resp = JsonConvert.DeserializeObject(response2.Content);
                var stscode2 = resp.Content.ADDINFO.statuscode.ToString();
                var message = resp.Content.ADDINFO.status.ToString();
                if (stscode2 == "TXN")
                {
                    var ouletid = resp.Content.ADDINFO.data.outlet_id.ToString();
                    var pin = resp.Content.ADDINFO.data.pin.ToString();
                    whitelabel_Retailer.AepsMerchandId = ouletid;
                    whitelabel_Retailer.AepsMPIN = pin;
                    db.SaveChanges();
                    whitelabel_Retailer = db.Whitelabel_Retailer_Details.SingleOrDefault(a => a.RetailerId == userid);
                }
                else
                {
                    var viewresponse = new { Status = "Failed", Message = message };
                    return Json(viewresponse, JsonRequestBehavior.AllowGet);
                }
            }
            var client1 = new RestClient("http://api.vastbazaar.com/api/AEPS/supermerchant?merchant=" + whitelabel_Retailer.AepsMerchandId + "");
            var request1 = new RestRequest(Method.POST);
            request1.AddHeader("Authorization", "Bearer " + token);
            IRestResponse response1 = client1.Execute(request1);
            var respsuper = response1.Content;
            dynamic chkresp = JsonConvert.DeserializeObject(respsuper);
            var stscode = chkresp.Content.ADDINFO.stscode.ToString();
            if (stscode == "0")
            {
                var msg = chkresp.Content.ADDINFO.message.ToString();
                var viewresponse = new { Status = "Failed", Message = msg };
                return Json(viewresponse, JsonRequestBehavior.AllowGet);
            }
            var superid = chkresp.Content.ADDINFO.data.superid.ToString();
            var superusername = chkresp.Content.ADDINFO.data.superusername.ToString();
            AepsModel reqObject = new AepsModel();
            var req = new
            {
                superMerchantId = superid,
                merchantLoginId = whitelabel_Retailer.AepsMerchandId,
                otp = otp,
                primaryKeyId = primaryhide,
                encodeFPTxnId = encodehide
            };
            dynamic RequestJson = JsonConvert.SerializeObject(req);
            byte[] hash = Main.generateSha256Hash(Encoding.ASCII.GetBytes(RequestJson));
            byte[] skey = Main.generateSessionKey();
            string encryptUsingSessionKey = Main.encryptUsingSessionKey(skey, Encoding.ASCII.GetBytes(RequestJson));
            string encryptUsingPublicKey = Main.encryptUsingPublicKey(skey);
            if (string.IsNullOrWhiteSpace(encryptUsingSessionKey) || string.IsNullOrWhiteSpace(encryptUsingPublicKey))
            {
                var viewresponse = new { Status = "Failed", Message = "Failed to initiate request." };
                return Json(viewresponse, JsonRequestBehavior.AllowGet);
            }
            var client = new RestClient();
            client = new RestClient(VastbazaarBaseUrl + "api/AEPS/ICICValidateOTP");
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "text/plain");
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("authorization", "bearer " + token);//OAUTH token
            request.AddHeader("hash", Convert.ToBase64String(hash));
            request.AddHeader("deviceIMEI", "352801082418919"); //can pass Unique device Id
            request.AddHeader("eskey", encryptUsingPublicKey);
            request.AddHeader("trnTimestamp", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture));
            request.AddHeader("Requestdata", RequestJson);
            request.AddParameter("text/plain",
                encryptUsingSessionKey, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            dynamic resp1 = JsonConvert.DeserializeObject(response.Content);
            var status = "Failed";
            var msg1 = "";
            var sts = resp1.Content.ADDINFO.status;
            msg1 = resp1.Content.ADDINFO.message;
            if (sts == true)
            {
                var statusCode = resp1.Content.ADDINFO.statusCode;

                if (statusCode == 10000)
                {
                    status = "Success";
                    var primaryKeyId = resp1.Content.ADDINFO.data.primaryKeyId;
                    var encodeFPTxnId = resp1.Content.ADDINFO.data.encodeFPTxnId;
                    var chk = db.whitelabel_ekycCheck.Where(aa => aa.userid == userid).SingleOrDefault();
                    if (chk == null)
                    {
                        whitelabel_ekycCheck ekyc = new whitelabel_ekycCheck();
                        ekyc.userid = userid;
                        ekyc.aadharcard = whitelabel_Retailer.AadharCard;
                        ekyc.devicename = "";
                        ekyc.devicesrno = "";
                        ekyc.whitelabelid = whitelabel_Retailer.Whitelabelid;
                        ekyc.encodeFPTxnId = encodeFPTxnId;
                        ekyc.mobilenumber = whitelabel_Retailer.Mobile;
                        ekyc.insertdate = DateTime.Now;
                        ekyc.updatedate = DateTime.Now;
                        ekyc.isvalid = false;
                        ekyc.primaryKeyId = Convert.ToInt32(primaryKeyId);
                        db.whitelabel_ekycCheck.Add(ekyc);
                        db.SaveChanges();
                    }
                    else
                    {
                        chk.aadharcard = whitelabel_Retailer.AadharCard;
                        chk.encodeFPTxnId = encodeFPTxnId;
                        chk.primaryKeyId = Convert.ToInt32(primaryKeyId);
                        chk.insertdate = DateTime.Now;
                        chk.updatedate = DateTime.Now;
                        db.SaveChanges();
                    }
                }
            }

            var viewresponse1 = new { Status = status, Message = msg1 };
            return Json(viewresponse1, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult ValidateDeviceinfo(string cap, string devicesrno, string devicenm)
        {
            var userid = User.Identity.GetUserId();
            var token = string.Empty;
            token = getAuthToken();
            var whitelabel_Retailer = db.Whitelabel_Retailer_Details.SingleOrDefault(a => a.RetailerId == userid);
            var city = db.District_Desc.Where(aa => aa.State_id == whitelabel_Retailer.State && aa.Dist_id == whitelabel_Retailer.District).SingleOrDefault().Dist_Desc;

            string lattitude = string.Empty;
            string longitude = string.Empty;
            var infocheck = db.Whitelabel_UserLocation.Where(aa => aa.RetailerId == userid).SingleOrDefault();
            if (infocheck == null)
            {
                insertGeoLocation(whitelabel_Retailer.RetailerId, out lattitude, out longitude);
            }
            else
            {
                lattitude = infocheck.Lattitude;
                longitude = infocheck.Longitute;
            }
            if (whitelabel_Retailer.AepsMerchandId == "")
            {
                var reque = new
                {
                    merchantName = whitelabel_Retailer.RetailerName,
                    stateid = whitelabel_Retailer.State,
                    latitude = lattitude,
                    longitude = longitude,
                    merchantPhoneNumber = whitelabel_Retailer.Mobile,
                    merchantPinCode = whitelabel_Retailer.Pincode,
                    merchantCityName = city,
                    merchantAddress = whitelabel_Retailer.Address,
                    userPan = whitelabel_Retailer.PanCard,
                    retilerid = whitelabel_Retailer.Email,
                    OTP = ""
                };
                var resquestchk = JsonConvert.SerializeObject(reque);
                var client2 = new RestClient("http://api.vastbazaar.com/api/AEPS/RegisterAEPS");
                client2.Timeout = -1;
                var request2 = new RestRequest(Method.POST);
                request2.AddHeader("Authorization", "Bearer " + token);
                request2.AddHeader("Content-Type", "application/json");

                request2.AddParameter("application/json", resquestchk, ParameterType.RequestBody);
                IRestResponse response2 = client2.Execute(request2);
                dynamic resp = JsonConvert.DeserializeObject(response2.Content);
                var stscode2 = resp.Content.ADDINFO.statuscode.ToString();
                var message = resp.Content.ADDINFO.status.ToString();
                if (stscode2 == "TXN")
                {
                    var ouletid = resp.Content.ADDINFO.data.outlet_id.ToString();
                    var pin = resp.Content.ADDINFO.data.pin.ToString();
                    whitelabel_Retailer.AepsMerchandId = ouletid;
                    whitelabel_Retailer.AepsMPIN = pin;
                    db.SaveChanges();
                    whitelabel_Retailer = db.Whitelabel_Retailer_Details.SingleOrDefault(a => a.RetailerId == userid);
                }
                else
                {
                    var viewresponse = new { Status = "Failed", Message = message };
                    return Json(viewresponse, JsonRequestBehavior.AllowGet);
                }
            }
            var client1 = new RestClient("http://api.vastbazaar.com/api/AEPS/supermerchant?merchant=" + whitelabel_Retailer.AepsMerchandId + "");
            var request1 = new RestRequest(Method.POST);
            request1.AddHeader("Authorization", "Bearer " + token);
            IRestResponse response1 = client1.Execute(request1);
            var respsuper = response1.Content;
            dynamic chkresp = JsonConvert.DeserializeObject(respsuper);
            var stscode = chkresp.Content.ADDINFO.stscode.ToString();
            if (stscode == "0")
            {
                var msg = chkresp.Content.ADDINFO.message.ToString();
                var viewresponse = new { Status = "Failed", Message = msg };
                return Json(viewresponse, JsonRequestBehavior.AllowGet);
            }
            var superid = chkresp.Content.ADDINFO.data.superid.ToString();
            var superusername = chkresp.Content.ADDINFO.data.superusername.ToString();
            var chk = db.whitelabel_ekycCheck.Where(aa => aa.userid == userid).SingleOrDefault();
            if (chk == null)
            {
                var viewresponse = new { Status = "Failed", Message = "Please regenerate the OTP first then try again" };
                return Json(viewresponse, JsonRequestBehavior.AllowGet);
            }
            var req = new
            {
                superMerchantId = superid,
                merchantLoginId = whitelabel_Retailer.AepsMerchandId,
                primaryKeyId = chk.primaryKeyId,
                encodeFPTxnId = chk.encodeFPTxnId,
                requestRemarks = whitelabel_Retailer.Frm_Name,
                cardnumberORUID = new CardnumberOruid { adhaarNumber = whitelabel_Retailer.AadharCard, nationalBankIdentificationNumber = null },
                captureResponse = JsonConvert.DeserializeObject<CaptureResponse>(cap),
            };
            dynamic RequestJson = JsonConvert.SerializeObject(req);
            byte[] hash = Main.generateSha256Hash(Encoding.ASCII.GetBytes(RequestJson));
            byte[] skey = Main.generateSessionKey();
            string encryptUsingSessionKey = Main.encryptUsingSessionKey(skey, Encoding.ASCII.GetBytes(RequestJson));
            string encryptUsingPublicKey = Main.encryptUsingPublicKey(skey);
            if (string.IsNullOrWhiteSpace(encryptUsingSessionKey) || string.IsNullOrWhiteSpace(encryptUsingPublicKey))
            {
                var viewresponse = new { Status = "Failed", Message = "Failed to initiate request." };
                return Json(viewresponse, JsonRequestBehavior.AllowGet);
            }
            var client = new RestClient();
            client = new RestClient(VastbazaarBaseUrl + "api/AEPS/ICICIFingerverify");
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "text/plain");
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("authorization", "bearer " + token);//OAUTH token
            request.AddHeader("hash", Convert.ToBase64String(hash));
            request.AddHeader("deviceIMEI", devicesrno); //can pass Unique device Id
            request.AddHeader("eskey", encryptUsingPublicKey);
            request.AddHeader("trnTimestamp", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture));
            request.AddHeader("Requestdata", RequestJson);
            request.AddParameter("text/plain",
                encryptUsingSessionKey, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            dynamic resp1 = JsonConvert.DeserializeObject(response.Content);
            var status = "Failed";
            var msg1 = "";

            var sts = resp1.Content.ADDINFO.status;
            msg1 = resp1.Content.ADDINFO.message;
            var statusCode = 0;
            if (sts == true)
            {
                statusCode = resp1.Content.ADDINFO.statusCode;

                if (statusCode == 10000)
                {
                    status = "Success";
                    chk.devicename = devicenm;
                    chk.devicesrno = devicesrno;
                    chk.updatedate = DateTime.Now;
                    chk.isvalid = true;
                    db.SaveChanges();
                }
            }
            else if (sts == false)
            {
                statusCode = resp1.Content.ADDINFO.statusCode;
                if ((statusCode == 10005) || (statusCode == 10002))
                {
                    var ekycchk = db.whitelabel_ekycCheck.Where(aa => aa.userid == userid && aa.isvalid == false).SingleOrDefault();
                    if (ekycchk != null)
                    {
                        db.whitelabel_ekycCheck.Remove(ekycchk);
                    }
                    //Remove merchanat
                    whitelabel_Retailer.AepsMerchandId = "";
                    whitelabel_Retailer.AepsMPIN = "";

                    db.SaveChanges();
                }
            }

            var viewresponse1 = new { Status = status, Message = msg1 };
            return Json(viewresponse1, JsonRequestBehavior.AllowGet);

        }
        public ActionResult RetailerName()
        {
            var userid = User.Identity.GetUserId();
            var name = db.Whitelabel_Retailer_Details.Where(aa => aa.RetailerId == userid).SingleOrDefault().RetailerName;
            return Json(name, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Updateremname(string name)
        {
            var userid = User.Identity.GetUserId();
            var reminfo = db.Whitelabel_Retailer_Details.Where(aa => aa.RetailerId == userid).SingleOrDefault();
            reminfo.RetailerName = name;
            //Remove merchanat
            reminfo.AepsMerchandId = "";
            reminfo.AepsMPIN = "";

            var chk = db.whitelabel_ekycCheck.Where(aa => aa.userid == userid).SingleOrDefault();
            if (chk != null)
            {
                db.whitelabel_ekycCheck.Remove(chk);
            }
            db.SaveChanges();
            return Json("Update", JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Senderdetails(string senderno)
        {
            var check = "OK"; var errormsg = "";
            var apinm = db.money_api_status.Where(aa => aa.status == true && aa.catagory== "PAYOUT").SingleOrDefault();
            if (apinm != null)
            {
                if (apinm.api_name == "VASTWEB")
                {
                    string userid = User.Identity.GetUserId();
                    var remdetails = db.Whitelabel_Retailer_Details.Where(aa => aa.RetailerId == userid).SingleOrDefault();
                    var dlmdetails = db.whitelabel_Dealer_Details.Where(aa => aa.DealerId == remdetails.DealerId).SingleOrDefault();
                    if (dlmdetails.moneysts == true)
                    {
                        if (remdetails.moneysts == "Y")
                        {
                            var Token = string.Empty;
                            Token = GetToken();
                            var client = new RestClient(Vastwebmulti.Areas.WHITELABEL.Models.WapiBaseUrl.GetBaseUrl() + "/Money/api/Money/GetBeneficiaryList?sender_number=" + senderno + "");
                            var request = new RestRequest(Method.GET);
                            request.AddHeader("Content-Type", "application/json");
                            request.AddHeader("Authorization", "bearer " + Token + "");
                            IRestResponse response = client.Execute(request);
                            var responsechk = response.Content;

                            dynamic json = JsonConvert.DeserializeObject(responsechk);

                            var ADDINFO = json["ADDINFO"] == null ? json.Message : json.ADDINFO;

                            json = JsonConvert.SerializeObject(ADDINFO);
                            return Json(json, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            var error_decribe = "Money Status Off";
                            var results = "{'status':'" + error_decribe + "','statuscode':'ERR'}";
                            var json1 = JsonConvert.DeserializeObject(results);
                            var json = JsonConvert.SerializeObject(json1);
                            // var jss = new JavaScriptSerializer();
                            // var dict = jss.Deserialize<dynamic>(results);
                            return Json(json, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        var error_decribe = "Money Status Off, Contact Distributor OR Customer Care.";
                        var results = "{'status':'" + error_decribe + "','statuscode':'ERR'}";
                        var json1 = JsonConvert.DeserializeObject(results);
                        var json = JsonConvert.SerializeObject(json1);
                        //  var jss = new JavaScriptSerializer();
                        // var dict = jss.Deserialize<dynamic>(results);
                        return Json(json, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    var error_decribe = "NO API OPEN";
                    var results = "{'status':'" + error_decribe + "','statuscode':'ERR'}";
                    var json1 = JsonConvert.DeserializeObject(results);
                    var json = JsonConvert.SerializeObject(json1);
                    //       var jss = new JavaScriptSerializer();
                    //         var dict = jss.Deserialize<dynamic>(results);
                    return Json(json, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                var error_decribe = "NO API OPEN";
                var results = "{'status':'" + error_decribe + "','statuscode':'ERR'}";
                var json1 = JsonConvert.DeserializeObject(results);
                var json = JsonConvert.SerializeObject(json1);
                //   var jss = new JavaScriptSerializer();
                //  var dict = jss.Deserialize<dynamic>(results);
                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult BindBankDdllist()
        {
            string userid = User.Identity.GetUserId();
            var ChkKYC = db.Whitelabel_Retailer_Details.Where(aa => aa.RetailerId == userid).SingleOrDefault();
            if (ChkKYC.PSAStatus == "Y" && ChkKYC.AadhaarStatus == "Y" && ChkKYC.ShopwithSalfieStatus == "Y")
            {
                try
                {
                    var Token = string.Empty;
                    Token = GetToken();
                    var client = new RestClient(Vastwebmulti.Areas.WHITELABEL.Models.WapiBaseUrl.GetBaseUrl() + "/Money/api/Money/MasterIfsc?AccountNo=123");
                    var request = new RestRequest(Method.GET);
                    request.AddHeader("Content-Type", "application/json");
                    request.AddHeader("Authorization", "bearer " + Token + "");
                    IRestResponse response = client.Execute(request);

                    var responsechk = response.Content;
                    dynamic json = JsonConvert.DeserializeObject(responsechk);
                    JArray a = (JArray)json["data"];
                    IList<BankNameModel> Details = a.ToObject<IList<BankNameModel>>();
                    List<SelectListItem> customers = new List<SelectListItem>();

                    foreach (var item in Details)
                    {
                        customers.Add(new SelectListItem
                        {
                            Value = item.branch_ifsc,
                            Text = item.bank_name

                        });
                    }

                    return Json(new { data = customers }, JsonRequestBehavior.AllowGet);
                }
                catch { return Json("", JsonRequestBehavior.AllowGet); }
            }
            else
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult Verify_account(string NUMBER, string account, string benIFSC, string bankname)
        {
            try
            {
                string userid = User.Identity.GetUserId();

                var Token = string.Empty;
                Token = GetToken();
                var client = new RestClient(Vastwebmulti.Areas.WHITELABEL.Models.WapiBaseUrl.GetBaseUrl() + "/Money/api/Money/mmmn2?mb=" + NUMBER + "&iff=" + benIFSC + "&cd={cd}&bno=" + account + "&mal=" + userid + "&sk={sk}&pi={pi}&kk={kk}&bankName=" + bankname + "&kyc={kyc}&AppType=ONLINE");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", "bearer " + Token + "");
                IRestResponse response = client.Execute(request);
                var responsechk = response.Content;
                dynamic json = JsonConvert.DeserializeObject(responsechk);

                var ADDINFO = json.ADDINFO;
                var ser = JsonConvert.SerializeObject(ADDINFO);
                var jss1 = new JavaScriptSerializer();
                var dict1 = jss1.Deserialize<dynamic>(ser);
                return Json(dict1, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult Verify_account_again(string NUMBER, string account, string benIFSC, string bankname)
        {
            try
            {
                string userid = User.Identity.GetUserId();

                var Token = string.Empty;
                Token = GetToken();
                var client = new RestClient(Vastwebmulti.Areas.WHITELABEL.Models.WapiBaseUrl.GetBaseUrl() + "/Money/api/Money/mmmn2Again?mb=" + NUMBER + "&iff=" + benIFSC + "&cd={cd}&bno=" + account + "&mal=" + userid + "&sk={sk}&pi={pi}&kk={kk}&bankName=" + bankname + "&kyc={kyc}");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", "bearer " + Token + "");
                IRestResponse response = client.Execute(request);
                var responsechk = response.Content;
                dynamic json = JsonConvert.DeserializeObject(responsechk);

                var ADDINFO = json.ADDINFO;
                var ser = JsonConvert.SerializeObject(ADDINFO);
                var jss1 = new JavaScriptSerializer();
                var dict1 = jss1.Deserialize<dynamic>(ser);
                return Json(dict1, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        /// <summary>
        /// [POST] Registers a new DMT beneficiary
        /// </summary>
        public ActionResult Register_ben(string senderno, string account, string ifsccode, string originalifsccode, string benname)
        {
            try
            {
                string userid = User.Identity.GetUserId();

                var Token = string.Empty;
                Token = GetToken();
                var client = new RestClient(Vastwebmulti.Areas.WHITELABEL.Models.WapiBaseUrl.GetBaseUrl() + "/Money/api/Money/AddNewRecipient?Name=" + benname + "&AccountNo=" + account + "&IFSC=" + ifsccode + "&Mobile=" + senderno + "&SenderNo=" + senderno + "&remitterid=&ifscoriginal=" + originalifsccode + "");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", "bearer " + Token + "");
                IRestResponse response = client.Execute(request);
                var responsechk = response.Content;
                dynamic json = JsonConvert.DeserializeObject(responsechk);

                var ADDINFO = json.ADDINFO;
                var ser = JsonConvert.SerializeObject(ADDINFO);
                var jss1 = new JavaScriptSerializer();
                var dict1 = jss1.Deserialize<dynamic>(ser);
                return Json(dict1, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                var results = "{'Message':'Something Went Wrong !!!','Response':'failure'}";
                var jss = new JavaScriptSerializer();
                var dict = jss.Deserialize<dynamic>(results);
                return Json(dict, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        /// <summary>
        /// [POST] Registers a new DMT sender
        /// </summary>
        public ActionResult Register_sender(string senderno, string name)
        {
            try
            {
                string userid = User.Identity.GetUserId();

                var Token = string.Empty;
                Token = GetToken();
                var client = new RestClient(Vastwebmulti.Areas.WHITELABEL.Models.WapiBaseUrl.GetBaseUrl() + "/Money/api/Money/RegisterMobileForIMPS?Name=" + name + "&Mobile=" + senderno + "&surname=Singh&pincode=332311");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", "bearer " + Token + "");
                IRestResponse response = client.Execute(request);
                var responsechk = response.Content;
                dynamic json = JsonConvert.DeserializeObject(responsechk);

                var ADDINFO = json.ADDINFO;
                var ser = JsonConvert.SerializeObject(ADDINFO);
                var jss1 = new JavaScriptSerializer();
                var dict1 = jss1.Deserialize<dynamic>(ser);
                return Json(dict1, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                var results = "{'Message':'Something Went Wrong !!!','Response':'failure'}";
                var jss = new JavaScriptSerializer();
                var dict = jss.Deserialize<dynamic>(results);
                return Json(dict, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult Otp_verify_sender(string senderno, string otp, string benid)
        {
            try
            {
                string userid = User.Identity.GetUserId();

                var Token = string.Empty;
                Token = GetToken();
                var client = new RestClient(Vastwebmulti.Areas.WHITELABEL.Models.WapiBaseUrl.GetBaseUrl() + "/Money/api/Money/VerifiyOTP_ToAddNewMobile?Mobile=" + senderno + "&OTP=" + otp + "&RequestId={RequestId}&remitterid=" + benid + "&beneficiaryid=" + benid + "&Action=addben");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", "bearer " + Token + "");
                IRestResponse response = client.Execute(request);
                var responsechk = response.Content;
                dynamic json = JsonConvert.DeserializeObject(responsechk);

                var ADDINFO = json.ADDINFO;
                var ser = JsonConvert.SerializeObject(ADDINFO);
                var jss1 = new JavaScriptSerializer();
                var dict1 = jss1.Deserialize<dynamic>(ser);
                return Json(dict1, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                var results = "{'Message':'Something Went Wrong !!!','Response':'failure'}";
                var jss = new JavaScriptSerializer();
                var dict = jss.Deserialize<dynamic>(results);
                return Json(dict, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult Delete_ben(string benid, string senderno)
        {
            try
            {
                string userid = User.Identity.GetUserId();

                var Token = string.Empty;
                Token = GetToken();
                var client = new RestClient(Vastwebmulti.Areas.WHITELABEL.Models.WapiBaseUrl.GetBaseUrl() + "/Money/api/Money/DeleteReceipent?remitterid=&beneficiaryid=" + benid + "&mobile=" + senderno + "&ifsc=&code=");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", "bearer " + Token + "");
                IRestResponse response = client.Execute(request);
                var responsechk = response.Content;

                dynamic json = JsonConvert.DeserializeObject(responsechk);
                var ADDINFO = json.ADDINFO.Value;
                var jss = new JavaScriptSerializer();
                var dict = jss.Deserialize<dynamic>(ADDINFO);
                return Json(dict, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                var results = "{'Message':'Something Went Wrong !!!','Response':'failure'}";
                var jss = new JavaScriptSerializer();
                var dict = jss.Deserialize<dynamic>(results);
                return Json(dict, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult Imps_check_transfer(string dmtpin, string account, string amount)
        {
            string userid = User.Identity.GetUserId();
            var pin = Encrypt(dmtpin);
            var pin_check = (from pi in db.Whitelabel_Retailer_Details where pi.RetailerId == userid select pi).Single().PIN;

            if (pin_check == pin)
            {
                decimal finalamount = Convert.ToDecimal(amount);
                var ch1 = db.IMPS_transtion_detsils.Where(aa => aa.accountno == account && aa.rch_from == userid && aa.totalamount == finalamount && aa.Status.ToUpper() == "SUCCESS").OrderByDescending(aa => aa.idno).ToList();
                var date = ch1.Any() ? ch1.FirstOrDefault().trans_time : System.DateTime.Now.AddDays(-1);
                int ggg = Convert.ToInt32((System.DateTime.Now - Convert.ToDateTime(date)).TotalSeconds);
                if (ggg >= 180)
                {
                    var remain = (from mon in db.Whitelabel_Remain_reteller_balance where mon.RetellerId == userid select mon).Single().Remainamount;
                    if (remain >= finalamount)
                    {
                        var apinm = db.money_api_status.Where(aa => aa.status == true && aa.catagory== "PAYOUT").SingleOrDefault();
                        if (apinm.api_name != "NO")
                        {
                            int amt = Convert.ToInt32(amount);
                            if (amt <= 50000)
                            {
                                if (amt >= 100)
                                {
                                    var results = "{'Details':'','status':'Success' }";
                                    var jss1 = new JavaScriptSerializer();
                                    var dict1 = jss1.Deserialize<dynamic>(results);
                                    return Json(dict1, JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    var results = "{'Details':' Amount Should be Greater Than Rs. 100','status':'Failed' }";
                                    var jss1 = new JavaScriptSerializer();
                                    var dict1 = jss1.Deserialize<dynamic>(results);
                                    return Json(dict1, JsonRequestBehavior.AllowGet);
                                }
                            }
                            else
                            {
                                var results = "{'Details':' Amount Should be Less Rs 50000','status':'Failed' }";
                                var jss1 = new JavaScriptSerializer();
                                var dict1 = jss1.Deserialize<dynamic>(results);
                                return Json(dict1, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            var results = "{'Details':'No api Open.','status':'Failed'}";
                            var jss1 = new JavaScriptSerializer();
                            var dict1 = jss1.Deserialize<dynamic>(results);
                            return Json(dict1, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        var results = "{'Details':'Remain Amount Low','status':'Failed' }";
                        var jss1 = new JavaScriptSerializer();
                        var dict1 = jss1.Deserialize<dynamic>(results);
                        return Json(dict1, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    var results = "{'Details':'Please Wait 5 Minutes... Same Amount Not Transfer in same Account','status':'Failed' }";
                    var jss1 = new JavaScriptSerializer();
                    var dict1 = jss1.Deserialize<dynamic>(results);
                    return Json(dict1, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                var results = "{'Details':'Wrong Pin!!! Please Enter Correct Pin!!!','status':'Failed'}";
                var jss1 = new JavaScriptSerializer();
                var dict1 = jss1.Deserialize<dynamic>(results);
                return Json(dict1, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult imps_transfer(string NUMBER, string type, string account, string ifsc, string dmtpin, string amount, string bankname, string benCode, decimal servicefee, string idprooftype, string idproofnumber)
        {
            try
            {
                string userid = User.Identity.GetUserId();

                var Token = string.Empty;
                Token = GetToken();

                int _min = 1000;
                int _max = 9999;
                Random _rdm = new Random();
                var otp = _rdm.Next(_min, _max).ToString();
                MobileOtp mobile = new MobileOtp();
                mobile.Date = DateTime.Now;
                mobile.mobileno = NUMBER;
                mobile.Otp = otp;
                mobile.Type = "PaymentConfirmation";
                mobile.Userid = userid;
                db.MobileOtps.Add(mobile);
                db.SaveChanges();

                ENCRYPTIONMODELS cnctype = new ENCRYPTIONMODELS();
                var value1 = "12345678901234567890123456789012";
                var value2 = "0987654321098765";

                value1 = ENCRYPTIONMODELS.Base64Encode("12345678901234567890123456789012");
                value2 = ENCRYPTIONMODELS.Base64Encode("0987654321098765");
                var keyidsssenckey = ENCRYPTIONMODELS.Base64Decode(value1);
                var encIVIDS = ENCRYPTIONMODELS.Base64Decode(value2);

                userid = cnctype.convertdatatoALL(userid, keyidsssenckey, encIVIDS);
                NUMBER = cnctype.convertdatatoALL(NUMBER, keyidsssenckey, encIVIDS);
                ifsc = cnctype.convertdatatoALL(ifsc, keyidsssenckey, encIVIDS);
                benCode = cnctype.convertdatatoALL(benCode, keyidsssenckey, encIVIDS);
                dmtpin = cnctype.convertdatatoALL(dmtpin, keyidsssenckey, encIVIDS);
                account = cnctype.convertdatatoALL(account, keyidsssenckey, encIVIDS);
                type = cnctype.convertdatatoALL(type, keyidsssenckey, encIVIDS);
                bankname = cnctype.convertdatatoALL(bankname, keyidsssenckey, encIVIDS);
                var nbb = cnctype.convertdatatoALL("nbb", keyidsssenckey, encIVIDS);
                var Latitude = cnctype.convertdatatoALL("Latitude", keyidsssenckey, encIVIDS);
                var Longitude = cnctype.convertdatatoALL("Longitude", keyidsssenckey, encIVIDS);
                var ModelNo = cnctype.convertdatatoALL("ModelNo", keyidsssenckey, encIVIDS);
                var City = cnctype.convertdatatoALL("City", keyidsssenckey, encIVIDS);
                var PostalCode = cnctype.convertdatatoALL("PostalCode", keyidsssenckey, encIVIDS);
                var InternetTYPE = cnctype.convertdatatoALL("InternetTYPE", keyidsssenckey, encIVIDS);
                var Address = cnctype.convertdatatoALL("Address", keyidsssenckey, encIVIDS);

                var ottp = otp;

                var client = new RestClient(Vastwebmulti.Areas.WHITELABEL.Models.WapiBaseUrl.GetBaseUrl() + "/Money/api/Money/yyyy2?umm=" + userid + "&snn=" + NUMBER + "&fggg=" + ifsc + "&eee=" + benCode + "&ttt=" + amount + "&nnn=" + dmtpin + "&nttt=" + account + "&peee=" + type + "&nbb=" + nbb + "&bnm=" + bankname + "&Latitude=" + Latitude + "&Longitude=" + Longitude + "&ModelNo=" + ModelNo + "&City=" + City + "&PostalCode=" + PostalCode + "&InternetTYPE=" + InternetTYPE + "&Address=" + Address + "&value1=" + value1 + "&value2=" + value2 + "&ottp=" + ottp + "&Devicetoken=&kyc=N&ip=&mac=&AppType=ONLINE");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", "bearer " + Token + "");
                IRestResponse response = client.Execute(request);
                var responsechk = response.Content;

                var jss = new JavaScriptSerializer();
                var dict = jss.Deserialize<dynamic>(responsechk);
                return Json(dict, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                var results = "{'Message':'Something Went Wrong !!!','Response':'failure'}";
                var jss = new JavaScriptSerializer();
                var dict = jss.Deserialize<dynamic>(results);
                return Json(dict, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Print_Imps_Pdf(string orderid)
        {
            string userid = User.Identity.GetUserId();
            var rem = db.Retailer_Details.Where(aa => aa.RetailerId == userid).SingleOrDefault();
            var logo = db.tblHeaderLogoes.Where(aa => aa.Role == "ADMIN").SingleOrDefault().LogoImage;
            var chk = db.IMPS_transtion_detsils.Where(aa => aa.trans_common_id == orderid && aa.Status.ToUpper() != "RESEND").SingleOrDefault();
            var sts = chk.Status;
            ViewData["amount"] = chk.amount;
            ViewData["orderid"] = orderid;
            var logochk = logo.Replace("\\Outside_logo\\", "");
            ViewData["logo"] = logochk;
            ViewData["FirmName"] = rem.Frm_Name;
            ViewData["mode"] = chk.Trans_Type;
            ViewData["amount"] = chk.amount;
            var charge = chk.charge;
            decimal tax = 0; decimal total = 0;
            if (rem.gststatus == "Y")
            {
                tax = (Convert.ToDecimal(charge) * 18) / 100;
                total = Convert.ToDecimal(chk.amount) + Convert.ToDecimal(tax) + Convert.ToDecimal(charge);
            }
            else
            {
                total = Convert.ToDecimal(chk.amount) + Convert.ToDecimal(tax) + Convert.ToDecimal(charge);
            }
            ViewData["servicefee"] = charge;
            ViewData["tax"] = tax;
            ViewData["total"] = total;
            ViewData["rrn"] = chk.bank_trans_id;
            ViewData["accountno"] = chk.accountno;
            ViewData["ifsccode"] = chk.ifsccode;
            if (sts.ToUpper() == "SUCCESS" || sts.ToUpper() == "PENDING")
            {
                ViewData["msg"] = "Transaction Successful";
            }
            else
            {
                ViewData["msg"] = "Transaction Failed";
            }
            return new ViewAsPdf("Print_Imps_Pdf");
        }
        [HttpPost]
        public ActionResult imps_email(string orderid)
        {
            string userid = User.Identity.GetUserId();
            var rem = db.Retailer_Details.Where(aa => aa.RetailerId == userid).SingleOrDefault();
            var logo = db.tblHeaderLogoes.Where(aa => aa.Role == "ADMIN").SingleOrDefault().LogoImage;
            var chk = db.IMPS_transtion_detsils.Where(aa => aa.trans_common_id == orderid).SingleOrDefault();

            var sts = chk.Status;
            ViewData["amount"] = chk.amount;
            ViewData["orderid"] = orderid;
            var logochk = logo.Replace("\\Outside_logo\\", "");
            ViewData["logo"] = logochk;
            ViewData["FirmName"] = rem.Frm_Name;
            ViewData["mode"] = chk.Trans_Type;
            ViewData["amount"] = chk.amount;
            ViewData["servicefee"] = 0;
            ViewData["tax"] = "0";
            ViewData["total"] = chk.amount + 0;
            ViewData["rrn"] = chk.bank_trans_id;
            ViewData["accountno"] = chk.accountno;
            ViewData["ifsccode"] = chk.ifsccode;
            var msg = "";
            if (sts.ToUpper() == "SUCCESS" || sts.ToUpper() == "PENDING")
            {
                msg = "Transaction Successful";
            }
            else
            {
                msg = "Transaction Failed";
            }
            decimal tax = 0; decimal total = 0;
            if (rem.gststatus == "Y")
            {
                var charge = Convert.ToDecimal(chk.charge);
                tax = (charge * 18) / 100;
            }
            total = Convert.ToDecimal(chk.charge) + Convert.ToDecimal(chk.amount) + tax;
            //string body = new CommonUtil().PopulateBodyEmail(msg, chk.amount.ToString(), orderid, rem.Frm_Name, chk.Trans_Type, chk.amount.ToString(), tax.ToString(), total.ToString(), chk.bank_trans_id, chk.accountno, chk.ifsccode);
            //new CommonUtil().InsertsendmailIMPS(rem.Email, "IMPS Transaction", body);
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public IRestResponse tokencheck()
        {
            var apidetails = db.Money_API_URLS.Where(aa => aa.API_Name == "VASTWEB").SingleOrDefault();
            var token = apidetails == null ? "" : apidetails.Token;
            var apiid = apidetails == null ? "" : apidetails.API_ID;
            var apiidpwd = apidetails == null ? "" : apidetails.Api_pwd;
            var client = new RestClient(VastbazaarBaseUrl + "token");
            var request = new RestRequest(Method.POST);
            request.AddHeader("iptoken", token);
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("application/x-www-form-urlencoded", "UserName=" + apiid + "&Password=" + apiidpwd + "&grant_type=password", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            return response;
        }
        public string getAuthToken()
        {
            try
            {
                var tokn = db.vastbazzartokens.SingleOrDefault();
                if (tokn == null)
                {
                    var response = tokencheck();
                    var responsechk = response.Content.ToString();
                    var responsecode = response.StatusCode.ToString();
                    if (responsecode == "OK")
                    {
                        Models.Vastbillpay vb = new Models.Vastbillpay();
                        dynamic json = JsonConvert.DeserializeObject(responsechk);
                        var token = json.access_token.ToString();
                        var expire = json[".expires"].ToString();
                        DateTime exp = Convert.ToDateTime(expire);
                        vastbazzartoken vast = new vastbazzartoken();
                        vast.apitoken = token;
                        vast.exptime = exp;
                        db.vastbazzartokens.Add(vast);
                        db.SaveChanges();
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
                            db.SaveChanges();
                            return token;
                        }
                        else
                        {
                            return null;
                        }
                    }

                }
            }
            catch
            {
                return null;
            }
        }
        public void UpdateAuthToken()
        {
            var response = tokencheck();
            var responsechk = response.Content.ToString();
            var responsecode = response.StatusCode.ToString();
            if (responsecode == "OK")
            {
                var tokn = db.vastbazzartokens.SingleOrDefault();
                dynamic json = JsonConvert.DeserializeObject(responsechk);
                var token = json.access_token.ToString();
                var expire = json[".expires"].ToString();
                DateTime exp = Convert.ToDateTime(expire);

                tokn.apitoken = token;
                tokn.exptime = exp;
                db.SaveChanges();

            }


        }
        /// <summary>
        /// [GET] Displays new-format DMT money transfer report
        /// </summary>
        public ActionResult DMTreportnew()
        {
            var userid = User.Identity.GetUserId();
            Recent_report recent = new Recent_report();
            recent.WRecent_report_imps = db.recent_imps_report(userid).ToList();
            recent.WRecent_report_Aeps = null;
            recent.WRecent_PAN_CARD_IPAY = null;
            return PartialView("_RecentReport", recent);
        }
        /// <summary>
        /// [GET] Displays new-format PAN card transaction listing
        /// </summary>
        public ActionResult PANCARDreportnew()
        {
            var userid = User.Identity.GetUserId();
            Recent_report recent = new Recent_report();
            recent.WRecent_report_imps = null;
            recent.WRecent_report_Aeps = null;
            recent.WRecent_PAN_CARD_IPAY = db.PAN_CARD_IPAY.Where(aa => aa.RetailerId == userid).OrderByDescending(aa => aa.Date).Take(10).ToList();
            return PartialView("_RecentReport", recent);
        }


        public ActionResult InsertRem_MposID(string mposmarchantid)
        {
            var RetailerID = User.Identity.GetUserId();

            var result = db.Retailer_Details.Where(x => x.RetailerId == RetailerID).SingleOrDefault();
            if (result != null)
            {
                if (result.PSAStatus == "Y" && result.AadhaarStatus == "Y" && result.ShopwithSalfieStatus == "Y" && result.pancardPath != null && result.aadharcardPath != null && result.ShopwithSalfie != null)
                {


                    string mposid = result.PartnerID;
                    if (string.IsNullOrEmpty(mposid))
                    {
                        result.PartnerID = mposmarchantid;
                        db.SaveChanges();

                        return Json("success", JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json("failed", JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json("KYCDue", JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json("failed", JsonRequestBehavior.AllowGet);
            }
        }
        #endregion End MONEYTRANSFER DMT 2


        #region Start DMT 1
        //Start Instantpay and Limit 25000 pay Money Transfer 
        [HttpGet]
        public ActionResult Money_transfer1(string senderno)
        {
            ViewBag.senderno = senderno;
            var userid = User.Identity.GetUserId();
            try
            {

                //ViewBag.tab = tabvalue;
                var apinm = db.money_api_status.Where(aa => aa.status == true && aa.catagory== "PAYOUT").SingleOrDefault();
                var apiname = apinm == null ? "NO" : apinm.api_name;
                if (apiname != "NO")
                {
                    if (apiname == "INSTANTPAY")
                    {
                        string respon = new InstantPayComnUtil().getBankList("0581000100109278");
                        if (respon != "ERROR")
                        {
                            dynamic dynJson = JsonConvert.DeserializeObject(respon.ToString());
                            List<BankNameModel> Details1 = dynJson.data.ToObject<List<BankNameModel>>();
                            var Details2 = Details1.Where(aa => aa.is_down == "1").ToList();
                            ViewBag.bankdown = Details2;

                            //ViewBag.banknm = "";
                            ViewBag.banknm = new SelectList("", "branch_ifsc", "bank_name");
                        }
                        else
                        {
                            var Details = (from ff in db.masterifscs select ff).ToList();
                            ViewBag.banknm = new SelectList(Details.OrderBy(aa => aa.bankname), "ifsccode", "bankname");
                            ViewBag.bankdown = null;
                        }

                    }
                    else if (apiname == "VASTWEB")
                    {
                        var tokn = db.vastbazzartokens.SingleOrDefault();
                        if (tokn == null)
                        {
                            var response = tokencheck();
                            var responsechk = response.Content.ToString();
                            var responsecode = response.StatusCode.ToString();
                            if (responsecode == "OK")
                            {

                                VastBazaar1 cb1 = new VastBazaar1();
                                dynamic json = JsonConvert.DeserializeObject(responsechk);
                                var token = json.access_token.ToString();
                                var expire = json[".expires"].ToString();
                                DateTime exp = Convert.ToDateTime(expire);
                                vastbazzartoken vast = new vastbazzartoken();
                                vast.apitoken = token;
                                vast.exptime = exp;
                                db.vastbazzartokens.Add(vast);
                                db.SaveChanges();

                                var responseall = cb1.Bank_details("0581000100109278", tokn.apitoken);
                                responsechk = responseall.Content;
                                json = JsonConvert.DeserializeObject(responsechk);
                                var respcode = json.Content.ResponseCode.ToString();
                                var ADDINFO = json.Content.ADDINFO;
                                var ser = JsonConvert.SerializeObject(ADDINFO);
                                json = JsonConvert.DeserializeObject(ser);
                                JArray a = (JArray)json["data"];
                                IList<BankNameModel> Details = a.ToObject<IList<BankNameModel>>();
                                ViewBag.banknm = new SelectList("", "branch_ifsc", "bank_name");
                                var Details2 = Details.Where(aa => aa.is_down == "1").ToList();
                                ViewBag.bankdown = Details2;
                                // ViewBag.banknm = null;
                            }
                            else
                            {
                                // var Details = (from ff in db.masterifscs select ff).ToList();
                                ViewBag.banknm = new SelectList("", "branch_ifsc", "bank_name");
                                ViewBag.bankdown = null;
                                // ViewBag.banknm = null;
                            }
                        }
                        else
                        {


                            DateTime curntdate = DateTime.Now.Date;
                            DateTime expdate = Convert.ToDateTime(tokn.exptime).Date;
                            var responsechk = "";
                            // var responsecode1 = "";
                            if (expdate > curntdate)
                            {
                                VastBazaar1 cb1 = new VastBazaar1();

                                var responseall = cb1.Bank_details("0581000100109278", tokn.apitoken);
                                responsechk = responseall.Content;
                                dynamic json = JsonConvert.DeserializeObject(responsechk);
                                var respcode = json.Content.ResponseCode.ToString();
                                var ADDINFO = json.Content.ADDINFO;
                                var ser = JsonConvert.SerializeObject(ADDINFO);
                                json = JsonConvert.DeserializeObject(ser);
                                JArray a = (JArray)json["data"];
                                IList<BankNameModel> Details = a.ToObject<IList<BankNameModel>>();
                                ViewBag.banknm = new SelectList("", "branch_ifsc", "bank_name");
                                var Details2 = Details.Where(aa => aa.is_down == "1").ToList();
                                ViewBag.bankdown = Details2;
                                // ViewBag.banknm = null;
                            }
                            else
                            {
                                var response = tokencheck();
                                var response1 = response.Content.ToString();
                                var responsecode = response.StatusCode.ToString();
                                if (responsecode == "OK")
                                {
                                    VastBazaar1 cb1 = new VastBazaar1();

                                    dynamic json = JsonConvert.DeserializeObject(response1);
                                    var token = json.access_token.ToString();
                                    var expire = json[".expires"].ToString();
                                    DateTime exp = Convert.ToDateTime(expire);
                                    tokn.apitoken = token;
                                    tokn.exptime = exp;
                                    db.SaveChanges();

                                    var responseall = cb1.Bank_details("0581000100109278", tokn.apitoken);
                                    responsechk = responseall.Content;
                                    json = JsonConvert.DeserializeObject(responsechk);
                                    var respcode = json.Content.ResponseCode.ToString();
                                    var ADDINFO = json.Content.ADDINFO;
                                    var ser = JsonConvert.SerializeObject(ADDINFO);
                                    json = JsonConvert.DeserializeObject(ser);
                                    JArray a = (JArray)json["data"];
                                    IList<BankNameModel> Details = a.ToObject<IList<BankNameModel>>();
                                    ViewBag.banknm = new SelectList("", "branch_ifsc", "bank_name");
                                    var Details2 = Details.Where(aa => aa.is_down == "1").ToList();
                                    ViewBag.bankdown = Details2;
                                    // ViewBag.banknm = null;
                                }
                                else
                                {

                                    ViewBag.banknm = new SelectList("", "branch_ifsc", "bank_name");
                                    ViewBag.bankdown = null;
                                    //ViewBag.banknm = null;
                                }
                            }

                        }
                    }
                }
                else
                {
                    //  var Details = (from ff in db.masterifscs select ff).ToList();
                    ViewBag.banknm = new SelectList("", "branch_ifsc", "bank_name");
                    ViewBag.bankdown = null;
                    // ViewBag.banknm = null;
                }
                var show = db.searchbyaccountno_imps("").ToList();
                ViewBag.showdata = show;
                //show pancard and aeps and mpos status
                var ch = db.Whitelabel_Retailer_Details.Where(a => a.RetailerId == userid).FirstOrDefault();
                ViewBag.psncardsts = ch.PSAStatus;
                ViewBag.aepsandmpos = ch.AepsMposstatus;
            }
            catch
            {
                var show = db.searchbyaccountno_imps("").ToList();
                ViewBag.showdata = show;
                //show pancard and aeps and mpos status
                var ch = db.Whitelabel_Retailer_Details.Where(a => a.RetailerId == userid).FirstOrDefault();
                ViewBag.psncardsts = ch.PSAStatus;
                ViewBag.aepsandmpos = ch.AepsMposstatus;
                ViewBag.banknm = new SelectList("", "branch_ifsc", "bank_name");
                ViewBag.bankdown = null;
            }
            return View();
        }
        //Fill Bank Name by account no
        public ActionResult BankListFill1(string accountno)
        {
            var userid = User.Identity.GetUserId();
            //ViewBag.tab = tabvalue;
            var apinm = db.money_api_status.Where(aa => aa.status == true && aa.catagory== "PAYOUT").SingleOrDefault();
            var apiname = apinm == null ? "NO" : apinm.api_name;
            try
            {
                if (apiname != "NO")
                {
                    if (apiname == "INSTANTPAY")
                    {
                        string respon = new InstantPayComnUtil().getBankList(accountno);
                        if (respon != "ERROR")
                        {
                            //dynamic dynJson = JsonConvert.DeserializeObject(respon.ToString());
                            var jss = new JavaScriptSerializer();
                            var dict = jss.Deserialize<dynamic>(respon);
                            return Json(dict, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            var results = "{'message':'Error','status':'failure'}";
                            var jss = new JavaScriptSerializer();
                            var dict = jss.Deserialize<dynamic>(results);
                            return Json(dict, JsonRequestBehavior.AllowGet);
                        }

                    }
                    else if (apiname == "VASTWEB")
                    {
                        var tokn = db.vastbazzartokens.SingleOrDefault();
                        if (tokn == null)
                        {
                            var response = tokencheck();
                            var responsechk = response.Content.ToString();
                            var responsecode = response.StatusCode.ToString();
                            if (responsecode == "OK")
                            {

                                VastBazaar1 cb1 = new VastBazaar1();
                                dynamic json = JsonConvert.DeserializeObject(responsechk);
                                var token = json.access_token.ToString();
                                var expire = json[".expires"].ToString();
                                DateTime exp = Convert.ToDateTime(expire);
                                vastbazzartoken vast = new vastbazzartoken();
                                vast.apitoken = token;
                                vast.exptime = exp;
                                db.vastbazzartokens.Add(vast);
                                db.SaveChanges();

                                var responseall = cb1.Bank_details(accountno, tokn.apitoken);
                                responsechk = responseall.Content;
                                json = JsonConvert.DeserializeObject(responsechk);
                                var respcode = json.Content.ResponseCode.ToString();
                                var ADDINFO = json.Content.ADDINFO;
                                var stscode = ADDINFO.statuscode;
                                var results = JsonConvert.SerializeObject(ADDINFO);
                                var jss = new JavaScriptSerializer();
                                var dict = jss.Deserialize<dynamic>(results);
                                return Json(dict, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                var results = "{'message':'Error','status':'failure'}";
                                var jss = new JavaScriptSerializer();
                                var dict = jss.Deserialize<dynamic>(results);
                                return Json(dict, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {

                            DateTime curntdate = DateTime.Now.Date;
                            DateTime expdate = Convert.ToDateTime(tokn.exptime).Date;
                            var responsechk = "";
                            // var responsecode1 = "";
                            if (expdate > curntdate)
                            {
                                VastBazaar1 cb1 = new VastBazaar1();


                                var responseall = cb1.Bank_details(accountno, tokn.apitoken);
                                responsechk = responseall.Content;
                                dynamic json = JsonConvert.DeserializeObject(responsechk);
                                var respcode = json.Content.ResponseCode.ToString();
                                var ADDINFO = json.Content.ADDINFO;
                                var stscode = ADDINFO.statuscode;
                                var results = JsonConvert.SerializeObject(ADDINFO);
                                var jss = new JavaScriptSerializer();
                                var dict = jss.Deserialize<dynamic>(results);
                                return Json(dict, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                var response = tokencheck();
                                var response1 = response.Content.ToString();
                                var responsecode = response.StatusCode.ToString();
                                if (responsecode == "OK")
                                {
                                    VastBazaar1 cb1 = new VastBazaar1();

                                    dynamic json = JsonConvert.DeserializeObject(responsechk);
                                    var token = json.access_token.ToString();
                                    var expire = json[".expires"].ToString();
                                    DateTime exp = Convert.ToDateTime(expire);
                                    tokn.apitoken = token;
                                    tokn.exptime = exp;
                                    db.SaveChanges();


                                    var responseall = cb1.Bank_details(accountno, tokn.apitoken);
                                    responsechk = responseall.Content;
                                    json = JsonConvert.DeserializeObject(responsechk);
                                    var respcode = json.Content.ResponseCode.ToString();
                                    var ADDINFO = json.Content.ADDINFO;
                                    var stscode = ADDINFO.statuscode;
                                    var results = JsonConvert.SerializeObject(ADDINFO);
                                    var jss = new JavaScriptSerializer();
                                    var dict = jss.Deserialize<dynamic>(results);
                                    return Json(dict, JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    var results = "{'message':'Error','status':'failure'}";
                                    var jss = new JavaScriptSerializer();
                                    var dict = jss.Deserialize<dynamic>(results);
                                    return Json(dict, JsonRequestBehavior.AllowGet);
                                }
                            }
                        }
                    }
                    else
                    {
                        var results = "{'message':'Error','status':'failure'}";
                        var jss = new JavaScriptSerializer();
                        var dict = jss.Deserialize<dynamic>(results);
                        return Json(dict, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    var results = "{'message':'Api Not Open','status':'failure'}";
                    var jss = new JavaScriptSerializer();
                    var dict = jss.Deserialize<dynamic>(results);
                    return Json(dict, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                var results = "{'message':'" + ex.Message + "','status':'failure'}";
                var jss = new JavaScriptSerializer();
                var dict = jss.Deserialize<dynamic>(results);
                return Json(dict, JsonRequestBehavior.AllowGet);
            }
        }

        //search data by account number
        public ActionResult Searchbyaccountno1(string accountno)
        {
            var show = db.searchbyaccountno_imps(accountno).ToList();
            ViewBag.showdata = show;
            return PartialView("_showdatabyaccountno", ViewBag.showdata);
        }

        //show All Remitter Details
        [HttpPost]
        public ActionResult process_sender_cyber1(string sender_number)
        {
            var userid = User.Identity.GetUserId();
            // var pending = db.IMPS_transtion_detsils.Where(p => p.senderno == sender_number && p.rch_from == userid && p.Status == "Pending").ToList();
            try
            {
                var ch = "";
                var apinm = db.money_api_status.Where(aa => aa.status == true && aa.catagory== "PAYOUT").SingleOrDefault();
                var apiname = apinm == null ? "NO" : apinm.api_name;
                if (apiname != "NO")
                {
                    //   var apiname = (db.money_api_name.Where(aa => aa.status == "Y").Single().Api_name);
                    /////////////Cyber///////////////////////
                    if (apiname.ToUpper() == "CYBER")
                    {

                        moneytransfer_cyberplate cb = new moneytransfer_cyberplate();
                        ch = cb.customer_verify(sender_number);
                        //////////////Result////////////////////////////
                        int start1 = ch.IndexOf("RESULT=") + 7;

                        int end1 = ch.IndexOf("\r\n", start1);
                        string result = ch.Substring(start1, end1 - start1);
                        /////////////ERROR////////////////////////////////////
                        start1 = ch.IndexOf("ERROR=") + 6;
                        end1 = ch.IndexOf("\r\n", start1);
                        string errornm = ch.Substring(start1, end1 - start1);
                        var results = "";
                        if (result == "1" && errornm == "224")
                        {
                            results = "{'message':'" + result + "','status':'register'}";
                            var jss1 = new JavaScriptSerializer();
                            var dict1 = jss1.Deserialize<dynamic>(results);
                            return Json(dict1, JsonRequestBehavior.AllowGet);
                        }
                        else if (result == "0" && errornm == "0")
                        {
                            int start = ch.IndexOf("ADDINFO=") + 8;
                            int end = ch.IndexOf("\r\n", start);
                            results = ch.Substring(start, end - start);
                            results = HttpUtility.UrlDecode(results);
                            dynamic jobject = JsonConvert.DeserializeObject(results);
                            var stscode = jobject.statuscode;
                            if (stscode == "TXN")
                            {
                                try
                                {
                                    bool isArray = jobject.data.beneficiary.Type == JTokenType.Array;
                                    if (isArray == false)
                                    {
                                        results = results.Replace("\"beneficiary\":{\"item\":{", "\"beneficiary\":{\"item\":[{");
                                        int modificationIndex = results.IndexOf("}},", results.IndexOf("beneficiary"));
                                        if (modificationIndex > 0)
                                        {
                                            results = results.Remove(modificationIndex, 2).Insert(modificationIndex, "}]}");
                                        }
                                    }
                                }
                                catch
                                { }
                            }

                            var jss1 = new JavaScriptSerializer();
                            var dict1 = jss1.Deserialize<dynamic>(results);
                            return Json(dict1, JsonRequestBehavior.AllowGet);

                            //return Json(new { Url = Url.Action("_FincialViewPendingImps", pending) });

                        }
                        else
                        {
                            if (ch.Contains("MSG"))
                            {
                                int start = ch.IndexOf("ERRMSG=") + 7;
                                int end = ch.IndexOf("\r\n", start);
                                var error = ch.Substring(start, end - start);
                                results = "{'message':'" + error + "','status':'failure'}";
                            }
                            else
                            {
                                var error = db.Cyber_error_name_list.Where(aa => aa.error_code == errornm).SingleOrDefault().Error_name;
                                results = "{'message':'" + error + "','status':'failure'}";
                            }
                        }
                        var jss = new JavaScriptSerializer();
                        var dict = jss.Deserialize<dynamic>(results);
                        return Json(dict, JsonRequestBehavior.AllowGet);
                    }
                    /////////////INSTANTPAY//////////////////
                    else if (apiname.ToUpper() == "INSTANTPAY")
                    {
                        var token = db.Money_API_URLS.Where(a => a.API_Name == apiname).Single().Token;

                        if (HttpContext.Request.IsLocal)
                        {
                            token = "ca23f55293bc9c2a51fe06d2d07e5ea4";
                        }

                        var data = new
                        {
                            token = token,
                            request = new
                            {
                                mobile = sender_number
                            }
                        };
                        string URL = "https://www.instantpay.in/ws/dmi/remitter_details";
                        string jsonData = JsonConvert.SerializeObject(data);
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
                        request.Method = "POST";
                        request.ContentType = "application/json";
                        StreamWriter requestWriter = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.ASCII);
                        requestWriter.Write(jsonData);
                        requestWriter.Close();

                        try
                        {
                            WebResponse webResponse = request.GetResponse();
                            Stream webStream = webResponse.GetResponseStream();
                            StreamReader responseReader = new StreamReader(webStream);
                            string response = responseReader.ReadToEnd();
                            response = response.Replace(@"<?xml version=""1.0"" encoding=""utf-8""?>", "");


                            XmlDocument doc = new XmlDocument();
                            doc.LoadXml(response);

                            string json = JsonConvert.SerializeXmlNode(doc);
                            JavaScriptSerializer ser = new JavaScriptSerializer();
                            var parsed = ser.Deserialize<dynamic>(json);
                            json = ser.Serialize(parsed["xml"]);

                            dynamic data1 = JObject.Parse(json);
                            var stscode = data1.statuscode;

                            if (stscode == "TXN")
                            {
                                dynamic jobject = JsonConvert.DeserializeObject(json);
                                try
                                {
                                    bool isArray = jobject.data.beneficiary.item.Type == JTokenType.Array;
                                    if (isArray == false)
                                    {
                                        json = json.Replace("\"beneficiary\":{\"item\":{", "\"beneficiary\":{\"item\":[{");
                                        int modificationIndex = json.IndexOf("}},", json.IndexOf("beneficiary"));
                                        if (modificationIndex > 0)
                                        {
                                            json = json.Remove(modificationIndex, 2).Insert(modificationIndex, "}]}");
                                        }
                                    }
                                }
                                catch
                                { }
                            }

                            var jss = new JavaScriptSerializer();
                            var dict = jss.Deserialize<dynamic>(json);
                            return Json(dict, JsonRequestBehavior.AllowGet);
                            //return Json(dict).AddPartialView("_FincialViewPendingImps", pending);
                        }
                        catch (Exception ex)
                        {
                            var msg = ex.Message.ToString();
                            var results = "{'message':'" + msg + "','status':'failure'}";
                            var jss = new JavaScriptSerializer();
                            var dict = jss.Deserialize<dynamic>(results);
                            return Json(dict, JsonRequestBehavior.AllowGet);
                        }
                    }
                    ////////////VastWeb//////////////////
                    ////////////VastWeb//////////////////
                    else if (apiname.ToUpper() == "VASTWEB")
                    {

                        var tokn = db.vastbazzartokens.SingleOrDefault();
                        if (tokn == null)
                        {
                            var response = tokencheck();
                            var responsechk = response.Content.ToString();
                            var responsecode = response.StatusCode.ToString();
                            if (responsecode == "OK")
                            {
                                VastBazaar1 cb1 = new VastBazaar1();
                                dynamic json = JsonConvert.DeserializeObject(responsechk);
                                var token = json.access_token.ToString();
                                var expire = json[".expires"].ToString();
                                DateTime exp = Convert.ToDateTime(expire);
                                vastbazzartoken vast = new vastbazzartoken();
                                vast.apitoken = token;
                                vast.exptime = exp;
                                db.vastbazzartokens.Add(vast);
                                db.SaveChanges();
                                var responseall = cb1.Remitter_details(sender_number, token);
                                responsechk = responseall.Content.ToString();
                                var responsecode1 = responseall.StatusCode.ToString();
                                if (responsecode1 == "OK")
                                {
                                    json = JsonConvert.DeserializeObject(responsechk);
                                    var respcode = json.Content.ResponseCode.ToString();
                                    var ADDINFO = json.Content.ADDINFO;
                                    var stscode = ADDINFO.statuscode;

                                    if (stscode == "TXN")
                                    {
                                        json = JsonConvert.SerializeObject(ADDINFO);
                                        //dynamic jobject = JsonConvert.DeserializeObject(json.ToString());
                                        dynamic jobject = JsonConvert.DeserializeObject(json);
                                        try
                                        {
                                            bool isArray = jobject.data.beneficiary.item.Type == JTokenType.Array;
                                            if (isArray == false)
                                            {
                                                json = json.Replace("\"beneficiary\":{\"item\":{", "\"beneficiary\":{\"item\":[{");
                                                int modificationIndex = json.IndexOf("}},", json.IndexOf("beneficiary"));
                                                if (modificationIndex > 0)
                                                {
                                                    json = json.Remove(modificationIndex, 2).Insert(modificationIndex, "}]}");
                                                }
                                            }
                                        }
                                        catch
                                        { }
                                    }
                                    //   var results = JsonConvert.SerializeObject(json);
                                    var jss = new JavaScriptSerializer();
                                    var dict = jss.Deserialize<dynamic>(json);
                                    return Json(dict, JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    json = JsonConvert.DeserializeObject(responsechk);
                                    var error = json.error.ToString();
                                    var error_decribe = json["error_description"].ToString();
                                    var results = "{'message':'" + error_decribe + "','status':'failure'}";
                                    var jss = new JavaScriptSerializer();
                                    var dict = jss.Deserialize<dynamic>(results);

                                    return Json(dict, JsonRequestBehavior.AllowGet);
                                }

                            }
                            else
                            {
                                dynamic json = JsonConvert.DeserializeObject(responsechk);
                                var error = json.error.ToString();
                                var error_decribe = json["error_description"].ToString();
                                var results = "{'message':'" + error_decribe + "','status':'failure'}";
                                var jss = new JavaScriptSerializer();
                                var dict = jss.Deserialize<dynamic>(results);

                                return Json(dict, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            VastBazaar1 cb1 = new VastBazaar1();

                            DateTime curntdate = DateTime.Now.Date;
                            DateTime expdate = Convert.ToDateTime(tokn.exptime).Date;
                            var responsechk = "";
                            var responsecode1 = "";
                            if (expdate > curntdate)
                            {
                                var responseall = cb1.Remitter_details(sender_number, tokn.apitoken);
                                responsechk = responseall.Content.ToString();
                                responsecode1 = responseall.StatusCode.ToString();
                            }
                            else
                            {
                                var response = tokencheck();
                                var response1 = response.Content.ToString();
                                var responsecode = response.StatusCode.ToString();
                                if (responsecode == "OK")
                                {
                                    dynamic json = JsonConvert.DeserializeObject(responsechk);
                                    var token = json.access_token.ToString();
                                    var expire = json[".expires"].ToString();
                                    DateTime exp = Convert.ToDateTime(expire);
                                    tokn.apitoken = token;
                                    tokn.exptime = exp;
                                    db.SaveChanges();
                                    var responseall = cb1.Remitter_details(sender_number, token);
                                    responsechk = responseall.Content.ToString();
                                    responsecode1 = responseall.StatusCode.ToString();
                                }
                                else
                                {
                                    dynamic json = JsonConvert.DeserializeObject(responsechk);
                                    var error = json.error.ToString();
                                    var error_decribe = json["error_description"].ToString();
                                    var results = "{'message':'" + error_decribe + "','status':'failure'}";
                                    var jss = new JavaScriptSerializer();
                                    var dict = jss.Deserialize<dynamic>(results);

                                    return Json(dict, JsonRequestBehavior.AllowGet);
                                }
                            }

                            if (responsecode1 == "OK")
                            {
                                dynamic json = JsonConvert.DeserializeObject(responsechk);
                                var respcode = json.Content.ResponseCode.ToString();
                                var ADDINFO = json.Content.ADDINFO;
                                var stscode = ADDINFO.statuscode;
                                json = JsonConvert.SerializeObject(ADDINFO);
                                if (stscode == "TXN")
                                {

                                    //dynamic jobject = JsonConvert.DeserializeObject(json.ToString());
                                    dynamic jobject = JsonConvert.DeserializeObject(json);
                                    try
                                    {
                                        bool isArray = jobject.data.beneficiary.item.Type == JTokenType.Array;
                                        if (isArray == false)
                                        {
                                            json = json.Replace("\"beneficiary\":{\"item\":{", "\"beneficiary\":{\"item\":[{");
                                            int modificationIndex = json.IndexOf("}},", json.IndexOf("beneficiary"));
                                            if (modificationIndex > 0)
                                            {
                                                json = json.Remove(modificationIndex, 2).Insert(modificationIndex, "}]}");
                                            }
                                        }
                                    }
                                    catch
                                    { }
                                }
                                //   var results = JsonConvert.SerializeObject(json);

                                var jss = new JavaScriptSerializer();
                                var dict = jss.Deserialize<dynamic>(json);
                                return Json(dict, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                dynamic json = JsonConvert.DeserializeObject(responsechk);
                                var error = json.error.ToString();
                                var error_decribe = json["error_description"].ToString();
                                var results = "{'message':'" + error_decribe + "','status':'failure'}";
                                var jss = new JavaScriptSerializer();
                                var dict = jss.Deserialize<dynamic>(results);

                                return Json(dict, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                    else
                    {
                        var results = "{'message':'Api Name Not Register','status':'failure'}";
                        var jss = new JavaScriptSerializer();
                        var dict = jss.Deserialize<dynamic>(results);
                        return Json(dict, JsonRequestBehavior.AllowGet);
                    }
                    ////////////extranal/////////////////////////
                }
                else
                {
                    var results = "{'message':'No api Open.','status':'failure'}";
                    var jss = new JavaScriptSerializer();
                    var dict = jss.Deserialize<dynamic>(results);
                    return Json(dict, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                var results = "{'message':'" + ex.Message + "','status':'failure'}";
                var jss = new JavaScriptSerializer();
                var dict = jss.Deserialize<dynamic>(results);
                return Json(dict, JsonRequestBehavior.AllowGet);
            }

        }
        //Register Remitter  Validate 
        [HttpPost]
        public ActionResult addcustomer_cyber_Validate1(string sender_number, string remitterid, int Otp, string outletid)
        {
            try
            {
                var ch = "";
                var apinm = db.money_api_status.Where(aa => aa.status == true && aa.catagory== "PAYOUT").SingleOrDefault();
                var apiname = apinm == null ? "NO" : apinm.api_name;
                if (apiname != "NO")
                {
                    //var // apiname = "INSTANTPAY";
                    //////////////////Cyber/////////
                    if (apiname == "Cyber")
                    {
                        moneytransfer_cyberplate cb = new moneytransfer_cyberplate();
                        ssl.CERTNo = cert;
                        ch = cb.add_customer(sender_number, sender_number);
                        int start1 = ch.IndexOf("RESULT=") + 7;
                        int end1 = ch.IndexOf("\r\n", start1);
                        string result = ch.Substring(start1, end1 - start1);
                        /////////////ERROR////////////////////////////////////
                        start1 = ch.IndexOf("ERROR=") + 6;
                        end1 = ch.IndexOf("\r\n", start1);
                        string errornm = ch.Substring(start1, end1 - start1);
                        if (result == "0" && errornm == "0")
                        {
                            int start = ch.IndexOf("ADDINFO=") + 8;
                            int end = ch.IndexOf("DATE=", start);
                            result = ch.Substring(start, end - start);
                            result = HttpUtility.UrlDecode(result);
                            var jss = new JavaScriptSerializer();
                            var dict = jss.Deserialize<dynamic>(result);
                            return Json(dict, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            if (ch.ToUpper().Contains("ERRMSG"))
                            {
                                int start = ch.IndexOf("ERRMSG=") + 7;
                                int end = ch.IndexOf("\r\n", start);
                                var errormsg = ch.Substring(start, end - start);
                                var results1 = "{'Message':'" + errormsg + "','Response':'failure'}";
                                var jss1 = new JavaScriptSerializer();
                                var dict1 = jss1.Deserialize<dynamic>(results1);
                                return Json(dict1, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {

                                var error = db.Cyber_error_name_list.Where(aa => aa.error_code == errornm).SingleOrDefault().Error_name;
                                var results = "{'Message':'" + error + "','Response':'failure'}";
                                var jss = new JavaScriptSerializer();
                                var dict = jss.Deserialize<dynamic>(results);
                                return Json(dict, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                    /////////////INSTANTPAY//////////////////
                    else if (apiname.ToUpper() == "INSTANTPAY")
                    {
                        var token = "";
                        token = db.Money_API_URLS.Where(a => a.API_Name == apiname).Single().Token;

                        if (HttpContext.Request.IsLocal)
                        {
                            token = "ca23f55293bc9c2a51fe06d2d07e5ea4";
                        }

                        var data = new
                        {
                            token = token,
                            request = new
                            {
                                mobile = sender_number,
                                remitterid = remitterid,
                                otp = Otp,
                                outletid = outletid
                            }
                        };

                        string URL = "https://www.instantpay.in/ws/dmi/remitter_validate";
                        string jsonData = JsonConvert.SerializeObject(data);
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
                        request.Method = "POST";
                        request.ContentType = "application/json";
                        StreamWriter requestWriter = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.ASCII);
                        requestWriter.Write(jsonData);
                        requestWriter.Close();

                        try
                        {

                            WebResponse webResponse = request.GetResponse();
                            Stream webStream = webResponse.GetResponseStream();
                            StreamReader responseReader = new StreamReader(webStream);
                            string response = responseReader.ReadToEnd();
                            response = response.Replace(@"<?xml version=""1.0"" encoding=""utf-8""?>", "");
                            XmlDocument doc = new XmlDocument();
                            doc.LoadXml(response);


                            string json = JsonConvert.SerializeXmlNode(doc);
                            JavaScriptSerializer ser = new JavaScriptSerializer();
                            var parsed = ser.Deserialize<dynamic>(json);
                            json = ser.Serialize(parsed["xml"]);
                            var jss = new JavaScriptSerializer();
                            var dict = jss.Deserialize<dynamic>(json);
                            return Json(dict, JsonRequestBehavior.AllowGet);


                        }
                        catch (Exception ex)
                        {
                            var msg = ex.Message.ToString();
                            var results = "{'Message':'" + msg + "','Response':'failure'}";
                            var jss = new JavaScriptSerializer();
                            var dict = jss.Deserialize<dynamic>(results);
                            return Json(dict, JsonRequestBehavior.AllowGet);
                        }

                    }
                    /////////////////VAstWeb////////////////
                    else if (apiname.ToUpper() == "VASTWEB")
                    {
                        var tokn = Responsetoken.gettoken();
                        VastBazaar1 cb1 = new VastBazaar1();
                        var responseall = cb1.Remitter_Register_Validate(sender_number, Otp, remitterid, outletid, tokn);
                        var responsechk = responseall.Content.ToString();
                        var responsecode1 = responseall.StatusCode.ToString();
                        if (responsecode1 == "OK")
                        {
                            dynamic json = JsonConvert.DeserializeObject(responsechk);
                            var respcode = json.Content.ResponseCode.ToString();
                            var ADDINFO = json.Content.ADDINFO;
                            var stscode = ADDINFO.statuscode;
                            var results = JsonConvert.SerializeObject(ADDINFO);
                            var jss = new JavaScriptSerializer();
                            var dict = jss.Deserialize<dynamic>(results);
                            return Json(dict, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            dynamic json = JsonConvert.DeserializeObject(responsechk);
                            var error = json.error.ToString();
                            var error_decribe = json["error_description"].ToString();
                            var results = "{'message':'" + error_decribe + "','status':'failure'}";
                            var jss = new JavaScriptSerializer();
                            var dict = jss.Deserialize<dynamic>(results);

                            return Json(dict, JsonRequestBehavior.AllowGet);
                        }


                    }
                    else
                    {
                        var results = "{'Message':'Api Name Not Register','Response':'failure'}";
                        var jss = new JavaScriptSerializer();
                        var dict = jss.Deserialize<dynamic>(results);
                        return Json(dict, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    var results = "{'Message':'No api Open.','Response':'failure'}";
                    var jss = new JavaScriptSerializer();
                    var dict = jss.Deserialize<dynamic>(results);
                    return Json(dict, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                var msg = ex.Message.ToString();
                var results = "  {'Message':'" + msg + "','Response':'failure' }";
                var jss = new JavaScriptSerializer();
                var dict = jss.Deserialize<dynamic>(results);
                return Json(dict, JsonRequestBehavior.AllowGet);
            }
        }
        //Register Remitter   
        [HttpPost]
        public ActionResult addcustomer_cyber1(string sender_number, string sender_name, string pincodeno, string surname)
        {
            try
            {
                var ch = "";
                var apinm = db.money_api_status.Where(aa => aa.status == true && aa.catagory== "PAYOUT").SingleOrDefault();
                var apiname = apinm == null ? "NO" : apinm.api_name;
                if (apiname != "NO")
                {
                    //var // apiname = "INSTANTPAY";
                    //////////////////Cyber/////////
                    if (apiname == "Cyber")
                    {
                        moneytransfer_cyberplate cb = new moneytransfer_cyberplate();
                        ssl.CERTNo = cert;
                        ch = cb.add_customer(sender_number, sender_name);
                        int start1 = ch.IndexOf("RESULT=") + 7;
                        int end1 = ch.IndexOf("\r\n", start1);
                        string result = ch.Substring(start1, end1 - start1);
                        /////////////ERROR////////////////////////////////////
                        start1 = ch.IndexOf("ERROR=") + 6;
                        end1 = ch.IndexOf("\r\n", start1);
                        string errornm = ch.Substring(start1, end1 - start1);
                        if (result == "0" && errornm == "0")
                        {
                            int start = ch.IndexOf("ADDINFO=") + 8;
                            int end = ch.IndexOf("DATE=", start);
                            result = ch.Substring(start, end - start);
                            result = HttpUtility.UrlDecode(result);
                            var jss = new JavaScriptSerializer();
                            var dict = jss.Deserialize<dynamic>(result);
                            return Json(dict, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            if (ch.ToUpper().Contains("ERRMSG"))
                            {
                                int start = ch.IndexOf("ERRMSG=") + 7;
                                int end = ch.IndexOf("\r\n", start);
                                var errormsg = ch.Substring(start, end - start);
                                var results1 = "{'Message':'" + errormsg + "','Response':'failure'}";
                                var jss1 = new JavaScriptSerializer();
                                var dict1 = jss1.Deserialize<dynamic>(results1);
                                return Json(dict1, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {

                                var error = db.Cyber_error_name_list.Where(aa => aa.error_code == errornm).SingleOrDefault().Error_name;
                                var results = "{'Message':'" + error + "','Response':'failure'}";
                                var jss = new JavaScriptSerializer();
                                var dict = jss.Deserialize<dynamic>(results);
                                return Json(dict, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                    /////////////INSTANTPAY//////////////////
                    else if (apiname.ToUpper() == "INSTANTPAY")
                    {
                        var token = "";
                        token = db.Money_API_URLS.Where(a => a.API_Name == apiname).Single().Token;

                        if (HttpContext.Request.IsLocal)
                        {
                            token = "ca23f55293bc9c2a51fe06d2d07e5ea4";
                        }

                        var data = new
                        {
                            token = token,
                            request = new
                            {
                                mobile = sender_number,
                                name = sender_name,
                                pincode = pincodeno,
                                surname = surname
                            }
                        };

                        string URL = "https://www.instantpay.in/ws/dmi/remitter";
                        string jsonData = JsonConvert.SerializeObject(data);
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
                        request.Method = "POST";
                        request.ContentType = "application/json";
                        StreamWriter requestWriter = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.ASCII);
                        requestWriter.Write(jsonData);
                        requestWriter.Close();

                        try
                        {

                            WebResponse webResponse = request.GetResponse();
                            Stream webStream = webResponse.GetResponseStream();
                            StreamReader responseReader = new StreamReader(webStream);
                            string response = responseReader.ReadToEnd();
                            response = response.Replace(@"<?xml version=""1.0"" encoding=""utf-8""?>", "");
                            XmlDocument doc = new XmlDocument();
                            doc.LoadXml(response);


                            string json = JsonConvert.SerializeXmlNode(doc);
                            JavaScriptSerializer ser = new JavaScriptSerializer();
                            var parsed = ser.Deserialize<dynamic>(json);
                            json = ser.Serialize(parsed["xml"]);
                            var jss = new JavaScriptSerializer();
                            var dict = jss.Deserialize<dynamic>(json);
                            return Json(dict, JsonRequestBehavior.AllowGet);


                        }
                        catch (Exception ex)
                        {
                            var msg = ex.Message.ToString();
                            var results = "{'Message':'" + msg + "','Response':'failure'}";
                            var jss = new JavaScriptSerializer();
                            var dict = jss.Deserialize<dynamic>(results);
                            return Json(dict, JsonRequestBehavior.AllowGet);
                        }

                    }
                    /////////////////VAstWeb////////////////
                    else if (apiname.ToUpper() == "VASTWEB")
                    {
                        var tokn = Responsetoken.gettoken();
                        VastBazaar1 cb1 = new VastBazaar1();
                        var responseall = cb1.Remitter_Register(sender_number, sender_name, pincodeno, surname, tokn);
                        var responsechk = responseall.Content.ToString();
                        var responsecode1 = responseall.StatusCode.ToString();
                        if (responsecode1 == "OK")
                        {
                            dynamic json = JsonConvert.DeserializeObject(responsechk);
                            var respcode = json.Content.ResponseCode.ToString();
                            var ADDINFO = json.Content.ADDINFO;
                            var stscode = ADDINFO.statuscode;
                            var results = JsonConvert.SerializeObject(ADDINFO);
                            var jss = new JavaScriptSerializer();
                            var dict = jss.Deserialize<dynamic>(results);
                            return Json(dict, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            dynamic json = JsonConvert.DeserializeObject(responsechk);
                            var error = json.error.ToString();
                            var error_decribe = json["error_description"].ToString();
                            var results = "{'message':'" + error_decribe + "','status':'failure'}";
                            var jss = new JavaScriptSerializer();
                            var dict = jss.Deserialize<dynamic>(results);

                            return Json(dict, JsonRequestBehavior.AllowGet);
                        }


                        //DateTime curntdate = DateTime.Now.Date;
                        //DateTime expdate = Convert.ToDateTime(tokn.exptime).Date;
                        //var responsechk = "";
                        //var responsecode1 = "";
                        //if (expdate > curntdate)
                        //{
                        //    var responseall = cb1.Remitter_Register(sender_number, sender_name, "332311", tokn.apitoken);
                        //    responsechk = responseall.Content.ToString();
                        //    responsecode1 = responseall.StatusCode.ToString();
                        //}
                        //else
                        //{
                        //    var response = tokencheck();
                        //    var response1 = response.Content.ToString();
                        //    var responsecode = response.StatusCode.ToString();
                        //    if (responsecode == "OK")
                        //    {
                        //        dynamic json = JsonConvert.DeserializeObject(responsechk);
                        //        var token = json.access_token.ToString();
                        //        var expire = json[".expires"].ToString();
                        //        DateTime exp = Convert.ToDateTime(expire);
                        //        tokn.apitoken = token;
                        //        tokn.exptime = exp;
                        //        db.SaveChanges();
                        //        var responseall = cb1.Remitter_Register(sender_number, sender_name, "332311", token);
                        //        responsechk = responseall.Content.ToString();
                        //        responsecode1 = responseall.StatusCode.ToString();
                        //    }
                        //    else
                        //    {
                        //        dynamic json = JsonConvert.DeserializeObject(responsechk);
                        //        var error = json.error.ToString();
                        //        var error_decribe = json["error_description"].ToString();
                        //        var results = "{'message':'" + error_decribe + "','status':'failure'}";
                        //        var jss = new JavaScriptSerializer();
                        //        var dict = jss.Deserialize<dynamic>(results);

                        //        return Json(dict, JsonRequestBehavior.AllowGet);
                        //    }
                        //}



                    }
                    else
                    {
                        var results = "{'Message':'Api Name Not Register','Response':'failure'}";
                        var jss = new JavaScriptSerializer();
                        var dict = jss.Deserialize<dynamic>(results);
                        return Json(dict, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    var results = "{'Message':'No api Open.','Response':'failure'}";
                    var jss = new JavaScriptSerializer();
                    var dict = jss.Deserialize<dynamic>(results);
                    return Json(dict, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                var msg = ex.Message.ToString();
                var results = "  {'Message':'" + msg + "','Response':'failure' }";
                var jss = new JavaScriptSerializer();
                var dict = jss.Deserialize<dynamic>(results);
                return Json(dict, JsonRequestBehavior.AllowGet);
            }
        }
        //End

        //delete beneficiary id  and send Otp your sender number
        [HttpPost]
        public ActionResult process_delete_recipient_cyber1(string mobile, string ifsc, string code, string remiterid)
        {
            try
            {

                ifsc = ifsc.ToUpper();
                var apinm = db.money_api_status.Where(aa => aa.status ==true && aa.catagory== "PAYOUT").SingleOrDefault();
                var apiname = apinm == null ? "NO" : apinm.api_name;
                if (apiname != "NO")
                {
                    //////////////////Cyber/////////
                    if (apiname == "Cyber")
                    {
                        moneytransfer_cyberplate cb = new moneytransfer_cyberplate();
                        var ch = cb.delete_benificy(mobile, ifsc, code, remiterid);
                        int start1 = ch.IndexOf("RESULT=") + 7;
                        int end1 = ch.IndexOf("\r\n", start1);
                        string result = ch.Substring(start1, end1 - start1);
                        /////////////ERROR////////////////////////////////////
                        start1 = ch.IndexOf("ERROR=") + 6;
                        end1 = ch.IndexOf("\r\n", start1);
                        string errornm = ch.Substring(start1, end1 - start1);
                        if (result == "0" && errornm == "0")
                        {
                            int start = ch.IndexOf("ADDINFO=") + 8;
                            int end = ch.IndexOf("DATE=", start);
                            string resultchk = ch.Substring(start, end - start);
                            resultchk = HttpUtility.UrlDecode(resultchk);
                            var jss = new JavaScriptSerializer();
                            var dict = jss.Deserialize<dynamic>(resultchk);
                            return Json(dict, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            if (ch.ToUpper().Contains("ERRMSG"))
                            {
                                int start = ch.IndexOf("ERRMSG=") + 7;
                                int end = ch.IndexOf("\r\n", start);
                                string errormsg = ch.Substring(start, end - start);
                                var results = "{'Message':'" + errormsg + "','Response':'failure' }";
                                var jss = new JavaScriptSerializer();
                                var dict = jss.Deserialize<dynamic>(results);
                                return Json(dict, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                var error = db.Cyber_error_name_list.Where(aa => aa.error_code == errornm).SingleOrDefault().Error_name;
                                var results = "{'Message':'" + error + "','Response':'failure'}";
                                var jss = new JavaScriptSerializer();
                                var dict = jss.Deserialize<dynamic>(results);
                                return Json(dict, JsonRequestBehavior.AllowGet);
                            }

                        }
                    }
                    /////////////INSTANTPAY//////////////////
                    else if (apiname.ToUpper() == "INSTANTPAY")
                    {
                        var token = db.Money_API_URLS.Where(a => a.API_Name == apiname).Single().Token;
                        if (HttpContext.Request.IsLocal)
                        {
                            token = "ca23f55293bc9c2a51fe06d2d07e5ea4";
                        }
                        var data = new
                        {
                            token = token,
                            request = new
                            {
                                beneficiaryid = code,
                                remitterid = remiterid
                            }
                        };



                        string URL = "https://www.instantpay.in/ws/dmi/beneficiary_remove";
                        string jsonData = JsonConvert.SerializeObject(data);
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
                        request.Method = "POST";
                        request.ContentType = "application/json";
                        StreamWriter requestWriter = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.ASCII);
                        requestWriter.Write(jsonData);
                        requestWriter.Close();

                        try
                        {
                            WebResponse webResponse = request.GetResponse();
                            Stream webStream = webResponse.GetResponseStream();
                            StreamReader responseReader = new StreamReader(webStream);
                            string response = responseReader.ReadToEnd();
                            response = response.Replace(@"<?xml version=""1.0"" encoding=""utf-8""?>", "");
                            XmlDocument doc = new XmlDocument();
                            doc.LoadXml(response);


                            string json = JsonConvert.SerializeXmlNode(doc);
                            JavaScriptSerializer ser = new JavaScriptSerializer();
                            var parsed = ser.Deserialize<dynamic>(json);
                            json = ser.Serialize(parsed["xml"]);

                            var jss = new JavaScriptSerializer();
                            var dict = jss.Deserialize<dynamic>(json);
                            return Json(dict, JsonRequestBehavior.AllowGet);


                        }
                        catch (Exception ex)
                        {
                            var msg = ex.Message.ToString();
                            var results = "{'Message':'" + msg + "','Response':'failure'}";
                            var jss = new JavaScriptSerializer();
                            var dict = jss.Deserialize<dynamic>(results);
                            return Json(dict, JsonRequestBehavior.AllowGet);
                        }

                    }
                    //////////////VAstWeb/////////////////
                    else if (apiname.ToUpper() == "VASTWEB")
                    {
                        var tokn = db.vastbazzartokens.SingleOrDefault();
                        VastBazaar1 cb1 = new VastBazaar1();

                        DateTime curntdate = DateTime.Now.Date;
                        DateTime expdate = Convert.ToDateTime(tokn.exptime).Date;
                        var responsechk = "";
                        var responsecode1 = "";
                        if (expdate > curntdate)
                        {
                            var responseall = cb1.Beneficiary_Delete(code, remiterid, tokn.apitoken, mobile);
                            responsechk = responseall.Content.ToString();
                            responsecode1 = responseall.StatusCode.ToString();
                        }
                        else
                        {
                            var response = tokencheck();
                            var response1 = response.Content.ToString();
                            var responsecode = response.StatusCode.ToString();
                            if (responsecode == "OK")
                            {
                                dynamic json = JsonConvert.DeserializeObject(responsechk);
                                var token = json.access_token.ToString();
                                var expire = json[".expires"].ToString();
                                DateTime exp = Convert.ToDateTime(expire);
                                tokn.apitoken = token;
                                tokn.exptime = exp;
                                db.SaveChanges();
                                var responseall = cb1.Beneficiary_Delete(code, remiterid, token, mobile);
                                responsechk = responseall.Content.ToString();
                                responsecode1 = responseall.StatusCode.ToString();
                            }
                            else
                            {
                                dynamic json = JsonConvert.DeserializeObject(responsechk);
                                var error = json.error.ToString();
                                var error_decribe = json["error_description"].ToString();
                                var results = "{'message':'" + error_decribe + "','status':'failure'}";
                                var jss = new JavaScriptSerializer();
                                var dict = jss.Deserialize<dynamic>(results);

                                return Json(dict, JsonRequestBehavior.AllowGet);
                            }
                        }

                        if (responsecode1 == "OK")
                        {
                            dynamic json = JsonConvert.DeserializeObject(responsechk);
                            var respcode = json.Content.ResponseCode.ToString();
                            var ADDINFO = json.Content.ADDINFO;
                            var stscode = ADDINFO.statuscode;
                            var results = JsonConvert.SerializeObject(ADDINFO);
                            var jss = new JavaScriptSerializer();
                            var dict = jss.Deserialize<dynamic>(results);
                            return Json(dict, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            dynamic json = JsonConvert.DeserializeObject(responsechk);
                            var error = json.error.ToString();
                            var error_decribe = json["error_description"].ToString();
                            var results = "{'message':'" + error_decribe + "','status':'failure'}";
                            var jss = new JavaScriptSerializer();
                            var dict = jss.Deserialize<dynamic>(results);

                            return Json(dict, JsonRequestBehavior.AllowGet);
                        }

                    }
                    else
                    {
                        var results = "{'Message':'Api Name Not Register','Response':'failure'}";
                        var jss = new JavaScriptSerializer();
                        var dict = jss.Deserialize<dynamic>(results);
                        return Json(dict, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    var results = "{'Message':'No api Open.','Response':'failure'}";
                    var jss = new JavaScriptSerializer();
                    var dict = jss.Deserialize<dynamic>(results);
                    return Json(dict, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                var msg = ex.Message.ToString();
                var results = "  { 'Message':'" + msg + "','Response':'failure' }";
                var jss = new JavaScriptSerializer();
                var dict = jss.Deserialize<dynamic>(results);
                return Json(dict, JsonRequestBehavior.AllowGet);
            }
        }

        // delete beneficary id with verfiy otp
        [HttpPost]
        public ActionResult otpverify_cyber1(string type, string sender_number, string otp, string benid, string remiterid, string deletecode)
        {
            try
            {

                var ch = "";
                var apinm = db.money_api_status.Where(aa => aa.status == true && aa.catagory== "PAYOUT").SingleOrDefault();
                var apiname = apinm == null ? "NO" : apinm.api_name;
                if (apiname != "NO")
                {
                    //////////////////Cyber/////////
                    if (apiname == "Cyber")
                    {
                        moneytransfer_cyberplate cb = new moneytransfer_cyberplate();
                        ssl.CERTNo = cert;
                        ch = cb.otp_verify(sender_number, otp, remiterid, benid, deletecode);
                        int start1 = ch.IndexOf("RESULT=") + 7;
                        int end1 = ch.IndexOf("\r\n", start1);
                        string result = ch.Substring(start1, end1 - start1);
                        /////////////ERROR////////////////////////////////////
                        start1 = ch.IndexOf("ERROR=") + 6;
                        end1 = ch.IndexOf("\r\n", start1);
                        string errornm = ch.Substring(start1, end1 - start1);
                        if (result == "0" && errornm == "0")
                        {
                            int start = ch.IndexOf("ADDINFO=") + 8;
                            int end = ch.IndexOf("DATE=", start);
                            result = ch.Substring(start, end - start);
                            result = HttpUtility.UrlDecode(result);
                            var jss = new JavaScriptSerializer();
                            var dict = jss.Deserialize<dynamic>(result);
                            return Json(dict, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            if (ch.ToUpper().Contains("ERRMSG"))
                            {
                                int start = ch.IndexOf("ERRMSG=") + 7;
                                int end = ch.IndexOf("\r\n", start);
                                string errormsg = ch.Substring(start, end - start);
                                var results = "{'Message':'" + errormsg + "','Response':'failure' }";
                                var jss = new JavaScriptSerializer();
                                var dict = jss.Deserialize<dynamic>(results);
                                return Json(dict, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                var error = db.Cyber_error_name_list.Where(aa => aa.error_code == errornm).SingleOrDefault().Error_name;
                                var results = "{'Message':'" + error + "','Response':'failure'}";
                                var jss = new JavaScriptSerializer();
                                var dict = jss.Deserialize<dynamic>(results);
                                return Json(dict, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                    /////////////INSTANTPAY//////////////////
                    else if (apiname.ToUpper() == "INSTANTPAY")
                    {
                        var token = db.Money_API_URLS.Where(a => a.API_Name == apiname).Single().Token;
                        if (HttpContext.Request.IsLocal)
                        {
                            token = "ca23f55293bc9c2a51fe06d2d07e5ea4";
                        }
                        var data = new
                        {
                            token = token,
                            request = new
                            {
                                beneficiaryid = benid,
                                remitterid = remiterid,
                                otp = otp
                            }
                        };


                        string URL = "";
                        if (type == "addben")
                        {
                            URL = "https://www.instantpay.in/ws/dmi/beneficiary_register_validate";
                        }
                        if (type == "deleteben")
                        {
                            URL = "https://www.instantpay.in/ws/dmi/beneficiary_remove_validate";
                        }

                        string jsonData = JsonConvert.SerializeObject(data);
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
                        request.Method = "POST";
                        request.ContentType = "application/json";
                        StreamWriter requestWriter = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.ASCII);
                        requestWriter.Write(jsonData);
                        requestWriter.Close();


                        try
                        {
                            WebResponse webResponse = request.GetResponse();
                            Stream webStream = webResponse.GetResponseStream();
                            StreamReader responseReader = new StreamReader(webStream);
                            string response = responseReader.ReadToEnd();
                            response = response.Replace(@"<?xml version=""1.0"" encoding=""utf-8""?>", "");
                            XmlDocument doc = new XmlDocument();
                            doc.LoadXml(response);


                            string json = JsonConvert.SerializeXmlNode(doc);
                            JavaScriptSerializer ser = new JavaScriptSerializer();
                            var parsed = ser.Deserialize<dynamic>(json);
                            json = ser.Serialize(parsed["xml"]);
                            var jss = new JavaScriptSerializer();
                            var dict = jss.Deserialize<dynamic>(json);
                            return Json(dict, JsonRequestBehavior.AllowGet);


                        }
                        catch (Exception ex)
                        {
                            var msg = ex.Message.ToString();
                            var results = "{'Message':'" + msg + "','Response':'failure'}";
                            var jss = new JavaScriptSerializer();
                            var dict = jss.Deserialize<dynamic>(results);
                            return Json(dict, JsonRequestBehavior.AllowGet);
                        }

                    }
                    //////////////VastWeb////////////////////
                    else if (apiname.ToUpper() == "VASTWEB")
                    {
                        var tokn = db.vastbazzartokens.SingleOrDefault();
                        VastBazaar1 cb1 = new VastBazaar1();

                        DateTime curntdate = DateTime.Now.Date;
                        DateTime expdate = Convert.ToDateTime(tokn.exptime).Date;
                        var responsechk = "";
                        var responsecode1 = "";
                        if (expdate > curntdate)
                        {
                            if (type == "addben")
                            {
                                var responseall = cb1.Beneficiary_register_Validate(benid, remiterid, otp, tokn.apitoken, sender_number);
                                responsechk = responseall.Content.ToString();
                                responsecode1 = responseall.StatusCode.ToString();
                            }
                            else
                            {
                                var responseall = cb1.Beneficiary_Delete_Vaildate(benid, remiterid, otp, tokn.apitoken, sender_number);
                                responsechk = responseall.Content.ToString();
                                responsecode1 = responseall.StatusCode.ToString();
                            }
                        }
                        else
                        {
                            var response = tokencheck();
                            var response1 = response.Content.ToString();
                            var responsecode = response.StatusCode.ToString();
                            if (responsecode == "OK")
                            {
                                dynamic json = JsonConvert.DeserializeObject(responsechk);
                                var token = json.access_token.ToString();
                                var expire = json[".expires"].ToString();
                                DateTime exp = Convert.ToDateTime(expire);
                                tokn.apitoken = token;
                                tokn.exptime = exp;
                                db.SaveChanges();
                                if (type == "addben")
                                {
                                    var responseall = cb1.Beneficiary_register_Validate(benid, remiterid, otp, token, sender_number);
                                    responsechk = responseall.Content.ToString();
                                    responsecode1 = responseall.StatusCode.ToString();
                                }
                                else
                                {
                                    var responseall = cb1.Beneficiary_Delete_Vaildate(benid, remiterid, otp, tokn.apitoken, sender_number);
                                    responsechk = responseall.Content.ToString();
                                    responsecode1 = responseall.StatusCode.ToString();
                                }
                            }
                            else
                            {
                                dynamic json = JsonConvert.DeserializeObject(responsechk);
                                var error = json.error.ToString();
                                var error_decribe = json["error_description"].ToString();
                                var results = "{'message':'" + error_decribe + "','status':'failure'}";
                                var jss = new JavaScriptSerializer();
                                var dict = jss.Deserialize<dynamic>(results);

                                return Json(dict, JsonRequestBehavior.AllowGet);
                            }
                        }

                        if (responsecode1 == "OK")
                        {
                            dynamic json = JsonConvert.DeserializeObject(responsechk);
                            var respcode = json.Content.ResponseCode.ToString();
                            var ADDINFO = json.Content.ADDINFO;
                            var stscode = ADDINFO.statuscode;
                            var results = JsonConvert.SerializeObject(ADDINFO);
                            var jss = new JavaScriptSerializer();
                            var dict = jss.Deserialize<dynamic>(results);
                            return Json(dict, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            dynamic json = JsonConvert.DeserializeObject(responsechk);
                            var error = json.error.ToString();
                            var error_decribe = json["error_description"].ToString();
                            var results = "{'message':'" + error_decribe + "','status':'failure'}";
                            var jss = new JavaScriptSerializer();
                            var dict = jss.Deserialize<dynamic>(results);

                            return Json(dict, JsonRequestBehavior.AllowGet);
                        }

                    }
                    else
                    {
                        var results = "{'Message':'Api Name Not Register','Response':'failure'}";
                        var jss = new JavaScriptSerializer();
                        var dict = jss.Deserialize<dynamic>(results);
                        return Json(dict, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    var results = "{'Message':'No api Open.','Response':'failure'}";
                    var jss = new JavaScriptSerializer();
                    var dict = jss.Deserialize<dynamic>(results);
                    return Json(dict, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                var msg = ex.Message.ToString();
                var results = "  {  'Message':'" + msg + "','Response':'failure' }";
                var jss = new JavaScriptSerializer();
                var dict = jss.Deserialize<dynamic>(results);
                return Json(dict, JsonRequestBehavior.AllowGet);
            }
        }
        //start add new beneficary details
        [HttpPost]
        public ActionResult process_recipientnew_cyber1(string senderNumber, string remiterid, string AccountNo, string beneficiary_name, string Ifsccode)
        {
            try
            {
                var RetailerID = User.Identity.GetUserId();
                Ifsccode = Ifsccode.ToUpper();
                var mob = db.Whitelabel_Retailer_Details.Where(aa => aa.RetailerId == RetailerID).Single().Mobile;
                var reciep_mobile = mob;
                var apinm = db.money_api_status.Where(aa => aa.status == true && aa.catagory== "VASTWEB").SingleOrDefault();
                var apiname = apinm == null ? "NO" : apinm.api_name;
                if (apiname != "NO")
                {
                    //////////////////Cyber/////////
                    if (apiname.ToUpper() == "CYBER")
                    {
                        moneytransfer_cyberplate cb = new moneytransfer_cyberplate();
                        ssl.CERTNo = cert;
                        var ch = cb.add_benificy(AccountNo, senderNumber, Ifsccode, beneficiary_name, remiterid);
                        int start1 = ch.IndexOf("RESULT=") + 7;
                        int end1 = ch.IndexOf("\r\n", start1);
                        string result = ch.Substring(start1, end1 - start1);
                        /////////////ERROR////////////////////////////////////
                        start1 = ch.IndexOf("ERROR=") + 6;
                        end1 = ch.IndexOf("\r\n", start1);
                        string errornm = ch.Substring(start1, end1 - start1);
                        if (result == "0" && errornm == "0")
                        {
                            int start = ch.IndexOf("ADDINFO=") + 8;
                            int end = ch.IndexOf("\r\n", start);
                            string result1 = ch.Substring(start, end - start);
                            result1 = HttpUtility.UrlDecode(result1);
                            var jss = new JavaScriptSerializer();
                            var dict = jss.Deserialize<dynamic>(result1);
                            return Json(dict, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            if (ch.ToUpper().Contains("ERRMSG"))
                            {
                                int start = ch.IndexOf("ERRMSG=") + 7;
                                int end = ch.IndexOf("\r\n", start);
                                string errormsg = ch.Substring(start, end - start);
                                var results = "{'Message':'" + errormsg + "','Response':'failure' }";
                                var jss = new JavaScriptSerializer();
                                var dict = jss.Deserialize<dynamic>(results);
                                return Json(dict, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                var error = db.Cyber_error_name_list.Where(aa => aa.error_code == errornm).SingleOrDefault().Error_name;
                                var results = "{'Message':'" + error + "','Response':'failure'}";
                                var jss = new JavaScriptSerializer();
                                var dict = jss.Deserialize<dynamic>(results);
                                return Json(dict, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                    /////////////INSTANTPAY//////////////////
                    else if (apiname.ToUpper() == "INSTANTPAY")
                    {
                        var token = db.Money_API_URLS.Where(a => a.API_Name == apiname).Single().Token;
                        if (HttpContext.Request.IsLocal)
                        {
                            token = "ca23f55293bc9c2a51fe06d2d07e5ea4";
                        }
                        var data = new
                        {
                            token = token,
                            request = new
                            {
                                remitterid = remiterid,
                                name = beneficiary_name,
                                mobile = senderNumber,
                                ifsc = Ifsccode,
                                account = AccountNo
                            }
                        };
                        string URL = "https://www.instantpay.in/ws/dmi/beneficiary_register";
                        string jsonData = JsonConvert.SerializeObject(data);
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
                        request.Method = "POST";
                        request.ContentType = "application/json";
                        StreamWriter requestWriter = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.ASCII);
                        requestWriter.Write(jsonData);
                        requestWriter.Close();

                        try
                        {
                            WebResponse webResponse = request.GetResponse();
                            Stream webStream = webResponse.GetResponseStream();
                            StreamReader responseReader = new StreamReader(webStream);
                            string response = responseReader.ReadToEnd();
                            response = response.Replace(@"<?xml version=""1.0"" encoding=""utf-8""?>", "");
                            XmlDocument doc = new XmlDocument();
                            doc.LoadXml(response);


                            string json = JsonConvert.SerializeXmlNode(doc);
                            JavaScriptSerializer ser = new JavaScriptSerializer();
                            var parsed = ser.Deserialize<dynamic>(json);
                            json = ser.Serialize(parsed["xml"]);
                            var jss = new JavaScriptSerializer();
                            var dict = jss.Deserialize<dynamic>(json);
                            return Json(dict, JsonRequestBehavior.AllowGet);


                        }
                        catch (Exception ex)
                        {
                            var msg = ex.Message.ToString();
                            var results = "{'Message':'" + msg + "','Response':'failure'}";
                            var jss = new JavaScriptSerializer();
                            var dict = jss.Deserialize<dynamic>(results);
                            return Json(dict, JsonRequestBehavior.AllowGet);
                        }

                    }
                    //////////////VastWeb/////////////
                    else if (apiname.ToUpper() == "VASTWEB")
                    {
                        var tokn = db.vastbazzartokens.SingleOrDefault();
                        VastBazaar1 cb1 = new VastBazaar1();

                        DateTime curntdate = DateTime.Now.Date;
                        DateTime expdate = Convert.ToDateTime(tokn.exptime).Date;
                        var responsechk = "";
                        var responsecode1 = "";
                        if (expdate > curntdate)
                        {
                            var responseall = cb1.Beneficiary_register(remiterid, beneficiary_name, senderNumber, Ifsccode, AccountNo, tokn.apitoken);
                            responsechk = responseall.Content.ToString();
                            responsecode1 = responseall.StatusCode.ToString();
                        }
                        else
                        {
                            var response = tokencheck();
                            var response1 = response.Content.ToString();
                            var responsecode = response.StatusCode.ToString();
                            if (responsecode == "OK")
                            {
                                dynamic json = JsonConvert.DeserializeObject(responsechk);
                                var token = json.access_token.ToString();
                                var expire = json[".expires"].ToString();
                                DateTime exp = Convert.ToDateTime(expire);
                                tokn.apitoken = token;
                                tokn.exptime = exp;
                                db.SaveChanges();
                                var responseall = cb1.Beneficiary_register(remiterid, beneficiary_name, senderNumber, Ifsccode, AccountNo, token);
                                responsechk = responseall.Content.ToString();
                                responsecode1 = responseall.StatusCode.ToString();
                            }
                            else
                            {
                                dynamic json = JsonConvert.DeserializeObject(responsechk);
                                var error = json.error.ToString();
                                var error_decribe = json["error_description"].ToString();
                                var results = "{'message':'" + error_decribe + "','status':'failure'}";
                                var jss = new JavaScriptSerializer();
                                var dict = jss.Deserialize<dynamic>(results);

                                return Json(dict, JsonRequestBehavior.AllowGet);
                            }
                        }

                        if (responsecode1 == "OK")
                        {
                            dynamic json = JsonConvert.DeserializeObject(responsechk);
                            var respcode = json.Content.ResponseCode.ToString();
                            var ADDINFO = json.Content.ADDINFO;
                            var stscode = ADDINFO.statuscode;


                            var results = JsonConvert.SerializeObject(ADDINFO);
                            var jss = new JavaScriptSerializer();
                            var dict = jss.Deserialize<dynamic>(results);
                            return Json(dict, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            dynamic json = JsonConvert.DeserializeObject(responsechk);
                            var error = json.error.ToString();
                            var error_decribe = json["error_description"].ToString();
                            var results = "{'message':'" + error_decribe + "','status':'failure'}";
                            var jss = new JavaScriptSerializer();
                            var dict = jss.Deserialize<dynamic>(results);

                            return Json(dict, JsonRequestBehavior.AllowGet);
                        }

                    }
                    else
                    {
                        var results = "{'Message':'Api Name Not Register','Response':'failure'}";
                        var jss = new JavaScriptSerializer();
                        var dict = jss.Deserialize<dynamic>(results);
                        return Json(dict, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    var results = "{'Message':'No api Open.','Response':'failure'}";
                    var jss = new JavaScriptSerializer();
                    var dict = jss.Deserialize<dynamic>(results);
                    return Json(dict, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                var msg = ex.Message.ToString();
                var results = "{'Message':'" + msg + "','Response':'failure'}";
                var jss = new JavaScriptSerializer();
                var dict = jss.Deserialize<dynamic>(results);
                return Json(dict, JsonRequestBehavior.AllowGet);
            }
        }

        //start Resend Otp of delete beneficiary  and add new beneficiary
        [HttpPost]
        public ActionResult BeneficiaryRegistrationResendOTP1(string RemitterId, string BenId, string senderno, string sendernm, string Type)
        {
            try
            {

                var apinm = db.money_api_status.Where(aa => aa.status == true && aa.catagory== "PAYOUT").SingleOrDefault();
                var apiname = apinm == null ? "NO" : apinm.api_name;
                if (apiname != "NO")
                {
                    if (apiname.ToUpper() == "CYBER")
                    {
                        moneytransfer_cyberplate cb = new moneytransfer_cyberplate();
                        var ch = cb.resend_otp(RemitterId, BenId, senderno, sendernm);
                        int start1 = ch.IndexOf("RESULT=") + 7;
                        int end1 = ch.IndexOf("\r\n", start1);
                        string result = ch.Substring(start1, end1 - start1);
                        /////////////ERROR////////////////////////////////////
                        start1 = ch.IndexOf("ERROR=") + 6;
                        end1 = ch.IndexOf("\r\n", start1);
                        string errornm = ch.Substring(start1, end1 - start1);
                        if (result == "0" && errornm == "0")
                        {
                            int start = ch.IndexOf("ADDINFO=") + 8;
                            int end = ch.IndexOf("DATE=", start);
                            var results = ch.Substring(start, end - start);
                            results = HttpUtility.UrlDecode(results);
                            dynamic jobject = JsonConvert.DeserializeObject(results);
                            var stscode = jobject.statuscode;
                            if (stscode == "TXN")
                            {
                                return Json("Success", JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                return Json("Failed", JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            return Json("Failed", JsonRequestBehavior.AllowGet);
                        }

                    }
                    else if (apiname.ToUpper() == "INSTANTPAY")
                    {
                        var token = (db.Money_API_URLS.Where(aa => aa.API_Name == apiname).Single().Token);
                        if (HttpContext.Request.IsLocal)
                        {
                            token = "5a7db562faadad009077b5c973901d7c";
                        }

                        HttpClient client = new HttpClient();
                        client.BaseAddress = new Uri("https://www.instantpay.in/ws/dmi/");
                        client.DefaultRequestHeaders.Accept.Clear();
                        dynamic request = new JObject();
                        request.token = token;
                        dynamic request1 = new JObject();
                        request1.remitterid = RemitterId;
                        request1.beneficiaryid = BenId;
                        request.request = request1;


                        ////
                        client.BaseAddress = new Uri("https://www.instantpay.in/ws/dmi/");
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        HttpResponseMessage response = new HttpResponseMessage();
                        if (Type == "addben")
                        {
                            response = client.PostAsJsonAsync("beneficiary_resend_otp", (JObject)request).Result;
                            response.EnsureSuccessStatusCode();
                        }
                        else if (Type == "deleteben")
                        {
                            response = client.PostAsJsonAsync("beneficiary_remove", (JObject)request).Result;
                            response.EnsureSuccessStatusCode();
                        }

                        if (response.IsSuccessStatusCode)
                        {
                            var contents = response.Content.ReadAsStringAsync();
                            var jsonString = contents.Result;

                            if (jsonString != null)
                            {
                                dynamic dynObject = JsonConvert.DeserializeObject(jsonString);

                                if (dynObject["statuscode"] == "TXN")
                                {
                                    return Json("Success", JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    return Json("Failed", JsonRequestBehavior.AllowGet);
                                }
                            }
                            else
                            {
                                //var results = "Please Try After Some Time!";
                                //dynamic res = new JObject();
                                //res.RESULT = "1";
                                //res.ADDINFO = results;
                                return Json("Faliled", JsonRequestBehavior.AllowGet);

                            }
                        }
                        else
                        {
                            //var results = "Somthing went wrong!";
                            //dynamic res = new JObject();
                            //res.RESULT = "1";
                            //res.ADDINFO = results;
                            return Json("Faliled", JsonRequestBehavior.AllowGet);
                        }
                    }
                    else if (apiname.ToUpper() == "VASTWEB")
                    {
                        var tokn = db.vastbazzartokens.SingleOrDefault();
                        VastBazaar1 cb1 = new VastBazaar1();

                        DateTime curntdate = DateTime.Now.Date;
                        DateTime expdate = Convert.ToDateTime(tokn.exptime).Date;
                        var responsechk = "";
                        var responsecode1 = "";
                        if (expdate > curntdate)
                        {
                            var responseall = cb1.Beneficiary_register_resend_otp(RemitterId, BenId, tokn.apitoken);
                            responsechk = responseall.Content.ToString();
                            responsecode1 = responseall.StatusCode.ToString();
                        }
                        else
                        {
                            var response = tokencheck();
                            var response1 = response.Content.ToString();
                            var responsecode = response.StatusCode.ToString();
                            if (responsecode == "OK")
                            {
                                dynamic json = JsonConvert.DeserializeObject(responsechk);
                                var token = json.access_token.ToString();
                                var expire = json[".expires"].ToString();
                                DateTime exp = Convert.ToDateTime(expire);
                                tokn.apitoken = token;
                                tokn.exptime = exp;
                                db.SaveChanges();
                                var responseall = cb1.Beneficiary_register_resend_otp(RemitterId, BenId, token);
                                responsechk = responseall.Content.ToString();
                                responsecode1 = responseall.StatusCode.ToString();
                            }
                            else
                            {
                                dynamic json = JsonConvert.DeserializeObject(responsechk);
                                var error = json.error.ToString();
                                var error_decribe = json["error_description"].ToString();
                                var results = "{'message':'" + error_decribe + "','status':'failure'}";
                                var jss = new JavaScriptSerializer();
                                var dict = jss.Deserialize<dynamic>(results);

                                return Json(dict, JsonRequestBehavior.AllowGet);
                            }
                        }

                        if (responsecode1 == "OK")
                        {
                            dynamic json = JsonConvert.DeserializeObject(responsechk);
                            var respcode = json.Content.ResponseCode.ToString();
                            var ADDINFO = json.Content.ADDINFO;
                            var stscode = ADDINFO.statuscode;



                            if (stscode == "TXN")
                            {

                                var ser = JsonConvert.SerializeObject(ADDINFO);
                                var jss1 = new JavaScriptSerializer();
                                var dict1 = jss1.Deserialize<dynamic>(ser);
                                return Json(dict1, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {

                                var ser = JsonConvert.SerializeObject(ADDINFO);
                                var jss1 = new JavaScriptSerializer();
                                var dict1 = jss1.Deserialize<dynamic>(ser);
                                return Json(dict1, JsonRequestBehavior.AllowGet);

                            }

                        }
                        else
                        {
                            dynamic json = JsonConvert.DeserializeObject(responsechk);
                            var error = json.error.ToString();
                            var error_decribe = json["error_description"].ToString();
                            var results = "{'message':'" + error_decribe + "','status':'failure'}";
                            var jss = new JavaScriptSerializer();
                            var dict = jss.Deserialize<dynamic>(results);

                            return Json(dict, JsonRequestBehavior.AllowGet);
                        }

                    }
                    else
                    {
                        return Json("Faliled", JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json("Faliled", JsonRequestBehavior.AllowGet);
                }

            }
            catch
            {
                return Json("Faliled", JsonRequestBehavior.AllowGet);
            }
        }
        //end

        //instantpay account number verify code 
        [HttpPost]
        public ActionResult process_recipient_verify_cyber1(string NUMBER, string benIFSC, string benCode, string account, int kycstatus, string bankname)
        {
            try
            {
                moneytransfer_cyberplate cb = new moneytransfer_cyberplate();
                var apinm = db.money_api_status.Where(aa => aa.status == true && aa.catagory== "PAYOUT").SingleOrDefault();
                string CommonTranid = "W" + DateTime.Parse(DateTime.Now.ToString()).ToString("yyMMddHHmmss") + cb.RandomString(4);
                var apiname = apinm == null ? "NO" : apinm.api_name;
                benIFSC = benIFSC.ToUpper();
                string userid = User.Identity.GetUserId();
                // get mac address
                var macaddress = GetMACAddress();
                //Ip current Ip Address
                var Ipaddress = GetComputer_InternetIP();
                var kycsts = "";
                if (kycstatus == 1)
                {
                    kycsts = "Y";
                }
                else
                {
                    kycsts = "N";
                }

                string URL = ""; string jsonData = ""; var requestsend = "";
                if (apiname != "NO")
                {
                    if (apiname == "Cyber")
                    {

                        var ch = cb.Verify_benificy(NUMBER, benIFSC, benCode, account, userid, bankname, kycsts, Ipaddress, macaddress, CommonTranid);
                        if (ch == "RETAILERLOW")
                        {
                            var results = "{'Message':'Remain Balance Low.','Response':'failure' }";
                            var jss = new JavaScriptSerializer();
                            var dict = jss.Deserialize<dynamic>(results);
                            return Json(dict, JsonRequestBehavior.AllowGet);
                        }
                        else if (ch == "DEALERLOW")
                        {
                            var results = "{'Message':'Dealer Remain Balance Low.','Response':'failure' }";
                            var jss = new JavaScriptSerializer();
                            var dict = jss.Deserialize<dynamic>(results);
                            return Json(dict, JsonRequestBehavior.AllowGet);
                        }
                        else if (ch == "MASTERLOW")
                        {
                            var results = "{'Message':'Admin Remain Balance Low.','Response':'failure' }";
                            var jss = new JavaScriptSerializer();
                            var dict = jss.Deserialize<dynamic>(results);
                            return Json(dict, JsonRequestBehavior.AllowGet);
                        }
                        else if (ch == "STATUSDOWN")
                        {
                            var results = "{'Message':'Your DMT status inactive please contact to Admin','Response':'failure' }";
                            var jss = new JavaScriptSerializer();
                            var dict = jss.Deserialize<dynamic>(results);
                            return Json(dict, JsonRequestBehavior.AllowGet);
                        }
                        else if (ch == "CAPPINGLOW")
                        {
                            var results = "{'Message':'Capping Low.','Response':'failure' }";
                            var jss = new JavaScriptSerializer();
                            var dict = jss.Deserialize<dynamic>(results);
                            return Json(dict, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            int start1 = ch.IndexOf("RESULT=") + 7;
                            int end1 = ch.IndexOf("\r\n", start1);
                            string result = ch.Substring(start1, end1 - start1);
                            /////////////ERROR////////////////////////////////////
                            start1 = ch.IndexOf("ERROR=") + 6;
                            end1 = ch.IndexOf("\r\n", start1);
                            string errornm = ch.Substring(start1, end1 - start1);
                            if (result == "0" && errornm == "0")
                            {
                                if (ch.Contains("ADDINFO="))
                                {
                                    start1 = ch.IndexOf("TRNXSTATUS=") + 11;
                                    end1 = ch.IndexOf("\r\n", start1);
                                    string transts = ch.Substring(start1, end1 - start1);
                                    if (transts == "7")
                                    {
                                        int start = ch.IndexOf("ADDINFO=") + 8;
                                        int end = ch.IndexOf("AUTHCODE", start);
                                        string result1 = ch.Substring(start, end - start);
                                        result1 = HttpUtility.UrlDecode(result1);
                                        dynamic jsonObject = JsonConvert.DeserializeObject(result1);
                                        if (jsonObject.statuscode == "TXN")
                                        {
                                            var receiverName = jsonObject.data.benename.ToString();
                                            var bankrefno = jsonObject.data.bankrefno.ToString();
                                            db.Money_transfer_update_new_new(CommonTranid, "SUCCESS", bankrefno, receiverName, ch, "", 0, 0);

                                            var jss = new JavaScriptSerializer();
                                            var dict = jss.Deserialize<dynamic>(result1);
                                            return Json(dict, JsonRequestBehavior.AllowGet);

                                        }
                                        else if (jsonObject.statuscode == "TUP")
                                        {
                                            var results = "{'Message':'Transaction under process','Response':'Pending' }";
                                            var jss = new JavaScriptSerializer();
                                            var dict = jss.Deserialize<dynamic>(results);
                                            return Json(dict, JsonRequestBehavior.AllowGet);
                                        }
                                        else
                                        {
                                            if (ch.ToUpper().Contains("ERRMSG"))
                                            {

                                                start = ch.IndexOf("ERRMSG=") + 7;
                                                end = ch.IndexOf("\r\n", start);
                                                string errormsg = ch.Substring(start, end - start);
                                                var results = "{'Message':'" + errormsg + "','Response':'failure' }";
                                                db.Money_transfer_update_new_new(CommonTranid, "FAILED", errormsg, "", ch, "", 0, 0);
                                                var jss = new JavaScriptSerializer();
                                                var dict = jss.Deserialize<dynamic>(results);
                                                return Json(dict, JsonRequestBehavior.AllowGet);
                                            }
                                            else
                                            {
                                                var error = db.Cyber_error_name_list.Where(aa => aa.error_code == errornm).SingleOrDefault().Error_name;
                                                var results = "{'Message':'" + error + "','Response':'failure'}";
                                                db.Money_transfer_update_new_new(CommonTranid, "FAILED", results, "", ch, "", 0, 0);
                                                var jss = new JavaScriptSerializer();
                                                var dict = jss.Deserialize<dynamic>(results);
                                                return Json(dict, JsonRequestBehavior.AllowGet);
                                            }
                                        }

                                    }
                                    else if (transts == "3")
                                    {
                                        var results = "{'Message':'Transaction under process','Response':'Pending' }";
                                        var jss = new JavaScriptSerializer();
                                        var dict = jss.Deserialize<dynamic>(results);
                                        return Json(dict, JsonRequestBehavior.AllowGet);
                                    }
                                }
                                else
                                {
                                    var results = "{'Message':'Transaction under process','Response':'Pending' }";
                                    var jss = new JavaScriptSerializer();
                                    var dict = jss.Deserialize<dynamic>(results);
                                    return Json(dict, JsonRequestBehavior.AllowGet);
                                }
                            }
                            else if (result == "1" && errornm == "36")
                            {
                                var results = "{'Message':'Transaction under process','Response':'Pending' }";
                                var jss = new JavaScriptSerializer();
                                var dict = jss.Deserialize<dynamic>(results);
                                return Json(dict, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                if (ch.ToUpper().Contains("ERRMSG"))
                                {
                                    int start = ch.IndexOf("ERRMSG=") + 7;
                                    int end = ch.IndexOf("\r\n", start);
                                    string errormsg = ch.Substring(start, end - start);
                                    db.Money_transfer_update_new_new(CommonTranid, "FAILED", errormsg, "", ch, "", 0, 0);
                                    var results = "{'Message':'" + errormsg + "','Response':'failure' }";
                                    var jss = new JavaScriptSerializer();
                                    var dict = jss.Deserialize<dynamic>(results);
                                    return Json(dict, JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    var error = db.Cyber_error_name_list.Where(aa => aa.error_code == errornm).SingleOrDefault().Error_name;
                                    var results = "{'Message':'" + error + "','Response':'failure'}";
                                    db.Money_transfer_update_new_new(CommonTranid, "FAILED", results, "", ch, "", 0, 0);
                                    var jss = new JavaScriptSerializer();
                                    var dict = jss.Deserialize<dynamic>(results);
                                    return Json(dict, JsonRequestBehavior.AllowGet);
                                }
                            }
                        }
                    }
                    else if (apiname.ToUpper() == "INSTANTPAY")
                    {
                        var token = db.Money_API_URLS.Where(a => a.API_Name == apiname).Single().Token;
                        if (HttpContext.Request.IsLocal)
                        {
                            token = "ca23f55293bc9c2a51fe06d2d07e5ea4";
                        }
                        var data = new
                        {
                            token = token,
                            request = new
                            {
                                remittermobile = NUMBER,
                                account = account,
                                ifsc = benIFSC,
                                agentid = CommonTranid

                            }
                        };


                        URL = "https://www.instantpay.in/ws/imps/account_validate";
                        jsonData = JsonConvert.SerializeObject(data);
                        requestsend = jsonData.ToString();
                        System.Data.Entity.Core.Objects.ObjectParameter outputchk = new System.Data.Entity.Core.Objects.ObjectParameter("Output", typeof(string));
                        var ch = db.whitelabel_Money_transfer(userid, 0, 0, NUMBER, account, bankname, benIFSC, CommonTranid, CommonTranid, "IMPS_VERIFY", "ONLINE", kycsts, requestsend, apiname, Ipaddress, macaddress, "", "DMT1", outputchk).Single().msg;
                        if (ch == "RETAILERLOW")
                        {
                            var results = "{'Message':'Remain Balance Low.','Response':'failure' }";
                            var jss = new JavaScriptSerializer();
                            var dict = jss.Deserialize<dynamic>(results);
                            return Json(dict, JsonRequestBehavior.AllowGet);
                        }
                        else if (ch == "DEALERLOW")
                        {
                            var results = "{'Message':'Dealer Remain Balance Low.','Response':'failure' }";
                            var jss = new JavaScriptSerializer();
                            var dict = jss.Deserialize<dynamic>(results);
                            return Json(dict, JsonRequestBehavior.AllowGet);
                        }
                        else if (ch == "MASTERLOW")
                        {
                            var results = "{'Message':'Admin Remain Balance Low.','Response':'failure' }";
                            var jss = new JavaScriptSerializer();
                            var dict = jss.Deserialize<dynamic>(results);
                            return Json(dict, JsonRequestBehavior.AllowGet);
                        }
                        else if (ch == "STATUSDOWN")
                        {
                            var results = "{'Message':'Your DMT status inactive please contact to Admin','Response':'failure' }";
                            var jss = new JavaScriptSerializer();
                            var dict = jss.Deserialize<dynamic>(results);
                            return Json(dict, JsonRequestBehavior.AllowGet);
                        }
                        else if (ch == "CAPPINGLOW")
                        {
                            var results = "{'Message':'Capping Low.','Response':'failure' }";
                            var jss = new JavaScriptSerializer();
                            var dict = jss.Deserialize<dynamic>(results);
                            return Json(dict, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
                            request.Method = "POST";
                            request.ContentType = "application/json";

                            StreamWriter requestWriter = new StreamWriter(request.GetRequestStream(),
                                System.Text.Encoding.ASCII);
                            requestWriter.Write(jsonData);
                            requestWriter.Close();

                            try
                            {
                                WebResponse webResponse = request.GetResponse();
                                Stream webStream = webResponse.GetResponseStream();
                                StreamReader responseReader = new StreamReader(webStream);
                                string response = responseReader.ReadToEnd();
                                response = response.Replace(@"<?xml version=""1.0"" encoding=""utf-8""?>", "");
                                XmlDocument doc = new XmlDocument();
                                doc.LoadXml(response);

                                string json = JsonConvert.SerializeXmlNode(doc);
                                JavaScriptSerializer ser = new JavaScriptSerializer();
                                var parsed = ser.Deserialize<dynamic>(json);
                                json = ser.Serialize(parsed["xml"]);
                                // save response to DB                                

                                var receiverName = "";
                                var bankrefno = "";
                                //Convert jsonString  to  json
                                dynamic jsonObject = JsonConvert.DeserializeObject(json);
                                if (jsonObject.statuscode == "TXN")
                                {
                                    receiverName = jsonObject.data.benename.ToString();
                                    bankrefno = jsonObject.data.bankrefno.ToString();

                                    //money transfer update procedure
                                    db.Money_transfer_update_new_new(CommonTranid, "SUCCESS", bankrefno, receiverName, json, "", 0, 0);
                                    var jss = new JavaScriptSerializer();
                                    var dict = jss.Deserialize<dynamic>(json);
                                    return Json(dict, JsonRequestBehavior.AllowGet);

                                }
                                else if (jsonObject.statuscode == "TUP")
                                {
                                    var results = "{'Message':'Transaction under process','Response':'Pending'}";
                                    var jss = new JavaScriptSerializer();
                                    var dict = jss.Deserialize<dynamic>(results);
                                    return Json(dict, JsonRequestBehavior.AllowGet);
                                }
                                else
                                {

                                    db.Money_transfer_update_new_new(CommonTranid, "FAILED", bankrefno, receiverName, json, "", 0, 0);
                                    var jss = new JavaScriptSerializer();
                                    var dict = jss.Deserialize<dynamic>(json);
                                    return Json(dict, JsonRequestBehavior.AllowGet);
                                }

                                //
                            }
                            catch (Exception ex)
                            {
                                var msg = ex.Message.ToString();
                                var results = "{'Message':'" + msg + "','Response':'failure'}";
                                var jss = new JavaScriptSerializer();
                                var dict = jss.Deserialize<dynamic>(results);
                                return Json(dict, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                    else if (apiname.ToUpper() == "VASTWEB")
                    {
                        var tokn = db.vastbazzartokens.SingleOrDefault();
                        requestsend = "{\"remittermobile\":\"" + NUMBER + "\",\"account\":\"" + account + "\",\"ifsc\":\"" + benIFSC + "\",\"agentid\":\"" + CommonTranid + "\"}";
                        System.Data.Entity.Core.Objects.ObjectParameter outputchk = new System.Data.Entity.Core.Objects.ObjectParameter("Output", typeof(string));
                        var ch = db.whitelabel_Money_transfer(userid, 0, 0, NUMBER, account, bankname, benIFSC, CommonTranid, CommonTranid, "IMPS_VERIFY", "ONLINE", kycsts, requestsend, apiname, Ipaddress, macaddress, "", "DMT1", outputchk).Single().msg;
                        if (ch == "RETAILERLOW")
                        {
                            var results = "{'Message':'Remain Balance Low.','Response':'failure' }";
                            var jss = new JavaScriptSerializer();
                            var dict = jss.Deserialize<dynamic>(results);
                            return Json(dict, JsonRequestBehavior.AllowGet);
                        }
                        else if (ch == "DEALERLOW")
                        {
                            var results = "{'Message':'Dealer Remain Balance Low.','Response':'failure' }";
                            var jss = new JavaScriptSerializer();
                            var dict = jss.Deserialize<dynamic>(results);
                            return Json(dict, JsonRequestBehavior.AllowGet);
                        }
                        else if (ch == "MASTERLOW")
                        {
                            var results = "{'Message':'Admin Remain Balance Low.','Response':'failure' }";
                            var jss = new JavaScriptSerializer();
                            var dict = jss.Deserialize<dynamic>(results);
                            return Json(dict, JsonRequestBehavior.AllowGet);
                        }
                        else if (ch == "STATUSDOWN")
                        {
                            var results = "{'Message':'Your DMT status inactive please contact to Admin','Response':'failure' }";
                            var jss = new JavaScriptSerializer();
                            var dict = jss.Deserialize<dynamic>(results);
                            return Json(dict, JsonRequestBehavior.AllowGet);
                        }
                        else if (ch == "CAPPINGLOW")
                        {
                            var results = "{'Message':'Capping Low.','Response':'failure' }";
                            var jss = new JavaScriptSerializer();
                            var dict = jss.Deserialize<dynamic>(results);
                            return Json(dict, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            System.Threading.Thread.Sleep(1000);
                            VastBazaar1 cb1 = new VastBazaar1();

                            DateTime curntdate = DateTime.Now.Date;
                            DateTime expdate = Convert.ToDateTime(tokn.exptime).Date;
                            var responsechk = "";
                            var responsecode1 = "";
                            if (expdate > curntdate)
                            {
                                var responseall = cb1.Beneficiary_Account_verify(NUMBER, account, benIFSC, CommonTranid, tokn.apitoken, bankname);
                                responsechk = responseall.Content.ToString();
                                responsecode1 = responseall.StatusCode.ToString();
                            }
                            else
                            {
                                var response = tokencheck();
                                var response1 = response.Content.ToString();
                                var responsecode = response.StatusCode.ToString();
                                if (responsecode == "OK")
                                {
                                    dynamic json = JsonConvert.DeserializeObject(responsechk);
                                    var token = json.access_token.ToString();
                                    var expire = json[".expires"].ToString();
                                    DateTime exp = Convert.ToDateTime(expire);
                                    tokn.apitoken = token;
                                    tokn.exptime = exp;
                                    db.SaveChanges();
                                    var responseall = cb1.Beneficiary_Account_verify(NUMBER, account, benIFSC, CommonTranid, token, bankname);
                                    responsechk = responseall.Content.ToString();
                                    responsecode1 = responseall.StatusCode.ToString();
                                }
                                else
                                {
                                    dynamic json = JsonConvert.DeserializeObject(responsechk);
                                    db.Money_transfer_update_new_new(CommonTranid, "FAILED", "", "", json.ToString(), "", 0, 0);
                                    var error = json.error.ToString();
                                    var error_decribe = json["error_description"].ToString();
                                    var results = "{'Message':'" + error_decribe + "','Response':'failure'}";
                                    var jss = new JavaScriptSerializer();
                                    var dict = jss.Deserialize<dynamic>(results);

                                    return Json(dict, JsonRequestBehavior.AllowGet);
                                }
                            }

                            if (responsecode1 == "OK")
                            {
                                dynamic json = JsonConvert.DeserializeObject(responsechk);
                                var respcode = json.Content.ResponseCode.ToString();
                                var ADDINFO = json.Content.ADDINFO;
                                var stscode = ADDINFO.statuscode;

                                if (stscode == "TXN")
                                {
                                    var receiverName = ADDINFO.data.benename.ToString();
                                    var bankrefno = ADDINFO.data.bankrefno.ToString();

                                    //money transfer update procedure
                                    db.Money_transfer_update_new_new(CommonTranid, "SUCCESS", bankrefno, receiverName, ADDINFO.ToString(), "", 0, 0);
                                    var ser = JsonConvert.SerializeObject(ADDINFO);
                                    var jss1 = new JavaScriptSerializer();
                                    var dict1 = jss1.Deserialize<dynamic>(ser);
                                    return Json(dict1, JsonRequestBehavior.AllowGet);

                                }
                                else if (stscode == "TUP")
                                {
                                    var results1 = "{'Message':'Transaction under process','Response':'Pending'}";
                                    var jss1 = new JavaScriptSerializer();
                                    var dict1 = jss1.Deserialize<dynamic>(results1);
                                    return Json(dict1, JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    db.Money_transfer_update_new_new(CommonTranid, "FAILED", "", "", ADDINFO.ToString(), "", 0, 0);
                                    var ser = JsonConvert.SerializeObject(ADDINFO);
                                    var jss1 = new JavaScriptSerializer();
                                    var dict1 = jss1.Deserialize<dynamic>(ser);
                                    return Json(dict1, JsonRequestBehavior.AllowGet);
                                }
                            }
                            else
                            {
                                dynamic json = JsonConvert.DeserializeObject(responsechk);

                                db.Money_transfer_update_new_new(CommonTranid, "FAILED", "", "", json, "", 0, 0);
                                if (json == "Invalid response..")
                                {
                                    var results = "{'Message':'Invalid Response','Response':'failure'}";
                                    var jss = new JavaScriptSerializer();
                                    var dict = jss.Deserialize<dynamic>(results);
                                    return Json(dict, JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    var error = json.error.ToString();
                                    var error_decribe = json["error_description"].ToString();
                                    var results = "{'Message':'" + error_decribe + "','Response':'failure'}";
                                    var jss = new JavaScriptSerializer();
                                    var dict = jss.Deserialize<dynamic>(results);
                                    return Json(dict, JsonRequestBehavior.AllowGet);
                                }
                            }
                        }
                    }
                    else
                    {
                        var results = "{'Message':'Api Name Not Register','Response':'failure'}";
                        var jss = new JavaScriptSerializer();
                        var dict = jss.Deserialize<dynamic>(results);
                        return Json(dict, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    var results = "{'Message':'No api Open.','Response':'failure'}";
                    var jss = new JavaScriptSerializer();
                    var dict = jss.Deserialize<dynamic>(results);
                    return Json(dict, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                var msg = ex.Message.ToString();
                var results = "  {  'Message':'" + msg + "','Response':'failure' }";
                var jss = new JavaScriptSerializer();
                var dict = jss.Deserialize<dynamic>(results);
                return Json(dict, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);

        }
        // IMPS Transfer

        [HttpPost]
        public ActionResult process_moneytransfer_cyber1(string SenderNumber, string benIFSC, string benCode, string transamount, string key, string bank_account, string transtype, string remiterid, int kycstatus, string bankname, string totalremaincheck)
        {
            var results = ""; var requestsend = ""; var kycsts = ""; string URL = ""; string jsonData = "";
            try
            {

                string userid = User.Identity.GetUserId();
                var dealerid = db.Whitelabel_Retailer_Details.Where(p => p.RetailerId == userid).Single().DealerId;
                var whitelabelid = db.whitelabel_Dealer_Details.Where(a => a.DealerId == dealerid).Single().Whitelabelid;
                var StatusSendSmsMoneyTransferSuccess = db.Whitelabel_SMSSendAll.Where(a => a.ServiceName == "dmtsucconline" && a.Whitelabelid == whitelabelid).SingleOrDefault().Status;
                var StatusSendSmsMoneyTransferFailed = db.Whitelabel_SMSSendAll.Where(a => a.ServiceName == "dmtfailedonline" && a.Whitelabelid == whitelabelid).SingleOrDefault().Status;
                var RetailerMob = db.Whitelabel_Retailer_Details.Where(p => p.RetailerId == userid).Single().Mobile;
                moneytransfer_cyberplate cb = new moneytransfer_cyberplate();
                var apinm = db.money_api_status.Where(aa => aa.status ==true && aa.catagory== "PAYOUT").SingleOrDefault();
                string CommonTranid = "W" + DateTime.Parse(DateTime.Now.ToString()).ToString("yyMMddHHmmss") + cb.RandomString(4);
                var apiname = apinm == null ? "NO" : apinm.api_name;
                var pin = Encrypt(key);
                var pin_check = (from pi in db.Whitelabel_Retailer_Details where pi.RetailerId == userid select pi).Single().PIN;
                // get mac address
                var macaddress = GetMACAddress();
                //Ip current Ip Address
                var Ipaddress = GetComputer_InternetIP();
                dynamic Response = new JObject();
                if (kycstatus == 1)
                {
                    kycsts = "Y";
                }
                else
                {
                    kycsts = "N";
                }
                if (pin_check == pin)
                {
                    if (Convert.ToDecimal(transamount) <= Convert.ToDecimal(totalremaincheck))
                    {
                        /***** 3 Minutes wait for same amount transfer*****/
                        decimal finalamount = Convert.ToDecimal(transamount);
                        var ch1 = db.IMPS_transtion_detsils.Where(aa => aa.accountno == bank_account && aa.rch_from == userid && aa.totalamount == finalamount && aa.Status.ToUpper() == "SUCCESS").OrderByDescending(aa => aa.idno).ToList();
                        var date = ch1.Any() ? ch1.FirstOrDefault().trans_time : System.DateTime.Now.AddDays(-1);
                        int ggg = Convert.ToInt32((System.DateTime.Now - Convert.ToDateTime(date)).TotalSeconds);
                        if (ggg >= 180)
                        {
                            var remain = (from mon in db.Whitelabel_Remain_reteller_balance where mon.RetellerId == userid select mon).Single().Remainamount;
                            if (remain >= 1)
                            {
                                if (apiname != "NO")
                                {
                                    int amt = Convert.ToInt32(transamount);
                                    if (amt <= 25000)
                                    {
                                        System.Data.Entity.Core.Objects.ObjectParameter outputchk = new System.Data.Entity.Core.Objects.ObjectParameter("Output", typeof(string));
                                        var remainchk = db.whitelabel_chk_remain_amount(Convert.ToDecimal(transamount), userid, outputchk).Single().msg;
                                        if (remainchk == "DONE")
                                        {
                                            if (apiname == "INSTANTPAY")
                                            {
                                                /***************************  Is Bank Down  ***********************/
                                                string respon = new InstantPayComnUtil().getBankList(bank_account);
                                                if (respon != "ERROR")
                                                {
                                                    dynamic dynJson = JsonConvert.DeserializeObject(respon.ToString());
                                                    List<BankNameModel> Details = dynJson.data.ToObject<List<BankNameModel>>();
                                                    var entry = Details.Where(aaa => aaa.branch_ifsc.Contains(benIFSC)).FirstOrDefault();
                                                    if (entry != null && entry.is_down == "1")
                                                    {
                                                        var msg = "" + entry.bank_name + " is down, please try latter.";
                                                        var js = "{'Details':'" + msg + "','status':'Failed'}";
                                                        var jss1 = new JavaScriptSerializer();
                                                        var dict1 = jss1.Deserialize<dynamic>(js);
                                                        return Json(dict1, JsonRequestBehavior.AllowGet);
                                                    }
                                                }
                                                /***************************  END  ***********************/
                                            }
                                            else if (apiname.ToUpper() == "CYBER")
                                            {
                                                var ch = cb.getbankname();
                                                int start1 = ch.IndexOf("ERROR=") + 6;
                                                int end1 = ch.IndexOf("\r\n", start1);
                                                string result = ch.Substring(start1, end1 - start1);
                                                results = "";
                                                if (result == "0")
                                                {
                                                    int start = ch.IndexOf("ADDINFO=") + 8;
                                                    int end = ch.IndexOf("DATE=", start);
                                                    results = ch.Substring(start, end - start);
                                                    results = HttpUtility.UrlDecode(results);
                                                    dynamic dynJson = JsonConvert.DeserializeObject(results.ToString());
                                                    List<BankNameModel> Details = dynJson.data.ToObject<List<BankNameModel>>();
                                                    var entry = Details.Where(aaa => aaa.branch_ifsc.Contains(benIFSC)).FirstOrDefault();
                                                    if (entry != null && entry.is_down == "1")
                                                    {
                                                        var msg = "" + entry.bank_name + " is down, please try latter.";
                                                        var js = "  {'Details':'" + msg + "','status':'Failed'}";
                                                        var jss1 = new JavaScriptSerializer();
                                                        var dict1 = jss1.Deserialize<dynamic>(js);
                                                        return Json(dict1, JsonRequestBehavior.AllowGet);
                                                    }
                                                }
                                            }
                                            else if (apiname.ToUpper() == "VASTWEB")
                                            {
                                                var tokn = db.vastbazzartokens.SingleOrDefault();
                                                VastBazaar1 cb1 = new VastBazaar1();
                                                var responseall = cb1.Bank_details("0581000100109278", tokn.apitoken);
                                                var responsechk = responseall.Content;
                                                dynamic json = JsonConvert.DeserializeObject(responsechk);
                                                var respcode = json.Content.ResponseCode.ToString();
                                                var ADDINFO = json.Content.ADDINFO;
                                                var ser = JsonConvert.SerializeObject(ADDINFO);
                                                json = JsonConvert.DeserializeObject(ser);

                                                JArray a = (JArray)json["data"];
                                                IList<BankNameModel> Details = a.ToObject<IList<BankNameModel>>();
                                                var entry = Details.Where(aaa => aaa.bank_name == bankname).FirstOrDefault();
                                                if (entry != null && entry.is_down == "1")
                                                {
                                                    var msg = "" + entry.bank_name + " is down, please try latter.";
                                                    var js = "  {'Details':'" + msg + "','status':'Failed'}";
                                                    var jss1 = new JavaScriptSerializer();
                                                    var dict1 = jss1.Deserialize<dynamic>(js);
                                                    return Json(dict1, JsonRequestBehavior.AllowGet);
                                                }
                                            }

                                            int remainder = Convert.ToInt32(transamount) % 5000;
                                            int div = Convert.ToInt32(transamount) / 5000;

                                            Response.status = "Success";
                                            Response.Details = "Success";
                                            Response.Accountno = bank_account;
                                            Response.Ifsccode = benIFSC;
                                            Response.BankName = bankname;
                                            Response.TotalAmount = finalamount;
                                            Response.Time = DateTime.Now;
                                            Response.data = new JArray() as dynamic;
                                            for (int i = 0; i < div; i++)
                                            {
                                                string Tranid = "W" + DateTime.Parse(DateTime.Now.ToString()).ToString("yyMMddHHmmss") + cb.RandomString(4);
                                                // decimal aam = 5000;
                                                if (apiname.ToUpper() == "CYBER")
                                                {
                                                    var ch = cb.fundtransfer(SenderNumber, benIFSC, benCode, "5000", transtype, userid, transamount, bank_account, bankname, CommonTranid, Tranid, kycsts, Ipaddress, macaddress);
                                                    if (ch == "RETAILERLOW")
                                                    {
                                                        results = "{'Details':'Remain Balance Low.','status':'Failed' }";
                                                        var jss1 = new JavaScriptSerializer();
                                                        var dict1 = jss1.Deserialize<dynamic>(results);
                                                        return Json(dict1, JsonRequestBehavior.AllowGet);
                                                    }
                                                    else if (ch == "DEALERLOW")
                                                    {
                                                        results = "{'Details':'Dealer Remain Balance Low.','status':'Failed' }";
                                                        var jss1 = new JavaScriptSerializer();
                                                        var dict1 = jss1.Deserialize<dynamic>(results);
                                                        return Json(dict1, JsonRequestBehavior.AllowGet);
                                                    }
                                                    else if (ch == "MASTERLOW")
                                                    {
                                                        results = "{'Details':'Admin Remain Balance Low.','status':'Failed' }";
                                                        var jss1 = new JavaScriptSerializer();
                                                        var dict1 = jss1.Deserialize<dynamic>(results);
                                                        return Json(dict1, JsonRequestBehavior.AllowGet);
                                                    }
                                                    else if (ch == "STATUSDOWN")
                                                    {
                                                        results = "{'Details':'Your DMT status inactive please contact to Admin','status':'Failed' }";
                                                        var jss1 = new JavaScriptSerializer();
                                                        var dict1 = jss1.Deserialize<dynamic>(results);
                                                        return Json(dict1, JsonRequestBehavior.AllowGet);
                                                    }
                                                    else if (ch == "CAPPINGLOW")
                                                    {
                                                        results = "{'Details':'Capping Low.','status':'Failed'}";
                                                        var jss1 = new JavaScriptSerializer();
                                                        var dict1 = jss1.Deserialize<dynamic>(results);
                                                        return Json(dict1, JsonRequestBehavior.AllowGet);
                                                    }
                                                    else
                                                    {
                                                        int start1 = ch.IndexOf("RESULT=") + 7;
                                                        int end1 = ch.IndexOf("\r\n", start1);
                                                        string result = ch.Substring(start1, end1 - start1);
                                                        /////////////ERROR////////////////////////////////////
                                                        start1 = ch.IndexOf("ERROR=") + 6;
                                                        end1 = ch.IndexOf("\r\n", start1);
                                                        string errornm = ch.Substring(start1, end1 - start1);
                                                        if (result == "0" && errornm == "0")
                                                        {
                                                            if (ch.Contains("ADDINFO="))
                                                            {
                                                                start1 = ch.IndexOf("TRNXSTATUS=") + 11;
                                                                end1 = ch.IndexOf("\r\n", start1);
                                                                string transts = ch.Substring(start1, end1 - start1);
                                                                if (transts == "7")
                                                                {
                                                                    int start = ch.IndexOf("ADDINFO=") + 8;
                                                                    int end = ch.IndexOf("AUTHCODE", start);
                                                                    string result1 = ch.Substring(start, end - start);
                                                                    result1 = HttpUtility.UrlDecode(result1);
                                                                    dynamic jsonObject = JsonConvert.DeserializeObject(result1);
                                                                    if (jsonObject.statuscode == "TXN")
                                                                    {
                                                                        var receiverName = jsonObject.data.benename.ToString();
                                                                        var bankrefno = jsonObject.data.ref_no.ToString();
                                                                        db.Money_transfer_update_new_new(Tranid, "SUCCESS", bankrefno, receiverName, ch, "", 0, 0);
                                                                        //if (StatusSendSmsMoneyTransferSuccess == "Y")
                                                                        //{
                                                                        //    smssend.sendsmsallwhitelabel(whitelabelid, RetailerMob, "Money Transfer in Bank " + bankname + " and Account Number " + bank_account + "and Bank Refernce Id " + bankrefno + " and Amount : 5000 is transfer Successfully.", "Recharge");
                                                                        //}
                                                                        smssend.sms_init_whitelabel(whitelabelid, StatusSendSmsMoneyTransferSuccess, "N", "MONEYTRANSFERMANUALMONEYSUCCESS", RetailerMob, bankname, bank_account + " ", bankrefno, "5000");

                                                                        dynamic resp = new JObject();
                                                                        resp.Amount = "5000";
                                                                        resp.Status = "Success";
                                                                        resp.bankrefid = bankrefno;
                                                                        Response.data.Add(resp);
                                                                    }
                                                                    else if (jsonObject.statuscode == "TUP")
                                                                    {
                                                                        dynamic resp = new JObject();
                                                                        resp.Amount = "5000";
                                                                        resp.Status = "Pending";
                                                                        resp.bankrefid = "";
                                                                        Response.data.Add(resp);
                                                                    }
                                                                    else
                                                                    {
                                                                        if (ch.ToUpper().Contains("ERRMSG"))
                                                                        {

                                                                            start = ch.IndexOf("ERRMSG=") + 7;
                                                                            end = ch.IndexOf("\r\n", start);
                                                                            string errormsg = ch.Substring(start, end - start);
                                                                            results = "{'Details':'" + errormsg + "','status':'failure' }";
                                                                            db.Money_transfer_update_new_new(Tranid, "FAILED", errormsg, "", ch, "", 0, 0);
                                                                            //if (StatusSendSmsMoneyTransferFailed == "Y")
                                                                            //{
                                                                            //    smssend.sendsmsallwhitelabel(whitelabelid, RetailerMob, "Money Transfer in Bank " + bankname + " and Account Number " + bank_account + "and Amount : 5000 is Failed Due To " + errormsg + ".", "Recharge");
                                                                            //}
                                                                            smssend.sms_init_whitelabel(whitelabelid, StatusSendSmsMoneyTransferFailed, "N", "MONEYTRANSFERMANUALMONEYFAILEDDUETO", RetailerMob, bankname, bank_account + " ", " 5000", errormsg);

                                                                            dynamic resp = new JObject();
                                                                            resp.Amount = "5000";
                                                                            resp.Status = "Failed";
                                                                            resp.bankrefid = errormsg;
                                                                            Response.data.Add(resp);
                                                                        }
                                                                        else
                                                                        {
                                                                            var error = db.Cyber_error_name_list.Where(aa => aa.error_code == errornm).SingleOrDefault().Error_name;
                                                                            results = "{'Details':'" + error + "','status':'failure'}";
                                                                            db.Money_transfer_update_new_new(Tranid, "FAILED", results, "", ch, "", 0, 0);
                                                                            //if (StatusSendSmsMoneyTransferFailed == "Y")
                                                                            //{
                                                                            //    smssend.sendsmsallwhitelabel(whitelabelid, RetailerMob, "Money Transfer in Bank " + bankname + " and Account Number " + bank_account + "and Amount : 5000 is Failed Due To " + results + ".", "Recharge");
                                                                            //}

                                                                            smssend.sms_init_whitelabel(whitelabelid, StatusSendSmsMoneyTransferFailed, "N", "MONEYTRANSFERMANUALMONEYFAILEDDUETO", RetailerMob, bankname, bank_account + " ", " 5000", results);

                                                                            dynamic resp = new JObject();
                                                                            resp.Amount = "5000";
                                                                            resp.Status = "Failed";
                                                                            resp.bankrefid = results;
                                                                            Response.data.Add(resp);
                                                                        }
                                                                    }

                                                                }
                                                                else if (transts == "3")
                                                                {

                                                                    dynamic resp = new JObject();
                                                                    resp.Amount = "5000";
                                                                    resp.Status = "Pending";
                                                                    resp.bankrefid = "";
                                                                    Response.data.Add(resp);
                                                                }
                                                            }
                                                            else
                                                            {

                                                                dynamic resp = new JObject();
                                                                resp.Amount = "5000";
                                                                resp.Status = "Pending";
                                                                resp.bankrefid = "";
                                                                Response.data.Add(resp);
                                                            }
                                                        }
                                                        else if (result == "1" && errornm == "36")
                                                        {

                                                            dynamic resp = new JObject();
                                                            resp.Amount = "5000";
                                                            resp.Status = "Pending";
                                                            resp.bankrefid = "";
                                                            Response.data.Add(resp);
                                                        }
                                                        else
                                                        {
                                                            if (ch.ToUpper().Contains("ERRMSG"))
                                                            {
                                                                int start = ch.IndexOf("ERRMSG=") + 7;
                                                                int end = ch.IndexOf("\r\n", start);
                                                                string errormsg = ch.Substring(start, end - start);
                                                                db.Money_transfer_update_new_new(Tranid, "FAILED", errormsg, "", ch, "", 0, 0);
                                                                //if (StatusSendSmsMoneyTransferFailed == "Y")
                                                                //{
                                                                //    smssend.sendsmsallwhitelabel(whitelabelid, RetailerMob, "Money Transfer in Bank " + bankname + " and Account Number " + bank_account + "and Amount : 5000 is Failed Due To " + errormsg + ".", "Recharge");
                                                                //}
                                                                smssend.sms_init_whitelabel(whitelabelid, StatusSendSmsMoneyTransferFailed, "N", "MONEYTRANSFERMANUALMONEYFAILEDDUETO", RetailerMob, bankname, bank_account + " ", " 5000", errormsg);

                                                                dynamic resp = new JObject();
                                                                resp.Amount = "5000";
                                                                resp.Status = "Failed";
                                                                resp.bankrefid = errormsg;
                                                                Response.data.Add(resp);
                                                            }
                                                            else
                                                            {
                                                                var error = db.Cyber_error_name_list.Where(aa => aa.error_code == errornm).SingleOrDefault().Error_name;
                                                                results = "{'Details':'" + error + "','status':'failure'}";
                                                                db.Money_transfer_update_new_new(Tranid, "FAILED", results, "", ch, "", 0, 0);
                                                                //if (StatusSendSmsMoneyTransferFailed == "Y")
                                                                //{
                                                                //    smssend.sendsmsallwhitelabel(whitelabelid, RetailerMob, "Money Transfer in Bank " + bankname + " and Account Number " + bank_account + "and Amount : 5000 is Failed Due To " + results + ".", "Recharge");
                                                                //}

                                                                smssend.sms_init_whitelabel(whitelabelid, StatusSendSmsMoneyTransferFailed, "N", "MONEYTRANSFERMANUALMONEYFAILEDDUETO", RetailerMob, bankname, bank_account + " ", " 5000", results);

                                                                dynamic resp = new JObject();
                                                                resp.Amount = "5000";
                                                                resp.Status = "Failed";
                                                                resp.bankrefid = results;
                                                                Response.data.Add(resp);
                                                            }
                                                        }
                                                    }
                                                }
                                                /////////////INSTANTPAY//////////////////
                                                else if (apiname.ToUpper() == "INSTANTPAY")
                                                {
                                                    var token = db.Money_API_URLS.Where(a => a.API_Name == apiname).Single().Token;
                                                    if (HttpContext.Request.IsLocal)
                                                    {
                                                        token = "5a7db562faadad009077b5c973901d7c";
                                                        //token = "ca23f55293bc9c2a51fe06d2d07e5ea4";
                                                    }

                                                    var data = new
                                                    {
                                                        token = token,
                                                        request = new
                                                        {
                                                            remittermobile = SenderNumber,
                                                            beneficiaryid = benCode,
                                                            agentid = Tranid,
                                                            amount = 5000,
                                                            mode = transtype
                                                        }
                                                    };

                                                    URL = "https://www.instantpay.in/ws/dmi/transfer";
                                                    jsonData = JsonConvert.SerializeObject(data);
                                                    requestsend = jsonData.ToString();
                                                    var ch = db.whitelabel_Money_transfer(userid, 5000, finalamount, SenderNumber, bank_account, bankname, benIFSC, CommonTranid, Tranid, "IMPS", "ONLINE", kycsts, requestsend, apiname, Ipaddress, macaddress, "", "DMT1", outputchk).Single().msg;
                                                    if (ch == "RETAILERLOW")
                                                    {
                                                        results = "{'Details':'Remain Balance Low.','status':'Failed' }";
                                                        var jss1 = new JavaScriptSerializer();
                                                        var dict1 = jss1.Deserialize<dynamic>(results);
                                                        return Json(dict1, JsonRequestBehavior.AllowGet);
                                                    }
                                                    else if (ch == "DEALERLOW")
                                                    {
                                                        results = "{'Details':'Dealer Remain Balance Low.','status':'Failed' }";
                                                        var jss1 = new JavaScriptSerializer();
                                                        var dict1 = jss1.Deserialize<dynamic>(results);
                                                        return Json(dict1, JsonRequestBehavior.AllowGet);
                                                    }
                                                    else if (ch == "MASTERLOW")
                                                    {
                                                        results = "{'Details':'Admin Remain Balance Low.','status':'Failed' }";
                                                        var jss1 = new JavaScriptSerializer();
                                                        var dict1 = jss1.Deserialize<dynamic>(results);
                                                        return Json(dict1, JsonRequestBehavior.AllowGet);
                                                    }
                                                    else if (ch == "STATUSDOWN")
                                                    {
                                                        results = "{'Details':'Your DMT status inactive please contact to Admin','status':'Failed' }";
                                                        var jss1 = new JavaScriptSerializer();
                                                        var dict1 = jss1.Deserialize<dynamic>(results);
                                                        return Json(dict1, JsonRequestBehavior.AllowGet);
                                                    }
                                                    else if (ch == "CAPPINGLOW")
                                                    {
                                                        results = "{'Details':'Capping Low.','status':'Failed'}";
                                                        var jss1 = new JavaScriptSerializer();
                                                        var dict1 = jss1.Deserialize<dynamic>(results);
                                                        return Json(dict1, JsonRequestBehavior.AllowGet);
                                                    }
                                                    else
                                                    {
                                                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
                                                        request.Method = "POST";
                                                        request.ContentType = "application/json";
                                                        StreamWriter requestWriter = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.ASCII);
                                                        requestWriter.Write(jsonData);
                                                        requestWriter.Close();

                                                        try
                                                        {
                                                            WebResponse webResponse = request.GetResponse();
                                                            Stream webStream = webResponse.GetResponseStream();
                                                            StreamReader responseReader = new StreamReader(webStream);
                                                            string response = responseReader.ReadToEnd();

                                                            /************** end ****************/
                                                            //Save response  to DB

                                                            response = response.Replace(@"<?xml version=""1.0"" encoding=""utf-8""?>", "");
                                                            XmlDocument doc = new XmlDocument();
                                                            doc.LoadXml(response);

                                                            string json = JsonConvert.SerializeXmlNode(doc);
                                                            JavaScriptSerializer ser = new JavaScriptSerializer();
                                                            var parsed = ser.Deserialize<dynamic>(json);
                                                            json = ser.Serialize(parsed["xml"]);
                                                            dynamic data1 = JObject.Parse(json);
                                                            var stscode = data1.statuscode.ToString();

                                                            if (stscode == "TXN")
                                                            {
                                                                var oprid = data1.data.ref_no.ToString();
                                                                var bname = data1.data.name.ToString();
                                                                string payidno = oprid;
                                                                decimal apiopeningbal = data1.data.opening_bal;
                                                                decimal chargeAmt = data1.data.charged_amt;
                                                                decimal apicloseingbal = (apiopeningbal - chargeAmt);
                                                                db.Money_transfer_update_new_new(Tranid, "SUCCESS", payidno, bname, ch, "", apiopeningbal, apicloseingbal);
                                                                //if (StatusSendSmsMoneyTransferSuccess == "Y")
                                                                //{
                                                                //    smssend.sendsmsallwhitelabel(whitelabelid, RetailerMob, "Money Transfer in Bank " + bankname + " and Account Number " + bank_account + "and Bank Refernce Id " + payidno + " and Amount : 5000 is transfer Successfully.", "Recharge");
                                                                //}

                                                                smssend.sms_init_whitelabel(whitelabelid, StatusSendSmsMoneyTransferSuccess, "N", "MONEYTRANSFERMANUALMONEYSUCCESS", RetailerMob, bankname, bank_account + " ", payidno, "5000");

                                                                dynamic resp = new JObject();
                                                                resp.Amount = "5000";
                                                                resp.Status = "Success";
                                                                resp.bankrefid = payidno;
                                                                Response.data.Add(resp);
                                                            }
                                                            else if (stscode == "TUP")
                                                            {

                                                                dynamic resp = new JObject();
                                                                resp.Amount = "5000";
                                                                resp.Status = "Pending";
                                                                resp.bankrefid = "";
                                                                Response.data.Add(resp);
                                                            }
                                                            else
                                                            {
                                                                var bname = ""; string payidno = "";
                                                                try
                                                                {
                                                                    //oprid = data1.xml.data.opr_id.ToString();
                                                                    //bname = data1.xml.data.name.ToString();
                                                                    payidno = data1.status.ToString();
                                                                }
                                                                catch { }
                                                                db.Money_transfer_update_new_new(Tranid, "FAILED", payidno, bname, ch, "", 0, 0);
                                                                //if (StatusSendSmsMoneyTransferFailed == "Y")
                                                                //{
                                                                //    smssend.sendsmsallwhitelabel(whitelabelid, RetailerMob, "Money Transfer in Bank " + bankname + " and Account Number " + bank_account + "and Amount : 5000 is Failed Due To " + payidno + ".", "Recharge");
                                                                //}

                                                                smssend.sms_init_whitelabel(whitelabelid, StatusSendSmsMoneyTransferFailed, "N", "MONEYTRANSFERMANUALMONEYFAILEDDUETO", RetailerMob, bankname, bank_account + " ", " 5000", payidno);

                                                                dynamic resp = new JObject();
                                                                resp.Amount = "5000";
                                                                resp.Status = "Failed";
                                                                resp.bankrefid = payidno;
                                                                Response.data.Add(resp);
                                                            }

                                                        }
                                                        catch
                                                        {
                                                            //var msg = ex.Message.ToString();
                                                            //results = "{'message':'" + msg + "','status':'failure'}";
                                                            //var jss = new JavaScriptSerializer();
                                                            //var dict = jss.Deserialize<dynamic>(results);
                                                            //return Json(dict, JsonRequestBehavior.AllowGet);
                                                        }

                                                    }
                                                }
                                                ///////////////VastWeb////////////////////
                                                else if (apiname.ToUpper() == "VASTWEB")
                                                {
                                                    var tokn = db.vastbazzartokens.SingleOrDefault();
                                                    requestsend = "{\"remittermobile\":\"" + SenderNumber + "\",\"account\":\"" + bank_account + "\",\"ifsc\":\"" + benIFSC + "\",\"agentid\":\"" + Tranid + "";
                                                    var ch = db.whitelabel_Money_transfer(userid, 5000, finalamount, SenderNumber, bank_account, bankname, benIFSC, CommonTranid, Tranid, transtype, "ONLINE", kycsts, requestsend, apiname, Ipaddress, macaddress, "", "DMT1", outputchk).Single().msg;
                                                    if (ch == "RETAILERLOW")
                                                    {
                                                        results = "{'Details':'Remain Balance Low.','status':'Failed' }";
                                                        var jss1 = new JavaScriptSerializer();
                                                        var dict1 = jss1.Deserialize<dynamic>(results);
                                                        return Json(dict1, JsonRequestBehavior.AllowGet);
                                                    }
                                                    else if (ch == "DEALERLOW")
                                                    {
                                                        results = "{'Details':'Dealer Remain Balance Low.','status':'Failed' }";
                                                        var jss1 = new JavaScriptSerializer();
                                                        var dict1 = jss1.Deserialize<dynamic>(results);
                                                        return Json(dict1, JsonRequestBehavior.AllowGet);
                                                    }
                                                    else if (ch == "MASTERLOW")
                                                    {
                                                        results = "{'Details':'Admin Remain Balance Low.','status':'Failed' }";
                                                        var jss1 = new JavaScriptSerializer();
                                                        var dict1 = jss1.Deserialize<dynamic>(results);
                                                        return Json(dict1, JsonRequestBehavior.AllowGet);
                                                    }
                                                    else if (ch == "STATUSDOWN")
                                                    {
                                                        results = "{'Details':'Your DMT status inactive please contact to Admin','status':'Failed' }";
                                                        var jss1 = new JavaScriptSerializer();
                                                        var dict1 = jss1.Deserialize<dynamic>(results);
                                                        return Json(dict1, JsonRequestBehavior.AllowGet);
                                                    }
                                                    else if (ch == "CAPPINGLOW")
                                                    {
                                                        results = "{'Details':'Capping Low.','status':'Failed'}";
                                                        var jss1 = new JavaScriptSerializer();
                                                        var dict1 = jss1.Deserialize<dynamic>(results);
                                                        return Json(dict1, JsonRequestBehavior.AllowGet);
                                                    }
                                                    else
                                                    {
                                                        System.Threading.Thread.Sleep(1000);
                                                        VastBazaar1 cb1 = new VastBazaar1();

                                                        DateTime curntdate = DateTime.Now.Date;
                                                        DateTime expdate = Convert.ToDateTime(tokn.exptime).Date;
                                                        var responsechk = "";
                                                        var responsecode1 = "";
                                                        if (expdate > curntdate)
                                                        {
                                                            var responseall = cb1.Fund_Transfer(SenderNumber, benCode, Tranid, "5000", transtype, bank_account, benIFSC, tokn.apitoken, bankname);
                                                            responsechk = responseall.Content.ToString();
                                                            responsecode1 = responseall.StatusCode.ToString();
                                                        }
                                                        else
                                                        {
                                                            var response = tokencheck();
                                                            var response1 = response.Content.ToString();
                                                            var responsecode = response.StatusCode.ToString();
                                                            if (responsecode == "OK")
                                                            {
                                                                dynamic json = JsonConvert.DeserializeObject(responsechk);
                                                                var token = json.access_token.ToString();
                                                                var expire = json[".expires"].ToString();
                                                                DateTime exp = Convert.ToDateTime(expire);
                                                                tokn.apitoken = token;
                                                                tokn.exptime = exp;
                                                                db.SaveChanges();
                                                                var responseall = cb1.Fund_Transfer(SenderNumber, benCode, Tranid, "5000", transtype, bank_account, benIFSC, token, bankname);
                                                                responsechk = responseall.Content.ToString();
                                                                responsecode1 = responseall.StatusCode.ToString();
                                                            }
                                                            else
                                                            {
                                                                dynamic json = JsonConvert.DeserializeObject(responsechk);
                                                                var error = json.error.ToString();
                                                                var error_decribe = json["error_description"].ToString();
                                                                results = "{'Details':'" + error_decribe + "','status':'failure'}";
                                                                var jss = new JavaScriptSerializer();
                                                                var dict = jss.Deserialize<dynamic>(results);

                                                                return Json(dict, JsonRequestBehavior.AllowGet);
                                                            }
                                                        }

                                                        if (responsecode1 == "OK")
                                                        {
                                                            dynamic json = JsonConvert.DeserializeObject(responsechk);
                                                            var respcode = json.Content.ResponseCode.ToString();
                                                            var ADDINFO = json.Content.ADDINFO;
                                                            var stscode = ADDINFO.statuscode;

                                                            if (stscode == "TXN")
                                                            {
                                                                var oprid = ADDINFO.data.ref_no.ToString();
                                                                var bname = ADDINFO.data.name.ToString();
                                                                string payidno = oprid;
                                                                decimal apiopeningbal = ADDINFO.data.opening_bal;
                                                                decimal chargeAmt = ADDINFO.data.charged_amt;
                                                                decimal apicloseingbal = (apiopeningbal - chargeAmt);
                                                                db.Money_transfer_update_new_new(Tranid, "SUCCESS", payidno, bname, ch, "", apiopeningbal, apicloseingbal);
                                                                //if (StatusSendSmsMoneyTransferSuccess == "Y")
                                                                //{
                                                                //    smssend.sendsmsallwhitelabel(whitelabelid, RetailerMob, "Money Transfer in Bank " + bankname + " and Account Number " + bank_account + "and Bank Refernce Id " + payidno + " and Amount : 5000 is transfer Successfully.", "Recharge");
                                                                //}

                                                                smssend.sms_init_whitelabel(whitelabelid, StatusSendSmsMoneyTransferSuccess, "N", "MONEYTRANSFERMANUALMONEYSUCCESS", RetailerMob, bankname, bank_account + " ", payidno, "5000");

                                                                dynamic resp = new JObject();
                                                                resp.Amount = "5000";
                                                                resp.Status = "Success";
                                                                resp.bankrefid = payidno;
                                                                Response.data.Add(resp);
                                                            }
                                                            else if (stscode == "TUP")
                                                            {
                                                                dynamic resp = new JObject();
                                                                resp.Amount = "5000";
                                                                resp.Status = "Pending";
                                                                resp.bankrefid = "";
                                                                Response.data.Add(resp);
                                                            }
                                                            else
                                                            {
                                                                var bname = ""; string payidno = "";
                                                                try
                                                                {
                                                                    //oprid = data1.xml.data.opr_id.ToString();
                                                                    //bname = data1.xml.data.name.ToString();
                                                                    payidno = ADDINFO.status.ToString();
                                                                }
                                                                catch { }
                                                                db.Money_transfer_update_new_new(Tranid, "FAILED", payidno, bname, ch, "", 0, 0);
                                                                //if (StatusSendSmsMoneyTransferFailed == "Y")
                                                                //{
                                                                //    smssend.sendsmsallwhitelabel(whitelabelid, RetailerMob, "Money Transfer in Bank " + bankname + " and Account Number " + bank_account + "and Amount : 5000 is Failed Due To " + payidno + ".", "Recharge");
                                                                //}

                                                                smssend.sms_init_whitelabel(whitelabelid, StatusSendSmsMoneyTransferFailed, "N", "MONEYTRANSFERMANUALMONEYFAILEDDUETO", RetailerMob, bankname, bank_account + " ", " 5000", payidno);

                                                                dynamic resp = new JObject();
                                                                resp.Amount = "5000";
                                                                resp.Status = "Failed";
                                                                resp.bankrefid = payidno;
                                                                Response.data.Add(resp);
                                                            }

                                                        }
                                                        else
                                                        {
                                                            dynamic json = JsonConvert.DeserializeObject(responsechk);
                                                            var error = json.error.ToString();
                                                            var error_decribe = json["error_description"].ToString();
                                                            var results1 = "{'Details':'" + error_decribe + "','status':'failure'}";
                                                            var jss = new JavaScriptSerializer();
                                                            var dict = jss.Deserialize<dynamic>(results1);

                                                            return Json(dict, JsonRequestBehavior.AllowGet);
                                                        }
                                                    }
                                                }
                                            }
                                            if (remainder > 0)
                                            {
                                                string Tranid = "W" + DateTime.Parse(DateTime.Now.ToString()).ToString("yyMMddHHmmss") + cb.RandomString(4);
                                                if (apiname.ToUpper() == "CYBER")
                                                {
                                                    var ch = cb.fundtransfer(SenderNumber, benIFSC, benCode, remainder.ToString(), transtype, userid, transamount, bank_account, bankname, CommonTranid, Tranid, kycsts, Ipaddress, macaddress);
                                                    if (ch == "RETAILERLOW")
                                                    {
                                                        results = "{'Details':'Remain Balance Low.','status':'Failed' }";
                                                        var jss1 = new JavaScriptSerializer();
                                                        var dict1 = jss1.Deserialize<dynamic>(results);
                                                        return Json(dict1, JsonRequestBehavior.AllowGet);
                                                    }
                                                    else if (ch == "DEALERLOW")
                                                    {
                                                        results = "{'Details':'Dealer Remain Balance Low.','status':'Failed' }";
                                                        var jss1 = new JavaScriptSerializer();
                                                        var dict1 = jss1.Deserialize<dynamic>(results);
                                                        return Json(dict1, JsonRequestBehavior.AllowGet);
                                                    }
                                                    else if (ch == "MASTERLOW")
                                                    {
                                                        results = "{'Details':'Admin Remain Balance Low.','status':'Failed' }";
                                                        var jss1 = new JavaScriptSerializer();
                                                        var dict1 = jss1.Deserialize<dynamic>(results);
                                                        return Json(dict1, JsonRequestBehavior.AllowGet);
                                                    }
                                                    else if (ch == "STATUSDOWN")
                                                    {
                                                        results = "{'Details':'Your DMT status inactive please contact to Admin','status':'Failed' }";
                                                        var jss1 = new JavaScriptSerializer();
                                                        var dict1 = jss1.Deserialize<dynamic>(results);
                                                        return Json(dict1, JsonRequestBehavior.AllowGet);
                                                    }
                                                    else if (ch == "CAPPINGLOW")
                                                    {
                                                        results = "{'Details':'Capping Low.','status':'Failed'}";
                                                        var jss1 = new JavaScriptSerializer();
                                                        var dict1 = jss1.Deserialize<dynamic>(results);
                                                        return Json(dict1, JsonRequestBehavior.AllowGet);
                                                    }
                                                    else
                                                    {
                                                        int start1 = ch.IndexOf("RESULT=") + 7;
                                                        int end1 = ch.IndexOf("\r\n", start1);
                                                        string result = ch.Substring(start1, end1 - start1);
                                                        /////////////ERROR////////////////////////////////////
                                                        start1 = ch.IndexOf("ERROR=") + 6;
                                                        end1 = ch.IndexOf("\r\n", start1);
                                                        string errornm = ch.Substring(start1, end1 - start1);
                                                        if (result == "0" && errornm == "0")
                                                        {
                                                            if (ch.Contains("ADDINFO="))
                                                            {
                                                                start1 = ch.IndexOf("TRNXSTATUS=") + 11;
                                                                end1 = ch.IndexOf("\r\n", start1);
                                                                string transts = ch.Substring(start1, end1 - start1);
                                                                if (transts == "7")
                                                                {
                                                                    int start = ch.IndexOf("ADDINFO=") + 8;
                                                                    int end = ch.IndexOf("AUTHCODE", start);
                                                                    string result1 = ch.Substring(start, end - start);
                                                                    result1 = HttpUtility.UrlDecode(result1);
                                                                    dynamic jsonObject = JsonConvert.DeserializeObject(result1);
                                                                    if (jsonObject.statuscode == "TXN")
                                                                    {
                                                                        var receiverName = jsonObject.data.benename.ToString();
                                                                        var bankrefno = jsonObject.data.ref_no.ToString();
                                                                        db.Money_transfer_update_new_new(Tranid, "SUCCESS", bankrefno, receiverName, ch, "", 0, 0);
                                                                        //if (StatusSendSmsMoneyTransferSuccess == "Y")
                                                                        //{
                                                                        //    smssend.sendsmsallwhitelabel(whitelabelid, RetailerMob, "Money Transfer in Bank " + bankname + " and Account Number " + bank_account + "and Bank Refernce Id " + bankrefno + " and Amount " + transamount + " is transfer Successfully.", "Recharge");
                                                                        //}

                                                                        smssend.sms_init_whitelabel(whitelabelid, StatusSendSmsMoneyTransferSuccess, "N", "MONEYTRANSFERMANUALMONEYSUCCESS", RetailerMob, bankname, bank_account + " ", bankrefno, transamount);

                                                                        dynamic resp = new JObject();
                                                                        resp.Amount = remainder;
                                                                        resp.Status = "Success";
                                                                        resp.bankrefid = bankrefno;
                                                                        Response.data.Add(resp);
                                                                    }
                                                                    else if (jsonObject.statuscode == "TUP")
                                                                    {

                                                                        dynamic resp = new JObject();
                                                                        resp.Amount = remainder;
                                                                        resp.Status = "Pending";
                                                                        resp.bankrefid = "";
                                                                        Response.data.Add(resp);
                                                                    }
                                                                    else
                                                                    {
                                                                        if (ch.ToUpper().Contains("ERRMSG"))
                                                                        {

                                                                            start = ch.IndexOf("ERRMSG=") + 7;
                                                                            end = ch.IndexOf("\r\n", start);
                                                                            string errormsg = ch.Substring(start, end - start);

                                                                            db.Money_transfer_update_new_new(Tranid, "FAILED", errormsg, "", ch, "", 0, 0);
                                                                            //if (StatusSendSmsMoneyTransferFailed == "Y")
                                                                            //{
                                                                            //    smssend.sendsmsallwhitelabel(whitelabelid, RetailerMob, "Money Transfer in Bank " + bankname + " and Account Number " + bank_account + "and Amount" + transamount + " is Failed Due To " + errormsg + ".", "Recharge");
                                                                            //}

                                                                            smssend.sms_init_whitelabel(whitelabelid, StatusSendSmsMoneyTransferFailed, "N", "MONEYTRANSFERMANUALMONEYFAILEDDUETO", RetailerMob, bankname, bank_account + " ", " " + transamount, errormsg);

                                                                            dynamic resp = new JObject();
                                                                            resp.Amount = remainder;
                                                                            resp.Status = "Failed";
                                                                            resp.bankrefid = errormsg;
                                                                            Response.data.Add(resp);
                                                                        }
                                                                        else
                                                                        {
                                                                            var error = db.Cyber_error_name_list.Where(aa => aa.error_code == errornm).SingleOrDefault().Error_name;

                                                                            db.Money_transfer_update_new_new(Tranid, "FAILED", results, "", ch, "", 0, 0);
                                                                            //if (StatusSendSmsMoneyTransferFailed == "Y")
                                                                            //{
                                                                            //    smssend.sendsmsallwhitelabel(whitelabelid, RetailerMob, "Money Transfer in Bank " + bankname + " and Account Number " + bank_account + "and Amount" + transamount + " is Failed Due To " + results + ".", "Recharge");
                                                                            //}

                                                                            smssend.sms_init_whitelabel(whitelabelid, StatusSendSmsMoneyTransferFailed, "N", "MONEYTRANSFERMANUALMONEYFAILEDDUETO", RetailerMob, bankname, bank_account + " ", " " + transamount, results);

                                                                            dynamic resp = new JObject();
                                                                            resp.Amount = remainder;
                                                                            resp.Status = "Failed";
                                                                            resp.bankrefid = results;
                                                                            Response.data.Add(resp);
                                                                        }
                                                                    }

                                                                }
                                                                else if (transts == "3")
                                                                {

                                                                    dynamic resp = new JObject();
                                                                    resp.Amount = remainder;
                                                                    resp.Status = "Pending";
                                                                    resp.bankrefid = "";
                                                                    Response.data.Add(resp);
                                                                }
                                                            }
                                                            else
                                                            {

                                                                dynamic resp = new JObject();
                                                                resp.Amount = remainder;
                                                                resp.Status = "Pending";
                                                                resp.bankrefid = "";
                                                                Response.data.Add(resp);
                                                            }
                                                        }
                                                        else if (result == "1" && errornm == "36")
                                                        {

                                                            dynamic resp = new JObject();
                                                            resp.Amount = remainder;
                                                            resp.Status = "Pending";
                                                            resp.bankrefid = "";
                                                            Response.data.Add(resp);
                                                        }
                                                        else
                                                        {
                                                            if (ch.ToUpper().Contains("ERRMSG"))
                                                            {
                                                                int start = ch.IndexOf("ERRMSG=") + 7;
                                                                int end = ch.IndexOf("\r\n", start);
                                                                string errormsg = ch.Substring(start, end - start);
                                                                db.Money_transfer_update_new_new(Tranid, "FAILED", errormsg, "", ch, "", 0, 0);
                                                                //if (StatusSendSmsMoneyTransferFailed == "Y")
                                                                //{
                                                                //    smssend.sendsmsallwhitelabel(whitelabelid, RetailerMob, "Money Transfer in Bank " + bankname + " and Account Number " + bank_account + "and Amount" + transamount + " is Failed Due To " + errormsg + ".", "Recharge");
                                                                //}

                                                                smssend.sms_init_whitelabel(whitelabelid, StatusSendSmsMoneyTransferFailed, "N", "MONEYTRANSFERMANUALMONEYFAILEDDUETO", RetailerMob, bankname, bank_account + " ", " " + transamount, errormsg);

                                                                dynamic resp = new JObject();
                                                                resp.Amount = remainder.ToString();
                                                                resp.Status = "Failed";
                                                                resp.bankrefid = errormsg;
                                                                Response.data.Add(resp);
                                                            }
                                                            else
                                                            {
                                                                var error = db.Cyber_error_name_list.Where(aa => aa.error_code == errornm).SingleOrDefault().Error_name;

                                                                db.Money_transfer_update_new_new(Tranid, "FAILED", results, "", ch, "", 0, 0);
                                                                //if (StatusSendSmsMoneyTransferFailed == "Y")
                                                                //{
                                                                //    smssend.sendsmsallwhitelabel(whitelabelid, RetailerMob, "Money Transfer in Bank " + bankname + " and Account Number " + bank_account + "and Amount" + transamount + " is Failed Due To " + results + ".", "Recharge");
                                                                //}

                                                                smssend.sms_init_whitelabel(whitelabelid, StatusSendSmsMoneyTransferFailed, "N", "MONEYTRANSFERMANUALMONEYFAILEDDUETO", RetailerMob, bankname, bank_account + " ", " " + transamount, results);

                                                                dynamic resp = new JObject();
                                                                resp.Amount = remainder;
                                                                resp.Status = "Failed";
                                                                resp.bankrefid = results;
                                                                Response.data.Add(resp);
                                                            }
                                                        }
                                                    }

                                                }
                                                /////////////INSTANTPAY//////////////////
                                                else if (apiname.ToUpper() == "INSTANTPAY")
                                                {
                                                    var token = db.Money_API_URLS.Where(a => a.API_Name == apiname).Single().Token;
                                                    if (HttpContext.Request.IsLocal)
                                                    {
                                                        token = "5a7db562faadad009077b5c973901d7c";
                                                        //token = "ca23f55293bc9c2a51fe06d2d07e5ea4";
                                                    }
                                                    var data = new
                                                    {
                                                        token = token,
                                                        request = new
                                                        {
                                                            remittermobile = SenderNumber,
                                                            beneficiaryid = benCode,
                                                            agentid = Tranid,
                                                            amount = remainder,
                                                            mode = transtype
                                                        }
                                                    };

                                                    URL = "https://www.instantpay.in/ws/dmi/transfer";
                                                    jsonData = JsonConvert.SerializeObject(data);
                                                    requestsend = jsonData.ToString();
                                                    var ch = db.whitelabel_Money_transfer(userid, remainder, finalamount, SenderNumber, bank_account, bankname, benIFSC, CommonTranid, Tranid, "IMPS", "ONLINE", kycsts, requestsend, apiname, Ipaddress, macaddress, "", "DMT1", outputchk).Single().msg;
                                                    if (ch == "RETAILERLOW")
                                                    {
                                                        results = "{'Details':'Remain Balance Low.','status':'Failed' }";
                                                        var jss1 = new JavaScriptSerializer();
                                                        var dict1 = jss1.Deserialize<dynamic>(results);
                                                        return Json(dict1, JsonRequestBehavior.AllowGet);
                                                    }
                                                    else if (ch == "DEALERLOW")
                                                    {
                                                        results = "{'Details':'Dealer Remain Balance Low.','status':'Failed' }";
                                                        var jss1 = new JavaScriptSerializer();
                                                        var dict1 = jss1.Deserialize<dynamic>(results);
                                                        return Json(dict1, JsonRequestBehavior.AllowGet);
                                                    }
                                                    else if (ch == "MASTERLOW")
                                                    {
                                                        results = "{'Details':'Admin Remain Balance Low.','status':'Failed' }";
                                                        var jss1 = new JavaScriptSerializer();
                                                        var dict1 = jss1.Deserialize<dynamic>(results);
                                                        return Json(dict1, JsonRequestBehavior.AllowGet);
                                                    }
                                                    else if (ch == "STATUSDOWN")
                                                    {
                                                        results = "{'Details':'Your DMT status inactive please contact to Admin','status':'Failed' }";
                                                        var jss1 = new JavaScriptSerializer();
                                                        var dict1 = jss1.Deserialize<dynamic>(results);
                                                        return Json(dict1, JsonRequestBehavior.AllowGet);
                                                    }
                                                    else if (ch == "CAPPINGLOW")
                                                    {
                                                        results = "{'Details':'Capping Low.','status':'Failed'}";
                                                        var jss1 = new JavaScriptSerializer();
                                                        var dict1 = jss1.Deserialize<dynamic>(results);
                                                        return Json(dict1, JsonRequestBehavior.AllowGet);
                                                    }
                                                    else
                                                    {
                                                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
                                                        request.Method = "POST";
                                                        request.ContentType = "application/json";
                                                        StreamWriter requestWriter = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.ASCII);
                                                        requestWriter.Write(jsonData);
                                                        requestWriter.Close();

                                                        try
                                                        {
                                                            WebResponse webResponse = request.GetResponse();
                                                            Stream webStream = webResponse.GetResponseStream();
                                                            StreamReader responseReader = new StreamReader(webStream);
                                                            string response = responseReader.ReadToEnd();

                                                            /************** end ****************/
                                                            //Save response  to DB

                                                            response = response.Replace(@"<?xml version=""1.0"" encoding=""utf-8""?>", "");
                                                            XmlDocument doc = new XmlDocument();
                                                            doc.LoadXml(response);

                                                            string json = JsonConvert.SerializeXmlNode(doc);
                                                            JavaScriptSerializer ser = new JavaScriptSerializer();
                                                            var parsed = ser.Deserialize<dynamic>(json);
                                                            json = ser.Serialize(parsed["xml"]);
                                                            dynamic data1 = JObject.Parse(json);
                                                            var stscode = data1.statuscode.ToString();

                                                            if (stscode == "TXN")
                                                            {
                                                                var oprid = data1.data.ref_no.ToString();
                                                                var bname = data1.data.name.ToString();
                                                                string payidno = oprid;
                                                                decimal apiopeningbal = data1.data.opening_bal;
                                                                decimal chargeAmt = data1.data.charged_amt;
                                                                decimal apicloseingbal = (apiopeningbal - chargeAmt);
                                                                db.Money_transfer_update_new_new(Tranid, "SUCCESS", payidno, bname, ch, "", apiopeningbal, apicloseingbal);
                                                                //if (StatusSendSmsMoneyTransferSuccess == "Y")
                                                                //{
                                                                //    smssend.sendsmsallwhitelabel(whitelabelid, RetailerMob, "Money Transfer in Bank " + bankname + " and Account Number " + bank_account + "and Bank Refernce Id " + payidno + " and Amount " + transamount + " is transfer Successfully.", "Recharge");
                                                                //}

                                                                smssend.sms_init_whitelabel(whitelabelid, StatusSendSmsMoneyTransferSuccess, "N", "MONEYTRANSFERMANUALMONEYSUCCESS", RetailerMob, bankname, bank_account + " ", payidno, transamount);

                                                                dynamic resp = new JObject();
                                                                resp.Amount = remainder;
                                                                resp.Status = "Success";
                                                                resp.bankrefid = payidno;
                                                                Response.data.Add(resp);
                                                            }
                                                            else if (stscode == "TUP")
                                                            {

                                                                dynamic resp = new JObject();
                                                                resp.Amount = remainder;
                                                                resp.Status = "Pending";
                                                                resp.bankrefid = "";
                                                                Response.data.Add(resp);
                                                            }
                                                            else
                                                            {
                                                                var bname = ""; string payidno = "";
                                                                try
                                                                {
                                                                    //oprid = data1.xml.data.opr_id.ToString();
                                                                    //bname = data1.xml.data.name.ToString();
                                                                    payidno = data1.status.ToString();
                                                                }
                                                                catch { }
                                                                db.Money_transfer_update_new_new(Tranid, "FAILED", payidno, bname, ch, "", 0, 0);
                                                                //if (StatusSendSmsMoneyTransferFailed == "Y")
                                                                //{
                                                                //    smssend.sendsmsallwhitelabel(whitelabelid, RetailerMob, "Money Transfer in Bank " + bankname + " and Account Number " + bank_account + "and Amount" + transamount + " is Failed Due To " + payidno + ".", "Recharge");
                                                                //}

                                                                smssend.sms_init_whitelabel(whitelabelid, StatusSendSmsMoneyTransferFailed, "N", "MONEYTRANSFERMANUALMONEYFAILEDDUETO", RetailerMob, bankname, bank_account + " ", " " + transamount, payidno);

                                                                dynamic resp = new JObject();
                                                                resp.Amount = remainder;
                                                                resp.Status = "Failed";
                                                                resp.bankrefid = payidno;
                                                                Response.data.Add(resp);
                                                            }

                                                        }
                                                        catch
                                                        {
                                                            //var msg = ex.Message.ToString();
                                                            //results = "{'message':'" + msg + "','status':'failure'}";
                                                            //var jss = new JavaScriptSerializer();
                                                            //var dict = jss.Deserialize<dynamic>(results);
                                                            //return Json(dict, JsonRequestBehavior.AllowGet);
                                                        }
                                                    }
                                                }
                                                ////////////VastWeb/////////////////
                                                else if (apiname.ToUpper() == "VASTWEB")
                                                {
                                                    var tokn = db.vastbazzartokens.SingleOrDefault();
                                                    VastBazaar1 cb1 = new VastBazaar1();
                                                    requestsend = "{\"remittermobile\":\"" + SenderNumber + "\",\"account\":\"" + bank_account + "\",\"ifsc\":\"" + benIFSC + "\",\"agentid\":\"" + CommonTranid + "";
                                                    var ch = db.whitelabel_Money_transfer(userid, remainder, finalamount, SenderNumber, bank_account, bankname, benIFSC, CommonTranid, Tranid, "IMPS", "ONLINE", kycsts, requestsend, apiname, Ipaddress, macaddress, "", "DMT1", outputchk).Single().msg;
                                                    if (ch == "RETAILERLOW")
                                                    {
                                                        results = "{'Details':'Remain Balance Low.','status':'Failed' }";
                                                        var jss1 = new JavaScriptSerializer();
                                                        var dict1 = jss1.Deserialize<dynamic>(results);
                                                        return Json(dict1, JsonRequestBehavior.AllowGet);
                                                    }
                                                    else if (ch == "DEALERLOW")
                                                    {
                                                        results = "{'Details':'Dealer Remain Balance Low.','status':'Failed' }";
                                                        var jss1 = new JavaScriptSerializer();
                                                        var dict1 = jss1.Deserialize<dynamic>(results);
                                                        return Json(dict1, JsonRequestBehavior.AllowGet);
                                                    }
                                                    else if (ch == "MASTERLOW")
                                                    {
                                                        results = "{'Details':'Admin Remain Balance Low.','status':'Failed' }";
                                                        var jss1 = new JavaScriptSerializer();
                                                        var dict1 = jss1.Deserialize<dynamic>(results);
                                                        return Json(dict1, JsonRequestBehavior.AllowGet);
                                                    }
                                                    else if (ch == "STATUSDOWN")
                                                    {
                                                        results = "{'Details':'Your DMT status inactive please contact to Admin','status':'Failed' }";
                                                        var jss1 = new JavaScriptSerializer();
                                                        var dict1 = jss1.Deserialize<dynamic>(results);
                                                        return Json(dict1, JsonRequestBehavior.AllowGet);
                                                    }
                                                    else if (ch == "CAPPINGLOW")
                                                    {
                                                        results = "{'Details':'Capping Low.','status':'Failed'}";
                                                        var jss1 = new JavaScriptSerializer();
                                                        var dict1 = jss1.Deserialize<dynamic>(results);
                                                        return Json(dict1, JsonRequestBehavior.AllowGet);
                                                    }
                                                    else
                                                    {
                                                        System.Threading.Thread.Sleep(1000);
                                                        DateTime curntdate = DateTime.Now.Date;
                                                        DateTime expdate = Convert.ToDateTime(tokn.exptime).Date;
                                                        var responsechk = "";
                                                        var responsecode1 = "";
                                                        if (expdate > curntdate)
                                                        {
                                                            var responseall = cb1.Fund_Transfer(SenderNumber, benCode, Tranid, remainder.ToString(), transtype, bank_account, benIFSC, tokn.apitoken, bankname);
                                                            responsechk = responseall.Content.ToString();
                                                            responsecode1 = responseall.StatusCode.ToString();
                                                        }
                                                        else
                                                        {
                                                            var response = tokencheck();
                                                            var response1 = response.Content.ToString();
                                                            var responsecode = response.StatusCode.ToString();
                                                            if (responsecode == "OK")
                                                            {
                                                                dynamic json = JsonConvert.DeserializeObject(responsechk);
                                                                var token = json.access_token.ToString();
                                                                var expire = json[".expires"].ToString();
                                                                DateTime exp = Convert.ToDateTime(expire);

                                                                tokn.apitoken = token;
                                                                tokn.exptime = exp;
                                                                db.SaveChanges();
                                                                var responseall = cb1.Fund_Transfer(SenderNumber, benCode, Tranid, remainder.ToString(), transtype, bank_account, benIFSC, tokn.apitoken, bankname);
                                                                responsechk = responseall.Content.ToString();
                                                                responsecode1 = responseall.StatusCode.ToString();
                                                            }
                                                            else
                                                            {
                                                                dynamic json = JsonConvert.DeserializeObject(responsechk);
                                                                var error = json.error.ToString();
                                                                var error_decribe = json["error_description"].ToString();
                                                                results = "{'Details':'" + error_decribe + "','status':'failure'}";
                                                                var jss = new JavaScriptSerializer();
                                                                var dict = jss.Deserialize<dynamic>(results);

                                                                return Json(dict, JsonRequestBehavior.AllowGet);
                                                            }
                                                        }

                                                        if (responsecode1 == "OK")
                                                        {
                                                            dynamic json = JsonConvert.DeserializeObject(responsechk);
                                                            var respcode = json.Content.ResponseCode.ToString();
                                                            var ADDINFO = json.Content.ADDINFO;
                                                            var stscode = ADDINFO.statuscode;
                                                            if (stscode == "TXN")
                                                            {
                                                                var oprid = ADDINFO.data.ref_no.ToString();
                                                                var bname = ADDINFO.data.name.ToString();
                                                                string payidno = oprid;
                                                                decimal apiopeningbal = ADDINFO.data.opening_bal;
                                                                decimal chargeAmt = ADDINFO.data.charged_amt;
                                                                decimal apicloseingbal = (apiopeningbal - chargeAmt);
                                                                db.Money_transfer_update_new_new(Tranid, "SUCCESS", payidno, bname, ch, "", apiopeningbal, apicloseingbal);
                                                                //if (StatusSendSmsMoneyTransferSuccess == "Y")
                                                                //{
                                                                //    smssend.sendsmsallwhitelabel(whitelabelid, RetailerMob, "Money Transfer in Bank " + bankname + " and Account Number " + bank_account + "and Bank Refernce Id " + payidno + " and Amount " + transamount + " is transfer Successfully.", "Recharge");
                                                                //}

                                                                smssend.sms_init_whitelabel(whitelabelid, StatusSendSmsMoneyTransferSuccess, "N", "MONEYTRANSFERMANUALMONEYSUCCESS", RetailerMob, bankname, bank_account + " ", payidno, transamount);

                                                                dynamic resp = new JObject();
                                                                resp.Amount = remainder;
                                                                resp.Status = "Success";
                                                                resp.bankrefid = payidno;
                                                                Response.data.Add(resp);
                                                            }
                                                            else if (stscode == "TUP")
                                                            {

                                                                dynamic resp = new JObject();
                                                                resp.Amount = remainder;
                                                                resp.Status = "Pending";
                                                                resp.bankrefid = "";
                                                                Response.data.Add(resp);
                                                            }
                                                            else
                                                            {
                                                                var bname = ""; string payidno = "";
                                                                try
                                                                {
                                                                    //oprid = data1.xml.data.opr_id.ToString();
                                                                    //bname = data1.xml.data.name.ToString();
                                                                    payidno = ADDINFO.status.ToString();
                                                                }
                                                                catch { }
                                                                db.Money_transfer_update_new_new(Tranid, "FAILED", payidno, bname, ch, "", 0, 0);
                                                                //if (StatusSendSmsMoneyTransferFailed == "Y")
                                                                //{
                                                                //    smssend.sendsmsallwhitelabel(whitelabelid, RetailerMob, "Money Transfer in Bank " + bankname + " and Account Number " + bank_account + "and Amount" + transamount + " is Failed Due To " + payidno + ".", "Recharge");
                                                                //}

                                                                smssend.sms_init_whitelabel(whitelabelid, StatusSendSmsMoneyTransferFailed, "N", "MONEYTRANSFERMANUALMONEYFAILEDDUETO", RetailerMob, bankname, bank_account + " ", " " + transamount, payidno);

                                                                dynamic resp = new JObject();
                                                                resp.Amount = remainder;
                                                                resp.Status = "Failed";
                                                                resp.bankrefid = payidno;
                                                                Response.data.Add(resp);
                                                            }


                                                        }
                                                        else
                                                        {
                                                            dynamic json = JsonConvert.DeserializeObject(responsechk);
                                                            var error = json.error.ToString();
                                                            var error_decribe = json["error_description"].ToString();
                                                            results = "{'Details':'" + error_decribe + "','status':'failure'}";
                                                            var jss = new JavaScriptSerializer();
                                                            var dict = jss.Deserialize<dynamic>(results);

                                                            return Json(dict, JsonRequestBehavior.AllowGet);
                                                        }

                                                    }
                                                }
                                            }

                                            var jjj = Response.ToString();
                                            var jss2 = new JavaScriptSerializer();
                                            var dict2 = jss2.Deserialize<dynamic>(jjj);
                                            return Json(dict2, JsonRequestBehavior.AllowGet);
                                        }
                                        else
                                        {
                                            results = "{'Details':'Your Remain Amount is Low.','status':'Failed'}";

                                            var jss1 = new JavaScriptSerializer();
                                            var dict1 = jss1.Deserialize<dynamic>(results);
                                            return Json(dict1, JsonRequestBehavior.AllowGet);
                                        }
                                    }
                                    else
                                    {
                                        results = "{'Details':' Amount Should be Less Rs 25000.','status':'Failed' }";

                                        var jss1 = new JavaScriptSerializer();
                                        var dict1 = jss1.Deserialize<dynamic>(results);
                                        return Json(dict1, JsonRequestBehavior.AllowGet);
                                    }

                                }
                                else
                                {
                                    results = "{'Details':'No api Open.','status':'Failed'}";
                                    var jss1 = new JavaScriptSerializer();
                                    var dict1 = jss1.Deserialize<dynamic>(results);
                                    return Json(dict1, JsonRequestBehavior.AllowGet);
                                }
                            }
                            else
                            {
                                results = "{'Details':'Remain Amount Low','status':'Failed' }";
                                var jss1 = new JavaScriptSerializer();
                                var dict1 = jss1.Deserialize<dynamic>(results);
                                return Json(dict1, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            results = "{'Details':'Please Wait 5 Minutes... Same Amount Not Transfer in same Account','status':'Failed' }";
                            var jss1 = new JavaScriptSerializer();
                            var dict1 = jss1.Deserialize<dynamic>(results);
                            return Json(dict1, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        results = "{'Details':'Failed!!! Limit Over, Current Limit(" + totalremaincheck + ")','status':'Failed'}";
                        var jss1 = new JavaScriptSerializer();
                        var dict1 = jss1.Deserialize<dynamic>(results);
                        return Json(dict1, JsonRequestBehavior.AllowGet);
                    }
                }

                else
                {
                    results = "{'Details':'Wrong Pin!!! Please Enter Correct Pin!!!','status':'Failed'}";
                    var jss1 = new JavaScriptSerializer();
                    var dict1 = jss1.Deserialize<dynamic>(results);
                    return Json(dict1, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                var msg = ex.Message.ToString();
                var results1 = "{'Details':'" + msg + "','status':'Failed' }";
                var jss1 = new JavaScriptSerializer();
                var dict1 = jss1.Deserialize<dynamic>(results1);
                return Json(dict1, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult Fillbankcheck1(string bankname, string accountno)
        {
            moneytransfer_cyberplate cb = new moneytransfer_cyberplate();
            var apinm = db.money_api_status.Where(aa => aa.status == true && aa.catagory== "PAYOUT").SingleOrDefault();
            var apiname = apinm.api_name;
            if (apiname == "INSTANTPAY")
            {
                /***************************  Is Bank Down  ***********************/
                string respon = new InstantPayComnUtil().getBankList(accountno);
                if (respon != "ERROR")
                {
                    dynamic dynJson = JsonConvert.DeserializeObject(respon.ToString());
                    List<BankNameModel> Details = dynJson.data.ToObject<List<BankNameModel>>();
                    var entry = Details.Where(aaa => aaa.bank_name == bankname).FirstOrDefault();
                    if (entry != null && entry.is_down == "1")
                    {
                        var msg = "" + entry.bank_name + " is down, please try later.";
                        var js = "{'Details':'" + msg + "','statuscode':'Failed'}";
                        var jss1 = new JavaScriptSerializer();
                        var dict1 = jss1.Deserialize<dynamic>(js);
                        return Json(dict1, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        //var js = "{'Details':'OK','status':'Success'}";
                        var jss = new JavaScriptSerializer();
                        var dict = jss.Deserialize<dynamic>(respon);
                        return Json(dict, JsonRequestBehavior.AllowGet);
                        //var js = "{'Details':'OK','status':'Success'}";
                        //var jss1 = new JavaScriptSerializer();
                        //var dict1 = jss1.Deserialize<dynamic>(js);
                        //return Json(dict1, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    var jss = new JavaScriptSerializer();
                    var dict = jss.Deserialize<dynamic>(respon);
                    return Json(dict, JsonRequestBehavior.AllowGet);
                    //var js = "{'Details':'OK','status':'Success'}";
                    //var jss1 = new JavaScriptSerializer();
                    //var dict1 = jss1.Deserialize<dynamic>(js);
                    //return Json(dict1, JsonRequestBehavior.AllowGet);
                }
                /***************************  END  ***********************/
            }
            else if (apiname.ToUpper() == "CYBER")
            {
                var ch = cb.getbankname();
                int start1 = ch.IndexOf("ERROR=") + 6;
                int end1 = ch.IndexOf("\r\n", start1);
                string result = ch.Substring(start1, end1 - start1);
                var results = "";
                if (result == "0")
                {
                    int start = ch.IndexOf("ADDINFO=") + 8;
                    int end = ch.IndexOf("DATE=", start);
                    results = ch.Substring(start, end - start);
                    results = HttpUtility.UrlDecode(results);
                    dynamic dynJson = JsonConvert.DeserializeObject(results.ToString());
                    List<BankNameModel> Details = dynJson.data.ToObject<List<BankNameModel>>();
                    var entry = Details.Where(aaa => aaa.bank_name == bankname).FirstOrDefault();
                    if (entry != null && entry.is_down == "1")
                    {
                        var msg = "" + entry.bank_name + " is down, please try latter.";
                        var js = "  {'Details':'" + msg + "','status':'Failed'}";
                        var jss1 = new JavaScriptSerializer();
                        var dict1 = jss1.Deserialize<dynamic>(js);
                        return Json(dict1, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var jss = new JavaScriptSerializer();
                        var dict = jss.Deserialize<dynamic>(Details.ToString());
                        return Json(dict, JsonRequestBehavior.AllowGet);
                        //var js = "  {'Details':'OK','status':'Success'}";
                        //var jss1 = new JavaScriptSerializer();
                        //var dict1 = jss1.Deserialize<dynamic>(js);
                        //return Json(dict1, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    //var jss = new JavaScriptSerializer();
                    //var dict = jss.Deserialize<dynamic>(Details);
                    //return Json(dict, JsonRequestBehavior.AllowGet);
                    var js = "  {'Details':'OK','status':'Success'}";
                    var jss1 = new JavaScriptSerializer();
                    var dict1 = jss1.Deserialize<dynamic>(js);
                    return Json(dict1, JsonRequestBehavior.AllowGet);
                }
            }
            else if (apiname.ToUpper() == "VASTWEB")
            {
                var tokn = db.vastbazzartokens.SingleOrDefault();
                VastBazaar1 cb1 = new VastBazaar1();
                var responseall = cb1.Bank_details(accountno, tokn.apitoken);
                var responsechk = responseall.Content;
                dynamic json = JsonConvert.DeserializeObject(responsechk);
                var respcode = json.Content.ResponseCode.ToString();
                var ADDINFO = json.Content.ADDINFO;
                var ser = JsonConvert.SerializeObject(ADDINFO);
                json = JsonConvert.DeserializeObject(ser);

                JArray a = (JArray)json["data"];
                IList<BankNameModel> Details = a.ToObject<IList<BankNameModel>>();
                var entry = Details.Where(aaa => aaa.bank_name == bankname).FirstOrDefault();
                if (entry != null && entry.is_down == "1")
                {
                    var msg = "" + entry.bank_name + " is down, please try latter.";
                    var js = "  {'Details':'" + msg + "','status':'Failed'}";
                    var jss1 = new JavaScriptSerializer();
                    var dict1 = jss1.Deserialize<dynamic>(js);
                    return Json(dict1, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var results = JsonConvert.SerializeObject(ADDINFO);
                    var jss = new JavaScriptSerializer();
                    var dict = jss.Deserialize<dynamic>(results);
                    return Json(dict, JsonRequestBehavior.AllowGet);
                    //var jss = new JavaScriptSerializer();
                    //var dict = jss.Deserialize<dynamic>(responseall.ToString());
                    //return Json(dict, JsonRequestBehavior.AllowGet);
                    //var js = "  {'Details':'OK','status':'Success'}";
                    //var jss1 = new JavaScriptSerializer();
                    //var dict1 = jss1.Deserialize<dynamic>(js);
                    //return Json(dict1, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                var js = "  {'Details':'OK','status':'Success'}";
                var jss1 = new JavaScriptSerializer();
                var dict1 = jss1.Deserialize<dynamic>(js);
                return Json(dict1, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Fillbankcheck_getname1(string bankname, string accountno)
        {
            moneytransfer_cyberplate cb = new moneytransfer_cyberplate();
            var apinm = db.money_api_status.Where(aa => aa.status ==true && aa.catagory== "PAYOUT").SingleOrDefault();
            var apiname = apinm.api_name;
            if (apiname == "INSTANTPAY")
            {
                /***************************  Is Bank Down  ***********************/
                string respon = new InstantPayComnUtil().getBankList(accountno);
                if (respon != "ERROR")
                {
                    dynamic dynJson = JsonConvert.DeserializeObject(respon.ToString());
                    List<BankNameModel> Details = dynJson.data.ToObject<List<BankNameModel>>();
                    var entry = Details.Where(aaa => aaa.bank_name == bankname).FirstOrDefault();
                    if (entry != null && entry.is_down == "1")
                    {
                        var msg = "" + entry.bank_name + " is down, please try later.";
                        var js = "{'Details':'" + msg + "','statuscode':'Failed'}";
                        var jss1 = new JavaScriptSerializer();
                        var dict1 = jss1.Deserialize<dynamic>(js);
                        return Json(dict1, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        //var js = "{'Details':'OK','status':'Success'}";
                        var js = "{'Details':'ok','status':'Success'}";
                        var jss1 = new JavaScriptSerializer();
                        var dict1 = jss1.Deserialize<dynamic>(js);
                        return Json(dict1, JsonRequestBehavior.AllowGet);
                        //var js = "{'Details':'OK','status':'Success'}";
                        //var jss1 = new JavaScriptSerializer();
                        //var dict1 = jss1.Deserialize<dynamic>(js);
                        //return Json(dict1, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    var jss = new JavaScriptSerializer();
                    var dict = jss.Deserialize<dynamic>(respon);
                    return Json(dict, JsonRequestBehavior.AllowGet);
                    //var js = "{'Details':'OK','status':'Success'}";
                    //var jss1 = new JavaScriptSerializer();
                    //var dict1 = jss1.Deserialize<dynamic>(js);
                    //return Json(dict1, JsonRequestBehavior.AllowGet);
                }
                /***************************  END  ***********************/
            }
            else if (apiname.ToUpper() == "CYBER")
            {
                var ch = cb.getbankname();
                int start1 = ch.IndexOf("ERROR=") + 6;
                int end1 = ch.IndexOf("\r\n", start1);
                string result = ch.Substring(start1, end1 - start1);
                var results = "";
                if (result == "0")
                {
                    int start = ch.IndexOf("ADDINFO=") + 8;
                    int end = ch.IndexOf("DATE=", start);
                    results = ch.Substring(start, end - start);
                    results = HttpUtility.UrlDecode(results);
                    dynamic dynJson = JsonConvert.DeserializeObject(results.ToString());
                    List<BankNameModel> Details = dynJson.data.ToObject<List<BankNameModel>>();
                    var entry = Details.Where(aaa => aaa.bank_name == bankname).FirstOrDefault();
                    if (entry != null && entry.is_down == "1")
                    {
                        var msg = "" + entry.bank_name + " is down, please try latter.";
                        var js = "  {'Details':'" + msg + "','status':'Failed'}";
                        var jss1 = new JavaScriptSerializer();
                        var dict1 = jss1.Deserialize<dynamic>(js);
                        return Json(dict1, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var jss = new JavaScriptSerializer();
                        var dict = jss.Deserialize<dynamic>(Details.ToString());
                        return Json(dict, JsonRequestBehavior.AllowGet);
                        //var js = "  {'Details':'OK','status':'Success'}";
                        //var jss1 = new JavaScriptSerializer();
                        //var dict1 = jss1.Deserialize<dynamic>(js);
                        //return Json(dict1, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    //var jss = new JavaScriptSerializer();
                    //var dict = jss.Deserialize<dynamic>(Details);
                    //return Json(dict, JsonRequestBehavior.AllowGet);
                    var js = "  {'Details':'OK','status':'Success'}";
                    var jss1 = new JavaScriptSerializer();
                    var dict1 = jss1.Deserialize<dynamic>(js);
                    return Json(dict1, JsonRequestBehavior.AllowGet);
                }
            }
            else if (apiname.ToUpper() == "VASTWEB")
            {
                var tokn = db.vastbazzartokens.SingleOrDefault();
                VastBazaar1 cb1 = new VastBazaar1();
                var responseall = cb1.Bank_details(accountno, tokn.apitoken);
                var responsechk = responseall.Content;
                dynamic json = JsonConvert.DeserializeObject(responsechk);
                var respcode = json.Content.ResponseCode.ToString();
                var ADDINFO = json.Content.ADDINFO;
                var ser = JsonConvert.SerializeObject(ADDINFO);
                json = JsonConvert.DeserializeObject(ser);

                JArray a = (JArray)json["data"];
                IList<BankNameModel> Details = a.ToObject<IList<BankNameModel>>();
                var entry = Details.Where(aaa => aaa.bank_name == bankname).FirstOrDefault();
                if (entry != null && entry.is_down == "1")
                {
                    var msg = "" + entry.bank_name + " is down, please try latter.";
                    var js = "  {'Details':'" + msg + "','status':'Failed'}";
                    var jss1 = new JavaScriptSerializer();
                    var dict1 = jss1.Deserialize<dynamic>(js);
                    return Json(dict1, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var js = "  {'Details':'fhfg','status':'Success'}";
                    var jss1 = new JavaScriptSerializer();
                    var dict1 = jss1.Deserialize<dynamic>(js);
                    return Json(dict1, JsonRequestBehavior.AllowGet);
                    //var results = JsonConvert.SerializeObject(ADDINFO);
                    //var jss = new JavaScriptSerializer();
                    //var dict = jss.Deserialize<dynamic>(results);
                    //return Json(dict, JsonRequestBehavior.AllowGet);
                    //var jss = new JavaScriptSerializer();
                    //var dict = jss.Deserialize<dynamic>(responseall.ToString());
                    //return Json(dict, JsonRequestBehavior.AllowGet);
                    //var js = "  {'Details':'OK','status':'Success'}";
                    //var jss1 = new JavaScriptSerializer();
                    //var dict1 = jss1.Deserialize<dynamic>(js);
                    //return Json(dict1, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                var js = "  {'Details':'OK','status':'Success'}";
                var jss1 = new JavaScriptSerializer();
                var dict1 = jss1.Deserialize<dynamic>(js);
                return Json(dict1, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion DMT 1


        #region Travels Report
        /// <summary>
        /// [GET] Displays travel booking transaction report
        /// </summary>
        public ActionResult Travels_Report()
        {
            var userid = User.Identity.GetUserId();
            // traves operators
            var operator_value = db.Operator_Code.Where(a => a.operator_Name == "Hotel" || a.operator_Name == "Bus" || a.operator_Name == "Flight").ToList();
            ViewBag.Operator = new SelectList(operator_value, "new_opt_code", "operator_Name");
            return View();
        }
        #endregion

        #region Hotel
        /// <summary>
        /// [GET] Displays hotel booking transaction report
        /// </summary>
        public ActionResult Hotel_Report()
        {
            var userid = User.Identity.GetUserId();
            // traves operators
            var operator_value = db.Operator_Code.Where(a => a.operator_Name == "Hotel" || a.operator_Name == "Bus" || a.operator_Name == "Flight").ToList();
            ViewBag.Operator = new SelectList(operator_value, "new_opt_code", "operator_Name");
            return View();
        }
        #endregion

        #region Flight Report
        /// <summary>
        /// [GET] Displays flight booking transaction report
        /// </summary>
        public ActionResult Flight_Report()
        {
            var userid = User.Identity.GetUserId();
            // traves operators
            var operator_value = db.Operator_Code.Where(a => a.operator_Name == "Hotel" || a.operator_Name == "Bus" || a.operator_Name == "Flight").ToList();
            ViewBag.Operator = new SelectList(operator_value, "new_opt_code", "operator_Name");
            return View();
        }
        #endregion

        #region Giftcard Report
        /// <summary>
        /// [GET] Displays gift card transaction report
        /// </summary>
        public ActionResult Giftcard_Report()
        {
            var userid = User.Identity.GetUserId();
            var category = db.GiftCards.Distinct().ToList();
            ViewBag.category = new SelectList(category, "Category", "Category");
            return View();
        }
        #endregion

        #region E-commerce Report
        /// <summary>
        /// [GET] Displays e-commerce transaction report
        /// </summary>
        public ActionResult Ecommerce_Report()
        {
            var userid = User.Identity.GetUserId();
            var category = db.Catagories.Distinct().ToList();
            ViewBag.category = new SelectList(category, "CatName", "CatName");
            return View();
        }
        #endregion

        /// <summary>
        /// [GET] Displays complaints and support ticket list
        /// </summary>
        public ActionResult Complaint()
        {
            var userid = User.Identity.GetUserId();
            var dealerid = db.Whitelabel_Retailer_Details.Where(a => a.RetailerId == userid).FirstOrDefault().DealerId;
            var whitelabelid = db.whitelabel_Dealer_Details.Where(a => a.DealerId == dealerid).FirstOrDefault().Whitelabelid;
            var whitelabelemail = db.WhiteLabel_userList.Where(a => a.WhiteLabelID == whitelabelid).FirstOrDefault().EmailId;
            var ch = db.proc_whitelabel_complaint_request(userid, "", whitelabelemail).ToList();
            return View(ch);
        }

        [HttpPost]
        public ActionResult Complaint_insert(string message)
        {
            string userid = User.Identity.GetUserId();
            var dealerid = db.Whitelabel_Retailer_Details.Where(a => a.RetailerId == userid).FirstOrDefault().DealerId;
            var whitelabelid = db.whitelabel_Dealer_Details.Where(a => a.DealerId == dealerid).FirstOrDefault().Whitelabelid;
            var whitelabelemail = db.WhiteLabel_userList.Where(a => a.WhiteLabelID == whitelabelid).FirstOrDefault().EmailId;
            Guid randomId = Guid.NewGuid();
            string uniqueId = randomId.ToString().Substring(0, 18).ToUpper();
            DateTime date = System.DateTime.Now;
            whitelabel_complaint_request objCourse = new whitelabel_complaint_request();
            objCourse.subject = "Chatting";
            objCourse.complant = message;
            objCourse.complaintid = uniqueId;
            objCourse.userid = userid;
            objCourse.sts = "Open";
            objCourse.rdate = date;
            objCourse.Emailid = whitelabelemail;
            db.whitelabel_complaint_request.Add(objCourse);
            db.SaveChanges();
            return RedirectToAction("Complaint");
        }
        //End
        //Profile
        [HttpGet]
        public new ActionResult Profile()
        {
            string userid = User.Identity.GetUserId();
            var userDetails = db.Users.Where(a => a.UserId == userid).SingleOrDefault();
            var ch = db.Whitelabel_Retailer_Details.FirstOrDefault(m => m.RetailerId == userid);
            ViewBag.show = ch;
            var gt = db.State_Desc.Where(a => a.State_id == ch.State).SingleOrDefault().State_name;
            ViewBag.ddlstate = gt;
            var cities = db.District_Desc.Where(c => c.Dist_id == ch.District && c.State_id == ch.State).SingleOrDefault().Dist_Desc;
            ViewBag.district = cities;
            //emd
            //Shwo Dealer Details
            var dealerid = db.Whitelabel_Retailer_Details.Where(a => a.RetailerId == userid).SingleOrDefault().DealerId;
            var dealerdetails = db.whitelabel_Dealer_Details.Where(a => a.DealerId == dealerid).ToList();
            ViewBag.email = dealerdetails.FirstOrDefault().Email;
            ViewBag.name = dealerdetails.FirstOrDefault().DealerName;
            ViewBag.framname = dealerdetails.FirstOrDefault().FarmName;
            ViewBag.dealermobile = dealerdetails.FirstOrDefault().Mobile;
            ViewBag.dealeraddress = dealerdetails.FirstOrDefault().Address;
            return View(userDetails);
        }

        //Edit Profile 
        /// <summary>
        /// [GET] Displays the Retailer profile edit form
        /// </summary>
        public ActionResult Edit_Profile(string Retailerid)
        {

            var show = db.Whitelabel_Retailer_Details.Find(Retailerid);
            ViewBag.Sate = db.State_Desc.Select(a => new SelectListItem { Text = a.State_name, Value = a.State_id.ToString() }).ToList();
            ViewBag.District = db.District_Desc.Where(a => a.State_id == show.State).Select(a => new SelectListItem { Text = a.Dist_Desc, Value = a.Dist_id.ToString() }).ToList();
            return View(show);
        }
        [HttpPost]
        /// <summary>
        /// [POST] Saves updated profile information
        /// </summary>
        public ActionResult Edit_Profile(string RetailerId, Whitelabel_Retailer_Details retalierdetails, HttpPostedFileBase pancardPath, HttpPostedFileBase aadharcardPath, HttpPostedFileBase frimregistrationPath, HttpPostedFileBase Photo, HttpPostedFileBase BackSideAadharcardphoto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["viewrespo"] = "One or more invalid update occured.";
                    return RedirectToAction("Edit_Profile");
                }

                WebImage pancardPath1 = null;
                var newFileName = "";
                var pancardPath2 = "";
                var aadharcardPath2 = "";
                var aadharcardPath3 = "";
                var frimregistrationPath2 = "";
                var Photo2 = "";
                if (Request.HttpMethod == "POST")
                {
                    try
                    {
                        pancardPath1 = WebImage.GetImageFromRequest();
                        if (pancardPath1 != null)
                        {
                            newFileName = Guid.NewGuid().ToString() + "_" +
                           Path.GetFileName(pancardPath1.FileName);
                            pancardPath2 = @"\Retailer_image\" + newFileName;
                            pancardPath1.Save(@"~\" + pancardPath2);
                        }
                        try
                        {
                            var aadharcardPath1 = aadharcardPath.FileName;
                            var aadharcardPathBack = BackSideAadharcardphoto.FileName;
                            if (aadharcardPath1 != null && aadharcardPathBack != null)
                            {
                                var aadharcardPath11 = new System.Web.Helpers.WebImage(aadharcardPath.InputStream);
                                var aadharcardPath22 = new System.Web.Helpers.WebImage(BackSideAadharcardphoto.InputStream);
                                newFileName = Guid.NewGuid().ToString() + "_" +
                                Path.GetFileName(aadharcardPath.FileName);
                                aadharcardPath2 = @"\Retailer_image\" + newFileName;

                                newFileName = Guid.NewGuid().ToString() + "_" +
                                Path.GetFileName(BackSideAadharcardphoto.FileName);

                                aadharcardPath3 = @"\Retailer_image\" + newFileName;

                                aadharcardPath11.Save(@"~\" + aadharcardPath2);
                                aadharcardPath22.Save(@"~\" + aadharcardPath3);
                                //BackSideAadharcardphoto.SaveAs(@"~\Retailer_image\" + aadharcardPath3);
                            }
                        }
                        catch
                        {

                        }
                        try
                        {

                            var frimregisterPath1 = frimregistrationPath.FileName;
                            if (frimregisterPath1 != null)
                            {
                                var frimregisterPath11 = new System.Web.Helpers.WebImage(frimregistrationPath.InputStream);
                                newFileName = Guid.NewGuid().ToString() + "_" +
                                Path.GetFileName(frimregistrationPath.FileName);
                                frimregistrationPath2 = @"\Retailer_image\" + newFileName;
                                frimregisterPath11.Save(@"~\" + frimregistrationPath2);
                            }
                        }
                        catch
                        {

                        }
                        try
                        {
                            var photo1 = Photo.FileName;
                            if (photo1 != null)
                            {
                                var photo11 = new System.Web.Helpers.WebImage(Photo.InputStream);
                                newFileName = Guid.NewGuid().ToString() + "_" +
                                Path.GetFileName(Photo.FileName);
                                Photo2 = @"\Retailer_image\" + newFileName;
                                photo11.Save(@"~\" + Photo2);
                            }
                        }
                        catch
                        {

                        }
                    }
                    catch
                    {

                    }

                }

                DateTime frm = Convert.ToDateTime(retalierdetails.dateofbirth);
                string dof = frm.ToString("MM/dd/yyyy");

                var retailer = db.Whitelabel_Retailer_Details.Single(a => a.RetailerId == RetailerId);
                retailer.RetailerName = string.IsNullOrWhiteSpace(retalierdetails.RetailerName) ? retailer.RetailerName : retalierdetails.RetailerName;
                retailer.Frm_Name = string.IsNullOrWhiteSpace(retalierdetails.Frm_Name) ? retailer.Frm_Name : retalierdetails.Frm_Name;
                retailer.Address = string.IsNullOrWhiteSpace(retalierdetails.Address) ? retailer.Address : retalierdetails.Address;
                retailer.dateofbirth = string.IsNullOrWhiteSpace(dof) ? retailer.dateofbirth : dof;
                retailer.Pincode = string.IsNullOrEmpty(retalierdetails.Pincode.ToString()) ? retailer.Pincode : retalierdetails.Pincode;
                retailer.State = retalierdetails.State;
                retailer.District = retalierdetails.District;
                retailer.AadharCard = string.IsNullOrWhiteSpace(retalierdetails.AadharCard) ? retailer.AadharCard : retalierdetails.AadharCard;
                retailer.PanCard = string.IsNullOrWhiteSpace(retalierdetails.PanCard) ? retailer.PanCard : retalierdetails.PanCard;
                retailer.gst = string.IsNullOrWhiteSpace(retalierdetails.gst) ? retailer.gst : retalierdetails.gst;
                retailer.Position = string.IsNullOrWhiteSpace(retalierdetails.Position) ? retailer.Position : retalierdetails.Position;
                retailer.BusinessType = string.IsNullOrWhiteSpace(retalierdetails.BusinessType) ? retailer.BusinessType : retalierdetails.BusinessType;
                retailer.Bankaccountno = string.IsNullOrWhiteSpace(retalierdetails.Bankaccountno) ? retailer.Bankaccountno : retalierdetails.Bankaccountno;
                retailer.Ifsccode = string.IsNullOrWhiteSpace(retalierdetails.Ifsccode) ? retailer.Ifsccode : retalierdetails.Ifsccode;
                retailer.bankname = string.IsNullOrWhiteSpace(retalierdetails.bankname) ? retailer.bankname : retalierdetails.bankname;
                retailer.accountholder = string.IsNullOrWhiteSpace(retalierdetails.accountholder) ? retailer.accountholder : retalierdetails.accountholder;
                retailer.city = string.IsNullOrWhiteSpace(retalierdetails.city) ? retailer.city : retalierdetails.city;
                retailer.Aepskycstatus = "Pending";

                retailer.pancardPath = string.IsNullOrWhiteSpace(pancardPath2) ? retailer.pancardPath : pancardPath2;
                retailer.aadharcardPath = string.IsNullOrWhiteSpace(aadharcardPath2) ? retailer.aadharcardPath : aadharcardPath2;
                retailer.BackSideAadharcardphoto = string.IsNullOrWhiteSpace(aadharcardPath3) ? retailer.BackSideAadharcardphoto : aadharcardPath3;
                retailer.frimregistrationPath = string.IsNullOrWhiteSpace(frimregistrationPath2) ? retailer.frimregistrationPath : frimregistrationPath2;
                retailer.photo = string.IsNullOrWhiteSpace(Photo2) ? retailer.photo : Photo2;

                db.SaveChanges();
                ViewBag.Sate = db.State_Desc.Select(a => new SelectListItem { Text = a.State_name, Value = a.State_id.ToString() }).ToList();
                ViewBag.District = db.District_Desc.Select(a => new SelectListItem { Text = a.Dist_Desc, Value = a.Dist_id.ToString() }).ToList();
                return RedirectToAction("Profile");
            }
            catch
            {

            }

            return RedirectToAction("Profile");
        }
        public JsonResult FillDistict(int State)
        {
            var cities = db.District_Desc.Where(c => c.State_id == State);
            return Json(cities, JsonRequestBehavior.AllowGet);
        }



        //FINANCIAL INDEX VIEW
        /// <summary>
        /// [GET] Displays the financial transactions landing page
        /// </summary>
        public ActionResult FinacialIndex(string tabvalue)
        {
            ViewBag.tab = tabvalue;
            return View();
        }
        //END

        //Start Offline Pancard  
        /// <summary>
        /// [GET] Displays PAN card service application list
        /// </summary>
        public ActionResult PAN_CARD()
        {
            var userid = User.Identity.GetUserId();

            var sts = (db.Whitelabel_Retailer_Details.Where(aa => aa.RetailerId == userid).Single().Status);
            if (sts == "N")
            {
                FormsAuthentication.SignOut();
                ViewData["msg"] = "Block";
                return RedirectToAction("Login", "Account", new { area = "" });
            }

            DateTime dtFrom = new DateTime();
            dtFrom = DateTime.Now.Date;
            DateTime dtTo = new DateTime();
            dtTo = DateTime.Now.AddDays(1);
            var model = (from x in db.PAN_CARD
                         join y in db.State_Desc on x.State equals y.State_id
                         join z in db.District_Desc
                         on new { a = (int)x.State, b = (int)x.District } equals new { a = z.State_id, b = (int)z.Dist_id }
                         where x.RetailerId == userid && x.IsHidden == false && x.Date < dtTo && x.Date > dtFrom
                         select new PAN_CARD_Info
                         {
                             Acknoledge = x.Acknowledgement,
                             IsApproved = x.IsApproved,
                             Date = x.Date,
                             District = z.Dist_Desc,
                             FName = x.FFirstName ?? "" + x.FMiddleName ?? "" + x.FLastName,
                             Idno = x.Idno,
                             Mobile = x.Mobile,
                             Name = x.Name_of_Card,
                             Retailer = x.RetailerId,
                             State = y.State_name,
                             Status = x.Status,
                             IsDocAccepted = x.IsDocAccepted,
                             IsDocUploaded = x.IsDocUploaded,
                             IsPhysical = x.IsPhysical
                         }
                         ).OrderByDescending(a => a.Date).Take(10).ToList();
            return View(model);

        }

        [HttpPost]
        public ActionResult PAN_CARD(string txt_frm_date, string txt_to_date, string ddl_type, int ddlTop)
        {
            var loggeduser = User.Identity.GetUserId();
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
            var model = new List<PAN_CARD_Info>();
            switch (ddl_type ?? "ALL")
            {
                case "All":
                    model = (from x in db.PAN_CARD
                             join y in db.State_Desc on x.State equals y.State_id
                             join z in db.District_Desc
                             on new { a = (int)x.State, b = (int)x.District } equals new { a = z.State_id, b = (int)z.Dist_id }
                             where x.Date < to_date && x.Date > frm_date && x.RetailerId == loggeduser && (x.IsHidden == null || x.IsHidden == false)
                             select new PAN_CARD_Info
                             {
                                 Acknoledge = x.Acknowledgement,
                                 IsApproved = x.IsApproved,
                                 Date = x.Date,
                                 District = z.Dist_Desc,
                                 FName = x.FFirstName ?? "" + x.FMiddleName ?? "" + x.FLastName,
                                 Idno = x.Idno,
                                 Mobile = x.Mobile,
                                 Name = x.Name_of_Card,
                                 Retailer = x.RetailerId,
                                 State = y.State_name,
                                 Status = x.Status,
                                 IsHidden = x.IsHidden,
                                 IsDocAccepted = x.IsDocAccepted,
                                 IsDocUploaded = x.IsDocUploaded,
                                 IsPhysical = x.IsPhysical
                             }
                            ).OrderByDescending(a => a.Date).Take(ddlTop).ToList();
                    return View(model);
                case "Approved":
                    model = (from x in db.PAN_CARD
                             join y in db.State_Desc on x.State equals y.State_id
                             join z in db.District_Desc
                             on new { a = (int)x.State, b = (int)x.District } equals new { a = z.State_id, b = (int)z.Dist_id }
                             where x.Date < to_date && x.Date > frm_date && x.IsApproved == "Approved" && x.RetailerId == loggeduser && (x.IsHidden == null || x.IsHidden == false)
                             select new PAN_CARD_Info
                             {
                                 Acknoledge = x.Acknowledgement,
                                 IsApproved = x.IsApproved,
                                 Date = x.Date,
                                 District = z.Dist_Desc,
                                 FName = x.FFirstName ?? "" + x.FMiddleName ?? "" + x.FLastName,
                                 Idno = x.Idno,
                                 Mobile = x.Mobile,
                                 Name = x.Name_of_Card,
                                 Retailer = x.RetailerId,
                                 State = y.State_name,
                                 Status = x.Status,
                                 IsHidden = x.IsHidden,
                                 IsDocAccepted = x.IsDocAccepted,
                                 IsDocUploaded = x.IsDocUploaded,
                                 IsPhysical = x.IsPhysical
                             }
                                           ).OrderByDescending(a => a.Date).Take(ddlTop).ToList();
                    return View(model);
                case "Pending":
                    model = (from x in db.PAN_CARD
                             join y in db.State_Desc on x.State equals y.State_id
                             join z in db.District_Desc
                             on new { a = (int)x.State, b = (int)x.District } equals new { a = z.State_id, b = (int)z.Dist_id }
                             where x.Date < to_date && x.Date > frm_date && x.IsApproved == "PENDING" && x.RetailerId == loggeduser && (x.IsHidden == null || x.IsHidden == false)
                             select new PAN_CARD_Info
                             {
                                 Acknoledge = x.Acknowledgement,
                                 IsApproved = x.IsApproved,
                                 Date = x.Date,
                                 District = z.Dist_Desc,
                                 FName = x.FFirstName ?? "" + x.FMiddleName ?? "" + x.FLastName,
                                 Idno = x.Idno,
                                 Mobile = x.Mobile,
                                 Name = x.Name_of_Card,
                                 Retailer = x.RetailerId,
                                 State = y.State_name,
                                 Status = x.Status,
                                 IsHidden = x.IsHidden,
                                 IsDocAccepted = x.IsDocAccepted,
                                 IsDocUploaded = x.IsDocUploaded,
                                 IsPhysical = x.IsPhysical
                             }
                          ).OrderByDescending(a => a.Date).Take(ddlTop).ToList();
                    return View(model);
                case "Rejected":
                    model = (from x in db.PAN_CARD
                             join y in db.State_Desc on x.State equals y.State_id
                             join z in db.District_Desc
                             on new { a = (int)x.State, b = (int)x.District } equals new { a = z.State_id, b = (int)z.Dist_id }
                             where x.Date < to_date && x.Date > frm_date && x.IsApproved == "Rejected" && x.RetailerId == loggeduser && (x.IsHidden == null || x.IsHidden == false)
                             select new PAN_CARD_Info
                             {
                                 Acknoledge = x.Acknowledgement,
                                 IsApproved = x.IsApproved,
                                 Date = x.Date,
                                 District = z.Dist_Desc,
                                 FName = x.FFirstName ?? "" + x.FMiddleName ?? "" + x.FLastName,
                                 Idno = x.Idno,
                                 Mobile = x.Mobile,
                                 Name = x.Name_of_Card,
                                 Retailer = x.RetailerId,
                                 State = y.State_name,
                                 Status = x.Status,
                                 IsHidden = x.IsHidden,
                                 IsDocAccepted = x.IsDocAccepted,
                                 IsDocUploaded = x.IsDocUploaded,
                                 IsPhysical = x.IsPhysical
                             }
                         ).OrderByDescending(a => a.Date).Take(ddlTop).ToList();
                    return View(model);
                case "PHY Submit":
                    model = (from x in db.PAN_CARD
                             join y in db.State_Desc on x.State equals y.State_id
                             join z in db.District_Desc
                             on new { a = (int)x.State, b = (int)x.District } equals new { a = z.State_id, b = (int)z.Dist_id }
                             where x.Date < to_date && x.Date > frm_date && x.IsPhysical == true && x.RetailerId == loggeduser && (x.IsHidden == null || x.IsHidden == false)
                             select new PAN_CARD_Info
                             {
                                 Acknoledge = x.Acknowledgement,
                                 IsApproved = x.IsApproved,
                                 Date = x.Date,
                                 District = z.Dist_Desc,
                                 FName = x.FFirstName ?? "" + x.FMiddleName ?? "" + x.FLastName,
                                 Idno = x.Idno,
                                 Mobile = x.Mobile,
                                 Name = x.Name_of_Card,
                                 Retailer = x.RetailerId,
                                 State = y.State_name,
                                 Status = x.Status,
                                 IsHidden = x.IsHidden,
                                 IsDocAccepted = x.IsDocAccepted,
                                 IsDocUploaded = x.IsDocUploaded,
                                 IsPhysical = x.IsPhysical
                             }
                           ).OrderByDescending(a => a.Date).Take(ddlTop).ToList();
                    return View(model);
                case "Document Pending":
                    model = (from x in db.PAN_CARD
                             join y in db.State_Desc on x.State equals y.State_id
                             join z in db.District_Desc
                             on new { a = (int)x.State, b = (int)x.District } equals new { a = z.State_id, b = (int)z.Dist_id }
                             where x.Date < to_date && x.Date > frm_date && (x.IsDocUploaded == null || x.IsDocUploaded == false) && x.RetailerId == loggeduser && (x.IsHidden == null || x.IsHidden == false)
                             select new PAN_CARD_Info
                             {
                                 Acknoledge = x.Acknowledgement,
                                 IsApproved = x.IsApproved,
                                 Date = x.Date,
                                 District = z.Dist_Desc,
                                 FName = x.FFirstName ?? "" + x.FMiddleName ?? "" + x.FLastName,
                                 Idno = x.Idno,
                                 Mobile = x.Mobile,
                                 Name = x.Name_of_Card,
                                 Retailer = x.RetailerId,
                                 State = y.State_name,
                                 Status = x.Status,
                                 IsHidden = x.IsHidden,
                                 IsDocAccepted = x.IsDocAccepted,
                                 IsDocUploaded = x.IsDocUploaded,
                                 IsPhysical = x.IsPhysical
                             }
                          ).OrderByDescending(a => a.Date).Take(ddlTop).ToList();
                    return View(model);
                case "Document Complete":
                    model = (from x in db.PAN_CARD
                             join y in db.State_Desc on x.State equals y.State_id
                             join z in db.District_Desc
                             on new { a = (int)x.State, b = (int)x.District } equals new { a = z.State_id, b = (int)z.Dist_id }
                             where x.Date < to_date && x.Date > frm_date && x.IsDocAccepted == true && x.RetailerId == loggeduser && (x.IsHidden == null || x.IsHidden == false)
                             select new PAN_CARD_Info
                             {
                                 Acknoledge = x.Acknowledgement,
                                 IsApproved = x.IsApproved,
                                 Date = x.Date,
                                 District = z.Dist_Desc,
                                 FName = x.FFirstName ?? "" + x.FMiddleName ?? "" + x.FLastName,
                                 Idno = x.Idno,
                                 Mobile = x.Mobile,
                                 Name = x.Name_of_Card,
                                 Retailer = x.RetailerId,
                                 State = y.State_name,
                                 Status = x.Status,
                                 IsHidden = x.IsHidden,
                                 IsDocAccepted = x.IsDocAccepted,
                                 IsDocUploaded = x.IsDocUploaded,
                                 IsPhysical = x.IsPhysical
                             }
                           ).OrderByDescending(a => a.Date).Take(ddlTop).ToList();
                    return View(model);
                default:
                    model = (from x in db.PAN_CARD
                             join y in db.State_Desc on x.State equals y.State_id
                             join z in db.District_Desc
                             on new { a = (int)x.State, b = (int)x.District } equals new { a = z.State_id, b = (int)z.Dist_id }
                             where x.Date < to_date && x.Date > frm_date && x.RetailerId == loggeduser && (x.IsHidden == null || x.IsHidden == false)
                             select new PAN_CARD_Info
                             {
                                 Acknoledge = x.Acknowledgement,
                                 IsApproved = x.IsApproved,
                                 Date = x.Date,
                                 District = z.Dist_Desc,
                                 FName = x.FFirstName ?? "" + x.FMiddleName ?? "" + x.FLastName,
                                 Idno = x.Idno,
                                 Mobile = x.Mobile,
                                 Name = x.Name_of_Card,
                                 Retailer = x.RetailerId,
                                 State = y.State_name,
                                 Status = x.Status,
                                 IsHidden = x.IsHidden,
                                 IsDocAccepted = x.IsDocAccepted,
                                 IsDocUploaded = x.IsDocUploaded,
                                 IsPhysical = x.IsPhysical
                             }
                           ).OrderByDescending(a => a.Date).Take(ddlTop).ToList();
                    return View(model);


            }


        }

        //insert pancard 
        /// <summary>
        /// [GET] Displays form to create a new PAN card application
        /// </summary>
        public ActionResult ADD_PAN_CARD()
        {
            ViewBag.state = new SelectList(db.Select_State_Details(), "State_Id", "State_Name");
            RETAILER.Models.PANCARD_Model model = new RETAILER.Models.PANCARD_Model();
            return View();
        }
        [HttpPost]
        public ActionResult ADD_PAN_CARD(RETAILER.Models.PANCARD_Model model)
        {
            try
            {
                var loggeduser = User.Identity.GetUserId();
                var dob = Convert.ToDateTime(model.DOB);
                System.Data.Entity.Core.Objects.ObjectParameter output = new
              System.Data.Entity.Core.Objects.ObjectParameter("output", typeof(string));
                var DOBproof = model.ProofDOB ?? "";

                var x = db.proc_insert_PAN_CARD_Request(loggeduser, model.Category, model.Status, model.PANNo, model.FirstName, model.MiddleName, model.LastName, model.Name_of_Card, model.FFirstName, model.FMiddleName, model.FLastName, model.MFirstName, model.MMiddleName, model.MLastName, dob, model.Gender, model.ISDCode, model.STDCode, model.Mobile, model.Email, model.communicationAddress, model.RA_Address, model.State, model.District, model.ProofIdentity, model.ProofAddress, DOBproof, model.ProcessingFee, model.NameOnAadhaar, "Aadhaar", model.AadhaarNo, output).SingleOrDefault().msg;

                if (x == "INVALID")
                {
                    TempData["Response"] = JsonConvert.SerializeObject(new { Status = false, Message = "Somthing Went wrong!" });
                }
                else if (x == "LOW BALANCE")
                {
                    TempData["Response"] = JsonConvert.SerializeObject(new { Status = false, Message = "Your Wallet Balance is Low!" });
                }

                else if (x == "Already Pending")
                {
                    TempData["Response"] = JsonConvert.SerializeObject(new { Status = false, Message = "This Pan card request is Pending!" });
                }
                else if (x == "OPT Block")
                {
                    TempData["Response"] = JsonConvert.SerializeObject(new { Status = false, Message = "Operator is blocked!" });
                }
                else
                {
                    TempData["Response"] = JsonConvert.SerializeObject(new { Status = true, Message = "PAN Card Added Succesfully!" });
                }
            }
            catch (Exception ex)
            {
                TempData["Response"] = JsonConvert.SerializeObject(new { Status = false, Message = ex.Message });
                return RedirectToAction("PAN_CARD");
            }
            return RedirectToAction("PAN_CARD");
        }

        public ActionResult ADD_PAN_CARD_FINAL(int idno)
        {
            var a = db.PAN_CARD.Where(b => b.Idno == idno).Single();
            RETAILER.Models.PANCARD_Model_FINAL model = new Models.PANCARD_Model_FINAL();//db.PAN_CARD.Where(a=>a.Idno == idno).Select(a => new PANCARD_Model_FINAL {
            model.AadhaarNo = a.AdhaarNo;
            model.AckNo = a.Acknowledgement;

            model.AOType = a.AOTYPE;
            model.AONO = a.AONO;
            model.APRDate = a.APR_DATE != null ? Convert.ToDateTime(a.APR_DATE) : DateTime.Now;
            model.AreaCode = a.AreaCode;
            model.Are_SUBDIV = a.Area_SubDivision;
            model.CapacityOfVerifier = a.CapacityOfVerifier;
            model.Category = a.Category;
            model.communicationAddress = a.communicationAddress;
            model.DefencePersonnel = a.DefencePersonnel;
            model.District = a.District;
            model.DOB = a.DOB.ToString();
            model.AadhaarDOB = a.AadhaarDOB != null ? Convert.ToDateTime(a.AadhaarDOB).ToString() : DateTime.Now.ToString();
            //DocFirmtPage = a.DocForm49FirstPage;
            //DocPDF = a.Docs;
            model.Email = a.Email;
            model.FFirstName = a.FFirstName;
            model.FirstName = a.FirstName;
            model.FLastName = a.FLastName;
            model.FLATNO = a.Flat_Door_Block_No;
            model.FMiddleName = a.FMiddleName;
            model.LastName = a.LastName;
            model.MFirstName = a.MFirstName;
            model.Gender = a.Gender;
            model.GenderAadhaar = a.AdhaarGender;
            model.Idno = a.Idno;
            model.IsBusinessProfession = a.IsBussinessProfession;
            model.IsBusinessProfessionCode = a.BussinessProfessionCode;
            model.ISDCode = a.ISDCode;
            model.IsFatherMother = a.Parent;
            model.isKYCCompliant = a.IsKYCCompliant;
            model.IsSalariedEmployee = a.IsSalariedEmployee;
            model.IsSignaturePresence = a.IsSignaturePresence;
            model.MiddleName = a.MiddleName;
            model.MLastName = a.MLastName;
            model.MMiddleName = a.MMiddleName;
            model.Mobile = a.Mobile;
            model.NameOfVerifier = a.NameofVerifier;
            model.NameOnAadhaar = a.NameOnAadhaar;
            model.Name_of_Card = a.Name_of_Card;
            model.OrganisationName = a.OrganisationName;
            model.PINCODE = a.PIN;
            model.PlaceofVerification = a.PlaceOfVerification;
            model.Post_Street_LANE = a.Road_Street_Lane_Post;
            model.ProcessingFee = a.ProcessingFee;
            model.ProofAddress = a.ProofAddress;
            model.ProofDOB = a.ProofDOB;
            model.ProofIdentity = a.ProofIdentity;
            model.Rangecode = a.RangeCode;
            model.RA_Address = a.RA_Address;
            model.RetailerId = a.RetailerId;
            model.SourceOFIncome = a.SourceOfIncome;
            model.State = a.State;

            model.STDCode = a.STDCode;
            model.Title = a.ApplicantTitle;
            model.VerificationDate = a.VerificationDate != null ? a.VerificationDate.ToString() : DateTime.Now.ToString();
            model.Village = a.Premises_Building_Village;
            var state = new SelectList(db.Select_State_Details(), "State_Id", "State_Name", new { State_Id = model.State }).ToList();
            var selected = state.Where(x => x.Value == model.State.ToString()).First();
            ViewBag.state = state;
            selected.Selected = true;
            var district = new SelectList(from s in db.District_Desc
                                          where s.State_id == model.State
                                          select s, "Dist_id", "Dist_Desc").ToList();
            var selectedDist = district.Where(c => c.Value == model.District.ToString()).First();
            ViewBag.District = district;
            selectedDist.Selected = true;
            return View(model);
        }

        [HttpPost]
        public ActionResult ADD_PAN_CARD_FINAL(RETAILER.Models.PANCARD_Model_FINAL model)
        {

            try
            {
                if (ModelState.IsValid)
                {
                }
                var loggeduser = User.Identity.GetUserId();
                var file = Directory.GetFiles(Server.MapPath("~/PAN_DOC_PDF/"), model.AckNo + "*", SearchOption.AllDirectories)
                    .FirstOrDefault();
                if (file == null)
                {
                    if (model.DocFirmtPage != null)
                    {
                        var InputFileName = Path.GetFileName(model.DocFirmtPage.FileName);
                        var fnWithoutExtension = Path.GetFileNameWithoutExtension(model.DocFirmtPage.FileName);
                        //Check file name to be equal to Acklodgement no.
                        var entryJPG = db.PAN_CARD.Where(a => a.Idno == model.Idno && a.Acknowledgement.Trim() == fnWithoutExtension.Trim()).SingleOrDefault();
                        if (InputFileName != null)
                        {
                            if (entryJPG == null)
                            {
                                TempData["Response"] = JsonConvert.SerializeObject(new { Status = false, Message = "Invalid file name" });
                                return RedirectToAction("PAN_CARD");
                            }
                            else
                            {
                                //Validate file formate and Size.
                                if (model.DocFirmtPage.ContentLength > 1200 * 1024.16) //in  1200kb
                                {
                                    TempData["Response"] = JsonConvert.SerializeObject(new { Status = false, Message = "Too large file!" });
                                    return RedirectToAction("PAN_CARD");
                                }
                                var fileNameParts = model.DocFirmtPage.FileName.Split('.');
                                if (fileNameParts[fileNameParts.Length - 1] != "jpg")
                                {
                                    TempData["Response"] = JsonConvert.SerializeObject(new { Status = false, Message = "Invalid File format!" });
                                    return RedirectToAction("PAN_CARD");
                                }

                            }

                        }
                        else
                        {
                            TempData["Response"] = JsonConvert.SerializeObject(new { Status = false, Message = "Invalid file name" });
                            return RedirectToAction("PAN_CARD");
                        }
                        var ServerSavePath = Path.Combine(Server.MapPath("~/PAN_FirstPage/") + InputFileName);
                        //Save file to server folder  
                        model.DocFirmtPage.SaveAs(ServerSavePath);
                        entryJPG.DocForm49FirstPage = InputFileName;
                        entryJPG.DocumentUpdateDate = DateTime.Now;
                        db.SaveChanges();

                    }
                    if (model.DocPDF != null)
                    {
                        var InputFileName = Path.GetFileName(model.DocPDF.FileName);
                        var fnWithoutExtension = Path.GetFileNameWithoutExtension(model.DocPDF.FileName);
                        var entryPDF = db.PAN_CARD.Where(a => a.Idno == model.Idno && a.Acknowledgement.Trim() == fnWithoutExtension.Trim()).SingleOrDefault();
                        //Check file name to be equal to Acklodgement no.
                        if (InputFileName != null)
                        {
                            if (entryPDF == null)
                            {
                                TempData["Response"] = JsonConvert.SerializeObject(new { Status = false, Message = "Invalid file name" });
                                return RedirectToAction("PAN_CARD");
                            }
                            else
                            {
                                //Validate file formate and Size.
                                if (model.DocFirmtPage.ContentLength > 1048739) //1mb
                                {
                                    TempData["Response"] = JsonConvert.SerializeObject(new { Status = false, Message = "Too large file!" });
                                    return RedirectToAction("PAN_CARD");
                                }
                                var fileNameParts = model.DocPDF.FileName.Split('.');
                                if (fileNameParts[fileNameParts.Length - 1] != "pdf")
                                {
                                    TempData["Response"] = JsonConvert.SerializeObject(new { Status = false, Message = "Invalid File format!" });
                                    return RedirectToAction("PAN_CARD");
                                }

                            }
                        }
                        else
                        {
                            TempData["Response"] = JsonConvert.SerializeObject(new { Status = false, Message = "Invalid file name" });
                            return RedirectToAction("PAN_CARD");
                        }
                        var ServerSavePath = Path.Combine(Server.MapPath("~/PAN_DOC_PDF/") + InputFileName);
                        //Save file to server folder  
                        model.DocPDF.SaveAs(ServerSavePath);
                        entryPDF.Docs = InputFileName;
                        entryPDF.DocumentUpdateDate = DateTime.Now;
                        db.SaveChanges();
                    }
                }



                //Final Update to PAN.
                var entry = db.PAN_CARD.Where(a => a.Idno == model.Idno).Single();

                entry.AdhaarNo = model.AadhaarNo;
                entry.Acknowledgement = model.AckNo;
                entry.Adhaar_EID = "Aadhaar";
                entry.AOTYPE = model.AOType;
                entry.AONO = model.AONO;
                entry.AreaCode = model.AreaCode;
                entry.Area_SubDivision = model.Are_SUBDIV;
                entry.CapacityOfVerifier = model.CapacityOfVerifier;
                entry.Category = model.Category;
                entry.communicationAddress = model.communicationAddress ?? entry.communicationAddress;
                entry.DefencePersonnel = model.DefencePersonnel;
                entry.District = (model.District == 0 ? entry.District : model.District);
                entry.DOB = Convert.ToDateTime(model.DOB);
                if (model.AadhaarDOB != null)
                {
                    entry.AadhaarDOB = Convert.ToDateTime(model.AadhaarDOB);
                }
                entry.Email = model.Email;
                entry.FFirstName = model.FFirstName;
                entry.FirstName = model.FirstName;
                entry.FLastName = model.FLastName;
                entry.Flat_Door_Block_No = model.FLATNO;
                entry.MFirstName = model.FMiddleName;
                entry.LastName = model.LastName;
                entry.MFirstName = model.MFirstName;
                entry.Gender = model.Gender;
                entry.AdhaarGender = model.GenderAadhaar;
                entry.Idno = model.Idno;
                entry.IsBussinessProfession = model.IsBusinessProfession;
                entry.BussinessProfessionCode = model.IsBusinessProfessionCode;
                entry.ISDCode = model.ISDCode;
                entry.Parent = model.IsFatherMother;
                entry.IsKYCCompliant = model.isKYCCompliant;
                entry.IsSalariedEmployee = model.IsSalariedEmployee;
                entry.IsSignaturePresence = model.IsSignaturePresence;
                entry.MiddleName = model.MiddleName;
                entry.MLastName = model.MLastName;
                entry.MMiddleName = model.MMiddleName;
                entry.Mobile = model.Mobile;
                entry.NameofVerifier = model.NameOfVerifier;
                entry.NameOnAadhaar = model.NameOnAadhaar;
                entry.Name_of_Card = model.Name_of_Card;
                entry.OrganisationName = model.OrganisationName;
                entry.PIN = model.PINCODE;
                entry.PlaceOfVerification = model.PlaceofVerification;
                entry.Road_Street_Lane_Post = model.Post_Street_LANE;
                entry.ProofAddress = (model.ProofAddress != null ? model.ProofAddress : entry.ProofAddress);
                entry.ProofDOB = model.ProofDOB ?? entry.ProofDOB;
                entry.ProofIdentity = model.ProofIdentity ?? entry.ProofIdentity;
                entry.RangeCode = model.Rangecode;
                entry.RA_Address = model.RA_Address ?? entry.RA_Address;
                entry.RetailerId = loggeduser;
                entry.SourceOfIncome = model.SourceOFIncome;
                entry.State = (model.State == 0 ? entry.State : model.State);
                entry.STDCode = model.STDCode;
                entry.ApplicantTitle = model.Title;
                if (model.VerificationDate != null)
                {
                    entry.VerificationDate = Convert.ToDateTime(model.VerificationDate);
                }
                entry.Premises_Building_Village = model.Village;
                entry.IsDocUploaded = (entry.DocForm49FirstPage != null && entry.Docs != null) ? true : false;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                TempData["Response"] = JsonConvert.SerializeObject(new { Status = false, Message = ex.Message });
                return RedirectToAction("PAN_CARD");
            }

            return RedirectToAction("PAN_CARD");
        }
        [HttpPost]
        public ActionResult View_PANCARD(int idno)
        {
            var entry = db.PAN_CARD.Where(about => about.Idno == idno).SingleOrDefault();
            ViewBag.state = db.State_Desc.Where(y => y.State_id == entry.State).SingleOrDefault().State_name;
            return View(entry);
        }

        [HttpPost]
        public ActionResult DocumentViewer(int Idno, string ak_number)
        {
            ViewBag.AckNo = ak_number;
            ViewBag.IdNo = Idno;
            return View();
        }

        public ActionResult PAN_FirstPageDownLoad(string ackno)
        {
            string[] filesInDirectory = Directory.GetFiles(Server.MapPath("~/PAN_FirstPage"), ackno + "*");
            if (filesInDirectory.Length == 0)
            {
                return RedirectToAction("PAN_CARD", "Home");
            }
            return File(filesInDirectory[0], "application/force-download", Path.GetFileName(filesInDirectory[0]));
        }
        public ActionResult PAN_DocDownLoad(string ackno)
        {
            string[] filesInDirectory = Directory.GetFiles(Server.MapPath("~/PAN_DOC_PDF"), ackno + "*");
            if (filesInDirectory.Length == 0)
            {
                return RedirectToAction("PAN_CARD", "Home");
            }
            //var FileVirtualPath = "~/APK/paynow.apk";
            return File(filesInDirectory[0], "application/force-download", Path.GetFileName(filesInDirectory[0]));
        }


        public ActionResult PAN_SlipDownload(string ackno)
        {
            if (!string.IsNullOrWhiteSpace(ackno))
            {
                string[] filesInDirectory = Directory.GetFiles(Server.MapPath("~/PAN_Slip"), ackno + "*");
                if (filesInDirectory.Length == 0)
                {
                    return RedirectToAction("PAN_CARD", "Home");
                }
                return File(filesInDirectory[0], "application/force-download", Path.GetFileName(filesInDirectory[0]));
            }
            else
            {
                TempData["Response"] = JsonConvert.SerializeObject(new { Status = false, Message = "Slip Not Available!" });
                return RedirectToAction("PAN_CARD");
            }
        }

        public ActionResult Edit_PAN_CARD(int idno)
        {
            var entry = db.PAN_CARD.Where(a => a.Idno == idno).SingleOrDefault();
            RETAILER.Models.PANCARD_Model model = new RETAILER.Models.PANCARD_Model();
            model.Idno = entry.Idno;
            model.AadhaarNo = entry.AdhaarNo;
            model.Category = entry.Category;
            model.communicationAddress = entry.communicationAddress;
            model.District = entry.District.ToString();
            model.DOB = entry.DOB.ToShortDateString();
            model.Email = entry.Email;
            model.FFirstName = entry.FFirstName;
            model.FirstName = entry.FirstName;
            model.FLastName = entry.FLastName;
            model.FMiddleName = entry.FMiddleName;
            model.Gender = entry.Gender;
            model.ISDCode = entry.ISDCode;
            model.LastName = entry.LastName;
            model.MiddleName = entry.MiddleName;
            model.Mobile = entry.Mobile;
            model.NameOnAadhaar = entry.NameOnAadhaar;
            model.Name_of_Card = entry.Name_of_Card;
            model.ProcessingFee = entry.ProcessingFee;
            model.ProofAddress = entry.ProofAddress;
            model.ProofDOB = entry.ProofDOB;
            model.ProofIdentity = entry.ProofIdentity;
            model.RA_Address = entry.RA_Address;
            model.State = entry.State.ToString();
            model.STDCode = entry.STDCode;
            model.Status = entry.Status;
            model.PANNo = entry.PAN_CARD_NO;
            var state = new SelectList(db.Select_State_Details(), "State_Id", "State_Name", new { State_Id = model.State }).ToList();
            var selected = state.Where(x => x.Value == model.State.ToString()).First();
            ViewBag.state = state;
            selected.Selected = true;
            var district = new SelectList(from s in db.District_Desc
                                          where s.State_id == entry.State
                                          select s, "Dist_id", "Dist_Desc").ToList();
            var selectedDist = district.Where(c => c.Value == model.District.ToString()).First();
            ViewBag.District = district;
            selectedDist.Selected = true;
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit_PAN_CARD(RETAILER.Models.PANCARD_Model model)
        {
            try
            {
                var entry = db.PAN_CARD.Where(a => a.Idno == model.Idno).SingleOrDefault();
                entry.Idno = model.Idno;
                entry.Adhaar_EID = "Aadhaar";
                entry.AdhaarNo = model.AadhaarNo;
                entry.Category = model.Category;
                entry.communicationAddress = model.communicationAddress;
                entry.District = Convert.ToInt16(model.District);
                entry.DOB = Convert.ToDateTime(model.DOB);
                entry.Email = model.Email;
                entry.FFirstName = model.FFirstName;
                entry.FirstName = model.FirstName;
                entry.FLastName = model.FLastName;
                entry.FMiddleName = model.FMiddleName;
                entry.Gender = model.Gender;
                entry.ISDCode = model.ISDCode;
                entry.LastName = model.LastName;
                entry.MiddleName = model.MiddleName;
                entry.Mobile = model.Mobile;
                entry.NameOnAadhaar = model.NameOnAadhaar;
                entry.Name_of_Card = model.Name_of_Card;
                entry.PAN_CARD_NO = model.PANNo ?? entry.PAN_CARD_NO;
                entry.ProcessingFee = model.ProcessingFee;
                entry.ProofAddress = model.ProofAddress;
                entry.ProofDOB = model.ProofDOB ?? entry.ProofDOB;
                entry.ProofIdentity = model.ProofIdentity;
                entry.RA_Address = model.RA_Address;
                entry.State = Convert.ToInt16(model.State);
                entry.STDCode = model.STDCode;
                entry.Status = model.Status ?? entry.Status;
                db.SaveChanges();

                TempData["Response"] = JsonConvert.SerializeObject(new { Status = false, Message = "Updated Successfully!" });
                return RedirectToAction("PAN_CARD");
            }
            catch (Exception ex)
            {

                TempData["Response"] = JsonConvert.SerializeObject(new { Status = false, Message = ex.Message });
                return RedirectToAction("PAN_CARD");
            }
        }

        /// <summary>
        /// [GET] Displays form to send a fund purchase/load request to Dealer
        /// </summary>
        public ActionResult Send_Purchase_Order()
        {
            WhitelabelRetailerFundtransfer vmodel = new WhitelabelRetailerFundtransfer();
            string userid = User.Identity.GetUserId();
            var dealerid = db.Whitelabel_Retailer_Details.Where(a => a.RetailerId == userid).Single().DealerId;
            var whitelabelids = db.whitelabel_Dealer_Details.Where(x => x.DealerId == dealerid).Single().Whitelabelid;

            var ALLDEALERidss = db.whitelabel_Dealer_Details.Where(x => x.Whitelabelid == whitelabelids).Select(x => x.DealerId).ToList();

            var ALLWHitelabelRetailer = db.Whitelabel_Retailer_Details.Where(x => ALLDEALERidss.Contains(x.DealerId));

            List<SelectListItem> listitem = new List<SelectListItem>();
            foreach (var item in ALLWHitelabelRetailer)
            {
                listitem.Add(new SelectListItem { Text = item.Frm_Name, Value = item.RetailerId.ToString() });
            }
            vmodel.ddlRetailers = listitem;
            return View(vmodel);
            // var emailid = db.Users.Where(a => a.UserId == userid).SingleOrDefault().Email;
            // var rem = db.select_whitelabel_retailer_for_ddl("Dealer", dealerid).Where(re => re.RetailerId != userid).ToList();
            // ViewBag.RetailerId = new SelectList(rem, "RetailerId", "RetailerName", null);
            // ViewBag.RetailerId1 = new SelectList(rem, "RetailerId", "RetailerName", null);

        }

        public ActionResult CheckRetailerEmailMobile(string emailmobile)
        {
            WhitelabelRetailerFundtransfer vmodel = new WhitelabelRetailerFundtransfer();

            var user = User.Identity.GetUserId();

            string userid = User.Identity.GetUserId();
            var dealerid = db.Whitelabel_Retailer_Details.Where(a => a.RetailerId == userid).Single().DealerId;
            var whitelabelids = db.whitelabel_Dealer_Details.Where(x => x.DealerId == dealerid).Single().Whitelabelid;

            var ALLDEALERidss = db.whitelabel_Dealer_Details.Where(x => x.Whitelabelid == whitelabelids).Select(x => x.DealerId).ToList();

            var ALLretailerButremoveone = db.Whitelabel_Retailer_Details.Where(x => x.RetailerId != userid).ToList();

            var ALLWHitelabelRetailer = ALLretailerButremoveone.Where(x => ALLDEALERidss.Contains(x.DealerId));

            //List<SelectListItem> listitem = new List<SelectListItem>();
            //foreach (var item in ALLWHitelabelRetailer.Where(x=>x.Mobile== emailmobile ||x.Email.ToLower()== emailmobile.ToLower()))
            //{
            //    listitem.Add(new SelectListItem { Text = item.Frm_Name, Value = item.RetailerId.ToString() });
            //}
            var ddlRetailers = ALLWHitelabelRetailer.Where(x => x.Mobile == emailmobile || x.Email.ToLower() == emailmobile.ToLower());

            if (ddlRetailers != null)
            {
                return Json(new { remfill = ddlRetailers }, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// [GET] Initiates a fund transfer between WhiteLabel retailers
        /// </summary>
        public ActionResult WlRetailerToWlRetailerFundTransfer(string mddistamount, string HDRetailerId, string txtmddidtcomment)
        {
            if (!string.IsNullOrEmpty(HDRetailerId))
            {
                string userid = User.Identity.GetUserId();
                var dealerid = db.Whitelabel_Retailer_Details.Where(a => a.RetailerId == userid).Single().DealerId;
                var whitelabelids = db.whitelabel_Dealer_Details.Where(x => x.DealerId == dealerid).Single().Whitelabelid;
                string reatilerto = db.Whitelabel_Retailer_Details.Where(a => a.RetailerId == HDRetailerId).FirstOrDefault().RetailerId;
                System.Data.Entity.Core.Objects.ObjectParameter output = new
                System.Data.Entity.Core.Objects.ObjectParameter("output", typeof(string));
                var ch = db.whitelabel_rem_to_rem_bal_transfer(userid, reatilerto, txtmddidtcomment, Convert.ToDecimal(mddistamount), whitelabelids, output).SingleOrDefault().msg.ToString();
                return Json(ch, JsonRequestBehavior.AllowGet);
            }
            return Json("Pls Enter Correct Mobile No", JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult MDTODealer(string tabtype, string txt_frm_date, string txt_to_date, string msg = "")
        {
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

            WhitelabelRetailerFundtransfer vmodel = new WhitelabelRetailerFundtransfer();

            string userid = User.Identity.GetUserId();
            switch (tabtype)
            {
                case "fundrecivewhitelabel":



                    vmodel.showdatafundrecivefromwhitelabel = db.select_whitelabel_report_dealer_to_retailer_balance(userid, Convert.ToDateTime(fromdate), Convert.ToDateTime(Todate)).OrderByDescending(x => x.transfer_date).ToList();

                    break;

                case "fundrecivedealer":

                    vmodel.showdatafundrecivefromdealer = db.show_fundrecive_retailer_wdealer(userid, Convert.ToDateTime(fromdate), Convert.ToDateTime(Todate)).OrderByDescending(x => x.RechargeDate).ToList();


                    //  vmodel.funrequesttoadmin = _db.select_master_pur_order(masterid, "ALL", DateTime.Now.AddDays(-1), DateTime.Now.AddDays(1)).ToList();

                    //  vmodel.FundRequestRecived = _db.select_dlm_pur_order("ALL", masterid, DateTime.Now.AddDays(-1), DateTime.Now.AddDays(1)).ToList();

                    break;

                case "creditReportwhitelabel":

                    vmodel.retailer_credit_report_by_Wadmin = db.select_retailer_credit_report_by_Wadmin(userid).ToList();


                    //  vmodel.funrequesttoadmin = _db.select_master_pur_order(masterid, "ALL", DateTime.Now.AddDays(-1), DateTime.Now.AddDays(1)).ToList();

                    //  vmodel.FundRequestRecived = _db.select_dlm_pur_order("ALL", masterid, DateTime.Now.AddDays(-1), DateTime.Now.AddDays(1)).ToList();

                    break;

                case "creditReportdealer":

                    vmodel.retailer_credit_report_by_dealer = db.select_Wretailer_credit_report_by_Wdealer(userid).ToList();


                    //  vmodel.funrequesttoadmin = _db.select_master_pur_order(masterid, "ALL", DateTime.Now.AddDays(-1), DateTime.Now.AddDays(1)).ToList();

                    //  vmodel.FundRequestRecived = _db.select_dlm_pur_order("ALL", masterid, DateTime.Now.AddDays(-1), DateTime.Now.AddDays(1)).ToList();

                    break;



                case "Purchase_ORDER":

                    vmodel.selectrem_purchase_order_reports = db.select_whitelabel_rem_pur_order(userid, "ALL", Convert.ToDateTime(fromdate), Convert.ToDateTime(Todate)).ToList();
                    vmodel.cashdepositecharges = db.chargeswhitelabeltoDLMs.ToList();
                    //vmodel.FundRecievedDetails = db.Select_balance_Super_stokist(masterid, DateTime.Now.AddDays(-30), DateTime.Now.AddDays(1)).ToList();
                    break;
                default:

                    vmodel.selectremtoremreports = db.show_Whitelabel_rem_to_whitelabel_rem_bal(userid, "ALL", Convert.ToDateTime(fromdate), Convert.ToDateTime(Todate)).ToList(); ;


                    break;

            }

            return PartialView("_FundTransferWlRetailerToWlRetailerPartial", vmodel);

        }

        public ActionResult Place_purchase_order(string Payto, string txtcode, string hdSuperstokistID, string hdMDDLM, string hdPaymentMode,
    string hdPaymentAmount, string hdMDDepositeSlipNo, string hdMDTransferType, string hdMDcollection, string hdMDComments,
    string hdMDBank, string hdsupraccno, string hdMDaccountno, string hdMDutrno, string hdMDwallet,
    string hdMDwalletno, string hdMDtransationno, string hdMDsettelment, string hdMDCreditDetail, string hdMDsubject)
        {
            string ch = "";

            string type = hdPaymentMode;
            string comment = hdMDComments;

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
                            imagePath = @"\PurchaseOrderImg\" + newFileName;
                            photo.Resize(300, 300, false, true);
                            photo.Save(@"~\" + imagePath);
                        }
                    }

                    string userid = User.Identity.GetUserId();

                    var count = (db.whitelabel_rem_purchage.Where(aa => aa.sts == "Pending" && aa.remid == userid).Count());
                    int countchk = Convert.ToInt32(count);
                    var acc = "";
                    if (countchk < 1)
                    {
                        var amount = Convert.ToDecimal(hdPaymentAmount);
                        if (amount > 0)
                        {
                            var from = "";
                            if (Payto == "Distributor")
                            {
                                from = (db.Whitelabel_Retailer_Details.Where(aa => aa.RetailerId == userid).Single().DealerId);
                                acc = adminacco;
                            }
                            else
                            {

                                var dealerid = db.Whitelabel_Retailer_Details.Where(a => a.RetailerId == userid).Single().DealerId;
                                var whitelabelids = db.whitelabel_Dealer_Details.Where(x => x.DealerId == dealerid).Single().Whitelabelid;
                                from = db.WhiteLabel_userList.Where(x => x.WhiteLabelID == whitelabelids).FirstOrDefault().WhiteLabelID;
                                acc = adminacco;
                            }
                            //if (Paymode == "3rdParty" || Paymode == "IMPS_RTGS_NEFT" || Paymode == "CASH")
                            //{
                            //    db.insert_whitelabel_rempurchageorder(userid, Paymode, utrno, "", "", Comment, Convert.ToDecimal(balance), payto, type, acc, "", imagePath);
                            //    TempData["success"] = "Your purcharge Order Successfully.";
                            //}
                            //if (type == "Credit")
                            //{
                            //    db.insert_whitelabel_rempurchageorder(userid, Paymode, utrno, "", "", Comment, Convert.ToDecimal(balance), payto, type, acc, "", imagePath);
                            //    TempData["success"] = "Credit Pay Successfully";
                            //}
                            db.insert_whitelabel_rempurchageorder(userid, hdPaymentMode, collectionby, bankname, "", hdMDComments, Convert.ToDecimal(amount), Payto, type, "", adminacco, "", "", "", "", 0, amount - 0);
                            ch = "Your purcharge Order Successfully.";
                            return Json(ch, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            // TempData["show"] = "Amount should be not zero";
                            // return RedirectToAction("Purchase_ORDER");
                            ch = "Amount should be not zero";
                        }
                    }
                    else
                    {
                        ch = "Your purcharge Order Allready Pending.";
                        // TempData["show"] = "Your purcharge Order Allready Pending.";
                    }
                    // return RedirectToAction("Purchase_ORDER");
                }
                catch (Exception e)
                {
                    throw;
                    ch = e.Message;
                }
            }


            return Json(ch, JsonRequestBehavior.AllowGet);
        }



        /// <summary>
        /// [GET] Displays list of all purchase/fund load orders
        /// </summary>
        public ActionResult Purchase_ORDER()
        {
            try
            {
                IEnumerable<select_whitelabel_rem_pur_order_Result> model = null;

                ViewData["show"] = TempData["show"];
                ViewData["success"] = TempData["success"];
                //show old credit pay value
                string userid = User.Identity.GetUserId();
                string txt_frm_date = DateTime.Now.ToString();
                string txt_to_date = DateTime.Now.ToString();
                string frm_date = Convert.ToDateTime(txt_frm_date).ToShortDateString();
                string to_date = Convert.ToDateTime(txt_to_date).AddDays(1).ToString();

                //account no
                var dealerid = db.Whitelabel_Retailer_Details.Where(a => a.RetailerId == userid).Single().DealerId;
                var whitelabelid = db.whitelabel_Dealer_Details.Where(a => a.DealerId == dealerid).Single().Whitelabelid;
                var accunt = (from acc in db.Whitelabel_bank_info where acc.userid == whitelabelid select new { acno = acc.acno, acnooo = acc.acno + "||" + acc.holdername });
                ViewBag.account = new SelectList(accunt, "acno", "acnooo", null);
                var diff1 = (db.Whitelabel_Dealer_To_Retailer_Balance.Where(aa => aa.RetailerId == userid).OrderByDescending(aa => aa.id).Select(c => c.cr).FirstOrDefault());
                diff1 = diff1 ?? 0;
                ViewData["oldcredit"] = diff1;
                model = db.select_whitelabel_rem_pur_order(userid, "ALL", Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
                return View(model);

            }
            catch
            {
                return RedirectToAction("Dashboard");

            }
        }
        [HttpPost]
        public ActionResult Purchase_ORDER(string txt_frm_date, string txt_to_date)
        {
            try
            {
                IEnumerable<select_whitelabel_rem_pur_order_Result> model = null;
                ViewData["show"] = TempData["show"];
                string userid = User.Identity.GetUserId();
                //Calander Code
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
                var dealerid = db.Whitelabel_Retailer_Details.Where(a => a.RetailerId == userid).Single().DealerId;
                var whitelabelid = db.whitelabel_Dealer_Details.Where(a => a.DealerId == dealerid).Single().Whitelabelid;
                var accunt = (from acc in db.Whitelabel_bank_info where acc.userid == whitelabelid select new { acno = acc.acno, acnooo = acc.acno + "||" + acc.holdername });
                ViewBag.account = new SelectList(accunt, "acno", "acnooo", null);

                var diff1 = (db.Whitelabel_Dealer_To_Retailer_Balance.Where(aa => aa.RetailerId == userid).OrderByDescending(aa => aa.id).Select(c => c.cr).FirstOrDefault());
                diff1 = diff1 ?? 0;
                ViewData["oldcredit"] = diff1;
                model = db.select_whitelabel_rem_pur_order(userid, "ALL", Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
                return View(model);
            }
            catch
            {
                return RedirectToAction("Dashboard");
            }
        }
        [HttpPost]
        public ActionResult purchageorder(string payto, string Paymode, string type, string utrno, string Comment, decimal balance, string accountno, string txttoaccountno)
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
                            imagePath = @"\PurchaseOrderImg\" + newFileName;
                            photo.Resize(300, 300, false, true);
                            photo.Save(@"~\" + imagePath);
                        }
                    }

                    string userid = User.Identity.GetUserId();

                    var count = (db.whitelabel_rem_purchage.Where(aa => aa.sts == "Pending" && aa.remid == userid).Count());
                    int countchk = Convert.ToInt32(count);
                    var acc = "";
                    if (countchk < 1)
                    {
                        var amount = Convert.ToDecimal(balance);
                        if (amount > 0)
                        {
                            var from = "";
                            if (payto == "Distributor")
                            {
                                from = (db.Whitelabel_Retailer_Details.Where(aa => aa.RetailerId == userid).Single().DealerId);
                                acc = txttoaccountno;
                            }
                            else
                            {
                                from = db.WhiteLabel_userList.Single().WhiteLabelID;
                                acc = accountno;
                            }
                            if (Paymode == "3rdParty" || Paymode == "IMPS_RTGS_NEFT" || Paymode == "CASH")
                            {
                                db.insert_whitelabel_rempurchageorder(userid, Paymode, utrno, "", "", Comment, amount, payto, type, acc, "", "", "", "", "", 0, amount - 0);
                                TempData["success"] = "Your purcharge Order Successfully.";
                            }
                            if (type == "Credit")
                            {
                                db.insert_whitelabel_rempurchageorder(userid, Paymode, utrno, "", "", Comment, amount, payto, type, acc, "", "", "", "", "", 0, amount - 0);
                                TempData["success"] = "Credit Pay Successfully";
                            }
                        }
                        else
                        {
                            TempData["show"] = "Amount should be not zero";
                            return RedirectToAction("Purchase_ORDER");
                        }
                    }
                    else
                    {
                        TempData["show"] = "Your purcharge Order Allready Pending.";
                    }
                    return RedirectToAction("Purchase_ORDER");
                }
                catch (Exception e)
                {
                    throw;
                }
            }
            //using (VastwebmultiEntities db = new VastwebmultiEntities())
            //{
            //    try
            //    {
            //        WebImage photo = null;
            //        var newFileName = "";
            //        var imagePath = "";

            //        if (Request.HttpMethod == "POST")
            //        {
            //            photo = WebImage.GetImageFromRequest();
            //            if (photo != null)
            //            {
            //                newFileName = Guid.NewGuid().ToString() + "_" +
            //                    Path.GetFileName(photo.FileName);
            //                imagePath = @"\PurchaseOrderImg\" + newFileName;
            //                photo.Resize(300, 300, false, true);
            //                photo.Save(@"~\" + imagePath);
            //            }
            //        }

            //        string userid = User.Identity.GetUserId();

            //        var count = (db.whitelabel_rem_purchage.Where(aa => aa.sts == "Pending" && aa.remid == userid).Count());
            //        int countchk = Convert.ToInt32(count);
            //        var acc = "";
            //        if (countchk < 1)
            //        {
            //            var amount = Convert.ToDecimal(balance);

            //            if (amount > 0)
            //            {
            //                var from = "";
            //                if (payto == "Distributor")
            //                {
            //                    acc = txttoaccountno;
            //                }
            //                else
            //                {

            //                    acc = accountno;
            //                }

            //                if (Paymode == "3rdParty" || Paymode == "IMPS_RTGS_NEFT" || Paymode == "CASH")
            //                {
            //                    db.insert_whitelabel_rempurchageorder(userid, Paymode, utrno, "", "", Comment, Convert.ToDecimal(balance), payto, type, acc, "", imagePath);
            //                    TempData["success"] = "Your purcharge Order Successfully.";


            //                }
            //                if (type == "Credit")
            //                {
            //                    db.insert_whitelabel_rempurchageorder(userid, Paymode, utrno, "", "", Comment, Convert.ToDecimal(balance), payto, type, acc, "", imagePath);
            //                    TempData["success"] = "Credit Pay Successfully";
            //                }
            //            }
            //            else
            //            {
            //                TempData["show"] = "Amount should be not zero";
            //                return RedirectToAction("Purchase_ORDER");
            //            }
            //        }
            //        else
            //        {
            //            TempData["show"] = "Your purcharge Order Allready Pending.";
            //        }
            //        return RedirectToAction("Purchase_ORDER");


            //    }
            //    catch (Exception ex)
            //    {
            //            //        throw;
            //    }
            //}
        }

        //public ActionResult R_Creditchk(string MID)
        //{
        //    string userid = User.Identity.GetUserId();
        //    var from = "";
        //    if (MID == "Distributor")
        //    {
        //        from = (db.Whitelabel_Retailer_Details.Where(aa => aa.RetailerId == userid).Single().DealerId);
        //    }
        //    else
        //    {
        //        var dealerid = db.Whitelabel_Retailer_Details.Where(a => a.RetailerId == userid).SingleOrDefault().DealerId;
        //        var whitelabelid = db.whitelabel_Dealer_Details.Where(a => a.DealerId == dealerid).SingleOrDefault().Whitelabelid;
        //        from = whitelabelid;
        //        //from = db.WhiteLabel_userList.Single().WhiteLabelID;
        //    }
        //    var diff1 = (db.Whitelabel_Dealer_To_Retailer_Balance.Where(aa => aa.RetailerId == userid && aa.DealerId == from).OrderByDescending(aa => aa.id).Select(aa => aa.cr).FirstOrDefault());
        //    diff1 = diff1 ?? 0;
        //    return Json(diff1, JsonRequestBehavior.AllowGet);

        //}


        public ActionResult R_Creditchk(string MID)
        {
            string tabtype = "";
            string userid = User.Identity.GetUserId();
            List<SelectListItem> bankitem = new List<SelectListItem>();
            List<SelectListItem> walletitem = new List<SelectListItem>();
            dynamic diff1;
            var from = "";
            if (MID == "Distributor")
            {
                //My Credit Bal to Dealer


                from = (db.Whitelabel_Retailer_Details.Where(aa => aa.RetailerId == userid).Single().DealerId);
                tabtype = "Distributor";
                diff1 = (db.Whitelabel_Dealer_To_Retailer_Balance.Where(aa => aa.RetailerId == userid && aa.DealerId == from).OrderByDescending(aa => aa.id).Select(aa => aa.cr).FirstOrDefault());



            }
            else
            {

                var dealerid = db.Whitelabel_Retailer_Details.Where(a => a.RetailerId == userid).SingleOrDefault().DealerId;
                var whitelabelid = db.whitelabel_Dealer_Details.Where(a => a.DealerId == dealerid).SingleOrDefault().Whitelabelid;
                from = whitelabelid;
                diff1 = (db.whitelabel_to_retailer.Where(aa => aa.retailerid == userid && aa.whitelabelid == from).OrderByDescending(aa => aa.idno).Select(aa => aa.cr).FirstOrDefault());

                tabtype = "whitelabelid";
            }

            diff1 = diff1 ?? 0;

            var bindbank = db.bank_info.Where(x => x.userid == from).ToList();

            foreach (var bank in bindbank)
            {
                bankitem.Add(new SelectListItem { Text = bank.banknm, Value = bank.acno });
            }
            var bindwallet = db.tblwallet_info.Where(x => x.userid == from).ToList();
            // List<SelectListItem> walletitem = new List<SelectListItem>();
            foreach (var wallet in bindwallet)
            {
                walletitem.Add(new SelectListItem { Text = wallet.walletname, Value = wallet.walletno });
            }

            return Json(new { diff = diff1, listbank = bankitem, walletinfo = walletitem }, JsonRequestBehavior.AllowGet);

        }







        //Change Password 
        [HttpGet]
        /// <summary>
        /// [GET] Displays the change password form
        /// </summary>
        public ActionResult ChangePassword()
        {
            ViewData["success"] = TempData["success"];
            ViewData["error"] = TempData["error"];
            TempData.Remove("success");
            TempData.Remove("error");
            return View();
        }
        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        /// <summary>
        /// [POST] Processes the password change request
        /// </summary>
        public async Task<ActionResult> ChangePassword([Bind(Prefix = "Item1")] ChangePasswordViewModel model)
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

                TempData["success"] = "Your Password has been Changed Successfully..";
                return RedirectToAction("ChangePassword");
            }
            AddErrors(result);
            return View(model);
        }

        /// <summary>
        /// [GET] Displays the change IMPS/transaction PIN form
        /// </summary>
        public ActionResult ChangePin()
        {
            return View();
        }

        public ActionResult Reset_IMPSPin(string txtemail)
        {
            var chk = db.Whitelabel_Retailer_Details.Any(a => a.Email == txtemail);

            if (chk == true)
            {
                int pin = new Random().Next(1000, 10000);
                var msg = Encrypt(pin.ToString());
                var userid = User.Identity.GetUserId();
                var id = (from tbl in db.Whitelabel_Retailer_Details
                          where tbl.RetailerId == userid
                          select tbl).SingleOrDefault();
                id.PIN = msg;
                db.SaveChanges();
                var emailid = db.Whitelabel_Retailer_Details.Single(p => p.RetailerId == userid).Email;

                var dealerid = db.Whitelabel_Retailer_Details.Where(a => a.RetailerId == userid).FirstOrDefault().DealerId;
                var whitelabelid = db.whitelabel_Dealer_Details.Where(a => a.DealerId == dealerid).FirstOrDefault().Whitelabelid;

                var ToCC = db.Admin_details.FirstOrDefault().email;
                CommUtilEmail emailsend = new CommUtilEmail();
                emailsend.WhiteLabelEmailLimitChk(emailid, ToCC, "IMPS Pin", "Your IMPS Transaction Pin Is  " + pin, "", whitelabelid);

                TempData["success"] = "Your IMPS Pin Send Successfully in Your Mail Id . Please Check Your Mail Id.";

            }
            else
            {
                TempData["error"] = "Your Email Not Verify.Please Enter Your Corrent Registered Email";
            }

            return RedirectToAction("ChangePassword");
        }

        [HttpPost]
        public ActionResult Edit_IMPSPin([Bind(Prefix = "Item2")] ChangePinViewModel model)
        {
            var userid = User.Identity.GetUserId();
            var oldpin = db.Whitelabel_Retailer_Details.Single(p => p.RetailerId == userid).PIN;
            var oldp = Decrypt(oldpin);
            if (oldp != model.OldPin)
            {
                TempData["error"] = "Your Old Pin does Not Match .Please Re-Enter Your Old Pin ";
                return RedirectToAction("ChangePassword");
            }
            var msg = Encrypt(model.NewPin);

            var id = (from tbl in db.Whitelabel_Retailer_Details
                      where tbl.RetailerId == userid
                      select tbl).SingleOrDefault();
            id.PIN = msg;
            db.SaveChanges();
            var emailid = db.Whitelabel_Retailer_Details.Single(p => p.RetailerId == userid).Email;
            var dealerid = db.Whitelabel_Retailer_Details.Where(a => a.RetailerId == userid).FirstOrDefault().DealerId;
            var whitelabelid = db.whitelabel_Dealer_Details.Where(a => a.DealerId == dealerid).FirstOrDefault().Whitelabelid;

            var ToCC = db.Admin_details.FirstOrDefault().email;
            CommUtilEmail emailsend = new CommUtilEmail();
            emailsend.WhiteLabelEmailLimitChk(emailid, ToCC, "IMPS Pin", "Your New IMPS Transaction Pin Is  " + model.NewPin, "", whitelabelid);

            TempData["success"] = "Your IMPS Transaction Pin Change and Send Successfully in Your Mail Id . Please Check Your Mail Id.";
            return RedirectToAction("ChangePassword");
        }


        public ActionResult Edit_IMPSPin()
        {
            // ViewData["succ"] = TempData["success"];
            return View();
        }
        /// <summary>
        /// [GET] Displays security settings and two-factor authentication page
        /// </summary>
        public ActionResult Security_Page()
        {

            return View();
        }
        /// <summary>
        /// [GET] Displays Retailer's earnings and commission summary
        /// </summary>
        public ActionResult My_Earn()
        {

            return View();
        }
        /// <summary>
        /// [GET] Displays retailer daily-book financial summary
        /// </summary>
        public ActionResult Retailer_Daybook_Reports()
        {

            return View();
        }
        public ActionResult Service_Fee()
        {

            return View();
        }
        public ActionResult Markup_Setting()
        {

            return View();
        }
        public ActionResult My_Credit()
        {
            var userid = User.Identity.GetUserId();
            var retailer = db.Whitelabel_Retailer_Details.Where(s => s.RetailerId == userid).Single();
            var dealerid = retailer.DealerId;
            var whitelabelid = retailer.Whitelabelid;
            decimal admin_cr = 0;
            decimal dealer_cr = 0;

            var admin = db.WhiteLabel_userList.Where(s => s.WhiteLabelID == whitelabelid).Single();
            var mybalance = db.Whitelabel_Remain_reteller_balance.Where(s => s.RetellerId == userid).Single().Remainamount;
            var admin_credit = db.Whitelabel_Dealer_To_Retailer_Balance.Where(s => s.DealerId == whitelabelid && s.RetailerId == userid).OrderByDescending(s => s.id).Take(1).FirstOrDefault();
            var dealer_credit = db.Whitelabel_Dealer_To_Retailer_Balance.Where(s => s.DealerId == dealerid && s.RetailerId == userid).OrderByDescending(s => s.id).Take(1).FirstOrDefault();
            if (admin_credit != null)
            {
                admin_cr = admin_credit?.cr ?? 0;
            }
            if (dealer_credit != null)
            {
                dealer_cr = dealer_credit?.cr ?? 0;
            }
            ViewBag.admin_cr = admin_cr;
            ViewBag.dealer_cr = dealer_cr;
            ViewBag.company = admin;
            ViewBag.dealer = db.whitelabel_Dealer_Details.Where(s => s.DealerId == dealerid).Single();
            ViewBag.balance = mybalance;

            return View();
        }
        /// <summary>
        /// [GET] Displays Retailer-to-Retailer fund transfer page
        /// </summary>
        public ActionResult FundTransferRetailer()
        {
            var userid = User.Identity.GetUserId();
            var purchase_entry = db.select_whitelabel_rem_pur_order(userid, "ALL", DateTime.Now.AddDays(-1), DateTime.Now.AddDays(1)).ToList();
            ViewBag.purchase_entry = purchase_entry;
            return View();
        }


        public ActionResult PlacePurchaseOrder(string payto, string txtcode, string hdSuperstokistID, string hdMDDLM, string hdPaymentMode,
      string hdPaymentAmount, string hdMDDepositeSlipNo, string hdMDTransferType, string hdMDcollection, string hdMDComments,
      string hdMDBank, string hdsupraccno, string hdMDaccountno, string hdMDutrno, string hdMDwallet,
      string hdMDwalletno, string hdMDtransationno, string hdMDsettelment, string hdMDCreditDetail, string hdMDsubject)
        {
            dynamic ch;

            string type = hdPaymentMode;
            string comment = hdMDComments;

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
            //  string ddRETaccountno = ddAdminaccountname;

            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                try
                {
                    string userid = User.Identity.GetUserId();
                    string wid = db.Whitelabel_Retailer_Details.Single(s => s.RetailerId == userid).Whitelabelid;

                    var count = (db.whitelabel_rem_purchage.Where(aa => aa.sts == "Pending" && aa.remid == userid).Count());

                    int countchk = Convert.ToInt32(count);
                    var acc = adminacco;

                    var amount = Convert.ToDecimal(hdPaymentAmount);
                    if (amount > 0)
                    {
                        var from = "";
                        decimal disCharge = 0;
                        if (payto == "Distributor")
                        {
                            from = (db.Whitelabel_Retailer_Details.Where(aa => aa.RetailerId == userid).Single().DealerId);

                            var chargeEntry = db.Whitelabel_creditchargeretailer.Where(aa => aa.chargefrom == from && aa.userid == userid && aa.Whitelabelid == wid).FirstOrDefault(a => a.type == hdPaymentMode);
                            if (chargeEntry != null)
                            {
                                if (chargeEntry != null)
                                {
                                    disCharge = (amount * Convert.ToDecimal(chargeEntry.charge.Value)) / 100;
                                }
                            }
                        }
                        else
                        {
                            from = userid;

                            var chargeEntry = db.Whitelabel_creditchargeretailer.Where(aa => aa.chargefrom == "WAdmin" && aa.userid == userid && aa.Whitelabelid == wid).FirstOrDefault(a => a.type == hdPaymentMode);

                            if (chargeEntry != null)
                            {
                                disCharge = (amount * Convert.ToDecimal(chargeEntry.charge.Value)) / 100;
                            }
                        }

                        db.insert_whitelabel_rempurchageorder(userid, hdPaymentMode, collectionby, adminacco, "", hdMDComments, Convert.ToDecimal(amount), payto, "", "", acc, "", "", "", "", disCharge, amount - disCharge);
                        TempData["success"] = "Your Rquest Proceed Successfully.";
                        ch = "Your purcharge Order Successfully.";
                    }
                    else
                    {
                        TempData["error"] = "Amount should be not zero";
                        ch = "Amount should be not zero";

                        return Json(ch, JsonRequestBehavior.AllowGet);
                    }

                    return Json(ch, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    TempData["error"] = ex;
                    ch = ex.Message;
                    return Json(ch, JsonRequestBehavior.AllowGet);
                }
            }
        }

        //public ActionResult UPITRANSFER()
        //{

        //    return View();
        //}
        public ActionResult wallethistory()
        {

            return View();
        }
        public ActionResult Fund_Transfer_Retailer_To_Retailer()
        {

            return View();
        }


        #region Upi 

        /// <summary>
        /// [GET] Displays UPI transfer history and interface
        /// </summary>
        public ActionResult UPITRANSFER()
        {
            ViewData["upi_msg"] = TempData["upimsg"];
            TempData.Remove("upimsg");
            var userid = User.Identity.GetUserId();
            var whitelabelid = db.Whitelabel_Retailer_Details.Where(s => s.RetailerId == userid).SingleOrDefault().Whitelabelid;
            var chk = db.Whitelabel_UPI_Ref_details.Where(aa => aa.userid == userid && aa.whitelabelid == whitelabelid).SingleOrDefault();
            var setok = "OK";
            var upistatus = db.Whitelabel_Hidw_Show.Where(x => x.UseFor.ToUpper() == "UPI" && x.whitelabelid == whitelabelid).FirstOrDefault().Status;

            if (upistatus == "Y")
            {
                if (chk == null)
                {
                    setok = "NOTOK";
                }

                ViewBag.set = setok;

                if (setok == "OK")
                {
                    var done = "OK";
                    var chkchk = db.Whitelabel_Upi_slab.Where(u => u.whitelabelid == whitelabelid).SingleOrDefault();
                    if (chkchk == null)
                    {
                        done = "NOTOK";
                        ViewBag.msg = "Charge Not Set";
                    }
                    if (done == "OK")
                    {
                        var rem = db.Whitelabel_Retailer_Details.Where(aa => aa.RetailerId == userid && aa.Whitelabelid == whitelabelid).SingleOrDefault();
                        var finaltxn = db.Whitelabel_UPI_Ref_details.Where(aa => aa.userid == userid && aa.whitelabelid == whitelabelid).SingleOrDefault().UPITxnid;
                        var upiinfo = db.Upi_info.SingleOrDefault();
                        var upitxt = "upi://pay?pa=" + upiinfo.MerchantVPA + "&pn=test&tr=" + finaltxn + "&am=&cu=INR&mc=5411";
                        var barcodeWriter = new BarcodeWriter();
                        barcodeWriter.Format = BarcodeFormat.QR_CODE;
                        barcodeWriter.Options = new ZXing.Common.EncodingOptions
                        {
                            Width = 250,
                            Height = 250,
                            Margin = 4
                        };
                        var result = barcodeWriter.Write(upitxt);
                        var barcodeBitmap = new Bitmap(result);
                        using (MemoryStream memory = new MemoryStream())
                        {
                            using (Bitmap bitMap = barcodeBitmap)
                            {

                                bitMap.Save(memory, ImageFormat.Png);

                                ViewBag.QRCodeImage = "data:image/png;base64," + Convert.ToBase64String(memory.ToArray());

                            }
                        }
                    }
                }

            }

            var slab = db.Whitelabel_Upi_slab.Where(u => u.whitelabelid == whitelabelid).SingleOrDefault();

            if (slab == null)
            {
                ViewBag.min = 0;
                ViewBag.charge = 0;
            }
            else
            {
                ViewBag.min = slab.min;
                ViewBag.charge = slab.Charge;
            }
            DateTime from = DateTime.Now.Date;
            DateTime to = DateTime.Now.AddDays(1).Date;
            var ch = db.Whitelabel_show_upi_txn_details(whitelabelid, "Retailer", userid, "ALL", from, to).ToList();

            var upitxndetails = db.Whitelabel_Upi_txn_details.Where(x => x.userid == userid && x.rolename == "Retailer" && x.txndate >= from && x.txndate <= to);
            var totalsuccess = upitxndetails.Where(x => x.status.ToUpper().Contains("SUCCESS")).Sum(x => x.amt);
            var totalFailed = upitxndetails.Where(x => x.status.ToUpper().Contains("FAILED")).Sum(x => x.amt);
            var totalpending = upitxndetails.Where(x => x.status.ToUpper().Contains("PENDING")).Sum(x => x.amt);
            var totalcharges = upitxndetails.Sum(x => x.charge);

            ViewBag.totalsuccess = totalsuccess;
            ViewBag.totalfailedamount = totalFailed;
            ViewBag.totalpendingamount = totalpending;
            ViewBag.totalchargesamount = totalcharges;

            return View(ch);
        }

        [HttpPost]
        public ActionResult UPITRANSFER(DateTime txt_frm_date, DateTime txt_to_date)
        {
            ViewBag.chk = "post";
            var userid = User.Identity.GetUserId();
            var rem = db.Whitelabel_Retailer_Details.Where(aa => aa.RetailerId == userid).SingleOrDefault();
            var whitelabelid = rem.Whitelabelid;
            var upistatus = db.Whitelabel_Hidw_Show.Where(x => x.UseFor.ToUpper() == "UPI" && x.whitelabelid == whitelabelid).FirstOrDefault().Status;

            if (upistatus == "Y")
            {
                var finaltxn = db.Whitelabel_UPI_Ref_details.Where(aa => aa.userid == userid && aa.whitelabelid == whitelabelid).SingleOrDefault().UPITxnid;
                var upiinfo = db.Upi_info.SingleOrDefault();
                var upitxt = "upi://pay?pa=" + upiinfo.MerchantVPA + "&pn=test&tr=" + finaltxn + "&am=&cu=INR&mc=5411";
                using (MemoryStream ms = new MemoryStream())
                {
                    var done = "OK";
                    var chkchk = db.Whitelabel_Upi_slab.Where(u => u.whitelabelid == whitelabelid).SingleOrDefault();
                    if (chkchk == null)
                    {
                        done = "NOTOK";
                        ViewBag.msg = "Charge Not Set";
                    }
                    if (done == "OK")
                    {
                        var barcodeWriter = new BarcodeWriter();
                        barcodeWriter.Format = BarcodeFormat.QR_CODE;
                        barcodeWriter.Options = new ZXing.Common.EncodingOptions
                        {
                            Width = 250,
                            Height = 250,
                            Margin = 4
                        };
                        var result = barcodeWriter.Write(upitxt);
                        var barcodeBitmap = new Bitmap(result);
                        using (MemoryStream memory = new MemoryStream())
                        {
                            using (Bitmap bitMap = barcodeBitmap)
                            {

                                bitMap.Save(memory, ImageFormat.Png);
                                ViewBag.QRCodeImage = "data:image/png;base64," + Convert.ToBase64String(memory.ToArray());
                            }
                        }
                    }
                }

            }


            var slab = db.Whitelabel_Upi_slab.Where(s => s.whitelabelid == whitelabelid).SingleOrDefault();
            if (slab == null)
            {
                ViewBag.min = 0;
                ViewBag.charge = 0;
            }
            else
            {
                ViewBag.min = slab.min;
                ViewBag.charge = slab.Charge;
            }
            DateTime to = txt_to_date.AddDays(1);
            var ch = db.Whitelabel_show_upi_txn_details(whitelabelid, "Retailer", userid, "ALL", txt_frm_date, to).ToList();

            var upitxndetails = db.Whitelabel_Upi_txn_details.Where(x => x.userid == userid && x.rolename == "Retailer" && x.txndate >= txt_frm_date && x.txndate <= to);
            var totalsuccess = upitxndetails.Where(x => x.status.ToUpper().Contains("SUCCESS")).Sum(x => x.amt);
            var totalFailed = upitxndetails.Where(x => x.status.ToUpper().Contains("FAILED")).Sum(x => x.amt);
            var totalpending = upitxndetails.Where(x => x.status.ToUpper().Contains("PENDING")).Sum(x => x.amt);
            var totalcharges = upitxndetails.Sum(x => x.charge);

            ViewBag.totalsuccess = totalsuccess;
            ViewBag.totalfailedamount = totalFailed;
            ViewBag.totalpendingamount = totalpending;
            ViewBag.totalchargesamount = totalcharges;

            return View(ch);
        }

        [HttpPost]
        public ActionResult UPITRANSFER_add()
        {

            var userid = User.Identity.GetUserId();
            var whitelabelid = db.Whitelabel_Retailer_Details.Where(s => s.RetailerId == userid).SingleOrDefault().Whitelabelid;
            var chkfor = db.Whitelabel_UPI_Ref_details.Where(aa => aa.userid == userid && aa.whitelabelid == whitelabelid).SingleOrDefault();
            if (chkfor != null)
            {
                return RedirectToAction("UPITRANSFER");
            }
            var upistatus = db.Whitelabel_Hidw_Show.Where(x => x.UseFor.ToUpper() == "UPI" && x.whitelabelid == whitelabelid).FirstOrDefault().Status;

            if (upistatus == "Y")
            {

                var token = string.Empty;
                token = getAuthToken();
                if (string.IsNullOrWhiteSpace(token))
                {
                    ViewBag.msg = "Failed at provider server";
                    return RedirectToAction("UPITRANSFER");
                }
                var rem = db.Whitelabel_Retailer_Details.Where(aa => aa.RetailerId == userid).SingleOrDefault();
                var admindetails = db.WhiteLabel_userList.Where(u => u.WhiteLabelID == whitelabelid).SingleOrDefault();
                var txnid = admindetails.websitename.Substring(0, 4);
                var datetxn = DateTime.Now.ToString("dd-MM-yyy hh-mm-ss").Replace("-", "").Replace(" ", "");
                Random random = new Random();
                var ran = random.Next(11, 99);
                var finaltxn = "MNO" + txnid + datetxn + ran;
                var req = new
                {
                    upitxnid = finaltxn,
                    useremail = rem.Email,
                    userrole = "Retailer",
                    username = rem.RetailerName,
                    userfirm = rem.Frm_Name
                };
                var freq = JsonConvert.SerializeObject(req);
                var client = new RestClient("http://api.vastbazaar.com/api/UPI/UPIUSER");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Authorization", "Bearer " + token);
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", freq, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                var resp = response.Content;

                dynamic reschk = JsonConvert.DeserializeObject(resp);
                var sts = reschk.Content.ADDINFO.Status.ToString();
                var statusMessage = reschk.Content.ADDINFO.statusMessage.ToString();

                if (sts.ToUpper() == "SUCCESS")
                {
                    Whitelabel_UPI_Ref_details upi = new Whitelabel_UPI_Ref_details();
                    upi.crdate = DateTime.Now;
                    upi.role = "WhitelabelRetailer";
                    upi.UPITxnid = finaltxn;
                    upi.userid = userid;
                    upi.whitelabelid = whitelabelid;
                    db.Whitelabel_UPI_Ref_details.Add(upi);
                    db.SaveChanges();
                }
                else
                {
                    ViewBag.msg = statusMessage;
                }
                return RedirectToAction("UPITRANSFER");
            }
            else
            {
                return RedirectToAction("UPITRANSFER");
            }
        }

        #endregion


        #region Retailer Gst Invoice Report
        /// <summary>
        /// [GET] Displays GST invoice report for this Retailer
        /// </summary>
        public ActionResult Gst_Invocing_Retailer_report()
        {
            var userid = User.Identity.GetUserId();
            DateTime txt_frm_date = DateTime.Now;
            DateTime to_date = DateTime.Now;
            DateTime date = DateTime.Now;
            string OldMonth = date.AddMonths(-1).ToString("MMMM");
            ViewBag.OldMonth = OldMonth;
            txt_frm_date = new DateTime(date.AddMonths(-1).Year, date.AddMonths(-1).Month, 1);
            to_date = new DateTime(date.Year, date.Month, 1);
            var show = db.GST_Monthly_WRetailer(Convert.ToDateTime(txt_frm_date), Convert.ToDateTime(to_date)).Where(a => a.apiid == userid).ToList();
            return View(show);
        }

        /// <summary>
        /// [GET] Displays account-verified GST invoicing report
        /// </summary>
        public ActionResult Account_verify_Gst_Invocing_report()
        {
            var userid = User.Identity.GetUserId();
            DateTime txt_frm_date = DateTime.Now;
            DateTime to_date = DateTime.Now;
            DateTime date = DateTime.Now;
            string OldMonth = date.AddMonths(-1).ToString("MMMM");

            ViewBag.OldMonth = OldMonth;
            txt_frm_date = new DateTime(date.AddMonths(-1).Year, date.AddMonths(-1).Month, 1);
            to_date = new DateTime(date.Year, date.Month, 1);
            var show = db.GST_Monthly_WRetailer_verify(Convert.ToDateTime(txt_frm_date), Convert.ToDateTime(to_date)).Where(a => a.apiid == userid).ToList();
            return View(show);
        }

        public ActionResult Account_Verify_Gst_Pdf()
        {
            var userid = User.Identity.GetUserId();
            var today = DateTime.Today;
            var month = new DateTime(today.Year, today.Month, 1);
            var first = month.AddMonths(-1);
            var last = month.AddDays(-1);
            ViewBag.last = last.ToShortDateString();
            DateTime from = Convert.ToDateTime(first);
            DateTime to = Convert.ToDateTime(last);
            to = to.AddDays(1);
            ViewBag.month = from.ToString("MMMM") + " " + from.ToString("yyyy");
            DateTime txt_frm_date = DateTime.Now;
            DateTime to_date = DateTime.Now;
            DateTime date = DateTime.Now;
            txt_frm_date = new DateTime(date.AddMonths(-1).Year, date.AddMonths(-1).Month, 1);
            to_date = new DateTime(date.Year, date.Month, 1);

            var entries = db.GST_Monthly_WRetailer_verify(Convert.ToDateTime(txt_frm_date), Convert.ToDateTime(to_date)).Where(a => a.apiid == userid).ToList();
            ViewBag.pancard = entries.SingleOrDefault().PanCard;
            ViewBag.name = entries.SingleOrDefault().RetailerName;
            ViewBag.firmname = entries.SingleOrDefault().Frm_Name;
            ViewBag.address = entries.SingleOrDefault().Address;
            ViewBag.customergst = entries.SingleOrDefault().GST;
            ViewBag.dmtverifycomm = entries.SingleOrDefault().rem_comm;

            ViewBag.statename = entries.SingleOrDefault().State_name;
            var totalcomm = entries.SingleOrDefault().rem_comm;

            var totalgst = entries.SingleOrDefault().rem_gst;

            if (entries.SingleOrDefault().State_name != "Rajasthan")
            {
                ViewBag.igst = 18;
                ViewBag.dmtverifyigst = entries.SingleOrDefault().rem_gst;
                ViewBag.totaligst = totalgst;
            }
            else
            {
                ViewBag.sgst = 9;
                ViewBag.cgst = 9;
                ViewBag.dmtverifycgst = entries.SingleOrDefault().rem_gst / 2;
                ViewBag.dmtverifysgst = entries.SingleOrDefault().rem_gst / 2;
                ViewBag.totalcgst = totalgst / 2;
                ViewBag.totalsgst = totalgst / 2;

            }
            ViewBag.dmtverifytotal = entries.SingleOrDefault().rem_comm + entries.SingleOrDefault().rem_gst;

            ViewBag.totalcomm = totalcomm;

            ViewBag.final = totalcomm + totalgst;
            ViewBag.finalgst = totalgst;

            var fin = totalcomm + totalgst;

            double fin1 = Convert.ToDouble(fin);
            string s = words(fin1, true).Substring(0, 3);
            if (s == "and")
            {
                s = words(fin1, true).Remove(0, 4);
            }
            else
            {
                s = words(fin1, true);
            }
            ViewBag.finalword = s;
            var dealerid = db.Whitelabel_Retailer_Details.Where(a => a.RetailerId == userid).FirstOrDefault().DealerId;
            var whitelabelid = db.whitelabel_Dealer_Details.Where(a => a.DealerId == dealerid).FirstOrDefault().Whitelabelid;
            var admininfo = db.WhiteLabel_userList.Where(a => a.WhiteLabelID == whitelabelid).FirstOrDefault();
            ViewBag.cmpyname = admininfo.FrmName;
            ViewBag.adminaddress = admininfo.Address;
            ViewBag.adminpan = admininfo.PanCardnumber;
            ViewBag.admingst = admininfo.GstNumber;

            return new ViewAsPdf("Account_Verify_Gst_Pdf", entries);

        }

        /// <summary>
        /// [GET] Generates PDF export of the GST invoicing report
        /// </summary>
        public ActionResult GST_Invocing_Report_Pdf()
        {
            var userid = User.Identity.GetUserId();
            var today = DateTime.Today;
            var month = new DateTime(today.Year, today.Month, 1);
            var last = month.AddDays(-1);
            ViewBag.last = last.ToShortDateString();

            DateTime txt_frm_date = DateTime.Now;
            DateTime to_date = DateTime.Now;
            DateTime date = DateTime.Now;
            txt_frm_date = new DateTime(date.AddMonths(-1).Year, date.AddMonths(-1).Month, 1);
            to_date = new DateTime(date.Year, date.Month, 1);

            var entries = db.GST_Monthly_WRetailer(Convert.ToDateTime(txt_frm_date), Convert.ToDateTime(to_date)).Where(a => a.apiid == userid).ToList();
            ViewBag.name = entries.SingleOrDefault().RetailerName;
            ViewBag.address = entries.SingleOrDefault().Address;
            ViewBag.customergst = entries.SingleOrDefault().GST;
            ViewBag.dmtcomm = entries.SingleOrDefault().dmtcomm;
            ViewBag.dmttotal = entries.SingleOrDefault().dmtcomm + entries.SingleOrDefault().dmtgst;
            ViewBag.rechargecomm = entries.SingleOrDefault().rchcomm;
            ViewBag.rechargetotal = entries.SingleOrDefault().rchcomm + entries.SingleOrDefault().rchgst;
            ViewBag.Mposcomm = entries.SingleOrDefault().mposcomm;
            ViewBag.Mpostotal = entries.SingleOrDefault().mposcomm + entries.SingleOrDefault().mposgst;
            ViewBag.Aepscomm = entries.SingleOrDefault().aepscomm;
            ViewBag.Aepstotal = entries.SingleOrDefault().aepscomm + entries.SingleOrDefault().aepsgst;
            ViewBag.Pancomm = entries.SingleOrDefault().pancomm;
            ViewBag.Pantotal = entries.SingleOrDefault().pancomm + entries.SingleOrDefault().pangst;
            ViewBag.Flightcomm = entries.SingleOrDefault().flightcomm;
            ViewBag.Flighttotal = entries.SingleOrDefault().flightcomm + entries.SingleOrDefault().flightgst;
            var final = entries.SingleOrDefault().dmtcomm + entries.SingleOrDefault().rchcomm +
                entries.SingleOrDefault().mposcomm + entries.SingleOrDefault().aepscomm +
                entries.SingleOrDefault().pancomm + entries.SingleOrDefault().flightcomm;
            ViewBag.totaltaxvalue = final;

            var valdmt = entries.SingleOrDefault().dmtgst;
            var valrecharge = entries.SingleOrDefault().rchgst;
            var valmpos = entries.SingleOrDefault().mposgst;
            var valaeps = entries.SingleOrDefault().aepsgst;
            var valpan = entries.SingleOrDefault().pangst;
            var valFlight = entries.SingleOrDefault().flightgst;
            var finalgst = valdmt + valrecharge + valmpos + valaeps + valpan +
                valFlight;

            ViewBag.finaltotal = finalgst + final;
            ViewBag.totalgsttotal = finalgst;
            double fin = Convert.ToDouble(finalgst + final);
            string s = words(fin, true).Substring(0, 3);
            if (s == "and")
            {
                s = words(fin, true).Remove(0, 4);
            }
            else
            {
                s = words(fin, true);
            }
            ViewBag.finalword = s;
            if (entries.SingleOrDefault().State_name != "Rajasthan")
            {
                ViewBag.type = "N";
                ViewBag.igst = 18;
                ViewBag.dmtigst = valdmt;
                ViewBag.rechargeigst = valrecharge;
                ViewBag.mposigst = valmpos;
                ViewBag.Aepsigst = valaeps;
                ViewBag.Panigst = valpan;
                ViewBag.Flightigst = valFlight;
                ViewBag.totaligst = finalgst;
            }
            else
            {
                ViewBag.type = "Y";
                ViewBag.cgst = 9;
                ViewBag.sgst = 9;


                ViewBag.dmtcgst = valdmt / 2;
                ViewBag.dmtsgst = valdmt / 2;
                ViewBag.rechargecgst = valrecharge / 2;
                ViewBag.rechargesgst = valrecharge / 2;
                ViewBag.mposcgst = valmpos / 2;
                ViewBag.mpossgst = valmpos / 2;
                ViewBag.Aepscgst = valaeps / 2;
                ViewBag.Aepssgst = valaeps / 2;
                ViewBag.Pancgst = valpan / 2;
                ViewBag.Pansgst = valpan / 2;
                ViewBag.Flightcgst = valFlight / 2;
                ViewBag.Flightsgst = valFlight / 2;
                ViewBag.totalcgst = finalgst / 2;
                ViewBag.totalsgst = finalgst / 2;
            }

            //ViewBag.particular = "Commission For " + from.ToString("MMMM") + " Month";
            ViewBag.netamount = entries.SingleOrDefault().dmtcomm;
            ViewBag.firmname = entries.SingleOrDefault().Frm_Name;
            ViewBag.total = entries.SingleOrDefault().dmtcomm + entries.SingleOrDefault().dmtgst;
            var dealerid = db.Whitelabel_Retailer_Details.Where(a => a.RetailerId == userid).FirstOrDefault().DealerId;
            var whitelabelid = db.whitelabel_Dealer_Details.Where(a => a.DealerId == dealerid).FirstOrDefault().Whitelabelid;
            var admininfo = db.WhiteLabel_userList.Where(a => a.WhiteLabelID == whitelabelid).FirstOrDefault();
            ViewBag.cmpyname = admininfo.FrmName;
            ViewBag.adminaddress = admininfo.Address;
            ViewBag.adminpan = admininfo.PanCardnumber;
            ViewBag.admingst = admininfo.GstNumber;
            var number = entries.SingleOrDefault().dmtcomm.ToString();
            number = Convert.ToDouble(number).ToString();

            return new ViewAsPdf("GST_Invocing_Report_Pdf", entries);

        }
        public string words(double numbers, Boolean paisaconversion = false)
        {
            var pointindex = numbers.ToString().IndexOf(".");
            int number = Convert.ToInt32(Math.Floor(numbers));
            decimal paisaamt = 0;
            var jj = numbers.ToString().Split('.');
            if (pointindex > 0)
                paisaamt = Convert.ToDecimal(jj[1]);



            if (number == 0) return "Zero";
            if (number == -2147483648) return "Minus Two Hundred and Fourteen Crore Seventy Four Lakh Eighty Three Thousand Six Hundred and Forty Eight";
            int[] num = new int[4];
            int first = 0;
            int u, h, t;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            if (number < 0)
            {
                sb.Append("Minus ");
                number = -number;
            }
            string[] words0 = { "", "One ", "Two ", "Three ", "Four ", "Five ", "Six ", "Seven ", "Eight ", "Nine " };
            string[] words1 = { "Ten ", "Eleven ", "Twelve ", "Thirteen ", "Fourteen ", "Fifteen ", "Sixteen ", "Seventeen ", "Eighteen ", "Nineteen " };
            string[] words2 = { "Twenty ", "Thirty ", "Forty ", "Fifty ", "Sixty ", "Seventy ", "Eighty ", "Ninety " };
            string[] words3 = { "Thousand ", "Lakh ", "Crore " };
            num[0] = number % 1000; // units
            num[1] = number / 1000;
            num[2] = number / 100000;
            num[1] = num[1] - 100 * num[2]; // thousands
            num[3] = number / 10000000; // crores
            num[2] = num[2] - 100 * num[3]; // lakhs
            for (int i = 3; i > 0; i--)
            {
                if (num[i] != 0)
                {
                    first = i;
                    break;
                }
            }
            for (int i = first; i >= 0; i--)
            {
                if (num[i] == 0) continue;
                u = num[i] % 10; // ones
                t = num[i] / 10;
                h = num[i] / 100; // hundreds
                t = t - 10 * h; // tens
                if (h > 0) sb.Append(words0[h] + "Hundred ");
                if (u > 0 || t > 0)
                {
                    if (h > 0 || i == 0) sb.Append("and ");
                    if (t == 0)
                        sb.Append(words0[u]);
                    else if (t == 1)
                        sb.Append(words1[u]);
                    else
                        sb.Append(words2[t - 2] + words0[u]);
                }
                if (i != 0) sb.Append(words3[i - 1]);
            }

            if (paisaamt == 0 && paisaconversion == false)
            {
                sb.Append("ruppes only");
            }
            else if (paisaamt > 0)
            {
                var paisatext = words(Convert.ToDouble(paisaamt), true);
                sb.AppendFormat("rupees {0} paise only", paisatext);
            }
            return sb.ToString().TrimEnd();
        }
        #endregion

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }


        #region FeeCollector
        /// <summary>
        /// [GET] Displays school fee payment interface
        /// </summary>
        public ActionResult SchoolFees()
        {
            var userid = User.Identity.GetUserId();
            ViewBag.schoolname = new SelectList(db.select_schoolname_dll(), "FCId", "SchoolName").ToList();
            var ch = db.getstudentdetails("", "", "RollNo").ToList();
            return View(ch);
        }
        [HttpGet]
        public ActionResult searchstudetdetails(string rollno, string schoolid, string radioval)
        {
            var userid = User.Identity.GetUserId();
            ViewBag.schoolname = new SelectList(db.select_schoolname_dll(), "FCId", "SchoolName").ToList();
            var ch = db.getstudentdetails(rollno, schoolid, radioval).ToList();
            return PartialView("_showstudentdetails", ch);
        }
        [HttpPost]
        public ActionResult Getstudentdetails(string rollno, string schoolname)
        {
            var userid = schoolname;
            ViewBag.schoolname = new SelectList(db.select_schoolname_dll(), "FCId", "SchoolName").ToList();
            if (!string.IsNullOrEmpty(rollno))
            {
                var respo = db.proc_GetFeeInstallmentToDeposit(rollno, userid).SingleOrDefault();
                if (respo != null && respo.InstallmentAmountFee != null)
                {
                    var student = db.Students.Single(a => a.RollNo.ToLower().Contains(rollno.ToString()));
                    var response = new
                    {
                        Status = "Success",
                        Message = new
                        {
                            NumberOfInstallments = respo.InstallmentCount,
                            PaidInstallment = respo.PaidIntallmentCount,
                            Amount = respo.InstallmentAmountFee,
                            AmountBus = respo.InstallmentAmountBusFee,
                            FixedCharge = respo.FixedCharges,
                            Name = student.Name,
                            FName = student.FName,
                            Rollno = student.RollNo,
                            stdClass = student.stdClass,
                            Section = student.Section,
                            Session = student.AcadmicSession,
                            Address = student.stdAddress,
                            ActualFee = respo.ActualFee,
                            ActualBusFee = respo.ActualBusFee,
                            DiscountedFee = respo.DiscountedFee,
                            DiscountedBusFee = respo.DiscountedBusFee
                        }
                    };
                    var output = new JavaScriptSerializer().Serialize(response);
                    return Json(output);
                }
                else
                {
                    var response = new { Status = "Failed", Message = "Invalid or Empty Roll Number" };
                    var output = new JavaScriptSerializer().Serialize(response);
                    return Json(output);
                }

            }
            else
            {
                var response = new { Status = "Failed", Message = "Invalid or Empty Roll Number" };
                var output = new JavaScriptSerializer().Serialize(response);
                return Json(output);
            }


            //return Json();
        }
        [HttpPost]
        public ActionResult InsertFeeDeposit(string txtRollNo, decimal InsAmount, decimal InsBusAmount)
        {
            var userid = User.Identity.GetUserId();
            System.Data.Entity.Core.Objects.ObjectParameter output = new System.Data.Entity.Core.Objects.ObjectParameter("response", typeof(string));
            var response = db.proc_DepositFeeInstallmentFromRetailer(txtRollNo, userid, InsAmount, InsBusAmount, output).SingleOrDefault().msg;
            if (response.Contains("Success"))
            {
                TempData["msg"] = "Payment successfully completed.";
                TempData["sts"] = "success";
            }
            else
            {
                TempData["msg"] = response;
                TempData["sts"] = "danger";
            }
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult FeeDeposit()
        {
            var userid = User.Identity.GetUserId();
            string txt_frm_date = DateTime.Now.ToString();
            string txt_to_date = DateTime.Now.ToString();
            string frm_date = Convert.ToDateTime(txt_frm_date).Date.ToString("yyyy-MM-dd");
            string to_date = Convert.ToDateTime(txt_to_date).AddDays(1).ToString("yyyy-MM-dd");
            FeeDepositModel model = new FeeDepositModel();
            model.lstFeedeposit = db.proc_getFeedepositDetails(null, userid, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            return View(model);
        }
        [HttpPost]
        public ActionResult FeeDeposit(string stdRollNO, string txt_frm_date, string txt_to_date)
        {

            ViewBag.fromdate = txt_frm_date;
            ViewBag.todate = txt_to_date;
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

            FeeDepositModel model = new FeeDepositModel();
            model.lstFeedeposit = db.proc_getFeedepositDetails(stdRollNO, userid, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            return View(model);
        }
        #endregion

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
        private static Random random = new Random();
        public string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public static string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
        public void insertGeoLocation(string userid, out string lat, out string longitude)
        {
            lat = string.Empty;
            longitude = string.Empty;
            var client = new RestClient("http://ipinfo.io");
            var request = new RestRequest(Method.GET);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("content-type", "text/plain");
            request.AddHeader("cache-control", "no-cache");
            IRestResponse response = client.Execute(request);

            dynamic respoJson = JsonConvert.DeserializeObject(response.Content);
            //dynamic respoJson = JsonConvert.DeserializeObject("{\"Version\":\"1.0\",\"StatusCode\":200,\"Content\":{\"ResponseCode\":0,\"ADDINFO\":{\"status\":true,\"message\":\"Request Completed\",\"data\":{\"terminalId\":\"FA026069\",\"requestTransactionTime\":\"26\/09\/2018 10:26:58\",\"transactionAmount\":0.0,\"transactionStatus\":\"successful\",\"balanceAmount\":4408.83,\"bankRRN\":\"826910115647\",\"transactionType\":\"BE\",\"fpTransactionId\":\"826910115647\"},\"statusCode\":10000}}}");
            if (response.StatusCode == HttpStatusCode.OK && !string.IsNullOrWhiteSpace(response.Content))
            {
                string loc = Convert.ToString(respoJson.loc);
                string[] latlong = loc.Split(new char[] { ',' });
                Whitelabel_UserLocation entry = new Whitelabel_UserLocation();
                entry.RetailerId = userid;
                entry.Address = "";
                entry.City = respoJson.city;
                entry.country = respoJson.country;
                entry.IP = respoJson.ip;
                entry.Lattitude = latlong[0];
                entry.Longitute = latlong[1];
                entry.postal = respoJson.postal;
                entry.CreatedOn = DateTime.Now;
                entry.UpdatedOn = DateTime.Now;
                db.Whitelabel_UserLocation.Add(entry);
                db.SaveChanges();
                lat = latlong[0];
                longitude = latlong[1];

            }
        }

        public void TryLogin()
        {
            var response = tokencheck();
            var responsechk = response.Content.ToString();
            var responsecode = response.StatusCode.ToString();
            if (responsecode == "OK")
            {
                var tokn = db.vastbazzartokens.SingleOrDefault();
                dynamic json = JsonConvert.DeserializeObject(responsechk);
                var token = json.access_token.ToString();
                var expire = json[".expires"].ToString();
                DateTime exp = Convert.ToDateTime(expire);
                tokn.apitoken = token;
                tokn.exptime = exp;
                db.SaveChanges();

            }
        }

        #region WalletToBankAmountTransfer
        /// <summary>
        /// [GET] Displays wallet-to-bank transfer request page
        /// </summary>
        public ActionResult WalletToBankAmountTransfer()
        {
            var entries = db.Whitelabel_WalletToBankAmountTransferCharge.Where(a => a.UserRole == "Retailer").ToList();
            return View(entries);
        }
        [HttpGet]

        /// <summary>
        /// [GET] Displays wallet unload transactions
        /// </summary>
        public ActionResult WalletUnloadReport()
        {
            var userid = User.Identity.GetUserId();
            DateTime todayStart = DateTime.Now.Date;
            DateTime todayEnd = DateTime.Now.AddDays(1).Date;
            modelviewwalletunload model = new modelviewwalletunload();
            model.WRequests = db.Whitelabel_WalletToBankAmountTransferRequests.Where(a => a.RetailerId == userid && a.RequestDate >= todayStart).OrderByDescending(aa => aa.RequestDate).ToList();
            model.WCharges = db.Whitelabel_WalletToBankAmountTransferCharge.Where(a => a.UserRole == "Retailer").ToList();
            ViewBag.WalletUnloadlimits = db.whitelabel_Aeps_MPos_unload.Where(x => x.userid == userid).OrderByDescending(x => x.Idno).FirstOrDefault();

            return View(model);
        }


        [HttpPost]
        /// <summary>
        /// [POST] Filters wallet unload report by date
        /// </summary>
        public ActionResult WalletUnloadReport(string txt_frm_date, string txt_to_date)
        {
            modelviewwalletunload model = new modelviewwalletunload();
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
            model.WRequests = db.Whitelabel_WalletToBankAmountTransferRequests.Where(a => a.RetailerId == userid && a.RequestDate >= frm_date && a.RequestDate <= to_date).OrderByDescending(aa => aa.RequestDate).ToList();
            model.WCharges = db.Whitelabel_WalletToBankAmountTransferCharge.Where(a => a.UserRole == "Retailer").ToList();
            ViewBag.WalletUnloadlimits = db.whitelabel_Aeps_MPos_unload.Where(x => x.userid == userid).OrderByDescending(x => x.Idno).FirstOrDefault();

            return View(model);
        }
        [HttpPost]
        public ActionResult AddWalletToBankRequest(decimal Amount, string Type)
        {
            var userid = User.Identity.GetUserId();
            if (Amount > 0 && !string.IsNullOrWhiteSpace(Type))
            {
                var retailerDetails = db.Whitelabel_Retailer_Details.Where(a => a.RetailerId == userid).FirstOrDefault();
                if (retailerDetails != null)
                {
                    if (retailerDetails.PSAStatus == "Y" && retailerDetails.AadhaarStatus == "Y" && !string.IsNullOrEmpty(retailerDetails.pancardPath) && !string.IsNullOrEmpty(retailerDetails.aadharcardPath))
                    {
                        int after = 0;
                        try
                        {
                            var checktime = db.Whitelabel_WalletToBankAmountTransferRequests.Where(x => x.RetailerId == userid && x.RequestDate <= DateTime.Now).OrderByDescending(x => x.RequestDate).Select(x => x.RequestDate).FirstOrDefault();
                            TimeSpan ts = (TimeSpan)(DateTime.Now - checktime);

                            if (ts.TotalMinutes < 1)
                            {
                                after = 0;
                            }
                            else
                            {
                                after = 1;
                            }
                        }
                        catch (Exception ex)
                        {
                            after = 1;
                        }
                        if (after == 1)
                        {

                            string str = string.Empty;
                            bool IsUpdateRequired = false;
                            if (retailerDetails.Bankaccountno == null || string.IsNullOrWhiteSpace(retailerDetails.Bankaccountno))
                            {
                                str = str + "BankAccount No,";
                                IsUpdateRequired = true;
                            }
                            if (retailerDetails.bankname == null || string.IsNullOrWhiteSpace(retailerDetails.bankname))
                            {
                                str = str + "BankName,";
                                IsUpdateRequired = true;
                            }
                            if (retailerDetails.Ifsccode == null || string.IsNullOrWhiteSpace(retailerDetails.Ifsccode))
                            {
                                str = str + "IFSC Code,";
                                IsUpdateRequired = true;
                            }
                            if (retailerDetails.accountholder == null || string.IsNullOrWhiteSpace(retailerDetails.accountholder))
                            {
                                str = str + "Account Holder Name,";
                                IsUpdateRequired = true;
                            }
                            if (IsUpdateRequired)
                            {
                                if (str.EndsWith(","))
                                    str = str.Substring(0, str.Length - 1);
                                str = str + " are required to become WalletUnload  Request.";
                                var ViewResponse = new { status = "Failed", Message = str };
                                return Json(JsonConvert.SerializeObject(ViewResponse));
                            }
                            else
                            {
                                Areas.RETAILER.Models.Vastbillpay vb = new Areas.RETAILER.Models.Vastbillpay();
                                string ProjectName = Path.GetDirectoryName(Path.GetDirectoryName(Server.MapPath(@"~/HomeControllers.cs")));
                                ProjectName = ProjectName.Substring(ProjectName.LastIndexOf("\\") + 1);
                                var RequestId = DateTime.Now.ToString("dd-MM-yyy hh-mm-ss").Replace("-", "").Replace(" ", "") + ProjectName.Substring(0, 4);
                                var Email = retailerDetails.Email;
                                var AccountNumber = retailerDetails.Bankaccountno;
                                var BankName = retailerDetails.bankname;
                                var IFSCCode = retailerDetails.Ifsccode;
                                var AccountholderName = retailerDetails.accountholder;
                                var Sts = db.walletunloadsts.FirstOrDefault();
                                System.Data.Entity.Core.Objects.ObjectParameter status = new System.Data.Entity.Core.Objects.ObjectParameter("Status", typeof(string));
                                System.Data.Entity.Core.Objects.ObjectParameter Message = new System.Data.Entity.Core.Objects.ObjectParameter("Message", typeof(string));
                                var response = db.spWhitelabel_InsertWalletToBankAmountTransfer(userid, Amount, Type, RequestId, status, Message).FirstOrDefault();
                                if (response.Status == "Success")
                                {
                                    var tokn = Responsetoken.gettoken();
                                    var responsechk1 = vb.WalletUnloadrRquest(tokn, Email, Amount, "", AccountNumber, BankName, IFSCCode, AccountholderName, RequestId, Type);
                                    var responsechk = responsechk1.Content.ToString();
                                    dynamic json = JsonConvert.DeserializeObject(responsechk);
                                    var respcode = json.Content.ResponseCode.ToString();
                                    var ADDINFO = json.Content.ADDINFO.ToString();
                                    dynamic json1 = JsonConvert.DeserializeObject(ADDINFO);
                                    var status1 = json1.Status.ToString();
                                    var msg = json1.Message.ToString();
                                    if (status1 == "Success")
                                    {
                                        var ViewResponse = new { status = "Success", Message = "Request proceeded successfully." };
                                        return Json(JsonConvert.SerializeObject(ViewResponse));
                                    }
                                    else if (status1 == "Failed")
                                    {
                                        var pp = db.Whitelabel_WalletToBankAmountTransferRequests.Where(a => a.Status == "Pending" && a.RequestId == RequestId).FirstOrDefault();
                                        if (pp != null)
                                        {
                                            var response1 = db.spWhitelabel_InsertWalletToBankAmountTransferRefund(pp.Idno, status, Message).FirstOrDefault();
                                            if (response1.Status == "Success")
                                            {
                                                var ViewResponse = new { status = "Success", Message = "Rejected successfully." };
                                                return Json(JsonConvert.SerializeObject(ViewResponse));
                                            }
                                            else
                                            {
                                                var ViewResponse = new { status = "Failed", Message = response.Message };
                                                return Json(JsonConvert.SerializeObject(ViewResponse));
                                            }
                                        }
                                        else
                                        {
                                            var ViewResponse = new { status = "Success", Message = "Rejected successfully." };
                                            return Json(JsonConvert.SerializeObject(ViewResponse));
                                        }
                                    }
                                    else
                                    {
                                        var ViewResponse = new { status = "Failed", Message = "something went wrong." };
                                        return Json(JsonConvert.SerializeObject(ViewResponse));
                                    }
                                }
                                else
                                {
                                    var ViewResponse = new { status = "Failed", Message = response.Message };
                                    return Json(JsonConvert.SerializeObject(ViewResponse));
                                }
                            }
                        }
                        else
                        {
                            var ViewResponse = new { status = "Failed", Message = "Try Again After 1 miniute." };
                            return Json(JsonConvert.SerializeObject(ViewResponse));
                        }
                    }
                    else
                    {
                        var ViewResponse = new { status = "Profile", Message = "" };
                        return Json(JsonConvert.SerializeObject(ViewResponse));
                    }
                }

                return View();
            }
            else
            {
                var ViewResponse = new { status = "Failed", Message = "Amount and transfer mode is mandatory" };
                return Json(JsonConvert.SerializeObject(ViewResponse));
            }

        }
        #endregion


        #region DMT View Status
        public ActionResult viewsearch(string idno)
        {
            var sts = "";
            //var resp = db.IMPS_transtion_detsils.Where(aa => aa.trans_common_id == idno).SingleOrDefault().Response_output;
            //dynamic jsonchk = JsonConvert.DeserializeObject(resp);
            //var tid = jsonchk.Content.ADDINFO.data.Tid.ToString();
            var tokn = Responsetoken.gettoken();
            VastBazaar cb1 = new VastBazaar();
            var respo = cb1.EKOStatus(idno, tokn);
            var content = respo.Content;
            dynamic jsonchk1 = JsonConvert.DeserializeObject(content);
            var txtrep = jsonchk1.Content.ADDINFO.response_status_id.ToString();
            if (txtrep == "0")
            {
                var txtsts = jsonchk1.Content.ADDINFO.data.tx_status.ToString();
                if (txtsts == "0")
                {
                    sts = "Success";

                    db.Money_transfer_update_new_new(idno, "SUCCESS", "", "", "", "", 0, 0);
                }
                else if (txtsts == "1")
                {
                    sts = "Failed";
                    db.Money_transfer_update_new_new(idno, "FAILED", "", "", "", "", 0, 0);
                }
                else if (txtsts == "2")
                {
                    sts = "Pending";

                }
                else if (txtsts == "3")
                {
                    sts = "RefundPending";
                }
                else if (txtsts == "4")
                {
                    sts = "Refunded";
                    db.Money_transfer_update_new_new(idno, "FAILED", "", "", "", "", 0, 0);
                }
                var stschk = sts + "~" + idno;

                return Json(stschk, JsonRequestBehavior.AllowGet);
            }
            else
            {
                sts = "Pending";
                var stschk = sts + "~" + idno;

                return Json(stschk, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult viewsearchold(string idno)
        {
            var sts = "";
            //var resp = db.IMPS_transtion_detsils.Where(aa => aa.trans_common_id == idno).SingleOrDefault().Response_output;
            //dynamic jsonchk = JsonConvert.DeserializeObject(resp);
            //var tid = jsonchk.Content.ADDINFO.data.Tid.ToString();
            var tokn = Responsetoken.gettoken();
            VastBazaar cb1 = new VastBazaar();
            var respo = cb1.EKOStatus(idno, tokn);
            var content = respo.Content;
            dynamic jsonchk1 = JsonConvert.DeserializeObject(content);
            var txtrep = jsonchk1.Content.ADDINFO.response_status_id.ToString();
            if (txtrep == "0")
            {
                var txtsts = jsonchk1.Content.ADDINFO.data.tx_status.ToString();
                if (txtsts == "0")
                {
                    sts = "Success";

                    db.Money_transfer_update_new_new_old(idno, "SUCCESS", "", "", "", "", 0, 0);
                }
                else if (txtsts == "1")
                {
                    sts = "Failed";
                    db.Money_transfer_update_new_new_old(idno, "FAILED", "", "", "", "", 0, 0);
                }
                else if (txtsts == "2")
                {
                    sts = "Pending";

                }
                else if (txtsts == "3")
                {
                    sts = "RefundPending";
                }
                else if (txtsts == "4")
                {
                    sts = "Refunded";
                    db.Money_transfer_update_new_new_old(idno, "FAILED", "", "", "", "", 0, 0);
                }
                var stschk = sts + "~" + idno;

                return Json(stschk, JsonRequestBehavior.AllowGet);
            }
            else
            {
                sts = "Pending";
                var stschk = sts + "~" + idno;

                return Json(stschk, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult viewsearchDMT1(string idno)
        {
            var sts = "";
            //var resp = db.IMPS_transtion_detsils.Where(aa => aa.trans_common_id == idno).SingleOrDefault().Response_output;
            //dynamic jsonchk = JsonConvert.DeserializeObject(resp);
            //var tid = jsonchk.Content.ADDINFO.data.Tid.ToString();
            var tokn = Responsetoken.gettoken();
            VastBazaar cb1 = new VastBazaar();
            var respo = cb1.InstaStatus(idno, tokn);
            var content = respo.Content;
            dynamic jsonchk1 = JsonConvert.DeserializeObject(content);

            var txtsts = jsonchk1.Content.ADDINFO.statuscode.ToString();
            if (txtsts == "TXN")
            {
                sts = "Success";
                var bankid = jsonchk1.Content.ADDINFO.data.opr_id.ToString();
                var name = jsonchk1.Content.ADDINFO.data.beneficiary_name.ToString();
                db.Money_transfer_update_new_new(idno, "SUCCESS", bankid, name, "", "", 0, 0);
            }
            else if (txtsts == "TUP")
            {
                sts = "Pending";
            }
            else if (txtsts == "ERR")
            {
                sts = "Pending";
            }
            else
            {
                sts = "Failed";
                var bankid = jsonchk1.Content.ADDINFO.status.ToString();
                db.Money_transfer_update_new_new(idno, "FAILED", bankid, "", "", "", 0, 0);
            }

            var stschk = sts + "~" + idno;

            return Json(stschk, JsonRequestBehavior.AllowGet);

        }
        public ActionResult viewsearcholdDMT1(string idno)
        {
            var sts = "";
            //var resp = db.IMPS_transtion_detsils.Where(aa => aa.trans_common_id == idno).SingleOrDefault().Response_output;
            //dynamic jsonchk = JsonConvert.DeserializeObject(resp);
            //var tid = jsonchk.Content.ADDINFO.data.Tid.ToString();
            var tokn = Responsetoken.gettoken();
            VastBazaar cb1 = new VastBazaar();
            var respo = cb1.InstaStatus(idno, tokn);
            var content = respo.Content;
            dynamic jsonchk1 = JsonConvert.DeserializeObject(content);

            var txtsts = jsonchk1.Content.ADDINFO.statuscode.ToString();
            if (txtsts == "TXN")
            {
                sts = "Success";
                var bankid = jsonchk1.Content.ADDINFO.data.opr_id.ToString();
                var name = jsonchk1.Content.ADDINFO.data.beneficiary_name.ToString();
                db.Money_transfer_update_new_new_old(idno, "SUCCESS", bankid, name, "", "", 0, 0);
            }
            else if (txtsts == "TUP")
            {
                sts = "Pending";
            }
            else if (txtsts == "ERR")
            {
                sts = "Pending";
            }
            else
            {
                sts = "Failed";
                db.Money_transfer_update_new_new_old(idno, "FAILED", "", "", "", "", 0, 0);
            }

            var stschk = sts + "~" + idno;

            return Json(stschk, JsonRequestBehavior.AllowGet);

        }
        public ActionResult Refund(string tid, string otp)
        {
            var tokn = Responsetoken.gettoken();
            VastBazaar cb1 = new VastBazaar();
            var respo = cb1.EKORefund(tid, otp, tokn);
            var content = respo.Content;
            dynamic jsonchk1 = JsonConvert.DeserializeObject(content);

            var message = jsonchk1.Content.ADDINFO.message.ToString();
            var sts = jsonchk1.Content.ADDINFO.response_status_id.ToString();
            if (sts == "0")
            {
                db.Money_transfer_update_new_new(tid, "FAILED", "", "", "", "", 0, 0);
            }
            return Json(message, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Refundold(string tid, string otp)
        {
            var tokn = Responsetoken.gettoken();
            VastBazaar cb1 = new VastBazaar();
            var respo = cb1.EKORefund(tid, otp, tokn);
            var content = respo.Content;
            dynamic jsonchk1 = JsonConvert.DeserializeObject(content);

            var message = jsonchk1.Content.ADDINFO.message.ToString();
            var sts = jsonchk1.Content.ADDINFO.response_status_id.ToString();
            if (sts == "0")
            {
                db.Money_transfer_update_new_new_old(tid, "FAILED", "", "", "", "", 0, 0);
            }
            return Json(message, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ResendDMT(string tid)
        {
            var tokn = Responsetoken.gettoken();
            VastBazaar cb1 = new VastBazaar();
            var respo = cb1.EKORefundResend(tid, tokn);
            var content = respo.Content;
            dynamic jsonchk1 = JsonConvert.DeserializeObject(content);
            var respsts = jsonchk1.Content.ADDINFO.response_status_id;
            var message = "";
            if (respsts == "-1")
            {
                var otp = jsonchk1.Content.ADDINFO.data.otp;
                var msg = jsonchk1.Content.ADDINFO.message;
                message = msg;
            }
            else
            {
                message = jsonchk1.Content.ADDINFO.message;
            }
            return Json(message, JsonRequestBehavior.AllowGet);
        }
        #endregion

        private string GetToken()
        {
            try
            {
                var userid = User.Identity.GetUserId();

                var token_tbl = db.TokenGenWApis.Where(a => a.UserId == userid).SingleOrDefault();
                string tokenCHK = String.Empty;

                if (token_tbl != null)
                {
                    tokenCHK = token_tbl.Token;
                }

                if (String.IsNullOrEmpty(tokenCHK))
                {
                    TempData.Remove("data");
                    AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                    return null;
                }
                else
                {
                    return tokenCHK;
                }
            }
            catch
            {
                TempData.Remove("data");
                AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return null;
            }

        }
        public static void WriteLog(string strMessage)
        {
            try
            {
                StreamWriter log;
                FileStream fileStream = null;
                DirectoryInfo logDirInfo = null;
                FileInfo logFileInfo;

                string logFilePath = "C:\\Logs\\";
                logFilePath = logFilePath + "AshaaWRTokenGenerateForWApi" + System.DateTime.Today.ToString("MM-dd-yyyy") + "." + "txt";

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