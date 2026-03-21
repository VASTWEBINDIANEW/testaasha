using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    /// <summary>
    /// View model for managing SMS gateway settings and templates.
    /// </summary>
    public class SMSSetting
    {
        public IEnumerable<Vastwebmulti.Models.SMSSendAll> usercreateinfo { get; set; }
    }
}