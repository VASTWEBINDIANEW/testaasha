using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vastwebmulti.Models;
namespace Vastwebmulti.Areas.ADMIN.Models
{
    public class AEPSCommonModel
    {
        public List<Aeps_comm_userwise> userWise { get; set; }
        public List<Aeps_Common_Comm> common { get; set; }
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
        public decimal? aadharpay { get; set; }
        public decimal? ministatement { get; set; }
        public decimal? per_500_999 { get; set; }
        public decimal? rs_500_999 { get; set; }
        public decimal? maxrs_500_999 { get; set; }
        public string Type_500_999 { get; set; }
        public decimal? per_1000_1499 { get; set; }
        public decimal? rs_1000_1499 { get; set; }
        public decimal? maxrs_1000_1499 { get; set; }
        public string Type_1000_1499 { get; set; }
        public decimal? per_1500_1999 { get; set; }
        public decimal? rs_1500_1999 { get; set; }
        public decimal? maxrs_1500_1999 { get; set; }
        public string Type_1500_1999 { get; set; }
        public decimal? per_2000_2499 { get; set; }
        public decimal? rs_2000_2499 { get; set; }
        public decimal? maxrs_2000_2499 { get; set; }
        public string Type_2000_2499 { get; set; }
        public decimal? per_2500_2999 { get; set; }
        public decimal? rs_2500_2999 { get; set; }
        public decimal? maxrs_2500_2999 { get; set; }
        public string Type_2500_2999 { get; set; }
        public decimal? per_3000_3499 { get; set; }
        public decimal? rs_3000_3499 { get; set; }
        public decimal? maxrs_3000_3499 { get; set; }
        public string Type_3000_3499 { get; set; }
        public decimal? per_3500_5000 { get; set; }
        public decimal? rs_3500_5000 { get; set; }
        public decimal? maxrs_3500_5000 { get; set; }
        public string Type_3500_5000 { get; set; }
        public decimal? per_5001_10000 { get; set; }
        public decimal? rs_5001_10000 { get; set; }
        public decimal? maxrs_5001_10000 { get; set; }
        public string Type_5001_10000 { get; set; }
        public string RolesName { get; set; }
    }
    public class AEPSUpdateMasterComm
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public string Email { get; set; }
        public decimal? aadharpay { get; set; }
        public decimal? ministatement { get; set; }
        public decimal? per_500_999 { get; set; }
        public decimal? rs_500_999 { get; set; }
        public decimal? maxrs_500_999 { get; set; }
        public string Type_500_999 { get; set; }
        public decimal? per_1000_1499 { get; set; }
        public decimal? rs_1000_1499 { get; set; }
        public decimal? maxrs_1000_1499 { get; set; }
        public string Type_1000_1499 { get; set; }
        public decimal? per_1500_1999 { get; set; }
        public decimal? rs_1500_1999 { get; set; }
        public decimal? maxrs_1500_1999 { get; set; }
        public string Type_1500_1999 { get; set; }
        public decimal? per_2000_2499 { get; set; }
        public decimal? rs_2000_2499 { get; set; }
        public decimal? maxrs_2000_2499 { get; set; }
        public string Type_2000_2499 { get; set; }
        public decimal? per_2500_2999 { get; set; }
        public decimal? rs_2500_2999 { get; set; }
        public decimal? maxrs_2500_2999 { get; set; }
        public string Type_2500_2999 { get; set; }
        public decimal? per_3000_3499 { get; set; }
        public decimal? rs_3000_3499 { get; set; }
        public decimal? maxrs_3000_3499 { get; set; }
        public string Type_3000_3499 { get; set; }
        public decimal? per_3500_5000 { get; set; }
        public decimal? rs_3500_5000 { get; set; }
        public decimal? maxrs_3500_5000 { get; set; }
        public string Type_3500_5000 { get; set; }
        public decimal? per_5001_10000 { get; set; }
        public decimal? rs_5001_10000 { get; set; }
        public decimal? maxrs_5001_10000 { get; set; }
        public string Type_5001_10000 { get; set; }
    }
    public class AEPSDealerComm
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public string Email { get; set; }
        public decimal? aadharpay { get; set; }
        public decimal? ministatement { get; set; }
        public decimal? per_500_999 { get; set; }
        public decimal? rs_500_999 { get; set; }
        public decimal? maxrs_500_999 { get; set; }
        public string Type_500_999 { get; set; }
        public decimal? per_1000_1499 { get; set; }
        public decimal? rs_1000_1499 { get; set; }
        public decimal? maxrs_1000_1499 { get; set; }
        public string Type_1000_1499 { get; set; }
        public decimal? per_1500_1999 { get; set; }
        public decimal? rs_1500_1999 { get; set; }
        public decimal? maxrs_1500_1999 { get; set; }
        public string Type_1500_1999 { get; set; }
        public decimal? per_2000_2499 { get; set; }
        public decimal? rs_2000_2499 { get; set; }
        public decimal? maxrs_2000_2499 { get; set; }
        public string Type_2000_2499 { get; set; }
        public decimal? per_2500_2999 { get; set; }
        public decimal? rs_2500_2999 { get; set; }
        public decimal? maxrs_2500_2999 { get; set; }
        public string Type_2500_2999 { get; set; }
        public decimal? per_3000_3499 { get; set; }
        public decimal? rs_3000_3499 { get; set; }
        public decimal? maxrs_3000_3499 { get; set; }
        public string Type_3000_3499 { get; set; }
        public decimal? per_3500_5000 { get; set; }
        public decimal? rs_3500_5000 { get; set; }
        public decimal? maxrs_3500_5000 { get; set; }
        public string Type_3500_5000 { get; set; }
        public decimal? per_5001_10000 { get; set; }
        public decimal? rs_5001_10000 { get; set; }
        public decimal? maxrs_5001_10000 { get; set; }
        public string Type_5001_10000 { get; set; }
        public string RolesName { get; set; }
    }
    public class AEPSUpdateDealerComm
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public string Email { get; set; }
        public decimal? aadharpay { get; set; }
        public decimal? ministatement { get; set; }
        public decimal? per_500_999 { get; set; }
        public decimal? rs_500_999 { get; set; }
        public decimal? maxrs_500_999 { get; set; }
        public string Type_500_999 { get; set; }
        public decimal? per_1000_1499 { get; set; }
        public decimal? rs_1000_1499 { get; set; }
        public decimal? maxrs_1000_1499 { get; set; }
        public string Type_1000_1499 { get; set; }
        public decimal? per_1500_1999 { get; set; }
        public decimal? rs_1500_1999 { get; set; }
        public decimal? maxrs_1500_1999 { get; set; }
        public string Type_1500_1999 { get; set; }
        public decimal? per_2000_2499 { get; set; }
        public decimal? rs_2000_2499 { get; set; }
        public decimal? maxrs_2000_2499 { get; set; }
        public string Type_2000_2499 { get; set; }
        public decimal? per_2500_2999 { get; set; }
        public decimal? rs_2500_2999 { get; set; }
        public decimal? maxrs_2500_2999 { get; set; }
        public string Type_2500_2999 { get; set; }
        public decimal? per_3000_3499 { get; set; }
        public decimal? rs_3000_3499 { get; set; }
        public decimal? maxrs_3000_3499 { get; set; }
        public string Type_3000_3499 { get; set; }
        public decimal? per_3500_5000 { get; set; }
        public decimal? rs_3500_5000 { get; set; }
        public decimal? maxrs_3500_5000 { get; set; }
        public string Type_3500_5000 { get; set; }
        public decimal? per_5001_10000 { get; set; }
        public decimal? rs_5001_10000 { get; set; }
        public decimal? maxrs_5001_10000 { get; set; }
        public string Type_5001_10000 { get; set; }

    }
    public class AEPSRetailerComm
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public string Email { get; set; }
        public decimal? aadharpay { get; set; }
        public decimal? ministatement { get; set; }
        public decimal? per_500_999 { get; set; }
        public decimal? rs_500_999 { get; set; }
        public decimal? maxrs_500_999 { get; set; }
        public string Type_500_999 { get; set; }
        public decimal? per_1000_1499 { get; set; }
        public decimal? rs_1000_1499 { get; set; }
        public decimal? maxrs_1000_1499 { get; set; }
        public string Type_1000_1499 { get; set; }
        public decimal? per_1500_1999 { get; set; }
        public decimal? rs_1500_1999 { get; set; }
        public decimal? maxrs_1500_1999 { get; set; }
        public string Type_1500_1999 { get; set; }
        public decimal? per_2000_2499 { get; set; }
        public decimal? rs_2000_2499 { get; set; }
        public decimal? maxrs_2000_2499 { get; set; }
        public string Type_2000_2499 { get; set; }
        public decimal? per_2500_2999 { get; set; }
        public decimal? rs_2500_2999 { get; set; }
        public decimal? maxrs_2500_2999 { get; set; }
        public string Type_2500_2999 { get; set; }
        public decimal? per_3000_3499 { get; set; }
        public decimal? rs_3000_3499 { get; set; }
        public decimal? maxrs_3000_3499 { get; set; }
        public string Type_3000_3499 { get; set; }
        public decimal? per_3500_5000 { get; set; }
        public decimal? rs_3500_5000 { get; set; }
        public decimal? maxrs_3500_5000 { get; set; }
        public string Type_3500_5000 { get; set; }
        public decimal? per_5001_10000 { get; set; }
        public decimal? rs_5001_10000 { get; set; }
        public decimal? maxrs_5001_10000 { get; set; }
        public string Type_5001_10000 { get; set; }
        public string RolesName { get; set; }

    }
    public class AEPSUpdateRetailerComm
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public string Email { get; set; }
        public decimal? aadharpay { get; set; }
        public decimal? ministatement { get; set; }
        public decimal? per_500_999 { get; set; }
        public decimal? rs_500_999 { get; set; }
        public decimal? maxrs_500_999 { get; set; }
        public string Type_500_999 { get; set; }
        public decimal? per_1000_1499 { get; set; }
        public decimal? rs_1000_1499 { get; set; }
        public decimal? maxrs_1000_1499 { get; set; }
        public string Type_1000_1499 { get; set; }
        public decimal? per_1500_1999 { get; set; }
        public decimal? rs_1500_1999 { get; set; }
        public decimal? maxrs_1500_1999 { get; set; }
        public string Type_1500_1999 { get; set; }
        public decimal? per_2000_2499 { get; set; }
        public decimal? rs_2000_2499 { get; set; }
        public decimal? maxrs_2000_2499 { get; set; }
        public string Type_2000_2499 { get; set; }
        public decimal? per_2500_2999 { get; set; }
        public decimal? rs_2500_2999 { get; set; }
        public decimal? maxrs_2500_2999 { get; set; }
        public string Type_2500_2999 { get; set; }
        public decimal? per_3000_3499 { get; set; }
        public decimal? rs_3000_3499 { get; set; }
        public decimal? maxrs_3000_3499 { get; set; }
        public string Type_3000_3499 { get; set; }
        public decimal? per_3500_5000 { get; set; }
        public decimal? rs_3500_5000 { get; set; }
        public decimal? maxrs_3500_5000 { get; set; }
        public string Type_3500_5000 { get; set; }
        public decimal? per_5001_10000 { get; set; }
        public decimal? rs_5001_10000 { get; set; }
        public decimal? maxrs_5001_10000 { get; set; }
        public string Type_5001_10000 { get; set; }
    }
    public class AEPSWhitelabelComm
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public string RolesName { get; set; }
        public string Email { get; set; }
        public decimal? aadharpay { get; set; }
        public decimal? ministatement { get; set; }
        public decimal? per_500_999 { get; set; }
        public decimal? rs_500_999 { get; set; }
        public decimal? maxrs_500_999 { get; set; }
        public string Type_500_999 { get; set; }
        public decimal? per_1000_1499 { get; set; }
        public decimal? rs_1000_1499 { get; set; }
        public decimal? maxrs_1000_1499 { get; set; }
        public string Type_1000_1499 { get; set; }
        public decimal? per_1500_1999 { get; set; }
        public decimal? rs_1500_1999 { get; set; }
        public decimal? maxrs_1500_1999 { get; set; }
        public string Type_1500_1999 { get; set; }
        public decimal? per_2000_2499 { get; set; }
        public decimal? rs_2000_2499 { get; set; }
        public decimal? maxrs_2000_2499 { get; set; }
        public string Type_2000_2499 { get; set; }
        public decimal? per_2500_2999 { get; set; }
        public decimal? rs_2500_2999 { get; set; }
        public decimal? maxrs_2500_2999 { get; set; }
        public string Type_2500_2999 { get; set; }
        public decimal? per_3000_3499 { get; set; }
        public decimal? rs_3000_3499 { get; set; }
        public decimal? maxrs_3000_3499 { get; set; }
        public string Type_3000_3499 { get; set; }
        public decimal? per_3500_5000 { get; set; }
        public decimal? rs_3500_5000 { get; set; }
        public decimal? maxrs_3500_5000 { get; set; }
        public string Type_3500_5000 { get; set; }
        public decimal? per_5001_10000 { get; set; }
        public decimal? rs_5001_10000 { get; set; }
        public decimal? maxrs_5001_10000 { get; set; }
        public string Type_5001_10000 { get; set; }
    }
    public class AEPSUpdateWhitelabelComm
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public string Email { get; set; }
        public decimal? aadharpay { get; set; }
        public decimal? ministatement { get; set; }
        public decimal? per_500_999 { get; set; }
        public decimal? rs_500_999 { get; set; }
        public decimal? maxrs_500_999 { get; set; }
        public string Type_500_999 { get; set; }
        public decimal? per_1000_1499 { get; set; }
        public decimal? rs_1000_1499 { get; set; }
        public decimal? maxrs_1000_1499 { get; set; }
        public string Type_1000_1499 { get; set; }
        public decimal? per_1500_1999 { get; set; }
        public decimal? rs_1500_1999 { get; set; }
        public decimal? maxrs_1500_1999 { get; set; }
        public string Type_1500_1999 { get; set; }
        public decimal? per_2000_2499 { get; set; }
        public decimal? rs_2000_2499 { get; set; }
        public decimal? maxrs_2000_2499 { get; set; }
        public string Type_2000_2499 { get; set; }
        public decimal? per_2500_2999 { get; set; }
        public decimal? rs_2500_2999 { get; set; }
        public decimal? maxrs_2500_2999 { get; set; }
        public string Type_2500_2999 { get; set; }
        public decimal? per_3000_3499 { get; set; }
        public decimal? rs_3000_3499 { get; set; }
        public decimal? maxrs_3000_3499 { get; set; }
        public string Type_3000_3499 { get; set; }
        public decimal? per_3500_5000 { get; set; }
        public decimal? rs_3500_5000 { get; set; }
        public decimal? maxrs_3500_5000 { get; set; }
        public string Type_3500_5000 { get; set; }
        public decimal? per_5001_10000 { get; set; }
        public decimal? rs_5001_10000 { get; set; }
        public decimal? maxrs_5001_10000 { get; set; }
        public string Type_5001_10000 { get; set; }
    }
}