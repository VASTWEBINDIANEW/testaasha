using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.WHITELABEL.ViewModel
{
    public class Sending_SMS_TemplatesViewmodel
    {
        public int idno  { get; set; }
        public string Templateid { get; set; }
       
        public string SMSAPI{ get; set; }
        public int SMSAPID{ get; set; }
        public string SMS_TYPE { get; set; }
        public string Templates { get; set; }
      
    }
    public class SendingSMSTemplateReports
    {
        public IEnumerable<Sending_SMS_TemplatesViewmodel> Sending_SMS_Templates_reports { get; set; }
   
    
    }

    public class Sending_SMS_Templateslist
    {
        public string Templateid { get; set; }
        public string SMSAPIID { get; set; }
        public string SMS_TYPE { get; set; }
        public string Templates { get; set; }
        public DateTime templatedate { get; set; }

    }

}