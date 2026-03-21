using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Vastwebmulti.Areas.DEALER.Models
{
    public class RetailerModel
    {
        public string Retailerid { get; set; }


        [Required(ErrorMessage = "* this field is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "* this field is required")]
        public string Name1 { get; set; }

        [Required(ErrorMessage = "* this field is required")]
        public int State { get; set; }

        [Required(ErrorMessage = "* this field is required")]
        public int State1 { get; set; }


        [Required(ErrorMessage = "* this field is required")]
        public string Firm { get; set; }

        [Required(ErrorMessage = "* this field is required")]
        public string Firm1 { get; set; }


        [Required(ErrorMessage = "* this field is required")]
        public int District { get; set; }

        [Required(ErrorMessage = "* this field is required")]
        public int District1 { get; set; }

        [DataType(DataType.PhoneNumber, ErrorMessage = "Please enter valid integer Number")]
        public string Mobile { get; set; }

        [DataType(DataType.PhoneNumber, ErrorMessage = "Please enter valid integer Number")]
        public string Mobile1 { get; set; }


        [Required(ErrorMessage = "* this field is required")]
        public string Slab { get; set; }

        [Required(ErrorMessage = "* this field is required")]
        public string Slab1 { get; set; }


        //[Required(ErrorMessage = "* this field is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "* this field is required")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Invalid Email Address")]
        public string Email1 { get; set; }


        public string Pan { get; set; }


        public string Pan1 { get; set; }


        [MaxLength(12, ErrorMessage = "12 digit number Only")]
        public string Adhaar { get; set; }


        [MaxLength(12, ErrorMessage = "12 digit number Only")]
        public string Adhaar1 { get; set; }



        [MaxLength(15, ErrorMessage = "15 digit number Only")]
        public string Gst { get; set; }


        [MaxLength(15, ErrorMessage = "15 digit number Only")]
        public string Gst1 { get; set; }

        public HttpPostedFileBase aadharcardfrontdoc { get; set; }
        public HttpPostedFileBase aadharcardbackdoc { get; set; }
        public HttpPostedFileBase pancarddoc { get; set; }
        public HttpPostedFileBase shopwithselfiledoc { get; set; }
        public HttpPostedFileBase KYCVIDEOfile { get; set; }

        [Required(ErrorMessage = "* this field is required")]
        public string Address { get; set; }

        [Required(ErrorMessage = "* this field is required")]
        public string Address1 { get; set; }


        [Required(ErrorMessage = "* this field is required")]
        public string Capping { get; set; }

        [Required(ErrorMessage = "* this field is required")]
        public string Capping1 { get; set; }


        [Required(ErrorMessage = "* this field is required")]
        public string Pincode { get; set; }

        [Required(ErrorMessage = "* this field is required")]
        public string Pincode1 { get; set; }
        [Required(ErrorMessage = "* this field is required")]
        public string Password { get; set; }
        [Required(ErrorMessage = "* this field is required")]
        [MaxLength(4, ErrorMessage = "Maximum 4 digit ")]
        [MinLength(4, ErrorMessage = "Minimum 4 digit ")]
        public string Pin { get; set; }
        public int Dealertoken { get; set; }


        public decimal? offmargineEditn { get; set; }




        public IEnumerable<Vastwebmulti.Models.Dealer_retailer_Result> dealer_retailer { get; set; }
        public IEnumerable<Vastwebmulti.Models.Message_top> messagetop { get; set; }
    }
}