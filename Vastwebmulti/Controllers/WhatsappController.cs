using Newtonsoft.Json;
using RestSharp;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Vastwebmulti.Models;

namespace Vastwebmulti.Controllers
{
    /// <summary>
    /// Handles WhatsApp message delivery. Validates the caller's user ID, authentication token,
    /// and IP address before forwarding the message to the VastBazaar WhatsApp messaging API.
    /// </summary>
    public class WhatsappController : Controller
    {
        // GET: Whatsapp
        private VastwebmultiEntities db;

        /// <summary>
        /// Initializes a new instance of <see cref="WhatsappController"/> and creates the database context.
        /// </summary>
        public WhatsappController()
        {
            db = new VastwebmultiEntities();
        }

        /// <summary>
        /// Sends a WhatsApp message to the specified mobile number after validating the user ID,
        /// authentication token, IP address, and an active subscription plan.
        /// </summary>
        /// <param name="userid">The email ID of the API user requesting the message send.</param>
        /// <param name="tokenid">The authentication token associated with the user's registered IP address.</param>
        /// <param name="message">The text content of the WhatsApp message to be sent.</param>
        /// <param name="mobile">The destination mobile number to receive the WhatsApp message.</param>
        /// <returns>A JSON-serialized string containing the message delivery status and a descriptive message.</returns>
        [HttpGet]
        public string whatsappSendSMS(string userid, string tokenid, string message, string mobile)
        {
            var sts = "Failed"; var msg = "";
            if (string.IsNullOrEmpty(userid) == false && string.IsNullOrEmpty(tokenid) == false)
            {
                var apiinfo = db.api_user_details.Where(aa => aa.emailid.ToUpper() == userid.ToUpper()).SingleOrDefault();
                if (apiinfo != null)
                {
                    var chk = db.Whatsapp_user_details.Where(aa => aa.userid == apiinfo.apiid).SingleOrDefault();
                    if (chk != null)
                    {
                        tokenid = tokenid.Replace(" ", "+");
                        var tokenchk = db.Api_ip_address.Where(aa => aa.userid == apiinfo.apiid && aa.token == tokenid).SingleOrDefault();
                        if (tokenchk != null)
                        {
                            if (tokenchk.token == tokenid)
                            {
                                var key = apiinfo.apiid.Substring(0, 16);
                                var usedip = Decrypt(tokenchk.token, key);
                                var currentip = GetComputer_InternetIP();
                                if (usedip == currentip)
                                {
                                    var purchasecheck = db.Whatsapp_purchase.Where(aa => aa.apiid == apiinfo.apiid && aa.status.ToUpper() == "SUCCESS").ToList();
                                    if (purchasecheck.Count > 0)
                                    {
                                        var expdate = purchasecheck.OrderByDescending(aa => aa.purchasedate).Take(1).SingleOrDefault().renewaldate;
                                        if (expdate >= DateTime.Now)
                                        {
                                            var api = apiinfo.apiid.ToString();
                                            var frmnm = apiinfo.farmname.ToString();
                                            var email = apiinfo.emailid.ToString();
                                            VastBazaartoken vbtoken = new VastBazaartoken();
                                            var btoken = vbtoken.gettoken();


                                            var client = new RestClient("http://api.vastbazaar.com/api/Web/WhatsappMsg_API");
                                            client.Timeout = -1;
                                            var request = new RestRequest(Method.POST);
                                            request.AddHeader("Authorization", "Bearer " + btoken);
                                            request.AddHeader("Content-Type", "application/json");
                                            var body = new
                                            {
                                                mobile,
                                                message,
                                                apiid = api,
                                                firmname = frmnm,
                                                Email = email
                                            };
                                            var reqchk = JsonConvert.SerializeObject(body);
                                            var ccc = JsonConvert.DeserializeObject(reqchk);
                                            request.AddParameter("application/json", ccc, ParameterType.RequestBody);
                                            IRestResponse response = client.Execute(request);



                                            //Console.WriteLine(response.Content);

                                            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                                            {
                                                var resp1 = response.Content;
                                                dynamic reschkk = JsonConvert.DeserializeObject(resp1);
                                                sts = reschkk.Content.ADDINFO.Status;
                                                msg = reschkk.Content.ADDINFO.Message;

                                                WhatsAppmessagesend wmsg = new WhatsAppmessagesend();
                                                wmsg.message = message;
                                                wmsg.mobile = mobile;
                                                wmsg.rchmessage = msg;
                                                wmsg.sendtime = DateTime.Now;
                                                wmsg.sts = sts;
                                                wmsg.userid = apiinfo.apiid;
                                                db.WhatsAppmessagesends.Add(wmsg);
                                                db.SaveChanges();
                                            }
                                        }
                                        else
                                        {
                                            msg = "Plan Expire";
                                            //plan Expire
                                        }
                                    }
                                    else
                                    {
                                        msg = "Firstly Purchase Any Plan";
                                        //prchase first
                                    }
                                }
                                else
                                {
                                    msg = "Ip is Not Valid. IP: " + currentip + ", token ip: " + usedip;
                                    // Ip is Not Valid. IP: " + currentip
                                }
                            }
                            else
                            {
                                msg = "Invalid Token";
                                //invalid token
                            }
                        }
                        else
                        {
                            msg = "Token Not Found";
                            //token not found
                        }
                    }
                    else
                    {
                        msg = "Firstlly Register Any Whats App Number";
                    }
                }
                else
                {
                    msg = "User Not Found";
                    //user not found
                }
            }
            else
            {
                msg = "All Filed Are Mandatory";
            }

            //  var purchasecheck=db.Whatsapp_purchase.Where()
            var resppchk = new
            {
                Message = msg,
                Status = sts
            };

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            //var dict = new Dictionary<bool, string>
            //                                                         {
            //                                                           {"Status", status},
            //                                                           {"Message", message}
            //                                                         };
            Response.Clear();
            Response.ContentType = "application/json";
            return serializer.Serialize(resppchk);
        }
        //[HttpGet]
        //public string whatsappstatus()
        //{
        //}

        /// <summary>
        /// Decrypts a Base64-encoded string that was encrypted with the TripleDES algorithm using ECB mode.
        /// Used to recover the original IP address stored inside an authentication token.
        /// </summary>
        /// <param name="input">The Base64-encoded ciphertext to decrypt.</param>
        /// <param name="key">The 16-character UTF-8 key used for decryption.</param>
        /// <returns>The decrypted plaintext string.</returns>
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

        /// <summary>
        /// Retrieves the public Internet IP address of the current HTTP request by inspecting
        /// the <c>HTTP_X_FORWARDED_FOR</c> header first, then falling back to <c>REMOTE_ADDR</c>.
        /// </summary>
        /// <returns>The client's IP address as a string.</returns>
        public string GetComputer_InternetIP()
        {
            string ipaddress;
            ipaddress = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (ipaddress == "" || ipaddress == null)
                ipaddress = Request.ServerVariables["REMOTE_ADDR"];
            return ipaddress;
        }
    }
}
