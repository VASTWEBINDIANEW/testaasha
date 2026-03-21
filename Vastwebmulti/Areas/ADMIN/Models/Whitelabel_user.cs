using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Vastwebmulti.Areas.ADMIN.Models
{

    /// <summary>
    /// Model representing a white-label partner user account and its configuration.
    /// </summary>
    public class Whitelabel_user
    {
        [Required(ErrorMessage = "Enter The Whitelabel Virtual Value")]
        public decimal Virtualvalue { set; get; }
        [Required(ErrorMessage = "Enter The Whitelabel Virtual Value")]
        public decimal Virtualvalue1 { set; get; }
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
            public int Pincode { get; set; }

            [Display(Name = "Enter Pincode Code")]
            [RegularExpression(@"^(\d{6})$", ErrorMessage = "Pincode should only be 6 digits")]
            public int Pincode1 { get; set; }

            [Required(ErrorMessage = "Enter Email Id")]
            [Display(Name = "email id")]
            [DataType(DataType.EmailAddress)]
            [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "Invalid Email ID Format")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Select Any Slab")]
            public string slab_name { get; set; }

            [Required(ErrorMessage = "Select Any Slab")]
            public string slab_name1 { get; set; }


            [Display(Name = "Enter Pancard Number")]
            [DataType(DataType.PhoneNumber)]
            [StringLength(11, MinimumLength = 10, ErrorMessage = "Pancard should only be 10 digits")]
        [RegularExpression("^([A-Za-z]){5}([0-9]){4}([A-Za-z]){1}$", ErrorMessage = "Invalid PAN Number")]
        public string pancard { get; set; }

            [Display(Name = "Enter Pancard Number")]
            [DataType(DataType.PhoneNumber)]
            [StringLength(11, MinimumLength = 10, ErrorMessage = "Pancard should only be 10 digits")]
        [RegularExpression("^([A-Za-z]){5}([0-9]){4}([A-Za-z]){1}$", ErrorMessage = "Invalid PAN Number")]
        public string pancard1 { get; set; }


            [Required(ErrorMessage = "Enter Aadhaar Number")]
            [Display(Name = "adharcard Number")]
            [DataType(DataType.PhoneNumber)]
            [RegularExpression(@"^(\d{12})$", ErrorMessage = "Aadhaar Card should only be 12 digits")]
            public string adharcard { get; set; }

            [Required(ErrorMessage = "Enter Aadhaar Number")]
            [Display(Name = "adharcard Number")]
            [DataType(DataType.PhoneNumber)]
            [RegularExpression(@"^(\d{12})$", ErrorMessage = "Aadhaar Card should only be 12 digits")]
            public string adharcard1 { get; set; }


            [Display(Name = "Enter Gst Number")]
            [StringLength(16, MinimumLength = 15, ErrorMessage = "GST No should only be 15 character and digits")]
            public string gst { get; set; }

             [Display(Name = "Enter Gst Number")]
             [StringLength(16, MinimumLength = 15, ErrorMessage = "GST No should only be 15 character and digits")]
            // [RegularExpression(@"^(\d{15})$", ErrorMessage = "GST No should only be 15 character and digits")]
             public string gst1 { get; set; }
             [Required(ErrorMessage = "Enter Website Name")]
     
               public string websitename { get; set; }
               [Required(ErrorMessage = "Enter Website Name")]
              public string websitename1 { get; set; }

        public IEnumerable<Vastwebmulti.Models.WhiteLabel_Select_Details_Result> White_labelUser { set; get; }
        }
    }