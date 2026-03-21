using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vastwebmulti.Areas.RETAILER.ViewModels;

namespace Vastwebmulti.Areas.RETAILER.ViewModels
{
    

    public partial class HotelRoomResultModel
    {
        public string Version { get; set; }
        public long StatusCode { get; set; }
        public HotelRoomContent Content { get; set; }
    }

    public partial class HotelRoomContent
    {
        public long ResponseCode { get; set; }
        public HotelRoomAddinfo Addinfo { get; set; }
    }

    public partial class HotelRoomAddinfo
    {
        public long ResponseStatus { get; set; }
        public Error Error { get; set; }
        public string TraceId { get; set; }
        public bool IsUnderCancellationAllowed { get; set; }
        public bool IsPolicyPerStay { get; set; }
        public List<HotelRoomsDetail> HotelRoomsDetails { get; set; }
        public RoomCombinations RoomCombinations { get; set; }
    }

    //public partial class Error
    //{
    //    public long ErrorCode { get; set; }
    //    public string ErrorMessage { get; set; }
    //}

    public partial class HotelRoomsDetail
    {
        public long ChildCount { get; set; }
        public bool RequireAllPaxDetails { get; set; }
        public long RoomId { get; set; }
        public long RoomStatus { get; set; }
        public long RoomIndex { get; set; }
        public string RoomTypeCode { get; set; }
        public string RoomTypeName { get; set; }
        public string RatePlanCode { get; set; }
        public long RatePlan { get; set; }
        public string InfoSource { get; set; }
        public string SequenceNo { get; set; }
        public List<DayRate> DayRates { get; set; }
        public object SupplierPrice { get; set; }
        public HotelRoomPrice Price { get; set; }
        public string RoomPromotion { get; set; }
        public List<string> Amenities { get; set; }
        public List<object> Amenity { get; set; }
        public SmokingPreference SmokingPreference { get; set; }
        public List<object> BedTypes { get; set; }
        public List<object> HotelSupplements { get; set; }
        public DateTime LastCancellationDate { get; set; }
        public List<CancellationPolicy> CancellationPolicies { get; set; }
        public string CancellationPolicy { get; set; }
        public string[] Inclusion { get; set; }
    }

    public partial class CancellationPolicy
    {
        public long Charge { get; set; }
        public long ChargeType { get; set; }
        public string Currency { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public bool? NoShowPolicy { get; set; }
    }

    public partial class DayRate
    {
        public double Amount { get; set; }
        public DateTimeOffset Date { get; set; }
    }

    public partial class HotelRoomPrice
    {
        public string CurrencyCode { get; set; }
        public decimal RoomPrice { get; set; }
        public decimal Tax { get; set; }
        public decimal ExtraGuestCharge { get; set; }
        public decimal ChildCharge { get; set; }
        public decimal OtherCharges { get; set; }
        public decimal Discount { get; set; }
        public decimal PublishedPrice { get; set; }
        public long PublishedPriceRoundedOff { get; set; }
        public decimal OfferedPrice { get; set; }
        public decimal OfferedPriceRoundedOff { get; set; }
        public decimal AgentCommission { get; set; }
        public decimal AgentMarkUp { get; set; }
        public decimal ServiceTax { get; set; }
        public decimal Tds { get; set; }
        public decimal ServiceCharge { get; set; }
        public decimal TotalGstAmount { get; set; }
        public Gst Gst { get; set; }
    }

    public partial class Gst
    {
        public decimal CGSTAmount { get; set; }
        public decimal CGSTRate { get; set; }
        public decimal CessAmount { get; set; }
        public decimal CessRate { get; set; }
        public decimal IGSTAmount { get; set; }
        public decimal IGSTRate { get; set; }
        public decimal SGSTAmount { get; set; }
        public decimal SGSTRate { get; set; }
        public decimal TaxableAmount { get; set; }
    }

    public partial class RoomCombinations
    {
        public string InfoSource { get; set; }
        public bool IsPolicyPerStay { get; set; }
        public List<RoomCombination> RoomCombination { get; set; }
    }

    public partial class RoomCombination
    {
        public int[] RoomIndex { get; set; }
    }
    public class HotelDetailedVM
    {
        public HotelInfoResultModel hotelInfo { get; set; }
        public HotelRoomResultModel roomInfo { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int TotalNights { get; set; }
        public int TotalRooms { get; set; }
        public int StarRating { get; set; }
        public string RIndex { get; set; }
        public string HCode { get; set; }
        public string GuestNationality { get; set; }
    }

}