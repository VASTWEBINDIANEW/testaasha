using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vastwebmulti.Areas.WRetailer.Controllers;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.WRetailer.Controllers
{
 
    [Authorize(Roles = "Whitelabelretailer")]
    /// <summary>
    /// Is class ka kaam DTHController area ke operations handle karna hai.
    /// </summary>
    public class DTHController : Controller
    {

        // GET: RETAILER/DTH
        /// <summary>
        /// DTH recharge/booking handle karta hai.
        /// </summary>
        public ActionResult DTHBooking()
        {
            return View();
        }
        [HttpPost]
        /// <summary>
        /// Data fetch karke view mein dikhata hai.
        /// </summary>
        public ActionResult GetPackageDetails(string Code)
        {
            string respo = new InstantPayComnUtil().getOperatorDetails(Code);
            if (respo != null && respo != "ERROR")
            {
                //DTH_Con model = new JavaScriptSerializer().Deserialize<DTH_Con>(respo);

                return Json(respo);
            }
            return RedirectToAction("DTHBooking");
        }
        [HttpPost]
        /// <summary>
        /// Is action ka kaam 'DoPayment' se related operation handle karna hai.
        /// </summary>
        public ActionResult DoPayment(string STB, string ConOpt, string ddlPackage, string packageAmt, string txtName, string txtMobile, string customerAddress, string txtPIN)
        {
            try
            {

               string userid= User.Identity.GetUserId();
                var respo = new InstantPayComnUtil().doPayment(userid, STB, ConOpt, ddlPackage, packageAmt, txtName, txtMobile, customerAddress, txtPIN);
                if (respo != "ERROR")
                {
                    return RedirectToAction("DTHBooking");
                }
                else
                {
                    return RedirectToAction("DTHBooking");
                }

            }
            catch 
            {
                return RedirectToAction("DTHBooking");
            }

        }
    }
}