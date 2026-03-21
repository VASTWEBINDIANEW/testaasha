using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.WRetailer.ViewModels
{
    public class MenuVM
    {
        public List<CatagoryModel> lstCatagory { get; set; }
        public List<SubCatagoryMoidel> subCata { get; set; }
    }
    public class CatagoryModel
    {
        public string CatName { get; set; }
        public int CatID { get; set; }
        
    }
       public class SubCatagoryMoidel
            {
                public int? CatID { get; set; }
                public int SubcatID { get; set; }
                public string SubCatagoryName { get; set; }
            }
   
}