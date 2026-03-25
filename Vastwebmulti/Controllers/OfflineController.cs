using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Xml;
using Vastwebmulti.Areas.RETAILER.Models;
using Vastwebmulti.Models;

namespace Vastwebmulti.Controllers
{
    /// <summary>
    /// Handles offline (SMS-triggered) recharge and balance operations. Processes incoming
    /// SMS commands from dealers and retailers to check balances, transfer funds, and
    /// initiate mobile recharges without a web interface.
    /// </summary>
    public class OfflineController : Controller
    {
        private VastwebmultiEntities db = new VastwebmultiEntities();

        /// <summary>
        /// Renders the default Index view for the Offline module.
        /// </summary>
        /// <returns>The default Index view.</returns>
        // GET: Offline
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Processes an offline SMS command sent from a mobile number. Supports "DynamicService"
        /// type commands including balance check (CB), balance transfer (BT for dealers), and
        /// mobile recharge requests (for retailers). Sends SMS confirmations via the priority SMS API.
        /// </summary>
        /// <param name="frmmobile">The mobile number from which the SMS was received.</param>
        /// <param name="first">The first keyword of the command (e.g., "CB" for check balance, "BT" for balance transfer, or operator code for recharge).</param>
        /// <param name="second">The second field of the command (e.g., target retailer ID for BT, or mobile number to recharge).</param>
        /// <param name="third">The third field of the command (e.g., amount for BT or recharge).</param>
        /// <param name="type">The type of service being requested (e.g., "DynamicService").</param>
        /// <returns>The default view after processing the command.</returns>
        public ActionResult Message(string frmmobile, string first, string second, string third, string type)
        {
            if (type == "DynamicService")
            {
                try
                {

                    System.Data.Entity.Core.Objects.ObjectParameter output = new
                                              System.Data.Entity.Core.Objects.ObjectParameter("output", typeof(string));

                    string OrderId = DateTime.Parse(DateTime.Now.ToString()).ToString("yyMMddHHmmss") + RandomString(4);

                    var countdlm = (db.Dealer_Details.Where(a => a.Mobile == frmmobile).Count());
                    var countrem = (db.Retailer_Details.Where(a => a.Mobile == frmmobile).Count());
                    //Dealer Part
                    if (countdlm > 0)
                    {
                        var dlmid = db.Dealer_Details.Where(a => a.Mobile == frmmobile).Single().DealerId;
                        var remain = db.Remain_dealer_balance.Where(a => a.DealerID == dlmid).Single().Remainamount;
                        //Check Balance
                        if (first.ToUpper() == "CB")
                        {

                            var text = "Your Dealer Id " + dlmid + " Reamin Amount is " + remain + "";

                            var msg = db.priority_sms_send_new(frmmobile, text, "Check Balance", output).Single().msg;
                            if (msg != "free" && msg != "sim")
                            {
                                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(msg);
                                request.Timeout = 15000;
                                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                                StreamReader sr = new StreamReader(response.GetResponseStream());
                                var s = sr.ReadToEnd();

                                sms_api_entry sms = new sms_api_entry();
                                sms.apiname = msg;
                                sms.msg = text;

                                sms.m_date = System.DateTime.Now;
                                sms.response = s;
                                db.sms_api_entry.Add(sms);
                                db.SaveChanges();

                            }
                        }
                        //balance Transfer
                        else if (first.ToUpper() == "BT")
                        {

                            decimal bal = Convert.ToDecimal(third);

                            var chk = db.insert_dealer_to_retailer_balance(dlmid, second, bal, 0, "Cash", "", "", "", "", "", "", "", output).Single().msg;
                            remain = db.Remain_dealer_balance.Where(a => a.DealerID == dlmid).Single().Remainamount;
                            var remainretailer = db.Remain_reteller_balance.Where(a => a.RetellerId == second).Single().Remainamount;
                            var remmobile = (db.Retailer_Details.Where(kk => kk.RetailerId == second).Single().Mobile);
                            var textdlm = "INR. " + third + " has been Dr to ur ID " + dlmid + " and Cr to " + second + " on " + System.DateTime.Now + " .Remain Rs . " + remain + "";
                            var textrem = "INR. " + third + " has been Cr to ur ID " + second + " on " + System.DateTime.Now + " .Remain Rs ." + remainretailer + "";

                            if (chk == "Balance Transfer Successfully.")
                            {
                                //Dealer Message
                                var msg = db.priority_sms_send_new(frmmobile, textdlm, "Balance Transfer", output).Single().msg;
                                if (msg != "free" && msg != "sim")
                                {
                                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(msg);
                                    request.Timeout = 15000;
                                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                                    StreamReader sr = new StreamReader(response.GetResponseStream());
                                    var s = sr.ReadToEnd();

                                    sms_api_entry sms = new sms_api_entry();
                                    sms.apiname = msg;
                                    sms.msg = textdlm;

                                    sms.m_date = System.DateTime.Now;
                                    sms.response = s;
                                    db.sms_api_entry.Add(sms);
                                    db.SaveChanges();

                                }
                                //Retailer Message
                                msg = db.priority_sms_send_new(remmobile, textrem, "Balance Transfer", output).Single().msg;
                                if (msg != "free" && msg != "sim")
                                {
                                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(msg);
                                    request.Timeout = 15000;
                                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                                    StreamReader sr = new StreamReader(response.GetResponseStream());
                                    var s = sr.ReadToEnd();

                                    sms_api_entry sms = new sms_api_entry();
                                    sms.apiname = msg;
                                    sms.msg = textrem;

                                    sms.m_date = System.DateTime.Now;
                                    sms.response = s;
                                    db.sms_api_entry.Add(sms);
                                    db.SaveChanges();

                                }

                            }


                        }

                    }
                    //retailer Part
                    else if (countrem > 0)
                    {
                        var remid = db.Retailer_Details.Where(m => m.Mobile == frmmobile).Single();
                        var remain = (db.Remain_reteller_balance.Where(s => s.RetellerId == remid.RetailerId).Single().Remainamount);
                        //check Balance
                        if (first.ToUpper() == "CB")
                        {
                            var text = "Your Retailer Id " + remid.Email + " Reamin Amount is " + remain + "";
                            var msg = db.priority_sms_send_new(frmmobile, text, "Check Balance", output).Single().msg;
                            if (msg != "free" && msg != "sim")
                            {
                                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(msg);
                                request.Timeout = 15000;
                                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                                StreamReader sr = new StreamReader(response.GetResponseStream());
                                var s = sr.ReadToEnd();
                                sms_api_entry sms = new sms_api_entry();
                                sms.apiname = msg;
                                sms.msg = text;

                                sms.m_date = System.DateTime.Now;
                                sms.response = s;
                                db.sms_api_entry.Add(sms);
                                db.SaveChanges();

                            }
                        }
                        //Recharge
                        else
                        {
                            var result = "";
                            var aaa = Convert.ToDecimal(third);
                            var operatorname = db.Operator_Code.Where(a => a.new_opt_code == first).Single().operator_Name;
                            var Blockstatus = db.AmountBlockunblocks.Where(a => a.OperatorName == operatorname && a.Amount == aaa && a.status == "Y").ToList();
                            if (Blockstatus.Count > 0)
                            {
                                result = "This Amount is Blocked by Admin.";
                            }
                            else
                            {
                                var commtime = db.utility_comm_bytiming.SingleOrDefault();
                                if (commtime == null)
                                {
                                    utility_comm_bytiming per = new utility_comm_bytiming();
                                    per.commper = 100;
                                    per.Date = DateTime.Now;
                                    per.Typefor = "Utilities";
                                    db.utility_comm_bytiming.Add(per);
                                    db.SaveChanges();
                                }

                                var ch = db.Recharge_retailer(remid.RetailerId, second, first, Convert.ToInt32(third), "Offline", "", "", "", "", "", "", "", 0, OrderId, "",false, output).Single().msg;

                                if (ch == "OKK")
                                {
                                    result = "Recharge Processed Successfully.";
                                }
                                else if (ch == "MOK")
                                {
                                    result = "Recharge Processed Successfully.";
                                }
                                else if (ch == "AOK")
                                {
                                    result = "Recharge Processed Successfully.";
                                }
                                else if (ch == "CAPINGLOW")
                                {
                                    result = "Amount Less Then Capping Amount.";

                                }
                                else if (ch == "AMOUNTLOW")
                                {
                                    result = "Invalid Amount.";

                                }
                                else if (ch.ToString() == "TIMEDIFF")
                                {
                                    result = "Your Recharge is Allready Success. Please Try After Some Time.";
                                }
                                else if (ch.ToString() == "BALANCELOW")
                                {
                                    result = "Your Balance is Low, Please Recharge Your Account. Your Current Balance is " + remain;

                                }
                                else if (ch.ToString() == "LOWBAL")
                                {
                                    result = "Your Balance is Low, Please Recharge Your Account. Your Current Balance is " + remain;
                                }
                                else if (ch.ToString() == "IDBLOCK")
                                {
                                    result = "Your ID is Blocked.";

                                    System.Data.Entity.Core.Objects.ObjectParameter output1 = new System.Data.Entity.Core.Objects.ObjectParameter("Output", typeof(string));

                                    var msg1 = db.priority_sms_send_new(frmmobile, result, "Check Balance", output1).Single().msg;
                                    if (msg1 != "free" && msg1 != "sim")
                                    {
                                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                                        HttpWebRequest WebRequestObject = (HttpWebRequest)HttpWebRequest.Create(msg1);
                                        WebRequestObject.Timeout = (System.Int32)TimeSpan.FromSeconds(10).TotalMilliseconds;
                                        try
                                        {
                                            WebResponse Response = WebRequestObject.GetResponse();
                                            Stream WebStream = Response.GetResponseStream();

                                            StreamReader Reader = new StreamReader(WebStream);
                                            string webcontent = Reader.ReadToEnd();

                                            sms_api_entry sms = new sms_api_entry();
                                            sms.apiname = msg1;
                                            sms.msg = result;

                                            sms.m_date = System.DateTime.Now;
                                            sms.response = webcontent;
                                            db.sms_api_entry.Add(sms);
                                            db.SaveChanges();

                                        }
                                        catch
                                        {

                                        }
                                    }
                                }
                                else if (ch.ToString() == "MINBAL")
                                {
                                    result = "Recharge Minimum 10 Rs.";


                                    var msg1 = db.priority_sms_send_new(frmmobile, result, "Check Balance", output).Single().msg;
                                    if (msg1 != "free" && msg1 != "sim")
                                    {
                                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                                        HttpWebRequest WebRequestObject = (HttpWebRequest)HttpWebRequest.Create(msg1);
                                        WebRequestObject.Timeout = (System.Int32)TimeSpan.FromSeconds(10).TotalMilliseconds;
                                        try
                                        {
                                            WebResponse Response = WebRequestObject.GetResponse();
                                            Stream WebStream = Response.GetResponseStream();

                                            StreamReader Reader = new StreamReader(WebStream);
                                            string webcontent = Reader.ReadToEnd();

                                            sms_api_entry sms = new sms_api_entry();
                                            sms.apiname = msg1;
                                            sms.msg = result;

                                            sms.m_date = System.DateTime.Now;
                                            sms.response = webcontent;
                                            db.sms_api_entry.Add(sms);
                                            db.SaveChanges();

                                        }
                                        catch
                                        {

                                        }
                                    }

                                }
                                else if (ch.ToString() == "OPTBLOCK")
                                {
                                    string date = (from opert in db.Operator_Code where opert.new_opt_code == first select opert.blocktime).SingleOrDefault().ToString();
                                    var optnm = db.Operator_Code.Where(oo => oo.new_opt_code == first).Single().operator_Name;
                                    if (date == "")
                                    {
                                        result = "" + optnm + " Recharge is not Working Now. Please Recharge After Some Time.";

                                    }
                                    else
                                    {
                                        string st = Convert.ToDateTime(date).ToString("hh:mm tt dd/MM/yyyy");
                                        optnm = db.Operator_Code.Where(oo => oo.new_opt_code == first).Single().operator_Name;
                                        result = "" + optnm + " Recharge is not Working Now. Please Recharge After " + st + "";

                                    }
                                }
                                else
                                {
                                    var url = ch.ToString();
                                    if (url.Contains("cyberplate"))
                                    {
                                        Cyberplate cb = new Cyberplate();

                                        var chkres = "";
                                        if (first.ToUpper() == "JIO")
                                        {
                                            chkres = cb.rechargeJIO(second, Convert.ToString(third), first, "");
                                        }
                                        else
                                        {
                                            chkres = cb.recharge(second, Convert.ToString(third), first);
                                        }
                                        if (chkres == "SUCCESS")
                                        {
                                            result = "Recharge SUCCESS.";
                                        }
                                        else if (chkres == "FAILED")
                                        {
                                            result = "Recharge FAILED.";
                                        }
                                        else
                                        {
                                            result = "Recharge Processed Successfully.";
                                        }
                                    }
                                    else if (url.ToUpper().Contains("RECHARGE/RECHARGE"))
                                    {

                                        var tkn = db.Recharge_API_URLS.Where(aa => aa.url == url).SingleOrDefault();
                                        var token = tkn.Token;
                                        var Tokenid = tkn.Token;
                                        var Userid = tkn.API_ID;

                                        POST_API PA = new POST_API();

                                        int idnn111 = 0;
                                        var idno = (from rch in db.Recharge_info where rch.Mobile == second where rch.amount == Convert.ToDecimal(third) where rch.Rstaus == "Request Send" || rch.Rstaus == "Request Sent" select rch.idno).SingleOrDefault().ToString();
                                        idnn111 = Convert.ToInt32(idno);
                                        string CommonTranid = "E" + DateTime.Parse(DateTime.Now.ToString()).ToString("yyMMddHHmmss") + PA.RandomString(4);
                                        var apioptcode = db.SRS_API.Where(aa => aa.api == url && aa.opt_code == second).SingleOrDefault().apioptcode;
                                        var responsechk1 = PA.RchReq(token, second, Tokenid, Userid, Convert.ToDecimal(third), apioptcode, CommonTranid, "", "", url);

                                        var responsechk = responsechk1.Content.ToString();
                                        dynamic json = JsonConvert.DeserializeObject(responsechk);

                                        var Request = responsechk1.Request.Parameters[2].Value;
                                        var ReqUrl = url + "  RequestBody : " + Request;

                                        Recharge_info objCourse = (from p in db.Recharge_info where p.idno == idnn111 select p).Single();
                                        objCourse.Recharge_response = json.ToString();
                                        objCourse.Recharge_request = ReqUrl;
                                        objCourse.Order_id = CommonTranid.ToString();
                                        db.SaveChanges();

                                        var status = json.Status.ToString();
                                        var operatorid = json.Transid.ToString();
                                        var errormsg = json.Errormsg.ToString();
                                        var remainamount = json.Remain.ToString();
                                        var Yourrchid = json.Yourrchid.ToString();
                                        var RechargeID = json.RechargeID.ToString();

                                        if (status.ToUpper() == "SUCCESS")
                                        {
                                            db.recharge_update(idnn111.ToString(), "Success", operatorid, Convert.ToInt32(remainamount), json.ToString(), "Response");
                                            result = "Recharge Processed Successfully.";
                                        }
                                        else if (status.ToUpper() == "FAILED")
                                        {
                                            db.recharge_update(idnn111.ToString(), "Failed", errormsg, 0, json.ToString(), "Response");
                                        }
                                    }
                                    else
                                    {

                                        var idno = "";
                                        //var sts = "";
                                        //var optid = "";
                                        var status = ""; var mobile = ""; var operator_code = ""; var R_id = ""; var operatorid = ""; var reason_chk = "";
                                        var remain_amount = ""; var amount = "";
                                        var count = (from ff in db.Recharge_info where ff.Recharge_request == url select ff).Count();
                                        if (count > 0)
                                        {

                                            HttpWebRequest WebRequestObject = (HttpWebRequest)HttpWebRequest.Create(url);
                                            WebRequestObject.Timeout = (System.Int32)TimeSpan.FromSeconds(25).TotalMilliseconds;
                                            try
                                            {
                                                WebResponse Response = WebRequestObject.GetResponse();
                                                Stream WebStream = Response.GetResponseStream();

                                                StreamReader Reader = new StreamReader(WebStream);
                                                string webcontent = Reader.ReadToEnd();

                                                //var ammt = Convert.ToDecimal(third);
                                                //idno = (from rch in db.Recharge_info where rch.Mobile == second where rch.amount == ammt where rch.Rstaus == "Request Send" || rch.Rstaus == "Request Sent" select rch.idno).SingleOrDefault().ToString();
                                                ////int idnn = Convert.ToInt32(idno);
                                                //TransactionMaster objCourse = (from p in db.TransactionMasters where p.IdNo == idnn select p).Single();
                                                //objCourse.response = webcontent.ToString();
                                                //db.SaveChanges();

                                                if (url.ToUpper().Contains("RECHARGE_REQ"))
                                                {
                                                    if (url.ToUpper().Contains("TXT"))
                                                    {
                                                        var dictionary = webcontent.Split(
                                                new[] { "\r\n" },
                                                StringSplitOptions.RemoveEmptyEntries)
                                            .Select(x => x.Split(':'))
                                            .ToDictionary(
                                                k => k[0].Trim(),
                                                v => v[1].Trim());
                                                        status = dictionary["status"].ToString();
                                                        mobile = dictionary["Mobile"].ToString();
                                                        amount = dictionary["Amount"].ToString();
                                                        operator_code = dictionary["operator"].ToString();
                                                        R_id = dictionary["RID"].ToString();
                                                        operatorid = dictionary["Operatorid"].ToString();
                                                        reason_chk = dictionary["reason"].ToString();
                                                        remain_amount = dictionary["remainamount"].ToString();
                                                    }
                                                    else if (url.ToUpper().Contains("JSON"))
                                                    {
                                                        dynamic stuff = JsonConvert.DeserializeObject(webcontent);

                                                        status = stuff.status;
                                                        mobile = stuff.Mobile;
                                                        amount = stuff.Amount;
                                                        operator_code = stuff.@operator;
                                                        R_id = stuff.RID;
                                                        operatorid = stuff.Operatorid;
                                                        reason_chk = stuff.reason;
                                                        remain_amount = stuff.remainamount;
                                                    }
                                                    else
                                                    {
                                                        XmlDocument xd = new XmlDocument();
                                                        xd.LoadXml(webcontent);
                                                        var sts1 = xd.SelectNodes("nodedata/NODE/status");
                                                        var mob = xd.SelectNodes("nodedata/NODE/Mobile");
                                                        var amt = xd.SelectNodes("nodedata/NODE/Amount");
                                                        var opr = xd.SelectNodes("nodedata/NODE/operator");
                                                        var rid = xd.SelectNodes("nodedata/NODE/RID");
                                                        var optid1 = xd.SelectNodes("nodedata/NODE/Operatorid");
                                                        var reason = xd.SelectNodes("nodedata/NODE/reason");
                                                        var remain1 = xd.SelectNodes("nodedata/NODE/remainamount");

                                                        status = sts1[0].InnerText;
                                                        mobile = mob[0].InnerText;
                                                        amount = amt[0].InnerText;
                                                        operator_code = opr[0].InnerText;
                                                        R_id = rid[0].InnerText;
                                                        operatorid = optid1[0].InnerText;
                                                        reason_chk = reason[0].InnerText;
                                                        remain_amount = remain1[0].InnerText;
                                                    }
                                                    if (status.ToUpper().Contains("REQ"))
                                                    {
                                                        decimal amt = Convert.ToDecimal(amount);
                                                        var entry = (from rch in db.Recharge_info
                                                                     where rch.Mobile == mobile
                                                                     where rch.amount == amt
                                                                   && rch.Rstaus.ToUpper().Contains("REQ")
                                                                     select new
                                                                     {
                                                                         Idno = rch.idno,
                                                                         Mobile = rch.Mobile,
                                                                         Port = rch.portno,
                                                                         Amount = rch.amount,
                                                                         Provider = rch.optcode


                                                                     }).SingleOrDefault();

                                                        if (status == "SUCCESS")
                                                        {
                                                            reason_chk = operatorid;
                                                            db.recharge_update(idno, "Success", reason_chk, 0, webcontent.ToString(), "Response");
                                                            result = "Recharge Processed Successfully.";
                                                        }
                                                        else if (status == "FAILED")
                                                        {
                                                            db.recharge_update(idno, "Success", reason_chk, 0, webcontent.ToString(), "Response");
                                                        }

                                                    }
                                                }

                                            }
                                            catch
                                            {

                                            }

                                        }

                                    }
                                }
                            }
                            if (!result.Contains("Processed"))
                            {
                                var msg = db.priority_sms_send_new(frmmobile, result, "Recharge", output).Single().msg;
                                if (msg != "free" && msg != "sim")
                                {
                                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                                    HttpWebRequest WebRequestObject = (HttpWebRequest)HttpWebRequest.Create(msg);
                                    WebRequestObject.Timeout = (System.Int32)TimeSpan.FromSeconds(10).TotalMilliseconds;
                                    try
                                    {
                                        WebResponse Response = WebRequestObject.GetResponse();
                                        Stream WebStream = Response.GetResponseStream();

                                        StreamReader Reader = new StreamReader(WebStream);
                                        string webcontent = Reader.ReadToEnd();

                                        sms_api_entry sms = new sms_api_entry();
                                        sms.apiname = msg;
                                        sms.msg = result;

                                        sms.m_date = System.DateTime.Now;
                                        sms.response = webcontent;
                                        db.sms_api_entry.Add(sms);
                                        db.SaveChanges();


                                    }
                                    catch
                                    {

                                    }
                                }
                            }
                        }
                    }
                }
                catch
                {
                }
                finally
                {
                }
            }
            return View();
        }

        private static Random random = new Random();

        /// <summary>
        /// Generates a random numeric string of the specified length, used for creating unique order IDs.
        /// </summary>
        /// <param name="length">The number of characters in the generated string.</param>
        /// <returns>A random numeric string of the given length.</returns>
        public string RandomString(int length)
        {
            const string chars = "1234567890";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
