using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    public class Show_Flight_Commission
    {
        public List<Flight> Flight { set; get; }
    }
    public class Flight
    {
        public int idno { get; set; }
        public decimal? RetailerCommission { set; get; }
        public decimal? DealerCommission { get; set; }
        public decimal MasterCommission { get; set; }
        public decimal? WhitelabelCommission { set; get; }
    }
}