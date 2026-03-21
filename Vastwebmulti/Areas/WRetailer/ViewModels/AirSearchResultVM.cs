using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.WRetailer.ViewModels
{
    public partial class AirSearchResultVM
    {
        public string Version { get; set; }
        public int StatusCode { get; set; }
        public Content Content { get; set; }
    }

    public partial class Content
    {
        public int ResponseCode { get; set; }
        public Addinfo Addinfo { get; set; }
    }

    public partial class Addinfo
    {
        public int ResponseStatus { get; set; }
        public Error Error { get; set; }
        public string TraceId { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        //public Result[][] Results { get; set; }
        public List<List<Result>>  Results { get; set; }
    }

    public partial class Error
    {
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }

    public partial class Result
    {
        public string ResultIndex { get; set; }
        public int Source { get; set; }
        public bool IsLcc { get; set; }
        public bool IsRefundable { get; set; }
        public bool GstAllowed { get; set; }
        public bool IsGstMandatory { get; set; }
        public string AirlineRemark { get; set; }
        public Fare Fare { get; set; }
        //public FareBreakdown[] FareBreakdown { get; set; }
        public List<FareBreakdown> FareBreakdown { get; set; }
        //public Segment[][] Segments { get; set; }
        public List<List<Segment>> Segments { get; set; }
        public object LastTicketDate { get; set; }
        public object TicketAdvisory { get; set; }
        //public FareRule[] FareRules { get; set; }
        public List<FareRule> FareRules { get; set; }
        public string AirlineCode { get; set; }
        public string ValidatingAirline { get; set; }
    }

    public partial class Fare
    {
        public string Currency { get; set; }
        public decimal BaseFare { get; set; }
        public decimal Tax { get; set; }
        //public ChargeBu[] TaxBreakup { get; set; }
        public List<ChargeBu> TaxBreakup { get; set; }
        public int YqTax { get; set; }
        public int AdditionalTxnFeeOfrd { get; set; }
        public decimal AdditionalTxnFeePub { get; set; }
        public int PgCharge { get; set; }
        public decimal OtherCharges { get; set; }
        //public ChargeBu[] ChargeBu { get; set; }
        public List<ChargeBu> ChargeBu { get; set; }
        public int Discount { get; set; }
        public decimal PublishedFare { get; set; }
        public decimal CommissionEarned { get; set; }
        public decimal PlbEarned { get; set; }
        public decimal IncentiveEarned { get; set; }
        public decimal OfferedFare { get; set; }
        public decimal TdsOnCommission { get; set; }
        public decimal TdsOnPlb { get; set; }
        public decimal TdsOnIncentive { get; set; }
        public int ServiceFee { get; set; }
        public int TotalBaggageCharges { get; set; }
        public int TotalMealCharges { get; set; }
        public int TotalSeatCharges { get; set; }
        public int TotalSpecialServiceCharges { get; set; }
    }

    public partial class ChargeBu
    {
        public string Key { get; set; }
        public decimal Value { get; set; }
    }

    public partial class FareBreakdown
    {
        public string Currency { get; set; }
        public int PassengerType { get; set; }
        public int PassengerCount { get; set; }
        public int BaseFare { get; set; }
        public decimal Tax { get; set; }
        public int YqTax { get; set; }
        public int AdditionalTxnFeeOfrd { get; set; }
        public decimal AdditionalTxnFeePub { get; set; }
        public int PgCharge { get; set; }
    }

    public partial class FareRule
    {
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string Airline { get; set; }
        public string FareBasisCode { get; set; }
        public string FareRuleDetail { get; set; }
        public string FareRestriction { get; set; }
    }

    public partial class Segment
    {
        public string Baggage { get; set; }
        public string CabinBaggage { get; set; }
        public int TripIndicator { get; set; }
        public int SegmentIndicator { get; set; }
        public Airline Airline { get; set; }
        public int NoOfSeatAvailable { get; set; }
        public OriginClass Origin { get; set; }
        public DestinationClass Destination { get; set; }
        public int Duration { get; set; }
        public int GroundTime { get; set; }
        public int Mile { get; set; }
        public bool StopOver { get; set; }
        public string StopPoint { get; set; }
        public DateTimeOffset StopPointArrivalTime { get; set; }
        public DateTimeOffset StopPointDepartureTime { get; set; }
        public string Craft { get; set; }
        public object Remark { get; set; }
        public bool IsETicketEligible { get; set; }
        public string FlightStatus { get; set; }
        public string Status { get; set; }
        public int? AccumulatedDuration { get; set; }
    }

    public partial class Airline
    {
        public string AirlineCode { get; set; }
        public string AirlineName { get; set; }
        public string FlightNumber { get; set; }
        public string FareClass { get; set; }
        public string OperatingCarrier { get; set; }
    }

    public partial class DestinationClass
    {
        public Airport Airport { get; set; }
        public DateTimeOffset ArrTime { get; set; }
    }

    public partial class Airport
    {
        public string AirportCode { get; set; }
        public string AirportName { get; set; }
        public string Terminal { get; set; }
        public string CityCode { get; set; }
        public string CityName { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
    }

    public partial class OriginClass
    {
        public Airport Airport { get; set; }
        public DateTimeOffset DepTime { get; set; }
    }

    //public enum AirlineCode { The6E };

    //public enum Key { Conveniencecharge, InTax, K3, OtherTaxes, Othercharge, Psf, Tbomarkup, TransactionFee, Udf, YqTax, Yr };

   // public enum Currency { Inr };

   // public enum DestinationEnum { Amd, Bdq, Bom, Idr };

   // public enum FareBasisCode { Qcip, Qz20Ap, R020Ap, Rcip };

   // public enum OriginEnum { Amd, Bdq, Del, Idr };

   // public enum AirlineName { Indigo };

   // public enum FareClass { Ho, Q, R };

   // public enum Baggage { The0Kg, The15Kg };

   // public enum CabinBaggage { The7Kg };

   // public enum Code { Amd, Bdq, Bom, Del, Idr };

    //public enum AirportName { Ahmedabad, IndiraGandhiAirport, Indore, Mumbai, Vadodara };
     

    //public enum CityName { Ahmedabad, Delhi, Indore, Mumbai, Vadodara };

    //public enum CountryCode { In };

   // public enum CountryName { India };

    //public enum FlightStatus { Confirmed };

}