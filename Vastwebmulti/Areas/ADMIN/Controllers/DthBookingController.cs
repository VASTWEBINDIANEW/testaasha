using Microsoft.AspNet.Identity.Owin;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vastwebmulti.Areas.ADMIN.Models;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.ADMIN.Controllers
{
    /// <summary>
    /// ADMIN area - DTH plans ka management karta hai: list, add, edit aur delete
    /// </summary>
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

        /// <summary>
        /// GET - Sabhi active DTH plans ki list dikhata hai
        /// </summary>
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

        /// <summary>
        /// POST - Operator ID ke basis par DTH plans filter karke dikhata hai
        /// </summary>
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

        /// <summary>
        /// GET - Naya DTH plan add karne ka form dikhata hai
        /// </summary>
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

        /// <summary>
        /// POST - Naya DTH plan diye gaye details aur specifications ke saath save karta hai
        /// </summary>
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
                        entry.MappingId = db.DthOperatorsSetTopBoxMappings.FirstOrDefault(a => a.OperatorId == optId && a.SetTopBoxId == BoxTypeId)?.Id ?? 0;
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

        /// <summary>
        /// POST - Existing DTH plan ki pricing aur specification update karta hai
        /// </summary>
        [HttpPost]
        public ActionResult EditDtdPlan(int Id, string title, string PlanName, decimal OfferPrice, decimal PublishedPrice, string specifications)
        {
            try
            {
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    if (!string.IsNullOrWhiteSpace(title) && !string.IsNullOrWhiteSpace(PlanName) && !string.IsNullOrWhiteSpace(specifications))
                    {
                        DthPlanAndSpecification entry = db.DthPlanAndSpecifications.FirstOrDefault(a => a.Id == Id);
                        if (entry != null)
                        {
                            entry.OfferePrice = OfferPrice;
                            entry.PlanName = PlanName;
                            entry.PublishePrice = PublishedPrice;
                            entry.Specification = specifications;
                            entry.Title = title;
                            db.SaveChanges();
                        }
                    }

                    return RedirectToAction("Index");
                }
            }
            catch
            {
                return View();
            }
        }

        /// <summary>
        /// GET - DTH plan ko soft-delete karke list page par redirect karta hai
        /// </summary>
        [HttpGet]
        public ActionResult DeleteDthPlan(int Id)
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                var item = db.DthPlanAndSpecifications.FirstOrDefault(a => a.Id == Id);
                if (item != null)
                {
                    item.IsDeleted = true;
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
        }

    }
}
