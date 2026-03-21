using System.ComponentModel.DataAnnotations;
namespace Vastwebmulti.Models
{
    public class Validation
    {
        [Required(ErrorMessage = "Please Enter Address..")]
        public string address { set; get; }
        [Required(ErrorMessage = "Please Enter Phone No")]
        [RegularExpression(@"^(\d{10})$", ErrorMessage = "Please Enter Valid Mobile Number.")]
        public string phone { set; get; }
        [Required(ErrorMessage = "Please Enter Email-Id")]
        [RegularExpression(@"^\w +[\w -\.] *\@\w + ((-\w +)|(\w*))\.[a-z]{2,3}$", ErrorMessage = "Please Enter Valid Email-id.")]
        public string emailid { set; get; }

    }
}