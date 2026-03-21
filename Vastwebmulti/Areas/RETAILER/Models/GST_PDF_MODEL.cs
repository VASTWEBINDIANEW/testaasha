using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.RETAILER.Models
{
    public class GST_PDF_MODEL
    {
        public IEnumerable<Vastwebmulti.Models.GST_Report_API_Result> APIGst { get; set; }
        public IEnumerable<Vastwebmulti.Models.GST_Report_Retailer_Result> RetailerGst { get; set; }
        public IEnumerable<Vastwebmulti.Models.GST_Report_Dealer_Result> DealerGst { get; set; }
        public IEnumerable<Vastwebmulti.Models.GST_Report_MD_Result> MdGst { get; set; }
        public String INVOICENO { get; set; }
    }
}