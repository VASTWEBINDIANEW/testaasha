using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    public class MasterDistributerModel
    {

        [Required(ErrorMessage = "* this field is required")]
        public string SSIS { get; set; }
        [Required(ErrorMessage = "Enter The Name")]
        public string SuperstokistName { get; set; }

        [Required(ErrorMessage = "Enter The Name")]
        public string SuperstokistName1 { get; set; }


        [Required(ErrorMessage = "Enter The Firm Name")]
        [RegularExpression(@"([A-Za-z]| *)*", ErrorMessage = "Use Alphabets Only")]
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
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^(\d{6})$", ErrorMessage = "Pincode should only be 6 digits")]
        public int Pincode { get; set; }

        [Display(Name = "Enter Pincode Code")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^(\d{6})$", ErrorMessage = "Pincode should only be 6 digits")]
        public int Pincode1 { get; set; }


        [Required(ErrorMessage = "Enter Email Id")]
        [Display(Name = "email id")]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "Invalid Email ID Format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "* this field is required")]
        public DateTime joindate { get; set; }

        [Required(ErrorMessage = "Select Any Slab")]
        public string slab_name { get; set; }

        [Required(ErrorMessage = "Select Any Slab")]
        public string slab_name1 { get; set; }


        [Display(Name = "Enter Pancard Number")]
        [DataType(DataType.PhoneNumber)]
        [StringLength(11, MinimumLength = 10, ErrorMessage = "Pancard should only be 10 digits")]
        public string pancard { get; set; }

        [Display(Name = "Enter Pancard Number")]
        [DataType(DataType.PhoneNumber)]
        [StringLength(11, MinimumLength = 10, ErrorMessage = "Pancard should only be 10 digits")]
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


        public char Status { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        public IEnumerable<Vastwebmulti.Models.Select_super_total_Result> _Select_super_total_Result { set; get; }
        public IEnumerable<Vastwebmulti.Models.show_master_dealer_Result> show_master_dealer { set; get; }
        public IEnumerable<Vastwebmulti.Models.show_master_retailer_Result> show_master_retailer { set; get; }
        public IEnumerable<Vastwebmulti.Models.Select_Dealer_total_Result> Select_dealer_list { set; get; }
        public IEnumerable<Vastwebmulti.Models.Dealer_retailer_Result> Select_Retailer_list { set; get; }
        public IEnumerable<Vastwebmulti.Models.Select_super_Permissionwise_total_Result> Select_super_Permissionwise { set; get; }
        public List<Usernickname> Usernickname { get; set; }
        public List<tblMenu> tblMenu { get; set; }
        public Usernickname UsernicknameByID { get; set; }
        public List<lowbalance> lowbalance { get; set; }
        public lowbalance lowbalanceBYID { get; set; }
        public List<Formservice> Formservice { get; set; }
        public Formservice FormserviceByID { get; set; }
        public List<AddForm_information> AddForm_information { get; set; }
        public AddForm_information AddForm_informationbyid { get; set; }
    }
}