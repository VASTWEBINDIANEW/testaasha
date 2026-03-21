using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    /// <summary>
    /// View model for displaying wallet unload (withdrawal) report data for admin review.
    /// </summary>
    public class WalletUnloadReport_VM
    {
        public IEnumerable<WalletUnloadReportAdmin_Result> WalletUnloadReportAdmin { get; set; }
    }
}