using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;
using Vastwebmulti.Areas.RETAILER.Models;
using Vastwebmulti.Models;
using static System.Web.Razor.Parser.SyntaxConstants;
using ZXing.Aztec.Internal;
using System.Collections.Generic;
using java.util;
using DocumentFormat.OpenXml.Spreadsheet;
using Quartz.Logging;

namespace Vastwebmulti.API
{
    public class RechargeController : ApiController
    {
        Backupapi backup = new Backupapi();
        [AllowAnonymous]
        [HttpGet]
        [Route("Recharge/Recharge_Get")]
        public async Task<IHttpActionResult> test11(string UserID, string Customernumber, string Optcode, string Amount, string Yourrchid, string optional1, string optional2, string Tokenid)
        {
            var corrId = string.Format("{0}{1}", DateTime.Now.Ticks, Thread.CurrentThread.ManagedThreadId);
            var requestInfo = string.Format("{0} {1}", Request.Method, Request.RequestUri);
            var requestMessage = Request.Content.ReadAsByteArrayAsync().Result;
            var apirequest = IncommingMessage(corrId, requestInfo, requestMessage);
            WriteLog("APIREQUEST", "*************************************************************************************************************");
            WriteLog("APIREQUEST", "APIREQUEST " + apirequest.ToString());
            try
            {

                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    try
                    {
                        if (!String.IsNullOrEmpty(Customernumber) && !String.IsNullOrEmpty(Amount) && Convert.ToDecimal(Amount) > 0 && !String.IsNullOrEmpty(Optcode) && !String.IsNullOrEmpty(Yourrchid) && !String.IsNullOrEmpty(Tokenid) && !String.IsNullOrEmpty(UserID))
                        {
                            ApiRecharge api = new ApiRecharge();
                            api.UserID = UserID; api.Customernumber = Customernumber; api.Optcode = Optcode;
                            api.optional1 = optional1; api.optional2 = optional2; api.optional3 = "";
                            Tokenid = Tokenid.Replace(" ", "+");
                            api.optional4 = ""; api.Tokenid = Tokenid; api.Amount = Convert.ToDecimal(Amount); api.Yourrchid = Yourrchid;

                            var operatorname = db.Operator_Code.Where(a => a.new_opt_code == api.Optcode).SingleOrDefault()?.operator_Name;

                            if (operatorname.ToUpper().StartsWith("LIFE INSURANCE CORPORATION"))
                            {
                                optional1 = db.api_user_details.Where(a => a.apiid == api.UserID).Select(a => a.emailid).SingleOrDefault();
                            }

                            if (String.IsNullOrEmpty(operatorname))
                            {
                                JObject rch = outputchk("Failed", "", String.Format("Operator Code: {0} does not exist.", api.Optcode), 0, api.Yourrchid, "");
                                return Ok(rch);
                            }
                            decimal aaaaa = Convert.ToDecimal(Amount);
                            var userid = db.Users.Where(aa => aa.UserName.ToUpper() == api.UserID.ToUpper()).SingleOrDefault();
                            var roleid = db.UserRoles.Where(x => x.UserId == userid.UserId).SingleOrDefault().RoleId;
                            var UserRole = db.Roles.Where(x => x.RoleId == roleid).SingleOrDefault().Name;
                            var blockamount = db.AmountBlockunblocks.Where(a => a.OperatorName == operatorname && a.Amount == api.Amount && a.status == "Y" && (a.User == UserRole || a.User == null)).ToList();
                            if (blockamount.Count > 0)
                            {
                                JObject rch = outputchk("Failed", "", "This Amount is Blocked Please Contact by Admin.", 0, api.Yourrchid, "");
                                return Ok(rch);
                            }
                            else
                            {
                                
                                if (userid != null)
                                {
                                    var tokenchk = db.Api_ip_address.Where(aa => aa.userid == userid.UserId && aa.token == api.Tokenid).SingleOrDefault();
                                    if (tokenchk != null)
                                    {
                                        if (tokenchk.token == api.Tokenid)
                                        {
                                            var key = userid.UserId.Substring(0, 16);
                                            var usedip = Decrypt(tokenchk.token, key);
                                            var currentip = GetComputer_InternetIP();
                                            if (usedip == currentip)
                                            {
                                                var optchk = db.Operator_Code.Where(aa => aa.new_opt_code == api.Optcode).SingleOrDefault();
                                                if (optchk != null)
                                                {
                                                    var remain = db.api_remain_amount.Where(aa => aa.apiid == userid.UserId).SingleOrDefault().balance;
                                                    if (remain >= api.Amount)
                                                    {
                                                        System.Data.Entity.Core.Objects.ObjectParameter orderidinfo = new
                                                     System.Data.Entity.Core.Objects.ObjectParameter("outputorderid", typeof(string));
                                                        System.Data.Entity.Core.Objects.ObjectParameter msg = new
                                                                        System.Data.Entity.Core.Objects.ObjectParameter("output", typeof(string));
                                                        string ordersend = DateTime.Parse(DateTime.Now.ToString()).ToString("yyMMddHHmmss") + RandomString(4);
                                                        decimal aammm = Convert.ToDecimal(api.Amount);
                                                        var chkstateswitch_list = db.swich_state_operator.Where(aa => aa.rch_from.ToUpper() == "API" && aa.optid == api.Optcode && (aa.users.ToUpper() == "ALL" || aa.users == userid.UserId) && aa.sts == "Y" && (aa.amount <= aammm && aa.maxamount >= aammm)).ToList();
                                                        var localrch = "NOT";
                                                        if (chkstateswitch_list.Count > 0)
                                                        {
                                                            try
                                                            {
                                                                var best = db.Bestoffers.Single();
                                                                //var client = new RestClient("https://catalog.paytm.com/v1/mobile/getopcirclebyrange?number=" + mobileno + "&client=androidapp&version=6.6.1&child_site_id=1&site_id=1");
                                                                var client = new RestClient("https://www.vastwebindia.com/Offer/GetInfo?userid=" + best.Emailid + "&token=" + best.token + "&number=" + api.Customernumber);
                                                                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                                                                var request = new RestRequest(Method.GET);
                                                                IRestResponse response = client.Execute(request);
                                                                dynamic stuff = JsonConvert.DeserializeObject(response.Content);
                                                                string state = (string)stuff.Response.Circle;
                                                                var chkstateswitch = chkstateswitch_list.Where(s => s.stateid.ToLower() == state.ToLower()).FirstOrDefault();
                                                                if (chkstateswitch != null)
                                                                {
                                                                    localrch = "DONE|" + chkstateswitch.api_name;
                                                                }
                                                            }
                                                            catch { }
                                                        }
                                                        var duedate = "";
                                                        if(optchk.Operator_type.ToUpper()== "BROADBAND" || optchk.Operator_type.ToUpper() == "ELECTRICITY" 
                                                            || optchk.Operator_type.ToUpper() == "FASTAG" || optchk.Operator_type.ToUpper() == "GAS" 
                                                            || optchk.Operator_type.ToUpper() == "LANDLINE" || optchk.Operator_type.ToUpper() == "LOAN"
                                                            || optchk.Operator_type.ToUpper() == "LPG GAS" || optchk.Operator_type.ToUpper() == "WATER")
                                                        {
                                                            //View Bill
                                                            Vastbillpay vb = new Vastbillpay();
                                                            VastBazaartoken Responsetoken = new VastBazaartoken();
                                                            var tokn = Responsetoken.gettoken();
                                                            string CommonTranid = "E" + DateTime.Parse(DateTime.Now.ToString()).ToString("yyMMddHHmmss") + RandomString(4);
                                                            var apioptcode = db.SRS_API.Where(aa => aa.api.ToUpper().Contains("API.VASTBAZAAR.COM") && aa.opt_code == api.Optcode).SingleOrDefault().apioptcode;

                                                            var responsechk1 = vb.Viewbill(tokn, Customernumber, apioptcode, Amount, optional1, optional2, CommonTranid);
                                                            var sts = responsechk1.StatusCode.ToString();
                                                            if(sts=="OK")
                                                            {
                                                                var responsechk = responsechk1.Content.ToString();
                                                                dynamic json = JsonConvert.DeserializeObject(responsechk);
                                                                var respcode = json.Content.ResponseCode.ToString();
                                                                var ADDINFO = json.Content.ADDINFO.ToString();
                                                                dynamic json1 = JsonConvert.DeserializeObject(ADDINFO);
                                                                if (Convert.ToBoolean(json1.IsSuccess))
                                                                {
                                                                    try
                                                                    {
                                                                        duedate = Convert.ToString(json1.BillInfo.billDueDate);
                                                                    }
                                                                    catch { }
                                                                }
                                                            }
                                                        }

                                                        var chkout = db.Recharge_API_new(userid.UserId, api.Customernumber, api.Optcode, api.Amount, "API", api.Yourrchid, ordersend, api.optional1, api.optional2, api.optional3, localrch, currentip, apirequest,duedate, orderidinfo, msg).ToList();
                                                        try
                                                        {
                                                            var retailerdetails = db.api_user_details.Where(aa => aa.apiid == userid.UserId).SingleOrDefault();    
                                                            var remdetails = db.api_remain_amount.Where(aa => aa.apiid == userid.UserId).SingleOrDefault();
                                                         
                                                            var admininfo = db.Admin_details.SingleOrDefault();
                                                            Backupinfo back = new Backupinfo();
                                                            var model = new Backupinfo.Addinfo
                                                            {
                                                                Websitename = admininfo.WebsiteUrl,
                                                                RetailerID = userid.UserId,
                                                                Email = retailerdetails.emailid,
                                                                Mobile = retailerdetails.mobile,
                                                                Details = "Recharge " + api.Customernumber + " Amount " + api.Amount,
                                                                RemainBalance = (decimal)remdetails.balance,
                                                                Usertype = "API"
                                                            };
                                                            back.Rechargeandutility(model);

                                                           
                                                        }
                                                        catch { }
                                                        var msgoutput = chkout.SingleOrDefault().msg;
                                                        var orderid = chkout.SingleOrDefault().orderinfo;
                                                        try
                                                        {   
                                                            WriteLog("APIREQUEST", "Message" + msgoutput);
                                                        }
                                                        catch { }
                                                        if (msgoutput == "APIIDBLOCK")
                                                        {
                                                            decimal am = 0;
                                                            JObject rch = outputchk("Failed", "", "Your API ID is Block.", am, api.Yourrchid, "");
                                                            return Ok(rch);
                                                        }
                                                        else if (msgoutput == "OPTBLOCK")
                                                        {
                                                            JObject rch = outputchk("Failed", "", "This Operator is Block.", 0, api.Yourrchid, "");
                                                            return Ok(rch);
                                                        }
                                                        else if (msgoutput == "APIOPTBLOCK")
                                                        {

                                                            JObject rch = outputchk("Failed", "", "This Operator is Block.", 0, api.Yourrchid, "");
                                                            return Ok(rch);
                                                        }
                                                        else if (msgoutput == "LOWBAL")
                                                        {


                                                            JObject rch = outputchk("Failed", "", "Remain Balance Low.", 0, api.Yourrchid, "");
                                                            return Ok(rch);
                                                        }
                                                        else if (msgoutput == "LOWCAPPING")
                                                        {

                                                            JObject rch = outputchk("Failed", "", "Capping Low.", 0, api.Yourrchid, "");
                                                            return Ok(rch);
                                                        }
                                                        else if (msgoutput == "MINMAXBAL")
                                                        {
                                                            JObject rch = outputchk("Failed", "", "Number is Not Valid.", 0, api.Yourrchid, "");
                                                            return Ok(rch);


                                                        }
                                                        else if (msgoutput == "TIMEDIFF")
                                                        {

                                                            JObject rch = outputchk("Pending", "", "Same Number And Same Amount Allready Success .", 0, api.Yourrchid, "");
                                                            return Ok(rch);
                                                        }
                                                        else if (msgoutput == "INVALIDINCOME")
                                                        {


                                                            JObject rch = outputchk("Failed", "", "Remain Balance Low(Invalid).", 0, api.Yourrchid, "");
                                                            return Ok(rch);
                                                        }
                                                        else if (msgoutput == "MOK")
                                                        {
                                                            remain = db.api_remain_amount.Where(aa => aa.apiid == userid.UserId).SingleOrDefault().balance;
                                                            var msgorderid = db.Recharge_info.Where(aa => aa.Order_id == orderid).OrderByDescending(aa => aa.idno).Take(1).SingleOrDefault().idno.ToString();


                                                            JObject rch = outputchk("Pending", "", "", (decimal)remain, api.Yourrchid, orderid);
                                                            return Ok(rch);
                                                        }
                                                        else if (msgoutput == "MOK" || msgoutput == "AOK" || msgoutput == "OKK")
                                                        {
                                                            remain = db.api_remain_amount.Where(aa => aa.apiid == userid.UserId).SingleOrDefault().balance;
                                                            var msgorderid = db.Recharge_info.Where(aa => aa.Order_id == orderid).OrderByDescending(aa => aa.idno).Take(1).SingleOrDefault().idno.ToString();


                                                            JObject rch = outputchk("Pending", "", "", (decimal)remain, api.Yourrchid, orderid);
                                                            return Ok(rch);
                                                        }
                                                        else
                                                        {
                                                            remain = db.api_remain_amount.Where(aa => aa.apiid == userid.UserId).SingleOrDefault().balance;
                                                            var msgorderid = db.Recharge_info.Where(aa => aa.Order_id == orderid).OrderByDescending(aa => aa.idno).Take(1).SingleOrDefault().idno.ToString();
                                                            int idnn11 = Convert.ToInt32(msgorderid);
                                                            var url = msgoutput.Replace("AOK", "");
                                                            if (url.ToUpper().Contains("API.VASTBAZAAR.COM"))
                                                            {
                                                                string CommonTranid = ordersend;//"E" + DateTime.Parse(DateTime.Now.ToString()).ToString("yyMMddHHmmss") + RandomString(4);
                                                                var apioptcode = db.SRS_API.Where(aa => aa.api.ToUpper().Contains("API.VASTBAZAAR.COM") && aa.opt_code == api.Optcode).SingleOrDefault().apioptcode;
                                                                var tokn = db.vastbazzartokens.SingleOrDefault();
                                                                if (tokn == null)
                                                                {
                                                                    var response = tokencheck();
                                                                    var responsechk = response.Content.ToString();
                                                                    var responsecode = response.StatusCode.ToString();
                                                                    if (responsecode == "Ok")
                                                                    {
                                                                        Vastbillpay vb = new Vastbillpay();
                                                                        dynamic json = JsonConvert.DeserializeObject(responsechk);
                                                                        var token = json.access_token.ToString();
                                                                        var expire = json[".expires"].ToString();
                                                                        DateTime exp = Convert.ToDateTime(expire);
                                                                        vastbazzartoken vast = new vastbazzartoken();
                                                                        vast.apitoken = token;
                                                                        vast.exptime = exp;
                                                                        db.vastbazzartokens.Add(vast);
                                                                        db.SaveChanges();
                                                                        var responsechk1 = vb.billpay(token, api.Customernumber, apioptcode, api.Amount.ToString(), api.optional1, api.optional2, CommonTranid);
                                                                        responsechk = responsechk1.Content.ToString();

                                                                        var Request = responsechk1.Request.Parameters[2].Value;
                                                                        var ReqUrl = url + "  RequestBody : " + Request;

                                                                        json = JsonConvert.DeserializeObject(responsechk);
                                                                        var respcode = json.Content.ResponseCode.ToString();
                                                                        var ADDINFO = json.Content.ADDINFO.ToString();
                                                                        dynamic json1 = JsonConvert.DeserializeObject(ADDINFO);

                                                                        Recharge_info objCourse = (from p in db.Recharge_info where p.idno == idnn11 select p).Single();
                                                                        objCourse.Recharge_response = json1.ToString();
                                                                        objCourse.Recharge_request = ReqUrl;
                                                                        db.SaveChanges();

                                                                        var status = json1.STATUS.ToString();
                                                                        decimal PRICE = 0;
                                                                        var errormsg = json1.ERRORMSG.ToString();
                                                                        string TRANSID = json1.TRANSID.ToString();

                                                                        if (status == "Success")
                                                                        {
                                                                            PRICE = Convert.ToDecimal(json1.PRICE.ToString());
                                                                            status = "Success";
                                                                            db.recharge_update(msgorderid, status, TRANSID, PRICE, json.ToString(), "Response");

                                                                            JObject liveRes = outputchk("Success", TRANSID, "", (decimal)remain, api.Yourrchid, orderid);
                                                                            try
                                                                            {
                                                                                Recharge_info objj = (from p in db.Recharge_info where p.idno == idnn11 select p).Single();
                                                                                objj.API_response = liveRes.ToString();
                                                                                objj.response_output = "";
                                                                                objj.Api_Response_output = "";
                                                                                db.SaveChanges();
                                                                                return Ok(liveRes);
                                                                            }
                                                                            catch
                                                                            {
                                                                                return Ok(liveRes);
                                                                            }
                                                                        }
                                                                        else if (status == "Failed")
                                                                        {
                                                                            var optcodei = db.Operator_Code.Where(aa => aa.new_opt_code == api.Optcode).SingleOrDefault().Operator_id.ToString();
                                                                            var show = db.failed_recharge_move.Where(aa => aa.operator_code == optcodei && aa.status == "Y").SingleOrDefault();
                                                                            if (show == null)
                                                                            {
                                                                                status = "Failed";
                                                                                db.recharge_update(msgorderid, status, errormsg, PRICE, json.ToString(), "Response");
                                                                                try
                                                                                {
                                                                                    var retailerdetails = db.api_user_details.Where(aa => aa.apiid == userid.UserId).SingleOrDefault();
                                                                                    var remdetails = db.api_remain_amount.Where(aa => aa.apiid == userid.UserId).SingleOrDefault();

                                                                                    var admininfo = db.Admin_details.SingleOrDefault();
                                                                                    Backupinfo back = new Backupinfo();
                                                                                    var model = new Backupinfo.Addinfo
                                                                                    {
                                                                                        Websitename = admininfo.WebsiteUrl,
                                                                                        RetailerID = userid.UserId,
                                                                                        Email = retailerdetails.emailid,
                                                                                        Mobile = retailerdetails.mobile,
                                                                                        Details = "Recharge Refund " + api.Customernumber + " Amount " + api.Amount,
                                                                                        RemainBalance = (decimal)remdetails.balance,
                                                                                        Usertype = "API"
                                                                                    };
                                                                                    back.Rechargeandutility(model);


                                                                                }
                                                                                catch { }
                                                                                JObject liveRes = outputchk("Failed", TRANSID, errormsg, (decimal)remain, api.Yourrchid, orderid);
                                                                                try
                                                                                {
                                                                                    Recharge_info objj = (from p in db.Recharge_info where p.idno == idnn11 select p).Single();
                                                                                    objj.API_response = liveRes.ToString();
                                                                                    objj.response_output = "";
                                                                                    objj.Api_Response_output = "";
                                                                                    db.SaveChanges();
                                                                                    return Ok(liveRes);
                                                                                }
                                                                                catch
                                                                                {
                                                                                    return Ok(liveRes);
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                var outputchkBkp = backup.recharge(api.Customernumber, api.Optcode, api.Amount, userid.UserId, idnn11, optcodei, ordersend, ref TRANSID);
                                                                                var entry = (from rch in db.Recharge_info
                                                                                             where rch.idno == idnn11
                                                                                             select new
                                                                                             { operatorid = rch.OPt_id }).SingleOrDefault();
                                                                                if (outputchkBkp == "SUCCESS")
                                                                                {
                                                                                    JObject liveRes = outputchk("Success", entry.operatorid, "", (decimal)remain, api.Yourrchid, orderid);
                                                                                    Recharge_info obj = (from p in db.Recharge_info where p.idno == idnn11 select p).Single();
                                                                                    obj.API_response = liveRes.ToString();
                                                                                    db.SaveChanges();
                                                                                    return Ok(liveRes);
                                                                                }
                                                                                else if (outputchkBkp == "FAILED")
                                                                                {
                                                                                    JObject liveRes = outputchk("Failed", "", entry.operatorid, (decimal)remain, api.Yourrchid, orderid);
                                                                                    Recharge_info obj = (from p in db.Recharge_info where p.idno == idnn11 select p).Single();
                                                                                    obj.API_response = liveRes.ToString();
                                                                                    db.SaveChanges();
                                                                                    return Ok(liveRes);
                                                                                }
                                                                                else
                                                                                {
                                                                                    JObject liverch = outputchk("Pending", "", "", (decimal)remain, api.Yourrchid, orderid);
                                                                                    return Ok(liverch);
                                                                                }
                                                                            }
                                                                        }
                                                                        else
                                                                        {


                                                                            JObject rch = outputchk("Pending", TRANSID, errormsg, (decimal)remain, api.Yourrchid, orderid);
                                                                            return Ok(rch);
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        var status = "Failed";
                                                                        dynamic json = JsonConvert.DeserializeObject(responsechk);
                                                                        var error = json.error.ToString();
                                                                        var error_decribe = json["error_description"].ToString();
                                                                        db.recharge_update(msgorderid, status, error_decribe, remain, json.ToString(), "Response");


                                                                        JObject rch = outputchk("Failed", "", error_decribe, (decimal)remain, api.Yourrchid, orderid);
                                                                        return Ok(rch);
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    Vastbillpay vb = new Vastbillpay();
                                                                    DateTime curntdate = DateTime.Now.Date;
                                                                    DateTime expdate = Convert.ToDateTime(tokn.exptime).Date;
                                                                    if (expdate > curntdate)
                                                                    {
                                                                        var responsechk1 = vb.billpay(tokn.apitoken, api.Customernumber, apioptcode, api.Amount.ToString(), api.optional1, api.optional2, CommonTranid);
                                                                        var responsechk = responsechk1.Content.ToString();

                                                                        var Request = responsechk1.Request.Parameters[2].Value;
                                                                        var ReqUrl = url + "  RequestBody : " + Request;

                                                                        dynamic json = JsonConvert.DeserializeObject(responsechk);
                                                                        var respcode = json.Content.ResponseCode.ToString();
                                                                        var ADDINFO = json.Content.ADDINFO.ToString();
                                                                        dynamic json1 = JsonConvert.DeserializeObject(ADDINFO);

                                                                        Recharge_info objCourse = (from p in db.Recharge_info where p.idno == idnn11 select p).Single();
                                                                        objCourse.Recharge_request = ReqUrl;
                                                                        db.SaveChanges();

                                                                        var status = json1.STATUS.ToString();
                                                                        decimal PRICE = 0;
                                                                        var errormsg = json1.ERRORMSG.ToString();
                                                                        string TRANSID = json1.TRANSID.ToString();

                                                                        if (status == "Success")
                                                                        {
                                                                            PRICE = Convert.ToDecimal(json1.PRICE.ToString());
                                                                            status = "Success";
                                                                            db.recharge_update(msgorderid, status, TRANSID, PRICE, json.ToString(), "Response");

                                                                            JObject liveRes = outputchk("Success", TRANSID, "", (decimal)remain, api.Yourrchid, orderid);
                                                                            try
                                                                            {
                                                                                Recharge_info objj = (from p in db.Recharge_info where p.idno == idnn11 select p).Single();
                                                                                objj.API_response = liveRes.ToString();
                                                                                objj.response_output = "";
                                                                                objj.Api_Response_output = "";
                                                                                db.SaveChanges();
                                                                                return Ok(liveRes);
                                                                            }
                                                                            catch
                                                                            {
                                                                                return Ok(liveRes);
                                                                            }
                                                                        }
                                                                        else if (status == "Failed")
                                                                        {
                                                                            var optcodei = db.Operator_Code.Where(aa => aa.new_opt_code == api.Optcode).SingleOrDefault().Operator_id.ToString();
                                                                            var show = db.failed_recharge_move.Where(aa => aa.operator_code == optcodei && aa.status == "Y").SingleOrDefault();
                                                                            if (show == null)
                                                                            {
                                                                                status = "Failed";
                                                                                db.recharge_update(msgorderid, status, errormsg, PRICE, json.ToString(), "Response");
                                                                                try
                                                                                {
                                                                                    var retailerdetails = db.api_user_details.Where(aa => aa.apiid == userid.UserId).SingleOrDefault();
                                                                                    var remdetails = db.api_remain_amount.Where(aa => aa.apiid == userid.UserId).SingleOrDefault();

                                                                                    var admininfo = db.Admin_details.SingleOrDefault();
                                                                                    Backupinfo back = new Backupinfo();
                                                                                    var model = new Backupinfo.Addinfo
                                                                                    {
                                                                                        Websitename = admininfo.WebsiteUrl,
                                                                                        RetailerID = userid.UserId,
                                                                                        Email = retailerdetails.emailid,
                                                                                        Mobile = retailerdetails.mobile,
                                                                                        Details = "Recharge Refund " + api.Customernumber + " Amount " + api.Amount,
                                                                                        RemainBalance = (decimal)remdetails.balance,
                                                                                        Usertype = "API"
                                                                                    };
                                                                                    back.Rechargeandutility(model);


                                                                                }
                                                                                catch { }
                                                                                JObject liveRes = outputchk("Failed", TRANSID, errormsg, (decimal)remain, api.Yourrchid, orderid);
                                                                                try
                                                                                {
                                                                                    Recharge_info objj = (from p in db.Recharge_info where p.idno == idnn11 select p).Single();
                                                                                    objj.API_response = liveRes.ToString();
                                                                                    objj.response_output = "";
                                                                                    objj.Api_Response_output = "";
                                                                                    db.SaveChanges();
                                                                                    return Ok(liveRes);
                                                                                }
                                                                                catch
                                                                                {
                                                                                    return Ok(liveRes);
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                var outputchkBkp = backup.recharge(api.Customernumber, api.Optcode, api.Amount, userid.UserId, idnn11, optcodei, ordersend, ref TRANSID);
                                                                                var entry = (from rch in db.Recharge_info
                                                                                             where rch.idno == idnn11
                                                                                             select new
                                                                                             { operatorid = rch.OPt_id }).SingleOrDefault();
                                                                                if (outputchkBkp == "SUCCESS")
                                                                                {
                                                                                    JObject liveRes = outputchk("Success", entry.operatorid, "", (decimal)remain, api.Yourrchid, orderid);
                                                                                    Recharge_info obj = (from p in db.Recharge_info where p.idno == idnn11 select p).Single();
                                                                                    obj.API_response = liveRes.ToString();
                                                                                    db.SaveChanges();
                                                                                    return Ok(liveRes);
                                                                                }
                                                                                else if (outputchkBkp == "FAILED")
                                                                                {
                                                                                    JObject liveRes = outputchk("Failed", "", entry.operatorid, (decimal)remain, api.Yourrchid, orderid);
                                                                                    Recharge_info obj = (from p in db.Recharge_info where p.idno == idnn11 select p).Single();
                                                                                    obj.API_response = liveRes.ToString();
                                                                                    db.SaveChanges();
                                                                                    return Ok(liveRes);
                                                                                }
                                                                                else
                                                                                {
                                                                                    JObject liverch = outputchk("Pending", "", "", (decimal)remain, api.Yourrchid, orderid);
                                                                                    return Ok(liverch);
                                                                                }
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            JObject rch = outputchk("Pending", TRANSID, errormsg, (decimal)remain, api.Yourrchid, orderid);
                                                                            return Ok(rch);
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        var response = tokencheck();
                                                                        var responsechk = response.Content.ToString();
                                                                        var responsecode = response.StatusCode.ToString();
                                                                        if (responsecode == "Ok")
                                                                        {
                                                                            dynamic json = JsonConvert.DeserializeObject(responsechk);
                                                                            var token = json.access_token.ToString();
                                                                            var expire = json[".expires"].ToString();
                                                                            DateTime exp = Convert.ToDateTime(expire);
                                                                            tokn.apitoken = token;
                                                                            tokn.exptime = exp;
                                                                            db.SaveChanges();
                                                                            var responsechk1 = vb.billpay(token, api.Customernumber, apioptcode, api.Amount.ToString(), api.optional1, api.optional2, CommonTranid);
                                                                            responsechk = responsechk1.Content.ToString();

                                                                            var Request = responsechk1.Request.Parameters[2].Value;
                                                                            var ReqUrl = url + "  RequestBody : " + Request;

                                                                            json = JsonConvert.DeserializeObject(responsechk);
                                                                            var respcode = json.Content.ResponseCode.ToString();
                                                                            var ADDINFO = json.Content.ADDINFO.ToString();
                                                                            dynamic json1 = JsonConvert.DeserializeObject(ADDINFO);

                                                                            Recharge_info objCourse = (from p in db.Recharge_info where p.idno == idnn11 select p).Single();
                                                                            objCourse.Recharge_request = ReqUrl;
                                                                            db.SaveChanges();

                                                                            var status = json1.STATUS.ToString();
                                                                            decimal PRICE = 0;
                                                                            var errormsg = json1.ERRORMSG.ToString();
                                                                            string TRANSID = json1.TRANSID.ToString();

                                                                            db.SaveChanges();
                                                                            if (status == "Success")
                                                                            {
                                                                                PRICE = Convert.ToDecimal(json1.PRICE.ToString());
                                                                                status = "Success";
                                                                                db.recharge_update(msgorderid, status, TRANSID, PRICE, json.ToString(), "Response");

                                                                                JObject liveRes = outputchk("Success", TRANSID, "", (decimal)remain, api.Yourrchid, orderid);
                                                                                try
                                                                                {
                                                                                    Recharge_info objj = (from p in db.Recharge_info where p.idno == idnn11 select p).Single();
                                                                                    objj.API_response = liveRes.ToString();
                                                                                    objj.response_output = "";
                                                                                    objj.Api_Response_output = "";
                                                                                    db.SaveChanges();
                                                                                    return Ok(liveRes);
                                                                                }
                                                                                catch
                                                                                {
                                                                                    return Ok(liveRes);
                                                                                }
                                                                            }
                                                                            else if (status == "Failed")
                                                                            {
                                                                                var optcodei = db.Operator_Code.Where(aa => aa.new_opt_code == api.Optcode).SingleOrDefault().Operator_id.ToString();
                                                                                var show = db.failed_recharge_move.Where(aa => aa.operator_code == optcodei && aa.status == "Y").SingleOrDefault();
                                                                                if (show == null)
                                                                                {
                                                                                    status = "Failed";
                                                                                    db.recharge_update(msgorderid, status, errormsg, PRICE, json.ToString(), "Response");
                                                                                    try
                                                                                    {
                                                                                        var retailerdetails = db.api_user_details.Where(aa => aa.apiid == userid.UserId).SingleOrDefault();
                                                                                        var remdetails = db.api_remain_amount.Where(aa => aa.apiid == userid.UserId).SingleOrDefault();

                                                                                        var admininfo = db.Admin_details.SingleOrDefault();
                                                                                        Backupinfo back = new Backupinfo();
                                                                                        var model = new Backupinfo.Addinfo
                                                                                        {
                                                                                            Websitename = admininfo.WebsiteUrl,
                                                                                            RetailerID = userid.UserId,
                                                                                            Email = retailerdetails.emailid,
                                                                                            Mobile = retailerdetails.mobile,
                                                                                            Details = "Recharge Refund " + api.Customernumber + " Amount " + api.Amount,
                                                                                            RemainBalance = (decimal)remdetails.balance,
                                                                                            Usertype = "API"
                                                                                        };
                                                                                        back.Rechargeandutility(model);


                                                                                    }
                                                                                    catch { }
                                                                                    JObject liveRes = outputchk("Failed", TRANSID, errormsg, (decimal)remain, api.Yourrchid, orderid);
                                                                                    try
                                                                                    {
                                                                                        Recharge_info objj = (from p in db.Recharge_info where p.idno == idnn11 select p).Single();
                                                                                        objj.API_response = liveRes.ToString();
                                                                                        objj.response_output = "";
                                                                                        objj.Api_Response_output = "";
                                                                                        db.SaveChanges();
                                                                                        return Ok(liveRes);
                                                                                    }
                                                                                    catch
                                                                                    {
                                                                                        return Ok(liveRes);
                                                                                    }
                                                                                }
                                                                                else
                                                                                {
                                                                                    var outputchkBkp = backup.recharge(api.Customernumber, api.Optcode, api.Amount, userid.UserId, idnn11, optcodei, ordersend, ref TRANSID);
                                                                                    var entry = (from rch in db.Recharge_info
                                                                                                 where rch.idno == idnn11
                                                                                                 select new
                                                                                                 { operatorid = rch.OPt_id }).SingleOrDefault();
                                                                                    if (outputchkBkp == "SUCCESS")
                                                                                    {
                                                                                        JObject liveRes = outputchk("Success", entry.operatorid, "", (decimal)remain, api.Yourrchid, orderid);
                                                                                        Recharge_info obj = (from p in db.Recharge_info where p.idno == idnn11 select p).Single();
                                                                                        obj.API_response = liveRes.ToString();
                                                                                        db.SaveChanges();
                                                                                        return Ok(liveRes);
                                                                                    }
                                                                                    else if (outputchkBkp == "FAILED")
                                                                                    {
                                                                                        JObject liveRes = outputchk("Failed", "", entry.operatorid, (decimal)remain, api.Yourrchid, orderid);
                                                                                        Recharge_info obj = (from p in db.Recharge_info where p.idno == idnn11 select p).Single();
                                                                                        obj.API_response = liveRes.ToString();
                                                                                        db.SaveChanges();
                                                                                        return Ok(liveRes);
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        JObject liverch = outputchk("Pending", "", "", (decimal)remain, api.Yourrchid, orderid);
                                                                                        return Ok(liverch);
                                                                                    }
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                JObject rch = outputchk("Pending", TRANSID, errormsg, (decimal)remain, api.Yourrchid, orderid);
                                                                                return Ok(rch);
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            var status = "Failed";
                                                                            dynamic json = JsonConvert.DeserializeObject(responsechk);
                                                                            var error = json.error.ToString();
                                                                            var error_decribe = json["error_description"].ToString();
                                                                            db.recharge_update(msgorderid, status, error_decribe, api.Amount, json.ToString(), "Response");


                                                                            JObject rch = outputchk("Failed", "", error_decribe, (decimal)remain, api.Yourrchid, orderid);
                                                                            return Ok(rch);
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                            else if (url.ToUpper().Contains("RECHARGE/RECHARGE"))
                                                            {
                                                                var tkn = db.Recharge_API_URLS.Where(aa => aa.url == url).SingleOrDefault();
                                                                var token = tkn.Token;
                                                                Tokenid = tkn.Token;
                                                                var Userid = tkn.API_ID;
                                                                POST_API PA = new POST_API();
                                                                idnn11 = 0;
                                                                decimal amt = Convert.ToDecimal(api.Amount);
                                                                msgorderid = db.Recharge_info.Where(aa => aa.Order_id == orderid).OrderByDescending(aa => aa.idno).Take(1).SingleOrDefault().idno.ToString();
                                                                idnn11 = Convert.ToInt32(msgorderid);
                                                                string CommonTranid = ordersend;//"E" + DateTime.Parse(DateTime.Now.ToString()).ToString("yyMMddHHmmss") + PA.RandomString(4);
                                                                var apioptcode = db.SRS_API.Where(aa => aa.api == url && aa.opt_code == api.Optcode).SingleOrDefault().apioptcode;
                                                                var responsechk1 = PA.RchReq(token, api.Customernumber, Tokenid, Userid, amt, apioptcode, CommonTranid, api.optional1, api.optional2, url);
                                                                var responsechk = responsechk1.Content.ToString();
                                                                try
                                                                {
                                                                    WriteLog("APIREQUEST", "Response json" + responsechk);
                                                                }
                                                                catch { }
                                                                dynamic json = JsonConvert.DeserializeObject(responsechk);

                                                                var Request = responsechk1.Request.Parameters[2].Value;
                                                                var ReqUrl = url + "  RequestBody : " + Request;
                                                                Recharge_info objCourse = (from p in db.Recharge_info where p.idno == idnn11 select p).Single();
                                                                objCourse.Recharge_response = json.ToString();
                                                                objCourse.Recharge_request = ReqUrl;
                                                                db.SaveChanges();
                                                                var status = json.Status.ToString();
                                                                string Transid = json.Transid.ToString();
                                                                var errormsg = json.Errormsg.ToString();
                                                                var remainamount = json.Remain.ToString();
                                                                Yourrchid = json.Yourrchid.ToString();
                                                                var RechargeID = json.RechargeID.ToString();
                                                                if (status.ToUpper() == "SUCCESS")
                                                                {
                                                                    status = "Success";
                                                                    db.recharge_update(msgorderid, status, Transid, Convert.ToDecimal(remainamount), json.ToString(), "Response");
                                                                    JObject liveRes = outputchk("Success", Transid, "", (decimal)remain, api.Yourrchid, orderid);
                                                                    try
                                                                    {
                                                                        Recharge_info objj = (from p in db.Recharge_info where p.idno == idnn11 select p).Single();
                                                                        objj.API_response = liveRes.ToString();
                                                                        objj.response_output = "";
                                                                        objj.Api_Response_output = "";
                                                                        db.SaveChanges();
                                                                        return Ok(liveRes);
                                                                    }
                                                                    catch
                                                                    {
                                                                        return Ok(liveRes);
                                                                    }
                                                                }
                                                                else if (status.ToUpper() == "FAILED")
                                                                {

                                                                    var optcodei = db.Operator_Code.Where(aa => aa.new_opt_code == api.Optcode).SingleOrDefault().Operator_id.ToString();
                                                                    var show = db.failed_recharge_move.Where(aa => aa.operator_code == optcodei && aa.status == "Y").SingleOrDefault();
                                                                    if (show == null)
                                                                    {
                                                                        status = "Failed";
                                                                        db.recharge_update(msgorderid, status, "NA", Convert.ToDecimal(0), json.ToString(), "Response");
                                                                        try
                                                                        {
                                                                            var retailerdetails = db.api_user_details.Where(aa => aa.apiid == userid.UserId).SingleOrDefault();
                                                                            var remdetails = db.api_remain_amount.Where(aa => aa.apiid == userid.UserId).SingleOrDefault();

                                                                            var admininfo = db.Admin_details.SingleOrDefault();
                                                                            Backupinfo back = new Backupinfo();
                                                                            var model = new Backupinfo.Addinfo
                                                                            {
                                                                                Websitename = admininfo.WebsiteUrl,
                                                                                RetailerID = userid.UserId,
                                                                                Email = retailerdetails.emailid,
                                                                                Mobile = retailerdetails.mobile,
                                                                                Details = "Recharge Refund " + api.Customernumber + " Amount " + api.Amount,
                                                                                RemainBalance = (decimal)remdetails.balance,
                                                                                Usertype = "API"
                                                                            };
                                                                            back.Rechargeandutility(model);


                                                                        }
                                                                        catch { }
                                                                        JObject liveRes = outputchk("Failed", "", "Failed", (decimal)remain, api.Yourrchid, orderid);
                                                                        try
                                                                        {
                                                                            Recharge_info objj = (from p in db.Recharge_info where p.idno == idnn11 select p).Single();
                                                                            objj.API_response = liveRes.ToString();
                                                                            objj.response_output = "";
                                                                            objj.Api_Response_output = "";
                                                                            db.SaveChanges();
                                                                            return Ok(liveRes);
                                                                        }
                                                                        catch
                                                                        {
                                                                            return Ok(liveRes);
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        var outputchkBkp = backup.recharge(api.Customernumber, api.Optcode, api.Amount, userid.UserId, idnn11, optcodei, ordersend, ref Transid);
                                                                        var entry = (from rch in db.Recharge_info
                                                                                     where rch.idno == idnn11
                                                                                     select new
                                                                                     { operatorid = rch.OPt_id }).SingleOrDefault();
                                                                        if (outputchkBkp == "SUCCESS")
                                                                        {
                                                                            JObject liveRes = outputchk("Success", entry.operatorid, "", (decimal)remain, api.Yourrchid, orderid);
                                                                            Recharge_info obj = (from p in db.Recharge_info where p.idno == idnn11 select p).Single();
                                                                            obj.API_response = liveRes.ToString();
                                                                            db.SaveChanges();
                                                                            return Ok(liveRes);
                                                                        }
                                                                        else if (outputchkBkp == "FAILED")
                                                                        {
                                                                            JObject liveRes = outputchk("Failed", "", entry.operatorid, (decimal)remain, api.Yourrchid, orderid);
                                                                            Recharge_info obj = (from p in db.Recharge_info where p.idno == idnn11 select p).Single();
                                                                            obj.API_response = liveRes.ToString();
                                                                            db.SaveChanges();
                                                                            return Ok(liveRes);
                                                                        }
                                                                        else
                                                                        {
                                                                            JObject liverch = outputchk("Pending", "", "", (decimal)remain, api.Yourrchid, orderid);
                                                                            return Ok(liverch);
                                                                        }
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    JObject rch = outputchk("Pending", "", "", (decimal)remain, api.Yourrchid, orderid);
                                                                    return Ok(rch);
                                                                }
                                                            }
                                                            else if (url.ToUpper().Contains("MROBOTICS.IN"))
                                                            {
                                                                var tkn = db.Recharge_API_URLS.Where(aa => aa.url == url).SingleOrDefault();
                                                                var token = tkn.Token;
                                                                Tokenid = tkn.Token;
                                                                var Userid = tkn.API_ID;
                                                                POST_API PA = new POST_API();
                                                                int idnn111 = 0;
                                                                decimal amt = Convert.ToDecimal(api.Amount);
                                                                msgorderid = db.Recharge_info.Where(aa => aa.Order_id == orderid).OrderByDescending(aa => aa.idno).Take(1).SingleOrDefault().idno.ToString();
                                                                idnn111 = Convert.ToInt32(msgorderid);
                                                                string CommonTranid = ordersend;//"E" + DateTime.Parse(DateTime.Now.ToString()).ToString("yyMMddHHmmss") + PA.RandomString(4);
                                                                var apioptcode = db.SRS_API.Where(aa => aa.api == url && aa.opt_code == api.Optcode).SingleOrDefault().apioptcode;
                                                                var task = Task.Run(() =>
                                                                {
                                                                    return PA.RchReqMrobotics(token, api.Customernumber, Tokenid, Userid, amt, apioptcode, CommonTranid, api.optional1, api.optional2, api.Optcode, url);
                                                                });
                                                                var Request = task.Result.Request.Parameters[1].Value;
                                                                var ReqUrl = url + "  RequestBody : " + Request;
                                                                Recharge_info objCourse = (from p in db.Recharge_info where p.idno == idnn111 select p).Single();
                                                                objCourse.Recharge_request = ReqUrl;
                                                                db.SaveChanges();
                                                                bool isCompletedSuccessfully = task.Wait(TimeSpan.FromMilliseconds(60000));     // 1 minutes
                                                                if (isCompletedSuccessfully)
                                                                {
                                                                    var finaloutput = task.Result;
                                                                    var responsechk = finaloutput.Content.ToString();
                                                                    try
                                                                    {
                                                                        WriteLog("APIREQUEST", "Response json" + responsechk);
                                                                    }
                                                                    catch { }
                                                                    dynamic json = JsonConvert.DeserializeObject(responsechk);
                                                                    string company_id = apioptcode.ToString();
                                                                    string remainamount = "0"; string status = ""; string Transid = ""; string webcontent = "";
                                                                    if (company_id == "4")
                                                                    {
                                                                        webcontent = "";
                                                                        status = json.status.ToString();
                                                                    }
                                                                    else
                                                                    {
                                                                        Recharge_info objCours = (from p in db.Recharge_info where p.idno == idnn111 select p).Single();
                                                                        objCours.Recharge_response = json.ToString();
                                                                        db.SaveChanges();
                                                                        webcontent = json.ToString();
                                                                        status = json.status.ToString();

                                                                    }
                                                                    if (status.ToUpper() == "SUCCESS")
                                                                    {
                                                                        remainamount = json.balance.ToString();
                                                                        Transid = json.tnx_id.ToString();

                                                                        status = "Success";
                                                                        db.recharge_update(msgorderid, status, Transid, Convert.ToDecimal(remainamount), webcontent, "Response");
                                                                        JObject liveRes = outputchk("Success", Transid, "", (decimal)remain, api.Yourrchid, orderid);
                                                                        try
                                                                        {
                                                                            Recharge_info objj = (from p in db.Recharge_info where p.idno == idnn11 select p).Single();
                                                                            objj.API_response = liveRes.ToString();
                                                                            objj.response_output = "";
                                                                            objj.Api_Response_output = "";
                                                                            db.SaveChanges();
                                                                            return Ok(liveRes);
                                                                        }
                                                                        catch
                                                                        {
                                                                            return Ok(liveRes);
                                                                        }
                                                                    }
                                                                    else if (status.ToUpper() == "FAILURE" || status.ToUpper() == "FAILED")
                                                                    {

                                                                        var optcodei = db.Operator_Code.Where(aa => aa.new_opt_code == api.Optcode).SingleOrDefault().Operator_id.ToString();
                                                                        var show = db.failed_recharge_move.Where(aa => aa.operator_code == optcodei && aa.status == "Y").SingleOrDefault();
                                                                        if (show == null)
                                                                        {
                                                                            status = "Failed";
                                                                            db.recharge_update(msgorderid, status, "NA", Convert.ToDecimal(0), webcontent, "Response");
                                                                            try
                                                                            {
                                                                                var retailerdetails = db.api_user_details.Where(aa => aa.apiid == userid.UserId).SingleOrDefault();
                                                                                var remdetails = db.api_remain_amount.Where(aa => aa.apiid == userid.UserId).SingleOrDefault();

                                                                                var admininfo = db.Admin_details.SingleOrDefault();
                                                                                Backupinfo back = new Backupinfo();
                                                                                var model = new Backupinfo.Addinfo
                                                                                {
                                                                                    Websitename = admininfo.WebsiteUrl,
                                                                                    RetailerID = userid.UserId,
                                                                                    Email = retailerdetails.emailid,
                                                                                    Mobile = retailerdetails.mobile,
                                                                                    Details = "Recharge Refund " + api.Customernumber + " Amount " + api.Amount,
                                                                                    RemainBalance = (decimal)remdetails.balance,
                                                                                    Usertype = "API"
                                                                                };
                                                                                back.Rechargeandutility(model);


                                                                            }
                                                                            catch { }
                                                                            JObject liveRes = outputchk("Failed", "", "Failed", (decimal)remain, api.Yourrchid, orderid);
                                                                            try
                                                                            {
                                                                                Recharge_info objj = (from p in db.Recharge_info where p.idno == idnn11 select p).Single();
                                                                                objj.API_response = liveRes.ToString();
                                                                                objj.response_output = "";
                                                                                objj.Api_Response_output = "";
                                                                                db.SaveChanges();
                                                                                return Ok(liveRes);
                                                                            }
                                                                            catch
                                                                            {
                                                                                return Ok(liveRes);
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            var outputchkBkp = backup.recharge(api.Customernumber, api.Optcode, api.Amount, userid.UserId, idnn11, optcodei, ordersend, ref Transid);
                                                                            var entry = (from rch in db.Recharge_info
                                                                                         where rch.idno == idnn11
                                                                                         select new
                                                                                         { operatorid = rch.OPt_id }).SingleOrDefault();
                                                                            if (outputchkBkp == "SUCCESS")
                                                                            {
                                                                                JObject liveRes = outputchk("Success", entry.operatorid, "", (decimal)remain, api.Yourrchid, orderid);
                                                                                Recharge_info obj = (from p in db.Recharge_info where p.idno == idnn11 select p).Single();
                                                                                obj.API_response = liveRes.ToString();
                                                                                db.SaveChanges();
                                                                                return Ok(liveRes);
                                                                            }
                                                                            else if (outputchkBkp == "FAILED")
                                                                            {
                                                                                JObject liveRes = outputchk("Failed", "", entry.operatorid, (decimal)remain, api.Yourrchid, orderid);
                                                                                Recharge_info obj = (from p in db.Recharge_info where p.idno == idnn11 select p).Single();
                                                                                obj.API_response = liveRes.ToString();
                                                                                db.SaveChanges();
                                                                                return Ok(liveRes);
                                                                            }
                                                                            else
                                                                            {
                                                                                JObject liverch = outputchk("Pending", "", "", (decimal)remain, api.Yourrchid, orderid);
                                                                                return Ok(liverch);
                                                                            }
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        JObject liverch = outputchk("Pending", "", "", (decimal)remain, api.Yourrchid, orderid);
                                                                        return Ok(liverch);
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    JObject liverch = outputchk("Pending", "", "", (decimal)remain, api.Yourrchid, orderid);
                                                                    return Ok(liverch);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                var apiinfo = db.RechargeapiInfoes.Where(aa => aa.apiendpoint.ToUpper() == url.ToUpper()).SingleOrDefault();
                                                                if (apiinfo != null)
                                                                {
                                                                    var apiremain = "0"; var _status = "Pending"; var transid = ""; var errormsg = "";
                                                                    var apiresponse = "";

                                                                    ApiResponse rech = RechargeServices.Recharge(apiinfo, api.Customernumber, api.Optcode, api.Amount, orderid);
                                                                    _status = rech.status;
                                                                    transid = rech.operatorId;
                                                                    errormsg = rech.errormsg;
                                                                    apiremain = rech.apiremain;
                                                                    apiresponse = rech.api_response;
                                                                    int idnn111 = rech.id;
                                                                    msgorderid = rech.id.ToString();

                                                                    if (_status == "Success")
                                                                    {
                                                                        var status = "Success";
                                                                        db.recharge_update(msgorderid, status, transid, Convert.ToDecimal(apiremain), apiresponse.ToString(), "Response");
                                                                        JObject obj1 = outputchk("Success", transid, "", (decimal)remain, api.Yourrchid, orderid);
                                                                        try
                                                                        {
                                                                            Recharge_info objj = (from p in db.Recharge_info where p.idno == idnn111 select p).Single();
                                                                            objj.API_response = obj1.ToString();
                                                                            objj.response_output = "";
                                                                            objj.Api_Response_output = "";
                                                                            db.SaveChanges();
                                                                            return Ok(obj1);
                                                                        }
                                                                        catch
                                                                        {
                                                                            return Ok(obj1);
                                                                        }

                                                                    }
                                                                    else if (_status == "Failed")
                                                                    {
                                                                        var optcodei = db.Operator_Code.Where(aa => aa.new_opt_code == api.Optcode).SingleOrDefault().Operator_id.ToString();
                                                                        var show = db.failed_recharge_move.Where(aa => aa.operator_code == optcodei && aa.status == "Y").SingleOrDefault();
                                                                        if (show == null)
                                                                        {
                                                                            //status = "Failed";
                                                                            var status = "Failed";
                                                                            db.recharge_update(msgorderid, status, "NA", Convert.ToDecimal(0), apiresponse, "Response");
                                                                            try
                                                                            {
                                                                                var retailerdetails = db.api_user_details.Where(aa => aa.apiid == userid.UserId).SingleOrDefault();
                                                                                var remdetails = db.api_remain_amount.Where(aa => aa.apiid == userid.UserId).SingleOrDefault();

                                                                                var admininfo = db.Admin_details.SingleOrDefault();
                                                                                Backupinfo back = new Backupinfo();
                                                                                var model = new Backupinfo.Addinfo
                                                                                {
                                                                                    Websitename = admininfo.WebsiteUrl,
                                                                                    RetailerID = userid.UserId,
                                                                                    Email = retailerdetails.emailid,
                                                                                    Mobile = retailerdetails.mobile,
                                                                                    Details = "Recharge Refund " + api.Customernumber + " Amount " + api.Amount,
                                                                                    RemainBalance = (decimal)remdetails.balance,
                                                                                    Usertype = "API"
                                                                                };
                                                                                back.Rechargeandutility(model);


                                                                            }
                                                                            catch { }
                                                                            JObject liveRes = outputchk("Failed", "", "Failed", (decimal)remain, api.Yourrchid, orderid);
                                                                            try
                                                                            {
                                                                                Recharge_info objj = (from p in db.Recharge_info where p.idno == idnn111 select p).Single();
                                                                                objj.API_response = liveRes.ToString();
                                                                                objj.response_output = "";
                                                                                objj.Api_Response_output = "";
                                                                                db.SaveChanges();
                                                                                return Ok(liveRes);
                                                                            }
                                                                            catch
                                                                            {
                                                                                return Ok(liveRes);
                                                                            }
                                                                        }
                                                                        else
                                                                        {

                                                                            var optcodei1 = db.Operator_Code.Where(aa => aa.new_opt_code == api.Optcode).SingleOrDefault().Operator_id.ToString();
                                                                            var show1 = db.failed_recharge_move.Where(aa => aa.operator_code == optcodei1 && aa.status == "Y").SingleOrDefault();
                                                                            if (show1 == null)
                                                                            {
                                                                                var status = "Failed";
                                                                                db.recharge_update(msgorderid, status, "NA", Convert.ToDecimal(0), apiresponse, "Response");
                                                                                try
                                                                                {
                                                                                    var retailerdetails = db.api_user_details.Where(aa => aa.apiid == userid.UserId).SingleOrDefault();
                                                                                    var remdetails = db.api_remain_amount.Where(aa => aa.apiid == userid.UserId).SingleOrDefault();

                                                                                    var admininfo = db.Admin_details.SingleOrDefault();
                                                                                    Backupinfo back = new Backupinfo();
                                                                                    var model = new Backupinfo.Addinfo
                                                                                    {
                                                                                        Websitename = admininfo.WebsiteUrl,
                                                                                        RetailerID = userid.UserId,
                                                                                        Email = retailerdetails.emailid,
                                                                                        Mobile = retailerdetails.mobile,
                                                                                        Details = "Recharge Refund " + api.Customernumber + " Amount " + api.Amount,
                                                                                        RemainBalance = (decimal)remdetails.balance,
                                                                                        Usertype = "API"
                                                                                    };
                                                                                    back.Rechargeandutility(model);


                                                                                }
                                                                                catch { }
                                                                                JObject liveRes = outputchk("Failed", "", "Failed", (decimal)remain, api.Yourrchid, orderid);
                                                                                try
                                                                                {
                                                                                    Recharge_info objj = (from p in db.Recharge_info where p.idno == idnn111 select p).Single();
                                                                                    objj.API_response = liveRes.ToString();
                                                                                    objj.response_output = "";
                                                                                    objj.Api_Response_output = "";
                                                                                    db.SaveChanges();
                                                                                    return Ok(liveRes);
                                                                                }
                                                                                catch
                                                                                {
                                                                                    return Ok(liveRes);
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                var outputchkBkp = backup.recharge(api.Customernumber, api.Optcode, api.Amount, userid.UserId, idnn111, optcodei, ordersend, ref transid);
                                                                                var entry = (from rch in db.Recharge_info
                                                                                             where rch.idno == idnn111
                                                                                             select new
                                                                                             { operatorid = rch.OPt_id }).SingleOrDefault();
                                                                                if (outputchkBkp == "SUCCESS")
                                                                                {
                                                                                    JObject liveRes = outputchk("Success", entry.operatorid, "", (decimal)remain, api.Yourrchid, orderid);
                                                                                    Recharge_info obj = (from p in db.Recharge_info where p.idno == idnn111 select p).Single();
                                                                                    obj.API_response = liveRes.ToString();
                                                                                    db.SaveChanges();
                                                                                    return Ok(liveRes);
                                                                                }
                                                                                else if (outputchkBkp == "FAILED")
                                                                                {
                                                                                    JObject liveRes = outputchk("Failed", "", entry.operatorid, (decimal)remain, api.Yourrchid, orderid);
                                                                                    Recharge_info obj = (from p in db.Recharge_info where p.idno == idnn111 select p).Single();
                                                                                    obj.API_response = liveRes.ToString();
                                                                                    db.SaveChanges();
                                                                                    return Ok(liveRes);
                                                                                }
                                                                                else
                                                                                {
                                                                                    JObject liverch = outputchk("Pending", "", "", (decimal)remain, api.Yourrchid, orderid);
                                                                                    return Ok(liverch);
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        JObject rch = outputchk("Pending", "", transid, (decimal)remain, api.Yourrchid, orderid);
                                                                        return Ok(rch);
                                                                    }

                                                                }
                                                                else
                                                                {
                                                                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                                                                    HttpWebRequest WebRequestObject = (HttpWebRequest)HttpWebRequest.Create(url);
                                                                    WebRequestObject.Timeout = (System.Int32)TimeSpan.FromSeconds(125).TotalMilliseconds;
                                                                    WebResponse Response = WebRequestObject.GetResponse();
                                                                    Stream WebStream = Response.GetResponseStream();
                                                                    StreamReader Reader = new StreamReader(WebStream);
                                                                    var webcontent = Reader.ReadToEnd();
                                                                    try
                                                                    {
                                                                        WriteLog("APIREQUEST", "Response json" + webcontent);
                                                                    }
                                                                    catch { }
                                                                    Recharge_info objjj = (from p in db.Recharge_info where p.idno == idnn11 select p).Single();
                                                                    objjj.Recharge_response = webcontent.ToString();
                                                                    db.SaveChanges();

                                                                    if (url.ToUpper().Contains("LIVE.VASTWEBINDIA.COM"))
                                                                    {
                                                                        dynamic stuff = JsonConvert.DeserializeObject(webcontent);
                                                                        var status = stuff.Status.ToString();
                                                                        var Mobile = stuff.Mobile.ToString();
                                                                        Amount = stuff.Amount.ToString();
                                                                        var RCHID = stuff.RCHID.ToString();
                                                                        string Operatorid = stuff.Operatorid.ToString();
                                                                        var remainamount = stuff.remainamount.ToString();
                                                                        var LapuNumber = stuff.LapuNumber.ToString();
                                                                        if (status.ToUpper() == "SUCCESS")
                                                                        {
                                                                            status = "Success";
                                                                            db.recharge_update(msgorderid, status, Operatorid, Convert.ToDecimal(remainamount), webcontent.ToString(), "Response");
                                                                            JObject obj1 = outputchk("Success", Operatorid, "", (decimal)remain, api.Yourrchid, orderid);
                                                                            try
                                                                            {
                                                                                Recharge_info objj = (from p in db.Recharge_info where p.idno == idnn11 select p).Single();
                                                                                objj.API_response = obj1.ToString();
                                                                                objj.response_output = "";
                                                                                objj.Api_Response_output = "";
                                                                                db.SaveChanges();
                                                                                return Ok(obj1);
                                                                            }
                                                                            catch
                                                                            {
                                                                                return Ok(obj1);
                                                                            }
                                                                        }
                                                                        else if (status.ToUpper() == "FAILED")
                                                                        {

                                                                            var optcodei = db.Operator_Code.Where(aa => aa.new_opt_code == api.Optcode).SingleOrDefault().Operator_id.ToString();
                                                                            var show = db.failed_recharge_move.Where(aa => aa.operator_code == optcodei && aa.status == "Y").SingleOrDefault();
                                                                            if (show == null)
                                                                            {
                                                                                status = "Failed";
                                                                                db.recharge_update(msgorderid, status, "NA", Convert.ToDecimal(0), webcontent, "Response");
                                                                                try
                                                                                {
                                                                                    var retailerdetails = db.api_user_details.Where(aa => aa.apiid == userid.UserId).SingleOrDefault();
                                                                                    var remdetails = db.api_remain_amount.Where(aa => aa.apiid == userid.UserId).SingleOrDefault();

                                                                                    var admininfo = db.Admin_details.SingleOrDefault();
                                                                                    Backupinfo back = new Backupinfo();
                                                                                    var model = new Backupinfo.Addinfo
                                                                                    {
                                                                                        Websitename = admininfo.WebsiteUrl,
                                                                                        RetailerID = userid.UserId,
                                                                                        Email = retailerdetails.emailid,
                                                                                        Mobile = retailerdetails.mobile,
                                                                                        Details = "Recharge Refund " + api.Customernumber + " Amount " + api.Amount,
                                                                                        RemainBalance = (decimal)remdetails.balance,
                                                                                        Usertype = "API"
                                                                                    };
                                                                                    back.Rechargeandutility(model);


                                                                                }
                                                                                catch { }
                                                                                JObject liveRes = outputchk("Failed", "", "Failed", (decimal)remain, api.Yourrchid, orderid);
                                                                                try
                                                                                {
                                                                                    Recharge_info objj = (from p in db.Recharge_info where p.idno == idnn11 select p).Single();
                                                                                    objj.API_response = liveRes.ToString();
                                                                                    objj.response_output = "";
                                                                                    objj.Api_Response_output = "";
                                                                                    db.SaveChanges();
                                                                                    return Ok(liveRes);
                                                                                }
                                                                                catch
                                                                                {
                                                                                    return Ok(liveRes);
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                var outputchkBkp = backup.recharge(api.Customernumber, api.Optcode, api.Amount, userid.UserId, idnn11, optcodei, ordersend, ref Operatorid);
                                                                                var entry = (from rch in db.Recharge_info
                                                                                             where rch.idno == idnn11
                                                                                             select new
                                                                                             { operatorid = rch.OPt_id }).SingleOrDefault();
                                                                                if (outputchkBkp == "SUCCESS")
                                                                                {
                                                                                    JObject liveRes = outputchk("Success", entry.operatorid, "", (decimal)remain, api.Yourrchid, orderid);
                                                                                    Recharge_info obj = (from p in db.Recharge_info where p.idno == idnn11 select p).Single();
                                                                                    obj.API_response = liveRes.ToString();
                                                                                    db.SaveChanges();
                                                                                    return Ok(liveRes);
                                                                                }
                                                                                else if (outputchkBkp == "FAILED")
                                                                                {
                                                                                    JObject liveRes = outputchk("Failed", "", entry.operatorid, (decimal)remain, api.Yourrchid, orderid);
                                                                                    Recharge_info obj = (from p in db.Recharge_info where p.idno == idnn11 select p).Single();
                                                                                    obj.API_response = liveRes.ToString();
                                                                                    db.SaveChanges();
                                                                                    return Ok(liveRes);
                                                                                }
                                                                                else
                                                                                {
                                                                                    JObject liverch = outputchk("Pending", "", "", (decimal)remain, api.Yourrchid, orderid);
                                                                                    return Ok(liverch);
                                                                                }
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            JObject rch = outputchk("Pending", "", Operatorid, (decimal)remain, api.Yourrchid, orderid);
                                                                            return Ok(rch);
                                                                        }
                                                                    }
                                                                    else if (url.ToUpper().Contains("VASTWEBINDIA.COM"))
                                                                    {
                                                                        dynamic stuff = Newtonsoft.Json.JsonConvert.DeserializeObject(webcontent);
                                                                        var status = stuff.Status.ToString();
                                                                        var mobile = stuff.Mobile.ToString();
                                                                        var amount1 = stuff.Amount.ToString();
                                                                        var R_id = stuff.RID.ToString();
                                                                        string operatorid = stuff.Operatorid.ToString();
                                                                        var remain_amount = stuff.remainamount.ToString();
                                                                        if (status.ToUpper() == "SUCCESS")
                                                                        {
                                                                            status = "Success";
                                                                            db.recharge_update(msgorderid, status, operatorid, Convert.ToDecimal(remain_amount), webcontent.ToString(), "Response");
                                                                            JObject rch = outputchk("Success", operatorid, "", (decimal)remain, api.Yourrchid, orderid);
                                                                            try
                                                                            {
                                                                                Recharge_info objj = (from p in db.Recharge_info where p.idno == idnn11 select p).Single();
                                                                                objj.API_response = rch.ToString();
                                                                                objj.response_output = "";
                                                                                objj.Api_Response_output = "";
                                                                                db.SaveChanges();
                                                                                return Ok(rch);
                                                                            }
                                                                            catch
                                                                            {
                                                                                return Ok(rch);
                                                                            }
                                                                        }
                                                                        else if (status.ToUpper() == "FAILED")
                                                                        {

                                                                            var optcodei = db.Operator_Code.Where(aa => aa.new_opt_code == api.Optcode).SingleOrDefault().Operator_id.ToString();
                                                                            var show = db.failed_recharge_move.Where(aa => aa.operator_code == optcodei && aa.status == "Y").SingleOrDefault();
                                                                            if (show == null)
                                                                            {
                                                                                status = "Failed";
                                                                                db.recharge_update(msgorderid, status, "NA", Convert.ToDecimal(0), webcontent, "Response");
                                                                                try
                                                                                {
                                                                                    var retailerdetails = db.api_user_details.Where(aa => aa.apiid == userid.UserId).SingleOrDefault();
                                                                                    var remdetails = db.api_remain_amount.Where(aa => aa.apiid == userid.UserId).SingleOrDefault();

                                                                                    var admininfo = db.Admin_details.SingleOrDefault();
                                                                                    Backupinfo back = new Backupinfo();
                                                                                    var model = new Backupinfo.Addinfo
                                                                                    {
                                                                                        Websitename = admininfo.WebsiteUrl,
                                                                                        RetailerID = userid.UserId,
                                                                                        Email = retailerdetails.emailid,
                                                                                        Mobile = retailerdetails.mobile,
                                                                                        Details = "Recharge Refund " + api.Customernumber + " Amount " + api.Amount,
                                                                                        RemainBalance = (decimal)remdetails.balance,
                                                                                        Usertype = "API"
                                                                                    };
                                                                                    back.Rechargeandutility(model);


                                                                                }
                                                                                catch { }
                                                                                JObject liveRes = outputchk("Failed", "", "Failed", (decimal)remain, api.Yourrchid, orderid);
                                                                                try
                                                                                {
                                                                                    Recharge_info objj = (from p in db.Recharge_info where p.idno == idnn11 select p).Single();
                                                                                    objj.API_response = liveRes.ToString();
                                                                                    objj.response_output = "";
                                                                                    objj.Api_Response_output = "";
                                                                                    db.SaveChanges();
                                                                                    return Ok(liveRes);
                                                                                }
                                                                                catch
                                                                                {
                                                                                    return Ok(liveRes);
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                var outputchkBkp = backup.recharge(api.Customernumber, api.Optcode, api.Amount, userid.UserId, idnn11, optcodei, ordersend, ref operatorid);
                                                                                var entry = (from rch in db.Recharge_info
                                                                                             where rch.idno == idnn11
                                                                                             select new
                                                                                             { operatorid = rch.OPt_id }).SingleOrDefault();
                                                                                if (outputchkBkp == "SUCCESS")
                                                                                {
                                                                                    JObject liveRes = outputchk("Success", entry.operatorid, "", (decimal)remain, api.Yourrchid, orderid);
                                                                                    Recharge_info obj = (from p in db.Recharge_info where p.idno == idnn11 select p).Single();
                                                                                    obj.API_response = liveRes.ToString();
                                                                                    db.SaveChanges();
                                                                                    return Ok(liveRes);
                                                                                }
                                                                                else if (outputchkBkp == "FAILED")
                                                                                {
                                                                                    JObject liveRes = outputchk("Failed", "", entry.operatorid, (decimal)remain, api.Yourrchid, orderid);
                                                                                    Recharge_info obj = (from p in db.Recharge_info where p.idno == idnn11 select p).Single();
                                                                                    obj.API_response = liveRes.ToString();
                                                                                    db.SaveChanges();
                                                                                    return Ok(liveRes);
                                                                                }
                                                                                else
                                                                                {
                                                                                    JObject liverch = outputchk("Pending", "", "", (decimal)remain, api.Yourrchid, orderid);
                                                                                    return Ok(liverch);
                                                                                }
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            JObject liverch = outputchk("Pending", "", "", (decimal)remain, api.Yourrchid, orderid);
                                                                            return Ok(liverch);
                                                                        }
                                                                    }
                                                                    else if (url.ToUpper().Contains("RECHARGETRAAWSERVICES.IN"))
                                                                    {

                                                                        dynamic stuff = Newtonsoft.Json.JsonConvert.DeserializeObject(webcontent);

                                                                        var status = stuff.status.ToString();

                                                                        string operatorid = stuff.operatorid.ToString();
                                                                        var amount1 = stuff.remainamount.ToString();

                                                                        if (amount1 == "" || amount1 == null)
                                                                        {
                                                                            amount1 = "0";
                                                                        }

                                                                        if (status.ToUpper() == "SUCCESS")
                                                                        {
                                                                            status = "Success";
                                                                            db.recharge_update(msgorderid, status, operatorid, Convert.ToDecimal(amount1), webcontent.ToString(), "Response");
                                                                            JObject rch = outputchk("Success", operatorid, "", (decimal)remain, api.Yourrchid, orderid);
                                                                            try
                                                                            {
                                                                                Recharge_info objj = (from p in db.Recharge_info where p.idno == idnn11 select p).Single();
                                                                                objj.API_response = rch.ToString();
                                                                                objj.response_output = "";
                                                                                objj.Api_Response_output = "";
                                                                                db.SaveChanges();
                                                                                return Ok(rch);
                                                                            }
                                                                            catch
                                                                            {
                                                                                return Ok(rch);
                                                                            }
                                                                        }
                                                                        else if (status.ToUpper() == "FAILED")
                                                                        {

                                                                            var optcodei = db.Operator_Code.Where(aa => aa.new_opt_code == api.Optcode).SingleOrDefault().Operator_id.ToString();
                                                                            var show = db.failed_recharge_move.Where(aa => aa.operator_code == optcodei && aa.status == "Y").SingleOrDefault();
                                                                            if (show == null)
                                                                            {
                                                                                status = "Failed";
                                                                                db.recharge_update(msgorderid, status, "NA", Convert.ToDecimal(0), webcontent, "Response");
                                                                                try
                                                                                {
                                                                                    var retailerdetails = db.api_user_details.Where(aa => aa.apiid == userid.UserId).SingleOrDefault();
                                                                                    var remdetails = db.api_remain_amount.Where(aa => aa.apiid == userid.UserId).SingleOrDefault();

                                                                                    var admininfo = db.Admin_details.SingleOrDefault();
                                                                                    Backupinfo back = new Backupinfo();
                                                                                    var model = new Backupinfo.Addinfo
                                                                                    {
                                                                                        Websitename = admininfo.WebsiteUrl,
                                                                                        RetailerID = userid.UserId,
                                                                                        Email = retailerdetails.emailid,
                                                                                        Mobile = retailerdetails.mobile,
                                                                                        Details = "Recharge Refund " + api.Customernumber + " Amount " + api.Amount,
                                                                                        RemainBalance = (decimal)remdetails.balance,
                                                                                        Usertype = "API"
                                                                                    };
                                                                                    back.Rechargeandutility(model);


                                                                                }
                                                                                catch { }
                                                                                JObject liveRes = outputchk("Failed", "", "Failed", (decimal)remain, api.Yourrchid, orderid);
                                                                                try
                                                                                {
                                                                                    Recharge_info objj = (from p in db.Recharge_info where p.idno == idnn11 select p).Single();
                                                                                    objj.API_response = liveRes.ToString();
                                                                                    objj.response_output = "";
                                                                                    objj.Api_Response_output = "";
                                                                                    db.SaveChanges();
                                                                                    return Ok(liveRes);
                                                                                }
                                                                                catch
                                                                                {
                                                                                    return Ok(liveRes);
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                var outputchkBkp = backup.recharge(api.Customernumber, api.Optcode, api.Amount, userid.UserId, idnn11, optcodei, ordersend, ref operatorid);
                                                                                var entry = (from rch in db.Recharge_info
                                                                                             where rch.idno == idnn11
                                                                                             select new
                                                                                             { operatorid = rch.OPt_id }).SingleOrDefault();
                                                                                if (outputchkBkp == "SUCCESS")
                                                                                {
                                                                                    JObject liveRes = outputchk("Success", entry.operatorid, "", (decimal)remain, api.Yourrchid, orderid);
                                                                                    Recharge_info obj = (from p in db.Recharge_info where p.idno == idnn11 select p).Single();
                                                                                    obj.API_response = liveRes.ToString();
                                                                                    db.SaveChanges();
                                                                                    return Ok(liveRes);
                                                                                }
                                                                                else if (outputchkBkp == "FAILED")
                                                                                {
                                                                                    JObject liveRes = outputchk("Failed", "", entry.operatorid, (decimal)remain, api.Yourrchid, orderid);
                                                                                    Recharge_info obj = (from p in db.Recharge_info where p.idno == idnn11 select p).Single();
                                                                                    obj.API_response = liveRes.ToString();
                                                                                    db.SaveChanges();
                                                                                    return Ok(liveRes);
                                                                                }
                                                                                else
                                                                                {
                                                                                    JObject liverch = outputchk("Pending", "", "", (decimal)remain, api.Yourrchid, orderid);
                                                                                    return Ok(liverch);
                                                                                }
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            JObject liverch = outputchk("Pending", "", "", (decimal)remain, api.Yourrchid, orderid);
                                                                            return Ok(liverch);
                                                                        }
                                                                    }
                                                                    else if (url.ToUpper().Contains("INSTANTPAY.IN"))
                                                                    {
                                                                        dynamic response = JsonConvert.DeserializeObject(webcontent.ToString());
                                                                        var RESCODE = response.res_code.ToString();
                                                                        var optid = response.opr_id.ToString();
                                                                        var openbal = response.opening_bal.ToString();
                                                                        decimal remainbal = Convert.ToDecimal(openbal);
                                                                        if (RESCODE == "TXN")
                                                                        {
                                                                            db.recharge_update(msgorderid, "SUCCESS", optid, remainbal, webcontent.ToString(), "Response");
                                                                            JObject rch = outputchk("Success", optid, "", (decimal)remain, api.Yourrchid, orderid);
                                                                            return Ok(rch);
                                                                        }
                                                                        else if (RESCODE.ToString() == "TUP")
                                                                        {
                                                                            JObject liverch = outputchk("Pending", "", "", (decimal)remain, api.Yourrchid, orderid);
                                                                            return Ok(liverch);
                                                                        }
                                                                        else
                                                                        {
                                                                            db.recharge_update(msgorderid, "FAILED", optid, remain, webcontent.ToString(), "Response");
                                                                            try
                                                                            {
                                                                                var retailerdetails = db.api_user_details.Where(aa => aa.apiid == userid.UserId).SingleOrDefault();
                                                                                var remdetails = db.api_remain_amount.Where(aa => aa.apiid == userid.UserId).SingleOrDefault();

                                                                                var admininfo = db.Admin_details.SingleOrDefault();
                                                                                Backupinfo back = new Backupinfo();
                                                                                var model = new Backupinfo.Addinfo
                                                                                {
                                                                                    Websitename = admininfo.WebsiteUrl,
                                                                                    RetailerID = userid.UserId,
                                                                                    Email = retailerdetails.emailid,
                                                                                    Mobile = retailerdetails.mobile,
                                                                                    Details = "Recharge Refund " + api.Customernumber + " Amount " + api.Amount,
                                                                                    RemainBalance = (decimal)remdetails.balance,
                                                                                    Usertype = "API"
                                                                                };
                                                                                back.Rechargeandutility(model);


                                                                            }
                                                                            catch { }
                                                                            JObject rch = outputchk("Failed", "", optid, (decimal)remain, api.Yourrchid, orderid);
                                                                            return Ok(rch);
                                                                        }
                                                                    }

                                                                    else
                                                                    {
                                                                        JObject rch = outputchk("Pending", "", "", (decimal)remain, api.Yourrchid, orderid);
                                                                        return Ok(rch);
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        JObject rch = outputchk("Failed", "", "Remain Balance Low.", (decimal)remain, api.Yourrchid, "");
                                                        return Ok(rch);
                                                    }
                                                }
                                                else
                                                {
                                                    JObject rch = outputchk("Failed", "", "Operator Code is invalid.", 0, api.Yourrchid, "");
                                                    return Ok(rch);
                                                }
                                            }
                                            else
                                            {
                                                JObject rch = outputchk("Failed", "", "Ip is Not Valid. IP: " + currentip, 0, api.Yourrchid, "");
                                                return Ok(rch);
                                            }
                                        }
                                        else
                                        {
                                            JObject rch = outputchk("Failed", "", "Invalid Token.", 0, api.Yourrchid, "");
                                            return Ok(rch);
                                        }
                                    }
                                    else
                                    {
                                        JObject rch = outputchk("Failed", "", "Token is Not Created.", 0, api.Yourrchid, "");
                                        return Ok(rch);
                                    }
                                }
                                else
                                {
                                    JObject rch = outputchk("Failed", "", "This User is Not Register With Us.", 0, api.Yourrchid, "");
                                    return Ok(rch);
                                }
                            }
                        }
                        else
                        {
                            JObject rch = outputchk("Failed", "", "Missing Parameter", 0, Yourrchid, "");
                            return Ok(rch);
                        }
                    }
                    catch (Exception ex)
                    {
                        test1 t1 = new test1();
                        t1.name = String.Format("Api Recharge Exception GET1 - Message: {0}, Stack: {1}, InnerMsg: {2}, InnerStack: {3}, Time: {4}", ex.Message, ex.StackTrace, ex.InnerException?.Message, ex.InnerException?.StackTrace, DateTime.Now.ToString());
                        db.test1.Add(t1);
                        db.SaveChanges();

                        WriteLog("Errormessage", "Errormessage" + ex.Message);
                        JObject rch = outputchk("Pending", "", ex.Message, 0, Yourrchid, "");
                        return Ok(rch);
                    }
                }
            }
            catch (Exception ex)
            {
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    test1 t1 = new test1();
                    t1.name = String.Format("Api Recharge Exception GET2 - Message: {0}, Stack: {1}, InnerMsg: {2}, InnerStack: {3}, Time: {4}", ex.Message, ex.StackTrace, ex.InnerException?.Message, ex.InnerException?.StackTrace, DateTime.Now.ToString());
                    db.test1.Add(t1);
                    db.SaveChanges();
                }

                WriteLog("Errormessage", "Errormessage" + ex.Message);
                JObject rch = outputchk("Pending", "", ex.Message, 0, Yourrchid, "");
                return Ok(rch);
            }
        }
        [AllowAnonymous]
        [HttpPost]
        [Route("Recharge/Recharge")]
        [CacheFilter(TimeDuration = 100)]
        public async Task<IHttpActionResult> Recharge(ApiRecharge api)
        {
            var corrId = string.Format("{0}{1}", DateTime.Now.Ticks, Thread.CurrentThread.ManagedThreadId);
            var requestInfo = string.Format("{0} {1}", Request.Method, Request.RequestUri);
            var requestMessage = Request.Content.ReadAsByteArrayAsync().Result;
            var apirequest = IncommingMessage(corrId, requestInfo, requestMessage);
            try
            {
                var reqss = JsonConvert.SerializeObject(api);
                WriteLog("APIREQUEST", "*************************************************************************************************************");
                WriteLog("APIREQUEST", "APIREQUEST " + reqss.ToString());
            }
            catch { }
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                try
                {
                    if (api.Customernumber != null && api.Amount > 0 && api.Optcode != null && api.Yourrchid != null && api.Tokenid != null && api.UserID != null)
                    {
                        var operatorname = db.Operator_Code.Where(a => a.new_opt_code == api.Optcode).SingleOrDefault()?.operator_Name;

                        if (operatorname.ToUpper().StartsWith("LIFE INSURANCE CORPORATION"))
                        {
                            api.optional1 = db.api_user_details.Where(a => a.apiid == api.UserID).Select(a => a.emailid).SingleOrDefault();
                        }

                        if (String.IsNullOrEmpty(operatorname))
                        {
                            JObject rch = outputchk("Failed", "", String.Format("Operator Code: {0} does not exist.", api.Optcode), 0, api.Yourrchid, "");
                            return Ok(rch);
                        }
                        var userid = db.Users.Where(aa => aa.UserName.ToUpper() == api.UserID.ToUpper()).SingleOrDefault();
                        var roleid = db.UserRoles.Where(x => x.UserId == userid.UserId).SingleOrDefault().RoleId;
                        var UserRole = db.Roles.Where(x => x.RoleId == roleid).SingleOrDefault().Name;
                        var blockamount = db.AmountBlockunblocks.Where(a => a.OperatorName == operatorname && a.Amount == api.Amount && a.status == "Y" && (a.User == UserRole || a.User == null)).ToList();
                        if (blockamount.Count > 0)
                        {
                            JObject rch = outputchk("Failed", "", "This Amount is Blocked Please Contact by Admin.", 0, api.Yourrchid, "");
                            return Ok(rch);

                        }
                        else
                        {
                            if (userid != null)
                            {
                                var tokenchk = db.Api_ip_address.Where(aa => aa.userid == userid.UserId && aa.token == api.Tokenid).SingleOrDefault();
                                if (tokenchk != null)
                                {
                                    if (tokenchk.token == api.Tokenid)
                                    {
                                        var key = userid.UserId.Substring(0, 16);
                                        var usedip = Decrypt(tokenchk.token, key);
                                        var currentip = GetComputer_InternetIP();

                                        //currentip = Request.UserHostAddress;
                                        if (currentip == "::1")
                                        {
                                            currentip = new WebClient().DownloadString("http://ipv4.icanhazip.com/");
                                            currentip = currentip.Replace("\n", "");
                                        }


                                        if (usedip == currentip)
                                        {
                                            var optchk = db.Operator_Code.Where(aa => aa.new_opt_code == api.Optcode).SingleOrDefault();
                                            if (optchk != null)
                                            {
                                                var remain = db.api_remain_amount.Where(aa => aa.apiid == userid.UserId).SingleOrDefault().balance;
                                                if (remain >= api.Amount)
                                                {
                                                    System.Data.Entity.Core.Objects.ObjectParameter orderidinfo = new
                                                 System.Data.Entity.Core.Objects.ObjectParameter("outputorderid", typeof(string));
                                                    System.Data.Entity.Core.Objects.ObjectParameter msg = new
                                                                    System.Data.Entity.Core.Objects.ObjectParameter("output", typeof(string));
                                                    //string ordersend = "R" + DateTime.Parse(DateTime.Now.ToString()).ToString("yyMMddHHmmss") + RandomString(4);
                                                    string ordersend = DateTime.Parse(DateTime.Now.ToString()).ToString("yyMMddHHmmss") + RandomString(4);
                                                    decimal aammm = Convert.ToDecimal(api.Amount);
                                                    var chkstateswitch_list = db.swich_state_operator.Where(aa => aa.rch_from.ToUpper() == "API" && aa.optid == api.Optcode && (aa.users.ToUpper() == "ALL" || aa.users == userid.UserId) && aa.sts == "Y" && (aa.amount <= aammm && aa.maxamount >= aammm)).ToList();
                                                    var localrch = "NOT";
                                                    if (chkstateswitch_list.Count > 0)
                                                    {
                                                        try
                                                        {
                                                            var best = db.Bestoffers.Single();
                                                            //var client = new RestClient("https://catalog.paytm.com/v1/mobile/getopcirclebyrange?number=" + mobileno + "&client=androidapp&version=6.6.1&child_site_id=1&site_id=1");
                                                            var client = new RestClient("https://www.vastwebindia.com/Offer/GetInfo?userid=" + best.Emailid + "&token=" + best.token + "&number=" + api.Customernumber);
                                                            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                                                            var request = new RestRequest(Method.GET);
                                                            IRestResponse response = client.Execute(request);
                                                            dynamic stuff = JsonConvert.DeserializeObject(response.Content);
                                                            string state = (string)stuff.Response.Circle;
                                                            var chkstateswitch = chkstateswitch_list.Where(s => s.stateid.ToLower() == state.ToLower()).FirstOrDefault();
                                                            if (chkstateswitch != null)
                                                            {
                                                                localrch = "DONE|" + chkstateswitch.api_name;
                                                            }
                                                        }
                                                        catch { }
                                                    }
                                                    var duedate = "";
                                                    if (optchk.Operator_type.ToUpper() == "BROADBAND" || optchk.Operator_type.ToUpper() == "ELECTRICITY"
                                                        || optchk.Operator_type.ToUpper() == "FASTAG" || optchk.Operator_type.ToUpper() == "GAS"
                                                        || optchk.Operator_type.ToUpper() == "LANDLINE" || optchk.Operator_type.ToUpper() == "LOAN"
                                                        || optchk.Operator_type.ToUpper() == "LPG GAS" || optchk.Operator_type.ToUpper() == "WATER")
                                                    {
                                                        //View Bill
                                                        Vastbillpay vb = new Vastbillpay();
                                                        VastBazaartoken Responsetoken = new VastBazaartoken();
                                                        var tokn = Responsetoken.gettoken();
                                                        string CommonTranid = "E" + DateTime.Parse(DateTime.Now.ToString()).ToString("yyMMddHHmmss") + RandomString(4);
                                                        var apioptcode = db.SRS_API.Where(aa => aa.api.ToUpper().Contains("API.VASTBAZAAR.COM") && aa.opt_code == api.Optcode).SingleOrDefault().apioptcode;
                                                        string amtt = api.Amount.ToString();
                                                        var responsechk1 = vb.Viewbill(tokn, api.Customernumber, apioptcode, amtt, api.optional1, api.optional2, CommonTranid);
                                                        var sts = responsechk1.StatusCode.ToString();
                                                        if (sts == "OK")
                                                        {
                                                            var responsechk = responsechk1.Content.ToString();
                                                            dynamic json = JsonConvert.DeserializeObject(responsechk);
                                                            var respcode = json.Content.ResponseCode.ToString();
                                                            var ADDINFO = json.Content.ADDINFO.ToString();
                                                            dynamic json1 = JsonConvert.DeserializeObject(ADDINFO);
                                                            if (Convert.ToBoolean(json1.IsSuccess))
                                                            {
                                                                try
                                                                {
                                                                    duedate = Convert.ToString(json1.BillInfo.billDueDate);
                                                                }
                                                                catch { }
                                                            }
                                                        }
                                                    }

                                                    var chkout = db.Recharge_API_new(userid.UserId, api.Customernumber, api.Optcode, api.Amount, "API", api.Yourrchid, ordersend, api.optional1, api.optional2, api.optional3, localrch, currentip, apirequest,duedate, orderidinfo, msg).ToList();
                                                    try
                                                    {
                                                        var retailerdetails = db.api_user_details.Where(aa => aa.apiid == userid.UserId).SingleOrDefault();
                                                        var remdetails = db.api_remain_amount.Where(aa => aa.apiid == userid.UserId).SingleOrDefault();

                                                        var admininfo = db.Admin_details.SingleOrDefault();
                                                        Backupinfo back = new Backupinfo();
                                                        var model = new Backupinfo.Addinfo
                                                        {
                                                            Websitename = admininfo.WebsiteUrl,
                                                            RetailerID = userid.UserId,
                                                            Email = retailerdetails.emailid,
                                                            Mobile = retailerdetails.mobile,
                                                            Details = "Recharge " + api.Customernumber + " Amount " + api.Amount,
                                                            RemainBalance = (decimal)remdetails.balance,
                                                            Usertype = "API"
                                                        };
                                                        back.Rechargeandutility(model);


                                                    }
                                                    catch { }
                                                    var msgoutput = chkout.SingleOrDefault().msg;
                                                    var orderid = chkout.SingleOrDefault().orderinfo;
                                                    try
                                                    {
                                                        WriteLog("APIREQUEST", "Message" + msgoutput);
                                                    }
                                                    catch { }
                                                    if (msgoutput == "APIIDBLOCK")
                                                    {
                                                        decimal am = 0;
                                                        JObject rch = outputchk("Failed", "", "Your API ID is Block.", am, api.Yourrchid, "");
                                                        return Ok(rch);
                                                    }
                                                    else if (msgoutput == "OPTBLOCK")
                                                    {
                                                        decimal am = 0;
                                                        JObject rch = outputchk("Failed", "", "This Operator is Block.", am, api.Yourrchid, "");
                                                        return Ok(rch);
                                                    }
                                                    else if (msgoutput == "APIOPTBLOCK")
                                                    {
                                                        decimal am = 0;
                                                        JObject rch = outputchk("Failed", "", "This Operator is Block.", am, api.Yourrchid, "");
                                                        return Ok(rch);
                                                    }
                                                    else if (msgoutput == "LOWBAL")
                                                    {

                                                        decimal am = 0;
                                                        JObject rch = outputchk("Failed", "", "Remain Balance Low.", am, api.Yourrchid, "");
                                                        return Ok(rch);
                                                    }
                                                    else if (msgoutput == "LOWCAPPING")
                                                    {
                                                        decimal am = 0;
                                                        JObject rch = outputchk("Failed", "", "Capping Low.", am, api.Yourrchid, "");
                                                        return Ok(rch);
                                                    }
                                                    else if (msgoutput == "MINMAXBAL")
                                                    {
                                                        decimal am = 0;
                                                        JObject rch = outputchk("Failed", "", "Number is Not Valid.", am, api.Yourrchid, "");
                                                        return Ok(rch);


                                                    }
                                                    else if (msgoutput == "TIMEDIFF")
                                                    {
                                                        decimal am = 0;
                                                        JObject rch = outputchk("Pending", "", "Same Number And Same Amount Allready Success .", am, api.Yourrchid, "");
                                                        return Ok(rch);
                                                    }
                                                    else if (msgoutput == "INVALIDINCOME")
                                                    {
                                                        decimal am = 0;

                                                        JObject rch = outputchk("Failed", "", "Remain Balance Low(Invalid).", am, api.Yourrchid, "");
                                                        return Ok(rch);
                                                    }
                                                    else if (msgoutput == "MOK")
                                                    {
                                                        remain = db.api_remain_amount.Where(aa => aa.apiid == userid.UserId).SingleOrDefault().balance;
                                                        var msgorderid = db.Recharge_info.Where(aa => aa.Order_id == orderid).OrderByDescending(aa => aa.idno).Take(1).SingleOrDefault().idno.ToString();


                                                        JObject rch = outputchk("Pending", "", "", (decimal)remain, api.Yourrchid, orderid);
                                                        return Ok(rch);
                                                    }
                                                    else if (msgoutput == "MOK" || msgoutput == "AOK" || msgoutput == "OKK")
                                                    {
                                                        remain = db.api_remain_amount.Where(aa => aa.apiid == userid.UserId).SingleOrDefault().balance;
                                                        var msgorderid = db.Recharge_info.Where(aa => aa.Order_id == orderid).OrderByDescending(aa => aa.idno).Take(1).SingleOrDefault().idno.ToString();


                                                        JObject rch = outputchk("Pending", "", "", (decimal)remain, api.Yourrchid, orderid);
                                                        return Ok(rch);
                                                    }
                                                    else
                                                    {
                                                        remain = db.api_remain_amount.Where(aa => aa.apiid == userid.UserId).SingleOrDefault().balance;
                                                        var msgorderid = db.Recharge_info.Where(aa => aa.Order_id == orderid).OrderByDescending(aa => aa.idno).Take(1).SingleOrDefault().idno.ToString();
                                                        int idnn11 = Convert.ToInt32(msgorderid);
                                                        var url = msgoutput.Replace("AOK", "");
                                                        if (url.ToUpper().Contains("API.VASTBAZAAR.COM"))
                                                        {
                                                            string CommonTranid = ordersend;//"E" + DateTime.Parse(DateTime.Now.ToString()).ToString("yyMMddHHmmss") + RandomString(4);
                                                            var apioptcode = db.SRS_API.Where(aa => aa.api.ToUpper().Contains("API.VASTBAZAAR.COM") && aa.opt_code == api.Optcode).SingleOrDefault().apioptcode;
                                                            var tokn = db.vastbazzartokens.SingleOrDefault();
                                                            if (tokn == null)
                                                            {
                                                                var response = tokencheck();
                                                                var responsechk = response.Content.ToString();
                                                                var responsecode = response.StatusCode.ToString();
                                                                if (responsecode == "Ok")
                                                                {
                                                                    Vastbillpay vb = new Vastbillpay();
                                                                    dynamic json = JsonConvert.DeserializeObject(responsechk);
                                                                    var token = json.access_token.ToString();
                                                                    var expire = json[".expires"].ToString();
                                                                    DateTime exp = Convert.ToDateTime(expire);
                                                                    vastbazzartoken vast = new vastbazzartoken();
                                                                    vast.apitoken = token;
                                                                    vast.exptime = exp;
                                                                    db.vastbazzartokens.Add(vast);
                                                                    db.SaveChanges();
                                                                    var responsechk1 = vb.billpay(token, api.Customernumber, apioptcode, api.Amount.ToString(), api.optional1, api.optional2, CommonTranid);
                                                                    responsechk = responsechk1.Content.ToString();

                                                                    var Request = responsechk1.Request.Parameters[2].Value;
                                                                    var ReqUrl = url + "  RequestBody : " + Request;

                                                                    json = JsonConvert.DeserializeObject(responsechk);
                                                                    var respcode = json.Content.ResponseCode.ToString();
                                                                    var ADDINFO = json.Content.ADDINFO.ToString();
                                                                    dynamic json1 = JsonConvert.DeserializeObject(ADDINFO);

                                                                    Recharge_info objCourse = (from p in db.Recharge_info where p.idno == idnn11 select p).Single();
                                                                    objCourse.Recharge_response = json1.ToString();
                                                                    objCourse.Recharge_request = ReqUrl;
                                                                    db.SaveChanges();

                                                                    var status = json1.STATUS.ToString();
                                                                    decimal PRICE = 0;
                                                                    var errormsg = json1.ERRORMSG.ToString();
                                                                    string TRANSID = json1.TRANSID.ToString();

                                                                    if (status == "Success")
                                                                    {
                                                                        PRICE = Convert.ToDecimal(json1.PRICE.ToString());
                                                                        status = "Success";
                                                                        db.recharge_update(msgorderid, status, TRANSID, PRICE, json.ToString(), "Response");

                                                                        JObject liveRes = outputchk("Success", TRANSID, "", (decimal)remain, api.Yourrchid, orderid);
                                                                        try
                                                                        {
                                                                            Recharge_info objj = (from p in db.Recharge_info where p.idno == idnn11 select p).Single();
                                                                            objj.API_response = liveRes.ToString();
                                                                            objj.response_output = "";
                                                                            objj.Api_Response_output = "";
                                                                            db.SaveChanges();
                                                                            return Ok(liveRes);
                                                                        }
                                                                        catch
                                                                        {
                                                                            return Ok(liveRes);
                                                                        }
                                                                    }
                                                                    else if (status == "Failed")
                                                                    {
                                                                        var optcodei = db.Operator_Code.Where(aa => aa.new_opt_code == api.Optcode).SingleOrDefault().Operator_id.ToString();
                                                                        var show = db.failed_recharge_move.Where(aa => aa.operator_code == optcodei && aa.status == "Y").SingleOrDefault();
                                                                        if (show == null)
                                                                        {
                                                                            status = "Failed";
                                                                            db.recharge_update(msgorderid, status, errormsg, PRICE, json.ToString(), "Response");
                                                                            try
                                                                            {
                                                                                var retailerdetails = db.api_user_details.Where(aa => aa.apiid == userid.UserId).SingleOrDefault();
                                                                                var remdetails = db.api_remain_amount.Where(aa => aa.apiid == userid.UserId).SingleOrDefault();

                                                                                var admininfo = db.Admin_details.SingleOrDefault();
                                                                                Backupinfo back = new Backupinfo();
                                                                                var model = new Backupinfo.Addinfo
                                                                                {
                                                                                    Websitename = admininfo.WebsiteUrl,
                                                                                    RetailerID = userid.UserId,
                                                                                    Email = retailerdetails.emailid,
                                                                                    Mobile = retailerdetails.mobile,
                                                                                    Details = "Recharge Refund " + api.Customernumber + " Amount " + api.Amount,
                                                                                    RemainBalance = (decimal)remdetails.balance,
                                                                                    Usertype = "API"
                                                                                };
                                                                                back.Rechargeandutility(model);


                                                                            }
                                                                            catch { }
                                                                            JObject liveRes = outputchk("Failed", TRANSID, errormsg, (decimal)remain, api.Yourrchid, orderid);
                                                                            try
                                                                            {
                                                                                Recharge_info objj = (from p in db.Recharge_info where p.idno == idnn11 select p).Single();
                                                                                objj.API_response = liveRes.ToString();
                                                                                objj.response_output = "";
                                                                                objj.Api_Response_output = "";
                                                                                db.SaveChanges();
                                                                                return Ok(liveRes);
                                                                            }
                                                                            catch
                                                                            {
                                                                                return Ok(liveRes);
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            var outputchkBkp = backup.recharge(api.Customernumber, api.Optcode, api.Amount, userid.UserId, idnn11, optcodei, ordersend, ref TRANSID);
                                                                            var entry = (from rch in db.Recharge_info
                                                                                         where rch.idno == idnn11
                                                                                         select new
                                                                                         { operatorid = rch.OPt_id }).SingleOrDefault();
                                                                            if (outputchkBkp == "SUCCESS")
                                                                            {
                                                                                JObject liveRes = outputchk("Success", entry.operatorid, "", (decimal)remain, api.Yourrchid, orderid);
                                                                                Recharge_info obj = (from p in db.Recharge_info where p.idno == idnn11 select p).Single();
                                                                                obj.API_response = liveRes.ToString();
                                                                                db.SaveChanges();
                                                                                return Ok(liveRes);
                                                                            }
                                                                            else if (outputchkBkp == "FAILED")
                                                                            {
                                                                                JObject liveRes = outputchk("Failed", "", entry.operatorid, (decimal)remain, api.Yourrchid, orderid);
                                                                                Recharge_info obj = (from p in db.Recharge_info where p.idno == idnn11 select p).Single();
                                                                                obj.API_response = liveRes.ToString();
                                                                                db.SaveChanges();
                                                                                return Ok(liveRes);
                                                                            }
                                                                            else
                                                                            {
                                                                                JObject liverch = outputchk("Pending", "", "", (decimal)remain, api.Yourrchid, orderid);
                                                                                return Ok(liverch);
                                                                            }
                                                                        }
                                                                    }
                                                                    else
                                                                    {


                                                                        JObject rch = outputchk("Pending", TRANSID, errormsg, (decimal)remain, api.Yourrchid, orderid);
                                                                        return Ok(rch);
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    var status = "Failed";
                                                                    dynamic json = JsonConvert.DeserializeObject(responsechk);
                                                                    var error = json.error.ToString();
                                                                    var error_decribe = json["error_description"].ToString();
                                                                    db.recharge_update(msgorderid, status, error_decribe, remain, json.ToString(), "Response");


                                                                    JObject rch = outputchk("Failed", "", error_decribe, (decimal)remain, api.Yourrchid, orderid);
                                                                    return Ok(rch);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                Vastbillpay vb = new Vastbillpay();
                                                                DateTime curntdate = DateTime.Now.Date;
                                                                DateTime expdate = Convert.ToDateTime(tokn.exptime).Date;
                                                                if (expdate > curntdate)
                                                                {
                                                                    var responsechk1 = vb.billpay(tokn.apitoken, api.Customernumber, apioptcode, api.Amount.ToString(), api.optional1, api.optional2, CommonTranid);
                                                                    var responsechk = responsechk1.Content.ToString();

                                                                    var Request = responsechk1.Request.Parameters[2].Value;
                                                                    var ReqUrl = url + "  RequestBody : " + Request;

                                                                    dynamic json = JsonConvert.DeserializeObject(responsechk);
                                                                    var respcode = json.Content.ResponseCode.ToString();
                                                                    var ADDINFO = json.Content.ADDINFO.ToString();
                                                                    dynamic json1 = JsonConvert.DeserializeObject(ADDINFO);

                                                                    Recharge_info objCourse = (from p in db.Recharge_info where p.idno == idnn11 select p).Single();
                                                                    objCourse.Recharge_request = ReqUrl;
                                                                    db.SaveChanges();

                                                                    var status = json1.STATUS.ToString();
                                                                    decimal PRICE = 0;
                                                                    var errormsg = json1.ERRORMSG.ToString();
                                                                    string TRANSID = json1.TRANSID.ToString();

                                                                    if (status == "Success")
                                                                    {
                                                                        PRICE = Convert.ToDecimal(json1.PRICE.ToString());
                                                                        status = "Success";
                                                                        db.recharge_update(msgorderid, status, TRANSID, PRICE, json.ToString(), "Response");

                                                                        JObject liveRes = outputchk("Success", TRANSID, "", (decimal)remain, api.Yourrchid, orderid);
                                                                        try
                                                                        {
                                                                            Recharge_info objj = (from p in db.Recharge_info where p.idno == idnn11 select p).Single();
                                                                            objj.API_response = liveRes.ToString();
                                                                            objj.response_output = "";
                                                                            objj.Api_Response_output = "";
                                                                            db.SaveChanges();
                                                                            return Ok(liveRes);
                                                                        }
                                                                        catch
                                                                        {
                                                                            return Ok(liveRes);
                                                                        }
                                                                    }
                                                                    else if (status == "Failed")
                                                                    {
                                                                        var optcodei = db.Operator_Code.Where(aa => aa.new_opt_code == api.Optcode).SingleOrDefault().Operator_id.ToString();
                                                                        var show = db.failed_recharge_move.Where(aa => aa.operator_code == optcodei && aa.status == "Y").SingleOrDefault();
                                                                        if (show == null)
                                                                        {
                                                                            status = "Failed";
                                                                            db.recharge_update(msgorderid, status, errormsg, PRICE, json.ToString(), "Response");
                                                                            try
                                                                            {
                                                                                var retailerdetails = db.api_user_details.Where(aa => aa.apiid == userid.UserId).SingleOrDefault();
                                                                                var remdetails = db.api_remain_amount.Where(aa => aa.apiid == userid.UserId).SingleOrDefault();

                                                                                var admininfo = db.Admin_details.SingleOrDefault();
                                                                                Backupinfo back = new Backupinfo();
                                                                                var model = new Backupinfo.Addinfo
                                                                                {
                                                                                    Websitename = admininfo.WebsiteUrl,
                                                                                    RetailerID = userid.UserId,
                                                                                    Email = retailerdetails.emailid,
                                                                                    Mobile = retailerdetails.mobile,
                                                                                    Details = "Recharge Refund " + api.Customernumber + " Amount " + api.Amount,
                                                                                    RemainBalance = (decimal)remdetails.balance,
                                                                                    Usertype = "API"
                                                                                };
                                                                                back.Rechargeandutility(model);


                                                                            }
                                                                            catch { }
                                                                            JObject liveRes = outputchk("Failed", TRANSID, errormsg, (decimal)remain, api.Yourrchid, orderid);
                                                                            try
                                                                            {
                                                                                Recharge_info objj = (from p in db.Recharge_info where p.idno == idnn11 select p).Single();
                                                                                objj.API_response = liveRes.ToString();
                                                                                objj.response_output = "";
                                                                                objj.Api_Response_output = "";
                                                                                db.SaveChanges();
                                                                                return Ok(liveRes);
                                                                            }
                                                                            catch
                                                                            {
                                                                                return Ok(liveRes);
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            var outputchkBkp = backup.recharge(api.Customernumber, api.Optcode, api.Amount, userid.UserId, idnn11, optcodei, ordersend, ref TRANSID);
                                                                            var entry = (from rch in db.Recharge_info
                                                                                         where rch.idno == idnn11
                                                                                         select new
                                                                                         { operatorid = rch.OPt_id }).SingleOrDefault();
                                                                            if (outputchkBkp == "SUCCESS")
                                                                            {
                                                                                JObject liveRes = outputchk("Success", entry.operatorid, "", (decimal)remain, api.Yourrchid, orderid);
                                                                                Recharge_info obj = (from p in db.Recharge_info where p.idno == idnn11 select p).Single();
                                                                                obj.API_response = liveRes.ToString();
                                                                                db.SaveChanges();
                                                                                return Ok(liveRes);
                                                                            }
                                                                            else if (outputchkBkp == "FAILED")
                                                                            {
                                                                                JObject liveRes = outputchk("Failed", "", entry.operatorid, (decimal)remain, api.Yourrchid, orderid);
                                                                                Recharge_info obj = (from p in db.Recharge_info where p.idno == idnn11 select p).Single();
                                                                                obj.API_response = liveRes.ToString();
                                                                                db.SaveChanges();
                                                                                return Ok(liveRes);
                                                                            }
                                                                            else
                                                                            {
                                                                                JObject liverch = outputchk("Pending", "", "", (decimal)remain, api.Yourrchid, orderid);
                                                                                return Ok(liverch);
                                                                            }
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        JObject rch = outputchk("Pending", TRANSID, errormsg, (decimal)remain, api.Yourrchid, orderid);
                                                                        return Ok(rch);
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    var response = tokencheck();
                                                                    var responsechk = response.Content.ToString();
                                                                    var responsecode = response.StatusCode.ToString();
                                                                    if (responsecode == "Ok")
                                                                    {
                                                                        dynamic json = JsonConvert.DeserializeObject(responsechk);
                                                                        var token = json.access_token.ToString();
                                                                        var expire = json[".expires"].ToString();
                                                                        DateTime exp = Convert.ToDateTime(expire);
                                                                        tokn.apitoken = token;
                                                                        tokn.exptime = exp;
                                                                        db.SaveChanges();
                                                                        var responsechk1 = vb.billpay(token, api.Customernumber, apioptcode, api.Amount.ToString(), api.optional1, api.optional2, CommonTranid);
                                                                        responsechk = responsechk1.Content.ToString();

                                                                        var Request = responsechk1.Request.Parameters[2].Value;
                                                                        var ReqUrl = url + "  RequestBody : " + Request;

                                                                        json = JsonConvert.DeserializeObject(responsechk);
                                                                        var respcode = json.Content.ResponseCode.ToString();
                                                                        var ADDINFO = json.Content.ADDINFO.ToString();
                                                                        dynamic json1 = JsonConvert.DeserializeObject(ADDINFO);

                                                                        Recharge_info objCourse = (from p in db.Recharge_info where p.idno == idnn11 select p).Single();
                                                                        objCourse.Recharge_request = ReqUrl;
                                                                        db.SaveChanges();

                                                                        var status = json1.STATUS.ToString();
                                                                        decimal PRICE = 0;
                                                                        var errormsg = json1.ERRORMSG.ToString();
                                                                        string TRANSID = json1.TRANSID.ToString();

                                                                        db.SaveChanges();
                                                                        if (status == "Success")
                                                                        {
                                                                            PRICE = Convert.ToDecimal(json1.PRICE.ToString());
                                                                            status = "Success";
                                                                            db.recharge_update(msgorderid, status, TRANSID, PRICE, json.ToString(), "Response");

                                                                            JObject liveRes = outputchk("Success", TRANSID, "", (decimal)remain, api.Yourrchid, orderid);
                                                                            try
                                                                            {
                                                                                Recharge_info objj = (from p in db.Recharge_info where p.idno == idnn11 select p).Single();
                                                                                objj.API_response = liveRes.ToString();
                                                                                objj.response_output = "";
                                                                                objj.Api_Response_output = "";
                                                                                db.SaveChanges();
                                                                                return Ok(liveRes);
                                                                            }
                                                                            catch
                                                                            {
                                                                                return Ok(liveRes);
                                                                            }
                                                                        }
                                                                        else if (status == "Failed")
                                                                        {
                                                                            var optcodei = db.Operator_Code.Where(aa => aa.new_opt_code == api.Optcode).SingleOrDefault().Operator_id.ToString();
                                                                            var show = db.failed_recharge_move.Where(aa => aa.operator_code == optcodei && aa.status == "Y").SingleOrDefault();
                                                                            if (show == null)
                                                                            {
                                                                                status = "Failed";
                                                                                db.recharge_update(msgorderid, status, errormsg, PRICE, json.ToString(), "Response");
                                                                                try
                                                                                {
                                                                                    var retailerdetails = db.api_user_details.Where(aa => aa.apiid == userid.UserId).SingleOrDefault();
                                                                                    var remdetails = db.api_remain_amount.Where(aa => aa.apiid == userid.UserId).SingleOrDefault();

                                                                                    var admininfo = db.Admin_details.SingleOrDefault();
                                                                                    Backupinfo back = new Backupinfo();
                                                                                    var model = new Backupinfo.Addinfo
                                                                                    {
                                                                                        Websitename = admininfo.WebsiteUrl,
                                                                                        RetailerID = userid.UserId,
                                                                                        Email = retailerdetails.emailid,
                                                                                        Mobile = retailerdetails.mobile,
                                                                                        Details = "Recharge Refund " + api.Customernumber + " Amount " + api.Amount,
                                                                                        RemainBalance = (decimal)remdetails.balance,
                                                                                        Usertype = "API "
                                                                                    };
                                                                                    back.Rechargeandutility(model);


                                                                                }
                                                                                catch { }
                                                                                JObject liveRes = outputchk("Failed", TRANSID, errormsg, (decimal)remain, api.Yourrchid, orderid);
                                                                                try
                                                                                {
                                                                                    Recharge_info objj = (from p in db.Recharge_info where p.idno == idnn11 select p).Single();
                                                                                    objj.API_response = liveRes.ToString();
                                                                                    objj.response_output = "";
                                                                                    objj.Api_Response_output = "";
                                                                                    db.SaveChanges();
                                                                                    return Ok(liveRes);
                                                                                }
                                                                                catch
                                                                                {
                                                                                    return Ok(liveRes);
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                var outputchkBkp = backup.recharge(api.Customernumber, api.Optcode, api.Amount, userid.UserId, idnn11, optcodei, ordersend, ref TRANSID);
                                                                                var entry = (from rch in db.Recharge_info
                                                                                             where rch.idno == idnn11
                                                                                             select new
                                                                                             { operatorid = rch.OPt_id }).SingleOrDefault();
                                                                                if (outputchkBkp == "SUCCESS")
                                                                                {
                                                                                    JObject liveRes = outputchk("Success", entry.operatorid, "", (decimal)remain, api.Yourrchid, orderid);
                                                                                    Recharge_info obj = (from p in db.Recharge_info where p.idno == idnn11 select p).Single();
                                                                                    obj.API_response = liveRes.ToString();
                                                                                    db.SaveChanges();
                                                                                    return Ok(liveRes);
                                                                                }
                                                                                else if (outputchkBkp == "FAILED")
                                                                                {
                                                                                    JObject liveRes = outputchk("Failed", "", entry.operatorid, (decimal)remain, api.Yourrchid, orderid);
                                                                                    Recharge_info obj = (from p in db.Recharge_info where p.idno == idnn11 select p).Single();
                                                                                    obj.API_response = liveRes.ToString();
                                                                                    db.SaveChanges();
                                                                                    return Ok(liveRes);
                                                                                }
                                                                                else
                                                                                {
                                                                                    JObject liverch = outputchk("Pending", "", "", (decimal)remain, api.Yourrchid, orderid);
                                                                                    return Ok(liverch);
                                                                                }
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            JObject rch = outputchk("Pending", TRANSID, errormsg, (decimal)remain, api.Yourrchid, orderid);
                                                                            return Ok(rch);
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        var status = "Failed";
                                                                        dynamic json = JsonConvert.DeserializeObject(responsechk);
                                                                        var error = json.error.ToString();
                                                                        var error_decribe = json["error_description"].ToString();
                                                                        db.recharge_update(msgorderid, status, error_decribe, api.Amount, json.ToString(), "Response");


                                                                        JObject rch = outputchk("Failed", "", error_decribe, (decimal)remain, api.Yourrchid, orderid);
                                                                        return Ok(rch);
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        else if (url.ToUpper().Contains("RECHARGE/RECHARGE"))
                                                        {
                                                            var tkn = db.Recharge_API_URLS.Where(aa => aa.url == url).SingleOrDefault();
                                                            var token = tkn.Token;
                                                            var Tokenid = tkn.Token;
                                                            var Userid = tkn.API_ID;
                                                            POST_API PA = new POST_API();
                                                            idnn11 = 0;
                                                            decimal amt = Convert.ToDecimal(api.Amount);
                                                            msgorderid = db.Recharge_info.Where(aa => aa.Order_id == orderid).OrderByDescending(aa => aa.idno).Take(1).SingleOrDefault().idno.ToString();
                                                            idnn11 = Convert.ToInt32(msgorderid);
                                                            string CommonTranid = ordersend;//"E" + DateTime.Parse(DateTime.Now.ToString()).ToString("yyMMddHHmmss") + PA.RandomString(4);
                                                            var apioptcode = db.SRS_API.Where(aa => aa.api == url && aa.opt_code == api.Optcode).SingleOrDefault().apioptcode;
                                                            var responsechk1 = PA.RchReq(token, api.Customernumber, Tokenid, Userid, amt, apioptcode, CommonTranid, api.optional1, api.optional2, url);
                                                            var responsechk = responsechk1.Content.ToString();
                                                            try
                                                            {
                                                                WriteLog("APIREQUEST", "Response json" + responsechk);
                                                            }
                                                            catch { }
                                                            dynamic json = JsonConvert.DeserializeObject(responsechk);

                                                            var Request = responsechk1.Request.Parameters[2].Value;
                                                            var ReqUrl = url + "  RequestBody : " + Request;
                                                            Recharge_info objCourse = (from p in db.Recharge_info where p.idno == idnn11 select p).Single();
                                                            objCourse.Recharge_response = json.ToString();
                                                            objCourse.Recharge_request = ReqUrl;
                                                            db.SaveChanges();
                                                            var status = json.Status.ToString();
                                                            string Transid = json.Transid.ToString();
                                                            var errormsg = json.Errormsg.ToString();
                                                            var remainamount = json.Remain.ToString();
                                                            var Yourrchid = json.Yourrchid.ToString();
                                                            var RechargeID = json.RechargeID.ToString();
                                                            if (status.ToUpper() == "SUCCESS")
                                                            {
                                                                status = "Success";
                                                                db.recharge_update(msgorderid, status, Transid, Convert.ToDecimal(remainamount), json.ToString(), "Response");
                                                                JObject liveRes = outputchk("Success", Transid, "", (decimal)remain, api.Yourrchid, orderid);
                                                                try
                                                                {
                                                                    Recharge_info objj = (from p in db.Recharge_info where p.idno == idnn11 select p).Single();
                                                                    objj.API_response = liveRes.ToString();
                                                                    objj.response_output = "";
                                                                    objj.Api_Response_output = "";
                                                                    db.SaveChanges();
                                                                    return Ok(liveRes);
                                                                }
                                                                catch
                                                                {
                                                                    return Ok(liveRes);
                                                                }
                                                            }
                                                            else if (status.ToUpper() == "FAILED")
                                                            {

                                                                var optcodei = db.Operator_Code.Where(aa => aa.new_opt_code == api.Optcode).SingleOrDefault().Operator_id.ToString();
                                                                var show = db.failed_recharge_move.Where(aa => aa.operator_code == optcodei && aa.status == "Y").SingleOrDefault();
                                                                if (show == null)
                                                                {
                                                                    status = "Failed";
                                                                    db.recharge_update(msgorderid, status, "NA", Convert.ToDecimal(0), json.ToString(), "Response");
                                                                    try
                                                                    {
                                                                        var retailerdetails = db.api_user_details.Where(aa => aa.apiid == userid.UserId).SingleOrDefault();
                                                                        var remdetails = db.api_remain_amount.Where(aa => aa.apiid == userid.UserId).SingleOrDefault();

                                                                        var admininfo = db.Admin_details.SingleOrDefault();
                                                                        Backupinfo back = new Backupinfo();
                                                                        var model = new Backupinfo.Addinfo
                                                                        {
                                                                            Websitename = admininfo.WebsiteUrl,
                                                                            RetailerID = userid.UserId,
                                                                            Email = retailerdetails.emailid,
                                                                            Mobile = retailerdetails.mobile,
                                                                            Details = "Recharge Refund " + api.Customernumber + " Amount " + api.Amount,
                                                                            RemainBalance = (decimal)remdetails.balance,
                                                                            Usertype = "API"
                                                                        };
                                                                        back.Rechargeandutility(model);


                                                                    }
                                                                    catch { }
                                                                    JObject liveRes = outputchk("Failed", "", "Failed", (decimal)remain, api.Yourrchid, orderid);
                                                                    try
                                                                    {
                                                                        Recharge_info objj = (from p in db.Recharge_info where p.idno == idnn11 select p).Single();
                                                                        objj.API_response = liveRes.ToString();
                                                                        objj.response_output = "";
                                                                        objj.Api_Response_output = "";
                                                                        db.SaveChanges();
                                                                        return Ok(liveRes);
                                                                    }
                                                                    catch
                                                                    {
                                                                        return Ok(liveRes);
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    var outputchkBkp = backup.recharge(api.Customernumber, api.Optcode, api.Amount, userid.UserId, idnn11, optcodei, ordersend, ref Transid);
                                                                    var entry = (from rch in db.Recharge_info
                                                                                 where rch.idno == idnn11
                                                                                 select new
                                                                                 { operatorid = rch.OPt_id }).SingleOrDefault();
                                                                    if (outputchkBkp == "SUCCESS")
                                                                    {
                                                                        JObject liveRes = outputchk("Success", entry.operatorid, "", (decimal)remain, api.Yourrchid, orderid);
                                                                        Recharge_info obj = (from p in db.Recharge_info where p.idno == idnn11 select p).Single();
                                                                        obj.API_response = liveRes.ToString();
                                                                        db.SaveChanges();
                                                                        return Ok(liveRes);
                                                                    }
                                                                    else if (outputchkBkp == "FAILED")
                                                                    {
                                                                        JObject liveRes = outputchk("Failed", "", entry.operatorid, (decimal)remain, api.Yourrchid, orderid);
                                                                        Recharge_info obj = (from p in db.Recharge_info where p.idno == idnn11 select p).Single();
                                                                        obj.API_response = liveRes.ToString();
                                                                        db.SaveChanges();
                                                                        return Ok(liveRes);
                                                                    }
                                                                    else
                                                                    {
                                                                        JObject liverch = outputchk("Pending", "", "", (decimal)remain, api.Yourrchid, orderid);
                                                                        return Ok(liverch);
                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                JObject rch = outputchk("Pending", "", "", (decimal)remain, api.Yourrchid, orderid);
                                                                return Ok(rch);
                                                            }
                                                        }
                                                        else if (url.ToUpper().Contains("MROBOTICS.IN"))
                                                        {
                                                            var tkn = db.Recharge_API_URLS.Where(aa => aa.url == url).SingleOrDefault();
                                                            var token = tkn.Token;
                                                            var Tokenid = tkn.Token;
                                                            var Userid = tkn.API_ID;
                                                            POST_API PA = new POST_API();
                                                            int idnn111 = 0;
                                                            decimal amt = Convert.ToDecimal(api.Amount);
                                                            msgorderid = db.Recharge_info.Where(aa => aa.Order_id == orderid).OrderByDescending(aa => aa.idno).Take(1).SingleOrDefault().idno.ToString();
                                                            idnn111 = Convert.ToInt32(msgorderid);
                                                            string CommonTranid = ordersend;//"E" + DateTime.Parse(DateTime.Now.ToString()).ToString("yyMMddHHmmss") + PA.RandomString(4);
                                                            var apioptcode = db.SRS_API.Where(aa => aa.api == url && aa.opt_code == api.Optcode).SingleOrDefault().apioptcode;
                                                            var task = Task.Run(() =>
                                                            {
                                                                return PA.RchReqMrobotics(token, api.Customernumber, Tokenid, Userid, amt, apioptcode, CommonTranid, api.optional1, api.optional2, api.Optcode, url);
                                                            });
                                                            var Request = task.Result.Request.Parameters[1].Value;
                                                            var ReqUrl = url + "  RequestBody : " + Request;
                                                            Recharge_info objCourse = (from p in db.Recharge_info where p.idno == idnn111 select p).Single();
                                                            objCourse.Recharge_request = ReqUrl;
                                                            db.SaveChanges();
                                                            bool isCompletedSuccessfully = task.Wait(TimeSpan.FromMilliseconds(60000));     // 1 minutes
                                                            if (isCompletedSuccessfully)
                                                            {
                                                                var finaloutput = task.Result;
                                                                var responsechk = finaloutput.Content.ToString();
                                                                try
                                                                {
                                                                    WriteLog("APIREQUEST", "Response json" + responsechk);
                                                                }
                                                                catch { }
                                                                dynamic json = JsonConvert.DeserializeObject(responsechk);
                                                                string company_id = apioptcode.ToString();
                                                                string remainamount = "0"; string Transid = "NA"; string webcontent = "";

                                                                Recharge_info objCours = (from p in db.Recharge_info where p.idno == idnn111 select p).Single();
                                                                objCours.Recharge_response = json.ToString();
                                                                db.SaveChanges();
                                                                webcontent = json.ToString();
                                                                string status = json.status.ToString();

                                                                if (status.ToUpper() == "SUCCESS")
                                                                {
                                                                    remainamount = json.balance.ToString();
                                                                    Transid = json.tnx_id.ToString();

                                                                    status = "Success";
                                                                    db.recharge_update(msgorderid, status, Transid, Convert.ToDecimal(remainamount), webcontent, "Response");
                                                                    JObject liveRes = outputchk("Success", Transid, "", (decimal)remain, api.Yourrchid, orderid);
                                                                    try
                                                                    {
                                                                        Recharge_info objj = (from p in db.Recharge_info where p.idno == idnn11 select p).Single();
                                                                        objj.API_response = liveRes.ToString();
                                                                        objj.response_output = "";
                                                                        objj.Api_Response_output = "";
                                                                        db.SaveChanges();
                                                                        return Ok(liveRes);
                                                                    }
                                                                    catch
                                                                    {
                                                                        return Ok(liveRes);
                                                                    }
                                                                }
                                                                else if (status.ToUpper() == "FAILURE" || status.ToUpper() == "FAILED")
                                                                {

                                                                    var optcodei = db.Operator_Code.Where(aa => aa.new_opt_code == api.Optcode).SingleOrDefault().Operator_id.ToString();
                                                                    var show = db.failed_recharge_move.Where(aa => aa.operator_code == optcodei && aa.status == "Y").SingleOrDefault();
                                                                    if (show == null)
                                                                    {
                                                                        status = "Failed";
                                                                        db.recharge_update(msgorderid, status, "NA", Convert.ToDecimal(0), webcontent, "Response");
                                                                        try
                                                                        {
                                                                            var retailerdetails = db.api_user_details.Where(aa => aa.apiid == userid.UserId).SingleOrDefault();
                                                                            var remdetails = db.api_remain_amount.Where(aa => aa.apiid == userid.UserId).SingleOrDefault();

                                                                            var admininfo = db.Admin_details.SingleOrDefault();
                                                                            Backupinfo back = new Backupinfo();
                                                                            var model = new Backupinfo.Addinfo
                                                                            {
                                                                                Websitename = admininfo.WebsiteUrl,
                                                                                RetailerID = userid.UserId,
                                                                                Email = retailerdetails.emailid,
                                                                                Mobile = retailerdetails.mobile,
                                                                                Details = "Recharge Refund " + api.Customernumber + " Amount " + api.Amount,
                                                                                RemainBalance = (decimal)remdetails.balance,
                                                                                Usertype = "API"
                                                                            };
                                                                            back.Rechargeandutility(model);


                                                                        }
                                                                        catch { }
                                                                        JObject liveRes = outputchk("Failed", "", "Failed", (decimal)remain, api.Yourrchid, orderid);
                                                                        try
                                                                        {
                                                                            Recharge_info objj = (from p in db.Recharge_info where p.idno == idnn11 select p).Single();
                                                                            objj.API_response = liveRes.ToString();
                                                                            objj.response_output = "";
                                                                            objj.Api_Response_output = "";
                                                                            db.SaveChanges();
                                                                            return Ok(liveRes);
                                                                        }
                                                                        catch
                                                                        {
                                                                            return Ok(liveRes);
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        var outputchkBkp = backup.recharge(api.Customernumber, api.Optcode, api.Amount, userid.UserId, idnn11, optcodei, ordersend, ref Transid);
                                                                        var entry = (from rch in db.Recharge_info
                                                                                     where rch.idno == idnn11
                                                                                     select new
                                                                                     { operatorid = rch.OPt_id }).SingleOrDefault();
                                                                        if (outputchkBkp == "SUCCESS")
                                                                        {
                                                                            JObject liveRes = outputchk("Success", entry.operatorid, "", (decimal)remain, api.Yourrchid, orderid);
                                                                            Recharge_info obj = (from p in db.Recharge_info where p.idno == idnn11 select p).Single();
                                                                            obj.API_response = liveRes.ToString();
                                                                            db.SaveChanges();
                                                                            return Ok(liveRes);
                                                                        }
                                                                        else if (outputchkBkp == "FAILED")
                                                                        {
                                                                            JObject liveRes = outputchk("Failed", "", entry.operatorid, (decimal)remain, api.Yourrchid, orderid);
                                                                            Recharge_info obj = (from p in db.Recharge_info where p.idno == idnn11 select p).Single();
                                                                            obj.API_response = liveRes.ToString();
                                                                            db.SaveChanges();
                                                                            return Ok(liveRes);
                                                                        }
                                                                        else
                                                                        {
                                                                            JObject liverch = outputchk("Pending", "", "", (decimal)remain, api.Yourrchid, orderid);
                                                                            return Ok(liverch);
                                                                        }
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    JObject liverch = outputchk("Pending", "", "", (decimal)remain, api.Yourrchid, orderid);
                                                                    return Ok(liverch);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                JObject liverch = outputchk("Pending", "", "", (decimal)remain, api.Yourrchid, orderid);
                                                                return Ok(liverch);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            var apiinfo = db.RechargeapiInfoes.Where(aa => aa.apiendpoint.ToUpper() == url.ToUpper()).SingleOrDefault();
                                                            if (apiinfo != null)
                                                            {
                                                                var apiremain = "0"; var _status = "Pending"; var transid = ""; var errormsg = "";
                                                                var apiresponse = "";

                                                                ApiResponse rech = RechargeServices.Recharge(apiinfo, api.Customernumber, api.Optcode, api.Amount, orderid);
                                                                _status = rech.status;
                                                                transid = rech.operatorId;
                                                                errormsg = rech.errormsg;
                                                                apiremain = rech.apiremain;
                                                                apiresponse = rech.api_response;
                                                                int idnn111 = rech.id;
                                                                msgorderid = rech.id.ToString();

                                                                if (_status == "Success")
                                                                {
                                                                    var status = "Success";
                                                                    db.recharge_update(msgorderid, status, transid, Convert.ToDecimal(apiremain), apiresponse.ToString(), "Response");
                                                                    JObject obj1 = outputchk("Success", transid, "", (decimal)remain, api.Yourrchid, orderid);
                                                                    try
                                                                    {
                                                                        Recharge_info objj = (from p in db.Recharge_info where p.idno == idnn111 select p).Single();
                                                                        objj.API_response = obj1.ToString();
                                                                        objj.response_output = "";
                                                                        objj.Api_Response_output = "";
                                                                        db.SaveChanges();
                                                                        return Ok(obj1);
                                                                    }
                                                                    catch
                                                                    {
                                                                        return Ok(obj1);
                                                                    }

                                                                }
                                                                else if (_status == "Failed")
                                                                {
                                                                    var optcodei = db.Operator_Code.Where(aa => aa.new_opt_code == api.Optcode).SingleOrDefault().Operator_id.ToString();
                                                                    var show = db.failed_recharge_move.Where(aa => aa.operator_code == optcodei && aa.status == "Y").SingleOrDefault();
                                                                    if (show == null)
                                                                    {
                                                                        //status = "Failed";
                                                                        var status = "Failed";
                                                                        db.recharge_update(msgorderid, status, "NA", Convert.ToDecimal(0), apiresponse, "Response");
                                                                        try
                                                                        {
                                                                            var retailerdetails = db.api_user_details.Where(aa => aa.apiid == userid.UserId).SingleOrDefault();
                                                                            var remdetails = db.api_remain_amount.Where(aa => aa.apiid == userid.UserId).SingleOrDefault();

                                                                            var admininfo = db.Admin_details.SingleOrDefault();
                                                                            Backupinfo back = new Backupinfo();
                                                                            var model = new Backupinfo.Addinfo
                                                                            {
                                                                                Websitename = admininfo.WebsiteUrl,
                                                                                RetailerID = userid.UserId,
                                                                                Email = retailerdetails.emailid,
                                                                                Mobile = retailerdetails.mobile,
                                                                                Details = "Recharge Refund " + api.Customernumber + " Amount " + api.Amount,
                                                                                RemainBalance = (decimal)remdetails.balance,
                                                                                Usertype = "API"
                                                                            };
                                                                            back.Rechargeandutility(model);


                                                                        }
                                                                        catch { }
                                                                        JObject liveRes = outputchk("Failed", "", "Failed", (decimal)remain, api.Yourrchid, orderid);
                                                                        try
                                                                        {
                                                                            Recharge_info objj = (from p in db.Recharge_info where p.idno == idnn111 select p).Single();
                                                                            objj.API_response = liveRes.ToString();
                                                                            objj.response_output = "";
                                                                            objj.Api_Response_output = "";
                                                                            db.SaveChanges();
                                                                            return Ok(liveRes);
                                                                        }
                                                                        catch
                                                                        {
                                                                            return Ok(liveRes);
                                                                        }
                                                                    }
                                                                    else
                                                                    {

                                                                        var optcodei1 = db.Operator_Code.Where(aa => aa.new_opt_code == api.Optcode).SingleOrDefault().Operator_id.ToString();
                                                                        var show1 = db.failed_recharge_move.Where(aa => aa.operator_code == optcodei1 && aa.status == "Y").SingleOrDefault();
                                                                        if (show1 == null)
                                                                        {
                                                                            var status = "Failed";
                                                                            db.recharge_update(msgorderid, status, "NA", Convert.ToDecimal(0), apiresponse, "Response");
                                                                            JObject liveRes = outputchk("Failed", "", "Failed", (decimal)remain, api.Yourrchid, orderid);
                                                                            try
                                                                            {
                                                                                Recharge_info objj = (from p in db.Recharge_info where p.idno == idnn111 select p).Single();
                                                                                objj.API_response = liveRes.ToString();
                                                                                objj.response_output = "";
                                                                                objj.Api_Response_output = "";
                                                                                db.SaveChanges();
                                                                                return Ok(liveRes);
                                                                            }
                                                                            catch
                                                                            {
                                                                                return Ok(liveRes);
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            var outputchkBkp = backup.recharge(api.Customernumber, api.Optcode, api.Amount, userid.UserId, idnn111, optcodei, ordersend, ref transid);
                                                                            var entry = (from rch in db.Recharge_info
                                                                                         where rch.idno == idnn111
                                                                                         select new
                                                                                         { operatorid = rch.OPt_id }).SingleOrDefault();
                                                                            if (outputchkBkp == "SUCCESS")
                                                                            {
                                                                                JObject liveRes = outputchk("Success", entry.operatorid, "", (decimal)remain, api.Yourrchid, orderid);
                                                                                Recharge_info obj = (from p in db.Recharge_info where p.idno == idnn111 select p).Single();
                                                                                obj.API_response = liveRes.ToString();
                                                                                db.SaveChanges();
                                                                                return Ok(liveRes);
                                                                            }
                                                                            else if (outputchkBkp == "FAILED")
                                                                            {
                                                                                JObject liveRes = outputchk("Failed", "", entry.operatorid, (decimal)remain, api.Yourrchid, orderid);
                                                                                Recharge_info obj = (from p in db.Recharge_info where p.idno == idnn111 select p).Single();
                                                                                obj.API_response = liveRes.ToString();
                                                                                db.SaveChanges();
                                                                                return Ok(liveRes);
                                                                            }
                                                                            else
                                                                            {
                                                                                JObject liverch = outputchk("Pending", "", "", (decimal)remain, api.Yourrchid, orderid);
                                                                                return Ok(liverch);
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    JObject rch = outputchk("Pending", "", transid, (decimal)remain, api.Yourrchid, orderid);
                                                                    return Ok(rch);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                                                                HttpWebRequest WebRequestObject = (HttpWebRequest)HttpWebRequest.Create(url);
                                                                WebRequestObject.Timeout = (System.Int32)TimeSpan.FromSeconds(125).TotalMilliseconds;
                                                                WebResponse Response = WebRequestObject.GetResponse();
                                                                Stream WebStream = Response.GetResponseStream();
                                                                StreamReader Reader = new StreamReader(WebStream);
                                                                var webcontent = Reader.ReadToEnd();
                                                                try
                                                                {
                                                                    WriteLog("APIREQUEST", "Response json" + webcontent);
                                                                }
                                                                catch { }
                                                                Recharge_info objjj = (from p in db.Recharge_info where p.idno == idnn11 select p).Single();
                                                                objjj.Recharge_response = webcontent.ToString();
                                                                db.SaveChanges();

                                                                if (url.ToUpper().Contains("LIVE.VASTWEBINDIA.COM"))
                                                                {
                                                                    dynamic stuff = JsonConvert.DeserializeObject(webcontent);
                                                                    var status = stuff.Status.ToString();
                                                                    var Mobile = stuff.Mobile.ToString();
                                                                    var Amount = stuff.Amount.ToString();
                                                                    var RCHID = stuff.RCHID.ToString();
                                                                    string Operatorid = stuff.Operatorid.ToString();
                                                                    var remainamount = stuff.remainamount.ToString();
                                                                    var LapuNumber = stuff.LapuNumber.ToString();
                                                                    if (status.ToUpper() == "SUCCESS")
                                                                    {
                                                                        status = "Success";
                                                                        db.recharge_update(msgorderid, status, Operatorid, Convert.ToDecimal(remainamount), webcontent.ToString(), "Response");
                                                                        JObject obj1 = outputchk("Success", Operatorid, "", (decimal)remain, api.Yourrchid, orderid);
                                                                        try
                                                                        {
                                                                            Recharge_info objj = (from p in db.Recharge_info where p.idno == idnn11 select p).Single();
                                                                            objj.API_response = obj1.ToString();
                                                                            objj.response_output = "";
                                                                            objj.Api_Response_output = "";
                                                                            db.SaveChanges();
                                                                            return Ok(obj1);
                                                                        }
                                                                        catch
                                                                        {
                                                                            return Ok(obj1);
                                                                        }
                                                                    }
                                                                    else if (status.ToUpper() == "FAILED")
                                                                    {

                                                                        var optcodei = db.Operator_Code.Where(aa => aa.new_opt_code == api.Optcode).SingleOrDefault().Operator_id.ToString();
                                                                        var show = db.failed_recharge_move.Where(aa => aa.operator_code == optcodei && aa.status == "Y").SingleOrDefault();
                                                                        if (show == null)
                                                                        {
                                                                            status = "Failed";
                                                                            db.recharge_update(msgorderid, status, "NA", Convert.ToDecimal(0), webcontent, "Response");
                                                                            try
                                                                            {
                                                                                var retailerdetails = db.api_user_details.Where(aa => aa.apiid == userid.UserId).SingleOrDefault();
                                                                                var remdetails = db.api_remain_amount.Where(aa => aa.apiid == userid.UserId).SingleOrDefault();

                                                                                var admininfo = db.Admin_details.SingleOrDefault();
                                                                                Backupinfo back = new Backupinfo();
                                                                                var model = new Backupinfo.Addinfo
                                                                                {
                                                                                    Websitename = admininfo.WebsiteUrl,
                                                                                    RetailerID = userid.UserId,
                                                                                    Email = retailerdetails.emailid,
                                                                                    Mobile = retailerdetails.mobile,
                                                                                    Details = "Recharge Refund " + api.Customernumber + " Amount " + api.Amount,
                                                                                    RemainBalance = (decimal)remdetails.balance,
                                                                                    Usertype = "API"
                                                                                };
                                                                                back.Rechargeandutility(model);


                                                                            }
                                                                            catch { }
                                                                            JObject liveRes = outputchk("Failed", "", "Failed", (decimal)remain, api.Yourrchid, orderid);
                                                                            try
                                                                            {
                                                                                Recharge_info objj = (from p in db.Recharge_info where p.idno == idnn11 select p).Single();
                                                                                objj.API_response = liveRes.ToString();
                                                                                objj.response_output = "";
                                                                                objj.Api_Response_output = "";
                                                                                db.SaveChanges();
                                                                                return Ok(liveRes);
                                                                            }
                                                                            catch
                                                                            {
                                                                                return Ok(liveRes);
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            var outputchkBkp = backup.recharge(api.Customernumber, api.Optcode, api.Amount, userid.UserId, idnn11, optcodei, ordersend, ref Operatorid);
                                                                            var entry = (from rch in db.Recharge_info
                                                                                         where rch.idno == idnn11
                                                                                         select new
                                                                                         { operatorid = rch.OPt_id }).SingleOrDefault();
                                                                            if (outputchkBkp == "SUCCESS")
                                                                            {
                                                                                JObject liveRes = outputchk("Success", entry.operatorid, "", (decimal)remain, api.Yourrchid, orderid);
                                                                                Recharge_info obj = (from p in db.Recharge_info where p.idno == idnn11 select p).Single();
                                                                                obj.API_response = liveRes.ToString();
                                                                                db.SaveChanges();
                                                                                return Ok(liveRes);
                                                                            }
                                                                            else if (outputchkBkp == "FAILED")
                                                                            {
                                                                                JObject liveRes = outputchk("Failed", "", entry.operatorid, (decimal)remain, api.Yourrchid, orderid);
                                                                                Recharge_info obj = (from p in db.Recharge_info where p.idno == idnn11 select p).Single();
                                                                                obj.API_response = liveRes.ToString();
                                                                                db.SaveChanges();
                                                                                return Ok(liveRes);
                                                                            }
                                                                            else
                                                                            {
                                                                                JObject liverch = outputchk("Pending", "", "", (decimal)remain, api.Yourrchid, orderid);
                                                                                return Ok(liverch);
                                                                            }
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        JObject rch = outputchk("Pending", "", Operatorid, (decimal)remain, api.Yourrchid, orderid);
                                                                        return Ok(rch);
                                                                    }
                                                                }
                                                                else if (url.ToUpper().Contains("VASTWEBINDIA.COM"))
                                                                {
                                                                    dynamic stuff = Newtonsoft.Json.JsonConvert.DeserializeObject(webcontent);
                                                                    var status = stuff.Status.ToString();
                                                                    var mobile = stuff.Mobile.ToString();
                                                                    var amount1 = stuff.Amount.ToString();
                                                                    var R_id = stuff.RID.ToString();
                                                                    string operatorid = stuff.Operatorid.ToString();
                                                                    var remain_amount = stuff.remainamount.ToString();
                                                                    if (status.ToUpper() == "SUCCESS")
                                                                    {
                                                                        status = "Success";
                                                                        db.recharge_update(msgorderid, status, operatorid, Convert.ToDecimal(remain_amount), webcontent.ToString(), "Response");
                                                                        JObject rch = outputchk("Success", operatorid, "", (decimal)remain, api.Yourrchid, orderid);
                                                                        try
                                                                        {
                                                                            Recharge_info objj = (from p in db.Recharge_info where p.idno == idnn11 select p).Single();
                                                                            objj.API_response = rch.ToString();
                                                                            objj.response_output = "";
                                                                            objj.Api_Response_output = "";
                                                                            db.SaveChanges();
                                                                            return Ok(rch);
                                                                        }
                                                                        catch
                                                                        {
                                                                            return Ok(rch);
                                                                        }
                                                                    }
                                                                    else if (status.ToUpper() == "FAILED")
                                                                    {

                                                                        var optcodei = db.Operator_Code.Where(aa => aa.new_opt_code == api.Optcode).SingleOrDefault().Operator_id.ToString();
                                                                        var show = db.failed_recharge_move.Where(aa => aa.operator_code == optcodei && aa.status == "Y").SingleOrDefault();
                                                                        if (show == null)
                                                                        {
                                                                            status = "Failed";
                                                                            db.recharge_update(msgorderid, status, "NA", Convert.ToDecimal(0), webcontent, "Response");
                                                                            try
                                                                            {
                                                                                var retailerdetails = db.api_user_details.Where(aa => aa.apiid == userid.UserId).SingleOrDefault();
                                                                                var remdetails = db.api_remain_amount.Where(aa => aa.apiid == userid.UserId).SingleOrDefault();

                                                                                var admininfo = db.Admin_details.SingleOrDefault();
                                                                                Backupinfo back = new Backupinfo();
                                                                                var model = new Backupinfo.Addinfo
                                                                                {
                                                                                    Websitename = admininfo.WebsiteUrl,
                                                                                    RetailerID = userid.UserId,
                                                                                    Email = retailerdetails.emailid,
                                                                                    Mobile = retailerdetails.mobile,
                                                                                    Details = "Recharge  Refund" + api.Customernumber + " Amount " + api.Amount,
                                                                                    RemainBalance = (decimal)remdetails.balance,
                                                                                    Usertype = "API"
                                                                                };
                                                                                back.Rechargeandutility(model);


                                                                            }
                                                                            catch { }
                                                                            JObject liveRes = outputchk("Failed", "", "Failed", (decimal)remain, api.Yourrchid, orderid);
                                                                            try
                                                                            {
                                                                                Recharge_info objj = (from p in db.Recharge_info where p.idno == idnn11 select p).Single();
                                                                                objj.API_response = liveRes.ToString();
                                                                                objj.response_output = "";
                                                                                objj.Api_Response_output = "";
                                                                                db.SaveChanges();
                                                                                return Ok(liveRes);
                                                                            }
                                                                            catch
                                                                            {
                                                                                return Ok(liveRes);
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            var outputchkBkp = backup.recharge(api.Customernumber, api.Optcode, api.Amount, userid.UserId, idnn11, optcodei, ordersend, ref operatorid);
                                                                            var entry = (from rch in db.Recharge_info
                                                                                         where rch.idno == idnn11
                                                                                         select new
                                                                                         { operatorid = rch.OPt_id }).SingleOrDefault();
                                                                            if (outputchkBkp == "SUCCESS")
                                                                            {
                                                                                JObject liveRes = outputchk("Success", entry.operatorid, "", (decimal)remain, api.Yourrchid, orderid);
                                                                                Recharge_info obj = (from p in db.Recharge_info where p.idno == idnn11 select p).Single();
                                                                                obj.API_response = liveRes.ToString();
                                                                                db.SaveChanges();
                                                                                return Ok(liveRes);
                                                                            }
                                                                            else if (outputchkBkp == "FAILED")
                                                                            {
                                                                                JObject liveRes = outputchk("Failed", "", entry.operatorid, (decimal)remain, api.Yourrchid, orderid);
                                                                                Recharge_info obj = (from p in db.Recharge_info where p.idno == idnn11 select p).Single();
                                                                                obj.API_response = liveRes.ToString();
                                                                                db.SaveChanges();
                                                                                return Ok(liveRes);
                                                                            }
                                                                            else
                                                                            {
                                                                                JObject liverch = outputchk("Pending", "", "", (decimal)remain, api.Yourrchid, orderid);
                                                                                return Ok(liverch);
                                                                            }
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        JObject liverch = outputchk("Pending", "", "", (decimal)remain, api.Yourrchid, orderid);
                                                                        return Ok(liverch);
                                                                    }
                                                                }
                                                                else if (url.ToUpper().Contains("RECHARGETRAAWSERVICES.IN"))
                                                                {

                                                                    dynamic stuff = Newtonsoft.Json.JsonConvert.DeserializeObject(webcontent);

                                                                    var status = stuff.status.ToString();

                                                                    string operatorid = stuff.operatorid.ToString();
                                                                    var amount1 = stuff.remainamount.ToString();

                                                                    if (amount1 == "" || amount1 == null)
                                                                    {
                                                                        amount1 = "0";
                                                                    }

                                                                    if (status.ToUpper() == "SUCCESS")
                                                                    {
                                                                        status = "Success";
                                                                        db.recharge_update(msgorderid, status, operatorid, Convert.ToDecimal(amount1), webcontent.ToString(), "Response");
                                                                        JObject rch = outputchk("Success", operatorid, "", (decimal)remain, api.Yourrchid, orderid);
                                                                        try
                                                                        {
                                                                            Recharge_info objj = (from p in db.Recharge_info where p.idno == idnn11 select p).Single();
                                                                            objj.API_response = rch.ToString();
                                                                            objj.response_output = "";
                                                                            objj.Api_Response_output = "";
                                                                            db.SaveChanges();
                                                                            return Ok(rch);
                                                                        }
                                                                        catch
                                                                        {
                                                                            return Ok(rch);
                                                                        }
                                                                    }
                                                                    else if (status.ToUpper() == "FAILED")
                                                                    {

                                                                        var optcodei = db.Operator_Code.Where(aa => aa.new_opt_code == api.Optcode).SingleOrDefault().Operator_id.ToString();
                                                                        var show = db.failed_recharge_move.Where(aa => aa.operator_code == optcodei && aa.status == "Y").SingleOrDefault();
                                                                        if (show == null)
                                                                        {
                                                                            status = "Failed";
                                                                            db.recharge_update(msgorderid, status, "NA", Convert.ToDecimal(0), webcontent, "Response");
                                                                            try
                                                                            {
                                                                                var retailerdetails = db.api_user_details.Where(aa => aa.apiid == userid.UserId).SingleOrDefault();
                                                                                var remdetails = db.api_remain_amount.Where(aa => aa.apiid == userid.UserId).SingleOrDefault();

                                                                                var admininfo = db.Admin_details.SingleOrDefault();
                                                                                Backupinfo back = new Backupinfo();
                                                                                var model = new Backupinfo.Addinfo
                                                                                {
                                                                                    Websitename = admininfo.WebsiteUrl,
                                                                                    RetailerID = userid.UserId,
                                                                                    Email = retailerdetails.emailid,
                                                                                    Mobile = retailerdetails.mobile,
                                                                                    Details = "Recharge Refund " + api.Customernumber + " Amount " + api.Amount,
                                                                                    RemainBalance = (decimal)remdetails.balance,
                                                                                    Usertype = "API"
                                                                                };
                                                                                back.Rechargeandutility(model);


                                                                            }
                                                                            catch { }
                                                                            JObject liveRes = outputchk("Failed", "", "Failed", (decimal)remain, api.Yourrchid, orderid);
                                                                            try
                                                                            {
                                                                                Recharge_info objj = (from p in db.Recharge_info where p.idno == idnn11 select p).Single();
                                                                                objj.API_response = liveRes.ToString();
                                                                                objj.response_output = "";
                                                                                objj.Api_Response_output = "";
                                                                                db.SaveChanges();
                                                                                return Ok(liveRes);
                                                                            }
                                                                            catch
                                                                            {
                                                                                return Ok(liveRes);
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            var outputchkBkp = backup.recharge(api.Customernumber, api.Optcode, api.Amount, userid.UserId, idnn11, optcodei, ordersend, ref operatorid);
                                                                            var entry = (from rch in db.Recharge_info
                                                                                         where rch.idno == idnn11
                                                                                         select new
                                                                                         { operatorid = rch.OPt_id }).SingleOrDefault();
                                                                            if (outputchkBkp == "SUCCESS")
                                                                            {
                                                                                JObject liveRes = outputchk("Success", entry.operatorid, "", (decimal)remain, api.Yourrchid, orderid);
                                                                                Recharge_info obj = (from p in db.Recharge_info where p.idno == idnn11 select p).Single();
                                                                                obj.API_response = liveRes.ToString();
                                                                                db.SaveChanges();
                                                                                return Ok(liveRes);
                                                                            }
                                                                            else if (outputchkBkp == "FAILED")
                                                                            {
                                                                                JObject liveRes = outputchk("Failed", "", entry.operatorid, (decimal)remain, api.Yourrchid, orderid);
                                                                                Recharge_info obj = (from p in db.Recharge_info where p.idno == idnn11 select p).Single();
                                                                                obj.API_response = liveRes.ToString();
                                                                                db.SaveChanges();
                                                                                return Ok(liveRes);
                                                                            }
                                                                            else
                                                                            {
                                                                                JObject liverch = outputchk("Pending", "", "", (decimal)remain, api.Yourrchid, orderid);
                                                                                return Ok(liverch);
                                                                            }
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        JObject liverch = outputchk("Pending", "", "", (decimal)remain, api.Yourrchid, orderid);
                                                                        return Ok(liverch);
                                                                    }
                                                                }
                                                                else if (url.ToUpper().Contains("INSTANTPAY.IN"))
                                                                {
                                                                    dynamic response = JsonConvert.DeserializeObject(webcontent.ToString());
                                                                    var RESCODE = response.res_code.ToString();
                                                                    var optid = response.opr_id.ToString();
                                                                    var openbal = response.opening_bal.ToString();
                                                                    decimal remainbal = Convert.ToDecimal(openbal);
                                                                    if (RESCODE == "TXN")
                                                                    {
                                                                        db.recharge_update(msgorderid, "SUCCESS", optid, remainbal, webcontent.ToString(), "Response");
                                                                        JObject rch = outputchk("Success", optid, "", (decimal)remain, api.Yourrchid, orderid);
                                                                        return Ok(rch);
                                                                    }
                                                                    else if (RESCODE.ToString() == "TUP")
                                                                    {
                                                                        JObject liverch = outputchk("Pending", "", "", (decimal)remain, api.Yourrchid, orderid);
                                                                        return Ok(liverch);
                                                                    }
                                                                    else
                                                                    {
                                                                        db.recharge_update(msgorderid, "FAILED", optid, remain, webcontent.ToString(), "Response");
                                                                        try
                                                                        {
                                                                            var retailerdetails = db.api_user_details.Where(aa => aa.apiid == userid.UserId).SingleOrDefault();
                                                                            var remdetails = db.api_remain_amount.Where(aa => aa.apiid == userid.UserId).SingleOrDefault();

                                                                            var admininfo = db.Admin_details.SingleOrDefault();
                                                                            Backupinfo back = new Backupinfo();
                                                                            var model = new Backupinfo.Addinfo
                                                                            {
                                                                                Websitename = admininfo.WebsiteUrl,
                                                                                RetailerID = userid.UserId,
                                                                                Email = retailerdetails.emailid,
                                                                                Mobile = retailerdetails.mobile,
                                                                                Details = "Recharge Refund " + api.Customernumber + " Amount " + api.Amount,
                                                                                RemainBalance = (decimal)remdetails.balance,
                                                                                Usertype = "API"
                                                                            };
                                                                            back.Rechargeandutility(model);


                                                                        }
                                                                        catch { }
                                                                        JObject rch = outputchk("Failed", "", optid, (decimal)remain, api.Yourrchid, orderid);
                                                                        return Ok(rch);
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    JObject rch = outputchk("Pending", "", "", (decimal)remain, api.Yourrchid, orderid);
                                                                    return Ok(rch);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    JObject rch = outputchk("Failed", "", "Remain Balance Low.", (decimal)remain, api.Yourrchid, "");
                                                    return Ok(rch);
                                                }
                                            }
                                            else
                                            {
                                                JObject rch = outputchk("Failed", "", "Operator Code is invalid.", 0, api.Yourrchid, "");
                                                return Ok(rch);
                                            }
                                        }
                                        else
                                        {
                                            JObject rch = outputchk("Failed", "", "Ip is Not Valid. IP: " + currentip, 0, api.Yourrchid, "");
                                            return Ok(rch);
                                        }
                                    }
                                    else
                                    {
                                        JObject rch = outputchk("Failed", "", "Invalid Token.", 0, api.Yourrchid, "");
                                        return Ok(rch);
                                    }
                                }
                                else
                                {
                                    JObject rch = outputchk("Failed", "", "Token is Not Created.", 0, api.Yourrchid, "");
                                    return Ok(rch);
                                }
                            }
                            else
                            {
                                JObject rch = outputchk("Failed", "", "This User is Not Register With Us.", 0, api.Yourrchid, "");
                                return Ok(rch);
                            }
                        }
                    }
                    else
                    {
                        JObject rch = outputchk("Failed", "", "Missing Parameter", 0, api.Yourrchid, "");
                        return Ok(rch);
                    }
                }
                catch (Exception ex)
                {
                    test1 t1 = new test1();
                    t1.name = String.Format("Api Recharge Exception POST - Message: {0}, Stack: {1}, InnerMsg: {2}, InnerStack: {3}, Time: {4}", ex.Message, ex.StackTrace, ex.InnerException?.Message, ex.InnerException?.StackTrace, DateTime.Now.ToString());
                    db.test1.Add(t1);
                    db.SaveChanges();

                    WriteLog("Errormessage", "Errormessage" + ex.Message);
                    JObject rch = outputchk("Pending", "", ex.Message, 0, api.Yourrchid, "");
                    return Ok(rch);
                }
            }
        }
        [HttpPost]
        [Route("Recharge/Status")]
        public IHttpActionResult Status(RchSTS api)
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                if (api.userid != null && api.tokenid != null && api.ClientRchid != null)
                {
                    var userid = db.Users.Where(aa => aa.UserName == api.userid).SingleOrDefault();
                    if (userid != null)
                    {
                        var tokenchk = db.Api_ip_address.Where(aa => aa.userid == userid.UserId && aa.token == api.tokenid).SingleOrDefault();
                        if (tokenchk != null)
                        {
                            if (tokenchk.token == api.tokenid)
                            {
                                var key = userid.UserId.Substring(0, 16);
                                var usedip = Decrypt(tokenchk.token, key);
                                var currentip = GetComputer_InternetIP();
                                if (currentip == usedip)
                                {
                                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                                    var clientchk = db.Recharge_info.Where(aa => aa.refid == api.ClientRchid && aa.rch_type == "API" && aa.Rch_from == userid.UserId).FirstOrDefault();
                                    if (clientchk != null)
                                    {
                                        decimal remain = Convert.ToDecimal(clientchk.Remain);
                                        JObject rch = outputchk(clientchk.Rstaus, clientchk.OPt_id, clientchk.OPt_id, remain, api.ClientRchid, clientchk.Order_id);
                                        return Ok(rch);
                                    }
                                    else
                                    {
                                        var clientchkold = db.Recharge_info_old.Where(aa => aa.refid == api.ClientRchid && aa.rch_type == "API" && aa.Rch_from == userid.UserId).FirstOrDefault();
                                        if (clientchkold != null)
                                        {
                                            decimal remain = Convert.ToDecimal(clientchkold.Remain);
                                            JObject rch = outputchk(clientchkold.Rstaus, clientchkold.OPt_id, clientchkold.OPt_id, remain, api.ClientRchid, clientchkold.Order_id);
                                            return Ok(rch);
                                        }
                                        else
                                        {
                                            JObject rch = outputchk("Failed", "", "The ClientID is Not Exist With Us", 0, api.ClientRchid, "");
                                            return Ok(rch);
                                        }
                                    }
                                }
                                else
                                {
                                    JObject rch = outputchk("Failed", "", "IP is Not Register. IP: " + currentip, 0, api.ClientRchid, "");
                                    return Ok(rch);
                                }
                            }
                            else
                            {
                                JObject rch = outputchk("Failed", "", "Token Is Miss Match", 0, api.ClientRchid, "");
                                return Ok(rch);
                            }
                        }
                        else
                        {
                            JObject rch = outputchk("Failed", "", "Token Is NOt Register", 0, api.ClientRchid, "");
                            return Ok(rch);
                        }
                    }
                    else
                    {
                        JObject rch = outputchk("Failed", "", "User Id Not Register", 0, api.ClientRchid, "");
                        return Ok(rch);
                    }
                }
                else
                {
                    JObject rch = outputchk("Failed", "", "Missing Parameter", 0, api.ClientRchid, "");
                    return Ok(rch);
                }
            }
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("Recharge/StatusCheck")]
        public IHttpActionResult StatusCheck(string emailid, string tokenid, string ClientRchid)
        {

            tokenid = tokenid.Replace(" ", "+");
            RchSTS api = new RchSTS()
            {
                userid = emailid,
                tokenid = tokenid,
                ClientRchid = ClientRchid
            };

            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                if (api.userid != null && api.tokenid != null && api.ClientRchid != null)
                {
                    var userid = db.Users.Where(aa => aa.UserName == api.userid).SingleOrDefault();
                    if (userid != null)
                    {
                        var tokenchk = db.Api_ip_address.Where(aa => aa.userid == userid.UserId && aa.token == api.tokenid).SingleOrDefault();
                        if (tokenchk != null)
                        {
                            if (tokenchk.token == api.tokenid)
                            {
                                var key = userid.UserId.Substring(0, 16);
                                var usedip = Decrypt(tokenchk.token, key);
                                var currentip = GetComputer_InternetIP();
                                if (currentip == usedip)
                                {
                                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                                    var clientchk = db.Recharge_info.Where(aa => aa.refid == api.ClientRchid && aa.rch_type == "API" && aa.Rch_from == userid.UserId).FirstOrDefault();
                                    if (clientchk != null)
                                    {
                                        decimal remain = Convert.ToDecimal(clientchk.Remain);
                                        JObject rch = outputchk(clientchk.Rstaus, clientchk.OPt_id, clientchk.OPt_id, remain, api.ClientRchid, clientchk.Order_id);
                                        return Ok(rch);
                                    }
                                    else
                                    {
                                        var clientchkold = db.Recharge_info_old.Where(aa => aa.refid == api.ClientRchid && aa.rch_type == "API" && aa.Rch_from == userid.UserId).FirstOrDefault();
                                        if (clientchkold != null)
                                        {
                                            decimal remain = Convert.ToDecimal(clientchkold.Remain);
                                            JObject rch = outputchk(clientchkold.Rstaus, clientchkold.OPt_id, clientchkold.OPt_id, remain, api.ClientRchid, clientchkold.Order_id);
                                            return Ok(rch);
                                        }
                                        else
                                        {
                                            JObject rch = outputchk("Failed", "", "The ClientID is Not Exist With Us", 0, api.ClientRchid, "");
                                            return Ok(rch);
                                        }
                                    }
                                }
                                else
                                {
                                    JObject rch = outputchk("Failed", "", "IP is Not Register . " + currentip + "", 0, api.ClientRchid, "");
                                    return Ok(rch);
                                }
                            }
                            else
                            {
                                JObject rch = outputchk("Failed", "", "Token Is Miss Match", 0, api.ClientRchid, "");
                                return Ok(rch);
                            }
                        }
                        else
                        {
                            JObject rch = outputchk("Failed", "", "Token Is NOt Register", 0, api.ClientRchid, "");
                            return Ok(rch);
                        }
                    }
                    else
                    {
                        JObject rch = outputchk("Failed", "", "User Id Not Register", 0, api.ClientRchid, "");
                        return Ok(rch);
                    }
                }
                else
                {
                    JObject rch = outputchk("Failed", "", "Missing Parameter", 0, api.ClientRchid, "");
                    return Ok(rch);
                }
            }
        }
        [HttpPost]
        [Route("Recharge/Balance")]
        public IHttpActionResult Balance(RchSTS api)
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                if (api.userid != null && api.tokenid != null)
                {
                    var userid = db.Users.Where(aa => aa.UserName == api.userid).SingleOrDefault();
                    if (userid != null)
                    {
                        var tokenchk = db.Api_ip_address.Where(aa => aa.userid == userid.UserId && aa.token == api.tokenid).SingleOrDefault();
                        if (tokenchk != null)
                        {
                            if (tokenchk.token == api.tokenid)
                            {
                                var key = userid.UserId.Substring(0, 16);
                                var usedip = Decrypt(tokenchk.token, key);
                                var currentip = GetComputer_InternetIP();
                                if (currentip == usedip)
                                {
                                    var remain = db.api_remain_amount.Where(aa => aa.apiid == userid.UserId).SingleOrDefault().balance;
                                    dynamic resp = new JObject();
                                    resp.Status = "Success";
                                    resp.Remain = remain;
                                    return Ok(resp);
                                }
                                else
                                {
                                    JObject rch = outputchk("Failed", "", "IP is Not Register. IP: " + currentip, 0, api.ClientRchid, "");
                                    return Ok(rch);
                                }
                            }
                            else
                            {
                                JObject rch = outputchk("Failed", "", "Token Is Miss Match", 0, api.ClientRchid, "");
                                return Ok(rch);
                            }
                        }
                        else
                        {
                            JObject rch = outputchk("Failed", "", "Token Is NOt Register", 0, api.ClientRchid, "");
                            return Ok(rch);
                        }
                    }
                    else
                    {
                        JObject rch = outputchk("Failed", "", "User Id Not Register", 0, api.ClientRchid, "");
                        return Ok(rch);
                    }
                }
                else
                {
                    JObject rch = outputchk("Failed", "", "Missing Parameter", 0, api.ClientRchid, "");
                    return Ok(rch);
                }
            }
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("Recharge/BalanceCheck")]
        public IHttpActionResult BalanceCheck(string emailid, string tokenid)
        {
            RchSTS api = new RchSTS()
            {
                userid = emailid,
                tokenid = tokenid
            };
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                if (emailid != null && tokenid != null)
                {
                    var userid = db.Users.Where(aa => aa.UserName == api.userid).SingleOrDefault();
                    if (userid != null)
                    {
                        var tokenchk = db.Api_ip_address.Where(aa => aa.userid == userid.UserId && aa.token == tokenid).SingleOrDefault();
                        if (tokenchk != null)
                        {
                            if (tokenchk.token == api.tokenid)
                            {
                                var key = userid.UserId.Substring(0, 16);
                                var usedip = Decrypt(tokenchk.token, key);
                                var currentip = GetComputer_InternetIP();
                                if (currentip == usedip)
                                {
                                    var remain = db.api_remain_amount.Where(aa => aa.apiid == userid.UserId).SingleOrDefault().balance;
                                    dynamic resp = new JObject();
                                    resp.Status = "Success";
                                    resp.Remain = remain;
                                    return Ok(resp);
                                }
                                else
                                {
                                    JObject rch = outputchk("Failed", "", "IP is Not Register. IP: " + currentip, 0, api.ClientRchid, "");
                                    return Ok(rch);
                                }
                            }
                            else
                            {
                                JObject rch = outputchk("Failed", "", "Token Is Miss Match", 0, api.ClientRchid, "");
                                return Ok(rch);
                            }
                        }
                        else
                        {
                            JObject rch = outputchk("Failed", "", "Token Is NOt Register", 0, api.ClientRchid, "");
                            return Ok(rch);
                        }
                    }
                    else
                    {
                        JObject rch = outputchk("Failed", "", "User Id Not Register", 0, api.ClientRchid, "");
                        return Ok(rch);
                    }
                }
                else
                {
                    JObject rch = outputchk("Failed", "", "Missing Parameter", 0, api.ClientRchid, "");
                    return Ok(rch);
                }
            }
        }


        [AllowAnonymous]
        [HttpGet]
        [Route("Recharge/Dispute")]
        public IHttpActionResult Dispute(string emailid, string tokenid, string Rchid, string reason)
        {
            RchSTS api = new RchSTS()
            {
                userid = emailid,
                tokenid = tokenid,
                ClientRchid = Rchid
            };

            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                if (api.userid != null && api.tokenid != null && api.ClientRchid != null)
                {
                    var userid = db.Users.Where(aa => aa.UserName == api.userid).SingleOrDefault();
                    if (userid != null)
                    {
                        var tokenchk = db.Api_ip_address.Where(aa => aa.userid == userid.UserId && aa.token == api.tokenid).SingleOrDefault();
                        if (tokenchk != null)
                        {
                            if (tokenchk.token == api.tokenid)
                            {
                                var key = userid.UserId.Substring(0, 16);
                                var usedip = Decrypt(tokenchk.token, key);
                                var currentip = GetComputer_InternetIP();
                                if (currentip == usedip)
                                {
                                    JObject obj = new JObject();
                                    System.Data.Entity.Core.Objects.ObjectParameter output = new
        System.Data.Entity.Core.Objects.ObjectParameter("output", typeof(string));

                                    var ch = db.distute_insert(api.ClientRchid, reason, output).SingleOrDefault().msg.ToString();
                                    if (ch == "Success")
                                    {
                                        obj.Add("Status", "Success");
                                        obj.Add("Errormsg", "Disputed Successfully");
                                    }
                                    else
                                    {
                                        obj.Add("Status", "Failed");
                                        obj.Add("Errormsg", "Either Already Disputed or Not Matched");
                                    }
                                    return Ok(obj);
                                }
                                else
                                {
                                    JObject rch = outputchk("Failed", "", "IP is Not Register . " + currentip + "", 0, api.ClientRchid, "");
                                    return Ok(rch);
                                }
                            }
                            else
                            {
                                JObject rch = outputchk("Failed", "", "Token Is Miss Match", 0, api.ClientRchid, "");
                                return Ok(rch);
                            }
                        }
                        else
                        {
                            JObject rch = outputchk("Failed", "", "Token Is NOt Register", 0, api.ClientRchid, "");
                            return Ok(rch);
                        }
                    }
                    else
                    {
                        JObject rch = outputchk("Failed", "", "User Id Not Register", 0, api.ClientRchid, "");
                        return Ok(rch);
                    }
                }
                else
                {
                    JObject rch = outputchk("Failed", "", "Missing Parameter", 0, api.ClientRchid, "");
                    return Ok(rch);
                }
            }
        }

        protected string IncommingMessage(string correlationId, string requestInfo, byte[] message)
        {
            return string.Format("{0} - Request: {1}\r\n{2}", correlationId, requestInfo, Encoding.UTF8.GetString(message));
        }
        public string check()
        {
            var userid = "e62cad15-ff31-41ac-a37a-0be89d761623";
            var key = userid.Substring(0, 16);
            var input = "45.248.194.10";
            var out1 = Encrypt(input, key);
            //"45wKTemPn/jbN7TrGLeApmc/yKZZtY3s+UknAj6KNipSW5xKfTqc+CCgKj6mak78K6P54g8y93k="
            return "";
        }
        public string GetComputer_InternetIP()
        {
            string ipaddress;
            ipaddress = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (ipaddress == "" || ipaddress == null)
                ipaddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            return ipaddress;
            //System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11;
            //using (WebClient client = new WebClient())
            //{
            //    ipaddress = client.DownloadString("https://api64.ipify.org");
            //}
            //return ipaddress;
        }
        public string Encrypt(string input, string key)
        {
            byte[] inputArray = UTF8Encoding.UTF8.GetBytes(input);
            TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
            tripleDES.Key = UTF8Encoding.UTF8.GetBytes(key);
            tripleDES.Mode = CipherMode.ECB;
            tripleDES.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tripleDES.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            tripleDES.Clear();
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
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
        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}
        private static System.Random random = new System.Random();
        public string RandomString(int length)
        {
            //const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string chars = "1234567890";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public IRestResponse tokencheck()
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
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
        }
        public JObject outputchk(string sts, string trnid, string errormsg, decimal remain, string yourid, string rchid)
        {
            dynamic resp = new JObject();
            resp.Status = sts;
            resp.Transid = trnid;
            resp.Errormsg = errormsg;
            resp.Remain = remain;
            resp.Yourrchid = yourid;
            resp.RechargeID = rchid;
            return resp;
        }
        public static void WriteLog(string strFileName, string strMessage)
        {
            try
            {
                StreamWriter log;
                FileStream fileStream = null;
                DirectoryInfo logDirInfo = null;
                FileInfo logFileInfo;
                string logFilePath = "C:\\Logs\\";
                logFilePath = logFilePath + "Recharge_Log-" + System.DateTime.Today.ToString("MM-dd-yyyy") + "." + "txt";
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
    }
}