using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.RETAILER.Controllers
{

    /// <summary>
    /// RETAILER Area - Handles retailer KYC submission, outlet registration, KYC document upload, and PAN card management via the InstantPay API.
    /// </summary>
    [Authorize(Roles = "Retailer")]
    [Low_Bal_CustomFilter()]
    public class OutletController : Controller
    {
        // GET: RETAILER/Outlet
        /// <summary>
        /// GET Loads and displays the retailer KYC form pre-populated with the retailer's state and district information.
        /// </summary>
        /// <returns>The retailer KYC view with current retailer details and state/city dropdowns.</returns>
        [HttpGet]
        public ActionResult RetailerKYC()
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                string userid = User.Identity.GetUserId();
                Retailer_Details ch = db.Retailer_Details.Where(m => m.RetailerId == userid).Single();
                var gt = db.State_Desc.AsNoTracking().ToList();
                ViewBag.ddlstate = gt;
                var cities = db.District_Desc.AsNoTracking().Where(c => c.State_id == ch.State).ToList();
                ViewBag.cities = cities;
                return View(ch);
            }
        }
        /// <summary>
        /// POST Sends a mobile number to the InstantPay API to initiate outlet verification via OTP.
        /// </summary>
        /// <param name="Mobile">The retailer's mobile number to verify.</param>
        /// <returns>A JSON result containing the verification response or an error message.</returns>
        [HttpPost]
        public JsonResult VerifyOutlet(string Mobile)
        {
            try
            {
                InstantPayComnUtil util = new InstantPayComnUtil();
                var response = util.VerifyOutletMobile(Mobile);
                return Json(response.ToString(), JsonRequestBehavior.AllowGet);


            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        /// <summary>
        /// POST Registers the retailer as an outlet on the InstantPay platform using the provided OTP and retailer details.
        /// </summary>
        /// <param name="RetailerId">The unique retailer identifier.</param>
        /// <param name="Mobile">The retailer's mobile number.</param>
        /// <param name="OTP">The one-time password received for verification.</param>
        /// <param name="email">The retailer's email address.</param>
        /// <param name="store_type">The type of store being registered.</param>
        /// <param name="Frm_Name">The firm/company name.</param>
        /// <param name="RetailerName">The owner's name.</param>
        /// <param name="pincode">The postal/PIN code of the store location.</param>
        /// <param name="address">The full address of the store.</param>
        /// <returns>A JSON result containing the registration response or an error message.</returns>
        [HttpPost]
        public ActionResult RegisterOutlet(string RetailerId, string Mobile, string OTP, string email, string store_type, string Frm_Name, string RetailerName, string pincode, string address)
        {
            try
            {
                InstantPayComnUtil util = new InstantPayComnUtil();
                var response = util.RegisterOutlet(RetailerId, Mobile, OTP, email, store_type, Frm_Name, RetailerName, pincode, address);
                return Json(response.ToString(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        /// <summary>
        /// POST Uploads a KYC document file (as base64) to the InstantPay outlet API for the specified retailer.
        /// </summary>
        /// <param name="RetailerId">The unique retailer identifier.</param>
        /// <param name="DocId">The document type identifier required by InstantPay.</param>
        /// <param name="PanCard">The retailer's PAN card number for verification.</param>
        /// <param name="file">The uploaded KYC document file.</param>
        /// <returns>A JSON result indicating success or failure with a descriptive message.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
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

        /// <summary>
        /// POST Updates the retailer's PAN card number via the InstantPay utility.
        /// </summary>
        /// <param name="RetailerID">The unique retailer identifier.</param>
        /// <param name="PanCard">The new PAN card number to update.</param>
        /// <returns>A JSON result containing the update response or an error message.</returns>
        [HttpPost]
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

        /// <summary>
        /// POST Retrieves the KYC document list and approval status for the specified retailer from the InstantPay API.
        /// </summary>
        /// <param name="RetailerID">The unique retailer identifier.</param>
        /// <param name="PanCard">The retailer's PAN card number for verification.</param>
        /// <returns>A JSON result containing the KYC documents and their current status.</returns>
        [HttpPost]
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
        /// Signs out the current user and redirects to the login page.
        /// </summary>
        /// <returns>A redirect to the Account/Login action.</returns>
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account", new { area = "" });
        }
    }
}