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
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using Vastwebmulti.Areas.RETAILER.ViewModels;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.ADMIN.Controllers
{

    /// <summary>
    /// ADMIN area - Hotel booking ka management karta hai: report, cancellation aur TBO API ke through booking
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class HotelController : Controller
    {
        // string VastbazaarBaseUrl = "http://localhost:62146/";
        string VastbazaarBaseUrl = "http://api.vastbazaar.com/";
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        VastwebmultiEntities db = new VastwebmultiEntities();
        public HotelController()
        {

        }
        public HotelController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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
        /// GET - Aaj ki date ka hotel booking report aur user filter dropdowns dikhata hai
        /// </summary>
        //[MenuAccessFilter] //used in paid and nonpaid services
        public ActionResult HotelReport()
        {
            try
            {
                var userid = User.Identity.GetUserId();
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    var stands = db.Superstokist_details.AsNoTracking().ToList();
                    IEnumerable<SelectListItem> selectList = from s in stands
                                                             select new SelectListItem
                                                             {
                                                                 Value = s.SSId,
                                                                 Text = s.FarmName.ToString()
                                                             };

                    var totalwhitelabel = db.WhiteLabel_userList.AsNoTracking().ToList();
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
        /// <summary>
        /// POST - Date range, status aur user ke basis par hotel booking report filter karke dikhata hai
        /// </summary>
        [HttpPost]
        public ActionResult HotelReport(string ddl_status, string txt_frm_date, string txt_to_date, string ddlusers, string allmaster, string alldealer, string allretailer, string ddl_status_ticket)
        {
            try
            {
                ViewBag.chk = "post";
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    // show master id 
                    var stands = db.Superstokist_details.AsNoTracking().ToList();
                    IEnumerable<SelectListItem> selectList = from s in stands
                                                             select new SelectListItem
                                                             {
                                                                 Value = s.SSId,
                                                                 Text = s.FarmName.ToString()
                                                             };

                    var totalwhitelabel = db.WhiteLabel_userList.AsNoTracking().ToList();
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



        /// <summary>
        /// GET - Hotel booking report ko Excel file mein export karke download karta hai
        /// </summary>
        public ActionResult ExcelHotelBookingReport(string ddl_status, string txt_frm_date, string txt_to_date, string ddlusers, string allmaster, string alldealer, string allretailer, string ddl_status_ticket)
        {
            try
            {
                ViewBag.chk = "post";
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    // show master id 
                    var stands = db.Superstokist_details.AsNoTracking().ToList();
                    IEnumerable<SelectListItem> selectList = from s in stands
                                                             select new SelectListItem
                                                             {
                                                                 Value = s.SSId,
                                                                 Text = s.FarmName.ToString()
                                                             };

                    var totalwhitelabel = db.WhiteLabel_userList.AsNoTracking().ToList();
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


                    if (ch.Any())
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

        /// <summary>
        /// GET - Hotel booking report ko PDF format mein export karke download karta hai
        /// </summary>
        public ActionResult PDFHotelReport(string ddl_status, string txt_frm_date, string txt_to_date, string ddlusers, string allmaster, string alldealer, string allretailer, string ddl_status_ticket)
        {
            try
            {
                ViewBag.chk = "post";
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    // show master id 
                    var stands = db.Superstokist_details.AsNoTracking().ToList();
                    IEnumerable<SelectListItem> selectList = from s in stands
                                                             select new SelectListItem
                                                             {
                                                                 Value = s.SSId,
                                                                 Text = s.FarmName.ToString()
                                                             };

                    var totalwhitelabel = db.WhiteLabel_userList.AsNoTracking().ToList();
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


        /// <summary>
        /// GET - Aaj ki hotel cancellation queue user filter ke saath dikhata hai
        /// </summary>
        [HttpGet]
        public ActionResult CancellationQueue()
        {

            try
            {
                var userid = User.Identity.GetUserId();
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    // show master id 
                    var stands = db.Superstokist_details.AsNoTracking().ToList();
                    IEnumerable<SelectListItem> selectList = from s in stands
                                                             select new SelectListItem
                                                             {
                                                                 Value = s.SSId,
                                                                 Text = s.Email + "--" + s.SuperstokistName.ToString()
                                                             };
                    ViewBag.allmaster = new SelectList(selectList, "Value", "Text");
                    //show whitelabel
                    var totalwhitelabel = db.WhiteLabel_userList.AsNoTracking().ToList();
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

        /// <summary>
        /// POST - Hotel cancellation queue ko date range, status aur user se filter karta hai
        /// </summary>
        [HttpPost]
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
                    var stands = db.Superstokist_details.AsNoTracking().ToList();
                    IEnumerable<SelectListItem> selectList = from s in stands
                                                             select new SelectListItem
                                                             {
                                                                 Value = s.SSId,
                                                                 Text = s.Email + "--" + s.SuperstokistName.ToString()
                                                             };
                    ViewBag.allmaster = new SelectList(selectList, "Value", "Text");
                    //show whitelabel
                    var totalwhitelabel = db.WhiteLabel_userList.AsNoTracking().ToList();
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

        /// <summary>
        /// POST - Provider se hotel cancellation request ka status check karke refund process karta hai
        /// </summary>
        [HttpPost]
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

        /// <summary>
        /// POST - Returns the hotel passenger guest list for a given transaction ID as a partial view.
        /// </summary>
        [HttpPost]
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
        /// <summary>
        /// POST - Returns the hotel price breakdown for a given transaction ID as a partial view.
        /// </summary>
        [HttpPost]
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
        /// <summary>
        /// POST - Fetches live hotel booking status from the provider and updates the booking record.
        /// </summary>
        [HttpPost]
        public ActionResult HotelBookingStatus(string TXNID)
        {
            try
            {
                var userid = db.Hotel_info.FirstOrDefault(aa => aa.TraceId == TXNID)?.retailerid;
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
        /// <summary>
        /// GET - Fetches and displays the full hotel booking details from the provider for a given transaction.
        /// </summary>
        [HttpGet]
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

        /// <summary>
        /// POST - Initiates a hotel booking cancellation request with the provider and saves the change request.
        /// </summary>
        [HttpPost]
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
            request.AddHeader("cache-control", "no-cache");
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
        public IRestResponse tokencheck()
        {
            var apidetails = db.Money_API_URLS.Where(aa => aa.API_Name == "VASTWEB").SingleOrDefault();
            var token = apidetails == null ? "" : apidetails.Token;
            var apiid = apidetails == null ? "" : apidetails.API_ID;
            var apiidpwd = apidetails == null ? "" : apidetails.Api_pwd;
            var client = new RestClient(VastbazaarBaseUrl + "token");
            var request = new RestRequest(Method.POST);
            request.AddHeader("iptoken", token.Trim());
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("application/x-www-form-urlencoded", "UserName=" + HttpUtility.UrlEncode(apiid) + "&Password=" + HttpUtility.UrlEncode(apiidpwd) + "&grant_type=password", ParameterType.RequestBody);
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
                        //Models.Vastbillpay vb = new Models.Vastbillpay();
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