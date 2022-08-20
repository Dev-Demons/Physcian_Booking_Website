
using System.Web;

namespace Binke.ViewModels
{
    public class SiteImageViewModel : BaseViewModel
    {
        public int SiteImageId { get; set; }
        public int? ReferenceId { get; set; }
        public int? UserTypeID { get; set; }
        public string ImagePath { get; set; }
        public HttpPostedFileBase ProfilePic { get; set; }
        public bool IsProfile { get; set; }
        public string Name{ get; set; }
    }     
}
