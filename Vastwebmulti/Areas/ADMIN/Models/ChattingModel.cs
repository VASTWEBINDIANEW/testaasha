using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    /// <summary>
    /// Model representing a chat message entry for the in-app admin messaging feature.
    /// </summary>
    public class ChattingModel
    {
        public IEnumerable<Vastwebmulti.Models.chat_user_Result> Chatuser { get; set; }
        public IEnumerable<Vastwebmulti.Models.proc_Complaint_request_Result>Complainreq { get; set; }


        public IEnumerable<Vastwebmulti.Models.whitelabel_chat_user_Result> WChatuser { get; set; }
        public IEnumerable<Vastwebmulti.Models.proc_whitelabel_complaint_request_Result> WComplainreq { get; set; }
    }
}