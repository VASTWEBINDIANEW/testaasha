using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.RETAILER.ViewModels
{
    public class BusPaxInfoVM
    {
        public string selectedSeats { get; set; }
        public string selectedSeatsName { get; set; }
        public string busOperatorName { get; set; }
        public bool IdProofRequired { get; set; }
   
        public string City { get; set; }
  
        public string IP { get; set; }
     
        public BlockBusTicketModel bookInfo { get; set; }
    }
    public class BlockBusTicketModel
    {
        //public string EndUserIp { get; set; }
        public string ResultIndex { get; set; }
        //public long AgencyId { get; set; }
        public string TraceId { get; set; }
       // public string TokenId { get; set; }
        public long BoardingPointId { get; set; }
        public long DropingPointId { get; set; }
        //public long TokenAgencyId { get; set; }
        //public long TokenMemberId { get; set; }
        public string ClientId { get; set; }
        public List<BusPassenger> Passenger { get; set; }
        public PickAndDropPointsDetail droppingPoint { set; get; } //For ViewPage Only
        public PickAndDropPointsDetail boardingPoint { set; get; }//For ViewPage Only
        public string sourceCity { set; get; }//For ViewPage Only
        public string destinationCity { set; get; }//For ViewPage Only
        public string doj { set; get; }//For ViewPage Only
    }
    public partial class BusPassenger
    {
        public bool LeadPassenger { get; set; }
        public long PassengerId { get; set; }
        public string Title { get; set; }
        public string Address { get; set; }
        public long Age { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public BusPaxGenderEnum Gender { get; set; }
        public string IdNumber { get; set; }
        public BusPaxIdTypeEnum? IdType { get; set; }
        public string LastName { get; set; }
        public string Phoneno { get; set; }
        public SeatDetail Seat { get; set; }
    }
    public enum BusPaxGenderEnum {
        Male = 1,
        Female = 2
    }
    public enum BusPaxIdTypeEnum {
        PanNo = 1,
        VoteridCard=2, Passport = 3
    }

}