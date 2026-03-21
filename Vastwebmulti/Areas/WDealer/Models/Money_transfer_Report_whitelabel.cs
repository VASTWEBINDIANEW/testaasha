using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.WDealer.Models
{
    public class Money_transfer_Report_whitelabel
    {
        public IEnumerable<Vastwebmulti.Models.whitelabel_money_transfer_report_Result> Money_live { get; set; }
        public IEnumerable<Vastwebmulti.Models.whitelabel_money_transfer_report_old_Result> Money_old { get; set; }
    }
}