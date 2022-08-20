using System.ComponentModel.DataAnnotations;

namespace Binke.ViewModels
{
    public class LanguageViewModel : BaseViewModel
    {
        public short LanguageId { get; set; }

        [Required(ErrorMessage = "Please enter language."),
         StringLength(50, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        public string LanguageName { get; set; }
        public string LanguageCode { get; set; }
    }
}
