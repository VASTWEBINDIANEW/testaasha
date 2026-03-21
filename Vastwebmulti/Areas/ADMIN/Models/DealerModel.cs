using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.ADMIN.Models
{

   public class DealerModel
    {
        public string Dealerid { get; set; }
        [Required(ErrorMessage = "Enter The Name")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Enter The Name")]
        public string Name1 { get; set; }
        [Required(ErrorMessage = "Enter The Firm Name")]
        [RegularExpression(@"([A-Za-z]| *)*", ErrorMessage = "Use Alphabets Only")]
        public string FarmName { get; set; }
        [Required(ErrorMessage = "Enter The Firm Name")]
        public string FarmName1 { get; set; }
        [Required(ErrorMessage = "Select State")]
        public string State { get; set; }
        [Required(ErrorMessage = "Select State")]
        public int  State1 { get; set; }

        [Required(ErrorMessage = "Select District")]
        public string  District { get; set; }

        [Required(ErrorMessage = "Select District")]
        public int  District1 { get; set; }


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


        [Required(ErrorMessage = "* this field is required")]
        public string Capping { get; set; }

        [Required(ErrorMessage = "* this field is required")]
        public string Capping1 { get; set; }


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

        [Required(ErrorMessage = "* this field is required")]
        public DateTime joindate { get; set; }

        [Required(ErrorMessage = "Select Any Slab")]
        public string slab_name { get; set; }

        [Required(ErrorMessage = "Select Any Slab")]
        public string slab_name1 { get; set; }


      
        public string pancard { get; set; }
        public string IsAgreement { get; set; }

      
        public string pancard1 { get; set; }


       
        public string adharcard { get; set; }
        [Required(ErrorMessage = "Fininacial Role")]
        public string ddlrole { get; set; }


        public string adharcard1 { get; set; }


      
       
        public string gst { get; set; }

       
        public string gst1 { get; set; }
        public IEnumerable<Select_Dealer_total_Result> Select_dealer_list { set; get; }
        public IEnumerable<Select_Permissionwise_Dealer_total_Result> Select_Permissionwise_Dealer_total { set; get; }
  
    }
    public partial class Select_Dealer_total_Result_model
    {
        public string MasterEmail { get; set; }
        public string DealerId { get; set; }
        public string DealerName { get; set; }
        public Nullable<System.DateTime> DOF { get; set; }
        public string FarmName { get; set; }
        public string Mobile { get; set; }
        public string Address { get; set; }
        public Nullable<int> Pincode { get; set; }
        public string Email { get; set; }
        public Nullable<System.DateTime> JoinDate { get; set; }
        public string Photo { get; set; }
        public string Status { get; set; }
        public Nullable<decimal> commission { get; set; }
        public string slab_name { get; set; }
        public string pancard { get; set; }
        public string adharcard { get; set; }
        public string gst { get; set; }
        public string Dist_Desc { get; set; }
        public string State_name { get; set; }
        public Nullable<decimal> remaindlm { get; set; }
        public Nullable<decimal> remainrem { get; set; }
    }
    public class RetailerDetalsModel
    {
        public string Retailerid { get; set; }


        [Required(ErrorMessage = "* this field is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "* this field is required")]
        public string Name1 { get; set; }
        [Required(ErrorMessage = "* this field is required")]
        public string retailename1 { get; set; }

        [Required(ErrorMessage = "* this field is required")]
        public int State { get; set; }

        [Required(ErrorMessage = "* this field is required")]
        public int State1 { get; set; }


        [Required(ErrorMessage = "* this field is required")]
        public string Firm { get; set; }

        [Required(ErrorMessage = "* this field is required")]
        public string Firm1 { get; set; }
        [Required(ErrorMessage = "* this field is required")]
        [RegularExpression(@"([A-Za-z]| *)*", ErrorMessage = "Use Alphabets Only")]

        public string firmnm1 { get; set; }
        //[Required(ErrorMessage = "* this field is required")]
        //[DataType(DataType.EmailAddress, ErrorMessage = "Invalid Email Address")]
        public string email1 { get; set; }

        [Required(ErrorMessage = "* this field is required")]
        public int District { get; set; }

        [Required(ErrorMessage = "* this field is required")]
        public int District1 { get; set; }
   

        [Required(ErrorMessage = "Enter Mobile Number")]
        [Display(Name = "Home Phone")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid Phone number")]

        public string Mobile { get; set; }

        [Required(ErrorMessage = "Enter Mobile Number")]
        [Display(Name = "Home Phone")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid Phone number")]

        public string Mobile1 { get; set; }


        [Required(ErrorMessage = "* this field is required")]
        public string Slab { get; set; }

        [Required(ErrorMessage = "* this field is required")]
        public string Slab1 { get; set; }


        [Required(ErrorMessage = "Enter Email Id")]
        [Display(Name = "email id")]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "Invalid Email ID Format")]

        public string Email { get; set; }

        [Required(ErrorMessage = "* this field is required")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Invalid Email Address")]
        public string Email1 { get; set; }

       
        public string Pan { get; set; }

       
        public string Pan1 { get; set; }

       
       
        [RegularExpression("^([A-Za-z]){5}([0-9]){4}([A-Za-z]){1}$", ErrorMessage = "Invalid PAN Number")]
       
        public string txtpencardno { get; set; }

       

      
        [MaxLength(12, ErrorMessage = "12 digit number Only")]
        public string Adhaar { get; set; }

       
        [MaxLength(12, ErrorMessage = "12 digit number Only")]
        public string Adhaar1 { get; set; }


        
        [MaxLength(15, ErrorMessage = "15 digit number Only")]
        public string Gst { get; set; }

       
        [MaxLength(15, ErrorMessage = "15 digit number Only")]
        public string Gst1 { get; set; }


        [Required(ErrorMessage = "* this field is required")]
        public string Address { get; set; }

        [Required(ErrorMessage = "* this field is required")]
        public string Address1 { get; set; }


        [Required(ErrorMessage = "* this field is required")]
        public string Capping { get; set; }

        [Required(ErrorMessage = "* this field is required")]
        public string Capping1 { get; set; }

        
        [Required(ErrorMessage = "* this field is required")]
        [Display(Name = "Enter Pincode Code")]
        [RegularExpression(@"^(\d{6})$", ErrorMessage = "Pincode should only be 6 digits")]
        [MaxLength(6, ErrorMessage = "6 digit number Only")]
        public string Pincode { get; set; }

        [Required(ErrorMessage = "* this field is required")]
        public string Pincode1 { get; set; }
        [Required(ErrorMessage = "* this field is required")]
        public string Password { get; set; }
        
        [MaxLength(4, ErrorMessage = "Maximum 4 digit ")]
        [MinLength(4, ErrorMessage = "Minimum 4 digit ")]
        public string Pin { get; set; }
        
        [MaxLength(12, ErrorMessage = "12 digit number Only")]
        public string txtadharcardno { get; set; }
       
       
        [Required(ErrorMessage = "* this field is required")]
        public string DealerID { get; set; }

        public List<SelectListItem> Dealerlist { get; set; }
        // public IEnumerable<SelectListItem> DealerID { get; set; }
        public IEnumerable<Select_Retailer_Details_all_Result> select_retailer_details { get; set; }

        public IEnumerable<Select_Retailer_Details_all_paging_Result> select_retailer_details_paging { get; set; }

        public IEnumerable<Vastwebmulti.Models.Dealer_retailer_Result> dealer_retailer { get; set; }
        public IEnumerable<Vastwebmulti.Models.Message_top> messagetop { get; set; }
        public IEnumerable<Service_BlockUserwisecls> Service_BlockUserwiseclslist { get; set; }
        public IEnumerable<Show_Service_name> Show_Service_namelist { get; set; }
    }
    public class Service_BlockUserwisecls
    {

        public int idno { get; set; }
        public string remid { get; set; }
        public int? serviceidno { get; set; }
        public string servicename { get; set; }
        public bool? servicestatus { get; set; }
        public int basedupdate_id { get; set; }

    }




}