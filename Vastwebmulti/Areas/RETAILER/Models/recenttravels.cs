using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.RETAILER.Models
{
    public class recenttravels
    {
        public IEnumerable<Vastwebmulti.Models.recent_flight_report_Result> recentflight { get; set; }
        public IEnumerable<Vastwebmulti.Models.recent_hotel_report_Result> recentHotel { get; set; }
        public IEnumerable<Vastwebmulti.Models.recent_Bus_report_Result> recentBus { get; set; }
        public IEnumerable<Vastwebmulti.Models.Rem_IRCTC_Txninfo> Rem_IRCTC_Txninfolist { get; set; }
        public IEnumerable<Vastwebmulti.Models.Rem_IRCTC_TokenPurchase_reports_Result> IRCTC_reports { get; set; }
    }
}