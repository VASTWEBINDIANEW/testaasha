using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.RETAILER.Models
{
    public class operatorlist
    {
        public decimal? comm { get; set; }
        public string img { get; set; }
        public string operator_Name { get; set; }
        public string opertor_type { get; set; }
        public DateTime? blocktime { get; set; }
       
        public List<operatorlist> optlist { get; set; }
    }
}