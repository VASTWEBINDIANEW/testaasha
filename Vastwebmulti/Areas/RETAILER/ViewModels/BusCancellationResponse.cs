using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.RETAILER.ViewModels
{
    public partial class BusCancellationResponse
    {
        public string Version { get; set; }
        public long StatusCode { get; set; }
        public BusCrInfoContent Content { get; set; }
    }

    public partial class BusCrInfoContent
    {
        public long ResponseCode { get; set; }
        public BusCrInfoAddinfo Addinfo { get; set; }
    }

    public partial class BusCrInfoAddinfo
    {
        public long ResponseStatus { get; set; }
        public Error Error { get; set; }
        public string TraceId { get; set; }
        public List<BusCrInfo> BusCrInfo { get; set; }
    }

    public partial class BusCrInfo
    {
        public CancellationChargeBreakUp CancellationChargeBreakUp { get; set; }
        public long ChangeRequestId { get; set; }
        public string CreditNoteNo { get; set; }
        public BusCancellationStatusEnum ChangeRequestStatus { get; set; }
        public DateTime CreditNoteCreatedOn { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal RefundedAmount { get; set; }
        public decimal CancellationCharge { get; set; }
        public decimal TotalServiceCharge { get; set; }
        public decimal TotalGstAmount { get; set; }
        public Gst Gst { get; set; }
    }

    public partial class CancellationChargeBreakUp
    {
        public decimal CancellationFees { get; set; }
        public decimal CancellationServiceCharge { get; set; }
    }

    public enum BusCancellationStatusEnum
    {
        NOTSET = 0,FAILED=1, PENDING = 2, SUCCESS = 3
    }


    

   

}