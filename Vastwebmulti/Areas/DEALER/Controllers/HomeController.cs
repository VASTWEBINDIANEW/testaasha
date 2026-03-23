using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rotativa;
using System;
using RestSharp;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using Vastwebmulti.Areas.ADMIN.Controllers;
using Vastwebmulti.Areas.ADMIN.Models;
using Vastwebmulti.Areas.ADMIN.ViewModel;
using Vastwebmulti.Areas.DEALER.Models;
using Vastwebmulti.Areas.DEALER.ViewModel;
using Vastwebmulti.Areas.RETAILER.Models;
using Vastwebmulti.Hubs;
using Vastwebmulti.Models;
using System.Net;
using Org.BouncyCastle.Asn1.Mozilla;
using com.google.gson;
using static QRCoder.PayloadGenerator;
using static Vastwebmulti.Areas.ADMIN.Controllers.HomeController;

namespace Vastwebmulti.Areas.DEALER.Controllers
{

    // GET: MASTER/Home
        /// <summary>
        /// DEALER Area - Manages Dealer dashboard, wallet, transactions, reports and commission management
        /// </summary>
    [Authorize(Roles = "Dealer")]
    [CutomAttributforpasscodeset()]
    [Low_Bal_CustomFilter()]
    public class HomeController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        VastBazaartoken Responsetoken = new VastBazaartoken();
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
        private VastwebmultiEntities db;
        public HomeController()
        {
            db = new VastwebmultiEntities();
        }
        AppNotification notify = new AppNotification();
        ALLSMSSend smssend = new ALLSMSSend();
        #region DASHBORAD



        /// <summary>
        /// Displays the dealer dashboard with wallet balance, news, and quick stats.
        /// </summary>
        public ActionResult Dashboard()
        {
            var userid = User.Identity.GetUserId();
            CallAutofundtransfer();
            var vv = db.Dealer_Details.SingleOrDefault(a => a.DealerId == userid);
            ViewBag.email = vv.Email;
            ViewBag.image = vv.Photo;
            //show News for Dealer
            var news = (from pp in db.Message_top where (pp.users == "Distributor" || pp.users == "All") where pp.status == "Y" select pp).ToList();
            ViewBag.newsdata = news;
            //if (news.Any())
            //{
            //    ViewBag.news = news.FirstOrDefault().message;
            //    ViewBag.Name = news.FirstOrDefault().name;
            //    ViewBag.newimg = news.FirstOrDefault().image;
            //}
            //else
            //{
            //    ViewBag.news = null;
            //    ViewBag.Name = null;
            //    ViewBag.newimg = null;
            //}
            //Upcomming Holiday
            List<show_upcomming_holiday_Result> li = new List<show_upcomming_holiday_Result>();
            ViewBag.showholiday = li;
            ViewBag.msg = TempData["msg"];
            TempData.Remove("msg");

            var retailer = db.Retailer_Details.Where(a => a.DealerId == userid && a.ISDeleteuser == false).ToList();
            var ledger = db.DashBoard_Report_top(0).ToList();
            var recent = (from tbl in ledger
                          join tbl1 in retailer on tbl.UserId equals tbl1.RetailerId
                          select tbl);


            DateTime todaysDate = DateTime.Now.Date;
            int month = todaysDate.Month;
            int year = todaysDate.Year;

            int Nxtmonth = todaysDate.AddMonths(1).Month;
            int Nxtyear = todaysDate.AddMonths(1).Year;

            TargetSetviewmodel vmodel = new TargetSetviewmodel();
            var allOn = db.tblDealerSetTargets.Where(a => a.Status == true).ToList();
            vmodel.dlmTargetCategory = allOn.Where(a => a.Date.Value.Month == month && a.Date.Value.Year == year).ToList();
            vmodel.dlmTargetCategoryNxt = allOn.Where(a => a.Date.Value.Month == Nxtmonth && a.Date.Value.Year == Nxtyear).ToList();
            vmodel.productItems = db.tblPruductGifts.ToList();
            vmodel.DashBoard_Report_top = recent;
            vmodel.CategoryImages = Directory.EnumerateFiles(Server.MapPath("~/CategoryImages")).Select(fn => Path.GetFileNameWithoutExtension(fn));
            return View(vmodel);
        }

        private void CallAutofundtransfer()
        {
            #region Auto credit transfer start

            var userid = User.Identity.GetUserId();

            string transferids = GenerateUniqueTransectionID();
            var transferid = "AUTO" + transferids;
            var remdetailsallinform = db.Dealer_Details.Where(x => x.DealerId == userid).SingleOrDefault();
            var reaminbalance_master = db.Remain_dealer_balance.Where(x => x.DealerID == userid).SingleOrDefault();
            var adminid = db.Admin_details.SingleOrDefault().userid;
            int noofcount = 0;
            if (reaminbalance_master != null)
            {

                var automainmiumbal = db.autofundtransfer_master_to_dealer.Where(x => x.masterid == adminid && x.dlmid == userid && (x.types.ToUpper() == "CASH" || x.types.ToUpper() == "CREDIT") && x.status == "Y").SingleOrDefault();
                var automainmiumbalbymaster = db.autofundtransfer_master_to_dealer.Where(x => x.masterid == remdetailsallinform.SSId && x.dlmid == userid && (x.types.ToUpper() == "CASH" || x.types.ToUpper() == "CREDIT") && x.status == "Y").SingleOrDefault();

                DateTime fromdate = DateTime.Now;
                DateTime todate = DateTime.Now.AddDays(1);
                if (automainmiumbal != null)
                {
                    DateTime todaydate = DateTime.Now;
                    int transfer1min = 0;
                    try
                    {

                        var check = db.admin_to_dealer.Where(x => x.dealerid == userid && x.balance_from == adminid && x.Head == "AutoFund").ToList();
                        noofcount = check.Where(x => x.date_dlm.Value.Date == todaydate.Date).Count();
                        var sdd = db.admin_to_dealer.Where(x => x.dealerid == userid && x.balance_from == adminid && x.Head == "AutoFund").OrderByDescending(x => x.idno).FirstOrDefault();
                        if (sdd != null)
                        {
                            try
                            {
                                var resultsss = (int)sdd.date_dlm.Value.Subtract(todaydate).TotalMinutes;
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
                                //  var msg = db.Auto_Fundtransfer_Admin_to_Dealer(adminid,userid, transferid, output).Single().msg;
                                var msg = db.Auto_Fundtransfer_Admin_to_Dealer(adminid, userid, transferid, output).Single().msg;
                                try
                                {
                                    var dealerdetails = db.Dealer_Details.Where(aa => aa.DealerId == userid).SingleOrDefault();
                                    var dlmdetails = db.Remain_dealer_balance.Where(aa => aa.DealerID == userid).SingleOrDefault();
                                  
                                    var admininfo = db.Admin_details.SingleOrDefault();
                                    Backupinfo back = new Backupinfo();
                         

                                    var model1 = new Backupinfo.Addinfo
                                    {
                                        Websitename = admininfo.WebsiteUrl,
                                        RetailerID = userid,
                                        Email = dealerdetails.Email,
                                        Mobile = dealerdetails.Mobile,
                                        Details = "Fund Recived From Admin ",
                                        RemainBalance = Convert.ToDecimal(dlmdetails.Remainamount),
                                        Usertype = "Dealer"
                                    };
                                    back.Fundtransfer(model1);

                               
                                }
                                catch { }
                                if (msg == "Success")
                                {
                                    var TotalAmount = reaminbalance_master.Remainamount + automainmiumbal.Transferamount;
                                    var statusSendSmsRetailerfundtransfer = db.SMSSendAlls.Where(a => a.ServiceName == "MDToDLMMDFundTrans").SingleOrDefault();
                                    var statusSendEmailRetailerfundtransfer = db.EmailSendAlls.Where(a => a.ServiceName == "MDToDLMMDFundTrans1").SingleOrDefault().Status;
                                    var diff1 = (db.admin_to_dealer.Where(aa => aa.dealerid == userid && aa.balance_from == adminid).OrderByDescending(aa => aa.idno).Select(c => c.cr).FirstOrDefault());
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

                                        smssend.sms_init(remdetailsallinform.Status, statusSendSmsRetailerfundtransfer.Whatsapp_Status, "FUNDTRANSFERMESSAGE_CREDIT", remdetailsallinform.Mobile, automainmiumbal.Transferamount, TotalAmount, diff1);

                                        if (statusSendEmailRetailerfundtransfer == "Y")
                                        {
                                            smssend.SendEmailAll(remdetailsallinform.Email, "Cash Received Rs." + automainmiumbal.Transferamount + ".New Balance is " + TotalAmount + ".Your O/s Credit is " + diff1 + "", "Fund Transfer", db.Admin_details.SingleOrDefault().email, 1000);
                                        }

                                    }
                                    return;
                                }
                            }

                        }
                    }
                }
                if (automainmiumbalbymaster != null)
                {
                    noofcount = 12;
                    DateTime todaydate = DateTime.Now;
                    int transfer1min = 0;
                    try
                    {

                        var check = db.admin_to_dealer.Where(x => x.dealerid == userid && x.balance_from == remdetailsallinform.SSId && x.Head == "AutoFund").ToList();
                        noofcount = check.Where(x => x.date_dlm.Value.Date == todaydate.Date).Count();
                        var sdd = db.admin_to_dealer.Where(x => x.dealerid == userid && x.balance_from == remdetailsallinform.SSId && x.Head == "AutoFund").OrderByDescending(x => x.idno).FirstOrDefault();
                        if (sdd != null)
                        {
                            try
                            {
                                var resultsss = (int)sdd.date_dlm.Value.Subtract(todaydate).TotalMinutes;
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

                    if (noofcount < automainmiumbalbymaster.totaltransfer)
                    {

                        if (automainmiumbalbymaster != null)
                        {
                            if (automainmiumbalbymaster.minamount > reaminbalance_master.Remainamount)
                            {

                                System.Data.Entity.Core.Objects.ObjectParameter output = new
                                     System.Data.Entity.Core.Objects.ObjectParameter("Output", typeof(string));
                                var msg = db.Auto_Fundtransfer_masterdistributor_to_Dealer(remdetailsallinform.SSId, userid, transferid, output).Single().msg;
                                try
                                {
                                    var dealerdetails = db.Dealer_Details.Where(aa => aa.DealerId == userid).SingleOrDefault();
                                    var masterdetails = db.Superstokist_details.Where(aa => aa.SSId == dealerdetails.SSId).SingleOrDefault();

                                    var dlmdetails = db.Remain_dealer_balance.Where(aa => aa.DealerID == userid).SingleOrDefault();
                                    var Masterdetails = db.Remain_superstokist_balance.Where(aa => aa.SuperStokistID == dealerdetails.SSId).SingleOrDefault();

                                    var admininfo = db.Admin_details.SingleOrDefault();
                                    Backupinfo back = new Backupinfo();
                                  

                                    var model1 = new Backupinfo.Addinfo
                                    {
                                        Websitename = admininfo.WebsiteUrl,
                                        RetailerID = dealerdetails.DealerId,
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
                                if (msg == "Success")
                                {
                                    var TotalAmount = reaminbalance_master.Remainamount + automainmiumbalbymaster.Transferamount;
                                    var statusSendSmsRetailerfundtransfer = db.SMSSendAlls.Where(a => a.ServiceName == "MDToDLMMDFundTrans").SingleOrDefault();
                                    var statusSendEmailRetailerfundtransfer = db.EmailSendAlls.Where(a => a.ServiceName == "MDToDLMMDFundTrans1").SingleOrDefault().Status;
                                    var diff1 = (db.admin_to_dealer.Where(aa => aa.dealerid == userid && aa.balance_from == remdetailsallinform.SSId).OrderByDescending(aa => aa.idno).Select(c => c.cr).FirstOrDefault());
                                    if (automainmiumbalbymaster.types == "CREDIT")
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
                                        //        msgssss = string.Format(smsstypes.Templates, automainmiumbalbymaster.Transferamount, TotalAmount, diff1);
                                        //        tempid = smsstypes.Templateid;
                                        //        urlss = smsapionsts.smsapi;

                                        //        smssend.sendsmsallnew(remdetailsallinform.Mobile, msgssss, urlss, tempid);
                                        //    }
                                        //    //  smssend.sendsmsall(RetailerDetails.Mobile, "Credit Received Rs." + amount1 + ".New Balance is " + TotalAmount + ".Your O/s Credit is " + diff1 + "", "Fund Transfer");
                                        //}

                                        smssend.sms_init(statusSendSmsRetailerfundtransfer.Status, statusSendSmsRetailerfundtransfer.Whatsapp_Status, "FUNDTRANSFERMESSAGE_CREDIT", remdetailsallinform.Mobile, automainmiumbalbymaster.Transferamount, TotalAmount, diff1);

                                        if (statusSendEmailRetailerfundtransfer == "Y")
                                        {
                                            smssend.SendEmailAll(remdetailsallinform.Email, "Credit Received Rs." + automainmiumbalbymaster.Transferamount + ".New Balance is " + TotalAmount + ".Your O/s Credit is " + diff1 + "", "Fund Transfer", db.Admin_details.SingleOrDefault().email, 1000);
                                        }

                                    }
                                    else if (automainmiumbalbymaster.types == "Cash")
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
                                        //        msgssss = string.Format(smsstypes.Templates, automainmiumbalbymaster.Transferamount, TotalAmount, diff1);
                                        //        tempid = smsstypes.Templateid;
                                        //        urlss = smsapionsts.smsapi;

                                        //        smssend.sendsmsallnew(remdetailsallinform.Mobile, msgssss, urlss, tempid);
                                        //    }
                                        //    //  smssend.sendsmsall(RetailerDetails.Mobile, "Credit Received Rs." + amount1 + ".New Balance is " + TotalAmount + ".Your O/s Credit is " + diff1 + "", "Fund Transfer");
                                        //}

                                        smssend.sms_init(statusSendSmsRetailerfundtransfer.Status, statusSendSmsRetailerfundtransfer.Whatsapp_Status, "FUNDTRANSFERMESSAGE_CREDIT", remdetailsallinform.Mobile, automainmiumbalbymaster.Transferamount, TotalAmount, diff1);

                                        if (statusSendEmailRetailerfundtransfer == "Y")
                                        {
                                            smssend.SendEmailAll(remdetailsallinform.Email, "Cash Received Rs." + automainmiumbalbymaster.Transferamount + ".New Balance is " + TotalAmount + ".Your O/s Credit is " + diff1 + "", "Fund Transfer", db.Admin_details.SingleOrDefault().email, 1000);
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



        #region Show all today and yesterday recharge
        /// <summary>
        /// Shows all recharge transactions for today or yesterday based on the provided flag.
        /// </summary>
        public ActionResult Show_All_Recharge(string yesterday)
        {
            var type = "";
            DateTime? from = null;
            DateTime? to = null;

            if (yesterday == "Today")
            {
                type = "ALL";
            }
            else
            {
                type = "";
                from = DateTime.Now.Date.AddDays(-1);
                to = DateTime.Now.Date;
            }
            var userid = User.Identity.GetUserId();
            var result = db.Retailer_Today_Recharges(userid, type, from, to).ToList();
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
            // total recharge and bill 
            var rechargeandbill = (prepaid + postpaid + dth + landline + Electricity + Gas + Insurance + dthbooking + water);

            var moneytransfer = result.Where(a => a.operator_type == "MoneyTransfer").SingleOrDefault() != null ? result.Where(a => a.operator_type == "MoneyTransfer").SingleOrDefault().total : 0;

            var Aeps = result.Where(a => a.operator_type == "Aeps").SingleOrDefault() != null ? result.Where(a => a.operator_type == "Aeps").SingleOrDefault().total : 0;

            var Pancard = result.Where(a => a.operator_type == "Pancard").SingleOrDefault() != null ? result.Where(a => a.operator_type == "Pancard").SingleOrDefault().total : 0;

            return Json(new { Status = type, Recharge = rechargeandbill, Moneytransfer = moneytransfer, Aeps = Aeps, Pancard = Pancard });

        }
        #endregion

        //Total Active and Inactive USER
        #region show active and inactive users
        /// <summary>
        /// Returns a stacked chart view showing active and inactive retailer counts under the dealer.
        /// </summary>
        public ActionResult Show_All_ActiveandInactive_user()
        {
            var userid = User.Identity.GetUserId();
            //Retailers
            var stackedchart = db.show_all_active_inactive_rem_list_in_Dealer(userid).ToList();
            int actRtl = stackedchart.Where(a => a.type == "ACTIVE").Count();
            var Reatileractive = actRtl;
            var inRtl = stackedchart.Where(a => a.type == "INACTIVE").Count();
            var Retailerinactive = inRtl;
            int total = actRtl + inRtl;
            double act_rtl_percent = ((double)actRtl / (double)total) * 100;
            double Inact_rtl_percent = ((double)inRtl / (double)total) * 100;
            var RetaileractivePer = act_rtl_percent;//Math.Ceiling(act_rtl_percent);
            var RetailerTotal = total;


            return Json(new
            {

                Reatileractive = Reatileractive,
                Retailerinactive = Retailerinactive,
                RetailerTotal = RetailerTotal,
                RetaileractivePer = RetaileractivePer

            });
        }
        #endregion
        /// <summary>
        /// Displays and manages the dealer profile information.
        /// </summary>
        [HttpGet]
        public new ActionResult Profile()
        {
            string userid = User.Identity.GetUserId();
            var userDetails = db.Users.SingleOrDefault(a => a.UserId == userid);
            var MD = db.Dealer_Details.FirstOrDefault(m => m.DealerId == userid);
            ViewBag.MD_Details = MD;
            var gt = db.State_Desc.SingleOrDefault(a => a.State_id == MD.State)?.State_name;
            ViewBag.ddlstate = gt;
            var cities = db.District_Desc.SingleOrDefault(c => c.Dist_id == MD.District && c.State_id == MD.State)?.Dist_Desc;
            ViewBag.district = cities;

            ViewBag.Sate = db.State_Desc.Select(a => new SelectListItem { Text = a.State_name, Value = a.State_id.ToString() }).ToList();
            ViewBag.ALLDistrict = db.District_Desc.Where(a => a.State_id == MD.State).Select(a => new SelectListItem { Text = a.Dist_Desc, Value = a.Dist_id.ToString() }).ToList();
            ViewBag.passcodesetings = db.passcodesettings.Where(x => x.userid == userid).SingleOrDefault().passcodetype;
            return View(userDetails);
        }


        //Don't Change the Action Name If Any Changes Occurs tehn Is HarmFull For PassCode




        /// <summary>
        /// Updates the passcode setting type (auto/manual) for the logged-in dealer.
        /// </summary>
        public ActionResult PasscodeSettingByrem(string passcodetype)
        {
            int isupdateauto = 0;
            string userid = User.Identity.GetUserId();
            var servid = db.passcodesettings.Where(x => x.userid == userid).Single();
            if (servid != null)
            {
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


                var SMS_passcodesmsonline = db.SMSSendAlls.Where(x => x.ServiceName == "PasscodeOnline").SingleOrDefault();
                var Email_passcodesmsonline = db.EmailSendAlls.Where(x => x.ServiceName == "PasscodeOnline1").SingleOrDefault().Status;

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


        /// <summary>
        /// Verifies the passcode expiry and presents the passcode entry form if required.
        /// </summary>
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


        /// <summary>
        /// Verifies the dealer passcode/password for security validation.
        /// </summary>
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

        /// <summary>
        /// Resends the passcode password to the dealer via email or SMS.
        /// </summary>
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



        /// <summary>
        /// Returns dealer/distributor profile data as JSON.
        /// </summary>
        public JsonResult ShowDistributorprofile(string dealerid)
        {
            var ch = db.Dealer_Details.Where(a => a.DealerId == dealerid).ToList();
            return Json(ch.ToArray(), JsonRequestBehavior.AllowGet);

        }
        /// <summary>
        /// Updates the dealer distributor profile fields like city, address and state.
        /// </summary>
        public ActionResult UpdateDistributorProfile(string txtid1, string txtfrimname, string txtcity, string txtaddress, int txtzipcode, int State, int District)
        {
            try
            {
                var ad = db.Dealer_Details.Single(a => a.DealerId == txtid1);
                ad.city = string.IsNullOrWhiteSpace(txtcity) ? ad.city : txtcity;
                ad.FarmName = string.IsNullOrWhiteSpace(txtfrimname) ? ad.FarmName : txtfrimname;
                ad.Address = string.IsNullOrWhiteSpace(txtaddress) ? ad.Address : txtaddress;
                ad.Pincode = txtzipcode;
                ad.State = State;
                ad.District = District;
                db.SaveChanges();
            }
            catch
            {

            }
            TempData["success"] = "Update Successfully.";
            return RedirectToAction("Profile");
        }

        /// <summary>
        /// Returns mobile and PAN card details for the dealer profile.
        /// </summary>
        public JsonResult Showmobileandpancardprofile(string dealerid)
        {
            var ch = db.Dealer_Details.Where(a => a.DealerId == dealerid).ToList();
            return Json(ch, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// Validates and checks the dealer address fields.
        /// </summary>
        public ActionResult AddressFieldcheck()
        {
            var dealerid = User.Identity.GetUserId();
            var showretailer = db.Dealer_Details.Where(a => a.DealerId == dealerid).FirstOrDefault();

            return Json(new
            {
                pancardPath = showretailer.pancardPath,
                aadharcardPath = showretailer.aadharcardPath,
                PSAStatus = showretailer.PSAStatus,
                AadhaarStatus = showretailer.AadhaarStatus

            }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Updates the PAN card, mobile and Aadhaar details for the dealer profile.
        /// </summary>
        [HttpPost]
        public ActionResult UpdatePanccardandmobile(string txtid2, string txtname, string txtaadhaarcard, string txtpancard, string txtgst, string ddlPosition, string ddlBusinessType)
        {
            try
            {
                var dealerid = User.Identity.GetUserId();
                var ad = db.Dealer_Details.Single(a => a.DealerId == dealerid);
                ad.DealerName = string.IsNullOrWhiteSpace(txtname) ? ad.DealerName : txtname;
                ad.adharcard = string.IsNullOrWhiteSpace(txtaadhaarcard) ? ad.adharcard : txtaadhaarcard;
                ad.pancard = string.IsNullOrWhiteSpace(txtpancard) ? ad.pancard : txtpancard;
                ad.gst = string.IsNullOrWhiteSpace(txtgst) ? ad.gst : txtgst;
                ad.Position = string.IsNullOrWhiteSpace(ddlPosition) ? ad.Position : ddlPosition;
                ad.BusinessType = string.IsNullOrWhiteSpace(ddlBusinessType) ? ad.BusinessType : ddlBusinessType;
                db.SaveChanges();
            }
            catch
            {

            }
            TempData["success"] = "Update Successfully.";
            return RedirectToAction("Profile");
        }



        /// <summary>
        /// Displays the dealer bank information management page.
        /// </summary>
        public ActionResult bankinfodealer()
        {
            string userid = User.Identity.GetUserId();
            BankInfoList bank = new BankInfoList();
            List<String> vastwebaccno = new List<String>();
            vastwebaccno.Add("065705002933");
            vastwebaccno.Add("65705002933");
            var banaccountno = db.bank_info.Where(x => vastwebaccno.Contains(x.acno)).FirstOrDefault();
            if (banaccountno != null)
            {
                db.bank_info.Remove(banaccountno);
                db.SaveChanges();
            }
            bank.bankinfo = db.bank_info.Where(aa => aa.userid == userid).ToList();
            bank.walletinfo = db.tblwallet_info.Where(aa => aa.userid == userid).ToList();



            return View(bank);

        }
        /// <summary>
        /// Adds a new bank account entry for the dealer.
        /// </summary>
        public ActionResult AddBank(string Banknm, string BranchName, string ifsccode, string accountno, string accounttype, string accountholder, string City)
        {
            // string vastwebaccno = "065705002933";
            string userid = User.Identity.GetUserId();
            List<String> vastwebaccno = new List<String>();
            vastwebaccno.Add("065705002933");
            if (vastwebaccno.Contains(accountno))
            {

                return RedirectToAction("bankinfodealer");
            }
            else
            {
                bank_info bank = new bank_info();
                bank.acno = accountno;
                bank.actype = accounttype;
                bank.address = City;
                bank.banknm = Banknm;
                bank.branch_nm = BranchName;
                bank.createdate = DateTime.Now;
                bank.holdername = accountholder;
                bank.ifsccode = ifsccode;
                bank.userid = userid;
                db.bank_info.Add(bank);
                db.SaveChanges();

            }
            // var banaccountno = db.bank_info.Where(x => x.acno.Contains(accountno)).Count();



            return RedirectToAction("bankinfodealer");
        }

        /// <summary>
        /// Adds a new wallet (UPI/mobile wallet) entry for the dealer.
        /// </summary>
        public ActionResult AddWallet(string walletnm, string walletno, string walletholdername)
        {
            string userid = User.Identity.GetUserId();
            tblwallet_info wall = new tblwallet_info();
            wall.createdate = DateTime.Now;
            wall.userid = userid;
            wall.walletholdername = walletholdername;
            wall.walletname = walletnm;
            wall.walletno = walletno;
            db.tblwallet_info.Add(wall);
            db.SaveChanges();
            return RedirectToAction("bankinfodealer");
        }

        /// <summary>
        /// Deletes a dealer bank account record by ID.
        /// </summary>
        public ActionResult Deletebank(int id)
        {
            var chk = db.bank_info.Where(aa => aa.idno == id).SingleOrDefault();
            db.bank_info.Remove(chk);
            db.SaveChanges();
            return Json("", JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Deletes a dealer wallet record by ID.
        /// </summary>
        public ActionResult Deletewallet(int id)
        {
            var chk = db.tblwallet_info.Where(aa => aa.walletid == id).SingleOrDefault();
            db.tblwallet_info.Remove(chk);
            db.SaveChanges();
            return Json("", JsonRequestBehavior.AllowGet);
        }






        /// <summary>
        /// Returns bank information for a dealer as JSON.
        /// </summary>
        public JsonResult ShowBankinfo(string dealerid)
        {
            var ch = db.Dealer_Details.Where(a => a.DealerId == dealerid).ToList();
            return Json(ch.ToArray(), JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// Updates existing bank account information for the dealer.
        /// </summary>
        [HttpPost]
        public ActionResult UpdateBankinfromation(string txtid3, string txtaccholder, string txtbankaccountno, string txtifsc, string txtbankname, string txtbranchaddress)
        {
            try
            {
                var dealerid = User.Identity.GetUserId();
                //  var ad = db.Dealer_Details.Single(a => a.DealerId == dealerid);
                var ad = db.bank_info.Where(a => a.userid == dealerid);
                bank_info bankmodel = new bank_info();
                bankmodel.holdername = string.IsNullOrWhiteSpace(txtaccholder) ? "" : txtaccholder;
                bankmodel.acno = string.IsNullOrWhiteSpace(txtbankaccountno) ? "" : txtbankaccountno;
                bankmodel.ifsccode = string.IsNullOrWhiteSpace(txtifsc) ? "" : txtifsc;
                bankmodel.banknm = string.IsNullOrWhiteSpace(txtbankname) ? "" : txtbankname;
                bankmodel.address = string.IsNullOrWhiteSpace(txtbranchaddress) ? "" : txtbranchaddress;
                bankmodel.createdate = DateTime.Now;
                db.bank_info.Add(bankmodel);
                db.SaveChanges();
            }
            catch
            {

            }
            TempData["success"] = "Update Successfully.";
            return RedirectToAction("Profile");
        }


        #region DealerIncome
        /// <summary>
        /// Displays the actual income report for the dealer including commission earned.
        /// </summary>
        public ActionResult Actual_Dealer_income()
        {
            var dealerid = User.Identity.GetUserId();
            string txt_to_date1 = DateTime.Now.ToString();
            string frm_date = Convert.ToDateTime(txt_to_date1).AddDays(-1).ToShortDateString();
            string to_date = Convert.ToDateTime(txt_to_date1).ToShortDateString();
            var ch = db.show_all_user_income(Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), "Dealer", dealerid).ToList();
            return View(ch);
        }
        /// <summary>
        /// Displays the actual dealer income report with date filtering.
        /// </summary>
        [HttpPost]
        public ActionResult Actual_Dealer_income(string txt_frm_date)
        {
            var dealerid = User.Identity.GetUserId();
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
            var ch = db.show_all_user_income(Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), "Dealer", dealerid).ToList();
            return View(ch);
        }
        #endregion
        //Upload Aadhaar Crad Doc
        /// <summary>
        /// Handles the upload of Aadhaar card document for the dealer.
        /// </summary>
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
                var dealerid = User.Identity.GetUserId();
                var ad = db.Dealer_Details.Single(a => a.DealerId == dealerid);
                ad.aadharcardPath = string.IsNullOrWhiteSpace(imagePath) ? ad.aadharcardPath : imagePath;
                ad.BackSideAadharcardphoto = string.IsNullOrWhiteSpace(imagePath2) ? ad.BackSideAadharcardphoto : imagePath2;
                db.SaveChanges();
                TempData["success"] = "Aadhaar Doucument Upload Successfully.";
                return RedirectToAction("Profile");
            }
            catch
            {

            }
            return RedirectToAction("Profile");
        }

        //Upload Pancard Crad Doc
        /// <summary>
        /// Handles the upload of PAN card document for the dealer.
        /// </summary>
        [HttpPost]
        public ActionResult UploadPancardcarddoc(string txtpancardid)
        {
            try
            {
                var dealerid = User.Identity.GetUserId();
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
                var ad = db.Dealer_Details.Single(a => a.DealerId == dealerid);
                ad.pancardPath = string.IsNullOrWhiteSpace(imagePath) ? ad.pancardPath : imagePath;
                db.SaveChanges();
                TempData["success"] = "Pancard Doucument Upload Successfully.";
                return RedirectToAction("Profile");
            }
            catch
            {

            }
            return RedirectToAction("Profile");
        }


        //Upload Service Aggrement Card Doc
        /// <summary>
        /// Handles the upload of service agreement document for the dealer.
        /// </summary>
        [HttpPost]
        public ActionResult UploadServiceAggrementdoc()
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
                var txtserviceid = User.Identity.GetUserId();
                var ad = db.Dealer_Details.Single(a => a.DealerId == txtserviceid);
                ad.serviceagreementpath = string.IsNullOrWhiteSpace(imagePath) ? ad.serviceagreementpath : imagePath;
                db.SaveChanges();
                TempData["success"] = "Service Agreement Doucument Upload Successfully.";
                return RedirectToAction("Profile");
            }
            catch
            {

            }
            return RedirectToAction("Profile");
        }

        //Upload Registraction Certificate Card Doc
        /// <summary>
        /// Handles the upload of registration certificate document for the dealer.
        /// </summary>
        [HttpPost]
        public ActionResult UploadRegistractionCertificatedoc()
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
                var txtRegistractionid = User.Identity.GetUserId();
                var ad = db.Dealer_Details.Single(a => a.DealerId == txtRegistractionid);
                ad.Registractioncertificatepath = string.IsNullOrWhiteSpace(imagePath) ? ad.Registractioncertificatepath : imagePath;
                db.SaveChanges();
                TempData["success"] = " Registraction Certificate Document Upload Successfully.";
                return RedirectToAction("Profile");
            }
            catch
            {

            }
            return RedirectToAction("Profile");
        }



        //Upload Registraction Certificate Card Doc
        /// <summary>
        /// Handles the upload of address proof document for the dealer.
        /// </summary>
        [HttpPost]
        public ActionResult UploadAddressProofdoc()
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
                var txtAddressproofid = User.Identity.GetUserId();
                var ad = db.Dealer_Details.Single(a => a.DealerId == txtAddressproofid);
                ad.AddressProofpath = string.IsNullOrWhiteSpace(imagePath) ? ad.AddressProofpath : imagePath;
                db.SaveChanges();
                TempData["success"] = "Address Proof Document Upload Successfully.";
                return RedirectToAction("Profile");
            }
            catch
            {

            }
            return RedirectToAction("Profile");
        }

        //Upload Profile Image
        /// <summary>
        /// Handles the upload of profile image for the dealer.
        /// </summary>
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
                var ad = db.Dealer_Details.Single(a => a.DealerId == txtprofileid);
                ad.Photo = string.IsNullOrWhiteSpace(imagePath) ? ad.Photo : imagePath;
                db.SaveChanges();
                TempData["success"] = "Profile Image Upload Successfully.";
                return RedirectToAction("Profile");
            }
            catch
            {

            }
            return RedirectToAction("Profile");
        }

        //delete Profile Doc 
        /// <summary>
        /// Deletes a profile document by dealer ID and document type name.
        /// </summary>
        public JsonResult DelereprofileDoc(string SSID, string Docname)
        {
            if (SSID != null && Docname == "Aadhaar")
            {
                var ad = db.Dealer_Details.Single(a => a.DealerId == SSID);
                ad.aadharcardPath = null;
                ad.BackSideAadharcardphoto = null;
                db.SaveChanges();
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            if (SSID != null && Docname == "Pancard")
            {
                var ad = db.Dealer_Details.Single(a => a.DealerId == SSID);
                ad.pancardPath = null;
                db.SaveChanges();
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            if (SSID != null && Docname == "ServiceAgrrement")
            {
                var ad = db.Dealer_Details.Single(a => a.DealerId == SSID);
                ad.serviceagreementpath = null;
                db.SaveChanges();
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            if (SSID != null && Docname == "RegistractionCertificate")
            {
                var ad = db.Dealer_Details.Single(a => a.DealerId == SSID);
                ad.Registractioncertificatepath = null;
                db.SaveChanges();
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            if (SSID != null && Docname == "AddressProof")
            {
                var ad = db.Dealer_Details.Single(a => a.DealerId == SSID);
                ad.AddressProofpath = null;
                db.SaveChanges();
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("Failed", JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// Returns a list of districts for the selected state as JSON.
        /// </summary>
        public JsonResult FillDistict(int State)
        {
            var cities = db.District_Desc.Where(c => c.State_id == State);
            return Json(cities, JsonRequestBehavior.AllowGet);
        }


        //Change Password 
        public void Passcode3(string ids)
        {



            if (ids != null && ids != "")
            {

                var details = db.Users.Where(s => s.Email == ids).SingleOrDefault();




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

                            setopt.forgetpin = Convert.ToString(pin);
                            db.SaveChanges();

                            apiurls = smsapionsts2.smsapi;
                            string text = "Your Reset Transaction Password otp is " + pin;
                            text = string.Format(text, "1230");

                            var apinamechange = apiurls.Replace("tttt", details.PhoneNumber).Replace("mmmm", text);

                            var client = new RestClient(apinamechange);
                            var request = new RestRequest(Method.GET);

                            VastBazaartoken Responsetoken = new VastBazaartoken();
                            var whatsts = db.Email_show_passcode.SingleOrDefault();
                            var userid = User.Identity.GetUserId();
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
                            sms.messagefor = details.UserId;
                            db.sms_api_entry.Add(sms);
                            db.SaveChanges();

                        }
                        var AdminDetails = db.Admin_details.SingleOrDefault();

                        smssend.SendEmailAll(details.Email, "Your  Reset Transaction Password otp is " + pin, "Transaction Password otp", AdminDetails.email);






                        // var smsapionsts = smsapi2.Where(s => s.api_type == "sms").SingleOrDefault();
                        //if (smsapionsts != null)
                        //{

                        //    apiurls = smsapionsts.smsapi;
                        //    string text = "Your New Password is  81108706 Thank you for business with us Regards Vast Web India Pvt Ltd";
                        //    text = string.Format(text, "1230");

                        //    var apinamechange = apiurls.Replace("tttt", details.PhoneNumber).Replace("mmmm", text);

                        //    var client = new RestClient(apinamechange);
                        //    var request = new RestRequest(Method.GET);

                        //    VastBazaartoken Responsetoken = new VastBazaartoken();

                        //    if (apinamechange.ToUpper().Contains("VASTBAZAAR.COM"))
                        //    {
                        //        var token = Responsetoken.gettoken();
                        //        request.AddHeader("authorization", "bearer " + token);
                        //        request.AddHeader("content-type", "application/json");
                        //    }
                        //    var task = Task.Run(() =>
                        //    {
                        //        return client.Execute(request).Content;
                        //    });
                        //    bool isCompletedSuccessfully = task.Wait(TimeSpan.FromSeconds(10000));
                        //    var resp = "";
                        //    if (isCompletedSuccessfully == true)
                        //    {
                        //        resp = task.Result;
                        //    }

                        //    sms_api_entry sms = new sms_api_entry();
                        //    sms.apiname = apinamechange;
                        //    sms.msg = text;
                        //    sms.m_date = System.DateTime.Now;
                        //    sms.response = resp;
                        //    sms.messagefor = details.UserId;
                        //    db.sms_api_entry.Add(sms);
                        //    db.SaveChanges();
                                                            
                        //}
                    }
                }
            }


        }
        /// <summary>
        /// Displays the change-password form for the dealer account.
        /// </summary>
        [HttpGet]
        public ActionResult ChangePassword()
        {
            ViewData["success"] = TempData["success"];
            ViewData["error"] = TempData["error"];
            TempData.Remove("success");
            TempData.Remove("error");

            var chk1 = db.Email_show_passcode.SingleOrDefault();
            if (chk1.forgetotp == true)
            {
                Session["chk1"] = "1";
            }
            else
            {
                Session["chk1"] = "";
            }

            var tuple = new Tuple<ChangePasswordViewModel, ChangeTransactionViewModel>(new ChangePasswordViewModel(), new ChangeTransactionViewModel());


            return View(tuple);
        }
        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        /// <summary>
        /// Displays the change password form and processes password change requests.
        /// </summary>
        public async Task<ActionResult> ChangePassword([Bind(Prefix = "Item1")] ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            if (model.ConfirmPassword.ToUpper() == model.OldPassword.ToUpper())
            {
                TempData["Messagewrong"] = "Please use Different Password.";
                return RedirectToAction("ChangePassword");
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
                TempData["success"] = "Your Password has been Changed Successfully..";
                var tuple = new Tuple<ChangePasswordViewModel, ChangeTransactionViewModel>(new ChangePasswordViewModel(), new ChangeTransactionViewModel());
                return RedirectToAction("ChangePassword", tuple);
            }
            AddErrors(result);
            return View(model);
        }

        /// <summary>
        /// Resets the transaction PIN using OTP verification.
        /// </summary>
        public ActionResult Reset_TransPin(string txtemail, string oTP, string oTP1, string oTP2, string oTP3)
        {
            string OTp = oTP + oTP1 + oTP2 + oTP3;
            if (oTP == null && oTP1 == null && oTP2 == null && oTP3 == null)
            {
                OTp = null;
            }


            var chk = db.Dealer_Details.Any(a => a.Email == txtemail);

            if (chk == true)
            {
                var chk1 = db.Dealer_Details.Where(a => a.Email == txtemail).SingleOrDefault();
                var optcheck = db.Users.Where(a => a.UserId == chk1.DealerId).SingleOrDefault();

                if (optcheck.forgetpin == OTp || OTp == null)
                {
                    int pin = new Random().Next(100000, 999999);
                    var msg = Encrypt(pin.ToString());
                    var userid = User.Identity.GetUserId();
                    var id = (from tbl in db.Dealer_Details
                              where tbl.DealerId == userid
                              select tbl).SingleOrDefault();
                    id.TransPIN = msg;
                    db.SaveChanges();
                    var emailid = db.Dealer_Details.Single(p => p.DealerId == userid).Email;
                    try
                    {
                        var ToCC = db.Admin_details.FirstOrDefault().email;
                        CommUtilEmail emailsend = new CommUtilEmail();
                        emailsend.EmailLimitChk(emailid, ToCC, "Transaction Pin", "Your Transaction Pin Is  " + pin, "No CallBackUrl");
                    }
                    catch { }
                    TempData["success"] = "Your Transaction Pin Send Successfully on Your Mail Id . Please Check Your Mail Id.";
                    optcheck.forgetpin = null;
                    db.SaveChanges();



                }
                else
                {
                    if (OTp == "")
                    {
                        TempData["error"] = "Please Enter Otp";
                    }
                    else
                    {
                        TempData["error"] = "Your Transaction Pin Reset OTP is Wrong Please Try Again";
                    }
                }
            }
            else
            {
                TempData["error"] = "Your Email Not Verify.Please Enter Your Corrent Registered Email";
            }

            return RedirectToAction("ChangePassword");
        }

        /// <summary>
        /// Processes the transaction PIN reset form submission.
        /// </summary>
        [HttpPost]
        public ActionResult Edit_Reset_TransPin([Bind(Prefix = "Item2")] ChangeTransactionViewModel model)
        {
            var userid = User.Identity.GetUserId();
            var oldpin = db.Dealer_Details.Single(p => p.DealerId == userid).TransPIN;
            var oldp = Decrypt(oldpin);
            if (oldp != model.Oldtransactionpass)
            {
                TempData["error"] = "Your Old Transaction Password does Not Match .Please Re-Enter Your Old Transaction Password ";
                return RedirectToAction("ChangePassword");
            }
            var msg = Encrypt(model.Newtransactionpass);

            var id = (from tbl in db.Dealer_Details
                      where tbl.DealerId == userid
                      select tbl).SingleOrDefault();
            id.TransPIN = msg;
            db.SaveChanges();
            var emailid = db.Dealer_Details.Single(p => p.DealerId == userid).Email;
            try
            {
                var ToCC = db.Admin_details.FirstOrDefault().email;
                CommUtilEmail emailsend = new CommUtilEmail();
                emailsend.EmailLimitChk(emailid, ToCC, "Transaction Pin", "Your New Transaction Pin Is  " + model.Newtransactionpass, "No CallBackUrl");
            }
            catch { }
            TempData["success"] = "Your Transaction Pin Change and Send Successfully on Your Mail Id . Please Check Your Mail Id.";
            return RedirectToAction("ChangePassword");
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        #endregion

        //check Retailer Outstanding and My Credit
        /// <summary>
        /// Returns a JSON result with the dealer's current credit balance and retailer outstanding balance.
        /// </summary>
        public ActionResult Chkbalance()
        {
            var userid = User.Identity.GetUserId();
            //Get Retailer Crdit Balance
            var retailercreditbal = db.total_Retailer_outstanding_by_dealer(userid).FirstOrDefault().totaloutstanding;
            //Dealer Credit to master
            var mastercreditbal = db.total_dealer_outstanding_by_master(userid).FirstOrDefault().totaloutstanding;
            //Dealer Credit to admin
            var admincreditbal = db.total_dealer_outstanding_by_admin(userid).FirstOrDefault().totaloutstanding;
            return Json(new
            {

                retailercreditbal = retailercreditbal,
                mastercreditbal = mastercreditbal,
                admincreditbal = admincreditbal
                //mastercreditbal = mastercreditbal
            });
        }
        //Today total balance all retailer and rcived bal from master and admin
        /// <summary>
        /// Displays the total balance transfer summary for the dealer.
        /// </summary>
        public ActionResult Totalbaltransfer()
        {
            var userid = User.Identity.GetUserId();
            //Today Total Dealer to Retaile Balance
            var dealertoretailer = db.total_retailer_bal_by_dealer(userid).FirstOrDefault().totalbal;
            //Today total balance by admin
            var admintodealer = db.dealer_Today_total_recive_bal_by_admin(userid).FirstOrDefault().totalbal;
            // Today total balance by master
            var mastertodealer = db.dealer_Today_total_recive_bal_by_master(userid).FirstOrDefault().totalbal;
            return Json(new
            {
                dealertoretailer = dealertoretailer,
                admintodealer = admintodealer,
                mastertodealer = mastertodealer

            });
        }
        /// <summary>
        /// Shows the retailer outstanding balance report.
        /// </summary>
        public ActionResult Show_Retailer_outstandingreport()
        {
            var userid = User.Identity.GetUserId();
            var show = db.Show_Retailer_credit_report_by_Dealer(userid).ToList();
            return View(show);
        }
        //credit report by admin
        /// <summary>
        /// Shows the credit report received from admin.
        /// </summary>
        public ActionResult Show_Credit_report_by_admin()
        {
            var userid = User.Identity.GetUserId();
            var show = db.select_Dealer_credit_report_by_admin(userid).ToList();
            return View(show);
        }
        //credit report by master
        /// <summary>
        /// Shows the credit report received from the master dealer.
        /// </summary>
        public ActionResult Show_Credit_report_by_master()
        {
            var userid = User.Identity.GetUserId();
            var show = db.select_Dealer_credit_report_by_master(userid).ToList();
            return View(show);
        }

        //Day Book Report
        /// <summary>
        /// Displays the dealer day-book report for the current date.
        /// </summary>
        [HttpGet]
        public ActionResult Dealer_Daybook_Report()
        {
            var userid = User.Identity.GetUserId();
            DayBookdealerreport rep = new DayBookdealerreport();
            string txt_frm_date = DateTime.Now.ToString();
            string txt_to_date = DateTime.Now.ToString();
            string frm_date = Convert.ToDateTime(txt_frm_date).Date.ToString();
            string to_date = Convert.ToDateTime(txt_to_date).AddDays(1).ToString();
            rep.DayBookLive = db.daybook_dealer_report(userid, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            return View(rep);
        }
        /// <summary>
        /// Displays the dealer daybook report with date range filtering.
        /// </summary>
        [HttpPost]
        public ActionResult Dealer_Daybook_Report(string txt_frm_date)
        {
            var userid = User.Identity.GetUserId();
            ViewBag.chk = "post";
            DayBookdealerreport rep = new DayBookdealerreport();
            string txtfrm = DateTime.Now.Date.ToString();
            string frm_date = Convert.ToDateTime(txt_frm_date).Date.ToString();
            string to_date = Convert.ToDateTime(txt_frm_date).AddDays(1).ToString();
            if (txtfrm == frm_date)
            {
                rep.DayBookLive = db.daybook_dealer_report(userid, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            }
            else
            {
                rep.DayBook_Old = db.daybook_dealer_old_report(userid, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            }
            return View(rep);

        }
        //Start Slab Setting 
        #region SlabSetting
        //GET : Show Slab Name 
        /// <summary>
        /// Displays the slab generation page for commission management.
        /// </summary>
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
                    Text = "Retailer",
                    Value = "Retailer"
                });
                ViewData["msg"] = TempData["msg"];
                ViewBag.slabfor = items;
                return View(viewModel);
            }
        }
        // Post : Add New Slab
        /// <summary>
        /// Adds a new commission slab name for the dealer.
        /// </summary>
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
                    var msg = db.insert_slab_list("Retailer", result.AccountVM.Slab_Name, userid, output).Single().slab;
                    TempData["msg"] = msg;
                    return RedirectToAction("generateSlab");
                }
                catch
                {
                    return RedirectToAction("generateSlab");
                }
            }
        }
        /// <summary>
        /// Deletes a commission slab name.
        /// </summary>
        public ActionResult Delete_slabName(string slabname)
        {
            try
            {
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    var userid = User.Identity.GetUserId();
                    System.Data.Entity.Core.Objects.ObjectParameter output = new System.Data.Entity.Core.Objects.ObjectParameter("Output", typeof(string));
                    var respo = db.proc_deleteSlabByName(userid, slabname, "Retailer", output).SingleOrDefault().msg;
                    if (respo == "SUCCESS")
                    {
                        TempData["msg"] = slabname + " deleted Successfully.";
                    }
                    else if (respo == "ASSIGNED")
                    {
                        TempData["msg"] = "Unable to delete assigned slab.";
                    }
                    return RedirectToAction("generateSlab");
                }
            }
            catch
            {
                return RedirectToAction("generateSlab");
            }
        }

        #endregion
        //End Slab Setting
        #region Account
        //Fund Transfer To Retailer
        //Fund Transfer To Retailer
        /// <summary>
        /// Displays the fund transfer page for sending funds to a retailer.
        /// </summary>
        public ActionResult SendFund()
        {

            string dealerid = User.Identity.GetUserId();
            var msg = TempData["result"];
            ViewData["output"] = msg;


            var stands = (from rem in db.Retailer_Details where rem.DealerId == dealerid select rem).ToList();
            IEnumerable<SelectListItem> selectList = from s in stands
                                                     select new SelectListItem
                                                     {
                                                         Value = s.RetailerId,
                                                         Text = s.Frm_Name + "--" + s.RetailerName.ToString()
                                                     };
            ViewBag.RetailerId = new SelectList(selectList, "Value", "Text");
            ViewBag.RetailerId1 = new SelectList(selectList, "Value", "Text");
            //fill dealer bank
            var account1 = (from bk in db.bank_info where bk.userid == dealerid select bk).ToList();
            IEnumerable<SelectListItem> bnkselectList = from s in account1
                                                        select new SelectListItem
                                                        {
                                                            Value = s.acno.ToString(),
                                                            Text = s.banknm.ToString()
                                                        };
            // var accunt1 = (from acc in db.bank_info.Where(a => a.userid == dealerid) select new { acno = acc.acno, acnooo = acc.banknm });
            ViewBag.fillaccount = bnkselectList;// new SelectList(bnkselectList, "acno", "acnooo", null);
                                                //  var wallet = (from wll in db.tblwallet_info.Where(a => a.userid == dealerid) select new { wallid = wll.walletno, wllnooo = wll.walletname });
                                                //fill dealer wallets
            var wallets = (from wll in db.tblwallet_info.Where(a => a.userid == dealerid) select new { wlno = wll.walletno, walletno = wll.walletname });
            ViewBag.walletsdlm = new SelectList(wallets, "wlno", "walletno", null);

            //fill bank Admin Acc
            var admin = (from acc in db.bank_info.Where(a => a.userid == "Admin") select new { acno = acc.acno, acnooo = acc.banknm });
            ViewBag.ddAdminaccountname = new SelectList(admin, "acno", "acnooo", null);
            //fill Wallets of Admin
            var adminwallet = (from wll in db.tblwallet_info.Where(a => a.userid == "Admin") select new { wlno = wll.walletno, wlname = wll.walletname });
            ViewBag.ddAdminwalletname = new SelectList(adminwallet, "wlno", "wlname", null);


            //find master of dealer
            var superid = db.Dealer_Details.FirstOrDefault(x => x.DealerId == dealerid).SSId;
            //fill bank of md
            var superaccunt = (from acc in db.bank_info.Where(x => x.userid == superid) select new { acno = acc.acno, acnooo = acc.banknm });
            ViewBag.superaccount = new SelectList(superaccunt, "acno", "acnooo", null);

            //fill wallet of master
            //  var MD = db.Dealer_Details.FirstOrDefault(x => x.DealerId == dealerid).SSId;
            var superwallet = (from wll in db.tblwallet_info.Where(x => x.userid == superid) select new { wlno = wll.walletno, wlname = wll.walletname });
            ViewBag.superwallet = new SelectList(superwallet, "wlno", "wlname", null);
            //dealer credit to master
            var mastercreditbal = db.total_dealer_outstanding_by_master(dealerid).FirstOrDefault().totaloutstanding;
            ViewData["oldcreditmaster"] = mastercreditbal;
            //Dealer Credit to admin
            var admincreditbal = db.total_dealer_outstanding_by_admin(dealerid).FirstOrDefault().totaloutstanding;
            ViewData["oldcreditadmin"] = admincreditbal;
            // ViewData["oldcredit"] = (mastercreditbal + admincreditbal);


            //  DateTime dt = !string.IsNullOrWhiteSpace(txt_frm_date) ? DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
            // DateTime dt1 = !string.IsNullOrWhiteSpace(txt_to_date) ? DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
            // DateTime frm_date = Convert.ToDateTime(dt).Date;
            //DateTime to_date = Convert.ToDateTime(dt1).Date.AddDays(1);


            FundRequestViewmodel vmodel = new FundRequestViewmodel();
            vmodel.DealerToRemFundTransfer = null;// db.select_dlm_rem(dealerid, Convert.ToDateTime(DateTime.Now), Convert.ToDateTime(DateTime.Now.AddDays(1)), "ALL", 1, 20).ToList();
            vmodel.PurchaseRequestRecived = null; // db.select_rem_pur_order("ALL", dealerid, Convert.ToDateTime(DateTime.Now), Convert.ToDateTime(DateTime.Now.AddDays(1))).ToList();
            vmodel.SendPurchaserequest = null;//db.select_dlm_pur_order(dealerid, "ALL", Convert.ToDateTime(DateTime.Now), Convert.ToDateTime(DateTime.Now.AddDays(1))).ToList();

            return View(vmodel);

        }

        /// <summary>
        /// Sends OTP to retailer mobile for chargeback authorization.
        /// </summary>
        public ActionResult ChargebackSendOTP(string retailermobile)
        {
            try
            {
                string dealerid = User.Identity.GetUserId();
                var email = db.Admin_details.SingleOrDefault().email;
                var cuurntuseremail = db.Retailer_Details.Where(r => r.Mobile == retailermobile).SingleOrDefault();
                string numbers = "0123456789";
                Random objrandom = new Random();
                string strrandom = string.Empty;
                for (int i = 0; i < 4; i++)
                {
                    int temp = objrandom.Next(0, numbers.Length);
                    strrandom += temp;
                }
                smssend.SendEmailAll(cuurntuseremail.Email, "OTP Verify for Retailer Charge back " + strrandom, "CHARGE BACK OTP", email, 1000);

                smssend.sms_init("Y", "Y", "CHARGEBACKOTP", cuurntuseremail.Mobile, strrandom);

                deleteuserotp saveOTP = new deleteuserotp();
                saveOTP.otp = Convert.ToInt32(strrandom);
                saveOTP.SentDate = DateTime.Now;
                saveOTP.userid = dealerid;
                db.deleteuserotps.Add(saveOTP);
                db.SaveChanges();
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("Failed", JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// Displays the dealer's saved bank account information.
        /// </summary>
        public ActionResult Bankinfo()
        {
            return View();
        }


        /// <summary>
        /// Displays the dealer held/pending commission reports.
        /// </summary>
        public ActionResult Dealerhold_comm_reports()
        {

            var userid = User.Identity.GetUserId();
            var userssmaster = db.masterhold_commisionTransfer_reports("dealer", userid).ToList();
            List<dealerCommon_status_master_dlm_Transfer_cls> listview1 = new List<dealerCommon_status_master_dlm_Transfer_cls>();
            foreach (var item in userssmaster)
            {

                dealerCommon_status_master_dlm_Transfer_cls model = new dealerCommon_status_master_dlm_Transfer_cls();
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

        /// <summary>
        /// Displays the dealer fund transfer dashboard with received and sent fund request tabs.
        /// </summary>
        public ActionResult FundTransferDealer(string tabname = "")
        {
            ViewData["successorder"] = TempData["successorder"];
            ViewData["failedorder"] = TempData["failedorder"];
            FundRequestViewmodel vmodel = new FundRequestViewmodel();
            string dealerid = User.Identity.GetUserId();
            var chargeback_otpsts = db.Dealer_Details.Where(a => a.DealerId == dealerid).Select(a => a.chargeback_otpsts).SingleOrDefault();
            int otpsts = chargeback_otpsts == true ? 1 : 0;
            ViewBag.otpsts = otpsts;
            var stands = (from rem in db.Retailer_Details where rem.DealerId == dealerid && rem.ISDeleteuser == false select rem).ToList();
            List<SelectListItem> items = new List<SelectListItem>();
            foreach (var item in stands)
            {
                items.Add(new SelectListItem { Text = item.Frm_Name + " / " + item.Mobile, Value = item.RetailerId.ToString() });
            }
            vmodel.ddlRetailer = items;
            var bindbank = db.bank_info.Where(x => x.userid == dealerid).ToList();
            List<SelectListItem> bankitem = new List<SelectListItem>();
            foreach (var bank in bindbank)
            {
                bankitem.Add(new SelectListItem { Text = bank.banknm, Value = bank.acno });
            }
            vmodel.ddlFillAllBank = bankitem;

            var bindwallet = db.tblwallet_info.Where(x => x.userid == dealerid).ToList();
            List<SelectListItem> walletitem = new List<SelectListItem>();
            foreach (var wallet in bindwallet)
            {
                walletitem.Add(new SelectListItem { Text = wallet.walletname, Value = wallet.walletno });
            }
            vmodel.ddlFillAllwallet = walletitem;
            TempData["tabActive"] = tabname;
            return View(vmodel);
        }


        /// <summary>
        /// Exports the fund transfer report to Excel with date and type filters.
        /// </summary>
        public ActionResult ExportExcellFundtransfer(string tabtype = "Reatiler", string txt_frm_date = "", string txt_to_date = "", string ddltype = "")
        {



            DataTable dtt = new DataTable("Grid");



            string dealerid = User.Identity.GetUserId();


            DateTime fromdate;
            DateTime Todate;

            if (string.IsNullOrEmpty(txt_frm_date) && string.IsNullOrEmpty(txt_frm_date))
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

            switch (tabtype)
            {
                case "RetailerToDLMREQ":
                    var result = db.select_rem_pur_order("ALL", dealerid, fromdate, Todate).ToList();
                    if (!string.IsNullOrEmpty(ddltype))
                    {
                        result = result.Where(x => x.remid == ddltype).ToList();
                    }

                    dtt.Columns.AddRange(new DataColumn[9] { new DataColumn("OrderNo"),
                                             new DataColumn("Status"),
                                            new DataColumn("RequestFrom"),
                                            new DataColumn("PaymentMode"),
                                            new DataColumn("Discription") ,
                                            new DataColumn("TotalAmount") ,
                                            new DataColumn("Charge") ,
                                            new DataColumn("NetAmount") ,
                                            new DataColumn("RequestDate") ,



                    });
                    foreach (var item in result)
                    {
                        dtt.Rows.Add(item.orderno, item.sts, item.RemEmail, item.paymode, item.utrno, item.amount, item.cashDepositCharge, item.finalAmount, item.reqdate);
                    }

                    // vmodel.PurchaseRequestRecived = result;
                    break;

                case "Admin":


                    // vmodel.SendPurchaserequest = db.select_dlm_pur_order(dealerid, "ALL", fromdate, Todate).ToList();


                    break;

                default:

                    var result1 = db.select_dlm_rem(dealerid, fromdate, Todate, "ALL", 1, 500).ToList();
                    if (!string.IsNullOrEmpty(ddltype))
                    {
                        result1 = result1.Where(x => x.RetailerId == ddltype).ToList();
                    }

                    dtt.Columns.AddRange(new DataColumn[13] { new DataColumn("Frm_Name"),
                                            new DataColumn("RechargeDate"),
                                            new DataColumn("Head"),
                                            new DataColumn("CollectionBy") ,
                                            new DataColumn("bal_type") ,
                                            new DataColumn("Balance") ,
                                            new DataColumn("commission") ,
                                            new DataColumn("TotalBal"),
                                            new DataColumn("oldcrbalance") ,
                                            new DataColumn("remain_amount") ,
                                            new DataColumn("remain_pre_amount") ,
                                            new DataColumn("remain_amount_dealer") ,
                                            new DataColumn("remain_pre_amount_dealer")


                    });
                    foreach (var item in result1)
                    {
                        dtt.Rows.Add(item.Frm_Name == null || item.Frm_Name == "" ? item.RetailerName : item.Frm_Name + "/" + item.Mobile, item.RechargeDate, item.Head, item.CollectionBy, item.bal_type, item.Balance, item.commission, item.TotalBal, item.oldcrbalance, item.remain_amount, item.remain_pre_amount, item.remain_amount_dealer, item.remain_pre_amount_dealer);
                    }

                    break;
            }
            var grid = new GridView();
            grid.DataSource = dtt;
            grid.DataBind();

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=MyExcelFile.xls");
            Response.ContentType = "application/ms-excel";

            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            grid.RenderControl(htw);

            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            return View("MyView");

        }

        /// <summary>
        /// Displays the wallet-based fund request form for the dealer.
        /// </summary>
        public ActionResult WalletRequestSend()
        {
            FundRequestViewmodel vmodel = new FundRequestViewmodel();
            string dealerid = User.Identity.GetUserId();
            var stands = (from rem in db.Retailer_Details where rem.DealerId == dealerid select rem).ToList();
            List<SelectListItem> items = new List<SelectListItem>();
            foreach (var item in stands)
            {
                items.Add(new SelectListItem { Text = item.Frm_Name + " / " + item.Mobile, Value = item.RetailerId.ToString() });
            }
            vmodel.ddlRetailer = items;
            var bindbank = db.bank_info.Where(x => x.userid == dealerid).ToList();
            List<SelectListItem> bankitem = new List<SelectListItem>();
            foreach (var bank in bindbank)
            {
                bankitem.Add(new SelectListItem { Text = bank.banknm, Value = bank.acno });
            }
            vmodel.ddlFillAllBank = bankitem;

            var bindwallet = db.tblwallet_info.Where(x => x.userid == dealerid).ToList();
            List<SelectListItem> walletitem = new List<SelectListItem>();
            foreach (var wallet in bindwallet)
            {
                walletitem.Add(new SelectListItem { Text = wallet.walletname, Value = wallet.walletno });
            }
            vmodel.ddlFillAllwallet = walletitem;

            return View(vmodel);
        }

        /// <summary>
        /// Displays the master-to-dealer fund transfer partial view.
        /// </summary>
        public PartialViewResult MDTODealer(string tabtype = "Reatiler", string txt_frm_date = "", string txt_to_date = "", string ddltype = "")
        {

            FundRequestViewmodel vmodel = new FundRequestViewmodel();

            string dealerid = User.Identity.GetUserId();


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

            switch (tabtype)
            {
                case "RetailerToDLMREQ":
                    var result = db.select_rem_pur_order("ALL", dealerid, fromdate, Todate).ToList();
                    if (!string.IsNullOrEmpty(ddltype))
                    {
                        result = result.Where(x => x.remid == ddltype).ToList();
                    }

                    vmodel.PurchaseRequestRecived = result;
                    break;

                case "Admin":


                    vmodel.SendPurchaserequest = db.select_dlm_pur_order(dealerid, "ALL", fromdate, Todate).ToList();


                    break;

                default:
                    vmodel.DealerToRemFundTransfer = db.select_dlm_rem(dealerid, fromdate, Todate, "ALL", 1, 3500).ToList();

                    var result1 = db.select_dlm_rem(dealerid, fromdate, Todate, "ALL", 1, 3500).ToList();
                    if (!string.IsNullOrEmpty(ddltype))
                    {
                        result1 = result1.Where(x => x.RetailerId == ddltype).ToList();
                    }

                    vmodel.DealerToRemFundTransfer = result1;


                    break;

            }



            return PartialView("_FundTransferDealetToRetailerPartial", vmodel);

        }

        /// <summary>
        /// Shows pending retailer requests for the dealer.
        /// </summary>
        public ActionResult RetailerReuest()
        {
            string dealerid = User.Identity.GetUserId();
            var result = db.rem_purchage.Where(x => x.frm == dealerid && x.sts == "Pending").Count();  // db.select_dlm_pur_order(dealerid, "ALL", DateTime.Now.AddDays(-1), DateTime.Now.AddDays(1)).ToList().Count();
            if (result == 0)
            {
                result = 0;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        #region SignalR
        /// <summary>
        /// Test endpoint for SignalR real-time notification functionality.
        /// </summary>
        public ActionResult TestSignlR()
        {
            var userid = User.Identity.Name;
            SendPushNotification(userid, "https://www.google.com", "Testing", "SignalR");
            return View();
        }
        public void SendPushNotification(string ReceiverMailID, string RedirectUrl, string Message, string Title)
        {
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

        /// <summary>
        /// Displays the send fund page and processes fund transfer to retailers.
        /// </summary>
        [HttpPost]
        public ActionResult SendFund(string txt_frm_date, string txt_to_date, string RetailerId1)
        {

            var dealerid = User.Identity.GetUserId();
            ViewBag.chk = "post";

            var stands = (from dlm in db.Retailer_Details where dlm.DealerId == dealerid select dlm).ToList();
            IEnumerable<SelectListItem> selectList = from s in stands
                                                     select new SelectListItem
                                                     {
                                                         Value = s.RetailerId,
                                                         Text = s.Email + "--" + s.RetailerName.ToString()
                                                     };
            ViewBag.RetailerId = new SelectList(selectList, "Value", "Text");
            ViewBag.RetailerId1 = new SelectList(selectList, "Value", "Text");

            return View();
        }
        /// <summary>
        /// Partial view action for the send fund section.
        /// </summary>
        public ActionResult _SendFund(string txt_frm_date, string txt_to_date, string RetailerId1)
        {
            if (txt_frm_date == null && txt_to_date == null)
            {

                txt_frm_date = DateTime.Now.ToString();
                txt_to_date = DateTime.Now.ToString();

            }

            var dealerid = User.Identity.GetUserId();
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
            if (RetailerId1 == "" || RetailerId1 == null)
            {
                RetailerId1 = "ALL";
            }

            var ch = db.select_dlm_rem(dealerid, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), RetailerId1, 1, 20).ToList();
            return View(ch);
        }
        /// <summary>
        /// Returns paginated fund send history for infinite scroll.
        /// </summary>
        public ActionResult InfiniteScroll_Sendfund(int pageindex, string txt_frm_date, string txt_to_date, string RetailerId1)
        {
            if (txt_frm_date == null && txt_to_date == null)
            {

                txt_frm_date = DateTime.Now.ToString();
                txt_to_date = DateTime.Now.ToString();

            }
            var dealerid = User.Identity.GetUserId();
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
            if (RetailerId1 == "" || RetailerId1 == null)
            {
                RetailerId1 = "ALL";
            }
            int pagesize = 20;
            var tbrow = db.select_dlm_rem(dealerid, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), RetailerId1, pageindex, pagesize).ToList();
            JsonModel jsonmodel = new JsonModel();
            jsonmodel.NoMoredata = tbrow.Count < pagesize;
            jsonmodel.HTMLString = renderPartialViewtostring("_SendFund", tbrow);
            return Json(jsonmodel);
        }

        /// <summary>
        /// Partial view for the secondary send fund section.
        /// </summary>
        public ActionResult _SendFund1(string txt_frm_date1, string txt_to_date1)
        {
            ViewBag.chk = "post";
            string userid = User.Identity.GetUserId();

            if (txt_frm_date1 == null && txt_to_date1 == null)
            {
                txt_frm_date1 = DateTime.Now.ToString();
                txt_to_date1 = DateTime.Now.ToString();
            }

            DateTime frm = Convert.ToDateTime(txt_frm_date1);
            DateTime to = Convert.ToDateTime(txt_to_date1);
            txt_frm_date1 = frm.ToString("dd-MM-yyyy");
            txt_to_date1 = to.ToString("dd-MM-yyyy");
            string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy",
                        "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
            DateTime dt = !string.IsNullOrWhiteSpace(txt_frm_date1) ? DateTime.ParseExact(txt_frm_date1, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
            DateTime dt1 = !string.IsNullOrWhiteSpace(txt_to_date1) ? DateTime.ParseExact(txt_to_date1, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
            DateTime frm_date = Convert.ToDateTime(dt).Date;
            DateTime to_date = Convert.ToDateTime(dt1).Date.AddDays(1);
            var ch = db.select_rem_pur_order("ALL", userid, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            return Json(ch, JsonRequestBehavior.AllowGet);

            //return Json(ch, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Displays fund transfer detail view by transaction ID.
        /// </summary>
        [HttpPost]
        public ActionResult SendFund1_View(int Idno)
        {

            //   var detail = db.selectrem_pur_order_view(Idno);
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

        /// <summary>
        /// Validates the OTP for chargeback authorization.
        /// </summary>
        public JsonResult CheckOTPChargeback(int otps)
        {
            var adminuserid = User.Identity.GetUserId();

            var delaerchargebackotp = (db.deleteuserotps.Where(aa => aa.userid == adminuserid).OrderByDescending(aa => aa.id).Select(aa => aa.otp).FirstOrDefault());
            if (delaerchargebackotp == otps)
            {
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            return Json("Failed", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Generates a unique transaction ID for dealer-to-REM fund transfers.
        /// </summary>
        public JsonResult Fundtransfer_dlm_to_Rem_Generate_Unique_ID()
        {
            var adminuserid = User.Identity.GetUserId();



            string transferids = GenerateUniqueTransectionID();
            var transferid = "DR" + transferids;



            TempData["transferdlmtorem"] = transferid;
            return Json(transferid, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Validates security code and processes dealer fund transfer.
        /// </summary>
        public ActionResult ChkSecurityDealerToFundTransfer(string txtcode, string hdSuperstokistID, string hdMDDLM, string hdPaymentMode,
    string hdPaymentAmount, string hdMDDepositeSlipNo, string hdMDTransferType, string hdMDcollection, string hdMDComments,
    string hdMDBank, string hdsupraccno, string hdMDaccountno, string hdMDutrno, string hdMDwallet,
    string hdMDwalletno, string hdMDtransationno, string hdMDsettelment, string hdMDCreditDetail, string hdMDsubject)
        {
            try
            {
                var ch = "";

                TempData.Keep("dlmid");
                TempData.Keep("bal");
                TempData.Keep("fundtype");
                TempData.Keep("comment");


                string userid = User.Identity.GetUserId();
                var pwd = db.Dealer_Details.Where(x => x.DealerId == userid).FirstOrDefault().TransPIN;
                var passwordget = Decrypt(pwd);
                var password = Encrypt(txtcode);
                var tranpass = (from paa in db.Dealer_Details where paa.TransPIN == password && paa.DealerId == userid select paa).Count();
                if (tranpass > 0)
                {
                    string transferid = null;
                    try
                    {
                        transferid = TempData["transferdlmtorem"].ToString();
                    }
                    catch (Exception ex)
                    {
                        return Json("Please Try After SomeTime", JsonRequestBehavior.AllowGet);
                    }

                    var counts = db.FundTransfercount(userid, hdMDDLM, hdPaymentMode, Convert.ToDecimal(hdPaymentAmount), transferid, "Admintoretailer").SingleOrDefault().msg;
                    int msgcount = Convert.ToInt32(counts);
                    if (msgcount == 0)
                    {
                        msgcount = 60001;
                    }
                    int max_limit = 60000;
                    if (msgcount > max_limit)
                    {
                        // var dealeremail = db.Users.Where(p => p.UserId == DealerId).Single().Email;
                        string balance = hdPaymentAmount;
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


                        if (!string.IsNullOrEmpty(hdMDTransferType) && hdPaymentMode == "Online Transfer")
                        {
                            hdPaymentMode = hdPaymentMode + "/" + hdMDTransferType;
                        }

                        var email = db.Users.Where(p => p.UserId == userid).Single().Email;
                        var useremail = db.Users.Where(p => p.UserId == hdMDDLM).Single().Email;
                        System.Data.Entity.Core.Objects.ObjectParameter output = new System.Data.Entity.Core.Objects.ObjectParameter("output", typeof(String));
                        var diff1 = (db.Dealer_To_Retailer_Balance.Where(aa => aa.RetailerId == hdMDDLM && aa.DealerId == userid).OrderByDescending(aa => aa.id).Select(aa => aa.cr).FirstOrDefault());
                        diff1 = diff1 ?? 0;

                        //   var ch = db.dealer_to_rem_fund_report(from, to, "ALL", "ALL").ToList();
                        // var ch = "";
                        //var tp = "";
                        decimal amount = Convert.ToDecimal(balance);
                        if (amount > 0)
                        {
                            //  if (ddl_fund_type == "Cash" || ddl_fund_type == "Credit")
                            // {
                            ch = db.insert_dealer_to_retailer_balance(userid, hdMDDLM, Convert.ToDecimal(balance), 0, hdPaymentMode, comment, collectionby, bankname, adminacco, "Direct", "Dealer", transferid, output).Single().msg;
                            try
                            {
                               var dealerdetails = db.Dealer_Details.Where(aa => aa.DealerId == userid).SingleOrDefault();
                                var retailerdetails = db.Retailer_Details.Where(aa => aa.RetailerId == hdMDDLM).SingleOrDefault();

                                var remdetails = db.Remain_reteller_balance.Where(aa => aa.RetellerId == hdMDDLM).SingleOrDefault();
                                var dlmdetails = db.Remain_dealer_balance.Where(aa => aa.DealerID == userid).SingleOrDefault();
                              
                                var admininfo = db.Admin_details.SingleOrDefault();
                                Backupinfo back = new Backupinfo();
                                var modeln = new Backupinfo.Addinfo
                                {
                                    Websitename = admininfo.WebsiteUrl,
                                    RetailerID = hdMDDLM,
                                    Email = retailerdetails.Email,
                                    Mobile = retailerdetails.Mobile,
                                    Details = "Fund Recived From Dealer ",
                                    RemainBalance = (decimal)remdetails.Remainamount,
                                    Usertype = "Retailer"
                                };
                                back.Fundtransfer(modeln);

                                var model1 = new Backupinfo.Addinfo
                                {
                                    Websitename = admininfo.WebsiteUrl,
                                    RetailerID = userid,
                                    Email = dealerdetails.Email,
                                    Mobile = dealerdetails.Mobile,
                                    Details = "Fund Transfer To Retailer ",

                                    RemainBalance = Convert.ToDecimal(dlmdetails.Remainamount),
                                    Usertype = "Dealer"
                                };
                                back.Fundtransfer(model1);

                              
                            }
                            catch { }
                            // }
                            var Websitename = db.Admin_details.Single().WebsiteUrl;
                            if (ch == "Balance Transfer Successfully.")
                            {
                                diff1 = (db.Dealer_To_Retailer_Balance.Where(aa => aa.RetailerId == hdMDDLM && aa.DealerId == userid).OrderByDescending(aa => aa.id).Select(aa => aa.cr).FirstOrDefault());
                                diff1 = diff1 ?? 0;
                                var remainretailer = db.Remain_reteller_balance.Where(p => p.RetellerId == hdMDDLM).Single().Remainamount;
                                var statusDealer = db.PushNotificationStatus.Where(a => a.UserRole == "Distributor").SingleOrDefault().Status;
                                var statusRetailer = db.PushNotificationStatus.Where(a => a.UserRole == "Retailer").SingleOrDefault().Status;

                                var statusSendSmsDlmToRetailerFundTransfer = db.SMSSendAlls.Where(a => a.ServiceName == "DLMtoRemRemFundTrans").SingleOrDefault();
                                var statusSendMailDlmToRetailerFundTransfer = db.EmailSendAlls.Where(a => a.ServiceName == "DLMtoRemRemFundTrans1").SingleOrDefault().Status;

                                var RetailerDetails = db.Retailer_Details.Where(p => p.RetailerId == hdMDDLM).Single();

                                var statusSendSmsDlmToRetailerFundTransferDlm = db.SMSSendAlls.Where(a => a.ServiceName == "DLMtoRemdlmFundTrans").SingleOrDefault();
                                var statusSendMailDlmToRetailerFundTransferDlm = db.EmailSendAlls.Where(a => a.ServiceName == "DLMtoRemdlmFundTrans1").SingleOrDefault().Status;

                                var DealerDetails = db.Dealer_Details.Where(p => p.DealerId == userid).Single();
                                var AdminEmail = db.Admin_details.Single().email;

                                if (hdPaymentMode == "Credit")
                                {
                                    //if (statusSendSmsDlmToRetailerFundTransfer == "Y")
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

                                    //            msgssss = string.Format(smsstypes.Templates, email, balance, remainretailer, diff1);
                                    //            tempid = smsstypes.Templateid;
                                    //            urlss = smsapionsts.smsapi;

                                    //            smssend.sendsmsallnew(RetailerDetails.Mobile, msgssss, urlss, tempid);
                                    //        }
                                    //    }
                                    //    catch { }
                                    //    //  smssend.sendsmsall(RetailerDetails.Mobile, "Credit Received by " + email + " Rs." + balance + ".New Balance is " + remainretailer + ".Your O/s Credit is " + diff1 + "", "Fund Transfer");
                                    //}

                                    smssend.sms_init(statusSendSmsDlmToRetailerFundTransfer.Status, statusSendSmsDlmToRetailerFundTransfer.Whatsapp_Status, "SEND_SMS_CREDIT_RECEIVEDBY", RetailerDetails.Mobile, email, balance, remainretailer, diff1);

                                    //if (statusSendSmsDlmToRetailerFundTransferDlm == "Y")
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

                                    //            msgssss = string.Format(smsstypes.Templates, useremail, balance, diff1);
                                    //            tempid = smsstypes.Templateid;
                                    //            urlss = smsapionsts.smsapi;

                                    //            smssend.sendsmsallnew(DealerDetails.Mobile, msgssss, urlss, tempid);
                                    //        }
                                    //    }
                                    //    catch { }
                                    //    // smssend.sendsmsall(DealerDetails.Mobile, "Credit Transferred To " + useremail + " Rs. " + balance + ".Total Credit(O/s) Balance is " + diff1 + "", "Fund Transfer");
                                    //}

                                    smssend.sms_init(statusSendSmsDlmToRetailerFundTransferDlm.Status, statusSendSmsDlmToRetailerFundTransferDlm.Whatsapp_Status, "SEND_SMS_CREDIT_RECEIVEDBY", DealerDetails.Mobile, useremail, balance, diff1);

                                    if (statusSendMailDlmToRetailerFundTransfer == "Y")
                                    {
                                        smssend.SendEmailAll(RetailerDetails.Email, "Credit Received by " + email + " Rs." + balance + ".New Balance is " + remainretailer + ".Your O/s Credit is " + diff1 + "", "Fund Transfer", AdminEmail, 1000);
                                    }
                                    if (statusSendMailDlmToRetailerFundTransferDlm == "Y")
                                    {
                                        smssend.SendEmailAll(DealerDetails.Email, "Credit Transferred To " + useremail + " Rs. " + balance + ".Total Credit(O/s) Balance is " + diff1 + "", "Fund by Dealer To CC", AdminEmail, 1000);
                                    }

                                    //if (statusDealer == "Y")
                                    //{
                                    //    SendPushNotification(email, Url.Action("SendFund", "Home"), "Credit Transferred Rs. " + balance + ".Total Credit(O/s) Balance is " + diff1 + "", "Fund Transfer ..");
                                    //}
                                    //if (statusRetailer == "Y")
                                    //{
                                    //    SendPushNotification(useremail, Websitename + "/RETAILER/Home/FundRecive", "Credit Received Rs." + balance + ".New Balance is " + remainretailer + ".Your O/s Credit is " + diff1 + "", "Fund Transfer ..");
                                    //}
                                    notify.sendmessage(useremail, "Credit Received Rs." + balance + ".New Balance is " + remainretailer + ".Your O/s Credit is " + diff1 + "");
                                    TempData["result"] = ch;
                                }
                                else
                                {
                                    var retailername = db.Retailer_Details.Where(p => p.RetailerId == hdMDDLM).Single().RetailerName;
                                    var dealername = db.Dealer_Details.Where(p => p.DealerId == userid).Single().DealerName;
                                    //if (statusSendSmsDlmToRetailerFundTransfer == "Y")
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

                                    //            msgssss = string.Format(smsstypes.Templates, balance, dealername, remainretailer, diff1);
                                    //            tempid = smsstypes.Templateid;
                                    //            urlss = smsapionsts.smsapi;

                                    //            smssend.sendsmsallnew(RetailerDetails.Mobile, msgssss, urlss, tempid);
                                    //        }
                                    //    }
                                    //    catch { }


                                    //    //  smssend.sendsmsall(RetailerDetails.Mobile, "Cash Paid Rs." + balance + " to " + dealername + ". New Balance is " + remainretailer + ". Your O/s Credit is " + diff1 + "", "Fund Transfer");
                                    //}

                                    smssend.sms_init(statusSendSmsDlmToRetailerFundTransfer.Status, statusSendSmsDlmToRetailerFundTransfer.Whatsapp_Status, "FUNDTRANSFERMESSAGE_CASH", RetailerDetails.Mobile, balance, dealername, remainretailer, diff1);

                                    //if (statusSendSmsDlmToRetailerFundTransferDlm == "Y")
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

                                    //            msgssss = string.Format(smsstypes.Templates, balance, retailername, diff1);
                                    //            tempid = smsstypes.Templateid;
                                    //            urlss = smsapionsts.smsapi;

                                    //            smssend.sendsmsallnew(DealerDetails.Mobile, msgssss, urlss, tempid);
                                    //        }
                                    //    }
                                    //    catch { }


                                    //    // smssend.sendsmsall(DealerDetails.Mobile, "Cash Recived Rs." + balance + " From " + retailername + ",his O/s Credit is " + diff1 + "", "Fund Transfer");
                                    //}

                                    smssend.sms_init(statusSendSmsDlmToRetailerFundTransferDlm.Status, statusSendSmsDlmToRetailerFundTransferDlm.Whatsapp_Status, "SENDSMSMASTERTODLMFUNDTRANSFERMSGCASH", DealerDetails.Mobile, balance, retailername, diff1);

                                    if (statusSendMailDlmToRetailerFundTransfer == "Y")
                                    {
                                        smssend.SendEmailAll(RetailerDetails.Email, "Cash Paid Rs." + balance + " to " + dealername + ". New Balance is " + remainretailer + ". Your O/s Credit is " + diff1 + "", "Fund Transfer", AdminEmail, 1000);
                                    }
                                    if (statusSendMailDlmToRetailerFundTransferDlm == "Y")
                                    {
                                        smssend.SendEmailAll(DealerDetails.Email, "Cash Recived Rs." + balance + " From " + retailername + ",his O/s Credit is " + diff1 + "", "Fund by Dealer To CC", AdminEmail, 1000);
                                    }
                                    //if (statusDealer == "Y")
                                    //{
                                    //    SendPushNotification(email, Url.Action("SendFund", "Home"), "Cash Recived Rs." + balance + " From " + retailername + ",his O/s Credit is " + diff1 + "", "Fund Transfer ..");
                                    //}
                                    //if (statusRetailer == "Y")
                                    //{
                                    //    SendPushNotification(useremail, Websitename + "/RETAILER/Home/FundRecive", "Cash Paid Rs." + balance + " to " + dealername + ". New Balance is " + remainretailer + ". Your O/s Credit is " + diff1 + "", "Fund Transfer ..");
                                    //}
                                    notify.sendmessage(useremail, "Cash Paid Rs." + balance + " to " + dealername + ". New Balance is " + remainretailer + ". Your O/s Credit is " + diff1 + "");
                                    TempData["result"] = ch;
                                    return Json(ch, JsonRequestBehavior.AllowGet);
                                }

                                return Json(ch, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {

                                return Json(ch, JsonRequestBehavior.AllowGet);
                            }

                        }
                        else
                        {
                            // TempData["result"] = "Amount should be not zero";
                            ch = "Amount should be not zero";
                            return Json(ch, JsonRequestBehavior.AllowGet);

                        }
                    }
                    else
                    {
                        return Json("Your Previous Request IN Process Please Try After SomeTime", JsonRequestBehavior.AllowGet);
                    }


                }
                else
                {
                    ch = "Transection Pin Wrong";
                    return Json(ch, JsonRequestBehavior.AllowGet);

                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }

            //return Json(ch, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Returns the partial view for the send fund section.
        /// </summary>
        public ActionResult _SendFundPartial()
        {
            FundRequestViewmodel vmodel = new FundRequestViewmodel();
            vmodel.DealerToRemFundTransfer = null;
            vmodel.PurchaseRequestRecived = null;
            vmodel.SendPurchaserequest = null;
            string dealerid = User.Identity.GetUserId();
            vmodel.DealerToRemFundTransfer = db.select_dlm_rem(dealerid, Convert.ToDateTime(DateTime.Now.AddDays(-1)), Convert.ToDateTime(DateTime.Now.AddDays(1)), "ALL", 1, 20).ToList();

            return PartialView("_SendFundPartial", vmodel);
        }

        /// <summary>
        /// Binds all fund transaction grids based on type and date filters.
        /// </summary>
        public ActionResult BindAllFundGrid(string tabtype, string hdtype, string txt_frm_daterecived, string txt_to_daterecived)
        {
            FundRequestViewmodel vmodel = new FundRequestViewmodel();
            vmodel.DealerToRemFundTransfer = null;
            vmodel.PurchaseRequestRecived = null;
            vmodel.SendPurchaserequest = null;

            string dealerid = User.Identity.GetUserId();
            if (tabtype == "FUNDTORETailer" || hdtype == "FUNDTORETailer")
            {
                vmodel.DealerToRemFundTransfer = db.select_dlm_rem(dealerid, DateTime.Now.AddDays(-1), DateTime.Now.AddDays(1), "ALL", 1, 20).ToList();
                return PartialView("_SendFundPartial", vmodel);
            }
            else if (tabtype == "FUNDTOMD" || hdtype == "FUNDTOMD")
            {
                vmodel.SendPurchaserequest = db.select_dlm_pur_order(dealerid, "ALL", DateTime.Now.AddDays(-1), DateTime.Now.AddDays(1)).ToList();
                return PartialView("_PurchaserequestDLMTOMDADMIN", vmodel);

            }
            else if (tabtype == "FUNDRecived" || hdtype == "FUNDRecived")
            {
                vmodel.PurchaseRequestRecived = db.select_rem_pur_order("ALL", dealerid, DateTime.Now.AddDays(-1), DateTime.Now.AddDays(1)).ToList();
                return PartialView("_PurchaserequestRecived", vmodel);

            }
            return View("SendFund");
        }



        /// <summary>
        /// Places a purchase/recharge request for the dealer.
        /// </summary>
        public ActionResult PlacePurchaseRequest(string payto, string txtcode, string hdSuperstokistID, string hdMDDLM, string hdPaymentMode,
     string hdPaymentAmount, string hdMDDepositeSlipNo, string hdMDTransferType, string hdMDcollection, string hdMDComments,
     string hdMDBank, string hdsupraccno, string hdMDaccountno, string hdMDutrno, string hdMDwallet,
     string hdMDwalletno, string hdMDtransationno, string hdMDsettelment, string hdMDCreditDetail, string hdMDsubject)
        {


            string ch = "";

            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                try
                {
                    WebImage photo = null;
                    var newFileName = "";
                    var imagePath = "";
                    // string balance = hdPaymentAmount;
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
                    string userid = User.Identity.GetUserId();
                    var dealerid = db.Dealer_Details.Where(aa => aa.DealerId == userid).SingleOrDefault().DealerId;

                    var dealercount = db.Dealer_Details.Where(aa => aa.DealerId == userid).Count();

                    var count = (db.dlm_purchage.Where(aa => aa.sts == "Pending" && aa.dlmid == userid).Count());
                    int countchk = Convert.ToInt32(count);
                    var acc = "";
                    //decimal ChargesApply = 0;
                    //if (countchk < 1)
                    //{
                    var amount = Convert.ToDecimal(hdPaymentAmount);

                    if (amount > 0)
                    {
                        if (dealercount > 0)
                        {

                            var from = "";
                            var frm = "";

                            if (payto == "Master")
                            {
                                from = (db.Dealer_Details.Where(aa => aa.DealerId == userid).Single().SSId);
                                acc = adminacco;
                                frm = from;

                            }
                            else
                            {
                                from = db.Admin_details.Single().userid;
                                acc = adminacco;
                                frm = "Admin";
                            }
                            var diff1 = (db.admin_to_dealer.Where(aa => aa.dealerid == userid && aa.balance_from == from).OrderByDescending(aa => aa.idno).Select(c => c.cr).FirstOrDefault());
                            diff1 = diff1 ?? 0;
                            decimal diff = Convert.ToDecimal(diff1);
                            decimal disCharge = 0;
                            creditchargedealer chargeEntry = null;
                            if (hdPaymentMode == "Credit")
                            {
                                //  var chargeEntry = db.PurchaseOrderCashDepositCharges.FirstOrDefault(a => a.PurchaseOrderType == "Credit");
                                chargeEntry = db.creditchargedealers.Where(aa => aa.chargefrom == frm && aa.userid == userid).FirstOrDefault(a => a.type == "Credit");
                            }
                            else if (hdPaymentMode == "Cash")
                            {

                                chargeEntry = db.creditchargedealers.Where(aa => aa.chargefrom == frm && aa.userid == userid).FirstOrDefault(a => a.type == "Cash");


                            }
                            else if (hdPaymentMode == "Branch / CMS Deposite")
                            {
                                chargeEntry = db.creditchargedealers.Where(aa => aa.chargefrom == frm && aa.userid == userid).FirstOrDefault(a => a.type == "Cash");
                            }
                            if (chargeEntry != null)
                            {

                                disCharge = (amount * chargeEntry.charge.Value) / 100;

                            }
                            db.insert_dlmpurchageorder(dealerid, hdPaymentMode, collectionby, bankname, "", comment, Convert.ToDecimal(amount), payto, "", acc, "", "", "", "", "", disCharge, amount - disCharge);

                            if (payto != "Master")
                            {
                                var Dealer_data = db.Dealer_Details.Where(s => s.DealerId == userid).SingleOrDefault();
                                var Dealer_name = Dealer_data.DealerName;
                                var Dealer_no = Dealer_data.Mobile;
                                string apiurls = "";




                                if (db.apisms.Any(s => s.sts == "Y"))
                                {
                                    var asd = db.SMSSendAlls.Where(s => s.ServiceName == "dlmtoAdminfundtrans1" && s.Whatsapp_Status == "Y").ToList();
                                    var smsapi = db.apisms.Where(x => x.sts == "Y").ToList();
                                    var mobile12 = db.Admin_details.SingleOrDefault().mobile;
                                    var smsapionsts = smsapi.Where(s => s.api_type == "whatsapp").SingleOrDefault();

                                    if (smsapionsts != null)
                                    {
                                        if (asd.Any())
                                        {
                                            apiurls = smsapionsts.smsapi;
                                            string text = Dealer_name + "-" + Dealer_data.FarmName + "(" + Dealer_no + ") is Requested to you for perchase order of " + hdPaymentAmount + "Rs";
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
                                    var asd1 = db.SMSSendAlls.Where(s => s.ServiceName == "dlmtoAdminfundtrans1" && s.Status == "Y").ToList();
                                    var smsapionsts1 = smsapi.Where(s => s.api_type == "sms").SingleOrDefault();
                                    if (smsapionsts1 != null)
                                    {
                                        if (asd1.Any())
                                        {
                                            apiurls = smsapionsts1.smsapi;
                                            string text = Dealer_name + "-" + Dealer_data.FarmName + "(" + Dealer_no + ") is Requested to you for perchase order of " + hdPaymentAmount + "Rs";
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

                                var emailcheck = db.EmailSendAlls.Where(s => s.ServiceName == "dlmtoAdminfundtrans1" && s.Status == "Y").ToList();

                                if (emailcheck.Any())
                                {
                                    var AdminDetails = db.Admin_details.SingleOrDefault();
                                    smssend.SendEmailAll(AdminDetails.email, Dealer_name + "-" + Dealer_data.FarmName + "(" + Dealer_no + ") is Requested to you for perchase order of " + hdPaymentAmount + "Rs", "Purchase Order Request", AdminDetails.email);

                                    

                                }




                            }
                            else if (payto == "Master")
                            {

                                var dlm_data = db.Dealer_Details.Where(s => s.DealerId == userid).SingleOrDefault();
                                var dlm_name = dlm_data.DealerName;
                                var dlm_no = dlm_data.Mobile;
                                string apiurls = "";
                                var md_data = db.Superstokist_details.Where(s => s.SSId == dlm_data.SSId).FirstOrDefault();
                                var mobile12 = md_data.Mobile;



                                if (db.apisms.Any(s => s.sts == "Y"))
                                {
                                    var asd = db.SMSSendAlls.Where(s => s.ServiceName == "DLMToDLMMDFundTrans1" && s.Whatsapp_Status == "Y").ToList();

                                    var smsapi = db.apisms.Where(x => x.sts == "Y").ToList();

                                    var smsapionsts = smsapi.Where(s => s.api_type == "whatsapp").SingleOrDefault();
                                    if (smsapionsts != null)
                                    {
                                        if (asd.Any())
                                        {
                                            apiurls = smsapionsts.smsapi;
                                            string text = dlm_name + "-" + dlm_data.FarmName + "(" + dlm_no + ") is Requested to you for perchase order of " + hdPaymentAmount + "Rs";
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
                                    var asd1 = db.SMSSendAlls.Where(s => s.ServiceName == "DLMToDLMMDFundTrans1" && s.Status == "Y").ToList();
                                    var smsapionsts1 = smsapi.Where(s => s.api_type == "sms").SingleOrDefault();
                                    if (smsapionsts1 != null)
                                    {
                                        if (asd1.Any())
                                        {
                                            apiurls = smsapionsts1.smsapi;
                                            string text = dlm_name + "-" + dlm_data.FarmName + "(" + dlm_no + ") is Requested to you for perchase order of " + hdPaymentAmount + "Rs";
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



                                    var asd2 = db.SMSSendAlls.Where(s => s.ServiceName == "dlmToDLMdlmFundTrans1" && s.Whatsapp_Status == "Y").ToList();
                                    var smsapi2 = db.apisms.Where(x => x.sts == "Y").ToList();

                                    var smsapionsts2 = smsapi2.Where(s => s.api_type == "whatsapp").SingleOrDefault();
                                    if (smsapionsts2 != null)
                                    {
                                        if (asd2.Any())
                                        {
                                            apiurls = smsapionsts2.smsapi;
                                            string text = "Your purchase order Request of " + hdPaymentAmount + "Rs" + "has been sent";
                                            text = string.Format(text, "1230");

                                            var apinamechange = apiurls.Replace("tttt", dlm_no).Replace("mmmm", text);

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
                                    var asd3 = db.SMSSendAlls.Where(s => s.ServiceName == "dlmToDLMdlmFundTrans1" && s.Status == "Y").ToList();
                                    var smsapionsts3 = smsapi.Where(s => s.api_type == "sms").SingleOrDefault();
                                    if (smsapionsts3 != null)
                                    {
                                        if (asd3.Any())
                                        {
                                            apiurls = smsapionsts3.smsapi;
                                            string text = "Your purchase order Request of " + hdPaymentAmount + "Rs" + "has been sent";
                                            text = string.Format(text, "1230");

                                            var apinamechange = apiurls.Replace("tttt", dlm_no).Replace("mmmm", text);

                                            var client = new RestClient(apinamechange);
                                            var request = new RestRequest(Method.GET);

                                            VastBazaartoken Responsetoken = new VastBazaartoken();

                                            if (apinamechange.ToUpper().Contains("API.VASTBAZAAR.COM"))
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

                                var emailcheck = db.EmailSendAlls.Where(s => s.ServiceName == "DLMToDLMMDFundTrans1" && s.Status == "Y").ToList();
                                if (emailcheck.Any())
                                {
                                    var AdminDetails = db.Admin_details.SingleOrDefault();
                                    smssend.SendEmailAll(md_data.Email, dlm_name + "-" + dlm_data.FarmName + "(" + dlm_no + ") is Requested to you for perchase order of " + hdPaymentAmount + "Rs", "Purchase Order Request", AdminDetails.email);

                                   

                                }

                                var emailcheck23 = db.EmailSendAlls.Where(s => s.ServiceName == "dlmToDLMdlmFundTrans1" && s.Status == "Y").ToList();
                                if (emailcheck23.Any())
                                {
                                    var AdminDetails = db.Admin_details.SingleOrDefault();
                                    smssend.SendEmailAll(dlm_data.Email, "Your purchase order Request of " + hdPaymentAmount + "Rs" + "has been sent", "Purchase Order Request", AdminDetails.email);

                                   

                                }






                            }

                            ch = "Your purcharge Order Successfully.";
                            return Json(ch, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            ch = "Dealer Id Not Found";
                            return Json(ch, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {

                        ch = "Amount should be not zero";
                        return Json(ch, JsonRequestBehavior.AllowGet);
                        // return RedirectToAction("PurchaseOrder");
                    }
                    //}
                    //else
                    //{
                    //   // TempData["error"] = "Your purcharge Order Allready Pending.";
                    //    ch = "Your purcharge Order Allready Pending.";
                    //    return Json(ch, JsonRequestBehavior.AllowGet);

                    //}
                    //  FundRequestViewmodel vmodel = new FundRequestViewmodel();
                    //  string dealerid = User.Identity.GetUserId();
                    //  vmodel.SendPurchaserequest = db.select_dlm_pur_order(dealerid, "ALL", DateTime.Now.AddDays(-1), DateTime.Now.AddDays(1)).ToList();
                    //  return PartialView("_PurchaserequestDLMTOMDADMIN", vmodel);

                    //   return RedirectToAction("PurchaseOrder");
                    // return MDTODealer("Admin", null, null);


                }
                catch (Exception ex)
                {
                    TempData["error"] = ex;
                    ch = ex.Message;
                    return Json(ch, JsonRequestBehavior.AllowGet);
                    //  return RedirectToAction("PurchaseOrder");
                }
            }
            return Json("errorss", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Updates a dealer-to-retailer purchase order status.
        /// </summary>
        [HttpPost]
        public ActionResult updatepurchage_dlmTOret(int hdidno, string hdtype, string txtcommentwrite)
        {
            var userid = User.Identity.GetUserId();
            if (hdtype == "APP")
            {
                hdtype = "Approved";
            }
            else
            {
                hdtype = "rejected";
            }
            var purinfo = db.rem_purchage.Where(a => a.idno == hdidno).SingleOrDefault();
            var sts = purinfo.sts.ToUpper();
            if (sts == "PENDING")
            {
                System.Data.Entity.Core.Objects.ObjectParameter output = new
                System.Data.Entity.Core.Objects.ObjectParameter("Output", typeof(string));
                var ch = db.update_rem_purchage(Convert.ToInt32(hdidno), hdtype, 0, txtcommentwrite, output).Single().msg;
                try
                {
                    var retailerdetails = db.Retailer_Details.Where(aa => aa.RetailerId == purinfo.remid).SingleOrDefault();
                    var dealerdetails = db.Dealer_Details.Where(aa => aa.DealerId == userid).SingleOrDefault();
                   
                    var remdetails = db.Remain_reteller_balance.Where(aa => aa.RetellerId == purinfo.remid).SingleOrDefault();
                    var dlmdetails = db.Remain_dealer_balance.Where(aa => aa.DealerID == userid).SingleOrDefault();
                    
                    var admininfo = db.Admin_details.SingleOrDefault();
                    Backupinfo back = new Backupinfo();
                    var modeln = new Backupinfo.Addinfo
                    {
                        Websitename = admininfo.WebsiteUrl,
                        RetailerID = purinfo.remid,
                        Email = retailerdetails.Email,
                        Mobile = retailerdetails.Mobile,
                        Details = "Fund Recived From Dealer ",
                        RemainBalance = (decimal)remdetails.Remainamount,
                        Usertype = "Retailer"
                    };
                    back.Fundtransfer(modeln);

                    var model1 = new Backupinfo.Addinfo
                    {
                        Websitename = admininfo.WebsiteUrl,
                        RetailerID = userid,
                        Email = dealerdetails.Email,
                        Mobile = dealerdetails.Mobile,
                        Details = "Fund Transfer To Retailer ",
                        RemainBalance = Convert.ToDecimal(dlmdetails.Remainamount),
                        Usertype = "Dealer"
                    };
                    back.Fundtransfer(model1);

                  
                }
                catch { }
                if (ch == "Credit Pay Successfully." || ch == "Balance Transfer Successfully.")
                {
                    TempData["successorder"] = ch;
                }
                else
                {
                    TempData["failedorder"] = ch;
                }
            }
            return MDTODealer("RetailerToDLMREQ", null, null);
        }

        /// <summary>
        /// Filters received requests by date range.
        /// </summary>
        public ActionResult Filterrecivedrequestfilterdate(string txt_frm_daterecived, string txt_to_daterecived)
        {
            FundRequestViewmodel vmodel = new FundRequestViewmodel();
            string userid = User.Identity.GetUserId();
            var userids = db.Users.FirstOrDefault(a => a.UserId == userid).UserId;
            DateTime frm = Convert.ToDateTime(txt_frm_daterecived);
            DateTime to = Convert.ToDateTime(txt_to_daterecived);
            txt_frm_daterecived = frm.ToString("dd-MM-yyyy");
            txt_to_daterecived = to.ToString("dd-MM-yyyy");
            string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy",
                        "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
            DateTime dt = !string.IsNullOrWhiteSpace(txt_frm_daterecived) ? DateTime.ParseExact(txt_frm_daterecived, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
            DateTime dt1 = !string.IsNullOrWhiteSpace(txt_to_daterecived) ? DateTime.ParseExact(txt_to_daterecived, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
            DateTime frm_date = Convert.ToDateTime(dt).Date;
            DateTime to_date = Convert.ToDateTime(dt1).Date.AddDays(1);
            vmodel.PurchaseRequestRecived = db.select_rem_pur_order("ALL", userids, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            return PartialView("_PurchaserequestRecived", vmodel);
        }


        /// <summary>
        /// Transfers balance from dealer to retailer account.
        /// </summary>
        [HttpPost]
        public ActionResult Dealer_retailer_bal(string RetailerId, string balance, string ddl_fund_type, string comment)
        {
            try
            {

                string userid = User.Identity.GetUserId();
                var email = db.Users.Where(p => p.UserId == userid).Single().Email;
                var useremail = db.Users.Where(p => p.UserId == RetailerId).Single().Email;
                System.Data.Entity.Core.Objects.ObjectParameter output = new System.Data.Entity.Core.Objects.ObjectParameter("output", typeof(String));
                var diff1 = (db.Dealer_To_Retailer_Balance.Where(aa => aa.RetailerId == RetailerId && aa.DealerId == userid).OrderByDescending(aa => aa.id).Select(aa => aa.cr).FirstOrDefault());
                diff1 = diff1 ?? 0;
                var ch = "";
                //var tp = "";
                decimal amount = Convert.ToDecimal(balance);
                if (amount > 0)
                {
                    if (ddl_fund_type == "Cash" || ddl_fund_type == "Credit")
                    {
                        ch = "";// db.insert_dealer_to_retailer_balance(userid, RetailerId, Convert.ToDecimal(balance), 0, ddl_fund_type, comment,"","","","","", output).Single().msg;
                    }
                    var AdminEmail = db.Admin_details.Single().email;
                    if (ch == "Balance Transfer Successfully.")
                    {
                        diff1 = (db.Dealer_To_Retailer_Balance.Where(aa => aa.RetailerId == RetailerId && aa.DealerId == userid).OrderByDescending(aa => aa.id).Select(aa => aa.cr).FirstOrDefault());
                        diff1 = diff1 ?? 0;
                        var remainretailer = db.Remain_reteller_balance.Where(p => p.RetellerId == RetailerId).Single().Remainamount;
                        var statusDealer = db.PushNotificationStatus.Where(a => a.UserRole == "Distributor").SingleOrDefault().Status;
                        var statusRetailer = db.PushNotificationStatus.Where(a => a.UserRole == "Retailer").SingleOrDefault().Status;

                        var statusSendSmsDlmToRetailerFundTransfer = db.SMSSendAlls.Where(a => a.ServiceName == "DLMtoRemRemFundTrans").SingleOrDefault();
                        var statusSendMailDlmToRetailerFundTransfer = db.EmailSendAlls.Where(a => a.ServiceName == "DLMtoRemRemFundTrans1").SingleOrDefault().Status;
                        var RetailerDetails = db.Retailer_Details.Where(p => p.RetailerId == RetailerId).Single();

                        var statusSendSmsDlmToRetailerFundTransferDlm = db.SMSSendAlls.Where(a => a.ServiceName == "DLMtoRemdlmFundTrans").SingleOrDefault();
                        var statusSendMailDlmToRetailerFundTransferDlm = db.EmailSendAlls.Where(a => a.ServiceName == "DLMtoRemdlmFundTrans1").SingleOrDefault().Status;
                        var DealerDetails = db.Dealer_Details.Where(p => p.DealerId == userid).Single();

                        if (ddl_fund_type == "Credit")
                        {
                            //if (statusSendSmsDlmToRetailerFundTransfer == "Y")
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

                            //            msgssss = string.Format(smsstypes.Templates, email, balance, remainretailer, diff1);
                            //            tempid = smsstypes.Templateid;
                            //            urlss = smsapionsts.smsapi;

                            //            smssend.sendsmsallnew(RetailerDetails.Mobile, msgssss, urlss, tempid);
                            //        }
                            //    }
                            //    catch { }
                            //    //  smssend.sendsmsall(RetailerDetails.Mobile, "Credit Received by " + email + " Rs." + balance + ".New Balance is " + remainretailer + ".Your O/s Credit is " + diff1 + "", "Fund Transfer");
                            //}

                            smssend.sms_init(statusSendSmsDlmToRetailerFundTransfer.Status, statusSendSmsDlmToRetailerFundTransfer.Whatsapp_Status, "SEND_SMS_CREDIT_RECEIVEDBY", RetailerDetails.Mobile, email, balance, remainretailer, diff1);

                            //if (statusSendSmsDlmToRetailerFundTransferDlm == "Y")
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

                            //            msgssss = string.Format(smsstypes.Templates, useremail, balance, diff1);
                            //            tempid = smsstypes.Templateid;
                            //            urlss = smsapionsts.smsapi;

                            //            smssend.sendsmsallnew(DealerDetails.Mobile, msgssss, urlss, tempid);
                            //        }
                            //    }
                            //    catch { }


                            //    // smssend.sendsmsall(DealerDetails.Mobile, "Credit Transferred To " + useremail + " Rs. " + balance + ".Total Credit(O/s) Balance is " + diff1 + "", "Fund Transfer");
                            //}

                            smssend.sms_init(statusSendSmsDlmToRetailerFundTransferDlm.Status, statusSendSmsDlmToRetailerFundTransferDlm.Whatsapp_Status, "SEND_SMS_CREDIT_TRANSFERREDTO", DealerDetails.Mobile, useremail, balance, diff1);

                            if (statusSendMailDlmToRetailerFundTransfer == "Y")
                            {
                                smssend.SendEmailAll(RetailerDetails.Email, "Credit Received by " + email + " Rs." + balance + ".New Balance is " + remainretailer + ".Your O/s Credit is " + diff1 + "", "Fund Transfer", AdminEmail);
                            }
                            if (statusSendMailDlmToRetailerFundTransferDlm == "Y")
                            {
                                smssend.SendEmailAll(DealerDetails.Email, "Credit Transferred To " + useremail + " Rs. " + balance + ".Total Credit(O/s) Balance is " + diff1 + "", "Fund Transfer", AdminEmail);
                            }
                            //if (statusDealer == "Y")
                            //{
                            //    SendPushNotification(email, Url.Action("SendFund", "Home"), "Credit Transferred Rs. " + balance + ".Total Credit(O/s) Balance is " + diff1 + "", "Fund Transfer ..");
                            //}
                            //if (statusRetailer == "Y")
                            //{
                            //    SendPushNotification(useremail, Websitename + "/RETAILER/Home/FundRecive", "Credit Received Rs." + balance + ".New Balance is " + remainretailer + ".Your O/s Credit is " + diff1 + "", "Fund Transfer ..");
                            //}
                            notify.sendmessage(useremail, "Credit Received Rs." + balance + ".New Balance is " + remainretailer + ".Your O/s Credit is " + diff1 + "");
                            TempData["result"] = ch;
                        }
                        else
                        {
                            var retailername = db.Retailer_Details.Where(p => p.RetailerId == RetailerId).Single().RetailerName;
                            var dealername = db.Dealer_Details.Where(p => p.DealerId == userid).Single().DealerName;
                            //if (statusSendSmsDlmToRetailerFundTransfer == "Y")
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

                            //            msgssss = string.Format(smsstypes.Templates, balance, dealername, remainretailer, diff1);
                            //            tempid = smsstypes.Templateid;
                            //            urlss = smsapionsts.smsapi;

                            //            smssend.sendsmsallnew(RetailerDetails.Mobile, msgssss, urlss, tempid);
                            //        }
                            //    }
                            //    catch { }


                            //    // smssend.sendsmsall(RetailerDetails.Mobile, "Cash Paid Rs." + balance + " to " + dealername + ". New Balance is " + remainretailer + ". Your O/s Credit is " + diff1 + "", "Fund Transfer");
                            //}

                            smssend.sms_init(statusSendSmsDlmToRetailerFundTransfer.Status, statusSendSmsDlmToRetailerFundTransfer.Whatsapp_Status, "FUNDTRANSFERMESSAGE_CASH", RetailerDetails.Mobile, balance, dealername, remainretailer, diff1);

                            //if (statusSendSmsDlmToRetailerFundTransferDlm == "Y")
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

                            //            msgssss = string.Format(smsstypes.Templates, balance, retailername, diff1);
                            //            tempid = smsstypes.Templateid;
                            //            urlss = smsapionsts.smsapi;

                            //            smssend.sendsmsallnew(DealerDetails.Mobile, msgssss, urlss, tempid);
                            //        }
                            //    }
                            //    catch { }
                            //    // smssend.sendsmsall(DealerDetails.Mobile, "Cash Recived Rs." + balance + " From " + retailername + ",his O/s Credit is " + diff1 + "", "Fund Transfer");
                            //}

                            smssend.sms_init(statusSendSmsDlmToRetailerFundTransferDlm.Status, statusSendSmsDlmToRetailerFundTransferDlm.Whatsapp_Status, "SENDSMSMASTERTODLMFUNDTRANSFERMSGCASH", DealerDetails.Mobile, balance, retailername, diff1);

                            if (statusSendMailDlmToRetailerFundTransfer == "Y")
                            {
                                smssend.SendEmailAll(RetailerDetails.Email, "Cash Paid Rs." + balance + " to " + dealername + ". New Balance is " + remainretailer + ". Your O/s Credit is " + diff1 + "", "Fund Transfer", AdminEmail);
                            }
                            if (statusSendMailDlmToRetailerFundTransferDlm == "Y")
                            {
                                smssend.SendEmailAll(DealerDetails.Email, "Cash Recived Rs." + balance + " From " + retailername + ",his O/s Credit is " + diff1 + "", "Fund Transfer", AdminEmail);
                            }
                            //if (statusDealer == "Y")
                            //{
                            //    SendPushNotification(email, Url.Action("SendFund", "Home"), "Cash Recived Rs." + balance + " From " + retailername + ",his O/s Credit is " + diff1 + "", "Fund Transfer ..");
                            //}
                            //if (statusRetailer == "Y")
                            //{
                            //    SendPushNotification(useremail, Websitename + "/RETAILER/Home/FundRecive", "Cash Paid Rs." + balance + " to " + dealername + ". New Balance is " + remainretailer + ". Your O/s Credit is " + diff1 + "", "Fund Transfer ..");
                            //}
                            notify.sendmessage(useremail, "Cash Paid Rs." + balance + " to " + dealername + ". New Balance is " + remainretailer + ". Your O/s Credit is " + diff1 + "");
                            TempData["result"] = ch;
                        }
                    }
                }
                else
                {
                    TempData["result"] = "Amount should be not zero";
                }
                return RedirectToAction("SendFund");
            }
            catch
            {
                TempData["result"] = "Internal Error, Please Check Our Wallet Balance.";
                return RedirectToAction("SendFund");
            }
        }

        //Credit Balance Check
        /// <summary>
        /// Returns the current credit balance for a specific retailer under this dealer.
        /// </summary>
        public ActionResult R_Creditchk(string retailerid)
        {
            string userid = User.Identity.GetUserId();

            var ch = db.Dealer_To_Retailer_Balance.Where(aa => aa.RetailerId == retailerid && aa.DealerId == userid).OrderByDescending(aa => aa.id).Select(c => c.cr).FirstOrDefault();
            ch = ch ?? 0;
            decimal? rembal;
            if (string.IsNullOrEmpty(retailerid))
            {
                rembal = 0;
            }
            else
            {
                rembal = db.Remain_reteller_balance.Where(x => x.RetellerId == retailerid).SingleOrDefault().Remainamount;
            }
            return Json(new { currntcr = ch, rembal = rembal }, JsonRequestBehavior.AllowGet);


            //  return Json(ch, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Applies cash deposit charges for a retailer.
        /// </summary>
        public ActionResult cashdepositecharge(string retailerid, string paytype, decimal? AMount, decimal currentcr)
        {
            string userid = User.Identity.GetUserId();
            decimal amounts = AMount.Value;
            decimal chargess = 0;
            decimal netamount = 0;
            var cashdeposchargess = db.creditchargeretailers.Where(x => x.chargefrom == userid && x.userid == retailerid && x.type == paytype).FirstOrDefault();
            if (cashdeposchargess != null)
            {
                chargess = amounts * cashdeposchargess.charge.Value / 100;
                chargess = Math.Round(chargess, 2);
                netamount = amounts - chargess - currentcr;
            }
            if (netamount < 0)
            {
                netamount = 0;
            }
            return Json(new { charge = chargess, netamount = netamount, currentcr = currentcr }, JsonRequestBehavior.AllowGet);
        }

        //fund recive from admin
        /// <summary>
        /// Shows the fund received from the admin report for the current day.
        /// </summary>
        [HttpGet]
        public ActionResult ReceiveFund_by_admin()
        {
            var userid = User.Identity.GetUserId();
            string txt_frm_date = DateTime.Now.ToString();
            string txt_to_date = DateTime.Now.ToString();
            string frm_date = Convert.ToDateTime(txt_frm_date).Date.ToString("yyyy-MM-dd");
            string to_date = Convert.ToDateTime(txt_to_date).AddDays(1).ToString("yyyy-MM-dd");
            var show = db.dealer_total_fund_recive_by_admin(userid, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            return View(show);
        }
        /// <summary>
        /// Displays funds received from admin with date filter.
        /// </summary>
        [HttpPost]
        public ActionResult ReceiveFund_by_admin(string txt_frm_date, string txt_to_date)
        {
            var userid = User.Identity.GetUserId();
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
            var show = db.dealer_total_fund_recive_by_admin(userid, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            return View(show);
        }
        // fund recive from master
        /// <summary>
        /// Displays funds received from master dealer with date filter.
        /// </summary>
        [HttpGet]
        public ActionResult ReceiveFund_by_master()
        {
            var userid = User.Identity.GetUserId();
            string txt_frm_date = DateTime.Now.ToString();
            string txt_to_date = DateTime.Now.ToString();
            string frm_date = Convert.ToDateTime(txt_frm_date).Date.ToString("yyyy-MM-dd");
            string to_date = Convert.ToDateTime(txt_to_date).AddDays(1).ToString("yyyy-MM-dd");
            var show = db.dealer_total_fund_recive_by_master(userid, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            return View(show);
        }
        /// <summary>
        /// Displays funds received from master dealer with date filter.
        /// </summary>
        [HttpPost]
        public ActionResult ReceiveFund_by_master(string txt_frm_date, string txt_to_date)
        {
            var userid = User.Identity.GetUserId();
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
            var show = db.dealer_total_fund_recive_by_master(userid, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            return View(show);
        }
        /// <summary>
        /// Resends registration or welcome email to a retailer.
        /// </summary>
        public ActionResult ResendRetailerEmail(string email)
        {
            var data = db.ResendConfirmMails.Where(aa => aa.Email == email).SingleOrDefault();
            if (data != null)
            {
                var callbackurl = data.CallBackUrl;
                var pass = data.Password;
                var pin = data.Pin;
                string body = new CommonUtil().PopulateBody("", "Confirm your account", "", "" + callbackurl + "", email, pass, pin, "");
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
        //Fund Received Pdf
        /// <summary>
        /// Redirects to the invoice PDF generation page.
        /// </summary>
        public ActionResult GotoPDF(string From, string Value, string DistOldBal, string DistNewBal, string Date)
        {
            string userid = User.Identity.GetUserId();
            return new Rotativa.ActionAsPdf("InvoicePDF", new { dlmloginid = userid, From = From, Value = Value, DistOldBal = DistOldBal, DistNewBal = DistNewBal, Date = Date });
        }
        /// <summary>
        /// Generates and returns the fund transfer invoice as PDF.
        /// </summary>
        public ActionResult InvoicePDF(string dlmloginid, string From, string Value, string DistOldBal, string DistNewBal, string Date)
        {
            var userdetaild = db.Dealer_Details.Where(a => a.DealerId == dlmloginid).SingleOrDefault();
            var PDF_Content = new DealerInvoiceModel()
            {
                From = From,
                Value = Value,
                DistOldBal = DistOldBal,
                DistNewBal = DistNewBal,
                Date = Date
            };
            TempData["retailername"] = userdetaild.DealerId.ToUpper();
            TempData["firmname"] = userdetaild.FarmName.ToUpper();
            TempData["retailermobile"] = userdetaild.Mobile.ToUpper();

            TempData["retailerdate"] = PDF_Content.Date;
            TempData["date"] = DateTime.Now;
            return View(PDF_Content);
            //return View();
        }


        //[HttpPost]
        //public ActionResult ReceiveFund_GST(int idno)
        //{
        //    try
        //    {
        //        using (VastwebmultiEntities db = new VastwebmultiEntities())
        //        {

        //            //TODO

        //        }
        //        return new ViewAsPdf("GST_PDF");
        //    }
        //    catch (Exception ex)
        //    {
        //        return RedirectToAction("Index");
        //    }


        //}
        /// <summary>
        /// Displays the dealer purchase order list page with pending and completed orders.
        /// </summary>
        public ActionResult PurchaseOrder()
        {
            try
            {
                IEnumerable<select_dlm_pur_order_Result> model = null;

                ViewData["error"] = TempData["error"];
                ViewData["success"] = TempData["success"];
                TempData.Remove("error");
                TempData.Remove("success");
                //show old credit pay value
                string userid = User.Identity.GetUserId();
                string txt_frm_date = DateTime.Now.ToString();
                string txt_to_date = DateTime.Now.ToString();
                string frm_date = Convert.ToDateTime(txt_frm_date).ToShortDateString();
                string to_date = Convert.ToDateTime(txt_to_date).AddDays(1).ToString();

                //account no
                var accunt = (from acc in db.bank_info select new { acno = acc.acno, acnooo = acc.acno + "||" + acc.holdername });
                ViewBag.account = new SelectList(accunt, "acno", "acnooo", null);
                //Dealer Credit to master
                var mastercreditbal = db.total_dealer_outstanding_by_master(userid).FirstOrDefault().totaloutstanding;
                //Dealer Credit to admin
                var admincreditbal = db.total_dealer_outstanding_by_admin(userid).FirstOrDefault().totaloutstanding;
                ViewData["oldcredit"] = (mastercreditbal + admincreditbal);

                model = db.select_dlm_pur_order(userid, "ALL", Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
                var entries = db.PurchaseOrderCashDepositCharges.ToList();
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
        /// <summary>
        /// Displays and filters the purchase order list for the dealer.
        /// </summary>
        [HttpPost]
        public ActionResult PurchaseOrder(string txt_frm_date, string txt_to_date)
        {
            try
            {
                IEnumerable<select_dlm_pur_order_Result> model = null;
                ViewData["show"] = TempData["show"];
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

                var accunt = (from acc in db.bank_info select new { acno = acc.acno, acnooo = acc.acno + "||" + acc.holdername });
                ViewBag.account = new SelectList(accunt, "acno", "acnooo", null);
                //Dealer Credit to master
                var mastercreditbal = db.total_dealer_outstanding_by_master(userid).FirstOrDefault().totaloutstanding;
                //Dealer Credit to admin
                var admincreditbal = db.total_dealer_outstanding_by_admin(userid).FirstOrDefault().totaloutstanding;
                ViewData["oldcredit"] = (mastercreditbal + admincreditbal);
                model = db.select_dlm_pur_order(userid, "ALL", Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
                var entries = db.PurchaseOrderCashDepositCharges.ToList();
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
        /// <summary>
        /// Processes and submits a new purchase order.
        /// </summary>
        [HttpPost]
        public ActionResult purchageorder(string payto, string Paymode, string type, string utrno, string Comment, decimal balance, string accountno, string txttoaccountno, string pancard, string branch, decimal? dipositCharge, string partyAcc, string AccHolderName)
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

                    var count = (db.dlm_purchage.Where(aa => aa.sts == "Pending" && aa.dlmid == userid).Count());
                    int countchk = Convert.ToInt32(count);
                    var acc = "";
                    if (countchk < 1)
                    {
                        var amount = Convert.ToDecimal(balance);

                        if (amount > 0)
                        {
                            var from = "";
                            if (payto == "Master")
                            {
                                from = (db.Dealer_Details.Where(aa => aa.DealerId == userid).Single().SSId);
                                acc = txttoaccountno;
                            }
                            else
                            {
                                from = db.Admin_details.Single().userid;
                                acc = accountno;
                            }
                            var diff1 = (db.admin_to_dealer.Where(aa => aa.dealerid == userid && aa.balance_from == from).OrderByDescending(aa => aa.idno).Select(c => c.cr).FirstOrDefault());
                            diff1 = diff1 ?? 0;
                            decimal diff = Convert.ToDecimal(diff1);

                            if (type == "Credit")
                            {
                                Paymode = "Credit";
                                db.insert_dlmpurchageorder(userid, Paymode, utrno, "", "", Comment, Convert.ToDecimal(balance), payto, type, "", acc, imagePath, "", "", "", 0, amount);
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
                                db.insert_dlmpurchageorder(userid, Paymode, utrno, "", "", Comment, Convert.ToDecimal(balance), payto, type, "", acc, imagePath, pancard, branch, AccHolderName, disCharge, amount - disCharge);
                                TempData["success"] = "Your purcharge Order Successfully.";


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

        //Check Old Credit pay
        //public ActionResult D_Creditchk(string MID)
        //{
        //    string userid = User.Identity.GetUserId();
        //    var from = "";
        //    if (MID == "Master")
        //    {
        //        from = (db.Dealer_Details.Where(aa => aa.DealerId == userid).Single().SSId);
        //    }
        //    else
        //    {
        //        from = db.Admin_details.Single().userid;
        //    }

        //    var diff1 = (db.admin_to_dealer.Where(aa => aa.dealerid == userid && aa.balance_from == from).OrderByDescending(aa => aa.idno).Select(c => c.cr).FirstOrDefault());
        //    diff1 = diff1 ?? 0;
        //    return Json(diff1, JsonRequestBehavior.AllowGet);

        //}
        /// <summary>
        /// Checks the dealer credit status.
        /// </summary>
        public ActionResult D_Creditchk(string MID)
        {
            string userid = User.Identity.GetUserId();
            var from = "";
            List<SelectListItem> bankitem = new List<SelectListItem>();
            List<SelectListItem> walletitem = new List<SelectListItem>();
            dynamic diff1;
            if (MID == "Master")
            {
                from = (db.Dealer_Details.Where(aa => aa.DealerId == userid).Single().SSId);
                var bindbank = db.bank_info.Where(x => x.userid == from).ToList();
                //  List<SelectListItem> bankitem = new List<SelectListItem>();
                foreach (var bank in bindbank)
                {
                    bankitem.Add(new SelectListItem { Text = bank.banknm, Value = bank.acno });
                }
                var bindwallet = db.tblwallet_info.Where(x => x.userid == from).ToList();

                foreach (var wallet in bindwallet)
                {
                    walletitem.Add(new SelectListItem { Text = wallet.walletname, Value = wallet.walletno });
                }
                diff1 = (db.admin_to_dealer.Where(aa => aa.dealerid == userid && aa.balance_from == from).OrderByDescending(aa => aa.idno).Select(c => c.cr).FirstOrDefault());
                diff1 = diff1 ?? 0;
                // return Json(diff1, JsonRequestBehavior.AllowGet);
                // return Json(new { diff = diff1, listbank = bankitem, walletinfo = walletitem }, JsonRequestBehavior.AllowGet);


            }
            else
            {
                from = db.Admin_details.Single().userid;

                var bindbank = db.bank_info.Where(x => x.userid == "Admin").ToList();

                foreach (var bank in bindbank)
                {
                    bankitem.Add(new SelectListItem { Text = bank.banknm, Value = bank.acno });
                }
                var bindwallet = db.tblwallet_info.Where(x => x.userid == "Admin").ToList();
                // List<SelectListItem> walletitem = new List<SelectListItem>();
                foreach (var wallet in bindwallet)
                {
                    walletitem.Add(new SelectListItem { Text = wallet.walletname, Value = wallet.walletno });
                }

                diff1 = (db.admin_to_dealer.Where(aa => aa.dealerid == userid && aa.balance_from == from).OrderByDescending(aa => aa.idno).Select(c => c.cr).FirstOrDefault());
                diff1 = diff1 ?? 0;
                // return Json(diff1, JsonRequestBehavior.AllowGet);


            }
            return Json(new { diff = diff1, listbank = bankitem, walletinfo = walletitem }, JsonRequestBehavior.AllowGet);




        }

        /// <summary>
        /// Displays and processes purchase requests.
        /// </summary>
        public ActionResult purcharge_request()
        {
            ViewData["successorder"] = TempData["successorder"];
            ViewData["failedorder"] = TempData["failedorder"];
            string userid = User.Identity.GetUserId();
            var Email = db.Users.FirstOrDefault(a => a.UserId == userid).Email;
            string txt_frm_date = DateTime.Now.Date.ToShortDateString();
            string txt_to_date = DateTime.Now.ToString();
            string frm_date = Convert.ToDateTime(txt_frm_date).ToShortDateString();
            string to_date = Convert.ToDateTime(txt_to_date).AddDays(1).ToString();
            var ch = db.select_rem_pur_order("ALL", userid, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            return View(ch);
        }
        /// <summary>
        /// Displays and processes purchase requests.
        /// </summary>
        [HttpPost]
        public ActionResult purcharge_request(string txt_frm_date, string txt_to_date)
        {
            string userid = User.Identity.GetUserId();
            var userids = db.Users.FirstOrDefault(a => a.UserId == userid).UserId;
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
            var ch = db.select_rem_pur_order("ALL", userids, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            return View(ch);
        }



        /// <summary>
        /// Updates a dealer purchase order record.
        /// </summary>
        [HttpPost]
        public ActionResult updatepurchage_dlm(int id, string type, string txtcomment, string HD_frm_date, string HD_to_date)
        {
            var userid = User.Identity.GetUserId();
            if (type == "APP")
            {
                type = "Approved";
            }
            else
            {
                type = "rejected";
            }
            var purinfo = db.rem_purchage.Where(a => a.idno == id).SingleOrDefault();
            var sts= purinfo.sts.ToUpper();
            if (sts == "PENDING")
            {
                System.Data.Entity.Core.Objects.ObjectParameter output = new System.Data.Entity.Core.Objects.ObjectParameter("Output", typeof(string));
                var ch = db.update_rem_purchage(Convert.ToInt32(id), type, 0, txtcomment, output).Single().msg;
                try
                {
                    var retailerdetails = db.Retailer_Details.Where(aa => aa.RetailerId == purinfo.remid).SingleOrDefault();
                    var dealerdetails = db.Dealer_Details.Where(aa => aa.DealerId == userid).SingleOrDefault();
                  
                    var remdetails = db.Remain_reteller_balance.Where(aa => aa.RetellerId == purinfo.remid).SingleOrDefault();
                    var dlmdetails = db.Remain_dealer_balance.Where(aa => aa.DealerID == userid).SingleOrDefault();
                   
                    var admininfo = db.Admin_details.SingleOrDefault();
                    Backupinfo back = new Backupinfo();
                    var modeln = new Backupinfo.Addinfo
                    {
                        Websitename = admininfo.WebsiteUrl,
                        RetailerID = userid,
                        Email = retailerdetails.Email,
                        Mobile = retailerdetails.Mobile,
                        Details = "Fund Recived From Dealer ",
                        RemainBalance = (decimal)remdetails.Remainamount,
                        Usertype = "Retailer"
                    };
                    back.Fundtransfer(modeln);

                    var model1 = new Backupinfo.Addinfo
                    {
                        Websitename = admininfo.WebsiteUrl,
                        RetailerID = dealerdetails.DealerId,
                        Email = dealerdetails.Email,
                        Mobile = dealerdetails.Mobile,
                        Details = "Fund Transfer To Retailer ",

                        RemainBalance = Convert.ToDecimal(dlmdetails.Remainamount),
                        Usertype = "Dealer"
                    };
                    back.Fundtransfer(model1);
                }
                catch { }
                if (ch == "Credit Pay Successfully." || ch == "Balance Transfer Successfully.")
                {
                    TempData["successorder"] = ch;
                }
                else
                {
                    TempData["failedorder"] = ch;
                }
            }
     
            var userids = db.Users.FirstOrDefault(a => a.UserId == userid).UserId;
            string txt_frm_date = HD_frm_date;
            string txt_to_date = HD_to_date;
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
            var model = db.select_rem_pur_order("ALL", userids, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            return PartialView("_Purchase_RequestDealer", model);
            //  return RedirectToAction("purcharge_request");
        }

        /// <summary>
        /// Filters dealer purchase requests by search criteria.
        /// </summary>
        public ActionResult SearchFilterDealerPurchaseRequest(string txt_frm_date, string txt_to_date)
        {
            string userid = User.Identity.GetUserId();
            var userids = db.Users.FirstOrDefault(a => a.UserId == userid).UserId;
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
            var model = db.select_rem_pur_order("ALL", userids, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            return PartialView("_Purchase_RequestDealer", model);
        }

        #endregion
        //fill District 
        /// <summary>
        /// Returns the district list for a given state.
        /// </summary>
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
        /// <summary>
        /// Default index action for the dealer area home.
        /// </summary>
        public ActionResult Index()
        {
            try
            {
                string userid = User.Identity.GetUserId();
                //var hh = db.Slab_name.Where(a => a.createdby.ToLower() == userid.ToLower()).ToList();
                //ViewBag.slabname = new SelectList(hh, "SlabName", "SlabName");
                //ViewBag.slabedit = new SelectList(hh, "SlabName", "SlabName");
                ViewBag.state = new SelectList(db.Select_State_Details(), "State_Id", "State_Name").ToList();
                ViewData["msg"] = TempData["msgrem"];
                int count = (from gg in db.Message_top where (gg.users == "Master" || gg.users == "All") where gg.status == "Y" select gg).Count();
                ViewData["text"] = count;
                var Dealertoken = (from gg in db.RetailerCreationTokens where gg.DealerId == userid select gg.Tokens).SingleOrDefault();
                var Details = db.Dealer_retailer(userid, "Distibutor", 1, 3000);
                RetailerModel viewModel = new RetailerModel();
                viewModel.dealer_retailer = Details;
                viewModel.Dealertoken = Dealertoken;
                viewModel.messagetop = (from gg in db.Message_top where (gg.users == "Master" || gg.users == "All") where gg.status == "Y" select gg).ToList();
                ViewBag.state1 = new SelectList(db.Select_State_Details(), "State_Id", "State_Name");
                ViewData["ResendMail"] = TempData["ResendMAil"];
                return View(viewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }


        //Complaint Request 
        /// <summary>
        /// Displays the complaint submission form.
        /// </summary>
        public ActionResult Complaint()
        {
            var userid = User.Identity.GetUserId();
            var ch = db.proc_Complaint_request(userid, "").ToList();
            return View(ch);
        }

        /// <summary>
        /// Submits a new complaint from the dealer.
        /// </summary>
        public ActionResult Complaint_insert(string message)
        {
            var statusAdmin = db.PushNotificationStatus.Where(a => a.UserRole == "Admin").SingleOrDefault().Status;
            var Emailid = db.Admin_details.Single().email;
            string userid = User.Identity.GetUserId();
            var retaileremaillid = db.Users.Where(p => p.UserId == userid).Single().Email;
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
            db.complaint_request.Add(objCourse);
            db.SaveChanges();
            if (statusAdmin == "Y")
            {
                SendPushNotification(Emailid, Url.Action("Money_Transfer_Report", "Home"), "User " + retaileremaillid + " is Send the Complaint For You .And Compalint is that " + message + "", "Complaint Insert..");
            }
            return RedirectToAction("Complaint");
        }
        /// <summary>
        /// Registers a new retailer under the dealer.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> InsertRetailer(RetailerModel model)
        {
            var appDbContext = HttpContext.GetOwinContext().Get<ApplicationDbContext>();
            using (var transaction = appDbContext.Database.BeginTransaction())
            {
                try
                {
                    using (VastwebmultiEntities db = new VastwebmultiEntities())
                    {
                        var RetailerId = string.Empty;
                        try
                        {
                            var emailverify = false;
                            var admininfo = db.Admin_details.SingleOrDefault();
                            if (string.IsNullOrWhiteSpace(model.Email))
                            {
                                model.Email = model.Mobile + "@" + admininfo.WebsiteUrl;
                                emailverify = true;
                            }
                            var frmname = db.Retailer_Details.Where(x => x.Frm_Name.ToUpper() == model.Firm.ToUpper()).Any();

                            if (frmname == true)
                            {
                                var fname = model.Firm.First();
                                var lchar = model.Firm.Last();
                                model.Firm = model.Firm + "_" + fname + lchar;
                            }
                            ViewBag.state = new SelectList(db.Select_State_Details(), "State_Id", "State_Name");
                            var chkmobile = db.Users.Where(a => a.PhoneNumber == model.Mobile).Any();
                            if (chkmobile == true)
                            {
                                TempData["mobileno"] = "This Mobile Number Already Exists.";
                                return RedirectToAction("Index");
                            }
                            if (model.State != 0 && model.District != 0)
                            {
                                RetailerId = User.Identity.GetUserId();

                                var dealertoken = db.RetailerCreationTokens.Where(a => a.DealerId == RetailerId).Single().Tokens;
                                if (dealertoken <= 0)
                                {
                                    TempData["Error"] = "Insufficient retailer creation token.";
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

                                    var enpin = Encrypt(pingen.ToString());
                                    model.Pin = pingen;

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

                                    model.Password = pass;

                                    var user = new ApplicationUser { UserName = model.Email, Email = model.Email, PhoneNumber = model.Mobile.ToString(), EmailConfirmed = emailverify };
                                    //Generate Random Password



                                    var result = await UserManager.CreateAsync(user, model.Password);
                                    if (result.Succeeded)
                                    {

                                        var DealerEmailId = db.Dealer_Details.Where(p => p.DealerId == RetailerId).Single().Email;
                                        System.Data.Entity.Core.Objects.ObjectParameter output = new
                                     System.Data.Entity.Core.Objects.ObjectParameter("output", typeof(string));

                                        string Firmname = model.Firm;
                                        var frmname1 = db.Retailer_Details.Where(x => x.Frm_Name.ToUpper() == Firmname.ToUpper()).Any();


                                        if (frmname1 == true)
                                        {
                                            var frmnamecontain = db.Retailer_Details.Where(x => x.Frm_Name.ToUpper() == Firmname.ToUpper()).FirstOrDefault().Frm_Name;
                                            var fname = Firmname.First();
                                            var lchar = Firmname.Last();
                                            int i = 0;
                                            while (Firmname.ToUpper() == frmnamecontain.ToUpper())
                                            {
                                                Firmname = Firmname + "_" + fname + lchar + "_" + i;
                                                i++;
                                            }

                                            // Firmname = Firmname + "_" + fname + lchar;
                                        }


                                        var ch = db.Insert_Retailer(RetailerId, user.Id, model.Name, model.State, model.District, model.Mobile, model.Address, Convert.ToInt32(model.Pincode), model.Email, "", "", Firmname, string.IsNullOrWhiteSpace(model.Adhaar) ? "" : model.Adhaar, string.IsNullOrWhiteSpace(model.Pan) ? "" : model.Pan, Convert.ToInt32(model.Capping), string.IsNullOrWhiteSpace(model.Gst) ? "" : model.Gst, enpin, "", RetailerId, output).Single().msg;
                                        if (ch == "Register SuccessFully.")
                                        {
                                            if (transaction.UnderlyingTransaction.Connection != null)
                                            {
                                                transaction.Commit();
                                                try
                                                {
                                                    var dealerinfo = db.Dealer_Details.Where(aa => aa.DealerId == user.Id).SingleOrDefault();
                                                    var masterinfo = db.Superstokist_details.Where(aa => aa.SSId == dealerinfo.SSId).SingleOrDefault();

                                                    var stateinfo_rem = db.State_Desc.Where(aa => aa.State_id == model.State).SingleOrDefault();
                                                    var district_rem = db.District_Desc.Where(aa => aa.State_id == model.State && aa.Dist_id == model.District).SingleOrDefault();

                                                    var stateinfo_dlm = db.State_Desc.Where(aa => aa.State_id == dealerinfo.State).SingleOrDefault();
                                                    var district_dlm = db.District_Desc.Where(aa => aa.State_id == dealerinfo.State && aa.Dist_id == dealerinfo.District).SingleOrDefault();

                                                    var stateinfo_master = db.State_Desc.Where(aa => aa.State_id == masterinfo.State).SingleOrDefault();
                                                    var district_master = db.District_Desc.Where(aa => aa.State_id == masterinfo.State && aa.Dist_id == masterinfo.District).SingleOrDefault();

                                                    var req = new pininsert
                                                    {
                                                        Mobile = model.Mobile,
                                                        RetailerName = model.Name,
                                                        Pincode = model.Pincode,
                                                        Email = model.Email,
                                                        Address = model.Address,
                                                        State_name = stateinfo_rem.State_name,
                                                        Dist_Desc = district_rem.Dist_Desc,
                                                        Frm_Name = Firmname,
                                                        DealerName = dealerinfo.DealerName,
                                                        dlmpin = dealerinfo.Pincode.ToString(),
                                                        dlmfirm = dealerinfo.FarmName,
                                                        dlmmobile = dealerinfo.Mobile,
                                                        dlmstate = stateinfo_dlm.State_name,
                                                        dealerdistrict = district_dlm.Dist_Desc,
                                                        dlmemail = dealerinfo.Email,
                                                        mstername = masterinfo.SuperstokistName,
                                                        msterfirmname = masterinfo.FarmName,
                                                        mstermobile = masterinfo.Mobile,
                                                        msteremail = masterinfo.Email,
                                                        mstrpin = masterinfo.Pincode.ToString(),
                                                        msterstatename = stateinfo_master.State_name,
                                                        msterdistictname = district_master.Dist_Desc,
                                                        isseleep = false
                                                    };

                                                    vastbazzarretailer(req);
                                                }
                                                catch { }
                                            }
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
                                            return RedirectToAction("Index");
                                        }

                                        // Send an email with this link
                                        string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                                        var callbackUrl = Url.Action("ConfirmEmailAdmin", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                                        callbackUrl = callbackUrl.Replace("/DEALER", "");
                                        string body = new CommonUtil().PopulateBody("", "Confirm your account", "", "" + callbackUrl + "", model.Email, model.Password, model.Pin.ToString(), "");
                                        string Welcomebody = new CommonUtil().PopulateBodyWelcome("", "Confirm your account", "", "" + callbackUrl + "", model.Email, model.Password, model.Pin.ToString(), "");
                                        new CommonUtil().Insertsendmail(model.Email, "Confirm your account", body, callbackUrl);
                                        new CommonUtil().InsertsendmailWelcome(model.Email, "Confirm your account", Welcomebody, callbackUrl);
                                        var adminemail = db.Admin_details.SingleOrDefault().email;
                                        new CommonUtil().Rsendmailadmin(adminemail, "Confirm your account", body, callbackUrl);
                                        var statusDealer = db.PushNotificationStatus.Where(a => a.UserRole == "Distributor").SingleOrDefault().Status;
                                        var statusSendSmsDealerToRetailerCreate = db.SMSSendAlls.Where(a => a.ServiceName == "RetailerCreateDLM").SingleOrDefault();
                                        var statusSendMailDealerToRetailerCreate = db.EmailSendAlls.Where(a => a.ServiceName == "RetailerCreateDLM1").SingleOrDefault().Status;

                                        if (ch.ToString() == "Register SuccessFully.")
                                        {
                                            string passsss = HttpUtility.UrlEncode(model.Password);
                                            //if (statusSendSmsDealerToRetailerCreate == "Y")
                                            //{
                                            //    try
                                            //    {
                                            //        string msgssss = "";
                                            //        string tempid = "";
                                            //        string urlss = "";

                                            //        var smsapionsts = db.apisms.Where(x => x.sts == "Y").SingleOrDefault();
                                            //        var smsstypes = db.Sending_SMS_Templates.Where(x => x.SMS_TYPE == "ADMIN_CREATE_NEW_RETAILER" && x.SMSAPIID == smsapionsts.id).SingleOrDefault();
                                            //        if (smsstypes != null)
                                            //        {

                                            //            msgssss = string.Format(smsstypes.Templates, model.Mobile, passsss, model.Pin);
                                            //            tempid = smsstypes.Templateid;
                                            //            urlss = smsapionsts.smsapi;

                                            //            smssend.sendsmsallnew(model.Mobile, msgssss, urlss, tempid);
                                            //        }
                                            //    }
                                            //    catch { }
                                            //    //  smssend.sendsmsall(model.Mobile, "Dear Partner ! Welcome Your user Id " + model.Mobile + " and Password " + passsss + " and PIN " + model.Pin + ". Thanks For Your Business . ", "User Create");
                                            //}

                                            smssend.sms_init(statusSendSmsDealerToRetailerCreate.Status, statusSendSmsDealerToRetailerCreate.Whatsapp_Status, "ADMIN_CREATE_NEW_RETAILER", model.Mobile, model.Mobile + " ", passsss + " ", model.Pin);

                                            if (statusSendMailDealerToRetailerCreate == "Y")
                                            {
                                                smssend.SendEmailAll(model.Email, "Dear Partner ! Welcome Your user Id " + model.Mobile + " and Password " + passsss + " and PIN " + model.Pin + ". Thanks For Your Business . ", "User Create", adminemail);
                                            }

                                            TempData["msgrem"] = ch;
                                            return RedirectToAction("Index");
                                        }
                                        else
                                        {
                                            transaction.Rollback();
                                            TempData["mobileno"] = ch;
                                            return RedirectToAction("Index");
                                        }

                                    }
                                    else
                                    {
                                        var ss = "";
                                        foreach (var item in result.Errors)
                                        {
                                            ss = item;
                                        }
                                        TempData["emailconfrim"] = ss;
                                        return RedirectToAction("Index");
                                    }

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
                                TempData["Error"] = "Please select District";
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

        public void vastbazzarretailer(pininsert model)
        {
            try
            {
                var tokenapi = Responsetoken.gettoken();

                var client = new RestClient("http://api.vastbazaar.com/api/Update/Pin");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Authorization", "Bearer " + tokenapi);
                request.AddHeader("Content-Type", "application/json");
                var tbody = new
                {
                    model.Mobile,
                    model.RetailerName,
                    model.Pincode,
                    model.Email,
                    model.Address,
                    model.State_name,
                    model.Dist_Desc,
                    model.Frm_Name,
                    model.DealerName,
                    model.dlmpin,
                    model.dlmfirm,
                    model.dlmmobile,
                    model.dlmstate,
                    model.dealerdistrict,
                    model.dlmemail,
                    model.mstername,
                    model.msterfirmname,
                    model.mstermobile,
                    model.msteremail,
                    model.mstrpin,
                    model.msterstatename,
                    model.msterdistictname,
                    model.isseleep
                };
                var infodata = JsonConvert.SerializeObject(tbody);
                request.AddParameter("application/json", infodata, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
            }
            catch { }
        }

        public class pininsert
        {
            public string Mobile { get; set; }
            public string RetailerName { get; set; }
            public string Pincode { get; set; }
            public string Email { get; set; }
            public string Address { get; set; }
            public string State_name { get; set; }
            public string Dist_Desc { get; set; }
            public string Frm_Name { get; set; }
            public string DealerName { get; set; }
            public string dlmpin { get; set; }
            public string dlmfirm { get; set; }
            public string dlmmobile { get; set; }
            public string dlmstate { get; set; }
            public string dealerdistrict { get; set; }
            public string dlmemail { get; set; }
            public string mstername { get; set; }
            public string msterfirmname { get; set; }
            public string mstermobile { get; set; }
            public string msteremail { get; set; }
            public string mstrpin { get; set; }
            public string msterstatename { get; set; }
            public string msterdistictname { get; set; }
            public bool isseleep { get; set; }
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
        //POST : Delaer Search
        /// <summary>
        /// Searches for retailers by name or ID.
        /// </summary>
        [HttpPost]
        public ActionResult RetailerSearch(string userid)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var ch = db.Retailer_Details.Where(aa => aa.RetailerId == userid).ToList();

            return Json(ch.ToArray(), JsonRequestBehavior.AllowGet);

        }

        //Edit Retailer List 
        /// <summary>
        /// Displays and processes the retailer edit form.
        /// </summary>
        [HttpPost]
        public ActionResult Editretailer(RetailerModel model)
        {
            try
            {
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    if (model.State1 != 0 && model.District1 != 0)
                    {
                        WebImage photo = null;
                        WebImage backphoto = null;
                        WebImage panphoto = null;
                        WebImage shopselfie = null;
                        var newFileName = "";
                        var imagePath = "";
                        var imagePath2 = "";
                        var panimagePath = "";
                        var selfieimagePath = "";
                        var kycvideopath = "";
                        if (Request.HttpMethod == "POST")
                        {
                            photo = WebImage.GetImageFromRequest("aadharcardfrontdoc");
                            backphoto = WebImage.GetImageFromRequest("aadharcardbackdoc");
                            panphoto = WebImage.GetImageFromRequest("pancarddoc");
                            shopselfie = WebImage.GetImageFromRequest("shopwithselfiledoc");
                            var kycvideofile = model.KYCVIDEOfile;

                            if (photo != null || backphoto != null)
                            {
                                if (photo != null && backphoto != null)
                                {
                                    newFileName = Guid.NewGuid().ToString() + "_" +
                                    Path.GetFileName(photo.FileName);
                                    newFileName = Regex.Replace(newFileName, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled);
                                    imagePath = @"\Retailer_image\" + newFileName;
                                    photo.Save(@"~\" + imagePath);

                                    newFileName = Guid.NewGuid().ToString() + "_" +
                                    Path.GetFileName(backphoto.FileName);
                                    newFileName = Regex.Replace(newFileName, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled);
                                    imagePath2 = @"\Retailer_image\" + newFileName;
                                    backphoto.Save(@"~\" + imagePath2);
                                }
                                else
                                {
                                    TempData["Error"] = "Please Upload Both Front And Back Aadhar";
                                    return RedirectToAction("Index");
                                }
                            }

                            if (panphoto != null)
                            {
                                newFileName = Guid.NewGuid().ToString() + "_" +
                                Path.GetFileName(panphoto.FileName);
                                newFileName = Regex.Replace(newFileName, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled);
                                panimagePath = @"\Retailer_image\" + newFileName;
                                panphoto.Save(@"~\" + panimagePath);
                            }

                            if (shopselfie != null)
                            {
                                newFileName = Guid.NewGuid().ToString() + "_" +
                                Path.GetFileName(shopselfie.FileName);
                                newFileName = Regex.Replace(newFileName, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled);
                                selfieimagePath = @"\Retailer_image\" + newFileName;
                                shopselfie.Save(@"~\" + selfieimagePath);
                            }

                            if (kycvideofile != null)
                            {
                                //converting byte into mb
                                decimal kyclenght = kycvideofile.ContentLength / (1024 * 1024);

                                if (kyclenght <= 20)
                                {
                                    string extension = System.IO.Path.GetExtension(Request.Files["kycvideofile"].FileName).ToUpper();
                                    string[] validFileTypes = { ".MP4", ".WEBM", ".OGG", ".AVI", ".DAT", ".MPEG", ".3GP", ".MPG", ".MP2", ".MPE", ".MPV", ".M4P", ".M4V", ".WMV", ".MOV", ".QT", ".FLV", ".SWF", ".AVCHD" };
                                    if (validFileTypes.Contains(extension))
                                    {

                                        newFileName = "WEBDealer_" + Guid.NewGuid().ToString() + "_" + "video";
                                        kycvideopath = @"\Retailer_Video\" + newFileName + ".mp4";
                                        kycvideofile.SaveAs(Server.MapPath("~/") + kycvideopath);
                                    }
                                    else
                                    {
                                        TempData["Error"] = "Please Upload Valid Video File";
                                        return RedirectToAction("Index");
                                    }
                                }
                                else
                                {
                                    TempData["Error"] = "Video file size is too large";
                                    return RedirectToAction("Index");
                                }


                            }

                        }
                        var userid = User.Identity.GetUserId();
                        Retailer_Details obj = (from pp in db.Retailer_Details where pp.RetailerId == model.Retailerid select pp).SingleOrDefault();
                        obj.RetailerName = model.Name1;
                        obj.Frm_Name = model.Firm1;
                        obj.State = model.State1;
                        obj.District = model.District1;
                        obj.Address = model.Address1;
                        obj.Pincode = Convert.ToInt32(model.Pincode1);
                        //obj.slab_name = model.Slab1;
                        obj.PanCard = model.Pan1;
                        obj.AadharCard = model.Adhaar1;
                        //obj.caption = Convert.ToInt32(model.Capping1);
                        obj.gst = model.Gst1;
                        obj.aadharcardPath = !string.IsNullOrWhiteSpace(imagePath) ? imagePath : obj.aadharcardPath;
                        obj.BackSideAadharcardphoto = !string.IsNullOrWhiteSpace(imagePath2) ? imagePath2 : obj.BackSideAadharcardphoto;
                        obj.pancardPath = !string.IsNullOrWhiteSpace(panimagePath) ? panimagePath : obj.pancardPath;
                        obj.ShopwithSalfie = !string.IsNullOrWhiteSpace(selfieimagePath) ? selfieimagePath : obj.ShopwithSalfie;
                        obj.videokycpath = !string.IsNullOrWhiteSpace(kycvideopath) ? kycvideopath : obj.videokycpath;

                        var dlm_to_rem_margine = db.dlm_to_rem_margine.Where(p => p.remid == model.Retailerid && p.dlmid == userid).SingleOrDefault();
                        dlm_to_rem_margine.marginecomm = model.offmargineEditn == null ? 0 : model.offmargineEditn;
                        dlm_to_rem_margine.dlmid = userid;
                        db.SaveChanges();

                        TempData["edit"] = "Update Successfully..";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["Error"] = "Please select District";
                        return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Something Went Wrong";
            }
            return RedirectToAction("Index");
        }

        //delete Retailer and send otp 
        /// <summary>
        /// Sends OTP before deleting a retailer account.
        /// </summary>
        public JsonResult DeleteRetailerSendOTP()
        {
            try
            {
                var userid = User.Identity.GetUserId();
                int pin = new Random().Next(1000, 10000);
                deleteuserotp motp = new deleteuserotp();
                motp.otp = pin;


                db.deleteuserotps.Add(motp);

                db.SaveChanges();
                new DeleteUserSendOtp().sendotpBydealermail(pin, userid);
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("Failed", JsonRequestBehavior.AllowGet);
            }

        }
        //delete retailer

        private static Random randomalpha = new Random();
        public static string RandomAlphanumericString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[randomalpha.Next(s.Length)]).ToArray());
        }


        #region distributor to rem setting

        /// <summary>
        /// Processes automatic fund transfer from dealer to REM.
        /// </summary>
        public async Task<ActionResult> AutoFundTransfer_Dealer_To_rem()
        {
            // IEnumerable<Autofundtransfermodel> modes;
            Autofundtransfermodelviewmodel modes = new Autofundtransfermodelviewmodel();
            MethodeClsForAuto autofund = new MethodeClsForAuto();
            var adminuserid = User.Identity.GetUserId();
            modes.autofundtransfermodel = (from tbl in db.autofundtransferdealer_to_retailer
                                           join tbl1 in db.Retailer_Details
                                           on tbl.remid equals tbl1.RetailerId
                                           where tbl.dlmid.Equals(adminuserid) && tbl1.ISDeleteuser == false
                                           select new Autofundtransfermodel
                                           {
                                               idno = tbl.idno,
                                               Name = tbl1.Frm_Name + " " + tbl1.Mobile,
                                               status = tbl.status,
                                               minimiumamount = tbl.minamount,
                                               transferamount = tbl.Transferamount,
                                               totaltransfer = tbl.totaltransfer,
                                               MaxCredit = tbl.MaxCredit,
                                               transferdatetime = tbl.updatedatetime,
                                               types = tbl.types

                                           }).ToList();


            return View(modes);
        }


        /// <summary>
        /// Gets the auto-transfer amount setting for dealer-to-REM.
        /// </summary>
        public JsonResult GetAutoTransferAmountDistributorTOrem(int idno)
        {
            var AutoCreditTransfer = db.autofundtransferdealer_to_retailer.Find(idno);

            return Json(AutoCreditTransfer, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Saves the auto-transfer amount for dealer-to-REM.
        /// </summary>
        public JsonResult SaveautoTransferDistributorToremamount(int idno, decimal transamnt, decimal minimiumset, int countperday, decimal uptolimit)
        {
            var AutoCreditTransfer = db.autofundtransferdealer_to_retailer.Find(idno);
            if (AutoCreditTransfer != null)
            {
                AutoCreditTransfer.minamount = minimiumset;
                AutoCreditTransfer.Transferamount = transamnt;
                AutoCreditTransfer.totaltransfer = countperday;
                AutoCreditTransfer.MaxCredit = uptolimit;
                db.SaveChanges();
            }
            return Json(new { minimiumbal = AutoCreditTransfer.minamount, transferamnt = AutoCreditTransfer.Transferamount, daycount = AutoCreditTransfer.totaltransfer, limit = AutoCreditTransfer.MaxCredit }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Toggles the status of auto credit transfer for dealer-to-REM.
        /// </summary>
        public ActionResult ChangeStatusAutoCreditDistributorToremTrnasfer(int idno, string curntsts)
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
                var AutoCreditTransfer = db.autofundtransferdealer_to_retailer.Find(idno);
                if (AutoCreditTransfer != null)
                {
                    AutoCreditTransfer.types = curntsts;

                    db.SaveChanges();

                }
                return Json(AutoCreditTransfer.types, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var AutoCreditTransfer = db.autofundtransferdealer_to_retailer.Find(idno);
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



        /// <summary>
        /// Deletes a retailer account from the dealer portfolio.
        /// </summary>
        public JsonResult Delete_Retailer(string RetailerId, int OTP)
        {
            try
            {
                var chk = db.deleteuserotps.Any(a => a.otp == OTP);
                if (chk == true)
                {
                    var retailerdetails = db.Retailer_Details.Where(a => a.RetailerId == RetailerId && a.ISDeleteuser == false).SingleOrDefault();

                    string chkstats = "OK";
                    if (chk != null)
                    {
                        var ekycstschk = db.ekycChecks.Where(x => x.userid == RetailerId).SingleOrDefault();
                        if (ekycstschk != null)
                        {
                            if (ekycstschk.isvalid == true)
                            {
                                chkstats = "NOTOK";
                            }
                        }
                        if (chkstats == "OK")
                        {
                            //  db.delete_Retailer(RetailerIddelete);

                            System.Data.Entity.Core.Objects.ObjectParameter output = new
                                   System.Data.Entity.Core.Objects.ObjectParameter("Output", typeof(string));

                            var msgres = db.Retailer_delete_Only_change_Status(RetailerId, output).SingleOrDefault().msg;



                            if (msgres.ToUpper() == "SUCCESS")
                            {

                                var retailersss = db.Retailer_Details.Where(x => x.RetailerId == RetailerId).SingleOrDefault();

                                string path = Server.MapPath("~/" + retailersss.videokycpath);


                                //   System.IO.DirectoryInfo di = new DirectoryInfo(path);

                                if (System.IO.File.Exists(path))
                                {
                                    System.IO.File.Delete(path);
                                    retailersss.videokycpath = null;
                                    db.SaveChanges();
                                }

                            }
                            return Json("Success", JsonRequestBehavior.AllowGet);
                        }




                        //    //decimal retailerRemain = db.Remain_reteller_balance.Where(pp => pp.RetellerId == RetailerId).Single().Remainamount;
                        //    //decimal? dealerremain = db.Remain_dealer_balance.Where(a => a.DealerID == retailerdetails.DealerId).Single().Remainamount;
                        //    //decimal? Totalvalue = dealerremain + retailerRemain;





                        //    //var admin = db.Remain_dealer_balance.Where(a => a.DealerID == retailerdetails.DealerId).SingleOrDefault();
                        //    //admin.Remainamount = Totalvalue;
                        //    //db.SaveChanges();
                        //    //db.delete_Retailer(RetailerId);
                        //    //delete otp table
                        //    //db.deleteOTP();
                        //    //ledger.UserId = retailerdetails.RetailerId;
                        //    //ledger.Role = "Retailer";
                        //    //ledger.Particulars = "Retailer Id Delete By Distributor";
                        //    //ledger.UserRemainAmount = retailerRemain;
                        //    //ledger.Date = Convert.ToDateTime(DateTime.Now.ToString());
                        //    //ledger.Amount = retailerRemain;
                        //    //ledger.Credit = 0;
                        //    //ledger.Debit = retailerRemain;
                        //    //db.LedgerReports.Add(ledger);
                        //    //db.SaveChanges();

                        //    //ledger.UserId = retailerdetails.DealerId;
                        //    //ledger.Role = "Dealer";
                        //    //ledger.Particulars = "Retailer Id Delete By Distributor";
                        //    //ledger.UserRemainAmount = Totalvalue;
                        //    //ledger.Date = Convert.ToDateTime(DateTime.Now.ToString());
                        //    //ledger.Amount = retailerRemain;
                        //    //ledger.Credit = retailerRemain;
                        //    //ledger.Debit = 0;
                        //    //db.LedgerReports.Add(ledger);
                        //    //db.SaveChanges();

                        //    return Json("Success", JsonRequestBehavior.AllowGet);
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
            catch (Exception ex)
            {
                return Json("Something went wrong", JsonRequestBehavior.AllowGet);
            }


            return Json("Something went wrong", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Displays and manages UPI dealer charge settings.
        /// </summary>
        public ActionResult UPIDealerCharges()
        {
            var Roleidrid = User.Identity.GetUserId();
            var userid = "ALL";
            var remlist = db.select_retailer_for_ddl(Roleidrid);
            ViewBag.Retailerid = new SelectList(remlist, "RetailerId", "Frm_Name", null);

            upiChargeAndgateway model = new upiChargeAndgateway();
            model.msg = "";
            DateTime txt_frm_date = DateTime.Now.Date;
            DateTime to = DateTime.Now.AddDays(1).Date;
            var ch = db.show_upi_txn_details(Roleidrid, userid, "ALL", txt_frm_date, to).ToList();
            var chk1 = db.Upi_slab.ToList();

            model.Upi_slabs = chk1;

            model.UPIREPORT = ch;
            model.Admin_UPIs = db.Admin_UPI.ToList();
            model.Dealer_VPAID = db.Dealer_VPAID.ToList();

            return View(model);
        }
        /// <summary>
        /// Displays and manages UPI dealer charge settings.
        /// </summary>
        [HttpPost]
        public ActionResult UPIDealerCharges(string ddlname, string Retailerid, string ddlstatus, DateTime txt_frm_date, DateTime txt_to_date)
        {
            ViewBag.chk = "post";
            var Roleidrid = User.Identity.GetUserId();
            var remlist = db.select_retailer_for_ddl(Roleidrid);
            ViewBag.Retailerid = new SelectList(remlist, "RetailerId", "Frm_Name", null);
            ViewBag.Retailerid1 = new SelectList(db.select_retailer_for_ddl(Roleidrid), "RetailerId", "Frm_Name", null);

            var userid = "ALL";

            if (ddlname == "Retailer")
            {
                if (Retailerid != "")
                {
                    userid = Retailerid;
                }
            }
            DateTime to = txt_to_date.AddDays(1);
            upiChargeAndgateway model = new upiChargeAndgateway();
            var ch = db.show_upi_txn_details(Roleidrid, userid, ddlstatus, txt_frm_date, to).ToList();
            var chk1 = db.Upi_slab.ToList();
            model.UPIREPORT = ch;


            model.msg = "";
            model.Upi_slabs = chk1;
            model.Admin_UPIs = db.Admin_UPI.ToList();
            model.Dealer_VPAID = db.Dealer_VPAID.ToList();

            return View(model);
        }

        /// <summary>
        /// Sends OTP for UPI dealer self-registration.
        /// </summary>
        public ActionResult DealerSENDOTP()
        {
            var userid = User.Identity.GetUserId();
            var dealerdetails = db.Dealer_Details.Where(aa => aa.DealerId == userid).SingleOrDefault();
            upiChargeAndgateway model = new upiChargeAndgateway();
            Random ran = new Random();
            var otpn = ran.Next(1111, 9999);
            MobileOtp mob = new MobileOtp();
            mob.Userid = "Dealer";
            mob.Otp = otpn.ToString();
            mob.Type = "UPISELF";
            mob.Date = DateTime.Now;
            mob.mobileno = dealerdetails.Mobile;
            db.MobileOtps.Add(mob);
            db.SaveChanges();

            smssend.sms_init("Y", "Y", "SELF UPI INSERT OTP", dealerdetails.Mobile, " " + otpn);

            try
            {
                CommUtilEmail common = new CommUtilEmail();
                common.EmailLimitChk(dealerdetails.Email, "OK", "Self UPI Insert OTP", "Your Self UPI Insert OTP Is " + otpn.ToString(), "");
            }
            catch { }
            var chk1 = db.Admin_UPI.ToList();
            model.Admin_UPIs = chk1;
            model.Dealer_VPAID = db.Dealer_VPAID.ToList();
            model.msg = "OTPSEND SUCCESSFULLY";
            return PartialView("_VPAsetDealer_SelfList", model);
        }

        /// <summary>
        /// Deletes a self UPI entry for the dealer.
        /// </summary>
        public ActionResult deleteSelefUpiSelf(int upiid)
        {
            var findgetusers = db.Dealer_VPAID.Find(upiid);
            if (findgetusers != null)
            {
                db.Dealer_VPAID.Remove(findgetusers);
                db.SaveChanges();
            }
            upiChargeAndgateway model = new upiChargeAndgateway();
            var chk1 = db.Admin_UPI.ToList();
            model.Admin_UPIs = chk1;
            model.Dealer_VPAID = db.Dealer_VPAID.ToList();
            return PartialView("_VPAsetDealer_SelfList", model);
        }

        /// <summary>
        /// Activates or deactivates a self UPI ID for the dealer.
        /// </summary>
        public ActionResult DealerActiveUpiSelf(int upiid)
        {
            var findgetusers = db.Dealer_VPAID.FirstOrDefault(x => x.idno == upiid);
            if (findgetusers != null)
            {
                var result = db.Dealer_VPAID.Select(x => x.idno).ToList();


                var friendsToUpdate = db.Dealer_VPAID.Where(f => result.Contains(f.idno)).ToList();

                foreach (var item in friendsToUpdate)
                {
                    item.Isdefault = false;

                }
                db.SaveChanges();

                findgetusers.Isdefault = true;
                // db.Admin_UPI.Remove(findgetusers);
                db.SaveChanges();
            }
            upiChargeAndgateway model = new upiChargeAndgateway();
            var chk1 = db.Admin_UPI.ToList();
            model.Admin_UPIs = chk1;
            model.Dealer_VPAID = db.Dealer_VPAID.ToList();
            return PartialView("_VPAsetDealer_SelfList", model);
        }

        /// <summary>
        /// Adds a new VPA/UPI ID for the dealer.
        /// </summary>
        public ActionResult DealerAddVpaID(string upiid, string otp, string paymenttype)
        {
            var userid = User.Identity.GetUserId();
            upiChargeAndgateway model = new upiChargeAndgateway();

            var admin = db.Dealer_Details.Where(aa => aa.DealerSelf_sts == true && aa.DealerId == userid).SingleOrDefault();
            if (admin != null)
            {

                var totalselfcount = db.Dealer_VPAID.Count();
                if (totalselfcount < 5)
                {

                    var lastotp = db.MobileOtps.Where(aa => aa.Userid == "Dealer" && aa.Type == "UPISELF").OrderByDescending(aa => aa.idno).FirstOrDefault().Otp;

                    if (lastotp == otp)
                    {
                        Dealer_VPAID ad = new Dealer_VPAID();
                        ad.DealerVPAID = upiid.Trim();
                        ad.DealerVPATYPE = paymenttype;
                        db.Dealer_VPAID.Add(ad);
                        db.SaveChanges();

                        //delete used otp
                        var dealerdetails = db.Dealer_Details.Where(aa => aa.DealerId == userid).SingleOrDefault();
                        var deleteotp = db.MobileOtps.Where(aa => aa.Type == "UPISELF" && aa.mobileno == dealerdetails.Mobile).ToList();
                        db.MobileOtps.RemoveRange(deleteotp);
                        db.SaveChanges();
                        var chk1 = db.Admin_UPI.ToList();
                        model.Admin_UPIs = chk1;
                        model.Upi_slabs = db.Upi_slab.ToList();
                        model.Dealer_VPAID = db.Dealer_VPAID.ToList();

                    }
                    else
                    {
                        var chk1 = db.Admin_UPI.ToList();
                        model.Admin_UPIs = chk1;
                        model.Upi_slabs = db.Upi_slab.ToList();
                        model.Dealer_VPAID = db.Dealer_VPAID.ToList();

                    }

                }

            }
            else
            {
                model.msg = "contact to  admin for allow";
            }
            model.Dealer_VPAID = db.Dealer_VPAID.ToList();
            return PartialView("_VPAsetDealer_SelfList", model);
        }

        /// <summary>
        /// Manually marks a pending UPI transaction as successful.
        /// </summary>
        public ActionResult UPI_Manual_Pending_To_Success(int? hideupiidres, string hideupiidrestypes, string txtBankRRN, string txtcode)
        {
            try
            {
                ViewBag.chk = "post";
                var userid = User.Identity.GetUserId();

                if (hideupiidrestypes == "REJECTED")
                {
                    txtBankRRN = hideupiidrestypes;
                }
                if (!string.IsNullOrEmpty(hideupiidrestypes) && !string.IsNullOrEmpty(txtBankRRN) && !string.IsNullOrEmpty(txtcode))
                {
                    if (hideupiidrestypes == "REJECTED")
                    {
                        txtBankRRN = null;
                    }
                    var password = Encrypt(txtcode);
                    var tranpass = db.Dealer_Details.Where(x => x.DealerId == userid && x.TransPIN == password).Count();

                    if (tranpass > 0)
                    {
                        System.Data.Entity.Core.Objects.ObjectParameter output = new
                          System.Data.Entity.Core.Objects.ObjectParameter("Output", typeof(string));
                        var outres = db.Update_Upitxn_Pending_toSuccessFailed(hideupiidres, hideupiidrestypes, txtBankRRN, output).SingleOrDefault().msg;
                        var infochk = db.Upi_txn_details.Where(aa => aa.idno == hideupiidres).SingleOrDefault();
                        try
                        {
                            var retailerdetails = db.Retailer_Details.Where(aa => aa.RetailerId == infochk.userid).SingleOrDefault();
                            var dealerdetails = db.Dealer_Details.Where(aa => aa.DealerId == userid).SingleOrDefault();
                           
                            var remdetails = db.Remain_reteller_balance.Where(aa => aa.RetellerId == infochk.userid).SingleOrDefault();
                            var dlmdetails = db.Remain_dealer_balance.Where(aa => aa.DealerID == userid).SingleOrDefault();
                           
                            var admininfo = db.Admin_details.SingleOrDefault();
                            Backupinfo back = new Backupinfo();
                            var modeln = new Backupinfo.Addinfo
                            {
                                Websitename = admininfo.WebsiteUrl,
                                RetailerID = userid,
                                Email = retailerdetails.Email,
                                Mobile = retailerdetails.Mobile,
                                Details = "UPI Txn Pending to Success",
                                RemainBalance = (decimal)remdetails.Remainamount,
                                Usertype = "Retailer"
                            };
                            back.Fundtransfer(modeln);

                            var model1 = new Backupinfo.Addinfo
                            {
                                Websitename = admininfo.WebsiteUrl,
                                RetailerID = dealerdetails.DealerId,
                                Email = dealerdetails.Email,
                                Mobile = dealerdetails.Mobile,
                                Details = "UPI Txn Pending to Success",

                                RemainBalance = Convert.ToDecimal(dlmdetails.Remainamount),
                                Usertype = "Dealer"
                            };
                            back.Fundtransfer(model1);

                           
                        }
                        catch { }
                        if (outres == "Successfully")
                        {
                            TempData["msgupisucc"] = outres;
                            return RedirectToAction("UPIDealerCharges");
                        }
                        else
                        {
                            TempData["msgupi"] = outres;
                            return RedirectToAction("UPIDealerCharges");
                        }

                    }
                    else
                    {
                        TempData["msgupi"] = "Transaction Password was Wrong.";
                        return RedirectToAction("UPIDealerCharges");
                    }
                }
                else
                {
                    TempData["msgupi"] = "All Field Required";
                    return RedirectToAction("UPIDealerCharges");
                }
            }
            catch (Exception ex)
            {
                TempData["msgupi"] = "Something went wrong";
                return RedirectToAction("UPIDealerCharges");
            }

        }

        //Login Deatils

        #region Notification
        /// <summary>
        /// Displays the dealer notification center.
        /// </summary>
        public ActionResult Notification()
        {
            ViewData["success"] = TempData["success"];
            TempData.Remove("success");
            var userid = User.Identity.GetUserId();
            ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl(userid), "RetailerId", "RetailerName", null).ToList();
            return View();
        }
        /// <summary>
        /// Sends a notification to retailers under the dealer.
        /// </summary>
        [HttpPost]
        public ActionResult Send_Notification(string retailerid, string txtMsgBody)
        {
            if (retailerid == "" || retailerid == null)
            {
                var usermobiles = db.Retailer_Details.ToList();
                foreach (var item in usermobiles)
                {
                    SendPushNotification(item.Email, Url.Action("Dashboard", "Home"), txtMsgBody, "Notification.");

                    notify.sendmessage(item.Email, txtMsgBody);
                }

            }
            else
            {
                var email = db.Retailer_Details.Where(a => a.RetailerId == retailerid).FirstOrDefault().Email;
                SendPushNotification(email, Url.Action("Dashboard", "Home"), txtMsgBody, "Notification.");

                notify.sendmessage(email, txtMsgBody);
            }
            TempData["success"] = "Notification send successfully.";
            return RedirectToAction("Notification", "Home");
        }
        #endregion End Notification
        //Web Login Details
        /// <summary>
        /// Handles the dealer web login page and authentication.
        /// </summary>
        public ActionResult WebLogin()
        {
            var userid = User.Identity.GetUserName();
            DateTime txt_frm_date = DateTime.Now.Date;
            DateTime txt_to_date = DateTime.Now;
            var Login_details = (db.Login_info.Where(aa => aa.UserId == userid && aa.CurrentLoginTime >= txt_frm_date && aa.CurrentLoginTime <= txt_to_date && aa.LoginFrom == "Web").Take(100).OrderByDescending(aa => aa.Idno)).ToList();
            return View(Login_details);
        }
        /// <summary>
        /// Handles the dealer web login page and authentication.
        /// </summary>
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

            var Login_details = (db.Login_info.Where(aa => aa.UserId == userid && aa.CurrentLoginTime >= frm_date && aa.CurrentLoginTime <= to_date && aa.LoginFrom == "Web").OrderByDescending(aa => aa.Idno)).ToList(); ;
            return View(Login_details);

        }
        //Login Failed
        /// <summary>
        /// Displays the web login failed page with error details.
        /// </summary>
        [HttpGet]
        public ActionResult WebLoginFailed()
        {
            var userid = User.Identity.GetUserName();
            DateTime txt_frm_date = DateTime.Now.Date;
            DateTime txt_to_date = DateTime.Now;
            var Faild_Login_details = (db.Failed_Login_info.Where(aa => aa.EmailId == userid && aa.LoginTime >= txt_frm_date && aa.LoginTime <= txt_to_date && aa.LoginFrom == "Web").Take(100).OrderByDescending(aa => aa.Idno)).ToList();
            return View(Faild_Login_details);
        }
        /// <summary>
        /// Displays the web login failed page with error details.
        /// </summary>
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

            var Faild_Login_details = (db.Failed_Login_info.Where(aa => aa.EmailId == userid && aa.LoginTime >= frm_date && aa.LoginTime <= to_date && aa.LoginFrom == "Web").OrderByDescending(aa => aa.Idno)).ToList();

            return View(Faild_Login_details);

        }
        //Bank Details
        /// <summary>
        /// Displays the bank details page for fund deposit reference.
        /// </summary>
        public ActionResult Bank_Details()
        {
            var userid = User.Identity.GetUserId();
            var admininfo = db.Admin_details.FirstOrDefault();
            var masterid = db.Dealer_Details.Where(x => x.DealerId == userid).FirstOrDefault().SSId;
            var ch = db.bank_info.Where(x => x.userid == "Admin" || x.userid == masterid || x.userid == userid);//(from hh in db.bank_info where  select hh).ToList();
            return View(ch);
        }
        //end

        //Help and Support
        /// <summary>
        /// Displays the dealer help and support page.
        /// </summary>
        public ActionResult Help()
        {
            var admininfo = db.Admin_details.FirstOrDefault();
            ViewBag.admin = admininfo;
            return View();
        }
        /// <summary>
        /// Displays and manages all operator/recharge information.
        /// </summary>
        public ActionResult All_Operator_Info()
        {
            var userid = User.Identity.GetUserId();
            var masterid = db.Dealer_Details.Where(aa => aa.DealerId == userid).SingleOrDefault();
            var info = db.Master_allow_commission.Where(aa => aa.masterid == masterid.SSId).SingleOrDefault().allowcommision;
            Operater_Commission sb = new Operater_Commission();
            sb.AmountRange = (from p in db.Recharge_Amount_range
                              select new Recharge_Amount_range_info
                              {
                                  idno = p.idno,
                                  maximum1 = p.maximum1,
                                  maximum2 = p.maximum2
                              }).ToList();
            if (info == false)
            {
                sb.Prepaid = (from cust in db.prepaid_dealer_comm
                              join ord in db.Operator_Code
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
            }
            else
            {
                sb.Prepaid = (from cust in db.prepaid_dealer_comm_by_master.Where(aa => aa.masterid == masterid.SSId)
                              join ord in db.Operator_Code
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
            }

            sb.Electricity_comm = (from cust in db.utility_dealer_comm
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

            sb.Money2 = (from cust in db.paytm_imps_dealer_comm
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


            //sb.Money = (from cust in db.imps_dealer_comm
            //            where (cust.userid == userid)
            //            select new money_comm
            //            {
            //                Verifycomm = cust.verify_comm,
            //                comm_1000 = cust.comm_1000,
            //                comm_2000 = cust.comm_2000,
            //                comm_3000 = cust.comm_3000,
            //                comm_4000 = cust.comm_4000,
            //                comm_5000 = cust.comm_5000,
            //                gst = cust.gst
            //            }).ToList();
            sb.Pencard_comm = (from cust in db.Pancard_distributor_Common_comm
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
            sb.MPOS = (from cust in db.Mpos_comm_details
                       where cust.Userid == userid && cust.userRole == "dealer"
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
            sb.INDONEPAL = (from cust in db.Slab_IndoNepal
                            where cust.UserId == userid && cust.Role == "Dealer"
                            select new INDONEPAL_Comm
                            {
                                min = cust.min,
                                max = cust.max,
                                charge = cust.charge,
                                margin = cust.margin

                            }).ToList();

            sb.FLIGHT = (from cust in db.Slab_Flight
                         where cust.UserId == userid && cust.Role == "Dealer"
                         select new FLIGHT_Comm
                         {
                             IsDomestic = cust.IsDomestic,
                             marginPercentage = cust.marginPercentage,
                             RetailerMarkupCharge = cust.RetailerMarkupCharge,
                             gst = cust.gst,
                             tds = cust.tds
                         }).ToList();


            sb.HOTEL = (from cust in db.Slab_Hotel
                        where cust.UserId == userid && cust.Role == "Dealer"
                        select new HOTEL_Comm
                        {
                            IsDomestic = cust.IsDomestic == true ? false : true,
                            marginPercentage = cust.marginPercentage,
                            RetailerMarkupCharge = cust.RetailerMarkupCharge,
                            gst = cust.gst,
                            tds = cust.tds
                        }).ToList();

            sb.BUS = (from cust in db.Slab_Bus
                      where cust.UserId == userid && cust.Role == "Dealer"
                      select new BUS_Comm
                      {
                          marginPercentage = cust.marginPercentage,
                          RetailerMarkupCharge = cust.retailerMarkup,
                          gst = cust.gst,
                          tds = cust.tds
                      }).ToList();

            sb.Giftcard = (from cust in db.giftcard_dealer_comm
                           join ord in db.Operator_Code
                           on cust.optcode equals ord.Operator_id.ToString()
                           where (cust.dealerid == userid)
                           select new Giftcard_Comm
                           {
                               OperatorCode = ord.new_opt_code,
                               Commission = cust.comm,
                               Status = ord.status,
                               OperatorType = ord.Operator_type,
                               OperatorName = ord.operator_Name
                           }).ToList();


            sb.Broadband = (from cust in db.Broandband_dealer_comm
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
            sb.Loan = (from cust in db.Loan_dealer_comm
                       join ord in db.Operator_Code
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

            sb.Water = (from cust in db.Water_dealer_comm
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

            sb.Insurance = (from cust in db.Insurance_dealer_comm
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

            ViewBag.allow = info;
            return View(sb);
        }
        /// <summary>
        /// Displays and manages all operator/recharge information.
        /// </summary>
        [HttpPost]
        public ActionResult All_Operator_Info(string ddlcomm)
        {
            var userid = User.Identity.GetUserId();
            Operater_Commission sb = new Operater_Commission();
            sb.Prepaid = (from cust in db.prepaid_dealer_comm
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
            sb.Electricity_comm = (from cust in db.utility_dealer_comm
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
            sb.Money = (from cust in db.imps_dealer_comm
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
            sb.Pencard_comm = (from cust in db.Pancard_distributor_Common_comm
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
            sb.MPOS = (from cust in db.Mpos_comm_details
                       where cust.Userid == userid && cust.userRole == "dealer"
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
            sb.INDONEPAL = (from cust in db.Slab_IndoNepal
                            where cust.UserId == userid && cust.Role == "Dealer"
                            select new INDONEPAL_Comm
                            {
                                min = cust.min,
                                max = cust.max,
                                charge = cust.charge,
                                margin = cust.margin

                            }).ToList();
            sb.FLIGHT = (from cust in db.Slab_Flight
                         where cust.UserId == userid && cust.Role == "Dealer"
                         select new FLIGHT_Comm
                         {
                             IsDomestic = cust.IsDomestic,
                             marginPercentage = cust.marginPercentage,
                             RetailerMarkupCharge = cust.RetailerMarkupCharge,
                             gst = cust.gst,
                             tds = cust.tds
                         }).ToList();

            sb.HOTEL = (from cust in db.Slab_Hotel
                        where cust.UserId == userid && cust.Role == "Dealer"
                        select new HOTEL_Comm
                        {
                            IsDomestic = cust.IsDomestic == true ? false : true,
                            marginPercentage = cust.marginPercentage,
                            RetailerMarkupCharge = cust.RetailerMarkupCharge,
                            gst = cust.gst,
                            tds = cust.tds
                        }).ToList();

            sb.BUS = (from cust in db.Slab_Bus
                      where cust.UserId == userid && cust.Role == "Dealer"
                      select new BUS_Comm
                      {
                          marginPercentage = cust.marginPercentage,
                          RetailerMarkupCharge = cust.retailerMarkup,
                          gst = cust.gst,
                          tds = cust.tds
                      }).ToList();

            sb.Giftcard = (from cust in db.giftcard_dealer_comm
                           join ord in db.Operator_Code
                           on cust.optcode equals ord.Operator_id.ToString()
                           where (cust.dealerid == userid)
                           select new Giftcard_Comm
                           {
                               OperatorCode = ord.new_opt_code,
                               Commission = cust.comm,
                               Status = ord.status,
                               OperatorType = ord.Operator_type,
                               OperatorName = ord.operator_Name
                           }).ToList();


            sb.Broadband = (from cust in db.Broandband_dealer_comm
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

            sb.Loan = (from cust in db.Loan_dealer_comm
                       join ord in db.Operator_Code
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
            sb.Water = (from cust in db.Water_dealer_comm
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

            sb.Insurance = (from cust in db.Insurance_dealer_comm
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

            ViewData["type"] = ddlcomm;
            var masterid = db.Dealer_Details.Where(aa => aa.DealerId == userid).SingleOrDefault();
            var info = db.Master_allow_commission.Where(aa => aa.masterid == masterid.SSId).SingleOrDefault().allowcommision;
            ViewBag.allow = info;
            return View(sb);
        }

        //Retailer Operator Block and Unblock

        /// <summary>
        /// Blocks or unblocks an operator option for the dealer.
        /// </summary>
        public ActionResult BlockOPT(string rem, string id, string opttype)
        {
            var userid = User.Identity.GetUserId();
            if (id != null)
            {
                db.update_retailer_opt_code(rem, id);
            }
            if (opttype == "")
            {
                opttype = "ALL";
            }
            var type = db.Operator_Code.Select(aa => aa.Operator_type).Distinct();
            ViewBag.opttype = new SelectList(type, null);
            var operator_value = (db.select_retailer_for_ddl(userid)).ToList();
            ViewBag.retailer = new SelectList(operator_value, "RetailerId", "RetailerName", null);
            //ViewBag.retailer = new SelectList(db.select_retailer_for_all(), "RetailerId", "RetailerName", null);
            var ch1 = db.select_operator_retailer(rem, opttype).ToList();
            return View(ch1);
        }
        /// <summary>
        /// Blocks or unblocks an operator option for the dealer.
        /// </summary>
        [HttpPost]
        public ActionResult BlockOPT(string rem, string opttype)
        {
            var userid = User.Identity.GetUserId();
            if (opttype == "")
            {
                opttype = "ALL";
            }
            var type = db.Operator_Code.Select(aa => aa.Operator_type).Distinct();
            ViewBag.opttype = new SelectList(type, null);
            var operator_value = (db.select_retailer_for_ddl(userid)).ToList();
            ViewBag.retailer = new SelectList(operator_value, "RetailerId", "RetailerName", null);
            var ch1 = db.select_operator_retailer(rem, opttype).ToList();
            return View(ch1);
        }
        //end
        //Retailer_Slab
        /// <summary>
        /// Displays and manages retailer commission slabs.
        /// </summary>
        [HttpGet]
        public ActionResult Retailer_Slab()
        {
            string userid = User.Identity.GetUserId();
            var hh = (from jj in db.dlm_rem_slab where jj.dlmid == userid where jj.sts == "Y" select jj).ToList();
            ViewBag.slabnm = new SelectList(hh, "slab_name", "slab_name");
            var ch = (from jj in db.retailer_slab where jj.retailer_id == "" select jj);
            return View(ch);
        }
        /// <summary>
        /// Displays and manages retailer commission slabs.
        /// </summary>
        [HttpPost]
        public ActionResult Retailer_Slab(string slabnm)
        {
            string userid = User.Identity.GetUserId();
            var hh = (from jj in db.dlm_rem_slab where jj.dlmid == userid where jj.sts == "Y" select jj).ToList();
            ViewBag.slabnm = new SelectList(hh, "slab_name", "slab_name");
            var ch = (from jj in db.retailer_slab where jj.retailer_id == slabnm select jj);
            return View(ch);
        }

        //Dealer Ledger Report
        /// <summary>
        /// Displays the dealer ledger with date range filtering.
        /// </summary>
        [HttpGet]
        public ActionResult DealerLedger()
        {
            var userid = User.Identity.GetUserId();
            string txt_to_date1 = DateTime.Now.ToString();
            string frm_date = Convert.ToDateTime(txt_to_date1).ToShortDateString();
            string to_date = Convert.ToDateTime(txt_to_date1).AddDays(1).ToString();
            var ledger = db.Retailer_Cr_Dr_Report("Dealer", userid, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            return View(ledger);
        }

        /// <summary>
        /// Displays the dealer ledger with date range filtering.
        /// </summary>
        [HttpPost]
        public ActionResult DealerLedger(string txt_frm_date)
        {
            var userid = User.Identity.GetUserId();
            ViewBag.chk = "post";
            string txtfrm = DateTime.Now.Date.ToString();
            string frm_date = Convert.ToDateTime(txt_frm_date).Date.ToString();
            string to_date = Convert.ToDateTime(txt_frm_date).AddDays(1).ToString();
            var ledger = db.Retailer_Cr_Dr_Report("Dealer", userid, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            return View(ledger);

        }
        /// <summary>
        /// Exports the dealer ledger report to Excel.
        /// </summary>
        public ActionResult ExcelDealerLedger(string txt_frm_date)
        {
            var userid = User.Identity.GetUserId();
            string frm_date = Convert.ToDateTime(txt_frm_date).Date.ToString();
            string to_date = Convert.ToDateTime(txt_frm_date).AddDays(1).ToString();
            var ledger = db.Retailer_Cr_Dr_Report("Dealer", userid, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            DataTable dataTbl = new DataTable();
            dataTbl.Columns.Add("Request Id", typeof(string));
            dataTbl.Columns.Add("Particular", typeof(string));
            dataTbl.Columns.Add("Amount", typeof(string));
            dataTbl.Columns.Add("Credit", typeof(string));
            dataTbl.Columns.Add("Debit", typeof(string));
            dataTbl.Columns.Add("Balance", typeof(string));
            dataTbl.Columns.Add("Date", typeof(string));
            if (ledger.Any())
            {
                foreach (var item in ledger)
                {
                    dataTbl.Rows.Add(item.RequestId, item.Particulars, item.Amount, item.credit, item.debit, item.Balance, item.Date);
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
            Response.AddHeader("content-disposition", "attachment; filename=Distributor Ledger Report.xls");
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
        //Money Transfer Report 
        /// <summary>
        /// Displays the money transfer report with filters.
        /// </summary>
        [HttpGet]
        public ActionResult Money_Transfer_Report()
        {
            Money_transfer_report money = new Money_transfer_report();
            var userid = User.Identity.GetUserId();

            //show retailer
            ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl(userid), "RetailerId", "Frm_Name", null).ToList();

            //apiname
            var apiname = db.money_api_status.ToList();
            IEnumerable<SelectListItem> selectapiname = from p in apiname
                                                        select new SelectListItem
                                                        {
                                                            Value = p.api_name,
                                                            Text = p.api_name
                                                        };
            ViewBag.apiname = new SelectList(selectapiname, "Value", "Text");


            return View();
        }
        /// <summary>
        /// Displays the money transfer report with filters.
        /// </summary>
        [HttpPost]
        public ActionResult Money_Transfer_Report(string txt_frm_date, string txt_to_date, string allretailer, string allapiuser, string ddl_status, string ddl_Type)
        {

            ViewBag.chk = "post";
            var loginid = User.Identity.GetUserId();


            //show retailer
            ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl(loginid), "RetailerId", "Frm_Name", null).ToList();


            return View();
        }
        /// <summary>
        /// Returns the money transfer report partial view.
        /// </summary>
        public ActionResult _MoneyTransferReport(string txt_frm_date, string txt_to_date, string allretailer, string allapiuser, string ddl_status, string ddl_Type)
        {
            Money_transfer_report money = new Money_transfer_report();

            var loginid = User.Identity.GetUserId();
            var userid = "";
            // var APIname = "";
            string ddlusers = "";
            if (txt_frm_date == null && txt_to_date == null && ddl_status == null && ddl_Type == null)
            {
                txt_frm_date = DateTime.Now.ToString();
                txt_to_date = DateTime.Now.ToString();
                ddlusers = "Master"; ddl_status = "ALL";
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


            if (allretailer == "" || allretailer == null)
            {
                userid = loginid;
                ddlusers = "Dealer";
            }
            else
            {
                userid = allretailer;
                ddlusers = "Retailer";
            }
            //if (ddlusers == "Retailer")
            //{
            //    if (allretailer == "" || allretailer.Contains("All Retailer") || allretailer == null)
            //    {
            //        userid = "ALL";
            //    }
            //    else
            //    {
            //        userid = allretailer;
            //    }
            //}


            if (ddl_status == "Status")
            {
                ddl_status = "ALL";
            }
            //if (api_name == "Select Api Name"  || api_name.Contains("Select Api Name") || api_name == "")
            //{
            //    APIname = "ALL";
            //}
            //else
            //{
            //    APIname = api_name.ToUpper();
            //}
            if (ddl_Type == null)
            {
                ddl_Type = "ALL";

            }

            var ch = db.money_transfer_report_paging(ddlusers, userid, ddl_status, "VASTWEB", Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), ddl_Type, 1, 20).ToList();

            return View(ch);
        }
        /// <summary>
        /// Returns paginated money transfer records for infinite scroll.
        /// </summary>
        public ActionResult InfiniteScrollMoneyReport(int pageindex, string txt_frm_date, string txt_to_date, string allretailer, string allapiuser, string ddl_status, string ddl_Type)
        {
            Money_transfer_report money = new Money_transfer_report();

            var loginid = User.Identity.GetUserId();
            var userid = "";
            // var APIname = "";
            string ddlusers = "";
            if (txt_frm_date == null && txt_to_date == null && ddl_status == null && ddl_Type == null)
            {
                txt_frm_date = DateTime.Now.ToString();
                txt_to_date = DateTime.Now.ToString();
                ddlusers = "Master"; ddl_status = "ALL";
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


            if (allretailer == "" || allretailer == null)
            {
                userid = loginid;
                ddlusers = "Dealer";
            }
            else
            {
                userid = allretailer;
                ddlusers = "Retailer";
            }
            //if (ddlusers == "Retailer")
            //{
            //    if (allretailer == "" || allretailer.Contains("All Retailer") || allretailer == null)
            //    {
            //        userid = "ALL";
            //    }
            //    else
            //    {
            //        userid = allretailer;
            //    }
            //}


            if (ddl_status == "Status")
            {
                ddl_status = "ALL";
            }
            //if (api_name == "Select Api Name"  || api_name.Contains("Select Api Name") || api_name == "")
            //{
            //    APIname = "ALL";
            //}
            //else
            //{
            //    APIname = api_name.ToUpper();
            //}
            if (ddl_Type == null)
            {
                ddl_Type = "ALL";

            }
            int pagesize = 20;
            var tbrow = db.money_transfer_report_paging(ddlusers, userid, ddl_status, "VASTWEB", Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), ddl_Type, pageindex, pagesize).ToList();

            JsonModel jsonmodel = new JsonModel();
            jsonmodel.NoMoredata = tbrow.Count < pagesize;
            jsonmodel.HTMLString = renderPartialViewtostring("_MoneyTransferReport", tbrow);
            return Json(jsonmodel);
        }
        /// <summary>
        /// Displays the total money transfer summary.
        /// </summary>
        public ActionResult TotalMoneyReport(string txt_frm_date, string txt_to_date, string allretailer, string allapiuser, string ddl_status, string ddl_Type)
        {


            var loginid = User.Identity.GetUserId();
            var userid = "";
            // var APIname = "";
            string ddlusers = "";


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


            if (allretailer == "" || allretailer == null)
            {
                userid = loginid;
                ddlusers = "Dealer";
            }
            else
            {
                userid = allretailer;
                ddlusers = "Retailer";
            }


            if (ddl_status == "Status")
            {
                ddl_status = "ALL";
            }

            if (ddl_Type == null)
            {
                ddl_Type = "ALL";

            }

            var ch = db.money_transfer_report_paging(ddlusers, userid, ddl_status, "VASTWEB", Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), ddl_Type, 1, 1000000).ToList();
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
        /// <summary>
        /// Exports the money transfer report to Excel.
        /// </summary>
        public ActionResult ExcelMoneyreport(string txt_frm_date, string txt_to_date, string allretailer, string allapiuser, string ddl_status, string ddl_Type)
        {
            var loginid = User.Identity.GetUserId();
            var userid = "";
            // var APIname = "";
            string ddlusers = "";


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


            if (allretailer == "" || allretailer == null)
            {
                userid = loginid;
                ddlusers = "Dealer";
            }
            else
            {
                userid = allretailer;
                ddlusers = "Retailer";
            }


            if (ddl_status == "Status")
            {
                ddl_status = "ALL";
            }

            if (ddl_Type == null)
            {
                ddl_Type = "ALL";

            }

            DataTable dt2 = new DataTable();
            dt2.Columns.Add("Status", typeof(string));
            dt2.Columns.Add("Type", typeof(string));
            dt2.Columns.Add("Beneficiary Account", typeof(string));
            dt2.Columns.Add("Sender No", typeof(string));
            dt2.Columns.Add("Net T/F", typeof(string));
            dt2.Columns.Add("Transfer Total", typeof(string));
            dt2.Columns.Add("User Income", typeof(string));
            dt2.Columns.Add("MY Pre", typeof(string));
            dt2.Columns.Add("MY Income", typeof(string));
            dt2.Columns.Add("MY Post", typeof(string));
            dt2.Columns.Add("Request Date", typeof(string));


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

                dt2.Rows.Add(sts + " - " + item.frm_name, item.Trans_type, item.accountno, item.senderno, item.totalamount, debit,
                  item.income, item.dlm_remain_pre
                , item.dlm_income, item.dlm_remain, item.response_time);
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
        /// <summary>
        /// Generates the money transfer report as a PDF.
        /// </summary>
        public ActionResult PDFMoneyReport(string txt_frm_date, string txt_to_date, string allretailer, string allapiuser, string ddl_status, string ddl_Type)
        {
            Money_transfer_report money = new Money_transfer_report();

            var loginid = User.Identity.GetUserId();
            var userid = "";
            // var APIname = "";
            string ddlusers = "";


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


            if (allretailer == "" || allretailer == null)
            {
                userid = loginid;
                ddlusers = "Dealer";
            }
            else
            {
                userid = allretailer;
                ddlusers = "Retailer";
            }
            //if (ddlusers == "Retailer")
            //{
            //    if (allretailer == "" || allretailer.Contains("All Retailer") || allretailer == null)
            //    {
            //        userid = "ALL";
            //    }
            //    else
            //    {
            //        userid = allretailer;
            //    }
            //}


            if (ddl_status == "Status")
            {
                ddl_status = "ALL";
            }
            //if (api_name == "Select Api Name"  || api_name.Contains("Select Api Name") || api_name == "")
            //{
            //    APIname = "ALL";
            //}
            //else
            //{
            //    APIname = api_name.ToUpper();
            //}
            if (ddl_Type == null)
            {
                ddl_Type = "ALL";

            }

            var respo = db.money_transfer_report_paging(ddlusers, userid, ddl_status, "VASTWEB", Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), ddl_Type, 1, 100000).ToList();

            return new ViewAsPdf(respo);
        }

        //End money transfer report

        //start recharge report
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

        /// <summary>
        /// Returns operator options for a given service type.
        /// </summary>
        [HttpPost]
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
        /// <summary>
        /// Displays the recharge transaction report with date filters.
        /// </summary>
        [HttpGet]
        public ActionResult Recharge_Report()
        {
            string userid = User.Identity.GetUserId();
            ViewBag.alldealer = new SelectList(db.select_Dealer_for_ddl(), "Dealerid", "FarmName", null).ToList();
            ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl(userid), "RetailerId", "Frm_Name", null).ToList();
            var operator_value = db.Operator_Code.Distinct().ToList();
            ViewBag.Operator = new SelectList("", "");

            return View();
        }
        /// <summary>
        /// Displays the recharge transaction report with date filters.
        /// </summary>
        [HttpPost]
        public ActionResult Recharge_Report(string btntype, string txt_frm_date, string txt_to_date, string ddl_status, string Operator, string ddl_top, string txtmob, string allretailer)
        {
            string userid = User.Identity.GetUserId();
            ViewBag.chk = "post";


            ViewBag.alldealer = new SelectList(db.select_Dealer_for_ddl(), "Dealerid", "FarmName", null).ToList();
            ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl(userid), "RetailerId", "Frm_Name", null).ToList();
            var operator_value = db.Operator_Code.Distinct().ToList();
            ViewBag.Operator = new SelectList(operator_value, "new_opt_code", "operator_Name");


            return View();
        }
        /// <summary>
        /// Returns the recharge report partial view.
        /// </summary>
        public ActionResult _Recharge_Report(string txt_frm_date, string txt_to_date, string ddl_status, string Operator, string txtmob, string allretailer)
        {
            string loginuserid = User.Identity.GetUserId();

            if ((txt_frm_date == null || txt_frm_date == "") && (txt_to_date == null || txt_to_date == ""))
            {
                txt_frm_date = DateTime.Now.ToString();
                txt_to_date = DateTime.Now.ToString();

            }
            var userid = "";
            var mobile = "";
            var optname = "";
            string ddlusers = "";

            if (allretailer == "" || allretailer == null)
            {
                userid = loginuserid;
                ddlusers = "Dealer";
            }
            else
            {
                userid = allretailer;
                ddlusers = "Retailer";
            }


            if (Operator == null || Operator == "" || Operator.Contains("ALL OPERATORS"))
            {
                optname = "ALL";
            }
            else
            {
                optname = Operator;
            }
            if (txtmob == null || txtmob == "")
            {
                mobile = "ALL";
            }
            else
            {
                mobile = txtmob;
            }
            if (ddl_status == null)
            {
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

            var ch = db.proc_Recharge_operator_report_newPaging(1, 20, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), ddlusers, userid, ddl_status, optname, mobile, "ALL").ToList();


            return View(ch);
        }
        /// <summary>
        /// Returns paginated recharge records for infinite scroll.
        /// </summary>
        public ActionResult InfiniteScrollRechargeReport(int pageindex, string txt_frm_date, string txt_to_date, string ddl_status, string Operator, string txtmob, string allretailer)
        {
            string loginuserid = User.Identity.GetUserId();


            var userid = "";
            var mobile = "";
            var optname = "";
            string ddlusers = "";

            if (allretailer == "" || allretailer == null)
            {
                userid = loginuserid;
                ddlusers = "Dealer";
            }
            else
            {
                userid = allretailer;
                ddlusers = "Retailer";
            }


            if (Operator == null || Operator == "" || Operator.Contains("ALL OPERATORS"))
            {
                optname = "ALL";
            }
            else
            {
                optname = Operator;
            }
            if (txtmob == null || txtmob == "")
            {
                mobile = "ALL";
            }
            else
            {
                mobile = txtmob;
            }
            if (ddl_status == null)
            {
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
            int pagesize = 20;
            var tbrow = db.proc_Recharge_operator_report_newPaging(pageindex, pagesize, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), ddlusers, userid, ddl_status, optname, mobile, "ALL").ToList();
            JsonModel jsonmodel = new JsonModel();
            jsonmodel.NoMoredata = tbrow.Count < pagesize;
            jsonmodel.HTMLString = renderPartialViewtostring("_Recharge_Report", tbrow);
            return Json(jsonmodel);
        }
        /// <summary>
        /// Displays the total recharge summary statistics.
        /// </summary>
        public ActionResult TotalRecharge_Report(string txt_frm_date, string txt_to_date, string ddl_status, string Operator, string txtmob, string allretailer)
        {
            string loginuserid = User.Identity.GetUserId();


            var userid = "";
            var mobile = "";
            var optname = "";
            string ddlusers = "";

            if (allretailer == "" || allretailer == null)
            {
                userid = loginuserid;
                ddlusers = "Dealer";
            }
            else
            {
                userid = allretailer;
                ddlusers = "Retailer";
            }
            //if (ddlusers == "Retailer")
            //{
            //    if (allretailer == "" || allretailer.Contains("All Retailer") || allretailer == null)
            //    {
            //        userid = "ALL";
            //    }
            //    else
            //    {
            //        userid = allretailer;
            //    }
            //}

            if (Operator == null || Operator == "" || Operator.Contains("ALL OPERATORS"))
            {
                optname = "ALL";
            }
            else
            {
                optname = Operator;
            }
            if (txtmob == null || txtmob == "")
            {
                mobile = "ALL";
            }
            else
            {
                mobile = txtmob;
            }
            if (ddl_status == null)
            {
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

            var ch = db.proc_Recharge_operator_report_newPaging(1, 100000, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), ddlusers, userid, ddl_status, optname, mobile, "ALL").ToList();
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


            };

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public virtual ActionResult ExcelRechargereport(string txt_frm_date, string txt_to_date, string ddl_status, string Operator, string txtmob, string allretailer)
        {
            string loginuserid = User.Identity.GetUserId();

            if ((txt_frm_date == null || txt_frm_date == "") && (txt_to_date == null || txt_to_date == ""))
            {
                txt_frm_date = DateTime.Now.ToString();
                txt_to_date = DateTime.Now.ToString();

            }
            var userid = "";
            var mobile = "";
            var optname = "";
            string ddlusers = "";

            if (allretailer == "" || allretailer == null)
            {
                userid = loginuserid;
                ddlusers = "Dealer";
            }
            else
            {
                userid = allretailer;
                ddlusers = "Retailer";
            }


            if (Operator == null || Operator == "" || Operator.Contains("ALL OPERATORS"))
            {
                optname = "ALL";
            }
            else
            {
                optname = Operator;
            }
            if (txtmob == null || txtmob == "")
            {
                mobile = "ALL";
            }
            else
            {
                mobile = txtmob;
            }
            if (ddl_status == null)
            {
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



            DataTable dt2 = new DataTable();
            dt2.Columns.Add("Status", typeof(string));
            dt2.Columns.Add("Consumer No", typeof(string));

            dt2.Columns.Add("Amount", typeof(string));
            dt2.Columns.Add("Operator", typeof(string));
            dt2.Columns.Add("Opt Type", typeof(string));
            dt2.Columns.Add("Distributor Income", typeof(string));
            dt2.Columns.Add("Retailer Income", typeof(string));
            dt2.Columns.Add("OperatorID", typeof(string));
            dt2.Columns.Add("Date", typeof(string));





            var respo = db.proc_Recharge_operator_report_newPaging(1, 100000, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), ddlusers, userid, ddl_status, optname, mobile, "ALL").ToList();
            foreach (var item in respo)
            {

                dt2.Rows.Add(item.rstatus + "-" + item.frm_name, item.mobile, item.amount, item.Operator_name, item.operator_type, item.dealerincome, item.income, item.opt_id,
                 item.Rch_time);
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
        /// <summary>
        /// Generates the recharge report as a PDF.
        /// </summary>
        public ActionResult PDFRecharge_Report(string txt_frm_date, string txt_to_date, string ddl_status, string Operator, string txtmob, string allretailer)
        {
            string loginuserid = User.Identity.GetUserId();


            var userid = "";
            var mobile = "";
            var optname = "";
            string ddlusers = "";

            if (allretailer == "" || allretailer == null)
            {
                userid = loginuserid;
                ddlusers = "Dealer";
            }
            else
            {
                userid = allretailer;
                ddlusers = "Retailer";
            }


            if (Operator == null || Operator == "" || Operator.Contains("ALL OPERATORS"))
            {
                optname = "ALL";
            }
            else
            {
                optname = Operator;
            }
            if (txtmob == null || txtmob == "")
            {
                mobile = "ALL";
            }
            else
            {
                mobile = txtmob;
            }
            if (ddl_status == null)
            {
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

            var respo = db.proc_Recharge_operator_report_newPaging(1, 1000000, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), ddlusers, userid, ddl_status, optname, mobile, "ALL").ToList();

            return new ViewAsPdf(respo);
        }


        //HotelReport
        /// <summary>
        /// Displays the hotel booking report with date filters.
        /// </summary>
        [HttpGet]
        public ActionResult Hotel_Report()
        {

            string userid = User.Identity.GetUserId();
            //show all retailer 
            ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl(userid), "RetailerId", "Frm_Name", null).ToList();

            return View();
        }
        /// <summary>
        /// Displays the hotel booking report with date filters.
        /// </summary>
        [HttpPost]
        public ActionResult Hotel_Report(string ddl_status, int? ddl_top, string txt_frm_date, string txt_to_date, string allretailer, string TXNID)
        {
            ViewBag.chk = "post";

            string userid = User.Identity.GetUserId();
            //show all retailer 
            ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl(userid), "RetailerId", "Frm_Name", null).ToList();

            return View();
        }
        /// <summary>
        /// Returns the hotel booking report partial view.
        /// </summary>
        public ActionResult _Hotel_Report(string ddl_status, int? ddl_top, string txt_frm_date, string txt_to_date, string allretailer, string TXNID)
        {
            ViewBag.chk = "post";
            string retailerid = null;
            string userid = User.Identity.GetUserId();
            //show all retailer 
            if ((txt_frm_date == null || txt_frm_date == "") && (txt_to_date == null || txt_to_date == ""))
            {
                txt_frm_date = DateTime.Now.ToString();
                txt_to_date = DateTime.Now.ToString();

            }

            if (allretailer == "" || allretailer == null)
            {
                retailerid = null;
            }
            else
            {
                retailerid = allretailer;
                userid = null;
            }
            TXNID = string.IsNullOrWhiteSpace(TXNID) ? null : TXNID;
            ddl_status = string.IsNullOrWhiteSpace(ddl_status) ? null : ddl_status;

            DateTime frm = Convert.ToDateTime(txt_frm_date);
            DateTime to = Convert.ToDateTime(txt_to_date);
            txt_frm_date = frm.ToString("dd-MM-yyyy");
            txt_to_date = to.ToString("dd-MM-yyyy");
            string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
            DateTime dt = DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            DateTime dt1 = DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            DateTime frm_date = dt.Date;
            DateTime to_date = dt1.AddDays(1);
            var proc_Response = db.proc_HotelReport(1, 20, ddl_status, retailerid, userid, null, null, null, TXNID, null, null, frm_date, to_date).ToList();

            return View(proc_Response);
        }
        /// <summary>
        /// Returns paginated hotel booking records for infinite scroll.
        /// </summary>
        public ActionResult InfiniteScrollHotelReport(int pageindex, string ddl_status, int? ddl_top, string txt_frm_date, string txt_to_date, string allretailer, string TXNID)
        {
            ViewBag.chk = "post";
            string retailerid = null;
            string userid = User.Identity.GetUserId();
            //show all retailer 
            if ((txt_frm_date == null || txt_frm_date == "") && (txt_to_date == null || txt_to_date == ""))
            {
                txt_frm_date = DateTime.Now.ToString();
                txt_to_date = DateTime.Now.ToString();

            }

            if (allretailer == "" || allretailer == null)
            {
                retailerid = null;
            }
            else
            {
                retailerid = allretailer;
                userid = null;
            }
            TXNID = string.IsNullOrWhiteSpace(TXNID) ? null : TXNID;
            ddl_status = string.IsNullOrWhiteSpace(ddl_status) ? null : ddl_status;

            DateTime frm = Convert.ToDateTime(txt_frm_date);
            DateTime to = Convert.ToDateTime(txt_to_date);
            txt_frm_date = frm.ToString("dd-MM-yyyy");
            txt_to_date = to.ToString("dd-MM-yyyy");
            string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
            DateTime dt = DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            DateTime dt1 = DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            DateTime frm_date = dt.Date;
            DateTime to_date = dt1.AddDays(1);
            int pagesize = 20;
            var tbrow = db.proc_HotelReport(pageindex, pagesize, ddl_status, retailerid, userid, null, null, null, TXNID, null, null, frm_date, to_date).ToList();
            JsonModel jsonmodel = new JsonModel();
            jsonmodel.NoMoredata = tbrow.Count < pagesize;
            jsonmodel.HTMLString = renderPartialViewtostring("_Hotel_Report", tbrow);
            return Json(jsonmodel);
        }
        /// <summary>
        /// Displays total hotel booking summary statistics.
        /// </summary>
        public ActionResult Total_HotelReport(string ddl_status, string txt_frm_date, string txt_to_date, string allretailer, string TXNID)
        {
            ViewBag.chk = "post";
            string retailerid = null;
            string userid = User.Identity.GetUserId();
            //show all retailer 
            if ((txt_frm_date == null || txt_frm_date == "") && (txt_to_date == null || txt_to_date == ""))
            {
                txt_frm_date = DateTime.Now.ToString();
                txt_to_date = DateTime.Now.ToString();

            }

            if (allretailer == "" || allretailer == null)
            {
                retailerid = null;
            }
            else
            {
                retailerid = allretailer;
                userid = null;
            }
            TXNID = string.IsNullOrWhiteSpace(TXNID) ? null : TXNID;
            ddl_status = string.IsNullOrWhiteSpace(ddl_status) ? null : ddl_status;

            DateTime frm = Convert.ToDateTime(txt_frm_date);
            DateTime to = Convert.ToDateTime(txt_to_date);
            txt_frm_date = frm.ToString("dd-MM-yyyy");
            txt_to_date = to.ToString("dd-MM-yyyy");
            string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
            DateTime dt = DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            DateTime dt1 = DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            DateTime frm_date = dt.Date;
            DateTime to_date = dt1.AddDays(1);

            var proc_Response = db.proc_HotelReport(1, 20, ddl_status, retailerid, userid, null, null, null, TXNID, null, null, frm_date, to_date).ToList();
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
        /// <summary>
        /// Exports the hotel booking report to Excel.
        /// </summary>
        public ActionResult ExcelHotalReport(string ddl_status, string txt_frm_date, string txt_to_date, string allretailer, string TXNID)
        {

            ViewBag.chk = "post";
            string retailerid = null;
            string userid = User.Identity.GetUserId();
            //show all retailer 
            if ((txt_frm_date == null || txt_frm_date == "") && (txt_to_date == null || txt_to_date == ""))
            {
                txt_frm_date = DateTime.Now.ToString();
                txt_to_date = DateTime.Now.ToString();

            }

            if (allretailer == "" || allretailer == null)
            {
                retailerid = null;
            }
            else
            {
                retailerid = allretailer;
                userid = null;
            }
            TXNID = string.IsNullOrWhiteSpace(TXNID) ? null : TXNID;
            ddl_status = string.IsNullOrWhiteSpace(ddl_status) ? null : ddl_status;

            DateTime frm = Convert.ToDateTime(txt_frm_date);
            DateTime to = Convert.ToDateTime(txt_to_date);
            txt_frm_date = frm.ToString("dd-MM-yyyy");
            txt_to_date = to.ToString("dd-MM-yyyy");
            string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
            DateTime dt = DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            DateTime dt1 = DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            DateTime frm_date = dt.Date;
            DateTime to_date = dt1.AddDays(1);

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

            dt2.Columns.Add("Booking Date", typeof(string));

            var respo = db.proc_HotelReport(1, 100000, ddl_status, retailerid, userid, null, null, null, TXNID, null, null, frm_date, to_date).ToList();
            foreach (var item in respo)
            {

                dt2.Rows.Add(item.TicketStatus + "" + item.Frm_Name, item.HotelName, item.NoOfRooms,
                    item.totalOfferedFare,
             item.BookingId, item.reminc, item.dlmpre, item.dlminc, item.dlmpost,
             item.ticketdate);
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
        /// <summary>
        /// Generates the hotel booking report as a PDF.
        /// </summary>
        public ActionResult PDFHotelReport(string ddl_status, string txt_frm_date, string txt_to_date, string allretailer, string TXNID)
        {

            ViewBag.chk = "post";
            string retailerid = null;
            string userid = User.Identity.GetUserId();
            //show all retailer 
            if ((txt_frm_date == null || txt_frm_date == "") && (txt_to_date == null || txt_to_date == ""))
            {
                txt_frm_date = DateTime.Now.ToString();
                txt_to_date = DateTime.Now.ToString();

            }

            if (allretailer == "" || allretailer == null)
            {
                retailerid = null;
            }
            else
            {
                retailerid = allretailer;
                userid = null;
            }
            TXNID = string.IsNullOrWhiteSpace(TXNID) ? null : TXNID;
            ddl_status = string.IsNullOrWhiteSpace(ddl_status) ? null : ddl_status;

            DateTime frm = Convert.ToDateTime(txt_frm_date);
            DateTime to = Convert.ToDateTime(txt_to_date);
            txt_frm_date = frm.ToString("dd-MM-yyyy");
            txt_to_date = to.ToString("dd-MM-yyyy");
            string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
            DateTime dt = DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            DateTime dt1 = DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            DateTime frm_date = dt.Date;
            DateTime to_date = dt1.AddDays(1);

            var respo = db.proc_HotelReport(1, 20, ddl_status, retailerid, userid, null, null, null, TXNID, null, null, frm_date, to_date).ToList();

            return new ViewAsPdf(respo);
        }
        /// <summary>
        /// Displays and manages hotel/flight cancellation queue.
        /// </summary>
        [HttpGet]
        public ActionResult CancellationQueue()
        {
            var userid = User.Identity.GetUserId();
            //show all retailer 
            ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl(userid), "RetailerId", "RetailerName", null).ToList();

            string txt_frm_date = DateTime.Now.ToString();
            string txt_to_date = DateTime.Now.ToString();
            var frm_date = Convert.ToDateTime(txt_frm_date).Date;
            var to_date = Convert.ToDateTime(txt_to_date).AddDays(1);
            var proc_Response = db.proc_HotelCancellationReport(50, null, null, userid, null, null, null, null, null, null, frm_date, to_date).ToList();
            //ViewData["Totalofferfare"] = proc_Response.Where(s => s.TicketStatus.ToUpper().Contains("Success")).Sum(s => Convert.ToInt32(s.totalOfferedFare));
            //ViewData["totalf"] = proc_Response.Where(s => s.TicketStatus.ToUpper().Contains("FAILED")).Sum(s => Convert.ToInt32(s.totalOfferedFare));
            //ViewData["totalp"] = proc_Response.Where(s => s.TicketStatus.ToUpper().Contains("Proccessed")).Sum(s => Convert.ToInt32(s.totalOfferedFare));
            return View(proc_Response);
        }
        /// <summary>
        /// Displays and manages hotel/flight cancellation queue.
        /// </summary>
        [HttpPost]
        public ActionResult CancellationQueue(string ddl_status, int? ddl_top, string txt_frm_date, string txt_to_date, string allretailer, string BookingId)
        {
            ViewBag.chk = "post";
            string retailerid = null;
            var userid = User.Identity.GetUserId();
            //show all retailer 
            ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl(userid), "RetailerId", "RetailerName", null).ToList();
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
                ddl_top = 1000000;
            }
            BookingId = string.IsNullOrWhiteSpace(BookingId) ? null : BookingId;
            ddl_status = string.IsNullOrWhiteSpace(ddl_status) ? null : ddl_status;


            if (allretailer == "" || allretailer.Contains("All Retailer") || allretailer == null)
            {
                retailerid = null;
            }
            else
            {
                retailerid = allretailer;
            }

            var proc_Response = db.proc_HotelCancellationReport(ddl_top, ddl_status, retailerid, userid, null, null, null, BookingId, null, null, frm_date, to_date).ToList();
            //ViewData["Totalofferfare"] = proc_Response.Where(s => s.TicketStatus.ToUpper().Contains("Success")).Sum(s => Convert.ToInt32(s.totalOfferedFare));
            //ViewData["totalf"] = proc_Response.Where(s => s.TicketStatus.ToUpper().Contains("FAILED")).Sum(s => Convert.ToInt32(s.totalOfferedFare));
            //ViewData["totalp"] = proc_Response.Where(s => s.TicketStatus.ToUpper().Contains("Proccessed")).Sum(s => Convert.ToInt32(s.totalOfferedFare));
            return View(proc_Response);
        }

        //BusReport
        /// <summary>
        /// Displays the bus booking report with date filters.
        /// </summary>
        [HttpGet]
        public ActionResult Bus_Report()
        {
            var userid = User.Identity.GetUserId();
            //show all retailer 
            ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl(userid), "RetailerId", "Frm_Name", null).ToList();
            return View();
        }
        /// <summary>
        /// Displays the bus booking report with date filters.
        /// </summary>
        [HttpPost]
        public ActionResult Bus_Report(string ddl_status, int? ddl_top, string txt_frm_date, string txt_to_date, string allretailer, string allwhitelabel, string TicketNo)
        {
            ViewBag.chk = "post";

            var userid = User.Identity.GetUserId();
            //show all retailer 
            ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl(userid), "RetailerId", "Frm_Name", null).ToList();

            return View();
        }
        /// <summary>
        /// Returns the bus booking report partial view.
        /// </summary>
        public ActionResult _Bus_Report(string ddl_status, string txt_frm_date, string txt_to_date, string allretailer, string TicketNo)
        {

            if ((txt_frm_date == null || txt_frm_date == "") && (txt_to_date == null || txt_to_date == ""))
            {
                txt_frm_date = DateTime.Now.ToString();
                txt_to_date = DateTime.Now.ToString();

            }

            string retailerid = null;
            var userid = User.Identity.GetUserId();
            //show all retailer 

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

            if (allretailer == "" || allretailer == null)
            {
                retailerid = null;

            }
            else
            {
                retailerid = allretailer;
                userid = null;
            }


            var proc_Response = db.proc_BusReport(1, 20, ddl_status, retailerid, userid, null, null, TicketNo, null, null, null, frm_date, to_date).ToList();
            return View(proc_Response);
        }
        /// <summary>
        /// Returns paginated bus booking records for infinite scroll.
        /// </summary>
        public ActionResult InfiniteScrollBus(int pageindex, string ddl_status, string txt_frm_date, string txt_to_date, string allretailer, string TicketNo)
        {
            if ((txt_frm_date == null || txt_frm_date == "") && (txt_to_date == null || txt_to_date == ""))
            {
                txt_frm_date = DateTime.Now.ToString();
                txt_to_date = DateTime.Now.ToString();

            }

            string retailerid = null;
            var userid = User.Identity.GetUserId();
            //show all retailer 

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

            if (allretailer == "" || allretailer == null)
            {
                retailerid = null;

            }
            else
            {
                retailerid = allretailer;
                userid = null;
            }
            int pagesize = 20;
            var tbrow = db.proc_BusReport(pageindex, pagesize, ddl_status, retailerid, userid, null, null, TicketNo, null, null, null, frm_date, to_date).ToList();
            JsonModel jsonmodel = new JsonModel();
            jsonmodel.NoMoredata = tbrow.Count < pagesize;
            jsonmodel.HTMLString = renderPartialViewtostring("_Bus_Report", tbrow);
            return Json(jsonmodel);
        }
        /// <summary>
        /// Displays total bus booking summary statistics.
        /// </summary>
        public ActionResult Bus_ReportTotal(string ddl_status, string txt_frm_date, string txt_to_date, string allretailer, string TicketNo)
        {

            string retailerid = null;
            var userid = User.Identity.GetUserId();
            //show all retailer 

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

            if (allretailer == "" || allretailer == null)
            {
                retailerid = null;

            }
            else
            {
                retailerid = allretailer;
                userid = null;
            }
            var proc_Response = db.proc_BusReport(1, 1000000, ddl_status, retailerid, userid, null, null, TicketNo, null, null, null, frm_date, to_date).ToList();
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
        /// <summary>
        /// Exports the bus booking report to Excel.
        /// </summary>
        public ActionResult ExcelBusReport(string ddl_status, string txt_frm_date, string txt_to_date, string allretailer, string TicketNo)
        {

            string retailerid = null;
            var userid = User.Identity.GetUserId();
            //show all retailer 

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

            if (allretailer == "" || allretailer == null)
            {
                retailerid = null;

            }
            else
            {
                retailerid = allretailer;
                userid = null;
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
            dt2.Columns.Add("Journey Date", typeof(string));
            dt2.Columns.Add("Booking Date", typeof(string));

            var respo = db.proc_BusReport(1, 100000, ddl_status, retailerid, userid, null, null, TicketNo, null, null, null, frm_date, to_date).ToList();
            foreach (var item in respo)
            {

                dt2.Rows.Add(item.TicketStatus + "" + item.Frm_Name, item.PassengerName, item.TicketNo,
                    item.PNR,
             item.totalseat, item.FareAmount, item.RemInc,
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
        /// <summary>
        /// Generates the bus booking report as a PDF.
        /// </summary>
        public ActionResult PDFBus_Report(string ddl_status, string txt_frm_date, string txt_to_date, string allretailer, string TicketNo)
        {

            string retailerid = null;
            var userid = User.Identity.GetUserId();
            //show all retailer 

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

            if (allretailer == "" || allretailer == null)
            {
                retailerid = null;

            }
            else
            {
                retailerid = allretailer;
                userid = null;
            }
            var respo = db.proc_BusReport(1, 100000, ddl_status, retailerid, userid, null, null, TicketNo, null, null, null, frm_date, to_date).ToList();
            return new ViewAsPdf(respo);
        }

        //Flightreport
        /// <summary>
        /// Displays the flight booking report with date filters.
        /// </summary>
        [HttpGet]
        public ActionResult Flight_Report()
        {
            var userid = User.Identity.GetUserId();
            //show all retailer 
            ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl(userid), "RetailerId", "Frm_Name", null).ToList();


            return View();
        }
        /// <summary>
        /// Displays the flight booking report with date filters.
        /// </summary>
        [HttpPost]
        public ActionResult Flight_Report(string txt_frm_date, string txt_to_date, int? ddl_top, string ddl_status, string PNR, string allretailer)
        {
            ViewBag.chk = "post";
            var userid = User.Identity.GetUserId();
            //show all retailer 
            ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl(userid), "RetailerId", "Frm_Name", null).ToList();
            ViewBag.chk = "post";



            return View();
        }
        /// <summary>
        /// Returns the flight booking report partial view.
        /// </summary>
        public ActionResult _Flight_Report(string txt_frm_date, string txt_to_date, string ddl_status, string PNR, string allretailer)
        {
            var userid = User.Identity.GetUserId();
            var DealerId = "";
            //show all retailer 
            if (txt_frm_date == null && txt_to_date == null && ddl_status == null)
            {
                txt_frm_date = DateTime.Now.ToString();
                txt_to_date = DateTime.Now.ToString();

            }
            ViewBag.chk = "post";
            string retailerid = null;

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
            ddl_status = string.IsNullOrWhiteSpace(ddl_status) ? null : ddl_status;

            if (allretailer == "" || allretailer == null)
            {
                retailerid = null;
                DealerId = userid;
            }
            else
            {
                retailerid = allretailer;
                DealerId = null;
            }

            var proc_Response = db.proc_FlightReport(1, 20, ddl_status, retailerid, DealerId, null, null, null, PNR, null, null, frm_date, to_date).ToList();

            return View(proc_Response);
        }
        /// <summary>
        /// Returns paginated flight ticket records for infinite scroll.
        /// </summary>
        public ActionResult InfiniteScroll_Ticket(int pageindex, string txt_frm_date, string txt_to_date, string ddl_status, string PNR, string allretailer)
        {
            var userid = User.Identity.GetUserId();
            var DealerId = "";
            //show all retailer 
            if (txt_frm_date == null && txt_to_date == null && ddl_status == null)
            {
                txt_frm_date = DateTime.Now.ToString();
                txt_to_date = DateTime.Now.ToString();

            }
            ViewBag.chk = "post";
            string retailerid = null;

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
            ddl_status = string.IsNullOrWhiteSpace(ddl_status) ? null : ddl_status;

            if (allretailer == "" || allretailer == null)
            {
                retailerid = null;
                DealerId = userid;
            }
            else
            {
                retailerid = allretailer;
                DealerId = null;
            }
            int pagesize = 20;
            var tbrow = db.proc_FlightReport(pageindex, pagesize, ddl_status, retailerid, DealerId, null, null, null, PNR, null, null, frm_date, to_date).ToList();
            JsonModel jsonmodel = new JsonModel();
            jsonmodel.NoMoredata = tbrow.Count < pagesize;
            jsonmodel.HTMLString = renderPartialViewtostring("_Flight_Report", tbrow);
            return Json(jsonmodel);
        }
        /// <summary>
        /// Displays total flight booking summary statistics.
        /// </summary>
        public ActionResult Flight_Total(string txt_frm_date, string txt_to_date, string ddl_status, string PNR, string allretailer)
        {

            var userid = User.Identity.GetUserId();
            var DealerId = "";
            //show all retailer 
            if (txt_frm_date == null && txt_to_date == null && ddl_status == null)
            {
                txt_frm_date = DateTime.Now.ToString();
                txt_to_date = DateTime.Now.ToString();

            }
            ViewBag.chk = "post";
            string retailerid = null;

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
            ddl_status = string.IsNullOrWhiteSpace(ddl_status) ? null : ddl_status;

            if (allretailer == "" || allretailer == null)
            {
                retailerid = null;
                DealerId = userid;
            }
            else
            {
                retailerid = allretailer;
                DealerId = null;
            }
            var proc_Response = db.proc_FlightReport(1, 100000, ddl_status, retailerid, DealerId, null, null, null, PNR, null, null, frm_date, to_date).ToList();
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
        /// <summary>
        /// Exports the flight booking report to Excel.
        /// </summary>
        public ActionResult ExcelFilghtReport(string txt_frm_date, string txt_to_date, string ddl_status, string PNR, string allretailer)
        {
            var userid = User.Identity.GetUserId();
            var DealerId = "";
            //show all retailer 
            if (txt_frm_date == null && txt_to_date == null && ddl_status == null)
            {
                txt_frm_date = DateTime.Now.ToString();
                txt_to_date = DateTime.Now.ToString();

            }
            ViewBag.chk = "post";
            string retailerid = null;

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
            ddl_status = string.IsNullOrWhiteSpace(ddl_status) ? null : ddl_status;

            if (allretailer == "" || allretailer == null)
            {
                retailerid = null;
                DealerId = userid;
            }
            else
            {
                retailerid = allretailer;
                DealerId = null;
            }

            DataTable dt2 = new DataTable();
            dt2.Columns.Add("Status", typeof(string));
            dt2.Columns.Add("PassangerName", typeof(string));
            dt2.Columns.Add("Flight", typeof(string));
            dt2.Columns.Add("Booking Id", typeof(string));
            dt2.Columns.Add("PNR ", typeof(string));
            dt2.Columns.Add("Fare Amount", typeof(string));
            dt2.Columns.Add("MY Income", typeof(string));
            dt2.Columns.Add("Booking Date", typeof(string));

            var respo = db.proc_FlightReport(1, 100000, ddl_status, retailerid, DealerId, null, null, null, PNR, null, null, frm_date, to_date).ToList();
            foreach (var item in respo)
            {
                var sts = item.TicketStatus;

                if (item.TicketStatus.Contains("Refund"))
                {
                    sts = "Refund";
                }
                dt2.Rows.Add(sts + "" + item.Frm_Name, item.LeadPaxFirstName, item.AirlineName,
                    item.BookingId, item.PNR,
                item.OfferedFare, item.DlmInc,
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
        /// <summary>
        /// Generates the flight booking report as a PDF.
        /// </summary>
        public ActionResult PDF_FilghtReport(string txt_frm_date, string txt_to_date, string ddl_status, string PNR, string allretailer)
        {

            var userid = User.Identity.GetUserId();
            var DealerId = "";
            //show all retailer 

            ViewBag.chk = "post";
            string retailerid = null;

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
            ddl_status = string.IsNullOrWhiteSpace(ddl_status) ? null : ddl_status;

            if (allretailer == "" || allretailer == null)
            {
                retailerid = null;
                DealerId = userid;
            }
            else
            {
                retailerid = allretailer;
                DealerId = null;
            }
            var respo = db.proc_FlightReport(1, 10000, ddl_status, retailerid, DealerId, null, null, null, PNR, null, null, frm_date, to_date).ToList();
            return new ViewAsPdf(respo);



        }
        /// <summary>
        /// Displays the cancellation report for bookings.
        /// </summary>
        [HttpGet]
        public ActionResult CancellationReport()
        {
            var userid = User.Identity.GetUserId();
            //show all retailer 
            ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl(userid), "RetailerId", "RetailerName", null).ToList();

            string txt_frm_date = DateTime.Now.ToString();
            string txt_to_date = DateTime.Now.ToString();
            var frm_date = Convert.ToDateTime(txt_frm_date).Date;
            var to_date = Convert.ToDateTime(txt_to_date).AddDays(1);
            var procRespo = db.proc_FlightCancellationReport(1, 20, null, null, userid, null, null, null, null, frm_date, to_date).ToList();

            return View(procRespo);
        }
        /// <summary>
        /// Displays the cancellation report for bookings.
        /// </summary>
        public ActionResult CancellationReport(string txt_frm_date, string txt_to_date, int? ddl_top, string ddl_status, string PNR, string allretailer)
        {
            var userid = User.Identity.GetUserId();
            ViewBag.chk = "post";
            string retailerid = null;
            //show all retailer 
            ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl(userid), "RetailerId", "RetailerName", null).ToList();
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
            ddl_status = string.IsNullOrWhiteSpace(ddl_status) ? null : ddl_status;

            if (allretailer == "" || allretailer.Contains("All Retailer") || allretailer == null)
            {
                retailerid = null;
            }
            else
            {
                retailerid = allretailer;
            }

            var procRespo = db.proc_FlightCancellationReport(1, 20, ddl_status, retailerid, userid, null, null, null, PNR, frm_date, to_date).ToList();
            return View(procRespo);
        }


        //m_poss
        /// <summary>
        /// Displays the mPOS transaction report.
        /// </summary>
        [HttpGet]
        public ActionResult m_possreport()
        {
            var userid = User.Identity.GetUserId();
            //show all retailer 
            ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl(userid), "RetailerId", "Frm_Name", null).ToList();

            return View();


        }
        /// <summary>
        /// Displays the mPOS transaction report.
        /// </summary>
        [HttpPost]
        public ActionResult m_possreport(string ddl_status, string txt_frm_date, string txt_to_date, string allretailer)
        {
            ViewBag.chk = "post";
            var userid = User.Identity.GetUserId();
            //show all retailer 
            ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl(userid), "RetailerId", "Frm_Name", null).ToList();

            return View();
        }
        /// <summary>
        /// Returns the mPOS report partial view.
        /// </summary>
        public ActionResult _m_possreport(string ddl_status, string txt_frm_date, string txt_to_date, string allretailer)
        {

            var userid = "";
            //show all retailer 
            if (txt_frm_date == null && txt_to_date == null)
            {

                txt_frm_date = DateTime.Now.ToString();
                txt_to_date = DateTime.Now.ToString();

            }
            var loginid = User.Identity.GetUserId();
            //show all retailer 
            ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl(loginid), "RetailerId", "Frm_Name", null).ToList();
            ViewBag.chk = "post";
            string ddlusers = "";


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

            if (allretailer == "" || allretailer == null)
            {
                userid = loginid;
                ddlusers = "Dealer";
            }
            else
            {
                userid = allretailer;
                ddlusers = "Retailer";
            }
            if (ddl_status == null)
            {
                ddl_status = "ALL";
            }

            int pagesize = 20;

            var ch = db.Mpos_Report_New_paging(1, pagesize, ddlusers, userid, ddl_status, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            return View(ch);
        }
        /// <summary>
        /// Returns paginated mPOS transaction records for infinite scroll.
        /// </summary>
        public ActionResult InfiniteScroll_mpos(int pageindex, string ddl_status, string txt_frm_date, string txt_to_date, string allretailer)
        {
            var userid = "";
            //show all retailer 
            if (txt_frm_date == null && txt_to_date == null)
            {

                txt_frm_date = DateTime.Now.ToString();
                txt_to_date = DateTime.Now.ToString();

            }
            var loginid = User.Identity.GetUserId();
            //show all retailer 
            ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl(loginid), "RetailerId", "Frm_Name", null).ToList();
            ViewBag.chk = "post";
            string ddlusers = "";


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

            if (allretailer == "" || allretailer == null)
            {
                userid = loginid;
                ddlusers = "Dealer";
            }
            else
            {
                userid = allretailer;
                ddlusers = "Retailer";
            }
            if (ddl_status == null)
            {
                ddl_status = "ALL";
            }
            int pagesize = 20;

            var tbrow = db.Mpos_Report_New_paging(pageindex, pagesize, ddlusers, userid, ddl_status, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            JsonModel jsonmodel = new JsonModel();
            jsonmodel.NoMoredata = tbrow.Count < pagesize;
            jsonmodel.HTMLString = renderPartialViewtostring("_m_possreport", tbrow);
            return Json(jsonmodel);
        }
        /// <summary>
        /// Displays the mPOS transaction summary totals.
        /// </summary>
        public ActionResult mpos_Total(string ddl_status, string txt_frm_date, string txt_to_date, string allretailer)
        {

            var userid = "";
            //show all retailer 
            if (txt_frm_date == null && txt_to_date == null)
            {

                txt_frm_date = DateTime.Now.ToString();
                txt_to_date = DateTime.Now.ToString();

            }
            var loginid = User.Identity.GetUserId();
            //show all retailer 
            ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl(loginid), "RetailerId", "Frm_Name", null).ToList();
            ViewBag.chk = "post";
            string ddlusers = "";


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

            if (allretailer == "" || allretailer == null)
            {
                userid = loginid;
                ddlusers = "Dealer";
            }
            else
            {
                userid = allretailer;
                ddlusers = "Retailer";
            }
            if (ddl_status == null)
            {
                ddl_status = "ALL";
            }


            var ch = db.Mpos_Report_New_paging(1, 1000000, ddlusers, userid, ddl_status, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            var successtotal1 = ch.Where(s => s.status == "00").Sum(s => Convert.ToInt32(s.amount));
            var Failedtotal1 = ch.Where(s => s.status == "22").Sum(s => Convert.ToInt32(s.amount));
            var data = new
            {
                success = successtotal1,
                failed = Failedtotal1,

            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Exports the mPOS transaction report to Excel.
        /// </summary>
        public ActionResult ExcelRechargereport_mpos(string ddl_status, string txt_frm_date, string txt_to_date, string allretailer)
        {
            var userid = "";
            //show all retailer 
            if (txt_frm_date == null && txt_to_date == null)
            {

                txt_frm_date = DateTime.Now.ToString();
                txt_to_date = DateTime.Now.ToString();

            }
            var loginid = User.Identity.GetUserId();
            //show all retailer 
            ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl(loginid), "RetailerId", "Frm_Name", null).ToList();
            ViewBag.chk = "post";
            string ddlusers = "";


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

            if (allretailer == "" || allretailer == null)
            {
                userid = loginid;
                ddlusers = "Dealer";
            }
            else
            {
                userid = allretailer;
                ddlusers = "Retailer";
            }
            if (ddl_status == null)
            {
                ddl_status = "ALL";
            }


            DataTable dt2 = new DataTable();
            dt2.Columns.Add("Status", typeof(string));

            dt2.Columns.Add("BankRRN No", typeof(string));
            dt2.Columns.Add("TxnId", typeof(string));
            dt2.Columns.Add("TransType", typeof(string));
            dt2.Columns.Add("Amount", typeof(string));
            dt2.Columns.Add("User Comm", typeof(string));
            dt2.Columns.Add("MY Comm", typeof(string));
            dt2.Columns.Add("Debit Amount", typeof(string));
            dt2.Columns.Add("Merchant ID", typeof(string));
            dt2.Columns.Add("Bank MerchantID", typeof(string));
            dt2.Columns.Add("Bank TerminalID", typeof(string));
            dt2.Columns.Add("Date", typeof(string));


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
                dt2.Rows.Add(sts + "=" + item.Frm_Name, item.BankRRN, item.TxnId, item.transType, item.amount, item.Rem_comm, item.dlmincome, item.totalamount,
                item.merchatID, item.bankmerchantID, item.bankterminalID, item.date);
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
        /// <summary>
        /// Generates the mPOS transaction report as a PDF.
        /// </summary>
        public ActionResult PDFMposReport(string ddl_status, string txt_frm_date, string txt_to_date, string allretailer)
        {
            var userid = "";
            //show all retailer 
            if (txt_frm_date == null && txt_to_date == null)
            {

                txt_frm_date = DateTime.Now.ToString();
                txt_to_date = DateTime.Now.ToString();

            }
            var loginid = User.Identity.GetUserId();
            //show all retailer 
            ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl(loginid), "RetailerId", "Frm_Name", null).ToList();
            ViewBag.chk = "post";
            string ddlusers = "";


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

            if (allretailer == "" || allretailer == null)
            {
                userid = loginid;
                ddlusers = "Dealer";
            }
            else
            {
                userid = allretailer;
                ddlusers = "Retailer";
            }
            if (ddl_status == null)
            {
                ddl_status = "ALL";
            }



            System.Threading.Thread.Sleep(1000);

            var respo = db.Mpos_Report_New_paging(1, 100000, ddlusers, userid, ddl_status, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            return new ViewAsPdf(respo);
        }


        /// <summary>
        /// Displays the AEPS (Aadhaar Enabled Payment) transaction report.
        /// </summary>
        [HttpGet]
        public ActionResult AepsReport()
        {
            var userid = User.Identity.GetUserId();
            //show all retailer 
            ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl(userid), "RetailerId", "Frm_Name", null).ToList();


            return View();
        }
        /// <summary>
        /// Displays the AEPS (Aadhaar Enabled Payment) transaction report.
        /// </summary>
        [HttpPost]
        public ActionResult AepsReport(string txt_frm_date, string txt_to_date, string allretailer, string ddl_status)
        {
            var loginid = User.Identity.GetUserId();
            //show all retailer 
            ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl(loginid), "RetailerId", "Frm_Name", null).ToList();
            ViewBag.chk = "post";


            return View();
        }
        /// <summary>
        /// Returns the AEPS report partial view.
        /// </summary>
        public ActionResult _AepsReport(string txt_frm_date, string txt_to_date, string allretailer, string ddl_status)
        {
            var loginid = User.Identity.GetUserId();
            //show all retailer 
            if (txt_frm_date == null && txt_to_date == null && ddl_status == null)
            {
                txt_frm_date = DateTime.Now.ToString();
                txt_to_date = DateTime.Now.ToString();

            }
            string ddlusers = "";
            string userid = "";

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

            if (allretailer == "" || allretailer == null)
            {
                userid = loginid;
                ddlusers = "Dealer";
            }
            else
            {
                userid = allretailer;
                ddlusers = "Retailer";
            }
            if (ddl_status == null)
            {
                ddl_status = "ALL";

            }


            var ch = db.Aeps_Report_New(ddlusers, userid, ddl_status, 0, "", "", "", "ALL", Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), 1, 20).ToList();


            return View(ch);
        }
        /// <summary>
        /// Returns paginated AEPS transaction records for infinite scroll.
        /// </summary>
        public ActionResult InfiniteScrollAeps(int pageindex, string txt_frm_date, string txt_to_date, string allretailer, string ddl_status)
        {
            var loginid = User.Identity.GetUserId();
            //show all retailer 
            if (txt_frm_date == null && txt_to_date == null && ddl_status == null)
            {
                txt_frm_date = DateTime.Now.ToString();
                txt_to_date = DateTime.Now.ToString();

            }
            string ddlusers = "";
            string userid = "";

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

            if (allretailer == "" || allretailer == null)
            {
                userid = loginid;
                ddlusers = "Dealer";
            }
            else
            {
                userid = allretailer;
                ddlusers = "Retailer";
            }
            if (ddl_status == null)
            {
                ddl_status = "ALL";

            }

            int pagesize = 20;
            var tbrow = db.Aeps_Report_New(ddlusers, userid, ddl_status, 0, "", "", "", "ALL", Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), pageindex, pagesize).ToList();


            JsonModel jsonmodel = new JsonModel();
            jsonmodel.NoMoredata = tbrow.Count < pagesize;
            jsonmodel.HTMLString = renderPartialViewtostring("_AepsReport", tbrow);
            return Json(jsonmodel);
        }
        /// <summary>
        /// Displays the AEPS transaction summary totals.
        /// </summary>
        public ActionResult TotalAepsreport(string txt_frm_date, string txt_to_date, string allretailer, string ddl_status)
        {
            var loginid = User.Identity.GetUserId();
            //show all retailer 
            if (txt_frm_date == null && txt_to_date == null && ddl_status == null)
            {
                txt_frm_date = DateTime.Now.ToString();
                txt_to_date = DateTime.Now.ToString();

            }
            string ddlusers = "";
            string userid = "";

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

            if (allretailer == "" || allretailer == null)
            {
                userid = loginid;
                ddlusers = "Dealer";
            }
            else
            {
                userid = allretailer;
                ddlusers = "Retailer";
            }
            if (ddl_status == null)
            {
                ddl_status = "ALL";

            }


            var ch = db.Aeps_Report_New(ddlusers, userid, ddl_status, 0, "", "", "", "ALL", Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), 1, 1000000).ToList();

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
        /// <summary>
        /// Exports the AEPS transaction report to Excel.
        /// </summary>
        public ActionResult ExcelAepsreport(string txt_frm_date, string txt_to_date, string allretailer, string ddl_status)
        {
            var loginid = User.Identity.GetUserId();
            //show all retailer 
            if (txt_frm_date == null && txt_to_date == null && ddl_status == null)
            {
                txt_frm_date = DateTime.Now.ToString();
                txt_to_date = DateTime.Now.ToString();

            }
            string ddlusers = "";
            string userid = "";

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

            if (allretailer == "" || allretailer == null)
            {
                userid = loginid;
                ddlusers = "Dealer";
            }
            else
            {
                userid = allretailer;
                ddlusers = "Retailer";
            }
            if (ddl_status == null)
            {
                ddl_status = "ALL";

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
                        var first = new String('x', 5);
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
                    dt2.Rows.Add(item.Status + "=" + item.Frm_Name, item.MerchantTxnId, maskedaadhar, item.BankName, item.Type, item.Amount, usercredit, item.DlmPost,
                     item.BankRRN, item.Txn_Date);
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
        /// <summary>
        /// Generates the AEPS transaction report as a PDF.
        /// </summary>
        public ActionResult PDFAepsReport(string txt_frm_date, string txt_to_date, string allretailer, string ddl_status)
        {

            var loginid = User.Identity.GetUserId();
            //show all retailer 
            if (txt_frm_date == null && txt_to_date == null && ddl_status == null)
            {
                txt_frm_date = DateTime.Now.ToString();
                txt_to_date = DateTime.Now.ToString();

            }
            string ddlusers = "";
            string userid = "";

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

            if (allretailer == "" || allretailer == null)
            {
                userid = loginid;
                ddlusers = "Dealer";
            }
            else
            {
                userid = allretailer;
                ddlusers = "Retailer";
            }
            if (ddl_status == null)
            {
                ddl_status = "ALL";

            }

            var respo = db.Aeps_Report_New(ddlusers, userid, ddl_status, 0, "", "", "", "ALL", Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), 1, 100000).ToList();

            return new ViewAsPdf(respo);
        }


        //pancardreport
        /// <summary>
        /// Displays the PAN card service transaction report.
        /// </summary>
        [HttpGet]
        public ActionResult Pancard_report()
        {
            var userid = User.Identity.GetUserId();
            //show all retailer 
            ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl(userid), "RetailerId", "Frm_Name", null).ToList();


            return View();
        }
        /// <summary>
        /// Displays the PAN card service transaction report.
        /// </summary>
        public ActionResult Pancard_report(string ddl_status, string ddl_top, string txt_frm_date, string txt_to_date, string allretailer)
        {
            var loginid = User.Identity.GetUserId();
            //show all retailer 
            ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl(loginid), "RetailerId", "Frm_Name", null).ToList();
            ViewBag.chk = "post";


            return View();
        }
        /// <summary>
        /// Returns the PAN card report partial view.
        /// </summary>
        public ActionResult _Pancard_report(string ddl_status, string txt_frm_date, string txt_to_date, string allretailer)
        {
            if (txt_frm_date == null && txt_to_date == null)
            {

                txt_frm_date = DateTime.Now.ToString();
                txt_to_date = DateTime.Now.ToString();

            }
            var loginid = User.Identity.GetUserId();
            //show all retailer 
            ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl(loginid), "RetailerId", "Frm_Name", null).ToList();
            ViewBag.chk = "post";
            string ddlusers = "";
            string userid = "";

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

            if (allretailer == "" || allretailer == null)
            {
                userid = loginid;
                ddlusers = "Dealer";
            }
            else
            {
                userid = allretailer;
                ddlusers = "Retailer";
            }
            if (ddl_status == null)
            {
                ddl_status = "ALL";
            }


            var ch = db.PAN_CARD_IPAY_Token_report_paging(1, 20, userid, ddlusers, ddl_status, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();


            return View(ch);
        }

        /// <summary>
        /// Displays the new format PAN card transaction report.
        /// </summary>
        public ActionResult Pancard_report_new()
        {
            var loginid = User.Identity.GetUserId();
            //show all retailer 
            ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl(loginid), "RetailerId", "Frm_Name", null).ToList();
            var userid = User.Identity.GetUserId();
            string txt_frm_date = DateTime.Now.ToString();
            string txt_to_date = DateTime.Now.ToString();
            string frm_date = Convert.ToDateTime(txt_frm_date).Date.ToString("yyyy-MM-dd");
            string to_date = Convert.ToDateTime(txt_to_date).AddDays(1).ToString("yyyy-MM-dd");
            var frm_date1 = Convert.ToDateTime(frm_date);
            var ch = db.pancard_transation.Where(a => a.dealer_id == userid && a.request_time > frm_date1).OrderByDescending(s => s.idno).ToList();

            return View(ch);
        }
        /// <summary>
        /// Displays the new format PAN card transaction report.
        /// </summary>
        [HttpPost]
        public ActionResult Pancard_report_new(string ddl_status, string txt_frm_date, string txt_to_date, string allretailer)
        {
            if (txt_frm_date == null && txt_to_date == null)
            {

                txt_frm_date = DateTime.Now.ToString();
                txt_to_date = DateTime.Now.ToString();

            }
            var loginid = User.Identity.GetUserId();
            //show all retailer 
            ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl(loginid), "RetailerId", "Frm_Name", null).ToList();
            ViewBag.chk = "post";
            string ddlusers = "";
            string userid = loginid;

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

            var ch = db.pancard_transation.Where(s => s.dealer_id == userid).ToList();
            if (string.IsNullOrWhiteSpace(allretailer))
            {
                if(ddl_status == "ALL")
                {
                     ch = db.pancard_transation.Where(s => s.dealer_id == userid && s.request_time > frm_date1 && s.request_time<to_date1).ToList();
                }
                else
                {
                     ch = db.pancard_transation.Where(s => s.dealer_id == userid && s.request_time > frm_date1 && s.request_time < to_date1 && s.status.ToUpper() == ddl_status).ToList();
                }
            }
            else
            {
                if (ddl_status == "ALL")
                {
                     ch = db.pancard_transation.Where(s => s.dealer_id == userid && s.request_time > frm_date1 && s.request_time < to_date1 && s.Reailerid == allretailer).ToList();
                }
                else
                {
                     ch = db.pancard_transation.Where(s => s.dealer_id == userid && s.request_time > frm_date1 && s.request_time < to_date1 && s.status.ToUpper() == ddl_status && s.Reailerid == allretailer).ToList();
                }
            }


            return View(ch);

        }

        /// <summary>
        /// Returns paginated PAN card transaction records for infinite scroll.
        /// </summary>
        public ActionResult InfiniteScrollpan(int pageindex, string ddl_status, string txt_frm_date, string txt_to_date, string allretailer)
        {

            if (txt_frm_date == null && txt_to_date == null)
            {

                txt_frm_date = DateTime.Now.ToString();
                txt_to_date = DateTime.Now.ToString();

            }
            var loginid = User.Identity.GetUserId();
            //show all retailer 
            ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl(loginid), "RetailerId", "Frm_Name", null).ToList();
            ViewBag.chk = "post";
            string ddlusers = "";
            string userid = "";

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

            if (allretailer == "" || allretailer == null)
            {
                userid = loginid;
                ddlusers = "Dealer";
            }
            else
            {
                userid = allretailer;
                ddlusers = "Retailer";
            }
            if (ddl_status == null)
            {
                ddl_status = "ALL";
            }

            int pagesize = 20;
            var tbrow = db.PAN_CARD_IPAY_Token_report_paging(pageindex, pagesize, userid, ddlusers, ddl_status, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();

            JsonModel jsonmodel = new JsonModel();
            jsonmodel.NoMoredata = tbrow.Count < pagesize;
            jsonmodel.HTMLString = renderPartialViewtostring("_Pancard_report", tbrow);
            return Json(jsonmodel);
        }
        /// <summary>
        /// Displays the PAN card transaction summary totals.
        /// </summary>
        public ActionResult PancardReport_Total(string ddl_status, string txt_frm_date, string txt_to_date, string allretailer)
        {
            var loginid = User.Identity.GetUserId();
            //show all retailer 
            ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl(loginid), "RetailerId", "Frm_Name", null).ToList();
            ViewBag.chk = "post";
            string ddlusers = "";
            string userid = "";

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

            if (allretailer == "" || allretailer == null)
            {
                userid = loginid;
                ddlusers = "Dealer";
            }
            else
            {
                userid = allretailer;
                ddlusers = "Retailer";
            }

            if (ddl_status == null)
            {
                ddl_status = "ALL";
            }

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
        /// <summary>
        /// Exports the PAN card report to Excel.
        /// </summary>
        public ActionResult ExcelRechargereportPan(string ddl_status, string txt_frm_date, string txt_to_date, string allretailer)
        {
            var loginid = User.Identity.GetUserId();
            //show all retailer 
            ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl(loginid), "RetailerId", "Frm_Name", null).ToList();
            ViewBag.chk = "post";
            string ddlusers = "";
            string userid = "";

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

            if (allretailer == "" || allretailer == null)
            {
                userid = loginid;
                ddlusers = "Dealer";
            }
            else
            {
                userid = allretailer;
                ddlusers = "Retailer";
            }

            if (ddl_status == null)
            {
                ddl_status = "ALL";
            }

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
                item.RetailerIncome, item.retailer_remain_post, item.retailer_remain_pre, item.retailer_dealer_pre, item.DealerIncome, item.retailer_dealer_post,
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



        /// <summary>
        /// Exports the new format PAN card report to Excel.
        /// </summary>
        public ActionResult ExcelRechargereportPan1(string ddl_status, string txt_frm_date, string txt_to_date, string allretailer)
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {

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
                int pagesize = 2000;

                var proc_Response = db.pancard_transation.Where(a => a.Reailerid == userid && a.request_time > frm_date && a.request_time < to_date).OrderByDescending(s => s.idno).ToList();

             
                if (string.IsNullOrWhiteSpace(allretailer))
                {
                    if (ddl_status == "ALL")
                    {
                        proc_Response = db.pancard_transation.Where(s => s.dealer_id == userid && s.request_time > frm_date && s.request_time < to_date).ToList();
                    }
                    else
                    {
                        proc_Response = db.pancard_transation.Where(s => s.dealer_id == userid && s.request_time > frm_date && s.request_time < to_date && s.status.ToUpper() == ddl_status).ToList();
                    }
                }
                else
                {
                    if (ddl_status == "ALL")
                    {
                        proc_Response = db.pancard_transation.Where(s => s.dealer_id == userid && s.request_time > frm_date && s.request_time < to_date && s.Reailerid == allretailer).ToList();
                    }
                    else
                    {
                        proc_Response = db.pancard_transation.Where(s => s.dealer_id == userid && s.request_time > frm_date && s.request_time < to_date && s.status.ToUpper() == ddl_status && s.Reailerid == allretailer).ToList();
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
                        dataTbl.Rows.Add(item.status, item.Email+"=>"+item.name, item.Amount, item.Rem_pre, item.Rem_tds, item.Rem_comm, item.rem_post, item.Rem_final, item.dlm_pre,item.dlm_comm,item.dlm_tds,item.dlm_post,item.dlm_final,item.request_time);
                    }
                }
                else
                {
                    dataTbl.Rows.Add("", "", "", "", "", "", "", "", "" ,"","","","","");
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
        }

        /// <summary>
        /// Generates the PAN card report as a PDF.
        /// </summary>
        public ActionResult PDFreport_PAN(string ddl_status, string txt_frm_date, string txt_to_date, string allretailer)
        {
            var loginid = User.Identity.GetUserId();
            //show all retailer 
            ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl(loginid), "RetailerId", "Frm_Name", null).ToList();
            ViewBag.chk = "post";
            string ddlusers = "";
            string userid = "";

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

            if (allretailer == "" || allretailer == null)
            {
                userid = loginid;
                ddlusers = "Dealer";
            }
            else
            {
                userid = allretailer;
                ddlusers = "Retailer";
            }
            if (ddl_status == null)
            {
                ddl_status = "ALL";
            }
            var respo = db.PAN_CARD_IPAY_Token_report_paging(1, 10000000, userid, ddlusers, ddl_status, frm_date, to_date).ToList();
            return new ViewAsPdf(respo);
        }

        /// <summary>
        /// Generates the new format PAN card report as a PDF.
        /// </summary>
        public ActionResult PDFreport_PAN1(string ddl_status, string txt_frm_date, string txt_to_date, string allretailer)
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
            var frm_date1 = Convert.ToDateTime(frm_date);
            var to_date1 = Convert.ToDateTime(to_date);

          
                var ch = db.pancard_transation.Where(a => a.Reailerid == userid && a.request_time > frm_date1 && a.request_time < to_date1).OrderByDescending(s => s.idno).ToList();
               
           

            if (string.IsNullOrWhiteSpace(allretailer))
            {
                if (ddl_status == "ALL")
                {
                    ch = db.pancard_transation.Where(s => s.dealer_id == userid && s.request_time > frm_date && s.request_time < to_date).ToList();
                }
                else
                {
                    ch = db.pancard_transation.Where(s => s.dealer_id == userid && s.request_time > frm_date && s.request_time < to_date && s.status.ToUpper() == ddl_status).ToList();
                }
            }
            else
            {
                if (ddl_status == "ALL")
                {
                    ch = db.pancard_transation.Where(s => s.dealer_id == userid && s.request_time > frm_date && s.request_time < to_date && s.Reailerid == allretailer).ToList();
                }
                else
                {
                    ch = db.pancard_transation.Where(s => s.dealer_id == userid && s.request_time > frm_date && s.request_time < to_date && s.status.ToUpper() == ddl_status && s.Reailerid == allretailer).ToList();
                }
            }
            return new ViewAsPdf(ch);
        }

        //MicroATMReport
        /// <summary>
        /// Displays the Micro ATM transaction report.
        /// </summary>
        [HttpGet]
        public ActionResult MIcroAtmReport()
        {
            var userid = User.Identity.GetUserId();
            //show all retailer 
            ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl(userid), "RetailerId", "Frm_Name", null).ToList();


            return View();
        }
        /// <summary>
        /// Displays the Micro ATM transaction report.
        /// </summary>
        [HttpPost]
        public ActionResult MIcroAtmReport(string txt_frm_date, string txt_to_date, string allretailer, string ddl_status)
        {
            var loginid = User.Identity.GetUserId();
            //show all retailer 
            ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl(loginid), "RetailerId", "Frm_Name", null).ToList();
            ViewBag.chk = "post";
            return View();
        }
        /// <summary>
        /// Returns the Micro ATM report partial view.
        /// </summary>
        public ActionResult _MIcroAtmReport(string txt_frm_date, string txt_to_date, string allretailer, string ddl_status)
        {
            if (txt_frm_date == null && txt_to_date == null)
            {

                txt_frm_date = DateTime.Now.ToString();
                txt_to_date = DateTime.Now.ToString();

            }
            var loginid = User.Identity.GetUserId();
            //show all retailer 
            ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl(loginid), "RetailerId", "Frm_Name", null).ToList();
            ViewBag.chk = "post";
            string ddlusers = "";
            string userid = "";

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

            if (allretailer == "" || allretailer == null)
            {
                userid = loginid;
                ddlusers = "Dealer";
            }
            else
            {
                userid = allretailer;
                ddlusers = "Retailer";
            }
            if (ddl_status == null)
            {
                ddl_status = "ALL";
            }


            var ch = db.microatm_report(ddlusers, userid, frm_date, to_date, ddl_status);


            return View(ch);
        }

        //CashDepositreport
        /// <summary>
        /// Displays the cash deposit transaction report.
        /// </summary>
        [HttpGet]
        public ActionResult CashDepositReport()
        {
            var userid = User.Identity.GetUserId();
            //show all retailer 
            ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl(userid), "RetailerId", "Frm_Name", null).ToList();
            return View();
        }
        /// <summary>
        /// Displays the cash deposit transaction report.
        /// </summary>
        [HttpPost]
        public ActionResult CashDepositReport(string txt_frm_date, string txt_to_date, string allretailer, string ddl_status)
        {
            var loginid = User.Identity.GetUserId();
            //show all retailer 
            ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl(loginid), "RetailerId", "Frm_Name", null).ToList();
            ViewBag.chk = "post";
            return View();
        }
        /// <summary>
        /// Returns the cash deposit report partial view.
        /// </summary>
        public ActionResult _CashDepositReport(string txt_frm_date, string txt_to_date, string allretailer, string ddl_status)
        {
            var loginid = User.Identity.GetUserId();
            //show all retailer 
            if (txt_frm_date == null && txt_to_date == null && ddl_status == null)
            {
                txt_frm_date = DateTime.Now.ToString();
                txt_to_date = DateTime.Now.ToString();

            }
            string ddlusers = "";
            string userid = "";

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

            if (allretailer == "" || allretailer == null)
            {
                userid = loginid;
                ddlusers = "Dealer";
            }
            else
            {
                userid = allretailer;
                ddlusers = "Retailer";
            }
            if (ddl_status == null)
            {
                ddl_status = "ALL";

            }

            var ch = db.report_cash_deposit(frm_date, to_date, ddl_status, ddlusers, userid).ToList();

            return View(ch);
        }



        /// <summary>
        /// Displays the Indo-Nepal remittance transaction history.
        /// </summary>
        [HttpGet]
        public ActionResult IndoNepalHistory()
        {
            string userid = User.Identity.GetUserId();
            string txt_frm_date = DateTime.Now.ToString();
            string txt_to_date = DateTime.Now.ToString();
            string frm_date = Convert.ToDateTime(txt_frm_date).Date.ToString("yyyy-MM-dd");
            string to_date = Convert.ToDateTime(txt_to_date).AddDays(1).ToString("yyyy-MM-dd");
            // show master id 
            var stands = db.Superstokist_details.ToList();
            IEnumerable<SelectListItem> selectList = from s in stands
                                                     select new SelectListItem
                                                     {
                                                         Value = s.SSId,
                                                         Text = s.Email + "--" + s.SuperstokistName.ToString()
                                                     };
            ViewBag.allmaster = new SelectList(selectList, "Value", "Text");
            //show dealer
            ViewBag.alldealer = new SelectList(db.select_Dealer_for_ddl(), "Dealerid", "DealerName", null).ToList(); ;
            //Retailer 
            ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl("Admin"), "RetailerId", "RetailerName", null);
            //api users 
            ViewBag.apiid = new SelectList(db.select_apiusers_for_ddl("Admin"), "apiid", "username", null);
            //apiname
            var apiname = db.money_api_status.ToList();
            IEnumerable<SelectListItem> selectapiname = from p in apiname
                                                        select new SelectListItem
                                                        {
                                                            Value = p.api_name,
                                                            Text = p.api_name
                                                        };
            ViewBag.apiname = new SelectList(selectapiname, "Value", "Text");
            var ch = db.proc_report_IndoNepal(null, userid, null, null, 50, null, null, null, null, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            ViewData["totals"] = ch.Where(s => s.status.ToUpper().Contains("SUCCESS")).Sum(s => Convert.ToInt32(s.Amount));
            ViewData["totalf"] = ch.Where(s => s.status.ToUpper().Contains("FAILED")).Sum(s => Convert.ToInt32(s.Amount));
            ViewData["totalp"] = ch.Where(s => s.status.ToUpper().Contains("PENDING")).Sum(s => Convert.ToInt32(s.Amount));
            return View(ch);
        }
        /// <summary>
        /// Displays the Indo-Nepal remittance transaction history.
        /// </summary>
        [HttpPost]
        public ActionResult IndoNepalHistory(string txt_frm_date, string txt_to_date, string ddlusers, string allmaster, string alldealer, string allretailer, string ddl_top, string ddl_status, string api_name, string allapiuser, string txtmob)
        {
            string userid = User.Identity.GetUserId();
            // var APIname = "";
            ViewBag.chk = "post";

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

            // show master id 
            var stands = db.Superstokist_details.ToList();
            IEnumerable<SelectListItem> selectList = from s in stands
                                                     select new SelectListItem
                                                     {
                                                         Value = s.SSId,
                                                         Text = s.Email + "--" + s.SuperstokistName.ToString()
                                                     };
            ViewBag.allmaster = new SelectList(selectList, "Value", "Text");
            //show dealer
            ViewBag.alldealer = new SelectList(db.select_Dealer_for_ddl(), "Dealerid", "DealerName", null).ToList(); ;
            //Retailer 
            ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl("Admin"), "RetailerId", "RetailerName", null);
            //api users 
            ViewBag.apiid = new SelectList(db.select_apiusers_for_ddl("Admin"), "apiid", "username", null);
            //apiname
            var apiname = db.money_api_status.ToList();
            IEnumerable<SelectListItem> selectapiname = from p in apiname
                                                        select new SelectListItem
                                                        {
                                                            Value = p.api_name,
                                                            Text = p.api_name
                                                        };
            ViewBag.apiname = new SelectList(selectapiname, "Value", "Text");

            if (ddlusers == "Master")
            {
                if (allmaster == "" || allmaster.Contains("Master") || allmaster == null)
                {
                    allmaster = null;
                }

            }
            //if (ddlusers == "Dealer")
            //{
            //    if (alldealer == "" || alldealer.Contains("Distubutor") || alldealer == null)
            //    {
            //        alldealer = null;
            //    }

            //}
            if (ddlusers == "Retailer")
            {
                if (allretailer == "" || allretailer.Contains("All Retailer") || allretailer == null)
                {
                    allretailer = null;
                }

            }
            if (ddlusers == "Admin")
            {
                userid = null;
            }

            if (ddl_top == "All")
            {
                ddl_top = "1000000";
            }

            if (ddl_status == "Status")
            {
                ddl_status = null;
            }
            if (ddl_status == "ALL")
            {
                ddl_status = null;
            }
            //if (api_name == "Select Api Name" || api_name.Contains("Select Api Name") || api_name == "")
            //{
            //    APIname = null;
            //}
            //else
            //{
            //    APIname = api_name.ToUpper();
            //}
            int ddltop = Convert.ToInt32(ddl_top);
            if (allretailer == "" || allretailer.Contains("All Retailer") || allretailer == null)
            {
                allretailer = null;
            }

            var ch = db.proc_report_IndoNepal(allretailer, userid, allmaster, null, ddltop, ddl_status, null, null, null, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            //txtmob = txtmob.Trim();
            //if (txtmob != "")
            //{
            //    ch = ch.Where(aa => aa.senderidnumber == txtmob || aa.Accountno == txtmob).ToList();
            //}

            ViewData["totals"] = ch.Where(s => s.status.ToUpper().Contains("SUCCESS")).Sum(s => Convert.ToInt32(s.Amount));
            ViewData["totalf"] = ch.Where(s => s.status.ToUpper().Contains("FAILED")).Sum(s => Convert.ToInt32(s.Amount));
            ViewData["totalp"] = ch.Where(s => s.status.ToUpper().Contains("PENDING")).Sum(s => Convert.ToInt32(s.Amount));

            return View(ch);
        }


        #region Giftcards

        /// <summary>
        /// Displays the prepaid card transaction report.
        /// </summary>
        public ActionResult Prepaid_Card_report()
        {
            string userid = User.Identity.GetUserId();
            var stands = db.Retailer_Details.Where(s=>s.DealerId == userid).ToList();
            IEnumerable<SelectListItem> selectList = from s in stands
                                                     select new SelectListItem
                                                     {
                                                         Value = s.RetailerId,
                                                         Text = s.Email + "--" + s.Frm_Name.ToString()
                                                     };

            ViewBag.retailer = new SelectList(selectList, "Value", "Text");

            var date = DateTime.Now.ToString();
            var date1 = Convert.ToDateTime(date).Date.ToString("yyyy-MM-dd");
            var date2 = Convert.ToDateTime(date1);
            var date3 = date2.AddDays(1);
            var report = db.prepaid_card_Transaction.Where(s => s.Request_time >= date2 && s.dlm_id == userid).OrderByDescending(a => a.idno).ToList();

            return View(report);
        }
        /// <summary>
        /// Displays the prepaid card transaction report.
        /// </summary>
        [HttpPost]
        public ActionResult Prepaid_Card_report(string allretailer, DateTime txt_frm_date, DateTime txt_to_date)
        {
            string userid = User.Identity.GetUserId();
            var date = DateTime.Now.ToString();
            var date1 = Convert.ToDateTime(txt_frm_date).Date.ToString("yyyy-MM-dd");
            var date2 = Convert.ToDateTime(date1);
            var date3 = Convert.ToDateTime(txt_to_date).Date.ToString("yyyy-MM-dd");
            var date4 = Convert.ToDateTime(date3).AddDays(1);

            if (string.IsNullOrWhiteSpace(allretailer))
            {
                /* db.prepaid_card_Transaction.Where(s => s.Request_time >= date2 && s.Request_time < date4).OrderByDescending(a => a.idno).ToList();*/


                var report = db.prepaid_card_Transaction.Where(s => s.Request_time >= date2 &&s.Request_time<date4 && s.dlm_id == userid).OrderByDescending(a => a.idno).ToList();

                return PartialView("_prepaidcard", report);

            }
            else
            {
                var del = db.Retailer_Details.Where(s => s.RetailerId == allretailer).SingleOrDefault();
                var report = db.prepaid_card_Transaction.Where(s => s.Request_time >= date2 && s.Request_time < date4 && s.dlm_id == userid && s.REtailerid == allretailer).OrderByDescending(a => a.idno).ToList();

                return PartialView("_prepaidcard", report);
            }
        }


        /// <summary>
        /// Displays the gift card management and report page.
        /// </summary>
        public ActionResult Giftcard()
        {
            string userid = User.Identity.GetUserId();
            //show all retailer 
            ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl(userid), "RetailerId", "RetailerName", null).ToList();
            var operator_value = db.Operator_Code.Where(a => a.Operator_type == "DigitalVoucher").Distinct().ToList();
            ViewBag.Operator = new SelectList(operator_value, "new_opt_code", "operator_Name");
            string txt_frm_date = DateTime.Now.ToString();
            string txt_to_date = DateTime.Now.ToString();
            string frm_date = Convert.ToDateTime(txt_frm_date).Date.ToString("yyyy-MM-dd");
            string to_date = Convert.ToDateTime(txt_to_date).AddDays(1).ToString("yyyy-MM-dd");
            var Liverechargecount = db.Gift_card_details_report(Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), userid, "", "Dealer", "", "ALL", 50).ToList();
            var totalsuccesscount = Liverechargecount.Where(a => a.status.ToUpper() == "SUCCESS").Sum(a => a.amount);
            var totalfailed = Liverechargecount.Where(a => a.status.ToUpper() == "FAILED").Sum(a => a.amount);
            var totalpending = Liverechargecount.Where(a => a.status.ToUpper() == "PENDING").Sum(a => a.amount);
            ViewData["Totals"] = totalsuccesscount;
            ViewData["Totalf"] = totalfailed;
            ViewData["Totalp"] = totalpending;
            return View(Liverechargecount);
        }

        /// <summary>
        /// Displays the gift card management and report page.
        /// </summary>
        [HttpPost]
        public ActionResult Giftcard(string txt_frm_date, string txt_to_date, string ddl_status, string Operator, string ddl_top, string allretailer)
        {
            var userid = "";
            var optname = "";
            var ddlusers = "";
            ViewBag.chk = "post";
            var dealerid = User.Identity.GetUserId();
            if (allretailer == "" || allretailer == null)
            {
                userid = dealerid;
                ddlusers = "Dealer";
            }
            else
            {
                userid = allretailer;
                ddlusers = "Retailer";
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
            // int ddltop = Convert.ToInt32(ddl_top);
            int ddltop = 1000000;
            //show all retailer 
            ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl(dealerid), "RetailerId", "RetailerName", null).ToList();
            var operator_value = db.Operator_Code.Where(a => a.Operator_type == "DigitalVoucher").Distinct().ToList();
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
            var Liverechargecount = db.Gift_card_details_report(Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), userid, optname, ddlusers, "", ddl_status, ddltop).ToList();
            var totalsuccesscount = Liverechargecount.Where(a => a.status.ToUpper() == "SUCCESS").Sum(a => a.amount);
            var totalfailed = Liverechargecount.Where(a => a.status.ToUpper() == "FAILED").Sum(a => a.amount);
            var totalpending = Liverechargecount.Where(a => a.status.ToUpper() == "PENDING").Sum(a => a.amount);
            ViewData["Totals"] = totalsuccesscount;
            ViewData["Totalf"] = totalfailed;
            ViewData["Totalp"] = totalpending;
            return View(Liverechargecount);

        }
        #endregion

        #region Ecommerce
        /// <summary>
        /// Displays the e-commerce transaction report.
        /// </summary>
        public ActionResult Ecommerce_Report()
        {
            string userid = User.Identity.GetUserId();
            //show all retailer 
            ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl(userid), "RetailerId", "RetailerName", null).ToList();

            var category = db.Catagories.Distinct().ToList();
            ViewBag.category = new SelectList(category, "CatID", "CatName");
            string txt_frm_date = DateTime.Now.ToString();
            string txt_to_date = DateTime.Now.ToString();
            string frm_date = Convert.ToDateTime(txt_frm_date).Date.ToString("yyyy-MM-dd");
            string to_date = Convert.ToDateTime(txt_to_date).AddDays(1).ToString("yyyy-MM-dd");
            var vv = db.show_ecomm_report(userid, "Dealer", "ALL", "", 50, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();

            var totalsuccess = vv.Where(a => a.OrderStatus == 2).Sum(a => a.Amount);
            var totalpending = vv.Where(a => a.OrderStatus == 1).Sum(a => a.Amount);
            var totalreject = vv.Where(a => a.OrderStatus == 3).Sum(a => a.Amount);
            ViewData["Totals"] = totalsuccess;
            ViewData["Totalp"] = totalpending;
            ViewData["Totalf"] = totalreject;
            return View(vv);
        }

        /// <summary>
        /// Displays the e-commerce transaction report.
        /// </summary>
        [HttpPost]
        public ActionResult Ecommerce_Report(string category, int ddl_top, string ddl_status, string txt_frm_date, string txt_to_date, string allretailer)
        {
            var loginid = User.Identity.GetUserId();
            //show all retailer 
            ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl(loginid), "RetailerId", "RetailerName", null).ToList();
            ViewBag.chk = "post";
            string ddlusers = "";
            string userid = "";

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

            var category1 = db.Catagories.Distinct().ToList();
            ViewBag.category = new SelectList(category1, "CatID", "CatName");

            if (allretailer == "" || allretailer == null)
            {
                userid = loginid;
                ddlusers = "Dealer";
            }
            else
            {
                userid = allretailer;
                ddlusers = "Retailer";
            }


            var vv = db.show_ecomm_report(userid, ddlusers, ddl_status, category, ddl_top, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();

            var totalsuccess = vv.Where(a => a.OrderStatus == 2).Sum(a => a.Amount);
            var totalpending = vv.Where(a => a.OrderStatus == 1).Sum(a => a.Amount);
            var totalreject = vv.Where(a => a.OrderStatus == 3).Sum(a => a.Amount);
            ViewData["Totals"] = totalsuccess;
            ViewData["Totalp"] = totalpending;
            ViewData["Totalf"] = totalreject;
            return View(vv);
        }
        #endregion

        //Start GST Report 
        /// <summary>
        /// Displays the GST transaction report with date filters.
        /// </summary>
        public ActionResult GST_Report()
        {
            string userid = User.Identity.GetUserId();
            DateTime fromdate = DateTime.Now.Date;
            DateTime todate = DateTime.Now.Date.AddDays(1);
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
            var gst = db.GST_Report_Dealer(fromdate, todate, userid, openingBal, ClosingBal, PurchaseAmt, SellAmt, IsIGST).ToList();
            // var gst = db.GST_Report_Retailer(fromdate, todate, userid, openingBal, ClosingBal, PurchaseAmt, IsIGST).ToList();

            ViewBag.OpnBal = !string.IsNullOrWhiteSpace(openingBal.Value.ToString()) ? openingBal.Value.ToString() : "0";
            ViewBag.CloseBal = !string.IsNullOrWhiteSpace(ClosingBal.Value.ToString()) ? ClosingBal.Value.ToString() : "0";
            ViewBag.Purchase = !string.IsNullOrWhiteSpace(PurchaseAmt.Value.ToString()) ? PurchaseAmt.Value.ToString() : "0";
            ViewBag.Sell = !string.IsNullOrWhiteSpace(SellAmt.Value.ToString()) ? SellAmt.Value.ToString() : "0";
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
        /// <summary>
        /// Displays the GST transaction report with date filters.
        /// </summary>
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
            var gst = db.GST_Report_Dealer(frm_date, to_date, userid, openingBal, ClosingBal, PurchaseAmt, SellAmt, IsIGST).ToList();
            // var gst = db.GST_Report_Retailer(fromdate, todate, userid, openingBal, ClosingBal, PurchaseAmt, IsIGST).ToList();

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
        /// <summary>
        /// Downloads the GST declaration form document.
        /// </summary>
        [HttpGet]
        public FileResult Download_Declaration_form()
        {
            string[] filesInDirectory = Directory.GetFiles(Server.MapPath("~/GST_Declaration"), "*.docx");

            return File(filesInDirectory[0], "application/force-download", Path.GetFileName(filesInDirectory[0]));
        }
        //GST Invocing 
        /// <summary>
        /// Displays the GST invoicing management page.
        /// </summary>
        [HttpPost]
        public ActionResult GST_Invoicing(DateTime month)
        {

            string UserId = User.Identity.GetUserId();
            string from = month.ToString("yyyy-MM-dd");
            string to = month.AddMonths(1).ToString("yyyy-MM-dd");
            //string x = DateTime.Now.ToString("yyyy-MM-dd");
            return new Rotativa.ActionAsPdf("GST_INVOICE_PDF", new { userid = UserId, txt_frm_date = from, txt_to_date = to, month = month });
        }
        /// <summary>
        /// Generates a GST invoice as a PDF.
        /// </summary>
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
            System.Data.Entity.Core.Objects.ObjectParameter SellAmt = new
        System.Data.Entity.Core.Objects.ObjectParameter("SellAmt", typeof(string));
            if (role.Name.Contains("Dealer"))
            {
                model.DealerGst = db.GST_Report_Dealer(frm_date, to_date, userid, openingBal, ClosingBal, PurchaseAmt, SellAmt, IsIGST).
                          Where(a => a.operator_Name != "Dish TV" && !a.Operator_type.Contains("PostPaid") && a.Operator_type != "Landline" && a.Operator_type != "Electricity" && a.Operator_type != "Gas" && a.Operator_type != "Insurance" && a.Operator_type != "Money" && a.Operator_type != "DTH-Booking").ToList();
                //Convert function call
                var converword = new Vastwebmulti.Areas.WRetailer.Models.Convertword().changeToWords(model.DealerGst.Sum(a => a.NetAmount).ToString());
                ViewData["total"] = converword;
                var UserDetails = db.Dealer_Details.Where(a => a.DealerId.ToLower() == userid.ToLower()).SingleOrDefault();
                ViewBag.UserDetails = UserDetails;
            }

            ViewBag.OpnBal = !string.IsNullOrWhiteSpace(openingBal.Value.ToString()) ? openingBal.Value.ToString() : "0";
            ViewBag.CloseBal = !string.IsNullOrWhiteSpace(ClosingBal.Value.ToString()) ? ClosingBal.Value.ToString() : "0";
            ViewBag.Purchase = !string.IsNullOrWhiteSpace(PurchaseAmt.Value.ToString()) ? PurchaseAmt.Value.ToString() : "0";
            ViewBag.IsIGST = !string.IsNullOrWhiteSpace(IsIGST.Value.ToString()) ? IsIGST.Value.ToString() : "0";
            var AdminDetails = db.Admin_details.SingleOrDefault();
            ViewBag.AdminDetails = AdminDetails;
            ViewBag.Role = role;


            model.INVOICENO = invc;
            //ViewBag.State = db.State_Desc.Where(y => y.State_id == AdminDetails.State).Single().State_name;
            ViewBag.city = db.District_Desc.Where(f => f.Dist_id == AdminDetails.District && f.State_id == AdminDetails.State).Single().Dist_Desc;

            return View(model);
        }

        //GST PDF Invoicing 
        /// <summary>
        /// Triggers GST PDF generation for selected records.
        /// </summary>
        public ActionResult GenerateGST_PDF(DateTime frm_date, DateTime to_date)
        {
            return new Rotativa.ActionAsPdf("GST_PDF", new { txt_frm_date = frm_date, txt_to_date = to_date });
        }
        /// <summary>
        /// Returns the generated GST PDF document.
        /// </summary>
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
            var gst = db.GST_Report_Dealer(frm_date, to_date, userid, openingBal, ClosingBal, PurchaseAmt, SellAmt, IsIGST).ToList();
            // var gst = db.GST_Report_Retailer(fromdate, todate, userid, openingBal, ClosingBal, PurchaseAmt, IsIGST).ToList();

            ViewBag.OpnBal = !string.IsNullOrWhiteSpace(openingBal.Value.ToString()) ? openingBal.Value.ToString() : "0";
            ViewBag.CloseBal = !string.IsNullOrWhiteSpace(ClosingBal.Value.ToString()) ? ClosingBal.Value.ToString() : "0";
            ViewBag.Purchase = !string.IsNullOrWhiteSpace(PurchaseAmt.Value.ToString()) ? PurchaseAmt.Value.ToString() : "0";
            ViewBag.IsIGST = !string.IsNullOrWhiteSpace(IsIGST.Value.ToString()) ? IsIGST.Value.ToString() : "0";
            var UserDetails = db.Dealer_Details.Where(a => a.DealerId.ToLower() == userid.ToLower()).SingleOrDefault();
            ViewBag.UserDetails = UserDetails;
            var AdminDetails = db.Admin_details.SingleOrDefault();
            ViewBag.AdminDetails = AdminDetails;
            return View(gst);
        }

        /// <summary>
        /// Displays active users/retailers under the dealer.
        /// </summary>
        public ActionResult Active_User()
        {
            return View();
        }

        //public ActionResult GatewayTRANSFER_Dealer()
        //{
        //    return View();
        //}
        /// <summary>
        /// Displays the dealer wallet transaction history.
        /// </summary>
        public ActionResult Wallethistory_Dealer()
        {
            return View();
        }
        /// <summary>
        /// Displays and processes UPI transfer transactions for the dealer.
        /// </summary>
        public ActionResult UPITRANSFER_Dealer()
        {
            var userid = User.Identity.GetUserId();
            var slab = db.Upi_slab.SingleOrDefault();
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
            var ch = db.show_upi_txn_details("Distributor", userid, "ALL", from, to);

            var upitxndetails = db.Upi_txn_details.Where(x => x.userid == userid && x.rolename == "Dealer" && x.txndate >= from && x.txndate <= to);
            var totalcharges = upitxndetails.Sum(x => x.charge);
            ViewBag.totalchargesamount = totalcharges;
            return View(ch);
        }


        /// <summary>
        /// Displays and processes UPI transfer transactions for the dealer.
        /// </summary>
        [HttpPost]
        public ActionResult UPITRANSFER_Dealer(DateTime txt_frm_date, DateTime txt_to_date)
        {
            ViewBag.chk = "post";
            var userid = User.Identity.GetUserId();
            var slab = db.Upi_slab.SingleOrDefault();
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
            DateTime from = txt_frm_date;
            DateTime to = txt_to_date.AddDays(1).Date;
            var ch = db.show_upi_txn_details("Distributor", userid, "ALL", from, to);

            var upitxndetails = db.Upi_txn_details.Where(x => x.userid == userid && x.rolename == "Dealer" && x.txndate >= from && x.txndate <= to);
            var totalcharges = upitxndetails.Sum(x => x.charge);
            ViewBag.totalchargesamount = totalcharges;
            return View(ch);
        }

        #region RetailerCreationToken
        /// <summary>
        /// Displays the token purchase history for the dealer.
        /// </summary>
        public ActionResult tokenPurchaseHistory()
        {
            var userid = User.Identity.GetUserId();
            var Entries = db.RetailerCreationTokensAssignHistories.Where(a => a.DealerId == userid).ToList();
            return View(Entries);
        }

        /// <summary>
        /// Displays the used token transaction history.
        /// </summary>
        public ActionResult usedTokens()
        {
            var userid = User.Identity.GetUserId();
            var Entries = db.prc_getRetailerCreationUsedTokens(userid).OrderByDescending(a => a.CreatedBy).ToList();
            return View(Entries);
        }
        /// <summary>
        /// Displays the token purchase page and processes token purchases.
        /// </summary>
        public ActionResult TokenPurchase()
        {
            var dealerid = User.Identity.GetUserId();
            var entries = db.RetailerCreationTokensAssignHistories.Where(a => a.DealerId == dealerid).Join(db.Dealer_Details, tkn => tkn.DealerId, dlm => dlm.DealerId, (tkn, dlm) => new RetailerCreationTokenVM
            {
                DealerId = dlm.DealerId,
                Role = tkn.CommonId,
                Email = dlm.FarmName,
                Idno = tkn.Idno,
                Tokens = tkn.Tokens,
                CteatedOn = tkn.CreatedOn,
                pre = tkn.RemainTokenPre,
                post = tkn.RemainTokenPost,
                DealerPre = tkn.DealerPre,
                DealerPost = tkn.DealerPost,
                //AdminPre = tkn.AdminPre,
                //AdminPost = tkn.AdminPost,
                PerTokenValue = tkn.PerTokenValue,
                TotalDebit = tkn.TotalDebit
            }).OrderByDescending(a => a.Idno);
            var tokenvaluedlmmd = db.TokenValueByAdmins.ToList();
            if (tokenvaluedlmmd.Any())
            {
                ViewBag.dealervalue = tokenvaluedlmmd.SingleOrDefault().DistributorValue;
                ViewBag.mastervalue = tokenvaluedlmmd.SingleOrDefault().MasterValue;
            }
            else
            {
                ViewBag.dealervalue = 0;
                ViewBag.mastervalue = 0;
            }
            //var dealers = dealrs.Select(a => new SelectListItem { Text = a.Email + " - " + a.Mobile, Value = a.DealerId }).ToList();
            //ViewBag.ddlDealers = dealers;
            //if (!string.IsNullOrWhiteSpace(dealerId) && dealerId != "ALL")
            //{
            //    entries = entries.Where(a => a.DealerId == dealerId);
            //}
            return View(entries);
        }

        /// <summary>
        /// Displays the token purchase page and processes token purchases.
        /// </summary>
        [HttpPost]
        public ActionResult TokenPurchase(int tokenCount)
        {
            try
            {
                if (tokenCount < 1) throw new Exception("Token Can Not Be Negative");
                var userid = User.Identity.GetUserId();
                RetailerCreationTokensAssignHistory entry = new RetailerCreationTokensAssignHistory();
                RetailerCreationToken token = db.RetailerCreationTokens.SingleOrDefault(a => a.DealerId == userid);
                if (token == null)
                {
                    token = new RetailerCreationToken();
                    token.DealerId = userid;
                    token.Tokens = tokenCount;
                    db.RetailerCreationTokens.Add(token);
                    entry.CreatedOn = DateTime.Now;
                    entry.DealerId = userid;
                    entry.Tokens = tokenCount;
                    entry.RemainTokenPre = 0;
                    entry.RemainTokenPost = tokenCount;
                    entry.CommonId = "Self";
                    //db.RetailerCreationTokensAssignHistories.Add(entry);
                }
                else
                {
                    entry.CreatedOn = DateTime.Now;
                    entry.DealerId = userid;
                    entry.CommonId = "Self";
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
                    var Dealerinfo = db.Remain_dealer_balance.Where(a => a.DealerID == userid).SingleOrDefault();
                    var DealerPre = Dealerinfo.Remainamount;
                    var AdminPre = db.Remain_Admin_balance.SingleOrDefault().RemainAmount;
                    if (DealerPre >= TotalTokenValue)
                    {
                        var DealerPost = DealerPre - TotalTokenValue;
                        var AdminPost = AdminPre + TotalTokenValue;

                        entry.DealerPre = DealerPre;
                        entry.DealerPost = DealerPost;
                        entry.AdminPre = AdminPre;
                        entry.AdminPost = AdminPost;
                        entry.TotalDebit = TotalTokenValue;
                        entry.TotalTokenValue = TotalTokenValue;
                        entry.PerTokenValue = Convert.ToDecimal(Dealervalue);
                        db.RetailerCreationTokensAssignHistories.Add(entry);

                        //Update Dealer Remain Balance
                        Dealerinfo.Remainamount = DealerPost;


                        //Update Admin Remain Balance
                        var id = db.Remain_Admin_balance.SingleOrDefault();
                        id.RemainAmount = AdminPost;
                        //insert into LedgerReport values(@Retailerid,'Retailer', getdate(), 'New Retailer Create', -@JoiningCharge, 0, -@JoiningCharge, -@JoiningCharge)
                        LedgerReport ledger = new LedgerReport();
                        ledger.UserId = userid;
                        ledger.Role = "Dealer";
                        ledger.Particulars = "Token Purchase";
                        ledger.UserRemainAmount = DealerPost;
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

                        TempData["success"] = "Token Debited Successfully.";
                    }
                    else
                    {
                        TempData["error"] = "Dealer Remain Value is Low.";
                        return RedirectToAction("TokenPurchase");
                    }
                }
                else
                {
                    TempData["error"] = "Please Set the Dealer token Value.";
                    return RedirectToAction("TokenPurchase");
                }
                db.SaveChanges();
                return RedirectToAction("TokenPurchase");
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return RedirectToAction("TokenPurchase");
            }
        }
        #endregion

        #region TDSReport
        /// <summary>
        /// Displays the TDS (Tax Deducted at Source) report.
        /// </summary>
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
            model.TDSDealer = db.TDS_Report_Dealer(Convert.ToDateTime(txt_frm_date), Convert.ToDateTime(to_date), userid).ToList();
            return View(model);
        }
        /// <summary>
        /// Displays the TDS (Tax Deducted at Source) report.
        /// </summary>
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
            model.TDSDealer = db.TDS_Report_Dealer(Convert.ToDateTime(txt_frm_date), Convert.ToDateTime(to_date), useridid).ToList();

            ViewBag.CurrentMonthName = CurrentMonthName;
            ViewBag.OldMonth1 = OldMonth1;
            ViewBag.OldMonth2 = OldMonth2;
            ViewBag.Crmonth = submit;

            return View(model);
        }
        #endregion
        #region GSTReport
        /// <summary>
        /// Displays the GST compliance report.
        /// </summary>
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
            model.GSTDealer = db.GST_Report_Dealer_All(Convert.ToDateTime(txt_frm_date), Convert.ToDateTime(to_date), userid).ToList();
            return View(model);
        }
        /// <summary>
        /// Displays the GST compliance report.
        /// </summary>
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
            model.GSTDealer = db.GST_Report_Dealer_All(Convert.ToDateTime(txt_frm_date), Convert.ToDateTime(to_date), userid).ToList();
            ViewBag.CurrentMonthName = CurrentMonthName;
            ViewBag.OldMonth1 = OldMonth1;
            ViewBag.OldMonth2 = OldMonth2;
            ViewBag.Crmonth = submit;

            return View(model);
        }

        #endregion
        //End
        #region Dealer Gst Invocie Report
        /// <summary>
        /// Displays the dealer GST invoicing report.
        /// </summary>
        public ActionResult Gst_Invocing_Dealer_report()
        {
            var userid = User.Identity.GetUserId();
            DateTime txt_frm_date = DateTime.Now;
            DateTime to_date = DateTime.Now;
            DateTime date = DateTime.Now;
            string OldMonth = date.AddMonths(-1).ToString("MMMM");

            ViewBag.OldMonth = OldMonth;
            txt_frm_date = new DateTime(date.AddMonths(-1).Year, date.AddMonths(-1).Month, 1);
            to_date = new DateTime(date.Year, date.Month, 1);
            var show = db.GST_Monthly_Dealer(Convert.ToDateTime(txt_frm_date), Convert.ToDateTime(to_date)).Where(a => a.apiid == userid).ToList();
            return View(show);
        }

        /// <summary>
        /// Generates the dealer GST invoicing report as a PDF.
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

            var entries = db.GST_Monthly_Dealer(Convert.ToDateTime(txt_frm_date), Convert.ToDateTime(to_date)).Where(a => a.apiid == userid).ToList();
            ViewBag.name = entries.SingleOrDefault().DealerName;
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
            var admininfo = db.Admin_details.FirstOrDefault();
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
        #region DTH BooKing
        /// <summary>
        /// Displays the DTH recharge booking report.
        /// </summary>
        public ActionResult DthBookingReport()
        {
            var userid = User.Identity.GetUserId();
            string txt_frm_date = DateTime.Now.ToString();
            string txt_to_date = DateTime.Now.ToString();
            string frm_date = Convert.ToDateTime(txt_frm_date).Date.ToString("yyyy-MM-dd");
            string to_date = Convert.ToDateTime(txt_to_date).AddDays(1).ToString("yyyy-MM-dd");
            var ch = db.DTHConnection_Report_New("Dealer", userid, "ALL", 50, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            ViewData["totals"] = ch.Where(s => s.Status == "Success").Sum(s => Convert.ToInt32(s.Amount));
            ViewData["Totalf"] = ch.Where(s => s.Status == "Failed").Sum(s => Convert.ToInt32(s.Amount));
            return View(ch);
        }
        /// <summary>
        /// Displays the DTH recharge booking report.
        /// </summary>
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
            var ch = db.DTHConnection_Report_New("Dealer", userid, ddl_status, ddltop, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            ViewData["totals"] = ch.Where(s => s.Status == "Success").Sum(s => Convert.ToInt32(s.Amount));
            ViewData["Totalf"] = ch.Where(s => s.Status == "Failed").Sum(s => Convert.ToInt32(s.Amount));
            return View(ch);
        }

        #endregion

        /// <summary>
        /// Displays the Micro ATM rental charge report for the dealer.
        /// </summary>
        public ActionResult Dealer_Microatm_rental_report()
        {
            var userid = User.Identity.GetUserId();
            string txt_to_date1 = DateTime.Now.ToString();
            string frm_date = Convert.ToDateTime(txt_to_date1).AddDays(-1).ToShortDateString();
            string to_date = Convert.ToDateTime(txt_to_date1).ToShortDateString();
            var ch = db.microatm_rental_report(null, userid, null, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            return View(ch);
        }

        /// <summary>
        /// Displays the Micro ATM rental charge report for the dealer.
        /// </summary>
        [HttpPost]
        public ActionResult Dealer_Microatm_rental_report(DateTime txt_frm_date, DateTime txt_to_date)
        {
            var userid = User.Identity.GetUserId();
            ViewBag.chk = "post";
            //txt_to_date = txt_to_date.AddDays(1);
            var ch = db.microatm_rental_report(null, userid, null, Convert.ToDateTime(txt_frm_date), Convert.ToDateTime(txt_to_date)).ToList();

            return View(ch);
        }
        /// <summary>
        /// Displays and processes dealer GST sell report.
        /// </summary>
        public ActionResult DealerSellgst()
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
            var respp = db.dlm_purchase_info(userid, monthname, currentyear).ToList();
            IEnumerable<SelectListItem> selectList = from s in respp
                                                     select new SelectListItem
                                                     {
                                                         Value = s.DealerId,
                                                         Text = s.FarmName.ToString()
                                                     };

            ViewBag.apiid = new SelectList(selectList, "Value", "Text");
            return View(respp);
        }
        /// <summary>
        /// Displays and processes dealer GST sell report.
        /// </summary>
        [HttpPost]
        public ActionResult DealerSellgst(string Month, string Year)
        {
            string dealerid = User.Identity.GetUserId();
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
            var respp = db.dlm_purchase_info(dealerid, Month, Year).ToList();
            IEnumerable<SelectListItem> selectList = from s in respp
                                                     select new SelectListItem
                                                     {
                                                         Value = s.DealerId,
                                                         Text = s.FarmName.ToString()
                                                     };

            ViewBag.apiid = new SelectList(selectList, "Value", "Text");
            return View(respp);
        }
        /// <summary>
        /// Prints the dealer GST sell invoice.
        /// </summary>
        public ActionResult Print_Sell_Gst_dealer(string Month, string Year)
        {
            var dlmid = User.Identity.GetUserId();
            var userid = dlmid;
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

            var userinfo = db.Dealer_Details.Where(aa => aa.DealerId == dlmid).SingleOrDefault();
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
            var entries = db.dealer_purchase_gst(userid, Month, Year).ToList();
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
            if (userinfo.FinincialRolesType == "FieldSalesOfficer")
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

            return new ViewAsPdf("Print_Sell_Gst_dealer", entries);
        }
        /// <summary>
        /// Displays the GST document upload page.
        /// </summary>
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
            var chk = db.Dealer_upload_gst.Where(aa => aa.dealerid == userid && aa.monthchk == month && aa.yearchk == year).ToList();
            return View(chk);
        }
        /// <summary>
        /// Displays the GST document upload page.
        /// </summary>
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
            var chk = db.Dealer_upload_gst.Where(aa => aa.dealerid == userid && aa.monthchk == Month && aa.yearchk == Year).ToList();
            return View(chk);
        }
        /// <summary>
        /// Handles the GST file upload submission.
        /// </summary>
        public ActionResult uploadgstfile()
        {
            var userid = User.Identity.GetUserId();
            if (Request.Files.AllKeys.Any())
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
                            fname = Path.Combine(Server.MapPath("~/UploadGst/Dealer"), fname);
                            file.SaveAs(fname);
                            var chk = db.Dealer_upload_gst.Where(aa => aa.dealerid == userid && aa.monthchk == Month && aa.yearchk == Year && (aa.status == "Approved" || aa.status == "Pending")).SingleOrDefault();
                            if (chk == null)
                            {
                                Dealer_upload_gst up = new Dealer_upload_gst();
                                up.monthchk = Month;
                                up.dealerid = userid;
                                up.status = "Pending";
                                up.uploadfile = "UploadGst/Dealer/" + filenm;
                                up.yearchk = Year;
                                db.Dealer_upload_gst.Add(up);
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


        /// <summary>
        /// Displays the payment gateway transfer report.
        /// </summary>
        public ActionResult GatewayTRANSFER()
        {
            ViewBag.msg = TempData["msg"];
            TempData.Remove("msg");
            var userid = User.Identity.GetUserId();
            DateTime from = DateTime.Now.Date;
            DateTime to = DateTime.Now.AddDays(1).Date;

            var chk = db.gateway_report(userid, "Dealer", from, to, "");

            var getwaytotals = db.Payment_Gateway_Txn_history.Where(x => x.userid == userid && x.roles == "Dealer" && x.f_date >= from && x.f_date <= to);
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

        /// <summary>
        /// Displays the payment gateway transfer report.
        /// </summary>
        [HttpPost]
        public ActionResult GatewayTRANSFER(DateTime txt_frm_date, DateTime txt_to_date)
        {

            ViewBag.chk = "post";
            var userid = User.Identity.GetUserId();
            DateTime to = txt_to_date.AddDays(1).Date;

            var chk = db.gateway_report(userid, "Dealer", txt_frm_date, to, "");

            var getwaytotals = db.Payment_Gateway_Txn_history.Where(x => x.userid == userid && x.roles == "Dealer" && x.f_date >= txt_frm_date && x.f_date <= to);
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

        /// <summary>
        /// Generates the gateway transfer report as a PDF.
        /// </summary>
        public ActionResult PDFGatewayTRANSFER(DateTime txt_frm_date, DateTime txt_to_date)
        {

            ViewBag.chk = "post";
            var userid = User.Identity.GetUserId();
            DateTime to = txt_to_date.AddDays(1).Date;

            var chk = db.gateway_report(userid, "Dealer", txt_frm_date, to, "");

            var getwaytotals = db.Payment_Gateway_Txn_history.Where(x => x.userid == userid && x.roles == "Dealer" && x.f_date >= txt_frm_date && x.f_date <= to);
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

        /// <summary>
        /// Exports the gateway transfer report to Excel.
        /// </summary>
        public ActionResult Execel_Gateway_TRANSFER(DateTime txt_frm_date, DateTime txt_to_date)
        {

            var userid = User.Identity.GetUserId();
            DateTime to = txt_to_date.AddDays(1).Date;

            var chk = db.gateway_report(userid, "Dealer", txt_frm_date, to, "").ToList();
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


        /// <summary>
        /// Displays the new gateway transfer management page.
        /// </summary>
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
            var checKYC = db.Dealer_Details.Where(x => x.DealerId == userid).SingleOrDefault();
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
                    var msg = db.PaymentGateway_Fund_insert("Dealer", userid, txtamt, txnid, ddl_type,"", output).SingleOrDefault().msg;
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
                        var DealerName = checKYC.DealerName;
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


        #region Prepaid and DTH Slab Setting
        /// <summary>
        /// Displays and manages prepaid commission slab settings.
        /// </summary>
        public ActionResult Prepaid_Slab_setting()
        {
            var userid = User.Identity.GetUserId();
            var masterid = db.Dealer_Details.Where(aa => aa.DealerId == userid).SingleOrDefault();
            var info = db.Master_allow_commission.Where(aa => aa.masterid == masterid.SSId).SingleOrDefault().allowcommision;
            if (info == true)
            {
                PrepaidSlab model = new PrepaidSlab();
                model.AmountRange = (from p in db.Recharge_Amount_range
                                     select new Recharge_Amount_range_info
                                     {
                                         idno = p.idno,
                                         maximum1 = p.maximum1,
                                         maximum2 = p.maximum2
                                     }).ToList();
                model.common =
               (from p in db.prepaid_Retailer_common_comm.Where(aa => aa.userid == userid)
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
                return RedirectToAction("All_Operator_Info");
            }
        }
        /// <summary>
        /// Returns the prepaid commission partial view.
        /// </summary>
        [HttpPost]
        public ActionResult _prepaid_comm(PrepaidSlab model)
        {
            var userid = User.Identity.GetUserId();
            foreach (var item in model.common)
            {
                var entry = db.prepaid_Retailer_common_comm.Find(item.idno);
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
           (from p in db.prepaid_Retailer_common_comm.Where(aa => aa.userid == userid)
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
        /// <summary>
        /// Returns the user prepaid slab partial view.
        /// </summary>
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
                       (from m in db.prepaid_retailer_comm_by_dealer.Where(aa => aa.dealerid == masterid)
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
                        var list = db.Retailer_Details.Where(aa => aa.DealerId == masterid && aa.ISDeleteuser == false).Select(a => new SelectListItem { Text = a.Frm_Name, Value = a.RetailerId, Selected = a.RetailerId == userid ? true : false }).ToList();
                        model1.UserId = list;
                        model1.RetailerId = Provinces;
                        model1.Email = "";
                        model1.Name = "";
                        model1.Phone = "";


                    }
                    else
                    {
                        model1.Dealeruser =
                         (from m in db.prepaid_retailer_comm_by_dealer.Where(aa => aa.userid == userid)
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
                        var dlm = db.Retailer_Details.Where(aa => aa.DealerId == masterid && aa.ISDeleteuser == false).ToList();
                        List<SelectListItem> Provinces = new List<SelectListItem>();
                        var list = dlm.Select(a => new SelectListItem { Text = a.Frm_Name, Value = a.RetailerId, Selected = a.RetailerId == userid ? true : false }).ToList();
                        model1.UserId = list;
                        model1.RetailerId = Provinces;
                        model1.Email = dlm.Where(aa => aa.RetailerId == userid).SingleOrDefault().Email;
                        model1.Name = dlm.Where(aa => aa.RetailerId == userid).SingleOrDefault().RetailerName;
                        model1.Phone = dlm.Where(aa => aa.RetailerId == userid).SingleOrDefault().Mobile;
                    }
                    model1.UpdateDealer =
                     (from com in db.prepaid_Retailer_common_comm.Where(aa => aa.userid == masterid)
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
                                var entry = db.prepaid_retailer_comm_by_dealer.Where(aa => aa.dealerid == masterid).Single(a => a.userid == ddlUserId && a.optcode == item.optcode);
                                entry.comm = item.comm;
                                entry.comm1 = item.comm1;
                                entry.comm2 = item.comm2;
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
                            var dlmlist = db.Retailer_Details.Where(aa => aa.DealerId == masterid && aa.ISDeleteuser == false).Select(x => x.RetailerId).ToList();
                            model2.UpdateDealer.ToList().ForEach(aa =>
                            {
                                db.prepaid_retailer_comm_by_dealer.Where(w => w.optcode == aa.optcode && dlmlist.Contains(w.userid) && w.dealerid == masterid).ToList()
                                .ForEach(i =>
                                { i.comm = aa.comm; i.comm1 = aa.comm1; i.comm2 = aa.comm2; });
                                if (Button == "Update Existing & New Users")
                                {
                                    db.prepaid_Retailer_common_comm.Where(chn => chn.optcode == aa.optcode && chn.userid == masterid).ToList()
                                    .ForEach(i =>
                                    { i.comm = aa.comm; i.comm1 = aa.comm1; i.comm2 = aa.comm2; });
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
                    (from m in db.prepaid_retailer_comm_by_dealer.Where(aa => aa.userid == masterid)
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
                    var list = db.Retailer_Details.Where(aa => aa.DealerId == masterid && aa.ISDeleteuser == false).Select(a => new SelectListItem { Text = a.Frm_Name, Value = a.RetailerId, Selected = a.RetailerId == userid ? true : false }).ToList();
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
                     (from m in db.prepaid_retailer_comm_by_dealer.Where(aa => aa.dealerid == masterid)
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
                    var dlm = db.Retailer_Details.Where(aa => aa.DealerId == masterid && aa.ISDeleteuser == false).ToList();
                    var list = dlm.Select(a => new SelectListItem { Text = a.Frm_Name, Value = a.RetailerId, Selected = a.RetailerId == userid ? true : false }).ToList();
                    model.UserId = list;
                    List<SelectListItem> Provinces = new List<SelectListItem>();
                    model.RetailerId = Provinces;
                    model.Email = dlm.Where(aa => aa.RetailerId == userid).SingleOrDefault().Email;
                    model.Name = dlm.Where(aa => aa.RetailerId == userid).SingleOrDefault().RetailerName;
                    model.Phone = dlm.Where(aa => aa.RetailerId == userid).SingleOrDefault().Mobile;
                }
                model.UpdateDealer =
                 (from com in db.prepaid_Retailer_common_comm.Where(aa => aa.userid == masterid)
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



        #region Dealer Seller 
        /// <summary>
        /// Displays and manages dealer seller configurations.
        /// </summary>
        public ActionResult Dealer_Seller()
        {
            var userid = User.Identity.GetUserId();
            var descdetail = (from a in db.dealer_seller_comm
                              join c in db.Operator_Code on a.OperatorCode equals c.new_opt_code.ToString()
                              where a.dlmid == userid
                              select new
                              {
                                  idno = a.idno,
                                  dlmid = a.dlmid,
                                  OperatorCode = a.OperatorCode,
                                  sellcomm = a.sellcomm,
                                  status = a.status,
                                  insertdate = a.insertdate,
                                  adminapproval_sts = a.adminapproval_sts,
                                  adminapproval_date = a.adminapproval_date,
                                  operator_Name = c.operator_Name
                              }).ToList().OrderByDescending(a => a.insertdate);
            ViewBag.sellercomm = descdetail;
            var optcode = db.Operator_Code.Where(a => a.Operator_type == "PrePaid" || a.Operator_type == "DTH").ToList();
            ViewBag.opcodelist = new SelectList(optcode, "new_opt_code", "operator_Name", null);
            return View();
        }

        /// <summary>
        /// Adds a commission setting for a dealer seller.
        /// </summary>
        [HttpPost]
        public ActionResult AddSeller_Commission(string optcode, decimal? Sellcomm = 0)
        {
            try
            {
                var userid = User.Identity.GetUserId();
                var isexistsopt = db.dealer_seller_comm.Where(a => a.dlmid == userid && a.OperatorCode == optcode && a.adminapproval_sts != "Rejected").Count();
                if (isexistsopt == 0)
                {
                    dealer_seller_comm sellercomm = new dealer_seller_comm();
                    sellercomm.dlmid = userid;
                    sellercomm.OperatorCode = optcode;
                    sellercomm.sellcomm = Sellcomm;
                    sellercomm.status = false;
                    sellercomm.insertdate = DateTime.Now;
                    sellercomm.adminapproval_sts = "Pending";
                    db.dealer_seller_comm.Add(sellercomm);
                    db.SaveChanges();
                }

                return RedirectToAction("Dealer_Seller");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Dealer_Seller");
            }
        }
        /// <summary>
        /// Changes the status of a seller commission entry.
        /// </summary>
        public ActionResult ChangeStatusSeller_Commission(int? idno)
        {
            var dealerscomm = db.dealer_seller_comm.Find(idno);
            if (dealerscomm != null)
            {
                var changests = dealerscomm.status == true ? false : true;
                dealerscomm.status = changests;
                db.SaveChanges();

            }
            return Json(dealerscomm.status, JsonRequestBehavior.AllowGet);

        }
        /// <summary>
        /// Updates a specific seller commission entry by ID.
        /// </summary>
        public ActionResult Seller_CommissionChangeByID(int? id)
        {
            var userid = User.Identity.GetUserId();
            ViewBag.sellercommChargeByID = db.dealer_seller_comm.Where(a => a.idno == id).SingleOrDefault();
            var descdetail = (from a in db.dealer_seller_comm
                              join c in db.Operator_Code on a.OperatorCode equals c.new_opt_code.ToString()
                              where a.dlmid == userid
                              select new
                              {
                                  idno = a.idno,
                                  dlmid = a.dlmid,
                                  OperatorCode = a.OperatorCode,
                                  sellcomm = a.sellcomm,
                                  status = a.status,
                                  insertdate = a.insertdate,
                                  adminapproval_sts = a.adminapproval_sts,
                                  adminapproval_date = a.adminapproval_date,
                                  operator_Name = c.operator_Name
                              }).ToList().OrderByDescending(a => a.insertdate);
            ViewBag.sellercomm = descdetail;
            return PartialView("_Edit_Seller_Commission");
        }

        /// <summary>
        /// Saves updated commission values for a seller.
        /// </summary>
        [HttpPost]
        public ActionResult UpdateSeller_Commission(int? Idno, decimal? Sellcomm = 0)
        {
            try
            {
                var userid = User.Identity.GetUserId();
                var entry = db.dealer_seller_comm.SingleOrDefault(a => a.idno == Idno);
                entry.sellcomm = Sellcomm;
                entry.insertdate = DateTime.Now;
                db.SaveChanges();

                ViewBag.sellercommChargeByID = db.dealer_seller_comm.Where(a => a.idno == Idno).SingleOrDefault();
                var descdetail = (from a in db.dealer_seller_comm
                                  join c in db.Operator_Code on a.OperatorCode equals c.new_opt_code.ToString()
                                  where a.dlmid == userid
                                  select new
                                  {
                                      idno = a.idno,
                                      dlmid = a.dlmid,
                                      OperatorCode = a.OperatorCode,
                                      sellcomm = a.sellcomm,
                                      status = a.status,
                                      insertdate = a.insertdate,
                                      adminapproval_sts = a.adminapproval_sts,
                                      adminapproval_date = a.adminapproval_date,
                                      operator_Name = c.operator_Name
                                  }).ToList().OrderByDescending(a => a.insertdate);
                ViewBag.sellercomm = descdetail;
                ViewBag.isReport = "1";
            }
            catch
            { }
            return PartialView("_Edit_Seller_Commission");
        }
        /// <summary>
        /// Adds a new SIM card entry for the dealer.
        /// </summary>
        public ActionResult AddSIM()
        {           
            var dealerid = User.Identity.GetUserId();
            SimDetail_Info d1 = new SimDetail_Info();
            d1.data1 = db.dealer_sim_new.Where(s => s.dlmid == dealerid).OrderByDescending(s=>s.idno).ToList();
            d1.message = "";
            return View(d1);
        }
       [HttpPost]
       public object EditSIMINFO(int? idno)
        {
            var chk = db.dealer_sim_new.Where(s => s.idno == idno).ToList();
            var rchpin = "";
            if (string.IsNullOrEmpty(chk[0].RechargePin))
            {
                rchpin = chk[0].RechargePin;
            }
            else
            {
                rchpin = Decrypt(chk[0].RechargePin);
            }
            return Json(new {list= chk , lst1 = Decrypt(chk[0].Password) , list2 = rchpin }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Edits an existing SIM card information entry.
        /// </summary>
        [HttpPost]
        public ActionResult EditSIMINFO1( string Simnumber,string Password, string Circlecode,string RechargePin, string Imeino, string minbal , string Daylimit,string Operator, string posid, string Macaddress)
        {        SimDetail_Info d1 = new SimDetail_Info();
                var dealerid = User.Identity.GetUserId();
            try
            {
                if (Operator == "BSNL Topup")
                {
                    Operator = "B";
                }
                else
                {

                    Operator = db.operatorcommforsells.Where(s => s.optname == Operator).SingleOrDefault().optcode;
                }

                var check1 = db.Money_API_URLS.Where(s => s.API_Name.Contains("VASTWEB")).FirstOrDefault();
                var checkdlm = db.Dealer_Details.Where(s => s.DealerId == dealerid).SingleOrDefault();
                var client = new RestClient("https://www.vastwebindia.com/DLMAPI/EditSimRegister");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                request.AddParameter("RegisterEmail", (check1.API_ID != null) ? check1.API_ID : "");
                request.AddParameter("Email", (checkdlm.Email != null) ? checkdlm.Email : "");
                request.AddParameter("Simnumber", (Simnumber != null) ? Simnumber : "");
                request.AddParameter("Password", (Password != null) ? Password : "");
                request.AddParameter("Circlecode", (Circlecode != null) ? Circlecode : "");
                request.AddParameter("RechargePin", (RechargePin != null) ? RechargePin : "");
                request.AddParameter("Imeino", (Imeino != null) ? Imeino : "");
                request.AddParameter("minbal", (minbal != null) ? minbal : "");
                request.AddParameter("Daylimit", (Daylimit != null) ? Daylimit : "");
                request.AddParameter("Operator", Operator);
                request.AddParameter("posid", (posid != null) ? posid : "");
                request.AddParameter("Macaddress", (Macaddress != null) ? Macaddress : "");


                var response = client.Execute(request);
                dynamic json = JsonConvert.DeserializeObject(response.Content);

                if (json.Responsecode == 1)
                {
                    if (Operator == "B")
                    {
                        Operator = "BSNL Topup";
                    }
                    var chk2 = db.dealer_sim_new.Where(s => s.dlmid == dealerid && s.opt_code == Operator && s.USERID == Simnumber).SingleOrDefault();
                    chk2.Password = Encrypt(Password);
                    chk2.Zone = Circlecode;
                    chk2.RechargePin = (string.IsNullOrEmpty(RechargePin))?RechargePin : Encrypt(RechargePin);
                    chk2.Imeino = Imeino;
                    chk2.Etopminbal = minbal;
                    chk2.DaylimitMax = Convert.ToInt32(Daylimit);
                    chk2.opt_code = Operator;
                    chk2.posid = posid;
                    chk2.Macaddress = Macaddress;
                    db.SaveChanges();

                    //return Json(new
                    //{
                    //    success = false
                    //});

                    d1.data1 = db.dealer_sim_new.Where(s => s.dlmid == dealerid).OrderByDescending(s => s.idno).ToList();
                    d1.message = json.Message;
                    return PartialView("_SimINfo", d1);
                }
                else
                {
                    d1.data1 = db.dealer_sim_new.Where(s => s.dlmid == dealerid).OrderByDescending(s => s.idno).ToList();
                    d1.message = json.Message;
                    return PartialView("_SimINfo", d1);

                }
            }
            catch
            {
                d1.data1 = db.dealer_sim_new.Where(s => s.dlmid == dealerid).OrderByDescending(s => s.idno).ToList();
                d1.message = "Something Went Wrong";
                return PartialView("_SimINfo", d1);
            }

              
        }
        /// <summary>
        /// Toggles SIM card on/off status.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> simonoff(string switch1, int? idno)
        {  var messege = "";
            try
                {
                if (!string.IsNullOrEmpty(switch1) && idno != null)
                {
                    VastBazaartoken Responsetoken = new VastBazaartoken();
                    var client21 = new RestClient("http://API.VASTBAZAAR.COM/api/Web/Siminfosts");

                    var request21 = new RestRequest(Method.POST);
                    var token21 = Responsetoken.gettoken();
                    request21.AddHeader("authorization", "bearer " + token21);
                    // request21.AddHeader("content-type", "application/json");

                    var response21 = client21.Execute(request21);
                    dynamic json21 = JsonConvert.DeserializeObject(response21.Content);
                    var statsus = json21.Content.ADDINFO.Status;
                    if (statsus == true)
                    {
                        var drop21 = "";
                        var chek = db.dealer_sim_new.Where(s => s.idno == idno).SingleOrDefault();
                        if (chek.opt_code == "BSNL Topup")
                        {
                            drop21 = "B";
                        }
                        else
                        {
                            drop21 = db.operatorcommforsells.Where(s => s.optname == chek.opt_code).SingleOrDefault().optcode;
                        }
                        if (switch1 == "OFF")
                        {
                            var client1 = new RestClient("https://www.vastwebindia.com/DLMAPI/UpdateStatus?Simnumber=" + chek.USERID + "&Operator=" + drop21 + "&currentsts=" + chek.status);
                            var request1 = new RestRequest(Method.POST);
                            request1.AddHeader("Content-Type", "application/json"); // Add headers if necessary

                            // Execute the request asynchronously
                            var response1 = client1.Execute(request1);
                            dynamic json1 = JsonConvert.DeserializeObject(response1.Content);
                            if (json1.Responsecode == 1)
                            {
                                chek.status = false;
                            }
                            messege = json1.Message;
                        }
                        else if (switch1 == "ON")
                        {
                            var client2 = new RestClient("https://www.vastwebindia.com/DLMAPI/UpdateStatus?Simnumber=" + chek.USERID + "&Operator=" + drop21 + "&currentsts=" + chek.status);
                            var request2 = new RestRequest(Method.POST);
                            request2.AddHeader("Content-Type", "application/json"); // Add headers if necessary

                            // Execute the request asynchronously
                            var response2 = client2.Execute(request2);
                            dynamic json2 = JsonConvert.DeserializeObject(response2.Content);
                            if (json2.Responsecode == 1)
                            {
                                chek.status = true;
                            }
                            messege = json2.Message;
                        }
                        else if (switch1 == "DELETE")
                        {
                            var client = new RestClient("https://www.vastwebindia.com/DLMAPI/DeleteSiminfo?Simnumber=" + chek.USERID + "&Operator=" + drop21);
                            var request = new RestRequest(Method.POST);
                            request.AddHeader("Content-Type", "application/json"); // Add headers if necessary

                            // Execute the request asynchronously
                            var response = client.Execute(request);
                            dynamic json = JsonConvert.DeserializeObject(response.Content);
                            if (json.Responsecode == 1)
                            {
                                db.dealer_sim_new.Remove(chek);

                            }


                            messege = json.Message;
                        }

                        db.SaveChanges();
                    }
                    else
                    {
                        messege = "Current Status is Off , Contact to Admin";
                    }

                    }
            else
                    {
                        messege = "SomeThing Went Wrong";
                    }

                    SimDetail_Info d1 = new SimDetail_Info();
                    var dealerid = User.Identity.GetUserId();
                    d1.data1 = db.dealer_sim_new.Where(s => s.dlmid == dealerid).OrderByDescending(s => s.idno).ToList();
                    d1.message = messege;
                    return PartialView("_SimINfo", d1);
                }
            catch (Exception ex)
            {
                messege = "SomeThing Went Wrong";
                SimDetail_Info d11 = new SimDetail_Info();
                var dealerid1 = User.Identity.GetUserId();
                d11.data1 = db.dealer_sim_new.Where(s => s.dlmid == dealerid1).OrderByDescending(s => s.idno).ToList();
                d11.message = messege;
                return PartialView("_SimINfo", d11);
            }
        }
        /// <summary>
        /// Checks the balance for a SIM card entry.
        /// </summary>
        [HttpPost]
        public ActionResult CheckbalSiminfo(int idno)
        {
            var userid = User.Identity.GetUserId();
            var chk = db.dealer_sim_new.Where(s => s.idno == idno).SingleOrDefault();
            if(chk!=null)
            {
                var chk2 = "";
                if (chk.opt_code == "BSNL Topup")
                {
                    chk2 = "B";
                }
                else
                {
                    chk2 = db.operatorcommforsells.Where(s => s.optname == chk.opt_code).SingleOrDefault().optcode;
                }
                var check1 = db.Money_API_URLS.Where(s => s.API_Name.Contains("VASTWEB")).FirstOrDefault();
                var dlmemail = db.Dealer_Details.Where(s => s.DealerId == userid).SingleOrDefault().Email;
                var client2 = new RestClient("https://www.vastwebindia.com/DLMAPI/CheckBalance?OptCode=" + chk2 + "&userid=" + chk.USERID + "&Email=" + dlmemail + "&Operator=" + chk2 + "&Registeremail=" + check1.API_ID);
                var request2 = new RestRequest(Method.POST);
                request2.AddHeader("Content-Type", "application/json"); // Add headers if necessary

                // Execute the request asynchronously
                var response2 = client2.Execute(request2);
                dynamic json2 = JsonConvert.DeserializeObject(response2.Content);
                string message = json2.Message;
                return Json(new { list = message }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { list = "Please Try After Sometime" }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public object Loginbysiminfo(int ? idno)
        {
            try
            {
                VastBazaartoken Responsetoken = new VastBazaartoken();
                var client21 = new RestClient("http://API.VASTBAZAAR.COM/api/Web/Siminfosts");
              
                var request21 = new RestRequest(Method.POST);
                var token21 = Responsetoken.gettoken();
                request21.AddHeader("authorization", "bearer " + token21);
               // request21.AddHeader("content-type", "application/json");

                var response21 = client21.Execute(request21);
                dynamic json21 = JsonConvert.DeserializeObject(response21.Content);
                var statsus = json21.Content.ADDINFO.Status;
                if (statsus == true)
                {
                    var chk = db.dealer_sim_new.Where(s => s.idno == idno).ToList();
                    if (chk.Any())
                    {
                        var chk1 = db.dealer_sim_new.Where(s => s.idno == idno).SingleOrDefault();
                        var chk2 = "";
                        if (chk1.opt_code == "BSNL Topup")
                        {
                            chk2 = "B";
                        }
                        else
                        {
                            chk2 = db.operatorcommforsells.Where(s => s.optname == chk1.opt_code).SingleOrDefault().optcode;
                        }
                        var check1 = db.Money_API_URLS.Where(s => s.API_Name.Contains("VASTWEB")).FirstOrDefault();

                        var client2 = new RestClient("https://www.vastwebindia.com/DLMAPI/LoginSimInfo?loginid=" + chk1.USERID + "&password=" + Decrypt(chk1.Password) + "&imeino=" + chk1.Imeino + "&Operator=" + chk2 + "&Registeremail=" + check1.API_ID);
                        var request2 = new RestRequest(Method.POST);
                        request2.AddHeader("Content-Type", "application/json"); // Add headers if necessary

                        // Execute the request asynchronously
                        var response2 = client2.Execute(request2);
                        dynamic json2 = JsonConvert.DeserializeObject(response2.Content);

                        string message = json2.Message;
                        var mess = "Otp Sent Successfully";
                        if (message.ToUpper() == mess.ToUpper())
                        {
                            message = "OTP";
                        }

                        return Json(new { list = message }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { list = "Something Went Wrong" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { list = "Current Status is Off , Contact to Admin" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { list = "Please Try Again" }, JsonRequestBehavior.AllowGet);
            }
        }  
        [HttpPost]
        public object verifysimotp(string otp , int idno)
        {
            var chk1 = db.dealer_sim_new.Where(s => s.idno == idno).SingleOrDefault();
            var chk2 = "";
            if (chk1.opt_code == "BSNL Topup")
            {
                chk2 = "B";
            }
            else
            {
                chk2 = db.operatorcommforsells.Where(s => s.optname == chk1.opt_code).SingleOrDefault().optcode;
            }
            var check1 = db.Money_API_URLS.Where(s => s.API_Name.Contains("VASTWEB")).FirstOrDefault();
            var client = new RestClient("https://www.vastwebindia.com/DLMAPI/Verifyotp?otp=" + otp +"&userid=" + chk1.USERID + "&optcode=" + chk2 + "&registeremail="+check1.API_ID);
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json"); // Add headers if necessary

            // Execute the request asynchronously
            var response = client.Execute(request);
            dynamic json = JsonConvert.DeserializeObject(response.Content);
            var message = "";
            if (json.Responsecode == 1)
            {
                message = "Done";
            }
            else
            {
                message = "No";
            }
            return Json(new { list = message }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Saves new SIM card details to the dealer account.
        /// </summary>
        [HttpPost]
        public ActionResult Add_SIM_Details(string drop2, string Zone , string USERID, string Password, string RechargePin, string Imeino, string Daylimit, string minbal, string posid, string Macaddress, string Remark   )
        { var dealerid = User.Identity.GetUserId();
            
           
            SimDetail_Info d11 = new SimDetail_Info();
            try
            {
                VastBazaartoken Responsetoken = new VastBazaartoken();
                var client21 = new RestClient("http://API.VASTBAZAAR.COM/api/Web/Siminfosts");

                var request21 = new RestRequest(Method.POST);
                var token21 = Responsetoken.gettoken();
                request21.AddHeader("authorization", "bearer " + token21);
                // request21.AddHeader("content-type", "application/json");

                var response21 = client21.Execute(request21);
                dynamic json21 = JsonConvert.DeserializeObject(response21.Content);
                var statsus = json21.Content.ADDINFO.Status;
                if (statsus == true)
                {

                    if (!string.IsNullOrEmpty(drop2))
                    {
                        var drop21 = db.operatorcommforsells.Where(s => s.optname == drop2).SingleOrDefault().optcode;
                        var checkr = db.dealer_sim_new.Where(s => s.USERID == USERID && s.opt_code == drop2).ToList();
                        if (!checkr.Any())
                        {
                            var check1 = db.Money_API_URLS.Where(s => s.API_Name.Contains("VASTWEB")).FirstOrDefault();
                            var checkdlm = db.Dealer_Details.Where(s => s.DealerId == dealerid).SingleOrDefault();
                            var client = new RestClient("https://www.vastwebindia.com/DLMAPI/SimRegister");
                            var request = new RestRequest(Method.POST);
                            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                            request.AddParameter("RegisterEmail", (check1.API_ID != null) ? check1.API_ID : "");
                            request.AddParameter("Email", (checkdlm.Email != null) ? checkdlm.Email : "");
                            request.AddParameter("Simnumber", (USERID != null) ? USERID : "");
                            request.AddParameter("Password", (Password != null) ? Password : "");
                            request.AddParameter("Circlecode", (Zone != null) ? Zone : "");
                            request.AddParameter("RechargePin", (RechargePin != null) ? RechargePin : "");
                            request.AddParameter("Imeino", (Imeino != null) ? Imeino : "");
                            request.AddParameter("minbal", (minbal != null) ? minbal : "");
                            request.AddParameter("Daylimit", (Daylimit != null) ? Daylimit : "");
                            request.AddParameter("Operator", (drop21 != null) ? drop21 : "");
                            request.AddParameter("posid", (posid != null) ? posid : "");
                            request.AddParameter("Macaddress", (Macaddress != null) ? Macaddress : "");



                            var response = client.Execute(request);
                            dynamic json = JsonConvert.DeserializeObject(response.Content);

                            if (json.Responsecode == 1)
                            {
                                dealer_sim_new d1 = new dealer_sim_new();
                                d1.dlmid = dealerid;
                                d1.opt_code = drop2;
                                d1.Zone = Zone;
                                d1.USERID = USERID;
                                d1.Password = Encrypt(Password);
                                d1.RechargePin = (string.IsNullOrEmpty(RechargePin)) ? RechargePin : Encrypt(RechargePin);
                                d1.Imeino = Imeino;
                                d1.DaylimitMax = Convert.ToInt32(Daylimit);
                                d1.Etopminbal = minbal;
                                d1.posid = posid;
                                d1.Macaddress = Macaddress;
                                d1.remaining = 0;
                                d1.status = false;
                                d1.checks = false;
                                d1.login = false;
                                d1.Remark = Remark;
                                db.dealer_sim_new.Add(d1);
                                db.SaveChanges();
                                d11.message = json.Message;
                                if (drop2 == "BSNL Recharge")
                                {
                                    dealer_sim_new d2 = new dealer_sim_new();
                                    d2.dlmid = dealerid;
                                    d2.opt_code = "BSNL Topup";
                                    d2.Zone = Zone;
                                    d2.USERID = USERID;
                                    d2.Password = Encrypt(Password);
                                    d2.RechargePin = (string.IsNullOrEmpty(RechargePin)) ? RechargePin : Encrypt(RechargePin);
                                    d2.Imeino = Imeino;
                                    d2.DaylimitMax = Convert.ToInt32(Daylimit);
                                    d2.Etopminbal = minbal;
                                    d2.posid = posid;
                                    d2.Macaddress = Macaddress;
                                    d2.remaining = 0;
                                    d2.status = false;
                                    d2.checks = false;
                                    d2.login = false;
                                    d2.Remark = Remark;
                                    db.dealer_sim_new.Add(d2);
                                    db.SaveChanges();

                                }
                            }
                        }
                    }

                }
                else
                {
                    d11.message = "Current Status is Off , Contact to Admin";
                }
                d11.data1 = db.dealer_sim_new.Where(s => s.dlmid == dealerid).OrderByDescending(s => s.idno).ToList();
              
                return PartialView("_SimINfo",d11);
            }
            catch (Exception ex)
            {
                d11.data1 = db.dealer_sim_new.Where(s => s.dlmid == dealerid).OrderByDescending(s => s.idno).ToList();
                d11.message = "Something Went Wrong";
                return PartialView("_SimINfo", d11);
            }
        }

        /// <summary>
        /// Changes the status of a seller SIM entry.
        /// </summary>
        public ActionResult ChangeStatusSeller_SIM(int? idno)
        {
            var dealersSIM = db.dealer_sim.Find(idno);
            if (dealersSIM != null)
            {
                var changests = dealersSIM.status == true ? false : true;
                dealersSIM.status = changests;
                db.SaveChanges();

            }
            return Json(dealersSIM.status, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// Updates a specific seller SIM entry by ID.
        /// </summary>
        public ActionResult Seller_SIMChangeByID(int? id)
        {
            var userid = User.Identity.GetUserId();
            ViewBag.sellerSIMChargeByID = db.dealer_sim.Where(a => a.idno == id).SingleOrDefault();
            var simdetails = (from a in db.dealer_sim
                              join c in db.Operator_Code on a.OperatorCode equals c.new_opt_code.ToString()
                              where a.dlmid == userid
                              select new
                              {
                                  idno = a.idno,
                                  dlmid = a.dlmid,
                                  OperatorCode = a.OperatorCode,
                                  dailyhome_limit = a.dailyhome_limit,
                                  dailyrom_limit = a.dailyrom_limit,
                                  circle = a.circle,
                                  simnumber = a.simnumber,
                                  simname = a.simname,
                                  userid = a.userid,
                                  password = a.password,
                                  pin = a.pin,
                                  status = a.status,
                                  remainbal = a.remainbal,
                                  totalrecharge = a.totalrecharge,
                                  insertdate = a.insertdate,
                                  purchasecomm = a.purchasecomm,
                                  operator_Name = c.operator_Name
                              }).ToList().OrderByDescending(a => a.insertdate);
            ViewBag.simdetails = simdetails;
            return PartialView("_Edit_SIM_Details");
        }

        /// <summary>
        /// Saves updated SIM information for a seller.
        /// </summary>
        [HttpPost]
        public ActionResult UpdateSeller_SIM(int? Idno, decimal? dailyhome_limit, decimal? dailyrom_limit, string circle, int? simnumber, string simname,
            string userid, string password, string pin, decimal? remainbal, decimal? totalrecharge, decimal? purchasecomm)
        {
            try
            {

                var dealerid = User.Identity.GetUserId();
                var entry = db.dealer_sim.SingleOrDefault(a => a.idno == Idno);
                entry.dailyhome_limit = dailyhome_limit;
                entry.dailyrom_limit = dailyrom_limit;
                entry.circle = circle;
                entry.simnumber = simnumber;
                entry.simname = simname;
                entry.userid = userid;
                entry.password = password;
                entry.pin = pin;
                entry.remainbal = remainbal;
                entry.totalrecharge = totalrecharge;
                entry.insertdate = DateTime.Now;
                entry.purchasecomm = purchasecomm;
                db.SaveChanges();

                ViewBag.sellerSIMChargeByID = db.dealer_sim.Where(a => a.idno == Idno).SingleOrDefault();
                var simdetails = (from a in db.dealer_sim
                                  join c in db.Operator_Code on a.OperatorCode equals c.new_opt_code.ToString()
                                  where a.dlmid == dealerid
                                  select new
                                  {
                                      idno = a.idno,
                                      dlmid = a.dlmid,
                                      OperatorCode = a.OperatorCode,
                                      dailyhome_limit = a.dailyhome_limit,
                                      dailyrom_limit = a.dailyrom_limit,
                                      circle = a.circle,
                                      simnumber = a.simnumber,
                                      simname = a.simname,
                                      userid = a.userid,
                                      password = a.password,
                                      pin = a.pin,
                                      status = a.status,
                                      remainbal = a.remainbal,
                                      totalrecharge = a.totalrecharge,
                                      insertdate = a.insertdate,
                                      purchasecomm = a.purchasecomm,
                                      operator_Name = c.operator_Name
                                  }).ToList().OrderByDescending(a => a.insertdate);
                ViewBag.simdetails = simdetails;
                ViewBag.isReport = "1";

            }
            catch (Exception ex)
            { }
            return PartialView("_Edit_SIM_Details");
        }


        #endregion



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

        /// <summary>
        /// Adds a selling operator option for the dealer.
        /// </summary>
        public ActionResult ADDSELLINGOPT()
        {
      
            var userid = User.Identity.GetUserId();
            var chk= db.operatorcommforsellStatus.Where(s => s.userid == userid).OrderByDescending(s=>s.idno).ToList();
           
            return View(chk);
        }
        /// <summary>
        /// Adds a selling operator option for the dealer.
        /// </summary>
        [HttpPost]
        public ActionResult ADDSELLINGOPT(string drop1 , string drop2, decimal comm1)
        {var userid = User.Identity.GetUserId();
            var chkd = db.Recharge_sell_by_dealer.Where(s => s.Dealerid == userid && s.registration_Status == true && s.Status == true).ToList();
            if (chkd.Any())
            {

                var chk1 = db.operatorcommforsellStatus.Where(s => s.userid == userid && s.operator_1 == drop2).ToList();
                if (!chk1.Any())
                {
                    var dlm = db.Dealer_Details.Where(s => s.DealerId == userid).SingleOrDefault();
                    operatorcommforsellStatu d1 = new operatorcommforsellStatu();
                    d1.userid = userid;
                    d1.dlm_Name = dlm.DealerName;
                    d1.farmname = dlm.FarmName;
                    d1.Email = dlm.Email;
                    d1.Timing = DateTime.Now;
                    d1.operator_1 = drop2;
                    d1.comm = comm1;
                    d1.status = "Pending";
                    db.operatorcommforsellStatus.Add(d1);
                    db.SaveChanges();
                }
                else
                {
                    var chk34 = db.operatorcommforsellStatus.Where(s => s.userid == userid && s.operator_1 == drop2).SingleOrDefault();
                    chk34.Timing = DateTime.Now;

                    chk34.comm = comm1;
                    chk34.status = "Pending";
                    db.SaveChanges();
                }

                var chk = db.operatorcommforsellStatus.Where(s => s.userid == userid).OrderByDescending(s => s.idno).ToList();
                return PartialView("optsellforrcharge", chk);
            }
            else
            {
                return RedirectToAction("Recharge_Report");
            }
        }


        public object filteroperatorforsell(string type)
        {var userid = User.Identity.GetUserId();
            var chk = db.operatorcommforsells.Where(s => s.OptType.Trim().ToUpper() == type.Trim().ToUpper()).ToList();
            return Json(new {list= chk});
        }
        public object filteroperatorforsell1()
        {
            var userid = User.Identity.GetUserId();
            var chk = db.operatorcommforsellStatus.Where(s =>s.status == "Success" && s.userid == userid).ToList();
            return Json(new { list = chk });
        }
        /// <summary>
        /// Deletes an operator commission entry.
        /// </summary>
        public ActionResult DeleteOptComm(int? idno)
        { var userid = User.Identity.GetUserId();
            if(idno != null)
            {
                var chk1 = db.operatorcommforsellStatus.Where(s => s.userid == userid && s.idno == idno).SingleOrDefault();
                db.operatorcommforsellStatus.Remove(chk1);
                db.SaveChanges();
            }
           
            var chk = db.operatorcommforsellStatus.Where(s => s.userid == userid).OrderByDescending(s => s.idno).ToList();
            return RedirectToAction("ADDSELLINGOPT");
        }
        /// <summary>
        /// Updates the recharge sell settings.
        /// </summary>
        public ActionResult Rechargesellupd(int? id, string status, string txnid)
        {
            db.recharge_sell_update(txnid, status, null, null, null);
            return RedirectToAction("sellRechargeReport");
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

        /// <summary>
        /// Displays the sell recharge transaction report.
        /// </summary>
        [HttpGet]
        public ActionResult sellRechargeReport()
        { var userid = User.Identity.GetUserId();
            var chkd = db.Recharge_sell_by_dealer.Where(s => s.Dealerid == userid && s.registration_Status == true && s.Status == true).ToList();
            if (chkd.Any())
            {
                var txt_frm_date = DateTime.Now.ToString();
                var txt_to_date = DateTime.Now.ToString();

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

                var operator_value = db.operatorcommforsells.Distinct().ToList();
                ViewBag.Operator = new SelectList(operator_value, "optname", "optname");

                var rowdata = db.Selling_Recharge_info.Where(s => s.Rch_time > frm_date && s.Rch_time < to_date && s.Dealer_id == userid).ToList();

                return View(rowdata);
            }
            else
            {
                return RedirectToAction("Recharge_Report");
            }
        }

        /// <summary>
        /// Displays the sell recharge transaction report.
        /// </summary>
        [HttpPost]
        public ActionResult sellRechargeReport(string txt_frm_date, string txt_to_date, string ddl_status, string Operator)
        {
            var userid = User.Identity.GetUserId();
            var chkd = db.Recharge_sell_by_dealer.Where(s => s.Dealerid == userid && s.registration_Status == true && s.Status == true).ToList();
            if (chkd.Any())
            {
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

                var operator_value = db.operatorcommforsells.Distinct().ToList();
                ViewBag.Operator = new SelectList(operator_value, "optname", "optname");

                var rowdata = db.Selling_Recharge_info.Where(s => s.Rch_time > frm_date && s.Rch_time < to_date && s.Dealer_id == userid).ToList();

                if (!string.IsNullOrEmpty(ddl_status) || !string.IsNullOrEmpty(Operator))
                {
                    if (!string.IsNullOrEmpty(ddl_status) && !string.IsNullOrEmpty(Operator))
                    {
                        rowdata = db.Selling_Recharge_info.Where(s => s.Rch_time > frm_date && s.Rch_time < to_date && s.Dealer_id == userid && s.Rstaus.ToUpper() == ddl_status.ToUpper() && s.optcode == Operator).ToList();
                    }
                    else if (!string.IsNullOrEmpty(ddl_status))
                    {
                        rowdata = db.Selling_Recharge_info.Where(s => s.Rch_time > frm_date && s.Rch_time < to_date && s.Dealer_id == userid && s.Rstaus.ToUpper() == ddl_status.ToUpper()).ToList();
                    }
                    else
                    {
                        rowdata = db.Selling_Recharge_info.Where(s => s.Rch_time > frm_date && s.Rch_time < to_date && s.Dealer_id == userid && s.optcode == Operator).ToList();
                    }
                }
                ViewBag.Operator = new SelectList(operator_value, "OptType", "optname");
                ViewBag.chk = "post";
                return PartialView("_sellRechargeReport", rowdata);
            }
            else
            {
                return RedirectToAction("Recharge_Report");
            }
        }
        /// <summary>
        /// Displays the extra commission report for the dealer.
        /// </summary>
        public ActionResult Extracomm_Report()
        {
            var userid = User.Identity.GetUserId();
            
                var chk = db.daywisecommsetforusers.Where(s => s.userid == userid && s.role == "Dealer").ToList();
                if (!chk.Any())
                {
                    daywisecommsetforuser d1 = new daywisecommsetforuser();
                    d1.role = "Dealer";
                    d1.userid = userid;
                    d1.Comm_2000_5000 = 0;
                    d1.Comm_5001_10000 = 0;
                    d1.Comm_10001_max = 0;
                    db.daywisecommsetforusers.Add(d1);
                    db.SaveChanges();
                }
                var dfg = db.daywisecomms.Where(s => s.dealerid == userid).OrderByDescending(s => s.date).ToList();
                return View(dfg);
         
        }
        /// <summary>
        /// Displays the extra commission report for the dealer.
        /// </summary>
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

            var dfg = db.daywisecomms.Where(s => s.dealerid == userid && s.date > frm_date && s.date < to_date).OrderByDescending(s => s.date).ToList();
            return View(dfg);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}