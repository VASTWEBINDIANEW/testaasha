using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    /// <summary>
    /// Composite view model aggregating multiple report or data views for the admin dashboard.
    /// </summary>
    public class Multiview
    {
        public SelectList CountryListModel { get; set; }
        public IEnumerable<Vastwebmulti.Models.SRS_pending_count_ALL_Result> srspending { get; set; }
        public IEnumerable<Vastwebmulti.Models.SRS_pending_count_ALL_Result> srspending1 { get; set; }
        public IEnumerable<Vastwebmulti.Models.SRS_pending_count_ALL_old_Result> srspending_old { get; set; }
        public IEnumerable<Vastwebmulti.Models.SRS_pending_count_ALL_old_Result> srspending1_old { get; set; }
        //public IEnumerable<Vastwebmulti.Models.Recharge_operator_report_Result>Recharge_operator_live { get; set; }
        public IEnumerable<Vastwebmulti.Models.proc_Recharge_operator_report_withPaging_Result>Recharge_operator_live { get; set; }
        public IEnumerable<Vastwebmulti.Models.Show_recharge_report_Pending_Result> Show_recharge_report_Pending { get; set; }
        //public IEnumerable<Vastwebmulti.Models.Recharge_operator_report_old_Result> Recharge_operator_old { get; set; }
        public IEnumerable<Vastwebmulti.Models.proc_Recharge_operator_report_old_withPaging_Result> Recharge_operator_old { get; set; }
        public IEnumerable<Vastwebmulti.Models.SRS_OPT_REPORT_new_old_Result> optreport3 { get; set; }

        public IEnumerable<Vastwebmulti.Models.proc_Recharge_operator_report_newPaging_Result> proc_Recharge_operator_report_newPaging { get; set; }


        public IEnumerable<Vastwebmulti.Models.spWhitelabel_SRS_pending_count_ALL_Result> Whitelabel_srspending { get; set; }
        public IEnumerable<Vastwebmulti.Models.spWhitelabel_SRS_pending_count_ALL_Result> Whitelabel_srspending1 { get; set; }
        public IEnumerable<Vastwebmulti.Models.SpWhitelabel_Show_recharge_report_pending_Result> Whitelabel_Show_recharge_report_Pending { get; set; }
    }
}