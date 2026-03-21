using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.FeeCollector.Models
{
    public class StdModel
    { 
        [Required(ErrorMessage = "* this field is required")]
        public string StdID { get; set; }

        [Required(ErrorMessage = "Enter The Name")]
        public string StdName { get; set; }

        [Required(ErrorMessage = "Enter The Name")]
        public string StdName1 { get; set; }


        [Required(ErrorMessage = "Enter The Firm Name")]
        public string FName { get; set; }

        [Required(ErrorMessage = "Enter The Firm Name")]
        public string FName1 { get; set; }


        [Required]
        public DateTime DOB { get; set; }
        [Required]
        public DateTime DOB1 { get; set; }
        [Required]
        public StudentsBoards Board { get; set; }
        [Required]
        public StudentsBoards Board1 { get; set; }
        [Required]
        public StudentClasses stdClass { get; set; }
        [Required]
        public StudentClasses stdClass1 { get; set; }
        [Required]
        public string Section { get; set; }
        [Required]
        public string Section1 { get; set; }
       
        [Required]
        public string AcadminSession { get; set; }
        [Required]
        public string AcadminSession1 { get; set; }

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
        public char Status { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal AnnualFee { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        public decimal AnnualFee1 { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal BusFee { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        public decimal BusFee1 { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        public decimal Discount { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        public decimal Discount1 { get; set; }
        [Display(Name ="Fee Discount")]
        public bool FeeDiscountEnabled { get; set; }
        [Display(Name = "Bus Discount")]
        public bool BusDiscountEnabled { get; set; }

        public string remark { get; set; }
        public string remark1 { get; set; }

        public IEnumerable<Vastwebmulti.Models.Student> students { set; get; }

    }
    public enum StudentClasses
    {
        I = 1,
        II = 2,
        III = 3,
        IV = 4,
        V = 5,
        VI=6,
        VII= 7,
        VIII = 8,
        IX = 9,
        X =10,
        XI =11,
        XII = 12

    }
    public enum StudentsBoards
    { 
        CBSE = 1,
        RBSE_BSER = 2,
        ICSE = 3,
        ISC=4,
        CVE=5,
        IB=6
    }
}