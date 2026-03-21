using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vastwebmulti.Models;
namespace Vastwebmulti.Areas.API.Models
{
    public class multi
    {
        public IEnumerable<Vastwebmulti.Models.Select_admin_report_history_Result> rechargereport { get; set; }
        public IEnumerable<Vastwebmulti.Models.Select_admin_report_history_old_Result> recharge_reportold { get; set; }
        public IEnumerable<Vastwebmulti.Models.Message_top> messagetop { get; set; }
    }
}