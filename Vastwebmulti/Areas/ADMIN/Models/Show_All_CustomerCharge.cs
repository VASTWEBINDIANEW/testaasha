using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    /// <summary>
    /// View model for displaying all customer-facing charge configurations.
    /// </summary>
    public class Show_All_CustomerCharge
    {
        public List<Prepaid> Prepaid { set; get; }
        public List<Postpaid> Postpaid { set; get; }
        public List<IMPS> IMPS { set; get; }
    }
    public class Prepaid
    {
        public int idno { get; set; }
        public string ddltype { set; get; }

        [Required(ErrorMessage = "Enter Commission")]
        public decimal? commission { get; set; }
        public string OperatorName { get; set; }
    }
    public class Postpaid
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
    public class IMPS
    {
        public int idno { get; set; }
        public string ddltype { set; get; }
        public decimal? minimum { set; get; }
        public decimal? maximum { set; get; }
        [Required(ErrorMessage = "Enter Commission")]
        public decimal? commission { get; set; }

        public string OperatorName { get; set; }
    }
}