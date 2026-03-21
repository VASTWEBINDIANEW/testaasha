using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Vastwebmulti.Models;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Vastwebmulti.Areas.RETAILER.Models;
using System.Web.Mvc;
using System.Net.Http;
using System.Text;
using System.Net.Http.Headers;
using RestSharp;
using com.sun.security.ntlm;
using System.Net;

namespace Vastwebmulti
{
    public class AppNotification
    {
        private static bool firebaseInitialized = false;
        VastwebmultiEntities db = new VastwebmultiEntities();
        public AppNotification()
        {
            InitializeFirebase();
        }

        private void InitializeFirebase()
        {
            if (!firebaseInitialized)
            {
                lock (typeof(AppNotification)) // Thread-safe initialization
                {
                    if (!firebaseInitialized)
                    {
                        if (FirebaseApp.DefaultInstance == null)
                        {
                            string credPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "notification.json");

                            if (!File.Exists(credPath))
                                throw new FileNotFoundException("Firebase credential file not found", credPath);

                            FirebaseApp.Create(new AppOptions()
                            {
                                Credential = GoogleCredential.FromFile(credPath)
                            });
                        }

                        firebaseInitialized = true;
                    }
                }
            }
        }



        //public async Task sendmessage(string userid, string text)
        //{
        //    using (VastwebmultiEntities db = new VastwebmultiEntities())
        //    {
        //        if (userid.Contains("vastweb"))
        //        {
        //            var company = db.Admin_details.FirstOrDefault();
        //            string company_name = company?.Companyname ?? "Notification";

        //           var chk= await SendNotificationUsingTopic(userid, company_name, text);
        //        }
        //        else
        //        {
        //            var user = db.Users.FirstOrDefault(jj => jj.Email == userid);
        //            if (user != null)
        //            {
        //                try
        //                {
        //                    var deviceId = db.Login_info
        //                        .Where(p => p.UserId == userid || p.UserId == user.UserId)
        //                        .OrderByDescending(p => p.Idno)
        //                        .Select(p => p.DeviceId)
        //                        .FirstOrDefault();

        //                    if (!string.IsNullOrEmpty(deviceId))
        //                    {
        //                        var company = db.Admin_details.FirstOrDefault();
        //                        string company_name = company?.Companyname ?? "Notification";

        //                        var firebaseMessaging = FirebaseMessaging.DefaultInstance;
        //                        if (firebaseMessaging == null)
        //                        {
        //                            throw new InvalidOperationException("FirebaseMessaging is not initialized properly.");
        //                        }

        //                        var message = new Message()
        //                        {
        //                            Notification = new FirebaseAdmin.Messaging.Notification
        //                            {
        //                                Title = company_name,
        //                                Body = text
        //                            },
        //                            Token = deviceId
        //                        };

        //                        string response = await firebaseMessaging.SendAsync(message);
        //                        Console.WriteLine("Message sent: " + response);
        //                    }
        //                    else
        //                    {
        //                        Console.WriteLine("No Device ID found for user.");
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    Console.WriteLine("Error sending notification: " + ex.Message);
        //                }
        //            }
        //            else
        //            {
        //                Console.WriteLine("User not found.");
        //            }
        //        }
        //    }
        //}

        public async Task<string> SendNotificationUsingTopic(string topic, string title, string message)
        {
            try
            {
                var msg = new Message()
                {
                    Notification = new FirebaseAdmin.Messaging.Notification()
                    {
                        Title = title,
                        Body = message,
                    },
                    Topic = topic,
                };

                var firebaseMessaging = FirebaseMessaging.DefaultInstance;
                if (firebaseMessaging == null)
                {
                    throw new InvalidOperationException("FirebaseMessaging is not initialized properly.");
                }

                string response = await firebaseMessaging.SendAsync(msg);
                return $"Message sent: {response}";
            }
            catch(Exception ex) 
            { 

            }
            return $"Message sent";
        }

        public  string sendmessage(string userid, string text)
        {
            try
            {
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    var respinfo = Respchk();
                    string deviceToken = ""; string projectId = "";
                    var serviceAccountPath1 = ""; string filenm = ""; var serviceAccountPath = "";
                    if (respinfo.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        dynamic json = JsonConvert.DeserializeObject(respinfo.Content);
                        deviceToken = json.Content.ADDINFO[0].deviceToken;
                        projectId = json.Content.ADDINFO[0].projectId;
                        filenm = json.Content.ADDINFO[0].Filename;
                        serviceAccountPath1 = "https://vastbazaar.com/AppnotificationFiles/" + filenm;
                    }
                    string serviceAccountUrl = serviceAccountPath1;
                    // Firebase scope
                    var scopes = new[] { "https://www.googleapis.com/auth/firebase.messaging" };
                    // Download JSON and load credentials
                    GoogleCredential credential;
                    using (var client = new WebClient())
                    {
                        string jsonContent = client.DownloadString(serviceAccountUrl);

                        using (var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(jsonContent)))
                        {
                            credential = GoogleCredential.FromStream(stream).CreateScoped(scopes);
                        }
                    }

                    // Get access token synchronously
                    var accessToken = credential.UnderlyingCredential.GetAccessTokenForRequestAsync().Result;

                    var url = $"https://fcm.googleapis.com/v1/projects/{projectId}/messages:send";
                    var company = db.Admin_details.FirstOrDefault();
                    string company_name = company?.Companyname ?? "Notification";
                    var payload = new
                    {
                        message = new
                        {
                            token = deviceToken,
                            notification = new
                            {
                                title = company_name,
                                body = text
                            },
                            data = new
                            {
                                click_action = "FLUTTER_NOTIFICATION_CLICK", // Or your app-specific action
                                customKey = "4646546"
                            }
                        }
                    };


                    var jsonMessage = JsonConvert.SerializeObject(payload);

                    using (var client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                        var content = new StringContent(jsonMessage, Encoding.UTF8, "application/json");
                        var response = client.PostAsync(url, content).Result;
                        var result = response.Content.ReadAsStringAsync().Result;

                        return $"FCM Response: {result}";
                    }
                }
            }
            catch { }
            return "Error in sending notification";
        }
        public IRestResponse Respchk()
        {
            var token = getAuthToken();
            var client = new RestClient("http://api.vastbazaar.com/api/Web/AppNotification");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", "Bearer " + token);
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
            catch (Exception Ex)
            {
                return null;
            }
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
    }
}
