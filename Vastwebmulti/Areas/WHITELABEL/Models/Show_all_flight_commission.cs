using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.WHITELABEL.Models
{
    public class Show_all_flight_commission
    {
        public List<Flight1> Flight1 { set; get; }
    }
    public class Flight1
    {
        public int idno { get; set; }
        public decimal? WRetailerCommission { set; get; }
        public decimal? WDealerCommission { get; set; }
        public string WhitelabelId { get; set; }
        public decimal? WhitelabelCommission { set; get; }
    }
}