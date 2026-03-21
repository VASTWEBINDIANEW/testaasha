using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    public class Dealeractiveinactive
    {
        public IEnumerable<Vastwebmulti.Models.show_all_active_inactive_dlm_list_Result> active { set; get; }
        public IEnumerable<Vastwebmulti.Models.show_all_active_inactive_dlm_list_Result> inactive { set; get; }
    }
}