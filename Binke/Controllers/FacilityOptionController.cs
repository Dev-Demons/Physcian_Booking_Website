using System;
using System.Linq;
using System.Transactions;
using System.Web.Mvc;
using Binke.Models;
using Binke.Utility;
using Binke.ViewModels;
using Doctyme.Model;
using Doctyme.Repository.Interface;

namespace Binke.Controllers
{
    [Authorize]
    public class FacilityOptionController : Controller
    {
        private readonly IFacilityOptionService _facilityOption;
        private readonly IUserService _appUser;

        public FacilityOptionController(IFacilityOptionService facilityOption, IUserService appUser)
        {
            _facilityOption = facilityOption;
            _appUser = appUser;
        }

        #region FacilityOption
        // GET: FacilityOption
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetFacilityOptionList(JQueryDataTableParamModel param)
        {
            var allFacilityOptions = _facilityOption.GetAll(x => x.IsActive && !x.IsDeleted).Select(x => new FacilityOptionViewModel()
            {
                Id = x.OrganizationAmenityOptionID,
                FacilityOptionName = x.Name,
                CreatedDate = x.CreatedDate.ToDefaultFormate(),
                UpdatedDate = x.UpdatedDate == null ? "---" : x.UpdatedDate?.ToDefaultFormate()
            }).ToList();

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                allFacilityOptions = allFacilityOptions.Where(x => x.FacilityOptionName.NullToString().ToLower().Contains(param.sSearch.ToLower())
                                                       || x.CreatedDate.ToString().ToLower().Contains(param.sSearch.ToLower())
                                                       || x.UpdatedDate.ToString().ToLower().Contains(param.sSearch.ToLower())).ToList();
            }
            var sortColumnIndex = Convert.ToInt32(HttpContext.Request.QueryString["iSortCol_0"]);

            Func<FacilityOptionViewModel, string> orderingFunction =
                c => sortColumnIndex == 1 ? c.FacilityOptionName
                    : sortColumnIndex == 2 ? c.CreatedDate
                        : sortColumnIndex == 3 ? c.UpdatedDate
                            : c.FacilityOptionName;

            var sortDirection = HttpContext.Request.QueryString["sSortDir_0"]; // asc or desc
            allFacilityOptions = sortDirection == "asc" ? allFacilityOptions.OrderBy(orderingFunction).ToList() : allFacilityOptions.OrderByDescending(orderingFunction).ToList();

            var display = allFacilityOptions.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var total = allFacilityOptions.Count;

            return Json(new
            {
                param.sEcho,
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                aaData = display
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet, Route("_FacilityOption/{id?}")]
        public PartialViewResult _FacilityOption(int id)
        {
            ViewBag.FacilityOption = _facilityOption.GetAll(x => x.IsActive && !x.IsDeleted).Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.OrganizationAmenityOptionID.ToString()
            }).ToList();
            if (id == 0) return PartialView(@"Partial/_FacilityOption", new FacilityOptionViewModel());
            var result = _facilityOption.GetById(id);
            var FacilityOption = new FacilityOptionViewModel()
            {
                Id = result.OrganizationAmenityOptionID,
                FacilityOptionId =(byte) result.OrganizationAmenityOptionID,
                FacilityOptionName = result.Name
            };
            return PartialView(@"Partial/_FacilityOption", FacilityOption);
        }

        [HttpPost, Route("AddEditFacilityOption"), ValidateAntiForgeryToken]
        public JsonResult AddEditFacilityOption(FacilityOptionViewModel model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (model.Id == 0)
                    {
                        var bResult = _facilityOption.GetSingle(x => x.Name.ToLower().Equals(model.FacilityOptionName.ToLower()) && x.IsActive && !x.IsDeleted);
                        if (bResult != null)
                        {
                            txscope.Dispose();
                            return Json(new JsonResponse() { Status = 0, Message = "Facility Option already exists. Could not add the data!" });
                        }

                        var facilityOption = new OrganizationAmenityOption()
                        {
                            //FacilityOptionName = model.FacilityOptionName,
                            OrganizationAmenityOptionID = model.FacilityOptionId,
                            IsActive = true,
                            IsDeleted = false,
                        };
                        _facilityOption.InsertData(facilityOption);
                        _facilityOption.SaveData();
                        txscope.Complete();
                        return Json(new JsonResponse() { Status = 1, Message = "Facility Option save successfully" });
                    }
                    else
                    {
                        var bResult = _facilityOption
                            .GetSingle(x => x.Name.ToLower().Equals(model.FacilityOptionName.ToLower()) && x.OrganizationAmenityOptionID != model.Id && !x.IsDeleted);
                        if (bResult != null)
                        {
                            txscope.Dispose();
                            return Json(new JsonResponse() { Status = 0, Message = "Facility Option already exists. Could not add the data!" });
                        }
                        var facilityOption = _facilityOption.GetById(model.Id);
                        facilityOption.Name = model.FacilityOptionName;
                        facilityOption.OrganizationAmenityOptionID = model.FacilityOptionId;
                        _facilityOption.UpdateData(facilityOption);
                        _facilityOption.SaveData();
                        txscope.Complete();
                        return Json(new JsonResponse() { Status = 1, Message = "Facility Option update successfully" });
                    }
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    Common.LogError(ex, "AddEditFacilityOption-POST");
                    return Json(new JsonResponse() { Status = 0, Message = "Something is wrong." });
                }
            }
        }

        [Route("RemoveFacilityOption/{id?}")]
        [HttpPost]
        public JsonResult RemoveFacilityOption(int id)
        {
            var facilityOption = _facilityOption.GetById(id);
            facilityOption.IsDeleted = true;
            _facilityOption.UpdateData(facilityOption);
            _facilityOption.SaveData();
            return Json(new JsonResponse() { Status = 1, Message = "Facility Option deleted successfully" });
        }
        #endregion

    }
}
