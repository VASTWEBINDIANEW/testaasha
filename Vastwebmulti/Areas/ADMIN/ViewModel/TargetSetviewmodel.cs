using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vastwebmulti.Areas.ADMIN.Models;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.ADMIN.ViewModel
{
    public class TargetSetviewmodel
    {
        public IEnumerable<SelectListItem> tblproductlist { get; set; }
        public IEnumerable<tblRetailerSetTarget> remTargetCategory { get; set; }
        public IEnumerable<tblDealerSetTarget> dlmTargetCategory { get; set; }
        public IEnumerable<tblsuperstockistsettarget> mdTargetCategory { get; set; }
        public IEnumerable<tblAPIsettarget> apiTargetCategory { get; set; }
        public IEnumerable<recent_Retailer_Result> recent_Retailer { get; set; }
        public IEnumerable<DashBoard_Report_top_Result> DashBoard_Report_top { get; set; }
        public IEnumerable<tblPruductGift> productItems { get; set; }
        public string RoleName { get; set; }
        public IEnumerable<tblRetailerSetTarget> remTargetCategoryNxt { get; set; }
        public IEnumerable<tblDealerSetTarget> dlmTargetCategoryNxt { get; set; }
        public IEnumerable<tblsuperstockistsettarget> mdTargetCategoryNxt { get; set; }
        public IEnumerable<tblAPIsettarget> apiTargetCategoryNxt { get; set; }
        public IEnumerable<string> CategoryImages { get; set; }
        public IEnumerable<AchiveHistory> AchivedTarget_His { get; set; }
    }
    public class Whitelabel_TargetSetviewmodel
    {
        public IEnumerable<SelectListItem> tblproductlist { get; set; }
        public IEnumerable<Whitelabel_RetailerSetTarget> remTargetCategory { get; set; }
        public IEnumerable<Whitelabel_DealerSetTarget> dlmTargetCategory { get; set; }
        public IEnumerable<Whitelabel_superstockistsettarget> mdTargetCategory { get; set; }
        public IEnumerable<Whitelabel_PruductGift> productItems { get; set; }
        public string RoleName { get; set; }
        public IEnumerable<Whitelabel_RetailerSetTarget> remTargetCategoryNxt { get; set; }
        public IEnumerable<Whitelabel_DealerSetTarget> dlmTargetCategoryNxt { get; set; }
        public IEnumerable<Whitelabel_superstockistsettarget> mdTargetCategoryNxt { get; set; }
        public IEnumerable<string> CategoryImages { get; set; }
        public IEnumerable<AchiveHistory> AchivedTarget_His { get; set; }
    }
}