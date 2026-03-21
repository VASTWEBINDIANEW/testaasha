using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    public class optmergeforsell
    {
     public IEnumerable<Vastwebmulti.Models.operatorcommforsellStatu> optcomm { get; set; }
     public IEnumerable<Vastwebmulti.Models.operatorMerge> optmerge { get; set; }
    }
}