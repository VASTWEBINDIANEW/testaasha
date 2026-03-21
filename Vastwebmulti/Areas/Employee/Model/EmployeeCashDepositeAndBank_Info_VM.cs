using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.Employee.Model
{
    public class EmployeeCashDepositeAndBank_Info_VM
    {

        public List<Retailerchargelist> creditchargeRetailer { get; set; }
        public List<Dealerchargelist> creditchargeDealer { get; set; }
        public List<masterchargelist> creditchargemaster { get; set; }
        public List<apichargelist> creditchargeapi { get; set; }
        public List<whitechargelist> creditchargewhite { get; set; }
        public List<PurchaseOrderCashDepositCharge> PurchaseOrderCashDepositCharge { get; set; }
        public string msg { get; set; }
        public List<bank_info> bank_info1 { get; set; }
        public List<bank_info> bank_info2 { get; set; }
        public List<bank_info> bank_info3 { get; set; }

        public IEnumerable<WalletUnloadReportAdmin_Result> WalletUnloadReportAdmin { get; set; }

        public List<WalletToBankAmountTransferCharge> WulChargeList { get; set; }
        public WalletToBankAmountTransferCharge WulChargeByid { get; set; }

    }

    public class masterchargelist
    {
        public string userinfo { get; set; }
        public string userid { get; set; }
        public decimal? Charge { get; set; }
        public string ctype { get; set; }

    }
    public class Retailerchargelist
    {
        public string userinfo { get; set; }
        public string userid { get; set; }
        public decimal? Charge { get; set; }
        public string ctype { get; set; }
        public string chargefrom { get; set; }

    }
    public class Dealerchargelist
    {
        public string userinfo { get; set; }
        public string userid { get; set; }
        public decimal? Charge { get; set; }
        public string ctype { get; set; }
        public string chargefrom { get; set; }

    }
    public class apichargelist
    {
        public string userinfo { get; set; }
        public string userid { get; set; }
        public decimal? Charge { get; set; }
        public string ctype { get; set; }
        public string chargefrom { get; set; }

    }
    public class whitechargelist
    {
        public string userinfo { get; set; }
        public string userid { get; set; }
        public decimal? Charge { get; set; }
        public string ctype { get; set; }
        public string chargefrom { get; set; }

    }
}