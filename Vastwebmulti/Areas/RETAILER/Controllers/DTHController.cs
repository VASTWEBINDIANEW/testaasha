using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using org.vwipl;
using RestSharp;
using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Vastwebmulti.Areas.RETAILER.Models;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.RETAILER.Controllers
{

    /// <summary>
    /// RETAILER Area - Manages DTH (Direct-to-Home) recharge, new connections and plan management for retailers
    /// </summary>
    [Authorize(Roles = "Retailer")]
    [Low_Bal_CustomFilter()]
    public class DTHController : Controller
    {
        VastwebmultiEntities db = new VastwebmultiEntities();
        string VastbazaarBaseUrl = "http://api.vastbazaar.com/";
        /// <summary>
        /// GET Initiates AEPS merchant registration if not already registered, fetches super merchant info, and sends an OTP for eKYC.
        /// </summary>
        public ActionResult Aepscheck()
        {
            var userid = User.Identity.GetUserId();
            var token = string.Empty;
            token = getAuthToken();
            var retailer = db.Retailer_Details.FirstOrDefault(a => a.RetailerId == userid);
            if (retailer.AepsMerchandId == "")
            {
                var reque = new
                {
                    merchantName = retailer.RetailerName,
                    stateid = retailer.State,
                    latitude = "27.616270",
                    longitude = "75.152443",
                    merchantPhoneNumber = retailer.Mobile,
                    merchantPinCode = retailer.Pincode,
                    merchantCityName = "Sikar",
                    merchantAddress = retailer.Address,
                    userPan = retailer.PanCard,
                    retilerid = retailer.Email,
                    OTP = ""
                };
                var resquestchk = JsonConvert.SerializeObject(reque);
                var client2 = new RestClient("http://api.vastbazaar.com/api/AEPS/RegisterAEPS");
                client2.Timeout = -1;
                var request2 = new RestRequest(Method.POST);
                request2.AddHeader("Authorization", "Bearer " + token);
                request2.AddHeader("Content-Type", "application/json");

                request2.AddParameter("application/json", resquestchk, ParameterType.RequestBody);
                IRestResponse response2 = client2.Execute(request2);
                dynamic resp = JsonConvert.DeserializeObject(response2.Content);
                var stscode2 = resp.Content.ADDINFO.statuscode.ToString();
                var message = resp.Content.ADDINFO.status.ToString();
                if (stscode2 == "TXN")
                {
                    var ouletid = resp.Content.ADDINFO.data.outlet_id.ToString();
                    var pin = resp.Content.ADDINFO.data.pin.ToString();
                    retailer.AepsMerchandId = ouletid;
                    retailer.AepsMPIN = pin;
                    db.SaveChanges();
                    retailer = db.Retailer_Details.FirstOrDefault(a => a.RetailerId == userid);
                }
                else
                {
                    var viewresponse = new { Status = "Failed", Message = message };
                    return Json(viewresponse, JsonRequestBehavior.AllowGet);
                }
            }
            var client1 = new RestClient("http://api.vastbazaar.com/api/AEPS/supermerchant?merchant=" + retailer.AepsMerchandId + "");
            var request1 = new RestRequest(Method.POST);
            request1.AddHeader("Authorization", "Bearer " + token);
            IRestResponse response1 = client1.Execute(request1);
            var respsuper = response1.Content;
            dynamic chkresp = JsonConvert.DeserializeObject(respsuper);
            var stscode = chkresp.Content.ADDINFO.stscode.ToString();
            if (stscode == "0")
            {
                var msg = chkresp.Content.ADDINFO.message.ToString();
                var viewresponse = new { Status = "Failed", Message = msg };
                return Json(viewresponse, JsonRequestBehavior.AllowGet);
            }
            var superid = chkresp.Content.ADDINFO.data.superid.ToString();
            var superusername = chkresp.Content.ADDINFO.data.superusername.ToString();
            AepsModel reqObject = new AepsModel();
            var req = new
            {
                superMerchantId = superid,
                merchantLoginId = retailer.AepsMerchandId,
                transactionType = "EKY",
                mobileNumber = "7414088222",
                aadharNumber = "856243750786",
                panNumber = "DCDPK6681P",
                matmSerialNumber = "",
                latitude = "27.616270",
                longitude = "75.152443"
            };
            dynamic RequestJson = JsonConvert.SerializeObject(req);
            byte[] hash = Main.generateSha256Hash(Encoding.ASCII.GetBytes(RequestJson));
            byte[] skey = Main.generateSessionKey();
            string encryptUsingSessionKey = Main.encryptUsingSessionKey(skey, Encoding.ASCII.GetBytes(RequestJson));
            string encryptUsingPublicKey = Main.encryptUsingPublicKey(skey);
            if (string.IsNullOrWhiteSpace(encryptUsingSessionKey) || string.IsNullOrWhiteSpace(encryptUsingPublicKey))
            {
                var viewresponse = new { Status = "Failed", Message = "Failed to initiate request." };
                return Json(viewresponse, JsonRequestBehavior.AllowGet);
            }
            var client = new RestClient();
            client = new RestClient(VastbazaarBaseUrl + "api/AEPS/ICICSendOtp");
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "text/plain");
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("authorization", "bearer " + token);//OAUTH token
            request.AddHeader("hash", Convert.ToBase64String(hash));
            request.AddHeader("deviceIMEI", "352801082418919"); //can pass Unique device Id
            request.AddHeader("eskey", encryptUsingPublicKey);
            request.AddHeader("trnTimestamp", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture));
            request.AddHeader("Requestdata", RequestJson);
            request.AddParameter("text/plain",
                encryptUsingSessionKey, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            return View();
        }
        /// <summary>
        /// GET Validates the AEPS OTP for the eKYC flow using the provided OTP and transaction identifiers.
        /// </summary>
        public ActionResult AepscheckVerify(string otp, string primaryKeyId, string encodeFPTxnId)
        {
            var userid = User.Identity.GetUserId();
            var token = string.Empty;
            token = getAuthToken();
            var retailer = db.Retailer_Details.FirstOrDefault(a => a.RetailerId == userid);
            if (retailer.AepsMerchandId == "")
            {
                var reque = new
                {
                    merchantName = retailer.RetailerName,
                    stateid = retailer.State,
                    latitude = "27.616270",
                    longitude = "75.152443",
                    merchantPhoneNumber = retailer.Mobile,
                    merchantPinCode = retailer.Pincode,
                    merchantCityName = "Sikar",
                    merchantAddress = retailer.Address,
                    userPan = retailer.PanCard,
                    retilerid = retailer.Email,
                    OTP = ""
                };
                var resquestchk = JsonConvert.SerializeObject(reque);
                var client2 = new RestClient("http://api.vastbazaar.com/api/AEPS/RegisterAEPS");
                client2.Timeout = -1;
                var request2 = new RestRequest(Method.POST);
                request2.AddHeader("Authorization", "Bearer " + token);
                request2.AddHeader("Content-Type", "application/json");

                request2.AddParameter("application/json", resquestchk, ParameterType.RequestBody);
                IRestResponse response2 = client2.Execute(request2);
                dynamic resp = JsonConvert.DeserializeObject(response2.Content);
                var stscode2 = resp.Content.ADDINFO.statuscode.ToString();
                var message = resp.Content.ADDINFO.status.ToString();
                if (stscode2 == "TXN")
                {
                    var ouletid = resp.Content.ADDINFO.data.outlet_id.ToString();
                    var pin = resp.Content.ADDINFO.data.pin.ToString();
                    retailer.AepsMerchandId = ouletid;
                    retailer.AepsMPIN = pin;
                    db.SaveChanges();
                    retailer = db.Retailer_Details.FirstOrDefault(a => a.RetailerId == userid);
                }
                else
                {
                    var viewresponse = new { Status = "Failed", Message = message };
                    return Json(viewresponse, JsonRequestBehavior.AllowGet);
                }
            }
            var client1 = new RestClient("http://api.vastbazaar.com/api/AEPS/supermerchant?merchant=" + retailer.AepsMerchandId + "");
            var request1 = new RestRequest(Method.POST);
            request1.AddHeader("Authorization", "Bearer " + token);
            IRestResponse response1 = client1.Execute(request1);
            var respsuper = response1.Content;
            dynamic chkresp = JsonConvert.DeserializeObject(respsuper);
            var stscode = chkresp.Content.ADDINFO.stscode.ToString();
            if (stscode == "0")
            {
                var msg = chkresp.Content.ADDINFO.message.ToString();
                var viewresponse = new { Status = "Failed", Message = msg };
                return Json(viewresponse, JsonRequestBehavior.AllowGet);
            }
            var superid = chkresp.Content.ADDINFO.data.superid.ToString();
            var superusername = chkresp.Content.ADDINFO.data.superusername.ToString();
            AepsModel reqObject = new AepsModel();
            var req = new
            {
                superMerchantId = superid,
                merchantLoginId = retailer.AepsMerchandId,
                otp = otp,
                primaryKeyId = primaryKeyId,
                encodeFPTxnId = encodeFPTxnId
            };
            dynamic RequestJson = JsonConvert.SerializeObject(req);
            byte[] hash = Main.generateSha256Hash(Encoding.ASCII.GetBytes(RequestJson));
            byte[] skey = Main.generateSessionKey();
            string encryptUsingSessionKey = Main.encryptUsingSessionKey(skey, Encoding.ASCII.GetBytes(RequestJson));
            string encryptUsingPublicKey = Main.encryptUsingPublicKey(skey);
            if (string.IsNullOrWhiteSpace(encryptUsingSessionKey) || string.IsNullOrWhiteSpace(encryptUsingPublicKey))
            {
                var viewresponse = new { Status = "Failed", Message = "Failed to initiate request." };
                return Json(viewresponse, JsonRequestBehavior.AllowGet);
            }
            var client = new RestClient();
            client = new RestClient(VastbazaarBaseUrl + "api/AEPS/ICICValidateOTP");
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "text/plain");
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("authorization", "bearer " + token);//OAUTH token
            request.AddHeader("hash", Convert.ToBase64String(hash));
            request.AddHeader("deviceIMEI", "352801082418919"); //can pass Unique device Id
            request.AddHeader("eskey", encryptUsingPublicKey);
            request.AddHeader("trnTimestamp", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture));
            request.AddHeader("Requestdata", RequestJson);
            request.AddParameter("text/plain",
                encryptUsingSessionKey, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            return View();
        }
        /// <summary>
        /// GET Performs AEPS fingerprint-based eKYC verification using a captured biometric response.
        /// </summary>
        public ActionResult FingerVerify()
        {
            string primaryKeyId = "128"; string encodeFPTxnId = "EKYKF1017396030421154029853I";
            string cap = "{\"PidDatatype\":\"X\",\"Piddata\":\"MjAyMS0wNC0wM1QxNjo0MjoyONcE2Cu84w3seQSpdMDuVkWHwBC12w52d+hQsD1hEADj9wGub3nR40lGrkgfMGlq1Fk36eRhcKlc2HDUmG9US2ogRjNIwPXT8P8EI2Ce83/C9dOp19snApijXw2YRhek/4xrsGNxolY9PURUM06hKVmsrWyFOnc3MoPjRBnO3NGtlXqas5hj6FX7LlgloBS7tLyFB1adP9GbV1HpaHzidiSSLW6h8Fgzo8g3kNiDfDk317IOPHAYDQScXB4OxlWk0LKt5mFh0zebiuVCaOrYh0EhqCVjJ+8UZ20KHPMs57meDdT1ImT5vR6NAmPmZca8rdT22w1kmRF5GNkmuenPCgY+9zt6MOl6AXL8PJ5AMIjvT53xvhdr+gl5NGHAUHkveaalDz+UCO/BmdW6QOjztaQxL1awlnr4Cg8SzK0sPtc8Trva2l/ZY4Z5HQaeSxhivMxFlltoRwutztsPgtlXur643OY0RJDHm4O0tfff3f+sHC70zbO+kigz0EOyj6aCX3L7qaUxWOyN94AHx6q3JdHjdib7hMMbBlcIDFJ5rfufZXfJTPdSU3kYmv9gsmDPqpIFxfV8FkhLZVLe/G9ArWDcQiJj5hZiF+NSOcEIRGhcT0lOmrakGPZIr6mBnB+FQzddg9gmMvYLCvrNf4Leew06ZYiFSzZkJA5UVz9WaV3xJ292r108sWvLMiMjCE07vVQQv7BzhAqWDvTDFSr4WBW2FcjLW1rGezJBEpmjGVn6G/q1VdCjK2+pVa4ZT9+GC1QPLv8POB7e5gD4Zd9Z47ZmwuvnSKlHNGTrfqfdhr6s9G6WaFrFqElNBJY4fWGUAOZ1I18gplyDMLcIErj9foBteBbtTARQqcqRF/nPIXIqDlK6kGBU7K8XOY4nozsge9/V4Qiri4VC2l4nN098leD/X6irldfynYPjaXe8M6AkCNeraOo/SO7t3lFsscUal8G5UnSwTGidrNxm6R223Y5Y7FKiwbJh4WZjrcCdB8yFx1Fes+JVsNsUKBMmXl5YZIZEDwKHrO2Imifj6vPJdsDNd49OlCMZBFeui8BB6Ne+7sBjI49DF9kIwnQdC+ipaQsoo2r1InPPNj6KgCvuNv3ygZVBEx+dH54vCa+kcqh4dyEcWF/vkNZBkq53wfeeBHLeuPzTmNidBXiZm8S78P8i/yIYCCvc6CYxWUU=\",\"ci\":\"20221021\",\"dc\":\"b930df02-c7ff-4322-8dc6-0d62b7d8a14a\",\"dpID\":\"MANTRA.MSIPL\",\"errCode\":\"0\",\"errInfo\":\"Success\",\"fCount\":\"1\",\"fType\":\"0\",\"hmac\":\"GrLkPlKa57VyCJho9be8h5Rd2a3uF2W96GpBFXH6xSn+Wvm6wWZy0q/Ydf5MBt7l\",\"iCount\":0,\"mc\":\"MIIEGDCCAwCgAwIBAgIEAkPVgDANBgkqhkiG9w0BAQsFADCB6jEqMCgGA1UEAxMhRFMgTWFudHJhIFNvZnRlY2ggSW5kaWEgUHZ0IEx0ZCA3MUMwQQYDVQQzEzpCIDIwMyBTaGFwYXRoIEhleGEgb3Bwb3NpdGUgR3VqYXJhdCBIaWdoIENvdXJ0IFMgRyBIaWdod2F5MRIwEAYDVQQJEwlBaG1lZGFiYWQxEDAOBgNVBAgTB0d1amFyYXQxHTAbBgNVBAsTFFRlY2huaWNhbCBEZXBhcnRtZW50MSUwIwYDVQQKExxNYW50cmEgU29mdGVjaCBJbmRpYSBQdnQgTHRkMQswCQYDVQQGEwJJTjAeFw0yMTA0MDMwNDIwMjJaFw0yMTA0MjIxMTEzMzhaMIGwMSUwIwYDVQQDExxNYW50cmEgU29mdGVjaCBJbmRpYSBQdnQgTHRkMR4wHAYDVQQLExVCaW9tZXRyaWMgTWFudWZhY3R1cmUxDjAMBgNVBAoTBU1TSVBMMRIwEAYDVQQHEwlBSE1FREFCQUQxEDAOBgNVBAgTB0dVSkFSQVQxCzAJBgNVBAYTAklOMSQwIgYJKoZIhvcNAQkBFhVzdXBwb3J0QG1hbnRyYXRlYy5jb20wggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEKAoIBAQC+wcTBuoof4uR5s0l1h/7deMN2di5HuCNdCmB9ztGhV2Q14OSmT9pdbOuDiHVvOMlAAmLzNrBdT2CB8zxOJCCEycMzP4px8rLsIqjYXIKuHjrNzExmL0TGwWRT9uNxS92Hq0IuPDMtmCQnakVB5cTkWZ55DExjCPcA3NoKSJChFMU2RBuTiqFrKAHJfkuzyc3fWU6AbAnxl0K+WjBM+E9f2gNAFydS9abRXed/7VR+xJC6PPfytTB8N6NED29BvAnySJCzJLYND/QC/Q/XNDJI1Nv+1+zKBmsEb4QUFeb9lujCpTAMjmkjvzZ64LcM/IyTLZM3mZ/vmFp6762moJfPAgMBAAEwDQYJKoZIhvcNAQELBQADggEBAAoL0c9JQmVsWodxjdJx5rRAelU1Xg3zJVSMsm0P8l03LM2qedUafSQurg5Mgi1Qp09cifi2b+kaxaTiSr6pyXikq99s/o98pKtj30SWYTH9ApV24H9pm9Ipffg+dC/jmYJ2Gqq9fnx2qgA9gdO8HSt/kDhip4GwopF5eNoYVeyVRPY/mNsZtP9sqZPcgAyZvJz5gHPVruwmqxpzt+I1B+TADBTjdtO4jRKfMy65eeNynpbdVImkWvjS/JyniEqkBh5/5/sMubfZ/K8LI748JLRwjqJRyVG+GmRaNG7gqybqnRUiaptOol5N1dW6iOtop2cecgALybTuZV0HStr37QY=\",\"mi\":\"MFS100\",\"nmPoints\":\"38\",\"pCount\":0,\"pType\":0,\"qScore\":\"57\",\"rdsID\":\"MANTRA.WIN.001\",\"rdsVer\":\"1.0.2\",\"sessionKey\":\"eCUKp/U3r1Sp1AdvgLu3ysare2Rb2vdewcpMfkdQ+79unOmN4WfFLWOIuYOilQA/8tmUHTnGJ7VfU6I6KbygCm4bFb0QafV90atmtoI7VW1VeTpMJw9NDo0svQF8/wf8D5GoUFZTOGCycrsddRm3rH1xXDC5y45ay37+h1zf+uY/EOZYyEQID2hz+fLEuf1axujC2DkJ7jqR8l0g+NBWZ1OGaZYqyexHa0LdwF2tJQJnYWgYaZKNo6NJku6LQUedoMSsK5QXtfnFR3RRb+U8RP4zUkG2Cd4BLG669pL707A+jqlSRLUoVkhA7uciv9wywpmCqIfxDFz3fHAz6lML6g==\"}";
            var userid = User.Identity.GetUserId();
            var token = string.Empty;
            token = getAuthToken();
            var retailer = db.Retailer_Details.FirstOrDefault(a => a.RetailerId == userid);
            if (retailer.AepsMerchandId == "")
            {
                var reque = new
                {
                    merchantName = retailer.RetailerName,
                    stateid = retailer.State,
                    latitude = "27.616270",
                    longitude = "75.152443",
                    merchantPhoneNumber = retailer.Mobile,
                    merchantPinCode = retailer.Pincode,
                    merchantCityName = "Sikar",
                    merchantAddress = retailer.Address,
                    userPan = retailer.PanCard,
                    retilerid = retailer.Email,
                    OTP = ""
                };
                var resquestchk = JsonConvert.SerializeObject(reque);
                var client2 = new RestClient("http://api.vastbazaar.com/api/AEPS/RegisterAEPS");
                client2.Timeout = -1;
                var request2 = new RestRequest(Method.POST);
                request2.AddHeader("Authorization", "Bearer " + token);
                request2.AddHeader("Content-Type", "application/json");

                request2.AddParameter("application/json", resquestchk, ParameterType.RequestBody);
                IRestResponse response2 = client2.Execute(request2);
                dynamic resp = JsonConvert.DeserializeObject(response2.Content);
                var stscode2 = resp.Content.ADDINFO.statuscode.ToString();
                var message = resp.Content.ADDINFO.status.ToString();
                if (stscode2 == "TXN")
                {
                    var ouletid = resp.Content.ADDINFO.data.outlet_id.ToString();
                    var pin = resp.Content.ADDINFO.data.pin.ToString();
                    retailer.AepsMerchandId = ouletid;
                    retailer.AepsMPIN = pin;
                    db.SaveChanges();
                    retailer = db.Retailer_Details.FirstOrDefault(a => a.RetailerId == userid);
                }
                else
                {
                    var viewresponse = new { Status = "Failed", Message = message };
                    return Json(viewresponse, JsonRequestBehavior.AllowGet);
                }
            }
            var client1 = new RestClient("http://api.vastbazaar.com/api/AEPS/supermerchant?merchant=" + retailer.AepsMerchandId + "");
            var request1 = new RestRequest(Method.POST);
            request1.AddHeader("Authorization", "Bearer " + token);
            IRestResponse response1 = client1.Execute(request1);
            var respsuper = response1.Content;
            dynamic chkresp = JsonConvert.DeserializeObject(respsuper);
            var stscode = chkresp.Content.ADDINFO.stscode.ToString();
            if (stscode == "0")
            {
                var msg = chkresp.Content.ADDINFO.message.ToString();
                var viewresponse = new { Status = "Failed", Message = msg };
                return Json(viewresponse, JsonRequestBehavior.AllowGet);
            }
            var superid = chkresp.Content.ADDINFO.data.superid.ToString();
            var superusername = chkresp.Content.ADDINFO.data.superusername.ToString();
            AepsModel reqObject = new AepsModel();
            reqObject.superMerchantId = superid;
            reqObject.merchantLoginId = retailer.AepsMerchandId;
            reqObject.primaryKeyId = primaryKeyId;
            reqObject.encodeFPTxnId = encodeFPTxnId;
            reqObject.requestRemarks = "Test";
            reqObject.cardnumberORUID = new CardnumberOruid { adhaarNumber = "856243750786", nationalBankIdentificationNumber = null };
            reqObject.captureResponse = JsonConvert.DeserializeObject<CaptureResponse>(cap);


            dynamic RequestJson = JsonConvert.SerializeObject(reqObject);
            byte[] hash = Main.generateSha256Hash(Encoding.ASCII.GetBytes(RequestJson));
            byte[] skey = Main.generateSessionKey();
            string encryptUsingSessionKey = Main.encryptUsingSessionKey(skey, Encoding.ASCII.GetBytes(RequestJson));
            string encryptUsingPublicKey = Main.encryptUsingPublicKey(skey);
            if (string.IsNullOrWhiteSpace(encryptUsingSessionKey) || string.IsNullOrWhiteSpace(encryptUsingPublicKey))
            {
                var viewresponse = new { Status = "Failed", Message = "Failed to initiate request." };
                return Json(viewresponse, JsonRequestBehavior.AllowGet);
            }
            var client = new RestClient();
            client = new RestClient(VastbazaarBaseUrl + "api/AEPS/ICICIFingerverify");
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "text/plain");
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("authorization", "bearer " + token);//OAUTH token
            request.AddHeader("hash", Convert.ToBase64String(hash));
            request.AddHeader("deviceIMEI", "352801082418919"); //can pass Unique device Id
            request.AddHeader("eskey", encryptUsingPublicKey);
            request.AddHeader("trnTimestamp", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture));
            request.AddHeader("Requestdata", RequestJson);
            request.AddParameter("text/plain",
                encryptUsingSessionKey, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            return View();
        }
        /// <summary>
        /// Retrieves a valid authentication token from the database, requesting a new one from the API if the current token is expired or missing.
        /// </summary>
        /// <returns>A valid bearer token string, or null if the token could not be retrieved.</returns>
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
        /// <summary>
        /// Forces a refresh of the stored authentication token by calling the token API and updating the database record.
        /// </summary>
        public void UpdateAuthToken()
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
        /// <summary>
        /// Sends a password grant request to the Vastbazaar token endpoint and returns the raw REST response containing the access token.
        /// </summary>
        /// <returns>The <see cref="IRestResponse"/> from the token endpoint.</returns>
        public IRestResponse tokencheck()
        {
            var apidetails = db.Money_API_URLS.FirstOrDefault(aa => aa.API_Name == "VASTWEB");
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
        /// <summary>
        /// GET Displays the DTH recharge booking page.
        /// </summary>
        // GET: RETAILER/DTH
        public ActionResult DTHBooking()
        {
            return View();
        }
        /// <summary>
        /// POST Fetches DTH operator package details for the given operator code via InstantPay utility.
        /// </summary>
        [HttpPost]
        public ActionResult GetPackageDetails(string Code)
        {
            string respo = new InstantPayComnUtil().getOperatorDetails(Code);
            if (respo != null && respo != "ERROR")
            {
                //DTH_Con model = new JavaScriptSerializer().Deserialize<DTH_Con>(respo);

                return Json(respo);
            }
            return RedirectToAction("DTHBooking");
        }
        /// <summary>
        /// POST Processes a DTH recharge payment using the InstantPay utility and redirects to the booking page.
        /// </summary>
        [HttpPost]
        public ActionResult DoPayment(string STB, string ConOpt, string ddlPackage, string packageAmt, string txtName, string txtMobile, string customerAddress, string txtPIN)
        {
            try
            {

                string userid = User.Identity.GetUserId();
                var respo = new InstantPayComnUtil().doPayment(userid, STB, ConOpt, ddlPackage, packageAmt, txtName, txtMobile, customerAddress, txtPIN);
                if (respo != "ERROR")
                {
                    return RedirectToAction("DTHBooking");
                }
                else
                {
                    return RedirectToAction("DTHBooking");
                }

            }
            catch
            {
                return RedirectToAction("DTHBooking");
            }

        }
    }
}