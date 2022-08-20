using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Binke.ViewModels
{

    public class InsuranceAcceptedViewModel : BaseViewModel
    {
        public int InsuranceAcceptedId { get; set; }
        [Required(ErrorMessage = "Please enter Insurance Name.")]
        public string Name { get; set; }

        public bool IsEnable { get; set; }
    }
}
