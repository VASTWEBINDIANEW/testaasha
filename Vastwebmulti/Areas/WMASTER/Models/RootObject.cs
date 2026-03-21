using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.WMASTER.Models
{
    public class RootObject
    {
        public string Role { get; set; }
        public decimal? Amount { get; set; }
        public string Type { get; set; }
        public string TotalSuccess { get; set; }
        public string TotalPending { get; set; }
        public string TotalFailed { get; set; }
    }
}