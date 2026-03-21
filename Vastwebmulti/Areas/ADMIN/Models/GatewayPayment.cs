using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    /// <summary>
    /// Aggregate view model for managing payment gateway charge configurations across user roles.
    /// </summary>
    public class GatewayPayment
    {
        public List<PaymentGatewaycharge_new> userWise { get; set; }
        public List<PaymentGatewaycharge_new_common> common { get; set; }
        public List<GatewayPaymentRetailerComm> Retaileruser { get; set; }
        public List<GatewayPaymentDealerComm> Dealeruser { get; set; }
        public List<GatewayPaymentMasterComm> Materuser { get; set; }
        public List<GatewayPaymentAPIComm> APIuser { get; set; }

        public List<GatewayPaymentUpdateRetailerComm> UpdateRetailer { get; set; }
        public List<GatewayPaymentUpdateDealerComm> UpdateDealer { get; set; }
        public List<GatewayPaymentUpdateMasterComm> UpdateMaster { get; set; }
        public List<GatewayPaymentUpdateAPIComm> UpdateAPI { get; set; }
      
        public List<SelectListItem> UserId { get; set; }
        public List<SelectListItem> Retailerid { get; set; }
        public string role { get; set; }
        public bool GstOnly { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }
    public class GatewayPaymentRetailerComm
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public string Email { get; set; }
        public bool? CreditCardsts { get; set; }
        public bool? DebitCardsts { get; set; }
        public bool? netbankingsts { get; set; }
        public bool? cashcardsts { get; set; }
        public bool? walletsts { get; set; }
        public bool? upists { get; set; }
        public decimal? debitupto2000 { get; set; }
        public decimal? debitabove2000 { get; set; }
        public decimal? rupaydebit { get; set; }
        public decimal? creditcard { get; set; }
        public decimal? netbanking { get; set; }
        public decimal? amex { get; set; }
        public decimal? internationalcard { get; set; }
        public decimal? wallet { get; set; }
        public decimal? UPI { get; set; }
        public decimal? prepaidcard { get; set; }
        public decimal? creditcardmin { get; set; }
        public decimal? creditcardmax { get; set; }
        public decimal? netbankmin { get; set; }
        public decimal? netbankmax { get; set; }
        public decimal? debitmin { get; set; }
        public decimal? debitmax { get; set; }
        public decimal? walletmin { get; set; }
        public decimal? upimin { get; set; }
        public decimal? upimax { get; set; }
        public decimal? walletmax { get; set; }
        public decimal? axis { get; set; }
        public decimal? others { get; set; }
        public string Retailerid { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string role { get; set; }
        public string creditsettlement { get; set; }

    }
    public class GatewayPaymentDealerComm
    {
        public string creditsettlement { get; set; }
        public int idno { get; set; }
        public string userid { get; set; }
        public string Email { get; set; }
        public bool? CreditCardsts { get; set; }
        public bool? DebitCardsts { get; set; }
        public bool? netbankingsts { get; set; }
        public bool? cashcardsts { get; set; }
        public bool? walletsts { get; set; }
        public bool? upists { get; set; }
        public decimal? debitupto2000 { get; set; }
        public decimal? debitabove2000 { get; set; }
        public decimal? rupaydebit { get; set; }
        public decimal? creditcard { get; set; }
        public decimal? netbanking { get; set; }
        public decimal? amex { get; set; }
        public decimal? internationalcard { get; set; }
        public decimal? wallet { get; set; }
        public decimal? UPI { get; set; }
        public decimal? prepaidcard { get; set; }
        public decimal? creditcardmin { get; set; }
        public decimal? creditcardmax { get; set; }
        public decimal? netbankmin { get; set; }
        public decimal? netbankmax { get; set; }
        public decimal? debitmin { get; set; }
        public decimal? debitmax { get; set; }
        public decimal? walletmin { get; set; }
        public decimal? upimin { get; set; }
        public decimal? upimax { get; set; }
        public decimal? walletmax { get; set; }
        public decimal? axis { get; set; }
        public decimal? others { get; set; }
        public string Retailerid { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string role { get; set; }

    }
    public class GatewayPaymentMasterComm
    {
        public string creditsettlement { get; set; }
        public int idno { get; set; }
        public string userid { get; set; }
        public string Email { get; set; }
        public bool? CreditCardsts { get; set; }
        public bool? DebitCardsts { get; set; }
        public bool? netbankingsts { get; set; }
        public bool? cashcardsts { get; set; }
        public bool? walletsts { get; set; }
        public bool? upists { get; set; }
        public decimal? debitupto2000 { get; set; }
        public decimal? debitabove2000 { get; set; }
        public decimal? rupaydebit { get; set; }
        public decimal? creditcard { get; set; }
        public decimal? netbanking { get; set; }
        public decimal? amex { get; set; }
        public decimal? internationalcard { get; set; }
        public decimal? wallet { get; set; }
        public decimal? UPI { get; set; }
        public decimal? prepaidcard { get; set; }
        public decimal? creditcardmin { get; set; }
        public decimal? creditcardmax { get; set; }
        public decimal? netbankmin { get; set; }
        public decimal? netbankmax { get; set; }
        public decimal? debitmin { get; set; }
        public decimal? debitmax { get; set; }
        public decimal? walletmin { get; set; }
        public decimal? upimin { get; set; }
        public decimal? upimax { get; set; }
        public decimal? walletmax { get; set; }
        public decimal? axis { get; set; }
        public decimal? others { get; set; }
        public string Retailerid { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string role { get; set; }

    }
    public class GatewayPaymentAPIComm
    {
        public string creditsettlement { get; set; }
        public int idno { get; set; }
        public string userid { get; set; }
        public string Email { get; set; }
        public bool? CreditCardsts { get; set; }
        public bool? DebitCardsts { get; set; }
        public bool? netbankingsts { get; set; }
        public bool? cashcardsts { get; set; }
        public bool? walletsts { get; set; }
        public bool? upists { get; set; }
        public decimal? debitupto2000 { get; set; }
        public decimal? debitabove2000 { get; set; }
        public decimal? rupaydebit { get; set; }
        public decimal? creditcard { get; set; }
        public decimal? netbanking { get; set; }
        public decimal? amex { get; set; }
        public decimal? internationalcard { get; set; }
        public decimal? wallet { get; set; }
        public decimal? UPI { get; set; }
        public decimal? prepaidcard { get; set; }
        public decimal? creditcardmin { get; set; }
        public decimal? creditcardmax { get; set; }
        public decimal? netbankmin { get; set; }
        public decimal? netbankmax { get; set; }
        public decimal? debitmin { get; set; }
        public decimal? debitmax { get; set; }
        public decimal? walletmin { get; set; }
        public decimal? upimin { get; set; }
        public decimal? upimax { get; set; }
        public decimal? walletmax { get; set; }
        public decimal? axis { get; set; }
        public decimal? others { get; set; }
        public string Retailerid { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string role { get; set; }

    }

    public class GatewayPaymentUpdateRetailerComm
    {
        public string creditsettlement { get; set; }
        public int idno { get; set; }
        public string userid { get; set; }
        public string Email { get; set; }
        public bool? CreditCardsts { get; set; }
        public bool? DebitCardsts { get; set; }
        public bool? netbankingsts { get; set; }
        public bool? cashcardsts { get; set; }
        public bool? walletsts { get; set; }
        public bool? upists { get; set; }
        public decimal? debitupto2000 { get; set; }
        public decimal? debitabove2000 { get; set; }
        public decimal? rupaydebit { get; set; }
        public decimal? creditcard { get; set; }
        public decimal? netbanking { get; set; }
        public decimal? amex { get; set; }
        public decimal? internationalcard { get; set; }
        public decimal? wallet { get; set; }
        public decimal? UPI { get; set; }
        public decimal? prepaidcard { get; set; }
        public decimal? creditcardmin { get; set; }
        public decimal? creditcardmax { get; set; }
        public decimal? netbankmin { get; set; }
        public decimal? netbankmax { get; set; }
        public decimal? debitmin { get; set; }
        public decimal? debitmax { get; set; }
        public decimal? walletmin { get; set; }
        public decimal? walletmax { get; set; }
        public decimal? upimin { get; set; }
        public decimal? upimax { get; set; }
        public decimal? axis { get; set; }
        public decimal? others { get; set; }
    }
    public class GatewayPaymentUpdateDealerComm
    {
        public string creditsettlement { get; set; }
        public int idno { get; set; }
        public string userid { get; set; }
        public string Email { get; set; }
        public bool? CreditCardsts { get; set; }
        public bool? DebitCardsts { get; set; }
        public bool? netbankingsts { get; set; }
        public bool? cashcardsts { get; set; }
        public bool? walletsts { get; set; }
        public bool? upists { get; set; }
        public decimal? debitupto2000 { get; set; }
        public decimal? debitabove2000 { get; set; }
        public decimal? rupaydebit { get; set; }
        public decimal? creditcard { get; set; }
        public decimal? netbanking { get; set; }
        public decimal? amex { get; set; }
        public decimal? internationalcard { get; set; }
        public decimal? wallet { get; set; }
        public decimal? UPI { get; set; }
        public decimal? prepaidcard { get; set; }
        public decimal? creditcardmin { get; set; }
        public decimal? creditcardmax { get; set; }
        public decimal? netbankmin { get; set; }
        public decimal? netbankmax { get; set; }
        public decimal? debitmin { get; set; }
        public decimal? debitmax { get; set; }
        public decimal? walletmin { get; set; }
        public decimal? walletmax { get; set; }
        public decimal? upimin { get; set; }
        public decimal? upimax { get; set; }
        public decimal? axis { get; set; }
        public decimal? others { get; set; }
    }
    public class GatewayPaymentUpdateMasterComm
    {
        public string creditsettlement { get; set; }
        public int idno { get; set; }
        public string userid { get; set; }
        public string Email { get; set; }
        public bool? CreditCardsts { get; set; }
        public bool? DebitCardsts { get; set; }
        public bool? netbankingsts { get; set; }
        public bool? cashcardsts { get; set; }
        public bool? walletsts { get; set; }
        public bool? upists { get; set; }
        public decimal? debitupto2000 { get; set; }
        public decimal? debitabove2000 { get; set; }
        public decimal? rupaydebit { get; set; }
        public decimal? creditcard { get; set; }
        public decimal? netbanking { get; set; }
        public decimal? amex { get; set; }
        public decimal? internationalcard { get; set; }
        public decimal? wallet { get; set; }
        public decimal? UPI { get; set; }
        public decimal? prepaidcard { get; set; }
        public decimal? creditcardmin { get; set; }
        public decimal? creditcardmax { get; set; }
        public decimal? netbankmin { get; set; }
        public decimal? netbankmax { get; set; }
        public decimal? debitmin { get; set; }
        public decimal? debitmax { get; set; }
        public decimal? walletmin { get; set; }
        public decimal? walletmax { get; set; }
        public decimal? upimin { get; set; }
        public decimal? upimax { get; set; }
        public decimal? axis { get; set; }
        public decimal? others { get; set; }
    }
    public class GatewayPaymentUpdateAPIComm
    {
        public string creditsettlement { get; set; }
        public int idno { get; set; }
        public string userid { get; set; }
        public string Email { get; set; }
        public bool? CreditCardsts { get; set; }
        public bool? DebitCardsts { get; set; }
        public bool? netbankingsts { get; set; }
        public bool? cashcardsts { get; set; }
        public bool? walletsts { get; set; }
        public bool? upists { get; set; }
        public decimal? debitupto2000 { get; set; }
        public decimal? debitabove2000 { get; set; }
        public decimal? rupaydebit { get; set; }
        public decimal? creditcard { get; set; }
        public decimal? netbanking { get; set; }
        public decimal? amex { get; set; }
        public decimal? internationalcard { get; set; }
        public decimal? wallet { get; set; }
        public decimal? UPI { get; set; }
        public decimal? prepaidcard { get; set; }
        public decimal? creditcardmin { get; set; }
        public decimal? creditcardmax { get; set; }
        public decimal? netbankmin { get; set; }
        public decimal? netbankmax { get; set; }
        public decimal? debitmin { get; set; }
        public decimal? debitmax { get; set; }
        public decimal? walletmin { get; set; }
        public decimal? walletmax { get; set; }
        public decimal? upimin { get; set; }
        public decimal? upimax { get; set; }
        public decimal? axis { get; set; }
        public decimal? others { get; set; }
    }
}