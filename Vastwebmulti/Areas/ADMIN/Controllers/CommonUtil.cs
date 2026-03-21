using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.ADMIN.Controllers
{


    public class CommonUtil
    {
        VastwebmultiEntities db = new VastwebmultiEntities();

        public void Insertsendmail(string recepientEmail, string subject, string body, string callbackUrl)
        {
            try
            {
                var ToCC = db.Admin_details.FirstOrDefault().email;
                CommUtilEmail emailsend = new CommUtilEmail();
                emailsend.EmailLimitChk(recepientEmail, ToCC, subject, body, callbackUrl);
            }
            catch { }
        }
        public void InsertsendmailWelcome(string recepientEmail, string subject, string body, string callbackUrl)
        {
            try
            {
                var ToCC = db.Admin_details.FirstOrDefault().email;
                CommUtilEmail emailsend = new CommUtilEmail();
                emailsend.EmailLimitChk(recepientEmail, ToCC, subject, body, callbackUrl);
            }
            catch { }
        }
        public void InsertsendmailIMPS(string recepientEmail, string subject, string body)
        {
            try
            {
                var ToCC = db.Admin_details.FirstOrDefault().email;
                CommUtilEmail emailsend = new CommUtilEmail();
                emailsend.EmailLimitChk(recepientEmail, ToCC, subject, body, "No CallBackUrl");
            }
            catch { }
        }
        //Resend For Mail By ADMIN
        public void Rsendmailadmin(string adminemail, string subject, string body, string callbackUrl)
        {
            try
            {
                var ToCC = db.Admin_details.FirstOrDefault().email;
                CommUtilEmail emailsend = new CommUtilEmail();
                emailsend.EmailLimitChk(adminemail, ToCC, subject, body, callbackUrl);
            }
            catch { }
        }
        public void Insertsendmail_whitelabel(string userid, string recepientEmail, string subject, string body)
        {
            var ToCC = db.Admin_details.FirstOrDefault().email;
            CommUtilEmail emailsend = new CommUtilEmail();
            emailsend.WhiteLabelEmailLimitChk(recepientEmail, ToCC, subject, body, "", userid);

        }
        // Retailer SignUp and Create Retailer Creation
        public string PopulateBody(string userName, string title, string url, string description, string Email, string password, string pin, string referralcode)
        {
            userName = Email;
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/Email-Tamplete/emailverifaction.html")))
            {
                body = reader.ReadToEnd();
            }

            var Admin_details = db.Admin_details.SingleOrDefault();

            var logoPath = db.tblHeaderLogoes.Where(x => x.UserId == Admin_details.userid).FirstOrDefault();
            var logo = logoPath == null ? "" : logoPath.LogoImage;



            int state = Convert.ToInt32(Admin_details.State);
            int district = Convert.ToInt32(Admin_details.District);
            var City = db.State_Desc.Where(a => a.State_id == state).SingleOrDefault().State_name;
            var State = db.District_Desc.Where(a => a.Dist_id == district && a.State_id == state).SingleOrDefault().Dist_Desc;

            var userid = db.Users.Where(a => a.Email == userName).SingleOrDefault();
            var userroleid = db.UserRoles.Where(aa => aa.UserId == userid.UserId).SingleOrDefault().RoleId;
            var userrole = db.Roles.Where(aa => aa.RoleId == userroleid).SingleOrDefault().Name;

            var User_Name = ""; var User_City = ""; var User_State = ""; var User_Firm = ""; var TrPin = "";
            if (userrole == "Admin")
            {
                User_Name = Admin_details.Name;
                User_City = City;
                User_State = State;

                body = body.Replace("Your (TXN) Pin is", "");
                body = body.Replace("{pin}", "");
                body = body.Replace("Your Referral code is", "");
                body = body.Replace("{referralcode}", "");
            }
            else if (userrole == "master")
            {
                var Master_details = db.Superstokist_details.Where(a => a.SSId == userid.UserId).SingleOrDefault();
                int User_state = Convert.ToInt32(Master_details.State);
                int User_district = Convert.ToInt32(Master_details.District);

                User_Name = Master_details.SuperstokistName;
                User_Firm = Master_details.FarmName;
                User_City = db.State_Desc.Where(a => a.State_id == User_state).SingleOrDefault().State_name; ;
                User_State = db.District_Desc.Where(a => a.Dist_id == User_district && a.State_id == User_state).SingleOrDefault().Dist_Desc;

                body = body.Replace("Your (TXN) Pin is", "");
                body = body.Replace("{pin}", "");
                body = body.Replace("Your Referral code is", "");
                body = body.Replace("{referralcode}", "");
            }
            else if (userrole == "Dealer")
            {
                var Dealer_details = db.Dealer_Details.Where(a => a.DealerId == userid.UserId).SingleOrDefault();
                int User_state = Convert.ToInt32(Dealer_details.State);
                int User_district = Convert.ToInt32(Dealer_details.District);

                User_Name = Dealer_details.DealerName;
                User_Firm = Dealer_details.FarmName;
                User_City = db.State_Desc.Where(a => a.State_id == User_state).SingleOrDefault().State_name; ;
                User_State = db.District_Desc.Where(a => a.Dist_id == User_district && a.State_id == User_state).SingleOrDefault().Dist_Desc;

                body = body.Replace("{pin}", pin);
                body = body.Replace("Your Referral code is", "");
                body = body.Replace("{referralcode}", "");
            }
            else if (userrole == "Retailer")
            {
                var Retailer_details = db.Retailer_Details.Where(a => a.RetailerId == userid.UserId).SingleOrDefault();
                int User_state = Convert.ToInt32(Retailer_details.State);
                int User_district = Convert.ToInt32(Retailer_details.District);

                User_Name = Retailer_details.RetailerName;
                User_Firm = Retailer_details.Frm_Name;
                User_City = db.State_Desc.Where(a => a.State_id == User_state).SingleOrDefault().State_name; ;
                User_State = db.District_Desc.Where(a => a.Dist_id == User_district && a.State_id == User_state).SingleOrDefault().Dist_Desc;

                body = body.Replace("{pin}", pin);
                body = body.Replace("{referralcode}", referralcode);
            }
            else if (userrole == "API")
            {
                var Api_details = db.api_user_details.Where(a => a.apiid == userid.UserId).SingleOrDefault();
                int User_state = Convert.ToInt32(Api_details.state);
                int User_district = Convert.ToInt32(Api_details.district);

                User_Name = Api_details.username;
                User_Firm = Api_details.farmname;
                User_City = db.State_Desc.Where(a => a.State_id == User_state).SingleOrDefault().State_name; ;
                User_State = db.District_Desc.Where(a => a.Dist_id == User_district && a.State_id == User_state).SingleOrDefault().Dist_Desc;

                body = body.Replace("Your (TXN) Pin is", "");
                body = body.Replace("{pin}", "");
                body = body.Replace("Your Referral code is", "");
                body = body.Replace("{referralcode}", "");
            }


            body = body.Replace("{websiteurl}", Admin_details.WebsiteUrl);
            body = body.Replace("{adminmailid}", Admin_details.email1);
            body = body.Replace("{useremail}", userName);
            body = body.Replace("{mobile}", Admin_details.mobile1);
            body = body.Replace("{companyname}", Admin_details.Companyname);
            body = body.Replace("{city}", City);
            body = body.Replace("{State}", State);
            body = body.Replace("{UserName}", User_Name);
            body = body.Replace("{UserFirm}", User_Firm);
            body = body.Replace("{User_City}", User_City);
            body = body.Replace("{User_State}", User_State);

            String strPathAndQuery = System.Web.HttpContext.Current.Request.Url.PathAndQuery;
            String strUrl = System.Web.HttpContext.Current.Request.Url.AbsoluteUri.Replace(strPathAndQuery, "/");

            var baseurl = strUrl + logo;
            body = body.Replace("{logo}", baseurl);

            body = body.Replace("{ResetImage}", strUrl);
            body = body.Replace("{message}", description);

            body = body.Replace("{email}", Email);
            body = body.Replace("{password}", password);

            body = body.Replace("{Title}", Admin_details.Companyname);
            return body;
        }
        public string PopulateBodyWelcome(string userName, string title, string SignUp, string description, string Email, string password, string pin, string referralcode)
        {
            userName = Email;
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/Email-Tamplete/WelCome.html")))
            {
                body = reader.ReadToEnd();
            }

            var Admin_details = db.Admin_details.SingleOrDefault();
            var logoPath = db.tblHeaderLogoes.Where(x => x.UserId == Admin_details.userid).FirstOrDefault();
            var logo = logoPath == null ? "" : logoPath.LogoImage;



            int state = Convert.ToInt32(Admin_details.State);
            int district = Convert.ToInt32(Admin_details.District);
            var City = db.State_Desc.Where(a => a.State_id == state).SingleOrDefault().State_name;
            var State = db.District_Desc.Where(a => a.Dist_id == district && a.State_id == state).SingleOrDefault().Dist_Desc;

            var userid = db.Users.Where(a => a.Email == userName).SingleOrDefault();
            var userroleid = db.UserRoles.Where(aa => aa.UserId == userid.UserId).SingleOrDefault().RoleId;
            var userrole = db.Roles.Where(aa => aa.RoleId == userroleid).SingleOrDefault().Name;

            var User_Name = ""; var User_City = ""; var User_State = ""; var User_Firm = "";
            if (userrole == "Admin")
            {
                User_Name = Admin_details.Name;
                User_City = City;
                User_State = State;

                body = body.Replace("Your (TXN) Pin is", "");
                body = body.Replace("{pin}", "");
                body = body.Replace("Your Referral code is", "");
                body = body.Replace("{referralcode}", "");
            }
            else if (userrole == "master")
            {
                var Master_details = db.Superstokist_details.Where(a => a.SSId == userid.UserId).SingleOrDefault();
                int User_state = Convert.ToInt32(Master_details.State);
                int User_district = Convert.ToInt32(Master_details.District);

                User_Name = Master_details.SuperstokistName;
                User_Firm = Master_details.FarmName;
                User_City = db.State_Desc.Where(a => a.State_id == User_state).SingleOrDefault().State_name; ;
                User_State = db.District_Desc.Where(a => a.Dist_id == User_district && a.State_id == User_state).SingleOrDefault().Dist_Desc;

                body = body.Replace("Your (TXN) Pin is", "");
                body = body.Replace("{pin}", "");
                body = body.Replace("Your Referral code is", "");
                body = body.Replace("{referralcode}", "");
            }
            else if (userrole == "Dealer")
            {
                var Dealer_details = db.Dealer_Details.Where(a => a.DealerId == userid.UserId).SingleOrDefault();
                int User_state = Convert.ToInt32(Dealer_details.State);
                int User_district = Convert.ToInt32(Dealer_details.District);

                User_Name = Dealer_details.DealerName;
                User_Firm = Dealer_details.FarmName;
                User_City = db.State_Desc.Where(a => a.State_id == User_state).SingleOrDefault().State_name; ;
                User_State = db.District_Desc.Where(a => a.Dist_id == User_district && a.State_id == User_state).SingleOrDefault().Dist_Desc;

                body = body.Replace("{pin}", pin);
                body = body.Replace("Your Referral code is", "");
                body = body.Replace("{referralcode}", "");
            }
            else if (userrole == "Retailer")
            {
                var Retailer_details = db.Retailer_Details.Where(a => a.RetailerId == userid.UserId).SingleOrDefault();
                int User_state = Convert.ToInt32(Retailer_details.State);
                int User_district = Convert.ToInt32(Retailer_details.District);

                User_Name = Retailer_details.RetailerName;
                User_Firm = Retailer_details.Frm_Name;
                User_City = db.State_Desc.Where(a => a.State_id == User_state).SingleOrDefault().State_name; ;
                User_State = db.District_Desc.Where(a => a.Dist_id == User_district && a.State_id == User_state).SingleOrDefault().Dist_Desc;

                if (SignUp == "SignUp")
                {
                    body = body.Replace("{pin}", pin);
                    body = body.Replace("{referralcode}", referralcode);
                }
                else
                {
                    body = body.Replace("{pin}", pin);
                    body = body.Replace("Your Referral code is", "");
                    body = body.Replace("{referralcode}", "");
                }

            }
            else if (userrole == "API")
            {
                var Api_details = db.api_user_details.Where(a => a.apiid == userid.UserId).SingleOrDefault();
                int User_state = Convert.ToInt32(Api_details.state);
                int User_district = Convert.ToInt32(Api_details.district);

                User_Name = Api_details.username;
                User_Firm = Api_details.farmname;
                User_City = db.State_Desc.Where(a => a.State_id == User_state).SingleOrDefault().State_name; ;
                User_State = db.District_Desc.Where(a => a.Dist_id == User_district && a.State_id == User_state).SingleOrDefault().Dist_Desc;

                body = body.Replace("Your (TXN) Pin is", "");
                body = body.Replace("{pin}", "");
                body = body.Replace("Your Referral code is", "");
                body = body.Replace("{referralcode}", "");
            }


            body = body.Replace("{websiteurl}", Admin_details.WebsiteUrl);
            body = body.Replace("{adminmailid}", Admin_details.email1);
            body = body.Replace("{useremail}", userName);
            body = body.Replace("{mobile}", Admin_details.mobile1);
            body = body.Replace("{companyname}", Admin_details.Companyname);
            body = body.Replace("{city}", City);
            body = body.Replace("{State}", State);
            body = body.Replace("{UserName}", User_Name);
            body = body.Replace("{UserFirm}", User_Firm);
            body = body.Replace("{User_City}", User_City);
            body = body.Replace("{User_State}", User_State);

            String strPathAndQuery = System.Web.HttpContext.Current.Request.Url.PathAndQuery;
            String strUrl = System.Web.HttpContext.Current.Request.Url.AbsoluteUri.Replace(strPathAndQuery, "/");

            var baseurl = strUrl + logo;
            body = body.Replace("{logo}", baseurl);

            body = body.Replace("{ResetImage}", strUrl);
            body = body.Replace("{message}", description);
            body = body.Replace("{email}", Email);
            body = body.Replace("{password}", password);

            body = body.Replace("{Title}", Admin_details.Companyname);
            return body;
        }

        public string PopulateBodyWelcomeforEmployee(string userName, string description, string Email, string password)
        {
            userName = Email;
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/Email-Tamplete/WelComeEmp.html")))
            {
                body = reader.ReadToEnd();
            }

            var Admin_details = db.Admin_details.SingleOrDefault();
            var logoPath = db.tblHeaderLogoes.Where(x => x.UserId == Admin_details.userid).FirstOrDefault();
            var logo = logoPath == null ? "" : logoPath.LogoImage;



            int state = Convert.ToInt32(Admin_details.State);
            int district = Convert.ToInt32(Admin_details.District);
            var City = db.State_Desc.Where(a => a.State_id == state).SingleOrDefault().State_name;
            var State = db.District_Desc.Where(a => a.Dist_id == district && a.State_id == state).SingleOrDefault().Dist_Desc;

            var userid = db.Users.Where(a => a.Email == userName).SingleOrDefault();
            var userroleid = db.UserRoles.Where(aa => aa.UserId == userid.UserId).SingleOrDefault().RoleId;
            var userrole = db.Roles.Where(aa => aa.RoleId == userroleid).SingleOrDefault().Name;

            var User_Name = "";


            if (userrole == "Employee")
            {
                var emp_details = db.tbl_Admin_Employee.Where(a => a.EmployeeID == userid.UserId).SingleOrDefault();

                User_Name = emp_details.Employee_name;


            }


            body = body.Replace("{websiteurl}", Admin_details.WebsiteUrl);
            body = body.Replace("{adminmailid}", Admin_details.email1);
            body = body.Replace("{useremail}", userName);
            body = body.Replace("{mobile}", Admin_details.mobile1);
            body = body.Replace("{companyname}", Admin_details.Companyname);
            body = body.Replace("{UserName}", User_Name);


            String strPathAndQuery = System.Web.HttpContext.Current.Request.Url.PathAndQuery;
            String strUrl = System.Web.HttpContext.Current.Request.Url.AbsoluteUri.Replace(strPathAndQuery, "/");

            var baseurl = strUrl + logo;
            body = body.Replace("{logo}", baseurl);

            body = body.Replace("{ResetImage}", strUrl);
            body = body.Replace("{message}", description);
            body = body.Replace("{email}", Email);
            body = body.Replace("{password}", password);

            body = body.Replace("{Title}", Admin_details.Companyname);
            return body;
        }

        public string PopulateBodyEmail(string msg, string amount, string orderid, string FirmName, string mode, string servicefee, string tax, string total, string rrn, string accountno, string recivername, string ifsccode)
        {
            var websiteurl = db.Admin_details.SingleOrDefault().WebsiteUrl;
            var adminmailid = db.Admin_details.SingleOrDefault().email;
            var comp = db.tblContects.ToList();
            var companyname = comp.Count == 0 ? "" : comp.SingleOrDefault().CompnayName;
            var mobile = comp.Count == 0 ? "" : comp.SingleOrDefault().phone;
            var gmail = db.tblHeaderLogoes.ToList();
            var logo = gmail.Count == 0 ? "" : gmail.SingleOrDefault().LogoImage;

            string body = string.Empty;
            using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/IMPSEMAIL.html")))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{msg}", msg);
            body = body.Replace("{amount}", amount);
            body = body.Replace("{orderid}", orderid);
            body = body.Replace("{FirmName}", FirmName);
            body = body.Replace("{mode}", mode);
            body = body.Replace("{servicefee}", servicefee);
            body = body.Replace("{tax}", tax);
            body = body.Replace("{total}", total);
            body = body.Replace("{rrn}", rrn);
            body = body.Replace("{accountno}", accountno);
            body = body.Replace("{name}", recivername);
            body = body.Replace("{ifsccode}", ifsccode);
            var baseurl = "http://" + HttpContext.Current.Request.Url.Host + logo;
            body = body.Replace("{logo}", baseurl);
            return body;
        }
        // Create Api user, Master, Rch, super, Vendor, Whitelabel
        public string PopulateBodyWelcome(string userName, string title, string SignUp, string description, string Email, string password)
        {
            userName = Email;
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/Email-Tamplete/WelCome.html")))
            {
                body = reader.ReadToEnd();
            }

            var Admin_details = db.Admin_details.SingleOrDefault();
            var logoPath = db.tblHeaderLogoes.Where(x => x.UserId == Admin_details.userid).FirstOrDefault();
            var logo = logoPath == null ? "" : logoPath.LogoImage;


            int state = Convert.ToInt32(Admin_details.State);
            int district = Convert.ToInt32(Admin_details.District);
            var City = db.State_Desc.Where(a => a.State_id == state).SingleOrDefault().State_name;
            var State = db.District_Desc.Where(a => a.Dist_id == district && a.State_id == state).SingleOrDefault().Dist_Desc;

            var userid = db.Users.Where(a => a.Email == userName).SingleOrDefault();
            var userroleid = db.UserRoles.Where(aa => aa.UserId == userid.UserId).SingleOrDefault().RoleId;
            var userrole = db.Roles.Where(aa => aa.RoleId == userroleid).SingleOrDefault().Name;

            var User_Name = ""; var User_City = ""; var User_State = ""; var User_Firm = ""; var TransPin = ""; var ReferralCode = ""; var TrPin = "";
            if (userrole == "Admin")
            {
                User_Name = Admin_details.Name;
                User_City = City;
                User_State = State;

                body = body.Replace("Your (TXN) Pin is", "");
                body = body.Replace("{pin}", "");
                body = body.Replace("Your Referral code is", "");
                body = body.Replace("{referralcode}", "");
            }
            else if (userrole == "master")
            {
                var Master_details = db.Superstokist_details.Where(a => a.SSId == userid.UserId).SingleOrDefault();
                int User_state = Convert.ToInt32(Master_details.State);
                int User_district = Convert.ToInt32(Master_details.District);

                User_Name = Master_details.SuperstokistName;
                User_Firm = Master_details.FarmName;
                User_City = db.State_Desc.Where(a => a.State_id == User_state).SingleOrDefault().State_name; ;
                User_State = db.District_Desc.Where(a => a.Dist_id == User_district && a.State_id == User_state).SingleOrDefault().Dist_Desc;

                body = body.Replace("Your (TXN) Pin is", "");
                body = body.Replace("{pin}", "");
                body = body.Replace("Your Referral code is", "");
                body = body.Replace("{referralcode}", "");
            }
            else if (userrole == "Dealer")
            {
                var Dealer_details = db.Dealer_Details.Where(a => a.DealerId == userid.UserId).SingleOrDefault();
                int User_state = Convert.ToInt32(Dealer_details.State);
                int User_district = Convert.ToInt32(Dealer_details.District);

                User_Name = Dealer_details.DealerName;
                User_Firm = Dealer_details.FarmName;
                User_City = db.State_Desc.Where(a => a.State_id == User_state).SingleOrDefault().State_name; ;
                User_State = db.District_Desc.Where(a => a.Dist_id == User_district && a.State_id == User_state).SingleOrDefault().Dist_Desc;
                TransPin = Dealer_details.TransPIN;

                TrPin = Decrypt(TransPin);
                body = body.Replace("{pin}", TrPin);
                body = body.Replace("Your Referral code is", "");
                body = body.Replace("{referralcode}", "");
            }
            else if (userrole == "Retailer")
            {
                var Retailer_details = db.Retailer_Details.Where(a => a.RetailerId == userid.UserId).SingleOrDefault();
                int User_state = Convert.ToInt32(Retailer_details.State);
                int User_district = Convert.ToInt32(Retailer_details.District);

                User_Name = Retailer_details.RetailerName;
                User_Firm = Retailer_details.Frm_Name;
                User_City = db.State_Desc.Where(a => a.State_id == User_state).SingleOrDefault().State_name; ;
                User_State = db.District_Desc.Where(a => a.Dist_id == User_district && a.State_id == User_state).SingleOrDefault().Dist_Desc;
                TransPin = Retailer_details.PIN;
                ReferralCode = Retailer_details.referralcode;


                if (SignUp == "SignUp")
                {
                    body = body.Replace("{pin}", TransPin);
                    body = body.Replace("{referralcode}", ReferralCode);
                }
                else
                {
                    body = body.Replace("{pin}", TransPin);
                    body = body.Replace("Your Referral code is", "");
                    body = body.Replace("{referralcode}", "");
                }
            }
            else if (userrole == "API")
            {
                var Api_details = db.api_user_details.Where(a => a.apiid == userid.UserId).SingleOrDefault();
                int User_state = Convert.ToInt32(Api_details.state);
                int User_district = Convert.ToInt32(Api_details.district);

                User_Name = Api_details.username;
                User_Firm = Api_details.farmname;
                User_City = db.State_Desc.Where(a => a.State_id == User_state).SingleOrDefault().State_name; ;
                User_State = db.District_Desc.Where(a => a.Dist_id == User_district && a.State_id == User_state).SingleOrDefault().Dist_Desc;

                body = body.Replace("Your (TXN) Pin is", "");
                body = body.Replace("{pin}", "");
                body = body.Replace("Your Referral code is", "");
                body = body.Replace("{referralcode}", "");
            }
            else if (userrole == "Vendor")
            {
                var Vendor_details = db.Vendor_details.Where(a => a.userid == userid.UserId).SingleOrDefault();
                int User_state = Convert.ToInt32(Vendor_details.state);
                int User_district = Convert.ToInt32(Vendor_details.district);

                User_Name = Vendor_details.Name;
                User_Firm = Vendor_details.Firmname;
                User_City = db.State_Desc.Where(a => a.State_id == User_state).SingleOrDefault().State_name; ;
                User_State = db.District_Desc.Where(a => a.Dist_id == User_district && a.State_id == User_state).SingleOrDefault().Dist_Desc;

                body = body.Replace("Your (TXN) Pin is", "");
                body = body.Replace("{pin}", "");
                body = body.Replace("Your Referral code is", "");
                body = body.Replace("{referralcode}", "");
            }
            else if (userrole == "RCH")
            {
                var Rch_details = db.RCH_Details.Where(a => a.RCHId == userid.UserId).SingleOrDefault();
                int User_state = Convert.ToInt32(Rch_details.State);
                int User_district = Convert.ToInt32(Rch_details.District);

                User_Name = Rch_details.RCHName;
                User_Firm = Rch_details.FarmName;
                User_City = db.State_Desc.Where(a => a.State_id == User_state).SingleOrDefault().State_name; ;
                User_State = db.District_Desc.Where(a => a.Dist_id == User_district && a.State_id == User_state).SingleOrDefault().Dist_Desc;

                body = body.Replace("Your (TXN) Pin is", "");
                body = body.Replace("{pin}", "");
                body = body.Replace("Your Referral code is", "");
                body = body.Replace("{referralcode}", "");
            }
            else if (userrole == "Whitelabel")
            {
                var WhiteLabel_details = db.WhiteLabel_userList.Where(a => a.WhiteLabelID == userid.UserId).SingleOrDefault();
                int User_state = Convert.ToInt32(WhiteLabel_details.state);
                int User_district = Convert.ToInt32(WhiteLabel_details.district);

                User_Name = WhiteLabel_details.Name;
                User_Firm = WhiteLabel_details.FrmName;
                User_City = db.State_Desc.Where(a => a.State_id == User_state).SingleOrDefault().State_name; ;
                User_State = db.District_Desc.Where(a => a.Dist_id == User_district && a.State_id == User_state).SingleOrDefault().Dist_Desc;

                body = body.Replace("Your (TXN) Pin is", "");
                body = body.Replace("{pin}", "");
                body = body.Replace("Your Referral code is", "");
                body = body.Replace("{referralcode}", "");
            }

            body = body.Replace("{websiteurl}", Admin_details.WebsiteUrl);
            body = body.Replace("{adminmailid}", Admin_details.email1);
            body = body.Replace("{useremail}", userName);
            body = body.Replace("{mobile}", Admin_details.mobile1);
            body = body.Replace("{companyname}", Admin_details.Companyname);
            body = body.Replace("{city}", City);
            body = body.Replace("{State}", State);
            body = body.Replace("{UserName}", User_Name);
            body = body.Replace("{UserFirm}", User_Firm);
            body = body.Replace("{User_City}", User_City);
            body = body.Replace("{User_State}", User_State);

            String strPathAndQuery = System.Web.HttpContext.Current.Request.Url.PathAndQuery;
            String strUrl = System.Web.HttpContext.Current.Request.Url.AbsoluteUri.Replace(strPathAndQuery, "/");

            var baseurl = strUrl + logo;
            body = body.Replace("{logo}", baseurl);

            body = body.Replace("{ResetImage}", strUrl);
            body = body.Replace("{message}", description);
            body = body.Replace("{email}", Email);
            body = body.Replace("{password}", password);

            body = body.Replace("{Title}", Admin_details.Companyname);
            return body;
        }
        public string PopulateBodyDealer(string userName, string title, string url, string description, string Email, string password)
        {
            userName = Email;
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/Email-Tamplete/emailverifaction.html")))
            {
                body = reader.ReadToEnd();
            }

            var Admin_details = db.Admin_details.SingleOrDefault();
            var logoPath = db.tblHeaderLogoes.Where(x => x.UserId == Admin_details.userid).FirstOrDefault();
            var logo = logoPath == null ? "" : logoPath.LogoImage;


            int state = Convert.ToInt32(Admin_details.State);
            int district = Convert.ToInt32(Admin_details.District);
            var City = db.State_Desc.Where(a => a.State_id == state).SingleOrDefault().State_name;
            var State = db.District_Desc.Where(a => a.Dist_id == district && a.State_id == state).SingleOrDefault().Dist_Desc;

            var userid = db.Users.Where(a => a.Email == userName).SingleOrDefault();
            var userroleid = db.UserRoles.Where(aa => aa.UserId == userid.UserId).SingleOrDefault().RoleId;
            var userrole = db.Roles.Where(aa => aa.RoleId == userroleid).SingleOrDefault().Name;

            var User_Name = ""; var User_City = ""; var User_State = ""; var User_Firm = ""; var TransPin = ""; var ReferralCode = "";
            if (userrole == "Admin")
            {
                User_Name = Admin_details.Name;
                User_City = City;
                User_State = State;
            }
            else if (userrole == "master")
            {
                var Master_details = db.Superstokist_details.Where(a => a.SSId == userid.UserId).SingleOrDefault();
                int User_state = Convert.ToInt32(Master_details.State);
                int User_district = Convert.ToInt32(Master_details.District);

                User_Name = Master_details.SuperstokistName;
                User_Firm = Master_details.FarmName;
                User_City = db.State_Desc.Where(a => a.State_id == User_state).SingleOrDefault().State_name; ;
                User_State = db.District_Desc.Where(a => a.Dist_id == User_district && a.State_id == User_state).SingleOrDefault().Dist_Desc;
            }
            else if (userrole == "Dealer")
            {
                var Dealer_details = db.Dealer_Details.Where(a => a.DealerId == userid.UserId).SingleOrDefault();
                int User_state = Convert.ToInt32(Dealer_details.State);
                int User_district = Convert.ToInt32(Dealer_details.District);

                User_Name = Dealer_details.DealerName;
                User_Firm = Dealer_details.FarmName;
                User_City = db.State_Desc.Where(a => a.State_id == User_state).SingleOrDefault().State_name; ;
                User_State = db.District_Desc.Where(a => a.Dist_id == User_district && a.State_id == User_state).SingleOrDefault().Dist_Desc;
                TransPin = Dealer_details.TransPIN;
            }
            else if (userrole == "Retailer")
            {
                var Retailer_details = db.Retailer_Details.Where(a => a.RetailerId == userid.UserId).SingleOrDefault();
                int User_state = Convert.ToInt32(Retailer_details.State);
                int User_district = Convert.ToInt32(Retailer_details.District);

                User_Name = Retailer_details.RetailerName;
                User_Firm = Retailer_details.Frm_Name;
                User_City = db.State_Desc.Where(a => a.State_id == User_state).SingleOrDefault().State_name; ;
                User_State = db.District_Desc.Where(a => a.Dist_id == User_district && a.State_id == User_state).SingleOrDefault().Dist_Desc;
                TransPin = Retailer_details.PIN;
                ReferralCode = Retailer_details.referralcode;
            }
            else if (userrole == "API")
            {
                var Api_details = db.api_user_details.Where(a => a.apiid == userid.UserId).SingleOrDefault();
                int User_state = Convert.ToInt32(Api_details.state);
                int User_district = Convert.ToInt32(Api_details.district);

                User_Name = Api_details.username;
                User_Firm = Api_details.farmname;
                User_City = db.State_Desc.Where(a => a.State_id == User_state).SingleOrDefault().State_name; ;
                User_State = db.District_Desc.Where(a => a.Dist_id == User_district && a.State_id == User_state).SingleOrDefault().Dist_Desc;
            }
            else if (userrole == "Vendor")
            {
                var Vendor_details = db.Vendor_details.Where(a => a.userid == userid.UserId).SingleOrDefault();
                int User_state = Convert.ToInt32(Vendor_details.state);
                int User_district = Convert.ToInt32(Vendor_details.district);

                User_Name = Vendor_details.Name;
                User_Firm = Vendor_details.Firmname;
                User_City = db.State_Desc.Where(a => a.State_id == User_state).SingleOrDefault().State_name; ;
                User_State = db.District_Desc.Where(a => a.Dist_id == User_district && a.State_id == User_state).SingleOrDefault().Dist_Desc;
            }
            else if (userrole == "RCH")
            {
                var Rch_details = db.RCH_Details.Where(a => a.RCHId == userid.UserId).SingleOrDefault();
                int User_state = Convert.ToInt32(Rch_details.State);
                int User_district = Convert.ToInt32(Rch_details.District);

                User_Name = Rch_details.RCHName;
                User_Firm = Rch_details.FarmName;
                User_City = db.State_Desc.Where(a => a.State_id == User_state).SingleOrDefault().State_name; ;
                User_State = db.District_Desc.Where(a => a.Dist_id == User_district && a.State_id == User_state).SingleOrDefault().Dist_Desc;
            }
            else if (userrole == "Whitelabel")
            {
                var WhiteLabel_details = db.WhiteLabel_userList.Where(a => a.WhiteLabelID == userid.UserId).SingleOrDefault();
                int User_state = Convert.ToInt32(WhiteLabel_details.state);
                int User_district = Convert.ToInt32(WhiteLabel_details.district);

                User_Name = WhiteLabel_details.Name;
                User_Firm = WhiteLabel_details.FrmName;
                User_City = db.State_Desc.Where(a => a.State_id == User_state).SingleOrDefault().State_name; ;
                User_State = db.District_Desc.Where(a => a.Dist_id == User_district && a.State_id == User_state).SingleOrDefault().Dist_Desc;
            }

            else if (userrole == "Employee")
            {

                var emp_details = db.tbl_Admin_Employee.Where(a => a.EmployeeID == userid.UserId).SingleOrDefault();
                int User_state = Convert.ToInt32(emp_details.State);
                int User_district = Convert.ToInt32(emp_details.District);

                User_Name = emp_details.Employee_name;
                User_Firm = "";
                User_City = "";
                User_State = "";
            }

            body = body.Replace("{websiteurl}", Admin_details.WebsiteUrl);
            body = body.Replace("{adminmailid}", Admin_details.email1);
            body = body.Replace("{useremail}", userName);
            body = body.Replace("{mobile}", Admin_details.mobile1);
            body = body.Replace("{companyname}", Admin_details.Companyname);
            body = body.Replace("{city}", City);
            body = body.Replace("{State}", State);
            body = body.Replace("{UserName}", User_Name);
            body = body.Replace("{UserFirm}", User_Firm);
            body = body.Replace("{User_City}", User_City);
            body = body.Replace("{User_State}", User_State);

            String strPathAndQuery = System.Web.HttpContext.Current.Request.Url.PathAndQuery;
            String strUrl = System.Web.HttpContext.Current.Request.Url.AbsoluteUri.Replace(strPathAndQuery, "/");

            var baseurl = strUrl + logo;
            body = body.Replace("{logo}", baseurl);

            body = body.Replace("{ResetImage}", strUrl);
            body = body.Replace("{message}", description);
            body = body.Replace("{email}", Email);
            body = body.Replace("{password}", password);
            body = body.Replace("{pin}", TransPin);
            body = body.Replace("{referralcode}", ReferralCode);
            body = body.Replace("{Title}", Admin_details.Companyname);
            return body;
        }

        public string PopulateBodyOTP(string title, string description, string user)
        {

            string body = string.Empty;
            using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/otpTemplete.html")))
            {
                body = reader.ReadToEnd();
            }
            // body = body.Replace("{UserName}", userName);
            body = body.Replace("{Title}", title.ToUpper());
            //body = body.Replace("{Url}", url);
            body = body.Replace("{message}", description);
            body = body.Replace("{User}", user);

            return body;
        }

        // Delete Distributor User send Mail
        public void DeleteDealer(string adminmailid, string dealermailid, string otp)
        {
            try
            {
                var ToCC = db.Admin_details.FirstOrDefault().email;
                CommUtilEmail emailsend = new CommUtilEmail();
                emailsend.EmailLimitChk(dealermailid, ToCC, "Delete OTP", "Distributor ID is -" + dealermailid + " Deleted OTP is : " + otp, "No CallBackUrl");
            }
            catch { }
        }

        //Delete Retailer User send mail ADMin
        public void DeleteRetailer(string adminmailid, string retailermailid, string otp)
        {
            try
            {
                var ToCC = db.Admin_details.FirstOrDefault().email;
                CommUtilEmail emailsend = new CommUtilEmail();
                emailsend.EmailLimitChk(retailermailid, ToCC, "Delete OTP", "Retailer ID is -" + retailermailid + " Deleted OTP is : " + otp, "No CallBackUrl");
            }
            catch { }


        }

        //send Refferal code Retailer Email
        public void SendReferralcodemail(string RetailerName, string RetailerEmail, long referralcode)
        {
            try
            {
                var ToCC = db.Admin_details.FirstOrDefault().email;
                CommUtilEmail emailsend = new CommUtilEmail();
                emailsend.EmailLimitChk(RetailerEmail, ToCC, "Referral Code", "<span style='font-size:15px;font-weight:700;'>Dear</span>" + "  " + RetailerName + "<br/>" + " Your Referral Code is : " + referralcode, "No CallBackUrl");
            }
            catch { }
        }

        public string WhitelabelRetailerPopulateBody(string userName, string title, string url, string description, string Email, string password, string pin, string referralcode, string whitelabelid)
        {
            var websiteurl = db.WhiteLabel_userList.Where(a => a.WhiteLabelID == whitelabelid).SingleOrDefault().websitename;
            var adminmailid = db.WhiteLabel_userList.Where(a => a.WhiteLabelID == whitelabelid).SingleOrDefault().EmailId;
            var comp = db.tblContects.ToList();
            var companyname = comp.Count == 0 ? "" : comp.SingleOrDefault().CompnayName;
            var mobile = comp.Count == 0 ? "" : comp.SingleOrDefault().phone;
            var gmail = db.tblForgetGmailContents.ToList();
            var logo = gmail.Count == 0 ? "" : gmail.SingleOrDefault().Image;
            var Gmailsubject = gmail.Count == 0 ? "" : gmail.SingleOrDefault().Subject;
            var GmailContent = gmail.Count == 0 ? "" : gmail.SingleOrDefault().GmailContent;
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/Email-Tamplete/emailverifaction.html")))
            {
                body = reader.ReadToEnd();
            }
            using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/Email-Tamplete/WelCome.html")))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{websiteurl}", websiteurl);
            body = body.Replace("{adminmailid}", adminmailid);
            body = body.Replace("{mobile}", mobile);
            var baseurl = "http://" + HttpContext.Current.Request.Url.Host + logo;
            body = body.Replace("{logo}", baseurl);
            body = body.Replace("{companyname}", companyname);
            body = body.Replace("{subject}", Gmailsubject);
            body = body.Replace("{content}", GmailContent);
            body = body.Replace("{Title}", title.ToUpper());
            body = body.Replace("{message}", description);
            body = body.Replace("{email}", Email);
            body = body.Replace("{password}", password);
            body = body.Replace("{pin}", pin);
            body = body.Replace("{referralcode}", referralcode);
            return body;
        }

        public string WhitelabelPopulateBodyDealer(string userName, string title, string url, string description, string Email, string password, string whitelabelid)
        {
            var websiteurl = db.WhiteLabel_userList.Where(a => a.WhiteLabelID == whitelabelid).SingleOrDefault().websitename;
            var adminmailid = db.WhiteLabel_userList.Where(a => a.WhiteLabelID == whitelabelid).SingleOrDefault().EmailId;
            var comp = db.tblContects.ToList();
            var companyname = comp.Count == 0 ? "" : comp.SingleOrDefault().CompnayName;
            var mobile = comp.Count == 0 ? "" : comp.SingleOrDefault().phone;
            var gmail = db.tblForgetGmailContents.ToList();
            var logo = gmail.Count == 0 ? "" : gmail.SingleOrDefault().Image;
            var Gmailsubject = gmail.Count == 0 ? "" : gmail.SingleOrDefault().Subject;
            var GmailContent = gmail.Count == 0 ? "" : gmail.SingleOrDefault().GmailContent;
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/Email-Tamplete/emailverifaction.html")))
            {
                body = reader.ReadToEnd();
            }
            using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/Email-Tamplete/WelCome.html")))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{websiteurl}", websiteurl);
            body = body.Replace("{adminmailid}", adminmailid);
            body = body.Replace("{mobile}", mobile);
            var baseurl = "http://" + HttpContext.Current.Request.Url.Host + logo;
            body = body.Replace("{logo}", baseurl);
            body = body.Replace("{companyname}", companyname);
            body = body.Replace("{subject}", Gmailsubject);
            body = body.Replace("{content}", GmailContent);
            body = body.Replace("{Title}", title.ToUpper());
            body = body.Replace("{message}", description);
            body = body.Replace("{email}", Email);
            body = body.Replace("{password}", password);
            return body;
        }



        static readonly string PasswordHash = "P@@Sw0rd";
        static readonly string SaltKey = "S@LT&KEY";
        static readonly string VIKey = "@1B2c3D4e5F6g7H8";

        public static string Decrypt(string encryptedText)
        {
            byte[] cipherTextBytes = Convert.FromBase64String(encryptedText);
            byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.None };

            var decryptor = symmetricKey.CreateDecryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));
            var memoryStream = new MemoryStream(cipherTextBytes);
            var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];

            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount).TrimEnd("\0".ToCharArray());
        }
    }
}