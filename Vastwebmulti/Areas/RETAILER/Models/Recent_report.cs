using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.RETAILER.Models
{
    public class Recent_report
    {
        public IEnumerable<Vastwebmulti.Models.recent_imps_report_Result> Recent_report_imps { get; set; }
        public IEnumerable<Vastwebmulti.Models.AEPS_TXN_Details> Recent_report_Aeps { get; set; }
        public IEnumerable<Vastwebmulti.Models.pancard_transation> Recent_PAN_CARD_IPAY { get; set; }
        public IEnumerable<Vastwebmulti.Models.pancard_transation_manual> Recent_pancard_transation_manual { get; set; }
        public IEnumerable<Vastwebmulti.Models.mPosInfo> Recent_mPosInfo { get; set; }
        public IEnumerable<Vastwebmulti.Models.MicroAtm_Trans_info> Recent_microatm { get; set; }
        public IEnumerable<Vastwebmulti.Models.cash_deposit_history> Recent_cash_deposit { get; set; }
        public string statustypeforpartial { get; set; }
        #region WAdmin_Users
        public IEnumerable<Vastwebmulti.Models.recent_imps_report_Result> WRecent_report_imps { get; set; }
        public IEnumerable<Vastwebmulti.Models.AEPS_TXN_Details> WRecent_report_Aeps { get; set; }
        public IEnumerable<Vastwebmulti.Models.PAN_CARD_IPAY> WRecent_PAN_CARD_IPAY { get; set; }
        //public IEnumerable<Vastwebmulti.Models.mPosInfo> WRecent_mPosInfo { get; set; }
        //public IEnumerable<Vastwebmulti.Models.MicroAtm_Trans_info> WRecent_microatm { get; set; }
        #endregion
    }
}