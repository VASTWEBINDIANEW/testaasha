using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.WHITELABEL.Models
{
    public class Whitelabel_DeleteUserSendOtp
    {
        //Delete Master Id send Otp by admin
        public void sendmasterotp(int Otp,string userid)
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                var admin = (from gg in db.WhiteLabel_userList.Where(aa => aa.WhiteLabelID == userid) select gg);
                var recepientEmail = admin.Single().EmailId;
                var ToCC = db.Admin_details.FirstOrDefault().email;
                CommUtilEmail emailsend = new CommUtilEmail();
                emailsend.WhiteLabelEmailLimitChk(recepientEmail, ToCC, "User Delete OTP", "Your OTP Is : " + Otp, "", userid);
            }

        }

        //Delete Dealer By Master and send Otp on Master Email
        public void sendotpBymastermail(int Otp, string userid,string whitelabelid)
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                var master = (from gg in db.Whitelabel_Superstokist_details where gg.SSId == userid select gg);
                var emailid = master.Single().Email;
                var ToCC = db.Admin_details.FirstOrDefault().email;
                CommUtilEmail emailsend = new CommUtilEmail();
                emailsend.WhiteLabelEmailLimitChk(emailid, ToCC, "User Delete OTP", "Your OTP Is : " + Otp, "", whitelabelid);
            }
        }


        //Delete Retailer By Dealer and send Otp on Distributor Email
        public void sendotpBydealermail(int Otp, string userid,string whitelabelid)
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                var dealer = (from gg in db.whitelabel_Dealer_Details where gg.DealerId == userid select gg);
                var emailid = dealer.Single().Email;
                var ToCC = db.Admin_details.FirstOrDefault().email;
                CommUtilEmail emailsend = new CommUtilEmail();
                emailsend.WhiteLabelEmailLimitChk(emailid, ToCC, "User Delete OTP", "Your OTP Is : " + Otp, "", whitelabelid);
            }
        }
    }
}