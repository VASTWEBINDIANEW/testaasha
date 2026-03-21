using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.RETAILER.Models
{
    public class Activeandinactivepaid
    {
        public IEnumerable<Vastwebmulti.Models.PaidServicesPaymentHistory> History { get; set; }
        public IEnumerable<Vastwebmulti.Models.proc_Activeandinactivepaidservice_Result> ActiveandInactive { get; set; }
    }
}