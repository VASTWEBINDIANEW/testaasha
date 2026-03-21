using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vastwebmulti.Models;
using Vastwebmulti.Models.Enumdata;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    /// <summary>
    /// Model for configuring UPI transaction status and gateway settings.
    /// </summary>
    public class UPI_status_setting
    {
         public IList<UPI_QR_API> upiqr { get; set; }
         public IList<UPI_Intent_API> upiintent { get; set; }
         public IList<UPI_Collection_API> upicollection { get; set; }
         public IList<Payment_GateWay_API> gateway { get; set; }
         public IList<UPIandGatewayStatu> upiandgateway { get; set; }
         public IList<Upi_slab> Upi_slabs { get; set; }
         public IList<Admin_UPI> Admin_UPIs { get; set; }
         public IList<admin_VPAID> admin_VPAIDs { get; set; }
        public Upi_Qr_codeGenerator Upi_Qr_codeGeneratorss { get; set; }
        public IEnumerable<show_upi_txn_details_Result> UPIREPORT { get; set; }
        public SelupIEnumlist ddlpaytmtype { get; set; }
        public string msg { get; set; }
    }
}