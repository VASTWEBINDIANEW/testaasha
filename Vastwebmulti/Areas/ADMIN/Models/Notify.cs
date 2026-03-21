using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    /// <summary>
    /// Model for sending system notifications or alerts to users.
    /// </summary>
    public class Notify
    {
      public  IEnumerable<Vastwebmulti.Models.Message_top> messagetop { get; set; }
      public  IEnumerable<Vastwebmulti.Models.tbl_Holiday> Holiday { get; set; }
      public  IEnumerable<Vastwebmulti.Models.tblAppslider> appsliders { get; set; }
        public string appslidestatus { get; set; }
        public IEnumerable<Vastwebmulti.Models.customersupports_times> customertimelist { get; set; }

    }
    public class Whitelabel_Notify
    {
        public IEnumerable<Vastwebmulti.Models.Whitelabel_Message_top> Message_top { get; set; }
        public IEnumerable<Vastwebmulti.Models.Whitelabel_tbl_Holiday> tbl_Holiday { get; set; }
    }
}