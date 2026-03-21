using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    /// <summary>
    /// View model for managing DTH new connection commission slab configurations.
    /// </summary>
    public class DthConnSlab
    {
        public List<DthConnectionCommon> common { get; set; }
        public List<SelectListItem> UserId { get; set; }
        public List<SelectListItem> RetailerId { get; set; }
        public List<DthConnectionMaster> Materuser { get; set; }
        public List<UpdateDthConnectionMaster> UpdateMaster { get; set; }
        public List<DthConnectionDealer> Dealeruser { get; set; }
        public List<UpdateDthConnectionDealer> UpdateDealer { get; set; }
        public List<DthConnectionRetailer> Retaileruser { get; set; }
        public List<UpdateDthConnectionRetailer> UpdateRetailer { get; set; }
        public List<DthConnectionAPI> APIuser { get; set; }
        public List<UpdateDthConnectionAPI> UpdateAPI { get; set; }

        public List<DthConnectionWhitelabel> Whitelabeluser { get; set; }
        public List<UpdateDthConnectionWhitelabel> UpdateWhitelabel { get; set; }
        public string role { get; set; }
    }
    public class DthConnectionCommon
    {
        public int idno { get; set; }
        public string optcode { get; set; }
        public string OperatorName { get; set; }
        public decimal? mastercomm { get; set; }
        public decimal? dlmcomm { get; set; }
        public decimal? retailercomm { get; set; }
        public decimal? apicomm { get; set; }
        public decimal? whitelabelcomm { get; set; }

    }
    public class DthConnectionMaster
    {
        public int idno { get; set; }
        public string optcode { get; set; }
        public string OperatorName { get; set; }
        public decimal? comm { get; set; }
        public string Email { get; set; }
        public string userid { get; set; }
        public string RolesName { get; set; }
        public string SSID { get; set; }

    }
    public class UpdateDthConnectionMaster
    {
        public int idno { get; set; }
        public decimal? comm { get; set; }
        public string OperatorName { get; set; }
        public string optcode { get; set; }

    }

    public class DthConnectionDealer
    {
        public int idno { get; set; }
        public string optcode { get; set; }
        public string OperatorName { get; set; }
        public decimal? comm { get; set; }
        public string Email { get; set; }
        public string userid { get; set; }
        public string RolesName { get; set; }
        public string DealerId { get; set; }

    }
    public class UpdateDthConnectionDealer
    {
        public int idno { get; set; }
        public decimal? comm { get; set; }
        public string OperatorName { get; set; }
        public string optcode { get; set; }

    }

    public class DthConnectionRetailer
    {
        public int idno { get; set; }
        public string optcode { get; set; }
        public string OperatorName { get; set; }
        public decimal? comm { get; set; }
        public string Email { get; set; }
        public string userid { get; set; }
        public string RolesName { get; set; }
        public string DealerId { get; set; }

    }
    public class UpdateDthConnectionRetailer
    {
        public int idno { get; set; }
        public decimal? comm { get; set; }
        public string OperatorName { get; set; }
        public string optcode { get; set; }

    }

    public class DthConnectionAPI
    {
        public int idno { get; set; }
        public string optcode { get; set; }
        public string OperatorName { get; set; }
        public decimal? comm { get; set; }
        public string Email { get; set; }
        public string userid { get; set; }
        public string RolesName { get; set; }
        public string apiid { get; set; }

    }
    public class UpdateDthConnectionAPI
    {
        public int idno { get; set; }
        public decimal? comm { get; set; }
        public string OperatorName { get; set; }
        public string optcode { get; set; }

    }

    public class DthConnectionWhitelabel
    {
        public int idno { get; set; }
        public string optcode { get; set; }
        public string OperatorName { get; set; }
        public decimal? comm { get; set; }
        public string Email { get; set; }
        public string userid { get; set; }
        public string RolesName { get; set; }
        public string WhiteLabelID { get; set; }

    }
    public class UpdateDthConnectionWhitelabel
    {
        public int idno { get; set; }
        public decimal? comm { get; set; }
        public string OperatorName { get; set; }
        public string optcode { get; set; }

    }
}