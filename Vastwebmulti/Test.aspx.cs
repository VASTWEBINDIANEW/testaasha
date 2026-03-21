using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Vastwebmulti
{
    public partial class Test : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            using (MailMessage mailMessage = new MailMessage())
            {
                string mailfrom = "noreply@redpay.in";
                string password = "3000529594";

                mailMessage.From = new MailAddress(mailfrom);
                mailMessage.Subject = "Welcome";
                mailMessage.Body = "Testing";
                mailMessage.IsBodyHtml = true;
                mailMessage.To.Add("madansaini90@gmail.com");
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "mail.redpay.in";
                smtp.EnableSsl = false;
                System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential();
                NetworkCred.UserName = mailfrom;
                NetworkCred.Password = password;
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = NetworkCred;
                smtp.Port = 25;
                smtp.Send(mailMessage);

            }

        }
    }
}