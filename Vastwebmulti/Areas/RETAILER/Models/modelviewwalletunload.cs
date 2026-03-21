using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.RETAILER.Models
{
    public class modelviewwalletunload
    {
 
        public IEnumerable<Vastwebmulti.Models.WalletToBankAmountTransferCharge> Charges { get; set; }
        public IEnumerable<Vastwebmulti.Models.WalletToBankAmountTransferRequest> Requests { get; set; }
        public IEnumerable<Vastwebmulti.Models.pos_ledger_report_Result> Posledger { get; set; }
        public IEnumerable<Vastwebmulti.Models.pos_ledger_report_Result> Posledger1 { get; set; }


        #region Whitelabel
        public IEnumerable<Vastwebmulti.Models.Whitelabel_WalletToBankAmountTransferCharge> WCharges { get; set; }
        public IEnumerable<Vastwebmulti.Models.Whitelabel_WalletToBankAmountTransferRequests> WRequests { get; set; }
        #endregion
    }
   
}