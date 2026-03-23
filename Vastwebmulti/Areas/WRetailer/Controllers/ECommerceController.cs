using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Vastwebmulti.Areas.WRetailer.Models;
using Vastwebmulti.Areas.WRetailer.ViewModels;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.WRetailer.Controllers
{
 
    [Authorize(Roles = "Whitelabelretailer")]
    /// <summary>
    /// Is class ka kaam ECommerceController area ke operations handle karna hai.
    /// </summary>
    public class ECommerceController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        VastwebmultiEntities db = new VastwebmultiEntities();
        public ECommerceController()
        {

        }
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
        /// <summary>
        /// Main dashboard ya home page dikhata hai.
        /// </summary>
        public ActionResult Index(string txtSearch, string SortBy)
        {
            ProductVM model = new ProductVM();
            model.ProductName = txtSearch;
            model.SortBy = SortBy;
            return View(model);
        }
        /// <summary>
        /// Partial view render karta hai.
        /// </summary>
        public PartialViewResult _MenuList()
        {
            MenuVM model = new MenuVM();
            model.lstCatagory = db.Catagories.Select(a => new CatagoryModel { CatID = a.CatID, CatName = a.CatName }).ToList();
            model.subCata = db.SubCatagories.Select(r=> new SubCatagoryMoidel
            {
                CatID = r.CatID,
                SubCatagoryName = r.SubCatName,
                SubcatID = r.SubCatID
            }).ToList();
            return PartialView(model);
        }
        /// <summary>
        /// Partial view render karta hai.
        /// </summary>
        public PartialViewResult _productlist(string ProductName,int? SubCatId,decimal? Price,string SortBy)
        {
            var RetailerID = User.Identity.GetUserId();
            ProductDetails model = new ProductDetails();
            model.itemsInCart = db.Carts.Count(a => a.BuyerId == RetailerID);
           
            if(SubCatId!= null && SubCatId > 0)
            {
                model.CataTitle = db.SubCatagories.SingleOrDefault(a => a.SubCatID == SubCatId)?.SubCatName;
            }
            model.lstProducts = db.proc_ProductList(0, "", SubCatId?? null, null, null, ProductName ?? null).Where(a => a.IsApproved == 1 && a.ShowOnHome == true).ToList();
            if(SortBy != null && !string.IsNullOrWhiteSpace(SortBy) && model.lstProducts.Count > 0)
            {
                if (SortBy.Contains("Date"))
                    model.lstProducts =  model.lstProducts.OrderByDescending(a => a.CreatedOn).ToList();
                else if (SortBy.Contains("PriceLow"))
                    model.lstProducts = model.lstProducts.OrderBy(a => a.StandardPrice).ToList();
                else if (SortBy.Contains("PriceHigh"))
                    model.lstProducts = model.lstProducts.OrderByDescending(a => a.StandardPrice).ToList();
            }
            return PartialView(model);
        }
        [HttpGet]
        /// <summary>
        /// Records ko search/filter karta hai.
        /// </summary>
        public JsonResult FindProductByName(string term)
        {
            //Searching records from list using LINQ query
            List<string> lstProducts = db.Products.Where(a => a.Name.Contains(term)).Select(r => r.Name).ToList<string>();
            
            return Json(lstProducts, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Is action ka kaam 'ProductView' se related operation handle karna hai.
        /// </summary>
        public ActionResult ProductView(int id)
        {
            try
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
            catch
            {
                return View();
            }
           
        }
        /// <summary>
        /// Data fetch karke view mein dikhata hai.
        /// </summary>
        public ActionResult viewCart()
        {
            var userid = User.Identity.GetUserId();
            var lstProductincart = db.proc_GetCartItems(userid).Select(a=>new CartVM
            {
                imgUrl = a.URL,
                ListPrice = a.ListPrice,
                productId = a.ProductID,
                productName = a.Name,
                Qty = a.Qty,
                satandardPrice = a.StandardPrice,
                VenderID = a.VenderID,
                VenderFirmName = a.Firmname
            }).ToList();
            
            return View(lstProductincart);
        }

        /// <summary>
        /// Partial view render karta hai.
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
                    VenderFirmName = a.Firmname
                }).ToList();
                return PartialView(lstProductincart);
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Existing record ko update/edit karta hai.
        /// </summary>
        public PartialViewResult UpdateQTY(int productId,int QTY)
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
                        VenderFirmName = a.Firmname
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
        /// Record ko delete karta hai.
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
                        VenderFirmName = a.Firmname
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
        /// Is action ka kaam 'doPayment' se related operation handle karna hai.
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
                    if(msg.Contains("Success"))
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
                throw;
            }
        }
        [HttpGet]
        /// <summary>
        /// Is action ka kaam 'Transaction' se related operation handle karna hai.
        /// </summary>
        public ActionResult Transaction()
        {
            try
            {
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    var lstTransaction = db.proc_getEcommTransactionHistory(null, null, null, null, null,null).ToList();
                    return View(lstTransaction);
                }
            }
            catch
            {
                throw;
            }
        }
        [HttpPost]
        /// <summary>
        /// Is action ka kaam 'Transaction' se related operation handle karna hai.
        /// </summary>
        public ActionResult Transaction(int? TransactionId,string BuyerRole,string BuyerId,DateTime fromdate,DateTime todate)
        {
            try
            {
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    var lstTransaction = db.proc_getEcommTransactionHistory(null,null, null, null, null, null).ToList();
                    return View(lstTransaction);
                }
            }
            catch 
            {
                throw;
            }
        }
        [HttpGet]
        /// <summary>
        /// File download ya export/PDF generate karta hai.
        /// </summary>
        public ActionResult EcommInvoicePdf(int OrderID)
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                var userid = User.Identity.GetUserId();
                var roleName = db.proc_getRoleByUserID(userid).SingleOrDefault().Name;
                var procResult = db.proc_getEcommTransactionHistory(null, OrderID, roleName,null, null, null).ToList();
                return new Rotativa.ViewAsPdf(procResult);
            }
               
        }

    }
}