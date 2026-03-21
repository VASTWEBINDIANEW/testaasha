using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.RETAILER.Models
{
    public class AepsModel
    {

        public string merchantTransactionId { get; set; }
        public string merchantTranId { get; set; }
        public CaptureResponse captureResponse { get; set; }
        public CardnumberOruid cardnumberORUID { get; set; }
        public string languageCode { get { return "en"; } }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string mobileNumber { get; set; }
        public string paymentType { get { return "B"; } }
        public string requestRemarks { get; set; }
        public string timestamp { get; set; }
        //public long TransactionAmount { get; set; }
        public long transactionAmount { get; set; }
        public string transactionType { get; set; }
        public string merchantUserName { get; set; }
        public string merchantPin { get; set; }
        public string subMerchantId { get; set; }
        public string superMerchantId { get; set; }
        public string merchantLoginId { get; set; }
        public string primaryKeyId { get; set; }
        public string encodeFPTxnId { get; set; }
    }
    public class AepsModelNew
    {
        public CaptureResponse captureResponse { get; set; }
        public CardnumberOruid cardnumberORUID { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string requestRemarks { get; set; }
        public string transactionType { get; set; }
        public string merchantUserName { get; set; }
        public string merchantPin { get; set; }
        public string superMerchantId { get; set; }
        public string merchantTranId { get; set; }
        public string mobileNumber { get; set; }
        public string serviceType { get; set; }
    }
    public class CardnumberOruid
    {
        public string adhaarNumber { get; set; }
        public long indicatorforUID { get { return 0; } }
        public long? nationalBankIdentificationNumber { get; set; }
    }
    public class CaptureResponse
    {
        public string PidDatatype { get; set; }
        public string Piddata { get; set; }
        public string ci { get; set; }
        public string dc { get; set; }
        public string dpID { get; set; }
        public string errCode { get; set; }
     //   public string iType { get; set; }
        public string errInfo { get; set; }
        public string fCount { get; set; }
        public string fType { get; set; }
        public string hmac { get; set; }
        public string iCount { get; set; }
        public string mc { get; set; }
        public string mi { get; set; }
        public string nmPoints { get; set; }
        public string pCount { get; set; }
        public string pType { get; set; }
        public string qScore { get; set; }
        public string rdsID { get; set; }
        public string rdsVer { get; set; }
        public string sessionKey { get; set; }
    }
}