using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    public class GiftcardSlab
    {
        public List<GiftcardCommon> common { get; set; }
        public List<SelectListItem> UserId { get; set; }
        public List<SelectListItem> RetailerId { get; set; }
        public List<GiftcardMaster> Materuser { get; set; }
        public List<UpdateGiftcardMaster> UpdateMaster { get; set; }
        public List<GiftcardDealer> Dealeruser { get; set; }
        public List<UpdateGiftcardDealer> UpdateDealer { get; set; }
        public List<GiftcardRetailer> Retaileruser { get; set; }
        public List<UpdateGiftcardRetailer> UpdateRetailer { get; set; }
          public List<GiftcardWhitelabel> Whitelabeluser { get; set; }
        public List<UpdateGiftcardWhitelabel> UpdateWhitelabel { get; set; }
        public string role { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
    }
    public class GiftcardCommon
    {
        public int idno { get; set; }
        public string optcode { get; set; }
        public string OperatorName { get; set; }
        public decimal? mastercomm { get; set; }
        public decimal? dlmcomm { get; set; }
        public decimal? retailercomm { get; set; }
        public decimal? whitelabelcomm { get; set; }

    }
    public class GiftcardMaster
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
    public class UpdateGiftcardMaster
    {
        public int idno { get; set; }
        public decimal? comm { get; set; }
        public string OperatorName { get; set; }
        public string optcode { get; set; }

    }

    public class GiftcardDealer
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
    public class UpdateGiftcardDealer
    {
        public int idno { get; set; }
        public decimal? comm { get; set; }
        public string OperatorName { get; set; }
        public string optcode { get; set; }

    }

    public class GiftcardRetailer
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
    public class UpdateGiftcardRetailer
    {
        public int idno { get; set; }
        public decimal? comm { get; set; }
        public string OperatorName { get; set; }
        public string optcode { get; set; }

    }

    public class GiftcardWhitelabel
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
    public class UpdateGiftcardWhitelabel
    {
        public int idno { get; set; }
        public decimal? comm { get; set; }
        public string OperatorName { get; set; }
        public string optcode { get; set; }

    }
}