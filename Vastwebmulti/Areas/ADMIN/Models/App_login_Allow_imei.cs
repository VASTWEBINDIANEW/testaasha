using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    /// <summary>
    /// Model for managing allowed IMEI numbers for app-based login access control.
    /// </summary>
    public class App_login_Allow_imei
    {
        public int idno { get; set; }
        public string firmname { get; set; }
        public string email { get; set; }
        public string userid { get; set; }
        public string role { get; set; }
        public string imeino { get; set; }
    }
}