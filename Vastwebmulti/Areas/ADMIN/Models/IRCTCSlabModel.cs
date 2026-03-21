using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    /// <summary>
    /// View model for managing IRCTC (Indian Railway) ticket booking commission slab configurations.
    /// </summary>
    public class IRCTCSlabModel
    {
        public List<IRCTC_COMMON_SLAB> common { get; set; }
        public List<MasterIRCTCComm> Materuser { get; set; }
        public List<DealerIRCTCComm> Dealeruser { get; set; }
        public List<RetailerIRCTCComm> Retaileruser { get; set; }
        public List<dlmremIRCTCComm> dlmremuser { get; set; }
        public List<APIIRCTCComm> APIuser { get; set; }
        public List<WhitelabelIRCTCComm> Whitelabeluser { get; set; }
        public List<MasterUpdateIRCTCComm> UpdateMaster { get; set; }
        public List<DealerUpdateIRCTCComm> UpdateDealer { get; set; }
        public List<RetailerUpdateIRCTCComm> UpdateRetailer { get; set; }
        public List<dlmremUpdateIRCTCComm> Updatedlmrem { get; set; }
        public List<APIUpdateIRCTCComm> UpdateAPI { get; set; }

        public List<WhitelabelUpdateIRCTCComm> UpdateWhitelabel { get; set; }
        public List<SelectListItem> UserId { get; set; }
        public List<SelectListItem> Retailerid { get; set; }
        public string role { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }
    public class MasterIRCTCComm
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public string Email { get; set; }
        public string RolesName { get; set; }
        public decimal? tokenvalue { get; set; }
        public decimal? RenewalToken { get; set; }
        public decimal? commtokenval { get; set; }

    }
    public class MasterUpdateIRCTCComm
    {
        public int idno { get; set; }
        public decimal? tokenvalue { get; set; }
        public decimal? RenewalToken { get; set; }
        public decimal? commmastertokenval { get; set; }

    }
    public class DealerIRCTCComm
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public string Email { get; set; }
        public string RolesName { get; set; }
        public decimal? tokenvalue { get; set; }
        public decimal? RenewalToken { get; set; }
        public decimal? commdlmtokenval { get; set; }

    }
    public class DealerUpdateIRCTCComm
    {
        public int idno { get; set; }
        public decimal? tokenvalue { get; set; }
        public decimal? RenewalToken { get; set; }
        public decimal? commdlmtokenval { get; set; }

    }
    public class RetailerIRCTCComm
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public string Email { get; set; }
        public string RolesName { get; set; }
        public decimal? tokenvalue { get; set; }
        public decimal? RenewalToken { get; set; }



    }
    public class dlmremIRCTCComm
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public string Email { get; set; }
        public string RolesName { get; set; }
        public decimal? tokenvalue { get; set; }
        public decimal? RenewalToken { get; set; }



    }

    public class RetailerUpdateIRCTCComm
    {
        public int idno { get; set; }

        public decimal? tokenvalue { get; set; }
        public decimal? RenewalToken { get; set; }



    }
    public class dlmremUpdateIRCTCComm
    {
        public int idno { get; set; }

        public decimal? tokenvalue { get; set; }
        public decimal? RenewalToken { get; set; }



    }

    public class APIIRCTCComm
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public string Email { get; set; }
        public string RolesName { get; set; }
        public decimal? tokenvalue { get; set; }
        public decimal? RenewalToken { get; set; }
        public decimal? commtokenval { get; set; }
        public decimal? commapiTokenval { get; set; }


    }

    public class APIUpdateIRCTCComm
    {
        public int idno { get; set; }

        public decimal? tokenvalue { get; set; }
        public decimal? RenewalToken { get; set; }
        public decimal? commapitokenval { get; set; }


    }

    public class WhitelabelIRCTCComm
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public string Email { get; set; }
        public string RolesName { get; set; }
        public decimal? tokenvalue { get; set; }
        public decimal? RenewalToken { get; set; }
        public decimal? commwhitelabeltokenval { get; set; }


    }

    public class WhitelabelUpdateIRCTCComm
    {
        public int idno { get; set; }
        public decimal? tokenvalue { get; set; }
        public decimal? RenewalToken { get; set; }
        public decimal? commWhitelabeltokenval { get; set; }

    }

}