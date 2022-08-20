using Doctyme.Model.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Doctyme.Model.ViewModels
{
    public class TestimonialViewModel
    {
    }
    public class TestimonialItem
    {

        public int TestimonialID { get; set; }

        [Required(ErrorMessage = "Client Name is required.")]
        [MaxLength(255,ErrorMessage ="Maximum length is 255")]
        public string Name { get; set; }

        
        [MaxLength(255, ErrorMessage = "Maximum length is 255")]
        public string Title { get; set; }

        
        [MaxLength(255, ErrorMessage = "Maximum length is 255")]
        public string Organization { get; set; }

        [Required(ErrorMessage = "Content is required.")]
        [MaxLength(500, ErrorMessage = "Maximum length is 500")]
        public string Content { get; set; }
        
        [MaxLength(255, ErrorMessage = "Maximum length is 255")]
        public string ImagePath { get; set; }

        [Url(ErrorMessage = "Invalid url")]
        [MaxLength(255, ErrorMessage = "Maximum length is 255")]
        public string FaceBookLink { get; set; }

        [Url(ErrorMessage = "Invalid url")]
        [MaxLength(255, ErrorMessage = "Maximum length is 255")]
        public string TwitterLink { get; set; }

        [Url(ErrorMessage = "Invalid url")]
        [MaxLength(255, ErrorMessage = "Maximum length is 255")]
        public string GooglePlusLink { get; set; }

        [Url(ErrorMessage = "Invalid url")]
        [MaxLength(255, ErrorMessage = "Maximum length is 255")]
        public string Plink { get; set; }

        [Url(ErrorMessage = "Invalid url")]
        [MaxLength(255, ErrorMessage = "Maximum length is 255")]
        public string LinkedLink { get; set; }

        [Url(ErrorMessage = "Invalid url")]
        [MaxLength(255, ErrorMessage = "Maximum length is 255")]
        public string YoutubeLink { get; set; }

        [Url(ErrorMessage = "Invalid url")]
        [MaxLength(255, ErrorMessage = "Maximum length is 255")]
        public string InstagramLink { get; set; }
        

        [MaxFileSize(1 * 1024 * 1024, ErrorMessage = "Maximum allowed file size is 1 MB")]
        public HttpPostedFileBase files { get; set; }

        [MaxLength(255, ErrorMessage = "Maximum length is 255")]
        public string Keywords { get; set; }

        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime LastUpdate { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public bool MainSite { get; set; }
    }

    public class TestimonialSearchResults
    {
        public List<TestimonialItem> TestimonialItems { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalRecordCount { get; set; }
    }

    public class StatusUpdateModel
    {
        public int Id { get; set; }
        public bool Flag { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class TestimonialsForHome
    {
        public int TestimonialId { get; set; }
        public string ClientName { get; set; }
        public string Title { get; set; }
        public string ImagePath { get; set; }
        public string Content { get; set; }
    }
}
