using RestSharp;
using System;
using System.Linq;
using System.Net;

namespace Vastwebmulti.Models
{
    public class InstantPay_BBPS
    {
        public IRestResponse bill_pay(string token, string apioptcode, string agentid, string customer_mobile, string customer_KNO, string optional1, string optional2, string outletid, decimal amount, string Billrefid)
        {
            var client = new RestClient("https://www.instantpay.in/ws/bbps/bill_pay");
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            if (apioptcode == "DGE")
            {
                var PaymentInfo = "WALLET|" + customer_mobile;
                request.AddParameter("application/json", "{\n\"token\": \"" + token + "\",\n\"request\": {\n\"sp_key\": \"" + apioptcode + "\",\n\"agentid\": \"" + agentid + "\",\n\"customer_mobile\": \"" + customer_mobile + "\",\n\"customer_params\": [\n\"" + customer_KNO + "\",\r\n      \"" + optional1 + "\",\r\n      \"" + optional2 + "\"\r\n    ],\n\"init_channel\": \"AGT\",\n\"endpoint_ip\": \"123.258.255.12\",\n\"mac\": \"AD-fg-12-78-GH\",\n\"payment_mode\": \"WALLET\",\n\"payment_info\": \"" + PaymentInfo + "\",\n\"amount\": \"" + amount + "\",\n\"reference_id\": \"" + Billrefid + "\",\n\"latlong\": \"24.1568,78.5263\",\n\"outletid\": \"" + outletid + "\"\n}\n}", ParameterType.RequestBody);
            }
            else
            {
                request.AddParameter("application/json", "{\n\"token\": \"" + token + "\",\n\"request\": {\n\"sp_key\": \"" + apioptcode + "\",\n\"agentid\": \"" + agentid + "\",\n\"customer_mobile\": \"" + customer_mobile + "\",\n\"customer_params\": [\n\"" + customer_KNO + "\",\r\n      \"" + optional1 + "\",\r\n      \"" + optional2 + "\"\r\n    ],\n\"init_channel\": \"AGT\",\n\"endpoint_ip\": \"123.258.255.12\",\n\"mac\": \"AD-fg-12-78-GH\",\n\"payment_mode\": \"Cash\",\n\"payment_info\": \"bill\",\n\"amount\": \"" + amount + "\",\n\"reference_id\": \"" + Billrefid + "\",\n\"latlong\": \"24.1568,78.5263\",\n\"outletid\": \"" + outletid + "\"\n}\n}", ParameterType.RequestBody);
            }
            IRestResponse responsecheck = client.Execute(request);
            return responsecheck;
        }
        public IRestResponse bill_fetch(string token, string apioptcode, string agentid, string customer_mobile, string customer_KNO, string optional1, string optional2, string outletid, decimal amount, string Billrefid)
        {

            var client = new RestClient("https://www.instantpay.in/ws/bbps/bill_fetch");
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", "{\n\"token\": \"" + token + "\",\n\"request\": {\n\"sp_key\": \"" + apioptcode + "\",\n\"agentid\": \"" + agentid + "\",\n\"customer_mobile\": \"" + customer_mobile + "\",\n\"customer_params\": [\n\"" + customer_KNO + "\",\r\n      \"" + optional1 + "\",\r\n      \"" + optional2 + "\"\r\n    ],\n\"init_channel\": \"AGT\",\n\"endpoint_ip\": \"123.258.255.12\",\n\"mac\": \"AD-fg-12-78-GH\",\n\"payment_mode\": \"Cash\",\n\"payment_info\": \"bill\",\n\"amount\": \"" + amount + "\",\n\"reference_id\": \"" + Billrefid + "\",\n\"latlong\": \"24.1568,78.5263\",\n\"outletid\": \"" + outletid + "\"\n}\n}", ParameterType.RequestBody);
            IRestResponse responsecheck = client.Execute(request);
            return responsecheck;
        }


        public IRestResponse RegisterOutlet(string token, string RetailerId, string Mobile, string OTP, string email, string store_type, string company, string name, string pincode, string address, string PanCard)
        {
            var client = new RestClient("https://www.instantpay.in/ws/outlet/registration");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", "{\t\"token\" : \"" + token + "\",\r\n\t\"request\" : {\r\n\t\t\"mobile\" : \"" + Mobile + "\",\r\n\t\t\"email\" : \"" + email + "\",\r\n\t\t\"company\" : \"" + company + "\",\r\n\t\t\"name\" : \"" + name + "\",\r\n\t\t\"pan\" : \"" + PanCard + "\",\r\n\t\t\"pincode\" : \"" + pincode + "\",\r\n\t\t\"address\" : \"" + address + "\",\r\n\t\t\"otp\" : \"" + OTP + "\"\r\n}\r\n}", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            return response;
        }


        private static Random random = new Random();
        public string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        //static readonly string PasswordHash = "P@@Sw0rd";
        //static readonly string SaltKey = "S@LT&KEY";
        //static readonly string VIKey = "@1B2c3D4e5F6g7H8";
    }
}