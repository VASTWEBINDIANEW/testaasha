using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    /// <summary>
    /// View model for displaying comprehensive money transfer reports across all channels.
    /// </summary>
    public class Money_transfer_all
    {
  
        public IEnumerable<Vastwebmulti.Models.money_transfer_report_paging_Result> money_transfer_report_paging { get; set; }
        public IEnumerable<Vastwebmulti.Models.money_transfer_report_Result> money_transfer_report { get; set; }
        public IEnumerable<Vastwebmulti.Models.money_transfer_report_old_Result> money_transfer_old_report { get; set; }
    }
}