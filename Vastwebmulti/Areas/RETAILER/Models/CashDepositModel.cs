using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.RETAILER.Models
{
    public class CashDepositModel
    {
        public int superMerchantId { get; set; }
        public string merchantUserName { get; set; }
        public string merchantPin { get; set; }
        public string secretKey { get; set; }
        public string mobileNumber { get; set; }
        public string iin { get; set; }
        public string transactionType { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string merchantTranId { get; set; }
        public string accountNumber { get; set; }
        public double amount { get; set; }
        public string fingpayTransactionId { get; set; }
        public string otp { get; set; }
        public int cdPkId { get; set; }
        public string timestamp { get; set; }
        public string paymentType { get { return "B"; } }
    }
}