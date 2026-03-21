using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace Vastwebmulti.Areas.WHITELABEL.Models
{
    public class VastBazaar
    {
        public IRestResponse MposStatus(string message, string transType, string PartnerId, string token)
        {
            var client = new RestClient("http://api.vastbazaar.com/api/DMT1/Bank_details");
            var request = new RestRequest(Method.POST);
            request.AddHeader("authorization", "bearer " + token);
            request.AddHeader("content-type", "application/json");
            var data = new
            {
                Message = message,
                transType = transType,
                PartnerId = PartnerId
            };

            var serializer = new JavaScriptSerializer();
            var json = serializer.Serialize(data);
            request.AddParameter("application/json", json, ParameterType.RequestBody);
            IRestResponse responseall = client.Execute(request);
            return responseall;
        }
    }
}