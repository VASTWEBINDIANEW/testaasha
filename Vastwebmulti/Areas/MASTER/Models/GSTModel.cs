using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.MASTER.Models
{
    public class GSTModel
    {
        public int SrNo { get; set; }
        public string Description { get; set; }
        public string HSN { get; set; }
        public decimal TaxableValue { get; set; }
        public decimal ActualValue { get; set; }
        public decimal GST_Rat { get; set; }
        public decimal CGST { get; set; }
        public decimal IGST { get; set; }
        public decimal SGST { get; set; }
        public bool StateFlag { get; set; }

    }
}