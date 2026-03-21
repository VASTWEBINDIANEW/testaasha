using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.RETAILER.ViewModels
{
    public partial class HotelSearchResultVm
    {
        public string Version { get; set; }
        public long StatusCode { get; set; }
        public ContentHotelSearchResult Content { get; set; }
    }

    public partial class ContentHotelSearchResult
    {
        public int ResponseCode { get; set; }
        public HotelSearchResultAddinfo Addinfo { get; set; }
    }

    public partial class HotelSearchResultAddinfo
    {
        public int ResponseStatus { get; set; }
        public Error Error { get; set; }
        public string TraceId { get; set; }
        public long CityId { get; set; }
        public string Remarks { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public string PreferredCurrency { get; set; }
        public int NoOfRooms { get; set; }
        public List<RoomGuest> RoomGuests { get; set; }
        public List<HotelResult> HotelResults { get; set; }
    }

    //public  class Error
    //{
    //    public long ErrorCode { get; set; }
    //    public string ErrorMessage { get; set; }
    //}

    public partial class HotelResult
    {
        public long ResultIndex { get; set; }
        public string HotelCode { get; set; }
        public string HotelName { get; set; }
        public string HotelCategory { get; set; }
        public int StarRating { get; set; }
        public string HotelDescription { get; set; }
        public string HotelPromotion { get; set; }
        public string HotelPolicy { get; set; }
        public Price Price { get; set; }
        public string HotelPicture { get; set; }
        public string HotelAddress { get; set; }
        public string HotelContactNo { get; set; }
        public object HotelMap { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string HotelLocation { get; set; }
        public object SupplierPrice { get; set; }
        public List<object> RoomDetails { get; set; }
    }

    public partial class Price
    {
        public string CurrencyCode { get; set; }
        public double RoomPrice { get; set; }
        public long Tax { get; set; }
        public long ExtraGuestCharge { get; set; }
        public long ChildCharge { get; set; }
        public double OtherCharges { get; set; }
        public long Discount { get; set; }
        public double PublishedPrice { get; set; }
        public long PublishedPriceRoundedOff { get; set; }
        public double OfferedPrice { get; set; }
        public long OfferedPriceRoundedOff { get; set; }
        public long AgentCommission { get; set; }
        public long AgentMarkUp { get; set; }
        public double ServiceTax { get; set; }
        public long Tds { get; set; }
        public long ServiceCharge { get; set; }
        public double TotalGstAmount { get; set; }
        public GST Gst { get; set; }
    }

    public partial class GST
    {
        public long CGSTAmount { get; set; }
        public long CGSTRate { get; set; }
        public long CessAmount { get; set; }
        public long CessRate { get; set; }
        public double IGSTAmount { get; set; }
        public long IGSTRate { get; set; }
        public long SGSTAmount { get; set; }
        public long SGSTRate { get; set; }
        public double TaxableAmount { get; set; }
    }

    public partial class RoomGuest
    {
        public long NoOfAdults { get; set; }
        public long NoOfChild { get; set; }
        public object ChildAge { get; set; }
    }

}