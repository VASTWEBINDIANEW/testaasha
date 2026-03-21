using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    /// <summary>
    /// Model for handling API callback request and response data.
    /// </summary>
    public class ApiCallBack
    {
        public int idno { get; set; }
        public int id { get; set; }
        public string apiname { get; set; }
        public string Requesttype { get; set; }
        public string bodytype { get; set; }
        public string keystatus { get; set; }
        public string valstatussuccess { get; set; }
        public string valstatusfailed { get; set; }
        public string keyrchid { get; set; }
        public string keymsg { get; set; }
        public string keyoperatorid { get; set; }
        public string keybalance { get; set; }
    }
}