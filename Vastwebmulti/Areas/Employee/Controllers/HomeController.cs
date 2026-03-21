using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using java.awt;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using org.vwipl;
using VWMain = org.vwipl.Main;
using RestSharp;
using Rotativa;
using sun.security.tools.keytool;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using Vastwebmulti.Areas.ADMIN.Controllers;
using Vastwebmulti.Areas.ADMIN.Models;
using Vastwebmulti.Areas.Employee.Model;
using Vastwebmulti.Areas.Employee.ViewModel;
using Vastwebmulti.Areas.RETAILER.Models;
using Vastwebmulti.Areas.RETAILER.ViewModels;
using Vastwebmulti.Hubs;
using Vastwebmulti.Models;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using DocumentFormat.OpenXml.Vml;

namespace Vastwebmulti.Areas.Employee.Controllers
{
    [Authorize(Roles = "Employee")]

    public class HomeController : Controller
    {
        // GET: Employee/Home

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
        AppNotification notify = new AppNotification();
        ALLSMSSend smssend = new ALLSMSSend();
        private VastwebmultiEntities db;

        VastBazaartoken Responsetoken = new VastBazaartoken();
        string VastbazaarBaseUrl = "http://api.vastbazaar.com/";
        string Websitename = "";
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


        public HomeController()
        {
            db = new VastwebmultiEntities();
        }

        #region Profile
        [HttpGet]
        public new ActionResult Profile()
        {
            var userid = User.Identity.GetUserId();
            var userDetails = db.Users.Where(a => a.UserId == userid).SingleOrDefault();
            var admindetails = db.tbl_Admin_Employee.Where(m => m.EmployeeID == userid).SingleOrDefault();
            ViewBag.admin = admindetails;
            int state = Convert.ToInt32(admindetails.State);
            int district = Convert.ToInt32(admindetails.District);
            ViewBag.ddlstate = db.State_Desc.Where(a => a.State_id == state).SingleOrDefault().State_name;
            ViewBag.dddistrict = db.District_Desc.Where(a => a.Dist_id == district && a.State_id == state).SingleOrDefault().Dist_Desc;
            ViewBag.Sate = db.State_Desc.Select(a => new SelectListItem { Text = a.State_name, Value = a.State_id.ToString() }).ToList();
            ViewBag.District = db.District_Desc.Where(a => a.State_id == state).Select(a => new SelectListItem { Text = a.Dist_Desc, Value = a.Dist_id.ToString() }).ToList();
            return View(userDetails);
        }
        public JsonResult FillDistict(int State)
        {
            var cities = db.District_Desc.Where(c => c.State_id == State);
            return Json(cities, JsonRequestBehavior.AllowGet);
        }
        #endregion

        public ActionResult Dashboard()
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                var userid = User.Identity.GetUserId();
                var emp = db.tbl_Admin_Employee.Where(x => x.EmployeeID == userid).SingleOrDefault();
                var ch = db.Total_account().ToList();
                EmployeeInformation empss = new EmployeeInformation();
                ViewData["Rem"] = ch.Single().totalrem;
                ViewData["posbal"] = ch.Single().totalposremain;
                empss.ename = emp.Employee_name;
                empss.Eaddress = emp.Address;
                empss.epincode = emp.Pincode.Value;
                empss.pancard = emp.PanCard;
                empss.adharcard = emp.AadharCard;
                return View(empss);
            }
        }

        #region Live Recharge
        //Live Recharge
        [HttpGet]
        [PermissioncheckingAttribute(servicename = "PREPAIDUTILITY", permision = "Read")]
        public ActionResult Operator_report_new(string prtname, string type, string ddlusers, string allmaster1, string alldealer, string allretailer, string Whitelabel, string API, string ddl_status, string Operator)
        {
            try
            {
                ViewBag.ReportType = "TODAY";
                if (type == null)
                {
                    ViewBag.type = "live";
                }
                else
                {
                    ViewBag.type = type;
                }
                var portname = "ALL";
                if (prtname != null)
                {
                    portname = db.PortManagers.Where(aa => aa.PortNm == prtname).Take(1).FirstOrDefault().PortNo;
                }
                string txt_frm_date = DateTime.Now.ToString();
                string txt_to_date = DateTime.Now.ToString();
                string frm_date = Convert.ToDateTime(txt_frm_date).Date.ToString("yyyy-MM-dd");
                string to_date = Convert.ToDateTime(txt_to_date).AddDays(1).ToString("yyyy-MM-dd");
                var query1 = db.SRS_code_all("").ToList();
                SelectList objmodeldata = new SelectList(query1, "mars", "mars", 0);
                var operator_value = db.port_list();
                ViewBag.Lapuno = new SelectList(operator_value, "PortNo", "PortNo", portname);
                var operator_value1 = db.Opt_codes_new();
                ViewBag.Operatornew = new SelectList(operator_value1, "new_opt_code", "operator_Name").ToList();
                Vastwebmulti.Areas.Employee.ViewModel.EmployeeViewModel viewModel = new Vastwebmulti.Areas.Employee.ViewModel.EmployeeViewModel();
                viewModel.CountryListModel = objmodeldata;
                viewModel.srspending = db.SRS_pending_count_ALL();
                viewModel.srspending1 = db.SRS_pending_count_ALL();

                var stands = db.Superstokist_details.ToList();
                IEnumerable<SelectListItem> selectList = from s in stands
                                                         select new SelectListItem
                                                         {
                                                             Value = s.SSId,
                                                             Text = s.FarmName.ToString() + " - " + s.Mobile
                                                         };
                ViewBag.allmaster = new SelectList(selectList, "Value", "Text");
                ViewBag.alldealer = new SelectList(db.select_Dealer_for_ddl(), "Dealerid", "DealerName", null).ToList();
                ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl("Admin"), "RetailerId", "Frm_Name", null).ToList();
                ViewBag.API = new SelectList(db.API_all_apiid(), "apiid", "farmname");
                ViewBag.whitelabel = new SelectList(db.Whitelabel_all_whitelabelid(), "WhiteLabelID", "FrmName");

                //int pagesize = 35;
                //var ch = db.proc_Recharge_operator_report_newPaging(1, pagesize, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), "Admin", "ALL", "ALL", "ALL", "ALL", "ALL").ToList();

                //viewModel.proc_Recharge_operator_report_newPaging = ch;
                //return View(viewModel);
                return View(viewModel);
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult Operator_report_new(string ddlusers, string allmaster1, string alldealer, string allretailer, string API, string Whitelabel, string txt_frm_date, string txtmob, string txt_to_date, string ddl_status, string txtdemo, string ddl_top, string lapuno11, string Operator)
        {
            ViewBag.ddlusers = ddlusers;
            if (ddlusers == "Master")
            {
                ViewBag.user = allmaster1;
            }
            else if (ddlusers == "Dealer")
            {
                ViewBag.user = alldealer;
            }
            else if (ddlusers == "Retailer")
            {
                ViewBag.user = allretailer;
            }
            else if (ddlusers == "API")
            {
                ViewBag.user = API;
            }
            else if (ddlusers == "WAdmin")
            {
                ViewBag.user = Whitelabel;
            }
            string userid = User.Identity.GetUserId();
            ViewBag.chk = "post";
            var portname = "";
            if (portname == "ALL")
            {
                portname = null;
            }

            var operator_value = db.port_list();
            ViewBag.Lapuno = new SelectList(operator_value, "PortNo", "PortNo");

            var operator_value1 = db.Opt_codes_new().Distinct().ToList();
            ViewBag.Operatornew = new SelectList(operator_value1, "new_opt_code", "operator_Name", null);

            var query1 = db.SRS_code_all(lapuno11).ToList();
            SelectList objmodeldata = new SelectList(query1, "mars", "mars", "-select-");
            EmployeeViewModel viewModel = new EmployeeViewModel();
            viewModel.CountryListModel = objmodeldata;

            viewModel.srspending = db.SRS_pending_count_ALL();
            viewModel.srspending1 = db.SRS_pending_count_ALL();


            var stands = db.Superstokist_details.ToList();
            IEnumerable<SelectListItem> selectList = from s in stands
                                                     select new SelectListItem
                                                     {
                                                         Value = s.SSId,
                                                         Text = s.FarmName.ToString()
                                                     };
            ViewBag.allmaster = new SelectList(selectList, "Value", "Text");
            ViewBag.alldealer = new SelectList(db.select_Dealer_for_ddl(), "Dealerid", "DealerName", null).ToList();
            ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl("Admin"), "RetailerId", "Frm_Name", null).ToList();
            ViewBag.API = new SelectList(db.API_all_apiid(), "apiid", "farmname");
            ViewBag.whitelabel = new SelectList(db.Whitelabel_all_whitelabelid(), "WhiteLabelID", "FrmName");
            return View(viewModel);
        }

        [ChildActionOnly]
        //[HttpPost]
        public ActionResult _showRechargeReport(string txt_frm_date, string txt_to_date, string txtmob, string ddl_status, string lapuno11, string Operator, string ddlusers, string allmaster1, string alldealer, string allretailer, string Whitelabel, string API, string txtdemo)
        {
            var optname = "";
            var portname = "";
            var mobile = "";
            var ddlst = "";
            string userid = User.Identity.GetUserId();

            if (ddlusers == "Master")
            {
                if (allmaster1 == "" || allmaster1.Contains("Master") || allmaster1 == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = allmaster1;
                }
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
            if (ddlusers == "WAdmin")
            {
                if (Whitelabel == "" || Whitelabel.Contains("Select Whitelabel") || Whitelabel == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = Whitelabel;
                }
            }
            if (ddlusers == "API")
            {
                if (API == "" || API.Contains("Select API") || API == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = API;
                }
            }
            if (ddlusers == "" || ddlusers == null || ddlusers == "Admin")
            {
                userid = "ALL";
                ddlusers = "Admin";
            }


            if ((txt_frm_date == null && txt_to_date == null) || (txt_frm_date == "" && txt_to_date == ""))
            {
                txt_frm_date = DateTime.Now.ToString();
                txt_to_date = DateTime.Now.ToString();

            }
            if (ddl_status == "" || ddl_status == null || ddl_status.ToUpper().Contains("ALL STATUS"))
            {
                //optname = "ALL";
                ddlst = "ALL";
            }
            else
            {
                ddlst = ddl_status;
            }

            if (Operator == "" || Operator == null || Operator.Contains("ALL OPERATORS") || Operator == null)
            {
                //optname = "ALL";
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
                txt_frm_date = DateTime.Now.AddDays(-30).ToString();
                txt_to_date = DateTime.Now.ToString();
                mobile = txtmob;
            }
            if (lapuno11 == "" || lapuno11 == null || lapuno11.Contains("Select Port"))
            {

                portname = "ALL";
            }
            else
            {
                portname = lapuno11.ToUpper();
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
            int pagesize = 35;
            var rowdata = db.proc_Recharge_operator_report_newPaging(1, pagesize, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), ddlusers, userid, ddlst, optname, mobile, portname).ToList();
            return View(rowdata);
        }

        [HttpPost]
        public ActionResult FetchRechargeData(string btntype, string txt_frm_date, string txtmob, string txt_to_date, string ddl_status, string txtdemo, string ddl_top, string lapuno11, string Operator, string kk)
        {
            string draw = Request.Form.GetValues("draw")[0];
            int start = Convert.ToInt32(Request.Form.GetValues("start")[0]);
            //if(pageNumer == 0)
            //{
            //    pageNumer = 1;
            //}
            int pageSize = Convert.ToInt32(Request.Form.GetValues("length")[0]);
            int pageNumer = (start / pageSize) + 1;
            var optname = "";
            var portname = "";
            var mobile = "";
            ViewBag.fromdate = txt_frm_date;
            ViewBag.todate = txt_to_date;
            DateTime frm = Convert.ToDateTime(txt_frm_date);
            DateTime to = Convert.ToDateTime(txt_to_date);

            if (ddl_top == "All")
            {
                ddl_top = "1000000";
            }

            int ddltop = Convert.ToInt32(ddl_top);

            if (Operator == "" || Operator.Contains("ALL OPERATORS") || Operator == null)
            {
                //optname = "ALL";
                optname = null;
            }
            else
            {
                optname = Operator;
            }
            if (txtmob == "" || txtmob == null)
            {
                //mobile = "ALL";
                mobile = null;
            }
            else
            {
                mobile = txtmob;
            }
            if (lapuno11 == "" || lapuno11.Contains("Select Port") || lapuno11 == null)
            {
                //portname = "ALL";
                portname = null;
            }
            else
            {
                portname = lapuno11.ToUpper();
            }
            txt_frm_date = frm.ToString("dd-MM-yyyy");
            txt_to_date = to.ToString("dd-MM-yyyy");
            string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy",
                            "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
            DateTime dt = !string.IsNullOrWhiteSpace(txt_frm_date) ? DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
            DateTime dt1 = !string.IsNullOrWhiteSpace(txt_to_date) ? DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
            DateTime frm_date = Convert.ToDateTime(dt).Date;
            DateTime to_date = Convert.ToDateTime(dt1).Date.AddDays(1);

            var operator_value = db.port_list();
            ViewBag.Lapuno = new SelectList(operator_value, "PortNo", "PortNo");
            //all operator list
            var operator_value1 = db.Opt_codes_new().Distinct().ToList();
            ViewBag.Operatornew = new SelectList(operator_value1, "new_opt_code", "operator_Name", null);
            //end
            var query1 = db.SRS_code_all(lapuno11).ToList();
            SelectList objmodeldata = new SelectList(query1, "mars", "mars", "-select-");
            EmployeeViewModel viewModel = new EmployeeViewModel();
            viewModel.CountryListModel = objmodeldata;
            if (ddl_status == "ALL STATUS")
            {
                ddl_status = null;
            }
            if (btntype == "TODAY")
            {
                //ViewBag.type = "live";
                //var ch = db.Recharge_operator_report(Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), ddl_status, optname, mobile, ddltop, portname).ToList();
                var ch = db.proc_Recharge_operator_report_withPaging(Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), ddl_status, optname, mobile, portname, pageNumer, pageSize).ToList();
                //ViewData["totals"] = ch.Where(s => s.Rstaus.ToUpper().Contains("SUCCESS")).Sum(s => Convert.ToInt32(s.amount));
                //ViewData["totalf"] = ch.Where(s => s.Rstaus.ToUpper().Contains("FAILED")).Sum(s => Convert.ToInt32(s.amount));
                //ViewData["totalp"] = ch.Where(s => s.Rstaus.ToUpper().Contains("REQ")).Sum(s => Convert.ToInt32(s.amount));
                //viewModel.Recharge_operator_live = ch;
                //viewModel.srspending = db.SRS_pending_count_ALL();
                //viewModel.srspending1 = db.SRS_pending_count_ALL();
                return this.Json(new
                {
                    draw = Convert.ToInt32(draw),
                    recordsTotal = ch.FirstOrDefault()?.TotalRows,
                    recordsFiltered = ch.FirstOrDefault()?.TotalRows,
                    data = ch
                }, JsonRequestBehavior.AllowGet);
            }
            else if (btntype == "OLD")
            {
                //ViewBag.type = "old";
                var ch1 = db.proc_Recharge_operator_report_old_withPaging(Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), ddl_status, optname, mobile, portname, pageNumer, pageSize).ToList();
                // var ch1 = db.proc_Recharge_operator_report_old_withPaging(Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), ddl_status, optname, mobile, ddltop, portname,pageNumer,pageSize).ToList();
                //ViewData["totals"] = ch1.Where(s => s.Rstaus.ToUpper().Contains("SUCCESS")).Sum(s => Convert.ToInt32(s.amount));
                //ViewData["totalf"] = ch1.Where(s => s.Rstaus.ToUpper().Contains("FAILED")).Sum(s => Convert.ToInt32(s.amount));
                //ViewData["totalp"] = ch1.Where(s => s.Rstaus.ToUpper().Contains("REQ")).Sum(s => Convert.ToInt32(s.amount));
                //viewModel.Recharge_operator_old = ch1;
                //viewModel.srspending = db.SRS_pending_count_ALL();
                //viewModel.srspending1 = db.SRS_pending_count_ALL();
                return this.Json(new
                {
                    draw = Convert.ToInt32(draw),
                    recordsTotal = ch1.FirstOrDefault()?.TotalRows,
                    recordsFiltered = ch1.FirstOrDefault()?.TotalRows,
                    data = ch1
                }, JsonRequestBehavior.AllowGet);
            }
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult InfiniteScroll(int pageindex, string txt_frm_date, string txt_to_date, string txtmob, string ddl_status, string lapuno11, string Operator, string ddlusers, string allmaster1, string alldealer, string allretailer, string Whitelabel, string API, string txtdemo)
        {
            var optname = "";
            var portname = "";
            var mobile = "";
            var ddlst = "";
            string userid = User.Identity.GetUserId();

            if (ddlusers == "Master")
            {
                if (allmaster1 == "" || allmaster1.Contains("Master") || allmaster1 == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = allmaster1;
                }
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
            if (ddlusers == "WAdmin")
            {
                if (Whitelabel == "" || Whitelabel.Contains("Select Whitelabel") || Whitelabel == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = Whitelabel;
                }
            }
            if (ddlusers == "API")
            {
                if (API == "" || API.Contains("Select API") || API == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = API;
                }
            }
            if (ddlusers == "" || ddlusers == null || ddlusers == "Admin")
            {
                userid = "ALL";
                ddlusers = "Admin";
            }



            if (ddl_status == "" || ddl_status.Contains("ALL STATUS") || Operator == null)
            {
                ddlst = "ALL";
            }
            else
            {
                ddlst = ddl_status;
            }

            if (Operator == "" || Operator.Contains("ALL OPERATORS") || Operator == null)
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
            if (lapuno11 == "" || lapuno11.Contains("Select Port") || lapuno11 == null)
            {
                portname = "ALL";
            }
            else
            {
                portname = lapuno11.ToUpper();
            }
            var operator_value = db.port_list();
            ViewBag.Lapuno = new SelectList(operator_value, "PortNo", "PortNo");

            if ((txt_frm_date == null && txt_to_date == null) || (txt_frm_date == "" && txt_to_date == ""))
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

            Vastwebmulti.Areas.Employee.ViewModel.EmployeeViewModel viewModel = new Vastwebmulti.Areas.Employee.ViewModel.EmployeeViewModel();

            int pagesize = 35;
            var tbrowdt = db.proc_Recharge_operator_report_newPaging(pageindex, pagesize, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), ddlusers, userid, ddlst, optname, mobile, portname).ToList();

            JsonModel jsonmodel = new JsonModel();
            jsonmodel.NoMoredata = tbrowdt.Count < pagesize;
            jsonmodel.HTMLString = renderPartialViewtostring("_showRechargeReport", tbrowdt);
            return Json(jsonmodel);
        }

        [HttpPost]
        public ActionResult Recharge_report_View(int Idno)
        {
            var details = db.Recharge_report_View_Details(Idno);
            return Json(details, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [PermissioncheckingAttribute(servicename = "PREPAIDUTILITY", permision = "Write")]
        public ActionResult Editoperator(string txtrchno, string txtidno, string txtamount, string txtprovider, string txtoptid, string txtlapubal)
        {
            try
            {
                if (txtlapubal == "")
                {
                    txtlapubal = "0";
                }
                db.update_optidnew(txtidno, txtoptid, Convert.ToDecimal(txtlapubal));
                return RedirectToAction("Operator_report_new");
            }
            catch
            {
                return RedirectToAction("Operator_report_new");
            }
        }

        public ActionResult FindTotal(string txt_frm_date, string txtmob, string txt_to_date, string ddl_status, string lapuno11, string Operator, string ddlusers, string allmaster1, string alldealer, string allretailer, string API, string Whitelabel)
        {
            var optname = "";
            var portname = "";
            var mobile = "";
            var ddlst = "";
            var ddluserid = "";
            if (ddlusers == "Master")
            {
                ddluserid = allmaster1;
            }
            else if (ddlusers == "Dealer")
            {
                ddluserid = alldealer;
            }
            else if (ddlusers == "Retailer")
            {
                ddluserid = allretailer;
            }
            else if (ddlusers == "API")
            {
                ddluserid = API;
            }
            else if (ddlusers == "WAdmin")
            {
                ddluserid = Whitelabel;
            }
            string userid = User.Identity.GetUserId();
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
            if (lapuno11 == null || lapuno11 == "" || lapuno11.Contains("Select Port"))
            {
                portname = "ALL";
                //portname = null;
            }
            else
            {
                portname = lapuno11.ToUpper();
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
            int pagesize = 20;



            //if (ddl_status.ToUpper() == "PENDING")
            //{
            //    ddl_status = "REQ";
            //}
            // var chk = db.Recharge_info.Where(aa => aa.Rch_time >= frm_date && aa.Rch_time <= to_date && aa.Rstaus.Contains(ddl_status) && (Operator == "" ? aa.optcode.Contains(Operator) : aa.optcode == Operator) && aa.Mobile.Contains(txtmob)).ToList();
            var chk = db.Total_Recharge(frm_date, to_date, ddlst, portname, optname, ddlusers, ddluserid).ToList();
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

        [PermissioncheckingAttribute(servicename = "PREPAIDUTILITY", permision = "Write")]
        public ActionResult Successall(string idnval, string optval)
        {
            string[] idnnstring = idnval.Split(',');
            foreach (string idnn in idnnstring)
            {
                int idno = Convert.ToInt32(idnn);
                var checkforlive = db.Recharge_info.Where(aa => aa.idno == idno).SingleOrDefault();
                if (checkforlive != null)
                {
                    if (checkforlive.Rstaus.ToUpper().Contains("REQ"))
                    {
                        db.recharge_update(idno.ToString(), "Success", optval, 0, "", "");
                        ApiUserResponse(idno, checkforlive.Rch_from, checkforlive.refid, "Success", optval);
                    }
                    else if (checkforlive.Rstaus.ToUpper() == "FAILED" || checkforlive.Rstaus.ToUpper().Contains("SUCCESS TO FAILED"))
                    {
                        db.recharge_update_failed_to_success(idno, optval);
                        ApiUserResponse(idno, checkforlive.Rch_from, checkforlive.refid, "SUCCESS", optval);
                    }
                }
                else
                {
                    var checkforold = db.Recharge_info_old.Where(aa => aa.idno == idno).SingleOrDefault();
                    if (checkforold != null)
                    {
                        if (checkforold.Rstaus.ToUpper().Contains("REQ"))
                        {
                            db.recharge_update_old(idno.ToString(), "Success", optval, 0, "", "");
                            ApiUserResponse(idno, checkforold.Rch_from, checkforold.refid, "Success", optval);
                        }
                        else if (checkforold.Rstaus.ToUpper() == "FAILED" || checkforold.Rstaus.ToUpper().Contains("SUCCESS TO FAILED"))
                        {
                            db.recharge_update_failed_to_success_old(idno, optval);
                            ApiUserResponse(idno, checkforold.Rch_from, checkforold.refid, "SUCCESS", optval);
                        }
                    }
                }
            }

            var responsemsg = "{'Message':'Recharge Successfully.','Response':'SUCCESS'}";
            var jss = new JavaScriptSerializer();
            var dict = jss.Deserialize<dynamic>(responsemsg);
            return Json(dict, JsonRequestBehavior.AllowGet);
        }
        [PermissioncheckingAttribute(servicename = "PREPAIDUTILITY", permision = "Write")]
        public ActionResult Failedall(string idnval, string optval)
        {
            string[] idnnstring = idnval.Split(',');
            foreach (string idnn in idnnstring)
            {
                int idno = Convert.ToInt32(idnn);
                var checkforlive = db.Recharge_info.Where(aa => aa.idno == idno).SingleOrDefault();
                if (checkforlive != null)
                {
                    if (checkforlive.Rstaus.ToUpper().Contains("REQ"))
                    {
                        db.recharge_update(idno.ToString(), "Failed", optval, 0, "", "");
                        ApiUserResponse(idno, checkforlive.Rch_from, checkforlive.refid, "Failed", optval);
                    }
                    else if (checkforlive.Rstaus.ToUpper() == "SUCCESS" || checkforlive.Rstaus.ToUpper().Contains("FAILED TO SUCCESS"))
                    {
                        db.recharge_update_success_to_failed(idno);
                        ApiUserResponse(idno, checkforlive.Rch_from, checkforlive.refid, "FAILED", optval);
                    }
                }
                else
                {
                    var checkforold = db.Recharge_info_old.Where(aa => aa.idno == idno).SingleOrDefault();
                    if (checkforold != null)
                    {
                        if (checkforold.Rstaus.ToUpper().Contains("REQ"))
                        {
                            db.recharge_update_old(idno.ToString(), "Failed", optval, 0, "", "");
                            ApiUserResponse(idno, checkforold.Rch_from, checkforold.refid, "Failed", optval);
                        }
                        else if (checkforold.Rstaus.ToUpper() == "SUCCESS" || checkforold.Rstaus.ToUpper().Contains("FAILED TO SUCCESS"))
                        {
                            db.recharge_update_success_to_failed_old(idno);
                            ApiUserResponse(idno, checkforold.Rch_from, checkforold.refid, "FAILED", optval);
                        }
                    }
                }
            }

            var responsemsg = "{'Message':'Recharge Successfully.','Response':'SUCCESS'}";
            var jss = new JavaScriptSerializer();
            var dict = jss.Deserialize<dynamic>(responsemsg);
            return Json(dict, JsonRequestBehavior.AllowGet);
        }
        [PermissioncheckingAttribute(servicename = "PREPAIDUTILITY", permision = "Write")]
        public ActionResult resend(string idnval)
        {
            string[] idarrary = idnval.Split(',');
            var responsemsg = "";
            foreach (string s in idarrary)
            {
                System.Data.Entity.Core.Objects.ObjectParameter output = new
                            System.Data.Entity.Core.Objects.ObjectParameter("Output", typeof(string));

                var ch = db.SRS_Rch_Resend(s, output).SingleOrDefault().msg.ToString();

                if (ch.ToUpper() == "OKK")
                {
                    responsemsg = "  {'Message':'Recharge Process Successfully.','Response':'SUCCESS'}";

                }
                else if (ch.ToUpper() == "AOK")
                {
                    responsemsg = "  {'Message':'Recharge Process Successfully.','Response':'SUCCESS'}";
                }
                else if (ch.ToUpper() == "MOK")
                {
                    responsemsg = "  {'Message':'Recharge Process Successfully.','Response':'SUCCESS'}";
                }
                else
                {
                    decimal? Amount; var mobileno = ""; var OptCode = ""; var optional1 = ""; var optional2 = ""; var idno = "";
                    var url = ""; decimal ammt; string webcontent = ""; int idnn11; string OrderId = "";
                    int id = Convert.ToInt32(s);
                    var data = db.Recharge_info.Where(pp => pp.idno == id).SingleOrDefault();
                    if (data != null)
                    {
                        Amount = data.amount;
                        mobileno = data.Mobile;
                        OptCode = data.optcode;
                        idnn11 = data.idno;
                        optional1 = data.optional1;
                        optional2 = data.optional2;
                        url = ch.Replace("AOK", "");
                        ammt = Convert.ToDecimal(Amount);
                        OrderId = data.Order_id;
                    }
                    else
                    {
                        var data1 = db.Recharge_info_old.Where(pp => pp.idno == id).SingleOrDefault();
                        Amount = data1.amount;
                        mobileno = data1.Mobile;
                        OptCode = data1.optcode;
                        optional1 = data1.optional1;
                        optional2 = data1.optional2;
                        idnn11 = data1.idno;
                        url = ch.Replace("AOK", "");
                        ammt = Convert.ToDecimal(Amount);
                        OrderId = data1.Order_id;
                    }
                    var infochkk = db.Recharge_info.Where(aa => aa.Mobile == mobileno && aa.amount == ammt && (aa.Rstaus == "Request Send" || aa.Rstaus == "Request Sent")).SingleOrDefault();
                    // idno = (from rch in db.Recharge_info where rch.Mobile == mobileno where rch.amount == ammt where rch.Rstaus == "Request Send" || rch.Rstaus == "Request Sent" select rch.idno).SingleOrDefault().ToString();
                    idno = infochkk.idno.ToString();
                    idnn11 = Convert.ToInt32(idno);
                    OrderId = infochkk.Order_id;
                    if (url.ToUpper().Contains("API.VASTBAZAAR.COM"))
                    {

                        string CommonTranid = "E" + DateTime.Parse(DateTime.Now.ToString()).ToString("yyMMddHHmmss") + RandomString(4);
                        var apioptcode = db.SRS_API.Where(aa => aa.api.ToUpper().Contains("API.VASTBAZAAR.COM") && aa.opt_code == OptCode).SingleOrDefault().apioptcode;

                        Vastbillpay vb = new Vastbillpay();
                        var tokenapi = Responsetoken.gettoken();
                        var responsechk1 = vb.billpay(tokenapi, mobileno, apioptcode, Amount.ToString(), optional1, optional2, CommonTranid);

                        var Request = responsechk1.Request.Parameters[2].Value;
                        var ReqUrl = url + "RequestBody : " + Request;

                        var responsechk = responsechk1.Content.ToString();
                        dynamic json = JsonConvert.DeserializeObject(responsechk);
                        var respcode = json.Content.ResponseCode.ToString();
                        var ADDINFO = json.Content.ADDINFO.ToString();
                        dynamic json1 = JsonConvert.DeserializeObject(ADDINFO);

                        Recharge_info objCourse = (from p in db.Recharge_info where p.idno == idnn11 select p).Single();
                        objCourse.Recharge_response = json1.ToString();
                        objCourse.Recharge_request = ReqUrl;
                        objCourse.Order_id = CommonTranid.ToString();
                        db.SaveChanges();

                        var status = json1.STATUS.ToString();
                        var PRICE = json1.PRICE.ToString();
                        var errormsg = json1.ERRORMSG.ToString();
                        var TRANSID = json1.TRANSID.ToString();
                        if (status == "Success")
                        {
                            status = "Success";
                            db.recharge_update(idnn11.ToString(), status, TRANSID, 0, json.ToString(), "Response");
                            responsemsg = " {'Message':'Recharge Success.','Response':'SUCCESS'}";
                        }
                        else if (status == "Failed")
                        {
                            status = "Failed";
                            db.recharge_update(idnn11.ToString(), status, errormsg, 0, json.ToString(), "Response");
                            responsemsg = " {'Message':'" + errormsg + "','Response':'EERROR','Price':'" + PRICE + "'}";
                        }
                        else
                        {
                            responsemsg = "{'Message':'Recharge Pending.','Response':'SUCCESS'}";
                        }
                    }
                    else if (url.ToUpper().Contains("RECHARGE/RECHARGE"))
                    {

                        var tkn = db.Recharge_API_URLS.Where(aa => aa.url == url).SingleOrDefault();
                        var token = tkn.Token;
                        var Tokenid = tkn.Token;
                        var Userid = tkn.API_ID;

                        POST_API PA = new POST_API();

                        int idnn111 = 0;
                        idno = (from rch in db.Recharge_info where rch.Mobile == mobileno where rch.amount == ammt where rch.Rstaus == "Request Send" || rch.Rstaus == "Request Sent" select rch.idno).SingleOrDefault().ToString();
                        idnn111 = Convert.ToInt32(idno);
                        string CommonTranid = "E" + DateTime.Parse(DateTime.Now.ToString()).ToString("yyMMddHHmmss") + RandomString(4);
                        var apioptcode = db.SRS_API.Where(aa => aa.api == url && aa.opt_code == OptCode).SingleOrDefault().apioptcode;
                        var responsechk1 = PA.RchReq(token, mobileno, Tokenid, Userid, Convert.ToDecimal(Amount), apioptcode, CommonTranid, optional1, optional2, url);

                        var responsechk = responsechk1.Content.ToString();
                        dynamic json = JsonConvert.DeserializeObject(responsechk);

                        var Request = responsechk1.Request.Parameters[2].Value;
                        var ReqUrl = url + "  RequestBody : " + Request;

                        Recharge_info objCourse = (from p in db.Recharge_info where p.idno == idnn111 select p).Single();
                        objCourse.Recharge_response = json.ToString();
                        objCourse.Recharge_request = ReqUrl;
                        objCourse.Order_id = CommonTranid.ToString();
                        db.SaveChanges();

                        var status = json.Status.ToString();
                        var Transid = json.Transid.ToString();
                        var errormsg = json.Errormsg.ToString();
                        var remainamount = json.Remain.ToString();
                        var Yourrchid = json.Yourrchid.ToString();
                        var RechargeID = json.RechargeID.ToString();

                        if (status.ToUpper() == "SUCCESS")
                        {
                            status = "Success";
                            db.recharge_update(idnn111.ToString(), status, Transid, Convert.ToDecimal(remainamount), json.ToString(), "Response");
                            responsemsg = " {'Message':'Recharge Success.','Response':'SUCCESS'}";
                        }
                        else if (status.ToUpper() == "FAILED")
                        {
                            status = "Failed";
                            db.recharge_update(idnn111.ToString(), status, errormsg, 0, json.ToString(), "Response");
                            responsemsg = " {'Message':'" + errormsg + "','Response':'EERROR','Price':'" + remainamount + "'}";
                        }
                        else
                        {
                            responsemsg = "{'Message':'Recharge Pending.','Response':'SUCCESS'}";
                        }
                    }
                    else if (url.ToUpper().Contains("MROBOTICS.IN"))
                    {

                        var tkn = db.Recharge_API_URLS.Where(aa => aa.url == url).SingleOrDefault();
                        var token = tkn.Token;
                        var Tokenid = tkn.Token;
                        var Userid = tkn.API_ID;

                        POST_API PA = new POST_API();

                        int idnn111 = 0;
                        idno = (from rch in db.Recharge_info where rch.Mobile == mobileno where rch.amount == ammt where rch.Rstaus == "Request Send" || rch.Rstaus == "Request Sent" select rch.idno).SingleOrDefault().ToString();
                        idnn111 = Convert.ToInt32(idno);
                        string CommonTranid = "E" + DateTime.Parse(DateTime.Now.ToString()).ToString("yyMMddHHmmss") + RandomString(4);
                        var apioptcode = db.SRS_API.Where(aa => aa.api == url && aa.opt_code == OptCode && aa.status == "Y").SingleOrDefault().apioptcode;
                        var responsechk1 = PA.RchReqMrobotics(token, mobileno, Tokenid, Userid, Convert.ToDecimal(Amount), apioptcode, CommonTranid, optional1, optional2, OptCode, url);

                        var responsechk = responsechk1.Content.ToString();
                        dynamic json = JsonConvert.DeserializeObject(responsechk);

                        var Request = responsechk1.Request.Parameters[1].Value;
                        var ReqUrl = url + "  RequestBody : " + Request;

                        Recharge_info objCourse = (from p in db.Recharge_info where p.idno == idnn111 select p).Single();

                        objCourse.Recharge_request = ReqUrl;
                        objCourse.Order_id = CommonTranid.ToString();
                        db.SaveChanges();

                        string company_id = json.company_id.ToString();
                        string remainamount = ""; string status = ""; string Transid = "";

                        if (company_id == "4")
                        {

                            status = json.status.ToString();
                            if (status.ToUpper() == "SUCCESS")
                            {
                                Transid = json.tnx_id.ToString();
                            }
                            else
                            {
                                Transid = "";
                            }


                        }
                        else
                        {
                            Recharge_info objCours = (from p in db.Recharge_info where p.idno == idnn11 select p).Single();
                            objCours.Recharge_response = json.ToString();
                            db.SaveChanges();
                            webcontent = json.ToString();


                            status = json.status.ToString();
                            if (status.ToUpper() == "SUCCESS")
                            {
                                Transid = json.tnx_id.ToString();
                            }
                            else
                            {
                                Transid = "";
                            }

                        }

                        if (status.ToUpper() == "SUCCESS")
                        {
                            remainamount = json.balance.ToString();
                            status = "Success";
                            db.recharge_update(idnn111.ToString(), status, Transid, Convert.ToDecimal(remainamount), webcontent, "Response");
                            ch = "Recharge  SUCCESS.";
                            TempData["Output"] = "Success";
                            TempData.Keep("Output");
                            TempData["OutputMessage"] = ch;
                            TempData.Keep("OutputMessage");
                            responsemsg = " {'Message':'Recharge Success.','Response':'SUCCESS'}";
                        }
                        else if (status.ToUpper() == "FAILURE")
                        {
                            status = "Failed";
                            db.recharge_update(idnn111.ToString(), status, Transid, 0, webcontent, "Response");
                            ch = "Recharge Failed.";
                            TempData["Output"] = "error";
                            TempData.Keep("Output");
                            TempData["OutputMessage"] = ch;
                            TempData.Keep("OutputMessage");
                            responsemsg = "{'Message':'Recharge Failed.','Response':'ERROR'}";
                        }
                        else
                        {
                            Recharge_info obj = (from p in db.Recharge_info where p.idno == idnn111 select p).Single();
                            obj.Recharge_response = webcontent.ToString();
                            db.SaveChanges();
                            ch = "Recharge Proceed Successfully.";
                            TempData["Output"] = "Success";
                            TempData.Keep("Output");
                            TempData["OutputMessage"] = ch;
                            TempData.Keep("OutputMessage");
                            responsemsg = " {'Message':'Recharge Processed.','Response':'SUCCESS'}";
                        }
                    }
                    else if (url.ToUpper() == "OKK" || url.ToUpper() == "AOK" || url.ToUpper() == "MOK")
                    {
                        responsemsg = "  {'Message':'Recharge Process Successfully.','Response':'SUCCESS'}";
                    }
                    else
                    {
                        try
                        {
                            var apiinfo = db.RechargeapiInfoes.Where(aa => aa.apiendpoint.ToUpper() == url.ToUpper()).SingleOrDefault();
                            if (apiinfo != null)
                            {
                                var apiremain = "0"; var _status = "Pending"; var transid = ""; var errormsg = "";

                                ApiResponse rech = RechargeServices.Recharge(apiinfo, mobileno, OptCode, ammt, OrderId);
                                _status = rech.status;
                                transid = rech.operatorId;
                                errormsg = rech.errormsg;
                                apiremain = rech.apiremain;
                                webcontent = rech.api_response;
                                idnn11 = rech.id;

                                if (_status == "Success")
                                {
                                    db.recharge_update(idnn11.ToString(), "Success", transid, Convert.ToDecimal(apiremain), webcontent, "Response");
                                    responsemsg = "{'Message':'Recharge Process Successfully.','Response':'SUCCESS'}";
                                }
                                else if (_status == "Failed")
                                {
                                    var optcodei = db.Operator_Code.Where(aa => aa.new_opt_code == OptCode).SingleOrDefault().Operator_id.ToString();
                                    db.recharge_update(idnn11.ToString(), "Failed", errormsg, Convert.ToDecimal(apiremain), webcontent, "Response");
                                    responsemsg = "{'Message':'Recharge Failed.','Response':'ERROR'}";
                                }
                                else
                                {
                                    Recharge_info obj = (from p in db.Recharge_info where p.idno == idnn11 select p).Single();
                                    obj.Recharge_response = webcontent.ToString();
                                    db.SaveChanges();

                                    responsemsg = "{'Message':'Recharge Process Successfully.','Response':'SUCCESS'}";
                                }

                                var jss1 = new JavaScriptSerializer();
                                var dict1 = jss1.Deserialize<dynamic>(responsemsg);
                                return Json(dict1, JsonRequestBehavior.AllowGet);
                            }

                            //  end

                            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                            var client = new RestClient(url);
                            client.Timeout = (System.Int32)TimeSpan.FromSeconds(25).TotalMilliseconds; ;
                            var request = new RestRequest(Method.GET);
                            IRestResponse response = client.Execute(request);
                            webcontent = response.Content;

                            if (url.ToUpper().Contains("LIVE.VASTWEBINDIA.COM"))
                            {

                                dynamic stuff = JsonConvert.DeserializeObject(webcontent);

                                var status = stuff.Status.ToString();
                                var Mobile = stuff.Mobile.ToString();
                                Amount = stuff.Amount.ToString();
                                var RCHID = stuff.RCHID.ToString();
                                var Operatorid = stuff.Operatorid.ToString();
                                var remainamount = stuff.remainamount.ToString();
                                var LapuNumber = stuff.LapuNumber.ToString();

                                if (status.ToUpper() == "SUCCESS")
                                {

                                    status = "Success";
                                    db.recharge_update(idnn11.ToString(), status, Operatorid, Convert.ToDecimal(Amount), webcontent, "Response");
                                    responsemsg = " {'Message':'Recharge Success.','Response':'SUCCESS'}";

                                }
                                else if (status.ToUpper() == "FAILED")
                                {
                                    status = "Failed";
                                    db.recharge_update(idnn11.ToString(), status, Operatorid, Convert.ToDecimal(Amount), webcontent, "Response");
                                    responsemsg = "{'Message':'Recharge Failed.','Response':'ERROR'}";

                                }
                                else
                                {
                                    Recharge_info obj = (from p in db.Recharge_info where p.idno == idnn11 select p).Single();
                                    obj.Recharge_response = webcontent.ToString();
                                    db.SaveChanges();
                                    responsemsg = " {'Message':'Recharge Success.','Response':'SUCCESS'}";
                                }
                            }
                            else if (url.ToUpper().Contains("VASTWEBINDIA.COM"))
                            {

                                dynamic stuff = Newtonsoft.Json.JsonConvert.DeserializeObject(webcontent);

                                var status = stuff.Status.ToString();
                                var mobile = stuff.Mobile.ToString();
                                var amount1 = stuff.Amount.ToString();
                                var R_id = stuff.RID.ToString();
                                var operatorid = stuff.Operatorid.ToString();
                                var remain_amount = stuff.remainamount.ToString();

                                if (status.ToUpper() == "SUCCESS")
                                {

                                    status = "Success";
                                    db.recharge_update(idnn11.ToString(), status, operatorid, Convert.ToDecimal(amount1), webcontent, "Response");
                                    responsemsg = " {'Message':'Recharge Success.','Response':'SUCCESS'}";

                                }
                                else if (status.ToUpper() == "FAILED")
                                {
                                    status = "Failed";
                                    db.recharge_update(idnn11.ToString(), status, operatorid, Convert.ToDecimal(amount1), webcontent, "Response");
                                    responsemsg = "{'Message':'Recharge Failed.','Response':'ERROR'}";

                                }
                                else
                                {
                                    Recharge_info obj = (from p in db.Recharge_info where p.idno == idnn11 select p).Single();
                                    obj.Recharge_response = webcontent.ToString();
                                    db.SaveChanges();
                                    responsemsg = " {'Message':'Recharge Success.','Response':'SUCCESS'}";
                                }
                            }
                            else if (url.ToUpper().Contains("RECHARGETRAAWSERVICES.IN"))
                            {

                                dynamic stuff = Newtonsoft.Json.JsonConvert.DeserializeObject(webcontent);

                                var status = stuff.status.ToString();

                                var operatorid = stuff.operatorid.ToString();
                                var amount1 = stuff.remainamount.ToString();

                                if (amount1 == "" || amount1 == null)
                                {
                                    amount1 = "0";
                                }
                                if (status.ToUpper() == "SUCCESS")
                                {

                                    status = "Success";
                                    db.recharge_update(idnn11.ToString(), status, operatorid, Convert.ToDecimal(amount1), webcontent, "Response");
                                    responsemsg = " {'Message':'Recharge Success.','Response':'SUCCESS'}";

                                }
                                else if (status.ToUpper() == "FAILED")
                                {
                                    status = "Failed";
                                    db.recharge_update(idnn11.ToString(), status, operatorid, Convert.ToDecimal(amount1), webcontent, "Response");
                                    responsemsg = "{'Message':'Recharge Failed.','Response':'ERROR'}";

                                }
                                else
                                {
                                    Recharge_info obj = (from p in db.Recharge_info where p.idno == idnn11 select p).Single();
                                    obj.Recharge_response = webcontent.ToString();
                                    db.SaveChanges();
                                    responsemsg = " {'Message':'Recharge Success.','Response':'SUCCESS'}";
                                }
                            }
                            else
                            {
                                Recharge_info obj = (from p in db.Recharge_info where p.idno == idnn11 select p).Single();
                                obj.Recharge_response = webcontent.ToString();
                                db.SaveChanges();
                                responsemsg = " {'Message':'Recharge Success.','Response':'SUCCESS'}";
                            }
                        }
                        catch (System.Net.WebException ex)
                        {
                        }
                        if (url.ToUpper().Contains("INSTANTPAY.IN"))
                        {
                            dynamic response = JsonConvert.DeserializeObject(webcontent.ToString());
                            var RESCODE = response.res_code.ToString();
                            var optid = response.opr_id.ToString();
                            var openbal = response.opening_bal.ToString();
                            decimal remain = Convert.ToDecimal(openbal);
                            if (RESCODE == "TXN")
                            {
                                db.recharge_update(idno, "SUCCESS", optid, remain, webcontent, "Response");
                                responsemsg = "{'Message':'Recharge Process Successfully.','Response':'SUCCESS'}";
                            }
                            else if (RESCODE.ToString() == "TUP")
                            {
                                responsemsg = "{'Message':'Recharge Process Successfully.','Response':'SUCCESS'}";
                            }
                            else
                            {
                                db.recharge_update(idno, "FAILED", optid, remain, webcontent, "Response");
                                responsemsg = "{'Message':'Recharge Failed.','Response':'ERROR'}";
                            }

                        }
                        else
                        {
                            responsemsg = "{'Message':'Recharge Process Successfully.','Response':'SUCCESS'}";
                        }
                    }

                }
            }

            var jss = new JavaScriptSerializer();
            var dict = jss.Deserialize<dynamic>(responsemsg);
            return Json(dict, JsonRequestBehavior.AllowGet);
            //return Json("Resend SuccessFully!!", JsonRequestBehavior.AllowGet);
        }

        [PermissioncheckingAttribute(servicename = "PREPAIDUTILITY", permision = "Write")]
        public ActionResult dispute(string id, string txtregion)
        {
            try
            {
                System.Data.Entity.Core.Objects.ObjectParameter output = new
                System.Data.Entity.Core.Objects.ObjectParameter("output", typeof(string));

                var ch = db.distute_insert(id, txtregion, output).SingleOrDefault().msg.ToString();
                return Json(ch, JsonRequestBehavior.AllowGet);
                //if (ch == "Success")
                //{
                //    TempData["success"] = "Disputed Successfully.";
                //}
                //else
                //{
                //    TempData["failed"] = "Already Disputed!";
                //}
                //return RedirectToAction("DisputeReport");
            }
            catch (Exception ex)
            {
                return Json("", JsonRequestBehavior.AllowGet);
                //TempData["failed"] = ex;
                //return RedirectToAction("DisputeReport");
            }
        }

        [HttpPost]
        [PermissioncheckingAttribute(servicename = "PREPAIDUTILITY", permision = "Write")]
        public ActionResult sucess(int prtname)
        {
            try
            {
                string idap = Convert.ToString(prtname);
                var RetailerEmailId = "";

                var entry = (from rch in db.Recharge_info
                             where rch.idno == prtname
                             && rch.Rstaus.ToUpper().Contains("REQ")
                             select new
                             {
                                 Idno = rch.idno,
                                 Mobile = rch.Mobile,
                                 Port = rch.portno,
                                 Amount = rch.amount,
                                 Provider = rch.optcode,
                                 Response = rch.Recharge_response,
                                 rch_from = rch.Rch_from,
                                 fid = rch.refid
                             }).SingleOrDefault();

                if (entry != null)
                {


                    var comm = db.Recharge_info.Where(p => p.idno == prtname);
                    var Retailerid = comm.Single().Rch_from;
                    var mobileno = comm.Single().Mobile;
                    var optcode = comm.Single().optcode;
                    var AdminEmail = db.Admin_details.Single().email;
                    var OperatorName = db.Operator_Code.Where(p => p.new_opt_code == optcode).Single().operator_Name;
                    try
                    {
                        RetailerEmailId = db.Retailer_Details.Where(p => p.RetailerId == Retailerid).Single().Email;
                    }
                    catch
                    {

                    }
                    try
                    {
                        RetailerEmailId = db.Whitelabel_Retailer_Details.Where(p => p.RetailerId == Retailerid).Single().Email;
                    }
                    catch
                    { }
                    if (entry.Mobile == "00000")
                    {
                        comm.Single().Rstaus = "Balance";
                        comm.Single().lapu_bal = 0;
                        comm.Single().Resp_time = DateTime.Now;
                        db.SaveChanges();
                    }
                    else
                    {
                        db.recharge_update(idap, "SUCCESS", "Manual SUCCESS", 0, entry.Response, "Response");
                        var statusAdmin = db.PushNotificationStatus.Where(a => a.UserRole == "Admin").SingleOrDefault().Status;
                        var statusRetailer = db.PushNotificationStatus.Where(a => a.UserRole == "Retailer").SingleOrDefault().Status;
                        if (statusAdmin == "Y")
                        {
                            SendPushNotification(AdminEmail, Url.Action("Operator_report_new", "Home"), "Recharge Mobile No " + mobileno + ", Operator " + OperatorName + " is Success.", "Txn. is Manual Success..");
                        }
                        if (statusRetailer == "Y")
                        {
                            SendPushNotification(RetailerEmailId, Url.Action("RechargeReport", "Home"), "Recharge Mobile No " + mobileno + ", Operator " + OperatorName + " is Success.", "Txn. is Manual Success..");
                        }
                        notify.sendmessage(RetailerEmailId, "Recharge Mobile No " + mobileno + ", Operator " + OperatorName + " is  Success.");

                        var role = (from rol in db.Roles join user in db.UserRoles on rol.RoleId equals user.RoleId where user.UserId == entry.rch_from select rol.Name).SingleOrDefault().ToString();
                        if (role == "API")
                        {
                            try
                            {
                                var url = db.Recharge_Update_Url.Where(aa => aa.UserId == entry.rch_from).SingleOrDefault().responseurl.ToString();
                                var curentbal = db.api_remain_amount.Where(aa => aa.apiid == entry.rch_from).SingleOrDefault().balance.ToString();
                                url = url.Replace("rrr", entry.fid);
                                url = url.Replace("sss", "Success");
                                url = url.Replace("ooo", "Manual SUCCESS");
                                url = url.Replace("bbb", curentbal);
                                HttpWebRequest WebRequestObject = (HttpWebRequest)HttpWebRequest.Create(url);
                                WebRequestObject.Timeout = (System.Int32)TimeSpan.FromSeconds(25).TotalMilliseconds;
                                WebResponse Response = WebRequestObject.GetResponse();
                                Stream WebStream = Response.GetResponseStream();
                                StreamReader Reader = new StreamReader(WebStream);
                                var webcontent = Reader.ReadToEnd();

                                Recharge_info obj = (from p in db.Recharge_info where p.idno == entry.Idno select p).Single();
                                obj.API_response = webcontent.ToString();
                                obj.Api_Response_output = url.ToString();
                                db.SaveChanges();
                            }
                            catch
                            {

                            }
                        }
                    }
                    return Json(new { Status = true, Message = "Successfully" });
                }
                else
                {
                    var entry1 = (from rch in db.Recharge_info_old
                                  where rch.idno == prtname
                                  && rch.Rstaus.ToUpper().Contains("REQ")
                                  select new
                                  {
                                      Idno = rch.idno,
                                      Mobile = rch.Mobile,
                                      Port = rch.portno,
                                      Amount = rch.amount,
                                      Provider = rch.optcode,
                                      Response = rch.Recharge_response,
                                      rch_from = rch.Rch_from,
                                      fid = rch.refid,
                                      Rstaus = rch.Rstaus
                                  }).SingleOrDefault();

                    var AdminEmail = db.Admin_details.Single().email;
                    var OperatorName = db.Operator_Code.Where(p => p.new_opt_code == entry1.Provider).Single().operator_Name;
                    try
                    {
                        RetailerEmailId = db.Retailer_Details.Where(p => p.RetailerId == entry1.rch_from).Single().Email;
                    }
                    catch
                    {

                    }
                    try
                    {
                        RetailerEmailId = db.Whitelabel_Retailer_Details.Where(p => p.RetailerId == entry1.rch_from).Single().Email;
                    }
                    catch
                    { }
                    if (entry1.Mobile == "00000")
                    {
                        var comm = db.Recharge_info_old.Where(p => p.idno == prtname).SingleOrDefault();
                        comm.Rstaus = "Balance";
                        comm.lapu_bal = 0;
                        comm.Resp_time = DateTime.Now;
                        db.SaveChanges();
                    }
                    else
                    {
                        db.recharge_update_old(idap, "SUCCESS", "Manual SUCCESS", 0, "", "Response");
                        var statusAdmin = db.PushNotificationStatus.Where(a => a.UserRole == "Admin").SingleOrDefault().Status;
                        var statusRetailer = db.PushNotificationStatus.Where(a => a.UserRole == "Retailer").SingleOrDefault().Status;
                        if (statusAdmin == "Y")
                        {
                            SendPushNotification(AdminEmail, Url.Action("Operator_report_new", "Home"), "Recharge Mobile No " + entry1.Mobile + ", Operator " + OperatorName + " is Success.", "Txn. is Manual Success..");
                        }
                        if (statusRetailer == "Y")
                        {
                            SendPushNotification(RetailerEmailId, Url.Action("RechargeReport", "Home"), "Recharge Mobile No " + entry1.Mobile + ", Operator " + OperatorName + " is Success.", "Txn. is Manual Success..");
                        }
                        notify.sendmessage(RetailerEmailId, "Recharge Mobile No " + entry1.Mobile + ", Operator " + OperatorName + " is  Success.");

                        var role = (from rol in db.Roles join user in db.UserRoles on rol.RoleId equals user.RoleId where user.UserId == entry1.rch_from select rol.Name).SingleOrDefault().ToString();
                        if (role == "API")
                        {
                            try
                            {
                                var url = db.Recharge_Update_Url.Where(aa => aa.UserId == entry1.rch_from).SingleOrDefault().responseurl.ToString();
                                var curentbal = db.api_remain_amount.Where(aa => aa.apiid == entry1.rch_from).SingleOrDefault().balance.ToString();
                                url = url.Replace("rrr", entry1.fid);
                                url = url.Replace("sss", "Success");
                                url = url.Replace("ooo", "Manual SUCCESS");
                                url = url.Replace("bbb", curentbal);
                                HttpWebRequest WebRequestObject = (HttpWebRequest)HttpWebRequest.Create(url);
                                WebRequestObject.Timeout = (System.Int32)TimeSpan.FromSeconds(25).TotalMilliseconds;
                                WebResponse Response = WebRequestObject.GetResponse();
                                Stream WebStream = Response.GetResponseStream();
                                StreamReader Reader = new StreamReader(WebStream);
                                var webcontent = Reader.ReadToEnd();

                                Recharge_info_old obj = (from p in db.Recharge_info_old where p.idno == entry1.Idno select p).Single();
                                obj.API_response = webcontent.ToString();
                                obj.Api_Response_output = url.ToString();
                                db.SaveChanges();
                            }
                            catch
                            { }
                        }
                    }
                    return Json(new { Status = true, Message = "Successfully" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, Message = ex.Message });
            }
        }

        [HttpGet]
        [PermissioncheckingAttribute(servicename = "PREPAIDUTILITY", permision = "Read")]
        public ActionResult show_Operator_report_new(string prtname, string type)
        {
            var portname = ""; var optchk = "";
            if (prtname == "" || prtname == null)
            {
                portname = "ALL";
            }
            else
            {
                portname = prtname;
            }
            try
            {
                string[] strArray = prtname.Split('-');
                portname = strArray[0].ToString();
                optchk = strArray[1].ToString();
            }
            catch
            {
                optchk = "ALL";
            }
            if (prtname.Contains("pen"))
            {
                portname = "NILLCOM";
                optchk = "";
            }

            var query1 = db.SRS_code_all("").ToList();
            SelectList objmodeldata = new SelectList(query1, "mars", "mars", 0);
            var operator_value = db.port_list();

            ViewBag.Lapuno = new SelectList(operator_value, "PortNo", "PortNo", portname);
            var operator_value1 = db.Opt_codes_new();
            ViewBag.Operatornew = new SelectList(operator_value1, "new_opt_code", "operator_Name", optchk).ToList();
            EmployeeViewModel viewModel = new EmployeeViewModel();
            viewModel.CountryListModel = objmodeldata;
            var ch = db.Show_recharge_report_Pending(prtname).ToList();
            viewModel.Show_recharge_report_Pending = ch;
            //return PartialView("_showRechargeReport", viewModel);
            return Json(ch, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [PermissioncheckingAttribute(servicename = "PREPAIDUTILITY", permision = "Read")]
        public ActionResult show_Operator_report_new1(string prtname, string type, string txt_frm_date, string txt_to_date)
        {
            var portname = ""; var optchk = "";
            if (prtname == "")
            {
                portname = "ALL";
            }
            else
            {
                portname = prtname;
            }
            try
            {
                string[] strArray = prtname.Split('-');
                optchk = strArray[1].ToString();
            }
            catch
            {
                optchk = "ALL";
            }
            if (prtname.Contains("pen"))
            {
                portname = "NILLCOM";
                optchk = "";
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

            var query1 = db.SRS_code_all("").ToList();
            SelectList objmodeldata = new SelectList(query1, "mars", "mars", 0);
            var operator_value = db.port_list();

            ViewBag.Lapuno = new SelectList(operator_value, "PortNo", "PortNo", portname);
            var operator_value1 = db.Opt_codes_new();
            ViewBag.Operatornew = new SelectList(operator_value1, "new_opt_code", "operator_Name", optchk).ToList();
            EmployeeViewModel viewModel = new EmployeeViewModel();

            viewModel.CountryListModel = objmodeldata;


            int pagesize = 35;
            var ch = db.proc_Recharge_operator_report_newPaging(1, pagesize, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), "Admin", "ALL", "ALL", "ALL", "ALL", portname).ToList();
            return Json(ch, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [PermissioncheckingAttribute(servicename = "PREPAIDUTILITY", permision = "Write")]
        public ActionResult Failed(int txtrefidno, string ddl_refund, string currentstatus)
        {
            if (currentstatus.Contains("Request"))
            {
                try
                {

                    var entry = (from rch in db.Recharge_info
                                 where rch.idno == txtrefidno
                                 && rch.Rstaus.ToUpper().Contains("REQ")
                                 select new
                                 {
                                     Idno = rch.idno,
                                     Mobile = rch.Mobile,
                                     Port = rch.portno,
                                     Amount = rch.amount,
                                     Provider = rch.optcode,
                                     Response = rch.Recharge_response,
                                     rch_from = rch.Rch_from,
                                     fid = rch.refid

                                 }).SingleOrDefault();

                    if (entry != null)
                    {

                        var RetailerEmailId = "";
                        var AdminEmail = db.Admin_details.Single().email;
                        string idap = Convert.ToString(txtrefidno);
                        var comm = db.Recharge_info.Where(p => p.idno == txtrefidno);
                        var Retailerid = comm.Single().Rch_from;
                        var mobileno = comm.Single().Mobile;
                        var optcode = comm.Single().optcode;
                        var Amount = comm.Single().amount;
                        var OperatorName = db.Operator_Code.Where(p => p.new_opt_code == optcode).Single().operator_Name;
                        try
                        {
                            RetailerEmailId = db.Retailer_Details.Where(p => p.RetailerId == Retailerid).Single().Email;
                        }
                        catch { }
                        try
                        {
                            RetailerEmailId = db.Whitelabel_Retailer_Details.Where(p => p.RetailerId == Retailerid).Single().Email;
                        }
                        catch { }
                        var statusAdmin = db.PushNotificationStatus.Where(a => a.UserRole == "Admin").SingleOrDefault().Status;
                        var statusRetailer = db.PushNotificationStatus.Where(a => a.UserRole == "Retailer").SingleOrDefault().Status;

                        if (currentstatus.ToUpper() == "SUCCESS")
                        {
                            db.recharge_update_success_to_failed(Convert.ToInt32(idap));

                            if (statusAdmin == "Y")
                            {
                                SendPushNotification(AdminEmail, Url.Action("Operator_report_new", "Home"), "Txn of " + mobileno + ". Amt :" + Amount + " - FAILED.(" + string.Format("{0:dd-MMM hh:mm:ss}", DateTime.Now) + ")", "Txn. Success To Failed.");
                            }
                            if (statusRetailer == "Y")
                            {
                                SendPushNotification(RetailerEmailId, "Home/RechargeReport", "Txn of " + mobileno + ". Amt :" + Amount + " - FAILED.(" + string.Format("{0:dd-MMM hh:mm:ss}", DateTime.Now) + ")", "Txn. Success To Failed.");
                            }
                            notify.sendmessage(RetailerEmailId, "Txn of " + mobileno + ". Amt :" + Amount + " - FAILED.(" + string.Format("{0:dd-MMM hh:mm:ss}", DateTime.Now) + ")");
                            //return Json(new { Status = true, Message = "Successfully" });
                        }
                        else if (currentstatus.ToUpper() == "FAILED")
                        {
                            db.recharge_update_failed_to_success(Convert.ToInt32(idap), "optval");
                            if (statusAdmin == "Y")
                            {
                                SendPushNotification(AdminEmail, Url.Action("Operator_report_new", "Home"), "Recharge Mobile No " + mobileno + ", Operator " + OperatorName + " is Success.", "Txn. Failed To Success..");
                            }
                            if (statusRetailer == "Y")
                            {
                                SendPushNotification(RetailerEmailId, Url.Action("RechargeReport", "Home"), "Recharge Mobile No " + mobileno + ", Operator " + OperatorName + " is Success.", "Txn. Failed To Success..");
                            }
                            notify.sendmessage(RetailerEmailId, "Recharge Mobile No " + mobileno + ", Operator " + OperatorName + " is Success By Admin.");
                            //return Json(new { Status = true, Message = "Successfully" });
                        }
                        else
                        {
                            db.recharge_update(idap, "FAILED", ddl_refund, 0, entry.Response, "Response");
                            if (statusAdmin == "Y")
                            {
                                SendPushNotification(AdminEmail, Url.Action("Operator_report_new", "Home"), "Recharge Mobile No " + mobileno + ", Operator " + OperatorName + " is Failed.", "Txn. Manual Failed ..");
                            }
                            if (statusRetailer == "Y")
                            {
                                SendPushNotification(RetailerEmailId, Url.Action("RechargeReport", "Home"), "Recharge Mobile No " + mobileno + ", Operator " + OperatorName + " is Failed.", "Txn. Manual Failed ..");
                            }
                            notify.sendmessage(RetailerEmailId, "Recharge Mobile No " + mobileno + ", Operator " + OperatorName + " is Manual Failed.");
                            //return Json(new { Status = true, Message = "Successfully" });
                        }


                        if (entry.Mobile == "00000")
                        {
                            comm.Single().Rstaus = "Balance";
                            comm.Single().Resp_time = DateTime.Now;
                            comm.Single().lapu_bal = 0;
                            db.SaveChanges();
                        }
                        var role = (from rol in db.Roles join user in db.UserRoles on rol.RoleId equals user.RoleId where user.UserId == entry.rch_from select rol.Name).SingleOrDefault().ToString();
                        if (role == "API")
                        {
                            try
                            {
                                var url = db.Recharge_Update_Url.Where(aa => aa.UserId == entry.rch_from).SingleOrDefault().responseurl.ToString();
                                var curentbal = db.api_remain_amount.Where(aa => aa.apiid == entry.rch_from).SingleOrDefault().balance.ToString();
                                url = url.Replace("rrr", entry.fid);
                                url = url.Replace("sss", "Failed");
                                url = url.Replace("ooo", "Manual Failed");
                                url = url.Replace("bbb", curentbal);
                                HttpWebRequest WebRequestObject = (HttpWebRequest)HttpWebRequest.Create(url);
                                WebRequestObject.Timeout = (System.Int32)TimeSpan.FromSeconds(25).TotalMilliseconds;
                                WebResponse Response = WebRequestObject.GetResponse();
                                Stream WebStream = Response.GetResponseStream();
                                StreamReader Reader = new StreamReader(WebStream);
                                var webcontent = Reader.ReadToEnd();

                                Recharge_info obj = (from p in db.Recharge_info where p.idno == entry.Idno select p).Single();
                                obj.API_response = webcontent.ToString();
                                obj.Api_Response_output = url.ToString();
                                db.SaveChanges();
                            }
                            catch
                            {

                            }
                        }
                        return Json(new { Status = true, Message = "Successfully" });
                    }
                    else
                    {
                        var entry1 = (from rch in db.Recharge_info_old
                                      where rch.idno == txtrefidno
                                      && rch.Rstaus.ToUpper().Contains("REQ")
                                      select new
                                      {
                                          Idno = rch.idno,
                                          Mobile = rch.Mobile,
                                          Port = rch.portno,
                                          Amount = rch.amount,
                                          Provider = rch.optcode,
                                          Response = rch.Recharge_response,
                                          rch_from = rch.Rch_from,
                                          fid = rch.refid

                                      }).SingleOrDefault();

                        var RetailerEmailId = "";
                        var AdminEmail = db.Admin_details.Single().email;
                        string idap = Convert.ToString(txtrefidno);
                        var comm = db.Recharge_info_old.Where(p => p.idno == txtrefidno);
                        var Retailerid = comm.Single().Rch_from;
                        var mobileno = comm.Single().Mobile;
                        var optcode = comm.Single().optcode;
                        var Amount = comm.Single().amount;
                        var OperatorName = db.Operator_Code.Where(p => p.new_opt_code == optcode).Single().operator_Name;
                        try
                        {
                            RetailerEmailId = db.Retailer_Details.Where(p => p.RetailerId == Retailerid).Single().Email;
                        }
                        catch { }
                        try
                        {
                            RetailerEmailId = db.Whitelabel_Retailer_Details.Where(p => p.RetailerId == Retailerid).Single().Email;
                        }
                        catch { }
                        var statusAdmin = db.PushNotificationStatus.Where(a => a.UserRole == "Admin").SingleOrDefault().Status;
                        var statusRetailer = db.PushNotificationStatus.Where(a => a.UserRole == "Retailer").SingleOrDefault().Status;
                        if (currentstatus.ToUpper() == "SUCCESS")
                        {
                            db.recharge_update_success_to_failed_old(Convert.ToInt32(idap));

                            if (statusAdmin == "Y")
                            {
                                SendPushNotification(AdminEmail, Url.Action("Operator_report_new", "Home"), "Txn of " + mobileno + ". Amt :" + Amount + " - FAILED.(" + string.Format("{0:dd-MMM hh:mm:ss}", DateTime.Now) + ")", "Txn. Success To Failed.");
                            }
                            if (statusRetailer == "Y")
                            {
                                SendPushNotification(RetailerEmailId, "Home/RechargeReport", "Txn of " + mobileno + ". Amt :" + Amount + " - FAILED.(" + string.Format("{0:dd-MMM hh:mm:ss}", DateTime.Now) + ")", "Txn. Success To Failed.");
                            }
                            notify.sendmessage(RetailerEmailId, "Txn of " + mobileno + ". Amt :" + Amount + " - FAILED.(" + string.Format("{0:dd-MMM hh:mm:ss}", DateTime.Now) + ")");
                            //return Json(new { Status = true, Message = "Successfully" });
                        }
                        else if (currentstatus.ToUpper() == "FAILED")
                        {
                            db.recharge_update_failed_to_success_old(Convert.ToInt32(idap), "optval");
                            if (statusAdmin == "Y")
                            {
                                SendPushNotification(AdminEmail, Url.Action("Operator_report_new", "Home"), "Recharge Mobile No " + mobileno + ", Operator " + OperatorName + " is Success.", "Txn. Failed To Success..");
                            }
                            if (statusRetailer == "Y")
                            {
                                SendPushNotification(RetailerEmailId, Url.Action("RechargeReport", "Home"), "Recharge Mobile No " + mobileno + ", Operator " + OperatorName + " is Success.", "Txn. Failed To Success..");
                            }
                            notify.sendmessage(RetailerEmailId, "Recharge Mobile No " + mobileno + ", Operator " + OperatorName + " is Success By Admin.");
                            //return Json(new { Status = true, Message = "Successfully" });
                        }
                        else
                        {
                            db.recharge_update_old(idap, "FAILED", ddl_refund, 0, "", "Response");
                            if (statusAdmin == "Y")
                            {
                                SendPushNotification(AdminEmail, Url.Action("Operator_report_new", "Home"), "Recharge Mobile No " + mobileno + ", Operator " + OperatorName + " is Failed.", "Txn. Manual Failed ..");
                            }
                            if (statusRetailer == "Y")
                            {
                                SendPushNotification(RetailerEmailId, Url.Action("RechargeReport", "Home"), "Recharge Mobile No " + mobileno + ", Operator " + OperatorName + " is Failed.", "Txn. Manual Failed ..");
                            }
                            notify.sendmessage(RetailerEmailId, "Recharge Mobile No " + mobileno + ", Operator " + OperatorName + " is Manual Failed.");
                            //return Json(new { Status = true, Message = "Successfully" });
                        }


                        if (entry1.Mobile == "00000")
                        {
                            comm.Single().Rstaus = "Balance";
                            comm.Single().Resp_time = DateTime.Now;
                            comm.Single().lapu_bal = 0;
                            db.SaveChanges();
                        }
                        var role = (from rol in db.Roles join user in db.UserRoles on rol.RoleId equals user.RoleId where user.UserId == entry1.rch_from select rol.Name).SingleOrDefault().ToString();
                        if (role == "API")
                        {
                            try
                            {
                                var url = db.Recharge_Update_Url.Where(aa => aa.UserId == entry1.rch_from).SingleOrDefault().responseurl.ToString();
                                var curentbal = db.api_remain_amount.Where(aa => aa.apiid == entry1.rch_from).SingleOrDefault().balance.ToString();
                                url = url.Replace("rrr", entry1.fid);
                                url = url.Replace("sss", "Failed");
                                url = url.Replace("ooo", "Manual Failed");
                                url = url.Replace("bbb", curentbal);
                                HttpWebRequest WebRequestObject = (HttpWebRequest)HttpWebRequest.Create(url);
                                WebRequestObject.Timeout = (System.Int32)TimeSpan.FromSeconds(25).TotalMilliseconds;
                                WebResponse Response = WebRequestObject.GetResponse();
                                Stream WebStream = Response.GetResponseStream();
                                StreamReader Reader = new StreamReader(WebStream);
                                var webcontent = Reader.ReadToEnd();

                                Recharge_info_old obj = (from p in db.Recharge_info_old where p.idno == entry1.Idno select p).Single();
                                obj.API_response = webcontent.ToString();
                                obj.Api_Response_output = url.ToString();
                                db.SaveChanges();
                            }
                            catch
                            {

                            }
                        }
                        return Json(new { Status = true, Message = "Successfully" });
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { Status = false, Message = ex.Message });
                }
            }
            //success to failed
            else if (currentstatus.Contains("SUCCESS"))
            {
                try
                {

                    var entry = (from rch in db.Recharge_info
                                 where rch.idno == txtrefidno
                                 && rch.Rstaus.ToUpper().Contains("SUCCESS")
                                 select new
                                 {
                                     Idno = rch.idno,
                                     Mobile = rch.Mobile,
                                     Port = rch.portno,
                                     Amount = rch.amount,
                                     Provider = rch.optcode,
                                     Response = rch.Recharge_response,
                                     rch_from = rch.Rch_from,
                                     fid = rch.refid

                                 }).SingleOrDefault();

                    if (entry != null)
                    {

                        var RetailerEmailId = "";
                        var AdminEmail = db.Admin_details.Single().email;
                        string idap = Convert.ToString(txtrefidno);
                        var comm = db.Recharge_info.Where(p => p.idno == txtrefidno);
                        var Retailerid = comm.Single().Rch_from;
                        var mobileno = comm.Single().Mobile;
                        var optcode = comm.Single().optcode;
                        var Amount = comm.Single().amount;
                        var OperatorName = db.Operator_Code.Where(p => p.new_opt_code == optcode).Single().operator_Name;
                        try
                        {
                            RetailerEmailId = db.Retailer_Details.Where(p => p.RetailerId == Retailerid).Single().Email;
                        }
                        catch { }
                        try
                        {
                            RetailerEmailId = db.Whitelabel_Retailer_Details.Where(p => p.RetailerId == Retailerid).Single().Email;
                        }
                        catch { }
                        var statusAdmin = db.PushNotificationStatus.Where(a => a.UserRole == "Admin").SingleOrDefault().Status;
                        var statusRetailer = db.PushNotificationStatus.Where(a => a.UserRole == "Retailer").SingleOrDefault().Status;
                        if (currentstatus.ToUpper() == "SUCCESS")
                        {
                            db.recharge_update_success_to_failed(Convert.ToInt32(idap));

                            if (statusAdmin == "Y")
                            {
                                SendPushNotification(AdminEmail, Url.Action("Operator_report_new", "Home"), "Txn of " + mobileno + ". Amt :" + Amount + " - FAILED.(" + string.Format("{0:dd-MMM hh:mm:ss}", DateTime.Now) + ")", "Txn. Success To Failed.");
                            }
                            if (statusRetailer == "Y")
                            {
                                SendPushNotification(RetailerEmailId, "Home/RechargeReport", "Txn of " + mobileno + ". Amt :" + Amount + " - FAILED.(" + string.Format("{0:dd-MMM hh:mm:ss}", DateTime.Now) + ")", "Txn. Success To Failed.");
                            }
                            notify.sendmessage(RetailerEmailId, "Txn of " + mobileno + ". Amt :" + Amount + " - FAILED.(" + string.Format("{0:dd-MMM hh:mm:ss}", DateTime.Now) + ")");
                            //return Json(new { Status = true, Message = "Successfully" });
                        }
                        else if (currentstatus.ToUpper() == "FAILED")
                        {
                            db.recharge_update_failed_to_success(Convert.ToInt32(idap), "optval");
                            if (statusAdmin == "Y")
                            {
                                SendPushNotification(AdminEmail, Url.Action("Operator_report_new", "Home"), "Recharge Mobile No " + mobileno + ", Operator " + OperatorName + " is Success.", "Txn. Failed To Success..");
                            }
                            if (statusRetailer == "Y")
                            {
                                SendPushNotification(RetailerEmailId, Url.Action("RechargeReport", "Home"), "Recharge Mobile No " + mobileno + ", Operator " + OperatorName + " is Success.", "Txn. Failed To Success..");
                            }
                            notify.sendmessage(RetailerEmailId, "Recharge Mobile No " + mobileno + ", Operator " + OperatorName + " is Success By Admin.");
                            //return Json(new { Status = true, Message = "Successfully" });
                        }
                        else
                        {
                            db.recharge_update(idap, "FAILED", ddl_refund, 0, entry.Response, "Response");
                            if (statusAdmin == "Y")
                            {
                                SendPushNotification(AdminEmail, Url.Action("Operator_report_new", "Home"), "Recharge Mobile No " + mobileno + ", Operator " + OperatorName + " is Failed.", "Txn. Manual Failed ..");
                            }
                            if (statusRetailer == "Y")
                            {
                                SendPushNotification(RetailerEmailId, Url.Action("RechargeReport", "Home"), "Recharge Mobile No " + mobileno + ", Operator " + OperatorName + " is Failed.", "Txn. Manual Failed ..");
                            }
                            notify.sendmessage(RetailerEmailId, "Recharge Mobile No " + mobileno + ", Operator " + OperatorName + " is Manual Failed.");
                            //return Json(new { Status = true, Message = "Successfully" });
                        }


                        if (entry.Mobile == "00000")
                        {
                            comm.Single().Rstaus = "Balance";
                            comm.Single().Resp_time = DateTime.Now;
                            comm.Single().lapu_bal = 0;
                            db.SaveChanges();
                        }
                        var role = (from rol in db.Roles join user in db.UserRoles on rol.RoleId equals user.RoleId where user.UserId == entry.rch_from select rol.Name).SingleOrDefault().ToString();
                        if (role == "API")
                        {
                            try
                            {
                                var url = db.Recharge_Update_Url.Where(aa => aa.UserId == entry.rch_from).SingleOrDefault().responseurl.ToString();
                                var curentbal = db.api_remain_amount.Where(aa => aa.apiid == entry.rch_from).SingleOrDefault().balance.ToString();
                                url = url.Replace("rrr", entry.fid);
                                url = url.Replace("sss", "Failed");
                                url = url.Replace("ooo", "Manual Failed");
                                url = url.Replace("bbb", curentbal);
                                HttpWebRequest WebRequestObject = (HttpWebRequest)HttpWebRequest.Create(url);
                                WebRequestObject.Timeout = (System.Int32)TimeSpan.FromSeconds(25).TotalMilliseconds;
                                WebResponse Response = WebRequestObject.GetResponse();
                                Stream WebStream = Response.GetResponseStream();
                                StreamReader Reader = new StreamReader(WebStream);
                                var webcontent = Reader.ReadToEnd();

                                Recharge_info obj = (from p in db.Recharge_info where p.idno == entry.Idno select p).Single();
                                obj.API_response = webcontent.ToString();
                                obj.Api_Response_output = url.ToString();
                                db.SaveChanges();
                            }
                            catch
                            {

                            }
                        }
                        return Json(new { Status = true, Message = "Successfully" });
                    }
                    else
                    {
                        var entry1 = (from rch in db.Recharge_info_old
                                      where rch.idno == txtrefidno
                                      && rch.Rstaus.ToUpper().Contains("SUCCESS")
                                      select new
                                      {
                                          Idno = rch.idno,
                                          Mobile = rch.Mobile,
                                          Port = rch.portno,
                                          Amount = rch.amount,
                                          Provider = rch.optcode,
                                          Response = rch.Recharge_response,
                                          rch_from = rch.Rch_from,
                                          fid = rch.refid

                                      }).SingleOrDefault();

                        var RetailerEmailId = "";
                        var AdminEmail = db.Admin_details.Single().email;
                        string idap = Convert.ToString(txtrefidno);
                        var comm = db.Recharge_info_old.Where(p => p.idno == txtrefidno);
                        var Retailerid = comm.Single().Rch_from;
                        var mobileno = comm.Single().Mobile;
                        var optcode = comm.Single().optcode;
                        var Amount = comm.Single().amount;
                        var OperatorName = db.Operator_Code.Where(p => p.new_opt_code == optcode).Single().operator_Name;
                        try
                        {
                            RetailerEmailId = db.Retailer_Details.Where(p => p.RetailerId == Retailerid).Single().Email;
                        }
                        catch { }
                        try
                        {
                            RetailerEmailId = db.Whitelabel_Retailer_Details.Where(p => p.RetailerId == Retailerid).Single().Email;
                        }
                        catch { }
                        var statusAdmin = db.PushNotificationStatus.Where(a => a.UserRole == "Admin").SingleOrDefault().Status;
                        var statusRetailer = db.PushNotificationStatus.Where(a => a.UserRole == "Retailer").SingleOrDefault().Status;
                        if (currentstatus.ToUpper() == "SUCCESS")
                        {
                            db.recharge_update_success_to_failed_old(Convert.ToInt32(idap));

                            if (statusAdmin == "Y")
                            {
                                SendPushNotification(AdminEmail, Url.Action("Operator_report_new", "Home"), "Txn of " + mobileno + ". Amt :" + Amount + " - FAILED.(" + string.Format("{0:dd-MMM hh:mm:ss}", DateTime.Now) + ")", "Txn. Success To Failed.");
                            }
                            if (statusRetailer == "Y")
                            {
                                SendPushNotification(RetailerEmailId, "Home/RechargeReport", "Txn of " + mobileno + ". Amt :" + Amount + " - FAILED.(" + string.Format("{0:dd-MMM hh:mm:ss}", DateTime.Now) + ")", "Txn. Success To Failed.");
                            }
                            notify.sendmessage(RetailerEmailId, "Txn of " + mobileno + ". Amt :" + Amount + " - FAILED.(" + string.Format("{0:dd-MMM hh:mm:ss}", DateTime.Now) + ")");
                            //return Json(new { Status = true, Message = "Successfully" });
                        }
                        else if (currentstatus.ToUpper() == "FAILED")
                        {
                            db.recharge_update_failed_to_success_old(Convert.ToInt32(idap), "optval");
                            if (statusAdmin == "Y")
                            {
                                SendPushNotification(AdminEmail, Url.Action("Operator_report_new", "Home"), "Recharge Mobile No " + mobileno + ", Operator " + OperatorName + " is Success.", "Txn. Failed To Success..");
                            }
                            if (statusRetailer == "Y")
                            {
                                SendPushNotification(RetailerEmailId, Url.Action("RechargeReport", "Home"), "Recharge Mobile No " + mobileno + ", Operator " + OperatorName + " is Success.", "Txn. Failed To Success..");
                            }
                            notify.sendmessage(RetailerEmailId, "Recharge Mobile No " + mobileno + ", Operator " + OperatorName + " is Success By Admin.");
                            //return Json(new { Status = true, Message = "Successfully" });
                        }
                        else
                        {
                            db.recharge_update_old(idap, "FAILED", ddl_refund, 0, "", "Response");
                            if (statusAdmin == "Y")
                            {
                                SendPushNotification(AdminEmail, Url.Action("Operator_report_new", "Home"), "Recharge Mobile No " + mobileno + ", Operator " + OperatorName + " is Failed.", "Txn. Manual Failed ..");
                            }
                            if (statusRetailer == "Y")
                            {
                                SendPushNotification(RetailerEmailId, Url.Action("RechargeReport", "Home"), "Recharge Mobile No " + mobileno + ", Operator " + OperatorName + " is Failed.", "Txn. Manual Failed ..");
                            }
                            notify.sendmessage(RetailerEmailId, "Recharge Mobile No " + mobileno + ", Operator " + OperatorName + " is Manual Failed.");
                            //return Json(new { Status = true, Message = "Successfully" });
                        }


                        if (entry1.Mobile == "00000")
                        {
                            comm.Single().Rstaus = "Balance";
                            comm.Single().Resp_time = DateTime.Now;
                            comm.Single().lapu_bal = 0;
                            db.SaveChanges();
                        }
                        var role = (from rol in db.Roles join user in db.UserRoles on rol.RoleId equals user.RoleId where user.UserId == entry1.rch_from select rol.Name).SingleOrDefault().ToString();
                        if (role == "API")
                        {
                            try
                            {
                                var url = db.Recharge_Update_Url.Where(aa => aa.UserId == entry1.rch_from).SingleOrDefault().responseurl.ToString();
                                var curentbal = db.api_remain_amount.Where(aa => aa.apiid == entry1.rch_from).SingleOrDefault().balance.ToString();
                                url = url.Replace("rrr", entry1.fid);
                                url = url.Replace("sss", "Failed");
                                url = url.Replace("ooo", "Manual Failed");
                                url = url.Replace("bbb", curentbal);
                                HttpWebRequest WebRequestObject = (HttpWebRequest)HttpWebRequest.Create(url);
                                WebRequestObject.Timeout = (System.Int32)TimeSpan.FromSeconds(25).TotalMilliseconds;
                                WebResponse Response = WebRequestObject.GetResponse();
                                Stream WebStream = Response.GetResponseStream();
                                StreamReader Reader = new StreamReader(WebStream);
                                var webcontent = Reader.ReadToEnd();

                                Recharge_info_old obj = (from p in db.Recharge_info_old where p.idno == entry1.Idno select p).Single();
                                obj.API_response = webcontent.ToString();
                                obj.Api_Response_output = url.ToString();
                                db.SaveChanges();
                            }
                            catch
                            {

                            }
                        }
                        return Json(new { Status = true, Message = "Successfully" });
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { Status = false, Message = ex.Message });
                }
            }
            //end
            //Failed to success

            else
            {
                try
                {

                    var entry = (from rch in db.Recharge_info
                                 where rch.idno == txtrefidno
                                 && rch.Rstaus.ToUpper().Contains("FAILED")
                                 select new
                                 {
                                     Idno = rch.idno,
                                     Mobile = rch.Mobile,
                                     Port = rch.portno,
                                     Amount = rch.amount,
                                     Provider = rch.optcode,
                                     Response = rch.Recharge_response,
                                     rch_from = rch.Rch_from,
                                     fid = rch.refid

                                 }).SingleOrDefault();

                    if (entry != null)
                    {

                        var RetailerEmailId = "";
                        var AdminEmail = db.Admin_details.Single().email;
                        string idap = Convert.ToString(txtrefidno);
                        var comm = db.Recharge_info.Where(p => p.idno == txtrefidno);
                        var Retailerid = comm.Single().Rch_from;
                        var mobileno = comm.Single().Mobile;
                        var optcode = comm.Single().optcode;
                        var Amount = comm.Single().amount;
                        var OperatorName = db.Operator_Code.Where(p => p.new_opt_code == optcode).Single().operator_Name;
                        try
                        {
                            RetailerEmailId = db.Retailer_Details.Where(p => p.RetailerId == Retailerid).Single().Email;
                        }
                        catch { }
                        try
                        {
                            RetailerEmailId = db.Whitelabel_Retailer_Details.Where(p => p.RetailerId == Retailerid).Single().Email;
                        }
                        catch { }
                        var statusAdmin = db.PushNotificationStatus.Where(a => a.UserRole == "Admin").SingleOrDefault().Status;
                        var statusRetailer = db.PushNotificationStatus.Where(a => a.UserRole == "Retailer").SingleOrDefault().Status;
                        if (currentstatus.ToUpper() == "SUCCESS")
                        {
                            db.recharge_update_success_to_failed(Convert.ToInt32(idap));

                            if (statusAdmin == "Y")
                            {
                                SendPushNotification(AdminEmail, Url.Action("Operator_report_new", "Home"), "Txn of " + mobileno + ". Amt :" + Amount + " - FAILED.(" + string.Format("{0:dd-MMM hh:mm:ss}", DateTime.Now) + ")", "Txn. Success To Failed.");
                            }
                            if (statusRetailer == "Y")
                            {
                                SendPushNotification(RetailerEmailId, "Home/RechargeReport", "Txn of " + mobileno + ". Amt :" + Amount + " - FAILED.(" + string.Format("{0:dd-MMM hh:mm:ss}", DateTime.Now) + ")", "Txn. Success To Failed.");
                            }
                            notify.sendmessage(RetailerEmailId, "Txn of " + mobileno + ". Amt :" + Amount + " - FAILED.(" + string.Format("{0:dd-MMM hh:mm:ss}", DateTime.Now) + ")");
                            //return Json(new { Status = true, Message = "Successfully" });
                        }
                        else if (currentstatus.ToUpper() == "FAILED")
                        {
                            db.recharge_update_failed_to_success(Convert.ToInt32(idap), "optval");
                            if (statusAdmin == "Y")
                            {
                                SendPushNotification(AdminEmail, Url.Action("Operator_report_new", "Home"), "Recharge Mobile No " + mobileno + ", Operator " + OperatorName + " is Success.", "Txn. Failed To Success..");
                            }
                            if (statusRetailer == "Y")
                            {
                                SendPushNotification(RetailerEmailId, Url.Action("RechargeReport", "Home"), "Recharge Mobile No " + mobileno + ", Operator " + OperatorName + " is Success.", "Txn. Failed To Success..");
                            }
                            notify.sendmessage(RetailerEmailId, "Recharge Mobile No " + mobileno + ", Operator " + OperatorName + " is Success By Admin.");
                            //return Json(new { Status = true, Message = "Successfully" });
                        }
                        else
                        {
                            db.recharge_update(idap, "FAILED", ddl_refund, 0, entry.Response, "Response");
                            if (statusAdmin == "Y")
                            {
                                SendPushNotification(AdminEmail, Url.Action("Operator_report_new", "Home"), "Recharge Mobile No " + mobileno + ", Operator " + OperatorName + " is Failed.", "Txn. Manual Failed ..");
                            }
                            if (statusRetailer == "Y")
                            {
                                SendPushNotification(RetailerEmailId, Url.Action("RechargeReport", "Home"), "Recharge Mobile No " + mobileno + ", Operator " + OperatorName + " is Failed.", "Txn. Manual Failed ..");
                            }
                            notify.sendmessage(RetailerEmailId, "Recharge Mobile No " + mobileno + ", Operator " + OperatorName + " is Manual Failed.");
                            //return Json(new { Status = true, Message = "Successfully" });
                        }


                        if (entry.Mobile == "00000")
                        {
                            comm.Single().Rstaus = "Balance";
                            comm.Single().Resp_time = DateTime.Now;
                            comm.Single().lapu_bal = 0;
                            db.SaveChanges();
                        }
                        var role = (from rol in db.Roles join user in db.UserRoles on rol.RoleId equals user.RoleId where user.UserId == entry.rch_from select rol.Name).SingleOrDefault().ToString();
                        if (role == "API")
                        {
                            try
                            {
                                var url = db.Recharge_Update_Url.Where(aa => aa.UserId == entry.rch_from).SingleOrDefault().responseurl.ToString();
                                var curentbal = db.api_remain_amount.Where(aa => aa.apiid == entry.rch_from).SingleOrDefault().balance.ToString();
                                url = url.Replace("rrr", entry.fid);
                                url = url.Replace("sss", "Failed");
                                url = url.Replace("ooo", "Manual Failed");
                                url = url.Replace("bbb", curentbal);
                                HttpWebRequest WebRequestObject = (HttpWebRequest)HttpWebRequest.Create(url);
                                WebRequestObject.Timeout = (System.Int32)TimeSpan.FromSeconds(25).TotalMilliseconds;
                                WebResponse Response = WebRequestObject.GetResponse();
                                Stream WebStream = Response.GetResponseStream();
                                StreamReader Reader = new StreamReader(WebStream);
                                var webcontent = Reader.ReadToEnd();

                                Recharge_info obj = (from p in db.Recharge_info where p.idno == entry.Idno select p).Single();
                                obj.API_response = webcontent.ToString();
                                obj.Api_Response_output = url.ToString();
                                db.SaveChanges();
                            }
                            catch
                            {

                            }
                        }
                        return Json(new { Status = true, Message = "Successfully" });
                    }
                    else
                    {
                        var entry1 = (from rch in db.Recharge_info_old
                                      where rch.idno == txtrefidno
                                      && rch.Rstaus.ToUpper().Contains("FAILED")
                                      select new
                                      {
                                          Idno = rch.idno,
                                          Mobile = rch.Mobile,
                                          Port = rch.portno,
                                          Amount = rch.amount,
                                          Provider = rch.optcode,
                                          Response = rch.Recharge_response,
                                          rch_from = rch.Rch_from,
                                          fid = rch.refid

                                      }).SingleOrDefault();

                        var RetailerEmailId = "";
                        var AdminEmail = db.Admin_details.Single().email;
                        string idap = Convert.ToString(txtrefidno);
                        var comm = db.Recharge_info_old.Where(p => p.idno == txtrefidno);
                        var Retailerid = comm.Single().Rch_from;
                        var mobileno = comm.Single().Mobile;
                        var optcode = comm.Single().optcode;
                        var Amount = comm.Single().amount;
                        var OperatorName = db.Operator_Code.Where(p => p.new_opt_code == optcode).Single().operator_Name;
                        try
                        {
                            RetailerEmailId = db.Retailer_Details.Where(p => p.RetailerId == Retailerid).Single().Email;
                        }
                        catch { }
                        try
                        {
                            RetailerEmailId = db.Whitelabel_Retailer_Details.Where(p => p.RetailerId == Retailerid).Single().Email;
                        }
                        catch { }
                        var statusAdmin = db.PushNotificationStatus.Where(a => a.UserRole == "Admin").SingleOrDefault().Status;
                        var statusRetailer = db.PushNotificationStatus.Where(a => a.UserRole == "Retailer").SingleOrDefault().Status;
                        if (currentstatus.ToUpper() == "SUCCESS")
                        {
                            db.recharge_update_success_to_failed_old(Convert.ToInt32(idap));

                            if (statusAdmin == "Y")
                            {
                                SendPushNotification(AdminEmail, Url.Action("Operator_report_new", "Home"), "Txn of " + mobileno + ". Amt :" + Amount + " - FAILED.(" + string.Format("{0:dd-MMM hh:mm:ss}", DateTime.Now) + ")", "Txn. Success To Failed.");
                            }
                            if (statusRetailer == "Y")
                            {
                                SendPushNotification(RetailerEmailId, "Home/RechargeReport", "Txn of " + mobileno + ". Amt :" + Amount + " - FAILED.(" + string.Format("{0:dd-MMM hh:mm:ss}", DateTime.Now) + ")", "Txn. Success To Failed.");
                            }
                            notify.sendmessage(RetailerEmailId, "Txn of " + mobileno + ". Amt :" + Amount + " - FAILED.(" + string.Format("{0:dd-MMM hh:mm:ss}", DateTime.Now) + ")");
                            //return Json(new { Status = true, Message = "Successfully" });
                        }
                        else if (currentstatus.ToUpper() == "FAILED")
                        {
                            db.recharge_update_failed_to_success_old(Convert.ToInt32(idap), "optval");
                            if (statusAdmin == "Y")
                            {
                                SendPushNotification(AdminEmail, Url.Action("Operator_report_new", "Home"), "Recharge Mobile No " + mobileno + ", Operator " + OperatorName + " is Success.", "Txn. Failed To Success..");
                            }
                            if (statusRetailer == "Y")
                            {
                                SendPushNotification(RetailerEmailId, Url.Action("RechargeReport", "Home"), "Recharge Mobile No " + mobileno + ", Operator " + OperatorName + " is Success.", "Txn. Failed To Success..");
                            }
                            notify.sendmessage(RetailerEmailId, "Recharge Mobile No " + mobileno + ", Operator " + OperatorName + " is Success By Admin.");
                            //return Json(new { Status = true, Message = "Successfully" });
                        }
                        else
                        {
                            db.recharge_update_old(idap, "FAILED", ddl_refund, 0, "", "Response");
                            if (statusAdmin == "Y")
                            {
                                SendPushNotification(AdminEmail, Url.Action("Operator_report_new", "Home"), "Recharge Mobile No " + mobileno + ", Operator " + OperatorName + " is Failed.", "Txn. Manual Failed ..");
                            }
                            if (statusRetailer == "Y")
                            {
                                SendPushNotification(RetailerEmailId, Url.Action("RechargeReport", "Home"), "Recharge Mobile No " + mobileno + ", Operator " + OperatorName + " is Failed.", "Txn. Manual Failed ..");
                            }
                            notify.sendmessage(RetailerEmailId, "Recharge Mobile No " + mobileno + ", Operator " + OperatorName + " is Manual Failed.");
                            //return Json(new { Status = true, Message = "Successfully" });
                        }


                        if (entry1.Mobile == "00000")
                        {
                            comm.Single().Rstaus = "Balance";
                            comm.Single().Resp_time = DateTime.Now;
                            comm.Single().lapu_bal = 0;
                            db.SaveChanges();
                        }
                        var role = (from rol in db.Roles join user in db.UserRoles on rol.RoleId equals user.RoleId where user.UserId == entry1.rch_from select rol.Name).SingleOrDefault().ToString();
                        if (role == "API")
                        {
                            try
                            {
                                var url = db.Recharge_Update_Url.Where(aa => aa.UserId == entry1.rch_from).SingleOrDefault().responseurl.ToString();
                                var curentbal = db.api_remain_amount.Where(aa => aa.apiid == entry1.rch_from).SingleOrDefault().balance.ToString();
                                url = url.Replace("rrr", entry1.fid);
                                url = url.Replace("sss", "Failed");
                                url = url.Replace("ooo", "Manual Failed");
                                url = url.Replace("bbb", curentbal);
                                HttpWebRequest WebRequestObject = (HttpWebRequest)HttpWebRequest.Create(url);
                                WebRequestObject.Timeout = (System.Int32)TimeSpan.FromSeconds(25).TotalMilliseconds;
                                WebResponse Response = WebRequestObject.GetResponse();
                                Stream WebStream = Response.GetResponseStream();
                                StreamReader Reader = new StreamReader(WebStream);
                                var webcontent = Reader.ReadToEnd();

                                Recharge_info_old obj = (from p in db.Recharge_info_old where p.idno == entry1.Idno select p).Single();
                                obj.API_response = webcontent.ToString();
                                obj.Api_Response_output = url.ToString();
                                db.SaveChanges();
                            }
                            catch
                            {

                            }
                        }
                        return Json(new { Status = true, Message = "Successfully" });
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { Status = false, Message = ex.Message });
                }
            }

            //end

        }
        [PermissioncheckingAttribute(servicename = "PREPAIDUTILITY", permision = "Write")]
        public ActionResult RchStatusCheck(string idno)
        {
            if (idno != null)
            {
                int iddno = Convert.ToInt32(idno);
                var stsArefid = db.Recharge_info.Where(aa => aa.idno == iddno).SingleOrDefault();
                if (stsArefid.Rstaus.ToUpper().Contains("REQUEST SEND"))
                {
                    var RefId = stsArefid.Order_id;
                    var ReqUrl = stsArefid.Recharge_request;
                    if (ReqUrl.ToUpper().Contains("MROBOTICS.IN"))
                    {
                        var tkn = db.Recharge_API_URLS.Where(aa => aa.API_Name == "mRobotics").SingleOrDefault();
                        var token = tkn.Token;
                        var role = (from rol in db.Roles join user in db.UserRoles on rol.RoleId equals user.RoleId where user.UserId == stsArefid.Rch_from select rol.Name).SingleOrDefault().ToString();
                        var client = new RestClient("https://mrobotics.in/api/order_id_status");
                        var request = new RestRequest(Method.POST);

                        request.AddHeader("Content-Type", "application/json");
                        //request.AddParameter("undefined", "api_token="+ token + "&order_id=" + RefId + "", ParameterType.RequestBody);
                        request.AddParameter("application/json", "{\n    \"api_token\": \"" + token + "\",\n    \"order_id\": \"" + RefId + "\"\n}", ParameterType.RequestBody);
                        IRestResponse response = client.Execute(request);
                        var responsechk = response.Content.ToString();
                        dynamic json = JsonConvert.DeserializeObject(responsechk);

                        string error = json.error.ToString();
                        if (error.ToUpper() == "FALSE")
                        {
                            try
                            {
                                string status = json.data.status.ToString();
                                string operatorid = json.data.tnx_id.ToString();

                                if (status.ToUpper() == "SUCCESS")
                                {
                                    status = "Success";
                                    db.recharge_update(idno.ToString(), status, operatorid, Convert.ToDecimal(0), json.ToString(), "Response");

                                    if (role == "API")
                                    {
                                        try
                                        {
                                            var url = db.Recharge_Update_Url.Where(aa => aa.UserId == stsArefid.Rch_from).SingleOrDefault().responseurl.ToString();
                                            var curentbal = db.api_remain_amount.Where(aa => aa.apiid == stsArefid.Rch_from).SingleOrDefault().balance.ToString();
                                            url = url.Replace("rrr", stsArefid.refid);
                                            url = url.Replace("sss", "Success");
                                            url = url.Replace("ooo", operatorid);
                                            url = url.Replace("bbb", curentbal);
                                            HttpWebRequest WebRequestObject = (HttpWebRequest)HttpWebRequest.Create(url);
                                            WebRequestObject.Timeout = (System.Int32)TimeSpan.FromSeconds(25).TotalMilliseconds;
                                            WebResponse Response = WebRequestObject.GetResponse();
                                            Stream WebStream = Response.GetResponseStream();
                                            StreamReader Reader = new StreamReader(WebStream);
                                            var webcontent = Reader.ReadToEnd();

                                            Recharge_info obj = (from p in db.Recharge_info where p.idno == stsArefid.idno select p).Single();
                                            obj.Api_Response_output = url.ToString();
                                            db.SaveChanges();
                                        }
                                        catch
                                        {

                                        }
                                    }

                                    return Json(new { Status = "Success", Currentsts = "Success" });
                                }
                                else if (status.ToUpper() == "FAILURE")
                                {
                                    status = "Failed";
                                    db.recharge_update(idno.ToString(), status, operatorid, Convert.ToDecimal(0), json.ToString(), "Response");

                                    if (role == "API")
                                    {
                                        try
                                        {
                                            var url = db.Recharge_Update_Url.Where(aa => aa.UserId == stsArefid.Rch_from).SingleOrDefault().responseurl.ToString();
                                            var curentbal = db.api_remain_amount.Where(aa => aa.apiid == stsArefid.Rch_from).SingleOrDefault().balance.ToString();
                                            url = url.Replace("rrr", stsArefid.refid);
                                            url = url.Replace("sss", "Failed");
                                            url = url.Replace("ooo", operatorid);
                                            url = url.Replace("bbb", curentbal);
                                            HttpWebRequest WebRequestObject = (HttpWebRequest)HttpWebRequest.Create(url);
                                            WebRequestObject.Timeout = (System.Int32)TimeSpan.FromSeconds(25).TotalMilliseconds;
                                            WebResponse Response = WebRequestObject.GetResponse();
                                            Stream WebStream = Response.GetResponseStream();
                                            StreamReader Reader = new StreamReader(WebStream);
                                            var webcontent = Reader.ReadToEnd();

                                            Recharge_info obj = (from p in db.Recharge_info where p.idno == stsArefid.idno select p).Single();
                                            obj.Api_Response_output = url.ToString();
                                            db.SaveChanges();
                                        }
                                        catch
                                        {

                                        }
                                    }

                                    return Json(new { Status = "Failed", Currentsts = "Failed" });
                                }
                                else
                                {
                                    return Json(new { Status = "Success", Currentsts = "Pending" });
                                }

                            }
                            catch
                            {
                                return Json(new { Status = "Failed" });
                            }
                        }
                        else
                        {
                            return Json(new { Status = "", Currentsts = json.errorMessage });
                        }

                    }

                    else
                    {
                        return Json(new { Status = "", balance = "" });
                    }
                }
                else
                {
                    return Json(new { Status = "", balance = "" });
                }
            }
            else
            {
                return Json(new { Status = "", balance = "" });
            }

        }

        public ActionResult service(string Id)
        {

            try
            {
                if (Id != null)
                {

                    Id = Regex.Replace(Id, " ", "");
                    var query1 = (from sr in db.PortManagers where sr.PortNo == Id select sr.Sname).Single().ToString();
                    return Json(query1, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("", JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                return Json("okk", JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult codelist(string port)
        {
            try
            {
                var query1 = db.SRS_code_all(port).ToList();
                return Json(new SelectList(query1.ToArray(), "mars", "mars"), JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new SelectList("", "mars", "mars"), JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult chhkbopt(string id, string aa)
        {
            string[] words = aa.Split(',');
            int aa1 = words.Count();
            int a12 = aa1 - 1;
            if (a12 > 0)
            {
                foreach (string word in words)
                {
                    var id1 = word;
                    if (id1 != "")
                    {
                        db.srs_chk_opt(id, id1);
                    }
                }
            }
            else
            {
                db.srs_chk_opt(id, "1");
            }
            var hhh = (from jj in db.PortManagers where jj.PortNo == id select jj).Single().PortNm.ToString();
            return Json(hhh, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExcelRechargereport(string txt_frm_date, string txt_to_date, string txtmob, string ddl_status, string lapuno11, string Operator, string ddlusers, string allmaster1, string alldealer, string allretailer, string Whitelabel, string API, string txtdemo)
        {
            var optname = "";
            var portname = "";
            var mobile = "";
            var ddlst = "";
            string userid = User.Identity.GetUserId();

            if (ddlusers == "Master")
            {
                if (allmaster1 == "" || allmaster1.Contains("Master") || allmaster1 == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = allmaster1;
                }
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
            if (ddlusers == "WAdmin")
            {
                if (Whitelabel == "" || Whitelabel.Contains("Select Whitelabel") || Whitelabel == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = Whitelabel;
                }
            }
            if (ddlusers == "API")
            {
                if (API == "" || API.Contains("Select API") || API == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = API;
                }
            }
            if (ddlusers == "" || ddlusers == null || ddlusers == "Admin")
            {
                userid = "ALL";
                ddlusers = "Admin";
            }


            if ((txt_frm_date == null && txt_to_date == null) || (txt_frm_date == "" && txt_to_date == ""))
            {
                txt_frm_date = DateTime.Now.ToString();
                txt_to_date = DateTime.Now.ToString();

            }
            if (ddl_status == "" || ddl_status == null || ddl_status.ToUpper().Contains("ALL STATUS"))
            {
                //optname = "ALL";
                ddlst = "ALL";
            }
            else
            {
                ddlst = ddl_status;
            }

            if (Operator == "" || Operator == null || Operator.Contains("ALL OPERATORS") || Operator == null)
            {
                //optname = "ALL";
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
                txt_frm_date = DateTime.Now.AddDays(-30).ToString();
                txt_to_date = DateTime.Now.ToString();
                mobile = txtmob;
            }
            if (lapuno11 == "" || lapuno11 == null || lapuno11.Contains("Select Port"))
            {

                portname = "ALL";
            }
            else
            {
                portname = lapuno11.ToUpper();
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
            var rowdata = db.proc_Recharge_operator_report_newPaging(1, 10000000, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), ddlusers, userid, ddlst, optname, mobile, portname).ToList();

            DataTable dataTbl = new DataTable();
            dataTbl.Columns.Add("Live Status", typeof(string));
            dataTbl.Columns.Add("Firm Name", typeof(string));
            dataTbl.Columns.Add("Operator Name", typeof(string));
            dataTbl.Columns.Add("Customer No", typeof(string));
            dataTbl.Columns.Add("Amount", typeof(string));
            dataTbl.Columns.Add("User Pre ₹", typeof(string));
            dataTbl.Columns.Add("User Post ₹", typeof(string));
            dataTbl.Columns.Add("Port/API", typeof(string));
            dataTbl.Columns.Add("Lapu Remain", typeof(string));
            dataTbl.Columns.Add("Dealer_Pre_remain", typeof(string));
            dataTbl.Columns.Add("Dealer_Post_remain", typeof(string));
            dataTbl.Columns.Add("master_remain_pre", typeof(string));
            dataTbl.Columns.Add("master_Post_Remain", typeof(string));
            dataTbl.Columns.Add("Transition ID", typeof(string));
            dataTbl.Columns.Add("Recharge Time", typeof(string));
            dataTbl.Columns.Add("Response Time", typeof(string));
            dataTbl.Columns.Add("Operator ID", typeof(string));

            if (rowdata.Count > 0)
            {
                foreach (var item in rowdata)
                {
                    var sts = item.rstatus;
                    if (item.rstatus.ToUpper().Contains("REQ"))
                    {
                        sts = "Pending";
                    }
                    dataTbl.Rows.Add(sts, item.frm_name, item.Operator_name, item.mobile, item.amount, item.user_remain_pre, item.remain,
                       item.portno, item.lapubal, item.dealer_remain_pre, item.dealerremain, item.master_remain_pre, item.masterremain,
                   item.opt_id, @item.Rch_time, item.resp_time, item.opt_id);
                }
            }
            else
            {
                dataTbl.Rows.Add("", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "");
            }
            var grid = new GridView();
            grid.DataSource = dataTbl;
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

        public ActionResult PDFRechargereport(string txt_frm_date, string txt_to_date, string txtmob, string ddl_status, string lapuno11, string Operator, string ddlusers, string allmaster1, string alldealer, string allretailer, string Whitelabel, string API, string txtdemo)
        {
            var optname = "";
            var portname = "";
            var mobile = "";
            var ddlst = "";
            string userid = User.Identity.GetUserId();

            if (ddlusers == "Master")
            {
                if (allmaster1 == "" || allmaster1.Contains("Master") || allmaster1 == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = allmaster1;
                }
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
            if (ddlusers == "WAdmin")
            {
                if (Whitelabel == "" || Whitelabel.Contains("Select Whitelabel") || Whitelabel == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = Whitelabel;
                }
            }
            if (ddlusers == "API")
            {
                if (API == "" || API.Contains("Select API") || API == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = API;
                }
            }
            if (ddlusers == "" || ddlusers == null || ddlusers == "Admin")
            {
                userid = "ALL";
                ddlusers = "Admin";
            }


            if ((txt_frm_date == null && txt_to_date == null) || (txt_frm_date == "" && txt_to_date == ""))
            {
                txt_frm_date = DateTime.Now.ToString();
                txt_to_date = DateTime.Now.ToString();

            }
            if (ddl_status == "" || ddl_status == null || ddl_status.ToUpper().Contains("ALL STATUS"))
            {
                //optname = "ALL";
                ddlst = "ALL";
            }
            else
            {
                ddlst = ddl_status;
            }

            if (Operator == "" || Operator == null || Operator.Contains("ALL OPERATORS") || Operator == null)
            {
                //optname = "ALL";
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
                txt_frm_date = DateTime.Now.AddDays(-30).ToString();
                txt_to_date = DateTime.Now.ToString();
                mobile = txtmob;
            }
            if (lapuno11 == "" || lapuno11 == null || lapuno11.Contains("Select Port"))
            {

                portname = "ALL";
            }
            else
            {
                portname = lapuno11.ToUpper();
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
            var rowdata = db.proc_Recharge_operator_report_newPaging(1, 10000000, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), ddlusers, userid, ddlst, optname, mobile, portname).ToList();

            return new ViewAsPdf(rowdata);
        }


        [PermissioncheckingAttribute(servicename = "PREPAIDUTILITY", permision = "Write")]
        protected void ApiUserResponse(int Idno, string rch_from, string fid, string status, string operatorId)
        {
            var role = (from rol in db.Roles join user in db.UserRoles on rol.RoleId equals user.RoleId where user.UserId == rch_from select rol.Name).SingleOrDefault().ToString();
            if (role == "API")
            {
                try
                {
                    var url = db.Recharge_Update_Url.Where(aa => aa.UserId == rch_from).SingleOrDefault().responseurl.ToString();
                    var curentbal = db.api_remain_amount.Where(aa => aa.apiid == rch_from).SingleOrDefault().balance.ToString();
                    url = url.Replace("rrr", fid);
                    url = url.Replace("sss", status);
                    url = url.Replace("ooo", operatorId);
                    url = url.Replace("bbb", curentbal);
                    HttpWebRequest WebRequestObject = (HttpWebRequest)HttpWebRequest.Create(url);
                    WebRequestObject.Timeout = (System.Int32)TimeSpan.FromSeconds(25).TotalMilliseconds;
                    WebResponse Response = WebRequestObject.GetResponse();
                    Stream WebStream = Response.GetResponseStream();
                    StreamReader Reader = new StreamReader(WebStream);
                    var webcontent = Reader.ReadToEnd();

                    var liveId = db.Recharge_info.Where(a => a.idno == Idno).SingleOrDefault();
                    if (liveId != null)
                    {
                        Recharge_info obj = (from p in db.Recharge_info where p.idno == Idno select p).SingleOrDefault();
                        obj.API_response = webcontent.ToString();
                        obj.Api_Response_output = url.ToString();
                        db.SaveChanges();
                    }
                    var OldId = db.Recharge_info_old.Where(a => a.idno == Idno).SingleOrDefault();
                    if (OldId != null)
                    {
                        Recharge_info_old obj = (from p in db.Recharge_info_old where p.idno == Idno select p).SingleOrDefault();
                        obj.API_response = webcontent.ToString();
                        obj.Api_Response_output = url.ToString();
                        db.SaveChanges();
                    }
                }
                catch
                {

                }
            }
        }
        private static Random random = new Random();
        public string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        #endregion



        public ActionResult ShowAll(int idno, string disput)
        {
            var ch = db.clear_dispute_list(idno);
            ViewBag.dis = disput == null ? "Not Region Found" : disput;
            return View(ch);
        }
        public ActionResult Dispute_list()
        {
            ViewData["success"] = TempData["success"];
            ViewData["error"] = TempData["error"];
            TempData.Remove("success");
            TempData.Remove("error");


            return View();
        }
        [HttpPost]
        public ActionResult Dispute_list(string ddl_status, string txt_frm_date, string txt_to_date)
        {
            ViewBag.chk = "post";

            return View();
        }
        [ChildActionOnly]
        public ActionResult _Dispute_list(string ddl_status, string txt_frm_date, string txt_to_date)
        {
            if (string.IsNullOrEmpty(txt_frm_date) || string.IsNullOrEmpty(txt_to_date))
            {
                txt_frm_date = DateTime.Now.ToString();
                txt_to_date = DateTime.Now.ToString();
                string frm_date = Convert.ToDateTime(txt_frm_date).Date.ToString("yyyy-MM-dd");
                string to_date = Convert.ToDateTime(txt_to_date).AddDays(1).ToString("yyyy-MM-dd");
                var ch = db.all_dispute_list(1, 20, ddl_status, frm_date, to_date).ToList();
                return View(ch);
            }
            else
            {
                DateTime frm = DateTime.ParseExact(txt_frm_date, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                DateTime to = DateTime.ParseExact(txt_to_date, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                to = to.AddDays(1);
                string frm_date = frm.ToString("yyyy-MM-dd");
                string to_date = to.ToString("yyyy-MM-dd");
                var ch = db.all_dispute_list(1, 20, ddl_status, frm_date, to_date).ToList();
                return View(ch);
            }
        }
        public ActionResult InfiniteScrollDispute(int pageindex, string ddl_status, string txt_frm_date, string txt_to_date)
        {
            int pagesize = 20;
            DateTime frm = DateTime.ParseExact(txt_frm_date, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            DateTime to = DateTime.ParseExact(txt_to_date, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            to = to.AddDays(1);
            string frm_date = frm.ToString("yyyy-MM-dd");
            string to_date = to.ToString("yyyy-MM-dd");
            var tbrow = db.all_dispute_list(pageindex, pagesize, ddl_status, frm_date, to_date).ToList();
            JsonModel jsonmodel = new JsonModel();
            jsonmodel.NoMoredata = tbrow.Count < pagesize;
            jsonmodel.HTMLString = renderPartialViewtostring("_Dispute_list", tbrow);
            return Json(jsonmodel);

        }
        public virtual ActionResult ExcelRechargereport_Dispute(string ddl_status, string txt_frm_date, string txt_to_date)
        {
            DataTable dt2 = new DataTable();
            dt2.Columns.Add("Status", typeof(string));
            dt2.Columns.Add("User Firm", typeof(string));
            dt2.Columns.Add("Rch No.", typeof(string));
            dt2.Columns.Add("Amount", typeof(string));
            dt2.Columns.Add("Operator", typeof(string));
            dt2.Columns.Add("Port No", typeof(string));
            dt2.Columns.Add("Port Nm", typeof(string));
            dt2.Columns.Add("Rch Time", typeof(string));
            dt2.Columns.Add("Disp Time", typeof(string));
            dt2.Columns.Add("Dispute Reason", typeof(string));
            dt2.Columns.Add("Transition ID", typeof(string));
            dt2.Columns.Add("Response", typeof(string));

            DateTime frm = DateTime.ParseExact(txt_frm_date, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            DateTime to = DateTime.ParseExact(txt_to_date, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            to = to.AddDays(1);
            string frm_date = frm.ToString("yyyy-MM-dd");
            string to_date = to.ToString("yyyy-MM-dd");
            var respo = db.all_dispute_list(1, 1000000, ddl_status, frm_date, to_date).ToList();
            if (respo.Count > 0)
            {
                foreach (var item in respo)
                {
                    dt2.Rows.Add(item.sts, item.rch_from, item.rch_number, item.rch_amount, item.opt, item.portno, item.portnm, item.rch_time,
            item.dis_time, item.dispute_region, item.opt_id, item.comment);
                }
            }
            else
            {
                dt2.Rows.Add("", "", "", "", "", "", "", "", "", "", "", "");
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


        public ActionResult diputerejected(string idno, string txtregion)
        {
            try
            {
                int idn = Convert.ToInt32(idno);

                // Sabhi matching records fetch karo
                var disputes = db.dispute_list.Where(a => a.rch_id == idn.ToString()).ToList();

                if (disputes.Count == 0)
                {
                    TempData["error"] = "No dispute entry found with this ID.";
                    return RedirectToAction("Dispute_list");
                }

                // Pehle record se email nikal lo
                var userid = disputes.FirstOrDefault().rch_from;
                var EmailId = db.Users.Where(a => a.UserId == userid).Select(a => a.Email).FirstOrDefault();

                // Sabhi disputes ko update karo (agar delete chahte ho to `db.dispute_list.RemoveRange(disputes);` use karein)
                foreach (var dispute in disputes)
                {
                    dispute.sts = "Rejected";
                    dispute.comment = txtregion;
                }

                db.SaveChanges();

                // Push notification bhejo
                SendPushNotification(EmailId, Url.Action("Dispute_list", "Home"),
                    "Your Dispute is Clear By Admin and Dispute Message is that " + txtregion + " .", "Dispute Clear..");

                TempData["success"] = "Dispute Rejected Successfully.";
                return RedirectToAction("Dispute_list");
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return RedirectToAction("Dispute_list");
            }
        }



        [HttpGet]
        public ActionResult All_Retailer_Status()
        {
            var ch = db.show_all_active_inactive_rem_list().ToList();
            activeinactive viewModel = new activeinactive();
            viewModel.active = ch.Where(aa => aa.type == "ACTIVE");
            viewModel.inactive = ch.Where(aa => aa.type == "INACTIVE");
            return View(viewModel);
        }

        public ActionResult Excel_All_Retailer_Status()
        {
            var ch = db.show_all_active_inactive_rem_list().ToList();

            DataTable dataTbl = new DataTable();
            dataTbl.Columns.Add("Firm Name", typeof(string));
            dataTbl.Columns.Add("Mobile", typeof(string));
            dataTbl.Columns.Add("Date", typeof(string));
            dataTbl.Columns.Add("Day's", typeof(string));
            dataTbl.Columns.Add("Status", typeof(string));

            if (ch.Count > 0)
            {
                foreach (var item in ch)
                {
                    dataTbl.Rows.Add(item.frm_name, item.mobile, item.Rch_time, item.todays, item.type);
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
            Response.AddHeader("content-disposition", "attachment; filename=All_Retailer_Status.xls");
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

        #region Retailer Bank List
        public ActionResult RetailerBnk()
        {
            TempData.Keep("msgbox");
            //var ad = db.Retailer_Details.Single(a => a.RetailerId == txtid3);
            //ad.accountholder = string.IsNullOrWhiteSpace(txtaccholder) ? ad.accountholder : txtaccholder;
            //ad.Bankaccountno = string.IsNullOrWhiteSpace(txtbankaccountno) ? ad.Bankaccountno : txtbankaccountno;
            //ad.Ifsccode = string.IsNullOrWhiteSpace(txtifsc) ? ad.Ifsccode : txtifsc;
            //ad.bankname = string.IsNullOrWhiteSpace(txtbankname) ? ad.bankname : txtbankname;
            //ad.bankAddress = string.IsNullOrWhiteSpace(txtbranchaddress) ? ad.bankAddress : txtbranchaddress;
            //db.SaveChanges();

            var RemPendingAc = db.ShowBankRequest().ToList();// db.UpdateREMAccounts.ToList();
            //db.UpdateREMAccounts.Where(a => a.UserId == userid && a.Status.ToUpper() == "PENDING").OrderByDescending(a => a.Date).Take(1);
            return View(RemPendingAc);
        }

        public ActionResult bankApproveOTP(string userid)
        {
            //var BANKEXIST = db.UpdateREMAccounts.Where(a => a.UserId == userid && a.Status.ToUpper() == "Approved").Any();
            //if (BANKEXIST == false)
            //{
            Random ran = new Random();
            var otprem = ran.Next(1111, 9999);

            MobileOtp mob = new MobileOtp();
            mob.mobileno = "";
            mob.Userid = userid;
            mob.Type = "RetailerBANKAPPROVEDOTP";
            mob.Otp = otprem.ToString();
            mob.Date = DateTime.Now;
            db.MobileOtps.Add(mob);
            db.SaveChanges();


            var admininfo = db.Admin_details.SingleOrDefault();

            CommUtilEmail emailsend = new CommUtilEmail();
            var retaileremail = db.Users.Where(x => x.UserId == userid).SingleOrDefault();
            if (retaileremail != null)
            {
                emailsend.EmailLimitChk(retaileremail.Email, "OK", "Retailer Bank Approved", "Retailer Bank Approved OTP is " + otprem, "");
            }

            var otpadmin = ran.Next(1111, 9999);


            MobileOtp mob1 = new MobileOtp();
            mob1.mobileno = "";
            mob1.Userid = userid;
            mob1.Type = "AdminBANKAPPROVEDOTP";
            mob1.Otp = otpadmin.ToString();
            mob1.Date = DateTime.Now;
            db.MobileOtps.Add(mob1);
            db.SaveChanges();

            emailsend.EmailLimitChk(admininfo.email, "OK", "Admin Bank Approved", "Admin Bank Approved OTP is " + otpadmin, "");

            return Json("DONE", JsonRequestBehavior.AllowGet);
            //}
            //else
            //{
            //    return Json("NOTDONE", JsonRequestBehavior.AllowGet);
            //}
        }
        [HttpPost]
        public ActionResult RetailerBnkApprove(int hdidno, string remotp, string adminotp, string hduserid)
        {
            var otpchkretailer = db.MobileOtps.Where(aa => aa.Userid == hduserid && aa.Type == "RetailerBANKAPPROVEDOTP").OrderByDescending(aa => aa.Date).Take(1).SingleOrDefault().Otp;
            var otpchkadmin = db.MobileOtps.Where(aa => aa.Userid == hduserid && aa.Type == "AdminBANKAPPROVEDOTP").OrderByDescending(aa => aa.Date).Take(1).SingleOrDefault().Otp;
            if (otpchkretailer == remotp && otpchkadmin == adminotp)
            {
                if (hdidno > 0)
                {
                    var acDts = db.UpdateREMAccounts.Single(a => a.Idno == hdidno && a.Status.ToUpper() == "PENDING");
                    if (acDts != null)
                    {
                        var userid = acDts.UserId;
                        var ad = db.Retailer_Details.Where(aa => aa.ISDeleteuser == false).Single(a => a.RetailerId == hduserid);

                        if (ad.Bankaccountno == null)
                        {
                            ad.accountholder = string.IsNullOrWhiteSpace(acDts.AcconutHolderName) ? ad.accountholder : acDts.AcconutHolderName;
                            ad.Bankaccountno = string.IsNullOrWhiteSpace(acDts.BankAccountNo) ? ad.Bankaccountno : acDts.BankAccountNo;
                            ad.Ifsccode = string.IsNullOrWhiteSpace(acDts.IFSC_CODE) ? ad.Ifsccode : acDts.IFSC_CODE;
                            ad.bankname = string.IsNullOrWhiteSpace(acDts.Bank_Name) ? ad.bankname : acDts.Bank_Name;
                            ad.bankAddress = string.IsNullOrWhiteSpace(acDts.Bank_Address) ? ad.bankAddress : acDts.Bank_Address;
                        }

                        acDts.Status = "Approved";
                        acDts.Approved_Date = DateTime.Now;

                        db.SaveChanges();
                        TempData["msgbox"] = "Successfuly Approve";
                        return RedirectToAction("RetailerBnk");
                    }
                }

                //  return Json("OK", JsonRequestBehavior.AllowGet);
            }
            else
            {
                TempData["msgbox"] = "OTP MissMatch";
                //  return Json("OTP MissMatch", JsonRequestBehavior.AllowGet);
                return RedirectToAction("RetailerBnk");
            }
            TempData["msgbox"] = "Error";
            //  return Json("OTP MissMatch", JsonRequestBehavior.AllowGet);
            return RedirectToAction("RetailerBnk");

        }
        public ActionResult RetailerBnkReject(int Idno)
        {
            if (Idno > 0)
            {
                var acDts = db.UpdateREMAccounts.Where(a => a.Idno == Idno).SingleOrDefault();
                if (acDts != null)
                {
                    string path = Server.MapPath("~/" + acDts.CancelCheckFile);
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                    db.UpdateREMAccounts.Remove(acDts);
                    db.SaveChanges();


                }
            }
            return RedirectToAction("RetailerBnk");
        }


        public ActionResult DeleteRetailerBnkt(int Idno)
        {
            if (Idno > 0)
            {
                var acDts = db.UpdateREMAccounts.Where(a => a.Idno == Idno).SingleOrDefault();
                var isdeleteallow = true;

                if (acDts.Status == "Approved")
                {
                    if (Convert.ToDateTime(acDts.Approved_Date).AddMonths(1) >= DateTime.Now)
                    {
                        isdeleteallow = false;
                    }
                }
                if (isdeleteallow)
                {
                    if (acDts != null)
                    {
                        deletedApprveBankInfo tblbank = new deletedApprveBankInfo();
                        tblbank.UserId = acDts.UserId;
                        tblbank.AcconutHolderName = acDts.AcconutHolderName;
                        tblbank.BankAccountNo = acDts.BankAccountNo;
                        tblbank.IFSC_CODE = acDts.IFSC_CODE;
                        tblbank.Bank_Name = acDts.Bank_Name;
                        tblbank.Bank_Address = acDts.Bank_Address;
                        tblbank.Status = acDts.Status;
                        tblbank.Date = acDts.Date;
                        tblbank.Approved_Date = acDts.Approved_Date;
                        tblbank.Email = acDts.Email;
                        tblbank.Firm_Name = acDts.Firm_Name;
                        tblbank.CancelCheckFile = acDts.CancelCheckFile;
                        tblbank.deleteddate = DateTime.Now;
                        db.deletedApprveBankInfoes.Add(tblbank);
                        db.SaveChanges();

                        var ad = db.UpdateREMAccounts.Single(a => a.Idno == Idno);
                        var retailerbank = db.Retailer_Details.Where(x => x.RetailerId == ad.UserId && x.Bankaccountno == ad.BankAccountNo).FirstOrDefault();
                        if (retailerbank != null)
                        {
                            retailerbank.bankAddress = null;
                            retailerbank.bankname = null;
                            retailerbank.Bankaccountno = null;
                            retailerbank.Ifsccode = null;
                            retailerbank.accountholder = null;
                            db.SaveChanges();


                            db.UpdateREMAccounts.Remove(ad);
                            db.SaveChanges();

                            var sdbanks = db.UpdateREMAccounts.Where(a => a.UserId == ad.UserId && a.Status == "Approved").FirstOrDefault();
                            var retailersd = db.Retailer_Details.Where(a => a.RetailerId == ad.UserId).FirstOrDefault();
                            if (sdbanks != null && retailersd.Bankaccountno == null)
                            {

                                retailersd.accountholder = string.IsNullOrWhiteSpace(sdbanks.AcconutHolderName) ? retailersd.accountholder : sdbanks.AcconutHolderName;
                                retailersd.Bankaccountno = string.IsNullOrWhiteSpace(sdbanks.BankAccountNo) ? retailersd.Bankaccountno : sdbanks.BankAccountNo;
                                retailersd.Ifsccode = string.IsNullOrWhiteSpace(sdbanks.IFSC_CODE) ? retailersd.Ifsccode : sdbanks.IFSC_CODE;
                                retailersd.bankname = string.IsNullOrWhiteSpace(sdbanks.Bank_Name) ? retailersd.bankname : sdbanks.Bank_Name;
                                retailersd.bankAddress = string.IsNullOrWhiteSpace(sdbanks.Bank_Address) ? retailersd.bankAddress : sdbanks.Bank_Address;
                                db.SaveChanges();
                            }
                        }
                        else
                        {
                            db.UpdateREMAccounts.Remove(ad);
                            db.SaveChanges();
                        }
                    }
                }
            }
            return RedirectToAction("RetailerBnk");
        }

        public ActionResult Delete_Approve_Bank_list()
        {
            var model = db.deletedApprveBankInfoes.OrderByDescending(x => x.deleteddate).ToList();
            return View(model);
        }
        #endregion 

        #region Imps Report
        //IMPS 
        [HttpGet]
        [PermissioncheckingAttribute(servicename = "DMT", permision = "Read")]
        public ActionResult Money_Transfer_Report()
        {
            string userid = User.Identity.GetUserId();
            string txt_frm_date = DateTime.Now.ToString();
            string txt_to_date = DateTime.Now.ToString();
            string frm_date = Convert.ToDateTime(txt_frm_date).Date.ToString("yyyy-MM-dd");
            string to_date = Convert.ToDateTime(txt_to_date).AddDays(1).ToString("yyyy-MM-dd");
            //show master id 
            var stands = db.Superstokist_details.ToList();
            IEnumerable<SelectListItem> selectList = from s in stands
                                                     select new SelectListItem
                                                     {
                                                         Value = s.SSId,
                                                         Text = s.FarmName.ToString()
                                                     };
            ViewBag.allmaster = new SelectList(selectList, "Value", "Text");
            ViewBag.alldealer = new SelectList(db.select_Dealer_for_ddl(), "Dealerid", "FarmName", null).ToList();
            ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl("Admin"), "RetailerId", "Frm_Name", null).ToList();
            ViewBag.apiid = new SelectList(db.API_all_apiid(), "apiid", "farmname");
            ViewBag.whitelabel = new SelectList(db.Whitelabel_all_whitelabelid(), "WhiteLabelID", "FrmName");
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
        [HttpPost]
        public ActionResult Money_Transfer_Report(string txt_frm_date, string txt_to_date, string ddlusers, string allmaster, string alldealer, string allretailer, string Whitelabel, string ddl_status, string allapiuser, string txtmob, string ddl_Type)
        {
            string userid = User.Identity.GetUserId();
            ViewBag.chk = "post";

            var stands = db.Superstokist_details.ToList();
            IEnumerable<SelectListItem> selectList = from s in stands
                                                     select new SelectListItem
                                                     {
                                                         Value = s.SSId,
                                                         Text = s.FarmName.ToString()
                                                     };
            ViewBag.allmaster = new SelectList(selectList, "Value", "Text");
            ViewBag.alldealer = new SelectList(db.select_Dealer_for_ddl(), "Dealerid", "FarmName", null).ToList();
            ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl("Admin"), "RetailerId", "Frm_Name", null).ToList();
            ViewBag.apiid = new SelectList(db.API_all_apiid(), "apiid", "farmname");
            ViewBag.whitelabel = new SelectList(db.Whitelabel_all_whitelabelid(), "WhiteLabelID", "FrmName");
            var apiname = db.money_api_status.ToList();
            IEnumerable<SelectListItem> selectapiname = from p in apiname
                                                        select new SelectListItem
                                                        {
                                                            Value = p.api_name,
                                                            Text = p.api_name
                                                        };
            ViewBag.apiname = new SelectList(selectapiname, "Value", "Text");


            string frm_date = Convert.ToDateTime(txt_frm_date).Date.ToString("yyyy-MM-dd");
            string to_date = Convert.ToDateTime(txt_to_date).AddDays(1).ToString("yyyy-MM-dd");

            ViewBag.apiname = new SelectList(selectapiname, "Value", "Text");

            return View();
        }
        [ChildActionOnly]
        [PermissioncheckingAttribute(servicename = "DMT", permision = "Read")]
        public ActionResult _Money_TransferReport(string txt_frm_date, string txt_to_date, string ddlusers, string allmaster, string alldealer, string allretailer, string Whitelabel, string ddl_status, string allapiuser, string txtmob, string ddl_Type)
        {

            string userid = User.Identity.GetUserId();
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

            ViewBag.apiid = new SelectList(db.select_apiusers_for_ddl("Admin"), "apiid", "username", null).ToList();
            string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy",
                            "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
            DateTime dt = DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            DateTime dt1 = DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            string frm_date = Convert.ToDateTime(dt).ToShortDateString();
            string to_date = Convert.ToDateTime(dt1).AddDays(1).ToString();


            if (ddlusers == "Master")
            {
                if (allmaster == "" || allmaster.Contains("allmaster") || allmaster == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = allmaster;
                }
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

            if (ddlusers == "WAdmin")
            {
                if (Whitelabel == "" || Whitelabel.Contains("Whitelabel") || Whitelabel == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = Whitelabel;
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


            if (txtmob == null)
            {
                txtmob = "";

            }
            int pagesize = 35;

            var ch = db.money_transfer_report_paging(ddlusers, userid, ddl_status, "VASTWEB", Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), ddl_Type, 1, pagesize).ToList();

            txtmob = txtmob.Trim();
            if (txtmob != "")
            {
                ch = db.money_transfer_report_paging(ddlusers, userid, ddl_status, "VASTWEB", Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), ddl_Type, 1, 15000).Where(z => z.senderno == txtmob || z.accountno == txtmob).ToList();//(aa => aa.senderno == txtmob || aa.accountno == txtmob).ToList();
                                                                                                                                                                                                                                                // ch = ch.Where(aa => aa.senderno == txtmob || aa.accountno == txtmob).ToList();
            }
            var rowdata = ch;


            return View(rowdata);
        }
        [PermissioncheckingAttribute(servicename = "DMT", permision = "Read")]
        public ActionResult InfiniteScroll1(int pageindex, string txt_frm_date, string txt_to_date, string ddlusers, string allmaster, string alldealer, string allretailer, string Whitelabel, string ddl_status, string allapiuser, string txtmob, string ddl_Type)
        {
            string userid = User.Identity.GetUserId();


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
                if (allmaster == "" || allmaster.Contains("allmaster") || allmaster == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = allmaster;
                }
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

            if (ddlusers == "WAdmin")
            {
                if (Whitelabel == "" || Whitelabel.Contains("Whitelabel") || Whitelabel == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = Whitelabel;
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


            if (txtmob == null)
            {
                txtmob = "";

            }


            System.Threading.Thread.Sleep(1000);
            int pagesize = 35;
            var tbrow = db.money_transfer_report_paging(ddlusers, userid, ddl_status, "VASTWEB", Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), ddl_Type, pageindex, pagesize).ToList();
            txtmob = txtmob.Trim();
            if (txtmob != "")
            {
                tbrow = db.money_transfer_report_paging(ddlusers, userid, ddl_status, "VASTWEB", Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), ddl_Type, pageindex, pagesize).ToList();
                tbrow = tbrow.Where(aa => aa.senderno == txtmob || aa.accountno == txtmob).ToList();
            }
            JsonModel jsonmodel = new JsonModel();
            jsonmodel.NoMoredata = tbrow.Count < pagesize;
            jsonmodel.HTMLString = renderPartialViewtostring("_Money_TransferReport", tbrow);
            return Json(jsonmodel);
        }
        [PermissioncheckingAttribute(servicename = "DMT", permision = "Read")]
        public virtual ActionResult ExcelMoneyReport(string txt_frm_date, string txt_to_date, string ddlusers, string ddl_status, string allmaster, string alldealer, string allretailer, string allapiuser, string Whitelabel, string ddl_Type)
        {
            var userid = User.Identity.GetUserId();
            if (ddlusers == "Master")
            {
                if (allmaster == "" || allmaster.Contains("allmaster") || allmaster == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = allmaster;
                }
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

            if (ddlusers == "WAdmin")
            {
                if (Whitelabel == "" || Whitelabel.Contains("Whitelabel") || Whitelabel == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = Whitelabel;
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

            DateTime now = DateTime.Now.Date;

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


            DataTable dataTbl = new DataTable();
            dataTbl.Columns.Add("User's Information", typeof(string));
            dataTbl.Columns.Add("Type", typeof(string));
            dataTbl.Columns.Add("Sender No", typeof(string));
            dataTbl.Columns.Add("Beneficiary Ac", typeof(string));
            dataTbl.Columns.Add("Net T/F", typeof(string));
            dataTbl.Columns.Add("User Pre", typeof(string));
            dataTbl.Columns.Add("Debit", typeof(string));
            dataTbl.Columns.Add("User Post", typeof(string));
            dataTbl.Columns.Add("Api Bal", typeof(string));
            dataTbl.Columns.Add("Req Time", typeof(string));
            dataTbl.Columns.Add("Bank UTR", typeof(string));
            dataTbl.Columns.Add("Request ID", typeof(string));
            dataTbl.Columns.Add("Bank Name", typeof(string));
            dataTbl.Columns.Add("Account Holder", typeof(string));
            dataTbl.Columns.Add("Master Pre", typeof(string));
            dataTbl.Columns.Add("Master Credit", typeof(string));
            dataTbl.Columns.Add("Master Post", typeof(string));
            dataTbl.Columns.Add("Dealer Pre", typeof(string));
            dataTbl.Columns.Add("Dealer Credit", typeof(string));
            dataTbl.Columns.Add("Dealer Post", typeof(string));
            dataTbl.Columns.Add("IFCS", typeof(string));
            dataTbl.Columns.Add("IP Address", typeof(string));

            var respo = db.money_transfer_report_paging(ddlusers, userid, ddl_status, "VASTWEB", Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), ddl_Type, 1, 10000000).ToList();

            if (respo.Count > 0)
            {
                foreach (var item in respo)
                {
                    /********************************************************************/
                    decimal debit1 = 0;
                    decimal amount1 = 0;
                    decimal usercomm = 0;
                    if (item.Roles == "Retailer")
                    {
                        if (item.Trans_type.Contains("IMPS_VERIFY"))
                        {

                            debit1 = Convert.ToDecimal(item.rem_comm);
                        }
                        else
                        {
                            amount1 = Convert.ToDecimal(item.amount);
                            usercomm = Convert.ToDecimal(item.rem_comm);
                            debit1 = amount1 + usercomm;

                        }
                    }
                    if (item.Roles == "API")
                    {
                        if (item.Trans_type.Contains("IMPS_VERIFY"))
                        {

                            debit1 = Convert.ToDecimal(item.api_comm);
                        }
                        else
                        {
                            amount1 = Convert.ToDecimal(item.amount);
                            usercomm = Convert.ToDecimal(item.api_comm);
                            debit1 = amount1 + usercomm;

                        }
                    }
                    if (item.Roles == "Whitelabelretailer")
                    {
                        if (item.Trans_type.Contains("IMPS_VERIFY"))
                        {

                            debit1 = Convert.ToDecimal(item.whitelabel_comm);
                        }
                        else
                        {
                            amount1 = Convert.ToDecimal(item.amount);
                            usercomm = Convert.ToDecimal(item.whitelabel_comm);
                            debit1 = amount1 + usercomm;

                        }
                    }

                    /********************************************************************/
                    var dbt = debit1;
                    var sts = item.status;
                    if (item.status.ToUpper().Contains("REQ"))
                    {
                        sts = "Pending";
                    }
                    dataTbl.Rows.Add(sts + "=>" + item.frm_name, item.Trans_type, item.senderno, item.accountno, item.amount, item.user_remain_pre, dbt, item.user_remain, item.Api_remain_post, item.trans_time, item.bank_trans_id, item.trans_id, item.bank_nm,
                    item.recivername, item.md_remain_pre, item.md_income, item.md_remain, item.dlm_remain_pre, item.dlm_income, item.dlm_remain, item.ifsccode, item.ipaddress);
                }

            }
            else
            {
                dataTbl.Rows.Add("", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "");
            }

            var grid = new GridView();
            grid.DataSource = dataTbl;
            grid.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=Money_Transfer_Report.xls");
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
        [PermissioncheckingAttribute(servicename = "DMT", permision = "Read")]
        public ActionResult PDFMoneyReport(string txt_frm_date, string txt_to_date, string ddlusers, string ddl_status, string allmaster, string alldealer, string allretailer, string allapiuser, string Whitelabel, string ddl_Type)
        {
            if (txt_frm_date == "")
            {
                return RedirectToAction("Money_Transfer_Report", "Home");

            }
            if (txt_frm_date == null && txt_to_date == null && ddl_status == null)
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


            var userid = User.Identity.GetUserId();
            if (ddlusers == "Master")
            {
                if (allmaster == "" || allmaster.Contains("allmaster") || allmaster == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = allmaster;
                }
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

            if (ddlusers == "WAdmin")
            {
                if (Whitelabel == "" || Whitelabel.Contains("Whitelabel") || Whitelabel == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = Whitelabel;
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

            var respo = db.money_transfer_report_paging(ddlusers, userid, ddl_status, "VASTWEB", Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), ddl_Type, 1, 10000000).ToList();
            return new ViewAsPdf(respo);
        }
        [PermissioncheckingAttribute(servicename = "DMT", permision = "Write")]
        public ActionResult EditDMToperator(int txtidno, string txtoptid)
        {
            var chk = db.IMPS_transtion_detsils.Where(aa => aa.idno == txtidno).SingleOrDefault();
            if (chk == null)
            {
                var chk1 = db.IMPS_transtion_detsils_old.Where(aa => aa.idno == txtidno).SingleOrDefault();
                if (chk1 != null)
                {
                    chk1.bank_trans_id = txtoptid;
                    db.SaveChanges();
                }
            }
            if (chk != null)
            {
                chk.bank_trans_id = txtoptid;
                db.SaveChanges();
            }

            return Json("", JsonRequestBehavior.AllowGet);
        }
        [PermissioncheckingAttribute(servicename = "DMT", permision = "Read")]
        public ActionResult FindMoneyTotal(string txt_frm_date, string txt_to_date, string ddlusers, string ddl_status, string ddl_Type, string txtmob, string allmaster, string alldealer, string allretailer, string allapiuser)
        {
            var userchk = "";
            var mobile = "";
            var ddlst = "";
            string userid = User.Identity.GetUserId();
            if (txt_frm_date == null && txt_to_date == null && ddl_status == null && txtmob == null)
            {
                txt_frm_date = DateTime.Now.ToString();
                txt_to_date = DateTime.Now.ToString();
                ddl_status = "ALL"; txtmob = "ALL";

            }
            if (ddl_status == "" || ddl_status.Contains("ALL STATUS"))
            {
                ddlst = "ALL";
            }
            else
            {
                ddlst = ddl_status;
            }

            if (txtmob == "" || txtmob == null)
            {
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
            int pagesize = 20;
            var role = "";
            if (ddl_status.ToUpper() == "PENDING")
            {
                ddl_status = "REQ";
            }
            if (ddlusers == null || ddlusers == "")
            {
                ddlusers = "Admin";

            }
            if (ddlusers == "Master")
            {
                userchk = allmaster;
            }
            if (ddlusers == "Dealer")
            {
                userchk = alldealer;
            }
            if (ddlusers == "Retailer")
            {
                userchk = allretailer;
            }
            if (ddlusers == "APIID")
            {
                userchk = allapiuser;
            }
            if (ddl_status == "ALL")
            {
                ddl_status = "";
            }
            var chk = db.Total_Money_Transfer(Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), ddl_status, ddl_Type, ddlusers, userchk).ToList();

            var successtotal1 = chk.Where(a => a.Status.ToUpper() == "SUCCESS" && a.Trans_Type == "IMPS_VERIFY").Count();
            var successtotal = chk.Where(a => a.Status.ToUpper() == "SUCCESS").Sum(a => a.amount);
            var Failedtotal = chk.Where(aa => aa.Status.ToUpper() == "FAILED").Sum(aa => aa.amount);
            var pendingtotal = chk.Where(aa => aa.Status.ToUpper() == "PENDING").Sum(aa => aa.amount);
            var data = new
            {
                success = successtotal + successtotal1,
                failed = Failedtotal,
                pending = pendingtotal
            };
            return Json(data, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        [PermissioncheckingAttribute(servicename = "DMT", permision = "Read")]
        public ActionResult Money_Tranfer_Details_View(int Idno)
        {
            var details = db.Money_Tranfer_Details_View(Idno);
            return Json(details, JsonRequestBehavior.AllowGet);
        }
        [PermissioncheckingAttribute(servicename = "DMT", permision = "Read")]
        public ActionResult AdminImpsIncmoe(string Type, string txt_frm_date, string txt_to_date)
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

            var chk = db.DMT_admin_income(Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            var Adminincome = chk.SingleOrDefault().total;
            //var Tds = chk.SingleOrDefault().TDS;
            var Gst = chk.SingleOrDefault().GST;
            return Json(new { Type = Type, Adminincome = Adminincome, Gst = Gst });
        }
        [PermissioncheckingAttribute(servicename = "DMT", permision = "Read")]
        public ActionResult CountImpasandVerify(string Type, string txt_frm_date, string txt_to_date)
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
            var ch = db.money_transfer_report_paging("Admin", "", "ALL", "VASTWEB", Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), "ALL", 1, 1000000).ToList();
            var Impscount = ch.Where(s => s.status.ToUpper().Contains("SUCCESS") && s.Trans_type.ToUpper() == "IMPS").Count();
            var VerifyCount = ch.Where(s => s.status.ToUpper().Contains("SUCCESS") && s.Trans_type.ToUpper() == "IMPS_VERIFY").Count();
            return Json(new { Type = Type, Impscount = Impscount, VerifyCount = VerifyCount });
        }

        #endregion Imps Report

        //Manual success money transfer
        [PermissioncheckingAttribute(servicename = "DMT", permision = "Write")]
        public ActionResult Moneysucess(string trans_id, string bank_trans_id, string recivername, string currentstatus)
        {
            try
            {
                var RetailerEmail = "";
                var AdminEmail = db.Admin_details.Single().email;
                var comm = db.IMPS_transtion_detsils.Where(p => p.trans_id == trans_id).SingleOrDefault();
                if (comm != null)
                {
                    var retailerid = comm.rch_from;
                    var bankname = comm.bank_nm;
                    var transamount = comm.amount;
                    var bank_account = comm.accountno;
                    var RetailerMobile = "";
                    var userroleid = db.UserRoles.Where(aa => aa.UserId == retailerid).SingleOrDefault().RoleId;
                    var userrole = db.Roles.Where(aa => aa.RoleId == userroleid).SingleOrDefault().Name;
                    if (userrole == "Retailer")
                    {
                        RetailerEmail = db.Retailer_Details.Where(a => a.RetailerId == retailerid).Single().Email;
                        RetailerMobile = db.Retailer_Details.Where(aa => aa.RetailerId == retailerid).Single().Mobile;
                    }
                    else if (userrole == "Whitelabelretailer")
                    {
                        RetailerEmail = db.Whitelabel_Retailer_Details.Where(a => a.RetailerId == retailerid).Single().Email;
                        RetailerMobile = db.Whitelabel_Retailer_Details.Where(aa => aa.RetailerId == retailerid).Single().Mobile;
                    }
                    else if (userrole == "API")
                    {
                        RetailerEmail = db.api_user_details.Where(a => a.apiid == retailerid).Single().emailid;
                        RetailerMobile = db.api_user_details.Where(aa => aa.apiid == retailerid).Single().mobile;
                    }
                    var statusAdmin = db.PushNotificationStatus.Where(a => a.UserRole == "Admin").SingleOrDefault().Status;
                    var statusRetailer = db.PushNotificationStatus.Where(a => a.UserRole == "Retailer").SingleOrDefault().Status;
                    var StatusSendSmsMoneyTransferFailedToSuccess = db.SMSSendAlls.Where(a => a.ServiceName == "dmtfailtosucconline").SingleOrDefault();
                    var StatusSendEmailMoneyTransferFailedToSuccess = db.EmailSendAlls.Where(a => a.ServiceName == "dmtfailtosucconline1").SingleOrDefault().Status;

                    if (currentstatus.ToUpper() == "FAILED")
                    {
                        db.Money_transfer_failed_to_success(trans_id);
                        if (statusAdmin == "Y")
                        {
                            SendPushNotification(AdminEmail, Url.Action("Money_transfer_Report", "Home"), "Money Transfer in Bank " + bankname + " and Account Number " + bank_account + "and Bank Refernce Id " + bank_trans_id + " and Amount " + transamount + " is transfer Successfully.", "Money Transfer Failed To Success ..");
                        }
                        if (statusRetailer == "Y")
                        {
                            SendPushNotification(RetailerEmail, Url.Action("Money_Transfer_Report", "Home"), "Money Transfer in Bank " + bankname + " and Account Number " + bank_account + "and Bank Refernce Id " + bank_trans_id + " and Amount " + transamount + " is transfer Successfully.", "Money Transfer Failed To Success ..");
                        }
                        //if (StatusSendSmsMoneyTransferFailedToSuccess == "Y")
                        //{
                        //    string msgssss = "";
                        //    string tempid = "";
                        //    string urlss = "";

                        //    var smsapionsts = db.apisms.Where(x => x.sts == "Y").SingleOrDefault();
                        //    var smsstypes = db.Sending_SMS_Templates.Where(x => x.SMS_TYPE == "MONEYTRANSFERMANUALMONEYSUCCESS" && x.SMSAPIID == smsapionsts.id).SingleOrDefault();
                        //    if (smsstypes != null)
                        //    {
                        //        msgssss = string.Format(smsstypes.Templates, bankname, bank_account, bank_trans_id, transamount);
                        //        tempid = smsstypes.Templateid;
                        //        urlss = smsapionsts.smsapi;
                        //        smssend.sendsmsallnew(RetailerMobile, msgssss, urlss, tempid);
                        //    }
                        //    //  smssend.sendsmsall(RetailerMobile, "Money Transfer in Bank " + bankname + " and Account Number " + bank_account + "and Bank Refernce Id " + bank_trans_id + " and Amount : " + transamount + " is transfer Successfully.", "Money Transfer");
                        //}

                        smssend.sms_init(StatusSendSmsMoneyTransferFailedToSuccess.Status, StatusSendSmsMoneyTransferFailedToSuccess.Whatsapp_Status, "MONEYTRANSFERMANUALMONEYSUCCESS", RetailerMobile, bankname, bank_account + " ", bank_trans_id, transamount);

                        if (StatusSendEmailMoneyTransferFailedToSuccess == "Y")
                        {
                            smssend.SendEmailAll(RetailerEmail, "Money Transfer in Bank " + bankname + " and Account Number " + bank_account + "and Bank Refernce Id " + bank_trans_id + " and Amount : " + transamount + " is transfer Successfully.", "Money Transfer", AdminEmail);
                        }
                        notify.sendmessage(RetailerEmail, "Money Transfer in Bank " + bankname + " and Account Number " + bank_account + "and Bank Refernce Id " + bank_trans_id + " and Amount " + transamount + " is transfer Successfully.");
                        return Json(new { Status = true, Message = "Successfully" });
                    }
                    else
                    {
                        db.Money_transfer_update_new_new(trans_id, "SUCCESS", bank_trans_id, recivername, "Manual SUCCESS", "", 0, 0);
                        if (statusAdmin == "Y")
                        {
                            SendPushNotification(AdminEmail, Url.Action("Money_transfer_Report", "Home"), "Money Transfer in Bank " + bankname + " and Account Number " + bank_account + "and Bank Refernce Id " + bank_trans_id + " and Amount " + transamount + " is transfer Successfully.", "Money Transfer Manual Success ..");
                        }
                        if (statusRetailer == "Y")
                        {
                            SendPushNotification(RetailerEmail, Url.Action("Money_Transfer_Report", "Home"), "Money Transfer in Bank " + bankname + " and Account Number " + bank_account + "and Bank Refernce Id " + bank_trans_id + " and Amount " + transamount + " is transfer Successfully.", "Money Transfer Manual Success ..");
                        }
                        //if (StatusSendSmsMoneyTransferFailedToSuccess == "Y")
                        //{
                        //    string msgssss = "";
                        //    string tempid = "";
                        //    string urlss = "";

                        //    var smsapionsts = db.apisms.Where(x => x.sts == "Y").SingleOrDefault();
                        //    var smsstypes = db.Sending_SMS_Templates.Where(x => x.SMS_TYPE == "MONEYTRANSFERMANUALMONEYSUCCESS" && x.SMSAPIID == smsapionsts.id).SingleOrDefault();
                        //    if (smsstypes != null)
                        //    {
                        //        msgssss = string.Format(smsstypes.Templates, bankname, bank_account, bank_trans_id, transamount);
                        //        tempid = smsstypes.Templateid;
                        //        urlss = smsapionsts.smsapi;

                        //        smssend.sendsmsallnew(RetailerMobile, msgssss, urlss, tempid);
                        //    }
                        //    //  smssend.sendsmsall(RetailerMobile, "Money Transfer in Bank " + bankname + " and Account Number " + bank_account + "and Bank Refernce Id " + bank_trans_id + " and Amount : " + transamount + " is transfer Successfully.", "Money Transfer");
                        //}

                        smssend.sms_init(StatusSendSmsMoneyTransferFailedToSuccess.Status, StatusSendSmsMoneyTransferFailedToSuccess.Whatsapp_Status, "MONEYTRANSFERMANUALMONEYSUCCESS", RetailerMobile, bankname, bank_account + " ", bank_trans_id, transamount);

                        if (StatusSendEmailMoneyTransferFailedToSuccess == "Y")
                        {
                            smssend.SendEmailAll(RetailerEmail, "Money Transfer in Bank " + bankname + " and Account Number " + bank_account + "and Bank Refernce Id " + bank_trans_id + " and Amount : " + transamount + " is transfer Successfully.", "Money Transfer", AdminEmail);
                        }
                        notify.sendmessage(RetailerEmail, "Money Transfer in Bank " + bankname + " and Account Number " + bank_account + "and Bank Refernce Id " + bank_trans_id + " and Amount " + transamount + " is transfer Successfully.");
                        return Json(new { Status = true, Message = "Successfully" });
                    }
                }
                else
                {
                    var comm1 = db.IMPS_transtion_detsils_old.Where(p => p.trans_id == trans_id).SingleOrDefault();
                    var retailerid = comm1.rch_from;
                    var bankname = comm1.bank_nm;
                    var transamount = comm1.amount;
                    var bank_account = comm1.accountno;
                    var RetailerMobile = "";
                    var userroleid = db.UserRoles.Where(aa => aa.UserId == retailerid).SingleOrDefault().RoleId;
                    var userrole = db.Roles.Where(aa => aa.RoleId == userroleid).SingleOrDefault().Name;
                    if (userrole == "Retailer")
                    {
                        RetailerEmail = db.Retailer_Details.Where(a => a.RetailerId == retailerid).Single().Email;
                        RetailerMobile = db.Retailer_Details.Where(aa => aa.RetailerId == retailerid).Single().Mobile;
                    }
                    else if (userrole == "Whitelabelretailer")
                    {
                        RetailerEmail = db.Whitelabel_Retailer_Details.Where(a => a.RetailerId == retailerid).Single().Email;
                        RetailerMobile = db.Whitelabel_Retailer_Details.Where(aa => aa.RetailerId == retailerid).Single().Mobile;
                    }
                    else if (userrole == "API")
                    {
                        RetailerEmail = db.api_user_details.Where(a => a.apiid == retailerid).Single().emailid;
                        RetailerMobile = db.api_user_details.Where(aa => aa.apiid == retailerid).Single().mobile;
                    }
                    var statusAdmin = db.PushNotificationStatus.Where(a => a.UserRole == "Admin").SingleOrDefault().Status;
                    var statusRetailer = db.PushNotificationStatus.Where(a => a.UserRole == "Retailer").SingleOrDefault().Status;
                    var StatusSendSmsMoneyTransferFailedToSuccess = db.SMSSendAlls.Where(a => a.ServiceName == "dmtfailtosucconline").SingleOrDefault();
                    var StatusSendEmailMoneyTransferFailedToSuccess = db.EmailSendAlls.Where(a => a.ServiceName == "dmtfailtosucconline1").SingleOrDefault().Status;

                    if (currentstatus.ToUpper() == "FAILED")
                    {
                        db.Money_transfer_failed_to_success_old(trans_id);
                        if (statusAdmin == "Y")
                        {
                            SendPushNotification(AdminEmail, Url.Action("Money_transfer_Report", "Home"), "Money Transfer in Bank " + bankname + " and Account Number " + bank_account + "and Bank Refernce Id " + bank_trans_id + " and Amount " + transamount + " is transfer Successfully.", "Money Transfer Failed To Success ..");
                        }
                        if (statusRetailer == "Y")
                        {
                            SendPushNotification(RetailerEmail, Url.Action("Money_Transfer_Report", "Home"), "Money Transfer in Bank " + bankname + " and Account Number " + bank_account + "and Bank Refernce Id " + bank_trans_id + " and Amount " + transamount + " is transfer Successfully.", "Money Transfer Failed To Success ..");
                        }
                        //if (StatusSendSmsMoneyTransferFailedToSuccess == "Y")
                        //{

                        //    string msgssss = "";
                        //    string tempid = "";
                        //    string urlss = "";

                        //    var smsapionsts = db.apisms.Where(x => x.sts == "Y").SingleOrDefault();
                        //    var smsstypes = db.Sending_SMS_Templates.Where(x => x.SMS_TYPE == "MONEYTRANSFERMANUALMONEYSUCCESS" && x.SMSAPIID == smsapionsts.id).SingleOrDefault();
                        //    if (smsstypes != null)
                        //    {
                        //        msgssss = string.Format(smsstypes.Templates, bankname, bank_account, bank_trans_id, transamount);
                        //        tempid = smsstypes.Templateid;
                        //        urlss = smsapionsts.smsapi;
                        //        smssend.sendsmsallnew(RetailerMobile, msgssss, urlss, tempid);
                        //    }
                        //    //  smssend.sendsmsall(RetailerMobile, "Money Transfer in Bank " + bankname + " and Account Number " + bank_account + "and Bank Refernce Id " + bank_trans_id + " and Amount : " + transamount + " is transfer Successfully.", "Money Transfer");
                        //}

                        smssend.sms_init(StatusSendSmsMoneyTransferFailedToSuccess.Status, StatusSendSmsMoneyTransferFailedToSuccess.Whatsapp_Status, "MONEYTRANSFERMANUALMONEYSUCCESS", RetailerMobile, bankname, bank_account + " ", bank_trans_id, transamount);

                        if (StatusSendEmailMoneyTransferFailedToSuccess == "Y")
                        {
                            smssend.SendEmailAll(RetailerEmail, "Money Transfer in Bank " + bankname + " and Account Number " + bank_account + "and Bank Refernce Id " + bank_trans_id + " and Amount : " + transamount + " is transfer Successfully.", "Money Transfer", AdminEmail);
                        }
                        notify.sendmessage(RetailerEmail, "Money Transfer in Bank " + bankname + " and Account Number " + bank_account + "and Bank Refernce Id " + bank_trans_id + " and Amount " + transamount + " is transfer Successfully.");
                        return Json(new { Status = true, Message = "Successfully" });
                    }
                    else
                    {
                        db.Money_transfer_update_new_new_old(trans_id, "SUCCESS", bank_trans_id, recivername, "Manual SUCCESS", "", 0, 0);
                        if (statusAdmin == "Y")
                        {
                            SendPushNotification(AdminEmail, Url.Action("Money_transfer_Report", "Home"), "Money Transfer in Bank " + bankname + " and Account Number " + bank_account + "and Bank Refernce Id " + bank_trans_id + " and Amount " + transamount + " is transfer Successfully.", "Money Transfer Manual Success ..");
                        }
                        if (statusRetailer == "Y")
                        {
                            SendPushNotification(RetailerEmail, Url.Action("Money_Transfer_Report", "Home"), "Money Transfer in Bank " + bankname + " and Account Number " + bank_account + "and Bank Refernce Id " + bank_trans_id + " and Amount " + transamount + " is transfer Successfully.", "Money Transfer Manual Success ..");
                        }
                        //if (StatusSendSmsMoneyTransferFailedToSuccess == "Y")
                        //{

                        //    string msgssss = "";
                        //    string tempid = "";
                        //    string urlss = "";

                        //    var smsapionsts = db.apisms.Where(x => x.sts == "Y").SingleOrDefault();
                        //    var smsstypes = db.Sending_SMS_Templates.Where(x => x.SMS_TYPE == "MONEYTRANSFERMANUALMONEYSUCCESS" && x.SMSAPIID == smsapionsts.id).SingleOrDefault();
                        //    if (smsstypes != null)
                        //    {
                        //        msgssss = string.Format(smsstypes.Templates, bankname, bank_account, bank_trans_id, transamount);
                        //        tempid = smsstypes.Templateid;
                        //        urlss = smsapionsts.smsapi;
                        //        smssend.sendsmsallnew(RetailerMobile, msgssss, urlss, tempid);
                        //    }
                        //    // smssend.sendsmsall(RetailerMobile, "Money Transfer in Bank " + bankname + " and Account Number " + bank_account + "and Bank Refernce Id " + bank_trans_id + " and Amount : " + transamount + " is transfer Successfully.", "Money Transfer");
                        //}

                        smssend.sms_init(StatusSendSmsMoneyTransferFailedToSuccess.Status, StatusSendSmsMoneyTransferFailedToSuccess.Whatsapp_Status, "MONEYTRANSFERMANUALMONEYSUCCESS", RetailerMobile, bankname, bank_account + " ", bank_trans_id, transamount);

                        if (StatusSendEmailMoneyTransferFailedToSuccess == "Y")
                        {
                            smssend.SendEmailAll(RetailerEmail, "Money Transfer in Bank " + bankname + " and Account Number " + bank_account + "and Bank Refernce Id " + bank_trans_id + " and Amount : " + transamount + " is transfer Successfully.", "Money Transfer", AdminEmail);
                        }
                        notify.sendmessage(RetailerEmail, "Money Transfer in Bank " + bankname + " and Account Number " + bank_account + "and Bank Refernce Id " + bank_trans_id + " and Amount " + transamount + " is transfer Successfully.");
                        return Json(new { Status = true, Message = "Successfully" });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, Message = ex.Message });
            }
        }
        //Manual Failed Money Transfer
        [PermissioncheckingAttribute(servicename = "DMT", permision = "Write")]
        public ActionResult MoneyFailed(string txtrefidno, string bankid, string recivername, string ddl_refund, string secuirtypass, string currentstatus)
        {
            try
            {
                var RetailerEmail = "";
                var AdminEmail = db.Admin_details.Single().email;
                var comm = db.IMPS_transtion_detsils.Where(p => p.trans_id == txtrefidno).SingleOrDefault();
                if (comm != null)
                {
                    var retailerid = comm.rch_from;
                    var bankname = comm.bank_nm;
                    var transamount = comm.amount;
                    var bank_account = comm.accountno;
                    var RetailerMobile = "";
                    var userroleid = db.UserRoles.Where(aa => aa.UserId == retailerid).SingleOrDefault().RoleId;
                    var userrole = db.Roles.Where(aa => aa.RoleId == userroleid).SingleOrDefault().Name;
                    if (userrole == "Retailer")
                    {
                        RetailerEmail = db.Retailer_Details.Where(a => a.RetailerId == retailerid).Single().Email;
                        RetailerMobile = db.Retailer_Details.Where(aa => aa.RetailerId == retailerid).Single().Mobile;
                    }
                    else if (userrole == "Whitelabelretailer")
                    {
                        RetailerEmail = db.Whitelabel_Retailer_Details.Where(a => a.RetailerId == retailerid).Single().Email;
                        RetailerMobile = db.Whitelabel_Retailer_Details.Where(aa => aa.RetailerId == retailerid).Single().Mobile;
                    }
                    else if (userrole == "API")
                    {
                        RetailerEmail = db.api_user_details.Where(a => a.apiid == retailerid).Single().emailid;
                        RetailerMobile = db.api_user_details.Where(aa => aa.apiid == retailerid).Single().mobile;
                    }

                    var password = Encrypt(secuirtypass);
                    var tranpass = (from paa in db.admin_new_pass where paa.securitypass == password select paa).Count();
                    if (tranpass > 0)
                    {
                        var statusAdmin = db.PushNotificationStatus.Where(a => a.UserRole == "Admin").SingleOrDefault().Status;
                        var statusRetailer = db.PushNotificationStatus.Where(a => a.UserRole == "Retailer").SingleOrDefault().Status;
                        var StatusSendSmsMoneyTransferSuccesstoFailed = db.SMSSendAlls.Where(a => a.ServiceName == "dmtsucctofailonline").SingleOrDefault();
                        var StatusSendEmailMoneyTransferSuccesstoFailed = db.EmailSendAlls.Where(a => a.ServiceName == "dmtsucctofailonline1").SingleOrDefault().Status;
                        if (currentstatus.ToUpper() == "SUCCESS")
                        {
                            if (comm.DmtType == "DMT1")
                            {
                                db.Money_transfer_success_to_failed(txtrefidno);
                            }
                            else
                            {
                                db.Money_transfer_update_by_paytm_success_to_failed(txtrefidno);
                            }
                            if (statusAdmin == "Y")
                            {
                                SendPushNotification(AdminEmail, Url.Action("Money_transfer_Report", "Home"), "Money Transfer in Bank " + bankname + " and Account Number " + bank_account + "and Bank Refernce Id " + bankid + " and Amount " + transamount + " is Failed.", "Money Transfer Success To Failed ..");
                            }
                            if (statusRetailer == "Y")
                            {
                                SendPushNotification(RetailerEmail, Url.Action("Money_Transfer_Report", "Home"), "Money Transfer in Bank " + bankname + " and Account Number " + bank_account + "and Bank Refernce Id " + bankid + " and Amount " + transamount + " is Failed .", "Money Transfer Success To Failed ..");
                            }
                            //if (StatusSendSmsMoneyTransferSuccesstoFailed == "Y")
                            //{
                            //    string msgssss = "";
                            //    string tempid = "";
                            //    string urlss = "";

                            //    var smsapionsts = db.apisms.Where(x => x.sts == "Y").SingleOrDefault();
                            //    var smsstypes = db.Sending_SMS_Templates.Where(x => x.SMS_TYPE == "MONEYTRANSFERMANUALMONEYFAILED" && x.SMSAPIID == smsapionsts.id).SingleOrDefault();
                            //    if (smsstypes != null)
                            //    {
                            //        msgssss = string.Format(smsstypes.Templates, bankname, bank_account, transamount);
                            //        tempid = smsstypes.Templateid;
                            //        urlss = smsapionsts.smsapi;

                            //        smssend.sendsmsallnew(RetailerMobile, msgssss, urlss, tempid);
                            //    }
                            //    //  smssend.sendsmsall(RetailerMobile, "Money Transfer in Bank " + bankname + " and Account Number " + bank_account + "and Amount" + transamount + " is Failed .", "Recharge");
                            //}

                            smssend.sms_init(StatusSendSmsMoneyTransferSuccesstoFailed.Status, StatusSendSmsMoneyTransferSuccesstoFailed.Whatsapp_Status, "MONEYTRANSFERMANUALMONEYFAILED", RetailerMobile, bankname + " ", bank_account + " ", transamount + " ");

                            if (StatusSendEmailMoneyTransferSuccesstoFailed == "Y")
                            {
                                smssend.SendEmailAll(RetailerEmail, "Money Transfer in Bank " + bankname + " and Account Number " + bank_account + "and Amount" + transamount + " is Failed .", "Money Transfer", AdminEmail);
                            }
                            notify.sendmessage(RetailerEmail, "Money Transfer in Bank " + bankname + " and Account Number " + bank_account + "and Bank Refernce Id " + bankid + " and Amount " + transamount + " is Failed .");
                            return Json(new { Status = true, Message = "Successfully" });
                        }
                        else
                        {
                            if (comm.DmtType == "DMT1")
                            {
                                db.Money_transfer_update_new_new(txtrefidno, "FAILED", bankid, recivername, ddl_refund, "", 0, 0);
                            }
                            else
                            {
                                db.Money_transfer_update_by_paytm(txtrefidno, "FAILED", bankid, recivername, ddl_refund, "", 0, 0);
                            }
                            if (statusAdmin == "Y")
                            {
                                SendPushNotification(AdminEmail, Url.Action("Money_transfer_Report", "Home"), "Money Transfer in Bank " + bankname + " and Account Number " + bank_account + "and Bank Refernce Id " + bankid + " and Amount " + transamount + " is Failed Due To " + ddl_refund + ".", "Money Transfer Failed ..");
                            }
                            if (statusRetailer == "Y")
                            {
                                SendPushNotification(RetailerEmail, Url.Action("Money_Transfer_Report", "Home"), "Money Transfer in Bank " + bankname + " and Account Number " + bank_account + "and Bank Refernce Id " + bankid + " and Amount " + transamount + " is Failed Due To " + ddl_refund + ".", "Money Transfer Failed ..");
                            }
                            //if (StatusSendSmsMoneyTransferSuccesstoFailed == "Y")
                            //{
                            //    string msgssss = "";
                            //    string tempid = "";
                            //    string urlss = "";

                            //    var smsapionsts = db.apisms.Where(x => x.sts == "Y").SingleOrDefault();
                            //    var smsstypes = db.Sending_SMS_Templates.Where(x => x.SMS_TYPE == "MONEYTRANSFERMANUALMONEYFAILEDDUETO" && x.SMSAPIID == smsapionsts.id).SingleOrDefault();
                            //    if (smsstypes != null)
                            //    {
                            //        msgssss = string.Format(smsstypes.Templates, bankname, bank_account, transamount, ddl_refund);
                            //        tempid = smsstypes.Templateid;
                            //        urlss = smsapionsts.smsapi;

                            //        smssend.sendsmsallnew(RetailerMobile, msgssss, urlss, tempid);
                            //    }

                            //    // smssend.sendsmsall(RetailerMobile, "Money Transfer in Bank " + bankname + " and Account Number " + bank_account + "and Amount" + transamount + " is Failed Due To " + ddl_refund + ".", "Money Transfer");
                            //}

                            smssend.sms_init(StatusSendSmsMoneyTransferSuccesstoFailed.Status, StatusSendSmsMoneyTransferSuccesstoFailed.Whatsapp_Status, "MONEYTRANSFERMANUALMONEYFAILEDDUETO", RetailerMobile, bankname, bank_account + " ", " " + transamount, ddl_refund);

                            if (StatusSendEmailMoneyTransferSuccesstoFailed == "Y")
                            {
                                smssend.SendEmailAll(RetailerEmail, "Money Transfer in Bank " + bankname + " and Account Number " + bank_account + "and Amount" + transamount + " is Failed Due To " + ddl_refund + ".", "Money Transfer", AdminEmail);
                            }
                            notify.sendmessage(RetailerEmail, "Money Transfer in Bank " + bankname + " and Account Number " + bank_account + "and Bank Refernce Id " + bankid + " and Amount " + transamount + " is Failed .");

                            return Json(new { Status = true, Message = "Successfully" });
                        }
                    }
                    else
                    {
                        return Json(new { Status = false, Message = "Your Security Password is Wrong!" });
                    }
                    /****************************************************************************************************************************************************/
                }
                else
                {
                    var comm1 = db.IMPS_transtion_detsils_old.Where(p => p.trans_id == txtrefidno).SingleOrDefault();
                    var retailerid = comm1.rch_from;
                    var bankname = comm1.bank_nm;
                    var transamount = comm1.amount;
                    var bank_account = comm1.accountno;
                    var RetailerMobile = "";
                    var userroleid = db.UserRoles.Where(aa => aa.UserId == retailerid).SingleOrDefault().RoleId;
                    var userrole = db.Roles.Where(aa => aa.RoleId == userroleid).SingleOrDefault().Name;
                    if (userrole == "Retailer")
                    {
                        RetailerEmail = db.Retailer_Details.Where(a => a.RetailerId == retailerid).Single().Email;
                        RetailerMobile = db.Retailer_Details.Where(aa => aa.RetailerId == retailerid).Single().Mobile;
                    }
                    else if (userrole == "Whitelabelretailer")
                    {
                        RetailerEmail = db.Whitelabel_Retailer_Details.Where(a => a.RetailerId == retailerid).Single().Email;
                        RetailerMobile = db.Whitelabel_Retailer_Details.Where(aa => aa.RetailerId == retailerid).Single().Mobile;
                    }
                    else if (userrole == "API")
                    {
                        RetailerEmail = db.api_user_details.Where(a => a.apiid == retailerid).Single().emailid;
                        RetailerMobile = db.api_user_details.Where(aa => aa.apiid == retailerid).Single().mobile;
                    }

                    var password = Encrypt(secuirtypass);
                    var tranpass = (from paa in db.admin_new_pass where paa.securitypass == password select paa).Count();
                    if (tranpass > 0)
                    {
                        var statusAdmin = db.PushNotificationStatus.Where(a => a.UserRole == "Admin").SingleOrDefault().Status;
                        var statusRetailer = db.PushNotificationStatus.Where(a => a.UserRole == "Retailer").SingleOrDefault().Status;
                        var StatusSendSmsMoneyTransferSuccesstoFailed = db.SMSSendAlls.Where(a => a.ServiceName == "dmtsucctofailonline").SingleOrDefault();
                        var StatusSendEmailMoneyTransferSuccesstoFailed = db.EmailSendAlls.Where(a => a.ServiceName == "dmtsucctofailonline1").SingleOrDefault().Status;

                        if (currentstatus.ToUpper() == "SUCCESS")
                        {
                            if (comm1.DmtType == "DMT1")
                            {
                                db.Money_transfer_success_to_failed_old(txtrefidno);
                            }
                            else
                            {
                                db.Money_transfer_update_by_paytm_success_to_failed_old(txtrefidno);
                            }
                            if (statusAdmin == "Y")
                            {
                                SendPushNotification(AdminEmail, Url.Action("Money_transfer_Report", "Home"), "Money Transfer in Bank " + bankname + " and Account Number " + bank_account + "and Bank Refernce Id " + bankid + " and Amount " + transamount + " is Failed.", "Money Transfer Success To Failed ..");
                            }
                            if (statusRetailer == "Y")
                            {
                                SendPushNotification(RetailerEmail, Url.Action("Money_Transfer_Report", "Home"), "Money Transfer in Bank " + bankname + " and Account Number " + bank_account + "and Bank Refernce Id " + bankid + " and Amount " + transamount + " is Failed .", "Money Transfer Success To Failed ..");
                            }
                            //if (StatusSendSmsMoneyTransferSuccesstoFailed == "Y")
                            //{
                            //    string msgssss = "";
                            //    string tempid = "";
                            //    string urlss = "";

                            //    var smsapionsts = db.apisms.Where(x => x.sts == "Y").SingleOrDefault();
                            //    var smsstypes = db.Sending_SMS_Templates.Where(x => x.SMS_TYPE == "MONEYTRANSFERMANUALMONEYFAILED" && x.SMSAPIID == smsapionsts.id).SingleOrDefault();
                            //    if (smsstypes != null)
                            //    {
                            //        msgssss = string.Format(smsstypes.Templates, bankname, bank_account, transamount);
                            //        tempid = smsstypes.Templateid;
                            //        urlss = smsapionsts.smsapi;

                            //        smssend.sendsmsallnew(RetailerMobile, msgssss, urlss, tempid);
                            //    }

                            //    //  smssend.sendsmsall(RetailerMobile, "Money Transfer in Bank " + bankname + " and Account Number " + bank_account + "and Amount" + transamount + " is Failed .", "Money Transfer");
                            //}

                            smssend.sms_init(StatusSendSmsMoneyTransferSuccesstoFailed.Status, StatusSendSmsMoneyTransferSuccesstoFailed.Whatsapp_Status, "MONEYTRANSFERMANUALMONEYFAILED", RetailerMobile, bankname + " ", bank_account + " ", transamount + " ");

                            if (StatusSendEmailMoneyTransferSuccesstoFailed == "Y")
                            {
                                smssend.SendEmailAll(RetailerEmail, "Money Transfer in Bank " + bankname + " and Account Number " + bank_account + "and Amount" + transamount + " is Failed .", "Money Transfer", AdminEmail);
                            }
                            notify.sendmessage(RetailerEmail, "Money Transfer in Bank " + bankname + " and Account Number " + bank_account + "and Bank Refernce Id " + bankid + " and Amount " + transamount + " is Failed .");
                            return Json(new { Status = true, Message = "Successfully" });
                        }
                        else
                        {
                            if (comm1.DmtType == "DMT1")
                            {
                                db.Money_transfer_update_new_new_old(txtrefidno, "FAILED", bankid, recivername, ddl_refund, "", 0, 0);
                            }
                            else
                            {
                                db.Money_transfer_update_by_paytm_old(txtrefidno, "FAILED", bankid, recivername, ddl_refund, "", 0, 0);
                            }

                            if (statusAdmin == "Y")
                            {
                                SendPushNotification(AdminEmail, Url.Action("Money_transfer_Report", "Home"), "Money Transfer in Bank " + bankname + " and Account Number " + bank_account + "and Bank Refernce Id " + bankid + " and Amount " + transamount + " is Failed Due To " + ddl_refund + ".", "Money Transfer Failed ..");
                            }
                            if (statusRetailer == "Y")
                            {
                                SendPushNotification(RetailerEmail, Url.Action("Money_Transfer_Report", "Home"), "Money Transfer in Bank " + bankname + " and Account Number " + bank_account + "and Bank Refernce Id " + bankid + " and Amount " + transamount + " is Failed Due To " + ddl_refund + ".", "Money Transfer Failed ..");
                            }
                            //if (StatusSendSmsMoneyTransferSuccesstoFailed == "Y")
                            //{
                            //    string msgssss = "";
                            //    string tempid = "";
                            //    string urlss = "";

                            //    var smsapionsts = db.apisms.Where(x => x.sts == "Y").SingleOrDefault();
                            //    var smsstypes = db.Sending_SMS_Templates.Where(x => x.SMS_TYPE == "MONEYTRANSFERMANUALMONEYFAILEDDUETO" && x.SMSAPIID == smsapionsts.id).SingleOrDefault();
                            //    if (smsstypes != null)
                            //    {
                            //        msgssss = string.Format(smsstypes.Templates, bankname, bank_account, transamount, ddl_refund);
                            //        tempid = smsstypes.Templateid;
                            //        urlss = smsapionsts.smsapi;

                            //        smssend.sendsmsallnew(RetailerMobile, msgssss, urlss, tempid);
                            //    }
                            //    //  smssend.sendsmsall(RetailerMobile, "Money Transfer in Bank " + bankname + " and Account Number " + bank_account + "and Amount" + transamount + " is Failed Due To " + ddl_refund + ".", "Money Transfer");
                            //}

                            smssend.sms_init(StatusSendSmsMoneyTransferSuccesstoFailed.Status, StatusSendSmsMoneyTransferSuccesstoFailed.Whatsapp_Status, "MONEYTRANSFERMANUALMONEYFAILEDDUETO", RetailerMobile, bankname, bank_account + " ", " " + transamount, ddl_refund);

                            if (StatusSendEmailMoneyTransferSuccesstoFailed == "Y")
                            {
                                smssend.SendEmailAll(RetailerEmail, "Money Transfer in Bank " + bankname + " and Account Number " + bank_account + "and Amount" + transamount + " is Failed Due To " + ddl_refund + ".", "Money Transfer", AdminEmail);
                            }
                            notify.sendmessage(RetailerEmail, "Money Transfer in Bank " + bankname + " and Account Number " + bank_account + "and Bank Refernce Id " + bankid + " and Amount " + transamount + " is Failed .");

                            return Json(new { Status = true, Message = "Successfully" });
                        }
                    }
                    else
                    {
                        return Json(new { Status = false, Message = "Your Security Password is Wrong!" });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, Message = ex.Message });
            }
        }

        //mannual failed

        #region MPOSSTART
        [PermissioncheckingAttribute(servicename = "MPOS", permision = "Read")]
        public ActionResult m_Possreport()
        {

            // show master id 
            var stands = db.Superstokist_details.ToList();
            IEnumerable<SelectListItem> selectList = from s in stands
                                                     select new SelectListItem
                                                     {
                                                         Value = s.SSId,
                                                         Text = s.FarmName.ToString()
                                                     };
            ViewBag.allmaster = new SelectList(selectList, "Value", "Text");
            //show dealer
            ViewBag.alldealer = new SelectList(db.select_Dealer_for_ddl(), "Dealerid", "FarmName", null).ToList();
            //Retailer 
            ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl("Admin"), "RetailerId", "Frm_Name", null).ToList();
            var totalwhitelabel = db.WhiteLabel_userList.ToList();
            IEnumerable<SelectListItem> selectList1 = from s in totalwhitelabel
                                                      select new SelectListItem
                                                      {
                                                          Value = s.WhiteLabelID,
                                                          Text = s.FrmName.ToString()
                                                      };
            ViewBag.allwhitelabel = new SelectList(selectList1, "Value", "Text");
            return View();
        }
        [HttpPost]
        [PermissioncheckingAttribute(servicename = "MPOS", permision = "Read")]
        public ActionResult m_Possreport(string txt_frm_date, string txt_to_date, string ddlusers, string allmaster2, string alldealer, string allretailer, string ddl_top, string ddl_status)
        {

            ViewBag.chk = "post";
            var totalwhitelabel = db.WhiteLabel_userList.ToList();
            IEnumerable<SelectListItem> selectList1 = from s in totalwhitelabel
                                                      select new SelectListItem
                                                      {
                                                          Value = s.WhiteLabelID,
                                                          Text = s.FrmName.ToString()
                                                      };
            ViewBag.allwhitelabel = new SelectList(selectList1, "Value", "Text");
            // show master id 
            var stands = db.Superstokist_details.ToList();
            IEnumerable<SelectListItem> selectList = from s in stands
                                                     select new SelectListItem
                                                     {
                                                         Value = s.SSId,
                                                         Text = s.FarmName.ToString()
                                                     };
            ViewBag.allmaster = new SelectList(selectList, "Value", "Text");
            //show dealer
            ViewBag.alldealer = new SelectList(db.select_Dealer_for_ddl(), "Dealerid", "FarmName", null).ToList();
            //Retailer 
            ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl("Admin"), "RetailerId", "Frm_Name", null);

            return View();
        }

        [ChildActionOnly]
        public ActionResult _m_Possreport(string txt_frm_date, string txt_to_date, string ddlusers, string allmaster2, string allwhitelabel1, string alldealer, string allretailer, string ddl_status)
        {

            if (txt_frm_date == null && txt_to_date == null && ddlusers == null || ddlusers == "")
            {

                txt_frm_date = DateTime.Now.ToString();
                txt_to_date = DateTime.Now.ToString();
                ddlusers = "Admin";

            }
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

            if (ddlusers == "Master")
            {
                if (allmaster2 == "" || allmaster2.Contains("Master") || allmaster2 == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = allmaster2;
                }
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
            if (ddlusers == "Whitelabel")
            {
                if (allwhitelabel1 == "" || allwhitelabel1.Contains("All Whitelabel") || allwhitelabel1 == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = allwhitelabel1;
                }
            }
            if (ddlusers == "Admin")
            {
                userid = "";
            }
            int pagesize = 20;

            var ch = db.Mpos_Report_New_paging(1, pagesize, ddlusers, userid, ddl_status, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            return View(ch);

        }

        [HttpPost]
        public ActionResult InfiniteScroll_mpos(int pageindex, string txt_frm_date, string txt_to_date, string ddlusers, string allmaster2, string allwhitelabel1, string alldealer, string allretailer, string ddl_status)
        {


            var userid = "";
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
            // show master id 



            if (ddlusers == "Master")
            {
                if (allmaster2 == "" || allmaster2.Contains("Master") || allmaster2 == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = allmaster2;
                }
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
            if (ddlusers == "Whitelabel")
            {
                if (allwhitelabel1 == "" || allwhitelabel1.Contains("All Whitelabel") || allwhitelabel1 == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = allwhitelabel1;
                }
            }
            if (ddlusers == "Admin")
            {
                userid = "";
            }
            int pagesize = 20;

            var tbrow = db.Mpos_Report_New_paging(pageindex, pagesize, ddlusers, userid, ddl_status, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            ViewData["totals"] = tbrow.Where(s => s.status == "00").Sum(s => Convert.ToInt32(s.amount));
            ViewData["Totalf"] = tbrow.Where(s => s.status != "00").Sum(s => Convert.ToInt32(s.amount));
            JsonModel jsonmodel = new JsonModel();
            jsonmodel.NoMoredata = tbrow.Count < pagesize;
            jsonmodel.HTMLString = renderPartialViewtostring("_m_Possreport", tbrow);
            return Json(jsonmodel);
        }
        [PermissioncheckingAttribute(servicename = "MPOS", permision = "Read")]
        public ActionResult mpos_Total(string txt_frm_date, string txt_to_date, string ddlusers, string allmaster2, string alldealer, string allretailer, string ddl_status)
        {

            if (txt_frm_date == null && txt_to_date == null && ddlusers == null || ddlusers == "")
            {

                txt_frm_date = DateTime.Now.ToString();
                txt_to_date = DateTime.Now.ToString();
                ddlusers = "Admin";

            }
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

            if (ddlusers == "Master")
            {
                if (allmaster2 == "" || allmaster2.Contains("Master") || allmaster2 == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = allmaster2;
                }
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

            if (ddlusers == "Admin")
            {
                userid = "";
            }
            int pagesize = 20;

            var ch = db.Mpos_Report_New_paging(1, pagesize, ddlusers, userid, ddl_status, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            var successtotal1 = ch.Where(s => s.status == "00").Sum(s => Convert.ToInt32(s.amount));
            var Failedtotal1 = ch.Where(s => s.status == "22").Sum(s => Convert.ToInt32(s.amount));
            var data = new
            {
                success = successtotal1,
                failed = Failedtotal1,

            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public virtual ActionResult ExcelRechargereport_mpos(string txt_frm_date, string txt_to_date, string ddlusers, string allmaster2, string alldealer, string allretailer, string ddl_status)
        {

            var userid = User.Identity.GetUserId();
            if (ddlusers == null || ddlusers == "")
            {

                ddlusers = "Admin";

            }

            if (ddlusers == "Master")
            {
                if (allmaster2 == "" || allmaster2.Contains("Master") || allmaster2 == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = allmaster2;
                }
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

            if (ddlusers == "Admin")
            {
                userid = "ALL";
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
            dt2.Columns.Add("Comm", typeof(string));
            dt2.Columns.Add("Debit Amount", typeof(string));
            dt2.Columns.Add("Merchant ID", typeof(string));
            dt2.Columns.Add("Bank MerchantID", typeof(string));
            dt2.Columns.Add("Bank TerminalID", typeof(string));
            dt2.Columns.Add("Card Brand", typeof(string));
            dt2.Columns.Add("Card Holder Name", typeof(string));
            dt2.Columns.Add("Card Number", typeof(string));
            dt2.Columns.Add("Card Type", typeof(string));
            dt2.Columns.Add("Date", typeof(string));
            dt2.Columns.Add("Invoice No", typeof(string));
            dt2.Columns.Add("Payment Id", typeof(string));
            dt2.Columns.Add("Merchant Address", typeof(string));
            dt2.Columns.Add("Admin Pre", typeof(string));
            dt2.Columns.Add("Admin Post", typeof(string));
            dt2.Columns.Add("Master Pre", typeof(string));
            dt2.Columns.Add("Master Post", typeof(string));
            dt2.Columns.Add("Dealer Pre", typeof(string));
            dt2.Columns.Add("Dealer Post", typeof(string));
            dt2.Columns.Add("Retailer Pre", typeof(string));
            dt2.Columns.Add("Retailer Post", typeof(string));
            dt2.Columns.Add("Master Comm", typeof(string));
            dt2.Columns.Add("Dealer Comm", typeof(string));
            dt2.Columns.Add("Master GST", typeof(string));
            dt2.Columns.Add("Dealer GST", typeof(string));
            dt2.Columns.Add("Retailer GST", typeof(string));
            dt2.Columns.Add("Master Income", typeof(string));
            dt2.Columns.Add("Dealer Income", typeof(string));
            dt2.Columns.Add("Retailer Income", typeof(string));
            dt2.Columns.Add("Admin Income", typeof(string));

            var respo = db.Mpos_Report_New_paging(1, 100000, ddlusers, userid, ddl_status, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            if (respo.Count > 0)
            {
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
                    dt2.Rows.Add(sts + "=" + item.Frm_Name, item.mPOSid, item.BankRRN, item.TxnId, item.transType, item.amount, item.Rem_comm, item.totalamount,
                    item.merchatID, item.bankmerchantID, item.bankterminalID, item.cardbrand, item.cardholdername, item.cardnumber, item.cardtype, item.date, item.invoiceno
                    , item.paymentid, item.merchantnameaddress, item.AdminPre, item.AdminPost, item.MasterPre, item.MasterPost, item.DealerPre, item.DealerPost, item.RetailerPre
                    , item.RetailerPost, item.MdComm, item.Dlm_Comm, item.mdgst, item.dlmgst, item.remgst, item.mdincome, item.dlmincome, item.remincome, item.adminincome);
                }
            }
            else
            {
                dt2.Rows.Add("", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "");
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

        public ActionResult PDFMposReport(string txt_frm_date, string txt_to_date, string ddlusers, string allmaster2, string alldealer, string allretailer, string ddl_status)
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


            var userid = User.Identity.GetUserId();
            if (ddlusers == "Master")
            {
                if (allmaster2 == "" || allmaster2.Contains("allmaster") || allmaster2 == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = allmaster2;
                }
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


            if (ddlusers == "" || ddlusers == null)
            {
                ddlusers = "Admin";
            }

            if (ddl_status == "" || ddl_status == null)
            {
                ddl_status = "ALL";
            }


            System.Threading.Thread.Sleep(1000);

            var respo = db.Mpos_Report_New_paging(1, 100000, ddlusers, userid, ddl_status, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            return new ViewAsPdf(respo);
        }


        [HttpPost]
        [PermissioncheckingAttribute(servicename = "MPOS", permision = "Read")]
        public ActionResult m_Possreport_View(int Idno)
        {
            var detail = db.Mpos_Report_View(Idno);
            return Json(detail, JsonRequestBehavior.AllowGet);
        }
        [PermissioncheckingAttribute(servicename = "MPOS", permision = "Write")]
        public ActionResult MposStatus(int idno)
        {
            var entites = db.mPosInfoes.Where(aa => aa.Idno == idno).SingleOrDefault();
            var tokn = Responsetoken.gettoken();
            VastBazaar cb = new VastBazaar();
            var PartnerId = db.Retailer_Details.Where(aa => aa.RetailerId == entites.RetailerId).SingleOrDefault().PartnerID;
            var responseall = cb.MposStatus(entites.Message, entites.transType, PartnerId, tokn);
            return Json("", JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region AEPS_Report
        [HttpGet]
        [PermissioncheckingAttribute(servicename = "AEPS", permision = "Read")]
        public ActionResult AepsReport()
        {

            // show master id 
            var stands = db.Superstokist_details.ToList();
            IEnumerable<SelectListItem> selectList = from s in stands
                                                     select new SelectListItem
                                                     {
                                                         Value = s.SSId,
                                                         Text = s.FarmName.ToString()
                                                     };
            ViewBag.allmaster = new SelectList(selectList, "Value", "Text");
            //show whitelabel
            var totalwhitelabel = db.WhiteLabel_userList.ToList();
            IEnumerable<SelectListItem> selectList1 = from s in totalwhitelabel
                                                      select new SelectListItem
                                                      {
                                                          Value = s.WhiteLabelID,
                                                          Text = s.FrmName.ToString()
                                                      };
            ViewBag.allwhitelabel = new SelectList(selectList1, "Value", "Text");
            //show dealer
            ViewBag.alldealer = new SelectList(db.select_Dealer_for_ddl(), "Dealerid", "FarmName", null).ToList();
            //Retailer 
            ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl("Admin"), "RetailerId", "Frm_Name", null);
            //api users 
            ViewBag.apiid = new SelectList(db.select_apiusers_for_ddl("Admin"), "apiid", "username", null);
            //apiname
            var apiname = db.money_api_status.Where(a => a.api_name == "VASTWEB" && a.catagory == "PAYOUT").ToList();
            IEnumerable<SelectListItem> selectapiname = from p in apiname
                                                        select new SelectListItem
                                                        {
                                                            Value = p.api_name,
                                                            Text = p.api_name
                                                        };
            ViewBag.apiname = new SelectList(selectapiname, "Value", "Text");


            return View();
        }
        [PermissioncheckingAttribute(servicename = "AEPS", permision = "Read")]
        [HttpPost]
        public ActionResult AepsReport(string txt_frm_date, string txt_to_date, string ddlusers, string allmaster1, string alldealer, string allretailer, string allwhitelabel1, string ddl_status, string api_name, string allapiuser, string ddl_settletype)
        {
            ViewBag.chk = "post";

            ViewBag.apiid = new SelectList(db.select_apiusers_for_ddl("Admin"), "apiid", "username", null).ToList();

            //show whitelabel
            var totalwhitelabel = db.WhiteLabel_userList.ToList();
            IEnumerable<SelectListItem> selectList1 = from s in totalwhitelabel
                                                      select new SelectListItem
                                                      {
                                                          Value = s.WhiteLabelID,
                                                          Text = s.EmailId + "--" + s.Name.ToString()
                                                      };
            ViewBag.allwhitelabel = new SelectList(selectList1, "Value", "Text");
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
            ViewBag.alldealer = new SelectList(db.select_Dealer_for_ddl(), "Dealerid", "FarmName", null).ToList();
            //Retailer 
            ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl("Admin"), "RetailerId", "Frm_Name", null);
            //api users 
            ViewBag.apiid = new SelectList(db.select_apiusers_for_ddl("Admin"), "apiid", "username", null);
            //apiname
            var apiname = db.money_api_status.Where(a => a.api_name == "VASTWEB" && a.catagory == "DMT").ToList();
            IEnumerable<SelectListItem> selectapiname = from p in apiname
                                                        select new SelectListItem
                                                        {
                                                            Value = p.api_name,
                                                            Text = p.api_name
                                                        };
            ViewBag.apiname = new SelectList(selectapiname, "Value", "Text");

            return View();
        }



        [ChildActionOnly]
        public ActionResult _AepsReport(string txt_frm_date, string txt_to_date, string ddlusers, string allmaster1, string alldealer, string allretailer, string allwhitelabel1, string ddl_status, string api_name, string allapiuser, string ddl_settletype)
        {

            var userid = "";

            if (txt_frm_date == null && txt_to_date == null && ddl_status == null && ddlusers == null)
            {
                txt_frm_date = DateTime.Now.ToString();
                txt_to_date = DateTime.Now.ToString();
                ddlusers = "Admin"; ddl_status = "ALL";
                ddl_settletype = "ALL";

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
            //show whitelabel


            if (ddl_status == "Status")
            {
                ddl_status = "ALL";
            }
            else if (ddl_status == "ALL")
            {
                ddl_status = "ALL";
            }
            if (ddl_settletype == "Settle")
            {
                ddl_settletype = "ALL";
            }
            else if (ddl_settletype == "ALL")
            {
                ddl_settletype = "ALL";
            }


            if (ddlusers == "Master")
            {
                if (allmaster1 == "" || allmaster1.Contains("Master") || allmaster1 == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = allmaster1;
                }
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
            if (ddlusers == "Whitelabel")
            {
                if (allwhitelabel1 == "" || allwhitelabel1.Contains("All Whitelabel") || allwhitelabel1 == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = allwhitelabel1;
                }
            }
            if (ddlusers == "Admin")
            {
                userid = "Admin";
            }

            var rowdata = db.Aeps_Report_New(ddlusers, userid, ddl_status, 0, "", "", "", ddl_settletype, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), 1, 20).ToList();

            return View(rowdata);


        }
        public ActionResult InfiniteScroll_Aeps(string txt_frm_date, string txt_to_date, string ddlusers, string allmaster1, string alldealer, string allretailer, string allwhitelabel1, string ddl_status, string api_name, string allapiuser, string ddl_settletype, int pageindex)
        {
            var userid = "";
            var APIname = "";


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


            if (ddl_status == "Status")
            {
                ddl_status = "ALL";
            }
            else if (ddl_status == "ALL")
            {
                ddl_status = "ALL";
            }
            if (ddl_settletype == "Settle")
            {
                ddl_settletype = "ALL";
            }
            else if (ddl_settletype == "ALL")
            {
                ddl_settletype = "ALL";
            }
            if (api_name == null)
            {
                api_name = "";
            }
            if (api_name == "Select Api Name" || api_name.Contains("Select Api Name") || api_name == "")
            {
                APIname = "ALL";
            }
            else
            {
                APIname = api_name.ToUpper();
            }

            if (ddlusers == "Master")
            {
                if (allmaster1 == "" || allmaster1.Contains("Master") || allmaster1 == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = allmaster1;
                }
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
            if (ddlusers == "Whitelabel")
            {
                if (allwhitelabel1 == "" || allwhitelabel1.Contains("All Retailer") || allwhitelabel1 == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = allwhitelabel1;
                }
            }
            if (ddlusers == "Admin")
            {
                userid = "Admin";
            }
            int pagesize = 20;
            var tbrow = db.Aeps_Report_New(ddlusers, userid, ddl_status, 0, "", "", "", ddl_settletype, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), pageindex, pagesize).ToList();
            JsonModel jsonmodel = new JsonModel();
            jsonmodel.NoMoredata = tbrow.Count < pagesize;
            jsonmodel.HTMLString = renderPartialViewtostring("_AepsReport", tbrow);
            return Json(jsonmodel);

        }
        [PermissioncheckingAttribute(servicename = "AEPS", permision = "Read")]
        public virtual ActionResult ExcelRechargereport_Aeps(string txt_frm_date, string txt_to_date, string ddlusers, string allmaster1, string alldealer, string allretailer, string allwhitelabel1, string ddl_status, string api_name, string allapiuser, string ddl_settletype)
        {
            var APIname = "";
            var userid = User.Identity.GetUserId();
            if (ddlusers == null || ddlusers == "")
            {

                ddlusers = "Admin";

            }
            if (ddl_status == "Status")
            {
                ddl_status = "ALL";
            }
            else if (ddl_status == "ALL")
            {
                ddl_status = "ALL";
            }
            if (ddl_settletype == "Settle")
            {
                ddl_settletype = "ALL";
            }
            else if (ddl_settletype == "ALL")
            {
                ddl_settletype = "ALL";
            }
            if (api_name == "Select Api Name" || api_name.Contains("Select Api Name") || api_name == "")
            {
                APIname = "ALL";
            }
            else
            {
                APIname = api_name.ToUpper();
            }

            if (ddlusers == "Master")
            {
                if (allmaster1 == "" || allmaster1.Contains("Master") || allmaster1 == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = allmaster1;
                }
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
            if (ddlusers == "Whitelabel")
            {
                if (allwhitelabel1 == "" || allwhitelabel1.Contains("All Retailer") || allwhitelabel1 == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = allwhitelabel1;
                }
            }
            if (ddlusers == "Admin")
            {
                userid = "Admin";
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
            dt2.Columns.Add("Live Status", typeof(string));
            dt2.Columns.Add("Customer Bank Name", typeof(string));
            dt2.Columns.Add("Aadhaar Number", typeof(string));
            dt2.Columns.Add("Transfer", typeof(string));
            dt2.Columns.Add("User Comm", typeof(string));
            dt2.Columns.Add("User Credit", typeof(string));
            dt2.Columns.Add("User Pre", typeof(string));
            dt2.Columns.Add("User Post", typeof(string));
            dt2.Columns.Add("Bank RRN", typeof(string));
            dt2.Columns.Add("Request Date", typeof(string));
            dt2.Columns.Add("Admin Pre", typeof(string));
            dt2.Columns.Add("Admin Post", typeof(string));
            dt2.Columns.Add("Master Pre", typeof(string));
            dt2.Columns.Add("Master Post", typeof(string));
            dt2.Columns.Add("Distributor Pre", typeof(string));
            dt2.Columns.Add("Distributor Post", typeof(string));
            dt2.Columns.Add("Terminal ID", typeof(string));
            dt2.Columns.Add("Remark", typeof(string));


            var respo = db.Aeps_Report_New(ddlusers, userid, ddl_status, 0, "", "", "", ddl_settletype, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), 1, 100000).ToList();

            if (respo.Count > 0)
            {
                foreach (var item in respo)
                {
                    var usercredit = (item.Amount + item.RemIncome);
                    dt2.Rows.Add(item.Status + "=" + item.Frm_Name, item.BankName, item.AccountHolderAadhaar, item.Amount, usercredit, item.RemIncome, item.RemPre, item.RemPost, item.BankRRN,
                    item.Txn_Date, item.AdminPre, item.AdminPost, item.MdPre, item.MdPost, item.DlmPre, item.DlmPost, item.TerminalId, item.Remark);
                }
            }
            else
            {
                dt2.Rows.Add("", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "");
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
        [PermissioncheckingAttribute(servicename = "AEPS", permision = "Read")]
        public ActionResult PDFAepsReport(string txt_frm_date, string txt_to_date, string ddlusers, string allmaster1, string alldealer, string allretailer, string allwhitelabel1, string ddl_status, string api_name, string allapiuser, string ddl_settletype)
        {
            var userid = "";


            if (txt_frm_date == "")
            {
                return RedirectToAction("_AepsReport", "Home");

            }
            if (txt_frm_date == null && txt_to_date == null && ddl_status == null)
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




            if (ddl_status == "Status")
            {
                ddl_status = "ALL";
            }
            else if (ddl_status == "ALL")
            {
                ddl_status = "ALL";
            }
            if (ddl_settletype == "Settle")
            {
                ddl_settletype = "ALL";
            }
            else if (ddl_settletype == "ALL")
            {
                ddl_settletype = "ALL";
            }

            if (ddlusers == "Master")
            {
                if (allmaster1 == "" || allmaster1.Contains("Master") || allmaster1 == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = allmaster1;
                }
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
            if (ddlusers == "Whitelabel")
            {
                if (allwhitelabel1 == "" || allwhitelabel1.Contains("All Retailer") || allwhitelabel1 == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = allwhitelabel1;
                }
            }
            if (ddlusers == "Admin")
            {
                userid = "Admin";
            }

            var respo = db.Aeps_Report_New(ddlusers, userid, ddl_status, 0, "", "", "", ddl_settletype, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), 1, 100000).ToList();


            return new ViewAsPdf(respo);
            //return View(ch);
        }


        [PermissioncheckingAttribute(servicename = "AEPS", permision = "Read")]
        public ActionResult AepsReport_Total(string txt_frm_date, string txt_to_date, string ddlusers, string allmaster1, string alldealer, string allretailer, string allwhitelabel1, string ddl_status, string api_name, string allapiuser, string ddl_settletype)
        {
            var userid = "";
            var APIname = "";
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




            if (ddl_status == "Status")
            {
                ddl_status = "ALL";
            }
            else if (ddl_status == "ALL")
            {
                ddl_status = "ALL";
            }
            if (ddl_settletype == "Settle")
            {
                ddl_settletype = "ALL";
            }
            else if (ddl_settletype == "ALL")
            {
                ddl_settletype = "ALL";
            }
            //if (api_name == "Select Api Name" || api_name.Contains("Select Api Name") || api_name==null)
            //{
            //    APIname = "ALL";
            //}
            //else
            //{
            //    APIname = api_name.ToUpper();
            //}

            if (ddlusers == "Master")
            {
                if (allmaster1 == "" || allmaster1.Contains("Master") || allmaster1 == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = allmaster1;
                }
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
            if (ddlusers == "Whitelabel")
            {
                if (allwhitelabel1 == "" || allwhitelabel1.Contains("All Retailer") || allwhitelabel1 == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = allwhitelabel1;
                }
            }
            if (ddlusers == "Admin")
            {
                userid = "Admin";
            }

            var ch = db.Aeps_Report_New(ddlusers, userid, ddl_status, 0, "", "", "", ddl_settletype, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), 1, 100000).ToList();
            var successtotal1 = ch.Where(s => s.Status.ToUpper().Contains("SUCCESS")).Sum(s => Convert.ToInt32(s.Amount));
            var Failedtotal1 = ch.Where(s => s.Status.ToUpper().Contains("FAILED")).Sum(s => Convert.ToInt32(s.Amount));
            var PendingTotal1 = ch.Where(s => s.Status.ToUpper().Contains("PENDING")).Sum(s => Convert.ToInt32(s.Amount));
            var data = new
            {
                success = successtotal1,
                failed = Failedtotal1,
                pending = PendingTotal1,

            };

            return Json(data, JsonRequestBehavior.AllowGet);
            //return View(ch);
        }


        [HttpPost]
        [PermissioncheckingAttribute(servicename = "AEPS", permision = "Read")]
        public ActionResult AepsReport_View(int Idno)
        {
            var detail = db.Aeps_Report_New_View(Idno);
            return Json(detail, JsonRequestBehavior.AllowGet);
        }
        [PermissioncheckingAttribute(servicename = "AEPS", permision = "Write")]
        public ActionResult AEPSsucess(string trans_id, string RRN, string terminalID, string Remark)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(trans_id) && !string.IsNullOrWhiteSpace(RRN))
                {
                    var aepstxn = db.AEPS_TXN_Details.Where(p => p.MerchantTxnId == trans_id).SingleOrDefault();
                    if (aepstxn.Status.ToUpper() == "PENDING")
                    {
                        var StatusSendSmsAEPSSuccess = db.SMSSendAlls.Where(a => a.ServiceName == "aepssucconline").SingleOrDefault();
                        var StatusSendEmailAEPSSuccess = db.EmailSendAlls.Where(a => a.ServiceName == "aepssucconline1").SingleOrDefault().Status;

                        var userid = User.Identity.GetUserId();

                        var Retailerid = aepstxn.UserId;
                        var bankname = aepstxn.BankName;
                        var Amount = aepstxn.Amount;
                        var status = aepstxn.Status;
                        var userroleid = db.UserRoles.Where(aa => aa.UserId == Retailerid).SingleOrDefault().RoleId;
                        var userrole = db.Roles.Where(aa => aa.RoleId == userroleid).SingleOrDefault().Name;
                        var RetailerMobile = ""; var RetailerEmail = "";
                        var AdminEmail = db.Admin_details.SingleOrDefault().email;
                        System.Data.Entity.Core.Objects.ObjectParameter procStatus = new System.Data.Entity.Core.Objects.ObjectParameter("ProcStatus", typeof(string));
                        System.Data.Entity.Core.Objects.ObjectParameter procMessage = new System.Data.Entity.Core.Objects.ObjectParameter("ProcMessage", typeof(string));
                        if (userrole == "Retailer")
                        {
                            RetailerMobile = db.Retailer_Details.Where(aa => aa.RetailerId == Retailerid).Single().Mobile;
                            RetailerEmail = db.Retailer_Details.Where(aa => aa.RetailerId == Retailerid).Single().Email;
                            if (status == "M_Pending")
                            {
                                db.update_Aeps_miniStatement(trans_id, "M_Success", RRN, 0);
                            }
                            else if (aepstxn.Type == "Aadhar Pay")
                            {
                                var xx = db.proc_AEPS_Aadharpay_PostProcess(Retailerid, trans_id, RRN, terminalID, "Manual Success", "", "web", Remark, 0, procStatus, procMessage).SingleOrDefault();
                            }
                            else
                            {
                                var xx = db.proc_AEPS_PostProcess(Retailerid, trans_id, RRN, terminalID, "Manual Success", "", "web", Remark, 0, procStatus, procMessage).SingleOrDefault();
                            }
                        }
                        if (userrole == "Whitelabelretailer")
                        {
                            RetailerMobile = db.Whitelabel_Retailer_Details.Where(aa => aa.RetailerId == Retailerid).Single().Mobile;
                            RetailerEmail = db.Whitelabel_Retailer_Details.Where(aa => aa.RetailerId == Retailerid).Single().Mobile;
                            if (status == "M_Pending")
                            {
                                db.update_Aeps_miniStatement(trans_id, "M_Success", RRN, 0);
                            }
                            else
                            {
                                var xx = db.proc_whitelabel_AEPS_PostProcess("WADMIN", trans_id, RRN, terminalID, "Manual Success", "", "web", Remark, 0, procStatus, procMessage).SingleOrDefault();
                            }
                        }


                        if (Convert.ToString(procStatus.Value) == "Success")
                        {
                            //if (StatusSendSmsAEPSSuccess == "Y")
                            //{
                            //    string msgssss = "";
                            //    string tempid = "";
                            //    string urlss = "";

                            //    var smsapionsts = db.apisms.Where(x => x.sts == "Y").SingleOrDefault();
                            //    var smsstypes = db.Sending_SMS_Templates.Where(x => x.SMS_TYPE == "ADMINAEPSTRANSACTIONSUCCESS" && x.SMSAPIID == smsapionsts.id).SingleOrDefault();
                            //    if (smsstypes != null)
                            //    {



                            //        msgssss = string.Format(smsstypes.Templates, bankname, trans_id, Amount);
                            //        tempid = smsstypes.Templateid;
                            //        urlss = smsapionsts.smsapi;

                            //        smssend.sendsmsallnew(RetailerMobile, msgssss, urlss, tempid);
                            //    }

                            //    //  smssend.sendsmsall(RetailerMobile, "AEPS in Bank " + bankname + " and Txn. Id " + trans_id + " and Amount : " + Amount + " is Success.", "AEPS Transaction");
                            //}

                            smssend.sms_init(StatusSendSmsAEPSSuccess.Status, StatusSendSmsAEPSSuccess.Whatsapp_Status, "ADMINAEPSTRANSACTIONSUCCESS", RetailerMobile, bankname, trans_id, Amount);

                            if (StatusSendEmailAEPSSuccess == "Y")
                            {
                                smssend.SendEmailAll(RetailerEmail, "AEPS in Bank " + bankname + " and Txn. Id " + trans_id + " and Amount : " + Amount + " is Success.", "AEPS Transaction", AdminEmail);
                            }
                            return Json(new { Status = true, Message = "Successfully" });
                        }
                        else
                        {
                            return Json(new { Status = false, Message = "Unable to manualy success." });
                        }
                    }
                    else
                    {
                        int? idnn = Convert.ToInt32(aepstxn.Idno);
                        db.AEPS_Failed_to_success(idnn, "Success", RRN);
                        return Json(new { Status = true, Message = "Successfully" });
                    }

                }
                else
                {
                    return Json(new { Status = false, Message = "RRN can not be empty." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, Message = ex.Message });
            }
        }
        [HttpPost]
        [PermissioncheckingAttribute(servicename = "AEPS", permision = "Write")]
        public JsonResult AepsStatus(string txnid)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(txnid))
                {
                    var StatusSendSmsAEPSSuccessSatus = db.SMSSendAlls.Where(a => a.ServiceName == "aepssucconline").SingleOrDefault();
                    var StatusSendEmailAEPSSuccessSatus = db.EmailSendAlls.Where(a => a.ServiceName == "aepssucconline1").SingleOrDefault().Status;

                    var userid = User.Identity.GetUserId();
                    //var retailer = db.Retailer_Details.SingleOrDefault(a => a.RetailerId == userid);
                    var aepstxn = db.AEPS_TXN_Details.Where(p => p.MerchantTxnId == txnid);
                    var Retailerid = aepstxn.Single().UserId;
                    var bankname = aepstxn.Single().BankName;
                    var Amount = aepstxn.Single().Amount;
                    var userroleid = db.UserRoles.Where(aa => aa.UserId == Retailerid).SingleOrDefault().RoleId;
                    var userrole = db.Roles.Where(aa => aa.RoleId == userroleid).SingleOrDefault().Name;
                    var trnTimestamp = aepstxn.SingleOrDefault().Txn_Date;
                    var from = trnTimestamp.Day + "/" + trnTimestamp.Month + "/" + trnTimestamp.Year + " " + trnTimestamp.Hour + ":" + trnTimestamp.Minute + ":" + trnTimestamp.Second;
                    var RetailerMobile = ""; var RetailerEmail = "";
                    var AdminEmail = db.Admin_details.SingleOrDefault().email;
                    if (userrole == "Retailer")
                    {
                        RetailerMobile = db.Retailer_Details.Where(aa => aa.RetailerId == Retailerid).Single().Mobile;
                        RetailerEmail = db.Retailer_Details.Where(aa => aa.RetailerId == Retailerid).Single().Email;
                    }
                    else if (userrole == "Whitelabelretailer")
                    {
                        RetailerMobile = db.Whitelabel_Retailer_Details.Where(aa => aa.RetailerId == Retailerid).Single().Mobile;
                        RetailerEmail = db.Whitelabel_Retailer_Details.Where(aa => aa.RetailerId == Retailerid).Single().Email;
                    }
                    var token = string.Empty;
                    token = Responsetoken.gettoken();
                    // Main obj = new Main();
                    VWMain obj = new VWMain();
                    if (string.IsNullOrWhiteSpace(token))
                    {
                        var viewresponse = new { Status = "Failed", Message = "Failed at provider server." };
                        return Json(JsonConvert.SerializeObject(obj), JsonRequestBehavior.AllowGet);
                    }
                    var client = new RestClient(VastbazaarBaseUrl + "api/AEPS/statusCheckinstant");
                    client.Timeout = -1;
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("Authorization", "Bearer " + token);
                    request.AddHeader("Content-Type", "application/json");
                    var req = new
                    {
                        merchantId = "",
                        merchantTranId = txnid,
                        hash = "123",
                        trnTimestamp = from
                    };
                    var requ = JsonConvert.SerializeObject(req);
                    request.AddParameter("application/json", requ, ParameterType.RequestBody);
                    IRestResponse response = client.Execute(request);
                    var respchk = response.Content;
                    dynamic respp = JsonConvert.DeserializeObject(respchk);
                    if (response.StatusCode != HttpStatusCode.OK || string.IsNullOrWhiteSpace(response.Content))
                    {
                        var bbb = new { Status = "Failed", Message = "Failed from bank." };
                        return Json(JsonConvert.SerializeObject(bbb));
                    }
                    else
                    {
                        var statuscode = respp.Content.ADDINFO.statuscode;
                        var msg = respp.Content.ADDINFO.status;
                        if (statuscode == "TXN")
                        {

                            var rrn = respp.Content.ADDINFO.data.serviceprovider_id.ToString();
                            var currentsts = aepstxn.SingleOrDefault().Status;
                            var cc = new { Status = "Success", Message = "AEPS SUCCESS" };
                            if (currentsts.ToUpper() == "PENDING")
                            {
                                System.Data.Entity.Core.Objects.ObjectParameter procStatus = new System.Data.Entity.Core.Objects.ObjectParameter("ProcStatus", typeof(string));
                                System.Data.Entity.Core.Objects.ObjectParameter procMessage = new System.Data.Entity.Core.Objects.ObjectParameter("ProcMessage", typeof(string));
                                if (userrole == "Retailer")
                                {
                                    db.proc_AEPS_PostProcess("ADMIN", txnid, rrn, "", "Manual Success", response.Content, "web", respchk, 0, procStatus, procMessage);
                                }
                                else
                                {
                                    db.proc_whitelabel_AEPS_PostProcess("WADMIN", txnid, rrn, "", "Manual Success", response.Content, "web", respchk, 0, procStatus, procMessage);
                                }

                                //if (StatusSendSmsAEPSSuccessSatus == "Y")
                                //{
                                //    string msgssss = "";
                                //    string tempid = "";
                                //    string urlss = "";

                                //    var smsapionsts = db.apisms.Where(x => x.sts == "Y").SingleOrDefault();
                                //    var smsstypes = db.Sending_SMS_Templates.Where(x => x.SMS_TYPE == "ADMINAEPSTRANSACTIONSUCCESS" && x.SMSAPIID == smsapionsts.id).SingleOrDefault();
                                //    if (smsstypes != null)
                                //    {
                                //        msgssss = string.Format(smsstypes.Templates, bankname, txnid, Amount);
                                //        tempid = smsstypes.Templateid;
                                //        urlss = smsapionsts.smsapi;

                                //        smssend.sendsmsallnew(RetailerMobile, msgssss, urlss, tempid);
                                //    }
                                //    //   smssend.sendsmsall(RetailerMobile, "AEPS in Bank " + bankname + " and Txn. Id " + txnid + " and Amount : " + Amount + " is Success.", "AEPS Transaction");
                                //}

                                smssend.sms_init(StatusSendSmsAEPSSuccessSatus.Status, StatusSendSmsAEPSSuccessSatus.Whatsapp_Status, "ADMINAEPSTRANSACTIONSUCCESS", RetailerMobile, bankname, txnid, Amount);

                                if (StatusSendEmailAEPSSuccessSatus == "Y")
                                {
                                    smssend.SendEmailAll(RetailerEmail, "AEPS in Bank " + bankname + " and Txn. Id " + txnid + " and Amount : " + Amount + " is Success.", "AEPS Transaction", AdminEmail);
                                }
                            }
                            return Json(JsonConvert.SerializeObject(cc));
                        }
                        else if (statuscode == "TUP")
                        {
                            var cc = new { Status = "Warning", Message = "Pending" };
                            return Json(JsonConvert.SerializeObject(cc));
                        }
                        else
                        {
                            var ff = new { Status = "Failed", Message = msg };
                            return Json(JsonConvert.SerializeObject(ff));
                        }
                    }
                }
                else
                {
                    var aa = new { Status = "Failed", Message = "Pass a valid txn Id to know status" };
                    return Json(JsonConvert.SerializeObject(aa));
                }
            }
            catch (Exception ex)
            {
                var aa = new { Status = "Failed", Message = "Internal Error" };
                return Json(JsonConvert.SerializeObject(aa));
            }
        }
        [PermissioncheckingAttribute(servicename = "AEPS", permision = "Write")]
        public ActionResult AepsFailed(string txtrefidno, string mobile, string ddl_refund, string secuirtypass)
        {
            try
            {
                var StatusSendSmsAEPSfailed = db.SMSSendAlls.Where(a => a.ServiceName == "Aepsfailonline").SingleOrDefault();
                var StatusSendEmailAEPSfailed = db.EmailSendAlls.Where(a => a.ServiceName == "Aepsfailonline1").SingleOrDefault().Status;

                var aepstxn = db.AEPS_TXN_Details.Where(p => p.MerchantTxnId == txtrefidno).Single();
                var Retailerid = aepstxn.UserId;
                var bankname = aepstxn.BankName;
                var Amount = aepstxn.Amount;
                var status = aepstxn.Status;
                var txnid = aepstxn.MerchantTxnId;
                var userroleid = db.UserRoles.Where(aa => aa.UserId == Retailerid).SingleOrDefault().RoleId;
                var userrole = db.Roles.Where(aa => aa.RoleId == userroleid).SingleOrDefault().Name;
                var RetailerMobile = ""; var RetailerEmail = "";
                var AdminEmail = db.Admin_details.SingleOrDefault().email;
                if (userrole == "Retailer")
                {
                    RetailerMobile = db.Retailer_Details.Where(aa => aa.RetailerId == Retailerid).Single().Mobile;
                    RetailerEmail = db.Retailer_Details.Where(aa => aa.RetailerId == Retailerid).Single().Email;
                }
                else if (userrole == "Whitelabelretailer")
                {
                    RetailerMobile = db.Whitelabel_Retailer_Details.Where(aa => aa.RetailerId == Retailerid).Single().Mobile;
                    RetailerEmail = db.Whitelabel_Retailer_Details.Where(aa => aa.RetailerId == Retailerid).Single().Email;
                }
                var userid = User.Identity.GetUserId();
                var password = Encrypt(secuirtypass);
                var tranpass = (from paa in db.admin_new_pass where paa.securitypass == password select paa).Count();
                if (tranpass > 0)
                {
                    System.Data.Entity.Core.Objects.ObjectParameter procStatus = new System.Data.Entity.Core.Objects.ObjectParameter("ProcStatus", typeof(string));
                    System.Data.Entity.Core.Objects.ObjectParameter procMessage = new System.Data.Entity.Core.Objects.ObjectParameter("ProcMessage", typeof(string));
                    if (userrole == "Retailer")
                    {
                        if (status == "M_Pending")
                        {
                            db.update_Aeps_miniStatement(txnid, "M_Failed", ddl_refund, 0);
                        }
                        else if (aepstxn.Type == "Aadhar Pay")
                        {
                            db.proc_AEPS_Aadharpay_PostProcess("ADMIN", txtrefidno, null, null, "Failed", "", "web", ddl_refund, 0, procStatus, procMessage);
                        }
                        else
                        {
                            db.proc_AEPS_PostProcess("ADMIN", txtrefidno, null, null, "Failed", "", "web", ddl_refund, 0, procStatus, procMessage);
                        }
                    }
                    if (userrole == "Whitelabelretailer")
                    {
                        if (status == "M_Pending")
                        {
                            db.update_Aeps_miniStatement(txnid, "M_Failed", ddl_refund, 0);
                        }
                        else
                        {
                            db.proc_whitelabel_AEPS_PostProcess("WADMIN", txtrefidno, null, null, "Failed", "", "web", ddl_refund, 0, procStatus, procMessage);
                        }
                    }
                    //if (StatusSendSmsAEPSfailed == "Y")
                    //{
                    //    string msgssss = "";
                    //    string tempid = "";
                    //    string urlss = "";

                    //    var smsapionsts = db.apisms.Where(x => x.sts == "Y").SingleOrDefault();
                    //    var smsstypes = db.Sending_SMS_Templates.Where(x => x.SMS_TYPE == "ADMINAEPSTRANSACTIONFAILED" && x.SMSAPIID == smsapionsts.id).SingleOrDefault();
                    //    if (smsstypes != null)
                    //    {
                    //        msgssss = string.Format(smsstypes.Templates, bankname, txtrefidno, Amount);
                    //        tempid = smsstypes.Templateid;
                    //        urlss = smsapionsts.smsapi;

                    //        smssend.sendsmsallnew(RetailerMobile, msgssss, urlss, tempid);
                    //    }
                    //    //  smssend.sendsmsall(RetailerMobile, "AEPS in Bank " + bankname + " and Txn. Id " + txtrefidno + " and Amount : " + Amount + " is Failed.", "AEPS Transaction");
                    //}

                    smssend.sms_init(StatusSendSmsAEPSfailed.Status, StatusSendSmsAEPSfailed.Whatsapp_Status, "ADMINAEPSTRANSACTIONFAILED", RetailerMobile, bankname, txtrefidno, Amount);

                    if (StatusSendEmailAEPSfailed == "Y")
                    {
                        smssend.SendEmailAll(RetailerEmail, "AEPS in Bank " + bankname + " and Txn. Id " + txtrefidno + " and Amount : " + Amount + " is Failed.", "AEPS Transaction", AdminEmail);
                    }
                    return Json(new { Status = true, Message = "Successfully" });
                }
                else
                {
                    return Json(new { Status = false, Message = "Your Security Password is Wrong!" });
                }

            }
            catch (Exception ex)
            {
                return Json(new { Status = false, Message = ex.Message });
            }
        }

        #endregion


        #region CashDepositReport

        [PermissioncheckingAttribute(servicename = "CASHDEPOSIT", permision = "Read")]
        public ActionResult CashDepositReport()
        {
            var stands = db.Superstokist_details.ToList();
            IEnumerable<SelectListItem> selectList = from s in stands
                                                     select new SelectListItem
                                                     {
                                                         Value = s.SSId,
                                                         Text = s.FarmName.ToString()
                                                     };
            ViewBag.allmaster = new SelectList(selectList, "Value", "Text");
            //show whitelabel
            var totalwhitelabel = db.WhiteLabel_userList.ToList();
            IEnumerable<SelectListItem> selectList1 = from s in totalwhitelabel
                                                      select new SelectListItem
                                                      {
                                                          Value = s.WhiteLabelID,
                                                          Text = s.FrmName.ToString()
                                                      };
            ViewBag.allwhitelabel = new SelectList(selectList1, "Value", "Text");
            //show dealer
            ViewBag.alldealer = new SelectList(db.select_Dealer_for_ddl(), "Dealerid", "FarmName", null).ToList();
            //Retailer 
            ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl("Admin"), "RetailerId", "Frm_Name", null);
            DateTime frm = DateTime.Now.Date;
            DateTime to = frm.AddDays(1);
            var chk = db.report_cash_deposit(frm, to, "", "Admin", "").ToList();
            return View(chk);
        }
        [HttpPost]
        [PermissioncheckingAttribute(servicename = "CASHDEPOSIT", permision = "Read")]
        public ActionResult CashDepositReport(string ddl_status, DateTime txt_frm_date, DateTime txt_to_date, string ddlusers, string allmaster1, string alldealer, string allretailer)
        {
            if (ddlusers == "")
            {
                ddlusers = "Admin";
            }
            ViewBag.chk = "post";
            var stands = db.Superstokist_details.ToList();
            IEnumerable<SelectListItem> selectList = from s in stands
                                                     select new SelectListItem
                                                     {
                                                         Value = s.SSId,
                                                         Text = s.FarmName.ToString()
                                                     };
            ViewBag.allmaster = new SelectList(selectList, "Value", "Text");
            //show whitelabel
            var totalwhitelabel = db.WhiteLabel_userList.ToList();
            IEnumerable<SelectListItem> selectList1 = from s in totalwhitelabel
                                                      select new SelectListItem
                                                      {
                                                          Value = s.WhiteLabelID,
                                                          Text = s.FrmName.ToString()
                                                      };
            ViewBag.allwhitelabel = new SelectList(selectList1, "Value", "Text");
            //show dealer
            ViewBag.alldealer = new SelectList(db.select_Dealer_for_ddl(), "Dealerid", "FarmName", null).ToList();
            //Retailer 
            ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl("Admin"), "RetailerId", "Frm_Name", null);
            //api users 
            ViewBag.apiid = new SelectList(db.select_apiusers_for_ddl("Admin"), "apiid", "username", null);
            var userid = "";
            if (allmaster1 != "")
            {
                userid = allmaster1;
            }
            else if (alldealer != "")
            {
                userid = alldealer;
            }
            else if (allretailer != "")
            {
                userid = allretailer;
            }
            DateTime to = txt_to_date.AddDays(1);
            var chk = db.report_cash_deposit(txt_frm_date, to, ddl_status, ddlusers, userid).ToList();
            return View(chk);
        }
        [PermissioncheckingAttribute(servicename = "CASHDEPOSIT", permision = "Read")]
        public ActionResult cashdeposit_view(int Idno)
        {
            var chkk = db.cash_deposit_history.Where(aa => aa.idno == Idno).ToList();
            return Json(chkk, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [PermissioncheckingAttribute(servicename = "CASHDEPOSIT", permision = "Write")]
        public ActionResult successcashdeposit(int idno, string bnkrrn)
        {
            var chkexist = db.cash_deposit_history.Where(x => x.bankrrn == bnkrrn.Trim()).FirstOrDefault();
            if (chkexist == null)
            {
                var chk = db.cash_deposit_history.Where(aa => aa.idno == idno).SingleOrDefault();
                if (chk.status.ToUpper() == "PENDING")
                {
                    chk.status = "Success";
                    chk.bankrrn = bnkrrn;
                    chk.res_date = DateTime.Now;
                    chk.response = "Manual Success";
                    db.SaveChanges();
                }
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
        [PermissioncheckingAttribute(servicename = "CASHDEPOSIT", permision = "Write")]
        public ActionResult Failedcashdeposit(int txtrefidno, string ddl_refund, string secuirtypass)
        {
            try
            {
                var RetailerEmail = "";
                var AdminEmail = db.Admin_details.Single().email;
                var comm = db.cash_deposit_history.Where(p => p.idno == txtrefidno).SingleOrDefault();
                if (comm.status.ToUpper() == "PENDING")
                {
                    var password = Encrypt(secuirtypass);
                    var tranpass = (from paa in db.admin_new_pass where paa.securitypass == password select paa).Count();
                    if (tranpass > 0)
                    {
                        db.update_cash_deposit(comm.reqid, "Failed", ddl_refund, "Manual Failed");
                        return Json(new { Status = true, Message = "Failed SuccessFully" });
                    }
                    else
                    {
                        return Json(new { Status = false, Message = "Your Security Password is Wrong!" });
                    }
                    /****************************************************************************************************************************************************/
                }
                else
                {
                    return Json(new { Status = false, Message = "Something Went Wrong" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, Message = ex.Message });
            }
        }
        [PermissioncheckingAttribute(servicename = "CASHDEPOSIT", permision = "Read")]
        public ActionResult ExcelCashReport(string ddl_status, DateTime txt_frm_date, DateTime txt_to_date, string ddlusers, string allmaster1, string alldealer, string allretailer)
        {
            if (ddlusers == "")
            {
                ddlusers = "Admin";
            }

            var userid = "";
            if (allmaster1 != "")
            {
                userid = allmaster1;
            }
            else if (alldealer != "")
            {
                userid = alldealer;
            }
            else if (allretailer != "")
            {
                userid = allretailer;
            }
            DateTime to = txt_to_date.AddDays(1);
            var chk = db.report_cash_deposit(txt_frm_date, to, ddl_status, ddlusers, userid).ToList();

            DataTable dataTbl = new DataTable();
            dataTbl.Columns.Add("Firm Name", typeof(string));
            dataTbl.Columns.Add("Status", typeof(string));
            dataTbl.Columns.Add("Amount", typeof(string));
            dataTbl.Columns.Add("Time", typeof(string));
            dataTbl.Columns.Add("BankRRN No", typeof(string));
            dataTbl.Columns.Add("Ben Name", typeof(string));
            dataTbl.Columns.Add("Bank Name", typeof(string));
            dataTbl.Columns.Add("Retailer Remain Pre", typeof(string));
            dataTbl.Columns.Add("Retailer Comm", typeof(string));
            dataTbl.Columns.Add("Retailer Gst", typeof(string));
            dataTbl.Columns.Add("Retailer Tds", typeof(string));
            dataTbl.Columns.Add("Retailer Remain Post", typeof(string));
            dataTbl.Columns.Add("Dealer Remain Pre", typeof(string));
            dataTbl.Columns.Add("Dealer Comm", typeof(string));
            dataTbl.Columns.Add("Dealer Gst", typeof(string));
            dataTbl.Columns.Add("Dealer Tds", typeof(string));
            dataTbl.Columns.Add("Dealer Remain Post", typeof(string));
            dataTbl.Columns.Add("Master Remain Pre", typeof(string));
            dataTbl.Columns.Add("Master Comm", typeof(string));
            dataTbl.Columns.Add("Master Gst", typeof(string));
            dataTbl.Columns.Add("Master Tds", typeof(string));
            dataTbl.Columns.Add("Master Remain Post", typeof(string));

            if (chk.Count > 0)
            {
                foreach (var item in chk)
                {
                    dataTbl.Rows.Add(item.Frm_Name, item.status, item.amount, item.req_date, item.bankrrn, item.benname, item.bankname, item.Rem_pre, item.rem_comm, item.rem_gst
                        , item.rem_tds, item.rem_post, item.dlm_pre, item.dlm_comm, item.dlm_gst, item.dlm_tds, item.dlm_post, item.md_pre, item.md_comm, item.md_gst, item.md_tds, item.md_post);
                }
            }
            else
            {
                dataTbl.Rows.Add("", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "");
            }
            var grid = new GridView();
            grid.DataSource = dataTbl;
            grid.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=CashDeposit_Report.xls");
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
        [PermissioncheckingAttribute(servicename = "CASHDEPOSIT", permision = "Read")]
        public ActionResult FindcashdepositTotal(string ddl_status, DateTime txt_frm_date, DateTime txt_to_date, string ddlusers, string allmaster1, string alldealer, string allretailer)
        {
            if (ddlusers == "")
            {
                ddlusers = "Admin";
            }
            ViewBag.chk = "post";
            var stands = db.Superstokist_details.ToList();
            IEnumerable<SelectListItem> selectList = from s in stands
                                                     select new SelectListItem
                                                     {
                                                         Value = s.SSId,
                                                         Text = s.FarmName.ToString()
                                                     };
            ViewBag.allmaster = new SelectList(selectList, "Value", "Text");
            //show whitelabel
            var totalwhitelabel = db.WhiteLabel_userList.ToList();
            IEnumerable<SelectListItem> selectList1 = from s in totalwhitelabel
                                                      select new SelectListItem
                                                      {
                                                          Value = s.WhiteLabelID,
                                                          Text = s.FrmName.ToString()
                                                      };
            ViewBag.allwhitelabel = new SelectList(selectList1, "Value", "Text");
            //show dealer
            ViewBag.alldealer = new SelectList(db.select_Dealer_for_ddl(), "Dealerid", "FarmName", null).ToList();
            //Retailer 
            ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl("Admin"), "RetailerId", "Frm_Name", null);
            //api users 
            ViewBag.apiid = new SelectList(db.select_apiusers_for_ddl("Admin"), "apiid", "username", null);
            var userid = "";
            if (allmaster1 != "")
            {
                userid = allmaster1;
            }
            else if (alldealer != "")
            {
                userid = alldealer;
            }
            else if (allretailer != "")
            {
                userid = allretailer;
            }
            DateTime to = txt_to_date.AddDays(1);
            var chk = db.report_cash_deposit(txt_frm_date, to, ddl_status, ddlusers, userid).ToList();
            var successtotal1 = chk.Where(a => a.status.ToUpper() == "SUCCESS").Count();
            var successtotal = chk.Where(a => a.status.ToUpper() == "SUCCESS").Sum(a => a.amount);
            var Failedtotal = chk.Where(aa => aa.status.ToUpper() == "FAILED").Sum(aa => aa.amount);
            var pendingtotal = chk.Where(aa => aa.status.ToUpper() == "PENDING").Sum(aa => aa.amount);
            var data = new
            {
                success = successtotal + successtotal1,
                failed = Failedtotal,
                pending = pendingtotal
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        #endregion

        public ActionResult Pancard_new_manual()
        {
            var allretailername = (db.select_retailer_for_ddl("Admin")).ToList();
            IEnumerable<SelectListItem> selectallretailer = from p in allretailername
                                                            select new SelectListItem
                                                            {
                                                                Value = p.RetailerId,
                                                                Text = p.Frm_Name,
                                                            };
            ViewBag.allretailer = new SelectList(selectallretailer, "Value", "Text");


            var txt_frm_date = DateTime.Now.ToString();
            var txt_to_date = DateTime.Now.ToString();


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


            pancard_reports p1 = new pancard_reports();
            var asas = Convert.ToDateTime(frm_date);
            p1.pancard_Transations_report_new_manual = db.pancard_transation_manual.Where(s => s.request_time > asas).OrderByDescending(s => s.idno).ToList();
            return View(p1);

        }
        [HttpPost]
        public ActionResult Pancard_new_manual(string allretailer, string ddl_status, string txt_frm_date, string txt_to_date)
        {
            var allretailername = (db.select_retailer_for_ddl("Admin")).ToList();
            IEnumerable<SelectListItem> selectallretailer = from p in allretailername
                                                            select new SelectListItem
                                                            {
                                                                Value = p.RetailerId,
                                                                Text = p.Frm_Name,
                                                            };
            ViewBag.allretailer = new SelectList(selectallretailer, "Value", "Text");

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
            pancard_reports p1 = new pancard_reports();
            var asd = Convert.ToDateTime(frm_date);
            var asd1 = Convert.ToDateTime(to_date);
            if (frm_date == null)
            {
                p1.pancard_Transations_report_new_manual = db.pancard_transation_manual.Where(s => s.request_time > asd).OrderByDescending(s => s.idno).ToList();
            }
            else
            {
                if (string.IsNullOrEmpty(allretailer))
                {
                    if (ddl_status == "ALL")
                    {
                        p1.pancard_Transations_report_new_manual = db.pancard_transation_manual.Where(s => s.request_time >= asd && s.request_time <= asd1).OrderByDescending(s => s.idno).ToList();
                    }
                    else
                    {
                        p1.pancard_Transations_report_new_manual = db.pancard_transation_manual.Where(s => s.request_time > asd && s.request_time < asd1 && s.status.ToUpper() == ddl_status.ToUpper()).OrderByDescending(s => s.idno).ToList();

                    }
                }
                else
                {
                    if (ddl_status == "ALL")
                    {
                        p1.pancard_Transations_report_new_manual = db.pancard_transation_manual.Where(s => s.request_time > asd && s.request_time < asd1 && s.Reailerid == allretailer).OrderByDescending(s => s.idno).ToList();
                    }
                    else
                    {
                        p1.pancard_Transations_report_new_manual = db.pancard_transation_manual.Where(s => s.request_time > asd && s.request_time < asd1 && s.Reailerid == allretailer && s.status.ToUpper() == ddl_status.ToUpper()).OrderByDescending(s => s.idno).ToList();

                    }
                }

            }
            return View(p1);
        }

        #region UITPANCARD
        [HttpGet]
        [PermissioncheckingAttribute(servicename = "PANCARD", permision = "Read")]
        public ActionResult TokenPurchaseReport()
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                var userid = User.Identity.GetUserId();


                // show master id 
                var stands = db.Superstokist_details.ToList();
                IEnumerable<SelectListItem> selectList = from s in stands
                                                         select new SelectListItem
                                                         {
                                                             Value = s.SSId,
                                                             Text = s.FarmName.ToString()
                                                         };
                var allretailername = (db.select_retailer_for_ddl("Admin")).ToList();
                IEnumerable<SelectListItem> selectallretailer = from p in allretailername
                                                                select new SelectListItem
                                                                {
                                                                    Value = p.RetailerId,
                                                                    Text = p.Frm_Name,
                                                                };
                var api = db.select_apiusers_for_ddl("Admin").ToList();
                IEnumerable<SelectListItem> selectapi = from p in api
                                                        select new SelectListItem
                                                        {
                                                            Value = p.apiid,
                                                            Text = p.username
                                                        };

                ViewBag.allmaster = new SelectList(selectList, "Value", "Text");

                ViewBag.allretailer = new SelectList(selectallretailer, "Value", "Text");


                //show dealer
                ViewBag.alldealer = new SelectList(db.select_Dealer_for_ddl(), "Dealerid", "FarmName", null).ToList();
                //Retailer 
                //ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl("Admin"), "RetailerId", "RetailerName", null);
                //api users 
                ViewBag.apiid = new SelectList(selectapi, "Value", "Text");
                //ViewBag.apiid = new SelectList(db.select_apiusers_for_ddl("Admin"), "apiid", "username", null);
                //whitelabel
                var totalwhitelabel = db.WhiteLabel_userList.ToList();
                IEnumerable<SelectListItem> selectList1 = from s in totalwhitelabel
                                                          select new SelectListItem
                                                          {
                                                              Value = s.WhiteLabelID,
                                                              Text = s.FrmName.ToString()
                                                          };
                ViewBag.whitelabel = new SelectList(selectList1, "Value", "Text");
                var apiname = db.money_api_status.Where(aa => aa.api_name == "VASTWEB" && aa.catagory == "PAYOUT").ToList();
                IEnumerable<SelectListItem> selectapiname = from p in apiname
                                                            select new SelectListItem
                                                            {
                                                                Value = p.api_name,
                                                                Text = p.api_name
                                                            };

                ViewBag.apiname = new SelectList(selectapiname, "Value", "Text");


                return View();
            }

        }
        [HttpPost]
        [PermissioncheckingAttribute(servicename = "PANCARD", permision = "Read")]
        public ActionResult TokenPurchaseReport(string ddl_status, string ddl_top, string txt_frm_date, string txt_to_date)
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                ViewBag.chk = "post";
                var userid = User.Identity.GetUserId();


                // show master id 
                var stands = db.Superstokist_details.ToList();
                IEnumerable<SelectListItem> selectList = from s in stands
                                                         select new SelectListItem
                                                         {
                                                             Value = s.SSId,
                                                             Text = s.Email + "--" + s.SuperstokistName.ToString()
                                                         };
                var allretailername = (db.select_retailer_for_ddl("Admin")).ToList();
                IEnumerable<SelectListItem> selectallretailer = from p in allretailername
                                                                select new SelectListItem
                                                                {
                                                                    Value = p.RetailerId,
                                                                    Text = p.Frm_Name
                                                                };
                var api = db.select_apiusers_for_ddl("Admin").ToList();
                IEnumerable<SelectListItem> selectapi = from p in api
                                                        select new SelectListItem
                                                        {
                                                            Value = p.apiid,
                                                            Text = p.username
                                                        };



                ViewBag.allretailer = new SelectList(selectallretailer, "Value", "Text");

                ViewBag.allmaster = new SelectList(selectList, "Value", "Text");
                //show dealer
                ViewBag.alldealer = new SelectList(db.select_Dealer_for_ddl(), "Dealerid", "FarmName", null).ToList();
                //Retailer 
                //ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl("Admin"), "RetailerId", "RetailerName", null);
                //api users 
                ViewBag.apiid = new SelectList(selectapi, "Value", "Text");
                //ViewBag.apiid = new SelectList(db.select_apiusers_for_ddl("Admin"), "apiid", "username", null);
                //whitelabel
                var totalwhitelabel = db.WhiteLabel_userList.ToList();
                IEnumerable<SelectListItem> selectList1 = from s in totalwhitelabel
                                                          select new SelectListItem
                                                          {
                                                              Value = s.WhiteLabelID,
                                                              Text = s.FrmName.ToString()
                                                          };
                ViewBag.whitelabel = new SelectList(selectList1, "Value", "Text");
                var apiname = db.money_api_status.Where(aa => aa.api_name == "VASTWEB" && aa.catagory == "PAYOUT").ToList();
                IEnumerable<SelectListItem> selectapiname = from p in apiname
                                                            select new SelectListItem
                                                            {
                                                                Value = p.api_name,
                                                                Text = p.api_name
                                                            };

                ViewBag.apiname = new SelectList(selectapiname, "Value", "Text");

                return View();
            }
        }

        [ChildActionOnly]
        public ActionResult _TokenPurchaseReport(string ddlusers, string allmaster, string alldealer, string allretailer, string allapiuser, string Whitelabel, string ddl_status, string txt_frm_date, string txt_to_date)
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {

                if (txt_frm_date == null && txt_to_date == null && ddlusers == null)
                {

                    txt_frm_date = DateTime.Now.ToString();
                    txt_to_date = DateTime.Now.ToString();
                    ddlusers = "Admin";
                }

                var userid = User.Identity.GetUserId();
                if (ddlusers == "Master")
                {
                    if (allmaster == "" || allmaster.Contains("Master") || allmaster == null)
                    {
                        userid = "ALL";
                    }
                    else
                    {
                        userid = allmaster;
                    }
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
                if (ddlusers == "WAdmin")
                {
                    if (Whitelabel == "" || Whitelabel.Contains("All Retailer") || Whitelabel == null)
                    {
                        userid = "ALL";
                    }
                    else
                    {
                        userid = Whitelabel;
                    }
                }
                if (ddlusers == "APIID")
                {
                    if (allapiuser == "" || allapiuser.Contains("All API") || Whitelabel == null)
                    {
                        userid = "ALL";
                    }
                    else
                    {
                        userid = allapiuser;
                    }
                }
                if (ddlusers == "Admin")
                {
                    userid = "ALL";
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
        }

        public ActionResult InfiniteScroll3(int pageindex, string ddlusers, string allmaster, string alldealer, string allretailer, string allapiuser, string Whitelabel, string ddl_status, string txt_frm_date, string txt_to_date)
        {

            var userid = User.Identity.GetUserId();
            if (ddlusers == "Master")
            {
                if (allmaster == "" || allmaster.Contains("Master") || allmaster == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = allmaster;
                }
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
            if (ddlusers == "WAdmin")
            {
                if (Whitelabel == "" || Whitelabel.Contains("All Retailer") || Whitelabel == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = Whitelabel;
                }
            }
            if (ddlusers == "APIID")
            {
                if (allapiuser == "" || allapiuser.Contains("All API") || Whitelabel == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = allapiuser;
                }
            }
            if (ddlusers == "Admin")
            {
                userid = "ALL";
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
            jsonmodel.HTMLString = renderPartialViewtostring("_TokenPurchaseReport", tbrow);
            return Json(jsonmodel);
        }

        [HttpPost]
        [PermissioncheckingAttribute(servicename = "PANCARD", permision = "Read")]
        public ActionResult Token_report_View(int Idno)
        {
            var detail = db.PAN_CARD_IPAY_Token_report_View(Idno);
            return Json(detail, JsonRequestBehavior.AllowGet);
        }
        [PermissioncheckingAttribute(servicename = "PANCARD", permision = "Read")]
        public virtual ActionResult ExcelRechargereport1(string txt_frm_date, string txt_to_date, string ddlusers, string ddl_status, string allmaster, string alldealer, string allretailer, string allapiuser, string Whitelabel, string ddl_Type)
        {

            var userid = User.Identity.GetUserId();
            if (ddlusers == null || ddlusers == "")
            {

                ddlusers = "Admin";

            }


            if (ddlusers == "Master")
            {
                if (allmaster == "" || allmaster.Contains("Master") || allmaster == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = allmaster;
                }
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
            if (ddlusers == "WAdmin")
            {
                if (Whitelabel == "" || Whitelabel.Contains("All Retailer") || Whitelabel == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = Whitelabel;
                }
            }
            if (ddlusers == "APIID")
            {
                if (allapiuser == "" || allapiuser.Contains("All API") || Whitelabel == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = allapiuser;
                }
            }
            if (ddlusers == "Admin")
            {
                userid = "ALL";
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
            dt2.Columns.Add("Merchant Id", typeof(string));
            dt2.Columns.Add("Physical", typeof(string));
            dt2.Columns.Add("Digital", typeof(string));
            dt2.Columns.Add("Processing Fees", typeof(string));
            dt2.Columns.Add("UTI TXN ID", typeof(string));
            dt2.Columns.Add("User Income", typeof(string));
            dt2.Columns.Add("User Remain Post", typeof(string));
            dt2.Columns.Add("User Remain Pre", typeof(string));
            dt2.Columns.Add("Distributor Remain Pre", typeof(string));
            dt2.Columns.Add("Distributor Income", typeof(string));
            dt2.Columns.Add("Distributor Remain Post", typeof(string));
            dt2.Columns.Add("MD Remain Pre", typeof(string));
            dt2.Columns.Add("MD Income", typeof(string));
            dt2.Columns.Add("MD Remain Post", typeof(string));
            dt2.Columns.Add("Date", typeof(string));


            var respo = db.PAN_CARD_IPAY_Token_report_paging(1, 10000000, userid, ddlusers, ddl_status, frm_date, to_date).ToList();
            if (respo.Count > 0)
            {
                foreach (var item in respo)
                {
                    var sts = item.Particulars;

                    if (item.Particulars.Contains("Refund"))
                    {
                        sts = "Refund";
                    }
                    dt2.Rows.Add(sts + "" + item.UserType + "=" + item.Frm_Name, item.MerchantTxnId, item.PhysicalCount, item.DigitalCount,
                        item.ProcessingFees, item.UTI_TXN_ID,
                    item.RetailerIncome, item.retailer_remain_post, item.retailer_remain_pre, item.retailer_dealer_pre, item.DealerIncome, item.retailer_dealer_post,
                    item.retailer_master_pre, item.MDIncome, item.retailer_master_post, item.Date);
                }
            }

            else
            {
                dt2.Rows.Add("", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "");
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
        [PermissioncheckingAttribute(servicename = "PANCARD", permision = "Read")]
        public ActionResult PDFRechargereport_token(string txt_frm_date, string txt_to_date, string ddlusers, string ddl_status, string allmaster, string alldealer, string allretailer, string allapiuser, string Whitelabel, string ddl_Type)
        {
            var userid = User.Identity.GetUserId();
            if (ddlusers == null || ddlusers == "")
            {

                ddlusers = "Admin";

            }


            if (ddlusers == "Master")
            {
                if (allmaster == "" || allmaster.Contains("Master") || allmaster == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = allmaster;
                }
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
            if (ddlusers == "WAdmin")
            {
                if (Whitelabel == "" || Whitelabel.Contains("All Retailer") || Whitelabel == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = Whitelabel;
                }
            }
            if (ddlusers == "APIID")
            {
                if (allapiuser == "" || allapiuser.Contains("All API") || Whitelabel == null)
                {
                    userid = "ALL";
                }
                else
                {
                    userid = allapiuser;
                }
            }
            if (ddlusers == "Admin")
            {
                userid = "ALL";
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
            var respo = db.PAN_CARD_IPAY_Token_report_paging(1, 10000000, userid, ddlusers, ddl_status, frm_date, to_date).ToList();
            return new ViewAsPdf(respo);
        }
        [PermissioncheckingAttribute(servicename = "PANCARD", permision = "Read")]
        public ActionResult TokenPurchaseReport_Total(string ddlusers, string allmaster, string alldealer, string allretailer, string allapiuser, string Whitelabel, string ddl_status, string txt_frm_date, string txt_to_date)
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {

                if (txt_frm_date == null && txt_to_date == null && ddlusers == null)
                {

                    txt_frm_date = DateTime.Now.ToString();
                    txt_to_date = DateTime.Now.ToString();
                    ddlusers = "Admin";
                }

                var userid = User.Identity.GetUserId();
                if (ddlusers == "Master")
                {
                    if (allmaster == "" || allmaster.Contains("Master") || allmaster == null)
                    {
                        userid = "ALL";
                    }
                    else
                    {
                        userid = allmaster;
                    }
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
                if (ddlusers == "WAdmin")
                {
                    if (Whitelabel == "" || Whitelabel.Contains("All Retailer") || Whitelabel == null)
                    {
                        userid = "ALL";
                    }
                    else
                    {
                        userid = Whitelabel;
                    }
                }
                if (ddlusers == "APIID")
                {
                    if (allapiuser == "" || allapiuser.Contains("All API") || Whitelabel == null)
                    {
                        userid = "ALL";
                    }
                    else
                    {
                        userid = allapiuser;
                    }
                }
                if (ddlusers == "Admin")
                {
                    userid = "ALL";
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
        }

        [HttpPost]
        [PermissioncheckingAttribute(servicename = "PANCARD", permision = "Write")]
        public ActionResult GetTokenStatus(string id)
        {
            var userid = User.Identity.GetUserId();
            RETAILER.Controllers.InstantPayComnUtil util = new RETAILER.Controllers.InstantPayComnUtil();
            var token = string.Empty;
            token = Responsetoken.gettoken();
            if (string.IsNullOrWhiteSpace(token))
            {
                var obj = new { RESULT = "1", ADDINFO = "Authentical Failed." };
                return Json(JsonConvert.SerializeObject(obj), JsonRequestBehavior.AllowGet);
            }
            var response = util.getUtiTokenStatus(id, token);
            return Json(response.ToString());
        }
        [HttpPost]
        [PermissioncheckingAttribute(servicename = "PANCARD", permision = "Write")]
        public ActionResult PanManualFailed(string id, string remark)
        {
            try
            {
                RETAILER.Controllers.InstantPayComnUtil util = new RETAILER.Controllers.InstantPayComnUtil();
                util.PanManualFailed(id, remark);
                var obj = new { RESULT = "0", ADDINFO = "Manually failed successfully." };
                return Json(JsonConvert.SerializeObject(obj), JsonRequestBehavior.AllowGet);
            }
            catch
            {
                var obj = new { RESULT = "1", ADDINFO = "Error occured." };
                return Json(JsonConvert.SerializeObject(obj), JsonRequestBehavior.AllowGet);
            }
        }


        #endregion

        #region MicroATM
        [PermissioncheckingAttribute(servicename = "MICROATM", permision = "Read")]
        public ActionResult MIcroAtmReport()
        {
            var stands = db.Superstokist_details.ToList();
            IEnumerable<SelectListItem> selectList = from s in stands
                                                     select new SelectListItem
                                                     {
                                                         Value = s.SSId,
                                                         Text = s.FarmName.ToString()
                                                     };
            ViewBag.allmaster = new SelectList(selectList, "Value", "Text");
            //show dealer
            ViewBag.alldealer = new SelectList(db.select_Dealer_for_ddl(), "Dealerid", "FarmName", null).ToList();
            //Retailer 
            ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl("Admin"), "RetailerId", "Frm_Name", null).ToList();
            var totalwhitelabel = db.WhiteLabel_userList.ToList();
            IEnumerable<SelectListItem> selectList1 = from s in totalwhitelabel
                                                      select new SelectListItem
                                                      {
                                                          Value = s.WhiteLabelID,
                                                          Text = s.FrmName.ToString()
                                                      };
            ViewBag.allwhitelabel = new SelectList(selectList1, "Value", "Text");
            DateTime from = DateTime.Now.Date;
            DateTime to = from.AddDays(1);

            var chk = db.microatm_report("Admin", "Admin", from, to, "");
            return View(chk);
        }

        [HttpPost]
        [PermissioncheckingAttribute(servicename = "MICROATM", permision = "Read")]
        public ActionResult MIcroAtmReport(string ddlusers, string allwhitelabel1, string allmaster2, string alldealer, string allretailer, string ddl_status, DateTime txt_frm_date, DateTime txt_to_date)
        {
            ViewBag.chk = "post";
            var userid = "";
            if (ddlusers == "Admin")
            {
                userid = "Admin";
            }
            else if (ddlusers == "Master")
            {
                userid = allmaster2;
            }
            else if (ddlusers == "Dealer")
            {
                userid = alldealer;
            }
            else if (ddlusers == "Retailer")
            {
                userid = allretailer;
            }
            var stands = db.Superstokist_details.ToList();
            IEnumerable<SelectListItem> selectList = from s in stands
                                                     select new SelectListItem
                                                     {
                                                         Value = s.SSId,
                                                         Text = s.FarmName.ToString()
                                                     };
            ViewBag.allmaster = new SelectList(selectList, "Value", "Text");
            //show dealer
            ViewBag.alldealer = new SelectList(db.select_Dealer_for_ddl(), "Dealerid", "FarmName", null).ToList();
            //Retailer 
            ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl("Admin"), "RetailerId", "Frm_Name", null).ToList();
            var totalwhitelabel = db.WhiteLabel_userList.ToList();
            IEnumerable<SelectListItem> selectList1 = from s in totalwhitelabel
                                                      select new SelectListItem
                                                      {
                                                          Value = s.WhiteLabelID,
                                                          Text = s.FrmName.ToString()
                                                      };
            ViewBag.allwhitelabel = new SelectList(selectList1, "Value", "Text");

            DateTime to = txt_to_date.AddDays(1);
            var chk = db.microatm_report(ddlusers, userid, txt_frm_date, to, ddl_status);
            return View(chk);
        }

        public ActionResult MICROATMEXCELLGENERATE(string ddlusers, string allwhitelabel1, string allmaster2, string alldealer, string allretailer, string ddl_status, DateTime txt_frm_date, DateTime txt_to_date)
        {
            var userid = "";
            if (ddlusers == "Admin")
            {
                userid = "Admin";
            }
            else if (ddlusers == "Master")
            {
                userid = allmaster2;
            }
            else if (ddlusers == "Dealer")
            {
                userid = alldealer;
            }
            else if (ddlusers == "Retailer")
            {
                userid = allretailer;
            }

            DateTime to = txt_to_date.AddDays(1);
            dynamic dddd;
            if (string.IsNullOrEmpty(ddlusers))
            {
                dddd = db.microatm_report("Admin", "Admin", txt_frm_date, to, "");
            }
            else
            {
                dddd = db.microatm_report(ddlusers, userid, txt_frm_date, to, ddl_status);
            }

            var chk = dddd;



            DataTable dataTbl = new DataTable();
            dataTbl.Columns.Add("Firm Name", typeof(string));
            dataTbl.Columns.Add("Status", typeof(string));
            dataTbl.Columns.Add("Amount", typeof(string));
            dataTbl.Columns.Add("Time", typeof(string));
            dataTbl.Columns.Add("BankRRN", typeof(string));
            dataTbl.Columns.Add("TranType", typeof(string));
            dataTbl.Columns.Add("Card Type", typeof(string));
            dataTbl.Columns.Add("Card No", typeof(string));
            dataTbl.Columns.Add("Net Work", typeof(string));
            dataTbl.Columns.Add("Remain Pre", typeof(string));
            dataTbl.Columns.Add("Comm", typeof(string));
            dataTbl.Columns.Add("GST", typeof(string));
            dataTbl.Columns.Add("TDS", typeof(string));
            dataTbl.Columns.Add("Remain Post", typeof(string));

            foreach (var item in chk)
            {
                decimal comm = 0;
                var transtype = "";
                if (item.status.Contains("success"))
                {
                    if (item.transaction_type == "microatm")
                    {
                        comm = (item.amount * item.Retailer_comm) / 100;
                        comm = comm + item.Retailer_gst + item.Retailer_tds;
                        transtype = "-cm";

                    }
                    else if (item.transaction_type == "purchase")
                    {
                        comm = (item.amount * item.Retailer_comm) / 100;
                        //  comm = comm - item.Retailer_gst;
                        transtype = "-ch";
                    }
                }
                dataTbl.Rows.Add(item.Frm_Name, item.status, item.amount, item.transtime, item.rrn, item.transaction_type, item.card_payment_type, item.masked_pan, item.network, item.retailer_remain_pre, comm + "" + transtype, item.Retailer_gst,
                item.Retailer_tds, item.retailer_remain_post);
            }

            var grid = new GridView();
            grid.DataSource = dataTbl;
            grid.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=MICROATMREPORTS.xls");
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

        //Manual Failed Money Transfer
        [PermissioncheckingAttribute(servicename = "MICROATM", permision = "Write")]
        public ActionResult microatmsuccesstofailed(string txtrefidno, string secuirtypass)
        {
            try
            {
                var AdminEmail = db.Admin_details.Single().email;
                var comm = db.MicroAtm_Trans_info.Where(p => p.rrn == txtrefidno && p.status.ToUpper() == "SUCCESS").SingleOrDefault();
                if (comm != null)
                {
                    var password = Encrypt(secuirtypass);
                    var tranpass = (from paa in db.admin_new_pass where paa.securitypass == password select paa).Count();
                    if (tranpass > 0)
                    {
                        db.microatmSuccess_to_failed(txtrefidno, "Failed");
                        return Json(new { Status = true, Message = "Failed Successfully" });
                    }
                    else
                    {
                        return Json(new { Status = false, Message = "Your Security Password is Wrong!" });
                    }
                    /****************************************************************************************************************************************************/
                }
                else
                {
                    return Json(new { Status = false, Message = "Something Went Wrong" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, Message = ex.Message });
            }
        }



        [HttpPost]
        [PermissioncheckingAttribute(servicename = "MICROATM", permision = "Read")]
        public ActionResult Microatm_View(int Idno)
        {
            var detail = db.microatm_view(Idno);
            return Json(detail, JsonRequestBehavior.AllowGet);
        }



        #endregion



        #region WalletToBankAmountTransfer
        [HttpGet]
        [PermissioncheckingAttribute(servicename = "WALLETUNLOAD", permision = "Read")]
        public ActionResult WalletUnloadReport(string type)
        {
            if (type == null)
            {
                type = "ALL";
            }
            string txt_frm_date = DateTime.Now.ToString();
            string txt_to_date = DateTime.Now.ToString();
            string frm_date = Convert.ToDateTime(txt_frm_date).AddDays(-30).Date.ToString("yyyy-MM-dd");
            string to_date = Convert.ToDateTime(txt_to_date).AddDays(1).ToString("yyyy-MM-dd");
            ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl("Admin"), "RetailerId", "Frm_Name", null);
            var entries = db.WalletUnloadReportAdmin(Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), type, "ALL", "ALL").ToList();
            return View(entries);
        }

        [HttpPost]
        [PermissioncheckingAttribute(servicename = "WALLETUNLOAD", permision = "Read")]
        public ActionResult WalletUnloadReport(string ddlusers, string allretailer, string ddl_status, DateTime txt_frm_date, DateTime txt_to_date)
        {
            var userid = "ALL";
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

            ViewBag.chk = "post";
            ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl("Admin"), "RetailerId", "Frm_Name", null).ToList();

            DateTime to_date = Convert.ToDateTime(txt_to_date).Date.AddDays(1);

            var entries = db.WalletUnloadReportAdmin(Convert.ToDateTime(txt_frm_date), Convert.ToDateTime(to_date), ddlusers, userid, ddl_status).ToList();
            return View(entries);
        }
        public ActionResult pdfWalletUnloadReport(string ddlusers, string allretailer, string ddl_status, DateTime txt_frm_date, DateTime txt_to_date)
        {
            var userid = "ALL";
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

            ViewBag.chk = "post";
            ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl("Admin"), "RetailerId", "Frm_Name", null).ToList();

            DateTime to_date = Convert.ToDateTime(txt_to_date).Date.AddDays(1);

            var entries = db.WalletUnloadReportAdmin(Convert.ToDateTime(txt_frm_date), Convert.ToDateTime(to_date), ddlusers, userid, ddl_status).ToList();
            return new ViewAsPdf(entries);
        }

        public ActionResult ExcelWalletUnloadReport(string ddlusers, string allretailer, string ddl_status, DateTime txt_frm_date, DateTime txt_to_date)
        {
            var userid = "ALL";
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

            ViewBag.chk = "post";
            ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl("Admin"), "RetailerId", "Frm_Name", null).ToList();

            DateTime to_date = Convert.ToDateTime(txt_to_date).Date.AddDays(1);

            var entries = db.WalletUnloadReportAdmin(Convert.ToDateTime(txt_frm_date), Convert.ToDateTime(to_date), ddlusers, userid, ddl_status).ToList();

            DataTable dataTbl = new DataTable();
            dataTbl.Columns.Add("Role", typeof(string));
            dataTbl.Columns.Add("Firm Name", typeof(string));
            dataTbl.Columns.Add("Status", typeof(string));
            dataTbl.Columns.Add("Transfer Type", typeof(string));
            dataTbl.Columns.Add("Bank Name", typeof(string));
            dataTbl.Columns.Add("Remain Pre", typeof(string));
            dataTbl.Columns.Add("Amount", typeof(string));
            dataTbl.Columns.Add("Charge", typeof(string));
            dataTbl.Columns.Add("Remain Post", typeof(string));
            dataTbl.Columns.Add("Request Time", typeof(string));
            dataTbl.Columns.Add("Response Time", typeof(string));
            dataTbl.Columns.Add("Bank RRN", typeof(string));


            decimal total = 0;
            foreach (var item in entries)
            {
                total += item.Amount ?? 0;
                dataTbl.Rows.Add(item.RoleName, item.FirmName, item.Status, item.TransferType, item.BankName, item.Rempre, item.Amount, item.ProcessingCharge, item.RemPost, item.RequestDate, item.ResponseDate, item.BankRRN);
            }
            dataTbl.Rows.Add("Total", "", "", "", "", "", total, "", "", "", "", "");

            var grid = new GridView();
            grid.DataSource = dataTbl;
            grid.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=Wallet_Unload_Report.xls");
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

        [PermissioncheckingAttribute(servicename = "WALLETUNLOAD", permision = "Write")]
        public ActionResult GiftcardFailed(string orderid, string status, string ddl_refund)
        {
            try
            {
                if (orderid != "")
                {
                    db.giftcard_purchase_update(orderid, status, "", "", "", null, 0, 0, ddl_refund);
                    return Json(new { Status = true, Message = "Successfully" });
                }
                else
                {
                    return Json(new { Status = false, Message = "someting wrong." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, Message = ex.Message });
            }

        }

        [PermissioncheckingAttribute(servicename = "WALLETUNLOAD", permision = "Write")]
        public ActionResult GiftcardSuccess(string orderid, string status)
        {
            try
            {
                if (orderid != "")
                {
                    db.giftcard_purchase_update(orderid, status, "", "", "", null, 0, 0, "Mannual Success");
                    return Json(new { Status = true, Message = "Successfully" });
                }
                else
                {
                    return Json(new { Status = false, Message = "someting wrong." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, Message = ex.Message });
            }

        }


        #endregion













        #region securityservices

        [PermissioncheckingAttribute(servicename = "SECURITYSERVICE", permision = "Read")]
        public ActionResult SecurityReport()
        {
            DateTime from = DateTime.Now.Date;
            DateTime to = DateTime.Now.Date.AddDays(1);
            ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl("Admin"), "RetailerId", "Frm_Name", null);
            var ch = db.history_norton("Admin", "", from, to, "").ToList();
            return View(ch);
        }
        [HttpPost]
        [PermissioncheckingAttribute(servicename = "SECURITYSERVICE", permision = "Read")]
        public ActionResult SecurityReport(string ddlusers, string allretailer, string ddl_status, DateTime txt_frm_date, DateTime txt_to_date)
        {
            ViewBag.chk = "post";
            DateTime to = txt_to_date.Date.AddDays(1);
            ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl("Admin"), "RetailerId", "Frm_Name", null);
            var ch = db.history_norton(ddlusers, allretailer, txt_frm_date, to, ddl_status).ToList();
            return View(ch);
        }
        public ActionResult ExcelSecurityReport(string ddlusers, string allretailer, string ddl_status, DateTime txt_frm_date, DateTime txt_to_date)
        {
            ViewBag.chk = "post";
            DateTime to = txt_to_date.Date.AddDays(1);
            ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl("Admin"), "RetailerId", "Frm_Name", null);
            var ch = db.history_norton(ddlusers, allretailer, txt_frm_date, to, ddl_status).ToList();

            DataTable dataTbl = new DataTable();
            dataTbl.Columns.Add("Status", typeof(string));
            dataTbl.Columns.Add("Request ID", typeof(string));
            dataTbl.Columns.Add("Plan Name", typeof(string));
            dataTbl.Columns.Add("User Firm", typeof(string));
            dataTbl.Columns.Add("Amount", typeof(string));
            dataTbl.Columns.Add("Income", typeof(string));
            dataTbl.Columns.Add("Pre Amt", typeof(string));
            dataTbl.Columns.Add("Cr/Dr", typeof(string));
            dataTbl.Columns.Add("post Amt", typeof(string));
            dataTbl.Columns.Add("Operator ID", typeof(string));
            dataTbl.Columns.Add("Date Time", typeof(string));

            if (ch.Count > 0)
            {
                foreach (var item in ch)
                {
                    decimal final = Convert.ToDecimal(item.amount) - Convert.ToDecimal(item.total_rem_pay);
                    dataTbl.Rows.Add(item.Status, item.req_id, item.plan_nm, item.Frm_Name, item.amount, item.total_rem_pay, item.rempre, @final, item.rempost, item.OPerator_id, item.transfer_time);
                }
            }
            else
            {
                dataTbl.Rows.Add("", "", "", "", "", "", "", "", "", "", "");
            }

            var grid = new GridView();
            grid.DataSource = dataTbl;
            grid.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=Security_Report.xls");
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
        public ActionResult PDFSecurityReport(string ddlusers, string allretailer, string ddl_status, DateTime txt_frm_date, DateTime txt_to_date)
        {
            ViewBag.chk = "post";
            DateTime to = txt_to_date.Date.AddDays(1);
            ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl("Admin"), "RetailerId", "Frm_Name", null);
            var ch = db.history_norton(ddlusers, allretailer, txt_frm_date, to, ddl_status).ToList();
            return new ViewAsPdf(ch);
        }


        //[HttpPost]
        //[ValidateInput(false)]
        //public FileResult Exportdata(string ExportData)
        //{
        //    using (MemoryStream stream = new System.IO.MemoryStream())
        //    {
        //        try
        //        {
        //            StringReader sr = new StringReader(ExportData);
        //            Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 100f, 0f);


        //            PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
        //            pdfDoc.Open();
        //            XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
        //            pdfDoc.Close();
        //            return File(stream.ToArray(), "application/pdf", "Grid.pdf");
        //        }
        //        catch (Exception ex)
        //        {

        //            return File(stream.ToArray(), "application/pdf", "Grid.pdf");


        //        }
        //    }

        //}


        [PermissioncheckingAttribute(servicename = "SECURITYSERVICE", permision = "Write")]
        public ActionResult SecurityManually(string orderid, string status)
        {
            db.update_norton(orderid, status, "", "", "", "", "");
            return Json("", JsonRequestBehavior.AllowGet);
        }


        #endregion

        #region FundTransfer

        [PermissioncheckingAttribute(servicename = "FUNDTRANSFER", permision = "Read")]
        public ActionResult Fund_transfer()
        {

            EmployeFundtransferviewmodel vmodel = new EmployeFundtransferviewmodel();
            List<SelectListItem> items = new List<SelectListItem>();

            foreach (var item in db.Superstokist_details)
            {
                items.Add(new SelectListItem
                {

                    Text = item.FarmName + " - " + item.Mobile.ToString(),
                    Value = item.SSId.ToString()
                });
            }
            vmodel.ddlSuperstokistdetails = items;
            var query = db.bank_info.Where(a => a.userid == "Admin").Select(c => new SelectListItem
            {
                Value = c.acno.ToString(),
                Text = c.banknm,

            });

            vmodel.ddlbankinfo = query.AsEnumerable();
            var fillwallet = db.tblwallet_info.Where(a => a.userid == "Admin").Select(aa => new SelectListItem
            {
                Text = aa.walletname,
                Value = aa.walletno.ToString(),

            });
            vmodel.ddlwalletInfo = fillwallet.AsEnumerable();
            vmodel.Superstocklistbalance = db.Select_balance_Super_stokist("ALL", Convert.ToDateTime(DateTime.Now.AddDays(-1)), Convert.ToDateTime(DateTime.Now.AddDays(1))).ToList();
            vmodel.admintodealerllist = null;
            return View(vmodel);
        }

        [PermissioncheckingAttribute(servicename = "FUNDTRANSFER", permision = "Write")]
        public JsonResult Fundtransfer_Admin_to_MD_Generate_Unique_ID()
        {
            var adminuserid = User.Identity.GetUserId();

            string transferids = GenerateUniqueTransectionID();
            var transferid = "AM" + transferids;

            TempData["transferatom"] = transferid;
            return Json(transferid, JsonRequestBehavior.AllowGet);
        }

        [PermissioncheckingAttribute(servicename = "FUNDTRANSFER", permision = "Write")]
        public JsonResult FundtransferAdmin_To_dealer_Generate_Unique_ID()
        {
            var adminuserid = User.Identity.GetUserId();

            string transferids = GenerateUniqueTransectionID();
            var transferid = "AD" + transferids;



            TempData["transferadmintodealer"] = transferid;
            return Json(transferid, JsonRequestBehavior.AllowGet);
        }

        [PermissioncheckingAttribute(servicename = "FUNDTRANSFER", permision = "Write")]
        public ActionResult ChkSecurityPass(string txtcode, string hdSuperstokistID, string hdPaymentMode,
   string hdPaymentAmount, string hdMDDepositeSlipNo, string hdMDTransferType, string hdMDcollection, string hdMDComments,
   string hdMDBank, string hdsupraccno, string hdMDaccountno, string hdMDutrno, string hdMDwallet,
   string hdMDwalletno, string hdMDtransationno, string hdMDsettelment, string hdMDCreditDetail, string hdMDsubject)
        {
            try
            {
                var ch = "";
                var userid = User.Identity.GetUserId();
                var adminemail = db.Admin_details.Single().email;
                TempData.Keep("dlmid");
                TempData.Keep("bal");
                TempData.Keep("fundtype");
                TempData.Keep("comment");
                var password = Encrypt(txtcode);
                var tranpass = (from paa in db.admin_new_pass where paa.transpass == password select paa).Count();
                if (tranpass > 0)
                {



                    string DealerId = hdSuperstokistID;
                    var dealeremail = db.Users.Where(p => p.UserId == DealerId).Single().Email;
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
                    string bankname = hdMDBank == null ? hdMDwallet : hdMDBank;
                    string adminacco = hdMDaccountno == null ? hdMDwalletno : hdMDaccountno;
                    adminacco = adminacco == null ? hdMDDepositeSlipNo : adminacco;
                    string DepositeSlipNo = hdMDDepositeSlipNo;
                    if (hdMDTransferType != null && hdPaymentMode == "Online Transfer")
                    {
                        type = hdPaymentMode + "/" + hdMDTransferType;
                    }
                    var Role = db.showrole(DealerId).SingleOrDefault();
                    System.Data.Entity.Core.Objects.ObjectParameter output = new
                    System.Data.Entity.Core.Objects.ObjectParameter("Output", typeof(string));

                    var tp = "";
                    Websitename = db.Admin_details.Single().WebsiteUrl;
                    decimal amount1 = Convert.ToDecimal(balance);

                    if (Role.Name == "master")
                    {
                        string transferid = null;
                        try
                        {
                            transferid = TempData["transferatom"].ToString();
                        }
                        catch (Exception ex)
                        {
                            return Json("Please Try After SomeTime", JsonRequestBehavior.AllowGet);
                        }


                        var counts = db.FundTransfercount(userid, DealerId, type, amount1, transferid, "Admintosuper").SingleOrDefault().msg;

                        int msgcount = Convert.ToInt32(counts);
                        if (msgcount == 0)
                        {
                            msgcount = 60001;
                        }
                        int max_limit = 60000;
                        if (msgcount > max_limit)
                        {

                            var diff1 = (db.admin_to_super_balance.Where(aa => aa.SuperStokistID == DealerId).OrderByDescending(aa => aa.ID).Select(c => c.cr).FirstOrDefault());
                            diff1 = diff1 ?? 0;

                            decimal diff = Convert.ToDecimal(diff1);
                            //if (amount1 > 0)
                            //{
                            //if (type == "Credit" || type == "Cash")
                            //{
                            //    ch = "";//db.Insert_Admin_To_SuperStokist(DealerId, Convert.ToDecimal(amount1), 0, type, comment,collectionby,bankname,adminacco, output).Single().msg;
                            //}if

                            ch = db.Insert_Admin_To_SuperStokist(DealerId, Convert.ToDecimal(amount1), type, comment, collectionby, bankname, adminacco, "Direct", transferid, output).Single().msg;
                            TempData["transferatom"] = null;
                            transferid = null;
                            if (ch == "Balance Transfer SuccessFully.")
                            {
                                diff1 = (db.admin_to_super_balance.Where(aa => aa.SuperStokistID == DealerId).OrderByDescending(aa => aa.ID).Select(c => c.cr).FirstOrDefault());
                                var remainmaster = db.Remain_superstokist_balance.Where(p => p.SuperStokistID == DealerId).Single().Remainamount;
                                var statusAdmin = db.PushNotificationStatus.Where(a => a.UserRole == "Admin").SingleOrDefault().Status;
                                var statusMaster = db.PushNotificationStatus.Where(a => a.UserRole == "Master").SingleOrDefault().Status;
                                var statusSendSmsMasterFundTransfer = db.SMSSendAlls.Where(a => a.ServiceName == "Admintomasterfundtrans").SingleOrDefault();
                                var statusSendEmailMasterFundTransfer = db.EmailSendAlls.Where(a => a.ServiceName == "Admintomasterfundtrans1").SingleOrDefault().Status;
                                var AdminToCC = db.Admin_details.SingleOrDefault().email;

                                var MasterDetails = db.Superstokist_details.Where(a => a.SSId == DealerId).Single();
                                var mastername = db.Superstokist_details.Where(p => p.SSId == DealerId).Single().SuperstokistName;
                                var Adminname = db.Admin_details.Single().Name;
                                var RemainAdmin = db.Remain_Admin_balance.Where(p => p.admin == userid).Single().RemainAmount;
                                if (type == "Credit")
                                {
                                    //if (statusSendSmsMasterFundTransfer == "Y")
                                    //{
                                    //    string msgssss = "";
                                    //    string tempid = "";
                                    //    string urlss = "";

                                    //    var smsapionsts = db.apisms.Where(x => x.sts == "Y").SingleOrDefault();
                                    //    var smsstypes = db.Sending_SMS_Templates.Where(x => x.SMS_TYPE == "FUNDTRANSFERMESSAGE_CREDIT" && x.SMSAPIID == smsapionsts.id).SingleOrDefault();
                                    //    if (smsstypes != null)
                                    //    {



                                    //        msgssss = string.Format(smsstypes.Templates, amount1, remainmaster, diff1);
                                    //        tempid = smsstypes.Templateid;
                                    //        urlss = smsapionsts.smsapi;

                                    //        smssend.sendsmsallnew(MasterDetails.Mobile, msgssss, urlss, tempid);
                                    //    }

                                    //    //  smssend.sendsmsall(MasterDetails.Mobile, "Credit Received Rs." + amount1 + ".New Balance is " + remainmaster + ".Your O/s Credit is " + diff1 + "", "Fund Transfer");
                                    //}

                                    smssend.sms_init(statusSendSmsMasterFundTransfer.Status, statusSendSmsMasterFundTransfer.Whatsapp_Status, "FUNDTRANSFERMESSAGE_CREDIT", MasterDetails.Mobile, amount1, remainmaster, diff1);

                                    if (statusSendEmailMasterFundTransfer == "Y")
                                    {
                                        smssend.SendEmailAll(MasterDetails.Email, "Credit Received Rs." + amount1 + ".New Balance is " + remainmaster + ".Your O/s Credit is " + diff1 + "", "Fund Transfer", AdminToCC, 1000);
                                    }
                                    if (statusAdmin == "Y")
                                    {
                                        SendPushNotification(adminemail, Url.Action("Admin_to_master_Dealer", "Home"), "Credit Trnf Rs. " + amount1 + "  to " + mastername + ", Pending Credit is " + diff1 + " . Bal is " + RemainAdmin + " . (" + string.Format("{0:dd-MMM hh:mm:ss}", DateTime.Now) + ")", "Fund Transfer ..");
                                    }
                                    if (statusMaster == "Y")
                                    {
                                        SendPushNotification(dealeremail, Websitename + "/Master/Home/ReceiveFund", "Credit Recd frm " + Adminname + " Rs." + amount1 + ". Your pending credit is " + diff1 + " . New Balance is " + remainmaster + ". (" + string.Format("{0:dd-MMM hh:mm:ss}", DateTime.Now) + ")", "Fund Transfer ..");
                                    }
                                    notify.sendmessage(dealeremail, "Credit Recd frm " + Adminname + " Rs." + amount1 + ". Your pending credit is " + diff1 + " . New Balance is " + remainmaster + ". (" + string.Format("{0:dd-MMM hh:mm:ss}", DateTime.Now) + ")");
                                    ch = "Fund Transfer Successful.";
                                    tp = "success";
                                    ch = "Fund Transfer Successful.";
                                    tp = "success";
                                }
                                else
                                {
                                    //if (statusSendSmsMasterFundTransfer == "Y")
                                    //{
                                    //    string msgssss = "";
                                    //    string tempid = "";
                                    //    string urlss = "";

                                    //    var smsapionsts = db.apisms.Where(x => x.sts == "Y").SingleOrDefault();
                                    //    var smsstypes = db.Sending_SMS_Templates.Where(x => x.SMS_TYPE == "FUNDTRANSFERMESSAGE_CASH" && x.SMSAPIID == smsapionsts.id).SingleOrDefault();
                                    //    if (smsstypes != null)
                                    //    {
                                    //        msgssss = string.Format(smsstypes.Templates, amount1, Adminname, remainmaster, diff1);
                                    //        tempid = smsstypes.Templateid;
                                    //        urlss = smsapionsts.smsapi;

                                    //        smssend.sendsmsallnew(MasterDetails.Mobile, msgssss, urlss, tempid);
                                    //    }
                                    //    //  smssend.sendsmsall(MasterDetails.Mobile, "Cash Paid Rs." + amount1 + " to " + Adminname + ". New Balance is " + remainmaster + ". Your O/s Credit is " + diff1 + "", "Fund Transfer");
                                    //}

                                    smssend.sms_init(statusSendSmsMasterFundTransfer.Status, statusSendSmsMasterFundTransfer.Whatsapp_Status, "FUNDTRANSFERMESSAGE_CASH", MasterDetails.Mobile, amount1, Adminname, remainmaster, diff1);

                                    if (statusSendEmailMasterFundTransfer == "Y")
                                    {
                                        smssend.SendEmailAll(MasterDetails.Mobile, "Cash Paid Rs." + amount1 + " to " + Adminname + ". New Balance is " + remainmaster + ". Your O/s Credit is " + diff1 + "", "Fund Transfer", AdminToCC, 1000);
                                    }
                                    if (statusAdmin == "Y")
                                    {
                                        SendPushNotification(adminemail, Url.Action("Admin_to_master_Dealer", "Home"), "Cash Received Rs." + amount1 + " Frm " + mastername + ",Pending Credit is " + diff1 + ". (" + string.Format("{0:dd-MMM hh:mm:ss}", DateTime.Now) + ")", "Fund Transfer ..");
                                    }
                                    if (statusMaster == "Y")
                                    {
                                        SendPushNotification(dealeremail, Websitename + "/Master/Home/ReceiveFund", "Cash Paid Rs." + amount1 + " to " + Adminname + ". A/c Bal is " + remainmaster + ". Your Pending Credit is " + diff1 + ". (" + string.Format("{0:dd-MMM hh:mm:ss}", DateTime.Now) + ")", "Fund Transfer ..");
                                    }
                                    notify.sendmessage(dealeremail, "Cash Paid Rs." + amount1 + " to " + Adminname + ". A/c Bal is " + remainmaster + ". Your Pending Credit is " + diff1 + ". (" + string.Format("{0:dd-MMM hh:mm:ss}", DateTime.Now) + ")");
                                    ch = "Fund Transfer Successful.";
                                    tp = "success";
                                }
                            }
                            //}
                            //else
                            //{
                            //    ch = "Amount should be not zero";
                            //    tp = "error";
                            //}

                            TempData["BalanceTransferMsg"] = ch;
                            TempData["BalanceTransfertype"] = tp;
                            TempData.Remove("dlmid");
                            TempData.Remove("bal");
                            TempData.Remove("dl_commission");
                            TempData.Remove("comment");
                            return Json(ch, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json("Your Previous Request IN Process Please Try After SomeTime", JsonRequestBehavior.AllowGet);
                        }
                        //  return RedirectToAction("Admin_to_master_Dealer");
                    }
                    else
                    {
                        string transferid = null;
                        try
                        {
                            transferid = TempData["transferadmintodealer"].ToString();
                        }
                        catch (Exception ex)
                        {
                            return Json("Please Try After SomeTime", JsonRequestBehavior.AllowGet);
                        }

                        var counts = db.FundTransfercount(userid, DealerId, type, amount1, transferid, "Admintodealer").SingleOrDefault().msg;
                        int msgcount = Convert.ToInt32(counts);
                        if (msgcount == 0)
                        {
                            msgcount = 60001;
                        }
                        int max_limit = 60000;
                        if (msgcount > max_limit)
                        {

                            decimal amount = Convert.ToDecimal(balance);
                            //if (amount > 0)
                            //{
                            //if (type == "Credit" || type == "Cash")
                            //{
                            //    ch = db.admin_to_dealer_balance(DealerId, amount, Convert.ToDecimal(0), type, comment, output).Single().msg;
                            //}
                            ch = db.admin_to_dealer_balance(DealerId, amount, type, comment, "Admin", collectionby, bankname, adminacco, "Direct", transferid, output).Single().msg;

                            if (ch == "ok")
                            {
                                var diff2 = (db.admin_to_dealer.Where(aa => aa.dealerid == DealerId).OrderByDescending(aa => aa.idno).Select(c => c.cr).FirstOrDefault());
                                var remaindealer = db.Remain_dealer_balance.Where(p => p.DealerID == DealerId).Single().Remainamount;
                                var statusAdmin = db.PushNotificationStatus.Where(a => a.UserRole == "Admin").SingleOrDefault().Status;
                                var statusDealer = db.PushNotificationStatus.Where(a => a.UserRole == "Distributor").SingleOrDefault().Status;
                                var statusSendSmsDealerFundTransfer = db.SMSSendAlls.Where(a => a.ServiceName == "Admintodlmfundtrans").SingleOrDefault();
                                var statusSendEmailDealerFundTransfer = db.EmailSendAlls.Where(a => a.ServiceName == "Admintodlmfundtrans1").SingleOrDefault().Status;


                                var DealerDetails = db.Dealer_Details.Where(a => a.DealerId == DealerId).Single();
                                var Dealername = db.Dealer_Details.Where(p => p.DealerId == DealerId).Single().DealerName;
                                var AdminDetails = db.Admin_details.Single();
                                var RemainAdmin = db.Remain_Admin_balance.Where(p => p.admin == userid).Single().RemainAmount;
                                if (type == "Credit")
                                {
                                    //if (statusSendSmsDealerFundTransfer == "Y")
                                    //{
                                    //    string msgssss = "";
                                    //    string tempid = "";
                                    //    string urlss = "";

                                    //    var smsapionsts = db.apisms.Where(x => x.sts == "Y").SingleOrDefault();
                                    //    var smsstypes = db.Sending_SMS_Templates.Where(x => x.SMS_TYPE == "FUNDTRANSFERMESSAGE_CREDIT" && x.SMSAPIID == smsapionsts.id).SingleOrDefault();
                                    //    if (smsstypes != null)
                                    //    {
                                    //        msgssss = string.Format(smsstypes.Templates, amount1, remaindealer, diff2);
                                    //        tempid = smsstypes.Templateid;
                                    //        urlss = smsapionsts.smsapi;

                                    //        smssend.sendsmsallnew(DealerDetails.Mobile, msgssss, urlss, tempid);
                                    //    }
                                    //    //   smssend.sendsmsall(DealerDetails.Mobile, "Credit Received Rs." + amount + ".New Balance is " + remaindealer + ".Your O/s Credit is " + diff2 + "", "Fund Transfer");
                                    //}

                                    smssend.sms_init(statusSendSmsDealerFundTransfer.Status, statusSendSmsDealerFundTransfer.Whatsapp_Status, "FUNDTRANSFERMESSAGE_CASH", DealerDetails.Mobile, amount1, remaindealer, diff2);

                                    if (statusSendEmailDealerFundTransfer == "Y")
                                    {
                                        smssend.SendEmailAll(DealerDetails.Email, "Credit Received Rs." + amount + ".New Balance is " + remaindealer + ".Your O/s Credit is " + diff2 + "", "Fund Transfer", AdminDetails.email);
                                    }
                                    if (statusAdmin == "Y")
                                    {
                                        SendPushNotification(adminemail, Url.Action("Admin_to_Dealer", "Home"), "Credit Trnf Rs. " + amount + "  to " + Dealername + ", Pending Credit is " + diff2 + " . Bal is " + RemainAdmin + " . (" + string.Format("{0:dd-MMM hh:mm:ss}", DateTime.Now) + ")", "Fund Transfer ..");
                                    }
                                    if (statusDealer == "Y")
                                    {
                                        SendPushNotification(dealeremail, Websitename + "/DEALER/Home/ReceiveFund", "Credit Recd frm " + AdminDetails.Name + " Rs." + amount + ". Your pending credit is " + diff2 + " . New Balance is " + remaindealer + ". (" + string.Format("{0:dd-MMM hh:mm:ss}", DateTime.Now) + ")", "Fund Transfer ..");
                                    }
                                    notify.sendmessage(dealeremail, "Credit Recd frm " + AdminDetails.Name + " Rs." + amount + ". Your pending credit is " + diff2 + " . New Balance is " + remaindealer + ". (" + string.Format("{0:dd-MMM hh:mm:ss}", DateTime.Now) + ")");
                                    ch = "Fund Transfer Successful.";
                                    tp = "success";
                                }
                                else
                                {
                                    var dealername = db.Dealer_Details.Where(p => p.DealerId == DealerId).Single().DealerName;
                                    //var Adminname = db.Admin_details.Single().Name;
                                    //if (statusSendSmsDealerFundTransfer == "Y")
                                    //{
                                    //    string msgssss = "";
                                    //    string tempid = "";
                                    //    string urlss = "";

                                    //    var smsapionsts = db.apisms.Where(x => x.sts == "Y").SingleOrDefault();
                                    //    var smsstypes = db.Sending_SMS_Templates.Where(x => x.SMS_TYPE == "FUNDTRANSFERMESSAGE_CASH" && x.SMSAPIID == smsapionsts.id).SingleOrDefault();
                                    //    if (smsstypes != null)
                                    //    {
                                    //        msgssss = string.Format(smsstypes.Templates, amount1, AdminDetails.Name, remaindealer, diff2);
                                    //        tempid = smsstypes.Templateid;
                                    //        urlss = smsapionsts.smsapi;

                                    //        smssend.sendsmsallnew(DealerDetails.Mobile, msgssss, urlss, tempid);
                                    //    }
                                    //    // smssend.sendsmsall(DealerDetails.Mobile, "Cash Paid Rs." + amount + " to " + AdminDetails.Name + ". New Balance is " + remaindealer + ". Your O/s Credit is " + diff2 + "", "Fund Transfer");
                                    //}

                                    smssend.sms_init(statusSendSmsDealerFundTransfer.Status, statusSendSmsDealerFundTransfer.Whatsapp_Status, "FUNDTRANSFERMESSAGE_CASH", DealerDetails.Mobile, amount1, AdminDetails.Name, remaindealer, diff2);

                                    if (statusSendEmailDealerFundTransfer == "Y")
                                    {
                                        smssend.SendEmailAll(DealerDetails.Email, "Cash Paid Rs." + amount + " to " + AdminDetails.Name + ". New Balance is " + remaindealer + ". Your O/s Credit is " + diff2 + "", "Fund Transfer", AdminDetails.email);
                                    }
                                    if (statusAdmin == "Y")
                                    {
                                        SendPushNotification(adminemail, Url.Action("Admin_to_Dealer", "Home"), "Cash Received Rs." + amount + " Frm " + dealername + ",Pending Credit is " + diff2 + ". (" + string.Format("{0:dd-MMM hh:mm:ss}", DateTime.Now) + ")", "Fund Transfer ..");
                                    }
                                    if (statusDealer == "Y")
                                    {
                                        SendPushNotification(dealeremail, Websitename + "/DEALER/Home/ReceiveFund", "Cash Paid Rs." + amount + " to " + AdminDetails.Name + ". A/c Bal is " + remaindealer + ". Your Pending Credit is " + diff2 + ". (" + string.Format("{0:dd-MMM hh:mm:ss}", DateTime.Now) + ")", "Fund Transfer ..");
                                    }
                                    notify.sendmessage(dealeremail, "Cash Paid Rs." + amount + " to " + AdminDetails.Name + ". A/c Bal is " + remaindealer + ". Your Pending Credit is " + diff2 + ". (" + string.Format("{0:dd-MMM hh:mm:ss}", DateTime.Now) + ")");
                                    ch = "Fund Transfer Successful.";
                                    tp = "success";
                                }
                            }
                            //}
                            //else
                            //{
                            //    ch = "Amount is not Zero";
                            //    tp = "error";
                            //}
                            TempData["BalanceTransferMsg"] = ch;
                            TempData["BalanceTransfertype"] = tp;
                            TempData.Keep("BalanceTransferMsg");
                            TempData.Remove("dlmid");
                            TempData.Remove("bal");
                            TempData.Remove("dl_commission");
                            TempData.Remove("comment");
                            // return RedirectToAction("Admin_to_Dealer");

                            return Json(ch, JsonRequestBehavior.AllowGet);

                        }
                        else
                        {
                            return Json("Please Try After SomeTime", JsonRequestBehavior.AllowGet);
                        }


                    }
                }
                else
                {
                    ViewData["MSG"] = "Transaction Password is Wrong.";
                    ch = "Transaction Password is Wrong.";
                    //  return View();
                    return Json(ch, JsonRequestBehavior.AllowGet);
                }
                //return Json(ch, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }



        [PermissioncheckingAttribute(servicename = "FUNDTRANSFER", permision = "Write")]
        public ActionResult ChkSecurityPassRetailer(string txtcode, string hdSuperstokistID, string hdPaymentMode,
     string hdPaymentAmount, string hdMDDepositeSlipNo, string hdMDTransferType, string hdMDcollection, string hdMDComments,
     string hdMDBank, string hdsupraccno, string hdMDaccountno, string hdMDutrno, string hdMDwallet,
     string hdMDwalletno, string hdMDtransationno, string hdMDsettelment, string hdMDCreditDetail, string hdMDsubject)
        {
            try
            {

                string ch = "";
                var userid = User.Identity.GetUserId();
                TempData.Keep("RetailerId");
                TempData.Keep("txtbal");
                TempData.Keep("type");
                TempData.Keep("comment");
                var password = Encrypt(txtcode);
                var tranpass = (from paa in db.admin_new_pass where paa.transpass == password select paa).Count();
                if (tranpass > 0)
                {
                    string transferid = null;
                    try
                    {
                        transferid = TempData["transferadmintorem"].ToString();
                    }
                    catch (Exception ex)
                    {
                        return Json("Please Try After SomeTime", JsonRequestBehavior.AllowGet);
                    }

                    var counts = db.FundTransfercount(userid, hdSuperstokistID, hdPaymentMode, Convert.ToDecimal(hdPaymentAmount), transferid, "Admintoretailer").SingleOrDefault().msg;
                    int msgcount = Convert.ToInt32(counts);
                    if (msgcount == 0)
                    {
                        msgcount = 60001;
                    }
                    int max_limit = 60000;
                    if (msgcount > max_limit)
                    {
                        string RetailerId = hdSuperstokistID;
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
                        string bankname = hdMDBank == null ? hdMDwallet : hdMDBank;
                        string adminacco = hdMDaccountno == null ? hdMDwalletno : hdMDaccountno;
                        adminacco = adminacco == null ? hdMDDepositeSlipNo : adminacco;
                        string DepositeSlipNo = hdMDDepositeSlipNo;
                        if (hdMDTransferType != null && hdPaymentMode == "Online Transfer")
                        {
                            type = hdPaymentMode + "/" + hdMDTransferType;
                        }
                        var retaileremail = db.Retailer_Details.Where(p => p.RetailerId == RetailerId).Single().Email;
                        var AdminEmail = db.Admin_details.Single().email;
                        //string txtbal = TempData["txtbal"].ToString();
                        //string type = TempData["type"].ToString();
                        //string comment = TempData["comment"].ToString();
                        decimal amount1 = Convert.ToDecimal(balance);
                        var remObj = db.Remain_reteller_balance.SingleOrDefault(pp => pp.RetellerId == RetailerId);
                        decimal oldrembal = remObj?.Remainamount ?? 0;
                        decimal finalvalue = oldrembal + amount1;
                        var msg = ""; var tp = "";
                        // var ch = "";
                        if (finalvalue >= 0)
                        {
                            System.Data.Entity.Core.Objects.ObjectParameter output = new
                              System.Data.Entity.Core.Objects.ObjectParameter("Output", typeof(string));
                            var diff1 = (db.Dealer_To_Retailer_Balance.Where(aa => aa.RetailerId == RetailerId && aa.DealerId == userid).OrderByDescending(aa => aa.id).Select(c => c.cr).FirstOrDefault());
                            diff1 = diff1 ?? 0;
                            decimal diff = Convert.ToDecimal(diff1);
                            // if (type == "Credit" || type == "Cash")
                            // {
                            //  ch = db.insert_admin_to_retailer_balance(RetailerId, Convert.ToDecimal(amount1), 0, type, comment, collectionby, bankname, adminacco, output).Single().msg;
                            // }
                            ch = db.insert_admin_to_retailer_balance(RetailerId, Convert.ToDecimal(amount1), type, comment, collectionby, bankname, adminacco, "Direct", transferid, output).Single().msg;
                            if (ch.Contains("Balance Transfer Successfully"))
                            {
                                Websitename = db.Admin_details.Single().WebsiteUrl;
                                diff1 = (db.Dealer_To_Retailer_Balance.Where(aa => aa.RetailerId == RetailerId && aa.DealerId == userid).OrderByDescending(aa => aa.id).Select(c => c.cr).FirstOrDefault());
                                var remainretailer = db.Remain_reteller_balance.Where(p => p.RetellerId == RetailerId).Single().Remainamount;
                                var TotalAmount = type == "Charge Back" ? remainretailer - amount1 : remainretailer + amount1;
                                var statusAdmin = db.PushNotificationStatus.Where(a => a.UserRole == "Admin").SingleOrDefault().Status;
                                var statusRetailer = db.PushNotificationStatus.Where(a => a.UserRole == "Retailer").SingleOrDefault().Status;
                                var statusSendSmsRetailerfundtransfer = db.SMSSendAlls.Where(a => a.ServiceName == "AdmintoREMfundtrans").SingleOrDefault();
                                var statusSendEmailRetailerfundtransfer = db.EmailSendAlls.Where(a => a.ServiceName == "AdmintoREMfundtrans1").SingleOrDefault().Status;
                                var AdminToCC = db.Admin_details.SingleOrDefault().email;

                                var RetailerDetails = db.Retailer_Details.Where(aa => aa.RetailerId == RetailerId).Single();
                                var Retailername = db.Retailer_Details.Where(p => p.RetailerId == RetailerId).Single().RetailerName;
                                var Adminname = db.Admin_details.Single().Name;
                                var AdminRemain = db.Remain_Admin_balance.Single().RemainAmount;
                                if (type == "Credit")
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
                                    //        msgssss = string.Format(smsstypes.Templates, amount1, TotalAmount, diff1);
                                    //        tempid = smsstypes.Templateid;
                                    //        urlss = smsapionsts.smsapi;

                                    //        smssend.sendsmsallnew(RetailerDetails.Mobile, msgssss, urlss, tempid);
                                    //    }
                                    //    //  smssend.sendsmsall(RetailerDetails.Mobile, "Credit Received Rs." + amount1 + ".New Balance is " + TotalAmount + ".Your O/s Credit is " + diff1 + "", "Fund Transfer");
                                    //}

                                    smssend.sms_init(statusSendSmsRetailerfundtransfer.Status, statusSendSmsRetailerfundtransfer.Whatsapp_Status, "FUNDTRANSFERMESSAGE_CREDIT", RetailerDetails.Mobile, amount1, TotalAmount, diff1);

                                    if (statusSendEmailRetailerfundtransfer == "Y")
                                    {
                                        smssend.SendEmailAll(RetailerDetails.Email, "Credit Received Rs." + amount1 + ".New Balance is " + TotalAmount + ".Your O/s Credit is " + diff1 + "", "Fund Transfer", AdminToCC, 1000);
                                    }
                                    if (statusAdmin == "Y")
                                    {
                                        SendPushNotification(AdminEmail, Url.Action("Admin_to_Retailler", "Home"), "Credit Trnf Rs. " + amount1 + "  to " + Retailername + ", Pending Credit is " + diff1 + " . Bal is " + AdminRemain + " . (" + string.Format("{0:dd-MMM hh:mm:ss}", DateTime.Now) + ")", "Fund Transfer ..");
                                    }
                                    if (statusRetailer == "Y")
                                    {
                                        SendPushNotification(retaileremail, Websitename + "/RETAILER/Home/FundRecive", "Credit Recd frm " + Adminname + " Rs." + amount1 + ". Your pending credit is " + diff1 + " . New Balance is " + remainretailer + ". (" + string.Format("{0:dd-MMM hh:mm:ss}", DateTime.Now) + ")", "Fund Transfer ..");
                                    }
                                    notify.sendmessage(retaileremail, "Credit Recd frm " + Adminname + " Rs." + amount1 + ". Your pending credit is " + diff1 + " . New Balance is " + remainretailer + ". (" + string.Format("{0:dd-MMM hh:mm:ss}", DateTime.Now) + ")");
                                    ch = "Fund Transfer Successful.";
                                    tp = "success";
                                }
                                else
                                {
                                    //if (statusSendSmsRetailerfundtransfer == "Y")
                                    //{
                                    //    string msgssss = "";
                                    //    string tempid = "";
                                    //    string urlss = "";

                                    //    var smsapionsts = db.apisms.Where(x => x.sts == "Y").SingleOrDefault();
                                    //    var smsstypes = db.Sending_SMS_Templates.Where(x => x.SMS_TYPE == "FUNDTRANSFERMESSAGE_CASH" && x.SMSAPIID == smsapionsts.id).SingleOrDefault();
                                    //    if (smsstypes != null)
                                    //    {
                                    //        msgssss = string.Format(smsstypes.Templates, amount1, Adminname, TotalAmount, diff1);
                                    //        tempid = smsstypes.Templateid;
                                    //        urlss = smsapionsts.smsapi;

                                    //        smssend.sendsmsallnew(RetailerDetails.Mobile, msgssss, urlss, tempid);
                                    //    }
                                    //    //  smssend.sendsmsall(RetailerDetails.Mobile, "Cash Paid Rs." + amount1 + " to " + Adminname + ". New Balance is " + TotalAmount + ". Your O/s Credit is " + diff1 + "", "Fund Transfer");
                                    //}

                                    smssend.sms_init(statusSendSmsRetailerfundtransfer.Status, statusSendSmsRetailerfundtransfer.Whatsapp_Status, "FUNDTRANSFERMESSAGE_CASH", RetailerDetails.Mobile, amount1, Adminname, TotalAmount, diff1);

                                    if (statusSendEmailRetailerfundtransfer == "Y")
                                    {
                                        smssend.SendEmailAll(RetailerDetails.Email, "Cash Paid Rs." + amount1 + " to " + Adminname + ". New Balance is " + TotalAmount + ". Your O/s Credit is " + diff1 + "", "Fund Transfer", AdminToCC, 1000);
                                    }
                                    if (statusAdmin == "Y")
                                    {
                                        SendPushNotification(AdminEmail, Url.Action("Admin_to_Retailler", "Home"), "Cash Received Rs." + amount1 + " Frm " + Retailername + ",Pending Credit is " + diff1 + ". (" + string.Format("{0:dd-MMM hh:mm:ss}", DateTime.Now) + ")", "Fund Transfer ..");
                                    }
                                    if (statusRetailer == "Y")
                                    {
                                        SendPushNotification(retaileremail, Websitename + "/RETAILER/Home/FundRecive", "Cash Paid Rs." + amount1 + " to " + Adminname + ". A/c Bal is " + TotalAmount + ". Your Pending Credit is " + diff1 + ". (" + string.Format("{0:dd-MMM hh:mm:ss}", DateTime.Now) + ")", "Fund Transfer ..");
                                    }
                                    notify.sendmessage(retaileremail, "Cash Paid Rs." + amount1 + " to " + Adminname + ". A/c Bal is " + TotalAmount + ". Your Pending Credit is " + diff1 + ". (" + string.Format("{0:dd-MMM hh:mm:ss}", DateTime.Now) + ")");
                                    ch = "Fund Transfer Successful.";
                                    tp = "success";
                                }
                            }
                        }
                        else
                        {
                            ch = "Old Remain Balance is Less then This Amount.";
                            tp = "error";
                        }
                        TempData["BalanceTransferMsg"] = ch;
                        TempData["BalanceTransfertype"] = tp;
                        TempData.Remove("RetailerId");
                        TempData.Remove("txtbal");
                        TempData.Remove("comment");
                        //  return RedirectToAction("Admin_to_Retailler");
                        return Json(ch, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json("Your Previous Request IN Process Please Try After SomeTime", JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    ch = "Transaction Password is Wrong.";
                    // ViewData["MSG"] = "Transaction Password is Wrong.";
                    //  return View();
                    return Json(ch, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }

        }

        [PermissioncheckingAttribute(servicename = "FUNDTRANSFER", permision = "Write")]
        public ActionResult ChkSecurityPassAPIUSER(string txtcode, string hdSuperstokistID, string hdPaymentMode,
     string hdPaymentAmount, string hdMDDepositeSlipNo, string hdMDTransferType, string hdMDcollection, string hdMDComments,
     string hdMDBank, string hdsupraccno, string hdMDaccountno, string hdMDutrno, string hdMDwallet,
     string hdMDwalletno, string hdMDtransationno, string hdMDsettelment, string hdMDCreditDetail, string hdMDsubject)
        {
            try
            {
                string ch = "";
                var password = Encrypt(txtcode);
                var tranpass = (from paa in db.admin_new_pass where paa.transpass == password select paa).Count();
                if (tranpass > 0)
                {
                    string transferid = null;
                    try
                    {
                        transferid = TempData["transferadmintoAPI"].ToString();
                    }
                    catch (Exception ex)
                    {
                        return Json("Please Try After SomeTime", JsonRequestBehavior.AllowGet);
                    }

                    var counts = db.FundTransfercount("", hdSuperstokistID, hdPaymentMode, Convert.ToDecimal(hdPaymentAmount), transferid, "Admintoapi").SingleOrDefault().msg;
                    int msgcount = Convert.ToInt32(counts);
                    if (msgcount == 0)
                    {
                        msgcount = 60001;
                    }
                    int max_limit = 60000;
                    if (msgcount > max_limit)
                    {

                        var userid = User.Identity.GetUserId();
                        // string api = TempData["api"].ToString();
                        var AdminEmail = db.Admin_details.Single().email;
                        var apiemail = db.api_user_details.Where(p => p.apiid == hdSuperstokistID).Single().emailid;

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
                        string bankname = hdMDBank == null ? hdMDwallet : hdMDBank;
                        string adminacco = hdMDaccountno == null ? hdMDwalletno : hdMDaccountno;
                        adminacco = adminacco == null ? hdMDDepositeSlipNo : adminacco;
                        string DepositeSlipNo = hdMDDepositeSlipNo;
                        if (hdMDTransferType != null && hdPaymentMode == "Online Transfer")
                        {
                            type = hdPaymentMode + "/" + hdMDTransferType;
                        }
                        var diff1 = (db.API_Balance_transfer.Where(aa => aa.apiid == hdSuperstokistID).OrderByDescending(aa => aa.id).Select(c => c.cr).FirstOrDefault());
                        diff1 = diff1 ?? 0;
                        decimal diff = Convert.ToDecimal(diff1);
                        //var ch = "";
                        var tp = "";
                        //decimal amount1 = Convert.ToDecimal(balance);
                        System.Data.Entity.Core.Objects.ObjectParameter output = new
                      System.Data.Entity.Core.Objects.ObjectParameter("Output", typeof(string));

                        //if (amount1 > 0)
                        //{
                        //  if (type == "Credit" || type == "Cash")
                        //   {
                        ch = db.Api_insert_balance(hdSuperstokistID, Convert.ToDecimal(balance), type, comment, collectionby, bankname, adminacco, "Direct", transferid, output).Single().msg;
                        //  }
                        if (ch == "Balance Transfer Successfully.")
                        {
                            Websitename = db.Admin_details.Single().WebsiteUrl;
                            diff1 = (db.API_Balance_transfer.Where(aa => aa.apiid == hdSuperstokistID).OrderByDescending(aa => aa.id).Select(c => c.cr).FirstOrDefault());
                            var remainapi = db.api_remain_amount.Where(p => p.apiid == hdSuperstokistID).Single().balance;
                            var statusApi = db.PushNotificationStatus.Where(a => a.UserRole == "ApiUser").SingleOrDefault().Status;
                            var statusAdmin = db.PushNotificationStatus.Where(a => a.UserRole == "Admin").SingleOrDefault().Status;
                            var statusSendSmsAPIfundtransfer = db.SMSSendAlls.Where(a => a.ServiceName == "AdmintoApifundtrans").SingleOrDefault();
                            var statusSendEmailAPIfundtransfer = db.EmailSendAlls.Where(a => a.ServiceName == "AdmintoApifundtrans1").SingleOrDefault().Status;
                            var AdminToCC = db.Admin_details.SingleOrDefault().email;

                            var ApiDetails = db.api_user_details.Where(aa => aa.apiid == hdSuperstokistID).Single();
                            var Apiname = db.api_user_details.Where(a => a.apiid == hdSuperstokistID).Single().username;
                            var AdminName = db.Admin_details.Single().Name;
                            var AdminRemain = db.Remain_Admin_balance.Where(a => a.admin == userid).Single().RemainAmount;

                            if (type == "Credit")
                            {
                                //if (statusSendSmsAPIfundtransfer == "Y")
                                //{
                                //    string msgssss = "";
                                //    string tempid = "";
                                //    string urlss = "";

                                //    var smsapionsts = db.apisms.Where(x => x.sts == "Y").SingleOrDefault();
                                //    var smsstypes = db.Sending_SMS_Templates.Where(x => x.SMS_TYPE == "FUNDTRANSFERCREDITRECIVEDBYADMIN" && x.SMSAPIID == smsapionsts.id).SingleOrDefault();
                                //    if (smsstypes != null)
                                //    {
                                //        msgssss = string.Format(smsstypes.Templates, balance, remainapi, diff1);
                                //        tempid = smsstypes.Templateid;
                                //        urlss = smsapionsts.smsapi;

                                //        smssend.sendsmsallnew(ApiDetails.mobile, msgssss, urlss, tempid);
                                //    }
                                //    // smssend.sendsmsall(ApiDetails.mobile, "Credit Received By Admin Rs." + balance + ".New Balance is " + remainapi + ".Your O/s Credit is " + diff1 + "", "Fund Transfer");
                                //}

                                smssend.sms_init(statusSendSmsAPIfundtransfer.Status, statusSendSmsAPIfundtransfer.Whatsapp_Status, "FUNDTRANSFERCREDITRECIVEDBYADMIN", ApiDetails.mobile, balance, remainapi, diff1);

                                if (statusSendEmailAPIfundtransfer == "Y")
                                {
                                    smssend.SendEmailAll(ApiDetails.emailid, "Credit Received By Admin Rs." + balance + ".New Balance is " + remainapi + ".Your O/s Credit is " + diff1 + "", "Fund Transfer", AdminToCC, 1000);
                                }
                                if (statusAdmin == "Y")
                                {
                                    SendPushNotification(AdminEmail, Url.Action("Admin_to_api", "Home"), "Credit Trnf Rs. " + balance + "  to " + Apiname + ", Pending Credit is " + diff1 + " . Bal is " + AdminRemain + " . (" + string.Format("{0:dd-MMM hh:mm:ss}", DateTime.Now) + ")", "Fund Transfer ..");
                                }
                                if (statusApi == "Y")
                                {
                                    SendPushNotification(apiemail, Websitename + "/Api/Home/ReceiveFund", "Credit Recd frm " + AdminName + " Rs." + balance + ". Your pending credit is " + diff1 + " . New Balance is " + remainapi + ". (" + string.Format("{0:dd-MMM hh:mm:ss}", DateTime.Now) + ")", "Fund Transfer ..");
                                }
                                ch = "Fund Transfer Successful.";
                                tp = "success";
                            }
                            else
                            {
                                //if (statusSendSmsAPIfundtransfer == "Y")
                                //{
                                //    string msgssss = "";
                                //    string tempid = "";
                                //    string urlss = "";

                                //    var smsapionsts = db.apisms.Where(x => x.sts == "Y").SingleOrDefault();
                                //    var smsstypes = db.Sending_SMS_Templates.Where(x => x.SMS_TYPE == "FUNDTRANSFERMESSAGE_CASH" && x.SMSAPIID == smsapionsts.id).SingleOrDefault();
                                //    if (smsstypes != null)
                                //    {
                                //        msgssss = string.Format(smsstypes.Templates, balance, AdminName, remainapi, diff1);
                                //        tempid = smsstypes.Templateid;
                                //        urlss = smsapionsts.smsapi;

                                //        smssend.sendsmsallnew(ApiDetails.mobile, msgssss, urlss, tempid);
                                //    }
                                //    // smssend.sendsmsall(ApiDetails.mobile, "Cash Paid Rs." + balance + " to " + AdminName + ". New Balance is " + remainapi + ". Your O/s Credit is " + diff1 + "", "Fund Transfer");
                                //}

                                smssend.sms_init(statusSendSmsAPIfundtransfer.Status, statusSendSmsAPIfundtransfer.Whatsapp_Status, "FUNDTRANSFERMESSAGE_CASH", ApiDetails.mobile, balance, AdminName, remainapi, diff1);

                                if (statusSendEmailAPIfundtransfer == "Y")
                                {
                                    smssend.SendEmailAll(ApiDetails.emailid, "Cash Paid Rs." + balance + " to " + AdminName + ". New Balance is " + remainapi + ". Your O/s Credit is " + diff1 + "", "Fund Transfer", AdminToCC, 1000);
                                }
                                if (statusAdmin == "Y")
                                {
                                    SendPushNotification(AdminEmail, Url.Action("Admin_to_api", "Home"), "Cash Received Rs." + balance + " Frm " + Apiname + ",Pending Credit is " + diff1 + ". (" + string.Format("{0:dd-MMM hh:mm:ss}", DateTime.Now) + ")", "Fund Transfer ..");
                                }
                                if (statusApi == "Y")
                                {
                                    SendPushNotification(apiemail, Websitename + "/Api/Home/ReceiveFund", "Cash Paid Rs." + balance + " to " + AdminName + ". A/c Bal is " + remainapi + ". Your Pending Credit is " + diff1 + ". (" + string.Format("{0:dd-MMM hh:mm:ss}", DateTime.Now) + ")", "Fund Transfer ..");
                                }
                                ch = "Fund Transfer Successful.";
                                tp = "success";
                            }
                        }

                        TempData["BalanceTransferMsg"] = ch.ToString();
                        TempData["BalanceTransfertype"] = tp;
                        TempData.Remove("api");
                        TempData.Remove("balance");
                        TempData.Remove("optname");
                        TempData.Remove("comment");
                        return Json(ch, JsonRequestBehavior.AllowGet);
                        // return RedirectToAction("Admin_to_api");
                    }
                    else
                    {
                        return Json("Please Try After SomeTime", JsonRequestBehavior.AllowGet);
                    }

                }
                else
                {
                    ch = "Transaction Password is Wrong.";
                    ViewData["MSG"] = "Transaction Password is Wrong.";
                    // return View();
                    return Json(ch, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [PermissioncheckingAttribute(servicename = "FUNDTRANSFER", permision = "Write")]
        public ActionResult ChkSecurityPassWLUSER(string txtcode, string hdSuperstokistID, string hdPaymentMode,
   string hdPaymentAmount, string hdMDDepositeSlipNo, string hdMDTransferType, string hdMDcollection, string hdMDComments,
   string hdMDBank, string hdsupraccno, string hdMDaccountno, string hdMDutrno, string hdMDwallet,
   string hdMDwalletno, string hdMDtransationno, string hdMDsettelment, string hdMDCreditDetail, string hdMDsubject)
        {
            var ch = "";
            TempData.Keep("api");
            TempData.Keep("balance");
            TempData.Keep("type");
            TempData.Keep("comment");
            var password = Encrypt(txtcode);
            try
            {
                var tranpass = (from paa in db.admin_new_pass where paa.transpass == password select paa).Count();
                if (tranpass > 0)
                {
                    string transferid = null;
                    try
                    {
                        transferid = TempData["transferadmintoWebapi"].ToString();
                    }
                    catch (Exception ex)
                    {
                        return Json("Please Try After SomeTime", JsonRequestBehavior.AllowGet);
                    }

                    var counts = db.FundTransfercount("", hdSuperstokistID, hdPaymentMode, Convert.ToDecimal(hdPaymentAmount), transferid, "Admintowhitelabel").SingleOrDefault().msg;
                    int msgcount = Convert.ToInt32(counts);
                    if (msgcount == 0)
                    {
                        msgcount = 60001;
                    }
                    int max_limit = 60000;
                    if (msgcount > max_limit)
                    {
                        string api = hdSuperstokistID;
                        var AdminEmail = db.Admin_details.Single().email;
                        var whitelabelemail = db.WhiteLabel_userList.Where(a => a.WhiteLabelID == api).Single().Name;
                        string balance = hdPaymentAmount;
                        string type = hdPaymentMode;
                        string comment = hdMDComments;
                        string collectionby = hdMDcollection == null ? hdMDtransationno : hdMDcollection;
                        collectionby = collectionby == null ? hdMDutrno : collectionby;
                        collectionby = collectionby == null ? hdMDtransationno : collectionby;
                        collectionby = collectionby == null ? hdMDsettelment : collectionby;
                        collectionby = collectionby == null ? hdMDCreditDetail : collectionby;
                        collectionby = collectionby == null ? hdMDsubject : collectionby;
                        string bankname = hdMDBank == null ? hdMDwallet : hdMDBank;
                        string adminacco = hdMDaccountno == null ? hdMDwalletno : hdMDaccountno;
                        adminacco = adminacco == null ? hdMDDepositeSlipNo : adminacco;
                        string DepositeSlipNo = hdMDDepositeSlipNo;
                        if (hdMDTransferType != null && hdPaymentMode == "Online Transfer")
                        {
                            type = hdPaymentMode + "/" + hdMDTransferType;
                        }
                        var diff1 = (db.whitelabel_Balance_transfer.Where(aa => aa.WhiteLabelID == api).OrderByDescending(aa => aa.id).Select(c => c.cr).FirstOrDefault());
                        diff1 = diff1 ?? 0;
                        decimal diff = Convert.ToDecimal(diff1);
                        var tp = "";
                        decimal amount1 = Convert.ToDecimal(balance);
                        System.Data.Entity.Core.Objects.ObjectParameter output = new
                      System.Data.Entity.Core.Objects.ObjectParameter("Output", typeof(string));

                        //if (amount1 > 0)
                        //{
                        // if (type == "Credit" || type == "Cash")
                        //  {
                        ch = db.whitelabel_insert_balance(api, Convert.ToDecimal(balance), type, comment, collectionby, bankname, adminacco, "Direct", transferid, output).Single().msg;
                        // }
                        if (ch == "Balance Transfer Successfully.")
                        {
                            diff1 = (db.whitelabel_Balance_transfer.Where(aa => aa.WhiteLabelID == api).OrderByDescending(aa => aa.id).Select(c => c.cr).FirstOrDefault());
                            var remainwhitelabel = db.White_label_remainbal.Where(p => p.userid == api).Single().remainbal;
                            var statusAdmin = db.PushNotificationStatus.Where(a => a.UserRole == "Admin").SingleOrDefault().Status;
                            var statusSendSmswhitelabelfundtransfer = db.SMSSendAlls.Where(a => a.ServiceName == "AdmintoWhitelabelfundtrans").SingleOrDefault();
                            var statusSendEMAILwhitelabelfundtransfer = db.EmailSendAlls.Where(a => a.ServiceName == "AdmintoWhitelabelfundtrans1").SingleOrDefault().Status;

                            var whitelabelDtls = db.WhiteLabel_userList.Where(p => p.WhiteLabelID == api).Single();
                            var AdminToCC = db.Admin_details.SingleOrDefault().email;
                            if (type == "Credit")
                            {
                                //if (statusSendSmswhitelabelfundtransfer == "Y")
                                //{
                                //    string msgssss = "";
                                //    string tempid = "";
                                //    string urlss = "";

                                //    var smsapionsts = db.apisms.Where(x => x.sts == "Y").SingleOrDefault();
                                //    var smsstypes = db.Sending_SMS_Templates.Where(x => x.SMS_TYPE == "FUNDTRANSFERMESSAGE_CREDIT" && x.SMSAPIID == smsapionsts.id).SingleOrDefault();
                                //    if (smsstypes != null)
                                //    {
                                //        msgssss = string.Format(smsstypes.Templates, amount1, remainwhitelabel, diff1);
                                //        tempid = smsstypes.Templateid;
                                //        urlss = smsapionsts.smsapi;

                                //        smssend.sendsmsallnew(whitelabelDtls.Mobile, msgssss, urlss, tempid);
                                //    }
                                //    //   smssend.sendsmsall(whitelabelDtls.Mobile, "Credit Received Rs." + amount1 + ".New Balance is " + remainwhitelabel + ".Your O/s Credit is " + diff1 + "", "Fund Transfer");
                                //}

                                smssend.sms_init(statusSendSmswhitelabelfundtransfer.Status, statusSendSmswhitelabelfundtransfer.Whatsapp_Status, "FUNDTRANSFERMESSAGE_CREDIT", whitelabelDtls.Mobile, amount1, remainwhitelabel, diff1);

                                if (statusSendEMAILwhitelabelfundtransfer == "Y")
                                {
                                    smssend.SendEmailAll(whitelabelDtls.EmailId, "Credit Received Rs." + amount1 + ".New Balance is " + remainwhitelabel + ".Your O/s Credit is " + diff1 + "", "Fund Transfer", AdminToCC, 1000);
                                }
                                if (statusAdmin == "Y")
                                {
                                    SendPushNotification(AdminEmail, Url.Action("Admin_to_white", "Home"), "Credit Transferred Rs. " + amount1 + ".Total Credit(O/s) Balance is " + diff1 + "", "Fund Transfer ..");
                                }
                                SendPushNotification(whitelabelemail, Websitename + "/WHITELABEL/Home/Dashboard", "Credit Received Rs." + amount1 + ".New Balance is " + remainwhitelabel + ".Your O/s Credit is " + diff1 + "", "Fund Transfer ..");
                                ch = "Fund Transfer Successful.";
                                tp = "success";
                            }
                            else
                            {
                                var whitelabelname = db.WhiteLabel_userList.Where(p => p.WhiteLabelID == api).Single().Name;
                                var Adminname = db.Admin_details.Single().Name;
                                //if (statusSendSmswhitelabelfundtransfer == "Y")
                                //{
                                //    string msgssss = "";
                                //    string tempid = "";
                                //    string urlss = "";

                                //    var smsapionsts = db.apisms.Where(x => x.sts == "Y").SingleOrDefault();
                                //    var smsstypes = db.Sending_SMS_Templates.Where(x => x.SMS_TYPE == "FUNDTRANSFERMESSAGE_CASH" && x.SMSAPIID == smsapionsts.id).SingleOrDefault();
                                //    if (smsstypes != null)
                                //    {



                                //        msgssss = string.Format(smsstypes.Templates, amount1, Adminname, remainwhitelabel, diff1);
                                //        tempid = smsstypes.Templateid;
                                //        urlss = smsapionsts.smsapi;

                                //        smssend.sendsmsallnew(whitelabelDtls.Mobile, msgssss, urlss, tempid);
                                //    }
                                //    //   smssend.sendsmsall(whitelabelDtls.Mobile, "Cash Paid Rs." + amount1 + " to " + Adminname + ". New Balance is " + remainwhitelabel + ". Your O/s Credit is " + diff1 + "", "Fund Transfer");
                                //}

                                smssend.sms_init(statusSendSmswhitelabelfundtransfer.Status, statusSendSmswhitelabelfundtransfer.Whatsapp_Status, "FUNDTRANSFERMESSAGE_CASH", whitelabelDtls.Mobile, amount1, Adminname, remainwhitelabel, diff1);

                                if (statusSendEMAILwhitelabelfundtransfer == "Y")
                                {
                                    smssend.SendEmailAll(whitelabelDtls.EmailId, "Cash Paid Rs." + amount1 + " to " + Adminname + ". New Balance is " + remainwhitelabel + ". Your O/s Credit is " + diff1 + "", "Fund Transfer", AdminToCC, 1000);
                                }
                                if (statusAdmin == "Y")
                                {
                                    SendPushNotification(AdminEmail, Url.Action("Admin_to_white", "Home"), "Cash Recived Rs." + amount1 + " From " + whitelabelname + ",his O/s Credit is " + diff1 + "", "Fund Transfer ..");
                                }
                                SendPushNotification(whitelabelemail, Websitename + "/WHITELABEL/Home/Dashboard", "Cash Paid Rs." + amount1 + " to " + Adminname + ". New Balance is " + remainwhitelabel + ". Your O/s Credit is " + diff1 + "", "Fund Transfer ..");
                                ch = "Fund Transfer Successful.";
                                tp = "success";
                            }
                        }
                        //}
                        //else
                        //{
                        //    ch = "Amount should be not zero";
                        //    tp = "error";
                        //}
                        TempData["BalanceTransferMsg"] = ch.ToString();
                        TempData["BalanceTransfertype"] = tp;
                        TempData.Remove("api");
                        TempData.Remove("balance");
                        TempData.Remove("optname");
                        TempData.Remove("comment");
                        //  return RedirectToAction("Admin_to_white");
                        return Json(ch, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json("Please Try After SomeTime", JsonRequestBehavior.AllowGet);
                    }

                }
                else
                {
                    ch = "Transaction Password is Wrong.";
                    ViewData["MSG"] = "Transaction Password is Wrong.";
                    // return View();
                    return Json(ch, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }



        public ActionResult GenerateExcellFileFundTransfer(string tabID, DateTime frm_date, DateTime to_date, string usernm)
        {
            EmployeFundtransferviewmodel vmodel = new EmployeFundtransferviewmodel();
            List<SelectListItem> items = new List<SelectListItem>();
            var userid = db.Admin_details.FirstOrDefault().userid;
            DataTable dtt = new DataTable();
            if (string.IsNullOrEmpty(usernm))
            {
                usernm = "ALL";
            }
            if (tabID == "home")
            {
                dtt.Columns.Add("Farm Name", typeof(string));
                dtt.Columns.Add("Head", typeof(string));
                dtt.Columns.Add("PayMent Mode", typeof(string));
                dtt.Columns.Add("Total Amount", typeof(string));
                dtt.Columns.Add("Charges", typeof(string));
                dtt.Columns.Add("Net Transfer", typeof(string));
                dtt.Columns.Add("MD Remain", typeof(string));
                dtt.Columns.Add("MD Remain POST", typeof(string));
                dtt.Columns.Add("MD Pre Credit", typeof(string));
                dtt.Columns.Add("MD Post Credit", typeof(string));
                dtt.Columns.Add("Comment", typeof(string));
                dtt.Columns.Add("Date", typeof(string));
                dtt.Columns.Add("Bank Name", typeof(string));
                DateTime fromto = to_date.AddDays(1);
                var respo = db.Select_balance_Super_stokist(usernm, frm_date, fromto).ToList();

                if (respo.Count > 0)
                {
                    foreach (var item in respo)
                    {
                        dtt.Rows.Add(item.FarmName, item.Head, item.bal_type, item.Balance, item.commistion, item.fund_transfer, item.remainsuper, item.remainsuperafter, item.oldcrbalance, item.cr, item.comment, item.RechargeDate, item.BankName);
                    }
                }
                else
                {
                    dtt.Rows.Add("", "", "", "", "", "", "", "", "", "", "", "", "");
                }
            }
            else if (tabID == "menu1")
            {
                dtt.Columns.Add("Farm Name", typeof(string));
                dtt.Columns.Add("Head", typeof(string));
                dtt.Columns.Add("PayMent Mode", typeof(string));
                dtt.Columns.Add("Total Amount", typeof(string));
                dtt.Columns.Add("Charges", typeof(string));
                dtt.Columns.Add("Net Transfer", typeof(string));
                dtt.Columns.Add("Distributor Remain", typeof(string));
                dtt.Columns.Add("Distributor Remain POST", typeof(string));
                dtt.Columns.Add("Distributor Pre Credit", typeof(string));
                dtt.Columns.Add("Distributor Post Credit", typeof(string));
                dtt.Columns.Add("Comment", typeof(string));
                dtt.Columns.Add("Date", typeof(string));
                dtt.Columns.Add("Bank Name", typeof(string));
                DateTime fromto = to_date.AddDays(1);
                var respo = db.select_admin_to_Dealer("Admin", usernm, frm_date, fromto).OrderByDescending(a => a.date_dlm).ToList();
                if (respo.Count > 0)
                {
                    foreach (var item in respo)
                    {
                        dtt.Rows.Add(item.FarmName, item.Head, item.bal_type, item.Newbalance, item.comm, item.balance, item.dealer_prebal, item.dealer_postbal, item.oldcrbalance, item.cr, item.comment, item.date_dlm, item.BankName);
                    }
                }
                else
                {
                    dtt.Rows.Add("", "", "", "", "", "", "", "", "", "", "", "", "");
                }


            }
            else if (tabID == "menu2")
            {
                dtt.Columns.Add("Farm Name", typeof(string));
                dtt.Columns.Add("Head", typeof(string));
                dtt.Columns.Add("PayMent Mode", typeof(string));
                dtt.Columns.Add("Total Amount", typeof(string));
                dtt.Columns.Add("Charges", typeof(string));
                dtt.Columns.Add("Net Transfer", typeof(string));
                dtt.Columns.Add("Retailer Remain", typeof(string));
                dtt.Columns.Add("Retailer Remain POST", typeof(string));
                dtt.Columns.Add("Retailer Pre Credit", typeof(string));
                dtt.Columns.Add("Retailer Post Credit", typeof(string));
                dtt.Columns.Add("Comment", typeof(string));
                dtt.Columns.Add("Date", typeof(string));
                dtt.Columns.Add("Bank Name", typeof(string));

                DateTime fromto = to_date.AddDays(1);
                var respo = db.select_dlm_rem(userid, frm_date, fromto, usernm, 1, 1500).ToList();

                if (respo.Count > 0)
                {
                    foreach (var item in respo)
                    {
                        dtt.Rows.Add(item.Frm_Name, item.Head, item.bal_type, item.TotalBal, item.commission, item.Balance, item.remain_pre_amount, item.remain_amount, item.oldcrbalance, item.cr, item.comment, item.RechargeDate, item.BankName);
                    }
                }
                else
                {
                    dtt.Rows.Add("", "", "", "", "", "", "", "", "", "", "", "", "");
                }


            }
            else if (tabID == "menu3")
            {
                dtt.Columns.Add("Farm Name", typeof(string));
                dtt.Columns.Add("Head", typeof(string));
                dtt.Columns.Add("PayMent Mode", typeof(string));
                dtt.Columns.Add("Total Amount", typeof(string));
                dtt.Columns.Add("Charges", typeof(string));
                dtt.Columns.Add("Net Transfer", typeof(string));
                dtt.Columns.Add("APi User Remain", typeof(string));
                dtt.Columns.Add("APi User Remain POST", typeof(string));
                dtt.Columns.Add("APi User Pre Credit", typeof(string));
                dtt.Columns.Add("APi User Post Credit", typeof(string));
                dtt.Columns.Add("Comment", typeof(string));
                dtt.Columns.Add("Date", typeof(string));
                dtt.Columns.Add("Bank Name", typeof(string));
                DateTime fromto = to_date.AddDays(1);
                var respo = db.API_balance_transfer_report(usernm, "ALL", frm_date, fromto).ToList();

                if (respo.Count > 0)
                {
                    foreach (var item in respo)
                    {
                        dtt.Rows.Add(item.farmname, item.Head, item.bal_type, item.TotalFund, item.commistion, item.balance, item.remianold, item.remainamount, item.oldcrbalance, item.cr, item.comment, item.date, item.BankName);
                    }
                }
                else
                {
                    dtt.Rows.Add("", "", "", "", "", "", "", "", "", "", "", "", "");
                }


            }
            else if (tabID == "menu4")
            {
                dtt.Columns.Add("Farm Name", typeof(string));
                dtt.Columns.Add("Head", typeof(string));
                dtt.Columns.Add("PayMent Mode", typeof(string));
                dtt.Columns.Add("Total Amount", typeof(string));
                dtt.Columns.Add("Charges", typeof(string));
                dtt.Columns.Add("Net Transfer", typeof(string));
                dtt.Columns.Add("Whitelabel User Remain", typeof(string));
                dtt.Columns.Add("Whitelabel User Remain POST", typeof(string));
                dtt.Columns.Add("Whitelabel User Pre Credit", typeof(string));
                dtt.Columns.Add("Whitelabel User Post Credit", typeof(string));
                dtt.Columns.Add("Comment", typeof(string));
                dtt.Columns.Add("Date", typeof(string));
                dtt.Columns.Add("Bank Name", typeof(string));
                DateTime fromto = to_date.AddDays(1);
                var respo = db.whitelabel_balance_transfer_report(usernm, "ALL", frm_date, fromto).ToList();
                if (respo.Count > 0)
                {
                    foreach (var item in respo)
                    {
                        dtt.Rows.Add(item.FrmName, item.Head, item.bal_type, item.TotalFund, item.comm, item.balance, item.remianold, item.remainamount, item.oldcrbalance, item.cr, item.comment, item.date, item.BankName);
                    }
                }
                else
                {
                    dtt.Rows.Add("", "", "", "", "", "", "", "", "", "", "", "", "");
                }
            }
            var grid = new GridView();
            grid.DataSource = dtt;
            grid.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=Fund_transferReport.xls");
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

        public ActionResult FundtransferPdfGenerate(string tabID, DateTime frm_date, DateTime to_date, string usernm)
        {
            DataTable dtt = new DataTable();
            if (string.IsNullOrEmpty(usernm))
            {
                usernm = "ALL";
            }
            DateTime fromto = to_date.AddDays(1);
            EmployeFundtransferviewmodel vmodel = new EmployeFundtransferviewmodel();
            if (tabID == "menu1")
            {


                vmodel.admintodealerllist = db.select_admin_to_Dealer("Admin", usernm, frm_date, fromto).ToList();
                return new ViewAsPdf(vmodel);


            }
            else if (tabID == "menu2")
            {
                var userid = db.Admin_details.FirstOrDefault().userid;


                vmodel.AdminTORetailer = db.select_dlm_rem(userid, frm_date, fromto, usernm, 1, 1500).ToList();
                return new ViewAsPdf(vmodel);

            }
            else if (tabID == "menu3")
            {


                vmodel.API_balance_transfer_reportList = db.API_balance_transfer_report(usernm, "ALL", frm_date, fromto).ToList();
                return new ViewAsPdf(vmodel);


            }
            else if (tabID == "menu4")
            {


                vmodel.whitelabel_balance_transfer_report_List = db.whitelabel_balance_transfer_report(usernm, "ALL", frm_date, fromto).ToList();

                return new ViewAsPdf(vmodel);
            }
            else
            {

                vmodel.Superstocklistbalance = db.Select_balance_Super_stokist(usernm, frm_date, fromto).ToList();

                return new ViewAsPdf(vmodel);
            }


        }

        [PermissioncheckingAttribute(servicename = "FUNDTRANSFER", permision = "Read")]
        public PartialViewResult TABCHANGEFORFUNDTRANSFER(string nameoftab, string usernm, string txt_frm_date, string txt_to_date, string txtsearch, string paymode)
        {

            EmployeFundtransferviewmodel vmodel = new EmployeFundtransferviewmodel();
            if (usernm == "" || usernm == null)
            {
                usernm = "ALL";
            }

            return PartialView("_FundTransferMasterDis", listALL(nameoftab, usernm, txt_frm_date, txt_to_date, txtsearch, paymode));
        }



        public EmployeFundtransferviewmodel listALL(string nameoftab, string usernm, string txtfrmdate, string txttodate, string txtsearch, string paymode)
        {
            //DateTime dtfrom = DateTime.Parse("2019-12-24 12:29:09.750");

            DateTime fromdate;
            DateTime Todate;

            if (string.IsNullOrEmpty(txtfrmdate) && string.IsNullOrEmpty(txttodate))
            {
                fromdate = DateTime.Now.AddDays(-1);
                Todate = DateTime.Now.AddDays(1);
            }
            else
            {

                string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy",
                            "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };

                DateTime dt = !string.IsNullOrWhiteSpace(txtfrmdate) ? DateTime.ParseExact(txtfrmdate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
                DateTime dt1 = !string.IsNullOrWhiteSpace(txttodate) ? DateTime.ParseExact(txttodate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
                fromdate = Convert.ToDateTime(dt).Date;
                Todate = Convert.ToDateTime(dt1).Date.AddDays(1);


            }
            var userid = db.Admin_details.FirstOrDefault().userid;
            EmployeFundtransferviewmodel vmodel = new EmployeFundtransferviewmodel();
            if (nameoftab == "home")
            {
                vmodel.Superstocklistbalance = db.Select_balance_Super_stokist(usernm, fromdate, Todate).ToList();
                if (!string.IsNullOrEmpty(txtsearch))
                {

                    vmodel.Superstocklistbalance = vmodel.Superstocklistbalance.Where(x => x.Head.ToLower().Contains(txtsearch) || x.FarmName.ToLower().Contains(txtsearch) || x.bal_type.ToLower().Contains(txtsearch) || x.Balance.ToString().Contains(txtsearch)).ToList();

                }
                else if (!string.IsNullOrEmpty(paymode))
                {
                    vmodel.Superstocklistbalance = vmodel.Superstocklistbalance.Where(x => x.Head.ToLower().Contains(paymode.ToLower())).ToList();

                }
                //    vmodel.admintodealerllist = null;
            }
            if (nameoftab == "menu1")
            {
                vmodel.admintodealerllist = db.select_admin_to_Dealer("Admin", usernm, fromdate, Todate).ToList();
                if (!string.IsNullOrEmpty(txtsearch))
                {

                    vmodel.admintodealerllist = vmodel.admintodealerllist.Where(x => x.Head.ToLower().Contains(txtsearch) || x.FarmName.ToLower().Contains(txtsearch) || x.bal_type.ToLower().Contains(txtsearch) || x.balance.ToString().Contains(txtsearch)).ToList();
                }
                else if (!string.IsNullOrEmpty(paymode))
                {
                    vmodel.admintodealerllist = vmodel.admintodealerllist.Where(x => x.Head.ToLower().Contains(paymode.ToLower())).ToList();

                }

                //  vmodel.Superstocklistbalance = null;
            }
            if (nameoftab == "menu2")
            {
                // vmodel.admintodealerllist = db.select_admin_to_Dealer("Admin", "ALL", Convert.ToDateTime(dtfrom), Convert.ToDateTime(DateTime.Now.AddDays(1))).ToList();


                vmodel.AdminTORetailer = db.select_dlm_rem(userid, fromdate, Todate, usernm, 1, 15000).ToList();

                if (!string.IsNullOrEmpty(txtsearch))
                {

                    vmodel.AdminTORetailer = vmodel.AdminTORetailer.Where(x => x.Head.ToLower().Contains(txtsearch) || x.Frm_Name.ToLower().Contains(txtsearch) || x.bal_type.ToLower().Contains(txtsearch) || x.Balance.ToString().Contains(txtsearch)).ToList();
                }
                else if (!string.IsNullOrEmpty(paymode))
                {
                    vmodel.AdminTORetailer = vmodel.AdminTORetailer.Where(x => x.Head.ToLower().Contains(paymode.ToLower())).ToList();


                }

            }
            if (nameoftab == "menu3")
            {
                string api1 = "ALL";
                // vmodel.admintodealerllist = db.select_admin_to_Dealer("Admin", "ALL", Convert.ToDateTime(dtfrom), Convert.ToDateTime(DateTime.Now.AddDays(1))).ToList();
                vmodel.API_balance_transfer_reportList = db.API_balance_transfer_report(api1, usernm, fromdate, Todate).ToList();



            }
            if (nameoftab == "menu4")
            {
                // string api1 = "ALL";

                // vmodel.API_balance_transfer_reportList = db.API_balance_transfer_report(api1, "ALL", Convert.ToDateTime(dtfrom), Convert.ToDateTime(DateTime.Now.AddDays(1))).ToList();
                vmodel.whitelabel_balance_transfer_report_List = db.whitelabel_balance_transfer_report(usernm, "ALL", fromdate, Todate).ToList();

            }
            return vmodel;

        }

        public ActionResult Retailer_accoumt_detail()
        {
            var ch = db.retailer_Reamin_Cr_report().ToList();
            return View(ch);
        }

        public JsonResult FillMasterDistributorDropDownList(string nameoftab)
        {
            EmployeFundtransferviewmodel vmodel = new EmployeFundtransferviewmodel();

            if (nameoftab == "menu1")
            {
                var filldealer = db.Dealer_Details.Select(aa => new SelectListItem
                {
                    Text = aa.FarmName + " - " + aa.Mobile,
                    Value = aa.DealerId.ToString(),
                });
                vmodel.ddldistributorInfo = filldealer.AsEnumerable();

                return Json(vmodel.ddldistributorInfo);
            }
            if (nameoftab == "menu2")
            {
                var fillRetailer = db.Retailer_Details.Where(x => x.ISDeleteuser == false).Select(aa => new SelectListItem
                {
                    Text = aa.Frm_Name + " - " + aa.Mobile,
                    Value = aa.RetailerId.ToString(),
                });
                vmodel.ddldistributorInfo = fillRetailer.AsEnumerable();

                return Json(vmodel.ddldistributorInfo);
            }
            if (nameoftab == "menu3")
            {
                var fillAPIUSER = db.api_user_details.Select(aa => new SelectListItem
                {
                    Text = aa.farmname + " - " + aa.mobile,
                    Value = aa.apiid.ToString(),
                });
                vmodel.ddldistributorInfo = fillAPIUSER.AsEnumerable();

                return Json(vmodel.ddldistributorInfo);
            }
            if (nameoftab == "menu4")
            {
                var fillAPIUSER = db.WhiteLabel_userList.Select(aa => new SelectListItem
                {
                    Text = aa.FrmName + " - " + aa.Mobile,
                    Value = aa.WhiteLabelID.ToString(),
                });
                vmodel.ddldistributorInfo = fillAPIUSER.AsEnumerable();
                return Json(vmodel.ddldistributorInfo);
            }
            return Json(vmodel.ddldistributorInfo);
        }
        public ActionResult M_Creditchk(string MID)
        {
            decimal mdbal;
            var ch = (db.admin_to_super_balance.Where(aa => aa.SuperStokistID == MID).OrderByDescending(aa => aa.ID).Select(c => c.cr).FirstOrDefault());
            ch = ch ?? 0;
            if (string.IsNullOrEmpty(MID))
            {
                mdbal = 0;
            }
            else
            {
                mdbal = db.Remain_superstokist_balance.Where(x => x.SuperStokistID == MID).Select(x => x.Remainamount).FirstOrDefault() ?? 0;
            }
            return Json(new { currntcr = ch, rembal = mdbal }, JsonRequestBehavior.AllowGet);
        }



        public ActionResult D_Creditchk(string Dealerid)
        {
            decimal? dlmbal;
            string userid = db.Admin_details.SingleOrDefault().userid;
            var ch = (db.admin_to_dealer.Where(aa => aa.dealerid == Dealerid && aa.balance_from == userid).OrderByDescending(aa => aa.idno).Select(c => c.cr).FirstOrDefault());
            ch = ch ?? 0;
            if (string.IsNullOrEmpty(Dealerid))
            {
                dlmbal = 0;
            }
            else
            {
                dlmbal = db.Remain_dealer_balance.Where(x => x.DealerID == Dealerid).SingleOrDefault().Remainamount;
            }
            return Json(new { currntcr = ch, rembal = dlmbal }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult R_Creditchk(string retailerid)
        {
            string userid = db.Admin_details.SingleOrDefault().userid;
            var ch = db.Dealer_To_Retailer_Balance.Where(aa => aa.RetailerId == retailerid && aa.DealerId == userid).OrderByDescending(aa => aa.id).Select(c => c.cr).FirstOrDefault();
            //ch = ch ?? 0;

            decimal rembal;

            if (string.IsNullOrEmpty(retailerid))
            {
                rembal = 0;
            }
            else
            {
                //rembal = db.Remain_reteller_balance.Where(x => x.RetellerId == retailerid).SingleOrDefault().Remainamount;
                rembal = db.Remain_reteller_balance
             .Where(x => x.RetellerId == retailerid)
             .Select(x => x.Remainamount)
             .FirstOrDefault() ?? 0;
            }
            return Json(new { currntcr = ch, rembal = rembal }, JsonRequestBehavior.AllowGet);


        }

        public ActionResult whitelabel_Creditchk(string whitelabelid)
        {
            var userid = User.Identity.GetUserId();
            var ch = (db.whitelabel_Balance_transfer.Where(aa => aa.WhiteLabelID == whitelabelid).OrderByDescending(aa => aa.id).Select(c => c.cr).FirstOrDefault());
            ch = ch ?? 0;
            return Json(ch, JsonRequestBehavior.AllowGet);
        }
        public ActionResult API_Creditchk(string apiid)
        {
            var userid = User.Identity.GetUserId();
            var ch = (db.API_Balance_transfer.Where(aa => aa.apiid == apiid && aa.groupname == userid).OrderByDescending(aa => aa.id).Select(c => c.cr).FirstOrDefault());
            ch = ch ?? 0;

            decimal rembal;
            if (string.IsNullOrEmpty(apiid))
            {
                rembal = 0;
            }
            else
            {
                rembal = db.api_remain_amount.Where(x => x.apiid == apiid).Select(x => x.balance).FirstOrDefault() ?? 0;
            }
            return Json(new { currntcr = ch, rembal = rembal }, JsonRequestBehavior.AllowGet);



        }

        [PermissioncheckingAttribute(servicename = "FUNDTRANSFER", permision = "Write")]
        public JsonResult FundtransferAdmin_To_Webapi_Generate_Unique_ID()
        {
            var adminuserid = User.Identity.GetUserId();

            string transferids = GenerateUniqueTransectionID();
            var transferid = "AW" + transferids;


            TempData["transferadmintoWebapi"] = transferid;
            return Json(transferid, JsonRequestBehavior.AllowGet);
        }




        [PermissioncheckingAttribute(servicename = "FUNDTRANSFER", permision = "Write")]

        public JsonResult FundtransferAdmin_To_API_Generate_Unique_ID()
        {
            var adminuserid = User.Identity.GetUserId();

            string transferids = GenerateUniqueTransectionID();
            var transferid = "AA" + transferids;



            TempData["transferadmintoAPI"] = transferid;
            return Json(transferid, JsonRequestBehavior.AllowGet);
        }


        public JsonResult FundtransferAdmin_To_Retailer_Generate_Unique_ID()
        {
            var adminuserid = User.Identity.GetUserId();

            string transferids = GenerateUniqueTransectionID();
            var transferid = "AR" + transferids;



            TempData["transferadmintorem"] = transferid;
            return Json(transferid, JsonRequestBehavior.AllowGet);
        }

        [PermissioncheckingAttribute(servicename = "FUNDTRANSFER", permision = "Write")]
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
        #endregion Fundtransfer End

        #region FundUserTOUser
        [PermissioncheckingAttribute(servicename = "FUNDUSERTOUSER", permision = "Read")]
        public ActionResult Fund_User()
        {
            EmployeeFundUserViewModel vmodel = new EmployeeFundUserViewModel();
            var dbData = db.Superstokist_details.ToList();
            vmodel.Masters = GetMasterSelectListItems(dbData);

            vmodel.master_to_dealer_report_BYAdmin_Result = db.Master_to_dealer_report_BYAdmin(DateTime.Today, DateTime.Today.AddDays(1), "ALL", "ALL", 1, 5000).ToList();
            var dbdealerData = db.Dealer_Details.ToList();
            vmodel.Dealesrs = GetDealerlistSelectListItems(dbdealerData);
            var dbRetailerData = db.Retailer_Details.Where(x => x.ISDeleteuser == false).ToList();
            vmodel.Retailerslist = GetRetailerlistSelectListItems(dbRetailerData);
            return View(vmodel);
        }


        //----------------------------------master----------------------------------------
        /////////////////////////////////Master//////////////////////////////////
        #region MasterUserList

        public ActionResult ExcellGenerateForMaster(string usernm)
        {
            DataTable dt = new DataTable("Grid");
            dt.Columns.AddRange(new DataColumn[7] {
                                            new DataColumn("Master Distributor Firm"),
                                            new DataColumn("Master Distributor Name"),
                                            new DataColumn("Mobile No"),
                                            new DataColumn("PANNo"),
                                            new DataColumn("M/D Bal"),
                                            new DataColumn("KYC"),
                                            new DataColumn("Email V/F") });
            // var retaiulerlist = db.Select_Retailer_Details_all("ADMIN", "ADMIN", "ADMIN").ToList();
            //if (!string.IsNullOrEmpty(usernm))
            //{
            //   var retaiulerlist1 = retaiulerlist.Where(x=>x.Frm_Name.ToUpper().Contains(usernm) || x.RetailerName.ToUpper().Contains(usernm) || x.Mobile.ToUpper().Contains(usernm) ||x.PanCard.ToUpper().Contains(usernm));
            //}

            MasterDistributerModel viewModel = new MasterDistributerModel();
            var Details = db.Select_super_total().ToList();
            var listall = new List<Select_super_total_Result>();
            if (!string.IsNullOrEmpty(usernm))
            {
                var masterlist = Details.Where(x => !string.IsNullOrEmpty(x.SuperstokistName) && x.SuperstokistName.ToUpper().Contains(usernm.ToUpper())).ToList();
                listall = masterlist;
                if (listall.Count == 0)
                {
                    var filterbyfrm = Details.Where(x => x.FarmName.ToUpper().Contains(usernm.ToUpper())).ToList();
                    listall = filterbyfrm;
                }
                if (listall.Count == 0)
                {
                    var filterbymob = Details.Where(x => x.Mobile.ToUpper().Contains(usernm.ToUpper())).ToList();
                    listall = filterbymob;
                }


            }
            else
            {
                var dealerlist = Details;
                listall = dealerlist;
            }
            // viewmodel.select_retailer_details = listall;



            foreach (var item in listall)
            {
                var psaveri = item.PSAStatus == "Y" && item.AadhaarStatus == "Y" ? "Done" : "Due";
                var emailssvarify = item.EmailConfirmed == true ? "Done" : "Due";
                dt.Rows.Add(item.FarmName, item.SuperstokistName, item.Mobile, item.pancard, item.superremain, psaveri, emailssvarify);
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.ColumnWidth = 24.14;
                wb.Worksheets.Add(dt);

                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "MasterDistributorlist.xlsx");
                }
            }


        }

        public ActionResult PrintMasterPartialViewToPdf(string usernm)
        {
            MasterDistributerModel viewModel = new MasterDistributerModel();
            var Details = db.Select_super_total().ToList();
            var listall = new List<Select_super_total_Result>();
            if (!string.IsNullOrEmpty(usernm))
            {
                var masterlist = Details.Where(x => !string.IsNullOrEmpty(x.SuperstokistName) && x.SuperstokistName.ToUpper().Contains(usernm.ToUpper())).ToList();
                listall = masterlist;
                if (listall.Count == 0)
                {
                    var filterbyfrm = Details.Where(x => x.FarmName.ToUpper().Contains(usernm.ToUpper())).ToList();
                    listall = filterbyfrm;
                }
                if (listall.Count == 0)
                {
                    var filterbymob = Details.Where(x => x.Mobile.ToUpper().Contains(usernm.ToUpper())).ToList();
                    listall = filterbymob;
                }


            }
            else
            {
                var dealerlist = Details;
                listall = dealerlist;
            }
            viewModel._Select_super_total_Result = listall;



            //var report = new PartialViewAsPdf("_PDFMaster_list", viewModel);
            //return report;

            return View("_PDFMaster_list", viewModel); // Just to check if rest works



        }

        [PermissioncheckingAttribute(servicename = "USERS", permision = "Read")]
        public ActionResult Master_list()
        {
            ViewData["value"] = "";
            ViewBag.state = new SelectList(db.Select_State_Details(), "State_Id", "State_Name").ToList();
            ViewBag.state1 = new SelectList(db.Select_State_Details(), "State_Id", "State_Name").ToList();

            var district = from s in db.District_Desc
                           where s.State_id == 0
                           select s;
            ViewBag.district = new SelectList(district, "Dist_id", "Dist_Desc").ToList();

            var results = (from p in db.Slab_name where p.SlabFor == "Master" group p.SlabName by p.SlabName into g select new { SlabName = g.Key });
            ViewBag.slabname = new SelectList(results, "SlabName", "SlabName");

            var ch = db.Select_super_total();
            MasterDistributerModel viewModel = new MasterDistributerModel();
            viewModel._Select_super_total_Result = ch;
            return View(viewModel);
        }


        [HttpPost]
        [PermissioncheckingAttribute(servicename = "USERS", permision = "Read")]

        public ActionResult Master_list(string MID)
        {
            if (MID != "" && MID != null)
            {
                ViewData["value"] = "OK";
            }
            ViewBag.state = new SelectList(db.Select_State_Details(), "State_Id", "State_Name").ToList();
            var results = (from p in db.master_slab where p.slabName != "Default" group p.slabName by p.slabName into g select new { SlabName = g.Key });
            ViewBag.slabname = new SelectList(results, "slabName", "slabName");
            var ch = db.Select_super_total();
            MasterDistributerModel viewModel = new MasterDistributerModel();
            viewModel._Select_super_total_Result = ch;
            viewModel.show_master_dealer = db.show_master_dealer(MID).ToList();
            return View(viewModel);
        }

        public PartialViewResult MasterlistPart()
        {

            ViewData["value"] = "";
            ViewBag.state = new SelectList(db.Select_State_Details(), "State_Id", "State_Name").ToList();
            ViewBag.state1 = new SelectList(db.Select_State_Details(), "State_Id", "State_Name").ToList();
            var district = from s in db.District_Desc
                           where s.State_id == 0
                           select s;
            ViewBag.district = new SelectList(district, "Dist_id", "Dist_Desc").ToList();

            var results = (from p in db.Slab_name where p.SlabFor == "Master" group p.SlabName by p.SlabName into g select new { SlabName = g.Key });
            ViewBag.slabname = new SelectList(results, "SlabName", "SlabName");
            var ch = db.Select_super_total();
            TempData.Keep("msgrem");
            MasterDistributerModel viewModel = new MasterDistributerModel();
            viewModel._Select_super_total_Result = ch;

            return PartialView("_Masterlist", viewModel);
        }

        [HttpPost]
        public PartialViewResult _SelectDlmID(string MdId)
        {
            var Details = db.Select_Dealer_total(MdId).ToList();
            MasterDistributerModel dlmViewModel = new MasterDistributerModel();
            dlmViewModel.Select_dealer_list = Details;
            return PartialView("_SelectDlmID", dlmViewModel);
        }
        [HttpPost]
        public async Task<ActionResult> insert_master_super(MasterDistributerModel model, string role)
        {
            string message = "";
            var appDbContext = HttpContext.GetOwinContext().Get<ApplicationDbContext>();
            using (var transaction = appDbContext.Database.BeginTransaction())
            {
                try
                {

                    ViewBag.state = new SelectList(db.Select_State_Details(), "State_Id", "State_Name").ToList();
                    var chckmobile = db.Users.Where(a => a.PhoneNumber == model.Mb).Any();
                    if (chckmobile == true)
                    {
                        //  TempData["mobileno"] = "This Mobile Number Already Exists.";
                        message = "This Mobile Number Already Exists.";
                        return Json(new { status = "Error", message = message }, JsonRequestBehavior.AllowGet); //RedirectToAction("Master_list");
                    }
                    var check = db.Superstokist_details.Where(es => es.Mobile == model.Mb).Any();
                    if (check == false)
                    {
                        //    if (db.Superstokist_details.Any(u => u.Mobile != model.Mb.ToString()))
                        //{
                        var user = new ApplicationUser { UserName = model.Email, Email = model.Email, PhoneNumber = model.Mb.ToString() };
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


                        model.Password = pass.ToString();
                        model.ConfirmPassword = pass.ToString();
                        var result = await UserManager.CreateAsync(user, model.Password);
                        if (result.Succeeded)
                        {
                            System.Data.Entity.Core.Objects.ObjectParameter output = new
                     System.Data.Entity.Core.Objects.ObjectParameter("Output", typeof(string));

                            var ch = db.Insert_SuperStokist(user.Id, model.SuperstokistName, model.FarmName, model.State, model.District, model.Mb.ToString(), model.Address, model.Pincode, model.Email,
                               "", string.IsNullOrWhiteSpace(model.pancard) ? "" : model.pancard, string.IsNullOrWhiteSpace(model.adharcard) ? "" : model.adharcard, string.IsNullOrWhiteSpace(model.gst) ? "" : model.gst, "", role, output).Single().msg.ToString();
                            var adminemail = db.Admin_details.SingleOrDefault().email;

                            var AdminMailId = db.Admin_details.Single().email;
                            var statusAdmin = db.PushNotificationStatus.Where(a => a.UserRole == "Admin").SingleOrDefault().Status;
                            var statusSendSmsMaster = db.SMSSendAlls.Where(a => a.ServiceName == "MasterCreate").SingleOrDefault();
                            var statusSendEmailMaster = db.EmailSendAlls.Where(a => a.ServiceName == "MasterCreate1").SingleOrDefault().Status;
                            if (ch.Contains("successfully"))
                            {
                                string passsss = HttpUtility.UrlEncode(model.Password);
                                transaction.Commit();

                                // Send an email with this link
                                string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                                var callbackUrl = Url.Action("ConfirmEmailAdmin", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                                callbackUrl = callbackUrl.Replace("/ADMIN", "");
                                string body = new CommonUtil().PopulateBodyDealer("", "Confirm your account", "", "" + callbackUrl + "", model.Email, model.Password);
                                new CommonUtil().Insertsendmail(model.Email, "Confirm your account", body, callbackUrl);

                                string Welcomebody = new CommonUtil().PopulateBodyWelcome("", "Confirm your account", "", "" + callbackUrl + "", model.Email, model.Password);
                                new CommonUtil().InsertsendmailWelcome(model.Email, "Confirm your account", Welcomebody, callbackUrl);

                                new CommonUtil().Rsendmailadmin(adminemail, "Confirm your account", body, callbackUrl);

                                ResendConfirmMail resend = new ResendConfirmMail();
                                resend.CallBackUrl = callbackUrl;
                                resend.Email = model.Email;
                                resend.Password = model.Password;
                                resend.Pin = "";
                                db.ResendConfirmMails.Add(resend);
                                db.SaveChanges();

                                //if (statusSendSmsMaster == "Y")
                                //{
                                //    string msgssss = "";
                                //    string tempid = "";
                                //    string urlss = "";

                                //    var smsapionsts = db.apisms.Where(x => x.sts == "Y").SingleOrDefault();
                                //    var smsstypes = db.Sending_SMS_Templates.Where(x => x.SMS_TYPE == "ADMIN_CREATE_NEW_USERS" && x.SMSAPIID == smsapionsts.id).SingleOrDefault();
                                //    if (smsstypes != null)
                                //    {



                                //        msgssss = string.Format(smsstypes.Templates, model.Mb, passsss);
                                //        tempid = smsstypes.Templateid;
                                //        urlss = smsapionsts.smsapi;

                                //        smssend.sendsmsallnew(model.Mb, msgssss, urlss, tempid);
                                //    }


                                //    //smssend.sendsmsall(model.Mb, "Dear Partner ! Welcome Your user Id " + model.Mb + " and Password " + passsss + ". Thanks For Your Business . ", "User Create");
                                //}

                                smssend.sms_init(statusSendSmsMaster.Status, statusSendSmsMaster.Whatsapp_Status, "ADMIN_CREATE_NEW_USERS", model.Mb, model.Mb + " ", passsss);

                                if (statusSendEmailMaster == "Y")
                                {
                                    smssend.SendEmailAll(model.Email, "Dear Partner ! Welcome Your user Id " + model.Mb + " and Password " + passsss + ". Thanks For Your Business . ", "User Create", AdminMailId);
                                }

                                if (statusAdmin == "Y")
                                {
                                    SendPushNotification(AdminMailId, Url.Action("Master_list", "Home"), "The New Master Id " + model.Email + " is Created Successfully.", "Master ID Create.");
                                }
                                // TempData["msgrem"] = ch;
                                return Json(new { status = "Success", message = ch }, JsonRequestBehavior.AllowGet);
                                //  return RedirectToAction("Master_list");
                            }
                            else
                            {
                                transaction.Rollback();
                                // TempData["Error"] = "User Not Created. Please Create After Some Time.";
                                message = "User Not Created. Please Create After Some Time.";
                                return Json(new { status = "Error", message = message }, JsonRequestBehavior.AllowGet);
                                // return RedirectToAction("Master_list");
                            }

                        }
                        else
                        {
                            // TempData["emailconfrim"] = "This Email Id Allready Exists.";
                            message = "This Email Id Allready Exists.";
                        }
                        return Json(new { status = "Error", message = message }, JsonRequestBehavior.AllowGet);
                        // return RedirectToAction("Master_list");
                    }
                    else
                    {
                        // TempData["mobileno"] = "This Mobile Number Already Exists.";
                        // return RedirectToAction("Master_list");
                        message = "This Mobile Number Already Exists.";
                        return Json(new { status = "Error", message = message }, JsonRequestBehavior.AllowGet);

                    }

                }
                catch (Exception ex)
                {
                    transaction.UnderlyingTransaction.Connection.Open();
                    transaction.Rollback();
                    message = "User Not Created. Please Create After Some Time.";
                    return Json(new { status = "Error", message = message }, JsonRequestBehavior.AllowGet);

                    // TempData["Error"] = "User Not Created. Please Create After Some Time.";
                    // return RedirectToAction("Master_list");
                }
            }
        }
        [HttpPost]
        public ActionResult Edit_master_dealer(string ssid, string NameEdit, string FirmEdit, string pancardEdit, string AadhaarEdit, string gstEdit, string StateEdit, string DistrictEdit, string AddressEdit, string Editddlrole, string PinEdit, decimal? CappingEdit, decimal? offmargineEdit)
        {
            try
            {
                var super = db.Superstokist_details.Where(p => p.SSId == ssid).SingleOrDefault();
                super.SuperstokistName = NameEdit;
                super.FarmName = FirmEdit;
                super.pancard = pancardEdit;
                super.adharcard = AadhaarEdit;
                super.gst = gstEdit;
                super.State = Convert.ToInt32(StateEdit);
                super.District = Convert.ToInt32(DistrictEdit);
                super.Address = AddressEdit;
                super.RolesType = Editddlrole;
                super.Pincode = Convert.ToInt32(PinEdit);
                super.caption = (decimal)CappingEdit;
                db.SaveChanges();
                var marginsetmaster = db.admin_to_master_margine.Where(x => x.ssid == ssid).FirstOrDefault();
                marginsetmaster.marginecomm = offmargineEdit;
                db.SaveChanges();
                ViewBag.state = new SelectList(db.Select_State_Details(), "State_Id", "State_Name").ToList();
                ViewBag.state1 = new SelectList(db.Select_State_Details(), "State_Id", "State_Name").ToList();
                var district = from s in db.District_Desc
                               where s.State_id == 0
                               select s;
                ViewBag.district = new SelectList(district, "Dist_id", "Dist_Desc").ToList();
                MasterDistributerModel viewModel = new MasterDistributerModel();
                var ch = db.Select_super_total();
                viewModel._Select_super_total_Result = ch;
                return PartialView("_Masterlist", viewModel);

            }
            catch
            {
                return RedirectToAction("Master_list");
            }
        }

        public ActionResult emailverify(string ssid)
        {
            var usersts = db.Users.Where(aa => aa.UserId == ssid).SingleOrDefault();
            usersts.EmailConfirmed = true;
            db.SaveChanges();
            ViewBag.state = new SelectList(db.Select_State_Details(), "State_Id", "State_Name").ToList();
            ViewBag.state1 = new SelectList(db.Select_State_Details(), "State_Id", "State_Name").ToList();
            var district = from s in db.District_Desc
                           where s.State_id == 0
                           select s;
            ViewBag.district = new SelectList(district, "Dist_id", "Dist_Desc").ToList();
            MasterDistributerModel viewModel = new MasterDistributerModel();
            var ch = db.Select_super_total();
            viewModel._Select_super_total_Result = ch;
            return PartialView("_Masterlist", viewModel);

        }

        public ActionResult ConfirmDeleteOTP(string ssiddelete, string Deleteotp)
        {

            var chk = db.MobileOtps.Where(aa => aa.Otp == Deleteotp).Take(1).OrderByDescending(aa => aa.Date).SingleOrDefault();
            MasterDistributerModel viewModel = new MasterDistributerModel();
            try
            {
                var isdealerexist = db.Dealer_Details.Where(s => s.SSId == ssiddelete).ToList();
                if (isdealerexist.Count > 0)
                {
                    throw new Exception("Not Allow To Delete, First Change The Distributor.");
                }
                if (chk != null)
                {
                    db.delete_Master(ssiddelete);

                    //for logout from all devices web
                    UserManager.UpdateSecurityStamp(ssiddelete);

                    ViewBag.state = new SelectList(db.Select_State_Details(), "State_Id", "State_Name").ToList();
                    ViewBag.state1 = new SelectList(db.Select_State_Details(), "State_Id", "State_Name").ToList();
                    var district = from s in db.District_Desc
                                   where s.State_id == 0
                                   select s;
                    ViewBag.district = new SelectList(district, "Dist_id", "Dist_Desc").ToList();

                    var ch = db.Select_super_total();
                    viewModel._Select_super_total_Result = ch;
                    return PartialView("_Masterlist", viewModel);
                }
                else
                {
                    throw new HttpException(404, "Product not found");
                    // return Json("failed", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }

        }





        #endregion
        /////////////////////////////////Master//////////////////////////////////
        /////////////////////////////////DistibutorList///////////////////////////////
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        [PermissioncheckingAttribute(servicename = "USERS", permision = "Read")]
        public ActionResult DistibutorList()
        {
            var results = (from p in db.Slab_name where p.SlabFor == "Distributor" group p.SlabName by p.SlabName into g select new { SlabName = g.Key });
            ViewBag.slabname = new SelectList(results, "SlabName", "SlabName");
            ViewBag.state = new SelectList(db.Select_State_Details(), "State_Id", "State_Name").ToList();
            ViewBag.state1 = new SelectList(db.Select_State_Details(), "State_Id", "State_Name").ToList();
            var district = from s in db.District_Desc
                           where s.State_id == 0
                           select s;
            ViewBag.district = new SelectList(district, "Dist_id", "Dist_Desc").ToList();

            var Details = db.Select_Dealer_total("ADMIN").Where(x => x.masterid == db.Admin_details.SingleOrDefault().userid).ToList();
            DealerModel viewModel = new DealerModel();
            viewModel.Select_dealer_list = Details;
            var stands = db.Superstokist_details.ToList();
            IEnumerable<SelectListItem> selectList = from s in stands
                                                     select new SelectListItem
                                                     {
                                                         Value = s.SSId,
                                                         Text = s.Email + " -- " + s.FarmName.ToString()
                                                     };
            ViewBag.masterid = new SelectList(selectList, "Value", "Text");
            TempData.Keep("Message");
            ViewData["ResendMail"] = TempData["ResendMAil"];
            return View(viewModel);
        }
        public ActionResult DistibutorListforseller()
        {
            var set = db.Recharge_sell_by_dealer.Where(s => s.registration_Status == true && s.Status == true).ToList();
            ViewBag.dealerdetailess = db.Dealer_Details.ToList();
            return View(set);
        }
        [HttpPost]
        public ActionResult DistibutorListforseller(string Dealertdd)
        {
            var set = db.Recharge_sell_by_dealer.Where(s => s.registration_Status == true && s.Status == true && s.Dealerid.Contains(Dealertdd)).ToList();
            ViewBag.dealerdetailess = db.Dealer_Details.ToList();
            return View(set);
        }


        public ActionResult ExcellGenerateForDistibutor(string usernm)
        {
            DataTable dt = new DataTable("Grid");
            dt.Columns.AddRange(new DataColumn[8] {

                                              new DataColumn(" Master Distributor Firm"),
                                            new DataColumn("DistributorFirm"),
                                            new DataColumn("Distributor Name"),
                                            new DataColumn("Mobile No"),
                                            new DataColumn("Aadhar No"),
                                            new DataColumn("PANNo"),
                                            new DataColumn("KYC"),
                                            new DataColumn("Email V/F") });


            var Details = db.Select_Dealer_total("ADMIN").Where(x => x.masterid == db.Admin_details.SingleOrDefault().userid).ToList();
            var listall = new List<Select_Dealer_total_Result>();
            if (!string.IsNullOrEmpty(usernm))
            {
                var dealerlist = Details.Where(x => !string.IsNullOrEmpty(x.FarmName) && x.FarmName.ToUpper().Contains(usernm.ToUpper())).ToList();
                listall = dealerlist;
                if (listall.Count == 0)
                {
                    var filterbyname = Details.Where(x => x.DealerName.ToUpper().Contains(usernm.ToUpper())).ToList();
                    listall = filterbyname;
                }
                if (listall.Count == 0)
                {
                    var filterbymob = Details.Where(x => x.Mobile.ToUpper().Contains(usernm.ToUpper())).ToList();
                    listall = filterbymob;
                }


            }
            else
            {
                var dealerlist = Details;
                listall = dealerlist;
            }
            // viewmodel.select_retailer_details = listall;



            foreach (var item in listall)
            {
                var psaveri = item.PSAStatus == "Y" && item.AadhaarStatus == "Y" && item.pancardsts == "Y" ? "Done" : "Due";
                var emailssvarify = item.Emailconfirmed == true ? "Done" : "Due";
                dt.Rows.Add(item.MasterEmail, item.FarmName, item.DealerName, item.Mobile, item.adharcard, item.pancard, psaveri, emailssvarify);
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.ColumnWidth = 24.14;
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Dealerlist.xlsx");
                }
            }


        }

        public ActionResult Stscanhgedlmsell(string userid, bool? status)
        {
            try
            {
                var dlmsell = db.Recharge_sell_by_dealer.Where(s => s.Dealerid == userid).SingleOrDefault();
                dlmsell.Status = status;
                db.SaveChanges();
            }
            catch
            {
                var dlmsell = db.Recharge_sell_by_dealer.Where(s => s.Dealerid == userid).SingleOrDefault();
                status = dlmsell.Status;
            }

            return Json(status, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PrintDistibutorPartialViewToPdf(string usernm)
        {
            DealerModel viewModel = new DealerModel();
            var Details = db.Select_Dealer_total("ADMIN").Where(x => x.masterid == db.Admin_details.SingleOrDefault().userid).ToList();
            var listall = new List<Select_Dealer_total_Result>();
            if (!string.IsNullOrEmpty(usernm))
            {
                var dealerlist = Details.Where(x => !string.IsNullOrEmpty(x.FarmName) && x.FarmName.ToUpper().Contains(usernm.ToUpper())).ToList();
                listall = dealerlist;
                if (listall.Count == 0)
                {
                    var filterbyname = Details.Where(x => x.DealerName.ToUpper().Contains(usernm.ToUpper())).ToList();
                    listall = filterbyname;
                }
                if (listall.Count == 0)
                {
                    var filterbymob = Details.Where(x => x.Mobile.ToUpper().Contains(usernm.ToUpper())).ToList();
                    listall = filterbymob;
                }


            }
            else
            {
                var dealerlist = Details;
                listall = dealerlist;
            }
            viewModel.Select_dealer_list = listall;

            var report = new PartialViewAsPdf("_PDFDistibutorlist", viewModel);
            return report;

        }



        [HttpPost]
        public PartialViewResult SelectRetailerID(string MdId)
        {
            var Details = db.Dealer_retailer(MdId, "Distibutor", 1, 3000).ToList();
            MasterDistributerModel RIViewModel = new MasterDistributerModel();
            RIViewModel.Select_Retailer_list = Details;
            return PartialView("_SelectRID", RIViewModel);
        }


        public JsonResult DealerByIdSearch(string ssid)
        {
            ViewData["value"] = "";
            ViewBag.state = new SelectList(db.Select_State_Details(), "State_Id", "State_Name").ToList();

            var district = from s in db.District_Desc
                           where s.State_id == 0
                           select s;
            ViewBag.district = new SelectList(district, "Dist_id", "Dist_Desc").ToList();

            var results = (from p in db.Slab_name where p.SlabFor == "Distributor" group p.SlabName by p.SlabName into g select new { SlabName = g.Key });
            ViewBag.slabname = new SelectList(results, "SlabName", "SlabName");
            int stateid = db.Select_Dealer_total("ADMIN").Where(x => x.masterid == db.Admin_details.SingleOrDefault().userid).Where(ii => ii.DealerId == ssid).Select(aa => aa.State).FirstOrDefault();
            ViewBag.state1 = new SelectList(db.Select_State_Details(), "State_Id", "State_Name", stateid).ToList();
            var ch = db.Select_Dealer_total("ADMIN").Where(x => x.masterid == db.Admin_details.SingleOrDefault().userid).Where(ii => ii.DealerId == ssid);
            return Json(new { list = ch, state = ViewBag.state1 }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult ChargeBack_OTP_ON_OFF(string DealerId)
        {
            var stschk = db.Dealer_Details.Where(a => a.DealerId == DealerId).SingleOrDefault();
            var updateotpsts = stschk.chargeback_otpsts == true ? false : true;
            stschk.chargeback_otpsts = updateotpsts;
            db.SaveChanges();

            return Json(new { status = "Success", Message = updateotpsts }, JsonRequestBehavior.AllowGet);

        }


        public ActionResult Self_Upi_STS_ON_OFF(string DealerId)
        {
            var dd = db.Dealer_Details.Where(a => a.DealerId == DealerId).SingleOrDefault();
            var updateUPIsts = dd.DealerSelf_sts == true ? false : true;
            dd.DealerSelf_sts = updateUPIsts;
            db.SaveChanges();

            return Json(new { status = "Success", Message = updateUPIsts }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult ConfirmDeleteDealerOTP(string DealerIddelete, string Deleteotp)
        {
            try
            {
                var chk = db.MobileOtps.Where(aa => aa.Otp == Deleteotp).Take(1).OrderByDescending(aa => aa.Date).SingleOrDefault();
                if (chk != null)
                {
                    db.delete_Dealer(DealerIddelete);

                    //for logout from all devices web
                    UserManager.UpdateSecurityStamp(DealerIddelete);

                    ViewBag.state = new SelectList(db.Select_State_Details(), "State_Id", "State_Name").ToList();
                    ViewBag.state1 = new SelectList(db.Select_State_Details(), "State_Id", "State_Name").ToList();
                    var district = from s in db.District_Desc
                                   where s.State_id == 0
                                   select s;
                    ViewBag.district = new SelectList(district, "Dist_id", "Dist_Desc").ToList();
                    DealerModel viewModel = new DealerModel();
                    viewModel.Select_dealer_list = db.Select_Dealer_total("ADMIN").Where(x => x.masterid == db.Admin_details.SingleOrDefault().userid).ToList(); ;
                    return PartialView("_DistibutorList", viewModel);
                }
                else
                {
                    throw new HttpException(404, "OTP Wronjg");
                    // return Json("failed", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }
        }

        public PartialViewResult DealerlistPart()
        {

            ViewBag.state = new SelectList(db.Select_State_Details(), "State_Id", "State_Name").ToList();
            ViewBag.state1 = new SelectList(db.Select_State_Details(), "State_Id", "State_Name").ToList();
            var district = from s in db.District_Desc
                           where s.State_id == 0
                           select s;
            ViewBag.district = new SelectList(district, "Dist_id", "Dist_Desc").ToList();

            var results = (from p in db.Slab_name where p.SlabFor == "Distributor" group p.SlabName by p.SlabName into g select new { SlabName = g.Key });
            ViewBag.slabname = new SelectList(results, "SlabName", "SlabName");
            var ch = db.Select_super_total();
            TempData.Keep("msgrem");
            DealerModel viewModel = new DealerModel();
            viewModel.Select_dealer_list = db.Select_Dealer_total("ADMIN").Where(x => x.masterid == db.Admin_details.SingleOrDefault().userid).ToList();
            return PartialView("_DistibutorList", viewModel);
        }

        [HttpPost]
        public ActionResult Edit_Distributer_dealer(string DealerId, string NameEdit, string DOBEdit, string FirmEdit, string pancardEdit, string AadhaarEdit, string gstEdit, string StateEdit, string DistrictEdit, string AddressEdit, string Editddlrole, string PinEdit, int CappingEdit, decimal? offmargineEdit)
        {
            DealerModel viewModel = new DealerModel();
            try
            {
                var super = db.Dealer_Details.Where(p => p.DealerId == DealerId).SingleOrDefault();
                super.DealerName = NameEdit;
                super.FarmName = FirmEdit;
                super.pancard = pancardEdit;
                super.adharcard = AadhaarEdit;
                super.gst = gstEdit;
                super.State = Convert.ToInt32(StateEdit);
                super.District = Convert.ToInt32(DistrictEdit);
                super.Address = AddressEdit;
                super.Pincode = Convert.ToInt32(PinEdit);
                super.DOF = Convert.ToDateTime(DOBEdit);
                super.FinincialRolesType = Editddlrole;
                super.caption = CappingEdit;
                db.SaveChanges();
                var dlmmargin = db.master_to_dlm_margine.Where(x => x.dlm == DealerId && x.masterid == db.Admin_details.FirstOrDefault().userid).FirstOrDefault();
                dlmmargin.marginecomm = offmargineEdit == null ? 0 : offmargineEdit;
                db.SaveChanges();
                ViewBag.state = new SelectList(db.Select_State_Details(), "State_Id", "State_Name").ToList();
                ViewBag.state1 = new SelectList(db.Select_State_Details(), "State_Id", "State_Name").ToList();
                var district = from s in db.District_Desc
                               where s.State_id == 0
                               select s;
                ViewBag.district = new SelectList(district, "Dist_id", "Dist_Desc").ToList();

                viewModel.Select_dealer_list = db.Select_Dealer_total("ADMIN").Where(x => x.masterid == db.Admin_details.SingleOrDefault().userid).ToList().ToList();

                return PartialView("_DistibutorList", viewModel);

            }
            catch
            {

                viewModel.Select_dealer_list = db.Select_Dealer_total("ADMIN").Where(x => x.masterid == db.Admin_details.SingleOrDefault().userid).ToList();

                return PartialView("_DistibutorList", viewModel);
            }


        }


        public ActionResult emailverifyfordealer(string dealerid)
        {
            var usersts = db.Users.Where(aa => aa.UserId == dealerid).SingleOrDefault();
            usersts.EmailConfirmed = true;
            db.SaveChanges();
            ViewBag.state = new SelectList(db.Select_State_Details(), "State_Id", "State_Name").ToList();
            ViewBag.state1 = new SelectList(db.Select_State_Details(), "State_Id", "State_Name").ToList();
            var district = from s in db.District_Desc
                           where s.State_id == 0
                           select s;
            ViewBag.district = new SelectList(district, "Dist_id", "Dist_Desc").ToList();
            // var Details = db.Select_Dealer_total("ADMIN").ToList();
            DealerModel viewModel = new DealerModel();
            viewModel.Select_dealer_list = db.Select_Dealer_total("ADMIN").Where(x => x.masterid == db.Admin_details.SingleOrDefault().userid).ToList(); ;

            return PartialView("_DistibutorList", viewModel);

        }

        //\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\Retailer\\\\\\\\\\\\\\\\\\\\\\



        public ActionResult ExcellGenerateForRetailer(string usernm, string usernminput)
        {
            DataTable dt = new DataTable("Grid");
            dt.Columns.AddRange(new DataColumn[10] {
                                            new DataColumn("DistributorFirm"),
                                            new DataColumn("RetailerFirm"),
                                            new DataColumn("RetailerName"),
                                            new DataColumn("MobileNo"),
                                            new DataColumn("Cap♽"),
                                            new DataColumn("Balance"),
                                            new DataColumn("AadharNo"),
                                            new DataColumn("PANNo"),
                                            new DataColumn("KYC"),
                                            new DataColumn("Email V/F") });
            // var retaiulerlist = db.Select_Retailer_Details_all("ADMIN", "ADMIN", "ADMIN").ToList();
            //if (!string.IsNullOrEmpty(usernm))
            //{
            //   var retaiulerlist1 = retaiulerlist.Where(x=>x.Frm_Name.ToUpper().Contains(usernm) || x.RetailerName.ToUpper().Contains(usernm) || x.Mobile.ToUpper().Contains(usernm) ||x.PanCard.ToUpper().Contains(usernm));
            //}

            var Details = db.Select_Retailer_Details_all_paging(1, 50000, "ADMIN").ToList();
            var listall = new List<Select_Retailer_Details_all_paging_Result>();
            if (!string.IsNullOrEmpty(usernm))
            {

                var filterbyfrm = Details.Where(x => x.DealerId == usernm).ToList();
                listall = filterbyfrm;
                usernminput = null;
                //var dealerlist = Details.Where(x => !string.IsNullOrEmpty(x.DealerFarmName) && x.DealerFarmName.ToUpper().Contains(usernm.ToUpper())).ToList();
                //listall = dealerlist;
                //if (listall.Count == 0)
                //{
                //    var filterbyfrm = Details.Where(x => x.Frm_Name.ToUpper().Contains(usernm.ToUpper())).ToList();
                //    listall = filterbyfrm;
                //}
                //if (listall.Count == 0)
                //{
                //    var filterbyremname = Details.Where(x => x.RetailerName.ToUpper().Contains(usernm.ToUpper())).ToList();
                //    listall = filterbyremname;
                //}
                //if (listall.Count == 0)
                //{
                //    var filterbymob = Details.Where(x => x.Mobile.ToUpper().Contains(usernm.ToUpper())).ToList();
                //    listall = filterbymob;
                //}


            }
            else if (!string.IsNullOrEmpty(usernminput))
            {
                var dealerlist = Details.Where(x => !string.IsNullOrEmpty(x.DealerFarmName) && x.DealerFarmName.ToUpper().Contains(usernminput.ToUpper())).ToList();
                listall = dealerlist;

                if (listall.Count == 0)
                {
                    var filterbymob = Details.Where(x => x.Mobile.ToUpper().StartsWith(usernminput.ToUpper())).ToList();
                    listall = filterbymob;
                }
                if (listall.Count == 0)
                {
                    var filterbyfrm = Details.Where(x => x.Frm_Name.ToUpper().Contains(usernminput.ToUpper())).ToList();
                    listall = filterbyfrm;
                }
                if (listall.Count == 0)
                {
                    var filterbyremname = Details.Where(x => x.RetailerName.ToUpper().Contains(usernminput.ToUpper())).ToList();
                    listall = filterbyremname;
                }
                if (listall.Count == 0)
                {
                    var filterbymob = Details.Where(x => x.DealerFarmName.ToUpper().Contains(usernminput.ToUpper())).ToList();
                    listall = filterbymob;
                }
            }
            else
            {
                var dealerlist = Details;
                listall = dealerlist;
            }
            // viewmodel.select_retailer_details = listall;



            foreach (var item in listall)
            {
                var psaveri = item.PSAStatus == "Y" && item.AadhaarStatus == "Y" && item.ShopwithSalfieStatus == "Y" ? "Done" : "Due";
                var emailssvarify = item.EmailConfirmed == true ? "Done" : "Due";
                dt.Rows.Add(item.DealerFarmName, item.Frm_Name, item.RetailerName, item.Mobile, item.caption, item.Remainamount, item.AadharCard, item.PanCard, psaveri, emailssvarify);
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.ColumnWidth = 24.14;
                wb.Worksheets.Add(dt);

                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Retailerlist.xlsx");
                }
            }


        }

        public ActionResult PrintPartialViewToPdf(string usernm, string usernminput)
        {
            RetailerDetalsModel viewmodel = new RetailerDetalsModel();
            var Details = db.Select_Retailer_Details_all_paging(1, 50000, "ADMIN").ToList();
            var listall = new List<Select_Retailer_Details_all_paging_Result>();
            if (!string.IsNullOrEmpty(usernm))
            {
                var dealerlist = Details.Where(x => x.DealerId == usernm).ToList();
                listall = dealerlist;
                //if (listall.Count == 0)
                //{
                //    var filterbyfrm = Details.Where(x => x.Frm_Name.ToUpper().Contains(usernm.ToUpper())).ToList();
                //    listall = filterbyfrm;
                //}
                //if (listall.Count == 0)
                //{
                //    var filterbyremname = Details.Where(x => x.RetailerName.ToUpper().Contains(usernm.ToUpper())).ToList();
                //    listall = filterbyremname;
                //}
                //if (listall.Count == 0)
                //{
                //    var filterbymob = Details.Where(x => x.Mobile.ToUpper().Contains(usernm.ToUpper())).ToList();
                //    listall = filterbymob;
                //}
                usernminput = null;

            }
            else if (!string.IsNullOrEmpty(usernminput))
            {
                var dealerlist = Details.Where(x => !string.IsNullOrEmpty(x.DealerFarmName) && x.DealerFarmName.ToUpper().Contains(usernminput.ToUpper())).ToList();
                listall = dealerlist;

                if (listall.Count == 0)
                {
                    var filterbymob = Details.Where(x => x.Mobile.ToUpper().StartsWith(usernminput.ToUpper())).ToList();
                    listall = filterbymob;
                }
                if (listall.Count == 0)
                {
                    var filterbyfrm = Details.Where(x => x.Frm_Name.ToUpper().Contains(usernminput.ToUpper())).ToList();
                    listall = filterbyfrm;
                }
                if (listall.Count == 0)
                {
                    var filterbyremname = Details.Where(x => x.RetailerName.ToUpper().Contains(usernminput.ToUpper())).ToList();
                    listall = filterbyremname;
                }
                if (listall.Count == 0)
                {
                    var filterbymob = Details.Where(x => x.DealerFarmName.ToUpper().Contains(usernminput.ToUpper())).ToList();
                    listall = filterbymob;
                }
            }
            else
            {
                var dealerlist = Details;
                listall = dealerlist;
            }
            viewmodel.select_retailer_details_paging = listall;

            var report = new PartialViewAsPdf("_PDFRetailerlist", viewmodel);
            return report;

        }

        public ActionResult retailerekyccheck()
        {
            var results = db.retailer_Ekycreport().ToList();
            return View("retailerekyccheck", results);
        }

        public ActionResult Excel_retailerekyccheck()
        {
            var results = db.retailer_Ekycreport().ToList();

            DataTable dataTbl = new DataTable();
            dataTbl.Columns.Add("Status", typeof(string));
            dataTbl.Columns.Add("Firm Name", typeof(string));
            dataTbl.Columns.Add("Email Id", typeof(string));
            dataTbl.Columns.Add("Mobile No", typeof(string));
            dataTbl.Columns.Add("Adharcard No", typeof(string));
            dataTbl.Columns.Add("Pancard", typeof(string));
            dataTbl.Columns.Add("Merchant Id", typeof(string));
            dataTbl.Columns.Add("e-kyc Mobile", typeof(string));
            dataTbl.Columns.Add("e-kyc Adhaar", typeof(string));
            dataTbl.Columns.Add("In Date", typeof(string));
            dataTbl.Columns.Add("Update Date", typeof(string));
            dataTbl.Columns.Add("Device Info", typeof(string));

            if (results.Count > 0)
            {
                foreach (var item in results)
                {
                    var sts = "";
                    if (item.ekycstatus == true)
                    {
                        sts = "verify";
                    }
                    else if (item.ekycstatus == false)
                    {
                        sts = "unverify";
                    }
                    else
                    {
                        sts = "unregisterd";
                    }

                    dataTbl.Rows.Add(sts, item.frm_name, item.email, item.mobile, item.AadharCard, item.Pancard, item.AepsMerchandId, item.ekycmobile, item.ekycadhaar, item.insertdate, item.updatedate, item.devicename + " " + item.devicesrno);
                }
            }
            else
            {
                dataTbl.Rows.Add("", "", "", "", "", "", "", "", "", "", "", "");
            }
            var grid = new GridView();
            grid.DataSource = dataTbl;
            grid.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=Excel_retailer_ekyc.xls");
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

        public ActionResult PDF_retailerekyccheck()
        {
            var results = db.retailer_Ekycreport().ToList();
            return new ViewAsPdf(results);
        }


        [HttpPost]
        [ValidateInput(false)]


        public JsonResult RetailerByIdSearch(string RetailerId)
        {
            ViewData["value"] = "";

            var district = from s in db.District_Desc
                           where s.State_id == 0
                           select s;

            var ch = db.Select_Retailer_Details_all_paging(1, 1, RetailerId).ToList();
            if (RetailerId != null)
            {
                ch = ch.Where(s => s.RetailerId == RetailerId).ToList();
            }
            int stateid = db.Select_Retailer_Details_all_paging(1, 1, RetailerId).Select(aa => aa.State).FirstOrDefault();
            ViewBag.state1 = new SelectList(db.Select_State_Details(), "State_Id", "State_Name", stateid).ToList();
            var poscp = db.Retailer_Details.Where(s => s.RetailerId == RetailerId).SingleOrDefault().POS_CAPPING;
            if (poscp == null)
            {
                poscp = 0;
            }
            return Json(new { list = ch, state = ViewBag.state1, poscapp = poscp }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult emailverifyforRetailer(string RetailerId, string Type)
        {
            var usersts = db.Users.Where(aa => aa.UserId == RetailerId).SingleOrDefault();
            if (Type == "Email")
            {
                usersts.EmailConfirmed = true;
            }
            else
            {
                usersts.PhoneNumberConfirmed = true;
            }
            db.SaveChanges();
            ViewData["value"] = "";
            ViewBag.state = new SelectList(db.Select_State_Details(), "State_Id", "State_Name").ToList();
            ViewBag.state1 = new SelectList(db.Select_State_Details(), "State_Id", "State_Name").ToList();
            var district = from s in db.District_Desc
                           where s.State_id == 0
                           select s;
            ViewBag.district = new SelectList(district, "Dist_id", "Dist_Desc").ToList();

            var results = (from p in db.Slab_name where p.SlabFor == "Retailer" group p.SlabName by p.SlabName into g select new { SlabName = g.Key });

            ViewBag.slabname = new SelectList(results, "SlabName", "SlabName");

            var ch = db.Select_Retailer_Details_all_paging(1, 1, RetailerId);
            RetailerDetalsModel viewmodel = new RetailerDetalsModel();
            viewmodel.select_retailer_details_paging = ch.ToList();
            viewmodel.Show_Service_namelist = db.Show_Service_name.ToList();
            viewmodel.Service_BlockUserwiseclslist = from tbl in db.Show_Service_name
                                                     join tbl1 in db.Service_BlockUserwise
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
            return PartialView("_Retailerlist", viewmodel);

        }



        //RetailerList
        [HttpGet]

        [PermissioncheckingAttribute(servicename = "USERS", permision = "Read")]
        public ActionResult Retailerlist()
        {
            ViewBag.state = new SelectList(db.Select_State_Details(), "State_Id", "State_Name").ToList();
            ViewBag.state1 = new SelectList(db.Select_State_Details(), "State_Id", "State_Name").ToList();
            ViewBag.Dealername = new SelectList(db.select_Dealer_for_ddl(), "Dealerid", "FarmName", null);

            ViewData["msg"] = TempData["msgshow"];
            var Details = db.Select_Retailer_Details_all_paging(1, 50, "ADMIN").ToList();
            var results = (from p in db.Slab_name where p.SlabFor == "Retailer" group p.SlabName by p.SlabName into g select new { SlabName = g.Key });
            //ViewBag.slab = new SelectList(results, "SlabName", "SlabName");
            // ViewBag.slabedit = new SelectList(results, "SlabName", "SlabName");
            //var slab = (from p in db.retailer_slab where p.retailer_id != "Default" group p.retailer_id by p.retailer_id into g select new { retailer_id = g.Key });
            //ViewBag.slab = new SelectList(slab, "retailer_id", "retailer_id");
            //ViewBag.slabedit = new SelectList(slab, "retailer_id", "retailer_id");
            var state = db.Select_State_Details().ToList();
            TempData.Remove("Message");
            ViewData["ResendMail"] = TempData["ResendMAil"];
            RetailerDetalsModel viewmodel = new RetailerDetalsModel();
            List<SelectListItem> items = new List<SelectListItem>();
            //foreach (var item in db.select_Dealer_for_ddl())
            //{
            //    items.Add(new SelectListItem { Text = item.FarmName, Value = item.DealerId.ToString() });

            //}
            //viewmodel.Dealerlist = items;

            viewmodel.Dealerlist = new SelectList(db.select_Dealer_for_ddl(), "DealerId", "FarmName").ToList();
            viewmodel.Show_Service_namelist = db.Show_Service_name.ToList();
            viewmodel.select_retailer_details_paging = Details;
            viewmodel.Service_BlockUserwiseclslist = from tbl in db.Show_Service_name
                                                     join tbl1 in db.Service_BlockUserwise
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
            return View(viewmodel);

        }
        public ActionResult InfiniteScroll_retailer(int pageindex)
        {
            int pagesize = 50;
            var ch = db.Select_Retailer_Details_all_paging(pageindex, pagesize, "ADMIN").ToList();
            RetailerDetalsModel viewmodel = new RetailerDetalsModel();
            viewmodel.select_retailer_details_paging = ch;
            viewmodel.Show_Service_namelist = db.Show_Service_name.ToList();

            viewmodel.Service_BlockUserwiseclslist = from tbl in db.Show_Service_name
                                                     join tbl1 in db.Service_BlockUserwise
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
            JsonModel jsonmodel = new JsonModel();
            jsonmodel.NoMoredata = ch.Count < pagesize;
            jsonmodel.HTMLString = renderPartialViewtostring("_Retailerlist_Paging", viewmodel);
            return Json(jsonmodel);
        }




        [HttpPost]


        public ActionResult ApprovedRetailersts(string val, string RetailerId)
        {

            var Retailerdetail = db.Retailer_Details.Where(aa => aa.RetailerId == RetailerId).SingleOrDefault();
            if (val == "pan")
            {
                Retailerdetail.PSAStatus = "Y";
            }
            else if (val == "aadhar")
            {
                Retailerdetail.AadhaarStatus = "Y";
            }
            else if (val == "gst")
            {
                Retailerdetail.gststatus = "Y";
            }
            else if (val == "Aggrement")
            {
                Retailerdetail.IsAgreement = true;

            }
            else if (val == "Salfie")
            {
                Retailerdetail.ShopwithSalfieStatus = "Y";

            }
            else if (val == "VIDEOKYC")
            {
                Retailerdetail.videokycstatus = "Y";

            }
            var res = db.SaveChanges();
            if (res > 0)
            {


                return Json(val, JsonRequestBehavior.AllowGet);
            }
            return Json("sdfdsgdf", JsonRequestBehavior.AllowGet);
            //RetailerDetalsModel viewmodel = new RetailerDetalsModel();
            //ViewBag.state = new SelectList(db.Select_State_Details(), "State_Id", "State_Name").ToList();
            //ViewBag.state1 = new SelectList(db.Select_State_Details(), "State_Id", "State_Name").ToList();
            //var district = from s in db.District_Desc
            //               where s.State_id == 0
            //               select s;
            //ViewBag.district = new SelectList(district, "Dist_id", "Dist_Desc").ToList();

            //viewmodel.select_retailer_details = db.Select_Retailer_Details_all("ADMIN", "ADMIN", "ADMIN").ToList();

            //return PartialView("_Retailerlist", viewmodel);


        }

        public ActionResult PanAadharDelete(string hdval, string RetailerIddeletedocument, string Deletedocumentotp)
        {

            try
            {
                var Retailerdetail = db.Retailer_Details.Where(aa => aa.RetailerId == RetailerIddeletedocument).SingleOrDefault();
                var chk = db.MobileOtps.Where(aa => aa.Otp == Deletedocumentotp).Take(1).OrderByDescending(aa => aa.Date).SingleOrDefault();
                if (chk != null)
                {
                    if (hdval == "pan")
                    {
                        Retailerdetail.PSAStatus = "N";
                        Retailerdetail.pancardPath = null;
                    }
                    else if (hdval == "aadhar")
                    {
                        Retailerdetail.AadhaarStatus = "N";
                        Retailerdetail.aadharcardPath = null;
                    }

                    var res = db.SaveChanges();
                    if (res > 0)
                    {
                        RetailerDetalsModel viewmodel = new RetailerDetalsModel();


                        viewmodel.select_retailer_details_paging = db.Select_Retailer_Details_all_paging(1, 1, RetailerIddeletedocument).ToList();
                        viewmodel.Show_Service_namelist = db.Show_Service_name.ToList();
                        viewmodel.Service_BlockUserwiseclslist = from tbl in db.Show_Service_name
                                                                 join tbl1 in db.Service_BlockUserwise
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
                        return PartialView("_Retailerlist", viewmodel);
                    }
                }
                else
                {
                    throw new HttpException(404, "Product not found");

                }
            }
            catch (Exception ewx)
            {
                return Json("WrongOTP", JsonRequestBehavior.AllowGet);
            }

            return Json("WrongOTP", JsonRequestBehavior.AllowGet);
        }



        public ActionResult RejectedPancardAadharRetailersts(string hdval, string RetailerIddeletedocument, string Deletedocumentotp)
        {
            try
            {
                var Retailerdetail = db.Retailer_Details.Where(aa => aa.RetailerId == RetailerIddeletedocument).SingleOrDefault();
                var adminids = db.Admin_details.SingleOrDefault().userid;
                var chk = db.MobileOtps.Where(aa => aa.Userid == adminids).OrderByDescending(aa => aa.Date).Select(a => a.Otp).FirstOrDefault();
                var chktype = db.MobileOtps.Where(aa => aa.Userid == adminids).OrderByDescending(aa => aa.Date).Select(a => a.Type).FirstOrDefault();
                if (chk == Deletedocumentotp && chktype == "DeleteRetailerAADharCard" || chktype == "DeleteRetailerPancard" || chktype == "DeleteRetailerMPOSID")
                {
                    if (hdval == "pan")
                    {
                        Retailerdetail.PSAStatus = "N";
                        Retailerdetail.pancardPath = null;
                    }
                    else if (hdval == "aadhar")
                    {
                        Retailerdetail.AadhaarStatus = "N";
                        Retailerdetail.aadharcardPath = null;
                        Retailerdetail.BackSideAadharcardphoto = null;
                    }
                    else if (hdval == "MPOSID")
                    {

                        Retailerdetail.PartnerID = null;
                    }

                    var res = db.SaveChanges();
                    if (res > 0)
                    {
                        RetailerDetalsModel viewmodel = new RetailerDetalsModel();


                        viewmodel.select_retailer_details_paging = db.Select_Retailer_Details_all_paging(1, 1, RetailerIddeletedocument).ToList();
                        viewmodel.Show_Service_namelist = db.Show_Service_name.ToList();
                        viewmodel.Service_BlockUserwiseclslist = from tbl in db.Show_Service_name
                                                                 join tbl1 in db.Service_BlockUserwise
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
                        return PartialView("_Retailerlist", viewmodel);
                    }
                }
                else
                {
                    throw new HttpException(400, "Product not found");

                }
            }
            catch (Exception ewx)
            {
                Response.StatusCode = 400;
                return Json(ewx.Message, JsonRequestBehavior.AllowGet);
            }
            return Json("drgdhfjgh", JsonRequestBehavior.AllowGet);


        }
        public ActionResult RejectedRetailersts(string val, string RetailerId)
        {
            var Retailerdetail = db.Retailer_Details.Where(aa => aa.RetailerId == RetailerId).SingleOrDefault();
            if (Retailerdetail.vastbazaarsts == "N" || string.IsNullOrEmpty(Retailerdetail.vastbazaarsts))
            {
                if (val == "gst")
                {
                    Retailerdetail.gststatus = "N";
                    Retailerdetail.frimregistrationPath = null;
                }
                else if (val == "Aggrement")
                {
                    Retailerdetail.IsAgreement = false;

                    Retailerdetail.Agreementpath = null;
                }
                else if (val == "Salfie")
                {
                    Retailerdetail.ShopwithSalfieStatus = "N";

                    Retailerdetail.ShopwithSalfie = null;
                }


            }
            if (val == "VIDEOKYC")
            {
                Retailerdetail.videokycstatus = "N";

                string path = Server.MapPath("~/" + Retailerdetail.videokycpath);


                //   System.IO.DirectoryInfo di = new DirectoryInfo(path);

                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                    Retailerdetail.videokycpath = null;

                }


            }

            var res = db.SaveChanges();
            if (res > 0)
            {
                return Json(val, JsonRequestBehavior.AllowGet);
            }

            return Json("drgdhfjgh", JsonRequestBehavior.AllowGet);
            //RetailerDetalsModel viewmodel = new RetailerDetalsModel();
            //ViewBag.state = new SelectList(db.Select_State_Details(), "State_Id", "State_Name").ToList();
            //ViewBag.state1 = new SelectList(db.Select_State_Details(), "State_Id", "State_Name").ToList();
            //var district = from s in db.District_Desc
            //               where s.State_id == 0
            //               select s;
            //ViewBag.district = new SelectList(district, "Dist_id", "Dist_Desc").ToList();

            //viewmodel.select_retailer_details = db.Select_Retailer_Details_all("ADMIN", "ADMIN", "ADMIN").ToList();

            //return PartialView("_Retailerlist", viewmodel);


        }

        public PartialViewResult RetailerlistPart(string dealerid)
        {

            RetailerDetalsModel viewmodel = new RetailerDetalsModel();
            ViewBag.state = new SelectList(db.Select_State_Details(), "State_Id", "State_Name").ToList();
            ViewBag.state1 = new SelectList(db.Select_State_Details(), "State_Id", "State_Name").ToList();
            var district = from s in db.District_Desc
                           where s.State_id == 0
                           select s;
            var results = (from p in db.Slab_name where p.SlabFor == "Retailer" group p.SlabName by p.SlabName into g select new { SlabName = g.Key });

            ViewBag.slabname = new SelectList(results, "SlabName", "SlabName");
            ViewBag.district = new SelectList(district, "Dist_id", "Dist_Desc").ToList();

            if (!string.IsNullOrEmpty(dealerid))
            {
                viewmodel.select_retailer_details_paging = db.Select_Retailer_Details_all_paging(1, 50000, "ADMIN").Where(x => x.DealerId == dealerid).ToList();
            }
            else
            {
                viewmodel.select_retailer_details_paging = db.Select_Retailer_Details_all_paging(1, 500, "ADMIN").ToList();
            }
            viewmodel.Show_Service_namelist = db.Show_Service_name.ToList();
            viewmodel.Service_BlockUserwiseclslist = from tbl in db.Show_Service_name
                                                     join tbl1 in db.Service_BlockUserwise
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
            return PartialView("_Retailerlist", viewmodel);
        }


        public PartialViewResult RetailerSearchBy(string txtmob)
        {

            RetailerDetalsModel viewmodel = new RetailerDetalsModel();
            ViewBag.state = new SelectList(db.Select_State_Details(), "State_Id", "State_Name").ToList();
            ViewBag.state1 = new SelectList(db.Select_State_Details(), "State_Id", "State_Name").ToList();
            var district = from s in db.District_Desc
                           where s.State_id == 0
                           select s;
            var results = (from p in db.Slab_name where p.SlabFor == "Retailer" group p.SlabName by p.SlabName into g select new { SlabName = g.Key });

            ViewBag.slabname = new SelectList(results, "SlabName", "SlabName");
            ViewBag.district = new SelectList(district, "Dist_id", "Dist_Desc").ToList();

            if (!string.IsNullOrEmpty(txtmob))
            {
                try
                {
                    viewmodel.select_retailer_details_paging = db.Select_Retailer_Details_all_paging(1, 50000, "ADMIN").Where(x =>
                    x.Mobile.ToUpper().StartsWith(txtmob.Trim().ToUpper()) ||
                    x.RetailerName.ToUpper().Contains(txtmob.Trim().ToUpper()) ||
                    x.Email.ToUpper().Contains(txtmob.Trim().ToUpper()) ||
                    x.AadharCard.Contains(txtmob.Trim()) ||
                    x.PanCard.Contains(txtmob.Trim()) ||
                    x.Frm_Name.ToUpper().Contains(txtmob.Trim().ToUpper()) ||
                    x.DealerFarmName.ToUpper().Contains(txtmob.Trim().ToUpper())).ToList();
                }

                catch (Exception ex)
                {
                    string aaharwise = db.Retailer_Details.Where(a => a.AadharCard.Contains(txtmob.Trim())).Select(a => a.Mobile).FirstOrDefault();
                    if (aaharwise != null)
                    {
                        txtmob = aaharwise;
                    }
                    else
                    {
                        string pancardwise = db.Retailer_Details.Where(a => a.PanCard.Contains(txtmob.Trim())).Select(a => a.Mobile).FirstOrDefault();
                        if (pancardwise != null)
                        {
                            txtmob = pancardwise;
                        }
                    }

                    viewmodel.select_retailer_details_paging = db.Select_Retailer_Details_all_paging(1, 50000, "ADMIN").Where(x =>
                    x.Mobile.ToUpper().StartsWith(txtmob.Trim().ToUpper()) ||
                    x.RetailerName.ToUpper().Contains(txtmob.Trim().ToUpper()) ||
                    x.Email.ToUpper().Contains(txtmob.Trim().ToUpper()) ||
                    x.Frm_Name.ToUpper().Contains(txtmob.Trim().ToUpper()) ||
                    x.DealerFarmName.ToUpper().Contains(txtmob.Trim().ToUpper())).ToList();
                }

            }
            else
            {
                viewmodel.select_retailer_details_paging = db.Select_Retailer_Details_all_paging(1, 500, "ADMIN").ToList();
            }
            viewmodel.Show_Service_namelist = db.Show_Service_name.ToList();
            viewmodel.Service_BlockUserwiseclslist = from tbl in db.Show_Service_name
                                                     join tbl1 in db.Service_BlockUserwise
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
            return PartialView("_Retailerlist", viewmodel);
        }

        //\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\Api_user\\\\\\\\\\\\\\\\\\\\\\

        [PermissioncheckingAttribute(servicename = "USERS", permision = "Read")]
        public ActionResult Api_user()
        {
            var results = (from p in db.Slab_name where p.SlabFor == "API" group p.SlabName by p.SlabName into g select new { SlabName = g.Key });
            //var results = (from p in db.API_Slab where p.Slab_Name != "Default" group p.Slab_Name by p.Slab_Name into g select new { SlabName = g.Key });
            ViewBag.slabname = new SelectList(results, "SlabName", "SlabName");
            var sate = db.Select_State_Details().ToList();
            ViewBag.state = new SelectList(sate, "State_Id", "State_Name");
            ViewBag.state1 = new SelectList(sate, "State_Id", "State_Name");
            ViewBag.ch = db.API_all_uesers();
            APIModelcs vmodel = new APIModelcs();
            vmodel.ApiList = db.API_all_uesers();
            ViewData["ResendMail"] = TempData["ResendMAil"];
            return View(vmodel);
        }

        public ActionResult PDF_Api_user()
        {
            var results = (from p in db.Slab_name where p.SlabFor == "API" group p.SlabName by p.SlabName into g select new { SlabName = g.Key });
            ViewBag.slabname = new SelectList(results, "SlabName", "SlabName");
            var sate = db.Select_State_Details().ToList();
            ViewBag.state = new SelectList(sate, "State_Id", "State_Name");
            ViewBag.state1 = new SelectList(sate, "State_Id", "State_Name");
            ViewBag.ch = db.API_all_uesers();
            APIModelcs vmodel = new APIModelcs();
            vmodel.ApiList = db.API_all_uesers();
            ViewData["ResendMail"] = TempData["ResendMAil"];
            return new ViewAsPdf(vmodel);
        }

        public ActionResult Excel_Api_user()
        {
            var results = (from p in db.Slab_name where p.SlabFor == "API" group p.SlabName by p.SlabName into g select new { SlabName = g.Key });
            //var results = (from p in db.API_Slab where p.Slab_Name != "Default" group p.Slab_Name by p.Slab_Name into g select new { SlabName = g.Key });
            ViewBag.slabname = new SelectList(results, "SlabName", "SlabName");
            var sate = db.Select_State_Details().ToList();
            ViewBag.state = new SelectList(sate, "State_Id", "State_Name");
            ViewBag.state1 = new SelectList(sate, "State_Id", "State_Name");
            ViewBag.ch = db.API_all_uesers();
            APIModelcs vmodel = new APIModelcs();
            vmodel.ApiList = db.API_all_uesers();

            DataTable dataTbl = new DataTable();
            dataTbl.Columns.Add("User Firm", typeof(string));
            dataTbl.Columns.Add("User Name", typeof(string));
            dataTbl.Columns.Add("Mobile No", typeof(string));
            dataTbl.Columns.Add("PAN No", typeof(string));
            dataTbl.Columns.Add("KYC", typeof(string));
            dataTbl.Columns.Add("Email V/F", typeof(string));

            foreach (var item in vmodel.ApiList)
            {
                dataTbl.Rows.Add(item.farmname, item.username, item.mobile, item.pancard, item.PSAStatus == "Y" && item.AadhaarStatus == "Y" ? "Done" : "Due", item.EmailConfirmed == true ? "Done" : "Due");
            }
            var grid = new GridView();
            grid.DataSource = dataTbl;
            grid.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=Api_user.xls");
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

        public ActionResult emailverifyforapi(string apiid)
        {
            var usersts = db.Users.Where(aa => aa.UserId == apiid).SingleOrDefault();
            usersts.EmailConfirmed = true;
            db.SaveChanges();
            APIModelcs vmodel = new APIModelcs();
            vmodel.ApiList = db.API_all_uesers();
            return PartialView("_Api_user", vmodel);

        }


        [HttpPost]
        public ActionResult Approveddealersts(string val, string DealerId)
        {

            var dealerdetail = db.Dealer_Details.Where(aa => aa.DealerId == DealerId).SingleOrDefault();
            if (val == "pan")
            {
                dealerdetail.PSAStatus = "Y";
            }
            else if (val == "aadhar")
            {
                dealerdetail.AadhaarStatus = "Y";
            }
            else if (val == "gst")
            {
                dealerdetail.gststatus = "Y";
            }
            else if (val == "Aggrement")
            {
                dealerdetail.IsAgreement = true;
            }

            var res = db.SaveChanges();
            if (res > 0)
            {
                return Json(val, JsonRequestBehavior.AllowGet);
            }
            return Json("dfcdfdf", JsonRequestBehavior.AllowGet);
            //ViewBag.state = new SelectList(db.Select_State_Details(), "State_Id", "State_Name").ToList();
            //ViewBag.state1 = new SelectList(db.Select_State_Details(), "State_Id", "State_Name").ToList();
            //var district = from s in db.District_Desc
            //               where s.State_id == 0
            //               select s;
            //ViewBag.district = new SelectList(district, "Dist_id", "Dist_Desc").ToList();
            //// var Details = db.Select_Dealer_total("ADMIN").ToList();
            //DealerModel viewModel = new DealerModel();
            //viewModel.Select_dealer_list = db.Select_Dealer_total("ADMIN").ToList(); ;

            //return PartialView("_DistibutorList", viewModel);


        }

        public ActionResult Rejecteddealersts(string val, string DealerId)
        {
            var dealerdetail = db.Dealer_Details.Where(aa => aa.DealerId == DealerId).SingleOrDefault();
            if (val == "pan")
            {
                dealerdetail.PSAStatus = "N";
                dealerdetail.pancardPath = null;
            }
            else if (val == "aadhar")
            {
                dealerdetail.AadhaarStatus = "N";
                dealerdetail.aadharcardPath = null;
            }
            else if (val == "gst")
            {
                dealerdetail.gststatus = "N";
                dealerdetail.frimregistrationPath = null;
            }
            else if (val == "Aggrement")
            {
                dealerdetail.IsAgreement = false;

                dealerdetail.Agreementpath = null;
            }

            var res = db.SaveChanges();
            if (res > 0)
            {
                return Json(val, JsonRequestBehavior.AllowGet);
            }
            return Json("dfcdfdf", JsonRequestBehavior.AllowGet);
            //ViewBag.state = new SelectList(db.Select_State_Details(), "State_Id", "State_Name").ToList();
            //ViewBag.state1 = new SelectList(db.Select_State_Details(), "State_Id", "State_Name").ToList();
            //var district = from s in db.District_Desc
            //               where s.State_id == 0
            //               select s;
            //ViewBag.district = new SelectList(district, "Dist_id", "Dist_Desc").ToList();
            //// var Details = db.Select_Dealer_total("ADMIN").ToList();
            //DealerModel viewModel = new DealerModel();
            //viewModel.Select_dealer_list = db.Select_Dealer_total("ADMIN").ToList(); ;

            //return PartialView("_DistibutorList", viewModel);

        }





        [HttpPost]
        public ActionResult Edit_master_dealer(string ssid, string NameEdit, string FirmEdit, string pancardEdit, string AadhaarEdit, string gstEdit, string StateEdit, string DistrictEdit, string AddressEdit, string Editddlrole, string PinEdit)
        {
            try
            {
                var empuserid = User.Identity.GetUserId();
                var super = db.Superstokist_details.Where(p => p.SSId == ssid).SingleOrDefault();
                super.SuperstokistName = NameEdit;
                super.FarmName = FirmEdit;
                super.pancard = pancardEdit;
                super.adharcard = AadhaarEdit;
                super.gst = gstEdit;
                super.State = Convert.ToInt32(StateEdit);
                super.District = Convert.ToInt32(DistrictEdit);
                super.Address = AddressEdit;
                super.RolesType = Editddlrole;
                super.Pincode = Convert.ToInt32(PinEdit);
                db.SaveChanges();
                ViewBag.state = new SelectList(db.Select_State_Details(), "State_Id", "State_Name").ToList();
                ViewBag.state1 = new SelectList(db.Select_State_Details(), "State_Id", "State_Name").ToList();
                var district = from s in db.District_Desc
                               where s.State_id == 0
                               select s;
                ViewBag.district = new SelectList(district, "Dist_id", "Dist_Desc").ToList();
                MasterDistributerModel viewModel = new MasterDistributerModel();
                var ch = db.Select_super_Permissionwise_total(empuserid);
                viewModel.Select_super_Permissionwise = ch;
                return PartialView("_Masterlist", viewModel);

            }
            catch
            {
                return RedirectToAction("Master_list");
            }
        }


        [HttpGet]
        //public ActionResult DistibutorList()
        //{
        //    var results = (from p in db.Slab_name where p.SlabFor == "Distributor" group p.SlabName by p.SlabName into g select new { SlabName = g.Key });
        //    ViewBag.slabname = new SelectList(results, "SlabName", "SlabName");
        //    ViewBag.state = new SelectList(db.Select_State_Details(), "State_Id", "State_Name").ToList();
        //    ViewBag.state1 = new SelectList(db.Select_State_Details(), "State_Id", "State_Name").ToList();
        //    var district = from s in db.District_Desc
        //                   where s.State_id == 0
        //                   select s;
        //    ViewBag.district = new SelectList(district, "Dist_id", "Dist_Desc").ToList();
        //    var adminuserid = User.Identity.GetUserId();
        //    var Details = db.Select_Permissionwise_Dealer_total(adminuserid).ToList();
        //    DealerModel viewModel = new DealerModel();
        //    viewModel.Select_Permissionwise_Dealer_total = Details;
        //    var stands = db.Superstokist_details.ToList();
        //    IEnumerable<SelectListItem> selectList = from s in stands
        //                                             select new SelectListItem
        //                                             {
        //                                                 Value = s.SSId,
        //                                                 Text = s.Email + " -- " + s.FarmName.ToString()
        //                                             };
        //    ViewBag.masterid = new SelectList(selectList, "Value", "Text");
        //    TempData.Keep("Message");
        //    ViewData["ResendMail"] = TempData["ResendMAil"];
        //    return View(viewModel);
        //}



        public PartialViewResult MDTODealer(string tabtype, string usernm, string remreciverss, string txt_frm_date, string txt_to_date, string msg, string txtsearch)
        {
            EmployeeFundUserViewModel vmodel = new EmployeeFundUserViewModel();
            vmodel.master_to_dealer_report_BYAdmin_Result = null;
            vmodel.dealer_to_rem_fund_report = null;

            DateTime fromdate;
            DateTime Todate;
            if (string.IsNullOrEmpty(usernm))
            {
                usernm = "ALL";
            }



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



            if (tabtype == "Master")
            {
                vmodel.master_to_dealer_report_BYAdmin_Result = db.Master_to_dealer_report_BYAdmin(fromdate, Todate, usernm, "ALL", 1, 5000).ToList();
                if (!string.IsNullOrEmpty(txtsearch))
                {

                    vmodel.master_to_dealer_report_BYAdmin_Result = vmodel.master_to_dealer_report_BYAdmin_Result.Where(x => x.Head.ToLower().Contains(txtsearch) || x.masterfarmname.ToLower().Contains(txtsearch) || x.dealerFarmName.ToLower().Contains(txtsearch) || x.bal_type.ToLower().Contains(txtsearch) || x.Newbalance.ToString().Contains(txtsearch)).ToList();
                }

            }
            if (tabtype == "Dealer")
            {
                vmodel.dealer_to_rem_fund_report = db.dealer_to_rem_fund_report(fromdate, Todate, usernm, "ALL", 1, 5000).ToList();
                if (!string.IsNullOrEmpty(txtsearch))
                {

                    vmodel.dealer_to_rem_fund_report = vmodel.dealer_to_rem_fund_report.Where(x => x.Head.ToLower().Contains(txtsearch) || x.FarmName.ToLower().Contains(txtsearch) || x.Frm_Name.ToLower().Contains(txtsearch) || x.bal_type.ToLower().Contains(txtsearch) || x.totalbal.ToString().Contains(txtsearch)).ToList();
                }
            }
            if (tabtype == "Retailer")
            {
                if (string.IsNullOrEmpty(remreciverss) || remreciverss == "Transfer")
                {
                    vmodel.show_rem_to_rem_balByAdmin = db.show_rem_to_rem_balByAdmin(usernm, "ALL", fromdate, Todate, 1, 5000).ToList();
                    if (!string.IsNullOrEmpty(txtsearch))
                    {

                        vmodel.show_rem_to_rem_balByAdmin = vmodel.show_rem_to_rem_balByAdmin.Where(x => x.Retailerfrom.ToLower().Contains(txtsearch) || x.value.ToString().Contains(txtsearch)).ToList();
                    }
                }
                else if (remreciverss == "Recived")
                {
                    vmodel.show_rem_to_rem_balByAdmin_Recived = db.show_rem_to_rem_balByAdmin("ALL", usernm, fromdate, Todate, 1, 5000).ToList();
                    if (!string.IsNullOrEmpty(txtsearch))
                    {

                        vmodel.show_rem_to_rem_balByAdmin_Recived = vmodel.show_rem_to_rem_balByAdmin.Where(x => x.RetailerTOFrm.ToLower().Contains(txtsearch) || x.value.ToString().Contains(txtsearch)).ToList();
                    }
                }


            }



            return PartialView("_FundUserListPart", vmodel);

        }

        [PermissioncheckingAttribute(servicename = "FUNDUSERTOUSER", permision = "Write")]
        public JsonResult FundUserAdmin_MD_dealer_Generate_Unique_ID()
        {
            var adminuserid = User.Identity.GetUserId();

            string transferids = GenerateUniqueTransectionID();
            var transferid = "AMD" + transferids;



            TempData["transferadminmdtodlm"] = transferid;
            return Json(transferid, JsonRequestBehavior.AllowGet);
        }

        [PermissioncheckingAttribute(servicename = "FUNDUSERTOUSER", permision = "Write")]
        public ActionResult ChkSecurityPassmastertodlm_bal(string txtcode, string hdSuperstokistID, string hdMDDLM, string hdPaymentMode,
      string hdPaymentAmount, string hdMDDepositeSlipNo, string hdMDTransferType, string hdMDcollection, string hdMDComments,
      string hdMDBank, string hdsupraccno, string hdMDaccountno, string hdMDutrno, string hdMDwallet,
      string hdMDwalletno, string hdMDtransationno, string hdMDsettelment, string hdMDCreditDetail, string hdMDsubject)
        {
            try
            {
                var ch = "";
                var userid = User.Identity.GetUserId();
                var adminemail = db.Admin_details.Single().email;
                TempData.Keep("dlmid");
                TempData.Keep("bal");
                TempData.Keep("fundtype");
                TempData.Keep("comment");
                var password = Encrypt(txtcode);
                var tranpass = (from paa in db.admin_new_pass where paa.transpass == password select paa).Count();

                if (tranpass > 0)
                {




                    string superstockid = hdSuperstokistID;

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
                    string bankname = hdMDBank == null ? hdMDwallet : hdMDBank;
                    string adminacco = hdMDaccountno == null ? hdMDwalletno : hdMDaccountno;
                    adminacco = adminacco == null ? hdMDDepositeSlipNo : adminacco;
                    string DepositeSlipNo = hdMDDepositeSlipNo;
                    if (hdMDTransferType != null && hdPaymentMode == "Online Transfer")
                    {
                        type = hdPaymentMode + "/" + hdMDTransferType;
                    }
                    var Role = db.showrole(superstockid).SingleOrDefault();
                    System.Data.Entity.Core.Objects.ObjectParameter output = new
                    System.Data.Entity.Core.Objects.ObjectParameter("Output", typeof(string));

                    var tp = "";
                    Websitename = db.Admin_details.Single().WebsiteUrl;
                    decimal amount1 = Convert.ToDecimal(balance);
                    //   var ch = db.dealer_to_rem_fund_report(from, to, "ALL", "ALL").ToList();
                    if (Role.Name == "master")
                    {
                        string transferid = null;
                        try
                        {
                            transferid = TempData["transferadminmdtodlm"].ToString();
                        }
                        catch (Exception ex)
                        {
                            return Json("Please Try After SomeTime", JsonRequestBehavior.AllowGet);
                        }

                        var counts = db.FundTransfercount(hdSuperstokistID, hdMDDLM, type, amount1, transferid, "Admintodealer").SingleOrDefault().msg;
                        int msgcount = Convert.ToInt32(counts);
                        if (msgcount == 0)
                        {
                            msgcount = 60001;
                        }
                        int max_limit = 60000;
                        if (msgcount > max_limit)
                        {
                            string masteremain = db.Superstokist_details.FirstOrDefault(x => x.SSId == hdSuperstokistID).Email;
                            var dealeremail = db.Dealer_Details.FirstOrDefault(p => p.DealerId == hdMDDLM).Email;

                            var diff1 = (db.admin_to_dealer.Where(aa => aa.dealerid == hdMDDLM && aa.balance_from == hdSuperstokistID).OrderByDescending(aa => aa.idno).Select(c => c.cr).FirstOrDefault());
                            //(db.admin_to_super_balance.Where(aa => aa.SuperStokistID == DealerId).OrderByDescending(aa => aa.ID).Select(c => c.cr).FirstOrDefault());
                            diff1 = diff1 ?? 0;

                            decimal diff = Convert.ToDecimal(diff1);
                            //if (amount1 > 0)
                            //{
                            //if (type == "Credit" || type == "Cash")
                            //{
                            //    ch = "";//db.Insert_Admin_To_SuperStokist(DealerId, Convert.ToDecimal(amount1), 0, type, comment,collectionby,bankname,adminacco, output).Single().msg;
                            //}
                            //   System.Data.Entity.Core.Objects.ObjectParameter output = new System.Data.Entity.Core.Objects.ObjectParameter("output", typeof(String));
                            ch = db.master_to_delaer_byAdmin(hdSuperstokistID, hdMDDLM, amount1, 0, type, comment, collectionby, bankname, adminacco, "Direct", transferid, output).SingleOrDefault().msg;
                            //  ch = db.Insert_Admin_To_SuperStokist(DealerId, Convert.ToDecimal(amount1), type, comment, collectionby, bankname, adminacco, "Direct", output).Single().msg;


                            if (ch == "Balance Transfer SuccessFully.")
                            {
                                diff1 = (db.admin_to_dealer.Where(aa => aa.dealerid == hdMDDLM && aa.balance_from == hdSuperstokistID).OrderByDescending(aa => aa.idno).Select(c => c.cr).FirstOrDefault());
                                // var remainmaster = db.Remain_dealer_balance.Where(p => p.DealerID == DealerId).Single().Remainamount;
                                // db.Remain_superstokist_balance.Where(p => p.SuperStokistID == DealerId).Single().Remainamount;
                                var remaindealer = db.Remain_dealer_balance.Where(p => p.DealerID == hdMDDLM).Single().Remainamount;
                                var statusMaster = db.PushNotificationStatus.Where(a => a.UserRole == "Master").SingleOrDefault().Status;
                                var statusDealer = db.PushNotificationStatus.Where(a => a.UserRole == "Distributor").SingleOrDefault().Status;
                                var DealerDetails = db.Dealer_Details.Where(p => p.DealerId == hdMDDLM).Single();
                                var statusSendSmsMasterFundTransfer = db.SMSSendAlls.Where(a => a.ServiceName == "MDToDLMdlmFundTrans").SingleOrDefault();
                                var statusSendMailMasterFundTransfer = db.EmailSendAlls.Where(a => a.ServiceName == "MDToDLMdlmFundTrans1").SingleOrDefault().Status;

                                var MasterDetails = db.Superstokist_details.Where(a => a.SSId == superstockid).Single();
                                var mastername = db.Superstokist_details.Where(p => p.SSId == superstockid).Single().SuperstokistName;
                                var Adminname = db.Admin_details.Single().Name;
                                var RemainAdmin = db.Remain_Admin_balance.Where(p => p.admin == userid).Single().RemainAmount;
                                var statusSendSmsMasterToDlmFundTransferMaster = db.SMSSendAlls.Where(a => a.ServiceName == "MDToDLMMDFundTrans").SingleOrDefault();
                                var statusSendMailMasterToDlmFundTransferMaster = db.EmailSendAlls.Where(a => a.ServiceName == "MDToDLMMDFundTrans1").SingleOrDefault().Status;

                                if (type == "Credit")
                                {
                                    //if (statusSendSmsMasterFundTransfer == "Y")
                                    //{
                                    //    try
                                    //    {
                                    //        string msgssss = "";
                                    //        string tempid = "";
                                    //        string urlss = "";

                                    //        var smsapionsts = db.apisms.Where(x => x.sts == "Y").SingleOrDefault();
                                    //        var smsstypes = db.Sending_SMS_Templates.Where(x => x.SMS_TYPE == "FUNDTRANSFERMESSAGE_CREDIT" && x.SMSAPIID == smsapionsts.id).SingleOrDefault();
                                    //        if (smsstypes != null)
                                    //        {
                                    //            msgssss = string.Format(smsstypes.Templates, amount1, remaindealer, diff1);
                                    //            tempid = smsstypes.Templateid;
                                    //            urlss = smsapionsts.smsapi;

                                    //            smssend.sendsmsallnew(DealerDetails.Mobile, msgssss, urlss, tempid);
                                    //        }
                                    //    }
                                    //    catch (Exception) { }

                                    //    //  smssend.sendsmsall(DealerDetails.Mobile, "Credit Received Rs." + amount1 + ".New Balance is " + remaindealer + ".Your O/s Credit is " + diff1 + "", "Fund Transfer");
                                    //}

                                    smssend.sms_init(statusSendSmsMasterFundTransfer.Status, statusSendSmsMasterFundTransfer.Whatsapp_Status, "FUNDTRANSFERMESSAGE_CREDIT", DealerDetails.Mobile, amount1, remaindealer, diff1);

                                    //if (statusSendSmsMasterToDlmFundTransferMaster == "Y")
                                    //{
                                    //    string msgssss = "";
                                    //    string tempid = "";
                                    //    string urlss = "";

                                    //    var smsapionsts = db.apisms.Where(x => x.sts == "Y").SingleOrDefault();
                                    //    var smsstypes = db.Sending_SMS_Templates.Where(x => x.SMS_TYPE == "SEND_SMS_CREDIT_TRANSFERREDTO" && x.SMSAPIID == smsapionsts.id).SingleOrDefault();
                                    //    if (smsstypes != null)
                                    //    {
                                    //        msgssss = string.Format(smsstypes.Templates, dealeremail, amount1, diff1);
                                    //        tempid = smsstypes.Templateid;
                                    //        urlss = smsapionsts.smsapi;

                                    //        smssend.sendsmsallnew(MasterDetails.Mobile, msgssss, urlss, tempid);
                                    //    }
                                    //    //  smssend.sendsmsall(MasterDetails.Mobile, "Credit Transferred To " + dealeremail + " Rs. " + amount1 + ".Total Credit(O/s) Balance is " + diff1 + "", "Fund Transfer");
                                    //}

                                    smssend.sms_init(statusSendSmsMasterToDlmFundTransferMaster.Status, statusSendSmsMasterToDlmFundTransferMaster.Whatsapp_Status, "SEND_SMS_CREDIT_TRANSFERREDTO", MasterDetails.Mobile, dealeremail, amount1, diff1);

                                    if (statusSendMailMasterFundTransfer == "Y")
                                    {
                                        smssend.SendEmailAll(DealerDetails.Email, "Credit Received Rs." + amount1 + ".New Balance is " + remaindealer + ".Your O/s Credit is " + diff1 + "", "Fund Transfer", adminemail, 1000);
                                    }
                                    if (statusSendMailMasterToDlmFundTransferMaster == "Y")
                                    {
                                        smssend.SendEmailAll(MasterDetails.Email, "Credit Transferred To " + dealeremail + " Rs. " + amount1 + ".Total Credit(O/s) Balance is " + diff1 + "", "Fund Transfer", adminemail, 1000);
                                    }

                                    //if (statusSendSmsMasterToDlmFundTransferMaster == "Y")
                                    //{
                                    //    SendPushNotification(adminemail, Url.Action("Admin_to_master_Dealer", "Home"), "Credit Trnf Rs. " + amount1 + "  to " + mastername + ", Pending Credit is " + diff1 + " . Bal is " + RemainAdmin + " . (" + string.Format("{0:dd-MMM hh:mm:ss}", DateTime.Now) + ")", "Fund Transfer ..");
                                    //}
                                    //if (statusMaster == "Y")
                                    //{
                                    //    SendPushNotification(dealeremail, Websitename + "/Master/Home/ReceiveFund", "Credit Recd frm " + Adminname + " Rs." + amount1 + ". Your pending credit is " + diff1 + " . New Balance is " + remainmaster + ". (" + string.Format("{0:dd-MMM hh:mm:ss}", DateTime.Now) + ")", "Fund Transfer ..");
                                    //}
                                    //  notify.sendmessage(dealeremail, "Credit Recd frm " + Adminname + " Rs." + amount1 + ". Your pending credit is " + diff1 + " . New Balance is " + remainmaster + ". (" + string.Format("{0:dd-MMM hh:mm:ss}", DateTime.Now) + ")");
                                    notify.sendmessage(dealeremail, "Credit Received Rs." + amount1 + ".New Balance is " + remaindealer + ".Your O/s Credit is " + diff1 + "");
                                    ch = "Fund Transfer Successful.";
                                    tp = "success";
                                    ch = "Fund Transfer Successful.";
                                    tp = "success";
                                }
                                else
                                {
                                    //  var mastername = db.Superstokist_details.Where(p => p.SSId == masterid).Single().SuperstokistName;
                                    var dealername = db.Dealer_Details.Where(p => p.DealerId == hdMDDLM).Single().DealerName;
                                    //if (statusSendSmsMasterFundTransfer == "Y")
                                    //{

                                    //    string msgssss = "";
                                    //    string tempid = "";
                                    //    string urlss = "";

                                    //    var smsapionsts = db.apisms.Where(x => x.sts == "Y").SingleOrDefault();
                                    //    var smsstypes = db.Sending_SMS_Templates.Where(x => x.SMS_TYPE == "FUNDTRANSFERMESSAGE_CASH" && x.SMSAPIID == smsapionsts.id).SingleOrDefault();
                                    //    if (smsstypes != null)
                                    //    {
                                    //        msgssss = string.Format(smsstypes.Templates, amount1, mastername, remaindealer, diff1);
                                    //        tempid = smsstypes.Templateid;
                                    //        urlss = smsapionsts.smsapi;

                                    //        smssend.sendsmsallnew(DealerDetails.Mobile, msgssss, urlss, tempid);
                                    //    }
                                    //    //  smssend.sendsmsall(DealerDetails.Mobile, "Cash Paid Rs." + amount1 + " to " + mastername + ". New Balance is " + remaindealer + ". Your O/s Credit is " + diff1 + "", "Fund Transfer");
                                    //}

                                    smssend.sms_init(statusSendSmsMasterFundTransfer.Status, statusSendSmsMasterFundTransfer.Whatsapp_Status, "FUNDTRANSFERMESSAGE_CASH", DealerDetails.Mobile, amount1, mastername, remaindealer, diff1);

                                    //if (statusSendSmsMasterToDlmFundTransferMaster == "Y")
                                    //{
                                    //    string msgssss = "";
                                    //    string tempid = "";
                                    //    string urlss = "";

                                    //    var smsapionsts = db.apisms.Where(x => x.sts == "Y").SingleOrDefault();
                                    //    var smsstypes = db.Sending_SMS_Templates.Where(x => x.SMS_TYPE == "SENDSMSMASTERTODLMFUNDTRANSFERMSGCASH" && x.SMSAPIID == smsapionsts.id).SingleOrDefault();
                                    //    if (smsstypes != null)
                                    //    {
                                    //        msgssss = string.Format(smsstypes.Templates, amount1, dealername, diff1);
                                    //        tempid = smsstypes.Templateid;
                                    //        urlss = smsapionsts.smsapi;

                                    //        smssend.sendsmsallnew(MasterDetails.Mobile, msgssss, urlss, tempid);
                                    //    }
                                    //    //  smssend.sendsmsall(MasterDetails.Mobile, "Cash Recived Rs." + amount1 + " From " + dealername + ",his O/s Credit is " + diff1 + "", "Fund Transfer");
                                    //}

                                    smssend.sms_init(statusSendSmsMasterToDlmFundTransferMaster.Status, statusSendSmsMasterToDlmFundTransferMaster.Whatsapp_Status, "SENDSMSMASTERTODLMFUNDTRANSFERMSGCASH", MasterDetails.Mobile, amount1, dealername, diff1);

                                    if (statusSendMailMasterFundTransfer == "Y")
                                    {
                                        smssend.SendEmailAll(DealerDetails.Email, "Cash Paid Rs." + amount1 + " to " + mastername + ". New Balance is " + remaindealer + ". Your O/s Credit is " + diff1 + "", "Fund Transfer", adminemail, 1000);
                                    }
                                    if (statusSendMailMasterToDlmFundTransferMaster == "Y")
                                    {
                                        smssend.SendEmailAll(MasterDetails.Email, "Cash Recived Rs." + amount1 + " From " + dealername + ",his O/s Credit is " + diff1 + "", "Fund Transfer", adminemail, 1000);
                                    }
                                    //if (statusMaster == "Y")
                                    //{
                                    //    SendPushNotification(email, Url.Action("SendFund", "Home"), "Cash Recived Rs." + amount + " From " + dealername + ",his O/s Credit is " + diff2 + "", "Fund Transfer ..");
                                    //}
                                    //if (statusDealer == "Y")
                                    //{
                                    //    SendPushNotification(useremail, Websitename + "/DEALER/Home/ReceiveFund", "Cash Paid Rs." + amount + " to " + mastername + ". New Balance is " + remaindealer + ". Your O/s Credit is " + diff2 + "", "Fund Transfer ..");
                                    //}
                                    notify.sendmessage(dealeremail, "Cash Paid Rs." + amount1 + " to " + mastername + ". New Balance is " + remaindealer + ". Your O/s Credit is " + diff1 + "");

                                }
                            }
                            //}
                            //else
                            //{
                            //    ch = "Amount should be not zero";
                            //    tp = "error";
                            //}

                            TempData["BalanceTransferMsg"] = ch;
                            TempData["BalanceTransfertype"] = tp;
                            TempData.Remove("dlmid");
                            TempData.Remove("bal");
                            TempData.Remove("dl_commission");
                            TempData.Remove("comment");
                            return Json(ch, JsonRequestBehavior.AllowGet);
                            //  return RedirectToAction("Admin_to_master_Dealer");
                        }
                        else
                        {
                            return Json("Please Try After SomeTime", JsonRequestBehavior.AllowGet);
                        }

                    }
                    else
                    {
                        string transferid = null;
                        try
                        {
                            transferid = TempData["transferadmindlmtorem"].ToString();
                        }
                        catch (Exception ex)
                        {
                            return Json("Please Try After SomeTime", JsonRequestBehavior.AllowGet);
                        }

                        var counts = db.FundTransfercount(hdSuperstokistID, hdMDDLM, type, amount1, transferid, "Admintoretailer").SingleOrDefault().msg;
                        int msgcount = Convert.ToInt32(counts);
                        if (msgcount == 0)
                        {
                            msgcount = 60001;
                        }
                        int max_limit = 60000;
                        if (msgcount > max_limit)
                        {
                            decimal amount = Convert.ToDecimal(balance);
                            var dealeremail = db.Dealer_Details.FirstOrDefault(p => p.DealerId == hdSuperstokistID).Email;

                            var diff1 = (db.Dealer_To_Retailer_Balance.Where(aa => aa.RetailerId == hdMDDLM && aa.DealerId == hdSuperstokistID).OrderByDescending(aa => aa.id).Select(aa => aa.cr).FirstOrDefault());
                            diff1 = diff1 ?? 0;

                            decimal diff = Convert.ToDecimal(diff1);
                            //if (amount1 > 0)
                            //{
                            //if (type == "Credit" || type == "Cash")
                            //{
                            //    ch = "";//db.Insert_Admin_To_SuperStokist(DealerId, Convert.ToDecimal(amount1), 0, type, comment,collectionby,bankname,adminacco, output).Single().msg;
                            //}
                            //   System.Data.Entity.Core.Objects.ObjectParameter output = new System.Data.Entity.Core.Objects.ObjectParameter("output", typeof(String));
                            ch = db.fund_dealer_to_retailer_byAdmin(hdSuperstokistID, hdMDDLM, amount1, type, comment, collectionby, bankname, adminacco, "Direct", transferid, output).SingleOrDefault().msg;
                            //  ch = db.Insert_Admin_To_SuperStokist(DealerId, Convert.ToDecimal(amount1), type, comment, collectionby, bankname, adminacco, "Direct", output).Single().msg;

                            if (ch == "Balance Transfer Successfully.")
                            {
                                diff1 = (db.Dealer_To_Retailer_Balance.Where(aa => aa.RetailerId == hdMDDLM && aa.DealerId == hdSuperstokistID).OrderByDescending(aa => aa.id).Select(aa => aa.cr).FirstOrDefault());
                                diff1 = diff1 ?? 0;
                                var remainretailer = db.Remain_reteller_balance.Where(p => p.RetellerId == hdMDDLM).Single().Remainamount;
                                var statusDealer = db.PushNotificationStatus.Where(a => a.UserRole == "Distributor").SingleOrDefault().Status;
                                var statusRetailer = db.PushNotificationStatus.Where(a => a.UserRole == "Retailer").SingleOrDefault().Status;

                                var statusSendSmsDlmToRetailerFundTransfer = db.SMSSendAlls.Where(a => a.ServiceName == "DLMtoRemRemFundTrans").SingleOrDefault();
                                var statusSendMailDlmToRetailerFundTransfer = db.EmailSendAlls.Where(a => a.ServiceName == "DLMtoRemRemFundTrans1").SingleOrDefault().Status;
                                var RetailerMobile = db.Retailer_Details.Where(p => p.RetailerId == hdMDDLM).Single().Mobile;
                                var statusSendSmsDlmToRetailerFundTransferDlm = db.SMSSendAlls.Where(a => a.ServiceName == "DLMtoRemdlmFundTrans").SingleOrDefault();
                                var statusSendMailDlmToRetailerFundTransferDlm = db.EmailSendAlls.Where(a => a.ServiceName == "DLMtoRemdlmFundTrans1").SingleOrDefault().Status;
                                var DealerMobile = db.Dealer_Details.Where(p => p.DealerId == hdSuperstokistID).Single().Mobile;
                                var DealerEmail = db.Dealer_Details.Where(p => p.DealerId == hdSuperstokistID).Single().Email;
                                var RetailerEmail = db.Dealer_Details.Where(p => p.DealerId == hdSuperstokistID).Single().Email;
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



                                    //            msgssss = string.Format(smsstypes.Templates, DealerEmail, balance, remainretailer, diff1);
                                    //            tempid = smsstypes.Templateid;
                                    //            urlss = smsapionsts.smsapi;

                                    //            smssend.sendsmsallnew(RetailerMobile, msgssss, urlss, tempid);
                                    //        }
                                    //    }
                                    //    catch (Exception) { }

                                    //    //  smssend.sendsmsall(RetailerMobile, "Credit Received by " + DealerEmail + " Rs." + balance + ".New Balance is " + remainretailer + ".Your O/s Credit is " + diff1 + "", "Fund Transfer");
                                    //}

                                    smssend.sms_init(statusSendSmsDlmToRetailerFundTransfer.Status, statusSendSmsDlmToRetailerFundTransfer.Whatsapp_Status, "SEND_SMS_CREDIT_RECEIVEDBY", RetailerMobile, DealerEmail, balance, remainretailer, diff1);

                                    //if (statusSendSmsDlmToRetailerFundTransferDlm == "Y")
                                    //{
                                    //    string msgssss = "";
                                    //    string tempid = "";
                                    //    string urlss = "";

                                    //    var smsapionsts = db.apisms.Where(x => x.sts == "Y").SingleOrDefault();
                                    //    var smsstypes = db.Sending_SMS_Templates.Where(x => x.SMS_TYPE == "SEND_SMS_CREDIT_TRANSFERREDTO" && x.SMSAPIID == smsapionsts.id).SingleOrDefault();
                                    //    if (smsstypes != null)
                                    //    {
                                    //        msgssss = string.Format(smsstypes.Templates, RetailerEmail, balance, diff1);
                                    //        tempid = smsstypes.Templateid;
                                    //        urlss = smsapionsts.smsapi;

                                    //        smssend.sendsmsallnew(DealerMobile, msgssss, urlss, tempid);
                                    //    }
                                    //    //  smssend.sendsmsall(DealerMobile, "Credit Transferred To " + RetailerEmail + " Rs. " + balance + ".Total Credit(O/s) Balance is " + diff1 + "", "Fund Transfer");
                                    //}

                                    smssend.sms_init(statusSendSmsDlmToRetailerFundTransferDlm.Status, statusSendSmsDlmToRetailerFundTransferDlm.Whatsapp_Status, "SEND_SMS_CREDIT_TRANSFERREDTO", DealerMobile, RetailerEmail, balance, diff1);


                                    if (statusSendMailDlmToRetailerFundTransfer == "Y")
                                    {
                                        smssend.SendEmailAll(RetailerEmail, "Credit Received by " + DealerEmail + " Rs." + balance + ".New Balance is " + remainretailer + ".Your O/s Credit is " + diff1 + "", "Fund Transfer", adminemail, 1000);
                                    }
                                    if (statusSendMailDlmToRetailerFundTransferDlm == "Y")
                                    {
                                        smssend.SendEmailAll(DealerEmail, "Credit Transferred To " + RetailerEmail + " Rs. " + balance + ".Total Credit(O/s) Balance is " + diff1 + "", "Fund Transfer", adminemail, 1000);
                                    }

                                    notify.sendmessage(RetailerEmail, "Credit Received Rs." + balance + ".New Balance is " + remainretailer + ".Your O/s Credit is " + diff1 + "");
                                    TempData["result"] = ch;
                                }
                                else
                                {
                                    var retailername = db.Retailer_Details.Where(p => p.RetailerId == hdMDDLM).Single().RetailerName;
                                    var dealername = db.Dealer_Details.Where(p => p.DealerId == hdSuperstokistID).Single().DealerName;
                                    //if (statusSendSmsDlmToRetailerFundTransfer == "Y")
                                    //{
                                    //    string msgssss = "";
                                    //    string tempid = "";
                                    //    string urlss = "";

                                    //    var smsapionsts = db.apisms.Where(x => x.sts == "Y").SingleOrDefault();
                                    //    var smsstypes = db.Sending_SMS_Templates.Where(x => x.SMS_TYPE == "FUNDTRANSFERMESSAGE_CASH" && x.SMSAPIID == smsapionsts.id).SingleOrDefault();
                                    //    if (smsstypes != null)
                                    //    {
                                    //        msgssss = string.Format(smsstypes.Templates, balance, dealername, remainretailer, diff1);
                                    //        tempid = smsstypes.Templateid;
                                    //        urlss = smsapionsts.smsapi;

                                    //        smssend.sendsmsallnew(RetailerMobile, msgssss, urlss, tempid);
                                    //    }

                                    //    //  smssend.sendsmsall(RetailerMobile, "Cash Paid Rs." + balance + " to " + dealername + ". New Balance is " + remainretailer + ". Your O/s Credit is " + diff1 + "", "Fund Transfer");
                                    //}

                                    smssend.sms_init(statusSendSmsDlmToRetailerFundTransfer.Status, statusSendSmsDlmToRetailerFundTransfer.Whatsapp_Status, "FUNDTRANSFERMESSAGE_CASH", RetailerMobile, balance, dealername, remainretailer, diff1);

                                    //if (statusSendSmsDlmToRetailerFundTransferDlm == "Y")
                                    //{
                                    //    string msgssss = "";
                                    //    string tempid = "";
                                    //    string urlss = "";

                                    //    var smsapionsts = db.apisms.Where(x => x.sts == "Y").SingleOrDefault();
                                    //    var smsstypes = db.Sending_SMS_Templates.Where(x => x.SMS_TYPE == "SENDSMSMASTERTODLMFUNDTRANSFERMSGCASH" && x.SMSAPIID == smsapionsts.id).SingleOrDefault();
                                    //    if (smsstypes != null)
                                    //    {
                                    //        msgssss = string.Format(smsstypes.Templates, balance, retailername, diff1);
                                    //        tempid = smsstypes.Templateid;
                                    //        urlss = smsapionsts.smsapi;

                                    //        smssend.sendsmsallnew(DealerMobile, msgssss, urlss, tempid);
                                    //    }
                                    //    // smssend.sendsmsall(DealerMobile, "Cash Recived Rs." + balance + " From " + retailername + ",his O/s Credit is " + diff1 + "", "Fund Transfer");
                                    //}

                                    smssend.sms_init(statusSendSmsDlmToRetailerFundTransferDlm.Status, statusSendSmsDlmToRetailerFundTransferDlm.Whatsapp_Status, "SENDSMSMASTERTODLMFUNDTRANSFERMSGCASH", DealerMobile, balance, retailername, diff1);


                                    if (statusSendMailDlmToRetailerFundTransfer == "Y")
                                    {
                                        smssend.SendEmailAll(RetailerEmail, "Cash Paid Rs." + balance + " to " + dealername + ". New Balance is " + remainretailer + ". Your O/s Credit is " + diff1 + "", "Fund Transfer", adminemail, 1000);
                                    }
                                    if (statusSendMailDlmToRetailerFundTransferDlm == "Y")
                                    {
                                        smssend.SendEmailAll(DealerEmail, "Cash Recived Rs." + balance + " From " + retailername + ",his O/s Credit is " + diff1 + "", "Fund Transfer", adminemail, 1000);
                                    }

                                    notify.sendmessage(RetailerEmail, "Cash Paid Rs." + balance + " to " + dealername + ". New Balance is " + remainretailer + ". Your O/s Credit is " + diff1 + "");
                                    TempData["result"] = ch;
                                }
                            }

                            return Json(ch, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json("Your Previous Request IN Process Please Try After SomeTime", JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                else
                {
                    ViewData["MSG"] = "Transaction Password is Wrong.";
                    ch = "Transaction Password is Wrong.";
                    //  return View();
                    return Json(ch, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }

            //return Json(ch, JsonRequestBehavior.AllowGet);
        }

        [PermissioncheckingAttribute(servicename = "FUNDUSERTOUSER", permision = "Write")]
        public JsonResult Fundtransfer_AdminRem_to_Rem_Generate_Unique_ID()
        {
            var adminuserid = User.Identity.GetUserId();

            string transferids = GenerateUniqueTransectionID();
            var transferid = "ARR" + transferids;



            TempData["transferadminremtorem"] = transferid;
            return Json(transferid, JsonRequestBehavior.AllowGet);
        }

        [PermissioncheckingAttribute(servicename = "FUNDUSERTOUSER", permision = "Write")]
        public ActionResult ChkSecurityPassRetailertoRetailer_bal(string txtcode, string hdSuperstokistID, string hdMDDLM, string hdMDComments, string hdPaymentAmount)
        {
            try
            {
                var ch = "";

                var AdminMail = db.Admin_details.SingleOrDefault().email;

                var userid = db.Retailer_Details.FirstOrDefault(a => a.RetailerId == hdSuperstokistID).RetailerId;
                string useremail = db.Retailer_Details.Where(p => p.RetailerId == hdSuperstokistID).FirstOrDefault().Email;
                //  string reatilerto = "";
                // var chkmb = db.Retailer_Details.Where(a => a.Mobile == FromRetailerId || a.Email == FromRetailerId).Any();
                //if (chkmb == false)
                //{
                //    TempData["error"] = "This User Not Exist";
                //    return RedirectToAction("Retailer_to_retailer");
                //}
                var reatilerto = db.Retailer_Details.Where(p => p.RetailerId == hdMDDLM).FirstOrDefault().RetailerId;
                var retaileremaillid = db.Retailer_Details.Where(p => p.RetailerId == hdMDDLM).FirstOrDefault().Email;
                //if (remid.Count > 0)
                //{
                //    reatilerto = remid.Single().RetailerId;
                //    retaileremaillid = remid.Single().Email;
                //}
                //else
                //{
                //    reatilerto = db.Retailer_Details.Where(a => a.Email == RetailerId).SingleOrDefault().RetailerId;
                //    retaileremaillid = RetailerId;
                //}
                var password = Encrypt(txtcode);

                var tranpass = (from paa in db.admin_new_pass where paa.transpass == password select paa).Count();

                if (tranpass > 0)
                {
                    string transferid = null;
                    try
                    {
                        transferid = TempData["transferadminremtorem"].ToString();
                    }
                    catch (Exception ex)
                    {
                        return Json("Please Try After SomeTime", JsonRequestBehavior.AllowGet);
                    }
                    var counts = db.FundTransfercount(userid, reatilerto, "", Convert.ToDecimal(hdPaymentAmount), transferid, "Remtorem").SingleOrDefault().msg;
                    int msgcount = Convert.ToInt32(counts);
                    if (msgcount == 0)
                    {
                        msgcount = 60001;
                    }
                    int max_limit = 60000;
                    if (msgcount > max_limit)
                    {
                        System.Data.Entity.Core.Objects.ObjectParameter output = new
                System.Data.Entity.Core.Objects.ObjectParameter("output", typeof(string));
                        ch = db.rem_to_rem_bal_transfer_ByAdmin(userid, reatilerto, Convert.ToDecimal(hdPaymentAmount), hdMDComments, transferid, output).SingleOrDefault().msg.ToString();
                        var remainbal = db.Remain_reteller_balance.Where(r => r.RetellerId == userid).Single().Remainamount;
                        var remainbalretailer = db.Remain_reteller_balance.Where(r => r.RetellerId == reatilerto).Single().Remainamount;
                        var statusRetailer = db.PushNotificationStatus.Where(a => a.UserRole == "Admin").SingleOrDefault().Status;

                        var statusSendSmsREMToREMFundTransfer = db.SMSSendAlls.Where(a => a.ServiceName == "RemtoRemRemFundTrans").SingleOrDefault();
                        var statusSendMailREMToREMFundTransfer = db.EmailSendAlls.Where(a => a.ServiceName == "RemtoRemRemFundTrans1").SingleOrDefault().Status;

                        var RetailerDetails = db.Retailer_Details.Where(p => p.RetailerId == userid).Single();
                        var statusSendSmsREMToREMFundTransferRetailer = db.SMSSendAlls.Where(a => a.ServiceName == "RemtoRem1RemFundTrans").SingleOrDefault();
                        var statusSendMailREMToREMFundTransferRetailer = db.EmailSendAlls.Where(a => a.ServiceName == "RemtoRem1RemFundTrans1").SingleOrDefault().Status;

                        var RetailerMobileRetailerDetails = db.Retailer_Details.Where(p => p.RetailerId == reatilerto).Single();

                        if (ch == "Balance Transfer SuccessFully.")
                        {
                            //if (statusSendSmsREMToREMFundTransferRetailer == "Y")
                            //{
                            //    string msgssss = "";
                            //    string tempid = "";
                            //    string urlss = "";

                            //    var smsapionsts = db.apisms.Where(x => x.sts == "Y").SingleOrDefault();
                            //    var smsstypes = db.Sending_SMS_Templates.Where(x => x.SMS_TYPE == "FUNDTRANSFERBALANCETRANSFER" +
                            //    "" && x.SMSAPIID == smsapionsts.id).SingleOrDefault();
                            //    if (smsstypes != null)
                            //    {
                            //        msgssss = string.Format(smsstypes.Templates, hdPaymentAmount, useremail, remainbalretailer);
                            //        tempid = smsstypes.Templateid;
                            //        urlss = smsapionsts.smsapi;

                            //        smssend.sendsmsallnew(RetailerMobileRetailerDetails.Mobile, msgssss, urlss, tempid);
                            //    }
                            //    //  smssend.sendsmsall(RetailerMobileRetailerDetails.Mobile, "Balance " + Convert.ToDecimal(hdPaymentAmount) + " Transfer to " + useremail + " is Successfully Transfer. Remain Balance is " + remainbalretailer + "", "Fund Transfer");
                            //}

                            smssend.sms_init(statusSendSmsREMToREMFundTransferRetailer.Status, statusSendSmsREMToREMFundTransferRetailer.Whatsapp_Status, "FUNDTRANSFERBALANCETRANSFER", RetailerMobileRetailerDetails.Mobile, hdPaymentAmount, useremail, remainbalretailer);

                            //if (statusSendSmsREMToREMFundTransfer == "Y")
                            //{
                            //    string msgssss = "";
                            //    string tempid = "";
                            //    string urlss = "";

                            //    var smsapionsts = db.apisms.Where(x => x.sts == "Y").SingleOrDefault();
                            //    var smsstypes = db.Sending_SMS_Templates.Where(x => x.SMS_TYPE == "RETAILERTORETAILERTOFUNDTRANSFER" && x.SMSAPIID == smsapionsts.id).SingleOrDefault();
                            //    if (smsstypes != null)
                            //    {
                            //        msgssss = string.Format(smsstypes.Templates, retaileremaillid, hdPaymentAmount, remainbal);
                            //        tempid = smsstypes.Templateid;
                            //        urlss = smsapionsts.smsapi;

                            //        smssend.sendsmsallnew(RetailerMobileRetailerDetails.Mobile, msgssss, urlss, tempid);
                            //    }
                            //    //   smssend.sendsmsall(RetailerDetails.Mobile, "Balance Transfer Successfully To Retailer Id is " + retaileremaillid + " and Balance is " + Convert.ToDecimal(hdPaymentAmount) + ", Remain Balance is " + remainbal + "", "Fund Transfer");
                            //}

                            smssend.sms_init(statusSendSmsREMToREMFundTransfer.Status, statusSendSmsREMToREMFundTransfer.Whatsapp_Status, "RETAILERTORETAILERTOFUNDTRANSFER", RetailerMobileRetailerDetails.Mobile, retaileremaillid, hdPaymentAmount, remainbal);

                            if (statusSendMailREMToREMFundTransferRetailer == "Y")
                            {
                                smssend.SendEmailAll(RetailerMobileRetailerDetails.Email, "Balance " + Convert.ToDecimal(hdPaymentAmount) + " Transfer to " + useremail + " is Successfully Transfer. Remain Balance is " + remainbalretailer + "", "Fund Transfer", AdminMail, 1000);
                            }
                            if (statusSendMailREMToREMFundTransfer == "Y")
                            {
                                smssend.SendEmailAll(RetailerDetails.Email, "Balance Transfer Successfully To Retailer Id is " + retaileremaillid + " and Balance is " + Convert.ToDecimal(hdPaymentAmount) + ", Remain Balance is " + remainbal + "", "Fund Transfer", AdminMail, 1000);
                            }

                            TempData["success"] = ch;
                            return Json(ch, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {

                            TempData["error"] = ch;
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
                    ViewData["MSG"] = "Transaction Password is Wrong.";
                    ch = "Transaction Password is Wrong.";
                    //  return View();
                    return Json(ch, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [PermissioncheckingAttribute(servicename = "FUNDUSERTOUSER", permision = "Write")]
        public JsonResult Fundtransfer_AdminDealer_to_Rem_Generate_Unique_ID()
        {
            var adminuserid = User.Identity.GetUserId();

            string transferids = GenerateUniqueTransectionID();
            var transferid = "ADR" + transferids;



            TempData["transferadmindlmtorem"] = transferid;
            return Json(transferid, JsonRequestBehavior.AllowGet);
        }



        [PermissioncheckingAttribute(servicename = "FUNDUSERTOUSER", permision = "Read")]
        public ActionResult GenerateExcellFileFundUser_to_User(string tabID, DateTime frm_date, DateTime to_date, string usernm, string remreciverss)
        {

            List<SelectListItem> items = new List<SelectListItem>();
            var userid = User.Identity.GetUserId();
            DataTable dtt = new DataTable();
            if (string.IsNullOrEmpty(usernm))
            {
                usernm = "ALL";
            }

            if (tabID == "Dealer")
            {
                dtt.Columns.Add("Dealer FarmName", typeof(string));
                dtt.Columns.Add("Retailer FarmName", typeof(string));
                dtt.Columns.Add("Head", typeof(string));
                dtt.Columns.Add("PayMent Mode", typeof(string));
                dtt.Columns.Add("Total Amount", typeof(string));
                dtt.Columns.Add("Charges", typeof(string));
                dtt.Columns.Add("Net Transfer", typeof(string));
                dtt.Columns.Add("Distributor Remain", typeof(string));
                dtt.Columns.Add("Distributor Remain Updated", typeof(string));
                dtt.Columns.Add("Retailer Remain", typeof(string));
                dtt.Columns.Add("Retailer Remain Updated", typeof(string));
                dtt.Columns.Add("Retailer Pre Credit", typeof(string));
                dtt.Columns.Add("Retailer Updated Credit", typeof(string));
                dtt.Columns.Add("Comment", typeof(string));
                dtt.Columns.Add("Date", typeof(string));
                DateTime fromto = to_date.AddDays(1);
                var respo = db.dealer_to_rem_fund_report(frm_date, fromto, usernm, "ALL", 1, 500).ToList().ToList();

                if (respo.Count > 0)
                {
                    foreach (var item in respo)
                    {
                        dtt.Rows.Add(item.FarmName, item.Frm_Name, item.Head, item.bal_type, item.balance, item.commission, item.balance, item.remain_pre_amount_dealer, item.remain_amount_dealer, item.remain_pre_amount, item.remain_amount, item.oldcrbalance, item.cr, item.comment, item.rechargedate);
                    }
                }
                else
                {
                    dtt.Rows.Add("", "", "", "", "", "", "", "", "", "", "", "", "", "", "");
                }


            }
            else if (tabID == "Retailer")
            {
                dtt.Columns.Add("Sender Retailer", typeof(string));
                dtt.Columns.Add("Reciver Retailer", typeof(string));
                dtt.Columns.Add("Total Amount", typeof(string));
                dtt.Columns.Add("Net Transfer", typeof(string));
                dtt.Columns.Add("Sender Retailer Remain Old", typeof(string));
                dtt.Columns.Add("Sender Retailer Remain Updated", typeof(string));
                dtt.Columns.Add("Reciver Retailer Remain Old", typeof(string));
                dtt.Columns.Add("Reciver Retailer Remain Updated", typeof(string));
                dtt.Columns.Add("Comment", typeof(string));
                dtt.Columns.Add("Date", typeof(string));
                DateTime fromto = to_date.AddDays(1);
                //  var respo = db.show_rem_to_rem_balByAdmin(usernm, "ALL", frm_date, fromto, 1, 500).ToList();
                List<show_rem_to_rem_balByAdmin_Result> respo = new List<show_rem_to_rem_balByAdmin_Result>();
                if (string.IsNullOrEmpty(remreciverss) || remreciverss == "Transfer")
                {
                    respo = db.show_rem_to_rem_balByAdmin(usernm, "ALL", frm_date, fromto, 1, 5000).ToList();

                }
                else if (remreciverss == "Recived")
                {

                    respo = db.show_rem_to_rem_balByAdmin("ALL", usernm, frm_date, fromto, 1, 5000).ToList();

                    // vmodel.show_rem_to_rem_balByAdmin_Recived = db.show_rem_to_rem_balByAdmin("ALL", usernm, fromdate, Todate, 1, 5000).ToList();

                }
                if (respo.Count > 0)
                {
                    foreach (var item in respo)
                    {
                        dtt.Rows.Add(item.Retailerfrom, item.RetailerTOFrm, item.value, item.value, item.rem_from_old_bal, item.rem_from_new, item.rem_to_old_bal, item.rem_to_new, item.comment, item.tran_date);
                    }
                }
                else
                {
                    dtt.Rows.Add("", "", "", "", "", "", "", "", "", "");
                }
            }

            else
            {
                dtt.Columns.Add("MasterFarmName", typeof(string));
                dtt.Columns.Add("DealerFarmName", typeof(string));
                dtt.Columns.Add("Head", typeof(string));
                dtt.Columns.Add("PayMent Mode", typeof(string));
                dtt.Columns.Add("Total Amount", typeof(string));
                dtt.Columns.Add("Charges", typeof(string));
                dtt.Columns.Add("Net Transfer", typeof(string));
                dtt.Columns.Add("Master Remain", typeof(string));
                dtt.Columns.Add("Master Remain Updated", typeof(string));
                dtt.Columns.Add("Dealer Remain Pre", typeof(string));
                dtt.Columns.Add("Dealer Remain Updated", typeof(string));
                dtt.Columns.Add("Dealer Pre Credit", typeof(string));
                dtt.Columns.Add("Dealer Updated Credit", typeof(string));
                dtt.Columns.Add("Comment", typeof(string));
                dtt.Columns.Add("Date", typeof(string));
                DateTime fromto = to_date.AddDays(1);
                var respo = db.Master_to_dealer_report_BYAdmin(frm_date, fromto, usernm, "ALL", 1, 500).ToList().ToList();

                if (respo.Count > 0)
                {
                    foreach (var item in respo)
                    {
                        dtt.Rows.Add(item.masterfarmname, item.dealerFarmName, item.Head, item.bal_type, item.Newbalance, item.comm, item.balance, item.admin_postbal, item.admin_prebal, item.dealer_prebal, item.dealer_postbal, item.oldcrbalance, item.cr, item.comment, item.date_dlm);
                    }
                }
                else
                {
                    dtt.Rows.Add("", "", "", "", "", "", "", "", "", "", "", "", "", "", "");
                }
            }

            var grid = new GridView();
            grid.DataSource = dtt;
            grid.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=Fund_UserToUser_Report.xls");
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

        [PermissioncheckingAttribute(servicename = "FUNDUSERTOUSER", permision = "Read")]
        public ActionResult FundUserToUserPdfGenerate(string tabID, DateTime frm_date, DateTime to_date, string usernm)
        {
            DataTable dtt = new DataTable();
            if (string.IsNullOrEmpty(usernm))
            {
                usernm = "ALL";
            }
            DateTime fromto = to_date.AddDays(1);
            EmployeeFundUserViewModel vmodel = new EmployeeFundUserViewModel();

            if (tabID == "Dealer")
            {
                vmodel.dealer_to_rem_fund_report = db.dealer_to_rem_fund_report(frm_date, fromto, usernm, "ALL", 1, 500).ToList();
                return new ViewAsPdf(vmodel);
            }
            if (tabID == "Retailer")
            {
                vmodel.show_rem_to_rem_balByAdmin = db.show_rem_to_rem_balByAdmin(usernm, "ALL", frm_date, fromto, 1, 500).ToList();
                return new ViewAsPdf(vmodel);
            }
            else
            {
                vmodel.master_to_dealer_report_BYAdmin_Result = db.Master_to_dealer_report_BYAdmin(frm_date, fromto, usernm, "ALL", 1, 500).ToList();
                return View(vmodel);
                // return new ViewAsPdf(vmodel);
            }

        }


        public ActionResult MToDLM_Creditchk(string MID, string dlmid)
        {

            var userid = db.Dealer_Details.Where(x => x.DealerId == dlmid && x.SSId == MID).FirstOrDefault(); //User.Identity.GetUserId();
            decimal? dlmbal;
            if (userid == null)
            {
                var ch = "Error";
                return Json(ch, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var ch = db.admin_to_dealer.Where(aa => aa.balance_from == userid.SSId && aa.dealerid == userid.DealerId).OrderByDescending(aa => aa.idno).Select(c => c.cr).FirstOrDefault();
                ch = ch ?? 0;
                if (string.IsNullOrEmpty(dlmid))
                {
                    dlmbal = 0;
                }
                else
                {
                    dlmbal = db.Remain_dealer_balance.Where(x => x.DealerID == dlmid).SingleOrDefault().Remainamount;
                }
                return Json(new { currntcr = ch, rembal = dlmbal }, JsonRequestBehavior.AllowGet);
                //return Json(ch, JsonRequestBehavior.AllowGet);
            }




        }


        public ActionResult DLMTORLM_Creditchk(string retailerid, string dlmid)
        {

            var userid = db.Retailer_Details.Where(x => x.DealerId == dlmid && x.RetailerId == retailerid && x.ISDeleteuser == false).FirstOrDefault(); //User.Identity.GetUserId();
            decimal? rembal;
            if (userid == null)
            {
                var ch = "Error";
                return Json(ch, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var ch = db.Dealer_To_Retailer_Balance.Where(aa => aa.RetailerId == userid.RetailerId && aa.DealerId == userid.DealerId).OrderByDescending(aa => aa.id).Select(c => c.cr).FirstOrDefault();
                ch = ch ?? 0;

                if (string.IsNullOrEmpty(retailerid))
                {
                    rembal = 0;
                }
                else
                {
                    rembal = db.Remain_reteller_balance.Where(x => x.RetellerId == userid.RetailerId).SingleOrDefault().Remainamount;
                }
                return Json(new { currntcr = ch, rembal = rembal }, JsonRequestBehavior.AllowGet);


            }


        }

        public JsonResult GetRetailerByID(string retID)
        {
            var dbData = db.Retailer_Details.Where(x => !x.RetailerId.Equals(retID)).ToList();
            var retailers = GetRetailerlistSelectListItems(dbData);
            return Json(new { Retailer = retailers }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult BindRetailerByID(string did)
        {

            var dbData = db.Retailer_Details.Where(x => x.DealerId == did).ToList();
            var dealer = GetRetailerlistSelectListItems(dbData);
            var query = db.bank_info.Where(a => a.userid == did).Select(c => new SelectListItem
            {
                Value = c.acno.ToString(),
                Text = c.banknm,

            });

            var fillwallet = db.tblwallet_info.Where(a => a.userid == did).Select(aa => new SelectListItem
            {
                Text = aa.walletname,
                Value = aa.walletno.ToString(),

            });

            return Json(new { dealer = dealer, bnkinfo = query.AsEnumerable(), walletinfo = fillwallet.AsEnumerable() }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult BindDealerByID(string ssid)
        {

            var dbData = db.Dealer_Details.Where(x => x.SSId == ssid).ToList();
            var dealer = GetDealerSelectListItems(dbData);
            var query = db.bank_info.Where(a => a.userid == ssid).Select(c => new SelectListItem
            {
                Value = c.acno.ToString(),
                Text = c.banknm,

            });

            var fillwallet = db.tblwallet_info.Where(a => a.userid == "Admin").Select(aa => new SelectListItem
            {
                Text = aa.walletname,
                Value = aa.walletno.ToString(),

            });

            return Json(new { dealer = dealer, bnkinfo = query.AsEnumerable(), walletinfo = fillwallet.AsEnumerable() }, JsonRequestBehavior.AllowGet);

        }


        private IEnumerable<SelectListItem> GetDealerSelectListItems(IEnumerable<Dealer_Details> elements)
        {
            var selectList = new List<SelectListItem>();
            foreach (var element in elements)
            {
                selectList.Add(new SelectListItem
                {
                    Value = element.DealerId.ToString(),
                    Text = element.FarmName
                });
            }
            return selectList;
        }

        public JsonResult FillDealerDropDownList(string nameoftab)
        {
            EmployeFundtransferviewmodel vmodel = new EmployeFundtransferviewmodel();


            if (nameoftab == "menu1")
            {
                var fillDealer = db.Dealer_Details.Select(aa => new SelectListItem
                {
                    Text = aa.FarmName + " - " + aa.Mobile,
                    Value = aa.DealerId.ToString(),


                });

                //vmodel.ddldistributorInfo = fillRetailer.AsEnumerable();

                return Json(fillDealer.AsEnumerable());
            }


            if (nameoftab == "menu2")
            {
                var fillRetailer = db.Retailer_Details.Select(aa => new SelectListItem
                {
                    Text = aa.Frm_Name == null ? aa.RetailerName : aa.Frm_Name + " - " + aa.Mobile,
                    Value = aa.RetailerId.ToString(),


                });



                return Json(fillRetailer.AsEnumerable());
            }
            return Json("rav i");

        }


        private IEnumerable<SelectListItem> GetMasterSelectListItems(IEnumerable<Superstokist_details> elements)
        {
            var selectList = new List<SelectListItem>();
            foreach (var element in elements)
            {
                selectList.Add(new SelectListItem
                {
                    Value = element.SSId.ToString(),
                    Text = element.FarmName
                });
            }
            return selectList;
        }


        private IEnumerable<SelectListItem> GetDealerlistSelectListItems(IEnumerable<Dealer_Details> elements)
        {
            var selectList = new List<SelectListItem>();
            foreach (var element in elements)
            {
                selectList.Add(new SelectListItem
                {
                    Value = element.DealerId.ToString(),
                    Text = element.FarmName
                });
            }
            return selectList;
        }

        private IEnumerable<SelectListItem> GetRetailerlistSelectListItems(IEnumerable<Retailer_Details> elements)
        {
            var selectList = new List<SelectListItem>();
            foreach (var element in elements)
            {
                selectList.Add(new SelectListItem
                {
                    Value = element.RetailerId.ToString(),
                    Text = element.Frm_Name
                });
            }
            return selectList;
        }

        #endregion Fundusertouser



        #region AirTicket

        [HttpGet]
        [PermissioncheckingAttribute(servicename = "FLIGHT", permision = "Read")]
        public ActionResult TicketReport()
        {
            try
            {
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    // show master id 
                    var stands = db.Superstokist_details.ToList();
                    IEnumerable<SelectListItem> selectList = from s in stands
                                                             select new SelectListItem
                                                             {
                                                                 Value = s.SSId,
                                                                 Text = s.FarmName.ToString()
                                                             };
                    ViewBag.allmaster = new SelectList(selectList, "Value", "Text");
                    //show whitelabel
                    var totalwhitelabel = db.WhiteLabel_userList.ToList();
                    IEnumerable<SelectListItem> selectList1 = from s in totalwhitelabel
                                                              select new SelectListItem
                                                              {
                                                                  Value = s.WhiteLabelID,
                                                                  Text = s.FrmName.ToString()
                                                              };
                    ViewBag.allwhitelabel = new SelectList(selectList1, "Value", "Text");
                    //show dealer
                    ViewBag.alldealer = new SelectList(db.select_Dealer_for_ddl(), "Dealerid", "FarmName", null).ToList();
                    //Retailer 
                    ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl("Admin"), "RetailerId", "Frm_Name", null).ToList();




                    //DateTime startDate = DateTime.Now.Date;
                    //DateTime endDate = DateTime.Now.AddDays(1).Date;
                    //var userid = User.Identity.GetUserId();
                    //var proc_Response = db.proc_FlightReport(1,20, null, null, null, null, null, null, null, null, null, startDate, endDate).ToList();
                    ////ViewData["Totalpublishfare"] = proc_Response.Where(s => s.TicketStatus.ToUpper().Contains("Success")).Sum(s => Convert.ToInt32(s.PublishedFare));
                    //ViewData["Totalofferfare"] = proc_Response.Where(s => s.TicketStatus.ToUpper().Contains("Success")).Sum(s => Convert.ToInt32(s.OfferedFare));
                    //ViewData["totalf"] = proc_Response.Where(s => s.TicketStatus.ToUpper().Contains("FAILED")).Sum(s => Convert.ToInt32(s.FareAmount));
                    //ViewData["totalp"] = proc_Response.Where(s => s.TicketStatus.ToUpper().Contains("Proccessed")).Sum(s => Convert.ToInt32(s.FareAmount));
                    //return View(proc_Response);
                    return View();
                }
            }
            catch
            {
                TempData["Status"] = "Failed";
                TempData["Message"] = "Failed to connect with provider.please try again.";
                return View();
            }
        }
        [HttpPost]
        [PermissioncheckingAttribute(servicename = "FLIGHT", permision = "Read")]
        public ActionResult TicketReport(string txt_frm_date, string txt_to_date)
        {
            try
            {
                ViewBag.chk = "post";
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    // show master id 
                    var stands = db.Superstokist_details.ToList();
                    IEnumerable<SelectListItem> selectList = from s in stands
                                                             select new SelectListItem
                                                             {
                                                                 Value = s.SSId,
                                                                 Text = s.FarmName.ToString()
                                                             };
                    ViewBag.allmaster = new SelectList(selectList, "Value", "Text");
                    //show whitelabel
                    var totalwhitelabel = db.WhiteLabel_userList.ToList();
                    IEnumerable<SelectListItem> selectList1 = from s in totalwhitelabel
                                                              select new SelectListItem
                                                              {
                                                                  Value = s.WhiteLabelID,
                                                                  Text = s.FrmName.ToString()
                                                              };
                    ViewBag.allwhitelabel = new SelectList(selectList1, "Value", "Text");
                    //show dealer
                    ViewBag.alldealer = new SelectList(db.select_Dealer_for_ddl(), "Dealerid", "FarmName", null).ToList();
                    //Retailer 
                    ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl("Admin"), "RetailerId", "Frm_Name", null).ToList();

                    //DateTime frm = Convert.ToDateTime(txt_frm_date);
                    //DateTime to = Convert.ToDateTime(txt_to_date);
                    //txt_frm_date = frm.ToString("dd-MM-yyyy");
                    //txt_to_date = to.ToString("dd-MM-yyyy");
                    //string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
                    //DateTime dt = DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
                    //DateTime dt1 = DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
                    //DateTime frm_date = dt.Date;
                    //DateTime to_date = dt1.AddDays(1);

                    //var proc_Response = db.proc_FlightReport(1,20, ddl_status, retailerid, DealerId, MasterId, null, null, PNR, null, null, frm_date, to_date).ToList();
                    //ViewData["Totalofferfare"] = proc_Response.Where(s => s.TicketStatus.ToUpper().Contains("Success")).Sum(s => Convert.ToInt32(s.OfferedFare));
                    //ViewData["totalf"] = proc_Response.Where(s => s.TicketStatus.ToUpper().Contains("FAILED")).Sum(s => Convert.ToInt32(s.FareAmount));
                    //ViewData["totalp"] = proc_Response.Where(s => s.TicketStatus.ToUpper().Contains("Proccessed")).Sum(s => Convert.ToInt32(s.FareAmount));
                    //return View(proc_Response);
                    return View();
                }
            }
            catch
            {
                TempData["Status"] = "Failed";
                TempData["Message"] = "Failed to connect with provider.please try again.";
                return RedirectToAction("Travel", "Home");
            }
        }
        [ChildActionOnly]
        public ActionResult _ticketReport(string txt_frm_date, string txt_to_date, string ddl_status, string PNR, string ddlusers, string allmaster, string alldealer, string allretailer, string allwhitelabel)
        {
            try
            {
                ViewBag.chk = "post";
                string retailerid = null;
                string DealerId = null;
                string MasterId = null;
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    // show master id 
                    var stands = db.Superstokist_details.ToList();
                    IEnumerable<SelectListItem> selectList = from s in stands
                                                             select new SelectListItem
                                                             {
                                                                 Value = s.SSId,
                                                                 Text = s.FarmName.ToString()
                                                             };
                    ViewBag.allmaster = new SelectList(selectList, "Value", "Text");
                    //show whitelabel
                    var totalwhitelabel = db.WhiteLabel_userList.ToList();
                    IEnumerable<SelectListItem> selectList1 = from s in totalwhitelabel
                                                              select new SelectListItem
                                                              {
                                                                  Value = s.WhiteLabelID,
                                                                  Text = s.FrmName.ToString()
                                                              };
                    ViewBag.allwhitelabel = new SelectList(selectList1, "Value", "Text");
                    //show dealer
                    ViewBag.alldealer = new SelectList(db.select_Dealer_for_ddl(), "Dealerid", "FarmName", null).ToList();
                    //Retailer 
                    ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl("Admin"), "RetailerId", "Frm_Name", null).ToList();

                    if ((txt_frm_date == null && txt_to_date == null) || (txt_frm_date == "" && txt_to_date == ""))
                    {
                        txt_frm_date = DateTime.Now.ToString();
                        txt_to_date = DateTime.Now.ToString();

                    }


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
                    if (ddlusers == "Master")
                    {
                        if (allmaster == "" || allmaster.Contains("All Master") || allmaster == null)
                        {
                            MasterId = null;
                        }
                        else
                        {
                            MasterId = allmaster;
                        }
                    }
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
                    if (ddlusers == "Admin")
                    {
                        retailerid = null;
                        DealerId = null;
                        MasterId = null;
                    }
                    var proc_Response = db.proc_FlightReport(1, 35, ddl_status, retailerid, DealerId, MasterId, null, null, PNR, null, null, frm_date, to_date).ToList();
                    return View(proc_Response);
                }
            }
            catch
            {
                TempData["Status"] = "Failed";
                TempData["Message"] = "Failed to connect with provider.please try again.";
                return RedirectToAction("TicketReport", "Air");
            }
        }
        [HttpPost]

        public ActionResult scroll_TicketReport(int pageindex, string txt_frm_date, string txt_to_date, string PNR, string ddl_status, string ddlusers, string allmaster, string alldealer, string allretailer, string allwhitelabel)
        {
            try
            {
                ViewBag.chk = "post";
                string retailerid = null;
                string DealerId = null;
                string MasterId = null;
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    // show master id 
                    var stands = db.Superstokist_details.ToList();
                    IEnumerable<SelectListItem> selectList = from s in stands
                                                             select new SelectListItem
                                                             {
                                                                 Value = s.SSId,
                                                                 Text = s.FarmName.ToString()
                                                             };
                    ViewBag.allmaster = new SelectList(selectList, "Value", "Text");
                    //show whitelabel
                    var totalwhitelabel = db.WhiteLabel_userList.ToList();
                    IEnumerable<SelectListItem> selectList1 = from s in totalwhitelabel
                                                              select new SelectListItem
                                                              {
                                                                  Value = s.WhiteLabelID,
                                                                  Text = s.FrmName.ToString()
                                                              };
                    ViewBag.allwhitelabel = new SelectList(selectList1, "Value", "Text");
                    //show dealer
                    ViewBag.alldealer = new SelectList(db.select_Dealer_for_ddl(), "Dealerid", "FarmName", null).ToList();
                    //Retailer 
                    ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl("Admin"), "RetailerId", "Frm_Name", null).ToList();

                    if ((txt_frm_date == null && txt_to_date == null) || (txt_frm_date == "" && txt_to_date == ""))
                    {
                        txt_frm_date = DateTime.Now.ToString();
                        txt_to_date = DateTime.Now.ToString();

                    }

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
                    if (ddlusers == "Master")
                    {
                        if (allmaster == "" || allmaster.Contains("All Master") || allmaster == null)
                        {
                            MasterId = null;
                        }
                        else
                        {
                            MasterId = allmaster;
                        }
                    }
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
                    if (ddlusers == "Admin")
                    {
                        retailerid = null;
                        DealerId = null;
                        MasterId = null;
                    }
                    var proc_Response = db.proc_FlightReport(pageindex, 35, ddl_status, retailerid, DealerId, MasterId, null, null, PNR, null, null, frm_date, to_date).ToList();

                    int pagesize = 35;
                    JsonModel jsonmodel = new JsonModel();
                    jsonmodel.NoMoredata = proc_Response.Count < pagesize;
                    jsonmodel.HTMLString = renderPartialViewtostring("_ticketReport", proc_Response);
                    return Json(jsonmodel);
                }
            }
            catch
            {
                TempData["Status"] = "Failed";
                TempData["Message"] = "Failed to connect with provider.please try again.";
                return RedirectToAction("TicketReport", "Air");
            }
        }

        [PermissioncheckingAttribute(servicename = "FLIGHT", permision = "Read")]
        public ActionResult ExcelTicketReport(string txt_frm_date, string txt_to_date, string ddl_status, string PNR, string ddlusers, string allmaster, string alldealer, string allretailer, string allwhitelabel)
        {
            try
            {
                ViewBag.chk = "post";
                string retailerid = null;
                string DealerId = null;
                string MasterId = null;
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    // show master id 
                    var stands = db.Superstokist_details.ToList();
                    IEnumerable<SelectListItem> selectList = from s in stands
                                                             select new SelectListItem
                                                             {
                                                                 Value = s.SSId,
                                                                 Text = s.FarmName.ToString()
                                                             };
                    ViewBag.allmaster = new SelectList(selectList, "Value", "Text");
                    //show whitelabel
                    var totalwhitelabel = db.WhiteLabel_userList.ToList();
                    IEnumerable<SelectListItem> selectList1 = from s in totalwhitelabel
                                                              select new SelectListItem
                                                              {
                                                                  Value = s.WhiteLabelID,
                                                                  Text = s.FrmName.ToString()
                                                              };
                    ViewBag.allwhitelabel = new SelectList(selectList1, "Value", "Text");
                    //show dealer
                    ViewBag.alldealer = new SelectList(db.select_Dealer_for_ddl(), "Dealerid", "FarmName", null).ToList();
                    //Retailer 
                    ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl("Admin"), "RetailerId", "Frm_Name", null).ToList();

                    if ((txt_frm_date == null && txt_to_date == null) || (txt_frm_date == "" && txt_to_date == ""))
                    {
                        txt_frm_date = DateTime.Now.ToString();
                        txt_to_date = DateTime.Now.ToString();

                    }


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
                    if (ddlusers == "Master")
                    {
                        if (allmaster == "" || allmaster.Contains("All Master") || allmaster == null)
                        {
                            MasterId = null;
                        }
                        else
                        {
                            MasterId = allmaster;
                        }
                    }
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
                    if (ddlusers == "Admin")
                    {
                        retailerid = null;
                        DealerId = null;
                        MasterId = null;
                    }
                    if (PNR == "undefined")
                    {
                        PNR = null;
                    }

                    DataTable dtt = new DataTable();
                    dtt.Columns.Add("Status", typeof(string));
                    dtt.Columns.Add("Firm Name", typeof(string));
                    dtt.Columns.Add("Retailer Name", typeof(string));
                    dtt.Columns.Add("Passenger Name", typeof(string));
                    dtt.Columns.Add("Flight", typeof(string));
                    dtt.Columns.Add("Booking Id", typeof(string));
                    dtt.Columns.Add("PNR", typeof(string));
                    dtt.Columns.Add("Booking Date", typeof(string));
                    dtt.Columns.Add("Fare Amount", typeof(string));
                    dtt.Columns.Add("Retailer Pre", typeof(string));
                    dtt.Columns.Add("Retailer Inc", typeof(string));
                    dtt.Columns.Add("Retailer Post", typeof(string));
                    dtt.Columns.Add("Dealer Pre", typeof(string));
                    dtt.Columns.Add("Dealer Inc", typeof(string));
                    dtt.Columns.Add("Dealer Post", typeof(string));
                    dtt.Columns.Add("Master Pre", typeof(string));
                    dtt.Columns.Add("Master Inc", typeof(string));
                    dtt.Columns.Add("Master Post", typeof(string));
                    dtt.Columns.Add("Admin Pre", typeof(string));
                    dtt.Columns.Add("Admin Inc", typeof(string));
                    dtt.Columns.Add("Admin Post", typeof(string));

                    var proc_Response = db.proc_FlightReport(1, 1000000, ddl_status, retailerid, DealerId, MasterId, null, null, PNR, null, null, frm_date, to_date).ToList();

                    if (proc_Response.Count > 0)
                    {
                        foreach (var item in proc_Response)
                        {
                            var sts = item.TicketStatus;
                            if (item.TicketStatus.ToUpper().Contains("REQ"))
                            {
                                sts = "Pending";
                            }
                            dtt.Rows.Add(sts, item.Frm_Name, item.RetailerName, item.LeadPaxFirstName + " " + item.LeadPaxLastName,
                                item.AirlineName, item.BookingId,
                            item.PNR, item.TicketDate, item.FareAmount, item.RemPre, item.RemInc, item.RemPost, item.DlmPre, item.DlmInc, item.DlmPost, item.MdPre, item.MdInc, item.MdPost, item.AdminPre, item.AdminInc, item.AdminPost);
                        }
                    }
                    else
                    {
                        dtt.Rows.Add("", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "");
                    }


                    var grid = new GridView();
                    grid.DataSource = dtt;
                    grid.DataBind();
                    Response.ClearContent();
                    Response.Buffer = true;
                    Response.AddHeader("content-disposition", "attachment; filename=Flight_Ticket_Report.xls");
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
            catch
            {
                TempData["Status"] = "Failed";
                TempData["Message"] = "Failed to connect with provider.please try again.";
                return RedirectToAction("TicketReport", "Home");
            }
        }
        [PermissioncheckingAttribute(servicename = "FLIGHT", permision = "Read")]
        public ActionResult PDFTicketReport(string txt_frm_date, string txt_to_date, string ddl_status, string PNR, string ddlusers, string allmaster, string alldealer, string allretailer, string allwhitelabel)
        {
            try
            {
                ViewBag.chk = "post";
                string retailerid = null;
                string DealerId = null;
                string MasterId = null;
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    // show master id 
                    var stands = db.Superstokist_details.ToList();
                    IEnumerable<SelectListItem> selectList = from s in stands
                                                             select new SelectListItem
                                                             {
                                                                 Value = s.SSId,
                                                                 Text = s.FarmName.ToString()
                                                             };
                    ViewBag.allmaster = new SelectList(selectList, "Value", "Text");
                    //show whitelabel
                    var totalwhitelabel = db.WhiteLabel_userList.ToList();
                    IEnumerable<SelectListItem> selectList1 = from s in totalwhitelabel
                                                              select new SelectListItem
                                                              {
                                                                  Value = s.WhiteLabelID,
                                                                  Text = s.FrmName.ToString()
                                                              };
                    ViewBag.allwhitelabel = new SelectList(selectList1, "Value", "Text");
                    //show dealer
                    ViewBag.alldealer = new SelectList(db.select_Dealer_for_ddl(), "Dealerid", "FarmName", null).ToList();
                    //Retailer 
                    ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl("Admin"), "RetailerId", "Frm_Name", null).ToList();

                    if ((txt_frm_date == null && txt_to_date == null) || (txt_frm_date == "" && txt_to_date == ""))
                    {
                        txt_frm_date = DateTime.Now.ToString();
                        txt_to_date = DateTime.Now.ToString();

                    }

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
                    if (ddlusers == "Master")
                    {
                        if (allmaster == "" || allmaster.Contains("All Master") || allmaster == null)
                        {
                            MasterId = null;
                        }
                        else
                        {
                            MasterId = allmaster;
                        }
                    }
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
                    if (ddlusers == "Admin")
                    {
                        retailerid = null;
                        DealerId = null;
                        MasterId = null;
                    }
                    var proc_Response = db.proc_FlightReport(1, 1000000, ddl_status, retailerid, DealerId, MasterId, null, null, PNR, null, null, frm_date, to_date).ToList();
                    return new ViewAsPdf(proc_Response);
                }
            }
            catch
            {
                TempData["Status"] = "Failed";
                TempData["Message"] = "Failed to connect with provider.please try again.";
                return RedirectToAction("TicketReport", "Home");
            }
        }
        [PermissioncheckingAttribute(servicename = "FLIGHT", permision = "Read")]
        public ActionResult TotalticketReport(string txt_frm_date, string txt_to_date, string ddl_status, string PNR, string ddlusers, string allmaster, string alldealer, string allretailer, string allwhitelabel)
        {
            try
            {
                ViewBag.chk = "post";
                string retailerid = null;
                string DealerId = null;
                string MasterId = null;
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    // show master id 
                    var stands = db.Superstokist_details.ToList();
                    IEnumerable<SelectListItem> selectList = from s in stands
                                                             select new SelectListItem
                                                             {
                                                                 Value = s.SSId,
                                                                 Text = s.FarmName.ToString()
                                                             };
                    ViewBag.allmaster = new SelectList(selectList, "Value", "Text");
                    //show whitelabel
                    var totalwhitelabel = db.WhiteLabel_userList.ToList();
                    IEnumerable<SelectListItem> selectList1 = from s in totalwhitelabel
                                                              select new SelectListItem
                                                              {
                                                                  Value = s.WhiteLabelID,
                                                                  Text = s.FrmName.ToString()
                                                              };
                    ViewBag.allwhitelabel = new SelectList(selectList1, "Value", "Text");
                    //show dealer
                    ViewBag.alldealer = new SelectList(db.select_Dealer_for_ddl(), "Dealerid", "FarmName", null).ToList();
                    //Retailer 
                    ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl("Admin"), "RetailerId", "Frm_Name", null).ToList();

                    if ((txt_frm_date == null && txt_to_date == null) || (txt_frm_date == "" && txt_to_date == ""))
                    {
                        txt_frm_date = DateTime.Now.ToString();
                        txt_to_date = DateTime.Now.ToString();

                    }

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
                    if (ddlusers == "Master")
                    {
                        if (allmaster == "" || allmaster.Contains("All Master") || allmaster == null)
                        {
                            MasterId = null;
                        }
                        else
                        {
                            MasterId = allmaster;
                        }
                    }
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
                    if (ddlusers == "Admin")
                    {
                        retailerid = null;
                        DealerId = null;
                        MasterId = null;
                    }
                    var chk = db.proc_FlightReport(1, 1000000, ddl_status, retailerid, DealerId, MasterId, null, null, PNR, null, null, frm_date, to_date).ToList();
                    var successtotal = chk.Where(a => a.TicketStatus.ToUpper() == "SUCCESS").Sum(a => a.FareAmount); //chk.Where(aa => aa.rstatus.ToUpper() == "SUCCESS").Sum(aa => aa.amount);
                    var Failedtotal = chk.Where(aa => aa.TicketStatus.ToUpper() == "FAILED").Sum(aa => aa.FareAmount);
                    var pendingtotal = chk.Where(aa => aa.TicketStatus.ToUpper().Contains("PROCCESSED")).Sum(aa => aa.FareAmount);

                    var data = new
                    {
                        success = successtotal,
                        failed = Failedtotal,
                        pending = pendingtotal
                    };
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                TempData["Status"] = "Failed";
                TempData["Message"] = "Failed to connect with provider.please try again.";
                return RedirectToAction("TicketReport", "Air");
            }
        }
        [HttpPost]
        [PermissioncheckingAttribute(servicename = "FLIGHT", permision = "Write")]
        public ActionResult GetFlightStatus(TicketBookinDetailsModel model)
        {
            try
            {
                var token = string.Empty;
                token = getAuthToken();
                if (string.IsNullOrWhiteSpace(token))
                {
                    var viewRespo = new { Status = "Failed", StatusCode = 0, Message = "Server Down!!" };
                    return Json(JsonConvert.SerializeObject(viewRespo), JsonRequestBehavior.AllowGet);
                }
                var client = new RestClient(VastbazaarBaseUrl + "api/Air/GetBookingDetails");
                var request = new RestRequest(Method.POST);
                request.RequestFormat = DataFormat.Json;
                request.AddHeader("authorization", "bearer " + token);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("content-type", "application/json");

                request.AddBody(model);
                IRestResponse response = client.Execute(request);
                dynamic respo = JsonConvert.DeserializeObject<dynamic>(response.Content);

                if (respo.StatusCode == 200 && respo.Content != null && respo.Content.ResponseCode == 0 && respo.Content.ADDINFO.Error.ErrorCode == 0)
                {
                    var viewRespo = new { Status = "Success", StatusCode = respo.Content.ADDINFO.FlightItinerary.Status, Message = "" };
                    return Json(JsonConvert.SerializeObject(viewRespo), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    using (VastwebmultiEntities db = new VastwebmultiEntities())
                    {
                        var ticket = db.TBO_AirTicketingDetails.Where(a => a.TraceId == model.TraceId).FirstOrDefault();
                        if (ticket != null)
                        {
                            try
                            {
                                System.Data.Entity.Core.Objects.ObjectParameter IsSuccess = new System.Data.Entity.Core.Objects.ObjectParameter("IsSuccess", typeof(bool));
                                System.Data.Entity.Core.Objects.ObjectParameter Message = new System.Data.Entity.Core.Objects.ObjectParameter("Message", typeof(Boolean));
                                //System.Data.Entity.Core.Objects.ObjectParameter Idno = new System.Data.Entity.Core.Objects.ObjectParameter("IdNo", typeof(int));
                                db.proc_UpdateFlightBooking(ticket.idno.ToString(), ticket.RetailerId, ticket.OfferedFare, ticket.PublishedFare, response.Content, 1, 0, "", "", "", "", IsSuccess, Message);
                            }
                            catch
                            {

                            }
                        }
                        var viewRespo = new { Status = "Failed", StatusCode = 0, Message = "Server Down!!" };
                        return Json(JsonConvert.SerializeObject(viewRespo), JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch
            {
                var viewRespo = new { Status = "Failed", StatusCode = 0, Message = "Server Down!!" };
                return Json(JsonConvert.SerializeObject(viewRespo), JsonRequestBehavior.AllowGet);
            }
            //return View(respo);
        }
        [PermissioncheckingAttribute(servicename = "FLIGHT", permision = "Read")]
        public ActionResult CancellationReport()
        {
            try
            {
                var userid = User.Identity.GetUserId();
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    // show master id 
                    var stands = db.Superstokist_details.ToList();
                    IEnumerable<SelectListItem> selectList = from s in stands
                                                             select new SelectListItem
                                                             {
                                                                 Value = s.SSId,
                                                                 Text = s.FarmName.ToString()
                                                             };
                    ViewBag.allmaster = new SelectList(selectList, "Value", "Text");
                    //show whitelabel
                    var totalwhitelabel = db.WhiteLabel_userList.ToList();
                    IEnumerable<SelectListItem> selectList1 = from s in totalwhitelabel
                                                              select new SelectListItem
                                                              {
                                                                  Value = s.WhiteLabelID,
                                                                  Text = s.FrmName.ToString()
                                                              };
                    ViewBag.allwhitelabel = new SelectList(selectList1, "Value", "Text");
                    //show dealer
                    ViewBag.alldealer = new SelectList(db.select_Dealer_for_ddl(), "Dealerid", "FarmName", null).ToList();
                    //Retailer 
                    ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl("Admin"), "RetailerId", "Frm_Name", null).ToList();


                    DateTime startDate = DateTime.Now.Date;
                    DateTime endDate = DateTime.Now.AddDays(1).Date;
                    var procRespo = db.proc_FlightCancellationReport(1, 20, "ALL", null, null, null, null, null, null, Convert.ToDateTime(startDate), Convert.ToDateTime(endDate)).ToList();

                    return View(procRespo);
                }
            }
            catch (Exception ex)
            {
                TempData["Status"] = "Failed";
                TempData["Message"] = "Failed to connect with provider.please try again.";
                return RedirectToAction("Travel", "Home");
            }
        }
        [HttpPost]
        [PermissioncheckingAttribute(servicename = "FLIGHT", permision = "Read")]
        public ActionResult CancellationReport(string txt_frm_date, string txt_to_date, int? ddl_top, string ddl_status, string PNR, string ddlusers, string allmaster, string alldealer, string allretailer, string allwhitelabe)
        {
            try
            {
                var userid = User.Identity.GetUserId();
                ViewBag.chk = "post";
                string retailerid = null;
                string DealerId = null;
                string MasterId = null;
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    DateTime frm = Convert.ToDateTime(txt_frm_date);
                    DateTime to = Convert.ToDateTime(txt_to_date);
                    txt_frm_date = frm.ToString("dd-MM-yyyy");
                    txt_to_date = to.ToString("dd-MM-yyyy");
                    string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
                    DateTime dt = DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
                    DateTime dt1 = DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
                    DateTime frm_date = dt.Date;
                    DateTime to_date = dt1.AddDays(1);
                    // show master id 
                    var stands = db.Superstokist_details.ToList();
                    IEnumerable<SelectListItem> selectList = from s in stands
                                                             select new SelectListItem
                                                             {
                                                                 Value = s.SSId,
                                                                 Text = s.FarmName.ToString()
                                                             };
                    ViewBag.allmaster = new SelectList(selectList, "Value", "Text");
                    //show whitelabel
                    var totalwhitelabel = db.WhiteLabel_userList.ToList();
                    IEnumerable<SelectListItem> selectList1 = from s in totalwhitelabel
                                                              select new SelectListItem
                                                              {
                                                                  Value = s.WhiteLabelID,
                                                                  Text = s.FrmName.ToString()
                                                              };
                    ViewBag.allwhitelabel = new SelectList(selectList1, "Value", "Text");
                    //show dealer
                    ViewBag.alldealer = new SelectList(db.select_Dealer_for_ddl(), "Dealerid", "FarmName", null).ToList();
                    //Retailer 
                    ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl("Admin"), "RetailerId", "Frm_Name", null).ToList();


                    if (ddl_top == null)
                    {
                        ddl_top = 10000;
                    }
                    PNR = string.IsNullOrWhiteSpace(PNR) ? null : PNR;
                    ddl_status = string.IsNullOrWhiteSpace(ddl_status) ? "ALL" : ddl_status;
                    if (ddlusers == "Master")
                    {
                        if (allmaster == "" || allmaster.Contains("Master") || allmaster == null)
                        {
                            MasterId = null;
                        }
                        else
                        {
                            MasterId = allmaster;
                        }
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
                    if (ddlusers == "Admin")
                    {
                        retailerid = null;
                        DealerId = null;
                        MasterId = null;
                    }
                    var procRespo = db.proc_FlightCancellationReport(1, 20, ddl_status, retailerid, DealerId, MasterId, null, null, PNR, frm_date, to_date).ToList();
                    return View(procRespo);
                }
            }
            catch
            {
                TempData["Status"] = "Failed";
                TempData["Message"] = "Failed to connect with provider.please try again.";
                return RedirectToAction("Travel", "Home");
            }
        }
        [HttpPost]
        [PermissioncheckingAttribute(servicename = "FLIGHT", permision = "Write")]
        public ActionResult CancellationStatus(int ChangeReqId, int idno)
        {
            try
            {
                var token = string.Empty;
                token = getAuthToken();
                if (string.IsNullOrWhiteSpace(token))
                {
                    UpdateAuthToken();
                    var jsrespo = new { Status = "Failed", Message = "Failed to connect with provider,please try later" };
                    return Json(JsonConvert.SerializeObject(jsrespo));
                }

                var SendChangeRequest = new
                {
                    ChangeRequestId = ChangeReqId
                };

                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {

                    var client = new RestClient(VastbazaarBaseUrl + "api/Air/GetCancellationRequestStatus");
                    var request = new RestRequest(Method.POST);
                    request.RequestFormat = DataFormat.Json;
                    request.AddHeader("authorization", "bearer " + token);
                    request.AddHeader("cache-control", "no-cache");
                    request.AddHeader("content-type", "application/json");
                    //request.AddParameter("application/json", "{\r\n\"EndUserIp\": \"192.168.10.10\",\r\n\"TokenId\": \"ac2751e9-4cc3-406f-b678-c947e4f57a00\",\r\n\"AdultCount\": \"1\",\r\n\"ChildCount\": \"0\",\r\n\"InfantCount\": \"0\",\r\n\"DirectFlight\": \"false\",\r\n\"OneStopFlight\": \"false\",\r\n\"JourneyType\": \"1\",\r\n\"PreferredAirlines\": null,\r\n\"Segments\": [\r\n{\r\n\"Origin\": \"DEL\",\r\n\"Destination\": \"BOM\",\r\n        \"FlightCabinClass\": \"1\",\r\n\"PreferredDepartureTime\": \"2018-09-06T00: 00: 00\",\r\n\"PreferredArrivalTime\": \"2018-09-06T00: 00: 00\"\r\n}\r\n        ],\r\n\"Sources\": [\r\n\"6E\"\r\n]\r\n}", ParameterType.RequestBody);
                    request.AddBody(SendChangeRequest);
                    IRestResponse response = client.Execute(request);

                    dynamic respo = JsonConvert.DeserializeObject(response.Content);


                    //string responseJs = "{\"Version\":\"1.0\",\"StatusCode\":200,\"Content\":{\"ResponseCode\":1,\"ADDINFO\":{\"B2B2BStatus\":null,\"ResponseStatus\":2,\"TraceId\":\"c4a3dd8e-f2ac-4fde-bfc0-6653821286fa\",\"Error\":{\"ErrorCode\":0,\"ErrorMessage\":\"\"}}}}";
                    if (respo.StatusCode == 200 && respo.Content.ADDINFO.ResponseStatus == 1)
                    {
                        if (respo.Content.Error.ErrorCode == 0)
                        {
                            int ChangeRequestStatus = Convert.ToString(respo.Content.ADDINFO.ChangeRequestStatus);
                            string Remark = Convert.ToInt32(respo.Content.Error.ErrorMessage);
                            decimal RefundAmount = Convert.ToDecimal(respo.Content.ADDINFO.RefundedAmount);
                            decimal CancellationCharge = Convert.ToDecimal(respo.Content.ADDINFO.CancellationCharge);
                            System.Data.Entity.Core.Objects.ObjectParameter Status = new System.Data.Entity.Core.Objects.ObjectParameter("Status", typeof(bool));
                            System.Data.Entity.Core.Objects.ObjectParameter Message = new System.Data.Entity.Core.Objects.ObjectParameter("Message", typeof(Boolean));
                            var procRespo = db.proc_FlightCancellationRefund(idno, ChangeReqId, RefundAmount, CancellationCharge, ChangeRequestStatus, response.Content, Status, Message);
                            var errMsg = Convert.ToString(respo.Content.ADDINFO.Error.ErrorMessage);
                            var jsrespo = new { Status = "Success", Message = "Ticket Cancellation Status : " + ((Vastwebmulti.Areas.RETAILER.Enums.ChangeRequestStatus)ChangeRequestStatus).ToString() };
                            return Json(JsonConvert.SerializeObject(jsrespo));
                        }
                        else
                        {
                            var errMsg = Convert.ToString(respo.Content.ADDINFO.Error.ErrorMessage);
                            var jsrespo = new { Status = "Failed", Message = errMsg };
                            return Json(JsonConvert.SerializeObject(jsrespo));
                        }

                    }
                    else
                    {
                        var jsrespo = new { Status = "Failed", Message = "Unable to fatch data" };
                        return Json(JsonConvert.SerializeObject(jsrespo));
                    }
                }
            }
            catch
            {
                var jsrespo = new { Status = "Failed", Message = "Server Error", Result = "" };
                return Json(JsonConvert.SerializeObject(jsrespo));
            }
        }
        [HttpGet]
        [PermissioncheckingAttribute(servicename = "FLIGHT", permision = "Write")]
        public ActionResult ViewTicket(int idno, string firsName, string lastName)
        {
            try
            {
                var airoplanedetails = db.TBO_AirTicketingDetails.Where(a => a.idno == idno).FirstOrDefault();
                var RequestObject = new
                {
                    TraceId = airoplanedetails.TraceId,
                    PNR = airoplanedetails.PNR
                };
                var token = string.Empty;
                token = getAuthToken();
                if (string.IsNullOrWhiteSpace(token))
                {
                    var viewRespo = new { Status = "Failed", StatusCode = 0, Message = "Server Down!!" };
                    return Json(JsonConvert.SerializeObject(viewRespo), JsonRequestBehavior.AllowGet);
                }
                var client = new RestClient(VastbazaarBaseUrl + "api/Air/GetBookingDetails");
                var request = new RestRequest(Method.POST);
                request.RequestFormat = DataFormat.Json;
                request.AddHeader("authorization", "bearer " + token);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("content-type", "application/json");
                //request.AddParameter("application/json", "{\r\n\"EndUserIp\": \"192.168.10.10\",\r\n\"TokenId\": \"ac2751e9-4cc3-406f-b678-c947e4f57a00\",\r\n\"AdultCount\": \"1\",\r\n\"ChildCount\": \"0\",\r\n\"InfantCount\": \"0\",\r\n\"DirectFlight\": \"false\",\r\n\"OneStopFlight\": \"false\",\r\n\"JourneyType\": \"1\",\r\n\"PreferredAirlines\": null,\r\n\"Segments\": [\r\n{\r\n\"Origin\": \"DEL\",\r\n\"Destination\": \"BOM\",\r\n        \"FlightCabinClass\": \"1\",\r\n\"PreferredDepartureTime\": \"2018-09-06T00: 00: 00\",\r\n\"PreferredArrivalTime\": \"2018-09-06T00: 00: 00\"\r\n}\r\n        ],\r\n\"Sources\": [\r\n\"6E\"\r\n]\r\n}", ParameterType.RequestBody);
                request.AddBody(RequestObject);
                IRestResponse response = client.Execute(request);
                Vastwebmulti.Areas.RETAILER.ViewModels.GetFlightDetailsVM respo = JsonConvert.DeserializeObject<Vastwebmulti.Areas.RETAILER.ViewModels.GetFlightDetailsVM>(response.Content);
                if (respo.StatusCode == 200 && respo.Content != null && respo.Content.ResponseCode == 0 && respo.Content.ADDINFO.Error.ErrorCode == 0)
                {
                    var logo = db.tblHeaderLogoes.Where(p => p.Role == "ADMIN").ToList();
                    if (logo.Count > 0)
                    {
                        ViewBag.logoimage = logo.SingleOrDefault().LogoImage;
                    }
                    else
                    {
                        ViewBag.logoimage = "";
                    }
                    var ticket = db.TBO_AirTicketingDetails.Where(a => a.PNR == respo.Content.ADDINFO.FlightItinerary.Pnr).FirstOrDefault();
                    var admin = db.Admin_details.Single();
                    ViewBag.Admin = admin;
                    var retailer = db.Retailer_Details.Where(a => a.RetailerId == ticket.RetailerId).SingleOrDefault();
                    ViewBag.CreatedBy = retailer.Frm_Name ?? ".............................";
                    ViewBag.CreatorAddresss = retailer.Address ?? "............................";
                    ViewBag.CreatorState = db.State_Desc.FirstOrDefault(a => a.State_id == retailer.State).State_name;
                    ViewBag.CreatorDistrict = db.District_Desc.FirstOrDefault(d => d.Dist_id == retailer.District).Dist_Desc;
                    ViewBag.CreatorPIN = retailer.Pincode;
                    ViewBag.CreatorMobile = retailer.Mobile;
                    ViewBag.TraceId = airoplanedetails.TraceId;
                    ViewBag.idno = idno;
                    ViewBag.LeadPaxFirstName = firsName;
                    ViewBag.LeadPaxFirstName = lastName;
                    ViewBag.RetailerMarkup = db.TBO_AirTicketingDetails.Where(a => a.PNR == respo.Content.ADDINFO.FlightItinerary.Pnr).FirstOrDefault().RetailerMarkup;
                    ViewBag.BaggagePerPax = respo.Content.ADDINFO.FlightItinerary.Passenger.FirstOrDefault().SegmentAdditionalInfo.FirstOrDefault().Baggage;

                    return View(respo);
                }
                else
                {
                    TempData["Status"] = "Failed";
                    TempData["Message"] = "Failed to fatch Data";
                    return RedirectToAction("TicketReport", "Air");
                }

            }
            catch
            {
                TempData["Status"] = "Failed";
                TempData["Message"] = "Internal server error.";
                return RedirectToAction("TicketReport", "Air");
            }
        }
        [PermissioncheckingAttribute(servicename = "FLIGHT", permision = "Write")]
        public ActionResult PrintTicket(int idno, string firsName, string lastName)
        {
            try
            {
                var airoplanedetails = db.TBO_AirTicketingDetails.Where(a => a.idno == idno).FirstOrDefault();
                var RequestObject = new
                {
                    TraceId = airoplanedetails.TraceId,
                    PNR = airoplanedetails.PNR
                };
                var token = string.Empty;
                token = getAuthToken();
                if (string.IsNullOrWhiteSpace(token))
                {
                    var viewRespo = new { Status = "Failed", StatusCode = 0, Message = "Server Down!!" };
                    return Json(JsonConvert.SerializeObject(viewRespo), JsonRequestBehavior.AllowGet);
                }
                var client = new RestClient(VastbazaarBaseUrl + "api/Air/GetBookingDetails");
                var request = new RestRequest(Method.POST);
                request.RequestFormat = DataFormat.Json;
                request.AddHeader("authorization", "bearer " + token);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("content-type", "application/json");
                //request.AddParameter("application/json", "{\r\n\"EndUserIp\": \"192.168.10.10\",\r\n\"TokenId\": \"ac2751e9-4cc3-406f-b678-c947e4f57a00\",\r\n\"AdultCount\": \"1\",\r\n\"ChildCount\": \"0\",\r\n\"InfantCount\": \"0\",\r\n\"DirectFlight\": \"false\",\r\n\"OneStopFlight\": \"false\",\r\n\"JourneyType\": \"1\",\r\n\"PreferredAirlines\": null,\r\n\"Segments\": [\r\n{\r\n\"Origin\": \"DEL\",\r\n\"Destination\": \"BOM\",\r\n        \"FlightCabinClass\": \"1\",\r\n\"PreferredDepartureTime\": \"2018-09-06T00: 00: 00\",\r\n\"PreferredArrivalTime\": \"2018-09-06T00: 00: 00\"\r\n}\r\n        ],\r\n\"Sources\": [\r\n\"6E\"\r\n]\r\n}", ParameterType.RequestBody);
                request.AddBody(RequestObject);
                IRestResponse response = client.Execute(request);
                Vastwebmulti.Areas.RETAILER.ViewModels.GetFlightDetailsVM respo = JsonConvert.DeserializeObject<Vastwebmulti.Areas.RETAILER.ViewModels.GetFlightDetailsVM>(response.Content);
                if (respo.StatusCode == 200 && respo.Content != null && respo.Content.ResponseCode == 0 && respo.Content.ADDINFO.Error.ErrorCode == 0)
                {
                    var logo = db.tblHeaderLogoes.Where(p => p.Role == "ADMIN").ToList();
                    if (logo.Count > 0)
                    {
                        ViewBag.logoimage = logo.SingleOrDefault().LogoImage;
                    }
                    else
                    {
                        ViewBag.logoimage = "";
                    }
                    var ticket = db.TBO_AirTicketingDetails.Where(a => a.PNR == respo.Content.ADDINFO.FlightItinerary.Pnr).FirstOrDefault();
                    var admin = db.Admin_details.Single();
                    ViewBag.Admin = admin;
                    var retailer = db.Retailer_Details.Where(a => a.RetailerId == ticket.RetailerId).SingleOrDefault();
                    ViewBag.CreatedBy = retailer.Frm_Name ?? ".............................";
                    ViewBag.CreatorAddresss = retailer.Address ?? "............................";
                    ViewBag.CreatorState = db.State_Desc.FirstOrDefault(a => a.State_id == retailer.State).State_name;
                    ViewBag.CreatorDistrict = db.District_Desc.FirstOrDefault(d => d.Dist_id == retailer.District).Dist_Desc;
                    ViewBag.CreatorPIN = retailer.Pincode;
                    ViewBag.CreatorMobile = retailer.Mobile;
                    ViewBag.TraceId = airoplanedetails.TraceId;
                    ViewBag.idno = idno;
                    ViewBag.LeadPaxFirstName = firsName;
                    ViewBag.LeadPaxFirstName = lastName;
                    ViewBag.RetailerMarkup = db.TBO_AirTicketingDetails.Where(a => a.PNR == respo.Content.ADDINFO.FlightItinerary.Pnr).FirstOrDefault().RetailerMarkup;
                    ViewBag.BaggagePerPax = respo.Content.ADDINFO.FlightItinerary.Passenger.FirstOrDefault().SegmentAdditionalInfo.FirstOrDefault().Baggage;
                    return new ViewAsPdf("ViewTicket", respo);
                }
                else
                {
                    TempData["Status"] = "Failed";
                    TempData["Message"] = "Failed to fatch Data";
                    return RedirectToAction("TicketReport", "Air");
                }

            }
            catch
            {
                TempData["Status"] = "Failed";
                TempData["Message"] = "Internal server error.";
                return RedirectToAction("TicketReport", "Air");
            }
        }
        [HttpGet]
        [PermissioncheckingAttribute(servicename = "FLIGHT", permision = "Write")]
        public ActionResult ViewTicketWithoutFare(int idno, string firsName, string lastName)
        {
            try
            {
                var airoplanedetails = db.TBO_AirTicketingDetails.Where(a => a.idno == idno).FirstOrDefault();
                var RequestObject = new
                {
                    TraceId = airoplanedetails.TraceId,
                    PNR = airoplanedetails.PNR
                };
                var token = string.Empty;
                token = getAuthToken();
                if (string.IsNullOrWhiteSpace(token))
                {
                    var viewRespo = new { Status = "Failed", StatusCode = 0, Message = "Server Down!!" };
                    return Json(JsonConvert.SerializeObject(viewRespo), JsonRequestBehavior.AllowGet);
                }
                var client = new RestClient(VastbazaarBaseUrl + "api/Air/GetBookingDetails");
                var request = new RestRequest(Method.POST);
                request.RequestFormat = DataFormat.Json;
                request.AddHeader("authorization", "bearer " + token);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("content-type", "application/json");
                //request.AddParameter("application/json", "{\r\n\"EndUserIp\": \"192.168.10.10\",\r\n\"TokenId\": \"ac2751e9-4cc3-406f-b678-c947e4f57a00\",\r\n\"AdultCount\": \"1\",\r\n\"ChildCount\": \"0\",\r\n\"InfantCount\": \"0\",\r\n\"DirectFlight\": \"false\",\r\n\"OneStopFlight\": \"false\",\r\n\"JourneyType\": \"1\",\r\n\"PreferredAirlines\": null,\r\n\"Segments\": [\r\n{\r\n\"Origin\": \"DEL\",\r\n\"Destination\": \"BOM\",\r\n        \"FlightCabinClass\": \"1\",\r\n\"PreferredDepartureTime\": \"2018-09-06T00: 00: 00\",\r\n\"PreferredArrivalTime\": \"2018-09-06T00: 00: 00\"\r\n}\r\n        ],\r\n\"Sources\": [\r\n\"6E\"\r\n]\r\n}", ParameterType.RequestBody);
                request.AddBody(RequestObject);
                IRestResponse response = client.Execute(request);
                Vastwebmulti.Areas.RETAILER.ViewModels.GetFlightDetailsVM respo = JsonConvert.DeserializeObject<Vastwebmulti.Areas.RETAILER.ViewModels.GetFlightDetailsVM>(response.Content);
                if (respo.StatusCode == 200 && respo.Content != null && respo.Content.ResponseCode == 0 && respo.Content.ADDINFO.Error.ErrorCode == 0)
                {
                    var logo = db.tblHeaderLogoes.Where(p => p.Role == "ADMIN").ToList();
                    if (logo.Count > 0)
                    {
                        ViewBag.logoimage = logo.SingleOrDefault().LogoImage;
                    }
                    else
                    {
                        ViewBag.logoimage = "";
                    }
                    var ticket = db.TBO_AirTicketingDetails.Where(a => a.PNR == respo.Content.ADDINFO.FlightItinerary.Pnr).FirstOrDefault();
                    var admin = db.Admin_details.Single();
                    ViewBag.Admin = admin;
                    var retailer = db.Retailer_Details.Where(a => a.RetailerId == ticket.RetailerId).SingleOrDefault();
                    ViewBag.CreatedBy = retailer.Frm_Name ?? ".............................";
                    ViewBag.CreatorAddresss = retailer.Address ?? "............................";
                    ViewBag.CreatorState = db.State_Desc.FirstOrDefault(a => a.State_id == retailer.State).State_name;
                    ViewBag.CreatorDistrict = db.District_Desc.FirstOrDefault(d => d.Dist_id == retailer.District).Dist_Desc;
                    ViewBag.CreatorPIN = retailer.Pincode;
                    ViewBag.CreatorMobile = retailer.Mobile;
                    ViewBag.TraceId = airoplanedetails.TraceId;
                    ViewBag.idno = idno;
                    ViewBag.LeadPaxFirstName = firsName;
                    ViewBag.LeadPaxFirstName = lastName;
                    ViewBag.RetailerMarkup = db.TBO_AirTicketingDetails.Where(a => a.PNR == respo.Content.ADDINFO.FlightItinerary.Pnr).FirstOrDefault().RetailerMarkup;
                    ViewBag.BaggagePerPax = respo.Content.ADDINFO.FlightItinerary.Passenger.FirstOrDefault().SegmentAdditionalInfo.FirstOrDefault().Baggage;

                    return View(respo);
                }
                else
                {
                    TempData["Status"] = "Failed";
                    TempData["Message"] = "Failed to fatch Data";
                    return RedirectToAction("TicketReport", "Air");
                }

            }
            catch
            {
                TempData["Status"] = "Failed";
                TempData["Message"] = "Internal server error.";
                return RedirectToAction("TicketReport", "Air");
            }
        }
        [PermissioncheckingAttribute(servicename = "FLIGHT", permision = "Write")]
        public ActionResult PrintWithoutFare(int idno, string firsName, string lastName)
        {
            try
            {
                var airoplanedetails = db.TBO_AirTicketingDetails.Where(a => a.idno == idno).FirstOrDefault();
                var RequestObject = new
                {
                    TraceId = airoplanedetails.TraceId,
                    PNR = airoplanedetails.PNR
                };
                var token = string.Empty;
                token = getAuthToken();
                if (string.IsNullOrWhiteSpace(token))
                {
                    var viewRespo = new { Status = "Failed", StatusCode = 0, Message = "Server Down!!" };
                    return Json(JsonConvert.SerializeObject(viewRespo), JsonRequestBehavior.AllowGet);
                }
                var client = new RestClient(VastbazaarBaseUrl + "api/Air/GetBookingDetails");
                var request = new RestRequest(Method.POST);
                request.RequestFormat = DataFormat.Json;
                request.AddHeader("authorization", "bearer " + token);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("content-type", "application/json");
                //request.AddParameter("application/json", "{\r\n\"EndUserIp\": \"192.168.10.10\",\r\n\"TokenId\": \"ac2751e9-4cc3-406f-b678-c947e4f57a00\",\r\n\"AdultCount\": \"1\",\r\n\"ChildCount\": \"0\",\r\n\"InfantCount\": \"0\",\r\n\"DirectFlight\": \"false\",\r\n\"OneStopFlight\": \"false\",\r\n\"JourneyType\": \"1\",\r\n\"PreferredAirlines\": null,\r\n\"Segments\": [\r\n{\r\n\"Origin\": \"DEL\",\r\n\"Destination\": \"BOM\",\r\n        \"FlightCabinClass\": \"1\",\r\n\"PreferredDepartureTime\": \"2018-09-06T00: 00: 00\",\r\n\"PreferredArrivalTime\": \"2018-09-06T00: 00: 00\"\r\n}\r\n        ],\r\n\"Sources\": [\r\n\"6E\"\r\n]\r\n}", ParameterType.RequestBody);
                request.AddBody(RequestObject);
                IRestResponse response = client.Execute(request);
                Vastwebmulti.Areas.RETAILER.ViewModels.GetFlightDetailsVM respo = JsonConvert.DeserializeObject<Vastwebmulti.Areas.RETAILER.ViewModels.GetFlightDetailsVM>(response.Content);
                if (respo.StatusCode == 200 && respo.Content != null && respo.Content.ResponseCode == 0 && respo.Content.ADDINFO.Error.ErrorCode == 0)
                {
                    var logo = db.tblHeaderLogoes.Where(p => p.Role == "ADMIN").ToList();
                    if (logo.Count > 0)
                    {
                        ViewBag.logoimage = logo.SingleOrDefault().LogoImage;
                    }
                    else
                    {
                        ViewBag.logoimage = "";
                    }
                    var ticket = db.TBO_AirTicketingDetails.Where(a => a.PNR == respo.Content.ADDINFO.FlightItinerary.Pnr).FirstOrDefault();
                    var admin = db.Admin_details.Single();
                    ViewBag.Admin = admin;
                    var retailer = db.Retailer_Details.Where(a => a.RetailerId == ticket.RetailerId).SingleOrDefault();
                    ViewBag.CreatedBy = retailer.Frm_Name ?? ".............................";
                    ViewBag.CreatorAddresss = retailer.Address ?? "............................";
                    ViewBag.CreatorState = db.State_Desc.FirstOrDefault(a => a.State_id == retailer.State).State_name;
                    ViewBag.CreatorDistrict = db.District_Desc.FirstOrDefault(d => d.Dist_id == retailer.District).Dist_Desc;
                    ViewBag.CreatorPIN = retailer.Pincode;
                    ViewBag.CreatorMobile = retailer.Mobile;
                    ViewBag.TraceId = airoplanedetails.TraceId;
                    ViewBag.idno = idno;
                    ViewBag.LeadPaxFirstName = firsName;
                    ViewBag.LeadPaxFirstName = lastName;
                    ViewBag.RetailerMarkup = db.TBO_AirTicketingDetails.Where(a => a.PNR == respo.Content.ADDINFO.FlightItinerary.Pnr).FirstOrDefault().RetailerMarkup;
                    ViewBag.BaggagePerPax = respo.Content.ADDINFO.FlightItinerary.Passenger.FirstOrDefault().SegmentAdditionalInfo.FirstOrDefault().Baggage;
                    return new ViewAsPdf("PrintWithoutFare", respo);
                }
                else
                {
                    TempData["Status"] = "Failed";
                    TempData["Message"] = "Failed to fatch Data";
                    return RedirectToAction("TicketReport", "Air");
                }

            }
            catch
            {
                TempData["Status"] = "Failed";
                TempData["Message"] = "Internal server error.";
                return RedirectToAction("TicketReport", "Air");
            }
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
            request.AddParameter("application/x-www-form-urlencoded", "UserName=" + apiid + "&Password=" + HttpUtility.UrlEncode(apiidpwd) + "&grant_type=password", ParameterType.RequestBody);
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
                        Vastwebmulti.Areas.RETAILER.Models.Vastbillpay vb = new Vastwebmulti.Areas.RETAILER.Models.Vastbillpay();
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


        #endregion Airticket

        #region BusBooking
        [PermissioncheckingAttribute(servicename = "BUS", permision = "Read")]
        public ActionResult BusBookingReport()
        {
            try
            {
                var userid = User.Identity.GetUserId();
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    // show master id 
                    var stands = db.Superstokist_details.ToList();
                    IEnumerable<SelectListItem> selectList = from s in stands
                                                             select new SelectListItem
                                                             {
                                                                 Value = s.SSId,
                                                                 Text = s.FarmName.ToString()
                                                             };
                    ViewBag.allmaster = new SelectList(selectList, "Value", "Text");
                    //show whitelabel
                    var totalwhitelabel = db.WhiteLabel_userList.ToList();
                    IEnumerable<SelectListItem> selectList1 = from s in totalwhitelabel
                                                              select new SelectListItem
                                                              {
                                                                  Value = s.WhiteLabelID,
                                                                  Text = s.FrmName.ToString()
                                                              };
                    ViewBag.allwhitelabel = new SelectList(selectList1, "Value", "Text");
                    //show dealer
                    ViewBag.alldealer = new SelectList(db.select_Dealer_for_ddl(), "Dealerid", "FarmName", null).ToList();
                    //Retailer 
                    ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl("Admin"), "RetailerId", "Frm_Name", null).ToList();

                    string txt_frm_date = DateTime.Now.ToString();
                    string txt_to_date = DateTime.Now.ToString();
                    var frm_date = Convert.ToDateTime(txt_frm_date).Date;
                    var to_date = Convert.ToDateTime(txt_to_date).AddDays(1);

                    var proc_Response = db.proc_BusReport(1, 20, null, null, null, null, null, null, null, null, null, frm_date, to_date).ToList();
                    ViewData["Totalofferfare"] = proc_Response.Where(s => s.TicketStatus.ToUpper().Contains("CONFIRMED")).Sum(s => Convert.ToInt32(s.FareAmount));
                    ViewData["totalf"] = proc_Response.Where(s => s.TicketStatus.ToUpper().Contains("FAILED")).Sum(s => Convert.ToInt32(s.FareAmount));
                    ViewData["totalp"] = proc_Response.Where(s => s.TicketStatus.ToUpper().Contains("PROCCESSED")).Sum(s => Convert.ToInt32(s.FareAmount));
                    return View(proc_Response);
                }
            }

            catch
            {
                TempData["Status"] = "Failed";
                TempData["Message"] = "Something went wrong,please later!";
                return RedirectToAction("Travel", "Home");
            }
        }
        [HttpPost]
        [PermissioncheckingAttribute(servicename = "BUS", permision = "Read")]
        public ActionResult BusBookingReport(string ddl_status, int? ddl_top, string txt_frm_date, string txt_to_date, string ddlusers, string allmaster, string alldealer, string allretailer, string allwhitelabel, string TicketNo)
        {
            try
            {
                ViewBag.chk = "post";
                string retailerid = null;
                string DealerId = null;
                string MasterId = null;
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    // show master id 
                    var stands = db.Superstokist_details.ToList();
                    IEnumerable<SelectListItem> selectList = from s in stands
                                                             select new SelectListItem
                                                             {
                                                                 Value = s.SSId,
                                                                 Text = s.FarmName.ToString()
                                                             };
                    ViewBag.allmaster = new SelectList(selectList, "Value", "Text");
                    //show whitelabel
                    var totalwhitelabel = db.WhiteLabel_userList.ToList();
                    IEnumerable<SelectListItem> selectList1 = from s in totalwhitelabel
                                                              select new SelectListItem
                                                              {
                                                                  Value = s.WhiteLabelID,
                                                                  Text = s.FrmName.ToString()
                                                              };
                    ViewBag.allwhitelabel = new SelectList(selectList1, "Value", "Text");
                    //show dealer
                    ViewBag.alldealer = new SelectList(db.select_Dealer_for_ddl(), "Dealerid", "FarmName", null).ToList();
                    //Retailer 
                    ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl("Admin"), "RetailerId", "Frm_Name", null).ToList();


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
                    TicketNo = string.IsNullOrWhiteSpace(TicketNo) ? null : TicketNo;
                    ddl_status = string.IsNullOrWhiteSpace(ddl_status) ? null : ddl_status;
                    if (ddlusers == "Master")
                    {
                        if (allmaster == "" || allmaster.Contains("All Master") || allmaster == null)
                        {
                            MasterId = null;
                        }
                        else
                        {
                            MasterId = allmaster;
                        }
                    }
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
                    if (ddlusers == "Admin")
                    {
                        retailerid = null;
                        DealerId = null;
                        MasterId = null;
                    }

                    var proc_Response = db.proc_BusReport(1, 50, ddl_status, retailerid, DealerId, MasterId, null, TicketNo, null, null, null, frm_date, to_date).ToList();
                    ViewData["Totalofferfare"] = proc_Response.Where(s => s.TicketStatus.ToUpper().Contains("CONFIRMED")).Sum(s => Convert.ToInt32(s.FareAmount));
                    ViewData["totalf"] = proc_Response.Where(s => s.TicketStatus.ToUpper().Contains("FAILED")).Sum(s => Convert.ToInt32(s.FareAmount));
                    ViewData["totalp"] = proc_Response.Where(s => s.TicketStatus.ToUpper().Contains("PROCCESSED")).Sum(s => Convert.ToInt32(s.FareAmount));
                    return View(proc_Response);
                }

            }
            catch (Exception ex)
            {
                TempData["Status"] = "Failed";
                TempData["Message"] = "An error occured while proccessing request.";
                return RedirectToAction("Travel", "Home");
            }
        }

        public ActionResult ExcelBusBookingReport(string ddl_status, int? ddl_top, string txt_frm_date, string txt_to_date, string ddlusers, string allmaster, string alldealer, string allretailer, string allwhitelabel, string TicketNo)
        {

            try
            {
                ViewBag.chk = "post";
                string retailerid = null;
                string DealerId = null;
                string MasterId = null;
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    // show master id 
                    var stands = db.Superstokist_details.ToList();
                    IEnumerable<SelectListItem> selectList = from s in stands
                                                             select new SelectListItem
                                                             {
                                                                 Value = s.SSId,
                                                                 Text = s.FarmName.ToString()
                                                             };
                    ViewBag.allmaster = new SelectList(selectList, "Value", "Text");
                    //show whitelabel
                    var totalwhitelabel = db.WhiteLabel_userList.ToList();
                    IEnumerable<SelectListItem> selectList1 = from s in totalwhitelabel
                                                              select new SelectListItem
                                                              {
                                                                  Value = s.WhiteLabelID,
                                                                  Text = s.FrmName.ToString()
                                                              };
                    ViewBag.allwhitelabel = new SelectList(selectList1, "Value", "Text");
                    //show dealer
                    ViewBag.alldealer = new SelectList(db.select_Dealer_for_ddl(), "Dealerid", "FarmName", null).ToList();
                    //Retailer 
                    ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl("Admin"), "RetailerId", "Frm_Name", null).ToList();


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
                    TicketNo = string.IsNullOrWhiteSpace(TicketNo) ? null : TicketNo;
                    ddl_status = string.IsNullOrWhiteSpace(ddl_status) ? null : ddl_status;
                    if (ddlusers == "Master")
                    {
                        if (allmaster == "" || allmaster.Contains("All Master") || allmaster == null)
                        {
                            MasterId = null;
                        }
                        else
                        {
                            MasterId = allmaster;
                        }
                    }
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
                    if (ddlusers == "Admin")
                    {
                        retailerid = null;
                        DealerId = null;
                        MasterId = null;
                    }
                    //if (TicketNo.ToUpper() == "UNDEFINED")
                    //{
                    //    TicketNo = null;
                    //}
                    var proc_Response = db.proc_BusReport(1, 50, ddl_status, retailerid, DealerId, MasterId, null, TicketNo, null, null, null, frm_date, to_date).ToList();
                    DataTable dataTbl = new DataTable();
                    dataTbl.Columns.Add("Ticket Status", typeof(string));
                    dataTbl.Columns.Add("Firm Name", typeof(string));
                    dataTbl.Columns.Add("Passanger Name", typeof(string));
                    dataTbl.Columns.Add("Source (From)", typeof(string));
                    dataTbl.Columns.Add("Destination (To)", typeof(string));
                    dataTbl.Columns.Add("Date Of Journey", typeof(string));
                    dataTbl.Columns.Add("Fare", typeof(string));
                    dataTbl.Columns.Add("Retailer Pre", typeof(string));
                    dataTbl.Columns.Add("Retailer Post", typeof(string));
                    dataTbl.Columns.Add("PNR", typeof(string));
                    dataTbl.Columns.Add("Booking Time", typeof(string));


                    if (proc_Response.Count > 0)
                    {


                        foreach (var item in proc_Response)
                        {
                            dataTbl.Rows.Add(item.TicketStatus, item.Frm_Name, item.PassengerName, item.sourceStationName, item.destinationStationName, item.dateOfJourney, item.FareAmount, item.RemPre, item.RemPost, item.PNR, item.TicketDate);
                        }
                    }
                    else
                    {
                        dataTbl.Rows.Add("", "", "", "", "", "", "", "", "", "", "");
                    }

                    var grid = new GridView();
                    grid.DataSource = dataTbl;
                    grid.DataBind();
                    Response.ClearContent();
                    Response.Buffer = true;
                    Response.AddHeader("content-disposition", "attachment; filename=Bus_Booking_Report.xls");
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
            catch (Exception ex)
            {
                TempData["Status"] = "Failed";
                TempData["Message"] = "An error occured while proccessing request.";
                return RedirectToAction("BusBookingReport", "Home");
            }



        }
        public ActionResult PDFBusBookingReport(string ddl_status, int? ddl_top, string txt_frm_date, string txt_to_date, string ddlusers, string allmaster, string alldealer, string allretailer, string allwhitelabel, string TicketNo)
        {
            try
            {
                ViewBag.chk = "post";
                string retailerid = null;
                string DealerId = null;
                string MasterId = null;
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    // show master id 
                    var stands = db.Superstokist_details.ToList();
                    IEnumerable<SelectListItem> selectList = from s in stands
                                                             select new SelectListItem
                                                             {
                                                                 Value = s.SSId,
                                                                 Text = s.FarmName.ToString()
                                                             };
                    ViewBag.allmaster = new SelectList(selectList, "Value", "Text");
                    //show whitelabel
                    var totalwhitelabel = db.WhiteLabel_userList.ToList();
                    IEnumerable<SelectListItem> selectList1 = from s in totalwhitelabel
                                                              select new SelectListItem
                                                              {
                                                                  Value = s.WhiteLabelID,
                                                                  Text = s.FrmName.ToString()
                                                              };
                    ViewBag.allwhitelabel = new SelectList(selectList1, "Value", "Text");
                    //show dealer
                    ViewBag.alldealer = new SelectList(db.select_Dealer_for_ddl(), "Dealerid", "FarmName", null).ToList();
                    //Retailer 
                    ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl("Admin"), "RetailerId", "Frm_Name", null).ToList();


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
                    TicketNo = string.IsNullOrWhiteSpace(TicketNo) ? null : TicketNo;
                    ddl_status = string.IsNullOrWhiteSpace(ddl_status) ? null : ddl_status;
                    if (ddlusers == "Master")
                    {
                        if (allmaster == "" || allmaster.Contains("All Master") || allmaster == null)
                        {
                            MasterId = null;
                        }
                        else
                        {
                            MasterId = allmaster;
                        }
                    }
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
                    if (ddlusers == "Admin")
                    {
                        retailerid = null;
                        DealerId = null;
                        MasterId = null;
                    }

                    var proc_Response = db.proc_BusReport(1, 50, ddl_status, retailerid, DealerId, MasterId, null, TicketNo, null, null, null, frm_date, to_date).ToList();
                    ViewData["Totalofferfare"] = proc_Response.Where(s => s.TicketStatus.ToUpper().Contains("CONFIRMED")).Sum(s => Convert.ToInt32(s.FareAmount));
                    ViewData["totalf"] = proc_Response.Where(s => s.TicketStatus.ToUpper().Contains("FAILED")).Sum(s => Convert.ToInt32(s.FareAmount));
                    ViewData["totalp"] = proc_Response.Where(s => s.TicketStatus.ToUpper().Contains("PROCCESSED")).Sum(s => Convert.ToInt32(s.FareAmount));
                    return new ViewAsPdf(proc_Response);
                }

            }
            catch (Exception ex)
            {
                TempData["Status"] = "Failed";
                TempData["Message"] = "An error occured while proccessing request.";
                return RedirectToAction("Travel", "Home");
            }
        }

        [HttpPost]
        [PermissioncheckingAttribute(servicename = "BUS", permision = "Write")]
        public ActionResult getBookingDetails(string TraceId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(TraceId))
                {
                    var ajaxRespo = new { Status = "Failed", Message = "Invalid request Id." };
                    return Json(JsonConvert.SerializeObject(ajaxRespo));
                }
                var token = string.Empty;
                token = getAuthToken();
                if (string.IsNullOrWhiteSpace(token))
                {
                    UpdateAuthToken();
                    var ajaxRespo = new { Status = "Failed", Message = "Failed to connect with provider.please try again." };
                    return Json(JsonConvert.SerializeObject(ajaxRespo));
                }
                var model = new
                {
                    TraceId = TraceId
                };
                var client = new RestClient(VastbazaarBaseUrl + "api/Bus/GetBookingDetails");
                var request = new RestRequest(Method.POST);
                request.RequestFormat = DataFormat.Json;
                request.AddHeader("authorization", "bearer " + token);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("content-type", "application/json");
                request.AddHeader("accept-encoding", "gzip");
                request.AddBody(model);

                Task<IRestResponse> task = Task.Run(() =>
                {
                    return client.Execute(request);
                });
                bool isCompletedSuccessfully = task.Wait(TimeSpan.FromMilliseconds(120000));
                if (isCompletedSuccessfully)
                {
                    if (task.Result.StatusCode == HttpStatusCode.BadRequest || task.Result.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        UpdateAuthToken();
                        var ajaxRespo = new { Status = "Failed", Message = "Unauthorized Request." };
                        return Json(JsonConvert.SerializeObject(ajaxRespo));
                    }
                    dynamic respo = JsonConvert.DeserializeObject(task.Result.Content);

                    if (string.IsNullOrWhiteSpace(task.Result.Content))
                    {
                        var ajaxRespo = new { Status = "Failed", Message = "Server is down,please try later." };
                        return Json(JsonConvert.SerializeObject(ajaxRespo));
                    }
                    if (respo.Content.ADDINFO.Error.ErrorCode != 0)
                    {
                        var ajaxRespo = new { Status = "Failed", Message = respo.Content.ADDINFO.Error.ErrorMessage };
                        return Json(JsonConvert.SerializeObject(ajaxRespo));
                    }
                    else
                    {
                        var ajaxRespo = new { Status = "Success", Message = respo.Content.ADDINFO };
                        return Json(JsonConvert.SerializeObject(ajaxRespo));
                    }
                }
                else
                {
                    var ajaxRespo = new { Status = "Failed", Message = "Server is down,please try later." };
                    return Json(JsonConvert.SerializeObject(ajaxRespo));
                }
            }
            catch
            {
                var ajaxRespo = new { Status = "Failed", Message = "Server is down,please try later." };
                return Json(JsonConvert.SerializeObject(ajaxRespo));
            }
        }


        #endregion BusBooking

        #region HotelBooking kar bahi


        //[MenuAccessFilter] //used in paid and nonpaid services

        [PermissioncheckingAttribute(servicename = "HOTEL", permision = "Read")]
        public ActionResult HotelReport()
        {
            try
            {
                var userid = User.Identity.GetUserId();
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    var stands = db.Superstokist_details.ToList();
                    IEnumerable<SelectListItem> selectList = from s in stands
                                                             select new SelectListItem
                                                             {
                                                                 Value = s.SSId,
                                                                 Text = s.FarmName.ToString()
                                                             };

                    var totalwhitelabel = db.WhiteLabel_userList.ToList();
                    IEnumerable<SelectListItem> selectList1 = from s in totalwhitelabel
                                                              select new SelectListItem
                                                              {
                                                                  Value = s.WhiteLabelID,
                                                                  Text = s.FrmName.ToString()
                                                              };
                    ViewBag.allwhitelabel = new SelectList(selectList1, "Value", "Text");

                    ViewBag.allmaster = new SelectList(selectList, "Value", "Text");
                    ViewBag.alldealer = new SelectList(db.select_Dealer_for_ddl(), "Dealerid", "FarmName", null).ToList();
                    ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl("Admin"), "RetailerId", "Frm_Name", null).ToList();
                    ViewBag.API = new SelectList(db.API_all_apiid(), "apiid", "farmname");
                    ViewBag.whitelabel = new SelectList(db.Whitelabel_all_whitelabelid(), "WhiteLabelID", "FrmName");



                    string txt_frm_date = DateTime.Now.ToString();
                    string txt_to_date = DateTime.Now.ToString();
                    var frm_date = Convert.ToDateTime(txt_frm_date).Date;
                    var to_date = Convert.ToDateTime(txt_to_date).AddDays(1);
                    var ch = db.proc_HotelReport_new(frm_date, to_date, "", "", "Admin", "").ToList();
                    return View(ch);
                }
            }


            catch
            {
                TempData["Status"] = "Failed";
                TempData["Message"] = "An error occured while proccessing request.";
                return RedirectToAction("Travel", "Home");
            }
        }
        [HttpPost]
        [PermissioncheckingAttribute(servicename = "HOTEL", permision = "Read")]
        public ActionResult HotelReport(string ddl_status, string txt_frm_date, string txt_to_date, string ddlusers, string allmaster, string alldealer, string allretailer, string ddl_status_ticket)
        {
            try
            {
                ViewBag.chk = "post";
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    // show master id 
                    var stands = db.Superstokist_details.ToList();
                    IEnumerable<SelectListItem> selectList = from s in stands
                                                             select new SelectListItem
                                                             {
                                                                 Value = s.SSId,
                                                                 Text = s.FarmName.ToString()
                                                             };

                    var totalwhitelabel = db.WhiteLabel_userList.ToList();
                    IEnumerable<SelectListItem> selectList1 = from s in totalwhitelabel
                                                              select new SelectListItem
                                                              {
                                                                  Value = s.WhiteLabelID,
                                                                  Text = s.FrmName.ToString()
                                                              };
                    ViewBag.allwhitelabel = new SelectList(selectList1, "Value", "Text");

                    ViewBag.allmaster = new SelectList(selectList, "Value", "Text");
                    ViewBag.alldealer = new SelectList(db.select_Dealer_for_ddl(), "Dealerid", "FarmName", null).ToList();
                    ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl("Admin"), "RetailerId", "Frm_Name", null).ToList();
                    ViewBag.API = new SelectList(db.API_all_apiid(), "apiid", "farmname");
                    ViewBag.whitelabel = new SelectList(db.Whitelabel_all_whitelabelid(), "WhiteLabelID", "FrmName");

                    DateTime frm = Convert.ToDateTime(txt_frm_date);
                    DateTime to = Convert.ToDateTime(txt_to_date);
                    txt_frm_date = frm.ToString("dd-MM-yyyy");
                    txt_to_date = to.ToString("dd-MM-yyyy");
                    string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
                    DateTime dt = DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
                    DateTime dt1 = DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
                    DateTime frm_date = dt.Date;
                    DateTime to_date = dt1.AddDays(1);
                    var userid = "";
                    if (ddlusers == "Master")
                    {
                        userid = allmaster;
                    }
                    else if (ddlusers == "Distributor")
                    {
                        userid = alldealer;
                    }
                    else if (ddlusers == "Retailer")
                    {
                        userid = allretailer;
                    }

                    var ch = db.proc_HotelReport_new(frm_date, to_date, ddl_status, ddl_status_ticket, ddlusers, userid).ToList();
                    return View(ch);
                }

            }
            catch
            {
                TempData["Status"] = "Failed";
                TempData["Message"] = "An error occured while proccessing request.";
                return RedirectToAction("Travel", "Home");
            }
        }
        public ActionResult ExcelHotelBookingReport(string ddl_status, string txt_frm_date, string txt_to_date, string ddlusers, string allmaster, string alldealer, string allretailer, string ddl_status_ticket)
        {
            try
            {
                ViewBag.chk = "post";
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    // show master id 
                    var stands = db.Superstokist_details.ToList();
                    IEnumerable<SelectListItem> selectList = from s in stands
                                                             select new SelectListItem
                                                             {
                                                                 Value = s.SSId,
                                                                 Text = s.FarmName.ToString()
                                                             };

                    var totalwhitelabel = db.WhiteLabel_userList.ToList();
                    IEnumerable<SelectListItem> selectList1 = from s in totalwhitelabel
                                                              select new SelectListItem
                                                              {
                                                                  Value = s.WhiteLabelID,
                                                                  Text = s.FrmName.ToString()
                                                              };
                    ViewBag.allwhitelabel = new SelectList(selectList1, "Value", "Text");

                    ViewBag.allmaster = new SelectList(selectList, "Value", "Text");
                    ViewBag.alldealer = new SelectList(db.select_Dealer_for_ddl(), "Dealerid", "FarmName", null).ToList();
                    ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl("Admin"), "RetailerId", "Frm_Name", null).ToList();
                    ViewBag.API = new SelectList(db.API_all_apiid(), "apiid", "farmname");
                    ViewBag.whitelabel = new SelectList(db.Whitelabel_all_whitelabelid(), "WhiteLabelID", "FrmName");

                    DateTime frm = Convert.ToDateTime(txt_frm_date);
                    DateTime to = Convert.ToDateTime(txt_to_date);
                    txt_frm_date = frm.ToString("dd-MM-yyyy");
                    txt_to_date = to.ToString("dd-MM-yyyy");
                    string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
                    DateTime dt = DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
                    DateTime dt1 = DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
                    DateTime frm_date = dt.Date;
                    DateTime to_date = dt1.AddDays(1);
                    var userid = "";
                    if (ddlusers == "Master")
                    {
                        userid = allmaster;
                    }
                    else if (ddlusers == "Distributor")
                    {
                        userid = alldealer;
                    }
                    else if (ddlusers == "Retailer")
                    {
                        userid = allretailer;
                    }

                    var ch = db.proc_HotelReport_new(frm_date, to_date, ddl_status, ddl_status_ticket, ddlusers, userid).ToList();
                    DataTable dataTbl = new DataTable();
                    dataTbl.Columns.Add("Status", typeof(string));
                    dataTbl.Columns.Add("Frm Name", typeof(string));
                    dataTbl.Columns.Add("Hotel Information", typeof(string));
                    dataTbl.Columns.Add("Rooms", typeof(string));
                    dataTbl.Columns.Add("Booking Date", typeof(string));
                    dataTbl.Columns.Add("Chek In Date", typeof(string));
                    dataTbl.Columns.Add("Publish Fare", typeof(string));
                    dataTbl.Columns.Add("Price", typeof(string));
                    dataTbl.Columns.Add("Pre(Rs)", typeof(string));
                    dataTbl.Columns.Add("Debit(Rs)", typeof(string));
                    dataTbl.Columns.Add("Post(Rs)", typeof(string));
                    dataTbl.Columns.Add("Total Debit", typeof(string));
                    dataTbl.Columns.Add("Booking Id", typeof(string));


                    if (ch.Count > 0)
                    {


                        foreach (var item in ch)
                        {
                            dataTbl.Rows.Add(item.TicketStatus, item.Frm_Name, item.HotelName, item.NoOfRooms, item.responsedate, item.checkindate, item.totalpublishfare, item.totalOfferedFare, item.rempre, item.reminc, item.rempost, item.reminc, item.BookingId);
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
                    Response.AddHeader("content-disposition", "attachment; filename=Hotel_Booking_Report.xls");
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
            catch
            {
                TempData["Status"] = "Failed";
                TempData["Message"] = "An error occured while proccessing request.";
                return RedirectToAction("HotelReport", "Home");
            }
        }

        public ActionResult PDFHotelReport(string ddl_status, string txt_frm_date, string txt_to_date, string ddlusers, string allmaster, string alldealer, string allretailer, string ddl_status_ticket)
        {
            try
            {
                ViewBag.chk = "post";
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    // show master id 
                    var stands = db.Superstokist_details.ToList();
                    IEnumerable<SelectListItem> selectList = from s in stands
                                                             select new SelectListItem
                                                             {
                                                                 Value = s.SSId,
                                                                 Text = s.FarmName.ToString()
                                                             };

                    var totalwhitelabel = db.WhiteLabel_userList.ToList();
                    IEnumerable<SelectListItem> selectList1 = from s in totalwhitelabel
                                                              select new SelectListItem
                                                              {
                                                                  Value = s.WhiteLabelID,
                                                                  Text = s.FrmName.ToString()
                                                              };
                    ViewBag.allwhitelabel = new SelectList(selectList1, "Value", "Text");

                    ViewBag.allmaster = new SelectList(selectList, "Value", "Text");
                    ViewBag.alldealer = new SelectList(db.select_Dealer_for_ddl(), "Dealerid", "FarmName", null).ToList();
                    ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl("Admin"), "RetailerId", "Frm_Name", null).ToList();
                    ViewBag.API = new SelectList(db.API_all_apiid(), "apiid", "farmname");
                    ViewBag.whitelabel = new SelectList(db.Whitelabel_all_whitelabelid(), "WhiteLabelID", "FrmName");

                    DateTime frm = Convert.ToDateTime(txt_frm_date);
                    DateTime to = Convert.ToDateTime(txt_to_date);
                    txt_frm_date = frm.ToString("dd-MM-yyyy");
                    txt_to_date = to.ToString("dd-MM-yyyy");
                    string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
                    DateTime dt = DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
                    DateTime dt1 = DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
                    DateTime frm_date = dt.Date;
                    DateTime to_date = dt1.AddDays(1);
                    var userid = "";
                    if (ddlusers == "Master")
                    {
                        userid = allmaster;
                    }
                    else if (ddlusers == "Distributor")
                    {
                        userid = alldealer;
                    }
                    else if (ddlusers == "Retailer")
                    {
                        userid = allretailer;
                    }

                    var ch = db.proc_HotelReport_new(frm_date, to_date, ddl_status, ddl_status_ticket, ddlusers, userid).ToList();
                    return new ViewAsPdf(ch);
                }

            }
            catch
            {
                TempData["Status"] = "Failed";
                TempData["Message"] = "An error occured while proccessing request.";
                return RedirectToAction("Travel", "Home");
            }
        }

        [HttpGet]
        [PermissioncheckingAttribute(servicename = "HOTEL", permision = "Read")]
        public ActionResult CancellationQueue()
        {

            try
            {
                var userid = User.Identity.GetUserId();
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    // show master id 
                    var stands = db.Superstokist_details.ToList();
                    IEnumerable<SelectListItem> selectList = from s in stands
                                                             select new SelectListItem
                                                             {
                                                                 Value = s.SSId,
                                                                 Text = s.Email + "--" + s.SuperstokistName.ToString()
                                                             };
                    ViewBag.allmaster = new SelectList(selectList, "Value", "Text");
                    //show whitelabel
                    var totalwhitelabel = db.WhiteLabel_userList.ToList();
                    IEnumerable<SelectListItem> selectList1 = from s in totalwhitelabel
                                                              select new SelectListItem
                                                              {
                                                                  Value = s.WhiteLabelID,
                                                                  Text = s.EmailId + "--" + s.Name.ToString()
                                                              };
                    ViewBag.allwhitelabel = new SelectList(selectList1, "Value", "Text");
                    //show dealer
                    ViewBag.alldealer = new SelectList(db.select_Dealer_for_ddl(), "Dealerid", "DealerName", null).ToList();
                    //Retailer 
                    ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl("Admin"), "RetailerId", "RetailerName", null).ToList();
                    string txt_frm_date = DateTime.Now.ToString();
                    string txt_to_date = DateTime.Now.ToString();
                    var frm_date = Convert.ToDateTime(txt_frm_date).Date;
                    var to_date = Convert.ToDateTime(txt_to_date).AddDays(1);

                    var proc_Response = db.proc_HotelCancellationReport(50, null, null, null, null, null, null, null, null, null, frm_date, to_date).ToList();
                    //ViewData["Totalofferfare"] = proc_Response.Where(s => s.TicketStatus.ToUpper().Contains("Success")).Sum(s => Convert.ToInt32(s.totalOfferedFare));
                    //ViewData["totalf"] = proc_Response.Where(s => s.TicketStatus.ToUpper().Contains("FAILED")).Sum(s => Convert.ToInt32(s.totalOfferedFare));
                    //ViewData["totalp"] = proc_Response.Where(s => s.TicketStatus.ToUpper().Contains("Proccessed")).Sum(s => Convert.ToInt32(s.totalOfferedFare));
                    return View(proc_Response);
                }
            }


            catch
            {
                TempData["Status"] = "Failed";
                TempData["Message"] = "An error occured while proccessing request.";
                return RedirectToAction("Travel", "Home");
            }
        }

        [HttpPost]
        [PermissioncheckingAttribute(servicename = "HOTEL", permision = "Read")]
        public ActionResult CancellationQueue(string ddl_status, int? ddl_top, string txt_frm_date, string txt_to_date, string ddlusers, string allmaster, string alldealer, string allretailer, string allwhitelabel, string BookingId)
        {
            try
            {
                ViewBag.chk = "post";
                string retailerid = null;
                string DealerId = null;
                string MasterId = null;
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    // show master id 
                    var stands = db.Superstokist_details.ToList();
                    IEnumerable<SelectListItem> selectList = from s in stands
                                                             select new SelectListItem
                                                             {
                                                                 Value = s.SSId,
                                                                 Text = s.Email + "--" + s.SuperstokistName.ToString()
                                                             };
                    ViewBag.allmaster = new SelectList(selectList, "Value", "Text");
                    //show whitelabel
                    var totalwhitelabel = db.WhiteLabel_userList.ToList();
                    IEnumerable<SelectListItem> selectList1 = from s in totalwhitelabel
                                                              select new SelectListItem
                                                              {
                                                                  Value = s.WhiteLabelID,
                                                                  Text = s.EmailId + "--" + s.Name.ToString()
                                                              };
                    ViewBag.allwhitelabel = new SelectList(selectList1, "Value", "Text");
                    //show dealer
                    ViewBag.alldealer = new SelectList(db.select_Dealer_for_ddl(), "Dealerid", "DealerName", null).ToList();
                    //Retailer 
                    ViewBag.allretailer = new SelectList(db.select_retailer_for_ddl("Admin"), "RetailerId", "RetailerName", null).ToList();

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
                    if (ddlusers == "Master")
                    {
                        if (allmaster == "" || allmaster.Contains("All Master") || allmaster == null)
                        {
                            MasterId = null;
                        }
                        else
                        {
                            MasterId = allmaster;
                        }
                    }
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
                    if (ddlusers == "Admin")
                    {
                        retailerid = null;
                        DealerId = null;
                        MasterId = null;
                    }
                    var proc_Response = db.proc_HotelCancellationReport(ddl_top, ddl_status, retailerid, DealerId, MasterId, null, null, BookingId, null, null, frm_date, to_date).ToList();
                    //ViewData["Totalofferfare"] = proc_Response.Where(s => s.TicketStatus.ToUpper().Contains("Success")).Sum(s => Convert.ToInt32(s.totalOfferedFare));
                    //ViewData["totalf"] = proc_Response.Where(s => s.TicketStatus.ToUpper().Contains("FAILED")).Sum(s => Convert.ToInt32(s.totalOfferedFare));
                    //ViewData["totalp"] = proc_Response.Where(s => s.TicketStatus.ToUpper().Contains("Proccessed")).Sum(s => Convert.ToInt32(s.totalOfferedFare));
                    return View(proc_Response);
                }

            }
            catch
            {
                TempData["Status"] = "Failed";
                TempData["Message"] = "An error occured while proccessing request.";
                return RedirectToAction("Travel", "Home");
            }
        }

        [HttpPost]
        [PermissioncheckingAttribute(servicename = "HOTEL", permision = "Write")]
        public ActionResult CancellationStatus(string ChangeReqId, int idno)
        {
            try
            {
                var userid = User.Identity.GetUserId();
                if (!string.IsNullOrWhiteSpace(ChangeReqId) && idno > 0)
                {
                    var token = string.Empty;
                    token = getAuthToken();
                    //token = "v8Vr_d0PbVU9mnzxjludAfL12RYq9moHIiJeWSxVL5fzydzLguHYRCfB-uws7pH79vB5pmqEYQqM3agIh6kx5r4Oby4Dvv5eWWAzbpQtuMLGhrdDl3FZ9ySXd2rlYGz_mn0svSJyFp8-LvHmM_qAIJRZ3tTxr0vstIdC14YLGdFKMiiXeWsIunHt5b6rZ3jqnXTDTFzXpOZFUH_arKpdFKUpR_VXg9Z9S10JCDt72jQp2_jQERaLFQ-SJbidA7VlSGxXRrtnBx-j1d1XpKg3JE_zeop02Q3iQEMmhDYrPkJBfTeEUHoFU7zLALcg2LL_mayd1IPnnCx4--rwD_ssiRyzmoqFKSaUqcAbHBbuG6opAqyBqI8f4dsp8PKNECll-PpO4fUIiOpDUbHNT0sWk1Yf5k2quxbc2MI2YnSFFscydX0B8KCLqSgPdGGjmaMXBcSwB0spXw2iikwLuOWkOmKwE1m0e2g3yr72g2O4srQZN8-6Icf6GAhU85G4LUXqimcnuhnEP0g6qaAyP7zgbg";
                    if (string.IsNullOrWhiteSpace(token))
                    {
                        UpdateAuthToken();
                        var AjaxRespo = new { Status = "Failed", Message = "Failed to connect with provider.please try again." };
                        return Json(JsonConvert.SerializeObject(AjaxRespo));
                    }
                    var changeReqModeljson = JsonConvert.SerializeObject(new { BookingMode = 5, ChangeRequestId = ChangeReqId });
                    var respo = GetCancellationRequestStatus(token, changeReqModeljson);
                    if (respo == null)
                    {
                        var AjaxRespo = new { Status = "Failed", Message = "Unable to fatch Data" };
                        return Json(JsonConvert.SerializeObject(AjaxRespo));
                    }
                    else if (respo.StatusCode == HttpStatusCode.OK)
                    {
                        dynamic respoObject = JsonConvert.DeserializeObject(respo.Content);
                        if (respoObject.Content != null || respoObject.Content.ADDINFO != null)
                        {
                            if (respoObject.Content.ADDINFO.Error.ErrorCode == 0)
                            {
                                decimal RefundedAmount = Convert.ToDecimal(respoObject.Content.ADDINFO.RefundedAmount);
                                decimal CancellationCharge = Convert.ToDecimal(respoObject.Content.ADDINFO.CancellationCharge);
                                int ChangeRequestStatus = Convert.ToInt32(respoObject.Content.ADDINFO.ChangeRequestStatus);
                                using (VastwebmultiEntities db = new VastwebmultiEntities())
                                {
                                    System.Data.Entity.Core.Objects.ObjectParameter Message = new System.Data.Entity.Core.Objects.ObjectParameter("Message", typeof(string));
                                    System.Data.Entity.Core.Objects.ObjectParameter Status = new System.Data.Entity.Core.Objects.ObjectParameter("Status", typeof(string));
                                    var dbRespo = db.proc_HotelCancellationRefund(idno, ChangeReqId, RefundedAmount, CancellationCharge, ChangeRequestStatus, respo.Content, Status, Message);
                                    if (ChangeRequestStatus == 2 || ChangeRequestStatus == 1)
                                    {
                                        var AjaxRespo1 = new { Status = "Pending", Message = "Cancellation request is in proccess." };
                                        return Json(JsonConvert.SerializeObject(AjaxRespo1));
                                    }
                                    else if (ChangeRequestStatus == 3)
                                    {
                                        var AjaxRespo1 = new { Status = "Success", Message = "Cancellation request accepted." };
                                        return Json(JsonConvert.SerializeObject(AjaxRespo1));
                                    }
                                    else
                                    {
                                        var AjaxRespo1 = new { Status = "Failed", Message = "Cancellation request rejected." };
                                        return Json(JsonConvert.SerializeObject(AjaxRespo1));
                                    }

                                }
                            }
                            else
                            {
                                var AjaxRespo1 = new { Status = "Failed", Message = Convert.ToString(respoObject.Content.ADDINFO.Error.ErrorMessage) };
                                return Json(JsonConvert.SerializeObject(AjaxRespo1));
                            }
                        }
                        var AjaxRespo = new { Status = "Failed", Message = "Unable to fatch Data" };
                        return Json(JsonConvert.SerializeObject(AjaxRespo));
                    }
                    else
                    {
                        var AjaxRespo = new { Status = "Failed", Message = "Unable to fatch Data" };
                        return Json(JsonConvert.SerializeObject(AjaxRespo));
                    }
                }
                else
                {
                    var AjaxRespo = new { Status = "Failed", Message = "Invalid Request" };
                    return Json(JsonConvert.SerializeObject(AjaxRespo));
                }
            }
            catch (Exception ex)
            {
                var AjaxRespo = new { Status = "Failed", Message = ex.Message };
                return Json(JsonConvert.SerializeObject(AjaxRespo));
            }
        }

        [HttpPost]
        [PermissioncheckingAttribute(servicename = "HOTEL", permision = "Read")]
        public ActionResult GuestDetails(string TXNID)
        {
            try
            {
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    var guestList = db.HotelPassengers.Where(a => a.AgentRefno == TXNID).ToList();
                    return PartialView(guestList);
                }
            }
            catch
            {
                return PartialView();
            }
        }
        [HttpPost]
        [PermissioncheckingAttribute(servicename = "HOTEL", permision = "Read")]
        public ActionResult HotelPriceDetails(string TXNID)
        {
            try
            {
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    var guestList = db.HotelPrices.Where(a => a.AgentRefno == TXNID).ToList();
                    return PartialView(guestList);
                }
            }
            catch
            {
                return PartialView();
            }
        }
        [HttpPost]
        [PermissioncheckingAttribute(servicename = "HOTEL", permision = "Write")]
        public ActionResult HotelBookingStatus(string TXNID)
        {
            try
            {
                var userid = db.Hotel_info.Where(aa => aa.TraceId == TXNID).SingleOrDefault().retailerid;
                if (!string.IsNullOrWhiteSpace(TXNID))
                {
                    var token = string.Empty;
                    token = getAuthToken();
                    //token = "v8Vr_d0PbVU9mnzxjludAfL12RYq9moHIiJeWSxVL5fzydzLguHYRCfB-uws7pH79vB5pmqEYQqM3agIh6kx5r4Oby4Dvv5eWWAzbpQtuMLGhrdDl3FZ9ySXd2rlYGz_mn0svSJyFp8-LvHmM_qAIJRZ3tTxr0vstIdC14YLGdFKMiiXeWsIunHt5b6rZ3jqnXTDTFzXpOZFUH_arKpdFKUpR_VXg9Z9S10JCDt72jQp2_jQERaLFQ-SJbidA7VlSGxXRrtnBx-j1d1XpKg3JE_zeop02Q3iQEMmhDYrPkJBfTeEUHoFU7zLALcg2LL_mayd1IPnnCx4--rwD_ssiRyzmoqFKSaUqcAbHBbuG6opAqyBqI8f4dsp8PKNECll-PpO4fUIiOpDUbHNT0sWk1Yf5k2quxbc2MI2YnSFFscydX0B8KCLqSgPdGGjmaMXBcSwB0spXw2iikwLuOWkOmKwE1m0e2g3yr72g2O4srQZN8-6Icf6GAhU85G4LUXqimcnuhnEP0g6qaAyP7zgbg";
                    if (string.IsNullOrWhiteSpace(token))
                    {
                        UpdateAuthToken();
                        TempData["Status"] = "Failed";
                        TempData["Message"] = "Failed to connect with provider.please try again.";
                        return RedirectToAction("Travel", "Home");
                    }
                    var respo = GetBookingDetails(token, "", "", TXNID);
                    if (respo == null)
                    {
                        var AjaxRespo = new { Status = "Failed", Message = "Unable to fatch Data" };
                        return Json(JsonConvert.SerializeObject(AjaxRespo));
                    }
                    else if (respo.StatusCode == HttpStatusCode.OK)
                    {
                        dynamic respoObject = JsonConvert.DeserializeObject(respo.Content);
                        if (respoObject.Content != null || respoObject.Content.ADDINFO != null)
                        {
                            if (respoObject.Content.ADDINFO.Error.ErrorCode == 0)
                            {
                                using (VastwebmultiEntities db = new VastwebmultiEntities())
                                {

                                    string BookingSts = Convert.ToString(respoObject.Content.ADDINFO.HotelBookingStatus);
                                    string BookingId = Convert.ToString(respoObject.Content.ADDINFO.BookingId);
                                    string BookingRefNo = Convert.ToString(respoObject.Content.ADDINFO.BookingRefNo);
                                    string TraceId = Convert.ToString(respoObject.Content.ADDINFO.TraceId);
                                    string InvoiceNumber = Convert.ToString(respoObject.Content.ADDINFO.InvoiceNo);
                                    bool VoucherStatus = Convert.ToBoolean(respoObject.Content.ADDINFO.VoucherStatus);
                                    bool IsPriceChanged = Convert.ToBoolean(respoObject.Content.ADDINFO.IsPriceChanged);
                                    bool IsCancellationPolicyChanged = Convert.ToBoolean(respoObject.Content.ADDINFO.IsCancellationPolicyChanged);
                                    var Entry = db.Hotel_info.FirstOrDefault(a => a.TraceId == TXNID);
                                    System.Data.Entity.Core.Objects.ObjectParameter IsSuccess = new System.Data.Entity.Core.Objects.ObjectParameter("IsSuccess", typeof(bool));
                                    System.Data.Entity.Core.Objects.ObjectParameter Message = new System.Data.Entity.Core.Objects.ObjectParameter("Message", typeof(Boolean));
                                    if (BookingSts == "Confirmed" && Entry.HotelBookingStatus == "Proccessed")
                                    {
                                        db.proc_UpdateHotelBooking(Entry.idno.ToString(), userid, respo.Content, 0, BookingSts, InvoiceNumber, "", IsPriceChanged,
                                    VoucherStatus, BookingRefNo, IsCancellationPolicyChanged, BookingId, IsSuccess, Message);
                                        var AjaxRespo1 = new { Status = "Success", Message = "Booking success." };
                                        return Json(JsonConvert.SerializeObject(AjaxRespo1));
                                    }
                                    else if (Entry.HotelBookingStatus == "Proccessed")
                                    {
                                        db.proc_UpdateHotelBooking(Entry.idno.ToString(), userid, respo.Content, 1, BookingSts, InvoiceNumber, "", IsPriceChanged,
                                   VoucherStatus, BookingRefNo, IsCancellationPolicyChanged, BookingId, IsSuccess, Message);
                                        var AjaxRespo1 = new { Status = "Failed", Message = "Booking status : " + BookingSts };
                                        return Json(JsonConvert.SerializeObject(AjaxRespo1));
                                    }
                                    else
                                    {
                                        var AjaxRespo1 = new { Status = "Success", Message = "Booking Status : " + BookingSts };
                                        return Json(JsonConvert.SerializeObject(AjaxRespo1));
                                    }
                                }
                            }
                            else
                            {
                                var AjaxRespo11 = new { Status = "Failed", Message = Convert.ToString(respoObject.Content.ADDINFO.Error.ErrorMessage) };
                                return Json(JsonConvert.SerializeObject(AjaxRespo11));
                            }
                        }
                        var AjaxRespo = new { Status = "Failed", Message = "Unable to fatch Data" };
                        return Json(JsonConvert.SerializeObject(AjaxRespo));
                    }
                    else
                    {
                        var AjaxRespo = new { Status = "Failed", Message = "StatusCode : " + respo.StatusCode.ToString() };
                        return Json(JsonConvert.SerializeObject(AjaxRespo));
                    }
                }
                else
                {
                    var AjaxRespo = new { Status = "Failed", Message = "Invalid Request" };
                    return Json(JsonConvert.SerializeObject(AjaxRespo));
                }
            }
            catch (Exception ex)
            {
                var AjaxRespo = new { Status = "Failed", Message = ex.Message };
                return Json(JsonConvert.SerializeObject(AjaxRespo));
            }
        }
        [HttpGet]
        [PermissioncheckingAttribute(servicename = "HOTEL", permision = "Write")]
        public ActionResult HotelFullDetails(string TXNID)
        {
            try
            {
                var userid = User.Identity.GetUserId();
                if (!string.IsNullOrWhiteSpace(TXNID))
                {
                    var token = string.Empty;
                    token = getAuthToken();
                    //token = "v8Vr_d0PbVU9mnzxjludAfL12RYq9moHIiJeWSxVL5fzydzLguHYRCfB-uws7pH79vB5pmqEYQqM3agIh6kx5r4Oby4Dvv5eWWAzbpQtuMLGhrdDl3FZ9ySXd2rlYGz_mn0svSJyFp8-LvHmM_qAIJRZ3tTxr0vstIdC14YLGdFKMiiXeWsIunHt5b6rZ3jqnXTDTFzXpOZFUH_arKpdFKUpR_VXg9Z9S10JCDt72jQp2_jQERaLFQ-SJbidA7VlSGxXRrtnBx-j1d1XpKg3JE_zeop02Q3iQEMmhDYrPkJBfTeEUHoFU7zLALcg2LL_mayd1IPnnCx4--rwD_ssiRyzmoqFKSaUqcAbHBbuG6opAqyBqI8f4dsp8PKNECll-PpO4fUIiOpDUbHNT0sWk1Yf5k2quxbc2MI2YnSFFscydX0B8KCLqSgPdGGjmaMXBcSwB0spXw2iikwLuOWkOmKwE1m0e2g3yr72g2O4srQZN8-6Icf6GAhU85G4LUXqimcnuhnEP0g6qaAyP7zgbg";
                    if (string.IsNullOrWhiteSpace(token))
                    {
                        UpdateAuthToken();
                        TempData["Status"] = "Failed";
                        TempData["Message"] = "Failed to connect with provider.please try again.";
                        return RedirectToAction("HotelReport", "Hotel");
                    }
                    var respo = GetBookingDetails(token, "", "", TXNID);
                    if (respo == null)
                    {
                        TempData["Status"] = "Failed";
                        TempData["Message"] = "Unable to fatch data.";
                        return RedirectToAction("HotelReport", "Hotel");
                    }
                    else if (respo.StatusCode == HttpStatusCode.OK)
                    {
                        var respoObject = JsonConvert.DeserializeObject<HotelBookingDetailsResponseModel>(respo.Content);
                        if (respoObject.Content != null || respoObject.Content.Addinfo != null)
                        {
                            if (respoObject.Content.Addinfo.Error.ErrorCode == 0)
                            {
                                return View(respoObject);
                            }
                            else
                            {
                                TempData["Status"] = "Failed";
                                TempData["Message"] = Convert.ToString(respoObject.Content.Addinfo.Error.ErrorMessage);
                                return RedirectToAction("HotelReport", "Hotel");
                            }
                        }
                        TempData["Status"] = "Failed";
                        TempData["Message"] = "Unable to fatch data.";
                        return RedirectToAction("HotelReport", "Hotel");
                    }
                    else
                    {
                        TempData["Status"] = "Failed";
                        TempData["Message"] = "StatusCode : " + respo.StatusCode.ToString();
                        return RedirectToAction("HotelReport", "Hotel");
                    }
                }
                else
                {
                    TempData["Status"] = "Failed";
                    TempData["Message"] = "Invalid request.";
                    return RedirectToAction("HotelReport", "Hotel");
                }
            }
            catch (Exception ex)
            {
                TempData["Status"] = "Failed";
                TempData["Message"] = ex.Message;
                return RedirectToAction("HotelReport", "Hotel");
            }
        }

        [HttpPost]
        [PermissioncheckingAttribute(servicename = "HOTEL", permision = "Write")]
        public ActionResult BookingCancallation(int id, string BookingId)
        {
            try
            {
                var userid = User.Identity.GetUserId();
                if (id > 0)
                {
                    var token = string.Empty;
                    token = getAuthToken();
                    //token = "v8Vr_d0PbVU9mnzxjludAfL12RYq9moHIiJeWSxVL5fzydzLguHYRCfB-uws7pH79vB5pmqEYQqM3agIh6kx5r4Oby4Dvv5eWWAzbpQtuMLGhrdDl3FZ9ySXd2rlYGz_mn0svSJyFp8-LvHmM_qAIJRZ3tTxr0vstIdC14YLGdFKMiiXeWsIunHt5b6rZ3jqnXTDTFzXpOZFUH_arKpdFKUpR_VXg9Z9S10JCDt72jQp2_jQERaLFQ-SJbidA7VlSGxXRrtnBx-j1d1XpKg3JE_zeop02Q3iQEMmhDYrPkJBfTeEUHoFU7zLALcg2LL_mayd1IPnnCx4--rwD_ssiRyzmoqFKSaUqcAbHBbuG6opAqyBqI8f4dsp8PKNECll-PpO4fUIiOpDUbHNT0sWk1Yf5k2quxbc2MI2YnSFFscydX0B8KCLqSgPdGGjmaMXBcSwB0spXw2iikwLuOWkOmKwE1m0e2g3yr72g2O4srQZN8-6Icf6GAhU85G4LUXqimcnuhnEP0g6qaAyP7zgbg";
                    if (string.IsNullOrWhiteSpace(token))
                    {
                        UpdateAuthToken();
                        var AjaxRespo = new { Status = "Failed", Message = "Failed to connect with provider.please try again." };
                        return Json(JsonConvert.SerializeObject(AjaxRespo));
                    }
                    using (VastwebmultiEntities db = new VastwebmultiEntities())
                    {
                        var entry = db.HotelCancellations.FirstOrDefault(a => a.Idno == id);
                        if (entry == null)
                        {
                            var CancalltionJsonString = JsonConvert.SerializeObject(new { BookingId = BookingId, BookingMode = 5, RequestType = 4 });
                            var respo = InitializeCancellation(token, CancalltionJsonString);
                            if (respo == null)
                            {
                                var AjaxRespo = new { Status = "Failed", Message = "Failed to get hotel cancellation response" };
                                return Json(JsonConvert.SerializeObject(AjaxRespo));
                            }
                            else if (respo.StatusCode == HttpStatusCode.OK)
                            {
                                dynamic respoObject = JsonConvert.DeserializeObject(respo.Content);
                                if (respoObject.Content != null || respoObject.Content.ADDINFO != null)
                                {
                                    if (respoObject.Content.ADDINFO.Error.ErrorCode == 0)
                                    {
                                        string ChangeRequestStatus = Convert.ToString(respoObject.Content.ADDINFO.ChangeRequestStatus);
                                        string TraceId = Convert.ToString(respoObject.Content.ADDINFO.TraceId);
                                        int ChangeRequestId = Convert.ToInt32(respoObject.Content.ADDINFO.ChangeRequestId);
                                        entry = new HotelCancellation();
                                        entry.Idno = id;
                                        entry.ChangeRequestStatus = ChangeRequestStatus;
                                        entry.TraceId = TraceId;//This Trace Id Callellation Trace Id that is diferent than  Hotel_info TraceId
                                        entry.ChangeRequestId = ChangeRequestId;
                                        entry.RequestDate = DateTime.Now;
                                        entry.RequestType = 4;
                                        entry.Remarks = "";
                                        entry.CancelRequestJson = CancalltionJsonString;
                                        db.HotelCancellations.Add(entry);
                                        db.SaveChanges();
                                        var AjaxRespo1 = new { Status = "Success", Message = "A cancellation request sent,You can track request using STATUS button." };
                                        return Json(JsonConvert.SerializeObject(AjaxRespo1));
                                    }
                                    else
                                    {
                                        var AjaxRespo11 = new { Status = "Failed", Message = Convert.ToString(respoObject.Content.ADDINFO.Error.ErrorMessage) };
                                        return Json(JsonConvert.SerializeObject(AjaxRespo11));
                                    }
                                }
                                var AjaxRespo = new { Status = "Failed", Message = "Unable to fatch Data" };
                                return Json(JsonConvert.SerializeObject(AjaxRespo));
                            }
                            else
                            {
                                var AjaxRespo = new { Status = "Failed", Message = "StatusCode : " + respo.StatusCode.ToString() };
                                return Json(JsonConvert.SerializeObject(AjaxRespo));
                            }
                        }
                        else
                        {
                            var AjaxRespo = new { Status = "Failed", Message = "A cancellation request is already sent. please hit status button. " };
                            return Json(JsonConvert.SerializeObject(AjaxRespo));
                        }
                    }
                }
                else
                {
                    var AjaxRespo = new { Status = "Failed", Message = "Invalid Request" };
                    return Json(JsonConvert.SerializeObject(AjaxRespo));
                }
            }
            catch (Exception ex)
            {
                var AjaxRespo = new { Status = "Failed", Message = ex.Message };
                return Json(JsonConvert.SerializeObject(AjaxRespo));
            }
        }


        public IRestResponse GetBookingDetails(string vastwebToken, string BookingId, string ConfirmationNo, string TraceId)
        {
            #region HotelGetBookingDetails
            var GetBookinDetailsModel = new { BookingId = BookingId, ConfirmationNo = ConfirmationNo, TraceId = TraceId };

            var url = VastbazaarBaseUrl + "api/Hotel/GetBookingDetails";
            var client = new RestClient(url);
            var request = new RestRequest(Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("authorization", "bearer " + vastwebToken);
            request.AddHeader("cache-contro,", "no-cache");
            request.AddHeader("content-type", "application/json");
            request.AddHeader("accept-encoding", "gzip");
            request.AddParameter("application/json", JsonConvert.SerializeObject(GetBookinDetailsModel), ParameterType.RequestBody);
            client.ReadWriteTimeout = 180000;
            var respo = client.Execute(request);
            return respo;
            #endregion
        }

        public IRestResponse InitializeCancellation(string vastwebToken, string changeRequestString)
        {
            #region HotelGetBookingDetails

            var url = VastbazaarBaseUrl + "api/Hotel/ChangeRequest";
            var client = new RestClient(url);
            var request = new RestRequest(Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("authorization", "bearer " + vastwebToken);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/json");
            request.AddHeader("accept-encoding", "gzip");
            request.AddParameter("application/json", changeRequestString, ParameterType.RequestBody);
            client.ReadWriteTimeout = 180000;
            var respo = client.Execute(request);
            return respo;
            #endregion
        }

        public IRestResponse GetCancellationRequestStatus(string vastwebToken, string changeRequestString)
        {
            #region HotelGetBookingDetails

            var url = VastbazaarBaseUrl + "api/Hotel/ChangeRequestStatus";
            var client = new RestClient(url);
            var request = new RestRequest(Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("authorization", "bearer " + vastwebToken);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/json");
            request.AddHeader("accept-encoding", "gzip");
            request.AddParameter("application/json", changeRequestString, ParameterType.RequestBody);
            client.ReadWriteTimeout = 180000;
            var respo = client.Execute(request);
            return respo;
            #endregion
        }


        #endregion Hotal Booking karli ab ja foot



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




        public class JsonModel
        {
            public string HTMLString { get; set; }
            public bool NoMoredata { get; set; }
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
    }
}