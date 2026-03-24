using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vastwebmulti.Areas.RETAILER.Models;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.RETAILER.Controllers
{

    [Authorize(Roles = "Retailer")]
    [Low_Bal_CustomFilter()]
    /// <summary>
    /// Retailer ke liye nayi DTH connection booking aur uske plans manage karta hai
    /// </summary>
    public class DthConnectionController : Controller
    {

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        VastwebmultiEntities db = new VastwebmultiEntities();
        /// <summary>
        /// Default constructor - controller initialize karta hai
        /// </summary>
        public DthConnectionController()
        {

        }
        /// <summary>
        /// UserManager aur SignInManager ke saath controller initialize karta hai
        /// </summary>
        public DthConnectionController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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
        // GET: RETAILER/DthConnection
        /// <summary>
        /// DTH connection ki listing dikhata hai, search aur sort bhi support karta hai
        /// </summary>
        public ActionResult Index(string txtSearch, string SortBy)
        {
            var userid = User.Identity.GetUserId();
            DthConVM model = new DthConVM();
            model.ProductName = txtSearch;
            model.SortBy = SortBy;
            model.itemsInCart = db.Carts.Count(a => a.BuyerId == userid);
            return View(model);
        }
        /// <summary>
        /// DTH operators aur set top box categories ka menu partial view return karta hai
        /// </summary>
        public PartialViewResult _MenuList()
        {
            DthConMenuVM model = new DthConMenuVM();
            model.lstCatagory = db.DthOperators.Select(a => new DthConCatagoryModel { CatID = a.Id, CatName = a.OptName }).ToList();
            model.subCata = db.DthSetTopBoxes.Select(r => new DthConSubCatagoryModel
            {
                SubCatagoryName = r.BoxType,
                SubcatID = r.Id
            }).ToList();
            return PartialView(model);
        }
        /// <summary>
        /// Filter ke hisaab se DTH plans ki list partial view mein dikhata hai
        /// </summary>
        public PartialViewResult _productlist(int? PlanId, int? SetTopBoxID, decimal? Price, string SortBy)
        {
            var RetailerID = User.Identity.GetUserId();
            DthConPlanDetails model = new DthConPlanDetails();
            model.itemsInCart = db.Carts.Count(a => a.BuyerId == RetailerID);
            if (SetTopBoxID != null && SetTopBoxID > 0)
            {
                model.CataTitle = "";// db.SubCatagories.SingleOrDefault(a => a.SubCatID == SubCatId)?.SubCatName;
            }
            model.lstProducts = db.proc_DthPlanList(PlanId, SetTopBoxID, null, null).Where(a => a.IsActive == true).ToList();
            if (SortBy != null && !string.IsNullOrWhiteSpace(SortBy) && model.lstProducts.Count > 0)
            {
                if (SortBy.Contains("Date"))
                    model.lstProducts = model.lstProducts.OrderByDescending(a => a.CreatedOn).ToList();
                else if (SortBy.Contains("PriceLow"))
                    model.lstProducts = model.lstProducts.OrderBy(a => a.OfferePrice).ToList();
                else if (SortBy.Contains("PriceHigh"))
                    model.lstProducts = model.lstProducts.OrderByDescending(a => a.OfferePrice).ToList();
            }
            return PartialView(model);
        }

        /// <summary>
        /// Ek specific DTH plan ka detail view dikhata hai
        /// </summary>
        public ActionResult ProductView(int id, string type)
        {
            try
            {
                var model = db.proc_DthPlanList(id, null, null, null).SingleOrDefault();
                return View(model);
            }
            catch
            {
                return View();
            }

        }
        [HttpGet]
        /// <summary>
        /// Payment process shuru karta hai, selected DTH plan ka data load karta hai
        /// </summary>
        public ActionResult ProcessToPay(int? Dthid)
        {
            try
            {
                if (Dthid == null)
                {
                    return RedirectToAction("Index");
                }
                ViewBag.state = new SelectList(db.Select_State_Details(), "State_Id", "State_Name").ToList();
                var Data = db.DthPlanAndSpecifications.Where(a => a.Id == Dthid).SingleOrDefault();
                if (Data == null)
                {
                    return RedirectToAction("Index");
                }
                ViewBag.PlanData = Data;
                return View();
            }
            catch
            {
                return View();
            }
        }
        [HttpPost]
        /// <summary>
        /// DTH booking ka final payment process karta hai aur balance deduct karta hai
        /// </summary>
        public ActionResult ProcessToPayFinal(int idno, string txtname, string txtEmail, string txtmobile, string txtpincode, string State1, string District1, string txtAddress)
        {
            try
            {
                var userid = User.Identity.GetUserId();
                var Data = db.DthPlanAndSpecifications.Where(a => a.Id == idno).ToList();
                if (Data.Count > 0)
                {
                    System.Data.Entity.Core.Objects.ObjectParameter output = new
                       System.Data.Entity.Core.Objects.ObjectParameter("Output", typeof(string));
                    var ch = db.Proc_DthBookingPayment(userid, txtname, txtEmail, txtmobile, txtpincode, State1, District1, txtAddress, idno, output).Single().msg;
                    try
                    {
                        var retailerdetails = db.Retailer_Details.Where(aa => aa.RetailerId == userid).SingleOrDefault();
                        var dealerdetails = db.Dealer_Details.Where(aa => aa.DealerId == retailerdetails.DealerId).SingleOrDefault();
                        var masterdetails = db.Superstokist_details.Where(aa => aa.SSId == dealerdetails.SSId).SingleOrDefault();

                        var remdetails = db.Remain_reteller_balance.Where(aa => aa.RetellerId == userid).SingleOrDefault();
                        var dlmdetails = db.Remain_dealer_balance.Where(aa => aa.DealerID == retailerdetails.DealerId).SingleOrDefault();
                        var Masterdetails = db.Remain_superstokist_balance.Where(aa => aa.SuperStokistID == dealerdetails.SSId).SingleOrDefault();

                        var admininfo = db.Admin_details.SingleOrDefault();
                        Backupinfo back = new Backupinfo();
                        var model = new Backupinfo.Addinfo
                        {
                            Websitename = admininfo.WebsiteUrl,
                            RetailerID = userid,
                            Email = retailerdetails.Email,
                            Mobile = retailerdetails.Mobile,
                            Details = "DTH Booking Payment ",
                            RemainBalance = (decimal)remdetails.Remainamount,
                            Usertype = "Retailer"
                        };
                        back.info(model);

                        var model1 = new Backupinfo.Addinfo
                        {
                            Websitename = admininfo.WebsiteUrl,
                            RetailerID = dealerdetails.DealerId,
                            Email = dealerdetails.Email,
                            Mobile = dealerdetails.Mobile,
                            Details = "DTH Booking Payment ",
                            RemainBalance = Convert.ToDecimal(dlmdetails.Remainamount),
                            Usertype = "Dealer"
                        };
                        back.info(model1);

                        var model2 = new Backupinfo.Addinfo
                        {
                            Websitename = admininfo.WebsiteUrl,
                            RetailerID = masterdetails.SSId,
                            Email = masterdetails.Email,
                            Mobile = masterdetails.Mobile,
                            Details = "DTH Booking Payment ",
                            RemainBalance = Convert.ToDecimal(Masterdetails.Remainamount),
                            Usertype = "Master"
                        };
                        back.info(model2);
                    }
                    catch { }
                    if (ch == "DTH Connection Booking Success.")
                    {
                        TempData["Status"] = "Success";
                        TempData["Message"] = "DTH Connection Booking Processed Successfully.";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["Status"] = "Failed";
                        TempData["Message"] = ch;
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    TempData["Status"] = "Failed";
                    TempData["Message"] = "Something Wrong.";
                    return RedirectToAction("ProcessToPay");
                }
            }
            catch (Exception ex)
            {
                TempData["Status"] = "Failed";
                TempData["Message"] = ex.Message;
                return RedirectToAction("ProcessToPay");
            }
        }
        /// <summary>
        /// State ID ke basis par district list JSON mein return karta hai
        /// </summary>
        public JsonResult DistrictList(int Id)
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                var district = from s in db.District_Desc
                               where s.State_id == Id
                               select s;
                ViewBag.district = new SelectList(district, "Dist_id", "Dist_Desc").ToList();
                return Json(new SelectList(district.ToArray(), "Dist_id", "Dist_Desc"), JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// DTH connection bookings ki report aaj ki date ke liye dikhata hai
        /// </summary>
        public ActionResult DthBookingReport()
        {
            var userid = User.Identity.GetUserId();
            string txt_frm_date = DateTime.Now.ToString();
            string txt_to_date = DateTime.Now.ToString();
            string frm_date = Convert.ToDateTime(txt_frm_date).Date.ToString("yyyy-MM-dd");
            string to_date = Convert.ToDateTime(txt_to_date).AddDays(1).ToString("yyyy-MM-dd");
            var ch = db.DTHConnection_Report_New("Retailer", userid, "ALL", 50, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            ViewData["totals"] = ch.Where(s => s.Status == "Success").Sum(s => Convert.ToInt32(s.Amount));
            ViewData["Totalf"] = ch.Where(s => s.Status == "Failed").Sum(s => Convert.ToInt32(s.Amount));
            return View(ch);
        }
        [HttpPost]
        /// <summary>
        /// Filter ke saath DTH booking report dikhata hai - date range aur status ke hisaab se
        /// </summary>
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
            var ch = db.DTHConnection_Report_New("Retailer", userid, ddl_status, ddltop, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            ViewData["totals"] = ch.Where(s => s.Status == "Success").Sum(s => Convert.ToInt32(s.Amount));
            ViewData["Totalf"] = ch.Where(s => s.Status == "Failed").Sum(s => Convert.ToInt32(s.Amount));
            return View(ch);
        }

    }
}