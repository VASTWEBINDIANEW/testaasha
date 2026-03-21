using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    /// <summary>
    /// Model for storing scheduled automatic fund transfer configuration details.
    /// </summary>
    public class SchduleFundInfo
    {
        public IEnumerable<schdule_fund> infoaccount_Schdule { get; set; }
        public string msg { get; set; }
    }
}