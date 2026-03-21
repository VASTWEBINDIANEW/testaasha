using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vastwebmulti.Models;
namespace Vastwebmulti.Areas.WHITELABEL.Models
{
    public class PostpaidUtilitySlab
    {
        public List<PostpaidCommon> common { get; set; }
        public List<SelectListItem> UserId { get; set; }
        public List<SelectListItem> RetailerId { get; set; }
        public List<PostpaidDealer> Dealeruser { get; set; }
        public List<PostpaidMaster> Materuser { get; set; }
        public List<UpdatePostpaidDealer> UpdateDealer { get; set; }
        public List<UpdatePostpaidMaster> UpdateMaster { get; set; }
        public List<PostpaidRetailer> Retaileruser { get; set; }
        public List<UpdatePostpaidRetailer> UpdateRetailer { get; set; }
        public List<SelectListItem> OPttype { get; set; }
        public List<PostpaiddlmRetailer> dlmremuser { get; set; }
        public List<chargetypecomm> chargetype { get; set; }
        public List<UpdatePostpaiddlmrem> Updatedlmrem { get; set; }
        public string role { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }
    public class PostpaidCommon
    {
        public int idno { get; set; }
        public string optcode { get; set; }
        public string OperatorName { get; set; }
        public decimal? dlmcomm { get; set; }
        public decimal? retailercomm { get; set; }
        public decimal? gst { get; set; }
        public string whitelabelid { get; set; }
        public decimal? mastercomm { get; set; }
        public decimal? dlm_rem_comm { get; set; }
        public decimal? surcharge { set; get; }
        public string opttype { get; set; }
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
        public string whitelabelid { get; set; }
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
        public string whitelabelid { get; set; }
    }
    public class PostpaidDealer
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
        public string whitelabelid { get; set; }
        public string opttype { get; set; }
    }
    public class UpdatePostpaidDealer
    {
        public int idno { get; set; }
        public decimal? comm { get; set; }
        public decimal? gst { get; set; }
        public string OperatorName { get; set; }
        public string optcode { get; set; }
        public string opttype { get; set; }
        public string whitelabelid { get; set; }

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
    public class PostpaidRetailer
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
        public string whitelabelid { get; set; }
        public string opttype { get; set; }
        public decimal? surcharge { get; set; }

    }
    public class UpdatePostpaidRetailer
    {
        public int idno { get; set; }
        public decimal? comm { get; set; }
        public decimal? gst { get; set; }
        public string OperatorName { get; set; }
        public string optcode { get; set; }
        public decimal? surcharge { get; set; }
        public string opttype { get; set; }
        public string whitelabelid { get; set; }

    }
    public class UpdatePostpaidMaster
    {
        public int idno { get; set; }
        public decimal? comm { get; set; }
        public decimal? gst { get; set; }
        public string OperatorName { get; set; }
        public string optcode { get; set; }
        public string opttype { get; set; }
        public string Whitelabelid { get; set; }
    }
    public class chargetypecomm
    {
        public string rch_type { get; set; }
        public string user_type { get; set; }
        public bool? fixedrs { get; set; }
    }
}