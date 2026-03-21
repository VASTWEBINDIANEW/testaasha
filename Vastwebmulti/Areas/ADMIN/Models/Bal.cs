using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    public class Bal
    {
        public IEnumerable<Vastwebmulti.Models.operator_wise_lapu_bal_report_Result> Alllapu { get; set; }
        public IEnumerable<Vastwebmulti.Models.all_lapu_bal_Result> singlelapu { get; set; }
    }
}