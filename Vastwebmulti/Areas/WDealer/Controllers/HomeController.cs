using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using Rotativa;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Vastwebmulti.Areas.WDealer.Models;
using Vastwebmulti.Areas.WDealer.ViewModel;
using Vastwebmulti.Areas.WHITELABEL.Models;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.WDealer.Controllers
{
    [Authorize(Roles = "Whitelabeldealer")]
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
        ALLSMSSend smssend = new ALLSMSSend();

        #region Retailer List
        //GET : Retailer List
        [HttpGet]
        public ActionResult Index()
        {
            WhitelabelRetailerModel viewModel = new WhitelabelRetailerModel();
            List<Whitelabel_Dealer_retailer_Result> list = new List<Whitelabel_Dealer_retailer_Result>();
            try
            {
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    var Token = string.Empty;
                    Token = GetToken();

                    var newlycreatedUserId = string.Empty;


                    var client = new RestClient(WapiBaseUrl.GetBaseUrl() + "/api/data/retailer_list");
                    client.Timeout = -1;
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("Authorization", "bearer " + Token + "");
                    IRestResponse response = client.Execute(request);

                    var statusCode = response.StatusCode.ToString();
                    if (statusCode == "Unauthorized")
                    {
                        TempData.Remove("data");
                        AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                        return RedirectToAction("Index", "Home", null);
                    }

                    var responsechk = response.Content.ToString();

                    JArray obj = Newtonsoft.Json.JsonConvert.DeserializeObject<JArray>(responsechk);

                    foreach (var result in obj)
                    {
                        Whitelabel_Dealer_retailer_Result model = new Whitelabel_Dealer_retailer_Result();

                        model.RetailerId = (string)result["UserID"];
                        model.Email = (string)result["Email"];
                        model.RetailerName = (string)result["Name"];
                        model.Frm_Name = (string)result["firmName"];
                        model.Mobile = (string)result["Mobile"];
                        model.Remainamount = (decimal)result["RemainAmt"];
                        model.State_name = (string)result["State"];
                        model.Dist_Desc = (string)result["District"];
                        model.Address = (string)result["Address"];
                        model.PanCard = (string)result["PAN"];
                        model.AadharCard = (string)result["Aadhar"];
                        model.gst = (string)result["GST"];
                        model.Status = (string)result["Status"];
                        model.Emailconfirmed = (bool)result["Emailconfirmed"];
                        model.JoinDate = (DateTime)result["JoinDate"];
                        list.Add(model);
                    }
                    viewModel.dealer_retailer = list;
                    var userid = User.Identity.GetUserId();
                    ViewBag.state = new SelectList(db.Select_State_Details(), "State_Id", "State_Name").ToList();

                    ViewData["ResendMail"] = TempData["ResendMAil"];
                    TempData.Remove("ResendMail");
                    ViewData["msg"] = TempData["msg"];

                    return View(viewModel);
                }
            }
            catch
            {
                viewModel.dealer_retailer = list;
                var userid = User.Identity.GetUserId();
                ViewBag.state = new SelectList(db.Select_State_Details(), "State_Id", "State_Name").ToList();

                ViewData["ResendMail"] = TempData["ResendMAil"];
                TempData.Remove("ResendMail");
                ViewData["msg"] = TempData["msg"];

                return View(viewModel);
            }

            //using (VastwebmultiEntities db = new VastwebmultiEntities())
            //{
            //    var userid = User.Identity.GetUserId();
            //    ViewBag.state = new SelectList(db.Select_State_Details(), "State_Id", "State_Name").ToList();
            //    var Details = db.Whitelabel_Dealer_retailer(userid, "Distibutor","",1,500).ToList();
            //    WhitelabelRetailerModel viewModel = new WhitelabelRetailerModel();
            //    viewModel.dealer_retailer = Details;
            //    viewModel.messagetop = (from gg in db.Message_top where (gg.users == "Master" || gg.users == "All") where gg.status == "Y" select gg).ToList();
            //    ViewBag.state1 = new SelectList(db.Select_State_Details(), "State_Id", "State_Name");
            //    ViewData["ResendMail"] = TempData["ResendMAil"];

            //    //var details = db.Slab_name.Where(aa => aa.SlabFor == "Retailer" && aa.createdby == userid).ToList();
            //    //ViewBag.slab_nm = new SelectList(details, "SlabName", "SlabName").ToList();
            //    ViewData["msg"] = TempData["msg"];
            //    TempData.Remove("msg");
            //    ViewData["error"] = TempData["error"];
            //    TempData.Remove("error");

            //    return View(viewModel);
            //}
        }
        // POST : Insert New Retailer
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Insert_retailer(WhitelabelRetailerModel rem)
        {
            try
            {
                var Token = string.Empty;
                Token = GetToken();

                var newlycreatedUserId = string.Empty;

                var client = new RestClient(WapiBaseUrl.GetBaseUrl() + "/api/data/Retailer_create");
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Authorization", "bearer " + Token + "");
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", "{\r\n  \"ParentID\": \"ParentID\",\r\n  \"UserID\": \"UserID\",\r\n  \"Name\": \"" + rem.Name + "\",\r\n  \"firmName\": \"" + rem.Firm + "\",\r\n  \"Mobile\": \"" + rem.Mobile + "\",\r\n  \"PINCode\": \"" + rem.Pincode + "\",\r\n  \"Email\": \"" + rem.Email + "\",\r\n  \"Address\": \"" + rem.Address + "\",\r\n  \"District\": \"" + rem.District + "\",\r\n  \"State\": \"" + rem.State + "\",\r\n  \"Aadhar\": \"" + rem.Adhaar + "\",\r\n  \"PAN\": \"" + rem.Pan + "\",\r\n  \"GST\": \"" + rem.Gst + "\",\r\n  \"Status\": \"Y\",\r\n  \"MoneySts\": \"N\",\r\n  \"RemainAmt\": " + 0 + ",\r\n  \"JoinDate\": \"" + DateTime.Now + "\",\r\n  \"dob\": \"" + "" + "\",\r\n  \"Password\": \"pass\",\r\n  \"PIN\": \"" + rem.Pincode + "\",\r\n  \"pancardimagePath\": \"pancardimagePath\",\r\n  \"aadharcardimagePath\": \"aadharcardimagePath\",\r\n  \"frimregistrationimagePath\": \"frimregistrationimagePath\",\r\n  \"PhotoImage\": \"PhotoImage\"\r\n}", ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);

                var statusCode = response.StatusCode.ToString();
                if (statusCode == "Unauthorized")
                {
                    TempData.Remove("data");
                    AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                    return RedirectToAction("Index", "Home", null);
                }

                var responsechk = response.Content.ToString();

                dynamic stuff = Newtonsoft.Json.JsonConvert.DeserializeObject(responsechk);
                string ch = stuff.Message;
                TempData["msgrem"] = ch;
                return RedirectToAction("Index");
            }
            catch
            {
                TempData["Error"] = "User Not Created. Please Create After Some Time.";
                return RedirectToAction("Index");
            }

            //using (var transaction = db.Database.BeginTransaction())
            //{
            //    try
            //    {
            //        using (VastwebmultiEntities db = new VastwebmultiEntities())
            //        {
            //            if (rem.State != 0 && rem.District != 0)
            //            {
            //                var chkmobile = db.Users.Where(a => a.PhoneNumber == rem.Mobile).Any();
            //                if (chkmobile == true)
            //                {
            //                    TempData["error"] = "This Mobile Number Already Exists.";
            //                    return RedirectToAction("Index");
            //                }
            //                var userid = User.Identity.GetUserId();
            //                var whitelabelid = db.whitelabel_Dealer_Details.Where(aa => aa.DealerId == userid).Single().Whitelabelid;
            //                var check = db.Whitelabel_Retailer_Details.Where(es => es.Mobile == rem.Mobile).Any();
            //                if (check == false)
            //                {
            //                    var user = new ApplicationUser { UserName = rem.Email, Email = rem.Email, PhoneNumber = rem.Mobile };
            //                    //Generate Random Password
            //                    bool includeLowercase = true;
            //                    bool includeUppercase = true;
            //                    bool includeNumeric = true;
            //                    bool includeSpecial = true;
            //                    bool includeSpaces = false;
            //                    int lengthOfPassword = 8;
            //                    string pass = PasswordGenerator.GeneratePassword(includeLowercase, includeUppercase, includeNumeric, includeSpecial, includeSpaces, lengthOfPassword);
            //                    while (!PasswordGenerator.PasswordIsValid(includeLowercase, includeUppercase, includeNumeric, includeSpecial, includeSpaces, pass))
            //                    {
            //                        pass = PasswordGenerator.GeneratePassword(includeLowercase, includeUppercase, includeNumeric, includeSpecial, includeSpaces, lengthOfPassword);
            //                    }
            //                    var Password = pass;
            //                    int pin = new Random().Next(1000, 10000);
            //                    var enpin = Encrypt(pin.ToString());
            //                    var result = await UserManager.CreateAsync(user, Password);
            //                    if (result.Succeeded)
            //                    {
            //                        System.Data.Entity.Core.Objects.ObjectParameter output = new
            //                         System.Data.Entity.Core.Objects.ObjectParameter("Output", typeof(string));
            //                        var ch = db.whitelabel_Insert_Retailer(userid, user.Id, rem.Name, rem.State, rem.District, rem.Mobile, rem.Address, Convert.ToInt32(rem.Pincode), rem.Email, "", rem.Firm, rem.Adhaar, rem.Pan, rem.Capping, rem.Gst, enpin, "", output).SingleOrDefault().msg;
            //                        transaction.Commit();
            //                        // Send an email with this link
            //                        string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
            //                        var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
            //                        callbackUrl = callbackUrl.Replace("/WDealer", "");
            //                        string body = new CommonUtil().WhitelabelRetailerPopulateBody("", "Confirm your account", "", "" + callbackUrl + "", rem.Email, Password, pin.ToString(), "", whitelabelid);
            //                        var statusSendSmsDealerToRetailerCreate = db.Whitelabel_SMSSendAll.Where(a => a.ServiceName == "RetailerCreateDLM" && a.Whitelabelid == whitelabelid).SingleOrDefault().Status;
            //                        new CommonUtil().Insertsendmail_whitelabel(whitelabelid, rem.Email, "Confirm your account", body);
            //                        if (ch.ToString() == "Register SuccessFully.")
            //                        {
            //                            if (statusSendSmsDealerToRetailerCreate == "Y")
            //                            {
            //                                smssend.sendsmsallwhitelabel(whitelabelid, rem.Mobile, "Dear Partner ! Welcome Your user Id " + rem.Mobile + " and Password " + Password + ". Thanks For Your Business . ", "User Create");
            //                            }
            //                            TempData["msg"] = ch;
            //                            return RedirectToAction("Index");
            //                        }
            //                        else
            //                        {
            //                            transaction.Rollback();
            //                            TempData["error"] = ch;
            //                            return RedirectToAction("Index");
            //                        }
            //                    }
            //                    else
            //                    {
            //                        TempData["error"] = "Your Email id is Allready Exist.";
            //                        return RedirectToAction("Index");
            //                    }
            //                }
            //                else
            //                {
            //                    TempData["error"] = "This Mobile Number Already Exists.";
            //                    return RedirectToAction("Index");
            //                }
            //            }
            //            else
            //            {
            //                return RedirectToAction("Index");
            //            }
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        transaction.Rollback();
            //        TempData["Error"] = "User Not Created. Please Create After Some Time.";
            //        return RedirectToAction("Index");
            //    }
            //}
        }
        public ActionResult RetailerEmailconfrimation(string id)
        {
            var sts = (from pp in db.Users where pp.UserId == id select pp).Single().EmailConfirmed;
            if (sts == false)
            {
                sts = true;
            }
            User obj = (from dd in db.Users
                        where dd.UserId == id
                        select dd).SingleOrDefault();
            obj.EmailConfirmed = sts;
            db.SaveChanges();
            TempData["msg"] = "Email Confirmation Update Successfully.";
            return RedirectToAction("Index");
        }
        //delete retailers
        public JsonResult Delete_Retailer(string retailerid)
        {
            if (retailerid != null && retailerid != "")
            {
                db.delete_whitelabel_retailer(retailerid);
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("Failed!", JsonRequestBehavior.AllowGet);
            }
        }
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
        //GET : Update Retailer Status
        public ActionResult Updateretailer_ststus(string id)
        {

            try
            {
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    var chk = (from ff in db.Whitelabel_Retailer_Details where ff.RetailerId == id select ff).Single().Status.ToString();
                    if (chk == "Y")
                    {
                        chk = "N";
                    }
                    else
                    {
                        chk = "Y";
                    }
                    Whitelabel_Retailer_Details objCourse = (from p in db.Whitelabel_Retailer_Details
                                                             where p.RetailerId == id
                                                             select p).SingleOrDefault();
                    objCourse.Status = chk;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }
        //GET : Update Retailer money Status
        public ActionResult Updateretailer_money_ststus(string id)
        {

            try
            {
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    var chk = (from ff in db.Whitelabel_Retailer_Details where ff.RetailerId == id select ff).Single().moneysts.ToString();
                    if (chk == "Y")
                    {
                        chk = "N";
                    }
                    else
                    {
                        chk = "Y";
                    }
                    Whitelabel_Retailer_Details objCourse = (from p in db.Whitelabel_Retailer_Details
                                                             where p.RetailerId == id
                                                             select p).SingleOrDefault();
                    objCourse.moneysts = chk;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }
        // POST : Retailer Search
        [HttpPost]
        public ActionResult RetailerSearch(string userid)
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {

                var ch = db.Whitelabel_Retailer_Details.Where(aa => aa.RetailerId == userid).ToList();
                db.Whitelabel_Retailer_Details.Where(ii => ii.RetailerId == userid).Select(a => new whitelabelRetailerEditModel
                {
                    DealerId = a.DealerId,
                    RetailerId = a.RetailerId,
                    RetailerName = a.RetailerName,
                    State = a.State,
                    District = a.District,
                    Mobile = a.Mobile,
                    Address = a.Address,
                    Pincode = a.Pincode,
                    Email = a.Email,
                    Frm_Name = a.Frm_Name,
                    AadharCard = a.AadharCard,
                    PanCard = a.PanCard,
                    caption = a.caption,
                    gst = a.gst,
                    PIN = a.PIN,
                    dateofbirth = a.dateofbirth,
                    PartnerID = a.PartnerID,
                    AepsMerchandId = a.AepsMerchandId,
                    AepsMPIN = a.AepsMPIN



                }).FirstOrDefault();
                return Json(JsonConvert.SerializeObject(ch), JsonRequestBehavior.AllowGet);
            }
        }

        //POST : Edit Retailer User
        [HttpPost]
        public ActionResult Edit_Retailer_user(WhitelabelRetailerModel rem)
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                Whitelabel_Retailer_Details objCourse = (from p in db.Whitelabel_Retailer_Details
                                                         where p.RetailerId == rem.Retailerid
                                                         select p).SingleOrDefault();
                objCourse.Address = rem.Address1;
                objCourse.AadharCard = rem.Adhaar1;
                objCourse.RetailerName = rem.Name1;
                objCourse.District = rem.District1;
                objCourse.Frm_Name = rem.Firm1;
                objCourse.gst = rem.Gst1;
                objCourse.PanCard = rem.Pan1;
                objCourse.Pincode = Convert.ToInt32(rem.Pincode1);
                objCourse.State = rem.State1;
                objCourse.caption = rem.Capping1;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
        }
        #endregion

        #region SlabSetting
        //GET : Show Slab Name 
        public ActionResult generateSlab()
        {
            ADMIN.Models.ResultSetViewModel viewModel = new ADMIN.Models.ResultSetViewModel();
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                var userid = User.Identity.GetUserId();
                viewModel.ResultSet = db.Slab_name.Where(aa => aa.createdby == userid).Select(aaa => new ADMIN.Models.Slab_name_model
                {
                    cdate = aaa.cdate,
                    createdby = aaa.createdby,
                    idno = aaa.idno,
                    SlabFor = aaa.SlabFor,
                    SlabName = aaa.SlabName
                }).ToList();

                ViewData["msg"] = TempData["msg"];
                return View(viewModel);
            }
        }
        // Post : Add New Slab
        [HttpPost]
        public ActionResult AddSlabname(ADMIN.Models.ResultSetViewModel result)
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                var userid = User.Identity.GetUserId();
                try
                {
                    System.Data.Entity.Core.Objects.ObjectParameter output = new
                      System.Data.Entity.Core.Objects.ObjectParameter("Output", typeof(string));
                    var msg = db.insert_slab_list("Retailer", result.AccountVM.Slab_Name, userid, output).Single().slab;
                    TempData["msg"] = msg;
                    return RedirectToAction("generateSlab");
                }
                catch
                {
                    return RedirectToAction("generateSlab");
                }
            }
        }

        public ActionResult Delete_slabName(string slabname)
        {
            try
            {
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    var userid = User.Identity.GetUserId();
                    System.Data.Entity.Core.Objects.ObjectParameter output = new System.Data.Entity.Core.Objects.ObjectParameter("Output", typeof(string));
                    var respo = db.proc_deleteSlabByName(userid, slabname, "Retailer", output).SingleOrDefault().msg;
                    if (respo == "SUCCESS")
                    {
                        TempData["msg"] = slabname + " deleted Successfully.";
                    }
                    else if (respo == "ASSIGNED")
                    {
                        TempData["msg"] = "Unable to delete assigned slab.";
                    }
                    return RedirectToAction("generateSlab");
                }
            }
            catch
            {
                return RedirectToAction("generateSlab");
            }
            //if (slabfor == "Retailer")
            //{
            //    var userid = User.Identity.GetUserId();
            //    var disresult = db.Whitelabel_Retailer_Details.Where(p => p.slab_name == slabname).ToList();
            //    if (disresult.Count > 0)
            //    {
            //        TempData["api"] = "This slab is Already Assign To Whitelabel Retailer User..";
            //    }
            //    else
            //    {
            //        var msg = db.Delete_slab(slabfor, slabname, userid);
            //        TempData["successmsg"] = "Slab Deteted Successfully..";
            //    }
            //}

        }



        #endregion

        #region Account


        #region Fundtransfer To Retailer
        public ActionResult FundTransferDealer(string tabname = "")
        {
            WhitelabelDealerfundtransfer vmodel = new WhitelabelDealerfundtransfer();
            string dealerid = User.Identity.GetUserId();

            string whitelabelid = db.whitelabel_Dealer_Details.Where(x => x.DealerId == dealerid).FirstOrDefault().Whitelabelid;

            var stands = (from rem in db.Whitelabel_Retailer_Details where rem.DealerId == dealerid select rem).ToList();
            List<SelectListItem> items = new List<SelectListItem>();
            foreach (var item in stands)
            {
                items.Add(new SelectListItem { Text = item.Frm_Name + " / " + item.Mobile, Value = item.RetailerId.ToString() });
            }
            vmodel.ddlRetailer = items;
            var bindbank = db.Whitelabel_bank_info.Where(x => x.userid == whitelabelid).ToList();
            List<SelectListItem> bankitem = new List<SelectListItem>();
            foreach (var bank in bindbank)
            {
                bankitem.Add(new SelectListItem { Text = bank.banknm + " / " + bank.holdername, Value = bank.acno });
            }
            vmodel.ddlFillAllBank = bankitem;

            var bindwallet = db.Whitelabel_Wallet_info.Where(x => x.userid == whitelabelid).ToList();
            List<SelectListItem> walletitem = new List<SelectListItem>();
            foreach (var wallet in bindwallet)
            {
                walletitem.Add(new SelectListItem { Text = wallet.walletname + " / " + wallet.walletholdername, Value = wallet.walletno });
            }
            vmodel.ddlFillAllwallet = walletitem;
            TempData["tabActive"] = tabname;
            return View(vmodel);
        }

        //public ActionResult Fundtransfer_Wldealer_To_WlRetailer()
        //{
        //    WhitelabelDealerfundtransfer vmodel = new WhitelabelDealerfundtransfer();
        //    string dealerid = User.Identity.GetUserId();
        //    var stands = (from rem in db.Whitelabel_Retailer_Details where rem.DealerId == dealerid select rem).ToList();
        //    IEnumerable<SelectListItem> selectList = from s in stands
        //                                             select new SelectListItem
        //                                             {
        //                                                 Value = s.RetailerId,
        //                                                 Text = s.Frm_Name.ToString()
        //                                             };
        //    ViewBag.RetailerId = new SelectList(selectList, "Value", "Text");

        //    //find whitelabel;

        //    string whitelabelid = db.whitelabel_Dealer_Details.Where(x => x.DealerId == dealerid).FirstOrDefault().Whitelabelid;



        //    //Bind whaitelabel bank

        //    var WLbindbank = db.bank_info.Where(x => x.userid == whitelabelid).ToList();

        //    List<SelectListItem> bankitemlist = new List<SelectListItem>();

        //    foreach (var bank in WLbindbank)
        //    {
        //        bankitemlist.Add(new SelectListItem { Text = bank.banknm, Value = bank.acno });
        //    }
        //    vmodel.ddlFillWLBank = bankitemlist;



        //    //Bind whaitelabel Wallet


        //    var WLbindwallet = db.tblwallet_info.Where(x => x.userid == whitelabelid).ToList();
        //    List<SelectListItem> walletitemlist = new List<SelectListItem>();
        //    foreach (var wallet in WLbindwallet)
        //    {
        //        walletitemlist.Add(new SelectListItem { Text = wallet.walletname, Value = wallet.walletno });
        //    }
        //    vmodel.ddlfillWLWallet = walletitemlist;


        //    //Bind whitelabel dealer bank
        //    var WLDLMbindbank = db.bank_info.Where(x => x.userid == dealerid).ToList();

        //    List<SelectListItem> bankitem = new List<SelectListItem>();

        //    foreach (var bank in WLDLMbindbank)
        //    {
        //        bankitem.Add(new SelectListItem { Text = bank.banknm, Value = bank.acno });
        //    }
        //    vmodel.ddlFillWLDLMBank = bankitem;
        //    //Bind whitelabel dealer Wallet
        //    var WLDLMbindwallet = db.tblwallet_info.Where(x => x.userid == dealerid).ToList();
        //    List<SelectListItem> walletitem = new List<SelectListItem>();
        //    foreach (var wallet in WLDLMbindwallet)
        //    {
        //        walletitem.Add(new SelectListItem { Text = wallet.walletname, Value = wallet.walletno });
        //    }
        //    vmodel.ddlfillWLDLMMWallet = walletitem;

        //    return View(vmodel);
        //}

        #endregion

        #region MDTODealer
        public PartialViewResult MDTODealer(string tabtype = "Reatiler", string txt_frm_date = "", string txt_to_date = "", string ddltype = "")
        {

            WhitelabelDealerfundtransfer vmodel = new WhitelabelDealerfundtransfer();

            string dealerid = User.Identity.GetUserId();


            DateTime fromdate;
            DateTime Todate;

            if (string.IsNullOrEmpty(txt_frm_date) && string.IsNullOrEmpty(txt_to_date))
            {
                fromdate = DateTime.Now.AddDays(-1);
                Todate = DateTime.Now.AddDays(1);
            }
            else
            {

                string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy",
                            "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };

                DateTime dt = !string.IsNullOrWhiteSpace(txt_frm_date) ? DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
                DateTime dt1 = !string.IsNullOrWhiteSpace(txt_to_date) ? DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
                fromdate = Convert.ToDateTime(dt).Date;
                Todate = Convert.ToDateTime(dt1).Date.AddDays(1);


            }

            switch (tabtype)
            {
                case "RetailerToDLMREQ":
                    var result = db.select_whitelabel_rem_pur_order("ALL", dealerid, fromdate, Todate).ToList();
                    if (!string.IsNullOrEmpty(ddltype))
                    {
                        result = result.Where(x => x.remid == ddltype).ToList();
                    }

                    vmodel.PurchaseRequestRecived = result;
                    break;

                case "Admin":


                    vmodel.SendPurchaserequest = db.select_whitelabel_dlm_pur_order(dealerid, "ALL", fromdate, Todate).ToList();


                    break;

                default:
                    vmodel.DealerToRemFundTransfer = db.select_whitelabel_dealer_dlm_rem(dealerid, fromdate, Todate, "ALL").ToList();

                    var result1 = db.select_whitelabel_dealer_dlm_rem(dealerid, fromdate, Todate, "ALL").ToList();
                    if (!string.IsNullOrEmpty(ddltype))
                    {
                        result1 = result1.Where(x => x.RetailerId == ddltype).ToList();
                    }

                    vmodel.DealerToRemFundTransfer = result1;


                    break;

            }



            return PartialView("_FundTransferDealetToRetailerPartial", vmodel);

        }
        //public PartialViewResult MDTODealer(string tabtype, string txt_frm_date, string txt_to_date, string msg = "")
        //{
        //    DateTime fromdate;
        //    DateTime Todate;

        //    if (string.IsNullOrEmpty(txt_frm_date) && string.IsNullOrEmpty(txt_to_date))
        //    {
        //        fromdate = DateTime.Now.AddDays(-1);
        //        Todate = DateTime.Now.AddDays(1);
        //    }
        //    else
        //    {

        //        string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy",
        //                    "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };

        //        DateTime dt = !string.IsNullOrWhiteSpace(txt_frm_date) ? DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
        //        DateTime dt1 = !string.IsNullOrWhiteSpace(txt_to_date) ? DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
        //        fromdate = Convert.ToDateTime(dt).Date;
        //        Todate = Convert.ToDateTime(dt1).Date.AddDays(1);


        //    }

        //    WhitelabelDealerfundtransfer vmodel = new WhitelabelDealerfundtransfer();


        //    string userid = User.Identity.GetUserId();
        //    switch (tabtype)
        //    {
        //        case "Wl_dlm_purchase_order":

        //            if (!string.IsNullOrEmpty(msg))
        //            {
        //                vmodel.message = msg;
        //            }

        //            vmodel.Wl_dlm_purchase_order_send = db.select_whitelabel_dlm_pur_order(userid, "ALL", Convert.ToDateTime(fromdate), Convert.ToDateTime(Todate)).ToList();
        //            //  var result = Select_whitelabel_dealer_to_REM
        //            //vmodel.ShowDataWhitelabelToDLMlist.Add(vmodel.ShowDataWhitelabelToDLM);

        //            // vmodel.ShowDataWhitelabelToREMlist = whitelabeltoremlist;

        //            //  vmodel.funrequesttoadmin = _db.select_master_pur_order(masterid, "ALL", DateTime.Now.AddDays(-1), DateTime.Now.AddDays(1)).ToList();
        //            break;

        //        case "Whitelabel_rem_pur_recive":

        //            vmodel.Whitelabel_rem_pur_recive = db.select_whitelabel_rem_pur_order("ALL", userid, Convert.ToDateTime(fromdate), Convert.ToDateTime(Todate)).ToList();


        //            break;

        //        case "WL_dlm_bal_recived_reports":

        //            vmodel.WL_dlm_bal_recived_reports = db.whitelabel_dlm_bal_recive_report(userid, Convert.ToDateTime(fromdate), Convert.ToDateTime(Todate)).ToList();

        //            break;


        //            vmodel.select_Dealer_credit_report = db.select_Dealer_credit_report_by_Wadmin(userid).ToList();
        //        default:

        //            vmodel.Select_whitelabel_dealer_to_REM = db.select_whitelabel_dealer_dlm_rem(userid, Convert.ToDateTime(fromdate), Convert.ToDateTime(Todate), "ALL").ToList();

        //            // vmodel.ShowDataWhitelabelToDLMlist = whitelabeltodlmlist;
        //            // vmodel.mastertodlmlist = db.select_admin_to_Dealer(masterid, "ALL", DateTime.Now.AddDays(-1), DateTime.Now.AddDays(1)).ToList();
        //            break;

        //    }

        //    return PartialView("_FundTransferWhiteLabelDLMToREMPartial", vmodel);

        //}

        #endregion

        //find transfer WL to Dlm
        public ActionResult ChkSecurityDealerToFundTransfer(string txtcode, string hdSuperstokistID, string hdMDDLM, string hdPaymentMode,
       string hdPaymentAmount, string hdMDDepositeSlipNo, string hdMDTransferType, string hdMDcollection, string hdMDComments,
       string hdMDBank, string hdsupraccno, string hdMDaccountno, string hdMDutrno, string hdMDwallet,
       string hdMDwalletno, string hdMDtransationno, string hdMDsettelment, string hdMDCreditDetail, string hdMDsubject)
        {
            try
            {
                var Token = string.Empty;
                Token = GetToken();

                var newlycreatedUserId = string.Empty;

                var client = new RestClient(WapiBaseUrl.GetBaseUrl() + "/api/data/jlklkj");
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Authorization", "bearer " + Token + "");
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", "{\r\n  \"hdMDDLM\": \"" + hdMDDLM + "\",\r\n  \"hdPaymentMode\": \"" + hdPaymentMode + "\",\r\n  \"hdPaymentAmount\": \"" + hdPaymentAmount + "\",\r\n  \"hdMDDepositeSlipNo\": \"" + hdMDDepositeSlipNo + "\",\r\n  \"hdMDTransferType\": \"" + hdMDTransferType + "\",\r\n  \"hdMDcollection\": \"" + hdMDcollection + "\",\r\n  \"hdMDComments\": \"" + hdPaymentAmount + "\",\r\n  \"hdMDaccountno\": \"" + hdMDcollection + "\",\r\n  \"hdMDutrno\": \"" + hdMDutrno + "\",\r\n  \"hdMDwallet\": \"" + hdMDwallet + "\",\r\n  \"hdMDwalletno\": \"" + hdMDwalletno + "\",\r\n  \"hdMDtransationno\": \"" + hdMDtransationno + "\",\r\n  \"hdMDsettelment\": \"" + hdMDsettelment + "\",\r\n  \"hdMDCreditDetail\": \"" + hdMDCreditDetail + "\",\r\n  \"hdMDsubject\": \"" + hdMDsubject + "\",\r\n  \"hdMDBank\": \"" + hdMDBank + "\",\r\n  \"txtcode\": \"" + txtcode + "\"\r\n}", ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);

                var statusCode = response.StatusCode.ToString();
                if (statusCode == "Unauthorized")
                {
                    TempData.Remove("data");
                    AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                    return RedirectToAction("Index", "Home", null);
                }

                var responsechk = response.Content.ToString();

                dynamic stuff = Newtonsoft.Json.JsonConvert.DeserializeObject(responsechk);
                string ch = stuff.Message;
                TempData["msgrem"] = ch;
                return Json(ch, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("Server error", JsonRequestBehavior.AllowGet);
            }
        }
        //       public ActionResult FundTransferWLDLM_TO_WLREM(string hdSuperstokistID, string hdMDDLM, string hdPaymentMode,
        //string hdPaymentAmount, string hdMDDepositeSlipNo, string hdMDTransferType, string hdMDcollection, string hdMDComments,
        //string hdMDBank, string hdsupraccno, string hdMDaccountno, string hdMDutrno, string hdMDwallet,
        //string hdMDwalletno, string hdMDtransationno, string hdMDsettelment, string hdMDCreditDetail, string hdMDsubject)
        //       {


        //           try
        //           {
        //               decimal balance = Convert.ToDecimal(hdPaymentAmount);
        //               string type = hdPaymentMode;
        //               string comment = hdMDComments;
        //               var DealerId = hdMDDLM;
        //               // string utrno = hdMDcollection == null ? hdMDutrno : hdMDcollection;
        //               string collectionby = hdMDcollection == null ? hdMDtransationno : hdMDcollection;
        //               collectionby = collectionby == null ? hdMDutrno : collectionby;
        //               collectionby = collectionby == null ? hdMDtransationno : collectionby;
        //               collectionby = collectionby == null ? hdMDsettelment : collectionby;
        //               collectionby = collectionby == null ? hdMDCreditDetail : collectionby;
        //               collectionby = collectionby == null ? hdMDsubject : collectionby;
        //               collectionby = collectionby == null ? hdMDDepositeSlipNo : collectionby;
        //               string bankname = hdMDBank == null ? hdMDwallet : hdMDBank;
        //               string adminacco = hdMDaccountno == null ? hdMDwalletno : hdMDaccountno;


        //               string userid = User.Identity.GetUserId();
        //               System.Data.Entity.Core.Objects.ObjectParameter output = new System.Data.Entity.Core.Objects.ObjectParameter("output", typeof(String));
        //               var diff1 = (db.Whitelabel_Dealer_To_Retailer_Balance.Where(aa => aa.RetailerId == hdMDDLM && aa.DealerId == userid).OrderByDescending(aa => aa.id).Select(aa => aa.cr).FirstOrDefault());
        //               diff1 = diff1 ?? 0;
        //               var ch = "";
        //               //var tp = "";
        //               decimal amount = Convert.ToDecimal(balance);
        //               var whitelabelid = db.whitelabel_Dealer_Details.Where(a => a.DealerId == userid).Single().Whitelabelid;
        //               var statusSendSmsDlmToRetailerFundTransfer = db.Whitelabel_SMSSendAll.Where(a => a.ServiceName == "DLMtoRemRemFundTrans" && a.Whitelabelid == whitelabelid).SingleOrDefault().Status;
        //               var RetailerMobile = db.Whitelabel_Retailer_Details.Where(a => a.RetailerId == hdMDDLM).Single().Mobile;
        //               var userdetails = db.Whitelabel_Retailer_Details.Where(a => a.RetailerId == hdMDDLM && a.DealerId == userid);
        //               var email = db.Users.Where(p => p.UserId == userid).Single().Email;
        //               if (userdetails != null)
        //               {
        //                   if (amount > 0)
        //                   {
        //                       // if (hdPaymentMode == "Cash" || ddl_fund_type == "Credit")
        //                       // {
        //                       ch = db.insert_Whitelabel_Dealer_To_Retailer_Balance(userid, hdMDDLM, Convert.ToDecimal(balance), 0, hdPaymentMode, comment, collectionby, bankname, adminacco, "Direct", output).Single().msg;
        //                       // }
        //                       if (ch == "Balance Transfer Successfully.")
        //                       {
        //                           if (hdPaymentMode == "Credit")
        //                           {
        //                               var remainretailer = db.Whitelabel_Remain_reteller_balance.Where(a => a.RetellerId == hdMDDLM).Single().Remainamount;
        //                               if (statusSendSmsDlmToRetailerFundTransfer == "Y")
        //                               {
        //                                   smssend.sendsmsallwhitelabel(whitelabelid, RetailerMobile, "Credit Received by " + email + " Rs." + balance + ".New Balance is " + remainretailer + ".Your O/s Credit is " + diff1 + "", "Fund Transfer");
        //                               }
        //                           }
        //                           else
        //                           {
        //                               var remainretailer = db.Whitelabel_Remain_reteller_balance.Where(a => a.RetellerId == hdMDDLM).Single().Remainamount;
        //                               var dealername = db.whitelabel_Dealer_Details.Where(p => p.DealerId == userid).Single().DealerName;
        //                               if (statusSendSmsDlmToRetailerFundTransfer == "Y")
        //                               {
        //                                   smssend.sendsmsallwhitelabel(whitelabelid, RetailerMobile, "Cash Paid Rs." + balance + " to " + dealername + ". New Balance is " + remainretailer + ". Your O/s Credit is " + diff1 + "", "Fund Transfer");
        //                               }
        //                           }
        //                           //TempData["result"] = ch;
        //                           return Json(ch, JsonRequestBehavior.AllowGet);
        //                       }
        //                       else
        //                       {
        //                           return Json(ch, JsonRequestBehavior.AllowGet);
        //                       }
        //                   }
        //                   else
        //                   {
        //                       //   TempData["result"] = "Amount should be not zero";
        //                       return Json("Amount should be not zero", JsonRequestBehavior.AllowGet);
        //                   }
        //               }

        //               // return RedirectToAction("SendFund");
        //           }
        //           catch (Exception ex)
        //           {
        //               return Json("Server error", JsonRequestBehavior.AllowGet);
        //               // return RedirectToAction("SendFund");
        //           }

        //           return Json("Something Wrong", JsonRequestBehavior.AllowGet);
        //       }


        //Purchase Order Send
        public ActionResult Purchasertequest_WLDLM_To_Whitelabel(string hdSuperstokistID, string hdMDDLM, string hdPaymentMode,
string hdPaymentAmount, string hdMDDepositeSlipNo, string hdMDTransferType, string hdMDcollection, string hdMDComments,
string hdMDBank, string hdsupraccno, string hdMDaccountno, string hdMDutrno, string hdMDwallet,
string hdMDwalletno, string hdMDtransationno, string hdMDsettelment, string hdMDCreditDetail, string hdMDsubject)
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                WebImage photo = null;
                var newFileName = "";
                var imagePath = "";
                try
                {

                    if (Request.HttpMethod == "POST")
                    {
                        photo = WebImage.GetImageFromRequest();
                        if (photo != null)
                        {
                            newFileName = Guid.NewGuid().ToString() + "_" +
                                          Path.GetFileName(photo.FileName);
                            imagePath = @"PurchaseOrderImg\" + newFileName;

                            photo.Save(@"~\" + imagePath);
                        }
                    }

                }
                catch (Exception e)
                {
                    throw;
                }

                decimal balance = Convert.ToDecimal(hdPaymentAmount);
                string type = hdPaymentMode;
                string comment = hdMDComments;
                var DealerId = hdMDDLM;
                // string utrno = hdMDcollection == null ? hdMDutrno : hdMDcollection;
                string collectionby = hdMDcollection == null ? hdMDtransationno : hdMDcollection;
                collectionby = collectionby == null ? hdMDutrno : collectionby;
                collectionby = collectionby == null ? hdMDtransationno : collectionby;
                collectionby = collectionby == null ? hdMDsettelment : collectionby;
                collectionby = collectionby == null ? hdMDCreditDetail : collectionby;
                collectionby = collectionby == null ? hdMDsubject : collectionby;
                collectionby = collectionby == null ? hdMDDepositeSlipNo : collectionby;
                string bankname = hdMDBank == null ? hdMDwallet : hdMDBank;
                string adminacco = hdMDaccountno == null ? hdMDwalletno : hdMDaccountno;

                string userid = User.Identity.GetUserId();

                if (db.Whitelabel_dlm_purchage.Count(aa => aa.dlmid.ToLower() == userid.ToLower() && aa.sts == "Pending") < 1)
                {
                    var amount = Convert.ToDecimal(balance);
                    if (amount > 0)
                    {
                        //  if (Paymode == "3rdParty" || Paymode == "IMPS_RTGS_NEFT" || Paymode == "CASH")
                        // {

                        db.insert_whitelabel_dlm_purchageorder(userid, hdPaymentMode, collectionby, bankname, "", hdMDComments, Convert.ToDecimal(balance), "", type, adminacco, "", "");
                        // TempData["success"] = "Purchase Order Successfully";
                        // }

                        //  if (type == "Credit")
                        //  {
                        //    db.insert_whitelabel_dlm_purchageorder(userid, Paymode, utrno, "", "", Comment, Convert.ToDecimal(balance), "", type, accountno, "", @"/PurchaseOrderImg/" + newFileName);
                        //  TempData["success"] = "Credit Pay Successfully";
                        return Json("Purchase Order Successfully", JsonRequestBehavior.AllowGet);
                        //}

                    }
                    else
                    {
                        //   TempData["show"] = "Amount should be not zero";
                        return Json("Amount should be not zero", JsonRequestBehavior.AllowGet);

                    }

                }
                else
                {
                    //  TempData["show"] = "Your purcharge Order Allready Pending.";
                    return Json("Your purcharge Order Allready Pending", JsonRequestBehavior.AllowGet);
                }

                //return RedirectToAction("PurchaseOrder");
            }



        }






        //Fund Transfer To Retailer

        public ActionResult SendFund()
        {

            string dealerid = User.Identity.GetUserId();
            var msg = TempData["result"];
            ViewData["output"] = msg;
            string txt_frm_date = DateTime.Now.ToString();
            string txt_to_date = DateTime.Now.ToString();
            string frm_date = Convert.ToDateTime(txt_frm_date).Date.ToString("yyyy-MM-dd");
            string to_date = Convert.ToDateTime(txt_to_date).AddDays(1).ToString("yyyy-MM-dd");


            var stands = (from rem in db.Whitelabel_Retailer_Details where rem.DealerId == dealerid select rem).ToList();
            IEnumerable<SelectListItem> selectList = from s in stands
                                                     select new SelectListItem
                                                     {
                                                         Value = s.RetailerId,
                                                         Text = s.Email + "--" + s.RetailerName.ToString()
                                                     };
            ViewBag.RetailerId = new SelectList(selectList, "Value", "Text");
            ViewBag.RetailerId1 = new SelectList(selectList, "Value", "Text");
            var ch = db.select_whitelabel_dealer_dlm_rem(dealerid, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), "ALL").ToList();
            return View(ch);

        }
        [HttpPost]
        public ActionResult SendFund(string txt_frm_date, string txt_to_date, string RetailerId1)
        {

            var dealerid = User.Identity.GetUserId();
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
            if (RetailerId1 == "")
            {
                RetailerId1 = "ALL";
            }
            var stands = (from dlm in db.Whitelabel_Retailer_Details where dlm.DealerId == dealerid select dlm).ToList();
            IEnumerable<SelectListItem> selectList = from s in stands
                                                     select new SelectListItem
                                                     {
                                                         Value = s.RetailerId,
                                                         Text = s.Email + "--" + s.RetailerName.ToString()
                                                     };
            ViewBag.RetailerId = new SelectList(selectList, "Value", "Text");
            ViewBag.RetailerId1 = new SelectList(selectList, "Value", "Text");
            var ch = db.select_whitelabel_dealer_dlm_rem(dealerid, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), RetailerId1).ToList();
            return View(ch);
        }
        [HttpPost]
        public ActionResult Dealer_retailer_bal(string RetailerId, string balance, string ddl_fund_type, string comment)
        {
            try
            {
                string userid = User.Identity.GetUserId();
                System.Data.Entity.Core.Objects.ObjectParameter output = new System.Data.Entity.Core.Objects.ObjectParameter("output", typeof(String));
                var diff1 = (db.Whitelabel_Dealer_To_Retailer_Balance.Where(aa => aa.RetailerId == RetailerId && aa.DealerId == userid).OrderByDescending(aa => aa.id).Select(aa => aa.cr).FirstOrDefault());
                diff1 = diff1 ?? 0;
                var ch = "";
                //var tp = "";
                decimal amount = Convert.ToDecimal(balance);
                var whitelabelid = db.whitelabel_Dealer_Details.Where(a => a.DealerId == userid).Single().Whitelabelid;
                var statusSendSmsDlmToRetailerFundTransfer = db.Whitelabel_SMSSendAll.Where(a => a.ServiceName == "DLMtoRemRemFundTrans" && a.Whitelabelid == whitelabelid).SingleOrDefault().Status;
                var RetailerMobile = db.Whitelabel_Retailer_Details.Where(a => a.RetailerId == RetailerId).Single().Mobile;
                var email = db.Users.Where(p => p.UserId == userid).Single().Email;
                if (amount > 0)
                {
                    if (ddl_fund_type == "Cash" || ddl_fund_type == "Credit")
                    {
                        ch = db.insert_Whitelabel_Dealer_To_Retailer_Balance(userid, RetailerId, Convert.ToDecimal(balance), 0, ddl_fund_type, comment, "", "", "", "", "", output).Single().msg;
                    }
                    if (ch == "Balance Transfer Successfully.")
                    {
                        if (ddl_fund_type == "Credit")
                        {
                            var remainretailer = db.Whitelabel_Remain_reteller_balance.Where(a => a.RetellerId == RetailerId).Single().Remainamount;
                            if (statusSendSmsDlmToRetailerFundTransfer == "Y")
                            {
                                //smssend.sendsmsallwhitelabel(whitelabelid, RetailerMobile, "Credit Received by " + email + " Rs." + balance + ".New Balance is " + remainretailer + ".Your O/s Credit is " + diff1 + "", "Fund Transfer");
                                smssend.sms_init_whitelabel(whitelabelid, "Y", "N", "SEND_SMS_CREDIT_RECEIVEDBY", RetailerMobile, email, balance, remainretailer, diff1);
                            }
                        }
                        else
                        {
                            var remainretailer = db.Whitelabel_Remain_reteller_balance.Where(a => a.RetellerId == RetailerId).Single().Remainamount;
                            var dealername = db.whitelabel_Dealer_Details.Where(p => p.DealerId == userid).Single().DealerName;
                            if (statusSendSmsDlmToRetailerFundTransfer == "Y")
                            {
                                smssend.sms_init_whitelabel(whitelabelid, "Y", "N", "FUNDTRANSFERMESSAGE_CASH", RetailerMobile, balance, dealername, remainretailer, diff1);
                            }
                        }
                        TempData["result"] = ch;
                    }
                }
                else
                {
                    TempData["result"] = "Amount should be not zero";

                }

                return RedirectToAction("SendFund");
            }
            catch
            {
                return RedirectToAction("SendFund");
            }
        }
        //Credit Balance Check
        public ActionResult R_Creditchk(string MID)
        {
            string userid = User.Identity.GetUserId();

            var ch = db.Whitelabel_Dealer_To_Retailer_Balance.Where(aa => aa.RetailerId == MID && aa.DealerId == userid).OrderByDescending(aa => aa.id).Select(c => c.cr).FirstOrDefault();
            ch = ch ?? 0;
            return Json(ch, JsonRequestBehavior.AllowGet);
        }

        //Fund Receive Transfer
        public ActionResult ReceiveFund()
        {
            try
            {
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    string userid = User.Identity.GetUserId();
                    string txt_frm_date = DateTime.Now.ToString();
                    string txt_to_date = DateTime.Now.ToString();
                    string frm_date = Convert.ToDateTime(txt_frm_date).Date.ToString("yyyy-MM-dd");
                    string to_date = Convert.ToDateTime(txt_to_date).AddDays(1).ToString("yyyy-MM-dd");
                    var ch = db.whitelabel_dlm_bal_recive_report(userid, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
                    return View(ch);
                }
            }
            catch
            {

                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public ActionResult ReceiveFund(string txt_frm_date, string txt_to_date)
        {
            try
            {
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    string userid = User.Identity.GetUserId();
                    //Calander Code
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
                    var ch = db.whitelabel_dlm_bal_recive_report(userid, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
                    return View(ch);
                }
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }
        public ActionResult GotoPDF(string From, string Value, string DistOldBal, string DistNewBal, string Date)
        {
            string userid = User.Identity.GetUserId();
            return new Rotativa.ActionAsPdf("InvoicePDF", new { dlmloginid = userid, From = From, Value = Value, DistOldBal = DistOldBal, DistNewBal = DistNewBal, Date = Date });
        }
        public ActionResult InvoicePDF(string dlmloginid, string From, string Value, string DistOldBal, string DistNewBal, string Date)
        {
            var userdetaild = db.whitelabel_Dealer_Details.Where(a => a.DealerId == dlmloginid).SingleOrDefault();
            var PDF_Content = new DealerInvoiceModel()
            {
                From = From,
                Value = Value,
                DistOldBal = DistOldBal,
                DistNewBal = DistNewBal,
                Date = Date
            };
            TempData["retailername"] = userdetaild.DealerId.ToUpper();
            TempData["firmname"] = userdetaild.FarmName.ToUpper();
            TempData["retailermobile"] = userdetaild.Mobile.ToUpper();

            TempData["retailerdate"] = PDF_Content.Date;
            TempData["date"] = DateTime.Now;
            return View(PDF_Content);
            //return View();
        }


        //[HttpPost]
        //public ActionResult ReceiveFund_GST(int idno)
        //{
        //    try
        //    {
        //        using (VastwebmultiEntities db = new VastwebmultiEntities())
        //        {
        //            //TODO
        //        }
        //        return new ViewAsPdf("GST_PDF");
        //    }
        //    catch (Exception ex)
        //    {
        //        return RedirectToAction("Index");
        //    }
        //}
        public ActionResult PurchaseOrder()
        {
            try
            {
                IEnumerable<select_whitelabel_dlm_pur_order_Result> model = null;

                ViewData["show"] = TempData["show"];
                ViewData["success"] = TempData["success"];
                string userid = User.Identity.GetUserId();
                string txt_frm_date = DateTime.Now.ToString();
                string txt_to_date = DateTime.Now.ToString();
                string frm_date = Convert.ToDateTime(txt_frm_date).ToShortDateString();
                string to_date = Convert.ToDateTime(txt_to_date).AddDays(1).ToString();
                //account no
                // whitelable id
                var whitelabelid = db.whitelabel_Dealer_Details.Where(a => a.DealerId == userid).Single().Whitelabelid;
                var accunt = (from acc in db.Whitelabel_bank_info where acc.userid == whitelabelid select new { acno = acc.acno, acnooo = acc.acno + "||" + acc.holdername });
                ViewBag.account = new SelectList(accunt, "acno", "acnooo", null);
                var diff1 = (db.Whitelabel_to_Dealer.Where(aa => aa.dealerid == userid && aa.WhitelabelId == whitelabelid).OrderByDescending(aa => aa.idno).Select(c => c.cr).FirstOrDefault());
                diff1 = diff1 ?? 0;
                ViewData["oldcredit"] = diff1;
                model = db.select_whitelabel_dlm_pur_order(userid, "ALL", Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
                return View(model);

            }
            catch
            {
                return RedirectToAction("Index");

            }
        }

        [HttpPost]
        public ActionResult PurchaseOrder(string txt_frm_date, string txt_to_date)
        {
            try
            {
                IEnumerable<select_whitelabel_dlm_pur_order_Result> model = null;
                string userid = User.Identity.GetUserId();
                //Calander Code
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
                // whitelable id
                var whitelabelid = db.whitelabel_Dealer_Details.Where(a => a.DealerId == userid).Single().Whitelabelid;
                var accunt = (from acc in db.Whitelabel_bank_info where acc.userid == whitelabelid select new { acno = acc.acno, acnooo = acc.acno + "||" + acc.holdername });
                ViewBag.account = new SelectList(accunt, "acno", "acnooo", null);
                var diff1 = (db.Whitelabel_to_Dealer.Where(aa => aa.dealerid == userid && aa.WhitelabelId == whitelabelid).OrderByDescending(aa => aa.idno).Select(c => c.cr).FirstOrDefault());
                diff1 = diff1 ?? 0;
                ViewData["oldcredit"] = diff1;
                model = db.select_whitelabel_dlm_pur_order(userid, "ALL", Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
                //model = db.select_whitelabel_dlm_pur_order(userid, "ALL", Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
                return View(model);
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public ActionResult purchageorder(string Paymode, string type, string utrno, string Comment, decimal balance, string accountno)
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                WebImage photo = null;
                var newFileName = "";
                var imagePath = "";
                try
                {

                    if (Request.HttpMethod == "POST")
                    {
                        photo = WebImage.GetImageFromRequest();
                        if (photo != null)
                        {
                            newFileName = Guid.NewGuid().ToString() + "_" +
                                          Path.GetFileName(photo.FileName);
                            imagePath = @"PurchaseOrderImg\" + newFileName;

                            photo.Save(@"~\" + imagePath);
                        }
                    }

                }
                catch (Exception e)
                {
                    throw;
                }
                string userid = User.Identity.GetUserId();

                if (db.Whitelabel_dlm_purchage.Count(aa => aa.dlmid.ToLower() == userid.ToLower() && aa.sts == "Pending") < 1)
                {
                    var amount = Convert.ToDecimal(balance);
                    if (amount > 0)
                    {
                        if (Paymode == "3rdParty" || Paymode == "IMPS_RTGS_NEFT" || Paymode == "CASH")
                        {

                            db.insert_whitelabel_dlm_purchageorder(userid, Paymode, utrno, "", "", Comment, Convert.ToDecimal(balance), "", type, accountno, "", @"/PurchaseOrderImg/" + newFileName);
                            TempData["success"] = "Purchase Order Successfully";
                        }

                        if (type == "Credit")
                        {
                            db.insert_whitelabel_dlm_purchageorder(userid, Paymode, utrno, "", "", Comment, Convert.ToDecimal(balance), "", type, accountno, "", @"/PurchaseOrderImg/" + newFileName);
                            TempData["success"] = "Credit Pay Successfully";
                        }

                    }
                    else
                    {
                        TempData["show"] = "Amount should be not zero";
                        return RedirectToAction("PurchaseOrder");
                    }

                }
                else
                {
                    TempData["show"] = "Your purcharge Order Allready Pending.";
                }
                return RedirectToAction("PurchaseOrder");
            }
        }

        //Check Old Credit pay
        public ActionResult D_Creditchk(string MID)
        {
            string userid = User.Identity.GetUserId();
            var from = "";
            if (MID == "Master")
            {
                from = (db.whitelabel_Dealer_Details.Where(aa => aa.DealerId == userid).Single().Whitelabelid);
            }
            else
            {
                from = "Admin";
            }

            var diff1 = (db.admin_to_dealer.Where(aa => aa.dealerid == userid && aa.balance_from == from).OrderByDescending(aa => aa.idno).Select(c => c.cr).FirstOrDefault());
            diff1 = diff1 ?? 0;
            return Json(diff1, JsonRequestBehavior.AllowGet);

        }
        //purchase request order
        public ActionResult purcharge_request()
        {
            ViewData["successorder"] = TempData["successorder"];
            ViewData["failedorder"] = TempData["failedorder"];
            string userid = User.Identity.GetUserId();
            var Email = db.Users.FirstOrDefault(a => a.UserId == userid).Email;
            string txt_frm_date = DateTime.Now.Date.ToShortDateString();
            string txt_to_date = DateTime.Now.ToString();
            string frm_date = Convert.ToDateTime(txt_frm_date).ToShortDateString();
            string to_date = Convert.ToDateTime(txt_to_date).AddDays(1).ToString();
            var ch = db.select_whitelabel_rem_pur_order("ALL", Email, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            return View(ch);
        }
        [HttpPost]
        public ActionResult purcharge_request(string txt_frm_date, string txt_to_date)
        {
            string userid = User.Identity.GetUserId();
            var Email = db.Users.FirstOrDefault(a => a.UserId == userid).Email;
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
            var ch = db.select_whitelabel_rem_pur_order("ALL", Email, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            return View(ch);
        }
        [HttpPost]
        public ActionResult updatepurchage_rem(int id, string type, string txtcomment)
        {
            if (type == "APP")
            {
                type = "Approved";
            }
            else
            {
                type = "rejected";
            }
            var sts = db.whitelabel_rem_purchage.Where(a => a.idno == id).Single().sts.ToUpper();
            if (sts == "PENDING")
            {
                System.Data.Entity.Core.Objects.ObjectParameter output = new System.Data.Entity.Core.Objects.ObjectParameter("Output", typeof(string));
                var ch = db.update_whitelabel_rem_purchage(Convert.ToInt32(id), type, 0, txtcomment, output).Single().msg;
                if (ch == "Credit Pay Successfully." || ch == "Balance Transfer Successfully.")
                {
                    TempData["successorder"] = ch;
                }
                else
                {
                    TempData["failedorder"] = ch;
                }
            }
            return RedirectToAction("purcharge_request");
        }
        //Show Credit Report
        public ActionResult show_credit_Report()
        {
            var userid = User.Identity.GetUserId();
            var show = db.select_Dealer_credit_report_by_Wadmin(userid).ToList();
            return View(show);
        }
        public ActionResult Show_Retailer_credit()
        {
            var userid = User.Identity.GetUserId();
            var show = db.Show_Retailer_credit_report_by_WDealer(userid).ToList();
            return View(show);

        }
        #endregion

        #region DASHBORAD
        public ActionResult Dashboard()
        {
            var userid = User.Identity.GetUserId();
            var vv = db.whitelabel_Dealer_Details.SingleOrDefault(a => a.DealerId == userid);
            ViewBag.email = vv.Email;
            ViewBag.image = vv.Photo;
            //Admin Contact Details
            var whitelabelid = db.whitelabel_Dealer_Details.Where(a => a.DealerId == userid).SingleOrDefault().Whitelabelid;
            ViewBag.mobile1 = db.WhiteLabel_userList.Where(a => a.WhiteLabelID == whitelabelid).SingleOrDefault().Mobile;
            ViewBag.email1 = db.WhiteLabel_userList.Where(a => a.WhiteLabelID == whitelabelid).SingleOrDefault().EmailId;

            var stackedchart = db.show_all_active_inactive_rem_list_in_WhitelabelDealer(userid).ToList();
            int actRtl = stackedchart.Where(a => a.type == "ACTIVE").Count();
            ViewData["actRtl"] = actRtl;
            var inRtl = stackedchart.Where(a => a.type == "INACTIVE").Count();
            ViewData["inRtl"] = inRtl;
            int total = actRtl + inRtl;
            ViewData["TotalRetailrs"] = total;
            ViewBag.showholiday = db.whitelabel_show_upcomming_holiday(userid, "WDealer").ToList();


            //show News for Dealer
            var news = (from pp in db.Message_top where (pp.users == "Distributor" || pp.users == "All") where pp.status == "Y" && pp.UserId == whitelabelid select pp).ToList();
            if (news.Any())
            {
                ViewBag.news = news.FirstOrDefault().message;
                ViewBag.newimg = news.FirstOrDefault().image;
            }
            else
            {
                ViewBag.news = null;
                ViewBag.newimg = null;
            }

            return View();
        }
        //show today and yesterday business

        #region Show all today and yesterday recharge
        public ActionResult Show_All_Recharge(string yesterday)
        {
            var type = "";
            DateTime? from = null;
            DateTime? to = null;

            if (yesterday == "Today")
            {
                type = "ALL";
            }
            else
            {
                type = "";
                from = DateTime.Now.Date.AddDays(-1);
                to = DateTime.Now.Date;
            }
            var userid = User.Identity.GetUserId();
            var result = db.whitelabel_Retailer_Today_Recharges(userid, type, from, to).ToList();
            //prepaid

            var prepaid = result.Where(a => a.operator_type == "PrePaid").SingleOrDefault() != null ? result.Where(a => a.operator_type == "PrePaid").SingleOrDefault().total : 0;
            //postpaid
            var postpaid = result.Where(a => a.operator_type == "PostPaid").SingleOrDefault() != null ? result.Where(a => a.operator_type == "PostPaid").SingleOrDefault().total : 0;
            //dth
            var dth = result.Where(a => a.operator_type == "DTH").SingleOrDefault() != null ? result.Where(a => a.operator_type == "DTH").SingleOrDefault().total : 0;
            //landline
            var landline = result.Where(a => a.operator_type == "Landline").SingleOrDefault() != null ? result.Where(a => a.operator_type == "Landline").SingleOrDefault().total : 0;
            //electricity
            var Electricity = result.Where(a => a.operator_type == "Electricity").SingleOrDefault() != null ? result.Where(a => a.operator_type == "Electricity").SingleOrDefault().total : 0;
            //gas
            var Gas = result.Where(a => a.operator_type == "Gas").SingleOrDefault() != null ? result.Where(a => a.operator_type == "Gas").SingleOrDefault().total : 0;
            //insurance
            var Insurance = result.Where(a => a.operator_type == "Insurance").SingleOrDefault() != null ? result.Where(a => a.operator_type == "Insurance").SingleOrDefault().total : 0;
            // DTH-Booking
            var dthbooking = result.Where(a => a.operator_type == "DTH-Booking").SingleOrDefault() != null ? result.Where(a => a.operator_type == "DTH-Booking").SingleOrDefault().total : 0;
            // DTH-Booking
            var water = result.Where(a => a.operator_type == "Water").SingleOrDefault() != null ? result.Where(a => a.operator_type == "Water").SingleOrDefault().total : 0;
            // total recharge and bill 
            var rechargeandbill = (prepaid + postpaid + dth + landline + Electricity + Gas + Insurance + dthbooking + water);

            var moneytransfer = result.Where(a => a.operator_type == "MoneyTransfer").SingleOrDefault() != null ? result.Where(a => a.operator_type == "MoneyTransfer").SingleOrDefault().total : 0;

            var Aeps = result.Where(a => a.operator_type == "Aeps").SingleOrDefault() != null ? result.Where(a => a.operator_type == "Aeps").SingleOrDefault().total : 0;

            var Pancard = result.Where(a => a.operator_type == "Pancard").SingleOrDefault() != null ? result.Where(a => a.operator_type == "Pancard").SingleOrDefault().total : 0;

            return Json(new { Status = type, Recharge = rechargeandbill, Moneytransfer = moneytransfer, Aeps = Aeps, Pancard = Pancard });

        }
        #endregion

        [HttpGet]
        public new ActionResult Profile()
        {
            string userid = User.Identity.GetUserId();
            var userDetails = db.Users.SingleOrDefault(a => a.UserId == userid);
            var MD = db.whitelabel_Dealer_Details.FirstOrDefault(m => m.DealerId == userid);
            ViewBag.MD_Details = MD;
            var gt = db.State_Desc.SingleOrDefault(a => a.State_id == MD.State)?.State_name;
            ViewBag.ddlstate = gt;
            var cities = db.District_Desc.SingleOrDefault(c => c.Dist_id == MD.District && c.State_id == MD.State)?.Dist_Desc;
            ViewBag.district = cities;

            //Shwo Dealer Details
            var mdid = db.whitelabel_Dealer_Details.Where(a => a.DealerId == userid).SingleOrDefault().Whitelabelid;
            var Mddetails = db.WhiteLabel_userList.FirstOrDefault(a => a.WhiteLabelID == mdid);
            ViewBag.showmddetails = Mddetails;


            return View(userDetails);
        }

        //Edit Profile 
        public ActionResult Edit_Profile(string Retailerid)
        {
            var show = db.whitelabel_Dealer_Details.Find(Retailerid);
            ViewBag.Sate = db.State_Desc.Select(a => new SelectListItem { Text = a.State_name, Value = a.State_id.ToString() }).ToList();
            ViewBag.District = db.District_Desc.Where(a => a.State_id == show.State).Select(a => new SelectListItem { Text = a.Dist_Desc, Value = a.Dist_id.ToString() }).ToList();
            return View(show);
        }
        [HttpPost]
        public ActionResult Edit_Profile(string DealerId, whitelabel_Dealer_Details dealerdetails)
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
                var retailer = db.whitelabel_Dealer_Details.Single(a => a.DealerId == DealerId);
                retailer.DealerName = string.IsNullOrWhiteSpace(dealerdetails.DealerName) ? "" : dealerdetails.DealerName;
                retailer.FarmName = string.IsNullOrWhiteSpace(dealerdetails.FarmName) ? "" : dealerdetails.FarmName;
                retailer.Address = string.IsNullOrWhiteSpace(dealerdetails.Address) ? "" : dealerdetails.Address;
                retailer.Pincode = (int)dealerdetails.Pincode;
                retailer.State = dealerdetails.State;
                retailer.District = dealerdetails.District;
                retailer.adharcard = string.IsNullOrWhiteSpace(dealerdetails.adharcard) ? "" : dealerdetails.adharcard;
                retailer.pancard = string.IsNullOrWhiteSpace(dealerdetails.pancard) ? "" : dealerdetails.pancard;
                retailer.gst = string.IsNullOrWhiteSpace(dealerdetails.gst) ? "" : dealerdetails.gst;
                //retailer.caption = masterdetails.caption ?? 0;
                retailer.Photo = string.IsNullOrWhiteSpace(imagePath) ? retailer.Photo : imagePath;
                db.SaveChanges();
                ViewBag.Sate = db.State_Desc.Select(a => new SelectListItem { Text = a.State_name, Value = a.State_id.ToString() }).ToList();
                ViewBag.District = db.District_Desc.Select(a => new SelectListItem { Text = a.Dist_Desc, Value = a.Dist_id.ToString() }).ToList();
                return RedirectToAction("Profile");
            }
            catch
            {

            }

            return RedirectToAction("Profile");
        }

        public JsonResult FillDistict(int State)
        {
            var cities = db.District_Desc.Where(c => c.State_id == State);
            return Json(cities, JsonRequestBehavior.AllowGet);
        }
        //Change Password 
        [HttpGet]
        public ActionResult ChangePassword()
        {

            return View();
        }
        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword([Bind(Prefix = "Item1")] ChangePasswordViewModel model)
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

        public ActionResult Reset_IMPSPin(string txtemail)
        {
            var chk = db.whitelabel_Dealer_Details.Any(a => a.Email == txtemail);

            if (chk == true)
            {
                int pin = new Random().Next(1000, 10000);
                var msg = Encrypt(pin.ToString());
                var userid = User.Identity.GetUserId();
                var id = (from tbl in db.whitelabel_Dealer_Details
                          where tbl.DealerId == userid
                          select tbl).SingleOrDefault();
                id.TransPIN = msg;
                db.SaveChanges();

                var emailid = db.whitelabel_Dealer_Details.Single(p => p.DealerId == userid).Email;
                var whitelabelid = db.whitelabel_Dealer_Details.Where(a => a.DealerId == userid).FirstOrDefault().Whitelabelid;

                var ToCC = db.WhiteLabel_userList.Where(s => s.WhiteLabelID == whitelabelid).FirstOrDefault().EmailId;

                CommUtilEmail emailsend = new CommUtilEmail();
                emailsend.WhiteLabelEmailLimitChk(emailid, ToCC, "IMPS Pin", "Your IMPS Transaction Pin Is  " + pin, "", whitelabelid);

                TempData["success"] = "Your IMPS Pin Send Successfully in Your Mail Id . Please Check Your Mail Id.";
            }
            else
            {
                TempData["error"] = "Your Email Not Verify.Please Enter Your Corrent Registered Email";
            }

            return RedirectToAction("ChangePassword");
        }

        [HttpPost]
        public ActionResult Edit_IMPSPin([Bind(Prefix = "Item2")] ChangePinViewModel model)
        {
            var userid = User.Identity.GetUserId();
            var oldpin = db.whitelabel_Dealer_Details.Single(p => p.DealerId == userid).TransPIN;
            var oldp = Decrypt(oldpin);

            if (oldp != model.OldPin)
            {
                TempData["error"] = "Your Old Pin does Not Match .Please Re-Enter Your Old Pin ";
                return RedirectToAction("ChangePassword");
            }
            var msg = Encrypt(model.NewPin);

            var id = (from tbl in db.whitelabel_Dealer_Details
                      where tbl.DealerId == userid
                      select tbl).SingleOrDefault();
            id.TransPIN = msg;
            db.SaveChanges();
            var emailid = db.whitelabel_Dealer_Details.Single(p => p.DealerId == userid).Email;
            var whitelabelid = db.whitelabel_Dealer_Details.Where(a => a.DealerId == userid).FirstOrDefault().Whitelabelid;

            var ToCC = db.WhiteLabel_userList.Where(s => s.WhiteLabelID == whitelabelid).FirstOrDefault().EmailId;
            CommUtilEmail emailsend = new CommUtilEmail();
            emailsend.WhiteLabelEmailLimitChk(emailid, ToCC, "IMPS Pin", "Your New IMPS Transaction Pin Is  " + model.NewPin, "", whitelabelid);

            TempData["success"] = "Your IMPS Transaction Pin Change and Send Successfully in Your Mail Id . Please Check Your Mail Id.";
            return RedirectToAction("ChangePassword");
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        #endregion

        #region Login
        public ActionResult WebLogin()
        {
            var userid = User.Identity.GetUserId();
            var Email = db.Users.Where(a => a.UserId == userid).Single().Email;
            var currentdate = DateTime.Now.Date;
            var Login_details = db.Login_info.Where(a => a.UserId == Email && a.CurrentLoginTime > currentdate).OrderByDescending(a => a.Idno).ToList();
            return View(Login_details);
        }
        [HttpPost]
        public ActionResult WebLogin(string txt_frm_date, string txt_to_date)
        {
            ViewBag.chk = "post";
            var userid = User.Identity.GetUserId();
            var Email = db.Users.Where(a => a.UserId == userid).Single().Email;

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

            var Login_details = (db.Login_info.Where(aa => aa.CurrentLoginTime >= frm && aa.CurrentLoginTime <= to &&
            aa.UserId == Email && aa.LoginFrom == "Web").OrderByDescending(aa => aa.Idno)).ToList();
            return View(Login_details);

        }


        [HttpGet]
        public ActionResult WebLoginFailed()
        {
            var userid = User.Identity.GetUserId();
            var Email = db.Users.Where(a => a.UserId == userid).Single().Email;
            var currentdate = DateTime.Now.Date;
            var Faild_Login_details = db.Failed_Login_info.Where(a => a.EmailId == Email && a.LoginTime > currentdate).OrderByDescending(a => a.Idno).ToList();
            return View(Faild_Login_details);
        }
        [HttpPost]
        public ActionResult WebLoginFailed(string txt_frm_date, string txt_to_date)
        {
            ViewBag.chk = "post";
            var userid = User.Identity.GetUserId();
            var Email = db.Users.Where(a => a.UserId == userid).Single().Email;

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

            var Faild_Login_details = (db.Failed_Login_info.Where(aa => aa.LoginTime >= frm && aa.LoginTime <= to &&
           aa.EmailId == Email && aa.LoginFrom == "Web").OrderByDescending(aa => aa.Idno)).ToList();
            return View(Faild_Login_details);

        }
        #endregion

        #region Bank
        public ActionResult Bank_Details()
        {
            var userid = User.Identity.GetUserId();
            var whitlelabelid = db.whitelabel_Dealer_Details.Where(a => a.DealerId == userid).Single().Whitelabelid;
            var ch = (from hh in db.Whitelabel_bank_info where hh.userid == whitlelabelid select hh).ToList();
            return View(ch);
        }
        #endregion

        #region Operator


        public ActionResult All_Operator_Info()
        {
            var userid = User.Identity.GetUserId();
            Operater_Commission sb = new Operater_Commission();
            sb.Prepaid = (from cust in db.prepaid_whitelabel_dealer_comm
                          join ord in db.Operator_Code
                          on cust.optcode equals ord.Operator_id.ToString()
                          where (cust.userid == userid)
                          select new Prepaid_Comm
                          {
                              OperatorCode = ord.new_opt_code,
                              Commission = cust.comm,
                              BlockTime = ord.blocktime,
                              Status = ord.status,
                              OperatorType = ord.Operator_type,
                              OperatorName = ord.operator_Name
                          }).ToList();
            sb.Electricity_comm = (from cust in db.utility_whitelabel_dealer_comm
                                   join ord in db.Operator_Code
                                   on cust.optcode equals ord.Operator_id.ToString()
                                   where (cust.userid == userid)
                                   select new Electricity
                                   {
                                       OperatorCode = ord.new_opt_code,
                                       Commission = cust.comm,
                                       BlockTime = ord.blocktime,
                                       Status = ord.status,
                                       OperatorType = ord.Operator_type,
                                       OperatorName = ord.operator_Name,
                                       Gst = cust.gst
                                   }).ToList();
            sb.Money = (from cust in db.imps_whitelabel_distributor_comm
                        where (cust.userid == userid)
                        select new money_comm
                        {
                            Verifycomm = cust.verify_comm,
                            comm_1000 = cust.comm_1000,
                            comm_2000 = cust.comm_2000,
                            comm_3000 = cust.comm_3000,
                            comm_4000 = cust.comm_4000,
                            comm_5000 = cust.comm_5000,
                            gst = cust.gst
                        }).ToList();
            sb.Pencard_comm = (from cust in db.Pancard_whitelabel_dealer_comm
                               where (cust.userid == userid)
                               select new Pencard
                               {
                                   TokenValueDigital = cust.tokenvalueDigital,
                                   TokenValuePhysical = cust.tokenvaluePhysical,
                                   Gst = cust.gst,
                                   CommissionDigital = cust.commDigital,
                                   CommissionPhysical = cust.commPhysical
                               }).ToList();
            //sb.AEPS = (from cust in db.Aeps_comm_new
            //           where cust.userid == userid && cust.role == "Dealer"
            //           select new AEPS_Comm
            //           {
            //               comm = cust.comm,
            //               Maxrs = cust.maxrs,
            //               MinValue = cust.minvalue,
            //               MiniStatement = cust.M_statement
            //           }).ToList();
            sb.Water = (from cust in db.Water_whitelabel_dealer_comm
                        join ord in db.Operator_Code
                        on cust.optcode equals ord.Operator_id.ToString()
                        where (cust.userid == userid)
                        select new Water_Comm
                        {
                            OperatorCode = ord.new_opt_code,
                            Commission = cust.comm,
                            BlockTime = ord.blocktime,
                            Status = ord.status,
                            OperatorType = ord.Operator_type,
                            OperatorName = ord.operator_Name
                        }).ToList();

            sb.Insurance = (from cust in db.Insurance_whitelabel_dealer_comm
                            join ord in db.Operator_Code
                            on cust.optcode equals ord.Operator_id.ToString()
                            where (cust.userid == userid)
                            select new Insurance_Comm
                            {
                                OperatorCode = ord.new_opt_code,
                                Commission = cust.comm,
                                BlockTime = ord.blocktime,
                                Status = ord.status,
                                OperatorType = ord.Operator_type,
                                OperatorName = ord.operator_Name,
                                Gst = cust.gst
                            }).ToList();

            sb.Broadband = (from cust in db.Broandband_whitelabel_dealer_comm
                            join ord in db.Operator_Code
                            on cust.optcode equals ord.Operator_id.ToString()
                            where (cust.userid == userid)
                            select new Broadband_Comm
                            {
                                OperatorCode = ord.new_opt_code,
                                Commission = cust.comm,
                                BlockTime = ord.blocktime,
                                Status = ord.status,
                                OperatorType = ord.Operator_type,
                                OperatorName = ord.operator_Name
                            }).ToList();
            return View(sb);
        }

        [HttpPost]
        public ActionResult All_Operator_Info(string ddlcomm)
        {
            var userid = User.Identity.GetUserId();
            Operater_Commission sb = new Operater_Commission();
            sb.Prepaid = (from cust in db.prepaid_whitelabel_dealer_comm
                          join ord in db.Operator_Code
                          on cust.optcode equals ord.Operator_id.ToString()
                          where (cust.userid == userid)
                          select new Prepaid_Comm
                          {
                              OperatorCode = ord.new_opt_code,
                              Commission = cust.comm,
                              BlockTime = ord.blocktime,
                              Status = ord.status,
                              OperatorType = ord.Operator_type,
                              OperatorName = ord.operator_Name
                          }).ToList();
            sb.Electricity_comm = (from cust in db.utility_whitelabel_dealer_comm
                                   join ord in db.Operator_Code
                                   on cust.optcode equals ord.Operator_id.ToString()
                                   where (cust.userid == userid)
                                   select new Electricity
                                   {
                                       OperatorCode = ord.new_opt_code,
                                       Commission = cust.comm,
                                       BlockTime = ord.blocktime,
                                       Status = ord.status,
                                       OperatorType = ord.Operator_type,
                                       OperatorName = ord.operator_Name,
                                       Gst = cust.gst
                                   }).ToList();
            sb.Money = (from cust in db.imps_whitelabel_distributor_comm
                        where (cust.userid == userid)
                        select new money_comm
                        {
                            Verifycomm = cust.verify_comm,
                            comm_1000 = cust.comm_1000,
                            comm_2000 = cust.comm_2000,
                            comm_3000 = cust.comm_3000,
                            comm_4000 = cust.comm_4000,
                            comm_5000 = cust.comm_5000,
                            gst = cust.gst
                        }).ToList();
            sb.Pencard_comm = (from cust in db.Pancard_whitelabel_dealer_comm
                               where (cust.userid == userid)
                               select new Pencard
                               {
                                   TokenValueDigital = cust.tokenvalueDigital,
                                   TokenValuePhysical = cust.tokenvaluePhysical,
                                   Gst = cust.gst,
                                   CommissionDigital = cust.commDigital,
                                   CommissionPhysical = cust.commPhysical
                               }).ToList();
            //sb.AEPS = (from cust in db.Aeps_comm_new
            //           where cust.userid == userid && cust.role == "Dealer"
            //           select new AEPS_Comm
            //           {
            //               comm = cust.comm,
            //               Maxrs = cust.maxrs,
            //               MinValue = cust.minvalue,
            //               MiniStatement = cust.M_statement
            //           }).ToList();
            sb.Water = (from cust in db.Water_whitelabel_dealer_comm
                        join ord in db.Operator_Code
                        on cust.optcode equals ord.Operator_id.ToString()
                        where (cust.userid == userid)
                        select new Water_Comm
                        {
                            OperatorCode = ord.new_opt_code,
                            Commission = cust.comm,
                            BlockTime = ord.blocktime,
                            Status = ord.status,
                            OperatorType = ord.Operator_type,
                            OperatorName = ord.operator_Name
                        }).ToList();

            sb.Insurance = (from cust in db.Insurance_whitelabel_dealer_comm
                            join ord in db.Operator_Code
                            on cust.optcode equals ord.Operator_id.ToString()
                            where (cust.userid == userid)
                            select new Insurance_Comm
                            {
                                OperatorCode = ord.new_opt_code,
                                Commission = cust.comm,
                                BlockTime = ord.blocktime,
                                Status = ord.status,
                                OperatorType = ord.Operator_type,
                                OperatorName = ord.operator_Name,
                                Gst = cust.gst
                            }).ToList();

            sb.Broadband = (from cust in db.Broandband_whitelabel_dealer_comm
                            join ord in db.Operator_Code
                            on cust.optcode equals ord.Operator_id.ToString()
                            where (cust.userid == userid)
                            select new Broadband_Comm
                            {
                                OperatorCode = ord.new_opt_code,
                                Commission = cust.comm,
                                BlockTime = ord.blocktime,
                                Status = ord.status,
                                OperatorType = ord.Operator_type,
                                OperatorName = ord.operator_Name
                            }).ToList();
            ViewData["type"] = ddlcomm;
            return View(sb);
        }

        public ActionResult BlockOPT(string rem, string id, string opttype)
        {
            var userid = User.Identity.GetUserId();
            if (id != null)
            {
                db.update_retailer_opt_code(rem, id);
            }
            if (opttype == "")
            {
                opttype = "ALL";
            }
            var type = db.Operator_Code.Select(aa => aa.Operator_type).Distinct();
            ViewBag.opttype = new SelectList(type, null);
            var operator_value = (db.select_whitelabel_retailer_for_ddl(userid, "")).ToList();
            ViewBag.retailer = new SelectList(operator_value, "RetailerId", "RetailerName", null);
            //ViewBag.retailer = new SelectList(db.select_retailer_for_all(), "RetailerId", "RetailerName", null);
            var ch1 = db.select_operator_retailer(rem, opttype);
            return View(ch1);
        }
        [HttpPost]
        public ActionResult BlockOPT(string rem, string opttype)
        {
            var userid = User.Identity.GetUserId();
            if (opttype == "")
            {
                opttype = "ALL";
            }
            var type = db.Operator_Code.Select(aa => aa.Operator_type).Distinct();
            ViewBag.opttype = new SelectList(type, null);
            var operator_value = (db.select_whitelabel_retailer_for_ddl(userid, "")).ToList();
            ViewBag.retailer = new SelectList(operator_value, "RetailerId", "RetailerName", null);
            var ch1 = db.select_operator_retailer(rem, opttype);
            return View(ch1);
        }
        #endregion

        #region Retailer_Slab
        [HttpGet]
        public ActionResult Retailer_Slab()
        {
            string userid = User.Identity.GetUserId();
            var hh = (from jj in db.dlm_rem_slab where jj.dlmid == userid where jj.sts == "Y" select jj).ToList();
            ViewBag.slabnm = new SelectList(hh, "slab_name", "slab_name");
            var ch = (from jj in db.retailer_slab where jj.retailer_id == "" select jj);
            return View(ch);
        }
        [HttpPost]
        public ActionResult Retailer_Slab(string slabnm)
        {
            string userid = User.Identity.GetUserId();
            var hh = (from jj in db.dlm_rem_slab where jj.dlmid == userid where jj.sts == "Y" select jj).ToList();
            ViewBag.slabnm = new SelectList(hh, "slab_name", "slab_name");
            var ch = (from jj in db.retailer_slab where jj.retailer_id == slabnm select jj);
            return View(ch);
        }

        #endregion

        #region Dealer Ledger Report
        [HttpGet]
        public ActionResult DealerLedger()
        {
            var userid = User.Identity.GetUserId();
            string txt_to_date1 = DateTime.Now.ToString();
            string frm_date = Convert.ToDateTime(txt_to_date1).ToShortDateString();
            string to_date = Convert.ToDateTime(txt_to_date1).AddDays(1).ToString();
            var ledger = db.whitelabel_Retailer_Cr_Dr_Report("Dealer", userid, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            return View(ledger);
        }

        [HttpPost]
        public ActionResult DealerLedger(string txt_frm_date, string txt_to_date)
        {
            var userid = User.Identity.GetUserId();
            ViewBag.chk = "post";
            DateTime frm = Convert.ToDateTime(txt_frm_date);
            DateTime to = Convert.ToDateTime(txt_to_date);
            txt_frm_date = frm.ToString("dd-MM-yyyy");
            txt_to_date = to.ToString("dd-MM-yyyy");
            string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
            DateTime dt = !string.IsNullOrWhiteSpace(txt_frm_date) ? DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
            DateTime dt1 = !string.IsNullOrWhiteSpace(txt_to_date) ? DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
            DateTime frm_date = Convert.ToDateTime(dt).Date;
            DateTime to_date = Convert.ToDateTime(dt1).Date.AddDays(1);
            var ledger = db.whitelabel_Retailer_Cr_Dr_Report("Dealer", userid, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            return View(ledger);

        }

        #endregion

        #region Money Transfer Report 
        [HttpGet]
        public ActionResult Money_Transfer_Report()
        {
            Money_transfer_Report_whitelabel money = new Money_transfer_Report_whitelabel();
            var userid = User.Identity.GetUserId();
            string txt_frm_date = DateTime.Now.ToString();
            string txt_to_date = DateTime.Now.ToString();
            string frm_date = Convert.ToDateTime(txt_frm_date).Date.ToString("yyyy-MM-dd");
            string to_date = Convert.ToDateTime(txt_to_date).AddDays(1).ToString("yyyy-MM-dd");
            //show retailer
            ViewBag.allretailer = new SelectList(db.select_whitelabel_retailer_for_ddl("Dealer", userid), "RetailerId", "RetailerName", null).ToList();
            //api users 
            ViewBag.apiid = new SelectList(db.select_apiusers_for_ddl("Admin"), "apiid", "username", null).ToList();
            //apiname
            var apiname = db.money_api_status.ToList();
            IEnumerable<SelectListItem> selectapiname = from p in apiname
                                                        select new SelectListItem
                                                        {
                                                            Value = p.api_name,
                                                            Text = p.api_name
                                                        };
            ViewBag.apiname = new SelectList(selectapiname, "Value", "Text");
            var ch = db.whitelabel_money_transfer_report("Dealer", userid, 50, "ALL", "ALL", Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            ViewData["totals"] = ch.Where(s => s.status.ToUpper().Contains("SUCCESS")).Sum(s => Convert.ToInt32(s.amount));
            ViewData["totalf"] = ch.Where(s => s.status.ToUpper().Contains("FAILED")).Sum(s => Convert.ToInt32(s.amount));
            ViewData["totalp"] = ch.Where(s => s.status.ToUpper().Contains("PENDING")).Sum(s => Convert.ToInt32(s.amount));
            money.Money_live = ch;
            return View(money);
        }
        [HttpPost]
        public ActionResult Money_Transfer_Report(string txt_frm_date, string txt_to_date,
      string allretailer, string allapiuser, string ddl_top, string ddl_status, string api_name, string btntype)
        {
            Money_transfer_Report_whitelabel money = new Money_transfer_Report_whitelabel();
            var loginid = User.Identity.GetUserId();
            var userid = "";
            var APIname = "";
            string ddlusers = "";
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

            //show retailer
            ViewBag.allretailer = new SelectList(db.select_whitelabel_retailer_for_ddl("Dealer", loginid), "RetailerId", "RetailerName", null).ToList();

            //apiname
            var apiname = db.money_api_status.ToList();
            IEnumerable<SelectListItem> selectapiname = from p in apiname
                                                        select new SelectListItem
                                                        {
                                                            Value = p.api_name,
                                                            Text = p.api_name
                                                        };
            ViewBag.apiname = new SelectList(selectapiname, "Value", "Text");
            if (allretailer == "" || allretailer == null)
            {
                userid = loginid;
                ddlusers = "Dealer";
            }
            else
            {
                userid = allretailer;
                ddlusers = "Retailer";
            }

            if (ddl_top == "All")
            {
                ddl_top = "1000000";
            }
            if (ddl_status == "Status")
            {
                ddl_status = "ALL";
            }
            if (api_name == "Select Api Name" || api_name.Contains("Select Api Name") || api_name == "")
            {
                APIname = "ALL";
            }
            else
            {
                APIname = api_name.ToUpper();
            }
            int ddltop = Convert.ToInt32(ddl_top);
            if (btntype == "LIVE")
            {
                var ch = db.whitelabel_money_transfer_report(ddlusers, userid, ddltop, ddl_status, APIname, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
                ViewData["totals"] = ch.Where(s => s.status.ToUpper().Contains("SUCCESS")).Sum(s => Convert.ToInt32(s.amount));
                ViewData["totalf"] = ch.Where(s => s.status.ToUpper().Contains("FAILED")).Sum(s => Convert.ToInt32(s.amount));
                ViewData["totalp"] = ch.Where(s => s.status.ToUpper().Contains("PENDING")).Sum(s => Convert.ToInt32(s.amount));
                money.Money_live = ch;
            }
            else
            {
                var ch = db.whitelabel_money_transfer_report_old(ddlusers, userid, ddltop, ddl_status, APIname, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
                ViewData["totals"] = ch.Where(s => s.status.ToUpper().Contains("SUCCESS")).Sum(s => Convert.ToInt32(s.amount));
                ViewData["totalf"] = ch.Where(s => s.status.ToUpper().Contains("FAILED")).Sum(s => Convert.ToInt32(s.amount));
                ViewData["totalp"] = ch.Where(s => s.status.ToUpper().Contains("PENDING")).Sum(s => Convert.ToInt32(s.amount));
                money.Money_old = ch;
            }
            return View(money);

        }
        //End money transfer report

        #endregion

        #region mPossReport
        public ActionResult m_Possreport()
        {
            var userid = User.Identity.GetUserId();
            string txt_frm_date = DateTime.Now.ToString();
            string txt_to_date = DateTime.Now.ToString();
            string frm_date = Convert.ToDateTime(txt_frm_date).Date.ToString("yyyy-MM-dd");
            string to_date = Convert.ToDateTime(txt_to_date).AddDays(1).ToString("yyyy-MM-dd");
            // show master id 

            var ch = db.Whitelabel_Mpos_Report_New("Dealer", userid, "ALL", 50, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            ViewData["totals"] = ch.Where(s => s.status == "00").Sum(s => Convert.ToInt32(s.amount));
            ViewData["Totalf"] = ch.Where(s => s.status != "00").Sum(s => Convert.ToInt32(s.amount));
            return View(ch);
        }

        [HttpPost]
        public ActionResult m_Possreport(string txt_frm_date, string txt_to_date)
        {
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

            var ch = db.Whitelabel_Mpos_Report_New("Dealer", userid, "ALL", 10000, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            ViewData["totals"] = ch.Where(s => s.status == "00").Sum(s => Convert.ToInt32(s.amount));
            ViewData["Totalf"] = ch.Where(s => s.status != "00").Sum(s => Convert.ToInt32(s.amount));
            return View(ch);
        }
        #endregion

        #region Start GST Report 
        public ActionResult GST_Report()
        {
            string userid = User.Identity.GetUserId();
            DateTime fromdate = DateTime.Now.Date;
            DateTime todate = DateTime.Now.Date;
            System.Data.Entity.Core.Objects.ObjectParameter openingBal = new
             System.Data.Entity.Core.Objects.ObjectParameter("OpeningBal", typeof(string));
            System.Data.Entity.Core.Objects.ObjectParameter ClosingBal = new
            System.Data.Entity.Core.Objects.ObjectParameter("ClosingBal", typeof(string));
            System.Data.Entity.Core.Objects.ObjectParameter PurchaseAmt = new
            System.Data.Entity.Core.Objects.ObjectParameter("PurchaseAmt", typeof(string));
            System.Data.Entity.Core.Objects.ObjectParameter IsIGST = new
            System.Data.Entity.Core.Objects.ObjectParameter("IsIGST", typeof(string));
            System.Data.Entity.Core.Objects.ObjectParameter SellAmt = new
        System.Data.Entity.Core.Objects.ObjectParameter("SellAmt", typeof(string));
            var gst = db.GST_Report_Dealer(fromdate, todate, userid, openingBal, ClosingBal, PurchaseAmt, SellAmt, IsIGST).ToList();
            // var gst = db.GST_Report_Retailer(fromdate, todate, userid, openingBal, ClosingBal, PurchaseAmt, IsIGST).ToList();

            ViewBag.OpnBal = !string.IsNullOrWhiteSpace(openingBal.Value.ToString()) ? openingBal.Value.ToString() : "0";
            ViewBag.CloseBal = !string.IsNullOrWhiteSpace(ClosingBal.Value.ToString()) ? ClosingBal.Value.ToString() : "0";
            ViewBag.Purchase = !string.IsNullOrWhiteSpace(PurchaseAmt.Value.ToString()) ? PurchaseAmt.Value.ToString() : "0";
            ViewBag.Sell = !string.IsNullOrWhiteSpace(SellAmt.Value.ToString()) ? SellAmt.Value.ToString() : "0";
            ViewBag.IsIGST = !string.IsNullOrWhiteSpace(IsIGST.Value.ToString()) ? IsIGST.Value.ToString() : "0";
            ViewBag.IsFormSubmmited = db.GST_Declaration.Where(a => a.UserId.ToLower() == userid.ToLower()).SingleOrDefault();

            ViewBag.Months = Enumerable
             .Range(0, 2)
             .Select(i => DateTime.Now.AddMonths(i - 2))
             .Select(date => date.ToString("MMMM/yyyy")).Select(x => new SelectListItem()
             {
                 Text = x.ToString(),
                 Value = x.ToString()

             });
            return View(gst);
        }
        [HttpPost]
        public ActionResult GST_Report(string txt_frm_date, string txt_to_date, HttpPostedFileBase file)
        {
            string userid = User.Identity.GetUserId();
            ViewBag.fromdate = txt_frm_date;
            ViewBag.todate = txt_to_date;
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

            System.Data.Entity.Core.Objects.ObjectParameter openingBal = new
             System.Data.Entity.Core.Objects.ObjectParameter("OpeningBal", typeof(string));
            System.Data.Entity.Core.Objects.ObjectParameter ClosingBal = new
            System.Data.Entity.Core.Objects.ObjectParameter("ClosingBal", typeof(string));
            System.Data.Entity.Core.Objects.ObjectParameter PurchaseAmt = new
            System.Data.Entity.Core.Objects.ObjectParameter("PurchaseAmt", typeof(string));
            System.Data.Entity.Core.Objects.ObjectParameter IsIGST = new
            System.Data.Entity.Core.Objects.ObjectParameter("IsIGST", typeof(string));
            System.Data.Entity.Core.Objects.ObjectParameter SellAmt = new
        System.Data.Entity.Core.Objects.ObjectParameter("SellAmt", typeof(string));
            var gst = db.GST_Report_Dealer(frm_date, to_date, userid, openingBal, ClosingBal, PurchaseAmt, SellAmt, IsIGST).ToList();
            // var gst = db.GST_Report_Retailer(fromdate, todate, userid, openingBal, ClosingBal, PurchaseAmt, IsIGST).ToList();

            ViewBag.OpnBal = !string.IsNullOrWhiteSpace(openingBal.Value.ToString()) ? openingBal.Value.ToString() : "0";
            ViewBag.CloseBal = !string.IsNullOrWhiteSpace(ClosingBal.Value.ToString()) ? ClosingBal.Value.ToString() : "0";
            ViewBag.Purchase = !string.IsNullOrWhiteSpace(PurchaseAmt.Value.ToString()) ? PurchaseAmt.Value.ToString() : "0";
            ViewBag.Sell = !string.IsNullOrWhiteSpace(SellAmt.Value.ToString()) ? SellAmt.Value.ToString() : "0";

            ViewBag.IsIGST = !string.IsNullOrWhiteSpace(IsIGST.Value.ToString()) ? IsIGST.Value.ToString() : "0";
            ViewBag.Months = Enumerable
            .Range(0, 2)
            .Select(i => DateTime.Now.AddMonths(i - 2))
            .Select(date => date.ToString("MMMM/yyyy")).Select(x => new SelectListItem()
            {
                Text = x.ToString(),
                Value = x.ToString()

            });
            if (file != null)
            {
                try
                {

                    if (file.ContentLength > 0)
                    {
                        string _FileName = Guid.NewGuid().ToString() + '_' + Path.GetFileName(file.FileName);
                        string _path = Path.Combine(Server.MapPath("~/GST_Declaration"), _FileName);
                        file.SaveAs(_path);
                        GST_Declaration obj = new GST_Declaration();
                        obj.UserId = userid;
                        obj.FilePath = _FileName;
                        obj.Status = "Pending";
                        obj.Date = DateTime.Now;
                        db.GST_Declaration.Add(obj);
                        db.SaveChanges();
                    }
                    ViewBag.Message = "Success";
                    return View("GST_Report", gst);
                }
                catch
                {
                    ViewBag.Message = "Faild";
                    return View("GST_Report", gst);
                }
            }

            ViewBag.IsFormSubmmited = db.GST_Declaration.Where(a => a.UserId.ToLower() == userid.ToLower()).SingleOrDefault();

            return View(gst);
        }

        #endregion

        #region Download GST declaration form

        [HttpGet]
        public FileResult Download_Declaration_form()
        {
            string[] filesInDirectory = Directory.GetFiles(Server.MapPath("~/GST_Declaration"), "*.docx");

            return File(filesInDirectory[0], "application/force-download", Path.GetFileName(filesInDirectory[0]));
        }

        #endregion

        #region GST Invocing 
        [HttpPost]
        public ActionResult GST_Invoicing(DateTime month)
        {

            string UserId = User.Identity.GetUserId();
            string from = month.ToString("yyyy-MM-dd");
            string to = month.AddMonths(1).ToString("yyyy-MM-dd");
            //string x = DateTime.Now.ToString("yyyy-MM-dd");
            return new Rotativa.ActionAsPdf("GST_INVOICE_PDF", new { userid = UserId, txt_frm_date = from, txt_to_date = to, month = month });
        }
        public ActionResult GST_INVOICE_PDF(string userid, DateTime txt_frm_date, DateTime txt_to_date, DateTime month)
        {
            string CurrentMonth = String.Format("{0:MMMM-yyyy}", month);
            string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy",
             "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
            ViewData["month"] = txt_to_date.AddDays(-1).ToString("dd/MM/yy");
            string MonthName = txt_frm_date.ToString("MMMM");
            string Year = txt_frm_date.ToString("yyyy");
            System.Data.Entity.Core.Objects.ObjectParameter InvoiceNo = new
       System.Data.Entity.Core.Objects.ObjectParameter("InvoiceNo", typeof(string));
            var invc = db.generate_GST_Invoice(userid, MonthName, Year, InvoiceNo).SingleOrDefault().Response.ToString();
            DateTime frm_date = txt_frm_date.Date;
            DateTime to_date = txt_to_date.Date;
            Vastwebmulti.Areas.WRetailer.Models.GST_PDF_MODEL model = new Vastwebmulti.Areas.WRetailer.Models.GST_PDF_MODEL();
            var role = db.showrole(userid).SingleOrDefault();
            System.Data.Entity.Core.Objects.ObjectParameter openingBal = new
             System.Data.Entity.Core.Objects.ObjectParameter("OpeningBal", typeof(string));
            System.Data.Entity.Core.Objects.ObjectParameter ClosingBal = new
            System.Data.Entity.Core.Objects.ObjectParameter("ClosingBal", typeof(string));
            System.Data.Entity.Core.Objects.ObjectParameter PurchaseAmt = new
            System.Data.Entity.Core.Objects.ObjectParameter("PurchaseAmt", typeof(string));
            System.Data.Entity.Core.Objects.ObjectParameter IsIGST = new
            System.Data.Entity.Core.Objects.ObjectParameter("IsIGST", typeof(string));
            System.Data.Entity.Core.Objects.ObjectParameter SellAmt = new
        System.Data.Entity.Core.Objects.ObjectParameter("SellAmt", typeof(string));
            if (role.Name.Contains("Dealer"))
            {
                model.DealerGst = db.GST_Report_Dealer(frm_date, to_date, userid, openingBal, ClosingBal, PurchaseAmt, SellAmt, IsIGST).
                          Where(a => a.operator_Name != "Dish TV" && !a.Operator_type.Contains("PostPaid") && a.Operator_type != "Landline" && a.Operator_type != "Electricity" && a.Operator_type != "Gas" && a.Operator_type != "Insurance" && a.Operator_type != "Money" && a.Operator_type != "DTH-Booking").ToList();
                //Convert function call
                var converword = new Vastwebmulti.Areas.WRetailer.Models.Convertword().changeToWords(model.DealerGst.Sum(a => a.NetAmount).ToString());
                ViewData["total"] = converword;
                var UserDetails = db.Dealer_Details.Where(a => a.DealerId.ToLower() == userid.ToLower()).SingleOrDefault();
                ViewBag.UserDetails = UserDetails;
            }

            ViewBag.OpnBal = !string.IsNullOrWhiteSpace(openingBal.Value.ToString()) ? openingBal.Value.ToString() : "0";
            ViewBag.CloseBal = !string.IsNullOrWhiteSpace(ClosingBal.Value.ToString()) ? ClosingBal.Value.ToString() : "0";
            ViewBag.Purchase = !string.IsNullOrWhiteSpace(PurchaseAmt.Value.ToString()) ? PurchaseAmt.Value.ToString() : "0";
            ViewBag.IsIGST = !string.IsNullOrWhiteSpace(IsIGST.Value.ToString()) ? IsIGST.Value.ToString() : "0";
            var AdminDetails = db.Admin_details.SingleOrDefault();
            ViewBag.AdminDetails = AdminDetails;
            ViewBag.Role = role;


            model.INVOICENO = invc;
            //ViewBag.State = db.State_Desc.Where(y => y.State_id == AdminDetails.State).Single().State_name;
            ViewBag.city = db.District_Desc.Where(f => f.Dist_id == AdminDetails.District && f.State_id == AdminDetails.State).Single().Dist_Desc;

            return View(model);
        }

        #endregion

        #region GST PDF Invoicing 
        public ActionResult GenerateGST_PDF(DateTime frm_date, DateTime to_date)
        {
            return new Rotativa.ActionAsPdf("GST_PDF", new { txt_frm_date = frm_date, txt_to_date = to_date });
        }
        public ActionResult GST_PDF(string txt_frm_date, string txt_to_date)
        {

            string userid = User.Identity.GetUserId();

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
            System.Data.Entity.Core.Objects.ObjectParameter openingBal = new
                  System.Data.Entity.Core.Objects.ObjectParameter("OpeningBal", typeof(string));
            System.Data.Entity.Core.Objects.ObjectParameter ClosingBal = new
            System.Data.Entity.Core.Objects.ObjectParameter("ClosingBal", typeof(string));
            System.Data.Entity.Core.Objects.ObjectParameter PurchaseAmt = new
            System.Data.Entity.Core.Objects.ObjectParameter("PurchaseAmt", typeof(string));
            System.Data.Entity.Core.Objects.ObjectParameter IsIGST = new
            System.Data.Entity.Core.Objects.ObjectParameter("IsIGST", typeof(string));
            System.Data.Entity.Core.Objects.ObjectParameter SellAmt = new
        System.Data.Entity.Core.Objects.ObjectParameter("SellAmt", typeof(string));
            var gst = db.GST_Report_Dealer(frm_date, to_date, userid, openingBal, ClosingBal, PurchaseAmt, SellAmt, IsIGST).ToList();
            // var gst = db.GST_Report_Retailer(fromdate, todate, userid, openingBal, ClosingBal, PurchaseAmt, IsIGST).ToList();

            ViewBag.OpnBal = !string.IsNullOrWhiteSpace(openingBal.Value.ToString()) ? openingBal.Value.ToString() : "0";
            ViewBag.CloseBal = !string.IsNullOrWhiteSpace(ClosingBal.Value.ToString()) ? ClosingBal.Value.ToString() : "0";
            ViewBag.Purchase = !string.IsNullOrWhiteSpace(PurchaseAmt.Value.ToString()) ? PurchaseAmt.Value.ToString() : "0";
            ViewBag.IsIGST = !string.IsNullOrWhiteSpace(IsIGST.Value.ToString()) ? IsIGST.Value.ToString() : "0";
            var UserDetails = db.Dealer_Details.Where(a => a.DealerId.ToLower() == userid.ToLower()).SingleOrDefault();
            ViewBag.UserDetails = UserDetails;
            var AdminDetails = db.Admin_details.SingleOrDefault();
            ViewBag.AdminDetails = AdminDetails;
            return View(gst);
        }
        #endregion

        #region Reports
        //start recharge report
        [HttpGet]
        public ActionResult Recharge_Report()
        {

            string userid = User.Identity.GetUserId();
            var whitelabelid = db.whitelabel_Dealer_Details.Where(a => a.DealerId == userid).Single().Whitelabelid;
            ViewBag.alldealer = new SelectList(db.select_whitelabel_DealerName_for_ddl(whitelabelid), "Dealerid", "DealerName", null).ToList();
            ViewBag.allretailer = new SelectList(db.select_whitelabel_retailer_for_ddl("Dealer", userid), "RetailerId", "RetailerName", null).ToList();
            var operator_value = db.Operator_Code.Distinct().ToList();
            ViewBag.Operator = new SelectList(operator_value, "new_opt_code", "operator_Name");
            string txt_frm_date = DateTime.Now.ToString();
            string txt_to_date = DateTime.Now.ToString();
            string frm_date = Convert.ToDateTime(txt_frm_date).Date.ToString("yyyy-MM-dd");
            string to_date = Convert.ToDateTime(txt_to_date).AddDays(1).ToString("yyyy-MM-dd");
            var Liverechargecount = db.whitelabel_Recharge_Report_live(Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), "Dealer", userid, "ALL", "ALL", "ALL", 50).ToList();
            var totalsuccesscount = Liverechargecount.Where(a => a.rstatus.ToUpper() == "SUCCESS").Sum(a => a.amount);
            var totalfailed = Liverechargecount.Where(a => a.rstatus.ToUpper() == "FAILED").Sum(a => a.amount);
            var totalpending = Liverechargecount.Where(a => a.rstatus.Contains("temp")).Sum(a => a.amount);
            var dlmincome = Liverechargecount.Where(a => a.rstatus.ToUpper() == "SUCCESS").Sum(a => a.dealerincome);
            var masterincome = Liverechargecount.Where(a => a.rstatus.ToUpper() == "SUCCESS").Sum(a => a.masterincome);
            var retailerincome = Liverechargecount.Where(a => a.rstatus.ToUpper() == "SUCCESS").Sum(a => a.income);
            var whitelabelincome = Liverechargecount.Where(a => a.rstatus.ToUpper() == "SUCCESS").Sum(a => a.whitelabelincome);
            ViewData["Totals"] = totalsuccesscount;
            ViewData["Totalf"] = totalfailed;
            ViewData["Totalp"] = totalpending;
            ViewData["Totald"] = dlmincome;
            ViewData["Totalr"] = retailerincome;
            Vastwebmulti.Areas.WDealer.Models.recharge_report viewModel = new Vastwebmulti.Areas.WDealer.Models.recharge_report();
            viewModel.rechargereport = db.whitelabel_Recharge_Report_live(Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), "Dealer", userid, "ALL", "ALL", "ALL", 50).ToList();

            return View(viewModel);
        }
        [HttpPost]
        public ActionResult Recharge_Report(string btntype, string txt_frm_date, string txt_to_date, string ddl_status, string Operator, string ddl_top, string txtmob, string allretailer)
        {


            ViewBag.chk = "post";
            var userid = User.Identity.GetUserId();
            var mobile = "";
            var optname = "";
            string ddlusers = "";
            ViewBag.alldealer = new SelectList(db.select_Dealer_for_ddl(), "Dealerid", "DealerName", null).ToList();
            ViewBag.allretailer = new SelectList(db.select_whitelabel_retailer_for_ddl("Dealer", userid), "RetailerId", "RetailerName", null).ToList();
            if (allretailer == "" || allretailer == null)
            {
                // userid = userid;
                ddlusers = "Dealer";
            }
            else
            {
                userid = allretailer;
                ddlusers = "Whitelabelretailer";
            }
            //if (ddlusers == "Retailer")
            //{
            //    if (allretailer == "" || allretailer.Contains("All Retailer") || allretailer == null)
            //    {
            //        userid = "ALL";
            //    }
            //    else
            //    {
            //        userid = allretailer;
            //    }
            //}
            if (ddl_top == "All")
            {
                ddl_top = "1000000";
            }
            if (Operator == null || Operator == "" || Operator.Contains("ALL OPERATORS"))
            {
                optname = "ALL";
            }
            else
            {
                optname = Operator;
            }
            if (txtmob == "")
            {
                mobile = "ALL";
            }
            else
            {
                mobile = txtmob;
            }

            int ddltop = Convert.ToInt32(ddl_top);

            var operator_value = db.Operator_Code.Distinct().ToList();
            ViewBag.Operator = new SelectList(operator_value, "new_opt_code", "operator_Name");
            DateTime frm1 = Convert.ToDateTime(txt_frm_date);
            DateTime to1 = Convert.ToDateTime(txt_to_date);
            txt_frm_date = frm1.ToString("dd-MM-yyyy");
            txt_to_date = to1.ToString("dd-MM-yyyy");
            string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy",
                            "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
            DateTime dt = !string.IsNullOrWhiteSpace(txt_frm_date) ? DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
            DateTime dt1 = !string.IsNullOrWhiteSpace(txt_to_date) ? DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
            DateTime frm_date = Convert.ToDateTime(dt).Date;
            DateTime to_date = Convert.ToDateTime(dt1).Date.AddDays(1);
            Vastwebmulti.Areas.WDealer.Models.recharge_report viewModel = new Vastwebmulti.Areas.WDealer.Models.recharge_report();
            if (btntype == "LIVE")
            {
                var Liverechargecount = db.whitelabel_Recharge_Report_live(Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), "Dealer", userid, ddl_status, optname, mobile, ddltop).ToList();
                var totalsuccesscount = Liverechargecount.Where(a => a.rstatus.ToUpper() == "SUCCESS").Sum(a => a.amount);
                var totalfailed = Liverechargecount.Where(a => a.rstatus.ToUpper() == "FAILED").Sum(a => a.amount);
                var totalpending = Liverechargecount.Where(a => a.rstatus.Contains("temp")).Sum(a => a.amount);
                var dlmincome = Liverechargecount.Where(a => a.rstatus.ToUpper() == "SUCCESS").Sum(a => a.dealerincome);
                var masterincome = Liverechargecount.Where(a => a.rstatus.ToUpper() == "SUCCESS").Sum(a => a.masterincome);
                var retailerincome = Liverechargecount.Where(a => a.rstatus.ToUpper() == "SUCCESS").Sum(a => a.income);
                var whitelabelincome = Liverechargecount.Where(a => a.rstatus.ToUpper() == "SUCCESS").Sum(a => a.whitelabelincome);
                ViewData["Totals"] = totalsuccesscount;
                ViewData["Totalf"] = totalfailed;
                ViewData["Totalp"] = totalpending;
                ViewData["Totald"] = dlmincome;
                ViewData["Totalr"] = retailerincome;
                viewModel.rechargereport = db.whitelabel_Recharge_Report_live(Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), ddlusers, userid, ddl_status, optname, mobile, ddltop).ToList();
            }
            else if (btntype == "OLD")
            {
                var Liverechargecount = db.whitelabel_Recharge_Report_old(Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), "Dealer", userid, ddl_status, optname, mobile, ddltop).ToList();
                var totalsuccesscount = Liverechargecount.Where(a => a.rstatus.ToUpper() == "SUCCESS").Sum(a => a.amount);
                var totalfailed = Liverechargecount.Where(a => a.rstatus.ToUpper() == "FAILED").Sum(a => a.amount);
                var totalpending = Liverechargecount.Where(a => a.rstatus.Contains("temp")).Sum(a => a.amount);
                var dlmincome = Liverechargecount.Where(a => a.rstatus.ToUpper() == "SUCCESS").Sum(a => a.dealerincome);
                var masterincome = Liverechargecount.Where(a => a.rstatus.ToUpper() == "SUCCESS").Sum(a => a.masterincome);
                var retailerincome = Liverechargecount.Where(a => a.rstatus.ToUpper() == "SUCCESS").Sum(a => a.income);
                var whitelabelincome = Liverechargecount.Where(a => a.rstatus.ToUpper() == "SUCCESS").Sum(a => a.whitelabelincome);
                ViewData["Totals"] = totalsuccesscount;
                ViewData["Totalf"] = totalfailed;
                ViewData["Totalp"] = totalpending;
                ViewData["Totald"] = dlmincome;
                ViewData["Totalr"] = retailerincome;
                viewModel.rechargereport_old = db.whitelabel_Recharge_Report_old(Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), ddlusers, userid, ddl_status, optname, mobile, ddltop).ToList();

            }

            return View(viewModel);

            //string loginuserid = User.Identity.GetUserId();
            //ViewBag.fromdate = txt_frm_date;
            //ViewBag.todate = txt_to_date;
            //var userid = "";
            //var mobile = "";
            //var optname = "";

            ////if (ddlusers == "Dealer")
            ////{
            ////    userid = loginuserid;
            ////}

            ////if (ddlusers == "Retailer")
            ////{
            ////    if (allretailer == "" || allretailer.Contains("All Retailer") || allretailer == null)
            ////    {
            ////        userid = "ALL";
            ////    }
            ////    else
            ////    {
            ////        userid = allretailer;
            ////    }
            ////}
            //if (ddl_top == "All")
            //{
            //    ddl_top = "1000000";
            //}
            //if (Operator == null || Operator == "" || Operator.Contains("ALL OPERATORS"))
            //{
            //    optname = "ALL";
            //}
            //else
            //{
            //    optname = Operator;
            //}
            //if (txtmob == "")
            //{
            //    mobile = "ALL";
            //}
            //else
            //{
            //    mobile = txtmob;
            //}

            //int ddltop = Convert.ToInt32(ddl_top);
            //ViewBag.alldealer = new SelectList(db.select_whitelabel_DealerName_for_ddl(), "Dealerid", "DealerName", null).ToList();
            //ViewBag.allretailer = new SelectList(db.select_whitelabel_retailer_for_ddl("Admin"), "RetailerId", "RetailerName", null).ToList();
            //var operator_value = db.Operator_Code.Distinct().ToList();
            //ViewBag.Operator = new SelectList(operator_value, "new_opt_code", "operator_Name");
            //DateTime frm1 = Convert.ToDateTime(txt_frm_date);
            //DateTime to1 = Convert.ToDateTime(txt_to_date);
            //txt_frm_date = frm1.ToString("dd-MM-yyyy");
            //txt_to_date = to1.ToString("dd-MM-yyyy");
            //string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy",
            //                "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
            //DateTime dt = !string.IsNullOrWhiteSpace(txt_frm_date) ? DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
            //DateTime dt1 = !string.IsNullOrWhiteSpace(txt_to_date) ? DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
            //DateTime frm_date = Convert.ToDateTime(dt).Date;
            //DateTime to_date = Convert.ToDateTime(dt1).Date.AddDays(1);
            //Vastwebmulti.Areas.WDealer.Models.recharge_report viewModel = new Vastwebmulti.Areas.WDealer.Models.recharge_report();
            //if (btntype == "LIVE")
            //{
            //    var Liverechargecount = db.whitelabel_Recharge_Report_live(Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), "Dealer", userid, ddl_status, optname, mobile, ddltop).ToList();
            //    var totalsuccesscount = Liverechargecount.Where(a => a.rstatus.ToUpper() == "SUCCESS").Sum(a => a.amount);
            //    var totalfailed = Liverechargecount.Where(a => a.rstatus.ToUpper() == "FAILED").Sum(a => a.amount);
            //    var totalpending = Liverechargecount.Where(a => a.rstatus.Contains("temp")).Sum(a => a.amount);
            //    var dlmincome = Liverechargecount.Where(a => a.rstatus.ToUpper() == "SUCCESS").Sum(a => a.dealerincome);
            //    var masterincome = Liverechargecount.Where(a => a.rstatus.ToUpper() == "SUCCESS").Sum(a => a.masterincome);
            //    var retailerincome = Liverechargecount.Where(a => a.rstatus.ToUpper() == "SUCCESS").Sum(a => a.income);
            //    var whitelabelincome = Liverechargecount.Where(a => a.rstatus.ToUpper() == "SUCCESS").Sum(a => a.whitelabelincome);
            //    ViewData["Totals"] = totalsuccesscount;
            //    ViewData["Totalf"] = totalfailed;
            //    ViewData["Totalp"] = totalpending;
            //    ViewData["Totald"] = dlmincome;
            //    ViewData["Totalr"] = retailerincome;
            //    viewModel.rechargereport = db.whitelabel_Recharge_Report_live(Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), ddlusers, userid, ddl_status, optname, mobile, ddltop).ToList();
            //}
            //else if (btntype == "OLD")
            //{

            //}

            //return View(viewModel);
        }
        //end
        #endregion

        #region  pancrad Report
        public ActionResult Pancard_Report()
        {
            string userid = User.Identity.GetUserId();
            string txt_frm_date = DateTime.Now.ToString();
            string txt_to_date = DateTime.Now.ToString();
            string frm_date = Convert.ToDateTime(txt_frm_date).Date.ToString("yyyy-MM-dd");
            string to_date = Convert.ToDateTime(txt_to_date).AddDays(1).ToString("yyyy-MM-dd");
            //Retailer 
            ViewBag.allretailer = new SelectList(db.select_whitelabel_retailer_for_ddl("Dealer", userid), "RetailerId", "RetailerName", null).ToList();
            var ch = db.proc_Whitelabel_PAN_CARD_IPAY_Token_report(userid, "Dealer", 50, "ALL", Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            ViewData["totals"] = ch.Where(s => s.Particulars.ToUpper().Contains("SUCCESS")).Sum(s => Convert.ToInt32(s.ProcessingFees));
            ViewData["totalf"] = ch.Where(s => s.Particulars.ToUpper().Contains("FAILED")).Sum(s => Convert.ToInt32(s.ProcessingFees));
            ViewData["totalp"] = ch.Where(s => s.Particulars.ToUpper().Contains("PENDING")).Sum(s => Convert.ToInt32(s.ProcessingFees));
            return View(ch);
        }
        [HttpPost]
        public ActionResult Pancard_Report(string txt_frm_date, string txt_to_date, string allretailer, string ddl_top, string ddl_status)
        {
            var userid = User.Identity.GetUserId();
            var status = "";
            var ddlusers = "";
            ViewBag.chk = "post";

            DateTime frm = Convert.ToDateTime(txt_frm_date);
            DateTime to = Convert.ToDateTime(txt_to_date);


            txt_frm_date = frm.ToString("dd-MM-yyyy");
            txt_to_date = to.ToString("dd-MM-yyyy");

            ViewBag.apiid = new SelectList(db.select_apiusers_for_ddl("Admin"), "apiid", "username", null).ToList();
            string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy",
                            "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
            DateTime dt = DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            DateTime dt1 = DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            string frm_date = Convert.ToDateTime(dt).ToShortDateString();
            string to_date = Convert.ToDateTime(dt1).AddDays(1).ToString();

            ///Retailer List
            ViewBag.allretailer = new SelectList(db.select_whitelabel_retailer_for_ddl("Dealer", userid), "RetailerId", "RetailerName", null).ToList();

            if (allretailer == "" || allretailer.Contains("All Retailer"))
            {
                userid = User.Identity.GetUserId();
                ddlusers = "Dealer";
            }
            else
            {
                userid = allretailer;
                ddlusers = "Retailer";
            }
            if (ddl_top == "All")
            {
                ddl_top = "1000000";
            }
            if (ddl_status == "All")
            {
                status = "ALL";
            }
            else
            {
                status = ddl_status;
            }
            int ddltop = Convert.ToInt32(ddl_top);
            var ch = db.proc_Whitelabel_PAN_CARD_IPAY_Token_report(userid, ddlusers, ddltop, status, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            ViewData["totals"] = ch.Where(s => s.Particulars.ToUpper().Contains("SUCCESS")).Sum(s => Convert.ToInt32(s.ProcessingFees));
            ViewData["totalf"] = ch.Where(s => s.Particulars.ToUpper().Contains("FAILED")).Sum(s => Convert.ToInt32(s.ProcessingFees));
            ViewData["totalp"] = ch.Where(s => s.Particulars.ToUpper().Contains("PENDING")).Sum(s => Convert.ToInt32(s.ProcessingFees));
            return View(ch);
        }
        #endregion

        #region Aeps_Report
        public ActionResult Aeps_Report()
        {
            string userid = User.Identity.GetUserId();
            string txt_frm_date = DateTime.Now.ToString();
            string txt_to_date = DateTime.Now.ToString();
            string frm_date = Convert.ToDateTime(txt_frm_date).Date.ToString("yyyy-MM-dd");
            string to_date = Convert.ToDateTime(txt_to_date).AddDays(1).ToString("yyyy-MM-dd");
            //Retailer 
            ViewBag.allretailer = new SelectList(db.select_whitelabel_retailer_for_ddl("Dealer", userid), "RetailerId", "RetailerName", null).ToList();
            var ch = db.proc_whitelabel_AepsReport(null, userid, null, null, 50, null, null, null, null, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            ViewData["totals"] = ch.Where(s => s.Status.ToUpper().Contains("SUCCESS")).Sum(s => Convert.ToInt32(s.Amount));
            ViewData["totalf"] = ch.Where(s => s.Status.ToUpper().Contains("FAILED")).Sum(s => Convert.ToInt32(s.Amount));
            ViewData["totalp"] = ch.Where(s => s.Status.ToUpper().Contains("PENDING")).Sum(s => Convert.ToInt32(s.Amount));
            return View(ch);
        }
        [HttpPost]
        public ActionResult Aeps_Report(string txt_frm_date, string txt_to_date, string ddlusers, string allmaster, string alldealer, string allretailer, string ddl_top, string ddl_status, string api_name, string allapiuser)
        {
            var retailerid = "";
            string userid = User.Identity.GetUserId();
            ViewBag.chk = "post";

            DateTime frm = Convert.ToDateTime(txt_frm_date);
            DateTime to = Convert.ToDateTime(txt_to_date);


            txt_frm_date = frm.ToString("dd-MM-yyyy");
            txt_to_date = to.ToString("dd-MM-yyyy");

            ViewBag.apiid = new SelectList(db.select_apiusers_for_ddl("Admin"), "apiid", "username", null).ToList();
            string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy",
                            "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
            DateTime dt = DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            DateTime dt1 = DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            string frm_date = Convert.ToDateTime(dt).ToShortDateString();
            string to_date = Convert.ToDateTime(dt1).AddDays(1).ToString();
            //Retailer 
            ViewBag.allretailer = new SelectList(db.select_whitelabel_retailer_for_ddl("Dealer", userid), "RetailerId", "RetailerName", null).ToList();
            if (ddl_top == "All")
            {
                ddl_top = "1000000";
            }
            if (ddl_status == "Status")
            {
                ddl_status = null;
            }
            if (ddl_status == "ALL")
            {
                ddl_status = null;
            }
            if (allretailer == "" || allretailer.Contains("All Retailer"))
            {
                retailerid = null;
            }
            else
            {
                retailerid = allretailer;
            }

            int ddltop = Convert.ToInt32(ddl_top);

            allretailer = string.IsNullOrWhiteSpace(allretailer) ? null : allretailer;
            alldealer = string.IsNullOrWhiteSpace(alldealer) ? null : alldealer;
            allretailer = string.IsNullOrWhiteSpace(allretailer) ? null : allretailer;
            var ch = db.proc_whitelabel_AepsReport(retailerid, userid, null, ddl_status, ddltop, null, null, null, null, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
            ViewData["totals"] = ch.Where(s => s.Status.ToUpper().Contains("SUCCESS")).Sum(s => Convert.ToInt32(s.Amount));
            ViewData["totalf"] = ch.Where(s => s.Status.ToUpper().Contains("FAILED")).Sum(s => Convert.ToInt32(s.Amount));
            ViewData["totalp"] = ch.Where(s => s.Status.ToUpper().Contains("PENDING")).Sum(s => Convert.ToInt32(s.Amount));
            return View(ch);
        }
        #endregion

        #region Travels Reports
        public ActionResult Travels()
        {
            string userid = User.Identity.GetUserId();
            //show all retailer 
            ViewBag.allretailer = new SelectList(db.select_whitelabel_retailer_for_ddl("Dealer", userid), "RetailerId", "RetailerName", null).ToList();
            // traves operators
            var operator_value = db.Operator_Code.Where(a => a.operator_Name == "Hotel" || a.operator_Name == "Bus" || a.operator_Name == "Flight").ToList();
            ViewBag.Operator = new SelectList(operator_value, "new_opt_code", "operator_Name");
            return View();
        }
        #endregion

        #region Giftcards Reports
        public ActionResult Giftcard()
        {
            string userid = User.Identity.GetUserId();
            //show all retailer 
            //show all retailer 
            ViewBag.allretailer = new SelectList(db.select_whitelabel_retailer_for_ddl("Dealer", userid), "RetailerId", "RetailerName", null).ToList();
            var category = db.GiftCards.Distinct().ToList();
            ViewBag.category = new SelectList(category, "Category", "Category");
            return View();
        }
        #endregion

        #region Ecommerce Reports
        public ActionResult Ecommerce_Report()
        {
            string userid = User.Identity.GetUserId();
            //show all retailer 
            //show all retailer 
            ViewBag.allretailer = new SelectList(db.select_whitelabel_retailer_for_ddl("Dealer", userid), "RetailerId", "RetailerName", null).ToList();

            var category = db.Catagories.Distinct().ToList();
            ViewBag.category = new SelectList(category, "CatName", "CatName");
            return View();
        }
        #endregion

        #region Complaint
        public ActionResult Complaint()
        {
            var userid = User.Identity.GetUserId();
            var whitelabelid = db.whitelabel_Dealer_Details.Where(a => a.DealerId == userid).FirstOrDefault().Whitelabelid;
            var whitelabelemail = db.WhiteLabel_userList.Where(a => a.WhiteLabelID == whitelabelid).FirstOrDefault().EmailId;
            var ch = db.proc_whitelabel_complaint_request(userid, "", whitelabelemail).ToList();
            return View(ch);
        }
        public ActionResult Complaint_insert(string message)
        {
            string userid = User.Identity.GetUserId();
            var whitelabelid = db.whitelabel_Dealer_Details.Where(a => a.DealerId == userid).FirstOrDefault().Whitelabelid;
            var whitelabelemail = db.WhiteLabel_userList.Where(a => a.WhiteLabelID == whitelabelid).FirstOrDefault().EmailId;
            Guid randomId = Guid.NewGuid();
            string uniqueId = randomId.ToString().Substring(0, 18).ToUpper();
            DateTime date = System.DateTime.Now;
            whitelabel_complaint_request objCourse = new whitelabel_complaint_request();
            objCourse.subject = "Chatting";
            objCourse.complant = message;
            objCourse.complaintid = uniqueId;
            objCourse.userid = userid;
            objCourse.sts = "Open";
            objCourse.rdate = date;
            objCourse.Emailid = whitelabelemail;
            db.whitelabel_complaint_request.Add(objCourse);
            db.SaveChanges();
            return RedirectToAction("Complaint");
        }
        #endregion

        #region Dealer Gst Invocie Report
        public ActionResult Gst_Invocing_Dealer_report()
        {
            var userid = User.Identity.GetUserId();
            DateTime txt_frm_date = DateTime.Now;
            DateTime to_date = DateTime.Now;
            DateTime date = DateTime.Now;
            string OldMonth = date.AddMonths(-1).ToString("MMMM");

            ViewBag.OldMonth = OldMonth;
            txt_frm_date = new DateTime(date.AddMonths(-1).Year, date.AddMonths(-1).Month, 1);
            to_date = new DateTime(date.Year, date.Month, 1);
            var show = db.GST_Monthly_wDealer(Convert.ToDateTime(txt_frm_date), Convert.ToDateTime(to_date)).Where(a => a.apiid == userid).ToList();
            return View(show);
        }

        public ActionResult GST_Invocing_Report_Pdf()
        {
            var userid = User.Identity.GetUserId();
            var today = DateTime.Today;
            var month = new DateTime(today.Year, today.Month, 1);
            var last = month.AddDays(-1);
            ViewBag.last = last.ToShortDateString();

            DateTime txt_frm_date = DateTime.Now;
            DateTime to_date = DateTime.Now;
            DateTime date = DateTime.Now;
            txt_frm_date = new DateTime(date.AddMonths(-1).Year, date.AddMonths(-1).Month, 1);
            to_date = new DateTime(date.Year, date.Month, 1);

            var entries = db.GST_Monthly_wDealer(Convert.ToDateTime(txt_frm_date), Convert.ToDateTime(to_date)).Where(a => a.apiid == userid).ToList();
            ViewBag.name = entries.SingleOrDefault().DealerName;
            ViewBag.address = entries.SingleOrDefault().Address;
            ViewBag.customergst = entries.SingleOrDefault().GST;
            ViewBag.dmtcomm = entries.SingleOrDefault().dmtcomm;
            ViewBag.dmttotal = entries.SingleOrDefault().dmtcomm + entries.SingleOrDefault().dmtgst;
            ViewBag.rechargecomm = entries.SingleOrDefault().rchcomm;
            ViewBag.rechargetotal = entries.SingleOrDefault().rchcomm + entries.SingleOrDefault().rchgst;
            ViewBag.Mposcomm = entries.SingleOrDefault().mposcomm;
            ViewBag.Mpostotal = entries.SingleOrDefault().mposcomm + entries.SingleOrDefault().mposgst;
            ViewBag.Aepscomm = entries.SingleOrDefault().aepscomm;
            ViewBag.Aepstotal = entries.SingleOrDefault().aepscomm + entries.SingleOrDefault().aepsgst;
            ViewBag.Pancomm = entries.SingleOrDefault().pancomm;
            ViewBag.Pantotal = entries.SingleOrDefault().pancomm + entries.SingleOrDefault().pangst;
            ViewBag.Flightcomm = entries.SingleOrDefault().flightcomm;
            ViewBag.Flighttotal = entries.SingleOrDefault().flightcomm + entries.SingleOrDefault().flightgst;
            var final = entries.SingleOrDefault().dmtcomm + entries.SingleOrDefault().rchcomm +
                entries.SingleOrDefault().mposcomm + entries.SingleOrDefault().aepscomm +
                entries.SingleOrDefault().pancomm + entries.SingleOrDefault().flightcomm;
            ViewBag.totaltaxvalue = final;

            var valdmt = entries.SingleOrDefault().dmtgst;
            var valrecharge = entries.SingleOrDefault().rchgst;
            var valmpos = entries.SingleOrDefault().mposgst;
            var valaeps = entries.SingleOrDefault().aepsgst;
            var valpan = entries.SingleOrDefault().pangst;
            var valFlight = entries.SingleOrDefault().flightgst;
            var finalgst = valdmt + valrecharge + valmpos + valaeps + valpan +
                valFlight;

            ViewBag.finaltotal = finalgst + final;
            ViewBag.totalgsttotal = finalgst;
            double fin = Convert.ToDouble(finalgst + final);
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
            if (entries.SingleOrDefault().State_name != "Rajasthan")
            {
                ViewBag.type = "N";
                ViewBag.igst = 18;
                ViewBag.dmtigst = valdmt;
                ViewBag.rechargeigst = valrecharge;
                ViewBag.mposigst = valmpos;
                ViewBag.Aepsigst = valaeps;
                ViewBag.Panigst = valpan;
                ViewBag.Flightigst = valFlight;
                ViewBag.totaligst = finalgst;
            }
            else
            {
                ViewBag.type = "Y";
                ViewBag.cgst = 9;
                ViewBag.sgst = 9;


                ViewBag.dmtcgst = valdmt / 2;
                ViewBag.dmtsgst = valdmt / 2;
                ViewBag.rechargecgst = valrecharge / 2;
                ViewBag.rechargesgst = valrecharge / 2;
                ViewBag.mposcgst = valmpos / 2;
                ViewBag.mpossgst = valmpos / 2;
                ViewBag.Aepscgst = valaeps / 2;
                ViewBag.Aepssgst = valaeps / 2;
                ViewBag.Pancgst = valpan / 2;
                ViewBag.Pansgst = valpan / 2;
                ViewBag.Flightcgst = valFlight / 2;
                ViewBag.Flightsgst = valFlight / 2;
                ViewBag.totalcgst = finalgst / 2;
                ViewBag.totalsgst = finalgst / 2;
            }

            //ViewBag.particular = "Commission For " + from.ToString("MMMM") + " Month";
            ViewBag.netamount = entries.SingleOrDefault().dmtcomm;
            ViewBag.firmname = entries.SingleOrDefault().FarmName;
            ViewBag.total = entries.SingleOrDefault().dmtcomm + entries.SingleOrDefault().dmtgst;
            var whitelabelid = db.whitelabel_Dealer_Details.Where(a => a.DealerId == userid).FirstOrDefault().Whitelabelid;
            var admininfo = db.WhiteLabel_userList.Where(a => a.WhiteLabelID == whitelabelid).FirstOrDefault();
            ViewBag.cmpyname = admininfo.FrmName;
            ViewBag.adminaddress = admininfo.Address;
            ViewBag.adminpan = admininfo.PanCardnumber;
            ViewBag.admingst = admininfo.GstNumber;
            ViewBag.pancard = entries.SingleOrDefault().PanCard;
            var number = entries.SingleOrDefault().dmtcomm.ToString();
            number = Convert.ToDouble(number).ToString();

            return new ViewAsPdf("GST_Invocing_Report_Pdf", entries);

        }
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

        static readonly string PasswordHash = "P@@Sw0rd";
        static readonly string SaltKey = "S@LT&KEY";
        static readonly string VIKey = "@1B2c3D4e5F6g7H8";
        public static string Encrypt(string plainText)
        {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.Zeros };
            var encryptor = symmetricKey.CreateEncryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));

            byte[] cipherTextBytes;

            using (var memoryStream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                    cryptoStream.FlushFinalBlock();
                    cipherTextBytes = memoryStream.ToArray();
                    cryptoStream.Close();
                }
                memoryStream.Close();
            }
            return Convert.ToBase64String(cipherTextBytes);
        }

        public static string Decrypt(string encryptedText)
        {
            byte[] cipherTextBytes = Convert.FromBase64String(encryptedText);
            byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.None };

            var decryptor = symmetricKey.CreateDecryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));
            var memoryStream = new MemoryStream(cipherTextBytes);
            var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];

            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount).TrimEnd("\0".ToCharArray());
        }
        private string GetToken()
        {
            var userid = User.Identity.GetUserId();
            var tokenCHK = db.TokenGenWApis.Where(a => a.UserId == userid).SingleOrDefault();

            if (tokenCHK == null)
            {
                TempData.Remove("data");
                AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return null;
            }
            else
            {
                return tokenCHK.Token;
            }
        }
        #region Helper
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
        #endregion
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