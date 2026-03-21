using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.MASTER.Models
{
    public class Token_PaidService_VM1
    {

        public List<RetailerCreationCharge> RetailerCreationCharge { get; set; }
        public List<PaidServicesChargeList> PaidServicesChargeList { get; set; }
    
        public PaidServicesChargeList PaidServicesChargeByID { get; set; }
        public List<DealerCreationTokenVM1> DealerCreationTokenVM { get; set; }
        public List<dealer_remain_token_report_Result> dealer_remain_token_report_Result { get; set; }
        public List<master_remain_token_report_Result> master_remain_token_report_Result { get; set; }
        public IEnumerable<Vastwebmulti.Models.Select_Dealer_total_Result> Select_dealer_list { set; get; }
    }
}