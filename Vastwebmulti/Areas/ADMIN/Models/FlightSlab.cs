using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    public class FlightSlab
    {
        public List<Slab_Flight> common { get; set; }
        public List<Slab_Flight> all { get; set; }
        public List<SelectListItem> UserId { get; set; }
        public string role { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
    }
   
}