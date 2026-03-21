using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    /// <summary>
    /// View model for displaying auto credit fund transfer reports.
    /// </summary>
    public class FundAutoTransferModel
    {
        public IEnumerable<Autocredittransferreports_Result> autocredittransferreports { get; set; }
    }
}