using Newtonsoft.Json;
using Quartz;
using RestSharp;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Vastwebmulti.Models.Scheduling
{
    public class Task_Delete_data : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            string IP = "";
            string chk = "1";
            try
            {
                IPAddressCheck ipchk = new IPAddressCheck();
                chk = ipchk.GetComputer_InternetIP();
                var hostname = Dns.GetHostName();
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        IP = ip.ToString();
                    }
                }
            }
            catch (Exception ex)
            {

            }
            var TaskServiceOnline = "ON";
            if (chk == IP)
            {
                TaskServiceOnline = "ON";
            }
            var task = Task.Run(() =>
            {
                if (TaskServiceOnline.Equals("ON"))
                {
                    WriteLog("Delete Start");
                    using (VastwebmultiEntities db = new VastwebmultiEntities())
                    {
                        // Safe null check to avoid NullReferenceException if admin record missing
                        var adminRec = db.Admin_details.FirstOrDefault();
                        var admininfo = adminRec != null ? adminRec.WebsiteUrl : "";
                        var client = new RestClient("http://api.vastbazaar.com/api/WEB/Planinfo?websitenm=" + admininfo + "");
                        //client.Timeout = -1;
                        var request = new RestRequest(Method.POST);
                        IRestResponse response = client.Execute(request);
                        WriteLog("Response " + response.Content);
                        dynamic resp = JsonConvert.DeserializeObject(response.Content);
                        var status = resp.Content.sts;
                        if (status == true)
                        {
                            int noofdays = 730;
                            var plannm = resp.Content.plnnm;
                            if (plannm == "days1")
                            {
                                noofdays = 40;
                            }
                            else if (plannm == "days2")
                            {
                                noofdays = 70;
                            }
                            else if (plannm == "days3")
                            {
                                noofdays = 100;
                            }
                            else if (plannm == "days4")
                            {
                                noofdays = 190;
                            }
                            else if (plannm == "days5")
                            {
                                noofdays = 370;
                            }
                            else
                            {
                                noofdays = 730;
                            }
                            WriteLog("No Of Days " + noofdays);
                            db.deletedata_auto(noofdays);
                            WriteLog("Delete Done ");
                        }
                    }
                }
            });
            return task;
        }
        public static void WriteLog(string strMessage)
        {
            try
            {
                StreamWriter log;
                FileStream fileStream = null;
                DirectoryInfo logDirInfo = null;
                FileInfo logFileInfo;
                string logFilePath = "C:\\Logs\\";
                logFilePath = logFilePath + "Sample3-" + System.DateTime.Today.ToString("MM-dd-yyyy") + "." + "txt";
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