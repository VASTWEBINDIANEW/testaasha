using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    public class SlabName
    {
        public int idno { set; get; }
        [Required(ErrorMessage = "Select User Type")]
        public string SlabFor { set; get; }
        [Required(ErrorMessage = "Enter Slab Name")]
        public string Slab_Name { set; get; }
    }
    public class ResultSetViewModel
    {
        public List<Slab_name_model> ResultSet { get; set; }
        public SlabName AccountVM { get; set; }
    }
    public partial class Slab_name_model
    {
        public int idno { get; set; }
        public string SlabFor { get; set; }
        public string SlabName { get; set; }
        public Nullable<System.DateTime> cdate { get; set; }
        public string createdby { get; set; }
    }

}