using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.RETAILER.Models
{
    public class SubService
    {
        public string name { get; set; }
        public string sub_product { get; set; }
        public string description { get; set; }
        public string package_key { get; set; }
        public string parent_name { get; set; }
        public string amount { get; set; }
    }

    public class Data
    {
        public string name { get; set; }
        public string description_api { get; set; }
        public string code { get; set; }
        public string type { get; set; }
        public IList<SubService> sub_services { get; set; }
    }

    public class DTH_Con
    {
        public string ipay_errorcode { get; set; }
        public string ipay_errordesc { get; set; }
        public Data data { get; set; }
    }
}