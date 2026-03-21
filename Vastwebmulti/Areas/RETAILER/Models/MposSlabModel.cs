using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.RETAILER.Models
{
    public class MposSlabModel
    {
        public List<Common_Mpos_comm> common { get; set; }

        public List<MasterMposComm> MasterMposUser { get; set; }

        public List<UpdateMasterMposComm> UpdateMposUser { get; set; }

        public List<DealerMposComm> DealerMposUser { get; set; }

        public List<UpdateDealerMposComm> UpdateDealerMposUser { get; set; }

        public List<RetailerMposComm> RetailerMposUser { get; set; }

        public List<UpdateRetailerMposComm> UpdateRetailerMposUser { get; set; }

        public List<WhitelabelMposComm> WhitelabelMposUser { get; set; }

        public List<UpdateWhitelabelMposComm> UpdateWhitelabelMposUser { get; set; }
        public List<SelectListItem> UserId { get; set; }
        public List<SelectListItem> Retailerid { get; set; }
        public List<SelectListItem> credit_type { get; set; }
        public string role { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }

    public class MasterMposComm
    {
        public int Idno { get; set; }
        public string Userid { get; set; }
        public string Email { get; set; }
        public string UserRole { get; set; }
        public string credite_type { get; set; }
        public decimal? maxcashcomm { get; set; }
        public decimal? prepaidcard { get; set; }
        public decimal? CashWithdraw { get; set; }
        public decimal? salesDebitUpto2000 { get; set; }
        public decimal? salesDebitAbove2000 { get; set; }
        public decimal? saleCredit { get; set; }
        public decimal? saleCreditGrocery { get; set; }
        public decimal? saleCreditEduAndIns { get; set; }
        public decimal? gst { get; set; }


    }
    public class UpdateMasterMposComm
    {
        public int idno { get; set; }
        public string Userid { get; set; }
        public string Email { get; set; }
        public string UserRole { get; set; }
        public decimal? saleCredit { get; set; }
        public decimal? maxcashcomm { get; set; }
        public decimal? prepaidcard { get; set; }
        public decimal? CashWithdraw { get; set; }
        public decimal? Salesdebitupto2000 { get; set; }
        public decimal? Salesdebitabove2000 { get; set; }
        public decimal? SalescreditNormal { get; set; }
        public decimal? Salescreditgrocery { get; set; }
        public decimal? SalescreditEduandInsu { get; set; }
        public decimal? gst { get; set; }

    }

    public class DealerMposComm
    {
        public int Idno { get; set; }
        public string Userid { get; set; }
        public string Email { get; set; }
        public string UserRole { get; set; }
        public string credite_type { get; set; }
        public decimal? maxcashcomm { get; set; }
        public decimal? prepaidcard { get; set; }
        public decimal? CashWithdraw { get; set; }
        public decimal? salesDebitUpto2000 { get; set; }
        public decimal? salesDebitAbove2000 { get; set; }
        public decimal? saleCredit { get; set; }
        public decimal? saleCreditGrocery { get; set; }
        public decimal? saleCreditEduAndIns { get; set; }
        public decimal? gst { get; set; }

    }
    public class UpdateDealerMposComm
    {
        public int idno { get; set; }
        public string Userid { get; set; }
        public string Email { get; set; }
        public string UserRole { get; set; }
        public decimal? maxcashcomm { get; set; }
        public decimal? prepaidcard { get; set; }
        public decimal? CashWithdraw { get; set; }
        public decimal? Salesdebitupto2000 { get; set; }
        public decimal? Salesdebitabove2000 { get; set; }
        public decimal? SalescreditNormal { get; set; }
        public decimal? Salescreditgrocery { get; set; }
        public decimal? SalescreditEduandInsu { get; set; }
        public decimal? gst { get; set; }

    }

    public class RetailerMposComm
    {
        public int Idno { get; set; }
        public string Userid { get; set; }
        public string Email { get; set; }
        public string UserRole { get; set; }
        public string credite_type { get; set; }
        public decimal? maxcashcomm { get; set; }
        public decimal? prepaidcard { get; set; }
        public decimal? CashWithdraw { get; set; }
        public decimal? salesDebitUpto2000 { get; set; }
        public decimal? salesDebitAbove2000 { get; set; }
        public decimal? saleCredit { get; set; }
        public decimal? saleCreditGrocery { get; set; }
        public decimal? saleCreditEduAndIns { get; set; }
        public decimal? gst { get; set; }

    }
    public class UpdateRetailerMposComm
    {
        public int idno { get; set; }
        public string Userid { get; set; }
        public string Email { get; set; }
        public string UserRole { get; set; }
        public decimal? maxcashcomm { get; set; }
        public decimal? prepaidcard { get; set; }
        public decimal? CashWithdraw { get; set; }
        public decimal? Salesdebitupto2000 { get; set; }
        public decimal? Salesdebitabove2000 { get; set; }
        public decimal? SalescreditNormal { get; set; }
        public decimal? Salescreditgrocery { get; set; }
        public decimal? SalescreditEduandInsu { get; set; }
        public decimal? gst { get; set; }

    }

    public class WhitelabelMposComm
    {
        public int Idno { get; set; }
        public string Userid { get; set; }
        public string Email { get; set; }
        public string UserRole { get; set; }
        public string credite_type { get; set; }
        public decimal? maxcashcomm { get; set; }
        public decimal? prepaidcard { get; set; }
        public decimal? CashWithdraw { get; set; }
        public decimal? salesDebitUpto2000 { get; set; }
        public decimal? salesDebitAbove2000 { get; set; }
        public decimal? saleCredit { get; set; }
        public decimal? saleCreditGrocery { get; set; }
        public decimal? saleCreditEduAndIns { get; set; }
        public decimal? gst { get; set; }

    }
    public class UpdateWhitelabelMposComm
    {
        public int idno { get; set; }
        public string Userid { get; set; }
        public string Email { get; set; }
        public string UserRole { get; set; }
        public decimal? maxcashcomm { get; set; }
        public decimal? prepaidcard { get; set; }
        public decimal? CashWithdraw { get; set; }
        public decimal? Salesdebitupto2000 { get; set; }
        public decimal? Salesdebitabove2000 { get; set; }
        public decimal? SalescreditNormal { get; set; }
        public decimal? Salescreditgrocery { get; set; }
        public decimal? SalescreditEduandInsu { get; set; }
        public decimal? gst { get; set; }

    }
    public class AepsSlabModel
    {
        public List<Common_AEPS_comm> common { get; set; }
        public List<AEPS_comm_details> userWise { get; set; }
        public AEPS_comm_details UpdateAll { get; set; }
        public List<SelectListItem> UserId { get; set; }
        public string role { get; set; }
        public bool GstOnly { get; set; }
    }
}