using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.RETAILER.ViewModels
{
    public partial class GetFlightDetailsVM
    {
        public string Version { get; set; }
        public int StatusCode { get; set; }
        public FlightDetailsContent Content { get; set; }
       
    }
    public partial class FlightDetailsContent
    {
        public long ResponseCode { get; set; }
        public FlightDetailsAddinfo ADDINFO { get; set; }
    }

    public partial class FlightDetailsAddinfo
    {
        public Error Error { get; set; }
        public long ResponseStatus { get; set; }
        public string TraceId { get; set; }
        public FlightItinerary FlightItinerary { get; set; }
    }

   

    public partial class FlightItinerary
    {
        public string IssuancePcc { get; set; }
        public long TripIndicator { get; set; }
        public long BookingId { get; set; }
        public bool IsManual { get; set; }
        public string Pnr { get; set; }
        public bool IsDomestic { get; set; }
        public long Source { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string AirlineCode { get; set; }
        public DateTime LastTicketDate { get; set; }
        public string ValidatingAirlineCode { get; set; }
        public string AirlineRemark { get; set; }
        public bool IsLcc { get; set; }
        public bool NonRefundable { get; set; }
        public string FareType { get; set; }
        public object CreditNoteNo { get; set; }
        public Fare Fare { get; set; }
        public object CreditNoteCreatedOn { get; set; }
        public List<Passenger> Passenger { get; set; }
        public object CancellationCharges { get; set; }
        public List<Segment> Segments { get; set; }
        public List<FareRule> FareRules { get; set; }
        public long Status { get; set; }
        public long InvoiceAmount { get; set; }
        public string InvoiceNo { get; set; }
        public long InvoiceStatus { get; set; }
        public DateTime InvoiceCreatedOn { get; set; }
        public string Remarks { get; set; }
    }

    //public partial class Fare
    //{
    //    public string Currency { get; set; }
    //    public long BaseFare { get; set; }
    //    public long Tax { get; set; }
    //    public List<ChargeBu> TaxBreakup { get; set; }
    //    public long YqTax { get; set; }
    //    public long AdditionalTxnFeeOfrd { get; set; }
    //    public long AdditionalTxnFeePub { get; set; }
    //    public long PgCharge { get; set; }
    //    public long OtherCharges { get; set; }
    //    public List<ChargeBu> ChargeBu { get; set; }
    //    public long Discount { get; set; }
    //    public long PublishedFare { get; set; }
    //    public double CommissionEarned { get; set; }
    //    public long PlbEarned { get; set; }
    //    public long IncentiveEarned { get; set; }
    //    public double OfferedFare { get; set; }
    //    public double TdsOnCommission { get; set; }
    //    public long TdsOnPlb { get; set; }
    //    public long TdsOnIncentive { get; set; }
    //    public long ServiceFee { get; set; }
    //    public long TotalBaggageCharges { get; set; }
    //    public long TotalMealCharges { get; set; }
    //    public long TotalSeatCharges { get; set; }
    //    public long TotalSpecialServiceCharges { get; set; }
    //}

    //public partial class ChargeBu
    //{
    //    public string Key { get; set; }
    //    public long Value { get; set; }
    //}

    public partial class FareRule
    {
        //public string Origin { get; set; }
        //public string Destination { get; set; }
        //public string Airline { get; set; }
        //public string FareBasisCode { get; set; }
        //public string FareRuleDetail { get; set; }
        //public string FareRestriction { get; set; }
        public string FareFamilyCode { get; set; }
        public string FareRuleIndex { get; set; }
    }

    public partial class Passenger
    {
        //public long PaxId { get; set; }
        //public string Title { get; set; }
        //public string FirstName { get; set; }
        //public string LastName { get; set; }
        //public long PaxType { get; set; }
        //public DateTimeOffset DateOfBirth { get; set; }
        //public long Gender { get; set; }
        //public string PassportNo { get; set; }
        //public string AddressLine1 { get; set; }
        //public string AddressLine2 { get; set; }
        //public Fare Fare { get; set; }
        //public string City { get; set; }
        //public string CountryCode { get; set; }
        //public string Nationality { get; set; }
        //public string ContactNo { get; set; }
        //public string Email { get; set; }
        //public bool IsLeadPax { get; set; }
        //public object FfAirlineCode { get; set; }
        //public object FfNumber { get; set; }
        //public List<Baggage> Baggage { get; set; }
        public List<object> Ssr { get; set; }
        //public Ticket Ticket { get; set; }
        public List<SegmentAdditionalInfo> SegmentAdditionalInfo { get; set; }
    }

    public partial class Baggage
    {
        public string AirlineCode { get; set; }
        public long FlightNumber { get; set; }
        public long WayType { get; set; }
        public string Code { get; set; }
        public long Description { get; set; }
        public long Weight { get; set; }
        public string Currency { get; set; }
        public long Price { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string Text { get; set; }
    }

    public partial class SegmentAdditionalInfo
    {
        public string FareBasis { get; set; }
        public string Nva { get; set; }
        public string Nvb { get; set; }
        public string Baggage { get; set; }
        public string Meal { get; set; }
        public string Seat { get; set; }
        public string SpecialService { get; set; }
    }

    //public partial class Ticket
    //{
    //    public long TicketId { get; set; }
    //    public string TicketNumber { get; set; }
    //    public DateTimeOffset IssueDate { get; set; }
    //    public string ValidatingAirline { get; set; }
    //    public string Remarks { get; set; }
    //    public string ServiceFeeDisplayType { get; set; }
    //    public string Status { get; set; }
    //    public string ConjunctionNumber { get; set; }
    //    public string TicketType { get; set; }
    //}

    public partial class Segment
    {
        //public object Baggage { get; set; }
        //public object CabinBaggage { get; set; }
        //public long TripIndicator { get; set; }
        //public long SegmentIndicator { get; set; }
        //public Airline Airline { get; set; }
        public string AirlinePnr { get; set; }
        //public Origin Origin { get; set; }
        //public Destination Destination { get; set; }
        //public long Duration { get; set; }
        //public long GroundTime { get; set; }
        //public long Mile { get; set; }
        //public bool StopOver { get; set; }
        public string FlightInfoIndex { get; set; }
        //public string StopPoint { get; set; }
        //public object StopPointArrivalTime { get; set; }
        //public object StopPointDepartureTime { get; set; }
        //public long Craft { get; set; }
        //public object Remark { get; set; }
        //public bool IsETicketEligible { get; set; }
        //public string FlightStatus { get; set; }
        //public string Status { get; set; }
    }

    //public partial class Airline
    //{
    //    public string AirlineCode { get; set; }
    //    public string AirlineName { get; set; }
    //    public long FlightNumber { get; set; }
    //    public string FareClass { get; set; }
    //    public string OperatingCarrier { get; set; }
    //}

    //public partial class Destination
    //{
    //    public Airport Airport { get; set; }
    //    public DateTimeOffset ArrTime { get; set; }
    //}

    //public partial class Airport
    //{
    //    public string AirportCode { get; set; }
    //    public string AirportName { get; set; }
    //    public string Terminal { get; set; }
    //    public string CityCode { get; set; }
    //    public string CityName { get; set; }
    //    public string CountryCode { get; set; }
    //    public string CountryName { get; set; }
    //}

    //public partial class Origin
    //{
    //    public Airport Airport { get; set; }
    //    public DateTimeOffset DepTime { get; set; }
    //}

}