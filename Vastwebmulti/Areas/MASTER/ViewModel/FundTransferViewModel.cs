using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.MASTER.ViewModel
{
    public class FundTransferViewModel
    {
        public int idno { get; set; }
        public string dealerid { get; set; }
        public string MDEmail { get; set; }
        public string AdminCompanyname { get; set; }
        public string DealerName { get; set; }
        public string FarmName { get; set; }
        public string Email { get; set; }

        public Nullable<decimal> balance { get; set; }
        public Nullable<decimal> admin_prebal { get; set; }
        public Nullable<decimal> admin_postbal { get; set; }
        public Nullable<decimal> Oldcreditbalance { get; set; }
        public Nullable<decimal> Newbalance { get; set; }
        public Nullable<decimal> dealer_prebal { get; set; }
        public Nullable<decimal> dealer_postbal { get; set; }
        public Nullable<System.DateTime> date_dlm { get; set; }
        public Nullable<decimal> comm { get; set; }
        public Nullable<decimal> final { get; set; }
        public string balance_from { get; set; }
        public string bal_type { get; set; }
        public Nullable<decimal> oldcrbalance { get; set; }
        public Nullable<decimal> cr { get; set; }
        public string comment { get; set; }
        public string Head { get; set; }
        public int? ddlDealerID { get; set; }
        public IEnumerable<SelectListItem> ddldealers { get; set; }
        public IEnumerable<select_admin_to_Dealer_Result> mastertodlmlist { get; set; }
        public int bankid { get; set; }
        public IEnumerable<SelectListItem> ddlFillAllBank { get; set; }
        public int walletid { get; set; }
        public IEnumerable<SelectListItem> ddlFillAllwallet { get; set; }


        public IEnumerable<select_master_pur_order_Result> funrequesttoadmin { get; set; }

        public IEnumerable<select_dlm_pur_order_Result> FundRequestRecived { get; set; }

        public IEnumerable<Select_balance_Super_stokist_Result> FundRecievedDetails { get; set; }


    }
}