using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    public class TwoFactorEnabled
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public bool TwoFactorEnable { get; set; }
        public string passcodetype { get; set; }
        public string UserRole { get; set; }
    }
}