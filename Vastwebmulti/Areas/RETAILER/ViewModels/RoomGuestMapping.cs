using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.RETAILER.ViewModels
{
    public class HotelGuests
    {
        public List<RoomGuestMapping> RoomNodes { get; set; }
    }

    public class RoomGuestMapping
    {
        public int NoOfAdults { get; set; }
        public int NoOfChild { get; set; }
        public int[] ChildAge { get; set; }
    }
}