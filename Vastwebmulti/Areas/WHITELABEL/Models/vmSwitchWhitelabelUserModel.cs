using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.WHITELABEL.Models
{
    public class vmSwitchWhitelabelUserModel
    {
        public IEnumerable<Whitelabel_ShowDealerMDSwitch_Result> ShowSwitch_DealerTOMD { get; set; }
        public IEnumerable<Whitelabel_ShowRetailerDealerSwitch_Result> ShowRetailerDealerSwitch_Result { get; set; }
    }
}