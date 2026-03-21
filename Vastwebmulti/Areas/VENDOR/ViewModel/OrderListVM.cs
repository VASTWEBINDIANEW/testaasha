using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.VENDOR.ViewModel
{
    public class OrderListVM
    {
        public int Idno { get; set; }
        public int OrderID { get; set; }
        public string BuyerID { get; set; }
        public string VenderID { get; set; }
        public string BuyerRole { get; set; }
        public int ProductId { get; set; }
        public decimal ListPrice { get; set; }
        public decimal StandardPrice { get; set; }
        public decimal Amount { get; set; }
        public int Qty { get; set; }
        public int OrderStatus { get; set; }
        public string ProductName { get; set; }
        public string BuyerName { get; set; }
        public string BuyerPhone { get; set; }
        public  string Buyeraddress { get; set; }
        public string ProductPhoto { get; set; }
        public DateTime ? OrderDate { get; set; }
        public DeliveryStatus DeliveryStatus { get; set; }
        public DateTime ? DeliverOn { get; set; }

    }
    public enum DeliveryStatus { NotDeliver=0,Delivered=1,Returned=2}
}