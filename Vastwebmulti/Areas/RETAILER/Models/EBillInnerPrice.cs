using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.RETAILER.Models
{
    public class EBillInnerPrice
    {
        [JsonProperty("label")]
        public string label { get; set; }
        [JsonProperty("value")]
        public string value { get; set; }
    }
}