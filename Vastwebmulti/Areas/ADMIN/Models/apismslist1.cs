using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    public partial class apismslist1
    {
        public int id { get; set; }
        [Required(ErrorMessage = "Enter The Sms Api")]
        [Url(ErrorMessage = "Not A Valid Url (Like Http:// or Https://)")]
        public string smsapi { get; set; }
        [Required(ErrorMessage = "Enter The Sms Api")]
        [Url(ErrorMessage = "Not A Valid Url (Like Http:// or Https://)")]
        public string smsapi1 { get; set; }
        public string sts { get; set; }
        public string userfor { get; set; }
        public string smsremainapi { get; set; }
        public string api_type { get; set; }

        public IEnumerable<apismslist1> apismslist_all { set; get; }
    }
}