using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using RestSharp;
using Rotativa;
using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.RETAILER.Controllers
{

    /// <summary>
    /// RETAILER Area - Handles PAN card application, status tracking and document upload for retailers
    /// </summary>
    [Authorize(Roles = "Retailer")]
    [Low_Bal_CustomFilter()]
    public class PANCARDController : Controller
    {
        VastwebmultiEntities db = new VastwebmultiEntities();
        // GET: RETAILER/PANCARD
        string VastbazaarBaseUrl = "http://api.vastbazaar.com/";
        //string VastbazaarBaseUrl = "http://localhost:65209/";

        /// <summary>
        /// GET - Main listing/management page
        /// </summary>
        public ActionResult Index()
        {
            var status = ""; var message = "";
            var userid = User.Identity.GetUserId();
            var remdetails = db.Retailer_Details.FirstOrDefault(aa => aa.RetailerId == userid);
            if (remdetails.pancardsts == "Y")
            {
                var dealerdetails = db.Dealer_Details.FirstOrDefault(aa => aa.DealerId == remdetails.DealerId);
                if (dealerdetails.pancardsts == "Y")
                {
                    var check = "OK";

                    var checkfreeservice = db.PaidServicesChargeLists.Where(aa => aa.ServiceName == "PANCARD").SingleOrDefault();
                    var checkALLfreeservice = db.PaidServicesChargeLists.Where(aa => aa.ServiceName == "ALL").SingleOrDefault();
                    if (checkfreeservice.IsFree == true)
                    {
                        if (checkALLfreeservice.IsFree == false)
                        {

                            var retailerautorenseting = db.autopaidserviceRenewalsettings.Where(x => x.retailerid == userid).SingleOrDefault().auto_set;
                            if (retailerautorenseting == "ALL" || retailerautorenseting == "PER")
                            {
                                var chkklatestdate = db.PaidServicesPaymentHistories.Where(aa => aa.UserId == userid && aa.ServiceName == "PANCARD").OrderByDescending(aa => aa.PurchaseDate).FirstOrDefault();
                                if (chkklatestdate != null)
                                {
                                    var expiredate = chkklatestdate.ExpiryDate.Date.AddDays(-1);
                                    if (expiredate == DateTime.Now.Date)
                                    {
                                        var chkadminperservice = db.PaidServicesChargeLists.Where(aa => aa.ServiceName == "PANCARD" && aa.IsFree == false).SingleOrDefault();
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
                            var servicecheck = db.PaidServicesPaymentHistories.Where(aa => aa.UserId == userid && aa.ServiceName == "PANCARD").OrderByDescending(aa => aa.PurchaseDate).FirstOrDefault();
                            if (servicecheck != null)
                            {

                                var expdate = servicecheck.ExpiryDate;
                                var currentdate = DateTime.Now;
                                if (expdate <= currentdate)
                                {
                                    status = "ALLNOTDONE";
                                    check = "NOTOK";
                                    message = "Pan Card Service is Expired.";
                                }
                            }
                            else
                            {
                                status = "ALLNOTDONE";
                                check = "NOTOK";
                                message = "Firstlly Purchase this Service.";
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
                                var chkklatestdate = db.PaidServicesPaymentHistories.Where(aa => aa.UserId == userid && aa.ServiceName == "PANCARD").OrderByDescending(aa => aa.PurchaseDate).FirstOrDefault();
                                if (chkklatestdate != null)
                                {
                                    var expiredate = chkklatestdate.ExpiryDate.Date.AddDays(-1);
                                    if (expiredate == DateTime.Now.Date)
                                    {
                                        var chkadminperservice = db.PaidServicesChargeLists.Where(aa => aa.ServiceName == "PANCARD" && aa.IsFree == false).SingleOrDefault();
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

                            var servicecheck = db.PaidServicesPaymentHistories.Where(aa => aa.UserId == userid && aa.ServiceName == "PANCARD").OrderByDescending(aa => aa.PurchaseDate).FirstOrDefault();
                            if (servicecheck != null)
                            {

                                var expdate = servicecheck.ExpiryDate;
                                var currentdate = DateTime.Now;
                                if (expdate <= currentdate)
                                {
                                    status = "BOTHNOTDONE";
                                    check = "NOTOK";
                                    message = "Pan Card Service is Expired.";
                                }
                            }
                            else
                            {
                                status = "BOTHNOTDONE";
                                check = "NOTOK";
                                message = "Firstlly Purchase this Service.";
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
                                var chkklatestdate = db.PaidServicesPaymentHistories.Where(aa => aa.UserId == userid && aa.ServiceName == "PANCARD").OrderByDescending(aa => aa.PurchaseDate).FirstOrDefault();
                                if (chkklatestdate != null)
                                {
                                    var expiredate = chkklatestdate.ExpiryDate.Date.AddDays(-1);
                                    if (expiredate >= DateTime.Now.Date)
                                    {
                                        var chkadminperservice = db.PaidServicesChargeLists.Where(aa => aa.ServiceName == "PANCARD" && aa.IsFree == false).SingleOrDefault();
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
                                    else
                                    {
                                        status = "NOTOK";
                                        check = "NOTOK";
                                        message = "Pan Card Service is Expired.";
                                    }

                                }
                                else
                                {
                                    status = "NOTOK";
                                    check = "NOTOK";
                                    message = "Firstlly Purchase this Service.";
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
                            var servicecheck = db.PaidServicesPaymentHistories.Where(aa => aa.UserId == userid && aa.ServiceName == "PANCARD").OrderByDescending(aa => aa.PurchaseDate).FirstOrDefault();
                            if (servicecheck != null)
                            {

                                var expdate = servicecheck.ExpiryDate;
                                var currentdate = DateTime.Now;
                                if (expdate <= currentdate)
                                {
                                    status = "NOTOK";
                                    check = "NOTOK";
                                    message = "Pan Card Service is Expired.";
                                }
                            }
                            else
                            {
                                status = "NOTOK";
                                check = "NOTOK";
                                message = "Firstlly Purchase this Service.";
                            }
                        }
                    }






                    //var checkfreeservice = db.PaidServicesChargeLists.Where(aa => aa.ServiceName == "PANCARD").SingleOrDefault();
                    //if (checkfreeservice.IsFree == false)
                    //{
                    //    var chk = db.PaidService_auto.Where(aa => aa.Userid == userid && aa.ServiceName == "PANCARD").SingleOrDefault().AutoSts;
                    //    if (chk == "Y")
                    //    {
                    //        System.Data.Entity.Core.Objects.ObjectParameter Status = new System.Data.Entity.Core.Objects.ObjectParameter("Status", typeof(string));
                    //        System.Data.Entity.Core.Objects.ObjectParameter Message = new System.Data.Entity.Core.Objects.ObjectParameter("Message", typeof(string));
                    //        int serviceid = db.PaidServicesChargeLists.Where(aa => aa.ServiceName == "PANCARD").SingleOrDefault().Idno;
                    //        var msg = db.proc_PurchasePaidServices(userid, serviceid, Status, Message).SingleOrDefault();
                    //    }
                    //    var servicecheck = db.PaidServicesPaymentHistories.Where(aa => aa.UserId == userid && aa.ServiceName == "PANCARD").OrderByDescending(aa => aa.PurchaseDate).FirstOrDefault();
                    //    if (servicecheck != null)
                    //    {
                    //        var expdate = servicecheck.ExpiryDate;
                    //        var currentdate = DateTime.Now;
                    //        if (expdate <= currentdate)
                    //        {
                    //            status = "NOTOK";
                    //            check = "NOTOK";
                    //            message = "Pan Card Service is Expired.";
                    //        }
                    //    }
                    //    else
                    //    {
                    //        status = "NOTOK";
                    //        check = "NOTOK";
                    //        message = "Firstlly Purchase this Service.";
                    //    }
                    //}
                    if (check == "OK")
                    {
                        if (remdetails.PSAStatus == "Y" && remdetails.AadhaarStatus == "Y" && remdetails.ShopwithSalfieStatus == "Y")
                        {
                            string str = string.Empty;
                            bool IsUpdateRequired = false;
                            if (remdetails.AadharCard == null || remdetails.AadharCard.Length < 12)
                            {
                                str = str + "Aadhar,";
                                IsUpdateRequired = true;
                            }
                            if (remdetails.PanCard == null || remdetails.PanCard.Length < 10)
                            {
                                str = str + "Pancard,";
                                IsUpdateRequired = true;
                            }
                            if (remdetails.Frm_Name == null || string.IsNullOrWhiteSpace(remdetails.Frm_Name))
                            {
                                str = str + "Firm name,";
                                IsUpdateRequired = true;
                            }
                            if (remdetails.Pincode == 0 || remdetails.Pincode.ToString().Length < 6)
                            {
                                str = str + "Pincode,";
                                IsUpdateRequired = true;
                            }
                            if (remdetails.dateofbirth == null)
                            {
                                str = str + "Date of birth";
                                IsUpdateRequired = true;
                            }
                            if (IsUpdateRequired)
                            {
                                if (str.EndsWith(","))
                                    str = str.Substring(0, str.Length - 1);
                                str = str + " are required to become PSA.";
                                status = "NOTKYC";
                                message = str;
                            }
                            else
                            {
                                var entry = db.VastBazaarRetailerOutlets.SingleOrDefault(a => a.RetailerId == userid && a.IsPanConfirmed == true);
                                if (entry != null)
                                {
                                    if (entry.status == "Approved")
                                    {
                                        status = "Registered";
                                        message = entry.outlet_id;
                                    }
                                    else
                                    {
                                        status = "PENDING";
                                        message = entry.outlet_id;
                                    }
                                }
                                else
                                {
                                    status = "NOTRegistered";
                                    message = "NOT REGISTER";
                                }
                            }
                        }
                        else
                        {
                            status = "NOTKYC";
                            message = "KYC Required";
                        }
                    }

                }
                else
                {
                    status = "FAILED";
                    message = "Pan Card Status is Off Contact Your Distributor.";
                    //dealerpan status
                }
            }
            else
            {
                status = "FAILED";
                message = "Pan Card Status is Off Contact Your Admin.";
                //retailer pan status
            }
            var results = "{'status':'" + status + "','msg':'" + message + "'}";
            var json1 = JsonConvert.DeserializeObject(results);
            var json = JsonConvert.SerializeObject(json1);
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Userinfo()
        {
            var userid = User.Identity.GetUserId();

            //var k=from rem in db.Retailer_Details 
            //      join st in db.State_Desc on rem.State equals st.State_id
            //      join dl in db.District_Desc on rem.District equals dl.Dist_id 

            var q = (from rem in db.Retailer_Details
                     select new
                     {
                         Retailerid = rem.RetailerId,
                         RetailerName = rem.RetailerName,
                         Frm_Name = rem.Frm_Name,
                         Email = rem.Email,
                         Mobile = rem.Mobile,
                         dateofbirth = rem.dateofbirth,
                         PanCard = rem.PanCard,
                         AadharCard = rem.AadharCard,
                         //State = st.State_name,
                         //District = di.Dist_Desc,
                         Address = rem.city,
                         Pincode = rem.Pincode
                     }).Where(aa => aa.Retailerid == userid).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult RegisterPSA()
        {
            var userid = User.Identity.GetUserId();
            var entry = db.Retailer_Details.SingleOrDefault(a => a.RetailerId == userid);
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
            return PartialView(model);
        }
        [HttpPost]
        public ActionResult RegisterPSA(string txtpanname, string txtfirmnmpan, string txtemailpan, string panphone, string dobpan, string panpancard, string aadharpan, string txtaddresspan, string pinpan)
        {
            try
            {
                var userid = User.Identity.GetUserId();
                var remdetails = db.Retailer_Details.FirstOrDefault(aa => aa.RetailerId == userid);
                var statenm = db.State_Desc.Where(aa => aa.State_id == remdetails.State).SingleOrDefault().State_name;
                var distnm = db.District_Desc.Where(aa => aa.State_id == remdetails.State && aa.Dist_id == remdetails.District).SingleOrDefault().Dist_Desc;

                Models.RegisterPSAModel model = new Models.RegisterPSAModel();
                model.psaname = txtfirmnmpan;
                model.location = distnm;
                model.phone1 = panphone;
                model.emailid = txtemailpan;
                model.pan = panpancard;
                model.pincode = pinpan;


                //var entry = db.RetailerOutlets.SingleOrDefault(a => a.RetailerId == userid);
                var entry = db.VastBazaarRetailerOutlets.SingleOrDefault(a => a.RetailerId == userid && a.IsPanConfirmed == true);
                if (entry == null)
                {
                    var token = string.Empty;
                    token = getAuthToken();
                    if (string.IsNullOrWhiteSpace(token))
                    {
                        var obj = new { Status = "Failed", Message = "Server Error" };
                        return Json(JsonConvert.SerializeObject(obj), JsonRequestBehavior.AllowGet);
                    }
                    //var res =  TBO_Utils.GetResponse(JsonConvert.SerializeObject(reqObject), FlightURLS.BaseAddress+ FlightURLS.SearchFlight);
                    var client = new RestClient(VastbazaarBaseUrl + "api/UTI/registerPSA_New");
                    var request = new RestRequest(Method.POST);
                    request.RequestFormat = DataFormat.Json;
                    request.AddHeader("authorization", "bearer " + token);
                    request.AddHeader("cache-control", "no-cache");
                    request.AddHeader("content-type", "application/json");
                    request.AddBody(model);
                    IRestResponse response = client.Execute(request);
                    if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        TryLogin();
                    }

                    dynamic respo = JsonConvert.DeserializeObject(response.Content);
                    string sts = respo.Content.ADDINFO[0].Txnstatus;
                    if (respo != null && sts.ToUpper() == "SUCCESS")
                    {
                        VastBazaarRetailerOutlet entry1 = new VastBazaarRetailerOutlet();
                        entry1.RetailerId = userid;
                        entry1.CreatedOn = DateTime.Now;
                        entry1.IsKycUploaded = false;
                        entry1.kyc_status = false;
                        entry1.outlet_id = respo.Content.ADDINFO[0].Psaid;
                        entry1.UTIRequestId = "123456";
                        entry1.outlet_status = true;
                        entry1.store_type = model.psaname;
                        entry1.UpdatedOn = DateTime.Now;
                        entry1.IsPanConfirmed = true;
                        entry1.status = respo.Content.ADDINFO[0].Txnstatus;
                        db.VastBazaarRetailerOutlets.Add(entry1);
                        db.SaveChanges();
                        var obj = new { Status = "Success", Message = "Request processed SuccessFully." };
                        return Json(JsonConvert.SerializeObject(obj), JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var obj = new { Status = "Failed", Message = respo.Content.ADDINFO[0].Msg, sts = "registererror" };
                        return Json(JsonConvert.SerializeObject(obj), JsonRequestBehavior.AllowGet);
                    }
                }
                else if (entry != null && entry.status == "Rejected")
                {
                    var token = string.Empty;
                    token = getAuthToken();
                    if (string.IsNullOrWhiteSpace(token))
                    {
                        var obj = new { Status = "Failed", Message = "Server Error" };
                        return Json(JsonConvert.SerializeObject(obj), JsonRequestBehavior.AllowGet);
                    }
                    //var res =  TBO_Utils.GetResponse(JsonConvert.SerializeObject(reqObject), FlightURLS.BaseAddress+ FlightURLS.SearchFlight);
                    var client = new RestClient(VastbazaarBaseUrl + "api/UTI/registerPSA_New");
                    var request = new RestRequest(Method.POST);
                    request.RequestFormat = DataFormat.Json;
                    request.AddHeader("authorization", "bearer " + token);
                    request.AddHeader("cache-control", "no-cache");
                    request.AddHeader("content-type", "application/json");
                    //request.AddParameter("application/json", "{\r\n\"EndUserIp\": \"192.168.10.10\",\r\n\"TokenId\": \"ac2751e9-4cc3-406f-b678-c947e4f57a00\",\r\n\"AdultCount\": \"1\",\r\n\"ChildCount\": \"0\",\r\n\"InfantCount\": \"0\",\r\n\"DirectFlight\": \"false\",\r\n\"OneStopFlight\": \"false\",\r\n\"JourneyType\": \"1\",\r\n\"PreferredAirlines\": null,\r\n\"Segments\": [\r\n{\r\n\"Origin\": \"DEL\",\r\n\"Destination\": \"BOM\",\r\n        \"FlightCabinClass\": \"1\",\r\n\"PreferredDepartureTime\": \"2018-09-06T00: 00: 00\",\r\n\"PreferredArrivalTime\": \"2018-09-06T00: 00: 00\"\r\n}\r\n        ],\r\n\"Sources\": [\r\n\"6E\"\r\n]\r\n}", ParameterType.RequestBody);
                    model.udf2 = "";
                    model.udf3 = "";
                    model.udf4 = "";
                    model.udf5 = "";
                    request.AddBody(model);
                    IRestResponse response = client.Execute(request);
                    if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        TryLogin();
                    }
                    dynamic respo = JsonConvert.DeserializeObject(response.Content);
                    string sts = respo.Content.ADDINFO[0].Txnstatus;
                    if (respo != null && sts.ToUpper() == "SUCCESS")
                    {
                        VastBazaarRetailerOutlet entry1 = new VastBazaarRetailerOutlet();
                        entry1.RetailerId = userid;
                        entry1.CreatedOn = DateTime.Now;
                        entry1.IsKycUploaded = false;
                        entry1.kyc_status = false;
                        entry1.outlet_id = respo.Content.ADDINFO[0].Psaid;
                        entry1.UTIRequestId = "123456";
                        entry1.outlet_status = true;
                        entry1.store_type = model.psaname;
                        entry1.UpdatedOn = DateTime.Now;
                        entry1.IsPanConfirmed = true;
                        entry1.status = respo.Content.ADDINFO[0].Txnstatus;
                        db.VastBazaarRetailerOutlets.Add(entry1);
                        db.SaveChanges();
                        var obj = new { Status = "Success", Message = "Request processed SuccessFully." };
                        return Json(JsonConvert.SerializeObject(obj), JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var obj = new { Status = "Failed", Message = respo.Content.ADDINFO[0].Msg, sts = "registererror" };
                        return Json(JsonConvert.SerializeObject(obj), JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    var obj = new { Status = "Failed", Message = "Allready registered,Your Agent Id is " + entry.outlet_id, sts = "registererror" };
                    return Json(JsonConvert.SerializeObject(obj), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                var obj = new { Status = "Failed", Message = "Server Error", sts = "registererror" };
                return Json(JsonConvert.SerializeObject(obj), JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// POST - Update or check status
        /// </summary>
        public ActionResult CheckStatus()
        {
            var userid = User.Identity.GetUserId();
            var UTIRequestId = db.VastBazaarRetailerOutlets.Where(a => a.RetailerId == userid && a.IsPanConfirmed == true).SingleOrDefault().outlet_id;
            var RequestObject = new
            {
                requestid = UTIRequestId,
            };
            var token = string.Empty;
            token = getAuthToken();
            if (string.IsNullOrWhiteSpace(token))
            {
                var obj = new { Status = "Failed", Message = "Server Error" };
                return Json(JsonConvert.SerializeObject(obj), JsonRequestBehavior.AllowGet);
            }
            var client = new RestClient(VastbazaarBaseUrl + "api/UTI/AgentStatus_New");
            var request = new RestRequest(Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("authorization", "bearer " + token);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/json");
            request.AddBody(RequestObject);
            IRestResponse response = client.Execute(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                dynamic respo = JsonConvert.DeserializeObject(response.Content);
                var message = "";
                string statuscode = respo.Content.ADDINFO[0].Status;
                VastBazaarRetailerOutlet entry = (from pp in db.VastBazaarRetailerOutlets
                                                  where pp.RetailerId == userid
                                                  && pp.IsPanConfirmed == true
                                                  select pp).FirstOrDefault();
                if (statuscode.ToUpper() == "REJECTED" || statuscode.ToUpper() == "BLOCKED")
                {
                    entry.status = "Rejected";
                    db.SaveChanges();
                    var remarks = "";
                    var obj = new { Status = "Failed", Message = message, Remark = remarks };
                    return Json(JsonConvert.SerializeObject(obj), JsonRequestBehavior.AllowGet);
                }
                else if (statuscode.ToUpper() == "APPROVED")
                {
                    entry.status = "Approved";
                    db.SaveChanges();
                    var obj = new { Status = "Success", Message = "Your PSA request has been accepted." };
                    return Json(JsonConvert.SerializeObject(obj), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var obj = new { Status = "Warning", Message = "Your PSA request is in process." };
                    return Json(JsonConvert.SerializeObject(obj), JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                var obj = new { Status = "Failed", Message = "Failed at provider server.", Remark = "" };
                return Json(JsonConvert.SerializeObject(obj), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        /// <summary>
        /// GET - View transaction or activity report
        /// </summary>
        public ActionResult TokenPurchaseReport()
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                var userid = User.Identity.GetUserId();
                string txt_frm_date = DateTime.Now.ToString();
                string txt_to_date = DateTime.Now.ToString();
                string frm_date = Convert.ToDateTime(txt_frm_date).Date.ToString("yyyy-MM-dd");
                string to_date = Convert.ToDateTime(txt_to_date).AddDays(1).ToString("yyyy-MM-dd");
                var ch = db.proc_PAN_CARD_IPAY_Token_report(userid, "Retailer", 50, "ALL", Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
                ViewData["totals"] = ch.Where(s => s.Particulars.ToUpper().Contains("SUCCESS")).Sum(s => Convert.ToInt32(s.ProcessingFees));
                ViewData["totalp"] = ch.Where(s => s.Particulars.ToUpper().Contains("PENDING")).Sum(s => Convert.ToInt32(s.ProcessingFees));
                ViewData["totalf"] = ch.Where(s => s.Particulars.ToUpper().Contains("FAILED")).Sum(s => Convert.ToInt32(s.ProcessingFees));
                ViewData["Rincome"] = ch.Where(s => s.Particulars.ToUpper().Contains("SUCCESS")).Sum(s => Convert.ToInt32(s.RetailerIncome));
                return View(ch);
            }

        }
        [HttpPost]
        /// <summary>
        /// GET - View transaction or activity report
        /// </summary>
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
                var ch = db.proc_PAN_CARD_IPAY_Token_report(userid, "Retailer", ddltop, ddl_status, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();

                ViewData["totals"] = ch.Where(s => s.Particulars.ToUpper().Contains("SUCCESS")).Sum(s => Convert.ToInt32(s.ProcessingFees));
                ViewData["totalp"] = ch.Where(s => s.Particulars.ToUpper().Contains("PENDING")).Sum(s => Convert.ToInt32(s.ProcessingFees));
                ViewData["totalf"] = ch.Where(s => s.Particulars.ToUpper().Contains("FAILED")).Sum(s => Convert.ToInt32(s.ProcessingFees));
                ViewData["Rincome"] = ch.Where(s => s.Particulars.ToUpper().Contains("SUCCESS")).Sum(s => Convert.ToInt32(s.RetailerIncome));
                return View(ch);
            }
        }

        [ChildActionOnly]
        /// <summary>
        /// GET - View transaction or activity report
        /// </summary>
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
                ViewBag.checkdata = null;
                var checkdata = db.PAN_CARD_IPAY.FirstOrDefault();
                if (checkdata != null)
                {
                    ViewBag.checkdata = "Data";
                }
                int pagesize = 20;

                var proc_Response = db.PAN_CARD_IPAY_Token_report_paging(1, pagesize, userid, "Retailer", ddl_status, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();

                return View(proc_Response);
            }
        }


        /// <summary>
        /// GET - View transaction or activity report
        /// </summary>
        public ActionResult TokenPurchaseReport_new()
        {
            var userid = User.Identity.GetUserId();
            string txt_frm_date = DateTime.Now.ToString();
            string txt_to_date = DateTime.Now.ToString();
            string frm_date = Convert.ToDateTime(txt_frm_date).Date.ToString("yyyy-MM-dd");
            string to_date = Convert.ToDateTime(txt_to_date).AddDays(1).ToString("yyyy-MM-dd");
            var frm_date1 = Convert.ToDateTime(frm_date);
            var ch = db.pancard_transation.Where(a => a.Reailerid == userid && a.request_time > frm_date1).OrderByDescending(s => s.idno).ToList();

            return View(ch);
        }
        [HttpPost]
        /// <summary>
        /// GET - View transaction or activity report
        /// </summary>
        public ActionResult TokenPurchaseReport_new(string ddl_status, string txt_frm_date, string txt_to_date)
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
            var frm_date1 = Convert.ToDateTime(frm_date);
            var to_date1 = Convert.ToDateTime(to_date);

            if(ddl_status == "ALL")
            {
                var ch = db.pancard_transation.Where(a => a.Reailerid == userid && a.request_time > frm_date1 && a.request_time < to_date1).OrderByDescending(s => s.idno).ToList();
                return View(ch);
            }
            else
            {
                var ch = db.pancard_transation.Where(a => a.Reailerid == userid && a.request_time > frm_date1 && a.request_time < to_date1 && a.status.ToUpper() == ddl_status).OrderByDescending(s => s.idno).ToList();
                return View(ch);
            }

            
        }


        /// <summary>
        /// GET - View transaction or activity report
        /// </summary>
        public ActionResult TokenPurchaseReport_new_manual()
        {
            var userid = User.Identity.GetUserId();
            string txt_frm_date = DateTime.Now.ToString();
            string txt_to_date = DateTime.Now.ToString();
            string frm_date = Convert.ToDateTime(txt_frm_date).Date.ToString("yyyy-MM-dd");
            string to_date = Convert.ToDateTime(txt_to_date).AddDays(1).ToString("yyyy-MM-dd");
            var frm_date1 = Convert.ToDateTime(frm_date);
            var ch = db.pancard_transation_manual.Where(a => a.Reailerid == userid && a.request_time > frm_date1).OrderByDescending(s => s.idno).ToList();

            return View(ch);
        }
        [HttpPost]
        /// <summary>
        /// GET - View transaction or activity report
        /// </summary>
        public ActionResult TokenPurchaseReport_new_manual(string ddl_status, string txt_frm_date, string txt_to_date)
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
            var frm_date1 = Convert.ToDateTime(frm_date);
            var to_date1 = Convert.ToDateTime(to_date);

            if (ddl_status == "ALL")
            {
                var ch = db.pancard_transation_manual.Where(a => a.Reailerid == userid && a.request_time > frm_date1 && a.request_time < to_date1).OrderByDescending(s => s.idno).ToList();
                return View(ch);
            }
            else
            {
                var ch = db.pancard_transation_manual.Where(a => a.Reailerid == userid && a.request_time > frm_date1 && a.request_time < to_date1 && a.status.ToUpper() == ddl_status).OrderByDescending(s => s.idno).ToList();
                return View(ch);
            }


        }
        /// <summary>
        /// GET - View transaction or activity report
        /// </summary>
        public ActionResult PDF_TokenPurchaseReport(string txt_frm_date, string txt_to_date, string ddl_status)
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
                int pagesize = 2000;

                var proc_Response = db.PAN_CARD_IPAY_Token_report_paging(1, pagesize, userid, "Retailer", ddl_status, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();

                return new ViewAsPdf(proc_Response);
            }
        }

        /// <summary>
        /// GET - View transaction or activity report
        /// </summary>
        public ActionResult PDF_TokenPurchaseReport1(string txt_frm_date, string txt_to_date, string ddl_status)
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
            var frm_date1 = Convert.ToDateTime(frm_date);
            var to_date1 = Convert.ToDateTime(to_date);

            if (ddl_status == "ALL")
            {
                var ch = db.pancard_transation.Where(a => a.Reailerid == userid && a.request_time > frm_date1 && a.request_time < to_date1).OrderByDescending(s => s.idno).ToList();
                return new ViewAsPdf(ch);
            }
            else
            {
                var ch = db.pancard_transation.Where(a => a.Reailerid == userid && a.request_time > frm_date1 && a.request_time < to_date1 && a.status.ToUpper() == ddl_status).OrderByDescending(s => s.idno).ToList();
                return new ViewAsPdf(ch);
            }



        }

        /// <summary>
        /// GET - View transaction or activity report
        /// </summary>
        public ActionResult PDF_TokenPurchaseReport1_manual(string txt_frm_date, string txt_to_date, string ddl_status)
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
            var frm_date1 = Convert.ToDateTime(frm_date);
            var to_date1 = Convert.ToDateTime(to_date);

            if (ddl_status == "ALL")
            {
                var ch = db.pancard_transation_manual.Where(a => a.Reailerid == userid && a.request_time > frm_date1 && a.request_time < to_date1).OrderByDescending(s => s.idno).ToList();
                return new ViewAsPdf(ch);
            }
            else
            {
                var ch = db.pancard_transation_manual.Where(a => a.Reailerid == userid && a.request_time > frm_date1 && a.request_time < to_date1 && a.status.ToUpper() == ddl_status).OrderByDescending(s => s.idno).ToList();
                return new ViewAsPdf(ch);
            }



        }
        /// <summary>
        /// GET - View transaction or activity report
        /// </summary>
        public ActionResult Excel_TokenPurchase_Report(string txt_frm_date, string txt_to_date, string ddl_status)
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {

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
                int pagesize = 2000;

                var proc_Response = db.PAN_CARD_IPAY_Token_report_paging(1, pagesize, userid, "Retailer", ddl_status, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date)).ToList();
                DataTable dataTbl = new DataTable();
                dataTbl.Columns.Add("Live Status", typeof(string));
                dataTbl.Columns.Add("Processing Fees", typeof(string));
                dataTbl.Columns.Add("Physical", typeof(string));
                dataTbl.Columns.Add("Digital", typeof(string));
                dataTbl.Columns.Add("UTI TXN ID ", typeof(string));
                dataTbl.Columns.Add("UTI Status ", typeof(string));
                dataTbl.Columns.Add("Date", typeof(string));
                dataTbl.Columns.Add("Remain Pre ", typeof(string));
                dataTbl.Columns.Add("Net Income ", typeof(string));
                dataTbl.Columns.Add("Remain Post", typeof(string));
                if (proc_Response.Any())
                {
                    foreach (var item in proc_Response)
                    {
                        dataTbl.Rows.Add(item.Particulars, item.ProcessingFees, item.PhysicalCount, item.DigitalCount, item.UTI_TXN_ID, item.CouponStatus, item.Date, item.retailer_remain_pre, item.RetailerIncome, item.retailer_remain_post);
                    }
                }
                else
                {
                    dataTbl.Rows.Add("", "", "", "", "", "", "", "", "", "");
                }
                var grid = new GridView();
                grid.DataSource = dataTbl;
                grid.DataBind();
                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=Excel_TokenPurchase_Report.xls");
                Response.ContentType = "application/ms-excel";
                Response.Charset = "";
                StringWriter sw = new StringWriter();
                HtmlTextWriter htw = new HtmlTextWriter(sw);
                grid.RenderControl(htw);
                Response.Output.Write(sw.ToString());
                Response.Flush();
                Response.End();
                return View();
            }
        }
        /// <summary>
        /// GET - View transaction or activity report
        /// </summary>
        public ActionResult Excel_TokenPurchase_Report1(string txt_frm_date, string txt_to_date, string ddl_status)
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {

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
                int pagesize = 2000;

                var proc_Response = db.pancard_transation.Where(a => a.Reailerid == userid && a.request_time > frm_date && a.request_time < to_date).OrderByDescending(s => s.idno).ToList();

                if (ddl_status != "ALL")
                {
                   proc_Response = db.pancard_transation.Where(a => a.Reailerid == userid && a.request_time > frm_date && a.request_time < to_date && a.status.ToUpper() == ddl_status).OrderByDescending(s => s.idno).ToList();

                }

                DataTable dataTbl = new DataTable();
                dataTbl.Columns.Add("Status", typeof(string));
                dataTbl.Columns.Add("Amount", typeof(string));
                dataTbl.Columns.Add("Request id", typeof(string));
                dataTbl.Columns.Add("Request Time", typeof(string));
                dataTbl.Columns.Add("Remain Pre", typeof(string));
                dataTbl.Columns.Add("Comm", typeof(string));
                dataTbl.Columns.Add("TDS", typeof(string));
                dataTbl.Columns.Add("Remain Post ₹", typeof(string));
                dataTbl.Columns.Add("Final ", typeof(string));
                
                if (proc_Response.Any())
                {
                    foreach (var item in proc_Response)
                    {
                        dataTbl.Rows.Add(item.status, item.Amount, item.requestid, item.request_time, item.Rem_pre, item.Rem_comm, item.Rem_tds, item.rem_post, item.Rem_final);
                    }
                }
                else
                {
                    dataTbl.Rows.Add("", "", "", "", "", "", "", "", "");
                }
                var grid = new GridView();
                grid.DataSource = dataTbl;
                grid.DataBind();
                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=Excel_TokenPurchase_Report.xls");
                Response.ContentType = "application/ms-excel";
                Response.Charset = "";
                StringWriter sw = new StringWriter();
                HtmlTextWriter htw = new HtmlTextWriter(sw);
                grid.RenderControl(htw);
                Response.Output.Write(sw.ToString());
                Response.Flush();
                Response.End();
                return View();
            }
        } 
        /// <summary>
        /// GET - View transaction or activity report
        /// </summary>
        public ActionResult Excel_TokenPurchase_Report1_manual(string txt_frm_date, string txt_to_date, string ddl_status)
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {

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
                int pagesize = 2000;

                var proc_Response = db.pancard_transation_manual.Where(a => a.Reailerid == userid && a.request_time > frm_date && a.request_time < to_date).OrderByDescending(s => s.idno).ToList();

                if (ddl_status != "ALL")
                {
                   proc_Response = db.pancard_transation_manual.Where(a => a.Reailerid == userid && a.request_time > frm_date && a.request_time < to_date && a.status.ToUpper() == ddl_status).OrderByDescending(s => s.idno).ToList();

                }

                DataTable dataTbl = new DataTable();
                dataTbl.Columns.Add("Status", typeof(string));
                dataTbl.Columns.Add("Name", typeof(string));
                dataTbl.Columns.Add("Mobile", typeof(string));
                dataTbl.Columns.Add("Email", typeof(string));
                dataTbl.Columns.Add("Father Name", typeof(string));
                dataTbl.Columns.Add("DOB", typeof(string));
                dataTbl.Columns.Add("Aadhaar", typeof(string));
                dataTbl.Columns.Add("Gender", typeof(string));
                 dataTbl.Columns.Add("Amount", typeof(string));
                dataTbl.Columns.Add("Request id", typeof(string));
                dataTbl.Columns.Add("Request Time", typeof(string));
                dataTbl.Columns.Add("Remain Pre", typeof(string));
                dataTbl.Columns.Add("Comm", typeof(string));
                dataTbl.Columns.Add("TDS", typeof(string));
                dataTbl.Columns.Add("Remain Post ₹", typeof(string));
                dataTbl.Columns.Add("Final ", typeof(string));
                
                if (proc_Response.Any())
                {
                    foreach (var item in proc_Response)
                    {
                        dataTbl.Rows.Add(item.status,item.Name,item.Mobile,item.Email,item.Fathername,item.DOB,item.Aadharno,item.Gender, item.Amount, item.requestid, item.request_time, item.Rem_pre, item.Rem_comm, item.Rem_tds, item.rem_post, item.Rem_final);
                    }
                }
                else
                {
                    dataTbl.Rows.Add("", "", "", "", "", "", "", "", "","","", "", "", "", "", "");
                }
                var grid = new GridView();
                grid.DataSource = dataTbl;
                grid.DataBind();
                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=Excel_TokenPurchase_Report.xls");
                Response.ContentType = "application/ms-excel";
                Response.Charset = "";
                StringWriter sw = new StringWriter();
                HtmlTextWriter htw = new HtmlTextWriter(sw);
                grid.RenderControl(htw);
                Response.Output.Write(sw.ToString());
                Response.Flush();
                Response.End();
                return View();
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
            ViewBag.checkdata = null;
            var checkdata = db.PAN_CARD_IPAY.Where(a => a.RetailerId == userid).FirstOrDefault();
            if (checkdata != null)
            {
                ViewBag.checkdata = "Data";
            }
            var tbrow = db.PAN_CARD_IPAY_Token_report_paging(pageindex, pagesize, userid, "Retailer", ddl_status, Convert.ToDateTime(txt_frm_date), Convert.ToDateTime(txt_to_date)).ToList();
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

        //[HttpPost]
        //public ActionResult getLoginCredentials()
        //{
        //    var userid = User.Identity.GetUserId();
        //    InstantPayComnUtil util = new InstantPayComnUtil();
        //    var response = util.GetUTILoginCredentials(userid);
        //    return Json(response.ToString());
        //}
        [HttpPost]
        public ActionResult buyUTIToken(decimal? amount)
        {
            try
            {
                var userid = User.Identity.GetUserId();
                var retailer = db.Retailer_Details.FirstOrDefault(s => s.RetailerId == userid);
                var psaid = db.VastBazaarRetailerOutlets.Where(s => s.RetailerId == userid).SingleOrDefault().outlet_id;
                string requestid = Guid.NewGuid().ToString();
                var token = getAuthToken();
                System.Data.Entity.Core.Objects.ObjectParameter output = new System.Data.Entity.Core.Objects.ObjectParameter("Output", typeof(string));
                var measge = db.proc_insert_PAN_CARD12(userid, amount, requestid, output).SingleOrDefault().msg;
                try
                {
                    var retailerdetails = db.Retailer_Details.FirstOrDefault(aa => aa.RetailerId == userid);
                    var dealerdetails = db.Dealer_Details.FirstOrDefault(aa => aa.DealerId == retailerdetails.DealerId);
                    var masterdetails = db.Superstokist_details.Where(aa => aa.SSId == dealerdetails.SSId).SingleOrDefault();

                    var remdetails = db.Remain_reteller_balance.Where(aa => aa.RetellerId == userid).SingleOrDefault();
                    var dlmdetails = db.Remain_dealer_balance.Where(aa => aa.DealerID == retailerdetails.DealerId).SingleOrDefault();
                    var Masterdetails = db.Remain_superstokist_balance.Where(aa => aa.SuperStokistID == dealerdetails.SSId).SingleOrDefault();

                    var admininfo = db.Admin_details.SingleOrDefault();
                    Backupinfo back = new Backupinfo();
                    var modeln = new Backupinfo.Addinfo
                    {
                        Websitename = admininfo.WebsiteUrl,
                        RetailerID = userid,
                        Email = retailerdetails.Email,
                        Mobile = retailerdetails.Mobile,
                        Details = "Pan Card Buy ",
                        RemainBalance = (decimal)remdetails.Remainamount,
                        Usertype = "Retailer"
                    };
                    back.Pancard(modeln);

                    var model1 = new Backupinfo.Addinfo
                    {
                        Websitename = admininfo.WebsiteUrl,
                        RetailerID = dealerdetails.DealerId,
                        Email = dealerdetails.Email,
                        Mobile = dealerdetails.Mobile,
                        Details = "Pan Card Buy ",

                        RemainBalance = Convert.ToDecimal(dlmdetails.Remainamount),
                        Usertype = "Dealer"
                    };
                    back.Pancard(model1);

                    var model2 = new Backupinfo.Addinfo
                    {
                        Websitename = admininfo.WebsiteUrl,
                        RetailerID = masterdetails.SSId,
                        Email = masterdetails.Email,
                        Mobile = masterdetails.Mobile,
                        Details = "Pan Card Buy ",
                        RemainBalance = Convert.ToDecimal(Masterdetails.Remainamount),
                        Usertype = "Master"
                    };
                    back.Pancard(model2);
                }
                catch { }
                if (measge == "Success")
                {
                    var reque = new
                    {
                        Amount = amount,
                        PSAID = psaid,
                        TrasId = requestid,
                        Name = retailer.Frm_Name,
                        Email = retailer.Email
                    };
                    var resquestchk = JsonConvert.SerializeObject(reque);
                    var client2 = new RestClient("http://api.vastbazaar.com/api/UTI/PurchasePanCard");
                    client2.Timeout = -1;
                    var request2 = new RestRequest(Method.POST);
                    request2.AddHeader("Authorization", "Bearer " + token);
                    request2.AddHeader("Content-Type", "application/json");
                    request2.AddParameter("application/json", resquestchk, ParameterType.RequestBody);
                    IRestResponse response2 = client2.Execute(request2);
                    dynamic resp = JsonConvert.DeserializeObject(response2.Content);
                    if (resp.Content.ADDINFO.Status == "Failed")
                    {
                        var entry = db.pancard_transation.Where(s => s.requestid == requestid).SingleOrDefault();
                        db.proc_PAN_CARD_Refund_new(Convert.ToString(entry.idno), "Failed", "Rejected", requestid);
                        try
                        {
                            var retailerdetails = db.Retailer_Details.FirstOrDefault(aa => aa.RetailerId == userid);
                            var dealerdetails = db.Dealer_Details.FirstOrDefault(aa => aa.DealerId == retailerdetails.DealerId);
                            var masterdetails = db.Superstokist_details.Where(aa => aa.SSId == dealerdetails.SSId).SingleOrDefault();

                            var remdetails = db.Remain_reteller_balance.Where(aa => aa.RetellerId == userid).SingleOrDefault();
                            var dlmdetails = db.Remain_dealer_balance.Where(aa => aa.DealerID == retailerdetails.DealerId).SingleOrDefault();
                            var Masterdetails = db.Remain_superstokist_balance.Where(aa => aa.SuperStokistID == dealerdetails.SSId).SingleOrDefault();

                            var admininfo = db.Admin_details.SingleOrDefault();
                            Backupinfo back = new Backupinfo();
                            var modeln = new Backupinfo.Addinfo
                            {
                                Websitename = admininfo.WebsiteUrl,
                                RetailerID = userid,
                                Email = retailerdetails.Email,
                                Mobile = retailerdetails.Mobile,
                                Details = "PAn Card Refund ",
                                RemainBalance = (decimal)remdetails.Remainamount,
                                Usertype = "Retailer"
                            };
                            back.Pancard(modeln);

                            var model1 = new Backupinfo.Addinfo
                            {
                                Websitename = admininfo.WebsiteUrl,
                                RetailerID = dealerdetails.DealerId,
                                Email = dealerdetails.Email,
                                Mobile = dealerdetails.Mobile,
                                Details = "PAn Card Refund ",

                                RemainBalance = Convert.ToDecimal(dlmdetails.Remainamount),
                                Usertype = "Dealer"
                            };
                            back.Pancard(model1);

                            var model2 = new Backupinfo.Addinfo
                            {
                                Websitename = admininfo.WebsiteUrl,
                                RetailerID = masterdetails.SSId,
                                Email = masterdetails.Email,
                                Mobile = masterdetails.Mobile,
                                Details = "PAn Card Refund ",
                                RemainBalance = Convert.ToDecimal(Masterdetails.Remainamount),
                                Usertype = "Master"
                            };
                            back.Pancard(model2);
                        }
                        catch { }
                        var tp = "Contact to Admin";
                        var obj = new { RESULT = "1", ADDINFO = tp };
                        return Json(JsonConvert.SerializeObject(obj), JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var obj = new { RESULT = "1", ADDINFO = resp.Content.ADDINFO.Message };
                        return Json(JsonConvert.SerializeObject(obj), JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    var obj = new { RESULT = "1", ADDINFO = measge };
                    return Json(JsonConvert.SerializeObject(obj), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                var obj = new { RESULT = "1", ADDINFO = ex.Message };
                return Json(JsonConvert.SerializeObject(obj), JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        /// <summary>
        /// POST - Update or check status
        /// </summary>
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

        /// <summary>
        /// POST - User logout and session clear
        /// </summary>
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


        [HttpPost]
        /// <summary>
        /// POST - Save form data to database
        /// </summary>
        public ActionResult CorrectionSubmit(string Title, string NameAsPerAadhar, DateTime? DateOfBirth, string FatherName, string Gender, string AadharNo, string AAdharRegisterNo, string CustomerMobileNo, string UserState, string EmailId, string PanCardNo, HttpPostedFileBase AAdharFrontImg, HttpPostedFileBase AAdharBackImg, HttpPostedFileBase SupportingDocument, string SupportingDocumentName, string CorrectionsType)
        {
            if (string.IsNullOrEmpty(Title))
            {
                return Json(new { success = false, message = "Title is required." });
            }
            else if (string.IsNullOrEmpty(NameAsPerAadhar))
            {
                return Json(new { success = false, message = "Name as per Aadhar is required." });
            }
            else if (DateOfBirth == null)
            {
                return Json(new { success = false, message = "Date of Birth is required." });
            }
            else if (string.IsNullOrEmpty(FatherName))
            {
                return Json(new { success = false, message = "Father's Name is required." });
            }
            else if (string.IsNullOrEmpty(Gender))
            {
                return Json(new { success = false, message = "Gender is required." });
            }
            else if (!Regex.IsMatch(AadharNo, @"^\d{12}$"))
            {
                return Json(new { success = false, message = "Aadhar Number must be a 12-digit number." });
            }
            else if (!Regex.IsMatch(AAdharRegisterNo, @"^[6-9][0-9]{9}$"))
            {
                return Json(new { success = false, message = "Aadhar Register Number must be a valid 10-digit number starting with 7, 8, or 9." });
            }
            else if (!Regex.IsMatch(CustomerMobileNo, @"^[6-9][0-9]{9}$"))
            {
                return Json(new { success = false, message = "Customer Mobile Number must be a valid 10-digit number starting with 7, 8, or 9." });
            }
            else if (string.IsNullOrEmpty(UserState))
            {
                return Json(new { success = false, message = "State is required." });
            }
            else if (!Regex.IsMatch(EmailId, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
            {
                return Json(new { success = false, message = "Please enter a valid Email ID." });
            }
            else if (!Regex.IsMatch(PanCardNo, @"^[A-Z]{5}[0-9]{4}[A-Z]{1}$"))
            {
                return Json(new { success = false, message = "Please enter a valid PAN Card Number. It should be in the format: ABCDE1234X." });
            }
            else if (AAdharFrontImg == null || AAdharFrontImg.ContentLength == 0)
            {
                return Json(new { success = false, message = "Aadhar Front Image is required." });
            }
            else if (AAdharBackImg == null || AAdharBackImg.ContentLength == 0)
            {
                return Json(new { success = false, message = "Aadhar Back Image is required." });
            }
            else if (string.IsNullOrEmpty(CorrectionsType))
            {
                return Json(new { success = false, message = "Correction Type is required." });
            }
            else
            {
                return Json(new { success = true, message = "" });
            }

        }

    }
}