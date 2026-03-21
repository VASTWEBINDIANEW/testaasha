using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.WHITELABEL.Models
{
    public class Whitelabel_Fundtransferviewmodel
    {
        public string SuperstokistID { get; set; }
        public int bankid { get; set; }
        public int walletid { get; set; }
        public IEnumerable<Vastwebmulti.Models.spWhitelabel_Select_balance_Super_stokist_Result> Superstocklistbalance { get; set; }
        public List<SelectListItem> ddlSuperstokistdetails { get; set; }
        public IEnumerable<SelectListItem> ddlbankinfo { get; set; }
        public IEnumerable<SelectListItem> ddlwalletInfo { get; set; }
        public IEnumerable<SelectListItem> ddldistributorInfo { get; set; }
        public IEnumerable<spWhitelabel_select_admin_to_Dealer_Result> admintodealerllist { get; set; }
        //public IEnumerable<select_dlm_rem_Result> select_dlm_rem_Resultlist { get; set; }
        public List<spWhitelabel_select_dlm_rem_Result> AdminTORetailer { get; set; }

        //public List<API_balance_transfer_report_Result> API_balance_transfer_reportList { get; set; }
        //public List<whitelabel_balance_transfer_report_Result> whitelabel_balance_transfer_report_List { get; set; }
        public Whitelabel_Fundtransferviewmodel()
        {
            this.ddlSuperstokistdetails = new List<SelectListItem>();

        }
    }

    public class Whitelabel_to_Admin_Fund_Request_Report
    {

    }
}