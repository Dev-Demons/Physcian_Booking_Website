using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using Binke.Models;
using Binke.Utility;
using Binke.ViewModels;
using Doctyme.Model;
using Doctyme.Repository.Enumerable;
using Doctyme.Repository.Interface;

namespace Binke.Controllers
{
    [Authorize]
    public class LanguageController : Controller
    {
        private readonly ILanguageService _language;
        private readonly IUserService _appUser;
        public LanguageController(ILanguageService language, IUserService appUser)
        {
            _language = language;
            _appUser = appUser;
        }
        #region Language
        // GET: Language
        [Authorize(Roles = UserRoles.Admin)]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetLanguageList(JQueryDataTableParamModel param)
        {
            var allLanguages = _language.GetAll(x => x.IsActive && !x.IsDeleted).Select(x => new LanguageViewModel()
            {
                Id = x.LanguageId,
                LanguageName = x.LanguageName,
                LanguageCode = x.LanguageCode,
                CreatedDate = x.CreatedDate.ToDefaultFormate(),
                UpdatedDate = x.UpdatedDate == null ? "---" : x.UpdatedDate?.ToDefaultFormate()
            }).ToList();

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                allLanguages = allLanguages.Where(x => x.LanguageName.NullToString().ToLower().Contains(param.sSearch.ToLower())
                                                       || x.CreatedDate.ToString().ToLower().Contains(param.sSearch.ToLower())
                                                       || x.UpdatedDate.ToString().ToLower().Contains(param.sSearch.ToLower())).ToList();
            }
            var sortColumnIndex = Convert.ToInt32(HttpContext.Request.QueryString["iSortCol_0"]);

            Func<LanguageViewModel, string> orderingFunction =
                c => sortColumnIndex == 1 ? c.LanguageName
                    : sortColumnIndex == 2 ? c.CreatedDate
                        : sortColumnIndex == 3 ? c.UpdatedDate
                            : c.LanguageName;

            var sortDirection = HttpContext.Request.QueryString["sSortDir_0"]; // asc or desc
            allLanguages = sortDirection == "asc" ? allLanguages.OrderBy(orderingFunction).ToList() : allLanguages.OrderByDescending(orderingFunction).ToList();

            var display = allLanguages.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var total = allLanguages.Count;

            return Json(new
            {
                param.sEcho,
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                aaData = display
            }, JsonRequestBehavior.AllowGet);
        }

        [Route("_Language/{id?}")]
        [HttpGet]
        public PartialViewResult _Language(int id)
        {
            if (id == 0) return PartialView(@"Partial/_Language", new LanguageViewModel());
            var result = _language.GetById(id);
            var language = new LanguageViewModel()
            {
                Id = result.LanguageId,
                LanguageName = result.LanguageName
            };
            return PartialView(@"Partial/_Language", language);
        }

        [Route("AddEditLanguage")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AddEditLanguage(LanguageViewModel model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (model.Id == 0)
                    {
                        var bResult = _language.GetSingle(x => x.LanguageName.ToLower().Equals(model.LanguageName.ToLower()) && x.IsActive && !x.IsDeleted);
                        if (bResult != null)
                        {
                            txscope.Dispose();
                            return Json(new JsonResponse() { Status = 0, Message = "Language already exists. Could not add the data!" });
                        }

                        var language = new Language()
                        {
                            LanguageName = model.LanguageName,
                            IsActive = true,
                            IsDeleted = false,
                            CreatedDate = DateTime.Now
                        };
                        _language.InsertData(language);
                        _language.SaveData();
                        txscope.Complete();
                        return Json(new JsonResponse() { Status = 1, Message = "Language save successfully" });
                    }
                    else
                    {
                        var bResult = _language
                            .GetSingle(x => x.LanguageName.ToLower().Equals(model.LanguageName.ToLower()) && x.LanguageId != model.Id && !x.IsDeleted);
                        if (bResult != null)
                        {
                            txscope.Dispose();
                            return Json(new JsonResponse() { Status = 0, Message = "Language already exists. Could not add the data!" });
                        }
                        var language = _language.GetById(model.Id);
                        language.LanguageName = model.LanguageName;
                        _language.UpdateData(language);
                        _language.SaveData();
                        txscope.Complete();
                        return Json(new JsonResponse() { Status = 1, Message = "Language update successfully" });
                    }
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    Common.LogError(ex, "AddEditLanguage");
                    return Json(new JsonResponse() { Status = 0, Message = "Something is wrong." });
                }
            }
        }

        [Route("RemoveLanguage/{id?}")]
        [HttpPost]
        public JsonResult RemoveLanguage(int id)
        {
            var Language = _language.GetById(id);
            Language.IsDeleted = true;
            _language.UpdateData(Language);
            _language.SaveData();
            return Json(new JsonResponse() { Status = 1, Message = "Language deleted successfully" });
        }
        #endregion
    }
}
