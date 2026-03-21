using Microsoft.AspNet.Identity.Owin;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vastwebmulti.Areas.ADMIN.Models;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.ADMIN.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DthBookingController : Controller
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
        // GET: RETAILER/DthBooking
        public ActionResult Index()
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                return View(db.DthPlanAndSpecifications.Where(a => a.IsDeleted == false).Select(a => new dthconnectionModel
                {

                    CreatedOn = a.CreatedOn,
                    Id = a.Id,
                    ImageUrl = a.DthOperatorsSetTopBoxMapping.DthOperator.ImgUrl,
                    IsActive = a.IsActive,
                    IsDeleted = a.IsDeleted,
                    MappingId = a.MappingId,
                    OfferePrice = a.OfferePrice,
                    PlanName = a.PlanName,
                    PublishePrice = a.PublishePrice,
                    Specification = a.Specification,
                    Title = a.Title
                }).ToList());
            }
        }
        [HttpPost]
        public ActionResult Index(int Opt)
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                return View(db.DthPlanAndSpecifications.Where(a => a.DthOperatorsSetTopBoxMapping.DthOperator.Id == Opt && a.IsDeleted == false).Select(a => new dthconnectionModel
                {
                    CreatedOn = a.CreatedOn,
                    Id = a.Id,
                    ImageUrl = a.DthOperatorsSetTopBoxMapping.DthOperator.ImgUrl,
                    IsActive = a.IsActive,
                    IsDeleted = a.IsDeleted,
                    MappingId = a.MappingId,
                    OfferePrice = a.OfferePrice,
                    PlanName = a.PlanName,
                    PublishePrice = a.PublishePrice,
                    Specification = a.Specification,
                    Title = a.Title
                }).ToList());
            }
        }

        public ActionResult AddDtdPlan()
        {
            try
            {
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {


                    ViewBag.ErrMsg = "";
                    return View();
                }
            }
            catch
            {
                return View();
            }
        }
        [HttpPost]
        public ActionResult AddDtdPlan(string title, string PlanName, decimal OfferPrice, decimal PublishedPrice, int optId, int BoxTypeId, string[] specifications)
        {
            try
            {
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    if (!string.IsNullOrWhiteSpace(title) && !string.IsNullOrWhiteSpace(PlanName) && specifications != null)
                    {

                        DthPlanAndSpecification entry = new DthPlanAndSpecification();
                        entry.CreatedOn = DateTime.Now;
                        entry.IsActive = true;
                        entry.IsDeleted = false;
                        entry.MappingId = db.DthOperatorsSetTopBoxMappings.SingleOrDefault(a => a.OperatorId == optId && a.SetTopBoxId == BoxTypeId).Id;
                        entry.OfferePrice = OfferPrice;
                        entry.PlanName = PlanName;
                        entry.PublishePrice = PublishedPrice;
                        entry.Specification = string.Join(",", specifications);
                        entry.Title = title;
                        db.DthPlanAndSpecifications.Add(entry);
                        db.SaveChanges();
                        ViewBag.ErrMsg = "Plan saved successfully";

                    }
                    else
                    {
                        ViewBag.ErrMsg = "Invalid request.Please fill all mandatory fields.";
                    }
                    return View();
                }
            }
            catch
            {
                return View();
            }
        }
        [HttpPost]
        public ActionResult EditDtdPlan(int Id, string title, string PlanName, decimal OfferPrice, decimal PublishedPrice, string specifications)
        {
            try
            {
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    if (!string.IsNullOrWhiteSpace(title) && !string.IsNullOrWhiteSpace(PlanName) && !string.IsNullOrWhiteSpace(specifications))
                    {
                        DthPlanAndSpecification entry = db.DthPlanAndSpecifications.SingleOrDefault(a => a.Id == Id);
                        entry.OfferePrice = OfferPrice;
                        entry.PlanName = PlanName;
                        entry.PublishePrice = PublishedPrice;
                        entry.Specification = specifications;
                        entry.Title = title;
                        db.SaveChanges();
                    }

                    return RedirectToAction("Index");
                }
            }
            catch
            {
                return View();
            }
        }

        [HttpGet]
        public ActionResult DeleteDthPlan(int Id)
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                var item = db.DthPlanAndSpecifications.SingleOrDefault(a => a.Id == Id);
                item.IsDeleted = true;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
        }

    }
}