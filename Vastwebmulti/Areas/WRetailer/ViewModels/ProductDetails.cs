using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.WRetailer.ViewModels
{
    public class ProductDetails
    {
        public List<proc_ProductList_Result> lstProducts { get; set; }
        public string CataTitle { get; set; }
        public int itemsInCart { get; set; }
    }
    public class ProductVM
    {
        public string ProductName { get; set; }
        public string SortBy { get; set; }
        public int CartCount { get; set; }
        public int CartProdId { get; set; }
        public decimal CartTotalAmount { get; set; }
    }
}