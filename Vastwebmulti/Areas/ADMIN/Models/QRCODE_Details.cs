using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    public class QRCODE_Details
    {
        public IEnumerable<details> QRDetails { get; set; }
        public string logoimage { get; set; }
    }
    public class details
    {
        public string userid { get; set; }
        public string firmname { get; set; }
        public string Mobile { get; set; }
        public string QRCODE { get; set; }
        public string UpiTxnDetails { get; set; }
        public string MerchantVPA { get; set; }
      

    }
}