using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    public class BusSlabModel
    {
        public List<Slab_Bus> common { get; set; }
        public List<BusMasterComm> Materuser { get; set; }
        public List<BusDealerComm> Dealeruser { get; set; }
        public List<BusRetailerComm> Retaileruser { get; set; }
        public List<BusWhitelabelComm> Whitelabeluser { get; set; }
        public List<UpdateBusMasterComm> UpdateMaster { get; set; }
        public List<UpdateBusDealerComm> UpdateDealer { get; set; }
        public List<UpdateBusRetailerComm> UpdateRetailer { get; set; }

        public List<UpdateBusWhitelabelComm> UpdateWhitelabel { get; set; }
        public List<SelectListItem> UserId { get; set; }
        public string role { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }
    public class BusMasterComm
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string RolesName { get; set; }
        public decimal marginPercentage { get; set; }
        public decimal retailerMarkup { get; set; }
        public decimal gst { get; set; }
        public decimal tds { get; set; }

    }
    public class UpdateBusMasterComm
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public decimal marginPercentage { get; set; }
        public decimal retailerMarkup { get; set; }
        public decimal gst { get; set; }
        public decimal tds { get; set; }

    }
    public class BusDealerComm
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string RolesName { get; set; }
        public decimal marginPercentage { get; set; }
        public decimal retailerMarkup { get; set; }
        public decimal gst { get; set; }
        public decimal tds { get; set; }

    }
    public class UpdateBusDealerComm
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public decimal marginPercentage { get; set; }
        public decimal retailerMarkup { get; set; }
        public decimal gst { get; set; }
        public decimal tds { get; set; }

    }
    public class BusRetailerComm
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string RolesName { get; set; }
        public decimal marginPercentage { get; set; }
        public decimal retailerMarkup { get; set; }
        public decimal gst { get; set; }
        public decimal tds { get; set; }

    }

    public class UpdateBusRetailerComm
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public decimal marginPercentage { get; set; }
        public decimal retailerMarkup { get; set; }
        public decimal gst { get; set; }
        public decimal tds { get; set; }

    }

    public class BusWhitelabelComm
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string RolesName { get; set; }
        public decimal marginPercentage { get; set; }
        public decimal retailerMarkup { get; set; }
        public decimal gst { get; set; }
        public decimal tds { get; set; }

    }

    public class UpdateBusWhitelabelComm
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public decimal marginPercentage { get; set; }
        public decimal retailerMarkup { get; set; }
        public decimal gst { get; set; }
        public decimal tds { get; set; }

    }
}
