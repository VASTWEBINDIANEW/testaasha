using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    public class AutoFund_Report
    {
        public IEnumerable<Auto_fund_transfer> info_auto_fund { get; set; }
        public string msg { get; set; }
    }
}