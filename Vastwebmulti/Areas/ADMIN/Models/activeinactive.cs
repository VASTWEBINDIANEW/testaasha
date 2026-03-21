using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    /// <summary>
    /// Model for toggling active/inactive status of a user account.
    /// </summary>
    public class activeinactive
    {
        public IEnumerable<Vastwebmulti.Models.show_all_active_inactive_rem_list_Result> active { get; set; }
        public IEnumerable<Vastwebmulti.Models.show_all_active_inactive_rem_list_Result> inactive { get; set; }
    }
}