using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.WRetailer.Models
{
    public class RegisterPSAModel
    {
        [Required]
        public string psaname { get; set; }
        [Required]
        public string contactperson { get; set; }
        [Required(ErrorMessage ="Complete Addres is Required")]
        
        public string location { get; set; }
        [Required]
        public string pincode { get; set; }
        [Required]
        public string state { get; set; }
        [Required]
        public string phone1 { get; set; }

        public string phone2 { get; set; }
        [Required]
        public string emailid { get; set; }
        [Required]
        public string pan { get; set; }
        [Required]
        public string dob { get; set; }
        [Required]
        public string adhaar { get; set; }
        public string udf1 { get; set; }
        public string udf2 { get; set; }
        public string udf3 { get; set; }
        public string udf4 { get; set; }
        public string udf5 { get; set; }
        public string psaid { get; set; }
    }
}