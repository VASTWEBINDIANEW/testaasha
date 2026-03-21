using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    public class Common_status_master_dlm_cls
    {
        public int idno { get; set; }
        public string Role { get; set; }
        public string Userid { get; set; }
        public string Frmname { get; set; }
        public string Name { get; set; }
        public string mobile { get; set; }
        public string Comm_Status { get; set; }
        public DateTime updatetime { get; set; }
    }
    public class Common_status_master_dlm_Transfer_cls
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

    public class CommonstatusMaster_dlm_list
    {
        public IEnumerable<Common_status_master_dlm_cls> Common_status_master_dlmlist { get; set; }
        public IEnumerable<Common_status_master_dlm_Transfer_cls> Common_status_master_dlm_Transferlist { get; set; }
    }
}