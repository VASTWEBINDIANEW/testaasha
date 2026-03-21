using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace Vastwebmulti.Areas.RETAILER.Models
{
    public class Instantpay_Payout
    {
        public static IRestResponse payout_pool(string name, string mode, string amount, string accountno, string ifsccode, string agentid, string email,string authcode,string clientid,string secret,string ipaddress,string accountnumber)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            var client = new RestClient("https://api.instantpay.in/payments/payout");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("X-Ipay-Auth-Code", authcode);
            request.AddHeader("X-Ipay-Client-Id", clientid);
            request.AddHeader("X-Ipay-Client-Secret", secret);
            request.AddHeader("X-Ipay-Endpoint-Ip", ipaddress);
            request.AddHeader("Content-Type", "application/json");

            var req = new
            {
                payer = new
                {
                    bankId = "0",
                    bankProfileId = "0",
                    accountNumber = accountnumber
                },
                payee = new
                {
                    name = name,
                    accountNumber = accountno,
                    bankIfsc = ifsccode
                },
                transferMode = mode,
                transferAmount = amount,
                externalRef = agentid,
                latitude = "20.5936",
                longitude = "78.9628",
                remarks = "PayOut Remark",
                alertEmail = email,
                purpose = "REIMBURSEMENT"
            };

            var body = JsonConvert.SerializeObject(req);

            request.AddParameter("application/json", body, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            try
            {
                InstantPayLog("------------ Start------------- " + DateTime.Now.ToString());
                InstantPayLog(" ");
                InstantPayLog("Request Body: " + body);
                InstantPayLog("Response Status: " + response.StatusCode.ToString());
                InstantPayLog("Response Content: " + response.Content);
                InstantPayLog(" ");
            }
            catch { }
            WriteLog("moneytransfer", "instantpay" + response.Content.ToString());
            return response;
        }


        public static IRestResponse payout_status(string transactionid, string Date,string authcode,string clientid, string secret,string ipaddress)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            var client = new RestClient("https://api.instantpay.in/reports/txnStatus");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("X-Ipay-Auth-Code", authcode);
            request.AddHeader("X-Ipay-Client-Id", clientid);
            request.AddHeader("X-Ipay-Client-Secret", secret);
            request.AddHeader("X-Ipay-Endpoint-Ip", ipaddress);
            request.AddHeader("Content-Type", "application/json");

            var req = new
            {
                transactionDate = Date,
                externalRef = transactionid,
                source = "ORDER"

            };

            var body = JsonConvert.SerializeObject(req);

            request.AddParameter("application/json", body, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            try
            {
                InstantPayLog("------------ Start------------- " + DateTime.Now.ToString());
                InstantPayLog(" ");
                InstantPayLog("Request Body: " + body);
                InstantPayLog("Response Status: " + response.StatusCode.ToString());
                InstantPayLog("Response Content: " + response.Content);
                InstantPayLog(" ");
            }
            catch { }
            WriteLog("moneytransfer", "instantpay_status" + response.Content.ToString());
            return response;
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
                logFilePath = logFilePath + "moneytransferlog__" + System.DateTime.Today.ToString("MM-dd-yyyy") + "." + "txt";
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
        public static void InstantPayLog(string strMessage, string filename = "INSTANTPAY_PAYOUT AEPSRECHARGE -")
        {
            try
            {
                StreamWriter log;
                FileStream fileStream = null;
                DirectoryInfo logDirInfo = null;
                FileInfo logFileInfo;

                string logFilePath = "C:\\Logs\\";
                logFilePath = logFilePath + filename + System.DateTime.Today.ToString("MM-dd-yyyy") + "." + "txt";
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
        public static IRestResponse bankvarificationinstantpay(string accountNumber, string bankIfsc,string authcode,string clientid,string secret,string ipaddress)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            var client = new RestClient("https://api.instantpay.in/identity/verifyBankAccount");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("X-Ipay-Auth-Code", authcode);
            request.AddHeader("X-Ipay-Client-Id", clientid);
            request.AddHeader("X-Ipay-Client-Secret", secret);
            request.AddHeader("X-Ipay-Endpoint-Ip", ipaddress);
            request.AddHeader("Content-Type", "application/json");

            var req = new
            {
                payee = new
                {
                    accountNumber = accountNumber,
                    bankIfsc = bankIfsc
                },
                externalRef = "PPT2",
                consent = "Y",
                isCached = "0",
                latitude = "20.5936",
                longitude = "78.9628"

            };

            var body = JsonConvert.SerializeObject(req);

            request.AddParameter("application/json", body, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            try
            {
                InstantPayLog("------------ Start------------- " + DateTime.Now.ToString());
                InstantPayLog(" ");
                InstantPayLog("Request Body: " + body);
                InstantPayLog("Response Status: " + response.StatusCode.ToString());
                InstantPayLog("Response Content: " + response.Content);
                InstantPayLog(" ");
            }
            catch { }
            WriteLog("moneytransfer", "instantpay_status" + response.Content.ToString());
            return response;
        }

    }
}