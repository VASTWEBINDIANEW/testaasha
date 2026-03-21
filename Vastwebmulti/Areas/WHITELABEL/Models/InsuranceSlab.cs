using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Vastwebmulti.Areas.WHITELABEL.Models
{
    public class InsuranceSlab
    {
        public List<InsuranceCommon> common { get; set; }
        public List<SelectListItem> UserId { get; set; }
        public List<SelectListItem> RetailerId { get; set; }
          public List<InsuranceDealer> Dealeruser { get; set; }
        public List<UpdateInsuranceDealer> UpdateDealer { get; set; }

        public List<InsuranceRetailer> Retaileruser { get; set; }
        public List<UpdateInsuranceRetailer> UpdateRetailer { get; set; }
         public string role { get; set; }
    }
    public class InsuranceCommon
    {
        public int idno { get; set; }
        public string optcode { get; set; }
        public string OperatorName { get; set; }
        public decimal? dlmcomm { get; set; }
        public decimal? retailercomm { get; set; }
        public decimal? gst { get; set; }
        public string whitelabelid { get; set; }
        public decimal? surcharge { set; get; }
    }

    public class InsuranceDealer
    {
        public int idno { get; set; }
        public string optcode { get; set; }
        public string OperatorName { get; set; }
        public decimal? comm { get; set; }
        public string Email { get; set; }
        public string userid { get; set; }
        public string RolesName { get; set; }
        public string DealerId { get; set; }
        public string whitelabelid { get; set; }
        public decimal? gst { get; set; }

    }
    public class UpdateInsuranceDealer
    {
        public int idno { get; set; }
        public decimal? comm { get; set; }
        public string OperatorName { get; set; }
        public string optcode { get; set; }
        public string whitelabelid { get; set; }
        public decimal? gst { get; set; }

    }

    public class InsuranceRetailer
    {
        public int idno { get; set; }
        public string optcode { get; set; }
        public string OperatorName { get; set; }
        public decimal? comm { get; set; }
        public string Email { get; set; }
        public string userid { get; set; }
        public string RolesName { get; set; }
        public string DealerId { get; set; }
        public decimal? gst { get; set; }
        public string whitelabelid { get; set; }
        public decimal? surcharge { set; get; }
    }
    public class UpdateInsuranceRetailer
    {
        public int idno { get; set; }
        public decimal? comm { get; set; }
        public string OperatorName { get; set; }
        public string optcode { get; set; }
        public decimal? gst { get; set; }
        public string whitelabelid { get; set; }
        public decimal? surcharge { set; get; }

    }

 }