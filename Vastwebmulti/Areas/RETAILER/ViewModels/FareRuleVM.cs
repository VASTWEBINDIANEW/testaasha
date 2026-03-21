using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.RETAILER.ViewModels
{
    public class FareRuleVM
    {
        public string Version { get; set; }
        public long StatusCode { get; set; }
        public FareRuleContent Content { get; set; }
    }
    public partial class FareRuleContent
    {
        public long ResponseCode { get; set; }
        public FareRuleAddinfo Addinfo { get; set; }
    }

    public partial class FareRuleAddinfo
    {
        public Error Error { get; set; }
        public List<FareRule> FareRules { get; set; }
        public int ResponseStatus { get; set; }
        public string TraceId { get; set; }
    }




}