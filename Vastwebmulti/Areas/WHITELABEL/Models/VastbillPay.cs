using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.WHITELABEL.Models
{
    
    public class VastbillPay
    {
        string VastbazaarBaseUrl = "http://api.vastbazaar.com/";
        public IRestResponse WalletUnloadrRquest(string token, string Email, decimal Amount, string comment, string AccountNo, string Bankname, string Ifsccode, string Accountholdername, string RequestId)
        {
            var client = new RestClient(VastbazaarBaseUrl + "api/Unload/WalletUnloaduser");
            var request = new RestRequest(Method.POST);
            request.AddHeader("authorization", "bearer " + token + "");
            request.AddHeader("content-type", "application/json");
            request.AddParameter("application/json", "{\r\n  \"customerid\": \"" + Email + "\",\r\n  \"amount\":\"" + Amount + "\",\r\n  \"comment\":\"" + comment + "\",\r\n  \"customeraccountno\":\"" + AccountNo + "\",\r\n  \"customerbankname\":\"" + Bankname + "\",\r\n  \"customerifsccode\":\"" + Ifsccode + "\",\r\n  \"accountholdername\":\"" + Accountholdername + "\",\r\n  \"Reqid\":\"" + RequestId + "\",\r\n}", ParameterType.RequestBody);
            IRestResponse responsecheck = client.Execute(request);
            return responsecheck;
        }
    }
}