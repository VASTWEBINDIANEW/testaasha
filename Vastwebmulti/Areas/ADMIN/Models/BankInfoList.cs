using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    /// <summary>
    /// View model containing lists of bank accounts and wallet info for admin management.
    /// </summary>
    public class BankInfoList
    {
        public IEnumerable<Vastwebmulti.Models.bank_info> bankinfo { get; set; }
        public IEnumerable<Vastwebmulti.Models.tblwallet_info> walletinfo { get; set; }
    }
}