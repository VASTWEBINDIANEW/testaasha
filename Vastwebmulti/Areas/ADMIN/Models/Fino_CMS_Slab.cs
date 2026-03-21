using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    public class Fino_CMS_Slab
    {
        public List<FinoCommon> common { get; set; }
        public List<SelectListItem> UserId { get; set; }
        public List<SelectListItem> RetailerId { get; set; }
        public List<FinoMaster> Materuser { get; set; }
        public List<UpdateFinoMaster> UpdateMaster { get; set; }
        public List<FinoDealer> Dealeruser { get; set; }
        public List<UpdateFinoDealer> UpdateDealer { get; set; }
        public List<FinoRetailer> Retaileruser { get; set; }
        public List<UpdateFinoRetailer> UpdateRetailer { get; set; }
        public List<FinodlmRetailer> dlmremuser { get; set; }
        public List<UpdateFinodlmrem> Updatedlmrem { get; set; }
        public string role { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public bool? allow { get; set; }
    }

    public class FinoCommon
    {
        public int idno { get; set; }
        public string optcode { get; set; }
        public string OperatorName { get; set; }
        public decimal? mastercomm { get; set; }
        public decimal? dlmcomm { get; set; }
        public decimal? retailercomm { get; set; }

    }
    public class FinoMaster
    {
        public int idno { get; set; }
        public string optcode { get; set; }
        public string OperatorName { get; set; }
        public decimal? comm { get; set; }
        public string Email { get; set; }
        public string userid { get; set; }
        public string RetailerId { get; set; }
        public string RolesName { get; set; }
        public string SSID { get; set; }

    }
    public class UpdateFinoMaster
    {
        public int idno { get; set; }
        public decimal? comm { get; set; }
        public string OperatorName { get; set; }
        public string optcode { get; set; }

    }

    public class FinoDealer
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
    public class UpdateFinoDealer
    {
        public int idno { get; set; }
        public decimal? comm { get; set; }
        public string OperatorName { get; set; }
        public string optcode { get; set; }

    }

    public class FinoRetailer
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
    public class FinodlmRetailer
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
    public class UpdateFinoRetailer
    {
        public int idno { get; set; }
        public decimal? comm { get; set; }
        public string OperatorName { get; set; }
        public string optcode { get; set; }

    }
    public class UpdateFinodlmrem
    {
        public int idno { get; set; }
        public decimal? comm { get; set; }
        public string OperatorName { get; set; }
        public string optcode { get; set; }
    }

    //public class Airtel_CMS_Slab
    //{
    //    public List<AirtelCommon> common { get; set; }
    //    public List<SelectListItem> UserId { get; set; }
    //    public List<SelectListItem> RetailerId { get; set; }
    //    public List<AirtelMaster> Materuser { get; set; }
    //    public List<UpdateAirtelMaster> UpdateMaster { get; set; }
    //    public List<AirtelDealer> Dealeruser { get; set; }
    //    public List<UpdateAirtelDealer> UpdateDealer { get; set; }
    //    public List<AirtelRetailer> Retaileruser { get; set; }
    //    public List<UpdateAirtelRetailer> UpdateRetailer { get; set; }
    //    public List<AirteldlmRetailer> dlmremuser { get; set; }
    //    public List<UpdateAirteldlmrem> Updatedlmrem { get; set; }
    //    public string role { get; set; }
    //    public string Name { get; set; }
    //    public string Phone { get; set; }
    //    public string Email { get; set; }
    //    public bool? allow { get; set; }
    //}

    //public class AirtelCommon
    //{
    //    public int idno { get; set; }
    //    public string optcode { get; set; }
    //    public string OperatorName { get; set; }
    //    public decimal? mastercomm { get; set; }
    //    public decimal? dlmcomm { get; set; }
    //    public decimal? retailercomm { get; set; }

    //}
    //public class AirtelMaster
    //{
    //    public int idno { get; set; }
    //    public string optcode { get; set; }
    //    public string OperatorName { get; set; }
    //    public decimal? comm { get; set; }
    //    public string Email { get; set; }
    //    public string userid { get; set; }
    //    public string RetailerId { get; set; }
    //    public string RolesName { get; set; }
    //    public string SSID { get; set; }

    //}
    //public class UpdateAirtelMaster
    //{
    //    public int idno { get; set; }
    //    public decimal? comm { get; set; }
    //    public string OperatorName { get; set; }
    //    public string optcode { get; set; }

    //}

    //public class AirtelDealer
    //{
    //    public int idno { get; set; }
    //    public string optcode { get; set; }
    //    public string OperatorName { get; set; }
    //    public decimal? comm { get; set; }
    //    public string Email { get; set; }
    //    public string userid { get; set; }
    //    public string RolesName { get; set; }
    //    public string DealerId { get; set; }

    //}
    //public class UpdateAirtelDealer
    //{
    //    public int idno { get; set; }
    //    public decimal? comm { get; set; }
    //    public string OperatorName { get; set; }
    //    public string optcode { get; set; }

    //}

    //public class AirtelRetailer
    //{
    //    public int idno { get; set; }
    //    public string optcode { get; set; }
    //    public string OperatorName { get; set; }
    //    public decimal? comm { get; set; }
    //    public string Email { get; set; }
    //    public string userid { get; set; }
    //    public string RolesName { get; set; }
    //    public string DealerId { get; set; }

    //}
    //public class AirteldlmRetailer
    //{
    //    public int idno { get; set; }
    //    public string optcode { get; set; }
    //    public string OperatorName { get; set; }
    //    public decimal? comm { get; set; }
    //    public string Email { get; set; }
    //    public string userid { get; set; }
    //    public string RolesName { get; set; }
    //    public string DealerId { get; set; }

    //}
    //public class UpdateAirtelRetailer
    //{
    //    public int idno { get; set; }
    //    public decimal? comm { get; set; }
    //    public string OperatorName { get; set; }
    //    public string optcode { get; set; }

    //}
    //public class UpdateAirteldlmrem
    //{
    //    public int idno { get; set; }
    //    public decimal? comm { get; set; }
    //    public string OperatorName { get; set; }
    //    public string optcode { get; set; }
    //}

}