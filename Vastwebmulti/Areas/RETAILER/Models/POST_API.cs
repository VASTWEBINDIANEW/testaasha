using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace Vastwebmulti.Areas.RETAILER.Models
{
    public class POST_API
    {
        public IRestResponse RchReq(string token, string Number, string Tokenid, string Userid, decimal ammt, string apioptcode, string CommonTranid, string optional1, string optional2, string url)
        {
            var client = new RestClient(url);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var request = new RestRequest(Method.POST);
            request.AddHeader("authorization", "bearer " + token + "");
            //request.AddHeader("cache-control", "no-cache");
            request.AddHeader("Content-Type", "application/json");
            var req = new
            {
                Customernumber = Number,
                Tokenid = Tokenid,
                Userid = Userid,
                Amount = ammt,
                Optcode = apioptcode,
                Yourrchid = CommonTranid,
                optional1 = optional1,
                optional2 = optional2
            };
            var jsonchk = JsonConvert.SerializeObject(req);
            dynamic vhhh = JsonConvert.DeserializeObject(jsonchk);
            request.AddParameter("application/json", vhhh, ParameterType.RequestBody);

            try
            {
                WriteLog("APIREQUEST", "Request json" + jsonchk);
            }
            catch { }
            IRestResponse GetResponse = client.Execute(request);
            return GetResponse;
        }
        public IRestResponse RchReqMrobotics(string token, string Number, string Tokenid, string Userid, decimal ammt, string apioptcode, string CommonTranid, string optional1, string optional2, string Special_OPT, string url)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var client = new RestClient(url);
            var request = new RestRequest(Method.POST);
            //request.AddHeader("Postman-Token", "716bfc7e-3f1c-4a99-8e1a-c55a99c471fc");
            //request.AddHeader("cache-control", "no-cache");
            request.AddHeader("Content-Type", "application/json");

            if (url.ToUpper().Contains("MROBOTICS.IN/API/RECHARGE_STATEWISE"))
            {
                if (Special_OPT == "BR" || Special_OPT == "TS" || Special_OPT == "JIOPR")
                {
                    request.AddParameter("application/json", "{\r\n    \"api_token\": \"" + token + "\",\r\n    \"amount\": \"" + ammt + "\",\r\n    \"mobile_no\": \"" + Number + "\",\r\n    \"company_id\": \"" + apioptcode + "\",\r\n    \"order_id\": \"" + CommonTranid + "\",\r\n    \"is_stv\": \"true\",\r\n    \"no_roaming\": \"false\",\r\n    \"state_code\": \"\"\r\n}", ParameterType.RequestBody);
                }
                else
                {
                    if (Special_OPT == "B" && (ammt == 29 || ammt == 74 || ammt == 75 || ammt == 99 || ammt == 118 || ammt == 187))
                    {
                        request.AddParameter("application/json", "{\r\n    \"api_token\": \"" + token + "\",\r\n    \"amount\": \"" + ammt + "\",\r\n    \"mobile_no\": \"" + Number + "\",\r\n    \"company_id\": \"" + apioptcode + "\",\r\n    \"order_id\": \"" + CommonTranid + "\",\r\n    \"is_stv\": \"true\",\r\n    \"no_roaming\": \"false\",\r\n    \"state_code\": \"\"\r\n}", ParameterType.RequestBody);
                    }
                    else
                    {
                        request.AddParameter("application/json", "{\r\n    \"api_token\": \"" + token + "\",\r\n    \"amount\": \"" + ammt + "\",\r\n    \"mobile_no\": \"" + Number + "\",\r\n    \"company_id\": \"" + apioptcode + "\",\r\n    \"order_id\": \"" + CommonTranid + "\",\r\n    \"is_stv\": \"false\",\r\n    \"no_roaming\": \"false\",\r\n    \"state_code\": \"\"\r\n}", ParameterType.RequestBody);
                    }

                }
            }
            else if (url.ToUpper().Contains("MROBOTICS.IN/API/RECHARGE"))
            {
                if (Special_OPT == "BR" || Special_OPT == "TS" || Special_OPT == "JIOPR")
                {
                    var req = new
                    {
                        api_token = token,
                        amount = ammt,
                        mobile_no = Number,
                        company_id = apioptcode,
                        order_id = CommonTranid,
                        is_stv = "true"
                    };
                    var jsonchk = JsonConvert.SerializeObject(req);
                    dynamic vhhh = JsonConvert.DeserializeObject(jsonchk);
                    request.AddParameter("application/json", vhhh, ParameterType.RequestBody);

                    try
                    {
                        WriteLog("APIREQUEST", "Request json" + jsonchk);
                    }
                    catch { }
                }
                else
                {
                    if (Special_OPT == "B" && (ammt == 29 || ammt == 74 || ammt == 75 || ammt == 99 || ammt == 118 || ammt == 187))
                    {
                        var req = new
                        {
                            api_token = token,
                            amount = ammt,
                            mobile_no = Number,
                            company_id = apioptcode,
                            order_id = CommonTranid,
                            is_stv = "true"
                        };
                        var jsonchk = JsonConvert.SerializeObject(req);
                        dynamic vhhh = JsonConvert.DeserializeObject(jsonchk);
                        request.AddParameter("application/json", vhhh, ParameterType.RequestBody);

                        try
                        {
                            WriteLog("APIREQUEST", "Request json" + jsonchk);
                        }
                        catch { }
                    }
                    else
                    {
                        var req = new
                        {
                            api_token = token,
                            amount = ammt,
                            mobile_no = Number,
                            company_id = apioptcode,
                            order_id = CommonTranid,
                            is_stv = "false"
                        };
                        var jsonchk = JsonConvert.SerializeObject(req);
                        dynamic vhhh = JsonConvert.DeserializeObject(jsonchk);
                        request.AddParameter("application/json", vhhh, ParameterType.RequestBody);

                        try
                        {
                            WriteLog("APIREQUEST", "Request json" + jsonchk);
                        }
                        catch { }
                    }
                }
            }
            else if (url.ToUpper().Contains("MROBOTICS.IN/API/MULTIRECHARGE"))
            {
                var subcompany_id = 15;

                if (Special_OPT == "BR" || Special_OPT == "TS" || Special_OPT == "JIOPR")
                {
                    var req = new
                    {
                        api_token = token,
                        amount = ammt,
                        mobile_no = Number,
                        company_id = subcompany_id,
                        subcompany_id = apioptcode,
                        order_id = CommonTranid,
                        is_stv = "true"
                    };
                    var jsonchk = JsonConvert.SerializeObject(req);
                    dynamic vhhh = JsonConvert.DeserializeObject(jsonchk);
                    request.AddParameter("application/json", vhhh, ParameterType.RequestBody);

                    try
                    {
                        WriteLog("APIREQUEST", "Request json" + jsonchk);
                    }
                    catch { }
                }
                else
                {
                    var req = new
                    {
                        api_token = token,
                        amount = ammt,
                        mobile_no = Number,
                        company_id = subcompany_id,
                        subcompany_id = apioptcode,
                        order_id = CommonTranid,
                        is_stv = "false"
                    };
                    var jsonchk = JsonConvert.SerializeObject(req);
                    dynamic vhhh = JsonConvert.DeserializeObject(jsonchk);
                    request.AddParameter("application/json", vhhh, ParameterType.RequestBody);

                    try
                    {
                        WriteLog("APIREQUEST", "Request json" + jsonchk);
                    }
                    catch { }
                }

            }

            IRestResponse GetResponse = client.Execute(request);
            return GetResponse;
        }
        private static Random random = new Random();
        public string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public static void WriteLog(string strFileName, string strMessage)
        {
            try
            {
                StreamWriter log;
                FileStream fileStream = null;
                DirectoryInfo logDirInfo = null;
                FileInfo logFileInfo;
                string logFilePath = "C:\\Logs\\";
                logFilePath = logFilePath + "Recharge_Log-" + System.DateTime.Today.ToString("MM-dd-yyyy") + "." + "txt";
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
            {

            }
        }
    }
}