using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.WHITELABEL.Models
{
    public class WhiteLabelRefferalListViewModel
    {
        public int referalid { get; set; }

        public decimal BenifitRecived { get; set; }
        public string sharingtype { get; set; }
        public decimal sharingvalue { get; set; }
        public decimal PreParrentRetailerBalance { get; set; }
        public decimal PostParrentRetailerBalance { get; set; }
        public DateTime RecivedDate { get; set; }
        public Whitelabel_Retailer_Details parrentfromretailer { get; set; }
        public string RefferedType { get; set; }
        public string ParentReatilerID { get; set; }
        public string childRetailerID { get; set; }
        public string ParrentRetailerName { get; set; }
        public string childRetailerName { get; set; }
    }
}