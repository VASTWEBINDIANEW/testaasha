using com.sun.jndi.toolkit.ctx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    public class AchiveHistory
    {
        public string FarmName { get; set; }
        public string Name { get; set; }
        public string Mobile { get; set; }
        public string Role { get; set; }
        public Nullable<decimal> Total_TargetSet { get; set; }
        public string Opt_Type { get; set; }
        public Nullable<decimal> Total_Txn_amt { get; set; }
        public string Target_Type { get; set; }
        public string Achived_Perce { get; set; }
        public string Received_Perce { get; set; }
        public Nullable<decimal> rem_pre { get; set; }
        public Nullable<decimal> rem_post { get; set; }
        public string total_achived_amt { get; set; }
        public Nullable<decimal> Admin_Pre { get; set; }
        public Nullable<decimal> Admin_Post { get; set; }
        public Nullable<decimal> GST { get; set; }
        public Nullable<decimal> TDS { get; set; }
        public Nullable<System.DateTime> Achived_Date { get; set; }
    }

    public class UsersWise
    {
        public string FarmName { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; }
    }
}