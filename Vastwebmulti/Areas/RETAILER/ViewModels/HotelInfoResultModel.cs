using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.RETAILER.ViewModels
{
    public class HotelInfoResultModel
    {
        public string Version { get; set; }
        public long StatusCode { get; set; }
        public HotelInfoContent Content { get; set; }
    }

    public partial class HotelInfoContent
    {
        public long ResponseCode { get; set; }
        public HotelInfoAddinfo Addinfo { get; set; }
    }

    public partial class HotelInfoAddinfo
    {
        public long ResponseStatus { get; set; }
        public Error Error { get; set; }
        public string TraceId { get; set; }
        public HotelDetails HotelDetails { get; set; }
    }

    //public partial class Error
    //{
    //    public long ErrorCode { get; set; }
    //    public string ErrorMessage { get; set; }
    //}

    public partial class HotelDetails
    {
        public string HotelCode { get; set; }
        public string HotelName { get; set; }
        public int StarRating { get; set; }
        public string HotelUrl { get; set; }
        public string Description { get; set; }
        public List<Attraction> Attractions { get; set; }
        public string[] HotelFacilities { get; set; }
        public string HotelPolicy { get; set; }
        public string SpecialInstructions { get; set; }
        public string HotelPicture { get; set; }
        public string[] Images { get; set; }
        public string Address { get; set; }
        public string CountryName { get; set; }
        public string PinCode { get; set; }
        public string HotelContactNo { get; set; }
        public string FaxNumber { get; set; }
        public string Email { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public object RoomData { get; set; }
        public object RoomFacilities { get; set; }
        public object Services { get; set; }
    }

    public partial class Attraction
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

}