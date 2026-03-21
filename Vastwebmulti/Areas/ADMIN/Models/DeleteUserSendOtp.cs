using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    public class DeleteUserSendOtp
    {
        //Delete Master Id send Otp by admin
        public void sendmasterotp(int Otp)
        {
            try
            {
                VastwebmultiEntities db = new VastwebmultiEntities();
                var ToCC = db.Admin_details.FirstOrDefault().email;
                CommUtilEmail emailsend = new CommUtilEmail();
                emailsend.EmailLimitChk(ToCC, ToCC, "User Delete OTP", "Your OTP Is = " + Otp , "No CallBackUrl");
            }
            catch { }
            
            //using (MailMessage mailMessage = new MailMessage())
            //{
            //    var ch = (from pp in db.priorityEmails select pp);
            //    var sendemail = ch.Single().Email;
            //    var sendpassword = ch.Single().Password;
            //    var sendhostname = ch.Single().Smpthost;
            //    var portno = ch.Single().Port;
            //    var enablessl = ch.Single().Enablessl;
            //    //admin details
            //    var admin= (from gg in db.Admin_details select gg);
            //    var emailid = admin.Single().email;
            //    var Name = admin.Single().Name;
            //    //end
            //    mailMessage.From = new MailAddress(sendemail);
            //    mailMessage.Subject = "User Delete OTP";
            //    mailMessage.Body = "Your OTP Is = " + Otp ;
            //    mailMessage.IsBodyHtml = true;
            //    mailMessage.To.Add(new MailAddress(emailid));
            //    //mailMessage.CC.Add(new MailAddress(mailid));
            //    SmtpClient smtp = new SmtpClient();
            //    smtp.Host = sendhostname;
            //    smtp.EnableSsl = Convert.ToBoolean(enablessl);
            //    //smtp.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableSsl"]);
            //    System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential();
            //    NetworkCred.UserName = sendemail;
            //    NetworkCred.Password = sendpassword;
            //    smtp.UseDefaultCredentials = true;
            //    smtp.Credentials = NetworkCred;
            //    smtp.Port = int.Parse(portno.ToString());
            //    try
            //    {
            //        Sent_Mail_History Emailsms = new Sent_Mail_History();
            //        Emailsms.Mail_Id = emailid;
            //        Emailsms.SentMsg = "Your OTP Is = " + Otp; ;
            //        Emailsms.SentDate = DateTime.Now;
            //        Emailsms.UserId = "Admin";
            //        db.Sent_Mail_History.Add(Emailsms);
            //        db.SaveChanges();

            //        smtp.Send(mailMessage);
            //    }
            //    catch
            //    {

            //    }
            //}

        }

        //Delete Dealer By Master and send Otp on Master Email
        public void sendotpBymastermail(int Otp,string userid)
        {
            try
            {
                VastwebmultiEntities db = new VastwebmultiEntities();
                var ToCC = db.Admin_details.FirstOrDefault().email;
                var MasterEmail = db.Superstokist_details.Where(a => a.SSId == userid).SingleOrDefault().Email;
                CommUtilEmail emailsend = new CommUtilEmail();
                emailsend.EmailLimitChk(MasterEmail, ToCC, "User Delete OTP", "Your OTP Is = " + Otp, "No CallBackUrl");
            }
            catch { }
            //using (MailMessage mailMessage = new MailMessage())
            //{
            //    var ch = (from pp in db.priorityEmails select pp);
            //    var sendemail = ch.Single().Email;
            //    var sendpassword = ch.Single().Password;
            //    var sendhostname = ch.Single().Smpthost;
            //    var portno = ch.Single().Port;
            //    var enablessl = ch.Single().Enablessl;
            //    //admin details
            //    var master = (from gg in db.Superstokist_details where gg.SSId==userid select gg);
            //    var emailid = master.Single().Email;
            //    var Name = master.Single().SuperstokistName;
            //    //end
            //    mailMessage.From = new MailAddress(sendemail);
            //    mailMessage.Subject = "User Delete OTP";
            //    mailMessage.Body = "Your OTP Is = " + Otp;
            //    mailMessage.IsBodyHtml = true;
            //    mailMessage.To.Add(new MailAddress(emailid));
            //    //mailMessage.CC.Add(new MailAddress(mailid));
            //    SmtpClient smtp = new SmtpClient();
            //    smtp.Host = sendhostname;
            //    smtp.EnableSsl = Convert.ToBoolean(enablessl);
            //    //smtp.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableSsl"]);
            //    System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential();
            //    NetworkCred.UserName = sendemail;
            //    NetworkCred.Password = sendpassword;
            //    smtp.UseDefaultCredentials = true;
            //    smtp.Credentials = NetworkCred;
            //    smtp.Port = int.Parse(portno.ToString());
            //    try
            //    {
            //        Sent_Mail_History Emailsms = new Sent_Mail_History();
            //        Emailsms.Mail_Id = emailid;
            //        Emailsms.SentMsg = "Your OTP Is = " + Otp; ;
            //        Emailsms.SentDate = DateTime.Now;
            //        Emailsms.UserId = userid;
            //        db.Sent_Mail_History.Add(Emailsms);
            //        db.SaveChanges();

            //        smtp.Send(mailMessage);
            //    }
            //    catch
            //    {

            //    }
            //}

        }


        //Delete Retailer By Dealer and send Otp on Distributor Email
        public void sendotpBydealermail(int Otp, string userid)
        {
            try
            {
                VastwebmultiEntities db = new VastwebmultiEntities();
                var ToCC = db.Admin_details.FirstOrDefault().email;
                var DealerEmail = db.Dealer_Details.Where(a => a.DealerId == userid).SingleOrDefault().Email;
                CommUtilEmail emailsend = new CommUtilEmail();
                emailsend.EmailLimitChk(DealerEmail, ToCC, "User Delete OTP", "Your OTP Is = " + Otp, "No CallBackUrl");
            }
            catch { }
            
            //VastwebmultiEntities db = new VastwebmultiEntities();
            //using (MailMessage mailMessage = new MailMessage())
            //{
            //    var ch = (from pp in db.priorityEmails select pp);
            //    var sendemail = ch.Single().Email;
            //    var sendpassword = ch.Single().Password;
            //    var sendhostname = ch.Single().Smpthost;
            //    var portno = ch.Single().Port;
            //    var enablessl = ch.Single().Enablessl;
            //    //admin details
            //    var dealer = (from gg in db.Dealer_Details where gg.DealerId == userid select gg);
            //    var emailid = dealer.Single().Email;
            //    var Name = dealer.Single().DealerName;
            //    //end
            //    mailMessage.From = new MailAddress(sendemail);
            //    mailMessage.Subject = "User Delete OTP";
            //    mailMessage.Body = "Your OTP Is = " + Otp;
            //    mailMessage.IsBodyHtml = true;
            //    mailMessage.To.Add(new MailAddress(emailid));
            //    //mailMessage.CC.Add(new MailAddress(mailid));
            //    SmtpClient smtp = new SmtpClient();
            //    smtp.Host = sendhostname;
            //    smtp.EnableSsl = Convert.ToBoolean(enablessl);
            //    //smtp.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableSsl"]);
            //    System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential();
            //    NetworkCred.UserName = sendemail;
            //    NetworkCred.Password = sendpassword;
            //    smtp.UseDefaultCredentials = true;
            //    smtp.Credentials = NetworkCred;
            //    smtp.Port = int.Parse(portno.ToString());
            //    try
            //    {
            //        Sent_Mail_History Emailsms = new Sent_Mail_History();
            //        Emailsms.Mail_Id = emailid;
            //        Emailsms.SentMsg = "Your OTP Is = " + Otp; ;
            //        Emailsms.SentDate = DateTime.Now;
            //        Emailsms.UserId = userid;
            //        db.Sent_Mail_History.Add(Emailsms);
            //        db.SaveChanges();

            //        smtp.Send(mailMessage);
            //    }
            //    catch
            //    {

            //    }
            //}

        }
    }
   }