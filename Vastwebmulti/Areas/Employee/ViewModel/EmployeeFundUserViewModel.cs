using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.Employee.ViewModel
{
    
    public class SwitchUserModel
    {
        public IEnumerable<ShowDealerMDSwitch_Result> ShowSwitch_DealerTOMD { get; set; }
        public IEnumerable<ShowRetailerDealerSwitch_Result> ShowRetailerDealerSwitch_Result { get; set; }
    }

    public class EmployeeFundUserViewModel
    {
        public string MastersID { get; set; }
        public string RetailersIDS { get; set; }

        public string DdlfilMasterID { get; set; }

        public int mdbnkid { get; set; }
        public int DDDealerid { get; set; }
        public IEnumerable<SelectListItem> Masters { get; set; }
        public IEnumerable<SelectListItem> Dealesrs { get; set; }
        public IEnumerable<SelectListItem> MDBANK { get; set; }

        public IEnumerable<SelectListItem> Retailerslist { get; set; }

        public IEnumerable<Master_to_dealer_report_BYAdmin_Result> master_to_dealer_report_BYAdmin_Result { get; set; }

        public IEnumerable<dealer_to_rem_fund_report_Result> dealer_to_rem_fund_report { get; set; }

        public IEnumerable<show_rem_to_rem_balByAdmin_Result> show_rem_to_rem_balByAdmin { get; set; }
        public IEnumerable<show_rem_to_rem_balByAdmin_Result> show_rem_to_rem_balByAdmin_Recived { get; set; }



        // public IEnumerable<Master_to_dealer_report_BYAdmin_Result> master_to_dealer_report_BYAdmin_Result { get; set; }
        public EmployeeFundUserViewModel()
        {
            this.Masters = new List<SelectListItem>();

        }




    }
}