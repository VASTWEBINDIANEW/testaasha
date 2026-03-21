using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.RETAILER.Models
{
    public class CartVM
    {
        public int productId { get; set; }
        public string productName { get; set; }
        public string imgUrl { get; set; }
        public decimal ListPrice { get; set; }
        public decimal satandardPrice { get; set; }
        public int Qty { get; set; }
        public string VenderID { get; set; }
        public string VenderFirmName { get; set; }
        public int MinOrderQty { get; set; }
    }
}