using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Xml;

namespace Vastwebmulti.Areas.RETAILER.Models
{
    public class mobikwikRecharge
    {
        static string endurl = "https://alpha3.mobikwik.com";
        static string Userid = "testalpha1@gmail.com";
        static string Password = "testalpha1@123";
        static string secretKey = "abcd@123";
        public IRestResponse Viewbill(string mobileno, string apioptcode,  string optional1, string optional2)
        {

            var client = new RestClient(endurl + "/retailer/v2/retailerViewbill");
            var request = new RestRequest(Method.POST);
            request.AddHeader("X-MClient", "14");
            request.AddHeader("Content-Type", "application/json");
            var req = new
            {
                uid = Userid,
                pswd = Password,
                cir = "",
                cn = mobileno,
                op = apioptcode,
                adParams = new { }
            };

            var respchk = JsonConvert.SerializeObject(req);
            request.AddParameter("application/json", respchk, ParameterType.RequestBody);
            IRestResponse responsecheck = client.Execute(request);
            return responsecheck;
        }
        public IRestResponse BillValidate( string mobileno, string apioptcode, string Amount, string optional1, string optional2,string CommonTranid)
        {
            var client = new RestClient(endurl + "/recharge/v1/retailerValidation");
            var request = new RestRequest(Method.POST);
            var req = new
            {
                uid = Userid,
                password = Password,
                amt = Amount,
                cir = "",
                cn = mobileno,
                op = apioptcode,
                adParams = new { }
            };
            var respchk = JsonConvert.SerializeObject(req);
            var checksum = GenerateHmacSha256(respchk, secretKey);
            client.Timeout = -1;
            request.AddHeader("X-MClient", "14");
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("checkSum", checksum);
            request.AddParameter("application/json", respchk, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            return response;
        }
        public IRestResponse BillPayment(string mobileno, string apioptcode, string Amount, string optional1, string optional2, string uniqueid)
        {
            endurl = endurl + "/recharge.do?uid=" + Userid + "&pwd=" + Password + "&cn=" + mobileno + "&op=" + apioptcode + "&cir=&amt=" + Amount + "&reqid=" + uniqueid + "&pvalue=&ad1=&ad2=&ad3=&ad4=&ad5=&ad6=";
            var request = new RestRequest(Method.GET);
            var client = new RestClient(endurl);
            client.Timeout = -1;
            IRestResponse response = client.Execute(request);
            return response;
        }
        static string GenerateHmacSha256(string message, string secret)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(secret);
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);

            using (var hmac = new HMACSHA256(keyBytes))
            {
                byte[] hashMessage = hmac.ComputeHash(messageBytes);
                return Convert.ToBase64String(hashMessage);
            }
        }
    }
}