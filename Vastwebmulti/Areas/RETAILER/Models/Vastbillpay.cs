using java.net;
using Newtonsoft.Json;
using RestSharp;
using sun.misc;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.RETAILER.Models
{
    public class Vastbillpay
    {
        string VastbazaarBaseUrl = "http://api.vastbazaar.com/";
        public IRestResponse billpay(string token,string mobileno,string apioptcode,string Amount,string optional1,string optional2,string CommonTranid)
        {
            var client = new RestClient(VastbazaarBaseUrl+"api/Electricity/Pay");
         
            var request = new RestRequest(Method.POST);
            request.AddHeader("authorization", "bearer " + token + "");
            request.AddHeader("content-type", "application/json");
            request.AddParameter("application/json", "{\r\n  \"Number\": \"" + mobileno + "\",\r\n  \"Operatorcode\":\"" + apioptcode + "\",\r\n  \"amount\":\"" + Amount + "\",\r\n  \"Account\":\"" + optional1 + "\",\r\n  \"optional1\":\"" + optional2 + "\",\r\n  \"RchId\":\"" + CommonTranid + "\",\r\n  \"Viewbill\":\"N\"\r\n}", ParameterType.RequestBody);
            IRestResponse responsecheck = client.Execute(request);
            return responsecheck;
        }
        public IRestResponse Viewbill(string token, string mobileno, string apioptcode, string Amount, string optional1, string optional2, string CommonTranid)
        {
       
            var client = new RestClient(VastbazaarBaseUrl+"api/Electricity/Pay");
         
            var request = new RestRequest(Method.POST);
            request.AddHeader("authorization", "bearer " + token + "");
            request.AddHeader("content-type", "application/json");
            request.AddParameter("application/json", "{\r\n  \"Number\": \"" + mobileno + "\",\r\n  \"Operatorcode\":\"" + apioptcode + "\",\r\n  \"amount\":\"" + Amount + "\",\r\n  \"Account\":\"" + optional1 + "\",\r\n  \"optional1\":\"" + optional2 + "\",\r\n  \"RchId\":\"" + CommonTranid + "\",\r\n  \"Viewbill\":\"Y\"\r\n}", ParameterType.RequestBody);
            IRestResponse responsecheck = client.Execute(request);
            return responsecheck;
        }
     
        public IRestResponse WalletUnloadrRquest(string token, string Email, decimal Amount, string comment, string AccountNo, string Bankname,string Ifsccode,string Accountholdername,string RequestId,string Type)
        {
            var client = new RestClient(VastbazaarBaseUrl + "api/Unload/WalletUnloaduserNew");
            var request = new RestRequest(Method.POST);
            request.AddHeader("authorization", "bearer " + token + "");
            request.AddHeader("content-type", "application/json");
            request.AddParameter("application/json", "{\r\n  \"customerid\": \"" + Email + "\",\r\n  \"amount\":\"" + Amount + "\",\r\n  \"comment\":\"" + comment + "\",\r\n  \"customeraccountno\":\"" + AccountNo.Trim() + "\",\r\n  \"Type\":\"" + Type + "\",\r\n  \"customerbankname\":\"" + Bankname.Trim() + "\",\r\n  \"customerifsccode\":\"" + Ifsccode.Trim() + "\",\r\n  \"accountholdername\":\"" + Accountholdername.Trim() + "\",\r\n  \"Reqid\":\"" + RequestId + "\",\r\n}", ParameterType.RequestBody);
            IRestResponse responsecheck = client.Execute(request);
            return responsecheck;
        }
        public IRestResponse CreditCard(string token,string idno, string RetailerId, string Name,  string Cardnumber, string exp, string cvv,decimal Amount,string RequestId,string otp)
        {
            var client = new RestClient(VastbazaarBaseUrl + "api/CREDITCARD/PayBill");
            var request = new RestRequest(Method.POST);
            request.AddHeader("authorization", "bearer " + token + "");
            request.AddHeader("content-type", "application/json");
            var bodyinfo = new
            {
                Idno = idno,
                RetailerId,
                Name,
                Cardnumber,
                exp,
                cvv,
                Amount,
                RequestId,
                otp
            };
            var jsoninfo=JsonConvert.SerializeObject(bodyinfo);
           request.AddParameter("application/json", jsoninfo, ParameterType.RequestBody);
            IRestResponse responsecheck = client.Execute(request);
            return responsecheck;
        }
    }
}