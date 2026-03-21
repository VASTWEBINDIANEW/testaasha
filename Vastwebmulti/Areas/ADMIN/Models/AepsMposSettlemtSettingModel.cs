using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    public class AepsMposSettlemtSettingModel
    {
        public int Idno { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }
        public bool IsDirect { get; set; }
        public decimal settlemtHourse { get; set; }
    }
}