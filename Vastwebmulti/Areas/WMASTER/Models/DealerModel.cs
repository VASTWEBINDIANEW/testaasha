using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.WMASTER.Models
{
    public class WDealerModel
    {
        public string Dealerid { get; set; }

        [Required(ErrorMessage = "* this field is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "* this field is required")]
        public string Name1 { get; set; }

        [Required(ErrorMessage = "* this field is required")]
        public string State { get; set; }

        [Required(ErrorMessage = "* this field is required")]
        public string State1 { get; set; }


        [Required(ErrorMessage = "* this field is required")]
        public string Firm { get; set; }

        [Required(ErrorMessage = "* this field is required")]
        public string Firm1 { get; set; }


        [Required(ErrorMessage = "* this field is required")]
        public string District { get; set; }

        [Required(ErrorMessage = "* this field is required")]
        public string District1 { get; set; }

        [DataType(DataType.PhoneNumber, ErrorMessage = "Please enter valid integer Number")]
        public string Mobile { get; set; }

        [DataType(DataType.PhoneNumber, ErrorMessage = "Please enter valid integer Number")]
        public string Mobile1 { get; set; }


        [Required(ErrorMessage = "* this field is required")]
        public string Slab { get; set; }

        [Required(ErrorMessage = "* this field is required")]
        public string Slab1 { get; set; }


        [Required(ErrorMessage = "* this field is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "* this field is required")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Invalid Email Address")]
        public string Email1 { get; set; }

        //[Required(ErrorMessage = "* this field is required")]
        public string Pan { get; set; }

        [Required(ErrorMessage = "* this field is required")]
        public string Pan1 { get; set; }

        //[Required(ErrorMessage = "* this field is required")]
        //[MaxLength(12, ErrorMessage = "12 digit number Only")]
        public string Adhaar { get; set; }

        [Required(ErrorMessage = "* this field is required")]
        [MaxLength(12, ErrorMessage = "12 digit number Only")]
        public string Adhaar1 { get; set; }


        //[Required(ErrorMessage = "* this field is required")]
        //[MaxLength(15, ErrorMessage = "15 digit number Only")]
        public string Gst { get; set; }

        [Required(ErrorMessage = "* this field is required")]
        [MaxLength(15, ErrorMessage = "15 digit number Only")]
        public string Gst1 { get; set; }


        [Required(ErrorMessage = "* this field is required")]
        public string Address { get; set; }

        [Required(ErrorMessage = "* this field is required")]
        public string Address1 { get; set; }


        [Required(ErrorMessage = "* this field is required")]
        [MinLength(6, ErrorMessage = "6 digit number Only")]
        [MaxLength(6, ErrorMessage = "6 digit number Only")]
        public string Pincode { get; set; }

        [Required(ErrorMessage = "* this field is required")]
        [MinLength(6, ErrorMessage = "6 digit number Only")]
        [MaxLength(6, ErrorMessage = "6 digit number Only")]
        public string Pincode1 { get; set; }


        public IEnumerable<Vastwebmulti.Models.SpWhitelabel_Select_Dealer_total_Result> SpWhitelabel_Select_Dealer_total_Result { set; get; }
        public IEnumerable<Vastwebmulti.Models.Message_top> messagetop { get; set; }
    }
}