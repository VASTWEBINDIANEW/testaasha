using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.RETAILER.Models
{
    public class SearchAirRequestModel
    {
        public string EndUserIp { get { return ""; } }
        //public string TokenId { get; set; }

        public string TokenId { get { return ""; } }

        public int AdultCount { get; set; }
        public int ChildCount { get; set; }
        public int InfantCount { get; set; }
        public bool DirectFlight { get; set; }
        public bool OneStopFlight { get; set; }
        public int JourneyType { get; set; }
        public string[] PreferredAirlines { get; set; }
        public List<Segment> Segments { get; set; }
        public string[] Sources { get; set; }
    }
    public partial class Segment
    {
        public string Origin { get; set; }
        public string Destination { get; set; }
        public int FlightCabinClass { get; set; }
        public DateTime PreferredDepartureTime { get; set; }
        public DateTime PreferredArrivalTime { get; set; }
    }
}