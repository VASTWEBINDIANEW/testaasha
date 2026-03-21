using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    /// <summary>
    /// Represents a day-wise commission slab entry for time-based commission rules.
    /// </summary>
    public class daywisecommslabclass
    {
        public int idno { get; set; }
        public string Role { get; set; }
        public Nullable<decimal> Comm_2000_5000 { get; set; }
        public Nullable<decimal> Comm_5001_10000 { get; set; }
        public Nullable<decimal> Comm_10001_max { get; set; }
    }
}