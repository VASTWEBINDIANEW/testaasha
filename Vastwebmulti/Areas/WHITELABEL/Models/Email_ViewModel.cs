using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.WHITELABEL.Models
{
    public class Email_ViewModel
    {
        public List<Whitelabel_priorityEmail> priorityEmail { get; set; }
        public List<Whitelabel_Sent_Mail_History> Sent_Mail_History { get; set; }
    }
}