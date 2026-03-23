using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Vastwebmulti.Areas.WRetailer.Models;
using Vastwebmulti.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Globalization;
using System.Web.Security;

namespace Vastwebmulti.Areas.WRetailer.Controllers
{
    
    [Authorize(Roles = "Whitelabelretailer")]
    /// <summary>
    /// Is class ka kaam OutletController area ke operations handle karna hai.
    /// </summary>
    public class OutletController : Controller
    {
        // GET: RETAILER/Outlet
        [HttpGet]
        /// <summary>
        /// Retailer se related data handle karta hai.
        /// </summary>
        public ActionResult RetailerKYC()
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                string userid = User.Identity.GetUserId();
                Whitelabel_Retailer_Details ch = db.Whitelabel_Retailer_Details.Where(m => m.RetailerId == userid).Single();
                var gt = db.State_Desc.AsNoTracking().ToList();
                ViewBag.ddlstate = gt;
                var cities = db.District_Desc.AsNoTracking().Where(c => c.State_id == ch.State).ToList();
                ViewBag.cities = cities;
                return View(ch);
            }
        }
        [HttpPost]
        /// <summary>
        /// Data ya status verify/check karta hai.
        /// </summary>
        public JsonResult VerifyOutlet(string Mobile)
        {
            try
            {
                InstantPayComnUtil util = new InstantPayComnUtil();
                var response = util.VerifyOutletMobile(Mobile);
                return Json(response.ToString());


            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        [HttpPost]
        /// <summary>
        /// Outlet registration ya management karta hai.
        /// </summary>
        public ActionResult RegisterOutlet(string RetailerId, string Mobile, string OTP, string email, string store_type, string Frm_Name, string RetailerName, string pincode, string address)
        {
            try
            {
                InstantPayComnUtil util = new InstantPayComnUtil();
                var response = util.RegisterOutlet(RetailerId, Mobile, OTP, email, store_type, Frm_Name, RetailerName, pincode, address);
                return Json(response.ToString());
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        /// <summary>
        /// File ya document upload karta hai.
        /// </summary>
        public ActionResult Upload_KYC_Doc(string RetailerId, string DocId, string PanCard, HttpPostedFileBase file)
        {
            try
            {
                if (file != null)
                {
                    if (string.IsNullOrWhiteSpace(PanCard))
                        return Json("Oops!! \n InValid Pan,Please Update PAN first.");
                    System.IO.Stream MyStream;
                    byte[] input = new byte[file.ContentLength];

                    // Initialize the stream.
                    MyStream = file.InputStream;

                    // Read the file into the byte array.
                    MyStream.Read(input, 0, file.ContentLength);

                    // Copy the byte array into a string.
                    string base64Content = Convert.ToBase64String(input);

                    InstantPayComnUtil util = new InstantPayComnUtil();
                    var response = util.Upload_KYC_Doc(RetailerId, DocId, PanCard, base64Content, file.FileName);
                    return Json(response.ToString());
                }
                else
                {
                    return Json("File Not Uploaded!");
                }

            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        [HttpPost]
        /// <summary>
        /// Existing record ko update/edit karta hai.
        /// </summary>
        public ActionResult UpdatePancard(string RetailerID, string PanCard)
        {
            try
            {
                InstantPayComnUtil util = new InstantPayComnUtil();
                var response = util.UpdatePAN(RetailerID, PanCard);
                return Json(response.ToString());
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        [HttpPost]
        /// <summary>
        /// Data fetch karke view mein dikhata hai.
        /// </summary>
        public ActionResult ViewKYCDocsAndStatus(string RetailerID, string PanCard)
        {
            try
            {
                InstantPayComnUtil util = new InstantPayComnUtil();
                var response = util.GetKycDoc(RetailerID, PanCard);
                return Json(response.ToString());
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }
        /// <summary>
        /// User login ya logout handle karta hai.
        /// </summary>
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account", new { area = "" });
        }
    }
}