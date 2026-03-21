using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.WHITELABEL.Models
{
    public class ApiTokenGen
    {
        public void TokenGenerateForWApi(string userid, string LoginId, string Password, string role, string whitelabelid)
        {
            try
            {
                if(role == "Whitelabelmaster" || role == "Whitelabeldealer" || role == "Whitelabelretailer")
                {
                    using (VastwebmultiEntities db = new VastwebmultiEntities())
                    {
                        var chk = db.TokenGenWApis.Where(a=> a.UserId == userid && a.Role == role).SingleOrDefault();
                        if (chk == null)
                        {
                            var response = reqestSend(LoginId, Password);
                            if (response != null)
                            {
                                dynamic json = JsonConvert.DeserializeObject(response);
                                var expire = json[".expires"].ToString();
                                DateTime exp = Convert.ToDateTime(expire);
                                TokenGenWApi tkn = new TokenGenWApi();
                                tkn.WhitelabelId = whitelabelid;
                                tkn.UserId = json.userId.ToString();
                                tkn.Token = json.access_token.ToString();
                                tkn.Role = json.role.ToString();
                                tkn.TknExpTime = exp;
                                db.TokenGenWApis.Add(tkn);
                                db.SaveChanges();
                            }
                        }
                        else
                        {
                            if (chk.UserId == userid)
                            {
                                var response = reqestSend(LoginId, Password);
                                if (response != null)
                                {
                                    dynamic json = JsonConvert.DeserializeObject(response);

                                    var expire = json[".expires"].ToString();
                                    DateTime exp = Convert.ToDateTime(expire);
                                    string Uid = json.userId.ToString();
                                    string Role = json.role.ToString();

                                    var getbyId = db.TokenGenWApis.Where(a => a.UserId == userid && a.UserId == Uid && a.Role == Role).SingleOrDefault();
                                    getbyId.UserId = json.userId.ToString();
                                    getbyId.Token = json.access_token.ToString();
                                    getbyId.Role = json.role.ToString();
                                    getbyId.TknExpTime = exp;
                                    db.SaveChanges();
                                }
                            }
                        }
                    }
                }
            }
            catch { }
        }
        public string reqestSend(string LoginId, string Password)
        {
            var checkID = LoginId;
            if (checkID != null)
            {
                try
                {
                    using (VastwebmultiEntities db = new VastwebmultiEntities())
                    {
                        //string ApiUrl = HttpContext.Current.Request.IsLocal ? "https://www.aashadigitalindia.co.in/" : HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
                        //ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                        //HttpWebRequest WebRequestObjectTarget = (HttpWebRequest)HttpWebRequest.Create(ApiUrl);
                        //WebRequestObjectTarget.Timeout = (System.Int32)TimeSpan.FromSeconds(250).TotalMilliseconds;
                        //WebResponse Response = WebRequestObjectTarget.GetResponse();

                        string WebsiteUrl = db.Admin_details.SingleOrDefault().WebsiteUrl;
                        string ExactUrl = WebsiteUrl.Replace("https://www.", "").Replace("http://www.", "").Replace("https://", "").Replace("http://", "");
                        string baseUrl = "http://wapi." + ExactUrl;

                        var client = HttpContext.Current.Request.IsLocal ? new RestClient("http://wapi.aashadigitalindia.co.in/token") : new RestClient(baseUrl + "/token");
                        var request = new RestRequest(Method.POST);
                        request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                        request.AddParameter("application/x-www-form-urlencoded", "UserName=" + LoginId + "&Password=" + Password + "&grant_type=password", ParameterType.RequestBody);

                        IRestResponse response = client.Execute(request);

                        return response.StatusCode.ToString() == "OK" ? response.Content : null;
                    }

                }
                catch { return null; }
            }
            else
            {
                return null;
            }
        }

        //public static void WriteLog(string strMessage)
        //{
        //    try
        //    {
        //        StreamWriter log;
        //        FileStream fileStream = null;
        //        DirectoryInfo logDirInfo = null;
        //        FileInfo logFileInfo;

        //        string logFilePath = "C:\\Logs\\";
        //        logFilePath = logFilePath + "AshaaWRTokenGenerateForWApi" + System.DateTime.Today.ToString("MM-dd-yyyy") + "." + "txt";

        //        logFileInfo = new FileInfo(logFilePath);
        //        logDirInfo = new DirectoryInfo(logFileInfo.DirectoryName);
        //        if (!logDirInfo.Exists) logDirInfo.Create();
        //        if (!logFileInfo.Exists)
        //        {
        //            fileStream = logFileInfo.Create();
        //        }
        //        else
        //        {
        //            fileStream = new FileStream(logFilePath, FileMode.Append);
        //        }
        //        log = new StreamWriter(fileStream);
        //        log.WriteLine(strMessage);
        //        log.Close();

        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}

    }
}