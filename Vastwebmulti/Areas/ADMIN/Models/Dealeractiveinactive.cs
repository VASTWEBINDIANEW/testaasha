using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    /// <summary>
    /// Model for toggling the active/inactive status of a dealer account.
    /// </summary>
    public class Dealeractiveinactive
    {
        public IEnumerable<Vastwebmulti.Models.show_all_active_inactive_dlm_list_Result> active { set; get; }
        public IEnumerable<Vastwebmulti.Models.show_all_active_inactive_dlm_list_Result> inactive { set; get; }
    }
}