using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.WHITELABEL.Models
{
    public class Token_PaidService_VM
    {
        public List<Whitelabel_RetailerCreationCharge> RetailerCreationCharge { get; set; }
        public List<Whitelabel_PaidServicesChargeList> PaidServicesChargeList { get; set; }
        public List<RetailerCreationTokenVM> RetailerCreationTokenVM { get; set; }
        public Whitelabel_PaidServicesChargeList PaidServicesChargeByID { get; set; }
        public List<DealerCreationTokenVM> DealerCreationTokenVM { get; set; }
        public List<Whitelabel_dealer_remain_token_report_Result> dealer_remain_token_report_Result { get; set; }
        public List<Whitelabel_master_remain_token_report_Result> master_remain_token_report_Result { get; set; }
        public IEnumerable<whitelabel_Select_Dealer_total_Result> Select_dealer_list { set; get; }
    }
}