using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.WHITELABEL.Models
{
    public class Whitelabel_CashDepositeAndBank_Info_VM
    {
        public List<WhitelabelRetailerchargelist> creditchargeRetailer { get; set; }
        public List<WhitelabelDealerchargelist> creditchargeDealer { get; set; }
        public List<Whitelabelmasterchargelist> creditchargemaster { get; set; }

        public List<Whitelabel_PurchaseOrderCashDepositCharge> PurchaseOrderCashDepositCharge { get; set; }
        public string msg { get; set; }

        public List<Whitelabel_WalletToBankAmountTransferCharge> WulChargeList { get; set; }
        public Whitelabel_WalletToBankAmountTransferCharge WulChargeByid { get; set; }
        public IEnumerable<spWhitelabel_WalletUnloadReport_Result> Whitelabel_WalletUnloadReport { get; set; }
    }

    public class Whitelabelmasterchargelist
    {
        public string userinfo { get; set; }
        public string userid { get; set; }
        public decimal? Charge { get; set; }
        public string ctype { get; set; }

    }
    public class WhitelabelDealerchargelist
    {
        public string userinfo { get; set; }
        public string userid { get; set; }
        public decimal? Charge { get; set; }
        public string ctype { get; set; }
        public string chargefrom { get; set; }

    }
    public class WhitelabelRetailerchargelist
    {
        public string userinfo { get; set; }
        public string userid { get; set; }
        public decimal? Charge { get; set; }
        public string ctype { get; set; }
        public string chargefrom { get; set; }

    }

    public class WhitelabelGetDetailsWalletUnload
    {
        public string RequestId { get; set; }
        public string FirmName { get; set; }
        public string BankAccount { get; set; }
        public string accountholder { get; set; }
        public string IfscCode { get; set; }
        public string BankName { get; set; }
        public decimal? Amount { get; set; }
        public decimal? ProcessingCharge { get; set; }
        public decimal? RemPre { get; set; }
        public decimal? Total_Debit { get; set; }
        public decimal? RemPost { get; set; }
        public string BankRRN { get; set; }
        public DateTime? RequestDate { get; set; }
    }
}