using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.RETAILER.ViewModels
{
    public  class HotelBlockRoomResponseModel
    {
        public string Version { get; set; }
        public long StatusCode { get; set; }
        public BlockRoomContent Content { get; set; }
        public List<HotelPassenger> HotelAdultPassengers { get; set; }
        public List<HotelPassenger> HotelChildPassengers { get; set; }
        public List<BookHotelRoomsDetail> BookHotelRoomDetails { get; set; }
        public string RTypeCode { get; set; }
        public bool IsDomestic { get; set; }
    }

    public partial class BlockRoomContent
    {
        public long ResponseCode { get; set; }
        public BlockRoomAddinfo Addinfo { get; set; }
    }

    public partial class BlockRoomAddinfo
    {
        public string AvailabilityType { get; set; }
        public string TraceId { get; set; }
        public long ResponseStatus { get; set; }
        public Error Error { get; set; }
        public bool GstAllowed { get; set; }
        public bool IsPriceChanged { get; set; }
        public bool IsPackageFare { get; set; }
        public bool IsCancellationPolicyChanged { get; set; }
        public bool IsHotelPolicyChanged { get; set; }
        public string HotelNorms { get; set; }
        public List<HotelRoomsDetail> HotelRoomsDetails { get; set; }
        public string HotelName { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public long StarRating { get; set; }
        public string HotelPolicyDetail { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public bool BookingAllowedForRoamer { get; set; }
    }

    //refer block room request model price node
    public class HotelPassenger
    {
        [Required]
        public string Title { get; set; }
        public long? PaxId { get; set; }
        [Required]
        public string FirstName { get; set; }
       
        public string MiddleName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public PaxType PaxType { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public bool LeadPassenger { get; set; } = false;
        [Required]
        public int Age { get; set; }
        public string PassportNo { get; set; }
        public string PassportExpDate { get; set; }
        public string PassportIssueDate { get; set; }
        public string phoneno { get; set; }
        public string GSTCompanyAddress { get; set; }
        public string GSTCompanyContactNumber { get; set; }
        public string GSTCompanyName { get; set; }
        public string GSTNumber { get; set; }
        public string GSTCompanyEmail { get; set; }
    }
    public enum PaxType { Adult = 1, Child = 2 }

}