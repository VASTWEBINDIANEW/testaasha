using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.WRetailer.ViewModels
{
    public class FlightDetailsVM
    {
        public bool isDomastic { get; set; }
        public string TraceId { get; set; }
        public string ResultIndex { get; set; }
        public FareRuleVM FareRuleVM { get; set; }
        public FareQuoteVM FareQuoteVM { get; set; }
        public SSRNonLcc SSRNonLcc { get; set; }
        public SSRLcc SSRLcc { get; set; }
        public List<Passenger> AdultPax { get; set; }
        public List<Passenger> ChildPax { get; set; }
        public List<Passenger> InfantPax { get; set; }
    }
    public partial class Passenger
    {
        public long? PaxId { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string PaxType { get; set; }
        [Required]
        public string DateOfBirth { get; set; }
        [Required]
        public Gender Gender { get; set; }
        public string PassportNo { get; set; }
        public string PassportExpiry { get; set; }
        [Required]
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public Fare Fare { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string CountryCode { get; set; }
        [Required]
        public string CountryName { get; set; }
        [Required]
        public string Nationality { get; set; }
        [Required]
        public string ContactNo { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public bool IsLeadPax { get; set; } = false;
        public object FfAirlineCode { get; set; }
        public string FfNumber { get; set; }
        public string GstCompanyAddress { get; set; }
        public string GstCompanyContactNumber { get; set; }
        public string GstCompanyName { get; set; }
        public string GstNumber { get; set; }
        public string GstCompanyEmail { get; set; }
        public List<object> SeatDynamic { get; set; }
    }
    
    public enum Gender { Male =1  , Female=2  }
    public partial class BaggageNonLcc
    {

    }
    public partial class BaggageLcc
    {
        public long WayType { get; set; }
        public string Code { get; set; }
        public long Description { get; set; }
        public long? Weight { get; set; }
        public long BaseCurrencyPrice { get; set; }
        public string BaseCurrency { get; set; }
        public string Currency { get; set; }
        public long Price { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string AirlineDescription { get; set; }
        public long? Quantity { get; set; }
    }
    /// <summary>
    /// ///////////////////////////////////  Ssr Json Response class ///////////////////////////
    /// </summary>
    public partial class SSRNonLcc
    {
        public string Version { get; set; }
        public long StatusCode { get; set; }
        public Content Content { get; set; }
    }

    //public partial class Content
    //{
    //    public long ResponseCode { get; set; }
    //    public Addinfo Addinfo { get; set; }
    //}

    public partial class Addinfo
    {
        public List<Meal> Meal { get; set; }
        public List<Meal> SeatPreference { get; set; }
        //public long ResponseStatus { get; set; }
        //public Error Error { get; set; }
        //public Guid TraceId { get; set; }
    }

    //public partial class Error
    //{
    //    public long ErrorCode { get; set; }
    //    public string ErrorMessage { get; set; }
    //}

    public partial class Meal
    {
        public string Code { get; set; }
        public string Description { get; set; }
    }
    ///////////////////////////////////// End Of SSr Non Lcc  ////////////////////////////////

    public partial class SSRLcc
    {
        public string Version { get; set; }
        public long StatusCode { get; set; }
        public ContentSSRLcc Content { get; set; }
    }

    public partial class ContentSSRLcc
    {
        public long ResponseCode { get; set; }
        public Addinfo AddinfoSSRLcc { get; set; }
    }

    public partial class AddinfoSSRLcc
    {
        public long ResponseStatus { get; set; }
        public ErrorSSRLcc Error { get; set; }
        public string TraceId { get; set; }
        public List<List<BaggageSSRLcc>> Baggage { get; set; }
        public List<List<BaggageSSRLcc>> MealDynamic { get; set; }
        public List<SeatDynamic> SeatDynamic { get; set; }
    }

    public partial class BaggageSSRLcc
    {
        public string AirlineCode { get; set; }
        public long FlightNumber { get; set; }
        public long WayType { get; set; }
        public string Code { get; set; }
        public long Description { get; set; }
        public long? Weight { get; set; }
        public string Currency { get; set; }
        public long Price { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string AirlineDescription { get; set; }
        public long? Quantity { get; set; }
    }

    public partial class ErrorSSRLcc
    {
        public long ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }

    public partial class SeatDynamic
    {
        public List<SegmentSeat> SegmentSeat { get; set; }
    }

    public partial class SegmentSeat
    {
        public List<RowSeat> RowSeats { get; set; }
    }

    public partial class RowSeat
    {
        public List<Seat> Seats { get; set; }
    }

    public partial class Seat
    {
        public string AirlineCode { get; set; }
        public long FlightNumber { get; set; }
        public string CraftType { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public long AvailablityType { get; set; }
        public long Description { get; set; }
        public string Code { get; set; }
        public long RowNo { get; set; }
        public string SeatNo { get; set; }
        public long SeatType { get; set; }
        public long SeatWayType { get; set; }
        public long Compartment { get; set; }
        public long Deck { get; set; }
        public string Currency { get; set; }
        public long Price { get; set; }
    }

   

    

    

    

    

    


}