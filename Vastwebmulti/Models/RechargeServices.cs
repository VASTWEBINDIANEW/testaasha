using Newtonsoft.Json;
using RestSharp;
using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Xml;

namespace Vastwebmulti.Models
{
    public class RechargeServices
    {
        public static ApiResponse Recharge(RechargeapiInfo apiinfo, string mobile, string optcode, decimal amount, string OrderId)
        {
            VastwebmultiEntities db = new VastwebmultiEntities();
            ApiResponse result = new ApiResponse();
            try
            {
                var idno = (from rch in db.Recharge_info where rch.Mobile == mobile where rch.amount == amount where rch.Rstaus == "Request Send" || rch.Rstaus == "Request Sent" select rch.idno).SingleOrDefault().ToString();
                int RechId = Convert.ToInt32(idno);
                var apiremain = "0";
                var api_response = "";
                var _status = "Pending";
                var operatorId = "NA";
                var errormsg = "";
                var stsapi = "OK";
                var requrl = "";
                string amt = amount.ToString();

                var apioptocdechk = db.SRS_API.Where(aa => aa.api == apiinfo.apiendpoint && aa.opt_code == optcode).SingleOrDefault();
                var opttype = db.Operator_Code.Where(aa => aa.new_opt_code == optcode).Single().Operator_type;

                if (opttype.ToUpper() == "PREPAID" || opttype.ToUpper() == "DTH")
                {
                    amt = Convert.ToInt32(amount).ToString();
                }

                if (apiinfo.apitype == "Get")
                {
                    string finalurl = apiinfo.apiendpoint
.Replace(apiinfo.Keyuserid + "=", apiinfo.Keyuserid + "=" + apiinfo.valuserid)
.Replace(apiinfo.Keypasscode + "=", apiinfo.Keypasscode + "=" + apiinfo.valpasscode)
.Replace("&" + apiinfo.Keyrechargeno + "=", "&" + apiinfo.Keyrechargeno + "=" + mobile)
.Replace("&" + apiinfo.Keyoptcode + "=", "&" + apiinfo.Keyoptcode + "=" + apioptocdechk.apioptcode)
.Replace("&" + apiinfo.KeyAmount + "=", "&" + apiinfo.KeyAmount + "=" + amt)
.Replace("&" + apiinfo.KeyReqid + "=", "&" + apiinfo.KeyReqid + "=" + OrderId);

                    var url_update = db.Recharge_info.Single(s => s.idno == RechId);
                    url_update.Recharge_request = finalurl;

                    ServicePointManager.SecurityProtocol =
SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                    var client = new RestClient(finalurl);
                    var request = new RestRequest(Method.GET);
                    IRestResponse response = client.Execute(request);

                    api_response = response.Content;
                    if (url_update.backupchk == null)
                    {
                        url_update.Recharge_response = response.Content;
                    }
                    //else if(url_update.backupchk == "B")
                    //{
                    //    url_update.Recharge_response = url_update.Recharge_response + ", Backup: " + response.Content;
                    //}
                    db.SaveChanges();

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var apiresp = db.rechargeapiresponses.Where(aa => aa.id == apiinfo.id).SingleOrDefault();

                        if (apiresp.Resptype == "JSON")
                        {
                            dynamic respp = JsonConvert.DeserializeObject(response.Content);
                            var sts1 = (string)respp[apiresp.KeyStatus];
                            var optid = "NA";
                            var msg = "";
                            var rembal = "0";
                            try
                            {
                                rembal = respp[apiresp.KeyRemainBal];
                                apiremain = Convert.ToDecimal(rembal).ToString();
                            }
                            catch { apiremain = "0"; }

                            var failedResp = apiresp.ValStatusFailed.Split(',');
                            var isfail = failedResp.Any(s => s.ToUpper() == sts1.ToUpper());

                            if (isfail)
                            {
                                _status = "Failed";
                                msg = String.IsNullOrEmpty(apiresp.KeyMsg) ? "" : respp[apiresp.KeyMsg];
                                errormsg = msg;
                            }
                            else if (sts1.ToUpper() == apiresp.ValStatusSucess.ToUpper())
                            {
                                _status = "Success";
                                optid = respp[apiresp.KeyOptID];
                                operatorId = optid;
                            }
                            else if (sts1 == apiresp.ValStatusPending)
                            {
                                _status = "Pending";
                            }
                        }
                        else if (apiresp.Resptype == "XML")
                        {
                            XmlDocument xdoc = new XmlDocument();
                            xdoc.LoadXml(response.Content);
                            string status = xdoc.GetElementsByTagName(apiresp.KeyStatus)[0].InnerText;
                            string optid = "NA";
                            string remark = "";
                            string remain = "0";
                            try
                            {
                                remain = xdoc.GetElementsByTagName(apiresp.KeyRemainBal)[0].InnerText;
                                apiremain = Convert.ToDecimal(remain).ToString();
                            }
                            catch { apiremain = "0"; }

                            var failedResp = apiresp.ValStatusFailed.Split(',');
                            var isfail = failedResp.Any(s => s.ToUpper() == status.ToUpper());

                            if (isfail)
                            {
                                _status = "Failed";
                                remark = String.IsNullOrEmpty(apiresp.KeyMsg) ? "" : xdoc.GetElementsByTagName(apiresp.KeyMsg)[0].InnerText;
                                errormsg = remark;
                            }
                            else if (status.ToUpper() == apiresp.ValStatusSucess.ToUpper())
                            {
                                _status = "Success";
                                optid = xdoc.GetElementsByTagName(apiresp.KeyOptID)[0].InnerText;
                                operatorId = optid;
                            }
                            else
                            {
                                _status = "Pending";
                            }

                        }
                        else if (apiresp.Resptype == "TXT")
                        {
                            string[] txtresponse = response.Content.Split(Convert.ToChar(apiresp.sapratewith));
                            var status = txtresponse[Convert.ToInt32(apiresp.KeyStatus)];
                            var optid = "NA";
                            var msg = "0";
                            string remainbal = "0";
                            try
                            {
                                remainbal = txtresponse[Convert.ToInt32(apiresp.KeyRemainBal)];
                                apiremain = Convert.ToDecimal(remainbal).ToString();
                            }
                            catch { apiremain = "0"; }

                            var failedResp = apiresp.ValStatusFailed.Split(',');
                            var isfail = failedResp.Any(s => s.ToUpper() == status.ToUpper());

                            if (isfail)
                            {
                                _status = "Failed";
                                msg = String.IsNullOrEmpty(apiresp.KeyMsg) ? "" : txtresponse[Convert.ToInt32(apiresp.KeyMsg)];
                                errormsg = msg;
                            }
                            else if (status.ToUpper() == apiresp.ValStatusSucess.ToUpper())
                            {
                                _status = "Success";
                                optid = txtresponse[Convert.ToInt32(apiresp.KeyOptID)];
                                operatorId = optid;
                            }
                            else
                            {
                                _status = "Pending";
                            }
                        }
                    }
                    else
                    {
                        _status = "Pending";
                        errormsg = "Response code Invlid";
                    }
                }
                else if (apiinfo.apitype == "Post")
                {
                    var url = apiinfo.apiendpoint;
                    if (apiinfo.Requesttype == "Query String")
                    {
                        string finalurl = apiinfo.apiendpoint
.Replace(apiinfo.Keyuserid + "=", apiinfo.Keyuserid + "=" + apiinfo.valuserid)
.Replace(apiinfo.Keypasscode + "=", apiinfo.Keypasscode + "=" + apiinfo.valpasscode)
.Replace("&" + apiinfo.Keyrechargeno + "=", "&" + apiinfo.Keyrechargeno + "=" + mobile)
.Replace("&" + apiinfo.Keyoptcode + "=", "&" + apiinfo.Keyoptcode + "=" + apioptocdechk.apioptcode)
.Replace("&" + apiinfo.KeyAmount + "=", "&" + apiinfo.KeyAmount + "=" + amt)
.Replace("&" + apiinfo.KeyReqid + "=", "&" + apiinfo.KeyReqid + "=" + OrderId);

                        requrl = finalurl;
                        var url_update = db.Recharge_info.Single(s => s.idno == RechId);
                        url_update.Recharge_request = finalurl;
                        db.SaveChanges();

                        url = finalurl;
                    }
                    var resp = "";
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls |
                        SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                    var client = new RestClient(url);
                    var request = new RestRequest(Method.POST);
                    if (apiinfo.Requesttype == "Form")
                    {
                        request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                        if (apiinfo.Keyuserid != "")
                        {
                            request.AddParameter(apiinfo.Keyuserid, apiinfo.valuserid);
                        }
                        request.AddParameter(apiinfo.Keypasscode, apiinfo.valpasscode);
                        request.AddParameter(apiinfo.Keyrechargeno, mobile);
                        request.AddParameter(apiinfo.KeyAmount, amt);
                        request.AddParameter(apiinfo.Keyoptcode, apioptocdechk.apioptcode);
                        request.AddParameter(apiinfo.KeyReqid, OrderId);
                        if (apiinfo.isstv != "")
                        {
                            request.AddParameter(apiinfo.isstv, false);
                        }
                        if (apiinfo.keystatecode != "")
                        {
                            request.AddParameter(apiinfo.keystatecode, "");
                        }
                        if (apiinfo.keynoromaing != "")
                        {
                            request.AddParameter(apiinfo.keynoromaing, false);
                        }
                        var corrId = string.Format("{0}{1}", DateTime.Now.Ticks, Thread.CurrentThread.ManagedThreadId);
                        var requestToLog = new
                        {
                            resource = request.Resource,
                            parameters = request.Parameters.Select(parameter => new
                            {
                                name = parameter.Name,
                                value = parameter.Value,
                                type = parameter.Type.ToString()
                            }),
                            method = request.Method.ToString(),
                            uri = apiinfo.apiendpoint
                        };
                        var requuu = JsonConvert.SerializeObject(requestToLog);

                        requrl = url + ", Body: " + requuu;
                    }
                    else if (apiinfo.Requesttype == "JSON")
                    {
                        request.AddHeader("Content-Type", "application/json");
                        var body = "{\"" + apiinfo.Keyrechargeno + "\":\"" + mobile + "\",\"" + apiinfo.Keypasscode + "\":\"" + apiinfo.valpasscode + "\",\"" + apiinfo.Keyuserid + "\":\"" + apiinfo.valuserid + "\",\"" + apiinfo.KeyAmount + "\":" + amt + ",\"" + apiinfo.Keyoptcode + "\":\"" + apioptocdechk.apioptcode + "\",\"" + apiinfo.KeyReqid + "\":\"" + OrderId + "\"}";
                        request.AddParameter("application/json", body, ParameterType.RequestBody);
                        var requestToLog = new
                        {
                            resource = request.Resource,
                            parameters = request.Parameters.Select(parameter => new
                            {
                                name = parameter.Name,
                                value = parameter.Value,
                                type = parameter.Type.ToString()
                            }),
                            method = request.Method.ToString(),
                            uri = apiinfo.apiendpoint
                        };
                        var requuu = JsonConvert.SerializeObject(requestToLog);
                        requrl = url + ", Body: " + requuu;
                    }

                    IRestResponse response = client.Execute(request);
                    api_response = response.Content;

                    var url_update1 = db.Recharge_info.Single(s => s.idno == RechId);
                    url_update1.Recharge_request = requrl;
                    if (url_update1.backupchk == null)
                    {
                        url_update1.Recharge_response = api_response;
                    }
                    //else if(url_update1.backupchk == "B")
                    //{
                    //    url_update1.Recharge_response = url_update1.Recharge_response + ", Backup: " + response.Content;
                    //}
                    db.SaveChanges();


                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        resp = response.Content;
                    }
                    else
                    {
                        stsapi = "NOTOK";
                    }

                    if (stsapi == "OK")
                    {
                        var apiresp = db.rechargeapiresponses.Where(aa => aa.id == apiinfo.id).SingleOrDefault();
                        if (apiresp.Resptype == "JSON")
                        {
                            dynamic respp = JsonConvert.DeserializeObject(resp);

                            var sts1 = (string)respp[apiresp.KeyStatus];
                            var optid = respp[apiresp.KeyOptID];
                            var rembal = "0";

                            try
                            {
                                rembal = respp[apiresp.KeyRemainBal];
                                apiremain = Convert.ToDecimal(rembal).ToString();
                            }
                            catch { apiremain = "0"; }

                            var failedResp = apiresp.ValStatusFailed.Split(',');
                            var isfail = failedResp.Any(s => s.ToUpper() == sts1.ToUpper());

                            if (isfail)
                            {
                                _status = "Failed";
                                errormsg = String.IsNullOrEmpty(apiresp.KeyMsg) ? "" : respp[apiresp.KeyMsg];
                            }
                            else if (sts1.ToUpper() == apiresp.ValStatusSucess.ToUpper())
                            {
                                _status = "Success";
                                operatorId = optid;
                            }
                            else
                            {
                                _status = "Pending";
                            }
                        }
                    }
                    else
                    {
                        _status = "Pending";
                        operatorId = "";
                        errormsg = "Response code Invlid";
                    }
                }

                result.status = _status;
                result.operatorId = operatorId;
                result.errormsg = errormsg;
                result.api_response = api_response;
                result.apiremain = apiremain;
                result.id = RechId;

                return result;
            }
            catch (Exception ex)
            {
                result.status = "Pending";
                result.operatorId = "";
                result.errormsg = ex.Message;
                result.api_response = ex.Message;

                test1 t1 = new test1();
                t1.name = String.Format("Web Exception Time: {0}, Message: {1}, Stack: {2}", DateTime.Now.ToString(), ex.Message, ex.StackTrace);
                db.test1.Add(t1);
                db.SaveChanges();

                return result;
            }
        }

    }

    public class ApiResponse
    {
        public int id { get; set; } = 0;
        public string status { get; set; } = "Pending";
        public string operatorId { get; set; } = "";
        public string errormsg { get; set; } = "";
        public string api_response { get; set; } = "";
        public string apiremain { get; set; } = "0";
    }
}