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
    public class FacilityTypeController : Controller
    {
        private readonly IFacilityTypeService _facilityType;
        private readonly IFacilityOptionService _facilityOption;
        private readonly IUserService _appUser;

        public FacilityTypeController(IFacilityTypeService facilityType, IFacilityOptionService facilityOption, IUserService appUser)
        {
            _facilityType = facilityType;
            _facilityOption = facilityOption;
            _appUser = appUser;
        }

        #region FacilityType
        // GET: FacilityType
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetFacilityTypeList(JQueryDataTableParamModel param)
        {
            var allFacilityTypes = _facilityType.GetAll(x => x.IsActive && !x.IsDeleted).Select(x => new FacilityTypeViewModel()
            {
                Id = x.OrganizationTypeID,
                FacilityTypeName = x.Org_Type_Name,
                FacilityOption = x.Org_Type_Description,
                CreatedDate = x.CreatedDate.ToDefaultFormate(),
                UpdatedDate = x.UpdatedDate == null ? "---" : x.UpdatedDate?.ToDefaultFormate()
            }).ToList();

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                allFacilityTypes = allFacilityTypes.Where(x => x.FacilityTypeName.NullToString().ToLower().Contains(param.sSearch.ToLower())
                                                       || x.FacilityOption.NullToString().ToLower().Contains(param.sSearch.ToLower())
                                                       || x.CreatedDate.ToString().ToLower().Contains(param.sSearch.ToLower())
                                                       || x.UpdatedDate.ToString().ToLower().Contains(param.sSearch.ToLower())).ToList();
            }
            var sortColumnIndex = Convert.ToInt32(HttpContext.Request.QueryString["iSortCol_0"]);

            Func<FacilityTypeViewModel, string> orderingFunction =
                c => sortColumnIndex == 1 ? c.FacilityTypeName
                    : sortColumnIndex == 2 ? c.FacilityOption
                    : sortColumnIndex == 3 ? c.CreatedDate
                        : sortColumnIndex == 4 ? c.UpdatedDate
                            : c.FacilityTypeName;

            var sortDirection = HttpContext.Request.QueryString["sSortDir_0"]; // asc or desc
            allFacilityTypes = sortDirection == "asc" ? allFacilityTypes.OrderBy(orderingFunction).ToList() : allFacilityTypes.OrderByDescending(orderingFunction).ToList();

            var display = allFacilityTypes.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var total = allFacilityTypes.Count;

            return Json(new
            {
                param.sEcho,
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                aaData = display
            }, JsonRequestBehavior.AllowGet);
        }

        [Route("_FacilityType/{id?}")]
        [HttpGet]
        public PartialViewResult _FacilityType(int id)
        {
            ViewBag.FacilityOption = _facilityOption.GetAll(x => x.IsActive && !x.IsDeleted).Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.OrganizationAmenityOptionID.ToString()
            }).ToList();
            if (id == 0) return PartialView(@"Partial/_FacilityType", new FacilityTypeViewModel());
            var result = _facilityType.GetById(id);
            var FacilityType = new FacilityTypeViewModel()
            {
                Id = result.OrganizationTypeID,
                FacilityOptionId =(byte) result.OrganizationTypeID,
                FacilityTypeName = result.Org_Type_Name
            };
            return PartialView(@"Partial/_FacilityType", FacilityType);
        }

        [Route("AddEditFacilityType")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AddEditFacilityType(FacilityTypeViewModel model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (model.Id == 0)
                    {
                        var bResult = _facilityType.GetSingle(x => x.Org_Type_Name.ToLower().Equals(model.FacilityTypeName.ToLower()) && x.IsActive && !x.IsDeleted);
                        if (bResult != null)
                        {
                            txscope.Dispose();
                            return Json(new JsonResponse() { Status = 0, Message = "Facility Type already exists. Could not add the data!" });
                        }

                        var FacilityType = new OrganisationType();
                        {
                            //FacilityTypeName = model.FacilityTypeName,
                            //FacilityOptionId = model.FacilityOptionId,
                            //IsActive = true,
                            //IsDeleted = false,
                        };
                        _facilityType.InsertData(FacilityType);
                        _facilityType.SaveData();
                        txscope.Complete();
                        return Json(new JsonResponse() { Status = 1, Message = "Facility Type save successfully" });
                    }
                    else
                    {
                        var bResult = _facilityType
                            .GetSingle(x => x.Org_Type_Name.ToLower().Equals(model.FacilityTypeName.ToLower()) && x.OrganizationTypeID != model.Id && !x.IsDeleted);
                        if (bResult != null)
                        {
                            txscope.Dispose();
                            return Json(new JsonResponse() { Status = 0, Message = "Facility Type already exists. Could not add the data!" });
                        }
                        var facilityType = _facilityType.GetById(model.Id);
                        facilityType.Org_Type_Name = model.FacilityTypeName;
                       // facilityType.FacilityOptionId = model.FacilityOptionId;
                        _facilityType.UpdateData(facilityType);
                        _facilityType.SaveData();
                        txscope.Complete();
                        return Json(new JsonResponse() { Status = 1, Message = "Facility Type update successfully" });
                    }
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    Common.LogError(ex, "AddEditFacilityType");
                    return Json(new JsonResponse() { Status = 0, Message = "Something is wrong." });
                }
            }
        }

        [Route("RemoveFacilityType/{id?}")]
        [HttpPost]
        public JsonResult RemoveFacilityType(int id)
        {
            var facilityType = _facilityType.GetById(id);
            facilityType.IsDeleted = true;
            _facilityType.UpdateData(facilityType);
            _facilityType.SaveData();
            return Json(new JsonResponse() { Status = 1, Message = "Facility Type deleted successfully" });
        }
        #endregion

    }
}
