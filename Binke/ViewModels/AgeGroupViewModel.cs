using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;


namespace Binke.ViewModels
{
    public class AgeGroupViewModel : BaseViewModel
    {
        public int AgeGroupId { get; set; }

        [Required(ErrorMessage = "Please enter AgeGroup Name.")]
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsViewMode { get; set; }
        public string DoctorName { get; set; }
        public int DoctorId { get; set; }
        public List<Doctyme.Model.AgeGroup> AgeGroupsList { get; set; }
    }
}
