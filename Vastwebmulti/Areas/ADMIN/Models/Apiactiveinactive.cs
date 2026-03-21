using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    public class Apiactiveinactive
    {
        public IEnumerable<Vastwebmulti.Models.show_all_active_inactive_api_list_Result> active { get; set; }
        public IEnumerable<Vastwebmulti.Models.show_all_active_inactive_api_list_Result> inactive { get; set; }
    }
}