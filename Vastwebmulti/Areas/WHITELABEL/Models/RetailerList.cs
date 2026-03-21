using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.WHITELABEL.Models
{
    public partial class Retailer_Details_all
    {
        [Required(ErrorMessage = "Select Any Dealer")]
        public string  Dealerid { get; set; }
        public string RetailerId { get; set; }
        [Required(ErrorMessage = "Enter The Name")]
        public string RetailerName { get; set; }
        [Required(ErrorMessage = "Enter The Name")]
        public string RetailerName1 { get; set; }
        [Required(ErrorMessage = "Select The State")]
        public int State { get; set; }
        [Required(ErrorMessage = "Select The State")]
        public int State1 { get; set; }
        [Required(ErrorMessage = "Select The District")]
        public int District { get; set; }
        [Required(ErrorMessage = "Select The District")]
        public int District1 { get; set; }
        [Required(ErrorMessage = "Enter The Mobile Number")]
        [Display(Name = "Home Phone")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid Phone number")]
        public string Mobile { get; set; }
        [Required(ErrorMessage = "Enter The Mobile Number")]
        [Display(Name = "Home Phone")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid Phone number")]
        public string Mobile1 { get; set; }
        [Required(ErrorMessage = "Enter The Address")]
        public string Address { get; set; }
        [Required(ErrorMessage = "Enter The Address")]
        public string Address1 { get; set; }
        [Required(ErrorMessage = "Enter The Pin Code")]
        [RegularExpression(@"^(\d{6})$", ErrorMessage = "Pincode should only be 6 digits")]
        public int Pincode { get; set; }
        [Required(ErrorMessage = "Enter The Pin Code")]
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
        [Required(ErrorMessage = "Enter Firm Name")]
        public string Frm_Name { get; set; }
        [Required(ErrorMessage = "Enter Firm Name")]
        public string Frm_Name1 { get; set; }
        [Required(ErrorMessage = "Select Slab Name")]
        public string slab_name { get; set; }
        [Required(ErrorMessage = "Select Slab Name")]
        public string slab_name1 { get; set; }
        [Required(ErrorMessage = "Enter Aadhaar Card")]
        public string AadharCard { get; set; }
        [Required(ErrorMessage = "Enter Aadhaar Card")]
        public string AadharCard1 { get; set; }
        [Required(ErrorMessage = "Enter Pan Card")]
        public string PanCard { get; set; }
        [Required(ErrorMessage = "Enter Pan Card")]
        public string PanCard1 { get; set; }
        [Required(ErrorMessage = "Enter Gst Number")]
        public string gst { get; set; }
        [Required(ErrorMessage = "Enter Gst Number")]
        public string gst1 { get; set; }
        [Required(ErrorMessage = "Enter Capping")]
        [Range(0, Int64.MaxValue, ErrorMessage = "Capping Should Be Numberic")]
        public int? capping { get; set; }
        [Required(ErrorMessage = "Enter Capping")]
        [Range(0, Int64.MaxValue, ErrorMessage = "Capping Should Be Numberic")]
        public int capping1 { get; set; }
        //public IEnumerable<Select_Retailer_Details_all_Result> Select_retailer_list { set; get; }
    }
    //public partial class Select_Retailer_Details_all_Result
    //{
    //    public string DealerId { get; set; }
    //    public string DealerEmail { get; set; }
    //    public string DealerName { get; set; }
    //    public string RetailerId { get; set; }
    //    public string RetailerName { get; set; }
    //    public string Mobile { get; set; }
    //    public string Address { get; set; }
    //    public int Pincode { get; set; }
    //    public string Email { get; set; }
    //    public System.DateTime JoinDate { get; set; }
    //    public string Photo { get; set; }
    //    public Nullable<int> caption { get; set; }
    //    public string gst { get; set; }
    //    public string AadharCard { get; set; }
    //    public string PanCard { get; set; }
    //    public string Status { get; set; }
    //    public string Frm_Name { get; set; }
    //    public decimal Remainamount { get; set; }
    //    public string Dist_Desc { get; set; }
    //    public string State_name { get; set; }
    //    public Nullable<bool> moneysts { get; set; }
    //    public string slab_name { get; set; }
    //    public bool EmailConfirmed { get; set; }
    //}
}
