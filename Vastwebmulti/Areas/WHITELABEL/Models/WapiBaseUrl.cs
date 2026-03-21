using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.WHITELABEL.Models
{
    public class WapiBaseUrl
    {
        public static string GetBaseUrl()
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                //string ApiUrl = HttpContext.Current.Request.IsLocal ? "https://www.aashadigitalindia.co.in/" : HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
                //ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                //HttpWebRequest WebRequestObjectTarget = (HttpWebRequest)HttpWebRequest.Create(ApiUrl);
                //WebRequestObjectTarget.Timeout = (System.Int32)TimeSpan.FromSeconds(250).TotalMilliseconds;
                //WebResponse Response = WebRequestObjectTarget.GetResponse();
                //string ExactUrl = Response.ResponseUri.Authority;
                //string baseUrl = ExactUrl.Contains("www") ? ExactUrl.Replace("www", "http://wapi") : "http://wapi" + ExactUrl.Replace("w1", "").Replace("w2", "");

                string WebsiteUrl = db.Admin_details.SingleOrDefault().WebsiteUrl;
                string ExactUrl = WebsiteUrl.Replace("https://www.", "").Replace("http://www.", "").Replace("https://", "").Replace("http://", "").Replace("test.", "");
                string baseUrl = HttpContext.Current.Request.IsLocal ? "http://wapi.vastwebindia.com/" : "http://white." + ExactUrl;
                baseUrl = "http://white." + ExactUrl;

                return baseUrl;
            }
        }
    }
}