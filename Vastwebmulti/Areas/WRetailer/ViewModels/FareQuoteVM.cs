using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.WRetailer.ViewModels
{
    public class FareQuoteVM
    {
        public string Version { get; set; }
        public long StatusCode { get; set; }
        public FareQuoteContent Content { get; set; }
    }
    public partial class FareQuoteContent
    {
        public long ResponseCode { get; set; }
        public FareQuoteAddinfo Addinfo { get; set; }
    }

    public partial class FareQuoteAddinfo
    {
        public Error Error { get; set; }
        public bool IsPriceChanged { get; set; }
        public int ResponseStatus { get; set; }
        public Results Results { get; set; }
        public string TraceId { get; set; }
    }

    //public partial class Error
    //{
    //    public long ErrorCode { get; set; }
    //    public string ErrorMessage { get; set; }
    //}

    public partial class Results
    {
        public string ResultIndex { get; set; }
        public long Source { get; set; }
        public bool IsLcc { get; set; }
        public bool IsRefundable { get; set; }
        public bool GstAllowed { get; set; }
        public bool IsGstMandatory { get; set; }
        public bool IsHoldAllowed { get; set; }
        public string AirlineRemark { get; set; }
        public Fare Fare { get; set; }
        public List<FareBreakdown> FareBreakdown { get; set; }
        public List<List<Segment>> Segments { get; set; }
        public object LastTicketDate { get; set; }
        public object TicketAdvisory { get; set; }
        public List<FareRule> FareRules { get; set; }
        public string AirlineCode { get; set; }
        public string ValidatingAirline { get; set; }
    }

    //public partial class Fare
    //{
    //    public string Currency { get; set; }
    //    public long BaseFare { get; set; }
    //    public double Tax { get; set; }
    //    public List<ChargeBu> TaxBreakup { get; set; }
    //    public long YqTax { get; set; }
    //    public long AdditionalTxnFeeOfrd { get; set; }
    //    public long AdditionalTxnFeePub { get; set; }
    //    public long PgCharge { get; set; }
    //    public long OtherCharges { get; set; }
    //    public List<ChargeBu> ChargeBu { get; set; }
    //    public long Discount { get; set; }
    //    public double PublishedFare { get; set; }
    //    public double CommissionEarned { get; set; }
    //    public double PlbEarned { get; set; }
    //    public double IncentiveEarned { get; set; }
    //    public double OfferedFare { get; set; }
    //    public double TdsOnCommission { get; set; }
    //    public double TdsOnPlb { get; set; }
    //    public double TdsOnIncentive { get; set; }
    //    public long ServiceFee { get; set; }
    //    public long TotalBaggageCharges { get; set; }
    //    public long TotalMealCharges { get; set; }
    //    public long TotalSeatCharges { get; set; }
    //    public long TotalSpecialServiceCharges { get; set; }
    //}

    //public partial class ChargeBu
    //{
    //    public string Key { get; set; }
    //    public double Value { get; set; }
    //}

    //public partial class FareBreakdown
    //{
    //    public string Currency { get; set; }
    //    public long PassengerType { get; set; }
    //    public long PassengerCount { get; set; }
    //    public long BaseFare { get; set; }
    //    public double Tax { get; set; }
    //    public long YqTax { get; set; }
    //    public long AdditionalTxnFeeOfrd { get; set; }
    //    public long AdditionalTxnFeePub { get; set; }
    //    public long PgCharge { get; set; }
    //}

    //public partial class FareRule
    //{
    //    public string Origin { get; set; }
    //    public string Destination { get; set; }
    //    public string Airline { get; set; }
    //    public string FareBasisCode { get; set; }
    //    public string FareRuleDetail { get; set; }
    //    public string FareRestriction { get; set; }
    //}

    //public partial class Segment
    //{
    //    public string Baggage { get; set; }
    //    public string CabinBaggage { get; set; }
    //    public long TripIndicator { get; set; }
    //    public long SegmentIndicator { get; set; }
    //    public Airline Airline { get; set; }
    //    public Origin Origin { get; set; }
    //    public Destination Destination { get; set; }
    //    public long Duration { get; set; }
    //    public long GroundTime { get; set; }
    //    public long Mile { get; set; }
    //    public bool StopOver { get; set; }
    //    public string StopPoint { get; set; }
    //    public DateTimeOffset StopPointArrivalTime { get; set; }
    //    public DateTimeOffset StopPointDepartureTime { get; set; }
    //    public long Craft { get; set; }
    //    public object Remark { get; set; }
    //    public bool IsETicketEligible { get; set; }
    //    public string FlightStatus { get; set; }
    //    public string Status { get; set; }
    //}

    //public partial class Airline
    //{
    //    public string AirlineCode { get; set; }
    //    public string AirlineName { get; set; }
    //    public long FlightNumber { get; set; }
    //    public string FareClass { get; set; }
    //    public string OperatingCarrier { get; set; }
    //}

    public partial class Destination
    {
        public Airport Airport { get; set; }
        public DateTimeOffset ArrTime { get; set; }
    }

    //public partial class Airport
    //{
    //    public string AirportCode { get; set; }
    //    public string AirportName { get; set; }
    //    public long Terminal { get; set; }
    //    public string CityCode { get; set; }
    //    public string CityName { get; set; }
    //    public string CountryCode { get; set; }
    //    public string CountryName { get; set; }
    //}

    public partial class Origin
    {
        public Airport Airport { get; set; }
        public DateTimeOffset DepTime { get; set; }
    }


}