using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    /// <summary>
    /// View model for managing insurance service commission slab configurations.
    /// </summary>
    public class InsuranceSlab
    {
        public List<InsuranceCommon> common { get; set; }
        public List<SelectListItem> UserId { get; set; }
        public List<SelectListItem> RetailerId { get; set; }
        public List<InsuranceMaster> Materuser { get; set; }
        public List<UpdateInsuranceMaster> UpdateMaster { get; set; }
        public List<InsuranceDealer> Dealeruser { get; set; }
        public List<UpdateInsuranceDealer> UpdateDealer { get; set; }

        public List<InsuranceRetailer> Retaileruser { get; set; }
        public List<UpdateInsuranceRetailer> UpdateRetailer { get; set; }
        public List<InsuranceAPI> APIuser { get; set; }
        public List<UpdateInsuranceAPI> UpdateAPI { get; set; }

        public List<InsuranceWhitelabel> Whitelabeluser { get; set; }
        public List<UpdateInsuranceWhitelabel> UpdateWhitelabel { get; set; }
        public string role { get; set; }
    }
    public class InsuranceCommon
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
    public class InsuranceMaster
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
    public class UpdateInsuranceMaster
    {
        public int idno { get; set; }
        public decimal? comm { get; set; }
        public decimal? gst { get; set; }
        public string OperatorName { get; set; }
        public string optcode { get; set; }

    }

    public class InsuranceDealer
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
    public class UpdateInsuranceDealer
    {
        public int idno { get; set; }
        public decimal? comm { get; set; }
        public decimal? gst { get; set; }
        public string OperatorName { get; set; }
        public string optcode { get; set; }

    }

    public class InsuranceRetailer
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
    public class UpdateInsuranceRetailer
    {
        public int idno { get; set; }
        public decimal? comm { get; set; }
        public decimal? gst { get; set; }
        public string OperatorName { get; set; }
        public string optcode { get; set; }
        public decimal? surcharge { set; get; }

    }

    public class InsuranceAPI
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
    public class UpdateInsuranceAPI
    {
        public int idno { get; set; }
        public decimal? comm { get; set; }
        public decimal? gst { get; set; }
        public string OperatorName { get; set; }
        public string optcode { get; set; }

    }

    public class InsuranceWhitelabel
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
    public class UpdateInsuranceWhitelabel
    {
        public int idno { get; set; }
        public decimal? comm { get; set; }
        public decimal? gst { get; set; }
        public string OperatorName { get; set; }
        public string optcode { get; set; }

    }
   
}