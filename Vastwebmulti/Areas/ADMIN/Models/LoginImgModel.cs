using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    /// <summary>
    /// View model for managing login page image uploads and settings.
    /// </summary>
    public class LoginImgModel
    {
        public IList<tblWhiteLabelLoginBackImage> LoginImages { get; set; }
    }
}