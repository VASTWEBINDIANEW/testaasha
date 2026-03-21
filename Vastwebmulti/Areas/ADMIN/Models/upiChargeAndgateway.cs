using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vastwebmulti.Models;
using Vastwebmulti.Models.Enumdata;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    public class upiChargeAndgateway
    {
        public IEnumerable<Upi_slab> Upi_slabs { get; set; }
        public IEnumerable<PaymentGatewaycharge> PaymentGatewaycharges { get; set; }
        public IEnumerable<show_upi_txn_details_Result> UPIREPORT { get; set; }
        public IEnumerable<gateway_report_Result> GATEWAYREPORT { get; set; }
        public IEnumerable<Admin_UPI> Admin_UPIs { get; set; }
        public IEnumerable<Dealer_VPAID> Dealer_VPAID { get; set; }
        public IEnumerable<admin_VPAID> admin_VPAIDs { get; set; }
        public List<Paymentgateway_min_max> Paymentgateway_min_maxs { get; set; }
        public List<PaymentGateway_Status> PaymentGateway_Status { get; set; } 
        public Upi_Qr_codeGenerator Upi_Qr_codeGeneratorss { get; set; }

        public string vpatypeid { get; set; }
        public SelupIEnumlist ddlpaytmtype { get; set; }
        public string msg { get; set; }
    }
}