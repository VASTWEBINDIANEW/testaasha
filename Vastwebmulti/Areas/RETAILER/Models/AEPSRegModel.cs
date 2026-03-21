using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Vastwebmulti.Areas.RETAILER.Models
{
    public class AEPSRegModel
    {
        [Required]
        public string RetailerId { get; set; }
        [Required]
        public string RetailerName { get; set; }
        [Required(ErrorMessage ="State is required")]
        public int State { get; set; }
        [Required]
        public string Mobile { get; set; }
        [Required(ErrorMessage ="Address is required")]
        public string Address { get; set; }
        [Required(ErrorMessage ="Email is reuired")]
        public string Email { get; set; }
        public string Status { get; set; }
        [Required(ErrorMessage ="Aadhaar Card is required")]
        public string AadharCard { get; set; }
        [Required]
        public string PanCard { get; set; }
        [Required(ErrorMessage ="Password is required")]
        public string Pwd{ get; set; }
        [Required]
        [Compare("Pwd",ErrorMessage ="Pasword miss match")]
        public string Pwdconfirm{ get; set; }
        public string UserType{ get; set; }
        public string LogoPath{ get; set; }
        [Required(ErrorMessage =" EKYC doc is required")]
        public string kycPath{ get; set; }
        public string canceledPath{ get; set; }
        public string shopPANPath{ get; set; }

    }
}