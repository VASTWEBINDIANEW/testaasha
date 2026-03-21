using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.RETAILER.Models
{
    public class Referal
    {
       public IEnumerable<Vastwebmulti.Models.tblRefferalsetting> refsetting { get; set; }
       public IEnumerable<Vastwebmulti.Models.tblRefferalRetailerIncome> Refincome { get; set; }
    }
}