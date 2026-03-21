using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.WHITELABEL.Models
{
        public  class fundtransferretailer
        {
            public int idno { get; set; }
            public string whitelabelid { get; set; }
            [Required(ErrorMessage = "Select Retailer Id")]
            public string retailerid { get; set; }
        public string retailerid1 { get; set; }
        public string retailername { get; set; }
            [Required(ErrorMessage = "Enter The Amount")]
            [RegularExpression("^[+-]?([0-9]*\\.?[0-9]+|[0-9]+\\.?[0-9]*)([eE][+-]?[0-9]+)?$", ErrorMessage = "Only Numeric Value")]
            public decimal? balance { get; set; }
            public decimal? whitelabelpre { get; set; }
            public decimal? whitelabelpost { get; set; }
            public decimal? retaileridpre { get; set; }
            public decimal? retaileridpost { get; set; }
            public System.DateTime? transfer_date { get; set; }
            [Required(ErrorMessage = "Select Balance Type")]
            public string baltype { get; set; }
            [Required(ErrorMessage = "Enter Old Cr.")]
           public decimal? oldcrbal { get; set; }
            public string comment { get; set; }
        public decimal? cr { get; set; }
        public decimal? Fundvalue { get; set; }
        public IEnumerable<fundtransferretailer> FundTransfer_retailer { set; get; }
    }
    }