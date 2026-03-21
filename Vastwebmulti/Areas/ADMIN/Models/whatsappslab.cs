using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    /// <summary>
    /// View model for managing WhatsApp messaging service commission slab configurations.
    /// </summary>
    public class whatsappslab
    {
        public List<WhatsappCommon> common { get; set; }
        public List<SelectListItem> UserId { get; set; }
        public List<WhatsappApi> Apiuser { get; set; }
        public List<UpdateWhatsapp> Update_whatsapp_api { get; set; }
        public string role { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public decimal? PP1 { get; set; }
        public decimal? PP2 { get; set; }
        public decimal? PP3 { get; set; }
    }
    public class WhatsappApi
    {
        public int idno { get; set; }
        public decimal? P1 { get; set; }
        public decimal? P2 { get; set; }
        public decimal? P3 { get; set; }
        public string Email { get; set; }
        public string userid { get; set; }
        public string RolesName { get; set; }
        public string ApiID { get; set; }

    }
    public class WhatsappCommon
    {
        public int idno { get; set; }
        public decimal? P1 { get; set; }
        public decimal? P2 { get; set; }
        public decimal? P3 { get; set; }
        public decimal? T1 { get; set; }
        public decimal? T2 { get; set; }
        public decimal? T3 { get; set; }
        public decimal? PP1 { get; set; }
        public decimal? PP2 { get; set; }
        public decimal? PP3 { get; set; }
    }
    public class UpdateWhatsapp
    {
        public int idno { get; set; }
        public decimal? P1 { get; set; }
        public decimal? P2 { get; set; }
        public decimal? P3 { get; set; }

    }
}