using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    public class remuploadgst
    {
        public int idno { get; set; }
        public string retailerid { get; set; }
        public string userid { get; set; }
        public string monthchk { get; set; }
        public string yearchk { get; set; }
        public string status { get; set; }
        public string uploadfile { get; set; }
        public string email { get; set; }
        public string firmname { get; set; }
        public string reason { get; set; }
    }
}