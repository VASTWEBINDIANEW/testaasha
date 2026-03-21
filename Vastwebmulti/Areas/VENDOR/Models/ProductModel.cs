using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vastwebmulti.CustomeAttributes;

namespace Vastwebmulti.Areas.VENDOR.Models
{
    public class ProductModel
    {
        public int ProductID { get; set; }
        [Required(ErrorMessage = "Sub catagory is required.")]
        [Display(Name = "Sub Catagory")]
        public Nullable<int> SubCatID { get; set; }
        [Required(ErrorMessage = "Name is required.")]
        [Display(Name = "Name")]
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        [AllowHtml]
        public string FullDescription { get; set; }
        public bool Published { get; set; }
        [Display(Name = "Show On Home")]
        public bool ShowOnHome { get; set; }
        public string HSN { get; set; }
        public short GST { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public System.DateTime UpdatedOn { get; set; }
        public int AttID { get; set; }
        public string AttValue { get; set; }
        public string AttName { get; set; }
        public string Photo { get; set; }
        [GreaterThan("StandardPrice")]
        public decimal ListPrice { get; set; }
        
        public decimal StandardPrice { get; set; }
        public decimal AdminComm { get; set; }
        public decimal MDComm { get; set; }
        public decimal DEComm { get; set; }
        public decimal REMComm { get; set; }
        public decimal WhiteLabelComm { get; set; }
        public int MinOrderQty { get; set; }
        public int MaxOrderQty { get; set; }
        public int OnOrderQty { get; set; }
        public Nullable<short> MinQtyFlag { get; set; }
        public Nullable<int> stock { get; set; }
        public string SubCatName { get; set; }
        public int CatID { get; set; }
        public string CatName { get; set; }
    }
}