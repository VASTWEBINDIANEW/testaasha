using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vastwebmulti.Models;
namespace Vastwebmulti.Areas.WHITELABEL.Models
{
    public class WhitelabelAEPSCommonModel
    {
        public List<Aeps_comm_new> userWise { get; set; }
        public List<Whitelabel_Aeps_common_comm_new> common { get; set; }
        public List<AEPSMasterComm> Materuser { get; set; }
        public List<AEPSDealerComm> Dealeruser { get; set; }
        public List<AEPSRetailerComm> Retaileruser { get; set; }
        public List<AEPSWhitelabelComm> Whitelabeluser { get; set; }
        public List<AEPSUpdateMasterComm> UpdateMaster { get; set; }
        public List<AEPSUpdateDealerComm> UpdateDealer { get; set; }
        public List<AEPSUpdateRetailerComm> UpdateRetailer { get; set; }
        public List<AEPSUpdateWhitelabelComm> UpdateWhitelabel { get; set; }
        public List<SelectListItem> UserId { get; set; }
        public List<SelectListItem> Retailerid { get; set; }
        public string role { get; set; }
        public bool GstOnly { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }
    public class AEPSMasterComm
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public string Email { get; set; }
        public string RolesName { get; set; }
        public decimal? comm { get; set; }
        public decimal? maxrs { get; set; }
        public decimal? minbal { get; set; }
        public decimal? M_statement { get; set; }
        public decimal? aadharpay { get; set; }
    }
    public class AEPSUpdateMasterComm
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public decimal? comm { get; set; }
        public decimal? maxrs { get; set; }
        public decimal? minbal { get; set; }
        public decimal? M_statement { get; set; }
        public decimal? aadharpay { get; set; }
    }
    public class AEPSDealerComm
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public string Email { get; set; }
        public string RolesName { get; set; }
        public decimal? comm { get; set; }
        public decimal? maxrs { get; set; }
        public decimal? minbal { get; set; }
        public decimal? M_statement { get; set; }
        public decimal? aadharpay { get; set; }
    }
    public class AEPSUpdateDealerComm
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public decimal? comm { get; set; }
        public decimal? maxrs { get; set; }
        public decimal? minbal { get; set; }
        public decimal? M_statement { get; set; }
        public decimal? aadharpay { get; set; }

    }
    public class AEPSRetailerComm
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public string Email { get; set; }
        public string RolesName { get; set; }
        public decimal? comm { get; set; }
        public decimal? maxrs { get; set; }
        public decimal? minbal { get; set; }
        public decimal? M_statement { get; set; }
        public decimal? aadharpay { get; set; }

    }
    public class AEPSUpdateRetailerComm
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public decimal? comm { get; set; }
        public decimal? maxrs { get; set; }
        public decimal? minbal { get; set; }
        public decimal? M_statement { get; set; }
        public decimal? aadharpay { get; set; }

    }
    public class AEPSWhitelabelComm
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public string Email { get; set; }
        public string RolesName { get; set; }
        public decimal? comm { get; set; }
        public decimal? maxrs { get; set; }
        public decimal? minbal { get; set; }
        public decimal? M_statement { get; set; }
        public decimal? aadharpay { get; set; }
    }
    public class AEPSUpdateWhitelabelComm
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public decimal? comm { get; set; }
        public decimal? maxrs { get; set; }
        public decimal? minbal { get; set; }
        public decimal? M_statement { get; set; }
        public decimal? aadharpay { get; set; }
    }
}