using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vastwebmulti.Areas.RETAILER.Models;

namespace Vastwebmulti.Areas.WRetailer.Models
{
    public class remmulti
    {
        //public IEnumerable<Vastwebmulti.Models.Select_Whitelabel_admin_report_history_Result> retailer_recharge { get; set; }
        //public IEnumerable<Vastwebmulti.Models.Select_whitelabel_admin_report_history_old_Result> retailer_recharge_old { get; set; }
        public IEnumerable<Vastwebmulti.Models.Message_top> messagetop { get; set; }
        public IEnumerable<Vastwebmulti.Models.Apps_opt_all_Result> allopt { get; set; }
        public IEnumerable<Vastwebmulti.Models.Operator_Code> optcode { get; set; }

        public IEnumerable<Vastwebmulti.Models.whitelabel_Recharge_Report_live_Result> retailer_rechargelive { get; set; }
        public IEnumerable<Vastwebmulti.Models.whitelabel_Recharge_Report_old_Result> retailer_rechargeold{ get; set; }
        public IEnumerable<operatorlist> optlist { get; set; }
        public IEnumerable<Vastwebmulti.Models.recent_recharge_report_Result> recent_rechargereport { get; set; }
    }
}