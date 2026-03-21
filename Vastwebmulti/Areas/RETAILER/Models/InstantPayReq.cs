using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.RETAILER.Models
{
    public class InstantPayReq
    {
        public string token { get; set; }
        public string format { get; set; }

        public InstantPayParamsPOCO request { get; set; }

    }
    public class InstantPayParamsPOCO
    {
        public string mobile { get; set; }
        public string remittermobile { get; set; }

        public string name { get; set; }
        public string company { get; set; }

        public string pincode { get; set; }
        public string remitterid { get; set; }
        public string beneficiaryid { get; set; }

        public string ifsc { get; set; }
        public string account { get; set; }
        public string otp { get; set; }
        public string agentid { get; set; }
        public string amount { get; set; }
        public string mode { get; set; }
        public string branchid { get; set; }
        public string accountnumber { get; set; }
        public string cardnumber { get; set; }
        public string pin { get; set; }
        public string partnerpin { get; set; }
        public string fromdate { get; set; }
        public string todate { get; set; }
        public string senderidtype { get; set; }
        public string sendername { get; set; }
        public string sendergender { get; set; }
        public string employer { get; set; }
        public string email { get; set; }

        public string senderaddress { get; set; }
        public string sendermobile { get; set; }

        public string senderidnumber { get; set; }
        public string store_type { get; set; }

        public string receivername { get; set; }
        public string receivergender { get; set; }
        public string receiveraddress { get; set; }
        public string doctype { get; set; }
        public string filename { get; set; }
        public string image_url { get; set; }
        public string address { get; internal set; }
        public string outletid { get; internal set; }
        public string pan_no { get; internal set; }
        public string base64Content { get; internal set; }
        public string id { get; internal set; }

        public documentUpload document { get; set; }
    }
    public class documentUpload
    {
        public string id { get; set; }
        public string base64 { get; set; }
        public string filename { get; set; }

    }

}