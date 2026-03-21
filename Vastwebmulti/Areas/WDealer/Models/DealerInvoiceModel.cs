using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.WDealer.Models
{
    public class DealerInvoiceModel
    {
        public string From
        {
            set;
            get;
        }
        public string Value { get; set; }
        public string DistOldBal { get; set; }
        public string DistNewBal { get; set; }
        public string Date { get; set; }
    }
}