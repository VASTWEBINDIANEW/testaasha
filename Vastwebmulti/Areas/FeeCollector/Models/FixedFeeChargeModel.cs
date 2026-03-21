using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Vastwebmulti.Areas.FeeCollector.Models
{
    public class FixedFeeChargeModel
    {
        [Required]
        public string FeeName { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        public decimal FeeValue { get; set; }
        [Required]
        public StudentClasses stdClass { get; set; }
        public List<Vastwebmulti.Models.FixedFeeCharge> lstFeeCharges { get; set; }

    }
}