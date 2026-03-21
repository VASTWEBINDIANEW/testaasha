using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    /// <summary>
    /// View model for composing and sending email notifications to users.
    /// </summary>
    public class Email_ViewModel
    {
        public List<priorityEmail> priorityEmail { get; set; }
        public List<Sent_Mail_History> Sent_Mail_History { get; set; }
        public List<SendMail> SendMail { get;set; }
    }
}