using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    /// <summary>
    /// Model for configuring or displaying other miscellaneous service offerings.
    /// </summary>
    public class otherservices
    {
        public string servicenm { get; set; }
        public decimal p1 { get; set; }
        public decimal p2 { get; set; }
        public decimal p3 { get; set; }
        public decimal t1 { get; set; }
        public decimal t2 { get; set; }
        public decimal t3 { get; set; }
    }
}