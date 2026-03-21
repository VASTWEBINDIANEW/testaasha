using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.WHITELABEL.Models
{
        public partial class bank_info_1
        {
         public int idno { get; set; }
        [Required(ErrorMessage = "Enter Bank Name")]
        public string banknm { get; set; }
        [Required(ErrorMessage = "Enter IFSC Code")]
        public string ifsccode { get; set; }
        [Required(ErrorMessage = "Enter Account Number")]
        public string acno { get; set; }
        [Required(ErrorMessage = "Enter Account Holder Name")]
        public string holdername { get; set; }
        [Required(ErrorMessage = "Enter Account Type")]
        public string actype { get; set; }
        [Required(ErrorMessage = "Enter Branch Address")]
        public string address { get; set; }
        [Required(ErrorMessage = "Enter Branch Name")]
        public string branch_nm { get; set; }
        public string userid { get; set; }
        public IEnumerable<bank_info_1> banklist { set; get; }

        [Required(ErrorMessage = "Enter Bank Name")]
        public string banknm1 { get; set; }
        [Required(ErrorMessage = "Enter IFSC Code")]
        public string ifsccode1 { get; set; }
        [Required(ErrorMessage = "Enter Account Number")]
        public string acno1 { get; set; }
        [Required(ErrorMessage = "Enter Account Holder Name")]
        public string holdername1 { get; set; }
        [Required(ErrorMessage = "Enter Account Type")]
        public string actype1 { get; set; }
        [Required(ErrorMessage = "Enter Branch Address")]
        public string address1 { get; set; }
        [Required(ErrorMessage = "Enter Branch Name")]
        public string branch_nm1 { get; set; }

    }
    }