using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    public class dealer_remain_token_report
    {
        public string DealerId { get; set; }
        public string Email { get; set; }
        public int Tokens { get; set; }
        public string DealerName { get; set; }
        public int FarmName { get; set; }
    }
}