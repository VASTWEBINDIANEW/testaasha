using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vastwebmulti.Models;
namespace Vastwebmulti.Areas.ADMIN.Models
{
    public class pancard_reports
    {
        public IEnumerable<pancard_transation> pancard_Transations_report_new { get; set; }
        public IEnumerable<pancard_transation_manual> pancard_Transations_report_new_manual { get; set; }

        public IEnumerable<PAN_CARD_IPAY_Token_report_paging_Result> p_Transations_report_olds11;

      
    }
}