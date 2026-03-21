using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    public class MicroatmcashCommonModel
    {
        public List<mtm_Comm_User_Wise> userWise { get; set; }
        public List<mtm_Comm_comm> common { get; set; }
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
        public decimal? cash_0_1000_per { get; set; }
        public decimal? cash_1001_2000_per { get; set; }
        public decimal? cash_0_1000_rs { get; set; }
        public decimal? cash_1001_2000_rs { get; set; }
        public decimal? cash_0_1000_maxrs { get; set; }
        public decimal? cash_1001_2000_maxrs { get; set; }
        public string cash_0_1000_Type { get; set; }
        public string cash_1001_2000_Type { get; set; }
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
    public class MicroatmcashUpdateMasterComm
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public string Email { get; set; }
        public string RolesName { get; set; }
        public decimal? cash_0_1000_per { get; set; }
        public decimal? cash_1001_2000_per { get; set; }
        public decimal? cash_0_1000_rs { get; set; }
        public decimal? cash_1001_2000_rs { get; set; }
        public decimal? cash_0_1000_maxrs { get; set; }
        public decimal? cash_1001_2000_maxrs { get; set; }
        public string cash_0_1000_Type { get; set; }
        public string cash_1001_2000_Type { get; set; }
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
    public class MicroatmcashDealerComm
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public string Email { get; set; }
        public string RolesName { get; set; }
        public decimal? cash_0_1000_per { get; set; }
        public decimal? cash_1001_2000_per { get; set; }
        public decimal? cash_0_1000_rs { get; set; }
        public decimal? cash_1001_2000_rs { get; set; }
        public decimal? cash_0_1000_maxrs { get; set; }
        public decimal? cash_1001_2000_maxrs { get; set; }
        public string cash_0_1000_Type { get; set; }
        public string cash_1001_2000_Type { get; set; }
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
    public class MicroatmcashUpdateDealerComm
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public string Email { get; set; }
        public string RolesName { get; set; }
        public decimal? cash_0_1000_per { get; set; }
        public decimal? cash_1001_2000_per { get; set; }
        public decimal? cash_0_1000_rs { get; set; }
        public decimal? cash_1001_2000_rs { get; set; }
        public decimal? cash_0_1000_maxrs { get; set; }
        public decimal? cash_1001_2000_maxrs { get; set; }
        public string cash_0_1000_Type { get; set; }
        public string cash_1001_2000_Type { get; set; }
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
    public class MicroatmcashRetailerComm
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public string Email { get; set; }
        public string RolesName { get; set; }
        public decimal? cash_0_1000_per { get; set; }
        public decimal? cash_1001_2000_per { get; set; }
        public decimal? cash_0_1000_rs { get; set; }
        public decimal? cash_1001_2000_rs { get; set; }
        public decimal? cash_0_1000_maxrs { get; set; }
        public decimal? cash_1001_2000_maxrs { get; set; }
        public string cash_0_1000_Type { get; set; }
        public string cash_1001_2000_Type { get; set; }
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
    public class MicroatmcashUpdateRetailerComm
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public string Email { get; set; }
        public string RolesName { get; set; }
        public decimal? cash_0_1000_per { get; set; }
        public decimal? cash_1001_2000_per { get; set; }
        public decimal? cash_0_1000_rs { get; set; }
        public decimal? cash_1001_2000_rs { get; set; }
        public decimal? cash_0_1000_maxrs { get; set; }
        public decimal? cash_1001_2000_maxrs { get; set; }
        public string cash_0_1000_Type { get; set; }
        public string cash_1001_2000_Type { get; set; }
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
    public class MicroatmcashWhitelabelComm
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public string Email { get; set; }
        public string RolesName { get; set; }
        public decimal? cash_0_1000_per { get; set; }
        public decimal? cash_1001_2000_per { get; set; }
        public decimal? cash_0_1000_rs { get; set; }
        public decimal? cash_1001_2000_rs { get; set; }
        public decimal? cash_0_1000_maxrs { get; set; }
        public decimal? cash_1001_2000_maxrs { get; set; }
        public string cash_0_1000_Type { get; set; }
        public string cash_1001_2000_Type { get; set; }
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
    public class MicroatmUpdateWhitelabelComm
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public string Email { get; set; }
        public string RolesName { get; set; }
        public decimal? cash_0_1000_per { get; set; }
        public decimal? cash_1001_2000_per { get; set; }
        public decimal? cash_0_1000_rs { get; set; }
        public decimal? cash_1001_2000_rs { get; set; }
        public decimal? cash_0_1000_maxrs { get; set; }
        public decimal? cash_1001_2000_maxrs { get; set; }
        public string cash_0_1000_Type { get; set; }
        public string cash_1001_2000_Type { get; set; }
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