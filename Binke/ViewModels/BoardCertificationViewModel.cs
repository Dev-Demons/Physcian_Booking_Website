using System.ComponentModel.DataAnnotations;

namespace Binke.ViewModels
{
    public class BoardCertificationViewModel : BaseViewModel
    {
        public short BoardCertificationId { get; set; }

        public short SpecialityId { get; set; }

        [Required(ErrorMessage = "Please enter board certification")]
        public string CertificationName { get; set; }

        public string SpecialityName { get; set; }
    }
}
