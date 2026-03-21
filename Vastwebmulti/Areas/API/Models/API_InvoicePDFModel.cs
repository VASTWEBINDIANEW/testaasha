using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace Vastwebmulti.Areas.API.Models
{
    public class API_InvoicePDFModel
    {
        public string Amount { get; set; }
        public string AmountPre { get; set; }
        public string AmountPost { get; set; }
        public string Date { get; set; }
    }
}