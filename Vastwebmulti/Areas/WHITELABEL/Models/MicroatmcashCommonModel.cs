using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.WHITELABEL.Models
{
    public class MicroatmcashCommonModel
    {
        public List<Microatmcash_comm_new> userWise { get; set; }
        public List<WhitelabelId_Microatmcash_common_comm_new> common { get; set; }
        public List<MicroatmcashMasterComm> Materuser { get; set; }
        public List<MicroatmcashDealerComm> Dealeruser { get; set; }
        public List<MicroatmcashRetailerComm> Retaileruser { get; set; }
        public List<MicroatmcashWhitelabelComm> Whitelabeluser { get; set; }
        public List<MicroatmcashUpdateMasterComm> UpdateMaster { get; set; }
        public List<MicroatmcashUpdateDealerComm> UpdateDealer { get; set; }
        public List<MicroatmcashUpdateRetailerComm> UpdateRetailer { get; set; }
        public List<MicroatmUpdateWhitelabelComm> UpdateWhitelabel { get; set; }
        public List<SelectListItem> UserId { get; set; }
        public List<SelectListItem> Retailerid { get; set; }
        public string role { get; set; }
        public bool GstOnly { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }
    public class MicroatmcashMasterComm
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
    public class MicroatmcashUpdateMasterComm
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public decimal? comm { get; set; }
        public decimal? maxrs { get; set; }
        public decimal? minbal { get; set; }
        public decimal? M_statement { get; set; }
        public decimal? aadharpay { get; set; }
    }
    public class MicroatmcashDealerComm
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
    public class MicroatmcashUpdateDealerComm
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public decimal? comm { get; set; }
        public decimal? maxrs { get; set; }
        public decimal? minbal { get; set; }
        public decimal? M_statement { get; set; }
        public decimal? aadharpay { get; set; }

    }
    public class MicroatmcashRetailerComm
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
    public class MicroatmcashUpdateRetailerComm
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public decimal? comm { get; set; }
        public decimal? maxrs { get; set; }
        public decimal? minbal { get; set; }
        public decimal? M_statement { get; set; }
        public decimal? aadharpay { get; set; }

    }
    public class MicroatmcashWhitelabelComm
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
    public class MicroatmUpdateWhitelabelComm
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