using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    public class WalletToBankSlabEditModel
    {
        public int Idno { get; set; }
        public decimal AmtFromTatkal { get; set; }
        public decimal AmtToTatkal { get; set; }
        public decimal chargeTatkal { get; set; }
        public decimal TimeTakesTatkal { get; set; }
        
    }
}