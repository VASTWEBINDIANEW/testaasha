using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.WRetailer.Models
{
    public class Vastbillpay
    {
        public IRestResponse billpay(string token,string mobileno,string apioptcode,string Amount,string optional1,string optional2,string CommonTranid)
        {
            var client = new RestClient("http://api.vastbazaar.com/api/Electricity/Pay");
            var request = new RestRequest(Method.POST);
            request.AddHeader("authorization", "bearer " + token + "");
            request.AddHeader("content-type", "application/json");
            request.AddParameter("application/json", "{\r\n  \"Number\": \"" + mobileno + "\",\r\n  \"Operatorcode\":\"" + apioptcode + "\",\r\n  \"amount\":\"" + Amount + "\",\r\n  \"Account\":\"" + optional1 + "\",\r\n  \"optional1\":\"" + optional2 + "\",\r\n  \"RchId\":\"" + CommonTranid + "\",\r\n  \"Viewbill\":\"N\"\r\n}", ParameterType.RequestBody);
            IRestResponse responsecheck = client.Execute(request);
            return responsecheck;
        }
        public IRestResponse ViewBill(string token, string mobileno, string apioptcode, string Amount, string optional1, string optional2, string CommonTranid)
        {
            var client = new RestClient("http://api.vastbazaar.com/api/Electricity/Pay");
            var request = new RestRequest(Method.POST);
            request.AddHeader("authorization", "bearer " + token + "");
            request.AddHeader("content-type", "application/json");
            request.AddParameter("application/json", "{\r\n  \"Number\": \"" + mobileno + "\",\r\n  \"Operatorcode\":\"" + apioptcode + "\",\r\n  \"amount\":\"" + Amount + "\",\r\n  \"Account\":\"" + optional1 + "\",\r\n  \"optional1\":\"" + optional2 + "\",\r\n  \"RchId\":\"" + CommonTranid + "\",\r\n  \"Viewbill\":\"Y\"\r\n}", ParameterType.RequestBody);
            IRestResponse responsecheck = client.Execute(request);
            return responsecheck;
        }

    }
}