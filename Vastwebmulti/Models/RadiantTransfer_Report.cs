using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//namespace Vastwebmulti.Models
//{
//    internal interface RadiantTransfer_Report
//    {
//    }
//}



namespace Vastwebmulti.Models
{
    public class RadiantTransfer_Report
    {
        public int idno { get; set; }
        public string Userid { get; set; }
        public string CEID { get; set; }
        public decimal amount { get; set; }
        public decimal Remainpre { get; set; }
        public decimal Remainpost { get; set; }
        public DateTime? Insertdate { get; set; }
        public DateTime? Responsedate { get; set; }
        public string Sts { get; set; }
        public decimal Adminremainpre { get; set; }
        public decimal Adminremainpost { get; set; }
        public string RequestID { get; set; }

        // ✅ ADD THESE TWO PROPERTIES
        public string RetailerName { get; set; }
        public string Frm_Name { get; set; }
    }
}


