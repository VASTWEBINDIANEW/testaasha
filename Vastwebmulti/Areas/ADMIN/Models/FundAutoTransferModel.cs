using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    public class FundAutoTransferModel
    {
        public IEnumerable<Autocredittransferreports_Result> autocredittransferreports { get; set; }
    }
}