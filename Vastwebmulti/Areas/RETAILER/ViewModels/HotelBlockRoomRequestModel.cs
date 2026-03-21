using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.RETAILER.ViewModels
{
        public class HotelBlockRoomRequestModel
        {
            public long ResultIndex { get; set; }
            public string HotelCode { get; set; }
            public string HotelName { get; set; }
            public string GuestNationality { get; set; }
            public long NoOfRooms { get; set; }
            public string ClientReferenceNo { get; set; }
            public bool IsVoucherBooking { get; set; }
            public List<BlockHotelRoomsDetail> HotelRoomsDetails { get; set; }
            
            public string TraceId { get; set; }
        }

        public class BlockHotelRoomsDetail
        {
            public long RoomIndex { get; set; }
            public string RoomTypeCode { get; set; }
            public string RoomTypeName { get; set; }
            public string RatePlanCode { get; set; }
            public object BedTypeCode { get; set; }
            public SmokingPreference SmokingPreference { get; set; }
            public object Supplements { get; set; }
            public BlockPrice Price { get; set; }
        }

        public class BlockPrice
        {
            public string CurrencyCode { get; set; }
            public decimal RoomPrice { get; set; }
            public decimal Tax { get; set; }
            public decimal ExtraGuestCharge { get; set; }
            public decimal ChildCharge { get; set; }
            public decimal OtherCharges { get; set; }
            public decimal Discount { get; set; }
            public decimal PublishedPrice { get; set; }
            public decimal PublishedPriceRoundedOff { get; set; }
            public decimal OfferedPrice { get; set; }
            public decimal OfferedPriceRoundedOff { get; set; }
            public decimal AgentCommission { get; set; }
            public decimal AgentMarkUp { get; set; }
            public decimal ServiceTax { get; set; }
            public decimal TDS { get; set; }
        }
    public enum SmokingPreference
    {
        NoPreference = 0,
        Smoking = 1,
        NonSmoking = 2,
        Either = 3,
    }

}