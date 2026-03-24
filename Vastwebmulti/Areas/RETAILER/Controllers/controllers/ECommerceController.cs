using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vastwebmulti.Areas.RETAILER.Models;
using Vastwebmulti.Areas.RETAILER.ViewModels;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.RETAILER.Controllers
{

    [Authorize(Roles = "Retailer")]
    [Low_Bal_CustomFilter()]
    /// <summary>
    /// Retailer ke liye eCommerce product listing, cart aur order management handle karta hai
    /// </summary>
    public class ECommerceController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        VastwebmultiEntities db = new VastwebmultiEntities();
        /// <summary>
        /// Default constructor - controller initialize karta hai
        /// </summary>
        public ECommerceController()
        {

        }
        /// <summary>
        /// UserManager aur SignInManager ke saath controller initialize karta hai
        /// </summary>
        public ECommerceController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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
        // GET: RETAILER/ECommerce
        //[MenuAccessFilter]
        /// <summary>
        /// ECommerce main page dikhata hai, KYC aur paid service status check karta hai
        /// </summary>
        public ActionResult Index(string txtSearch, string SortBy)
        {

            var userid = User.Identity.GetUserId();
            string check = "OK";

            var ChkKYC = db.Retailer_Details.Where(aa => aa.RetailerId == userid).SingleOrDefault();
            if (ChkKYC.PSAStatus == "Y" && ChkKYC.AadhaarStatus == "Y" && ChkKYC.ShopwithSalfieStatus == "Y")
            {
                var freeservice = db.PaidServicesChargeLists.Where(aa => aa.ServiceName == "ECOMM").SingleOrDefault();
                var ALLfreeservice = db.PaidServicesChargeLists.Where(aa => aa.ServiceName == "ALL").SingleOrDefault();
                if (freeservice.IsFree == true && ALLfreeservice.IsFree == true)
                {

                    check = "OK";
                }
                var checkfreeservice = db.PaidServicesChargeLists.Where(aa => aa.ServiceName == "ECOMM").SingleOrDefault();
                var checkALLfreeservice = db.PaidServicesChargeLists.Where(aa => aa.ServiceName == "ALL").SingleOrDefault();
                if (checkfreeservice.IsFree == true)
                {
                    if (checkALLfreeservice.IsFree == false)
                    {

                        var retailerautorenseting = db.autopaidserviceRenewalsettings.Where(x => x.retailerid == userid).SingleOrDefault().auto_set;
                        if (retailerautorenseting == "ALL" || retailerautorenseting == "PER")
                        {
                            var chkklatestdate = db.PaidServicesPaymentHistories.Where(aa => aa.UserId == userid && aa.ServiceName == "ECOMM").OrderByDescending(aa => aa.PurchaseDate).FirstOrDefault();
                            if (chkklatestdate != null)
                            {
                                var expiredate = chkklatestdate.ExpiryDate.Date.AddDays(-1);
                                if (expiredate == DateTime.Now.Date)
                                {
                                    var chkadminperservice = db.PaidServicesChargeLists.Where(aa => aa.ServiceName == "ECOMM" && aa.IsFree == false).SingleOrDefault();
                                    var chkadminallservice = db.PaidServicesChargeLists.Where(aa => aa.ServiceName == "ALL" && aa.IsFree == false).SingleOrDefault();
                                    System.Data.Entity.Core.Objects.ObjectParameter Status = new System.Data.Entity.Core.Objects.ObjectParameter("Status", typeof(string));
                                    System.Data.Entity.Core.Objects.ObjectParameter Message = new System.Data.Entity.Core.Objects.ObjectParameter("Message", typeof(string));
                                    if (chkadminallservice != null && retailerautorenseting == "ALL")
                                    {
                                        var msg = db.proc_PurchasePaidServices(userid, chkadminallservice.Idno, Status, Message).SingleOrDefault();
                                    }
                                    else if (chkadminperservice != null && retailerautorenseting == "PER")
                                    {
                                        var msg = db.proc_PurchasePaidServices(userid, chkadminperservice.Idno, Status, Message).SingleOrDefault();
                                    }
                                }

                            }



                        }
                        //    var chk = db.PaidService_auto.Where(aa => aa.Userid == userid && aa.ServiceName == "AEPS").SingleOrDefault().AutoSts;
                        //if (chk == "Y")
                        //{
                        //    System.Data.Entity.Core.Objects.ObjectParameter Status = new System.Data.Entity.Core.Objects.ObjectParameter("Status", typeof(string));
                        //    System.Data.Entity.Core.Objects.ObjectParameter Message = new System.Data.Entity.Core.Objects.ObjectParameter("Message", typeof(string));
                        //    int serviceid = db.PaidServicesChargeLists.Where(aa => aa.ServiceName == "AEPS").SingleOrDefault().Idno;
                        //    var msg = db.proc_PurchasePaidServices(userid, serviceid, Status, Message).SingleOrDefault();
                        //}
                        var servicecheck = db.PaidServicesPaymentHistories.Where(aa => aa.UserId == userid && aa.ServiceName == "ECOMM").OrderByDescending(aa => aa.PurchaseDate).FirstOrDefault();
                        if (servicecheck != null)
                        {

                            var expdate = servicecheck.ExpiryDate;
                            var currentdate = DateTime.Now;
                            if (expdate <= currentdate)
                            {
                                //  status = "ALLNOTDONE";
                                ViewBag.servicefressornot = "ALLNOTDONE";
                                check = "ALLNOTDONE";

                                //  message = "Pan Card Service is Expired.";

                                //  var results = "{'status':'ALLNOTDONE','msg':'MICROATM Service is Expired.'}";
                                //  var json1 = JsonConvert.DeserializeObject(results);
                                //  var json = JsonConvert.SerializeObject(json1);
                                //  return Json(json, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            // status = "ALLNOTDONE";
                            ViewBag.servicefressornot = "ALLNOTDONE";
                            check = "ALLNOTDONE";

                            // message = "Firstlly Purchase this Service.";
                            //  var results = "{'status':'ALLNOTDONE','msg':'Firstlly Purchase this Service.'}";
                            //  var json1 = JsonConvert.DeserializeObject(results);
                            //  var json = JsonConvert.SerializeObject(json1);
                            // return Json(json, JsonRequestBehavior.AllowGet);
                        }
                    }
                }

                if (checkfreeservice.IsFree == false)
                {
                    if (checkALLfreeservice.IsFree == false)
                    {
                        //var chk = db.PaidService_auto.Where(aa => aa.Userid == userid && aa.ServiceName == "PANCARD").SingleOrDefault().AutoSts;


                        var retailerautorenseting = db.autopaidserviceRenewalsettings.Where(x => x.retailerid == userid).SingleOrDefault().auto_set;
                        if (retailerautorenseting == "ALL" || retailerautorenseting == "PER")
                        {
                            var chkklatestdate = db.PaidServicesPaymentHistories.Where(aa => aa.UserId == userid && aa.ServiceName == "ECOMM").OrderByDescending(aa => aa.PurchaseDate).FirstOrDefault();
                            if (chkklatestdate != null)
                            {
                                var expiredate = chkklatestdate.ExpiryDate.Date.AddDays(-1);
                                if (expiredate == DateTime.Now.Date)
                                {
                                    var chkadminperservice = db.PaidServicesChargeLists.Where(aa => aa.ServiceName == "ECOMM" && aa.IsFree == false).SingleOrDefault();
                                    var chkadminallservice = db.PaidServicesChargeLists.Where(aa => aa.ServiceName == "ALL" && aa.IsFree == false).SingleOrDefault();
                                    System.Data.Entity.Core.Objects.ObjectParameter Status = new System.Data.Entity.Core.Objects.ObjectParameter("Status", typeof(string));
                                    System.Data.Entity.Core.Objects.ObjectParameter Message = new System.Data.Entity.Core.Objects.ObjectParameter("Message", typeof(string));
                                    if (chkadminallservice != null && retailerautorenseting == "ALL")
                                    {
                                        var msg = db.proc_PurchasePaidServices(userid, chkadminallservice.Idno, Status, Message).SingleOrDefault();
                                    }
                                    else if (chkadminperservice != null && retailerautorenseting == "PER")
                                    {
                                        var msg = db.proc_PurchasePaidServices(userid, chkadminperservice.Idno, Status, Message).SingleOrDefault();
                                    }

                                }

                            }



                        }
                        //if (chk == "Y")
                        //{
                        //    System.Data.Entity.Core.Objects.ObjectParameter Status = new System.Data.Entity.Core.Objects.ObjectParameter("Status", typeof(string));
                        //    System.Data.Entity.Core.Objects.ObjectParameter Message = new System.Data.Entity.Core.Objects.ObjectParameter("Message", typeof(string));
                        //    int serviceid = db.PaidServicesChargeLists.Where(aa => aa.ServiceName == "AEPS").SingleOrDefault().Idno;
                        //    var msg = db.proc_PurchasePaidServices(userid, serviceid, Status, Message).SingleOrDefault();
                        //}

                        var servicecheck = db.PaidServicesPaymentHistories.Where(aa => aa.UserId == userid && aa.ServiceName == "ECOMM").OrderByDescending(aa => aa.PurchaseDate).FirstOrDefault();
                        if (servicecheck != null)
                        {

                            var expdate = servicecheck.ExpiryDate;
                            var currentdate = DateTime.Now;
                            if (expdate <= currentdate)
                            {
                                ViewBag.servicefressornot = "BOTHNOTDONE";
                                check = "BOTHNOTDONE";

                                //   var results = "{'status':'BOTHNOTDONE','msg':'MICROATM Service is Expired'}";
                                //   var json1 = JsonConvert.DeserializeObject(results);
                                //   var json = JsonConvert.SerializeObject(json1);
                                //   return Json(json, JsonRequestBehavior.AllowGet);
                                //  status = "BOTHNOTDONE";

                                //  message = "Pan Card Service is Expired.";
                            }
                        }
                        else
                        {
                            ViewBag.servicefressornot = "BOTHNOTDONE";
                            check = "BOTHNOTDONE";

                            //   var results = "{'status':'BOTHNOTDONE','msg':'Firstlly Purchase this Service.'}";
                            //   var json1 = JsonConvert.DeserializeObject(results);
                            //   var json = JsonConvert.SerializeObject(json1);
                            //   return Json(json, JsonRequestBehavior.AllowGet);
                            //  status = "BOTHNOTDONE";

                            // message = "Firstlly Purchase this Service.";
                        }
                    }
                }

                if (checkfreeservice.IsFree == false)
                {
                    if (checkALLfreeservice.IsFree == true)
                    {

                        var retailerautorenseting = db.autopaidserviceRenewalsettings.Where(x => x.retailerid == userid).SingleOrDefault().auto_set;
                        if (retailerautorenseting == "ALL" || retailerautorenseting == "PER")
                        {
                            var chkklatestdate = db.PaidServicesPaymentHistories.Where(aa => aa.UserId == userid && aa.ServiceName == "ECOMM").OrderByDescending(aa => aa.PurchaseDate).FirstOrDefault();
                            if (chkklatestdate != null)
                            {
                                var expiredate = chkklatestdate.ExpiryDate.Date.AddDays(-1);
                                if (expiredate == DateTime.Now.Date)
                                {
                                    var chkadminperservice = db.PaidServicesChargeLists.Where(aa => aa.ServiceName == "ECOMM" && aa.IsFree == false).SingleOrDefault();
                                    var chkadminallservice = db.PaidServicesChargeLists.Where(aa => aa.ServiceName == "ALL" && aa.IsFree == false).SingleOrDefault();
                                    System.Data.Entity.Core.Objects.ObjectParameter Status = new System.Data.Entity.Core.Objects.ObjectParameter("Status", typeof(string));
                                    System.Data.Entity.Core.Objects.ObjectParameter Message = new System.Data.Entity.Core.Objects.ObjectParameter("Message", typeof(string));

                                    if (chkadminallservice != null && retailerautorenseting == "ALL")
                                    {
                                        var msg = db.proc_PurchasePaidServices(userid, chkadminallservice.Idno, Status, Message).SingleOrDefault();
                                    }
                                    else if (chkadminperservice != null && retailerautorenseting == "PER")
                                    {
                                        var msg = db.proc_PurchasePaidServices(userid, chkadminperservice.Idno, Status, Message).SingleOrDefault();
                                    }

                                }


                            }
                            else
                            {
                                //  status = "NOTOK";
                                ViewBag.servicefressornot = "NOTOK";
                                check = "NOTOK";

                                // message = "Firstlly Purchase this Service.";

                                //  var results = "{'status':'NOTOK','msg':'Firstlly Purchase this Service.'}";
                                //  var json1 = JsonConvert.DeserializeObject(results);
                                //  var json = JsonConvert.SerializeObject(json1);
                                //  return Json(json, JsonRequestBehavior.AllowGet);
                            }


                        }


                        //var chk = db.PaidService_auto.Where(aa => aa.Userid == userid && aa.ServiceName == "AEPS").SingleOrDefault().AutoSts;
                        //if (chk == "Y")
                        //{
                        //    System.Data.Entity.Core.Objects.ObjectParameter Status = new System.Data.Entity.Core.Objects.ObjectParameter("Status", typeof(string));
                        //    System.Data.Entity.Core.Objects.ObjectParameter Message = new System.Data.Entity.Core.Objects.ObjectParameter("Message", typeof(string));
                        //    int serviceid = db.PaidServicesChargeLists.Where(aa => aa.ServiceName == "AEPS").SingleOrDefault().Idno;
                        //    var msg = db.proc_PurchasePaidServices(userid, serviceid, Status, Message).SingleOrDefault();
                        //}
                        var servicecheck = db.PaidServicesPaymentHistories.Where(aa => aa.UserId == userid && aa.ServiceName == "ECOMM").OrderByDescending(aa => aa.PurchaseDate).FirstOrDefault();
                        if (servicecheck != null)
                        {

                            var expdate = servicecheck.ExpiryDate;
                            var currentdate = DateTime.Now;
                            if (expdate <= currentdate)
                            {
                                // status = "NOTOK";
                                ViewBag.servicefressornot = "NOTOK";
                                check = "NOTOK";

                                //  message = "Pan Card Service is Expired.";
                                //   var results = "{'status':'NOTOK','msg':'MICROATM Service is Expired.'}";
                                //   var json1 = JsonConvert.DeserializeObject(results);
                                //   var json = JsonConvert.SerializeObject(json1);
                                //   return Json(json, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            //status = "NOTOK";
                            ViewBag.servicefressornot = "NOTOK";
                            check = "NOTOK";

                            //message = "Firstlly Purchase this Service.";
                            //  var results = "{'status':'NOTOK','msg':'Firstlly Purchase this Service.'}";
                            //  var json1 = JsonConvert.DeserializeObject(results);
                            //  var json = JsonConvert.SerializeObject(json1);
                            //  return Json(json, JsonRequestBehavior.AllowGet);
                        }
                    }
                }

                if (check == "OK")
                {
                    ViewBag.servicefressornot = "OK";
                    ProductVM model = new ProductVM();
                    model.ProductName = txtSearch;
                    model.SortBy = SortBy;
                    model.itemsInCart = db.Carts.Count(a => a.BuyerId == userid);
                    return View(model);
                }
            }
            else
            {
                return RedirectToAction("Profile");
            }
            ProductVM model1 = new ProductVM();
            return View(model1);
        }
        /// <summary>
        /// Product categories aur sub-categories ka menu partial view return karta hai
        /// </summary>
        public PartialViewResult _MenuList()
        {
            MenuVM model = new MenuVM();
            model.lstCatagory = db.Catagories.Select(a => new CatagoryModel { CatID = a.CatID, CatName = a.CatName }).ToList();
            model.subCata = db.SubCatagories.Select(r => new SubCatagoryMoidel
            {
                CatID = r.CatID,
                SubCatagoryName = r.SubCatName,
                SubcatID = r.SubCatID
            }).ToList();
            return PartialView(model);
        }



        /// <summary>
        /// Filter ke hisaab se products ki list partial view mein dikhata hai
        /// </summary>
        public PartialViewResult _productlist(string ProductName, int? SubCatId, decimal? Price, string SortBy)
        {
            var RetailerID = User.Identity.GetUserId();
            ProductDetails model = new ProductDetails();
            model.itemsInCart = db.Carts.Count(a => a.BuyerId == RetailerID);

            if (SubCatId != null && SubCatId > 0)
            {
                model.CataTitle = db.SubCatagories.SingleOrDefault(a => a.SubCatID == SubCatId)?.SubCatName;
            }
            model.lstProducts = db.proc_ProductList(0, "", SubCatId ?? null, null, null, ProductName ?? null).Where(a => a.IsApproved == 1 && a.ShowOnHome == true).ToList();
            if (SortBy != null && !string.IsNullOrWhiteSpace(SortBy) && model.lstProducts.Count > 0)
            {
                if (SortBy.Contains("Date"))
                    model.lstProducts = model.lstProducts.OrderByDescending(a => a.CreatedOn).ToList();
                else if (SortBy.Contains("PriceLow"))
                    model.lstProducts = model.lstProducts.OrderBy(a => a.StandardPrice).ToList();
                else if (SortBy.Contains("PriceHigh"))
                    model.lstProducts = model.lstProducts.OrderByDescending(a => a.StandardPrice).ToList();
            }
            return PartialView(model);
        }
        [HttpGet]
        /// <summary>
        /// Product name se search karke matching products ki list JSON mein return karta hai
        /// </summary>
        public JsonResult FindProductByName(string term)
        {
            //Searching records from list using LINQ query
            List<string> lstProducts = db.Products.Where(a => a.Name.Contains(term)).Select(r => r.Name).ToList<string>();

            return Json(lstProducts, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Product ka detail page dikhata hai, cart mein add bhi kar sakta hai
        /// </summary>
        public ActionResult ProductView(int id, string type)
        {
            try
            {
                if (type == "Addcartitem")
                {
                    var userid = User.Identity.GetUserId();
                    var model = db.proc_ProductList(id, null, null, null, null, null).SingleOrDefault();
                    var xx = db.ProductVendors.SingleOrDefault(a => a.ProductID == model.ProductID).VenderID;
                    ViewBag.Vendor = db.Vendor_details.SingleOrDefault(a => a.userid == xx);
                    //Add/Update product to cart
                    var cartEntry = db.Carts.SingleOrDefault(a => a.BuyerId.ToLower() == userid.ToLower() && a.ProductId == id);
                    if (cartEntry == null)
                    {
                        var cart = new Cart();
                        cart.BuyerId = userid;
                        cart.ListPrice = model.ListPrice;
                        cart.StandardPrice = model.StandardPrice;
                        cart.ProductId = id;
                        cart.Qty = 1;
                        cart.SellerId = xx;
                        cart.OrderStatus = 1;
                        db.Carts.Add(cart);
                        db.SaveChanges();
                    }
                    else
                    {
                        cartEntry.Qty = cartEntry.Qty + 1;
                        db.SaveChanges();
                    }
                    return View(model);
                }
                else
                {
                    var model = db.proc_ProductList(id, null, null, null, null, null).SingleOrDefault();
                    var xx = db.ProductVendors.SingleOrDefault(a => a.ProductID == model.ProductID).VenderID;
                    ViewBag.Vendor = db.Vendor_details.SingleOrDefault(a => a.userid == xx);

                    return View(model);
                }

            }
            catch
            {
                return View();
            }

        }
        /// <summary>
        /// Retailer ka shopping cart dikhata hai sabhi selected products ke saath
        /// </summary>
        public ActionResult viewCart()
        {
            var userid = User.Identity.GetUserId();
            var lstProductincart = db.proc_GetCartItems(userid).Select(a => new CartVM
            {
                imgUrl = a.URL,
                ListPrice = a.ListPrice,
                productId = a.ProductID,
                productName = a.Name,
                Qty = a.Qty,
                satandardPrice = a.StandardPrice,
                VenderID = a.VenderID,
                VenderFirmName = a.Firmname,
                MinOrderQty = a.MinOrderQty
            }).ToList();

            return View(lstProductincart);
        }

        /// <summary>
        /// Cart partial view return karta hai current cart items ke saath
        /// </summary>
        public PartialViewResult _viewCart()
        {
            try
            {
                var userid = User.Identity.GetUserId();
                var lstProductincart = db.proc_GetCartItems(userid).Select(a => new CartVM
                {
                    imgUrl = a.URL,
                    ListPrice = a.ListPrice,
                    productId = a.ProductID,
                    productName = a.Name,
                    Qty = a.Qty,
                    satandardPrice = a.StandardPrice,
                    VenderID = a.VenderID,
                    VenderFirmName = a.Firmname,
                    MinOrderQty = a.MinOrderQty
                }).ToList();
                return PartialView(lstProductincart);
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Cart mein product ki quantity update karta hai aur updated cart return karta hai
        /// </summary>
        public PartialViewResult UpdateQTY(int productId, int QTY)
        {
            try
            {
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    var userid = User.Identity.GetUserId();
                    var cartItem = db.Carts.SingleOrDefault(a => a.ProductId == productId && a.BuyerId == userid);
                    cartItem.Qty = QTY < 1 ? cartItem.Qty : QTY;
                    db.SaveChanges();
                    var lstProductincart = db.proc_GetCartItems(userid).Select(a => new CartVM
                    {
                        imgUrl = a.URL,
                        ListPrice = a.ListPrice,
                        productId = a.ProductID,
                        productName = a.Name,
                        Qty = a.Qty,
                        satandardPrice = a.StandardPrice,
                        VenderID = a.VenderID,
                        VenderFirmName = a.Firmname,
                        MinOrderQty = a.MinOrderQty
                    }).ToList();
                    return PartialView("_viewCart", lstProductincart);
                }
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Cart se ek product remove karta hai aur updated cart partial view return karta hai
        /// </summary>
        public PartialViewResult RemoveCartItem(int productId)
        {
            try
            {
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    var userid = User.Identity.GetUserId();
                    var cartItem = db.Carts.SingleOrDefault(a => a.ProductId == productId && a.BuyerId == userid);
                    db.Carts.Remove(cartItem);
                    db.SaveChanges();
                    var lstProductincart = db.proc_GetCartItems(userid).Select(a => new CartVM
                    {
                        imgUrl = a.URL,
                        ListPrice = a.ListPrice,
                        productId = a.ProductID,
                        productName = a.Name,
                        Qty = a.Qty,
                        satandardPrice = a.StandardPrice,
                        VenderID = a.VenderID,
                        VenderFirmName = a.Firmname,
                        MinOrderQty = a.MinOrderQty
                    }).ToList();
                    return PartialView("_viewCart", lstProductincart);
                }
            }
            catch
            {
                throw;
            }
        }
        [HttpPost]
        /// <summary>
        /// Cart mein rakhe product ka payment process karta hai aur order confirm karta hai
        /// </summary>
        public ActionResult doPayment(int productID)
        {
            try
            {
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    var userid = User.Identity.GetUserId();
                    //var user = await UserManager.FindAsync(model.Email, model.Password);
                    string rolename = UserManager.GetRoles(userid).FirstOrDefault();
                    System.Data.Entity.Core.Objects.ObjectParameter output = new
                        System.Data.Entity.Core.Objects.ObjectParameter("output", typeof(string));
                    var msg = db.proc_doCartPayment(productID, userid, rolename, output).SingleOrDefault().msg;
                    if (msg.Contains("Success"))
                    {
                        TempData["Status"] = "Success";
                        TempData["Message"] = "Your shoping order accepted successfully.";
                    }
                    else
                    {
                        TempData["Status"] = "Failed";
                        TempData["Message"] = msg;
                    }
                    return RedirectToAction("Index");
                }
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }
        [HttpGet]
        /// <summary>
        /// ECommerce transactions ki report aaj ki date ke liye dikhata hai
        /// </summary>
        public ActionResult Transaction()
        {
            try
            {
                string userid = User.Identity.GetUserId();
                var category = db.Catagories.Distinct().ToList();
                ViewBag.category = new SelectList(category, "CatID", "CatName");
                string txt_frm_date = DateTime.Now.ToString();
                string txt_to_date = DateTime.Now.ToString();
                string frm_date = Convert.ToDateTime(txt_frm_date).Date.ToString("yyyy-MM-dd");
                string to_date = Convert.ToDateTime(txt_to_date).AddDays(1).ToString("yyyy-MM-dd");
                var vv = db.show_ecomm_report(userid, "Retailer", "ALL", "", 50, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
                var totalsuccess = vv.Where(a => a.OrderStatus == 2).Sum(a => a.Amount);
                var totalpending = vv.Where(a => a.OrderStatus == 1).Sum(a => a.Amount);
                var totalreject = vv.Where(a => a.OrderStatus == 3).Sum(a => a.Amount);
                var rmincome = vv.Where(a => a.OrderStatus == 2).Sum(a => a.Remcomm);
                ViewData["Totals"] = totalsuccess;
                ViewData["Totalp"] = totalpending;
                ViewData["Totalf"] = totalreject;
                ViewData["Rincome"] = rmincome;

                return View(vv);

            }
            catch
            {
                return View();
            }
        }
        [HttpPost]
        /// <summary>
        /// Filter ke saath ECommerce transaction report dikhata hai - category, date aur status se
        /// </summary>
        public ActionResult Transaction(string category, int ddl_top, string txt_frm_date, string txt_to_date, string ddl_status)
        {
            try
            {
                ViewBag.chk = "post";
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
                var category1 = db.Catagories.Distinct().ToList();
                ViewBag.category = new SelectList(category1, "CatID", "CatName");
                var vv = db.show_ecomm_report(userid, "Retailer", ddl_status, category, ddl_top, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
                var totalsuccess = vv.Where(a => a.OrderStatus == 2).Sum(a => a.Amount);
                var totalpending = vv.Where(a => a.OrderStatus == 1).Sum(a => a.Amount);
                var totalreject = vv.Where(a => a.OrderStatus == 3).Sum(a => a.Amount);
                var rmincome = vv.Where(a => a.OrderStatus == 2).Sum(a => a.Remcomm);
                ViewData["Totals"] = totalsuccess;
                ViewData["Totalp"] = totalpending;
                ViewData["Totalf"] = totalreject;
                ViewData["Rincome"] = rmincome;
                return View(vv);

            }
            catch
            {
                return View();
            }
        }
        [HttpGet]
        /// <summary>
        /// Ecommerce order ka invoice PDF generate karke return karta hai
        /// </summary>
        public ActionResult EcommInvoicePdf(int OrderID)
        {

            var userid = User.Identity.GetUserId();
            var roleName = db.proc_getRoleByUserID(userid).SingleOrDefault().Name;
            var procResult = db.proc_getEcommTransactionHistory(null, OrderID, roleName, null, null, null).ToList();
            return new Rotativa.ViewAsPdf(procResult);


        }

    }
}