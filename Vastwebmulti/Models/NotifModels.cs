using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Models
{
    public class NotifModels
    {
        public string UserID { get; set; }
        public string Message { get; set; }
        public string DetaileUrl { get; set; }
        public string Title { get; set; }
    }
    public class UpdateNotifReadProp
    {
        public int Id { get; set; }
    }

}