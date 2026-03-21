using Newtonsoft.Json;
using Quartz;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace Vastwebmulti.Models.Scheduling
{
    public class Phonepe : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            var task = Task.Run(() =>
            {
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    var infochk = db.phonepestatusinfo().ToList();
                    foreach (var item in infochk)
                    {
                        var uniqueid = item.txnid;
                        var phonepeauth = db.phonepeauths.SingleOrDefault();
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                        var Phonepeurl = "https://api.phonepe.com/apis/hermes/pg/v1/status/" + phonepeauth.merchantid + "/" + uniqueid + "";
                        WriteLogPHONEPE("merchantid " + phonepeauth.merchantid);
                        WriteLogPHONEPE("uniqueid " + uniqueid);
                        WriteLogPHONEPE("status check API URL " + Phonepeurl);
                        //    var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(req);
                        //   var base64encode = System.Convert.ToBase64String(plainTextBytes);
                        WriteLogPHONEPE("xverify before " + "/pg/v1/status/" + phonepeauth.merchantid + "/" + uniqueid + "" + phonepeauth.secretkey);
                        var xverify = ComputeStringToSha256Hash("/pg/v1/status/" + phonepeauth.merchantid + "/" + uniqueid + "" + phonepeauth.secretkey);
                        xverify = xverify + "###" + phonepeauth.key1;
                        WriteLogPHONEPE("status check xverify " + xverify);
                        var client = new RestClient(Phonepeurl);
                        var request = new RestRequest(Method.GET);
                        WriteLogPHONEPE("Method Type POST ");
                        request.AddHeader("Content-Type", "application/json");
                        request.AddHeader("X-VERIFY", xverify);
                        request.AddHeader("X-MERCHANT-ID", phonepeauth.merchantid);
                        IRestResponse response = client.Execute(request);
                        WriteLogPHONEPE("status status Code " + response.StatusCode);
                        WriteLogPHONEPE("status check Response " + response.Content);
                        string repp = response.Content;
                        dynamic respdata = JsonConvert.DeserializeObject(response.Content);
                        try
                        {
                            string status = respdata.code;
                            string BankRRN = ""; string upi_id = "";
                            string custid = respdata.data.merchantTransactionId;
                            string amt = respdata.data.amount;
                            try
                            {
                                BankRRN = respdata.data.paymentInstrument.utr;
                            }
                            catch
                            {
                                BankRRN = custid;
                            }
                            try
                            {
                                upi_id = respdata.data.paymentInstrument.payerVpa;
                            }
                            catch
                            { }

                            var exitsup = db.Upi_txn_details.Where(a => a.refid == custid).SingleOrDefault();
                            if (exitsup == null)
                            {
                                decimal amount = Convert.ToDecimal(amt);
                                amount = amount / 100;
                                if (status == "PAYMENT_SUCCESS")
                                {
                                    status = "SUCCESS";
                                }
                                else if (status == "PAYMENT_ERROR" || status == "TIMED_OUT" || status == "PAYMENT_DECLINED")
                                {
                                    status = "Failed";
                                }
                                else
                                {
                                    status = "Pending";
                                }
                                var count = db.phonepepayments.Where(aa => aa.txnid == custid).FirstOrDefault();
                                if (count != null)
                                {
                                    System.Data.Entity.Core.Objects.ObjectParameter output = new
                                          System.Data.Entity.Core.Objects.ObjectParameter("Output", typeof(string));

                                    //var chkinfo=dbsrs.upi
                                    db.UPI_TXN("Retailer", count.userid, custid, amount, status, "", upi_id, BankRRN, repp, "PHONEPE", output).SingleOrDefault();
                                    try
                                    {
                                        var retailerdetails = db.Retailer_Details.Where(aa => aa.RetailerId == count.userid).SingleOrDefault();
                                        
                                        var remdetails = db.Remain_reteller_balance.Where(aa => aa.RetellerId == count.userid).SingleOrDefault();
                                       
                                        var admininfo = db.Admin_details.SingleOrDefault();
                                        Backupinfo back = new Backupinfo();
                                        var model = new Backupinfo.Addinfo
                                        {
                                            Websitename = admininfo.WebsiteUrl,
                                            RetailerID = count.userid,
                                            Email = retailerdetails.Email,
                                            Mobile = retailerdetails.Mobile,
                                            Details = "Phone pe Upi Fund Recived ",
                                            RemainBalance = (decimal)remdetails.Remainamount,
                                            Usertype = "Retailer"
                                        };
                                        back.Fundtransfer(model);

                                    }
                                    catch { }
                                    var retailer = db.Retailer_Details.Where(aa => aa.RetailerId == count.userid).SingleOrDefault();
                                    var newremain = db.Remain_reteller_balance.Where(aa => aa.RetellerId == count.userid).SingleOrDefault().Remainamount.ToString();
                                    var AdminDetails = db.Admin_details.SingleOrDefault();

                                    //try
                                    //{
                                    //    if (status == "SUCCESS")
                                    //    {
                                    //        smssend.sms_init("Y", "Y", "UPITRANSFERSUCCESSFULLY", retailer.Mobile, amount, newremain);

                                    //    }
                                    //    else if (status == "Failed")
                                    //    {
                                    //        smssend.sms_init("Y", "Y", "UPITRANSFERFAILED", retailer.Mobile, amount, newremain);
                                    //    }
                                    //}
                                    //catch { }
                                    //try
                                    //{
                                    //    if (status == "SUCCESS")
                                    //    {
                                    //        smssend.SendEmailAll(retailer.Email, "UPI Transfer Rs." + amount + " Successfull .New Balance is " + newremain + "", "UPI TRANSFER", AdminDetails.email);

                                    //    }
                                    //    else if (status == "Failed")
                                    //    {
                                    //        smssend.SendEmailAll(retailer.Email, "UPI Transfer Rs." + amount + " Failed .New Balance is " + newremain + "", "UPI TRANSFER", AdminDetails.email);
                                    //    }
                                    //}
                                    //catch { }
                                }
                            }
                        }
                        catch { }
                        var resp = new
                        {
                            Status = "Hit SuccessFully",
                        };

                    }
                }
            });
            return task;
        }
        public static void WriteLogPHONEPE(string strMessage)
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
                    logFilePath = logFilePath + "PhonePeAuto-" + name + " -" + DateTime.Today.ToString("MM-dd-yyyy") + "." + "txt";
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
        public static string ComputeStringToSha256Hash(string plainText)
        {
            // Create a SHA256 hash from string   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Computing Hash - returns here byte array
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(plainText));

                // now convert byte array to a string   
                StringBuilder stringbuilder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    stringbuilder.Append(bytes[i].ToString("x2"));
                }
                return stringbuilder.ToString();
            }
        }
    }
}