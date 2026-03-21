using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.RETAILER.Enums
{
    public class Enums
    {
    }
    public enum ChangeRequestStatus
    {
        NotSet = 0,
        Unassigned = 1,
        Assigned = 2,
        Acknowledged = 3,
        Completed = 4,
        Rejected = 5,
        Closed = 6,
        Pending = 7, Other = 8
    }
    public enum FlightCancellationType
    {
        NotSet = 0,
        NoShow = 1,
        FlightCancelled = 2,
        Others = 3
    }
    public enum FlightCancellationRequestType
    {
        NotSet = 0,
        FullCancellation = 1,
        PartialCancellation = 2,
        Reissuance = 3
    }
    public enum JourneyType
    {
        OneWay = 1,
        Return = 2,
        MultiStop = 3,
        AdvanceSearch = 4,
        SpecialReturn=5
    }
    public enum TicketStatus
    {
        Failed = 0,
        Successful = 1,
        NotSaved = 2,
        NotCreated = 3,
        NotAllowed = 4,
        InProgress = 5,
        TicketeAlreadyCreated = 6,
        PriceChanged = 8,
        OtherError = 9
    }




}