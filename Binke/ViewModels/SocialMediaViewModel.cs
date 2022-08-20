namespace Binke.ViewModels
{
    public class SocialMediaViewModel : BaseViewModel
    {
        public int SocialMediaId { get; set; }

        public string Facebook { get; set; }

        public string Twitter { get; set; }

        public string LinkedIn { get; set; }

        public string Instagram { get; set; }

        public int? DoctorId { get; set; }

        public int? SeniorCareId { get; set; }
    }
}
