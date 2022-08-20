using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Doctyme.Repository.Interface;
using Binke.Models;
using Binke.ViewModels;
using Binke.Utility;
using System.Transactions;
using Doctyme.Model;
using Binke.App_Helpers;


namespace Binke.Controllers
{
   
    public class UserController : Controller
    {
        private readonly IUserTypeService _user;
        private readonly IDrugTypeService  _drugtype;
        private readonly  IRepository _repo;
   

        public UserController(IUserTypeService user, IRepository repo, IDrugTypeService drugtype)
        {
            _user = user;
            _drugtype = drugtype;
            _repo = repo;


        }
        // GET: User
        #region UserType
        [Route("UserType")]
        public ActionResult index()
        {
            return View();
        }

        [Route("GetUserTypeList/{flag}")]
        public ActionResult GetUserTypeList(bool flag,JQueryDataTableParamModel param)
        {
           

            var allUserType = _user.GetAll(x =>  !x.IsDeleted).Select(x => new UserTypeViewModel()
            {
                UserTypeId = x.UserTypeId,
                UserTypeName = x.UserTypeName,
                Description = x.Description==null?"": x.Description,
                CreatedDate = x.CreatedDate.ToDefaultFormate(),
                UpdatedDate = x.UpdatedDate == null ? "---" : x.UpdatedDate?.ToDefaultFormate(),
                IsActive = x.IsActive
            }).ToList();

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                allUserType = allUserType.Where(x => Convert.ToString(x.UserTypeName).ToLower().Contains(param.sSearch.ToLower())
                                                || Convert.ToString(x.Description).ToLower().Contains(param.sSearch.ToLower())
                                               ).ToList();
            }

            var sortColumnIndex = Convert.ToInt32(HttpContext.Request.QueryString["iSortCol_0"]);
            var sortDirection = HttpContext.Request.QueryString["sSortDir_0"]; // asc or desc

            Func<UserTypeViewModel, string> orderingFunction =
                c => sortColumnIndex == 1 ? c.UserTypeName
                    : sortColumnIndex == 2 ? c.Description
                    : sortColumnIndex == 4 ? c.CreatedDate
                        : sortColumnIndex == 5 ? c.UpdatedDate
                         :c.UserTypeName;
            allUserType = sortDirection == "asc" ? allUserType.OrderBy(orderingFunction).ToList() : allUserType.OrderByDescending(orderingFunction).ToList();

            var display = allUserType.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var total = allUserType.Count;

            return Json(new
            {
                param.sEcho,
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                aaData = display
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet, Route("_UserType/{id?}")]
        public PartialViewResult _UserType(int id)
        {
           
            if (id == 0) return PartialView(@"Partial/_UserType", new UserTypeViewModel());
            var usertype = _user.GetById(id);
            var result = new UserTypeViewModel()
            {
                IsActive=usertype.IsActive,
                UserTypeId = usertype.UserTypeId,
                UserTypeName = usertype.UserTypeName,
                Description=usertype.Description
            };
            if (result == null) return PartialView(@"Partial/_UserType", new UserTypeViewModel());

            return PartialView(@"Partial/_UserType", result);
        }
        [HttpGet, Route("_ViewUserType/{id?}")]
        public PartialViewResult _ViewUserType(int id)
        {

            if (id == 0) return PartialView(@"Partial/_UserType", new UserTypeViewModel() { IsViewMode = true });
            var usertype = _user.GetById(id);
            var result = new UserTypeViewModel()
            {
                IsActive = usertype.IsActive,
                UserTypeId = usertype.UserTypeId,
                UserTypeName = usertype.UserTypeName,
                Description = usertype.Description
            };

            if (result == null) return PartialView(@"Partial/_UserType", new UserTypeViewModel() { IsViewMode = true });
            result.IsViewMode = true;
            return PartialView(@"Partial/_UserType", result);
        }

        [HttpGet]
        public JsonResult GetUserTypeList(string query = "")
        {
            var userTypeList = _user.GetAll(x => x.IsActive && !x.IsDeleted)
                .Select(x => new KeyValuePairModel()
                {
                    Id = x.UserTypeId.ToString(),
                    Name = x.UserTypeName
                }).ToList();
            userTypeList = userTypeList.Where(x => x.Name.ToLower().Equals(query.ToLower())).ToList();
            return Json(new JsonResponse() { Status = 1, Data = userTypeList });
        }

        [Route("UserTypeDetail/{npi?}")]
        public ActionResult UserTypeDetail(string npi)
        {
            if (string.IsNullOrWhiteSpace(npi))
            {
                return RedirectToAction("UserType", "User");
            }
            var userType = _user.GetSingle(x => x.UserTypeId.ToString() == npi);
            return View(userType);
        }

        [HttpPost, Route("AddEditUserType"), ValidateAntiForgeryToken]
        public JsonResult AddEditUserType(UserTypeViewModel model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                string filePath = string.Empty;
                try
                {
                    if (ModelState.IsValid)
                    {
                        if (model.UserTypeId == 0)
                        {
                            var bResult = _user.GetSingle(x => x.UserTypeName.ToLower().Equals(model.UserTypeName.ToLower()) && x.IsActive && !x.IsDeleted);
                            if (bResult != null)
                            {
                                txscope.Dispose();
                                return Json(new JsonResponse() { Status = 0, Message = "User Type already exists. Could not add the data!" });
                            }

                            var userType = new UserType()
                            {
                                UserTypeName = model.UserTypeName,
                                Description = model.Description,
                                UserTypeId = model.UserTypeId,
                                CreatedDate = DateTime.Now,
                                IsActive = true,
                                IsDeleted = false,
                            };
                            _user.InsertData(userType);
                            _user.SaveData();
                            txscope.Complete();
                            return Json(new JsonResponse() { Status = 1, Message = "User Type save successfully" });
                        }
                        else
                        {

                            var bResult = _user.GetSingle(x => x.UserTypeId != model.UserTypeId && x.UserTypeName == model.UserTypeName && x.IsActive && !x.IsDeleted);
                            if (bResult != null)
                            {
                                txscope.Dispose();
                                return Json(new JsonResponse { Status = 0, Message = "User Type already exists. Could not add the data!" });
                            }

                            var oHour = _user.GetById(model.UserTypeId);
                            oHour.UserTypeName = model.UserTypeName;
                            oHour.Description = model.Description;
                            _user.UpdateData(oHour);
                            _user.SaveData();
                            txscope.Complete();
                            return Json(new JsonResponse { Status = 1, Message = "User Type updated successfully" });
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
                    Common.LogError(ex, "AddEditUserType-Post");
                    return Json(new JsonResponse { Status = 0, Message = "Something is wrong." });
                }
            }
        }

        [HttpPost, Route("RemoveUserType/{id?}")]
        public JsonResult RemoveFeaturedDoctor(int id)
        {
            var usertype = _user.GetById(id);
            _user.DeleteData(usertype);
            
            _user.SaveData();
            return Json(new JsonResponse() { Status = 1, Message = "User Type remove successfully" });
        }

        [HttpPost, Route("ActiveDeActiveUserType/{flag}/{id}")]
        public JsonResult ActiveDeActiveUserType(bool flag, int id)
        {
            var usertype = _user.GetById(id);
            usertype.IsActive = !flag;
            _user.UpdateData(usertype);
            _user.SaveData();
            return Json(new JsonResponse() { Status = 1, Message = $@"The User Type has been {(flag ? "deactivated" : "reactivated")} successfully" });
        }
        #endregion

        #region DrugType
        [Route("DrugType")]
        public ActionResult DrugType()
        {
            return View();
        }

        [Route("GetDrugTypeList/{flag}")]
        public ActionResult GetDrugTypeList(bool flag, JQueryDataTableParamModel param)
        {
            var allDrugType = _repo.All<DrugType_LookUp>().Where(x => !x.IsDeleted).Select(x => new DrugTypeViewModel()
            {
                Id = x.DrugType_LookUpID,
                Name = x.Name,
                Description = x.Description==null?"": x.Description,
                IsActive = x.IsActive
            }).ToList();

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                allDrugType = allDrugType.Where(x =>Convert.ToString( x.Name).ToLower().Contains(param.sSearch.ToLower())
                                                || Convert.ToString(x.Description).ToLower().Contains(param.sSearch.ToLower())
                                              ).ToList();
            }

            var sortColumnIndex = Convert.ToInt32(HttpContext.Request.QueryString["iSortCol_0"]);
            var sortDirection = HttpContext.Request.QueryString["sSortDir_0"]; // asc or desc

            Func<DrugTypeViewModel, string> orderingFunction =
                c => sortColumnIndex == 1 ? c.Name
                    : sortColumnIndex == 2 ? c.Description
                    : sortColumnIndex == 4 ? c.CreatedDate
                        : sortColumnIndex == 5 ? c.UpdatedDate
                         : c.Name;
            allDrugType = sortDirection == "asc" ? allDrugType.OrderBy(orderingFunction).ToList() : allDrugType.OrderByDescending(orderingFunction).ToList();

            var display = allDrugType.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var total = allDrugType.Count();

            return Json(new
            {
                param.sEcho,
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                aaData = display
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet, Route("_DrugType/{id?}")]
        public PartialViewResult _DrugType(int id)
        {

            if (id == 0) return PartialView(@"Partial/_DrugType", new DrugTypeViewModel());
            var DrugType = _repo.Find<DrugType_LookUp>(id);
            var result = new DrugTypeViewModel()
            {
                IsActive = DrugType.IsActive,
                Id = DrugType.DrugType_LookUpID,
                Name = DrugType.Name,
                Description = DrugType.Description
            };
            if (result == null) return PartialView(@"Partial/_DrugType", new DrugTypeViewModel());

            return PartialView(@"Partial/_DrugType", result);
        }
        [HttpGet, Route("_ViewDrugType/{id?}")]
        public PartialViewResult _ViewDrugType(int id)
        {

            if (id == 0) return PartialView(@"Partial/_DrugType", new DrugTypeViewModel());
            var DrugType = _repo.Find<DrugType_LookUp>(id);
            var result = new DrugTypeViewModel()
            {
                IsActive = DrugType.IsActive,
                Id = DrugType.DrugType_LookUpID,
                Name = DrugType.Name,
                Description = DrugType.Description,
                IsViewMode=true
            };
            if (result == null) return PartialView(@"Partial/_DrugType", new DrugTypeViewModel());

            return PartialView(@"Partial/_DrugType", result);
        }

        [HttpGet]
        public JsonResult GetDrugTypeList(string query = "")
        {
            var DrugTypeList = _repo.All<DrugType_LookUp>().Where(x => x.IsActive && !x.IsDeleted)
                .Select(x => new KeyValuePairModel()
                {
                    Id = x.DrugType_LookUpID.ToString(),
                    Name = x.Name
                }).ToList();
            DrugTypeList = DrugTypeList.Where(x => x.Name.ToLower().Equals(query.ToLower())).ToList();
            return Json(new JsonResponse() { Status = 1, Data = DrugTypeList });
        }

        [Route("DrugTypeDetail/{npi?}")]
        public ActionResult DrugTypeDetail(string npi)
        {
            if (string.IsNullOrWhiteSpace(npi))
            {
                return RedirectToAction("DrugType", "User");
            }
            var DrugType = _repo.Find<DrugType_LookUp>(x => x.DrugType_LookUpID.ToString() == npi);
            return View(DrugType);
        }

        [HttpPost, Route("AddEditDrugType"), ValidateAntiForgeryToken]
        public JsonResult AddEditDrugType(DrugTypeViewModel model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                string filePath = string.Empty;
                try
                {
                    if (ModelState.IsValid)
                    {
                        if (model.Id == 0)
                        {
                            var bResult = _repo.Find<DrugType_LookUp>(x => x.Name.ToLower().Equals(model.Name.ToLower()) && x.IsActive && !x.IsDeleted);
                            if (bResult != null)
                            {
                                txscope.Dispose();
                                return Json(new JsonResponse() { Status = 0, Message = "Drug Type Type already exists. Could not add the data!" });
                            }

                            var DrugType = new DrugType_LookUp()
                            {
                                Name = model.Name,
                                Description = model.Description,
                                DrugType_LookUpID = model.Id,
                                CreatedDate = DateTime.Now,
                                IsActive = true,
                                IsDeleted = false,
                            };
                            _repo.Insert<DrugType_LookUp>(DrugType,true);
                           // _repo.sa();
                            txscope.Complete();
                            return Json(new JsonResponse() { Status = 1, Message = "Drug Type save successfully" });
                        }
                        else
                        {

                            var bResult = _repo.Find<DrugType_LookUp>(x => x.DrugType_LookUpID != model.Id && x.Name == model.Name && x.IsActive && !x.IsDeleted);
                            if (bResult != null)
                            {
                                txscope.Dispose();
                                return Json(new JsonResponse { Status = 0, Message = "Drug Type already exists. Could not add the data!" });
                            }

                            var oHour = _repo.Find<DrugType_LookUp>(model.Id);
                            oHour.Name = model.Name;
                            oHour.Description = model.Description;
                            _repo.Update<DrugType_LookUp>(oHour,true);
                            //_user.SaveData();
                            txscope.Complete();
                            return Json(new JsonResponse { Status = 1, Message = "Drug Type updated successfully" });
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

        [HttpPost, Route("RemoveDrugType/{id?}")]
        public JsonResult RemoveDrugType(int id)
        {
            var DrugType = _repo.Find<DrugType_LookUp>(id);
            _repo.Delete<DrugType_LookUp>(DrugType);

           // _user.SaveData();
            return Json(new JsonResponse() { Status = 1, Message = "Drug Type remove successfully" });
        }

        [HttpPost, Route("ActiveDeActiveDrugType/{flag}/{id}")]
        public JsonResult ActiveDeActiveDrugType(bool flag, int id)
        {
            var DrugType = _repo.Find<DrugType_LookUp>(id);
            DrugType.IsActive = !flag;
            _repo.Update<DrugType_LookUp>(DrugType,true);
           // _user.SaveData();
            return Json(new JsonResponse() { Status = 1, Message = $@"The Drug Type has been {(flag ? "deactivated" : "reactivated")} successfully" });
        }
        #endregion

        #region AddressType
        [Route("AddressType")]
        public ActionResult AddressType()
        {
            return View();
        }

        [Route("GetAddressTypeList/{flag}")]
        public ActionResult GetAddressTypeList(bool flag, JQueryDataTableParamModel param)
        {
            var allAddressType = _repo.All<AddressType>().Where(x => !x.IsDeleted).Select(x => new AddressTypeViewModel()
            {
                Id = x.AddressTypeId,
                Name = x.Name,
                Description = x.Description == null?"":x.Description,
                IsActive = x.IsActive
            }).ToList();

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                allAddressType = allAddressType.Where(x => Convert.ToString(x.Name).ToLower().Contains(param.sSearch.ToLower())
                                                || Convert.ToString(x.Description).ToLower().Contains(param.sSearch.ToLower())
                                              ).ToList();
            }

            var sortColumnIndex = Convert.ToInt32(HttpContext.Request.QueryString["iSortCol_0"]);
            var sortDirection = HttpContext.Request.QueryString["sSortDir_0"]; // asc or desc

            Func<AddressTypeViewModel, string> orderingFunction =
                c => sortColumnIndex == 1 ? c.Name
                    : sortColumnIndex == 2 ? c.Description
                    : sortColumnIndex == 4 ? c.CreatedDate
                        : sortColumnIndex == 5 ? c.UpdatedDate
                         : c.Name;
            allAddressType = sortDirection == "asc" ? allAddressType.OrderBy(orderingFunction).ToList() : allAddressType.OrderByDescending(orderingFunction).ToList();

            var display = allAddressType.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var total = allAddressType.Count();

            return Json(new
            {
                param.sEcho,
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                aaData = display
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet, Route("_AddressType/{id?}")]
        public PartialViewResult _AddressType(int id)
        {

            if (id == 0) return PartialView(@"Partial/_AddressType", new AddressTypeViewModel());
            var AddressType = _repo.Find<AddressType>(id);
            var result = new AddressTypeViewModel()
            {
                IsActive = AddressType.IsActive,
                Id = AddressType.AddressTypeId,
                Name = AddressType.Name,
                Description = AddressType.Description
            };
            if (result == null) return PartialView(@"Partial/_AddressType", new AddressTypeViewModel());

            return PartialView(@"Partial/_AddressType", result);
        }
        [HttpGet, Route("_ViewAddressType/{id?}")]
        public PartialViewResult _ViewAddressType(int id)
        {

            if (id == 0) return PartialView(@"Partial/_AddressType", new AddressTypeViewModel());
            var AddressType = _repo.Find<AddressType>(id);
            var result = new AddressTypeViewModel()
            {
                IsActive = AddressType.IsActive,
                Id = AddressType.AddressTypeId,
                Name = AddressType.Name,
                Description = AddressType.Description,
                IsViewMode = true
            };
            if (result == null) return PartialView(@"Partial/_AddressType", new AddressTypeViewModel());

            return PartialView(@"Partial/_AddressType", result);
        }

        [HttpGet]
        public JsonResult GetAddressTypeList(string query = "")
        {
            var AddressTypeList = _repo.All<AddressType>().Where(x => x.IsActive && !x.IsDeleted)
                .Select(x => new KeyValuePairModel()
                {
                    Id = x.AddressTypeId.ToString(),
                    Name = x.Name
                }).ToList();
            AddressTypeList = AddressTypeList.Where(x => x.Name.ToLower().Equals(query.ToLower())).ToList();
            return Json(new JsonResponse() { Status = 1, Data = AddressTypeList });
        }

        [Route("AddressTypeDetail/{npi?}")]
        public ActionResult AddressTypeDetail(string npi)
        {
            if (string.IsNullOrWhiteSpace(npi))
            {
                return RedirectToAction("AddressType", "User");
            }
            var AddressType = _repo.Find<AddressType>(x => x.AddressTypeId.ToString() == npi);
            return View(AddressType);
        }

        [HttpPost, Route("AddEditAddressType"), ValidateAntiForgeryToken]
        public JsonResult AddEditAddressType(AddressTypeViewModel model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                string filePath = string.Empty;
                try
                {
                    if (ModelState.IsValid)
                    {
                        if (model.Id == 0)
                        {
                            var bResult = _repo.Find<AddressType>(x => x.Name.ToLower().Equals(model.Name.ToLower()) && x.IsActive && !x.IsDeleted);
                            if (bResult != null)
                            {
                                txscope.Dispose();
                                return Json(new JsonResponse() { Status = 0, Message = "Address Type Type already exists. Could not add the data!" });
                            }

                            var AddressType = new AddressType()
                            {
                                Name = model.Name,
                                Description = model.Description,
                                AddressTypeId = model.Id,
                                CreatedDate = DateTime.Now,
                                IsActive = true,
                                IsDeleted = false,
                            };
                            _repo.Insert<AddressType>(AddressType, true);
                            // _repo.sa();
                            txscope.Complete();
                            return Json(new JsonResponse() { Status = 1, Message = "Address Type save successfully" });
                        }
                        else
                        {

                            var bResult = _repo.Find<AddressType>(x => x.AddressTypeId != model.Id && x.Name == model.Name && x.IsActive && !x.IsDeleted);
                            if (bResult != null)
                            {
                                txscope.Dispose();
                                return Json(new JsonResponse { Status = 0, Message = "Address Type already exists. Could not add the data!" });
                            }

                            var oHour = _repo.Find<AddressType>(model.Id);
                            oHour.Name = model.Name;
                            oHour.Description = model.Description;
                            _repo.Update<AddressType>(oHour, true);
                            //_user.SaveData();
                            txscope.Complete();
                            return Json(new JsonResponse { Status = 1, Message = "Address Type updated successfully" });
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
                    Common.LogError(ex, "AddEditAddressType-Post");
                    return Json(new JsonResponse { Status = 0, Message = "Something is wrong." });
                }
            }
        }

        [HttpPost, Route("RemoveAddressType/{id?}")]
        public JsonResult RemoveAddressType(int id)
        {
            var AddressType = _repo.Find<AddressType>(id);
            _repo.Delete<AddressType>(AddressType);

            // _user.SaveData();
            return Json(new JsonResponse() { Status = 1, Message = "Address Type remove successfully" });
        }

        [HttpPost, Route("ActiveDeActiveAddressType/{flag}/{id}")]
        public JsonResult ActiveDeActiveAddressType(bool flag, int id)
        {
            var AddressType = _repo.Find<AddressType>(id);
            AddressType.IsActive = !flag;
            _repo.Update<AddressType>(AddressType, true);
            // _user.SaveData();
            return Json(new JsonResponse() { Status = 1, Message = $@"TheBoard Certifications has been {(flag ? "deactivated" : "reactivated")} successfully" });
        }
        #endregion

        #region AmenityOption
        [Route("AmenityOption")]
        public ActionResult AmenityOption()
        {
            return View();
        }

        [Route("GetAmenityOptionList/{flag}")]
        public ActionResult GetAmenityOptionList(bool flag, JQueryDataTableParamModel param)
        {
            var allAmenityOption = _repo.All<AmenityOption>().Where(x => !x.IsDeleted).Select(x => new AmenityOptionViewModel()
            {
                Id = x.AmenityId,
                Name = x.Name,
                Description = x.Description==null?"": x.Description,
                IsActive = x.IsActive,
                IsOption=x.IsOption
            }).ToList();

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                allAmenityOption = allAmenityOption.Where(x => Convert.ToString(x.Name).ToLower().Contains(param.sSearch.ToLower())
                                                || Convert.ToString(x.Description).ToLower().Contains(param.sSearch.ToLower())
                                               ).ToList();
            }

            var sortColumnIndex = Convert.ToInt32(HttpContext.Request.QueryString["iSortCol_0"]);
            var sortDirection = HttpContext.Request.QueryString["sSortDir_0"]; // asc or desc

            Func<AmenityOptionViewModel, string> orderingFunction =
                c => sortColumnIndex == 1 ? c.Name
                    : sortColumnIndex == 2 ? c.Description
                    : sortColumnIndex == 4 ? c.CreatedDate
                        : sortColumnIndex == 5 ? c.UpdatedDate
                         : c.Name;
            allAmenityOption = sortDirection == "asc" ? allAmenityOption.OrderBy(orderingFunction).ToList() : allAmenityOption.OrderByDescending(orderingFunction).ToList();

            var display = allAmenityOption.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var total = allAmenityOption.Count();

            return Json(new
            {
                param.sEcho,
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                aaData = display
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet, Route("_AmenityOption/{id?}")]
        public PartialViewResult _AmenityOption(int id)
        {

            if (id == 0) return PartialView(@"Partial/_AmenityOption", new AmenityOptionViewModel());
            var AmenityOption = _repo.Find<AmenityOption>(id);
            var result = new AmenityOptionViewModel()
            {
                IsActive = AmenityOption.IsActive,
                Id = AmenityOption.AmenityId,
                Name = AmenityOption.Name,
                Description = AmenityOption.Description
            };
            if (result == null) return PartialView(@"Partial/_AmenityOption", new AmenityOptionViewModel());

            return PartialView(@"Partial/_AmenityOption", result);
        }
        [HttpGet, Route("_ViewAmenityOption/{id?}")]
        public PartialViewResult _ViewAmenityOption(int id)
        {

            if (id == 0) return PartialView(@"Partial/_AmenityOption", new AmenityOptionViewModel());
            var AmenityOption = _repo.Find<AmenityOption>(id);
            var result = new AmenityOptionViewModel()
            {
                IsActive = AmenityOption.IsActive,
                Id = AmenityOption.AmenityId,
                Name = AmenityOption.Name,
                Description = AmenityOption.Description,
                IsViewMode = true
            };
            if (result == null) return PartialView(@"Partial/_AmenityOption", new AmenityOptionViewModel());

            return PartialView(@"Partial/_AmenityOption", result);
        }

        [HttpGet]
        public JsonResult GetAmenityOptionList(string query = "")
        {
            var AmenityOptionList = _repo.All<AmenityOption>().Where(x => x.IsActive && !x.IsDeleted)
                .Select(x => new KeyValuePairModel()
                {
                    Id = x.AmenityId.ToString(),
                    Name = x.Name
                }).ToList();
            AmenityOptionList = AmenityOptionList.Where(x => x.Name.ToLower().Equals(query.ToLower())).ToList();
            return Json(new JsonResponse() { Status = 1, Data = AmenityOptionList });
        }

        [Route("AmenityOptionDetail/{npi?}")]
        public ActionResult AmenityOptionDetail(string npi)
        {
            if (string.IsNullOrWhiteSpace(npi))
            {
                return RedirectToAction("AmenityOption", "User");
            }
            var AmenityOption = _repo.Find<AmenityOption>(x => x.AmenityId.ToString() == npi);
            return View(AmenityOption);
        }

        [HttpPost, Route("AddEditAmenityOption"), ValidateAntiForgeryToken]
        public JsonResult AddEditAmenityOption(AmenityOptionViewModel model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                string filePath = string.Empty;
                try
                {
                    if (ModelState.IsValid)
                    {
                        if (model.Id == 0)
                        {
                            var bResult = _repo.Find<AmenityOption>(x => x.Name.ToLower().Equals(model.Name.ToLower()) && x.IsActive && !x.IsDeleted);
                            if (bResult != null)
                            {
                                txscope.Dispose();
                                return Json(new JsonResponse() { Status = 0, Message = "Amenity Option Option already exists. Could not add the data!" });
                            }

                            var AmenityOption = new AmenityOption()
                            {
                                Name = model.Name,
                                Description = model.Description,
                                AmenityId = model.Id,
                                CreatedDate = DateTime.Now,
                                IsActive = true,
                                IsDeleted = false,
                                IsOption=true
                            };
                            _repo.Insert<AmenityOption>(AmenityOption, true);
                            // _repo.sa();
                            txscope.Complete();
                            return Json(new JsonResponse() { Status = 1, Message = "Amenity Option save successfully" });
                        }
                        else
                        {

                            var bResult = _repo.Find<AmenityOption>(x => x.AmenityId != model.Id && x.Name == model.Name && x.IsActive && !x.IsDeleted);
                            if (bResult != null)
                            {
                                txscope.Dispose();
                                return Json(new JsonResponse { Status = 0, Message = "Amenity Option already exists. Could not add the data!" });
                            }

                            var oHour = _repo.Find<AmenityOption>(model.Id);
                            oHour.Name = model.Name;
                            oHour.Description = model.Description;
                            _repo.Update<AmenityOption>(oHour, true);
                            //_user.SaveData();
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
                    Common.LogError(ex, "AddEditAmenityOption-Post");
                    return Json(new JsonResponse { Status = 0, Message = "Something is wrong." });
                }
            }
        }

        [HttpPost, Route("RemoveAmenityOption/{id?}")]
        public JsonResult RemoveAmenityOption(int id)
        {
            var AmenityOption = _repo.Find<AmenityOption>(id);
            _repo.Delete<AmenityOption>(AmenityOption);

            // _user.SaveData();
            return Json(new JsonResponse() { Status = 1, Message = "Amenity Option remove successfully" });
        }

        [HttpPost, Route("ActiveDeActiveAmenityOption/{flag}/{id}")]
        public JsonResult ActiveDeActiveAmenityOption(bool flag, int id)
        {
            var AmenityOption = _repo.Find<AmenityOption>(id);
            AmenityOption.IsActive = !flag;
            _repo.Update<AmenityOption>(AmenityOption, true);
            // _user.SaveData();
            return Json(new JsonResponse() { Status = 1, Message = $@"The Amenity Option has been {(flag ? "deactivated" : "reactivated")} successfully" });
        }
        [HttpPost, Route("ActiveDeActiveOption/{flag}/{id}")]
        public JsonResult ActiveDeActiveOption(bool flag, int id)
        {
            var AmenityOption = _repo.Find<AmenityOption>(id);
            AmenityOption.IsOption = !flag;
            _repo.Update<AmenityOption>(AmenityOption, true);
            // _user.SaveData();
            return Json(new JsonResponse() { Status = 1, Message = $@"The Amenity Option has been {(flag ? "deactivated" : "reactivated")} successfully" });
        }
        #endregion

        #region InsuranceType
        [Route("InsuranceType")]
        public ActionResult InsuranceType()
        {
            return View();
        }

        [Route("GetInsuranceTypeList/{flag}")]
        public ActionResult GetInsuranceTypeList(bool flag, JQueryDataTableParamModel param)
        {
            var allInsuranceType = _repo.All<InsuranceType>().Where(x => !x.IsDeleted).Select(x => new InsuranceTypeViewModel()
            {
                Id = x.InsuranceTypeId,
                Name = x.Name,
                Description = x.Description==null?"": x.Description,
                IsActive = x.IsActive
            }).ToList();

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                allInsuranceType = allInsuranceType.Where(x => Convert.ToString(x.Name).ToString().ToLower().Contains(param.sSearch.ToLower())
                                                || Convert.ToString(x.Description).ToLower().Contains(param.sSearch.ToLower())
                                                ).ToList();
            }

            var sortColumnIndex = Convert.ToInt32(HttpContext.Request.QueryString["iSortCol_0"]);
            var sortDirection = HttpContext.Request.QueryString["sSortDir_0"]; // asc or desc

            Func<InsuranceTypeViewModel, string> orderingFunction =
                c => sortColumnIndex == 1 ? c.Name
                    : sortColumnIndex == 2 ? c.Description
                    : sortColumnIndex == 4 ? c.CreatedDate
                        : sortColumnIndex == 5 ? c.UpdatedDate
                         : c.Name;
            allInsuranceType = sortDirection == "asc" ? allInsuranceType.OrderBy(orderingFunction).ToList() : allInsuranceType.OrderByDescending(orderingFunction).ToList();

            var display = allInsuranceType.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var total = allInsuranceType.Count();

            return Json(new
            {
                param.sEcho,
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                aaData = display
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet, Route("_InsuranceType/{id?}")]
        public PartialViewResult _InsuranceType(int id)
        {

            if (id == 0) return PartialView(@"Partial/_InsuranceType", new InsuranceTypeViewModel());
            var InsuranceType = _repo.Find<InsuranceType>(id);
            var result = new InsuranceTypeViewModel()
            {
                IsActive = InsuranceType.IsActive,
                Id = InsuranceType.InsuranceTypeId,
                Name = InsuranceType.Name,
                Description = InsuranceType.Description
            };
            if (result == null) return PartialView(@"Partial/_InsuranceType", new InsuranceTypeViewModel());

            return PartialView(@"Partial/_InsuranceType", result);
        }
        [HttpGet, Route("_ViewInsuranceType/{id?}")]
        public PartialViewResult _ViewInsuranceType(int id)
        {

            if (id == 0) return PartialView(@"Partial/_InsuranceType", new InsuranceTypeViewModel());
            var InsuranceType = _repo.Find<InsuranceType>(id);
            var result = new InsuranceTypeViewModel()
            {
                IsActive = InsuranceType.IsActive,
                Id = InsuranceType.InsuranceTypeId,
                Name = InsuranceType.Name,
                Description = InsuranceType.Description,
                IsViewMode = true
            };
            if (result == null) return PartialView(@"Partial/_InsuranceType", new InsuranceTypeViewModel());

            return PartialView(@"Partial/_InsuranceType", result);
        }

        [HttpGet]
        public JsonResult GetInsuranceTypeList(string query = "")
        {
            var InsuranceTypeList = _repo.All<InsuranceType>().Where(x => x.IsActive && !x.IsDeleted)
                .Select(x => new KeyValuePairModel()
                {
                    Id = x.InsuranceTypeId.ToString(),
                    Name = x.Name
                }).ToList();
            InsuranceTypeList = InsuranceTypeList.Where(x => x.Name.ToLower().Equals(query.ToLower())).ToList();
            return Json(new JsonResponse() { Status = 1, Data = InsuranceTypeList });
        }

        [Route("InsuranceTypeDetail/{npi?}")]
        public ActionResult InsuranceTypeDetail(string npi)
        {
            if (string.IsNullOrWhiteSpace(npi))
            {
                return RedirectToAction("InsuranceType", "User");
            }
            var InsuranceType = _repo.Find<InsuranceType>(x => x.InsuranceTypeId.ToString() == npi);
            return View(InsuranceType);
        }

        [HttpPost, Route("AddEditInsuranceType"), ValidateAntiForgeryToken]
        public JsonResult AddEditInsuranceType(InsuranceTypeViewModel model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                string filePath = string.Empty;
                try
                {
                    if (ModelState.IsValid)
                    {
                        if (model.Id == 0)
                        {
                            var bResult = _repo.Find<InsuranceType>(x => x.Name.ToLower().Equals(model.Name.ToLower()) && x.IsActive && !x.IsDeleted);
                            if (bResult != null)
                            {
                                txscope.Dispose();
                                return Json(new JsonResponse() { Status = 0, Message = "Insurance Type Type already exists. Could not add the data!" });
                            }

                            var InsuranceType = new InsuranceType()
                            {
                                Name = model.Name,
                                Description = model.Description,
                                InsuranceTypeId = model.Id,
                                CreatedDate = DateTime.Now,
                                IsActive = true,
                                IsDeleted = false,
                            };
                            _repo.Insert<InsuranceType>(InsuranceType, true);
                            // _repo.sa();
                            txscope.Complete();
                            return Json(new JsonResponse() { Status = 1, Message = "Insurance Type save successfully" });
                        }
                        else
                        {

                            var bResult = _repo.Find<InsuranceType>(x => x.InsuranceTypeId != model.Id && x.Name == model.Name && x.IsActive && !x.IsDeleted);
                            if (bResult != null)
                            {
                                txscope.Dispose();
                                return Json(new JsonResponse { Status = 0, Message = "Insurance Type already exists. Could not add the data!" });
                            }

                            var oHour = _repo.Find<InsuranceType>(model.Id);
                            oHour.Name = model.Name;
                            oHour.Description = model.Description;
                            _repo.Update<InsuranceType>(oHour, true);
                            //_user.SaveData();
                            txscope.Complete();
                            return Json(new JsonResponse { Status = 1, Message = "Insurance Type updated successfully" });
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
                    Common.LogError(ex, "AddEditInsuranceType-Post");
                    return Json(new JsonResponse { Status = 0, Message = "Something is wrong." });
                }
            }
        }

        [HttpPost, Route("RemoveInsuranceType/{id?}")]
        public JsonResult RemoveInsuranceType(int id)
        {
            var InsuranceType = _repo.Find<InsuranceType>(id);
            _repo.Delete<InsuranceType>(InsuranceType);

            // _user.SaveData();
            return Json(new JsonResponse() { Status = 1, Message = "Insurance Type remove successfully" });
        }

        [HttpPost, Route("ActiveDeActiveInsuranceType/{flag}/{id}")]
        public JsonResult ActiveDeActiveInsuranceType(bool flag, int id)
        {
            var InsuranceType = _repo.Find<InsuranceType>(id);
            InsuranceType.IsActive = !flag;
            _repo.Update<InsuranceType>(InsuranceType, true);
            // _user.SaveData();
            return Json(new JsonResponse() { Status = 1, Message = $@"The Insurance Type has been {(flag ? "deactivated" : "reactivated")} successfully" });
        }
        #endregion

        
        #region GenderTypes
        [Route("GenderType")]
        public ActionResult GenderType()
        {
            return View();
        }

        [Route("GetGenderTypeList/{flag}")]
        public ActionResult GetGenderTypeList(bool flag, JQueryDataTableParamModel param)
        {
            var allGenderType = _repo.All<GenderType>().Where(x => !x.IsDeleted).Select(x => new GenderTypeViewModel()
            {
                Id = x.GenderTypeId,
                Name = x.Name,
                Description = x.Description==null?"": x.Description,
                IsActive = x.IsActive
            }).ToList();

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                allGenderType = allGenderType.Where(x => Convert.ToString(x.Name).ToLower().Contains(param.sSearch.ToLower())
                                                || Convert.ToString(x.Description).ToLower().Contains(param.sSearch.ToLower())
                                               ).ToList();
            }

            var sortColumnIndex = Convert.ToInt32(HttpContext.Request.QueryString["iSortCol_0"]);
            var sortDirection = HttpContext.Request.QueryString["sSortDir_0"]; // asc or desc

            Func<GenderTypeViewModel, string> orderingFunction =
                c => sortColumnIndex == 1 ? c.Name
                    : sortColumnIndex == 2 ? c.Description
                    : sortColumnIndex == 4 ? c.CreatedDate
                        : sortColumnIndex == 5 ? c.UpdatedDate
                         : c.Name;
            allGenderType = sortDirection == "asc" ? allGenderType.OrderBy(orderingFunction).ToList() : allGenderType.OrderByDescending(orderingFunction).ToList();

            var display = allGenderType.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var total = allGenderType.Count();

            return Json(new
            {
                param.sEcho,
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                aaData = display
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet, Route("_GenderType/{id?}")]
        public PartialViewResult _GenderType(int id)
        {

            if (id == 0) return PartialView(@"Partial/_GenderType", new GenderTypeViewModel());
            var GenderType = _repo.Find<GenderType>(id);
            var result = new GenderTypeViewModel()
            {
                IsActive = GenderType.IsActive,
                Id = GenderType.GenderTypeId,
                Name = GenderType.Name,
                Description = GenderType.Description
            };
            if (result == null) return PartialView(@"Partial/_GenderType", new GenderTypeViewModel());

            return PartialView(@"Partial/_GenderType", result);
        }
        [HttpGet, Route("_ViewGenderType/{id?}")]
        public PartialViewResult _ViewGenderType(int id)
        {

            if (id == 0) return PartialView(@"Partial/_GenderType", new GenderTypeViewModel());
            var GenderType = _repo.Find<GenderType>(id);
            var result = new GenderTypeViewModel()
            {
                IsActive = GenderType.IsActive,
                Id = GenderType.GenderTypeId,
                Name = GenderType.Name,
                Description = GenderType.Description,
                IsViewMode = true
            };
            if (result == null) return PartialView(@"Partial/_GenderType", new GenderTypeViewModel());

            return PartialView(@"Partial/_GenderType", result);
        }

        [HttpGet]
        public JsonResult GetGenderTypeList(string query = "")
        {
            var GenderTypeList = _repo.All<GenderType>().Where(x => x.IsActive && !x.IsDeleted)
                .Select(x => new KeyValuePairModel()
                {
                    Id = x.GenderTypeId.ToString(),
                    Name = x.Name
                }).ToList();
            GenderTypeList = GenderTypeList.Where(x => x.Name.ToLower().Equals(query.ToLower())).ToList();
            return Json(new JsonResponse() { Status = 1, Data = GenderTypeList });
        }

        [Route("GenderTypeDetail/{npi?}")]
        public ActionResult GenderTypeDetail(string npi)
        {
            if (string.IsNullOrWhiteSpace(npi))
            {
                return RedirectToAction("GenderType", "User");
            }
            var GenderType = _repo.Find<GenderType>(x => x.GenderTypeId.ToString() == npi);
            return View(GenderType);
        }

        [HttpPost, Route("AddEditGenderType"), ValidateAntiForgeryToken]
        public JsonResult AddEditGenderType(GenderTypeViewModel model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                string filePath = string.Empty;
                try
                {
                    if (ModelState.IsValid)
                    {
                        if (model.Id == 0)
                        {
                            var bResult = _repo.Find<GenderType>(x => x.Name.ToLower().Equals(model.Name.ToLower()) && x.IsActive && !x.IsDeleted);
                            if (bResult != null)
                            {
                                txscope.Dispose();
                                return Json(new JsonResponse() { Status = 0, Message = "Gender Type already exists. Could not add the data!" });
                            }

                            var GenderType = new GenderType()
                            {
                                Name = model.Name,
                                Description = model.Description,
                                GenderTypeId = model.Id,
                                CreatedDate = DateTime.Now,
                                IsActive = true,
                                IsDeleted = false,
                            };
                            _repo.Insert<GenderType>(GenderType, true);
                            // _repo.sa();
                            txscope.Complete();
                            return Json(new JsonResponse() { Status = 1, Message = "Gender Type save successfully" });
                        }
                        else
                        {

                            var bResult = _repo.Find<GenderType>(x => x.GenderTypeId != model.Id && x.Name == model.Name && x.IsActive && !x.IsDeleted);
                            if (bResult != null)
                            {
                                txscope.Dispose();
                                return Json(new JsonResponse { Status = 0, Message = "Gender Type  already exists. Could not add the data!" });
                            }

                            var oHour = _repo.Find<GenderType>(model.Id);
                            oHour.Name = model.Name;
                            oHour.Description = model.Description;
                            _repo.Update<GenderType>(oHour, true);
                            //_user.SaveData();
                            txscope.Complete();
                            return Json(new JsonResponse { Status = 1, Message = "Gender Type  updated successfully" });
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
                    Common.LogError(ex, "AddEditGenderType-Post");
                    return Json(new JsonResponse { Status = 0, Message = "Something is wrong." });
                }
            }
        }

        [HttpPost, Route("RemoveGenderType/{id?}")]
        public JsonResult RemoveGenderType(int id)
        {
            var GenderType = _repo.Find<GenderType>(id);
            _repo.Delete<GenderType>(GenderType);

            // _user.SaveData();
            return Json(new JsonResponse() { Status = 1, Message = "Gender Type  remove successfully" });
        }

        [HttpPost, Route("ActiveDeActiveGenderType/{flag}/{id}")]
        public JsonResult ActiveDeActiveGenderType(bool flag, int id)
        {
            var GenderType = _repo.Find<GenderType>(id);
            GenderType.IsActive = !flag;
            _repo.Update<GenderType>(GenderType, true);
            // _user.SaveData();
            return Json(new JsonResponse() { Status = 1, Message = $@"The Gender Type  has been {(flag ? "deactivated" : "reactivated")} successfully" });
        }
        #endregion

        #region PaymentType
        [Route("PaymentType")]
        public ActionResult PaymentType()
        {
            return View();
        }

        [Route("GetPaymentTypeList/{flag}")]
        public ActionResult GetPaymentTypeList(bool flag, JQueryDataTableParamModel param)
        {
            var allPaymentType = _repo.All<PaymentType>().Where(x => !x.IsDeleted).Select(x => new PaymentTypeViewModel()
            {
                Id = x.PaymentTypeId,
                Name = x.Name,
                Description = x.Description==null?"": x.Description,
                IsActive = x.IsActive
            }).ToList();

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                allPaymentType = allPaymentType.Where(x => Convert.ToString(x.Name).ToLower().Contains(param.sSearch.ToLower())
                                                || Convert.ToString(x.Description).ToLower().Contains(param.sSearch.ToLower())
                                             ).ToList();
            }

            var sortColumnIndex = Convert.ToInt32(HttpContext.Request.QueryString["iSortCol_0"]);
            var sortDirection = HttpContext.Request.QueryString["sSortDir_0"]; // asc or desc

            Func<PaymentTypeViewModel, string> orderingFunction =
                c => sortColumnIndex == 1 ? c.Name
                    : sortColumnIndex == 2 ? c.Description
                    : sortColumnIndex == 4 ? c.CreatedDate
                        : sortColumnIndex == 5 ? c.UpdatedDate
                         : c.Name;
            allPaymentType = sortDirection == "asc" ? allPaymentType.OrderBy(orderingFunction).ToList() : allPaymentType.OrderByDescending(orderingFunction).ToList();

            var display = allPaymentType.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var total = allPaymentType.Count();

            return Json(new
            {
                param.sEcho,
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                aaData = display
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet, Route("_PaymentType/{id?}")]
        public PartialViewResult _PaymentType(int id)
        {

            if (id == 0) return PartialView(@"Partial/_PaymentType", new PaymentTypeViewModel());
            var PaymentType = _repo.Find<PaymentType>(id);
            var result = new PaymentTypeViewModel()
            {
                IsActive = PaymentType.IsActive,
                Id = PaymentType.PaymentTypeId,
                Name = PaymentType.Name,
                Description = PaymentType.Description
            };
            if (result == null) return PartialView(@"Partial/_PaymentType", new PaymentTypeViewModel());

            return PartialView(@"Partial/_PaymentType", result);
        }
        [HttpGet, Route("_ViewPaymentType/{id?}")]
        public PartialViewResult _ViewPaymentType(int id)
        {

            if (id == 0) return PartialView(@"Partial/_PaymentType", new PaymentTypeViewModel());
            var PaymentType = _repo.Find<PaymentType>(id);
            var result = new PaymentTypeViewModel()
            {
                IsActive = PaymentType.IsActive,
                Id = PaymentType.PaymentTypeId,
                Name = PaymentType.Name,
                Description = PaymentType.Description,
                IsViewMode = true
            };
            if (result == null) return PartialView(@"Partial/_PaymentType", new PaymentTypeViewModel());

            return PartialView(@"Partial/_PaymentType", result);
        }

        [HttpGet]
        public JsonResult GetPaymentTypeList(string query = "")
        {
            var PaymentTypeList = _repo.All<PaymentType>().Where(x => x.IsActive && !x.IsDeleted)
                .Select(x => new KeyValuePairModel()
                {
                    Id = x.PaymentTypeId.ToString(),
                    Name = x.Name
                }).ToList();
            PaymentTypeList = PaymentTypeList.Where(x => x.Name.ToLower().Equals(query.ToLower())).ToList();
            return Json(new JsonResponse() { Status = 1, Data = PaymentTypeList });
        }

        [Route("PaymentTypeDetail/{npi?}")]
        public ActionResult PaymentTypeDetail(string npi)
        {
            if (string.IsNullOrWhiteSpace(npi))
            {
                return RedirectToAction("PaymentType", "User");
            }
            var PaymentType = _repo.Find<PaymentType>(x => x.PaymentTypeId.ToString() == npi);
            return View(PaymentType);
        }

        [HttpPost, Route("AddEditPaymentType"), ValidateAntiForgeryToken]
        public JsonResult AddEditPaymentType(PaymentTypeViewModel model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                string filePath = string.Empty;
                try
                {
                    if (ModelState.IsValid)
                    {
                        if (model.Id == 0)
                        {
                            var bResult = _repo.Find<PaymentType>(x => x.Name.ToLower().Equals(model.Name.ToLower()) && x.IsActive && !x.IsDeleted);
                            if (bResult != null)
                            {
                                txscope.Dispose();
                                return Json(new JsonResponse() { Status = 0, Message = "Payment Type Type already exists. Could not add the data!" });
                            }
                            var obj = _repo.All<PaymentType>();
                            var count = 0;
                            if (obj!=null)
                            {
                                count =   obj.AsEnumerable().LastOrDefault().PaymentTypeId + 1;
                            }
                            else
                            {
                                count = 1;
                            }
                            
                                var PaymentType = new PaymentType()
                            {
                                Name = model.Name,
                                Description = model.Description,
                                PaymentTypeId = count,
                                CreatedDate = DateTime.Now,
                                IsActive = true,
                                IsDeleted = false,
                            };
                            _repo.Insert<PaymentType>(PaymentType, true);
                            // _repo.sa();
                            txscope.Complete();
                            return Json(new JsonResponse() { Status = 1, Message = "Payment Type save successfully" });
                        }
                        else
                        {

                            var bResult = _repo.Find<PaymentType>(x => x.PaymentTypeId != model.Id && x.Name == model.Name && x.IsActive && !x.IsDeleted);
                            if (bResult != null)
                            {
                                txscope.Dispose();
                                return Json(new JsonResponse { Status = 0, Message = "Payment Type already exists. Could not add the data!" });
                            }

                            var oHour = _repo.Find<PaymentType>(model.Id);
                            oHour.Name = model.Name;
                            oHour.Description = model.Description;
                            _repo.Update<PaymentType>(oHour, true);
                            //_user.SaveData();
                            txscope.Complete();
                            return Json(new JsonResponse { Status = 1, Message = "Payment Type updated successfully" });
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
                    Common.LogError(ex, "AddEditPaymentType-Post");
                    return Json(new JsonResponse { Status = 0, Message = "Something is wrong." });
                }
            }
        }

        [HttpPost, Route("RemovePaymentType/{id?}")]
        public JsonResult RemovePaymentType(int id)
        {
            var PaymentType = _repo.Find<PaymentType>(id);
            _repo.Delete<PaymentType>(PaymentType);

            // _user.SaveData();
            return Json(new JsonResponse() { Status = 1, Message = "Payment Type remove successfully" });
        }

        [HttpPost, Route("ActiveDeActivePaymentType/{flag}/{id}")]
        public JsonResult ActiveDeActivePaymentType(bool flag, int id)
        {
            var PaymentType = _repo.Find<PaymentType>(id);
            PaymentType.IsActive = !flag;
            _repo.Update<PaymentType>(PaymentType, true);
            // _user.SaveData();
            return Json(new JsonResponse() { Status = 1, Message = $@"The Payment Type has been {(flag ? "deactivated" : "reactivated")} successfully" });
        }
        #endregion

        #region DrugSymptoms
        [Route("DrugSymptoms")]
        public ActionResult DrugSymptoms()
        {
            return View();
        }

        [Route("GetDrugSymptomsList/{flag}")]
        public ActionResult GetDrugSymptomsList(bool flag, JQueryDataTableParamModel param)
        {
            var allDrugSymptoms = _repo.All<DrugSymptoms_LookUp>().Where(x => !x.IsDeleted).Select(x => new DrugSymptomsViewModel()
            {
                Id = x.DrugSymptoms_LookUpID,
                Name = x.Name,
                Description = x.Description!=null ? x.Description:"",
                IsActive = x.IsActive
            }).ToList();

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                allDrugSymptoms = allDrugSymptoms.Where(x => Convert.ToString(x.Name).ToLower().Contains(param.sSearch.ToLower())
                                                || Convert.ToString(x.Description).ToLower().Contains(param.sSearch.ToLower())
                                               ).ToList();
            }

            var sortColumnIndex = Convert.ToInt32(HttpContext.Request.QueryString["iSortCol_0"]);
            var sortDirection = HttpContext.Request.QueryString["sSortDir_0"]; // asc or desc

            Func<DrugSymptomsViewModel, string> orderingFunction =
                c => sortColumnIndex == 1 ? c.Name
                    : sortColumnIndex == 2 ? c.Description
                    : sortColumnIndex == 4 ? c.CreatedDate
                        : sortColumnIndex == 5 ? c.UpdatedDate
                         : c.Name;
            allDrugSymptoms = sortDirection == "asc" ? allDrugSymptoms.OrderBy(orderingFunction).ToList() : allDrugSymptoms.OrderByDescending(orderingFunction).ToList();

            var display = allDrugSymptoms.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var total = allDrugSymptoms.Count();

            return Json(new
            {
                param.sEcho,
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                aaData = display
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet, Route("_DrugSymptoms/{id?}")]
        public PartialViewResult _DrugSymptoms(int id)
        {

            if (id == 0) return PartialView(@"Partial/_DrugSymptoms", new DrugSymptomsViewModel());
            var DrugSymptoms = _repo.Find<DrugSymptoms_LookUp>(id);
            var result = new DrugSymptomsViewModel()
            {
                IsActive = DrugSymptoms.IsActive,
                Id = DrugSymptoms.DrugSymptoms_LookUpID,
                Name = DrugSymptoms.Name,
                Description = DrugSymptoms.Description
            };
            if (result == null) return PartialView(@"Partial/_DrugSymptoms", new DrugSymptomsViewModel());

            return PartialView(@"Partial/_DrugSymptoms", result);
        }
        [HttpGet, Route("_ViewDrugSymptoms/{id?}")]
        public PartialViewResult _ViewDrugSymptoms(int id)
        {

            if (id == 0) return PartialView(@"Partial/_DrugSymptoms", new DrugSymptomsViewModel());
            var DrugSymptoms = _repo.Find<DrugSymptoms_LookUp>(id);
            var result = new DrugSymptomsViewModel()
            {
                IsActive = DrugSymptoms.IsActive,
                Id = DrugSymptoms.DrugSymptoms_LookUpID,
                Name = DrugSymptoms.Name,
                Description = DrugSymptoms.Description,
                IsViewMode = true
            };
            if (result == null) return PartialView(@"Partial/_DrugSymptoms", new DrugSymptomsViewModel());

            return PartialView(@"Partial/_DrugSymptoms", result);
        }

        [HttpGet]
        public JsonResult GetDrugSymptomsList(string query = "")
        {
            var DrugSymptomsList = _repo.All<DrugSymptoms_LookUp>().Where(x => x.IsActive && !x.IsDeleted)
                .Select(x => new KeyValuePairModel()
                {
                    Id = x.DrugSymptoms_LookUpID.ToString(),
                    Name = x.Name
                }).ToList();
            DrugSymptomsList = DrugSymptomsList.Where(x => x.Name.ToLower().Equals(query.ToLower())).ToList();
            return Json(new JsonResponse() { Status = 1, Data = DrugSymptomsList });
        }

        [Route("DrugSymptomsDetail/{npi?}")]
        public ActionResult DrugSymptomsDetail(string npi)
        {
            if (string.IsNullOrWhiteSpace(npi))
            {
                return RedirectToAction("DrugSymptoms", "User");
            }
            var DrugSymptoms = _repo.Find<DrugSymptoms_LookUp>(x => x.DrugSymptoms_LookUpID.ToString() == npi);
            return View(DrugSymptoms);
        }

        [HttpPost, Route("AddEditDrugSymptoms"), ValidateAntiForgeryToken]
        public JsonResult AddEditDrugSymptoms(DrugSymptomsViewModel model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                string filePath = string.Empty;
                try
                {
                    if (ModelState.IsValid)
                    {
                        if (model.Id == 0)
                        {
                            var bResult = _repo.Find<DrugSymptoms_LookUp>(x => x.Name.ToLower().Equals(model.Name.ToLower()) && x.IsActive && !x.IsDeleted);
                            if (bResult != null)
                            {
                                txscope.Dispose();
                                return Json(new JsonResponse() { Status = 0, Message = "Drug Symptoms  already exists. Could not add the data!" });
                            }

                            var DrugSymptoms = new DrugSymptoms_LookUp()
                            {
                                Name = model.Name,
                                Description = model.Description,
                                DrugSymptoms_LookUpID = model.Id,
                                CreatedDate = DateTime.Now,
                                IsActive = true,
                                IsDeleted = false,
                            };
                            _repo.Insert<DrugSymptoms_LookUp>(DrugSymptoms, true);
                            // _repo.sa();
                            txscope.Complete();
                            return Json(new JsonResponse() { Status = 1, Message = "Drug Symptoms save successfully" });
                        }
                        else
                        {

                            var bResult = _repo.Find<DrugSymptoms_LookUp>(x => x.DrugSymptoms_LookUpID != model.Id && x.Name == model.Name && x.IsActive && !x.IsDeleted);
                            if (bResult != null)
                            {
                                txscope.Dispose();
                                return Json(new JsonResponse { Status = 0, Message = "Drug Symptoms already exists. Could not add the data!" });
                            }

                            var oHour = _repo.Find<DrugSymptoms_LookUp>(model.Id);
                            oHour.Name = model.Name;
                            oHour.Description = model.Description;
                            _repo.Update<DrugSymptoms_LookUp>(oHour, true);
                            //_user.SaveData();
                            txscope.Complete();
                            return Json(new JsonResponse { Status = 1, Message = "Drug Symptoms updated successfully" });
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
                    Common.LogError(ex, "AddEditDrugSymptoms-Post");
                    return Json(new JsonResponse { Status = 0, Message = "Something is wrong." });
                }
            }
        }

        [HttpPost, Route("RemoveDrugSymptoms/{id?}")]
        public JsonResult RemoveDrugSymptoms(int id)
        {
            var DrugSymptoms = _repo.Find<DrugSymptoms_LookUp>(id);
            _repo.Delete<DrugSymptoms_LookUp>(DrugSymptoms);

            // _user.SaveData();
            return Json(new JsonResponse() { Status = 1, Message = "Drug Symptoms remove successfully" });
        }

        [HttpPost, Route("ActiveDeActiveDrugSymptoms/{flag}/{id}")]
        public JsonResult ActiveDeActiveDrugSymptoms(bool flag, int id)
        {
            var DrugSymptoms = _repo.Find<DrugSymptoms_LookUp>(id);
            DrugSymptoms.IsActive = !flag;
            _repo.Update<DrugSymptoms_LookUp>(DrugSymptoms, true);
            // _user.SaveData();
            return Json(new JsonResponse() { Status = 1, Message = $@"The Drug Symptoms has been {(flag ? "deactivated" : "reactivated")} successfully" });
        }
        #endregion

        #region DrugStrength
        [Route("DrugStrength")]
        public ActionResult DrugStrength()
        {
            return View();
        }

        [Route("GetDrugStrengthList/{flag}")]
        public ActionResult GetDrugStrengthList(bool flag, JQueryDataTableParamModel param)
        {
            var allDrugStrength = _repo.All<DrugStrength_LookUp>().Where(x => !x.IsDeleted).Select(x => new DrugStrengthViewModel()
            {
                Id = x.DrugStrength_LookUpID,
                Name = x.Name,
                Description = x.Description != null ? x.Description : "",
                IsActive = x.IsActive
            }).ToList();

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                allDrugStrength = allDrugStrength.Where(x => Convert.ToString(x.Name).ToLower().Contains(param.sSearch.ToLower())
                                                || Convert.ToString(x.Description).ToLower().Contains(param.sSearch.ToLower())
                                               ).ToList();
            }

            var sortColumnIndex = Convert.ToInt32(HttpContext.Request.QueryString["iSortCol_0"]);
            var sortDirection = HttpContext.Request.QueryString["sSortDir_0"]; // asc or desc

            Func<DrugStrengthViewModel, string> orderingFunction =
                c => sortColumnIndex == 1 ? c.Name
                    : sortColumnIndex == 2 ? c.Description
                    : sortColumnIndex == 4 ? c.CreatedDate
                        : sortColumnIndex == 5 ? c.UpdatedDate
                         : c.Name;
            allDrugStrength = sortDirection == "asc" ? allDrugStrength.OrderBy(orderingFunction).ToList() : allDrugStrength.OrderByDescending(orderingFunction).ToList();

            var display = allDrugStrength.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var total = allDrugStrength.Count();

            return Json(new
            {
                param.sEcho,
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                aaData = display
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet, Route("_DrugStrength/{id?}")]
        public PartialViewResult _DrugStrength(int id)
        {

            if (id == 0) return PartialView(@"Partial/_DrugStrength", new DrugStrengthViewModel());
            var DrugStrength = _repo.Find<DrugStrength_LookUp>(id);
            var result = new DrugStrengthViewModel()
            {
                IsActive = DrugStrength.IsActive,
                Id = DrugStrength.DrugStrength_LookUpID,
                Name = DrugStrength.Name,
                Description = DrugStrength.Description
            };
            if (result == null) return PartialView(@"Partial/_DrugStrength", new DrugStrengthViewModel());

            return PartialView(@"Partial/_DrugStrength", result);
        }
        [HttpGet, Route("_ViewDrugStrength/{id?}")]
        public PartialViewResult _ViewDrugStrength(int id)
        {

            if (id == 0) return PartialView(@"Partial/_DrugStrength", new DrugStrengthViewModel());
            var DrugStrength = _repo.Find<DrugStrength_LookUp>(id);
            var result = new DrugStrengthViewModel()
            {
                IsActive = DrugStrength.IsActive,
                Id = DrugStrength.DrugStrength_LookUpID,
                Name = DrugStrength.Name,
                Description = DrugStrength.Description,
                IsViewMode = true
            };
            if (result == null) return PartialView(@"Partial/_DrugStrength", new DrugStrengthViewModel());

            return PartialView(@"Partial/_DrugStrength", result);
        }

        [HttpGet]
        public JsonResult GetDrugStrengthList(string query = "")
        {
            var DrugStrengthList = _repo.All<DrugStrength_LookUp>().Where(x => x.IsActive && !x.IsDeleted)
                .Select(x => new KeyValuePairModel()
                {
                    Id = x.DrugStrength_LookUpID.ToString(),
                    Name = x.Name
                }).ToList();
            DrugStrengthList = DrugStrengthList.Where(x => x.Name.ToLower().Equals(query.ToLower())).ToList();
            return Json(new JsonResponse() { Status = 1, Data = DrugStrengthList });
        }

        [Route("DrugStrengthDetail/{npi?}")]
        public ActionResult DrugStrengthDetail(string npi)
        {
            if (string.IsNullOrWhiteSpace(npi))
            {
                return RedirectToAction("DrugStrength", "User");
            }
            var DrugStrength = _repo.Find<DrugStrength_LookUp>(x => x.DrugStrength_LookUpID.ToString() == npi);
            return View(DrugStrength);
        }

        [HttpPost, Route("AddEditDrugStrength"), ValidateAntiForgeryToken]
        public JsonResult AddEditDrugStrength(DrugStrengthViewModel model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                string filePath = string.Empty;
                try
                {
                    if (ModelState.IsValid)
                    {
                        if (model.Id == 0)
                        {
                            var bResult = _repo.Find<DrugStrength_LookUp>(x => x.Name.ToLower().Equals(model.Name.ToLower()) && x.IsActive && !x.IsDeleted);
                            if (bResult != null)
                            {
                                txscope.Dispose();
                                return Json(new JsonResponse() { Status = 0, Message = "Drug Strength  already exists. Could not add the data!" });
                            }

                            var DrugStrength = new DrugStrength_LookUp()
                            {
                                Name = model.Name,
                                Description = model.Description,
                                DrugStrength_LookUpID = model.Id,
                                CreatedDate = DateTime.Now,
                                IsActive = true,
                                IsDeleted = false,
                            };
                            _repo.Insert<DrugStrength_LookUp>(DrugStrength, true);
                            // _repo.sa();
                            txscope.Complete();
                            return Json(new JsonResponse() { Status = 1, Message = "Drug Strength save successfully" });
                        }
                        else
                        {

                            var bResult = _repo.Find<DrugStrength_LookUp>(x => x.DrugStrength_LookUpID != model.Id && x.Name == model.Name && x.IsActive && !x.IsDeleted);
                            if (bResult != null)
                            {
                                txscope.Dispose();
                                return Json(new JsonResponse { Status = 0, Message = "Drug Strength already exists. Could not add the data!" });
                            }

                            var oHour = _repo.Find<DrugStrength_LookUp>(model.Id);
                            oHour.Name = model.Name;
                            oHour.Description = model.Description;
                            _repo.Update<DrugStrength_LookUp>(oHour, true);
                            //_user.SaveData();
                            txscope.Complete();
                            return Json(new JsonResponse { Status = 1, Message = "Drug Strength updated successfully" });
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
                    Common.LogError(ex, "AddEditDrugStrength-Post");
                    return Json(new JsonResponse { Status = 0, Message = "Something is wrong." });
                }
            }
        }

        [HttpPost, Route("RemoveDrugStrength/{id?}")]
        public JsonResult RemoveDrugStrength(int id)
        {
            var DrugStrength = _repo.Find<DrugStrength_LookUp>(id);
            _repo.Delete<DrugStrength_LookUp>(DrugStrength);

            // _user.SaveData();
            return Json(new JsonResponse() { Status = 1, Message = "Drug Strength remove successfully" });
        }

        [HttpPost, Route("ActiveDeActiveDrugStrength/{flag}/{id}")]
        public JsonResult ActiveDeActiveDrugStrength(bool flag, int id)
        {
            var DrugStrength = _repo.Find<DrugStrength_LookUp>(id);
            DrugStrength.IsActive = !flag;
            _repo.Update<DrugStrength_LookUp>(DrugStrength, true);
            // _user.SaveData();
            return Json(new JsonResponse() { Status = 1, Message = $@"The Drug Strength has been {(flag ? "deactivated" : "reactivated")} successfully" });
        }
        #endregion

        #region AdvertisementLocation
        [Route("AdvertisementLocation")]
        public ActionResult AdvertisementLocation()
        {
            return View();
        }

        [Route("GetAdvertisementLocationList/{flag}")]
        public ActionResult GetAdvertisementLocationList(bool flag, JQueryDataTableParamModel param)
        {
            var allAdvertisementLocation = _repo.All<AdvertisementLocation>().Where(x => !x.IsDeleted).Select(x => new AdvertisementLocationViewModel()
            {
                Id = x.AdvertisementLocationId,
                Name = x.Name,
                Description = x.Description != null ? x.Description : "",
                IsActive = x.IsActive
            }).ToList();

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                allAdvertisementLocation = allAdvertisementLocation.Where(x => Convert.ToString(x.Name).ToLower().Contains(param.sSearch.ToLower())
                                                || Convert.ToString(x.Description).ToLower().Contains(param.sSearch.ToLower())
                                               ).ToList();
            }

            var sortColumnIndex = Convert.ToInt32(HttpContext.Request.QueryString["iSortCol_0"]);
            var sortDirection = HttpContext.Request.QueryString["sSortDir_0"]; // asc or desc

            Func<AdvertisementLocationViewModel, string> orderingFunction =
                c => sortColumnIndex == 1 ? c.Name
                    : sortColumnIndex == 2 ? c.Description
                    : sortColumnIndex == 4 ? c.CreatedDate
                        : sortColumnIndex == 5 ? c.UpdatedDate
                         : c.Name;
            allAdvertisementLocation = sortDirection == "asc" ? allAdvertisementLocation.OrderBy(orderingFunction).ToList() : allAdvertisementLocation.OrderByDescending(orderingFunction).ToList();

            var display = allAdvertisementLocation.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var total = allAdvertisementLocation.Count();

            return Json(new
            {
                param.sEcho,
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                aaData = display
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet, Route("_AdvertisementLocation/{id?}")]
        public PartialViewResult _AdvertisementLocation(int id)
        {

            if (id == 0) return PartialView(@"Partial/_AdvertisementLocation", new AdvertisementLocationViewModel());
            var AdvertisementLocation = _repo.Find<AdvertisementLocation>(id);
            var result = new AdvertisementLocationViewModel()
            {
                IsActive = AdvertisementLocation.IsActive,
                Id = AdvertisementLocation.AdvertisementLocationId,
                Name = AdvertisementLocation.Name,
                Description = AdvertisementLocation.Description
            };
            if (result == null) return PartialView(@"Partial/_AdvertisementLocation", new AdvertisementLocationViewModel());

            return PartialView(@"Partial/_AdvertisementLocation", result);
        }
        [HttpGet, Route("_ViewAdvertisementLocation/{id?}")]
        public PartialViewResult _ViewAdvertisementLocation(int id)
        {

            if (id == 0) return PartialView(@"Partial/_AdvertisementLocation", new AdvertisementLocationViewModel());
            var AdvertisementLocation = _repo.Find<AdvertisementLocation>(id);
            var result = new AdvertisementLocationViewModel()
            {
                IsActive = AdvertisementLocation.IsActive,
                Id = AdvertisementLocation.AdvertisementLocationId,
                Name = AdvertisementLocation.Name,
                Description = AdvertisementLocation.Description,
                IsViewMode = true
            };
            if (result == null) return PartialView(@"Partial/_AdvertisementLocation", new AdvertisementLocationViewModel());

            return PartialView(@"Partial/_AdvertisementLocation", result);
        }

        [HttpGet]
        public JsonResult GetAdvertisementLocationList(string query = "")
        {
            var AdvertisementLocationList = _repo.All<AdvertisementLocation>().Where(x => x.IsActive && !x.IsDeleted)
                .Select(x => new KeyValuePairModel()
                {
                    Id = x.AdvertisementLocationId.ToString(),
                    Name = x.Name
                }).ToList();
            AdvertisementLocationList = AdvertisementLocationList.Where(x => x.Name.ToLower().Equals(query.ToLower())).ToList();
            return Json(new JsonResponse() { Status = 1, Data = AdvertisementLocationList });
        }

        [Route("AdvertisementLocationDetail/{npi?}")]
        public ActionResult AdvertisementLocationDetail(string npi)
        {
            if (string.IsNullOrWhiteSpace(npi))
            {
                return RedirectToAction("AdvertisementLocation", "User");
            }
            var AdvertisementLocation = _repo.Find<AdvertisementLocation>(x => x.AdvertisementLocationId.ToString() == npi);
            return View(AdvertisementLocation);
        }

        [HttpPost, Route("AddEditAdvertisementLocation"), ValidateAntiForgeryToken]
        public JsonResult AddEditAdvertisementLocation(AdvertisementLocationViewModel model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                string filePath = string.Empty;
                try
                {
                    if (ModelState.IsValid)
                    {
                        if (model.Id == 0)
                        {
                            var bResult = _repo.Find<AdvertisementLocation>(x => x.Name.ToLower().Equals(model.Name.ToLower()) && x.IsActive && !x.IsDeleted);
                            if (bResult != null)
                            {
                                txscope.Dispose();
                                return Json(new JsonResponse() { Status = 0, Message = " Advertisement Location Type already exists. Could not add the data!" });
                            }

                            var AdvertisementLocation = new AdvertisementLocation()
                            {
                                Name = model.Name,
                                Description = model.Description,
                                AdvertisementLocationId = model.Id,
                                CreatedDate = DateTime.Now,
                                IsActive = true,
                                IsDeleted = false,
                            };
                            _repo.Insert<AdvertisementLocation>(AdvertisementLocation, true);
                            // _repo.sa();
                            txscope.Complete();
                            return Json(new JsonResponse() { Status = 1, Message = " Advertisement Location save successfully" });
                        }
                        else
                        {

                            var bResult = _repo.Find<AdvertisementLocation>(x => x.AdvertisementLocationId != model.Id && x.Name == model.Name && x.IsActive && !x.IsDeleted);
                            if (bResult != null)
                            {
                                txscope.Dispose();
                                return Json(new JsonResponse { Status = 0, Message = " Advertisement Location already exists. Could not add the data!" });
                            }

                            var oHour = _repo.Find<AdvertisementLocation>(model.Id);
                            oHour.Name = model.Name;
                            oHour.Description = model.Description;
                            _repo.Update<AdvertisementLocation>(oHour, true);
                            //_user.SaveData();
                            txscope.Complete();
                            return Json(new JsonResponse { Status = 1, Message = " Advertisement Location updated successfully" });
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
                    Common.LogError(ex, "AddEditAdvertisementLocation-Post");
                    return Json(new JsonResponse { Status = 0, Message = "Something is wrong." });
                }
            }
        }

        [HttpPost, Route("RemoveAdvertisementLocation/{id?}")]
        public JsonResult RemoveAdvertisementLocation(int id)
        {
            var AdvertisementLocation = _repo.Find<AdvertisementLocation>(id);
            _repo.Delete<AdvertisementLocation>(AdvertisementLocation);

            // _user.SaveData();
            return Json(new JsonResponse() { Status = 1, Message = " Advertisement Location remove successfully" });
        }

        [HttpPost, Route("ActiveDeActiveAdvertisementLocation/{flag}/{id}")]
        public JsonResult ActiveDeActiveAdvertisementLocation(bool flag, int id)
        {
            var AdvertisementLocation = _repo.Find<AdvertisementLocation>(id);
            AdvertisementLocation.IsActive = !flag;
            _repo.Update<AdvertisementLocation>(AdvertisementLocation, true);
            // _user.SaveData();
            return Json(new JsonResponse() { Status = 1, Message = $@"The  Advertisement Location has been {(flag ? "deactivated" : "reactivated")} successfully" });
        }
        #endregion

        //#region DrugManufacturer
        //[Route("DrugManufacturer")]
        //public ActionResult DrugManufacturer()
        //{
        //    return View();
        //}

        //[Route("GetDrugManufacturerList/{flag}")]
        //public ActionResult GetDrugManufacturerList(bool flag, JQueryDataTableParamModel param)
        //{
        //    var allDrugManufacturer = _repo.All<DrugManufacturer_LookUp>().Where(x => !x.IsDeleted).Select(x => new DrugManufacturerViewModel()
        //    {
        //        Id = x.DrugManufacturer_LookUpID,
        //        Name = x.Name,
        //        Description = x.Description,
        //        IsActive = x.IsActive
        //    }).ToList();

        //    if (!string.IsNullOrEmpty(param.sSearch))
        //    {
        //        allDrugManufacturer = allDrugManufacturer.Where(x => x.Name.ToString().ToLower().Contains(param.sSearch.ToLower())
        //                                        || x.Description.ToString().ToLower().Contains(param.sSearch.ToLower())
        //                                        || x.CreatedDate.ToString().ToLower().Contains(param.sSearch.ToLower())
        //                                        || x.UpdatedDate.ToString().ToLower().Contains(param.sSearch.ToLower())).ToList();
        //    }

        //    var sortColumnIndex = Convert.ToInt32(HttpContext.Request.QueryString["iSortCol_0"]);
        //    var sortDirection = HttpContext.Request.QueryString["sSortDir_0"]; // asc or desc

        //    Func<DrugManufacturerViewModel, string> orderingFunction =
        //        c => sortColumnIndex == 1 ? c.Name
        //            : sortColumnIndex == 2 ? c.Description
        //            : sortColumnIndex == 4 ? c.CreatedDate
        //                : sortColumnIndex == 5 ? c.UpdatedDate
        //                 : c.Name;
        //    allDrugManufacturer = sortDirection == "asc" ? allDrugManufacturer.OrderBy(orderingFunction).ToList() : allDrugManufacturer.OrderByDescending(orderingFunction).ToList();

        //    var display = allDrugManufacturer.Skip(param.iDisplayStart).Take(param.iDisplayLength);
        //    var total = allDrugManufacturer.Count();

        //    return Json(new
        //    {
        //        param.sEcho,
        //        iTotalRecords = total,
        //        iTotalDisplayRecords = total,
        //        aaData = display
        //    }, JsonRequestBehavior.AllowGet);
        //}

        //[HttpGet, Route("_DrugManufacturer/{id?}")]
        //public PartialViewResult _DrugManufacturer(int id)
        //{

        //    if (id == 0) return PartialView(@"Partial/_DrugManufacturer", new DrugManufacturerViewModel());
        //    var DrugManufacturer = _repo.Find<DrugManufacturer_LookUp>(id);
        //    var result = new DrugManufacturerViewModel()
        //    {
        //        IsActive = DrugManufacturer.IsActive,
        //        Id = DrugManufacturer.DrugManufacturer_LookUpID,
        //        Name = DrugManufacturer.Name,
        //        Description = DrugManufacturer.Description
        //    };
        //    if (result == null) return PartialView(@"Partial/_DrugManufacturer", new DrugManufacturerViewModel());

        //    return PartialView(@"Partial/_DrugManufacturer", result);
        //}
        //[HttpGet, Route("_ViewDrugManufacturer/{id?}")]
        //public PartialViewResult _ViewDrugManufacturer(int id)
        //{

        //    if (id == 0) return PartialView(@"Partial/_DrugManufacturer", new DrugManufacturerViewModel());
        //    var DrugManufacturer = _repo.Find<DrugManufacturer_LookUp>(id);
        //    var result = new DrugManufacturerViewModel()
        //    {
        //        IsActive = DrugManufacturer.IsActive,
        //        Id = DrugManufacturer.DrugManufacturer_LookUpID,
        //        Name = DrugManufacturer.Name,
        //        Description = DrugManufacturer.Description,
        //        IsViewMode = true
        //    };
        //    if (result == null) return PartialView(@"Partial/_DrugManufacturer", new DrugManufacturerViewModel());

        //    return PartialView(@"Partial/_DrugManufacturer", result);
        //}

        //[HttpGet]
        //public JsonResult GetDrugManufacturerList(string query = "")
        //{
        //    var DrugManufacturerList = _repo.All<DrugManufacturer_LookUp>().Where(x => x.IsActive && !x.IsDeleted)
        //        .Select(x => new KeyValuePairModel()
        //        {
        //            Id = x.DrugManufacturer_LookUpID.ToString(),
        //            Name = x.Name
        //        }).ToList();
        //    DrugManufacturerList = DrugManufacturerList.Where(x => x.Name.ToLower().Equals(query.ToLower())).ToList();
        //    return Json(new JsonResponse() { Status = 1, Data = DrugManufacturerList });
        //}

        //[Route("DrugManufacturerDetail/{npi?}")]
        //public ActionResult DrugManufacturerDetail(string npi)
        //{
        //    if (string.IsNullOrWhiteSpace(npi))
        //    {
        //        return RedirectToAction("DrugManufacturer", "User");
        //    }
        //    var DrugManufacturer = _repo.Find<DrugManufacturer_LookUp>(x => x.DrugManufacturer_LookUpId.ToString() == npi);
        //    return View(DrugManufacturer);
        //}

        //[HttpPost, Route("AddEditDrugManufacturer"), ValidateAntiForgeryToken]
        //public JsonResult AddEditDrugManufacturer(DrugManufacturerViewModel model)
        //{
        //    using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        //    {
        //        string filePath = string.Empty;
        //        try
        //        {
        //            if (ModelState.IsValid)
        //            {
        //                if (model.Id == 0)
        //                {
        //                    var bResult = _repo.Find<DrugManufacturer_LookUp>(x => x.CompanyName.ToLower().Equals(model.Name.ToLower()) && x.IsActive && !x.IsDeleted);
        //                    if (bResult != null)
        //                    {
        //                        txscope.Dispose();
        //                        return Json(new JsonResponse() { Status = 0, Message = "Drug Manufacturer Type already exists. Could not add the data!" });
        //                    }

        //                    var DrugManufacturer = new DrugManufacturer_LookUp()
        //                    {
        //                        Name = model.Name,
        //                        Description = model.Description,
        //                        DrugManufacturer_LookUpID = model.Id,
        //                        CreatedDate = DateTime.Now,
        //                        IsActive = true,
        //                        IsDeleted = false,
        //                    };
        //                    _repo.Insert<DrugManufacturer_LookUp>(DrugManufacturer, true);
        //                    // _repo.sa();
        //                    txscope.Complete();
        //                    return Json(new JsonResponse() { Status = 1, Message = "Drug Manufacturer save successfully" });
        //                }
        //                else
        //                {

        //                    var bResult = _repo.Find<DrugManufacturer_LookUp>(x => x.DrugManufacturer_LookUpID != model.Id && x.Name == model.Name && x.IsActive && !x.IsDeleted);
        //                    if (bResult != null)
        //                    {
        //                        txscope.Dispose();
        //                        return Json(new JsonResponse { Status = 0, Message = "Drug Manufacturer already exists. Could not add the data!" });
        //                    }

        //                    var oHour = _repo.Find<DrugManufacturer_LookUp>(model.Id);
        //                    oHour.Name = model.Name;
        //                    oHour.Description = model.Description;
        //                    _repo.Update<DrugManufacturer_LookUp>(oHour, true);
        //                    //_user.SaveData();
        //                    txscope.Complete();
        //                    return Json(new JsonResponse { Status = 1, Message = "Drug Manufacturer updated successfully" });
        //                }
        //            }
        //            else
        //            {
        //                txscope.Dispose();
        //                return Json(new JsonResponse { Status = 4, Data = ModelState.Errors() });
        //            }
        //        }
        //        catch (Exception ex)
        //        {

        //            txscope.Dispose();
        //            Common.LogError(ex, "AddEditDrugManufacturer-Post");
        //            return Json(new JsonResponse { Status = 0, Message = "Something is wrong." });
        //        }
        //    }
        //}

        //[HttpPost, Route("RemoveDrugManufacturer/{id?}")]
        //public JsonResult RemoveDrugManufacturer(int id)
        //{
        //    var DrugManufacturer = _repo.Find<DrugManufacturer_LookUp>(id);
        //    _repo.Delete<DrugManufacturer_LookUp>(DrugManufacturer);

        //    // _user.SaveData();
        //    return Json(new JsonResponse() { Status = 1, Message = "Drug Manufacturer remove successfully" });
        //}

        //[HttpPost, Route("ActiveDeActiveDrugManufacturer/{flag}/{id}")]
        //public JsonResult ActiveDeActiveDrugManufacturer(bool flag, int id)
        //{
        //    var DrugManufacturer = _repo.Find<DrugManufacturer_LookUp>(id);
        //    DrugManufacturer.IsActive = !flag;
        //    _repo.Update<DrugManufacturer_LookUp>(DrugManufacturer, true);
        //    // _user.SaveData();
        //    return Json(new JsonResponse() { Status = 1, Message = $@"The Drug Manufacturer has been {(flag ? "deactivated" : "reactivated")} successfully" });
        //}
        //#endregion

        #region DrugStatus
        [Route("DrugStatus")]
        public ActionResult DrugStatus()
        {
            return View();
        }

        [Route("GetDrugStatusList/{flag}")]
        public ActionResult GetDrugStatusList(bool flag, JQueryDataTableParamModel param)
        {
            var allDrugStatus = _repo.All<DrugStatus_LookUp>().Where(x => !x.IsDeleted).Select(x => new DrugStatusViewModel()
            {
                Id = x.DrugStatus_LookUpID,
                Name = x.Name,
                Description = x.Description != null ? x.Description : "",
                IsActive = x.IsActive
            }).ToList();

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                allDrugStatus = allDrugStatus.Where(x => Convert.ToString(x.Name).ToLower().Contains(param.sSearch.ToLower())
                                                || Convert.ToString(x.Description).ToLower().Contains(param.sSearch.ToLower())
                                               ).ToList();
            }

            var sortColumnIndex = Convert.ToInt32(HttpContext.Request.QueryString["iSortCol_0"]);
            var sortDirection = HttpContext.Request.QueryString["sSortDir_0"]; // asc or desc

            Func<DrugStatusViewModel, string> orderingFunction =
                c => sortColumnIndex == 1 ? c.Name
                    : sortColumnIndex == 2 ? c.Description
                    : sortColumnIndex == 4 ? c.CreatedDate
                        : sortColumnIndex == 5 ? c.UpdatedDate
                         : c.Name;
            allDrugStatus = sortDirection == "asc" ? allDrugStatus.OrderBy(orderingFunction).ToList() : allDrugStatus.OrderByDescending(orderingFunction).ToList();

            var display = allDrugStatus.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var total = allDrugStatus.Count();

            return Json(new
            {
                param.sEcho,
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                aaData = display
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet, Route("_DrugStatus/{id?}")]
        public PartialViewResult _DrugStatus(int id)
        {

            if (id == 0) return PartialView(@"Partial/_DrugStatus", new DrugStatusViewModel());
            var DrugStatus = _repo.Find<DrugStatus_LookUp>(id);
            var result = new DrugStatusViewModel()
            {
                IsActive = DrugStatus.IsActive,
                Id = DrugStatus.DrugStatus_LookUpID,
                Name = DrugStatus.Name,
                Description = DrugStatus.Description
            };
            if (result == null) return PartialView(@"Partial/_DrugStatus", new DrugStatusViewModel());

            return PartialView(@"Partial/_DrugStatus", result);
        }
        [HttpGet, Route("_ViewDrugStatus/{id?}")]
        public PartialViewResult _ViewDrugStatus(int id)
        {

            if (id == 0) return PartialView(@"Partial/_DrugStatus", new DrugStatusViewModel());
            var DrugStatus = _repo.Find<DrugStatus_LookUp>(id);
            var result = new DrugStatusViewModel()
            {
                IsActive = DrugStatus.IsActive,
                Id = DrugStatus.DrugStatus_LookUpID,
                Name = DrugStatus.Name,
                Description = DrugStatus.Description,
                IsViewMode = true
            };
            if (result == null) return PartialView(@"Partial/_DrugStatus", new DrugStatusViewModel());

            return PartialView(@"Partial/_DrugStatus", result);
        }

        [HttpGet]
        public JsonResult GetDrugStatusList(string query = "")
        {
            var DrugStatusList = _repo.All<DrugStatus_LookUp>().Where(x => x.IsActive && !x.IsDeleted)
                .Select(x => new KeyValuePairModel()
                {
                    Id = x.DrugStatus_LookUpID.ToString(),
                    Name = x.Name
                }).ToList();
            DrugStatusList = DrugStatusList.Where(x => x.Name.ToLower().Equals(query.ToLower())).ToList();
            return Json(new JsonResponse() { Status = 1, Data = DrugStatusList });
        }

        [Route("DrugStatusDetail/{npi?}")]
        public ActionResult DrugStatusDetail(string npi)
        {
            if (string.IsNullOrWhiteSpace(npi))
            {
                return RedirectToAction("DrugStatus", "User");
            }
            var DrugStatus = _repo.Find<DrugStatus_LookUp>(x => x.DrugStatus_LookUpID.ToString() == npi);
            return View(DrugStatus);
        }

        [HttpPost, Route("AddEditDrugStatus"), ValidateAntiForgeryToken]
        public JsonResult AddEditDrugStatus(DrugStatusViewModel model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                string filePath = string.Empty;
                try
                {
                    if (ModelState.IsValid)
                    {
                        if (model.Id == 0)
                        {
                            var bResult = _repo.Find<DrugStatus_LookUp>(x => x.Name.ToLower().Equals(model.Name.ToLower()) && x.IsActive && !x.IsDeleted);
                            if (bResult != null)
                            {
                                txscope.Dispose();
                                return Json(new JsonResponse() { Status = 0, Message = "Drug Status Type already exists. Could not add the data!" });
                            }

                            var DrugStatus = new DrugStatus_LookUp()
                            {
                                Name = model.Name,
                                Description = model.Description,
                                DrugStatus_LookUpID = model.Id,
                                CreatedDate = DateTime.Now,
                                IsActive = true,
                                IsDeleted = false,
                            };
                            _repo.Insert<DrugStatus_LookUp>(DrugStatus, true);
                            // _repo.sa();
                            txscope.Complete();
                            return Json(new JsonResponse() { Status = 1, Message = "Drug Status save successfully" });
                        }
                        else
                        {

                            var bResult = _repo.Find<DrugStatus_LookUp>(x => x.DrugStatus_LookUpID != model.Id && x.Name == model.Name && x.IsActive && !x.IsDeleted);
                            if (bResult != null)
                            {
                                txscope.Dispose();
                                return Json(new JsonResponse { Status = 0, Message = "Drug Status already exists. Could not add the data!" });
                            }

                            var oHour = _repo.Find<DrugStatus_LookUp>(model.Id);
                            oHour.Name = model.Name;
                            oHour.Description = model.Description;
                            _repo.Update<DrugStatus_LookUp>(oHour, true);
                            //_user.SaveData();
                            txscope.Complete();
                            return Json(new JsonResponse { Status = 1, Message = "Drug Status updated successfully" });
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
                    Common.LogError(ex, "AddEditDrugStatus-Post");
                    return Json(new JsonResponse { Status = 0, Message = "Something is wrong." });
                }
            }
        }

        [HttpPost, Route("RemoveDrugStatus/{id?}")]
        public JsonResult RemoveDrugStatus(int id)
        {
            var DrugStatus = _repo.Find<DrugStatus_LookUp>(id);
            _repo.Delete<DrugStatus_LookUp>(DrugStatus);

            // _user.SaveData();
            return Json(new JsonResponse() { Status = 1, Message = "Drug Status remove successfully" });
        }

        [HttpPost, Route("ActiveDeActiveDrugStatus/{flag}/{id}")]
        public JsonResult ActiveDeActiveDrugStatus(bool flag, int id)
        {
            var DrugStatus = _repo.Find<DrugStatus_LookUp>(id);
            DrugStatus.IsActive = !flag;
            _repo.Update<DrugStatus_LookUp>(DrugStatus, true);
            // _user.SaveData();
            return Json(new JsonResponse() { Status = 1, Message = $@"The Drug Status has been {(flag ? "deactivated" : "reactivated")} successfully" });
        }
        #endregion

        #region DrugTabs
        [Route("DrugTabs")]
        public ActionResult DrugTabs()
        {
            return View();
        }

        [Route("GetDrugTabsList/{flag}")]
        public ActionResult GetDrugTabsList(bool flag, JQueryDataTableParamModel param)
        {
            var allDrugTabs = _repo.All<DrugTabs_LookUp>().Where(x => !x.IsDeleted).Select(x => new DrugTabsViewModel()
            {
                Id = x.DrugTabs_LookUpID,
                Name = x.Name,
                Description = x.Description != null ? x.Description : "",
                IsActive = x.IsActive
            }).ToList();

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                allDrugTabs = allDrugTabs.Where(x => Convert.ToString(x.Name).ToLower().Contains(param.sSearch.ToLower())
                                                || Convert.ToString(x.Description).ToLower().Contains(param.sSearch.ToLower())
                                               ).ToList();
            }

            var sortColumnIndex = Convert.ToInt32(HttpContext.Request.QueryString["iSortCol_0"]);
            var sortDirection = HttpContext.Request.QueryString["sSortDir_0"]; // asc or desc

            Func<DrugTabsViewModel, string> orderingFunction =
                c => sortColumnIndex == 1 ? c.Name
                    : sortColumnIndex == 2 ? c.Description
                    : sortColumnIndex == 4 ? c.CreatedDate
                        : sortColumnIndex == 5 ? c.UpdatedDate
                         : c.Name;
            allDrugTabs = sortDirection == "asc" ? allDrugTabs.OrderBy(orderingFunction).ToList() : allDrugTabs.OrderByDescending(orderingFunction).ToList();

            var display = allDrugTabs.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var total = allDrugTabs.Count();

            return Json(new
            {
                param.sEcho,
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                aaData = display
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet, Route("_DrugTabs/{id?}")]
        public PartialViewResult _DrugTabs(int id)
        {

            if (id == 0) return PartialView(@"Partial/_DrugTabs", new DrugTabsViewModel());
            var DrugTabs = _repo.Find<DrugTabs_LookUp>(id);
            var result = new DrugTabsViewModel()
            {
                IsActive = DrugTabs.IsActive,
                Id = DrugTabs.DrugTabs_LookUpID,
                Name = DrugTabs.Name,
                Description = DrugTabs.Description
            };
            if (result == null) return PartialView(@"Partial/_DrugTabs", new DrugTabsViewModel());

            return PartialView(@"Partial/_DrugTabs", result);
        }
        [HttpGet, Route("_ViewDrugTabs/{id?}")]
        public PartialViewResult _ViewDrugTabs(int id)
        {

            if (id == 0) return PartialView(@"Partial/_DrugTabs", new DrugTabsViewModel());
            var DrugTabs = _repo.Find<DrugTabs_LookUp>(id);
            var result = new DrugTabsViewModel()
            {
                IsActive = DrugTabs.IsActive,
                Id = DrugTabs.DrugTabs_LookUpID,
                Name = DrugTabs.Name,
                Description = DrugTabs.Description,
                IsViewMode = true
            };
            if (result == null) return PartialView(@"Partial/_DrugTabs", new DrugTabsViewModel());

            return PartialView(@"Partial/_DrugTabs", result);
        }

        [HttpGet]
        public JsonResult GetDrugTabsList(string query = "")
        {
            var DrugTabsList = _repo.All<DrugTabs_LookUp>().Where(x => x.IsActive && !x.IsDeleted)
                .Select(x => new KeyValuePairModel()
                {
                    Id = x.DrugTabs_LookUpID.ToString(),
                    Name = x.Name
                }).ToList();
            DrugTabsList = DrugTabsList.Where(x => x.Name.ToLower().Equals(query.ToLower())).ToList();
            return Json(new JsonResponse() { Status = 1, Data = DrugTabsList });
        }

        [Route("DrugTabsDetail/{npi?}")]
        public ActionResult DrugTabsDetail(string npi)
        {
            if (string.IsNullOrWhiteSpace(npi))
            {
                return RedirectToAction("DrugTabs", "User");
            }
            var DrugTabs = _repo.Find<DrugTabs_LookUp>(x => x.DrugTabs_LookUpID.ToString() == npi);
            return View(DrugTabs);
        }

        [HttpPost, Route("AddEditDrugTabs"), ValidateAntiForgeryToken]
        public JsonResult AddEditDrugTabs(DrugTabsViewModel model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                string filePath = string.Empty;
                try
                {
                    if (ModelState.IsValid)
                    {
                        if (model.Id == 0)
                        {
                            var bResult = _repo.Find<DrugTabs_LookUp>(x => x.Name.ToLower().Equals(model.Name.ToLower()) && x.IsActive && !x.IsDeleted);
                            if (bResult != null)
                            {
                                txscope.Dispose();
                                return Json(new JsonResponse() { Status = 0, Message = "Drug Tabs Type already exists. Could not add the data!" });
                            }

                            var DrugTabs = new DrugTabs_LookUp()
                            {
                                Name = model.Name,
                                Description = model.Description,
                                DrugTabs_LookUpID = model.Id,
                                CreatedDate = DateTime.Now,
                                IsActive = true,
                                IsDeleted = false,
                            };
                            _repo.Insert<DrugTabs_LookUp>(DrugTabs, true);
                            // _repo.sa();
                            txscope.Complete();
                            return Json(new JsonResponse() { Status = 1, Message = "Drug Tabs save successfully" });
                        }
                        else
                        {

                            var bResult = _repo.Find<DrugTabs_LookUp>(x => x.DrugTabs_LookUpID != model.Id && x.Name == model.Name && x.IsActive && !x.IsDeleted);
                            if (bResult != null)
                            {
                                txscope.Dispose();
                                return Json(new JsonResponse { Status = 0, Message = "Drug Tabs already exists. Could not add the data!" });
                            }

                            var oHour = _repo.Find<DrugTabs_LookUp>(model.Id);
                            oHour.Name = model.Name;
                            oHour.Description = model.Description;
                            _repo.Update<DrugTabs_LookUp>(oHour, true);
                            //_user.SaveData();
                            txscope.Complete();
                            return Json(new JsonResponse { Status = 1, Message = "Drug Tabs updated successfully" });
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
                    Common.LogError(ex, "AddEditDrugTabs-Post");
                    return Json(new JsonResponse { Status = 0, Message = "Something is wrong." });
                }
            }
        }

        [HttpPost, Route("RemoveDrugTabs/{id?}")]
        public JsonResult RemoveDrugTabs(int id)
        {
            var DrugTabs = _repo.Find<DrugTabs_LookUp>(id);
            _repo.Delete<DrugTabs_LookUp>(DrugTabs);

            // _user.SaveData();
            return Json(new JsonResponse() { Status = 1, Message = "Drug Tabs remove successfully" });
        }

        [HttpPost, Route("ActiveDeActiveDrugTabs/{flag}/{id}")]
        public JsonResult ActiveDeActiveDrugTabs(bool flag, int id)
        {
            var DrugTabs = _repo.Find<DrugTabs_LookUp>(id);
            DrugTabs.IsActive = !flag;
            _repo.Update<DrugTabs_LookUp>(DrugTabs, true);
            // _user.SaveData();
            return Json(new JsonResponse() { Status = 1, Message = $@"The Drug Tabs has been {(flag ? "deactivated" : "reactivated")} successfully" });
        }
        #endregion

        #region OrganisationType
        [Route("OrganisationType")]
        public ActionResult OrganisationType()
        {
            return View();
        }

        [Route("GetOrganisationTypesList/{flag}")]
        public ActionResult GetOrganisationTypesList(bool flag, JQueryDataTableParamModel param)
        {
            var allOrganisationTypes = _repo.All<OrganisationType>().Where(x => !x.IsDeleted).Select(x => new OrganisationTypeViewModel()
            {
                Id = x.OrganizationTypeID,
                Name = x.Org_Type_Name,
                Description = x.Org_Type_Description != null ? x.Org_Type_Description : "",
                IsActive = x.IsActive
            }).ToList();

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                allOrganisationTypes = allOrganisationTypes.Where(x => Convert.ToString(x.Name).ToLower().Contains(param.sSearch.ToLower())
                                                || Convert.ToString(x.Description).ToLower().Contains(param.sSearch.ToLower())
                                             ).ToList();
            }

            var sortColumnIndex = Convert.ToInt32(HttpContext.Request.QueryString["iSortCol_0"]);
            var sortDirection = HttpContext.Request.QueryString["sSortDir_0"]; // asc or desc

            Func<OrganisationTypeViewModel, string> orderingFunction =
                c => sortColumnIndex == 1 ? c.Name
                    : sortColumnIndex == 2 ? c.Description
                    : sortColumnIndex == 4 ? c.CreatedDate
                        : sortColumnIndex == 5 ? c.UpdatedDate
                         : c.Name;
            allOrganisationTypes = sortDirection == "asc" ? allOrganisationTypes.OrderBy(orderingFunction).ToList() : allOrganisationTypes.OrderByDescending(orderingFunction).ToList();

            var display = allOrganisationTypes.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var total = allOrganisationTypes.Count();

            return Json(new
            {
                param.sEcho,
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                aaData = display
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet, Route("_OrganisationTypes/{id?}")]
        public PartialViewResult _OrganisationTypes(int id)
        {

            if (id == 0) return PartialView(@"Partial/_OrganisationTypes", new OrganisationTypeViewModel());
            var OrganisationType = _repo.Find<OrganisationType>(id);
            var result = new OrganisationTypeViewModel()
            {
                IsActive = OrganisationType.IsActive,
                Id = OrganisationType.OrganizationTypeID,
                Name = OrganisationType.Org_Type_Name,
                Description = OrganisationType.Org_Type_Description
            };
            if (result == null) return PartialView(@"Partial/_OrganisationTypes", new OrganisationTypeViewModel());

            return PartialView(@"Partial/_OrganisationTypes", result);
        }
        [HttpGet, Route("_ViewOrganisationTypes/{id?}")]
        public PartialViewResult _ViewOrganisationTypes(int id)
        {

            if (id == 0) return PartialView(@"Partial/_OrganisationTypes", new OrganisationTypeViewModel());
            var OrganisationType = _repo.Find<OrganisationType>(id);
            var result = new OrganisationTypeViewModel()
            {
                IsActive = OrganisationType.IsActive,
                Id = OrganisationType.OrganizationTypeID,
                Name = OrganisationType.Org_Type_Name,
                Description = OrganisationType.Org_Type_Description,
                IsViewMode = true
            };
            if (result == null) return PartialView(@"Partial/_OrganisationTypes", new OrganisationTypeViewModel());

            return PartialView(@"Partial/_OrganisationTypes", result);
        }

        [HttpGet]
        public JsonResult GetOrganisationTypesList(string query = "")
        {
            var OrganisationTypesList = _repo.All<OrganisationType>().Where(x => x.IsActive && !x.IsDeleted)
                .Select(x => new KeyValuePairModel()
                {
                    Id = x.OrganizationTypeID.ToString(),
                    Name = x.Org_Type_Name
                }).ToList();
            OrganisationTypesList = OrganisationTypesList.Where(x => x.Name.ToLower().Equals(query.ToLower())).ToList();
            return Json(new JsonResponse() { Status = 1, Data = OrganisationTypesList });
        }

        [Route("OrganisationTypesDetail/{npi?}")]
        public ActionResult OrganisationTypesDetail(string npi)
        {
            if (string.IsNullOrWhiteSpace(npi))
            {
                return RedirectToAction("OrganisationType", "User");
            }
            var OrganisationType = _repo.Find<OrganisationType>(x => x.OrganizationTypeID.ToString() == npi);
            return View(OrganisationType);
        }

        [HttpPost, Route("AddEditOrganisationType"), ValidateAntiForgeryToken]
        public JsonResult AddEditOrganisationType(OrganisationTypeViewModel model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                string filePath = string.Empty;
                try
                {
                    if (ModelState.IsValid)
                    {
                        if (model.Id == 0)
                        {
                            var bResult = _repo.Find<OrganisationType>(x => x.Org_Type_Name.ToLower().Equals(model.Name.ToLower()) && x.IsActive && !x.IsDeleted);
                            if (bResult != null)
                            {
                                txscope.Dispose();
                                return Json(new JsonResponse() { Status = 0, Message = "Organisation Type Type already exists. Could not add the data!" });
                            }

                            var OrganisationType = new OrganisationType()
                            {
                                Org_Type_Name = model.Name,
                                Org_Type_Description = model.Description,
                                OrganizationTypeID = model.Id,
                                CreatedDate = DateTime.Now,
                                IsActive = true,
                                IsDeleted = false,
                            };
                            _repo.Insert<OrganisationType>(OrganisationType, true);
                            // _repo.sa();
                            txscope.Complete();
                            return Json(new JsonResponse() { Status = 1, Message = "Organisation Type save successfully" });
                        }
                        else
                        {

                            var bResult = _repo.Find<OrganisationType>(x => x.OrganizationTypeID != model.Id && x.Org_Type_Name == model.Name && x.IsActive && !x.IsDeleted);
                            if (bResult != null)
                            {
                                txscope.Dispose();
                                return Json(new JsonResponse { Status = 0, Message = "Organisation Type already exists. Could not add the data!" });
                            }

                            var oHour = _repo.Find<OrganisationType>(model.Id);
                            oHour.Org_Type_Name = model.Name;
                            oHour.Org_Type_Description = model.Description;
                            _repo.Update<OrganisationType>(oHour, true);
                            //_user.SaveData();
                            txscope.Complete();
                            return Json(new JsonResponse { Status = 1, Message = "Organisation Type updated successfully" });
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
                    Common.LogError(ex, "AddEditOrganisationTypes-Post");
                    return Json(new JsonResponse { Status = 0, Message = "Something is wrong." });
                }
            }
        }

        [HttpPost, Route("RemoveOrganisationTypes/{id?}")]
        public JsonResult RemoveOrganisationTypes(int id)
        {
            var OrganisationType = _repo.Find<OrganisationType>(id);
            _repo.Delete<OrganisationType>(OrganisationType);

            // _user.SaveData();
            return Json(new JsonResponse() { Status = 1, Message = "Organisation Type remove successfully" });
        }

        [HttpPost, Route("ActiveDeActiveOrganisationTypes/{flag}/{id}")]
        public JsonResult ActiveDeActiveOrganisationTypes(bool flag, int id)
        {
            var OrganisationType = _repo.Find<OrganisationType>(id);
            OrganisationType.IsActive = !flag;
            _repo.Update<OrganisationType>(OrganisationType, true);
            // _user.SaveData();
            return Json(new JsonResponse() { Status = 1, Message = $@"The Organisation Type has been {(flag ? "deactivated" : "reactivated")} successfully" });
        }
        #endregion


        //#region Speciality
        //[Route("Speciality")]
        //public ActionResult Speciality()
        //{
        //    return View();
        //}

        //[Route("GetSpecialitysList/{flag}")]
        //public ActionResult GetSpecialitysList(bool flag, JQueryDataTableParamModel param)
        //{
        //    var allSpecialitys = _repo.All<Speciality>().Where(x => !x.IsDeleted).Select(x => new SpecialitiesViewModel()
        //    {
        //        Id = x.SpecialityId,
        //        Name = x.SpecialityName,
        //        Description = x.Description,
        //        TaxonomyID=x.TaxonomyID,
        //        IsActive = x.IsActive
        //    }).ToList();

        //    if (!string.IsNullOrEmpty(param.sSearch))
        //    {
        //        allSpecialitys = allSpecialitys.Where(x => x.Name.ToString().ToLower().Contains(param.sSearch.ToLower())
        //                                        || x.Description.ToString().ToLower().Contains(param.sSearch.ToLower())
        //                                        || x.CreatedDate.ToString().ToLower().Contains(param.sSearch.ToLower())
        //                                        || x.UpdatedDate.ToString().ToLower().Contains(param.sSearch.ToLower())).ToList();
        //    }

        //    var sortColumnIndex = Convert.ToInt32(HttpContext.Request.QueryString["iSortCol_0"]);
        //    var sortDirection = HttpContext.Request.QueryString["sSortDir_0"]; // asc or desc

        //    Func<SpecialitiesViewModel, string> orderingFunction =
        //        c => sortColumnIndex == 1 ? c.Name
        //            : sortColumnIndex == 2 ? c.Description
        //            : sortColumnIndex == 4 ? c.CreatedDate
        //                : sortColumnIndex == 5 ? c.UpdatedDate
        //                 : c.Name;
        //    allSpecialitys = sortDirection == "asc" ? allSpecialitys.OrderBy(orderingFunction).ToList() : allSpecialitys.OrderByDescending(orderingFunction).ToList();

        //    var display = allSpecialitys.Skip(param.iDisplayStart).Take(param.iDisplayLength);
        //    var total = allSpecialitys.Count();

        //    return Json(new
        //    {
        //        param.sEcho,
        //        iTotalRecords = total,
        //        iTotalDisplayRecords = total,
        //        aaData = display
        //    }, JsonRequestBehavior.AllowGet);
        //}

        //[HttpGet, Route("_Specialitys/{id?}")]
        //public PartialViewResult _Specialitys(int id)
        //{

        //    if (id == 0) return PartialView(@"Partial/_Specialitys", new SpecialitiesViewModel());
        //    var Speciality = _repo.Find<Speciality>(id);
        //    var result = new SpecialitiesViewModel()
        //    {
        //        IsActive = Speciality.IsActive,
        //        Id = Speciality.SpecialityId,
        //        Name = Speciality.SpecialityName,
        //        Description = Speciality.Description
        //    };
        //    if (result == null) return PartialView(@"Partial/_Specialitys", new SpecialitiesViewModel());

        //    return PartialView(@"Partial/_Specialitys", result);
        //}
        //[HttpGet, Route("_ViewSpecialitys/{id?}")]
        //public PartialViewResult _ViewSpecialitys(int id)
        //{

        //    if (id == 0) return PartialView(@"Partial/_Specialitys", new SpecialitiesViewModel());
        //    var Speciality = _repo.Find<Speciality>(id);
        //    var result = new SpecialitiesViewModel()
        //    {
        //        IsActive = Speciality.IsActive,
        //        Id = Speciality.SpecialityId,
        //        Name = Speciality.SpecialityName,
        //        Description = Speciality.Description,
        //        IsViewMode = true
        //    };
        //    if (result == null) return PartialView(@"Partial/_Specialitys", new SpecialitiesViewModel());

        //    return PartialView(@"Partial/_Specialitys", result);
        //}

        //[HttpGet]
        //public JsonResult GetSpecialitysList(string query = "")
        //{
        //    var SpecialitysList = _repo.All<Speciality>().Where(x => x.IsActive && !x.IsDeleted)
        //        .Select(x => new KeyValuePairModel()
        //        {
        //            Id = x.SpecialityId.ToString(),
        //            Name = x.SpecialityName
        //        }).ToList();
        //    SpecialitysList = SpecialitysList.Where(x => x.Name.ToLower().Equals(query.ToLower())).ToList();
        //    return Json(new JsonResponse() { Status = 1, Data = SpecialitysList });
        //}

        //[Route("SpecialitysDetail/{npi?}")]
        //public ActionResult SpecialitysDetail(string npi)
        //{
        //    if (string.IsNullOrWhiteSpace(npi))
        //    {
        //        return RedirectToAction("Speciality", "User");
        //    }
        //    var Speciality = _repo.Find<Speciality>(x => x.SpecialityId.ToString() == npi);
        //    return View(Speciality);
        //}

        //[HttpPost, Route("AddEditSpeciality"), ValidateAntiForgeryToken]
        //public JsonResult AddEditSpeciality(SpecialitiesViewModel model)
        //{
        //    using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        //    {
        //        string filePath = string.Empty;
        //        try
        //        {
        //            if (ModelState.IsValid)
        //            {
        //                if (model.Id == 0)
        //                {
        //                    var bResult = _repo.Find<Speciality>(x => x.SpecialityName.ToLower().Equals(model.Name.ToLower()) && x.IsActive && !x.IsDeleted);
        //                    if (bResult != null)
        //                    {
        //                        txscope.Dispose();
        //                        return Json(new JsonResponse() { Status = 0, Message = "Speciality already exists. Could not add the data!" });
        //                    }

        //                    var Speciality = new Speciality()
        //                    {
        //                        SpecialityName = model.Name,
        //                        Description = model.Description,
        //                        SpecialityId = Convert.ToInt16(model.Id),
        //                        CreatedDate = DateTime.Now,
        //                        IsActive = true,
        //                        IsDeleted = false,
        //                    };
        //                    _repo.Insert<Speciality>(Speciality, true);
        //                    // _repo.sa();
        //                    txscope.Complete();
        //                    return Json(new JsonResponse() { Status = 1, Message = "Speciality save successfully" });
        //                }
        //                else
        //                {

        //                    var bResult = _repo.Find<Speciality>(x => x.SpecialityId != model.Id && x.SpecialityName == model.Name && x.IsActive && !x.IsDeleted);
        //                    if (bResult != null)
        //                    {
        //                        txscope.Dispose();
        //                        return Json(new JsonResponse { Status = 0, Message = "Speciality already exists. Could not add the data!" });
        //                    }

        //                    var oHour = _repo.Find<Speciality>(model.Id);
        //                    oHour.SpecialityName = model.Name;
        //                    oHour.Description = model.Description;
        //                    _repo.Update<Speciality>(oHour, true);
        //                    //_user.SaveData();
        //                    txscope.Complete();
        //                    return Json(new JsonResponse { Status = 1, Message = "Speciality updated successfully" });
        //                }
        //            }
        //            else
        //            {
        //                txscope.Dispose();
        //                return Json(new JsonResponse { Status = 4, Data = ModelState.Errors() });
        //            }
        //        }
        //        catch (Exception ex)
        //        {

        //            txscope.Dispose();
        //            Common.LogError(ex, "AddEditSpecialitys-Post");
        //            return Json(new JsonResponse { Status = 0, Message = "Something is wrong." });
        //        }
        //    }
        //}

        //[HttpPost, Route("RemoveSpecialitys/{id?}")]
        //public JsonResult RemoveSpecialitys(int id)
        //{
        //    var Speciality = _repo.Find<Speciality>(id);
        //    _repo.Delete<Speciality>(Speciality);

        //    // _user.SaveData();
        //    return Json(new JsonResponse() { Status = 1, Message = "Speciality remove successfully" });
        //}

        //[HttpPost, Route("ActiveDeActiveSpecialitys/{flag}/{id}")]
        //public JsonResult ActiveDeActiveSpecialitys(bool flag, int id)
        //{
        //    var Speciality = _repo.Find<Speciality>(id);
        //    Speciality.IsActive = !flag;
        //    _repo.Update<Speciality>(Speciality, true);
        //    // _user.SaveData();
        //    return Json(new JsonResponse() { Status = 1, Message = $@"The Speciality has been {(flag ? "deactivated" : "reactivated")} successfully" });
        //}
        //#endregion








    }
}
