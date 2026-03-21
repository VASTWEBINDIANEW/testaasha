using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.WMASTER.Models
{
  
    public class DealerCreationTokenVM1
    {
        public int Idno { get; set; }
        public string Masterid { get; set; }
        public string Email { get; set; }
        public int Tokens { get; set; }
        public DateTime? CteatedOn { get; set; }
        public int pre { get; set; }
        public int post { get; set; }
        public decimal? MasterPre { set; get; }
        public decimal? MasterPost { set; get; }
        public decimal? AdminPre { set; get; }
        public decimal? AdminPost { set; get; }
        public decimal? PerTokenValue { set; get; }
        public decimal? TotalDebit { set; get; }

    }

}