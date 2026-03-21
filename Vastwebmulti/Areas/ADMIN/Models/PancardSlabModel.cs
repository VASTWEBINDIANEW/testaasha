using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vastwebmulti.Models;
namespace Vastwebmulti.Areas.ADMIN.Models
{
    /// <summary>
    /// View model for managing PAN card service commission slab configurations.
    /// </summary>
    public class PancardSlabModel
    {
        public List<Pancard_common_comm_new> common { get; set; }
        public List<Pancard_common_comm_new_manual> commonmanual { get; set; }
       
        public List<MasterPanComm> Materuser { get; set; }
        public List<DealerPanComm> Dealeruser { get; set; }
        public List<RetailerPanComm> Retaileruser { get; set; }
        public List<dlmremPanComm> dlmremuser { get; set; }
        public List<APIPanComm> APIuser { get; set; }
        public List<WhitelabelPanComm> Whitelabeluser { get; set; }
        public List<MasterUpdatePanComm> UpdateMaster { get; set; }
        public List<DealerUpdatePanComm> UpdateDealer { get; set; }
        public List<RetailerUpdatePanComm> UpdateRetailer { get; set; }
        public List<dlmremUpdatePanComm> Updatedlmrem { get; set; }
        public List<APIUpdatePanComm> UpdateAPI { get; set; }

        public List<WhitelabelUpdatePanComm> UpdateWhitelabel { get; set; }
        public List<SelectListItem> UserId { get; set; }
        public List<SelectListItem> Retailerid { get; set; }
        public string role { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }
    public class MasterPanComm
    {
        public int idno { get; set; }
        public string userid { get; set; }
       
        public decimal? comm { get; set; }
      
    }
    public class MasterUpdatePanComm
    {
        public int idno { get; set; }
        public string userid { get; set; }

        public decimal? comm { get; set; }

    }
    public class DealerPanComm
    {
        public int idno { get; set; }
        public string userid { get; set; }

        public decimal? comm { get; set; }

    }
    public class DealerUpdatePanComm
    {
        public int idno { get; set; }
        public string userid { get; set; }

        public decimal? comm { get; set; }

    }
    public class RetailerPanComm
    {
        public int idno { get; set; }
        public string userid { get; set; }

        public decimal? comm { get; set; }


    }
    public class dlmremPanComm
    {
        public int idno { get; set; }
        public string userid { get; set; }

        public decimal? comm { get; set; }


    }

    public class RetailerUpdatePanComm
    {
        public int idno { get; set; }
        public string userid { get; set; }

        public decimal? comm { get; set; }


    }
    public class dlmremUpdatePanComm
    {
        public int idno { get; set; }
        public string userid { get; set; }

        public decimal? comm { get; set; }


    }

    public class APIPanComm
    {
        public int idno { get; set; }
        public string userid { get; set; }

        public decimal? comm { get; set; }


    }

    public class APIUpdatePanComm
    {
        public int idno { get; set; }
        public string userid { get; set; }

        public decimal? comm { get; set; }

    }

    public class WhitelabelPanComm
    {
        public int idno { get; set; }
        public string userid { get; set; }

        public decimal? comm { get; set; }


    }

    public class WhitelabelUpdatePanComm
    {
        public int idno { get; set; }
        public string userid { get; set; }

        public decimal? comm { get; set; }

    }
}