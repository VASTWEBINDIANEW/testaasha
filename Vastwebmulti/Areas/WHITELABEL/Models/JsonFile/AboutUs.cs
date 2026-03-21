using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.WHITELABEL.Models.JsonFile
{
    public class AboutUs
    {
        public int AboutId { get; set; }
        public string AboutHeading { get; set; }
        public string AboutContent { get; set; }
        public string UserId { get; set; }
        public string Role { get; set; }
    }
}