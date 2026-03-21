using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;
using Vastwebmulti.Areas.ADMIN.Models;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.RETAILER.Models
{
    public class microATM
    {
        public string apipassword(string token)  //1
        {
            var client = new RestClient("http://api.vastbazaar.com/api/MicroATM/apipassword");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", "Bearer " + token);
            IRestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);

            return response.StatusCode.ToString() == "OK" ? response.Content : null;
        }

        public string changeapipassword(string token, string userid)  //2
        {
            var client = new RestClient("http://api.vastbazaar.com/api/MicroATM/Changeapipassword" + userid + "");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", "Bearer " + token);
            IRestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);

            return response.StatusCode.ToString() == "OK" ? response.Content : "";
        }

        public string merchantRequest(MerchantCreate mercr)         // 3
        {
            var client = new RestClient("http://api.vastbazaar.com/api/MicroATM/MerchantsCreate");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", "Bearer " + mercr.vbtoken);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", "{\r\n  \"Name\": \"" + mercr.Name.Trim() + "\",\r\n  \"aadhar_number\": \"" + mercr.aadhar_number.Trim() + "\",\r\n  \"BrandName\": \"" + mercr.BrandName.Trim() + "\",\r\n  \"Address\": \"" + HttpUtility.UrlEncode(mercr.Address.Trim()) + "\",\r\n  \"Pincode\": \"" + mercr.Pincode + "\",\r\n  \"PanCard\": \"" + mercr.PanCard + "\",\r\n  \"Mobile\": \"" + mercr.Mobile + "\",\r\n  \"Email\": \"" + mercr.Email + "\",\r\n  \"DOB\": \"" + mercr.DOB + "\",\r\n  \"Accountno\": \"" + mercr.Bankaccountno + "\",\r\n  \"Ifsccode\": \"" + mercr.Ifsccode + "\",\r\n  \"PanPath\": \"" + HttpUtility.UrlEncode(mercr.PanPath) + "\",\r\n  \"PanFileName\": \"" + HttpUtility.UrlEncode(mercr.PanFileName) + "\",\r\n  \"AadharPath\": \"" + HttpUtility.UrlEncode(mercr.AadharPath) + "\",\r\n  \"AadharFileName\": \"" + HttpUtility.UrlEncode(mercr.AadharFileName) + "\",\r\n  \"MerchantCode\": \"" + mercr.MerchantCode + "\",\r\n  \"CancelCheckpath\": \"" + HttpUtility.UrlEncode(mercr.CancelCheckpath) + "\",\r\n  \"CancelCheckFileName\": \"" + HttpUtility.UrlEncode(mercr.CancelCheckFileName) + "\"\r\n}", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            return response.StatusCode.ToString() == "OK" ? response.Content : null;
        }

        public string TerminalCreate(string merchantid, string location, string address, int pincode, string vbtoken)          //4
        {
            var client = new RestClient("http://api.vastbazaar.com/api/MicroATM/terminalsCreate");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", "Bearer " + vbtoken);
            //request.AddHeader("Authorization", "Bearer 1NWecleNYqVjWrwvXKROj_cBkrEgcHgUjbjYCTH_1uq-GCKx6Zs9NOSCkcuPFkht70EMGtGCnzovCkNbgKHI_HwWbq6mgY2dWnQF1rW4dzEIYDFQCU1x23UIXWwJDqlEEfrvNS42K3yX7vyz3pDJwUaNvBA6joebAuT1d5vwYNN9RW4UtOPVZBWIi4O4G4Cj3Q8ZqAGPVTAaOCAx8WjLVEagJJkDzd9aQHIiIbcg8aCCcltLbnXgvOfUINxeqnFcOSsU16XGRiUdTsgOBOK6gUaMGfTM7kUAge9fVNckq8NLvS29MLXbVcJRF02ws1Wtf7EwgLh6h08mpY7df9duHooPLho1mNqOZuEjq4fszawc4_lnwRGY3Wx8MMgb6xGWhf_Rk7mOCuGbMblgSFp_4eoCx9ddqa8RITJSfEfJ0vcTRjtir3ZumEpANWc8jaNGp039kAwM8uxOYIBmqUpChTGlLvQ_U9CiewS5bvc0GcfezmxG-wEKD-3l5mcmUyOul5ezvwSUgjLOfbBA0p_V8g");
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", "{\r\n  \"merchantid\": \"" + merchantid + "\",\r\n  \"location\": \"" + location + "\",\r\n  \"address\": \"" + HttpUtility.UrlEncode(address) + "\",\r\n  \"pincode\": \"" + pincode + "\"\r\n}", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            return response.StatusCode.ToString() == "OK" ? response.Content : null;
        }
        public string TerminalUpdate(string merchantid, string location, string address, int pincode, string vbtoken)          //4
        {
            var client = new RestClient("http://api.vastbazaar.com/api/MicroATM/terminalUpdate");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", "Bearer " + vbtoken);
            //request.AddHeader("Authorization", "Bearer 1NWecleNYqVjWrwvXKROj_cBkrEgcHgUjbjYCTH_1uq-GCKx6Zs9NOSCkcuPFkht70EMGtGCnzovCkNbgKHI_HwWbq6mgY2dWnQF1rW4dzEIYDFQCU1x23UIXWwJDqlEEfrvNS42K3yX7vyz3pDJwUaNvBA6joebAuT1d5vwYNN9RW4UtOPVZBWIi4O4G4Cj3Q8ZqAGPVTAaOCAx8WjLVEagJJkDzd9aQHIiIbcg8aCCcltLbnXgvOfUINxeqnFcOSsU16XGRiUdTsgOBOK6gUaMGfTM7kUAge9fVNckq8NLvS29MLXbVcJRF02ws1Wtf7EwgLh6h08mpY7df9duHooPLho1mNqOZuEjq4fszawc4_lnwRGY3Wx8MMgb6xGWhf_Rk7mOCuGbMblgSFp_4eoCx9ddqa8RITJSfEfJ0vcTRjtir3ZumEpANWc8jaNGp039kAwM8uxOYIBmqUpChTGlLvQ_U9CiewS5bvc0GcfezmxG-wEKD-3l5mcmUyOul5ezvwSUgjLOfbBA0p_V8g");
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", "{\r\n  \"merchantid\": \"" + merchantid + "\",\r\n  \"location\": \"" + location + "\",\r\n  \"address\": \"" + HttpUtility.UrlEncode(address) + "\",\r\n  \"pincode\": \"" + pincode + "\"\r\n}", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            return response.StatusCode.ToString() == "OK" ? response.Content : null;
        }

        public string MerchantUpdate(MerchantCreate mercr, string merchantid)         // 3
        {
            MicroatmLOG("**************************** MerchantUpdate ***************************");
            var client = new RestClient("http://api.vastbazaar.com/api/MicroATM/MerchantsUpdate");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", "Bearer " + mercr.vbtoken);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", "{\r\n  \"merchantid\": \"" + merchantid + "\",\r\n  \"Name\": \"" + mercr.Name.Trim() + "\",\r\n  \"aadhar_number\": \"" + mercr.aadhar_number.Trim() + "\",\r\n  \"BrandName\": \"" + mercr.BrandName.Trim() + "\",\r\n  \"Address\": \"" + HttpUtility.UrlEncode(mercr.Address.Trim()) + "\",\r\n  \"Pincode\": \"" + mercr.Pincode + "\",\r\n  \"PanCard\": \"" + mercr.PanCard + "\",\r\n  \"Mobile\": \"" + mercr.Mobile + "\",\r\n  \"Email\": \"" + mercr.Email + "\",\r\n  \"DOB\": \"" + mercr.DOB + "\",\r\n  \"Accountno\": \"" + mercr.Bankaccountno + "\",\r\n  \"Ifsccode\": \"" + mercr.Ifsccode + "\",\r\n  \"PanPath\": \"" + HttpUtility.UrlEncode(mercr.PanPath) + "\",\r\n  \"PanFileName\": \"" + HttpUtility.UrlEncode(mercr.PanFileName) + "\",\r\n  \"AadharPath\": \"" + HttpUtility.UrlEncode(mercr.AadharPath) + "\",\r\n  \"AadharFileName\": \"" + HttpUtility.UrlEncode(mercr.AadharFileName) + "\",\r\n  \"MerchantCode\": \"" + mercr.MerchantCode + "\",\r\n  \"CancelCheckpath\": \"" + HttpUtility.UrlEncode(mercr.CancelCheckpath) + "\",\r\n  \"CancelCheckFileName\": \"" + HttpUtility.UrlEncode(mercr.CancelCheckFileName) + "\"\r\n}", ParameterType.RequestBody);

            //          request.AddParameter("application/json", "{\r\n" +
            //"  \"Name\": \"" + mercr.Name + "\",\r\n" +
            //"  \"aadhar_number\": \"" + mercr.aadhar_number + "\",\r\n" + // Removed extra comma
            //"  \"BrandName\": \"" + mercr.BrandName + "\",\r\n" +
            //"  \"Address\": \"" + HttpUtility.UrlEncode(mercr.Address) + "\",\r\n" +
            //"  \"Pincode\": \"" + mercr.Pincode + "\",\r\n" +
            //"  \"PanCard\": \"" + mercr.PanCard + "\",\r\n" +
            //"  \"Mobile\": \"" + mercr.Mobile + "\",\r\n" +
            //"  \"Email\": \"" + mercr.Email + "\",\r\n" +
            //"  \"DOB\": \"" + mercr.DOB + "\",\r\n" +
            //"  \"PanPath\": \"" + HttpUtility.UrlEncode(mercr.PanPath) + "\",\r\n" +
            //"  \"PanFileName\": \"" + HttpUtility.UrlEncode(mercr.PanFileName) + "\",\r\n" +
            //"  \"AadharPath\": \"" + HttpUtility.UrlEncode(mercr.AadharPath) + "\",\r\n" +
            //"  \"AadharFileName\": \"" + HttpUtility.UrlEncode(mercr.AadharFileName) + "\",\r\n" +
            //"  \"MerchantCode\": \"" + mercr.MerchantCode + "\",\r\n" +
            //"  \"MerchantId\": \"" + merchantid + "\",\r\n" +
            //"  \"CancelCheckpath\": \"" + mercr.CancelCheckpath + "\",\r\n" +
            //"  \"CancelCheckFileName\": \"" + mercr.CancelCheckFileName + "\"\r\n" +
            //"}", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            MicroatmLOG("response Status Code " + response.StatusCode);
            return response.StatusCode.ToString() == "OK" ? response.Content : null;
        }


        public string TerminalSubmit(string merchantID, string vbtoken)     //5
        {
            var client = new RestClient("http://api.vastbazaar.com/api/MicroATM/terminalsubmit?terminalid=" + merchantID + "");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", "Bearer " + vbtoken);
            //request.AddHeader("Authorization", "Bearer 1NWecleNYqVjWrwvXKROj_cBkrEgcHgUjbjYCTH_1uq-GCKx6Zs9NOSCkcuPFkht70EMGtGCnzovCkNbgKHI_HwWbq6mgY2dWnQF1rW4dzEIYDFQCU1x23UIXWwJDqlEEfrvNS42K3yX7vyz3pDJwUaNvBA6joebAuT1d5vwYNN9RW4UtOPVZBWIi4O4G4Cj3Q8ZqAGPVTAaOCAx8WjLVEagJJkDzd9aQHIiIbcg8aCCcltLbnXgvOfUINxeqnFcOSsU16XGRiUdTsgOBOK6gUaMGfTM7kUAge9fVNckq8NLvS29MLXbVcJRF02ws1Wtf7EwgLh6h08mpY7df9duHooPLho1mNqOZuEjq4fszawc4_lnwRGY3Wx8MMgb6xGWhf_Rk7mOCuGbMblgSFp_4eoCx9ddqa8RITJSfEfJ0vcTRjtir3ZumEpANWc8jaNGp039kAwM8uxOYIBmqUpChTGlLvQ_U9CiewS5bvc0GcfezmxG-wEKD-3l5mcmUyOul5ezvwSUgjLOfbBA0p_V8g");
            IRestResponse response = client.Execute(request);

            return response.StatusCode.ToString() == "OK" ? response.Content : null;
        }

        public string MerchantSubmit(string merchantID, string vbtoken)     //5
        {
            var client = new RestClient("http://api.vastbazaar.com/api/MicroATM/MerchantsSubmit?ID=" + merchantID + "");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", "Bearer " + vbtoken);
            //request.AddHeader("Authorization", "Bearer 1NWecleNYqVjWrwvXKROj_cBkrEgcHgUjbjYCTH_1uq-GCKx6Zs9NOSCkcuPFkht70EMGtGCnzovCkNbgKHI_HwWbq6mgY2dWnQF1rW4dzEIYDFQCU1x23UIXWwJDqlEEfrvNS42K3yX7vyz3pDJwUaNvBA6joebAuT1d5vwYNN9RW4UtOPVZBWIi4O4G4Cj3Q8ZqAGPVTAaOCAx8WjLVEagJJkDzd9aQHIiIbcg8aCCcltLbnXgvOfUINxeqnFcOSsU16XGRiUdTsgOBOK6gUaMGfTM7kUAge9fVNckq8NLvS29MLXbVcJRF02ws1Wtf7EwgLh6h08mpY7df9duHooPLho1mNqOZuEjq4fszawc4_lnwRGY3Wx8MMgb6xGWhf_Rk7mOCuGbMblgSFp_4eoCx9ddqa8RITJSfEfJ0vcTRjtir3ZumEpANWc8jaNGp039kAwM8uxOYIBmqUpChTGlLvQ_U9CiewS5bvc0GcfezmxG-wEKD-3l5mcmUyOul5ezvwSUgjLOfbBA0p_V8g");
            IRestResponse response = client.Execute(request);

            return response.StatusCode.ToString() == "OK" ? response.Content : null;
        }
        public string MerchantStatus(string merchantID, string vbtoken)     //6
        {
            var client = new RestClient("http://api.vastbazaar.com/api/MicroATM/MerchantsStatus?Merchantid=" + merchantID + "");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", "Bearer " + vbtoken);
            //request.AddHeader("Authorization", "Bearer 1NWecleNYqVjWrwvXKROj_cBkrEgcHgUjbjYCTH_1uq-GCKx6Zs9NOSCkcuPFkht70EMGtGCnzovCkNbgKHI_HwWbq6mgY2dWnQF1rW4dzEIYDFQCU1x23UIXWwJDqlEEfrvNS42K3yX7vyz3pDJwUaNvBA6joebAuT1d5vwYNN9RW4UtOPVZBWIi4O4G4Cj3Q8ZqAGPVTAaOCAx8WjLVEagJJkDzd9aQHIiIbcg8aCCcltLbnXgvOfUINxeqnFcOSsU16XGRiUdTsgOBOK6gUaMGfTM7kUAge9fVNckq8NLvS29MLXbVcJRF02ws1Wtf7EwgLh6h08mpY7df9duHooPLho1mNqOZuEjq4fszawc4_lnwRGY3Wx8MMgb6xGWhf_Rk7mOCuGbMblgSFp_4eoCx9ddqa8RITJSfEfJ0vcTRjtir3ZumEpANWc8jaNGp039kAwM8uxOYIBmqUpChTGlLvQ_U9CiewS5bvc0GcfezmxG-wEKD-3l5mcmUyOul5ezvwSUgjLOfbBA0p_V8g");
            IRestResponse response = client.Execute(request);

            return response.StatusCode.ToString() == "OK" ? response.Content : null;
        }
        public string TerminalStatus(string TerminalID, string vbtoken)     //7
        {
            var client = new RestClient("http://api.vastbazaar.com/api/MicroATM/TerminalStatus?TerminalID=" + TerminalID + "");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", "Bearer " + vbtoken);
            //request.AddHeader("Authorization", "Bearer 1NWecleNYqVjWrwvXKROj_cBkrEgcHgUjbjYCTH_1uq-GCKx6Zs9NOSCkcuPFkht70EMGtGCnzovCkNbgKHI_HwWbq6mgY2dWnQF1rW4dzEIYDFQCU1x23UIXWwJDqlEEfrvNS42K3yX7vyz3pDJwUaNvBA6joebAuT1d5vwYNN9RW4UtOPVZBWIi4O4G4Cj3Q8ZqAGPVTAaOCAx8WjLVEagJJkDzd9aQHIiIbcg8aCCcltLbnXgvOfUINxeqnFcOSsU16XGRiUdTsgOBOK6gUaMGfTM7kUAge9fVNckq8NLvS29MLXbVcJRF02ws1Wtf7EwgLh6h08mpY7df9duHooPLho1mNqOZuEjq4fszawc4_lnwRGY3Wx8MMgb6xGWhf_Rk7mOCuGbMblgSFp_4eoCx9ddqa8RITJSfEfJ0vcTRjtir3ZumEpANWc8jaNGp039kAwM8uxOYIBmqUpChTGlLvQ_U9CiewS5bvc0GcfezmxG-wEKD-3l5mcmUyOul5ezvwSUgjLOfbBA0p_V8g");
            IRestResponse response = client.Execute(request);

            return response.StatusCode.ToString() == "OK" ? response.Content : null;
        }
        public string ActivateDevice(string MerchantID, string deviceSnNo, string vbToken)  //8
        {
            var client = new RestClient("http://api.vastbazaar.com/api/MicroATM/Activatedevice?ID=" + MerchantID + "&devicesnno=" + deviceSnNo + "");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", "Bearer " + vbToken);
            //request.AddHeader("Authorization", "Bearer 1NWecleNYqVjWrwvXKROj_cBkrEgcHgUjbjYCTH_1uq-GCKx6Zs9NOSCkcuPFkht70EMGtGCnzovCkNbgKHI_HwWbq6mgY2dWnQF1rW4dzEIYDFQCU1x23UIXWwJDqlEEfrvNS42K3yX7vyz3pDJwUaNvBA6joebAuT1d5vwYNN9RW4UtOPVZBWIi4O4G4Cj3Q8ZqAGPVTAaOCAx8WjLVEagJJkDzd9aQHIiIbcg8aCCcltLbnXgvOfUINxeqnFcOSsU16XGRiUdTsgOBOK6gUaMGfTM7kUAge9fVNckq8NLvS29MLXbVcJRF02ws1Wtf7EwgLh6h08mpY7df9duHooPLho1mNqOZuEjq4fszawc4_lnwRGY3Wx8MMgb6xGWhf_Rk7mOCuGbMblgSFp_4eoCx9ddqa8RITJSfEfJ0vcTRjtir3ZumEpANWc8jaNGp039kAwM8uxOYIBmqUpChTGlLvQ_U9CiewS5bvc0GcfezmxG-wEKD-3l5mcmUyOul5ezvwSUgjLOfbBA0p_V8g");
            IRestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);

            return response.StatusCode.ToString() == "OK" ? response.Content : null;
        }

        public static string GetCurrentUrl()
        {
            string urlss = "";
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                urlss = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);//db.Admin_details.SingleOrDefault().WebsiteUrl;
            }

            //string ApiUrl = HttpContext.Current.Request.IsLocal ? "https://www.rechargepartners.in/" : HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            //ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            //HttpWebRequest WebRequestObjectTarget = (HttpWebRequest)HttpWebRequest.Create(ApiUrl);
            //WebRequestObjectTarget.Timeout = (System.Int32)TimeSpan.FromSeconds(250).TotalMilliseconds;
            //WebResponse Response = WebRequestObjectTarget.GetResponse();
            //string ExactUrl = Response.ResponseUri.Authority;

            return HttpContext.Current.Request.IsLocal ? urlss : HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
        }
        public static void MicroatmLOG(string strMessage)
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                try
                {
                    string name = db.Admin_details.SingleOrDefault().WebsiteUrl;
                    StreamWriter log;
                    FileStream fileStream = null;
                    DirectoryInfo logDirInfo = null;
                    FileInfo logFileInfo;
                    string logFilePath = "C:\\Logs\\";
                    logFilePath = logFilePath + "MIcro ATM LOG-" + name + " -" + DateTime.Today.ToString("MM-dd-yyyy") + "." + "txt";
                    logFileInfo = new FileInfo(logFilePath);
                    logDirInfo = new DirectoryInfo(logFileInfo.DirectoryName);
                    if (!logDirInfo.Exists) logDirInfo.Create();
                    if (!logFileInfo.Exists)
                    {
                        fileStream = logFileInfo.Create();
                    }
                    else
                    {
                        fileStream = new FileStream(logFilePath, FileMode.Append);
                    }
                    log = new StreamWriter(fileStream);
                    log.WriteLine(strMessage);
                    log.Close();
                }
                catch (Exception ex)
                { }
            }
        }
    }

    public class MerchantCreate
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string BrandName { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public double Pincode { get; set; }
        [Required]
        public string PanCard { get; set; }
        [Required]
        public string Mobile { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Bankaccountno { get; set; }
        [Required]
        public string Ifsccode { get; set; }
        [Required]
        public string DOB { get; set; }
        [Required]
        public string PanPath { get; set; }
        [Required]
        public string PanFileName { get; set; }
        [Required]
        public string AadharPath { get; set; }
        [Required]
        public string AadharFileName { get; set; }
        [Required]
        public string CancelCheckpath { get; set; }
        [Required]
        public string CancelCheckFileName { get; set; }
        [Required]
        public string vbtoken { get; set; }
        [Required]
        public string MerchantCode { get; set; }
        public string aadhar_number { get; set; }

    }

}