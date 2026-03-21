using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.WHITELABEL.Models
{
    public class whitelabeldealer
    {
        public string Dealerid { get; set; }
        [Required(ErrorMessage = "Enter The Name")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Enter The Name")]
        public string Name1 { get; set; }
        [Required(ErrorMessage = "Enter The Firm Name")]
        public string FarmName { get; set; }
        [Required(ErrorMessage = "Enter The Firm Name")]
        public string FarmName1 { get; set; }
        [Required(ErrorMessage = "Select State")]
        public string State { get; set; }
        [Required(ErrorMessage = "Select State")]
        public int State1 { get; set; }

        [Required(ErrorMessage = "Select District")]
        public string District { get; set; }

        [Required(ErrorMessage = "Select District")]
        public int District1 { get; set; }


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


        [Required(ErrorMessage = "Enter The Address")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Enter The Address")]
        public string Address1 { get; set; }


        [Display(Name = "Enter Pincode Code")]
        [RegularExpression(@"^(\d{6})$", ErrorMessage = "Pincode should only be 6 digits")]
        public int? Pincode { get; set; }
        [Display(Name = "Enter Pincode Code")]
        [RegularExpression(@"^(\d{6})$", ErrorMessage = "Pincode should only be 6 digits")]
        public int? Pincode1 { get; set; }


        [Required(ErrorMessage = "Enter Capping")]
        [Range(0, Int64.MaxValue, ErrorMessage = "Capping Should Be Numberic")]
        public decimal? capping { get; set; }
        [Required(ErrorMessage = "Enter Capping")]
        [Range(0, Int64.MaxValue, ErrorMessage = "Capping Should Be Numberic")]
        public int capping1 { get; set; }

        [Required(ErrorMessage = "Enter Email Id")]
        [Display(Name = "email id")]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "Invalid Email ID Format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "* this field is required")]
        public DateTime? joindate { get; set; }

        [Required(ErrorMessage = "Select Any Slab")]
        public string slab_name { get; set; }

        [Required(ErrorMessage = "Select Any Slab")]
        public string slab_name1 { get; set; }


        [Required(ErrorMessage = "Enter Pancard Number")]
        public string pancard { get; set; }

        [Required(ErrorMessage = "Enter Pancard Number")]
        public string pancard1 { get; set; }


        [Required(ErrorMessage = "Enter Aadhaar Number")]
        public string adharcard { get; set; }

        [Required(ErrorMessage = "Enter Aadhaar Number")]
        public string adharcard1 { get; set; }


        [Required(ErrorMessage = "Enter Gst Number")]

        public string gst { get; set; }

        [Required(ErrorMessage = "Enter Gst Number")]
        public string gst1 { get; set; }
        public string status { get; set; }
        public decimal? remaindlm { get; set; }
        public decimal? remainrem { get; set; }
        public Boolean Emailconfrimedsts { set; get; }
        public IEnumerable<whitelabeldealer> Select_dealer_list { set; get; }
    }

}