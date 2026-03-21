using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.RETAILER.ViewModels
{
    public class RetailerFundRequestViewmodel
    {
        public int bankid { get; set; }
        public IEnumerable<SelectListItem> ddlRetailer { get; set; }
        public IEnumerable<SelectListItem> ddlFillAllBank { get; set; }
        public int walletid { get; set; }
        public IEnumerable<SelectListItem> ddlFillAllwallet { get; set; }

        public IEnumerable<select_rem_pur_order_Result> RemPurchaseOrder { get; set; }
        public IEnumerable<show_rem_to_rem_bal_Result> ShowRem_to_Rem { get; set; }

        public IEnumerable<fundTransferALLReports_Result> fundTransferALLReports { get; set; }
    }
}