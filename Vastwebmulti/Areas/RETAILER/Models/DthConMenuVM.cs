using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.RETAILER.Models
{
    public class DthConMenuVM
    {
        public List<DthConCatagoryModel> lstCatagory { get; set; }
        public List<DthConSubCatagoryModel> subCata { get; set; }
    }
    public class DthConCatagoryModel
    {
        public string CatName { get; set; }
        public int CatID { get; set; }

    }
    public class DthConSubCatagoryModel
    {
        //public int? CatID { get; set; }
        public int SubcatID { get; set; }
        public string SubCatagoryName { get; set; }
    }
}