using System;
using System.Linq;
using System.Web.Mvc;
using System.IO;
using Vastwebmulti.Areas.WRetailer.Models;
using Vastwebmulti.Models;
using Microsoft.AspNet.Identity;
using System.Globalization;
using System.Web.Security;
using Newtonsoft.Json;
using RestSharp;
using Microsoft.Owin.Security;
using System.Web;

namespace Vastwebmulti.Areas.WRetailer.Controllers
{

    [Authorize(Roles = "Whitelabelretailer")]
    public class PANCARDController : Controller
    {
        VastwebmultiEntities db = new VastwebmultiEntities();
        // GET: RETAILER/PANCARD
        string VastbazaarBaseUrl = "http://api.vastbazaar.com/";
        //string VastbazaarBaseUrl = "http://localhost:65209/";

        public ActionResult Index()
        {
            try
            {
                PanLog("------------- Pan Log ----------------");
                var Token = string.Empty;
                Token = GetToken();
                PanLog("Token: " + Token);
                PanLog("Url: " + Vastwebmulti.Areas.WHITELABEL.Models.WapiBaseUrl.GetBaseUrl() + "/PAN/api/PAN/PancardStatusCheck");
                var client = new RestClient(Vastwebmulti.Areas.WHITELABEL.Models.WapiBaseUrl.GetBaseUrl() + "/PAN/api/PAN/PancardStatusCheck");
                var request = new RestRequest(Method.GET);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", "bearer " + Token + "");
                IRestResponse response = client.Execute(request);

                PanLog("Response: " + response.Content);
                var responsechk = response.Content.ToString();
                dynamic stuff = Newtonsoft.Json.JsonConvert.DeserializeObject(responsechk);

                var results = "{'status':'" + stuff.Response + "','msg':'" + stuff.Message + "'}";
                var json1 = JsonConvert.DeserializeObject(results);
                var json = JsonConvert.SerializeObject(json1);
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var results = "{'status':'Failed','msg':'Something went wrong'}";
                var json1 = JsonConvert.DeserializeObject(results);

                var obj = new
                {
                    status = "Failed",
                    msg = ex.Message
                };
                var json = JsonConvert.SerializeObject(obj);
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            
        }

        public static void PanLog(string strMessage)
        {
            try
            {
                StreamWriter log;
                FileStream fileStream = null;
                DirectoryInfo logDirInfo = null;
                FileInfo logFileInfo;
                string logFilePath = "C:\\Logs\\";
                logFilePath = logFilePath + "aashadigital_pan_log-" + DateTime.Today.ToString("MM-dd-yyyy") + "." + "txt";
                logFileInfo = new FileInfo(logFilePath);
                logDirInfo = new DirectoryInfo(logFileInfo.DirectoryName);
                if (!logDirInfo.Exists) logDirInfo.Create();
                if (!logFileInfo.Exists)
                {
                    fileStream = logFileInfo.Create();
                }
                else
                {
                    fileStream = new FileStream(logFilePath, FileMode.Append);
                }
                log = new StreamWriter(fileStream);
                log.WriteLine(strMessage);
                log.Close();

            }
            catch (Exception ex)
            {

            }
        }

        public ActionResult Userinfo()
        {
            try
            {
                var Token = string.Empty;
                Token = GetToken();
                var client = new RestClient(Vastwebmulti.Areas.WHITELABEL.Models.WapiBaseUrl.GetBaseUrl() + "/PAN/api/PAN/PencardRegisterInformation");
                var request = new RestRequest(Method.GET);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", "bearer " + Token + "");
                IRestResponse response = client.Execute(request);

                var responsechk = response.Content.ToString();
                dynamic stuff = Newtonsoft.Json.JsonConvert.DeserializeObject(responsechk);
                var res = stuff.Message.ToString();

                return Json(res, JsonRequestBehavior.AllowGet);

            }
            catch { return Json("", JsonRequestBehavior.AllowGet); }            
        }

        [HttpPost]
        public ActionResult RegisterPSA(string txtpanname, string txtfirmnmpan, string txtemailpan, string panphone, string dobpan, string panpancard, string aadharpan, string txtaddresspan, string pinpan)
        {
            try
            {
                string pathreq = "/PAN/api/PAN/RegisterPSAPancard?txtpanname=" + txtpanname + "&txtfirmnmpan=" + txtfirmnmpan + "&txtemailpan=" + txtemailpan + "&panphone=" + panphone + "&dobpan=" + dobpan + "&panpancard=" + panpancard + "&aadharpan=" + aadharpan + "&txtaddresspan=" + txtaddresspan + "&pinpan=" + pinpan;
                var Token = string.Empty;
                Token = GetToken();
                var client = new RestClient(Vastwebmulti.Areas.WHITELABEL.Models.WapiBaseUrl.GetBaseUrl() + "/PAN/api/PAN/RegisterPSAPancard?txtpanname=" + txtpanname + "&txtfirmnmpan="+ txtfirmnmpan + "&txtemailpan="+ txtemailpan + "&panphone="+ panphone + "&dobpan="+ dobpan + "&panpancard="+ panpancard + "&aadharpan="+ aadharpan + "&txtaddresspan="+ txtaddresspan + "&pinpan="+ pinpan + "");
                var request = new RestRequest(Method.GET);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", "bearer " + Token + "");
                IRestResponse response = client.Execute(request);

                var responsechk = response.Content.ToString();
                dynamic stuff = Newtonsoft.Json.JsonConvert.DeserializeObject(responsechk);
                
                var obj = new { Status = "Failed", Message = stuff.Message };
                return Json(JsonConvert.SerializeObject(obj), JsonRequestBehavior.AllowGet);

            }
            catch {
                var obj = new { Status = "Failed", Message = "Server Error", sts = "registererror" };
                return Json(JsonConvert.SerializeObject(obj), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult CheckStatus()
        {
            try
            {
                var Token = string.Empty;
                Token = GetToken();
                var client = new RestClient(Vastwebmulti.Areas.WHITELABEL.Models.WapiBaseUrl.GetBaseUrl() + "/PAN/api/PAN/PSALiveCheckStatus");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", "bearer " + Token + "");
                IRestResponse response = client.Execute(request);

                var responsechk = response.Content.ToString();
                dynamic stuff = Newtonsoft.Json.JsonConvert.DeserializeObject(responsechk);

                var obj = new { Status = stuff.Message, Message = stuff.Message };
                return Json(JsonConvert.SerializeObject(obj), JsonRequestBehavior.AllowGet);

            }
            catch
            {
                var obj = new { Status = "Failed", Message = "Server Error", sts = "registererror" };
                return Json(JsonConvert.SerializeObject(obj), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult buyUTIToken(string digitalCount, string physicalCount)
        {
            try
            {
                PanLog("-------Buy Uti Token---------");
                var Token = string.Empty;
                Token = GetToken();
                var client = new RestClient(Vastwebmulti.Areas.WHITELABEL.Models.WapiBaseUrl.GetBaseUrl() + "/PAN/api/PAN/Pancardbuy?digitalCount=" + digitalCount + "&physicalCount="+ physicalCount + "");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", "bearer " + Token + "");
                IRestResponse response = client.Execute(request);

                var responsechk = response.Content.ToString();
                PanLog("Url: " + Vastwebmulti.Areas.WHITELABEL.Models.WapiBaseUrl.GetBaseUrl() + "/PAN/api/PAN/Pancardbuy?digitalCount=" + digitalCount + "&physicalCount=" + physicalCount + "");
                PanLog("Response: " + responsechk);
                dynamic stuff = Newtonsoft.Json.JsonConvert.DeserializeObject(responsechk);

                var obj = new { RESULT = stuff.RESULT, ADDINFO = stuff.ADDINFO };
                return Json(JsonConvert.SerializeObject(obj), JsonRequestBehavior.AllowGet);
            }
            catch
            {
                var obj = new { RESULT = "1", ADDINFO = "Failed." };
                return Json(JsonConvert.SerializeObject(obj), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult UpdatePSA()
        {
            try
            {
                var userid = User.Identity.GetUserId();
                var entry = db.Whitelabel_Retailer_Details.SingleOrDefault(a => a.RetailerId == userid);
                Models.RegisterPSAModel model = new Models.RegisterPSAModel();
                model.adhaar = entry.AadharCard ?? "";
                model.contactperson = entry.RetailerName ?? "";
                model.emailid = entry.Email ?? "";
                model.location = entry.city ?? "";
                model.pan = entry.PanCard ?? "";
                model.phone1 = entry.Mobile ?? "";
                model.pincode = entry.Pincode.ToString();
                model.psaname = entry.Frm_Name ?? "";
                model.dob = entry.dateofbirth ?? "";
                model.state = db.State_Desc.Where(y => y.State_id == entry.State).SingleOrDefault().State_name;
                model.psaid = entry.Whitelabel_VastBazaarRetailerOutlets.outlet_id;
                return PartialView(model);
            }
            catch
            {
                throw;
            }
        }
        [HttpPost]
        public ActionResult UpdatePSA(string txtpanname, string txtfirmnmpan, string panpancard, string aadharpan, string txtaddresspan, string pinpan)
        {
            try
            {
                Models.RegisterPSAModel model = new Models.RegisterPSAModel();
                var userid = User.Identity.GetUserId();
                var remdetails = db.Whitelabel_Retailer_Details.Where(aa => aa.RetailerId == userid).SingleOrDefault();
                var statedetails = db.State_Desc.Where(aa => aa.State_id == remdetails.State).SingleOrDefault().State_name;
                model.adhaar = aadharpan;
                model.contactperson = txtpanname;
                model.dob = remdetails.dateofbirth;
                model.emailid = remdetails.Email;
                model.location = remdetails.city;
                model.pan = panpancard;
                model.phone1 = remdetails.Mobile;
                model.pincode = pinpan;

                var entry = db.Whitelabel_VastBazaarRetailerOutlets.Where(a => a.RetailerId == userid).SingleOrDefault();
                if (entry != null)
                {
                    model.psaid = entry.outlet_id;
                    model.psaname = txtfirmnmpan;
                    model.state = statedetails;

                    var token = string.Empty;
                    token = getAuthToken();
                    if (string.IsNullOrWhiteSpace(token))
                    {
                        var obj = new { Status = "Failed", Message = "Server Error", sts = "updaterror" };
                        return Json(JsonConvert.SerializeObject(obj), JsonRequestBehavior.AllowGet);
                    }
                    //var res =  TBO_Utils.GetResponse(JsonConvert.SerializeObject(reqObject), FlightURLS.BaseAddress+ FlightURLS.SearchFlight);

                    var client = new RestClient(VastbazaarBaseUrl + "api/UTI/updateAgent");
                    var request = new RestRequest(Method.POST);
                    request.RequestFormat = DataFormat.Json;
                    request.AddHeader("authorization", "bearer " + token);
                    request.AddHeader("cache-control", "no-cache");
                    request.AddHeader("content-type", "application/json");
                    //request.AddParameter("application/json", "{\r\n\"EndUserIp\": \"192.168.10.10\",\r\n\"TokenId\": \"ac2751e9-4cc3-406f-b678-c947e4f57a00\",\r\n\"AdultCount\": \"1\",\r\n\"ChildCount\": \"0\",\r\n\"InfantCount\": \"0\",\r\n\"DirectFlight\": \"false\",\r\n\"OneStopFlight\": \"false\",\r\n\"JourneyType\": \"1\",\r\n\"PreferredAirlines\": null,\r\n\"Segments\": [\r\n{\r\n\"Origin\": \"DEL\",\r\n\"Destination\": \"BOM\",\r\n        \"FlightCabinClass\": \"1\",\r\n\"PreferredDepartureTime\": \"2018-09-06T00: 00: 00\",\r\n\"PreferredArrivalTime\": \"2018-09-06T00: 00: 00\"\r\n}\r\n        ],\r\n\"Sources\": [\r\n\"6E\"\r\n]\r\n}", ParameterType.RequestBody);
                    model.udf1 = userid;
                    model.udf2 = "";
                    model.udf3 = "";
                    model.udf4 = "";
                    model.udf5 = "";
                    model.phone2 = model.phone2 ?? "";
                    request.AddBody(model);
                    IRestResponse response = client.Execute(request);
                    if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        TryLogin();
                    }
                    dynamic respo = JsonConvert.DeserializeObject(response.Content);
                    if (respo != null && respo.Content.ADDINFO[0].StatusCode == "000")
                    {
                        entry = new Whitelabel_VastBazaarRetailerOutlets();
                        //entry.RetailerId = userid;
                        //entry.CreatedOn = DateTime.Now;
                        //entry.IsKycUploaded = false;
                        //entry.IsPanConfirmed = false;
                        //entry.kyc_status = false;
                        //entry.outlet_id = respo.Content.ADDINFO[0].psaid;
                        //entry.outlet_status = true;
                        //entry.store_type = model.psaname;
                        entry.UpdatedOn = DateTime.Now;
                        db.SaveChanges();
                        var obj = new { Status = "Success", Message = "Updated SuccessFully." };
                        return Json(JsonConvert.SerializeObject(obj), JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var obj = new { Status = "Failed", Message = respo.Content.ADDINFO[0].Message, sts = "updaterror" };
                        return Json(JsonConvert.SerializeObject(obj), JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    var obj = new { Status = "Failed", Message = "Please register first!!!", sts = "updaterror" };
                    return Json(JsonConvert.SerializeObject(obj), JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                var obj = new { Status = "Failed", Message = "Server Error", sts = "updaterror" };
                return Json(JsonConvert.SerializeObject(obj), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult TokenPurchaseReport()
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                var userid = User.Identity.GetUserId();
                string txt_frm_date = DateTime.Now.ToString();
                string txt_to_date = DateTime.Now.ToString();
                string frm_date = Convert.ToDateTime(txt_frm_date).Date.ToString("yyyy-MM-dd");
                string to_date = Convert.ToDateTime(txt_to_date).AddDays(1).ToString("yyyy-MM-dd");
                var ch = db.proc_Whitelabel_PAN_CARD_IPAY_Token_report(userid, "Retailer", 50, "ALL", Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
                ViewData["totals"] = ch.Where(s => s.Particulars.ToUpper().Contains("SUCCESS")).Sum(s => Convert.ToInt32(s.ProcessingFees));
                ViewData["totalp"] = ch.Where(s => s.Particulars.ToUpper().Contains("PENDING")).Sum(s => Convert.ToInt32(s.ProcessingFees));
                ViewData["totalf"] = ch.Where(s => s.Particulars.ToUpper().Contains("FAILED")).Sum(s => Convert.ToInt32(s.ProcessingFees));
                ViewData["Rincome"] = ch.Where(s => s.Particulars.ToUpper().Contains("SUCCESS")).Sum(s => Convert.ToInt32(s.RetailerIncome));
                return View(ch);
            }

        }
        [HttpPost]
        public ActionResult TokenPurchaseReport(string ddl_status, string ddl_top, string txt_frm_date, string txt_to_date)
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
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
                if (ddl_top == "All")
                {
                    ddl_top = "1000000";
                }

                int ddltop = Convert.ToInt32(ddl_top);
                var ch = db.proc_Whitelabel_PAN_CARD_IPAY_Token_report(userid, "Retailer", ddltop, ddl_status, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();

                ViewData["totals"] = ch.Where(s => s.Particulars.ToUpper().Contains("SUCCESS")).Sum(s => Convert.ToInt32(s.ProcessingFees));
                ViewData["totalp"] = ch.Where(s => s.Particulars.ToUpper().Contains("PENDING")).Sum(s => Convert.ToInt32(s.ProcessingFees));
                ViewData["totalf"] = ch.Where(s => s.Particulars.ToUpper().Contains("FAILED")).Sum(s => Convert.ToInt32(s.ProcessingFees));
                ViewData["Rincome"] = ch.Where(s => s.Particulars.ToUpper().Contains("SUCCESS")).Sum(s => Convert.ToInt32(s.RetailerIncome));
                return View(ch);
            }
        }

        [ChildActionOnly]
        public ActionResult _TokenPurchaseReport(string txt_frm_date, string txt_to_date, string ddl_status)
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                if (txt_frm_date == null && txt_to_date == null && ddl_status == null)
                {
                    txt_frm_date = DateTime.Now.ToString();
                    txt_to_date = DateTime.Now.ToString();
                    ddl_status = "ALL";
                }
                if (ddl_status == "Status")
                {
                    ddl_status = "ALL";
                }

                DateTime frm1 = Convert.ToDateTime(txt_frm_date);
                DateTime to1 = Convert.ToDateTime(txt_to_date);

                txt_frm_date = frm1.ToString("dd-MM-yyyy");
                txt_to_date = to1.ToString("dd-MM-yyyy");
                string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
                DateTime dt = DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
                DateTime dt1 = DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
                DateTime frm_date = dt.Date;
                DateTime to_date = dt1.AddDays(1);
                var userid = User.Identity.GetUserId();

                ddl_status = string.IsNullOrWhiteSpace(ddl_status) ? null : ddl_status;
                int pagesize = 20;

                var proc_Response = db.proc_Whitelabel_PAN_CARD_IPAY_Token_report_Paging(userid, "Retailer", ddl_status, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), 1, 20).ToList();

                return View(proc_Response);
            }
        }
        [HttpPost]
        public ActionResult InfiniteScroll(int pageindex, string txt_frm_date, string txt_to_date, string ddl_status)
        {
            string userid = User.Identity.GetUserId();
            System.Threading.Thread.Sleep(1000);
            int pagesize = 20;
            if (ddl_status == "Status")
            {
                ddl_status = "ALL";
            }

            var tbrow = db.proc_Whitelabel_PAN_CARD_IPAY_Token_report_Paging(userid, "Retailer", ddl_status, Convert.ToDateTime(txt_frm_date), Convert.ToDateTime(txt_to_date), pageindex, 20).ToList();
            JsonModel jsonmodel = new JsonModel();
            jsonmodel.NoMoredata = tbrow.Count < pagesize;
            jsonmodel.HTMLString = renderPartialViewtostring("_TokenPurchaseReport", tbrow);
            return Json(jsonmodel);
        }
        public class JsonModel
        {
            public string HTMLString { get; set; }
            public bool NoMoredata { get; set; }
        }
        protected string renderPartialViewtostring(string Viewname, object model)
        {
            if (string.IsNullOrEmpty(Viewname))

                Viewname = ControllerContext.RouteData.GetRequiredString("action");
            ViewData.Model = model;
            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewresult = ViewEngines.Engines.FindPartialView(ControllerContext, Viewname);
                ViewContext viewcontext = new ViewContext(ControllerContext, viewresult.View, ViewData, TempData, sw);
                viewresult.View.Render(viewcontext, sw);
                return sw.GetStringBuilder().ToString();
            }
        }

        [HttpPost]
        public ActionResult GetTokenStatus(string id)
        {
            InstantPayComnUtil util = new InstantPayComnUtil();
            var userid = User.Identity.GetUserId();
            var token = string.Empty;
            token = getAuthToken();
            if (string.IsNullOrWhiteSpace(token))
            {
                var obj = new { RESULT = "1", ADDINFO = "Authentical Failed." };
                return Json(JsonConvert.SerializeObject(obj), JsonRequestBehavior.AllowGet);
            }
            var response = util.getUtiTokenStatus(id, token);
            return Json(response.ToString());
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account", new { area = "" });
        }
        public IRestResponse tokencheck()
        {
            var apidetails = db.Money_API_URLS.Where(aa => aa.API_Name == "VASTWEB").SingleOrDefault();
            var token = apidetails == null ? "" : apidetails.Token;
            var apiid = apidetails == null ? "" : apidetails.API_ID;
            var apiidpwd = apidetails == null ? "" : apidetails.Api_pwd;

            var client = new RestClient(VastbazaarBaseUrl + "token");
            var request = new RestRequest(Method.POST);
            request.AddHeader("iptoken", token);
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("application/x-www-form-urlencoded", "UserName=" + apiid + "&Password=" + apiidpwd + "&grant_type=password", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            return response;
        }
        public string getAuthToken()
        {
            try
            {
                var tokn = db.vastbazzartokens.SingleOrDefault();
                if (tokn == null)
                {
                    var response = tokencheck();
                    var responsechk = response.Content.ToString();
                    var responsecode = response.StatusCode.ToString();
                    if (responsecode == "OK")
                    {
                        Models.Vastbillpay vb = new Models.Vastbillpay();
                        dynamic json = JsonConvert.DeserializeObject(responsechk);
                        var token = json.access_token.ToString();
                        var expire = json[".expires"].ToString();
                        DateTime exp = Convert.ToDateTime(expire);
                        vastbazzartoken vast = new vastbazzartoken();
                        vast.apitoken = token;
                        vast.exptime = exp;
                        db.vastbazzartokens.Add(vast);
                        db.SaveChanges();
                        return tokn.apitoken;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    DateTime curntdate = DateTime.Now.Date;
                    DateTime expdate = Convert.ToDateTime(tokn.exptime).Date;
                    if (expdate > curntdate)
                    {
                        return tokn.apitoken;
                    }
                    else
                    {
                        var response = tokencheck();
                        var responsechk = response.Content.ToString();
                        var responsecode = response.StatusCode.ToString();
                        if (responsecode == "OK")
                        {

                            dynamic json = JsonConvert.DeserializeObject(responsechk);
                            var token = json.access_token.ToString();
                            var expire = json[".expires"].ToString();
                            DateTime exp = Convert.ToDateTime(expire);

                            tokn.apitoken = token;
                            tokn.exptime = exp;
                            db.SaveChanges();
                            return token;
                        }
                        else
                        {
                            return null;
                        }
                    }

                }
            }
            catch
            {
                return null;
            }
        }

        public void TryLogin()
        {
            var response = tokencheck();
            var responsechk = response.Content.ToString();
            var responsecode = response.StatusCode.ToString();
            if (responsecode == "OK")
            {
                var tokn = db.vastbazzartokens.SingleOrDefault();
                dynamic json = JsonConvert.DeserializeObject(responsechk);
                var token = json.access_token.ToString();
                var expire = json[".expires"].ToString();
                DateTime exp = Convert.ToDateTime(expire);

                tokn.apitoken = token;
                tokn.exptime = exp;
                db.SaveChanges();

            }
        }
        private string GetToken()
        {
            try
            {
                var userid = User.Identity.GetUserId();
                //var localSystemToken = "3pQzlVFnyYyV8Owj3pFuVV09UONr1q4WLr8nifXuWdZxzw3hYvrULQcSF2yKOV8ND-XVJDTZ7ssph6Ollnqcx8BpWXnDCMGU3lF9AWpu7Lvaa9A0eqUOc2ElG8uBLGCyyHnhAEq5jdeQmHijEhlw7zO3vPpKqVKmr6FZRVyTSzYsnEgCIenMbPNBxO42P5l0AUoQM3hHusAG2zqLOOc0ZE_Vqt5dHiMRSp0Efkfp4rVb__ZFkjlvYlnIPLxST7ifUCBd_3M6ZWgN78ZQGfXYUdnTy3S3wXM0wgIFFR2xK_lbxVC909RBeO1o9RTWTPxvCE5gwy1z9zY6QOHM1ly_GoR7hF2_u6tGUxVX0jXMDeTNm1oRpuQtxmI2bLVS8D874f-H03MqgQ1XFqcB4R8DMhCxNnWqmovxIqPvUOtqIMseg4iMSvLg1bppdo8LysuvLK_lAHlQZMJgTYSNyFuUV13bFjvEwi83D1FQj8ll2I-f68Pr7a7GkaYFEmP4jdeGSn7E7-hEAfu8njiIigkt7IlKuZfV6c2xs_qWKT7qiO9equneVRODf02V3R0UU3kv";
                string tokenCHK = db.TokenGenWApis.Where(a => a.UserId == userid).SingleOrDefault() != null ? db.TokenGenWApis.Where(a => a.UserId == userid).SingleOrDefault().Token : null;
                //try { tokenCHK = System.Web.HttpContext.Current.Request.IsLocal ? localSystemToken : db.TokenGenWApis.Where(a => a.UserId == userid).SingleOrDefault().Token; } catch { tokenCHK = null; }

                if (tokenCHK == null)
                {
                    TempData.Remove("data");
                    AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                    return null;
                }
                else
                {
                    return tokenCHK;
                }
            }
            catch
            {
                TempData.Remove("data");
                AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return null;
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
    }
}