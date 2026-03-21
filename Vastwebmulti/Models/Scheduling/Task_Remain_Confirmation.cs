using Quartz;
using RestSharp;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Vastwebmulti.Models.Scheduling
{
    public class Task_Remain_Confirmation : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            var task = Task.Run(() =>
            {
                try
                {
                    using (VastwebmultiEntities db = new VastwebmultiEntities())
                    {
                        var virtualremain = db.Remain_Admin_balance.SingleOrDefault().VirtualAmount;
                        var totalremain = db.account_chk.OrderByDescending(aa => aa.tdate).FirstOrDefault().total;
                        if (totalremain != virtualremain)
                        {
                            var admininfo = db.Admin_details.SingleOrDefault().WebsiteUrl;
                            var mobile = "8696690102";
                            var message = admininfo + " Total Balance Disturb , Current Total Balance is " + totalremain;
                            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                            var client = new RestClient("https://www.vastbazaar.com/Response/MessageSend?mobile=" + mobile + "&Whatsappmsg=" + message + "");
                            client.Timeout = -1;
                            var request = new RestRequest(Method.GET);
                            IRestResponse response = client.Execute(request);
                        }
                    }
                }
                catch { }
            });
            return task;
        }
    }
}