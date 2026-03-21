using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.RETAILER.ViewModels
{
    public class BusSearchResultVM
    {
        public string Version { get; set; }
        public long StatusCode { get; set; }
        public SearchResultContent Content { get; set; }
        public string doj { get; set; }

    }
    public partial class SearchResultContent
    {
        public long ResponseCode { get; set; }
        public SearchResultAddinfo Addinfo { get; set; }
    }
    public partial class SearchResultAddinfo
    {
        public long ResponseStatus { get; set; }
        public Error Error { get; set; }
        public string Destination { get; set; }
        public string Origin { get; set; }
        public string TraceId { get; set; }
        public List<BusResult> BusResults { get; set; }
    }
    public partial class BusResult
    {
        public string ResultIndex { get; set; }
        public DateTime ArrivalTime { get; set; }
        public long AvailableSeats { get; set; }
        public DateTime DepartureTime { get; set; }
        public string RouteId { get; set; }
        public string BusType { get; set; }
        public string ServiceName { get; set; }
        public string TravelName { get; set; }
        public bool IdProofRequired { get; set; }
        public bool IsDropPointMandatory { get; set; }
        public bool LiveTrackingAvailable { get; set; }
        public bool MTicketEnabled { get; set; }
        public long MaxSeatsPerTicket { get; set; }
        public long OperatorId { get; set; }
        public bool PartialCancellationAllowed { get; set; }
        public List<PickAndDropPointsDetail> BoardingPointsDetails { get; set; }
        public List<PickAndDropPointsDetail> DroppingPointsDetails { get; set; }
        public BusPrice BusPrice { get; set; }
        public List<BusCancellationPolicy> CancellationPolicies { get; set; }
    }
    public partial class PickAndDropPointsDetail
    {
        public long CityPointIndex { get; set; }
        public string CityPointLocation { get; set; }
        public string CityPointName { get; set; }
        public DateTime CityPointTime { get; set; }
    }
    public partial class BusPrice
    {
        public string CurrencyCode { get; set; }
        public decimal BasePrice { get; set; }
        public decimal Tax { get; set; }
        public decimal OtherCharges { get; set; }
        public decimal Discount { get; set; }
        public decimal PublishedPrice { get; set; }
        public decimal PublishedPriceRoundedOff { get; set; }
        public decimal OfferedPrice { get; set; }
        public decimal OfferedPriceRoundedOff { get; set; }
        public decimal AgentCommission { get; set; }
        public decimal AgentMarkUp { get; set; }
        public decimal TDS { get; set; }
        public Gst GST { get; set; }
    }
    //public partial class Gst
    //{
    //    public long CgstAmount { get; set; }
    //    public long CgstRate { get; set; }
    //    public long CessAmount { get; set; }
    //    public long CessRate { get; set; }
    //    public long IgstAmount { get; set; }
    //    public long IgstRate { get; set; }
    //    public long SgstAmount { get; set; }
    //    public long SgstRate { get; set; }
    //    public long TaxableAmount { get; set; }
    //}

    public partial class BusCancellationPolicy
    {
        public long? CancellationCharge { get; set; }
        public long CancellationChargeType { get; set; }
        public string PolicyString { get; set; }
        public string TimeBeforeDept { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }

}