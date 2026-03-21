using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    /// <summary>
    /// View model for managing MicroATM service commission slab configurations.
    /// </summary>
    public class MicroAtmSlabModel
    {
        public List<Common_Microatm_comm> common { get; set; }
        public List<MasterMicroatmComm> MasterMicroatmUser { get; set; }
        public List<UpdateMasterMicroatmComm> UpdateMicroatmUser { get; set; }
        public List<DealerMicroatmComm> DealerMicroatmUser { get; set; }
        public List<UpdateDealerMicroatmComm> UpdateDealerMicroatmUser { get; set; }
        public List<RetailerMicroatmComm> RetailerMicroatmUser { get; set; }
        public List<UpdateRetailerMicroatmComm> UpdateRetailerMicroatmUser { get; set; }
        public List<WhitelabelMicroatmComm> WhitelabelMicroatmUser { get; set; }
        public List<UpdateWhitelabelMicroatmComm> UpdateWhitelabelMicroatmUser { get; set; }
        public List<SelectListItem> UserId { get; set; }
        public List<SelectListItem> Retailerid { get; set; }
        public List<SelectListItem> credit_type { get; set; }
        public string role { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }
    public class MasterMicroatmComm
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public string Email { get; set; }
        public string usertype { get; set; }
        public string chargetype { get; set; }
        public string cardtype { get; set; }
        public string cardcategorytype { get; set; }
        public string cardname { get; set; }
        public decimal? charge { get; set; }
        public decimal? minvalue { get; set; }
        public decimal? maxvalue { get; set; }
    }
    public class UpdateMasterMicroatmComm
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public string Email { get; set; }
        public string usertype { get; set; }
        public string chargetype { get; set; }
        public string cardtype { get; set; }
        public string cardcategorytype { get; set; }
        public string cardname { get; set; }
        public decimal? charge { get; set; }
        public decimal? minvalue { get; set; }
        public decimal? maxvalue { get; set; }

    }
    public class DealerMicroatmComm
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public string Email { get; set; }
        public string usertype { get; set; }
        public string chargetype { get; set; }
        public string cardtype { get; set; }
        public string cardcategorytype { get; set; }
        public string cardname { get; set; }
        public decimal? charge { get; set; }
        public decimal? minvalue { get; set; }
        public decimal? maxvalue { get; set; }
    }
    public class UpdateDealerMicroatmComm
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public string Email { get; set; }
        public string usertype { get; set; }
        public string chargetype { get; set; }
        public string cardtype { get; set; }
        public string cardcategorytype { get; set; }
        public string cardname { get; set; }
        public decimal? charge { get; set; }
        public decimal? minvalue { get; set; }
        public decimal? maxvalue { get; set; }
    }
    public class RetailerMicroatmComm
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public string Email { get; set; }
        public string usertype { get; set; }
        public string chargetype { get; set; }
        public string cardtype { get; set; }
        public string cardcategorytype { get; set; }
        public string cardname { get; set; }
        public decimal? charge { get; set; }
        public decimal? minvalue { get; set; }
        public decimal? maxvalue { get; set; }
    }
    public class UpdateRetailerMicroatmComm
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public string Email { get; set; }
        public string usertype { get; set; }
        public string chargetype { get; set; }
        public string cardtype { get; set; }
        public string cardcategorytype { get; set; }
        public string cardname { get; set; }
        public decimal? charge { get; set; }
        public decimal? minvalue { get; set; }
        public decimal? maxvalue { get; set; }
    }
    public class WhitelabelMicroatmComm
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public string Email { get; set; }
        public string usertype { get; set; }
        public string chargetype { get; set; }
        public string cardtype { get; set; }
        public string cardcategorytype { get; set; }
        public string cardname { get; set; }
        public decimal? charge { get; set; }
        public decimal? minvalue { get; set; }
        public decimal? maxvalue { get; set; }
    }
    public class UpdateWhitelabelMicroatmComm
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public string Email { get; set; }
        public string usertype { get; set; }
        public string chargetype { get; set; }
        public string cardtype { get; set; }
        public string cardcategorytype { get; set; }
        public string cardname { get; set; }
        public decimal? charge { get; set; }
        public decimal? minvalue { get; set; }
        public decimal? maxvalue { get; set; }
    }
}