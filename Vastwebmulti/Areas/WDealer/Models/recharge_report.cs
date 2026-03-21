using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.WDealer.Models
{
    public class recharge_report
    {
        public IEnumerable<Vastwebmulti.Models.whitelabel_Recharge_Report_live_Result> rechargereport { get; set; }
        public IEnumerable<Vastwebmulti.Models.whitelabel_Recharge_Report_old_Result> rechargereport_old { get; set; }
    }
}