using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Vastwebmulti.Models
{
    public class IndexViewModel
    {
        public bool HasPassword { get; set; }
        public IList<UserLoginInfo> Logins { get; set; }
        public string PhoneNumber { get; set; }
        public bool TwoFactor { get; set; }
        public bool BrowserRemembered { get; set; }
    }

    public class ManageLoginsViewModel
    {
        public IList<UserLoginInfo> CurrentLogins { get; set; }
        public IList<AuthenticationDescription> OtherLogins { get; set; }
    }

    public class FactorViewModel
    {
        public string Purpose { get; set; }
    }

    public class SetPasswordViewModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ChangePinViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPin { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "New Pin")]
        public string NewPin { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new Pin")]
        [Compare("NewPin", ErrorMessage = "The new Pin and confirmation Pin do not match.")]
        public string ConfirmPin { get; set; }
    }
    public class ChangeTransactionViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current Transaction Password")]
        public string Oldtransactionpass { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New Transaction Password")]
        public string Newtransactionpass { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new Transaction Password")]
        [Compare("Newtransactionpass", ErrorMessage = "The new Transaction Password and confirmation Password do not match.")]
        public string Confirmtransactionpass { get; set; }
    }

    public class ChangeSecurityViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current Security Password")]
        public string OldSecuritypass { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New Security Password")]
        public string NewSecuritypass { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new Security Password")]
        [Compare("NewSecuritypass", ErrorMessage = "The new Security Password and confirmation Password do not match.")]
        public string ConfirmSecuritypass { get; set; }
    }

    public class AddPhoneNumberViewModel
    {
        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string Number { get; set; }
    }

    public class VerifyPhoneNumberViewModel
    {
        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
    }

    public class ConfigureTwoFactorViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
    }
}