using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.RETAILER.Models
{
    public class Servicefee
    {
        public IEnumerable<Vastwebmulti.Models.PaidServiceCharge_list_Result> PaidList { get; set; }
        public IEnumerable<Vastwebmulti.Models.PaidServicesPaymentHistory> PaidHistorty { get; set; }
   
        public List<AddForm_information> AddForm_information { get; set; }
        public List<Retailerformsubmit> Retailerformsubmit { get; set; }
        public AddForm_information AddForm_informationbyid { get; set; }
    }
}