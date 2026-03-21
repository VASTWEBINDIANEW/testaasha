using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Vastwebmulti.Areas.WHITELABEL.Models
{
    public class Waterslab
    {

        public List<WaterCommon> common { get; set; }
        public List<SelectListItem> UserId { get; set; }
        public List<SelectListItem> RetailerId { get; set; }
        public List<WaterDealer> Dealeruser { get; set; }
        public List<UpdateWaterDealer> UpdateDealer { get; set; }
        public List<WaterRetailer> Retaileruser { get; set; }
        public List<UpdateWaterRetailer> UpdateRetailer { get; set; }
        public string role { get; set; }
    }

    public class WaterCommon
    {
        public int idno { get; set; }
        public string optcode { get; set; }
        public string OperatorName { get; set; }
        public decimal? dlmcomm { get; set; }
        public decimal? retailercomm { get; set; }
        public decimal? gst { get; set; }
        public string whitelabelid { get; set; }

    }
  
    public class WaterDealer
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
    public class UpdateWaterDealer
    {
        public int idno { get; set; }
        public decimal? comm { get; set; }
        public string OperatorName { get; set; }
        public string optcode { get; set; }
        public string whitelabelid { get; set; }
        public decimal? gst { get; set; }

    }

    public class WaterRetailer
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

    }
    public class UpdateWaterRetailer
    {
        public int idno { get; set; }
        public decimal? comm { get; set; }
        public string OperatorName { get; set; }
        public string optcode { get; set; }
        public decimal? gst { get; set; }
        public string whitelabelid { get; set; }

    }


}