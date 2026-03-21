using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.WHITELABEL.Models
{
  
        public class MposSlabModel
        {
            public List<Common_whitelabel_Mpos_comm> common { get; set; }

         

            public List<DealerMposComm> DealerMposUser { get; set; }

            public List<UpdateDealerMposComm> UpdateDealerMposUser { get; set; }

            public List<RetailerMposComm> RetailerMposUser { get; set; }

            public List<UpdateRetailerMposComm> UpdateRetailerMposUser { get; set; }

              public List<SelectListItem> UserId { get; set; }
             public string role { get; set; }
        }



        public class DealerMposComm
        {
            public int Idno { get; set; }
            public string Userid { get; set; }
        public string whitelabelid { get; set; }
            public string Email { get; set; }
            public string UserRole { get; set; }
            public string credite_type { get; set; }
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
        public string whitelabelid { get; set; }
        public string UserRole { get; set; }
            public string credite_type { get; set; }
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
            public decimal? CashWithdraw { get; set; }
            public decimal? Salesdebitupto2000 { get; set; }
            public decimal? Salesdebitabove2000 { get; set; }
            public decimal? SalescreditNormal { get; set; }
            public decimal? Salescreditgrocery { get; set; }
            public decimal? SalescreditEduandInsu { get; set; }
            public decimal? gst { get; set; }

        }

      
   
}