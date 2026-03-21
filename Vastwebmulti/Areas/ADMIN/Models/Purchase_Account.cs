using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    /// <summary>
    /// Model representing a purchase account transaction entry.
    /// </summary>
    public class Purchase_Account
    {
        public IEnumerable<Purchase_Account_info> infoaccount { get; set; }
       public string msg { get; set; } 
    }
}