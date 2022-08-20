using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Doctyme.Model.ViewModels;

namespace Binke.ViewModels
{
    public class HomeViewModel
    {
        public NetworkCount NetworkCount { get; set; }
        public List<FeaturedDoctorListModel> FeaturedDoctors { get; set; }
        public List<FeaturedSpecialityViewModel> FeaturedSpecialities { get; set; }
        public List<FeaturedFacilityListModel> Facilities { get; set; }
        public List<BlogItem> BlogItems { get; set; }
        public List<TestimonialsForHome> Testimonials { get; set; }

        public int DoctorCount { get; set; }
        public int PharmacyCount { get; set; }
        public int FacilityCount { get; set; }
        public int SeniorcareCount { get; set; }
        public IpInfo IpInfo { get; set; }

    }

    public class ContactUsViewModel : BaseViewModel
    {
        [Required(ErrorMessage = "Please Enter Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please Enter Subject")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "Please Enter Department")]
        public string Department { get; set; }

        [Required(ErrorMessage = "Please Enter Message")]
        [StringLength(1000, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 50)]
        public string Message { get; set; }

        [Required(ErrorMessage = "Please Enter Email Address")]
        [EmailAddress]
        public string Email { get; set; }

        public DateTime DateSubmit { get; set; }
    }
}
