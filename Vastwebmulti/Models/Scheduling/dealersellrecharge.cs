using Quartz;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using Vastwebmulti.Models;

namespace Vastwebmulti.Models.Scheduling
{
    public class dealersellrecharge
    {

        public Task Execute(IJobExecutionContext context)
        {
            var task = Task.Run(() =>
            {
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    var date = DateTime.Now.Date;

                    var radiantresponse = db.Recharge_info.Where(s => s.Rch_time > date).ToList();
                };
            });
            return task;
        }
    }
}