using System.Web;

namespace Binke.ViewModels
{
    public class FeaturedDoctorViewModel : BaseViewModel
    {
        public int FeaturedDoctorId { get; set; }

        public int DoctorId { get; set; }

        public string DoctorName { get; set; }

        public HttpPostedFileBase ProfilePic { get; set; }

        public string ProfilePicture { get; set; }

        public string SpecialityName { get; set; }

        public int Reviews { get; set; }

        public SocialMediaViewModel SocialMedia { get; set; }

        public string NPI { get; set; }
    }
}
