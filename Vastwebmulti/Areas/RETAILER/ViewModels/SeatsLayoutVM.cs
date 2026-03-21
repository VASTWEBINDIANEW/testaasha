using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.RETAILER.ViewModels
{
    public class SeatsLayoutVM
    {
        public string Version { get; set; }
        public long StatusCode { get; set; }
        public LayoutContent Content { get; set; }
        public List<PickAndDropPointsDetail> boardingPoints { get; set; }
        public List<PickAndDropPointsDetail> droppingPoints { get; set; }
        public string ResultIndex { get; set; }
        public DateTime doj { get; set; }
        public string source { get; set; }
        public string destination { get; set; }
    }
    public partial class LayoutContent
    {
        public long ResponseCode { get; set; }
        public LayoutAddinfo Addinfo { get; set; }
    }

    public partial class LayoutAddinfo
    {
        public long ResponseStatus { get; set; }
        public Error Error { get; set; }
        public string TraceId { get; set; }
        public SeatLayoutDetails SeatLayoutDetails { get; set; }
    }
    public partial class SeatLayoutDetails
    {
        public long AvailableSeats { get; set; }
        public string HtmlLayout { get; set; }
        public SeatLayout SeatLayout { get; set; }
    }

    public partial class SeatLayout
    {
        public long NoOfColumns { get; set; }
        public long NoOfRows { get; set; }
        public List<List<SeatDetail>> SeatDetails { get; set; }
    }
    public partial class SeatDetail
    {
        public string ColumnNo { get; set; }
        public long Height { get; set; }
        public bool IsLadiesSeat { get; set; }
        public bool IsMalesSeat { get; set; }
        public bool IsUpper { get; set; }
        public string RowNo { get; set; }
        public decimal SeatFare { get; set; }
        public long SeatIndex { get; set; }
        public string SeatName { get; set; }
        public bool SeatStatus { get; set; }
        public EnumSeatType SeatType { get; set; }
        public long Width { get; set; }
        public BusPrice Price { get; set; }
    }
    
    public enum EnumSeatType
    {
        NotSet = 0, seat = 1, sleeper = 2,
        seatCumSleeper = 3, UpperBerth =
        4, LowerBerth = 5, vertical_sleeper=6
    }
}