using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.WHITELABEL.Models.JsonFile
{
    public class NewsUpdate
    {
        public int Id { get; set; }
        public string Role { get; set; }
        public string UserId { get; set; }
        public string Image { get; set; }
        public string UpdateContent { get; set; }
    }
}