using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.RETAILER.Controllers
{

    [Authorize(Roles = "Retailer")]
    [Low_Bal_CustomFilter()]
    public class OutletController : Controller
    {
        // GET: RETAILER/Outlet
        [HttpGet]
        public ActionResult RetailerKYC()
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                string userid = User.Identity.GetUserId();
                Retailer_Details ch = db.Retailer_Details.Where(m => m.RetailerId == userid).Single();
                var gt = db.State_Desc.ToList();
                ViewBag.ddlstate = gt;
                var cities = db.District_Desc.Where(c => c.State_id == ch.State).ToList();
                ViewBag.cities = cities;
                return View(ch);
            }
        }
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
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account", new { area = "" });
        }
    }
}