using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    /// <summary>
    /// Model for capturing additional verification or information check parameters.
    /// </summary>
    public class Extrainfochk
    {
        public string optcode { get; set; }
        public decimal? move1 { get; set; }
        public decimal? move2 { get; set; }
        public decimal? move3 { get; set; }
        public decimal? move4 { get; set; }
        public decimal? move5 { get; set; }
        public string operator_Name { get; set; }
        public string Operator_type { get; set; }
    }
}