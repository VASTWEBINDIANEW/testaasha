using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    /// <summary>
    /// View model for displaying TDS (Tax Deducted at Source) and GST reports across all user levels.
    /// </summary>
    public class TDSReportModel
    {
        public IEnumerable<Vastwebmulti.Models.TDS_Report_Result> TDSAdmin { get; set; }
        public IEnumerable<Vastwebmulti.Models.TDS_Report_master_Result> TDSMaster { get; set; }
        public IEnumerable<Vastwebmulti.Models.TDS_Report_Dealer_Result> TDSDealer { get; set; }
        public IEnumerable<Vastwebmulti.Models.TDS_Report_Retailer_Result> TDSRetailer { get; set; }
    }
    public class GSTReportModel
    {
        public IEnumerable<Vastwebmulti.Models.GST_Report_Admin_Result> GSTAdmin { get; set; }
        public IEnumerable<Vastwebmulti.Models.GST_Report_master_Result> GSTMaster { get; set; }
        public IEnumerable<Vastwebmulti.Models.GST_Report_Dealer_All_Result> GSTDealer { get; set; }
        public IEnumerable<Vastwebmulti.Models.GST_Report_Retailer_All_Result> GSTRetailer { get; set; }
    }
    public class DeleteUser
    {
        public IEnumerable<Vastwebmulti.Models.DeleteUserHistory> Deleteuserall { get; set; }
    }
}