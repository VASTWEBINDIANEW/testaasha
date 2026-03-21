using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
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
using Vastwebmulti.Areas.ADMIN.Controllers;
using Vastwebmulti.Areas.ADMIN.Models;
using Vastwebmulti.Areas.ADMIN.ViewModel;
using Vastwebmulti.Areas.MASTER.Models;
using Vastwebmulti.Areas.MASTER.ViewModel;
using Vastwebmulti.Areas.RETAILER.Models;
using Vastwebmulti.Hubs;
using Vastwebmulti.Models;


namespace Vastwebmulti.Areas.MASTER.Controllers
{
    [Authorize(Roles = "master")]
    [CutomAttributforpasscodeset()]
    [Low_Bal_CustomFilter()]
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
        // GET: MASTER/Home
        private VastwebmultiEntities _db;
        public HomeController()
        {
            _db = new VastwebmultiEntities();
        }
        AppNotification notify = new AppNotification();
        ALLSMSSend smssend = new ALLSMSSend();
        VastwebmultiEntities db = new VastwebmultiEntities();
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

            db.Configuration.ProxyCreationEnabled = false;
            db.Notifications.Add(objNotif);
            db.SaveChanges();

            objNotifHub.SendNotification(objNotif.SentTo);
        }
        #endregion
        //Dashboard 
        public ActionResult Dashboard()
        {
            try
            {
                CallAutofundtransfer();
            }
            catch { }
            var userid = User.Identity.GetUserId();
            var vv = _db.Superstokist_details.SingleOrDefault(a => a.SSId == userid);
            //ViewBag.email = vv.Email;
            ViewBag.image = vv.Photo;
            //show News for master
            var news = (from pp in _db.Message_top where (pp.users == "Master" || pp.users == "All") where pp.status == "Y" select pp).ToList();
            ViewBag.news = news;
            //if (news.Any())
            //{
            //    ViewBag.news = news.FirstOrDefault().message;
            //    ViewBag.newimg = news.FirstOrDefault().image;
            //}
            //else
            //{
            //    ViewBag.news = null;
            //    ViewBag.newimg = null;
            //}
            //Upcomming Holiday
            List<show_upcomming_holiday_Result> li = new List<show_upcomming_holiday_Result>();
            ViewBag.showholiday = li;

            var dealer = db.Dealer_Details.Where(a => a.SSId == userid).ToList();
            var retailer = db.Retailer_Details.Where(a => a.ISDeleteuser == false).ToList();
            var retailerchk = (from tbl in dealer
                               join tbl1 in retailer on tbl.DealerId equals tbl1.DealerId
                               select tbl1).ToList();

            var ledger = db.DashBoard_Report_top(0).ToList();
            var recent = (from tbl in ledger
                          join tbl1 in retailerchk on tbl.UserId equals tbl1.RetailerId
                          select tbl);

            DateTime todaysDate = DateTime.Now.Date;
            int month = todaysDate.Month;
            int year = todaysDate.Year;

            int Nxtmonth = todaysDate.AddMonths(1).Month;
            int Nxtyear = todaysDate.AddMonths(1).Year;

            TargetSetviewmodel vmodel = new TargetSetviewmodel();
            var allOn = db.tblsuperstockistsettargets.Where(a => a.Status == true).ToList();
            vmodel.mdTargetCategory = allOn.Where(a => a.Date.Value.Month == month && a.Date.Value.Year == year).ToList();
            vmodel.mdTargetCategoryNxt = allOn.Where(a => a.Date.Value.Month == Nxtmonth && a.Date.Value.Year == Nxtyear).ToList();
            vmodel.productItems = db.tblPruductGifts.ToList();
            vmodel.DashBoard_Report_top = recent;
            vmodel.CategoryImages = Directory.EnumerateFiles(Server.MapPath("~/CategoryImages")).Select(fn => Path.GetFileNameWithoutExtension(fn));
            return View(vmodel);
        }

        public void CallAutofundtransfer()
        {
            #region Auto credit transfer start

            var userid = User.Identity.GetUserId();

            string transferids = GenerateUniqueTransectionID();
            var transferid = "AUTO" + transferids;
            var remdetailsallinform = db.Superstokist_details.Where(x => x.SSId == userid).SingleOrDefault();
            var reaminbalance_master = db.Remain_superstokist_balance.Where(x => x.SuperStokistID == userid).SingleOrDefault();
            var adminid = db.Admin_details.SingleOrDefault().userid;
            int noofcount = 0;
            if (reaminbalance_master != null)
            {

                var automainmiumbal = db.autofundtransfer_admnin_to_master.Where(x => x.masterid == userid && (x.types.ToUpper() == "CASH" || x.types.ToUpper() == "CREDIT") && x.status == "Y").SingleOrDefault();

                DateTime fromdate = DateTime.Now;
                DateTime todate = DateTime.Now.AddDays(1);
                if (automainmiumbal != null)
                {
                    DateTime todaydate = DateTime.Now;
                    int transfer1min = 0;
                    try
                    {

                        var check = db.admin_to_super_balance.Where(x => x.SuperStokistID == userid && x.Head == "AutoFund").ToList();
                        noofcount = check.Where(x => x.RechargeDate.Value.Date == todaydate.Date).Count();
                        var sdd = db.admin_to_super_balance.Where(x => x.SuperStokistID == userid && x.Head == "AutoFund").OrderByDescending(x => x.ID).FirstOrDefault();
                        if (sdd != null)
                        {
                            try
                            {
                                var resultsss = (int)sdd.RechargeDate.Value.Subtract(todaydate).TotalMinutes;
                                if (resultsss < -1)
                                {

                                }
                                else
                                {
                                    return;
                                }
                            }
                            catch { }
                        }
                        // noofcount = db.admin_to_super_balance.Where(x => x.SuperStokistID == userid && x.RechargeDate.Value.ToString("dd/MM/yyyy") == todaydate.ToString("dd/MM/yyyy") && x.Head== "AutoFund").ToList().Count();
                    }
                    catch
                    {
                        noofcount = 0;
                    }
                    //db.AutoCreditTransfers.Where(x => x.remid == userid && (x.transfertype == "Cash" || x.transfertype == "Credit") && x.remcurrentstatus == "Y" && x.transferdate != null).FirstOrDefault();

                    if (noofcount < automainmiumbal.totaltransfer)
                    {

                        if (automainmiumbal != null)
                        {
                            if (automainmiumbal.minamount > reaminbalance_master.Remainamount)
                            {

                                System.Data.Entity.Core.Objects.ObjectParameter output = new
                                     System.Data.Entity.Core.Objects.ObjectParameter("Output", typeof(string));
                                var msg = db.Auto_Fundtransfer_Admin_to_master(userid, transferid, output).Single().msg;
                                try
                                {
                                    var masterdetails = db.Superstokist_details.Where(aa => aa.SSId == userid).SingleOrDefault();

                                   var Masterdetails = db.Remain_superstokist_balance.Where(aa => aa.SuperStokistID == userid).SingleOrDefault();

                                    var admininfo = db.Admin_details.SingleOrDefault();
                                    Backupinfo back = new Backupinfo();
                                 

                                    var model2 = new Backupinfo.Addinfo
                                    {
                                        Websitename = admininfo.WebsiteUrl,
                                        RetailerID = userid,
                                        Email = masterdetails.Email,
                                        Mobile = masterdetails.Mobile,
                                        Details = "Fund Recived From Admin ",
                                        RemainBalance = Convert.ToDecimal(Masterdetails.Remainamount),
                                        Usertype = "Master"
                                    };
                                    back.info(model2);
                                }
                                catch { }
                                if (msg == "Success")
                                {
                                    var TotalAmount = reaminbalance_master.Remainamount + automainmiumbal.Transferamount;
                                    var statusSendSmsRetailerfundtransfer = db.SMSSendAlls.Where(a => a.ServiceName == "Admintomasterfundtrans").SingleOrDefault();
                                    var statusSendEmailRetailerfundtransfer = db.EmailSendAlls.Where(a => a.ServiceName == "Admintomasterfundtrans1").SingleOrDefault().Status;
                                    var diff1 = (db.admin_to_super_balance.Where(aa => aa.SuperStokistID == userid && aa.balance_from == adminid).OrderByDescending(aa => aa.ID).Select(c => c.cr).FirstOrDefault());
                                    if (automainmiumbal.types == "CREDIT")
                                    {
                                        //if (statusSendSmsRetailerfundtransfer == "Y")
                                        //{
                                        //    string msgssss = "";
                                        //    string tempid = "";
                                        //    string urlss = "";

                                        //    var smsapionsts = db.apisms.Where(x => x.sts == "Y").SingleOrDefault();
                                        //    var smsstypes = db.Sending_SMS_Templates.Where(x => x.SMS_TYPE == "FUNDTRANSFERMESSAGE_CREDIT" && x.SMSAPIID == smsapionsts.id).SingleOrDefault();
                                        //    if (smsstypes != null)
                                        //    {
                                        //        msgssss = string.Format(smsstypes.Templates, automainmiumbal.Transferamount, TotalAmount, diff1);
                                        //        tempid = smsstypes.Templateid;
                                        //        urlss = smsapionsts.smsapi;

                                        //        smssend.sendsmsallnew(remdetailsallinform.Mobile, msgssss, urlss, tempid);
                                        //    }
                                        //    //  smssend.sendsmsall(RetailerDetails.Mobile, "Credit Received Rs." + amount1 + ".New Balance is " + TotalAmount + ".Your O/s Credit is " + diff1 + "", "Fund Transfer");
                                        //}

                                        smssend.sms_init(statusSendSmsRetailerfundtransfer.Status, statusSendSmsRetailerfundtransfer.Whatsapp_Status, "FUNDTRANSFERMESSAGE_CREDIT", remdetailsallinform.Mobile, automainmiumbal.Transferamount, TotalAmount, diff1);

                                        if (statusSendEmailRetailerfundtransfer == "Y")
                                        {
                                            smssend.SendEmailAll(remdetailsallinform.Email, "Credit Received Rs." + automainmiumbal.Transferamount + ".New Balance is " + TotalAmount + ".Your O/s Credit is " + diff1 + "", "Fund Transfer", db.Admin_details.SingleOrDefault().email, 1000);
                                        }

                                    }
                                    else if (automainmiumbal.types == "Cash")
                                    {
                                        //if (statusSendSmsRetailerfundtransfer == "Y")
                                        //{
                                        //    string msgssss = "";
                                        //    string tempid = "";
                                        //    string urlss = "";

                                        //    var smsapionsts = db.apisms.Where(x => x.sts == "Y").SingleOrDefault();
                                        //    var smsstypes = db.Sending_SMS_Templates.Where(x => x.SMS_TYPE == "FUNDTRANSFERMESSAGE_CREDIT" && x.SMSAPIID == smsapionsts.id).SingleOrDefault();
                                        //    if (smsstypes != null)
                                        //    {
                                        //        msgssss = string.Format(smsstypes.Templates, automainmiumbal.Transferamount, TotalAmount, diff1);
                                        //        tempid = smsstypes.Templateid;
                                        //        urlss = smsapionsts.smsapi;

                                        //        smssend.sendsmsallnew(remdetailsallinform.Mobile, msgssss, urlss, tempid);
                                        //    }
                                        //    //  smssend.sendsmsall(RetailerDetails.Mobile, "Credit Received Rs." + amount1 + ".New Balance is " + TotalAmount + ".Your O/s Credit is " + diff1 + "", "Fund Transfer");
                                        //}

                                        smssend.sms_init(statusSendSmsRetailerfundtransfer.Status, statusSendSmsRetailerfundtransfer.Whatsapp_Status, "FUNDTRANSFERMESSAGE_CREDIT", remdetailsallinform.Mobile, automainmiumbal.Transferamount, TotalAmount, diff1);

                                        if (statusSendEmailRetailerfundtransfer == "Y")
                                        {
                                            smssend.SendEmailAll(remdetailsallinform.Email, "Cash Received Rs." + automainmiumbal.Transferamount + ".New Balance is " + TotalAmount + ".Your O/s Credit is " + diff1 + "", "Fund Transfer", db.Admin_details.SingleOrDefault().email, 1000);
                                        }

                                    }

                                }
                            }

                        }
                    }
                }

            }


            #endregion
        }



        #region mdmasterdistributor

        public async Task<ActionResult> AutoFundTransfer_Master_To_Distributor()
        {
            // IEnumerable<Autofundtransfermodel> modes;
            Autofundtransfermodelviewmodel modes = new Autofundtransfermodelviewmodel();
            MethodeClsForAuto autofund = new MethodeClsForAuto();
            var adminuserid = User.Identity.GetUserId();
            await autofund.Autofundtransfer_Master_to_dealer(modes, adminuserid);


            return View(modes);
        }


        public JsonResult GetAutoTransferAmountDistributor(int idno)
        {
            var AutoCreditTransfer = db.autofundtransfer_master_to_dealer.Find(idno);

            return Json(AutoCreditTransfer, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveautoTransferDistributoramount(int idno, decimal transamnt, decimal minimiumset)
        {
            var AutoCreditTransfer = db.autofundtransfer_master_to_dealer.Find(idno);
            if (AutoCreditTransfer != null)
            {
                AutoCreditTransfer.minamount = minimiumset;
                AutoCreditTransfer.Transferamount = transamnt;
                db.SaveChanges();
            }
            return Json(new { minimiumbal = AutoCreditTransfer.minamount, transferamnt = AutoCreditTransfer.Transferamount }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ChangeStatusAutoCreditDistributorTrnasfer(int idno, string curntsts)
        {
            if (curntsts.ToUpper() == "CREDIT" || curntsts.ToUpper() == "CASH")
            {
                if (curntsts.ToUpper() == "CREDIT")
                {
                    curntsts = "CASH";
                }
                else if (curntsts.ToUpper() == "CASH")
                {
                    curntsts = "CREDIT";
                }
                //curntsts = curntsts == "Credit" ? "Cash": "Credit";
                var AutoCreditTransfer = db.autofundtransfer_master_to_dealer.Find(idno);
                if (AutoCreditTransfer != null)
                {
                    AutoCreditTransfer.types = curntsts;

                    db.SaveChanges();

                }
                return Json(AutoCreditTransfer.types, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var AutoCreditTransfer = db.autofundtransfer_master_to_dealer.Find(idno);
                if (AutoCreditTransfer != null)
                {
                    AutoCreditTransfer.status = curntsts;
                    AutoCreditTransfer.updatedatetime = DateTime.Now;
                    db.SaveChanges();

                }
                return Json(AutoCreditTransfer.status, JsonRequestBehavior.AllowGet);
            }


        }

        #endregion



        //check master balance
        public ActionResult Chkbalance()
        {
            var userid = User.Identity.GetUserId();
            //get Master Remain Balance

            //Get Dealer Crdit Balance
            var dealercreditbal = _db.total_dealer_outstanding(userid).FirstOrDefault().totaloutstanding;
            //get master Credit Balance
            var mastercreditbal = _db.total_master_outstanding(userid, "").FirstOrDefault().totaloutstanding;
            return Json(new
            {

                dealercreditbal = dealercreditbal,
                mastercreditbal = mastercreditbal
            });
        }

        public ActionResult Show_Dealer_outstandingreport()
        {
            var userid = User.Identity.GetUserId();
            var show = _db.Show_Dealer_credit_report_by_admin(userid).ToList();
            return View(show);
        }

        public ActionResult Totalbaltransfer()
        {
            var userid = User.Identity.GetUserId();
            //Total Master to Dealer Balance
            var mastertodealer = _db.total_master_to_dealer_balance_by_master(userid).FirstOrDefault().totalbal;
            //Admin to  Master Balance 
            var admintomaster = _db.total_bal_master_balance_by_admin(userid).FirstOrDefault().totalbal;
            return Json(new
            {
                mastertodealer = mastertodealer,
                admintomaster = admintomaster

            });
        }

        //Total Active and Inactive USER
        #region show active and inactive users
        public ActionResult Show_All_ActiveandInactive_user()
        {
            var userid = User.Identity.GetUserId();
            //Retailers
            var stackedchart = _db.show_all_active_inactive_rem_list_Master(userid).ToList();
            int actRtl = stackedchart.Where(a => a.type == "ACTIVE").Count();
            var Reatileractive = actRtl;
            var inRtl = stackedchart.Where(a => a.type == "INACTIVE").Count();
            var Retailerinactive = inRtl;
            int total = actRtl + inRtl;
            double act_rtl_percent = ((double)actRtl / (double)total) * 100;
            double Inact_rtl_percent = ((double)inRtl / (double)total) * 100;
            var RetaileractivePer = act_rtl_percent;//Math.Ceiling(act_rtl_percent);
            var RetailerTotal = total;
            //Distributers

            var stackedchartd = _db.show_all_active_inactive_dlm_list_Master(userid).ToList();
            int actDealer = stackedchartd.Where(a => a.type == "ACTIVE").Count();
            var Dealeractive = actDealer;
            var inDealer = stackedchartd.Where(a => a.type == "INACTIVE").Count();
            var Dealerinactive = inDealer;
            int dealertotal = actDealer + inDealer;
            double act_dlm_percent = ((double)actDealer / (double)total) * 100;
            double Inact_dlm_percent = ((double)inDealer / (double)total) * 100;
            var DealeractivePer = act_dlm_percent;//Math.Ceiling(act_rtl_percent);
            var DealerTotal = dealertotal;

            return Json(new
            {
                Dealeractive = Dealeractive,
                Dealerinactive = Dealerinactive,
                DealerTotal = DealerTotal,
                DealeractivePer = DealeractivePer,
                Reatileractive = Reatileractive,
                Retailerinactive = Retailerinactive,
                RetailerTotal = RetailerTotal,
                RetaileractivePer = RetaileractivePer

            });
        }
        #endregion

        //show today and yesterday business
        #region show today and yesterday business
        public ActionResult Show_All_Recharge(string type)
        {
            var userid = User.Identity.GetUserId();
            // Donught Chart
            var result = _db.show_todayrecharge_master(userid, type).ToList();
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
            ViewData["success"] = TempData["success"];
            TempData.Remove("success");
            var userid = User.Identity.GetUserId();
            ViewBag.alldealer = new SelectList(_db.select_Dealer_for_master_ddl(userid), "Dealerid", "DealerName", null).ToList();
            return View();
        }
        [HttpPost]
        public ActionResult Send_Notification(string dealerid, string txtMsgBody)
        {
            if (dealerid == "" || dealerid == null)
            {
                var usermobiles = _db.Dealer_Details.ToList();
                foreach (var item in usermobiles)
                {
                    SendPushNotification(item.Email, Url.Action("Dashboard", "Home"), txtMsgBody, "Notification.");

                    notify.sendmessage(item.Email, txtMsgBody);
                }

            }
            else
            {
                var email = _db.Dealer_Details.Where(a => a.DealerId == dealerid).FirstOrDefault().Email;
                SendPushNotification(email, Url.Action("Dashboard", "Home"), txtMsgBody, "Notification.");

                notify.sendmessage(email, txtMsgBody);
            }
            TempData["success"] = "Notification send successfully.";
            return RedirectToAction("Notification", "Home");
        }
        #endregion End Notification
        #region  Master Income
        public ActionResult Master_income()
        {
            var userid = User.Identity.GetUserId();
            string txt_to_date1 = DateTime.Now.ToString();
            string frm_date = Convert.ToDateTime(txt_to_date1).AddDays(-1).ToShortDateString();
            string to_date = Convert.ToDateTime(txt_to_date1).ToShortDateString();
            var ch = _db.show_all_user_income(Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), "Master", userid).ToList();
            return View(ch);
        }
        [HttpPost]
        public ActionResult Master_income(string txt_frm_date)
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
            var ch = _db.show_all_user_income(Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), "Master", userid).ToList();
            return View(ch);
        }
        #endregion

        public ActionResult Actual_Master_income()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            var client = new RestClient("https://fingpayap.tapits.in/fpaepsservice/api/bankdata/bank/details");
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);
            var items = JsonConvert.DeserializeObject<BankIiNoModel>(response.Content);
            ViewBag.BankList = items.Data.Select(a => new SelectListItem { Text = a.BankName, Value = a.BankName }).ToList();
            var userid = User.Identity.GetUserId();
            var ch = db.bank_info.Where(a => a.userid == userid).ToList();
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
            var ch = _db.show_all_user_income(Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), "Master", userid).ToList();
            return View(ch);
        }
        [HttpPost]
        public ActionResult Delete_bankinfo(int idno)
        {
            if (idno != 0)
            {
                //  var ch = db.bank_info.Where(a => a.idno == idno).ToList();
                var result = db.bank_info.Find(idno);
                db.bank_info.Remove(result);
                db.SaveChanges();

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
            var ch = db.bank_info.Where(a => a.acno == txtacno).ToList();
            //var ch = (from jj in db.bank_info select jj).ToList();
            var count1 = ch.Count;
            //int count = Convert.ToInt16("count1");


            if (count1 == 0)
            {
                bank_info objCourse = new bank_info();
                objCourse.banknm = txtbanknm;
                objCourse.branch_nm = txtbranchnm;
                objCourse.ifsccode = txtifsc;
                objCourse.acno = txtacno;
                objCourse.actype = txtacctype;
                objCourse.holdername = txtname;
                objCourse.address = txtaddress;
                objCourse.userid = userid;

                db.bank_info.Add(objCourse);
                db.SaveChanges();
                TempData["add"] = "Success";
            }
            else
            {
                TempData["add"] = "Failed";
            }
            return RedirectToAction("Actual_Master_income");
        }

        public ActionResult Index()
        {
            try
            {
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    ViewData["msg"] = TempData["msgrem"];
                    ViewData["ResendMail"] = TempData["ResendMAil"];
                    string userid = User.Identity.GetUserId();
                    ViewBag.state = new SelectList(db.Select_State_Details(), "State_Id", "State_Name").ToList(); ViewBag.state1 = new SelectList(db.Select_State_Details(), "State_Id", "State_Name");
                    var Details = db.Select_Dealer_total(userid).ToList();
                    MASTER.Models.DealerModel viewModel = new MASTER.Models.DealerModel();
                    viewModel.Select_Dealer_total_Result = Details;

                    ViewBag.state1 = new SelectList(db.Select_State_Details(), "State_Id", "State_Name");
                    ViewData["ResendMail"] = TempData["ResendMAil"];

                    return View(viewModel);
                }
            }
            catch (Exception)
            {

                throw;
            }

        }

        public ActionResult ShowDistributorRem()
        {
            string userid = User.Identity.GetUserId();
            var Details = db.Select_Dealer_total(userid).ToList();
            MASTER.Models.DealerModel viewModel = new MASTER.Models.DealerModel();
            viewModel.Select_Dealer_total_Result = Details;
            return View(viewModel);
        }

        [HttpPost]
        public PartialViewResult SelectRetailerID(string MdId)
        {
            var Details = db.Dealer_retailer(MdId, "Distibutor", 1, 3000).ToList();
            MASTER.Models.DealerModel viewModel = new MASTER.Models.DealerModel();
            viewModel.Select_Retailer_list = Details;
            return PartialView("_SelectRetailersIDs", viewModel);
        }

        public JsonResult DealerByIdSearch(string ssid)
        {

            string userid = User.Identity.GetUserId();
            var ch = db.Select_Dealer_total(userid).Where(ii => ii.DealerId == ssid);
            return Json(new { list = ch }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult SignupTokens()
        {
            var userid = User.Identity.GetUserId();
            // ViewData["success"] = TempData["success"];
            // ViewData["error"] = TempData["error"];
            // TempData.Remove("success");
            //  TempData.Remove("error");
            var ch = db.Master_token(userid).FirstOrDefault();


            var dealrs = _db.Dealer_Details.Where(a => a.SSId == userid).ToList();
            var entries = _db.RetailerCreationTokensAssignHistories.Where(a => a.CommonId == userid).Join(_db.Dealer_Details.Where(a => a.SSId == userid), tkn => tkn.DealerId, dlm => dlm.DealerId, (tkn, dlm) => new { tkn, dlm });
            var dealers = dealrs.Select(a => new SelectListItem { Text = a.Email + " - " + a.Mobile, Value = a.DealerId }).ToList();
            ViewBag.ddlDealers = dealers;
            var msvalue = _db.TokenValueByAdmins.ToList();
            if (msvalue.Count > 0)
            {
                ViewBag.mastervalue = msvalue.SingleOrDefault().MasterValue;
                ViewBag.masterToken = ch;
            }
            else
            {
                ViewBag.mastervalue = 0;
            }
            return View(entries.Select(a => new RetailerCreationTokenVM
            {
                DealerId = a.dlm.DealerId,
                Email = a.dlm.FarmName,
                Idno = a.tkn.Idno,
                Tokens = a.tkn.Tokens,
                CteatedOn = a.tkn.CreatedOn,
                pre = a.tkn.RemainTokenPre,
                post = a.tkn.RemainTokenPost,
                DealerPre = a.tkn.DealerPre,
                DealerPost = a.tkn.DealerPost,
                AdminPre = a.tkn.AdminPre,
                AdminPost = a.tkn.AdminPost,
                PerTokenValue = a.tkn.PerTokenValue,
                TotalDebit = a.tkn.TotalDebit,
            }));


        }

        [HttpPost]
        public ActionResult SignupTokens(string dealerId)
        {
            var userid = User.Identity.GetUserId();
            var ch = db.Master_token(userid).FirstOrDefault();

            var dealrs = _db.Dealer_Details.Where(a => a.SSId == userid).ToList();
            var entries = _db.RetailerCreationTokensAssignHistories.Where(a => a.CommonId == userid).Join(_db.Dealer_Details.Where(a => a.SSId == userid), tkn => tkn.DealerId, dlm => dlm.DealerId, (tkn, dlm) => new RetailerCreationTokenVM
            {
                DealerId = dlm.DealerId,
                Email = dlm.FarmName,
                Idno = tkn.Idno,
                Tokens = tkn.Tokens,
                CteatedOn = tkn.CreatedOn,
                pre = tkn.RemainTokenPre,
                post = tkn.RemainTokenPost,
                DealerPre = tkn.DealerPre,
                DealerPost = tkn.DealerPost,
                AdminPre = tkn.AdminPre,
                AdminPost = tkn.AdminPost,
                PerTokenValue = tkn.PerTokenValue,
                TotalDebit = tkn.TotalDebit
            });

            var dealers = dealrs.Select(a => new SelectListItem { Text = a.Email + " - " + a.Mobile, Value = a.DealerId }).ToList();
            ViewBag.ddlDealers = dealers;
            if (!string.IsNullOrWhiteSpace(dealerId) && dealerId != "ALL")
            {
                entries = entries.Where(a => a.DealerId == dealerId);
            }
            var msvalue = _db.TokenValueByAdmins.ToList();
            if (msvalue.Count > 0)
            {
                ViewBag.masterToken = ch;
                ViewBag.mastervalue = msvalue.SingleOrDefault().MasterValue;
            }
            else
            {
                ViewBag.mastervalue = 0;
            }

            return View(entries);
        }
        [HttpPost]
        //public ActionResult addJoiningToken(string ddlDealer, int tokenCount)
        //{
        //    try
        //    {
        //        var userid = User.Identity.GetUserId();
        //        RetailerCreationTokensAssignHistory entry = new RetailerCreationTokensAssignHistory();
        //        var mastertokencount = _db.DealerCreationTokensAssignHistories.Where(a => a.MasterId == userid).SingleOrDefault().RemainTokenPost;
        //        if (mastertokencount >= tokenCount)
        //        {
        //            RetailerCreationToken token = _db.RetailerCreationTokens.SingleOrDefault(a => a.DealerId == ddlDealer);
        //            if (token == null)
        //            {
        //                token = new RetailerCreationToken();
        //                token.DealerId = ddlDealer;
        //                token.Tokens = tokenCount;
        //                _db.RetailerCreationTokens.Add(token);
        //                entry.CreatedOn = DateTime.Now;
        //                entry.DealerId = ddlDealer;
        //                entry.Tokens = tokenCount;
        //                entry.RemainTokenPre = 0;
        //                entry.RemainTokenPost = tokenCount;
        //                entry.CommonId = userid;
        //                //db.RetailerCreationTokensAssignHistories.Add(entry);
        //            }
        //            else
        //            {
        //                entry.CreatedOn = DateTime.Now;
        //                entry.DealerId = ddlDealer;
        //                entry.CommonId = userid;
        //                entry.Tokens = tokenCount;
        //                entry.RemainTokenPre = token.Tokens;
        //                entry.RemainTokenPost = token.Tokens + tokenCount;
        //                //db.RetailerCreationTokensAssignHistories.Add(entry);
        //                token.Tokens = token.Tokens + tokenCount;
        //            }
        //            var count = _db.TokenValueByAdmins.ToList();
        //            if (count.Any())
        //            {
        //                var Dealervalue = _db.TokenValueByAdmins.SingleOrDefault().DistributorValue;
        //                decimal TotalTokenValue = Convert.ToDecimal(Dealervalue) * Convert.ToDecimal(tokenCount);
        //                var Dealerinfo = _db.Remain_dealer_balance.Where(a => a.DealerID == ddlDealer).SingleOrDefault();
        //                var DealerPre = Dealerinfo.Remainamount;
        //                var MasterPre = _db.Remain_superstokist_balance.Where(a => a.SuperStokistID == userid).SingleOrDefault().Remainamount;
        //                if (DealerPre >= TotalTokenValue)
        //                {
        //                    var DealerPost = DealerPre - TotalTokenValue;
        //                    var MasterPost = MasterPre + TotalTokenValue;

        //                    entry.DealerPre = DealerPre;
        //                    entry.DealerPost = DealerPost;
        //                    entry.AdminPre = MasterPre;
        //                    entry.AdminPost = MasterPost;
        //                    entry.TotalDebit = TotalTokenValue;
        //                    entry.TotalTokenValue = TotalTokenValue;
        //                    entry.PerTokenValue = Convert.ToDecimal(Dealervalue);
        //                    _db.RetailerCreationTokensAssignHistories.Add(entry);

        //                    //Update Dealer Remain Balance
        //                    Dealerinfo.Remainamount = DealerPost;

        //                    //Update Admin Remain Balance
        //                    var id = _db.Remain_superstokist_balance.Where(a => a.SuperStokistID == userid).SingleOrDefault();
        //                    id.Remainamount = MasterPost;


        //                    LedgerReport ledger = new LedgerReport();
        //                    ledger.UserId = ddlDealer;
        //                    ledger.Role = "Dealer";
        //                    ledger.Particulars = "Token Purchase";
        //                    ledger.UserRemainAmount = DealerPost;
        //                    ledger.Date = Convert.ToDateTime(DateTime.Now.ToString());
        //                    ledger.Amount = TotalTokenValue;
        //                    ledger.Credit = 0;
        //                    ledger.Debit = TotalTokenValue;
        //                    _db.LedgerReports.Add(ledger);

        //                    ledger.UserId = userid;
        //                    ledger.Role = "Master";
        //                    ledger.Particulars = "Token Purchase";
        //                    ledger.UserRemainAmount = MasterPost;
        //                    ledger.Date = Convert.ToDateTime(DateTime.Now.ToString());
        //                    ledger.Amount = TotalTokenValue;
        //                    ledger.Credit = TotalTokenValue;
        //                    ledger.Debit = 0;
        //                    _db.LedgerReports.Add(ledger);

        //                    var mastertoken = _db.DealerCreationTokensAssignHistories.Where(a => a.MasterId == userid).SingleOrDefault();
        //                    mastertoken.RemainTokenPost = mastertokencount - tokenCount;


        //                    TempData["success"] = "Token Debited Successfully.";
        //                }
        //                else
        //                {
        //                    TempData["error"] = "Dealer Remain Value is Low.";
        //                    return RedirectToAction("SignupTokens");
        //                }
        //            }
        //            else
        //            {
        //                TempData["error"] = "Please Set the Dealer token Value.Contect To Admin.";
        //                return RedirectToAction("SignupTokens");
        //            }
        //            _db.SaveChanges();
        //            return RedirectToAction("SignupTokens");
        //        }
        //        else
        //        {
        //            TempData["error"] = "Master Remaining Token is Less Then Dealer Total Token.Please Contact Admin to Incress your Token .";
        //            return RedirectToAction("SignupTokens");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["error"] = ex.Message;
        //        return RedirectToAction("SignupTokens");
        //    }
        //}
        public ActionResult addJoiningToken_dlm(string ddlDealer, int tokenCount)
        {
            Token_PaidService_VM Tkn_PaidSer = new Token_PaidService_VM();
            try
            {
                if (tokenCount < 1) throw new Exception("Token Can Not Be Negative");
                var userid = User.Identity.GetUserId();
                RetailerCreationTokensAssignHistory entry = new RetailerCreationTokensAssignHistory();
                RetailerCreationToken token = db.RetailerCreationTokens.SingleOrDefault(a => a.DealerId == ddlDealer);

                var JoinTokenentries = db.RetailerCreationTokensAssignHistories.Where(a => a.CommonId == userid).Join(db.Dealer_Details, tkn => tkn.DealerId, dlm => dlm.DealerId, (tkn, dlm) => new RetailerCreationTokenVM
                {
                    DealerId = dlm.DealerId,
                    Email = dlm.FarmName,
                    Idno = tkn.Idno,
                    Tokens = tkn.Tokens,
                    CteatedOn = tkn.CreatedOn,
                    pre = tkn.RemainTokenPre,
                    post = tkn.RemainTokenPost,
                    DealerPre = tkn.DealerPre,
                    DealerPost = tkn.DealerPost,
                    AdminPre = tkn.AdminPre,
                    AdminPost = tkn.AdminPost,
                    PerTokenValue = tkn.PerTokenValue,
                    TotalDebit = tkn.TotalDebit
                }).OrderByDescending(aa => aa.CteatedOn);
                Tkn_PaidSer.RetailerCreationTokenVM = JoinTokenentries.ToList();

                if (token == null)
                {
                    token = new RetailerCreationToken();
                    token.DealerId = ddlDealer;
                    token.Tokens = tokenCount;
                    db.RetailerCreationTokens.Add(token);
                    entry.CreatedOn = DateTime.Now;
                    entry.DealerId = ddlDealer;
                    entry.Tokens = tokenCount;
                    entry.RemainTokenPre = 0;
                    entry.RemainTokenPost = tokenCount;
                    entry.CommonId = userid;
                    //db.RetailerCreationTokensAssignHistories.Add(entry);
                }
                else
                {
                    entry.CreatedOn = DateTime.Now;
                    entry.DealerId = ddlDealer;
                    entry.CommonId = userid;
                    entry.Tokens = tokenCount;
                    entry.RemainTokenPre = token.Tokens;
                    entry.RemainTokenPost = token.Tokens + tokenCount;
                    //db.RetailerCreationTokensAssignHistories.Add(entry);
                    token.Tokens = token.Tokens + tokenCount;
                }
                var count = db.TokenValueByAdmins.ToList();
                if (count.Any())
                {
                    var Dealervalue = db.TokenValueByAdmins.SingleOrDefault().DistributorValue;
                    decimal TotalTokenValue = Convert.ToDecimal(Dealervalue) * Convert.ToDecimal(tokenCount);
                    var Dealerinfo = db.Remain_dealer_balance.Where(a => a.DealerID == ddlDealer).SingleOrDefault();
                    var DealerPre = Dealerinfo.Remainamount;
                    var Masterinfo = db.Remain_superstokist_balance.Where(a => a.SuperStokistID == userid).SingleOrDefault();
                    var MasterPre = Masterinfo.Remainamount;
                    if (DealerPre >= TotalTokenValue)
                    {
                        var DealerPost = DealerPre - TotalTokenValue;
                        var AdminPost = MasterPre + TotalTokenValue;

                        entry.DealerPre = DealerPre;
                        entry.DealerPost = DealerPost;
                        entry.AdminPre = MasterPre;
                        entry.AdminPost = AdminPost;
                        entry.TotalDebit = TotalTokenValue;
                        entry.TotalTokenValue = TotalTokenValue;
                        entry.PerTokenValue = Convert.ToDecimal(Dealervalue);
                        db.RetailerCreationTokensAssignHistories.Add(entry);

                        //Update Dealer Remain Balance
                        Dealerinfo.Remainamount = DealerPost;

                        var TokenValue = db.DealerCreationTokens.Where(a => a.Masterid == userid).SingleOrDefault();
                        var tok = Convert.ToInt32(TokenValue.Tokens);
                        TokenValue.Tokens = tok - tokenCount;
                        //Update Admin Remain Balance
                        var id = db.Remain_superstokist_balance.Where(a => a.SuperStokistID == userid).SingleOrDefault();
                        id.Remainamount = AdminPost;
                        //insert into LedgerReport values(@Retailerid,'Retailer', getdate(), 'New Retailer Create', -@JoiningCharge, 0, -@JoiningCharge, -@JoiningCharge)
                        LedgerReport ledger = new LedgerReport();
                        ledger.UserId = ddlDealer;
                        ledger.Role = "Dealer";
                        ledger.Particulars = "Token Purchase";
                        ledger.UserRemainAmount = DealerPost;
                        ledger.Date = Convert.ToDateTime(DateTime.Now.ToString());
                        ledger.Amount = TotalTokenValue;
                        ledger.Credit = 0;
                        ledger.Debit = TotalTokenValue;
                        db.LedgerReports.Add(ledger);

                        ledger.UserId = userid;
                        ledger.Role = "master";
                        ledger.Particulars = "Token Purchase";
                        ledger.UserRemainAmount = AdminPost;
                        ledger.Date = Convert.ToDateTime(DateTime.Now.ToString());
                        ledger.Amount = TotalTokenValue;
                        ledger.Credit = TotalTokenValue;
                        ledger.Debit = 0;
                        db.LedgerReports.Add(ledger);

                        TempData["success"] = "Credited Successfully.";
                    }
                    else
                    {
                        return Json("Insufficient balance.", JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json("Set Token Value.", JsonRequestBehavior.AllowGet);
                }
                db.SaveChanges();

                JoinTokenentries = db.RetailerCreationTokensAssignHistories.Where(a => a.CommonId == "Admin").Join(db.Dealer_Details, tkn => tkn.DealerId, dlm => dlm.DealerId, (tkn, dlm) => new RetailerCreationTokenVM
                {
                    DealerId = dlm.DealerId,
                    Email = dlm.FarmName,
                    Idno = tkn.Idno,
                    Tokens = tkn.Tokens,
                    CteatedOn = tkn.CreatedOn,
                    pre = tkn.RemainTokenPre,
                    post = tkn.RemainTokenPost,
                    DealerPre = tkn.DealerPre,
                    DealerPost = tkn.DealerPost,
                    AdminPre = tkn.AdminPre,
                    AdminPost = tkn.AdminPost,
                    PerTokenValue = tkn.PerTokenValue,
                    TotalDebit = tkn.TotalDebit
                }).OrderByDescending(aa => aa.CteatedOn);
                Tkn_PaidSer.RetailerCreationTokenVM = JoinTokenentries.ToList();
                return Json(Tkn_PaidSer.RetailerCreationTokenVM, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
                //return Json(Tkn_PaidSer.RetailerCreationTokenVM, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Remain_dealer_token_report()
        {
            var userid = User.Identity.GetUserId();
            var show = _db.dealer_remain_token_report_by_master(userid).ToList();
            //return View(show);
            return Json(show, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ResendDealerEmail(string email)
        {
            var data = _db.ResendConfirmMails.Where(aa => aa.Email == email).SingleOrDefault();
            if (data != null)
            {
                var callbackurl = data.CallBackUrl;
                var pass = data.Password;
                string body = new CommonUtil().PopulateBodyDealer("", "Confirm your account", "", "" + callbackurl + "", email, pass);
                new CommonUtil().Insertsendmail(email, "Confirm your account", body, callbackurl);
                TempData["ResendMAil"] = "Confirmation Mail Is Successfully Resend .";
            }
            else
            {
                TempData["ResendMAil"] = "Confirmation Mail Not Resend .";
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }
        //delete Dealer and send otp 
        public JsonResult DeleteDealerSendOTP()
        {
            try
            {
                var userid = User.Identity.GetUserId();
                int pin = new Random().Next(1000, 10000);
                deleteuserotp motp = new deleteuserotp();
                motp.otp = pin;
                _db.deleteuserotps.Add(motp);
                _db.SaveChanges();
                new DeleteUserSendOtp().sendotpBymastermail(pin, userid);
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("Failed", JsonRequestBehavior.AllowGet);
            }

        }
        public JsonResult Deletedealer(string DealerId, int OTP)
        {
            try
            {
                var chk = _db.deleteuserotps.Any(a => a.otp == OTP);
                if (chk == true)
                {
                    if (DealerId != null && DealerId != "")
                    {
                        _db.delete_Dealer(DealerId);
                        _db.deleteOTP();
                        return Json("Success", JsonRequestBehavior.AllowGet);

                    }
                    else
                    {
                        return Json("Failed", JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json("OTPWrong", JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                return Json("Failed", JsonRequestBehavior.AllowGet);
            }
        }
        //end
        //Complaint Request 
        public ActionResult Complaint()
        {
            var userid = User.Identity.GetUserId();
            var ch = _db.proc_Complaint_request(userid, "").ToList();
            return View(ch);
        }

        [HttpPost]
        public ActionResult Complaint_insert(string message)
        {
            var statusAdmin = _db.PushNotificationStatus.Where(a => a.UserRole == "Admin").SingleOrDefault().Status;
            var Emailid = _db.Admin_details.Single().email;
            string userid = User.Identity.GetUserId();
            var retaileremaillid = _db.Users.Where(p => p.UserId == userid).Single().Email;
            Guid randomId = Guid.NewGuid();
            string uniqueId = randomId.ToString().Substring(0, 18).ToUpper();
            DateTime date = System.DateTime.Now;
            complaint_request objCourse = new complaint_request();
            objCourse.subject = "Chatting";
            objCourse.complant = message;
            objCourse.complaintid = uniqueId;
            objCourse.userid = userid;
            objCourse.sts = "Open";
            objCourse.rdate = date;
            _db.complaint_request.Add(objCourse);
            _db.SaveChanges();
            if (statusAdmin == "Y")
            {
                SendPushNotification(Emailid, Url.Action("Money_Transfer_Report", "Home"), "User " + retaileremaillid + " is Send the Complaint For You .And Compalint is that " + message + "", "Complaint Insert..");
            }
            return RedirectToAction("Complaint");
        }
        public ActionResult RenderHeader()
        {
            return View();
        }
        public ActionResult TopBAR()
        {
            var currentuser = User.Identity.GetUserId();
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                TempData["VendorName"] = db.Vendor_details.FirstOrDefault(a => a.userid == currentuser)?.Name;
                TempData["VendorEmail"] = db.Users.SingleOrDefault(a => a.UserId == currentuser)?.Email;
                TempData["Balance"] = db.Remain_superstokist_balance.SingleOrDefault(a => a.SuperStokistID == currentuser)?.Remainamount;
            }
            return View();
        }
        public ActionResult RenderSection()
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                var currentuser = User.Identity.GetUserId();
                TempData["VendorEmail"] = db.Users.SingleOrDefault(a => a.UserId == currentuser)?.Email;
            }
            return View();
        }
        //End
        //Profile
        [HttpGet]
        public new ActionResult Profile()
        {
            string userid = User.Identity.GetUserId();
            var userDetails = _db.Users.SingleOrDefault(a => a.UserId == userid);
            var MD = _db.Superstokist_details.FirstOrDefault(m => m.SSId == userid);
            ViewBag.MD_Details = MD;
            var gt = _db.State_Desc.SingleOrDefault(a => a.State_id == MD.State)?.State_name;
            ViewBag.ddlstate = gt;
            var cities = _db.District_Desc.SingleOrDefault(c => c.Dist_id == MD.District && c.State_id == MD.State)?.Dist_Desc;
            ViewBag.district = cities;

            ViewBag.Sate = _db.State_Desc.Select(a => new SelectListItem { Text = a.State_name, Value = a.State_id.ToString() }).ToList();
            ViewBag.ALLDistrict = _db.District_Desc.Where(a => a.State_id == MD.State).Select(a => new SelectListItem { Text = a.Dist_Desc, Value = a.Dist_id.ToString() }).ToList();
            ViewBag.passcodesetings = db.passcodesettings.Where(x => x.userid == userid).SingleOrDefault().passcodetype;
            ViewData["msg"] = TempData["success"];
            TempData.Remove("success");
            return View(userDetails);
        }


        //Don't Change the Action Name If Any Changes Occurs tehn Is HarmFull For PassCode




        public ActionResult PasscodeSettingByrem(string passcodetype)
        {
            int isupdateauto = 0;
            string userid = User.Identity.GetUserId();
            var servid = db.passcodesettings.Where(x => x.userid == userid).Single();
            if (servid != null)
            {
                var SMS_passcodesmsonline = db.SMSSendAlls.Where(x => x.ServiceName == "PasscodeOnline").SingleOrDefault();
                var Email_passcodesmsonline = db.EmailSendAlls.Where(x => x.ServiceName == "PasscodeOnline1").SingleOrDefault().Status;


                bool includeLowercase = false;
                bool includeUppercase = false;
                bool includeNumeric = true;
                bool includeSpecial = false;
                bool includeSpaces = false;
                int lengthOfPassword = 6;
                string pass = PasswordGenerator.GeneratePassword(includeLowercase, includeUppercase, includeNumeric, includeSpecial, includeSpaces, lengthOfPassword);

                DateTime? expiredate = DateTime.Now.Date;
                if (passcodetype == "OFF")
                {

                    while (!PasswordGenerator.PasswordIsValid(includeLowercase, includeUppercase, includeNumeric, includeSpecial, includeSpaces, pass))
                    {
                        pass = PasswordGenerator.GeneratePassword(includeLowercase, includeUppercase, includeNumeric, includeSpecial, includeSpaces, lengthOfPassword);
                    }

                    pass = null;
                    expiredate = null;

                }



                var Password = pass;
                var remmobile = db.Users.Where(x => x.UserId == userid).SingleOrDefault().PhoneNumber;

                if (passcodetype == "PERDAY")
                {
                    expiredate = expiredate.Value.AddDays(1);
                }
                else if (passcodetype == "WEAKS")
                {
                    expiredate = expiredate.Value.AddDays(7);
                }
                else if (passcodetype == "MONTHS")
                {
                    expiredate = expiredate.Value.AddMonths(1);
                }

                try
                {
                    if (passcodetype != "OFF")
                    {
                        smssend.sms_init(SMS_passcodesmsonline.Status, SMS_passcodesmsonline.Whatsapp_Status, "PASSCODEOTP", remmobile, Password, expiredate);
                        if (Email_passcodesmsonline == "Y")
                        {
                            var ToCC = db.Admin_details.FirstOrDefault().email;
                            CommUtilEmail emailsend = new CommUtilEmail();
                            var rememail = db.Users.Where(x => x.UserId == userid).SingleOrDefault().UserName;
                            emailsend.EmailLimitChk(rememail, ToCC, "PASSCODE", "Your  Passcode " + Password + " Your Passcode valid for " + expiredate, "No CallBackUrl");
                        }
                    }
                }
                catch { }
                servid.passcode = Password;
                servid.passcodetype = passcodetype;
                servid.expiretime = expiredate;
                isupdateauto = db.SaveChanges();

                if (isupdateauto > 0 || passcodetype == "OFF")
                {

                    UserManager.SetTwoFactorEnabled(User.Identity.GetUserId(), false);
                    var user = UserManager.FindById(User.Identity.GetUserId());

                    SignInManager.SignIn(user, isPersistent: false, rememberBrowser: false);
                    isupdateauto = 1;
                }
            }
            return Json(isupdateauto, JsonRequestBehavior.AllowGet);
        }



        public ActionResult PassCodeVeryFY()
        {
            var userid = User.Identity.GetUserId();
            var expiredateSSS = db.passcodesettings.Where(x => x.userid == userid).SingleOrDefault();
            if (expiredateSSS != null)
            {

                Cookiesforattributes cook = new Cookiesforattributes();
                var passexptime = cook.getcookies(userid);
                if (passexptime != null && passexptime != "")
                {
                    DateTime exp = Convert.ToDateTime(passexptime);
                    if (exp >= expiredateSSS.expiretime)
                    {
                        return RedirectToAction("Dashboard", "Home");
                    }
                }
            }
            return PartialView("_PASSCODEOTP");
        }


        public ActionResult CHECKPASSCODEPASSWORD(string Passscodes)
        {
            VastwebmultiEntities db = new VastwebmultiEntities();
            var userid = User.Identity.GetUserId();
            var expiredateSSS = db.passcodesettings.Where(x => x.userid == userid && x.passcode == Passscodes).SingleOrDefault();

            if (expiredateSSS != null)
            {
                Cookiesforattributes cook = new Cookiesforattributes();
                cook.setcookies(expiredateSSS.expiretime.ToString(), userid);
                return RedirectToAction("Dashboard", "Home");

            }
            else
            {
                return RedirectToAction("PassCodeVeryFY", "Home");
            }




        }

        public ActionResult ResendPASSCODEPASSWORD()
        {
            VastwebmultiEntities db = new VastwebmultiEntities();
            var userid = User.Identity.GetUserId();
            var rememailid = User.Identity.GetUserName();
            var expiredateSSS = db.passcodesettings.Where(x => x.userid == userid).SingleOrDefault();
            var remmobile = db.Users.Where(x => x.UserId == userid).SingleOrDefault().PhoneNumber;
            var SMS_passcodesmsonline = db.SMSSendAlls.Where(x => x.ServiceName == "PasscodeOnline").SingleOrDefault();
            var Email_passcodesmsonline = db.EmailSendAlls.Where(x => x.ServiceName == "PasscodeOnline1").SingleOrDefault().Status;


            if (expiredateSSS != null)
            {
                if (expiredateSSS.expiretime > DateTime.Now)
                {
                    smssend.sms_init(SMS_passcodesmsonline.Status, SMS_passcodesmsonline.Whatsapp_Status, "PASSCODEOTP", remmobile, expiredateSSS.passcode, expiredateSSS.expiretime);
                    if (Email_passcodesmsonline == "Y")
                    {
                        var ToCC = db.Admin_details.FirstOrDefault().email;
                        CommUtilEmail emailsend = new CommUtilEmail();
                        var rememail = db.Users.Where(x => x.UserId == userid).SingleOrDefault().UserName;
                        emailsend.EmailLimitChk(rememailid, ToCC, "PASSCODE", "Your  Passcode " + expiredateSSS.passcode + " Your Passcode valid for " + expiredateSSS.expiretime, "No CallBackUrl");
                    }
                    TempData["checkuseractive"] = "";
                    return RedirectToAction("PassCodeVeryFY", "Home");
                }
                else
                {
                    bool includeLowercase = false;
                    bool includeUppercase = false;
                    bool includeNumeric = true;
                    bool includeSpecial = false;
                    bool includeSpaces = false;
                    int lengthOfPassword = 6;
                    string pass = PasswordGenerator.GeneratePassword(includeLowercase, includeUppercase, includeNumeric, includeSpecial, includeSpaces, lengthOfPassword);

                    //  DateTime? expiredate = DateTime.Now;

                    var expiredate = "";
                    while (!PasswordGenerator.PasswordIsValid(includeLowercase, includeUppercase, includeNumeric, includeSpecial, includeSpaces, pass))
                    {
                        pass = PasswordGenerator.GeneratePassword(includeLowercase, includeUppercase, includeNumeric, includeSpecial, includeSpaces, lengthOfPassword);
                    }




                    var passcodeuser = db.passcodesettings.Where(x => x.userid == userid).SingleOrDefault();
                    if (passcodeuser.passcodetype == "PERDAY")
                    {
                        expiredate = DateTime.Now.Date.AddDays(1).ToString();

                        //DateTime.Now.AddDays(1).Date.ToString();
                    }
                    else if (passcodeuser.passcodetype == "WEAKS")
                    {
                        expiredate = DateTime.Now.Date.AddDays(7).ToString();
                    }
                    else if (passcodeuser.passcodetype == "MONTHS")
                    {
                        expiredate = DateTime.Now.Date.AddDays(30).ToString();
                    }


                    var Password = pass;

                    passcodeuser.passcode = Password;
                    passcodeuser.expiretime = Convert.ToDateTime(expiredate);
                    db.SaveChanges();

                    smssend.sms_init(SMS_passcodesmsonline.Status, SMS_passcodesmsonline.Whatsapp_Status, "PASSCODEOTP", remmobile, Password, expiredate);
                    if (Email_passcodesmsonline == "Y")
                    {
                        var ToCC = db.Admin_details.FirstOrDefault().email;
                        CommUtilEmail emailsend = new CommUtilEmail();
                        var USERSEMAIL = db.Users.Where(X => X.UserId == userid).SingleOrDefault().UserName;
                        emailsend.EmailLimitChk(USERSEMAIL, ToCC, "PASSCODE", "Your  Passcode " + Password + " Your Passcode valid for " + expiredate, "No CallBackUrl");
                    }
                }

            }
            return RedirectToAction("PassCodeVeryFY", "Home");
        }


        //Don't Change the Action Name If Any Changes Occurs then it will HarmFull For PassCode








        [HttpGet]
        public ActionResult addTokenMaster()
        {
            var ddlMaster = User.Identity.GetUserId();
            return View();
        }

        [ChildActionOnly]
        public ActionResult _Tokenreport()
        {
            var ddlMaster = User.Identity.GetUserId();
            var ch = db.Master_token_select(ddlMaster).ToList();

            return View(ch);
        }

        public ActionResult addTokenMaster1(int tokenCount)
        {
            var ddlMaster = User.Identity.GetUserId();
            Token_PaidService_VM1 Tkn_PaidSer = new Token_PaidService_VM1();
            try
            {
                if (tokenCount < 1) throw new Exception("Token Can Not Be Negative");
                DealerCreationTokensAssignHistory entry = new DealerCreationTokensAssignHistory();
                DealerCreationToken token = db.DealerCreationTokens.SingleOrDefault(a => a.Masterid == ddlMaster);
                var TokenAssignEntriess = db.DealerCreationTokensAssignHistories.Join(db.Superstokist_details, tkn => tkn.MasterId, dlm => dlm.SSId, (tkn, dlm) => new DealerCreationTokenVM1
                {
                    Masterid = dlm.SSId,
                    Email = dlm.FarmName,
                    Idno = tkn.Idno,
                    Tokens = tkn.Tokens,
                    CteatedOn = tkn.CreatedOn,
                    pre = tkn.RemainTokenPre,
                    post = tkn.RemainTokenPost,
                    MasterPre = tkn.MasterPre,
                    MasterPost = tkn.MasterPost,
                    AdminPre = tkn.AdminPre,
                    AdminPost = tkn.AdminPost,
                    PerTokenValue = tkn.PerTokenValue,
                    TotalDebit = tkn.TotalDebit
                }).OrderByDescending(aa => aa.CteatedOn);
                Tkn_PaidSer.DealerCreationTokenVM = TokenAssignEntriess.ToList();
                if (token == null)
                {
                    token = new DealerCreationToken();
                    token.Masterid = ddlMaster;
                    token.Tokens = tokenCount;
                    db.DealerCreationTokens.Add(token);
                    entry.CreatedOn = DateTime.Now;
                    entry.MasterId = ddlMaster;
                    entry.Tokens = tokenCount;
                    entry.RemainTokenPre = 0;
                    entry.RemainTokenPost = tokenCount;
                    //db.RetailerCreationTokensAssignHistories.Add(entry);
                }
                else
                {
                    entry.CreatedOn = DateTime.Now;
                    entry.MasterId = ddlMaster;
                    entry.Tokens = tokenCount;
                    entry.RemainTokenPre = token.Tokens;
                    entry.RemainTokenPost = token.Tokens + tokenCount;
                    //db.RetailerCreationTokensAssignHistories.Add(entry);
                    token.Tokens = token.Tokens + tokenCount;
                }
                var count = db.TokenValueByAdmins.ToList();
                if (count.Any())
                {
                    var Mastervalue = db.TokenValueByAdmins.SingleOrDefault().MasterValue;
                    decimal TotalTokenValue = Convert.ToDecimal(Mastervalue) * Convert.ToDecimal(tokenCount);
                    var Masterinfo = db.Remain_superstokist_balance.Where(a => a.SuperStokistID == ddlMaster).SingleOrDefault();
                    var MasterPre = Masterinfo.Remainamount;
                    var AdminPre = db.Remain_Admin_balance.SingleOrDefault().RemainAmount;
                    if (MasterPre >= TotalTokenValue)
                    {
                        var MasterPost = MasterPre - TotalTokenValue;
                        var AdminPost = AdminPre + TotalTokenValue;

                        entry.MasterPre = MasterPre;
                        entry.MasterPost = MasterPost;
                        entry.AdminPre = AdminPre;
                        entry.AdminPost = AdminPost;
                        entry.TotalDebit = TotalTokenValue;
                        entry.TotalTokenValue = TotalTokenValue;
                        entry.PerTokenValue = Convert.ToDecimal(Mastervalue);
                        entry.role = "Self";
                        db.DealerCreationTokensAssignHistories.Add(entry);

                        //Update Dealer Remain Balance
                        Masterinfo.Remainamount = MasterPost;

                        //Update Admin Remain Balance
                        var id = db.Remain_Admin_balance.SingleOrDefault();
                        id.RemainAmount = AdminPost;

                        LedgerReport ledger = new LedgerReport();
                        ledger.UserId = ddlMaster;
                        ledger.Role = "Master";
                        ledger.Particulars = "Token Purchase";
                        ledger.UserRemainAmount = MasterPost;
                        ledger.Date = Convert.ToDateTime(DateTime.Now.ToString());
                        ledger.Amount = TotalTokenValue;
                        ledger.Credit = 0;
                        ledger.Debit = TotalTokenValue;
                        db.LedgerReports.Add(ledger);

                        ledger.UserId = "Admin";
                        ledger.Role = "Admin";
                        ledger.Particulars = "Token Purchase";
                        ledger.UserRemainAmount = AdminPost;
                        ledger.Date = Convert.ToDateTime(DateTime.Now.ToString());
                        ledger.Amount = TotalTokenValue;
                        ledger.Credit = TotalTokenValue;
                        ledger.Debit = 0;
                        db.LedgerReports.Add(ledger);

                        TempData["success"] = "Credited Successfully.";
                    }
                    else
                    {
                        return Json("Insufficient balance.", JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    TempData["error"] = "Set Token Value.";
                    return Json(Tkn_PaidSer.DealerCreationTokenVM, JsonRequestBehavior.AllowGet);
                }
                db.SaveChanges();
                TokenAssignEntriess = db.DealerCreationTokensAssignHistories.Join(db.Superstokist_details, tkn => tkn.MasterId, dlm => dlm.SSId, (tkn, dlm) => new DealerCreationTokenVM1
                {
                    Masterid = dlm.SSId,
                    Email = dlm.FarmName,
                    Idno = tkn.Idno,
                    Tokens = tkn.Tokens,
                    CteatedOn = tkn.CreatedOn,
                    pre = tkn.RemainTokenPre,
                    post = tkn.RemainTokenPost,
                    MasterPre = tkn.MasterPre,
                    MasterPost = tkn.MasterPost,
                    AdminPre = tkn.AdminPre,
                    AdminPost = tkn.AdminPost,
                    PerTokenValue = tkn.PerTokenValue,
                    TotalDebit = tkn.TotalDebit
                }).OrderByDescending(aa => aa.CteatedOn);
                Tkn_PaidSer.DealerCreationTokenVM = TokenAssignEntriess.ToList();
                return Json(Tkn_PaidSer.DealerCreationTokenVM, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
                //return Json(Tkn_PaidSer.DealerCreationTokenVM, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Masterhold_comm_reports()
        {

            var userid = User.Identity.GetUserId();
            var userssmaster = db.masterhold_commisionTransfer_reports("master", userid).ToList();
            List<masterCommon_status_master_dlm_Transfer_cls> listview1 = new List<masterCommon_status_master_dlm_Transfer_cls>();
            foreach (var item in userssmaster)
            {

                masterCommon_status_master_dlm_Transfer_cls model = new masterCommon_status_master_dlm_Transfer_cls();
                model.Frmname = item.FarmName;
                model.Name = item.Name;
                model.mobile = item.Mobile;
                model.Userid = item.usersid;
                model.totalcomm = item.totalcomm.Value;
                model.Role = item.userroles;
                model.Hold_date = item.Hold_date.Value;
                model.trasnferdate = item.trasnferdate.Value;
                model.master_remain = item.master_remain.Value;
                model.master_remain_pre = item.master_remain_pre.Value;
                model.admin_prebal = item.admin_prebal.Value;
                model.admin_postbal = item.admin_postbal.Value;
                model.master_remain = item.master_remain.Value;
                model.idno = item.idno;
                listview1.Add(model);

            }

            return View(listview1);
        }

        #region TDSReport
        public ActionResult TDSReport()
        {
            var userid = User.Identity.GetUserId();
            TDSReportModel model = new TDSReportModel();
            DateTime date = DateTime.Now;
            var txt_frm_date = new DateTime(date.Year, date.Month, 1);
            string to_date = DateTime.Now.ToString();

            string CurrentMonthName = date.ToString("MMMM");
            string OldMonth1 = date.AddMonths(-1).ToString("MMMM");
            string OldMonth2 = date.AddMonths(-2).ToString("MMMM");
            ViewBag.CurrentMonthName = CurrentMonthName;
            ViewBag.OldMonth1 = OldMonth1;
            ViewBag.OldMonth2 = OldMonth2;
            ViewBag.Crmonth = CurrentMonthName;
            model.TDSMaster = _db.TDS_Report_master(Convert.ToDateTime(txt_frm_date), Convert.ToDateTime(to_date), userid).ToList();
            return View(model);
        }
        [HttpPost]
        public ActionResult TDSReport(string submit)
        {
            var useridid = User.Identity.GetUserId();
            TDSReportModel model = new TDSReportModel();
            DateTime txt_frm_date = DateTime.Now;
            DateTime to_date = DateTime.Now;
            DateTime date = DateTime.Now;
            string CurrentMonthName = date.ToString("MMMM");
            string OldMonth1 = date.AddMonths(-1).ToString("MMMM");
            string OldMonth2 = date.AddMonths(-2).ToString("MMMM");
            if (CurrentMonthName == submit)
            {
                txt_frm_date = new DateTime(date.Year, date.Month, 1);
                to_date = DateTime.Now;

            }
            else if (OldMonth1 == submit)
            {
                txt_frm_date = new DateTime(date.AddMonths(-1).Year, date.AddMonths(-1).Month, 1);
                to_date = new DateTime(date.Year, date.Month, 1);
            }
            else
            {
                txt_frm_date = new DateTime(date.AddMonths(-2).Year, date.AddMonths(-2).Month, 1);
                to_date = new DateTime(date.AddMonths(-1).Year, date.AddMonths(-1).Month, 1);
            }
            model.TDSMaster = _db.TDS_Report_master(Convert.ToDateTime(txt_frm_date), Convert.ToDateTime(to_date), useridid).ToList();

            ViewBag.CurrentMonthName = CurrentMonthName;
            ViewBag.OldMonth1 = OldMonth1;
            ViewBag.OldMonth2 = OldMonth2;
            ViewBag.Crmonth = submit;

            return View(model);
        }
        #endregion
        #region GSTReport
        public ActionResult GSTReport()
        {
            var userid = User.Identity.GetUserId();
            GSTReportModel model = new GSTReportModel();
            DateTime date = DateTime.Now;
            var txt_frm_date = new DateTime(date.Year, date.Month, 1);
            string to_date = DateTime.Now.ToString();

            string CurrentMonthName = date.ToString("MMMM");
            string OldMonth1 = date.AddMonths(-1).ToString("MMMM");
            string OldMonth2 = date.AddMonths(-2).ToString("MMMM");
            ViewBag.CurrentMonthName = CurrentMonthName;
            ViewBag.OldMonth1 = OldMonth1;
            ViewBag.OldMonth2 = OldMonth2;
            ViewBag.Crmonth = CurrentMonthName;
            model.GSTMaster = _db.GST_Report_master(Convert.ToDateTime(txt_frm_date), Convert.ToDateTime(to_date), userid).ToList();
            return View(model);
        }
        [HttpPost]
        public ActionResult GSTReport(string submit)
        {
            GSTReportModel model = new GSTReportModel();
            var userid = User.Identity.GetUserId();
            DateTime txt_frm_date = DateTime.Now;
            DateTime to_date = DateTime.Now;
            DateTime date = DateTime.Now;
            string CurrentMonthName = date.ToString("MMMM");
            string OldMonth1 = date.AddMonths(-1).ToString("MMMM");
            string OldMonth2 = date.AddMonths(-2).ToString("MMMM");
            if (CurrentMonthName == submit)
            {
                txt_frm_date = new DateTime(date.Year, date.Month, 1);
                to_date = DateTime.Now;

            }
            else if (OldMonth1 == submit)
            {
                txt_frm_date = new DateTime(date.AddMonths(-1).Year, date.AddMonths(-1).Month, 1);
                to_date = new DateTime(date.Year, date.Month, 1);
            }
            else
            {
                txt_frm_date = new DateTime(date.AddMonths(-2).Year, date.AddMonths(-2).Month, 1);
                to_date = new DateTime(date.AddMonths(-1).Year, date.AddMonths(-1).Month, 1);
            }
            model.GSTMaster = _db.GST_Report_master(Convert.ToDateTime(txt_frm_date), Convert.ToDateTime(to_date), userid).ToList();
            ViewBag.CurrentMonthName = CurrentMonthName;
            ViewBag.OldMonth1 = OldMonth1;
            ViewBag.OldMonth2 = OldMonth2;
            ViewBag.Crmonth = submit;

            return View(model);
        }

        #endregion

        #region Master Gst Invocing Report
        public ActionResult Gst_Invocing_Master_report()
        {
            var userid = User.Identity.GetUserId();
            DateTime txt_frm_date = DateTime.Now;
            DateTime to_date = DateTime.Now;
            DateTime date = DateTime.Now;
            string OldMonth = date.AddMonths(-1).ToString("MMMM");
            ViewBag.OldMonth = OldMonth;
            txt_frm_date = new DateTime(date.AddMonths(-1).Year, date.AddMonths(-1).Month, 1);
            to_date = new DateTime(date.Year, date.Month, 1);
            var show = _db.GST_Monthly_master(Convert.ToDateTime(txt_frm_date), Convert.ToDateTime(to_date)).Where(a => a.apiid == userid).ToList();
            return View(show);
        }
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

            var entries = _db.GST_Monthly_master(Convert.ToDateTime(txt_frm_date), Convert.ToDateTime(to_date)).Where(a => a.apiid == userid).ToList();
            ViewBag.name = entries.SingleOrDefault().SuperstokistName;
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
            ViewBag.firmname = entries.SingleOrDefault().FarmName;
            ViewBag.total = entries.SingleOrDefault().dmtcomm + entries.SingleOrDefault().dmtgst;
            var admininfo = _db.Admin_details.FirstOrDefault();
            ViewBag.cmpyname = admininfo.Companyname;
            ViewBag.adminaddress = admininfo.Address;
            ViewBag.adminpan = admininfo.pencardno;
            ViewBag.admingst = admininfo.Gstno;
            ViewBag.pancard = entries.SingleOrDefault().PanCard;
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
        public JsonResult ShowMasterprofile(string SSID)
        {
            var ch = _db.Superstokist_details.Where(a => a.SSId == SSID).ToList();
            return Json(ch.ToArray(), JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public ActionResult UpdateMasterProfile(string txtid1, string txtfrimname, string txtcity, string txtaddress, int txtzipcode, int State, int District)
        {
            try
            {
                var ad = _db.Superstokist_details.Single(a => a.SSId == txtid1);
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

        public ActionResult TargetHistory()
        {
            return View();
        }
        public JsonResult Showmobileandpancardprofile(string SSID)
        {
            var ch = _db.Superstokist_details.Where(a => a.SSId == SSID).ToList();
            return Json(ch.ToArray(), JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public ActionResult UpdatePanccardandmobile(string txtid2, string txtname, string txtaadhaarcard, string txtpancard, string txtgst, string ddlPosition, string ddlBusinessType)
        {
            try
            {
                var ad = _db.Superstokist_details.Single(a => a.SSId == txtid2);
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
        public JsonResult ShowBankinfo(string SSID)
        {
            var ch = _db.Superstokist_details.Where(a => a.SSId == SSID).ToList();
            return Json(ch.ToArray(), JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public ActionResult UpdateBankinfromation(string txtid3, string txtaccholder, string txtbankaccountno, string txtifsc, string txtbankname, string txtbranchaddress)
        {
            try
            {
                var ad = _db.Superstokist_details.Single(a => a.SSId == txtid3);
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
                var ad = _db.Superstokist_details.Single(a => a.SSId == txtaadharid);
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
                var ad = _db.Superstokist_details.Single(a => a.SSId == txtpancardid);
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
                var ad = _db.Superstokist_details.Single(a => a.SSId == txtserviceid);
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
                var ad = _db.Superstokist_details.Single(a => a.SSId == txtRegistractionid);
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
                var ad = _db.Superstokist_details.Single(a => a.SSId == txtAddressproofid);
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
                var ad = _db.Superstokist_details.Single(a => a.SSId == txtprofileid);
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
                var ad = _db.Superstokist_details.Single(a => a.SSId == SSID);
                ad.aadharcardPath = null;
                _db.SaveChanges();
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            if (SSID != null && Docname == "Pancard")
            {
                var ad = _db.Superstokist_details.Single(a => a.SSId == SSID);
                ad.pancardPath = null;
                _db.SaveChanges();
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            if (SSID != null && Docname == "ServiceAgrrement")
            {
                var ad = _db.Superstokist_details.Single(a => a.SSId == SSID);
                ad.serviceagreementpath = null;
                _db.SaveChanges();
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            if (SSID != null && Docname == "RegistractionCertificate")
            {
                var ad = _db.Superstokist_details.Single(a => a.SSId == SSID);
                ad.Registractioncertificatepath = null;
                _db.SaveChanges();
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            if (SSID != null && Docname == "AddressProof")
            {
                var ad = _db.Superstokist_details.Single(a => a.SSId == SSID);
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
        //Change Password 
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
                // change security code
                await UserManager.UpdateSecurityStampAsync(User.Identity.GetUserId());
                string userid = User.Identity.GetUserId();
                var chk22 = db.checklogouts.Where(a => a.userid == userid).SingleOrDefault();
                if (chk22 == null)
                {
                    checklogout chlogout = new checklogout();
                    chlogout.userid = userid;
                    chlogout.lastupdatedate = DateTime.UtcNow;
                    db.checklogouts.Add(chlogout);
                    db.SaveChanges();

                }
                else
                {
                    chk22.lastupdatedate = DateTime.UtcNow;
                    db.SaveChanges();
                }

                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                //var tokendelete = db.Reftkns.Where(aa => aa.Userid == userid).SingleOrDefault();
                //if (tokendelete != null)
                //{
                //    db.Reftkns.Remove(tokendelete);
                //    db.SaveChanges();
                //}

                TempData["Message"] = "Your Password has been Changed Successfully..";
                return RedirectToAction("ChangePassword");
            }
            AddErrors(result);
            return View(model);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
        [HttpPost]
        public async Task<ActionResult> InsertDealer(MASTER.Models.DealerModel model, string ddlrole)
        {
            var appDbContext = HttpContext.GetOwinContext().Get<ApplicationDbContext>();
            using (var transaction = appDbContext.Database.BeginTransaction())
            {
                try
                {
                    using (VastwebmultiEntities db = new VastwebmultiEntities())
                    {
                        var newlycreatedUserId = string.Empty;
                        try
                        {
                            ViewBag.state = new SelectList(db.Select_State_Details(), "State_Id", "State_Name");
                            //var hh = db.slab_name_list();
                            //ViewBag.slab = new SelectList(hh, "slab_name", "slab_name");
                            if (model.State != "0" && model.District != "0")
                            {
                                string MasterId = User.Identity.GetUserId();
                                var MasterEmailid = db.Superstokist_details.Where(p => p.SSId == MasterId).Single().Email;
                                var adminEmail = db.Admin_details.SingleOrDefault().email;
                                var chkmobile = db.Users.Where(a => a.PhoneNumber == model.Mobile).Any();
                                if (chkmobile == true)
                                {
                                    TempData["mobileno"] = "This Mobile Number Already Exists.";
                                    return RedirectToAction("Index");
                                }

                                var check = db.Dealer_Details.Where(es => es.Mobile == model.Mobile).Any();
                                if (check == false)
                                {
                                    bool includeLowercase = false;
                                    bool includeUppercase = false;
                                    bool includeNumeric = true;
                                    bool includeSpecial = false;
                                    bool includeSpaces = false;
                                    int lengthOfPassword = 4;

                                    string pingen = PasswordGenerator.GeneratePassword(includeLowercase, includeUppercase, includeNumeric, includeSpecial, includeSpaces, lengthOfPassword);

                                    while (!PasswordGenerator.PasswordIsValid(includeLowercase, includeUppercase, includeNumeric, includeSpecial, includeSpaces, pingen))
                                    {
                                        pingen = PasswordGenerator.GeneratePassword(includeLowercase, includeUppercase, includeNumeric, includeSpecial, includeSpaces, lengthOfPassword);
                                    }
                                    var enpin = Encrypt(pingen);



                                    var user = new ApplicationUser { UserName = model.Email, Email = model.Email, PhoneNumber = model.Mobile.ToString() };
                                    //Generate Random Password
                                    includeLowercase = true;
                                    includeUppercase = true;
                                    includeNumeric = true;
                                    includeSpecial = true;
                                    includeSpaces = false;
                                    lengthOfPassword = 8;

                                    string pass = PasswordGenerator.GeneratePassword(includeLowercase, includeUppercase, includeNumeric, includeSpecial, includeSpaces, lengthOfPassword);

                                    while (!PasswordGenerator.PasswordIsValid(includeLowercase, includeUppercase, includeNumeric, includeSpecial, includeSpaces, pass))
                                    {
                                        pass = PasswordGenerator.GeneratePassword(includeLowercase, includeUppercase, includeNumeric, includeSpecial, includeSpaces, lengthOfPassword);
                                    }


                                    var result = await UserManager.CreateAsync(user, pass);
                                    if (result.Succeeded)
                                    {
                                        newlycreatedUserId = user.Id;
                                        System.Data.Entity.Core.Objects.ObjectParameter output = new
                                 System.Data.Entity.Core.Objects.ObjectParameter("output", typeof(string));

                                        var ch = db.Insert_Dealer(MasterId, user.Id, model.Name, model.Firm, model.State.ToString(), model.District.ToString(), model.Mobile, model.Address, Convert.ToInt32(model.Pincode), model.Email, "", 0, "", model.Pan, model.Adhaar, model.Gst, "ADMIN", ddlrole, enpin, output).Single().msg;
                                        transaction.Commit();


                                        var statusMaster = db.PushNotificationStatus.Where(a => a.UserRole == "Master").SingleOrDefault().Status;
                                        var statusSendSmsMasterToDealerCreate = db.SMSSendAlls.Where(a => a.ServiceName == "DistributorCreateDlm").SingleOrDefault();
                                        var statusSendMailMasterToDealerCreate = db.EmailSendAlls.Where(a => a.ServiceName == "DistributorCreateDlm1").SingleOrDefault().Status;

                                        if (ch.Contains("SuccessFully"))
                                        {
                                            string passsss = HttpUtility.UrlEncode(pass);
                                            if (transaction.UnderlyingTransaction.Connection != null)
                                            {
                                                transaction.Commit();
                                            }
                                            //if (statusSendSmsMasterToDealerCreate == "Y")
                                            //{
                                            //    try
                                            //    {
                                            //        string msgssss = "";
                                            //        string tempid = "";
                                            //        string urlss = "";

                                            //        var smsapionsts = db.apisms.Where(x => x.sts == "Y").SingleOrDefault();
                                            //        var smsstypes = db.Sending_SMS_Templates.Where(x => x.SMS_TYPE == "ADMIN_CREATE_NEW_USERS" && x.SMSAPIID == smsapionsts.id).SingleOrDefault();
                                            //        if (smsstypes != null)
                                            //        {
                                            //            msgssss = string.Format(smsstypes.Templates, model.Mobile, passsss);
                                            //            tempid = smsstypes.Templateid;
                                            //            urlss = smsapionsts.smsapi;

                                            //            smssend.sendsmsallnew(model.Mobile, msgssss, urlss, tempid);
                                            //        }
                                            //    }
                                            //    catch { }
                                            //    //  smssend.sendsmsall(model.Mobile, "Dear Partner ! Welcome Your user Id " + model.Mobile + " and Password " + passsss + ". Thanks For Your Business . ", "User Create");
                                            //}

                                            smssend.sms_init(statusSendSmsMasterToDealerCreate.Status, statusSendSmsMasterToDealerCreate.Whatsapp_Status, "ADMIN_CREATE_NEW_USERS", model.Mobile, model.Mobile + " ", passsss);

                                            if (statusSendMailMasterToDealerCreate == "Y")
                                            {
                                                smssend.SendEmailAll(model.Email, "Dear Partner ! Welcome Your user Id " + model.Mobile + " and Password " + passsss + ". Thanks For Your Business . ", "User Create", adminEmail);
                                            }
                                            TempData["msgrem"] = ch;
                                            //return RedirectToAction("Index");
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
                                            //transaction.Rollback();
                                            TempData["mobileno"] = "User Not Created. Please Create After Some Time.";
                                            return RedirectToAction("Index");
                                        }
                                        // Send an email with this link
                                        string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                                        var callbackUrl = Url.Action("ConfirmEmailAdmin", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                                        callbackUrl = callbackUrl.Replace("/MASTER", "");
                                        string body = new CommonUtil().PopulateBodyDealer("", "Confirm your account", "", "" + callbackUrl + "", model.Email, pass);
                                        string Welcomebody = new CommonUtil().PopulateBodyWelcome("", "Confirm your account", "", "" + callbackUrl + "", model.Email, pass);
                                        new CommonUtil().Insertsendmail(model.Email, "Confirm your account", body, callbackUrl);
                                        new CommonUtil().InsertsendmailWelcome(model.Email, "Confirm your account", Welcomebody, callbackUrl);
                                        return RedirectToAction("Index");
                                    }
                                    TempData["emailconfrim"] = "Your Email id is Allready Exist.";
                                    return RedirectToAction("Index");
                                }
                                else
                                {
                                    TempData["mobileno"] = "This Mobile Number Already Exists.";
                                    return RedirectToAction("Index");
                                }
                            }
                            else
                            {
                                ViewBag.state = new SelectList(db.Select_State_Details(), "State_Id", "State_Name").ToList();
                                return RedirectToAction("Index");
                            }
                        }
                        catch
                        {
                            transaction.UnderlyingTransaction.Connection.Open();
                            transaction.Rollback();
                            TempData["Error"] = "User Not Created. Please Create After Some Time.";
                            return RedirectToAction("Index");
                        }
                    }

                }
                catch
                {
                    transaction.UnderlyingTransaction.Connection.Open();
                    transaction.Rollback();
                    TempData["Error"] = "User Not Created. Please Create After Some Time.";
                    return RedirectToAction("Index");
                }
            }
        }
        [HttpPost]
        public ActionResult Edit_Distibutor_user(MASTER.Models.DealerModel dlm)
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                string masterid = User.Identity.GetUserId();
                Dealer_Details objCourse = (from p in db.Dealer_Details
                                            where p.DealerId == dlm.Dealerid
                                            select p).SingleOrDefault();
                objCourse.Address = dlm.Address1;
                objCourse.adharcard = dlm.Adhaar1;
                objCourse.DealerName = dlm.Name1;
                objCourse.District = Convert.ToInt32(dlm.District1);
                objCourse.FarmName = dlm.Firm1;
                objCourse.gst = dlm.Gst1;
                objCourse.pancard = dlm.Pan1;
                objCourse.Pincode = Convert.ToInt32(dlm.Pincode1);
                //objCourse.slab_name = dlm.Slab1;
                objCourse.State = Convert.ToInt32(dlm.State1);
                db.SaveChanges();
                var marginsetmaster = db.master_to_dlm_margine.Where(x => x.dlm == dlm.Dealerid && x.masterid == masterid).FirstOrDefault();
                marginsetmaster.marginecomm = dlm.offmargineEditn;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
        }
        //POST : Delaer Search
        [HttpPost]
        public ActionResult DealerSearch(string userid)
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                var ch = db.Dealer_Details.Where(aa => aa.DealerId == userid).ToList();
                return Json(ch.ToArray(), JsonRequestBehavior.AllowGet);
            }
        }

        //fill District 
        public JsonResult DistrictList(int Id)
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                var district = from s in db.District_Desc
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
            ResultSetViewModel viewModel = new ResultSetViewModel();
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                var userid = User.Identity.GetUserId();
                viewModel.ResultSet = db.Slab_name.Where(aa => aa.createdby == userid).Select(aaa => new Slab_name_model
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
        public ActionResult AddSlabname(ResultSetViewModel result)
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                var userid = User.Identity.GetUserId();
                try
                {
                    System.Data.Entity.Core.Objects.ObjectParameter output = new
                      System.Data.Entity.Core.Objects.ObjectParameter("Output", typeof(string));
                    var msg = db.insert_slab_list("Distributor", result.AccountVM.Slab_Name, userid, output).Single().slab;
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
                    var disresult = db.Dealer_Details.Where(p => p.slab_name == slabname).ToList();
                    if (disresult.Count > 0)
                    {
                        TempData["api"] = "This slab is Already Assign To Distributor User..";
                    }
                    else
                    {
                        var msg = db.Delete_slab(slabfor, slabname, userid);
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


                var stands = (from dlm in db.Dealer_Details where dlm.SSId == masterid select dlm).ToList();
                IEnumerable<SelectListItem> selectList = from s in stands
                                                         select new SelectListItem
                                                         {
                                                             Value = s.DealerId,
                                                             Text = s.Email + "--" + s.DealerName.ToString()
                                                         };
                ViewBag.DealerId = new SelectList(selectList, "Value", "Text");
                ViewBag.DealerId1 = new SelectList(selectList, "Value", "Text");
                var ch = db.select_admin_to_Dealer(masterid, "ALL", Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
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
                var stands = (from dlm in db.Dealer_Details where dlm.SSId == userid select dlm).ToList();
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
                var ch = db.select_admin_to_Dealer(userid, DealerId1, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();

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
                    var email = db.Users.Where(p => p.UserId == masterid).Single().Email;
                    var useremail = db.Users.Where(p => p.UserId == DealerId).Single().Email;
                    var diff1 = (db.admin_to_dealer.Where(aa => aa.dealerid == DealerId && aa.balance_from == masterid).OrderByDescending(aa => aa.idno).Select(c => c.cr).FirstOrDefault());
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
                            ch = "";// db.Insert_SuperStokist_To_Dealer(masterid, DealerId, amount, 0, ddl_fund_type, comment,"","","","","", output).Single().msg;
                        }
                        var AdminDetails = db.Admin_details.Single();
                        if (ch == "Balance Transfer SuccessFully.")
                        {
                            var diff2 = (db.admin_to_dealer.Where(aa => aa.dealerid == DealerId && aa.balance_from == masterid).OrderByDescending(aa => aa.idno).Select(c => c.cr).FirstOrDefault());
                            diff2 = diff2 ?? 0;
                            var remaindealer = db.Remain_dealer_balance.Where(p => p.DealerID == DealerId).Single().Remainamount;
                            var statusMaster = db.PushNotificationStatus.Where(a => a.UserRole == "Master").SingleOrDefault().Status;
                            var statusDealer = db.PushNotificationStatus.Where(a => a.UserRole == "Distributor").SingleOrDefault().Status;
                            var statusSendSmsMasterToDlmFundTransfer = db.SMSSendAlls.Where(a => a.ServiceName == "MDToDLMdlmFundTrans").SingleOrDefault();
                            var statusSendMailMasterToDlmFundTransfer = db.EmailSendAlls.Where(a => a.ServiceName == "MDToDLMdlmFundTrans1").SingleOrDefault().Status;

                            var DealerDetails = db.Dealer_Details.Where(p => p.DealerId == DealerId).Single();
                            var statusSendSmsMasterToDlmFundTransferMaster = db.SMSSendAlls.Where(a => a.ServiceName == "MDToDLMMDFundTrans").SingleOrDefault();
                            var statusSendMailMasterToDlmFundTransferMaster = db.EmailSendAlls.Where(a => a.ServiceName == "MDToDLMMDFundTrans1").SingleOrDefault().Status;

                            var MasterDetails = db.Superstokist_details.Where(p => p.SSId == masterid).Single();
                            if (ddl_fund_type == "Credit")
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
                                //    // smssend.sendsmsall(DealerDetails.Mobile, "Credit Received by "+ email + " Rs." + amount + ".New Balance is " + remaindealer + ".Your O/s Credit is " + diff2 + "", "Fund Transfer");
                                //}

                                smssend.sms_init(statusSendSmsMasterToDlmFundTransfer.Status, statusSendSmsMasterToDlmFundTransfer.Whatsapp_Status, "SEND_SMS_CREDIT_RECEIVEDBY", DealerDetails.Mobile, email, amount, remaindealer, diff2);

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

                                //            msgssss = string.Format(smsstypes.Templates, useremail, amount, diff1);
                                //            tempid = smsstypes.Templateid;
                                //            urlss = smsapionsts.smsapi;

                                //            smssend.sendsmsallnew(MasterDetails.Mobile, msgssss, urlss, tempid);
                                //        }
                                //    }
                                //    catch { }


                                //    //  smssend.sendsmsall(MasterDetails.Mobile, "Credit Transferred To " + useremail + " Rs. " + amount + ".Total Credit(O/s) Balance is " + diff1 + "", "Fund Transfer");
                                //}

                                smssend.sms_init(statusSendSmsMasterToDlmFundTransferMaster.Status, statusSendSmsMasterToDlmFundTransferMaster.Whatsapp_Status, "SEND_SMS_CREDIT_TRANSFERREDTO", MasterDetails.Mobile, useremail, amount, diff1);

                                if (statusSendMailMasterToDlmFundTransfer == "Y")
                                {
                                    smssend.SendEmailAll(DealerDetails.Email, "Credit Received by " + email + " Rs." + amount + ".New Balance is " + remaindealer + ".Your O/s Credit is " + diff2 + "", "Fund Transfer", AdminDetails.email);
                                }
                                if (statusSendSmsMasterToDlmFundTransferMaster.Status == "Y")
                                {
                                    smssend.SendEmailAll(MasterDetails.Email, "Credit Transferred To " + useremail + " Rs. " + amount + ".Total Credit(O/s) Balance is " + diff1 + "", "Fund by Master To CC", AdminDetails.email);
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
                                var mastername = db.Superstokist_details.Where(p => p.SSId == masterid).Single().SuperstokistName;
                                var dealername = db.Dealer_Details.Where(p => p.DealerId == DealerId).Single().DealerName;
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

                                //            smssend.sendsmsallnew(DealerDetails.Mobile, msgssss, urlss, tempid);
                                //        }
                                //    }
                                //    catch { }
                                //    //   smssend.sendsmsall(DealerDetails.Mobile, "Cash Paid Rs." + amount + " to " + mastername + ". New Balance is " + remaindealer + ". Your O/s Credit is " + diff2 + "", "Fund Transfer");
                                //}

                                smssend.sms_init(statusSendSmsMasterToDlmFundTransfer.Status, statusSendSmsMasterToDlmFundTransfer.Whatsapp_Status, "FUNDTRANSFERMESSAGE_CASH", DealerDetails.Mobile, amount, mastername, remaindealer, diff2);

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

                                //            msgssss = string.Format(smsstypes.Templates, amount, dealername, diff2);
                                //            tempid = smsstypes.Templateid;
                                //            urlss = smsapionsts.smsapi;

                                //            smssend.sendsmsallnew(MasterDetails.Mobile, msgssss, urlss, tempid);
                                //        }
                                //    }
                                //    catch { }
                                //    //  smssend.sendsmsall(MasterDetails.Mobile, "Cash Recived Rs." + amount + " From " + dealername + ",his O/s Credit is " + diff2 + "", "Fund Transfer");
                                //}

                                smssend.sms_init(statusSendSmsMasterToDlmFundTransferMaster.Status, statusSendSmsMasterToDlmFundTransferMaster.Whatsapp_Status, "SENDSMSMASTERTODLMFUNDTRANSFERMSGCASH", MasterDetails.Mobile, amount, dealername, diff2);

                                if (statusSendMailMasterToDlmFundTransfer == "Y")
                                {
                                    smssend.SendEmailAll(DealerDetails.Email, "Cash Paid Rs." + amount + " to " + mastername + ". New Balance is " + remaindealer + ". Your O/s Credit is " + diff2 + "", "Fund Transfer", AdminDetails.email);
                                }
                                if (statusSendMailMasterToDlmFundTransferMaster == "Y")
                                {
                                    smssend.SendEmailAll(MasterDetails.Email, "Cash Recived Rs." + amount + " From " + dealername + ",his O/s Credit is " + diff2 + "", "Fund by Master To CC", AdminDetails.email);
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


            var stands = (from dlm in db.Dealer_Details where dlm.SSId == masterid select dlm).ToList();
            IEnumerable<SelectListItem> selectList = from s in stands
                                                     select new SelectListItem
                                                     {
                                                         Value = s.DealerId,
                                                         Text = s.FarmName + "--" + s.Email.ToString()
                                                     };
            ViewBag.DealerId = new SelectList(selectList, "Value", "Text");
            //vmodel.ddldealer= new SelectList(selectList, "Value", "Text");
            ViewBag.DealerId1 = new SelectList(selectList, "Value", "Text");
            vmodel.mastertodlmlist = db.select_admin_to_Dealer(masterid, "ALL", Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            List<SelectListItem> items = new List<SelectListItem>();
            foreach (var item in stands)
            {
                items.Add(new SelectListItem { Text = item.FarmName + " / " + item.Mobile, Value = item.DealerId.ToString() });
            }
            vmodel.ddldealers = items;
            var bindbank = db.bank_info.Where(x => x.userid == masterid).ToList();
            List<SelectListItem> bankitem = new List<SelectListItem>();
            foreach (var bank in bindbank)
            {
                bankitem.Add(new SelectListItem { Text = bank.banknm, Value = bank.acno });
            }
            vmodel.ddlFillAllBank = bankitem;
            var bindwallet = db.tblwallet_info.Where(x => x.userid == masterid).ToList();
            List<SelectListItem> walletitem = new List<SelectListItem>();
            foreach (var wallet in bindwallet)
            {
                walletitem.Add(new SelectListItem { Text = wallet.walletname, Value = wallet.walletno });
            }
            vmodel.ddlFillAllwallet = walletitem;



            return View(vmodel);
            //  return View();
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
            // vmodel.master_to_dealer_report_BYAdmin_Result = null;
            // vmodel.dealer_to_rem_fund_report = null;
            string masterid = User.Identity.GetUserId();
            //  vmodel.mastertodlmlist = db.select_admin_to_Dealer(masterid, "ALL", DateTime.Now.AddDays(-1), DateTime.Now.AddDays(1)).ToList();
            switch (tabtype)
            {
                case "Dealer":
                    vmodel.funrequesttoadmin = _db.select_master_pur_order(masterid, "ALL", fromdate, Todate).ToList();
                    vmodel.AdminCompanyname = db.Admin_details.FirstOrDefault().Companyname;
                    break;

                case "Retailer":
                    //  vmodel.funrequesttoadmin = _db.select_master_pur_order(masterid, "ALL", DateTime.Now.AddDays(-1), DateTime.Now.AddDays(1)).ToList();

                    vmodel.FundRequestRecived = _db.select_dlm_pur_order(usernm, masterid, fromdate, Todate).ToList();

                    break;
                case "Recivefunddetails":

                    vmodel.FundRecievedDetails = db.Select_balance_Super_stokist(masterid, fromdate, Todate).ToList();
                    break;
                default:
                    vmodel.mastertodlmlist = db.select_admin_to_Dealer(masterid, usernm, fromdate, Todate).ToList();
                    break;

            }

            return PartialView("_FundTransferMasterToDlmPartial", vmodel);

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

            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                try
                {

                    WebImage photo = null;
                    var newFileName = "";
                    var imagePath = "";

                    //if (Request.HttpMethod == "POST")
                    //{
                    //    photo = WebImage.GetImageFromRequest();
                    //    if (photo != null)
                    //    {
                    //        newFileName = Guid.NewGuid().ToString() + "_" +
                    //                      Path.GetFileName(photo.FileName);
                    //        imagePath = @"PurchaseOrderImg\" + newFileName;

                    //        photo.Save(@"~\" + imagePath);
                    //    }
                    //}



                    string userid = User.Identity.GetUserId();
                    var masterid = db.Superstokist_details.Where(aa => aa.SSId == userid).SingleOrDefault().SSId;
                    var dealercount = db.Superstokist_details.Where(aa => aa.SSId == userid).Count();
                    //if (db.master_purchase.Count(aa => aa.masterid.ToLower() == userid.ToLower() && aa.sts == "Pending") < 1)
                    //{
                    var diff1 = (_db.admin_to_super_balance.Where(aa => aa.SuperStokistID == userid).OrderByDescending(aa => aa.ID).Select(c => c.cr).FirstOrDefault());
                    diff1 = diff1 ?? 0;
                    decimal diff =


                        Convert.ToDecimal(diff1);
                    var amount = Convert.ToDecimal(balance);
                    decimal disCharge = 0;
                    if (amount > 0)
                    {
                        if (dealercount > 0)
                        {
                            creditchargemaster charges = null;
                            // if (hdPaymentMode == "Credit")
                            // {
                            charges = db.creditchargemasters.Where(aa => aa.userid == userid).FirstOrDefault(aa => aa.type == hdPaymentMode);
                            // }

                            if (hdPaymentMode == "Branch / CMS Deposite")
                            {
                                charges = db.creditchargemasters.Where(aa => aa.userid == userid).FirstOrDefault(aa => aa.type == "Cash");
                            }
                            if (charges != null)
                            {
                                disCharge = (amount * charges.charge.Value) / 100;
                            }









                            db.insert_masterpurchageorder(masterid, hdPaymentMode, collectionby, bankname, "", hdMDComments, Convert.ToDecimal(balance), "", "", "", adminacco, "", "", "", "", disCharge, amount - disCharge);

                            var md_data = db.Superstokist_details.Where(s => s.SSId == userid).SingleOrDefault();
                            var md_name = md_data.SuperstokistName;
                            var md_no = md_data.Mobile;
                            string apiurls = "";




                            if (db.apisms.Any(s => s.sts == "Y"))
                            {
                                var asd = db.SMSSendAlls.Where(s => s.ServiceName == "mastertoadminfundtrans1" && s.Whatsapp_Status == "Y").ToList();
                                var smsapi = db.apisms.Where(x => x.sts == "Y").ToList();
                                var mobile12 = db.Admin_details.SingleOrDefault().mobile;
                                var smsapionsts = smsapi.Where(s => s.api_type == "whatsapp").SingleOrDefault();

                                if (smsapionsts != null)
                                {
                                    if (asd.Any())
                                    {
                                        apiurls = smsapionsts.smsapi;
                                        string text = md_name + "-" + md_data.FarmName + "(" + md_no + ") is Requested to you for perchase order of " + hdPaymentAmount + "Rs";
                                        text = string.Format(text, "1230");

                                        var apinamechange = apiurls.Replace("tttt", mobile12).Replace("mmmm", text);

                                        var client = new RestClient(apinamechange);
                                        var request = new RestRequest(Method.GET);

                                        VastBazaartoken Responsetoken = new VastBazaartoken();
                                        var whatsts = db.Email_show_passcode.SingleOrDefault();
                                        var userwise = db.retailerwise_whatsappsts.Where(a => a.userid == userid).ToList();
                                        if (apinamechange.ToUpper().Contains("API.VASTBAZAAR.COM/API/WEB/WHATSAPPMSG") && whatsts.whatsappapists == true && userwise[0].sts == true)
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
                                        sms.messagefor = userid;
                                        db.sms_api_entry.Add(sms);
                                        db.SaveChanges();
                                    }
                                }
                                var asd1 = db.SMSSendAlls.Where(s => s.ServiceName == "mastertoadminfundtrans1" && s.Status == "Y").ToList();
                                var smsapionsts1 = smsapi.Where(s => s.api_type == "sms").SingleOrDefault();
                                if (smsapionsts1 != null)
                                {
                                    if (asd1.Any())
                                    {
                                        apiurls = smsapionsts1.smsapi;
                                        string text = md_name + "-" + md_data.FarmName + "(" + md_no + ") is Requested to you for perchase order of " + hdPaymentAmount + "Rs";
                                        text = string.Format(text, "1230");

                                        var apinamechange = apiurls.Replace("tttt", mobile12).Replace("mmmm", text);

                                        var client = new RestClient(apinamechange);
                                        var request = new RestRequest(Method.GET);

                                        VastBazaartoken Responsetoken = new VastBazaartoken();
                                        var whatsts = db.Email_show_passcode.SingleOrDefault();
                                        var userwise = db.retailerwise_whatsappsts.Where(a => a.userid == userid).ToList();
                                        if (apinamechange.ToUpper().Contains("API.VASTBAZAAR.COM/API/WEB/WHATSAPPMSG") && whatsts.whatsappapists == true && userwise[0].sts == true)
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
                                        sms.messagefor = userid;
                                        db.sms_api_entry.Add(sms);
                                        db.SaveChanges();

                                    }
                                }
                            }

                            var emailcheck = db.EmailSendAlls.Where(s => s.ServiceName == "mastertoadminfundtrans1" && s.Status == "Y").ToList();

                            if (emailcheck.Any())
                            {
                                var AdminDetails = db.Admin_details.SingleOrDefault();
                                smssend.SendEmailAll(AdminDetails.email, md_name + "-" + md_data.FarmName + "(" + md_no + ") is Requested to you for perchase order of " + hdPaymentAmount + "Rs", "Purchase Order Request", AdminDetails.email);

                              

                            }




                            return Json("SuccessFully", JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json("Masterid not found", JsonRequestBehavior.AllowGet);
                        }
                    }

                    else
                    {
                        TempData["error"] = "Amount should be Grater then 100";
                        return Json("Amount should be not zero", JsonRequestBehavior.AllowGet);
                        //  return RedirectToAction("PurchaseOrder");

                    }

                    //}
                    //else
                    //{
                    //    TempData["error"] = "Your purcharge Order Allready Pending.";
                    //    return Json("Your purcharge Order Allready Pending.", JsonRequestBehavior.AllowGet);

                    //}
                    //return RedirectToAction("PurchaseOrder");
                }
                catch (Exception ex)
                {
                    TempData["error"] = ex;
                    return Json(ex, JsonRequestBehavior.AllowGet);
                    // return RedirectToAction("PurchaseOrder");
                }
            }

            return Json("", JsonRequestBehavior.AllowGet);
        }

        private static string GenerateUniqueTransectionID()
        {
            bool includeLowercase = false;
            bool includeUppercase = true;
            bool includeNumeric = true;
            bool includeSpecial = false;
            bool includeSpaces = false;
            int lengthOfPassword = 10;

            string transferids = PasswordGenerator.GeneratePassword(includeLowercase, includeUppercase, includeNumeric, includeSpecial, includeSpaces, lengthOfPassword);
            while (!PasswordGenerator.PasswordIsValid(includeLowercase, includeUppercase, includeNumeric, includeSpecial, includeSpaces, transferids))
            {
                transferids = PasswordGenerator.GeneratePassword(includeLowercase, includeUppercase, includeNumeric, includeSpecial, includeSpaces, lengthOfPassword);
            }

            return transferids;
        }

        public JsonResult Fundtransfer_Adminmd_to_dlm_Generate_Unique_ID()
        {
            var adminuserid = User.Identity.GetUserId();

            string transferids = GenerateUniqueTransectionID();
            var transferid = "MD" + transferids;



            TempData["transferMDtoDlm"] = transferid;
            return Json(transferid, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult FundTransfermaster_to_Dealer_bal(string txtcode, string hdSuperstokistID, string hdMDDLM, string hdPaymentMode,
      string hdPaymentAmount, string hdMDDepositeSlipNo, string hdMDTransferType, string hdMDcollection, string hdMDComments,
      string hdMDBank, string hdsupraccno, string hdMDaccountno, string hdMDutrno, string hdMDwallet,
      string hdMDwalletno, string hdMDtransationno, string hdMDsettelment, string hdMDCreditDetail, string hdMDsubject)
        {
            try
            {
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    string superstockid = hdSuperstokistID;

                    string masterid = User.Identity.GetUserId();
                    // var dealeremail = db.Users.Where(p => p.UserId == DealerId).Single().Email;
                    string balance = hdPaymentAmount;
                    string type = hdPaymentMode;
                    string comment = hdMDComments;
                    var DealerId = hdMDDLM;
                    string transferid = null;
                    try
                    {
                        transferid = TempData["transferMDtoDlm"].ToString();
                    }
                    catch (Exception ex)
                    {
                        return Json("Please Try After SomeTime", JsonRequestBehavior.AllowGet);
                    }
                    var counts = db.FundTransfercount(masterid, hdMDDLM, type, Convert.ToDecimal(balance), transferid, "Admintodealer").SingleOrDefault().msg;
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
                        var diff1 = (db.admin_to_dealer.Where(aa => aa.dealerid == DealerId && aa.balance_from == masterid).OrderByDescending(aa => aa.idno).Select(c => c.cr).FirstOrDefault());
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
                            ch = db.Insert_SuperStokist_To_Dealer(masterid, DealerId, amount, 0, hdPaymentMode, comment, "Master", hdMDcollection, hdMDBank, hdMDaccountno, "Direct", transferid, output).Single().msg;
                            // }
                            try
                            {
                                var dealerdetails = db.Dealer_Details.Where(aa => aa.DealerId == DealerId).SingleOrDefault();
                                var masterdetails = db.Superstokist_details.Where(aa => aa.SSId == masterid).SingleOrDefault();

                                var dlmdetails = db.Remain_dealer_balance.Where(aa => aa.DealerID == DealerId).SingleOrDefault();
                                var Masterdetails = db.Remain_superstokist_balance.Where(aa => aa.SuperStokistID == masterid).SingleOrDefault();

                                var admininfo = db.Admin_details.SingleOrDefault();
                                Backupinfo back = new Backupinfo();
                          

                                var model1 = new Backupinfo.Addinfo
                                {
                                    Websitename = admininfo.WebsiteUrl,
                                    RetailerID = DealerId,
                                    Email = dealerdetails.Email,
                                    Mobile = dealerdetails.Mobile,
                                    Details = "Fund Recived From Master ",

                                    RemainBalance = Convert.ToDecimal(dlmdetails.Remainamount),
                                    Usertype = "Dealer"
                                };
                                back.Fundtransfer(model1);

                                var model2 = new Backupinfo.Addinfo
                                {
                                    Websitename = admininfo.WebsiteUrl,
                                    RetailerID = masterid,
                                    Email = masterdetails.Email,
                                    Mobile = masterdetails.Mobile,
                                    Details = "Fund Transfer to Dealer ",
                                    RemainBalance = Convert.ToDecimal(Masterdetails.Remainamount),
                                    Usertype = "Master"
                                };
                                back.Fundtransfer(model2);
                            }
                            catch { }

                            var Admindetails = db.Admin_details.Single();
                            if (ch == "Balance Transfer SuccessFully.")
                            {
                                var diff2 = (db.admin_to_dealer.Where(aa => aa.dealerid == DealerId && aa.balance_from == masterid).OrderByDescending(aa => aa.idno).Select(c => c.cr).FirstOrDefault());
                                diff2 = diff2 ?? 0;
                                var remaindealer = db.Remain_dealer_balance.Where(p => p.DealerID == DealerId).Single().Remainamount;
                                var statusMaster = db.PushNotificationStatus.Where(a => a.UserRole == "Master").SingleOrDefault().Status;
                                var statusDealer = db.PushNotificationStatus.Where(a => a.UserRole == "Distributor").SingleOrDefault().Status;
                                var statusSendSmsMasterToDlmFundTransfer = db.SMSSendAlls.Where(a => a.ServiceName == "MDToDLMdlmFundTrans").SingleOrDefault();
                                var statusSendMailMasterToDlmFundTransfer = db.EmailSendAlls.Where(a => a.ServiceName == "MDToDLMdlmFundTrans1").SingleOrDefault().Status;

                                var DealerDetails = db.Dealer_Details.Where(p => p.DealerId == DealerId).Single();
                                var statusSendSmsMasterToDlmFundTransferMaster = db.SMSSendAlls.Where(a => a.ServiceName == "MDToDLMMDFundTrans").SingleOrDefault();
                                var statusSendMailMasterToDlmFundTransferMaster = db.EmailSendAlls.Where(a => a.ServiceName == "MDToDLMMDFundTrans1").SingleOrDefault().Status;

                                var MasterDetails = db.Superstokist_details.Where(p => p.SSId == masterid).Single();
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

                                    smssend.sms_init(statusSendSmsMasterToDlmFundTransfer.Status, statusSendSmsMasterToDlmFundTransfer.Whatsapp_Status, "SEND_SMS_CREDIT_RECEIVEDBY", DealerDetails.Mobile, email, amount, remaindealer, diff2);

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

                                    smssend.sms_init(statusSendSmsMasterToDlmFundTransferMaster.Status, statusSendSmsMasterToDlmFundTransferMaster.Whatsapp_Status, "SEND_SMS_CREDIT_TRANSFERREDTO", MasterDetails.Mobile, useremail, amount, diff2);

                                    if (statusSendMailMasterToDlmFundTransfer == "Y")
                                    {
                                        smssend.SendEmailAll(DealerDetails.Email, "Credit Received by " + email + " Rs." + amount + ".New Balance is " + remaindealer + ".Your O/s Credit is " + diff2 + "", "Fund Transfer", Admindetails.email, 1000);
                                    }
                                    if (statusSendMailMasterToDlmFundTransferMaster == "Y")
                                    {
                                        smssend.SendEmailAll(MasterDetails.Email, "Credit Transferred To " + useremail + " Rs. " + amount + ".Total Credit(O/s) Balance is " + diff1 + "", "Fund by Master To CC", Admindetails.email, 1000);
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
                                    var mastername = db.Superstokist_details.Where(p => p.SSId == masterid).Single().SuperstokistName;
                                    var dealername = db.Dealer_Details.Where(p => p.DealerId == DealerId).Single().DealerName;
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

                                    smssend.sms_init(statusSendSmsMasterToDlmFundTransfer.Status, statusSendSmsMasterToDlmFundTransfer.Whatsapp_Status, "FUNDTRANSFERMESSAGE_CASH", MasterDetails.Mobile, amount, mastername, remaindealer, diff2);

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

                                    smssend.sms_init(statusSendSmsMasterToDlmFundTransferMaster.Status, statusSendSmsMasterToDlmFundTransferMaster.Whatsapp_Status, "SENDSMSMASTERTODLMFUNDTRANSFERMSGCASH", MasterDetails.Mobile, amount, dealername, remaindealer, diff2);

                                    if (statusSendMailMasterToDlmFundTransfer == "Y")
                                    {
                                        smssend.SendEmailAll(DealerDetails.Email, "Cash Paid Rs." + amount + " to " + mastername + ". New Balance is " + remaindealer + ". Your O/s Credit is " + diff2 + "", "Fund Transfer", Admindetails.email, 1000);
                                    }
                                    if (statusSendMailMasterToDlmFundTransferMaster == "Y")
                                    {
                                        smssend.SendEmailAll(MasterDetails.Email, "Cash Recived Rs." + amount + " From " + dealername + ",his O/s Credit is " + diff2 + "", "Fund by Master To CC", Admindetails.email, 1000);
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
                        //  return RedirectToAction("SendFund");
                    }
                    else
                    {
                        return Json("Please Try After 1 min ", JsonRequestBehavior.AllowGet);
                    }

                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
                // return RedirectToAction("SendFund");
            }
        }



        public ActionResult D_Creditchk(string dealerid)
        {
            string masterid = User.Identity.GetUserId();
            decimal? dlmbal;
            var ch = (_db.admin_to_dealer.Where(aa => aa.dealerid == dealerid && aa.balance_from == masterid).OrderByDescending(aa => aa.idno).Select(c => c.cr).FirstOrDefault());
            ch = ch ?? 0;

            if (string.IsNullOrEmpty(dealerid))
            {
                dlmbal = 0;
            }
            else
            {
                dlmbal = db.Remain_dealer_balance.Where(x => x.DealerID == dealerid).SingleOrDefault().Remainamount;
            }

            return Json(new { currntcr = ch, rembal = dlmbal }, JsonRequestBehavior.AllowGet);
            //  return Json(ch, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MyOLDCreditChk()
        {
            string masterid = User.Identity.GetUserId();
            var diff1 = (_db.admin_to_super_balance.Where(aa => aa.SuperStokistID == masterid).OrderByDescending(aa => aa.ID).Select(c => c.cr).FirstOrDefault());
            diff1 = diff1 ?? 0;
            var bindbank = db.bank_info.Where(x => x.userid == "Admin").ToList();
            List<SelectListItem> bankitem = new List<SelectListItem>();
            foreach (var bank in bindbank)
            {
                bankitem.Add(new SelectListItem { Text = bank.banknm + " / " + bank.holdername, Value = bank.acno });
            }
            var bindwallet = db.tblwallet_info.Where(x => x.userid == "Admin").ToList();
            List<SelectListItem> walletitem = new List<SelectListItem>();
            foreach (var wallet in bindwallet)
            {
                walletitem.Add(new SelectListItem { Text = wallet.walletname, Value = wallet.walletno });
            }

            return Json(new { diff = diff1, listbank = bankitem, walletinfo = walletitem }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult updatepurchage_dlmqwwwwwwww(int? hdidno, string hdtype, string txtcommentwrite)
        {

            if (hdtype == "APP")
            {
                hdtype = "Approved";
            }
            else
            {
                hdtype = "rejected";
            }
            var payinfo = _db.dlm_purchage.Where(a => a.id == hdidno.Value).SingleOrDefault();
            var sts= payinfo.sts.ToUpper();
            if (sts == "PENDING")
            {
                System.Data.Entity.Core.Objects.ObjectParameter output = new
         System.Data.Entity.Core.Objects.ObjectParameter("Output", typeof(string));
                var ch = _db.update_dlm_purchage(Convert.ToInt32(hdidno), hdtype, 0, txtcommentwrite, output).Single().msg;
                try
                {
                   var dealerdetails = db.Dealer_Details.Where(aa => aa.DealerId == payinfo.dlmid).SingleOrDefault();
                    var masterdetails = db.Superstokist_details.Where(aa => aa.SSId == payinfo.frm).SingleOrDefault();

                    var dlmdetails = db.Remain_dealer_balance.Where(aa => aa.DealerID == payinfo.dlmid).SingleOrDefault();
                    var Masterdetails = db.Remain_superstokist_balance.Where(aa => aa.SuperStokistID == payinfo.frm).SingleOrDefault();

                    var admininfo = db.Admin_details.SingleOrDefault();
                    Backupinfo back = new Backupinfo();
                   

                    var model1 = new Backupinfo.Addinfo
                    {
                        Websitename = admininfo.WebsiteUrl,
                        RetailerID = payinfo.dlmid,
                        Email = dealerdetails.Email,
                        Mobile = dealerdetails.Mobile,
                        Details = "Fund Recived From Master ",

                        RemainBalance = Convert.ToDecimal(dlmdetails.Remainamount),
                        Usertype = "Dealer"
                    };
                    back.Fundtransfer(model1);

                    var model2 = new Backupinfo.Addinfo
                    {
                        Websitename = admininfo.WebsiteUrl,
                        RetailerID = masterdetails.SSId,
                        Email = masterdetails.Email,
                        Mobile = masterdetails.Mobile,
                        Details = "Fund Transfer to Dealer ",
                        RemainBalance = Convert.ToDecimal(Masterdetails.Remainamount),
                        Usertype = "Master"
                    };
                    back.Fundtransfer(model2);
                }
                catch { }
                if (ch == "Balance Transfer SuccessFully.")
                {
                    TempData["successorder"] = ch;
                }
                else
                {
                    TempData["failedorder"] = ch;
                }

            }

            //return RedirectToAction("purcharge_request");

            //  var url= Url.Action("MDTODealer", "Home", new RouteValueDictionary(new { tabtype = "Retailer" }), HttpContext.Request.Url.Scheme, HttpContext.Request.Url.Authority);
            return MDTODealer("Retailer", "", "", "");

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
                    var ch = db.Select_balance_Super_stokist(masterid, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
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
                    var masterlist = (from ma in db.Master_details select ma);
                    ViewBag.masterid1 = new SelectList(masterlist, "msterid", "msterid");
                    ViewBag.masterid = new SelectList(masterlist, "msterid", "msterid");
                    var ch = db.Select_balance_Super_stokist(masterid, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
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
                    var entries = db.PurchaseOrderCashDepositCharges.ToList();
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

                    if (db.master_purchase.Count(aa => aa.masterid.ToLower() == userid.ToLower() && aa.sts == "Pending") < 1)
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
                                db.insert_masterpurchageorder(userid, Paymode, utrno, "", "", Comment, Convert.ToDecimal(balance), "", type, accountno, "", @"/PurchaseOrderImg/" + newFileName, "", "", "", 0, amount);
                                TempData["success"] = "Credit Pay Successfully";
                            }

                            if (Paymode == "Third Party Transfer" || Paymode.Contains("Online Transfer") || Paymode.Contains("Deposit"))
                            {
                                decimal disCharge = 0;
                                if (Paymode == "ATM Machine Deposit")
                                {
                                    var chargeEntry = db.PurchaseOrderCashDepositCharges.FirstOrDefault(a => a.PurchaseOrderType == "ATM Machine Deposit");
                                    disCharge = (amount * Convert.ToDecimal(chargeEntry.ChargePercant)) / 100;
                                    // alert(disCharge);
                                    if (disCharge <= Convert.ToDecimal(chargeEntry.MinCharge))
                                    {
                                        disCharge = Convert.ToDecimal(chargeEntry.MinCharge);
                                    }
                                }
                                else if (Paymode == "Branch Cash Deposit")
                                {
                                    var chargeEntry = db.PurchaseOrderCashDepositCharges.FirstOrDefault(a => a.PurchaseOrderType == "Branch Cash Deposit");
                                    disCharge = (amount * Convert.ToDecimal(chargeEntry.ChargePercant)) / 100;
                                    // alert(disCharge);
                                    if (disCharge <= Convert.ToDecimal(chargeEntry.MinCharge))
                                    {
                                        disCharge = Convert.ToDecimal(chargeEntry.MinCharge);
                                    }
                                }
                                db.insert_masterpurchageorder(userid, Paymode, utrno, "", "", Comment, Convert.ToDecimal(balance), "", type, accountno, "", @"/PurchaseOrderImg/" + newFileName, pancard, branch, AccHolderName, disCharge, amount - disCharge);
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


        #endregion Operator_INdex

        public ActionResult OperatorIndex()
        {
            var userid = User.Identity.GetUserId();
            var info = db.Master_allow_commission.Where(aa => aa.masterid == userid).SingleOrDefault();
            Operater_Commission sb = new Operater_Commission();
            sb.AmountRange = (from p in db.Recharge_Amount_range
                              select new Recharge_Amount_range_info
                              {
                                  idno = p.idno,
                                  maximum1 = p.maximum1,
                                  maximum2 = p.maximum2
                              }).ToList();

            sb.Prepaid = (from cust in _db.prepaid_master_comm
                          join ord in _db.Operator_Code
                          on cust.optcode equals ord.Operator_id.ToString()
                          where (cust.userid == userid)
                          select new Prepaid_Comm
                          {
                              OperatorCode = ord.new_opt_code,
                              Commission = cust.comm,
                              Commission1 = cust.comm1,
                              Commission2 = cust.comm2,
                              BlockTime = ord.blocktime,
                              Status = ord.status,
                              OperatorType = ord.Operator_type,
                              OperatorName = ord.operator_Name
                          }).OrderBy(aa => aa.OperatorName).ToList();

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
            sb.Money2 = (from cust in db.paytm_imps_master_comm
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

                             comm_21000 = cust.comm_21000,
                             comm_22000 = cust.comm_22000,
                             comm_23000 = cust.comm_23000,
                             comm_24000 = cust.comm_24000,
                             comm_25000 = cust.comm_25000,
                             comm_26000 = cust.comm_26000,
                             comm_27000 = cust.comm_27000,
                             comm_28000 = cust.comm_28000,
                             comm_29000 = cust.comm_29000,
                             comm_30000 = cust.comm_30000,

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
                             comm_50000 = cust.comm_50000,
                             per_26000 = cust.per_26000,
                             maxrs_26000 = cust.maxrs_26000,
                             Type_26000 = cust.Type_26000,
                             Type_1000 = cust.Type_1000,
                             Type_2000 = cust.Type_2000,
                             Type_3000 = cust.Type_3000,
                             Type_4000 = cust.Type_4000,
                             Type_5000 = cust.Type_5000,
                             Type_6000 = cust.Type_6000,
                             Type_7000 = cust.Type_7000,
                             Type_8000 = cust.Type_8000,
                             Type_9000 = cust.Type_9000,
                             Type_10000 = cust.Type_10000,

                             Type_11000 = cust.Type_11000,
                             Type_12000 = cust.Type_12000,
                             Type_13000 = cust.Type_13000,
                             Type_14000 = cust.Type_14000,
                             Type_15000 = cust.Type_15000,
                             Type_16000 = cust.Type_16000,
                             Type_17000 = cust.Type_17000,
                             Type_18000 = cust.Type_18000,
                             Type_19000 = cust.Type_19000,
                             Type_20000 = cust.Type_20000,

                             Type_21000 = cust.Type_21000,
                             Type_22000 = cust.Type_22000,
                             Type_23000 = cust.Type_23000,
                             Type_24000 = cust.Type_24000,
                             Type_25000 = cust.Type_25000

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
            sb.AEPS = (from cust in _db.Aeps_comm_userwise
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

            sb.MPOS = (from cust in _db.Mpos_comm_details
                       where cust.Userid == userid && cust.userRole == "Master"
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
                            where cust.UserId == userid && cust.Role == "Master"
                            select new INDONEPAL_Comm
                            {
                                min = cust.min,
                                max = cust.max,
                                charge = cust.charge,
                                margin = cust.margin

                            }).ToList();
            sb.FLIGHT = (from cust in _db.Slab_Flight
                         where cust.UserId == userid && cust.Role == "Master"
                         select new FLIGHT_Comm
                         {
                             IsDomestic = cust.IsDomestic,
                             marginPercentage = cust.marginPercentage,
                             RetailerMarkupCharge = cust.RetailerMarkupCharge,
                             gst = cust.gst,
                             tds = cust.tds
                         }).ToList();

            sb.HOTEL = (from cust in _db.Slab_Hotel
                        where cust.UserId == userid && cust.Role == "Master"
                        select new HOTEL_Comm
                        {
                            IsDomestic = cust.IsDomestic == true ? false : true,
                            marginPercentage = cust.marginPercentage,
                            RetailerMarkupCharge = cust.RetailerMarkupCharge,
                            gst = cust.gst,
                            tds = cust.tds
                        }).ToList();

            sb.BUS = (from cust in _db.Slab_Bus
                      where cust.UserId == userid && cust.Role == "Master"
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

            ViewBag.show = info.allowcommision;
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
            sb.AEPS = (from cust in _db.Aeps_comm_userwise
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

            sb.MPOS = (from cust in _db.Mpos_comm_details
                       where cust.Userid == userid && cust.userRole == "Master"
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
                            where cust.UserId == userid && cust.Role == "Master"
                            select new INDONEPAL_Comm
                            {
                                min = cust.min,
                                max = cust.max,
                                charge = cust.charge,
                                margin = cust.margin

                            }).ToList();
            sb.FLIGHT = (from cust in _db.Slab_Flight
                         where cust.UserId == userid && cust.Role == "Master"
                         select new FLIGHT_Comm
                         {
                             IsDomestic = cust.IsDomestic,
                             marginPercentage = cust.marginPercentage,
                             RetailerMarkupCharge = cust.RetailerMarkupCharge,
                             gst = cust.gst,
                             tds = cust.tds
                         }).ToList();

            sb.HOTEL = (from cust in _db.Slab_Hotel
                        where cust.UserId == userid && cust.Role == "Master"
                        select new HOTEL_Comm
                        {
                            IsDomestic = cust.IsDomestic == true ? false : true,
                            marginPercentage = cust.marginPercentage,
                            RetailerMarkupCharge = cust.RetailerMarkupCharge,
                            gst = cust.gst,
                            tds = cust.tds
                        }).ToList();

            sb.BUS = (from cust in _db.Slab_Bus
                      where cust.UserId == userid && cust.Role == "Master"
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

        #region Prepaid and DTH Slab Setting
        public ActionResult Prepaid_Slab_setting()
        {
            var userid = User.Identity.GetUserId();
            var info = db.Master_allow_commission.Where(aa => aa.masterid == userid).SingleOrDefault();
            if (info.allowcommision == true)
            {
                //var data = TempData.Peek("data");
                //if (data == "" || data == null)
                //{
                //    return RedirectToAction("tran_passslabsetting");
                //}
                //else
                //{
                PrepaidSlab model = new PrepaidSlab();
                model.AmountRange = (from p in db.Recharge_Amount_range
                                     select new Recharge_Amount_range_info
                                     {
                                         idno = p.idno,
                                         maximum1 = p.maximum1,
                                         maximum2 = p.maximum2
                                     }).ToList();
                model.common =
               (from p in db.prepaid_dealer_common_comm.Where(aa => aa.userid == userid)
                join opt in db.Operator_Code on p.optcode equals opt.Operator_id.ToString()

                select new PrepaidCommon
                {
                    idno = p.idno,
                    optcode = p.optcode,
                    OperatorName = opt.operator_Name,
                    dlmcomm = p.comm,
                    dlmcomm1 = p.comm1,
                    dlmcomm2 = p.comm2,
                }).OrderBy(p => p.OperatorName).ToList();
                return View(model);
            }
            else
            {
                return RedirectToAction("OperatorIndex", "Home");
            }

            //}
        }
        [HttpPost]
        public ActionResult _prepaid_comm(PrepaidSlab model)
        {
            var userid = User.Identity.GetUserId();
            foreach (var item in model.common)
            {
                var entry = db.prepaid_dealer_common_comm.Find(item.idno);
                var check = db.prepaid_master_comm.Where(aa => aa.userid == userid && aa.optcode == entry.optcode).SingleOrDefault();
                //if (check.comm >= item.dlmcomm)
                //{
                //    entry.comm = item.dlmcomm;
                //}
                //if (check.comm1 >= item.dlmcomm1)
                //{
                //    entry.comm1 = item.dlmcomm1;
                //}
                //if (check.comm2 >= item.dlmcomm2)
                //{
                //    entry.comm2 = item.dlmcomm2;
                //}
                entry.comm = item.dlmcomm;
                entry.comm1 = item.dlmcomm1;
                entry.comm2 = item.dlmcomm2;
                db.SaveChanges();
            }
            PrepaidSlab pmodel = new PrepaidSlab();
            pmodel.AmountRange = (from p in db.Recharge_Amount_range
                                  select new Recharge_Amount_range_info
                                  {
                                      idno = p.idno,
                                      maximum1 = p.maximum1,
                                      maximum2 = p.maximum2
                                  }).ToList();

            pmodel.common =
           (from p in db.prepaid_dealer_common_comm.Where(aa => aa.userid == userid)
            join opt in db.Operator_Code on p.optcode equals opt.Operator_id.ToString()
            select new PrepaidCommon
            {
                idno = p.idno,
                optcode = p.optcode,
                OperatorName = opt.operator_Name,
                dlmcomm = p.comm,
                dlmcomm1 = p.comm1,
                dlmcomm2 = p.comm2,
            }).OrderBy(p => p.OperatorName).ToList();
            return PartialView(pmodel);
            // return RedirectToAction("Prepaid_Slab_setting");
        }
        [HttpPost]
        public PartialViewResult _UserPrepaidSlab(string role, string userid, string ddlUserIdrem, string dlmid, PrepaidSlab model2, string ddlUserId, string Button)
        {
            var masterid = User.Identity.GetUserId();
            if (ddlUserId == null)
            {
                PrepaidSlab model1 = new PrepaidSlab();
                model1.AmountRange = (from p in db.Recharge_Amount_range
                                      select new Recharge_Amount_range_info
                                      {
                                          idno = p.idno,
                                          maximum1 = p.maximum1,
                                          maximum2 = p.maximum2
                                      }).ToList();
                //end user
                //Dealer User
                if (role == "Dealer")
                {
                    if (string.IsNullOrWhiteSpace(userid))
                    {
                        model1.Dealeruser =
                       (from m in db.prepaid_dealer_comm_by_master.Where(aa => aa.masterid == masterid)
                        join u in db.Users on m.userid equals u.UserId
                        join opt in db.Operator_Code on m.optcode equals opt.Operator_id.ToString()
                        where m.userid == userid
                        select new PrepaidDealer
                        {
                            idno = m.idno,
                            userid = m.userid,
                            optcode = m.optcode,
                            OperatorName = opt.operator_Name,
                            comm = m.comm,
                            comm1 = m.comm1,
                            comm2 = m.comm2,
                            Email = u.Email,
                            RolesName = "Dealer"
                        }).OrderBy(p => p.OperatorName).ToList();
                        List<SelectListItem> Provinces = new List<SelectListItem>();
                        var list = db.Dealer_Details.Where(aa => aa.SSId == masterid).Select(a => new SelectListItem { Text = a.FarmName, Value = a.DealerId, Selected = a.DealerId == userid ? true : false }).ToList();
                        model1.UserId = list;
                        model1.RetailerId = Provinces;
                        model1.Email = "";
                        model1.Name = "";
                        model1.Phone = "";


                    }
                    else
                    {
                        model1.Dealeruser =
                         (from m in db.prepaid_dealer_comm_by_master.Where(aa => aa.masterid == masterid)
                          join u in db.Users on m.userid equals u.UserId
                          join opt in db.Operator_Code on m.optcode equals opt.Operator_id.ToString()
                          where m.userid == userid
                          select new PrepaidDealer
                          {
                              idno = m.idno,
                              userid = m.userid,
                              optcode = m.optcode,
                              OperatorName = opt.operator_Name,
                              comm = m.comm,
                              comm1 = m.comm1,
                              comm2 = m.comm2,
                              Email = u.Email,
                              RolesName = "Dealer"
                          }).OrderBy(g => g.OperatorName).ToList();
                        var dlm = db.Dealer_Details.Where(aa => aa.SSId == masterid).ToList();
                        List<SelectListItem> Provinces = new List<SelectListItem>();
                        var list = dlm.Select(a => new SelectListItem { Text = a.FarmName, Value = a.DealerId, Selected = a.DealerId == userid ? true : false }).ToList();
                        model1.UserId = list;
                        model1.RetailerId = Provinces;
                        model1.Email = dlm.Where(aa => aa.DealerId == userid).SingleOrDefault().Email;
                        model1.Name = dlm.Where(aa => aa.DealerId == userid).SingleOrDefault().DealerName;
                        model1.Phone = dlm.Where(aa => aa.DealerId == userid).SingleOrDefault().Mobile;
                    }
                    model1.UpdateDealer =
                     (from com in db.prepaid_dealer_common_comm.Where(aa => aa.userid == masterid)
                      join opt in db.Operator_Code on com.optcode equals opt.Operator_id.ToString()
                      select new UpdatePrepaidDealer
                      {
                          idno = com.idno,
                          OperatorName = opt.operator_Name,
                          comm = com.comm,
                          comm1 = com.comm1,
                          comm2 = com.comm2,
                          optcode = com.optcode,


                      }).Distinct().OrderBy(a => a.OperatorName).ToList();

                }
                //End User
                return PartialView(model1);
            }
            else
            {

                if (role == "Dealer")
                {
                    if (ddlUserId != "")
                    {
                        try
                        {
                            db.Configuration.AutoDetectChangesEnabled = false;
                            foreach (var item in model2.Dealeruser)
                            {
                                var entry = db.prepaid_dealer_comm_by_master.Where(aa => aa.masterid == masterid).Single(a => a.userid == ddlUserId && a.optcode == item.optcode);
                                var check = db.prepaid_master_comm.Where(aa => aa.userid == masterid && aa.optcode == entry.optcode).SingleOrDefault();
                                if (check.comm >= item.comm)
                                {
                                    entry.comm = item.comm;
                                }
                                if (check.comm1 >= item.comm1)
                                {
                                    entry.comm1 = item.comm1;
                                }
                                if (check.comm2 >= item.comm2)
                                {
                                    entry.comm2 = item.comm2;
                                }
                                db.ChangeTracker.DetectChanges();
                                db.SaveChanges();
                            }
                        }
                        finally
                        {
                            db.Configuration.AutoDetectChangesEnabled = true;
                        }

                    }
                    else
                    {
                        try
                        {
                            db.Configuration.AutoDetectChangesEnabled = false;
                            var dlmlist = db.Dealer_Details.Where(aa => aa.SSId == masterid).Select(x => x.DealerId).ToList();
                            model2.UpdateDealer.ToList().ForEach(aa =>
                            {
                                db.prepaid_dealer_comm_by_master.Where(w => w.optcode == aa.optcode && dlmlist.Contains(w.userid) && w.masterid == masterid).ToList()
                                .ForEach(i =>
                                {
                                    var check = db.prepaid_master_comm.Where(aa1 => aa1.userid == masterid && aa1.optcode == aa.optcode).SingleOrDefault();
                                    if (check.comm >= aa.comm)
                                    {
                                        i.comm = aa.comm;
                                    }
                                    if (check.comm1 >= aa.comm1)
                                    {
                                        i.comm1 = aa.comm1;
                                    }
                                    if (check.comm2 >= aa.comm2)
                                    {
                                        i.comm2 = aa.comm2;
                                    }
                                });
                                if (Button == "Update Existing & New Users")
                                {
                                    db.prepaid_dealer_common_comm.Where(chn => chn.optcode == aa.optcode && chn.userid == masterid).ToList()
                                    .ForEach(i =>
                                    {
                                        var check = db.prepaid_master_comm.Where(aa1 => aa1.userid == masterid && aa1.optcode == aa.optcode).SingleOrDefault();
                                        if (check.comm >= aa.comm)
                                        {
                                            i.comm = aa.comm;
                                        }
                                        if (check.comm1 >= aa.comm1)
                                        {
                                            i.comm1 = aa.comm1;
                                        }
                                        if (check.comm2 >= aa.comm2)
                                        {
                                            i.comm2 = aa.comm2;
                                        }
                                    }
                                    );
                                }
                                db.ChangeTracker.DetectChanges();
                                int count = db.SaveChanges();
                            });
                        }
                        finally
                        {
                            db.Configuration.AutoDetectChangesEnabled = true;
                        }
                        //foreach (var item in model2.UpdateDealer)
                        //{
                        //    db.update_All_prepaid_comm("Dealer", item.optcode, item.comm, "");
                        //    if (Button == "Update Existing & New Users")
                        //    {
                        //        var entry = db.prepaid_common_comm.Single(a => a.optcode == item.optcode);
                        //        entry.dlmcomm = item.comm;
                        //        db.SaveChanges();
                        //    }
                        //}
                    }
                }
            }
            PrepaidSlab model = new PrepaidSlab();
            model.AmountRange = (from p in db.Recharge_Amount_range
                                 select new Recharge_Amount_range_info
                                 {
                                     idno = p.idno,
                                     maximum1 = p.maximum1,
                                     maximum2 = p.maximum2
                                 }).ToList();
            userid = ddlUserId;
            //end user
            //Dealer User
            if (role == "Dealer")
            {
                if (string.IsNullOrWhiteSpace(userid))
                {
                    model.Dealeruser =
                    (from m in db.prepaid_dealer_comm_by_master.Where(aa => aa.userid == masterid)
                     join u in db.Users on m.userid equals u.UserId
                     join opt in db.Operator_Code on m.optcode equals opt.Operator_id.ToString()
                     where m.userid == userid
                     select new PrepaidDealer
                     {
                         idno = m.idno,
                         userid = m.userid,
                         optcode = m.optcode,
                         OperatorName = opt.operator_Name,
                         comm = m.comm,
                         comm1 = m.comm1,
                         comm2 = m.comm2,
                         Email = u.Email,
                         RolesName = "Dealer"
                     }).OrderBy(p => p.OperatorName).ToList();
                    var list = db.Dealer_Details.Where(aa => aa.SSId == masterid).Select(a => new SelectListItem { Text = a.FarmName, Value = a.DealerId, Selected = a.DealerId == userid ? true : false }).ToList();
                    model.UserId = list;
                    List<SelectListItem> Provinces = new List<SelectListItem>();
                    model.RetailerId = Provinces;
                    model.Email = "";
                    model.Name = "";
                    model.Phone = "";
                }
                else
                {
                    model.Dealeruser =
                     (from m in db.prepaid_dealer_comm_by_master.Where(aa => aa.masterid == masterid)
                      join u in db.Users on m.userid equals u.UserId
                      join opt in db.Operator_Code on m.optcode equals opt.Operator_id.ToString()
                      where m.userid == userid
                      select new PrepaidDealer
                      {
                          idno = m.idno,
                          userid = m.userid,
                          optcode = m.optcode,
                          OperatorName = opt.operator_Name,
                          comm = m.comm,
                          comm1 = m.comm1,
                          comm2 = m.comm2,
                          Email = u.Email,
                          RolesName = "Dealer"
                      }).OrderBy(g => g.OperatorName).ToList();
                    var dlm = db.Dealer_Details.Where(aa => aa.SSId == masterid).ToList();
                    var list = dlm.Select(a => new SelectListItem { Text = a.FarmName, Value = a.DealerId, Selected = a.DealerId == userid ? true : false }).ToList();
                    model.UserId = list;
                    List<SelectListItem> Provinces = new List<SelectListItem>();
                    model.RetailerId = Provinces;
                    model.Email = dlm.Where(aa => aa.DealerId == userid).SingleOrDefault().Email;
                    model.Name = dlm.Where(aa => aa.DealerId == userid).SingleOrDefault().DealerName;
                    model.Phone = dlm.Where(aa => aa.DealerId == userid).SingleOrDefault().Mobile;
                }
                model.UpdateDealer =
                 (from com in db.prepaid_dealer_common_comm.Where(aa => aa.userid == masterid)
                  join opt in db.Operator_Code on com.optcode equals opt.Operator_id.ToString()
                  select new UpdatePrepaidDealer
                  {
                      idno = com.idno,
                      OperatorName = opt.operator_Name,
                      comm = com.comm,
                      comm1 = com.comm1,
                      comm2 = com.comm2,
                      optcode = com.optcode,


                  }).Distinct().OrderBy(a => a.OperatorName).ToList();

            }
            //End User
            //End User
            model.role = role;
            return PartialView(model);
        }

        #endregion

        #region Reports

        //END DisputeReport
        // RechargeReport 

        public JsonResult Getopt(string ID)
        {
            var Operator1 = db.Operator_Code.Distinct().Where(a => a.Operator_type == ID).ToList();
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
            ViewBag.alldealer = new SelectList(_db.select_Dealer_for_master_ddl(userid), "Dealerid", "DealerName", null).ToList();
            ViewBag.allretailer = new SelectList(_db.select_retailer_for_master_ddl(userid), "RetailerId", "RetailerName", null).ToList();

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


            ViewBag.alldealer = new SelectList(_db.select_Dealer_for_master_ddl(loginuserid), "Dealerid", "DealerName", null).ToList();
            ViewBag.allretailer = new SelectList(_db.select_retailer_for_master_ddl(loginuserid), "RetailerId", "RetailerName", null).ToList();

            var Operator1 = db.Operator_Code.Distinct().Where(a => a.Operator_type == Operator_Type).ToList();
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



            //var ch = db.proc_Recharge_operator_report_newPaging(1, 36, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), ddlusers, userid, ddl_status, optname, mobile, "ALL").ToList();

            var ch = db.proc_Recharge_operator_report_newPaging(1, 35, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), ddlusers, userid, ddl_status, optname, mobile, "ALL").ToList();

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

            int pagesize = 35;
            var tbrow = db.proc_Recharge_operator_report_newPaging(pageindex, 36, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), ddlusers, userid, ddl_status, optname, mobile, "ALL").ToList();

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



            var ch = db.proc_Recharge_operator_report_newPaging(1, 1000000, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), ddlusers, userid, ddl_status, optname, mobile, "ALL").ToList();

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



            var respo = db.proc_Recharge_operator_report_newPaging(1, 100000, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), ddlusers, userid, ddl_status, optname, mobile, "ALL").ToList();
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



            var respo = db.proc_Recharge_operator_report_newPaging(1, 100000, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), ddlusers, userid, ddl_status, optname, mobile, "ALL").ToList();

            return new ViewAsPdf(respo);
        }

        [HttpPost]
        public ActionResult Recharge_report_View(int Idno)
        {
            var details = db.Recharge_report_View_Details(Idno);
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
            var Liverechargecount = _db.Gift_card_details_report(Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), userid, "", "Master", "", "ALL", 50).ToList();
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
            if (ddlusers == "Master")
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
            var vv = _db.show_ecomm_report(userid, "Master", "ALL", "", 50, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();

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

            if (ddlusers == "Master")
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
            var ledger = _db.Retailer_Cr_Dr_Report("Master", userid, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
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
            var ledger = _db.Retailer_Cr_Dr_Report("Master", userid, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
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
            Money_transfer_report money = new Money_transfer_report();
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
                // ddlusers = "Admin";
                ddlusers = "Master";
                ddl_status = "ALL";
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

            if (ddlusers == "Master")
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


            var ch = db.money_transfer_report_paging(ddlusers, userid, ddl_status, "VASTWEB", Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), ddl_Type, 1, 20).ToList();


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
            ViewBag.apiid = new SelectList(db.select_apiusers_for_ddl("Admin"), "apiid", "username", null).ToList();
            string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy",
                            "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
            DateTime dt = DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            DateTime dt1 = DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            string frm_date = Convert.ToDateTime(dt).ToShortDateString();
            string to_date = Convert.ToDateTime(dt1).AddDays(1).ToString();


            if (ddlusers == "Master")
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
            var tbrow = db.money_transfer_report_paging(ddlusers, userid, ddl_status, "VASTWEB", Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), ddl_Type, pageindex, pagesize).ToList();

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

            var respo = db.money_transfer_report_paging(ddlusers, userid, ddl_status, "VASTWEB", Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), ddl_Type, 1, 10000).ToList();
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
                var sts = item.status;
                if (item.status.Contains("Pending"))
                {
                    double difftime = DateTime.Now.Subtract(item.trans_time.Value).TotalMinutes;
                    if (difftime < 2)
                    {
                        sts = "In Process";
                    }
                }
                dt2.Rows.Add(sts + " => " + item.frm_name, item.Trans_type, item.accountno, item.senderno, item.totalamount, debit, item.md_income, item.md_remain,
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

            if (ddlusers == "Master")
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

            var respo = db.money_transfer_report_paging(ddlusers, userid, ddl_status, "VASTWEB", Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), ddl_Type, 1, 100000).ToList();

            return new ViewAsPdf(respo);
        }
        public ActionResult Money_Tranfer_Details_View(int Idno)
        {
            var details = db.Money_Tranfer_Details_View(Idno);
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

            if (ddlusers == "Master")
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

            var ch = db.money_transfer_report_paging(ddlusers, userid, ddl_status, "VASTWEB", Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), ddl_Type, 1, 100000).ToList();
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
                ddlusers = "Master";

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
            if (ddlusers == "Master")
            {
                MasterId = loginuserid;
                retailerid = null;
                DealerId = null;

            }
            var proc_Response = db.proc_FlightReport(1, 20, ddl_status, retailerid, DealerId, MasterId, null, null, PNR, null, null, frm_date, to_date).ToList();

            return View(proc_Response);


        }
        public ActionResult InfiniteScroll_Ticket(int pageindex, string txt_frm_date, string txt_to_date, string ddl_status, string PNR, string ddlusers, string alldealer, string allretailer)
        {
            if ((txt_frm_date == null || txt_frm_date == "") && (txt_to_date == null || txt_to_date == ""))
            {
                txt_frm_date = DateTime.Now.ToString();
                txt_to_date = DateTime.Now.ToString();
                ddlusers = "Master";

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
            if (ddlusers == "Master")
            {
                MasterId = loginuserid;
                retailerid = null;
                DealerId = null;

            }
            int pagesize = 20;
            var tbrow = db.proc_FlightReport(pageindex, pagesize, ddl_status, retailerid, DealerId, MasterId, null, null, PNR, null, null, frm_date, to_date).ToList();
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
            if (ddlusers == "Master")
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

            var respo = db.proc_FlightReport(1, 100000, ddl_status, retailerid, DealerId, MasterId, null, null, PNR, null, null, frm_date, to_date).ToList();
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
            if (ddlusers == "Master")
            {
                MasterId = loginuserid;
                retailerid = null;
                DealerId = null;

            }
            var proc_Response = db.proc_FlightReport(1, 20, ddl_status, retailerid, DealerId, MasterId, null, null, PNR, null, null, frm_date, to_date).ToList();
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
            if (ddlusers == "Master")
            {
                MasterId = loginuserid;
                retailerid = null;
                DealerId = null;

            }
            var respo = db.proc_FlightReport(1, 20, ddl_status, retailerid, DealerId, MasterId, null, null, PNR, null, null, frm_date, to_date).ToList();
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
            if (ddlusers == "Master")
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
                ddlusers = "Master";

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
            if (ddlusers == "Master")
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
            var proc_Response = db.proc_BusReport(1, 20, ddl_status, retailerid, DealerId, userid, null, TicketNo, null, null, null, frm_date, to_date).ToList();
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
            if (ddlusers == "Master")
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
            var tbrow = db.proc_BusReport(pageindex, pagesize, ddl_status, retailerid, DealerId, userid, null, TicketNo, null, null, null, frm_date, to_date).ToList();
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
                ddlusers = "Master";

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
            if (ddlusers == "Master")
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
            if (ddlusers == "Master")
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

            var respo = db.proc_BusReport(1, 100000, ddl_status, retailerid, DealerId, userid, null, TicketNo, null, null, null, frm_date, to_date).ToList();
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
                ddlusers = "Master";

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
            if (ddlusers == "Master")
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
            var respo = db.proc_BusReport(1, 100000, ddl_status, retailerid, DealerId, userid, null, TicketNo, null, null, null, frm_date, to_date).ToList();
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
                ddlusers = "Master";

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
            if (ddlusers == "Master")
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
                ddlusers = "Master";

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
            if (ddlusers == "Master")
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
                ddlusers = "Master";

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
            if (ddlusers == "Master")
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

            var proc_Response = db.proc_HotelReport(1, 20, ddl_status, retailerid, DealerId, userid, null, null, TXNID, null, null, frm_date, to_date).ToList();
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
            if (ddlusers == "Master")
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

            var respo = db.proc_HotelReport(1, 100000, ddl_status, retailerid, DealerId, userid, null, null, TXNID, null, null, frm_date, to_date).ToList();
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
            if (ddlusers == "Master")
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
            if (ddlusers == "Master")
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
                ddlusers = "Master"; ddl_status = "ALL";


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

            if (ddlusers == "Master")
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
                ddlusers = "Master"; ddl_status = "ALL";


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

            if (ddlusers == "Master")
            {
                userid = loginid;
            }

            int pagesize = 20;
            var tbrow = db.Aeps_Report_New(ddlusers, userid, ddl_status, 0, "", "", "", "ALL", Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), pageindex, pagesize).ToList();


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
                ddlusers = "Master"; ddl_status = "ALL";


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

            if (ddlusers == "Master")
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
                ddlusers = "Master"; ddl_status = "ALL";


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

            if (ddlusers == "Master")
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
            dt2.Columns.Add("Master Pre", typeof(string));
            dt2.Columns.Add("Master Income", typeof(string));
            dt2.Columns.Add("Master Post", typeof(string));
            dt2.Columns.Add("Distributor Pre", typeof(string));
            dt2.Columns.Add("Distributor Income", typeof(string));
            dt2.Columns.Add("Distributor Post", typeof(string));
            dt2.Columns.Add("Remark", typeof(string));

            var respo = db.Aeps_Report_New(ddlusers, userid, ddl_status, 0, "", "", "", "ALL", Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), 1, 1000000).ToList();

            if (respo.Any())
            {
                foreach (var item in respo)
                {
                    decimal usercredit = 0;
                    var maskedaadhar = "";
                    if (!string.IsNullOrEmpty(item.AccountHolderAadhaar))
                    {
                        var aadhar = item.AccountHolderAadhaar;
                        var len = aadhar.Length;
                        var last = aadhar.Substring(len - 4, 4);
                        var first = new String('x', 8);
                        maskedaadhar = first + last;

                    }
                    if (item.Role == "Retailer")
                    {
                        decimal amount = 0;
                        decimal remincome = 0;
                        amount = Convert.ToDecimal(item.Amount);
                        remincome = Convert.ToDecimal(item.RemIncome);
                        usercredit = (amount + remincome);
                    }

                    dt2.Rows.Add(item.Status + "=" + item.Frm_Name, item.MerchantTxnId, maskedaadhar, item.BankName, item.Type, item.Amount, usercredit, item.MdPost,
                     item.BankRRN, item.Txn_Date, item.RemPre, item.RemIncome, item.RemPost, item.MdPre, item.MdIncome, item.MdPost, item.DlmPre
                    , item.DlmIncome, item.DlmPost, item.Remark);
                }

            }
            else
            {
                dt2.Rows.Add("", "", "", "", "", "", "", "", "", "");
            }
            var grid = new GridView();
            grid.DataSource = dt2;
            grid.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=Aeps_Report.xls");
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
                ddlusers = "Master"; ddl_status = "ALL";

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

            if (ddlusers == "Master")
            {
                userid = loginid;
            }

            var respo = db.Aeps_Report_New(ddlusers, userid, ddl_status, 0, "", "", "", "ALL", Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), 1, 100000).ToList();

            return new ViewAsPdf(respo);
        }

        [HttpPost]
        public ActionResult AepsReport_View(int Idno)
        {
            var detail = db.Aeps_Report_New_View(Idno);
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
                ddlusers = "Master";
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


            if (ddlusers == "Master")
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
            var ch = db.PAN_CARD_IPAY_Token_report_paging(1, pagesize, userid, ddlusers, ddl_status, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();

            return View(ch);
        }



        public ActionResult Pancard_report_new()
        {
            var loginid = User.Identity.GetUserId();
            //show dealer
            ViewBag.alldealer = new SelectList(_db.select_Dealer_for_master_ddl(loginid), "Dealerid", "DealerName", null).ToList();
            //show retailer
            ViewBag.allretailer = new SelectList(_db.select_retailer_for_master_ddl(loginid), "RetailerId", "RetailerName", null).ToList();

            var userid = User.Identity.GetUserId();
            string txt_frm_date = DateTime.Now.ToString();
            string txt_to_date = DateTime.Now.ToString();
            string frm_date = Convert.ToDateTime(txt_frm_date).Date.ToString("yyyy-MM-dd");
            string to_date = Convert.ToDateTime(txt_to_date).AddDays(1).ToString("yyyy-MM-dd");
            var frm_date1 = Convert.ToDateTime(frm_date);
            var ch = db.pancard_transation.Where(a => a.master_id == userid && a.request_time > frm_date1).OrderByDescending(s => s.idno).ToList();

            return View(ch);
        }
        [HttpPost]
        public ActionResult Pancard_report_new(string ddlusers, string ddl_status, string txt_frm_date, string txt_to_date, string allretailer, string alldealer)
        {
            var userid = "";
            var userid1 = "";
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
                    userid1 = "ALL";
                }
                else
                {
                    userid1 = allretailer;
                }
            }

            if (txt_frm_date == null && txt_to_date == null)
            {

                txt_frm_date = DateTime.Now.ToString();
                txt_to_date = DateTime.Now.ToString();

            }
            var loginid = User.Identity.GetUserId();
            //show all retailer 
            ViewBag.alldealer = new SelectList(_db.select_Dealer_for_master_ddl(loginid), "Dealerid", "DealerName", null).ToList();
            //show retailer
            ViewBag.allretailer = new SelectList(_db.select_retailer_for_master_ddl(loginid), "RetailerId", "RetailerName", null).ToList();




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
            var frm_date1 = Convert.ToDateTime(frm_date);
            var to_date1 = Convert.ToDateTime(to_date);

            var ch = db.pancard_transation.Where(s => s.master_id == loginid).OrderByDescending(s => s.idno).ToList();

            if(userid != "ALL")
            {
                if (ddl_status == "ALL")
                {
                    ch = db.pancard_transation.Where(s => s.master_id == loginid && s.request_time > frm_date1 && s.request_time < to_date1 && s.dealer_id == userid).OrderByDescending(s => s.idno).ToList();
                }
                else
                {
                    ch = db.pancard_transation.Where(s => s.master_id == loginid && s.request_time > frm_date1 && s.request_time < to_date1 && s.dealer_id == userid && ddl_status.ToUpper() == ddl_status.ToUpper()).OrderByDescending(s => s.idno).ToList();

                }
            }
            else if(userid1 != "ALL")
            {
                if (ddl_status == "ALL")
                {
                    ch = db.pancard_transation.Where(s => s.master_id == loginid && s.request_time > frm_date1 && s.request_time < to_date1 && s.Reailerid == userid1).OrderByDescending(s => s.idno).ToList();

                }
                else
                {
                    ch = db.pancard_transation.Where(s => s.master_id == loginid && s.request_time > frm_date1 && s.request_time < to_date1 && s.Reailerid == userid1 && ddl_status.ToUpper() == ddl_status.ToUpper()).OrderByDescending(s => s.idno).ToList();

                }
            }

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


            if (ddlusers == "Master")
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
            var tbrow = db.PAN_CARD_IPAY_Token_report_paging(pageindex, pagesize, userid, ddlusers, ddl_status, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();

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


            if (ddlusers == "Master")
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


            var ch = db.PAN_CARD_IPAY_Token_report_paging(1, 100000, userid, ddlusers, ddl_status, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();

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


            if (ddlusers == "Master")
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

            var respo = db.PAN_CARD_IPAY_Token_report_paging(1, 10000000, userid, ddlusers, ddl_status, frm_date, to_date).ToList();
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
        public virtual ActionResult ExcelRechargereportPan1(string ddlusers, string alldealer, string allretailer, string ddl_status, string txt_frm_date, string txt_to_date)
        {

            var userid = "";
            var userid1 = "";
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
                    userid1 = "ALL";
                }
                else
                {
                    userid1 = allretailer;
                }
            }

            if (txt_frm_date == null && txt_to_date == null)
            {

                txt_frm_date = DateTime.Now.ToString();
                txt_to_date = DateTime.Now.ToString();

            }
            var loginid = User.Identity.GetUserId();
            //show all retailer 
            ViewBag.alldealer = new SelectList(_db.select_Dealer_for_master_ddl(loginid), "Dealerid", "DealerName", null).ToList();
            //show retailer
            ViewBag.allretailer = new SelectList(_db.select_retailer_for_master_ddl(loginid), "RetailerId", "RetailerName", null).ToList();




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
            var frm_date1 = Convert.ToDateTime(frm_date);
            var to_date1 = Convert.ToDateTime(to_date);

            var proc_Response = db.pancard_transation.Where(s => s.master_id == loginid).OrderByDescending(s => s.idno).ToList();

            if (userid != "ALL")
            {
                if (ddl_status == "ALL")
                {
                    proc_Response = db.pancard_transation.Where(s => s.master_id == loginid && s.request_time > frm_date1 && s.request_time < to_date1 && s.dealer_id == userid).OrderByDescending(s => s.idno).ToList();
                }
                else
                {
                    proc_Response = db.pancard_transation.Where(s => s.master_id == loginid && s.request_time > frm_date1 && s.request_time < to_date1 && s.dealer_id == userid && s.status.ToUpper() == ddl_status.ToUpper()).OrderByDescending(s => s.idno).ToList();

                }
            }
            else if (userid1 != "ALL")
            {
                if (ddl_status == "ALL")
                {
                    proc_Response = db.pancard_transation.Where(s => s.master_id == loginid && s.request_time > frm_date1 && s.request_time < to_date1 && s.Reailerid == userid1).OrderByDescending(s => s.idno).ToList();
                }
                else
                {
                    proc_Response = db.pancard_transation.Where(s => s.master_id == loginid && s.request_time > frm_date1 && s.request_time < to_date1 && s.Reailerid == userid1 && s.status.ToUpper() == ddl_status.ToUpper()).OrderByDescending(s => s.idno).ToList();

                }
            }




            DataTable dataTbl = new DataTable();
            dataTbl.Columns.Add("Status", typeof(string));
            dataTbl.Columns.Add("User", typeof(string));
            dataTbl.Columns.Add("Amount", typeof(string));
            dataTbl.Columns.Add("User Pre", typeof(string));
            dataTbl.Columns.Add("TDS", typeof(string));
            dataTbl.Columns.Add("Comm", typeof(string));
            dataTbl.Columns.Add("User Post", typeof(string));
            dataTbl.Columns.Add("User Final", typeof(string));
            dataTbl.Columns.Add("MY Pre", typeof(string));
            dataTbl.Columns.Add("MY COMM", typeof(string));
            dataTbl.Columns.Add("MY TDS", typeof(string));
            dataTbl.Columns.Add("MY Post", typeof(string));
            dataTbl.Columns.Add("MY Final", typeof(string));
            dataTbl.Columns.Add("Time", typeof(string));


            if (proc_Response.Any())
            {
                foreach (var item in proc_Response)
                {
                    dataTbl.Rows.Add(item.status, item.Email + "=>" + item.name, item.Amount, item.Rem_pre, item.Rem_tds, item.Rem_comm, item.rem_post, item.Rem_final, item.master_pre, item.master_comm, item.master_tds, item.master_post, item.master_final, item.request_time);
                }
            }
            else
            {
                dataTbl.Rows.Add("", "", "", "", "", "", "", "", "", "", "", "", "", "");
            }
            var grid = new GridView();
            grid.DataSource = dataTbl;
            grid.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=Excel_TokenPurchase_Report.xls");
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


            if (ddlusers == "Master")
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
            var respo = db.PAN_CARD_IPAY_Token_report_paging(1, 10000000, userid, ddlusers, ddl_status, frm_date, to_date).ToList();
            return new ViewAsPdf(respo);
        }

        public ActionResult PDFreport_PAN1(string ddlusers, string alldealer, string allretailer, string ddl_status, string txt_frm_date, string txt_to_date)
        {
            var userid = "";
            var userid1 = "";
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
                    userid1 = "ALL";
                }
                else
                {
                    userid1 = allretailer;
                }
            }

            if (txt_frm_date == null && txt_to_date == null)
            {

                txt_frm_date = DateTime.Now.ToString();
                txt_to_date = DateTime.Now.ToString();

            }
            var loginid = User.Identity.GetUserId();
            //show all retailer 
            ViewBag.alldealer = new SelectList(_db.select_Dealer_for_master_ddl(loginid), "Dealerid", "DealerName", null).ToList();
            //show retailer
            ViewBag.allretailer = new SelectList(_db.select_retailer_for_master_ddl(loginid), "RetailerId", "RetailerName", null).ToList();




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
            var frm_date1 = Convert.ToDateTime(frm_date);
            var to_date1 = Convert.ToDateTime(to_date);

            var ch = db.pancard_transation.Where(s => s.master_id == loginid).OrderByDescending(s => s.idno).ToList();

            if (userid != "ALL")
            {
                if (ddl_status == "ALL")
                {
                    ch = db.pancard_transation.Where(s => s.master_id == loginid && s.request_time > frm_date1 && s.request_time < to_date1 && s.dealer_id == userid).OrderByDescending(s => s.idno).ToList();
                }
                else
                {
                    ch = db.pancard_transation.Where(s => s.master_id == loginid && s.request_time > frm_date1 && s.request_time < to_date1 && s.dealer_id == userid && ddl_status.ToUpper() == ddl_status.ToUpper()).OrderByDescending(s => s.idno).ToList();

                }
            }
            else if (userid1 != "ALL")
            {
                if (ddl_status == "ALL")
                {
                    ch = db.pancard_transation.Where(s => s.master_id == loginid && s.request_time > frm_date1 && s.request_time < to_date1 && s.Reailerid == userid1).OrderByDescending(s => s.idno).ToList();

                }
                else
                {
                    ch = db.pancard_transation.Where(s => s.master_id == loginid && s.request_time > frm_date1 && s.request_time < to_date1 && s.Reailerid == userid1 && ddl_status.ToUpper() == ddl_status.ToUpper()).OrderByDescending(s => s.idno).ToList();

                }
            }

            return new ViewAsPdf(ch);
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
            var ch = _db.money_transfer_report("Master", userid, Convert.ToInt32(ddlval), "ALL", "ALL", Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), "").ToList();
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
                ddlusers = "Master"; ddl_status = "ALL";

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

            if (ddlusers == "Master")
            {
                userid = loginid;
            }


            int pagesize = 20;

            var ch = db.Mpos_Report_New_paging(1, pagesize, ddlusers, userid, ddl_status, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
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
                ddlusers = "Master"; ddl_status = "ALL";

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

            if (ddlusers == "Master")
            {
                userid = loginid;
            }

            int pagesize = 20;

            var tbrow = db.Mpos_Report_New_paging(pageindex, pagesize, ddlusers, userid, ddl_status, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
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
            // show master id 


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

            if (ddlusers == "Master")
            {
                userid = loginid;
            }


            var ch = db.Mpos_Report_New_paging(1, 100000, ddlusers, userid, ddl_status, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
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

                ddlusers = "Master";

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

            if (ddlusers == "Master")
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



            var respo = db.Mpos_Report_New_paging(1, 100000, ddlusers, userid, ddl_status, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
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
            ViewBag.apiid = new SelectList(db.select_apiusers_for_ddl("Admin"), "apiid", "username", null).ToList();
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
                ddlusers = "Master";
            }

            if (ddl_status == "" || ddl_status == null)
            {
                ddl_status = "ALL";
            }
            if (ddlusers == "Master")
            {
                userid = loginid;
            }


            System.Threading.Thread.Sleep(1000);

            var respo = db.Mpos_Report_New_paging(1, 100000, ddlusers, userid, ddl_status, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
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
            string userid = User.Identity.GetUserId();
            var ch = db.bank_info.Where(x => x.userid == userid || x.userid == "Admin"); //(from jj in _db.bank_info select jj).ToList();
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
            if (role.Name.Contains("master"))
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
            //ViewBag.State = db.State_Desc.Where(y => y.State_id == AdminDetails.State).Single().State_name;
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
            // var gst = db.GST_Report_Retailer(fromdate, todate, userid, openingBal, ClosingBal, PurchaseAmt, IsIGST).ToList();

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
            var ch = _db.DTHConnection_Report_New("Master", userid, "ALL", 50, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
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
            var ch = _db.DTHConnection_Report_New("Master", userid, ddl_status, ddltop, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            ViewData["totals"] = ch.Where(s => s.Status == "Success").Sum(s => Convert.ToInt32(s.Amount));
            ViewData["Totalf"] = ch.Where(s => s.Status == "Failed").Sum(s => Convert.ToInt32(s.Amount));
            return View(ch);
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

        public ActionResult Master_Microatm_rental_report()
        {
            var userid = User.Identity.GetUserId();
            string txt_to_date1 = DateTime.Now.ToString();
            string frm_date = Convert.ToDateTime(txt_to_date1).AddDays(-1).ToShortDateString();
            string to_date = Convert.ToDateTime(txt_to_date1).ToShortDateString();
            var ch = db.microatm_rental_report(null, null, userid, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            return View(ch);
        }

        [HttpPost]
        public ActionResult Master_Microatm_rental_report(DateTime txt_frm_date, DateTime txt_to_date)
        {
            var userid = User.Identity.GetUserId();
            ViewBag.chk = "post";
            //txt_to_date = txt_to_date.AddDays(1);
            var ch = db.microatm_rental_report(null, null, userid, Convert.ToDateTime(txt_frm_date), Convert.ToDateTime(txt_to_date)).ToList();

            return View(ch);
        }
        public ActionResult MasterSellgst()
        {
            var userid = User.Identity.GetUserId();
            var today = DateTime.Today.AddMonths(-1);
            var list = new SelectList(new[]
            {
                new {ID = "January", Name = "January"},
                new {ID = "February", Name = "February"},
                new {ID = "March", Name = "March"},
                new {ID = "April", Name = "April"},
                new {ID = "May", Name = "May"},
                new {ID = "June", Name = "June"},
                new {ID = "July", Name = "July"},
                new {ID = "August", Name = "August"},
                new {ID = "September", Name = "September"},
                new {ID = "October", Name = "October"},
                new {ID = "November", Name = "November"},
                new {ID = "December", Name = "December"},
            },
            "ID", "Name");

            var currentyear = today.Year.ToString();
            var lastyear = today.AddYears(-1).Year.ToString();
            var month = new DateTime(today.Year, today.Month, 1);

            ViewBag.month = new SelectList(list, "Value", "Text");
            var yesrlist = new SelectList(new[]
            {
                new { ID = currentyear, Name = currentyear },
                new { ID = lastyear, Name = lastyear},
            },
            "ID", "Name");
            ViewBag.year = new SelectList(yesrlist, "Value", "Text");
            ViewBag.chkmonth = today.ToString("MMMM");

            var fromchk = today.ToString("MMMM").Substring(0, 3);
            var fromchkdate = currentyear;
            var mn = fromchk + "" + fromchkdate;
            var monthname = today.ToString("MMMM");
            var respp = db.masterinfo_purchase_info(userid, monthname, currentyear).ToList();
            return View(respp);
        }
        [HttpPost]
        public ActionResult MasterSellgst(string Month, string Year)
        {
            var masterid = User.Identity.GetUserId();
            var today = DateTime.Today.AddMonths(-1);
            var list = new SelectList(new[]
            {
                new {ID = "January", Name = "January"},
                new {ID = "February", Name = "February"},
                new {ID = "March", Name = "March"},
                new {ID = "April", Name = "April"},
                new {ID = "May", Name = "May"},
                new {ID = "June", Name = "June"},
                new {ID = "July", Name = "July"},
                new {ID = "August", Name = "August"},
                new {ID = "September", Name = "September"},
                new {ID = "October", Name = "October"},
                new {ID = "November", Name = "November"},
                new {ID = "December", Name = "December"},
            },
            "ID", "Name");

            var currentyear = today.Year.ToString();
            var lastyear = today.AddYears(-1).Year.ToString();
            var month = new DateTime(today.Year, today.Month, 1);

            ViewBag.month = new SelectList(list, "Value", "Text");
            var yesrlist = new SelectList(new[]
            {
                new { ID = currentyear, Name = currentyear },
                new { ID = lastyear, Name = lastyear},
            },
            "ID", "Name");
            ViewBag.year = new SelectList(yesrlist, "Value", "Text");
            ViewBag.chkmonth = today.ToString("MMMM");

            var fromchk = today.ToString("MMMM").Substring(0, 3);
            var fromchkdate = currentyear;
            var mn = fromchk + "" + fromchkdate;
            var monthname = today.ToString("MMMM");
            var respp = db.masterinfo_purchase_info(masterid, Month, Year).ToList();
            IEnumerable<SelectListItem> selectList = from s in respp
                                                     select new SelectListItem
                                                     {
                                                         Value = s.SSId,
                                                         Text = s.FarmName.ToString()
                                                     };

            ViewBag.apiid = new SelectList(selectList, "Value", "Text");
            return View(respp);
        }
        public ActionResult Print_Sell_Gst_Master(string Month, string Year)
        {
            string masterid = User.Identity.GetUserId();
            var userid = masterid;
            var today = DateTime.Today.AddMonths(-1);
            var admininfo = db.Admin_details.SingleOrDefault();
            ViewBag.adminfirm = admininfo.Companyname;
            ViewBag.adminaddress = admininfo.Address;
            ViewBag.adminpancard = admininfo.pencardno;
            if (string.IsNullOrEmpty(admininfo.Gstno))
            {
                ViewBag.admingst = "UNREGISTERED";
            }
            else
            {
                ViewBag.admingst = admininfo.Gstno;
            }

            var userinfo = db.Superstokist_details.Where(aa => aa.SSId == masterid).SingleOrDefault();
            var sateinfo = db.State_Desc.Where(aa => aa.State_id == userinfo.State).SingleOrDefault();
            ViewBag.firmname = userinfo.FarmName;
            ViewBag.address = userinfo.Address;
            ViewBag.pancard = userinfo.pancard;
            int monthcc1 = DateTimeFormatInfo.CurrentInfo.MonthNames.ToList().IndexOf(Month) + 1;
            DateTime lastDate1 = new DateTime(Convert.ToInt32(Year), monthcc1, 1).AddMonths(1);

            if (string.IsNullOrEmpty(userinfo.gst) == true || userinfo.gststatus == "N")
            {
                ViewBag.customergst = "UNREGISTERED";
            }
            else
            {
                ViewBag.customergst = userinfo.gst;
            }
            if (string.IsNullOrEmpty(admininfo.Gstno))
            {
                ViewBag.customergst = "UNREGISTERED";
            }
            ViewBag.statename = sateinfo.State_name;

            var month = new DateTime(today.Year, today.Month, 1);

            ViewBag.month = Month + " - " + Year;
            int monthcc = DateTimeFormatInfo.CurrentInfo.MonthNames.ToList().IndexOf(Month) + 1;
            DateTime lastDate = new DateTime(Convert.ToInt32(Year), monthcc, 1).AddMonths(1).AddDays(-1);
            var lst = lastDate.ToString("dd-MM-yyyy");
            ViewBag.ldate = lst;
            var ltoday = DateTime.Today;
            var cuumonth = ltoday.ToString("MMMM");
            var cuuyear = ltoday.Year.ToString();

            if (cuumonth == Month && cuuyear == Year)
            {
                userid = "";
            }
            var entries = db.master_purchase_gst(userid, Month, Year).ToList();
            var chkk = db.Admin_details.SingleOrDefault().State;
            var statename = db.State_Desc.Where(aa => aa.State_id == chkk).SingleOrDefault().State_name;
            ViewBag.stname = statename;
            if (userinfo.State == chkk)
            {
                ViewBag.localgst = "N";
            }
            else
            {
                ViewBag.localgst = "Y";
            }
            double total = Convert.ToDouble(entries.Where(aa => aa.hsn != "0000").Sum(aa => aa.amount) + (entries.Where(aa => aa.hsn != "0000").Sum(aa => aa.gst)));
            string s = words(total, true).Substring(0, 3);
            if (s == "and")
            {
                s = words(total, true).Remove(0, 4);
            }
            else
            {
                s = words(total, true);
            }
            ViewBag.finalword = s;
            var fromchk = Month.Substring(0, 3);
            var fromchkdate = Year;
            var mn = fromchk + "" + fromchkdate;
            if (userinfo.RolesType == "FieldSalesOfficer")
            {
                ViewBag.invoice = "Salary";
            }
            else
            {
                ViewBag.invoice = "";
            }
            //    var invoice = db.Retailer_invoice_Numbers.Where(aa => aa.retailerid == userid && aa.month == Month && aa.year == Year).SingleOrDefault();
            //   DB.Repository<InvoiceNumber>().GetAll(aa => aa.UserID == userid && aa.type == "VERIFY" && aa.month == mn).SingleOrDefault();

            //  var newinvoice = invoice.invoiceno;

            return new ViewAsPdf("Print_Sell_Gst_Master", entries);
        }
        public ActionResult uploadgst()
        {
            var userid = User.Identity.GetUserId();
            var today = DateTime.Now.AddMonths(-1);
            ViewBag.chkmonth = today.ToString("MMMM");
            var currentyear = today.Year.ToString();
            var lastyear = today.AddYears(-1).Year.ToString();
            var list = new SelectList(new[]
              {
                new {ID = "January", Name = "January"},
                new {ID = "February", Name = "February"},
                new {ID = "March", Name = "March"},
                new {ID = "April", Name = "April"},
                new {ID = "May", Name = "May"},
                new {ID = "June", Name = "June"},
                new {ID = "July", Name = "July"},
                new {ID = "August", Name = "August"},
                new {ID = "September", Name = "September"},
                new {ID = "October", Name = "October"},
                new {ID = "November", Name = "November"},
                new {ID = "December", Name = "December"},
              }, "ID", "Name");
            ViewBag.month = new SelectList(list, "Value", "Text");
            var yesrlist = new SelectList(new[]
            {
                new { ID = currentyear, Name = currentyear },
                new { ID = lastyear, Name = lastyear},
            }, "ID", "Name");
            ViewBag.year = new SelectList(yesrlist, "Value", "Text");
            var month = today.ToString("MMMM");
            var year = today.Year.ToString();
            var chk = db.Master_upload_gst.Where(aa => aa.masterid == userid && aa.monthchk == month && aa.yearchk == year).ToList();
            return View(chk);
        }
        [HttpPost]
        public ActionResult uploadgst(string Month, string Year)
        {
            var userid = User.Identity.GetUserId();
            var today = DateTime.Now.AddMonths(-1);
            ViewBag.chkmonth = today.ToString("MMMM");
            var currentyear = today.Year.ToString();
            var lastyear = today.AddYears(-1).Year.ToString();
            var list = new SelectList(new[]
              {
                new {ID = "January", Name = "January"},
                new {ID = "February", Name = "February"},
                new {ID = "March", Name = "March"},
                new {ID = "April", Name = "April"},
                new {ID = "May", Name = "May"},
                new {ID = "June", Name = "June"},
                new {ID = "July", Name = "July"},
                new {ID = "August", Name = "August"},
                new {ID = "September", Name = "September"},
                new {ID = "October", Name = "October"},
                new {ID = "November", Name = "November"},
                new {ID = "December", Name = "December"},
              }, "ID", "Name");
            ViewBag.month = new SelectList(list, "Value", "Text");
            var yesrlist = new SelectList(new[]
            {
                new { ID = currentyear, Name = currentyear },
                new { ID = lastyear, Name = lastyear},
            }, "ID", "Name");
            ViewBag.year = new SelectList(yesrlist, "Value", "Text");
            var month = today.ToString("MMMM");
            var year = today.Year.ToString();
            var chk = db.Master_upload_gst.Where(aa => aa.masterid == userid && aa.monthchk == Month && aa.yearchk == Year).ToList();
            return View(chk);
        }
        public ActionResult uploadgstfile()
        {
            var userid = User.Identity.GetUserId();
            if (Request.Files.Count > 0)
            {
                try
                {
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        string Month = Request.Form["month"];
                        string Year = Request.Form["year"];
                        HttpPostedFileBase file = files[i];
                        string extension = Path.GetExtension(file.FileName);
                        if (extension.ToUpper() == ".PDF")
                        {
                            string fname = file.FileName;
                            fname = Month + Year + userid + fname;
                            string filenm = fname;
                            fname = Path.Combine(Server.MapPath("~/UploadGst/Master"), fname);
                            file.SaveAs(fname);
                            var chk = db.Master_upload_gst.Where(aa => aa.masterid == userid && aa.monthchk == Month && aa.yearchk == Year && (aa.status == "Approved" || aa.status == "Pending")).SingleOrDefault();
                            if (chk == null)
                            {
                                Master_upload_gst up = new Master_upload_gst();
                                up.monthchk = Month;
                                up.masterid = userid;
                                up.status = "Pending";
                                up.uploadfile = "UploadGst/Master/" + filenm;
                                up.yearchk = Year;
                                db.Master_upload_gst.Add(up);
                                db.SaveChanges();
                            }
                        }
                    }
                    return Json("File Uploaded Successfully!");
                }
                catch (Exception ex)
                {
                    return Json("Error occurred. Error details: " + ex.Message);
                }
            }
            else
            {
                return Json("No files selected.");
            }
        }


        public ActionResult GatewayTRANSFER()
        {
            ViewBag.msg = TempData["msg"];
            TempData.Remove("msg");
            var userid = User.Identity.GetUserId();
            DateTime from = DateTime.Now.Date;
            DateTime to = DateTime.Now.AddDays(1).Date;

            var chk = db.gateway_report(userid, "Master", from, to, "");

            var getwaytotals = db.Payment_Gateway_Txn_history.Where(x => x.userid == userid && x.roles == "Master" && x.f_date >= from && x.f_date <= to);
            var totalsuccess = getwaytotals.Where(x => x.status.ToUpper().Contains("SUCCESS")).Sum(x => x.amount);
            var totalFailed = getwaytotals.Where(x => x.status.ToUpper().Contains("FAILED")).Sum(x => x.amount);
            var totalpending = getwaytotals.Where(x => x.status.ToUpper().Contains("PENDING")).Sum(x => x.amount);
            var totalcharges = getwaytotals.Sum(x => x.charge);

            ViewBag.totalsuccess = totalsuccess;
            ViewBag.totalfailedamount = totalFailed;
            ViewBag.totalpendingamount = totalpending;
            ViewBag.totalchargesamount = totalcharges;

            return View(chk);
        }

        [HttpPost]
        public ActionResult GatewayTRANSFER(DateTime txt_frm_date, DateTime txt_to_date)
        {

            ViewBag.chk = "post";
            var userid = User.Identity.GetUserId();
            DateTime to = txt_to_date.AddDays(1).Date;

            var chk = db.gateway_report(userid, "Master", txt_frm_date, to, "");

            var getwaytotals = db.Payment_Gateway_Txn_history.Where(x => x.userid == userid && x.roles == "Master" && x.f_date >= txt_frm_date && x.f_date <= to);
            var totalsuccess = getwaytotals.Where(x => x.status.ToUpper().Contains("SUCCESS")).Sum(x => x.amount);
            var totalFailed = getwaytotals.Where(x => x.status.ToUpper().Contains("FAILED")).Sum(x => x.amount);
            var totalpending = getwaytotals.Where(x => x.status.ToUpper().Contains("PENDING")).Sum(x => x.amount);
            var totalcharges = getwaytotals.Sum(x => x.charge);

            ViewBag.totalsuccess = totalsuccess;
            ViewBag.totalfailedamount = totalFailed;
            ViewBag.totalpendingamount = totalpending;
            ViewBag.totalchargesamount = totalcharges;


            return View(chk);
        }

        public ActionResult PDFGatewayTRANSFER(DateTime txt_frm_date, DateTime txt_to_date)
        {
            ViewBag.chk = "post";
            var userid = User.Identity.GetUserId();
            DateTime to = txt_to_date.AddDays(1).Date;

            var chk = db.gateway_report(userid, "Master", txt_frm_date, to, "");

            var getwaytotals = db.Payment_Gateway_Txn_history.Where(x => x.userid == userid && x.roles == "Master" && x.f_date >= txt_frm_date && x.f_date <= to);
            var totalsuccess = getwaytotals.Where(x => x.status.ToUpper().Contains("SUCCESS")).Sum(x => x.amount);
            var totalFailed = getwaytotals.Where(x => x.status.ToUpper().Contains("FAILED")).Sum(x => x.amount);
            var totalpending = getwaytotals.Where(x => x.status.ToUpper().Contains("PENDING")).Sum(x => x.amount);
            var totalcharges = getwaytotals.Sum(x => x.charge);

            ViewBag.totalsuccess = totalsuccess;
            ViewBag.totalfailedamount = totalFailed;
            ViewBag.totalpendingamount = totalpending;
            ViewBag.totalchargesamount = totalcharges;


            return new ViewAsPdf(chk);
        }

        public ActionResult Execel_Gateway_TRANSFER(DateTime txt_frm_date, DateTime txt_to_date)
        {

            var userid = User.Identity.GetUserId();
            DateTime to = txt_to_date.AddDays(1).Date;

            var chk = db.gateway_report(userid, "Master", txt_frm_date, to, "").ToList();
            DataTable dataTbl = new DataTable();
            dataTbl.Columns.Add("Mode Mode", typeof(string));
            dataTbl.Columns.Add("Amount", typeof(string));
            dataTbl.Columns.Add("Charges", typeof(string));
            dataTbl.Columns.Add("Net Received", typeof(string));
            dataTbl.Columns.Add("Bank RRN ", typeof(string));
            dataTbl.Columns.Add("Transaction Time ", typeof(string));
            dataTbl.Columns.Add("Response Time", typeof(string));

            if (chk.Any())
            {

                foreach (var item in chk)
                {
                    var sts = item.status;
                    var bankrrn = item.errormsg;
                    if (sts == "Success")
                    {
                        bankrrn = item.bankrrnno;
                    }
                    dataTbl.Rows.Add(item.PG_TYPE, item.amount, item.charge, item.totalpay, bankrrn, item.f_date, item.resptime);
                }
            }
            else
            {
                dataTbl.Rows.Add("", "", "", "", "", "", "");
            }
            var grid = new GridView();
            grid.DataSource = dataTbl;
            grid.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=Execel_Gateway_TRANSFER.xls");
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


        [HttpPost]
        public ActionResult GatewayTransfer_new(decimal txtamt, string ddl_type, string latss, string longloc)
        {
            var userid = User.Identity.GetUserId();

            string city = null;
            string address = null;
            //string remlocationstatus = null;
            //RetailerRemloclogic.checklocationbyRem(userid, "CHECK", out remlocationstatus, latss, longloc, ref city, ref address);
            //if (remlocationstatus == "NOTALLOW")
            //{
            //    TempData["msg"] = "Getway Transfer not allowed at this location";
            //    return RedirectToAction("GatewayTRANSFER");
            //}
            if (ddl_type == "")
            {
                TempData["msg"] = "Select Any Type";
                return RedirectToAction("GatewayTRANSFER");
            }

            string txnid = Generatetxnid();
            var checKYC = db.Superstokist_details.Where(x => x.SSId == userid).SingleOrDefault();
            System.Data.Entity.Core.Objects.ObjectParameter output = new System.Data.Entity.Core.Objects.ObjectParameter("Output", typeof(string));
            var auth = db.Gateway_Auth.SingleOrDefault();
            var PaymentGatewaykycsts = db.paymentgateway_kyc_status.SingleOrDefault().isekyc;

            if (auth != null)
            {
                string kycstatus = "FULLKYC";
                if (PaymentGatewaykycsts == true)
                {
                    if (!string.IsNullOrEmpty(checKYC.pancardPath) && !string.IsNullOrEmpty(checKYC.aadharcardPath) && !string.IsNullOrEmpty(checKYC.BackSideAadharcardphoto) && checKYC.PSAStatus == "Y" && checKYC.AadhaarStatus == "Y")
                    {
                        kycstatus = "FULLKYC";
                    }
                    else
                    {
                        kycstatus = "NOTKYC";
                    }
                }
                if (kycstatus == "FULLKYC")
                {
                    var msg = db.PaymentGateway_Fund_insert("Master", userid, txtamt, txnid, ddl_type,"", output).SingleOrDefault().msg;
                    if (msg == "OK")
                    {
                        var dropcategory = "DC,CC,NB,UPI,CASH";
                        if (ddl_type == "NB")
                        {
                            dropcategory = "DC,CC,UPI,CASH";
                        }
                        else if (ddl_type == "WA")
                        {
                            dropcategory = "DC,CC,NB,UPI";
                        }
                        else if (ddl_type == "UP")
                        {
                            dropcategory = "DC,CC,NB,CASH";
                        }
                        else if (ddl_type == "DC")
                        {
                            dropcategory = "CC,NB,UPI,CASH";
                        }
                        else if (ddl_type == "CC")
                        {
                            dropcategory = "DC,NB,UPI,CASH";
                        }

                        String strPathAndQuery = System.Web.HttpContext.Current.Request.Url.PathAndQuery;
                        String strUrl = System.Web.HttpContext.Current.Request.Url.AbsoluteUri.Replace(strPathAndQuery, "/");
                        var url = strUrl + "Response/GatewayResponse";
                        // var key = "CbR19P";
                        var key = auth.merchantkey;
                        var DealerName = checKYC.SuperstokistName;
                        var Email = checKYC.Email;
                        //  var salt = "i0dSyipJyJAzKovgBSPhWfUQAsj1GGIZ";
                        var salt = auth.merchantsalt;
                        RemotePost myremotepost = new RemotePost();
                        //  myremotepost.Url = "https://test.payu.in/_payment";
                        myremotepost.Url = "https://secure.payu.in/_payment";
                        myremotepost.Add("key", key);

                        myremotepost.Add("txnid", txnid);
                        myremotepost.Add("amount", txtamt.ToString());
                        myremotepost.Add("productinfo", "Fund Transfer");
                        myremotepost.Add("firstname", DealerName);
                        myremotepost.Add("phone", checKYC.Mobile);
                        myremotepost.Add("email", checKYC.Email);
                        myremotepost.Add("surl", url);//Change the success url here depending upon the port number of your local system.
                        myremotepost.Add("furl", url);//Change the failure url here depending upon the port number of your local system.
                        myremotepost.Add("service_provider", "payu_paisa");
                        myremotepost.Add("drop_category", dropcategory);
                        string hashString = key + "|" + txnid + "|" + txtamt + "|" + "Fund Transfer" + "|" + DealerName + "|" + Email + "|||||||||||" + salt;
                        //string hashString = "3Q5c3q|2590640|3053.00|OnlineBooking|vimallad|ladvimal@gmail.com|||||||||||mE2RxRwx";
                        string hash = Generatehash512(hashString);
                        myremotepost.Add("hash", hash);
                        myremotepost.Post();
                    }
                    else
                    {
                        TempData["msg"] = msg;
                        return RedirectToAction("GatewayTRANSFER");
                    }
                }
                else
                {
                    TempData["msg"] = "Due To KYC You are Not Elligable To Transfer";
                    return RedirectToAction("GatewayTRANSFER");
                }
            }
            else
            {
                TempData["msg"] = "Auth Key not Set";

            }
            return RedirectToAction("GatewayTRANSFER");
        }

        public string calculatecharge_gateway(decimal amount, string type)
        {
            var userid = User.Identity.GetUserId();
            var chk = db.PaymentGatewaycharge_new.Where(s => s.userid == userid).SingleOrDefault();
            var obj = new JObject();

            if (chk == null)
            {
                obj.Add("status", false);
                return JsonConvert.SerializeObject(obj);
            }
            else
            {
                obj.Add("status", true);

                if (type == "NB")
                {
                    var cols = new JArray();

                    var status = new JObject();
                    var hdfc_icici = new JObject();
                    var axis = new JObject();
                    var other = new JObject();

                    var sub_cols = new JArray();
                    sub_cols.Add("charge");
                    sub_cols.Add("total");
                    sub_cols.Add("net");

                    status.Add("text", "Status");

                    hdfc_icici.Add("text", "HDFC/ICICI");
                    hdfc_icici.Add("cols", sub_cols);

                    axis.Add("text", "AXIS/SBI/KOTAK");
                    axis.Add("cols", sub_cols);

                    other.Add("text", "OTHERS");
                    other.Add("cols", sub_cols);

                    cols.Add(status);
                    cols.Add(hdfc_icici);
                    cols.Add(axis);
                    cols.Add(other);

                    var values = new JArray();

                    values.Add(chk.netbankingsts);

                    var hdfc_val = new JArray();
                    hdfc_val.Add(chk.netbanking);
                    var charge_amt = (chk.netbanking * amount) / 100;
                    var gst = (charge_amt * 18) / 100;
                    var total_charge = charge_amt + gst;
                    hdfc_val.Add(total_charge);
                    hdfc_val.Add(amount - total_charge);

                    var axis_val = new JArray();
                    axis_val.Add(chk.axis);

                    var chargeaxis_amt = (chk.axis * amount) / 100;
                    var gstaxis = (chargeaxis_amt * 18) / 100;
                    var totalaxis_charge = chargeaxis_amt + gstaxis;
                    axis_val.Add(totalaxis_charge);
                    axis_val.Add(amount - totalaxis_charge);

                    var other_val = new JArray();
                    other_val.Add(chk.others);

                    var chargeother_amt = chk.others;
                    var gstother = (chargeother_amt * 18) / 100;
                    var totalother_charge = chargeother_amt + gstother;
                    other_val.Add(totalother_charge);
                    other_val.Add(amount - totalother_charge);

                    values.Add(hdfc_val);
                    values.Add(axis_val);
                    values.Add(other_val);

                    obj.Add("cols", cols);
                    obj.Add("values", values);

                }
                else if (type == "WA")
                {
                    var cols = new JArray();
                    cols.Add("Status");
                    cols.Add("Charge");
                    cols.Add("Total");
                    cols.Add("Net Received");

                    var values = new JArray();
                    values.Add(chk.walletsts);
                    values.Add(chk.wallet);
                    var charge_amt = (chk.wallet * amount) / 100;
                    var gst = (charge_amt * 18) / 100;
                    var total_charge = charge_amt + gst;
                    values.Add(total_charge);
                    values.Add(amount - total_charge);

                    obj.Add("cols", cols);
                    obj.Add("values", values);

                }
                else if (type == "UP")
                {
                    var cols = new JArray();
                    cols.Add("Status");
                    cols.Add("Charge");
                    cols.Add("Total");
                    cols.Add("Net Received");

                    var values = new JArray();
                    values.Add(chk.upists);
                    values.Add(chk.UPI);
                    var charge_amt = (chk.UPI * amount) / 100;
                    var gst = (charge_amt * 18) / 100;
                    var total_charge = charge_amt + gst;
                    values.Add(total_charge);
                    values.Add(amount - total_charge);

                    obj.Add("cols", cols);
                    obj.Add("values", values);

                }
                else if (type == "DC")
                {
                    var cols = new JArray();
                    cols.Add("Status");

                    var sub_cols = new JArray();
                    sub_cols.Add("charge");
                    sub_cols.Add("total");
                    sub_cols.Add("net");

                    var rupay = new JObject();
                    rupay.Add("text", "RUPAY");
                    rupay.Add("cols", sub_cols);

                    var other = new JObject();
                    other.Add("text", "OTHERS");

                    var other_cols = new JArray();

                    var upto2k = new JObject();
                    upto2k.Add("text", "UpTo 2K");
                    upto2k.Add("cols", sub_cols);

                    var above2k = new JObject();
                    above2k.Add("text", "Above 2K");
                    above2k.Add("cols", sub_cols);

                    other_cols.Add(upto2k);
                    other_cols.Add(above2k);

                    other.Add("cols", other_cols);

                    cols.Add(rupay);
                    cols.Add(other);


                    var values = new JArray();
                    values.Add(chk.DebitCardsts);

                    var rupay_array = new JArray();
                    rupay_array.Add(chk.rupaydebit);
                    rupay_array.Add((chk.rupaydebit * amount) / 100);
                    rupay_array.Add(amount - (chk.rupaydebit * amount) / 100);

                    var upto2k_array = new JArray();
                    upto2k_array.Add(chk.debitupto2000);

                    var charge2000_amt = (chk.debitupto2000 * amount) / 100;
                    var gst2000 = (charge2000_amt * 18) / 100;
                    var total2000_charge = charge2000_amt + gst2000;

                    upto2k_array.Add(total2000_charge);
                    upto2k_array.Add(amount - total2000_charge);

                    var above2k_array = new JArray();
                    above2k_array.Add(chk.debitabove2000);

                    var charge2001_amt = (chk.debitabove2000 * amount) / 100;
                    var gst2001 = (charge2001_amt * 18) / 100;
                    var total2001_charge = charge2001_amt + gst2001;

                    above2k_array.Add(total2001_charge);
                    above2k_array.Add(amount - total2001_charge);

                    var other_array = new JArray();
                    other_array.Add(upto2k_array);
                    other_array.Add(above2k_array);

                    values.Add(rupay_array);
                    values.Add(other_array);

                    obj.Add("cols", cols);
                    obj.Add("values", values);

                }
                else if (type == "CC")
                {
                    var cols = new JArray();
                    cols.Add("Status");
                    cols.Add("Charge");
                    cols.Add("Total");
                    cols.Add("Net Received");

                    var values = new JArray();
                    values.Add(chk.CreditCardsts);
                    values.Add(chk.creditcard);

                    var charge_amt = (chk.creditcard * amount) / 100;
                    var gst = (charge_amt * 18) / 100;
                    var total_charge = charge_amt + gst;

                    values.Add(total_charge);
                    values.Add(amount - total_charge);

                    obj.Add("cols", cols);
                    obj.Add("values", values);

                }

                return JsonConvert.SerializeObject(obj);

            }


        }


        public string Generatetxnid()
        {

            Random rnd = new Random();
            string strHash = Generatehash512(rnd.ToString() + DateTime.Now);
            string txnid1 = strHash.ToString().Substring(0, 20);

            return txnid1;
        }

        public class RemotePost
        {
            private System.Collections.Specialized.NameValueCollection Inputs = new System.Collections.Specialized.NameValueCollection();
            public string Url = "";
            public string Method = "post";
            public string FormName = "form1";

            public void Add(string name, string value)
            {
                Inputs.Add(name, value);
            }

            public void Post()
            {
                System.Web.HttpContext.Current.Response.Clear();

                System.Web.HttpContext.Current.Response.Write("<html><head>");

                System.Web.HttpContext.Current.Response.Write(string.Format("</head><body onload=\"document.{0}.submit()\">", FormName));
                System.Web.HttpContext.Current.Response.Write(string.Format("<form name=\"{0}\" method=\"{1}\" action=\"{2}\" >", FormName, Method, Url));
                for (int i = 0; i < Inputs.Keys.Count; i++)
                {
                    System.Web.HttpContext.Current.Response.Write(string.Format("<input name=\"{0}\" type=\"hidden\" value=\"{1}\">", Inputs.Keys[i], Inputs[Inputs.Keys[i]]));
                }
                System.Web.HttpContext.Current.Response.Write("</form>");
                System.Web.HttpContext.Current.Response.Write("</body></html>");

                System.Web.HttpContext.Current.Response.End();
            }
        }


        public string Generatehash512(string text)
        {

            byte[] message = Encoding.UTF8.GetBytes(text);

            UnicodeEncoding UE = new UnicodeEncoding();
            byte[] hashValue;
            SHA512Managed hashString = new SHA512Managed();
            string hex = "";
            hashValue = hashString.ComputeHash(message);
            foreach (byte x in hashValue)
            {
                hex += String.Format("{0:x2}", x);
            }
            return hex;

        }
        public ActionResult Extracomm_Report()
        {
            var userid = User.Identity.GetUserId();

            var chk = db.daywisecommsetforusers.Where(s => s.userid == userid && s.role == "Master").ToList();
            if (chk.Count == 0)
            {
                daywisecommsetforuser d1 = new daywisecommsetforuser();
                d1.role = "Master";
                d1.userid = userid;
                d1.Comm_2000_5000 = 0;
                d1.Comm_5001_10000 = 0;
                d1.Comm_10001_max = 0;
                db.daywisecommsetforusers.Add(d1);
                db.SaveChanges();
            }
            var dfg = db.daywisecomms.Where(s => s.mdid == userid).OrderByDescending(s => s.date).ToList();
            return View(dfg);

        }
        [HttpPost]
        public ActionResult Extracomm_Report(string txt_frm_date, string txt_to_date)
        {
            var userid = User.Identity.GetUserId();


            if (txt_frm_date == null && txt_to_date == null)
            {
                txt_frm_date = DateTime.Now.ToString();
                txt_to_date = DateTime.Now.ToString();

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

            var dfg = db.daywisecomms.Where(s => s.mdid == userid && s.date > frm_date && s.date < to_date).OrderByDescending(s => s.date).ToList();
            return View(dfg);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}