using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    public class whatsapppurinfo
    {
        public string apiid { get; set; }
        public string farmname { get; set; }
        public string planname { get; set; }
        public decimal? planprice { get; set; }
        public decimal? gst { get; set; }
        public decimal? remainpre { get; set; }
        public decimal? remainpost { get; set; }
        public DateTime? purchasedate { get; set; }
        public DateTime? renewaldate { get; set; }
        public string status { get; set; }
    }
}