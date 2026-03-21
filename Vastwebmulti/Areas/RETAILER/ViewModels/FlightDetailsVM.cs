using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.RETAILER.ViewModels
{
    public class FlightDetailsVM
    {
        public bool isDomastic { get; set; }
        public string TraceId { get; set; }
        public int JourneyType { get; set; }
        //public string ResultIndex { get; set; }
        public List<Passenger> AdultPax { get; set; }
        public List<Passenger> ChildPax { get; set; }
        public List<Passenger> InfantPax { get; set; }
        public FlightOnwardDetail Onward { get; set; }
        public FlightInwardDetail Inward { get; set; }
        public string InBoundResultIndexForSpacailReturn { get; set; }

    }
    public class FlightOnwardDetail
    {
        public string ResultIndex { get; set; }
        public string MealCode { get; set; }
        public string BaggageCode { get; set; }
        public FareRuleVM FareRuleVM { get; set; }
        public FareQuoteVM FareQuoteVM { get; set; }
        public SSRNonLcc SSRNonLcc { get; set; }
        public SSRLcc SSRLcc { get; set; }

    }
    public class FlightInwardDetail
    {
        public string ResultIndex { get; set; }
        public FareRuleVM FareRuleVM { get; set; }
        public FareQuoteVM FareQuoteVM { get; set; }
        public SSRNonLcc SSRNonLcc { get; set; }
        public SSRLcc SSRLcc { get; set; }
    }
    public partial class Passenger
    {
        public long? PaxId { get; set; }
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }
        [Required(ErrorMessage = "The First Name is required.")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "The Last Name is required.")]
        public string LastName { get; set; }
        [Required]
        public string PaxType { get; set; }
        [Required(ErrorMessage = "The Date Of Birth is required.")]
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
        [Required(ErrorMessage = "The Contact No is required.")]
        public string ContactNo { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public bool IsLeadPax { get; set; } = false;
        public object FfAirlineCode { get; set; }
        public string FfNumber { get; set; }
        public string GSTCompanyAddress { get; set; }
        public string GSTCompanyContactNumber { get; set; }
        public string GSTCompanyName { get; set; }
        public string GSTNumber { get; set; }
        public string GSTCompanyEmail { get; set; }
        public List<object> SeatDynamic { get; set; }
        public List<BaggageSSRLcc> Baggage { get; set; } //used is LCC Airline
        public List<BaggageSSRLcc> MealDynamic { get; set; }//used is LCC Airline
        public Ticket Ticket { get; set; }
    }
    public class PassengerNonLcc
    {
        public PassengerNonLcc(Passenger passanger)
        {
            PaxId = passanger.PaxId;
            Title = passanger.Title;
            FirstName = passanger.FirstName;
            LastName = passanger.LastName;
            PaxType = passanger.PaxType;
            DateOfBirth = passanger.DateOfBirth;
            Gender = passanger.Gender;
            PassportNo = passanger.PassportNo;
            PassportExpiry = passanger.PassportExpiry;
            AddressLine1 = passanger.AddressLine1;
            AddressLine2 = passanger.AddressLine2;
            Fare = passanger.Fare;
            City = passanger.City;
            CountryCode = passanger.CountryCode;
            CountryName = passanger.CountryName;
            Nationality = passanger.Nationality;
            ContactNo = passanger.ContactNo;
            Email = passanger.Email;
            IsLeadPax = passanger.IsLeadPax;
            FfAirlineCode = passanger.FfAirlineCode;
            GSTCompanyAddress = passanger.GSTCompanyAddress;
            GSTCompanyContactNumber = passanger.GSTCompanyContactNumber;
            GSTCompanyEmail = passanger.GSTCompanyEmail;
            GSTCompanyName = passanger.GSTCompanyName;
            GSTNumber = passanger.GSTNumber;
            Baggage = passanger.Baggage;
            Ticket = passanger.Ticket;
        }
        public long? PaxId { get; set; }
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }
        [Required(ErrorMessage = "FirstName is required")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "LastName is required")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "PaxType is required")]
        public string PaxType { get; set; }
        [Required(ErrorMessage = "DateOfBirth is required")]
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
        public string GSTCompanyAddress { get; set; }
        public string GSTCompanyContactNumber { get; set; }
        public string GSTCompanyName { get; set; }
        public string GSTNumber { get; set; }
        public string GSTCompanyEmail { get; set; }
        public List<BaggageSSRLcc> Baggage { get; set; } //used is LCC Airline
        public Meal Meal { get; set; }//used is NON-LCC Airline
        public SeatPreference Seat { get; set; }//used is NON-LCC Airline
        public Ticket Ticket { get; set; }
    }

    public enum Gender { Male = 1, Female = 2 }
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
        public AddinfoSSRLcc Addinfo { get; set; }
    }

    public partial class AddinfoSSRLcc
    {
        public long ResponseStatus { get; set; }
        public ErrorSSRLcc Error { get; set; }
        public string TraceId { get; set; }
        public List<List<BaggageSSRLcc>> Baggage { get; set; }
        public List<List<BaggageSSRLcc>> MealDynamic { get; set; }
        public List<SeatDynamic> SeatDynamic { get; set; }
        public List<SpecialService> SpecialServices { get; set; }
    }

    public partial class BaggageSSRLcc
    {
        public string AirlineCode { get; set; }
        public string FlightNumber { get; set; }
        public long WayType { get; set; }
        public string Code { get; set; }
        public long Description { get; set; }
        public long? Weight { get; set; }
        public string Currency { get; set; }
        public decimal Price { get; set; }
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
        public string FlightNumber { get; set; }
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
    public partial class SpecialService
    {
        public List<SegmentSpecialService> SegmentSpecialService { get; set; }
    }
    public partial class SegmentSpecialService
    {
        public List<SsrService> SsrService { get; set; }
    }
    public partial class SsrService
    {
        public string Origin { get; set; }
        public string Destination { get; set; }
        public DateTime DepartureTime { get; set; }
        public string AirlineCode { get; set; }
        public string FlightNumber { get; set; }
        public string Code { get; set; }
        public long ServiceType { get; set; }
        public string Text { get; set; }
        public long WayType { get; set; }
        public string Currency { get; set; }
        public decimal Price { get; set; }
    }
    public partial class Ticket
    {
        public long TicketId { get; set; }
        public string TicketNumber { get; set; }
        public DateTime IssueDate { get; set; }
        public string ValidatingAirline { get; set; }
        public string Remarks { get; set; }
        public string ServiceFeeDisplayType { get; set; }
        public string Status { get; set; }
        public string ConjunctionNumber { get; set; }
        public string TicketType { get; set; }
    }
    public partial class SeatPreference
    {
        public string Code { get; set; }
        public string Description { get; set; }
    }
}