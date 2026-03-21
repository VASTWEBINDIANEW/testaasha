using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    /// <summary>
    /// View model for displaying recharge transaction reports in the admin panel.
    /// </summary>
    public class recharge_report
    {
        public IEnumerable<Vastwebmulti.Models.Recharge_Report_live_Result> rechargereport { get; set; }
        public IEnumerable<Vastwebmulti.Models.Recharge_Report_old_Result> recharge_reportold { get; set; }
    }
}