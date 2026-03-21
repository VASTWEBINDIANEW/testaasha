using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    public class RchApiResponse
    {
        public int idno { get; set; }
        public string apiname { get; set; }
        public string Resptype { get; set; }
        public string KeyRequestid { get; set; }
        public string KeyStatus { get; set; }
        public string KeyOptID { get; set; }
        public string KeyMsg { get; set; }
        public string KeyRemainBal { get; set; }
        public string ValStatusSucess { get; set; }
        public string ValStatusFailed { get; set; }
        public string ValStatusPending { get; set; }
        public string sapratewith { get; set; }
        public string Keyerror { get; set; }
        public string KeyerrorMessage { get; set; }
        public string valerror { get; set; }
        public string valerrorMessage { get; set; }
    }
}