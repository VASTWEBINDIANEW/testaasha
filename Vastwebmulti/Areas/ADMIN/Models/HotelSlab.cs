using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    /// <summary>
    /// View model for managing hotel booking commission slab configurations.
    /// </summary>
    public class HotelSlab
    {
        public List<Slab_Hotel> common { get; set; }
        public List<Slab_Hotel> all { get; set; }
        public List<SelectListItem> UserId { get; set; }
        public string role { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
    }
}