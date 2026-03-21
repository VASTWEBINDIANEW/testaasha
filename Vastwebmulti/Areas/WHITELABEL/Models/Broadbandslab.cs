using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Vastwebmulti.Areas.WHITELABEL.Models
{
    public class Broadbandslab
    {
        public List<BroadbandCommon> common { get; set; }
        public List<SelectListItem> UserId { get; set; }
        public List<SelectListItem> RetailerId { get; set; }
         public List<BroandbandDealer> Dealeruser { get; set; }
        public List<UpdateBroadbandDealer> UpdateDealer { get; set; }
        public List<BroadbandRetailer> Retaileruser { get; set; }
        public List<UpdateBroadbandRetailer> UpdateRetailer { get; set; }
        public string role { get; set; }
    }


    public class BroadbandCommon
    {
        public int idno { get; set; }
        public string optcode { get; set; }
        public string OperatorName { get; set; }
        public decimal? dlmcomm { get; set; }
        public decimal? retailercomm { get; set; }
        public decimal? gst { get; set; }
        public string whitelabelid { get; set; }

    }
   
    public class BroandbandDealer
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
    public class UpdateBroadbandDealer
    {
        public int idno { get; set; }
        public decimal? comm { get; set; }
        public string OperatorName { get; set; }
        public string optcode { get; set; }
        public string whitelabelid { get; set; }
        public decimal? gst { get; set; }

    }

    public class BroadbandRetailer
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
    public class UpdateBroadbandRetailer
    {
        public int idno { get; set; }
        public decimal? comm { get; set; }
        public string OperatorName { get; set; }
        public string optcode { get; set; }
        public decimal? gst { get; set; }
        public string whitelabelid { get; set; }

    }

  
}