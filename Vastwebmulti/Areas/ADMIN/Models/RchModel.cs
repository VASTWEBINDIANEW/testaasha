using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    public class GoodVM
    {
        public int id { set; get; }
        public string Rch_id { set; get; }
        public string Opt { set; get; }
        public string Optcode { set; get; }

        public string Email { get; set; }
        public decimal? comm { set; get; }
    }
    public class RchModel
    {
        public IEnumerable<Vastwebmulti.Models.Select_manual_opt_Result> show_all { get; set; }
        //public IEnumerable<PayNowRecharge.Models.Rch_purcharge> Rch_purcharge { get; set; }

        public List<GoodVM> Rch_purcharge { get; set; }
    }
}