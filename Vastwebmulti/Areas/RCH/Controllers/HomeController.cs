using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vastwebmulti.Models;
using System.Data;
using System.Data.SqlClient;
using System.Web.Security;
using Vastwebmulti.Areas.ADMIN.Models;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Configuration;
using System.Threading;
using System.Web.Helpers;


namespace Vastwebmulti.Areas.RCH.Controllers
{
    [Authorize(Roles = "RCH")]
    /// <summary>
    /// RCH Area - Handles recharge processing callbacks, status updates and transaction reconciliation
    /// </summary>
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
        // GET: RCH/Home
        VastwebmultiEntities db = new VastwebmultiEntities();
        /// <summary>
        /// [GET] - Displays today's manual recharge transactions with success/failure/pending summary counts
        /// </summary>
        public ActionResult Index()
        {
            string userid = User.Identity.GetUserId();
            //string userid = Membership.GetUser(User.Identity.Name).UserName.ToString();
            var ch = db.maual_rch_details(userid, DateTime.Now, DateTime.Now, "", null).ToList();
            ViewData["snsucc"] = ch.Where(a => a.Rstaus == "SUCCESS").Count();
            ViewData["snsamt"] = ch.Where(a => a.Rstaus == "SUCCESS").Sum(a => a.amount);
            ViewData["snfai"] = ch.Where(a => a.Rstaus == "FAILED").Count();
            ViewData["snfamt"] = ch.Where(a => a.Rstaus == "FAILED").Sum(a => a.amount);
            ViewData["snpai"] = ch.Where(a => a.Rstaus == "Request Send").Count();
            ViewData["snpamt"] = ch.Where(a => a.Rstaus == "Request Send").Sum(a => a.amount);
            ViewData["snpai12"] = ch.Where(a => a.Rstaus == "Resend").Count();
            ViewData["snpamt12"] = ch.Where(a => a.Rstaus == "Resend").Sum(a => a.amount);
            return View(ch);
        }

        [HttpPost]
        /// <summary>
        /// [POST] - Filters and displays manual recharge transactions by date range and status
        /// </summary>
        public ActionResult Index(string txt_frm_date, string txt_to_date, string DDLsts, string txtamount)
        {
            if (txtamount == null || txtamount == "")
            {
                txtamount = "0";
            }
            if (txt_frm_date != null && txt_to_date != null && txt_frm_date != "" && txt_to_date != "")
            {
                string userid = User.Identity.GetUserId();
                string[] words = txt_frm_date.Split('-');
                string year = words[0].ToString();
                string month = words[1].ToString();
                string date = words[2].ToString();
                string dtfrm = year + "-" + month + "-" + date;
                DateTime frm_date = Convert.ToDateTime(dtfrm);
                words = txt_to_date.Split('-');
                year = words[0].ToString();
                month = words[1].ToString();
                date = words[2].ToString();
                string dtto = year + "-" + month + "-" + date;

                string to_date = Convert.ToDateTime(dtto).AddDays(1).ToString("yyyy-MM-dd");


                if (DDLsts == "ALL")
                {
                    DDLsts = "";
                }
                else if (DDLsts == "PENDING")
                {
                    DDLsts = "Request Send";
                }
                var ch = db.maual_rch_details(userid, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), DDLsts, "B").ToList();

                ViewData["snsucc"] = ch.Where(a => a.Rstaus == "SUCCESS").Count();
                ViewData["snsamt"] = ch.Where(a => a.Rstaus == "SUCCESS").Sum(a => a.amount);
                ViewData["snfai"] = ch.Where(a => a.Rstaus == "FAILED").Count();
                ViewData["snfamt"] = ch.Where(a => a.Rstaus == "FAILED").Sum(a => a.amount);
                ViewData["snpai"] = ch.Where(a => a.Rstaus == "Request Send").Count();
                ViewData["snpamt"] = ch.Where(a => a.Rstaus == "Request Send").Sum(a => a.amount);
                return View(ch);
            }
            else
            {

                if (DDLsts == "ALL")
                {
                    DDLsts = "";
                }
                else if (DDLsts == "PENDING")
                {
                    DDLsts = "Request Send";
                }
                string userid = User.Identity.GetUserId();

                var ch = db.maual_rch_details(userid, DateTime.Now, DateTime.Now, DDLsts, null).ToList();

                ViewData["snsucc"] = ch.Where(a => a.Rstaus == "SUCCESS").Count();
                ViewData["snsamt"] = ch.Where(a => a.Rstaus == "SUCCESS").Sum(a => a.amount);
                ViewData["snfai"] = ch.Where(a => a.Rstaus == "FAILED").Count();
                ViewData["snfamt"] = ch.Where(a => a.Rstaus == "FAILED").Sum(a => a.amount);
                ViewData["snpai"] = ch.Where(a => a.Rstaus == "Request Send").Count();
                ViewData["snpamt"] = ch.Where(a => a.Rstaus == "Request Send").Sum(a => a.amount);
                return View(ch);

            }
        }
        /// <summary>
        /// [GET] - Marks a recharge transaction as SUCCESS and redirects to the dashboard
        /// </summary>
        public ActionResult success(string idno)
        {
            try
            {
                var today_recharge = db.Recharge_info.Where(s => s.idno == Convert.ToInt32(idno)).ToList();
                if (today_recharge.Count == 1)
                {
                    db.recharge_update(idno, "SUCCESS", "BY RCH", 0, "", "Response");

                }
                else
                {
                    db.recharge_update_old(idno, "SUCCESS", "BY RCH", 0, "", "Response");

                }

                return RedirectToAction("Index");


            }
            catch
            {
                return RedirectToAction("Index");
            }
        }
        /// <summary>
        /// [GET] - Marks a recharge transaction as FAILED and redirects to the dashboard
        /// </summary>
        public ActionResult failed(string fid)
        {
            try
            {
                var sdk = Convert.ToInt32(fid);
                var today_recharge = db.Recharge_info.Where(s => s.idno == sdk).ToList();
                if (today_recharge.Count == 1)
                {
                    db.recharge_update(fid, "FAILED", "BY RCH", 0, "", "Response");

                }
                else
                {
                    db.recharge_update_old(fid, "FAILED", "BY RCH", 0, "", "Response");

                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }
        }
        /// <summary>
        /// [GET] - Resends a pending recharge request via the SRS system
        /// </summary>
        public ActionResult resend(string fid)
        {
            try
            {
                System.Data.Entity.Core.Objects.ObjectParameter output = new
                                   System.Data.Entity.Core.Objects.ObjectParameter("Output", typeof(string));
                var ch = db.SRS_Rch_Resend(fid, output).Single().msg.ToString();
                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }
        /// <summary>
        /// [GET] - Marks a recharge transaction as SUCCESS and redirects to the dashboard
        /// </summary>
        public ActionResult failed_to_success(string fid)
        {
            try
            {

                var sdk = Convert.ToInt32(fid);
                var today_recharge = db.Recharge_info.Where(s => s.idno == sdk).ToList();
                if (today_recharge.Count == 1)
                {
                    db.recharge_update_failed_to_success(sdk, "Manual Success");

                }
                else
                {

                    db.recharge_update_failed_to_success_old(sdk, "Manual Success");
                }

                return RedirectToAction("Index");




            }
            catch
            {
                return RedirectToAction("Index");
            }
        }


        /// <summary>
        /// [GET] - Renders the RCH operator dashboard view
        /// </summary>
        public ActionResult Dashboard()
        {
            return View();
        }

        /// <summary>
        /// [GET] - Adds or updates a manual operator configuration for the RCH user
        /// </summary>
        public ActionResult submit_operator1(string Operatorids, string opnamelistbyid, int? textminiamount, int? textmaxamount)
        {
            try
            {
                string userid = User.Identity.GetUserId();
                var lists = db.Manual_opt_sts.Where(s => s.@operator == opnamelistbyid && s.respID == userid).ToList();
                if (!lists.Any())
                {


                    Manual_opt_sts st = new Manual_opt_sts();
                    st.@operator = opnamelistbyid;
                    st.minamount = textminiamount;
                    st.maxamount = textmaxamount;
                    st.portname = "ALL";
                    st.status = "Y";
                    st.respID = userid;
                    db.Manual_opt_sts.Add(st);
                    db.SaveChanges();
                }
                else
                {
                    var lists1 = db.Manual_opt_sts.Where(s => s.@operator == opnamelistbyid && s.respID == userid).SingleOrDefault();
                    lists1.minamount = textminiamount;
                    lists1.maxamount = textmaxamount;
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                TempData["message1"] = "Please Try Again";
                return RedirectToAction("addoperaterforrecharge", "Home");
            }



            return RedirectToAction("Operator_Manage", "Home");
        }
        /// <summary>
        /// [GET] - JSON - returns all operator codes filtered by operator type for dropdown binding
        /// </summary>
        public JsonResult FillAllOperator(string opname)
        {
            var type = db.Operator_Code.Where(x => x.Operator_type == opname).ToList();
            IEnumerable<SelectListItem> selecttypetlist = from g in type
                                                          select new SelectListItem
                                                          {
                                                              Value = g.new_opt_code,
                                                              Text = g.operator_Name
                                                          };
            ViewBag.optypelist = new SelectList(selecttypetlist, "Value", "Text");
            return Json(ViewBag.optypelist, JsonRequestBehavior.AllowGet);
        }

        public void statesw()
        {
           
        }

        /// <summary>
        /// [GET] - Displays the add-operator form for recharge configuration
        /// </summary>
        public ActionResult addoperaterforrecharge()
        {
            ViewData["mesage1"] = TempData["message1"];

            return View();
        }
        /// <summary>
        /// [GET] - Shows all manually configured operators for the current RCH user
        /// </summary>
        public ActionResult Operator_Manage()
        {
            string userid = User.Identity.GetUserId();
            var ch = db.Select_manual_opt(userid);
            return View(ch);
        }
        [HttpPost]
        /// <summary>
        /// [POST] - Bulk-enables or bulk-disables all manually configured operators
        /// </summary>
        public ActionResult Operator_manage(string but)
        {
            string userid = User.Identity.GetUserId();
            if (but == "All Block")
            {
                db.Manual_opt_sts.Where(x => x.respID == userid).ToList().ForEach(x =>
                {
                    x.status = "N";
                });
                db.SaveChanges();
            }
            else if (but == "All Unblock Block")
            {
                db.Manual_opt_sts.Where(x => x.respID == userid).ToList().ForEach(x =>
                {
                    x.status = "Y";
                });
                db.SaveChanges();
            }
            var ch = db.Select_manual_opt(userid);
            return View(ch);
        }
        /// <summary>
        /// [GET] - Toggles the active/inactive status of a single manual operator entry
        /// </summary>
        public ActionResult manualedit(string id, string sts)
        {
            try
            {
                int idd = Convert.ToInt32(id);
                if (sts == "Y")
                {
                    sts = "N";
                }
                else
                {
                    sts = "Y";
                }
                Manual_opt_sts objCourse = (from p in db.Manual_opt_sts
                                            where p.id == idd
                                            select p).SingleOrDefault();
                objCourse.status = sts;
                db.SaveChanges();
                return RedirectToAction("Operator_manage");
            }
            catch
            {
                return RedirectToAction("Operator_manage");
            }
        }

        /// <summary>
        /// [GET] - Renders the change-password form for the RCH user
        /// </summary>
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
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
        //Profile
        [HttpGet]
        /// <summary>
        /// [GET] - Displays the user profile details
        /// </summary>
        public new ActionResult Profile()
        {
            string userid = User.Identity.GetUserId();
            var userDetails = db.Users.Where(a => a.UserId == userid).SingleOrDefault();
            var ch = db.RCH_Details.FirstOrDefault(m => m.RCHId == userid);
            //show  retailer Profile details
            ViewBag.rch = ch;
            ViewBag.image = ch.Photo;
            int state = Convert.ToInt32(ch.State);
            int district = Convert.ToInt32(ch.District);
            var gt = db.State_Desc.Where(a => a.State_id == state).SingleOrDefault().State_name;
            ViewBag.ddlstate = gt;
            var cities = db.District_Desc.Where(c => c.Dist_id == district && c.State_id == state).SingleOrDefault().Dist_Desc;
            ViewBag.district = cities;
            //emd
            //Shwo Admin Details

            var admindetails = db.Admin_details.FirstOrDefault();
            ViewBag.admin = admindetails;
            var editstate = db.State_Desc.ToList();
            ViewBag.ddlstate1 = editstate;
            var editedistrict = db.District_Desc.Where(c => c.State_id == state).ToList();
            ViewBag.cities1 = editedistrict;
            return View(userDetails);
        }

        //Edit Profile 
        /// <summary>
        /// [GET] - Displays the RCH user profile edit form
        /// </summary>
        public ActionResult Edit_Profile(string RchId)
        {

            var show = db.RCH_Details.Find(RchId);
            ViewBag.Sate = db.State_Desc.Select(a => new SelectListItem { Text = a.State_name, Value = a.State_id.ToString() }).ToList();
            ViewBag.District = db.District_Desc.Where(a => a.State_id.ToString() == show.State).Select(a => new SelectListItem { Text = a.Dist_Desc, Value = a.Dist_id.ToString() }).ToList();
            return View(show);
        }
        [HttpPost]
        /// <summary>
        /// [POST] - Saves updated profile information including optional photo upload
        /// </summary>
        public ActionResult Edit_Profile(string RchId, RCH_Details rchdetails)
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
                var rch = db.RCH_Details.Single(a => a.RCHId == RchId);
                rch.RCHName = string.IsNullOrWhiteSpace(rchdetails.RCHName) ? "" : rchdetails.RCHName;
                rch.Email = string.IsNullOrWhiteSpace(rchdetails.Email) ? "" : rchdetails.Email;
                rch.Mobile = string.IsNullOrWhiteSpace(rchdetails.Mobile) ? "" : rchdetails.Mobile;
                rch.FarmName = string.IsNullOrWhiteSpace(rchdetails.FarmName) ? "" : rchdetails.FarmName;
                rch.Address = string.IsNullOrWhiteSpace(rchdetails.Address) ? "" : rchdetails.Address;
                rch.Pincode = rchdetails.Pincode;
                rch.State = rchdetails.State;
                rch.District = rchdetails.District;
                rch.Photo = string.IsNullOrWhiteSpace(imagePath) ? rch.Photo : imagePath;
                db.SaveChanges();
                ViewBag.Sate = db.State_Desc.Select(a => new SelectListItem { Text = a.State_name, Value = a.State_id.ToString() }).ToList();
                ViewBag.District = db.District_Desc.Select(a => new SelectListItem { Text = a.Dist_Desc, Value = a.Dist_id.ToString() }).ToList();
                return RedirectToAction("Profile");
            }
            catch
            {

            }

            return RedirectToAction("Profile");
        }

    }
}