using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.RETAILER.ViewModels
{
    public class BoradingPointDetails
    {
        public string Version { get; set; }
        public long StatusCode { get; set; }
        public BoradingPointContent Content { get; set; }
    }

    public partial class BoradingPointContent
    {
        public long ResponseCode { get; set; }
        public BoradingPointAddinfo Addinfo { get; set; }
    }

    public partial class BoradingPointAddinfo
    {
        public long ResponseStatus { get; set; }
        public Error Error { get; set; }
        public Guid TraceId { get; set; }
        public List<BoardingPointsDetail> BoardingPointsDetails { get; set; }
        public List<DroppingPointsDetail> DroppingPointsDetails { get; set; }
    }
    public partial class BoardingPointsDetail
    {
        public string CityPointAddress { get; set; }
        public long CityPointContactNumber { get; set; }
        public long CityPointIndex { get; set; }
        public string CityPointLandmark { get; set; }
        public string CityPointLocation { get; set; }
        public string CityPointName { get; set; }
        public DateTime CityPointTime { get; set; }
    }
    public partial class DroppingPointsDetail
    {
        public long CityPointIndex { get; set; }
        public string CityPointLocation { get; set; }
        public string CityPointName { get; set; }
        public DateTime CityPointTime { get; set; }
    }
}