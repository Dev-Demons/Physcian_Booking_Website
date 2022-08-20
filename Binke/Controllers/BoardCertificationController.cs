using System;
using System.Linq;
using System.Web.Mvc;
using Binke.Models;
using Binke.Utility;
using System.Transactions;
using Binke.ViewModels;
using Doctyme.Repository.Enumerable;
using Doctyme.Repository.Interface;
using Doctyme.Model;

namespace Binke.Controllers
{
    [Authorize(Roles = UserRoles.Admin)]
    public class BoardCertificationController : Controller
    {
        private readonly IBoardCertificationService _boardCertification;
        private readonly ISpecialityService _speciality;

        public BoardCertificationController(IBoardCertificationService boardCertification, ISpecialityService speciality)
        {
            _boardCertification = boardCertification;
            _speciality = speciality;
        }

        #region BoardCertification
        // GET: BoardCertification
        [Route("BoardCertification/{id}")]
        public ActionResult Index(short id = 0)
        {
            try
            {
                if (id == 0)
                {
                    return RedirectToAction("Index", "Speciality");
                }
                ViewBag.SpecialityId = id;
                ViewBag.SpecialityName = _speciality.GetById(id)?.SpecialityName ?? "";
                return View();
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "BoardCertification");
                return View();
            }
        }

        public ActionResult GetBoardCertificationList(short id, JQueryDataTableParamModel param)
        {
            try
            {
                var allBoardCertifications = _boardCertification.GetAll(x => x.IsActive && !x.IsDeleted && x.SpecialityId == id).Select(x => new BoardCertificationViewModel()
                {
                    Id = x.BoardCertificationId,
                    CertificationName = x.CertificationName,
                    CreatedDate = x.CreatedDate.ToDefaultFormate(),
                    UpdatedDate = x.UpdatedDate == null ? "---" : x.UpdatedDate?.ToDefaultFormate()
                }).ToList();

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    allBoardCertifications = allBoardCertifications.Where(x => x.CertificationName.NullToString().ToLower().Contains(param.sSearch.ToLower())
                                                           || x.CreatedDate.ToString().ToLower().Contains(param.sSearch.ToLower())
                                                           || x.UpdatedDate.ToString().ToLower().Contains(param.sSearch.ToLower())).ToList();
                }
                var sortColumnIndex = Convert.ToInt32(HttpContext.Request.QueryString["iSortCol_0"]);

                Func<BoardCertificationViewModel, string> orderingFunction =
                    c => sortColumnIndex == 1 ? c.CertificationName
                        : sortColumnIndex == 2 ? c.CreatedDate
                            : sortColumnIndex == 3 ? c.UpdatedDate
                                : c.CertificationName;

                var sortDirection = HttpContext.Request.QueryString["sSortDir_0"]; // asc or desc
                allBoardCertifications = sortDirection == "asc" ? allBoardCertifications.OrderBy(orderingFunction).ToList() : allBoardCertifications.OrderByDescending(orderingFunction).ToList();

                var display = allBoardCertifications.Skip(param.iDisplayStart).Take(param.iDisplayLength);
                var total = allBoardCertifications.Count;

                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = total,
                    iTotalDisplayRecords = total,
                    aaData = display
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "GetBoardCertificationList");
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        [Route("_BoardCertification/{id}/{typeId}")]
        [HttpGet]
        public PartialViewResult _BoardCertification(int id, short typeId)
        {
            try
            {
                if (id == 0) return PartialView(@"Partial/_BoardCertification", new BoardCertificationViewModel() { SpecialityId = typeId });
                var result = _boardCertification.GetById(id);
                var boardCertification = new BoardCertificationViewModel()
                {
                    Id = result.BoardCertificationId,
                    CertificationName = result.CertificationName
                    //  SpecialityId = result.SpecialityId
                };
                return PartialView(@"Partial/_BoardCertification", boardCertification);
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "_BoardCertification");
                return null;
            }
        }

        [Route("AddEditBoardCertification")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AddEditBoardCertification(BoardCertificationViewModel model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (model.Id == 0)
                    {
                        var bResult = _boardCertification.GetSingle(x => x.CertificationName.ToLower().Equals(model.CertificationName.ToLower()) && x.IsActive && !x.IsDeleted);
                        if (bResult != null)
                        {
                            txscope.Dispose();
                            return Json(new JsonResponse() { Status = 0, Message = "Board Certification already exists. Could not add the data!" });
                        }

                        var boardCertification = new BoardCertifications()
                        {
                            CertificationName = model.CertificationName,
                            SpecialityId = model.SpecialityId,
                            IsActive = true,
                            CreatedDate = DateTime.Now,
                        };
                        _boardCertification.InsertData(boardCertification);
                        _boardCertification.SaveData();
                        txscope.Complete();
                        return Json(new JsonResponse() { Status = 1, Message = "Board Certification save successfully" });
                    }
                    else
                    {
                        var bResult = _boardCertification
                            .GetSingle(x => x.CertificationName.ToLower().Equals(model.CertificationName.ToLower()) && x.BoardCertificationId != model.Id && !x.IsDeleted);
                        if (bResult != null)
                        {
                            txscope.Dispose();
                            return Json(new JsonResponse() { Status = 0, Message = "Board Certification already exists. Could not add the data!" });
                        }
                        var boardCertification = _boardCertification.GetById(model.Id);
                        boardCertification.CertificationName = model.CertificationName;
                        _boardCertification.UpdateData(boardCertification);
                        _boardCertification.SaveData();
                        txscope.Complete();
                        return Json(new JsonResponse() { Status = 1, Message = "Board Certification update successfully" });
                    }
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    Common.LogError(ex, "AddEditBoardCertification");
                    return Json(new JsonResponse() { Status = 0, Message = "Something is wrong." });
                }
            }
        }

        [Route("RemoveBoardCertification/{id?}")]
        [HttpPost]
        public JsonResult RemoveBoardCertification(int id)
        {
            try
            {
                var boardCertification = _boardCertification.GetById(id);
                boardCertification.IsDeleted = true;
                _boardCertification.UpdateData(boardCertification);
                _boardCertification.SaveData();
                return Json(new JsonResponse() { Status = 1, Message = "Board Certification deleted successfully" });
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "RemoveBoardCertification");
                return Json(new JsonResponse() { Status = 0, Message = "Something is wrong." });
            }
        }
        #endregion

    }
}
