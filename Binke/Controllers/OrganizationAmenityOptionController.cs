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
using System.Data;
using System.Collections.Generic;
using Doctyme.Repository.Enumerable;
using Microsoft.AspNet.Identity;

namespace Binke.Controllers
{
    [Authorize]
    public class OrganizationAmenityOptionController : Controller
    {
        private readonly IUserService _appUser;
        private readonly IRepository _repo;
        private readonly ISeniorCareService _seniorCare;

        public OrganizationAmenityOptionController(IUserService appUser, RepositoryService repo, ISeniorCareService seniorCare)
        {
            _appUser = appUser;
            _repo = repo;
            _seniorCare = seniorCare;
        }

        #region Amenity Option

        public ActionResult Index()
        {            
            return View();
        }
        [Route("GetOrganizationAmenityAmenityOptionList/{flag}/{id?}")]

        public ActionResult GetOrganizationAmenityAmenityOptionList(bool flag, JQueryDataTableParamModel param, int id=0)
        {
            var sortColumnIndex = Convert.ToInt32(HttpContext.Request.QueryString["iSortCol_0"]);
            var sortDirection = HttpContext.Request.QueryString["sSortDir_0"]; // asc or desc
            var pageIndex = param.iDisplayStart / param.iDisplayLength;
            var model = new SearchParamModel();
            model.PageIndex = pageIndex;
            model.SearchBox = param.sSearch;
            model.PageSize = param.iDisplayLength;
            var searchResult = GetOrganizationAmenityOptionSearchResult(model, id);
            var allSeniorCare = searchResult.OrganizationAmenityOptionList.AsEnumerable();
            Func<OrganizationAmenityOptionViewModel, string> orderingFunction =
                c => sortColumnIndex == 1 ? c.Name
                            //: sortColumnIndex == 3 ? c.Email
                            //    : sortColumnIndex == 4 ? c.
                            : c.Description;
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

        [Route("_OrganizationAmenityAmenityOption/{id?}/{selectedid?}")]
        public PartialViewResult _OrganizationAmenityAmenityOption(int id = 0, int SelectedId = 0)
        {
            if (id == 0)
            {
                OrganizationAmenityOptionViewModel _amenityOptionViewModel = new OrganizationAmenityOptionViewModel();
                _amenityOptionViewModel.OrganizationID = SelectedId;
                return PartialView(@"/Views/OrganizationAmenityOption/Partial/_OrganizationAmenityAmenityOption.cshtml", _amenityOptionViewModel);
            }

            var result = _repo.ExecWithStoreProcedure<OrganizationAmenityOptionViewModel>("spOrganizationAmenityOption_GetById @OrganizationAmenityOptionID",
                   new SqlParameter("OrganizationAmenityOptionID", System.Data.SqlDbType.Int) { Value = id }
                   ).Select(x => new OrganizationAmenityOptionViewModel
                   {
                       OrganizationAmenityOptionID = x.OrganizationAmenityOptionID,
                       OrganizationID = x.OrganizationID,
                       Name = x.Name,
                       Description = x.Description,
                       IsActive = x.IsActive,
                       IsOption = x.IsOption,                                              
                   }).First();

            //var AmenityOption = _repo.Find<OrganizationAmenityOption>(id);
            //var result = new OrganizationAmenityOptionViewModel()
            //{

            //    OrganizationAmenityOptionID = AmenityOption.OrganizationAmenityOptionID,
            //    OrganizationID = AmenityOption.OrganizationID,
            //    Name = AmenityOption.Name,
            //    Description = AmenityOption.Description,
            //    IsActive = AmenityOption.IsActive,
            //    IsOption = AmenityOption.IsOption,
            //};
            if (result == null)
            {
                return PartialView(@"/Views/OrganizationAmenityOption/Partial/_OrganizationAmenityAmenityOption.cshtml", new OrganizationAmenityOptionViewModel());
            }
            return PartialView(@"/Views/OrganizationAmenityOption/Partial/_OrganizationAmenityAmenityOption.cshtml", result);

        }

        [HttpPost, Route("AddEditOrganizationAmenityAmenityOption"), ValidateAntiForgeryToken]
        public JsonResult AddEditOrganizationAmenityAmenityOption(OrganizationAmenityOptionViewModel model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                string filePath = string.Empty;
                try
                {
                    if (ModelState.IsValid)
                    {
                        int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spOrganizationAmenityOption_Create " +
                                "@OrganizationAmenityOptionID,@OrganizationID,@Name,@Description,@ImagePath,@IsOption,@CreatedDate,@UpdatedDate,@IsDeleted,@CreatedBy,@ModifiedBy,@IsActive",
                                new SqlParameter("OrganizationAmenityOptionID", System.Data.SqlDbType.Int) { Value = model.OrganizationAmenityOptionID },
                                new SqlParameter("OrganizationID", System.Data.SqlDbType.Int) { Value = (object)model.OrganizationID ?? DBNull.Value },
                                new SqlParameter("Name", System.Data.SqlDbType.VarChar) { Value = (object)model.Name ?? DBNull.Value },
                                new SqlParameter("Description", System.Data.SqlDbType.VarChar) { Value = (object)model.Description ?? DBNull.Value },
                                new SqlParameter("ImagePath", System.Data.SqlDbType.VarChar) { Value = (object)model.ImagePath ?? DBNull.Value },
                                new SqlParameter("IsOption", System.Data.SqlDbType.Bit) { Value = model.IsOption },
                                new SqlParameter("CreatedDate", System.Data.SqlDbType.DateTime) { Value = DateTime.UtcNow },
                                new SqlParameter("UpdatedDate", System.Data.SqlDbType.DateTime) { Value = model.OrganizationAmenityOptionID > 0 ? DateTime.UtcNow : (object)DBNull.Value },
                                new SqlParameter("IsDeleted", System.Data.SqlDbType.Bit) { Value = model.OrganizationAmenityOptionID > 0 ? model.IsDeleted : false },
                                new SqlParameter("CreatedBy", System.Data.SqlDbType.Int) { Value = User.Identity.GetUserId<int>() },
                                new SqlParameter("ModifiedBy", System.Data.SqlDbType.Int) { Value = model.OrganizationAmenityOptionID > 0 ? User.Identity.GetUserId<int>() : (object)DBNull.Value },
                                new SqlParameter("IsActive", System.Data.SqlDbType.Bit) { Value = model.IsActive }
                                );
                        if (model.OrganizationAmenityOptionID == 0)
                        {                            
                            //var bResult = _repo.Find<OrganizationAmenityOption>(x => x.Name.ToLower().Equals(model.Name.ToLower()) && x.IsActive && !x.IsDeleted);
                            //if (bResult != null)
                            //{
                            //    txscope.Dispose();
                            //    return Json(new JsonResponse() { Status = 0, Message = "Name already exists. Could not add the data!" });
                            //}
                            //var AmenityOption = new OrganizationAmenityOption()
                            //{
                            //    Name = model.Name,
                            //    Description = model.Description,
                            //    OrganizationID = model.OrganizationID,
                            //    CreatedDate = DateTime.Now,
                            //    IsActive = true,
                            //    IsDeleted = false,
                            //};
                            //_repo.Insert<OrganizationAmenityOption>(AmenityOption, true);
                            //// _repo.sa();
                            txscope.Complete();
                            return Json(new JsonResponse() { Status = 1, Message = "Amenity Option save successfully" });
                        }
                        else
                        {
                            //var bResult = _repo.Find<OrganizationAmenityOption>(x => x.OrganizationAmenityOptionID != model.OrganizationAmenityOptionID && x.Name == model.Name && x.IsActive && !x.IsDeleted);
                            //if (bResult != null)
                            //{
                            //    txscope.Dispose();
                            //    return Json(new JsonResponse { Status = 0, Message = "Name already exists. Could not add the data!" });
                            //}

                            //var oHour = _repo.Find<OrganizationAmenityOption>(model.OrganizationAmenityOptionID);
                            //oHour.Name = model.Name;
                            //oHour.Description = model.Description;
                            //_repo.Update<OrganizationAmenityOption>(oHour, true);
                            ////_user.SaveData();
                            txscope.Complete();
                            return Json(new JsonResponse { Status = 1, Message = "Amenity Option updated successfully" });
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
        
        [Route("RemoveOrganizationAmenityOption/{id?}")]
        [HttpPost]
        public JsonResult RemoveOrganizationAmenityOption(int id)
        {
            if (id > 0)
            {
                int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spOrganizationAmenityOption_Delete  " + 
                    "@OrganizationAmenityOptionID ", new SqlParameter("OrganizationAmenityOptionID", System.Data.SqlDbType.Int) { Value = id });
                return Json(new JsonResponse() { Status = 1, Message = "Amenity Option deleted successfully" });
            }
            else
            {
                return Json(new JsonResponse() { Status = 0, Message = "Something is wrong" });
            }
            //var AmenityOption = _repo.Find<OrganizationAmenityOption>(id);
            //_repo.Delete<OrganizationAmenityOption>(AmenityOption);
            //return Json(new JsonResponse() { Status = 1, Message = "Amenity Option deleted successfully" });
        }

        [Route("OrganizationAmenityOptionStatusChange/{id?}/{flag?}/{isoption?}")]
        [HttpPost]
        public JsonResult OrganizationAmenityOptionStatusChange(int id, int flag = 0, int IsOption = 0)
        {
            if (id > 0)
            {
                int ExecWithStoreProcedure = _repo.ExecuteSQLQuery("spOrganizationAmenityOption_StatusChange  " +
                    "@OrganizationAmenityOptionId,@Flag,@IsOption", new SqlParameter("OrganizationAmenityOptionId", System.Data.SqlDbType.Int) { Value = id }, new SqlParameter("Flag", System.Data.SqlDbType.Int) { Value = flag }, new SqlParameter("IsOption", System.Data.SqlDbType.Int) { Value = IsOption });
                return Json(new JsonResponse() { Status = 1, Message = "Organization Amenity Option Status Update successfully" });
            }
            else
            {
                return Json(new JsonResponse() { Status = 0, Message = "Something is wrong" });
            }
        }

        private OrganizationAmenityOptionDataListModel GetOrganizationAmenityOptionSearchResult(SearchParamModel model, int organizationId)
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
                var allslotList = _seniorCare.GetOrganizationAmenityOptionByOrganization(StoredProcedureList.GetSeniorCareOrganizationAmenityOption, parameters, out totalRecord).ToList();
                var searchResult = new OrganizationAmenityOptionDataListModel() { OrganizationAmenityOptionList = allslotList };
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
