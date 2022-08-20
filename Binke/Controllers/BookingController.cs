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

namespace Binke.Controllers
{
    [Authorize]
    public class BookingController : Controller
    {
        private readonly IUserService _appUser;
        private readonly IRepository _repo;

        public BookingController(IUserService appUser, RepositoryService repo)
        {
            _appUser = appUser;
            _repo = repo;
        }

        #region Booking

        public ActionResult Index()
        {
            return View();
        }
        //public ActionResult GetSlotList(JQueryDataTableParamModel param, int Id = 0)
        //{

        //    int count = _repo.All<Slot>().Where(f => f.ReferenceId == Id).Count();

        //    var allSlot = _repo.ExecWithStoreProcedure<Slot>("spSlot_Get @PageIndex, @PageSize, @OrganizationID ",
        //                             new SqlParameter("PageIndex", System.Data.SqlDbType.Int) { Value = param.iDisplayStart > 0 ? ((param.iDisplayStart / param.iDisplayLength) + 1) : 1 },
        //                             new SqlParameter("PageSize", System.Data.SqlDbType.Int) { Value = param.iDisplayStart + param.iDisplayLength },
        //                             new SqlParameter("OrganizationID", System.Data.SqlDbType.Int) { Value = Id }
        //                         )
        //                       .Select(x => new SlotViewModel()
        //                       {
        //                           SlotId = x.SlotId,
        //                           ReferenceId = x.ReferenceId,
        //                           SlotDate = x.SlotDate,
        //                           SlotTime = x.SlotTime,
        //                           BookedFor = x.BookedFor,
        //                           Description = x.Description,
        //                           IsBooked = x.IsBooked,
        //                           IsEmailReminder = x.IsEmailReminder,
        //                           IsTextReminder = x.IsTextReminder,
        //                           IsInsuranceChanged = x.IsInsuranceChanged,
        //                           InsurancePlanId = x.InsurancePlanId,
        //                           AddressId = x.AddressId,
        //                           CreatedDate = x.CreatedDate.ToDefaultFormate(),
        //                           UpdatedDate = x.UpdatedDate == null ? "---" : x.UpdatedDate?.ToDefaultFormate()
        //                       });

        //    allSlot = allSlot.ToList();
        //    if (!string.IsNullOrEmpty(param.sSearch))
        //    {
        //        allSlot = allSlot.Where(x => x.SlotDate.ToString().ToLower().Contains(param.sSearch.ToLower())).ToList();
        //    }

        //    var sortColumnIndex = Convert.ToInt32(HttpContext.Request.QueryString["iSortCol_0"]);
        //    var sortDirection = HttpContext.Request.QueryString["sSortDir_0"]; // asc or desc

        //    Func<SlotViewModel, string> orderingFunction =
        //        c => sortColumnIndex == 1 ? c.SlotDate
        //                    : sortColumnIndex == 2 ? c.SlotTime
        //                    : c.CreatedDate;

        //    allSlot = sortDirection == "asc" ? allSlot.OrderBy(orderingFunction).ToList() : allSlot.OrderByDescending(orderingFunction).ToList();

        //    var display = allSlot.Skip(param.iDisplayStart).Take(param.iDisplayLength);
        //    var total = allSlot.Count();

        //    return Json(new
        //    {
        //        param.sEcho,
        //        iTotalRecords = count,
        //        iTotalDisplayRecords = count,
        //        aaData = display
        //    }, JsonRequestBehavior.AllowGet);

        //}

        [Route("_Slot/{id?}/{selectedid?}")]
        public PartialViewResult _Slot(int id = 0, int SelectedId = 0)
        {
            try
            {
                if (id == 0)
                {
                    SlotViewModel _SlotViewModel = new SlotViewModel();
                    _SlotViewModel.ReferenceId = SelectedId;
                    return PartialView(@"Partial/_Slot", _SlotViewModel);
                }
                var Slot = _repo.Find<Slot>(id);
                var result = new SlotViewModel()
                {

                    SlotId = Slot.SlotId,
                    ReferenceId = Slot.ReferenceId,
                    SlotDate = Convert.ToDateTime(Slot.SlotDate).ToString("yyyy-MM-dd"),
                    SlotTime = Slot.SlotTime,
                    BookedFor = Slot.BookedFor,
                    Description = Slot.Description,
                    IsBooked = Slot.IsBooked,
                    IsEmailReminder = Slot.IsEmailReminder,
                    IsTextReminder = Slot.IsTextReminder,
                    IsInsuranceChanged = Slot.IsInsuranceChanged,
                    InsurancePlanId = Slot.InsurancePlanId,
                    AddressId = Slot.AddressId,
                    IsActive = Slot.IsActive,
                };
                if (result == null) return PartialView(@"Partial/_Slot", new SlotViewModel());

                return PartialView(@"Partial/_Slot", result);
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "Booking/_Slot");
                return null;
            }
        }

        [HttpPost, Route("AddEditSlotBooking"), ValidateAntiForgeryToken]
        public JsonResult AddEditSlotBooking(SlotViewModel model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                string filePath = string.Empty;
                try
                {
                    if (ModelState.IsValid)
                    {
                        if (model.SlotId == 0)
                        {
                            //var bResult = _repo.Find<OrganizationAmenityOption>(x => x.Name.ToLower().Equals(model.Name.ToLower()) && x.IsActive && !x.IsDeleted);
                            //if (bResult != null)
                            //{
                            //    txscope.Dispose();
                            //    return Json(new JsonResponse() { Status = 0, Message = "Name already exists. Could not add the data!" });
                            //}

                            var slot = new Slot()
                            {
                                SlotDate = Convert.ToDateTime(model.SlotDate).ToString("yyyy-MM-dd"),
                                SlotTime = model.SlotTime,
                                ReferenceId = model.ReferenceId,
                                //BookedFor = slot.BookedFor,
                                Description = model.Description,
                                IsBooked = model.IsBooked,
                                IsEmailReminder = model.IsEmailReminder,
                                IsTextReminder = model.IsTextReminder,
                                IsInsuranceChanged = model.IsInsuranceChanged,
                                CreatedDate = DateTime.Now,
                                IsActive = true,
                                IsDeleted = false,
                            };
                            _repo.Insert<Slot>(slot, true);
                            // _repo.sa();
                            txscope.Complete();
                            return Json(new JsonResponse() { Status = 1, Message = "Slot save successfully" });
                        }
                        else
                        {
                            //var bResult = _repo.Find<Slot>(x => x.SlotId != model.ReferenceId && x.Name == model.Name && x.IsActive && !x.IsDeleted);
                            //if (bResult != null)
                            //{
                            //    txscope.Dispose();
                            //    return Json(new JsonResponse { Status = 0, Message = "Name already exists. Could not add the data!" });
                            //}

                            var slot = _repo.Find<Slot>(model.SlotId);
                            slot.SlotDate = Convert.ToDateTime(model.SlotDate).ToString("yyyy-MM-dd");
                            slot.SlotTime = model.SlotTime;
                            slot.Description = model.Description;
                            slot.IsBooked = model.IsBooked;
                            slot.IsEmailReminder = model.IsEmailReminder;
                            slot.IsTextReminder = model.IsTextReminder;
                            slot.IsInsuranceChanged = model.IsInsuranceChanged;
                            slot.UpdatedDate = DateTime.Now;
                            _repo.Update<Slot>(slot, true);
                            //_user.SaveData();
                            txscope.Complete();
                            return Json(new JsonResponse { Status = 1, Message = "Slot updated successfully" });
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
                    Common.LogError(ex, "AddEditSlot-Post");
                    return Json(new JsonResponse { Status = 0, Message = "Something is wrong." });
                }
            }
        }


        [Route("RemoveBookingSlot/{id?}")]
        [HttpPost]
        public JsonResult RemoveBookingSlot(int id)
        {
            try
            {
                var Slot = _repo.Find<Slot>(id);
                _repo.Delete<Slot>(Slot);
                return Json(new JsonResponse() { Status = 1, Message = "Slot deleted successfully" });
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "RemoveBookingSlot-Post");
                return Json(new JsonResponse { Status = 0, Message = "Something is wrong." });
            }
        }

        #endregion
    }
}
