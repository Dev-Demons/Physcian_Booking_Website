using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Doctyme.Model.Attributes;

namespace Doctyme.Model.ViewModels
{
   public class BlogViewModel
    {
        public int ArticleId { get; set; }
        public string ArticleName { get; set;}
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int SubCategoryId { get; set; }
        public string SubCategoryName { get; set;}
        public string CategoryDesc { get; set; }
        public string ParentDesc { get; set; }
        public int NextArticleId { get; set; }
        public int PrevArticleId { get; set;}
        public string NextArticleName { get; set;}
        public string PrevArticleName { get; set;}
        public string SubCategoryNameReplace { get; set; }
        public string CategoryNameReplace { get; set; }
        public List<BlogItem> lstbundles { get; set; }
        public BlogItem blogItem { get; set; }
        public List<BlogItem> RelatedTopArticles { get; set; }
        public List<BlogItem> OtherRelatedArticles { get; set; }
        public List<Category> lstCategories { get; set; }
    }
    public class BlogItem
    {
        public int ArticleId { get; set; }
        [DataType(DataType.MultilineText)]
        [Required(ErrorMessage = "ArticleName is required.")]
        public string ArticleName { get; set; }
        public DateTime CreateDate { get; set; }
        public string Url { get; set; }

        
        public bool MainSite { get; set; }

        [Required(ErrorMessage = "Content is required.")]
        public string Content { get; set; }
        [Required(ErrorMessage = "Article Image is required.")]
        public string ImagePath { get; set; }

        [Url(ErrorMessage = "Invalid url")]
        public string FaceBookLink { get; set; }

        [Url(ErrorMessage = "Invalid url")]
        public string TwitterLink { get; set; }

        [Url(ErrorMessage = "Invalid url")]
        public string GooglePlusLink { get; set; }

        [Url(ErrorMessage = "Invalid url")]
        public string Plink { get; set; }

        [Url(ErrorMessage = "Invalid url")]
        public string LinkedInLink { get; set; }

        

        

        [Url(ErrorMessage ="Invalid url")]
        public string YoutubeLink { get; set; }

        [Required(ErrorMessage = "Select category")]
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public long Rno { get; set; }
        public string Type { get; set; }
        public string DateString { get; set; }
        public string SubCategoryName { get; set; }

        public int SubCategoryId { get; set; }
        public string SubCategoryIdstring { get; set; }
        [DataType(DataType.MultilineText)]
        [Required(ErrorMessage = "Article summary required."), StringLength(300)]
        public string ArticleSummary { get; set; }
        public string SubCategoryNameReplace { get; set; }
        public string CategoryNameReplace { get; set; }
        public string ArticleNameReplace { get; set; }

        [MaxFileSize(5 * 1024 * 1024, ErrorMessage = "Maximum allowed file size is 5MB")]
        public HttpPostedFileBase files { get; set; }
        public bool IsActive { get; set; }
        public bool Deleted { get; set; }
        
        public string CreatedBy { get; set; }     
        public string BlogUrl
        {
            get;set;
        }
        [Url(ErrorMessage = "Invalid url")]
        public string InstagramLink { get; set; }
        public string Keywords { get; set; }
        public string FileName { get; set; }
    }
    public class BlogItemSiteMap
    {
        public int ArticleId { get; set; }
        public string ArticleName { get; set; }
        public string ArticleNameReplace { get; set; }
        
    }
    public class DrugNameSiteMap
    {
       
        public int drugId { get; set; }
        public string drugname { get; set; }

    }
    public class Category
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int ParentCategoryId { get; set; }
        public string ParentCategoryName { get; set; }
        public string CategoryDesc { get; set; }
        public string ParentDesc { get; set; }
        public string CategoryNameReplace { get; set; }
        public string ParentCategoryNameReplace { get; set; }
    }
    public class BlogSearchResults
    {
        public List<BlogItem> _blogItems { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalRecordCount { get; set; }
    }

}
