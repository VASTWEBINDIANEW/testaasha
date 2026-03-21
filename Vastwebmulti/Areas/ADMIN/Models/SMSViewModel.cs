using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Vastwebmulti.Models;
using System.Web.Mvc;
using Vastwebmulti.Areas.ADMIN.ViewModel;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    public class SMSViewModel
    {
        public int id { get; set; }
        [Required(ErrorMessage = "Enter The Sms Api")]
        [Url(ErrorMessage = "Not A Valid Url (Like Http:// or Https://)")]
        public string smsapi { get; set; }
        [Required(ErrorMessage = "Enter The Sms Api")]
        [Url(ErrorMessage = "Not A Valid Url (Like Http:// or Https://)")]
        public string smsapi1 { get; set; }
        public string smssts { get; set; }
        public string userfor { get; set; }
        public apismslist1 apiapisms { get; set; }
       public SMS_Priority sms_priority { get; set; }
        public srs_show_sent_message_Result srs_show_sent_message_result { get; set; }
        public sms_api_entry SMS_API_entry { get; set; }
        public SMSSendAll smssendall { get; set; }

        public IEnumerable<apismslist1> apismslist_all { set; get; }
        public List<SMS_Priority> SMS_PriorityList { get; set; }
        public List<srs_show_sent_message_Result> srs_show_sent_message_Result_List { get; set; }

        public List<sms_api_entry> sms_api_entryList { get; set; }

        public List<SMSSendAll> SMSSendAllslist { get; set; }
        public sms_api_entry sms_api_entrybyfilter { get; set; }
      public List<SelectListItem> portmanager { get; set; }
      public IEnumerable<Sending_SMS_TemplatesViewmodel> Sending_SMS_Templates_reports { get; set; }
        public SMSViewModel()
        {
            this.portmanager = new  List<SelectListItem>();
        }
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