using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.RETAILER.ViewModels
{
    public partial class HotelBookingDetailsResponseModel
    {
        public string Version { get; set; }
        public long StatusCode { get; set; }
        public BookingDetailsContent Content { get; set; }
    }

    public partial class BookingDetailsContent
    {
        public long ResponseCode { get; set; }
        public BookingDetailsAddinfo Addinfo { get; set; }
    }

    public partial class BookingDetailsAddinfo
    {
        public bool VoucherStatus { get; set; }
        public long ResponseStatus { get; set; }
        public Error Error { get; set; }
        public string TraceId { get; set; }
        public string Status { get; set; }
        public string HotelBookingStatus { get; set; }
        public string ConfirmationNo { get; set; }
        public long BookingRefNo { get; set; }
        public string BookingId { get; set; }
        public bool IsPriceChanged { get; set; }
        public bool IsCancellationPolicyChanged { get; set; }
        public List<BookingDetailsHotelRoomsDetail> HotelRoomsDetails { get; set; }
        public string HotelPolicyDetail { get; set; }
        public object IntHotelPassportDetails { get; set; }
        public object InvoiceCreatedOn { get; set; }
        public object InvoiceNo { get; set; }
        public object HotelConfirmationNo { get; set; }
        public string HotelName { get; set; }
        public long StarRating { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string CountryCode { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string City { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime InitialCheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public DateTime InitialCheckOutDate { get; set; }
        public DateTime LastCancellationDate { get; set; }
        public long NoOfRooms { get; set; }
        public DateTime BookingDate { get; set; }
        public string SpecialRequest { get; set; }
        public bool IsDomestic { get; set; }
        public bool BookingAllowedForRoamer { get; set; }
    }

    //public partial class Error
    //{
    //    public long ErrorCode { get; set; }
    //    public string ErrorMessage { get; set; }
    //}

    public partial class BookingDetailsHotelRoomsDetail
    {
        public long AdultCount { get; set; }
        public long ChildCount { get; set; }
        public List<BookingDetailsHotelPassenger> HotelPassenger { get; set; }
        public bool RequireAllPaxDetails { get; set; }
        public long RoomId { get; set; }
        public long RoomStatus { get; set; }
        public long RoomIndex { get; set; }
        public string RoomTypeCode { get; set; }
        public string RoomTypeName { get; set; }
        public string RatePlanCode { get; set; }
        public long RatePlan { get; set; }
        public List<DayRate> DayRates { get; set; }
        public object SupplierPrice { get; set; }
        public Price Price { get; set; }
        public string RoomPromotion { get; set; }
        public List<string> Amenities { get; set; }
        public List<object> Amenity { get; set; }
        public string SmokingPreference { get; set; }
        public List<object> BedTypes { get; set; }
        public object HotelSupplements { get; set; }
        public DateTime LastCancellationDate { get; set; }
        public List<CancellationPolicy> CancellationPolicies { get; set; }
        public string CancellationPolicy { get; set; }
        public List<string> Inclusion { get; set; }
    }

    //public partial class CancellationPolicy
    //{
    //    public long Charge { get; set; }
    //    public long ChargeType { get; set; }
    //    public string Currency { get; set; }
    //    public DateTimeOffset FromDate { get; set; }
    //    public DateTimeOffset ToDate { get; set; }
    //    public bool? NoShowPolicy { get; set; }
    //}

    //public partial class DayRate
    //{
    //    public double Amount { get; set; }
    //    public DateTimeOffset Date { get; set; }
    //}

    public partial class BookingDetailsHotelPassenger
    {
        public long Age { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public object GstCompanyAddress { get; set; }
        public object GstCompanyContactNumber { get; set; }
        public object GstCompanyEmail { get; set; }
        public object GstCompanyName { get; set; }
        public object GstNumber { get; set; }
        public string LastName { get; set; }
        public bool LeadPassenger { get; set; }
        public string MiddleName { get; set; }
        public object Pan { get; set; }
        public object PassportExpDate { get; set; }
        public object PassportIssueDate { get; set; }
        public object PassportNo { get; set; }
        public long PaxId { get; set; }
        public long PaxType { get; set; }
        public string Phoneno { get; set; }
        public string Title { get; set; }
    }

    //public partial class Price
    //{
    //    public string CurrencyCode { get; set; }
    //    public double RoomPrice { get; set; }
    //    public long Tax { get; set; }
    //    public long ExtraGuestCharge { get; set; }
    //    public long ChildCharge { get; set; }
    //    public double OtherCharges { get; set; }
    //    public long Discount { get; set; }
    //    public double PublishedPrice { get; set; }
    //    public long PublishedPriceRoundedOff { get; set; }
    //    public double OfferedPrice { get; set; }
    //    public long OfferedPriceRoundedOff { get; set; }
    //    public long AgentCommission { get; set; }
    //    public long AgentMarkUp { get; set; }
    //    public double ServiceTax { get; set; }
    //    public long Tds { get; set; }
    //    public long ServiceCharge { get; set; }
    //    public double TotalGstAmount { get; set; }
    //    public Gst Gst { get; set; }
    //}

    //public partial class Gst
    //{
    //    public long CgstAmount { get; set; }
    //    public long CgstRate { get; set; }
    //    public long CessAmount { get; set; }
    //    public long CessRate { get; set; }
    //    public double IgstAmount { get; set; }
    //    public long IgstRate { get; set; }
    //    public long SgstAmount { get; set; }
    //    public long SgstRate { get; set; }
    //    public double TaxableAmount { get; set; }
    //}

}