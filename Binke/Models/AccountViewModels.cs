using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Binke.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Email")]
        [RegularExpression(@"^ *([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,6}) *$", ErrorMessage = "Enter valid Email Address")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
    public class RegisterJsonModel
    {
        public string RegisterViewModels { get; set; }
    }
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Please enter First name")]
        [MaxLength(50, ErrorMessage = "Please enter First name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter Last name")]
        [MaxLength(50, ErrorMessage = "Please enter Last name")]
        public string LastName { get; set; }

        [MaxLength(50, ErrorMessage = "Please enter Middle name")]
        public string MiddleName { get; set; }

        [Required(ErrorMessage = "Please enter email address")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please select grouptype.")]
        [Display(Name = "Group Type")]
        public string GroupTypeId { get; set; }

        [Required(ErrorMessage = "Please select facility type.")]
        public byte FacilityTypeId { get; set; }

        //[Required(ErrorMessage = "Please select facility name.")]
        public string FacilityName { get; set; }

        public string UserType { get; set; }

        [Required(ErrorMessage = "Please enter NPI")]
        [MinLength(10, ErrorMessage = "NPI should be 10 digit")]
        [StringLength(10)]
        [RegularExpression("([1-9][0-9]*)", ErrorMessage = "Accept only number")]
        public string Uniquekey { get; set; }

        [Required(ErrorMessage = "Please enter password")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,150}$",
         ErrorMessage = "Passwords must contain at least one lowercase letter, one uppercase letter, one number and one special character.")]
   
        [DataType(DataType.Password)]
        
        public string Password { get; set; }

        [Required(ErrorMessage = "Please enter confirm password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

    }

    public class Advertisements //Added by Reena
    {
        public int AdvertisementId { get; set; }
        public int ReferenceId { get; set; }
        public int PaymentTypeId { get; set; }
        public string Title { get; set; }
        public string ImagePath { get; set; }
        public string PaymentTypeName { get; set; }
        public string PaymentTypeDescription { get; set; }
    }
}
