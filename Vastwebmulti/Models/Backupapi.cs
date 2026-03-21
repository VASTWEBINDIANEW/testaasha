using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml;
using Vastwebmulti.Areas.RETAILER.Models;

namespace Vastwebmulti.Models
{
    public class Backupapi
    {
        VastwebmultiEntities db = new VastwebmultiEntities();
        public string recharge(string mobile, string optcode, decimal amount, string retailerid, int idno, string optcodeid, string OrderId, ref string opid)
        {
            var sts = "";

            System.Data.Entity.Core.Objects.ObjectParameter output = new System.Data.Entity.Core.Objects.ObjectParameter("Output", typeof(string));
            var rchinfo = db.Recharge_info.Where(aa => aa.Mobile == mobile && aa.amount == amount && (aa.Rstaus == "Request Send" || aa.Rstaus == "Request Sent")).SingleOrDefault();

            var rchfrom = rchinfo.Rch_from;
            var rchtype = rchinfo.rch_type;
            if (rchinfo.backupchk == "B")
            {
                db.recharge_update(idno.ToString(), "Failed", "NA", 0, "", "Response");
                try
                {
                    Backupinfo back = new Backupinfo();
                    var admininfo = db.Admin_details.SingleOrDefault();

                    if (rchtype.ToUpper() == "API")
                    {
                        var apidetails = db.api_user_details.Where(aa => aa.apiid == rchfrom).SingleOrDefault();
                        var apiinforem = db.api_remain_amount.Where(aa => aa.apiid == rchfrom).SingleOrDefault();
                        var modeln = new Backupinfo.Addinfo
                        {
                            Websitename = admininfo.WebsiteUrl,
                            RetailerID = rchfrom,
                            Email = apidetails.emailid,
                            Mobile = apidetails.mobile,
                            Details = "Recharge Refund ",
                            RemainBalance = (decimal)apiinforem.balance,
                            Usertype = "API"
                        };
                        back.Rechargeandutility(modeln);
                    }
                    else
                    {
                        var retailerdetails = db.Retailer_Details.Where(aa => aa.RetailerId == rchfrom).SingleOrDefault();
                        var dealerdetails = db.Dealer_Details.Where(aa => aa.DealerId == retailerdetails.DealerId).SingleOrDefault();
                        var masterdetails = db.Superstokist_details.Where(aa => aa.SSId == dealerdetails.SSId).SingleOrDefault();

                        var remdetails = db.Remain_reteller_balance.Where(aa => aa.RetellerId == rchfrom).SingleOrDefault();
                        var dlmdetails = db.Remain_dealer_balance.Where(aa => aa.DealerID == retailerdetails.DealerId).SingleOrDefault();
                        var Masterdetails = db.Remain_superstokist_balance.Where(aa => aa.SuperStokistID == dealerdetails.SSId).SingleOrDefault();

                        var modeln = new Backupinfo.Addinfo
                        {
                            Websitename = admininfo.WebsiteUrl,
                            RetailerID = rchfrom,
                            Email = retailerdetails.Email,
                            Mobile = retailerdetails.Mobile,
                            Details = "Recharge Refund ",
                            RemainBalance = (decimal)remdetails.Remainamount,
                            Usertype = "Retailer"
                        };
                        back.Rechargeandutility(modeln);

                        var model1 = new Backupinfo.Addinfo
                        {
                            Websitename = admininfo.WebsiteUrl,
                            RetailerID = dealerdetails.DealerId,
                            Email = dealerdetails.Email,
                            Mobile = dealerdetails.Mobile,
                            Details = "Recharge Refund ",

                            RemainBalance = Convert.ToDecimal(dlmdetails.Remainamount),
                            Usertype = "Dealer"
                        };
                        back.Rechargeandutility(model1);

                        var model2 = new Backupinfo.Addinfo
                        {
                            Websitename = admininfo.WebsiteUrl,
                            RetailerID = masterdetails.SSId,
                            Email = masterdetails.Email,
                            Mobile = masterdetails.Mobile,
                            Details = "Recharge Refund ",
                            RemainBalance = Convert.ToDecimal(Masterdetails.Remainamount),
                            Usertype = "Master"
                        };
                        back.Rechargeandutility(model2);
                    }
                }
                catch { }
                sts = "FAILED";
                return sts;
            }
            else
            {
                var idnnn = idno.ToString();
                rchinfo.backupchk = "B";
                db.SaveChanges();
                string uniqueid = OrderId;//DateTime.Parse(DateTime.Now.ToString()).ToString("yyMMddHHmmss") + RandomString(4);
                var datetime = DateTime.Now.ToString("MM.dd.yyyy hh:mm:ss");
                var backup = db.failed_recharge_move.Where(aa => aa.operator_code == optcodeid && aa.status == "Y").SingleOrDefault();
                var apiname = backup.api_name;
                var srschk = db.SRS_API.Where(aa => aa.opt_code == optcode && aa.api_nm == apiname).SingleOrDefault();
                if (srschk != null)
                {
                    if (srschk.apioptcode != null)
                    {
                        //var amt1 = amount.ToString();
                        string amt1 = Math.Floor(amount).ToString();

                        var url = srschk.api;
                        url = url.Replace("mmm", mobile);
                        url = url.Replace("ooo", srschk.apioptcode);
                        url = url.Replace("aaa", amt1);
                        url = url.Replace("iii", uniqueid);
                        url = url.Replace("ttt", datetime);

                        var portno = srschk.apiname;
                        Recharge_info obj = (from p in db.Recharge_info where p.idno == idno select p).Single();
                        obj.portno = portno;
                        obj.Recharge_request = url;
                        obj.response_output = "";
                        db.SaveChanges();
                        if (url.ToUpper().Contains("API.VASTBAZAAR.COM"))
                        {
                            int idnn111 = 0;
                            var idnoo = (from rch in db.Recharge_info where rch.Mobile == mobile where rch.amount == amount where rch.Rstaus == "Request Send" || rch.Rstaus == "Request Sent" select rch.idno).SingleOrDefault().ToString();
                            idnn111 = Convert.ToInt32(idnoo);
                            var apioptcode = db.SRS_API.Where(aa => aa.api == url && aa.opt_code == optcode).SingleOrDefault().apioptcode;
                            VastBazaartoken Responsetoken = new VastBazaartoken();
                            var tokn = Responsetoken.gettoken();
                            Vastbillpay vb = new Vastbillpay();
                            var responsechk1 = vb.billpay(tokn, mobile, apioptcode, amount.ToString(), "", "", uniqueid);

                            var responsechk = responsechk1.Content.ToString();
                            dynamic json = JsonConvert.DeserializeObject(responsechk);
                            var respcode = json.Content.ResponseCode.ToString();
                            var ADDINFO = json.Content.ADDINFO.ToString();
                            dynamic json1 = JsonConvert.DeserializeObject(ADDINFO);

                            //Recharge_info objCourse = (from p in db.Recharge_info where p.idno == idno select p).Single();
                            //objCourse.Recharge_response = json1.ToString();
                            //db.SaveChanges();

                            var status = json1.STATUS.ToString();
                            decimal PRICE = 0;
                            var errormsg = json1.ERRORMSG.ToString();
                            var operatorid = json1.TRANSID.ToString();

                            if (status.ToUpper() == "SUCCESS")
                            {
                                PRICE = Convert.ToDecimal(json1.PRICE.ToString());
                                db.recharge_update(idnnn.ToString(), "Success", operatorid, Convert.ToDecimal(PRICE), json.ToString(), "Response");
                                sts = "SUCCESS";
                            }
                            else if (status.ToUpper() == "FAILED")
                            {
                                db.recharge_update(idnnn.ToString(), "Failed", operatorid, Convert.ToDecimal(PRICE), json.ToString(), "Response");
                                sts = "FAILED";
                            }
                            else
                            {
                                sts = "PENDING";
                            }
                            opid = operatorid;
                        }
                        else if (url.ToUpper().Contains("RECHARGE/RECHARGE"))
                        {

                            var tkn = db.Recharge_API_URLS.Where(aa => aa.url == url).SingleOrDefault();
                            var token = tkn.Token;
                            var Tokenid = tkn.Token;
                            var Userid = tkn.API_ID;

                            POST_API PA = new POST_API();

                            int idnn111 = 0;
                           var idnoo = rchinfo.idno.ToString();


                            idnn111 = Convert.ToInt32(idnoo);


                            var apioptcode = db.SRS_API.Where(aa => aa.api == url && aa.opt_code == optcode).SingleOrDefault().apioptcode;
                            var responsechk1 = PA.RchReq(token, mobile, Tokenid, Userid, amount, apioptcode, uniqueid, "", "", url);


                            var responsechk = responsechk1.Content.ToString();
                            dynamic json = JsonConvert.DeserializeObject(responsechk);

                            //Recharge_info objCourse = (from p in db.Recharge_info where p.idno == idno select p).Single();
                            //objCourse.Recharge_response = json.ToString();
                            //db.SaveChanges();

                            var status = json.Status.ToString();
                            string operatorid = json.Transid.ToString();
                            var errormsg = json.Errormsg.ToString();
                            var remainamount = json.Remain.ToString();
                            var Yourrchid = json.Yourrchid.ToString();
                            var RechargeID = json.RechargeID.ToString();


                            if (status.ToUpper() == "SUCCESS")
                            {
                                db.recharge_update(idnnn.ToString(), "Success", operatorid, Convert.ToDecimal(remainamount), json.ToString(), "Response");
                                sts = "SUCCESS";
                            }
                            else if (status.ToUpper() == "FAILED")
                            {
                                db.recharge_update(idnnn.ToString(), "Failed", operatorid, Convert.ToDecimal(0), json.ToString(), "Response");
                                try
                                {
                                    Backupinfo back = new Backupinfo();
                                    var admininfo = db.Admin_details.SingleOrDefault();

                                    if (rchtype.ToUpper() == "API")
                                    {
                                        var apidetails = db.api_user_details.Where(aa => aa.apiid == rchfrom).SingleOrDefault();
                                        var apiinforem = db.api_remain_amount.Where(aa => aa.apiid == rchfrom).SingleOrDefault();
                                        var modeln = new Backupinfo.Addinfo
                                        {
                                            Websitename = admininfo.WebsiteUrl,
                                            RetailerID = rchfrom,
                                            Email = apidetails.emailid,
                                            Mobile = apidetails.mobile,
                                            Details = "Recharge Refund ",
                                            RemainBalance = (decimal)apiinforem.balance,
                                            Usertype = "API"
                                        };
                                        back.Rechargeandutility(modeln);
                                    }
                                    else
                                    {
                                        var retailerdetails = db.Retailer_Details.Where(aa => aa.RetailerId == rchfrom).SingleOrDefault();
                                        var dealerdetails = db.Dealer_Details.Where(aa => aa.DealerId == retailerdetails.DealerId).SingleOrDefault();
                                        var masterdetails = db.Superstokist_details.Where(aa => aa.SSId == dealerdetails.SSId).SingleOrDefault();

                                        var remdetails = db.Remain_reteller_balance.Where(aa => aa.RetellerId == rchfrom).SingleOrDefault();
                                        var dlmdetails = db.Remain_dealer_balance.Where(aa => aa.DealerID == retailerdetails.DealerId).SingleOrDefault();
                                        var Masterdetails = db.Remain_superstokist_balance.Where(aa => aa.SuperStokistID == dealerdetails.SSId).SingleOrDefault();

                                        var modeln = new Backupinfo.Addinfo
                                        {
                                            Websitename = admininfo.WebsiteUrl,
                                            RetailerID = rchfrom,
                                            Email = retailerdetails.Email,
                                            Mobile = retailerdetails.Mobile,
                                            Details = "Recharge Refund ",
                                            RemainBalance = (decimal)remdetails.Remainamount,
                                            Usertype = "Retailer"
                                        };
                                        back.Rechargeandutility(modeln);

                                        var model1 = new Backupinfo.Addinfo
                                        {
                                            Websitename = admininfo.WebsiteUrl,
                                            RetailerID = dealerdetails.DealerId,
                                            Email = dealerdetails.Email,
                                            Mobile = dealerdetails.Mobile,
                                            Details = "Recharge Refund ",

                                            RemainBalance = Convert.ToDecimal(dlmdetails.Remainamount),
                                            Usertype = "Dealer"
                                        };
                                        back.Rechargeandutility(model1);

                                        var model2 = new Backupinfo.Addinfo
                                        {
                                            Websitename = admininfo.WebsiteUrl,
                                            RetailerID = masterdetails.SSId,
                                            Email = masterdetails.Email,
                                            Mobile = masterdetails.Mobile,
                                            Details = "Recharge Refund ",
                                            RemainBalance = Convert.ToDecimal(Masterdetails.Remainamount),
                                            Usertype = "Master"
                                        };
                                        back.Rechargeandutility(model2);
                                    }
                                }
                                catch { }
                                sts = "FAILED";
                            }
                            else
                            {
                                sts = "PENDING";
                            }
                            opid = operatorid;
                        }
                        else if (url.ToUpper().Contains("MROBOTICS.IN"))
                        {

                            var tkn = db.Recharge_API_URLS.Where(aa => aa.url == url).SingleOrDefault();
                            var token = tkn.Token;
                            var Tokenid = tkn.Token;
                            var Userid = tkn.API_ID;

                            POST_API PA = new POST_API();

                            decimal amt = Convert.ToDecimal(amount);
                            int idnn11 = 0;
                            var idnoo = (from rch in db.Recharge_info where rch.Mobile == mobile where rch.amount == amount where rch.Rstaus == "Request Send" || rch.Rstaus == "Request Sent" select rch.idno).SingleOrDefault().ToString();
                            idnn11 = Convert.ToInt32(idnoo);
                            string CommonTranid = OrderId;//"E" + DateTime.Parse(DateTime.Now.ToString()).ToString("yyMMddHHmmss") + PA.RandomString(4);
                            var apioptcode = db.SRS_API.Where(aa => aa.api == url && aa.opt_code == optcode).SingleOrDefault().apioptcode;
                            var task = Task.Run(() =>
                            {
                                return PA.RchReqMrobotics(token, mobile, Tokenid, Userid, amt, apioptcode, CommonTranid, "", "", optcode, url);
                            });

                            bool isCompletedSuccessfully = task.Wait(TimeSpan.FromMilliseconds(120000));     // 1 minutes
                            if (isCompletedSuccessfully == true)
                            {
                                var finaloutput = task.Result;

                                var responsechk = finaloutput.Content.ToString();

                                dynamic json = JsonConvert.DeserializeObject(responsechk);


                                string remainamount = ""; string status = ""; string Transid = "";

                                status = json.status.ToString();
                                if (status.ToUpper() == "SUCCESS")
                                {
                                    Transid = json.tnx_id.ToString();
                                }
                                else
                                {
                                    Transid = "";
                                }

                                if (status.ToUpper() == "SUCCESS")
                                {
                                    remainamount = json.balance.ToString();
                                    db.recharge_update(idnnn.ToString(), "Success", Transid, Convert.ToDecimal(remainamount), json.ToString(), "Response");
                                    sts = "SUCCESS";
                                }
                                else if (status.ToUpper() == "FAILURE")
                                {
                                    db.recharge_update(idnnn.ToString(), "Failed", Transid, Convert.ToDecimal(0), json.ToString(), "Response");
                                    try
                                    {
                                        Backupinfo back = new Backupinfo();
                                        var admininfo = db.Admin_details.SingleOrDefault();

                                        if (rchtype.ToUpper() == "API")
                                        {
                                            var apidetails = db.api_user_details.Where(aa => aa.apiid == rchfrom).SingleOrDefault();
                                            var apiinforem = db.api_remain_amount.Where(aa => aa.apiid == rchfrom).SingleOrDefault();
                                            var modeln = new Backupinfo.Addinfo
                                            {
                                                Websitename = admininfo.WebsiteUrl,
                                                RetailerID = rchfrom,
                                                Email = apidetails.emailid,
                                                Mobile = apidetails.mobile,
                                                Details = "Recharge Refund ",
                                                RemainBalance = (decimal)apiinforem.balance,
                                                Usertype = "API"
                                            };
                                            back.Rechargeandutility(modeln);
                                        }
                                        else
                                        {
                                            var retailerdetails = db.Retailer_Details.Where(aa => aa.RetailerId == rchfrom).SingleOrDefault();
                                            var dealerdetails = db.Dealer_Details.Where(aa => aa.DealerId == retailerdetails.DealerId).SingleOrDefault();
                                            var masterdetails = db.Superstokist_details.Where(aa => aa.SSId == dealerdetails.SSId).SingleOrDefault();

                                            var remdetails = db.Remain_reteller_balance.Where(aa => aa.RetellerId == rchfrom).SingleOrDefault();
                                            var dlmdetails = db.Remain_dealer_balance.Where(aa => aa.DealerID == retailerdetails.DealerId).SingleOrDefault();
                                            var Masterdetails = db.Remain_superstokist_balance.Where(aa => aa.SuperStokistID == dealerdetails.SSId).SingleOrDefault();

                                            var modeln = new Backupinfo.Addinfo
                                            {
                                                Websitename = admininfo.WebsiteUrl,
                                                RetailerID = rchfrom,
                                                Email = retailerdetails.Email,
                                                Mobile = retailerdetails.Mobile,
                                                Details = "Recharge Refund ",
                                                RemainBalance = (decimal)remdetails.Remainamount,
                                                Usertype = "Retailer"
                                            };
                                            back.Rechargeandutility(modeln);

                                            var model1 = new Backupinfo.Addinfo
                                            {
                                                Websitename = admininfo.WebsiteUrl,
                                                RetailerID = dealerdetails.DealerId,
                                                Email = dealerdetails.Email,
                                                Mobile = dealerdetails.Mobile,
                                                Details = "Recharge Refund ",

                                                RemainBalance = Convert.ToDecimal(dlmdetails.Remainamount),
                                                Usertype = "Dealer"
                                            };
                                            back.Rechargeandutility(model1);

                                            var model2 = new Backupinfo.Addinfo
                                            {
                                                Websitename = admininfo.WebsiteUrl,
                                                RetailerID = masterdetails.SSId,
                                                Email = masterdetails.Email,
                                                Mobile = masterdetails.Mobile,
                                                Details = "Recharge Refund ",
                                                RemainBalance = Convert.ToDecimal(Masterdetails.Remainamount),
                                                Usertype = "Master"
                                            };
                                            back.Rechargeandutility(model2);
                                        }
                                    }
                                    catch { }
                                    sts = "FAILED";
                                }
                                else
                                {
                                    sts = "PENDING";
                                }
                                opid = Transid;
                            }
                            else
                            {
                                sts = "PENDING";
                            }

                        }
                        else
                        {
                            var apiinfo = db.RechargeapiInfoes.Where(aa => aa.apiendpoint.ToUpper() == url.ToUpper()).SingleOrDefault();
                            if (apiinfo != null)
                            {
                                ApiResponse rech = RechargeServices.Recharge(apiinfo, mobile, optcode, amount, OrderId);
                                idnnn = rech.id.ToString();

                                if (rech.status == "Success")
                                {
                                    db.recharge_update(idnnn, "Success", rech.operatorId, Convert.ToDecimal(rech.apiremain), rech.api_response, "Response");
                                    sts = "SUCCESS";
                                }
                                else if (rech.status == "Failed")
                                {
                                    sts = "FAILED";
                                    db.recharge_update(idnnn, "Failed", rech.errormsg, Convert.ToDecimal(rech.apiremain), rech.api_response, "Response");
                                    try
                                    {
                                        Backupinfo back = new Backupinfo();
                                        var admininfo = db.Admin_details.SingleOrDefault();

                                        if (rchtype.ToUpper() == "API")
                                        {
                                            var apidetails = db.api_user_details.Where(aa => aa.apiid == rchfrom).SingleOrDefault();
                                            var apiinforem = db.api_remain_amount.Where(aa => aa.apiid == rchfrom).SingleOrDefault();
                                            var modeln = new Backupinfo.Addinfo
                                            {
                                                Websitename = admininfo.WebsiteUrl,
                                                RetailerID = rchfrom,
                                                Email = apidetails.emailid,
                                                Mobile = apidetails.mobile,
                                                Details = "Recharge Refund ",
                                                RemainBalance = (decimal)apiinforem.balance,
                                                Usertype = "API"
                                            };
                                            back.Rechargeandutility(modeln);
                                        }
                                        else
                                        {
                                            var retailerdetails = db.Retailer_Details.Where(aa => aa.RetailerId == rchfrom).SingleOrDefault();
                                            var dealerdetails = db.Dealer_Details.Where(aa => aa.DealerId == retailerdetails.DealerId).SingleOrDefault();
                                            var masterdetails = db.Superstokist_details.Where(aa => aa.SSId == dealerdetails.SSId).SingleOrDefault();

                                            var remdetails = db.Remain_reteller_balance.Where(aa => aa.RetellerId == rchfrom).SingleOrDefault();
                                            var dlmdetails = db.Remain_dealer_balance.Where(aa => aa.DealerID == retailerdetails.DealerId).SingleOrDefault();
                                            var Masterdetails = db.Remain_superstokist_balance.Where(aa => aa.SuperStokistID == dealerdetails.SSId).SingleOrDefault();

                                            var modeln = new Backupinfo.Addinfo
                                            {
                                                Websitename = admininfo.WebsiteUrl,
                                                RetailerID = rchfrom,
                                                Email = retailerdetails.Email,
                                                Mobile = retailerdetails.Mobile,
                                                Details = "Recharge Refund ",
                                                RemainBalance = (decimal)remdetails.Remainamount,
                                                Usertype = "Retailer"
                                            };
                                            back.Rechargeandutility(modeln);

                                            var model1 = new Backupinfo.Addinfo
                                            {
                                                Websitename = admininfo.WebsiteUrl,
                                                RetailerID = dealerdetails.DealerId,
                                                Email = dealerdetails.Email,
                                                Mobile = dealerdetails.Mobile,
                                                Details = "Recharge Refund ",

                                                RemainBalance = Convert.ToDecimal(dlmdetails.Remainamount),
                                                Usertype = "Dealer"
                                            };
                                            back.Rechargeandutility(model1);

                                            var model2 = new Backupinfo.Addinfo
                                            {
                                                Websitename = admininfo.WebsiteUrl,
                                                RetailerID = masterdetails.SSId,
                                                Email = masterdetails.Email,
                                                Mobile = masterdetails.Mobile,
                                                Details = "Recharge Refund ",
                                                RemainBalance = Convert.ToDecimal(Masterdetails.Remainamount),
                                                Usertype = "Master"
                                            };
                                            back.Rechargeandutility(model2);
                                        }
                                    }
                                    catch { }
                                }
                                else
                                {
                                    obj.Order_id = uniqueid;
                                    db.SaveChanges();
                                    sts = "PENDING";
                                }

                            }
                            else
                            {
                                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                                HttpWebRequest WebRequestObject = (HttpWebRequest)HttpWebRequest.Create(url);
                                WebRequestObject.Timeout = (System.Int32)TimeSpan.FromSeconds(125).TotalMilliseconds;
                                WebResponse Response = WebRequestObject.GetResponse();
                                Stream WebStream = Response.GetResponseStream();
                                StreamReader Reader = new StreamReader(WebStream);
                                var webcontent = Reader.ReadToEnd();

                                if (url.ToUpper().Contains("LIVE.VASTWEBINDIA.COM"))
                                {
                                    dynamic stuff = JsonConvert.DeserializeObject(webcontent);
                                    var status = stuff.Status.ToString();
                                    var Mobile = stuff.Mobile.ToString();
                                    var Amount = stuff.Amount.ToString();
                                    var RCHID = stuff.RCHID.ToString();
                                    var Operatorid = stuff.Operatorid.ToString();
                                    var remainamount = stuff.remainamount.ToString();
                                    var LapuNumber = stuff.LapuNumber.ToString();
                                    if (status.ToUpper() == "SUCCESS")
                                    {
                                        db.recharge_update(idnnn, "Success", Operatorid, Convert.ToDecimal(Amount), webcontent, "Response");
                                        sts = "SUCCESS";
                                    }
                                    else if (status.ToUpper() == "FAILED")
                                    {
                                        db.recharge_update(idnnn, "Failed", Operatorid, Convert.ToDecimal(Amount), webcontent, "Response");
                                        try
                                        {
                                            Backupinfo back = new Backupinfo();
                                            var admininfo = db.Admin_details.SingleOrDefault();

                                            if (rchtype.ToUpper() == "API")
                                            {
                                                var apidetails = db.api_user_details.Where(aa => aa.apiid == rchfrom).SingleOrDefault();
                                                var apiinforem = db.api_remain_amount.Where(aa => aa.apiid == rchfrom).SingleOrDefault();
                                                var modeln = new Backupinfo.Addinfo
                                                {
                                                    Websitename = admininfo.WebsiteUrl,
                                                    RetailerID = rchfrom,
                                                    Email = apidetails.emailid,
                                                    Mobile = apidetails.mobile,
                                                    Details = "Recharge Refund ",
                                                    RemainBalance = (decimal)apiinforem.balance,
                                                    Usertype = "API"
                                                };
                                                back.Rechargeandutility(modeln);
                                            }
                                            else
                                            {
                                                var retailerdetails = db.Retailer_Details.Where(aa => aa.RetailerId == rchfrom).SingleOrDefault();
                                                var dealerdetails = db.Dealer_Details.Where(aa => aa.DealerId == retailerdetails.DealerId).SingleOrDefault();
                                                var masterdetails = db.Superstokist_details.Where(aa => aa.SSId == dealerdetails.SSId).SingleOrDefault();

                                                var remdetails = db.Remain_reteller_balance.Where(aa => aa.RetellerId == rchfrom).SingleOrDefault();
                                                var dlmdetails = db.Remain_dealer_balance.Where(aa => aa.DealerID == retailerdetails.DealerId).SingleOrDefault();
                                                var Masterdetails = db.Remain_superstokist_balance.Where(aa => aa.SuperStokistID == dealerdetails.SSId).SingleOrDefault();

                                                var modeln = new Backupinfo.Addinfo
                                                {
                                                    Websitename = admininfo.WebsiteUrl,
                                                    RetailerID = rchfrom,
                                                    Email = retailerdetails.Email,
                                                    Mobile = retailerdetails.Mobile,
                                                    Details = "Recharge Refund ",
                                                    RemainBalance = (decimal)remdetails.Remainamount,
                                                    Usertype = "Retailer"
                                                };
                                                back.Rechargeandutility(modeln);

                                                var model1 = new Backupinfo.Addinfo
                                                {
                                                    Websitename = admininfo.WebsiteUrl,
                                                    RetailerID = dealerdetails.DealerId,
                                                    Email = dealerdetails.Email,
                                                    Mobile = dealerdetails.Mobile,
                                                    Details = "Recharge Refund ",

                                                    RemainBalance = Convert.ToDecimal(dlmdetails.Remainamount),
                                                    Usertype = "Dealer"
                                                };
                                                back.Rechargeandutility(model1);

                                                var model2 = new Backupinfo.Addinfo
                                                {
                                                    Websitename = admininfo.WebsiteUrl,
                                                    RetailerID = masterdetails.SSId,
                                                    Email = masterdetails.Email,
                                                    Mobile = masterdetails.Mobile,
                                                    Details = "Recharge Refund ",
                                                    RemainBalance = Convert.ToDecimal(Masterdetails.Remainamount),
                                                    Usertype = "Master"
                                                };
                                                back.Rechargeandutility(model2);
                                            }
                                        }
                                        catch { }
                                        sts = "FAILED";
                                    }
                                    else
                                    {
                                        obj.Order_id = uniqueid;
                                        db.SaveChanges();
                                        sts = "PENDING";
                                    }
                                }
                                else if (url.ToUpper().Contains("VASTWEBINDIA.COM"))
                                {
                                    dynamic stuff = Newtonsoft.Json.JsonConvert.DeserializeObject(webcontent);

                                    var status = stuff.Status.ToString();
                                    mobile = stuff.Mobile.ToString();
                                    var amount1 = stuff.Amount.ToString();
                                    var R_id = stuff.RID.ToString();
                                    string operatorid = stuff.Operatorid.ToString();
                                    var remain_amount = stuff.remainamount.ToString();

                                    if (status.ToUpper() == "SUCCESS")
                                    {
                                        db.recharge_update(idnnn.ToString(), "Success", operatorid, Convert.ToDecimal(amount1), webcontent, "Response");
                                        sts = "SUCCESS";
                                    }
                                    else if (status.ToUpper() == "FAILED")
                                    {
                                        db.recharge_update(idnnn.ToString(), "Failed", operatorid, Convert.ToDecimal(amount1), webcontent, "Response");
                                        try
                                        {
                                            Backupinfo back = new Backupinfo();
                                            var admininfo = db.Admin_details.SingleOrDefault();

                                            if (rchtype.ToUpper() == "API")
                                            {
                                                var apidetails = db.api_user_details.Where(aa => aa.apiid == rchfrom).SingleOrDefault();
                                                var apiinforem = db.api_remain_amount.Where(aa => aa.apiid == rchfrom).SingleOrDefault();
                                                var modeln = new Backupinfo.Addinfo
                                                {
                                                    Websitename = admininfo.WebsiteUrl,
                                                    RetailerID = rchfrom,
                                                    Email = apidetails.emailid,
                                                    Mobile = apidetails.mobile,
                                                    Details = "Recharge Refund ",
                                                    RemainBalance = (decimal)apiinforem.balance,
                                                    Usertype = "API"
                                                };
                                                back.Rechargeandutility(modeln);
                                            }
                                            else
                                            {
                                                var retailerdetails = db.Retailer_Details.Where(aa => aa.RetailerId == rchfrom).SingleOrDefault();
                                                var dealerdetails = db.Dealer_Details.Where(aa => aa.DealerId == retailerdetails.DealerId).SingleOrDefault();
                                                var masterdetails = db.Superstokist_details.Where(aa => aa.SSId == dealerdetails.SSId).SingleOrDefault();

                                                var remdetails = db.Remain_reteller_balance.Where(aa => aa.RetellerId == rchfrom).SingleOrDefault();
                                                var dlmdetails = db.Remain_dealer_balance.Where(aa => aa.DealerID == retailerdetails.DealerId).SingleOrDefault();
                                                var Masterdetails = db.Remain_superstokist_balance.Where(aa => aa.SuperStokistID == dealerdetails.SSId).SingleOrDefault();

                                                var modeln = new Backupinfo.Addinfo
                                                {
                                                    Websitename = admininfo.WebsiteUrl,
                                                    RetailerID = rchfrom,
                                                    Email = retailerdetails.Email,
                                                    Mobile = retailerdetails.Mobile,
                                                    Details = "Recharge Refund ",
                                                    RemainBalance = (decimal)remdetails.Remainamount,
                                                    Usertype = "Retailer"
                                                };
                                                back.Rechargeandutility(modeln);

                                                var model1 = new Backupinfo.Addinfo
                                                {
                                                    Websitename = admininfo.WebsiteUrl,
                                                    RetailerID = dealerdetails.DealerId,
                                                    Email = dealerdetails.Email,
                                                    Mobile = dealerdetails.Mobile,
                                                    Details = "Recharge Refund ",

                                                    RemainBalance = Convert.ToDecimal(dlmdetails.Remainamount),
                                                    Usertype = "Dealer"
                                                };
                                                back.Rechargeandutility(model1);

                                                var model2 = new Backupinfo.Addinfo
                                                {
                                                    Websitename = admininfo.WebsiteUrl,
                                                    RetailerID = masterdetails.SSId,
                                                    Email = masterdetails.Email,
                                                    Mobile = masterdetails.Mobile,
                                                    Details = "Recharge Refund ",
                                                    RemainBalance = Convert.ToDecimal(Masterdetails.Remainamount),
                                                    Usertype = "Master"
                                                };
                                                back.Rechargeandutility(model2);
                                            }
                                        }
                                        catch { }
                                        sts = "FAILED";
                                    }
                                    else
                                    {
                                        obj.Order_id = uniqueid;
                                        db.SaveChanges();
                                        sts = "PENDING";
                                    }
                                    opid = operatorid;
                                }
                            }
                        }
                    }
                    else
                    {
                        sts = "FAILED";
                    }
                }
                else
                {
                    sts = "FAILED";
                }
                return sts;
            }
        }
        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static JObject StatusCheck(string apiname, string clientId)
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                string response = "", status = "Other", operatorid = "NA";
                JObject rch = new JObject();
                var recharege_status = db.RechargeStatusChecks.Where(s => s.apiname == apiname).SingleOrDefault();
                if (recharege_status != null)
                {
                    string url = recharege_status.url;
                    url = url.Replace("rrrr", clientId);
                    url = url.Replace("uuuu", recharege_status.userid_value);
                    url = url.Replace("tttt", recharege_status.tokenId_value);

                    if (recharege_status.method_type.ToLower() == "get")
                    {
                        response = callApi(url);
                    }
                    else
                    {
                        if (recharege_status.body == null)
                        {
                            response = callApi(url, "POST");
                        }
                        else
                        {
                            var body = recharege_status.body;
                            body = body.Replace("rrrr", clientId);
                            body = body.Replace("uuuu", recharege_status.userid_value);
                            body = body.Replace("tttt", recharege_status.tokenId_value);
                            response = callApi(url, "POST", body);
                        }
                    }

                    if (!String.IsNullOrEmpty(response))
                    {
                        if (recharege_status.response_type.ToUpper() == "JSON")
                        {
                            var responseObject = JsonConvert.DeserializeObject<JObject>(response);

                            status = responseObject[recharege_status.status_key].ToString();
                            operatorid = responseObject[recharege_status.operatorid_key].ToString();

                        }
                        else if (recharege_status.response_type.ToUpper() == "XML")
                        {
                            XmlDocument responseObject = new XmlDocument();
                            responseObject.LoadXml(response);

                            status = responseObject.GetElementsByTagName(recharege_status.status_key)[0].InnerText;
                            operatorid = responseObject.GetElementsByTagName(recharege_status.operatorid_key)[0].InnerText;

                        }
                        else if (recharege_status.response_type.ToUpper() == "CSV")
                        {
                            char spliter = Convert.ToChar(recharege_status.separated_by);
                            var responseObject = response.Split(spliter);

                            status = responseObject[Convert.ToInt32(recharege_status.status_key)];
                            operatorid = responseObject[Convert.ToInt32(recharege_status.operatorid_value)];
                        }

                        if (status.ToUpper() == recharege_status.status_success_value)
                        {
                            status = "Success";
                        }
                        else if (status.ToUpper() == recharege_status.status_failed_value)
                        {
                            status = "Failed";
                        }

                    }
                }

                rch = outputchk(status, operatorid, "", 0, clientId, "");
                return rch;
            }
        }

        public static JObject Status_Check(string apiname, string clientId)
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                string response = "", status = "Unknown", operatorid = "NA";
                JObject rch = new JObject();
                var recharege_status = db.RechargeStatusChecks.Where(s => s.apiname == apiname).SingleOrDefault();
                if (recharege_status != null)
                {
                    string url = recharege_status.url;
                    url = url.Replace("rrrr", clientId);
                    url = url.Replace("uuuu", recharege_status.userid_value);
                    url = url.Replace("tttt", recharege_status.tokenId_value);

                    if (recharege_status.method_type.ToLower() == "get")
                    {
                        response = callApi(url);
                    }
                    else
                    {
                        if (recharege_status.body == null)
                        {
                            response = callApi(url, "POST");
                        }
                        else
                        {
                            var body = recharege_status.body;
                            body = body.Replace("rrrr", clientId);
                            body = body.Replace("uuuu", recharege_status.userid_value);
                            body = body.Replace("tttt", recharege_status.tokenId_value);
                            response = callApi(url, "POST", body);
                        }
                    }

                    if (!String.IsNullOrEmpty(response))
                    {
                        if (recharege_status.response_type.ToUpper() == "JSON")
                        {
                            var responseObject = JsonConvert.DeserializeObject<JObject>(response);

                            status = responseObject[recharege_status.status_key].ToString();
                            operatorid = responseObject[recharege_status.operatorid_key].ToString();

                        }
                        else if (recharege_status.response_type.ToUpper() == "XML")
                        {
                            XmlDocument responseObject = new XmlDocument();
                            responseObject.LoadXml(response);

                            status = responseObject.GetElementsByTagName(recharege_status.status_key)[0].InnerText;
                            operatorid = responseObject.GetElementsByTagName(recharege_status.operatorid_key)[0].InnerText;

                        }
                        else if (recharege_status.response_type.ToUpper() == "CSV")
                        {
                            char spliter = Convert.ToChar(recharege_status.separated_by);
                            var responseObject = response.Split(spliter);

                            status = responseObject[Convert.ToInt32(recharege_status.status_key)];
                            operatorid = responseObject[Convert.ToInt32(recharege_status.operatorid_value)];
                        }

                        if (status.ToUpper() == recharege_status.status_success_value)
                        {
                            status = "Success";
                        }
                        else if (status.ToUpper() == recharege_status.status_failed_value)
                        {
                            status = "Failed";
                        }

                    }
                }

                rch = outputchk(status, operatorid, "", 0, clientId, "");
                return rch;
            }
        }

        private static string callApi(string url, string type = "GET", string body = null)
        {
            if (type.ToUpper() == "GET")
            {
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                HttpWebRequest WebRequestObject = (HttpWebRequest)HttpWebRequest.Create(url);
                WebRequestObject.Timeout = (System.Int32)TimeSpan.FromSeconds(125).TotalMilliseconds;
                WebResponse Response = WebRequestObject.GetResponse();
                Stream WebStream = Response.GetResponseStream();
                StreamReader Reader = new StreamReader(WebStream);
                return Reader.ReadToEnd();
            }
            else
            {
                var client = new RestClient(url);
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", body, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                return response.Content;
            }
        }

        public static JObject outputchk(string sts, string trnid, string errormsg, decimal remain, string yourid, string rchid)
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


    }
}