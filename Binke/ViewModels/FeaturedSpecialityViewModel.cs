using System.Web;

namespace Binke.ViewModels
{
    public class FeaturedSpecialityViewModel : BaseViewModel
    {
        public int FeaturedSpecialityId { get; set; }

        public short SpecialityId { get; set; }

        public string SpecialityName { get; set; }

        public string Description { get; set; }

        public HttpPostedFileBase ProfilePic { get; set; }

        public string ProfilePicture { get; set; }
    }
}
