using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    public class BankInfoList
    {
        public IEnumerable<Vastwebmulti.Models.bank_info> bankinfo { get; set; }
        public IEnumerable<Vastwebmulti.Models.tblwallet_info> walletinfo { get; set; }
    }
}