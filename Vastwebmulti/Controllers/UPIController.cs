using A2ZMultiService;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Windows.Interop;
using System.Xml;
using Vastwebmulti.Areas.RETAILER.Models;
using Vastwebmulti.Areas.WMASTER.Models;
using Vastwebmulti.Models;
using Formatting = Newtonsoft.Json.Formatting;

namespace Vastwebmulti.Controllers
{
    /// <summary>
    /// UPI payout controller — sender details, beneficiary registration, fund transfer, aur UPI verification ke liye API actions provide karta hai.
    /// </summary>
    public class UPIController : Controller
    {
        private VastwebmultiEntities db;
        bool? upiapists=false;
        public UPIController()
        {
            db = new VastwebmultiEntities();
             var serviceinfo = db.Serviceallows.Where(aa => aa.ServiceName == "UPI API").SingleOrDefault();
            if (serviceinfo != null)
            {
                upiapists = serviceinfo.Sts;
            }
        }
        /// <summary>
        /// UPI sender ki details fetch karta hai — user authenticate karke VastBazaar API se remitter information return karta hai.
        /// </summary>
        [HttpPost]
        public string UPI_Senderdetails(Api_Details api)
        {
            if (api.Senderno != null && api.Tokenid != null && api.Userid != null)
            {
                if (upiapists == true)
                {
                    var output = authentication(api.Tokenid, api.Userid);
                    dynamic jsonchk = JsonConvert.DeserializeObject(output);
                    var chksts = jsonchk.Status.ToString();
                    var msg = jsonchk.response.ToString();
                    if (chksts == "True")
                    {
                        var userid = db.Users.Where(aa => aa.UserName == api.Userid).SingleOrDefault().UserId;
                        var apinm = db.money_api_status.Where(aa => aa.status == true && aa.catagory== "PAYOUT").SingleOrDefault();
                        if (apinm != null)
                        {
                            if (apinm.api_name.ToUpper() == "VASTWEB")
                            {
                                var vastchk = vastbazarcheck();
                                jsonchk = JsonConvert.DeserializeObject(vastchk);
                                chksts = jsonchk.Status.ToString();
                                var token = jsonchk.response.ToString();
                                if (chksts == "True")
                                {
                                    VastBazaar cb = new VastBazaar();
                                    var responseall = cb.Remitter_details_UPI(api.Senderno, token);
                                    var responsechk = responseall.Content.ToString();
                                    var responsecode1 = responseall.StatusCode.ToString();
                                    if (responsecode1 == "OK")
                                    {
                                        dynamic json = JsonConvert.DeserializeObject(responsechk);
                                        var respcode = json.Content.ResponseCode.ToString();
                                        var ADDINFO = json.Content.ADDINFO;
                                        json = JsonConvert.SerializeObject(ADDINFO);
                                        dynamic jsonObject = JsonConvert.DeserializeObject(json);
                                        dynamic outputchk = new JObject();
                                        outputchk.status = "Success";
                                        outputchk.response = jsonObject;
                                        JArray responseArray = (JArray)outputchk["response"];
                                        foreach (JObject item in responseArray)
                                        {
                                            item.Remove("apiid");
                                            item.Remove("Retailerid");
                                            item.Remove("frenoid");
                                            item.Remove("insertdate");
                                        }
                                        string modifiedJson = jsonObject.ToString();
                                        return JsonConvert.SerializeObject(outputchk);
                                    }
                                    else
                                    {
                                        dynamic json = JsonConvert.DeserializeObject(responsechk);
                                        var error = json.error.ToString();
                                        var error_decribe = json["error_description"].ToString();
                                        return outputchk("Failed", error_decribe);
                                    }
                                }
                                else
                                {
                                    return outputchk("Failed", token);
                                }
                            }
                            else
                            {
                                return outputchk("Failed", "Not Allow.");
                            }
                        }
                        else
                        {
                            return outputchk("Failed", "No Account Transfer API Assign.");
                        }
                    }
                    else
                    {
                        return outputchk("Failed", msg);
                    }
                }
                else
                {
                    return outputchk("Failed", "Not Allow");
                }
            }
            else
            {
                return outputchk("Failed", "Missing Parameter");
            }
        }
        /// <summary>
        /// UPI beneficiary register karta hai — sender ke account mein naya UPI ID add karta hai VastBazaar API ke zariye.
        /// </summary>
        [HttpPost]
        public string UPI_Ben_Registration(Api_Details api)
        {
            if (api.Senderno != null && api.Tokenid != null && api.Userid != null && api.benname != null && api.UpiID != null)
            {
                if (upiapists==true)
                {
                    var output = authentication(api.Tokenid, api.Userid);
                    dynamic jsonchk = JsonConvert.DeserializeObject(output);
                    var chksts = jsonchk.Status.ToString();
                    var msg = jsonchk.response.ToString();
                    if (chksts == "True")
                    {
                        var apinm = db.money_api_status.Where(aa => aa.status == true && aa.catagory== "PAYOUT").SingleOrDefault();
                        if (apinm != null)
                        {
                            if (apinm.api_name.ToUpper() == "VASTWEB")
                            {

                                var vastchk = vastbazarcheck();
                                jsonchk = JsonConvert.DeserializeObject(vastchk);
                                chksts = jsonchk.Status.ToString();
                                var token = jsonchk.response.ToString();
                                if (chksts == "True")
                                {
                                    VastBazaar cb = new VastBazaar();
                                    var responseall = cb.Beneficiary_register_UPI(api.benname, api.Senderno, api.UpiID, token);
                                    var responsechk = responseall.Content.ToString();
                                    var responsecode1 = responseall.StatusCode.ToString();
                                    if (responsecode1 == "OK")
                                    {
                                        dynamic json = JsonConvert.DeserializeObject(responsechk);
                                        var respcode = json.Content.ResponseCode.ToString();
                                        var ADDINFO = json.Content.ADDINFO;
                                        var stscode = ADDINFO.statuscode;
                                        json = JsonConvert.SerializeObject(ADDINFO);

                                        dynamic jsonout = JsonConvert.DeserializeObject(json);
                                        dynamic outputchk = new JObject();
                                        outputchk.status = "Success";
                                        outputchk.response = jsonout;
                                        return JsonConvert.SerializeObject(outputchk);
                                    }
                                    else
                                    {
                                        dynamic json = JsonConvert.DeserializeObject(responsechk);
                                        var error = json.error.ToString();
                                        var error_decribe = json["error_description"].ToString();
                                        return outputchk("Failed", error_decribe);
                                    }
                                }
                                else
                                {
                                    return outputchk("Failed", token);
                                }
                            }
                            else
                            {
                                return outputchk("Failed", "Not Allow.");
                            }
                        }
                        else
                        {
                            return outputchk("Failed", "No Account Transfer API Assign.");
                        }
                    }
                    else
                    {
                        return outputchk("Failed", msg);
                    }
                }
                else
                {
                    return outputchk("Failed", "NOT Allow");
                }
            }
            else
            {
                return outputchk("Failed", "Missing Parameter");
            }
        }
        /// <summary>
        /// UPI beneficiary delete karta hai — given ID se registered UPI beneficiary ko account se remove karta hai.
        /// </summary>
        [HttpPost]
        public string UPI_Ben_Delete(Api_Details api)
        {
            if (api.Senderno != null && api.Tokenid != null && api.Userid != null && api.Idno != null)
            {
                if (upiapists == true)
                {
                    var output = authentication(api.Tokenid, api.Userid);
                    dynamic jsonchk = JsonConvert.DeserializeObject(output);
                    var chksts = jsonchk.Status.ToString();
                    var msg = jsonchk.response.ToString();
                    if (chksts == "True")
                    {
                        var apinm = db.money_api_status.Where(aa => aa.status == true && aa.catagory== "PAYOUT").SingleOrDefault();
                        if (apinm != null)
                        {
                            if (apinm.api_name.ToUpper() == "VASTWEB")
                            {

                                var vastchk = vastbazarcheck();
                                jsonchk = JsonConvert.DeserializeObject(vastchk);
                                chksts = jsonchk.Status.ToString();
                                var token = jsonchk.response.ToString();
                                if (chksts == "True")
                                {
                                    VastBazaar cb = new VastBazaar();
                                    var responseall = cb.Beneficiary_Delete_UPI(api.Idno, token);
                                    var responsechk = responseall.Content.ToString();
                                    var responsecode1 = responseall.StatusCode.ToString();
                                    if (responsecode1 == "OK")
                                    {
                                        dynamic json = JsonConvert.DeserializeObject(responsechk);
                                        var respcode = json.Content.ResponseCode.ToString();
                                        var ADDINFO = json.Content.ADDINFO;
                                        var stscode = ADDINFO.statuscode;
                                        json = JsonConvert.SerializeObject(ADDINFO);

                                        dynamic jsonout = JsonConvert.DeserializeObject(json);
                                        dynamic outputchk = new JObject();
                                        outputchk.status = "Success";
                                        outputchk.response = jsonout;
                                        return JsonConvert.SerializeObject(outputchk);
                                    }
                                    else
                                    {
                                        dynamic json = JsonConvert.DeserializeObject(responsechk);
                                        var error = json.error.ToString();
                                        var error_decribe = json["error_description"].ToString();
                                        return outputchk("Failed", error_decribe);
                                    }
                                }
                                else
                                {
                                    return outputchk("Failed", token);
                                }
                            }
                            else
                            {
                                return outputchk("Failed", "Not Allow.");
                            }
                        }
                        else
                        {
                            return outputchk("Failed", "No Account Transfer API Assign.");
                        }
                    }
                    else
                    {
                        return outputchk("Failed", msg);
                    }
                }
                else
                {
                    return outputchk("Failed", "Not Allow");
                }
            }
            else
            {
                return outputchk("Failed", "Missing Parameter");
            }
        }
        /// <summary>
        /// UPI ID verify karta hai — VastBazaar API se beneficiary ka UPI ID validate karta hai aur result return karta hai.
        /// </summary>
        [HttpPost]
        public string Beneficiary_UPI_Verification(Api_Details api)
        {
            if (api.Senderno != null && api.Tokenid != null && api.Userid != null && api.UpiID != null && api.Txnid != null)
            {
                if (upiapists == true)
                {
                    var output = authentication(api.Tokenid, api.Userid);
                    var userid = db.Users.Where(aa => aa.UserName == api.Userid).SingleOrDefault().UserId;
                    dynamic jsonchk = JsonConvert.DeserializeObject(output);
                    var chksts = jsonchk.Status.ToString();
                    var msg = jsonchk.response.ToString();
                    var currentip = GetComputer_InternetIP();
                    // get mac address
                    var macaddress = GetMACAddress();
                    string CommonTranid = "W" + DateTime.Parse(DateTime.Now.ToString()).ToString("yyMMddHHmmss") + RandomString(4);
                    var bodyStream = new StreamReader(HttpContext.Request.InputStream);
                    bodyStream.BaseStream.Seek(0, SeekOrigin.Begin);
                    var bodyText = bodyStream.ReadToEnd();
                    var currentrequest = Request.Url + " body: " + bodyText;
                    if (chksts == "True")
                    {
                        var remain = db.api_remain_amount.Where(aa => aa.apiid == userid).SingleOrDefault().balance;
                        if (remain >= 0)
                        {
                            //var apislabname = db.api_user_details.Where(aa => aa.apiid == userid).SingleOrDefault().SlabName;
                            var comm = db.imps_api_comm.Where(aa => aa.userid == userid).SingleOrDefault().verify_comm;
                            var apinm = db.money_api_status.Where(aa => aa.status == true && aa.catagory== "PAYOUT").SingleOrDefault();
                            if (apinm != null)
                            {
                                if (apinm.api_name.ToUpper() == "VASTWEB")
                                {
                                    var vastchk = vastbazarcheck();
                                    jsonchk = JsonConvert.DeserializeObject(vastchk);
                                    chksts = jsonchk.Status.ToString();
                                    var token = jsonchk.response.ToString();
                                    if (chksts == "True")
                                    {
                                        decimal amount = 0;
                                        System.Data.Entity.Core.Objects.ObjectParameter msgchk = new
                                                System.Data.Entity.Core.Objects.ObjectParameter("output", typeof(string));
                                        var requestsend = "{\"remittermobile\":\"" + api.Senderno + "\",\"account\":\"" + api.UpiID + "\",\"agentid\":\"" + CommonTranid + "";
                                        var respout = db.Money_transfer_api_new(userid, amount, amount, api.Senderno, api.UpiID, "", "", api.Txnid, CommonTranid, "IMPS_VERIFY", "API", "N", requestsend, apinm.api_name, currentip, macaddress, "", currentrequest, "", "", msgchk).SingleOrDefault().msg;
                                        if (respout == "OK")
                                        {
                                            VastBazaar cb = new VastBazaar();
                                            var responseall = cb.Remitter_Verify_UPI(api.Senderno, api.UpiID, token, CommonTranid);
                                            var responsechk = responseall.Content.ToString();
                                            var responsecode1 = responseall.StatusCode.ToString();
                                            if (responsecode1 == "OK")
                                            {
                                                dynamic json = JsonConvert.DeserializeObject(responsechk);
                                                var respcode = json.Content.ResponseCode.ToString();
                                                var ADDINFO = json.Content.ADDINFO;
                                                var stscode = ADDINFO.statuscode;
                                                json = JsonConvert.SerializeObject(ADDINFO);

                                                dynamic data1 = JObject.Parse(json);
                                                dynamic jsObj = JsonConvert.DeserializeObject(json);
                                                try
                                                {
                                                    jsObj.data.ipay_id = CommonTranid;
                                                    jsObj.data.charged_amt = -(comm);
                                                }
                                                catch
                                                { }

                                                var modifiedJsonString = JsonConvert.SerializeObject(jsObj);
                                                dynamic jsObjlatest = JsonConvert.DeserializeObject(modifiedJsonString);
                                                stscode = data1.statuscode.ToString();
                                                if (stscode == "TXN")
                                                {
                                                    // var oprid = data1.data.bankrefno.ToString();
                                                    var bname = data1.data.benename.ToString();
                                                    string payidno = "";
                                                    var jsonoutput = JsonConvert.SerializeObject(jsObjlatest);
                                                    db.Money_transfer_update_new_new(CommonTranid, "SUCCESS", payidno, bname, json, jsonoutput, 0, 0);
                                                    return jsonoutput;
                                                }
                                                else if (stscode == "TUP")
                                                {
                                                    var jsonoutput = JsonConvert.SerializeObject(jsObjlatest);
                                                    return jsonoutput;
                                                }
                                                else if (stscode == "ERR")
                                                {
                                                    var oprid = ""; var bname = "";
                                                    try
                                                    {
                                                        oprid = data1.data.bankrefno.ToString();
                                                        bname = data1.data.benename.ToString();

                                                    }
                                                    catch
                                                    { }
                                                    var jsonoutput = JsonConvert.SerializeObject(jsObjlatest);
                                                    db.Money_transfer_update_new_new(CommonTranid, "FAILED", oprid, bname, json, jsonoutput, 0, 0);
                                                    return jsonoutput;
                                                }
                                                else
                                                {
                                                    var oprid = ""; var bname = "";
                                                    try
                                                    {
                                                        oprid = data1.data.bankrefno.ToString();
                                                        bname = data1.data.benename.ToString();

                                                    }
                                                    catch
                                                    {
                                                        oprid = data1.data.remarks.ToString();
                                                    }
                                                    var jsonoutput = JsonConvert.SerializeObject(jsObjlatest);
                                                    db.Money_transfer_update_new_new(CommonTranid, "FAILED", oprid, bname, json, jsonoutput, 0, 0);
                                                    return jsonoutput;
                                                }
                                            }
                                            else
                                            {
                                                dynamic json = JsonConvert.DeserializeObject(responsechk);
                                                var error = json.error.ToString();
                                                var error_decribe = json["error_description"].ToString();
                                                return outputchk("Failed", error_decribe);
                                            }
                                        }
                                        else if (respout == "PERTRANSTIONOVER")
                                        {
                                            return outputchk("Failed", "Per Transtion Limit Over.");
                                        }
                                        else if (respout == "MONTHLYLIMITOVER")
                                        {
                                            return outputchk("Failed", "Monthly Limit Over.");
                                        }
                                        else if (respout == "SAMEIDNOTALLOW")
                                        {
                                            return outputchk("Failed", "Same Trans Id Not Allowed.");
                                        }
                                        else if (respout == "RETAILERLOW")
                                        {
                                            return outputchk("Failed", "Remain Balance Low.");
                                        }
                                        else if (respout == "CAPPINGLOW")
                                        {
                                            return outputchk("Failed", "Capping Low.");
                                        }
                                        else if (respout == "STATUSDOWN")
                                        {
                                            return outputchk("Failed", "Account Transfer Status Down.");
                                        }
                                        else if (respout == "RETAILERLOW")
                                        {
                                            return outputchk("Failed", "Remain Balance Low.");
                                        }
                                        else if (respout == "APIBLOCK")
                                        {
                                            return outputchk("Failed", "Api Blocked.");
                                        }
                                        else if (respout == "DUPLICATEID")
                                        {
                                            return outputchk("Failed", "Transaction id must be unique");
                                        }
                                        else
                                        {
                                            return outputchk("Failed", "Money Transfer Down.");
                                        }
                                    }
                                    else
                                    {
                                        return outputchk("Failed", token);
                                    }
                                }
                                else
                                {
                                    return outputchk("Failed", "Not Allow.");
                                }
                            }
                            else
                            {
                                return outputchk("Failed", "No Account Transfer API Assign.");
                            }
                        }
                        else
                        {
                            return outputchk("Failed", "Remain Balance Low.");
                        }
                    }
                    else
                    {
                        return outputchk("Failed", msg);
                    }
                }
                else
                {
                    return outputchk("Failed", "Not Allow");
                }
            }
            else
            {
                return outputchk("Failed", "Missing Parameter");
            }
        }
        /// <summary>
        /// UPI ke zariye fund transfer karta hai — balance check, duplicate prevention, aur VastBazaar API se actual payment process karta hai.
        /// </summary>
        [HttpPost]
        public string Fund_transfer(Api_Details api)
        {
            if (api.Senderno != null && api.Tokenid != null && api.Userid != null && api.amount > 0  && api.Txnid != null && api.UpiID != null)
            {
                if (upiapists == true)
                {
                    var output = authentication(api.Tokenid, api.Userid);
                    var userid = db.Users.Where(aa => aa.UserName == api.Userid).SingleOrDefault().UserId;
                    var apiuserinfo = db.api_user_details.Where(aa => aa.apiid == userid).SingleOrDefault();
                    var apiremain = db.api_remain_amount.Where(aa => aa.apiid == userid).SingleOrDefault();
                    dynamic jsonchk = JsonConvert.DeserializeObject(output);
                    var chksts = jsonchk.Status.ToString();
                    var msg = jsonchk.response.ToString();
                    var currentip = GetComputer_InternetIP();
                    // get mac address
                    var macaddress = GetMACAddress();
                    var bodyStream = new StreamReader(HttpContext.Request.InputStream);
                    bodyStream.BaseStream.Seek(0, SeekOrigin.Begin);
                    var bodyText = bodyStream.ReadToEnd();
                    var currentrequest = Request.Url + " body: " + bodyText;
                    if (chksts == "True")
                    {
                        var remain = db.api_remain_amount.Where(aa => aa.apiid == userid).SingleOrDefault().balance;
                        if (remain >= api.amount)
                        {
                            //var apinmchk = db.Money_API_URLS.Where(aa => aa.Status == "Y").SingleOrDefault();
                            decimal finalamount = Convert.ToDecimal(api.amount);
                            var ch1 = db.IMPS_transtion_detsils.Where(aa => aa.accountno.ToUpper() == api.UpiID.ToUpper()
                            && aa.rch_from == userid && aa.totalamount == finalamount && aa.Status.ToUpper() == "SUCCESS").OrderByDescending(aa => aa.idno).ToList();
                            var date = ch1.Any() ? ch1.FirstOrDefault().trans_time : DateTime.Now.AddDays(-1);
                            int ggg = Convert.ToInt32((DateTime.Now - Convert.ToDateTime(date)).TotalSeconds);
                            if (ggg >= 180)
                            {
                                int amt = Convert.ToInt32(api.amount);
                                if (amt <= 50000)
                                {
                                    if (amt >= 100)
                                    {
                                        moneytransfer_cyberplate cb = new moneytransfer_cyberplate();
                                        string Tranid = "API" + DateTime.Parse(DateTime.Now.ToString()).ToString("yyMMddHHmmss") + cb.RandomString(4);
                                        var requestsend = ""; var dmttype = "";
                                        var apiname = "VASTWEB";
                                        if (apiname == "VASTWEB")
                                        {
                                            dmttype = "DMT2";
                                            requestsend = "{\"remittermobile\":\"" + api.Senderno + "\",\"account\":\"" + api.UpiID + "\",\"agentid\":\"" + Tranid + "}";
                                        }

                                        System.Data.Entity.Core.Objects.ObjectParameter outputchk1 = new System.Data.Entity.Core.Objects.ObjectParameter("Output", typeof(string));
                                        var ch = db.Money_transfer_api_new(userid, api.amount, api.amount, api.Senderno, api.UpiID, "", "", api.Txnid, Tranid, "UPI", "API", "Y", requestsend, apiname, currentip, macaddress, "", currentrequest, dmttype, "", outputchk1).Single().msg;
                                        if (ch == "RETAILERLOW")
                                        {
                                            return responsechk("Failed", api.Txnid, "", api.amount.ToString(), "", "Api Remain Balance LOW", remain.ToString());
                                        }
                                        else if (ch == "PERTRANSTIONOVER")
                                        {
                                            return responsechk("Failed", api.Txnid, "", api.amount.ToString(), "", "PER TRANSACTION LIMIT OVER", remain.ToString());
                                        }
                                        else if (ch == "MONTHLYLIMITOVER")
                                        {
                                            return responsechk("Failed", api.Txnid, "", api.amount.ToString(), "", "MONTHLY TRANSACTION LIMIT OVER", remain.ToString());
                                        }
                                        else if (ch == "APIBLOCK")
                                        {
                                            return responsechk("Failed", api.Txnid, "", api.amount.ToString(), "", "Api Block", remain.ToString());
                                        }
                                        else if (ch == "SAMEIDNOTALLOW")
                                        {
                                            return responsechk("Failed", api.Txnid, "", api.amount.ToString(), "", "Same TransID Not Allow", remain.ToString());
                                        }
                                        else if (ch == "CAPPINGLOW")
                                        {
                                            return responsechk("Failed", api.Txnid, "", api.amount.ToString(), "", "Capping Low", remain.ToString());
                                        }
                                        else if (ch == "STATUSDOWN")
                                        {
                                            return responsechk("Failed", api.Txnid, "", api.amount.ToString(), "", "Account Transfer Status Down", remain.ToString());
                                        }
                                        else if (ch == "DUPLICATEID")
                                        {
                                            return responsechk("Failed", api.Txnid, "", api.amount.ToString(), "", "Transaction id must be unique", remain.ToString());
                                        }
                                        else if (ch == "OK")
                                        {
                                            if (apiname == "VASTWEB")
                                            {
                                                VastBazaar cb1 = new VastBazaar();
                                                var vastchk = vastbazarcheck();
                                                jsonchk = JsonConvert.DeserializeObject(vastchk);
                                                chksts = jsonchk.Status.ToString();
                                                string token = jsonchk.response.ToString();
                                                System.Threading.Thread.Sleep(1000);
                                                Task<IRestResponse> task = Task.Run(() =>
                                                {
                                                    return cb1.Fund_Transfer_UPI(api.Senderno, "", Tranid, amt.ToString(), "UPI", api.UpiID, "", token, "", "Y", apiuserinfo.adharcard);
                                                });
                                                bool isCompletedSuccessfully = task.Wait(TimeSpan.FromMilliseconds(120000));
                                                var resp_imps = db.IMPS_transtion_detsils.Where(aa => aa.trans_id == Tranid).SingleOrDefault();
                                                if (isCompletedSuccessfully == true)
                                                {
                                                    var responsechk1 = task.Result.Content.ToString();
                                                    var responsecode1 = task.Result.StatusCode.ToString();
                                                    if (responsecode1 == "OK")
                                                    {
                                                        dynamic json = JsonConvert.DeserializeObject(responsechk1);
                                                        var respcode = json.Content.ResponseCode.ToString();
                                                        var ADDINFO = json.Content.ADDINFO;
                                                        var txnSts = ADDINFO.status;
                                                        if (txnSts == "SUCCESS")
                                                        {
                                                            decimal apiopeningbal = ADDINFO.result.opening_bal;
                                                            decimal chargeAmt = ADDINFO.result.charged_amt;
                                                            decimal apicloseingbal = (apiopeningbal - chargeAmt);
                                                            var oprid = ADDINFO.result.rrn?.ToString();
                                                            var bname = ADDINFO.result.name?.ToString();
                                                            string payidno = oprid;

                                                            db.Money_transfer_update_by_paytm(Tranid, "SUCCESS", payidno, bname, responsechk1, "", apiopeningbal, apiopeningbal);
                                                            //if (StatusSendSmsMoneyTransferSuccess == "Y")
                                                            //{
                                                            //    smssend.sendsmsall(RetailerMob, "Money Transfer in Bank " + bankname + " and Account Number " + account + "and Bank Refernce Id " + payidno + " and Amount " + amount + " is transfer Successfully.", "Recharge");
                                                            //}
                                                            //if (StatusSendMailMoneyTransferSuccess == "Y")
                                                            //{
                                                            //    smssend.SendEmailAll(RetailerEmail, "Money Transfer in Bank " + bankname + " and Account Number " + account + "and Bank Refernce Id " + payidno + " and Amount " + amount + " is transfer Successfully.", "Recharge", AdminEmail);
                                                            //}
                                                            remain = db.api_remain_amount.Where(aa => aa.apiid == userid).SingleOrDefault().balance;
                                                            return responsechk("Success", api.Txnid, Tranid, api.amount.ToString(), payidno, "Fund Transfer SuccessFully", remain.ToString());
                                                            //Response.servicefee = resp_imps.rem_comm;
                                                            //Response.tax = resp_imps.rem_gst;
                                                            //Response.total = resp_imps.totalamount;
                                                            //Response.status = "Success";
                                                            //Response.Details = payidno;
                                                            //Response.servicefee = resp_imps.charge;
                                                            //if (rem_details.gststatus == "N")
                                                            //{
                                                            //    Response.tax = 0;
                                                            //}
                                                            //else
                                                            //{
                                                            //    var charge = Convert.ToDecimal(resp_imps.charge);
                                                            //    Response.tax = (charge * 18) / 100;
                                                            //}
                                                            //Response.total = Convert.ToDecimal(resp_imps.totalamount) + Convert.ToDecimal(resp_imps.charge) + Convert.ToDecimal(Response.tax);
                                                            //dynamic resp = new JObject();
                                                            //resp.Amount = amt;
                                                            //resp.Status = "Success";
                                                            //resp.bankrefid = payidno;
                                                            //Response.data.Add(resp);
                                                        }
                                                        else if (txnSts == "ACCEPTED" || txnSts == "PENDING")
                                                        {
                                                            return responsechk("Pending", api.Txnid, Tranid, api.amount.ToString(), "", "Pending", remain.ToString());
                                                            //Response.servicefee = resp_imps.charge;
                                                            //if (rem_details.gststatus == "N")
                                                            //{
                                                            //    Response.tax = 0;
                                                            //}
                                                            //else
                                                            //{
                                                            //    var charge = Convert.ToDecimal(resp_imps.charge);
                                                            //    Response.tax = (charge * 18) / 100;
                                                            //}
                                                            //Response.total = Convert.ToDecimal(resp_imps.totalamount) + Convert.ToDecimal(resp_imps.charge) + Convert.ToDecimal(Response.tax);
                                                            //Response.status = "Pending";
                                                            //Response.Details = "";
                                                            //dynamic resp = new JObject();
                                                            //resp.Amount = amt;
                                                            //resp.Status = "Pending";
                                                            //resp.bankrefid = "Pending";
                                                            //Response.data.Add(resp);
                                                        }
                                                        else
                                                        {
                                                            var bname = ""; string payidno = "";
                                                            try
                                                            {
                                                                payidno = ADDINFO.status.ToString();
                                                            }
                                                            catch { }
                                                            if (payidno.Contains("Some Technical Issue") || payidno.Contains("Insufficient balance") || payidno.Contains("Balance Fatching Problem") || (payidno.Contains("Failed Due To Balance Issue")))
                                                            {
                                                                return responsechk("Pending", api.Txnid, Tranid, api.amount.ToString(), "", payidno, remain.ToString());
                                                                //Response.servicefee = resp_imps.charge;
                                                                //if (rem_details.gststatus == "N")
                                                                //{
                                                                //    Response.tax = 0;
                                                                //}
                                                                //else
                                                                //{
                                                                //    var charge = Convert.ToDecimal(resp_imps.charge);
                                                                //    Response.tax = (charge * 18) / 100;
                                                                //}
                                                                //Response.total = Convert.ToDecimal(resp_imps.totalamount) + Convert.ToDecimal(resp_imps.charge) + Convert.ToDecimal(Response.tax);
                                                                //Response.status = "Pending";
                                                                //Response.Details = "";
                                                                //dynamic resp = new JObject();
                                                                //resp.Amount = amt;
                                                                //resp.Status = "Pending";
                                                                //resp.bankrefid = "Pending";
                                                                //Response.data.Add(resp);
                                                            }
                                                            else
                                                            {
                                                                //db.Money_transfer_update_new_new(Tranid, "FAILED", payidno, bname, json.ToString(), "", 0, 0);
                                                                db.Money_transfer_update_by_paytm(Tranid, "FAILED", "Failed", bname, json.ToString(), "", 0, 0);
                                                                return responsechk("Failed", api.Txnid, Tranid, api.amount.ToString(), "", "Failed", remain.ToString());
                                                                //if (StatusSendSmsMoneyTransferFailed == "Y")
                                                                //{
                                                                //    smssend.sendsmsall(RetailerMob, "Money Transfer in Bank " + bankname + " and Account Number " + account + "and Amount" + amount + " is Failed Due To " + payidno + ".", "Recharge");
                                                                //}
                                                                //if (StatusSendMailMoneyTransferFailed == "Y")
                                                                //{
                                                                //    smssend.SendEmailAll(RetailerEmail, "Money Transfer in Bank " + bankname + " and Account Number " + account + "and Amount" + amount + " is Failed Due To " + payidno + ".", "Recharge", AdminEmail);
                                                                //}

                                                                //Response.status = "Failed";
                                                                //Response.Details = payidno;
                                                                //dynamic resp = new JObject();
                                                                //resp.Amount = amt;
                                                                //resp.Status = "Failed";
                                                                //resp.bankrefid = payidno;
                                                                //Response.data.Add(resp);
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        return responsechk("Pending", api.Txnid, Tranid, api.amount.ToString(), "", "Pending", remain.ToString());
                                                        //Response.status = "Pending";
                                                        //Response.Details = "Pending";
                                                        //dynamic resp = new JObject();
                                                        //resp.Amount = amt;
                                                        //resp.Status = "Pending";
                                                        //resp.bankrefid = "Pending";
                                                        //Response.data.Add(resp);
                                                    }
                                                }
                                                else
                                                {
                                                    return responsechk("Pending", api.Txnid, Tranid, api.amount.ToString(), "", "Pending", remain.ToString());
                                                    //dynamic jsObj = JsonConvert.DeserializeObject(response);
                                                    //jsObj.data.ipay_id = CommonTranid;
                                                    //jsObj.data.charged_amt = api.amount - (Convert.ToDecimal(comm));
                                                    //jsObj.data.opening_bal = remain;
                                                    //var modifiedJsonString = Newtonsoft.Json.JsonConvert.SerializeObject(jsObj);
                                                    //dynamic jsObjlatest = JsonConvert.DeserializeObject(modifiedJsonString);
                                                    //var jsonoutput = JsonConvert.SerializeObject(jsObjlatest);
                                                    //dynamic jsonout = JsonConvert.DeserializeObject(jsonoutput);
                                                    //dynamic outputchk = new JObject();
                                                    //outputchk.status = "Pending";
                                                    //outputchk.response = jsonout;
                                                    //return JsonConvert.SerializeObject(outputchk);
                                                }
                                            }

                                            else
                                            {
                                                return responsechk("Failed", api.Txnid, "", api.amount.ToString(), "", "No Api Open", remain.ToString());
                                            }
                                        }
                                        else
                                        {
                                            return responsechk("Pending", api.Txnid, "", api.amount.ToString(), "", "Pending", remain.ToString());
                                        }
                                    }
                                    else
                                    {
                                        return responsechk("Failed", api.Txnid, "", api.amount.ToString(), "", "Amount Should be Greater Than Rs. 100", remain.ToString());
                                    }
                                }
                                else
                                {
                                    return responsechk("Failed", api.Txnid, "", api.amount.ToString(), "", "Amount Should be Less Rs 50000", remain.ToString());
                                }
                            }
                            else
                            {
                                return responsechk("Failed", api.Txnid, "", api.amount.ToString(), "", "Please Wait 5 Minutes... Same Amount Not Transfer in same Account", remain.ToString());
                            }
                        }
                        else
                        {
                            return responsechk("Failed", api.Txnid, "", api.amount.ToString(), "", "Remain Balance Low", remain.ToString());
                        }
                    }
                    else
                    {
                        return responsechk("Failed", api.Txnid, "", api.amount.ToString(), "", msg, apiremain.ToString());
                    }
                }
                else
                {
                    return responsechk("Failed", api.Txnid, "", api.amount.ToString(), "", "Not Allow", "0");
                }
            }
            else
            {
                return responsechk("Failed", api.Txnid, "", api.amount.ToString(), "", "Missing Parameter", "0");
            }
        }
        /// <summary>
        /// Fund transfer ka standard JSON response banata hai — status, order ID, amount, bank RRN aur remaining balance include karta hai.
        /// </summary>
        public string responsechk(string status, string orderid, string apiorderid, string Amount, string bankrrn, string msg, string remain)
        {
            var resp = new
            {
                Status = status,
                orderId = orderid,
                ApiOrderId = apiorderid,
                Amount = Amount,
                rrn = bankrrn,
                Message = msg,
                Remainbal = remain
            };
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Response.Clear();
            Response.ContentType = "application/json";
            return serializer.Serialize(resp);
        }
        /// <summary>
        /// Simple JSON response banata hai — status aur output message ke saath JSON string return karta hai.
        /// </summary>
        public string outputchk(string status, string output)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var dict = new Dictionary<string, string>
                      {
                        {"Status",status},
                        {"response", output},

                      };
            Response.Clear();
            Response.ContentType = "application/json";
            return serializer.Serialize(dict);
        }
        /// <summary>
        /// User aur token ko authenticate karta hai — IP address validate karke True/False status return karta hai.
        /// </summary>
        public string authentication(string tokenid, string userid)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var userchk = db.Users.Where(aa => aa.UserName == userid).SingleOrDefault();
            if (userchk != null)
            {
                var tokenchk = db.Api_ip_address.Where(aa => aa.userid == userchk.UserId && aa.token == tokenid).SingleOrDefault();
                if (tokenchk != null)
                {
                    if (tokenchk.token == tokenid)
                    {
                        var key = userchk.UserId.Substring(0, 16);
                        var usedip = Decrypt(tokenchk.token, key);
                        var currentip = GetComputer_InternetIP();
                        if (usedip == currentip)
                        {
                            var dict = new Dictionary<string, string>
                                                                     {
                                                                       {"Status","True"},
                                                                       {"response", "Ok."}
                                                                     };
                            Response.Clear();
                            Response.ContentType = "application/json";
                            return serializer.Serialize(dict);
                        }
                        else
                        {
                            var dict = new Dictionary<string, string>
                                                                     {
                                                                       {"Status","False"},
                                                                       {"response", "Ip is Not Valid. Your Ip is :"+currentip}
                                                                     };
                            Response.Clear();
                            Response.ContentType = "application/json";
                            return serializer.Serialize(dict);
                        }
                    }
                    else
                    {
                        var dict = new Dictionary<string, string>
                                                                     {
                                                                       {"Status","False"},
                                                                       {"response", "InValid Token."}
                                                                     };
                        Response.Clear();
                        Response.ContentType = "application/json";
                        return serializer.Serialize(dict);
                    }
                }
                else
                {
                    var dict = new Dictionary<string, string>
                                                                     {
                                                                       {"Status","False"},
                                                                       {"response", "Token Id Not Created."}
                                                                     };
                    Response.Clear();
                    Response.ContentType = "application/json";
                    return serializer.Serialize(dict);
                }
            }
            else
            {
                var dict = new Dictionary<string, string>
                                                                     {
                                                                       {"Status","False"},
                                                                       {"response", "User Id Not Register With us."}
                                                                     };
                Response.Clear();
                Response.ContentType = "application/json";
                return serializer.Serialize(dict);
            }
        }
        private string GetComputer_InternetIP()
        {
            string ipaddress;
            ipaddress = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (ipaddress == "" || ipaddress == null)
                ipaddress = Request.ServerVariables["REMOTE_ADDR"];
            return ipaddress;
        }
        /// <summary>
        /// Server ki MAC address return karta hai — network interface se physical address fetch karta hai.
        /// </summary>
        public string GetMACAddress()
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            String sMacAddress = string.Empty;
            foreach (NetworkInterface adapter in nics)
            {
                if (sMacAddress == String.Empty)// only return MAC Address from first card
                {
                    IPInterfaceProperties properties = adapter.GetIPProperties();
                    sMacAddress = adapter.GetPhysicalAddress().ToString();
                }
            }
            return sMacAddress;
        }
        /// <summary>
        /// TripleDES algorithm se encrypted token ko decrypt karta hai — IP address recover karne ke liye use hota hai.
        /// </summary>
        public string Decrypt(string input, string key)
        {
            byte[] inputArray = Convert.FromBase64String(input);
            TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
            tripleDES.Key = UTF8Encoding.UTF8.GetBytes(key);
            tripleDES.Mode = CipherMode.ECB;
            tripleDES.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tripleDES.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            tripleDES.Clear();
            return UTF8Encoding.UTF8.GetString(resultArray);
        }
        private static Random random = new Random();
        /// <summary>
        /// Random alphabetic string generate karta hai — unique transaction ID banane ke liye use hota hai.
        /// </summary>
        public string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        /// <summary>
        /// VastBazaar API ka valid token check karta hai — token expired hone par naya generate karta hai aur return karta hai.
        /// </summary>
        public string vastbazarcheck()
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var tokn = db.vastbazzartokens.SingleOrDefault();
            if (tokn == null)
            {
                var response = tokencheck();
                var responsechk = response.Content.ToString();
                var responsecode = response.StatusCode.ToString();
                if (responsecode == "OK")
                {
                    VastBazaar cb = new VastBazaar();
                    dynamic json = JsonConvert.DeserializeObject(responsechk);
                    var token = json.access_token.ToString();
                    var expire = json[".expires"].ToString();
                    DateTime exp = Convert.ToDateTime(expire);
                    vastbazzartoken vast = new vastbazzartoken();
                    vast.apitoken = token;
                    vast.exptime = exp;
                    db.vastbazzartokens.Add(vast);
                    db.SaveChanges();
                    var dict = new Dictionary<string, string>
                                                                     {
                                                                       {"Status","True"},
                                                                       {"response", token}
                                                                     };
                    Response.Clear();
                    Response.ContentType = "application/json";
                    return serializer.Serialize(dict);
                }
                else
                {
                    dynamic json = JsonConvert.DeserializeObject(responsechk);
                    var error = json.error.ToString();
                    var error_decribe = json["error_description"].ToString();
                    var dict = new Dictionary<string, string>
                                                                     {
                                                                       {"Status","False"},
                                                                       {"response", error_decribe}
                                                                     };
                    Response.Clear();
                    Response.ContentType = "application/json";
                    return serializer.Serialize(dict);
                }
            }
            else
            {
                VastBazaar cb = new VastBazaar();
                DateTime curntdate = DateTime.Now.Date;
                DateTime expdate = Convert.ToDateTime(tokn.exptime).Date;
                //var responsechk = "";
                //var responsecode1 = "";
                if (expdate > curntdate)
                {
                    var dict = new Dictionary<string, string>
                                                                     {
                                                                       {"Status","True"},
                                                                       {"response",  tokn.apitoken}
                                                                     };
                    Response.Clear();
                    Response.ContentType = "application/json";
                    return serializer.Serialize(dict);
                }
                else
                {
                    var response = tokencheck();
                    var response1 = response.Content.ToString();
                    var responsecode = response.StatusCode.ToString();
                    if (responsecode == "OK")
                    {
                        dynamic json = JsonConvert.DeserializeObject(response1);
                        var token = json.access_token.ToString();
                        var expire = json[".expires"].ToString();
                        DateTime exp = Convert.ToDateTime(expire);
                        tokn.apitoken = token;
                        tokn.exptime = exp;
                        db.SaveChanges();
                        var dict = new Dictionary<string, string>
                                                                     {
                                                                       {"Status","True"},
                                                                       {"response", token}
                                                                     };
                        Response.Clear();
                        Response.ContentType = "application/json";
                        return serializer.Serialize(dict);
                    }
                    else
                    {
                        dynamic json = JsonConvert.DeserializeObject(response1);
                        if (json != null)
                        {
                            var error = json.error.ToString();
                            var error_decribe = json["error_description"].ToString();
                            var results = error_decribe;
                            var dict = new Dictionary<string, string>
                                                                     {
                                                                       {"Status","False"},
                                                                       {"response", results}
                                                                     };
                            Response.Clear();
                            Response.ContentType = "application/json";
                            return serializer.Serialize(dict);
                        }
                        else
                        {
                            var dict = new Dictionary<string, string>
                                                                     {
                                                                       {"Status","False"},
                                                                       {"response", "REsponse Error"}
                                                                     };
                            Response.Clear();
                            Response.ContentType = "application/json";
                            return serializer.Serialize(dict);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// VastBazaar API se naya auth token generate karne ke liye authentication request bhejta hai.
        /// </summary>
        public IRestResponse tokencheck()
        {
            var apidetails = db.Money_API_URLS.Where(aa => aa.API_Name == "VASTWEB").SingleOrDefault();
            var token = apidetails == null ? "" : apidetails.Token;
            var apiid = apidetails == null ? "" : apidetails.API_ID;
            var apiidpwd = apidetails == null ? "" : apidetails.Api_pwd;
            var client = new RestClient("http://api.vastbazaar.com/token");
            var request = new RestRequest(Method.POST);
            request.AddHeader("iptoken", token);
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("application/x-www-form-urlencoded", "UserName=" + apiid + "&Password=" + apiidpwd + "&grant_type=password", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            return response;
        }
        /// <summary>
        /// UPI beneficiary ki ek entry hold karne wala model — UPI ID, name, verification status aur sender number rakhta hai.
        /// </summary>
        public class ResponseItem
        {
            public int idno { get; set; }
            public string senderno { get; set; }
            public string UPIID { get; set; }
            public string BenName { get; set; }
            public bool isverified { get; set; }
            public object frenoid { get; set; }
            [JsonIgnore]
            public string apiid { get; set; }

            [JsonIgnore]
            public object Retailerid { get; set; }
            public DateTime insertdate { get; set; }
        }
        /// <summary>
        /// API response ka root object — status aur ResponseItem ki list hold karta hai.
        /// </summary>
        public class RootObject
        {
            public string Status { get; set; }
            public List<ResponseItem> Response { get; set; }
        }
    }
}