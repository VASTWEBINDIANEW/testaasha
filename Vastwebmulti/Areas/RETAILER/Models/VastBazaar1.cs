using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace Vastwebmulti.Areas.RETAILER.Models
{
    public class VastBazaar1
    {
        #region DMT
        public IRestResponse Remitter_details(string mobile, string token)
        {
            var client = new RestClient("http://api.vastbazaar.com/api/DMT1/Remitter_details");
            var request = new RestRequest(Method.POST);
            request.AddHeader("authorization", "bearer " + token);
            request.AddHeader("content-type", "application/json");
            var data = new
            {
                mobile = mobile
            };
            var serializer = new JavaScriptSerializer();
            var json = serializer.Serialize(data);
            request.AddParameter("application/json", json, ParameterType.RequestBody);
            IRestResponse responseall = client.Execute(request);
            return responseall;
        }
        public IRestResponse Remitter_Register(string mobile, string name, string pincode, string surname, string token)
        {
            var client = new RestClient("http://api.vastbazaar.com/api/DMT1/Remitter_Register");
            var request = new RestRequest(Method.POST);
            request.AddHeader("authorization", "bearer " + token);
            request.AddHeader("content-type", "application/json");
            var data = new
            {
                mobile = mobile,
                name = name,
                surname= surname,
                pincode = pincode

            };
            var serializer = new JavaScriptSerializer();
            var json = serializer.Serialize(data);
            request.AddParameter("application/json", json, ParameterType.RequestBody);
            IRestResponse responseall = client.Execute(request);
            return responseall;
        }

        public IRestResponse Remitter_Register_Validate(string mobile, int OTP, string remitterid, string outletid , string token)
        {
            var client = new RestClient("http://api.vastbazaar.com/api/DMT1/Remitter_Register_Validate");
            var request = new RestRequest(Method.POST);
            request.AddHeader("authorization", "bearer " + token);
            request.AddHeader("content-type", "application/json");
            var data = new
            {
                mobile = mobile,
                OTP = OTP,
                remitterid = remitterid,
                outletid= outletid
            };
            var serializer = new JavaScriptSerializer();
            var json = serializer.Serialize(data);
            request.AddParameter("application/json", json, ParameterType.RequestBody);
            IRestResponse responseall = client.Execute(request);
            return responseall;
        }
        public IRestResponse Beneficiary_Delete(string beneficiaryid, string remitterid, string token, string SenderNumber)
        {
            var client = new RestClient("http://api.vastbazaar.com/api/DMT1/Beneficiary_Delete");
            var request = new RestRequest(Method.POST);
            request.AddHeader("authorization", "bearer " + token);
            request.AddHeader("content-type", "application/json");
            var data = new
            {
                Sendernumber = SenderNumber,
                beneficiaryid = beneficiaryid,
                remitterid = remitterid


            };

            var serializer = new JavaScriptSerializer();
            var json = serializer.Serialize(data);
            request.AddParameter("application/json", json, ParameterType.RequestBody);
            IRestResponse responseall = client.Execute(request);
            return responseall;
        }
        public IRestResponse Beneficiary_register_Validate(string beneficiaryid, string remitterid, string otp, string token, string sendernumber)
        {
            var client = new RestClient("http://api.vastbazaar.com/api/DMT1/Beneficiary_register_Validate");
            var request = new RestRequest(Method.POST);
            request.AddHeader("authorization", "bearer " + token);
            request.AddHeader("content-type", "application/json");
            var data = new
            {
                Sendernumber = sendernumber,
                beneficiaryid = beneficiaryid,
                remitterid = remitterid,
                otp = otp

            };

            var serializer = new JavaScriptSerializer();
            var json = serializer.Serialize(data);
            request.AddParameter("application/json", json, ParameterType.RequestBody);
            IRestResponse responseall = client.Execute(request);
            return responseall;
        }
        public IRestResponse Beneficiary_Delete_Vaildate(string beneficiaryid, string remitterid, string otp, string token, string SenderNumber)
        {
            var client = new RestClient("http://api.vastbazaar.com/api/DMT1/Beneficiary_Delete_Vaildate");
            var request = new RestRequest(Method.POST);
            request.AddHeader("authorization", "bearer " + token);
            request.AddHeader("content-type", "application/json");
            var data = new
            {
                SenderNumber = SenderNumber,
                beneficiaryid = beneficiaryid,
                remitterid = remitterid,
                otp = otp

            };

            var serializer = new JavaScriptSerializer();
            var json = serializer.Serialize(data);
            request.AddParameter("application/json", json, ParameterType.RequestBody);
            IRestResponse responseall = client.Execute(request);
            return responseall;
        }
        public IRestResponse Beneficiary_register(string remitterid, string name, string mobile, string ifsc, string account, string token)
        {
            var client = new RestClient("http://api.vastbazaar.com/api/DMT1/Beneficiary_register");
            var request = new RestRequest(Method.POST);
            request.AddHeader("authorization", "bearer " + token);
            request.AddHeader("content-type", "application/json");
            var data = new
            {
                remitterid = remitterid,
                name = name,
                mobile = mobile,
                ifsc = ifsc,
                account = account
            };

            var serializer = new JavaScriptSerializer();
            var json = serializer.Serialize(data);
            request.AddParameter("application/json", json, ParameterType.RequestBody);
            IRestResponse responseall = client.Execute(request);
            return responseall;
        }
        public IRestResponse Beneficiary_register_resend_otp(string remitterid, string beneficiaryid, string token)
        {
            var client = new RestClient("http://api.vastbazaar.com/api/DMT1/Beneficiary_register_resend_otp");
            var request = new RestRequest(Method.POST);
            request.AddHeader("authorization", "bearer " + token);
            request.AddHeader("content-type", "application/json");
            var data = new
            {
                remitterid = remitterid,
                beneficiaryid = beneficiaryid
            };

            var serializer = new JavaScriptSerializer();
            var json = serializer.Serialize(data);
            request.AddParameter("application/json", json, ParameterType.RequestBody);
            IRestResponse responseall = client.Execute(request);
            return responseall;
        }
        public IRestResponse Beneficiary_Account_verify(string remittermobile, string account, string ifsc, string agentid, string token, string bnkname)
        {
            var client = new RestClient("http://api.vastbazaar.com/api/DMT1/Beneficiary_Account_verify");
            var request = new RestRequest(Method.POST);
            request.AddHeader("authorization", "bearer " + token);
            request.AddHeader("content-type", "application/json");
            var data = new
            {
                remittermobile = remittermobile,
                account = account,
                ifsc = ifsc,
                Bankname = bnkname,
                agentid = agentid
            };

            var serializer = new JavaScriptSerializer();
            var json = serializer.Serialize(data);
            request.AddParameter("application/json", json, ParameterType.RequestBody);
            IRestResponse responseall = client.Execute(request);
            return responseall;
        }
        public IRestResponse Fund_Transfer(string remittermobile, string benid, string agentid, string amount, string mode, string accountno, string ifsccode, string token, string bankname)
        {
            var client = new RestClient("http://api.vastbazaar.com/api/DMT1/Fund_Transfer");
            var request = new RestRequest(Method.POST);
            request.AddHeader("authorization", "bearer " + token);
            request.AddHeader("content-type", "application/json");
            var data = new
            {
                remittermobile = remittermobile,
                beneficiaryid = benid,
                agentid = agentid,
                amount = amount,
                mode = mode,
                accountno = accountno,
                bankname = bankname,
                ifsccode = ifsccode


            };

            var serializer = new JavaScriptSerializer();
            var json = serializer.Serialize(data);
            request.AddParameter("application/json", json, ParameterType.RequestBody);
            IRestResponse responseall = client.Execute(request);
            return responseall;
        }
        public IRestResponse Bank_details(string account, string token)
        {
            var client = new RestClient("http://api.vastbazaar.com/api/DMT1/Bank_details");
            var request = new RestRequest(Method.POST);
            request.AddHeader("authorization", "bearer " + token);
            request.AddHeader("content-type", "application/json");
            var data = new
            {
                account = account
            };

            var serializer = new JavaScriptSerializer();
            var json = serializer.Serialize(data);
            request.AddParameter("application/json", json, ParameterType.RequestBody);
            IRestResponse responseall = client.Execute(request);
            return responseall;
        }
        #endregion

        #region Digital Gift Card
        public IRestResponse ProductDetails(string optcode, string token )
        {
            var client = new RestClient("http://api.vastbazaar.com/api/GIftCard/ProductDetails");
            var request = new RestRequest(Method.POST);
            request.AddHeader("authorization", "bearer " + token);
            request.AddHeader("content-type", "application/json");
            var data = new
            {
                optcode = optcode
            };
            var serializer = new JavaScriptSerializer();
            var json = serializer.Serialize(data);
            request.AddParameter("application/json", json, ParameterType.RequestBody);
            IRestResponse responseall = client.Execute(request);
            return responseall;
        }
        public IRestResponse Transaction(string optcode, string token,string agentid,decimal amount,string sender_name,string sender_email,string receiver_name,string receiver_email,string gift_message,string sender_mobile,string receiver_mobile)
        {
            var client = new RestClient("http://api.vastbazaar.com/api/GIftCard/Transaction_euro");
            var request = new RestRequest(Method.POST);
            request.AddHeader("authorization", "bearer " + token);
            request.AddHeader("content-type", "application/json");
            var data = new
            {
                optcode = optcode,
                agentid= agentid,
                amount= amount,
                sender_name= sender_name,
                sender_email= sender_email,
                receiver_name= receiver_name,
                receiver_email= receiver_email,
                gift_message= gift_message,
                sender_mobile= sender_mobile,
                receiver_mobile= receiver_mobile
            };
            var serializer = new JavaScriptSerializer();
            var json = serializer.Serialize(data);
            request.AddParameter("application/json", json, ParameterType.RequestBody);
            IRestResponse responseall = client.Execute(request);
            return responseall;
        }

        public IRestResponse GiftVoucherbalance(string cardnumber,string pin, string token)
        {
            var client = new RestClient("http://api.vastbazaar.com/api/GIftCard/Voucherbalance");
            var request = new RestRequest(Method.POST);
            request.AddHeader("authorization", "bearer " + token);
            request.AddHeader("content-type", "application/json");
            var data = new
            {
                cardnumber = cardnumber,
                pin= pin
            };
            var serializer = new JavaScriptSerializer();
            var json = serializer.Serialize(data);
            request.AddParameter("application/json", json, ParameterType.RequestBody);
            IRestResponse responseall = client.Execute(request);
            return responseall;
        }
        public IRestResponse GiftResendvoucher(string agentid, string token)
        {
            var client = new RestClient("http://api.vastbazaar.com/api/GIftCard/Resendvoucher");
            var request = new RestRequest(Method.POST);
            request.AddHeader("authorization", "bearer " + token);
            request.AddHeader("content-type", "application/json");
            var data = new
            {
                agentid = agentid,
          
            };
            var serializer = new JavaScriptSerializer();
            var json = serializer.Serialize(data);
            request.AddParameter("application/json", json, ParameterType.RequestBody);
            IRestResponse responseall = client.Execute(request);
            return responseall;
        }
        #endregion
        private static Random random = new Random();
        public string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}