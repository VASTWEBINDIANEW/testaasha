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

    /// <summary>
    /// RETAILER Area - Handles retailer e-commerce shopping, cart management, payment processing and transaction reporting.
    /// </summary>
    [Authorize(Roles = "Retailer")]
    [Low_Bal_CustomFilter()]
    public class ECommerceController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        VastwebmultiEntities db = new VastwebmultiEntities();
        /// <summary>
        /// Initializes a new instance of <see cref="ECommerceController"/> using the default dependency resolution.
        /// </summary>
        public ECommerceController()
        {

        }
        /// <summary>
        /// Initializes a new instance of <see cref="ECommerceController"/> with explicit user and sign-in managers.
        /// </summary>
        /// <param name="userManager">The application user manager instance.</param>
        /// <param name="signInManager">The application sign-in manager instance.</param>
        public ECommerceController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        /// <summary>
        /// Gets the application sign-in manager, resolving it from the OWIN context if not explicitly set.
        /// </summary>
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

        /// <summary>
        /// Gets the application user manager, resolving it from the OWIN context if not explicitly set.
        /// </summary>
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
        /// GET Displays the e-commerce home page; validates KYC and paid service status before returning the product listing.
        /// </summary>
        /// <param name="txtSearch">Optional product name search filter.</param>
        /// <param name="SortBy">Optional sort order (Date, PriceLow, PriceHigh).</param>
        /// <returns>The e-commerce index view with the product view model, or a redirect to Profile if KYC is incomplete.</returns>
        public ActionResult Index(string txtSearch, string SortBy)
        {

            var userid = User.Identity.GetUserId();
            string check = "OK";

            var ChkKYC = db.Retailer_Details.Where(aa => aa.RetailerId == userid).SingleOrDefault();
            if (ChkKYC.PSAStatus == "Y" && ChkKYC.AadhaarStatus == "Y" && ChkKYC.ShopwithSalfieStatus == "Y")
            {
                // Load service config once - reused throughout this method
                var checkfreeservice = db.PaidServicesChargeLists.AsNoTracking().Where(aa => aa.ServiceName == "ECOMM").SingleOrDefault();
                var checkALLfreeservice = db.PaidServicesChargeLists.AsNoTracking().Where(aa => aa.ServiceName == "ALL").SingleOrDefault();
                if (checkfreeservice.IsFree == true && checkALLfreeservice.IsFree == true)
                {
                    check = "OK";
                }
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
        /// GET Renders the partial menu list of e-commerce categories and sub-categories.
        /// </summary>
        /// <returns>A partial view containing category and sub-category navigation data.</returns>
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
        /// GET Renders a filtered and sorted partial product list based on name, sub-category, price and sort order.
        /// </summary>
        /// <param name="ProductName">Optional product name filter.</param>
        /// <param name="SubCatId">Optional sub-category ID filter.</param>
        /// <param name="Price">Optional price filter (reserved for future use).</param>
        /// <param name="SortBy">Optional sort order (Date, PriceLow, PriceHigh).</param>
        /// <returns>A partial view containing the filtered product list and cart item count.</returns>
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
        /// <summary>
        /// GET Searches products by name and returns a JSON list of matching product names for autocomplete.
        /// </summary>
        /// <param name="term">The search term to match against product names.</param>
        /// <returns>A JSON array of matching product name strings.</returns>
        [HttpGet]
        public JsonResult FindProductByName(string term)
        {
            //Searching records from list using LINQ query
            List<string> lstProducts = db.Products.Where(a => a.Name.Contains(term)).Select(r => r.Name).ToList<string>();

            return Json(lstProducts, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// GET Displays the product detail page; adds the product to the retailer's cart when the type is "Addcartitem".
        /// </summary>
        /// <param name="id">The product ID to display.</param>
        /// <param name="type">Action type; pass "Addcartitem" to add the product to the cart.</param>
        /// <returns>The product detail view, or an empty view on error.</returns>
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
        /// GET Displays the retailer's shopping cart with all current cart items.
        /// </summary>
        /// <returns>The cart view populated with current cart items.</returns>
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
        /// GET Renders the partial cart view showing all items currently in the retailer's cart.
        /// </summary>
        /// <returns>A partial view with the list of cart items.</returns>
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
        /// Updates the quantity of a specific product in the retailer's cart and returns the refreshed cart partial view.
        /// </summary>
        /// <param name="productId">The ID of the product whose quantity should be updated.</param>
        /// <param name="QTY">The new quantity to set; values less than 1 are ignored.</param>
        /// <returns>The updated _viewCart partial view.</returns>
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
        /// Removes a specific product from the retailer's cart and returns the refreshed cart partial view.
        /// </summary>
        /// <param name="productId">The ID of the product to remove from the cart.</param>
        /// <returns>The updated _viewCart partial view after removal.</returns>
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
        /// <summary>
        /// POST Processes the cart payment for the specified product; redirects to Index with a success or failure message.
        /// </summary>
        /// <param name="productID">The ID of the product being purchased from the cart.</param>
        /// <returns>A redirect to the Index action with status in TempData.</returns>
        [HttpPost]
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
        /// <summary>
        /// GET Displays today's e-commerce transactions with totals for successful, pending and rejected orders.
        /// </summary>
        /// <returns>The transaction report view with order records and aggregated totals in ViewData.</returns>
        [HttpGet]
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
        /// <summary>
        /// POST Displays the e-commerce transaction report filtered by category, date range, record count and order status.
        /// </summary>
        /// <param name="category">Category ID filter.</param>
        /// <param name="ddl_top">Maximum number of records to return.</param>
        /// <param name="txt_frm_date">Start date of the report range.</param>
        /// <param name="txt_to_date">End date of the report range.</param>
        /// <param name="ddl_status">Order status filter (e.g. Success, Failed, ALL).</param>
        /// <returns>The transaction report view with filtered results and aggregated totals.</returns>
        [HttpPost]
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
        /// <summary>
        /// GET Generates and returns a PDF invoice for the specified e-commerce order.
        /// </summary>
        /// <param name="OrderID">The unique identifier of the e-commerce order to invoice.</param>
        /// <returns>A PDF representation of the order invoice.</returns>
        [HttpGet]
        public ActionResult EcommInvoicePdf(int OrderID)
        {

            var userid = User.Identity.GetUserId();
            var roleName = db.proc_getRoleByUserID(userid).SingleOrDefault().Name;
            var procResult = db.proc_getEcommTransactionHistory(null, OrderID, roleName, null, null, null).ToList();
            return new Rotativa.ViewAsPdf(procResult);


        }

    }
}