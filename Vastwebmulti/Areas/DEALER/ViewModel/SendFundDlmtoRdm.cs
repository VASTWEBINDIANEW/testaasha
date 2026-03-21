using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.DEALER.ViewModel
{
    public class SendFundDlmtoRdm
    {
        public List<SelectListItem> ddlRetailer { get; set; }
    }
    public class FundRequestViewmodel
    {
        public int? ddlretailerID { get; set; }
        public int bankid { get; set; }
        public IEnumerable<SelectListItem> ddlRetailer { get; set; }
        public IEnumerable<SelectListItem> ddlFillAllBank { get; set; }
        public int walletid { get; set; }
        public IEnumerable<SelectListItem> ddlFillAllwallet { get; set; }
        public IEnumerable<select_rem_pur_order_Result> PurchaseRequestRecived { get; set; }

        public IEnumerable<select_dlm_rem_Result> DealerToRemFundTransfer { get; set; }


        public IEnumerable<select_dlm_pur_order_Result> SendPurchaserequest { get; set; }


    }
}