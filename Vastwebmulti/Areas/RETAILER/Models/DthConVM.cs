using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.RETAILER.Models
{
    public class DthConVM
    {
        public string ProductName { get; set; }
        public string SortBy { get; set; }
        public int CartCount { get; set; }
        public int CartProdId { get; set; }
        public decimal CartTotalAmount { get; set; }
        public int itemsInCart { get; set; }
    }
}