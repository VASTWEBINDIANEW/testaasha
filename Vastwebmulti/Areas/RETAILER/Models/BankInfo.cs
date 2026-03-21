using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.RETAILER.Models
{
    public class BankInfo
    {
       public IEnumerable<Vastwebmulti.Models.bank_info> adminbank { get; set; }
       public IEnumerable<Vastwebmulti.Models.bank_info> Dealerbank { get; set; }
        public IEnumerable<Vastwebmulti.Models.tblwallet_info> Walletinfoadmin { get; set; }
        public IEnumerable<Vastwebmulti.Models.tblwallet_info> Walletinfodealer { get; set; }
    }
}