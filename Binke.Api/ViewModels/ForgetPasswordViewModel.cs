using System.ComponentModel.DataAnnotations;

namespace Binke.Api.ViewModels
{
    public class ForgetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}