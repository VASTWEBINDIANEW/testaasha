using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    /// <summary>
    /// View model for displaying sales and commission income reports.
    /// </summary>
    public class salesincome
    {
        public IEnumerable<Vastwebmulti.Models.main_rch_total_Result> sales { get; set; }
    }
}