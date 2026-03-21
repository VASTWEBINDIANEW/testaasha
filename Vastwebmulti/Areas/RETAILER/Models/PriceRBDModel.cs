using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.RETAILER.Models
{
    public class PriceRBDModel
    {
        [Required]
        public int AdultCount { get; set; }
        [Required]
        public int ChildCount { get; set; }
        [Required]
        public int InfantCount { get; set; }
        //public string EndUserIp { get; set; }
        //public string TokenId { get; set; }
        [Required]
        public string TraceId { get; set; }
        public List<AirSearchResult> AirSearchResult { get; set; }
    }

    public partial class AirSearchResult
    {
        [Required]
        public string ResultIndex { get; set; }
        public long Source { get; set; }
        [Required]
        public bool IsLcc { get; set; }
        public bool IsRefundable { get; set; }
        public string AirlineRemark { get; set; }
        [Required]
        public List<List<PriceRBDSegment>> Segments { get; set; }
    }

    public partial class PriceRBDSegment
    {
        public long TripIndicator { get; set; }
        public long SegmentIndicator { get; set; }
        public Airline Airline { get; set; }
    }

    //public partial class Airline
    //{
    //    public string AirlineCode { get; set; }
    //    public string AirlineName { get; set; }
    //    public long FlightNumber { get; set; }
    //    public string FareClass { get; set; }
    //    public string OperatingCarrier { get; set; }
    //}
}