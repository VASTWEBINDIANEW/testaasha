using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    /// <summary>
    /// Model for editing wallet-to-bank transfer commission slab configurations.
    /// </summary>
    public class WalletToBankSlabEditModel
    {
        public int Idno { get; set; }
        public decimal AmtFromTatkal { get; set; }
        public decimal AmtToTatkal { get; set; }
        public decimal chargeTatkal { get; set; }
        public decimal TimeTakesTatkal { get; set; }
        
    }
}