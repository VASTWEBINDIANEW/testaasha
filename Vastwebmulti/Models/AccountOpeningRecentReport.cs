using System.Collections.Generic;

namespace Vastwebmulti.Models
{
    public class AccountOpeningRecentReport
    {
        public IEnumerable<AccountOpening> Recent_report_axis { get; set; }
        public IEnumerable<Vastwebmulti.Models.AEPS_TXN_Details> Recent_report_icici { get; set; }
    }
}