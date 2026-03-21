using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.RETAILER.Models
{
    public  class AepsResponseJson
    {
        public string Version { get; set; }
        public long StatusCode { get; set; }
        public AepsContent Content { get; set; }
    }

    public class AepsContent
    {
        public long ResponseCode { get; set; }
        public AepsAddinfo ADDINFO { get; set; }
    }

    public class AepsAddinfo
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public AepsData Data { get; set; }
        public long StatusCode { get; set; }
    }

    public class AepsData
    {
        public string TerminalId { get; set; }
        public string RequestTransactionTime { get; set; }
        public long TransactionAmount { get; set; }
        public string TransactionStatus { get; set; }
        public double BalanceAmount { get; set; }
        public string BankRrn { get; set; }
        public string Bank { get; set; }
        public string TransactionType { get; set; }
        public string FpTransactionId { get; set; }

        public string messagetypeIdentifier { get; set; }
        public string primaryBitmap { get; set; }
        public string secondaryBitmap { get; set; }
        public string processingCode { get; set; }
        public string transmissionDateandTime { get; set; }
        public string systemsTraceAuditNumber { get; set; }
        public string localTransactionTime { get; set; }
        public string localTransactionDate { get; set; }
        public string cardExpiryDate { get; set; }
        public string merchantCategorycode { get; set; }
        public string pOSEntryMode { get; set; }
        public string pOSServicecode { get; set; }
        public string acquiringInstitutionIdentificationCode { get; set; }
        public string retrievalReferenceNumber { get; set; }
        public string authorizationIdentificationResponse { get; set; }
        public string responseCode { get; set; }
        public string cardAcceptorTerminalIdentification { get; set; }
        public string merchantCode { get; set; }
        public string cardAcceptorName { get; set; }
        public string accountbalanceamount { get; set; }
        public string uIDAIAuthenticationCode { get; set; }
        public string transactionCurrencyCode { get; set; }
        public string transactionFee { get; set; }
        public string transactionServiceCharge { get; set; }
        public string accountIdentification1 { get; set; }
        public string accountIdentification2 { get; set; }
        public string customerName { get; set; }
        public string merchantOrBCPassCode { get; set; }
        public string personalIdentificationNumber { get; set; }
        public string transactionProcessingIndicator { get; set; }
        public string uIDBiometricAuthenticationData { get; set; }
        public string uIDAuthenticationinputdata { get; set; }
        public string additionalData { get; set; }
        public string cardNumberOrUIDNumber { get; set; }
        public string additionalAmounts { get; set; }
         public string privateUse { get; set; }
         public string reservedPrivateUse { get; set; }
         public string otp { get; set; }
         public string reservedPrivateUse1 { get; set; }
         public string uidaiauthenticationCode { get; set; }
         public string bcIdentificationCode { get; set; }
         public string transactionIdentifier { get; set; }
         public string bcRrn { get; set; }
         public string responseTimeStamp { get; set; }
         public string pan { get; set; }
         public string requestedRemarks { get; set; }
         public string responseMessage { get; set; }
         public string responseRequestId { get; set; }
         public string responseOrderId { get; set; }
         public string responseAgentCustId { get; set; }
         public string responseStatus { get; set; }
         public string payloadStatus { get; set; }
         public string uidaiAuthCode { get; set; }
         public string balanceIndicator { get; set; }
         public string balanceType { get; set; }
         public string currencyCode { get; set; }
         public string receiptRequired { get; set; }
         public string bankName { get; set; }
         public string productType { get; set; }
         public string terminalCode { get; set; }
        public string errorCode { get; set; }
        public string errorMessage { get; set; }
        public string miniStatementTransactions { get; set; }
        public string bankErrorMessage { get; set; }
        public string responseCardAcceptorTerminalIdentification { get; set; }
        public string bankSwitch { get; set; }
        public string validationFlag { get; set; }
        public string uidnumber { get; set; }
        public string uidtransactionAuthCode { get; set; }
        public string transactionReferrenceNumber { get; set; }
    }
}