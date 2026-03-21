using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.RETAILER.Models
{
    public class Money_transfer_report
    {
        public IEnumerable<Vastwebmulti.Models.money_transfer_report_Result> Money_report_live { get; set; }
        public IEnumerable<Vastwebmulti.Models.money_transfer_report_old_Result> Money_report_old { get; set; }
    }
}