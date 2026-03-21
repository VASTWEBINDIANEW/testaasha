using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using Vastwebmulti.Areas.ADMIN.Models;
using Vastwebmulti.Areas.ADMIN.ViewModel;
using Vastwebmulti.Areas.API.Models;
using Vastwebmulti.Hubs;
using Vastwebmulti.Models;
namespace Vastwebmulti.Areas.API.Controllers
{
    [Authorize(Roles = "API")]
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
        // GET: API/Home
        VastwebmultiEntities db = new VastwebmultiEntities();

        AppNotification notify = new AppNotification();
        ALLSMSSend smssend = new ALLSMSSend();
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
        public ActionResult Index()
        {
            var operator_value = db.Operator_Code.Distinct().ToList();
            ViewBag.Operator = new SelectList(operator_value, "new_opt_code", "operator_Name");
            return View();
        }
        [HttpPost]
        public ActionResult Index(string txt_frm_date, string txt_to_date, string ddl_status, string Operator, string txtmob)
        {
            string userid = User.Identity.GetUserId();
            var operator_value = db.Operator_Code.Distinct().ToList();
            ViewBag.Operator = new SelectList(operator_value, "new_opt_code", "operator_Name");
            ViewBag.chk = "post";
            return View();
        }
        public ActionResult ExcelRechargereport(string txt_frm_date, string txt_to_date, string ddl_status, string Operator, string txtmob)
        {
            DataTable dtt = new DataTable("Grid");
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
            int pagesize = 100000;
            var rowdata = db.Sp_Recharge_info_LazyLoad(1, pagesize, "Retailer", userid, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), Operator, txtmob, ddl_status).ToList();



            dtt.Columns.AddRange(new DataColumn[11] { new DataColumn("status"),
                                            new DataColumn("Operator_type"),
                                             new DataColumn("Mobile"),
                                             new DataColumn("operator_Name"),
                                            new DataColumn("amount"),
                                            new DataColumn("Debit"),
                                            new DataColumn("income") ,
                                            new DataColumn("Remain") ,
                                            new DataColumn("Rch_time") ,
                                            new DataColumn("OPt_id") ,
                                            new DataColumn("refid") ,



                    });
            if (rowdata.Count > 0)
            {

                string stus = "";
                foreach (var item in rowdata)
                {
                    if (item.Rstaus.ToUpper().Contains("FAILED"))
                    {
                        stus = "FAILED";

                    }
                    if (item.Rstaus.ToUpper().Contains("REFUND"))
                    {
                        stus = "REFUND";
                    }
                    if (item.Rstaus.ToUpper().Contains("RESEND"))
                    {
                        stus = "RESEND";
                    }
                    if (item.Rstaus.ToUpper().Contains("SUCCESS"))
                    {
                        stus = "SUCCESS";
                    }
                    dtt.Rows.Add(item.Rstaus, item.Operator_type, item.Mobile, item.operator_Name, item.amount, "Debit", item.income, item.Remain, item.Rch_time, item.OPt_id, item.refid);
                }
            }
            else
            {
                dtt.Rows.Add("", "", "", "", "", "", "", "", "", "", "");
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
        public class JsonModel
        {
            public string HTMLString { get; set; }
            public bool NoMoredata { get; set; }
        }
        [ChildActionOnly]
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
            int pagesize = 1000;
            var rowdata = db.Sp_Recharge_info_LazyLoad(1, pagesize, "Retailer", userid, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), Operator, txtmob, ddl_status).ToList();
            return View(rowdata);
        }

        public ActionResult PDFRechargereport(string txt_frm_date, string txt_to_date, string ddl_status, string Operator, string txtmob)
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
            int pagesize = 100000;
            var rowdata = db.Sp_Recharge_info_LazyLoad(1, pagesize, "Retailer", userid, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), Operator, txtmob, ddl_status).ToList();
            return new ViewAsPdf(rowdata);
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
            int pagesize = 20;
            var tbrow = db.Sp_Recharge_info_LazyLoad(pageindex, pagesize, "Retailer", userid, txt_frm_date, txt_to_date, Operator, txtmob, ddl_status).ToList();
            JsonModel jsonmodel = new JsonModel();
            jsonmodel.NoMoredata = tbrow.Count < pagesize;
            if (tbrow.Count() > 0)
            {
                jsonmodel.HTMLString = renderPartialViewtostring("_Rechargereport", tbrow);
            }
            else
            {
                jsonmodel.HTMLString = "";
            }

            return Json(jsonmodel);
        }

        public ActionResult dispute(string id, string txtregion)
        {
            try
            {
                System.Data.Entity.Core.Objects.ObjectParameter output = new
         System.Data.Entity.Core.Objects.ObjectParameter("output", typeof(string));

                var ch = db.distute_insert(id, txtregion, output).SingleOrDefault().msg.ToString();
                if (ch == "Success")
                {
                    TempData["success"] = "Disputed Successfully.";
                }
                else
                {
                    TempData["failed"] = "Already Disputed!";
                }
                return RedirectToAction("Dispute_Report");
            }
            catch (Exception ex)
            {
                TempData["failed"] = ex;
                return RedirectToAction("Dispute_Report");
            }
        }

        //Dmt Report
        [HttpGet]
        public ActionResult DMT_Report()
        {
            var userid = User.Identity.GetUserId();
            string txt_frm_date = DateTime.Now.ToString();
            string txt_to_date = DateTime.Now.ToString();
            string frm_date = Convert.ToDateTime(txt_frm_date).Date.ToString("yyyy-MM-dd");
            string to_date = Convert.ToDateTime(txt_to_date).AddDays(1).ToString("yyyy-MM-dd");
            //api users 
            //ViewBag.apiid = new SelectList(db.select_apiusers_for_ddl("Admin"), "apiid", "username", null).ToList();
            //apiname
            var apiname = db.money_api_status.Distinct().ToList();
            IEnumerable<SelectListItem> selectapiname = from p in apiname
                                                        select new SelectListItem
                                                        {
                                                            Value = p.api_name,
                                                            Text = p.api_name
                                                        };
            ViewBag.apiname = new SelectList(selectapiname, "Value", "Text");
            var proc_Response = db.money_transfer_report_paging("APIID", userid, "ALL", "ALL", Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), "ALL", 1, 50000).ToList();
            return View(proc_Response);
        }
        [HttpPost]
        public ActionResult DMT_Report(string txt_frm_date, string txt_to_date, string ddl_status, string ddl_Type)
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
            var apiname = db.money_api_status.ToList();
            IEnumerable<SelectListItem> selectapiname = from p in apiname
                                                        select new SelectListItem
                                                        {
                                                            Value = p.api_name,
                                                            Text = p.api_name
                                                        };
            ViewBag.apiname = new SelectList(selectapiname, "Value", "Text");
            if (ddl_status == "Status")
            {
                ddl_status = "ALL";
            }
            var proc_Response = db.money_transfer_report_paging("APIID", userid, ddl_status, "ALL", Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), ddl_Type, 1, 50000).ToList();

            return View(proc_Response);
        }

        public ActionResult ExcelDMT_Report(string txt_frm_date, string txt_to_date, string ddl_status, string ddl_Type)
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
            var apiname = db.money_api_status.ToList();
            IEnumerable<SelectListItem> selectapiname = from p in apiname
                                                        select new SelectListItem
                                                        {
                                                            Value = p.api_name,
                                                            Text = p.api_name
                                                        };
            ViewBag.apiname = new SelectList(selectapiname, "Value", "Text");

            if (ddl_status == "Status")
            {
                ddl_status = "ALL";
            }
            var proc_Response = db.money_transfer_report_paging("APIID", userid, ddl_status, "ALL", Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), ddl_Type, 1, 50000).ToList();

            DataTable dataTbl = new DataTable();
            dataTbl.Columns.Add("Status", typeof(string));
            dataTbl.Columns.Add("Sender No", typeof(string));
            dataTbl.Columns.Add("Beneficiary Account Information", typeof(string));
            dataTbl.Columns.Add("Request Date", typeof(string));
            dataTbl.Columns.Add("Transfer+Fee=Total", typeof(string));
            dataTbl.Columns.Add("My Sharing", typeof(string));
            dataTbl.Columns.Add("GST TDS", typeof(string));
            dataTbl.Columns.Add("Net INC", typeof(string));
            dataTbl.Columns.Add("Order ID", typeof(string));
            dataTbl.Columns.Add("Bank Ref No", typeof(string));
            dataTbl.Columns.Add("Bank Name", typeof(string));
            dataTbl.Columns.Add("IFSC code", typeof(string));
            dataTbl.Columns.Add("Total Amount", typeof(string));
            dataTbl.Columns.Add("Opening Amount", typeof(string));
            dataTbl.Columns.Add("Closing Amount", typeof(string));
            //dataTbl.Columns.Add("API Request", typeof(string));
            //dataTbl.Columns.Add("API Response", typeof(string));
            dataTbl.Columns.Add("Response Date", typeof(string));

            if (proc_Response.Count > 0)
            {
                foreach (var item in proc_Response)
                {
                    dataTbl.Rows.Add(item.status, item.senderno, item.accountno, item.trans_time, item.amount,
                        item.share_api_comm, item.api_gst, item.api_comm_include_gst, item.trans_id, item.bank_trans_id,
                         item.bank_nm, item.ifsccode, item.totalamount, item.user_remain_pre, item.user_remain, /*item.api_request, item.api_response, */ item.response_time);
                }
            }
            else
            {
                dataTbl.Rows.Add("", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""/*, "", ""*/);
            }

            var grid = new GridView();
            grid.DataSource = dataTbl;
            grid.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=DMT_Report.xls");
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

        public ActionResult IMPSPDF(string Idno)
        {
            var user = User.Identity.GetUserId();
            var ch = db.pdf_creator(Idno).ToList();
            var aliid = ch.First().rch_from;
            var gg = (from hh in db.api_user_details where hh.apiid == aliid select hh).ToList();
            ViewData["apiname"] = gg.Single().username;
            ViewData["firmname"] = gg.Single().farmname;
            ViewData["apimobile"] = gg.Single().mobile;
            //ViewData["surcharge"] = ch.Single().charge;
            ViewData["surcharge"] = ch.Sum(s => s.charge);
            ViewData["Total"] = ch.Sum(s => s.amount);
            var surcharge = ch.Sum(s => s.charge);
            var tranamount = ch.Sum(s => s.amount);
            var totalamount = surcharge + tranamount;
            ViewData["Totalamount"] = totalamount;
            var ddtt = ch.First().trans_time;
            ViewData["retailerdate"] = ddtt;
            ViewData["invoiceno"] = Idno.ToUpper();
            ViewData["date"] = System.DateTime.Now;
            return new ViewAsPdf("IMPSPDF", ch);
        }
        //DMT Report End

        [HttpPost]
        public ActionResult FindTotal(string txt_frm_date, string txt_to_date, string txtmob, string ddl_status, string Operator)
        {
            var optname = "";
            var mobile = "";
            var ddlst = "";
            var ddluserid = "";

            string userid = User.Identity.GetUserId();
            ddluserid = userid;

            if ((txt_frm_date == null || txt_frm_date == "") && (txt_to_date == null || txt_to_date == ""))
            {
                txt_frm_date = DateTime.Now.ToString();
                txt_to_date = DateTime.Now.ToString();

            }
            if (ddl_status == "" || ddl_status.Contains("ALL STATUS") || Operator == null)
            {
                //optname = "ALL";
                ddlst = "";
            }
            else
            {
                ddlst = ddl_status;
            }

            if (Operator == "" || Operator.Contains("ALL OPERATORS") || Operator == null)
            {
                //optname = "ALL";
                optname = "ALL";
            }
            else
            {
                optname = Operator;
            }
            if (txtmob == "" || txtmob == null)
            {
                //mobile = "ALL";
                txtmob = "ALL";
            }
            else
            {
                mobile = txtmob;
            }
            var operator_value = db.port_list();
            ViewBag.Lapuno = new SelectList(operator_value, "PortNo", "PortNo");

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

            //if (ddl_status.ToUpper() == "PENDING")
            //{
            //    ddl_status = "REQ";
            //}
            // var chk = db.Recharge_info.Where(aa => aa.Rch_time >= frm_date && aa.Rch_time <= to_date && aa.Rstaus.Contains(ddl_status) && (Operator == "" ? aa.optcode.Contains(Operator) : aa.optcode == Operator) && aa.Mobile.Contains(txtmob)).ToList();
            var chk = db.Total_Recharge(frm_date, to_date, ddlst, "ALL", optname, "API", ddluserid).ToList();
            //var chk = db.proc_Recharge_operator_report_newPaging(1, 25000000, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), "Admin", "All", ddlst, optname, txtmob, portname).ToList();
            // whitelabel_Recharge_Total_Succ_Failed '2020-02-21','2020-03-22','WAdmin','e20bd2df-b9a0-4741-b5f6-d94bfb83763c','ALL','ALL','ALL'
            var successtotal = chk.Where(a => a.Rstaus.ToUpper() == "SUCCESS").Sum(a => a.amount); //chk.Where(aa => aa.rstatus.ToUpper() == "SUCCESS").Sum(aa => aa.amount);
            var Failedtotal = chk.Where(aa => aa.Rstaus.ToUpper() == "FAILED").Sum(aa => aa.amount);
            var pendingtotal = chk.Where(aa => aa.Rstaus.ToUpper().Contains("REQ")).Sum(aa => aa.amount);
            //var retailerincome = chk.Where(a => a.Rstaus.ToUpper() == "SUCCESS").Sum(a => a.income);
            //var dlmincome = chk.Where(a => a.Rstaus.ToUpper() == "SUCCESS").Sum(a => a.dealerincome);
            var data = new
            {
                success = successtotal,
                failed = Failedtotal,
                pending = pendingtotal

            };

            return Json(data, JsonRequestBehavior.AllowGet);

        }

        #region Generate_Token
        public ActionResult Generate_Token()
        {
            ViewData["success"] = TempData["success"];
            ViewData["error"] = TempData["error"];
            TempData.Remove("success");
            TempData.Remove("error");
            var loginid = User.Identity.GetUserId();
            var show = db.Api_ip_address.Where(a => a.userid == loginid).ToList();
            var reposeurl = db.Recharge_Update_Url.Where(a => a.UserId == loginid).ToList();
            ViewBag.responseurlchk = reposeurl;
            return View(show);
        }
        // Genertate Token with Ip address
        public ActionResult insertipaddress(string ipaddress)
        {
            try
            {
                var loginid = User.Identity.GetUserId();
                if (db.Api_ip_address.Where(o => o.userid == loginid).Any(p => p.ipaddress == ipaddress) == false)
                {
                    if (!string.IsNullOrWhiteSpace(ipaddress))
                    {
                        var key = loginid.Substring(0, 16);
                        var token = Encrypt(ipaddress, key);
                        Api_ip_address api = new Api_ip_address();
                        api.userid = loginid;
                        api.ipaddress = ipaddress;
                        api.sts = "N";
                        api.token = token;
                        db.Api_ip_address.Add(api);
                        db.SaveChanges();
                        return Json(new { Status = true, Message = "Token Generate Successfully." });

                    }
                    else
                    {
                        return Json(new { Status = false, Message = "Please Enter IP Address!" });
                    }
                }
                return Json(new { Status = false, Message = "IP Address Allready Exist!" });
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, Message = ex.Message });
            }


        }
        public string Encrypt(string input, string key)
        {
            byte[] inputArray = UTF8Encoding.UTF8.GetBytes(input);
            TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
            tripleDES.Key = UTF8Encoding.UTF8.GetBytes(key);
            tripleDES.Mode = CipherMode.ECB;
            tripleDES.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tripleDES.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            tripleDES.Clear();
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        public JsonResult DeleteIpaddress(int idno)
        {
            if (idno > 0)
            {
                var v = db.Api_ip_address.Single(m => m.idno == idno);
                db.Api_ip_address.Remove(v);
                db.SaveChanges();
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("Failed", JsonRequestBehavior.AllowGet);
            }

        }

        //Add New Response Url
        [HttpPost]
        public ActionResult AddUrl(string apiurl)
        {
            var loginid = User.Identity.GetUserId();
            //var websiteurl = db.Users.Where(a => a.UserId == loginid).SingleOrDefault().WebsiteURL;
            System.Data.Entity.Core.Objects.ObjectParameter output = new
                 System.Data.Entity.Core.Objects.ObjectParameter("Output", typeof(string));
            var show = db.API_insert_Updateurl(loginid, apiurl, output).SingleOrDefault().msg;
            if (show == "Successfully.")
            {
                TempData["success"] = "Response Url Add Successfully.";
                return RedirectToAction("Generate_Token");
            }
            else
            {
                TempData["error"] = "This UserId is Allready Exist.";
                return RedirectToAction("Generate_Token");
            }

        }

        //Edit Response Url
        [HttpPost]
        public ActionResult EditResponseUrl(string txtid, string editapiurl)
        {
            try
            {
                System.Data.Entity.Core.Objects.ObjectParameter output = new System.Data.Entity.Core.Objects.ObjectParameter("Output", typeof(string));
                var show = db.Update_resposeurl(txtid, editapiurl, output).SingleOrDefault().msg;
                if (show == "Update Successfully.")
                {
                    TempData["success"] = "Update Successfully.";
                    return RedirectToAction("Generate_Token");
                }
                else
                {
                    return RedirectToAction("Generate_Token");
                }

            }
            catch
            {

            }
            return RedirectToAction("APIDocument");
        }

        public JsonResult DeleteResponseurl(int id)
        {
            if (id > 0)
            {
                var ch = db.Recharge_Update_Url.Single(a => a.idno == id);
                db.Recharge_Update_Url.Remove(ch);
                db.SaveChanges();
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("Failed", JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        public ActionResult APIDOC()
        {
            bool? payoutapists = false;
            bool? upiapists = false;
            var serviceinfo = db.Serviceallows.Where(aa => aa.ServiceName == "PAYOUT API").SingleOrDefault();
            if(serviceinfo!=null)
            {
                payoutapists = serviceinfo.Sts;
            }
            serviceinfo = db.Serviceallows.Where(aa => aa.ServiceName == "UPI API").SingleOrDefault();
            if (serviceinfo != null)
            {
                upiapists = serviceinfo.Sts;
            }
            ViewBag.payoutsts = payoutapists;
            ViewBag.upists = upiapists;
            string websiteUrl = db.Admin_details.SingleOrDefault().WebsiteUrl;
            var port = Request.Url.Port;

            ViewBag.websiteurl = (port == 443 ? "https://www." : "http://") + websiteUrl;

            return View();
        }
        public ActionResult DMTDOC()
        {
            ViewBag.websiteurl = db.Admin_details.SingleOrDefault().WebsiteUrl;
            return View();
        }
        //Complaint Request 
        public ActionResult Complaint()
        {
            var userid = User.Identity.GetUserId();
            var ch = db.proc_Complaint_request(userid, "").ToList();
            return View(ch);
        }
        [HttpPost]
        public ActionResult Complaint_insert(string message)
        {
            var statusAdmin = db.PushNotificationStatus.Where(a => a.UserRole == "ApiUser").SingleOrDefault().Status;
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
                SendPushNotification(Emailid, Url.Action("Dashboard", "Home"), "User " + retaileremaillid + " is Send the Complaint For You .And Compalint is that " + message + "", "Complaint Insert..");
            }
            return RedirectToAction("Complaint");
        }

        public ActionResult Gst_Invocing_API_report()
        {
            var userid = User.Identity.GetUserId();
            DateTime txt_frm_date = DateTime.Now;
            DateTime to_date = DateTime.Now;
            DateTime date = DateTime.Now;
            string OldMonth = date.AddMonths(-1).ToString("MMMM");

            ViewBag.OldMonth = OldMonth;
            txt_frm_date = new DateTime(date.AddMonths(-1).Year, date.AddMonths(-1).Month, 1);
            to_date = new DateTime(date.Year, date.Month, 1);
            var show = db.GST_Monthly_API(Convert.ToDateTime(txt_frm_date), Convert.ToDateTime(to_date)).Where(a => a.apiid == userid).ToList();
            return View(show);

        }

        public ActionResult ExcelGst_Invocing_API_report()
        {
            var userid = User.Identity.GetUserId();
            DateTime txt_frm_date = DateTime.Now;
            DateTime to_date = DateTime.Now;
            DateTime date = DateTime.Now;
            string OldMonth = date.AddMonths(-1).ToString("MMMM");

            ViewBag.OldMonth = OldMonth;
            txt_frm_date = new DateTime(date.AddMonths(-1).Year, date.AddMonths(-1).Month, 1);
            to_date = new DateTime(date.Year, date.Month, 1);
            var show = db.GST_Monthly_API(Convert.ToDateTime(txt_frm_date), Convert.ToDateTime(to_date)).Where(a => a.apiid == userid).ToList();

            DataTable dtt = new DataTable();
            dtt.Columns.Add("Email ID", typeof(decimal));
            dtt.Columns.Add("Name", typeof(decimal));
            dtt.Columns.Add("Firm Name", typeof(decimal));
            dtt.Columns.Add("DMT Comm", typeof(decimal));
            dtt.Columns.Add("DMT GST", typeof(decimal));
            dtt.Columns.Add("RCH Comm", typeof(decimal));
            dtt.Columns.Add("RCH GST", typeof(decimal));
            dtt.Columns.Add("Total GST ", typeof(decimal));
            if (show.Count > 0)
            {
                foreach (var item in show)
                {
                    dtt.Rows.Add(item.emailid, item.username, item.farmname, item.dmtcomm, item.dmtgst, item.rchcomm, item.rchgst, item.total);
                }
            }

            else
            {
                dtt.Rows.Add("", "", "", "", "", "", "", "");
            }
            var grid = new GridView();
            grid.DataSource = dtt;
            grid.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=Gst_Invocing_API_report.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

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

            var entries = db.GST_Monthly_API(Convert.ToDateTime(txt_frm_date), Convert.ToDateTime(to_date)).Where(a => a.apiid == userid).ToList();
            ViewBag.address = entries.SingleOrDefault().Address;
            ViewBag.customergst = entries.SingleOrDefault().GST;
            ViewBag.dmtcomm = entries.SingleOrDefault().dmtcomm;
            ViewBag.dmttotal = entries.SingleOrDefault().dmtcomm + entries.SingleOrDefault().dmtgst;
            ViewBag.rechargecomm = entries.SingleOrDefault().rchcomm;
            ViewBag.rechargetotal = entries.SingleOrDefault().rchcomm + entries.SingleOrDefault().rchgst;

            var final = entries.SingleOrDefault().dmtcomm + entries.SingleOrDefault().rchcomm;
            ViewBag.totaltaxvalue = final;

            var valdmt = entries.SingleOrDefault().dmtgst;
            var valrecharge = entries.SingleOrDefault().rchgst;

            var finalgst = valdmt + valrecharge;

            ViewBag.finaltotal = finalgst + final;
            ViewBag.totalgsttotal = Convert.ToDouble(finalgst);
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

                ViewBag.totaligst = Convert.ToDouble(finalgst);
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
                ViewBag.totalcgst = finalgst / 2;
                ViewBag.totalsgst = finalgst / 2;
            }

            //ViewBag.particular = "Commission For " + from.ToString("MMMM") + " Month";
            ViewBag.netamount = entries.SingleOrDefault().dmtcomm;
            ViewBag.firmname = entries.SingleOrDefault().farmname;
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
        public ActionResult Dashboard()
        {
            var userid = User.Identity.GetUserId();
            CallAutofundtransfer();
            //Vonder Profile Details
            var vv = db.api_user_details.FirstOrDefault(a => a.apiid == userid);
            ViewBag.email = vv.emailid;
            ViewBag.image = vv.Photo;

            //show News for master
            var news = (from pp in db.Message_top where (pp.users == "Api" || pp.users == "All") where pp.status == "Y" select pp).ToList();
            ViewBag.news = news;
            //if (news.Count() > 0)
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

            DateTime todaysDate = DateTime.Now.Date;
            int month = todaysDate.Month;
            int year = todaysDate.Year;

            int Nxtmonth = todaysDate.AddMonths(1).Month;
            int Nxtyear = todaysDate.AddMonths(1).Year;

            TargetSetviewmodel vmodel = new TargetSetviewmodel();
            var allOn = db.tblAPIsettargets.Where(a => a.Status == true).ToList();
            vmodel.apiTargetCategory = allOn.Where(a => a.Date.Value.Month == month && a.Date.Value.Year == year).ToList();
            vmodel.apiTargetCategoryNxt = allOn.Where(a => a.Date.Value.Month == Nxtmonth && a.Date.Value.Year == Nxtyear).ToList();
            vmodel.productItems = db.tblPruductGifts.ToList();

            vmodel.CategoryImages = Directory.EnumerateFiles(Server.MapPath("~/CategoryImages")).Select(fn => Path.GetFileNameWithoutExtension(fn));

            return View(vmodel);
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


        public void CallAutofundtransfer()
        {
            #region Auto credit transfer start

            var userid = User.Identity.GetUserId();

            string transferids = GenerateUniqueTransectionID();
            var transferid = "AUTO" + transferids;
            var remdetailsallinform = db.api_user_details.Where(x => x.apiid == userid).SingleOrDefault();
            var reaminbalance_master = db.api_remain_amount.Where(x => x.apiid == userid).SingleOrDefault();
            var adminid = db.Admin_details.SingleOrDefault().userid;
            int noofcount = 0;
            if (reaminbalance_master != null)
            {

                var automainmiumbal = db.AutoFundtransfer_admin_to_apiuser.Where(x => x.api_userid == userid && (x.types.ToUpper() == "CASH" || x.types.ToUpper() == "CREDIT") && x.status == "Y").SingleOrDefault();

                DateTime fromdate = DateTime.Now;
                DateTime todate = DateTime.Now.AddDays(1);
                if (automainmiumbal != null)
                {
                    DateTime todaydate = DateTime.Now;
                    int transfer1min = 0;
                    try
                    {

                        var check = db.API_Balance_transfer.Where(x => x.apiid == userid && x.Head == "AutoFund").ToList();
                        noofcount = check.Where(x => x.date.Value.Date == todaydate.Date).Count();
                        var sdd = db.API_Balance_transfer.Where(x => x.apiid == userid && x.Head == "AutoFund").OrderByDescending(x => x.id).FirstOrDefault();
                        if (sdd != null)
                        {
                            try
                            {
                                var resultsss = (int)sdd.date.Value.Subtract(todaydate).TotalMinutes;
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
                            if (automainmiumbal.minamount > reaminbalance_master.balance)
                            {

                                System.Data.Entity.Core.Objects.ObjectParameter output = new
                                     System.Data.Entity.Core.Objects.ObjectParameter("Output", typeof(string));
                                var msg = db.auto_Fundtransfer_admin_Api_insert_balance(userid, transferid, output).Single().msg;
                                try
                                {
                                    var apiuserdetails = db.api_user_details.Where(aa => aa.apiid == userid).SingleOrDefault();
                                  
                                    var apiinfo = db.api_remain_amount.Where(aa => aa.apiid == userid).SingleOrDefault();
                                   
                                    var admininfo = db.Admin_details.SingleOrDefault();
                                    Backupinfo back = new Backupinfo();
                                    var modeln = new Backupinfo.Addinfo
                                    {
                                        Websitename = admininfo.WebsiteUrl,
                                        RetailerID = userid,
                                        Email = apiuserdetails.emailid,
                                        Mobile = apiuserdetails.mobile,
                                        Details = "Fund recived From Admin ",
                                        RemainBalance = (decimal)apiinfo.balance,
                                        Usertype = "API"
                                    };
                                    back.Fundtransfer(modeln);

                                }
                                catch { }

                                if (msg == "Success")
                                {
                                    var TotalAmount = reaminbalance_master.balance + automainmiumbal.Transferamount;
                                    var statusSendSmsRetailerfundtransfer = db.SMSSendAlls.Where(a => a.ServiceName == "AdmintoApifundtrans").SingleOrDefault();
                                    var statusSendEmailRetailerfundtransfer = db.EmailSendAlls.Where(a => a.ServiceName == "AdmintoApifundtrans1").SingleOrDefault().Status;
                                    var diff1 = (db.API_Balance_transfer.Where(aa => aa.apiid == userid && aa.groupname == adminid).OrderByDescending(aa => aa.id).Select(c => c.cr).FirstOrDefault());
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

                                        //        smssend.sendsmsallnew(remdetailsallinform.mobile, msgssss, urlss, tempid);
                                        //    }



                                        //    //  smssend.sendsmsall(RetailerDetails.Mobile, "Credit Received Rs." + amount1 + ".New Balance is " + TotalAmount + ".Your O/s Credit is " + diff1 + "", "Fund Transfer");
                                        //}

                                        smssend.sms_init(statusSendSmsRetailerfundtransfer.Status, statusSendSmsRetailerfundtransfer.Whatsapp_Status, "FUNDTRANSFERMESSAGE_CREDIT", remdetailsallinform.mobile, automainmiumbal.Transferamount, TotalAmount, diff1);

                                        if (statusSendEmailRetailerfundtransfer == "Y")
                                        {
                                            smssend.SendEmailAll(remdetailsallinform.emailid, "Credit Received Rs." + automainmiumbal.Transferamount + ".New Balance is " + TotalAmount + ".Your O/s Credit is " + diff1 + "", "Fund Transfer", db.Admin_details.SingleOrDefault().email, 1000);
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

                                        //        smssend.sendsmsallnew(remdetailsallinform.mobile, msgssss, urlss, tempid);
                                        //    }



                                        //    //  smssend.sendsmsall(RetailerDetails.Mobile, "Credit Received Rs." + amount1 + ".New Balance is " + TotalAmount + ".Your O/s Credit is " + diff1 + "", "Fund Transfer");
                                        //}

                                        smssend.sms_init(statusSendSmsRetailerfundtransfer.Status, statusSendSmsRetailerfundtransfer.Whatsapp_Status, "FUNDTRANSFERMESSAGE_CREDIT", remdetailsallinform.mobile, automainmiumbal.Transferamount, TotalAmount, diff1);

                                        if (statusSendEmailRetailerfundtransfer == "Y")
                                        {
                                            smssend.SendEmailAll(remdetailsallinform.emailid, "Cash Received Rs." + automainmiumbal.Transferamount + ".New Balance is " + TotalAmount + ".Your O/s Credit is " + diff1 + "", "Fund Transfer", db.Admin_details.SingleOrDefault().email, 1000);
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





        //User wise Operator comm report
        public ActionResult User_wise_operator_comm()
        {
            var userid = User.Identity.GetUserId();
            string txt_frm_date = DateTime.Now.ToString();
            string txt_to_date = DateTime.Now.ToString();
            string frm_date = Convert.ToDateTime(txt_frm_date).Date.ToString("yyyy-MM-dd");
            string to_date = Convert.ToDateTime(txt_to_date).AddDays(1).ToString("yyyy-MM-dd");
            var ch = db.User_wise_comm_report("APIID", userid, 50, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            ViewData["totalrch"] = ch.Where(a => a.optnm != "Money" && a.optnm != "Pancard" && a.optnm != "Aeps").Sum(a => a.total);
            ViewData["totalmoney"] = ch.Where(a => a.optnm == "Money").Sum(a => a.total);
            ViewData["remincome"] = ch.Sum(x => x.remincome);
            return View(ch);
        }

        [HttpPost]
        public ActionResult User_wise_operator_comm(string txt_frm_date, string txt_to_date, string ddl_top = "ALL")
        {
            var userid = User.Identity.GetUserId();

            ViewBag.chk = "post";

            if (ddl_top == "ALL")
            {
                ddl_top = "1000000";
            }

            int ddltop = Convert.ToInt32(ddl_top);
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
            //show whitelabel
            var ch = db.User_wise_comm_report("APIID", userid, ddltop, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            ViewData["totalrch"] = ch.Where(a => a.optnm != "Money" && a.optnm != "Pancard" && a.optnm != "Aeps").Sum(a => a.total);
            ViewData["totalmoney"] = ch.Where(a => a.optnm == "Money").Sum(a => a.total);

            ViewData["remincome"] = ch.Sum(x => x.remincome);
            return View(ch);
        }

        public ActionResult ExcelUser_wise_operator_comm(string txt_frm_date, string txt_to_date, string ddl_top = "ALL")
        {
            var userid = User.Identity.GetUserId();

            ViewBag.chk = "post";

            if (ddl_top == "ALL")
            {
                ddl_top = "1000000";
            }

            int ddltop = Convert.ToInt32(ddl_top);
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
            //show whitelabel
            var ch = db.User_wise_comm_report("APIID", userid, ddltop, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();


            DataTable dataTbl = new DataTable();
            dataTbl.Columns.Add("Email-ID", typeof(string));
            dataTbl.Columns.Add("Name", typeof(string));
            dataTbl.Columns.Add("Firm Name", typeof(string));
            dataTbl.Columns.Add("Opratore Name", typeof(string));
            dataTbl.Columns.Add("Income", typeof(string));
            dataTbl.Columns.Add("Total sucess", typeof(string));

            if (ch.Count > 0)
            {
                foreach (var item in ch)
                {

                    dataTbl.Rows.Add(item.Email, item.RetailerName, item.Frm_Name, item.optnm, item.remincome, item.remincome);

                }
            }
            else
            {
                dataTbl.Rows.Add("", "", "", "", "", "");
            }
            var grid = new GridView();
            grid.DataSource = dataTbl;
            grid.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=User_wise_operator_comm.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            return View(ch);
        }

        public ActionResult PDFUser_wise_operator_comm(string txt_frm_date, string txt_to_date, string ddl_top = "ALL")
        {
            var userid = User.Identity.GetUserId();

            ViewBag.chk = "post";

            if (ddl_top == "ALL")
            {
                ddl_top = "1000000";
            }

            int ddltop = Convert.ToInt32(ddl_top);
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
            //show whitelabel
            var ch = db.User_wise_comm_report("APIID", userid, ddltop, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            ViewData["totalrch"] = ch.Where(a => a.optnm != "Money" && a.optnm != "Pancard" && a.optnm != "Aeps").Sum(a => a.total);
            ViewData["totalmoney"] = ch.Where(a => a.optnm == "Money").Sum(a => a.total);

            ViewData["remincome"] = ch.Sum(x => x.remincome);
            return new ViewAsPdf(ch);
        }



        //show today and yesterday business
        #region show today and yesterday business
        public ActionResult Show_All_Recharge(string type)
        {
            var userid = User.Identity.GetUserId();
            // Donught Chart
            var result = db.show_todayrecharge_api(userid, type).ToList();
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

            return Json(new { Status = type, Recharge = rechargeandbill, Moneytransfer = moneytransfer });
        }
        #endregion
        //My Credit Balance
        [HttpGet]
        public ActionResult Chkbalance()
        {
            var userid = User.Identity.GetUserId();
            // get total credit balance from admin
            var apicreditbal = db.total_api_outstanding_by_admin(userid).FirstOrDefault().totaloutstanding;
            var banklist = db.bank_info.Where(x => x.userid == "Admin").ToList();
            var walletlist = db.tblwallet_info.Where(x => x.userid == "Admin").ToList();


            return Json(new { apicreditbal = apicreditbal, listbank = banklist, walletinfo = walletlist }, JsonRequestBehavior.AllowGet);
            //return Json(new
            //{

            //    apicreditbal = apicreditbal,
            //    listbank = banklist,
            //    walletinfo = walletlist,JsonRequestBehavior.AllowGet
            //});
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
                    WebImage photo = null;
                    var newFileName = "";
                    var imagePath = "";


                    string userid = User.Identity.GetUserId();

                    var count = (db.api_purchase.Where(aa => aa.sts == "Pending" && aa.apiid == userid).Count());

                    int countchk = Convert.ToInt32(count);
                    var acc = adminacco;
                    if (countchk < 1)
                    {
                        var amount = Convert.ToDecimal(hdPaymentAmount);
                        if (amount > 0)
                        {
                            var from = "";
                            decimal disCharge = 0;


                            from = db.Admin_details.Single().userid;

                            var chargeEntry = db.creditchargeapis.Where(aa => aa.userid == "Admin").FirstOrDefault(a => a.type == hdPaymentMode);


                            if (chargeEntry != null)
                            {
                                disCharge = (amount * Convert.ToDecimal(chargeEntry.charge.Value)) / 100;
                                //  alert(disCharge);

                            }
                            //if (hdPaymentMode == "Cash")
                            //{
                            //    //var chargeEntry = db.PurchaseOrderCashDepositCharges.FirstOrDefault(a => a.PurchaseOrderType == hdPaymentMode);

                            //}



                            db.insert_Api_purchageorder(userid, hdPaymentMode, collectionby, bankname, "", hdMDComments, Convert.ToDecimal(hdPaymentAmount), "", hdPaymentMode, "", adminacco, @"/PurchaseOrderImg/" + newFileName, "", "", "", disCharge, amount - disCharge);
                            // TempData["success"] = "Your Rquest Proceed Successfully.";
                            // ch = "Your purcharge Order Successfully.";

                            IEnumerable<select_api_pur_order_Result> model = null;
                            DateTime frm = Convert.ToDateTime("2020-12-30 11:58:20.650");
                            DateTime to = Convert.ToDateTime("2021-07-30 11:58:20.650");
                            DateTime frm_date = Convert.ToDateTime(frm).Date;
                            DateTime to_date = Convert.ToDateTime(to).Date.AddDays(1);
                            model = db.select_api_pur_order(userid, "ALL", frm_date, to_date).ToList();


                            var api_data = db.api_user_details.Where(s => s.apiid == userid).SingleOrDefault();
                            var api_name = api_data.username;
                            var api_no = api_data.mobile;
                            string apiurls = "";



                            bool isSmsOn = db.apisms.Any(s => s.sts == "Y");

                            if (isSmsOn)
                            {
                                var asd = db.SMSSendAlls.Where(s => s.ServiceName == "ApitoAdminfundtrans1" && s.Whatsapp_Status == "Y").ToList();
                                var asdcount = asd.Count();
                                var smsapi = db.apisms.Where(x => x.sts == "Y").ToList();
                                var mobile12 = db.Admin_details.SingleOrDefault().mobile;
                                var smsapionsts = smsapi.Where(s => s.api_type == "whatsapp").SingleOrDefault();

                                if (smsapionsts != null)
                                {
                                    if (asdcount > 0)
                                    {
                                        apiurls = smsapionsts.smsapi;
                                        string text = api_name + "-" + api_data.farmname + "(" + api_no + ") is Requested to you for perchase order of " + hdPaymentAmount + "Rs";
                                        text = string.Format(text, "1230");

                                        var apinamechange = apiurls.Replace("tttt", mobile12).Replace("mmmm", text);

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
                                        sms.messagefor = userid;
                                        db.sms_api_entry.Add(sms);
                                        db.SaveChanges();
                                    }
                                }
                                var asd1 = db.SMSSendAlls.Where(s => s.ServiceName == "ApitoAdminfundtrans1" && s.Status == "Y").ToList();
                                var asd1count = asd1.Count();
                                var smsapionsts1 = smsapi.Where(s => s.api_type == "sms").SingleOrDefault();
                                if (smsapionsts1 != null)
                                {
                                    if (asd1count > 0)
                                    {
                                        apiurls = smsapionsts1.smsapi;
                                        string text = api_name + "-" + api_data.farmname + "(" + api_no + ") is Requested to you for perchase order of " + hdPaymentAmount + "Rs";
                                        text = string.Format(text, "1230");

                                        var apinamechange = apiurls.Replace("tttt", mobile12).Replace("mmmm", text);

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
                                        sms.messagefor = userid;
                                        db.sms_api_entry.Add(sms);
                                        db.SaveChanges();

                                    }
                                }
                            }

                            var emailcheck = db.EmailSendAlls.Where(s => s.ServiceName == "ApitoAdminfundtrans1" && s.Status == "Y").ToList();
                            var emailceckcount = emailcheck.Count();

                            if (emailceckcount > 0)
                            {
                                var AdminDetails = db.Admin_details.SingleOrDefault();
                                smssend.SendEmailAll(AdminDetails.email, api_name + "-" + api_data.farmname + "(" + api_no + ") is Requested to you for perchase order of " + hdPaymentAmount + "Rs", "Purchase Order Request", AdminDetails.email);

                            }














                            return RedirectToAction("SendPurchaseOrder");
                        }
                        else
                        {
                            TempData["error"] = "Amount should be not zero";
                            ch = "Amount should be not zero";
                            // return RedirectToAction("Purchase_ORDER");
                            return Json(ch, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        TempData["error"] = "Your purcharge Order Allready Pending.";
                        ch = "Your purcharge Order Allready Pending.";
                        return Json(ch, JsonRequestBehavior.AllowGet);
                    }
                    // return RedirectToAction("Purchase_ORDER");
                    return Json(ch, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    TempData["error"] = ex;
                    //  return RedirectToAction("Purchase_ORDER");
                    ch = ex.Message;
                    return Json(ch, JsonRequestBehavior.AllowGet);
                }
            }
        }

        public ActionResult Show_Credit_report_by_admin()
        {
            var userid = User.Identity.GetUserId();
            var show = db.select_api_credit_report_by_admin(userid).ToList();
            return View(show);
        }

        //today Recived balaance from admin

        public ActionResult Totalbaltransfer()
        {
            var userid = User.Identity.GetUserId();
            var apibalfromadmin = db.api_Today_total_recive_bal_by_admin(userid).FirstOrDefault().totalbal;
            return Json(new
            {
                apibalfromadmin = apibalfromadmin
            });
        }

        public ActionResult ReceiveFund_by_admin()
        {
            var userid = User.Identity.GetUserId();
            string txt_frm_date = DateTime.Now.ToString();
            string txt_to_date = DateTime.Now.ToString();
            string frm_date = Convert.ToDateTime(txt_frm_date).Date.ToString("yyyy-MM-dd");
            string to_date = Convert.ToDateTime(txt_to_date).AddDays(1).ToString("yyyy-MM-dd");
            var show = db.api_total_fund_recive_by_admin(userid, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            return View(show);
        }

        //Help and Support
        public ActionResult Help()
        {
            var admininfo = db.Admin_details.FirstOrDefault();
            ViewBag.admin = admininfo;
            return View();
        }
        [HttpGet]
        public ActionResult SendPurchaseOrder()
        {
            string txt_frm_date = DateTime.Now.ToString();
            string txt_to_date = DateTime.Now.ToString();
            var userid = User.Identity.GetUserId();
            var mycredit = db.total_api_outstanding_by_admin(userid).FirstOrDefault().totaloutstanding;
            ViewData["oldcredit"] = mycredit;
            var amdninid = db.Admin_details.SingleOrDefault().userid;
            var account1 = (from bk in db.bank_info where bk.userid == "Admin" select bk).ToList();
            IEnumerable<SelectListItem> bnkselectList = from s in account1
                                                        select new SelectListItem
                                                        {
                                                            Value = s.acno.ToString(),
                                                            Text = s.banknm.ToString()
                                                        };
            ViewBag.fillaccount = bnkselectList;
            var wallets = (from wll in db.tblwallet_info.Where(a => a.userid == "Admin") select new { wlno = wll.walletno, walletno = wll.walletname });
            ViewBag.walletsdlm = new SelectList(wallets, "wlno", "walletno", null);
            IEnumerable<select_api_pur_order_Result> model = null;
            DateTime frm = Convert.ToDateTime(txt_frm_date);
            DateTime to = Convert.ToDateTime(txt_to_date);
            txt_frm_date = frm.ToString("dd-MM-yyyy");
            txt_to_date = to.ToString("dd-MM-yyyy");
            string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy",
                            "yyyy-MM-dd", "dd-MM-yyyy", "MM-dd-yyyy", "dd MMM yyyy","M/d/yyyy hh:mm:ss tt" ,"dd-MM-yyyy HH:mm:ss","M/dd/yyyy hh:mm:ss tt","MM/dd/yyyy hh:mm tt", "MM/dd/yyyy HH:mm:ss"};

            DateTime dt = !string.IsNullOrWhiteSpace(txt_frm_date) ? DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
            DateTime dt1 = !string.IsNullOrWhiteSpace(txt_to_date) ? DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
            DateTime frm_date = Convert.ToDateTime(dt).Date;
            DateTime to_date = Convert.ToDateTime(dt1).Date.AddDays(1);
            model = db.select_api_pur_order(userid, "ALL", dt, to_date).ToList();
            return View(model);
        }
        [HttpPost]
        public ActionResult SendPurchaseOrder(string txt_frm_date, string txt_to_date)
        {
            var userid = User.Identity.GetUserId();
            var mycredit = db.total_api_outstanding_by_admin(userid).FirstOrDefault().totaloutstanding;
            ViewData["oldcredit"] = mycredit;
            var amdninid = db.Admin_details.SingleOrDefault().userid;
            var account1 = (from bk in db.bank_info where bk.userid == "Admin" select bk).ToList();
            IEnumerable<SelectListItem> bnkselectList = from s in account1
                                                        select new SelectListItem
                                                        {
                                                            Value = s.acno.ToString(),
                                                            Text = s.banknm.ToString()
                                                        };
            ViewBag.fillaccount = bnkselectList;
            var wallets = (from wll in db.tblwallet_info.Where(a => a.userid == "Admin") select new { wlno = wll.walletno, walletno = wll.walletname });
            ViewBag.walletsdlm = new SelectList(wallets, "wlno", "walletno", null);
            IEnumerable<select_api_pur_order_Result> model = null;
            DateTime frm = Convert.ToDateTime(txt_frm_date);
            DateTime to = Convert.ToDateTime(txt_to_date);
            txt_frm_date = frm.ToString("dd-MM-yyyy");
            txt_to_date = to.ToString("dd-MM-yyyy");
            string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy",
                            "yyyy-MM-dd", "dd-MM-yyyy", "MM-dd-yyyy", "dd MMM yyyy","M/d/yyyy hh:mm:ss tt" ,"dd-MM-yyyy HH:mm:ss","M/dd/yyyy hh:mm:ss tt","MM/dd/yyyy hh:mm tt", "MM/dd/yyyy HH:mm:ss"};

            DateTime dt = !string.IsNullOrWhiteSpace(txt_frm_date) ? DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
            DateTime dt1 = !string.IsNullOrWhiteSpace(txt_to_date) ? DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
            DateTime frm_date = Convert.ToDateTime(dt).Date;
            DateTime to_date = Convert.ToDateTime(dt1).Date.AddDays(1);
            model = db.select_api_pur_order(userid, "ALL", dt, to_date).ToList();
            return PartialView("_FundTransferAdminToAPi_userPartial", model);
        }

        public ActionResult ExcelSendPurchaseOrder(string txt_frm_date, string txt_to_date)
        {
            var userid = User.Identity.GetUserId();
            var mycredit = db.total_api_outstanding_by_admin(userid).FirstOrDefault().totaloutstanding;
            ViewData["oldcredit"] = mycredit;
            var amdninid = db.Admin_details.SingleOrDefault().userid;
            var account1 = (from bk in db.bank_info where bk.userid == "Admin" select bk).ToList();
            IEnumerable<SelectListItem> bnkselectList = from s in account1
                                                        select new SelectListItem
                                                        {
                                                            Value = s.acno.ToString(),
                                                            Text = s.banknm.ToString()
                                                        };
            ViewBag.fillaccount = bnkselectList;
            var wallets = (from wll in db.tblwallet_info.Where(a => a.userid == "Admin") select new { wlno = wll.walletno, walletno = wll.walletname });
            ViewBag.walletsdlm = new SelectList(wallets, "wlno", "walletno", null);
            IEnumerable<select_api_pur_order_Result> model = null;
            DateTime frm = Convert.ToDateTime(txt_frm_date);
            DateTime to = Convert.ToDateTime(txt_to_date);
            txt_frm_date = frm.ToString("dd-MM-yyyy");
            txt_to_date = to.ToString("dd-MM-yyyy");
            string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy",
                            "yyyy-MM-dd", "dd-MM-yyyy", "MM-dd-yyyy", "dd MMM yyyy","M/d/yyyy hh:mm:ss tt" ,"dd-MM-yyyy HH:mm:ss","M/dd/yyyy hh:mm:ss tt","MM/dd/yyyy hh:mm tt", "MM/dd/yyyy HH:mm:ss"};

            DateTime dt = !string.IsNullOrWhiteSpace(txt_frm_date) ? DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
            DateTime dt1 = !string.IsNullOrWhiteSpace(txt_to_date) ? DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
            DateTime frm_date = Convert.ToDateTime(dt).Date;
            DateTime to_date = Convert.ToDateTime(dt1).Date.AddDays(1);
            model = db.select_api_pur_order(userid, "ALL", dt, to_date).ToList();

            DataTable dataTbl = new DataTable();
            dataTbl.Columns.Add("Status", typeof(string));
            dataTbl.Columns.Add("orderNo", typeof(string));
            dataTbl.Columns.Add("RequestTo", typeof(string));
            dataTbl.Columns.Add("Payment mode", typeof(string));
            dataTbl.Columns.Add("description", typeof(string));
            dataTbl.Columns.Add("Total AMT", typeof(string));
            dataTbl.Columns.Add("charge", typeof(string));
            dataTbl.Columns.Add("Net T/F", typeof(string));
            dataTbl.Columns.Add("Req.Date", typeof(string));
            dataTbl.Columns.Add("Resp.Date", typeof(string));

            if (model.Count() > 0)
            {

                foreach (var item in model)
                {


                    dataTbl.Rows.Add(item.sts, item.orderno, "Admin", item.paymode, item.details, item.amount, item.cashDepositCharge,
                       item.finalAmount, item.reqdate, item.responsedate);
                }
            }

            else
            {

                dataTbl.Rows.Add("", "", "", "", "", "", "", "", "", "");
            }


            var grid = new GridView();
            grid.DataSource = dataTbl;
            grid.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=SendPurchaseOrder.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();


            return View(model);
        }
        public ActionResult PDFSendPurchaseOrder(string txt_frm_date, string txt_to_date)
        {
            var userid = User.Identity.GetUserId();
            var mycredit = db.total_api_outstanding_by_admin(userid).FirstOrDefault().totaloutstanding;
            ViewData["oldcredit"] = mycredit;
            var amdninid = db.Admin_details.SingleOrDefault().userid;
            var account1 = (from bk in db.bank_info where bk.userid == "Admin" select bk).ToList();
            IEnumerable<SelectListItem> bnkselectList = from s in account1
                                                        select new SelectListItem
                                                        {
                                                            Value = s.acno.ToString(),
                                                            Text = s.banknm.ToString()
                                                        };
            ViewBag.fillaccount = bnkselectList;
            var wallets = (from wll in db.tblwallet_info.Where(a => a.userid == "Admin") select new { wlno = wll.walletno, walletno = wll.walletname });
            ViewBag.walletsdlm = new SelectList(wallets, "wlno", "walletno", null);
            IEnumerable<select_api_pur_order_Result> model = null;
            DateTime frm = Convert.ToDateTime(txt_frm_date);
            DateTime to = Convert.ToDateTime(txt_to_date);
            txt_frm_date = frm.ToString("dd-MM-yyyy");
            txt_to_date = to.ToString("dd-MM-yyyy");
            string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy",
                            "yyyy-MM-dd", "dd-MM-yyyy", "MM-dd-yyyy", "dd MMM yyyy","M/d/yyyy hh:mm:ss tt" ,"dd-MM-yyyy HH:mm:ss","M/dd/yyyy hh:mm:ss tt","MM/dd/yyyy hh:mm tt", "MM/dd/yyyy HH:mm:ss"};

            DateTime dt = !string.IsNullOrWhiteSpace(txt_frm_date) ? DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
            DateTime dt1 = !string.IsNullOrWhiteSpace(txt_to_date) ? DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
            DateTime frm_date = Convert.ToDateTime(dt).Date;
            DateTime to_date = Convert.ToDateTime(dt1).Date.AddDays(1);
            model = db.select_api_pur_order(userid, "ALL", dt, to_date).ToList();
            return new ViewAsPdf(model);
        }
        public ActionResult ShowALLAPIRequest(string txt_frm_date, string txt_to_date)
        {
            var userid = User.Identity.GetUserId();
            IEnumerable<select_api_pur_order_Result> model = null;
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
            model = db.select_api_pur_order(userid, "ALL", Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            return PartialView("_FundTransferAdminToAPiuserPartial", model);
        }


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
            var show = db.api_total_fund_recive_by_admin(userid, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            return View(show);
        }

        public ActionResult ExcelReceiveFund_by_admin(string txt_frm_date, string txt_to_date)
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
            var show = db.api_total_fund_recive_by_admin(userid, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();

            DataTable dataTbl = new DataTable();
            dataTbl.Columns.Add("Admin Info", typeof(string));
            dataTbl.Columns.Add("Total Transfer", typeof(string));
            dataTbl.Columns.Add("Type", typeof(string));
            dataTbl.Columns.Add("Remain Pre", typeof(string));
            dataTbl.Columns.Add("Remain Post", typeof(string));
            dataTbl.Columns.Add("Current Credit", typeof(string));
            dataTbl.Columns.Add("Old Credit", typeof(string));
            dataTbl.Columns.Add("Net Amt", typeof(string));
            dataTbl.Columns.Add("Date", typeof(string));


            if (show.Count > 0)
            {
                foreach (var item in show)
                {


                    dataTbl.Rows.Add(item.email, item.balance, item.bal_type, item.remianold, item.remainamount, item.cr,
                       item.oldcrbalance, item.TotalFund, item.date);

                }
            }
            else
            {
                dataTbl.Rows.Add("", "", "", "", "", "", "", "", "");
            }
            var grid = new GridView();
            grid.DataSource = dataTbl;
            grid.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=ReceiveFund_by_admin.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            return View(show);
        }

        public ActionResult PDFReceiveFund_by_admin(string txt_frm_date, string txt_to_date)
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
            var show = db.api_total_fund_recive_by_admin(userid, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            return new ViewAsPdf(show);
        }


        public ActionResult Purchase_Order()
        {
            try
            {
                IEnumerable<select_api_pur_order_Result> model = null;

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
                var accunt = (from acc in db.bank_info select new { acno = acc.acno, acnooo = acc.acno + "||" + acc.holdername });
                ViewBag.account = new SelectList(accunt, "acno", "acnooo", null);
                var nm = (from bn in db.bank_details select bn);
                ViewBag.banknm = new SelectList(nm, "banknm", "banknm", null);
                CultureInfo ci = new CultureInfo("en-US");
                var month = DateTime.Now.ToString("MMMM", ci);
                ViewData["dayofmonth"] = month + " Month Holiday List.";
                List<SelectListItem> li = new List<SelectListItem>();
                // total credit balance
                var mycredit = db.total_api_outstanding_by_admin(userid).FirstOrDefault().totaloutstanding;
                ViewData["oldcredit"] = mycredit;
                model = db.select_api_pur_order(userid, "ALL", Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
                var entries = db.PurchaseOrderCashDepositCharges.ToList();
                ViewBag.ATMMinCharge = entries.SingleOrDefault(a => a.PurchaseOrderType == "ATM Machine Deposit").MinCharge;
                ViewBag.ATMChargePercant = entries.SingleOrDefault(a => a.PurchaseOrderType == "ATM Machine Deposit").ChargePercant;

                ViewBag.CashMinCharge = entries.SingleOrDefault(a => a.PurchaseOrderType == "Branch Cash Deposit").MinCharge;
                ViewBag.CashChargePercant = entries.SingleOrDefault(a => a.PurchaseOrderType == "Branch Cash Deposit").ChargePercant;
                return View(model);

            }
            catch
            {
                return RedirectToAction("Purchase_Order");

            }
        }

        [HttpPost]
        public ActionResult Purchase_Order(string txt_frm_date, string txt_to_date)
        {
            try
            {
                IEnumerable<select_api_pur_order_Result> model = null;
                //using (VastwebmultiEntities db = new VastwebmultiEntities())
                //{
                ViewBag.chk = "post";
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

                //account no
                var accunt = (from acc in db.bank_info select new { acno = acc.acno, acnooo = acc.acno + "||" + acc.holdername });
                ViewBag.account = new SelectList(accunt, "acno", "acnooo", null);
                // total credit balance
                var mycredit = db.total_api_outstanding_by_admin(userid).FirstOrDefault().totaloutstanding;
                ViewData["oldcredit"] = mycredit;
                model = db.select_api_pur_order(userid, "ALL", Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date));
                var entries = db.PurchaseOrderCashDepositCharges.ToList();
                ViewBag.ATMMinCharge = entries.SingleOrDefault(a => a.PurchaseOrderType == "ATM Machine Deposit").MinCharge;
                ViewBag.ATMChargePercant = entries.SingleOrDefault(a => a.PurchaseOrderType == "ATM Machine Deposit").ChargePercant;

                ViewBag.CashMinCharge = entries.SingleOrDefault(a => a.PurchaseOrderType == "Branch Cash Deposit").MinCharge;
                ViewBag.CashChargePercant = entries.SingleOrDefault(a => a.PurchaseOrderType == "Branch Cash Deposit").ChargePercant;
                return View(model);
                //}
            }
            catch
            {
                return RedirectToAction("Purchase_Order");
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

                    if (db.api_purchase.Count(aa => aa.apiid.ToLower() == userid.ToLower() && aa.sts == "Pending") < 1)
                    {

                        var amount = Convert.ToDecimal(balance);
                        if (amount > 0)
                        {

                            if (type == "Credit")
                            {
                                Paymode = "Credit";
                                db.insert_Api_purchageorder(userid, Paymode, utrno, "", "", Comment, Convert.ToDecimal(balance), "", type, accountno, "", @"/PurchaseOrderImg/" + newFileName, "", "", "", 0, amount);
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
                                db.insert_Api_purchageorder(userid, Paymode, utrno, "", "", Comment, Convert.ToDecimal(balance), "", type, accountno, "", @"/PurchaseOrderImg/" + newFileName, pancard, branch, AccHolderName, disCharge, amount - disCharge);
                                TempData["success"] = "Purchase Order Successfully";
                            }
                        }
                        else
                        {
                            TempData["error"] = "Amount should be not zero";
                            return RedirectToAction("Purchase_Order");
                        }

                    }
                    else
                    {
                        TempData["error"] = "Your purcharge Order Allready Pending.";
                    }
                    return RedirectToAction("Purchase_Order");


                }
                catch (Exception ex)
                {
                    TempData["error"] = ex.ToString();
                    return RedirectToAction("Purchase_Order");
                }
            }

        }


        //Fund Received Pdf
        public ActionResult GotoPDF(string From, string Value, string DistOldBal, string Date)
        {
            string userid = User.Identity.GetUserId();
            return new Rotativa.ActionAsPdf("InvoicePDF", new { dlmloginid = userid, From = From, Value = Value, DistOldBal = DistOldBal, Date = Date });
        }

        public ActionResult InvoicePDF(string dlmloginid, string From, string Value, string DistOldBal, string Date)
        {
            var userdetaild = db.api_user_details.Where(a => a.apiid == dlmloginid).SingleOrDefault();
            var PDF_Content = new APIInvoiceModel()
            {
                From = From,
                Value = Value,
                DistOldBal = DistOldBal,
                Date = Date
            };
            TempData["retailername"] = userdetaild.apiid.ToUpper();
            TempData["firmname"] = userdetaild.farmname.ToUpper();
            TempData["retailermobile"] = userdetaild.mobile.ToUpper();

            TempData["retailerdate"] = PDF_Content.Date;
            TempData["date"] = DateTime.Now;
            return View(PDF_Content);
            //return View();
        }
        public ActionResult Commission()
        {
            var userid = User.Identity.GetUserId();
            Operater_Commission sb = new Operater_Commission();
            sb.AmountRange = (from p in db.Recharge_Amount_range
                              select new Recharge_Amount_range_info
                              {
                                  idno = p.idno,
                                  maximum1 = p.maximum1,
                                  maximum2 = p.maximum2
                              }).ToList();
            sb.Prepaid = (from cust in db.prepaid_api_comm
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
                          }).ToList();
            sb.Electricity_comm = (from cust in db.utility_api_comm
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
            sb.Money = (from cust in db.imps_api_comm
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
            sb.Pencard_comm = (from cust in db.Pancard_api_Common_comm
                               where (cust.userid == userid)
                               select new Pencard
                               {
                                   TokenValueDigital = cust.tokenvalueDigital,
                                   TokenValuePhysical = cust.tokenvaluePhysical,
                                   Gst = cust.gst,
                                   CommissionDigital = cust.commDigital,
                                   CommissionPhysical = cust.commPhysical
                               }).ToList();
            sb.INDONEPAL = (from cust in db.Slab_IndoNepal
                            where cust.UserId == userid && cust.Role == "API"
                            select new INDONEPAL_Comm
                            {
                                min = cust.min,
                                max = cust.max,
                                charge = cust.charge,
                                margin = cust.margin

                            }).ToList();

            sb.Broadband = (from cust in db.Broandband_api_comm
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
            sb.Loan = (from cust in db.Loan_api_comm
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

            sb.Water = (from cust in db.Water_api_comm
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

            sb.Insurance = (from cust in db.Insurance_api_comm
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
            return View(sb);
        }

        [HttpPost]
        public ActionResult Commission(string ddlcomm)
        {
            var userid = User.Identity.GetUserId();
            Operater_Commission sb = new Operater_Commission();
            sb.AmountRange = (from p in db.Recharge_Amount_range
                              select new Recharge_Amount_range_info
                              {
                                  idno = p.idno,
                                  maximum1 = p.maximum1,
                                  maximum2 = p.maximum2
                              }).ToList();
            sb.Prepaid = (from cust in db.prepaid_api_comm
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
                          }).ToList();
            sb.Electricity_comm = (from cust in db.utility_api_comm
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
            sb.Money = (from cust in db.imps_api_comm
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
            sb.Pencard_comm = (from cust in db.Pancard_api_Common_comm
                               where (cust.userid == userid)
                               select new Pencard
                               {
                                   TokenValueDigital = cust.tokenvalueDigital,
                                   TokenValuePhysical = cust.tokenvaluePhysical,
                                   Gst = cust.gst,
                                   CommissionDigital = cust.commDigital,
                                   CommissionPhysical = cust.commPhysical
                               }).ToList();
            sb.INDONEPAL = (from cust in db.Slab_IndoNepal
                            where cust.UserId == userid && cust.Role == "API"
                            select new INDONEPAL_Comm
                            {
                                min = cust.min,
                                max = cust.max,
                                charge = cust.charge,
                                margin = cust.margin

                            }).ToList();

            sb.Broadband = (from cust in db.Broandband_api_comm
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
            sb.Loan = (from cust in db.Loan_api_comm
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

            sb.Water = (from cust in db.Water_api_comm
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

            sb.Insurance = (from cust in db.Insurance_api_comm
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
            return View(sb);
        }
        public ActionResult Ledger_Report()
        {
            var userid = User.Identity.GetUserId();
            string txt_to_date1 = DateTime.Now.ToString();
            string frm_date = Convert.ToDateTime(txt_to_date1).ToShortDateString();
            string to_date = Convert.ToDateTime(txt_to_date1).AddDays(1).ToString();
            var ledger = db.Retailer_Cr_Dr_Report("API", userid, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            return View(ledger);
        }
        [HttpPost]
        public ActionResult Ledger_Report(DateTime txt_frm_date,DateTime txt_to_date)
        {
            var userid = User.Identity.GetUserId();
            ViewBag.chk = "post";
            string txtfrm = DateTime.Now.Date.ToString();
            txt_to_date = txt_to_date.AddDays(1);
            var ledger = db.Retailer_Cr_Dr_Report("API", userid, txt_frm_date, txt_to_date).ToList();
            return View(ledger);
        }
        public ActionResult ExcelLedger_Report(string txt_frm_date)
        {
            var userid = User.Identity.GetUserId();
            ViewBag.chk = "post";
            string txtfrm = DateTime.Now.Date.ToString();
            string frm_date = Convert.ToDateTime(txt_frm_date).Date.ToString();
            string to_date = Convert.ToDateTime(txt_frm_date).AddDays(1).ToString();
            var ledger = db.Retailer_Cr_Dr_Report("API", userid, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();



            DataTable dataTbl = new DataTable();
            dataTbl.Columns.Add("Request Id", typeof(string));
            dataTbl.Columns.Add("Particular", typeof(string));
            dataTbl.Columns.Add("Amount", typeof(string));
            dataTbl.Columns.Add("Credit", typeof(string));
            dataTbl.Columns.Add("Debit", typeof(string));
            dataTbl.Columns.Add("Balance", typeof(string));
            dataTbl.Columns.Add("Date", typeof(string));

            if (ledger.Count > 0)
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
            Response.AddHeader("content-disposition", "attachment; filename=Ledger_Report.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            return View(ledger);

        }

        public ActionResult PDFLedger_Report(string txt_frm_date)
        {
            var userid = User.Identity.GetUserId();
            ViewBag.chk = "post";
            string txtfrm = DateTime.Now.Date.ToString();
            string frm_date = Convert.ToDateTime(txt_frm_date).Date.ToString();
            string to_date = Convert.ToDateTime(txt_frm_date).AddDays(1).ToString();
            var ledger = db.Retailer_Cr_Dr_Report("API", userid, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            return new ViewAsPdf(ledger);

        }


        public ActionResult Bank_Information()
        {
            var ch = db.bank_info.Where(x => x.userid == "Admin").ToList();

            return View(ch);
        }
        public ActionResult Web_Login()
        {
            var userid = User.Identity.GetUserName();
            DateTime txt_frm_date = DateTime.Now.Date;
            DateTime txt_to_date = DateTime.Now;
            var Login_details = (db.Login_info.Where(aa => aa.UserId == userid && aa.CurrentLoginTime >= txt_frm_date && aa.CurrentLoginTime <= txt_to_date && aa.LoginFrom == "Web").Take(100).OrderByDescending(aa => aa.Idno)).ToList();
            return View(Login_details);
        }
        [HttpPost]
        public ActionResult Web_Login(string txt_frm_date, string txt_to_date)
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


        public ActionResult ExcelWeb_Login(string txt_frm_date, string txt_to_date)
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

            DataTable dataTbl = new DataTable();
            dataTbl.Columns.Add("Registered User Id", typeof(string));
            dataTbl.Columns.Add("IP Address", typeof(string));
            dataTbl.Columns.Add("Browser", typeof(string));
            dataTbl.Columns.Add("Lattitude", typeof(string));
            dataTbl.Columns.Add("Longitude", typeof(string));
            dataTbl.Columns.Add("Region", typeof(string));
            dataTbl.Columns.Add("Country", typeof(string));
            dataTbl.Columns.Add("ISP", typeof(string));
            dataTbl.Columns.Add("Current Login Details", typeof(string));


            if (Login_details.Count > 0)
            {


                foreach (var item in Login_details)
                {
                    dataTbl.Rows.Add(item.UserId, item.IP_Address, item.browser, item.Latitude, item.Logitude, "demy", "demy", "demy", item.CurrentLoginTime);
                }
            }
            else
            {
                dataTbl.Rows.Add("", "", "", "", "", "", "", "", "");
            }

            var grid = new GridView();
            grid.DataSource = dataTbl;
            grid.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=Web_Login.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();




            return View(Login_details);
        }


        public ActionResult PDFWeb_Login(string txt_frm_date, string txt_to_date)
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
            return new ViewAsPdf(Login_details);
        }

        public ActionResult Web_LoginFailed()
        {
            var userid = User.Identity.GetUserName();
            DateTime txt_frm_date = DateTime.Now.Date;
            DateTime txt_to_date = DateTime.Now;
            var Faild_Login_details = (db.Failed_Login_info.Where(aa => aa.EmailId == userid && aa.LoginTime >= txt_frm_date && aa.LoginTime <= txt_to_date && aa.LoginFrom == "Web").Take(100).OrderByDescending(aa => aa.Idno)).ToList();
            return View(Faild_Login_details);
        }
        [HttpPost]
        public ActionResult Web_LoginFailed(string txt_frm_date, string txt_to_date)
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

        public ActionResult ExcelWeb_LoginFailed(string txt_frm_date, string txt_to_date)
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

            DataTable dataTbl = new DataTable();
            dataTbl.Columns.Add("Registered User Id", typeof(string));
            dataTbl.Columns.Add("Password", typeof(string));
            dataTbl.Columns.Add("IP Address", typeof(string));
            dataTbl.Columns.Add("Browser", typeof(string));
            dataTbl.Columns.Add("Login Date", typeof(string));


            if (Faild_Login_details.Count > 0)
            {


                foreach (var item in Faild_Login_details)
                {
                    dataTbl.Rows.Add(item.EmailId, item.pwd, item.IP_Address, item.browser, item.LoginTime);
                }
            }
            else
            {
                dataTbl.Rows.Add("", "", "", "", "");
            }

            var grid = new GridView();
            grid.DataSource = dataTbl;
            grid.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=Web_LoginFailed.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();



            return View(Faild_Login_details);
        }
        public ActionResult Dispute_Report()
        {
            ViewData["success"] = TempData["success"];
            ViewData["failed"] = TempData["failed"];
            TempData.Remove("success");
            TempData.Remove("failed");
            var userid = User.Identity.GetUserId();
            string txt_frm_date = DateTime.Now.ToString();
            string txt_to_date = DateTime.Now.ToString();
            DateTime frm_date = Convert.ToDateTime(txt_frm_date).Date;
            DateTime to_date = Convert.ToDateTime(txt_to_date).AddDays(1);


            var disputelist = db.show_dispute_list(userid, frm_date, to_date).ToList();
            return View(disputelist);
        }
        [HttpPost]
        public ActionResult Dispute_Report(string txt_frm_date, string txt_to_date)
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
            var disputelist = db.show_dispute_list(userid, frm_date, to_date).ToList();
            return View(disputelist);
        }


        public ActionResult ExcelDispute_Report(string txt_frm_date, string txt_to_date)
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
            var disputelist = db.show_dispute_list(userid, frm_date, to_date).ToList();

            DataTable dataTbl = new DataTable();

            dataTbl.Columns.Add("Rch Number", typeof(string));
            dataTbl.Columns.Add("Operator Name", typeof(string));
            dataTbl.Columns.Add("Amount", typeof(string));
            dataTbl.Columns.Add("Recharge Date", typeof(string));
            dataTbl.Columns.Add("Dispute Reason", typeof(string));
            dataTbl.Columns.Add("Response", typeof(string));
            dataTbl.Columns.Add("Dispute Date", typeof(string));
            dataTbl.Columns.Add("Status", typeof(string));



            if (disputelist.Count > 0)
            {

                foreach (var item in disputelist)
                {

                    dataTbl.Rows.Add(item.rch_number, item.operator_Name, item.rch_amount, item.rch_time, item.dispute_region, item.comment,
                       item.dis_time, item.sts);

                }
            }
            else
            {
                dataTbl.Rows.Add("", "", "", "", "", "", "", "");
            }
            var grid = new GridView();
            grid.DataSource = dataTbl;
            grid.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=Dispute_Report.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();


            return View(disputelist);
        }

        public ActionResult PDFDispute_Report(string txt_frm_date, string txt_to_date)
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
            var disputelist = db.show_dispute_list(userid, frm_date, to_date).ToList();
            return new ViewAsPdf(disputelist);
        }

        //Day Book Report
        [HttpGet]
        public ActionResult api_Daybook_Report()
        {
            var userid = User.Identity.GetUserId();
            Daybookapireport rep = new Daybookapireport();
            string txt_frm_date = DateTime.Now.ToString();
            string txt_to_date = DateTime.Now.ToString();
            string frm_date = Convert.ToDateTime(txt_frm_date).Date.ToString();
            string to_date = Convert.ToDateTime(txt_to_date).AddDays(1).ToString();
            rep.DaybookLive = db.daybook_api_report(userid, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            return View(rep);
        }
        [HttpPost]
        public ActionResult api_Daybook_Report(string txt_frm_date)
        {
            var userid = User.Identity.GetUserId();
            ViewBag.chk = "post";
            Daybookapireport rep = new Daybookapireport();
            string txtfrm = DateTime.Now.Date.ToString();
            string frm_date = Convert.ToDateTime(txt_frm_date).Date.ToString();
            string to_date = Convert.ToDateTime(txt_frm_date).AddDays(1).ToString();
            if (txtfrm == frm_date)
            {
                rep.DaybookLive = db.daybook_api_report(userid, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            }
            else
            {
                rep.Daybook_Old = db.daybook_api_old_report(userid, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            }
            return View(rep);

        }
        public ActionResult Excelapi_Daybook_Report(string txt_frm_date)
        {
            var userid = User.Identity.GetUserId();
            ViewBag.chk = "post";
            Daybookapireport rep = new Daybookapireport();
            string txtfrm = DateTime.Now.Date.ToString();
            string frm_date = Convert.ToDateTime(txt_frm_date).Date.ToString();
            string to_date = Convert.ToDateTime(txt_frm_date).AddDays(1).ToString();
            if (txtfrm == frm_date)
            {
                rep.DaybookLive = db.daybook_api_report(userid, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            }
            else
            {
                rep.Daybook_Old = db.daybook_api_old_report(userid, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            }

            DataTable dataTbl = new DataTable();
            dataTbl.Columns.Add("Firm Name", typeof(string));
            dataTbl.Columns.Add("Recharge Bill", typeof(string));
            dataTbl.Columns.Add("IMPS", typeof(string));
            dataTbl.Columns.Add("AEPS", typeof(string));
            dataTbl.Columns.Add("M - POS", typeof(string));
            dataTbl.Columns.Add("Pancard", typeof(string));
            dataTbl.Columns.Add("Travel", typeof(string));
            dataTbl.Columns.Add("Purchase", typeof(string));
            dataTbl.Columns.Add("Opening Bal", typeof(string));
            dataTbl.Columns.Add("Closing Bal", typeof(string));
            dataTbl.Columns.Add("Old Day Refund", typeof(string));
            dataTbl.Columns.Add("Old Day Failed", typeof(string));
            dataTbl.Columns.Add("Diff", typeof(string));

            if (rep.DaybookLive.Count() > 0)
            {
                if (txtfrm == frm_date)
                {
                    foreach (var item in rep.DaybookLive)
                    {
                        dataTbl.Rows.Add(item.farmname == null ? item.username : item.farmname, item.RCH, item.IMPS, item.AEPS, "0.00", item.PAN, "0.00", item.PURCHASE, item.openbal, item.closebal, item.OLDDAYREFUND, item.OLDDAYFAILED, item.DIFF);
                    }
                }

                else
                {

                    foreach (var item in rep.Daybook_Old)
                    {
                        dataTbl.Rows.Add(item.farmname == null ? item.username : item.farmname, item.RCH, item.IMPS, item.AEPS, "0.00", item.PAN, "0.00", item.PURCHASE, item.openbal, item.closebal, item.OLDDAYREFUND, item.OLDDAYFAILED, item.DIFF);
                    }
                }

            }
            else
            {
                dataTbl.Rows.Add("", "", "", "", "", "", "", "", "", "", "", "", "");
            }


            var grid = new GridView();
            grid.DataSource = dataTbl;
            grid.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=api_Daybook_Report.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            return View(rep);

        }


        public ActionResult PDFapi_Daybook_Report(string txt_frm_date)
        {
            var userid = User.Identity.GetUserId();
            ViewBag.chk = "post";
            Daybookapireport rep = new Daybookapireport();
            string txtfrm = DateTime.Now.Date.ToString();
            string frm_date = Convert.ToDateTime(txt_frm_date).Date.ToString();
            string to_date = Convert.ToDateTime(txt_frm_date).AddDays(1).ToString();
            if (txtfrm == frm_date)
            {
                rep.DaybookLive = db.daybook_api_report(userid, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            }
            else
            {
                rep.Daybook_Old = db.daybook_api_old_report(userid, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            }
            return new ViewAsPdf(rep);

        }

        #region Manage
        public ActionResult Recharge_Url()
        {
            var userid = User.Identity.GetUserId();
            ViewData["Token"] = (db.api_user_details.Where(aa => aa.apiid == userid).Select(aa => aa.Token)).FirstOrDefault();
            var ch = (from tbl in db.Operator_Code select tbl);
            return View(ch);
        }
        public ActionResult Response_Url()
        {
            string userid = User.Identity.GetUserId();
            ViewData["Token"] = (db.api_user_details.Where(aa => aa.apiid == userid).Select(aa => aa.Token)).FirstOrDefault();

            var ch = db.API_select_apiupdateurl(userid);
            return View(ch);
        }
        [HttpPost]
        public ActionResult Response_Url(string apiurl)
        {
            string userid = User.Identity.GetUserId();
            db.API_insert_Updateurl_new(userid, apiurl);
            var ch = db.API_select_apiupdateurl(userid);
            TempData["success"] = "API Response Url Add Successfully..";
            return View(ch);
        }
        //[HttpPost]
        public ActionResult Edit_ResponseUrl(string txtid, string editapiurl)
        {
            try
            {
                db.update_updateurl(txtid, editapiurl);
                TempData["success"] = "API Response Url Edit Successfully..";
                return RedirectToAction("Response_Url");

            }
            catch
            {
                TempData["success"] = "API Response Url Not Edit..";
                return RedirectToAction("Response_Url");
            }
        }
        public ActionResult IPAddress()
        {
            string userid = User.Identity.GetUserId();
            ViewData["Token"] = (db.api_user_details.Where(aa => aa.apiid == userid).Select(aa => aa.Token)).FirstOrDefault();

            var ch = (from ff in db.Api_ip_address where ff.userid == userid select ff).ToList();
            return View(ch);
        }
        [HttpPost]
        public ActionResult insertapiip(string apiip)
        {
            string userid = User.Identity.GetUserId();
            Api_ip_address objCourse = new Api_ip_address();
            objCourse.ipaddress = apiip;
            objCourse.userid = userid;
            objCourse.sts = "N";
            TempData["success"] = "IP Address Insert Successfully..";
            db.Api_ip_address.Add(objCourse);
            db.SaveChanges();
            return RedirectToAction("IPAddress");
        }
        public ActionResult deleteip(string Id)
        {
            int idnn = Convert.ToInt32(Id);
            Api_ip_address objCourse = (from p in db.Api_ip_address
                                        where p.idno == idnn
                                        select p).SingleOrDefault();
            db.Api_ip_address.Remove(objCourse);
            TempData["success"] = "IP Address Deleted Successfully..";
            db.SaveChanges();
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public ActionResult Download()
        {
            var FileVirtualPath = "~/moneyapi/MoneyTransferAPI.zip";
            return File(FileVirtualPath, "application/force-download", Path.GetFileName(FileVirtualPath));
        }
        #endregion


        //Don't Change the Action Name If Any Changes Occurs tehn Is HarmFull For PassCode

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



                var Password = pass;
                var SMS_passcodesmsonline = db.SMSSendAlls.Where(x => x.ServiceName == "PasscodeOnline").SingleOrDefault();
                var Email_passcodesmsonline = db.EmailSendAlls.Where(x => x.ServiceName == "PasscodeOnline1").SingleOrDefault().Status;

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
                        var remmobile = db.Users.Where(x => x.UserId == userid).SingleOrDefault().PhoneNumber;
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




        public new ActionResult Profile()
        {
            string userid = User.Identity.GetUserId();
            var userDetails = db.Users.Where(a => a.UserId == userid).SingleOrDefault();
            var MD = db.api_user_details.FirstOrDefault(m => m.apiid == userid);
            ViewBag.MD_Details = MD;

            var gt = db.State_Desc.SingleOrDefault(a => a.State_id == MD.state)?.State_name;
            ViewBag.ddlstate = gt;
            var cities = db.District_Desc.SingleOrDefault(c => c.Dist_id == MD.district && c.State_id == MD.state)?.Dist_Desc;
            ViewBag.district = cities;

            ViewBag.Sate = db.State_Desc.Select(a => new SelectListItem { Text = a.State_name, Value = a.State_id.ToString() }).ToList();
            ViewBag.ALLDistrict = db.District_Desc.Where(a => a.State_id == MD.state).Select(a => new SelectListItem { Text = a.Dist_Desc, Value = a.Dist_id.ToString() }).ToList();
            ViewBag.passcodesetings = db.passcodesettings.Where(x => x.userid == userid).SingleOrDefault().passcodetype;
            ViewData["msg"] = TempData["success"];
            TempData.Remove("success");

            return View(userDetails);
        }

        public JsonResult Showapiprofile(string apiid)
        {
            var ch = db.api_user_details.Where(a => a.apiid == apiid).ToList();
            return Json(ch.ToArray(), JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public ActionResult UpdateapiProfile(string txtid1, string txtfrimname, string txtcity, string txtaddress, int txtzipcode, int State, int District)
        {
            try
            {
                var ad = db.api_user_details.Single(a => a.apiid == txtid1);
                ad.city = string.IsNullOrWhiteSpace(txtcity) ? ad.city : txtcity;
                ad.farmname = string.IsNullOrWhiteSpace(txtfrimname) ? ad.farmname : txtfrimname;
                ad.address = string.IsNullOrWhiteSpace(txtaddress) ? ad.address : txtaddress;
                ad.pincode = txtzipcode;
                ad.state = State;
                ad.district = District;
                db.SaveChanges();
            }
            catch
            {

            }
            TempData["success"] = "Update Successfully.";
            return RedirectToAction("Profile");
        }

        public JsonResult Showmobileandpancardprofile(string apiid)
        {
            var ch = db.api_user_details.Where(a => a.apiid == apiid).ToList();
            return Json(ch.ToArray(), JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public ActionResult UpdatePanccardandmobile(string txtid2, string txtname, string txtaadhaarcard, string txtpancard, string txtgst, string ddlPosition, string ddlBusinessType)
        {
            try
            {
                var ad = db.api_user_details.Single(a => a.apiid == txtid2);
                ad.username = string.IsNullOrWhiteSpace(txtname) ? ad.username : txtname;
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
        public JsonResult ShowBankinfo(string apiid)
        {
            var ch = db.api_user_details.Where(a => a.apiid == apiid).ToList();
            return Json(ch.ToArray(), JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public ActionResult UpdateBankinfromation(string txtid3, string txtaccholder, string txtbankaccountno, string txtifsc, string txtbankname, string txtbranchaddress)
        {
            try
            {
                var ad = db.api_user_details.Single(a => a.apiid == txtid3);
                ad.accountholder = string.IsNullOrWhiteSpace(txtaccholder) ? "" : txtaccholder;
                ad.Bankaccountno = string.IsNullOrWhiteSpace(txtbankaccountno) ? "" : txtbankaccountno;
                ad.Ifsccode = string.IsNullOrWhiteSpace(txtifsc) ? "" : txtifsc;
                ad.bankname = string.IsNullOrWhiteSpace(txtbankname) ? "" : txtbankname;
                ad.bankAddress = string.IsNullOrWhiteSpace(txtbranchaddress) ? "" : txtbranchaddress;
                db.SaveChanges();
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
                var ad = db.api_user_details.Single(a => a.apiid == txtaadharid);
                ad.aadharcardPath = string.IsNullOrWhiteSpace(imagePath) ? ad.aadharcardPath : imagePath;
                db.SaveChanges();
                TempData["success"] = "Aadhaar Doucument Upload Successfully.";
                return RedirectToAction("Profile");
            }
            catch
            {

            }
            return View();
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
                var ad = db.api_user_details.Single(a => a.apiid == txtpancardid);
                ad.pancardPath = string.IsNullOrWhiteSpace(imagePath) ? ad.pancardPath : imagePath;
                db.SaveChanges();
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
                var ad = db.api_user_details.Single(a => a.apiid == txtserviceid);
                ad.serviceagreementpath = string.IsNullOrWhiteSpace(imagePath) ? ad.serviceagreementpath : imagePath;
                db.SaveChanges();
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
                var ad = db.api_user_details.Single(a => a.apiid == txtRegistractionid);
                ad.Registractioncertificatepath = string.IsNullOrWhiteSpace(imagePath) ? ad.Registractioncertificatepath : imagePath;
                db.SaveChanges();
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
                var ad = db.api_user_details.Single(a => a.apiid == txtAddressproofid);
                ad.AddressProofpath = string.IsNullOrWhiteSpace(imagePath) ? ad.AddressProofpath : imagePath;
                db.SaveChanges();
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
                var ad = db.api_user_details.Single(a => a.apiid == txtprofileid);
                ad.Photo = string.IsNullOrWhiteSpace(imagePath) ? ad.Photo : imagePath;
                db.SaveChanges();
                TempData["success"] = "Profile Image Upload Successfully.";
                return RedirectToAction("Profile");
            }
            catch
            {

            }
            return View();
        }
        //delete Profile Doc 
        public JsonResult DelereprofileDoc(string apiid, string Docname)
        {
            if (apiid != null && Docname == "Aadhaar")
            {
                var ad = db.api_user_details.Single(a => a.apiid == apiid);
                ad.aadharcardPath = null;
                db.SaveChanges();
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            if (apiid != null && Docname == "Pancard")
            {
                var ad = db.api_user_details.Single(a => a.apiid == apiid);
                ad.pancardPath = null;
                db.SaveChanges();
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            if (apiid != null && Docname == "ServiceAgrrement")
            {
                var ad = db.api_user_details.Single(a => a.apiid == apiid);
                ad.serviceagreementpath = null;
                db.SaveChanges();
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            if (apiid != null && Docname == "RegistractionCertificate")
            {
                var ad = db.api_user_details.Single(a => a.apiid == apiid);
                ad.Registractioncertificatepath = null;
                db.SaveChanges();
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            if (apiid != null && Docname == "AddressProof")
            {
                var ad = db.api_user_details.Single(a => a.apiid == apiid);
                ad.AddressProofpath = null;
                db.SaveChanges();
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("Failed", JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult FillDistict(int State)
        {
            var cities = db.District_Desc.Where(c => c.State_id == State);
            return Json(cities, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ChangePassword()
        {
            return View();
        }
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

                TempData["Message"] = "Your Password has been Changed Successfully..";
                return RedirectToAction("ChangePassword");
            }
            AddErrors(result);
            return View(model);
        }


        public ActionResult TokenSetting()
        {
            var userid = User.Identity.GetUserId();
            var tokeninfo = db.Api_ip_address.Where(aa => aa.userid == userid).ToList();

            return View(tokeninfo);
        }
        [HttpPost]
        public ActionResult TokenSetting(string enterip, string otp)
        {
            var userid = User.Identity.GetUserId();
            var msg = "";
            var otpchk = db.OTP_IPAddress.Where(aa => aa.Clientid == userid && aa.type == enterip).OrderByDescending(aa => aa.Date).First().OTP;
            if (otpchk == otp)
            {
                //var chk = db.Api_ip_address.Where(aa => aa.userid == userid && aa.ipaddress == enterip).SingleOrDefault();
                //if (chk == null)
                //{
                var key = userid.Substring(0, 16);
                var token = Encrypt(enterip, key);
                Api_ip_address ip = new Api_ip_address();
                ip.ipaddress = enterip;
                ip.sts = "Y";
                ip.userid = userid;
                ip.token = token;
                db.Api_ip_address.Add(ip);
                db.SaveChanges();

                try
                {
                    var otpdel = db.OTP_IPAddress.Where(aa => aa.Clientid == userid).ToList();
                    db.OTP_IPAddress.RemoveRange(otpdel);
                    db.SaveChanges();


                }
                catch { }

                msg = "OK";
            }
            else
            {
                msg = "OTP is MissMatch";
            }

            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult tokenotp(string enterip)
        {
            var userid = User.Identity.GetUserId();
            var msg = "";
            var chk = db.Api_ip_address.Where(aa => aa.userid == userid && aa.ipaddress == enterip).SingleOrDefault();
            if (chk == null)
            {
                var email = db.Admin_details.SingleOrDefault().email;
                var apiinfo = db.api_user_details.Where(aa => aa.apiid == userid).SingleOrDefault();
                var otp = RandomNumber(111111, 999999);
                var text = otp + " is the OTP For Your ip Address " + enterip;
                OTP_IPAddress otpip = new OTP_IPAddress();
                otpip.Clientid = userid;
                otpip.Date = DateTime.Now;
                otpip.OTP = otp.ToString();
                otpip.OtpType = "IP";
                otpip.type = enterip;
                db.OTP_IPAddress.Add(otpip);
                db.SaveChanges();
                ALLSMSSend sendsmsall = new ALLSMSSend();
                //try
                //{
                //    string msgssss = "";
                //    string tempid = "";
                //    string urlss = "";

                //    var smsapionsts = db.apisms.Where(x => x.sts == "Y").SingleOrDefault();
                //    var smsstypes = db.Sending_SMS_Templates.Where(x => x.SMS_TYPE == "TOKENOTPFORIPADDRESS" && x.SMSAPIID == smsapionsts.id).SingleOrDefault();
                //    if (smsstypes != null)
                //    {



                //        msgssss = string.Format(smsstypes.Templates, otp, enterip);
                //        tempid = smsstypes.Templateid;
                //        urlss = smsapionsts.smsapi;

                //        smssend.sendsmsallnew(apiinfo.mobile, msgssss, urlss, tempid);
                //    }
                //}
                //catch { }

                smssend.sms_init("Y", "Y", "TOKENOTPFORIPADDRESS", apiinfo.mobile, otp, enterip);

                // sendsmsall.sendsmsall(apiinfo.mobile, text, "Recharge");
                sendsmsall.SendEmailAll(apiinfo.emailid, text, "IP Address Token", email);
                msg = "OK";
            }
            else
            {
                msg = "Same Ip Already Register";
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }
        private readonly Random _random = new Random();

        // Generates a random number within a range.      
        public int RandomNumber(int min, int max)
        {
            return _random.Next(min, max);
        }
        [HttpPost]
        public ActionResult DeleteToken(string token)
        {
            var userid = User.Identity.GetUserId();
            var tokenchk = db.Api_ip_address.Where(aa => aa.userid == userid && aa.token == token).SingleOrDefault();
            db.Api_ip_address.Remove(tokenchk);
            db.SaveChanges();
            return Json("", JsonRequestBehavior.AllowGet);
        }

        public ActionResult Operatorcode()
        {
            var chk = db.Operator_Code.Where(aa => aa.Operator_type == "Prepaid"
            || aa.Operator_type == "DTH" || aa.Operator_type == "PostPaid" ||
            aa.Operator_type == "Electricity" || aa.Operator_type == "Gas" ||
            aa.Operator_type == "Insurance" || aa.Operator_type == "Landline" ||
            aa.Operator_type == "LPG Gas" || aa.Operator_type == "Water" || aa.Operator_type == "Fastag" || aa.Operator_type == "Loan").ToList().OrderBy(aa => aa.Operator_type);
            return View(chk);
        }
        public ActionResult Balancecheck()
        {
            var websiteurl = db.Admin_details.SingleOrDefault().WebsiteUrl;
            websiteurl = "https://" + websiteurl + "/Recharge/Balance";
            ViewBag.website = websiteurl;
            return View();
        }

        public ActionResult StatusCheck()
        {
            var websiteurl = db.Admin_details.SingleOrDefault().WebsiteUrl;
            websiteurl = "https://" + websiteurl + "/Recharge/Status";
            ViewBag.website = websiteurl;
            return View();
        }

        public ActionResult ApiSellgst()
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
            var respp = db.apichk_purchase_info(userid, monthname, currentyear).ToList();
            IEnumerable<SelectListItem> selectList = from s in respp
                                                     select new SelectListItem
                                                     {
                                                         Value = s.apiid,
                                                         Text = s.farmname.ToString()
                                                     };

            ViewBag.apiid = new SelectList(selectList, "Value", "Text");
            return View(respp);
        }
        [HttpPost]
        public ActionResult ApiSellgst(string Month, string Year)
        {
            string apiid = User.Identity.GetUserId();
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
            var respp = db.apichk_purchase_info(apiid, Month, Year).ToList();
            IEnumerable<SelectListItem> selectList = from s in respp
                                                     select new SelectListItem
                                                     {
                                                         Value = s.apiid,
                                                         Text = s.farmname.ToString()
                                                     };

            ViewBag.apiid = new SelectList(selectList, "Value", "Text");
            return View(respp);
        }
        public ActionResult Print_sell_Gst_apiid(string Month, string Year)
        {
            string apiid = User.Identity.GetUserId();
            var userid = apiid;
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

            var userinfo = db.api_user_details.Where(aa => aa.apiid == apiid).SingleOrDefault();
            var sateinfo = db.State_Desc.Where(aa => aa.State_id == userinfo.state).SingleOrDefault();
            ViewBag.firmname = userinfo.farmname;
            ViewBag.address = userinfo.address;
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
            var entries = db.api_purchase_gst(userid, Month, Year).ToList();
            var chkk = db.Admin_details.SingleOrDefault().State;
            var statename = db.State_Desc.Where(aa => aa.State_id == chkk).SingleOrDefault().State_name;
            ViewBag.stname = statename;
            if (userinfo.state == chkk)
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

            ViewBag.invoice = "";
            return new ViewAsPdf("Print_sell_Gst_apiid", entries);
        }

        public ActionResult ApiPurchasegst()
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
            var respp = db.show_sell_invoice_api(userid, monthname, currentyear).ToList();
            IEnumerable<SelectListItem> selectList = from s in respp
                                                     select new SelectListItem
                                                     {
                                                         Value = s.apiid,
                                                         Text = s.farmname.ToString()
                                                     };

            ViewBag.apiid = new SelectList(selectList, "Value", "Text");
            return View(respp);
        }
        [HttpPost]
        public ActionResult ApiPurchasegst(string Month, string Year)
        {
            string apiid = User.Identity.GetUserId();
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
            var respp = db.show_sell_invoice_api(apiid, Month, Year).ToList();
            return View(respp);
        }
        public ActionResult Print_Purchase_Gst_Api(string Month, string Year)
        {
            string apiid = User.Identity.GetUserId();
            var userid = apiid;
            var today = DateTime.Today.AddMonths(-1);
            var admininfo = db.Admin_details.SingleOrDefault();
            ViewBag.adminfirm = admininfo.Companyname;
            ViewBag.adminaddress = admininfo.Address;
            ViewBag.pancardadmin = admininfo.pencardno;
            if (string.IsNullOrEmpty(admininfo.Gstno))
            {
                ViewBag.admingst = "UNREGISTERED";
            }
            else
            {
                ViewBag.admingst = admininfo.Gstno;
            }

            var userinfo = db.api_user_details.Where(aa => aa.apiid == apiid).SingleOrDefault();
            var sateinfo = db.State_Desc.Where(aa => aa.State_id == userinfo.state).SingleOrDefault();
            ViewBag.firmname = userinfo.farmname;
            ViewBag.address = userinfo.address;
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
            var entries = db.api_sell_gst(userid, Month, Year).ToList();
            var chkk = db.Admin_details.SingleOrDefault().State;
            var statename = db.State_Desc.Where(aa => aa.State_id == chkk).SingleOrDefault().State_name;
            ViewBag.stname = statename;
            if (userinfo.state == chkk)
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
            var invoice = db.Api_invoice_Numbers.Where(aa => aa.apiid == userid && aa.monthchkl == Month && aa.yearchk == Year).SingleOrDefault();
            //   DB.Repository<InvoiceNumber>().GetAll(aa => aa.UserID == userid && aa.type == "VERIFY" && aa.month == mn).SingleOrDefault();

            var newinvoice = invoice.invoiceno;
            ViewBag.invoice = newinvoice;
            return new ViewAsPdf("Print_Purchase_Gst_Api", entries);
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
            var chk = db.Api_upload_gst.Where(aa => aa.Apiid == userid && aa.monthchk == month && aa.yearchk == year).ToList();
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
            var chk = db.Api_upload_gst.Where(aa => aa.Apiid == userid && aa.monthchk == Month && aa.yearchk == Year).ToList();
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
                            fname = Path.Combine(Server.MapPath("~/UploadGst/API"), fname);
                            file.SaveAs(fname);
                            var chk = db.Api_upload_gst.Where(aa => aa.Apiid == userid && aa.monthchk == Month && aa.yearchk == Year && (aa.status == "Approved" || aa.status == "Pending")).SingleOrDefault();
                            if (chk == null)
                            {
                                Api_upload_gst up = new Api_upload_gst();
                                up.monthchk = Month;
                                up.Apiid = userid;
                                up.status = "Pending";
                                up.uploadfile = "UploadGst/API/" + filenm;
                                up.yearchk = Year;
                                db.Api_upload_gst.Add(up);
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

        public ActionResult ApiRechargeSellgst()
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
            var respp = db.api_purchase_info_prepaid(userid, monthname, currentyear).ToList();
            IEnumerable<SelectListItem> selectList = from s in respp
                                                     select new SelectListItem
                                                     {
                                                         Value = s.apiid,
                                                         Text = s.farmname.ToString()
                                                     };

            ViewBag.apiid = new SelectList(selectList, "Value", "Text");
            return View(respp);
        }
        [HttpPost]
        public ActionResult ApiRechargeSellgst(string Month, string Year)
        {
            string apiid = User.Identity.GetUserId();
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
            var respp = db.api_purchase_info_prepaid(apiid, Month, Year).ToList();
            IEnumerable<SelectListItem> selectList = from s in respp
                                                     select new SelectListItem
                                                     {
                                                         Value = s.apiid,
                                                         Text = s.farmname.ToString()
                                                     };

            ViewBag.apiid = new SelectList(selectList, "Value", "Text");
            return View(respp);
        }
        public ActionResult Print_sell_Gst_recharge_apiid(string Month, string Year)
        {
            string apiid = User.Identity.GetUserId();
            var userid = apiid;
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

            var userinfo = db.api_user_details.Where(aa => aa.apiid == apiid).SingleOrDefault();
            var sateinfo = db.State_Desc.Where(aa => aa.State_id == userinfo.state).SingleOrDefault();
            ViewBag.firmname = userinfo.farmname;
            ViewBag.address = userinfo.address;
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
            var entries = db.retailer_purchase_gst_prepaid(userid, Month, Year).ToList();
            var chkk = db.Admin_details.SingleOrDefault().State;
            var statename = db.State_Desc.Where(aa => aa.State_id == chkk).SingleOrDefault().State_name;
            ViewBag.stname = statename;
            if (userinfo.state == chkk)
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

            ViewBag.invoice = "";
            return new ViewAsPdf("Print_sell_Gst_recharge_apiid", entries);
        }

        public ActionResult uploadgstRecharge()
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
            var chk = db.Api_Recharge_upload_gst.Where(aa => aa.Apiid == userid && aa.monthchk == month && aa.yearchk == year).ToList();
            return View(chk);
        }
        [HttpPost]
        public ActionResult uploadgstRecharge(string Month, string Year)
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
            var chk = db.Api_Recharge_upload_gst.Where(aa => aa.Apiid == userid && aa.monthchk == Month && aa.yearchk == Year).ToList();
            return View(chk);
        }
        public ActionResult uploadgstfileRecharge()
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
                            fname = "RECHARGE" + Month + Year + userid + fname;
                            string filenm = fname;
                            fname = Path.Combine(Server.MapPath("~/UploadGst/API"), fname);
                            file.SaveAs(fname);
                            var chk = db.Api_Recharge_upload_gst.Where(aa => aa.Apiid == userid && aa.monthchk == Month && aa.yearchk == Year && (aa.status == "Approved" || aa.status == "Pending")).SingleOrDefault();
                            if (chk == null)
                            {
                                Api_Recharge_upload_gst up = new Api_Recharge_upload_gst();
                                up.monthchk = Month;
                                up.Apiid = userid;
                                up.status = "Pending";
                                up.uploadfile = "UploadGst/API/" + filenm;
                                up.yearchk = Year;
                                db.Api_Recharge_upload_gst.Add(up);
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

            var chk = db.gateway_report(userid, "Api", from, to, "");

            var getwaytotals = db.Payment_Gateway_Txn_history.Where(x => x.userid == userid && x.roles == "Api" && x.f_date >= from && x.f_date <= to);
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

            var chk = db.gateway_report(userid, "Api", txt_frm_date, to, "");

            var getwaytotals = db.Payment_Gateway_Txn_history.Where(x => x.userid == userid && x.roles == "Api" && x.f_date >= txt_frm_date && x.f_date <= to);
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

            var chk = db.gateway_report(userid, "Api", txt_frm_date, to, "");

            var getwaytotals = db.Payment_Gateway_Txn_history.Where(x => x.userid == userid && x.roles == "Api" && x.f_date >= txt_frm_date && x.f_date <= to);
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

            var chk = db.gateway_report(userid, "Api", txt_frm_date, to, "").ToList();
            DataTable dataTbl = new DataTable();
            dataTbl.Columns.Add("Mode Mode", typeof(string));
            dataTbl.Columns.Add("Amount", typeof(string));
            dataTbl.Columns.Add("Charges", typeof(string));
            dataTbl.Columns.Add("Net Received", typeof(string));
            dataTbl.Columns.Add("Bank RRN ", typeof(string));
            dataTbl.Columns.Add("Transaction Time ", typeof(string));
            dataTbl.Columns.Add("Response Time", typeof(string));

            if (chk.Count() > 0)
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

            if (ddl_type == "")
            {
                TempData["msg"] = "Select Any Type";
                return RedirectToAction("GatewayTRANSFER");
            }

            string txnid = Generatetxnid();
            var checKYC = db.api_user_details.Where(x => x.apiid == userid).SingleOrDefault();
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
                    var msg = db.PaymentGateway_Fund_insert("API", userid, txtamt, txnid, ddl_type, "", output).SingleOrDefault().msg;
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
                        var ApiUserName = checKYC.username;
                        var Email = checKYC.emailid;
                        //  var salt = "i0dSyipJyJAzKovgBSPhWfUQAsj1GGIZ";
                        var salt = auth.merchantsalt;
                        RemotePost myremotepost = new RemotePost();
                        //  myremotepost.Url = "https://test.payu.in/_payment";
                        myremotepost.Url = "https://secure.payu.in/_payment";
                        myremotepost.Add("key", key);

                        myremotepost.Add("txnid", txnid);
                        myremotepost.Add("amount", txtamt.ToString());
                        myremotepost.Add("productinfo", "Fund Transfer");
                        myremotepost.Add("firstname", ApiUserName);
                        myremotepost.Add("phone", checKYC.mobile);
                        myremotepost.Add("email", checKYC.emailid);
                        myremotepost.Add("surl", url);//Change the success url here depending upon the port number of your local system.
                        myremotepost.Add("furl", url);//Change the failure url here depending upon the port number of your local system.
                        myremotepost.Add("service_provider", "payu_paisa");
                        myremotepost.Add("drop_category", dropcategory);
                        string hashString = key + "|" + txnid + "|" + txtamt + "|" + "Fund Transfer" + "|" + ApiUserName + "|" + Email + "|||||||||||" + salt;
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
        public ActionResult WhatsappDocument()
        {
            return View();
        }
        public ActionResult WhatsappLogin()
        {
            var userid = User.Identity.GetUserId();
            var chkk = db.Whatsapp_user_details.Where(aa => aa.userid == userid).ToList();
            if (chkk.Count > 0)
            {
                if (chkk[0].qrdeatils != "")
                {
                    var qrvalue = chkk[0].qrdeatils;
                    var bw = new ZXing.BarcodeWriter();

                    var encOptions = new ZXing.Common.EncodingOptions
                    {
                        Width = 500,
                        Height = 500,
                        Margin = 0,
                        PureBarcode = false
                    };
                    bw.Renderer = new ZXing.Rendering.BitmapRenderer();
                    bw.Options = encOptions;
                    bw.Format = ZXing.BarcodeFormat.QR_CODE;
                    using (MemoryStream memory = new MemoryStream())
                    {
                        Bitmap bm = bw.Write(qrvalue);
                        bm.Save(memory, ImageFormat.Png);
                        ViewBag.qr = "data:image/png;base64," + Convert.ToBase64String(memory.ToArray());
                    }
                }
            }
            return View(chkk);
        }
        [HttpPost]
        public ActionResult whatsapp_login_insert(string mobile)
        {
            var userid = User.Identity.GetUserId();
            var msg = "";
            var purchasecheck = db.Whatsapp_purchase.Where(aa => aa.apiid == userid && aa.status.ToUpper() == "SUCCESS").ToList();
            if (purchasecheck.Count > 0)
            {
                var expdate = purchasecheck.OrderByDescending(aa => aa.purchasedate).Take(1).SingleOrDefault().renewaldate;
                var purchasedate = purchasecheck.OrderByDescending(aa => aa.purchasedate).Take(1).SingleOrDefault().purchasedate;
                if (expdate >= DateTime.Now)
                {
                    var chkk = db.Whatsapp_user_details.Where(aa => aa.userid == userid).SingleOrDefault();
                    if (chkk == null)
                    {
                        VastBazaartoken vbtoken = new VastBazaartoken();
                        var btoken = vbtoken.gettoken();
                        var url = "http://api.vastbazaar.com/Api/Web/WhatsAppLoginInsert?mobile=" + mobile + "&apiid=" + userid + "";
                        var client = new RestClient(url);
                        client.Timeout = -1;
                        var request = new RestRequest(Method.POST);
                        request.AddHeader("Authorization", "Bearer " + btoken);
                        IRestResponse response = client.Execute(request);
                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            var resp1 = response.Content;
                            dynamic reschkk = JsonConvert.DeserializeObject(resp1);
                            var sts = reschkk.Content.ADDINFO.status;
                            if (sts == true)
                            {
                                var servicenm = reschkk.Content.ADDINFO.sname;
                                Whatsapp_user_details wt = new Whatsapp_user_details();
                                wt.loginstatus = "Pending";
                                wt.logintime = DateTime.Now;
                                wt.mobile = mobile;
                                wt.qrdeatils = "";
                                wt.servicename = servicenm;
                                wt.userid = userid;
                                wt.purchasedate = purchasedate;
                                wt.expdate = expdate;
                                db.Whatsapp_user_details.Add(wt);
                                db.SaveChanges();
                                msg = "OK";
                            }
                            else
                            {
                                msg = reschkk.Content.ADDINFO.msg;
                                //msg
                            }
                        }
                    }
                    else
                    {
                        msg = "Allow Only One Number";
                    }
                }
                else
                {
                    msg = "Plan Expire";
                }
            }
            else
            {
                msg = "Firstly Purchase Any Plan";
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult whatsapp_login(int id)
        {
            var userid = User.Identity.GetUserId();
            var msg = "";
            var chk = db.Whatsapp_user_details.Where(aa => aa.userid == userid).SingleOrDefault();
            if (chk != null)
            {
                var purchasecheck = db.Whatsapp_purchase.Where(aa => aa.apiid == userid).ToList();
                if (purchasecheck.Count > 0)
                {
                    var expdate = purchasecheck.OrderByDescending(aa => aa.purchasedate).Take(1).SingleOrDefault().renewaldate;
                    if (expdate >= DateTime.Now)
                    {
                        VastBazaartoken vbtoken = new VastBazaartoken();
                        var btoken = vbtoken.gettoken();
                        var url = "http://api.vastbazaar.com/Api/Web/whatsapp_login?apiid=" + userid + "";
                        var client = new RestClient(url);
                        client.Timeout = -1;
                        var request = new RestRequest(Method.POST);
                        request.AddHeader("Authorization", "Bearer " + btoken);
                        IRestResponse response = client.Execute(request);
                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            var resp1 = response.Content;
                            dynamic reschkk = JsonConvert.DeserializeObject(resp1);
                            var sts = reschkk.Content.ADDINFO.sts;
                            if (sts == true)
                            {
                                msg = reschkk.Content.ADDINFO.msg;
                                if (msg == "Inprocess")
                                {
                                    chk.qrdeatils = reschkk.Content.ADDINFO.qrcode;
                                    chk.loginstatus = "Inprocess";
                                    chk.logintime = DateTime.Now;
                                    db.SaveChanges();
                                    msg = "OK";
                                }
                                else if (msg == "LOGIN")
                                {
                                    chk.qrdeatils = "";
                                    chk.loginstatus = "LOGIN";
                                    chk.logintime = DateTime.Now;
                                    db.SaveChanges();
                                    msg = "OK";
                                }
                            }
                            else
                            {
                                msg = reschkk.Content.ADDINFO.msg;
                                //msg
                            }
                        }
                    }
                    else
                    {
                        msg = "Plan Expire";
                        //plan Expire
                    }
                }
                else
                {
                    msg = "Firstly Purchase Any Plan";
                    //prchase first
                }
            }
            else
            {
                msg = "Firstlly Register Any Whats App Number";
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult whatsapp_Refresh(int id)
        {
            var userid = User.Identity.GetUserId();
            var msg = "";
            var chk = db.Whatsapp_user_details.Where(aa => aa.userid == userid).SingleOrDefault();
            var purchasecheck = db.Whatsapp_purchase.Where(aa => aa.apiid == userid).ToList();
            if (purchasecheck.Count > 0)
            {
                var expdate = purchasecheck.OrderByDescending(aa => aa.purchasedate).Take(1).SingleOrDefault().renewaldate;
                if (expdate >= DateTime.Now)
                {
                    VastBazaartoken vbtoken = new VastBazaartoken();
                    var btoken = vbtoken.gettoken();
                    var url = "http://api.vastbazaar.com/Api/Web/whatsapp_Refresh?apiid=" + userid + "";
                    var client = new RestClient(url);
                    client.Timeout = -1;
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("Authorization", "Bearer " + btoken);
                    IRestResponse response = client.Execute(request);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var resp1 = response.Content;
                        dynamic reschkk = JsonConvert.DeserializeObject(resp1);
                        var sts = reschkk.Content.ADDINFO.sts;
                        if (sts == true)
                        {
                            msg = reschkk.Content.ADDINFO.msg;
                            if (msg == "Inprocess")
                            {
                                chk.qrdeatils = reschkk.Content.ADDINFO.qrcode;
                                chk.loginstatus = "Inprocess";
                                chk.logintime = DateTime.Now;
                                db.SaveChanges();
                                msg = "Inprocess";
                            }
                            else if (msg == "LOGIN")
                            {
                                chk.qrdeatils = "";
                                chk.loginstatus = "LOGIN";
                                chk.logintime = DateTime.Now;
                                db.SaveChanges();
                                msg = "LOGIN";
                            }
                        }
                        else
                        {
                            msg = reschkk.Content.ADDINFO.msg;
                            //msg
                        }
                    }
                }
                else
                {
                    msg = "Plan Expire";
                    //plan Expire
                }
            }
            else
            {
                msg = "Firstly Purchase Any Plan";
                //prchase first
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult whatsapp_Status(int id)
        {
            var userid = User.Identity.GetUserId();
            var msg = "";
            var purchasecheck = db.Whatsapp_purchase.Where(aa => aa.apiid == userid).ToList();
            var chk = db.Whatsapp_user_details.Where(aa => aa.userid == userid).SingleOrDefault();
            if (purchasecheck.Count > 0)
            {
                var expdate = purchasecheck.OrderByDescending(aa => aa.purchasedate).Take(1).SingleOrDefault().renewaldate;
                if (expdate >= DateTime.Now)
                {
                    VastBazaartoken vbtoken = new VastBazaartoken();
                    var btoken = vbtoken.gettoken();
                    var url = "http://api.vastbazaar.com/Api/Web/whatsapp_Status?apiid=" + userid + "";
                    var client = new RestClient(url);
                    client.Timeout = -1;
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("Authorization", "Bearer " + btoken);
                    IRestResponse response = client.Execute(request);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var resp1 = response.Content;
                        dynamic reschkk = JsonConvert.DeserializeObject(resp1);
                        var sts = reschkk.Content.ADDINFO;
                        if (sts == "NOT CONNECTED")
                        {
                            msg = reschkk.Content.ADDINFO;
                            chk.qrdeatils = "";
                            chk.loginstatus = "Pending";
                            chk.logintime = DateTime.Now;
                            db.SaveChanges();
                        }
                        else
                        {
                            msg = "CONNECTED";
                            //msg
                        }
                    }
                }
                else
                {
                    msg = "Plan Expire";
                    //plan Expire
                }
            }
            else
            {
                msg = "Firstly Purchase Any Plan";
                //prchase first
            }

            return Json(msg, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult whatsapp_logout(int id)
        {
            var userid = User.Identity.GetUserId();
            var msg = "";
            var purchasecheck = db.Whatsapp_purchase.Where(aa => aa.apiid == userid).ToList();
            var chk = db.Whatsapp_user_details.Where(aa => aa.userid == userid).SingleOrDefault();
            if (purchasecheck.Count > 0)
            {
                var expdate = purchasecheck.OrderByDescending(aa => aa.purchasedate).Take(1).SingleOrDefault().renewaldate;
                if (expdate >= DateTime.Now)
                {
                    VastBazaartoken vbtoken = new VastBazaartoken();
                    var btoken = vbtoken.gettoken();
                    var url = "http://api.vastbazaar.com/Api/Web/whatsapp_logout?apiid=" + userid + "";
                    var client = new RestClient(url);
                    client.Timeout = -1;
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("Authorization", "Bearer " + btoken);
                    IRestResponse response = client.Execute(request);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var resp1 = response.Content;
                        dynamic reschkk = JsonConvert.DeserializeObject(resp1);
                        var sts = reschkk.Content.ADDINFO;
                        if (sts == "DONE")
                        {
                            msg = reschkk.Content.ADDINFO;
                            chk.qrdeatils = "";
                            chk.loginstatus = "Pending";
                            chk.logintime = DateTime.Now;
                            db.SaveChanges();
                        }
                        else
                        {
                            msg = "NOT DONE";
                            //msg
                        }
                    }
                }
                else
                {
                    msg = "Plan Expire";
                    //plan Expire
                }
            }
            else
            {
                msg = "Firstly Purchase Any Plan";
                //prchase first
            }

            return Json(msg, JsonRequestBehavior.AllowGet);
        }
        public ActionResult WhatsappPurchase()
        {
            var userid = User.Identity.GetUserId();
            Whatsapp_purchase_new model = new Whatsapp_purchase_new();
            model.userwise = db.Whatsapp_userWise.Where(aa => aa.Apiid == userid).ToList();
            model.Report = db.Whatsapp_purchase.Where(aa => aa.apiid == userid).OrderByDescending(aa => aa.purchasedate).ToList();
            return View(model);
        }
        [HttpPost]
        public ActionResult WhatsappPurchase(string idno, bool gsten)
        {
            VastBazaartoken Responsetoken = new VastBazaartoken();
            var msg = "";
            if (idno == "1" || idno == "2" || idno == "3")
            {
                var plannm = "plan" + idno;
                var month = 0;
                if (idno == "1")
                {
                    month = 1;
                }
                else if (idno == "2")
                {
                    month = 3;
                }
                else
                {
                    month = 6;
                }
                var userid = User.Identity.GetUserId();
                System.Data.Entity.Core.Objects.ObjectParameter output = new System.Data.Entity.Core.Objects.ObjectParameter("output", typeof(string));
                var chkk = db.WhatsappPurchase(userid, plannm, month, gsten, output).SingleOrDefault().msg;
                if (chkk == "OK")
                {
                    var apiinfo = db.api_user_details.Where(aa => aa.apiid == userid).SingleOrDefault();
                    var apivast = userid + "|" + apiinfo.farmname;
                    var token = Responsetoken.gettoken();
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    var websiteurl = db.Admin_details.SingleOrDefault().WebsiteUrl;
                    var client = new RestClient("http://api.vastbazaar.com/Api/Web/WhatsAppPurchase?apiid=" + apivast);
                    client.Timeout = -1;
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("Authorization", "Bearer " + token);
                    var resp = new
                    {
                        websiteurl = websiteurl,
                        servicenm = "WHATSAPP",
                        quantity = month,
                        gststs = gsten
                    };
                    var reqbody = JsonConvert.SerializeObject(resp);
                    request.AddParameter("application/json", reqbody, ParameterType.RequestBody);
                    IRestResponse response = client.Execute(request);
                    dynamic res = JsonConvert.DeserializeObject(response.Content);
                    string outmsg = res.Content.ADDINFO;
                    if (outmsg.ToUpper() == "PURCHASE DONE")
                    {
                        msg = "Purchase Done";
                        var infochk = db.Whatsapp_purchase.Where(aa => aa.apiid == userid).OrderByDescending(aa => aa.purchasedate).Take(1).SingleOrDefault();
                        System.Data.Entity.Core.Objects.ObjectParameter output1 = new System.Data.Entity.Core.Objects.ObjectParameter("output", typeof(string));
                        db.updateWhatsapppurchase(infochk.idno, "Success", output1);
                    }
                    else if (outmsg.ToUpper() == "YOUR REMAIN BALANCE LOW")
                    {
                        msg = "Purchase Not Done, Contact To Admin";
                        var infochk = db.Whatsapp_purchase.Where(aa => aa.apiid == userid).OrderByDescending(aa => aa.purchasedate).Take(1).SingleOrDefault();
                        System.Data.Entity.Core.Objects.ObjectParameter output1 = new System.Data.Entity.Core.Objects.ObjectParameter("output", typeof(string));
                        db.updateWhatsapppurchase(infochk.idno, "Failed", output1);
                    }
                }
                else
                {
                    msg = chkk;
                }
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Whatsapp_Report()
        {
            var userid = User.Identity.GetUserId();
            string txt_frm_date = DateTime.Now.ToString();
            string txt_to_date = DateTime.Now.ToString();
            DateTime frm_date = Convert.ToDateTime(txt_frm_date).Date;
            DateTime to_date = Convert.ToDateTime(txt_to_date).AddDays(1);
            var disputelist = db.whatsapp_report(userid, frm_date, to_date).ToList();
            return View(disputelist);
        }
        [HttpPost]
        public ActionResult Whatsapp_Report(string txt_frm_date, string txt_to_date)
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
            var disputelist = db.whatsapp_report(userid, frm_date, to_date).ToList();
            return View(disputelist);
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


        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
    }
}