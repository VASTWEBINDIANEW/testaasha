using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.DEALER.Models
{
    public class DayBookdealerreport
    {
        public IEnumerable<Vastwebmulti.Models.daybook_dealer_report_Result> DayBookLive { get; set; }
        public IEnumerable<Vastwebmulti.Models.daybook_dealer_old_report_Result> DayBook_Old { get; set; }
    }
}