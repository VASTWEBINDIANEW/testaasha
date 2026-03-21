using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.WHITELABEL.Models
{
    public class fundtransferdealer
    {

        public string whitelabelid { get; set; }
        public string DealerName { get; set; }
        [Required(ErrorMessage = "Select Dealer Id")]
        public string dealerid { get; set; }
        public string dealerid1 { get; set; }
        [Required(ErrorMessage = "Enter The Amount")]
        [RegularExpression("^[+-]?([0-9]*\\.?[0-9]+|[0-9]+\\.?[0-9]*)([eE][+-]?[0-9]+)?$", ErrorMessage = "Only Numeric Value")]
        public decimal? balance { get; set; }
        public decimal? whitelabelpre { get; set; }
        public decimal? whitelabelpost { get; set; }
        public decimal? dealerpre { get; set; }
        public decimal? dealerpost { get; set; }
        public DateTime? transfer_date { get; set; }
        [Required(ErrorMessage = "Select Balance Type")]
        public string bal_type { get; set; }
        [Required(ErrorMessage = "Enter Old Cr.")]
        public decimal? oldcrbalance { get; set; }
        public string comment { get; set; }
        public decimal? cr { set; get; }
        public decimal? Fundvalue { set; get; }
        public IEnumerable<fundtransferdealer> FundTransfer_Dealer { set; get; }
    }
}