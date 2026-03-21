using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Drawing.Imaging;
using System.Web.Mvc;
using Vastwebmulti.Validators;
using System.ComponentModel.DataAnnotations.Schema;
using Vastwebmulti.Areas.RETAILER.Models;

namespace Vastwebmulti.Areas.WRetailer.Models
{
    public class PANCARD_Model
    {
        public int Idno { get; set; }

        public string RetailerId { get; set; }
        [Required(ErrorMessage = "Category is required")]
        public string Category { get; set; }
        [Required(ErrorMessage = "Status is required")]
        public string Status { get; set; }
        [Required(ErrorMessage = "PAN is required")]
        public string PANNo { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }
        [Required(ErrorMessage = "Last Name is required")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Name on card  is required")]
        public string Name_of_Card { get; set; }

        //[Required(ErrorMessage = "First Name is required")]
        public string FFirstName { get; set; }


        public string FMiddleName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        public string FLastName { get; set; }

        public string MFirstName { get; set; }
        public string MMiddleName { get; set; }
        public string MLastName { get; set; }

        [Required(ErrorMessage = "DOB is required")]
        public string DOB { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        public string Gender { get; set; }


        public string ISDCode { get; set; }

        public string STDCode { get; set; }
        [Required(ErrorMessage = "Mobile/Telephone is required")]
        [RegularExpression(@"^[6-9]{1}[0-9]{9}$", ErrorMessage = "Only 10 digit valid mobile is required? ")]
        public string Mobile { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required(ErrorMessage = "Address is required")]
        public string communicationAddress { get; set; }

        // [Required(ErrorMessage = "Addresss is required")]
        public string RA_Address { get; set; }

        [Required(ErrorMessage = "State is required")]
        public string State { get; set; }

        [Required(ErrorMessage = "District is required")]
        public string District { get; set; }

        [Required(ErrorMessage = "Identity proof is required")]
        public string ProofIdentity { get; set; }

        [Required(ErrorMessage = "Address proof is required")]
        public string ProofAddress { get; set; }
        [Required(ErrorMessage = "DOB proof is required")]
        public string ProofDOB { get; set; }
        [Required]
        public decimal ProcessingFee { get; set; }

        [Required(ErrorMessage = "Adhaar is required")]
        public string NameOnAadhaar { get; set; }

        [Required(ErrorMessage = "Aadhaar Number is required")]

        public string AadhaarNo { get; set; }

    }
    public class PANCARD_Model_FINAL
    {
        public int Idno { get; set; }

        public string RetailerId { get; set; }
        [Required(ErrorMessage = "Category is required")]
        public string Category { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }
        [Required(ErrorMessage = "Last Name is required")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Name on card  is required")]
        public string Name_of_Card { get; set; }

        //[Required(ErrorMessage = "First Name is required")]
        public string FFirstName { get; set; }


        public string FMiddleName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        public string FLastName { get; set; }


        [Required(ErrorMessage = "DOB is required")]
        public string DOB { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        public string Gender { get; set; }
        public string ISDCode { get; set; }

        public string STDCode { get; set; }
        [Required(ErrorMessage = "Mobile/Telephone is required")]
        [RegularExpression(@"^[6-9]{1}[0-9]{9}$", ErrorMessage = "Only 10 digit valid mobile is required? ")]
        public string Mobile { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required(ErrorMessage = "Address is required")]
        public string communicationAddress { get; set; }

        // [Required(ErrorMessage = "Addresss is required")]
        public string RA_Address { get; set; }

        [Required(ErrorMessage = "State is required")]
        public int State { get; set; }

        [Required(ErrorMessage = "District is required")]
        public int District { get; set; }

        [Required(ErrorMessage = "Identity proof is required")]
        public string ProofIdentity { get; set; }

        [Required(ErrorMessage = "Address proof is required")]
        public string ProofAddress { get; set; }
        [Required(ErrorMessage = "DOB proof is required")]
        public string ProofDOB { get; set; }
        [Required]
        public decimal ProcessingFee { get; set; }

        [Required(ErrorMessage = "Adhaar is required")]
        public string NameOnAadhaar { get; set; }
        public string AckNo { get; set; }
        // [Required(ErrorMessage ="Approval Date is Required ?")]
        public DateTime? APRDate { get; set; }
        public string DefencePersonnel { get; set; }
        public string AreaCode { get; set; }
        public string AOType { get; set; }
        public string Rangecode { get; set; }
        public string AONO { get; set; }
        [Required(ErrorMessage = "Title is Required!")]
        public string Title { get; set; }
        public string IsFatherMother { get; set; }
        public string MFirstName { get; set; }
        public string MMiddleName { get; set; }
        public string MLastName { get; set; }
        public string FLATNO { get; set; }
        public string Village { get; set; }
        public string Post_Street_LANE { get; set; }
        public string Are_SUBDIV { get; set; }
        public string PINCODE { get; set; }
        public string SourceOFIncome { get; set; }
        public string IsSalariedEmployee { get; set; }
        public string OrganisationName { get; set; }
        public string IsBusinessProfession { get; set; }
        public string IsBusinessProfessionCode { get; set; }

        [Required(ErrorMessage = "Aadhaar No is required")]

        public string AadhaarNo { get; set; }

        public string AadhaarDOB { get; set; }
        public string GenderAadhaar { get; set; }
        public string IsSignaturePresence { get; set; }
        public string NameOfVerifier { get; set; }
        public string CapacityOfVerifier { get; set; }
        public string PlaceofVerification { get; set; }
        public string isKYCCompliant { get; set; }
        public string VerificationDate { get; set; }
        [MinimumFileSizeValidator(0.01)]
        [MaximumFileSizeValidator(1.2)]
        [ValidFileTypeValidator("jpg")]
        [Required(ErrorMessage = "Please upload form first page!")]
        public HttpPostedFileBase DocFirmtPage { get; set; }
        [MinimumFileSizeValidator(0.01)]
        [MaximumFileSizeValidator(1.0)]
        [ValidFileTypeValidator("pdf")]
        [Required(ErrorMessage = "Please upload complete documents pdf!")]
        public HttpPostedFileBase DocPDF { get; set; }

        public static implicit operator PANCARD_Model_FINAL(RETAILER.Models.PANCARD_Model_FINAL v)
        {
            throw new NotImplementedException();
        }
    }
}