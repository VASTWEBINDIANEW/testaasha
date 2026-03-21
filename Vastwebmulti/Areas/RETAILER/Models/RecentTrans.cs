using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.RETAILER.Models
{
    public class RecentTrans
    {
     public List<Vastwebmulti.Models.Retailer_Cr_Dr_Report_Result> Retailer_Cr_Dr_Report { set; get; }
     public List<Vastwebmulti.Models.recent_Retailer_Result> RecentReport { set; get; }
    }
}