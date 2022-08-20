using Doctyme.Repository.Interface;
using BinkeBlog.Utility;
using Doctyme.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BinkeBlog.Controllers
{
    public class BlogController : Controller
    {
        private readonly IBlogService _blogService;
        public BlogController(IBlogService blogService)
        {
            _blogService = blogService;
        }

        // GET: Blog
        public ActionResult Index()
        {
            BlogItem _model = new BlogItem();
            _model.Type = "Details";

            var result = _blogService.GetQueryResult(_model);
            var lstArticle = Common.ConvertDataTable<BlogItem>(result.Tables[0]).ToList();

            Random rnd = new Random();
            BlogViewModel blogViewModel = new BlogViewModel();
            blogViewModel.lstbundles = lstArticle;
            var toparticles = lstArticle.OrderBy(doc => rnd.Next()).Take(6).ToList();
            var otherRelatedAritcles = (from x in lstArticle
                                        where !toparticles.Any(c => x.ArticleId == c.ArticleId)
                                        select x).OrderBy(doc => rnd.Next()).ToList();

            blogViewModel.RelatedTopArticles = toparticles;
            blogViewModel.OtherRelatedArticles = otherRelatedAritcles;
            return View(blogViewModel);
        }       
        [Route("Blog/BlogItem/{ArticleName}")]
        public ActionResult BlogItem(string ArticleName ="")
        {
            BlogItem _model = new BlogItem();
            //  _model.ArticleId = Id;
            _model.ArticleName = ArticleName;
            var result = _blogService.GetArticleId(_model);
            var blogItem = Common.ConvertDataTable<BlogItem>(result.Tables[1]).FirstOrDefault();
            var lstItems = Common.ConvertDataTable<BlogItem>(result.Tables[2]).ToList();
            BlogViewModel blogViewModel = new BlogViewModel();
            blogViewModel.blogItem = blogItem;
            blogViewModel.lstbundles = lstItems;
            string NextArticleId = result.Tables[0].Rows[0]["nxtArticleId"].ToString();
            string PrevArticleId = result.Tables[0].Rows[0]["prvArticleId"].ToString();
            string CategoryId = result.Tables[0].Rows[0]["nxtArticleId"].ToString();
            blogViewModel.NextArticleId = !string.IsNullOrEmpty(NextArticleId) ? Convert.ToInt32(NextArticleId) : 0;
            blogViewModel.PrevArticleId = !string.IsNullOrEmpty(PrevArticleId) ? Convert.ToInt32(PrevArticleId) : 0;
            blogViewModel.CategoryId = !string.IsNullOrEmpty(CategoryId) ? Convert.ToInt32(CategoryId) : 0;
            blogViewModel.NextArticleName = result.Tables[0].Rows[0]["nxtArticleName"].ToString();
            blogViewModel.PrevArticleName = result.Tables[0].Rows[0]["prvArticleName"].ToString();
            Random rnd = new Random();
            //blogViewModel.RelatedTopArticles = 
            //    lstItems.Where(i => i.CategoryId == blogViewModel.CategoryId).OrderBy(i => rnd.Next()).Take(3).ToList();

            List<BlogItem> RelatedTopArticles = new List<BlogItem>();
            RelatedTopArticles = lstItems.Where(i => i.CategoryId == blogViewModel.CategoryId).OrderBy(i => rnd.Next()).Take(3).ToList();
            if (RelatedTopArticles.Count <= 0)
            {
                RelatedTopArticles = lstItems.OrderBy(i => rnd.Next()).Take(3).ToList();
            }
            blogViewModel.RelatedTopArticles = RelatedTopArticles;
            List<BlogItem> otherRelatedAritcles = new List<BlogItem>();
            otherRelatedAritcles = (from x in lstItems
                                    where !blogViewModel.RelatedTopArticles.Any(c => x.ArticleId == c.ArticleId) && x.CategoryId == blogViewModel.CategoryId
                                    select x).OrderBy(doc => rnd.Next()).Take(3).ToList();
            if (otherRelatedAritcles.Count <= 0)
            {
                otherRelatedAritcles = (from x in lstItems
                                        where !blogViewModel.RelatedTopArticles.Any(c => x.ArticleId == c.ArticleId) //&& x.CategoryId == blogViewModel.CategoryId
                                        select x).OrderBy(doc => rnd.Next()).Take(3).ToList();
            }

            blogViewModel.OtherRelatedArticles = otherRelatedAritcles;
            return View(blogViewModel);

        }
        [Route("Blog/Category/{CategoryName}")]
        public ActionResult Category(string CategoryName)
        {
            BlogItem _model = new BlogItem();
            _model.Type = "Category";
            //_model.CategoryId = CategoryId;
            _model.CategoryName = CategoryName;
            var result = _blogService.GetQueryResult(_model);
            var lstArticle = Common.ConvertDataTable<BlogItem>(result.Tables[0]).ToList();
            var selectedCategory = Common.ConvertDataTable<Category>(result.Tables[1]).FirstOrDefault();
            var lstCategory = Common.ConvertDataTable<Category>(result.Tables[2]).ToList();
            BlogViewModel blogViewModel = new BlogViewModel();
            blogViewModel.lstbundles = lstArticle;
            blogViewModel.lstCategories = lstCategory;
            blogViewModel.CategoryId = selectedCategory.CategoryId;
            blogViewModel.CategoryName = selectedCategory.CategoryName;
            blogViewModel.CategoryDesc = selectedCategory.CategoryDesc;
            return View(blogViewModel);
        }
        [Route("Blog/SubCategory/{SubCategoryName}")]
        public ActionResult SubCategory(string SubCategoryName)
        {
            BlogItem _model = new BlogItem();
            _model.Type = "SubCategory";
            //_model.SubCategoryId = SubCategoryId;
            _model.SubCategoryName = SubCategoryName;
            var result = _blogService.GetQueryResult(_model);
            var lstArticle = Common.ConvertDataTable<BlogItem>(result.Tables[0]).ToList();
            var selectedCategory = Common.ConvertDataTable<Category>(result.Tables[1]).FirstOrDefault();
            BlogViewModel blogViewModel = new BlogViewModel();
            blogViewModel.lstbundles = lstArticle;
            blogViewModel.SubCategoryId = selectedCategory.CategoryId;
            blogViewModel.SubCategoryName = selectedCategory.CategoryName;
            blogViewModel.CategoryDesc = selectedCategory.CategoryDesc;
            return View(blogViewModel);
        }
        public ActionResult GetMenuList()
        {
            var result = _blogService.GetCategories();
            var lstCategory = Common.ConvertDataTable<Category>(result).ToList();
            return PartialView("_MenuList", lstCategory);
        }
    }
}