using Newtonsoft.Json;
using RestSharp;
using System;
using System.Linq;
using System.Net;
using Vastwebmulti.Models;
using System.IO;
using sun.net.idn;
using sun.security.krb5.@internal;
using com.sun.security.ntlm;
using Google.Apis.Auth.OAuth2;
using java.time;

namespace Vastwebmulti.Areas.RETAILER.Models
{
    public class Radiantdmt
    {
        public void Token(out string accesstoken, out string agentID, string clientId, string ClientSecret, string apiKey, string userid, string password)
        {
            WriteLog("Token  *******************");
            accesstoken = ""; agentID = "";
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                var radiantauthinfo = db.radiantauths.SingleOrDefault();
                if (radiantauthinfo != null)
                {
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    string Baseurl = "https://aceneobank.com/apiService/";
                    var client = new RestClient(Baseurl + "apiLogin");
                    var request = new RestRequest(Method.POST);
                    //request.AlwaysMultipartFormData = true;
                    request.AddHeader("clientId", clientId);
                    request.AddHeader("ClientSecret", ClientSecret);
                    //request.AddHeader("distributorCode", "70359687666670616055318500299916_Si6zudoNA+/VffLEesneSA==");
                    request.AddHeader("apiKey", apiKey);
                    //request.AddParameter("agentID", userid);
                    //request.AddParameter("agentSecret", password);
                    var body = new
                    {
                        agentID = userid,
                        agentSecret = password

                    };
                    var jsonbody = JsonConvert.SerializeObject(body);
                    WriteLog("Request " + jsonbody);
                    request.AddParameter("application/json", jsonbody, ParameterType.RequestBody);


                    IRestResponse responsecheck = client.Execute(request);

                    try
                    {
                        WriteLog("API URL " + Baseurl + "apiLogin");
                        WriteLog("clientId " + clientId);
                        WriteLog("ClientSecret " + ClientSecret);
                        WriteLog("apiKey " + apiKey);
                        WriteLog("agentID " + userid);
                        WriteLog("agentSecret " + password);


                    }
                    catch { }


                    WriteLog("Token Api Response Code " + responsecheck.StatusCode);
                    try
                    {
                        WriteLog("Token Api Response " + responsecheck.Content);
                    }
                    catch { }
                    if (responsecheck.StatusCode == HttpStatusCode.OK)
                    {
                        var outputresp = responsecheck.Content;
                        WriteLog("Token Api Response " + outputresp);

                        dynamic respchk = JsonConvert.DeserializeObject<dynamic>(outputresp);
                        if (respchk.success == true)
                        {
                            accesstoken = respchk.accessToken;
                            agentID = respchk.agentID;
                            var checktokeninfo = db.radianttokens.SingleOrDefault();
                            if (checktokeninfo != null)
                            {
                                checktokeninfo.accessToken = accesstoken;
                                checktokeninfo.agentID = agentID;
                                db.SaveChanges();
                            }
                            else
                            {

                                radianttoken info = new radianttoken();
                                info.agentID = agentID;
                                info.accessToken = accesstoken;
                                db.radianttokens.Add(info);
                                db.SaveChanges();
                            }
                        }
                    }
                }
            }
        }
        //DMT
        public IRestResponse Getcustomer(string agentid, string sendernumber, string token, string clientId, string ClientSecret, string apiKey)
        {
            WriteLog("************************ Getcustomer ************************************");
            WriteLog("sendernumber " + sendernumber);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string Baseurl = "https://aceneobank.com/apiService/";

            WriteLog("Request " + Baseurl + "V4/dmt/get-customer");
            var client = new RestClient(Baseurl + "V4/dmt/get-customer");
            var request = new RestRequest(Method.POST);
            request.AddHeader("X-Root-id", agentid);
            request.AddHeader("clientId", clientId);
            request.AddHeader("ClientSecret", ClientSecret);
            request.AddHeader("apiKey", apiKey);
            request.AddHeader("Authorization", "Bearer " + token + "");
            var body = new
            {
                agentid,
                CustomerMobileNo = sendernumber,
                dmt_type=2
            };
            var reqbody = JsonConvert.SerializeObject(body);
            WriteLog("Request " + reqbody);
            request.AddParameter("application/json", reqbody, ParameterType.RequestBody);
            IRestResponse responsecheck = client.Execute(request);
            try
            {
                WriteLog("Response Code " + responsecheck.StatusCode);
                WriteLog("Response " + responsecheck.Content);
            }
            catch { }
            return responsecheck;
        }
        ////////
        public IRestResponse GetLimit(string agentid, string sendernumber, string token, string clientId, string ClientSecret, string apiKey)
        {
            WriteLog("************************ GetLimit ************************************");
            WriteLog("sendernumber " + sendernumber);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string Baseurl = "https://aceneobank.com/apiService/";

            WriteLog("Request " + Baseurl + "V4/dmt/customer-limit");
            var client = new RestClient(Baseurl + "V4/dmt/customer-limit");
            var request = new RestRequest(Method.POST);
            request.AddHeader("X-Root-id", agentid);
            request.AddHeader("clientId", clientId);
            request.AddHeader("ClientSecret", ClientSecret);
            request.AddHeader("apiKey", apiKey);
            request.AddHeader("Authorization", "Bearer " + token + "");
            var body = new
            {
                agentid,
                CustomerMobileNo = sendernumber,
                dmt_type = 2
            };
            var reqbody = JsonConvert.SerializeObject(body);
            WriteLog("Request " + reqbody);
            request.AddParameter("application/json", reqbody, ParameterType.RequestBody);
            IRestResponse responsecheck = client.Execute(request);
            try
            {
                WriteLog("Response Code " + responsecheck.StatusCode);
                WriteLog("Response " + responsecheck.Content);
            }
            catch { }
            return responsecheck;
        }


        public IRestResponse KYCRegister(string agentid, string token, string sendernumber, string AadharNo,string Pid,string Latitude,string longitude,string PublicIP, string clientId, string ClientSecret, string apiKey)
        {
            WriteLog("************************ KYCRegister ************************************");
            WriteLog("sendernumber " + sendernumber);
            WriteLog("sendernumber " + sendernumber);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string Baseurl = "https://aceneobank.com/apiService/";
            var client = new RestClient(Baseurl + "V3/dmt/ekyc");
            var request = new RestRequest(Method.POST);
            request.AddHeader("X-Root-id", agentid);
            request.AddHeader("clientId", clientId);
            request.AddHeader("ClientSecret", ClientSecret);
            //request.AddHeader("distributorCode", "70359687666670616055318500299916_Si6zudoNA+/VffLEesneSA==");
            request.AddHeader("apiKey", apiKey);
            request.AddHeader("Authorization", "Bearer " + token + "");
            //request.AlwaysMultipartFormData = true;
            var body = new
            {
                sender_no = sendernumber,
                AadharNo ,
                Pid,
                Latitude,
                longitude,
                PublicIP
            };
            var jsonbody = JsonConvert.SerializeObject(body);
            WriteLog("Request " + jsonbody);
            request.AddParameter("application/json", jsonbody, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            try
            {
                WriteLog("Response Code " + response.StatusCode);
                WriteLog("Response " + response.Content);
            }
            catch { }
            return response;
        }
    
        public IRestResponse CustomerKYC(string agentid, string sendernumber, string token, string clientId, string ClientSecret, string apiKey,string CustomerName,string AadharNo,string Pid,string Latitude,string longitude,string PublicIP)
        {
            WriteLog("************************ CustomerKYC ************************************");
            WriteLog("sendernumber " + sendernumber);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string Baseurl = "https://aceneobank.com/apiService/";

            WriteLog("Request " + Baseurl + "V4/dmt/processCustomerEKYC");
            var client = new RestClient(Baseurl + "V4/dmt/processCustomerEKYC");
            var request = new RestRequest(Method.POST);
            request.AddHeader("X-Root-id", agentid);
            request.AddHeader("clientId", clientId);
            request.AddHeader("ClientSecret", ClientSecret);
            request.AddHeader("apiKey", apiKey);
            request.AddHeader("Authorization", "Bearer " + token + "");
            var body = new
            {
                agentid,
                CustomerMobileNo = sendernumber,
                CustomerName,
                AadharNo,
                Pid,
                dmt_type=2
            };
            var reqbody = JsonConvert.SerializeObject(body);
            WriteLog("Request " + reqbody);
            request.AddParameter("application/json", reqbody, ParameterType.RequestBody);
            IRestResponse responsecheck = client.Execute(request);
            try
            {
                WriteLog("Response Code " + responsecheck.StatusCode);
                WriteLog("Response " + responsecheck.Content);
            }
            catch { }
            return responsecheck;
        }
        public IRestResponse processCustomerEKYC(string agentid, string sendernumber, string token, string clientId, string ClientSecret, string apiKey, string CustomerName,  string Latitude, string longitude, string PublicIP)
        {
            WriteLog("************************ processCustomerEKYC ************************************");
            WriteLog("sendernumber " + sendernumber);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string Baseurl = "https://aceneobank.com/apiService/";

            WriteLog("Request " + Baseurl + "V4/dmt/processCustomerEKYC");
            var client = new RestClient(Baseurl + "V4/dmt/processCustomerEKYC");
            var request = new RestRequest(Method.POST);
            request.AddHeader("X-Root-id", agentid);
            request.AddHeader("clientId", clientId);
            request.AddHeader("ClientSecret", ClientSecret);
            request.AddHeader("apiKey", apiKey);
            request.AddHeader("Authorization", "Bearer " + token + "");
            var body = new
            {
                agentid,
                CustomerMobileNo = sendernumber,
                CustomerName,
                Latitude,
                longitude,
                PublicIP
            };
            var reqbody = JsonConvert.SerializeObject(body);
            WriteLog("Request " + reqbody);
            request.AddParameter("application/json", reqbody, ParameterType.RequestBody);
            IRestResponse responsecheck = client.Execute(request);
            try
            {
                WriteLog("Response Code " + responsecheck.StatusCode);
                WriteLog("Response " + responsecheck.Content);
            }
            catch { }
            return responsecheck;
        }
        public IRestResponse CreateCustomer(string agentid, string sendernumber, string token, string clientId, string ClientSecret, string apiKey, string CustomerName, string Latitude, string longitude, string PublicIP,string KYCRequestId,string OTPRequestId,string OTPPin)
        {
            WriteLog("************************ CreateCustomer ************************************");
          //WriteLog("sendernumber " + sendernumber);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string Baseurl = "https://aceneobank.com/apiService/";
            WriteLog("Request " + Baseurl + "V4/dmt/create-customer");
            var client = new RestClient(Baseurl + "V4/dmt/create-customer");
            var request = new RestRequest(Method.POST);
            request.AddHeader("X-Root-id", agentid);
            request.AddHeader("clientId", clientId);
            request.AddHeader("ClientSecret", ClientSecret);
            request.AddHeader("apiKey", apiKey);
            request.AddHeader("Authorization", "Bearer " + token + "");
            var body = new
            {
                agentid,
                CustomerMobileNo = sendernumber,
                CustomerName,
                KYCRequestId,
                OTPRequestId,
                OTPPin,
                dmt_type=2,
                address="Delhi",
                pincode="110011"
            };
            var reqbody = JsonConvert.SerializeObject(body);
            WriteLog("Request " + reqbody);
            request.AddParameter("application/json", reqbody, ParameterType.RequestBody);
            IRestResponse responsecheck = client.Execute(request);
            try
            {
                WriteLog("Response Code " + responsecheck.StatusCode);
                WriteLog("Response " + responsecheck.Content);
            }
            catch { }
            return responsecheck;
        }
     
        public IRestResponse Getbenificry(string agentid, string sendernumber, string token, string clientId, string ClientSecret, string apiKey)
        {
            WriteLog("************************ Getbenificry ************************************");
            WriteLog("sendernumber " + sendernumber);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string Baseurl = "https://aceneobank.com/apiService/";
            WriteLog("Request " + Baseurl + "dmt/getBeneficiers?agent_id=" + agentid + "&senderno=" + sendernumber + "");
            var client = new RestClient(Baseurl + "dmt/getBeneficiers?agent_id=" + agentid + "&senderno=" + sendernumber + "");
            var request = new RestRequest(Method.GET);
            request.AddHeader("X-Root-id", agentid);
            request.AddHeader("clientId", clientId);
            request.AddHeader("ClientSecret", ClientSecret);
            //  request.AddHeader("distributorCode", "70359687666670616055318500299916_Si6zudoNA+/VffLEesneSA==");
            request.AddHeader("apiKey", apiKey);


            request.AddHeader("Authorization", "Bearer " + token + "");
            try
            {
                WriteLog("Header X-Root-id " + agentid);
                WriteLog("Header clientId " + clientId);
                WriteLog("Header ClientSecret " + ClientSecret);
                WriteLog("Header apiKey " + apiKey);
                WriteLog("Header Authorization " + "Bearer " + token + "");

            }
            catch { }
            IRestResponse responsecheck = client.Execute(request);
            try
            {
                WriteLog("Response Code " + responsecheck.StatusCode);
                WriteLog("Response " + responsecheck.Content);
            }
            catch { }
            return responsecheck;
        }
        public IRestResponse CreateSendernumber(string agentid, string token, string sendernumber, string Name, string clientId, string ClientSecret, string apiKey)
        {
            WriteLog("************************ CreateSendernumber ************************************");
            WriteLog("sendernumber " + sendernumber);
            WriteLog("Name " + Name);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string Baseurl = "https://aceneobank.com/apiService/";
            var client = new RestClient(Baseurl + "dmt/CreateCustomer");
            var request = new RestRequest(Method.POST);
            request.AddHeader("X-Root-id", agentid);
            request.AddHeader("clientId", clientId);
            request.AddHeader("ClientSecret", ClientSecret);
            // request.AddHeader("distributorCode", "70359687666670616055318500299916_Si6zudoNA+/VffLEesneSA==");
            request.AddHeader("apiKey", apiKey);


            request.AddHeader("Authorization", "Bearer " + token + "");
            //  request.AlwaysMultipartFormData = true;
            var body = new
            {
                agent_id = agentid,
                senderno = sendernumber,
                sendername = Name
            };
            var jsonbody = JsonConvert.SerializeObject(body);
            WriteLog("Request " + jsonbody);
            request.AddParameter("application/json", jsonbody, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            try
            {
                WriteLog("Response Code " + response.StatusCode);
                WriteLog("Response " + response.Content);
            }
            catch { }
            return response;
        }
        public IRestResponse CreateSendernumber_NEW(string agentid, string token, string sendernumber, string KYCRequestId,string OTPRequestId,string OTPPin,string Latitude,string longitude,string PublicIP, string clientId, string ClientSecret, string apiKey)
        {
            WriteLog("************************ CreateSendernumber_NEW ************************************");
            WriteLog("sendernumber " + sendernumber);
         //   WriteLog("Name " + Name);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string Baseurl = "https://aceneobank.com/apiService/";
            var client = new RestClient(Baseurl + "V3/dmt/createCustomer");
            var request = new RestRequest(Method.POST);
            request.AddHeader("X-Root-id", agentid);
            request.AddHeader("clientId", clientId);
            request.AddHeader("ClientSecret", ClientSecret);
            // request.AddHeader("distributorCode", "70359687666670616055318500299916_Si6zudoNA+/VffLEesneSA==");
            request.AddHeader("apiKey", apiKey);


            request.AddHeader("Authorization", "Bearer " + token + "");
            //  request.AlwaysMultipartFormData = true;
            var body = new
            {
                sender_no = sendernumber,
                KYCRequestId,
                OTPRequestId,
                OTPPin,
                Latitude,
                longitude,
                PublicIP
            };
            var jsonbody = JsonConvert.SerializeObject(body);
            WriteLog("Request " + jsonbody);
            request.AddParameter("application/json", jsonbody, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            try
            {
                WriteLog("Response Code " + response.StatusCode);
                WriteLog("Response " + response.Content);
            }
            catch { }
            return response;
        }
        public IRestResponse OTPGenrate_CUstomer(string agentid, string token, string sendernumber, string sender_name, string Latitude, string longitude, string PublicIP, string clientId, string ClientSecret, string apiKey)
        {
            WriteLog("************************ OTPGenrate_CUstomer ************************************");
            WriteLog("sendernumber " + sendernumber);
            //   WriteLog("Name " + Name);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string Baseurl = "https://aceneobank.com/apiService/";
            var client = new RestClient(Baseurl + "V3/dmt/otp/sender");
            var request = new RestRequest(Method.POST);
            request.AddHeader("X-Root-id", agentid);
            request.AddHeader("clientId", clientId);
            request.AddHeader("ClientSecret", ClientSecret);
            // request.AddHeader("distributorCode", "70359687666670616055318500299916_Si6zudoNA+/VffLEesneSA==");
            request.AddHeader("apiKey", apiKey);


            request.AddHeader("Authorization", "Bearer " + token + "");
            //  request.AlwaysMultipartFormData = true;
            var body = new
            {
                sender_no = sendernumber,
                sender_name,
                OTPType="1",
                Latitude,
                longitude,
                PublicIP
            };
            var jsonbody = JsonConvert.SerializeObject(body);
            WriteLog("Request " + jsonbody);
            request.AddParameter("application/json", jsonbody, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            try
            {
                WriteLog("Response Code " + response.StatusCode);
                WriteLog("Response " + response.Content);
            }
            catch { }
            return response;
        }

        public IRestResponse Verifyotp(string agentid, string token, string sendernumber, string Otp, string clientId, string ClientSecret, string apiKey)
        {
            WriteLog("************************ Verifyotp ************************************");
            WriteLog("sendernumber " + sendernumber);
            WriteLog("Otp " + Otp);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string Baseurl = "https://aceneobank.com/apiService/";
            var client = new RestClient(Baseurl + "dmt/VerifyCustomer");
            var request = new RestRequest(Method.POST);
            request.AddHeader("X-Root-id", agentid);
            request.AddHeader("clientId", clientId);
            request.AddHeader("ClientSecret", ClientSecret);
            //request.AddHeader("distributorCode", "70359687666670616055318500299916_Si6zudoNA+/VffLEesneSA==");
            request.AddHeader("apiKey", apiKey);
            request.AddHeader("Authorization", "Bearer " + token + "");
            //request.AlwaysMultipartFormData = true;
            var body = new
            {
                otp = Otp,
                agent_id = agentid,
                senderno = sendernumber
            };
            var jsonbody = JsonConvert.SerializeObject(body);
            WriteLog("Request " + jsonbody);
            request.AddParameter("application/json", jsonbody, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            try
            {
                WriteLog("Response Code " + response.StatusCode);
                WriteLog("Response " + response.Content);
            }
            catch { }
            return response;
        }
        public IRestResponse AddBeneficiary(string agentid, string token, string sendernumber, string Name, string Accountnumber, string bankname, string ifsccode, string clientId, string ClientSecret, string apiKey)
        {
            WriteLog("****************************** AddBeneficiary *********************************");
            WriteLog("sendernumber " + sendernumber);
            WriteLog("Accountnumber " + Accountnumber);
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string Baseurl = "https://aceneobank.com/apiService/";
            var client = new RestClient(Baseurl + "dmt/AddBeneficiar");
            var request = new RestRequest(Method.POST);
            request.AddHeader("X-Root-id", agentid);
            request.AddHeader("clientId", clientId);
            request.AddHeader("ClientSecret", ClientSecret);
            //  request.AddHeader("distributorCode", "70359687666670616055318500299916_Si6zudoNA+/VffLEesneSA==");
            request.AddHeader("apiKey", apiKey);


            request.AddHeader("Authorization", "Bearer " + token + "");
            //  request.AlwaysMultipartFormData = true;
            var body = new
            {
                agent_id = agentid,
                senderno = sendernumber,
                mobileno = sendernumber,
                name = Name,
                accountno = Accountnumber,
                bankname = bankname,
                ifsccode = ifsccode
            };
            var jsonbody = JsonConvert.SerializeObject(body);
            WriteLog("Request  " + jsonbody);
            request.AddParameter("application/json", jsonbody, ParameterType.RequestBody);
            IRestResponse responsecheck = client.Execute(request);
            try
            {
                WriteLog("Response Code " + responsecheck.StatusCode);
                WriteLog("Response  " + responsecheck.Content);
            }
            catch { }
            return responsecheck;
        }
        public IRestResponse DeleteBeneficiary(string agentid, string token, string id, string clientId, string ClientSecret, string apiKey)
        {
            WriteLog("*************************** DeleteBeneficiary ************************");
            WriteLog("ID " + id);
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string Baseurl = "https://aceneobank.com/apiService/";
            var client = new RestClient(Baseurl + "dmt/DeleteBeneficiar");
            var request = new RestRequest(Method.POST);
            request.AddHeader("X-Root-id", agentid);
            request.AddHeader("clientId", clientId);
            request.AddHeader("ClientSecret", ClientSecret);
            //    request.AddHeader("distributorCode", "70359687666670616055318500299916_Si6zudoNA+/VffLEesneSA==");
            request.AddHeader("apiKey", apiKey);


            request.AddHeader("Authorization", "Bearer " + token + "");
            request.AddHeader("content-type", "application/json");
            // request.AlwaysMultipartFormData = true;
            var body = new
            {
                agent_id = agentid,
                id = id
            };
            var jsonbody = JsonConvert.SerializeObject(body);
            WriteLog("Request " + jsonbody);
            request.AddParameter("application/json", jsonbody, ParameterType.RequestBody);
            IRestResponse responsecheck = client.Execute(request);
            try
            {

                WriteLog("Response Code " + responsecheck.StatusCode);
                WriteLog("Response " + responsecheck.Content);
            }
            catch { }
            return responsecheck;
        }
        public IRestResponse VerifyBeneficiary(string agentid, string token, string sendernumber, string Name, string Accountnumber, string bankname, string ifsccode, string id, string clientId, string ClientSecret, string apiKey)
        {
            WriteLog("*************************** VerifyBeneficiary *******************************");
            WriteLog("sendernumber " + sendernumber);
            WriteLog("Accountnumber " + Accountnumber);
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string Baseurl = "https://aceneobank.com/apiService/";
            var client = new RestClient(Baseurl + "dmt/VerifyBeneficiar");
            var request = new RestRequest(Method.POST);
            request.AddHeader("X-Root-id", agentid);
            request.AddHeader("clientId", clientId);
            request.AddHeader("ClientSecret", ClientSecret);
            //   request.AddHeader("distributorCode", "70359687666670616055318500299916_Si6zudoNA+/VffLEesneSA==");
            request.AddHeader("apiKey", apiKey);
            request.AddHeader("Authorization", "Bearer " + token + "");
            //  request.AlwaysMultipartFormData = true;
            var body = new
            {
                agent_id = agentid,
                mobileno = sendernumber,
                name = Name,
                accountno = Accountnumber,
                bankname = bankname,
                ifsccode = ifsccode,
                id = id,
                customermobile = sendernumber
            };
            var jsonbody = JsonConvert.SerializeObject(body);
            WriteLog("Request " + jsonbody);
            request.AddParameter("application/json", jsonbody, ParameterType.RequestBody);
            IRestResponse responsecheck = client.Execute(request);
            try
            {
                WriteLog("Response Code " + responsecheck.StatusCode);
                WriteLog("Response " + responsecheck.Content);
            }
            catch { }
            return responsecheck;
        }
        public IRestResponse VerifyBeneficiary_New(string agentid, string token, string sendernumber, string customerId, string accountno, string name, string ifsccode, string OtpRefrenceId,string OtpPin,string Latitude,string longitude,string PublicIP, string clientId, string ClientSecret, string apiKey)
        {
            WriteLog("*************************** VerifyBeneficiary_New *******************************");
            WriteLog("sendernumber " + sendernumber);
            WriteLog("Accountnumber " + accountno);
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string Baseurl = "https://aceneobank.com/apiService/";
            var client = new RestClient(Baseurl + "V3/dmt/verifyBeneficiary");
            var request = new RestRequest(Method.POST);
            request.AddHeader("X-Root-id", agentid);
            request.AddHeader("clientId", clientId);
            request.AddHeader("ClientSecret", ClientSecret);
            //   request.AddHeader("distributorCode", "70359687666670616055318500299916_Si6zudoNA+/VffLEesneSA==");
            request.AddHeader("apiKey", apiKey);
            request.AddHeader("Authorization", "Bearer " + token + "");
            //  request.AlwaysMultipartFormData = true;
            var body = new
            {
                customerId,
                name,
                accountno,
                agentid,
                ifsccode,
            };
            var jsonbody = JsonConvert.SerializeObject(body);
            WriteLog("Request " + jsonbody);
            request.AddParameter("application/json", jsonbody, ParameterType.RequestBody);
            IRestResponse responsecheck = client.Execute(request);
            try
            {
                WriteLog("Response Code " + responsecheck.StatusCode);
                WriteLog("Response " + responsecheck.Content);
            }
            catch { }
            return responsecheck;
        }

        public IRestResponse Fundtransfer(string agentid, string token, string sendernumber, string benid, string amount, string customerid, string clientId, string ClientSecret, string apiKey,string Pid,string referenceId,string OtpRefrenceId,string OtpPin)
        {
            WriteLog("************************* Fundtransfer *****************************");
            WriteLog("sendernumber " + sendernumber);
            WriteLog("benid " + benid);
            WriteLog("customerid " + customerid);
            WriteLog("amount " + amount);
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string Baseurl = "https://aceneobank.com/apiService/";
            var client = new RestClient(Baseurl + "V4/dmt/transaction");
            var request = new RestRequest(Method.POST);
            request.AddHeader("X-Root-id", agentid);
            request.AddHeader("clientId", clientId);
            request.AddHeader("ClientSecret", ClientSecret);
            // request.AddHeader("distributorCode", "70359687666670616055318500299916_Si6zudoNA+/VffLEesneSA==");
            request.AddHeader("apiKey", apiKey);
            request.AddHeader("Authorization", "Bearer " + token + "");
            //  request.AlwaysMultipartFormData = true;
            var body = new
            {
                customerId = customerid,
                beneId = benid,
                amount = amount,
                Pid = "123456",
                referenceId,
                OtpRefrenceId,
                OtpPin,
                paymentMode = "IMPS",
                agentid,
                dmt_type=2
            };
            var jsonbody = JsonConvert.SerializeObject(body);
            WriteLog("Request " + jsonbody);
            request.AddParameter("application/json", jsonbody, ParameterType.RequestBody);
            IRestResponse responsecheck = client.Execute(request);
            try
            {
                WriteLog("Response Code " + responsecheck.StatusCode);
                WriteLog("Response " + responsecheck.Content);
            }
            catch { }
            return responsecheck;
        }
        public IRestResponse FundtransferSendotp(string customerId,string BeneName,string agentid,string BeneAccountNo,string Latitude,string longitude,string PublicIP,string clientId,string ClientSecret,string apiKey,string token,string amount)
        {
            WriteLog("************************* FundtransferSendotp *****************************");
            WriteLog("customerid " + customerId);
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string Baseurl = "https://aceneobank.com/apiService/";
            var client = new RestClient(Baseurl + "V4/dmt/generateTxnOtp");
            var request = new RestRequest(Method.POST);
            request.AddHeader("X-Root-id", agentid);
            request.AddHeader("clientId", clientId);
            request.AddHeader("ClientSecret", ClientSecret);
            // request.AddHeader("distributorCode", "70359687666670616055318500299916_Si6zudoNA+/VffLEesneSA==");
            request.AddHeader("apiKey", apiKey);
            request.AddHeader("Authorization", "Bearer " + token + "");
            //  request.AlwaysMultipartFormData = true;
            var body = new
            {
                customerId,
                BeneName,
                agentid,
                BeneAccountNo,
                amount,
                dmt_type=2
            };
            var jsonbody = JsonConvert.SerializeObject(body);
            WriteLog("Request " + jsonbody);
            request.AddParameter("application/json", jsonbody, ParameterType.RequestBody);
            IRestResponse responsecheck = client.Execute(request);
            try
            {
                WriteLog("Response Code " + responsecheck.StatusCode);
                WriteLog("Response " + responsecheck.Content);
            }
            catch { }
            return responsecheck;
        }
        
        public IRestResponse PayoutTransfer(string agentid, string token, string sendernumber, string name, string amount, string clientId, string ClientSecret, string apiKey, string beneAccount, string beneBankName, string beneifsc, string pinCode)
        {
            WriteLog("************************* PayoutTransfer *****************************");
            WriteLog("sendernumber " + sendernumber);
            WriteLog("amount " + amount);
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string Baseurl = "https://aceneobank.com/apiService/";
            var client = new RestClient(Baseurl + "payout/reedem");
            var request = new RestRequest(Method.POST);
            request.AddHeader("X-Root-id", agentid);
            request.AddHeader("clientId", clientId);
            request.AddHeader("ClientSecret", ClientSecret);
            //  request.AddHeader("distributorCode", "70359687666670616055318500299916_Si6zudoNA+/VffLEesneSA==");
            request.AddHeader("apiKey", apiKey);
            request.AddHeader("Authorization", "Bearer " + token + "");
            // request.AlwaysMultipartFormData = true;
            //  request.AlwaysMultipartFormData = true;
            var body = new
            {
                redeemAmount = amount,
                beneAccount = beneAccount,
                beneBankName = beneBankName,
                beneName = name,
                beneifsc = beneifsc,
                benePhoneNo = sendernumber,
                pinCode = pinCode
            };
            var jsonbody = JsonConvert.SerializeObject(body);
            WriteLog("Request " + jsonbody);
            request.AddParameter("application/json", jsonbody, ParameterType.RequestBody);
            IRestResponse responsecheck = client.Execute(request);
            try
            {
                WriteLog("Response Code " + responsecheck.StatusCode);
                WriteLog("Response " + responsecheck.Content);
            }
            catch { }
            return responsecheck;
        }
        public IRestResponse RadiantDistributorCreate(string firstname, string lastname, string email, string phoneno, string address, string dob, string pincode, string landmark, string state, string district, string image, string image1, string clientId, string ClientSecret, string apiKey)
        {
            WriteLog("**************************** RadiantDistributorCreate **********************");

            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            string Baseurl = "https://aceneobank.com/apiService/";
            var client = new RestClient(Baseurl + "StoreDistributorDetails");
            var request = new RestRequest(Method.POST);
            request.AddHeader("clientId", clientId);
            request.AddHeader("ClientSecret", ClientSecret);
            request.AddHeader("apiKey", apiKey);
            request.AddHeader("content-type", "application/json");
            var body = new
            {
                firstname = firstname,
                lastname = lastname,
                email = email,
                phoneno = phoneno,
                address = address,
                dob = dob,
                pincode = pincode,
                landmark = landmark,
                state = state,
                district = district,
                ekycimagefront = image,
                ekycimageback = image1,
                Edit = "Enable"
            };
            var reqbody = JsonConvert.SerializeObject(body);
            WriteLog("Request " + reqbody);
            request.AddParameter("application/json", reqbody, ParameterType.RequestBody);
            IRestResponse responsecheck = client.Execute(request);
            try
            {
                WriteLog("Response Code " + responsecheck.StatusCode);
                WriteLog("Response " + responsecheck.Content);
            }
            catch { }

            return responsecheck;
        }
        public IRestResponse RadiantAgentCreate(string gst, string tan, string compan, string distributorCode, string firstname, string lastname, string gender, string dob, string email, string phoneno, string altphoneno,
            string address, string pancard, string adhaarno, string pincode, string landmark, string district, string state, string accountname, string accountno,
            string bankname, string branch, string ifsc, string bank_pincode, string companyname, string shopname, string shopaddress, string shopstate, string shopcity,
            string shopdistrict, string shoparea, string shoppincode, string mcc, string bussiness, string aadharimage, string aadharimageback, string panimage, string chequeimage,
            string pancardimage, string passportsizeimage, string shopimage, string shoplicenseimage, string clientId, string ClientSecret, string apiKey)
        {
            WriteLog("******************************** RadiantAgentCreate **************************");

            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string Baseurl = "https://aceneobank.com/apiService/";
            var client = new RestClient(Baseurl + "StoredetailsAgent");
            var request = new RestRequest(Method.POST);
            request.AddHeader("clientId", clientId);
            request.AddHeader("ClientSecret", ClientSecret);
            //request.AddHeader("distributorCode", distributorCode);
            request.AddHeader("apiKey", apiKey);
            request.AddHeader("content-type", "application/json");
            var body = new
            {
                gst = gst,
                tan = tan,
                compan = compan,
                firstname = firstname,
                lastname = lastname,
                gender = gender,
                dob = dob,
                email = email,
                phoneno = phoneno,
                kyctype = "aadhaar",
                altphoneno = altphoneno,
                address = address,
                pancard = pancard,
                adhaarno = adhaarno,
                pincode = pincode,
                landmark = landmark,
                district = district,
                state = state,
                accountname = accountname,
                accountno = accountno,
                bankname = bankname,
                branch = branch,
                ifsc = ifsc,
                bank_pincode = bank_pincode,
                companyname = companyname,
                shopname = shopname,
                shopaddress = shopaddress,
                shopstate = shopstate,
                shopcity = shopcity,
                shopdistrict = shopdistrict,
                shoparea = shoparea,
                shoppincode = shoppincode,
                mcc = mcc,
                bussiness = bussiness,
                ekycimage = aadharimage,
                //panimage= panimage,
                chequeimage = chequeimage,
                aadharimagefront = aadharimage,
                aadharimageback = aadharimageback,
                pancardimage = panimage,
                passportsizeimage = passportsizeimage,
                shopimage = shopimage,
                shoplicenseimage = shoplicenseimage
            };
            var reqbody = JsonConvert.SerializeObject(body);
            WriteLog("Request " + reqbody);
            request.AddParameter("application/json", reqbody, ParameterType.RequestBody);
            IRestResponse responsecheck = client.Execute(request);
            try
            {
                WriteLog("Response Code rediant create " + responsecheck.StatusCode);
                WriteLog("Response " + responsecheck.Content);

            }
            catch { }
            return responsecheck;
        }
        public IRestResponse Radiantstate(string clientId, string ClientSecret, string apiKey)
        {
            WriteLog("************************** Radiantstate **************************");
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string Baseurl = "https://aceneobank.com/apiService/";
            var client = new RestClient(Baseurl + "getState");
            var request = new RestRequest(Method.GET);
            request.AddHeader("clientId", clientId);
            request.AddHeader("ClientSecret", ClientSecret);
            request.AddHeader("apiKey", apiKey);
            request.AddHeader("content-type", "application/json");
            IRestResponse responsecheck = client.Execute(request);
            try
            {
                WriteLog("Response " + responsecheck.Content);
            }
            catch { }
            return responsecheck;
        }
        public IRestResponse RadiantgetMcc(string clientId, string ClientSecret, string apiKey)
        {
            WriteLog("******************** RadiantgetMcc **********************************");
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string Baseurl = "https://aceneobank.com/apiService/";
            var client = new RestClient(Baseurl + "getMcc");
            var request = new RestRequest(Method.GET);
            request.AddHeader("clientId", clientId);
            request.AddHeader("ClientSecret", ClientSecret);
            request.AddHeader("apiKey", apiKey);
            request.AddHeader("content-type", "application/json");
            IRestResponse responsecheck = client.Execute(request);
            try
            {
                WriteLog("Response Code " + responsecheck.StatusCode);
                WriteLog("Response " + responsecheck.Content);
            }
            catch { }
            return responsecheck;
        }
        public IRestResponse customerparamsAsync(string agentid, string token, string clientId, string ClientSecret, string billerid, string apiKey)
        {
            WriteLog("********************* customerparamsAsync ***********************");
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string Baseurl = "https://aceneobank.com/apiService/";
            var client = new RestClient(Baseurl + "billpay/customer-params");
            var request = new RestRequest(Method.POST);
            request.AddHeader("X-Root-id", agentid);
            request.AddHeader("clientId", clientId);
            request.AddHeader("ClientSecret", ClientSecret);
            // request.AddHeader("distributorCode", "70359687666670616055318500299916_Si6zudoNA+/VffLEesneSA==");
            request.AddHeader("apiKey", apiKey);

            request.AddHeader("Authorization", "Bearer " + token);
            request.AddHeader("content-type", "application/json");
            var requestBody = new
            {
                agent = agentid,
                billerid = billerid
            };
            string jsonRequestBody = JsonConvert.SerializeObject(requestBody);
            WriteLog("Request " + jsonRequestBody);
            request.AddParameter("application/json", jsonRequestBody, ParameterType.RequestBody);
            IRestResponse responsecheck = client.Execute(request);
            try
            {
                WriteLog("Response Code " + responsecheck.StatusCode);
                WriteLog("Response " + responsecheck.Content);
            }
            catch { }
            return responsecheck;
        }
        public IRestResponse BillFetch(string agentid, string token, string clientId, string ClientSecret, string distributorCode, string billerid, string paramName, string paramValue, string apiKey)
        {
            WriteLog("********************** BillFetch ****************************");
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string Baseurl = "https://aceneobank.com/apiService/";
            var client = new RestClient(Baseurl + "billpay/bill-fetch");
            var request = new RestRequest(Method.POST);
            request.AddHeader("X-Root-id", agentid);
            request.AddHeader("clientId", clientId);
            request.AddHeader("ClientSecret", ClientSecret);
            //  request.AddHeader("distributorCode", "70359687666670616055318500299916_Si6zudoNA+/VffLEesneSA==");
            request.AddHeader("apiKey", apiKey);
            request.AddHeader("Authorization", "Bearer " + token + "");
            request.AddHeader("content-type", "application/json");
            var body = new
            {
                agent = agentid,
                billerid,
                paramName,
                paramValue
            };
            var reqbody = JsonConvert.SerializeObject(body);
            WriteLog("Request " + reqbody);
            request.AddParameter("application/json", reqbody, ParameterType.RequestBody);
            IRestResponse responsecheck = client.Execute(request);
            try
            {
                WriteLog("Response Code " + responsecheck.StatusCode);
                WriteLog("Response " + responsecheck.Content);
            }
            catch { }
            return responsecheck;
        }
        public IRestResponse BillPay(string agentid, string token, string clientId, string ClientSecret, string billerid, string billcategory, string billpaymentid, string billermode, string amount, string apiKey)
        {
            WriteLog("********************** BillPay **********************************");
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string Baseurl = "https://aceneobank.com/apiService/";
            var client = new RestClient(Baseurl + "billpay/bill-pay");
            var request = new RestRequest(Method.POST);
            request.AddHeader("X-Root-id", agentid);
            request.AddHeader("clientId", clientId);
            request.AddHeader("ClientSecret", ClientSecret);
            //  request.AddHeader("distributorCode", "70359687666670616055318500299916_Si6zudoNA+/VffLEesneSA==");
            request.AddHeader("apiKey", apiKey);


            request.AddHeader("Authorization", "Bearer " + token + "");
            request.AddHeader("content-type", "application/json");
            var body = new
            {
                agent = agentid,
                billerid,
                billcategory,
                billpaymentid,
                billermode,
                amount
            };
            var reqbody = JsonConvert.SerializeObject(body);
            WriteLog("Request " + reqbody);
            request.AddParameter("application/json", reqbody, ParameterType.RequestBody);
            IRestResponse responsecheck = client.Execute(request);
            try
            {
                WriteLog("Response Code " + responsecheck.StatusCode);
                WriteLog("Response " + responsecheck.Content);
            }
            catch { }
            return responsecheck;
        }
        public IRestResponse Billvalidation(string agentid, string token, string clientId, string ClientSecret, string distributorCode, string billerid, string paramName, string paramValue, string amount, string apiKey)
        {
            WriteLog("********************* Billvalidation *****************************");
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string Baseurl = "https://aceneobank.com/apiService/";
            var client = new RestClient(Baseurl + "billpay/bill-validation");
            var request = new RestRequest(Method.POST);
            request.AddHeader("X-Root-id", agentid);
            request.AddHeader("clientId", clientId);
            request.AddHeader("ClientSecret", ClientSecret);
            //request.AddHeader("distributorCode", "70359687666670616055318500299916_Si6zudoNA+/VffLEesneSA==");
            request.AddHeader("apiKey", apiKey);


            request.AddHeader("Authorization", "Bearer " + token + "");

            request.AddHeader("content-type", "application/json");
            var body = new
            {
                agent = agentid,
                billerid,
                paramName,
                paramValue,
                amount
            };
            var reqbody = JsonConvert.SerializeObject(body);
            WriteLog("Request " + reqbody);
            request.AddParameter("application/json", reqbody, ParameterType.RequestBody);
            IRestResponse responsecheck = client.Execute(request);
            try
            {
                WriteLog("Response Code " + responsecheck.StatusCode);
                WriteLog("Response " + responsecheck.Content);
            }
            catch { }
            return responsecheck;
        }
        public IRestResponse AEPSbankDetails(string agentid, string token, string clientId, string ClientSecret, string apiKey)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string Baseurl = "https://aceneobank.com/apiService/";
            var client = new RestClient(Baseurl + "AEPS/bankDetails");
            var request = new RestRequest(Method.GET);
            request.AddHeader("X-Root-id", agentid);
            request.AddHeader("clientId", clientId);
            request.AddHeader("ClientSecret", ClientSecret);
            //request.AddHeader("distributorCode", distributorCode);
            request.AddHeader("apiKey", apiKey);

            request.AddHeader("Authorization", "Bearer " + token + "");
            IRestResponse responsecheck = client.Execute(request);
            return responsecheck;
        }
        public IRestResponse AEPStwoFactorAuth(string agentid, string token, string data, string latitude, string longitude, string aadhaarNo, string nationalbankidentification, string remarks, string bank, string type, string clientId, string ClientSecret, string apiKey)
        {
            WriteLog("********************* customerparamsAsync ***********************");

            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string Baseurl = "https://aceneobank.com/apiService/";
            var client = new RestClient(Baseurl + "AEPS/twoFactorAuth");
            var request = new RestRequest(Method.POST);
            request.AddHeader("X-Root-id", agentid);
            request.AddHeader("clientId", clientId);
            request.AddHeader("ClientSecret", ClientSecret);
            //request.AddHeader("distributorCode", "70359687666670616055318500299916_Si6zudoNA+/VffLEesneSA==");
            request.AddHeader("apiKey", apiKey);

            request.AddHeader("Authorization", "Bearer " + token);
            request.AddHeader("content-type", "application/json");
            var requestBody = new
            {
                agent_id = agentid,
                data,
                latitude,
                longitude,
                aadhaarNo,
                nationalbankidentification,
                remarks,
                bank,
                type
            };
            string jsonRequestBody = JsonConvert.SerializeObject(requestBody);
            WriteLog("Request " + jsonRequestBody);
            request.AddParameter("application/json", jsonRequestBody, ParameterType.RequestBody);
            IRestResponse responsecheck = client.Execute(request);
            try
            {
                WriteLog("Response Code " + responsecheck.StatusCode);
                WriteLog("Response " + responsecheck.Content);
            }
            catch { }
            return responsecheck;
        }
        public IRestResponse AEPScashwithdrawal(string agentid, string token, string data, string latitude, string longitude, string aadhaarNo, string nationalbankidentification, string remarks, string bank, string mobilenumber, string transcationamount, string clientId, string ClientSecret, string apiKey)
        {
            WriteLog("********************* customerparamsAsync ***********************");
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string Baseurl = "https://aceneobank.com/apiService/";
            var client = new RestClient(Baseurl + "AEPS/cashwithdrawal");
            var request = new RestRequest(Method.POST);

            request.AddHeader("X-Root-id", agentid);
            request.AddHeader("clientId", clientId);
            request.AddHeader("ClientSecret", ClientSecret);
            // request.AddHeader("distributorCode", "70359687666670616055318500299916_Si6zudoNA+/VffLEesneSA==");
            request.AddHeader("apiKey", apiKey);

            request.AddHeader("Authorization", "Bearer " + token);
            request.AddHeader("content-type", "application/json");
            var requestBody = new
            {
                agent_id = agentid,
                data,
                latitude,
                longitude,
                aadhaarNo,
                nationalbankidentification,
                remarks,
                bank,
                mobilenumber,
                transcationamount,
                type = "CW"
            };
            string jsonRequestBody = JsonConvert.SerializeObject(requestBody);
            WriteLog("Request " + jsonRequestBody);
            request.AddParameter("application/json", jsonRequestBody, ParameterType.RequestBody);
            IRestResponse responsecheck = client.Execute(request);
            try
            {
                WriteLog("Response Code " + responsecheck.StatusCode);
                WriteLog("Response " + responsecheck.Content);
            }
            catch { }
            return responsecheck;
        }
        public IRestResponse AEPSBalanceEnquiry(string agentid, string token, string data, string latitude, string longitude, string aadhaarNo, string nationalbankidentification, string remarks, string bank, string mobilenumber, string clientId, string ClientSecret, string apiKey)
        {
            WriteLog("********************* customerparamsAsync ***********************");
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string Baseurl = "https://aceneobank.com/apiService/";
            var client = new RestClient(Baseurl + "AEPS/BalanceEnquiry");
            var request = new RestRequest(Method.POST);
            request.AddHeader("X-Root-id", agentid);
            request.AddHeader("clientId", clientId);
            request.AddHeader("ClientSecret", ClientSecret);
            // request.AddHeader("distributorCode", "70359687666670616055318500299916_Si6zudoNA+/VffLEesneSA==");
            request.AddHeader("apiKey", apiKey);

            request.AddHeader("Authorization", "Bearer " + token);
            request.AddHeader("content-type", "application/json");
            var requestBody = new
            {
                agent_id = agentid,
                data,
                latitude,
                longitude,
                aadhaarNo,
                nationalbankidentification,
                remarks,
                bank,
                mobilenumber,
                type = "BE"
            };
            string jsonRequestBody = JsonConvert.SerializeObject(requestBody);
            WriteLog("Request " + jsonRequestBody);
            request.AddParameter("application/json", jsonRequestBody, ParameterType.RequestBody);
            IRestResponse responsecheck = client.Execute(request);
            try
            {
                WriteLog("Response Code " + responsecheck.StatusCode);
                WriteLog("Response " + responsecheck.Content);
            }
            catch { }
            return responsecheck;
        }
        public IRestResponse AEPSMiniStatement(string agentid, string token, string data, string latitude, string longitude, string aadhaarNo, string nationalbankidentification, string remarks, string bank, string mobilenumber, string clientId, string ClientSecret, string apiKey)
        {
            WriteLog("********************* customerparamsAsync ***********************");
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string Baseurl = "https://aceneobank.com/apiService/";
            var client = new RestClient(Baseurl + "AEPS/MiniStatement");
            var request = new RestRequest(Method.POST);

            request.AddHeader("X-Root-id", agentid);
            request.AddHeader("clientId", clientId);
            request.AddHeader("ClientSecret", ClientSecret);
            //    request.AddHeader("distributorCode", "70359687666670616055318500299916_Si6zudoNA+/VffLEesneSA==");
            request.AddHeader("apiKey", apiKey);


            request.AddHeader("Authorization", "Bearer " + token);
            request.AddHeader("content-type", "application/json");
            var requestBody = new
            {
                agent_id = agentid,
                data,
                latitude,
                longitude,
                aadhaarNo,
                nationalbankidentification,
                remarks,
                bank,
                mobilenumber,
                type = "MS"
            };
            string jsonRequestBody = JsonConvert.SerializeObject(requestBody);
            WriteLog("Request " + jsonRequestBody);
            request.AddParameter("application/json", jsonRequestBody, ParameterType.RequestBody);
            IRestResponse responsecheck = client.Execute(request);
            try
            {
                WriteLog("Response Code " + responsecheck.StatusCode);
                WriteLog("Response " + responsecheck.Content);
            }
            catch { }
            return responsecheck;
        }
        public IRestResponse AEPSAadhaarPay(string agentid, string token, string data, string latitude, string longitude, string aadhaarNo, string nationalbankidentification, string remarks, string bank, string mobilenumber, string transcationamount, string clientId, string ClientSecret, string apiKey)
        {
            WriteLog("********************* customerparamsAsync ***********************");
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string Baseurl = "https://aceneobank.com/apiService/";
            var client = new RestClient(Baseurl + "AEPS/AadhaarPay");
            var request = new RestRequest(Method.POST);
            request.AddHeader("X-Root-id", agentid);
            request.AddHeader("clientId", clientId);
            request.AddHeader("ClientSecret", ClientSecret);
            // request.AddHeader("distributorCode", "70359687666670616055318500299916_Si6zudoNA+/VffLEesneSA==");
            request.AddHeader("apiKey", apiKey);

            request.AddHeader("Authorization", "Bearer " + token);
            request.AddHeader("content-type", "application/json");
            var requestBody = new
            {
                agent_id = agentid,
                data,
                latitude,
                longitude,
                aadhaarNo,
                nationalbankidentification,
                remarks,
                bank,
                mobilenumber,
                transcationamount,
                type = "AP"
            };
            string jsonRequestBody = JsonConvert.SerializeObject(requestBody);
            WriteLog("Request " + jsonRequestBody);
            request.AddParameter("application/json", jsonRequestBody, ParameterType.RequestBody);
            IRestResponse responsecheck = client.Execute(request);
            try
            {
                WriteLog("Response Code " + responsecheck.StatusCode);
                WriteLog("Response " + responsecheck.Content);
            }
            catch { }
            return responsecheck;
        }
        public IRestResponse EkycSendOTP(string agentid, string aadhaarNo, string apiKey)
        {
            WriteLog("********************* EkycSendOTP *****************************");
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string Baseurl = "https://aceneobank.com/apiService/";
            var client = new RestClient(Baseurl + "AEPS/ekycSendOTP");
            var request = new RestRequest(Method.POST);
            request.AddHeader("apiKey", apiKey);
            request.AddHeader("content-type", "application/json");
            var body = new
            {
                agent_id = agentid,
                aadhaarNo,
            };
            var reqbody = JsonConvert.SerializeObject(body);
            WriteLog("Request " + reqbody);
            request.AddParameter("application/json", reqbody, ParameterType.RequestBody);
            IRestResponse responsecheck = client.Execute(request);
            try
            {
                WriteLog("Response Code " + responsecheck.StatusCode);
                WriteLog("Response " + responsecheck.Content);
            }
            catch { }
            return responsecheck;
        }
        public IRestResponse EkycVerifyOTP(string agentid, string otp, string primaryKeyId, string encodeTxnId, string apiKey)
        {
            WriteLog("********************* EkycVerifyOTP *****************************");
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string Baseurl = "https://aceneobank.com/apiService/";
            var client = new RestClient(Baseurl + "AEPS/ekycVerifyOTP");
            var request = new RestRequest(Method.POST);
            request.AddHeader("apiKey", apiKey);
            request.AddHeader("content-type", "application/json");
            var body = new
            {
                agent_id = agentid,
                otp,
                primaryKeyId,
                encodeTxnId,
            };
            var reqbody = JsonConvert.SerializeObject(body);
            WriteLog("Request " + reqbody);
            request.AddParameter("application/json", reqbody, ParameterType.RequestBody);
            IRestResponse responsecheck = client.Execute(request);
            try
            {
                WriteLog("Response Code " + responsecheck.StatusCode);
                WriteLog("Response " + responsecheck.Content);
            }
            catch { }
            return responsecheck;
        }
        public IRestResponse EkycAuthBiometric(string agent_id, string primaryKeyId, string encodeTxnId, string data, string apiKey)
        {
            WriteLog("********************* EkycAuthBiometric *****************************");
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string Baseurl = "https://aceneobank.com/apiService/";
            var client = new RestClient(Baseurl + "AEPS/EkycAuthBiometric");
            var request = new RestRequest(Method.POST);
            request.AddHeader("apiKey", apiKey);
            request.AddHeader("content-type", "application/json");
            var body = new
            {
                agent_id,
                primaryKeyId,
                encodeTxnId,
                data
            };
            var reqbody = JsonConvert.SerializeObject(body);
            WriteLog("Request " + reqbody);
            request.AddParameter("application/json", reqbody, ParameterType.RequestBody);
            IRestResponse responsecheck = client.Execute(request);
            try
            {
                WriteLog("Response Code " + responsecheck.StatusCode);
                WriteLog("Response " + responsecheck.Content);
            }
            catch { }
            return responsecheck;
        }
        public IRestResponse EkycStatusCheck(string agent_id, string apiKey)
        {
            WriteLog("********************* EkycStatusCheck *****************************");
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string Baseurl = "https://aceneobank.com/apiService/";

            var client = new RestClient(Baseurl + "AEPS/EkycStatusCheck/" + agent_id);
            var request = new RestRequest(Method.GET);
            request.AddHeader("apiKey", apiKey);
            IRestResponse responsecheck = client.Execute(request);
            try
            {
                WriteLog("Response Code " + responsecheck.StatusCode);
                WriteLog("Response " + responsecheck.Content);
            }
            catch { }
            return responsecheck;
        }
        public IRestResponse UPIsendOTP(string agentid, string token, string clientId, string ClientSecret, string distributorCode, string cusMobile, string cusName, string amount, string apiKey)
        {
            WriteLog("********************* UPIsendOTP *****************************");
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string Baseurl = "https://aceneobank.com/apiService/";
            var client = new RestClient(Baseurl + "upiAtm/sendOTP");
            var request = new RestRequest(Method.POST);
            request.AddHeader("X-Root-id", agentid);
            request.AddHeader("clientId", clientId);
            request.AddHeader("ClientSecret", ClientSecret);
            // request.AddHeader("distributorCode", "70359687666670616055318500299916_Si6zudoNA+/VffLEesneSA==");
            request.AddHeader("apiKey", apiKey);


            request.AddHeader("Authorization", "Bearer " + token + "");

            request.AddHeader("content-type", "application/json");
            var body = new
            {
                cusMobile,
                cusName,
                amount,
            };
            var reqbody = JsonConvert.SerializeObject(body);
            WriteLog("Request " + reqbody);
            //request.AddParameter("application/json", reqbody, ParameterType.RequestBody);
            IRestResponse responsecheck = client.Execute(request);
            try
            {
                WriteLog("Response Code " + responsecheck.StatusCode);
                WriteLog("Response " + responsecheck.Content);
            }
            catch { }
            return responsecheck;
        }
        public IRestResponse DistributorStatus(string distCode, string clientId, string ClientSecret, string apiKey)
        {
            WriteLog("********************* EkycAuthBiometric *****************************");
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string Baseurl = "https://aceneobank.com/apiService/";
            var client = new RestClient(Baseurl + "listDistributor");
            var request = new RestRequest(Method.POST);
            request.AddHeader("apiKey", apiKey);
            request.AddHeader("clientId", clientId);
            request.AddHeader("ClientSecret", ClientSecret);
            request.AddHeader("content-type", "application/json");
            var body = new
            {
                fromDate = "",
                toDate = "",
                distCode,
            };
            var reqbody = JsonConvert.SerializeObject(body);
            WriteLog("Request " + reqbody);
            request.AddParameter("application/json", reqbody, ParameterType.RequestBody);
            IRestResponse responsecheck = client.Execute(request);
            try
            {
                WriteLog("Response Code " + responsecheck.StatusCode);
                WriteLog("Response " + responsecheck.Content);
            }
            catch { }
            return responsecheck;
        }
        public IRestResponse AgentStatus(string agent_code, string clientId, string ClientSecret, string apiKey)
        {
            WriteLog("********************* EkycAuthBiometric *****************************");
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string Baseurl = "https://aceneobank.com/apiService/";
            var client = new RestClient(Baseurl + "listAgent");
            var request = new RestRequest(Method.POST);
            request.AddHeader("apiKey", apiKey);
            request.AddHeader("clientId", clientId);
            request.AddHeader("ClientSecret", ClientSecret);
            request.AddHeader("content-type", "application/json");
            var body = new
            {
                fromDate = "",
                toDate = "",
                agent_code,
            };
            var reqbody = JsonConvert.SerializeObject(body);
            WriteLog("Request " + reqbody);
            request.AddParameter("application/json", reqbody, ParameterType.RequestBody);
            IRestResponse responsecheck = client.Execute(request);
            dynamic responseData = JsonConvert.DeserializeObject(responsecheck.Content);
            try
            {
                WriteLog("Response Code " + responsecheck.StatusCode);
                WriteLog("Response " + responsecheck.Content);
            }
            catch { }
            return responsecheck;
        }
        public IRestResponse WalletCreateOrder(string agent_id, string clientId, string ClientSecret, string apiKey, string token, string amount, string type)
        {
            WriteLog("********************* WalletCreateOrder *****************************");
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string Baseurl = "https://aceneobank.com/apiService/";
            var client = new RestClient(Baseurl + "wallet/orderCreate");
            var request = new RestRequest(Method.POST);
            request.AddHeader("X-Root-id", agent_id);
            request.AddHeader("clientId", clientId);
            request.AddHeader("ClientSecret", ClientSecret);
            // request.AddHeader("distributorCode", "70359687666670616055318500299916_Si6zudoNA+/VffLEesneSA==");
            request.AddHeader("apiKey", apiKey);
            request.AddHeader("Authorization", "Bearer " + token + "");
            request.AddHeader("content-type", "application/json");
            var body = new
            {
                agent_id,
                amount,
                type,
            };
            var reqbody = JsonConvert.SerializeObject(body);
            WriteLog("Request " + reqbody);
            request.AddParameter("application/json", reqbody, ParameterType.RequestBody);
            IRestResponse responsecheck = client.Execute(request);
            try
            {
                WriteLog("Response Code " + responsecheck.StatusCode);
                WriteLog("Response " + responsecheck.Content);
            }
            catch { }
            return responsecheck;
        }
        public IRestResponse WallepayOrder(string agent_id, string clientId, string ClientSecret, string apiKey, string token, string amount, string orderId, string payerVpa, string payerName, string remarks)
        {
            WriteLog("********************* WallepayOrder *****************************");
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string Baseurl = "https://aceneobank.com/apiService/";
            var client = new RestClient(Baseurl + "wallet/collect/payOrder");
            var request = new RestRequest(Method.POST);
            request.AddHeader("X-Root-id", agent_id);
            request.AddHeader("clientId", clientId);
            request.AddHeader("ClientSecret", ClientSecret);
            // request.AddHeader("distributorCode", "70359687666670616055318500299916_Si6zudoNA+/VffLEesneSA==");
            request.AddHeader("apiKey", apiKey);
            request.AddHeader("Authorization", "Bearer " + token + "");
            request.AddHeader("content-type", "application/json");
            var body = new
            {
                agent_id,
                orderId,
                amount,
                payerVpa,
                payerName,
                remarks
            };
            var reqbody = JsonConvert.SerializeObject(body);
            WriteLog("Request " + reqbody);
            request.AddParameter("application/json", reqbody, ParameterType.RequestBody);
            IRestResponse responsecheck = client.Execute(request);
            try
            {
                WriteLog("Response Code " + responsecheck.StatusCode);
                WriteLog("Response " + responsecheck.Content);
            }
            catch { }
            return responsecheck;
        }
        public IRestResponse collectPayVerify(string agent_id, string clientId, string ClientSecret, string apiKey, string token, string payerVpa, string orderId, string amount, string payerName, string remarks)
        {
            WriteLog("********************* collectPayVerify *****************************");
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string Baseurl = "https://aceneobank.com/apiService/";
            var client = new RestClient(Baseurl + "wallet/upiVerify");
            var request = new RestRequest(Method.POST);
            request.AddHeader("X-Root-id", agent_id);
            request.AddHeader("clientId", clientId);
            request.AddHeader("ClientSecret", ClientSecret);
            // request.AddHeader("distributorCode", "70359687666670616055318500299916_Si6zudoNA+/VffLEesneSA==");
            request.AddHeader("apiKey", apiKey);
            request.AddHeader("Authorization", "Bearer " + token + "");
            request.AddHeader("content-type", "application/json");
            var body = new
            {
                agent_id,
                orderId,
                amount,
                payerVpa,
                payerName,
                remarks
            };
            var reqbody = JsonConvert.SerializeObject(body);
            WriteLog("Request " + reqbody);
            //request.AddParameter("application/json", reqbody, ParameterType.RequestBody);
            IRestResponse responsecheck = client.Execute(request);
            try
            {
                WriteLog("Response Code " + responsecheck.StatusCode);
                WriteLog("Response " + responsecheck.Content);
            }
            catch { }
            return responsecheck;
        }
        public IRestResponse collectPayStatus(string agent_id, string clientId, string ClientSecret, string apiKey, string token, string orderId)
        {
            WriteLog("********************* collectPayStatus *****************************");
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string Baseurl = "https://aceneobank.com/apiService/";
            var client = new RestClient(Baseurl + "wallet/upiVerify");
            var request = new RestRequest(Method.POST);
            request.AddHeader("X-Root-id", agent_id);
            request.AddHeader("clientId", clientId);
            request.AddHeader("ClientSecret", ClientSecret);
            // request.AddHeader("distributorCode", "70359687666670616055318500299916_Si6zudoNA+/VffLEesneSA==");
            request.AddHeader("apiKey", apiKey);
            request.AddHeader("Authorization", "Bearer " + token + "");
            request.AddHeader("content-type", "application/json");
            var body = new
            {
                agent_id,
                orderId,
                type = "collect",
            };
            var reqbody = JsonConvert.SerializeObject(body);
            WriteLog("Request " + reqbody);
            //request.AddParameter("application/json", reqbody, ParameterType.RequestBody);
            IRestResponse responsecheck = client.Execute(request);
            try
            {
                WriteLog("Response Code " + responsecheck.StatusCode);
                WriteLog("Response " + responsecheck.Content);
            }
            catch { }
            return responsecheck;
        }
        public IRestResponse UPIATMsubscriptionCheck(string agentId, string clientId, string ClientSecret, string apiKey, string token)
        {
            WriteLog("********************* UPIATMsubscriptionCheck *****************************");
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string Baseurl = "https://aceneobank.com/apiService/";
            var client = new RestClient(Baseurl + "upiAtm/UPIATMsubscriptionCheck");
            var request = new RestRequest(Method.POST);
            request.AddHeader("X-Root-id", agentId);
            request.AddHeader("clientId", clientId);
            request.AddHeader("ClientSecret", ClientSecret);
            // request.AddHeader("distributorCode", "70359687666670616055318500299916_Si6zudoNA+/VffLEesneSA==");
            request.AddHeader("apiKey", apiKey);
            request.AddHeader("Authorization", "Bearer " + token + "");
            request.AddHeader("content-type", "application/json");
            var body = new
            {
                agentId,
                flag = "app"
            };
            var reqbody = JsonConvert.SerializeObject(body);
            WriteLog("Request " + reqbody);
            //request.AddParameter("application/json", reqbody, ParameterType.RequestBody);
            IRestResponse responsecheck = client.Execute(request);
            try
            {
                WriteLog("Response Code " + responsecheck.StatusCode);
                WriteLog("Response " + responsecheck.Content);
            }
            catch { }
            return responsecheck;
        }
        public IRestResponse UPIATMsubscriptionOTP(string agentId, string clientId, string ClientSecret, string apiKey, string token)
        {
            WriteLog("********************* UPIATMsubscriptionOTP *****************************");
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string Baseurl = "https://aceneobank.com/apiService/";
            var client = new RestClient(Baseurl + "upiAtm/subscriptionOTP");
            var request = new RestRequest(Method.POST);
            request.AddHeader("X-Root-id", agentId);
            request.AddHeader("clientId", clientId);
            request.AddHeader("ClientSecret", ClientSecret);
            // request.AddHeader("distributorCode", "70359687666670616055318500299916_Si6zudoNA+/VffLEesneSA==");
            request.AddHeader("apiKey", apiKey);
            request.AddHeader("Authorization", "Bearer " + token + "");
            request.AddHeader("content-type", "application/json");
            var body = new
            {
                otp_type = "Send",
                flag = "app",
                agentId
            };
            var reqbody = JsonConvert.SerializeObject(body);
            WriteLog("Request " + reqbody);
            request.AddParameter("application/json", reqbody, ParameterType.RequestBody);
            IRestResponse responsecheck = client.Execute(request);
            try
            {
                WriteLog("Response Code " + responsecheck.StatusCode);
                WriteLog("Response " + responsecheck.Content);
            }
            catch { }
            return responsecheck;
        }
        public IRestResponse UPIATMsubscription(string agentId, string clientId, string ClientSecret, string apiKey, string token, string otp)
        {
            WriteLog("********************* UPIATMsubscription *****************************");
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string Baseurl = "https://aceneobank.com/apiService/";
            var client = new RestClient(Baseurl + "upiAtm/subscription");
            var request = new RestRequest(Method.POST);
            request.AddHeader("X-Root-id", agentId);
            request.AddHeader("clientId", clientId);
            request.AddHeader("ClientSecret", ClientSecret);
            // request.AddHeader("distributorCode", "70359687666670616055318500299916_Si6zudoNA+/VffLEesneSA==");
            request.AddHeader("apiKey", apiKey);
            request.AddHeader("Authorization", "Bearer " + token + "");
            request.AddHeader("content-type", "application/json");
            var body = new
            {
                otp,
                flag = "app",
                agentId,
                subscription = "subscribe"
            };
            var reqbody = JsonConvert.SerializeObject(body);
            WriteLog("Request " + reqbody);
            request.AddParameter("application/json", reqbody, ParameterType.RequestBody);
            IRestResponse responsecheck = client.Execute(request);
            try
            {
                WriteLog("Response Code " + responsecheck.StatusCode);
                WriteLog("Response " + responsecheck.Content);
            }
            catch { }
            return responsecheck;
        }
        public IRestResponse UPIATMUnsubscription(string agentId, string clientId, string ClientSecret, string apiKey, string token, string otp)
        {
            WriteLog("********************* UPIATMUnsubscription *****************************");
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string Baseurl = "https://aceneobank.com/apiService/";
            var client = new RestClient(Baseurl + "wallet/Unsubscription");
            var request = new RestRequest(Method.POST);
            request.AddHeader("X-Root-id", agentId);
            request.AddHeader("clientId", clientId);
            request.AddHeader("ClientSecret", ClientSecret);
            // request.AddHeader("distributorCode", "70359687666670616055318500299916_Si6zudoNA+/VffLEesneSA==");
            request.AddHeader("apiKey", apiKey);
            request.AddHeader("Authorization", "Bearer " + token + "");
            request.AddHeader("content-type", "application/json");
            var body = new
            {
                otp,
                flag = "app",
                agentId,
                subscription = "Unsubscription"
            };
            var reqbody = JsonConvert.SerializeObject(body);
            WriteLog("Request " + reqbody);
            //request.AddParameter("application/json", reqbody, ParameterType.RequestBody);
            IRestResponse responsecheck = client.Execute(request);
            try
            {
                WriteLog("Response Code " + responsecheck.StatusCode);
                WriteLog("Response " + responsecheck.Content);
            }
            catch { }
            return responsecheck;
        }
        public IRestResponse UPIATMsendOTP(string agentId, string clientId, string ClientSecret, string apiKey, string token, string cusMobile, string cusName, string amount)
        {
            WriteLog("********************* UPIATMsendOTP *****************************");
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string Baseurl = "https://aceneobank.com/apiService/";
            var client = new RestClient(Baseurl + "upiAtm/sendOTP");
            var request = new RestRequest(Method.POST);
            request.AddHeader("X-Root-id", agentId);
            request.AddHeader("clientId", clientId);
            request.AddHeader("ClientSecret", ClientSecret);
            // request.AddHeader("distributorCode", "70359687666670616055318500299916_Si6zudoNA+/VffLEesneSA==");
            request.AddHeader("apiKey", apiKey);
            request.AddHeader("Authorization", "Bearer " + token + "");
            request.AddHeader("content-type", "application/json");
            var body = new
            {
                cusMobile,
                cusName,
                amount
            };
            var reqbody = JsonConvert.SerializeObject(body);
            WriteLog("Request " + reqbody);
            request.AddParameter("application/json", reqbody, ParameterType.RequestBody);
            IRestResponse responsecheck = client.Execute(request);
            try
            {
                WriteLog("Response Code " + responsecheck.StatusCode);
                WriteLog("Response " + responsecheck.Content);
            }
            catch { }
            return responsecheck;
        }
        public IRestResponse UPIATMverifyOTP(string agentId, string clientId, string ClientSecret, string apiKey, string token, string otpRef_id, string refId, string otp)
        {
            WriteLog("********************* UPIATMverifyOTP *****************************");
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string Baseurl = "https://aceneobank.com/apiService/";
            var client = new RestClient(Baseurl + "upiAtm/verifyOTP");
            var request = new RestRequest(Method.POST);
            request.AddHeader("X-Root-id", agentId);
            request.AddHeader("clientId", clientId);
            request.AddHeader("ClientSecret", ClientSecret);
            // request.AddHeader("distributorCode", "70359687666670616055318500299916_Si6zudoNA+/VffLEesneSA==");
            request.AddHeader("apiKey", apiKey);
            request.AddHeader("Authorization", "Bearer " + token + "");
            request.AddHeader("content-type", "application/json");
            var body = new
            {
                otpRef_id,
                refId,
                otp
            };
            var reqbody = JsonConvert.SerializeObject(body);
            WriteLog("Request " + reqbody);
            request.AddParameter("application/json", reqbody, ParameterType.RequestBody);
            IRestResponse responsecheck = client.Execute(request);
            try
            {
                WriteLog("Response Code " + responsecheck.StatusCode);
                WriteLog("Response " + responsecheck.Content);
            }
            catch { }
            return responsecheck;
        }
        public IRestResponse RadiantUPIStatus(string agentId, string clientId, string ClientSecret, string apiKey, string token, string refId)
        {
            WriteLog("********************* Radiant UPI Status  *****************************");
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string Baseurl = "https://aceneobank.com/apiService/";
            var client = new RestClient(Baseurl + "upiAtm/statusCheck");
            var request = new RestRequest(Method.POST);
            request.AddHeader("X-Root-id", agentId);
            request.AddHeader("clientId", clientId);
            request.AddHeader("ClientSecret", ClientSecret);
            // request.AddHeader("distributorCode", "70359687666670616055318500299916_Si6zudoNA+/VffLEesneSA==");
            request.AddHeader("apiKey", apiKey);
            request.AddHeader("Authorization", "Bearer " + token + "");
            request.AddHeader("content-type", "application/json");
            var body = new
            {
                refId,
            };

            var reqbody = JsonConvert.SerializeObject(body);
            WriteLog("Request " + reqbody);
            request.AddParameter("application/json", reqbody, ParameterType.RequestBody);
            IRestResponse responsecheck = client.Execute(request);
            try
            {
                WriteLog("Response Code " + responsecheck.StatusCode);
                WriteLog("Response " + responsecheck.Content);
            }
            catch { }
            return responsecheck;
        }
        public IRestResponse UPIIntentStatusCheck(string agent_id, string clientId, string ClientSecret, string apiKey, string token, string orderId)
        {
            WriteLog("********************* UPIIntentStatusCheck *****************************");
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string Baseurl = "https://aceneobank.com/apiService/";
            var client = new RestClient(Baseurl + "wallet/checkIntentStatus");
            var request = new RestRequest(Method.POST);
            request.AddHeader("X-Root-id", agent_id);
            request.AddHeader("clientId", clientId);
            request.AddHeader("ClientSecret", ClientSecret);
            // request.AddHeader("distributorCode", "70359687666670616055318500299916_Si6zudoNA+/VffLEesneSA==");
            request.AddHeader("apiKey", apiKey);
            request.AddHeader("Authorization", "Bearer " + token + "");
            request.AddHeader("content-type", "application/json");
            var body = new
            {
                agent_id,
                orderId,
                type = "intent",
            };
            var reqbody = JsonConvert.SerializeObject(body);
            WriteLog("Request " + reqbody);
            request.AddParameter("application/json", reqbody, ParameterType.RequestBody);
            IRestResponse responsecheck = client.Execute(request);
            try
            {
                WriteLog("Response Code " + responsecheck.StatusCode);
                WriteLog("Response " + responsecheck.Content);
            }
            catch { }
            return responsecheck;
        }
        public IRestResponse UPICollectionStatusCheck(string agent_id, string clientId, string ClientSecret, string apiKey, string token, string orderId)
        {
            WriteLog("********************* UPICollectionStatusCheck *****************************");
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string Baseurl = "https://aceneobank.com/apiService/";
            var client = new RestClient(Baseurl + "wallet/checkCollectStatus");
            var request = new RestRequest(Method.POST);
            request.AddHeader("X-Root-id", agent_id);
            request.AddHeader("clientId", clientId);
            request.AddHeader("ClientSecret", ClientSecret);
            // request.AddHeader("distributorCode", "70359687666670616055318500299916_Si6zudoNA+/VffLEesneSA==");
            request.AddHeader("apiKey", apiKey);
            WriteLog("Header " + "X-Root-id" + agent_id);
            WriteLog("Header " + "clientId" + clientId);
            WriteLog("Header " + "ClientSecret" + ClientSecret);
            WriteLog("Header " + "apiKey" + apiKey);
            request.AddHeader("Authorization", "Bearer " + token + "");
            request.AddHeader("content-type", "application/json");
            var body = new
            {
                agent_id,
                orderId,
                type = "collect",
            };
            var reqbody = JsonConvert.SerializeObject(body);
            WriteLog("Request " + reqbody);
            request.AddParameter("application/json", reqbody, ParameterType.RequestBody);
            IRestResponse responsecheck = client.Execute(request);
            try
            {
                WriteLog("Response Code " + responsecheck.StatusCode);
                WriteLog("Response " + responsecheck.Content);
            }
            catch { }
            return responsecheck;
        }
        public IRestResponse UPIATMstatusCheck(string agentId, string clientId, string ClientSecret, string apiKey, string token, string refId)
        {
            WriteLog("********************* UPIsendOTP *****************************");
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string Baseurl = "https://aceneobank.com/apiService/";
            var client = new RestClient(Baseurl + "upiAtm/statusCheck");
            var request = new RestRequest(Method.POST);
            request.AddHeader("X-Root-id", agentId);
            request.AddHeader("clientId", clientId);
            request.AddHeader("ClientSecret", ClientSecret);
            // request.AddHeader("distributorCode", "70359687666670616055318500299916_Si6zudoNA+/VffLEesneSA==");
            request.AddHeader("apiKey", apiKey);
            request.AddHeader("Authorization", "Bearer " + token + "");
            request.AddHeader("content-type", "application/json");
            var body = new
            {
                refId,
            };
            var reqbody = JsonConvert.SerializeObject(body);
            //WriteLog("Request " + reqbody);
            request.AddParameter("application/json", reqbody, ParameterType.RequestBody);
            IRestResponse responsecheck = client.Execute(request);
            try
            {
                WriteLog("Response Code " + responsecheck.StatusCode);
                WriteLog("Response " + responsecheck.Content);
            }
            catch { }
            return responsecheck;
        }
        public IRestResponse UPIIntent(string agentId, string clientId, string ClientSecret, string apiKey, string token, string orderId)
        {
            WriteLog("********************* UPIIntent *****************************");
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string Baseurl = "https://aceneobank.com/apiService/";
            var client = new RestClient(Baseurl + "wallet/intent/payOrder");
            var request = new RestRequest(Method.POST);
            request.AddHeader("X-Root-id", agentId);
            request.AddHeader("clientId", clientId);
            request.AddHeader("ClientSecret", ClientSecret);
            // request.AddHeader("distributorCode", "70359687666670616055318500299916_Si6zudoNA+/VffLEesneSA==");
            request.AddHeader("apiKey", apiKey);
            request.AddHeader("Authorization", "Bearer " + token + "");
            request.AddHeader("content-type", "application/json");
            var body = new
            {
                orderId,
                agent_id=agentId
            };
            var reqbody = JsonConvert.SerializeObject(body);
            //WriteLog("Request " + reqbody);
            request.AddParameter("application/json", reqbody, ParameterType.RequestBody);
            IRestResponse responsecheck = client.Execute(request);
            try
            {
                WriteLog("Response Code " + responsecheck.StatusCode);
                WriteLog("Response " + responsecheck.Content);
            }
            catch { }
            return responsecheck;
        }


        public IRestResponse collectPayVerify(string agent_id, string clientId, string ClientSecret, string apiKey, string token, string payerVpa)
        {

            WriteLog("********************* collectPayVerify *****************************");
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string Baseurl = "https://aceneobank.com/apiService/";
            var client = new RestClient(Baseurl + "wallet/upiVerify");
            var request = new RestRequest(Method.POST);
            request.AddHeader("X-Root-id", agent_id);
            request.AddHeader("clientId", clientId);
            request.AddHeader("ClientSecret", ClientSecret);
            // request.AddHeader("distributorCode", "70359687666670616055318500299916_Si6zudoNA+/VffLEesneSA==");
            request.AddHeader("apiKey", apiKey);
            request.AddHeader("Authorization", "Bearer " + token + "");
            request.AddHeader("content-type", "application/json");
            var body = new
            {
                agent_id,
                payerVpa
            };
            var reqbody = JsonConvert.SerializeObject(body);
            WriteLog("Request " + reqbody);
            request.AddParameter("application/json", reqbody, ParameterType.RequestBody);
            IRestResponse responsecheck = client.Execute(request);
            try
            {
                WriteLog("Response Code " + responsecheck.StatusCode);
                WriteLog("Response " + responsecheck.Content);
            }
            catch { }
            return responsecheck;


        }

        public static void WriteLog(string strMessage)
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
                    logFilePath = logFilePath + "Radiant WEB-" + name + " -" + DateTime.Today.ToString("MM-dd-yyyy") + "." + "txt";
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
}