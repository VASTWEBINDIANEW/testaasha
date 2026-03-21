using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Models
{
    public class ApiRecharge
    {
        public string UserID { get; set; }
        public string Customernumber { get; set; }
        public string Optcode { get; set; }
        public decimal Amount { get; set; }
        public string Yourrchid { get; set; }
        public string optional1 { get; set; }
        public string optional2 { get; set; }
        public string optional3 { get; set; }
        public string optional4 { get; set; }
        public string Tokenid { get; set; }
    }
}