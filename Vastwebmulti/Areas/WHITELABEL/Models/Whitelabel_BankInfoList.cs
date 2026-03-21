using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.WHITELABEL.Models
{
    public class Whitelabel_BankInfoList
    {
        public IEnumerable<Vastwebmulti.Models.Whitelabel_bank_info> bankinfo { get; set; }
        public IEnumerable<Vastwebmulti.Models.Whitelabel_Wallet_info> walletinfo { get; set; }
    }
}