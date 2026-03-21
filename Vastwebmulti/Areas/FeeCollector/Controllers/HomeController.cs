using LinqToExcel;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using Rotativa;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.Data.OleDb;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Vastwebmulti.Areas.FeeCollector.Models;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.FeeCollector.Controllers
{

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
        private VastwebmultiEntities db;
        public HomeController()
        {
            db = new VastwebmultiEntities();
        }
        // GET: FeeCollector/Home
        public ActionResult Index()
        {
            var userid = User.Identity.GetUserId();
            var vv = db.FeeCollector_details.SingleOrDefault(a => a.FCId == userid);
            ViewBag.email = vv.Email;
            ViewBag.image = vv.Photo;
            //Admin Contact Details
            var contact = db.Admin_details.SingleOrDefault();
            ViewBag.mobile1 = contact.mobile;
            ViewBag.mobile2 = contact.mobile1;
            ViewBag.customercareno = contact.CustomerCare_No;
            ViewBag.email1 = contact.email;
            ViewBag.email2 = contact.email1;
            //show News for master
            ViewBag.news = (from pp in db.Message_top where (pp.users == "Distributor" || pp.users == "All") where pp.status == "Y" select pp).ToList();
            ////Retailers
            //var stackedchart = db.show_all_active_inactive_rem_list().ToList();
            //int actRtl = stackedchart.Where(a => a.type == "ACTIVE").Count();
            //ViewData["actRtl"] = actRtl;
            //var inRtl = stackedchart.Where(a => a.type == "INACTIVE").Count();
            //ViewData["inRtl"] = inRtl;
            //int total = actRtl + inRtl;
            //ViewData["TotalRetailrs"] = total;
            //Upcomming Holiday
            List<show_upcomming_holiday_Result> li = new List<show_upcomming_holiday_Result>();
            ViewBag.showholiday = li;
            return View();
        }
        [HttpGet]
        public ActionResult FixedFeeCharges()
        {
            var userid = User.Identity.GetUserId();
            FixedFeeChargeModel model = new FixedFeeChargeModel();
            model.lstFeeCharges = db.FixedFeeCharges.Where(a => a.FCID == userid).ToList();
            return View(model);
        }
        [HttpPost]
        public ActionResult insertFixedFeeCharges(FixedFeeChargeModel model)
        {
            if(!ModelState.IsValid)
            {
                return View("FixedFeeCharges", model);
            }
            var userid = User.Identity.GetUserId();
            int dd = (int)model.stdClass;
            if (!db.FixedFeeCharges.Any(a=>a.FCID == userid && a.FeeName.ToLower() == model.FeeName.ToLower() && a.stdClass == dd.ToString()))
            {
                FixedFeeCharge entry = new FixedFeeCharge();
                entry.FCID = User.Identity.GetUserId();
                entry.FeeName = model.FeeName;
                entry.FeeValue = model.FeeValue;
                entry.stdClass = dd.ToString();
                db.FixedFeeCharges.Add(entry);
                db.SaveChanges();
            }
            
            return RedirectToAction("FixedFeeCharges");
        }
        //[HttpPost]
        //public ActionResult GetSession(string stdBoard)
        //{
        //    var AcadmicSession = string.Empty;
        //    if(!string.IsNullOrWhiteSpace(stdBoard) && stdBoard != "CBSE")
        //    {
        //        if(DateTime.Now.Month > 5)
        //        {
        //            AcadmicSession = DateTime.Now.AddYears(-1).Year.ToString() + "-" + DateTime.Now.Year.ToString();
        //        }
        //        else
        //        {
        //            AcadmicSession = DateTime.Now.Year.ToString() + "-" + DateTime.Now.AddYears(1).Year.ToString();
        //        }
        //    }
        //    return Json(AcadmicSession);
        //}
        public ActionResult Students()
        {
            var userid = User.Identity.GetUserId();
            if (db.FixedFeeCharges.Count(a => a.FCID == userid) <= 0)
            {
                TempData["mobileno"] = "Please Complete FixedCharge Entry Process First.";
            }
            ViewData["value"] = "";
            //ViewBag.state = new SelectList(db.Select_State_Details(), "State_Id", "State_Name").ToList();
            //var district = from s in db.District_Desc
            //               where s.State_id == 0
            //               select s;
            //ViewBag.district = new SelectList(district, "Dist_id", "Dist_Desc").ToList();

            //var results = (from p in db.Slab_name where p.SlabFor == "Master" group p.SlabName by p.SlabName into g select new { SlabName = g.Key });
            //ViewBag.slabname = new SelectList(results, "SlabName", "SlabName");
            var ch = db.Students.Where(a=>a.FeeCollectorID == userid).ToList();
            //TempData.Keep("msgrem");
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem
            {
                Text = DateTime.Now.AddYears(-1).Year.ToString() +"-"+ DateTime.Now.Year.ToString(),
                Value = DateTime.Now.AddYears(-1).Year.ToString() + "-" + DateTime.Now.Year.ToString(),
            });
            list.Add(new SelectListItem
            {
                Text = DateTime.Now.Year.ToString() + "-" + DateTime.Now.AddYears(1).Year.ToString(),
                Value = DateTime.Now.Year.ToString() + "-" + DateTime.Now.AddYears(1).Year.ToString()
            });
            //list.Add(new SelectListItem
            //{
            //    Text = DateTime.Now.AddYears(1).Year.ToString() + "-" + DateTime.Now.AddYears(2).Year.ToString(),
            //    Value = DateTime.Now.AddYears(1).Year.ToString() + "-" + DateTime.Now.AddYears(2).Year.ToString(),
            //});
            ViewBag.AcadmicSession = list;
            StdModel viewModel = new StdModel();
            viewModel.students = ch;
            //  viewModel.show_master_dealer = db.show_master_dealer("").ToList();

            return View(viewModel);
        }
        [HttpPost]
        public ActionResult insert_Std(StdModel model)
        {
            try
            {
                var userid = User.Identity.GetUserId();
                if (db.FixedFeeCharges.Count(a=>a.FCID == userid) <= 0)
                {
                    TempData["mobileno"] = "Please Complete FixedCharge Entry Process First.";
                    return RedirectToAction("Students");
                }
                ViewBag.state = new SelectList(db.Select_State_Details(), "State_Id", "State_Name").ToList();
                var xx = db.Students.Any(u => u.RollNo == model.StdID);
                if (!db.Students.Any(u => u.RollNo == model.StdID.ToString()))
                {
                    Student std = new Student();
                    std.RollNo = model.StdID;
                    std.Name = model.StdName;
                    std.FeeCollectorID = User.Identity.GetUserId();
                    std.FName = model.FName;
                    std.AnnualFee = model.AnnualFee;
                    std.BusFee = model.BusFee;
                    std.Discount = model.Discount;
                    std.BusDiscountEnabled = model.BusDiscountEnabled;
                    std.FeeDiscountEnabled = model.FeeDiscountEnabled;

                    int borad = (int)model.Board;
                    std.Board = borad.ToString();
                    std.CreatedOn = DateTime.Now;
                    std.DOB = DateTime.Now.Date;
                    std.Phone = model.Mb;
                    std.Remark = "";
                
                    std.Section = model.Section;
                    std.stdAddress = model.Address;
                    var value = (int)model.stdClass;
                    std.stdClass = value.ToString();
                    std.AcadmicSession = model.AcadminSession;
                    db.Students.Add(std);
                    db.SaveChanges();
                    return RedirectToAction("Students");
                }
                else
                {
                    TempData["mobileno"] = "Student Already Exists.";
                    return RedirectToAction("Students");
                }
            }
            catch 
            {
                return RedirectToAction("Students");
            }
        }
        public JsonResult RollNoSearch(string id)
        {
            var ch = db.Students.SingleOrDefault(ii => ii.RollNo == id);
            return Json(JsonConvert.SerializeObject(ch), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult EditStudent(string txtRollNo, string StdName1, string FName1, string Mb1,string Address1, string DOB1, string stdClass1,string Section1,string SchoolName1,decimal AnnualFee1,string remark1,string AcadmicSession1,int? Board1)
        {
            try
            {
                Student objCourse = (from p in db.Students
                                                  where p.RollNo == txtRollNo
                                                  select p).SingleOrDefault();
                objCourse.stdAddress = Address1;
                objCourse.AnnualFee = AnnualFee1;
                objCourse.DOB = Convert.ToDateTime(DOB1);
                objCourse.FName = FName1;
                objCourse.Name = StdName1;
                objCourse.Phone = Mb1;
                objCourse.Remark = remark1;
                //objCourse.Board = Board1;//TODO
                objCourse.Section = Section1;
                objCourse.stdAddress = Address1;
                objCourse.stdClass = stdClass1;
                objCourse.AcadmicSession = AcadmicSession1;

                db.SaveChanges();

                TempData["edit"] = "Data Edit Successfully..";
                return RedirectToAction("Students");
            }
            catch
            {
                return RedirectToAction("Students");
            }
        }

        [HttpPost]
        public ActionResult UploadExcel(User users, HttpPostedFileBase FileUpload)
        {

            List<string> data = new List<string>();
            if (FileUpload != null)
            {
                // tdata.ExecuteCommand("truncate table OtherCompanyAssets");  
                if (FileUpload.ContentType == "application/vnd.ms-excel" || FileUpload.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {


                    string filename = FileUpload.FileName;
                    string targetpath = Server.MapPath("~/Doc/");
                    FileUpload.SaveAs(targetpath + filename);
                    string pathToExcelFile = targetpath + filename;
                    var connectionString = "";
                    if (filename.EndsWith(".xls"))
                    {
                        connectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0; data source={0}; Extended Properties=Excel 8.0;", pathToExcelFile);
                    }
                    else if (filename.EndsWith(".xlsx"))
                    {
                        connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\";", pathToExcelFile);
                    }

                    var adapter = new OleDbDataAdapter("SELECT * FROM [Sheet1$]", connectionString);
                    var ds = new DataSet();

                    adapter.Fill(ds, "ExcelTable");

                    DataTable dtable = ds.Tables["ExcelTable"];

                    string sheetName = "Sheet1";

                    var excelFile = new ExcelQueryFactory(pathToExcelFile);
                    var artistAlbums = from a in excelFile.Worksheet<Student>(sheetName) select a;

                    foreach (var a in artistAlbums)
                    {
                        try
                        {
                            
                             //Validation and insert login Here
                             if(!db.Students.Any(aa=>aa.RollNo == a.RollNo))
                            {
                                if (!string.IsNullOrWhiteSpace(a.RollNo) && a.stdAddress != "" && a.stdClass != "")
                                {
                                    Student entry = new Student();
                                    entry.Name = a.Name;
                                    entry.stdAddress = a.stdAddress;
                                    entry.AcadmicSession = a.AcadmicSession;
                                    entry.AnnualFee = a.AnnualFee;
                                    entry.Board = a.Board;
                                    entry.BusFee = a.BusFee;
                                    entry.CreatedOn = DateTime.Now;
                                    entry.DOB = a.DOB;
                                    entry.FeeCollectorID = User.Identity.GetUserId();
                                    entry.FName = a.FName;
                                    entry.Phone = a.Phone;
                                    entry.Remark = a.Remark;
                                    entry.RollNo = a.RollNo;
                                    entry.Section = a.Section;
                                    entry.stdClass = a.stdClass;
                                    entry.Discount = a.Discount;
                                    entry.FeeDiscountEnabled = a.FeeDiscountEnabled;
                                    entry.BusDiscountEnabled = a.BusDiscountEnabled;
                                    db.Students.Add(entry);
                                    db.SaveChanges();
                                }
                                //else
                                //{
                                //    data.Add("<ul>");
                                //    if (a.Name == "" || a.Name == null) data.Add("<li> name is required</li>");
                                //    if (a.Address == "" || a.Address == null) data.Add("<li> Address is required</li>");
                                //    if (a.ContactNo == "" || a.ContactNo == null) data.Add("<li>ContactNo is required</li>");

                                //    data.Add("</ul>");
                                //    data.ToArray();
                                //    return Json(data, JsonRequestBehavior.AllowGet);
                                //}
                            }

                        }

                        catch (DbEntityValidationException ex)
                        {
                            foreach (var entityValidationErrors in ex.EntityValidationErrors)
                            {

                                foreach (var validationError in entityValidationErrors.ValidationErrors)
                                {

                                    Response.Write("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);

                                }

                            }
                        }
                    }
                    //deleting excel file from folder  
                    if ((System.IO.File.Exists(pathToExcelFile)))
                    {
                        System.IO.File.Delete(pathToExcelFile);
                    }
                   
                    TempData["edit"] = "File uploaded successfully.";
                    return RedirectToAction("Students");
                }
                else
                {
                    //alert message for invalid file format  
                    TempData["mobileno"] = "Only Excel file format is allowed.";
                    return RedirectToAction("Students");
                }
            }
            else
            {
                TempData["mobileno"] = "Please choose Excel file.";
                return RedirectToAction("Students");
            }
        }
        [HttpGet]
        public FileResult Download()
        {
            byte[] fileBytes = System.IO.File.ReadAllBytes(Server.MapPath("~/Doc/StudentBook1.xlsx"));
            string fileName = "StudentBook1.xlsx";
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }
        [HttpGet]
        public ActionResult FeeDeposit()
        {
            var userid = User.Identity.GetUserId();
            string txt_frm_date = DateTime.Now.ToString();
            string txt_to_date = DateTime.Now.ToString();
            string frm_date = Convert.ToDateTime(txt_frm_date).Date.ToString("yyyy-MM-dd");
            string to_date = Convert.ToDateTime(txt_to_date).AddDays(1).ToString("yyyy-MM-dd");
            FeeDepositModel model = new FeeDepositModel();
            model.lstFeedeposit = db.proc_getFeedepositDetails(null, null, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).Where(a=>a.FeeCollectorId.ToLower() == userid.ToLower()).ToList();
            return View(model);
        }
        [HttpPost]
        public ActionResult FeeDeposit(string stdRollNO, string txt_frm_date, string txt_to_date)
        {

            var userid = User.Identity.GetUserId();
            ViewBag.fromdate = txt_frm_date;
            ViewBag.todate = txt_to_date;

            DateTime frm = Convert.ToDateTime(txt_frm_date);
            DateTime to = Convert.ToDateTime(txt_to_date);
            if (to < frm)
            {
                ViewBag.format = "The first date will always be smaller or parallel than the second date.";
            }

            txt_frm_date = frm.ToString("dd-MM-yyyy");
            txt_to_date = to.ToString("dd-MM-yyyy");
            string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy",
                            "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
            DateTime dt = !string.IsNullOrWhiteSpace(txt_frm_date) ? DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
            DateTime dt1 = !string.IsNullOrWhiteSpace(txt_to_date) ? DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
            DateTime frm_date = Convert.ToDateTime(dt).Date;
            DateTime to_date = Convert.ToDateTime(dt1).Date.AddDays(1);
            FeeDepositModel model = new FeeDepositModel();
            model.lstFeedeposit = db.proc_getFeedepositDetails(stdRollNO, userid, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            return View(model);
        }
        [HttpPost]
        public ActionResult InsertFeeDeposit(string txtRollNo, decimal InsAmount,decimal InsBusAmount)
        {
            var userid = User.Identity.GetUserId();
            System.Data.Entity.Core.Objects.ObjectParameter output = new System.Data.Entity.Core.Objects.ObjectParameter("response", typeof(string));
            var response = db.proc_DepositFeeInstallmentFromFeeCollector(txtRollNo, userid, InsAmount,InsBusAmount, output).SingleOrDefault().msg;
            if(response.Contains("Success"))
            {
                TempData["msgrem"] = "Payment successfully completed.";
            }else
            {
                TempData["mobileno"] = "Failed to make payment.";
            }
            return RedirectToAction("FeeDeposit");
        }
        [HttpPost]
        public ActionResult getInstallmentAmount(string rollno)
        {
            var userid = User.Identity.GetUserId();
            if(!string.IsNullOrEmpty(rollno))
            {
                var respo = db.proc_GetFeeInstallmentToDeposit(rollno, userid).SingleOrDefault();
                if(respo != null && respo.InstallmentAmountFee != null)
                {
                    var student = db.Students.Single(a=>a.RollNo.ToLower().Contains(rollno.ToString()));
                    var response = new { Status = "Success", Message = new {NumberOfInstallments = respo.InstallmentCount,
                        PaidInstallment = respo.PaidIntallmentCount, Amount = respo.InstallmentAmountFee,
                        AmountBus = respo.InstallmentAmountBusFee,FixedCharge = respo.FixedCharges,
                        Name = student.Name,FName = student.FName,stdClass=student.stdClass,
                        Section = student.Section,Session = student.AcadmicSession,Address = student.stdAddress,ActualFee =respo.ActualFee,
                        ActualBusFee = respo.ActualBusFee,DiscountedFee = respo.DiscountedFee,
                        DiscountedBusFee = respo.DiscountedBusFee} };
                    var output = new JavaScriptSerializer().Serialize(response);
                    return Json(output);
                }
                else
                {
                    var response = new { Status = "Failed", Message = "Invalid or Empty Roll Number" };
                    var output = new JavaScriptSerializer().Serialize(response);
                    return Json(output);
                }
               
            }
            else
            {
                var response = new { Status = "Failed", Message = "Invalid or Empty Roll Number" };
                var output = new JavaScriptSerializer().Serialize(response);
                return Json(output);
            }
            
            //return Json();
        }
        [HttpGet]
        public ActionResult Logo()
        {
            var userid = User.Identity.GetUserId();
            var result = db.FeeCollector_details.Where(a => a.FCId == userid);
        
            return View(result);
        }
        [HttpPost]
        public ActionResult Logo(string f)
        {
                var userid = User.Identity.GetUserId();
                WebImage photo = null;
                var newFileName = "";
                var imagePath = "";
                photo = WebImage.GetImageFromRequest();
                if (photo != null)
                {
                    newFileName = Guid.NewGuid().ToString() + "_" +
                        Path.GetFileName(photo.FileName);
                    imagePath = @"images\" + newFileName;
                    photo.Resize(110, 110, false, true);
                    photo.Save(@"~\" + imagePath);
                }
                var fc = db.FeeCollector_details.SingleOrDefault(a => a.FCId == userid);
                fc.Logo = "images/"+newFileName;
                db.SaveChanges();
                TempData["msgrem"] = "Logo uploaded.";
                var result = db.FeeCollector_details.Where(a => a.FCId == userid);
            return View(result);
        }
        [HttpGet]
        public ActionResult InvoicePDF(int invoiceNo)
        {
            //Code to get content
            return new Rotativa.ActionAsPdf("FeeInvoicePDF", new { invc = invoiceNo}) { FileName = "InvoiceAsPdf.pdf" };
        }
        public ActionResult FeeInvoicePDF(int invc)
        {
            var model = new FeeInvoiceModel();
            model.lstReceiptItems = db.proc_getFeedepositReceiptDetails(invc).ToList();
            string RollNo = model.lstReceiptItems.FirstOrDefault().RollNo;
            model.student = db.Students.Single(a => a.RollNo == RollNo);
            string dcid = model.lstReceiptItems.FirstOrDefault().FeeCollectorId;
            model.feecollcector = db.FeeCollector_details.Single(a => a.FCId == dcid);
            return View(model);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}