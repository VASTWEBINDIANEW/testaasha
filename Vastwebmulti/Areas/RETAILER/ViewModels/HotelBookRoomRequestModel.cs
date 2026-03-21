using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.RETAILER.ViewModels
{
    public class HotelBookRoomRequestModel
    {
        public string GuestNationality { get; set; }
        public int NoOfRooms { get; set; }
        public string ClientReferenceNo { get; set; }
        public bool IsVoucherBooking { get; set; }
        public List<BookHotelRoomsDetail> HotelRoomsDetails { get; set; }
        public object PaymentInfo { get; set; }
        public long AgentReferenceNo { get; set; }
        public object FlightInfo { get; set; }
        public long OnlinePaymentId { get; set; }
        public object TransactionId { get; set; }
        public bool CancelAtPriceChangeAfterBooking { get; set; }
        public long AgencyId { get; set; }
        public string HotelCode { get; set; }
        public string HotelName { get; set; }
        public long ResultIndex { get; set; }
        public string TraceId { get; set; }
        public string EndUserIp { get; set; }
        public string TokenId { get; set; }
        public string ClientId { get; set; }
        public long TokenAgencyId { get; set; }
        public long TokenMemberId { get; set; }
        public object CategoryIndexes { get; set; }
        public object CategoryId { get; set; }
        public DateTime checkindate { get; set; }
        public DateTime checkoutdate { get; set; }
    }

    public partial class BookHotelRoomsDetail
    {
        public List<HotelPassenger> HotelPassenger { get; set; }
        public long RoomIndex { get; set; }
        public string RoomTypeCode { get; set; }
        public string RatePlanCode { get; set; }
        public object BedTypeCode { get; set; }
        public SmokingPreference SmokingPreference { get; set; }
        public object Supplements { get; set; }
        public BlockPrice Price { get; set; }
    }
}