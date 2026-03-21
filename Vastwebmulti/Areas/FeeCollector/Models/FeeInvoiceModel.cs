using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.FeeCollector.Models
{
    public class FeeInvoiceModel
    {
        public Student student { get; set; }
        public FeeCollector_details feecollcector { get; set; }
        public List<proc_getFeedepositReceiptDetails_Result> lstReceiptItems{ get; set; }
    }
}