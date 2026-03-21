using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace Vastwebmulti.Areas.API.Models
{
    public class API_Slab_ViewModel
    {
        public string OperatorName { get; set; }
        public string OperatorCode { get; set; }
        public string OperatorType { get; set; }
        public decimal? comm { get; set; }
        public decimal? commrs { get; set; }
        public decimal? Maxrs { get; set; }
    }
}