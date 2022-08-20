using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Binke.Models;
//using Binke.Repository.Interface;
using Binke.Utility;
using Binke.ViewModels;
using Doctyme.Model.ViewModels;
using Doctyme.Repository.Enumerable;
using Doctyme.Repository.Interface;
using Microsoft.AspNet.Identity;

namespace Binke.Controllers
{
    public class BlogController : Controller
    {
        private readonly IBlogService _blogService;
        private readonly IDoctorService _doctor;

        public BlogController(IDoctorService doctor, IBlogService blogService)
        {
            _doctor = doctor;
            _blogService = blogService;
        }

        // GET: Blog
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }
        [Authorize]
        public ActionResult Create(int Id = 0)
        {
            try
            {

                BlogItem model = new BlogItem();
                ViewBag.Categories = GetCategories().Where(i => i.ParentCategoryId == 31).Select(i => new SelectListItem
                {
                    Value = i.CategoryId.ToString(),
                    Text = i.CategoryName
                }).ToList();
                if (Id > 0)
                {
                    DataSet ds = _doctor.GetQueryResult(StoredProcedureList.GetBlogById + " " + Id + "");
                    var result = Common.ConvertDataTable<Doctyme.Model.ViewModels.BlogItem>(ds.Tables[0]).FirstOrDefault();
                    if (result.CategoryId == 31)//
                    {
                        result.CategoryId = result.SubCategoryId;
                    }

                    ViewBag.SubCategories = GetCategories().Where(i => i.ParentCategoryId == result.CategoryId && i.ParentCategoryId != 31).Select(i => new SelectListItem
                    {
                        Value = i.CategoryId.ToString(),
                        Text = i.CategoryName
                    }).ToList();
                    result.FileName = Path.GetFileName(result.ImagePath);
                    result.SubCategoryIdstring = result.SubCategoryId > 0 ? result.SubCategoryId.ToString() : "";
                    return View(result);

                }
                else
                {
                    model.SubCategoryId = 0;
                    ViewBag.SubCategories = new List<SelectListItem>();
                }


                return View(model);
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "Blog/Create-GET");
                return View(new BlogItem());
            }
        }

        [Authorize]
        [HttpPost, Route("SaveBlogitems")]
        public JsonResult SaveBLogs(BlogItem model, HttpPostedFileBase Image1)
        {
            try
            {
                string imagePath = "";

                if (Image1 != null && Image1.ContentLength > 0)
                {
                    DirectoryInfo dir = new DirectoryInfo(Server.MapPath("~/Uploads/FacilitySiteImages/"));
                    if (!dir.Exists)
                    {
                        Directory.CreateDirectory(Server.MapPath("~/Uploads/FacilitySiteImages"));
                    }

                    string extension = Path.GetExtension(Image1.FileName);
                    string newImageName = "Blog-" + DateTime.Now.Ticks.ToString() + extension;

                    var path = Path.Combine(Server.MapPath("~/Uploads/FacilitySiteImages"), newImageName);
                    Image1.SaveAs(path);

                    imagePath = newImageName;
                }



                return Json(new JsonResponse() { Status = 0, Message = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "Blog/SaveBlogs-GET");
                return Json(new JsonResponse() { Status = 0, Message = "Image information uploading Error.." });
            }
        }

        [Authorize]
        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Create(BlogItem model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    HttpPostedFileBase file = model.files;
                    var UserHostName = HttpContext.Request.Url;
                    var address = HttpContext.Request.UserHostAddress;
                    if (model.files != null && model.files.ContentLength > 0)
                    {
                        DirectoryInfo dir = new DirectoryInfo(Server.MapPath("~/Uploads/Blog/"));
                        if (!dir.Exists)
                        {
                            Directory.CreateDirectory(Server.MapPath("~/Uploads/Blog"));
                        }

                        string extension = Path.GetExtension(file.FileName);
                        string newImageName = "Blog-" + DateTime.Now.Ticks.ToString() + extension;

                        var path = Path.Combine(Server.MapPath("~/Uploads/Blog"), newImageName);
                        file.SaveAs(path);
                        if (UserHostName.DnsSafeHost == "localhost")
                        {
                            model.ImagePath = HttpContext.Request.PhysicalApplicationPath.ToString().Replace("\\", "//") + "" + "Uploads/Blog/";
                        }
                        else
                        {
                            model.ImagePath = UserHostName.AbsoluteUri.Replace(UserHostName.AbsolutePath, "/Uploads/Blog/");
                        }
                        model.ImagePath = Path.Combine(model.ImagePath, newImageName);
                    }
                    if (!string.IsNullOrEmpty(model.Content))
                    {
                        model.Content = System.Uri.UnescapeDataString(model.Content);
                    }
                    if (model.SubCategoryIdstring != "-- Select SubCategory --" && !string.IsNullOrEmpty(model.SubCategoryIdstring) && Convert.ToInt32(model.SubCategoryIdstring) > 0)
                    {
                        model.CategoryId = Convert.ToInt32(model.SubCategoryIdstring);
                    }
                    string usrId = !string.IsNullOrEmpty(User.Identity.GetUserId()) ? User.Identity.GetUserId().ToString() : "0";
                    //DataSet ds = _doctor.GetQueryResult(StoredProcedureList.InsertBlogItem + " " + model.ArticleId + "," + "'" + model.CategoryId + "'" + " ," + "'" + model.ArticleName + "'" + ","
                    //                                              + "'" + model.ArticleSummary + "'" + "," + "'" + model.Content + "'" + "," + "'" + model.ImagePath + "'" +
                    //                                              "," + usrId + ",'" + model.YoutubeLink + "','" + model.FaceBookLink + "','" + model.TwitterLink + "','" +
                    //                                              model.GooglePlusLink + "','" + model.Plink + "','" + model.LinkedInLink + "','" +
                    //                                              model.InstagramLink + "','" + model.Keywords + "'," + model.MainSite);
                    var parameters = AddBlogParameters(model, usrId);
                    var ds = _doctor.GetDataSetList(StoredProcedureList.InsertBlogItem, parameters);
                    int result = Convert.ToInt32(ds.Tables[0].Rows[0][0].ToString());

                    if (result == 1)
                    {
                        return RedirectToAction("Index", "Blog");
                    }
                    else if (result == 2)
                    {
                        ModelState.AddModelError(string.Empty, "Maximum allowed MainSite blogs are 3");
                        ModelState.AddModelError(string.Empty, "Already 3 MainSite blogs exists");

                    }
                    else if (result == 3)
                    {
                        ModelState.AddModelError(string.Empty, "Name alredy exists");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Un expected error");
                    }
                }
                ViewBag.Categories = GetCategories().Where(i => i.ParentCategoryId == 31).Select(i => new SelectListItem
                {
                    Value = i.CategoryId.ToString(),
                    Text = i.CategoryName
                }).ToList();

                ViewBag.SubCategories = GetCategories().Where(i => i.ParentCategoryId == model.CategoryId).Select(i => new SelectListItem
                {
                    Value = i.CategoryId.ToString(),
                    Text = i.CategoryName
                }).ToList();

                return View(model);
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "Blog/Create-POST");
                return View(model);
            }
        }
        private List<SqlParameter> AddBlogParameters(BlogItem model, string usrId)
        {
            var parameters = new List<SqlParameter>()
            {
                new SqlParameter("@ArticleId", SqlDbType.Int)  {Value = model.ArticleId   },
                new SqlParameter("@CategoryId",SqlDbType.Int) {Value =  model.CategoryId },
                new SqlParameter("@ArticleName",SqlDbType.VarChar,-1) {Value =  model.ArticleName },
                new SqlParameter("@ArticleSummary",SqlDbType.VarChar,-1) {Value =   model.ArticleSummary},
                new SqlParameter("@Content",SqlDbType.NVarChar,-1) {Value =  model.Content},
                new SqlParameter("@ImagePath", SqlDbType.NVarChar,-1) {Value =  model.ImagePath},
                new SqlParameter("@UserID", SqlDbType.Int) {Value =Convert.ToInt32( usrId )},
                new SqlParameter("@YoutubeLink", SqlDbType.NVarChar,-1) {Value =  model.YoutubeLink},
                new SqlParameter("@FaceBookLink", SqlDbType.NVarChar,-1) {Value =  model.FaceBookLink},
                new SqlParameter("@TwitterLink",SqlDbType.NVarChar,-1) {Value =  model.TwitterLink},
                new SqlParameter("@GooglePlusLink",SqlDbType.NVarChar,-1) {Value =  model.GooglePlusLink},
                new SqlParameter("@Plink",SqlDbType.NVarChar,-1) {Value =  model.Plink},
                new SqlParameter("@LinkedInLink",SqlDbType.NVarChar,-1) {Value =  model.LinkedInLink},
                new SqlParameter("@InstagramLink",SqlDbType.NVarChar,-1) {Value =  model.InstagramLink},
                new SqlParameter("@KeyWords",SqlDbType.NVarChar,-1) {Value =  model.Keywords},
                new SqlParameter("@MainSite",SqlDbType.NVarChar,-1) {Value =  model.MainSite},
            };

            return parameters;
        }
        public List<Doctyme.Model.ViewModels.Category> GetCategories()
        {
            DataSet ds = _doctor.GetQueryResult(StoredProcedureList.GetCategorySubCategory);
            var result = Common.ConvertDataTable<Doctyme.Model.ViewModels.Category>(ds.Tables[0]).ToList();
            return result;
        }
        [HttpGet]
        public JsonResult GetSubCategory(int cId)
        {
            try
            {
                var lstSubCategories = GetCategories().Where(i => i.ParentCategoryId == cId).Select(i => new SelectListItem
                {
                    Value = i.CategoryId.ToString(),
                    Text = i.CategoryName
                }).ToList();
                return Json(new JsonResponse { Status = 0, Data = lstSubCategories }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "Blog/GetSubCategory-GET");
                return null;
            }
        }
        [HttpPost]
        public ActionResult UpdateSwitch(SwitchUpdateViewModel switchUpdateViewModel)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter(switchUpdateViewModel.FieldToUpdateName, SqlDbType.VarChar) { Value = switchUpdateViewModel.FieldToUpdateValue });
            try
            {
                _doctor.ExecuteSqlCommandForUpdate(switchUpdateViewModel.TableName, switchUpdateViewModel.PrimaryKeyName, switchUpdateViewModel.PrimaryKeyValue, parameters);
                if (switchUpdateViewModel.FieldToUpdateName.ToLower().Contains("deleted"))
                    return Json(new JsonResponse { Status = 1, Message = "Record deleted successfully", Data = new { } }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new JsonResponse { Status = 1, Message = switchUpdateViewModel.FieldToUpdateName.Replace("Is", "") + " Updated Successfully", Data = new { } }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "Blog/UpdateSwitch-POST");
                if (switchUpdateViewModel.FieldToUpdateName.ToLower().Contains("deleted"))
                    return Json(new JsonResponse { Status = 0, Message = "Error occured in deleting record", Data = new { } }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new JsonResponse { Status = 0, Message = "Error occured in updating " + switchUpdateViewModel.FieldToUpdateName.ToLower().Replace("is", ""), Data = new { } }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult UpdateMainSite(SwitchUpdateViewModel switchUpdateViewModel)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter(switchUpdateViewModel.FieldToUpdateName, SqlDbType.Bit) { Value = Convert.ToInt16(switchUpdateViewModel.FieldToUpdateValue) });
            try
            {
                var result = 1;
                if (Convert.ToBoolean(switchUpdateViewModel.FieldToUpdateValue))
                {
                    DataSet ds = _doctor.GetQueryResult(StoredProcedureList.GetIsMainSiteCount + " " + "");
                    result = (int)ds.Tables[0].Rows[0]["IsUpdateMainSite"];
                }
                if (result == 1)
                {
                    _doctor.ExecuteSqlCommandForUpdate(switchUpdateViewModel.TableName, switchUpdateViewModel.PrimaryKeyName, switchUpdateViewModel.PrimaryKeyValue, parameters);
                    if (switchUpdateViewModel.FieldToUpdateName.ToLower().Contains("MainSite"))
                        return Json(new JsonResponse { Status = 1, Message = "Record updated successfully", Data = new { } }, JsonRequestBehavior.AllowGet);
                    else
                        return Json(new JsonResponse { Status = 1, Message = switchUpdateViewModel.FieldToUpdateName.Replace("Is", "") + " Updated Successfully", Data = new { } }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new JsonResponse { Status = 0, Message = "Already 3 Main site is exist,please remove.", Data = new { } }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "Blog/UpdateMainSite-POST");
                if (switchUpdateViewModel.FieldToUpdateName.ToLower().Contains("deleted"))
                    return Json(new JsonResponse { Status = 0, Message = "Error occured in deleting record", Data = new { } }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new JsonResponse { Status = 0, Message = "Error occured in updating " + switchUpdateViewModel.FieldToUpdateName.ToLower().Replace("is", ""), Data = new { } }, JsonRequestBehavior.AllowGet);
            }
        }
        [Route("GetBlogSearchList/{flag}")]
        public ActionResult GetBLogList(bool flag, JQueryDataTableParamModel param)
        {
            try
            {
                var sortColumnIndex = param.iSortCol_0;// Convert.ToInt32(HttpContext.Request.QueryString["iSortCol_0"]);
                var sortDirection = HttpContext.Request.QueryString["sSortDir_0"]; // asc or desc
                var pageIndex = param.iDisplayStart / param.iDisplayLength;
                var model = new SearchParamModel();
                model.PageIndex = pageIndex;
                model.SearchBox = param.sSearch;
                model.PageSize = param.iDisplayLength;
                if (sortColumnIndex == 0)
                    model.Sorting = "DESC";

                var result = GetReviewSearchResult(model, 0);
                var allResults = result._blogItems.AsEnumerable();
                // var allFacilitys = searchResult.FacilityProviderList.AsEnumerable();
                if (sortColumnIndex == 1)
                {

                    allResults = sortDirection == "asc" ? allResults.OrderBy(c => c.ArticleName).ToList() : allResults.OrderByDescending(c => c.ArticleName).ToList();
                }

                //var display = allFacilitys.Skip(param.iDisplayStart).Take(param.iDisplayLength);

                var total = result.TotalRecordCount;

                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = total,
                    iTotalDisplayRecords = total,
                    aaData = allResults
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "Blog/GetBLogList-GET");
                return null;
            }
        }
        private BlogSearchResults GetReviewSearchResult(SearchParamModel model, int CategoryId)
        {
            try
            {
                model.PageSize = model.PageSize == 0 ? 10 : model.PageSize;
                // model.PageIndex = model.PageIndex == 0 ? 1 : model.PageIndex;
                // var pageIndex = model.PageIndex == 0 ? 0 : model.PageIndex;
                var pageIndex = model.PageIndex + 1;
                int totalRecord = 0;
                DataSet ds = _doctor.GetQueryResult(StoredProcedureList.GetAllBlogWithInActive + " " + "'" + model.SearchBox + "'" + "," + CategoryId + "," + pageIndex + "," + model.PageSize + "," + "'" + model.Sorting + "'");
                var result = Common.ConvertDataTable<Doctyme.Model.ViewModels.BlogItem>(ds.Tables[0]).ToList();
                //var lstblogs = _doctor.GetBlogsSearchResults(StoredProcedureList.GetAllBlog, parameters, out totalRecord).ToList();
                var searchResult = new BlogSearchResults() { _blogItems = result.Where(x => !x.Deleted).ToList() };

                var Result = new BlogSearchResults()
                {
                    _blogItems = searchResult._blogItems.Where(w => w.CategoryName == "Articles By Topic")
                                            .Select(s => { s.CategoryName = s.SubCategoryName; return s; }).ToList()
                };
                if (ds.Tables[0].Rows.Count > 0)
                {
                    totalRecord = !string.IsNullOrEmpty(ds.Tables[0].Rows[0]["TotalRecordCount"].ToString()) ? Convert.ToInt32(ds.Tables[0].Rows[0]["TotalRecordCount"].ToString()) : 0;
                }
                ViewBag.SearchBox = model.SearchBox ?? "";

                searchResult.TotalRecordCount = totalRecord;
                searchResult.PageSize = model.PageSize;
                searchResult.PageIndex = model.PageIndex;

                return searchResult;
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "Blog/GetReviewSearchResult");
                return null;
            }
        }

        [Route("Blog/BlogItem/{ArticleName}")]
        [Route("Blog/BlogItem")]
        public ActionResult BlogItem(string ArticleName)
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

        [Route("Blog/Category/{CategoryName?}")]
        [Route("Blog/Category")]
        public ActionResult Category(string CategoryName)
        {
            BlogItem _model = new BlogItem();
            _model.Type = "Category";
            ViewBag.Category = CategoryName ?? "Category";
            //_model.CategoryId = CategoryId;
            _model.CategoryName = CategoryName;
            var result = _blogService.GetQueryResult(_model);
            var lstArticle = Common.ConvertDataTable<BlogItem>(result.Tables[0]).ToList();
            var selectedCategory = Common.ConvertDataTable<Category>(result.Tables[1]).FirstOrDefault();
            var lstCategory = Common.ConvertDataTable<Category>(result.Tables[2]).ToList();
            BlogViewModel blogViewModel = new BlogViewModel();
            lstCategory.ToList().ForEach(c => c.CategoryNameReplace = !string.IsNullOrEmpty(c.CategoryNameReplace) ? c.CategoryName.Replace(' ', '-').Replace('&', '-') : "");
            blogViewModel.lstbundles = lstArticle;
            blogViewModel.lstCategories = lstCategory;
            blogViewModel.CategoryId = selectedCategory.CategoryId;
            blogViewModel.CategoryName = selectedCategory.CategoryName;
            blogViewModel.CategoryDesc = selectedCategory.CategoryDesc;
            blogViewModel.CategoryNameReplace = !string.IsNullOrEmpty(selectedCategory.CategoryName) ? selectedCategory.CategoryName.Replace(' ', '-').Replace('&', '-') : "";
            return View(blogViewModel);
        }
        [Route("Blog/SubCategory/{SubCategoryName?}")]
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

        [Route("Blogs")]
        public ActionResult AllBlogs()
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
    }
}
