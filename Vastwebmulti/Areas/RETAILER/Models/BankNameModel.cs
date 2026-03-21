using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.WRetailer.Models
{
    public class BankNameModel
    {
        public string id { set; get; }
        public string bank_name { set; get; }
        public string imps_enabled { set; get; }
        public string aeps_enabled { set; get; }
        public string bank_sort_name { set; get; }
        public string branch_ifsc { set; get; }
        public string ifsc_alias { set; get; }
        public string bank_iin { set; get; }
        public string is_down { set; get; }
    }
    public class IndoNepalBankNameModel
    {
        public string branchid { set; get; }
        public string bank_name { set; get; }
        public string branch_name { set; get; }
        public string address { set; get; }
        public string is_down { set; get; }
    }
}