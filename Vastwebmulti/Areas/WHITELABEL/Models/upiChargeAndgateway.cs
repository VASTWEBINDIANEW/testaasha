using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.WHITELABEL.Models
{
    public class upiChargeAndgateway
    {
        public IEnumerable<Whitelabel_Upi_slab> Upi_slabs { get; set; }
        public IEnumerable<Whitelabel_PaymentGatewaycharge> PaymentGatewaycharges { get; set; }
        public IEnumerable<Whitelabel_show_upi_txn_details_Result> UPIREPORT { get; set; }
        public IEnumerable<Whitelabel_gateway_report_Result> GATEWAYREPORT { get; set; }
        public IEnumerable<Whitelabel_Admin_UPI> Admin_UPIs { get; set; }
        public List<Whitelabel_Paymentgateway_min_max> Paymentgateway_min_maxs { get; set; }
        public List<Whitelabel_PaymentGateway_Status> PaymentGateway_Status { get; set; }
        public string msg { get; set; }
    }
}