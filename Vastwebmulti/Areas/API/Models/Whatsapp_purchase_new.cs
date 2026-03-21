using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.API.Models
{
    public class Whatsapp_purchase_new
    {
           public IEnumerable<Vastwebmulti.Models.Whatsapp_userWise> userwise { get; set; }
           public IEnumerable<Vastwebmulti.Models.Whatsapp_purchase> Report { get; set; }
    }
}