using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Models
{
    public class ServiceReport
    {
        public IEnumerable<SERVICECOLUMN> s11 { get; set; }
        public IEnumerable<SERVICES_DATA_INFO> s22 { get; set; }
    }
}