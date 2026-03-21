using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.ADMIN.ViewModel
{
    public class OperatorIndexViewModel
    {

        public string operator_Name { get; set; }
        public int Operator_id { get; set; }
        public string Operator_type { get; set; }
        public Nullable<int> his_opt_id { get; set; }
        public string status { get; set; }
       
        public string new_opt_code { get; set; }
       
       
        public Nullable<System.DateTime> blocktime { get; set; }
        public Nullable<int> resendcount { get; set; }
        public Nullable<int> nillcount { get; set; }
        public Nullable<int> nillblocktime { get; set; }
        public Nullable<int> resendtime { get; set; }
        public string ImagePath { get; set; }
        public string minimum { get; set; }
        public string maximum { get; set; }
        public string Operator_img { get; set; }

        public List<Operator_Code> opcodelist { get; set; }
        public List<Operator_Code> Utilitylist { get; set; }

        public List<Operator_Code> Servicelist { get; set; }
        public List<Operator_Code> Travellist { get; set; }

        public List<Operator_Code> OtherServicelist { get; set; }

        public List<Operator_Code> OperatorTypeList { get; set; }


       

    }
    public class NillPortalViewmodel
    {
        public IEnumerable<select_nill_com_Result> NillCOmopcodeamountlist { get; set; }
        public Nill_Com NillCOmGetbyid { get; set; }
        public IEnumerable<show_recharge_switch_special_Result> ShowREchargelist { get; set; }
        public recharge_switch_special GetRechargeSpecialGetById { get; set; }
       public IEnumerable<show_recharge_switch_Result> ShowOPSwitchList { get; set; }

        public IEnumerable<switch_State_wise_Result> SwitchStateWise { get; set; }
        public switch_State_wise_Result SwitchStateWiseByID { get; set; }
        public IEnumerable<range_wise_comm_Report_Result> rangewisecomm_report { get; set; }

    }
    public class Recharge_specialViewmodel
    {
        public int idno { get; set; }
        public string rch_from { get; set; }
        public Nullable<int> amount { get; set; }
        public Nullable<int> maxamount { get; set; }
        public string rch_to { get; set; }
        public string sts { get; set; }
        public string operator_Name { get; set; }
        public string portno { get; set; }
        public string RetailerId { get; set; }

        public string Frm_Name { get; set; }

       
        public IEnumerable<Retailer_Details> retDetail { get; set; }
        public IEnumerable<api_user_details> ApiuserDetail { get; set; }
       

    }
    public class MicroatmchargeViewmodel
    {
        public IEnumerable<Micro_atm_device_plan> Showdeviceplanlist { get; set; }
        public Micro_atm_device_plan GetChargelistById { get; set; }

    }
}