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
    /// ADMIN Area - Manages DTH plan listings, creation, editing, and deletion
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
        /// GET - Displays all active DTH plans.
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
        /// POST - Filters and displays DTH plans by operator ID.
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
        /// GET - Shows the form to add a new DTH plan.
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
        /// POST - Saves a new DTH plan with the provided details and specifications.
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
        /// POST - Updates an existing DTH plan's pricing and specification details.
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
        /// GET - Soft-deletes a DTH plan by marking it as deleted and redirects to the list.
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
