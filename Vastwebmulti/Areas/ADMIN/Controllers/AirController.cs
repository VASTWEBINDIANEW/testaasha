using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using RestSharp;
using Rotativa;  
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using Vastwebmulti.Areas.RETAILER.Models;
using Vastwebmulti.Models;
                                          
namespace Vastwebmulti.Areas.ADMIN.Controllers
{
    /// <summary>
    /// ADMIN Area - Manages flight ticket reports, cancellations, and ticket viewing/printing
    /// </summary>
    //Remote
    [Authorize(Roles = "Admin")]
    public class AirController : Controller
    {
        //string VastbazaarBaseUrl = "http://localhost:62146/";

        string VastbazaarBaseUrl =  "http://api.vastbazaar.com/";
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        VastwebmultiEntities db = new VastwebmultiEntities();
        public AirController()
        {

        }
        public AirController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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
        /// <summary>
        /// GET - Displays the flight ticket report page with user filter dropdowns.
        /// </summary>
        [HttpGet]
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
                TempData["Status"] = "Processed";
                TempData["Message"] = "Processed.";
                return View();
            }
        }
        /// <summary>
        /// POST - Filters and returns the flight ticket report by date range.
        /// </summary>
        [HttpPost]
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
                TempData["Status"] = "Processed";
                TempData["Message"] = "Processed.";
                return RedirectToAction("Travel", "Home");
            }
        }
        /// <summary>
        /// Child action - Renders the partial ticket report view filtered by the provided criteria.
        /// </summary>
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
                TempData["Status"] = "Processed";
                TempData["Message"] = "Processed.";
                return RedirectToAction("TicketReport", "Air");
            }
        }
        /// <summary>
        /// POST - Returns a paginated chunk of ticket report data as JSON for infinite scroll.
        /// </summary>
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
                TempData["Status"] = "Processed";
                TempData["Message"] = "Processed.";
                return RedirectToAction("TicketReport", "Air");
            }
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
        public class JsonModel
        {
            public string HTMLString { get; set; }
            public bool NoMoredata { get; set; }
        }
        /// <summary>
        /// GET - Exports the flight ticket report to an Excel file for download.
        /// </summary>
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

                    if (proc_Response.Any())
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
                TempData["Status"] = "Processed";
                TempData["Message"] = "Processed.";
                return RedirectToAction("TicketReport", "Air");
            }
        }
        /// <summary>
        /// GET - Exports the flight ticket report as a PDF for download.
        /// </summary>
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
                    if (PNR == "undefined")
                    {
                        PNR = null;
                    }
                    var proc_Response = db.proc_FlightReport(1, 1000000, ddl_status, retailerid, DealerId, MasterId, null, null, PNR, null, null, frm_date, to_date).ToList();
                    return new ViewAsPdf(proc_Response);
                }
            }
            catch
            {
                TempData["Status"] = "Processed";
                TempData["Message"] = "Processed.";
                return RedirectToAction("TicketReport", "Air");
            }
        }
        /// <summary>
        /// GET - Returns JSON totals (success, failed, pending) for the flight ticket report by date range.
        /// </summary>
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
                TempData["Status"] = "Processed";
                TempData["Message"] = "Processed";
                return RedirectToAction("TicketReport", "Air");
            }
        }
        /// <summary>
        /// POST - Fetches live flight booking status from the provider API and returns JSON.
        /// </summary>
        [HttpPost]
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
                        //if (ticket != null)
                        //{
                        //    try
                        //    {
                        //        System.Data.Entity.Core.Objects.ObjectParameter IsSuccess = new System.Data.Entity.Core.Objects.ObjectParameter("IsSuccess", typeof(bool));
                        //        System.Data.Entity.Core.Objects.ObjectParameter Message = new System.Data.Entity.Core.Objects.ObjectParameter("Message", typeof(Boolean));
                        //        //System.Data.Entity.Core.Objects.ObjectParameter Idno = new System.Data.Entity.Core.Objects.ObjectParameter("IdNo", typeof(int));
                        //        db.proc_UpdateFlightBooking(ticket.idno.ToString(), ticket.RetailerId, ticket.OfferedFare, ticket.PublishedFare, response.Content, 1, 0, "", "", "", "", IsSuccess, Message);
                        //    }
                        //    catch
                        //    {

                        //    }
                        //}
                        var viewRespo = new { Status = "Processed", StatusCode = 0, Message = "Processed!!" };
                        return Json(JsonConvert.SerializeObject(viewRespo), JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch
            {
                var viewRespo = new { Status = "Processed", StatusCode = 0, Message = "Processed!!" };
                return Json(JsonConvert.SerializeObject(viewRespo), JsonRequestBehavior.AllowGet);
            }
            //return View(respo);
        }
        /// <summary>
        /// GET - Displays the flight cancellation report for today's date.
        /// </summary>
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
                TempData["Status"] = "Processed";
                TempData["Message"] = "Processed.";
                return RedirectToAction("Travel", "Home");
            }
        }
        /// <summary>
        /// POST - Filters and displays the flight cancellation report by date range and user.
        /// </summary>
        [HttpPost]
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
                TempData["Status"] = "Processed";
                TempData["Message"] = "Processed.";
                return RedirectToAction("Travel", "Home");
            }
        }
        /// <summary>
        /// POST - Checks the cancellation request status with the provider and updates the refund record.
        /// </summary>
        [HttpPost]
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
                var jsrespo = new { Status = "Processed", Message = "Processed", Result = "" };
                return Json(JsonConvert.SerializeObject(jsrespo));
            }
        }
        /// <summary>
        /// GET - Displays the full flight ticket details view including fare and baggage information.
        /// </summary>
        [HttpGet]
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
                    ViewBag.CreatorDistrict = db.District_Desc.FirstOrDefault(d => d.Dist_id == retailer.District && d.State_id == retailer.State).Dist_Desc;
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
                TempData["Status"] = "Processed";
                TempData["Message"] = "Processed.";
                return RedirectToAction("TicketReport", "Air");
            }
        }
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
                    ViewBag.CreatorDistrict = db.District_Desc.FirstOrDefault(d => d.Dist_id == retailer.District && d.State_id == retailer.State).Dist_Desc;
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
                TempData["Status"] = "Processed";
                TempData["Message"] = "Processed.";
                return RedirectToAction("TicketReport", "Air");
            }
        }
        [HttpGet]
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
                    ViewBag.CreatorDistrict = db.District_Desc.FirstOrDefault(d => d.Dist_id == retailer.District && d.State_id == retailer.State).Dist_Desc;
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
                TempData["Status"] = "Processed";
                TempData["Message"] = "Processed.";
                return RedirectToAction("TicketReport", "Air");
            }
        }
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
                    ViewBag.CreatorDistrict = db.District_Desc.FirstOrDefault(d => d.Dist_id == retailer.District && d.State_id == retailer.State).Dist_Desc;
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
                TempData["Status"] = "Processed";
                TempData["Message"] = "Processed.";
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
    }
}