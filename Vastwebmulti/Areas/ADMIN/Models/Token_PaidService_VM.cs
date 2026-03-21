using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    /// <summary>
    /// Composite view model for managing token-based paid service charges and dealer token reports.
    /// </summary>
    public class Token_PaidService_VM
    {
        public List<RetailerCreationCharge> RetailerCreationCharge { get; set; }
        public List<PaidServicesChargeList> PaidServicesChargeList { get; set; }
        public List<RetailerCreationTokenVM> RetailerCreationTokenVM { get; set; }
        public PaidServicesChargeList PaidServicesChargeByID { get; set; }
        public List<DealerCreationTokenVM> DealerCreationTokenVM { get; set; }
        public List<dealer_remain_token_report_Result> dealer_remain_token_report_Result { get; set; }
        public List<master_remain_token_report_Result> master_remain_token_report_Result { get; set; }
        public IEnumerable<Vastwebmulti.Models.Select_Dealer_total_Result> Select_dealer_list { set; get; }
    }
}