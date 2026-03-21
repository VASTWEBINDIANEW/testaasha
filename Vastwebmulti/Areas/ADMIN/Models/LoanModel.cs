using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    /// <summary>
    /// View model for managing loan service commission slab configurations across user roles.
    /// </summary>
    public class LoanModel
    {
        public List<LoanCommon> common { get; set; }
        public List<SelectListItem> UserId { get; set; }
        public List<SelectListItem> RetailerId { get; set; }
        public List<LoanMaster> Materuser { get; set; }
        public List<UpdateLoanMaster> UpdateMaster { get; set; }
        public List<LoanDealer> Dealeruser { get; set; }
        public List<UpdateLoanDealer> UpdateDealer { get; set; }

        public List<LoanRetailer> Retaileruser { get; set; }
        public List<UpdateLoanRetailer> UpdateRetailer { get; set; }
        public List<LoanAPI> APIuser { get; set; }
        public List<UpdateLoanAPI> UpdateAPI { get; set; }

        public List<LoanWhitelabel> Whitelabeluser { get; set; }
        public List<UpdateLoanWhitelabel> UpdateWhitelabel { get; set; }
        public string role { get; set; }
    }
    public class LoanCommon
    {
        public int idno { get; set; }
        public string optcode { get; set; }
        public string OperatorName { get; set; }
        public decimal? mastercomm { get; set; }
        public decimal? dlmcomm { get; set; }
        public decimal? retailercomm { get; set; }
        public decimal? apicomm { get; set; }
        public decimal? whitelabelcomm { get; set; }
        public decimal? gst { get; set; }
        public decimal? surcharge { set; get; }
    }
    public class LoanMaster
    {
        public int idno { get; set; }
        public string optcode { get; set; }
        public string OperatorName { get; set; }
        public decimal? comm { get; set; }
        public decimal? gst { get; set; }
        public string Email { get; set; }
        public string userid { get; set; }
        public string RolesName { get; set; }
        public string SSID { get; set; }

    }
    public class UpdateLoanMaster
    {
        public int idno { get; set; }
        public decimal? comm { get; set; }
        public decimal? gst { get; set; }
        public string OperatorName { get; set; }
        public string optcode { get; set; }

    }

    public class LoanDealer
    {
        public int idno { get; set; }
        public string optcode { get; set; }
        public string OperatorName { get; set; }
        public decimal? comm { get; set; }
        public decimal? gst { get; set; }
        public string Email { get; set; }
        public string userid { get; set; }
        public string RolesName { get; set; }
        public string DealerId { get; set; }

    }
    public class UpdateLoanDealer
    {
        public int idno { get; set; }
        public decimal? comm { get; set; }
        public decimal? gst { get; set; }
        public string OperatorName { get; set; }
        public string optcode { get; set; }

    }

    public class LoanRetailer
    {
        public int idno { get; set; }
        public string optcode { get; set; }
        public string OperatorName { get; set; }
        public decimal? comm { get; set; }
        public decimal? gst { get; set; }
        public string Email { get; set; }
        public string userid { get; set; }
        public string RolesName { get; set; }
        public string RetailerId { get; set; }
        public decimal? surcharge { set; get; }
    }
    public class UpdateLoanRetailer
    {
        public int idno { get; set; }
        public decimal? comm { get; set; }
        public decimal? gst { get; set; }
        public string OperatorName { get; set; }
        public string optcode { get; set; }
        public decimal? surcharge { set; get; }

    }

    public class LoanAPI
    {
        public int idno { get; set; }
        public string optcode { get; set; }
        public string OperatorName { get; set; }
        public decimal? comm { get; set; }
        public decimal? gst { get; set; }
        public string Email { get; set; }
        public string userid { get; set; }
        public string RolesName { get; set; }
        public string apiid { get; set; }

    }
    public class UpdateLoanAPI
    {
        public int idno { get; set; }
        public decimal? comm { get; set; }
        public decimal? gst { get; set; }
        public string OperatorName { get; set; }
        public string optcode { get; set; }

    }

    public class LoanWhitelabel
    {
        public int idno { get; set; }
        public string optcode { get; set; }
        public string OperatorName { get; set; }
        public decimal? comm { get; set; }
        public decimal? gst { get; set; }
        public string Email { get; set; }
        public string userid { get; set; }
        public string RolesName { get; set; }
        public string WhiteLabelID { get; set; }

    }
    public class UpdateLoanWhitelabel
    {
        public int idno { get; set; }
        public decimal? comm { get; set; }
        public decimal? gst { get; set; }
        public string OperatorName { get; set; }
        public string optcode { get; set; }

    }
}