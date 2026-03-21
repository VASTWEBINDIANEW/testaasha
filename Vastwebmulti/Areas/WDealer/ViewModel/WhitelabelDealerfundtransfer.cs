using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.WDealer.ViewModel
{
    public class WhitelabelDealerfundtransfer
    {
        public string WLbankid { get; set; }
        public string WLwalletid { get; set; }
        public string bankid { get; set; }
        public string walletid { get; set; }

        public string message { get; set; }

        public IEnumerable<select_whitelabel_dealer_dlm_rem_Result> Select_whitelabel_dealer_to_REM { get; set; }
        public IEnumerable<select_whitelabel_dlm_pur_order_Result> Wl_dlm_purchase_order_send { get; set; }
        public IEnumerable<select_whitelabel_rem_pur_order_Result> Whitelabel_rem_pur_recive { get; set; }
        public IEnumerable<whitelabel_dlm_bal_recive_report_Result> WL_dlm_bal_recived_reports { get; set; }

        public IEnumerable<select_Dealer_credit_report_by_Wadmin_Result> select_Dealer_credit_report { get; set; }

        public IEnumerable<SelectListItem> ddlRetailer { get; set; }
        public IEnumerable<SelectListItem> ddlFillAllBank { get; set; }
        public IEnumerable<SelectListItem> ddlFillAllwallet { get; set; }

        #region New Fundtransfer
        public int? ddlretailerID { get; set; }
        public IEnumerable<SelectListItem> ddlFillWLBank { get; set; }
        public IEnumerable<SelectListItem> ddlfillWLWallet { get; set; }
        public IEnumerable<SelectListItem> ddlFillWLDLMBank { get; set; }
        public IEnumerable<SelectListItem> ddlfillWLDLMMWallet { get; set; }

        public IEnumerable<select_whitelabel_rem_pur_order_Result> PurchaseRequestRecived { get; set; }

        public IEnumerable<select_whitelabel_dealer_dlm_rem_Result> DealerToRemFundTransfer { get; set; }


        public IEnumerable<select_whitelabel_dlm_pur_order_Result> SendPurchaserequest { get; set; }
        #endregion
    }
}