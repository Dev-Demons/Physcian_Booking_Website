using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Binke.Models;
using Binke.ViewModels;
using Doctyme.Model.ViewModels;
using Doctyme.Repository.Interface;
using Microsoft.AspNet.Identity;

namespace Binke.Controllers
{
    public class TestimonialController : Controller
    {
        private readonly ITestimonialsService _testimonial;

        #region Constructor
        public TestimonialController(ITestimonialsService testimonial)
        {
            _testimonial = testimonial;
        }
        #endregion

        #region  Index 
        // GET: Testimonial
        public ActionResult Index()
        {
            return View();
        }
        #endregion


        #region Create Get
        [HttpGet]
        public ActionResult Create(int id = 0)
        {
            TestimonialItem model;

            if (id > 0)
            {
                try
                {
                    model = _testimonial.GetTestimonial(id);
                    if (model == null)
                    {
                        model = new TestimonialItem();
                        ModelState.AddModelError(string.Empty, "Testimonials not found with id " + id.ToString());
                    }
                }
                catch (Exception ex)
                {
                    model = new TestimonialItem();
                    ModelState.AddModelError(string.Empty, "Unexpected error");
                }
            }
            else
            {
                model = new TestimonialItem();
            }
            return View(model);
        }
        #endregion

        #region Create Post
        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Create(TestimonialItem model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    HttpPostedFileBase file = model.files;
                    var UserHostName = HttpContext.Request.Url;
                    var address = HttpContext.Request.UserHostAddress;
                    if (model.files != null && model.files.ContentLength > 0)
                    {
                        DirectoryInfo dir = new DirectoryInfo(Server.MapPath("~/Uploads/Testimonials/"));
                        if (!dir.Exists)
                        {
                            Directory.CreateDirectory(Server.MapPath("~/Uploads/Testimonials"));
                        }

                        string extension = Path.GetExtension(file.FileName);
                        string newImageName = "Testimonial-" + DateTime.Now.Ticks.ToString() + extension;

                        var path = Path.Combine(Server.MapPath("~/Uploads/Testimonials"), newImageName);
                        file.SaveAs(path);
                        model.ImagePath = UserHostName.AbsoluteUri.Replace(UserHostName.AbsolutePath, "/Uploads/Testimonials/");
                        model.ImagePath = Path.Combine(model.ImagePath, newImageName);
                    }
                    if (!string.IsNullOrEmpty(model.Content))
                    {
                        model.Content = System.Uri.UnescapeDataString(model.Content);
                    }

                    string userId = !string.IsNullOrEmpty(User.Identity.GetUserId()) ? User.Identity.GetUserId().ToString() : "0";
                    model.CreatedBy = Convert.ToInt32(userId);
                    int result = _testimonial.SaveTestimonial(model);
                    if (result == 1)
                    {
                        return RedirectToAction("Index", "Testimonial");
                        
                    }
                    else if (result==2)
                    {
                        ModelState.AddModelError(string.Empty, "Maximum allowed MainSite Testimonials are 3");
                        ModelState.AddModelError(string.Empty, "Already 3 MainSite Testimonials exists");
                        return View(model);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Failed to save testimonial");
                        return View(model);
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "Unexpected error");
                }
            }
            return View(model);
        }
        #endregion

        #region Index search
        [Route("GetTestimonialSearchList/{flag}")]
        public ActionResult GetTestmonialsList(bool flag, JQueryDataTableParamModel param)
        {
            var sortColumnIndex = param.iSortCol_0;// Convert.ToInt32(HttpContext.Request.QueryString["iSortCol_0"]);
            var sortDirection = HttpContext.Request.QueryString["sSortDir_0"]; // asc or desc
            var pageIndex = param.iDisplayStart / param.iDisplayLength;
            var model = new SearchParamModel();
            model.PageIndex = pageIndex;
            model.SearchBox = param.sSearch;
            model.PageSize = param.iDisplayLength;

            var result = GetTestimonialSearchResult(model);

            var allResults = result.TestimonialItems.AsEnumerable();

            if (sortColumnIndex == 1)
            {
                allResults = sortDirection == "asc" ? allResults.OrderBy(c=>c.Name).ToList() : allResults.OrderByDescending(c=>c.Name).ToList();
            }
            var total = result.TotalRecordCount;

            return Json(new
            {
                param.sEcho,
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                aaData = allResults
            }, JsonRequestBehavior.AllowGet);
        }

        private TestimonialSearchResults GetTestimonialSearchResult(SearchParamModel model)
        {
            try
            {
                model.PageSize = model.PageSize == 0 ? 10 : model.PageSize;
                //model.PageIndex = model.PageIndex == 0 ? 1 : model.PageIndex;
                //var pageIndex = model.PageIndex == 0 ? 0 : model.PageIndex;
                var pageIndex = model.PageIndex + 1;
                int totalRecord = 0;
                var searchResult = _testimonial.GetTestimonialsForIndex(model.SearchBox, pageIndex, model.PageSize, model.Sorting);
                totalRecord = searchResult.TotalRecordCount;

                ViewBag.SearchBox = model.SearchBox ?? "";

                searchResult.TotalRecordCount = totalRecord;
                searchResult.PageSize = model.PageSize;
                searchResult.PageIndex = model.PageIndex;

                return searchResult;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Update Active /  Delete status

        [HttpPost]
        public ActionResult UpdateSwitch(StatusUpdateModel item)
        {
            try
            {
                int result = _testimonial.TestimonialStatusUpdate(item);
                if (result == 1 && item.IsDeleted)
                    return Json(new JsonResponse { Status = 1, Message = "Record deleted successfully", Data = new { } }, JsonRequestBehavior.AllowGet);
                else if (result == 1 && !item.IsDeleted)
                    return Json(new JsonResponse { Status = 1, Message = "Stauts Updated Successfully", Data = new { } }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new JsonResponse { Status = 0, Message = "Update failed ", Data = new { } }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse { Status = 0, Message = "Error occured in updating ", Data = new { } }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
    }
}
