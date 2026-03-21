using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    public class Vandorlist
    {
        public string userid { get; set; }
        [Required(ErrorMessage = "Enter The Name")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Enter The Name")]
        public string Name1 { get; set; }
        [Required(ErrorMessage = "Enter The Firm Name")]
        public string FarmName { get; set; }
        [Required(ErrorMessage = "Enter The Firm Name")]
        public string FarmName1 { get; set; }
        [Required(ErrorMessage = "Select State")]
        public int State { get; set; }
        [Required(ErrorMessage = "Select State")]
        public int State1 { get; set; }

        [Required(ErrorMessage = "Select District")]
        public int District { get; set; }

        [Required(ErrorMessage = "Select District")]
        public int District1 { get; set; }
        [Required(ErrorMessage = "Enter The Address")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Enter The Address")]
        public string Address1 { get; set; }
        [Display(Name = "Enter Pincode Code")]
        [RegularExpression(@"^(\d{6})$", ErrorMessage = "Pincode should only be 6 digits")]
        public int Pincode { get; set; }

        [Display(Name = "Enter Pincode Code")]
        [RegularExpression(@"^(\d{6})$", ErrorMessage = "Pincode should only be 6 digits")]
        public int Pincode1 { get; set; }
        [Required(ErrorMessage = "Enter Email Id")]
        [Display(Name = "email id")]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "Invalid Email ID Format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Enter Email Id")]
        [Display(Name = "email id")]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "Invalid Email ID Format")]
        public string Email1 { get; set; }
        [Required(ErrorMessage = "Enter Mobile Number")]
        [Display(Name = "Home Phone")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid Phone number")]
        public string Mb { get; set; }

        [Required(ErrorMessage = "Enter Mobile Number")]
        [Display(Name = "Home Phone")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid Phone number")]
        public string Mb1 { get; set; }
       
        public string aadharnumber { get; set; }
        [Required(ErrorMessage = "Enter Aadhar Card Number")]
        public string aadharnumber1 { get; set; }
       
     
        public string pancard { get; set; }
        [Required(ErrorMessage = "Enter Pan  Card Number")]
        
        public string pancard1 { get; set; }
      
       
        public string gstnumber { get; set; }

        [Required(ErrorMessage = "Enter Gst Number")]
        public string gstnumber1 { get; set; }

        public IEnumerable<Vastwebmulti.Models.Vendor_Select_Details_Result> Vendor_list { set; get; }
    }


}