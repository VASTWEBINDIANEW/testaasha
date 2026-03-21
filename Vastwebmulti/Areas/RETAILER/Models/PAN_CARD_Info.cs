using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.RETAILER.Models
{
    public class PAN_CARD_Info
    {
        public int Idno { get; set; }
        public string Name { get; set; }
        public string FName { get; set; }
        public string Mobile { get; set; }



        public string Acknoledge { get; set; }
        public DateTime Date { get; set; }
        public string State { get; set; }
        public string District { get; set; }

        public string Retailer { get; set; }

        public string Status { get; set; }
        public string IsApproved { get; set; }
        public bool? IsDocUploaded { get; set; }
        public bool? IsHidden { get; set; }
        public bool? IsDocAccepted { get; set; }
        public bool? IsPhysical { get; set; }

    }
}