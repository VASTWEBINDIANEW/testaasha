using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace Vastwebmulti.Areas.WRetailer.Models
{
    public class ReferralcodeListRem
    {
        public string whitelabelid { get; set; }
        public string whitelabelemail { get; set; }
        public string Retailerid { get; set; }
        public string Retailermail { get; set; }
        public string Retailername { get; set; }
        public string Retailerfirmname { get; set; }
        public decimal? Refamount { get; set; }
        public decimal? whitelabelremainpre { get; set; }
        public decimal? whitelabelremainpost { get; set; }
        public decimal? retailerremainpre { get; set; }
        public decimal? retailerremainpost { get; set; }
        public DateTime? refdate { get; set; }
        public IEnumerable<ReferralcodeListRem> Referalcode_benifitHistoryrem { set; get; }
    }
}