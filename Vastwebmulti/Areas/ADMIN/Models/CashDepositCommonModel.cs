using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    public class CashDepositCommonModel
    {
        public List<CashDeposit_comm_userwise> userWise { get; set; }
        public List<CashDeposit_Common_Comm> common { get; set; }
        public List<CashDepositMasterComm> Materuser { get; set; }
        public List<CashDepositDealerComm> Dealeruser { get; set; }
        public List<CashDepositRetailerComm> Retaileruser { get; set; }
        public List<CashDepositWhitelabelComm> Whitelabeluser { get; set; }
        public List<CashDepositUpdateMasterComm> UpdateMaster { get; set; }
        public List<CashDepositUpdateDealerComm> UpdateDealer { get; set; }
        public List<CashDepositUpdateRetailerComm> UpdateRetailer { get; set; }
        public List<CashDepositUpdateWhitelabelComm> UpdateWhitelabel { get; set; }
        public List<SelectListItem> UserId { get; set; }
        public List<SelectListItem> Retailerid { get; set; }
        public string role { get; set; }
        public bool GstOnly { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }
    public class CashDepositMasterComm
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public string Email { get; set; }
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
    public class CashDepositUpdateMasterComm
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public string Email { get; set; }
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
    public class CashDepositDealerComm
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public string Email { get; set; }
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
    public class CashDepositUpdateDealerComm
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public string Email { get; set; }
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
    public class CashDepositRetailerComm
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public string Email { get; set; }
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
    public class CashDepositUpdateRetailerComm
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public string Email { get; set; }
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
    public class CashDepositWhitelabelComm
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public string RolesName { get; set; }
        public string Email { get; set; }
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
    public class CashDepositUpdateWhitelabelComm
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public string Email { get; set; }
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