using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.DEALER.Models
{
    public class dealerCommon_status_master_dlm_Transfer_cls
    {
        public int idno { get; set; }
        public string Role { get; set; }
        public string Userid { get; set; }
        public string Frmname { get; set; }
        public string Name { get; set; }
        public string mobile { get; set; }
        public DateTime Hold_date { get; set; }
        public DateTime trasnferdate { get; set; }
        public decimal master_remain { get; set; }
        public decimal master_remain_pre { get; set; }
        public decimal admin_prebal { get; set; }
        public decimal admin_postbal { get; set; }
        public decimal totalcomm { get; set; }
    }
}