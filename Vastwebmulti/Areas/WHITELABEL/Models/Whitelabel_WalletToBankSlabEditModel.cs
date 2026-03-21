using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.WHITELABEL.Models
{
    public class Whitelabel_WalletToBankSlabEditModel
    {
        public int Idno { get; set; }
        public decimal amtFromGeneral { get; set; }
        public decimal amtToGeneral { get; set; }
        public decimal chargeGeneral { get; set; }
        public decimal TimeTakesGeneral { get; set; }
        public decimal amtFromTatkal { get; set; }
        public decimal amtToTatkal { get; set; }
        public decimal chargeTatkal { get; set; }
        public decimal TimeTakesTatkal { get; set; }
    }
}