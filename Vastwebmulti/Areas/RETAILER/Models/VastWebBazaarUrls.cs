using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.RETAILER.Models
{
    public class VastWebBazaarUrls
    {
        public static string BASEURL = "http://api.vastbazaar.com/";
        public static string token = "token";
        public static string Remiter_detail = "/api/DMT/Remitter_details";
        public static string Remiter_Register = "api/DMT/Remitter_Register";
        public static string Ben_Register = "api/DMT/Beneficiary_register";
        public static string Ben_Register_Resend_Otp = "api/DMT/Beneficiary_register_resend_otp";
        public static string Ben_Register_Validate = "api/DMT/Beneficiary_register_Validate";
        public static string Ben_Account_Verification = "api/DMT/Beneficiary_Account_verify";
        public static string Ben_Delete = "api/DMT/Beneficiary_Delete";
        public static string Ben_Delete_validate = "api/DMT/Beneficiary_Delete_Vaildate";
        public static string fund_transfer = "api/DMT/Fund_Transfer";
        public static string bank_details = "api/DMT/Bank_details";
    }
}