using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vastwebmulti.Areas.WHITELABEL.Models;
using Vastwebmulti.Areas.WHITELABEL.ViewModel;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.WHITELABEL.Models
{
    public class Whitelabel_SMSViewModel
    {
        public List<whitelabel_sms_api_entry> whitelabel_sms_api_entry { get; set; }
        public IEnumerable<apismslist1> SMS_API_LIST { set; get; }
        public apismslist1 apisms { get; set; }
        public IEnumerable<Sending_SMS_TemplatesViewmodel> Sending_SMS_Templates_reports { get; set; }
    }
}