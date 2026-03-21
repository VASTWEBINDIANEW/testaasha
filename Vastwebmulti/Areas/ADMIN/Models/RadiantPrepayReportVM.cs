using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    /// <summary>
    /// View model for displaying Radiant prepay transaction report data.
    /// </summary>
    public class RadiantPrepayReportVM
    {
        public string Firmname { get; set; }
        public string UserId { get; set; }
        public string CEID { get; set; }
        public decimal? Amount { get; set; }
        public decimal? RemainPre { get; set; }
        public decimal? RemainPost { get; set; }
        public DateTime? InsertDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string Status { get; set; }
        public decimal? AdminRemainPre { get; set; }
        public decimal? AdminRemainPost { get; set; }
        public string RequestID { get; set; }

        public string RetailerName { get; set; }
        public string Mobile { get; set; }
    }
}