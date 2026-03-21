using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.RETAILER.Models
{
    public class Daybookretailerrreport
    {
        public IEnumerable<Vastwebmulti.Models.Retailer_daybook_report_live_Result> Daybooklive { get; set; }
        public IEnumerable<Vastwebmulti.Models.Retailer_daybook_report_Old_Result> DayboolOLD { get; set; }
    }
}