using System;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Vastwebmulti.Models
{
    /// <summary>
    /// Utility class for sending emails with priority queue management and send limit tracking
    /// </summary>
    public class CommUtilEmail
    {
        public void EmailLimitChk(string recepientEmail, string AdminToCC, string subject, string body, string callbackUrl, int ciunt = 5000)
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                var CCmail = db.Admin_details.FirstOrDefault();
                if (CCmail != null) { AdminToCC = CCmail.email2; }
                var email = db.priorityEmails.Where(a => a.Status == "Y").FirstOrDefault();
                if (email != null)
                {
                    if (email.Send_Limit == 0)
                    {
                        email.Status = "N";
                        db.Entry(email).State = EntityState.Modified;
                        db.SaveChanges();

                        var againChk = db.priorityEmails.Where(a => a.Status == "N" && a.Send_Limit == a.Total_Send_Limit).FirstOrDefault();
                        if (againChk != null)
                        {
                            againChk.Status = "Y";
                            db.Entry(againChk).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        else
                        {
                            Sent_Mail_History Emailsms = new Sent_Mail_History();
                            Emailsms.Mail_Id = recepientEmail;
                            Emailsms.SentMsg = "Mail not Sending, Because Your All Business Mail Sending Daily Limit Exceed/Over.";
                            Emailsms.SentDate = DateTime.Now;
                            Emailsms.UserId = "Admin";
                            db.Sent_Mail_History.Add(Emailsms);
                            db.SaveChanges();
                        }
                    }
                }
                else
                {
                    var StatuOn = db.priorityEmails.Where(a => a.Send_Limit > 0 && a.Send_Limit <= a.Total_Send_Limit).FirstOrDefault();
                    if (StatuOn != null)
                    {
                        StatuOn.Status = "Y";
                        db.Entry(StatuOn).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        Sent_Mail_History Emailsms = new Sent_Mail_History();
                        Emailsms.Mail_Id = recepientEmail;
                        Emailsms.SentMsg = "Mail not Sending, Because Your All Business Mail Sending Daily Limit Exceed/Over.";
                        Emailsms.SentDate = DateTime.Now;
                        Emailsms.UserId = "Admin";
                        db.Sent_Mail_History.Add(Emailsms);
                        db.SaveChanges();
                    }

                }
                var Chkemail = db.priorityEmails.Where(a => a.Status == "Y").FirstOrDefault();
                if (Chkemail != null)
                {
                    if (Chkemail.Send_Limit > 0 && Chkemail.Send_Limit <= Chkemail.Total_Send_Limit)
                    {
                        if (callbackUrl == "No CallBackUrl" || callbackUrl == "")
                        {
                            callbackUrl = body;
                        }
                        if (AdminToCC == null)
                        {
                            AdminToCC = recepientEmail;
                        }
                        using (MailMessage mailMessage = new MailMessage())
                        {
                            try
                            {
                                var ch = Chkemail;
                                var sendemail = Chkemail.Email;
                                var sendpassword = Chkemail.Password;
                                var sendhostname = Chkemail.Smpthost;
                                var portno = Chkemail.Port;
                                var enablessl = Chkemail.Enablessl;

                                mailMessage.From = new MailAddress(sendemail);
                                mailMessage.Subject = subject;
                                mailMessage.Body = body;
                                mailMessage.IsBodyHtml = true;
                                mailMessage.To.Add(new MailAddress(recepientEmail));
                                if (AdminToCC != "OK")
                                {
                                    if (subject == "Fund by Dealer To CC" || subject == "Fund by Master To CC") { callbackUrl = body; mailMessage.CC.Add(new MailAddress(AdminToCC)); mailMessage.CC.Add(new MailAddress(recepientEmail)); }
                                    else { mailMessage.CC.Add(new MailAddress(AdminToCC)); }
                                }
                                SmtpClient smtp = new SmtpClient();
                                smtp.Host = sendhostname;
                                smtp.EnableSsl = Convert.ToBoolean(enablessl);
                                System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential();
                                NetworkCred.UserName = sendemail;
                                NetworkCred.Password = sendpassword;
                                smtp.UseDefaultCredentials = true;
                                smtp.Credentials = NetworkCred;
                                smtp.Port = int.Parse(portno.ToString());

                                Sent_Mail_History Emailsms = new Sent_Mail_History();
                                Emailsms.Mail_Id = recepientEmail;
                                Emailsms.SentMsg = callbackUrl == "OTP NOT SAVING" ? "OTP Send Successfully on Admin Mail. Admin Must be Check your mailbox." : callbackUrl; // Security Purpose we not have history of this situation(Mail Changes)
                                Emailsms.SentDate = DateTime.Now;
                                Emailsms.UserId = "Admin";
                                db.Sent_Mail_History.Add(Emailsms);
                                db.SaveChanges();

                                //try
                                //{
                                //    await smtp.SendMailAsync(mailMessage);
                                //}
                                //catch (InvalidOperationException ex)
                                //{
                                //    ////////////After Sending//////////////
                                //    Task.Run(async () => { await smtp.SendMailAsync(mailMessage); }).Wait(1000);
                                //}


                                try
                                {
                                    var res = Task.Run(async () => { await smtp.SendMailAsync(mailMessage); }).Wait(ciunt);
                                }
                                catch (SmtpFailedRecipientsException ex)
                                {
                                    SmtpStatusCode status = ex.InnerExceptions[0].StatusCode;
                                    if (status == SmtpStatusCode.MailboxBusy ||
                                     status == SmtpStatusCode.MailboxUnavailable)
                                    {

                                    }

                                }

                                ////////////After Sending//////////////
                                int remainLimit = (int)Chkemail.Send_Limit - 1;
                                Chkemail.Send_Limit = remainLimit;
                                db.Entry(Chkemail).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            catch (Exception ex)
                            { }
                        }
                    }
                }
            }
        }



        public void EmailLimitChknew(string recepientEmail, string AdminToCC, string subject, string body, string callbackUrl, int ciunt = 5000)
        {

            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                
                var CCmail = db.Admin_details.FirstOrDefault();
                if (CCmail != null) { AdminToCC = CCmail.email2; }
              
               
                        if (callbackUrl == "No CallBackUrl" || callbackUrl == "")
                        {
                            callbackUrl = body;
                        }
                        if (AdminToCC == null)
                        {
                            AdminToCC = recepientEmail;
                        }
                        using (MailMessage mailMessage = new MailMessage())
                        {
                            try
                            {
                                //var ch = Chkemail;
                                //var sendemail = Chkemail.Email;
                                var sendemail = ConfigurationManager.AppSettings["UserNamenew"];
                                //var sendpassword = Chkemail.Password;
                                var sendpassword = ConfigurationManager.AppSettings["Passwordnew"];
                        //var sendhostname = Chkemail.Smpthost;
                                var sendhostname = ConfigurationManager.AppSettings["Hostnew"];
                        //var portno = Chkemail.Port;
                                var portno = ConfigurationManager.AppSettings["Portnew"];

                                var enablessl = ConfigurationManager.AppSettings["EnableSslnew"];

                                mailMessage.From = new MailAddress(sendemail);
                                mailMessage.Subject = subject;
                                mailMessage.Body = body;
                                mailMessage.IsBodyHtml = true;
                                mailMessage.To.Add(new MailAddress(recepientEmail));
                                if (AdminToCC != "OK")
                                {
                                    if (subject == "Fund by Dealer To CC" || subject == "Fund by Master To CC") { callbackUrl = body; mailMessage.CC.Add(new MailAddress(AdminToCC)); mailMessage.CC.Add(new MailAddress(recepientEmail)); }
                                    else { mailMessage.CC.Add(new MailAddress(AdminToCC)); }
                                }
                                SmtpClient smtp = new SmtpClient();
                                smtp.Host = sendhostname;
                                smtp.EnableSsl = Convert.ToBoolean(enablessl);
                                System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential();
                                NetworkCred.UserName = sendemail;
                                NetworkCred.Password = sendpassword;
                                smtp.UseDefaultCredentials = true;
                                smtp.Credentials = NetworkCred;
                                smtp.Port = int.Parse(portno.ToString());

                                Sent_Mail_History Emailsms = new Sent_Mail_History();
                                Emailsms.Mail_Id = recepientEmail;
                                Emailsms.SentMsg = callbackUrl == "OTP NOT SAVING" ? "OTP Send Successfully on Admin Mail. Admin Must be Check your mailbox." : callbackUrl; // Security Purpose we not have history of this situation(Mail Changes)
                                Emailsms.SentDate = DateTime.Now;
                                Emailsms.UserId = "Admin";
                                db.Sent_Mail_History.Add(Emailsms);
                                db.SaveChanges();

                                //try
                                //{
                                //    await smtp.SendMailAsync(mailMessage);
                                //}
                                //catch (InvalidOperationException ex)
                                //{
                                //    ////////////After Sending//////////////
                                //    Task.Run(async () => { await smtp.SendMailAsync(mailMessage); }).Wait(1000);
                                //}


                                try
                                {
                                    var res = Task.Run(async () => { await smtp.SendMailAsync(mailMessage); }).Wait(ciunt);
                                }
                                catch (SmtpFailedRecipientsException ex)
                                {
                                    SmtpStatusCode status = ex.InnerExceptions[0].StatusCode;
                                    if (status == SmtpStatusCode.MailboxBusy ||
                                     status == SmtpStatusCode.MailboxUnavailable)
                                    {

                                    }

                                }

                                ////////////After Sending//////////////
                              
                            }
                            catch (Exception ex)
                            { }
                        }
                    
                
            }
        }

        public void WhiteLabelEmailLimitChk(string recepientEmail, string AdminToCC, string subject, string body, string callbackUrl, string WhiteLabelAdminId)
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                var userid = WhiteLabelAdminId;
                var ChekWTMailExist = db.Whitelabel_priorityEmail.Where(a => a.Userid == userid).FirstOrDefault();
                if (ChekWTMailExist != null)
                {
                    var email = db.Whitelabel_priorityEmail.Where(a => a.Status == "Y" && a.Userid == userid).FirstOrDefault();
                    if (email != null)
                    {
                        if (email.Send_Limit == 0)
                        {
                            email.Status = "N";
                            db.Entry(email).State = EntityState.Modified;
                            db.SaveChanges();

                            var againChk = db.Whitelabel_priorityEmail.Where(a => a.Status == "N" && a.Send_Limit == 1000 && a.Userid == userid).FirstOrDefault();
                            if (againChk != null)
                            {
                                againChk.Status = "Y";
                                db.Entry(againChk).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            else
                            {
                                Whitelabel_Sent_Mail_History Emailsms = new Whitelabel_Sent_Mail_History();
                                Emailsms.Mail_Id = recepientEmail;
                                Emailsms.SentMsg = "Mail not Sending, Because Your All Business Mail Sending Daily Limit Exceed/Over.";
                                Emailsms.SentDate = DateTime.Now;
                                Emailsms.UserId = userid;
                                db.Whitelabel_Sent_Mail_History.Add(Emailsms);
                                db.SaveChanges();
                            }
                        }
                    }
                    else
                    {
                        var StatuOn = db.Whitelabel_priorityEmail.Where(a => a.Send_Limit > 0 && a.Send_Limit <= 1000 && a.Userid == userid).FirstOrDefault();
                        if (StatuOn != null)
                        {
                            StatuOn.Status = "Y";
                            db.Entry(StatuOn).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        else
                        {
                            Whitelabel_Sent_Mail_History Emailsms = new Whitelabel_Sent_Mail_History();
                            Emailsms.Mail_Id = recepientEmail;
                            Emailsms.SentMsg = "Mail not Sending, Because Your All Business Mail Sending Daily Limit Exceed/Over.";
                            Emailsms.SentDate = DateTime.Now;
                            Emailsms.UserId = userid;
                            db.Whitelabel_Sent_Mail_History.Add(Emailsms);
                            db.SaveChanges();
                        }

                    }
                    var Chkemail = db.Whitelabel_priorityEmail.Where(a => a.Status == "Y" && a.Userid == userid).FirstOrDefault();
                    if (Chkemail != null)
                    {
                        if (Chkemail.Send_Limit > 0 && Chkemail.Send_Limit <= 1000)
                        {
                            //var jjj = Chkemail.Email;
                            if (callbackUrl == "No CallBackUrl" || callbackUrl == "")
                            {
                                callbackUrl = body;
                            }
                            else if (callbackUrl == "OTP NOT SAVING")
                            {
                                callbackUrl = subject;
                            }
                            if (AdminToCC == null)
                            {
                                AdminToCC = recepientEmail;
                            }
                            using (MailMessage mailMessage = new MailMessage())
                            {
                                try
                                {
                                    var ch = Chkemail;
                                    var sendemail = Chkemail.Email;
                                    var sendpassword = Chkemail.Password;
                                    var sendhostname = Chkemail.Smpthost;
                                    var portno = Chkemail.Port;
                                    var enablessl = Chkemail.Enablessl;

                                    mailMessage.From = new MailAddress(sendemail);
                                    mailMessage.Subject = subject;
                                    mailMessage.Body = body;
                                    mailMessage.IsBodyHtml = true;
                                    mailMessage.To.Add(new MailAddress(recepientEmail));
                                    mailMessage.CC.Add(new MailAddress(AdminToCC));
                                    SmtpClient smtp = new SmtpClient();
                                    smtp.Host = sendhostname;
                                    smtp.EnableSsl = Convert.ToBoolean(enablessl);
                                    System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential();
                                    NetworkCred.UserName = sendemail;
                                    NetworkCred.Password = sendpassword;
                                    smtp.UseDefaultCredentials = true;
                                    smtp.Credentials = NetworkCred;
                                    smtp.Port = int.Parse(portno.ToString());

                                    Whitelabel_Sent_Mail_History Emailsms = new Whitelabel_Sent_Mail_History();
                                    Emailsms.Mail_Id = recepientEmail;
                                    Emailsms.SentMsg = callbackUrl;
                                    Emailsms.SentDate = DateTime.Now;
                                    Emailsms.UserId = userid;
                                    db.Whitelabel_Sent_Mail_History.Add(Emailsms);
                                    db.SaveChanges();

                                    smtp.Send(mailMessage);

                                    ////////////After Sending//////////////
                                    int remainLimit = (int)Chkemail.Send_Limit - 1;
                                    Chkemail.Send_Limit = remainLimit;
                                    db.Entry(Chkemail).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                                catch (Exception ex)
                                { }
                            }
                        }
                    }
                }
                else
                {
                    EmailLimitChk(recepientEmail, AdminToCC, subject, body, callbackUrl);
                }
            }
        }
    }
}