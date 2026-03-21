using Microsoft.AspNet.Identity;
using RestSharp;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Vastwebmulti.Models
{
    /// <summary>
    /// Utility class for sending SMS and WhatsApp notifications to all user types
    /// </summary>
    public class ALLSMSSend
    {
        VastwebmultiEntities db = new VastwebmultiEntities();

        public void sendsmsallnew(string frmmobile, string text, string apiurls, string Templateid)
        {
            string userid = null;
            try
            {
                try
                {
                    userid = System.Web.HttpContext.Current.User.Identity.GetUserId();
                }
                catch { }

                if (userid == null)
                {
                    var isExist = db.Users.Any(a => a.PhoneNumber == frmmobile);
                    if (isExist)
                    {
                        userid = db.Users.Where(a => a.PhoneNumber == frmmobile).FirstOrDefault().UserId;
                    }
                }
                text = string.Format(text, "1230");
                var apinamechange = apiurls.Replace("tttt", frmmobile).Replace("mmmm", text).Replace("iiii", Templateid);
                // Use FirstOrDefault with null check to avoid NullReferenceException
                var whatsts = db.Email_show_passcode.FirstOrDefault();
                if (whatsts != null && whatsts.whatsappapists == false)
                {
                    sms_api_entry sms = new sms_api_entry();
                    sms.apiname = apinamechange;
                    sms.msg = "The WhatsApp status is off, so please contact the administrator.";
                    sms.m_date = System.DateTime.Now;
                    sms.response = "";
                    sms.messagefor = userid;
                    db.sms_api_entry.Add(sms);
                    db.SaveChanges();
                }
                else if (apinamechange != null)
                {
                    if (apinamechange.ToUpper().Contains("VASTBAZAAR"))
                    {
                        VastBazaartoken Responsetoken = new VastBazaartoken();
                        var client = new RestClient(apinamechange);
                        var request = new RestRequest(Method.GET);
                        var token = Responsetoken.gettoken();
                        request.AddHeader("authorization", "bearer " + token);
                        request.AddHeader("content-type", "application/json");
                        var task = Task.Run(() =>
                        {
                            return client.Execute(request).Content;
                        });
                        bool isCompletedSuccessfully = task.Wait(TimeSpan.FromSeconds(10000));
                        var resp = "";
                        if (isCompletedSuccessfully == true)
                        {
                            resp = task.Result;
                        }
                        sms_api_entry sms = new sms_api_entry();
                        sms.apiname = apinamechange;
                        sms.msg = text;
                        sms.m_date = System.DateTime.Now;
                        sms.response = resp;
                        sms.messagefor = userid;
                        db.sms_api_entry.Add(sms);
                        db.SaveChanges();
                    }
                    else
                    {
                        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                        var client = new RestClient(apinamechange);
                        client.Timeout = -1;
                        var request = new RestRequest(Method.GET);
                        IRestResponse response = client.Execute(request);
                        var resp = response.Content;
                        sms_api_entry sms = new sms_api_entry();
                        sms.apiname = apinamechange;
                        sms.msg = text;
                        sms.m_date = System.DateTime.Now;
                        sms.response = resp;
                        sms.messagefor = userid;
                        db.sms_api_entry.Add(sms);
                        db.SaveChanges();
                    }
                }
                else
                {
                    sms_api_entry sms = new sms_api_entry();
                    sms.apiname = apinamechange;
                    sms.msg = "Your WhatsApp url is not correct.";
                    sms.m_date = System.DateTime.Now;
                    sms.response = "";
                    sms.messagefor = userid;
                    db.sms_api_entry.Add(sms);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                sms_api_entry sms = new sms_api_entry();
                sms.apiname = apiurls;
                sms.msg = text;
                sms.m_date = System.DateTime.Now;
                sms.response = ex.Message;
                sms.messagefor = userid;
                db.sms_api_entry.Add(sms);
                db.SaveChanges();
            }
        }

        public void sms_init(string sms_status, string whatsapp_status, string sms_type, string remmobile, params object[] vals)
        {
            try
            {
                var smsapi = db.apisms.Where(x => x.sts == "Y").ToList();
                if (smsapi.Any())
                {

                    var smsapionsts = smsapi.Where(s => s.api_type == "whatsapp").SingleOrDefault();
                    if (whatsapp_status == "Y" && smsapionsts != null)
                    {
                        var smsstypes = db.Sending_SMS_Templates.Where(x => x.SMS_TYPE == sms_type && x.SMSAPIID == smsapionsts.id).SingleOrDefault();
                        if (smsstypes != null)
                        {
                            callSms(smsstypes, smsapionsts.smsapi, remmobile, vals);
                        }
                    }

                    smsapionsts = smsapi.Where(s => s.api_type == "sms").FirstOrDefault();
                    if (sms_status == "Y" && smsapionsts != null)
                    {
                        var smsstypes = db.Sending_SMS_Templates.Where(x => x.SMS_TYPE == sms_type && x.SMSAPIID == smsapionsts.id).SingleOrDefault();
                        if (smsstypes != null)
                        {
                            callSms(smsstypes, smsapionsts.smsapi, remmobile, vals);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                sms_api_entry sms = new sms_api_entry();
                sms.apiname = "Exception Occured";
                sms.msg = sms_type;
                sms.m_date = System.DateTime.Now;
                sms.response = ex.Message;
                sms.messagefor = remmobile;
                db.sms_api_entry.Add(sms);
                db.SaveChanges();

                test1 t1 = new test1();
                t1.name = String.Format("Sms Exception - Message: {0}, Stack: {1}", ex.Message, ex.StackTrace);
                db.test1.Add(t1);
                db.SaveChanges();
            }
        }

        private void callSms(Sending_SMS_Templates smsstypes, string smsapi, string remmobile, params object[] vals)
        {
            string msgssss = string.Format(smsstypes.Templates, vals);
            string tempid = smsstypes.Templateid;
            string urlss = smsapi;

            sendsmsallnew(remmobile, msgssss, urlss, tempid);
        }


        public void SendEmailAll(string recepientEmail, string textMsg, string subject, string ToCC, int waittime = 5000)
        {
            CommUtilEmail emailsend = new CommUtilEmail();
            emailsend.EmailLimitChk(recepientEmail, ToCC, subject, textMsg, "No CallBackUrl", waittime);
        }
        public void SendEmailAll1(string recepientEmail, string textMsg, string subject, string ToCC, int waittime = 5000)
        {
            CommUtilEmail emailsend = new CommUtilEmail();
            emailsend.EmailLimitChknew(recepientEmail, ToCC, subject, textMsg, "No CallBackUrl", waittime);
        }
        public void SendWhitelabelEmailAll(string recepientEmail, string textMsg, string subject, string ToCC, string whitelabelid)
        {
            CommUtilEmail emailsend = new CommUtilEmail();
            emailsend.WhiteLabelEmailLimitChk(recepientEmail, ToCC, subject, textMsg, "No CallBackUrl", whitelabelid);
        }

        public void sendsmsallwhitelabel(string whitelabelid, string frmmobile, string text, string smsType)
        {
            System.Data.Entity.Core.Objects.ObjectParameter output = new
            System.Data.Entity.Core.Objects.ObjectParameter("Output", typeof(string));
            try
            {
                var msg = db.whitelabel_priority_sms_send_new(whitelabelid, frmmobile, text, smsType, output).Single().msg;
                if (msg != "free" && msg != "sim")
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(msg);
                    request.Timeout = 15000;
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    StreamReader sr = new StreamReader(response.GetResponseStream());
                    var s = sr.ReadToEnd();
                    whitelabel_sms_api_entry sms = new whitelabel_sms_api_entry();
                    sms.apiname = msg;
                    sms.msg = text;

                    sms.m_date = System.DateTime.Now;
                    sms.response = s;
                    sms.userid = whitelabelid;
                    db.whitelabel_sms_api_entry.Add(sms);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                db.sms_api_entry.Add(new sms_api_entry
                {
                    apiname = "sendsmsallwhitelabel",
                    msg = text,
                    m_date = DateTime.Now,
                    response = ex.Message,
                    messagefor = whitelabelid
                });
                db.SaveChanges();
            }
        }

        public void sendsmsallnew_whitelabel(string whitelabelid, string frmmobile, string text, string apiurls, string Templateid)
        {
            string userid = null;
            try
            {
                try
                {
                    userid = System.Web.HttpContext.Current.User.Identity.GetUserId();
                }
                catch { }

                if (userid == null)
                {
                    var isExist = db.Users.Any(a => a.PhoneNumber == frmmobile);
                    if (isExist)
                    {
                        userid = db.Users.Where(a => a.PhoneNumber == frmmobile).FirstOrDefault().UserId;
                    }
                }
                text = string.Format(text, "1230");
                var apinamechange = apiurls.Replace("tttt", frmmobile).Replace("mmmm", text).Replace("iiii", Templateid);

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var client = new RestClient(apinamechange);
                var request = new RestRequest(Method.GET);

                var task = Task.Run(() =>
                {
                    return client.Execute(request).Content;
                });

                bool isCompletedSuccessfully = task.Wait(TimeSpan.FromSeconds(10000));
                var resp = "";
                if (isCompletedSuccessfully == true)
                {
                    resp = task.Result;
                }

                whitelabel_sms_api_entry sms = new whitelabel_sms_api_entry();
                sms.apiname = apinamechange;
                sms.msg = text;
                sms.m_date = System.DateTime.Now;
                sms.response = resp;
                sms.messagefor = userid;
                sms.userid = whitelabelid;
                db.whitelabel_sms_api_entry.Add(sms);
                db.SaveChanges();


            }
            catch (Exception ex)
            {
                whitelabel_sms_api_entry sms = new whitelabel_sms_api_entry();
                sms.apiname = apiurls;
                sms.msg = text;
                sms.m_date = System.DateTime.Now;
                sms.response = ex.Message;
                sms.userid = whitelabelid;
                sms.messagefor = userid;
                db.whitelabel_sms_api_entry.Add(sms);
                db.SaveChanges();
            }
        }

        public void sms_init_whitelabel(string whitelabelid, string sms_status, string whatsapp_status, string sms_type, string remmobile, params object[] vals)
        {
            try
            {
                var smsapionsts = db.whitelabel_apisms.Where(x => x.sts == "Y" && x.userfor == whitelabelid).FirstOrDefault();
                if (sms_status == "Y" && smsapionsts != null)
                {
                    var smsstypes = db.Whitelabel_Sending_SMS_Templates.Where(x => x.SMS_TYPE == sms_type && x.SMSAPIID == smsapionsts.id && x.whitelabelId == whitelabelid).SingleOrDefault();
                    if (smsstypes != null)
                    {
                        callSms_whitelabel(whitelabelid, smsstypes, smsapionsts.smsapi, remmobile, vals);
                    }
                }
            }
            catch (Exception ex)
            {
                whitelabel_sms_api_entry sms = new whitelabel_sms_api_entry();
                sms.apiname = "Exception Occured";
                sms.msg = sms_type;
                sms.m_date = System.DateTime.Now;
                sms.response = ex.Message;
                sms.messagefor = remmobile;
                sms.userid = whitelabelid;
                db.whitelabel_sms_api_entry.Add(sms);
                db.SaveChanges();

                test1 t1 = new test1();
                t1.name = String.Format("Sms Exception - Message: {0}, Stack: {1}", ex.Message, ex.StackTrace);
                db.test1.Add(t1);
                db.SaveChanges();
            }
        }

        private void callSms_whitelabel(string whitelabelid, Whitelabel_Sending_SMS_Templates smsstypes, string smsapi, string remmobile, params object[] vals)
        {
            string msgssss = string.Format(smsstypes.Templates, vals);
            string tempid = smsstypes.Templateid;
            string urlss = smsapi;

            sendsmsallnew_whitelabel(whitelabelid, remmobile, msgssss, urlss, tempid);
        }
    }
}