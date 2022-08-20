using System;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;
using System.Web.Mvc;
using Binke.Models;
using Binke.Utility;
using Binke.ViewModels;
using Doctyme.Model;
using Doctyme.Repository.Interface;
using Doctyme.Repository.Services;
using Binke.App_Helpers;
using Doctyme.Model.ViewModels;
using System.IO;
using Doctyme.Repository.Enumerable;
using System.Data;
using System.Collections.Generic;
using Microsoft.AspNet.Identity;

namespace Binke.Controllers
{
    [Authorize]
    public class SiteImageController : Controller
    {
        private readonly IUserService _appUser;
        private readonly IRepository _repo;
        private readonly ISeniorCareService _seniorCare;

        public SiteImageController(IUserService appUser, RepositoryService repo, ISeniorCareService seniorCare)
        {
            _appUser = appUser;
            _repo = repo;
            _seniorCare = seniorCare;
        }

        #region Site Image

        public ActionResult Index()
        {
            return View();
        }

        [Route("GetSiteImageList/{flag}/{id?}")]
        public ActionResult GetSiteImageList(bool flag, JQueryDataTableParamModel param, int id = 0)
        {
            var sortColumnIndex = Convert.ToInt32(HttpContext.Request.QueryString["iSortCol_0"]);
            var sortDirection = HttpContext.Request.QueryString["sSortDir_0"]; // asc or desc
            var pageIndex = param.iDisplayStart / param.iDisplayLength;
            var model = new SearchParamModel();
            model.PageIndex = pageIndex;
            model.SearchBox = param.sSearch;
            model.PageSize = param.iDisplayLength;
            var searchResult = GetSiteImageSearchResult(model, id);
            var allSeniorCare = searchResult.SiteImageList.AsEnumerable();
            Func<Doctyme.Model.ViewModels.SiteImageViewModel, string> orderingFunction =
                c => sortColumnIndex == 1 ? c.ImagePath
                            //: sortColumnIndex == 3 ? c.Email
                            //    : sortColumnIndex == 4 ? c.
                            : c.ImagePath;
            allSeniorCare = sortDirection == "asc" ? allSeniorCare.OrderBy(orderingFunction).ToList() : allSeniorCare.OrderByDescending(orderingFunction).ToList();

            //var display = allSeniorCare.Skip(param.iDisplayStart).Take(param.iDisplayLength);

            var total = searchResult.TotalRecord;

            return Json(new
            {
                param.sEcho,
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                aaData = allSeniorCare
            }, JsonRequestBehavior.AllowGet);
        }

        [Route("_SiteImage/{id?}/{selectedid?}")]
        public PartialViewResult _SiteImage(int id = 0, int SelectedId = 0)
        {
            if (id == 0)
            {
                Binke.ViewModels.SiteImageViewModel _SiteImageViewModel = new Binke.ViewModels.SiteImageViewModel();
                _SiteImageViewModel.ReferenceId = SelectedId;
                return PartialView(@"/Views/SiteImage/Partial/_SiteImage.cshtml", _SiteImageViewModel);
            }
            var result = _repo.ExecWithStoreProcedure<Binke.ViewModels.SiteImageViewModel>("spSeniorCare_SiteImage_GetById @SiteImageId",
                   new SqlParameter("SiteImageId", System.Data.SqlDbType.Int) { Value = id }
                   ).Select(x => new Binke.ViewModels.SiteImageViewModel
                   {
                       SiteImageId = x.SiteImageId,
                       ReferenceId = x.ReferenceId,
                       ImagePath = x.ImagePath,
                       IsProfile = x.IsProfile,
                       IsActive = x.IsActive,
                       Name = x.Name,
                   }).First();
            //var SiteImage = _repo.Find<SiteImage>(id);
            //var result = new Binke.ViewModels.SiteImageViewModel()
            //{
            //    SiteImageId = SiteImage.SiteImageId,
            //    ReferenceId = SiteImage.ReferenceId,
            //    ImagePath = SiteImage.ImagePath,
            //    IsProfile = SiteImage.IsProfile,                                  
            //};
            if (result == null)
            {
                return PartialView(@"/Views/SiteImage/Partial/_SiteImage.cshtml", new Binke.ViewModels.SiteImageViewModel());
            }
            return PartialView(@"/Views/SiteImage/Partial/_SiteImage.cshtml", result);

        }

        [HttpPost, Route("AddEditSiteImage"), ValidateAntiForgeryToken]
        public JsonResult AddEditSiteImage(Binke.ViewModels.SiteImageViewModel model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                string filePath = string.Empty;
                try
                {
                    if (ModelState.IsValid)
                    {
                        if (model.SiteImageId == 0)
                        {
                            model.ProfilePic = Request.Files[0];
                            string extension = Path.GetExtension(model.ProfilePic.FileName);
                            model.ImagePath = $@"SeniorCare-{DateTime.Now.Ticks}{extension}";
                            filePath = Path.Combine(Server.MapPath(FilePathList.SeniorCare), model.ImagePath);
                            Common.CheckServerPath(Server.MapPath(FilePathList.SiteImage));
                            model.ProfilePic.SaveAs(filePath);

                            int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spSeniorCare_SiteImage_Create " +
                                "@SiteImageId,@ReferenceId,@ImagePath,@IsProfile,@UserTypeID,@CreatedDate,@UpdatedDate,@IsDeleted,@CreatedBy,@ModifiedBy,@IsActive,@Name",
                                new SqlParameter("SiteImageId", System.Data.SqlDbType.Int) { Value = model.SiteImageId },
                                new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = (object)model.ReferenceId ?? DBNull.Value },
                                new SqlParameter("ImagePath", System.Data.SqlDbType.VarChar) { Value = (object)model.ImagePath ?? DBNull.Value },
                                new SqlParameter("IsProfile", System.Data.SqlDbType.Bit) { Value = model.IsProfile },
                                new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = (int)UserTypes.SeniorCare },
                                new SqlParameter("CreatedDate", System.Data.SqlDbType.DateTime) { Value = DateTime.UtcNow },
                                new SqlParameter("UpdatedDate", System.Data.SqlDbType.DateTime) { Value = model.SiteImageId > 0 ? DateTime.UtcNow : (object)DBNull.Value },
                                new SqlParameter("IsDeleted", System.Data.SqlDbType.Bit) { Value = model.SiteImageId > 0 ? model.IsDeleted : false },
                                new SqlParameter("CreatedBy", System.Data.SqlDbType.Int) { Value = User.Identity.GetUserId<int>() },
                                new SqlParameter("ModifiedBy", System.Data.SqlDbType.Int) { Value = model.SiteImageId > 0 ? User.Identity.GetUserId<int>() : (object)DBNull.Value },
                                new SqlParameter("IsActive", System.Data.SqlDbType.Bit) { Value = true },
                                new SqlParameter("Name", System.Data.SqlDbType.VarChar) { Value = model.Name }
                                );
                                

                            //var SiteImage = new SiteImage()
                            //{
                            //    ReferenceId = model.ReferenceId,
                            //    ImagePath = model.ImagePath,
                            //    IsProfile = model.IsProfile,
                            //    UserTypeID = (int)UserTypes.SeniorCare,
                            //    CreatedDate = DateTime.Now,                                
                            //    IsDeleted = false,
                            //};
                            //_repo.Insert<SiteImage>(SiteImage, true);
                            //// _repo.sa();
                            txscope.Complete();
                            return Json(new JsonResponse() { Status = 1, Message = "SiteImage save successfully" });
                        }
                        else
                        {

                            //var SiteImage = _repo.Find<SiteImage>(model.SiteImageId);

                            if (Request.Files.Count > 0)
                            {
                                model.ProfilePic = Request.Files[0];
                                string extension = Path.GetExtension(model.ProfilePic.FileName);
                                model.ImagePath = $@"SeniorCare-{DateTime.Now.Ticks}{extension}";
                                filePath = Path.Combine(Server.MapPath(FilePathList.SeniorCare), model.ImagePath);
                                Common.CheckServerPath(Server.MapPath(FilePathList.SiteImage));
                                model.ProfilePic.SaveAs(filePath);
                                //SiteImage.ImagePath = model.ImagePath;
                            }

                            int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spSeniorCare_SiteImage_Create " +
                                "@SiteImageId,@ReferenceId,@ImagePath,@IsProfile,@UserTypeID,@CreatedDate,@UpdatedDate,@IsDeleted,@CreatedBy,@ModifiedBy,@IsActive,@Name",
                                new SqlParameter("SiteImageId", System.Data.SqlDbType.Int) { Value = model.SiteImageId },
                                new SqlParameter("ReferenceId", System.Data.SqlDbType.Int) { Value = (object)model.ReferenceId ?? DBNull.Value },
                                new SqlParameter("ImagePath", System.Data.SqlDbType.VarChar) { Value = (model.ImagePath == null ? "" : model.ImagePath) },
                                new SqlParameter("IsProfile", System.Data.SqlDbType.Bit) { Value = model.IsProfile },
                                new SqlParameter("UserTypeID", System.Data.SqlDbType.Int) { Value = (int)UserTypes.SeniorCare },
                                new SqlParameter("CreatedDate", System.Data.SqlDbType.DateTime) { Value = DateTime.UtcNow },
                                new SqlParameter("UpdatedDate", System.Data.SqlDbType.DateTime) { Value = model.SiteImageId > 0 ? DateTime.UtcNow : (object)DBNull.Value },
                                new SqlParameter("IsDeleted", System.Data.SqlDbType.Bit) { Value = model.SiteImageId > 0 ? model.IsDeleted : false },
                                new SqlParameter("CreatedBy", System.Data.SqlDbType.Int) { Value = User.Identity.GetUserId<int>() },
                                new SqlParameter("ModifiedBy", System.Data.SqlDbType.Int) { Value = model.SiteImageId > 0 ? User.Identity.GetUserId<int>() : (object)DBNull.Value },
                                new SqlParameter("IsActive", System.Data.SqlDbType.Bit) { Value = model.IsActive },
                                new SqlParameter("Name", System.Data.SqlDbType.VarChar) { Value = model.Name }
                                );

                            //SiteImage.IsProfile = model.IsProfile;
                            //SiteImage.UpdatedDate = DateTime.Now;
                            //_repo.Update<SiteImage>(SiteImage, true);
                            ////_user.SaveData();
                            txscope.Complete();
                            return Json(new JsonResponse { Status = 1, Message = "SiteImage updated successfully" });
                        }
                    }
                    else
                    {
                        txscope.Dispose();
                        return Json(new JsonResponse { Status = 4, Data = ModelState.Errors() });
                    }
                }
                catch (Exception ex)
                {

                    txscope.Dispose();
                    Common.LogError(ex, "AddEditDrugType-Post");
                    return Json(new JsonResponse { Status = 0, Message = "Something is wrong." });
                }
            }
        }


        [Route("RemoveSiteImage/{id?}")]
        [HttpPost]
        public JsonResult RemoveSiteImage(int id)
        {
            if (id > 0)
            {
                var result = _repo.ExecWithStoreProcedure<Binke.ViewModels.SiteImageViewModel>("spSeniorCare_SiteImage_GetById @SiteImageId",
                   new SqlParameter("SiteImageId", System.Data.SqlDbType.Int) { Value = id }
                   ).Select(x => new Binke.ViewModels.SiteImageViewModel
                   {
                       SiteImageId = x.SiteImageId,
                       ReferenceId = x.ReferenceId,
                       ImagePath = x.ImagePath,
                       IsProfile = x.IsProfile,
                       IsActive = x.IsActive,
                       Name = x.Name,
                   }).First();

                string path = Path.Combine(Server.MapPath(FilePathList.SeniorCare), result.ImagePath);
                FileInfo file = new FileInfo(path);
                if (file.Exists)//check file exsit or not  
                {
                    file.Delete();
                }

                    int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spSeniorCare_SiteImage_Delete  " +
                    "@SiteImageId ", new SqlParameter("SiteImageId", System.Data.SqlDbType.Int) { Value = id });
                return Json(new JsonResponse() { Status = 1, Message = "SiteImage deleted successfully" });
            }
            else
            {
                return Json(new JsonResponse() { Status = 0, Message = "Something is wrong" });
            }

            //var SiteImage = _repo.Find<SiteImage>(id);
            //_repo.Delete<SiteImage>(SiteImage);
            //return Json(new JsonResponse() { Status = 1, Message = "Facility Option deleted successfully" });
        }

        [Route("SiteImageStatusChange/{id?}/{flag?}/{isprofile?}")]
        [HttpPost]
        public JsonResult SiteImageStatusChange(int id,int flag=0, int IsProfile =0)
        {
            if (id > 0)
            {
                int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spSeniorCare_SiteImage_StatusChange  " +
                    "@SiteImageId,@Flag,@IsProfile", new SqlParameter("SiteImageId", System.Data.SqlDbType.Int) { Value = id }, new SqlParameter("Flag", System.Data.SqlDbType.Int) { Value = flag }, new SqlParameter("IsProfile", System.Data.SqlDbType.Int) { Value = IsProfile });
                return Json(new JsonResponse() { Status = 1, Message = "SiteImage Status Update successfully" });
            }
            else
            {
                return Json(new JsonResponse() { Status = 0, Message = "Something is wrong" });
            }             
        }

        private SiteImageDataListModel GetSiteImageSearchResult(SearchParamModel model, int organizationId)
        {
            try
            {
                model.PageSize = model.PageSize == 0 ? 10 : model.PageSize;
                model.PageIndex = model.PageIndex == 0 ? 1 : model.PageIndex;

                var parameters = new List<SqlParameter>()
                {
                    new SqlParameter("@Search", SqlDbType.NVarChar) { Value = model.SearchBox ?? ""},
                    new SqlParameter("@OrganizationID",SqlDbType.Int) {Value = organizationId},
                    new SqlParameter("@PageIndex",SqlDbType.Int) {Value = model.PageIndex==0?0:model.PageIndex},
                    new SqlParameter("@PageSize",SqlDbType.Int) {Value =model.PageSize},
                    new SqlParameter("@Sorting",SqlDbType.VarChar) {Value = model.Sorting ?? ""},
                };

                int totalRecord = 0;
                var allslotList = _seniorCare.GetSiteImageByOrganization(StoredProcedureList.GetSeniorCareSiteImage, parameters, out totalRecord).ToList();
                var searchResult = new SiteImageDataListModel() { SiteImageList = allslotList };
                ViewBag.SearchBox = model.SearchBox ?? "";

                searchResult.TotalRecord = totalRecord;
                searchResult.PageSize = model.PageSize;
                searchResult.PageIndex = model.PageIndex;

                return searchResult;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #endregion
    }
}
