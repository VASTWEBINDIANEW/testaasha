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
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Windows.Interop;
using System.Xml;
using Vastwebmulti.Areas.RETAILER.Models;
using Vastwebmulti.Models;

namespace Vastwebmulti.Controllers
{
    /// <summary>
    /// Handles Domestic Money Transfer (DMT) operations. Provides API endpoints for remitter
    /// management, Aadhaar verification, beneficiary registration, account verification,
    /// fund transfers, and transaction status checks via the configured payout API provider.
    /// </summary>
    public class DMTController : Controller
    {
        private VastwebmultiEntities db;
        bool? apists = false;

        /// <summary>
        /// Initializes a new instance of <see cref="DMTController"/>, creates the database context,
        /// and reads the current enable/disable status of the PAYOUT API service.
        /// </summary>
        public DMTController()
        {
            db = new VastwebmultiEntities();
            var serviceinfo = db.Serviceallows.Where(aa => aa.ServiceName == "PAYOUT API").SingleOrDefault();
            if (serviceinfo != null)
            {
                apists = serviceinfo.Sts;
            }
        }
        /// <summary>
        /// Retrieves the details of a remitter (sender) identified by their mobile number.
        /// Authenticates the caller, determines the active payout API provider, and delegates
        /// the lookup to either InstantPay or VastBazaar depending on configuration.
        /// </summary>
        /// <param name="api">DMT request model containing the sender mobile number, user ID, and authentication token.</param>
        /// <returns>A JSON string with status and the remitter detail response from the payout provider.</returns>
        // GET: DMT
        [HttpPost]
        public string Remitter_details(DmtDetails api)
        {
            if (api.Senderno != null && api.Tokenid != null && api.Userid != null)
            {
                if (apists == true)
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
                            if (apinm.api_name.ToUpper() == "INSTANTPAY")
                            {
                                var tokenchk = db.Money_API_URLS.Where(a => a.API_Name == apinm.api_name).SingleOrDefault();
                                if (tokenchk != null)
                                {
                                    var token = tokenchk.Token;

                                    if (HttpContext.Request.IsLocal)
                                    {
                                        token = "ca23f55293bc9c2a51fe06d2d07e5ea4";
                                    }

                                    var data = new
                                    {
                                        token = token,
                                        request = new
                                        {
                                            mobile = api.Senderno
                                        }
                                    };
                                    string URL = "https://www.instantpay.in/ws/dmi/remitter_details";
                                    string jsonData = JsonConvert.SerializeObject(data);
                                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
                                    request.Method = "POST";
                                    request.ContentType = "application/json";
                                    StreamWriter requestWriter = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.ASCII);
                                    requestWriter.Write(jsonData);
                                    requestWriter.Close();

                                    try
                                    {
                                        WebResponse webResponse = request.GetResponse();
                                        Stream webStream = webResponse.GetResponseStream();
                                        StreamReader responseReader = new StreamReader(webStream);
                                        string response = responseReader.ReadToEnd();
                                        response = response.Replace(@"<?xml version=""1.0"" encoding=""utf-8""?>", "");


                                        XmlDocument doc = new XmlDocument();
                                        doc.LoadXml(response);

                                        string json = JsonConvert.SerializeXmlNode(doc);
                                        JavaScriptSerializer ser = new JavaScriptSerializer();
                                        var parsed = ser.Deserialize<dynamic>(json);
                                        json = ser.Serialize(parsed["xml"]);

                                        dynamic data1 = JObject.Parse(json);
                                        var stscode = data1.statuscode;

                                        if (stscode == "TXN")
                                        {
                                            dynamic jobject = JsonConvert.DeserializeObject(json);
                                            bool isArray = jobject.data.beneficiary.item.Type == JTokenType.Array;
                                            if (isArray == false)
                                            {
                                                json = json.Replace("\"beneficiary\":{\"item\":{", "\"beneficiary\":{\"item\":[{");
                                                int modificationIndex = json.IndexOf("}},", json.IndexOf("beneficiary"));
                                                if (modificationIndex > 0)
                                                {
                                                    json = json.Remove(modificationIndex, 2).Insert(modificationIndex, "}]}");
                                                }
                                            }
                                        }

                                        dynamic jsonout = JsonConvert.DeserializeObject(json);
                                        dynamic outputchk = new JObject();
                                        outputchk.status = "Success";
                                        outputchk.response = jsonout;
                                        return JsonConvert.SerializeObject(outputchk);
                                    }
                                    catch
                                    {
                                        return outputchk("Failed", "Something Wrong Please Try Again Later.");
                                    }
                                }
                                else
                                {
                                    return outputchk("Failed", "Token is Missing.");
                                }
                            }
                            else if (apinm.api_name.ToUpper() == "VASTWEB")
                            {

                                var vastchk = vastbazarcheck();
                                jsonchk = JsonConvert.DeserializeObject(vastchk);
                                chksts = jsonchk.Status.ToString();
                                var token = jsonchk.response.ToString();
                                if (chksts == "True")
                                {
                                    VastBazaar cb = new VastBazaar();
                                    var responseall = cb.Remitter_details(api.Senderno, token);
                                    var responsechk = responseall.Content.ToString();
                                    var responsecode1 = responseall.StatusCode.ToString();
                                    if (responsecode1 == "OK")
                                    {
                                        dynamic json = JsonConvert.DeserializeObject(responsechk);
                                        var respcode = json.Content.ResponseCode.ToString();
                                        var ADDINFO = json.Content.ADDINFO;
                                        var stscode = ADDINFO.statuscode;

                                        if (stscode == "TXN")
                                        {
                                            json = JsonConvert.SerializeObject(ADDINFO);
                                            //dynamic jobject = JsonConvert.DeserializeObject(json.ToString());
                                            dynamic jobject = JsonConvert.DeserializeObject(json);
                                            try
                                            {
                                                bool isArray = jobject.data.beneficiary.item.Type == JTokenType.Array;
                                                if (isArray == false)
                                                {
                                                    json = json.Replace("\"beneficiary\":{\"item\":{", "\"beneficiary\":{\"item\":[{");
                                                    int modificationIndex = json.IndexOf("}},", json.IndexOf("beneficiary"));
                                                    if (modificationIndex > 0)
                                                    {
                                                        json = json.Remove(modificationIndex, 2).Insert(modificationIndex, "}]}");
                                                    }
                                                }
                                            }
                                            catch
                                            { }
                                        }
                                        else
                                        {
                                            json = JsonConvert.SerializeObject(ADDINFO);
                                            //dynamic jobject = JsonConvert.DeserializeObject(json.ToString());
                                            dynamic jobject = JsonConvert.DeserializeObject(json);
                                        }
                                        json = json.Replace(",\"address\":\"\",\"pincode\":\"\",\"city\":\"\",\"state\":\"\",\"kycstatus\":\"0\",\"consumedlimit\":\"25000\",\"remaininglimit\":\"25000\",\"kycdocs\":\"REQUIRED\",\"is_verified\":1,\"perm_txn_limit\":5000", "");
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
        /// Initiates Aadhaar verification by sending an OTP to the mobile number linked to the
        /// provided Aadhaar number. Validates the Aadhaar number format before calling the
        /// VastBazaar Aadhaar validation API and persists the verification attempt to the database.
        /// </summary>
        /// <param name="api">DMT request model containing the sender mobile number, Aadhaar number, user ID, and authentication token.</param>
        /// <returns>A JSON string with status, a message, the client ID issued by the provider, and a transaction ID.</returns>
        [HttpPost]
        public string AadharVerification_Send_OTP(DmtDetails api)
        {
            if (api.Senderno != null && api.Tokenid != null && api.Userid != null && api.AadharNumber != null)
            {
                if (apists == true)
                {
                    var output = authentication(api.Tokenid, api.Userid);
                    dynamic jsonchk = JsonConvert.DeserializeObject(output);
                    var chksts = jsonchk.Status.ToString();
                    var msg = jsonchk.response.ToString();
                    if (chksts == "True")
                    {
                        var userid = db.Users.Where(aa => aa.UserName == api.Userid).SingleOrDefault().UserId;
                        if (String.IsNullOrEmpty(api.AadharNumber) || api.AadharNumber.Length != 12)
                        {
                            return outputchk_DMT("Failed", "Invalid Aadhar Card", "", "");
                        }
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
                                    var txnid = new DateTime().Millisecond.ToString() + RandomString(8);
                                    System.Data.Entity.Core.Objects.ObjectParameter output1 = new System.Data.Entity.Core.Objects.ObjectParameter("output", typeof(string));

                                    var outp = db.sender_info_aadhar_proc_insert_API(userid, api.Senderno, api.AadharNumber, txnid, output1).SingleOrDefault().msg;
                                    if (outp == "OK")
                                    {
                                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                                        var client = new RestClient("http://api.vastbazaar.com/api/DMTPAYTM/validaadhar?aadharno=" + api.AadharNumber + "&txnid=" + txnid + "&senderno=" + api.Senderno);
                                        client.Timeout = -1;
                                        var request = new RestRequest(Method.POST);
                                        request.AddHeader("Authorization", "Bearer " + token);
                                        IRestResponse response = client.Execute(request);
                                        if (response.Content.Contains("Invalid response"))
                                        {
                                            return outputchk_DMT("Failed", "Bad Request", "", "");
                                        }
                                        else
                                        {
                                            dynamic res = JsonConvert.DeserializeObject(response.Content);
                                            db.update_sender_aadhar_API(txnid, "Success", "Pending", "", "");
                                            var status = (bool)res.Content.ADDINFO.success;
                                            if (status)
                                            {
                                                var clientId = (string)res.Content.ADDINFO.data.client_id;
                                                return outputchk_DMT("Success", "Aadhar Verification Otp Sent", clientId, txnid);
                                            }
                                            else
                                            {
                                                var message = (string)res.Content.ADDINFO.message;
                                                return outputchk_DMT("Failed", message, "", "");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        return outputchk_DMT("Failed", outp, "", "");
                                    }
                                }
                                else
                                {
                                    return outputchk_DMT("Failed", token, "", "");
                                }
                            }
                            else
                            {
                                return outputchk_DMT("Failed", "Not Allow.", "", "");
                            }
                        }
                        else
                        {
                            return outputchk_DMT("Failed", "No Account Transfer API Assign.", "", "");
                        }
                    }
                    else
                    {
                        return outputchk_DMT("Failed", msg, "", "");
                    }
                }
                else
                {
                    return outputchk_DMT("Failed", "Not Allow", "", "");
                }
            }
            else
            {
                return outputchk_DMT("Failed", "Missing Parameter", "", "");
            }
        }
        /// <summary>
        /// Completes Aadhaar verification by validating the OTP submitted by the user against the
        /// provider's response. On success, stores the verified name, photo, and address details
        /// and returns full Aadhaar profile information.
        /// </summary>
        /// <param name="api">DMT request model containing the Aadhaar number, OTP, client ID, transaction ID, sender mobile, user ID, and authentication token.</param>
        /// <returns>A JSON string with verification status and the verified Aadhaar holder's personal and address details.</returns>
        [HttpPost]
        public string AadharVerification_Verify_OTP(DmtDetails api)
        {
            if (api.Senderno != null && api.Tokenid != null && api.Userid != null && api.AadharNumber != null && api.client_id != null && api.Transid != null && api.otp != null)
            {
                if (apists == true)
                {
                    var output = authentication(api.Tokenid, api.Userid);
                    dynamic jsonchk = JsonConvert.DeserializeObject(output);
                    var chksts = jsonchk.Status.ToString();
                    var msg = jsonchk.response.ToString();
                    if (chksts == "True")
                    {
                        var userid = db.Users.Where(aa => aa.UserName == api.Userid).SingleOrDefault().UserId;
                        if (String.IsNullOrEmpty(api.AadharNumber) || api.AadharNumber.Length != 12)
                        {

                            return outputchk_DMT_ALL("Failed", "Invalid Aadhar Card", "", "", "", "", "", "", "", "", "", api.AadharNumber, "false", "");

                        }
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

                                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                                    var client = new RestClient("http://api.vastbazaar.com/api/DMTPAYTM/validaadharotp?client_id=" + api.client_id + "&otp=" + api.otp + "&txnid=" + api.Transid + "&aadhaar_number=" + api.AadharNumber + "&senderno=" + api.Senderno);

                                    client.Timeout = -1;
                                    var request = new RestRequest(Method.POST);
                                    request.AddHeader("Authorization", "Bearer " + token);
                                    IRestResponse response = client.Execute(request);
                                    if (response.Content.Contains("Invalid response"))
                                    {
                                        return outputchk_DMT_ALL("Failed", "Bad Request", "", "", "", "", "", "", "", "", "", api.AadharNumber, "false", "");
                                    }
                                    else
                                    {
                                        dynamic res = JsonConvert.DeserializeObject(response.Content);
                                        var status = (bool)res.Content.ADDINFO.success;
                                        var statuscode = (string)res.Content.ADDINFO.status_code;
                                        if (status)
                                        {
                                            var full_name = (string)res.Content.ADDINFO.data.full_name;
                                            var dob = (string)res.Content.ADDINFO.data.dob;
                                            var profile_image = (string)res.Content.ADDINFO.data.profile_image;

                                            var house = (string)res.Content.ADDINFO.data.address.house;
                                            var street = (string)res.Content.ADDINFO.data.address.street;
                                            var landmark = (string)res.Content.ADDINFO.data.address.landmark;
                                            var loc = (string)res.Content.ADDINFO.data.address.loc;
                                            var subdist = (string)res.Content.ADDINFO.data.address.subdist;
                                            var dist = (string)res.Content.ADDINFO.data.address.dist;
                                            var state = (string)res.Content.ADDINFO.data.address.state;

                                            var infochk = db.sender_aadhar_info.Where(aa => aa.txnid == api.Transid).SingleOrDefault();
                                            infochk.Statuscode = statuscode;
                                            infochk.Name = full_name;
                                            infochk.photo = profile_image;

                                            var chk = db.Sender_aadhar.Where(aa => aa.aadharnumber == api.AadharNumber).SingleOrDefault();
                                            if (chk == null)
                                            {
                                                Sender_aadhar send = new Sender_aadhar();
                                                send.aadharnumber = api.AadharNumber;
                                                send.sendernumber = api.Senderno;
                                                db.Sender_aadhar.Add(send);

                                            }
                                            db.SaveChanges();

                                            //Api_user.username = full_name;
                                            //Api_user.aadhar_image = "data:image/png;base64," + profile_image;
                                            //retailer.residential_address = String.Format("{0} {1} {2} {3} {4} {5} {6}", house, street, landmark, loc, subdist, dist, state).Trim();
                                            //retailer.AadharCard = aadhar;
                                            //retailer.aadhar_verification = true;
                                            //retailer.aadhar_dob = dob;
                                            //db.SaveChanges();
                                            return outputchk_DMT_ALL("Success", "Aadhar Verification Done", full_name, profile_image, house, street, landmark, loc, subdist, dist, state, api.AadharNumber, "true", dob);

                                        }
                                        else
                                        {
                                            var message = (string)res.Content.ADDINFO.message;
                                            return outputchk_DMT_ALL("Failed", message, "", "", "", "", "", "", "", "", "", api.AadharNumber, "false", "");


                                        }
                                    }

                                }
                                else
                                {
                                    return outputchk_DMT_ALL("Failed", token, "", "", "", "", "", "", "", "", "", api.AadharNumber, "false", "");


                                }
                            }
                            else
                            {
                                return outputchk_DMT_ALL("Failed", "Not Allow.", "", "", "", "", "", "", "", "", "", api.AadharNumber, "false", "");

                            }
                        }
                        else
                        {
                            return outputchk_DMT_ALL("Failed", "No Account Transfer API Assign.", "", "", "", "", "", "", "", "", "", api.AadharNumber, "false", "");

                        }
                    }
                    else
                    {
                        return outputchk_DMT_ALL("Failed", msg, "", "", "", "", "", "", "", "", "", api.AadharNumber, "false", "");

                    }
                }
                else
                {
                    return outputchk_DMT_ALL("Failed", "Not Allow", "", "", "", "", "", "", "", "", "", api.AadharNumber, "false", "");

                }
            }
            else
            {
                return outputchk_DMT_ALL("Failed", "Missing Parameter", "", "", "", "", "", "", "", "", "", api.AadharNumber, "false", "");

            }
        }
        /// <summary>
        /// Registers a new remitter (money sender) with the configured payout API provider using
        /// the provided mobile number and name. Authenticates the caller before delegating to
        /// either InstantPay or VastBazaar.
        /// </summary>
        /// <param name="api">DMT request model containing the sender mobile number, sender name, user ID, and authentication token.</param>
        /// <returns>A JSON string with status and the provider's registration response.</returns>
        [HttpPost]
        public string Remitter_Registration(DmtDetails api)
        {
            if (api.Senderno != null && api.Tokenid != null && api.Userid != null && api.SenderName != null)
            {
                if (apists == true)
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
                            if (apinm.api_name.ToUpper() == "INSTANTPAY")
                            {
                                var tokenchk = db.Money_API_URLS.Where(a => a.API_Name == apinm.api_name).SingleOrDefault();
                                if (tokenchk != null)
                                {
                                    var token = tokenchk.Token;

                                    if (HttpContext.Request.IsLocal)
                                    {
                                        token = "ca23f55293bc9c2a51fe06d2d07e5ea4";
                                    }

                                    var data = new
                                    {
                                        token = token,
                                        request = new
                                        {
                                            mobile = api.Senderno,
                                            name = api.SenderName,
                                            pincode = "304801"
                                        }
                                    };
                                    string URL = "https://www.instantpay.in/ws/dmi/remitter_details";
                                    string jsonData = JsonConvert.SerializeObject(data);
                                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
                                    request.Method = "POST";
                                    request.ContentType = "application/json";
                                    StreamWriter requestWriter = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.ASCII);
                                    requestWriter.Write(jsonData);
                                    requestWriter.Close();

                                    try
                                    {
                                        WebResponse webResponse = request.GetResponse();
                                        Stream webStream = webResponse.GetResponseStream();
                                        StreamReader responseReader = new StreamReader(webStream);
                                        string response = responseReader.ReadToEnd();
                                        response = response.Replace(@"<?xml version=""1.0"" encoding=""utf-8""?>", "");


                                        XmlDocument doc = new XmlDocument();
                                        doc.LoadXml(response);

                                        string json = JsonConvert.SerializeXmlNode(doc);
                                        JavaScriptSerializer ser = new JavaScriptSerializer();
                                        var parsed = ser.Deserialize<dynamic>(json);
                                        json = ser.Serialize(parsed["xml"]);

                                        dynamic jsonout = JsonConvert.DeserializeObject(json);
                                        dynamic outputchk = new JObject();
                                        outputchk.status = "Success";
                                        outputchk.response = jsonout;
                                        return JsonConvert.SerializeObject(outputchk);
                                    }
                                    catch
                                    {
                                        return outputchk("Failed", "Something Wrong Please Try Again Later.");
                                    }
                                }
                                else
                                {
                                    return outputchk("Failed", "Token is Missing.");
                                }
                            }
                            else if (apinm.api_name.ToUpper() == "VASTWEB")
                            {

                                var vastchk = vastbazarcheck();
                                jsonchk = JsonConvert.DeserializeObject(vastchk);
                                chksts = jsonchk.Status.ToString();
                                var token = jsonchk.response.ToString();
                                if (chksts == "True")
                                {
                                    VastBazaar cb = new VastBazaar();
                                    var responseall = cb.Remitter_Register(api.Senderno, api.SenderName, "332311", token);
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
        /// Verifies the OTP submitted during remitter registration or beneficiary validation flow.
        /// Delegates the OTP check to the active payout API provider after authenticating the caller.
        /// </summary>
        /// <param name="api">DMT request model containing the sender mobile number, OTP, beneficiary ID, user ID, and authentication token.</param>
        /// <returns>A JSON string with status and the provider's OTP verification response.</returns>
        [HttpPost]
        public string Remitter_Otp_Verify(DmtDetails api)
        {
            if (api.Senderno != null && api.Tokenid != null && api.Userid != null && api.otp != null && api.benid != null)
            {
                if (apists == true)
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
                            if (apinm.api_name.ToUpper() == "INSTANTPAY")
                            {
                                var tokenchk = db.Money_API_URLS.Where(a => a.API_Name == apinm.api_name).SingleOrDefault();
                                if (tokenchk != null)
                                {
                                    var token = tokenchk.Token;

                                    if (HttpContext.Request.IsLocal)
                                    {
                                        token = "ca23f55293bc9c2a51fe06d2d07e5ea4";
                                    }

                                    var data = new
                                    {
                                        token = token,
                                        request = new
                                        {
                                            mobile = api.Senderno,
                                            name = api.SenderName,
                                            pincode = "304801"
                                        }
                                    };
                                    string URL = "https://www.instantpay.in/ws/dmi/remitter_details";
                                    string jsonData = JsonConvert.SerializeObject(data);
                                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
                                    request.Method = "POST";
                                    request.ContentType = "application/json";
                                    StreamWriter requestWriter = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.ASCII);
                                    requestWriter.Write(jsonData);
                                    requestWriter.Close();

                                    try
                                    {
                                        WebResponse webResponse = request.GetResponse();
                                        Stream webStream = webResponse.GetResponseStream();
                                        StreamReader responseReader = new StreamReader(webStream);
                                        string response = responseReader.ReadToEnd();
                                        response = response.Replace(@"<?xml version=""1.0"" encoding=""utf-8""?>", "");


                                        XmlDocument doc = new XmlDocument();
                                        doc.LoadXml(response);

                                        string json = JsonConvert.SerializeXmlNode(doc);
                                        JavaScriptSerializer ser = new JavaScriptSerializer();
                                        var parsed = ser.Deserialize<dynamic>(json);
                                        json = ser.Serialize(parsed["xml"]);

                                        dynamic jsonout = JsonConvert.DeserializeObject(json);
                                        dynamic outputchk = new JObject();
                                        outputchk.status = "Success";
                                        outputchk.response = jsonout;
                                        return JsonConvert.SerializeObject(outputchk);
                                    }
                                    catch
                                    {
                                        return outputchk("Failed", "Something Wrong Please Try Again Later.");
                                    }
                                }
                                else
                                {
                                    return outputchk("Failed", "Token is Missing.");
                                }
                            }
                            else if (apinm.api_name.ToUpper() == "VASTWEB")
                            {

                                var vastchk = vastbazarcheck();
                                jsonchk = JsonConvert.DeserializeObject(vastchk);
                                chksts = jsonchk.Status.ToString();
                                var token = jsonchk.response.ToString();
                                if (chksts == "True")
                                {
                                    VastBazaar cb = new VastBazaar();
                                    var responseall = cb.Beneficiary_register_Validate(api.benid, api.benid, api.otp, token, api.Senderno);
                                    var responsechk = responseall.Content.ToString();
                                    var responsecode1 = responseall.StatusCode.ToString();
                                    if (responsecode1 == "OK")
                                    {
                                        dynamic json = JsonConvert.DeserializeObject(responsechk);
                                        var respcode = json.Content.ResponseCode.ToString();
                                        var ADDINFO = json.Content.ADDINFO;
                                        var stscode = ADDINFO.statuscode;
                                        var results = JsonConvert.SerializeObject(ADDINFO);
                                        dynamic jsonout = JsonConvert.DeserializeObject(results);
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
        /// Registers a new bank account beneficiary for a remitter with the payout API provider.
        /// Requires beneficiary name, IFSC code, account number, and the original IFSC code.
        /// Authentication is validated before the provider call is made.
        /// </summary>
        /// <param name="api">DMT request model containing the remitter ID, beneficiary name, account number, IFSC code, original IFSC code, sender mobile, user ID, and authentication token.</param>
        /// <returns>A JSON string with status and the provider's beneficiary registration response.</returns>
        [HttpPost]
        public string Beneficiary_Registration(DmtDetails api)
        {
            if (api.Senderno != null && api.Tokenid != null && api.Userid != null && api.benname != null && api.ifsccode != null && api.accountno != null && api.originalifsccode != null)
            {
                if (apists == true)
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
                            if (apinm.api_name.ToUpper() == "INSTANTPAY")
                            {
                                var tokenchk = db.Money_API_URLS.Where(a => a.API_Name == apinm.api_name).SingleOrDefault();
                                if (tokenchk != null)
                                {
                                    var token = tokenchk.Token;

                                    if (HttpContext.Request.IsLocal)
                                    {
                                        token = "ca23f55293bc9c2a51fe06d2d07e5ea4";
                                    }

                                    var data = new
                                    {
                                        token = token,
                                        request = new
                                        {
                                            remitterid = api.Remiterid,
                                            name = api.benname,
                                            mobile = api.Senderno,
                                            ifsc = api.ifsccode,
                                            account = api.accountno
                                        }
                                    };
                                    string URL = "https://www.instantpay.in/ws/dmi/beneficiary_register";
                                    string jsonData = JsonConvert.SerializeObject(data);
                                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
                                    request.Method = "POST";
                                    request.ContentType = "application/json";
                                    StreamWriter requestWriter = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.ASCII);
                                    requestWriter.Write(jsonData);
                                    requestWriter.Close();

                                    try
                                    {
                                        WebResponse webResponse = request.GetResponse();
                                        Stream webStream = webResponse.GetResponseStream();
                                        StreamReader responseReader = new StreamReader(webStream);
                                        string response = responseReader.ReadToEnd();
                                        response = response.Replace(@"<?xml version=""1.0"" encoding=""utf-8""?>", "");


                                        XmlDocument doc = new XmlDocument();
                                        doc.LoadXml(response);

                                        string json = JsonConvert.SerializeXmlNode(doc);
                                        JavaScriptSerializer ser = new JavaScriptSerializer();
                                        var parsed = ser.Deserialize<dynamic>(json);
                                        json = ser.Serialize(parsed["xml"]);

                                        dynamic jsonout = JsonConvert.DeserializeObject(json);
                                        dynamic outputchk = new JObject();
                                        outputchk.status = "Success";
                                        outputchk.response = jsonout;
                                        return JsonConvert.SerializeObject(outputchk);
                                    }
                                    catch
                                    {
                                        return outputchk("Failed", "Something Wrong Please Try Again Later.");
                                    }
                                }
                                else
                                {
                                    return outputchk("Failed", "Token is Missing.");
                                }
                            }
                            else if (apinm.api_name.ToUpper() == "VASTWEB")
                            {

                                var vastchk = vastbazarcheck();
                                jsonchk = JsonConvert.DeserializeObject(vastchk);
                                chksts = jsonchk.Status.ToString();
                                var token = jsonchk.response.ToString();
                                if (chksts == "True")
                                {
                                    VastBazaar cb = new VastBazaar();
                                    var responseall = cb.Beneficiary_register(api.Remiterid, api.benname, api.Senderno, api.ifsccode, api.accountno, token, api.originalifsccode);
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
        /// Resends the OTP for a pending beneficiary registration by delegating to the
        /// configured payout API provider. Authenticates the caller before the provider call.
        /// </summary>
        /// <param name="api">DMT request model containing the remitter ID, beneficiary ID, user ID, and authentication token.</param>
        /// <returns>A JSON string with status and the provider's resend OTP response.</returns>
        [HttpPost]
        public string Beneficiary_Resend_otp(DmtDetails api)
        {
            if (api.Tokenid != null && api.Userid != null && api.benid != null)
            {
                if (apists == true)
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
                            if (apinm.api_name.ToUpper() == "INSTANTPAY")
                            {
                                var tokenchk = db.Money_API_URLS.Where(a => a.API_Name == apinm.api_name).SingleOrDefault();
                                if (tokenchk != null)
                                {
                                    var token = tokenchk.Token;

                                    if (HttpContext.Request.IsLocal)
                                    {
                                        token = "ca23f55293bc9c2a51fe06d2d07e5ea4";
                                    }

                                    var data = new
                                    {
                                        token = token,
                                        request = new
                                        {
                                            remitterid = api.Remiterid,
                                            beneficiaryid = api.benid
                                        }
                                    };
                                    string URL = "https://www.instantpay.in/ws/dmi/beneficiary_resend_otp";
                                    string jsonData = JsonConvert.SerializeObject(data);
                                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
                                    request.Method = "POST";
                                    request.ContentType = "application/json";
                                    StreamWriter requestWriter = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.ASCII);
                                    requestWriter.Write(jsonData);
                                    requestWriter.Close();

                                    try
                                    {
                                        WebResponse webResponse = request.GetResponse();
                                        Stream webStream = webResponse.GetResponseStream();
                                        StreamReader responseReader = new StreamReader(webStream);
                                        string response = responseReader.ReadToEnd();
                                        response = response.Replace(@"<?xml version=""1.0"" encoding=""utf-8""?>", "");


                                        XmlDocument doc = new XmlDocument();
                                        doc.LoadXml(response);

                                        string json = JsonConvert.SerializeXmlNode(doc);
                                        JavaScriptSerializer ser = new JavaScriptSerializer();
                                        var parsed = ser.Deserialize<dynamic>(json);
                                        json = ser.Serialize(parsed["xml"]);

                                        dynamic jsonout = JsonConvert.DeserializeObject(json);
                                        dynamic outputchk = new JObject();
                                        outputchk.status = "Success";
                                        outputchk.response = jsonout;
                                        return JsonConvert.SerializeObject(outputchk);
                                    }
                                    catch
                                    {
                                        return outputchk("Failed", "Something Wrong Please Try Again Later.");
                                    }
                                }
                                else
                                {
                                    return outputchk("Failed", "Token is Missing.");
                                }
                            }
                            else if (apinm.api_name.ToUpper() == "VASTWEB")
                            {

                                var vastchk = vastbazarcheck();
                                jsonchk = JsonConvert.DeserializeObject(vastchk);
                                chksts = jsonchk.Status.ToString();
                                var token = jsonchk.response.ToString();
                                if (chksts == "True")
                                {
                                    VastBazaar cb = new VastBazaar();
                                    var responseall = cb.Beneficiary_register_resend_otp(api.Remiterid, api.benid, token);
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
        /// Validates a beneficiary registration by verifying the OTP provided for the given
        /// beneficiary and remitter combination. Delegates to the active payout API after
        /// successful caller authentication.
        /// </summary>
        /// <param name="api">DMT request model containing the beneficiary ID, remitter ID, OTP, sender mobile, user ID, and authentication token.</param>
        /// <returns>A JSON string with status and the provider's beneficiary validation response.</returns>
        [HttpPost]
        public string Beneficiary_Registration_validate(DmtDetails api)
        {
            if (api.Tokenid != null && api.Userid != null && api.benid != null && api.otp != null && api.Senderno != null)
            {
                if (apists == true)
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
                            if (apinm.api_name.ToUpper() == "INSTANTPAY")
                            {
                                var tokenchk = db.Money_API_URLS.Where(a => a.API_Name == apinm.api_name).SingleOrDefault();
                                if (tokenchk != null)
                                {
                                    var token = tokenchk.Token;

                                    if (HttpContext.Request.IsLocal)
                                    {
                                        token = "ca23f55293bc9c2a51fe06d2d07e5ea4";
                                    }

                                    var data = new
                                    {
                                        token = token,
                                        request = new
                                        {
                                            beneficiaryid = api.benid,
                                            remitterid = api.Remiterid,
                                            otp = api.otp
                                        }
                                    };
                                    string URL = "https://www.instantpay.in/ws/dmi/beneficiary_register_validate";
                                    string jsonData = JsonConvert.SerializeObject(data);
                                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
                                    request.Method = "POST";
                                    request.ContentType = "application/json";
                                    StreamWriter requestWriter = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.ASCII);
                                    requestWriter.Write(jsonData);
                                    requestWriter.Close();

                                    try
                                    {
                                        WebResponse webResponse = request.GetResponse();
                                        Stream webStream = webResponse.GetResponseStream();
                                        StreamReader responseReader = new StreamReader(webStream);
                                        string response = responseReader.ReadToEnd();
                                        response = response.Replace(@"<?xml version=""1.0"" encoding=""utf-8""?>", "");


                                        XmlDocument doc = new XmlDocument();
                                        doc.LoadXml(response);

                                        string json = JsonConvert.SerializeXmlNode(doc);
                                        JavaScriptSerializer ser = new JavaScriptSerializer();
                                        var parsed = ser.Deserialize<dynamic>(json);
                                        json = ser.Serialize(parsed["xml"]);

                                        dynamic jsonout = JsonConvert.DeserializeObject(json);
                                        dynamic outputchk = new JObject();
                                        outputchk.status = "Success";
                                        outputchk.response = jsonout;
                                        return JsonConvert.SerializeObject(outputchk);
                                    }
                                    catch
                                    {
                                        return outputchk("Failed", "Something Wrong Please Try Again Later.");
                                    }
                                }
                                else
                                {
                                    return outputchk("Failed", "Token is Missing.");
                                }
                            }
                            else if (apinm.api_name.ToUpper() == "VASTWEB")
                            {

                                var vastchk = vastbazarcheck();
                                jsonchk = JsonConvert.DeserializeObject(vastchk);
                                chksts = jsonchk.Status.ToString();
                                var token = jsonchk.response.ToString();
                                if (chksts == "True")
                                {
                                    VastBazaar cb = new VastBazaar();
                                    var responseall = cb.Beneficiary_register_Validate(api.benid, api.Remiterid, api.otp, token, api.Senderno);
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
        /// Verifies a beneficiary's bank account by performing a penny-drop or account validation
        /// call through the configured payout API. Deducts a verification charge and records the
        /// transaction. Supports both InstantPay and VastBazaar providers.
        /// </summary>
        /// <param name="api">DMT request model containing the sender mobile, account number, IFSC code, bank name, transaction ID, user ID, and authentication token.</param>
        /// <returns>A JSON string with the verification status including bank reference number and beneficiary name.</returns>
        [HttpPost]
        public string Beneficiary_AccountVerification(DmtDetails api)
        {
            if (api.Senderno != null && api.Tokenid != null && api.Userid != null && api.accountno != null && api.ifsccode != null && api.Transid != null && api.BankName != null)
            {
                if (apists == true)
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
                                if (apinm.api_name.ToUpper() == "INSTANTPAY")
                                {
                                    var tokenchk = db.Money_API_URLS.Where(a => a.API_Name == apinm.api_name).SingleOrDefault();
                                    if (tokenchk != null)
                                    {
                                        var token = tokenchk.Token;

                                        if (HttpContext.Request.IsLocal)
                                        {
                                            token = "ca23f55293bc9c2a51fe06d2d07e5ea4";
                                        }

                                        var data = new
                                        {
                                            token = token,
                                            request = new
                                            {
                                                remittermobile = api.Senderno,
                                                account = api.accountno,
                                                ifsc = api.ifsccode,
                                                agentid = CommonTranid
                                            }
                                        };
                                        string URL = "https://www.instantpay.in/ws/dmi/account_validate";
                                        string jsonData = JsonConvert.SerializeObject(data);
                                        System.Data.Entity.Core.Objects.ObjectParameter msgchk = new
                                                         System.Data.Entity.Core.Objects.ObjectParameter("output", typeof(string));
                                        decimal? amount = 0;
                                        var respout = db.Money_transfer_api_new(userid, amount, amount, api.Senderno, api.accountno, api.BankName, api.ifsccode, CommonTranid, api.Transid, "IMPS_VERIFY", "API", "N", jsonData.ToString(), apinm.api_name, currentip, macaddress, "", currentrequest, "", "", msgchk).SingleOrDefault().msg;
                                        if (respout == "OK")
                                        {
                                            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
                                            request.Method = "POST";
                                            request.ContentType = "application/json";
                                            StreamWriter requestWriter = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.ASCII);
                                            requestWriter.Write(jsonData);
                                            requestWriter.Close();

                                            //try
                                            //{
                                            WebResponse webResponse = request.GetResponse();
                                            Stream webStream = webResponse.GetResponseStream();
                                            StreamReader responseReader = new StreamReader(webStream);
                                            string response = responseReader.ReadToEnd();
                                            response = response.Replace(@"<?xml version=""1.0"" encoding=""utf-8""?>", "");


                                            XmlDocument doc = new XmlDocument();
                                            doc.LoadXml(response);

                                            string json = JsonConvert.SerializeXmlNode(doc);
                                            JavaScriptSerializer ser = new JavaScriptSerializer();
                                            var parsed = ser.Deserialize<dynamic>(json);
                                            json = ser.Serialize(parsed["xml"]);
                                            dynamic data1 = JObject.Parse(json);
                                            dynamic jsObj = JsonConvert.DeserializeObject(json);
                                            jsObj.data.ipay_id = CommonTranid;
                                            jsObj.data.charged_amt = -(comm);
                                            var modifiedJsonString = Newtonsoft.Json.JsonConvert.SerializeObject(jsObj);
                                            dynamic jsObjlatest = JsonConvert.DeserializeObject(modifiedJsonString);
                                            var stscode = data1.statuscode.ToString();
                                            if (stscode == "TXN")
                                            {
                                                var oprid = data1.data.bankrefno.ToString();
                                                var bname = data1.data.benename.ToString();
                                                string payidno = oprid;
                                                var jsonoutput = JsonConvert.SerializeObject(jsObjlatest);
                                                db.Money_transfer_update_new_new(CommonTranid, "SUCCESS", oprid, bname, json, jsonoutput, 0, 0);
                                                return jsonoutput;
                                            }
                                            else if (stscode == "TUP")
                                            {
                                                var jsonoutput = JsonConvert.SerializeObject(jsObjlatest);
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
                                        else if (respout == "APIBLOCK")
                                        {
                                            return outputchk("Failed", "Api Blocked.");
                                        }
                                        else if (respout == "SAMEIDNOTALLOW")
                                        {
                                            return outputchk("Failed", "Same Trans Id Not Allowed.");
                                        }
                                        else if (respout == "APILOW")
                                        {
                                            return outputchk("Failed", "Remain Balance Low.");
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
                                        return outputchk("Failed", "Token is Missing.");
                                    }
                                }
                                else if (apinm.api_name.ToUpper() == "VASTWEB")
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
                                        var requestsend = "{\"remittermobile\":\"" + api.Senderno + "\",\"account\":\"" + api.accountno + "\",\"ifsc\":\"" + api.ifsccode + "\",\"agentid\":\"" + CommonTranid + "";
                                        var respout = db.Money_transfer_api_new(userid, amount, amount, api.Senderno, api.accountno, api.BankName, api.ifsccode, api.Transid, CommonTranid, "IMPS_VERIFY", "API", "N", requestsend, apinm.api_name, currentip, macaddress, "", currentrequest, "", "", msgchk).SingleOrDefault().msg;
                                        if (respout == "OK")
                                        {
                                            VastBazaar cb = new VastBazaar();
                                            var responseall = cb.Beneficiary_Account_verify(api.Senderno, api.accountno, api.ifsccode, CommonTranid, token, api.BankName);
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
                                                    var oprid = data1.data.bankrefno.ToString();
                                                    var bname = data1.data.benename.ToString();
                                                    string payidno = oprid;
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
        /// Initiates a money transfer (IMPS/DMT) from a remitter to a registered beneficiary.
        /// Validates the caller's authentication and remaining balance, records the transaction,
        /// and delegates the actual transfer to the active payout API provider.
        /// </summary>
        /// <param name="api">DMT request model containing the sender mobile, beneficiary ID, transfer amount, transaction ID, bank details, user ID, and authentication token.</param>
        /// <returns>A JSON string with transfer status, order ID, bank reference number, amount, and remaining balance.</returns>
        [HttpPost]
        public string Fund_transfer(DmtDetails api)
        {
            if (api.Senderno != null && api.Tokenid != null && api.Userid != null && api.amount > 0 && api.Mode != null && api.Transid != null && api.accountno != null && api.BankName != null && api.ifsccode != null)
            {
                if (apists == true)
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
                            var ch1 = db.IMPS_transtion_detsils.Where(aa => aa.accountno == api.accountno && aa.rch_from == userid && aa.totalamount == finalamount && aa.Status.ToUpper() == "SUCCESS").OrderByDescending(aa => aa.idno).ToList();
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
                                            requestsend = "{\"remittermobile\":\"" + api.Senderno + "\",\"account\":\"" + api.accountno + "\",\"ifsc\":\"" + api.ifsccode + "\",\"agentid\":\"" + Tranid + "}";
                                        }

                                        System.Data.Entity.Core.Objects.ObjectParameter outputchk1 = new System.Data.Entity.Core.Objects.ObjectParameter("Output", typeof(string));
                                        var ch = db.Money_transfer_api_new(userid, api.amount, api.amount, api.Senderno, api.accountno, api.BankName, api.ifsccode, api.Transid, Tranid, api.Mode, "API", "Y", requestsend, apiname, currentip, macaddress, "", currentrequest, dmttype, "", outputchk1).Single().msg;
                                        if (ch == "RETAILERLOW")
                                        {
                                            return responsechk("Failed", api.Transid, "", api.amount.ToString(), "", "Api Remain Balance LOW", remain.ToString());
                                        }
                                        else if (ch == "PERTRANSTIONOVER")
                                        {
                                            return responsechk("Failed", api.Transid, "", api.amount.ToString(), "", "PER TRANSACTION LIMIT OVER", remain.ToString());
                                        }
                                        else if (ch == "MONTHLYLIMITOVER")
                                        {
                                            return responsechk("Failed", api.Transid, "", api.amount.ToString(), "", "MONTHLY TRANSACTION LIMIT OVER", remain.ToString());
                                        }
                                        else if (ch == "APIBLOCK")
                                        {
                                            return responsechk("Failed", api.Transid, "", api.amount.ToString(), "", "Api Block", remain.ToString());
                                        }
                                        else if (ch == "SAMEIDNOTALLOW")
                                        {
                                            return responsechk("Failed", api.Transid, "", api.amount.ToString(), "", "Same TransID Not Allow", remain.ToString());
                                        }
                                        else if (ch == "CAPPINGLOW")
                                        {
                                            return responsechk("Failed", api.Transid, "", api.amount.ToString(), "", "Capping Low", remain.ToString());
                                        }
                                        else if (ch == "STATUSDOWN")
                                        {
                                            return responsechk("Failed", api.Transid, "", api.amount.ToString(), "", "Account Transfer Status Down", remain.ToString());
                                        }
                                        else if (ch == "DUPLICATEID")
                                        {
                                            return responsechk("Failed", api.Transid, "", api.amount.ToString(), "", "Transaction id must be unique", remain.ToString());
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
                                                    return cb1.Fund_Transfer(api.Senderno, "", Tranid, amt.ToString(), api.Mode, api.accountno, api.ifsccode, token, api.BankName, "Y", apiuserinfo.adharcard,"","");
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
                                                            return responsechk("Success", api.Transid, Tranid, api.amount.ToString(), payidno, "Fund Transfer SuccessFully", remain.ToString());
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
                                                            return responsechk("Pending", api.Transid, Tranid, api.amount.ToString(), "", "Pending", remain.ToString());
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
                                                                return responsechk("Pending", api.Transid, Tranid, api.amount.ToString(), "", payidno, remain.ToString());
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
                                                                return responsechk("Failed", api.Transid, Tranid, api.amount.ToString(), "", "Failed", remain.ToString());
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
                                                        return responsechk("Pending", api.Transid, Tranid, api.amount.ToString(), "", "Pending", remain.ToString());
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
                                                    return responsechk("Pending", api.Transid, Tranid, api.amount.ToString(), "", "Pending", remain.ToString());
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
                                                return responsechk("Failed", api.Transid, "", api.amount.ToString(), "", "No Api Open", remain.ToString());
                                            }
                                        }
                                        else
                                        {
                                            return responsechk("Pending", api.Transid, "", api.amount.ToString(), "", "Pending", remain.ToString());
                                        }
                                    }
                                    else
                                    {
                                        return responsechk("Failed", api.Transid, "", api.amount.ToString(), "", "Amount Should be Greater Than Rs. 100", remain.ToString());
                                    }
                                }
                                else
                                {
                                    return responsechk("Failed", api.Transid, "", api.amount.ToString(), "", "Amount Should be Less Rs 50000", remain.ToString());
                                }
                            }
                            else
                            {
                                return responsechk("Failed", api.Transid, "", api.amount.ToString(), "", "Please Wait 5 Minutes... Same Amount Not Transfer in same Account", remain.ToString());
                            }
                        }
                        else
                        {
                            return responsechk("Failed", api.Transid, "", api.amount.ToString(), "", "Remain Balance Low", remain.ToString());
                        }
                    }
                    else
                    {
                        return responsechk("Failed", api.Transid, "", api.amount.ToString(), "", msg, apiremain.ToString());
                    }
                }
                else
                {
                    return responsechk("Failed", api.Transid, "", api.amount.ToString(), "", "Not Allow","0");
                }
            }
            else
            {
                return responsechk("Failed", api.Transid, "", api.amount.ToString(), "", "Missing Parameter", "0");
            }
        }
        /// <summary>
        /// Checks the current status of a previously initiated money transfer transaction.
        /// Authenticates the caller and queries the payout API provider using the internal
        /// transaction ID to retrieve an updated transfer status.
        /// </summary>
        /// <param name="api">DMT request model containing the transaction ID, user ID, and authentication token.</param>
        /// <returns>A JSON string with the current transfer status, bank reference number, amount, and remaining balance.</returns>
        [HttpPost]
        public string statuscheck(DmtDetails api)
        {
            if (api.Tokenid != null && api.Userid != null && api.Transid != null)
            {
                try
                {
                    if (apists == true)
                    {
                        var output = authentication(api.Tokenid, api.Userid);
                        dynamic jsonchk = JsonConvert.DeserializeObject(output);
                        var chksts = jsonchk.Status.ToString();
                        var msg = jsonchk.response.ToString();
                        if (chksts == "True")
                        {
                            var commontxn = api.Transid; var message = ""; var stscode = ""; var status = ""; decimal? amount = 0; var account = ""; var ifsc = ""; var Bankname = ""; var Banktransid = "";
                            var find = db.IMPS_transtion_detsils.Where(a => a.trans_common_id == api.Transid).SingleOrDefault();
                            if (find == null)
                            {
                                var findold = db.IMPS_transtion_detsils_old.Where(a => a.trans_common_id == api.Transid).SingleOrDefault();
                                if (findold == null)
                                {
                                    stscode = "TUP";
                                    message = "Transaction Id Not Found";
                                }
                                else
                                {
                                    stscode = "TXN";
                                    message = "Transaction Id Found";
                                    status = findold.Status;
                                    amount = findold.amount;
                                    account = findold.accountno;
                                    ifsc = findold.ifsccode;
                                    Bankname = findold.bank_nm;
                                    Banktransid = findold.bank_trans_id;
                                }
                            }
                            else
                            {
                                stscode = "TXN";
                                message = "Transaction Id Found";
                                status = find.Status;
                                amount = find.amount;
                                account = find.accountno;
                                ifsc = find.ifsccode;
                                Bankname = find.bank_nm;
                                Banktransid = find.bank_trans_id;
                            }
                            var Respoutput = new
                            {
                                statuscode = stscode,
                                Msg = message,
                                data = new
                                {
                                    Status = status,
                                    amount = amount,
                                    account = account,
                                    ifsc = ifsc,
                                    Bankname = Bankname,
                                    Banktransid = Banktransid
                                }
                            };
                            var serializer = new JavaScriptSerializer();
                            var json = serializer.Serialize(Respoutput);
                            return json;
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
                catch (Exception ex)
                {
                    return outputchk("Failed", "Something Went Wrong");
                }
            }
            else
            {
                return outputchk("Failed", "Missing Parameter");
            }

        }
        /// <summary>
        /// Initiates deletion of a registered beneficiary from the remitter's account.
        /// Authenticates the caller and sends a delete request to the payout API provider.
        /// The deletion may require subsequent OTP validation via <see cref="Beneficiary_delete_validate"/>.
        /// </summary>
        /// <param name="api">DMT request model containing the beneficiary ID, sender mobile number, user ID, and authentication token.</param>
        /// <returns>A JSON string with the deletion request status and provider response.</returns>
        [HttpPost]
        public string Beneficiary_delete(DmtDetails api)
        {
            if (api.Tokenid != null && api.Userid != null && api.benid != null && api.Senderno != null)
            {
                if (apists == true)
                {
                    var output = authentication(api.Tokenid, api.Userid);
                    dynamic jsonchk = JsonConvert.DeserializeObject(output);
                    var chksts = jsonchk.Status.ToString();
                    var msg = jsonchk.response.ToString();
                    string CommonTranid = "W" + DateTime.Parse(DateTime.Now.ToString()).ToString("yyMMddHHmmss") + RandomString(4);
                    if (chksts == "True")
                    {
                        var apinm = db.money_api_status.Where(aa => aa.status == true && aa.catagory== "PAYOUT").SingleOrDefault();
                        if (apinm != null)
                        {
                            if (apinm.api_name.ToUpper() == "INSTANTPAY")
                            {
                                var tokenchk = db.Money_API_URLS.Where(a => a.API_Name == apinm.api_name).SingleOrDefault();
                                if (tokenchk != null)
                                {
                                    var token = tokenchk.Token;

                                    if (HttpContext.Request.IsLocal)
                                    {
                                        token = "ca23f55293bc9c2a51fe06d2d07e5ea4";
                                    }

                                    var data = new
                                    {
                                        token = token,
                                        request = new
                                        {
                                            beneficiaryid = api.benid,
                                            remitterid = api.Remiterid
                                        }
                                    };
                                    string URL = "https://www.instantpay.in/ws/dmi/beneficiary_remove";
                                    string jsonData = JsonConvert.SerializeObject(data);
                                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
                                    request.Method = "POST";
                                    request.ContentType = "application/json";
                                    StreamWriter requestWriter = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.ASCII);
                                    requestWriter.Write(jsonData);
                                    requestWriter.Close();

                                    try
                                    {
                                        WebResponse webResponse = request.GetResponse();
                                        Stream webStream = webResponse.GetResponseStream();
                                        StreamReader responseReader = new StreamReader(webStream);
                                        string response = responseReader.ReadToEnd();
                                        response = response.Replace(@"<?xml version=""1.0"" encoding=""utf-8""?>", "");


                                        XmlDocument doc = new XmlDocument();
                                        doc.LoadXml(response);

                                        string json = JsonConvert.SerializeXmlNode(doc);
                                        JavaScriptSerializer ser = new JavaScriptSerializer();
                                        var parsed = ser.Deserialize<dynamic>(json);
                                        json = ser.Serialize(parsed["xml"]);

                                        dynamic jsonout = JsonConvert.DeserializeObject(json);
                                        dynamic outputchk = new JObject();
                                        outputchk.status = "Success";
                                        outputchk.response = jsonout;
                                        return JsonConvert.SerializeObject(outputchk);
                                    }
                                    catch
                                    {
                                        return outputchk("Failed", "Something Wrong Please Try Again Later.");
                                    }
                                }
                                else
                                {
                                    return outputchk("Failed", "Token is Missing.");
                                }
                            }
                            else if (apinm.api_name.ToUpper() == "VASTWEB")
                            {

                                var vastchk = vastbazarcheck();
                                jsonchk = JsonConvert.DeserializeObject(vastchk);
                                chksts = jsonchk.Status.ToString();
                                var token = jsonchk.response.ToString();
                                if (chksts == "True")
                                {
                                    VastBazaar cb = new VastBazaar();
                                    var responseall = cb.Beneficiary_Delete(api.benid, api.Remiterid, token, api.Senderno);
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
        /// Validates the OTP to confirm deletion of a registered beneficiary.
        /// Completes the two-step beneficiary removal process by verifying the OTP
        /// issued during <see cref="Beneficiary_delete"/> with the payout API provider.
        /// </summary>
        /// <param name="api">DMT request model containing the beneficiary ID, OTP, sender mobile number, user ID, and authentication token.</param>
        /// <returns>A JSON string with the deletion validation status and provider response.</returns>
        [HttpPost]
        public string Beneficiary_delete_validate(DmtDetails api)
        {
            if (api.Tokenid != null && api.Userid != null && api.benid != null && api.otp != null && api.Senderno != null)
            {
                if (apists == true)
                {
                    var output = authentication(api.Tokenid, api.Userid);
                    dynamic jsonchk = JsonConvert.DeserializeObject(output);
                    var chksts = jsonchk.Status.ToString();
                    var msg = jsonchk.response.ToString();
                    string CommonTranid = "W" + DateTime.Parse(DateTime.Now.ToString()).ToString("yyMMddHHmmss") + RandomString(4);
                    if (chksts == "True")
                    {
                        var apinm = db.money_api_status.Where(aa => aa.status == true && aa.catagory== "PAYOUT").SingleOrDefault();
                        if (apinm != null)
                        {
                            if (apinm.api_name.ToUpper() == "INSTANTPAY")
                            {
                                var tokenchk = db.Money_API_URLS.Where(a => a.API_Name == apinm.api_name).SingleOrDefault();
                                if (tokenchk != null)
                                {
                                    var token = tokenchk.Token;

                                    if (HttpContext.Request.IsLocal)
                                    {
                                        token = "ca23f55293bc9c2a51fe06d2d07e5ea4";
                                    }

                                    var data = new
                                    {
                                        token = token,
                                        request = new
                                        {
                                            beneficiaryid = api.benid,
                                            remitterid = api.Remiterid,
                                            otp = api.otp
                                        }
                                    };
                                    string URL = "https://www.instantpay.in/ws/dmi/beneficiary_remove_validate";
                                    string jsonData = JsonConvert.SerializeObject(data);
                                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
                                    request.Method = "POST";
                                    request.ContentType = "application/json";
                                    StreamWriter requestWriter = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.ASCII);
                                    requestWriter.Write(jsonData);
                                    requestWriter.Close();

                                    try
                                    {
                                        WebResponse webResponse = request.GetResponse();
                                        Stream webStream = webResponse.GetResponseStream();
                                        StreamReader responseReader = new StreamReader(webStream);
                                        string response = responseReader.ReadToEnd();
                                        response = response.Replace(@"<?xml version=""1.0"" encoding=""utf-8""?>", "");


                                        XmlDocument doc = new XmlDocument();
                                        doc.LoadXml(response);

                                        string json = JsonConvert.SerializeXmlNode(doc);
                                        JavaScriptSerializer ser = new JavaScriptSerializer();
                                        var parsed = ser.Deserialize<dynamic>(json);
                                        json = ser.Serialize(parsed["xml"]);

                                        dynamic jsonout = JsonConvert.DeserializeObject(json);
                                        dynamic outputchk = new JObject();
                                        outputchk.status = "Success";
                                        outputchk.response = jsonout;
                                        return JsonConvert.SerializeObject(outputchk);
                                    }
                                    catch
                                    {
                                        return outputchk("Failed", "Something Wrong Please Try Again Later.");
                                    }
                                }
                                else
                                {
                                    return outputchk("Failed", "Token is Missing.");
                                }
                            }
                            else if (apinm.api_name.ToUpper() == "VASTWEB")
                            {

                                var vastchk = vastbazarcheck();
                                jsonchk = JsonConvert.DeserializeObject(vastchk);
                                chksts = jsonchk.Status.ToString();
                                var token = jsonchk.response.ToString();
                                if (chksts == "True")
                                {
                                    VastBazaar cb = new VastBazaar();
                                    var responseall = cb.Beneficiary_Delete_Vaildate(api.benid, api.Remiterid, api.otp, token, api.Senderno);
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
        /// Retrieves the list of supported bank names from the payout API provider.
        /// Authenticates the caller before requesting the bank list, which is used to
        /// populate bank selection options during beneficiary registration.
        /// </summary>
        /// <param name="api">DMT request model containing the user ID and authentication token.</param>
        /// <returns>A JSON string with status and the provider's list of supported bank names.</returns>
        [HttpPost]
        public string Bank_name(DmtDetails api)
        {
            if (api.Tokenid != null && api.Userid != null)
            {
                if (apists == true)
                {
                    var output = authentication(api.Tokenid, api.Userid);
                    dynamic jsonchk = JsonConvert.DeserializeObject(output);
                    var chksts = jsonchk.Status.ToString();
                    var msg = jsonchk.response.ToString();
                    string CommonTranid = "W" + DateTime.Parse(DateTime.Now.ToString()).ToString("yyMMddHHmmss") + RandomString(4);
                    if (chksts == "True")
                    {
                        var apinm = db.money_api_status.Where(aa => aa.api_name == "VASTWEB" && aa.catagory== "PAYOUT").SingleOrDefault();
                        if (apinm != null)
                        {
                            if (apinm.api_name.ToUpper() == "INSTANTPAY")
                            {
                                var tokenchk = db.Money_API_URLS.Where(a => a.API_Name == apinm.api_name).SingleOrDefault();
                                if (tokenchk != null)
                                {
                                    var token = tokenchk.Token;

                                    if (HttpContext.Request.IsLocal)
                                    {
                                        token = "ca23f55293bc9c2a51fe06d2d07e5ea4";
                                    }

                                    var data = new
                                    {
                                        token = token,
                                        request = new
                                        {
                                            account = api.accountno

                                        }
                                    };
                                    string URL = "https://www.instantpay.in/ws/dmi/bank_details";
                                    string jsonData = JsonConvert.SerializeObject(data);
                                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
                                    request.Method = "POST";
                                    request.ContentType = "application/json";
                                    StreamWriter requestWriter = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.ASCII);
                                    requestWriter.Write(jsonData);
                                    requestWriter.Close();

                                    try
                                    {
                                        WebResponse webResponse = request.GetResponse();
                                        Stream webStream = webResponse.GetResponseStream();
                                        StreamReader responseReader = new StreamReader(webStream);
                                        string response = responseReader.ReadToEnd();
                                        response = response.Replace(@"<?xml version=""1.0"" encoding=""utf-8""?>", "");


                                        XmlDocument doc = new XmlDocument();
                                        doc.LoadXml(response);

                                        string json = JsonConvert.SerializeXmlNode(doc);
                                        JavaScriptSerializer ser = new JavaScriptSerializer();
                                        var parsed = ser.Deserialize<dynamic>(json);
                                        json = ser.Serialize(parsed["xml"]);

                                        dynamic jsonout = JsonConvert.DeserializeObject(json);
                                        dynamic outputchk = new JObject();
                                        outputchk.status = "Success";
                                        outputchk.response = jsonout;
                                        return JsonConvert.SerializeObject(outputchk);
                                    }
                                    catch
                                    {
                                        return outputchk("Failed", "Something Wrong Please Try Again Later.");
                                    }
                                }
                                else
                                {
                                    return outputchk("Failed", "Token is Missing.");
                                }
                            }
                            else if (apinm.api_name.ToUpper() == "VASTWEB")
                            {

                                var vastchk = vastbazarcheck();
                                jsonchk = JsonConvert.DeserializeObject(vastchk);
                                chksts = jsonchk.Status.ToString();
                                var token = jsonchk.response.ToString();
                                if (chksts == "True")
                                {
                                    VastBazaar cb = new VastBazaar();
                                    var responseall = cb.Bank_details(api.accountno, token);
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
        /// Retrieves a valid VastBazaar API bearer token, fetching a new one if no token is
        /// stored or refreshing it if the existing token has expired. Persists the token to
        /// the database for reuse across subsequent requests.
        /// </summary>
        /// <returns>A JSON string with a "Status" of "True" and the bearer token in "response" on success,
        /// or "Status" of "False" with an error description on failure.</returns>
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
        /// Authenticates a caller by verifying their user name, token ID, and IP address.
        /// Looks up the user in the database, validates the token, and confirms the current
        /// request IP matches the IP that was registered with the token.
        /// </summary>
        /// <param name="tokenid">The authentication token to validate.</param>
        /// <param name="userid">The user name (login ID) of the caller.</param>
        /// <returns>A JSON string with "Status" of "True" and message "Ok." on success,
        /// or "Status" of "False" and an error message on failure.</returns>
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
        /// <summary>
        /// Builds a standardized JSON response object for money transfer operations, including
        /// status, order IDs, amount, bank reference number, message, and remaining balance.
        /// </summary>
        /// <param name="status">The overall status of the operation (e.g., "Success", "Failed", "Pending").</param>
        /// <param name="orderid">The internal order ID for the transaction.</param>
        /// <param name="apiorderid">The order ID returned by the external payout API.</param>
        /// <param name="Amount">The transaction amount as a string.</param>
        /// <param name="bankrrn">The bank reference number (RRN) for the transfer.</param>
        /// <param name="msg">A descriptive message explaining the result.</param>
        /// <param name="remain">The caller's remaining wallet balance after the transaction.</param>
        /// <returns>A JSON-serialized string containing all provided transaction details.</returns>
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
        /// Builds a simple two-field JSON response containing a status and a response message.
        /// Used as a uniform error or result wrapper for DMT API endpoints.
        /// </summary>
        /// <param name="status">The result status (e.g., "Success" or "Failed").</param>
        /// <param name="output">The response message or detail to return to the caller.</param>
        /// <returns>A JSON-serialized string with "Status" and "response" fields.</returns>
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
        /// Builds a JSON response for Aadhaar OTP send operations, including status, message,
        /// the provider-issued client ID, and the internal transaction ID.
        /// </summary>
        /// <param name="status">The result status (e.g., "Success" or "Failed").</param>
        /// <param name="output">The response message or description.</param>
        /// <param name="clientId">The client ID returned by the Aadhaar verification provider.</param>
        /// <param name="txnid">The internal transaction ID for this verification attempt.</param>
        /// <returns>A JSON-serialized string containing status, response, clientId, and txnid fields.</returns>
        public string outputchk_DMT(string status, string output, string clientId, string txnid)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var dict = new Dictionary<string, string>
                      {
                        {"Status",status},
                        {"response", output},
                        {"clientId", clientId},
                        {"txnid", txnid}
                      };
            Response.Clear();
            Response.ContentType = "application/json";
            return serializer.Serialize(dict);
        }
        /// <summary>
        /// Builds a comprehensive JSON response for a completed Aadhaar verification, including
        /// the verified holder's full name, profile photo, complete address, Aadhaar number,
        /// verification flag, and date of birth.
        /// </summary>
        /// <param name="status">The result status (e.g., "Success" or "Failed").</param>
        /// <param name="output">A response message or error description.</param>
        /// <param name="full_name">The full name of the Aadhaar card holder.</param>
        /// <param name="profile_image">Base64-encoded profile image from the Aadhaar record.</param>
        /// <param name="house">House or flat number from the Aadhaar address.</param>
        /// <param name="street">Street name from the Aadhaar address.</param>
        /// <param name="landmark">Landmark from the Aadhaar address.</param>
        /// <param name="loc">Locality from the Aadhaar address.</param>
        /// <param name="subdist">Sub-district from the Aadhaar address.</param>
        /// <param name="dist">District from the Aadhaar address.</param>
        /// <param name="state">State from the Aadhaar address.</param>
        /// <param name="AadharCard">The Aadhaar card number.</param>
        /// <param name="aadhar_verification">String flag ("true"/"false") indicating verification success.</param>
        /// <param name="aadhar_dob">Date of birth as recorded on the Aadhaar card.</param>
        /// <returns>A JSON-serialized string containing all Aadhaar profile and address fields.</returns>
        public string outputchk_DMT_ALL(string status, string output, string full_name, string profile_image, string house, string street, string landmark, string loc, string subdist, string dist, string state, string AadharCard, string aadhar_verification, string aadhar_dob)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var dict = new Dictionary<string, string>
                      {
                        {"Status",status},
                        {"response", output},
                        {"FullName", full_name},
                        {"profile_image", profile_image},
                        {"residential_address", String.Format("{0} {1} {2} {3} {4} {5} {6}", house, street, landmark, loc, subdist, dist, state).Trim()},
                        {"AadharCard", AadharCard},
                        {"aadhar_verification", aadhar_verification},
                        {"aadhar_dob", aadhar_dob},
            };
            Response.Clear();
            Response.ContentType = "application/json";
            return serializer.Serialize(dict);
        }
        /// <summary>
        /// Retrieves the public Internet IP address of the current HTTP request by inspecting
        /// the <c>HTTP_X_FORWARDED_FOR</c> server variable first, then falling back to <c>REMOTE_ADDR</c>.
        /// </summary>
        /// <returns>The client's IP address as a string.</returns>
        //get your current Ip Address
        private string GetComputer_InternetIP()
        {
            string ipaddress;
            ipaddress = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (ipaddress == "" || ipaddress == null)
                ipaddress = Request.ServerVariables["REMOTE_ADDR"];
            return ipaddress;
        }
        //get mac addresss 
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
        public string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
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