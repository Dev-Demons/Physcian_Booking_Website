using System;
using System.Linq;
using System.Web.Mvc;
//using Binke.Model;
using Binke.Models;
//using Binke.Repository.Enumerable;
//using Binke.Repository.Interface;
using Binke.Utility;
using System.Transactions;
using Binke.ViewModels;
using Doctyme.Repository.Interface;
using Doctyme.Repository.Enumerable;
using Doctyme.Model;

namespace Binke.Controllers
{
    [Authorize(Roles = UserRoles.Admin)]
    public class SpecialityController : Controller
    {
        private readonly ISpecialityService _speciality;

        public SpecialityController(ISpecialityService speciality)
        {
            _speciality = speciality;
        }

        #region Speciality
        // GET: Speciality
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetSpecialityList(JQueryDataTableParamModel param)
        {
            var allSpecialitys = _speciality.GetAll(x => x.IsActive && !x.IsDeleted).Select(x => new SpecialityViewModel()
            {
                Id = x.SpecialityId,
                SpecialityName = x.SpecialityName,
                BoardCertification = ""
                //BoardCertification = string.Join(", ", x.BoardCertifications.Select(y => y.CertificationName)),
            }).ToList();

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                allSpecialitys = allSpecialitys.Where(x => x.SpecialityName.NullToString().ToLower().Contains(param.sSearch.ToLower())
                                                       || x.BoardCertification.ToString().ToLower().Contains(param.sSearch.ToLower())
                                                     ).ToList();
            }
            var sortColumnIndex = Convert.ToInt32(HttpContext.Request.QueryString["iSortCol_0"]);

            Func<SpecialityViewModel, string> orderingFunction =
                c => sortColumnIndex == 1 ? c.SpecialityName
                    : sortColumnIndex == 2 ? c.BoardCertification
                            : c.SpecialityName;

            var sortDirection = HttpContext.Request.QueryString["sSortDir_0"]; // asc or desc
            allSpecialitys = sortDirection == "asc" ? allSpecialitys.OrderBy(orderingFunction).ToList() : allSpecialitys.OrderByDescending(orderingFunction).ToList();

            var display = allSpecialitys.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var total = allSpecialitys.Count;

            return Json(new
            {
                param.sEcho,
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                aaData = display
            }, JsonRequestBehavior.AllowGet);
        }

        [Route("_Speciality/{id?}")]
        [HttpGet]
        public PartialViewResult _Speciality(int id)
        {
            if (id == 0) return PartialView(@"Partial/_Speciality", new SpecialityViewModel());
            var result = _speciality.GetById(id);
            var speciality = new SpecialityViewModel()
            {
                Id = result.SpecialityId,
                SpecialityName = result.SpecialityName
            };
            return PartialView(@"Partial/_Speciality", speciality);
        }

        [Route("AddEditSpeciality")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AddEditSpeciality(SpecialityViewModel model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (model.Id == 0)
                    {
                        var bResult = _speciality.GetSingle(x => x.SpecialityName.ToLower().Equals(model.SpecialityName.ToLower()) && x.IsActive && !x.IsDeleted);
                        if (bResult != null)
                        {
                            txscope.Dispose();
                            return Json(new JsonResponse() { Status = 0, Message = "Speciality already exists. Could not add the data!" });
                        }

                        var speciality = new Speciality()
                        {
                            SpecialityName = model.SpecialityName,
                            IsActive = true,
                            IsDeleted = false,
                            CreatedDate = DateTime.Now,
                        };
                        _speciality.InsertData(speciality);
                        _speciality.SaveData();
                        txscope.Complete();
                        return Json(new JsonResponse() { Status = 1, Message = "Speciality save successfully" });
                    }
                    else
                    {
                        var bResult = _speciality
                            .GetSingle(x => x.SpecialityName.ToLower().Equals(model.SpecialityName.ToLower()) && x.SpecialityId != model.Id && !x.IsDeleted);
                        if (bResult != null)
                        {
                            txscope.Dispose();
                            return Json(new JsonResponse() { Status = 0, Message = "Speciality already exists. Could not add the data!" });
                        }
                        var speciality = _speciality.GetById(model.Id);
                        speciality.SpecialityName = model.SpecialityName;
                        _speciality.UpdateData(speciality);
                        _speciality.SaveData();
                        txscope.Complete();
                        return Json(new JsonResponse() { Status = 1, Message = "Speciality update successfully" });
                    }
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    Common.LogError(ex, "AddEditSpeciality");
                    return Json(new JsonResponse() { Status = 0, Message = "Something is wrong." });
                }
            }
        }

        [Route("RemoveSpeciality/{id?}")]
        [HttpPost]
        public JsonResult RemoveSpeciality(int id)
        {
            var speciality = _speciality.GetById(id);
            speciality.IsDeleted = true;
            _speciality.UpdateData(speciality);
            _speciality.SaveData();
            return Json(new JsonResponse() { Status = 1, Message = "Speciality deleted successfully" });
        }
        #endregion

        #region Featured Specialities
        [Route("FeaturedSpecialities")]
        public ActionResult FeaturedSpecialities()
        {
            return View();
        }
        #endregion
    }
}
