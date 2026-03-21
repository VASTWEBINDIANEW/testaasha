using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vastwebmulti.Areas.RETAILER.ViewModels;

namespace Vastwebmulti.Areas.RETAILER.Models
{
    public class Buspdfmodel
    {
        public BusDetailsResponseModel BookingDetails { get; set; }
        public string AdminCompanyName { get; set; }
        public string adminMobile { get; set; }
        public string adminPin { get; set; }
        public string adminaddress { get; set; }
        public string adminemail { get; set; }
        //retailer info
        public string retailerName { get; set; }
        public string retailerMobile { get; set; }
        public string retailerEmail { get; set; }
        public string retailerAddress { get; set; }
        public decimal Markup { get; set; }
    }
    public class BusDetailsResponseModel
    {
        public string Version { get; set; }
        public long StatusCode { get; set; }
        public Contentpdf Content { get; set; }
    }
    public partial class Contentpdf
    {
        public long ResponseCode { get; set; }
        public Addinfopdf Addinfo { get; set; }
    }

    public partial class Addinfopdf
    {
        public Error Error { get; set; }
        public Itinerarypdf Itinerary { get; set; }
        public long ResponseStatus { get; set; }
        public string TraceId { get; set; }
    }

   

    public partial class Itinerarypdf
    {
        public bool IsDomestic { get; set; }
        public string TicketNo { get; set; }
        public DateTime ArrivalTime { get; set; }
        public string BlockDuration { get; set; }
        public string BookingMode { get; set; }
        public string BusId { get; set; }
        public string BusType { get; set; }
        public DateTime DateOfJourney { get; set; }
        public DateTime DepartureTime { get; set; }
        public string Destination { get; set; }
        public string DestinationId { get; set; }
        public string NoOfSeats { get; set; }
        public string Origin { get; set; }
        public List<Passengerpdf> Passenger { get; set; }
        public string RouteId { get; set; }
        public string ServiceName { get; set; }
        public string SourceId { get; set; }
        public string Status { get; set; }
        public string TravelName { get; set; }
        public string TravelOperatorPnr { get; set; }
        public BoardingPointdetails BoardingPointdetails { get; set; }
        public List<CancelPolicy> CancelPolicy { get; set; }
        public Price Price { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime InvoiceCreatedOn { get; set; }
        public string InvoiceCreatedBy { get; set; }
        public decimal InvoiceAmount { get; set; }
        public string InvoiceCreatedByName { get; set; }
        public string InvoiceLastModifiedBy { get; set; }
        public string InvoiceLastModifiedByName { get; set; }
        public string InvoiceStatus { get; set; }
        public List<BookingHistory> BookingHistory { get; set; }
    }

    public partial class BoardingPointdetails
    {
        public string CityPointAddress { get; set; }
        public string CityPointContactNumber { get; set; }
        public string CityPointLandmark { get; set; }
        public string CityPointLocation { get; set; }
        public string CityPointName { get; set; }
        public DateTime CityPointTime { get; set; }
    }

    public partial class BookingHistory
    {
        public string CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public DateTime CreatedOn { get; set; }
        public string EventCategory { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedByName { get; set; }
        public DateTime LastModifiedOn { get; set; }
        public string Remarks { get; set; }
    }

    public partial class CancelPolicy
    {
        public decimal CancellationCharge { get; set; }
        public decimal CancellationChargeType { get; set; }
        public string PolicyString { get; set; }
        public string TimeBeforeDept { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }

    public partial class Passengerpdf
    {
        public bool LeadPassenger { get; set; }
        public string Title { get; set; }
        public string Address { get; set; }
        public string Age { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string Gender { get; set; }
        public object IdNumber { get; set; }
        public object IdType { get; set; }
        public string LastName { get; set; }
        public string Phoneno { get; set; }
        public Seatpdf Seat { get; set; }
    }
    public partial class Seatpdf
    {
        public bool IsLadiesSeat { get; set; }
        public bool IsMalesSeat { get; set; }
        public bool IsUpper { get; set; }
        public decimal SeatFare { get; set; }
        public string SeatId { get; set; }
        public string SeatName { get; set; }
        public bool SeatStatus { get; set; }
        public string SeatType { get; set; }
        public Pricepdf Price { get; set; }
    }
    public partial class Pricepdf
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
        public decimal Tds { get; set; }
        public object Gst { get; set; }
    }


}