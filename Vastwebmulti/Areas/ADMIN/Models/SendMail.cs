using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    public class SendMail
    {
        public string Userform1 { get; set; }
        public string txtEmail { get; set; }
        public string txtSubject { get; set; }
        public string txtMsgBody { get; set; }
    }
}