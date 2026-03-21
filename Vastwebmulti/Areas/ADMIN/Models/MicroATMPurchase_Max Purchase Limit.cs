using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    public class MicroATMPurchase_Max_Purchase_Limit
    {
        public string Retailer_ID { get; set; }
        //public string UserWise { get; set; }

        //public string Card_type { get; set; }
       // public decimal? MaxAmount_Limit { get; set; }
        public List<hold_microATM_Purchase> holdlist { get; set; }
        public List<SelectListItem> retailer_lists { get; set; }
      
        public string userid { get; set; }
        public decimal maxamountlimit { get; set; }
        public string Card_type { get; set; }
        public List<RetailerMicroATMPurchaseamountlist> microchargelist { get; set; }
    }
    public class RetailerMicroATMPurchaseamountlist
    {
        public int idno { get; set; }
        public string userinfo { get; set; }
        public string userid { get; set; }
        public decimal? maxamount { get; set; }
        public string Card_type { get; set; }
     //   public string chargefrom { get; set; }

    }


}