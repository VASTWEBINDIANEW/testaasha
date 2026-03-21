using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vastwebmulti.Models;
namespace Vastwebmulti.Areas.ADMIN.Models
{
    /// <summary>
    /// View model for managing postpaid and utility bill payment commission slab configurations.
    /// </summary>
    public class PostpaidUtilitySlab
    {
        public List<ExtraCommon> Extra { get; set; }
        public List<ExtraCommonAPI> ExtraAPI { get; set; }
        public List<PostpaidCommon> common { get; set; }
        public List<chargetypecomm> chargetype { get; set; }
        public List<SelectListItem> OPttype { get; set; }
        public List<SelectListItem> UserId { get; set; }
        public List<SelectListItem> RetailerId { get; set; }
        public List<PostpaidMaster> Materuser { get; set; }
        public List<UpdatePostpaidMaster> UpdateMaster { get; set; }
        public List<PostpaidDealer> Dealeruser { get; set; }
        public List<PostpaiddlmRetailer> dlmremuser { get; set; }
        public List<UpdatePostpaiddlmrem> Updatedlmrem { get; set; }
        public List<UpdatePostpaidDealer> UpdateDealer { get; set; }

        public List<PostpaidRetailer> Retaileruser { get; set; }
        public List<UpdatePostpaidRetailer> UpdateRetailer { get; set; }
        public List<PostpaidAPI> APIuser { get; set; }
        public List<UpdatePostpaidAPI> UpdateAPI { get; set; }

        public List<PostpaidWhitelabel> Whitelabeluser { get; set; }
        public List<UpdatePostpaidWhitelabel> UpdateWhitelabel { get; set; }
        public string role { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

    }
    public class ExtraCommon
    {
        public string optcode { get; set; }
        public string Operator_type { get; set; }
        public string operator_Name { get; set; }
        public decimal? move1 { get; set; }          
        public decimal? move2 { get; set; }
        public decimal? move3 { get; set; }
        public decimal? move4 { get; set; }
        public decimal? move5 { get; set; }
    }
    public class ExtraCommonAPI
    {
        public string optcode { get; set; }
        public string Operator_type { get; set; }
        public string operator_Name { get; set; }
        public decimal? move1 { get; set; }
        public decimal? move2 { get; set; }
        public decimal? move3 { get; set; }
        public decimal? move4 { get; set; }
        public decimal? move5 { get; set; }
    }
    public class chargetypecomm
    {
        public string rch_type { get; set; }
        public string user_type { get; set; }
        public bool? fixedrs { get; set; }
    }
    public class PostpaidCommon
    {
        public int idno { get; set; }
        public string optcode { get; set; }
        public string opttype { get; set; }
        public string OperatorName { get; set; }
        public decimal? mastercomm { get; set; }
        public decimal? dlmcomm { get; set; }
        public decimal? retailercomm { get; set; }
        public decimal? dlm_rem_comm { get; set; }
        public decimal? apicomm { get; set; }
        public decimal? whitelabelcomm { get; set; }
        public decimal? gst { get; set; }
        public decimal? surcharge { set; get; }
    }
    public class PostpaidMaster
    {
        public int idno { get; set; }
        public string optcode { get; set; }
        public string opttype { get; set; }
        public string OperatorName { get; set; }
        public decimal? comm { get; set; }
        public decimal? gst { get; set; }
        public string Email { get; set; }
        public string userid { get; set; }
        public string RolesName { get; set; }
        public string SSID { get; set; }

    }
    public class UpdatePostpaidMaster
    {
        public int idno { get; set; }
        public decimal? comm { get; set; }
        public decimal? gst { get; set; }
        public string OperatorName { get; set; }
        public string optcode { get; set; }
        public string opttype { get; set; }
    }

    public class PostpaidDealer
    {
        public int idno { get; set; }
        public string optcode { get; set; }
        public string opttype { get; set; }
        
        public string OperatorName { get; set; }
        public decimal? comm { get; set; }
        public decimal? gst { get; set; }
        public string Email { get; set; }
        public string userid { get; set; }
        public string RolesName { get; set; }
        public string DealerId { get; set; }

    }
    public class PostpaiddlmRetailer
    {
        public int idno { get; set; }
        public string optcode { get; set; }
        public string opttype { get; set; }

        public string OperatorName { get; set; }
        public decimal? comm { get; set; }
        public decimal? gst { get; set; }
        public string Email { get; set; }
        public string userid { get; set; }
        public string RolesName { get; set; }
        public string DealerId { get; set; }

    }
    public class UpdatePostpaidDealer
    {
        public int idno { get; set; }
        public decimal? comm { get; set; }
        public decimal? gst { get; set; }
        public string OperatorName { get; set; }
        public string optcode { get; set; }
        public string opttype { get; set; }
    }

    public class PostpaidRetailer
    {
        public int idno { get; set; }
        public string optcode { get; set; }
        public string opttype { get; set; }
        public string OperatorName { get; set; }
        public decimal? comm { get; set; }
        public decimal? gst { get; set; }
        public string Email { get; set; }
        public string userid { get; set; }
        public string RolesName { get; set; }
        public string RetailerId { get; set; }
        public decimal? surcharge { set; get; }
    }
    public class UpdatePostpaiddlmrem
    {
        public int idno { get; set; }
        public decimal? comm { get; set; }
        public decimal? gst { get; set; }
        public string OperatorName { get; set; }
        public string optcode { get; set; }
        public decimal? surcharge { set; get; }
        public string opttype { get; set; }
    }
        public class UpdatePostpaidRetailer
    {
        public int idno { get; set; }
        public decimal? comm { get; set; }
        public decimal? gst { get; set; }
        public string OperatorName { get; set; }
        public string optcode { get; set; }
        public decimal? surcharge { set; get; }
        public string opttype { get; set; }
    }

    public class PostpaidAPI
    {
        public int idno { get; set; }
        public string optcode { get; set; }
        public string opttype { get; set; }
        
        public string OperatorName { get; set; }
        public decimal? comm { get; set; }
        public decimal? gst { get; set; }
        public string Email { get; set; }
        public string userid { get; set; }
        public string RolesName { get; set; }
        public string apiid { get; set; }

    }
    public class UpdatePostpaidAPI
    {
        public int idno { get; set; }
        public decimal? comm { get; set; }
        public decimal? gst { get; set; }
        public string OperatorName { get; set; }
        public string optcode { get; set; }
        public string opttype { get; set; }
    }

    public class PostpaidWhitelabel
    {
        public int idno { get; set; }
        public string optcode { get; set; }
        public string opttype { get; set; }
        public string OperatorName { get; set; }
        public decimal? comm { get; set; }
        public decimal? gst { get; set; }
        public string Email { get; set; }
        public string userid { get; set; }
        public string RolesName { get; set; }
        public string WhiteLabelID { get; set; }

    }
    public class UpdatePostpaidWhitelabel
    {
        public int idno { get; set; }
        public decimal? comm { get; set; }
        public decimal? gst { get; set; }
        public string OperatorName { get; set; }
        public string optcode { get; set; }
        public string opttype { get; set; }
    }
}