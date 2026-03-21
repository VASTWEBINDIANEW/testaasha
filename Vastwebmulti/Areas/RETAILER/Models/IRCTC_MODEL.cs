using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.RETAILER.Models
{
    public class IRCTC_MODEL
    {


        public string IRCTC_Request(IRCTC_Info_Create IRCT)         // 3
        {
            
            var client = new RestClient("http://api.vastbazaar.com/api/IRCTC/UserPurchaseToken");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", "Bearer " + IRCT.vbtoken);
            request.AddHeader("Content-Type", "application/json");
        
            var req = new
            {
            retailerid = IRCT.retailerid,
            retaileremail = IRCT.retaileremail,
            FirstName = IRCT.FirstName,
            LastName = IRCT.LastName,
            Mobile = IRCT.Mobile,
            Email = IRCT.Email,
            secondarymobile = IRCT.secondarymobile,
            Address = IRCT.Address,
            state = IRCT.state,
            dictrict = IRCT.dictrict,
            agentfirmname = IRCT.agentfirmname,
            pancardno = IRCT.pancardno,
            agentid = IRCT.agentid,
            pancardcopy = IRCT.pancardcopy,//----path
            businesstype = IRCT.businesstype,
            gstno = IRCT.gstno,
            gstcopy = IRCT.gstcopy, //----path
            vbtoken = IRCT.vbtoken,
            PinCode = IRCT.PinCode,
                      
            };

            var reqq = JsonConvert.SerializeObject(req);

            request.AddParameter("application/json", reqq, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            // request.AddBody(databods, "application/json");
            //  request.AddParameter("application/json", "{\r\n  \"Name\": \"" + mercr.Name.Trim() + "\",\r\n  \"BrandName\": \"" + mercr.BrandName.Trim() + "\",\r\n  \"Address\": \"" + HttpUtility.UrlEncode(mercr.Address.Trim()) + "\",\r\n  \"Pincode\": \"" + mercr.Pincode + "\",\r\n  \"PanCard\": \"" + mercr.PanCard + "\",\r\n  \"Mobile\": \"" + mercr.Mobile + "\",\r\n  \"Email\": \"" + mercr.Email + "\",\r\n  \"DOB\": \"" + mercr.DOB + "\",\r\n  \"PanPath\": \"" + HttpUtility.UrlEncode(mercr.PanPath) + "\",\r\n  \"PanFileName\": \"" + HttpUtility.UrlEncode(mercr.PanFileName) + "\",\r\n  \"AadharPath\": \"" + HttpUtility.UrlEncode(mercr.AadharPath) + "\",\r\n  \"AadharFileName\": \"" + HttpUtility.UrlEncode(mercr.AadharFileName) + "\",\r\n  \"MerchantCode\": \"" + mercr.MerchantCode + "\"\r\n}", ParameterType.RequestBody);
           // IRestResponse response = client.Execute(request);

            return response.StatusCode.ToString() == "OK" ? response.Content : null;
        }


        public string IRCTC_Request_Status_check(string txnids,string vbtoken)         // 3
        {

            var client = new RestClient("http://api.vastbazaar.com/api/IRCTC/StatusPurchaseToken");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
          
            request.AddHeader("Authorization", "Bearer " + vbtoken);
            request.AddQueryParameter("agentid", txnids);
            request.AddHeader("Content-Type", "application/json");

           

           // var reqq = JsonConvert.SerializeObject(req);

          //  request.AddParameter("application/json", reqq, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            // request.AddBody(databods, "application/json");
            //  request.AddParameter("application/json", "{\r\n  \"Name\": \"" + mercr.Name.Trim() + "\",\r\n  \"BrandName\": \"" + mercr.BrandName.Trim() + "\",\r\n  \"Address\": \"" + HttpUtility.UrlEncode(mercr.Address.Trim()) + "\",\r\n  \"Pincode\": \"" + mercr.Pincode + "\",\r\n  \"PanCard\": \"" + mercr.PanCard + "\",\r\n  \"Mobile\": \"" + mercr.Mobile + "\",\r\n  \"Email\": \"" + mercr.Email + "\",\r\n  \"DOB\": \"" + mercr.DOB + "\",\r\n  \"PanPath\": \"" + HttpUtility.UrlEncode(mercr.PanPath) + "\",\r\n  \"PanFileName\": \"" + HttpUtility.UrlEncode(mercr.PanFileName) + "\",\r\n  \"AadharPath\": \"" + HttpUtility.UrlEncode(mercr.AadharPath) + "\",\r\n  \"AadharFileName\": \"" + HttpUtility.UrlEncode(mercr.AadharFileName) + "\",\r\n  \"MerchantCode\": \"" + mercr.MerchantCode + "\"\r\n}", ParameterType.RequestBody);
            // IRestResponse response = client.Execute(request);

            return response.StatusCode.ToString() == "OK" ? response.Content : null;
        }


        public static string GetCurrentUrl()
        {
            string urlss = "";
          
                urlss = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);//db.Admin_details.SingleOrDefault().WebsiteUrl;
          

            //string ApiUrl = HttpContext.Current.Request.IsLocal ? "https://www.rechargepartners.in/" : HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            //ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            //HttpWebRequest WebRequestObjectTarget = (HttpWebRequest)HttpWebRequest.Create(ApiUrl);
            //WebRequestObjectTarget.Timeout = (System.Int32)TimeSpan.FromSeconds(250).TotalMilliseconds;
            //WebResponse Response = WebRequestObjectTarget.GetResponse();
            //string ExactUrl = Response.ResponseUri.Authority;

            return HttpContext.Current.Request.IsLocal ? urlss : HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
        }

    }

    public class IRCTC_Info_Create
    {
        [Required]
        public string retailerid { get; set; }
        [Required]
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Required]
        public string agentfirmname { get; set; } 
        [Required]
        public string state { get; set; }
        [Required]
        public string dictrict { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string agentaddress { get; set; }
        [Required]
        public int PinCode { get; set; }
        [Required]
        public string pancardno { get; set; }
        [Required]
        public string Mobile { get; set; }
        [Required]
        public string secondarymobile { get; set; }
        [Required]
        public string retaileremail { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string DOB { get; set; }
        [Required]
        public string pancardcopy { get; set; }
        [Required]
        public string PanFileName { get; set; }
       
        [Required]
        public string vbtoken { get; set; } 
        [Required]
        public string businesstype { get; set; }
       
        public string gstno { get; set; }
      
        public string gstcopy { get; set; }

        [Required]
        public string agentid { get; set; }

    }
}