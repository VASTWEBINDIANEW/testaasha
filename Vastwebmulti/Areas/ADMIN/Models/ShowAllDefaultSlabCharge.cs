using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    public class ShowAllDefaultSlabCharge
    {
        public List<DefaultPrepaid> DefaultPrepaid { set; get; }
        public List<DefaultPostpaid> DefaultPostpaid { set; get; }
        public List<DefaultIMPS> DefaultIMPS { set; get; }
    }
    public class DefaultPrepaid
    {
        public int idno { get; set; }
        public string ddltype { set; get; }

        [Required(ErrorMessage = "Enter Commission")]
        public decimal? commission { get; set; }
        public string OperatorName { get; set; }
    }
    public class DefaultPostpaid
    {
        public int idno { get; set; }
        public string ddltype { set; get; }

        [Required(ErrorMessage = "Enter Commission")]
        public decimal? commission1 { get; set; }
        [Required(ErrorMessage = "Enter Commission")]
        public decimal? commission2 { get; set; }
        [Required(ErrorMessage = "Enter Commission")]
        public decimal? commission3 { get; set; }
        public string OperatorName { get; set; }
    }
    public class DefaultIMPS
    {
        public int idno { get; set; }
        public string ddltype { set; get; }
        public decimal? minimum { set; get; }
        public decimal? maximum { set; get; }
        [Required(ErrorMessage = "Enter Commission")]
        public string commission { get; set; }

        public string OperatorName { get; set; }
    }
}