using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    public class CatagoryModel
    {
        public int CatId { get; set; }
        public string CatName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public List<SubCatagoryModel> SubCatagories { get; set; }
    }

    public class SubCatagoryModel
    {
        public int SubCatId { get; set; }
        public int CatId { get; set; }
        public string SubCatName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}