using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    /// <summary>
    /// View model for displaying day book report data in the admin panel.
    /// </summary>
    public class DayBookreport
    {
        public IEnumerable<Vastwebmulti.Models.daybook_Result> daybook { get; set; }
        public IEnumerable<Vastwebmulti.Models.daybook_old_Result> daybook_old { get; set; }
        public IEnumerable<Vastwebmulti.Models.daybook_api_Result> daybook_api { get; set; }
        public IEnumerable<Vastwebmulti.Models.daybook_api_old_Result> daybook_api_old { get; set; }
        public IEnumerable<Vastwebmulti.Models.daybook_dealer_Result> daybook_dealer { get; set; }
        public IEnumerable<Vastwebmulti.Models.daybook_dealer_old_Result> daybook_dealer_old { get; set; }
        public IEnumerable<Vastwebmulti.Models.daybook_master_Result> daybook_master { get; set; }
        public IEnumerable<Vastwebmulti.Models.daybook_master_old_Result> daybook_master_old { get; set; }
        public IEnumerable<Vastwebmulti.Models.daybook_whitelabel_Result> daybook_whitelabel { get; set; }
        public IEnumerable<Vastwebmulti.Models.daybook_whitelabel_old_Result> daybook_whitelabel_old { get; set; }
    }
}