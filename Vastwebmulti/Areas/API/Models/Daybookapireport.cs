using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.API.Models
{
    public class Daybookapireport
    {
        public IEnumerable<Vastwebmulti.Models.daybook_api_report_Result> DaybookLive { get; set; }
        public IEnumerable<Vastwebmulti.Models.daybook_api_old_report_Result> Daybook_Old { get; set; }
    }
}