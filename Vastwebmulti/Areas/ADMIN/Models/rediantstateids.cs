using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vastwebmulti.Models;  

namespace Vastwebmulti.Areas.ADMIN.Models
{
    /// <summary>
    /// Model for storing Radiant payment gateway state ID mappings.
    /// </summary>
    public class rediantstateids
    {
        public int id { get; set; }
        public int state_code { get; set; } 
        public string dist { get; set; } 
        public string state { get; set; }
    }

    public class rediantdisids
    {
        public int id { get; set; }
        public int state_code { get; set; }
        public string state { get; set; }
    }
    public class rediantMcc
    {
        public int id { get; set; }
        public string category { get; set; }
        public string mcc { get; set; }
    }

    public class rediantbussinesstype
    {
        public int id { get; set; }
       
        public string bussinesstype { get; set; }
    }
}
    