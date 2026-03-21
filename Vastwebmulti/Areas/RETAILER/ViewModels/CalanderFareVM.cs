using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.RETAILER.ViewModels
{
    public class CalanderFareVM
    {
        public string Version { get; set; }
        public long StatusCode { get; set; }
        public CalanderFareContent Content { get; set; }
    }

    public partial class CalanderFareContent
    {
        public long ResponseCode { get; set; }
        public CalanderFareAddinfo Addinfo { get; set; }
    }

    public partial class CalanderFareAddinfo
    {
        public long ResponseStatus { get; set; }
        public Error Error { get; set; }
        public Guid TraceId { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public List<CalanderFareSearchResult> SearchResults { get; set; }
    }

    public partial class CalanderFareSearchResult
    {
        public string AirlineCode { get; set; }
        public string AirlineName { get; set; }
        public DateTimeOffset DepartureDate { get; set; }
        public bool IsLowestFareOfMonth { get; set; }
        public double Fare { get; set; }
        public double BaseFare { get; set; }
        public double Tax { get; set; }
        public long OtherCharges { get; set; }
        public long FuelSurcharge { get; set; }
    }

}