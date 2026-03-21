using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vastwebmulti.Models;
namespace Vastwebmulti.Areas.WHITELABEL.Models
{
    public class PancardSlabModel
    {
        public List<Pancard_whitelabel_CommonAll_comm> common { get; set; }
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
        public string Email { get; set; }
        public string RolesName { get; set; }
        public decimal? tokenvalueDigital { get; set; }
        public decimal? commDigital { get; set; }
        public decimal? tokenvaluePhysical { get; set; }
        public decimal? commPhysical { get; set; }
        public decimal? gst { get; set; }
    }
    public class MasterUpdatePanComm
    {
        public int idno { get; set; }
        public decimal? tokenvalueDigital { get; set; }
        public decimal? commDigital { get; set; }
        public decimal? tokenvaluePhysical { get; set; }
        public decimal? commPhysical { get; set; }
        public decimal? gst { get; set; }
    }
    public class DealerPanComm
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public string Email { get; set; }
        public string RolesName { get; set; }
        public decimal? tokenvalueDigital { get; set; }
        public decimal? commDigital { get; set; }
        public decimal? tokenvaluePhysical { get; set; }
        public decimal? commPhysical { get; set; }
        public decimal? gst { get; set; }
    }
    public class DealerUpdatePanComm
    {
        public int idno { get; set; }
        public decimal? tokenvalueDigital { get; set; }
        public decimal? commDigital { get; set; }
        public decimal? tokenvaluePhysical { get; set; }
        public decimal? commPhysical { get; set; }
        public decimal? gst { get; set; }
    }
    public class RetailerPanComm
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public string Email { get; set; }
        public string RolesName { get; set; }
        public decimal? tokenvalueDigital { get; set; }
        public decimal? commDigital { get; set; }
        public decimal? tokenvaluePhysical { get; set; }
        public decimal? commPhysical { get; set; }
        public decimal? gst { get; set; }

    }
    public class dlmremPanComm
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public string Email { get; set; }
        public string RolesName { get; set; }
        public decimal? tokenvalueDigital { get; set; }
        public decimal? dlmremcommDigital { get; set; }
        public decimal? tokenvaluePhysical { get; set; }
        public decimal? dlmremcommPhysical { get; set; }
        public decimal? gst { get; set; }

    }

    public class RetailerUpdatePanComm
    {
        public int idno { get; set; }

        public decimal? tokenvalueDigital { get; set; }
        public decimal? commDigital { get; set; }
        public decimal? tokenvaluePhysical { get; set; }
        public decimal? commPhysical { get; set; }
        public decimal? commdlmremDigital { get; set; }
        public decimal? commdlmremPhysical { get; set; }
        public decimal? gst { get; set; }

    }
    public class dlmremUpdatePanComm
    {
        public int idno { get; set; }

        public decimal? tokenvalueDigital { get; set; }
        public decimal? commDigital { get; set; }
        public decimal? tokenvaluePhysical { get; set; }
        public decimal? commPhysical { get; set; }
        public decimal? commdlmremDigital { get; set; }
        public decimal? commdlmremPhysical { get; set; }
        public decimal? gst { get; set; }

    }

    public class APIPanComm
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public string Email { get; set; }
        public string RolesName { get; set; }
        public decimal? tokenvalueDigital { get; set; }
        public decimal? commDigital { get; set; }
        public decimal? tokenvaluePhysical { get; set; }
        public decimal? commPhysical { get; set; }
        public decimal? commdlmremDigital { get; set; }
        public decimal? commdlmremPhysical { get; set; }
        public decimal? gst { get; set; }

    }

    public class APIUpdatePanComm
    {
        public int idno { get; set; }

        public decimal? tokenvalueDigital { get; set; }
        public decimal? commDigital { get; set; }
        public decimal? tokenvaluePhysical { get; set; }
        public decimal? commPhysical { get; set; }
        public decimal? commdlmremDigital { get; set; }
        public decimal? commdlmremPhysical { get; set; }
        public decimal? gst { get; set; }

    }

    public class WhitelabelPanComm
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public string Email { get; set; }
        public string RolesName { get; set; }
        public decimal? tokenvalueDigital { get; set; }
        public decimal? commDigital { get; set; }
        public decimal? tokenvaluePhysical { get; set; }
        public decimal? commPhysical { get; set; }
        public decimal? gst { get; set; }

    }

    public class WhitelabelUpdatePanComm
    {
        public int idno { get; set; }
        public decimal? tokenvalueDigital { get; set; }
        public decimal? commDigital { get; set; }
        public decimal? tokenvaluePhysical { get; set; }
        public decimal? commPhysical { get; set; }
        public decimal? commdlmremDigital { get; set; }
        public decimal? commdlmremPhysical { get; set; }
        public decimal? gst { get; set; }
    }
}