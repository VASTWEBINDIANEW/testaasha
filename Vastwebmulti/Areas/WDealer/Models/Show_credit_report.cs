using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.WDealer.Models
{
  
    public class wdealer_show_cr_report_details
    {
        public List<Show_credit_report> showdetails { get; set; }
    }
    public class Show_credit_report
    {
        public int idno { get; set; }
        public string EmailId { get; set; }
        public decimal? balance { get; set; }

        public decimal? dealerpre { get; set; }
        public decimal? dealerpost { get; set; }
        public string bal_type { get; set; }

        public decimal? cr { get; set; }

        public decimal? oldcrbalance { get; set; }

        public DateTime? transfer_date { get; set; }

    }
}