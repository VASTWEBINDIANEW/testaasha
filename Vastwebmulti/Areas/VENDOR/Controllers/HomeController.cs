using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using Rotativa;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;
using Vastwebmulti.Areas.VENDOR.Models;
using Vastwebmulti.Areas.VENDOR.ViewModel;
using Vastwebmulti.Models;


namespace Vastwebmulti.Areas.VENDOR.Controllers
{
 
    /// <summary>
    /// MVC controller for the VENDOR area. Manages vendor dashboard operations including
    /// product and attribute management, order tracking, e-commerce reporting, profile editing,
    /// and wallet-to-bank transfer requests.
    /// </summary>
    [Authorize(Roles = "Vendor")]
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
        ApplicationDbContext context = new ApplicationDbContext();
        VastwebmultiEntities db = new VastwebmultiEntities();
        VastBazaartoken Responsetoken = new VastBazaartoken();
        // GET: VENDOR/Home
        /// <summary>
        /// Displays the main vendor dashboard home page. Loads the vendor's email and profile photo into ViewBag.
        /// </summary>
        /// <returns>The Index view for the vendor dashboard.</returns>
        public ActionResult Index()
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                var userid = User.Identity.GetUserId();
                //Vonder Profile Details
                var vv = db.Vendor_details.AsNoTracking().FirstOrDefault(a => a.userid == userid);
                ViewBag.email = vv.emailid;
                ViewBag.image = vv.Photo;
                return View();
            }
        }

        /// <summary>
        /// Displays the paginated list of all products belonging to the currently authenticated vendor.
        /// Passes success and existence feedback messages from TempData to the view.
        /// </summary>
        /// <returns>The ProductList view.</returns>
        public ActionResult ProductList()
        {
            try
            {
                ViewData["success"] = TempData["success"];
                ViewData["exists"] = TempData["exists"];
                TempData.Remove("success");
                TempData.Remove("exists");
                var vendorId = User.Identity.GetUserId();
                return View();
            }
            catch 
            {
                throw;
            }
        }
        /// <summary>
        /// Renders the product list as a partial view for the currently authenticated vendor.
        /// Retrieves all products via the stored procedure <c>proc_ProductList</c>.
        /// </summary>
        /// <returns>A <see cref="PartialViewResult"/> containing the vendor's product list.</returns>
        public PartialViewResult _productListPartial()
        {
            try
            {
                var vendorId = User.Identity.GetUserId();
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    var entries = db.proc_ProductList(0, vendorId, null, null, null, null).ToList();
                    return PartialView(entries);
                }
            }
            catch 
            {
                throw;
            }
        }
        /// <summary>
        /// Displays the form for adding a new product. Populates the category dropdown from the database.
        /// </summary>
        /// <returns>The AddProduct view pre-populated with category data.</returns>
        public ActionResult AddProduct()
        {
            try
            {
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    ViewBag.category = new SelectList(db.Catagories.AsNoTracking().Where(a=>a.IsDeleted==false).ToList(), "CatID", "CatName").ToList();
                    //ViewBag.Catagories = new SelectList(db.SubCatagories.ToList(), "SubCatID", "SubCatName").ToList();
                    return View();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// Processes the form submission to add a new product. Validates that the standard price
        /// does not exceed the list price, then calls the <c>proc_InsertProduct</c> stored procedure.
        /// </summary>
        /// <param name="model">The product details submitted from the add-product form.</param>
        /// <returns>Redirects to <see cref="ProductList"/> after processing.</returns>
        [HttpPost]
        public ActionResult AddProduct(ProductModel model)
        {
            try
            {
                if (model.StandardPrice > model.ListPrice)
                {
                    return RedirectToAction("ProductList");
                }
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    var vendorId = User.Identity.GetUserId();
                    ViewBag.category = new SelectList(db.Catagories.Where(a => a.IsDeleted == false).ToList(), "CatID", "CatName").ToList();
                    //ViewBag.Catagories = new SelectList(db.SubCatagories.ToList(), "SubCatID", "SubCatName").ToList();
                    System.Data.Entity.Core.Objects.ObjectParameter output = new
                        System.Data.Entity.Core.Objects.ObjectParameter("Output", typeof(string));
                    var respo = db.proc_InsertProduct(vendorId, model.SubCatID, model.Name, model.ShortDescription,
                        model.FullDescription, model.Published, model.ShowOnHome, model.HSN, model.GST, model.IsActive,
                        DateTime.Now, DateTime.Now,
                        model.stock, model.MinQtyFlag, model.StandardPrice, model.ListPrice, model.AdminComm, model.MinOrderQty,
                        model.MaxOrderQty, output).SingleOrDefault()
                        ?.msg;
                    if (respo.Contains("SUCCESS"))
                    {
                        TempData["success"] = "Product Add Successfully.";
                    }
                    else
                    {
                        TempData["exists"] = "This Product Allready Exists";
                    }
                }
            }
            catch 
            {
                throw;
            }
            return RedirectToAction("ProductList");
        }

        /// <summary>
        /// Displays the form for editing an existing product. Populates subcategory dropdown
        /// and loads the current product data by the given product ID.
        /// </summary>
        /// <param name="id">The unique identifier of the product to edit.</param>
        /// <returns>The EditProduct view pre-populated with the product's current data.</returns>
        public ActionResult EditProduct(int id)
        {
            try
            {
                var vendorid = User.Identity.GetUserId();
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    //ViewBag.category = new SelectList(db.Catagories.Where(a => a.IsDeleted == false).ToList(), "CatID", "CatName").ToList();
                    ViewBag.Catagories = new SelectList(db.SubCatagories.Where(a => a.IsDeleted == false).ToList(), "SubCatID", "SubCatName").ToList();
                    var entry = db.proc_ProductList(id, vendorid, null, null, null, null).Select(a => new ProductModel()
                    {
                        Published = a.Published,
                        IsActive = a.IsActive,
                        SubCatID = a.SubCatID,
                        CatID = a.CatID,
                        CatName = a.CatName,
                        IsDeleted = a.IsDeleted,
                        FullDescription = a.FullDescription,
                        GST = a.GST,
                        HSN = a.HSN,
                        ListPrice = a.ListPrice,
                        MaxOrderQty = a.MaxOrderQty,
                        MinOrderQty = a.MinOrderQty,
                        MinQtyFlag = a.MinQtyFlag,
                        Name = a.Name,
                        OnOrderQty = a.OnOrderQty,
                        Photo = a.Photo,
                        ProductID = a.ProductID,
                        ShortDescription = a.ShortDescription,
                        ShowOnHome = a.ShowOnHome,
                        StandardPrice = a.StandardPrice,
                        SubCatName = a.SubCatName,
                        stock = a.stock,
                        AdminComm = a.AdminComm,
                    }).SingleOrDefault();
                    return View(entry);
                }
            }
            catch
            {
                throw;
            }

        }
        /// <summary>
        /// Processes the form submission to update an existing product's details.
        /// Calls the <c>proc_EditProduct</c> stored procedure with the updated model data.
        /// </summary>
        /// <param name="model">The updated product details submitted from the edit-product form.</param>
        /// <returns>Redirects to <see cref="ProductList"/> after processing.</returns>
        [HttpPost]
        public ActionResult EditProduct(ProductModel model)
        {
            try
            {
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    var vendorId = User.Identity.GetUserId();
                    System.Data.Entity.Core.Objects.ObjectParameter output = new
                        System.Data.Entity.Core.Objects.ObjectParameter("Output", typeof(string));
                    var respo = db.proc_EditProduct(model.ProductID, model.SubCatID, model.Name, model.ShortDescription,
                            model.FullDescription, model.Published, model.ShowOnHome, model.HSN, model.GST, model.IsActive,
                            DateTime.Now, model.stock, model.MinQtyFlag, model.StandardPrice, model.ListPrice, model.AdminComm, model.MinOrderQty,
                            model.MaxOrderQty, output).SingleOrDefault()
                        ?.msg;
                    if (respo == "SUCCESS")
                    {
                        TempData["success"] = "Product Update Successfully.";
                    }
                    else
                    {
                        TempData["exists"] = respo;
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["exists"] = ex;
                return RedirectToAction("ProductList");
            }
            return RedirectToAction("ProductList");
        }

        //fill Sub category
        /// <summary>
        /// Returns a JSON list of subcategories that belong to the specified parent category.
        /// Used to dynamically populate the subcategory dropdown on the product form.
        /// </summary>
        /// <param name="Id">The parent category ID used to filter subcategories.</param>
        /// <returns>A JSON array of subcategory name/value pairs for use in a dropdown.</returns>
        public JsonResult SubcategoryList(int Id)
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                var district = from s in db.SubCatagories
                               where s.CatID == Id && s.IsDeleted ==false
                               select s;
                //ViewBag.district = new SelectList(district, "Dist_id", "Dist_Desc").ToList();
                return Json(new SelectList(district.ToArray(), "SubCatID", "SubCatName"), JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Handles the upload of a product image. Saves the uploaded image to the
        /// <c>ProductImages</c> directory and records the file path in the database.
        /// </summary>
        /// <param name="ProductID">The ID of the product to associate the uploaded image with.</param>
        /// <returns>Redirects to <see cref="ProductList"/> after processing.</returns>
        [HttpPost]
        public ActionResult ImgUpload(int ProductID)
        {
            if (ProductID > 0)
            {
                WebImage photo = null;
                var newFileName = "";
                var imagePath = "";
                photo = WebImage.GetImageFromRequest();
                if (photo != null)
                {
                    newFileName = Guid.NewGuid().ToString() + "_" +
                                  Path.GetFileName(photo.FileName);
                    imagePath = @"ProductImages\" + newFileName;

                    photo.Save(@"~\" + imagePath);
                    //save in db
                    using (VastwebmultiEntities db = new VastwebmultiEntities())
                    {
                        var entry = new ProductPhoto();
                        entry.ProductID = ProductID;
                        entry.URL = @"/ProductImages/" + newFileName;
                        entry.CreatedOn = DateTime.Now;
                        entry.UpdatedOn = DateTime.Now;
                        db.ProductPhotoes.Add(entry);
                        db.SaveChanges();
                    }
                }
            }

            return RedirectToAction("ProductList");
        }

        /// <summary>
        /// Displays the list of products that have been soft-deleted (marked for deletion)
        /// by the currently authenticated vendor.
        /// </summary>
        /// <returns>The DeleteproductList view with soft-deleted products, or redirects to Index on error.</returns>
        [HttpGet]
        public ActionResult DeleteproductList()
        {
            try
            {
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    var userid = User.Identity.GetUserId();
                    var show = db.deleteproductlist(userid).ToList();
                    return View(show);
                }
            }
            catch 
            {
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// Restores a previously soft-deleted product by setting its <c>IsDeleted</c> flag to <c>false</c>.
        /// </summary>
        /// <param name="idno">The unique identifier of the product to restore.</param>
        /// <returns>Redirects to <see cref="ProductList"/> after updating the status.</returns>
        [HttpPost]
        public ActionResult UpdateDeleteStatus(int idno)
        {
            try
            {
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    var entry = db.Products.SingleOrDefault(a => a.ProductID == idno);
                    if (entry != null)
                    {
                        entry.IsDeleted = false;
                        db.SaveChanges();
                    }
                }
            }
            catch 
            {
                throw;
            }
            return RedirectToAction("ProductList");
        }
        /// <summary>
        /// Soft-deletes a product by setting its <c>IsDeleted</c> flag to <c>true</c>.
        /// The product record is retained in the database but treated as removed.
        /// </summary>
        /// <param name="id">The unique identifier of the product to remove.</param>
        /// <returns>Redirects to <see cref="ProductList"/> after marking the product as deleted.</returns>
        [HttpGet]
        public ActionResult RemoveProduct(int id)
        {
            try
            {
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    var entry = db.Products.SingleOrDefault(a => a.ProductID == id);
                    if (entry != null)
                    {
                        entry.IsDeleted = true;
                        db.SaveChanges();
                    }
                }
            }
            catch 
            {
                throw;
            }
            return RedirectToAction("ProductList");
        }

        /// <summary>
        /// Creates a copy of an existing product for the currently authenticated vendor.
        /// The replicated product is named with a "copy of" prefix and is set as inactive by default.
        /// </summary>
        /// <param name="id">The unique identifier of the product to replicate.</param>
        /// <returns>A JSON result with "Success" or "Failed" status.</returns>
        [HttpPost]
        public ActionResult ReplicateProduct(int id)
        {
            try
            {
                var vendorId = User.Identity.GetUserId();
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {

                    var product = db.proc_ProductList(id, vendorId, null, null, null, null).SingleOrDefault();
                    var replicateCount = db.Products.Count(a => a.Name.Contains(product.Name) && a.ProductVendors.FirstOrDefault().VenderID.Contains(vendorId));
                    System.Data.Entity.Core.Objects.ObjectParameter output = new
                        System.Data.Entity.Core.Objects.ObjectParameter("Output", typeof(string));

                    var respo = db.proc_InsertProduct(vendorId, product.SubCatID, replicateCount.ToString() + " copy of " + product.Name, product.ShortDescription,
                        product.FullDescription, product.Published, product.ShowOnHome, replicateCount.ToString() + "copy of " + product.HSN, product.GST, false,
                        DateTime.Now, DateTime.Now,
                        product.stock, product.MinQtyFlag, product.StandardPrice, product.ListPrice, product.AdminComm, product.MinOrderQty,
                        product.MaxOrderQty, output).SingleOrDefault()
                        ?.msg;
                    if (respo.Contains("FAILED"))
                    {
                        return Json("Failed");
                    }
                }
                return Json("Success");
            }
            catch 
            {
                return Json("Failed");
            }
        }

        /// <summary>
        /// Displays the product attributes management page. Lists all attributes assigned to
        /// the vendor's products and populates the products dropdown.
        /// </summary>
        /// <returns>The Attributes view with the list of product-assigned attributes.</returns>
        public ActionResult Attributes()
        {
            try
            {
                ViewData["success"] = TempData["success"];
                ViewData["duplicate"] = TempData["duplicate"];
                ViewData["limitexceed"] = TempData["limitexceed"];
                TempData.Remove("success");
                TempData.Remove("duplicate");
                TempData.Remove("limitexceed");
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    var vendorid = User.Identity.GetUserId();
                    ViewBag.Products = db.proc_ProductList(0, vendorid, null, null, null, null).ToList();
                    var entries = db.proc_ProductAssignedAttributeList(vendorid, 0).ToList();
                    return View(entries);
                }
            }
            catch 
            {

                throw;
            }

        }

        /// <summary>
        /// Assigns a named attribute with a value and quantity to the specified product.
        /// Calls the <c>proc_AssigneProductAttribute</c> stored procedure and handles
        /// duplicate and limit-exceeded responses.
        /// </summary>
        /// <param name="ProductId">The ID of the product to assign the attribute to.</param>
        /// <param name="AttName">The name of the attribute (e.g., "Color", "Size").</param>
        /// <param name="AttValue">The value for the attribute (e.g., "Red", "XL").</param>
        /// <param name="QTY">The quantity available for this attribute variant.</param>
        /// <returns>Redirects to <see cref="Attributes"/> after processing.</returns>
        [HttpPost]
        public ActionResult AssignAttribute(int ProductId, string AttName, string AttValue, int QTY)
        {
            try
            {
                if (ProductId > 0 && !string.IsNullOrWhiteSpace(AttName) && !string.IsNullOrWhiteSpace(AttValue))
                {
                    using (VastwebmultiEntities db = new VastwebmultiEntities())
                    {
                        var vendorid = User.Identity.GetUserId();
                        System.Data.Entity.Core.Objects.ObjectParameter output = new
                            System.Data.Entity.Core.Objects.ObjectParameter("Output", typeof(string));
                        var respo = db.proc_AssigneProductAttribute(vendorid, ProductId, AttName, AttValue, QTY, output)
                            .SingleOrDefault().msg.ToString();
                        if (respo.Contains("SUCCESS"))
                        {
                            TempData["success"] = "Attributes Add Successfully.";
                        }
                        if (respo.Contains("DUPLICATE"))
                        {
                            TempData["duplicate"] = "This is Allready Exists";
                        }
                        if (respo.Contains("LIMIT EXCEED"))
                        {
                            TempData["limitexceed"] = "LIMIT EXCEED";
                        }
                        return RedirectToAction("Attributes");

                    }
                }
                else
                {
                    return RedirectToAction("Attributes");
                }
            }
            catch 
            {
                return RedirectToAction("Attributes");
            }

        }

        /// <summary>
        /// Permanently deletes a product attribute record from the database by its ID.
        /// </summary>
        /// <param name="Id">The unique identifier of the product attribute to remove.</param>
        /// <returns>A JSON result with "Success" if deleted, or "Failed" if the ID is invalid.</returns>
        public JsonResult RemoveAttribute(int Id)
        {
            try
            {
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    if (Id > 0 )
                    {
                        var entry = db.ProductAttributes.Single(a => a.AttID == Id);
                        db.ProductAttributes.Remove(entry);
                        db.SaveChanges();
                        return Json("Success", JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json("Failed", JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch 
            {
                throw;
            }

        }

        #region E-commerce Report
        /// <summary>
        /// Displays the e-commerce transaction report for today, showing success, pending,
        /// and rejected order totals for the currently authenticated vendor.
        /// </summary>
        /// <returns>The Ecommerce_Report view with today's transaction data.</returns>
        public ActionResult Ecommerce_Report()
        {

            VastwebmultiEntities db = new VastwebmultiEntities();
            string userid = User.Identity.GetUserId();
            var category = db.Catagories.Distinct().ToList();
            ViewBag.category = new SelectList(category, "CatID", "CatName");
            string txt_frm_date = DateTime.Now.ToString();
            string txt_to_date = DateTime.Now.ToString();
            string frm_date = Convert.ToDateTime(txt_frm_date).Date.ToString("yyyy-MM-dd");
            string to_date = Convert.ToDateTime(txt_to_date).AddDays(1).ToString("yyyy-MM-dd");
            var vv = db.show_ecomm_report(userid, "Vendor", "ALL", "", 50, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            var totalsuccess = vv.Where(a => a.OrderStatus == 2).Sum(a => a.Amount);
            var totalpending = vv.Where(a => a.OrderStatus == 1).Sum(a => a.Amount);
            var totalreject = vv.Where(a => a.OrderStatus == 3).Sum(a => a.Amount);
            var rmincome = vv.Where(a => a.OrderStatus == 2).Sum(a => a.Remcomm);
            ViewData["Totals"] = totalsuccess;
            ViewData["Totalp"] = totalpending;
            ViewData["Totalf"] = totalreject;
            return View(vv);

        }
        /// <summary>
        /// Filters and displays the e-commerce transaction report based on the supplied criteria.
        /// </summary>
        /// <param name="category">The category filter for the report.</param>
        /// <param name="ddl_top">The maximum number of records to return.</param>
        /// <param name="ddl_status">The order status filter (e.g., "ALL", "Pending", "Success").</param>
        /// <param name="txt_frm_date">The start date of the report date range.</param>
        /// <param name="txt_to_date">The end date of the report date range.</param>
        /// <returns>The Ecommerce_Report view filtered by the specified parameters.</returns>
        [HttpPost]
        public ActionResult Ecommerce_Report(string category, int ddl_top, string ddl_status, string txt_frm_date, string txt_to_date)
        {
            VastwebmultiEntities db = new VastwebmultiEntities();
            var userid = User.Identity.GetUserId();
            ViewBag.chk = "post";
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
            var category1 = db.SubCatagories.Distinct().ToList();
            ViewBag.category = new SelectList(category1, "SubCatID", "SubCatName");
                var vv = db.show_ecomm_report(userid, "Vendor", ddl_status, category, ddl_top, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            var totalsuccess = vv.Where(a => a.OrderStatus == 2).Sum(a => a.Amount);
            var totalpending = vv.Where(a => a.OrderStatus == 1).Sum(a => a.Amount);
            var totalreject = vv.Where(a => a.OrderStatus == 3).Sum(a => a.Amount);
            var rmincome = vv.Where(a => a.OrderStatus == 2).Sum(a => a.Remcomm);
            ViewData["Totals"] = totalsuccess;
            ViewData["Totalp"] = totalpending;
            ViewData["Totalf"] = totalreject;
            return View(vv);
        }

        /// <summary>
        /// Generates and returns a PDF bill invoice for the specified order.
        /// Calculates GST (IGST for inter-state or CGST/SGST for intra-state) and
        /// converts the total amount to words.
        /// </summary>
        /// <param name="id">The order ID for which to generate the invoice PDF.</param>
        /// <returns>A PDF view of the product bill invoice, or an empty view on error.</returns>
        public ActionResult ProductBillInvoice(int id)
               {
            try
            {
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    var entries = db.show_ecomm_report_pdf(id).FirstOrDefault();

                        ViewBag.VendorMobile = entries.VendorMobile;
                        ViewBag.VendorEmail = entries.VendorEmail;
                        ViewBag.Vendorfrmname = entries.Vendorfrmname;
                        ViewBag.VendorAddress = entries.VendorAddress;
                        ViewBag.VendorName= entries.VendorName;
                        ViewBag.VendorPincode = entries.VendorPincode;
                        ViewBag.VendorState = entries.VendorState;
                        ViewBag.OrderID = entries.ID;
                        ViewBag.OrderDate = entries.OrderDate;
                        ViewBag.RetailerName = entries.RetailerName;
                        ViewBag.RetailerEmail = entries.Email;
                        ViewBag.RetailerAddress = entries.RetailerAddress;
                        ViewBag.RetailerPincode = entries.RetailerPincode;
                        ViewBag.RetailerState = entries.RetailerState;
                        ViewBag.RetailerMobile = entries.RetailerMobile;
                        ViewBag.ProductName = entries.ProductName;
                        ViewBag.ProductTitle = entries.ShortDescription;
                        ViewBag.HSN = entries.HSN;
                        ViewBag.Qty = entries.QTY;
                       ViewBag.Amount = entries.Amount;
                      ViewBag.StandardPrice = entries.StandardPrice;
                   

                    if (entries.RetailerState != "Rajasthan")
                        {
                           
                            ViewBag.igsttax = entries.GST;
                            ViewBag.totaligst = Convert.ToDouble(entries.IGST);
                            ViewBag.Totalamount = entries.IGST + entries.Amount;
                             ViewBag.TotalProductGatval =  entries.IGST;

                    }
                        else
                        {
                            var totalgst = entries.GST;
                            ViewBag.cgst = totalgst / 2;
                            ViewBag.sgst = totalgst / 2;
                             ViewBag.TotalCgstamt = entries.CGST;
                             ViewBag.TotalSgstamt = entries.SGST ;
                            ViewBag.Totalamount = entries.CGST + entries.SGST + entries.Amount;
                             ViewBag.TotalProductGatval = entries.SGST + entries.CGST ;

                    }

      
                    double fin = Convert.ToDouble(ViewBag.Totalamount);
                    string s = words(fin, true).Substring(0, 3);
                    if (s == "and")
                    {
                        s = words(fin, true).Remove(0, 4);
                    }
                    else
                    {
                        s = words(fin, true);
                    }
                    ViewBag.finalword = s;

                    var number = ViewBag.Totalamount;
                    number = Convert.ToDouble(number).ToString();

                    return new ViewAsPdf("ProductBillInvoice", entries);

              
     
                }
            }
         catch
            {
                return View();
            }
           
        }
        /// <summary>
        /// Converts a numeric double value to its equivalent English words representation
        /// using the Indian numbering system (thousands, lakhs, crores).
        /// Optionally includes paisa (sub-rupee) conversion.
        /// </summary>
        /// <param name="numbers">The numeric amount to convert to words.</param>
        /// <param name="paisaconversion">
        /// When <c>true</c>, only converts the number without appending "rupees only".
        /// When <c>false</c>, appends "rupees only" or "rupees X paise only" to the result.
        /// </param>
        /// <returns>A string representing the amount in English words.</returns>
        public string words(double numbers, Boolean paisaconversion = false)
        {
            var pointindex = numbers.ToString().IndexOf(".");
            int number = Convert.ToInt32(Math.Floor(numbers));
            decimal paisaamt = 0;
            var jj = numbers.ToString().Split('.');
            if (pointindex > 0)
                paisaamt = Convert.ToDecimal(jj[1]);



            if (number == 0) return "Zero";
            if (number == -2147483648) return "Minus Two Hundred and Fourteen Crore Seventy Four Lakh Eighty Three Thousand Six Hundred and Forty Eight";
            int[] num = new int[4];
            int first = 0;
            int u, h, t;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            if (number < 0)
            {
                sb.Append("Minus ");
                number = -number;
            }
            string[] words0 = { "", "One ", "Two ", "Three ", "Four ", "Five ", "Six ", "Seven ", "Eight ", "Nine " };
            string[] words1 = { "Ten ", "Eleven ", "Twelve ", "Thirteen ", "Fourteen ", "Fifteen ", "Sixteen ", "Seventeen ", "Eighteen ", "Nineteen " };
            string[] words2 = { "Twenty ", "Thirty ", "Forty ", "Fifty ", "Sixty ", "Seventy ", "Eighty ", "Ninety " };
            string[] words3 = { "Thousand ", "Lakh ", "Crore " };
            num[0] = number % 1000; // units
            num[1] = number / 1000;
            num[2] = number / 100000;
            num[1] = num[1] - 100 * num[2]; // thousands
            num[3] = number / 10000000; // crores
            num[2] = num[2] - 100 * num[3]; // lakhs
            for (int i = 3; i > 0; i--)
            {
                if (num[i] != 0)
                {
                    first = i;
                    break;
                }
            }
            for (int i = first; i >= 0; i--)
            {
                if (num[i] == 0) continue;
                u = num[i] % 10; // ones
                t = num[i] / 10;
                h = num[i] / 100; // hundreds
                t = t - 10 * h; // tens
                if (h > 0) sb.Append(words0[h] + "Hundred ");
                if (u > 0 || t > 0)
                {
                    if (h > 0 || i == 0) sb.Append("and ");
                    if (t == 0)
                        sb.Append(words0[u]);
                    else if (t == 1)
                        sb.Append(words1[u]);
                    else
                        sb.Append(words2[t - 2] + words0[u]);
                }
                if (i != 0) sb.Append(words3[i - 1]);
            }

            if (paisaamt == 0 && paisaconversion == false)
            {
                sb.Append("ruppes only");
            }
            else if (paisaamt > 0)
            {
                var paisatext = words(Convert.ToDouble(paisaamt), true);
                sb.AppendFormat("rupees {0} paise only", paisatext);
            }
            return sb.ToString().TrimEnd();
        }
        #endregion

        /// <summary>
        /// Displays the product detail statistics report for the current vendor for today.
        /// Populates the category dropdown and loads today's product detail data.
        /// </summary>
        /// <returns>The Product_details view with today's product statistics.</returns>
        public ActionResult Product_details()
        {
            try
            {
                var vendorId = User.Identity.GetUserId();
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    string txt_frm_date = DateTime.Now.ToString();
                    string txt_to_date = DateTime.Now.ToString();
                    string frm_date = Convert.ToDateTime(txt_frm_date).Date.ToString("yyyy-MM-dd");
                    string to_date = Convert.ToDateTime(txt_to_date).AddDays(1).ToString("yyyy-MM-dd");
                    var cate = db.Catagories.ToList();
                    IEnumerable<SelectListItem> catgory = from c in cate
                                                          select new SelectListItem
                                                          {
                                                              Text = c.CatName,
                                                              Value = c.CatID.ToString()
                                                          };
                    ViewBag.category = new SelectList(catgory, "Value", "Text");
                    var entries = db.show_vendor_product_details(0, null, vendorId, null, null, null, null, null, 50, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
                    return View(entries);
                }
            }
            catch
            {
                return View();
            }
        }
        /// <summary>
        /// Filters the product detail statistics report by category, status, top count, and date range.
        /// </summary>
        /// <param name="Cateid">The category ID to filter by, or empty for all categories.</param>
        /// <param name="ddl_status">The status filter (e.g., "Active", "Inactive"), or empty for all.</param>
        /// <param name="ddl_top">The maximum number of records to display.</param>
        /// <param name="txt_frm_date">The start date of the filter date range.</param>
        /// <param name="txt_to_date">The end date of the filter date range.</param>
        /// <returns>The Product_details view filtered by the specified parameters.</returns>
        [HttpPost]
        public ActionResult Product_details(string Cateid, string ddl_status, int ddl_top, string txt_frm_date, string txt_to_date)
        {
            ViewBag.chk = "post";
            var cateidval = "";
            var ddlstsus = "";
            try
            {
                if (Cateid == "")
                {
                    cateidval = null;
                }
                else
                {
                    cateidval = Cateid;
                }
                if (ddl_status == "")
                {
                    ddlstsus = null;
                }
                else
                {
                    ddlstsus = ddl_status;
                }
                var vendorId = User.Identity.GetUserId();
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    var cate = db.Catagories.ToList();
                    IEnumerable<SelectListItem> catgory = from c in cate
                                                          select new SelectListItem
                                                          {
                                                              Text = c.CatName,
                                                              Value = c.CatID.ToString()
                                                          };
                    ViewBag.category = new SelectList(catgory, "Value", "Text");
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

                    var entries = db.show_vendor_product_details(0, ddlstsus, vendorId, null, cateidval, null, null, null, ddl_top, frm_date, to_date).ToList();
                    return View(entries);
                }
            }
            catch
            {
                return View();
            }
        }
        //Change Password
        /// <summary>
        /// Displays the change password form for the currently authenticated vendor.
        /// </summary>
        /// <returns>The ChangePassword view.</returns>
        [HttpGet]
        public ActionResult ChangePassword()
        {

            return View();
        }
        //
        // POST: /Manage/ChangePassword
        /// <summary>
        /// Processes the vendor's password change request. Validates the model, changes the password
        /// via the Identity framework, and re-signs in the user on success.
        /// </summary>
        /// <param name="model">The view model containing the old password and the new password.</param>
        /// <returns>
        /// Redirects to <see cref="ChangePassword"/> on success, or returns the view with
        /// validation errors on failure.
        /// </returns>
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

        /// <summary>
        /// Adds Identity errors from the given result to the ModelState dictionary,
        /// so they can be displayed as validation messages in the view.
        /// </summary>
        /// <param name="result">The <see cref="IdentityResult"/> containing any error messages to add.</param>
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
        //Profile
        /// <summary>
        /// Displays the vendor's profile page, including personal details, state/district names,
        /// and the associated admin contact information.
        /// </summary>
        /// <returns>The Profile view populated with the vendor's user and detail records.</returns>
        [HttpGet]
        public new ActionResult Profile()
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                string userid = User.Identity.GetUserId();
                var userDetails = db.Users.Where(a => a.UserId == userid).SingleOrDefault();
                var ch = db.Vendor_details.FirstOrDefault(a => a.userid == userid);
                //show  Vendor Profile details
                ViewBag.vendor = ch;
                ViewBag.image = ch.Photo;
                int state = ch.state;
                int district = ch.district;
                var gt = db.State_Desc.Where(a => a.State_id == state).SingleOrDefault().State_name;
                ViewBag.ddlstate = gt;
                var cities = db.District_Desc.Where(c => c.Dist_id == district && c.State_id == state).SingleOrDefault().Dist_Desc;
                ViewBag.district = cities;
                //Admin Details
                var admin = db.Admin_details.FirstOrDefault();
                ViewBag.admin = admin;

                return View(userDetails);
            }
        }

        //Edit Profile
        /// <summary>
        /// Displays the vendor profile edit form, pre-populated with the current vendor details
        /// and the state and district dropdown lists.
        /// </summary>
        /// <param name="idno">The primary key of the vendor detail record to edit.</param>
        /// <returns>The Edit_Profile view with the vendor's current details.</returns>
        public ActionResult Edit_Profile(int idno)
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {

                var show = db.Vendor_details.Find(idno);
                ViewBag.Sate = db.State_Desc.Select(a => new SelectListItem { Text = a.State_name, Value = a.State_id.ToString() }).ToList();
                ViewBag.District = db.District_Desc.Where(a => a.State_id == show.state).Select(a => new SelectListItem { Text = a.Dist_Desc, Value = a.Dist_id.ToString() }).ToList();
                return View(show);
            }
        }
        /// <summary>
        /// Processes the submission of the vendor profile edit form. Updates all profile fields
        /// including personal info, bank details, and optionally a new profile photo.
        /// </summary>
        /// <param name="idno">The primary key of the vendor detail record to update.</param>
        /// <param name="vendor">The updated vendor detail values submitted from the form.</param>
        /// <returns>Redirects to <see cref="Profile"/> after saving the changes.</returns>
        [HttpPost]
        public ActionResult Edit_Profile(int idno, Vendor_details vendor)
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
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    var ch = db.Vendor_details.Single(a => a.idno == idno);
                    ch.Name = string.IsNullOrWhiteSpace(vendor.Name) ? "" : vendor.Name;
                    ch.Firmname = string.IsNullOrWhiteSpace(vendor.Firmname) ? "" : vendor.Firmname;
                    ch.state = vendor.state;
                    ch.district = vendor.district;
                    ch.address = string.IsNullOrWhiteSpace(vendor.address) ? "" : vendor.address;
                    ch.aadharnumber = string.IsNullOrWhiteSpace(vendor.aadharnumber) ? "" : vendor.aadharnumber;
                    ch.pancardnumber = string.IsNullOrWhiteSpace(vendor.pancardnumber) ? "" : vendor.pancardnumber;
                    ch.gstnumber = string.IsNullOrWhiteSpace(vendor.gstnumber) ? "" : vendor.gstnumber;
                    ch.accountholder = string.IsNullOrWhiteSpace(vendor.accountholder) ? "" : vendor.accountholder;
                    ch.Bankaccountno = string.IsNullOrWhiteSpace(vendor.Bankaccountno) ? "" : vendor.Bankaccountno;
                    ch.Ifsccode = string.IsNullOrWhiteSpace(vendor.Ifsccode) ? "" : vendor.Ifsccode;
                    ch.bankname = string.IsNullOrWhiteSpace(vendor.bankname) ? "" : vendor.bankname;
                    ch.gstnumber = string.IsNullOrWhiteSpace(vendor.gstnumber) ? "" : vendor.gstnumber;
                    ch.pin = string.IsNullOrWhiteSpace(vendor.pin) ? "" : vendor.pin;
                    ch.Photo = string.IsNullOrWhiteSpace(imagePath) ? ch.Photo : imagePath;
                    db.SaveChanges();
                    ViewBag.Sate = db.State_Desc.Select(a => new SelectListItem { Text = a.State_name, Value = a.State_id.ToString() }).ToList();
                    ViewBag.District = db.District_Desc.Select(a => new SelectListItem { Text = a.Dist_Desc, Value = a.Dist_id.ToString() }).ToList();
                    return RedirectToAction("Profile");
                }
            }
            catch
            {

            }

            return RedirectToAction("Profile");
        }

        //Bind District
        /// <summary>
        /// Returns a JSON list of districts belonging to the specified state.
        /// Used to dynamically populate the district dropdown when a state is selected.
        /// </summary>
        /// <param name="Id">The state ID used to filter districts.</param>
        /// <returns>A JSON array of district name/value pairs for use in a dropdown.</returns>
        public JsonResult DistrictList(int Id)
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                var district = from s in db.District_Desc
                               where s.State_id == Id
                               select s;
                ViewBag.district = new SelectList(district, "Dist_id", "Dist_Desc").ToList();
                return Json(new SelectList(district.ToArray(), "Dist_id", "Dist_Desc"), JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// Displays today's order list for the currently authenticated vendor.
        /// Joins transaction histories with product and retailer details to build the order view model.
        /// </summary>
        /// <returns>The OrderList view with today's orders, or an empty view on error.</returns>
        public ActionResult OrderList()
        {
            try
            {
                ViewBag.success = TempData["Success"];
                ViewBag.failed = TempData["Failed"];
                TempData.Remove("Success");
                TempData.Remove("Failed");
                var vendorId = User.Identity.GetUserId();
         
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    DateTime txt_frm_date = DateTime.Now.Date;
                    DateTime txt_to_date = DateTime.Now.AddDays(1);
                    var cate = db.Catagories.ToList();
                    IEnumerable<SelectListItem> catgory = from c in cate
                                                          select new SelectListItem
                                                          {
                                                              Text = c.CatName,
                                                              Value = c.CatID.ToString()
                                                          };
                    ViewBag.category = new SelectList(catgory, "Value", "Text");
                    var lstOrderRequest = db.EcommTransactionHistories.Join(db.Products, eth => eth.ProductId,
                       p => p.ProductID, (eth, p) => new { eth, p }).Join(db.Retailer_Details,
                       xx => xx.eth.BuyerId, r => r.RetailerId, (xx, r) => new OrderListVM
                       {
                           Amount = xx.eth.Amount,
                           BuyerID = xx.eth.BuyerId,
                           BuyerName = r.RetailerName,
                           BuyerPhone = r.Mobile,
                           Buyeraddress = r.Address,
                           BuyerRole = xx.eth.BuyerRole,
                           Idno = xx.eth.Idno,
                           ListPrice = xx.eth.ListPrice,
                           OrderStatus = xx.eth.OrderStatus,
                           OrderID = xx.eth.OrderId,
                           ProductId = xx.eth.ProductId,
                           ProductName = xx.p.Name,
                           OrderDate = xx.eth.OrderDate,
                           ProductPhoto = xx.p.ProductPhotoes.FirstOrDefault().URL,
                           Qty = xx.eth.QTY,
                           StandardPrice = xx.eth.StandardPrice,
                           VenderID = xx.eth.VenderId,
                           DeliverOn = xx.eth.DeliveredOn,
                           DeliveryStatus = ((DeliveryStatus)xx.eth.DeliveryStatus)

                       })
                       .Where(m => m.VenderID == vendorId && m.OrderDate >= txt_frm_date && m.OrderDate <= txt_to_date).OrderByDescending(p => p.Idno).ToList();
                    return View(lstOrderRequest);
                }
            }
            catch 
            {
                return View();
            }
        }

        /// <summary>
        /// Filters the vendor's order list by category, order status, record count, and date range.
        /// Supports all four combinations of category/status filters.
        /// </summary>
        /// <param name="Cateid">The category ID filter, or empty string for all categories.</param>
        /// <param name="ddl_status">The order status filter (e.g., "1" for pending, "2" for success).</param>
        /// <param name="ddl_top">The maximum number of records to retrieve.</param>
        /// <param name="txt_frm_date">The start date of the filter range.</param>
        /// <param name="txt_to_date">The end date of the filter range.</param>
        /// <returns>The OrderList view filtered by the specified parameters, or an empty view on error.</returns>
        [HttpPost]
        public ActionResult OrderList(string Cateid, string ddl_status, int ddl_top, string txt_frm_date, string txt_to_date)
        {
            try
            {
                var vendorId = User.Identity.GetUserId();
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    int catid = 0;
                    int ddlstatus = 0;
                           ViewBag.chk = "post";
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
                      
                        var cate = db.Catagories.ToList();
                    IEnumerable<SelectListItem> catgory = from c in cate
                                                          select new SelectListItem
                                                          {
                                                              Text = c.CatName,
                                                              Value = c.CatID.ToString()
                                                          };
                    ViewBag.category = new SelectList(catgory, "Value", "Text");

                    if (Cateid == "" && ddl_status == "")
                    {
                        var lstOrderRequest = db.EcommTransactionHistories.Join(db.Products, eth => eth.ProductId,
                           p => p.ProductID, (eth, p) => new { eth, p }).Join(db.Retailer_Details,
                           xx => xx.eth.BuyerId, r => r.RetailerId, (xx, r) => new OrderListVM
                           {
                               Amount = xx.eth.Amount,
                               BuyerID = xx.eth.BuyerId,
                               BuyerName = r.RetailerName,
                               BuyerPhone = r.Mobile,
                               Buyeraddress = r.Address,
                               BuyerRole = xx.eth.BuyerRole,
                               Idno = xx.eth.Idno,
                               ListPrice = xx.eth.ListPrice,
                               OrderStatus = xx.eth.OrderStatus,
                               OrderID = xx.eth.OrderId,
                               ProductId = xx.eth.ProductId,
                               ProductName = xx.p.Name,
                               OrderDate = xx.eth.OrderDate,
                               ProductPhoto = xx.p.ProductPhotoes.FirstOrDefault().URL,
                               Qty = xx.eth.QTY,
                               StandardPrice = xx.eth.StandardPrice,
                               VenderID = xx.eth.VenderId,
                               DeliverOn = xx.eth.DeliveredOn,
                               DeliveryStatus = ((DeliveryStatus)xx.eth.DeliveryStatus)
                           })
                           .Where(m => m.VenderID == vendorId && m.OrderDate >= frm_date && m.OrderDate<= to_date).Take(ddl_top).OrderByDescending(p => p.Idno).ToList();
                        return View(lstOrderRequest);
                    }
                    if (Cateid != "" && ddl_status != "")
                    {
                        ddlstatus = Convert.ToInt32(ddl_status);
                        catid = Convert.ToInt32(Cateid);
                        var lstOrderRequest = db.EcommTransactionHistories.Join(db.Products, eth => eth.ProductId,
                           p => p.ProductID, (eth, p) => new { eth, p }).Join(db.Retailer_Details,
                           xx => xx.eth.BuyerId, r => r.RetailerId, (xx, r) => new OrderListVM
                           {
                               Amount = xx.eth.Amount,
                               BuyerID = xx.eth.BuyerId,
                               BuyerName = r.RetailerName,
                               BuyerPhone = r.Mobile,
                               Buyeraddress = r.Address,
                               BuyerRole = xx.eth.BuyerRole,
                               Idno = xx.eth.Idno,
                               ListPrice = xx.eth.ListPrice,
                               OrderStatus = xx.eth.OrderStatus,
                               OrderID = xx.eth.OrderId,
                               ProductId = xx.eth.ProductId,
                               ProductName = xx.p.Name,
                               OrderDate = xx.eth.OrderDate,
                               ProductPhoto = xx.p.ProductPhotoes.FirstOrDefault().URL,
                               Qty = xx.eth.QTY,
                               StandardPrice = xx.eth.StandardPrice,
                               VenderID = xx.eth.VenderId,
                               DeliverOn = xx.eth.DeliveredOn,
                               DeliveryStatus = ((DeliveryStatus)xx.eth.DeliveryStatus)
                           })
                           .Where(m => m.VenderID == vendorId && m.OrderDate >= frm_date && m.OrderDate <= to_date && m.OrderStatus== ddlstatus && m.ProductId== catid).Take(ddl_top).OrderByDescending(p => p.Idno).ToList();
                        return View(lstOrderRequest);
                    }
                    if (Cateid != "" && ddl_status == "")
                    {
                         catid = Convert.ToInt32(Cateid);
                        var lstOrderRequest = db.EcommTransactionHistories.Join(db.Products, eth => eth.ProductId,
                           p => p.ProductID, (eth, p) => new { eth, p }).Join(db.Retailer_Details,
                           xx => xx.eth.BuyerId, r => r.RetailerId, (xx, r) => new OrderListVM
                           {
                               Amount = xx.eth.Amount,
                               BuyerID = xx.eth.BuyerId,
                               BuyerName = r.RetailerName,
                               BuyerPhone = r.Mobile,
                               Buyeraddress = r.Address,
                               BuyerRole = xx.eth.BuyerRole,
                               Idno = xx.eth.Idno,
                               ListPrice = xx.eth.ListPrice,
                               OrderStatus = xx.eth.OrderStatus,
                               OrderID = xx.eth.OrderId,
                               ProductId = xx.eth.ProductId,
                               ProductName = xx.p.Name,
                               OrderDate = xx.eth.OrderDate,
                               ProductPhoto = xx.p.ProductPhotoes.FirstOrDefault().URL,
                               Qty = xx.eth.QTY,
                               StandardPrice = xx.eth.StandardPrice,
                               VenderID = xx.eth.VenderId,
                               DeliverOn = xx.eth.DeliveredOn,
                               DeliveryStatus = ((DeliveryStatus)xx.eth.DeliveryStatus)
                           })
                           .Where(m => m.VenderID == vendorId && m.OrderDate >= frm_date && m.OrderDate <= to_date  && m.ProductId == catid).Take(ddl_top).OrderByDescending(p => p.Idno).ToList();
                        return View(lstOrderRequest);
                    }
                    if (Cateid == "" && ddl_status != "")
                    {
                       ddlstatus = Convert.ToInt32(ddl_status);
                        var lstOrderRequest = db.EcommTransactionHistories.Join(db.Products, eth => eth.ProductId,
                           p => p.ProductID, (eth, p) => new { eth, p }).Join(db.Retailer_Details,
                           xx => xx.eth.BuyerId, r => r.RetailerId, (xx, r) => new OrderListVM
                           {
                               Amount = xx.eth.Amount,
                               BuyerID = xx.eth.BuyerId,
                               BuyerName = r.RetailerName,
                               BuyerPhone = r.Mobile,
                               Buyeraddress = r.Address,
                               BuyerRole = xx.eth.BuyerRole,
                               Idno = xx.eth.Idno,
                               ListPrice = xx.eth.ListPrice,
                               OrderStatus = xx.eth.OrderStatus,
                               OrderID = xx.eth.OrderId,
                               ProductId = xx.eth.ProductId,
                               ProductName = xx.p.Name,
                               OrderDate = xx.eth.OrderDate,
                               ProductPhoto = xx.p.ProductPhotoes.FirstOrDefault().URL,
                               Qty = xx.eth.QTY,
                               StandardPrice = xx.eth.StandardPrice,
                               VenderID = xx.eth.VenderId,
                               DeliverOn = xx.eth.DeliveredOn,
                               DeliveryStatus = ((DeliveryStatus)xx.eth.DeliveryStatus)
                           })
                           .Where(m => m.VenderID == vendorId && m.OrderDate >= frm_date && m.OrderDate <= to_date && m.OrderStatus == ddlstatus).Take(ddl_top).OrderByDescending(p => p.Idno).ToList();
                        return View(lstOrderRequest);
                    }
                }
                return View();
            }
            catch 
            {
                return View();
            }
        }
        /// <summary>
        /// Returns all orders for the vendor as a partial view, optionally setting a success or
        /// failed message in TempData based on the <paramref name="type"/> parameter.
        /// </summary>
        /// <param name="type">The message type: "Success" or "Failed".</param>
        /// <param name="Message">The message text to store in TempData.</param>
        /// <returns>A partial view <c>_orderListPartail</c> with the vendor's full order list.</returns>
        public ActionResult OrderList1(string type, string Message)
        {
            try
            {
                if (type == "Success")
                {
                    TempData["Success"] = Message;
                }
                if (type == "Failed")
                {
                    TempData["Failed"] = Message;
                }
                var vendorId = User.Identity.GetUserId();
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    var lstOrderRequest = db.EcommTransactionHistories.Join(db.Products, eth => eth.ProductId,
                       p => p.ProductID, (eth, p) => new { eth, p }).Join(db.Retailer_Details,
                       xx => xx.eth.BuyerId, r => r.RetailerId, (xx, r) => new OrderListVM
                       {
                           Amount = xx.eth.Amount,
                           BuyerID = xx.eth.BuyerId,
                           BuyerName = r.RetailerName,
                           BuyerPhone = r.Mobile,
                           Buyeraddress = r.Address,
                           BuyerRole = xx.eth.BuyerRole,
                           Idno = xx.eth.Idno,
                           ListPrice = xx.eth.ListPrice,
                           OrderStatus = xx.eth.OrderStatus,
                           OrderID = xx.eth.OrderId,
                           ProductId = xx.eth.ProductId,
                           ProductName = xx.p.Name,
                           OrderDate = xx.eth.OrderDate,
                           ProductPhoto = xx.p.ProductPhotoes.FirstOrDefault().URL,
                           Qty = xx.eth.QTY,
                           StandardPrice = xx.eth.StandardPrice,
                           VenderID = xx.eth.VenderId

                       })
                       .Where(m => m.VenderID == vendorId).OrderByDescending(p => p.Idno).ToList();
                    return PartialView("_orderListPartail", lstOrderRequest);
                }
            }
            catch 
            {
                throw;
            }
        }
        
        //show all buyer details
        /// <summary>
        /// Returns all detail records for the specified buyer (retailer) as a JSON array.
        /// Used to display buyer information in the order management view.
        /// </summary>
        /// <param name="buyerid">The unique user ID of the buyer (retailer) to look up.</param>
        /// <returns>A JSON array containing all detail records for the specified buyer.</returns>
        public JsonResult Showallbuyerdetails(string buyerid)
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                var ch = db.Show_all_retailer_details(buyerid).ToList();
                return Json(ch.ToArray(), JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// Updates the fulfillment/approval status of a specific order item.
        /// Calls <c>proc_ApproveRejectShopingOrder</c> and returns a JSON result indicating
        /// whether the approval was successful or failed.
        /// </summary>
        /// <param name="idno">The transaction history record ID.</param>
        /// <param name="OrderId">The order ID to update.</param>
        /// <param name="status">The new status to apply (e.g., "Approved", "Rejected").</param>
        /// <param name="retailerid">The retailer/buyer user ID for the order.</param>
        /// <param name="role">The role of the buyer associated with the order.</param>
        /// <returns>A JSON object with <c>Status</c>, <c>Message</c>, and <c>Type</c> fields.</returns>
        public ActionResult UpdateOrderStatus(int idno, int OrderId, string status, string retailerid, string role)
        {
            try
            {
                var vendorId = User.Identity.GetUserId();
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    if (idno > 0 && OrderId > 0 && !string.IsNullOrWhiteSpace(status))
                    {
                        System.Data.Entity.Core.Objects.ObjectParameter output = new System.Data.Entity.Core.Objects.ObjectParameter("output", typeof(string));
     
                        var procResult = db.proc_ApproveRejectShopingOrder(retailerid, role, idno, OrderId, status, output).SingleOrDefault().msg;
                        if (procResult == "Product Purchase Approved Successfully.")
                        {
                            return Json(new { Status = true, Message = procResult, Type = "Success" });

                        }
                        else
                        {
                            return Json(new { Status = true, Message = procResult, Type = "Failed" });
                        }
                    }
                    return Json(new { Status = false, Message = "Interal Error" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, Message = ex.Message });
            }
            //return Json("Failed"); 
        }
        /// <summary>
        /// Generates and returns a PDF of the e-commerce transaction invoice for the specified order.
        /// Retrieves the user's role and fetches the transaction history before rendering the PDF.
        /// </summary>
        /// <param name="OrderID">The order ID for which to generate the invoice PDF.</param>
        /// <returns>A PDF view rendered from the e-commerce transaction history.</returns>
        [HttpGet]
        public ActionResult EcommInvoicePdf(int OrderID)
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                var userid = User.Identity.GetUserId();
                var roleName = db.proc_getRoleByUserID(userid).SingleOrDefault().Name;
                var procResult = db.proc_getEcommTransactionHistory(null, OrderID, roleName, null, null, null).ToList();
                return new Rotativa.ViewAsPdf(procResult);
            }
        }

        /// <summary>
        /// Updates the delivery status of an order after verifying the provided OTP.
        /// Calls <c>proc_UpdateDeliveryStatusByVendorID</c> and returns a JSON result.
        /// </summary>
        /// <param name="orderId">The order ID whose delivery status is to be updated.</param>
        /// <param name="otp">The one-time password provided to confirm delivery.</param>
        /// <returns>A JSON object with <c>Status</c> ("Success" or "Failed") and a <c>Message</c>.</returns>
        [HttpPost]
        public JsonResult UpdateDeliveryStatus(int orderId,int otp)
        {
            try
            {
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    var vendorId = User.Identity.GetUserId();
                    System.Data.Entity.Core.Objects.ObjectParameter Status = new System.Data.Entity.Core.Objects.ObjectParameter("Status",typeof(Boolean));
                    System.Data.Entity.Core.Objects.ObjectParameter Message = new System.Data.Entity.Core.Objects.ObjectParameter("Message", typeof(string));
                    var procResult = db.proc_UpdateDeliveryStatusByVendorID(vendorId, 2, otp, orderId, Status, Message).FirstOrDefault();
                    if(Convert.ToBoolean(Status.Value))
                    {
                        return Json(new { Status ="Success",Message=Convert.ToString(Message.Value)});
                    }
                    else
                    {
                        return Json(new { Status = "Failed", Message = Convert.ToString(Message.Value) });
                    }
                }
            }
            catch(Exception ex)
            {
                return Json(new { Status = "Failed", Message = ex.Message });
            }
        }
        #region WalletToBankAmountTransfer
        /// <summary>
        /// Displays the wallet-to-bank transfer page, showing the available transfer charge
        /// slabs configured for the "Seller" role.
        /// </summary>
        /// <returns>The WalletToBankAmountTransfer view with the applicable charge slabs.</returns>
        public ActionResult WalletToBankAmountTransfer()
        {
            var entries = db.WalletToBankAmountTransferCharges.Where(a => a.UserRole == "Seller").ToList();
            return View(entries);
        }
        /// <summary>
        /// Displays today's wallet unload (withdrawal) request report for the currently authenticated vendor.
        /// </summary>
        /// <returns>The WalletUnloadReport view with today's transfer request records.</returns>
        [HttpGet]
        public ActionResult WalletUnloadReport()
        {
            var userid = User.Identity.GetUserId();
            DateTime todayStart = DateTime.Now.Date;
            DateTime todayEnd = DateTime.Now.AddDays(1).Date;
            var entries = db.VendorWalletToBankAmountTransferRequests.Where(a => a.VendorId == userid && a.RequestDate >= todayStart).ToList();
            return View(entries);
        }
        /// <summary>
        /// Filters the wallet unload (withdrawal) request report by the specified date range.
        /// </summary>
        /// <param name="txt_frm_date">The start date for filtering wallet unload requests.</param>
        /// <param name="txt_to_date">The end date for filtering wallet unload requests.</param>
        /// <returns>The WalletUnloadReport view filtered by the specified date range.</returns>
        [HttpPost]
        public ActionResult WalletUnloadReport(string txt_frm_date, string txt_to_date)
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
            var entries = db.VendorWalletToBankAmountTransferRequests.Where(a => a.VendorId == userid && a.RequestDate >= frm_date && a.RequestDate <= to_date).ToList();
            return View(entries);
        }
        /// <summary>
        /// Submits a wallet-to-bank transfer request for the currently authenticated vendor.
        /// Validates that all required bank details are present on the vendor profile.
        /// Depending on the system configuration, either processes the request via admin approval
        /// or directly via the payment API.
        /// </summary>
        /// <param name="Amount">The amount to transfer from the wallet to the bank account.</param>
        /// <param name="Type">The transfer type (e.g., "IMPS", "NEFT", "RTGS").</param>
        /// <returns>A serialized JSON string with <c>status</c> ("Success" or "Failed") and a <c>Message</c>.</returns>
        [HttpPost]
        public ActionResult AddWalletToBankRequest(decimal Amount, string Type)
        {
            var userid = User.Identity.GetUserId();
            if (Amount > 0 && !string.IsNullOrWhiteSpace(Type))
            {
                var vendorDetails = db.Vendor_details.Where(a => a.userid == userid).FirstOrDefault();
                if (vendorDetails != null)
                {
                    string str = string.Empty;
                    bool IsUpdateRequired = false;
                    if (vendorDetails.Bankaccountno == null || string.IsNullOrWhiteSpace(vendorDetails.Bankaccountno))
                    {
                        str = str + "BankAccount No,";
                        IsUpdateRequired = true;
                    }
                    if (vendorDetails.bankname == null || string.IsNullOrWhiteSpace(vendorDetails.bankname))
                    {
                        str = str + "BankName,";
                        IsUpdateRequired = true;
                    }
                    if (vendorDetails.Ifsccode == null || string.IsNullOrWhiteSpace(vendorDetails.Ifsccode))
                    {
                        str = str + "IFSC Code,";
                        IsUpdateRequired = true;
                    }
                    if (vendorDetails.accountholder == null || string.IsNullOrWhiteSpace(vendorDetails.accountholder))
                    {
                        str = str + "Account Holder Name,";
                        IsUpdateRequired = true;
                    }
                    if (IsUpdateRequired)
                    {
                        if (str.EndsWith(","))
                            str = str.Substring(0, str.Length - 1);
                        str = str + " are required to become WalletUnload  Request.";
                        var ViewResponse = new { status = "Failed", Message = str };
                        return Json(JsonConvert.SerializeObject(ViewResponse));
                    }
                    else
                    {
                        RETAILER.Models.Vastbillpay vb = new RETAILER.Models.Vastbillpay();
                        string ProjectName = Path.GetDirectoryName(Path.GetDirectoryName(Server.MapPath(@"~/HomeControllers.cs")));
                        ProjectName = ProjectName.Substring(ProjectName.LastIndexOf("\\") + 1);
                        var RequestId = DateTime.Now.ToString("dd-MM-yyy hh-mm-ss").Replace("-", "").Replace(" ", "") + ProjectName.Substring(0, 4);
                        var Email = vendorDetails.emailid;
                        var AccountNumber = vendorDetails.Bankaccountno;
                        var BankName = vendorDetails.bankname;
                        var IFSCCode = vendorDetails.Ifsccode;
                        var AccountholderName = vendorDetails.accountholder;
                        var Sts = db.walletunloadsts.FirstOrDefault();
                        System.Data.Entity.Core.Objects.ObjectParameter status = new System.Data.Entity.Core.Objects.ObjectParameter("Status", typeof(string));
                        System.Data.Entity.Core.Objects.ObjectParameter Message = new System.Data.Entity.Core.Objects.ObjectParameter("Message", typeof(string));
                        if (Sts.Status == "Y" && Sts.Type == "Admin")
                        {

                            var response = db.proc_InsertVendorWalletToBankAmountTransfer(userid, Amount, Type, RequestId, status, Message).FirstOrDefault();
                            if (response.Status == "Success")
                            {
                                var ViewResponse = new { status = "Success", Message = "Request proceeded successfully." };
                                return Json(JsonConvert.SerializeObject(ViewResponse));
                            }
                            else
                            {
                                var ViewResponse = new { status = "Failed", Message = response.Message };
                                return Json(JsonConvert.SerializeObject(ViewResponse));
                            }
                        }
                        else
                        {
                            var response = db.proc_InsertVendorWalletToBankAmountTransfer(userid, Amount, Type, RequestId, status, Message).FirstOrDefault();
                            if (response.Status == "Success")
                            {
                                var tokn = Responsetoken.gettoken();
                                var responsechk1 = vb.WalletUnloadrRquest(tokn, Email, Amount, "", AccountNumber, BankName, IFSCCode, AccountholderName, RequestId,Type);
                                var responsechk = responsechk1.Content.ToString();
                                dynamic json = JsonConvert.DeserializeObject(responsechk);
                                var respcode = json.Content.ResponseCode.ToString();
                                var ADDINFO = json.Content.ADDINFO.ToString();
                                dynamic json1 = JsonConvert.DeserializeObject(ADDINFO);
                                var status1 = json1.Status.ToString();
                                var msg = json1.Message.ToString();
                                if (status1 == "Success")
                                {

                                    var ViewResponse = new { status = "Success", Message = "Request proceeded successfully." };
                                    return Json(JsonConvert.SerializeObject(ViewResponse));
                                }
                                else if (status1 == "Failed")
                                {
                                    var pp = db.VendorWalletToBankAmountTransferRequests.Where(a => a.Status == "Pending" && a.RequestId == RequestId).FirstOrDefault();
                                    if (pp != null)
                                    {
                                        var response1 = db.proc_InsertVendorWalletToBankAmountTransferRefund(pp.Idno, status, Message).FirstOrDefault();
                                        if (response1.Status == "Success")
                                        {
                                            var ViewResponse = new { status = "Success", Message = "Rejected successfully." };
                                            return Json(JsonConvert.SerializeObject(ViewResponse));
                                        }
                                        else
                                        {
                                            var ViewResponse = new { status = "Failed", Message = response.Message };
                                            return Json(JsonConvert.SerializeObject(ViewResponse));
                                        }
                                    }
                                }
                                else
                                {
                                    var ViewResponse = new { status = "Failed", Message = "something went wrong." };
                                    return Json(JsonConvert.SerializeObject(ViewResponse));
                                }
                            }
                            else
                            {
                                var ViewResponse = new { status = "Failed", Message = response.Message };
                                return Json(JsonConvert.SerializeObject(ViewResponse));
                            }

                        }

                    }
                }
                return View();
            }
            else
            {
                var ViewResponse = new { status = "Failed", Message = "Amount and transfer mode is mandatory" };
                return Json(JsonConvert.SerializeObject(ViewResponse));
            }

        }
        #endregion
    }
}