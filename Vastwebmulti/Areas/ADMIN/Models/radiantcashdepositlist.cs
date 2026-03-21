using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    public class radiantcashdepositlist
    {
        public string Userid { get; set; }
        public string Firmanme { get; set; }
        public decimal? Amount { get; set; }
        public decimal? Remainpre { get; set; }
        public decimal? Remainpost { get; set; }
        public decimal? Adminremainpre { get; set; }
        public decimal? Adminremainpost { get; set; }
        public DateTime? Insertdate { get; set; }
        public DateTime? Updatedate { get; set; }
        public string Status { get; set; }
        public string BankName { get; set; }
        public string Ifsccode { get; set; }
        public string Accountnumber { get; set; }
        public decimal? Charge { get; set; }
        public decimal? FinalCharge { get; set; }
        public decimal? Gst { get; set; }
        public decimal? Tds { get; set; }
        public string Mode { get; set; }
        public string Slipid { get; set; }
        public string Slipname { get; set; }
        public string RejectedReson { get; set; }
        public string ResponseBy { get; set; }
        public string Requestid { get; set; }
    }
}