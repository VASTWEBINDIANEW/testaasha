using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vastwebmulti.Models;
namespace Vastwebmulti.Areas.WRetailer.ViewModels
{
    public class WhitelabelRetailerFundtransfer
    {

        public IEnumerable<show_Whitelabel_rem_to_whitelabel_rem_bal_Result> selectremtoremreports { get; set; }
        public IEnumerable<select_whitelabel_rem_pur_order_Result> selectrem_purchase_order_reports { get; set; }
        public IEnumerable<select_whitelabel_report_dealer_to_retailer_balance_Result> showdatafundrecivefromwhitelabel { get; set; }
        public IEnumerable<show_fundrecive_retailer_wdealer_Result> showdatafundrecivefromdealer { get; set; }
        public IEnumerable<select_retailer_credit_report_by_Wadmin_Result> retailer_credit_report_by_Wadmin { get; set; }
        public IEnumerable<select_Wretailer_credit_report_by_Wdealer_Result> retailer_credit_report_by_dealer { get; set; }
        public IEnumerable<SelectListItem> ddlRetailers { get; set; }
        public IEnumerable<chargeswhitelabeltoDLM> cashdepositecharges { get; set; }




    }
}