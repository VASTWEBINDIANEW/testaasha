using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.WHITELABEL.ViewModel
{
    public class FundTransferWhitelabelToDLM
    {
        public int? ddlDealerID { get; set; }
        public IEnumerable<SelectListItem> ddldealers { get; set; }

        public int? ddlRetailerID { get; set; }
        public IEnumerable<SelectListItem> ddlRetailers { get; set; }

        public int bankid { get; set; }
        public IEnumerable<SelectListItem> ddlFillAllBank { get; set; }
        public int walletid { get; set; }
        public IEnumerable<SelectListItem> ddlFillAllwallet { get; set; }

        public IEnumerable<select_whitelabel_dlm_pur_order_Result> WLDealerPurchaseorderRequest { get; set; }
        public IEnumerable<select_whitelabel_rem_pur_order_Result> WLRetailerPurchaseorderRequest { get; set; }

        public IEnumerable<whitelabel_fund_recive_by_admin_report_Result> fund_recive_by_admin_report { get; set; }

        public IEnumerable<select_whitelabel_master_pur_order_Result> WhitelabelPurchaseOrder { get; set; }

        public IEnumerable<select_whitelabel_TO_WhitelabelDlm_Result> ShowDataWhitelabelToDLMlist { get; set; }

        public IEnumerable<FundWLMTOREM> ShowDataWhitelabelToREMlist { get; set; }

        public string message { get; set; }

    }

    public class FundWLMTODLM
    {
        public  string DealerName { get; set; }
        public string dealerEmail { get; set; }
        public string balance { get; set; }
        public string whitelabelpre { get; set; }
        public string whitelabelpost { get; set; }
        public string dealerpre { get; set; }
        public string dealerpost { get; set; }
        public string transfer_date { get; set; }
        public string bal_type { get; set; }
        public string oldcrbalance { get; set; }
        public string comment { get; set; }
        public string cr { get; set; }
        public string Fundvalue { get; set; }
         public string CollectionBy { get; set; }
        public string BankName { get; set; }
        public string Admin_AccountNo { get; set; }
        public string Head { get; set; }
             
        public decimal? Commision { get; set; }
    }

    public class FundWLMTOREM : FundWLMTODLM
    {
        public string RetailerName { get; set; }
        public string RetailerEMAIL { get; set; }
     
        public string retailerpre { get; set; }
        public string retailerpost { get; set; }

    }

}