using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.RETAILER.ViewModels
{
    public  class TicketChangeOrCancelModel
    {
        public long BookingId { get; set; }
        public long RequestType { get; set; }
        public long CancellationType { get; set; }
        public List<Sector> Sectors { get; set; }
        public List<long> TicketId { get; set; }
        public string Remarks { get; set; }
    }

    public  class Sector
    {
        public string Origin { get; set; }
        public string Destination { get; set; }
    }
}