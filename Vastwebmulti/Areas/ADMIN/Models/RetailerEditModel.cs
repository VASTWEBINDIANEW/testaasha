using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    /// <summary>
    /// Form model for editing an existing retailer account profile and settings.
    /// </summary>
    public class RetailerEditModel
    {
            public string DealerId { get; set; }
            public string RetailerId { get; set; }
            public string RetailerName { get; set; }
            public int State { get; set; }
            public int District { get; set; }
            public string Mobile { get; set; }
            public string Address { get; set; }
            public int Pincode { get; set; }
            public string Email { get; set; }
            public System.DateTime JoinDate { get; set; }
            public string Photo { get; set; }
            public string Status { get; set; }
            public string Frm_Name { get; set; }
            public string slab_name { get; set; }
            public string AadharCard { get; set; }
            public string PanCard { get; set; }
            public Nullable<int> caption { get; set; }
            public string gst { get; set; }
            public Nullable<bool> moneysts { get; set; }
            public string PIN { get; set; }
            public string dateofbirth { get; set; }
            public string PartnerID { get; set; }
            public string AepsMerchandId { get; set; }
            public string AepsMPIN { get; set; }
        }
    
}