using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.WHITELABEL.Models
{
    public class DashBoard
    {
        public Nullable<decimal> Virtualbalance { get; set; }
        public Nullable<decimal> RealBalance { get; set; }
        public decimal DealerBalance { get; set; }
        public decimal retailerBalance { get; set; }
        public Nullable<decimal> diff { get; set; }
        public IEnumerable<DashBoard> DashBoard1 { set; get; }
    }

}