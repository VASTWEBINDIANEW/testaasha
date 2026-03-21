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
    public class DTHController : Controller
    {

        // GET: RETAILER/DTH
        public ActionResult DTHBooking()
        {
            return View();
        }
        [HttpPost]
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