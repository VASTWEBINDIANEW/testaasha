using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.RETAILER.Models
{
    public class Transmsg
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public string BankRRN { get; set; }
        public decimal Amount { get; set; }
    }
}