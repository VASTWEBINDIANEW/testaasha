using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.MASTER.Models
{
    public class Daybookmasterreport
    {
        public IEnumerable<Vastwebmulti.Models.daybook_master_report_Result> DaybookLive { get; set; }
        public IEnumerable<Vastwebmulti.Models.daybook_master_old_report_Result> DaybookOld { get; set; }
    }
}