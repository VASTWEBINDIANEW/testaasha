using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vastwebmulti.Areas.RETAILER.ViewModels;

namespace Vastwebmulti.Areas.RETAILER.Models
{
    public partial class TicketNonLcc
    {
        public string  PNR { get; set; }
        public string TraceId { get; set; }
        public string BookingId { get; set; }
        public decimal OfferedFare { get; set; }
        public decimal PublishedFare { get; set; }
        public string ResultIndex { get; set; }

    }
    public partial class TicketLcc
    {
        public decimal OfferedFare { get; set; }
        public object PreferredCurrency { get; set; }
        public string ResultIndex { get; set; }
        //public string AgentReferenceNo { get; set; }
        public List<Passenger> Passengers { get; set; }
        public string TraceId { get; set; }

    }
    //public partial class Passenger
    //{
        //public string Title { get; set; }
        //public string FirstName { get; set; }
        //public string LastName { get; set; }
        //public long PaxType { get; set; }
        //public DateTime DateOfBirth { get; set; }
        //public long Gender { get; set; }
        //public string PassportNo { get; set; }
        //public DateTime PassportExpiry { get; set; }
        //public string AddressLine1 { get; set; }
        //public string AddressLine2 { get; set; }
        //public Fare Fare { get; set; }
        //public string City { get; set; }
        //public string CountryCode { get; set; }
        //public string CountryName { get; set; }
        //public string ContactNo { get; set; }
        //public string Email { get; set; }
        //public bool IsLeadPax { get; set; }
        //public string FfAirlineCode { get; set; }
        //public long FfNumber { get; set; }
       // public List<Baggage> Baggage { get; set; }
       //public List<Baggage> MealDynamic { get; set; }
       //public List<object> SeatDynamic { get; set; }
       //public string GstCompanyAddress { get; set; }
       //public string GstCompanyContactNumber { get; set; }
       //public string GstCompanyName { get; set; }
       // public string GstNumber { get; set; }
       // public string GstCompanyEmail { get; set; }
   // }
    public partial class Baggage
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
}