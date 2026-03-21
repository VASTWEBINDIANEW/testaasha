using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    public class LoginImg
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public string Role { get; set; }
        public string UserID { get; set; }
        public string otherimage { get; set; }
        public string UsedFor { get; set; }
        public string StatusCheck { get; set; }
    }
}