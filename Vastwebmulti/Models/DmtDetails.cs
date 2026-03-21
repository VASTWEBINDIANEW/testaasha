using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Models
{
    public class DmtDetails
    {
        public string Userid { get; set; }
        public string Senderno { get; set; }
        public string SenderName { get; set; }
        public string Remiterid { get; set; }
        public string benname { get; set; }
        public string ifsccode { get; set; }
        public string accountno { get; set; }
        public string benid { get; set; }
        public string otp { get; set; }
        public string BankName { get; set; }
        public decimal amount { get; set; }
        public string Mode { get; set; }
        public string Transid { get; set; }
        public string Tokenid { get; set; }
        public string originalifsccode { get; set; }
        public string AadharNumber { get; set; }
        public string client_id { get; set; }
    }
}