using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    /// <summary>
    /// View model for configuring AEPS and MPOS settlement settings.
    /// </summary>
    public class AepsMposSettlemtSettingModel
    {
        public int Idno { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }
        public bool IsDirect { get; set; }
        public decimal settlemtHourse { get; set; }
    }
}