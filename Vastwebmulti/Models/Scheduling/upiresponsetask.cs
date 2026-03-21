using System;
using Quartz;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Vastwebmulti.Areas.RETAILER.Controllers;
namespace Vastwebmulti.Models.Scheduling
{
    public class upiresponsetask : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            var task = Task.Run(() =>
            {
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    var paytmlist = db.Upi_txn_details.Where(s => s.AppName == "Paytm" && s.status.ToUpper() == "PENDING").ToList();
                    var phonepelist = db.Upi_txn_details.Where(s => s.AppName == "Phonepe" && s.status.ToUpper() == "PENDING").ToList();
                    Vastwebmulti.Areas.RETAILER.Controllers.HomeController d2 = new HomeController();
                    foreach (var i in paytmlist)
                    {
                       
                        dynamic s = d2.Paytmqrresponse(i.refid);
                        var jsonData = s.Data;
                        WriteLogUPI("Response Time: " + DateTime.Now + "\n" + "Response: " + jsonData);
                    }   

                    foreach(var i in phonepelist)
                    {
                        dynamic s = d2.phonepeqrresponse(i.refid);
                        var jsonData = s.Data;
                        WriteLogUPI("Response Time: " + DateTime.Now + "\n" + "Response: " + jsonData);
                    }
               }

            });
            return task;
        }
        public static void WriteLogUPI(string strMessage)
        {
            try
            {
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    var WebsiteName = db.Admin_details.FirstOrDefault().Companyname.ToUpper();
                    StreamWriter log;
                    FileStream fileStream = null;
                    DirectoryInfo logDirInfo = null;
                    FileInfo logFileInfo;
                    string logFilePath = "C:\\Logs\\";
                    logFilePath = logFilePath + "SchedulingResponse-" + WebsiteName + "-" + DateTime.Today.ToString("MM-dd-yyyy") + "." + "txt";
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
            }
            catch (Exception ex)
            {

            }
        }
    }
}